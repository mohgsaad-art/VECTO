using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.Auxiliaries;

public class BusAuxTests
{
    [TestCase(12000, 1256, 148, 148, 5649.8149)]
    [TestCase(12000, 1256, -45, -30, 8516.9257)]
    [TestCase(15700, 1319, -45.79263, -24.0441, 8656.7333)]
    public void AuxDemandtest(double vehicleWeight, double engineSpeedRpm, double driveLinePower, double internalPower,
        double expectedPowerDemand)
    {
        var busAux = CreateBusAuxAdapterForTesting(vehicleWeight);

        var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
        var engineSpeed = engineSpeedRpm.RPMtoRad();
        busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

        var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
            engineSpeed);

        Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-2);
    }


    public static BusAuxiliariesAdapter CreateBusAuxAdapterForTesting(double vehicleMass, DrivingBehavior behavior = DrivingBehavior.Braking, DrivingAction action = DrivingAction.Brake)
    {
        var container = new Mock<IVehicleContainer>();
        var modelData = new CombustionEngineData()
        {
            Fuels = new List<CombustionEngineFuelData>() {
                    new CombustionEngineFuelData()
                },
           IdleSpeed = 560.SI<PerSecond>()
        };
        container.Setup(v => v.RunData).Returns(new VectoRunData()
        {
            EngineData = modelData,
            VehicleData = new VehicleData()
            {
                CurbMass = vehicleMass.SI<Kilogram>()
            }
        });
        var fld = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(ICEFldHeader, ICEFld));
        var engine = new Mock<IEngineInfo>();
        engine.Setup(e => e.EngineIdleSpeed).Returns(560.RPMtoRad());
        engine.Setup(e => e.EngineDragPower(It.IsNotNull<PerSecond>())).Returns((PerSecond r) => fld.DragLoadStationaryPower(r));
        var engineCtl = engine.As<IEngineControl>();
        engineCtl.Setup(e => e.CombustionEngineOn).Returns(true);
        container.Setup(v => v.EngineInfo).Returns(engine.Object);
        container.Setup(v => v.EngineCtl).Returns(engineCtl.Object);
        var driver = new Mock<IDriverInfo>();
        driver.Setup(d => d.DrivingAction).Returns(action);
        driver.Setup(d => d.DriverBehavior).Returns(behavior);
        container.Setup(v => v.DriverInfo).Returns(driver.Object);
        var gbx = new Mock<IGearboxInfo>();
        gbx.Setup(g => g.GearEngaged(It.IsNotNull<Second>())).Returns(true);
        gbx.Setup(g => g.Gear).Returns(new GearshiftPosition(1));
        container.Setup(v => v.GearboxInfo).Returns(gbx.Object);
        var clutch = new Mock<IClutchInfo>();
        clutch.Setup(c => c.ClutchClosed(It.IsNotNull<Second>())).Returns(true);
        container.Setup(v => v.ClutchInfo).Returns(clutch.Object);
        var brk = new Mock<IBrakes>();
        brk.Setup(b => b.BrakePower).Returns(0.SI<Watt>());
        container.Setup(v => v.Brakes).Returns(brk.Object);
        var veh = new Mock<IVehicleInfo>();
        veh.Setup(v => v.VehicleSpeed).Returns(50.KMPHtoMeterPerSecond());
        veh.Setup(v => v.VehicleStopped).Returns(false);
        container.Setup(v => v.VehicleInfo).Returns(veh.Object);

        var heatPumpCoP = new Dictionary<HeatPumpType, double>();
        var heaterEff = new Dictionary<HeaterType, double>();
        foreach (var entry in EnumHelper.GetValues<HeatPumpType>())
        {
            heatPumpCoP.Add(entry, 3.5);
        }
        var averageCurrentDemandInclBaseLoad = 59.782177.SI<Ampere>();
        var averageCurrentDemandWithoutBaseLoad = 35.631777385159026.SI<Ampere>();
        var auxConfig = new AuxiliaryConfig()
        {
            ElectricalUserInputsConfig = new ElectricsUserInputsConfig()
            {
                AlternatorType = AlternatorType.Conventional,
                AlternatorMap = AlternatorReader.ReadMap(InputDataHelper.InputDataAsStream(AltHeader, AltData)),
                ConnectESToREESS = false,
                PowerNetVoltage = 28.3.SI<Volt>(),
                AlternatorGearEfficiency = 0.92,
                ElectricalConsumers = new Dictionary<string, ElectricConsumerEntry>() {
                    {"BaseLoad", new ElectricConsumerEntry() {
                        BaseVehicle = true,
                        Current = averageCurrentDemandInclBaseLoad - averageCurrentDemandWithoutBaseLoad
                    }},
                    {"Consumers", new ElectricConsumerEntry() {
                        BaseVehicle = false,
                        Current = averageCurrentDemandWithoutBaseLoad
                    }}
                },
                DoorActuationTimeSecond = 4.SI<Second>()
            },
            PneumaticUserInputsConfig = new PneumaticUserInputsConfig()
            {
                CompressorMap = CompressorMapReader.ReadStream(InputDataHelper.InputDataAsStream(PSHeader, PSData), 1.0, "Testdata"),
                CompressorGearRatio = 1,
                CompressorGearEfficiency = 0.8,
                KneelingHeight = 80.SI(Unit.SI.Milli.Meter).Cast<Meter>(),
                AdBlueDosing = ConsumerTechnology.Pneumatically,
                AirSuspensionControl = ConsumerTechnology.Electrically,
                Doors = ConsumerTechnology.Pneumatically,
                SmartAirCompression = true,
                SmartRegeneration = true,
            },
            PneumaticAuxiliariesConfig = new PneumaticsConsumersDemand()
            {
                AdBlueInjection = 21.25.SI(Unit.SI.Liter.Per.Minute).Cast<NormLiterPerSecond>(),
                AirControlledSuspension = 15.SI(Unit.SI.Liter.Per.Minute).Cast<NormLiterPerSecond>(),
                Braking = 0.0006.SI(Unit.SI.NormLiter.Per.Kilo.Gramm).Cast<NormLiterPerKilogram>(),
                BreakingWithKneeling = 6.6e-5.SI(Unit.SI.NormLiter.Per.Kilo.Gramm.Milli.Meter).Cast<NormLiterPerKilogramMeter>(),
                DeadVolBlowOuts = 24.SI(Unit.SI.Per.Hour).Cast<PerSecond>(),
                DeadVolume = 30.SI<NormLiter>(),
                NonSmartRegenFractionTotalAirDemand = 0.26,
                OverrunUtilisationForCompressionFraction = 0.97,
                DoorOpening = 12.7.SI<NormLiter>(),
                StopBrakeActuation = 0.00064.SI(Unit.SI.Liter.Per.Kilo.Gramm).Cast<NormLiterPerKilogram>(),
                SmartRegenFractionTotalAirDemand = 0.12,
            },
            SSMInputsCooling = new SSMInputs("testdata")
            {
                GFactor = 0.95,
                HeatingBoundaryTemperature = 18.DegCelsiusToKelvin(),
                CoolingBoundaryTemperature = 23.DegCelsiusToKelvin(),
                VentilationRate = 20.SI(Unit.SI.Per.Hour).Cast<PerSecond>(),
                VentilationRateHeating = 20.SI(Unit.SI.Per.Hour).Cast<PerSecond>(),
                BusFloorType = FloorType.HighFloor,
                BusSurfaceArea = 114.42.SI<SquareMeter>(),
                BusWindowSurface = 20.9825.SI<SquareMeter>(),
                BusVolumeVentilation = 61.81231875.SI<CubicMeter>(),
                NumberOfPassengers = 34,
                UValue = 3.SI<WattPerKelvinSquareMeter>(),
                SpecificVentilationPower = 0.56.SI(Unit.SI.Watt.Hour.Per.Cubic.Meter).Cast<JoulePerCubicMeter>(),
                AuxHeaterEfficiency = 0.84,

                DefaultConditions = new EnvironmentalConditionMapEntry(0,
                    25.DegCelsiusToKelvin(), 400.SI<WattPerSquareMeter>(), 1.0, heatPumpCoP, heaterEff),
                HVACMaxCoolingPowerPassenger = 18000.SI<Watt>(),
                VentilationOnDuringHeating = true,
                VentilationDuringAC = true,
                VentilationWhenBothHeatingAndACInactive = true,
                FuelFiredHeaterPower = 30000.SI<Watt>(),
                FuelEnergyToHeatToCoolant = 0.2,
                CoolantHeatTransferredToAirCabinHeater = 0.75,
                Technologies = new TechnologyBenefits()
                {
                    CValueVariation = 0,
                    HValueVariation = 0,
                    VVValueVariation = 0,
                    VHValueVariation = 0,
                    VCValueVariation = 0
                }
            },
            Actuations = new Actuations()
            {
                Braking = 27,
                Kneeling = 0,
                ParkBrakeAndDoors = 6,
                CycleTime = 15000.SI<Second>()
            },
            VehicleData = container.Object.RunData.VehicleData,
        };
        auxConfig.SSMInputsHeating = auxConfig.SSMInputsCooling;
        //var str = JsonConvert.SerializeObject(auxConfig);

        var busAux = new BusAuxiliariesAdapter(container.Object, auxConfig);
        var electricStorage = auxConfig.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
            ? new SimpleBattery(container.Object, auxConfig.ElectricalUserInputsConfig.ElectricStorageCapacity, auxConfig.ElectricalUserInputsConfig.StoredEnergyEfficiency)
            : (ISimpleBattery)new NoBattery(container.Object);
        busAux.ElectricStorage = electricStorage;
        return busAux;
    }

    public const string ICEFldHeader = "n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s]";

    public static readonly string[] ICEFld = new[] {
        "560,1180,-149,0.6",
        "600,1282,-148,0.6",
        "799.9999999,1791,-149,0.6",
        "1000,2300,-160,0.6",
        "1200,2300,-179,0.6",
        "1400,2300,-203,0.6",
        "1599.999999,2079,-235,0.49",
        "1800,1857,-264,0.25",
        "2000.000001,1352,-301,0.25",
        "2100,1100,-320,0.25",
    };

    public const string PSHeader = "rpm,flowRate [l/min],power on [W],power off [W]";

    public static readonly string[] PSData = new[] {
        "800,250.5365596,3139.5,524.3",
        "1200,374.3533986,4609.5,1027.2",
        "1600,508.4123859,6205.5,1572.9",
        "2000,619.1263282,7770,2065.1",
        "2400,762.6185788,9723,2696.4",
        "2550,819.2371476,10363.5,2856.9",
        "2800,898.7501978,11613,3349.1",
        "3200,979.4827586,13282.5,4012.5"
    };

    public const string AltHeader = "AlternatorName,RPM,Amps,Efficiency,PulleyRatio";

    public static readonly string[] AltData = new[] {
        "Alt1,2000,10.000,50.000,3.000",
        "Alt1,2000,40.000,50.000,3.000",
        "Alt1,2000,60.000,50.000,3.000",
        "Alt1,4000,10.000,70.000,3.000",
        "Alt1,4000,40.000,70.000,3.000",
        "Alt1,4000,60.000,70.000,3.000",
        "Alt1,6000,10.000,60.000,3.000",
        "Alt1,6000,40.000,60.000,3.000",
        "Alt1,6000,60.000,60.000,3.000",
    };
}