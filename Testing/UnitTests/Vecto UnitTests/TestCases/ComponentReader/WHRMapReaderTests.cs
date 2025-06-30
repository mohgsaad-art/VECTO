using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class WHRMapReaderTests
{
    [TestCase]
    public void TestWHRMapCSVDataElectric()
    {

        var data = "engine speed, torque, fuel consumption, whr power electric\n" +
                    "600, -100, 0, 100\n" +
                    "600, 500, 200, 400\n" +
                    "2400, -100, 0, 100\n" +
                    "2400, 500, 200, 400";

        var whrMap = WHRPowerReader.Create(VectoCSVFile.ReadStream(data.ToStream()), WHRType.ElectricalOutput);
        var result = whrMap.GetWHRPower(500.SI<NewtonMeter>(), 600.RPMtoRad(), true);

        Assert.IsFalse(result.Extrapolated);
        Assert.AreEqual(400, result.GeneratedPower.Value());
    }

    [TestCase]
    public void TestWHRMapCSVDataMechanical()
    {

        var data = "engine speed, torque, fuel consumption, whr power mechanical\n" +
                    "600, -100, 0, 100\n" +
                    "600, 500, 200, 400\n" +
                    "2400, -100, 0, 100\n" +
                    "2400, 500, 200, 400";

        var whrMap = WHRPowerReader.Create(VectoCSVFile.ReadStream(data.ToStream()), WHRType.MechanicalOutputDrivetrain);
        var result = whrMap.GetWHRPower(500.SI<NewtonMeter>(), 600.RPMtoRad(), true);

        Assert.IsFalse(result.Extrapolated);
        Assert.AreEqual(400, result.GeneratedPower.Value());
    }


    [TestCase]
    public void TestWHRMapCSVDataElectricAndMechanical()
    {

        var data = "engine speed, torque, fuel consumption, whr power electric, whr power mechanical\n" +
                    " 600, -100,   0,  50, 100\n" +
                    " 600,  500, 200, 200, 400\n" +
                    "2400, -100,   0, 100, 100\n" +
                    "2400,  500, 200, 200, 400";

        var whrMapEl = WHRPowerReader.Create(VectoCSVFile.ReadStream(data.ToStream()), WHRType.ElectricalOutput);
        var resultEl = whrMapEl.GetWHRPower(500.SI<NewtonMeter>(), 600.RPMtoRad(), true);

        Assert.IsFalse(resultEl.Extrapolated);
        Assert.AreEqual(200, resultEl.GeneratedPower.Value());

        var whrMapMech = WHRPowerReader.Create(VectoCSVFile.ReadStream(data.ToStream()), WHRType.MechanicalOutputDrivetrain);
        var resultMech = whrMapMech.GetWHRPower(500.SI<NewtonMeter>(), 600.RPMtoRad(), true);

        Assert.IsFalse(resultMech.Extrapolated);
        Assert.AreEqual(400, resultMech.GeneratedPower.Value());

    }

}