using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Driver;

public class LorryDriverDataTests
{
    [TestCase(VehicleClass.Class1s),
    TestCase(VehicleClass.Class1),
    TestCase(VehicleClass.Class2),
    TestCase(VehicleClass.Class3),
    TestCase(VehicleClass.Class4),
    TestCase(VehicleClass.Class5),
    TestCase(VehicleClass.Class9),
    TestCase(VehicleClass.Class10),
    TestCase(VehicleClass.Class11),
    TestCase(VehicleClass.Class12),
    TestCase(VehicleClass.Class16),
    TestCase(VehicleClass.Class53),
    TestCase(VehicleClass.Class54),
    ]
    public void TestLorryDeclarationDriverData(VehicleClass group)
    {
        var da = new LorryDriverDataAdapter();

        var segment = GetLorrySegment(group);
        Assert.AreEqual(group, segment.VehicleClass);

        var driverData = da.CreateDriverData(segment);

        Assert.IsNotNull(driverData);

        Assert.IsTrue(driverData.OverSpeed.Enabled);
        Assert.AreEqual(50.KMPHtoMeterPerSecond(), driverData.OverSpeed.MinSpeed);
        Assert.AreEqual(2.5.KMPHtoMeterPerSecond(), driverData.OverSpeed.OverSpeed);

        Assert.AreEqual(2.SI<Second>(), driverData.EngineStopStart.EngineOffStandStillActivationDelay);
        Assert.AreEqual(120.SI<Second>(), driverData.EngineStopStart.MaxEngineOffTimespan);
        Assert.AreEqual(0.8, driverData.EngineStopStart.UtilityFactorDriving);
        Assert.AreEqual(0.8, driverData.EngineStopStart.UtilityFactorDriving);

        Assert.AreEqual(60.KMPHtoMeterPerSecond(), driverData.EcoRoll.MinSpeed);
        Assert.AreEqual(0.SI<MeterPerSquareSecond>(), driverData.EcoRoll.AccelerationLowerLimit);
        Assert.AreEqual(0.1.SI<MeterPerSquareSecond>(), driverData.EcoRoll.AccelerationUpperLimit);
        Assert.AreEqual(2.SI<Second>(), driverData.EcoRoll.ActivationPhaseDuration);
        Assert.AreEqual(0.KMPHtoMeterPerSecond(), driverData.EcoRoll.UnderspeedThreshold);

        Assert.AreEqual(50.KMPHtoMeterPerSecond(), driverData.PCC.MinSpeed);
        Assert.AreEqual(5.KMPHtoMeterPerSecond(), driverData.PCC.OverspeedUseCase3);
        Assert.AreEqual(80.KMPHtoMeterPerSecond(), driverData.PCC.PCCEnableSpeed);
        Assert.AreEqual(1500.SI<Meter>(), driverData.PCC.PreviewDistanceUseCase1);
        Assert.AreEqual(1000.SI<Meter>(), driverData.PCC.PreviewDistanceUseCase2);
        Assert.AreEqual(8.KMPHtoMeterPerSecond(), driverData.PCC.UnderSpeed);

        Assert.IsTrue(driverData.LookAheadCoasting.Enabled);
        Assert.AreEqual(50.KMPHtoMeterPerSecond(), driverData.LookAheadCoasting.MinSpeed);
        //Assert.AreEqual(, driverData.LookAheadCoasting.LookAheadDecisionFactor);
        Assert.AreEqual(10, driverData.LookAheadCoasting.LookAheadDistanceFactor);
    }


    private Segment GetLorrySegment(VehicleClass group)
    {
        switch (group)
        {
            case VehicleClass.Class1s:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class1:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500.01.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class2:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10001.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class3:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12001.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class4:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 99000.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class5:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 99000.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            //case VehicleClass.Class6:
            //	return DeclarationData.TruckSegments.Lookup();
            //case VehicleClass.Class7:
            //	return DeclarationData.TruckSegments.Lookup();
            //case VehicleClass.Class8:
            //	return DeclarationData.TruckSegments.Lookup();
            case VehicleClass.Class9:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class10:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 16000.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class11:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 7500.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class12:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 7500.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            //case VehicleClass.Class13:
            //	return DeclarationData.TruckSegments.Lookup();
            //case VehicleClass.Class14:
            //	return DeclarationData.TruckSegments.Lookup();
            //case VehicleClass.Class15:
            //	return DeclarationData.TruckSegments.Lookup();
            case VehicleClass.Class16:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 7500.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class53:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 5000.01.SI<Kilogram>(), 0.SI<Kilogram>(), false);
            case VehicleClass.Class54:
                return DeclarationData.TruckSegments.Lookup(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 5000.01.SI<Kilogram>(), 0.SI<Kilogram>(), false);

            default:
                throw new ArgumentOutOfRangeException(nameof(group), group, null);
        }
    }

}