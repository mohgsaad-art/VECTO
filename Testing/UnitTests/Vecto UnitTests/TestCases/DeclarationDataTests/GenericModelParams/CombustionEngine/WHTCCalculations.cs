using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.CombustionEngine;

[TestFixture]
public class WHTCCalculations
{

	private readonly MissionType[] _missions = {
		MissionType.LongHaul,
		MissionType.RegionalDelivery,
		MissionType.UrbanDelivery,
		MissionType.MunicipalUtility,
		MissionType.Construction,
	};

    [TestCase]
	public void WHTCWeightingFactorsTest() // WHTCTest()
	{
		var whtc = DeclarationData.WHTCCorrection;

		var factors = new {
			urban = new[] { 0.0, 0.17, 0.69, 0.98, 0.62, 1.0, 1.0, 1.0, 0.45, 0.0 },
			rural = new[] { 0.0, 0.3, 0.27, 0.0, 0.32, 0.0, 0.0, 0.0, 0.36, 0.22 },
			motorway = new[] { 1.0, 0.53, 0.04, 0.02, 0.06, 0.0, 0.0, 0.0, 0.19, 0.78 }
		};

		var r = new Random();
		for (var i = 0; i < _missions.Length; i++) {
			var urban = r.NextDouble() * 2;
			var rural = r.NextDouble() * 2;
			var motorway = r.NextDouble() * 2;
			var whtcValue = whtc.Lookup(_missions[i], rural: rural, urban: urban, motorway: motorway);
			Assert.AreEqual(urban * factors.urban[i] + rural * factors.rural[i] + motorway * factors.motorway[i],
				whtcValue);
		}
	}

	[TestCase(MissionType.LongHaul, 1.0265, 1.0948, 1.0057, 1.0057, TestName= "WHTCLookupTestLongHaul"),
	TestCase(MissionType.RegionalDelivery, 1.0265, 1.0948, 1.0057, 1.02708700, TestName = "WHTCLookupTestRegionalDelivery")]
	public void WHTCLookupTestLongHaul(MissionType mission, double rural, double urban, double motorway, double expected)
	{
		var lookup = DeclarationData.WHTCCorrection.Lookup(mission, rural: rural, urban: urban,
			motorway: motorway);
		Assert.AreEqual(expected, lookup, 1e-8);
	}

}