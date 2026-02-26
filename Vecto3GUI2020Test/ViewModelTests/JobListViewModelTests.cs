using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Implementation.Document;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using Vecto3GUI2020Test.Utils;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class JobListViewModelTests// : ViewModelTestBase
	{
		private IKernel _kernel;
		private JobListViewModel _jobListViewModel;
		private MockDialogHelper _dialogHelper;
		private MockWindowHelper _mockWindowHelper;


		[SetUp]
		public void SetupViewModelTests()
		{
			_kernel = TestHelper.GetKernel();
			_kernel.Rebind<IDialogHelper>().To<MockDialogHelper>().InSingletonScope();
			_kernel.Rebind<IWindowHelper>().To<MockWindowHelper>().InSingletonScope();
			_jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			_dialogHelper = _kernel.Get<IDialogHelper>() as MockDialogHelper;
			_mockWindowHelper = _kernel.Get<IWindowHelper>() as MockWindowHelper;

		}

		[Test]
		public async Task CancelSimulationWhileLoadingFiles()
		{
			var watch = new Stopwatch();
			watch.Start();
			//load final vif
			var finalVifPath = Path.GetFullPath(TestData.FinalVif);
			AssertHelper.FileExists(finalVifPath);
			var loadedFile = await _jobListViewModel.AddJobAsync(finalVifPath).ConfigureAwait(false);
			_dialogHelper.AssertNoErrorDialogs();
			//select vif for simulation
			Assert.AreNotEqual(0, _jobListViewModel.Jobs.Count);
			_jobListViewModel.Jobs[0].Selected = true;

			await _jobListViewModel.RunSimulationExecute();
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

		[TestCase(TestData.NewVifCompletedConventional, TestName = "VIFConventionalCompleted")]
		[TestCase(TestData.NewVifInterimDiesel, TestName="VIFConventionalInterim")]
		[TestCase(TestData.NewVifExempted, TestName = "VIFExempted")]
		[TestCase(TestData.NewVifExemptedIncomplete, TestName = "VIFExemptedInterim")]
		public async Task AddJobAsyncTest(string fileName)
		{
			AssertHelper.FileExists(Path.GetFullPath(fileName));
			await DoAddJobAsync(Path.GetFullPath(fileName));
			Assert.That(_jobListViewModel.Jobs.Count == 1);
			_dialogHelper.AssertNoErrorDialogs();
		}

		
		[TestCase("TestData/XML/SchemaVersionMultistage.0.1/vecto_multistage_consolidated_multiple_stages.xml")]
		public async Task AddVIF(string fileName)
		{
			var documentViewModel = await DoAddJobAsync(Path.GetFullPath(fileName));
			_dialogHelper.AssertNoErrorDialogs();
			Assert.That(documentViewModel is MultiStageJobViewModel_v0_1);
			
		}
		[TestCase(@"TestData\XML\vecto_vehicle-stage_input_full-sample.xml")]
		public async Task AddStepInput(string fileName)
		{
			var documentViewModel = await DoAddJobAsync(Path.GetFullPath(fileName));
			Assert.That(documentViewModel is StageInputViewModel, $"expected {typeof(StageInputViewModel)} got {documentViewModel.GetType()}");
        }

		[TestCase(@"TestData\Case1\special_case_1.vecto", "Special Case I")]
		[TestCase(@"TestData\Case2\special_case_2.vecto", "Special Case II")]
        public async Task AddNewVIFJob(string fileName, string specialCase)
		{
			var documentViewModel = await DoAddJobAsync(Path.GetFullPath(fileName));
			var createVifViewModel = documentViewModel as CreateVifViewModel;
			Assert.That(createVifViewModel != null);

			Assert.That(_jobListViewModel.Jobs.Count == 1);
			switch (specialCase) {
				case "Special Case I":
					Assert.False(createVifViewModel.RunSimulation);
			

					break;
				case "Special Case II":
					Assert.True(createVifViewModel.RunSimulation);
				

					break;
				default:
					Assert.Fail();
					break;
			}
		}


		#region New Files
		[Test, Description("File -> Create Interim/Completed Input")]
		public void CreateStepInput()
		{
			_jobListViewModel.NewCompletedInputCommand.Execute(null);
			Assert.That(!_jobListViewModel.Jobs[0].DocumentName.Contains("exempted"));
            Assert.That(_jobListViewModel.Jobs.Count == 1);
		}

		[Test, Description("File -> Create Exempted Interim/Completed Input")]
		[Ignore("Command removed")]
		public void CreateExemptedStepInput()
		{
			_jobListViewModel.NewExemptedCompletedInputCommand.Execute(null);
			Assert.That(_jobListViewModel.Jobs[0].DocumentName.Contains("exempted"));
			Assert.That(_jobListViewModel.Jobs.Count == 1);
		}

		[Test, Description("File -> New Completed Job")]
		public void CreateSpecialCaseCompletedVIF()
		{
			_jobListViewModel.NewVifCommand.Execute(true);
			Assert.That(_jobListViewModel.Jobs.Count == 1);
		}

		[Test, Description("File -> New Primary job with Interim Input")]
		public void CreateSpecialCaseVIF()
		{
			_jobListViewModel.NewVifCommand.Execute(false);
			Assert.That(_jobListViewModel.Jobs.Count == 1);
        }

		[Test, Description("File -> New Interim/Completed Job")]
		public void CreateStep()
		{
			_jobListViewModel.NewManufacturingStageFileCommand.Execute(null);
			var openedViewModel = _mockWindowHelper.Windows[0] as NewMultiStageJobViewModel;
			Assert.That(openedViewModel != null);
			
			//Assert.That(_jobListViewModel.Jobs.Count == 1);
		}
		#endregion

		[TestCase(TestData.PrimaryHeavybusSampleXML)]
		[TestCase(TestData.HeavylorryHevSHeavylorryS3XML)]
        public async Task AddSimulationOnlyJob(string fileName)
		{
			var documentViewModel = await DoAddJobAsync(Path.GetFullPath(fileName));
			Assert.That(documentViewModel is SimulationOnlyDeclarationJob);
			_dialogHelper.AssertNoErrorDialogs();
		}


		private async Task<IDocumentViewModel> DoAddJobAsync(string filepath) {
			AssertHelper.FileExists(filepath);
			Assert.AreEqual(0, _jobListViewModel.Jobs.Count);
            var documentViewModel = await _jobListViewModel.AddJobAsync(filepath);
			Assert.AreEqual(1, _jobListViewModel.Jobs.Count);

			Assert.AreEqual(filepath, _jobListViewModel.Jobs[0].DataSource.SourceFile);
			return documentViewModel;
		}

		[TestCase(true, TestName = "Exempted")]
		public void addNewFilesToJobList(bool exempted)
		{
			_jobListViewModel.NewCompletedInputCommand.Execute(null);
			Assert.AreEqual(1, _jobListViewModel.Jobs.Count);
		}


		[Test]
		public async Task CancelSimulationWhenJobContainerIsRunning()
		{
			var watch = new Stopwatch();
			watch.Start();

			//load final vif
			var loadedFile = await _jobListViewModel.AddJobAsync(Path.GetFullPath(TestData.FinalVif)).ConfigureAwait(false);

			_dialogHelper.AssertNoErrorDialogs();
			//select vif for simulation
			Assert.AreNotEqual(0, _jobListViewModel.Jobs.Count);

			Assert.IsTrue(_jobListViewModel.Jobs[0].CanBeSimulated);
			_jobListViewModel.Jobs[0].Selected = true;


			//Simulate for a while
			var outputVm = _kernel.Get<IOutputViewModel>(); // SINGLETON
			var simulationTask = _jobListViewModel.RunSimulationExecute();
			//Wait until simulation reached 2%
			Assert.IsTrue(_jobListViewModel.SimulationRunning);
			while (outputVm.Progress < 4 && (!outputVm.StatusMessage?.Contains("finished") ?? false)) {
				//DO nothing
			}


            // Assert.That(() => outputVm.Progress, Is.GreaterThanOrEqualTo(25).After(1 * 60 * 1000, 1),
            // 	() => $"Simulation reached {outputVm.Progress}%");
			//_dialogHelper.AssertNoErrorDialogs();
            TestContext.Write("Canceling Simulation ... ");
			Assert.IsTrue(_jobListViewModel.SimulationRunning);
			_jobListViewModel.CancelSimulation.Execute(null);
			Assert.That(() => _jobListViewModel.SimulationRunning, Is.False.After(20*1000, 50) );
			TestContext.WriteLine("Done!");

			watch.Stop();
			TestContext.WriteLine($"ExecutionTime {watch.Elapsed.TotalSeconds}s");
        }

       
  //      public async Task LoadStageInputOnly(string file)
		//{
		//	var path = GetTestDataPath(file);
		//	_dialogHelper.AssertNoErrorDialogs();
  //          AssertHelper.FileExists(path);
		
		//	var documentViewModel = await _jobListViewModel.AddJobAsync(path);
  //          Assert.AreEqual(typeof(StageInputViewModel), documentViewModel.GetType());

  //          var stageInputDocumentViewModel = documentViewModel.EditViewModel as StageInputViewModel;
  //          Assert.NotNull(stageInputDocumentViewModel);
		//}

    }
}