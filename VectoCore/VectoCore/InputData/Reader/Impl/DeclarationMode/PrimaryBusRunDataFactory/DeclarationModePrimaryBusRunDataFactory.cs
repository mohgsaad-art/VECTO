using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Declaration.PostMortemAnalysisStrategy;
using TUGraz.VectoCore.Models.Declaration.VehicleOperation;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory
{
	public abstract class DeclarationModePrimaryBusRunDataFactory
	{
		public abstract class PrimaryBusBase : AbstractDeclarationVectoRunDataFactory
		{
            protected VehicleOperationLookup VehicleOperation => new VehicleOperationLookup();

            #region Implementation of IVectoRunDataFactory

            protected internal IPrimaryBusDeclarationDataAdapter DataAdapter { get; }
			protected new IDeclarationInputDataProvider DataProvider { get; }

			//public IDeclarationReport Report { get; }

			private PrimaryAndCompletedGroups _primaryAndCompletedGroups;

			protected PrimaryBusBase(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, cycleFactory, missionFilter, false, ptBuilder)
			{
				DataAdapter = declarationDataAdapter;
				DataProvider = dataProvider;
				Report = report;

				_primaryAndCompletedGroups = new PrimaryAndCompletedGroups();
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

			protected Mission[] GetMissions()
			{
				if (CompletedVehicle == null)
				{
					return _segment.Missions;
				}

				var completedVehicleSegment = DeclarationData.CompletedBusSegments.Lookup(
					Vehicle.AxleConfiguration.NumAxles(), CompletedVehicle.VehicleCode, CompletedVehicle.RegisteredClass, CompletedVehicle.NumberPassengerSeatsLowerDeck,
					CompletedVehicle.Height, CompletedVehicle.LowEntry);

				if (completedVehicleSegment.Missions == null)
				{
					throw new VectoException(
						$"Failed to find missions for completed vehicle with code: {CompletedVehicle.VehicleCode}, registration class: {CompletedVehicle.RegisteredClass}, " +
						$"passenger seats in lower deck: {CompletedVehicle.NumberPassengerSeatsLowerDeck}, height: {CompletedVehicle.Height}, " +
						$"low entry: {CompletedVehicle.LowEntry}, number of axles: {Vehicle.AxleConfiguration.NumAxles()}");
				}

				var missionTypes = completedVehicleSegment.Missions.Select(x => x.MissionType).Distinct();

				var completedGroup = VehicleClassHelper.GetClassNumber(completedVehicleSegment.VehicleClass);
				var groupData = _primaryAndCompletedGroups.Lookup(completedGroup);

				var matchingMissions = _segment.Missions
					.Where(x => missionTypes.Contains(x.MissionType) && VehicleClassHelper.GetClassNumber(x.BusParameter.BusGroup) == groupData.PrimaryGroup)
					.ToArray();

				if (matchingMissions.Length == 0)
				{
					throw new VectoException(
						$"Failed to find missions for a {groupData.CompletedGroup} completed vehicle (matches {groupData.PrimaryGroup} primary vehicles), " +
						$"with an input of a {_segment.VehicleClass.ToXML()} primary vehicle.");
				}

				return matchingMissions;
			}

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
						"no segment found for vehicle configuration: vehicle category: {0}, axle configuration: {1}, articulated: {2}, primary",
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

                var vehicleOperation = VehicleOperation.LookupVehicleOperation(mission.BusParameter?.BusGroup ?? _segment.VehicleClass, mission.MissionType);

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
					MaxChargingPower = Vehicle.MaxChargingPower ?? vehicleOperation.StationaryChargingMaxPwrInfrastructure,
					VehicleDesignSpeed = segment.DesignSpeed,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					ExecutionMode = ExecutionMode.Declaration,
					InMotionCharging = !Vehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable),
					InMotionChargingTechnology = Vehicle.InMotionCharging.Technology,
				};
				simulationRunData.PostMortemStrategy = new PrimaryBusPostMortemStrategy();
				return simulationRunData;
			}

			protected abstract void CreateGearboxAndGearshiftData(VectoRunData runData);

			protected abstract bool AxleGearRequired();
		}

		public abstract class MultiplePowertrainsBase : PrimaryBusBase
		{
			protected MultiplePowertrainsBase(
				IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder) 
				:
				base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder)
			{ }

            protected VectoRunData CreateCommonMultiplePowertrainsVectoRunData(
                Mission mission,
                KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
                IList<IEngineModeDeclarationInputData> engineModes = null,
                int? modeIdx = null,
                OvcHevMode ovcMode = OvcHevMode.NotApplicable)
            {
                var runData = CreateCommonRunData(mission, loading, _segment, engineModes, modeIdx.HasValue ? modeIdx.Value : 0);

                runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);

                runData.DriverData = DriverData;

                DataAdapter.CreateREESSData(
                    Vehicle.Components.ElectricStorage,
                    Vehicle.VehicleType,
                    Vehicle.OVC,
					Vehicle.BatteryOnlyMode,
                    (bs) => runData.BatteryData = bs,
                    (sc) => runData.SuperCapData = sc);

                var averageVoltage = runData.BatteryData?.CalculateVoltageCenterSoc();

                runData.AxlePowertrains = DataAdapter.CreateAxlePowertrainsData(
                    Vehicle,
                    averageVoltage,
                    runData.BatteryOnlyHybridMode,
                    runData.VehicleData,
                    mission);

                runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);

                runData.Aux = DataAdapter.CreateAuxiliaryData(
                    Vehicle.Components.AuxiliaryInputData,
                    Vehicle.Components.BusAuxiliaries,
                    mission.MissionType,
                    _segment.VehicleClass,
                    Vehicle.Length ?? mission.BusParameter.VehicleLength,
                    Vehicle.Components.AxleWheels.NumSteeredAxles,
                    Vehicle.VehicleType,
					runData.BatteryOnlyHybridMode);

                runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(mission, InputDataProvider.JobInputData.Vehicle, runData);

                runData.AirdragData = DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);

                runData.OVCMode = ovcMode;

                return runData;
            }

            protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
            {
                throw new NotImplementedException();
            }

            protected override bool AxleGearRequired()
            {
                throw new NotImplementedException();
            }
        }

		public class MultiplePEV : MultiplePowertrainsBase
		{
			public MultiplePEV(
				IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
                IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder)
				:
				base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder)
			{ }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var missions = GetMissions();

				foreach (var mission in missions)
				{
					foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true))
					{
						yield return CreateVectoRunData(mission, loading);
					}
				}
			}

            protected override VectoRunData CreateVectoRunData(
                Mission mission,
                KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
                int? modeIdx = null,
                OvcHevMode ovcMode = OvcHevMode.NotApplicable)
            {
                var runData = CreateCommonMultiplePowertrainsVectoRunData(mission, loading, null, modeIdx, ovcMode);

                runData.BatteryData.Batteries.ForEach(t => t.Item2.ChargeDepletingBattery = true);

                return runData;
            }
        }

        public class MultipleFCHV : MultiplePowertrainsBase
        {
            public MultipleFCHV(
                IDeclarationInputDataProvider dataProvider,
                IDeclarationReport report,
                IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
                IDeclarationCycleFactory cycleFactory,
                IMissionFilter missionFilter,
                IPowertrainBuilder ptBuilder)
                :
                base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder)
            { }

            protected override IEnumerable<VectoRunData> GetNextRun()
            {
                var missions = GetMissions();

                foreach (var mission in missions)
                {
                    foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true))
                    {
                        var ovcMode = InputDataProvider.JobInputData.Vehicle.OVC ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;

                        yield return CreateVectoRunData(mission, loading, null, ovcMode);
                    }
                }
            }

            protected override VectoRunData CreateVectoRunData(
                Mission mission,
                KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
                int? modeIdx = null,
                OvcHevMode ovcMode = OvcHevMode.NotApplicable)
            {
                var runData = CreateCommonMultiplePowertrainsVectoRunData(mission, loading, null, modeIdx, ovcMode);

                runData.BatteryData = new FCHVDeclarationDataAdapter(DataProvider.DataSource).CreateFuelCellPreProcessingBattery(
                    DataAdapter.CreateFuelCells(Vehicle.Components.FuelCellSystem).ConvertToEngineeringData(),
                    runData.BatteryData,
                    out var fcBatteries);

                runData.ModFileSuffix += "_pre";

                runData.IterativeRunStrategy = DeclarationFuelCellIterativeStrategy.SetUpFuelCellIterativeRunStrategy(
                    runData,
                    InputDataProvider.JobInputData.Vehicle,
                    DataAdapter,
                    InputDataProvider,
                    runData.JobType,
					fcBatteries);

                runData.BatteryData.Batteries.ForEach(t => t.Item2.ChargeDepletingBattery = true);

                return runData;
            }
        }

        public class MultipleSHEV : MultiplePowertrainsBase
        {
            public MultipleSHEV(
                IDeclarationInputDataProvider dataProvider,
                IDeclarationReport report,
                IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
                IDeclarationCycleFactory cycleFactory,
                IMissionFilter missionFilter,
                IPowertrainBuilder ptBuilder)
                :
                base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder)
            { }

            protected override IEnumerable<VectoRunData> GetNextRun()
            {
                var vehicle = InputDataProvider.JobInputData.Vehicle;
                var missions = GetMissions();

                for (var modeIdx = 0; modeIdx < vehicle.Components.EngineInputData.EngineModes.Count; modeIdx++)
                {
                    foreach (var mission in missions)
                    {
                        foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true))
                        {
                            if (vehicle.OVC)
                            {
                                yield return CreateVectoRunData(mission, loading, modeIdx, OvcHevMode.ChargeDepleting);
                                yield return CreateVectoRunData(mission, loading, modeIdx, OvcHevMode.ChargeSustaining);
                            }
                            else
                            {
                                yield return CreateVectoRunData(mission, loading, modeIdx, OvcHevMode.ChargeSustaining);
                            }
                        }
                    }
                }
            }

            protected override VectoRunData CreateVectoRunData(
                Mission mission,
                KeyValuePair<LoadingType,
                Tuple<Kilogram, double?>> loading,
                int? modeIdx = null,
                OvcHevMode ovcMode = OvcHevMode.NotApplicable)
            {
                var vehicle = InputDataProvider.JobInputData.Vehicle;
                var engine = vehicle.Components.EngineInputData;

                var runData = CreateCommonMultiplePowertrainsVectoRunData(mission, loading, engine.EngineModes, modeIdx, ovcMode);

                runData.EngineData = DataAdapter.CreateEngineData(Vehicle, engine.EngineModes[modeIdx.Value], mission);

                runData.HybridStrategyParameters = DataAdapter.CreateHybridStrategy(
                    runData.BatteryData,
                    runData.SuperCapData,
                    runData.VehicleData.TotalVehicleMass,
                    ovcMode,
                    loading.Key,
                    runData.VehicleData.VehicleClass,
                    mission.MissionType);

                var generatorData = DataAdapter.CreateElectricMachine(
                    vehicle.Components.Generator,
                    vehicle.ElectricMotorTorqueLimits,
                    runData.BatteryData?.CalculateVoltageCenterSoc(),
                    Constants.NOT_IN_AXLE_POWERTRAIN);

                runData.ElectricMachinesSinglePwt = new List<Tuple<PowertrainPosition, ElectricMotorData>>() { generatorData };

                if ((runData.BatteryData != null) && (ovcMode != OvcHevMode.NotApplicable))
                {
                    runData.BatteryData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;

                    if (ovcMode == OvcHevMode.ChargeDepleting)
                    {
                        runData.BatteryData.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
                    }
                }

                if ((runData.SuperCapData != null) && (ovcMode != OvcHevMode.NotApplicable))
                {
                    runData.SuperCapData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
                }

                if ((ovcMode != OvcHevMode.NotApplicable) && vehicle.OVC)
                {
                    runData.ModFileSuffix += (ovcMode == OvcHevMode.ChargeSustaining) ? "CS" : "CD";
                }

                return runData;
            }
        }

        public class Conventional : PrimaryBusBase
		{
			public Conventional(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = DataProvider.JobInputData.Vehicle;
				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var missions = GetMissions();

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					foreach (var mission in missions) {
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
				simulationRunData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, InputDataProvider.JobInputData.Vehicle);
				simulationRunData.AirdragData = DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				simulationRunData.EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission);
				simulationRunData.ElectricMachinesSinglePwt = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				simulationRunData.AxleGearSinglePwt = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				simulationRunData.AngledriveSinglePwt = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				simulationRunData.Aux = DataAdapter.CreateAuxiliaryData(
					Vehicle.Components.AuxiliaryInputData,
					Vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					Vehicle.Length ?? mission.BusParameter.VehicleLength,
					Vehicle.Components.AxleWheels.NumSteeredAxles, Vehicle.VehicleType, false);
				simulationRunData.DriverData = DriverData;


				simulationRunData.EngineData.FuelMode = modeIdx.Value;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;

				CreateGearboxAndGearshiftData(simulationRunData);

				simulationRunData.RetarderSinglePwt = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);
				simulationRunData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, simulationRunData);

				return simulationRunData;
			}

			#region Overrides of PrimaryBusBase

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData);

				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						Vehicle.EngineIdleSpeed,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count);

			}

			protected override bool AxleGearRequired()
			{
				return true;
			}

			#endregion
		}

		public abstract class Hybrid : PrimaryBusBase
		{
			protected Hybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = DataProvider.JobInputData.Vehicle;
				var engine = vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var missions = GetMissions();

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					foreach (var mission in missions) {
						foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {

							if (vehicle.OVC) {
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
			protected SerialHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				//CheckMaxChargingPowerPresent(vehicle);
				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx.Value];
				var runData = CreateCommonRunData(mission, loading, _segment, engineModes, modeIdx.Value);

				if (ovcMode == OvcHevMode.ChargeDepleting) {
					runData.BatteryOnlyHybridMode = Vehicle.BatteryOnlyMode;
				}

                runData.DriverData = DriverData;
				runData.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);

				runData.EngineData = DataAdapter.CreateEngineData(Vehicle, engineMode, mission);

				DataAdapter.CreateREESSData(Vehicle.Components.ElectricStorage, Vehicle.VehicleType, Vehicle.OVC, Vehicle.BatteryOnlyMode,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));

				runData.ElectricMachinesSinglePwt = DataAdapter.CreateElectricMachines(
					Vehicle.Components.ElectricMachines, Vehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateVoltageCenterSoc());

				if (Vehicle.VehicleType == VectoSimulationJobType.IEPC_S) {
					var iepcData = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						runData.BatteryData.CalculateVoltageCenterSoc());
					iepcData.ForEach(iepc => runData.ElectricMachinesSinglePwt.Add(iepc));
				}

				if (AxleGearRequired()) {
					runData.AxleGearSinglePwt = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				CreateGearboxAndGearshiftData(runData);
				runData.RetarderSinglePwt = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);

				runData.Aux = DataAdapter.CreateAuxiliaryData(Vehicle.Components.AuxiliaryInputData,
					Vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, Vehicle.Length, Vehicle.Components.AxleWheels.NumSteeredAxles,
					VectoSimulationJobType.SerialHybridVehicle, runData.BatteryOnlyHybridMode);
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

				if (ovcMode != OvcHevMode.NotApplicable && runData.InputData.JobInputData.Vehicle.OVC) {
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
				runData.GearshiftParametersSinglePwt = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}

			protected override bool AxleGearRequired()
			{
				return InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData != null;
			}
		}


		public class HEV_S2 : SerialHybrid
		{
			public HEV_S2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID != ArchitectureID.S2) {
					throw new ArgumentException(nameof(Vehicle));
				}

				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						null,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count);

				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData);

			}
		}

		public class HEV_S3 : SerialHybrid
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

		}

		public class HEV_S4 : SerialHybrid
		{
			public HEV_S4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

		}

		public class HEV_S_IEPC : SerialHybrid
		{
			public HEV_S_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

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
				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						null,
						GearboxType.APTN,
						Vehicle.Components.IEPC.Gears.Count);

				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData, GearboxType.APTN);

			}
		}

		public class FuelCell : BatteryElectric
		{
			public FuelCell(
				IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder)
				: base(
					  dataProvider,
					  report,
					  declarationDataAdapter,
					  cycleFactory,
					  missionFilter,
					  ptBuilder)
			{
			}
			public virtual VectoSimulationJobType FuelCellJobType => VectoSimulationJobType.FCHV;

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = DataProvider.JobInputData.Vehicle;

				var missions = GetMissions();

                foreach (var mission in missions)
				{
					foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true))
					{
						var ovcMode = vehicle.OVC ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;

						var simulationRunData = CreateVectoRunData(mission, loading, null, ovcMode);
						yield return simulationRunData;
					}
				}
			}

			protected override VectoRunData CreateVectoRunData(
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var runData = CreateCommonRunData(mission, loading, _segment);

				DataAdapter.CreateREESSData(
					componentsElectricStorage: Vehicle.Components.ElectricStorage,
					Vehicle.VehicleType,
					Vehicle.OVC,
					Vehicle.BatteryOnlyMode,
					(bs) => runData.BatteryData = bs,
					(sc) => runData.SuperCapData = sc);

				if (Vehicle.VehicleType == VectoSimulationJobType.FCHV_IEPC)
				{
					runData.ElectricMachinesSinglePwt = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						runData.BatteryData.CalculateVoltageCenterSoc());
				}
				else
				{
					runData.ElectricMachinesSinglePwt = DataAdapter.CreateElectricMachines(Vehicle.Components.ElectricMachines,
						Vehicle.ElectricMotorTorqueLimits, runData.BatteryData.CalculateVoltageCenterSoc(), null);
				}

				runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);
				runData.AirdragData = DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				if (AxleGearRequired() || Vehicle.Components.AxleGearInputData != null)
				{
					runData.AxleGearSinglePwt = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				runData.AngledriveSinglePwt = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				runData.Aux = DataAdapter.CreateAuxiliaryData(
					Vehicle.Components.AuxiliaryInputData,
					Vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					Vehicle.Length ?? mission.BusParameter.VehicleLength,
					Vehicle.Components.AxleWheels.NumSteeredAxles, Vehicle.VehicleType, runData.BatteryOnlyHybridMode);
				var emPos = runData.ElectricMachinesSinglePwt.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
				runData.DriverData = DriverData;

				runData.VehicleData.VehicleClass = _segment.VehicleClass;
				CreateGearboxAndGearshiftData(runData);
				runData.RetarderSinglePwt = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);
				runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, runData);

				/// Refer to [1] EngineeringModeVectoRunDataFactory.GetFCHV_RunData():
				/// Comment from [1]:
				///		In case the battery is modified after creating the rundata
				///		(testing, do not create new battery data).
				runData.BatteryData = new FCHVDeclarationDataAdapter(DataProvider.DataSource).CreateFuelCellPreProcessingBattery(
					DataAdapter.CreateFuelCells(Vehicle.Components.FuelCellSystem).ConvertToEngineeringData(),
					runData.BatteryData,
					out var fcBatteries);

				runData.OVCMode = ovcMode;
				runData.ModFileSuffix += "_pre";
				runData.IterativeRunStrategy = DeclarationFuelCellIterativeStrategy.SetUpFuelCellIterativeRunStrategy(
					runData,
                    InputDataProvider.JobInputData.Vehicle,
                    DataAdapter,
					DataProvider,
					FuelCellJobType,
					fcBatteries);
				runData.BatteryData.Batteries.ForEach(t => t.Item2.ChargeDepletingBattery = true);

				return runData;

			}

			protected override bool AxleGearRequired()
			{
				return InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData != null &&
					(InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E4
					&& InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.F4);
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID.IsOneOf(ArchitectureID.S2, ArchitectureID.S_IEPC))
				{
					throw new ArgumentException(nameof(Vehicle.ArchitectureID));
				}
				runData.GearshiftParametersSinglePwt = new ShiftStrategyParameters()
				{
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}
		}

		public class HEV_F2 : FuelCell
		{
			public HEV_F2(
				IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder)
				: base(
					  dataProvider,
					  report,
					  declarationDataAdapter,
					  cycleFactory,
					  missionFilter,
					  ptBuilder) { }

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID != ArchitectureID.F2 && Vehicle.VehicleType == FuelCellJobType)
				{
					throw new ArgumentException(nameof(Vehicle));
				}

				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						null,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count);


				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData);

			}
		}

		public class HEV_F3 : FuelCell
		{
			public HEV_F3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(
					  dataProvider,
					  report,
					  declarationDataAdapter,
					  cycleFactory,
					  missionFilter,
					  ptBuilder) { }

		}

		public class HEV_F4 : FuelCell
		{
			public HEV_F4(
				IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder)
				: base(
					  dataProvider,
					  report,
					  declarationDataAdapter,
					  cycleFactory,
					  missionFilter,
					  ptBuilder) { }

		}

		public class HEV_F_IEPC : FuelCell
		{
			public HEV_F_IEPC(
				IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder)
				: base(
					  dataProvider,
					  report,
					  declarationDataAdapter,
					  cycleFactory,
					  missionFilter,
					  ptBuilder) { }

			public override VectoSimulationJobType FuelCellJobType => VectoSimulationJobType.FCHV_IEPC;

			protected override VectoRunData CreateVectoRunData(
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				AxleGearRequired();
				return base.CreateVectoRunData(
					mission,
					loading,
					modeIdx,
					ovcMode);
			}

			protected override bool AxleGearRequired()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				var iepcInput = vehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
				if (axleGearRequired && vehicle.Components.AxleGearInputData == null)
				{
					throw new VectoException(
						$"Axlegear reqhired for selected type of IEPC! DifferentialIncluded: {iepcInput.DifferentialIncluded}, DesignTypeWheelMotor: {iepcInput.DesignTypeWheelMotor}");
				}

				var numGearsPowermap =
					iepcInput.VoltageLevels.Select(x => Tuple.Create(x.VoltageLevel, x.PowerMap.Count)).ToArray();
				var gearCount = iepcInput.Gears.Count;
				var numGearsDrag = iepcInput.DragCurves.Count;

				if (numGearsPowermap.Any(x => x.Item2 != gearCount))
				{
					throw new VectoException(
						$"Number of gears for voltage levels does not match! PowerMaps: {numGearsPowermap.Select(x => $"{x.Item1}: {x.Item2}").Join()}; Gear count: {gearCount}");
				}

				if (numGearsDrag > 1 && numGearsDrag != gearCount)
				{
					throw new VectoException(
						$"Number of gears drag curve does not match gear count! DragCurve {numGearsDrag}; Gear count: {gearCount}");
				}

				return axleGearRequired;
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						null,
						GearboxType.APTN,
						Vehicle.Components.IEPC.Gears.Count);

				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData, GearboxType.APTN);
			}
		}

		public abstract class ParallelHybrid : Hybrid
		{
			protected ParallelHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

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

				if (ovcMode == OvcHevMode.ChargeDepleting) {
					runData.BatteryOnlyHybridMode = Vehicle.BatteryOnlyMode;
				}

				runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);
				runData.AirdragData = DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				runData.EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission);
				DataAdapter.CreateREESSData(Vehicle.Components.ElectricStorage, Vehicle.VehicleType, Vehicle.OVC, Vehicle.BatteryOnlyMode,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));
				runData.ElectricMachinesSinglePwt = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				runData.AxleGearSinglePwt = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				runData.AngledriveSinglePwt = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				runData.Aux = DataAdapter.CreateAuxiliaryData(
					Vehicle.Components.AuxiliaryInputData,
					Vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					Vehicle.Length ?? mission.BusParameter.VehicleLength,
					Vehicle.Components.AxleWheels.NumSteeredAxles,
					VectoSimulationJobType.ParallelHybridVehicle, runData.BatteryOnlyHybridMode);//Hardcode to override IHPC

				if (runData.BatteryOnlyHybridMode && runData.Aux.Any(x => x.ID != Constants.Auxiliaries.IDs.Fan && !x.ConnectToREESS)) {
					throw new VectoException(
						"Vehicles with a battery dominant mode are required to have electrically powered auxiliaries");
				}
				runData.DriverData = DriverData;


				runData.EngineData.FuelMode = modeIdx.Value;
				runData.VehicleData.VehicleClass = _segment.VehicleClass;

                var gearboxType = InputDataProvider.JobInputData.Vehicle.Components.GetGearboxTypes().FirstOrDefault();
				var gearlist = gearboxType == GearboxType.IHPC
					? new GearList(InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.Gears
						.Select(x => new GearshiftPosition((uint)x.Gear)).ToArray())
					: null;

                runData.ElectricMachinesSinglePwt = DataAdapter.CreateElectricMachines(
					Vehicle.Components.ElectricMachines, 
					Vehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateVoltageCenterSoc(), 
					gearlist);

                CreateGearboxAndGearshiftData(runData);
                
                runData.RetarderSinglePwt = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);
				runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, InputDataProvider.JobInputData.Vehicle, runData);

				if (runData.BatteryOnlyHybridMode) {
					EnsureElectricBusAux(runData.BusAuxiliaries);
				}

				runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData,
						runData.SuperCapData,
						runData.VehicleData.TotalVehicleMass,
						ovcMode, loading.Key,
						//runData.VehicleData.VehicleClass,
						mission.BusParameter.BusGroup,
						mission.MissionType, Vehicle.BoostingLimitations, runData.GearboxSinglePwt, runData.EngineData, runData.ElectricMachinesSinglePwt, Vehicle.ArchitectureID);

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
				if (ovcMode != OvcHevMode.NotApplicable && runData.InputData.JobInputData.Vehicle.OVC) {
					runData.ModFileSuffix += ovcMode == OvcHevMode.ChargeSustaining ? "CS" : "CD";
				}
				runData.OVCMode = ovcMode;

				return runData;
			}

			private void EnsureElectricBusAux(IAuxiliaryConfig busAux)
			{
				if (busAux.InputData.PneumaticSupply.CompressorDrive != CompressorDrive.electrically) {
					throw new VectoException("Compressor drive is required to be electrically driven!");
				}

				if (busAux.InputData.ElectricSupply.ESSupplyFromHEVREESS) {
					throw new VectoException("Electric system must be supplied from HV REESS!");
				}
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						Vehicle.EngineIdleSpeed,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count);

				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData);

			}

			protected override bool AxleGearRequired()
			{
				return true;
			}
		}

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

		}

		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

		}

		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }


		}

		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

		}

		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

		}

		public class HEV_P_IHPC : HEV_P2
		{
			public HEV_P_IHPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter,
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public abstract class BatteryElectric : PrimaryBusBase
		{
			public BatteryElectric(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of PrimaryBusBase

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				var missions = GetMissions();

				foreach (var mission in missions) {
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
					Vehicle.OVC,
					Vehicle.BatteryOnlyMode,
					(bs) => result.BatteryData = bs,
					(sc) => result.SuperCapData = sc);


				if (Vehicle.VehicleType == VectoSimulationJobType.IEPC_E) {
					result.ElectricMachinesSinglePwt = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						result.BatteryData.CalculateVoltageCenterSoc());
				} else {
					result.ElectricMachinesSinglePwt = DataAdapter.CreateElectricMachines(Vehicle.Components.ElectricMachines,
						Vehicle.ElectricMotorTorqueLimits, result.BatteryData.CalculateVoltageCenterSoc(), null);
				}

				result.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				result.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);
				result.AirdragData = DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				if (AxleGearRequired() || Vehicle.Components.AxleGearInputData != null) {
					result.AxleGearSinglePwt = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				result.AngledriveSinglePwt = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				result.Aux = DataAdapter.CreateAuxiliaryData(
					Vehicle.Components.AuxiliaryInputData,
					Vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					Vehicle.Length ?? mission.BusParameter.VehicleLength,
					Vehicle.Components.AxleWheels.NumSteeredAxles, Vehicle.VehicleType, false);
				var emPos = result.ElectricMachinesSinglePwt.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
				result.DriverData = DriverData;

				result.VehicleData.VehicleClass = _segment.VehicleClass;
				CreateGearboxAndGearshiftData(result);
				result.RetarderSinglePwt = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);
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
				runData.GearshiftParametersSinglePwt = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}
		}


		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of PrimaryBusBase

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(Vehicle));
				}

				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						null,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count);

				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData);

			}
			#endregion
		}

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }


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
				var axleGearRatio = axleGearRequired ? runData.AxleGearSinglePwt.AxleGear.Ratio : 1.0;
				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						axleGearRatio,
						null,
						GearboxType.APTN,
						Vehicle.Components.IEPC.Gears.Count
					);

				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData, GearboxType.APTN);

			}
		}

		public class Exempted : PrimaryBusBase
		{
			public Exempted(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

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
					VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, _segment,
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
		}
	}
}