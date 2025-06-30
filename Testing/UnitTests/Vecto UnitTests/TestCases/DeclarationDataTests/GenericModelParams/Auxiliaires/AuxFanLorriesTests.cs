using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Auxiliaires;

public class AuxFanLorriesTests
{
	private const double Tolerance = 0.0001;

	private readonly MissionType[] _missions = {
		MissionType.LongHaul,
		MissionType.RegionalDelivery,
		MissionType.UrbanDelivery,
		MissionType.MunicipalUtility,
		MissionType.Construction,
	};

    [TestCase(VehicleClass.Class1, "", new[] { 618, 671, 516, 566, 1037 }),
	TestCase(VehicleClass.Class1, "Crankshaft mounted - Electronically controlled visco clutch", new[] { 618, 671, 516, 566, 1037 }),
	TestCase(VehicleClass.Class1, "Crankshaft mounted - Bimetallic controlled visco clutch", new[] { 818, 871, 676, 766, 1277 }),
	TestCase(VehicleClass.Class1, "Crankshaft mounted - Discrete step clutch", new[] { 668, 721, 616, 616, 1157 }),
	TestCase(VehicleClass.Class1, "Crankshaft mounted - On/off clutch", new[] { 718, 771, 666, 666, 1237 }),
	TestCase(VehicleClass.Class1, "Belt driven or driven via transm. - Electronically controlled visco clutch",
	    new[] { 989, 1044, 833, 933, 1478 }),
	TestCase(VehicleClass.Class1, "Belt driven or driven via transm. - Bimetallic controlled visco clutch",
	    new[] { 1189, 1244, 993, 1133, 1718 }),
	TestCase(VehicleClass.Class1, "Belt driven or driven via transm. - Discrete step clutch", new[] { 1039, 1094, 983, 983, 1598 }),
	TestCase(VehicleClass.Class1, "Belt driven or driven via transm. - On/off clutch", new[] { 1089, 1144, 1033, 1033, 1678 }),
	TestCase(VehicleClass.Class1, "Hydraulic driven - Variable displacement pump", new[] { 938, 1155, 832, 917, 1872 }),
	TestCase(VehicleClass.Class1, "Hydraulic driven - Constant displacement pump", new[] { 1200, 1400, 1000, 1100, 2300 }),
	TestCase(VehicleClass.Class1, "Electrically driven - Electronically controlled", new[] { 700, 800, 600, 600, 1400 }),

	//medium lorries
	TestCase(VehicleClass.Class51, "", new[] { 0, 258, 198, 0, 0 }),
	TestCase(VehicleClass.Class51, "Crankshaft mounted - Electronically controlled visco clutch", new[] { 0, 258, 198, 0, 0 }),
	TestCase(VehicleClass.Class51, "Crankshaft mounted - Bimetallic controlled visco clutch", new[] { 0, 335, 260, 0, 0 }),
	TestCase(VehicleClass.Class51, "Crankshaft mounted - Discrete step clutch", new[] { 0, 277, 237, 0, 0 }),
	TestCase(VehicleClass.Class51, "Crankshaft mounted - On/off clutch", new[] { 0, 297, 256, 0, 0 }),
	TestCase(VehicleClass.Class51, "Belt driven or driven via transm. - Electronically controlled visco clutch",
	    new[] { 0, 402, 320, 0, 0 }),
	TestCase(VehicleClass.Class51, "Belt driven or driven via transm. - Bimetallic controlled visco clutch",
	    new[] { 0, 478, 382, 0, 0 }),
	TestCase(VehicleClass.Class51, "Belt driven or driven via transm. - Discrete step clutch", new[] { 0, 421, 378, 0, 0 }),
	TestCase(VehicleClass.Class51, "Belt driven or driven via transm. - On/off clutch", new[] { 0, 440, 397, 0, 0 }),
	TestCase(VehicleClass.Class51, "Hydraulic driven - Variable displacement pump", new[] { 0, 444, 320, 0, 0 }),
	TestCase(VehicleClass.Class51, "Hydraulic driven - Constant displacement pump", new[] { 0, 538, 385, 0, 0 }),
	TestCase(VehicleClass.Class51, "Electrically driven - Electronically controlled", new[] { 0, 308, 231, 0, 0 })
	]

    public void AuxFanTechTest(VehicleClass vehicleClass, string technology, int[] expected)
    {
        for (var i = 0; i < _missions.Length; i++) {
            if (expected[i] == 0)
                continue;
            var lookup = DeclarationData.Fan.LookupPowerDemand(vehicleClass, _missions[i], technology);
            NUnit.Framework.Assert.AreEqual(expected[i], lookup.Value(), Tolerance);
        }
    }

    [TestCase(VehicleClass.Class1, "Superfluid Hydraulic", MissionType.LongHaul, TestName = "AuxFanTechError( wrong tech )"),
    TestCase(VehicleClass.Class1, "Hydraulic driven - Electronically controlled", MissionType.Coach,
        TestName = "AuxFanTechError( wrong mission )")
    ]
    public void AuxFanTechError(VehicleClass vehicleClass, string technology, MissionType missionType)
    {
        AssertHelper.Exception<VectoException>(() => DeclarationData.Fan.LookupPowerDemand(vehicleClass, missionType, technology));
    }
}