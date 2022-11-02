using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.DualFuel
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DualFuelTests
	{
		private StandardKernel _kernel;
		private IXMLInputDataReader xmlInputReader;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.3\vehicle_sampleDualModeDualFuel.xml")]
		public void DualModeDualFuelVehicleTest(string jobName)
		{
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = xmlInputReader.CreateDeclaration(jobName);

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			runsFactory.WriteModalResults = true;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.AllCompleted);
			Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.3\vehicle_sampleSingleModeDualFuel.xml")]
		public void SingleModeDualFuelVehicleTest(string jobName)
		{
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = xmlInputReader.CreateDeclaration(jobName);

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			runsFactory.WriteModalResults = true;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.AllCompleted);
			Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
		}
	}
}
