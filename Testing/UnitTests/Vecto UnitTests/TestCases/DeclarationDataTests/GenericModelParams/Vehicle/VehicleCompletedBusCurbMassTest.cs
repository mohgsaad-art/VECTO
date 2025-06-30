using System.Data;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.TestCases.RunDataFactory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Vehicle;

public class VehicleCompletedBusCurbMassTest
{

	private const AxleConfiguration Axl2 = AxleConfiguration.AxleConfig_4x2;
	private const AxleConfiguration Axl3 = AxleConfiguration.AxleConfig_6x2;

	private const VehicleCode VC_CA = VehicleCode.CA;
	private const VehicleCode VC_CB = VehicleCode.CB;
	private const VehicleCode VC_CE = VehicleCode.CE;
	private const VehicleCode VC_CF = VehicleCode.CF;

	private const RegistrationClass RC_II = RegistrationClass.II;

	protected IKernel _kernel;
	private IVectoRunDataFactoryFactory _runDataFactoryFactory;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_runDataFactoryFactory = _kernel.Get<IVectoRunDataFactoryFactory>();
	}

    [
            // testcases for vehicle mass with a high tpmlm
            TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl2, false, 25000, 0, VC_CE, RC_II, false,
                VehicleClass.ClassP31SD, 11975),
            TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl2, false, 25000, 0, VC_CE, RC_II, false,
                VehicleClass.ClassP31SD, 12291),
            TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl2, false, 25000, 0, VC_CE, RC_II, false,
                VehicleClass.ClassP31SD, 12291),
            TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl2, false, 25000, 0, VC_CE, RC_II, false,
                VehicleClass.ClassP31SD, 11416),

            TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl2, false, 25000, 4, VC_CF, RC_II, false,
                VehicleClass.ClassP31DD, 12350),
            TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl2, false, 25000, 4, VC_CF, RC_II, false,
                VehicleClass.ClassP31DD, 12666),
            TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl2, false, 25000, 4, VC_CF, RC_II, false,
                VehicleClass.ClassP31DD, 12666),
            TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl2, false, 25000, 4, VC_CF, RC_II, false,
                VehicleClass.ClassP31DD, 11791),

            TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl2, false, 25000, 7, VC_CA, RC_II, false,
                VehicleClass.ClassP32SD, 13150),
            TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl2, false, 25000, 7, VC_CA, RC_II, false,
                VehicleClass.ClassP32SD, 13466),
            TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl2, false, 25000, 7, VC_CA, RC_II, false,
                VehicleClass.ClassP32SD, 13466),
            TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl2, false, 25000, 7, VC_CA, RC_II, false,
                VehicleClass.ClassP32SD, 12229),

            TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl2, false, 25000, 9, VC_CB, RC_II, false,
                VehicleClass.ClassP32DD, 13400),
            TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl2, false, 25000, 9, VC_CB, RC_II, false,
                VehicleClass.ClassP32DD, 13716),
            TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl2, false, 25000, 9, VC_CB, RC_II, false,
                VehicleClass.ClassP32DD, 13716),
            TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl2, false, 25000, 9, VC_CB, RC_II, false,
                VehicleClass.ClassP32DD, 12479),

            TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl3, false, 25000, 0, VC_CE, RC_II, false,
                VehicleClass.ClassP33SD, 14175),
            TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl3, false, 25000, 0, VC_CE, RC_II, false,
                VehicleClass.ClassP33SD, 14491),
            TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl3, false, 25000, 0, VC_CE, RC_II, false,
                VehicleClass.ClassP33SD, 14491),
            TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl3, false, 25000, 0, VC_CE, RC_II, false,
                VehicleClass.ClassP33SD, 13616),

            TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl3, false, 25000, 4, VC_CF, RC_II, false,
                VehicleClass.ClassP33DD, 14725),
            TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl3, false, 25000, 4, VC_CF, RC_II, false,
                VehicleClass.ClassP33DD, 15041),
            TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl3, false, 25000, 4, VC_CF, RC_II, false,
                VehicleClass.ClassP33DD, 15041),
            TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl3, false, 25000, 4, VC_CF, RC_II, false,
                VehicleClass.ClassP33DD, 14166),
        ]
    public void CompletedGenericBusCurbMassTest_HighTPMLM(VectoSimulationJobType jobType, ArchitectureID archId,
            AxleConfiguration axleConfiguration, bool articulated, double TPMLM, int runIdx,
            VehicleCode vehicleCode, RegistrationClass registrationClass, bool lowEntry, VehicleClass expectedVehicleClass, double expectedCurbMass)
    {
        CompletedGenericBusCurbMassTest(jobType, archId, axleConfiguration, articulated, TPMLM, runIdx, vehicleCode, registrationClass, lowEntry, expectedVehicleClass, expectedCurbMass);
    }

    [
        // testcases for vehicle mass with a low tpmlm
        TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl2, false, 13000, 0, VC_CE, RC_II, false,
            VehicleClass.ClassP31SD, 13000 * 0.7),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl2, false, 13000, 0, VC_CE, RC_II, false,
            VehicleClass.ClassP31SD, 13000 * 0.7),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl2, false, 13000, 0, VC_CE, RC_II, false,
            VehicleClass.ClassP31SD, 13000 * 0.7),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl2, false, 13000, 0, VC_CE, RC_II, false,
            VehicleClass.ClassP31SD, 13000 * 0.7),

        TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl2, false, 13000, 4, VC_CF, RC_II, false,
            VehicleClass.ClassP31DD, 13000 * 0.7),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl2, false, 13000, 4, VC_CF, RC_II, false,
            VehicleClass.ClassP31DD, 13000 * 0.7),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl2, false, 13000, 4, VC_CF, RC_II, false,
            VehicleClass.ClassP31DD, 13000 * 0.7),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl2, false, 13000, 4, VC_CF, RC_II, false,
            VehicleClass.ClassP31DD, 13000 * 0.7),

        TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl2, false, 13000, 7, VC_CA, RC_II, false,
            VehicleClass.ClassP32SD, 13000 * 0.75),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl2, false, 13000, 7, VC_CA, RC_II, false,
            VehicleClass.ClassP32SD, 13000 * 0.75),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl2, false, 13000, 7, VC_CA, RC_II, false,
            VehicleClass.ClassP32SD, 13000 * 0.75),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl2, false, 13000, 7, VC_CA, RC_II, false,
            VehicleClass.ClassP32SD, 13000 * 0.75),

        TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl2, false, 13000, 9, VC_CB, RC_II, false,
            VehicleClass.ClassP32DD, 13000 * 0.75),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl2, false, 13000, 9, VC_CB, RC_II, false,
            VehicleClass.ClassP32DD, 13000 * 0.75),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl2, false, 13000, 9, VC_CB, RC_II, false,
            VehicleClass.ClassP32DD, 13000 * 0.75),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl2, false, 13000, 9, VC_CB, RC_II, false,
            VehicleClass.ClassP32DD, 13000 * 0.75),

        TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl3, false, 13000, 0, VC_CE, RC_II, false,
            VehicleClass.ClassP33SD, 14175),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl3, false, 13000, 0, VC_CE, RC_II, false,
            VehicleClass.ClassP33SD, 14491),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl3, false, 13000, 0, VC_CE, RC_II, false,
            VehicleClass.ClassP33SD, 14491),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl3, false, 13000, 0, VC_CE, RC_II, false,
            VehicleClass.ClassP33SD, 13616),

        TestCase(VectoSimulationJobType.ConventionalVehicle, ArchitectureID.UNKNOWN, Axl3, false, 13000, 4, VC_CF, RC_II, false,
            VehicleClass.ClassP33DD, 14725),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, ArchitectureID.S2, Axl3, false, 13000, 4, VC_CF, RC_II, false,
            VehicleClass.ClassP33DD, 15041),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, ArchitectureID.P2, Axl3, false, 13000, 4, VC_CF, RC_II, false,
            VehicleClass.ClassP33DD, 15041),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, ArchitectureID.E2, Axl3, false, 13000, 4, VC_CF, RC_II, false,
            VehicleClass.ClassP33DD, 14166),
    ]
    public void CompletedGenericBusCurbMassTest_LowTPMLM(VectoSimulationJobType jobType, ArchitectureID archId,
        AxleConfiguration axleConfiguration, bool articulated, double TPMLM, int runIdx,
        VehicleCode vehicleCode, RegistrationClass registrationClass, bool lowEntry, VehicleClass expectedVehicleClass, double expectedCurbMass)
    {
        CompletedGenericBusCurbMassTest(jobType, archId, axleConfiguration, articulated, TPMLM, runIdx, vehicleCode, registrationClass, lowEntry, expectedVehicleClass, expectedCurbMass);
    }

    public void PrimaryBusCurbMassTest(VectoSimulationJobType jobType, ArchitectureID archId, AxleConfiguration axleConfiguration, bool articulated, double TPMLM, int runIdx, VehicleClass expectedVehicleClass, double expectedCurbMass)
    {
        var addElectricComponents = false;
        var input = new Mock<IDeclarationInputDataProvider>()
            .PrimaryBus();
        switch (jobType) {
            case VectoSimulationJobType.ConventionalVehicle:
                input = input.Conventional();
                break;
            case VectoSimulationJobType.ParallelHybridVehicle:
            case VectoSimulationJobType.SerialHybridVehicle:
                input = input.HEV(archId);
                addElectricComponents = true;
                break;
            case VectoSimulationJobType.BatteryElectricVehicle:
                input = input.PEV(archId);
                addElectricComponents = true;
                break;
            default:
                throw new Exception();
        }

        var result = _runDataFactoryFactory.CreateDeclarationRunDataFactory(input.Object, null,
            null);

        var dataAdapter = (result as DeclarationModePrimaryBusRunDataFactory.PrimaryBusBase)?.DataAdapter;
        Assert.NotNull(dataAdapter);

        var vehicleType = VehicleCategory.HeavyBusPrimaryVehicle;

        var segment = DeclarationData.PrimaryBusSegments.Lookup(
            vehicleType, axleConfiguration, articulated);

        var mission = segment.Missions[runIdx];
        var vehicleInputData = GetMockVehicleData(TPMLM, axleConfiguration, addElectricComponents);
        var vehicleData =
            dataAdapter.CreateVehicleData(vehicleInputData, segment, mission, mission.Loadings.First(), false);

        Assert.AreEqual(expectedVehicleClass, mission.BusParameter.BusGroup);
        Assert.AreEqual(expectedCurbMass, vehicleData.CurbMass.Value());
    }

    public void CompletedGenericBusCurbMassTest(VectoSimulationJobType jobType, ArchitectureID archId,
            AxleConfiguration axleConfiguration, bool articulated, double TPMLM, int runIdx,
            VehicleCode vehicleCode,
            RegistrationClass registrationClass, bool lowEntry,
            VehicleClass expectedVehicleClass, double expectedCurbMassGeneric)
    {
        var addElectricComponents = false;
        var input = new Mock<IMultistageVIFInputData>()
            .CompletedBus();
        switch (jobType) {
            case VectoSimulationJobType.ConventionalVehicle:
                input = input.Conventional();
                break;
            case VectoSimulationJobType.ParallelHybridVehicle:
            case VectoSimulationJobType.SerialHybridVehicle:
                input = input.HEV(archId);
                addElectricComponents = true;
                break;
            case VectoSimulationJobType.BatteryElectricVehicle:
                input = input.PEV(archId);
                addElectricComponents = true;
                break;
            default:
                throw new Exception();
        }

        var result = _runDataFactoryFactory.CreateDeclarationRunDataFactory(input.Object, null,
            null);

        var dataAdapterSpecific = (result as DeclarationModeCompletedBusRunDataFactory.CompletedBusBase)?.DataAdapterSpecific;
        Assert.NotNull(dataAdapterSpecific);

        var dataAdapterGeneric = (result as DeclarationModeCompletedBusRunDataFactory.CompletedBusBase)?.DataAdapterGeneric;
        Assert.NotNull(dataAdapterGeneric);

        var vehicleType = VehicleCategory.HeavyBusPrimaryVehicle;

        var primarySegment = DeclarationData.PrimaryBusSegments.Lookup(
            vehicleType, axleConfiguration, articulated);
        var completedSegment = DeclarationData.CompletedBusSegments.Lookup(axleConfiguration.NumAxles(),
            vehicleCode, registrationClass, 10, 3.1.SI<Meter>(), lowEntry);

        var curbMassCompleted = 12345;

        var mission = primarySegment.Missions[runIdx];
        var completedMission = completedSegment.Missions.FirstOrDefault(m => m.MissionType == mission.MissionType);
        var loading = mission.Loadings.First();
        var vehicleInputData = GetMockVehicleData(TPMLM, axleConfiguration, addElectricComponents);
        var completedInput = GetMockCompletedInputData(curbMassCompleted);


        var vehicleDataGeneric =
            dataAdapterGeneric.CreateVehicleData(vehicleInputData, primarySegment, mission, loading, false);
        var vehicleDataSpecific =
            dataAdapterSpecific.CreateVehicleData(vehicleInputData, completedInput, completedSegment, completedMission, loading);

        Assert.AreEqual(expectedVehicleClass, mission.BusParameter.BusGroup);
        Assert.AreEqual(expectedCurbMassGeneric, vehicleDataGeneric.CurbMass.Value());
        Assert.AreEqual(curbMassCompleted, vehicleDataSpecific.CurbMass.Value());
    }

    private IVehicleDeclarationInputData GetMockCompletedInputData(double curbMass)
    {
        var mock = new Mock<IVehicleDeclarationInputData>();
        mock.Setup(v => v.NumberPassengerSeatsLowerDeck).Returns(10);
        mock.Setup(v => v.NumberPassengerSeatsUpperDeck).Returns(0);
        mock.Setup(v => v.NumberPassengersStandingLowerDeck).Returns(0);
        mock.Setup(v => v.NumberPassengersStandingUpperDeck).Returns(0);
        mock.Setup(v => v.Length).Returns(8.SI<Meter>());
        mock.Setup(v => v.Width).Returns(2.55.SI<Meter>());
        mock.Setup(v => v.Height).Returns(3.5.SI<Meter>());
        mock.Setup(v => v.GrossVehicleMassRating).Returns(30000.SI<Kilogram>());
        mock.Setup(v => v.CurbMassChassis).Returns(curbMass.SI<Kilogram>());
        return mock.Object;
    }


    private IVehicleDeclarationInputData GetMockVehicleData(double tpmlm, AxleConfiguration axleConfiguration, bool addElectricComponents)
    {
        var mock = new Mock<IVehicleDeclarationInputData>();

        var components = new Mock<IVehicleComponentsDeclaration>();
        mock.Setup(v => v.GrossVehicleMassRating).Returns(tpmlm.SI<Kilogram>());
        mock.Setup(v => v.Components).Returns(components.Object);
        var axleWheels = new Mock<IAxlesDeclarationInputData>();
        components.Setup(c => c.AxleWheels).Returns(axleWheels.Object);
        var axle1 = new Mock<IAxleDeclarationInputData>();

        axle1.Setup(a => a.AxleType).Returns(AxleType.VehicleDriven);
        var tyre = new Mock<ITyreDeclarationInputData>();
        axle1.Setup(a => a.Tyre).Returns(tyre.Object);
        tyre.Setup(t => t.Dimension).Returns("275/70 R22.5");

        var axles = new List<IAxleDeclarationInputData>() { axle1.Object };
        for (var i = 0; i < axleConfiguration.NumAxles(); i++) {
            var axle = new Mock<IAxleDeclarationInputData>();
            axle.Setup(a => a.AxleType).Returns(AxleType.VehicleNonDriven);
            axle.Setup(a => a.Tyre).Returns(tyre.Object);
            axles.Add(axle.Object);
        }
        axleWheels.Setup(a => a.AxlesDeclaration).Returns(axles);

        var adas = new Mock<IAdvancedDriverAssistantSystemDeclarationInputData>();
        mock.Setup(v => v.ADAS).Returns(adas.Object);
        adas.Setup(a => a.EcoRoll).Returns(EcoRollType.None);
        adas.Setup(a => a.PredictiveCruiseControl).Returns(PredictiveCruiseControlType.None);
        adas.Setup(a => a.EngineStopStart).Returns(false);

        if (!addElectricComponents) {
            return mock.Object;
        }

        var bat = new Mock<IBatteryPackDeclarationInputData>();
        var reess = new Mock<IElectricStorageSystemDeclarationInputData>();
        components.Setup(c => c.ElectricStorage).Returns(reess.Object);
        reess.Setup(r => r.ElectricStorageElements).Returns(new[] { new ElectricStorageWrapper() { REESSPack = bat.Object } }.Cast<IElectricStorageDeclarationInputData>().ToList());

        bat.Setup(m => m.MaxCurrentMap).Returns(
            GetMockTableData(new[] {
                    new[]{"0.0", "100.0", "100.0"},
                    new[]{"0.5", "100.0", "100.0"},
                    new[]{"1.0", "100.0", "100.0"}
            }));

        bat.Setup(m => m.InternalResistanceCurve).Returns(
            GetMockTableData(new[] {
                    new[]{"0.0", "0.001"},
                    new[]{"0.5", "0.001"},
                    new[]{"1.0", "0.001"}
            }));
        bat.Setup(m => m.VoltageCurve).Returns(
            GetMockTableData(new[] {
                    new[]{"0.0", "3000.0"},
                    new[]{"0.5", "310.0"},
                    new[]{"1.0", "320.0"}
            }));
        bat.Setup(b => b.Capacity).Returns(50.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>());

        var em = new Mock<IElectricMotorDeclarationInputData>();
        var ems = new Mock<IElectricMachinesDeclarationInputData>();
        ems.Setup(x => x.Entries)
            .Returns(new[] {
                    new ElectricMachineEntry<IElectricMotorDeclarationInputData>()
                        { ElectricMachine = em.Object, Count = 1 }
            }.ToList());
        var voltageLevel = new Mock<IElectricMotorVoltageLevel>();
        voltageLevel.SetupGet(x => x.ContinuousTorque).Returns(400.SI<NewtonMeter>());
        voltageLevel.SetupGet(x => x.ContinuousTorqueSpeed).Returns(2000.RPMtoRad());
        em.Setup(x => x.VoltageLevels).Returns(new[] { voltageLevel.Object }.ToList());
        components.Setup(c => c.ElectricMachines).Returns(ems.Object);
        return mock.Object;
    }

    public static TableData GetMockTableData(string[][] values)
    {
        var result = new TableData();


        foreach (var col in values.First()) {
            result.Columns.Add(new DataColumn());
        }

        foreach (var row in values) {
            result.Rows.Add(result.NewRow().ItemArray = row);
        }
        return result;

    }

	public class ElectricStorageWrapper : IElectricStorageDeclarationInputData
	{
		#region Implementation of IElectricStorageDeclarationInputData

		public IREESSPackInputData REESSPack { get; set; }
		public int Count { get; set; }
		public int StringId { get; set; }

		#endregion
	}
}