using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class FullLoadCurveReaderTests
{
	private const double Tolerance = 0.0001;

    [TestCase]
    public void TestFullLoadStaticTorque()
    {
		var data = InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData);
		var fldCurve = FullLoadCurveReader.Create(data);

        Assert.AreEqual(1180, fldCurve.FullLoadStationaryTorque(560.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(1352, fldCurve.FullLoadStationaryTorque(2000.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(1231, fldCurve.FullLoadStationaryTorque(580.RPMtoRad()).Value(), Tolerance);
    }

    
    [TestCase]
    public void TestFullLoadStaticPower()
    {
		var data = InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData);
		var fldCurve = FullLoadCurveReader.Create(data);

        Assert.AreEqual(69198.814183, fldCurve.FullLoadStationaryPower(560.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(283162.218372, fldCurve.FullLoadStationaryPower(2000.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(74767.810760, fldCurve.FullLoadStationaryPower(580.RPMtoRad()).Value(), Tolerance);
    }

    [TestCase]
    public void TestDragLoadStaticTorque()
    {
		var data = InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData);
		var fldCurve = FullLoadCurveReader.Create(data);

        Assert.AreEqual(-149, fldCurve.DragLoadStationaryTorque(560.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(-301, fldCurve.DragLoadStationaryTorque(2000.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(-148.5, fldCurve.DragLoadStationaryTorque(580.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(-150, fldCurve.DragLoadStationaryTorque(520.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(-339, fldCurve.DragLoadStationaryTorque(2200.RPMtoRad()).Value(), Tolerance);
    }

    [TestCase]
    public void TestDragLoadStaticPower()
    {

		var data = InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData);
		var fldCurve = FullLoadCurveReader.Create(data);

        Assert.AreEqual(-8737.81636, fldCurve.DragLoadStationaryPower(560.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(-63041.29254, fldCurve.DragLoadStationaryPower(2000.RPMtoRad()).Value(), Tolerance);
        Assert.AreEqual(-9019.51251, fldCurve.DragLoadStationaryPower(580.RPMtoRad()).Value(), Tolerance);
    }

    [TestCase]
    public void TestPT1()
	{
		var data = InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData);
		var fldCurve = FullLoadCurveReader.Create(data);

        Assert.AreEqual(0.6, fldCurve.PT1(560.RPMtoRad()).Value.Value(), Tolerance);
        Assert.AreEqual(0.25, fldCurve.PT1(2000.RPMtoRad()).Value.Value(), Tolerance);
        Assert.AreEqual(0.37, fldCurve.PT1(1700.RPMtoRad()).Value.Value(), Tolerance);
    }

    /// <summary>
    ///     [VECTO-78]
    /// </summary>
    [TestCase]
    public void Test_FileRead_WrongFileFormat_InsufficientColumns()
	{
		const string hdr = "engine speed, full load torque";
		var fld = new string[] {
			"560,1180",
			"600,1282",
			"799.9999999,1791",
			"1000,2300",
			"1200,2300",
			"1400,2300",
			"1599.999999,2079",
			"1800,1857",
			"2000.000001,1352",
			"2100,1100",
		};
		var data = InputDataHelper.InputDataAsTableData(hdr, fld);
        AssertHelper.Exception<VectoException>(
            () => FullLoadCurveReader.Create(data),
            "Engine FullLoadCurve Data File must consist of at least 3 columns.");
    }


	public static List<string> LogList = new List<string>();

    /// <summary>
    /// [VECTO-78]
    /// </summary>
    [TestCase]
    public void Test_FileRead_HeaderColumnsNotNamedCorrectly()
    {
        LogList.Clear();
        var target = new MethodCallTarget {
            ClassName = typeof(FullLoadCurveReaderTests).AssemblyQualifiedName,
            MethodName = "LogMethod_Test_FileRead_HeaderColumnsNotNamedCorrectly"
        };
        target.Parameters.Add(new MethodCallParameter("${level}"));
        target.Parameters.Add(new MethodCallParameter("${message}"));
        SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Warn);
		var data = InputDataHelper.InputDataAsTableData("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s]", EngineFldData);

        FullLoadCurveReader.Create(data);
        Assert.IsTrue(
            LogList.Contains(
                "FullLoadCurve: Header Line is not valid. Expected: 'engine speed, full load torque, motoring torque', Got: 'n, Mfull, Mdrag, PT1'. Falling back to column index."),
            string.Join("\n", LogList));
        LogList.Clear();
    }

    public static void LogMethod_Test_FileRead_HeaderColumnsNotNamedCorrectly(string level, string message)
    {
        LogList.Add(message);
    }

    /// <summary>
    ///     [VECTO-78]
    /// </summary>
    [TestCase]
    public void Test_FileRead_NoHeader()
	{
		var data = InputDataHelper.InputDataAsTableData(null, EngineFldData);

        var curve = FullLoadCurveReader.Create(data);
        var result = curve.FullLoadStationaryTorque(1.SI<PerSecond>());
        Assert.AreNotEqual(result.Value(), 0.0);
    }

    /// <summary>
    ///     [VECTO-78]
    /// </summary>
    [TestCase]
    public void Test_FileRead_InsufficientEntries()
	{
		var data = InputDataHelper.InputDataAsTableData(EngineFldHeader, EngineFldData.Slice(0, 1));

        AssertHelper.Exception<VectoException>(
			() => FullLoadCurveReader.Create(data),
            "FullLoadCurve must consist of at least two lines with numeric values (below file header)");
    }

	/// <summary>
	///     [VECTO-190]
	/// </summary>
	[TestCase]
	public void TestSortingFullLoadEntries()
	{
		var fldEntries = new[] {
			"600,1282,-148,0.6			 ",
			"799.9999999,1791,-149,0.6	 ",
			"560,1180,-149,0.6			 ",
			"1000,2300,-160,0.6			 ",
			"1599.999999,2079,-235,0.49	 ",
			"1200,2300,-179,0.6			 ",
			"1800,1857,-264,0.25		 ",
			"1400,2300,-203,0.6			 ",
			"2000.000001,1352,-301,0.25	 ",
			"2100,1100,-320,0.25		 ",
		};

		var data = InputDataHelper.InputDataAsTableData(EngineFldHeader, fldEntries);

        var fldCurve = FullLoadCurveReader.Create(data);

		Assert.AreEqual(1180, fldCurve.FullLoadStationaryTorque(560.RPMtoRad()).Value(), Tolerance);
		Assert.AreEqual(1352, fldCurve.FullLoadStationaryTorque(2000.RPMtoRad()).Value(), Tolerance);
		Assert.AreEqual(1231, fldCurve.FullLoadStationaryTorque(580.RPMtoRad()).Value(), Tolerance);
	}

	[TestCase]
	public void TestDuplicateEntries()
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
			"1200,2410,-180,0.6",
		};
		var data = InputDataHelper.InputDataAsTableData(EngineFldHeader, fldData);

		AssertHelper.Exception<VectoException>(
			() => {
				var fldCurve = FullLoadCurveReader.Create(data);
			}, messageContains: "Error reading full-load curve: multiple entries for engine speeds 1200");
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