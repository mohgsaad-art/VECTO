using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunPrimaryBusRunDataFactory : DeclarationModePrimaryBusRunDataFactory.Conventional
{
    public DummyRunPrimaryBusRunDataFactory(IDeclarationInputDataProvider dataProvider,
        IDeclarationReport report,
        IPrimaryBusDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
        IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) :
        base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder)
    { }

    #region Overrides of AbstractDeclarationVectoRunDataFactory

    protected override IEnumerable<VectoRunData> GetNextRun()
    {
        if (InputDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle)
        {
            if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle)
            {
                yield return CreateVectoRunData(null,
                    new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 0);
            }
            else
            {
                foreach (var vectoRunData in VectoRunDataHeavyBusPrimary())
                {
                    yield return vectoRunData;
                }
            }
        }

        foreach (var entry in new List<VectoRunData>())
        {
            yield return entry;
        }
    }

    private IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
    {
        switch (InputDataProvider.JobInputData.JobType)
        {
            case VectoSimulationJobType.ConventionalVehicle:
            case VectoSimulationJobType.ParallelHybridVehicle:
            case VectoSimulationJobType.SerialHybridVehicle:
            case VectoSimulationJobType.IHPC:
            case VectoSimulationJobType.IEPC_S:
                return VectoRunDataConventionalHeavyBusPrimaryNonExempted();
            case VectoSimulationJobType.IEPC_E:
            case VectoSimulationJobType.BatteryElectricVehicle:
                return VectoRunDataBatteryElectricHeavyBusPrimaryNonExempted();
            case VectoSimulationJobType.EngineOnlySimulation:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return VectoRunDataConventionalHeavyBusPrimaryNonExempted();
    }

    private IEnumerable<VectoRunData> VectoRunDataBatteryElectricHeavyBusPrimaryNonExempted()
    {
        var vehicle = InputDataProvider.JobInputData.Vehicle;
        foreach (var mission in _segment.Missions)
        {
            foreach (var loading in mission.Loadings)
            {
                var simulationRunData = CreateVectoRunData(mission, loading, 0);
                if (simulationRunData == null)
                {
                    continue;
                }
                yield return simulationRunData;
            }

        }
    }

    private IEnumerable<VectoRunData> VectoRunDataConventionalHeavyBusPrimaryNonExempted()
    {
        var vehicle = InputDataProvider.JobInputData.Vehicle;
        var engine = vehicle.Components.EngineInputData;
        var engineModes = engine.EngineModes;

        for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
        {
            foreach (var mission in _segment.Missions)
            {
                foreach (var loading in mission.Loadings)
                {
                    var simulationRunData = CreateVectoRunData(mission, loading, modeIdx);
                    if (simulationRunData == null)
                    {
                        continue;
                    }
                    if (vehicle.OVC)
                    {
                        simulationRunData.OVCMode = OvcHevMode.ChargeDepleting;
                        yield return simulationRunData;
                        simulationRunData = CreateVectoRunData(mission, loading, modeIdx);
                        simulationRunData.OVCMode = OvcHevMode.ChargeSustaining;
                    }
                    yield return simulationRunData;
                }
            }
        }
    }

    protected override void Initialize()
    {
        _segment = GetSegment();
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
            powertrainConfig = CreateVectoRunData(null,
                new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 0);
        }
        else
        {
            powertrainConfig = _segment.Missions.Select(
                    mission => CreateVectoRunData(mission, mission.Loadings.First(), 0))
                .FirstOrDefault(x => x != null);
        }

        Report.InitializeReport(powertrainConfig);
    }

    #region Overrides of DeclarationModePrimaryBusVectoRunDataFactory

    protected override VectoRunData CreateVectoRunData(Mission mission,
        KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
        int? modeIdx,
        OvcHevMode ovcMode = OvcHevMode.NotApplicable)
    {

        VectoRunData runData;
        if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle)
        {
            runData = new VectoRunData
            {
                Exempted = true,
                Report = Report,
                Mission = new Mission() { MissionType = MissionType.ExemptedMission },
                VehicleData = CreateDummyExemptedVehicleData(Vehicle, _segment),
                InputDataHash = InputDataProvider.XMLHash
            };
            runData.VehicleData.InputData = Vehicle;
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
                VehicleData = CreateDummyVehicleData(Vehicle, _segment, loading),
                Retarder = CreateDummyRetarder(Vehicle),
                AxleGearData = CreateDummyAxleGearData(Vehicle),
                GearboxData = CreateDummyGearboxData(Vehicle),
                AngledriveData = CreateDummyAngleDriveData(Vehicle),
                EngineData = CreateDummyEngineData(Vehicle, modeIdx),
                BusAuxiliaries = CreateDummyBusAux(Vehicle),
                InputDataHash = InputDataProvider.XMLHash,

                JobType = InputDataProvider.JobInputData.JobType,
            };

            if (InputDataProvider.JobInputData.Vehicle.ArchitectureID.IsBatteryElectricVehicle() ||
                InputDataProvider.JobInputData.Vehicle.ArchitectureID.IsHybridVehicle())
            {
                runData.BatteryData = CreateBatteryData();
            }
        }

        runData.InputData = InputDataProvider;

        return runData;
    }


    #endregion

    #endregion

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

    public static IAuxiliaryConfig CreateDummyBusAux(IVehicleDeclarationInputData vehicle)
    {
        return new AuxiliaryConfig()
        {
            InputData = vehicle.Components.BusAuxiliaries,

        };
    }

    public static AirdragData CreateDummyAirdragData(IVehicleDeclarationInputData vehicle)
    {
        var airdrag = vehicle.Components.AirdragInputData;
        if (airdrag == null)
        {
            return new AirdragData()
            {
                CertificationMethod = CertificationMethod.StandardValues,
                DeclaredAirdragArea = 0.SI<SquareMeter>(), // dummy value -- no used at all
            };
        }
        return new AirdragData()
        {
            CertificationMethod = airdrag.CertificationMethod,
            CertificationNumber = airdrag.CertificationNumber,
            DeclaredAirdragAreaInput = airdrag.AirDragArea,
        };
    }
    public static RetarderData CreateDummyRetarder(IVehicleDeclarationInputData vehicle)
    {
        var xmlVehicle = vehicle as IXMLDeclarationVehicleData;
        return new RetarderData()
        {
            Type = xmlVehicle.GetRetarderType(),

            Ratio = xmlVehicle.GetRetarderType().IsDedicatedComponent() ? xmlVehicle.GetRetarderRatio() : 0,
        };
    }

    public static AngledriveData CreateDummyAngleDriveData(IVehicleDeclarationInputData vehicle)
    {
        if (vehicle.Components.AngledriveInputData == null || vehicle.Components.AngledriveInputData.Type != AngledriveType.SeparateAngledrive)
        {
            return null;
        }

        var componentData = vehicle.Components.AngledriveInputData;
        var angleDriveData = new AngledriveData
        {
            InputData = vehicle.Components.AngledriveInputData,
            Type = componentData.Type,
        };

        if (componentData.Type == AngledriveType.SeparateAngledrive)
        {

            angleDriveData.Angledrive = new TransmissionData()
            {
                Ratio = componentData.Ratio,
            };

            angleDriveData.Manufacturer = componentData.Manufacturer;
            angleDriveData.ModelName = componentData.Model;
            angleDriveData.CertificationNumber = componentData.CertificationNumber;
            angleDriveData.Date = componentData.Date;
        }

        return angleDriveData;
    }

    public static AxleGearData CreateDummyAxleGearData(IVehicleDeclarationInputData vehicle)
    {
        if (vehicle.Components.AxleGearInputData == null)
        {
            return null;
        }

        var componentData = vehicle.Components.AxleGearInputData;
        return new AxleGearData()
        {
            InputData = vehicle.Components.AxleGearInputData,

            Manufacturer = componentData.Manufacturer,
            ModelName = componentData.Model,
            CertificationNumber = componentData.CertificationNumber,
            Date = componentData.Date,
            LineType = componentData.LineType,
            AxleGear = new TransmissionData()
            {
                Ratio = vehicle.Components.AxleGearInputData.Ratio,

            }
        };
    }

    public static GearboxData CreateDummyGearboxData(IVehicleDeclarationInputData vehicle)
    {
        if (vehicle.Components.GearboxInputData == null)
        {
            return null;
        }

        var componentData = vehicle.Components.GearboxInputData;
        var gears = new Dictionary<uint, GearData>();
        foreach (var gearInputData in componentData.Gears)
        {
            gears.Add((uint)gearInputData.Gear, new GearData()
            {
                Ratio = gearInputData.Ratio,
                MaxTorque = gearInputData.MaxTorque,
                MaxSpeed = gearInputData.MaxInputSpeed,
            });
        }


        return new GearboxData()
        {
            InputData = vehicle.Components.GearboxInputData,
            Type = vehicle.Components.GearboxInputData.Type,
            Manufacturer = componentData.Manufacturer,
            ModelName = componentData.Model,
            CertificationNumber = componentData.CertificationNumber,
            Date = componentData.Date,
            Gears = gears,

        };
    }


    public static CombustionEngineData CreateDummyEngineData(IVehicleDeclarationInputData vehicleData, int? modeIdx, TankSystem? tankSystem = null)
    {

        var engine = vehicleData.Components.EngineInputData;
        if (engine == null || modeIdx == null)
        {
            return null;
        }

        var engineModes = engine.EngineModes;
        var engineMode = engineModes[modeIdx.Value];
        var fuels = new List<CombustionEngineFuelData>();
        foreach (var fuel in engineMode.Fuels)
        {
            fuels.Add(new CombustionEngineFuelData()
            {
                FuelData = DeclarationData.FuelData.Lookup(fuel.FuelType, tankSystem ?? vehicleData.TankSystem)
            });
        }

        var componentData = vehicleData.Components.EngineInputData;
        return new CombustionEngineData()
        {
            Fuels = fuels,
            RatedPowerDeclared = vehicleData.Components.EngineInputData.RatedPowerDeclared,
            IdleSpeed = vehicleData.EngineIdleSpeed,
            InputData = vehicleData.Components.EngineInputData,
            WHRType = vehicleData.Components.EngineInputData.WHRType,
            RatedSpeedDeclared = engine.RatedSpeedDeclared,
            Displacement = engine.Displacement,

            Manufacturer = componentData.Manufacturer,
            ModelName = componentData.Model,
            CertificationNumber = componentData.CertificationNumber,
            Date = componentData.Date,


        };
    }

    private VehicleData CreateDummyExemptedVehicleData(IVehicleDeclarationInputData vehicleData, Segment segment)
    {
        return new VehicleData()
        {
            InputData = vehicleData,
            SleeperCab = vehicleData.SleeperCab,
            //Loading = loading.Value.Item1,
            VehicleClass = segment.VehicleClass,
            OffVehicleCharging = vehicleData.OVC,
            VehicleCategory = vehicleData.VehicleCategory,
            ZeroEmissionVehicle = vehicleData.ZeroEmissionVehicle,

            Manufacturer = vehicleData.Manufacturer,
            ManufacturerAddress = vehicleData.ManufacturerAddress,
            ModelName = vehicleData.Model,
            VIN = vehicleData.VIN,
            LegislativeClass = vehicleData.LegislativeClass,
            AxleConfiguration = vehicleData.AxleConfiguration,
            Date = vehicleData.Date,

        };
    }

    public static VehicleData CreateDummyVehicleData(IVehicleDeclarationInputData vehicleData, Segment segment,
        KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
    {
        return new VehicleData()
        {

            InputData = vehicleData,
            SleeperCab = vehicleData.SleeperCab,
            Loading = loading.Value.Item1,
            VehicleClass = segment.VehicleClass,
            OffVehicleCharging = vehicleData.OVC,
            VehicleCategory = vehicleData.VehicleCategory,
            ZeroEmissionVehicle = vehicleData.ZeroEmissionVehicle,
            ADAS = CreateDummyAdasData(vehicleData),
            PassengerCount = loading.Value.Item2,
            Manufacturer = vehicleData.Manufacturer,
            ManufacturerAddress = vehicleData.ManufacturerAddress,
            ModelName = vehicleData.Model,
            VIN = vehicleData.VIN,
            LegislativeClass = vehicleData.LegislativeClass,
            AxleConfiguration = vehicleData.AxleConfiguration,
            Date = vehicleData.Date,
            AxleData = vehicleData.Components.AxleWheels.AxlesDeclaration.Select((x, idx) => new Axle()
            {
                AxleType = idx == 1 ? AxleType.VehicleDriven : AxleType.VehicleNonDriven,
            }).ToList(),
        };
    }

    public static VehicleData.ADASData CreateDummyAdasData(IVehicleDeclarationInputData vehicleData)
    {
        var adas = vehicleData.ADAS;
        return new VehicleData.ADASData()
        {
            EcoRoll = adas.EcoRoll,
            EngineStopStart = adas.EngineStopStart,
            InputData = adas,
            PredictiveCruiseControl = adas.PredictiveCruiseControl,
        };
    }
}