using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Driver;

public class CompletedBusDriverDataTests
{

	private const CompressorDrive psMech = CompressorDrive.mechanically;
	private const CompressorDrive psEl = CompressorDrive.electrically;

    [TestCase(VehicleClass.Class31a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
   TestCase(VehicleClass.Class31a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
   TestCase(VehicleClass.Class31a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
   TestCase(VehicleClass.Class31a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
   TestCase(VehicleClass.Class31a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
   TestCase(VehicleClass.Class31a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
   TestCase(VehicleClass.Class31a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
   TestCase(VehicleClass.Class31a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),

   TestCase(VehicleClass.Class33a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
   TestCase(VehicleClass.Class33a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
   TestCase(VehicleClass.Class33a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
   TestCase(VehicleClass.Class33a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
   TestCase(VehicleClass.Class33a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
   TestCase(VehicleClass.Class33a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
   TestCase(VehicleClass.Class33a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
   TestCase(VehicleClass.Class33a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),

   TestCase(VehicleClass.Class37a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
   TestCase(VehicleClass.Class37a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
   TestCase(VehicleClass.Class37a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
   TestCase(VehicleClass.Class37a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
   TestCase(VehicleClass.Class37a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
   TestCase(VehicleClass.Class37a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
   TestCase(VehicleClass.Class37a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
   TestCase(VehicleClass.Class37a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),
   ]
    public void TestCompletedGenericBusDeclarationDriverData(VehicleClass group, VectoSimulationJobType jobType,
   ArchitectureID arch, CompressorDrive compressorDrive, double expectedESSUF)
    {
        var da = new CompletedBusGenericDriverDataAdapter();

        var segment = GetBusSegment(group);
        Assert.AreEqual(group, segment.VehicleClass);

        var driverData = da.CreateBusDriverData(segment, jobType, arch, compressorDrive);

        Assert.IsNotNull(driverData);

        // overspeed is disabled for completed simulations
        Assert.IsFalse(driverData.OverSpeed.Enabled);
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

        // lookahead coasting is disabled for completed simulations
        Assert.IsFalse(driverData.LookAheadCoasting.Enabled);
        Assert.AreEqual(50.KMPHtoMeterPerSecond(), driverData.LookAheadCoasting.MinSpeed);
        //Assert.AreEqual(, driverData.LookAheadCoasting.LookAheadDecisionFactor);
        Assert.AreEqual(10, driverData.LookAheadCoasting.LookAheadDistanceFactor);
    }

    [TestCase(VehicleClass.Class31a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
    TestCase(VehicleClass.Class31a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
    TestCase(VehicleClass.Class31a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
    TestCase(VehicleClass.Class31a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
    TestCase(VehicleClass.Class31a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
    TestCase(VehicleClass.Class31a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
    TestCase(VehicleClass.Class31a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
    TestCase(VehicleClass.Class31a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),

    TestCase(VehicleClass.Class33a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
    TestCase(VehicleClass.Class33a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
    TestCase(VehicleClass.Class33a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
    TestCase(VehicleClass.Class33a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
    TestCase(VehicleClass.Class33a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
    TestCase(VehicleClass.Class33a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
    TestCase(VehicleClass.Class33a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
    TestCase(VehicleClass.Class33a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),

    TestCase(VehicleClass.Class37a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psMech, 0.35),
    TestCase(VehicleClass.Class37a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psMech, 0.55),
    TestCase(VehicleClass.Class37a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psMech, 0.55),
    TestCase(VehicleClass.Class37a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psMech, 0.55),
    TestCase(VehicleClass.Class37a, VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, psEl, 0.35),
    TestCase(VehicleClass.Class37a, VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, psEl, 0.65),
    TestCase(VehicleClass.Class37a, VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, psEl, 0.65),
    TestCase(VehicleClass.Class37a, VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, psEl, 0.65),
    ]
    public void TestCompletedSpecificBusDeclarationDriverData(VehicleClass group, VectoSimulationJobType jobType,
        ArchitectureID arch, CompressorDrive compressorDrive, double expectedESSUF)
    {
        var da = new CompletedBusGenericDriverDataAdapter();

        var segment = GetBusSegment(group);
        Assert.AreEqual(group, segment.VehicleClass);

        var driverData = da.CreateBusDriverData(segment, jobType, arch, compressorDrive);

        Assert.IsNotNull(driverData);

        // overspeed is disabled for completed simulations
        Assert.IsFalse(driverData.OverSpeed.Enabled);
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

        // lookahead coasting is disabled for completed simulations
        Assert.IsFalse(driverData.LookAheadCoasting.Enabled);
        Assert.AreEqual(50.KMPHtoMeterPerSecond(), driverData.LookAheadCoasting.MinSpeed);
        //Assert.AreEqual(, driverData.LookAheadCoasting.LookAheadDecisionFactor);
        Assert.AreEqual(10, driverData.LookAheadCoasting.LookAheadDistanceFactor);
    }

    private Segment GetBusSegment(VehicleClass group)
    {
        switch (group) {
			case VehicleClass.Class31a:
                return DeclarationData.CompletedBusSegments.Lookup(AxleConfiguration.AxleConfig_4x2.NumAxles(), VehicleCode.CE, RegistrationClass.I, 0, 0.SI<Meter>(), false);
            //case VehicleClass.Class31b1:
            //	break;
            //case VehicleClass.Class31b2:
            //	break;
            //case VehicleClass.Class31c:
            //	break;
            //case VehicleClass.Class31d:
            //	break;
            //case VehicleClass.Class31e:
            //	break;
            //        case VehicleClass.Class32a:
            //break;
            //case VehicleClass.Class32b:
            //	break;
            //case VehicleClass.Class32c:
            //	break;
            //case VehicleClass.Class32d:
            //	break;
            //case VehicleClass.Class32e:
            //	break;
            //case VehicleClass.Class32f:
            //	break;
            case VehicleClass.Class33a:
                return DeclarationData.CompletedBusSegments.Lookup(AxleConfiguration.AxleConfig_6x2.NumAxles(), VehicleCode.CE, RegistrationClass.I, 0, 0.SI<Meter>(), false);
            //case VehicleClass.Class33b1:
            //	break;
            //case VehicleClass.Class33b2:
            //	break;
            //case VehicleClass.Class33c:
            //	break;
            //case VehicleClass.Class33d:
            //	break;
            //case VehicleClass.Class33e:
            //	break;
            //case VehicleClass.Class34a:
            //	break;
            //case VehicleClass.Class34b:
            //	break;
            //case VehicleClass.Class34c:
            //	break;
            //case VehicleClass.Class34d:
            //	break;
            //case VehicleClass.Class34e:
            //	break;
            //case VehicleClass.Class34f:
            //	break;
            //case VehicleClass.Class35a:
            //	break;
            //case VehicleClass.Class35b1:
            //	break;
            //case VehicleClass.Class35b2:
            //	break;
            //case VehicleClass.Class35c:
            //	break;
            //case VehicleClass.Class36a:
            //	break;
            //case VehicleClass.Class36b:
            //	break;
            //case VehicleClass.Class36c:
            //	break;
            //case VehicleClass.Class36d:
            //	break;
            //case VehicleClass.Class36e:
            //	break;
            //case VehicleClass.Class36f:
            //	break;
            case VehicleClass.Class37a:
                return DeclarationData.CompletedBusSegments.Lookup(AxleConfiguration.AxleConfig_8x2.NumAxles(), VehicleCode.CE, RegistrationClass.I, 0, 0.SI<Meter>(), false);
            //case VehicleClass.Class37b1:
            //	break;
            //case VehicleClass.Class37b2:
            //	break;
            //case VehicleClass.Class37c:
            //	break;
            //case VehicleClass.Class37d:
            //	break;
            //case VehicleClass.Class37e:
            //	break;
            //case VehicleClass.Class38a:
            //	break;
            //case VehicleClass.Class38b:
            //	break;
            //case VehicleClass.Class38c:
            //	break;
            //case VehicleClass.Class38d:
            //	break;
            //case VehicleClass.Class38e:
            //	break;
            //case VehicleClass.Class38f:
            //	break;
            //case VehicleClass.Class39a:
            //	break;
            //case VehicleClass.Class39b1:
            //	break;
            //case VehicleClass.Class39b2:
            //	break;
            //case VehicleClass.Class39c:
            //	break;
            //case VehicleClass.Class40a:
            //	break;
            //case VehicleClass.Class40b:
            //	break;
            //case VehicleClass.Class40c:
            //	break;
            //case VehicleClass.Class40d:
            //	break;
            //case VehicleClass.Class40e:
            //	break;
            //case VehicleClass.Class40f:
            //	break;
            default:
                throw new ArgumentOutOfRangeException(nameof(group), group, null);
        }
    }

}