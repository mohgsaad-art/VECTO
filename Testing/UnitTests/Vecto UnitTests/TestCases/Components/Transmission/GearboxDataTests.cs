using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.MockComponents;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.Transmission;

public class GearboxDataTests
{




    [TestCase(6.38, 2300, 1600, 2356.2326),
    TestCase(6.38, -1300, 1000, -1267.0686),
    // the following entries are beyond the original loss map, but are not 'extrapolated' because the loss-map is extended on reading
    TestCase(6.38, 6300, 1600, 6437.86530),
    TestCase(6.38, -3300, 1000, -3227.8529411)]
    public void Gearbox_LossMapInterpolation(double ratio, double torque,
        double inAngularSpeed, double expectedTorque)
    {
        var expectedTq = expectedTorque.SI<NewtonMeter>();
        var expectedRpm = inAngularSpeed.RPMtoRad();


        var inputData = GetMockInputData();
        var gbxTypes = new[] {
            GearboxType.AMT
        };
        var runData = GetDummyRunData(inputData.Components.GearboxInputData);
        var shiftPolygonCalc = new Mock<IShiftPolygonCalculator>();
        // create gearbox data
        var gearboxData = new GearboxDataAdapter(null).CreateGearboxData(inputData, runData, shiftPolygonCalc.Object, gbxTypes); // MockSimulationDataFactory.CreateGearboxDataFromFile(gbxFile, engineFile);

        runData.GearboxData = gearboxData;
        var container = GetMockVehicleContainer(runData);
        var shiftStrategy = new Mock<IShiftStrategy>();
        shiftStrategy.Setup(s =>
                s.InitGear(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>()))
            .Returns(new GearshiftPosition(1));
        var gearbox = new AMTGearbox(container, shiftStrategy.Object);

        NewtonMeter tqRequest = null;
        PerSecond rpmRequest = null;
        var port = new Mock<ITnOutPort>();
        port.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
            It.IsAny<PerSecond>(), It.Is<bool>(b => !b))).Returns((Second _, Second _, NewtonMeter tq, PerSecond rpm, bool _) =>
            {
                tqRequest = tq;
                rpmRequest = rpm;
                return new ResponseSuccess(this);
            });
        gearbox.InPort().Connect(port.Object);


        gearbox.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());

        var absTime = 0.SI<Second>();
        var dt = 2.SI<Second>();
        var tq = torque.SI<NewtonMeter>();
        var n = inAngularSpeed.RPMtoRad();
        Mock.Get(container).Setup(c => c.AbsTime).Returns(absTime);

        var response = (ResponseSuccess)gearbox.OutPort().Request(absTime, dt, tq * ratio, n / ratio, false);

        port.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), false), Times.Once);
        Assert.IsNotNull(tqRequest);
        Assert.IsNotNull(rpmRequest);
        Assert.AreEqual(expectedTq.Value(), tqRequest.Value(), 0.01, "Torque Engine Side");
        Assert.AreEqual(expectedRpm.Value(), rpmRequest.Value(), 0.01, "AngularVelocity Engine Side");

        Assert.IsFalse(gearbox.CurrentState.TorqueLossResult.Extrapolated);

        var modData = new MockModalDataContainer();
        gearbox.CommitSimulationStep(absTime, dt, modData);
    }



    private static IVehicleContainer GetMockVehicleContainer(VectoRunData runData)
    {
        var container = new Mock<IVehicleContainer>();
        var powertrainInfo = new Mock<IPowertainInfo>();
        var vehicle = new Mock<IVehicleInfo>();
        var driver = new Mock<IDriverInfo>();
        var engine = new Mock<IEngineInfo>();

        container.Setup(c => c.RunData).Returns(runData);
        container.Setup(c => c.VehicleInfo).Returns(vehicle.Object);
        container.Setup(c => c.PowertrainInfo).Returns(powertrainInfo.Object);
        container.Setup(c => c.DriverInfo).Returns(driver.Object);

        vehicle.Setup(v => v.VehicleSpeed).Returns(0.KMPHtoMeterPerSecond());
        powertrainInfo.Setup(p => p.HasCombustionEngine).Returns(true);
        driver.Setup(d => d.DriverBehavior).Returns(DrivingBehavior.Accelerating);
        driver.Setup(d => d.DrivingAction).Returns(DrivingAction.Accelerate);
        container.Setup(c => c.EngineInfo).Returns(engine.Object);

        return container.Object;
    }

    private IVehicleDeclarationInputData GetMockInputData()
    {
        var input = new Mock<IVehicleDeclarationInputData>();
        var components = new Mock<IVehicleComponentsDeclaration>();
        input.Setup(i => i.Components).Returns(components.Object);
        var gbx = new Mock<IGearboxDeclarationInputData>();
        components.Setup(c => c.GearboxInputData).Returns(gbx.Object);
        var gearRatios = new double[] {
            6.38, 4.63, 3.44, 2.59, 1.86, 1.35, 1.0, 0.76
        };
        var gears = gearRatios.Select((x, idx) =>
        {
            var gear = new Mock<ITransmissionInputData>();
            gear.Setup(g => g.Ratio).Returns(x);
            gear.Setup(g => g.Gear).Returns(idx + 1);
            var lossMap = x != 1.0 ? LossMapIndirect : LossMapDirect;
            gear.Setup(g => g.LossMap)
                .Returns(InputDataHelper.InputDataAsTableData(LossMapHdr, lossMap));
            return gear.Object;
        }).ToList();
        gbx.Setup(g => g.Type).Returns(GearboxType.AMT);
        gbx.Setup(g => g.Gears).Returns(gears);
        return input.Object;
    }

    private static VectoRunData GetDummyRunData(IGearboxDeclarationInputData gbxData)
    {
        var fld = new[] {
                "560,1180,-149,0.6		   ",
                "600,1282,-148,0.6		   ",
                "799.9999999,1791,-149,0.6 ",
                "1000,2300,-160,0.6		   ",
                "1200,2300,-179,0.6		   ",
                "1400,2300,-203,0.6		   ",
                "1599.999999,2079,-235,0.49",
                "1800,1857,-264,0.25	   ",
                "2000.000001,1352,-301,0.25",
                "2100,1100,-320,0.25	   ",
            };
        var engineData = new CombustionEngineData()
        {
            IdleSpeed = 600.RPMtoRad(),
            Inertia = 0.SI<KilogramSquareMeter>(),
            EngineStartTime = 1.SI<Second>(),
        };
        var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>();
        fullLoadCurves[0] = FullLoadCurveReader.Create(
			InputDataHelper.InputDataAsTableData("engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]",
                    fld));
        fullLoadCurves[0].EngineData = engineData;
        foreach (var gears in gbxData.Gears)
        {
            fullLoadCurves[(uint)gears.Gear] = fullLoadCurves[0];
        }
        engineData.FullLoadCurves = fullLoadCurves;
        return new VectoRunData()
        {
            VehicleData = new VehicleData()
            {
                DynamicTyreRadius = 0.492.SI<Meter>(),
			},
            AxleGearData = new AxleGearData()
            {
                AxleGear = new GearData()
                {
                    Ratio = 2.64
                }
            },
			EngineData = engineData,
            GearshiftParameters = new ShiftStrategyParameters()
            {
                StartSpeed = 2.SI<MeterPerSecond>(),
                StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
                TimeBetweenGearshifts = DeclarationData.Gearbox.MinTimeBetweenGearshifts,
                DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
                UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
                UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
            },
		};
    }

    public const string LossMapHdr = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";

    public static readonly string[] LossMapIndirect = new[] {
        "0,-650,18.06",
        "0,-850,22.06",
        "0,-1050,26.06",
        "0,-1250,30.06",
        "0,-1450,34.06",
        "0,-1650,38.06",
        "0,-1850,42.06",
        "0,-2050,46.06",
        "0,-2250,50.06",
        "0,-2450,54.06",
        "0,-350,12.06",
        "0,-150,8.06",
        "0,50,6.06",
        "0,250,10.06",
        "0,450,14.06",
        "0,650,18.06",
        "0,850,22.06",
        "0,1050,26.06",
        "0,1250,30.06",
        "0,1450,34.06",
        "0,1650,38.06",
        "0,1850,42.06",
        "0,2050,46.06",
        "0,2250,50.06",
        "0,2450,54.06",
        "200,-650,19.072",
        "200,-850,23.072",
        "200,-1050,27.072",
        "200,-1250,31.072",
        "200,-1450,35.072",
        "200,-1650,39.072",
        "200,-1850,43.072",
        "200,-2050,47.072",
        "200,-2250,51.072",
        "200,-2450,55.072",
        "200,-350,13.072",
        "200,-150,9.072",
        "200,50,7.072",
        "200,250,11.072",
        "200,450,15.072",
        "200,650,19.072",
        "200,850,23.072",
        "200,1050,27.072",
        "200,1250,31.072",
        "200,1450,35.072",
        "200,1650,39.072",
        "200,1850,43.072",
        "200,2050,47.072",
        "200,2250,51.072",
        "200,2450,55.072",
        "400,-650,20.084",
        "400,-850,24.084",
        "400,-1050,28.084",
        "400,-1250,32.084",
        "400,-1450,36.084",
        "400,-1650,40.084",
        "400,-1850,44.084",
        "400,-2050,48.084",
        "400,-2250,52.084",
        "400,-2450,56.084",
        "400,-350,14.084",
        "400,-150,10.084",
        "400,50,8.084",
        "400,250,12.084",
        "400,450,16.084",
        "400,650,20.084",
        "400,850,24.084",
        "400,1050,28.084",
        "400,1250,32.084",
        "400,1450,36.084",
        "400,1650,40.084",
        "400,1850,44.084",
        "400,2050,48.084",
        "400,2250,52.084",
        "400,2450,56.084",
        "600,-650,21.096",
        "600,-850,25.096",
        "600,-1050,29.096",
        "600,-1250,33.096",
        "600,-1450,37.096",
        "600,-1650,41.096",
        "600,-1850,45.096",
        "600,-2050,49.096",
        "600,-2250,53.096",
        "600,-350,15.096",
        "600,-150,11.096",
        "600,50,9.096",
        "600,250,13.096",
        "600,450,17.096",
        "600,650,21.096",
        "600,850,25.096",
        "600,1050,29.096",
        "600,1250,33.096",
        "600,1450,37.096",
        "600,1650,41.096",
        "600,1850,45.096",
        "600,2050,49.096",
        "600,2250,53.096",
        "600,2450,57.096",
        "800,-650,22.108",
        "800,-850,26.108",
        "800,-1050,30.108",
        "800,-1250,34.108",
        "800,-1450,38.108",
        "800,-1650,42.108",
        "800,-1850,46.108",
        "800,-2050,50.108",
        "800,-2250,54.108",
        "800,-2450,58.108",
        "800,-350,16.108",
        "800,-150,12.108",
        "800,50,10.108",
        "800,250,14.108",
        "800,450,18.108",
        "800,650,22.108",
        "800,850,26.108",
        "800,1050,30.108",
        "800,1250,34.108",
        "800,1450,38.108",
        "800,1650,42.108",
        "800,1850,46.108",
        "800,2050,50.108",
        "800,2250,54.108",
        "800,2450,58.108",
        "1000,-650,23.12",
        "1000,-850,27.12",
        "1000,-1050,31.12",
        "1000,-1250,35.12",
        "1000,-1450,39.12",
        "1000,-1650,43.12",
        "1000,-1850,47.12",
        "1000,-2050,51.12",
        "1000,-2250,55.12",
        "1000,-2450,59.12",
        "1000,-350,17.12",
        "1000,-150,13.12",
        "1000,50,11.12",
        "1000,250,15.12",
        "1000,450,19.12",
        "1000,650,23.12",
        "1000,850,27.12",
        "1000,1050,31.12",
        "1000,1250,35.12",
        "1000,1450,39.12",
        "1000,1650,43.12",
        "1000,1850,47.12",
        "1000,2050,51.12",
        "1000,2250,55.12",
        "1000,2450,59.12",
        "1200,-650,24.132",
        "1200,-850,28.132",
        "1200,-1050,32.132",
        "1200,-1250,36.132",
        "1200,-1450,40.132",
        "1200,-1650,44.132",
        "1200,-1850,48.132",
        "1200,-2050,52.132",
        "1200,-2250,56.132",
        "1200,-2450,60.132",
        "1200,-350,18.132",
        "1200,-150,14.132",
        "1200,50,12.132",
        "1200,250,16.132",
        "1200,450,20.132",
        "1200,650,24.132",
        "1200,850,28.132",
        "1200,1050,32.132",
        "1200,1250,36.132",
        "1200,1450,40.132",
        "1200,1650,44.132",
        "1200,1850,48.132",
        "1200,2050,52.132",
        "1200,2250,56.132",
        "1200,2450,60.132",
        "1400,-650,25.144",
        "1400,-850,29.144",
        "1400,-1050,33.144",
        "1400,-1250,37.144",
        "1400,-1450,41.144",
        "1400,-1650,45.144",
        "1400,-1850,49.144",
        "1400,-2050,53.144",
        "1400,-2250,57.144",
        "1400,-2450,61.144",
        "1400,-350,19.144",
        "1400,-150,15.144",
        "1400,50,13.144",
        "1400,250,17.144",
        "1400,450,21.144",
        "1400,650,25.144",
        "1400,850,29.144",
        "1400,1050,33.144",
        "1400,1250,37.144",
        "1400,1450,41.144",
        "1400,1650,45.144",
        "1400,1850,49.144",
        "1400,2050,53.144",
        "1400,2250,57.144",
        "1400,2450,61.144",
        "1600,-650,26.156",
        "1600,-850,30.156",
        "1600,-1050,34.156",
        "1600,-1250,38.156",
        "1600,-1450,42.156",
        "1600,-1650,46.156",
        "1600,-1850,50.156",
        "1600,-2050,54.156",
        "1600,-2250,58.156",
        "1600,-2450,62.156",
        "1600,-350,20.156",
        "1600,-150,16.156",
        "1600,50,14.156",
        "1600,250,18.156",
        "1600,450,22.156",
        "1600,650,26.156",
        "1600,850,30.156",
        "1600,1050,34.156",
        "1600,1250,38.156",
        "1600,1450,42.156",
        "1600,1650,46.156",
        "1600,1850,50.156",
        "1600,2050,54.156",
        "1600,2250,58.156",
        "1600,2450,62.156",
        "1800,-650,27.168",
        "1800,-850,31.168",
        "1800,-1050,35.168",
        "1800,-1250,39.168",
        "1800,-1450,43.168",
        "1800,-1650,47.168",
        "1800,-1850,51.168",
        "1800,-2050,55.168",
        "1800,-2250,59.168",
        "1800,-2450,63.168",
        "1800,-350,21.168",
        "1800,-150,17.168",
        "1800,50,15.168",
        "1800,250,19.168",
        "1800,450,23.168",
        "1800,650,27.168",
        "1800,850,31.168",
        "1800,1050,35.168",
        "1800,1250,39.168",
        "1800,1450,43.168",
        "1800,1650,47.168",
        "1800,1850,51.168",
        "1800,2050,55.168",
        "1800,2250,59.168",
        "1800,2450,63.168",
        "2000,-650,28.18",
        "2000,-850,32.18",
        "2000,-1050,36.18",
        "2000,-1250,40.18",
        "2000,-1450,44.18",
        "2000,-1650,48.18",
        "2000,-1850,52.18",
        "2000,-2050,56.18",
        "2000,-2250,60.18",
        "2000,-2450,64.18",
        "2000,-350,22.18",
        "2000,-150,18.18",
        "2000,50,16.18",
        "2000,250,20.18",
        "2000,450,24.18",
        "2000,650,28.18",
        "2000,850,32.18",
        "2000,1050,36.18",
        "2000,1250,40.18",
        "2000,1450,44.18",
        "2000,1650,48.18",
        "2000,1850,52.18",
        "2000,2050,56.18",
        "2000,2250,60.18",
        "2000,2450,64.18",
        "3000,-650,28.18",
        "3000,-850,32.18",
        "3000,-1050,36.18",
        "3000,-1250,40.18",
        "3000,-1450,44.18",
        "3000,-1650,48.18",
        "3000,-1850,52.18",
        "3000,-2050,56.18",
        "3000,-2250,60.18",
        "3000,-2450,64.18",
        "3000,-350,22.18",
        "3000,-150,18.18",
        "3000,50,16.18",
        "3000,250,20.18",
        "3000,450,24.18",
        "3000,650,28.18",
        "3000,850,32.18",
        "3000,1050,36.18",
        "3000,1250,40.18",
        "3000,1450,44.18",
        "3000,1650,48.18",
        "3000,1850,52.18",
        "3000,2050,56.18",
        "3000,2250,60.18",
        "3000,2450,64.18",
    };

    public static readonly string[] LossMapDirect = new[] {
        "0,-650,8.31",
        "0,-850,9.31",
        "0,-1050,10.31",
        "0,-1250,11.31",
        "0,-1450,12.31",
        "0,-1650,13.31",
        "0,-1850,14.31",
        "0,-2050,15.31",
        "0,-2250,16.31",
        "0,-2450,17.31",
        "0,-350,6.81",
        "0,-150,5.81",
        "0,50,5.31",
        "0,250,6.31",
        "0,450,7.31",
        "0,650,8.31",
        "0,850,9.31",
        "0,1050,10.31",
        "0,1250,11.31",
        "0,1450,12.31",
        "0,1650,13.31",
        "0,1850,14.31",
        "0,2050,15.31",
        "0,2250,16.31",
        "0,2450,17.31",
        "200,-650,9.322",
        "200,-850,10.322",
        "200,-1050,11.322",
        "200,-1250,12.322",
        "200,-1450,13.322",
        "200,-1650,14.322",
        "200,-1850,15.322",
        "200,-2050,16.322",
        "200,-2250,17.322",
        "200,-2450,18.322",
        "200,-350,7.822",
        "200,-150,6.822",
        "200,50,6.322",
        "200,250,7.322",
        "200,450,8.322",
        "200,650,9.322",
        "200,850,10.322",
        "200,1050,11.322",
        "200,1250,12.322",
        "200,1450,13.322",
        "200,1650,14.322",
        "200,1850,15.322",
        "200,2050,16.322",
        "200,2250,17.322",
        "200,2450,18.322",
        "400,-650,10.334",
        "400,-850,11.334",
        "400,-1050,12.334",
        "400,-1250,13.334",
        "400,-1450,14.334",
        "400,-1650,15.334",
        "400,-1850,16.334",
        "400,-2050,17.334",
        "400,-2250,18.334",
        "400,-2450,19.334",
        "400,-350,8.834",
        "400,-150,7.834",
        "400,50,7.334",
        "400,250,8.334",
        "400,450,9.334",
        "400,650,10.334",
        "400,850,11.334",
        "400,1050,12.334",
        "400,1250,13.334",
        "400,1450,14.334",
        "400,1650,15.334",
        "400,1850,16.334",
        "400,2050,17.334",
        "400,2250,18.334",
        "400,2450,19.334",
        "600,-650,11.346",
        "600,-850,12.346",
        "600,-1050,13.346",
        "600,-1250,14.346",
        "600,-1450,15.346",
        "600,-1650,16.346",
        "600,-1850,17.346",
        "600,-2050,18.346",
        "600,-2250,19.346",
        "600,-2450,20.346",
        "600,-350,9.846",
        "600,-150,8.846",
        "600,50,8.346",
        "600,250,9.346",
        "600,450,10.346",
        "600,650,11.346",
        "600,850,12.346",
        "600,1050,13.346",
        "600,1250,14.346",
        "600,1450,15.346",
        "600,1650,16.346",
        "600,1850,17.346",
        "600,2050,18.346",
        "600,2250,19.346",
        "600,2450,20.346",
        "800,-650,12.358",
        "800,-850,13.358",
        "800,-1050,14.358",
        "800,-1250,15.358",
        "800,-1450,16.358",
        "800,-1650,17.358",
        "800,-1850,18.358",
        "800,-2050,19.358",
        "800,-2250,20.358",
        "800,-2450,21.358",
        "800,-350,10.858",
        "800,-150,9.858",
        "800,50,9.358",
        "800,250,10.358",
        "800,450,11.358",
        "800,650,12.358",
        "800,850,13.358",
        "800,1050,14.358",
        "800,1250,15.358",
        "800,1450,16.358",
        "800,1650,17.358",
        "800,1850,18.358",
        "800,2050,19.358",
        "800,2250,20.358",
        "800,2450,21.358",
        "1000,-650,13.37",
        "1000,-850,14.37",
        "1000,-1050,15.37",
        "1000,-1250,16.37",
        "1000,-1450,17.37",
        "1000,-1650,18.37",
        "1000,-1850,19.37",
        "1000,-2050,20.37",
        "1000,-2250,21.37",
        "1000,-2450,22.37",
        "1000,-350,11.87",
        "1000,-150,10.87",
        "1000,50,10.37",
        "1000,250,11.37",
        "1000,450,12.37",
        "1000,650,13.37",
        "1000,850,14.37",
        "1000,1050,15.37",
        "1000,1250,16.37",
        "1000,1450,17.37",
        "1000,1650,18.37",
        "1000,1850,19.37",
        "1000,2050,20.37",
        "1000,2250,21.37",
        "1000,2450,22.37",
        "1200,-650,14.382",
        "1200,-850,15.382",
        "1200,-1050,16.382",
        "1200,-1250,17.382",
        "1200,-1450,18.382",
        "1200,-1650,19.382",
        "1200,-1850,20.382",
        "1200,-2050,21.382",
        "1200,-2250,22.382",
        "1200,-2450,23.382",
        "1200,-350,12.882",
        "1200,-150,11.882",
        "1200,50,11.382",
        "1200,250,12.382",
        "1200,450,13.382",
        "1200,650,14.382",
        "1200,850,15.382",
        "1200,1050,16.382",
        "1200,1250,17.382",
        "1200,1450,18.382",
        "1200,1650,19.382",
        "1200,1850,20.382",
        "1200,2050,21.382",
        "1200,2250,22.382",
        "1200,2450,23.382",
        "1400,-650,15.394",
        "1400,-850,16.394",
        "1400,-1050,17.394",
        "1400,-1250,18.394",
        "1400,-1450,19.394",
        "1400,-1650,20.394",
        "1400,-1850,21.394",
        "1400,-2050,22.394",
        "1400,-2250,23.394",
        "1400,-350,13.894",
        "1400,-150,12.894",
        "1400,50,12.394",
        "1400,250,13.394",
        "1400,450,14.394",
        "1400,650,15.394",
        "1400,850,16.394",
        "1400,1050,17.394",
        "1400,1250,18.394",
        "1400,1450,19.394",
        "1400,1650,20.394",
        "1400,1850,21.394",
        "1400,2050,22.394",
        "1400,2250,23.394",
        "1400,2450,24.394",
        "1600,-650,16.406",
        "1600,-850,17.406",
        "1600,-1050,18.406",
        "1600,-1250,19.406",
        "1600,-1450,20.406",
        "1600,-1650,21.406",
        "1600,-1850,22.406",
        "1600,-2050,23.406",
        "1600,-2250,24.406",
        "1600,-2450,25.406",
        "1600,-350,14.906",
        "1600,-150,13.906",
        "1600,50,13.406",
        "1600,250,14.406",
        "1600,450,15.406",
        "1600,650,16.406",
        "1600,850,17.406",
        "1600,1050,18.406",
        "1600,1250,19.406",
        "1600,1450,20.406",
        "1600,1650,21.406",
        "1600,1850,22.406",
        "1600,2050,23.406",
        "1600,2250,24.406",
        "1600,2450,25.406",
        "1800,-650,17.418",
        "1800,-850,18.418",
        "1800,-1050,19.418",
        "1800,-1250,20.418",
        "1800,-1450,21.418",
        "1800,-1650,22.418",
        "1800,-1850,23.418",
        "1800,-2050,24.418",
        "1800,-2250,25.418",
        "1800,-2450,26.418",
        "1800,-350,15.918",
        "1800,-150,14.918",
        "1800,50,14.418",
        "1800,250,15.418",
        "1800,450,16.418",
        "1800,650,17.418",
        "1800,850,18.418",
        "1800,1050,19.418",
        "1800,1250,20.418",
        "1800,1450,21.418",
        "1800,1650,22.418",
        "1800,1850,23.418",
        "1800,2050,24.418",
        "1800,2250,25.418",
        "1800,2450,26.418",
        "2000,-650,18.43",
        "2000,-850,19.43",
        "2000,-1050,20.43",
        "2000,-1250,21.43",
        "2000,-1450,22.43",
        "2000,-1650,23.43",
        "2000,-1850,24.43",
        "2000,-2050,25.43",
        "2000,-2250,26.43",
        "2000,-350,16.93",
        "2000,-150,15.93",
        "2000,50,15.43",
        "2000,250,16.43",
        "2000,450,17.43",
        "2000,650,18.43",
        "2000,850,19.43",
        "2000,1050,20.43",
        "2000,1250,21.43",
        "2000,1450,22.43",
        "2000,1650,23.43",
        "2000,1850,24.43",
        "2000,2050,25.43",
        "2000,2250,26.43",
        "3000,-650,18.43",
        "3000,-850,19.43",
        "3000,-1050,20.43",
        "3000,-1250,21.43",
        "3000,-1450,22.43",
        "3000,-1650,23.43",
        "3000,-1850,24.43",
        "3000,-2050,25.43",
        "3000,-2250,26.43",
        "3000,-2450,27.43",
        "3000,-350,16.93",
        "3000,-150,15.93",
        "3000,50,15.43",
        "3000,250,16.43",
        "3000,450,17.43",
        "3000,650,18.43",
        "3000,850,19.43",
        "3000,1050,20.43",
        "3000,1250,21.43",
        "3000,1450,22.43",
        "3000,1650,23.43",
        "3000,1850,24.43",
        "3000,2050,25.43",
        "3000,2250,26.43",
        "3000,2450,27.43",
    };
}