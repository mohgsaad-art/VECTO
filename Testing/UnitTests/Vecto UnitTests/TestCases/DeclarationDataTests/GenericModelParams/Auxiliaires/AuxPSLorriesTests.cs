using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Auxiliaires;

[TestFixture]
public class AuxPSLorriesTests
{

	private readonly MissionType[] _missions = {
		MissionType.LongHaul,
		MissionType.RegionalDelivery,
		MissionType.UrbanDelivery,
		MissionType.MunicipalUtility,
		MissionType.Construction,
	};

    [Test,
       TestCase("Small", new[] { 1400, 1300, 1200, 1200, 1300 }),
       TestCase("Small + ESS", new[] { 900, 800, 800, 800, 800 }),
       TestCase("Small + visco clutch", new[] { 800, 700, 700, 700, 700 }),
       TestCase("Small + mech. clutch", new[] { 600, 600, 650, 650, 600 }),
       TestCase("Small + ESS + AMS", new[] { 500, 400, 500, 500, 400 }),
       TestCase("Small + visco clutch + AMS", new[] { 400, 300, 400, 400, 300 }),
       TestCase("Small + mech. clutch + AMS", new[] { 200, 200, 350, 350, 200 }),
       TestCase("Medium Supply 1-stage", new[] { 1600, 1400, 1350, 1350, 1500 }),
       TestCase("Medium Supply 1-stage + ESS", new[] { 1000, 900, 900, 900, 900 }),
       TestCase("Medium Supply 1-stage + visco clutch", new[] { 850, 800, 800, 800, 750 }),
       TestCase("Medium Supply 1-stage + mech. clutch", new[] { 600, 550, 550, 550, 600 }),
       TestCase("Medium Supply 1-stage + ESS + AMS", new[] { 600, 700, 700, 700, 500 }),
       TestCase("Medium Supply 1-stage + visco clutch + AMS", new[] { 450, 600, 600, 600, 350 }),
       TestCase("Medium Supply 1-stage + mech. clutch + AMS", new[] { 200, 350, 350, 350, 200 }),
       TestCase("Medium Supply 2-stage", new[] { 2100, 1750, 1700, 1700, 2100 }),
       TestCase("Medium Supply 2-stage + ESS", new[] { 1100, 1050, 1000, 1000, 1000 }),
       TestCase("Medium Supply 2-stage + visco clutch", new[] { 1000, 850, 800, 800, 900 }),
       TestCase("Medium Supply 2-stage + mech. clutch", new[] { 700, 650, 600, 600, 800 }),
       TestCase("Medium Supply 2-stage + ESS + AMS", new[] { 700, 850, 800, 800, 500 }),
       TestCase("Medium Supply 2-stage + visco clutch + AMS", new[] { 600, 650, 600, 600, 400 }),
       TestCase("Medium Supply 2-stage + mech. clutch + AMS", new[] { 300, 450, 400, 400, 300 }),
       TestCase("Large Supply", new[] { 4300, 3600, 3500, 3500, 4100 }),
       TestCase("Large Supply + ESS", new[] { 1600, 1300, 1200, 1200, 1500 }),
       TestCase("Large Supply + visco clutch", new[] { 1300, 1100, 1000, 1000, 1200 }),
       TestCase("Large Supply + mech. clutch", new[] { 800, 800, 700, 700, 900 }),
       TestCase("Large Supply + ESS + AMS", new[] { 1100, 1000, 1000, 1000, 1000 }),
       TestCase("Large Supply + visco clutch + AMS", new[] { 800, 800, 800, 800, 700 }),
       TestCase("Large Supply + mech. clutch + AMS", new[] { 300, 500, 500, 500, 400 }),
       TestCase("Vacuum pump", new[] { 190, 160, 130, 130, 130 }),
       ]
    public void AuxPneumaticSystemTest(string technology, int[] expected)
    {
        for (var i = 0; i < _missions.Length; i++) {
            var lookup = DeclarationData.PneumaticSystem.Lookup(_missions[i], technology);
            AssertHelper.AreRelativeEqual(expected[i], lookup.PowerDemand.Value());
        }
    }

}