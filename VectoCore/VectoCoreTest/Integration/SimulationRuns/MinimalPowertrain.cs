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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;
using NUnit.Framework;
using System.IO;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Tests.Models.SimulationComponent;

namespace TUGraz.VectoCore.Tests.Integration.SimulationRuns
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class MinimalPowertrain
	{
		public const string CycleFile = @"TestData\Integration\MinimalPowerTrain\1-Gear-Test-dist.vdri";
		public const string CycleFileStop = @"TestData\Integration\MinimalPowerTrain\1-Gear-StopTest-dist.vdri";
		public const string EngineFile = @"TestData\Integration\MinimalPowerTrain\24t Coach.veng";
		public const string GearboxFile = @"TestData\Integration\MinimalPowerTrain\24t Coach-1Gear.vgbx";
		public const string GbxLossMap = @"TestData\Integration\MinimalPowerTrain\NoLossGbxMap.vtlm";
		public const string AccelerationFile = @"TestData\Components\Coach.vacc";
		public const string AccelerationFile2 = @"TestData\Components\Truck.vacc";
		public const double Tolerance = 0.001;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void TestWheelsAndEngineInitialize()
		{
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile, 1);

			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());

			var axleGearData = CreateAxleGearData();

			var driverData = CreateDriverData(AccelerationFile);

			var fileWriter = new FileOutputWriter("Coach_MinimalPowertrainOverload");
			var runData = new VectoRunData() {
				JobName = "Coach_MinimalPowertrainOverload"
			};
			var modData = new ModalDataContainer(runData, fileWriter, null);
			var container = new VehicleContainer(ExecutionMode.Engineering, modData) {
				RunData = new VectoRunData() {
					VehicleData = vehicleData,
					DriverData = driverData
				}
			};

			var driver = new Driver(container, driverData, new DefaultDriverStrategy(container));
			var engine = new CombustionEngine(container, engineData);
			driver.AddComponent(new Vehicle(container, vehicleData, CreateAirdragData()))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new Clutch(container, engineData))
				.AddComponent(engine);

			var gbx = new MockGearbox(container);
			// ReSharper disable once ObjectCreationAsStatement
			new DummyCycle(container);
			var driverPort = driver.OutPort();

			gbx.Gear = new GearshiftPosition(1);

			var response = driverPort.Initialize(18.KMPHtoMeterPerSecond(),
				VectoMath.InclinationToAngle(2.842372037 / 100));

			Assert.IsInstanceOf<ResponseSuccess>(response);

			//			time [s] , dist [m] , v_act [km/h] , v_targ [km/h] , acc [m/s²] , grad [%] , n_eng_avg [1/min] , T_eng_fcmap [Nm] , Tq_clutch [Nm] , Tq_full [Nm] , Tq_drag [Nm] , P_eng_out [kW] , P_eng_full [kW] , P_eng_drag [kW] , P_clutch_out [kW] , Pa Eng [kW] , P_aux_mech [kW] , Gear [-] , Ploss GB [kW] , Ploss Diff [kW] , Ploss Retarder [kW] , Pa GB [kW] , Pa Veh [kW] , P_roll [kW] , P_air [kW] , P_slope [kW] , P_wheel_in [kW] , P_brake_loss [kW] , FC-Map [g/h] , FC-AUXc [g/h] , FC-WHTCc [g/h]
			//			1.5      , 5        , 18           , 18            , 0          , 2.842372 , 964.1117  , 323.7562    , 323.7562       , 2208.664     , -158.0261    , 32.68693    , 222.9902     , -15.95456    , 32.68693       , 0           , 0         , 1        , 0             , 0               , 0                   , 0          , 0           , 5.965827   , 0.2423075 , 26.47879   , 32.68693    , 0           , 7574.113     , -             , -

			AssertHelper.AreRelativeEqual(964.1117.RPMtoRad().Value(), container.EngineInfo.EngineSpeed.Value());
			Assert.AreEqual(2208.664, engine.PreviousState.StationaryFullLoadTorque.Value(), Tolerance);
			Assert.AreEqual(-158.0261, engine.PreviousState.FullDragTorque.Value(), Tolerance);

			Assert.AreEqual(323.6485, engine.PreviousState.EngineTorque.Value(), Tolerance);
		}

		[TestCase, Category("LongRunning")]
		public void TestWheelsAndEngine()
		{
			NLog.LogManager.DisableLogging();
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile, 1);
			var cycleData = DrivingCycleDataReader.ReadFromFile(CycleFile, CycleType.DistanceBased, false);

			var axleGearData = CreateAxleGearData();

			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());

			var driverData = CreateDriverData(AccelerationFile);

			var fileWriter = new FileOutputWriter("Coach_MinimalPowertrain");
			var runData = new VectoRunData() {
				JobName = "Coach_MinimalPowertrain",
				VehicleData = vehicleData,
				EngineData = engineData,
				AxleGearData = axleGearData,
				DriverData = driverData,
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				}

			};
			var modData = new ModalDataContainer(runData, fileWriter, null);
			var container = new VehicleContainer(ExecutionMode.Engineering, modData) {
				RunData = runData
			};

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			cycle.AddComponent(new Driver(container, driverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, vehicleData, CreateAirdragData()))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new Clutch(container, engineData))
				.AddComponent(new CombustionEngine(container, engineData));
			//engine.IdleController.RequestPort = clutch.IdleControlPort;

			var gbx = new MockGearbox(container) { Gear = new GearshiftPosition(0) };

			var cyclePort = cycle.OutPort();
			cyclePort.Initialize();
			gbx.Gear = new GearshiftPosition(0);

			var absTime = 0.SI<Second>();
			var ds = Constants.SimulationSettings.DriveOffDistance;
			var response = cyclePort.Request(absTime, ds);
			Assert.IsInstanceOf<ResponseSuccess>(response);
			container.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			gbx.Gear = new GearshiftPosition(1);
			var cnt = 0;
			while (!(response is ResponseCycleFinished) && container.MileageCounter.Distance < 17000) {
				response = cyclePort.Request(absTime, ds);
				switch (response) {
					case ResponseCycleFinished _:
						break;
					case ResponseDrivingCycleDistanceExceeded r:
						ds = r.MaxDistance;
						break;
					case ResponseSuccess r:
						container.CommitSimulationStep(absTime, r.SimulationInterval);
						absTime += r.SimulationInterval;
						ds = container.VehicleInfo.VehicleSpeed.IsEqual(0)
							? Constants.SimulationSettings.DriveOffDistance
							: (Constants.SimulationSettings.TargetTimeInterval * container.VehicleInfo.VehicleSpeed).Cast<Meter>();
						if (cnt++ % 100 == 0) {
							modData.Finish(VectoRun.Status.Success);
						}
						break;
					default:
						Assert.Fail("Unexpected Response: {0}", response);
						break;
				}
			}

			Assert.IsInstanceOf<ResponseCycleFinished>(response);
			modData.Finish(VectoRun.Status.Success);
			NLog.LogManager.EnableLogging();
		}

		[TestCase]
		public void TestWheelsAndEngineLookahead()
		{
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile, 1);
			var cycleData = DrivingCycleDataReader.ReadFromFile(CycleFileStop, CycleType.DistanceBased, false);

			var axleGearData = CreateAxleGearData();

			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());

			var driverData = CreateDriverData(AccelerationFile2);

			var fileWriter = new FileOutputWriter("Coach_MinimalPowertrainOverload");
			var runData = new VectoRunData() {
				JobName = "Coach_MinimalPowertrain",
				SimulationType = SimulationType.DistanceCycle,
				VehicleData = vehicleData,
				EngineData = engineData,
				AxleGearData = axleGearData,
				DriverData = driverData,
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>()
			};
			var modData = new ModalDataContainer(runData, fileWriter, null);
			var container = new VehicleContainer(ExecutionMode.Engineering, modData) {
				RunData = runData
			};

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);
			cycle.AddComponent(new Driver(container, driverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, vehicleData, CreateAirdragData()))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new Clutch(container, engineData))
				.AddComponent(new CombustionEngine(container, engineData));
			//engine.IdleController.RequestPort = clutch.IdleControlPort;

			var gbx = new MockGearbox(container);
			gbx.Gear = new GearshiftPosition(0);

			var cyclePort = cycle.OutPort();

			cyclePort.Initialize();

			gbx.Gear = new GearshiftPosition(0);

			var absTime = 0.SI<Second>();

			gbx.Gear = new GearshiftPosition(1);
			var ds = Constants.SimulationSettings.DriveOffDistance;
			while (container.MileageCounter.Distance < 100) {
				var response = cyclePort.Request(absTime, ds);
				switch (response) {
					case ResponseDrivingCycleDistanceExceeded r:
						ds = r.MaxDistance;
						break;
					case ResponseSuccess r:
						container.CommitSimulationStep(absTime, r.SimulationInterval);
						absTime += r.SimulationInterval;
						ds = container.VehicleInfo.VehicleSpeed.IsEqual(0)
							? Constants.SimulationSettings.DriveOffDistance
							: (Constants.SimulationSettings.TargetTimeInterval * container.VehicleInfo.VehicleSpeed).Cast<Meter>();
						modData.Finish(VectoRun.Status.Success);
						break;
				}
			}

			modData.Finish(VectoRun.Status.Success);
		}

		private static AxleGearData CreateAxleGearData()
		{
			return new AxleGearData() {
				AxleGear = new GearData {
					Ratio = 3.0 * 3.5,
					LossMap = TransmissionLossMapReader.ReadFromFile(GbxLossMap, 3.0 * 3.5, "AxleGear")
				}
			};
		}

		private static VehicleData CreateVehicleData(Kilogram loading)
		{
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.4375,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false,
					TyreTestLoad = 62538.75.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.375,
					Inertia = 10.83333.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0065,
					TwinTyres = true,
					TyreTestLoad = 52532.55.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.1875,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false,
					TyreTestLoad = 62538.75.SI<Newton>()
				}
			};
			return new VehicleData {
				AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
				AirDensity = DeclarationData.AirDensity,
				CurbMass = 15700.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = 0.52.SI<Meter>(),
				AxleData = axles,
				SavedInDeclarationMode = false
			};
		}

		private static AirdragData CreateAirdragData()
		{
			return new AirdragData() {
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(3.2634.SI<SquareMeter>(),
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(3.2634.SI<SquareMeter>()),
						CrossWindCorrectionMode.NoCorrection),
			};
		}

		private static DriverData CreateDriverData(string accelerationFile)
		{
			return new DriverData {
				AccelerationCurve = AccelerationCurveReader.ReadFromFile(accelerationFile),
				LookAheadCoasting = new DriverData.LACData {
					Enabled = false,
					//Deceleration = -0.5.SI<MeterPerSquareSecond>()
					LookAheadDecisionFactor = new LACDecisionFactor()
				},
				OverSpeed = new DriverData.OverSpeedData {
					Enabled = false
				},
			};
		}
	}
}