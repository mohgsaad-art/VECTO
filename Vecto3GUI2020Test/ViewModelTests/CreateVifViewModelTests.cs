using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Ninject;
using NUnit.Framework;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class CreateVifViewModelTests : ViewModelTestBase
	{
		private ICreateVifViewModel _createVifViewModel;

		private const string testdata_2_4 = "XML\\XMLReaderDeclaration\\SchemaVersion2.4\\";
		//private const string testdata_2_10 = "XML\\XMLReaderDeclaration\\SchemaVersion2.10\\";

		private const string vecto_vehicle_primary_heavyBusSample =
			testdata_2_4 + "vecto_vehicle-primary_heavyBus-sample.xml";

		private const string vecto_vehicle_exempted_input_only_certain_entries =
			"vecto_vehicle-exempted_input_only_certain_entries01-sample.xml";

		private const string vecto_vehicle_primary_heavyBusExempted = testdata_2_4 + "exempted_primary_heavyBus.xml";

		[SetUp]
		public void SetUpCreateVif()
		{
			_createVifViewModel = _kernel.Get<ICreateVifViewModel>();
		}

		[TestCase(stageInputFullSample, TestName="InvalidPrimaryFile_StageInput")]
		[TestCase(airdragLoadTestFile, TestName="InvalidPrimaryFile_Airdrag")]
		[TestCase(consolidated_multiple_stages_airdrag, TestName = "InvalidPrimaryFile_VIF")]
		[TestCase(vecto_vehicle_exempted_input_only_certain_entries, TestName="InvalidPrimaryFile_ExemptedStageInput")]
		public void LoadInvalidPrimaryFile(string fileName)
		{
			var filePath = GetTestDataPath(fileName);
			Assert.IsFalse(_createVifViewModel.LoadPrimaryInput(filePath));
			Assert.IsNull(_createVifViewModel.PrimaryInputPath);
			Assert.IsNull(_createVifViewModel.IsPrimaryExempted);
		}


		[TestCase(vecto_vehicle_primary_heavyBusSample, TestName = "InvalidStageInput_Primary")]
		[TestCase(airdragLoadTestFile, TestName = "InvalidStageInput_Airdrag")]
		[TestCase(consolidated_multiple_stages_airdrag, TestName = "InvalidStageFile_VIF")]
		public void LoadInvalidCompletedFile(string fileName)
		{
			var filePath = GetTestDataPath(fileName);
			Assert.IsFalse(_createVifViewModel.LoadStageInput(filePath));
			Assert.IsNull(_createVifViewModel.IsStageInputExempted);
			Assert.IsNull(_createVifViewModel.StageInputPath);
		}


		[Test]
		public void SaveFile()
		{
			LoadValidNonExemptedFiles();
			var outputFile = "test.json";
			var outputPath = GetFullPath(outputFile);
			_createVifViewModel.SaveJob(outputPath);

			FileAssert.Exists(outputPath);

			Assert.AreEqual(Path.GetDirectoryName(outputPath), _createVifViewModel.DataSource.SourcePath);
			Assert.AreEqual(outputPath, _createVifViewModel.DataSource.SourceFile);
		}


		[Test]
		public void LoadNonExemptedCompletedAndExemptedPrimary()
		{
			var exemptedPrimaryPath = GetTestDataPath(vecto_vehicle_primary_heavyBusExempted);
			var stageInputPath = GetTestDataPath(stageInputFullSample);

			Assert.IsTrue(_createVifViewModel.LoadPrimaryInput(exemptedPrimaryPath));
			Assert.IsFalse(_createVifViewModel.LoadStageInput(stageInputPath));
		}

		

		[Test]
		public void LoadValidPrimaryFile()
		{
			var filePath = GetTestDataPath(vecto_vehicle_primary_heavyBusSample);
			Assert.IsTrue(_createVifViewModel.LoadPrimaryInput(filePath));
			Assert.AreEqual(filePath, _createVifViewModel.PrimaryInputPath);
		}

		[TestCase(stageInputFullSample, TestName = "ValidStageInput_fullStageInput")]
		[TestCase(vecto_vehicle_exempted_input_only_certain_entries, TestName = "ValidStageInput_exemptedStageInput")]
		public void LoadValidStageInputFile(string fileName)
		{
			var filePath = GetTestDataPath(fileName);
			Assert.IsTrue(_createVifViewModel.LoadStageInput(filePath));
			Assert.AreEqual(filePath, _createVifViewModel.StageInputPath);
		}

		[Test]
		public void LoadValidNonExemptedFilesTest()
		{
			LoadValidNonExemptedFiles();
		}


		public void LoadValidNonExemptedFiles()
		{
			var primaryPath = GetTestDataPath(vecto_vehicle_primary_heavyBusSample);
			var stageInputPath = GetTestDataPath(stageInputFullSample);

			Assert.IsTrue(_createVifViewModel.LoadPrimaryInput(primaryPath));
			Assert.IsTrue(_createVifViewModel.LoadStageInput(stageInputPath));
		}

		[Test]
		public void LoadValidExemptedFiles()
		{
			var primaryPath = GetTestDataPath(vecto_vehicle_primary_heavyBusExempted);
			var stageInputPath = GetTestDataPath(vecto_vehicle_exempted_input_only_certain_entries);
			Assert.IsTrue(_createVifViewModel.LoadPrimaryInput(primaryPath));
			Assert.IsTrue(_createVifViewModel.LoadStageInput(stageInputPath));

		}

		[Test]
		public void RemoveInputs()
		{
			LoadValidNonExemptedFiles();
			Assert.NotNull(_createVifViewModel.PrimaryInputPath);
			Assert.NotNull(_createVifViewModel.StageInputPath);

			bool removePrimaryNotified = false;
			bool removeStageInputNotified = false;
			_createVifViewModel.RemovePrimaryCommand.CanExecuteChanged +=
				((sender, args) => removePrimaryNotified = true);
			_createVifViewModel.RemoveStageInputCommand.CanExecuteChanged +=
				((sender, args) => removeStageInputNotified = true);


			//Remove Primary
			Assert.IsTrue(_createVifViewModel.RemovePrimaryCommand.CanExecute(null));

			_createVifViewModel.RemovePrimaryCommand.Execute(null);
			Assert.IsTrue(removePrimaryNotified);
			removePrimaryNotified = false;

			Assert.IsFalse(_createVifViewModel.RemovePrimaryCommand.CanExecute(null));
			Assert.IsNull(_createVifViewModel.PrimaryInputPath);
			Assert.IsNull(_createVifViewModel.IsPrimaryExempted);


			//Remove Stage Input
			Assert.IsTrue(_createVifViewModel.RemoveStageInputCommand.CanExecute(null));

			_createVifViewModel.RemoveStageInputCommand.Execute(null);
			Assert.IsTrue(removeStageInputNotified);
			removeStageInputNotified = false;

			Assert.IsFalse(_createVifViewModel.RemovePrimaryCommand.CanExecute(null));
			Assert.IsNull(_createVifViewModel.PrimaryInputPath);
			Assert.IsNull(_createVifViewModel.IsPrimaryExempted);
		}


		[Test]
		public async Task SaveAndReload()
		{
			LoadValidNonExemptedFiles();
			var primaryPath = _createVifViewModel.PrimaryInputPath;
			var stagePath = _createVifViewModel.StageInputPath;
			

			var savedToPath = _createVifViewModel.SaveJob(GetFullPath("non_exempted.vecto"));
			WriteLine($"Saved to: {savedToPath}");

			Assert.AreEqual(primaryPath, _createVifViewModel.PrimaryInputPath);
			Assert.AreEqual(stagePath, _createVifViewModel.StageInputPath);

			var jobListVm = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			var loadedDoc = await jobListVm.AddJobAsync(savedToPath) as CreateVifViewModel;

			Assert.AreEqual(stagePath, loadedDoc.StageInputPath);
			Assert.AreEqual(primaryPath, loadedDoc.PrimaryInputPath);




		}
	}
}