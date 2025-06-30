using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Driver;

public class PrimaryBusDriverDataTests
{
    private const VehicleClass P31_32 = VehicleClass.ClassP31_32;
    private const VehicleClass P33_34 = VehicleClass.ClassP33_34;
    private const VehicleClass P35_36 = VehicleClass.ClassP35_36;
    private const VehicleClass P37_38 = VehicleClass.ClassP37_38;
    private const VehicleClass P39_40 = VehicleClass.ClassP39_40;

    private const CompressorDrive psMech = CompressorDrive.mechanically;
    private const CompressorDrive psEl = CompressorDrive.electrically;

    [
    TestCase(P31_32, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
    TestCase(P31_32, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
    TestCase(P31_32, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
    TestCase(P31_32, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
    TestCase(P31_32, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
    TestCase(P31_32, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
    TestCase(P31_32, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
    TestCase(P31_32, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),

    TestCase(P33_34, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
    TestCase(P33_34, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
    TestCase(P33_34, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
    TestCase(P33_34, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
    TestCase(P33_34, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
    TestCase(P33_34, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
    TestCase(P33_34, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
    TestCase(P33_34, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),

    TestCase(P35_36, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
    TestCase(P35_36, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
    TestCase(P35_36, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
    TestCase(P35_36, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
    TestCase(P35_36, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
    TestCase(P35_36, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
    TestCase(P35_36, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
    TestCase(P35_36, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),

    TestCase(P37_38, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
    TestCase(P37_38, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
    TestCase(P37_38, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
    TestCase(P37_38, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
    TestCase(P37_38, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
    TestCase(P37_38, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
    TestCase(P37_38, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
    TestCase(P37_38, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),

    TestCase(P39_40, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
    TestCase(P39_40, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
    TestCase(P39_40, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
    TestCase(P39_40, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
    TestCase(P39_40, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
    TestCase(P39_40, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
    TestCase(P39_40, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
    TestCase(P39_40, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),
    ]
    public void TestPrimaryBusDeclarationDriverData(VehicleClass group, VectoSimulationJobType jobType,
        ArchitectureID arch, CompressorDrive compressorDrive, double expectedESSUF)
    {
        var da = new PrimaryBusDriverDataAdapter();

        var segment = GetBusSegment(group);
        Assert.AreEqual(group, segment.VehicleClass);

        var driverData = da.CreateBusDriverData(segment, jobType, arch, compressorDrive);

        Assert.IsNotNull(driverData);

        Assert.IsTrue(driverData.OverSpeed.Enabled);
        Assert.AreEqual(50.KMPHtoMeterPerSecond(), driverData.OverSpeed.MinSpeed);
        Assert.AreEqual(2.5.KMPHtoMeterPerSecond(), driverData.OverSpeed.OverSpeed);

        Assert.AreEqual(2.SI<Second>(), driverData.EngineStopStart.EngineOffStandStillActivationDelay);
        Assert.AreEqual(120.SI<Second>(), driverData.EngineStopStart.MaxEngineOffTimespan);
        Assert.AreEqual(expectedESSUF, driverData.EngineStopStart.UtilityFactorDriving);
        Assert.AreEqual(expectedESSUF, driverData.EngineStopStart.UtilityFactorDriving);

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

    private Segment GetBusSegment(VehicleClass group)
    {
        switch (group)
        {
            case VehicleClass.ClassP31_32:
                return DeclarationData.PrimaryBusSegments.Lookup(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, false);
            case VehicleClass.ClassP33_34:
                return DeclarationData.PrimaryBusSegments.Lookup(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, false);
            case VehicleClass.ClassP35_36:
                return DeclarationData.PrimaryBusSegments.Lookup(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, true);
            case VehicleClass.ClassP37_38:
                return DeclarationData.PrimaryBusSegments.Lookup(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, false);
            case VehicleClass.ClassP39_40:
                return DeclarationData.PrimaryBusSegments.Lookup(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, true);

            default:
                throw new ArgumentOutOfRangeException(nameof(group), group, null);
        }
    }

}