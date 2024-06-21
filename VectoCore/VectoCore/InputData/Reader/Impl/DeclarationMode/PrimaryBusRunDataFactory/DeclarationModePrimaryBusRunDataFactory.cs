using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Declaration.PostMortemAnalysisStrategy;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory
{
	public abstract class DeclarationModePrimaryBusRunDataFactory
	{
		public abstract class PrimaryBusBase : AbstractDeclarationVectoRunDataFactory
		{
			#region Implementation of IVectoRunDataFactory

			protected internal IPrimaryBusDeclarationDataAdapter DataAdapter { get; }
			protected IDeclarationInputDataProvider DataProvider { get; }

			//public IDeclarationReport Report { get; }
	   
			protected PrimaryBusBase(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter) : base(dataProvider, report, cycleFactory, missionFilter, false)
			{
				DataAdapter = declarationDataAdapter;
				DataProvider = dataProvider;
				Report = report;
			}

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override DriverData CreateDriverData(Segment segment)
			{
				VectoSimulationJobType jobType = DataProvider.JobInputData.JobType;
				ArchitectureID arch = DataProvider.JobInputData.Vehicle.ArchitectureID;
				CompressorDrive compressorDrive = DataProvider.JobInputData.Vehicle.Components.BusAuxiliaries.PneumaticSupply.CompressorDrive;
				return DataAdapter.CreateBusDriverData(segment, jobType, arch, compressorDrive);
			}

			#endregion

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				return GetNextRun().First(x => x != null);
			}

			protected Segment GetSegment()
			{
				if (Vehicle.VehicleCategory != VehicleCategory.HeavyBusPrimaryVehicle)
				{
					throw new VectoException(
						"Invalid vehicle category for bus factory! {0}", Vehicle.VehicleCategory.GetCategoryName());
				}

				var segment = DeclarationData.PrimaryBusSegments.Lookup(
					Vehicle.VehicleCategory, Vehicle.AxleConfiguration, Vehicle.Articulated);
				if (!segment.Found)
				{
					throw new VectoException(
						"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, primary",
						Vehicle.VehicleCategory, Vehicle.AxleConfiguration,
						Vehicle.Articulated);
				}

				return segment;
			}

			protected override void Initialize()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle)
				{
					return;
				}

				_segment = GetSegment();
				
			}

			#endregion

			protected VectoRunData CreateCommonRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				Segment segment,
				IList<IEngineModeDeclarationInputData> engineModes = null, int modeIdx = 0)
			{
				var cycle = CycleFactory.GetDeclarationCycle(mission);

                CheckSuperCap(Vehicle);
				AngleDriveAllowed(inputData:Vehicle);
				var simulationRunData = new VectoRunData {
					InputData = DataProvider,
					Loading = loading.Key,
					JobType = Vehicle.VehicleType,
					Mission = mission,
					InputDataHash = InputDataProvider.XMLHash,
					SimulationType = SimulationType.DistanceCycle,
					Report = Report,
					JobName = InputDataProvider.JobInputData.JobName,
					ModFileSuffix = $"{(engineModes?.Count > 1 ? $"_EngineMode{modeIdx}_" : "")}" +
									$"_{mission.BusParameter.BusGroup.GetClassNumber()}_{loading.Key}",
					MaxChargingPower = Vehicle.MaxChargingPower,
					VehicleDesignSpeed = segment.DesignSpeed,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					ExecutionMode = ExecutionMode.Declaration,
				};
				simulationRunData.PostMortemStrategy = new PrimaryBusPostMortemStrategy();
                return simulationRunData;
			}

			protected abstract void CreateGearboxAndGearshiftData(VectoRunData runData);

			protected abstract bool AxleGearRequired();

			protected abstract void AngleDriveAllowed(IVehicleDeclarationInputData inputData);
        }

		public class Conventional : PrimaryBusBase
		{
			public Conventional(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = DataProvider.JobInputData.Vehicle;
				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					foreach (var mission in _segment.Missions) {
						foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {
							var simulationRunData = CreateVectoRunData(mission, loading, modeIdx);
							if (simulationRunData == null) {
								continue;
							}
							yield return simulationRunData;
						}
					}
				}
			}

			#endregion

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var engine = Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				if (!modeIdx.HasValue) {
					throw new VectoException("Engine mode has to be specified for conventional vehicle");
				}
				var engineMode = engineModes[modeIdx.Value];

				var simulationRunData = CreateCommonRunData(mission, loading, _segment, engineModes, modeIdx.Value);

				simulationRunData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				simulationRunData.AirdragData = DataAdapter.CreateAirdragData(null, mission, new Segment());
				simulationRunData.EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission);
				simulationRunData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				simulationRunData.AxleGearData = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				simulationRunData.AngledriveData = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				simulationRunData.Aux = DataAdapter.CreateAuxiliaryData(
					Vehicle.Components.AuxiliaryInputData,
					Vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					Vehicle.Length ?? mission.BusParameter.VehicleLength,
					Vehicle.Components.AxleWheels.NumSteeredAxles, Vehicle.VehicleType);
				simulationRunData.DriverData = DriverData;
				

				simulationRunData.EngineData.FuelMode = modeIdx.Value;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
				
				CreateGearboxAndGearshiftData(simulationRunData);

				simulationRunData.Retarder = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);
				simulationRunData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, simulationRunData);
				
				return simulationRunData;
			}

			#region Overrides of PrimaryBusBase

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				var shiftStrategyName = PowertrainBuilder.GetShiftStrategyName(Vehicle.Components.GearboxInputData.Type, Vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));
				
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						Vehicle.EngineIdleSpeed, 
						Vehicle.Components.GearboxInputData.Type, 
						Vehicle.Components.GearboxInputData.Gears.Count);

			}

			protected override bool AxleGearRequired()
			{
				return true;
			}

			protected override void AngleDriveAllowed(IVehicleDeclarationInputData inputData)
			{
				//No checks necessary with conventional vehicles
				return;
			}

			#endregion
		}

		public abstract class Hybrid : PrimaryBusBase
		{
			protected Hybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = DataProvider.JobInputData.Vehicle;
				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					foreach (var mission in _segment.Missions) {
						foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {

							if (vehicle.OvcHev) {
								if (vehicle.MaxChargingPower == null || vehicle.MaxChargingPower.IsEqual(0)) {
									throw new VectoException(
										"MaxChargingPower is required and has to be greater than 0 if OVC is selected");
								}
								yield return CreateVectoRunData(mission, loading, modeIdx, OvcHevMode.ChargeDepleting);
								yield return CreateVectoRunData(mission, loading, modeIdx, OvcHevMode.ChargeSustaining);
							} else {
								yield return CreateVectoRunData(mission, loading, modeIdx, OvcHevMode.ChargeSustaining);
							}
						}
					}
				}
			}


		}

		public abstract class SerialHybrid : Hybrid
		{
			protected SerialHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter)
			{

			}

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				//CheckMaxChargingPowerPresent(vehicle);
				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx.Value];
				var runData = CreateCommonRunData(mission, loading, _segment, engineModes, modeIdx.Value);


				runData.DriverData = DriverData;
				runData.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle.Components.AirdragInputData, mission, _segment);
				runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);


				runData.EngineData = DataAdapter.CreateEngineData(Vehicle, engineMode, mission);

				DataAdapter.CreateREESSData(Vehicle.Components.ElectricStorage, Vehicle.VehicleType, Vehicle.OvcHev,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));

				runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(
					Vehicle.Components.ElectricMachines, Vehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateAverageVoltage());

				if (Vehicle.VehicleType == VectoSimulationJobType.IEPC_S) {
					var iepcData = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						runData.BatteryData.CalculateAverageVoltage());
					iepcData.ForEach(iepc => runData.ElectricMachinesData.Add(iepc));
				}

				if (AxleGearRequired()) {
					runData.AxleGearData = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				CreateGearboxAndGearshiftData(runData);
				runData.Retarder = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);

				runData.Aux = DataAdapter.CreateAuxiliaryData(Vehicle.Components.AuxiliaryInputData, 
					Vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, Vehicle.Length, Vehicle.Components.AxleWheels.NumSteeredAxles,
					VectoSimulationJobType.SerialHybridVehicle);
				runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, runData);


				runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData, runData.SuperCapData, runData.VehicleData.TotalVehicleMass, 
						ovcMode, loading.Key, runData.VehicleData.VehicleClass, mission.MissionType);

				if (ovcMode != OvcHevMode.NotApplicable) {
					if (runData.BatteryData != null) {
						runData.BatteryData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}

					if (runData.SuperCapData != null) {
						runData.SuperCapData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}
				}

				if (ovcMode != OvcHevMode.NotApplicable && runData.InputData.JobInputData.Vehicle.OvcHev) {
					runData.ModFileSuffix += ovcMode == OvcHevMode.ChargeSustaining ? "CS" : "CD";
				}

				if (ovcMode == OvcHevMode.ChargeDepleting) {
					runData.BatteryData.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
				}

				runData.OVCMode = ovcMode;

				return runData;
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID.IsOneOf(ArchitectureID.S2, ArchitectureID.S_IEPC)) {
					throw new ArgumentException(nameof(Vehicle.ArchitectureID));
				}
				runData.GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}

			protected override bool AxleGearRequired()
			{
				return InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData != null;
			}

			protected override void AngleDriveAllowed(IVehicleDeclarationInputData inputData)
			{
				if (inputData.Components.AngledriveInputData != null && inputData.Components.AngledriveInputData.Type != AngledriveType.None)
				{
					throw new VectoException("Angledrive not allowed in serial hybrid vehicles");
				}
			}
        }


		public class HEV_S2 : SerialHybrid
		{
			public HEV_S2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID != ArchitectureID.S2) {
					throw new ArgumentException(nameof(Vehicle));
				}

				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null, 
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count);


				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(Vehicle.Components.GearboxInputData.Type,
						Vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}
		}

		public class HEV_S3 : SerialHybrid
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

		}

		public class HEV_S4 : SerialHybrid
		{
			public HEV_S4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

		}

		public class HEV_S_IEPC : SerialHybrid
		{
			public HEV_S_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				AxleGearRequired();
				return base.CreateVectoRunData(mission, loading, modeIdx, ovcMode);
			}

			protected override bool AxleGearRequired()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				var iepcInput = vehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
				if (axleGearRequired && vehicle.Components.AxleGearInputData == null) {
					throw new VectoException(
						$"Axlegear reqhired for selected type of IEPC! DifferentialIncluded: {iepcInput.DifferentialIncluded}, DesignTypeWheelMotor: {iepcInput.DesignTypeWheelMotor}");
				}

				var numGearsPowermap =
					iepcInput.VoltageLevels.Select(x => Tuple.Create(x.VoltageLevel, x.PowerMap.Count)).ToArray();
				var gearCount = iepcInput.Gears.Count;
				var numGearsDrag = iepcInput.DragCurves.Count;

				if (numGearsPowermap.Any(x => x.Item2 != gearCount)) {
					throw new VectoException(
						$"Number of gears for voltage levels does not match! PowerMaps: {numGearsPowermap.Select(x => $"{x.Item1}: {x.Item2}").Join()}; Gear count: {gearCount}");
				}

				if (numGearsDrag > 1 && numGearsDrag != gearCount) {
					throw new VectoException(
						$"Number of gears drag curve does not match gear count! DragCurve {numGearsDrag}; Gear count: {gearCount}");
				}

				return axleGearRequired;
			}

            protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null,
						GearboxType.APTN,
						Vehicle.Components.IEPC.Gears.Count);


				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(GearboxType.APTN,
						Vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}
		}

		public abstract class ParallelHybrid : Hybrid
		{
			protected ParallelHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var engine = Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				if (!modeIdx.HasValue) {
					throw new VectoException("Engine mode has to be specified for parallel hybrid vehicle");
				}
				var engineMode = engineModes[modeIdx.Value];

				var runData = CreateCommonRunData(mission, loading, _segment, engineModes, modeIdx.Value);

				runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				runData.AirdragData = DataAdapter.CreateAirdragData(null, mission, new Segment());
				runData.EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission);
				DataAdapter.CreateREESSData(Vehicle.Components.ElectricStorage, Vehicle.VehicleType, Vehicle.OvcHev,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));
				runData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				runData.AxleGearData = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				runData.AngledriveData = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				runData.Aux = DataAdapter.CreateAuxiliaryData(
					Vehicle.Components.AuxiliaryInputData,
					Vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					Vehicle.Length ?? mission.BusParameter.VehicleLength,
					Vehicle.Components.AxleWheels.NumSteeredAxles,
					VectoSimulationJobType.ParallelHybridVehicle);//Hardcode to override IHPC
				runData.DriverData = DriverData;


				runData.EngineData.FuelMode = modeIdx.Value;
				runData.VehicleData.VehicleClass = _segment.VehicleClass;

				CreateGearboxAndGearshiftData(runData);
				runData.Retarder = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);
				runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, runData);

				runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(
					Vehicle.Components.ElectricMachines, Vehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateAverageVoltage(), runData.GearboxData.GearList);

				runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData,
						runData.SuperCapData,
						runData.VehicleData.TotalVehicleMass,
						ovcMode, loading.Key,
						//runData.VehicleData.VehicleClass,
						mission.BusParameter.BusGroup,
						mission.MissionType, Vehicle.BoostingLimitations, runData.GearboxData, runData.EngineData, runData.ElectricMachinesData, Vehicle.ArchitectureID);

				if (ovcMode != OvcHevMode.NotApplicable) {
					if (runData.BatteryData?.InitialSoC != null) {
						runData.BatteryData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}

					if (runData.SuperCapData?.InitialSoC != null) {
						runData.SuperCapData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}
				}

				if (ovcMode == OvcHevMode.ChargeDepleting) {
					runData.BatteryData.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
				}

				if (ovcMode == OvcHevMode.ChargeSustaining) {
					runData.IterativeRunStrategy = new HevChargeSustainingIterativeRunStrategy();
				}
				if (ovcMode != OvcHevMode.NotApplicable && runData.InputData.JobInputData.Vehicle.OvcHev) {
					runData.ModFileSuffix += ovcMode == OvcHevMode.ChargeSustaining ? "CS" : "CD";
				}
				runData.OVCMode = ovcMode;
				
				return runData;
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{	
				var shiftStrategyName = PowertrainBuilder.GetShiftStrategyName(Vehicle.Components.GearboxInputData.Type, Vehicle.VehicleType);
				
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						Vehicle.EngineIdleSpeed,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count);
				
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}

			protected override bool AxleGearRequired()
			{
				return true;
			}

			protected override void AngleDriveAllowed(IVehicleDeclarationInputData inputData)
			{
				//No checks necessary with conventional vehicles
				return;
			}
        }

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

		}

		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

		}

		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

			
		}

		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

		}

		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

		}

		public class HEV_P_IHPC : HEV_P2
		{
			public HEV_P_IHPC(IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, 
				IDeclarationCycleFactory cycleFactory, 
				IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }
		}

		public abstract class BatteryElectric : PrimaryBusBase
		{
			public BatteryElectric(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				foreach (var mission in _segment.Missions) {
					foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {
						var simulationRunData = CreateVectoRunData(mission, loading);
						simulationRunData.BatteryData.Batteries.ForEach(t => t.Item2.ChargeDepletingBattery = true);
						yield return simulationRunData;
					}
				}
			}

			#endregion

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var result = CreateCommonRunData(mission, loading, _segment);


				DataAdapter.CreateREESSData(
					componentsElectricStorage: Vehicle.Components.ElectricStorage,
					Vehicle.VehicleType,
					true,
					(bs) => result.BatteryData = bs,
					(sc) => result.SuperCapData = sc);

				
				if (Vehicle.VehicleType == VectoSimulationJobType.IEPC_E) {
					result.ElectricMachinesData = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						result.BatteryData.CalculateAverageVoltage());
				} else {
					result.ElectricMachinesData = DataAdapter.CreateElectricMachines(Vehicle.Components.ElectricMachines,
						Vehicle.ElectricMotorTorqueLimits, result.BatteryData.CalculateAverageVoltage(), null);
				}

				result.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				result.AirdragData = DataAdapter.CreateAirdragData(null, mission, new Segment());
				if (AxleGearRequired() || Vehicle.Components.AxleGearInputData != null) {
					result.AxleGearData = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				result.AngledriveData = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				result.Aux = DataAdapter.CreateAuxiliaryData(
					Vehicle.Components.AuxiliaryInputData,
					Vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					Vehicle.Length ?? mission.BusParameter.VehicleLength,
					Vehicle.Components.AxleWheels.NumSteeredAxles, Vehicle.VehicleType);
				var emPos = result.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
				result.DriverData = DriverData;

				result.VehicleData.VehicleClass = _segment.VehicleClass;
				CreateGearboxAndGearshiftData(result);
				result.Retarder = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);
				result.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, result);
				
				return result;
			}
			protected override bool AxleGearRequired()
			{
				return InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E4;
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID == ArchitectureID.E2) {
					throw new ArgumentException();
				}
				runData.GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}

			protected override void AngleDriveAllowed(IVehicleDeclarationInputData inputData)
			{
				if (inputData.Components.AngledriveInputData != null && inputData.Components.AngledriveInputData.Type != AngledriveType.None)
				{
					throw new VectoException("Angledrive not allowed in pure electric vehicles");
				}
			}
        }


		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

			#region Overrides of PrimaryBusBase

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(Vehicle));
				}

				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count);


				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(Vehicle.Components.GearboxInputData.Type,
						Vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}
			#endregion
		}

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }


			protected override bool AxleGearRequired()
			{
				var iepcInput = Vehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
				if (axleGearRequired && Vehicle.Components.AxleGearInputData == null) {
					throw new VectoException(
						$"Axlegear required for selected type of IEPC! DifferentialIncluded: {iepcInput.DifferentialIncluded}, DesignTypeWheelMotor: {iepcInput.DesignTypeWheelMotor}");
				}

				var numGearsPowermap =
					iepcInput.VoltageLevels.Select(x => Tuple.Create(x.VoltageLevel, x.PowerMap.Count)).ToArray();
				var gearCount = iepcInput.Gears.Count;
				var numGearsDrag = iepcInput.DragCurves.Count;

				if (numGearsPowermap.Any(x => x.Item2 != gearCount)) {
					throw new VectoException(
						$"Number of gears for voltage levels does not match! PowerMaps: {numGearsPowermap.Select(x => $"{x.Item1}: {x.Item2}").Join()}; Gear count: {gearCount}");
				}

				if (numGearsDrag > 1 && numGearsDrag != gearCount) {
					throw new VectoException(
						$"Number of gears drag curve does not match gear count! DragCurve {numGearsDrag}; Gear count: {gearCount}");
				}

				return axleGearRequired || Vehicle.Components.AxleGearInputData != null;

			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				var iepcInput = Vehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
                var axleGearRatio = axleGearRequired ? runData.AxleGearData.AxleGear.Ratio : 1.0;
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						axleGearRatio,
						null,
						GearboxType.APTN,
						Vehicle.Components.IEPC.Gears.Count
					);
				var shiftStrategyName =
					PowertrainBuilder.GetShiftStrategyName(GearboxType.APTN,
						Vehicle.VehicleType);
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData,
					ShiftPolygonCalculator.Create(shiftStrategyName, runData.GearshiftParameters));

			}
		}

		public class Exempted : PrimaryBusBase
		{
			public Exempted(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter) : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter) { }

			#region Overrides of PrimaryBusBase

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				return CreateVectoRunData(null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 0);
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				throw new NotImplementedException();
			}

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var simulationRunData = CreateVectoRunData(null,
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(),
					0);

				yield return simulationRunData;
			}

			#endregion

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var runData = new VectoRunData {
					InputData = DataProvider,
					Exempted = true,
					Report = Report,
					Mission = new Mission { MissionType = MissionType.ExemptedMission },
					VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, new Segment(),
						null,
						new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad,
							Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null)), _allowVocational),
					InputDataHash = InputDataProvider.XMLHash
				};
				return runData;
			}

			protected override bool AxleGearRequired()
			{
				return false;
			}

			#region Overrides of PrimaryBusBase

			protected override void AngleDriveAllowed(IVehicleDeclarationInputData inputData)
			{
				return;
			}

			#endregion
		}
	}
}