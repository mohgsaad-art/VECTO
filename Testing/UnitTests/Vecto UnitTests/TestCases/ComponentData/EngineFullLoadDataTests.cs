using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class EngineFullLoadDataTests
{
	private const double Tolerance = 0.0001;

	/// <summary>
	///		VECTO-190
	/// </summary>
	[TestCase]
	public void TestFullLoadSorting()
	{
		var gbxFldString = new[] {
			"600, 1000, -100",
			"2400, 2000, -120",
			"1000, 500, -110"
		};

		var dataGbx =
			InputDataHelper.InputDataAsTableData("n [U/min],Mfull [Nm], Mdrag [Nm]", gbxFldString);
		var gbxFld = FullLoadCurveReader.Create(dataGbx, true);

		var maxTorque = gbxFld.FullLoadStationaryTorque(800.RPMtoRad());
		Assert.AreEqual(750, maxTorque.Value());
	}

    [TestCase]
	public void TestFullLoadEngineSpeedRated()
	{
		var fldCurve = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData));
		Assert.AreEqual(181.8444, fldCurve.RatedSpeed.Value(), Tolerance);
	}

    [TestCase]
    public void TestPreferredSpeed()
    {
		var fldCurve = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData));
        fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };
        AssertHelper.AreRelativeEqual(130.691151551712.SI<PerSecond>(), fldCurve.PreferredSpeed);
        var totalArea = fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.N95hSpeed);
        Assert.AreEqual((0.51 * totalArea).Value(),
            fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.PreferredSpeed).Value(), 1E-3);
        AssertHelper.AreRelativeEqual(194.515816596908.SI<PerSecond>(), fldCurve.N95hSpeed);
        AssertHelper.AreRelativeEqual(94.24639.SI<PerSecond>(), fldCurve.LoSpeed);
        //AssertHelper.AreRelativeEqual(219.084329211505.SI<PerSecond>(), fldCurve.HiSpeed);
        AssertHelper.AreRelativeEqual(2300.SI<NewtonMeter>(), fldCurve.MaxTorque);
        AssertHelper.AreRelativeEqual(-320.SI<NewtonMeter>(), fldCurve.MaxDragTorque);
    }

    [TestCase()]
    public void TestP99HighSpeed()
    {
		var fldCurve = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData));
        fldCurve.EngineData = new CombustionEngineData() { IdleSpeed = 560.RPMtoRad() };

        var nP99h = fldCurve.NP99hSpeed;

        Assert.IsTrue(nP99h > fldCurve.PreferredSpeed);
        Assert.AreEqual(fldCurve.MaxPower.Value() * 0.99, (fldCurve.FullLoadStationaryTorque(nP99h) * nP99h).Value(), 1e-3);
        Assert.AreEqual(1810.67898, nP99h.AsRPM, 1e-3);
    }

    [TestCase()]
    public void TestTq99HighSpeed()
    {
		var fldCurve = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData));
        fldCurve.EngineData = new CombustionEngineData() { IdleSpeed = 560.RPMtoRad() };

        var nTq99h = fldCurve.NTq99hSpeed;

        Assert.IsTrue(nTq99h > fldCurve.PreferredSpeed);
        Assert.AreEqual(fldCurve.MaxTorque.Value() * 0.99, fldCurve.FullLoadStationaryTorque(nTq99h).Value(), 1e-3);
        Assert.AreEqual(1420.8144, nTq99h.AsRPM, 1e-3);
    }


    [TestCase()]
    public void TestTq99LowSpeed()
    {
		var fldCurve = FullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData));
        fldCurve.EngineData = new CombustionEngineData() { IdleSpeed = 560.RPMtoRad() };

        var nTq99l = fldCurve.NTq99lSpeed;

        Assert.IsTrue(nTq99l < fldCurve.PreferredSpeed);
        Assert.AreEqual(fldCurve.MaxTorque.Value() * 0.99, fldCurve.FullLoadStationaryTorque(nTq99l).Value(), 1e-3);
        Assert.AreEqual(990.9626, nTq99l.AsRPM, 1e-3);
    }

    [TestCase]
    public void TestPreferredSpeed2()
    {
        var fldData = new[] {
                "560,1180,-149,0.6",
                "600,1282,-148,0.6",
                "800,1791,-149,0.6",
                "1000,2300,-160,0.6",
                "1200,2400,-179,0.6",
                "1400,2300,-203,0.6",
                "1600,2079,-235,0.49",
                "1800,1857,-264,0.25",
                "2000,1352,-301,0.25",
                "2100,1100,-320,0.25",
            };
        var fldEntries = InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s] ", fldData);
        var fldCurve = FullLoadCurveReader.Create(VectoCSVFile.ReadStream(fldEntries));
        fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };

        var totalArea = fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.N95hSpeed);
        Assert.AreEqual((0.51 * totalArea).Value(),
            fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.PreferredSpeed).Value(), 1E-3);
        //AssertHelper.AreRelativeEqual(130.691151551712.SI<PerSecond>(), fldCurve.PreferredSpeed);
    }

    [TestCase]
    public void TestN95hSpeedInvalid()
    {
        var fldData = new[] {
                "600,539.8228,-59.02274, 1.0",
                "821,673.5694587,-62.77795, 1.0",
                "1041,1102.461949,-68.37734, 1.0",
                "1262,1112.899122,-76.0485, 1.0",
                "1482,1098.632364,-85.00573, 1.0",
                "1606,1093.403667,-90.9053, 1.0",
                "1800,1058.081866,-100.937, 1.0",
                "1995,992.0155535,-112.1166, 1.0",
                "2189,926.7779212,-124.9432, 1.0",
                "4000,811.7189964,-138.7132, 1.0",
            };
        var fldEntries = InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s] ", fldData);
        var fldCurve = FullLoadCurveReader.Create(VectoCSVFile.ReadStream(fldEntries));
        fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };

        AssertHelper.Exception<VectoException>(() => { var tmp = fldCurve.N95hSpeed; });
        //var totalArea = fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, );
        //Assert.AreEqual((0.51 * totalArea).Value(),
        //	fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.PreferredSpeed).Value(), 1E-3);
        //AssertHelper.AreRelativeEqual(130.691151551712.SI<PerSecond>(), fldCurve.PreferredSpeed);
    }

    /// <summary>
    ///		[VECTO-679]
    /// </summary>
    [TestCase]
    public void TestN95hComputation()
    {
        var fldData = new[] {
                "590,486,-44,0.60	",
                "600,486,-44,0.60	",
                "800,755,-54,0.60	",
                "1000,883,-62,0.60	",
                "1200,899,-74,0.60	",
                "1300,899,-80,0.60	",
                "1400,899,-87,0.60	",
                "1500,899,-92,0.60	",
                "1600,899,-97,0.60	",
                "1700,890,-100,0.60	",
                "1800,881,-103,0.60	",
                "1900,867,-107,0.60	",
                "2000,853,-111,0.43	",
                "2150,811,-118,0.29	",
                "2200,802,-125,0.25	",
                "2300,755,-130,0.25	",
                "2400,705,-135,0.25	",
                "2500,644,-140,0.25	",
                "2600,479,-145,0.25	",
                "2700,0,-149,0.25	",
            };
        var fldEntries = InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s] ", fldData);
        var fldCurve = FullLoadCurveReader.Create(VectoCSVFile.ReadStream(fldEntries));
        fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };

        Assert.AreEqual(2420.5, fldCurve.N95hSpeed.AsRPM, 1);

    }

	[TestCase]
	public void TestFullLoadCurveIntersection()
	{
		var engineFldString = new[] {
			"560, 1180, -149",
			"600, 1282, -148",
			"800, 1791, -149",
			"1000, 2300, -160",
			"1200, 2300, -179",
			"1400, 2300, -203",
			"1600, 2079, -235",
			"1800, 1857, -264",
			"2000, 1352, -301",
			"2100, 1100, -320",
		};
		var dataEng =
			InputDataHelper.InputDataAsTableData("n [U/min],Mfull [Nm],Mdrag [Nm]", engineFldString);
		var engineFld = FullLoadCurveReader.Create(dataEng, true);


		var fullLoadCurve = AbstractSimulationDataAdapter.IntersectFullLoadCurves(engineFld, 2500.SI<NewtonMeter>());

		Assert.AreEqual(10, fullLoadCurve.FullLoadEntries.Count);

		Assert.AreEqual(1180.0, fullLoadCurve.FullLoadStationaryTorque(560.RPMtoRad()).Value());
		Assert.AreEqual(1100.0, fullLoadCurve.FullLoadStationaryTorque(2100.RPMtoRad()).Value());
	}

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
}