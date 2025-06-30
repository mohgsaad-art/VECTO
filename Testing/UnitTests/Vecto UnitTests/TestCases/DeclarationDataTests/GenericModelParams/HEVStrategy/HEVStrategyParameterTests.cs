using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.HEVStrategy;

public class HEVStrategyParameterTests
{
    [TestCase]
    public void TestHevStrategyParametersInputLorry()
    {
        Assert.DoesNotThrow(() => new HEVStrategyParametersLorry());
    }

    [TestCase]
    public void TestHevStrategyParametersInputBus()
    {
        Assert.DoesNotThrow(() => new HEVStrategyParametersBus());
    }

    [Test, Combinatorial]
    public void LorryLookupCombinations(
        [Values(MissionType.LongHaul,
                MissionType.LongHaulEMS,
                MissionType.MunicipalUtility,
                MissionType.RegionalDelivery,
                MissionType.RegionalDeliveryEMS,
                MissionType.UrbanDelivery)]
            MissionType missionType,
        [Values(VehicleClass.Class2)]
                VehicleClass vehClass,
        [Values(LoadingType.LowLoading,
                LoadingType.ReferenceLoad)]
            LoadingType loadingType,
        [Values(10.0,
                20.0,
                40.0)]
            double socRange)
    {
        socRange /= 100;
        Assert.DoesNotThrow(() => DeclarationData.HEVStrategyParameters.LookupEquivalenceFactor(missionType, vehClass,
            loadingType, socRange));
    }



    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 2.00, 40)]
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 2.20, 40)]
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 1.9, 20)]
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 2.40, 20)]
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 0.10, 10)]
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 0.10, 10)]

    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 2.00, 90)]
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 2.20, 90)]
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 0.10, 5)]
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 0.10, 5)]

    //Interpolate if between two tables
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 1.95, 30)]
    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 1.0, 15)]

    //[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 0.10, 10)]
    //[TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.ReferenceLoad, 0.10, 10)]
	public void TestHevStrategyLookup(MissionType missionType, VehicleClass vehicleClass, LoadingType loadingType, double expected, double socRange)
    {
        socRange /= 100;
        LookupEquivAndAssert(missionType, vehicleClass, loadingType, expected, socRange);
    }

    [TestCase(MissionType.LongHaul, VehicleClass.Class2, LoadingType.LowLoading, 0.0956)]
    public void LookUpSlope(MissionType missionType, VehicleClass vehicleClass, LoadingType loadingType,
        double expected)
    {
        LookupSlopeAndAssert(missionType, vehicleClass, loadingType, expected);
    }

    private void LookupEquivAndAssert(MissionType mission, VehicleClass hdvClass, LoadingType loading, double expected, double socRange)
    {
        var feq = DeclarationData.HEVStrategyParameters.LookupEquivalenceFactor(mission, hdvClass,
            loading, socRange);
        if (!feq.IsEqual(expected)) {
            Assert.Fail($"Expected {expected} got {feq}");
        }

    }

    private void LookupSlopeAndAssert(MissionType mission, VehicleClass hdvClass, LoadingType loading, double expected)
    {
        var slope = DeclarationData.HEVStrategyParameters.LookupSlope(mission, hdvClass,
            loading);
        if (!slope.IsEqual(expected)) {
            Assert.Fail($"Expected {expected} got {slope}");
        }

    }
}