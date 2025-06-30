using System.Data;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.VehicleOperation;

public class VehicleOperationLookupBusTests
{
    [TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 10)]
    [TestCase(VehicleClass.ClassP31DD, MissionType.HeavyUrban, 10)]
    [TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 5)]
    [TestCase(VehicleClass.ClassP32DD, MissionType.Interurban, 5)]
    [TestCase(VehicleClass.Class31a, MissionType.Urban, 10)]
    [TestCase(VehicleClass.Class31e, MissionType.Suburban, 10)]
    [TestCase(VehicleClass.Class32a, MissionType.Coach, 2)]
    [TestCase(VehicleClass.Class32b, MissionType.Interurban, 5)]
    public void VehicleOperationLookupChargingEventsBus(VehicleClass hdvClass, MissionType mission, double expected)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        NUnit.Framework.Assert.AreEqual(expected, val.StationaryChargingDuringMission_NbrEvents);
    }

    [TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 0.17)]
    [TestCase(VehicleClass.ClassP31DD, MissionType.HeavyUrban, 0.17)]
    [TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 0.17)]
    [TestCase(VehicleClass.ClassP32DD, MissionType.Interurban, 0.17)]
    [TestCase(VehicleClass.Class31a, MissionType.Urban, 0.17)]
    [TestCase(VehicleClass.Class31e, MissionType.Suburban, 0.17)]
    [TestCase(VehicleClass.Class32a, MissionType.Coach, 0.75)]
    [TestCase(VehicleClass.Class32b, MissionType.Interurban, 0.17)]
    public void VehicleOperationLookupChargingDurationBus(VehicleClass hdvClass, MissionType mission, double expected)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        NUnit.Framework.Assert.AreEqual(expected * 3600, val.StationaryChargingDuringMission_AvgDurationPerEvent.Value()); //stored in seconds
    }

    [TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 450)]
    [TestCase(VehicleClass.ClassP31DD, MissionType.HeavyUrban, 450)]
    [TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 300)]
    [TestCase(VehicleClass.ClassP32DD, MissionType.Interurban, 300)]
    [TestCase(VehicleClass.Class31a, MissionType.Urban, 450)]
    [TestCase(VehicleClass.Class31e, MissionType.Suburban, 450)]
    [TestCase(VehicleClass.Class32a, MissionType.Coach, 300)]
    [TestCase(VehicleClass.Class32b, MissionType.Interurban, 300)]
    public void VehicleOperationLookupMaxChargingPowerBus(VehicleClass hdvClass, MissionType mission, double expected)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        NUnit.Framework.Assert.AreEqual(expected * 1000, val.StationaryChargingMaxPwrInfrastructure.Value()); //stored in watt
    }

    [TestCase(VehicleClass.ClassP31SD, MissionType.HeavyUrban, 60000, 240)]
    [TestCase(VehicleClass.ClassP31DD, MissionType.HeavyUrban, 60000, 240)]
    [TestCase(VehicleClass.ClassP32SD, MissionType.Interurban, 80000, 320)]
    [TestCase(VehicleClass.ClassP32DD, MissionType.Interurban, 80000, 320)]
    [TestCase(VehicleClass.Class31a, MissionType.Urban, 60000, 240)]
    [TestCase(VehicleClass.Class31e, MissionType.Suburban, 60000, 240)]
    [TestCase(VehicleClass.Class32a, MissionType.Coach, 100000, 400)]
    [TestCase(VehicleClass.Class32b, MissionType.Interurban, 80000, 320)]
    public void VehicleOperationLookupMileageBus(VehicleClass hdvClass, MissionType mission, double expectedAnnual, double expectedDaily)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        NUnit.Framework.Assert.AreEqual(expectedAnnual * 1000, val.Mileage.AnnualMileage.Value()); //stored in meter
        NUnit.Framework.Assert.AreEqual(expectedDaily * 1000, val.Mileage.DailyMileage.Value()); //stored in meter
    }

    [TestCaseSource(nameof(VehicleOperationTestSourcePrimaryBus))]
    [TestCaseSource(nameof(VehicleOperationTestSourceCompletedBus))]
    public void VehicleOperationLookupMileageBus(VehicleClass hdvClass, MissionType mission)
    {
        var val = DeclarationData.VehicleOperation.LookupVehicleOperation(hdvClass, mission);
        Assert.IsNotNull(val);
    }


    public static IEnumerable<object[]> VehicleOperationTestSourcePrimaryBus()
    {
        var missions = EnumHelper.GetValues<MissionType>();

        var segmentTable = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".PrimaryBusSegmentationTable.csv"));
        var hdvMissionDict = new Dictionary<VehicleClass, HashSet<MissionType>>();

        foreach (DataRow row in segmentTable.Rows) {
            var hdvGroup = VehicleClassHelper.Parse(row["HDV group"].ToString());
            hdvMissionDict.TryAdd(hdvGroup, new HashSet<MissionType>());
            foreach (var missionType in missions.Where(
                        m => m.IsDeclarationMission() && m != MissionType.ExemptedMission &&
                            segmentTable.Columns.Contains(m.GetLabel()) &&
                            !row.Field<string>(m.GetLabel()).IsNullOrWhiteSpace())) {
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

    public static IEnumerable<object[]> VehicleOperationTestSourceCompletedBus()
    {
        var missions = EnumHelper.GetValues<MissionType>();

        var segmentTable = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(DeclarationData.DeclarationDataResourcePrefix + ".CompletedBusSegmentationTable.csv"));
        var hdvMissionDict = new Dictionary<VehicleClass, HashSet<MissionType>>();

        foreach (DataRow row in segmentTable.Rows) {
            var hdvGroup = VehicleClassHelper.Parse(row["HDV group"].ToString());
            hdvMissionDict.TryAdd(hdvGroup, new HashSet<MissionType>());
            foreach (var missionType in missions.Where(
                        m => m.IsDeclarationMission() && m != MissionType.ExemptedMission &&
                            segmentTable.Columns.Contains(m.GetLabel()) &&
                            !row.Field<string>(m.GetLabel()).IsNullOrWhiteSpace())) {
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