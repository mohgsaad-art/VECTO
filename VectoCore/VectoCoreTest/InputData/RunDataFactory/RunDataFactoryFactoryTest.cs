using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
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
using TUGraz.VectoCore.Tests.Integration.Hybrid;

namespace TUGraz.VectoCore.Tests.InputData.RunDataFactory
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
    internal class RunDataFactoryFactoryTest
	{
		private IVectoRunDataFactoryFactory _runDataFactoryFactory;
		


		[OneTimeSetUp]
		public void OneTimeSetup()
		{
#if (MOCKUP)
			Assert.Ignore("Tests not meaningful in mockup mode");
#endif
			var kernel = new StandardKernel(new VectoNinjectModule()) {
			};

			kernel.Bind<IDeclarationDataAdapter>().ToMethod(
				x => {
					var mock = new Mock<IDeclarationDataAdapter>();
					return mock.Object;
				});
			_runDataFactoryFactory = kernel.Get<IVectoRunDataFactoryFactory>();

			Assert.IsTrue(_runDataFactoryFactory.GetType() == typeof(VectoRunDataFactoryFactory));

		}

		private IVectoRunDataFactory CreateRunDataFactory(Mock inputMock, Type expectedRunDataType,
			Type expectedDeclarationDataAdapterType = null)
		{
			var s = inputMock.Object;
			var result = _runDataFactoryFactory.CreateDeclarationRunDataFactory((IInputDataProvider)inputMock.Object, null,
				null);
			Assert.IsTrue(result.GetType() == expectedRunDataType, $"Invalid type of RunDataFactory! Expected {expectedRunDataType} got {result.GetType()}");
			if (expectedDeclarationDataAdapterType != null) {
				dynamic concreteResult = result;
				Assert.IsTrue(concreteResult.DataAdapter.GetType() == expectedDeclarationDataAdapterType, $"Invalid type of DeclarationDataAdapter! Expected {expectedDeclarationDataAdapterType} got {concreteResult.DataAdapter.GetType()}");
			}

			return result;
		}

		private void CheckCompletedBusAdapters(IVectoRunDataFactory runDataResult, Type genericDataAdapter,
			Type specificDataAdapter)
		{

			var genericPropertyInfo = runDataResult.GetType().GetProperty(nameof(DeclarationModeCompletedBusRunDataFactory.CompletedBusBase.DataAdapterGeneric));
			var generic = genericPropertyInfo.GetValue(runDataResult, null);

			
			var specificPropertyInfo = runDataResult.GetType().GetProperty(nameof(DeclarationModeCompletedBusRunDataFactory.CompletedBusBase.DataAdapterSpecific));
			var specific = specificPropertyInfo.GetValue(runDataResult, null);

            Assert.AreEqual(genericDataAdapter, generic.GetType());
			Assert.AreEqual(specificDataAdapter, specific.GetType());
		}

#region HeavyLorry
		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.Conventional)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void ConventionalHeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.Conventional()
				.Lorry();

			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.Conventional), expectedDataAdapter);
			
		}
		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_S2)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_S2_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S2)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S2), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_S3)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_S3_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S3)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S3), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_S4)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_S4_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S4)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S4), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_S_IEPC)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_S_IEPC_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S_IEPC)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_S_IEPC), expectedDataAdapter);
		}


		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_P1)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_P1_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P1)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P1), expectedDataAdapter);
		}
		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_P2)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_P2_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P2)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P2), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_P2_5)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_P2_5_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P2_5)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P2_5), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_P3)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_P3_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P3)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P3), expectedDataAdapter);
		}
		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_P4)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_P4_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P4)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P4), expectedDataAdapter);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		//[TestCase(typeof(DeclarationDataAdapterHeavyLorry.HEV_))]
		public void HEV_P_IHPC_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P_IHPC)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.HEV_P_IHPC), expectedDataAdapter);
		}





		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.PEV_E2)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void PEV_E2_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E2)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E2), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.PEV_E3)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void PEV_E3_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E3)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E3), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.PEV_E4)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void PEV_E4_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E4)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E4), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterHeavyLorry.PEV_E_IEPC)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void PEV_E_IEPC_HeavyLorryTest(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E_IEPC)
				.Lorry();
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.PEV_E_IEPC), expectedDataAdapter);
		}

		[Test,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Exempted_HeavyLorryTest([Values]ArchitectureID architectureId, [Values]VectoSimulationJobType simType, [Values]bool checkDeclarationDataAdapter)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.Exempted()
				.Lorry();

			input.Setup(m => m.JobInputData.JobType).Returns(simType);
			input.Setup(m => m.JobInputData.Vehicle.ArchitectureID).Returns(architectureId);
			CreateRunDataFactory(input, typeof(DeclarationModeHeavyLorryRunDataFactory.Exempted), 
				checkDeclarationDataAdapter ? typeof(DeclarationDataAdapterHeavyLorry.Exempted) : null);

		}


        #endregion HeavyLorry
        #region PrimaryBus
        [TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.Conventional)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void ConventionalPrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.Conventional()
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.Conventional), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.HEV_S2)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_S2_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S2)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S2), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.HEV_S3)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_S3_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S3)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S3), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.HEV_S4)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_S4_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S4)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S4), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.HEV_S_IEPC)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_S_IEPC_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.S_IEPC)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_S_IEPC), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.HEV_P1)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_P1_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P1)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P1), expectedDataAdapter);
		}


		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.HEV_P2)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_P2_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P2)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P2), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.HEV_P2_5)),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void HEV_P2_5_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P2_5)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P2_5), expectedDataAdapter);
		}

        [TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.HEV_P3)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_P3_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P3)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P3), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.HEV_P4)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_P4_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.HEV(ArchitectureID.P4)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.HEV_P4), expectedDataAdapter);
		}


		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.PEV_E2)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void PEV_E2_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E2)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E2), expectedDataAdapter);
		}


		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.PEV_E3)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void PEV_E3_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E3)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E3), expectedDataAdapter);
		}
		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.PEV_E4)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void PEV_E4_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E4)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E4), expectedDataAdapter);
		}

		[TestCase()]
		[TestCase(typeof(DeclarationDataAdapterPrimaryBus.PEV_E_IEPC)),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void PEV_E_IEPC_PrimaryBus(Type expectedDataAdapter = null)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.PEV(ArchitectureID.E_IEPC)
				.PrimaryBus();
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.PEV_E_IEPC), expectedDataAdapter);
		}


		[Test,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Exempted_PrimaryBusTest([Values] ArchitectureID architectureId, [Values] VectoSimulationJobType simType, [Values] bool checkDeclarationDataAdapter)
		{
			var input = new Mock<IDeclarationInputDataProvider>()
				.Exempted()
				.PrimaryBus();

			input.Setup(m => m.JobInputData.JobType).Returns(simType);
			input.Setup(m => m.JobInputData.Vehicle.ArchitectureID).Returns(architectureId);
			CreateRunDataFactory(input, typeof(DeclarationModePrimaryBusRunDataFactory.Exempted), checkDeclarationDataAdapter ? typeof(DeclarationDataAdapterPrimaryBus.Exempted) : null);

		}


        #endregion PrimaryBus
        #region CompletedBus
        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void ConventionalCompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .Conventional()
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.Conventional));
			if(checkCompletedBusAdapters){
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.Conventional), 
				typeof(DeclarationDataAdapterSpecificCompletedBus.Conventional));
			}

        }

        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_S2_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(ArchitectureID.S2)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S2));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S2),
					typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S2));
			}

        }

        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_S3_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(ArchitectureID.S3)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S3));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S3),
					typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S3));
			}
        }

        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_S4_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(ArchitectureID.S4)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S4));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S4),
					typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S4));
			}
        }

        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_S_IEPC_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(ArchitectureID.S_IEPC)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_S_IEPC));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_S_IEPC),
					typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_S_IEPC));
			}
        }

        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_P1_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(ArchitectureID.P1)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P1));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P1),
					typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P1));
			}
        }


        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_P2_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(ArchitectureID.P2)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P2));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P2),
					typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P2));
			}
        }


        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_P3_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(ArchitectureID.P3)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P3));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P3),
					typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P3));
			}
        }


        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_P2_5_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(ArchitectureID.P2_5)
                .CompletedBus();
			var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P2_5));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P2_5),
					typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P2_5));
			}
        }


        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void HEV_P4_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .HEV(ArchitectureID.P4)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.HEV_P4));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.HEV_P4),
					typeof(DeclarationDataAdapterSpecificCompletedBus.HEV_P4));
			}
        }


        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void PEV_E2_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .PEV(ArchitectureID.E2)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E2));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E2),
					typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E2));
			}
        }


        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void PEV_E3_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .PEV(ArchitectureID.E3)
                .CompletedBus();
			var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E3));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E3),
					typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E3));
			}
        }
        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void PEV_E4_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .PEV(ArchitectureID.E4)
                .CompletedBus();
			var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E4));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E4),
					typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E4));
			}
        }

        [TestCase()]
		[TestCase(true),
		Category(Definitions.TESTCASE_MIGRATED)]
        public void PEV_E_IEPC_CompletedBus(bool checkCompletedBusAdapters = false)
        {
            var input = new Mock<IMultistageVIFInputData>()
                .PEV(ArchitectureID.E_IEPC)
                .CompletedBus();
            var fact = CreateRunDataFactory(input, typeof(DeclarationModeCompletedBusRunDataFactory.PEV_E_IEPC));
			if (checkCompletedBusAdapters)
			{
				CheckCompletedBusAdapters(fact, typeof(DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration.PEV_E_IEPC),
					typeof(DeclarationDataAdapterSpecificCompletedBus.PEV_E_IEPC));
			}

        }
        #endregion CompletedBus
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
			switch (arch) {
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
			if (arch == ArchitectureID.E_IEPC) {
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
			if (arch == ArchitectureID.S_IEPC) {
				type = VectoSimulationJobType.IEPC_S;
			}

			if (arch == ArchitectureID.E_IEPC) {
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
