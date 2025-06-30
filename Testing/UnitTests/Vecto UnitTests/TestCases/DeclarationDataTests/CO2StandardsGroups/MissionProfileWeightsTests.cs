using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.CO2StandardsGroups;

public class MissionProfileWeightsTests
{
    [

        TestCase(WeightingGroup.Group51, 0, 0, 0.25, 0.25, 0.25, 0.25),
        TestCase(WeightingGroup.Group52, 0, 0, 0.25, 0.25, 0.25, 0.25),
        TestCase(WeightingGroup.Group53, 0, 0, 0.25, 0.25, 0.25, 0.25),
        TestCase(WeightingGroup.Group54, 0, 0, 0.25, 0.25, 0.25, 0.25),
        TestCase(WeightingGroup.Group55, 0, 0, 0.25, 0.25, 0.25, 0.25),
        TestCase(WeightingGroup.Group56, 0, 0, 0.25, 0.25, 0.25, 0.25),

        TestCase(WeightingGroup.Group1, 0, 0, 0.1, 0.3, 0.18, 0.42, TestName = "TestMissionProfileWeights Grp 1"),
        TestCase(WeightingGroup.Group2, 0, 0, 0.125, 0.375, 0.15, 0.35, TestName = "TestMissionProfileWeights Grp 2"),
        TestCase(WeightingGroup.Group3, 0, 0, 0.125, 0.375, 0.15, 0.35, TestName = "TestMissionProfileWeights Grp 3"),


        TestCase(WeightingGroup.Group4UD, 0, 0, 0, 0, 0.5, 0.5, 0.25, 0.25, 0.25, 0.25, TestName = "TestMissionProfileWeights Grp 4UD"),
        TestCase(WeightingGroup.Group4RD, 0.05, 0.05, 0.45, 0.45, 0, 0, 0.25, 0.25, 0.25, 0.25, TestName = "TestMissionProfileWeights Grp 4RD"),
        TestCase(WeightingGroup.Group4LH, 0.45, 0.45, 0.05, 0.05, 0, 0, 0.25, 0.25, 0.25, 0.25, TestName = "TestMissionProfileWeights Grp 4LH"),

        TestCase(WeightingGroup.Group5RD, 0.03, 0.07, 0.27, 0.63, 0, 0, 0, 0, 0.5, 0.5, TestName = "TestMissionProfileWeights Grp 5RD"),
        TestCase(WeightingGroup.Group5LH, 0.27, 0.63, 0.03, 0.07, 0, 0, 0, 0, 0.5, 0.5, TestName = "TestMissionProfileWeights Grp 5LH"),

        TestCase(WeightingGroup.Group9RD, 0.03, 0.07, 0.27, 0.63, 0, 0, 0.25, 0.25, 0.25, 0.25, TestName = "TestMissionProfileWeights Grp 9RD"),
        TestCase(WeightingGroup.Group9LH, 0.27, 0.63, 0.03, 0.07, 0, 0, 0.25, 0.25, 0.25, 0.25, TestName = "TestMissionProfileWeights Grp 9LH"),

        TestCase(WeightingGroup.Group10RD, 0.03, 0.07, 0.27, 0.63, 0, 0, 0, 0, 0.5, 0.5, TestName = "TestMissionProfileWeights Grp 10RD"),
        TestCase(WeightingGroup.Group10LH, 0.27, 0.63, 0.03, 0.07, 0, 0, 0, 0, 0.5, 0.5, TestName = "TestMissionProfileWeights Grp 10LH"),

        TestCase(WeightingGroup.Group11, 0, 0, 0.3, 0.7, 0, 0, 0.1, 0.23, 0.3, 0.37, TestName = "TestMissionProfileWeights Grp 11"),
        TestCase(WeightingGroup.Group12, 0, 0, 0.3, 0.7, 0, 0, 0, 0, 0.3, 0.7, TestName = "TestMissionProfileWeights Grp 12"),
        TestCase(WeightingGroup.Group16, 0, 0, 0, 0, 0, 0, 0, 0, 0.3, 0.7, TestName = "TestMissionProfileWeights Grp 16"),
    ]
    public void TestMissionProfileWeights(WeightingGroup group, double eLhLow, double eLhRef, double eRdLow, double eRdRef, double eUdLow, double eUdRef, double eMuLow = 0, double eMuRef = 0, double eCoLow = 0, double eCoRef = 0, double elhEmsLow = 0, double eLhEmsRef = 0, double eRdEmsLow = 0, double eRdEmsRef = 0)
    {
        var factors = DeclarationData.WeightingFactors.Lookup(group);

		if (new[] { WeightingGroup.Group4LH, WeightingGroup.Group4RD, WeightingGroup.Group4UD, WeightingGroup.Group5LH, WeightingGroup.Group5RD, WeightingGroup.Group9LH, 
				WeightingGroup.Group9RD, WeightingGroup.Group11, WeightingGroup.Group12, WeightingGroup.Group10LH, WeightingGroup.Group10RD }.Contains(group)) {
			Assert.AreEqual(2, factors.Values.Sum(x => x), 1e-9);
		} else {
			Assert.AreEqual(1, factors.Values.Sum(x => x), 1e-9);
		}

		Assert.AreEqual(eLhLow, factors[Tuple.Create(MissionType.LongHaul, LoadingType.LowLoading)], 1e-9);
        Assert.AreEqual(eLhRef, factors[Tuple.Create(MissionType.LongHaul, LoadingType.ReferenceLoad)], 1e-9);
        Assert.AreEqual(eRdLow, factors[Tuple.Create(MissionType.RegionalDelivery, LoadingType.LowLoading)], 1e-9);
        Assert.AreEqual(eRdRef, factors[Tuple.Create(MissionType.RegionalDelivery, LoadingType.ReferenceLoad)], 1e-9);
        Assert.AreEqual(eUdLow, factors[Tuple.Create(MissionType.UrbanDelivery, LoadingType.LowLoading)], 1e-9);
        Assert.AreEqual(eUdRef, factors[Tuple.Create(MissionType.UrbanDelivery, LoadingType.ReferenceLoad)], 1e-9);

		Assert.AreEqual(eMuLow, factors[Tuple.Create(MissionType.MunicipalUtility, LoadingType.LowLoading)], 1e-9);
		Assert.AreEqual(eMuRef, factors[Tuple.Create(MissionType.MunicipalUtility, LoadingType.ReferenceLoad)], 1e-9);

        Assert.AreEqual(eCoLow, factors[Tuple.Create(MissionType.Construction, LoadingType.LowLoading)], 1e-9);
        Assert.AreEqual(eCoRef, factors[Tuple.Create(MissionType.Construction, LoadingType.ReferenceLoad)], 1e-9);

        //Assert.AreEqual(0, factors[Tuple.Create(MissionType.MunicipalUtility, LoadingType.ReferenceLoad)], 1e-9);
        //Assert.AreEqual(0, factors[Tuple.Create(MissionType.LongHaulEMS, LoadingType.LowLoading)], 1e-9);

        //Assert.AreEqual(0, factors[Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.LowLoading)], 1e-9);
        //Assert.AreEqual(0, factors[Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad)], 1e-9);
        //Assert.AreEqual(0, factors[Tuple.Create(MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad)], 1e-9);
    }
}