using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class BatterySystemTest
	{
		public const string componentFile = @"TestData/Hybrids/Battery/GenericBattery.vbat";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(0.5, 1, 7000, new[] { 0.50020169, 0.50020169, 0.50020169 }),
		TestCase(0.35, 1, 14000, new[] { 0.350404181979, 0.350404181979, 0.350404181979 }),
		TestCase(0.75, 1, -14500, new[] { 0.7495793463, 0.7495793463, 0.7495793463 }),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestBatterySystemRequest_2Serial(double initialSoC, double simInterval, double powerDemand, double[] expectedSoC)
		{
			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData() {
				_electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = inputData,
						StringId = 0,
						Count = 2
					}
				}
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new BatterySystem(container, batteryData);
			var modData = new MockModalDataContainer();
			bat.Initialize(initialSoC);

			var absTime = 0.SI<Second>();
			var dt = simInterval.SI<Second>();
			var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
			Assert.IsInstanceOf<RESSResponseSuccess>(response);
			bat.CommitSimulationStep(absTime, dt, modData);

			var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
			for (var i = 0; i < socs.Length; i++) {
				Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
			}

			Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
		}

		[TestCase(0.5, 1, 7000, new[] { 0.50020169, 0.50020169, 0.50020169 }),
		TestCase(0.35, 1, 14000, new[] { 0.350404181979, 0.350404181979, 0.350404181979 }),
		TestCase(0.75, 1, -14500, new[] { 0.7495793463, 0.7495793463, 0.7495793463 }),
			Category(Definitions.TESTCASE_MIGRATED)

		]
		public void TestBatterySystemRequest_2Parallel(double initialSoC, double simInterval, double powerDemand, double[] expectedSoC)
		{
			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData() {
				_electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = inputData,
						StringId = 0,
						Count = 1
					},
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = inputData,
						StringId = 1,
						Count = 1
					}
				}
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new BatterySystem(container, batteryData);
			var modData = new MockModalDataContainer();
			bat.Initialize(initialSoC);

			var absTime = 0.SI<Second>();
			var dt = simInterval.SI<Second>();
			var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
			Assert.IsInstanceOf<RESSResponseSuccess>(response);
			bat.CommitSimulationStep(absTime, dt, modData);

			var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
			for (var i = 0; i < socs.Length; i++) {
				Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
			}

			Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
		}

		[TestCase(0.5, 1, 60000, new[] {10.0, 20, 30}, new[] { 0.500693147221155, 0.500692716960012, 0.500693147221155, 0.5006930038 }),
        TestCase(0.35, 1, 14000, new[] { 2.33333333, 4.666666666, 7 }, new[] { 0.350161966207795, 0.350161942625547, 0.350161966207795, 0.3501619583470 }),
        TestCase(0.75, 1, -14500, new[] { -2.4166666666, -4.833333333, -7.25 },  new[] { 0.749832099811648, 0.749832074409512, 0.749832099811648, 0.74983209134426 }),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestBatterySystemRequest_3Strings_1_Balanced(double initialSoC, double simInterval, double powerDemand, double[] expectedPowerDemand, double[] expectedSoC)
		{
			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData() {
				_electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(10.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							400.SI<Volt>(), 500.SI<Ampere>(), 0.03.SI<Ohm>()),
						StringId = 0,
						Count = 1
					},
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(20.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							400.SI<Volt>(), 500.SI<Ampere>(), 0.02.SI<Ohm>()),
						StringId = 1,
						Count = 1
					},
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(30.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							400.SI<Volt>(), 500.SI<Ampere>(), 0.01.SI<Ohm>()),
						StringId = 2,
						Count = 1
					}

				}
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new BatterySystem(container, batteryData);
			var modData = new MockModalDataContainer();
			bat.Initialize(initialSoC);

			var absTime = 0.SI<Second>();
			var dt = simInterval.SI<Second>();
			var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
			Assert.IsInstanceOf<RESSResponseSuccess>(response);
			bat.CommitSimulationStep(absTime, dt, modData);

			for (var i = 0; i < 3; i++) {
				Assert.AreEqual(expectedPowerDemand[i] * 1e3, ((SI)modData[ModalResultField.P_reess_terminal, i]).Value(), 1e-3);
			}

			var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
			Console.WriteLine(socs.Join());
			for (var i = 0; i < socs.Length; i++) {
				Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
			}

			Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
		}

		[
			TestCase(new[] {0.48, 0.52, 0.5}, 1, 60000, new[] { 10.463576, 19.337748, 30.198675 }, new[] { 0.480725217195313, 0.520669834354267, 0.500697728984787, 0.5040263454 }),
			TestCase(new[] { 0.48, 0.52, 0.5 }, 1, -60000, new[] { -9.536424, -20.662252, -29.801325 }, new[] { 0.479336559932017, 0.519280698083512, 0.499308864822235, 0.502637425094291 }),
			TestCase(new[] { 0.48, 0.52, 0.5 }, 10, 60000, new[] { 10.463576, 19.337748, 30.198675 }, new[] { 0.487252171953127, 0.526698343542669, 0.506977289847874, 0.510263454763681 }),
			TestCase(new[] { 0.48, 0.52, 0.5 }, 10, -60000, new[] { -9.536424, -20.662252, -29.801325 }, new[] { 0.473365599320166, 0.512806980835123, 0.493088648222347, 0.496374250942909 }),
		//TestCase(0.35, 1, 14000, new[] { 2.33333333, 4.666666666, 7 }, new[] { 0.350161966207795, 0.350161942625547, 0.350161966207795, 0.3501619583470 }),
		//TestCase(0.75, 1, -14500, new[] { -2.4166666666, -4.833333333, -7.25 }, new[] { 0.749832099811648, 0.749832074409512, 0.749832099811648, 0.74983209134426 })
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestBatterySystemRequest_3Strings_1_Unbalanced(double[] initialSoC, double simInterval, double powerDemand, double[] expectedPowerDemand, double[] expectedSoC)
		{
			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData() {
				_electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(10.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							400.SI<Volt>(), 500.SI<Ampere>(), 0.03.SI<Ohm>()),
						StringId = 0,
						Count = 1
					},
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(20.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							400.SI<Volt>(), 500.SI<Ampere>(), 0.02.SI<Ohm>()),
						StringId = 1,
						Count = 1
					},
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(30.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							400.SI<Volt>(), 500.SI<Ampere>(), 0.01.SI<Ohm>()),
						StringId = 2,
						Count = 1
					}

				}
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new BatterySystem(container, batteryData);
			var modData = new MockModalDataContainer();
			for (var i = 0; i < bat.Batteries.Count; i++) {
				foreach (var b in bat.Batteries[i].Batteries) {
					b.Initialize(initialSoC[i]);
				}
			}
			//bat.Initialize(initialSoC);

			var absTime = 0.SI<Second>();
			var dt = simInterval.SI<Second>();
			var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
			Assert.IsInstanceOf<RESSResponseSuccess>(response);
			bat.CommitSimulationStep(absTime, dt, modData);

			for (var i = 0; i < 3; i++) {
				Assert.AreEqual(expectedPowerDemand[i] * 1e3, ((SI)modData[ModalResultField.P_reess_terminal, i]).Value(), 1e-3);
			}

			var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
			Console.WriteLine(socs.Join());
			Console.WriteLine(bat.StateOfCharge);
			for (var i = 0; i < socs.Length; i++) {
				Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
			}

			Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
		}


		[
			TestCase(0.5, 1, 60000, new[] { 3.74922167, 6.25077833, 20, 30 }, new[] { 0.500693147221155, 0.500462098147437, 0.500692716960012, 0.500693147221155, 0.5006930038 }),
			TestCase(0.5, 1, -60000, new[] { -3.75078419, -6.24921581, -20, -30 }, new[] { 0.499304248566401, 0.499536165710934, 0.499303810709253, 0.499304248566401, 0.499304102614018 }),
		//TestCase(0.35, 1, 14000, new[] { 2.33333333, 4.666666666, 7 }, new[] { 0.350161966207795, 0.350161942625547, 0.350161966207795, 0.3501619583470 }),
		//TestCase(0.75, 1, -14500, new[] { -2.4166666666, -4.833333333, -7.25 }, new[] { 0.749832099811648, 0.749832074409512, 0.749832099811648, 0.74983209134426 })
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestBatterySystemRequest_3Strings_2_Balanced(double initialSoC, double simInterval, double powerDemand, double[] expectedPowerDemand, double[] expectedSoC)
		{
			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData() {
				_electricStorageElements = new List<IElectricStorageDeclarationInputData>() {
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(10.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							150.SI<Volt>(), 200.SI<Ampere>(), 0.01.SI<Ohm>()),
						StringId = 0,
						Count = 1
					},
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(15.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							250.SI<Volt>(), 300.SI<Ampere>(), 0.02.SI<Ohm>()),
						StringId = 0,
						Count = 1
					},
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(20.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							400.SI<Volt>(), 500.SI<Ampere>(), 0.02.SI<Ohm>()),
						StringId = 1,
						Count = 1
					},
					new MockBatteryInputData.MockElectricStorageInputWrapper() {
						REESSPack = new MockREESSInputData(30.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
							400.SI<Volt>(), 500.SI<Ampere>(), 0.01.SI<Ohm>()),
						StringId = 2,
						Count = 1
					}

				}
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new BatterySystem(container, batteryData);
			var modData = new MockModalDataContainer();
			bat.Initialize(initialSoC);

			var absTime = 0.SI<Second>();
			var dt = simInterval.SI<Second>();
			var response = bat.Request(absTime, dt, powerDemand.SI<Watt>(), false);
			Assert.IsInstanceOf<RESSResponseSuccess>(response);
			bat.CommitSimulationStep(absTime, dt, modData);

			for (var i = 0; i < 3; i++) {
				Assert.AreEqual(expectedPowerDemand[i] * 1e3, ((SI)modData[ModalResultField.P_reess_terminal, i]).Value(), 1e-3);
			}

			var socs = bat.Batteries.SelectMany(x => x.Value.Batteries.Select(y => y.StateOfCharge)).ToArray();
			Console.WriteLine(socs.Join());
			Console.WriteLine(bat.StateOfCharge);
			for (var i = 0; i < socs.Length; i++) {
				Assert.AreEqual(expectedSoC[i], socs[i], 1e-9, $"Bat_{i} SoC");
			}

			Assert.AreEqual(expectedSoC.Last(), bat.StateOfCharge, 1e-9);
		}
	}

	public class MockREESSInputData : IBatteryPackEngineeringInputData
	{
		private Volt _voltage;
		private Ampere _maxCurrent;
		private Ohm _internalResistance;

		public MockREESSInputData(AmpereSecond capacity, Volt ocv, Ampere maxCurrent, Ohm internalResistance)
		{
			Capacity = capacity;
			_voltage = ocv;
			_maxCurrent = maxCurrent;
			_internalResistance = internalResistance;
		}

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }
		public string Manufacturer { get; }
		public string Model { get; }
		public DateTime Date { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get; }
		public string CertificationNumber { get; }
		public DigestData DigestValue { get; }

		#endregion

		#region Implementation of IREESSPackInputData

		public REESSType StorageType => REESSType.Battery;

		#endregion

		#region Implementation of IBatteryPackDeclarationInputData

		public double? MinSOC => 0.2;
		public double? MaxSOC => 0.8;

        public double? DeteriorationPerformanceRatio { get; }

        public BatteryType BatteryType { get; }
		public AmpereSecond Capacity { get; }
		public bool? ConnectorsSubsystemsIncluded { get; }
		public bool? JunctionboxIncluded { get; }
		public Kelvin TestingTemperature => null;

		public TableData InternalResistanceCurve => VectoCSVFile.ReadStream(
			InputDataHelper.InputDataAsStream("SoC, Ri",
				new[] { $"0, {_internalResistance.Value()}", $"100, {_internalResistance.Value()}" }));

		public TableData VoltageCurve => VectoCSVFile.ReadStream(
			InputDataHelper.InputDataAsStream("SOC, V", 
				new[] { $"0, {_voltage.Value()}", $"100, {_voltage.Value()}" }));

		public TableData MaxCurrentMap => VectoCSVFile.ReadStream(
			InputDataHelper.InputDataAsStream("SOC, I_charge, I_discharge",
				new[] {
					$"0, {_maxCurrent.Value()}, {_maxCurrent.Value()}",
					$"100, {_maxCurrent.Value()}, {_maxCurrent.Value()}"
				}));

		#endregion
	}
}