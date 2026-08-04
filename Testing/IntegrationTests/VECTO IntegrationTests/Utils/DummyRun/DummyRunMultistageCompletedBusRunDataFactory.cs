using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

internal class DummyRunMultistageCompletedBusRunDataFactory : DeclarationModeCompletedBusRunDataFactory.CompletedBusBase
{

    public DummyRunMultistageCompletedBusRunDataFactory(IMultistageVIFInputData dataProvider,
        IDeclarationReport report,
        ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
        IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
        : base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter, ptBuilder)
    {

    }

    protected override void Initialize()
    {
        _segment = GetCompletedSegment();
    }

    protected override IEnumerable<VectoRunData> GetNextRun()
    {
        var InputDataProvider = DataProvider.MultistageJobInputData;
        if (InputDataProvider.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle)
        {
            return new[] { GetExemptedVectoRunData() };
        }
        return VectoRunDataHeavyBusCompleted();
    }

    protected virtual VectoRunData GetExemptedVectoRunData()
    {
        var InputDataProvider = DataProvider.MultistageJobInputData;
        return new VectoRunData()
        {
            Exempted = true,
            VehicleData = new VehicleData()
            {
                ModelName = CompletedVehicle.Model,
                Manufacturer = CompletedVehicle.Manufacturer,
                ManufacturerAddress = CompletedVehicle.ManufacturerAddress,
                VIN = CompletedVehicle.VIN,
                VehicleCategory = CompletedVehicle.VehicleCategory,
                LegislativeClass = CompletedVehicle.LegislativeClass,
                RegisteredClass = CompletedVehicle.RegisteredClass,
                VehicleCode = CompletedVehicle.VehicleCode,
                CurbMass = CompletedVehicle.CurbMassChassis,
                GrossVehicleMass = CompletedVehicle.GrossVehicleMassRating,
                ZeroEmissionVehicle = PrimaryVehicle.ZeroEmissionVehicle,
                MaxNetPower1 = PrimaryVehicle.MaxNetPower1,
                InputData = CompletedVehicle
            },
            Report = Report,
            Mission = new Mission()
            {
                MissionType = MissionType.ExemptedMission
            },
            InputData = InputDataProvider
        };
    }

    protected virtual IEnumerable<VectoRunData> VectoRunDataHeavyBusCompleted()
    {
        if (PrimaryVehicle.VehicleType.IsOneOf(VectoSimulationJobType.IEPC_E, VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.Multiple_PEV, VectoSimulationJobType.FCHV, VectoSimulationJobType.FCHV_IEPC, VectoSimulationJobType.Multiple_FCHV))
        {
            foreach (var vectoRunData in CreateVectoRunDataForMissions(0, ""))
                yield return vectoRunData;
        }
        else
        {
            var engineModes = PrimaryVehicle.Components.EngineInputData
                ?.EngineModes;

            for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
            {
                var fuelMode = "single fuel mode";
                if (engineModes[modeIdx].Fuels.Count > 1)
                {
                    fuelMode = "dual fuel mode";
                }

                foreach (var vectoRunData in CreateVectoRunDataForMissions(modeIdx, fuelMode))
                    yield return vectoRunData;
            }
        }
    }


