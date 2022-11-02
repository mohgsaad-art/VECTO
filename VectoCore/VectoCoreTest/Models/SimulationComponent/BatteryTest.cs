using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Battery = TUGraz.VectoCore.Models.SimulationComponent.Impl.Battery;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class BatteryTest
	{
		public const string componentFile = @"TestData\Hybrids\Battery\GenericBattery.vbat";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[
		TestCase(0.5, 0.5, -500, 0.499985523),
		TestCase(0.5, 1, -14000, 0.499175514),
		TestCase(0.5, 1, 7000, 0.5004016980),
		TestCase(0.35, 0.5, -200, 0.349994175),
		TestCase(0.35, 0.5, -16000, 0.3495245545),
		TestCase(0.75, 0.5, -300, 0.7499913702),
		TestCase(0.75, 0.5, -14500, 0.7495755113),
			]

		public void BatteryRequestTest(double initialSoC, double simInterval, double powerDemand, double expectedSoC)
		{
			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false) ;
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);
			var modData = new MockModalDataContainer();
			bat.Initialize(initialSoC);

			//Assert.AreEqual(3.2*200, bat.Voltage.Value());

			var absTime = 0.SI<Second>();
			var dt = simInterval.SI<Second>();
			var response = bat.Request(absTime, dt, powerDemand.SI<Watt>());
			Assert.IsInstanceOf<RESSResponseSuccess>(response);
			bat.CommitSimulationStep(absTime, dt, modData);

			Assert.AreEqual(expectedSoC, bat.StateOfCharge, 1e-9);
		}

		[TestCase(0.5, 0.5, -500000, -169875, 70125),
		 TestCase(0.35, 0.5, -500000, -168375, 70125),
		 TestCase(0.75, 0.5, -500000, -171375, 70125),
		 TestCase(0.2001, 3, -500000, -563.00328, 0.40392),
		 TestCase(0.2001, 1, -500000, -1686.58632, 3.63528),]
		public void BatteryOverloadTestNoAux(double initialSoC, double dt, double powerDemand,
			double maxPowerDischarge, double battLoss)
		{

			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);

			bat.Initialize(initialSoC);

			var response = bat.Request(0.SI<Second>(), dt.SI<Second>(), powerDemand.SI<Watt>());
			Assert.IsInstanceOf<RESSUnderloadResponse>(response);

			Assert.AreEqual(maxPowerDischarge, response.MaxDischargePower.Value(), 1e-2);
			Assert.AreEqual(battLoss, response.LossPower.Value(), 1e-2);
			Assert.AreEqual(powerDemand, response.PowerDemand.Value(), 1e-2);
		}

		[TestCase(0.5, 0.5, -500000, 10000, -169875, 70125),
		 TestCase(0.35, 0.5, -500000, 10000, -168375, 70125),
		 TestCase(0.75, 0.5, -500000, 10000, -171375, 70125),
		 TestCase(0.2001, 3, -500000, 10000, -563.00328, 0.40392),
		 TestCase(0.2001, 1, -500000, 10000, -1686.58632, 3.63528),]
		public void BatteryOverloadTestAux(double initialSoC, double dt, double powerDemand, double auxPower,
			double maxPowerDischarge, double battLoss)
		{

			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var es = new ElectricSystem(container);
			var bat = new Battery(container, batteryData.Batteries.First().Item2);
			es.Connect(bat);
			es.Connect(new MockElectricConsumer(auxPower.SI<Watt>()));
			bat.Initialize(initialSoC);

			var response = es.Request(0.SI<Second>(), dt.SI<Second>(), powerDemand.SI<Watt>());
			Assert.IsInstanceOf<ElectricSystemUnderloadResponse>(response);

			Assert.AreEqual(maxPowerDischarge, response.RESSResponse.MaxDischargePower.Value(), 1e-2);
			Assert.AreEqual(battLoss, response.RESSResponse.LossPower.Value(), 1e-2);
			Assert.AreEqual(auxPower, response.AuxPower.Value(), 1e-2);
			Assert.AreEqual(powerDemand - auxPower, response.RESSResponse.PowerDemand.Value(), 1e-2);
		}

		[TestCase(0.5, 0.5, 500000, 310125, 70125),
		 TestCase(0.35, 0.5, 500000, 308625, 70125),
		 TestCase(0.75, 0.5, 500000, 311625, 70125),
		 TestCase(0.2001, 3, 500000, 304878, 70125),
		 TestCase(0.7999, 1, 500000, 1747.82448, 3.63528),
		 TestCase(0.7999, 3, 500000, 581.80032, 0.40392)]
		public void BatteryUnderloadTestNoAux(double initialSoC, double dt, double powerDemand,
			double maxPowerDischarge, double battLoss)
		{

			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);

			bat.Initialize(initialSoC);

			var response = bat.Request(0.SI<Second>(), dt.SI<Second>(), powerDemand.SI<Watt>());
			Assert.IsInstanceOf<RESSOverloadResponse>(response);

			Assert.AreEqual(maxPowerDischarge, response.MaxChargePower.Value(), 1e-2);
			Assert.AreEqual(battLoss, response.LossPower.Value(), 1e-2);
			Assert.AreEqual(powerDemand, response.PowerDemand.Value(), 1e-2);
		}

		[TestCase(0.5, 0.5, 500000, 10000, 310125, 70125),
		 TestCase(0.35, 0.5, 500000, 10000, 308625, 70125),
		 TestCase(0.75, 0.5, 500000, 10000, 311625, 70125),
		 TestCase(0.2001, 3, 500000, 10000, 304878, 70125),
		 TestCase(0.7999, 1, 500000, 10000, 1747.82448, 3.63528),
		 TestCase(0.7999, 3, 500000, 10000, 581.80032, 0.40392)]
		public void BatteryUnderloadTestAux(double initialSoC, double dt, double powerDemand, double auxPower,
			double maxPowerDischarge, double battLoss)
		{

			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);
			var es = new ElectricSystem(container);
			es.Connect(bat);
			es.Connect(new MockElectricConsumer(auxPower.SI<Watt>()));
			bat.Initialize(initialSoC);

			var response = es.Request(0.SI<Second>(), dt.SI<Second>(), powerDemand.SI<Watt>());
			Assert.IsInstanceOf<ElectricSystemOverloadResponse>(response);

			Assert.AreEqual(maxPowerDischarge, response.RESSResponse.MaxChargePower.Value(), 1e-2);
			Assert.AreEqual(battLoss, response.RESSResponse.LossPower.Value(), 1e-2);
			Assert.AreEqual(auxPower, response.AuxPower.Value(), 1e-2);
			Assert.AreEqual(powerDemand - auxPower, response.RESSResponse.PowerDemand.Value(), 1e-2);
		}


		public const double REESS_Capacity = 4.5;
		public const double REESS_MinSoC = 0.2;
		public const double REESS_MaxSoC = 0.8;

		[TestCase(0.5, 0.5, 5000),
		TestCase(0.5, 0.5, -5000)]
		public void BatteryTimeDependentInternalResistanceTest_ConstantLoad(double initialSoC, double dt, double powerDemand)
		{
			var r1 = 0.02;
			var r2 = 0.04;
			var r3 = 0.1;

			var batteryData = new BatterySystemData() {
				Batteries = new List<Tuple<int, BatteryData>>() {
					Tuple.Create(0, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					})
				}
			};

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);
			var es = new ElectricSystem(container);
			es.Connect(bat);
			es.Connect(new MockElectricConsumer(0.SI<Watt>()));
			bat.Initialize(initialSoC);

			var modData = new MockModalDataContainer();
			
			var absTime = 0.SI<Second>();

			var i = 0;
			for (; i < 5; i++) { // constant for the first 2 sec
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				Assert.AreEqual(r1, rREESS.Value(), 1e-9, $"{i} / {absTime}");
				
				absTime += dt.SI<Second>();
			}

			for (; i < 21; i++) { // linear increase for the next 8s to 0.04
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				var slope = (r2 - r1) / (10 - 2);
				var r = slope * absTime.Value() + r1 - slope * 2;
				Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 41; i++) { // linear increase for the next 10s to 0.1
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				var slope = (r3 - r2) / (20 - 10);
				var r = slope * absTime.Value() + r2 - slope * 10;
				Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 100; i++) { // constant after 20 sec
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				Assert.AreEqual(r3, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}
		}


		[TestCase(0.5, 0.5, 5000)]
		public void BatteryTimeDependentInternalResistanceTest_120s(double initialSoC, double dt,
			double powerDemand)
		{
			var r1 = 0.02;
			var r2 = 0.04;
			var r3 = 0.1;
			var r4 = 0.15;

			var batteryData = new BatterySystemData() {
				Batteries = new List<Tuple<int, BatteryData>>() {
					Tuple.Create(0, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create(
								$"SoC, Ri-2, Ri-10, Ri-20, Ri-120\n0, {r1}, {r2}, {r3},{r4}\n100, {r1}, {r2}, {r3}, {r4}".ToStream(),
								false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					})
				}
			};
		}

		[TestCase(0.5, 0.5, 5000),
		TestCase(0.5, 0.5, -5000)]
		public void BatteryTimeDependentInternalResistanceTest_LoadChanges(double initialSoC, double dt, double powerDemand)
		{
			var r1 = 0.02;
			var r2 = 0.04;
			var r3 = 0.1;

			var batteryData = new BatterySystemData() {
				Batteries = new List<Tuple<int, BatteryData>>() {
					Tuple.Create(0, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					})
				}
			};

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);
			var es = new ElectricSystem(container);
			es.Connect(bat);
			es.Connect(new MockElectricConsumer(0.SI<Watt>()));
			bat.Initialize(initialSoC);

			var modData = new MockModalDataContainer();

			var absTime = 0.SI<Second>();

			var i = 0;
			for (; i < 11; i++) { // constant for the first 5 sec
				var response = es.Request(absTime, dt.SI<Second>(), -Math.Sign(powerDemand) * Math.Abs(powerDemand).SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				absTime += dt.SI<Second>();
			}

			for (; i < 11 + 5; i++) { // constant for the first 2 sec
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				Assert.AreEqual(r1, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 10 + 21; i++) { // linear increase for the next 8s to 0.04
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				var slope = (r2 - r1) / (10 - 2);
				var r = slope * (absTime.Value() - 5.5) + r1 - slope * 2;
				Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 10 + 41; i++) { // linear increase for the next 10s to 0.1
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				var slope = (r3 - r2) / (20 - 10);
				var r = slope * (absTime.Value() - 5.5) + r2 - slope * 10;
				Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 100; i++) { // constant after 20 sec
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				Assert.AreEqual(r3, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}
		}


		[TestCase(0.5, 0.5, 5000),
		TestCase(0.5, 0.5, -5000)]
		public void BatterySystemTimeDependentInternalResistanceTest_ConstantLoad(double initialSoC, double dt, double powerDemand)
		{
			var r1 = 0.02;
			var r2 = 0.04;
			var r3 = 0.1;

			var batteryData = new BatterySystemData() {
				Batteries = new List<Tuple<int, BatteryData>>() {
					Tuple.Create(0, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					}),
					Tuple.Create(0, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					}),
					Tuple.Create(1, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					}),
					Tuple.Create(1, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					})
				}
			};

			var container = new MockVehicleContainer();
			var bat = new BatterySystem(container, batteryData);
			var es = new ElectricSystem(container);
			es.Connect(bat);
			es.Connect(new MockElectricConsumer(0.SI<Watt>()));
			bat.Initialize(initialSoC);

			var modData = new MockModalDataContainer();

			var absTime = 0.SI<Second>();

			var i = 0;
			for (; i < 5; i++) { // constant for the first 2 sec
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				Assert.AreEqual(r1, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 21; i++) { // linear increase for the next 8s to 0.04
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				var slope = (r2 - r1) / (10 - 2);
				var r = slope * absTime.Value() + r1 - slope * 2;
				Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 41; i++) { // linear increase for the next 10s to 0.1
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				var slope = (r3 - r2) / (20 - 10);
				var r = slope * absTime.Value() + r2 - slope * 10;
				Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 100; i++) { // constant after 20 sec
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				Assert.AreEqual(r3, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}
		}

		[TestCase(0.5, 0.5, 5000),
		TestCase(0.5, 0.5, -5000)]
		public void BatterySystemTimeDependentInternalResistanceTest_LoadChanges(double initialSoC, double dt, double powerDemand)
		{
			var r1 = 0.02;
			var r2 = 0.04;
			var r3 = 0.1;

			var batteryData = new BatterySystemData() {
				Batteries = new List<Tuple<int, BatteryData>>() {
					Tuple.Create(0, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					}),
					Tuple.Create(0, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					}),
					Tuple.Create(1, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					}),
					Tuple.Create(1, new BatteryData() {
						Capacity = REESS_Capacity.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						MinSOC = REESS_MinSoC,
						MaxSOC = REESS_MaxSoC,
						SOCMap = BatterySOCReader.Create("SOC,V\n0,590\n100,658".ToStream()),
						InternalResistance =
							BatteryInternalResistanceReader.Create($"SoC, Ri-2, Ri-10, Ri-20\n0, {r1}, {r2}, {r3}\n100, {r1}, {r2}, {r3}".ToStream(), false),
						MaxCurrent = BatteryMaxCurrentReader.Create(
							"SOC, I_charge, I_discharge\n0, 375, 573\n100, 375, 375".ToStream()),
					})
				}
			};

			var container = new MockVehicleContainer();
			var bat = new BatterySystem(container, batteryData);
			var es = new ElectricSystem(container);
			es.Connect(bat);
			es.Connect(new MockElectricConsumer(0.SI<Watt>()));
			bat.Initialize(initialSoC);

			var modData = new MockModalDataContainer();

			var absTime = 0.SI<Second>();

			var i = 0;

			for (; i < 11; i++) { // constant for the first 2 sec
				var response = es.Request(absTime, dt.SI<Second>(), -Math.Sign(powerDemand) * Math.Abs(powerDemand).SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				absTime += dt.SI<Second>();
			}
			
			for (; i < 11 + 5; i++) { // constant for the first 2 sec
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				Assert.AreEqual(r1, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 11 + 21; i++) { // linear increase for the next 8s to 0.04
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				var slope = (r2 - r1) / (10 - 2);
				var r = slope * (absTime.Value() - 5.5) + r1 - slope * 2;
				Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 11 + 41; i++) { // linear increase for the next 10s to 0.1
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				var slope = (r3 - r2) / (20 - 10);
				var r = slope * (absTime.Value() - 5.5)  + r2 - slope * 10;
				Assert.AreEqual(r, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}

			for (; i < 11 + 100; i++) { // constant after 20 sec
				var response = es.Request(absTime, dt.SI<Second>(), powerDemand.SI<Watt>());
				Assert.IsInstanceOf<ElectricSystemResponseSuccess>(response);
				bat.CommitSimulationStep(absTime, dt.SI<Second>(), modData);

				var current = (Ampere)modData[ModalResultField.I_reess];
				var rREESS = (Watt)modData[ModalResultField.P_reess_loss] / current / current;
				Assert.AreEqual(r3, rREESS.Value(), 1e-9, $"{i} / {absTime}");

				absTime += dt.SI<Second>();
			}
		}
	}
}