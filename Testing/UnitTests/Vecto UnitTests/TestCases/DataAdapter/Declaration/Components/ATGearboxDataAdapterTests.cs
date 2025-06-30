using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class ATGearboxDataAdapterTests
{
    GearboxType[] GbxTypes = new[] { GearboxType.ATSerial };

    [TestCase()]
    public void TestATGearboxLastGearDisabled()
    {
        var limits = new List<ITorqueLimitInputData>() {
            new TorqueLimitInputData() {
                Gear = 8,
                MaxTorque = 0.SI<NewtonMeter>()
            }
        };

        var inputData = GetMockInputData(limits);
        var runData = GetDummyVectoRunData(inputData.Components.GearboxInputData.Gears.Count);

        var dao = new GearboxDataAdapter(new TorqueConverterDataAdapter());
        var gbxData = dao.CreateGearboxData(inputData, runData, null, GbxTypes);

        Assert.AreEqual(7, gbxData.Gears.Count);
    }

    [TestCase()]
    public void TestATGearboxLastButOneGearDisabled()
    {
        var limits = new List<ITorqueLimitInputData>() {
            new TorqueLimitInputData() {
                Gear = 7,
                MaxTorque = 0.SI<NewtonMeter>()
            }
        };

        var inputData = GetMockInputData(limits);
        var runData = GetDummyVectoRunData(inputData.Components.GearboxInputData.Gears.Count);

        var dao = new GearboxDataAdapter(new TorqueConverterDataAdapter());

        AssertHelper.Exception<VectoException>(
            () =>
            {
                var gbx = dao.CreateGearboxData(inputData, runData, null, GbxTypes);
            }, "Only the last 1 or 2 gears can be disabled. Disabling gear 7 for a 8-speed gearbox is not allowed.");

    }

    [TestCase()]
    public void TestATGearboxLastTwoGearsDisabled()
    {
        var limits = new List<ITorqueLimitInputData>() {
            new TorqueLimitInputData() {
                Gear = 7,
                MaxTorque = 0.SI<NewtonMeter>()
            },
            new TorqueLimitInputData() {
                Gear = 8,
                MaxTorque = 0.SI<NewtonMeter>()
            }
        };

        var inputData = GetMockInputData(limits);
        var runData = GetDummyVectoRunData(inputData.Components.GearboxInputData.Gears.Count);

        var dao = new GearboxDataAdapter(new TorqueConverterDataAdapter());
        var gbxData = dao.CreateGearboxData(inputData, runData, null, GbxTypes);
        Assert.AreEqual(6, gbxData.Gears.Count);
    }

    [TestCase()]
    public void TestATGearboxFirstGearDisabled()
    {
        var limits = new List<ITorqueLimitInputData>() {
            new TorqueLimitInputData() {
                Gear = 1,
                MaxTorque = 0.SI<NewtonMeter>()
            },

        };

        var inputData = GetMockInputData(limits);
        var runData = GetDummyVectoRunData(inputData.Components.GearboxInputData.Gears.Count);

        var dao = new GearboxDataAdapter(new TorqueConverterDataAdapter());

        AssertHelper.Exception<VectoException>(() =>
        {
            var gbxData = dao.CreateGearboxData(inputData, runData, null, GbxTypes);
        }, messageContains: "Only the last 1 or 2 gears can be disabled.");
    }

    private IVehicleDeclarationInputData GetMockInputData(List<ITorqueLimitInputData> torqueLimits)
    {
        var input = new Mock<IVehicleDeclarationInputData>();
        var components = new Mock<IVehicleComponentsDeclaration>();
        input.Setup(i => i.Components).Returns(components.Object);
        input.Setup(i => i.TorqueLimits).Returns(torqueLimits);
        var gbx = new Mock<IGearboxDeclarationInputData>();
        var tc = new Mock<ITorqueConverterDeclarationInputData>();

        components.Setup(c => c.GearboxInputData).Returns(gbx.Object);
        components.Setup(c => c.TorqueConverterInputData).Returns(tc.Object);

        var gearRatios = new double[] {
            3.4, 1.9, 1.42, 1.0, 0.7, 0.62, 0.62, 0.62
        };
        var header = "Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]";
        var efficiency = 0.98;
        var data = new List<string>();
        foreach (var speed in new[] { 0, 10000 })
        {
            foreach (var tq in new[] { 1e5, -1e5, 0 })
            {
                data.Add($"{speed:f2}, {tq:f2}, {(1 - efficiency) * Math.Abs(tq)}");
            }
        }
        var lossmap = InputDataHelper.InputDataAsTableData(header, data.ToArray());
        var gears = gearRatios.Select((x, idx) =>
        {
            var gear = new Mock<ITransmissionInputData>();
            gear.Setup(g => g.Ratio).Returns(x);
            gear.Setup(g => g.Gear).Returns(idx + 1);
            //gear.Setup(g => g.Efficiency).Returns(0.98);
            gear.Setup(g => g.LossMap).Returns(lossmap);
            return gear.Object;
        }).ToList();
        gbx.Setup(g => g.Type).Returns(GearboxType.ATSerial);
        gbx.Setup(g => g.Gears).Returns(gears);

        var tcData = InputDataHelper.InputDataAsTableData(TcHeader, TcData);
        tc.Setup(t => t.TCData).Returns(tcData);
        return input.Object;
    }

    public const string TcHeader = "Speed Ratio, Torque Ratio,MP1000";

    public static readonly string[] TcData = new[] {
        "0.0,1.80,377.80",
        "0.1,1.71,365.21",
        "0.2,1.61,352.62",
        "0.3,1.52,340.02",
        "0.4,1.42,327.43",
        "0.5,1.33,314.84",
        "0.6,1.23,302.24",
        "0.7,1.14,264.46",
        "0.8,1.04,226.68",
        "0.9,1.02,188.90",
        "1.0,1.0,0.00",
        "1.100,0.999,-40.34",
        "1.222,0.998,-80.34",
        "1.375,0.997,-136.11",
        "1.571,0.996,-216.52",
        "1.833,0.995,-335.19",
        "2.200,0.994,-528.77",
        "2.750,0.993,-883.40",
        "4.400,0.992,-2462.17",
        "11.000,0.991,-16540.98",
    };

    public const string EngineFldHeader = "engine speed [1/min],full load torque [Nm],motoring torque [Nm],PT1 [s]";

    public static readonly string[] EngineFldData = new[] {
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

    private static VectoRunData GetDummyVectoRunData(int numGears)
    {
        var fldData = InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData);
        var fld = FullLoadCurveReader.Create(fldData);
        var runData = new VectoRunData()
        {
            VehicleData = new VehicleData()
            {
                DynamicTyreRadius = 0.465.SI<Meter>(),
            },
            EngineData = new CombustionEngineData()
            {
                Inertia = 0.SI<KilogramSquareMeter>(),
                FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(),
            }
        };
        for (uint i = 0; i <= numGears; i++)
            runData.EngineData.FullLoadCurves[i] = fld;
        return runData;
    }

}