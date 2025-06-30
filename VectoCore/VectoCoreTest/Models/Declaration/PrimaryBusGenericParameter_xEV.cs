using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration;

[TestFixture]
public class PrimaryBusGenericParameter_xEV
{
	private const FloorType LF = FloorType.LowFloor;
	private const FloorType HF = FloorType.HighFloor;

	[TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, LF, false, false,
			VehicleClass.ClassP31_32, 875),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, LF, true, false,
			VehicleClass.ClassP31_32, 875),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, HF, false, true,
			VehicleClass.ClassP31_32, 1237),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, HF, true, true,
			VehicleClass.ClassP31_32, 1237),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, LF, false, false,
			VehicleClass.ClassP33_34, 875),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, LF, true, false,
			VehicleClass.ClassP33_34, 875),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, HF, false, false,
			VehicleClass.ClassP33_34, 1237),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, HF, true, false,
			VehicleClass.ClassP33_34, 1237),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, LF, false, true,
			VehicleClass.ClassP35_36, 875),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, LF, true, true,
			VehicleClass.ClassP35_36, 875),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, HF, false, true,
			VehicleClass.ClassP35_36, 1237),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, HF, true, true,
			VehicleClass.ClassP35_36, 1237),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, LF, false, false,
			VehicleClass.ClassP37_38, 1195),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, LF, true, false,
			VehicleClass.ClassP37_38, 1195),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, HF, false, false,
			VehicleClass.ClassP37_38, 1489),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, HF, true, false,
			VehicleClass.ClassP37_38, 1489),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, LF, false, true,
			VehicleClass.ClassP39_40, 1195),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, LF, true, true,
			VehicleClass.ClassP39_40, 1195),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, HF, false, true,
			VehicleClass.ClassP39_40, 1489),
        TestCase(
            VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, HF, true, true,
			VehicleClass.ClassP39_40, 1489),
        Category(Definitions.TESTCASE_MIGRATED)
	]
	public void TestVehicleMassCalculation(VehicleCategory category, AxleConfiguration axleConfiguration, FloorType floorType, bool doubleDecker,
		bool articulated, VehicleClass expectedClass, double expectedICEMass)
	{
		var segment = DeclarationData.PrimaryBusSegments.Lookup(
			category, axleConfiguration, articulated);

		Assert.AreEqual(11, segment.Missions.Length);
		Assert.AreEqual(expectedClass, segment.VehicleClass);

        var mission = segment.Missions.First(x =>
			x.BusParameter.FloorType == floorType &&
			x.BusParameter.DoubleDecker == doubleDecker);

		Assert.AreEqual(expectedICEMass, mission.GenericMassICE.Value(), 1e-3);
    }
}