    protected override VectoRunData CreateVectoRunDataSpecific(Mission mission,
        KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx,
        OvcHevMode ovcMode = OvcHevMode.NotApplicable)
    {
        var cycle = CycleFactory.GetDeclarationCycle(mission);

        var simulationRunData = new VectoRunData
        {
            Loading = loading.Key,
            VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segment,
                mission, loading),
            AirdragData = DummyRunPrimaryBusRunDataFactory.CreateDummyAirdragData(CompletedVehicle),
            EngineData = DummyRunPrimaryBusRunDataFactory.CreateDummyEngineData(PrimaryVehicle, modeIdx, CompletedVehicle.TankSystem),
            //ElectricMachinesData = PrimaryBusMockupRunDataFactory.CreateMockupElectricMachineData()
            Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
            Mission = mission,
            InputData = DataProvider.MultistageJobInputData,
            SimulationType = SimulationType.DistanceCycle,
            ExecutionMode = ExecutionMode.Declaration,
            JobName = DataProvider.MultistageJobInputData.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,
            Report = Report,
            ModFileSuffix = $"_{_segment.VehicleClass.GetClassNumber()}-Specific_{loading.Key}",

            JobType = DataProvider.MultistageJobInputData.JobInputData.JobType
        };
        if (PrimaryVehicle.VehicleType.IsMultiplePowertrains()) {
			simulationRunData.AxlePowertrains = new List<AxlePowertrainData>();
			foreach (var axlePt in PrimaryVehicle.Components.AxlePowertrainInputData) {
				var axlePowertrainData = new AxlePowertrainData() {
					AxleNumber = axlePt.AxleNumber,
					Retarder = DummyRunPrimaryBusRunDataFactory.CreateDummyRetarder(axlePt),
					AxleGearData = DummyRunPrimaryBusRunDataFactory.CreateDummyAxleGearData(axlePt.AxleGearInputData),
					GearboxData = DummyRunPrimaryBusRunDataFactory.CreateDummyGearboxData(axlePt.GearboxInputData),
					AngledriveData = DummyRunPrimaryBusRunDataFactory.CreateDummyAngleDriveData(axlePt.AngledriveInputData)
				};
				simulationRunData.AxlePowertrains.Add(axlePowertrainData);
            }
		} else {
			simulationRunData.RetarderSinglePwt = DummyRunPrimaryBusRunDataFactory.CreateDummyRetarder(PrimaryVehicle);
			simulationRunData.AngledriveSinglePwt = DummyRunPrimaryBusRunDataFactory.CreateDummyAngleDriveData(
					PrimaryVehicle.Components.AngledriveInputData);
			simulationRunData.AxleGearSinglePwt = DummyRunPrimaryBusRunDataFactory.CreateDummyAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
			simulationRunData.GearboxSinglePwt = DummyRunPrimaryBusRunDataFactory.CreateDummyGearboxData(PrimaryVehicle.Components.GearboxInputData);

		}
        if (simulationRunData.EngineData != null) {
            simulationRunData.EngineData.FuelMode = 0;
        }
        if (PrimaryVehicle.ArchitectureID.IsBatteryElectricVehicle() ||
            PrimaryVehicle.ArchitectureID.IsHybridVehicle() ||
			PrimaryVehicle.ArchitectureID.IsFuelCellVehicle())
        {
            simulationRunData.BatteryData = CreateBatteryData();
        }

        simulationRunData.ElectricMachinesSinglePwt = simulationRunData.JobType.IsIEPC()
            ? DummyRunPrimaryBusRunDataFactory.CreateDummyIEPCData()
            : DummyRunPrimaryBusRunDataFactory.CreateDummyElecticMachinesData(PrimaryVehicle.Components.ElectricMachines);

        if (PrimaryVehicle.ArchitectureID.IsFuelCellVehicle()) {
			simulationRunData.VehicleData.H2StorageUsableCapacity = PrimaryVehicle.H2StorageUsableCapacity;
			simulationRunData.OVCMode = PrimaryVehicle.OVC ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
			simulationRunData.VehicleData.H2StorageUsableCapacity = 30.SI<Kilogram>();
			simulationRunData.FuelCellSystemData = new FuelCellSystemData() {
				Fuel = { FuelData.H2 }
			};
			var fchviterativeStrategy =
				new FCHEVIterativeRunStrategy(new[] { new PreRunOptions(), new PreRunOptions() });
			fchviterativeStrategy.Update = UpdateFCHVRunData;

			simulationRunData.IterativeRunStrategy = fchviterativeStrategy;
        }
		if (simulationRunData.EngineData != null && simulationRunData.EngineData.Fuels.Any(x => x.FuelData.FuelType.IsOneOf(FuelType.H2CI, FuelType.H2PI))) {
			simulationRunData.VehicleData.H2StorageUsableCapacity = 30.SI<Kilogram>();
		}

        simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
        simulationRunData.BusAuxiliaries = DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, simulationRunData);

        return simulationRunData;
    }

    protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
    {
        throw new NotImplementedException();
    }

    private IEnumerable<VectoRunData> CreateVectoRunDataForMissions(int modeIdx, string fuelMode)
    {
        var InputDataProvider = DataProvider.MultistageJobInputData;
        var ovc = PrimaryVehicle.OVC;
        foreach (var mission in _segment.Missions)
        {
            foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true))
            {
                var simulationRunData = CreateVectoRunDataSpecific(mission, loading, modeIdx);
                if (ovc)
                {
                    simulationRunData.OVCMode = OvcHevMode.ChargeDepleting;
                    yield return simulationRunData;
                    simulationRunData = CreateVectoRunDataSpecific(mission, loading, modeIdx);
                    simulationRunData.OVCMode = OvcHevMode.ChargeSustaining;
                }
                yield return simulationRunData;


                var primarySegment = GetPrimarySegment();
                var primaryMission = primarySegment.Missions.Where(
                    m =>
                    {
                        return m.BusParameter.DoubleDecker ==
                                CompletedVehicle.VehicleCode.IsDoubleDeckerBus() &&
                                m.MissionType == mission.MissionType &&
                                m.BusParameter.FloorType == CompletedVehicle.VehicleCode.GetFloorType();
                    }).First();

                simulationRunData = CreateVectoRunDataGeneric(
                    primaryMission,
                    new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(loading.Key,
                        primaryMission.Loadings[loading.Key]),
                    primarySegment, modeIdx);
                if (ovc)
                {
                    var primaryResult = GetPrimaryResult(fuelMode, InputDataProvider, simulationRunData, OvcHevMode.ChargeDepleting);
                    simulationRunData.PrimaryResult = primaryResult;
                    simulationRunData.OVCMode = OvcHevMode.ChargeDepleting;
                    yield return simulationRunData;

                    simulationRunData = CreateVectoRunDataGeneric(
                        primaryMission,
                        new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(loading.Key,
                            primaryMission.Loadings[loading.Key]),
                        primarySegment, modeIdx);
                    primaryResult = GetPrimaryResult(fuelMode, InputDataProvider, simulationRunData, OvcHevMode.ChargeSustaining);
                    simulationRunData.OVCMode = OvcHevMode.ChargeSustaining;
                    simulationRunData.PrimaryResult = primaryResult;
                }
                else
                {
                    var primaryResult = GetPrimaryResult(fuelMode, InputDataProvider, simulationRunData, OvcHevMode.NotApplicable);
                    simulationRunData.PrimaryResult = primaryResult;
                }


                yield return simulationRunData;
            }
        }
    }

    private static IResult? GetPrimaryResult(string fuelMode, IMultistepBusInputDataProvider InputDataProvider,
        VectoRunData simulationRunData, OvcHevMode ovc)
    {
        var primaryResult = InputDataProvider.JobInputData.PrimaryVehicle.GetResult(
            simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
            simulationRunData.VehicleData.Loading, ovc);
        if (primaryResult == null)
        {
            throw new VectoException(
                "Failed to find results in PrimaryVehicleReport for vehicle group: {0},  mission: {1}, fuel mode: '{2}', payload: {3}. Make sure PIF and completed vehicle data match!",
                simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
                simulationRunData.VehicleData.Loading);
        }

        if (primaryResult.ResultStatus == ResultStatus.PrimaryRunIgnored)
        {
            throw new VectoException(
                "The vehicle group of the complete(d) vehicle falls into a primary vehicle sub-group for which no result could be calculated due to the criterion of insufficient powertrain power.   mission: {1}, fuel mode: '{2}', payload: {3}.",
                simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
                simulationRunData.VehicleData.Loading);
        }

        if (primaryResult.ResultStatus != ResultStatus.Success)
        {
            throw new VectoException(
                "Simulation results in PrimaryVehicleReport for vehicle group: {0},  mission: {1}, fuel mode: '{2}', payload: {3} not finished successfully.",
                simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
                simulationRunData.VehicleData.Loading);
        }

        return primaryResult;
    }

    protected override VectoRunData CreateVectoRunDataGeneric(Mission mission,
        KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx,
        OvcHevMode ovcHevMode = OvcHevMode.NotApplicable)
    {
        var cycle = CycleFactory.GetDeclarationCycle(mission);

        var runData = new VectoRunData()
        {
            Mission = mission,
            Loading = loading.Key,
            VehicleData = new VehicleData()
            {
                Loading = loading.Value.Item1,
                VehicleClass = primarySegment.VehicleClass,
            },
            EngineData = DummyRunPrimaryBusRunDataFactory.CreateDummyEngineData(PrimaryVehicle, modeIdx, CompletedVehicle.TankSystem),
            JobName = DataProvider.MultistageJobInputData.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,
            ExecutionMode = ExecutionMode.Declaration,
            SimulationType = SimulationType.DistanceCycle,
            Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
            Report = Report,
            ModFileSuffix = $"_{_segment.VehicleClass.GetClassNumber()}-Generic_{loading.Key}",
            InputData = DataProvider.MultistageJobInputData,

            JobType = DataProvider.MultistageJobInputData.JobInputData.JobType
		};
		if (PrimaryVehicle.VehicleType.IsMultiplePowertrains()) {
			runData.AxlePowertrains = new List<AxlePowertrainData>();
			foreach (var axlePt in PrimaryVehicle.Components.AxlePowertrainInputData) {
				var axlePowertrainData = new AxlePowertrainData() {
					GearboxData = DummyRunPrimaryBusRunDataFactory.CreateDummyGearboxData(axlePt.GearboxInputData),
					AxleGearData = DummyRunPrimaryBusRunDataFactory.CreateDummyAxleGearData(axlePt.AxleGearInputData),
				};
                runData.AxlePowertrains.Add(axlePowertrainData);
			}
		} else {
			runData.GearboxSinglePwt = DummyRunPrimaryBusRunDataFactory.CreateDummyGearboxData(PrimaryVehicle.Components.GearboxInputData);
			runData.AxleGearSinglePwt = DummyRunPrimaryBusRunDataFactory.CreateDummyAxleGearData(PrimaryVehicle.Components.AxleGearInputData);

        }
        if (PrimaryVehicle.ArchitectureID.IsBatteryElectricVehicle() ||
            PrimaryVehicle.ArchitectureID.IsHybridVehicle() ||
            PrimaryVehicle.ArchitectureID.IsFuelCellVehicle()) {
            runData.BatteryData = CreateBatteryData();
        }

        runData.ElectricMachinesSinglePwt = runData.JobType.IsIEPC()
            ? DummyRunPrimaryBusRunDataFactory.CreateDummyIEPCData()
            : DummyRunPrimaryBusRunDataFactory.CreateDummyElecticMachinesData(PrimaryVehicle.Components.ElectricMachines);

        if (PrimaryVehicle.ArchitectureID.IsFuelCellVehicle()) {
			runData.VehicleData.H2StorageUsableCapacity = PrimaryVehicle.H2StorageUsableCapacity;
			runData.OVCMode = PrimaryVehicle.OVC ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
			runData.VehicleData.H2StorageUsableCapacity = 30.SI<Kilogram>();
            runData.FuelCellSystemData = new FuelCellSystemData() {
				Fuel = { FuelData.H2 }
			};
			var fchviterativeStrategy =
				new FCHEVIterativeRunStrategy(new[] { new PreRunOptions(), new PreRunOptions() });
			fchviterativeStrategy.Update = UpdateFCHVRunData;

			runData.IterativeRunStrategy = fchviterativeStrategy;
		}
		if (runData.EngineData != null && runData.EngineData.Fuels.Any(x => x.FuelData.FuelType.IsOneOf(FuelType.H2CI, FuelType.H2PI))) {
			runData.VehicleData.H2StorageUsableCapacity = 30.SI<Kilogram>();
		}
        return runData;
    }

	private void UpdateFCHVRunData(IModalDataContainer moddata, VectoRunData runData)
	{

		runData.OVCMode = runData.OVCMode == OvcHevMode.NotApplicable
			? OvcHevMode.NotApplicable
			: OvcHevMode.ChargeSustaining;
		if (runData.PrimaryResult != null && runData.InputData is IMultistepBusInputDataProvider vif) {
			runData.PrimaryResult = vif.JobInputData.PrimaryVehicle.GetResult(
				runData.Mission.BusParameter.BusGroup, runData.Mission.MissionType, null,
				runData.VehicleData.Loading, runData.OVCMode);
		}

		runData.Iteration++;

	}

	protected BatterySystemData CreateBatteryData()
    {
        return new BatterySystemData()
        {
            Batteries = new List<Tuple<int, BatteryData>>() {
                Tuple.Create(1, new BatteryData() {
                    BatteryId = 0,
                    Capacity = 7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                    ChargeDepletingBattery = true,
                    MinSOC = 0.2,
                    MaxSOC = 0.8,
                    SOCMap = BatterySOCReader.Create("SoC, V\n0, 600\n100, 650\n".ToStream()),
                    InternalResistance = BatteryInternalResistanceReader.Create(
                        "SoC, Ri-2, Ri-10, Ri-20\n0, 20, 20, 20\n100, 20, 20, 20\n".ToStream(), true),
                    MaxCurrent =
                        BatteryMaxCurrentReader.Create(
                            "SoC, I_charge, I_discharge\n0, 300, 300\n100, 500, 500\n"
                                .ToStream())

                })
            }
        };
    }
    //#endregion
}