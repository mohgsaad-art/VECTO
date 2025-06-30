using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class EngineDataAdapterTests
{
    [TestCase()]
    public void TestGearboxTorqueLimitsAbove90FLD()
    {
        // ICE max torque = 956 Nm; 90% of 956 = 860.4 Nm.
        // 865 Nm is above this threshold, so limits are applied to the lower 50% of gears
        var maxGbxTq = 865.SI<NewtonMeter>();

        var doa = new CombustionEngineComponentDataAdapter();

        var vehicleInputData = GetMockVehicleInputData(maxGbxTq, null);
        var mission = new Mission()
        {
            MissionType = MissionType.RegionalDelivery
        };

        var engineData = doa.CreateEngineData(vehicleInputData, vehicleInputData.Components.EngineInputData.EngineModes.First(), mission);

        // check default FLD
        Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

        // check first gear - limited by gbx
        Assert.AreEqual(865, engineData.FullLoadCurves[1].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

        // check fourth gear - limited by gbx but not applicaple
        Assert.AreEqual(956, engineData.FullLoadCurves[4].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

        // check last gear - limited by gbx but not applicaple
        Assert.AreEqual(956, engineData.FullLoadCurves[6].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
    }

    [TestCase()]
    public void TestGearboxTorqueLimitsBelow90FLD()
    {
        // ICE max torque = 956 Nm; 90% of 956 = 860.4 Nm.
        // 800 Nm is lower than this threshold, so limit applies to all gears
        var maxGbxTq = 800.SI<NewtonMeter>();

        var doa = new CombustionEngineComponentDataAdapter();

        var vehicleInputData = GetMockVehicleInputData(maxGbxTq, null);
        var mission = new Mission()
        {
            MissionType = MissionType.RegionalDelivery
        };

        var engineData = doa.CreateEngineData(vehicleInputData, vehicleInputData.Components.EngineInputData.EngineModes.First(), mission);

        // check default FLD
        Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

        // check first gear - limited by gbx
        Assert.AreEqual(800, engineData.FullLoadCurves[1].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

        // check fourth gear - limited by gbx
        Assert.AreEqual(800, engineData.FullLoadCurves[4].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

        // check last gear - limited by gbx
        Assert.AreEqual(800, engineData.FullLoadCurves[6].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
    }

    [TestCase()]
    public void TestVehicleTorqueLimitsAbove95FLD()
    {
        // ICE max torque = 956 Nm; 95% of 956 = 908.2 Nm.
        // 910 Nm is higher than this threshold, so  no limit is applied
        var maxVehTq = 910.SI<NewtonMeter>();
        var doa = new CombustionEngineComponentDataAdapter();

        var vehicleInputData = GetMockVehicleInputData(null, maxVehTq);
        var mission = new Mission()
        {
            MissionType = MissionType.RegionalDelivery
        };

        var engineData = doa.CreateEngineData(vehicleInputData, vehicleInputData.Components.EngineInputData.EngineModes.First(), mission);

        // check default FLD
        Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

        // check first gear - limited by vehicle but not applicaple
        Assert.AreEqual(956, engineData.FullLoadCurves[1].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

        // check fourth gear - limited by vehicle but not applicaple
        Assert.AreEqual(956, engineData.FullLoadCurves[4].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

        // check last gear - limited by vehicle but not applicaple
        Assert.AreEqual(956, engineData.FullLoadCurves[6].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
    }

    [TestCase()]
    public void TestVehicleTorqueLimitsBelow95FLD()
    {
        // ICE max torque = 956 Nm; 95% of 956 = 908.2 Nm.
        // 850 Nm is lower than this threshold, so the upper half of the gears is limited to 850 Nm
        var maxVehTq = 850.SI<NewtonMeter>();

        var doa = new CombustionEngineComponentDataAdapter();

        var vehicleInputData = GetMockVehicleInputData(null, maxVehTq);
        var mission = new Mission()
        {
            MissionType = MissionType.RegionalDelivery
        };

        var engineData = doa.CreateEngineData(vehicleInputData, vehicleInputData.Components.EngineInputData.EngineModes.First(), mission);

        // check default FLD
        Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

        // check first gear - limited by vehicle but not applicaple
        Assert.AreEqual(956, engineData.FullLoadCurves[1].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

        // check fourth gear - limited by vehicle
        Assert.AreEqual(850, engineData.FullLoadCurves[4].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

        // check last gear - limited by vehicle 
        Assert.AreEqual(850, engineData.FullLoadCurves[6].MaxTorque.Value());
        Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
    }

    private IVehicleDeclarationInputData GetMockVehicleInputData(NewtonMeter? maxGbxTq, NewtonMeter? maxVehTq)
    {
        var vehicle = new Mock<IVehicleDeclarationInputData>();

        var components = new Mock<IVehicleComponentsDeclaration>();
        vehicle.Setup(v => v.Components).Returns(components.Object);

        var eng = new Mock<IEngineDeclarationInputData>();
        components.Setup(c => c.EngineInputData).Returns(eng.Object);
        var gbx = new Mock<IGearboxDeclarationInputData>();
        components.Setup(c => c.GearboxInputData).Returns(gbx.Object);

        vehicle.Setup(v => v.EngineIdleSpeed).Returns(600.RPMtoRad());

        eng.Setup(e => e.Displacement).Returns(6.871.SI<Liter>().Cast<CubicMeter>());
        var mode = new Mock<IEngineModeDeclarationInputData>();
        eng.Setup(e => e.EngineModes).Returns(new List<IEngineModeDeclarationInputData>() { mode.Object });
        var fuel = new Mock<IEngineFuelDeclarationInputData>();
        mode.Setup(m => m.Fuels).Returns(new List<IEngineFuelDeclarationInputData>() { fuel.Object });

        mode.Setup(m => m.IdleSpeed).Returns(vehicle.Object.EngineIdleSpeed);
        mode.Setup(m => m.FullLoadCurve).Returns(InputDataHelper.InputDataAsTableData(IceFldHdr, IceFldData));
        fuel.Setup(f => f.FuelType).Returns(FuelType.DieselCI);
        fuel.Setup(f => f.WHTCMotorway).Returns(1.0);
        fuel.Setup(f => f.WHTCRural).Returns(1.0);
        fuel.Setup(f => f.WHTCUrban).Returns(1.0);
        fuel.Setup(f => f.ColdHotBalancingFactor).Returns(1.0);
        fuel.Setup(f => f.CorrectionFactorRegPer).Returns(1.0);
        fuel.Setup(f => f.FuelConsumptionMap).Returns(InputDataHelper.InputDataAsTableData(IceMapHdr, IceMapData));

        var ratios = new[] { 6.7, 3.8, 2.29, 1.48, 1.0, 0.73 };
        var gears = ratios.Select((r, idx) =>
        {
            var g = new Mock<ITransmissionInputData>();
            g.Setup(x => x.Ratio).Returns(r);
            g.Setup(x => x.Gear).Returns(idx + 1);
            if (maxGbxTq != null)
                g.Setup(x => x.MaxTorque).Returns(maxGbxTq);
            return g.Object;
        }).ToList();
        gbx.Setup(g => g.Gears).Returns(gears);
        gbx.Setup(g => g.Type).Returns(GearboxType.MT);

        if (maxVehTq != null)
        {
            var tqLimits = ratios.Select((r, idx) =>
            {
                var t = new Mock<ITorqueLimitInputData>();
                t.Setup(x => x.Gear).Returns(idx + 1);
                t.Setup(x => x.MaxTorque).Returns(maxVehTq);
                return t.Object;
            }).ToList();
            vehicle.Setup(v => v.TorqueLimits).Returns(tqLimits);
        }

        return vehicle.Object;
    }


    const string IceFldHdr = "engine speed [1/min], full load torque [Nm], motoring torque [Nm]";

    private static readonly string[] IceFldData = new[] {
        "600,478,-35",
        "608,485.52,-35.31693",
        "616,493.04,-35.63385",
        "624,500.56,-35.95077",
        "632,508.08,-36.26769",
        "640,515.6,-36.58462",
        "648,523.12,-36.90154",
        "656,530.64,-37.21846",
        "664,538.16,-37.53539",
        "672,545.68,-37.85231",
        "680,553.2,-38.16923",
        "688,560.72,-38.48615",
        "696,568.24,-38.80308",
        "704,575.76,-39.12",
        "712,583.28,-39.43692",
        "720,590.8,-39.75385",
        "728,598.32,-40.07077",
        "736,605.84,-40.3877",
        "744,613.36,-40.70462",
        "752,620.88,-41.02154",
        "760,628.4,-41.33846",
        "768,635.92,-41.65539",
        "776,643.44,-41.97231",
        "784,650.96,-42.28923",
        "792,658.48,-42.60616",
        "800,666,-42.92308",
        "808,673.44,-43.24",
        "816,680.88,-43.48",
        "824,688.32,-43.72",
        "832,695.76,-43.96",
        "840,703.2,-44.2",
        "848,710.64,-44.44",
        "856,718.08,-44.68",
        "864,725.52,-44.92",
        "872,732.96,-45.16",
        "880,740.4,-45.4",
        "888,747.84,-45.64",
        "896,755.28,-45.88",
        "904,762.72,-46.12",
        "912,770.16,-46.36",
        "920,777.6,-46.6",
        "928,785.04,-46.84",
        "936,792.48,-47.08",
        "944,799.92,-47.32",
        "952,807.36,-47.56",
        "960,814.8,-47.8",
        "968,822.24,-48.04",
        "976,829.68,-48.28",
        "984,837.12,-48.52",
        "992,844.56,-48.76",
        "1000,852,-49",
        "1008,856.16,-49.4",
        "1016,860.32,-49.8",
        "1024,864.48,-50.19579",
        "1032,868.64,-50.59098",
        "1040,872.8,-50.98618",
        "1048,876.96,-51.38137",
        "1056,881.12,-51.77656",
        "1064,885.28,-52.17175",
        "1072,889.44,-52.56694",
        "1080,893.6,-52.96214",
        "1088,897.76,-53.35733",
        "1096,901.92,-53.75252",
        "1104,906.08,-54.14772",
        "1112,910.24,-54.54291",
        "1120,914.4,-54.9381",
        "1128,918.56,-55.33329",
        "1136,922.72,-55.72849",
        "1144,926.88,-56.12368",
        "1152,931.04,-56.51887",
        "1160,935.2,-56.91406",
        "1168,939.36,-57.30925",
        "1176,943.52,-57.70444",
        "1184,947.68,-58.09964",
        "1192,951.84,-58.49483",
        "1200,956,-58.89002",
        "1208,956,-59.28522",
        "1216,956,-59.68041",
        "1224,956,-60.0756",
        "1232,956,-60.44",
        "1240,956,-60.8",
        "1248,956,-61.16",
        "1256,956,-61.52",
        "1264,956,-61.88",
        "1272,956,-62.24",
        "1280,956,-62.6",
        "1288,956,-62.96",
        "1296,956,-63.32",
        "1304,956,-63.68",
        "1312,956,-64.04",
        "1320,956,-64.4",
        "1328,956,-64.76",
        "1336,956,-65.12",
        "1344,956,-65.48",
        "1352,956,-65.84",
        "1360,956,-66.2",
        "1368,956,-66.56",
        "1376,956,-66.92",
        "1384,956,-67.28",
        "1392,956,-67.64",
        "1400,956,-68",
        "1408,956,-68.36",
        "1416,956,-68.72",
        "1424,956,-69.08",
        "1432,956,-69.44",
        "1440,956,-69.76736",
        "1448,956,-70.08386",
        "1456,956,-70.40035",
        "1464,956,-70.71684",
        "1472,956,-71.03333",
        "1480,956,-71.34982",
        "1488,956,-71.66631",
        "1496,956,-71.9828",
        "1504,956,-72.2993",
        "1512,956,-72.61579",
        "1520,956,-72.93228",
        "1528,956,-73.24877",
        "1536,956,-73.56526",
        "1544,956,-73.88175",
        "1552,956,-74.19825",
        "1560,956,-74.51474",
        "1568,956,-74.83123",
        "1576,956,-75.14772",
        "1584,956,-75.46421",
        "1592,956,-75.7807",
        "1600,956,-76.0972",
        "1608,953.56,-76.41369",
        "1616,951.12,-76.73018",
        "1624,948.68,-77.04667",
        "1632,946.24,-77.36316",
        "1640,943.8,-77.67965",
        "1648,941.36,-77.99614",
        "1656,938.92,-78.31264",
        "1664,936.48,-78.6",
        "1672,934.04,-78.8",
        "1680,931.6,-79",
        "1688,929.16,-79.2",
        "1696,926.72,-79.4",
        "1704,924.28,-79.6",
        "1712,921.84,-79.8",
        "1720,919.4,-80",
        "1728,916.96,-80.2",
        "1736,914.52,-80.4",
        "1744,912.08,-80.6",
        "1752,909.64,-80.8",
        "1760,907.2,-81",
        "1768,904.76,-81.2",
        "1776,902.32,-81.4",
        "1784,899.88,-81.6",
        "1792,897.44,-81.8",
        "1800,895,-82",
        "1808,892.24,-82.24",
        "1816,889.48,-82.48",
        "1824,886.72,-82.72",
        "1832,883.96,-82.96",
        "1840,881.2,-83.2",
        "1848,878.44,-83.44",
        "1856,875.68,-83.68",
        "1864,872.92,-83.92",
        "1872,870.16,-84.16",
        "1880,867.4,-84.4",
        "1888,864.64,-84.64",
        "1896,861.88,-84.88",
        "1904,859.12,-85.12",
        "1912,856.36,-85.36",
        "1920,853.6,-85.6",
        "1928,850.84,-85.84",
        "1936,848.08,-86.08",
        "1944,845.32,-86.32",
        "1952,842.56,-86.56",
        "1960,839.8,-86.8",
        "1968,837.04,-87.04",
        "1976,834.28,-87.28",
        "1984,831.52,-87.52",
        "1992,828.76,-87.76",
        "2000,826,-88",
        "2008,823.36,-88.44",
        "2016,820.72,-88.88",
        "2024,818.08,-89.32",
        "2032,815.44,-89.76",
        "2040,812.8,-90.2",
        "2048,810.16,-90.64",
        "2056,807.52,-91.08",
        "2064,804.88,-91.52",
        "2072,802.24,-91.96",
        "2080,799.6,-92.4",
        "2088,796.96,-92.84",
        "2096,794.32,-93.28",
        "2104,791.68,-93.72",
        "2112,789.04,-94.16",
        "2120,786.4,-94.6",
        "2128,783.76,-95.04",
        "2136,781.12,-95.48",
        "2144,778.48,-95.92",
        "2152,775.84,-96.36",
        "2160,773.2,-96.8",
        "2168,770.56,-97.24",
        "2176,767.92,-97.68",
        "2184,765.28,-98.06694",
        "2192,762.64,-98.40081",
        "2200,760,-98.73468",
        "2208,755.56,-99.06856",
        "2216,751.12,-99.40243",
        "2224,746.68,-99.73631",
        "2232,742.24,-100.0702",
        "2240,737.8,-100.404",
        "2248,733.36,-100.7379",
        "2256,728.92,-101.0718",
        "2264,724.48,-101.4057",
        "2272,720.04,-101.7395",
        "2280,715.6,-102.0734",
        "2288,711.16,-102.4073",
        "2296,706.72,-102.7412",
        "2304,702.28,-103.075",
        "2312,697.84,-103.4089",
        "2320,693.4,-103.7428",
        "2328,688.96,-104.0767",
        "2336,684.52,-104.4105",
        "2344,680.08,-104.7444",
        "2352,675.64,-105.0783",
        "2360,671.2,-105.4",
        "2368,666.76,-105.72",
        "2376,662.32,-106.04",
        "2384,657.88,-106.36",
        "2392,653.44,-106.68",
        "2400,649,-107",
        "2408,642.36,-107.32",
        "2416,635.72,-107.64",
        "2424,629.08,-107.96",
        "2432,622.44,-108.28",
        "2440,615.8,-108.6",
        "2448,609.16,-108.92",
        "2456,602.52,-109.24",
        "2464,595.88,-109.56",
        "2472,589.24,-109.88",
        "2480,582.6,-110.2",
        "2488,575.96,-110.52",
        "2496,569.32,-110.84",
        "2504,543.36,-111.16",
        "2512,498.08,-111.48",
        "2520,452.8,-111.8",
        "2528,407.52,-112.12",
        "2536,362.24,-112.44",
        "2544,316.96,-112.76",
        "2552,271.68,-113.08",
        "2560,226.4,-113.4",
        "2568,181.12,-113.72",
        "2576,135.84,-114.04",
        "2584,90.56,-114.36",
        "2592,45.28,-114.68",
        "2600,0,-115",
    };

    const string IceMapHdr = "engine speed [rpm], torque [Nm], fuel consumption [g/h]";

    private static readonly string[] IceMapData = new[] {
        "500,-31,0",
        "500,0,508",
        "500,95.6,1814.959",
        "500,191.2,3075.43",
        "500,286.8,4327.79",
        "500,382.4,6036.866",
        "500,478,7983",
        "500,573.6,9771.095",
        "600,-35,0",
        "600,0,508",
        "600,95.6,1814.959",
        "600,191.2,3075.43",
        "600,286.8,4327.79",
        "600,382.4,6036.866",
        "600,478,7983",
        "600,573.6,9771.095",
        "808,-43.24,0",
        "808.5,0,737.35",
        "808.5,95.6,2156.667",
        "808.5,191.2,3750.051",
        "808.5,286.8,5348.091",
        "808.5,382.4,7281.769",
        "808.5,478,9331.995",
        "808.5,573.6,11361.22",
        "808.5,669.2,13292.96",
        "808.5,673.905,13387.96",
        "808,769.505,15319.69",
        "1017,-49.85,0",
        "1017,0,966.7",
        "1017,95.6,2499.359",
        "1017,191.2,4425.586",
        "1017,286.8,6368.761",
        "1017,382.4,8527.475",
        "1017,478,10681.08",
        "1017,573.6,12806.98",
        "1017,669.2,14926.89",
        "1017,764.8,17075.42",
        "1017,860.4,19211.62",
        "1017,860.84,19221.39",
        "1017,956.44,21357.58",
        "1225,-60.125,0",
        "1225.4,0,1216.133",
        "1225.4,95.6,2867.396",
        "1225.4,191.2,5129.114",
        "1225.4,286.8,7421.546",
        "1225.4,382.4,9808.684",
        "1225.4,478,12096.76",
        "1225.4,573.6,14371.23",
        "1225.4,669.2,16697.39",
        "1225.4,764.8,19043.79",
        "1225.4,860.4,21380.34",
        "1225.4,956,23976.15",
        "1225,1051.6,26399.12",
        "1434,-69.53,0",
        "1433.9,0,1607.511",
        "1433.9,95.6,3422.282",
        "1433.9,191.2,6045.75",
        "1433.9,286.8,8717.55",
        "1433.9,382.4,11388.84",
        "1433.9,478,14040.14",
        "1433.9,573.6,16812.16",
        "1433.9,669.2,19499.88",
        "1433.9,764.8,22089.68",
        "1433.9,860.4,24706.84",
        "1433.9,956,27415.66",
        "1434,1051.6,30063.37",
        "1662,-78.55,0",
        "1661.8,0,2026.982",
        "1661.8,95.6,4054.852",
        "1661.8,191.2,7064.631",
        "1661.8,286.8,10168.59",
        "1661.8,382.4,13313.27",
        "1661.8,478,16389.77",
        "1661.8,573.6,19514.32",
        "1661.8,669.2,22625.12",
        "1661.8,764.8,25652.52",
        "1661.8,860.4,28788.1",
        "1661.8,937.151,31372.42",
        "1662,1032.751,34529.97",
        "1835,-83.05,0",
        "1834.7,0,2385.627",
        "1834.7,95.6,4596.783",
        "1834.7,191.2,7871.156",
        "1834.7,286.8,11300.52",
        "1834.7,382.4,14757.68",
        "1834.7,478,18117.38",
        "1834.7,573.6,21557.68",
        "1834.7,669.2,25079.78",
        "1834.7,764.8,28600.34",
        "1834.7,860.4,32191.22",
        "1834.7,883.0285,33047.82",
        "1835,978.6285,36639.92",
        "2008,-88.44,0",
        "2007.5,0,2806.425",
        "2007.5,95.6,5238.11",
        "2007.5,191.2,8755.323",
        "2007.5,286.8,12501.62",
        "2007.5,382.4,16278.62",
        "2007.5,478,20040.57",
        "2007.5,573.6,23826.03",
        "2007.5,669.2,27760.66",
        "2007.5,764.8,31692.9",
        "2007.5,823.525,34019.71",
        "2008,919.125,37924.6",
        "2180,-97.9,0",
        "2180.3,0,3323.097",
        "2180.3,95.6,5859.055",
        "2180.3,191.2,9668.133",
        "2180.3,286.8,13730.37",
        "2180.3,382.4,17786.81",
        "2180.3,478,21943.1",
        "2180.3,573.6,26354.73",
        "2180.3,669.2,30668.08",
        "2180.3,764.8,34924.28",
        "2180.3,766.501,35000.3",
        "2180,862.101,39256.51",
        "2353,-105.12,0",
        "2353.2,0,3807.896",
        "2353.2,95.6,6495.978",
        "2353.2,191.2,10634.86",
        "2353.2,286.8,15048",
        "2353.2,382.4,19654.95",
        "2353.2,478,24298.67",
        "2353.2,573.6,29311.43",
        "2353.2,669.2,34144.93",
        "2353,764.8,39097.94",
        "2453,-109.12,0",
        "2453.2,0,3807.896",
        "2453.2,95.6,6495.978",
        "2453.2,191.2,10634.86",
        "2453.2,286.8,15048",
        "2453.2,382.4,19654.95",
        "2453.2,478,24298.67",
        "2453.2,573.6,29311.43",
        "2453.2,669.2,34144.93",
        "2453,764.8,39097.94",
    };
}