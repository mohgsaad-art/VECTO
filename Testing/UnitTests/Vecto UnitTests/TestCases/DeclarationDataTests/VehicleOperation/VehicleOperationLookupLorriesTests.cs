using System.Data;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.VehicleOperation;

public class VehicleOperationLookupLorriesTests
{
    [TestCase(VehicleClass.Class1s, MissionType.RegionalDelivery, 2)]
    [TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 2)]
    [TestCase(VehicleClass.Class16, MissionType.Construction, 2)]
    [TestCase(VehicleClass.Class53, MissionType.UrbanDelivery, 4)]
    public void VehicleOperationLookupChargingEventsLorry(VehicleClass hdvClass, MissionType mission, double expected)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        Assert.AreEqual(expected, val.StationaryChargingDuringMission_NbrEvents);
    }

    [TestCase(VehicleClass.Class1s, MissionType.RegionalDelivery, 0.5)]
    [TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 0.5)]
    [TestCase(VehicleClass.Class16, MissionType.Construction, 0.5)]
    [TestCase(VehicleClass.Class53, MissionType.UrbanDelivery, 0.5)]
    public void VehicleOperationLookupChargingDurationLorry(VehicleClass hdvClass, MissionType mission, double expected)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        Assert.AreEqual(expected * 3600, val.StationaryChargingDuringMission_AvgDurationPerEvent.Value()); //stored in seconds
    }

    [TestCase(VehicleClass.Class1s, MissionType.RegionalDelivery, 250)]
    [TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 250)]
    [TestCase(VehicleClass.Class16, MissionType.Construction, 100)]
    [TestCase(VehicleClass.Class53, MissionType.UrbanDelivery, 250)]
    public void VehicleOperationLookupMaxChargingPowerLorry(VehicleClass hdvClass, MissionType mission, double expected)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        Assert.AreEqual(expected * 1000, val.StationaryChargingMaxPwrInfrastructure.Value()); //stored in watt
    }

    [TestCase(VehicleClass.Class1s, MissionType.RegionalDelivery, 80000, 320)]
    [TestCase(VehicleClass.Class1, MissionType.RegionalDelivery, 80000, 320)]
    [TestCase(VehicleClass.Class16, MissionType.Construction, 60000, 240)]
    [TestCase(VehicleClass.Class53, MissionType.UrbanDelivery, 60000, 240)]
    public void VehicleOperationLookupMileageLorry(VehicleClass hdvClass, MissionType mission, double expectedAnnual, double expectedDaily)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        Assert.AreEqual(expectedAnnual * 1000, val.Mileage.AnnualMileage.Value()); //stored in meter
        Assert.AreEqual(expectedDaily * 1000, val.Mileage.DailyMileage.Value()); //stored in meter
    }

    [TestCaseSource(nameof(VehicleOperationTestSourceLorry))]
    public void VehicleOperationLookupMileage(VehicleClass hdvClass, MissionType mission)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        Assert.IsNotNull(val);
    }

	[TestCase(VehicleClass.Class53, MissionType.LongHaul)]
	[TestCase(VehicleClass.Class16, MissionType.UrbanDelivery)]
	[TestCase(VehicleClass.Class1s, MissionType.Coach)]
	public void VehicleOperationHeavyLorryFail(VehicleClass hdvClass, MissionType mission)
	{
		Assert.Throws<VectoException>(() => {
			DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
		});

	}

    public static IEnumerable<object[]> VehicleOperationTestSourceLorry()
	{
		var missions = EnumHelper.GetValues<MissionType>();

		var segmentTable = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".SegmentTable.csv"));
		var hdvMissionDict = new Dictionary<VehicleClass, HashSet<MissionType>>();

		foreach (DataRow row in segmentTable.Rows) {
			if (!row["valid"].ToString().ToBoolean()) {
				continue;
			}
			var hdvGroup = VehicleClassHelper.Parse(row["HDV group"].ToString());
			hdvMissionDict.TryAdd(hdvGroup, new HashSet<MissionType>());
			foreach (var missionType in missions.Where(
						m => m.IsDeclarationMission() && m != MissionType.ExemptedMission &&
							row.Field<string>(m.GetLabel()) != "-")) {
				hdvMissionDict[hdvGroup].Add(missionType);
			}
		}

		foreach (var hdvCl in
				hdvMissionDict
					.Where(kv => kv.Value.Count > 0)
					.OrderBy(kv => kv.Key)) {
			foreach (var mission in hdvCl.Value) {
				yield return new object[] { hdvCl.Key, mission };
			}
		}
	}
}