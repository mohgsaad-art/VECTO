using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData;
using System.Data;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Tests.Integration
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
    public class Auxiliaries
    {
        [OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

        private const string TRACTOR_AT_JOB = @"TestData/Integration/ConventionalTimeruns/Class5_Tractor_4x2/Class5_Tractor_ENG_Aux.vecto";

        [Category("Integration")]
        [
        TestCase(TRACTOR_AT_JOB, TestName = "Tractor AT distance Padd cycle")
        ]
        public void TestAuxiliaryLoadFromCycle(String jobFile)
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
			
			var modData = (jobContainer.Runs[0].Run.GetContainer().ModalData as ModalDataContainer).Data;
			
			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);

			var row = modData.Rows.Cast<DataRow>().First();
			Assert.AreEqual(9330.SI<Watt>(), row[ModalResultField.P_aux.GetName()]);
        }
    }
}
