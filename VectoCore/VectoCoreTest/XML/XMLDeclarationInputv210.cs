using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	public class XMLDeclarationInputv210
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		private const string BASE_DIR = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\";


		[TestCase(@"HeavyLorry\Conventional_heavyLorry_AMT.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_S3.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_S4.xml"),
		TestCase(@"HeavyLorry\HEV_heavyLorry_AMT_Px.xml"),
		TestCase(@"HeavyLorry\IEPC_heavyLorry.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_AMT_E2.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_APT-N_E2.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_E3.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_E4.xml"),
		TestCase(@"MediumLorry\Conventional_MediumLorry_AMT.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_AMT_S2.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_IEPC-S.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_S3.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_S4.xml"),
		TestCase(@"MediumLorry\HEV_mediumLorry_AMT_Px.xml"),
		TestCase(@"MediumLorry\IEPC_mediumLorry.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_AMT_E2.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_APT-N_E2.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_E3.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_E4.xml"),
		TestCase(@"PrimaryBus\Conventional_primaryBus_AMT.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_AMT_S2.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_IEPC-S.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_S3.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_S4.xml"),
		TestCase(@"PrimaryBus\HEV_primaryBus_AMT_Px.xml"),
		TestCase(@"PrimaryBus\IEPC_primaryBus.xml"),
		TestCase(@"PrimaryBus\PEV_primaryBus_AMT_E2.xml"),
		TestCase(@"PrimaryBus\PEV_primaryBus_E3.xml"),
		TestCase(@"PrimaryBus\PEV_primaryBus_E4.xml"),
		//TestCase(@"CompletedBus\Conventional_completedBus_1.xml"),
		//TestCase(@"CompletedBus\HEV_completedBus_1.xml"),
		//TestCase(@"CompletedBus\IEPC_completedBus_1.xml"),
		//TestCase(@"CompletedBus\PEV_completedBus_1.xml"),
		TestCase(@"ExemptedVehicles\exempted_completedBus_input_full.xml"),
		TestCase(@"ExemptedVehicles\exempted_completedBus_input_only_mandatory_entries.xml"),
		TestCase(@"ExemptedVehicles\exempted_heavyLorry.xml"),
		TestCase(@"ExemptedVehicles\exempted_mediumLorry.xml"),
		TestCase(@"ExemptedVehicles\exempted_primaryBus.xml"),
		]
		public void TestReadingJobVersion_V24(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"CompletedBus\Conventional_completedBus_1.xml"),
		TestCase(@"CompletedBus\HEV_completedBus_1.xml"),
		TestCase(@"CompletedBus\IEPC_completedBus_1.xml"),
		TestCase(@"CompletedBus\PEV_completedBus_1.xml"),
		]
		public void TestReadingCompletedBus_V24(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);
			Assert.NotNull(dataProvider.JobInputData.Vehicle);

			var veh = dataProvider.JobInputData.Vehicle;
			Assert.AreEqual(1, veh.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(9.5, veh.Length.Value());

			Assert.NotNull(veh.Components);
			Assert.NotNull(veh.Components.BusAuxiliaries);

			var busAux = veh.Components.BusAuxiliaries;
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration0, busAux.HVACAux.SystemConfiguration);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, busAux.HVACAux.HeatPumpTypeCoolingPassengerCompartment);

			Assert.IsTrue(busAux.ElectricConsumers.BrakelightsLED);
		}

		public IVectoRun[] ReadDeclarationJob(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);

			var fileWriter = new FileOutputWriter(filename);
			//var sumWriter = new SummaryDataContainer(fileWriter);
			//var jobContainer = new JobContainer(sumWriter);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = false;
			runsFactory.ActualModalData = false;
			runsFactory.Validate = false;

			var runs = runsFactory.SimulationRuns().ToArray();
			Assert.IsTrue(runs.Length > 0);

			return runs;
		}
	}
}