using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunLorryVectoRunDataFactory : DeclarationModeHeavyLorryRunDataFactory.Conventional
{
    public DummyRunLorryVectoRunDataFactory(IDeclarationInputDataProvider dataProvider,
        IDeclarationReport report,
        ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
        : base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder)
    {

    }

    #region Overrides of AbstractDeclarationVectoRunDataFactory

    //protected override IDeclarationDataAdapter DataAdapter { get; }
    protected override IEnumerable<VectoRunData> GetNextRun()
    {
        if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle)
        {
            yield return CreateVectoRunData(InputDataProvider.JobInputData.Vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
        }
        else
        {
            foreach (var vectoRunData in VectoRunDataTruckNonExempted())
            {
                yield return vectoRunData;
            }
        }

        //         var nextRun = base.GetNextRun();
        //return nextRun;
    }

    protected override void InitializeReport()
    {
        if (InputDataProvider.JobInputData.JobType == VectoSimulationJobType.ConventionalVehicle)
        {
            base.InitializeReport();
            return;
        }
        VectoRunData powertrainConfig;
        var vehicle = InputDataProvider.JobInputData.Vehicle;
        if (vehicle.ExemptedVehicle)
        {
            powertrainConfig = CreateVectoRunData(vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
        }
        else
        {
            powertrainConfig = _segment.Missions.Select(
                    mission => CreateVectoRunData(
                        vehicle, 0, mission, mission.Loadings.First()))
                .FirstOrDefault(x => x != null);
        }
        Report.InitializeReport(powertrainConfig);
    }

    private IEnumerable<VectoRunData> VectoRunDataTruckNonExempted()
    {
        switch (InputDataProvider.JobInputData.JobType)
        {
            case VectoSimulationJobType.ConventionalVehicle:
            case VectoSimulationJobType.ParallelHybridVehicle:
            case VectoSimulationJobType.SerialHybridVehicle:
            case VectoSimulationJobType.IEPC_S:
            case VectoSimulationJobType.IHPC:
                return VectoRunDataConventionalTruckNonExempted();
            case VectoSimulationJobType.BatteryElectricVehicle:
            case VectoSimulationJobType.IEPC_E:
                return VectoRunDataBatteryElectricVehicle();
            case VectoSimulationJobType.EngineOnlySimulation:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return VectoRunDataConventionalTruckNonExempted();

    }

    private IEnumerable<VectoRunData> VectoRunDataBatteryElectricVehicle()
    {
        var vehicle = InputDataProvider.JobInputData.Vehicle;
        foreach (var mission in _segment.Missions)
        {
            foreach (var loading in mission.Loadings)
            {
                var simulationRunData = CreateVectoRunData(vehicle, 0, mission, loading);
                yield return simulationRunData;
            }

        }
    }

    private IEnumerable<VectoRunData> VectoRunDataConventionalTruckNonExempted()
    {
        var vehicle = InputDataProvider.JobInputData.Vehicle;

        var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
        var engineModes = engine.EngineModes;

        for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
        {
            foreach (var mission in _segment.Missions)
            {
                if (mission.MissionType.IsEMS() &&
                    engine.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS))
                {
                    continue;
                }

                foreach (var loading in mission.Loadings)
                {
                    var simulationRunData = CreateVectoRunData(vehicle, modeIdx, mission, loading);
                    if (vehicle.OVC)
                    {
                        simulationRunData.OVCMode = OvcHevMode.ChargeDepleting;
                        yield return simulationRunData;
                        simulationRunData = CreateVectoRunData(vehicle, modeIdx, mission, loading);
                        simulationRunData.OVCMode = OvcHevMode.ChargeSustaining;
                    }
                    yield return simulationRunData;
                }
            }
        }
    }

    protected virtual VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
        KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
    {
        VectoRunData runData;
        if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle)
        {
            runData = new VectoRunData
            {
                Exempted = true,
                Report = Report,
                Mission = new Mission() { MissionType = MissionType.ExemptedMission },
                VehicleData = CreateDummyVehicleData(vehicle),
                InputDataHash = InputDataProvider.XMLHash
            };
            runData.VehicleData.InputData = vehicle;
        }
        else
        {
            var cycle = CycleFactory.GetDeclarationCycle(mission);

            runData = new VectoRunData()
            {
                Loading = loading.Key,
                Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                ExecutionMode = ExecutionMode.Declaration,
                Report = Report,
                Mission = mission,
                SimulationType = SimulationType.DistanceCycle,
                VehicleData = CreateDummyVehicleData(vehicle),
                EngineData = CreateDummyEngineData(vehicle, modeIdx),
                GearboxData = CreateDummyGearboxData(vehicle),
                AxleGearData = CreateDummyAxleGearData(vehicle),

                JobType = InputDataProvider.JobInputData.JobType,

            };

            if (vehicle.ArchitectureID.IsBatteryElectricVehicle() ||
                vehicle.ArchitectureID.IsHybridVehicle())
            {
                runData.BatteryData = CreateBatteryData();
            }

        }

        if (!vehicle.ExemptedVehicle)
        {
            runData.VehicleData.Loading = loading.Value.Item1;
            runData.VehicleData.CargoVolume = mission.MissionType != MissionType.Construction
                ? mission.TotalCargoVolume
                : 0.SI<CubicMeter>();
        }

        runData.InputData = InputDataProvider;


        return runData;



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


    protected override void Initialize()
    {
        _segment = DeclarationData.GetTruckSegment(InputDataProvider.JobInputData.Vehicle).Segment;

    }

    #endregion

    private AxleGearData CreateDummyAxleGearData(IVehicleDeclarationInputData vehicle)
    {
        if (vehicle.Components.AxleGearInputData == null)
        {
            return null;
        }

        return new AxleGearData()
        {
            InputData = vehicle.Components.AxleGearInputData,
        };
    }

    private GearboxData CreateDummyGearboxData(IVehicleDeclarationInputData vehicle)
    {
        if (vehicle.Components.GearboxInputData == null)
        {
            return null;
        }

        return new GearboxData()
        {
            InputData = vehicle.Components.GearboxInputData,
        };
    }


    private CombustionEngineData CreateDummyEngineData(IVehicleDeclarationInputData vehicleData, int modeIdx)
    {

        var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
        if (engine == null)
        {
            return null;
        }
        var engineModes = engine.EngineModes;
        var engineMode = engineModes[modeIdx];
        var fuels = new List<CombustionEngineFuelData>();
        foreach (var fuel in engineMode.Fuels)
        {
            fuels.Add(new CombustionEngineFuelData()
            {
                FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, vehicleData.TankSystem)
            });
        }

        return new CombustionEngineData()
        {
            Fuels = fuels,
            RatedPowerDeclared = vehicleData.Components.EngineInputData.RatedPowerDeclared
        };
    }


    private VehicleData CreateDummyVehicleData(IVehicleDeclarationInputData vehicleData)
    {
        return new VehicleData()
        {
            InputData = vehicleData,
            SleeperCab = vehicleData.SleeperCab,
            VehicleClass = _segment.VehicleClass,
            VehicleCategory = VehicleCategory.RigidTruck,
            OffVehicleCharging = vehicleData.OVC,
            VocationalVehicle = vehicleData.VocationalVehicle,
            AxleData = vehicleData.ExemptedVehicle ? null : vehicleData.Components.AxleWheels.AxlesDeclaration.Select((x, idx) => new Axle()
            {
                AxleType = idx == 1 ? AxleType.VehicleDriven : AxleType.VehicleNonDriven,
            }).ToList(),
        };
    }
}