using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.CombustionEngine;

[TestFixture]
public class PT1Calculations
{
	private const double Tolerance = 0.0001;

    [
		// fixed points
		TestCase(400, 0),
		TestCase(800, 0.47),
		TestCase(1000, 0.58),
		TestCase(1200, 0.53),
		TestCase(1400, 0.46),
		TestCase(1500, 0.43),
		TestCase(1750, 0.22),
		TestCase(1800, 0.2),
		TestCase(2000, 0.11),
		TestCase(2500, 0.11),
		// interpolate
		TestCase(600, 0.235),
		TestCase(900, 0.525),
		TestCase(1100, 0.555),
		TestCase(1300, 0.495),
		TestCase(1450, 0.445),
		TestCase(1625, 0.325),
		TestCase(1775, 0.21),
		TestCase(1900, 0.155),
		TestCase(2250, 0.11),
	]
	public void PT1Test(double rpm, double expectedPt1)
	{
		var pt1 = DeclarationData.PT1.Lookup(rpm.RPMtoRad());
		NUnit.Framework.Assert.AreEqual(expectedPt1, pt1.Value.Value(), Tolerance);
		NUnit.Framework.Assert.IsFalse(pt1.Extrapolated);
	}

	[TestCase(200),
	TestCase(0),
	TestCase(13000),]
	public void PT1ExceptionsTest(double rpm)
	{
		// EXTRAPOLATE 
		var tmp = DeclarationData.PT1.Lookup(rpm.RPMtoRad());
		NUnit.Framework.Assert.IsTrue(tmp.Extrapolated);
	}
}