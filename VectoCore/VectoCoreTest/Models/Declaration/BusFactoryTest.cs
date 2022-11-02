using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class BusFactoryTest
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

		[
			TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_nonSmart.xml", 0, TestName = "Run Primary Bus NonSmart HeavyUrban Low"),
			TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_SmartPS.xml", -1, TestName = "Run Primary Bus SmartPS ALL"),

			TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_SmartPS.xml", 0, TestName = "Run Primary Bus SmartPS HeavyUrban Low"),
			TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_SmartPS.xml", 1, TestName = "Run Primary Bus SmartPS HeavyUrban Ref"),
			TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_SmartPS.xml", 13, TestName = "Run Primary Bus SmartPS InterUrban Ref"),

			TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_nonSmart.xml", -1, TestName = "Run Primary Bus NonSmart ALL"),
			TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_SmartES.xml", -1, TestName = "Run Primary Bus SmartES ALL"),
			TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_nonSmart_ESS.xml", -1, TestName = "Run Primary Bus NonSmart ESS ALL"),

		TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_nonSmart.xml", 4, TestName = "Run Primary Bus NonSmart SubUrban Low"),
		TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_SmartPS.xml", 4, TestName = "Run Primary Bus SmartPS SubUrban Low"),
		TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_SmartES.xml", 4, TestName = "Run Primary Bus SmartES SubUrban Low"),
		TestCase(@"TestData\Integration\Buses\vecto_vehicle-primary_heavyBus_nonSmart_ESS.xml", 4, TestName = "Run Primary Bus NonSmart ESS SubUrban Low"),
			]
		public void CreateRunDataPrimaryBus(string jobFile, int runIdx)
		{
			var writer = new FileOutputWriter(jobFile);
			var inputData = Path.GetExtension(jobFile) == ".xml"
				? xmlInputReader.CreateDeclaration(jobFile)
				//? new XMLDeclarationInputDataProvider(relativeJobPath, true)
				: JSONInputDataFactory.ReadJsonJob(jobFile);
			var sumContainer = new SummaryDataContainer(writer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			factory.SumData = sumContainer; //ActualModalData = true,
			factory.Validate = false;
			var jobContainer = new JobContainer(sumContainer);

			
			if (runIdx >= 0) {
				var runs = factory.SimulationRuns().ToArray();
				jobContainer.AddRun(runs[runIdx]);
			} else {
				jobContainer.AddRuns(factory);
			}

			jobContainer.Execute();
			jobContainer.WaitFinished();
			

			Assert.IsTrue(jobContainer.AllCompleted);
			Assert.IsTrue(jobContainer.Runs.All(x => x.Success));
		}

		[TestCase()]
		public void CreateRunSingleBus()
		{
			var jobFile =
				@"TestData\Integration\Buses\SingleBus.vecto";
			var writer = new FileOutputWriter(jobFile);
			var inputData = Path.GetExtension(jobFile) == ".xml"
				? xmlInputReader.CreateDeclaration(jobFile)
				//? new XMLDeclarationInputDataProvider(relativeJobPath, true)
				: JSONInputDataFactory.ReadJsonJob(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;

			var jobContainer = new JobContainer(new SummaryDataContainer(writer));
		
			//var runs = factory.SimulationRuns().ToArray();

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.AllCompleted);
			Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
			//jobContainer.AddRun(runs[runIdx]);
			//runs[runIdx].Run();
			//Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);
		}
	}
}
