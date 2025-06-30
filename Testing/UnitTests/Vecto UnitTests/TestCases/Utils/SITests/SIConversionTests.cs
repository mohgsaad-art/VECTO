using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto_UnitTests.TestCases.Utils.SITests;

public class SIConversionTests
{
    [TestCase(1, 1000),
        TestCase(1e-3, 1),
        TestCase(2.65344, 2653.44)]
    public void SI_Convert_ConvertToGramm(double val, double converted)
    {
        var siVal = val.SI<Kilogram>();
        var siConv = siVal.ConvertToGramm();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("g", siConv.Units);
    }

    [TestCase(1000, 1),
    TestCase(1, 1e-3),
    TestCase(5243, 5.243)]
    public void SI_Convert_ConvertToTon(double val, double converted)
    {
        var siVal = val.SI<Kilogram>();
        var siConv = siVal.ConvertToTon();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("t", siConv.Units);
    }

    [TestCase(1, 3.6),
    TestCase(0.2777777777777777, 1),
    TestCase(13.7603, 49.53708)]
    public void SI_Convert_ConvertToKiloMeterPerHour(double val, double converted)
    {
        var siVal = val.SI<MeterPerSecond>();
        var siConv = siVal.ConvertToKiloMeterPerHour();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("km/h", siConv.Units);
    }

    [TestCase(1, 1e6),
    TestCase(1e-6, 1),
    TestCase(7.54214451, 7542144.51)]
    public void SI_Convert_ConvertToGrammPerKiloMeter(double val, double converted)
    {
        var siVal = val.SI<KilogramPerMeter>();
        var siConv = siVal.ConvertToGrammPerKiloMeter();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("g/km", siConv.Units);
    }

    [TestCase(1, 1e8),
    TestCase(1e-8, 1),
    TestCase(0.00935934235, 935934.235)]
    public void SI_Convert_ConvertToLiterPer100Kilometer(double val, double converted)
    {
        var siVal = val.SI<VolumePerMeter>();
        var siConv = siVal.ConvertToLiterPer100Kilometer();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("l/100km", siConv.Units);
    }

    [TestCase(1, 1e11),
    TestCase(1e-11, 1),
    TestCase(0.00013243241234, 13243241.234)]
    public void SI_Convert_ConvertToLiterPer100TonKiloMeter(double val, double converted)
    {
        var siVal = val.SI<VolumePerMeterMass>();
        var siConv = siVal.ConvertToLiterPer100TonKiloMeter();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("l/100tkm", siConv.Units);
    }

    [TestCase(1, 1e8),
    TestCase(1e-8, 1),
    TestCase(0.13243241234, 13243241.234)]
    public void SI_Convert_ConvertToLiterPerCubicMeter100KiloMeter(double val, double converted)
    {
        var siVal = val.SI<VolumePerMeterVolume>();
        var siConv = siVal.ConvertToLiterPerCubicMeter100KiloMeter();
        Assert.AreEqual(converted, siConv, 1e-6);
        Assert.AreEqual("l/100m³-km", siConv.Units);
    }

    [TestCase(1, 3.6e6),
    TestCase(0.277777777777777777e-6, 1),
    TestCase(0.0135897845, 13589.7845 * 3.6)]
    public void SI_Convert_ConvertToGrammPerHour(double val, double converted)
    {
        var siVal = val.SI<KilogramPerSecond>();
        var siConv = siVal.ConvertToGrammPerHour();
        Assert.AreEqual(converted, siConv, 1e-6);
        Assert.AreEqual("g/h", siConv.Units);
    }

    [TestCase(1, 1e-3),
    TestCase(1e3, 1),
    TestCase(4353.32, 4.35332)]
    public void SI_Convert_ConvertToKiloMeter(double val, double converted)
    {
        var siVal = val.SI<Meter>();
        var siConv = siVal.ConvertToKiloMeter();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("km", siConv.Units);
    }

    [TestCase(1, 1e6),
    TestCase(1e-6, 1),
    TestCase(0.053798513789, 53798.513789)]
    public void SI_Convert_ConvertToCubicCentiMeter(double val, double converted)
    {
        var siVal = val.SI<CubicMeter>();
        var siConv = siVal.ConvertToCubicCentiMeter();
        Assert.AreEqual(converted, siConv, 1e-6);
        Assert.AreEqual("cm³", siConv.Units);
    }

    [TestCase(1, 1e6),
    TestCase(1e-6, 1),
    TestCase(7.54214451, 7542144.51)]
    public void SI_Convert_ConvertToGrammPerCubicMeterKiloMeter(double val, double converted)
    {
        var siVal = val.SI<KilogramPerMeterCubicMeter>();
        var siConv = siVal.ConvertToGrammPerCubicMeterKiloMeter();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("g/m³-km", siConv.Units);
    }

    [TestCase(1, 1e9),
    TestCase(1e-9, 1),
    TestCase(7.54214451, 7542144510)]
    public void SI_Convert_ConvertToGrammPerTonKilometer(double val, double converted)
    {
        var siVal = val.SI<KilogramPerMeterMass>();
        var siConv = siVal.ConvertToGrammPerTonKilometer();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("g/t-km", siConv.Units);
    }

    [TestCase(1, 0.277777777777e-6),
    TestCase(3600e3, 1),
    TestCase(135890, 0.0377472222)]
    public void SI_Convert_ConvertToKiloWattHour(double val, double converted)
    {
        var siVal = val.SI<WattSecond>();
        var siConv = siVal.ConvertToKiloWattHour();
        Assert.AreEqual(converted, siConv, 1e-6);
        Assert.AreEqual("kWh", siConv.Units);
    }

    [TestCase(1, 1e-3),
    TestCase(1e3, 1),
    TestCase(23453, 23.453)]
    public void SI_Convert_ConvertToKiloWatt(double val, double converted)
    {
        var siVal = val.SI<Watt>();
        var siConv = siVal.ConvertToKiloWatt();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("kW", siConv.Units);
    }

    [TestCase(1, 9.549296586),
    TestCase(0.104719755, 1),
    TestCase(62.83185307, 600)]
    public void SI_Convert_ConvertToRoundsPerMinute(double val, double converted)
    {
        var siVal = val.SI<PerSecond>();
        var siConv = siVal.ConvertToRoundsPerMinute();
        Assert.AreEqual(converted, siConv, 1e-6);
        Assert.AreEqual(siVal.Value(), ((double)siConv).RPMtoRad().Value());
        Assert.AreEqual("rpm", siConv.Units);
    }

    [TestCase(1, 1e3),
    TestCase(1e-3, 1),
    TestCase(123.780, 123780)]
    public void SI_Convert_ConvertToCubicDeziMeter(double val, double converted)
    {
        var siVal = val.SI<CubicMeter>();
        var siConv = siVal.ConvertToCubicDeziMeter();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("dm^3", siConv.Units);
    }

    [TestCase(1, 1e3),
    TestCase(1e-3, 1),
    TestCase(0.255, 255)]
    public void SI_Convert_ConvertToMilliMeter(double val, double converted)
    {
        var siVal = val.SI<Meter>();
        var siConv = siVal.ConvertToMilliMeter();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("mm", siConv.Units);
    }


    [TestCase(0.2 / 1000 / 3600, 200)]
    public void SI_Convert_ConvertToGramPerKiloWattHour(double val, double converted)
    {
        var siVal = val.SI<SpecificFuelConsumption>();
        var siConv = siVal.ConvertToGramPerKiloWattHour();
        Assert.AreEqual(converted, siConv, 1e-12);
        Assert.AreEqual("g/kWh", siConv.Units);
    }

}