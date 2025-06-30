/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using System.IO;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using MockDriver = TUGraz.VectoCore.Tests.Utils.MockDriver;
using MockDrivingCycle = TUGraz.VectoCore.Tests.Utils.MockDrivingCycle;

// ReSharper disable RedundantAssignment
// ReSharper disable UnusedVariable
// ReSharper disable InconsistentNaming

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class GearboxTest
	{
		private IKernel _kernel;

		public const string GearboxDataFile = @"TestData/Components/24t Coach.vgbx";
		public const string EngineDataFile = @"TestData/Components/24t Coach.veng";

		public const string CycleFile = @"TestData/Integration/FullPowerTrain/1-Gear-Test-dist.vdri";
		public const string CoachCycleFile = @"TestData/Integration/FullPowerTrain/Coach.vdri";
		public const string EngineFile = @"TestData/Components/24t Coach.veng";

		public const string AccelerationFile = @"TestData/Components/Coach.vacc";

		public const string IndirectLossMap = @"TestData/Components/Indirect Gear.vtlm";
		public const string DirectLossMap = @"TestData/Components/Direct Gear.vtlm";

		public const string GearboxShiftPolygonFile = @"TestData/Components/ShiftPolygons.vgbs";
		//public const string GearboxFullLoadCurveFile = @"TestData/Components/Gearbox.vfld";

		public const string AxleGearValidRangeDataFile = @"TestData/Components/AxleGearValidRange.vgbx";
		public const string AxleGearInvalidRangeDataFile = @"TestData/Components/AxleGearInvalidRange.vgbx";

		public const string AngledriveLossMap = @"TestData/Components/AngleGear.vtlm";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			_kernel = new StandardKernel(new VectoNinjectModule());
		}

		private static GearboxData CreateGearboxData()
		{
			var ratios = new[] { 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };

			return new GearboxData {
				Gears = ratios.Select((ratio, i) =>
					Tuple.Create((uint)i,
						new GearData {
//								MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap = TransmissionLossMapReader.ReadFromFile(i != 6 ? IndirectLossMap : DirectLossMap, ratio,
								$"Gear {i}"),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile)
						}))
					.ToDictionary(k => k.Item1 + 1, v => v.Item2),
				//ShiftTime = 2.SI<Second>(),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
				//DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				//UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				//UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
				//StartSpeed = 2.SI<MeterPerSecond>()
			};
		}

		protected PerSecond SpeedToAngularSpeed(double v, double r)
		{
			return ((60 * v) / (2 * r * Math.PI / 1000)).RPMtoRad();
		}

		[TestCase(520, 20.320, 279698.4, 9401.44062),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void AxleGearTest(double rdyn, double speed, double power, double expectedLoss)
		{
			var vehicle = VehicleContainer.CreateVehicleContainer(new VectoRunData(), null, null);
            var axleGearData = MockSimulationDataFactory.CreateAxleGearDataFromFile(GearboxDataFile);
			var axleGear = new AxleGear(vehicle, axleGearData);

			var mockPort = new MockTnOutPort();
			axleGear.InPort().Connect(mockPort);

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var angSpeed = SpeedToAngularSpeed(speed, rdyn);
			var PvD = power.SI<Watt>();
			var torqueToWheels = Formulas.PowerToTorque(PvD, angSpeed);

			axleGear.Request(absTime, dt, torqueToWheels, angSpeed, false);

			var loss = expectedLoss.SI<Watt>();

			Assert.AreEqual(Formulas.PowerToTorque(PvD + loss, angSpeed * axleGearData.AxleGear.Ratio).Value(),
				mockPort.Torque.Value(), 0.01, "Torque Engine Side");
			Assert.AreEqual((angSpeed * axleGearData.AxleGear.Ratio).Value(), mockPort.AngularVelocity.Value(), 0.01,
				"AngularVelocity Engine Side");
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void AxleGearValidRangeTest()
		{
			var vehicle = VehicleContainer.CreateVehicleContainer(new VectoRunData(), null, null);
            var axleGearData = MockSimulationDataFactory.CreateAxleGearDataFromFile(AxleGearValidRangeDataFile);
			var axleGear = new AxleGear(vehicle, axleGearData);
			Assert.AreEqual(0, axleGear.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false).Count);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void AxleGearInvalidRangeTest()
		{
			var vehicle = VehicleContainer.CreateVehicleContainer(new VectoRunData(), null, null);
            var axleGearData = MockSimulationDataFactory.CreateAxleGearDataFromFile(AxleGearInvalidRangeDataFile);
			var axleGear = new AxleGear(vehicle, axleGearData);
			var errors = axleGear.Validate(ExecutionMode.Declaration, VectoSimulationJobType.ConventionalVehicle, null, null, false);
			Assert.AreEqual(1, errors.Count);
		}

		[TestCase(520, 20.320, 279698.4, 9401.44062, 3.240355),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Angledrive_Losses(double rdyn, double speed, double power, double expectedLoss, double ratio)
		{
			// convert to SI
			var angSpeed = SpeedToAngularSpeed(speed, rdyn);
			var PvD = power.SI<Watt>();
			var torqueToWheels = PvD / angSpeed;
			var loss = expectedLoss.SI<Watt>();

			// setup components
			var vehicle = VehicleContainer.CreateVehicleContainer(new VectoRunData(), null, null);
            var angledriveData = new AngledriveData {
				Angledrive = new TransmissionData {
					LossMap = TransmissionLossMapReader.Create(VectoCSVFile.Read(AngledriveLossMap), ratio, "Angledrive"),
					Ratio = ratio
				}
			};
			var mockPort = new MockTnOutPort();
			var angledrive = new Angledrive(vehicle, angledriveData);
			angledrive.InPort().Connect(mockPort);

			// issue request
			angledrive.Request(0.SI<Second>(), 1.SI<Second>(), torqueToWheels, angSpeed);

			// test
			AssertHelper.AreRelativeEqual(angSpeed * angledriveData.Angledrive.Ratio, mockPort.AngularVelocity,
				message: "AngularVelocity Engine Side");

			AssertHelper.AreRelativeEqual((PvD + loss) / (angSpeed * angledriveData.Angledrive.Ratio), mockPort.Torque,
				message: "Torque Engine Side");
		}

		[TestCase(@"TestData/Components/24t Coach LessThanTwoGears.vgbx"),
			// this testcase checks the json readin method, not the data adapter, nor the gearbox itself
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void Gearbox_LessThanTwoGearsException(string wrongFile)
		{
			AssertHelper.Exception<VectoSimulationException>(
				() => MockSimulationDataFactory.CreateGearboxDataFromFile(wrongFile, EngineDataFile),
				"At least one Gear-Entry must be defined in Gearbox!");
		}

		[TestCase(GearboxDataFile, EngineDataFile, 6.38, 2300, 1600, 2356.2326),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -1300, 1000, -1267.0686),
		// the following entries are beyond the original loss map, but are not 'extrapolated' because the loss-map is extended on reading
		TestCase(GearboxDataFile, EngineDataFile, 6.38, 6300, 1600, 6437.86530),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -3300, 1000, -3227.8529411),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Gearbox_LossMapInterpolation(string gbxFile, string engineFile, double ratio, double torque,
			double inAngularSpeed, double expectedTorque)
		{
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(gbxFile, engineFile);
			var container = VehicleContainer.CreateVehicleContainer(GetDummyRunData(gearboxData), null, null);
			
			var gearbox = new AMTGearbox(container, new AMTShiftStrategy(container));
			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			//container.EngineInfo = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();
			var tq = torque.SI<NewtonMeter>();
			var n = inAngularSpeed.RPMtoRad();
			container.AbsTime = absTime;
			var response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, tq * ratio, n / ratio, false);

			Assert.IsFalse(gearbox.CurrentState.TorqueLossResult.Extrapolated);

			AssertHelper.AreRelativeEqual(absTime, port.AbsTime);
			AssertHelper.AreRelativeEqual(dt, port.Dt);
			AssertHelper.AreRelativeEqual(inAngularSpeed, port.AngularVelocity.Value() / Constants.RPMToRad);
			AssertHelper.AreRelativeEqual(expectedTorque, port.Torque.Value());

			Assert.IsFalse(gearbox.CurrentState.TorqueLossResult.Extrapolated);
			var modData = new MockModalDataContainer();
			gearbox.CommitSimulationStep(absTime, dt, modData);
		}

		private static VectoRunData GetDummyRunData(GearboxData gearboxData)
		{
			var fld = new[] {
				"560,1180,-149,0.6		   ",
				"600,1282,-148,0.6		   ",
				"799.9999999,1791,-149,0.6 ",
				"1000,2300,-160,0.6		   ",
				"1200,2300,-179,0.6		   ",
				"1400,2300,-203,0.6		   ",
				"1599.999999,2079,-235,0.49",
				"1800,1857,-264,0.25	   ",
				"2000.000001,1352,-301,0.25",
				"2100,1100,-320,0.25	   ",
			};
			var engineData = new CombustionEngineData() {
				IdleSpeed = 600.RPMtoRad(),
				Inertia = 0.SI<KilogramSquareMeter>(),
				EngineStartTime = 1.SI<Second>(),
			};
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
			fullLoadCurves[0] = FullLoadCurveReader.Create(
				VectoCSVFile.ReadStream(
					InputDataHelper.InputDataAsStream("engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]",
						fld)));
			fullLoadCurves[0].EngineData = engineData;
			foreach (var gears in gearboxData.Gears) {
				fullLoadCurves[gears.Key] = fullLoadCurves[0];
			}
			engineData.FullLoadCurves = fullLoadCurves;
			return new VectoRunData() {
				VehicleData = new VehicleData() {
					DynamicTyreRadius = 0.492.SI<Meter>(),
					CurbMass = 10000.SI<Kilogram>(),
					AxleData = new List<Axle>() {
						new Axle() {
							AxleWeightShare = 1,
							Inertia = 0.SI<KilogramSquareMeter>(),
							RollResistanceCoefficient = 0.0055,
							TyreTestLoad = 33500.SI<Newton>(),
						}
					}
				},
				AxleGearData = new AxleGearData() {
					AxleGear = new GearData() {
						Ratio = 2.64
					}
				},
				Retarder = new RetarderData() { Type = RetarderType.None },
				EngineData = engineData,
				GearboxData = gearboxData,
				GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = 2.SI<MeterPerSecond>(),
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
					TimeBetweenGearshifts = DeclarationData.Gearbox.MinTimeBetweenGearshifts,
					DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
					UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
					UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
				},
				Cycle = new DrivingCycleData() {
					CycleType = CycleType.EngineOnly
				},
				ExecutionMode = ExecutionMode.Engineering,
            };
		}

		[TestCase(GearboxDataFile, EngineDataFile, 6.38, 96000, 1600, 96499.10109),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -96000, 1000, -95502.403188),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Gearbox_LossMapExtrapolation_Declaration(string gbxFile, string engineFile, double ratio, double torque,
			double inAngularSpeed, double expectedTorque)
		{
			// read gearbox data in engineering mode so that the loss-map is not extrapolated.
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(gbxFile, engineFile, false);
			var container = VehicleContainer.CreateVehicleContainer(GetDummyRunData(gearboxData), null, null);
            
			var gearbox = new AMTGearbox(container, new AMTShiftStrategy(container));
			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			//container.EngineInfo = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();
			var tq = torque.SI<NewtonMeter>();
			var n = inAngularSpeed.RPMtoRad();
			container.AbsTime = absTime;
			var response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, tq * ratio, n / ratio, false);

			Assert.IsTrue( gearbox.CurrentState.TorqueLossResult.Extrapolated);
			Assert.AreEqual(absTime, port.AbsTime);
			Assert.AreEqual(dt, port.Dt);
			Assert.AreEqual(inAngularSpeed, port.AngularVelocity.Value() / Constants.RPMToRad, 1e-3);
			AssertHelper.AreRelativeEqual(expectedTorque.SI<NewtonMeter>(), port.Torque, 1e-2);

			var modData = new MockModalDataContainer();
			Assert.IsTrue(gearbox.CurrentState.TorqueLossResult.Extrapolated);
			AssertHelper.Exception<VectoException>(() => { gearbox.CommitSimulationStep(absTime, dt, modData); });
		}

		[TestCase(GearboxDataFile, EngineDataFile, 6.38, 96000, 1600, 96499.10109),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -96000, 1000, -95502.403188),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Gearbox_LossMapExtrapolation_Engineering(string gbxFile, string engineFile, double ratio, double torque,
			double inAngularSpeed, double expectedTorque)
		{
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile, false);
			var container = VehicleContainer.CreateVehicleContainer(GetDummyRunData(gearboxData), null, null);
            
			var gearbox = new AMTGearbox(container, new AMTShiftStrategy(container));
			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			//container.EngineInfo = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();
			var t = torque.SI<NewtonMeter>();
			var n = inAngularSpeed.RPMtoRad();
			container.AbsTime = absTime;

			var response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, t * ratio, n / ratio, false);

			Assert.IsTrue(gearbox.CurrentState.TorqueLossResult.Extrapolated);

			Assert.AreEqual(absTime, port.AbsTime);
			Assert.AreEqual(dt, port.Dt);
			Assert.AreEqual(n, port.AngularVelocity);
			AssertHelper.AreRelativeEqual(expectedTorque.SI<NewtonMeter>(), port.Torque, 1e-2);

			var modData = new MockModalDataContainer();
			Assert.IsTrue(gearbox.CurrentState.TorqueLossResult.Extrapolated);
			gearbox.CommitSimulationStep(absTime, dt, modData);
		}

		[TestCase(GearboxDataFile, EngineDataFile, 6.38, 96000, 1600, true, 96499.10109),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -2500, 1000, false, -2443.5392),
		TestCase(GearboxDataFile, EngineDataFile, 6.38, -3000, 1000, false, -2933.73529),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Gearbox_LossMapExtrapolation_DryRun(string gbxFile, string engineFile, double ratio, double torque,
			double inAngularSpeed, bool extrapolated, double expectedTorque)
		{
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile, false);
			var container = VehicleContainer.CreateVehicleContainer(GetDummyRunData(gearboxData), null, null);
            var gearbox = new AMTGearbox(container, new AMTShiftStrategy(container));
			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			//container.EngineInfo = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();
			var t = torque.SI<NewtonMeter>();
			var n = inAngularSpeed.RPMtoRad();
			container.AbsTime = absTime;
			var response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, t * ratio, n / ratio, false);

			Assert.AreEqual(absTime, port.AbsTime);
			Assert.AreEqual(dt, port.Dt);
			Assert.AreEqual(n, port.AngularVelocity);
			Assert.AreEqual(extrapolated, gearbox.CurrentState.TorqueLossResult.Extrapolated);
			AssertHelper.AreRelativeEqual(expectedTorque.SI<NewtonMeter>(), port.Torque, 1e-2);

			var modData = new MockModalDataContainer();
			gearbox.CommitSimulationStep(absTime, dt, modData);
		}

		[TestCase,
		Category(Definitions.DEPRECATED)] // does not test intersection of full-load curves
		public void Gearbox_IntersectFullLoadCurves()
		{
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);
			var container = VehicleContainer.CreateVehicleContainer(GetDummyRunData(gearboxData), null, null);
            var gearbox = new AMTGearbox(container, new AMTShiftStrategy( container));
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };
			var driver = new MockDriver(container);

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			//container.EngineInfo = port;

			gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

			const double ratio = 6.38;

			var expected = new[] {
				new { t = 2500, n = 900 },
				new { t = 2500, n = 1700 },
				new { t = 2129, n = 1600 }, // to high for gearbox, but ok for engine
			};

			foreach (var exp in expected) {
				var torque = exp.t.SI<NewtonMeter>() * ratio;
				var angularVelocity = exp.n.RPMtoRad() / ratio;
				var response = (ResponseSuccess)gearbox.OutPort().Request(0.SI<Second>(), 1.SI<Second>(), torque, angularVelocity, false);
			}

			var expectedCorrect = new[] {
				new { t = 500, n = 700 },
				new { t = 1500, n = 1100 },
				new { t = 2000, n = 1500 },
				new { t = 2240, n = 1200 } // ok for gearbox but would be to high for engine
			};

			foreach (var exp in expectedCorrect) {
				var torque = exp.t.SI<NewtonMeter>() * ratio;
				var angularVelocity = exp.n.RPMtoRad() / ratio;

				var response = (ResponseSuccess)gearbox.OutPort().Request(0.SI<Second>(), 1.SI<Second>(), torque, angularVelocity, false);
			}
		}

		[TestCase(2, -1000, 600, 28.096, typeof(ResponseSuccess)),
		TestCase(2, -1000, 600, 28.096, typeof(ResponseSuccess)),
		TestCase(2, 50, 600, 9.096, typeof(ResponseSuccess)),
		TestCase(1, 2450, 800, 58.11, typeof(ResponseSuccess)),
		TestCase(2, 850, 800, 26.11, typeof(ResponseSuccess)),
		TestCase(2, 850, 200, 23.07, typeof(ResponseSuccess)),
		TestCase(2, 50, 600, 9.096, typeof(ResponseSuccess)),
		TestCase(2, 2050, 1200, 52.132, typeof(ResponseSuccess)),
		TestCase(2, 850, 600, 25.096, typeof(ResponseSuccess)),
		TestCase(1, 850, 0, 0, typeof(ResponseSuccess)), // torque loss is 0 if speed is 0
		Category(Definitions.TESTCASE_MIGRATED)
		]
		public void Gearbox_Request_engaged(int gear, double t, double n, double loss, Type responseType)
		{
			
			var gearboxData = CreateGearboxData();
			var container = VehicleContainer.CreateVehicleContainer(GetDummyRunData(gearboxData), null, null);
            //runData.VehicleData.DynamicTyreRadius = 0.3.SI<Meter>();
            container.RunData.AxleGearData.AxleGear.Ratio = 5;
			var gearbox = new AMTGearbox(container, new AMTShiftStrategy(container));

			var driver = new MockDriver(container);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 0.KMPHtoMeterPerSecond() };

			var port = new MockTnOutPort();
			gearbox.InPort().Connect(port);
			//container.EngineInfo = port;

			var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
			// the first element 0.0 is just a placeholder for axlegear, not used in this test

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();

			var torque = t.SI<NewtonMeter>();
			var expectedN = n.RPMtoRad();
			var expectedLoss = loss.SI<NewtonMeter>();

			var outTorque = (torque - expectedLoss) * ratios[gear];
			var angularVelocity = expectedN / ratios[gear];

			gearbox.OutPort().Initialize(outTorque, angularVelocity);

			Assert.AreEqual(gear, gearbox.Gear.Gear);

			gearbox.Gear = new GearshiftPosition((uint)gear);
			container.AbsTime = absTime;
			var response = gearbox.OutPort().Request(absTime, dt, outTorque, angularVelocity, false);
			Assert.IsTrue(response.GetType() == responseType);

			AssertHelper.AreRelativeEqual(absTime, port.AbsTime);
			AssertHelper.AreRelativeEqual(dt, port.Dt);
			AssertHelper.AreRelativeEqual(expectedN, port.AngularVelocity);
			AssertHelper.AreRelativeEqual(t, port.Torque, toleranceFactor: 1e-5);
		}

		[TestCase(8, 7, 1800, 750, typeof(ResponseGearShift)),
		TestCase(7, 6, 1800, 750, typeof(ResponseGearShift)),
		TestCase(6, 5, 1800, 750, typeof(ResponseGearShift)),
		TestCase(5, 4, 1800, 750, typeof(ResponseGearShift)),
		TestCase(4, 3, 1800, 750, typeof(ResponseGearShift)),
		TestCase(3, 2, 1800, 750, typeof(ResponseGearShift)),
		TestCase(2, 1, 1900, 750, typeof(ResponseGearShift)),
		TestCase(1, 1, 1200, 700, typeof(ResponseSuccess)),
		TestCase(8, 4, 15000, 200, typeof(ResponseGearShift)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Gearbox_ShiftDown_ACEA_Shiftlines(int gear, int newGear, double t, double n, Type responseType)
		{
			
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);

			var gearboxInput = JSONInputDataFactory.ReadGearbox(GearboxDataFile);
			var engineInput = JSONInputDataFactory.ReadEngine(EngineDataFile);
			var dao = new DeclarationDataAdapterHeavyLorry.Conventional();
			var engineData = dao.CreateEngineData(new MockDeclarationVehicleInputData() {
				EngineInputData = engineInput,
				GearboxInputData = gearboxInput
			}, engineInput.EngineModes.First(), new Mission() {
					MissionType = MissionType.LongHaul
				});
			foreach (var entry in gearboxData.Gears) {
				entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(
					(int)(entry.Key - 1), engineData.FullLoadCurves.First().Value,
					gearboxInput.Gears, engineData, ((IAxleGearInputData)gearboxInput).Ratio, 0.5.SI<Meter>());
			}

			var container = VehicleContainer.CreateVehicleContainer(GetDummyRunData(gearboxData), null, null);
            var gearbox = new AMTGearbox(container, new AMTShiftStrategy(container));
			var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);
			var cycle = new MockDrivingCycle(container, cycleData);
			container.RunData.Cycle = cycleData ;

			var driver = new MockDriver(container);
			var port = new MockTnOutPort() { EngineN95hSpeed = 2000.RPMtoRad() };
			gearbox.InPort().Connect(port);
			var vehicle = new MockVehicle(container) { MyVehicleSpeed = 10.SI<MeterPerSecond>() };
			//container.EngineInfo = port;

			var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
			// the first element 0.0 is just a placeholder for axlegear, not used in this test

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();

			var expectedN = n.RPMtoRad();
			var angularVelocity = expectedN / ratios[gear];
			gearbox.OutPort().Initialize(1.SI<NewtonMeter>(), angularVelocity);

			var expectedT = t.SI<NewtonMeter>();
			

			var torque = expectedT * ratios[gear];
			

			gearbox.Gear = new GearshiftPosition((uint)gear);
			container.AbsTime = absTime;
			var gearShiftResponse = gearbox.OutPort().Request(absTime, dt, torque, angularVelocity, false);
			Assert.IsTrue(gearShiftResponse.GetType() == responseType);

			absTime += dt;
			var successResponse = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, torque, angularVelocity, false);
			Assert.AreEqual((uint)newGear, container.GearboxInfo.Gear.Gear);
		}

		[TestCase(7, 8, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(6, 8, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(5, 6, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(4, 5, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(3, 4, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(2, 4, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(1, 2, 1000, 1400, typeof(ResponseGearShift)),
		TestCase(8, 8, 1000, 1400, typeof(ResponseSuccess)),
		TestCase(1, 6, 200, 9000, typeof(ResponseGearShift)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Gearbox_ShiftUp(int gear, int newGear, double tq, double n, Type responseType)
		{
			var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile);

			var gearboxInput = JSONInputDataFactory.ReadGearbox(GearboxDataFile);
			var engineInput = JSONInputDataFactory.ReadEngine(EngineDataFile);
			var dao = new DeclarationDataAdapterHeavyLorry.Conventional();
			var engineData = dao.CreateEngineData(new MockDeclarationVehicleInputData() {
				EngineInputData = engineInput,
				GearboxInputData = gearboxInput
			}, engineInput.EngineModes.First(), new Mission() {
				MissionType = MissionType.LongHaul
			});
			foreach (var entry in gearboxData.Gears) {
				entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(
					(int)(entry.Key - 1), engineData.FullLoadCurves.First().Value,
					gearboxInput.Gears, engineData, ((IAxleGearInputData)gearboxInput).Ratio, 0.5.SI<Meter>());
			}

			var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);
			var container = new MockVehicleContainer() {
				VehicleSpeed = 10.SI<MeterPerSecond>(),
				DriverBehavior = DrivingBehavior.Driving,
				DrivingAction = DrivingAction.Accelerate,
				Altitude = 0.SI<Meter>(),
				VehicleMass = 10000.SI<Kilogram>(),
				ReducedMassWheels = 100.SI<Kilogram>(),
				TotalMass = 19000.SI<Kilogram>(),
				EngineSpeed = n.SI<PerSecond>(),
				RunData = GetDummyRunData(gearboxData),
				CycleData = new CycleData() {
					LeftSample = cycleData.Entries.First(),
				},
				ElectricMotorPositions = new PowertrainPosition[]{},
				HasCombustionEngine = true,
			};
			var cycle = new MockDrivingCycle(container, cycleData);
			
			var gearbox = new AMTGearbox(container, new AMTShiftStrategy(container));
			var port = new MockTnOutPort() { EngineN95hSpeed = 2000.RPMtoRad() };
			container.EngineInfo = port;
			gearbox.InPort().Connect(port);

			var ratios = new[] { 0.0, 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };
			// the first element 0.0 is just a placeholder for axlegear, not used in this test

			var absTime = 0.SI<Second>();
			var dt = 2.SI<Second>();

			var expectedN = n.RPMtoRad();
			var angularVelocity = expectedN / ratios[gear];
			gearbox.OutPort().Initialize(1.SI<NewtonMeter>(), angularVelocity);

			absTime += dt;

			var expectedT = tq.SI<NewtonMeter>();
			

			var torque = expectedT * ratios[gear];
			

			gearbox.Gear = new GearshiftPosition((uint)gear);
			container.AbsTime = absTime;
			var response = gearbox.OutPort().Request(absTime, dt, torque, angularVelocity, false);
			Assert.IsTrue(response.GetType() == responseType);

			absTime += dt;
			response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, torque, angularVelocity, false);
			Assert.AreEqual((uint)newGear, gearbox.Gear.Gear);
		}
	}
}