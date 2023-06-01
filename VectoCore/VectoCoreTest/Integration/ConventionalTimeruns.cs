using NUnit.Framework;
using System;
using System.IO;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Integration
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
    public class ConventionalTimeruns
    {
        [OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		private const string TRACTOR_AT_JOB = @"TestData/Integration/ConventionalTimeruns/Class5_Tractor_4x2/Class5_Tractor_ENG.vecto";

        [Category("Integration")]
        [
        TestCase(TRACTOR_AT_JOB, TestName = "Tractor AT timerun")
        ]
        public void TestTimerun(String jobFile)
        {
            var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
				ValidateComponentHashes = false
			};

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);
        }

    }
}
