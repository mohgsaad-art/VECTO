using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.SingleBus
{

    public abstract class DeclarationModeSingleBusRunDataFactory
	{
		public abstract class SingleBusBase : AbstractDeclarationVectoRunDataFactory
        {
			protected ISingleBusDeclarationDataAdapter DataAdapter { get; }

			public ISingleBusInputDataProvider SingleBusDataProvider { get; }

			protected virtual IVehicleDeclarationInputData PrimaryVehicle => SingleBusDataProvider.PrimaryVehicle;

			protected virtual IVehicleDeclarationInputData CompletedVehicle => SingleBusDataProvider.CompletedVehicle;

			protected override IVehicleDeclarationInputData Vehicle => throw new NotImplementedException();

			protected SingleBusBase(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) : base(dataProvider, report, cycleFactory, missionFilter, true, ptBuilder)
			{
				DataAdapter = dataAdapter;
				Report = report;
				SingleBusDataProvider = dataProvider;
			}

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override void Initialize()
			{
				var vehicle = SingleBusDataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle) {
					return;
				}

				_segment = GetSegment();
				
			}

			protected virtual VectoRunData CreateCommonRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
                var cycle = CycleFactory.GetDeclarationCycle(mission);

                CheckSuperCap(SingleBusDataProvider.PrimaryVehicle);
				
				var simulationRunData = new VectoRunData {
					InputData = SingleBusDataProvider,
					Loading = loading.Key,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					DriverData = DriverData,
					ExecutionMode = ExecutionMode.Declaration,
					JobName = SingleBusDataProvider.JobInputData.Vehicle
						.Identifier, //?!? Jobname
					JobType = PrimaryVehicle.VehicleType,
					ModFileSuffix = $"_{_segment.VehicleClass.GetClassNumber()}_{loading.Key}",
					Report = Report,
					Mission = mission,
					InputDataHash = SingleBusDataProvider.XMLHash, // right hash?!?
					SimulationType = SimulationType.DistanceCycle,
					VehicleDesignSpeed = _segment.DesignSpeed,
					MaxChargingPower = SingleBusDataProvider.PrimaryVehicle.MaxChargingPower,
				};

				return simulationRunData;
            }

			#endregion
			protected Segment GetSegment()
			{
				var segment = DeclarationData.CompletedBusSegments.Lookup(
					PrimaryVehicle.AxleConfiguration.NumAxles(), CompletedVehicle.VehicleCode, CompletedVehicle.RegisteredClass, CompletedVehicle.NumberPassengerSeatsLowerDeck,
					CompletedVehicle.Height, CompletedVehicle.LowEntry);
				if (!segment.Found)
				{
					throw new VectoException(
						"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, vehicle code: {3}, registered class: {4}, passengersLowerDeck: {5}, height: {6}, lowfloor: {7}. completed",
						PrimaryVehicle.VehicleCategory, PrimaryVehicle.AxleConfiguration,
						PrimaryVehicle.Articulated, CompletedVehicle.VehicleCode, CompletedVehicle.RegisteredClass.GetLabel(), CompletedVehicle.NumberPassengerSeatsLowerDeck,
						CompletedVehicle.Height, CompletedVehicle.LowEntry);
				}
				foreach (var mission in segment.Missions)
				{
					mission.VehicleHeight = CompletedVehicle.Height + mission.BusParameter.DeltaHeight;
					mission.BusParameter.VehicleLength = CompletedVehicle.Length;
				}
				return segment;
			}

			protected override DriverData CreateDriverData(Segment segment)
			{
				
				return DataAdapter.CreateBusDriverData(segment, 
					jobType: PrimaryVehicle.VehicleType, 
					arch: PrimaryVehicle.ArchitectureID, 
					compressorDrive: PrimaryVehicle.Components.BusAuxiliaries.PneumaticSupply.CompressorDrive);
			}

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				return GetNextRun().First(x => x != null);
			}

            protected abstract void CreateGearboxAndGearshiftData(VectoRunData runData);

			protected abstract bool AxleGearRequired();
        }

		

		public class Conventional : SingleBusBase
		{
			public Conventional(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var doubleDecker = CompletedVehicle.NumberPassengerSeatsUpperDeck > 0;
                if (mission.BusParameter.DoubleDecker != doubleDecker) {
                    return null;
                }

                var engine = PrimaryVehicle.Components.EngineInputData;
                var engineModes = engine.EngineModes;
                var engineMode = engineModes[modeIdx.Value];

                //var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

				var runData = CreateCommonRunData(mission, loading);

                runData.VehicleData = DataAdapter.CreateVehicleData(SingleBusDataProvider, _segment, mission, loading, _allowVocational); //Primary
				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, PrimaryVehicle);
				runData.AirdragData = DataAdapter.CreateAirdragData(CompletedVehicle, mission, _segment, ovcMode); //Single
				runData.EngineData = DataAdapter.CreateEngineData(PrimaryVehicle, engineMode, mission); //Primary
				runData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				runData.AxleGearData = DataAdapter.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
				runData.AngledriveData =
					DataAdapter.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
				runData.Aux = DataAdapter.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, PrimaryVehicle.Length ?? mission.BusParameter.VehicleLength,
					PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);
				
				runData.EngineData.FuelMode = modeIdx.Value;
				runData.VehicleData.VehicleClass = _segment.VehicleClass;
				CreateGearboxAndGearshiftData(runData);
				runData.Retarder = DataAdapter.CreateRetarderData(PrimaryVehicle.Components.RetarderInputData, PrimaryVehicle.ArchitectureID, PrimaryVehicle.Components.IEPC);
				runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, runData);
				
                return runData;
            }

			

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				var primaryVehicle = SingleBusDataProvider.PrimaryVehicle;
               
				runData.GearboxData = DataAdapter.CreateGearboxData(primaryVehicle, runData);
				var gbxInput = primaryVehicle.Components.GearboxInputData;
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						primaryVehicle.EngineIdleSpeed,
						gbxInput.Type,
						gbxInput.Gears.Count
					);
			}

			protected override bool AxleGearRequired()
			{
				return true;
			}

			protected override IEnumerable<VectoRunData> GetNextRun()
			{

				var vehicle = SingleBusDataProvider.JobInputData.Vehicle;

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

        }

        public abstract class Hybrid : SingleBusBase
		{
			protected Hybrid(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var engineModes = SingleBusDataProvider.PrimaryVehicle.Components.EngineInputData
					?.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					
					foreach (var mission in _segment.Missions) {
						foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {
							if (SingleBusDataProvider.PrimaryVehicle.OVC) {
								if (SingleBusDataProvider.PrimaryVehicle.MaxChargingPower != null &&
									SingleBusDataProvider.PrimaryVehicle.MaxChargingPower.IsEqual(0))
								{
									throw new VectoException("MaxChargingPower has to be greater than 0 if OVC is selected");
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
			protected SerialHybrid(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
            {
                //CheckMaxChargingPowerPresent(vehicle);
                var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
                var engineModes = engine.EngineModes;
                var engineMode = engineModes[modeIdx.Value];
                var runData = CreateCommonRunData(mission, loading);


                runData.DriverData = DriverData;
                runData.AirdragData =
                    DataAdapter.CreateAirdragData(CompletedVehicle, mission, _segment, ovcMode);
                runData.VehicleData = DataAdapter.CreateVehicleData(SingleBusDataProvider, _segment, mission, loading, _allowVocational);
				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, PrimaryVehicle);

                runData.EngineData = DataAdapter.CreateEngineData(PrimaryVehicle, engineMode, mission);

                DataAdapter.CreateREESSData(PrimaryVehicle.Components.ElectricStorage, PrimaryVehicle.VehicleType, PrimaryVehicle.OVC,
                    ((batteryData) => runData.BatteryData = batteryData),
                    ((sCdata => runData.SuperCapData = sCdata)));

                runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(
					PrimaryVehicle.Components.ElectricMachines, PrimaryVehicle.ElectricMotorTorqueLimits,
                    runData.BatteryData.CalculateVoltageCenterSoc());

                if (PrimaryVehicle.VehicleType == VectoSimulationJobType.IEPC_S) {
                    var iepcData = DataAdapter.CreateIEPCElectricMachines(PrimaryVehicle.Components.IEPC,
                        runData.BatteryData.CalculateVoltageCenterSoc());
                    iepcData.ForEach(iepc => runData.ElectricMachinesData.Add(iepc));
                }

                if (AxleGearRequired()) {
                    runData.AxleGearData = DataAdapter.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
                }

				CreateGearboxAndGearshiftData(runData);
				runData.Retarder = DataAdapter.CreateRetarderData(PrimaryVehicle.Components.RetarderInputData, PrimaryVehicle.ArchitectureID, PrimaryVehicle.Components.IEPC);

                runData.Aux = DataAdapter.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType,
                    _segment.VehicleClass, PrimaryVehicle.Length, PrimaryVehicle.Components.AxleWheels.NumSteeredAxles,
                    VectoSimulationJobType.SerialHybridVehicle);
                runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
                    mission, PrimaryVehicle, CompletedVehicle , runData);

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
                if (SingleBusDataProvider.PrimaryVehicle.ArchitectureID.IsOneOf(ArchitectureID.S2, ArchitectureID.S_IEPC)) {
                    throw new ArgumentException(nameof(SingleBusDataProvider.PrimaryVehicle.ArchitectureID));
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
        }
		public  class HEV_S2 : SerialHybrid
		{
			public HEV_S2(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (SingleBusDataProvider.PrimaryVehicle.ArchitectureID != ArchitectureID.S2) {
					throw new ArgumentException(nameof(SingleBusDataProvider.PrimaryVehicle));
				}

				var primaryVehicle = SingleBusDataProvider.PrimaryVehicle;
				var gbxInput = primaryVehicle.Components.GearboxInputData;
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						primaryVehicle.EngineIdleSpeed,
						gbxInput.Type,
						gbxInput.Gears.Count
					);
				runData.GearboxData = DataAdapter.CreateGearboxData(primaryVehicle, runData);
				
			}
        }
        
		public class HEV_S3 : SerialHybrid
		{
			public HEV_S3(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_S4 : SerialHybrid
		{
			public HEV_S4(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}


		public class HEV_S_IEPC : SerialHybrid
		{
			public HEV_S_IEPC(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

            protected override VectoRunData CreateVectoRunData(Mission mission,
                KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
                int? modeIdx, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
            {
                AxleGearRequired();
                return base.CreateVectoRunData(mission, loading, modeIdx, ovcMode);
            }

            protected override bool AxleGearRequired()
            {
                //var vehicle = InputDataProvider.JobInputData.Vehicle;
                var iepcInput = PrimaryVehicle.Components.IEPC;
                var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
                if (axleGearRequired && PrimaryVehicle.Components.AxleGearInputData == null) {
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
						PrimaryVehicle.Components.IEPC.Gears.Count);

                runData.GearboxData = DataAdapter.CreateGearboxData(PrimaryVehicle, runData, GearboxType.APTN);

            }
        }

        public abstract class ParallelHybrid : Hybrid
		{
			protected ParallelHybrid(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var engine = PrimaryVehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				if (!modeIdx.HasValue) {
					throw new VectoException("Engine mode has to be specified for parallel hybrid vehicle");
				}

				var engineMode = engineModes[modeIdx.Value];

				var runData = CreateCommonRunData(mission, loading);

				runData.VehicleData =
					DataAdapter.CreateVehicleData(SingleBusDataProvider, _segment, mission, loading, _allowVocational);

				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, PrimaryVehicle);
				runData.AirdragData = DataAdapter.CreateAirdragData(CompletedVehicle, mission, _segment, ovcMode);
				runData.EngineData =
					DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission);
				DataAdapter.CreateREESSData(PrimaryVehicle.Components.ElectricStorage, PrimaryVehicle.VehicleType, PrimaryVehicle.OVC,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));
				runData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				runData.AxleGearData = DataAdapter.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
				runData.AngledriveData = DataAdapter.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
				runData.Aux = DataAdapter.CreateAuxiliaryData(
					PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					PrimaryVehicle.Length ?? mission.BusParameter.VehicleLength,
					PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);
				runData.DriverData = DriverData;

				runData.EngineData.FuelMode = modeIdx.Value;
				runData.VehicleData.VehicleClass = _segment.VehicleClass;
				
				CreateGearboxAndGearshiftData(runData);
				runData.Retarder = DataAdapter.CreateRetarderData(PrimaryVehicle.Components.RetarderInputData, PrimaryVehicle.ArchitectureID, PrimaryVehicle.Components.IEPC);
				runData.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
					mission, PrimaryVehicle, CompletedVehicle, runData);

				runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(
					PrimaryVehicle.Components.ElectricMachines, PrimaryVehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateVoltageCenterSoc(), runData.GearboxData.GearList);

				runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData,
						runData.SuperCapData,
						runData.VehicleData.TotalVehicleMass,
						ovcMode, loading.Key,
						//runData.VehicleData.VehicleClass,
						mission.BusParameter.BusGroup,
						mission.MissionType, PrimaryVehicle.BoostingLimitations, runData.GearboxData, runData.EngineData,
						runData.ElectricMachinesData,
						PrimaryVehicle.ArchitectureID);

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

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
            {
				runData.GearboxData = DataAdapter.CreateGearboxData(PrimaryVehicle, runData);
				var gbxInput = PrimaryVehicle.Components.GearboxInputData;
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						PrimaryVehicle.EngineIdleSpeed,
						gbxInput.Type,
						gbxInput.Gears.Count
					);
			}

            protected override bool AxleGearRequired()
            {
                return true;
            }
        }

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P_IHPC : ParallelHybrid
		{
			public HEV_P_IHPC(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public abstract class BatteryElectric : SingleBusBase
		{
			protected BatteryElectric(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				foreach (var mission in _segment.Missions) {
					foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {
						var run = CreateVectoRunData(mission, loading);
						run.BatteryData.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
						yield return run;
					}
				}
			}

            protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var vehicle = SingleBusDataProvider.PrimaryVehicle;

                var result = CreateCommonRunData(mission, loading);

                DataAdapter.CreateREESSData(
                    componentsElectricStorage: SingleBusDataProvider.PrimaryVehicle.Components.ElectricStorage,
                    vehicle.VehicleType,
                    true,
                    (bs) => result.BatteryData = bs,
                    (sc) => result.SuperCapData = sc);


                if (vehicle.VehicleType == VectoSimulationJobType.IEPC_E) {
                    result.ElectricMachinesData = DataAdapter.CreateIEPCElectricMachines(vehicle.Components.IEPC,
                        result.BatteryData.CalculateVoltageCenterSoc());
                } else {
                    result.ElectricMachinesData = DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines,
                        vehicle.ElectricMotorTorqueLimits, result.BatteryData.CalculateVoltageCenterSoc(), null);
                }

                result.VehicleData = DataAdapter.CreateVehicleData(SingleBusDataProvider, _segment, mission, loading, _allowVocational);
				result.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, vehicle);

				result.AirdragData = DataAdapter.CreateAirdragData(SingleBusDataProvider.CompletedVehicle, mission, _segment, ovcMode);
                if (AxleGearRequired() || vehicle.Components.AxleGearInputData != null) {
                    result.AxleGearData = DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
                }

                result.AngledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
                result.Aux = DataAdapter.CreateAuxiliaryData(
                    vehicle.Components.AuxiliaryInputData,
                    vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
                    vehicle.Length ?? mission.BusParameter.VehicleLength,
                    vehicle.Components.AxleWheels.NumSteeredAxles, vehicle.VehicleType);
                result.DriverData = DriverData;

                result.VehicleData.VehicleClass = _segment.VehicleClass;
                CreateGearboxAndGearshiftData(result);
                result.Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData, vehicle.ArchitectureID, vehicle.Components.IEPC);
                result.BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(
                    mission, SingleBusDataProvider.PrimaryVehicle, SingleBusDataProvider.CompletedVehicle, result);

                return result;
            }
            protected override bool AxleGearRequired()
            {
                return InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E4;
            }

            protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
            {
                if (SingleBusDataProvider.PrimaryVehicle.ArchitectureID == ArchitectureID.E2) {
                    throw new ArgumentException();
                }
                runData.GearshiftParameters = new ShiftStrategyParameters() {
                    StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
                    StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
                };
            }
        }

		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (SingleBusDataProvider.PrimaryVehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(SingleBusDataProvider.PrimaryVehicle));
				}

				var gbxInput = PrimaryVehicle.Components.GearboxInputData;
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null,
						gbxInput.Type,
						gbxInput.Gears.Count
					);
				runData.GearboxData = DataAdapter.CreateGearboxData(PrimaryVehicle, runData);
				
			}
        }

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			protected override bool AxleGearRequired()
			{
				var iepcInput = PrimaryVehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
				if (axleGearRequired && PrimaryVehicle.Components.AxleGearInputData == null) {
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

				return axleGearRequired; // || PrimaryVehicle.Components.AxleGearInputData != null;

			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				var iepcInput = PrimaryVehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
				var axleGearRatio = axleGearRequired ? runData.AxleGearData.AxleGear.Ratio : 1.0;
                runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						axleGearRatio,
						null,
						GearboxType.APTN,
						PrimaryVehicle.Components.IEPC.Gears.Count
					);
				runData.GearboxData = DataAdapter.CreateGearboxData(PrimaryVehicle, runData, GearboxType.APTN);

			}
        }


		public class Exempted : SingleBusBase
		{
			public Exempted(ISingleBusInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
                ISingleBusDeclarationDataAdapter dataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
				: base(dataProvider, report, dataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of SingleBusBase

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				throw new NotImplementedException("Exempted SingleBus Simulation is not supported!");
			}

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				throw new NotImplementedException();
			}

			#endregion

			#region Overrides of SingleBusBase

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				throw new NotImplementedException();
			}

			protected override bool AxleGearRequired()
			{
				throw new NotImplementedException();
			}

			#endregion
		}


	}

}
