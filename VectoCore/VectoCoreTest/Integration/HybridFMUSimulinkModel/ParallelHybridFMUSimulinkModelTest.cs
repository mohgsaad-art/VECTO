using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModFilter;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.ElectricSystem;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Integration.HybridFMUSimulinkModel
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ParallelHybridFMUSimulinkModelTest
    {
        //private ModalResultField[] Yfields;
        public const string MotorFile = @"TestData\Hybrids\ElectricMotor\GenericEMotor.vem";
        public const string BatFile = @"TestData\Hybrids\Battery\GenericBattery.vbat";

        public const string AccelerationFile = @"TestData\Components\Truck.vacc";
        public const string MotorFile240kW = @"TestData\Hybrids\ElectricMotor\GenericEMotor240kW.vem";

        public const string GearboxIndirectLoss = @"TestData\Components\Indirect Gear.vtlm";
        public const string GearboxDirectLoss = @"TestData\Components\Direct Gear.vtlm";
        public const bool PlotGraphs = true;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);


            //InitGraphWriter();
        }

        private GraphWriter GetGraphWriter(ModalResultField[] emYFields)
        {
            var Yfields = new[] {
                ModalResultField.v_act, ModalResultField.altitude, ModalResultField.acc, ModalResultField.Gear,
                ModalResultField.P_ice_out, ModalResultField.REESSStateOfCharge, ModalResultField.FCMap
            }.Concat(emYFields).ToArray();

            var graphWriter = new GraphWriter();
            graphWriter.Xfields = new[] { ModalResultField.dist };
            graphWriter.Yfields = Yfields;
            graphWriter.Series1Label = "Hybrid";
            graphWriter.PlotIgnitionState = true;

            if (PlotGraphs)
            {
                graphWriter.Enable();
            }
            else
            {
                graphWriter.Disable();
            }

            return graphWriter;
        }

        [
            TestCase(30, 0.7, 0, TestName = "P2 Hybrid DriveOff 30km/h SoC: 0.7, level"),
            TestCase(80, 0.7, 0, TestName = "P2 Hybrid DriveOff 80km/h SoC: 0.7, level"),
            TestCase(80, 0.7, 5, TestName = "P2 Hybrid DriveOff 80km/h SoC: 0.7, UH 5"),
        TestCase(30, 0.22, 0, TestName = "P2 Hybrid DriveOff 30km/h SoC: 0.22, level"),
        ]
        public void P2HybridDriveOff(double vmax, double initialSoC, double slope)
        {
            var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
            var cycleData = string.Format(
                @"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
            var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

            const bool largeMotor = true;

            var modFilename = string.Format("SimulinkMatlabSimpleParallelHybrid-P2_acc_{0}-{1}_{2}.vmod", vmax, initialSoC, slope);
            const PowertrainPosition pos = PowertrainPosition.HybridP2;
            var job = CreateEngineeringRun(
                cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
            var run = job.Runs.First().Run;

            var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
            Assert.NotNull(hybridController);

            var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

            //run.Run();
            job.Execute();
            job.WaitFinished();
            Assert.IsTrue(run.FinishedWithoutErrors);

            Assert.IsTrue(modData.Rows.Count > 0);
            graphWriter.Write(modFilename);
        }

        // =================================================

        public static JobContainer CreateEngineeringRun(DrivingCycleData cycleData, string modFileName, double initialSoc, PowertrainPosition pos, double ratio, bool largeMotor = false, double pAuxEl = 0, Kilogram payload = null, Watt maxDriveTrainPower = null)
        {
            var fileWriter = new FileOutputWriter(Path.GetFileNameWithoutExtension(modFileName));
            var sumData = new SummaryDataContainer(fileWriter);
            var jobContainer = new JobContainer(sumData);
            var container = CreateParallelHybridPowerTrain(
                cycleData, modFileName, initialSoc, largeMotor, sumData, pAuxEl, pos, ratio, payload, maxDriveTrainPower);
            var run = new DistanceRun(container);
            jobContainer.AddRun(run);
            return jobContainer;
        }

		public static VehicleContainer CreateParallelHybridPowerTrain(DrivingCycleData cycleData, string modFileName,
			double initialBatCharge, bool largeMotor, SummaryDataContainer sumData, double pAuxEl,
			PowertrainPosition pos, double ratio, Kilogram payload = null, Watt maxDriveTrainPower = null)
		{
			var gearboxData = CreateGearboxData();
			var axleGearData = CreateAxleGearData();

			var vehicleData = CreateVehicleData(payload ?? 3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var electricMotorData =
				MockSimulationDataFactory.CreateElectricMotorData(largeMotor ? MotorFile240kW : MotorFile, 1, pos, ratio, 1);

			var batteryData = MockSimulationDataFactory.CreateBatteryData(BatFile, initialBatCharge);
			//batteryData.TargetSoC = 0.5;

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(
				 Truck40tPowerTrain.EngineFile, gearboxData.Gears.Count);

			foreach (var entry in gearboxData.Gears)
			{
				entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
					(int)entry.Key, engineData.FullLoadCurves[entry.Key], new TransmissionInputData().Repeat(gearboxData.Gears.Count + 1).Cast<ITransmissionInputData>().ToList(), engineData, axleGearData.AxleGear.Ratio,
					vehicleData.DynamicTyreRadius);
			}

			var runData = new VectoRunData()
			{
				//PowertrainConfiguration = PowertrainConfiguration.ParallelHybrid,
				JobRunId = 0,
				JobType = VectoSimulationJobType.ParallelHybridVehicle,
				DriverData = driverData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				JobName = Path.GetFileNameWithoutExtension(modFileName),
				Cycle = cycleData,
				Retarder = new RetarderData() { Type = RetarderType.None },
				Aux = new List<VectoRunData.AuxData>(),
				ElectricMachinesData = electricMotorData,
				EngineData = engineData,
				BatteryData = batteryData,
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio, engineData.IdleSpeed),
				HybridStrategyParameters = CreateHybridStrategyData(maxDriveTrainPower),
				ElectricAuxDemand = pAuxEl.SI<Watt>()
			};
			var fileWriter = new FileOutputWriter(modFileName);
			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(runData, fileWriter, null, modDataFilter)
			{
				WriteModalResults = true,
			};
			var container = new VehicleContainer(
				ExecutionMode.Engineering, modData, x => { sumData?.Write(x, 1, 1, runData); });
			container.RunData = runData;

			//var strategy = new HybridStrategy(runData, container);
			var strategy = new FMUSimulinkModelHybridStrategy(runData, container);
			var es = new ElectricSystem(container);
			var battery = new Battery(container, batteryData);
			battery.Initialize(initialBatCharge);

			var clutch = new SwitchableClutch(container, runData.EngineData);
			var ctl = new HybridController(container, strategy, es);

			es.Connect(battery);

			var engine = new StopStartCombustionEngine(container, runData.EngineData);
			//var gearbox = new Gearbox(container, ctl.ShiftStrategy);
			var amtShiftStrategy = new AMTShiftStrategy(container);
			var gearbox = new Gearbox(container, amtShiftStrategy);
			//var hybridStrategy = new DelegateParallelHybridStrategy();
			ctl.Gearbox = gearbox;

			var idleController = engine.IdleController;
			ctl.Engine = engine;

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", pAuxEl.SI<Watt>());
			es.Connect(aux);

			cycle
				.AddComponent(new Driver(container, runData.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, runData.VehicleData, runData.AirdragData))
				.AddComponent(new Wheels(container, runData.VehicleData.DynamicTyreRadius,
					runData.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(new AxleGear(container, runData.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(runData.AngledriveData != null ? new Angledrive(container, runData.AngledriveData) : null)
				.AddComponent(gearbox, runData.Retarder, container)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, runData);

			return container;
		}

		private static HybridStrategyParameters CreateHybridStrategyData(Watt maxDriveTrainPower)
		{
			return new HybridStrategyParameters()
			{
				EquivalenceFactor = 2.5,
				MinSoC = 0.22,
				MaxSoC = 0.8,
				TargetSoC = 0.5,
				AuxReserveTime = 5.SI<Second>(),
				AuxReserveChargeTime = 2.SI<Second>(),
				MinICEOnTime = 3.SI<Second>(),
				MaxDrivetrainPower = maxDriveTrainPower ?? 1e12.SI<Watt>(),
			};
		}

		public static ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
		{
			var retVal = new ShiftStrategyParameters
			{
				TimeBetweenGearshifts = 2.SI<Second>(),
				TorqueReserve = 0.2,
				StartTorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
				StartSpeed = 2.SI<MeterPerSecond>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),

				StartVelocity = DeclarationData.GearboxTCU.StartSpeed,
				//StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
				GearResidenceTime = DeclarationData.GearboxTCU.GearResidenceTime,
				DnT99L_highMin1 = DeclarationData.GearboxTCU.DnT99L_highMin1,
				DnT99L_highMin2 = DeclarationData.GearboxTCU.DnT99L_highMin2,
				AllowedGearRangeUp = DeclarationData.GearboxTCU.AllowedGearRangeUp,
				AllowedGearRangeDown = DeclarationData.GearboxTCU.AllowedGearRangeDown,
				LookBackInterval = DeclarationData.GearboxTCU.LookBackInterval,
				DriverAccelerationLookBackInterval = DeclarationData.GearboxTCU.DriverAccelerationLookBackInterval,
				DriverAccelerationThresholdLow = DeclarationData.GearboxTCU.DriverAccelerationThresholdLow,
				AverageCardanPowerThresholdPropulsion = DeclarationData.GearboxTCU.AverageCardanPowerThresholdPropulsion,
				CurrentCardanPowerThresholdPropulsion = DeclarationData.GearboxTCU.CurrentCardanPowerThresholdPropulsion,
				TargetSpeedDeviationFactor = DeclarationData.GearboxTCU.TargetSpeedDeviationFactor,
				EngineSpeedHighDriveOffFactor = DeclarationData.GearboxTCU.EngineSpeedHighDriveOffFactor,
				RatingFactorCurrentGear = gbx.Type.AutomaticTransmission()
					? DeclarationData.GearboxTCU.RatingFactorCurrentGearAT
					: DeclarationData.GearboxTCU.RatingFactorCurrentGear,

				//--------------------
				RatioEarlyUpshiftFC = DeclarationData.GearboxTCU.RatioEarlyUpshiftFC / axleRatio,
				RatioEarlyDownshiftFC = DeclarationData.GearboxTCU.RatioEarlyDownshiftFC / axleRatio,
				AllowedGearRangeFC = gbx.Type.AutomaticTransmission()
					? (gbx.Gears.Count > DeclarationData.GearboxTCU.ATSkipGearsThreshold
						? DeclarationData.GearboxTCU.AllowedGearRangeFCATSkipGear
						: DeclarationData.GearboxTCU.AllowedGearRangeFCAT)
					: DeclarationData.GearboxTCU.AllowedGearRangeFCAMT,
				VelocityDropFactor = DeclarationData.GearboxTCU.VelocityDropFactor,
				AccelerationFactor = DeclarationData.GearboxTCU.AccelerationFactor,
				MinEngineSpeedPostUpshift = 0.RPMtoRad(),
				ATLookAheadTime = DeclarationData.GearboxTCU.ATLookAheadTime,

				LoadStageThresoldsUp = DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				LoadStageThresoldsDown = DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				ShiftSpeedsTCToLocked = DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked
														.Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
			};

			return retVal;
		}

		private static IElectricMotor GetElectricMachine(PowertrainPosition pos,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, VehicleContainer container,
			IElectricSystem es, IHybridController ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null)
			{
				return null;
			}

			container.ModData.AddElectricMotor(pos);
			ctl.AddElectricMotor(pos, motorData.Item2);
			var motor = new ElectricMotor(container, motorData.Item2, ctl.ElectricMotorControl(pos), pos);
			motor.Connect(es);
			return motor;
		}

		private static GearboxData CreateGearboxData()
		{
			var ratios = new[] { 14.93, 11.64, 9.02, 7.04, 5.64, 4.4, 3.39, 2.65, 2.05, 1.6, 1.28, 1.0 };

			return new GearboxData
			{
				Gears = ratios.Select(
					(ratio, i) => Tuple.Create(
						(uint)i, new GearData
						{
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap =
								TransmissionLossMapReader.ReadFromFile(
									ratio.IsEqual(1) ? GearboxIndirectLoss : GearboxDirectLoss, ratio,
									string.Format("Gear {0}", i)),
							Ratio = ratio,
							//ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(i,)
						})).ToDictionary(k => k.Item1 + 1, v => v.Item2),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
			};
		}


		private static AxleGearData CreateAxleGearData()
		{
			var ratio = 2.59;
			return new AxleGearData
			{
				AxleGear = new GearData
				{
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.Create(0.95, ratio, "Axlegear"),
				}
			};
		}

		private static VehicleData CreateVehicleData(Kilogram loading)
		{
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.38,
					Inertia = 20.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = false,
					TyreTestLoad = 30436.0.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.62,
					Inertia = 18.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = true,
					TyreTestLoad = 30436.SI<Newton>()
				},
			};
			return new VehicleData
			{
				AirDensity = DeclarationData.AirDensity,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				CurbMass = 11500.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = 0.465.SI<Meter>(),
				AxleData = axles,
				SavedInDeclarationMode = false,
				ADAS = new VehicleData.ADASData()
				{
					EngineStopStart = true
				}
			};
		}

		private static AirdragData CreateAirdragData()
		{
			return new AirdragData()
			{
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(
						3.2634.SI<SquareMeter>(),
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(3.2634.SI<SquareMeter>()),
						CrossWindCorrectionMode.NoCorrection),
			};
		}

		private static DriverData CreateDriverData(string accelerationFile, bool overspeed = false)
		{
			return new DriverData
			{
				AccelerationCurve = AccelerationCurveReader.ReadFromFile(accelerationFile),
				LookAheadCoasting = new DriverData.LACData
				{
					Enabled = true,
					MinSpeed = 50.KMPHtoMeterPerSecond(),

					//Deceleration = -0.5.SI<MeterPerSquareSecond>()
					LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
					LookAheadDecisionFactor = new LACDecisionFactor()
				},
				OverSpeed = new DriverData.OverSpeedData()
				{
					Enabled = true,
					MinSpeed = 50.KMPHtoMeterPerSecond(),
					OverSpeed = 5.KMPHtoMeterPerSecond()
				},
				EngineStopStart = new DriverData.EngineStopStartData()
				{
					EngineOffStandStillActivationDelay = DeclarationData.Driver.EngineStopStart.ActivationDelay,
					MaxEngineOffTimespan = DeclarationData.Driver.EngineStopStart.MaxEngineOffTimespan,
					UtilityFactor = DeclarationData.Driver.EngineStopStart.UtilityFactor
				}
			};
		}
	}
}