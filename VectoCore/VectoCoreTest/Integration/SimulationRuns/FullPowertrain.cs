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
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;

namespace TUGraz.VectoCore.Tests.Integration.SimulationRuns
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class FullPowerTrain
	{
		public const string CycleFile = @"TestData/Integration/FullPowerTrain/1-Gear-Test-dist.vdri";
		public const string CoachCycleFile = @"TestData/Integration/FullPowerTrain/Coach.vdri";
		public const string EngineFile = @"TestData/Components/24t Coach.veng";
		public const string AccelerationFile = @"TestData/Components/Coach.vacc";
		public const string GearboxLossMap = @"TestData/Components/Indirect Gear.vtlm";
		public const string AxleLossMap = @"TestData/Components/Axle.vtlm";
		public const string GearboxShiftPolygonFile = @"TestData/Components/ShiftPolygons.vgbs";
		//public const string GearboxFullLoadCurveFile = @"TestData/Components/Gearbox.vfld";
		private static readonly LoggingObject Log = LogManager.GetLogger(typeof(FullPowerTrain).ToString());

		[OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase, Category("LongRunning")]
		public void Test_FullPowertrain_SimpleGearbox()
		{
			var gearboxData = CreateSimpleGearboxData();
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile, gearboxData.Gears.Count);
			var cycleData = DrivingCycleDataReader.ReadFromFile(CycleFile, CycleType.DistanceBased, false);
			var axleGearData = CreateAxleGearData();
			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var driverData = CreateDriverData(AccelerationFile);
			var airDragData = CreateAirdragData();

			var runData = new VectoRunData() {
				JobName = "Coach_FullPowertrain_SimpleGearbox",
				DriverData = driverData,
				EngineData = engineData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				GearshiftParameters = CreateGearshiftData(),
				VehicleData = vehicleData,
				AirdragData = airDragData,
				Retarder = new RetarderData() { Type = RetarderType.None},
				Cycle = cycleData,
				Aux = new List<VectoRunData.AuxData>(),
				SimulationType = SimulationType.DistanceCycle,
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				ExecutionMode = ExecutionMode.Engineering,
            };
			var fileWriter = new FileOutputWriter("Coach_FullPowertrain_SimpleGearbox");
			var modData = new ModalDataContainer(runData, fileWriter, null);
			var container = VehicleContainer.CreateVehicleContainer(runData, modData, null);

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);
			var cyclePort = cycle.OutPort();

			cycle.AddComponent(new Driver(container, driverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, vehicleData, airDragData))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new AMTGearbox(container, new AMTShiftStrategy(container)))
				.AddComponent(new Clutch(container, engineData))
				.AddComponent(new CombustionEngine(container, engineData));

			cyclePort.Initialize();

			var absTime = 0.SI<Second>();
			var ds = Constants.SimulationSettings.DriveOffDistance;
			container.AbsTime = absTime;
			IResponse response;

			var cnt = 0;
			do {
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
							: Constants.SimulationSettings.TargetTimeInterval * container.VehicleInfo.VehicleSpeed;
						if (cnt++ % 100 == 0) {
							modData.Finish(VectoRun.Status.Success);
						}
						break;
					default:
						Assert.Fail("Unexpected Response: {0}", response);
						break;
				}
			} while (!(response is ResponseCycleFinished));
			modData.Finish(VectoRun.Status.Success);
			Assert.IsInstanceOf<ResponseCycleFinished>(response);
		}

		[Category("LongRunning")]
		[TestCase]
		public void Test_FullPowertrain()
		{
			var gearboxData = CreateGearboxData();
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile, gearboxData.Gears.Count);
			var cycleData = DrivingCycleDataReader.ReadFromFile(CoachCycleFile, CycleType.DistanceBased, false);
			var axleGearData = CreateAxleGearData();
			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var driverData = CreateDriverData(AccelerationFile);
			var airDragData = CreateAirdragData();

			var runData = new VectoRunData() {
				JobName = "Coach_FullPowertrain",
				Cycle = cycleData,
				EngineData = engineData,
				VehicleData = vehicleData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				GearshiftParameters = CreateGearshiftData(),
				AirdragData = airDragData,
				DriverData = driverData,
				Retarder = new RetarderData() {Type = RetarderType.None },
				SimulationType = SimulationType.DistanceCycle,
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				Aux = new List<VectoRunData.AuxData>(),
				ExecutionMode = ExecutionMode.Engineering,
            };
			var fileWriter = new FileOutputWriter("Coach_FullPowertrain");
			var modData = new ModalDataContainer(runData, fileWriter, null);
			var container = VehicleContainer.CreateVehicleContainer(runData, modData, null);


			var cycle = new DistanceBasedDrivingCycle(container, cycleData);
			var cyclePort = cycle.OutPort();
			cycle.AddComponent(new Driver(container, driverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, vehicleData, airDragData))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new AMTGearbox(container, new AMTShiftStrategy(container)))
				.AddComponent(new Clutch(container, engineData))
				.AddComponent(new CombustionEngine(container, engineData));

			cyclePort.Initialize();

			//gbx.Gear = 0;

			var absTime = 0.SI<Second>();
			var ds = Constants.SimulationSettings.DriveOffDistance;
			var response = cyclePort.Request(absTime, ds);
			container.AbsTime = absTime;
			Assert.IsInstanceOf<ResponseSuccess>(response);
			container.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			//gbx.Gear = 1;
			var cnt = 0;
			while (!(response is ResponseCycleFinished) && container.MileageCounter.Distance < 17000) {
				Log.Info("Test New Request absTime: {0}, ds: {1}", absTime, ds);
				try {
					response = cyclePort.Request(absTime, ds);
				} catch (Exception) {
					modData.Finish(VectoRun.Status.Success);
					throw;
				}
				Log.Info("Test Got Response: {0},", response);

				switch (response) {
					case ResponseCycleFinished _:
						break;
					case ResponseDrivingCycleDistanceExceeded r:
						ds = r.MaxDistance;
						break;
					case ResponseGearShift _:
						Log.Debug("Gearshift");
						break;
					case ResponseSuccess r:
						container.CommitSimulationStep(absTime, r.SimulationInterval);
						absTime += r.SimulationInterval;
						ds = container.VehicleInfo.VehicleSpeed.IsEqual(0)
							? Constants.SimulationSettings.DriveOffDistance
							: Constants.SimulationSettings.TargetTimeInterval * container.VehicleInfo.VehicleSpeed;
						if (cnt++ % 100 == 0) {
							modData.Finish(VectoRun.Status.Success);
						}
						break;
					default:
						Assert.Fail("Unexpected Response: {0}", response);
						break;
				}
			}
			modData.Finish(VectoRun.Status.Success);
			Assert.IsInstanceOf<ResponseSuccess>(response);
		}

		[TestCase, Category("LongRunning")]
		public void Test_FullPowertrain_LowSpeed()
		{
			var gearboxData = CreateGearboxData();
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile, gearboxData.Gears.Count);
			var cycleData = DrivingCycleDataReader.ReadFromFile(CycleFile, CycleType.DistanceBased, false);
			var axleGearData = CreateAxleGearData();
			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var airDragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile);

			var runData = new VectoRunData() {
				JobName = "Coach_FullPowertrain_LowSpeed",
				EngineData = engineData,
				VehicleData = vehicleData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				GearshiftParameters = CreateGearshiftData(),
				AirdragData = airDragData,
				Retarder = new RetarderData() { Type = RetarderType.None},
				DriverData = driverData,
				Cycle = cycleData,
				Aux = new List<VectoRunData.AuxData>(),
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				ExecutionMode = ExecutionMode.Engineering,
            };
			var fileWriter = new FileOutputWriter("Coach_FullPowertrain_LowSpeed");
			var modData = new ModalDataContainer(runData, fileWriter, null);
			var container = VehicleContainer.CreateVehicleContainer(runData, modData, null);

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);
			var cyclePort = cycle.OutPort();
			cycle.AddComponent(new Driver(container, driverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, vehicleData, airDragData))
				.AddComponent(new Wheels(container, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, axleGearData))
				.AddComponent(new AMTGearbox(container, new AMTShiftStrategy(container)))
				.AddComponent(new Clutch(container, engineData))
				.AddComponent(new CombustionEngine(container, engineData));

			cyclePort.Initialize();

			//container.Gear = 0;
			var absTime = 0.SI<Second>();
			var ds = Constants.SimulationSettings.DriveOffDistance;
			container.AbsTime = absTime;
			var response = cyclePort.Request(absTime, ds);
			Assert.IsInstanceOf<ResponseSuccess>(response);
			container.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			//container.Gear = 1;
			var cnt = 0;
			while (!(response is ResponseCycleFinished) && container.MileageCounter.Distance < 17000) {
				Log.Info("Test New Request absTime: {0}, ds: {1}", absTime, ds);
				try {
					response = cyclePort.Request(absTime, ds);
				} catch (Exception) {
					modData.Finish(VectoRun.Status.Success);
					throw;
				}
				Log.Info("Test Got Response: {0},", response);

				switch (response) {
					case ResponseCycleFinished _:
						break;
					case ResponseDrivingCycleDistanceExceeded r:
						ds = r.MaxDistance;
						break;
					case ResponseGearShift _:
						Log.Debug("Gearshift");
						break;
					case ResponseSuccess r:
						container.CommitSimulationStep(absTime, r.SimulationInterval);
						absTime += r.SimulationInterval;
						ds = container.VehicleInfo.VehicleSpeed.IsEqual(0)
							? Constants.SimulationSettings.DriveOffDistance
							: Constants.SimulationSettings.TargetTimeInterval * container.VehicleInfo.VehicleSpeed;
						if (cnt++ % 100 == 0) {
							modData.Finish(VectoRun.Status.Success);
						}
						break;
					default:
						modData.Finish(VectoRun.Status.Success);
						Assert.Fail("Unexpected Response: {0}", response);
						break;
				}
			}
			modData.Finish(VectoRun.Status.Success);
			Assert.IsInstanceOf<ResponseCycleFinished>(response);
		}

		[Category("LongRunning")]
		[TestCase]
		public void Test_FullPowerTrain_JobFile()
		{
			const string jobFile = @"TestData/job.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(factory);
			jobContainer.Execute();

			jobContainer.WaitFinished();
			ResultFileHelper.TestSumFile(@"TestData/Results/Integration/job.vsum", @"TestData/job.vsum");

			ResultFileHelper.TestModFile(@"TestData/Results/Integration/job_1-Gear-Test-dist.vmod",
				@"TestData/job_1-Gear-Test-dist.vmod", testRowCount: false);
		}

		private static GearboxData CreateGearboxData()
		{
			var ratios = new[] { 6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1, 0.76 };

			return new GearboxData {
				Gears = ratios.Select((ratio, i) =>
					Tuple.Create((uint)i,
						new GearData {
							// MaxTorque = ratio > 5 ? 2300.SI<NewtonMeter>() : null,
							LossMap = TransmissionLossMapReader.ReadFromFile(GearboxLossMap, ratio, $"Gear {i}"),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile)
						}))
					.ToDictionary(k => k.Item1 + 1, v => v.Item2),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
			};
		}

		private static ShiftStrategyParameters CreateGearshiftData()
		{
			return new ShiftStrategyParameters() {
				TimeBetweenGearshifts = 2.SI<Second>(),
				StartSpeed = 2.SI<MeterPerSecond>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),
				StartTorqueReserve = 0.2,
				TorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration

			};
		}

		private static AxleGearData CreateAxleGearData()
		{
			var ratio = 3.240355;
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.ReadFromFile(AxleLossMap, ratio, "AxleGear")
				}
			};
		}

		private static GearboxData CreateSimpleGearboxData()
		{
			var ratio = 3.44;
			return new GearboxData {
				Gears = new Dictionary<uint, GearData> {
					{
						1, new GearData {
							LossMap = TransmissionLossMapReader.ReadFromFile(GearboxLossMap, ratio, "Gear 1"),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile)
						}
					}
				},
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 0.SI<Second>(),
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
				VehicleCategory = VehicleCategory.RigidTruck,
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
			return new AirdragData {
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(3.2634.SI<SquareMeter>(), 0.SI<SquareMeter>(),
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
				OverSpeed = new DriverData.OverSpeedData { Enabled = false }
			};
		}
	}
}