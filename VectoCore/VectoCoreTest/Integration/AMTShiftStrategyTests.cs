using System;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration
{
	public class AMTShiftStrategyTests
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


		//[TestCase()]
		//public void VECTO_EffShift()
		//{
		//	var jobName =
		//		@"E:\QUAM\tmp\1a_EffShift_high-engine-rev_UD-cycle_LH-tractor_model\vecto_tractor_4x2_overdr_EffShift-def.vecto";
		//	RunJob_DeclSingle(jobName, 9);
		//}

        public void RunJob_DeclSingle(string jobName, int runIdx)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToArray();
			
			runs[runIdx].Run();

			Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);
		}


		public void RunJob_DeclAll(string jobName)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), String.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

	}
}
