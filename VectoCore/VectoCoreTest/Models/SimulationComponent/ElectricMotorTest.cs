using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ElectricMotorTest
	{
		public const string MotorFile = @"TestData/Hybrids/ElectricMotor/GenericEMotor.vem";
		public const string MotorFile_v2 = @"TestData/Hybrids/ElectricMotor/GenericEMotorV2.vem";
		public const string BatFile = @"TestData/Hybrids/Battery/GenericBattery.vbat";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(1),
		TestCase(2),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void ElectricMotorModelDataTest(int count)
		{
			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData() {
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = count,
						RatioADC = 1,
						MechanicalTransmissionEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine, null, null);
			var emModelData = data.First().Item2;

			Assert.AreEqual(0.15 * count, emModelData.Inertia.Value(), 1e-3);

			//Assert.AreEqual(2000, emModelData.ContinuousTorqueSpeed.AsRPM, 1e-3);
			Assert.AreEqual(238.7323 * count, emModelData.Overload.ContinuousTorque.Value(), 1e-3);

			Assert.AreEqual(334.23 * count, -emModelData.EfficiencyData.VoltageLevels.First().FullLoadCurve.FullLoadDriveTorque(2000.RPMtoRad()).Value(), 1e-3);
			Assert.AreEqual(-334.23 * count, -emModelData.EfficiencyData.VoltageLevels.First().FullLoadCurve.FullGenerationTorque(2000.RPMtoRad()).Value(), 1e-3);

			Assert.AreEqual(30 * count, emModelData.DragCurveLookup(2500.RPMtoRad(), 0u).Value(), 1e-3);

			Assert.AreEqual(-14579 * count,
				emModelData.EfficiencyData.VoltageLevels.First()
					.LookupElectricPower(190.99.RPMtoRad(), (-500 * count).SI<NewtonMeter>(), 0, false).ElectricalPower.Value(),
				1e-3);


		}

		[TestCase(100, 100, -1484.401151),
		 TestCase(100, 30, -498.336701),
		 TestCase(100, 300, -5058.393920),
		 TestCase(600, 100, -7292.591952),
		 TestCase(600, 300, -21459.016866),
		 TestCase(800, -100, 7174.730264),
		 TestCase(800, -300, 22354.108093),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ElectricMotorOnlyRequestTest(double speed, double torque, double expectedBatteryPower)
		{
			var container = new MockVehicleContainer();
			container.Gear = new GearshiftPosition(0);

			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData() {
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						RatioADC = 1,
						MechanicalTransmissionEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine, null, null);
			var strategy = new MockHybridControl();

			var battery = new MockBattery();
			container.BatteryInfo = battery;
			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			var es = new ElectricSystem(container, new BatterySystemData());
			es.Connect(battery);
			motor.Connect(es);

			strategy.ElectricShare = -torque.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var response = motor.Request(0.SI<Second>(), 0.5.SI<Second>(), torque.SI<NewtonMeter>(), speed.RPMtoRad(), false);

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var enginePower = speed.RPMtoRad() * strategy.ElectricShare;
			Assert.AreEqual(0, response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(enginePower.Value(), response.ElectricMotor.ElectricMotorPowerMech.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.RESSResponse.PowerDemand.Value(), 1e-6);
			Assert.IsTrue(response.ElectricSystem.ConsumerPower.Value() < enginePower.Value());
		}

		[TestCase(100, 100, -1566.457317),
		 TestCase(100, 30, -519.553356),
		 TestCase(100, 300, -5395.304809),
		 TestCase(600, 100, -7653.267447),
		 TestCase(600, 300, -22631.653148),
		 TestCase(800, -100, 6785.258050),
		 TestCase(800, -300, 21273.378603),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ElectricMotorOnlyRequestTestMechLoss(double speed, double torque, double expectedBatteryPower)
		{
			var container = new MockVehicleContainer();
			container.Gear = new GearshiftPosition(0);
            var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData() {
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						RatioADC = 1,
						MechanicalTransmissionEfficiency= 0.95
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine, null, null);
			var strategy = new MockHybridControl();

			var battery = new MockBattery();
			container.BatteryInfo = battery;
			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			var es = new ElectricSystem(container, new BatterySystemData());
			es.Connect(battery);
			motor.Connect(es);

			strategy.ElectricShare = -torque.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var response = motor.Request(0.SI<Second>(), 0.5.SI<Second>(), torque.SI<NewtonMeter>(), speed.RPMtoRad(), false);

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var enginePower = speed.RPMtoRad() * strategy.ElectricShare;
			Assert.AreEqual(0, response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(enginePower.Value(), response.ElectricMotor.ElectricMotorPowerMech.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.RESSResponse.PowerDemand.Value(), 1e-6);
			Assert.IsTrue(response.ElectricSystem.ConsumerPower.Value() < enginePower.Value());
		}

		[TestCase(100, 100, -30, -498.33670),
		TestCase(100, 300, -150, -2273.504629),
		TestCase(600, 100, 100, 5367.264248),
		TestCase(600, 300, -50, -3925.642046),
		TestCase(800, -100, 200, 14907.629627),
		TestCase(800, -300, 200, 14907.629627),
		Category(Definitions.TESTCASE_MIGRATED),]
		public void ElectricMotorAssistingRequestTest(double speed, double torque, double electricTorque, double expectedBatteryPower)
		{
			var container = new MockVehicleContainer();

			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData()
			{
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						RatioADC = 1,
						MechanicalTransmissionEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine, null, null);
			var strategy = new MockHybridControl();

			var battery = new MockBattery();
			container.BatteryInfo = battery;

			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			var es = new ElectricSystem(container, new BatterySystemData());
			es.Connect(battery);
			motor.Connect(es);
			var tnPort = new MockTnOutPort();
			motor.Connect(tnPort);

			strategy.ElectricShare = electricTorque.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var response = motor.Request(0.SI<Second>(), 0.5.SI<Second>(), torque.SI<NewtonMeter>(), speed.RPMtoRad(), false);

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var enginePower = speed.RPMtoRad() * (torque + electricTorque).SI<NewtonMeter>();
			var motorMechPower = speed.RPMtoRad() * electricTorque.SI<NewtonMeter>();
			Assert.AreEqual(enginePower.Value(), response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(motorMechPower, response.ElectricMotor.ElectricMotorPowerMech);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.RESSResponse.PowerDemand.Value(), 1e-6);
			Assert.IsTrue(response.ElectricSystem.ConsumerPower.Value() < response.ElectricMotor.ElectricMotorPowerMech.Value());
		}

		[TestCase(100, 100, -30, -519.553356),
		TestCase(100, 300, -150, -2407.931677),
		TestCase(600, 100, 100, 5075.160087),
		TestCase(600, 300, -50, -4102.198874),
		TestCase(800, -100, 200, 14165.993213),
		TestCase(800, -300, 200, 14165.993213),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ElectricMotorAssistingRequestTestMechLoss(double speed, double torque, double electricTorque, double expectedBatteryPower)
		{
			var container = new MockVehicleContainer();

			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData() {
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						RatioADC = 1,
						MechanicalTransmissionEfficiency = 0.95
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine, null, null);
			var strategy = new MockHybridControl();

			var battery = new MockBattery();
			container.BatteryInfo = battery;
			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			var es = new ElectricSystem(container, new BatterySystemData());
			es.Connect(battery);
			motor.Connect(es);
			var tnPort = new MockTnOutPort();
			motor.Connect(tnPort);

			strategy.ElectricShare = electricTorque.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var response = motor.Request(0.SI<Second>(), 0.5.SI<Second>(), torque.SI<NewtonMeter>(), speed.RPMtoRad(), false);

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var enginePower = speed.RPMtoRad() * (torque + electricTorque).SI<NewtonMeter>();
			var motorMechPower = speed.RPMtoRad() * electricTorque.SI<NewtonMeter>();
			Assert.AreEqual(enginePower.Value(), response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(motorMechPower, response.ElectricMotor.ElectricMotorPowerMech);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.RESSResponse.PowerDemand.Value(), 1e-6);
			Assert.IsTrue(response.ElectricSystem.ConsumerPower.Value() < response.ElectricMotor.ElectricMotorPowerMech.Value());
		}

		[TestCase(800, 300),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ElectricMotorWithBatteryIdlingRequestTest(double speed, double torque)
		{
			var container = new MockVehicleContainer();

			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData()
			{
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						RatioADC = 1,
						MechanicalTransmissionEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine, null, null);
			var strategy = new MockHybridControl();

			var batInput = JSONInputDataFactory.ReadREESSData(BatFile, false);
			var tmp = new MockBatteryInputData() {
				REESSPack = batInput,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);
			var battery = new Battery(container, batteryData.Batteries.First().Item2);
			var es = new ElectricSystem(container, batteryData);
			es.Connect(battery);

			container.BatteryInfo = battery;

			battery.Initialize(0.5);
			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			motor.Connect(es);
			var tnPort = new MockTnOutPort();
			motor.Connect(tnPort);

			strategy.ElectricShare = null; //0.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var absTime = 0.SI<Second>();
			var dt = 0.5.SI<Second>();
			var response = motor.Request(absTime, dt, torque.SI<NewtonMeter>(), speed.RPMtoRad(), false);

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var dragTorque = data.First().Item2.DragCurveLookup(speed.RPMtoRad(), 0u);
			var enginePower = speed.RPMtoRad() * (torque.SI<NewtonMeter>() + dragTorque);
			var motorMechPower = dragTorque * speed.RPMtoRad();
			Assert.AreEqual(enginePower.Value(), response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(motorMechPower.Value(), response.ElectricMotor.ElectricMotorPowerMech.Value(), 1e-6);
			Assert.AreEqual(0, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(0, response.ElectricSystem.RESSResponse.PowerDemand.Value(), 1e-6);
			var modData = new MockModalDataContainer();
			battery.CommitSimulationStep(absTime, dt, modData);
		}

		[TestCase(0.5, 100, 100, -1484.401151, 2.69232),
		TestCase(0.5, 100, 300, -5058.393920, 31.54095),
		TestCase(0.5, 600, 100, -7292.591952, 65.92202),
		TestCase(0.5, 600, 300, -21459.016866, 591.97964),
		TestCase(0.5, 800, -100, 7174.730264, 61.59876),
		TestCase(0.5, 800, -300, 22354.108093, 577.346990),
		Category(Definitions.TESTCASE_MIGRATED)
        ]
		public void ElectricMotorOnlyWithBatteryRequestTest(double initialSoc, double speed, double torque, double expectedBatteryPower, double expectedBatteryLoss)
		{
			var container = new MockVehicleContainer();
			container.Gear = new GearshiftPosition(0);
            var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile, false);
			var batInput = JSONInputDataFactory.ReadREESSData(BatFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData()
			{
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Count = 1,
						RatioADC = 1,
						MechanicalTransmissionEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine, null, null);
			var strategy = new MockHybridControl();

			var tmp = new MockBatteryInputData()
			{
				REESSPack = batInput,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);
			var battery = new Battery(container, batteryData.Batteries.First().Item2);
			container.BatteryInfo = battery;
			var es = new ElectricSystem(container, batteryData);
			es.Connect(battery);
			battery.Initialize(initialSoc);
			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			motor.Connect(es);

			strategy.ElectricShare = -torque.SI<NewtonMeter>();
			motor.Initialize(0.SI<NewtonMeter>(), speed.RPMtoRad());

			var response = motor.Request(0.SI<Second>(), 0.5.SI<Second>(), torque.SI<NewtonMeter>(), speed.RPMtoRad(), false);

			Assert.IsInstanceOf<ResponseSuccess>(response);
			var enginePower = speed.RPMtoRad() * strategy.ElectricShare;
			Assert.AreEqual(0, response.Engine.PowerRequest.Value(), 1e-6);
			Assert.AreEqual(enginePower.Value(), response.ElectricMotor.ElectricMotorPowerMech.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.ConsumerPower.Value(), 1e-6);
			Assert.AreEqual(expectedBatteryPower, response.ElectricSystem.RESSResponse.PowerDemand.Value(), 1e-6);
			Assert.IsTrue(response.ElectricSystem.ConsumerPower.Value() < enginePower.Value());
			Assert.AreEqual(expectedBatteryLoss, response.ElectricSystem.RESSResponse.LossPower.Value(), 1e-4);
		}


		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestElectricMotorThermalDeRating()
		{
			var initialSoc = 0.8;
			var torque = 300.SI<NewtonMeter>();
			var continuousTorque = 24.SI<NewtonMeter>();
			var speed = 2000.RPMtoRad();

			var inputData = JSONInputDataFactory.ReadElectricMotorData(MotorFile_v2, false);
			var batInput = JSONInputDataFactory.ReadREESSData(BatFile, false);
			var dao = new EngineeringDataAdapter();
			var electricMachine = new MockElectricMachinesInputData()
			{
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						ElectricMachine = inputData,
						Position = PowertrainPosition.HybridP2,
						Count = 1,
						RatioADC = 1,
						MechanicalTransmissionEfficiency = 1
					}
				}
			};
			var data = dao.CreateElectricMachines(electricMachine, null, null);
			var strategy = new MockHybridControl();

			var tmp = new MockBatteryInputData()
			{
				REESSPack = batInput,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var runData = new VectoRunData() {
				JobName = "EM-Derating",
				ElectricMachinesData = data
			};

			var kernel = new StandardKernel(new VectoNinjectModule());
			var modData = kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, new FileOutputWriter("debug.csv"), null, null) as ModalDataContainer;
			Assert.NotNull(modData);
			modData.WriteModalResults = true;

			//modData.AddElectricMotor(PowertrainPosition.HybridP2);
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
			
			var container = VehicleContainer.CreateVehicleContainer(null, modData, null);
			new EngineOnlyGearboxInfo(container);

			var battery = new Battery(container, batteryData.Batteries.First().Item2);
			var es = new ElectricSystem(container, batteryData);
			es.Connect(battery);
			battery.Initialize(initialSoc);

			var motor = new ElectricMotor(container, data.First().Item2, strategy, PowertrainPosition.HybridP2);
			motor.Connect(es);

			
			strategy.ElectricShare = -torque;
			motor.Initialize(0.SI<NewtonMeter>(), speed);

			var i = 0;

			var t1 = 20;
			var t2 = 9;
			var t3 = 20;

			Assert.AreEqual(19008.29074, data.First().Item2.Overload.OverloadBuffer.Value(), 1e-3);
			Assert.AreEqual(100, data.First().Item2.Overload.ContinuousTorque.Value(), 1e-3);
			Assert.AreEqual(3687.46233, data.First().Item2.Overload.ContinuousPowerLoss.Value(), 1e-3);

			try {
				// energy buffer is empty - overload is available
				for (; i < t1; i++) {
					var dt = 0.5.SI<Second>();
					var absTime = i * dt;

					var response = motor.Request(absTime, dt, torque, speed, false);
					Assert.AreEqual(-334.23, response.ElectricMotor.MaxDriveTorque.Value(), 1e-2, $"{i}");
					motor.CommitSimulationStep(absTime, dt, modData);
					modData[ModalResultField.time] = absTime;
					modData.CommitSimulationStep();
				}

				// energy buffer reached - only P_cont is available

				strategy.ElectricShare = -continuousTorque;
				for (; i < t1 + t2; i++) {
					var dt = 0.5.SI<Second>();
					var absTime = i * dt;

					var response = motor.Request(absTime, dt, continuousTorque, speed, false);
					Assert.AreEqual(-100, response.ElectricMotor.MaxDriveTorque.Value(), 1e-2, $"{i}");
					motor.CommitSimulationStep(absTime, dt, modData);
					modData[ModalResultField.time] = absTime;
					modData.CommitSimulationStep();
				}


				// energy buffer below lower threshold - overload is available again
				for (; i < t1 + t2 + t3; i++) {
					var dt = 0.5.SI<Second>();
					var absTime = i * dt;

					var response = motor.Request(absTime, dt, torque * 0.5, speed, false);
					Assert.AreEqual(-334.23, response.ElectricMotor.MaxDriveTorque.Value(), 1e-2, $"{i}");
					motor.CommitSimulationStep(absTime, dt, modData);
					modData[ModalResultField.time] = absTime;
					modData.CommitSimulationStep();
				}
			} finally {
				modData.WriteModalResults = true;
				modData.Finish(VectoRun.Status.Aborted);
			}
		}

		[TestCase(0.95, 1000, 1000, 1052.63157894),   // case EM drag: EM torque is lower than DT torque
		 TestCase(0.95, 1000, -1000, -950), // case EM drive: DT torque is lower than EM torque
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestADCEfficiencyMapLookupFWD(double eff, double emSpeed, double emTorque, double expectedDTTorque)
		{
			var lossMap = TransmissionLossMapReader.CreateEmADCLossMap(eff, 1.0, "EM ADC Eff");

			var outTorque = lossMap.GetOutTorque(emSpeed.RPMtoRad(), emTorque.SI<NewtonMeter>());

			Assert.AreEqual(expectedDTTorque, outTorque.Value(), 1e-6);

			var effValue = emTorque < 0 ? outTorque / emTorque : emTorque / outTorque;

			Assert.AreEqual(eff, effValue.Value(), 1e-6);
		}

		[TestCase(0.95, 1000, 1000, 950),   // case EM drag: EM torque is lower than DT torque
		TestCase(0.95, 1000, -1000, -1052.63157894), // case EM drive: DT torque is lower than EM torque
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestADCEfficiencyMapLookupBWD(double eff, double dtSpeed, double dtTorque, double expectedEMTorque)
		{
			var lossMap = TransmissionLossMapReader.CreateEmADCLossMap(eff, 1.0, "EM ADC Eff");

			var torqueLoss = lossMap.GetTorqueLoss(dtSpeed.RPMtoRad(), dtTorque.SI<NewtonMeter>());

			var emTorque = dtTorque.SI<NewtonMeter>() + torqueLoss.Value;

			Assert.AreEqual(expectedEMTorque, emTorque.Value(), 1e-6);

			
		}

		[TestCase(1000, 1000, 1050),
		TestCase(1000, -1000, -950),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestADCEfficiencyMapLookupFWD(double emSpeed, double emTorque, double expectedDTTorque)
		{
			var header = "n, T_in, T_loss";
			var mapData = new [] {
				"0, -100000, 5000",
				"0, 0, 0",
				"0, 100000, 5000",
				"10000, -100000, 5000",
				"10000, 0, 0",
				"10000, 100000, 5000"
			};

			var lossMap =
				TransmissionLossMapReader.CreateEmADCLossMap(VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream(header, mapData)), 1.0,
					"EM ADC Map", false);

			var outTorque = lossMap.GetOutTorque(emSpeed.RPMtoRad(), emTorque.SI<NewtonMeter>());

			Assert.AreEqual(expectedDTTorque, outTorque.Value(), 1e-6);
		}

		[TestCase(1000, 1000, 952.380952),
		TestCase(1000, -1000, -1052.6315789), 
		Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestADCEfficiencyMapLookupBWD(double dtSpeed, double dtTorque, double expectedEMTorque)
		{
			var header = "n, T_in, T_loss";
			var mapData = new[] {
				"0, -100000, 5000",
				"0, 0, 0",
				"0, 100000, 5000",
				"10000, -100000, 5000",
				"10000, 0, 0",
				"10000, 100000, 5000"
			};

			var lossMap =
				TransmissionLossMapReader.CreateEmADCLossMap(VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream(header, mapData)), 1.0,
					"EM ADC Map", false);

			var torqueLoss = lossMap.GetTorqueLoss(dtSpeed.RPMtoRad(), dtTorque.SI<NewtonMeter>());

			var emTorque = dtTorque.SI<NewtonMeter>() + torqueLoss.Value;

			Assert.AreEqual(expectedEMTorque, emTorque.Value(), 1e-6);
		}
	}

}