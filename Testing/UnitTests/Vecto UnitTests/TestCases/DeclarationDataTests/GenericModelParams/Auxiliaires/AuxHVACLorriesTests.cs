using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Auxiliaires;

[TestFixture]
public class AuxHVACLorriesTests
{

	private readonly MissionType[] _missions = {
		MissionType.LongHaul,
		MissionType.RegionalDelivery,
		MissionType.UrbanDelivery,
		MissionType.MunicipalUtility,
		MissionType.Construction,
	};

    [TestCase(VehicleClass.Class51, new[] { 200, 150, 150, -1, -1 }),
        TestCase(VehicleClass.Class52, new[] { 200, 150, 150, -1, -1 }),
        TestCase(VehicleClass.Class53, new[] { 200, 150, 150, -1, -1 }),
        TestCase(VehicleClass.Class54, new[] { 200, 150, 150, -1, -1 }),
        TestCase(VehicleClass.Class55, new[] { 200, 150, 150, -1, -1 }),
        TestCase(VehicleClass.Class56, new[] { 200, 150, 150, -1, -1 }),
        TestCase(VehicleClass.Class1s, new[] { 200, 150, 150, -1, -1 }),
        TestCase(VehicleClass.Class1, new[] { 200, 150, 150, -1, -1 }),
        TestCase(VehicleClass.Class2, new[] { 200, 200, 150, -1, -1 }),
        TestCase(VehicleClass.Class3, new[] { 200, 200, 150, -1, -1 }),
        TestCase(VehicleClass.Class4, new[] { 350, 200, 150, 300, 200 }),
        TestCase(VehicleClass.Class5, new[] { 350, 200, 150, 0, 200 }),
        TestCase(VehicleClass.Class9, new[] { 350, 200, 150, 300, 200 }),
        TestCase(VehicleClass.Class10, new[] { 350, 200, 150, 0, 200 }),
        TestCase(VehicleClass.Class11, new[] { 350, 200, 150, 300, 200 }),
        TestCase(VehicleClass.Class12, new[] { 350, 200, 150, -1, 200 }),
        TestCase(VehicleClass.Class16, new[] { 350, 200, 150, -1, 200 })]
    public void AuxHeatingVentilationAirConditionTest_Default(VehicleClass vehicleClass, int[] expected)
    {
        for (var i = 0; i < expected.Length; i++) {
            if (expected[i] >= 0) {
                AssertHelper.AreRelativeEqual(expected[i],
                    DeclarationData.HeatingVentilationAirConditioning.Lookup(_missions[i], "Default", vehicleClass)
                        .PowerDemand.Value());
            } else {
                var i1 = i;
                AssertHelper.Exception<VectoException>(
                    () => DeclarationData.HeatingVentilationAirConditioning.Lookup(_missions[i1], "Default", vehicleClass));
            }
        }
    }

    [
    TestCase(VehicleClass.Class51, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class52, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class53, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class54, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class55, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class56, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class1s, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class1, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class2, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class3, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class4, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class5, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class9, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class10, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class11, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class12, new[] { 0, 0, 0, 0, 0 }),
    TestCase(VehicleClass.Class16, new[] { 0, 0, 0, 0, 0 })]
    public void AuxHeatingVentilationAirConditionTest_None(VehicleClass vehicleClass, int[] expected)
    {
        for (var i = 0; i < expected.Length; i++) {
            AssertHelper.AreRelativeEqual(expected[i],
                DeclarationData.HeatingVentilationAirConditioning.Lookup(_missions[i], "None", vehicleClass).PowerDemand.Value());
        }
    }

    [TestCase()]
    public void AuxHeatingVentilationAirConditionTechnologyTest()
    {
        var tech = DeclarationData.HeatingVentilationAirConditioning.GetTechnologies();
        Assert.AreEqual(2, tech.Length);
        Assert.IsTrue(tech.Contains("Default"));
        Assert.IsTrue(tech.Contains("None"));
    }
}