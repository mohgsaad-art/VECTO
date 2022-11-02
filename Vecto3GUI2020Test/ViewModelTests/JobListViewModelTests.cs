using System.Diagnostics;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using VECTO3GUI2020.ViewModel;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using Vecto3GUI2020Test.BugReports;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class JobListViewModelTests : ViewModelTestBase
	{
		private const string finalVIF = "vecto_multistage_conventional_final_vif.VIF_Report_1.xml";

		private JobListViewModel _jobListViewModel;

		private const string _newVifCompletedConventional = "newVifCompletedConventional.vecto";
		private const string _newVifExempted = "newVifExempted.vecto";
		private const string _newVifInterimDiesel = "newVifInterimDiesel.vecto";
		private const string _newVifExemptedIncomplete = "newVifExemptedIncomplete.vecto";

		[SetUp]
		public void SetupViewModelTests()
		{
			_jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;
		}

		[Test]
		public async Task CancelSimulationWhileLoadingFiles()
		{
			var watch = new Stopwatch();
			watch.Start();
			//load final vif
			var loadedFile = await _jobListViewModel.AddJobAsync(GetTestDataPath(finalVIF)).ConfigureAwait(false);

			//select vif for simulation
			Assert.AreNotEqual(0, _jobListViewModel.Jobs.Count);
			_jobListViewModel.Jobs[0].Selected = true;


			_jobListViewModel.RunSimulationExecute();
			TestContext.Write("Canceling Simulation ... ");
			Assert.IsTrue(_jobListViewModel.SimulationRunning);
			_jobListViewModel.CancelSimulation.Execute(null);


			//Wait 
			var constraint = Is.True.After(delayInMilliseconds: 100000, pollingInterval: 100);
			Assert.That(() => _jobListViewModel.SimulationRunning == false, constraint);
			TestContext.WriteLine("Done!");

			watch.Stop();
			TestContext.WriteLine($"ExecutionTime {watch.Elapsed.TotalSeconds}s");
		}

		[TestCase(_newVifCompletedConventional, TestName = "VIFConventionalCompleted")]
		[TestCase(_newVifInterimDiesel, TestName="VIFConventionalInterim")]
		[TestCase(_newVifExempted, TestName = "VIFExempted")]
		[TestCase(_newVifExemptedIncomplete, TestName = "VIFExemptedInterim")]
		[TestCase(VIFTests.exempted_primary_vif, TestName="Exempted")]
		public async Task AddJobAsyncTest(string fileName)
		{
			var path = GetTestDataPath(fileName);
			Assert.AreEqual(0, _jobListViewModel.Jobs.Count);
			await DoAddJobAsync(path);
		}


		private async Task DoAddJobAsync(string filepath) {
			await _jobListViewModel.AddJobAsync(filepath);
			Assert.AreEqual(1, _jobListViewModel.Jobs.Count);

			Assert.AreEqual(filepath, _jobListViewModel.Jobs[0].DataSource.SourceFile);
		}

		[TestCase(true, TestName = "Exempted")]
		[TestCase(false, TestName = "NotExempted")]
		public void addNewFilesToJobList(bool exempted)
		{
			if (exempted)
			{
				_jobListViewModel.NewExemptedCompletedInputCommand.Execute(null);
			}
			else
			{
				_jobListViewModel.NewCompletedInputCommand.Execute(null);
			}

			Assert.AreEqual(1, _jobListViewModel.Jobs.Count);
		}


		[Test]
		public async Task CancelSimulationWhenJobContainerIsRunning()
		{
			var watch = new Stopwatch();
			watch.Start();

			//load final vif
			var loadedFile = await _jobListViewModel.AddJobAsync(GetTestDataPath(finalVIF)).ConfigureAwait(false);

			//select vif for simulation
			Assert.AreNotEqual(0, _jobListViewModel.Jobs.Count);

			Assert.IsTrue(_jobListViewModel.Jobs[0].CanBeSimulated);
			_jobListViewModel.Jobs[0].Selected = true;


			//Simulate for a while
			var outputVm = _kernel.Get<IOutputViewModel>(); // SINGLETON
			var simulationTask = _jobListViewModel.RunSimulationExecute();
			Assert.That(() => outputVm.Progress, Is.GreaterThanOrEqualTo(25).After(1 * 60 * 1000, 1),
				() => $"Simulation reached {outputVm.Progress}%");

			TestContext.Write("Canceling Simulation ... ");
			Assert.IsTrue(_jobListViewModel.SimulationRunning);
			_jobListViewModel.CancelSimulation.Execute(null);
			Assert.That(() => _jobListViewModel.SimulationRunning, Is.False.After(20*1000, 50) );
			TestContext.WriteLine("Done!");

			watch.Stop();
			TestContext.WriteLine($"ExecutionTime {watch.Elapsed.TotalSeconds}s");
        }

        [Test]
        public async Task LoadStageInputOnly()
        {
			var documentViewModel = await _jobListViewModel.AddJobAsync(GetTestDataPath(stageInputFullSample));
            Assert.AreEqual(typeof(StageInputViewModel), documentViewModel.GetType());

            var stageInputDocumentViewModel = documentViewModel.EditViewModel as StageInputViewModel;
            Assert.NotNull(stageInputDocumentViewModel);
		}

    }
}