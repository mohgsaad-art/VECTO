using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Generic;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.RunDataFactory
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal class RunDataFactoryFactoryTests
    {
        private IVectoRunDataFactoryFactory _runDataFactoryFactory;

		[OneTimeSetUp]
        public void OneTimeSetup()
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			_runDataFactoryFactory = kernel.Get<IVectoRunDataFactoryFactory>();

            Assert.IsTrue(_runDataFactoryFactory.GetType() == typeof(VectoRunDataFactoryFactory));

        }

        #region HeavyLorry

        [TestCase(typeof(DeclarationModeHeavyLorryRunDataFactory.Conventional), typeof(DeclarationDataAdapterHeavyLorry.Conventional))]
        public void ConventionalHeavyLorryTest(Type expectedRunDataFactory, Type expectedDataAdapter)
        {
            var input = new Mock<IDeclarationInputDataProvider>()
                .Conventional()
                .Lorry();

            dynamic factory = CreateRunDataFactory(input);
            Assert.IsInstanceOf(expectedRunDataFactory, factory, $"Invalid type of RunDataFactory! Expected {expectedRunDataFactory} got {factory.GetType()}");
            Assert.IsInstanceOf(expectedDataAdapter, factory.DataAdapter, $"Invalid type of DeclarationDataAdapter! Expected {expectedDataAdapter} got {factory.DataAdapter.GetType()}");
		}
       
		[TestCase(ArchitectureID.S2, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S2), typeof(DeclarationDataAdapterHeavyLorry.HEV_S2))]
        [TestCase(ArchitectureID.S3, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S3), typeof(DeclarationDataAdapterHeavyLorry.HEV_S3))] 
        [TestCase(ArchitectureID.S4, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S4), typeof(DeclarationDataAdapterHeavyLorry.HEV_S4))]
        [TestCase(ArchitectureID.S_IEPC, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S_IEPC), typeof(DeclarationDataAdapterHeavyLorry.HEV_S_IEPC))]
        [TestCase(ArchitectureID.P1, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P1), typeof(DeclarationDataAdapterHeavyLorry.HEV_P1))]
        [TestCase(ArchitectureID.P2, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P2), typeof(DeclarationDataAdapterHeavyLorry.HEV_P2))]
        [TestCase(ArchitectureID.P2_5, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P2_5), typeof(DeclarationDataAdapterHeavyLorry.HEV_P2_5))]
        [TestCase(ArchitectureID.P3, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P3), typeof(DeclarationDataAdapterHeavyLorry.HEV_P3))]
        [TestCase(ArchitectureID.P4, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P4), typeof(DeclarationDataAdapterHeavyLorry.HEV_P4))]
        [TestCase(ArchitectureID.P_IHPC, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P_IHPC), typeof(DeclarationDataAdapterHeavyLorry.HEV_P2))]
        public void HEV_HeavyLorryTest(ArchitectureID arch, Type expectedRunDataFactory, Type expectedDataAdapter)
        {
            var input = new Mock<IDeclarationInputDataProvider>()
                .HEV(arch)
                .Lorry();
			dynamic factory = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, factory, $"Invalid type of RunDataFactory! Expected {expectedRunDataFactory} got {factory.GetType()}");
			Assert.IsInstanceOf(expectedDataAdapter, factory.DataAdapter, $"Invalid type of DeclarationDataAdapter! Expected {expectedDataAdapter} got {factory.DataAdapter.GetType()}");

        }

        [TestCase(ArchitectureID.E2, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E2), typeof(DeclarationDataAdapterHeavyLorry.PEV_E2))]
        [TestCase(ArchitectureID.E3, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E3), typeof(DeclarationDataAdapterHeavyLorry.PEV_E3))]
        [TestCase(ArchitectureID.E4, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E4), typeof(DeclarationDataAdapterHeavyLorry.PEV_E4))]
        [TestCase(ArchitectureID.E_IEPC, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E_IEPC), typeof(DeclarationDataAdapterHeavyLorry.PEV_E_IEPC))]
        public void PEV_HeavyLorryTest(ArchitectureID arch, Type expectedRunDataFactory, Type expectedDataAdapter)
        {
            var input = new Mock<IDeclarationInputDataProvider>()
                .PEV(arch)
                .Lorry();
			dynamic factory = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, factory, $"Invalid type of RunDataFactory! Expected {expectedRunDataFactory} got {factory.GetType()}");
			Assert.IsInstanceOf(expectedDataAdapter, factory.DataAdapter, $"Invalid type of DeclarationDataAdapter! Expected {expectedDataAdapter} got {factory.DataAdapter.GetType()}");

        }

        [Test]
        public void Exempted_HeavyLorryTest([Values] ArchitectureID architectureId, [Values] VectoSimulationJobType simType)
		{
			var expectedRunDataFactory = typeof(DeclarationModeHeavyLorryRunDataFactory.Exempted);
			var expectedDataAdapter = typeof(DeclarationDataAdapterHeavyLorry.Exempted);
            var input = new Mock<IDeclarationInputDataProvider>()
                .Exempted()
                .Lorry();

            input.Setup(m => m.JobInputData.JobType).Returns(simType);
            input.Setup(m => m.JobInputData.Vehicle.ArchitectureID).Returns(architectureId);
            dynamic factory = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, factory, $"Invalid type of RunDataFactory! Expected {expectedRunDataFactory} got {factory.GetType()}");
			Assert.IsInstanceOf(expectedDataAdapter, factory.DataAdapter, $"Invalid type of DeclarationDataAdapter! Expected {expectedDataAdapter} got {factory.DataAdapter.GetType()}");


        }

        #endregion HeavyLorry

        #region PrimaryBus

        [TestCase(typeof(DeclarationModePrimaryBusRunDataFactory.Conventional), typeof(DeclarationDataAdapterPrimaryBus.Conventional))]
        public void ConventionalPrimaryBus(Type expectedRunDataFactory, Type expectedDataAdapter)
        {
            var input = new Mock<IDeclarationInputDataProvider>()
                .Conventional()
                .PrimaryBus();
			dynamic factory = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, factory, $"Invalid type of RunDataFactory! Expected {expectedRunDataFactory} got {factory.GetType()}");
			Assert.IsInstanceOf(expectedDataAdapter, factory.DataAdapter, $"Invalid type of DeclarationDataAdapter! Expected {expectedDataAdapter} got {factory.DataAdapter.GetType()}");

        }

        [TestCase(ArchitectureID.S2, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S2), typeof(DeclarationDataAdapterPrimaryBus.HEV_S2))]
        [TestCase(ArchitectureID.S3, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S3), typeof(DeclarationDataAdapterPrimaryBus.HEV_S3))]
        [TestCase(ArchitectureID.S4, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S4), typeof(DeclarationDataAdapterPrimaryBus.HEV_S4))]
        [TestCase(ArchitectureID.S_IEPC, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S_IEPC), typeof(DeclarationDataAdapterPrimaryBus.HEV_S_IEPC))]
        [TestCase(ArchitectureID.P1, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P1), typeof(DeclarationDataAdapterPrimaryBus.HEV_P1))]
        [TestCase(ArchitectureID.P2, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P2), typeof(DeclarationDataAdapterPrimaryBus.HEV_P2))]
        [TestCase(ArchitectureID.P2_5, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P2_5), typeof(DeclarationDataAdapterPrimaryBus.HEV_P2_5))]
        [TestCase(ArchitectureID.P3, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P3), typeof(DeclarationDataAdapterPrimaryBus.HEV_P3))]
        [TestCase(ArchitectureID.P4, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P4), typeof(DeclarationDataAdapterPrimaryBus.HEV_P4))]
        public void HEV_PrimaryBus(ArchitectureID arch, Type expectedRunDataFactory, Type expectedDataAdapter)
        {
            var input = new Mock<IDeclarationInputDataProvider>()
                .HEV(arch)
                .PrimaryBus();
			dynamic factory = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, factory, $"Invalid type of RunDataFactory! Expected {expectedRunDataFactory} got {factory.GetType()}");
			Assert.IsInstanceOf(expectedDataAdapter, factory.DataAdapter, $"Invalid type of DeclarationDataAdapter! Expected {expectedDataAdapter} got {factory.DataAdapter.GetType()}");

        }

        [TestCase(ArchitectureID.E2, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E2), typeof(DeclarationDataAdapterPrimaryBus.PEV_E2))]
        [TestCase(ArchitectureID.E3, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E3), typeof(DeclarationDataAdapterPrimaryBus.PEV_E3))]
        [TestCase(ArchitectureID.E4, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E4), typeof(DeclarationDataAdapterPrimaryBus.PEV_E4))]
        [TestCase(ArchitectureID.E_IEPC, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E_IEPC), typeof(DeclarationDataAdapterPrimaryBus.PEV_E_IEPC))]
        public void PEV_PrimaryBus(ArchitectureID arch, Type expectedRunDataFactory, Type expectedDataAdapter)
        {
            var input = new Mock<IDeclarationInputDataProvider>()
                .PEV(arch)
                .PrimaryBus();
			dynamic factory = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, factory, $"Invalid type of RunDataFactory! Expected {expectedRunDataFactory} got {factory.GetType()}");
			Assert.IsInstanceOf(expectedDataAdapter, factory.DataAdapter, $"Invalid type of DeclarationDataAdapter! Expected {expectedDataAdapter} got {factory.DataAdapter.GetType()}");

        }

        [Test]
        public void Exempted_PrimaryBusTest([Values] ArchitectureID architectureId, [Values] VectoSimulationJobType simType)
		{
			var expectedRunDataFactory = typeof(DeclarationModePrimaryBusRunDataFactory.Exempted);
			var expectedDataAdapter = typeof(DeclarationDataAdapterPrimaryBus.Exempted);
            var input = new Mock<IDeclarationInputDataProvider>()
                .Exempted()
                .PrimaryBus();

            input.Setup(m => m.JobInputData.JobType).Returns(simType);
            input.Setup(m => m.JobInputData.Vehicle.ArchitectureID).Returns(architectureId);
            dynamic factory = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, factory, $"Invalid type of RunDataFactory! Expected {expectedRunDataFactory} got {factory.GetType()}");
			Assert.IsInstanceOf(expectedDataAdapter, factory.DataAdapter, $"Invalid type of DeclarationDataAdapter! Expected {expectedDataAdapter} got {factory.DataAdapter.GetType()}");

        }

        #endregion PrimaryBus

        #region CompletedBus

        [TestCase(typeof(DeclarationModeCompletedBusRunDataFactory.Conventional), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.Conventional), typeof(DeclarationDataAdapterSpecificCompletedBus.Conventional))]
        public void ConventionalCompletedBus(Type expectedRunDataFactory, Type expectedDataAdapterGeneric, Type expectedDataAdapterSpecific)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .Conventional()
                .CompletedBus();
			dynamic fact = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, fact);
			Assert.IsInstanceOf(expectedDataAdapterGeneric, fact.DataAdapterGeneric);
			Assert.IsInstanceOf(expectedDataAdapterSpecific, fact.DataAdapterSpecific);

        }

        [TestCase(ArchitectureID.S2, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S2), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S2), typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S2))]
		[TestCase(ArchitectureID.S3, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S3), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S3), typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S3))]
        [TestCase(ArchitectureID.S4, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S4), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S4), typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S4))]
        [TestCase(ArchitectureID.S_IEPC, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S_IEPC), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S_IEPC), typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S_IEPC))]
        [TestCase(ArchitectureID.P1, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P1), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P1), typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P1))]
        [TestCase(ArchitectureID.P2, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P2), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P2), typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P2))]
        [TestCase(ArchitectureID.P3, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P3), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P3), typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P3))]
        [TestCase(ArchitectureID.P2_5, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P2_5), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P2_5), typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P2_5))]
        [TestCase(ArchitectureID.P4,typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P4), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P4), typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P4))]
        public void HEV_CompletedBus(ArchitectureID arch, Type expectedRunDataFactory, Type expectedDataAdapterGeneric, Type expectedDataAdapterSpecific)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(arch)
                .CompletedBus();
            dynamic fact = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, fact);
			Assert.IsInstanceOf(expectedDataAdapterGeneric, fact.DataAdapterGeneric);
			Assert.IsInstanceOf(expectedDataAdapterSpecific, fact.DataAdapterSpecific);
        }

        [TestCase(ArchitectureID.E2, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E2), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E2), typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E2))]
		[TestCase(ArchitectureID.E3, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E3), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E3), typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E3))]
		[TestCase(ArchitectureID.E4, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E4), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E4), typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E4))]
		[TestCase(ArchitectureID.E_IEPC, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E_IEPC), typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E_IEPC), typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E_IEPC))]
        public void PEV_CompletedBus(ArchitectureID arch, Type expectedRunDataFactory, Type expectedDataAdapterGeneric, Type expectedDataAdapterSpecific)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .PEV(arch)
                .CompletedBus();
            dynamic fact = CreateRunDataFactory(input);
			Assert.IsInstanceOf(expectedRunDataFactory, fact);
            Assert.IsInstanceOf(expectedDataAdapterGeneric, fact.DataAdapterGeneric);
            Assert.IsInstanceOf(expectedDataAdapterSpecific, fact.DataAdapterSpecific);
		}

        #endregion CompletedBus

		private IVectoRunDataFactory CreateRunDataFactory(Mock inputMock)
		{
			var s = inputMock.Object;
			var result = _runDataFactoryFactory.CreateDeclarationRunDataFactory((IInputDataProvider)inputMock.Object, null,
				null);
			return result;
		}

	}

    internal static class InputDataProviderMockExtension
    {
        internal static Mock<IDeclarationInputDataProvider> Conventional(this Mock<IDeclarationInputDataProvider> mock)
        {
            mock.Setup(provider => provider.JobInputData.JobType)
                .Returns(VectoSimulationJobType.ConventionalVehicle);

            mock.Setup(provider => provider.JobInputData.Vehicle.VehicleType).
                Returns(VectoSimulationJobType.ConventionalVehicle);

            mock.Setup(p => p.JobInputData.Vehicle.ArchitectureID).
                Returns(ArchitectureID.UNKNOWN);

            return mock;
        }

        internal static Mock<IDeclarationInputDataProvider> HEV(this Mock<IDeclarationInputDataProvider> mock, ArchitectureID arch)
        {

            VectoSimulationJobType type;
            switch (arch)
            {
                case ArchitectureID.P1:
                case ArchitectureID.P2:
                case ArchitectureID.P2_5:
                case ArchitectureID.P3:
                case ArchitectureID.P4:
                    type = VectoSimulationJobType.ParallelHybridVehicle;
                    break;
                case ArchitectureID.P_IHPC:
                    type = VectoSimulationJobType.IHPC;
                    break;
                case ArchitectureID.S2:
                case ArchitectureID.S3:
                case ArchitectureID.S4:
                    type = VectoSimulationJobType.SerialHybridVehicle;
                    break;
                case ArchitectureID.S_IEPC:
                    type = VectoSimulationJobType.IEPC_S;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arch), arch, null);
            }

            arch = arch == ArchitectureID.P_IHPC ? ArchitectureID.P2 : arch;
            mock.Setup(p => p.JobInputData.Vehicle.ArchitectureID).
                Returns(arch);
            mock.Setup(p => p.JobInputData.JobType).
                Returns(type);
            mock.Setup(p => p.JobInputData.Vehicle.VehicleType).
                Returns(type);
            return mock;
        }

        internal static Mock<IDeclarationInputDataProvider> PEV(this Mock<IDeclarationInputDataProvider> mock, ArchitectureID arch)
        {
            var type = VectoSimulationJobType.BatteryElectricVehicle;
            if (arch == ArchitectureID.E_IEPC)
            {
                type = VectoSimulationJobType.IEPC_E;
            }

            mock.Setup(p => p.JobInputData.Vehicle.ArchitectureID).
                Returns(arch);
            mock.Setup(p => p.JobInputData.JobType).
                Returns(
                VectoSimulationJobType.BatteryElectricVehicle);
            mock.Setup(p => p.JobInputData.Vehicle.VehicleType).
                Returns(
                type);
            return mock;
        }
        internal static Mock<IDeclarationInputDataProvider> Lorry(this Mock<IDeclarationInputDataProvider> mock)
        {
            mock.Setup(provider => provider.JobInputData.Vehicle.VehicleCategory)
                .Returns(VehicleCategory.RigidTruck);
            return mock;
        }

        internal static Mock<IDeclarationInputDataProvider> PrimaryBus(this Mock<IDeclarationInputDataProvider> mock)
        {
            mock.Setup(p => p.JobInputData.Vehicle.VehicleCategory).
                Returns(VehicleCategory.HeavyBusPrimaryVehicle);
            return mock;
        }

        internal static Mock<IDeclarationInputDataProvider> Exempted(this Mock<IDeclarationInputDataProvider> mock)
        {
            mock.Setup(p => p.JobInputData.Vehicle.ExemptedVehicle)
                .Returns(true);
            return mock;
        }

        internal static Mock<IMultistageVIFInputData> Conventional(this Mock<IMultistageVIFInputData> mock)
        {
            mock.Setup(p => p.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.VehicleType)
                .Returns(VectoSimulationJobType.ConventionalVehicle);
            mock.Setup(p => p.MultistageJobInputData.JobInputData.JobType)
                .Returns(VectoSimulationJobType.ConventionalVehicle);
            mock.Setup(p => p.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID)
                .Returns(ArchitectureID.UNKNOWN);
            return mock;
        }
        internal static Mock<IMultistageVIFInputData> CompletedBus(this Mock<IMultistageVIFInputData> mock)
        {
            mock.Setup(p => p.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory)
                .Returns(VehicleCategory.HeavyBusCompletedVehicle);
            return mock;
        }

        internal static Mock<IMultistageVIFInputData> HEV(this Mock<IMultistageVIFInputData> mock, ArchitectureID arch)
        {
            var type = arch.ToString().StartsWith("P")
                ? VectoSimulationJobType.ParallelHybridVehicle
                : VectoSimulationJobType.SerialHybridVehicle;
            if (arch == ArchitectureID.S_IEPC)
            {
                type = VectoSimulationJobType.IEPC_S;
            }

            if (arch == ArchitectureID.E_IEPC)
            {
                type = VectoSimulationJobType.IEPC_E;
            }
            mock.Setup(p => p.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID).
                Returns(arch);
            mock.Setup(p => p.MultistageJobInputData.JobInputData.JobType).
                Returns(type);
            mock.Setup(p => p.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.VehicleType).
                Returns(type);
            return mock;
        }

        internal static Mock<IMultistageVIFInputData> PEV(this Mock<IMultistageVIFInputData> mock, ArchitectureID arch)
        {
            var type = arch == ArchitectureID.E_IEPC ? VectoSimulationJobType.IEPC_E : VectoSimulationJobType.BatteryElectricVehicle;
            mock.Setup(p => p.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID).
                Returns(arch);
            mock.Setup(p => p.MultistageJobInputData.JobInputData.JobType).
                Returns(type);
            mock.Setup(p => p.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.VehicleType).
                Returns(type);
            return mock;
        }

        internal static Mock<IMultistageVIFInputData> Exempted(this Mock<IMultistageVIFInputData> mock)
        {
            mock.Setup(p => p.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle).Returns(true);
            return mock;
        }




    }

}
