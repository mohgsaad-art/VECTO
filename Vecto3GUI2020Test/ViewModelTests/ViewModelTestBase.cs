using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Ninject;
using VECTO3GUI2020.Ninject.Vehicle;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace Vecto3GUI2020Test
{
	public class ViewModelTestBase
	{
		protected string TestDataFullPath;
		protected string SourceDirectoryRoot;
		protected string prevDirectory;
		protected const string consolidated_multiple_stages = "vecto_multistage_consolidated_multiple_stages.xml";
		protected const string consolidated_multiple_stages_airdrag = "vecto_multistage_consolidated_multiple_stages_airdrag.xml";
		protected const string consolidated_multiple_stages_hev = "vecto_multistage_consolidated_multiple_stages_hev.xml";
		protected const string consolidated_one_stage = "vecto_multistage_consolidated_one_stage.xml";
		protected const string primary_vehicle_only = "vecto_multistage_primary_vehicle_only.xml";
		protected const string exempted_primary_vif = "exempted_primary_heavyBus.VIF.xml";
		protected const string stageInputFullSample = "vecto_vehicle-stage_input_full-sample.xml";
		protected const string airdragLoadTestFile = "AirdragLoadTestFile.xml";
		protected const string airdragLoadTestFilev2 = "AirdragLoadTestFilev2.xml";

		//protected IXMLInputDataReader xmlInputReader;
		protected IKernel _kernel;
		private Mock<IDialogHelper> _mockDialogHelper;


		protected TestHelper _testHelper;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			prevDirectory = Environment.CurrentDirectory;
			SourceDirectoryRoot = Directory.GetParent(prevDirectory).Parent.Parent.FullName;
			TestDataFullPath = Path.Combine(SourceDirectoryRoot + "\\Testdata\\");
		}



		[SetUp]
		public void SetUp()
		{
			_kernel = new StandardKernel(
				new VectoNinjectModule(),
				new JobEditModule(),
				new ComponentModule(),
				new DocumentModule(),
				new XMLWriterFactoryModule(),
				new FactoryModule(),
				new MultistageModule(),
				new Vecto3GUI2020Module()
			);
			//xmlInputReader = _kernel.Get<IXMLInputDataReader>();
			_kernel.Rebind<IDialogHelper>().ToConstant(SetMockDialogHelper().Object);
			_kernel.Rebind<IWindowHelper>().ToConstant(GetMockWindowHelper());
			_testHelper = new TestHelper(_kernel.Get<IXMLInputDataReader>());


			SetOutputDirectory();
		}

		private void SetOutputDirectory()
		{

			prevDirectory = Environment.CurrentDirectory;
			SourceDirectoryRoot = Directory.GetParent(prevDirectory).Parent.Parent.FullName;
			TestDataFullPath = Path.GetFullPath(@"Testdata\");


			var className = TestContext.CurrentContext.Test.ClassName.Replace("Vecto3GUI2020Test.", "");
			var testName = TestContext.CurrentContext.Test.Name;
			var invalidPathChars = Path.GetInvalidPathChars();
			foreach (var invalidPathChar in invalidPathChars) {
				testName = testName.Replace(invalidPathChar, '_');
			}

			var testOutputDirPath = Path.Combine(SourceDirectoryRoot + @"\Testdata\output\" + className + "\\" + testName);

			//Create output directory

			if (Directory.Exists(testOutputDirPath)) {
				Directory.Delete(testOutputDirPath, true);
			}
			Directory.CreateDirectory(testOutputDirPath);
			_kernel.Get<ISettingsViewModel>().DefaultOutputPath = testOutputDirPath;
			Directory.SetCurrentDirectory(testOutputDirPath);

			//var currentContext = TestContext.CurrentContext;

			//var outputPath = Path.GetFullPath(TestDataDirPath + CurrentTestOutputPath);
			//TestContext.CurrentContext.Test.Name = currentContext.
			//var SettingsVm = _kernel.Get<ISettingsViewModel>();
			//SettingsVm.DefaultOutputPath = 
		}

		protected string GetFullPath(string fileName)
		{
			return Path.GetFullPath(fileName);
		}

		private IWindowHelper GetMockWindowHelper()
		{
			Mock<IWindowHelper> mockWindowHelper = new Mock<IWindowHelper>();
			mockWindowHelper.Setup(windowHelper => windowHelper.ShowWindow(It.IsAny<object>()))
				.Callback<object>((obj) => WriteLine($"Window containing {obj.GetType().ToString()} was opened"));

			return mockWindowHelper.Object;
		}


		[TearDown]
		public void TearDown()
		{
			_kernel.Dispose();
			_kernel = null;

			Directory.SetCurrentDirectory(prevDirectory);
		}

		public bool checkFileNameExists(string fileName)
		{
			var filePath = Path.GetFullPath(fileName);
			return checkFilePathExists(filePath);
		}

		public bool checkFilePathExists(string filePath)
		{
			var exists = File.Exists(filePath);
			if (exists)
			{
				Console.WriteLine(filePath + @" exists");
			}
			else
			{
				Console.WriteLine(filePath + @" not existing");
			}

			return exists;
		}

		public void deleteFile(string fileName)
		{
			var filePath = Path.GetFullPath(fileName);
			File.Delete(fileName);
		}

		public virtual NewMultiStageJobViewModel LoadFileFromPath(string filePath)
		{
			var newMultistageJobViewModel = _kernel.Get<NewMultiStageJobViewModel>();

			WriteLine("Loading" + filePath);
			newMultistageJobViewModel.AddVifFile(filePath);


			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);

			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(manstageVehicleViewModel);

			//Assert.AreEqual(GetTestDataPath(fileName), newMultistageJobViewModel.VifPath);

			if (!manstageVehicleViewModel.ExemptedVehicle)
			{
				var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
				Assert.NotNull(auxiliariesViewModel);

				var airdragViewModel = (manstageVehicleViewModel as InterimStageBusVehicleViewModel_v2_8)?.MultistageAirdragViewModel;
				Assert.NotNull(airdragViewModel);
			}
			return newMultistageJobViewModel;
		}

		public virtual NewMultiStageJobViewModel LoadFileFromTestDirectory(string fileName)
		{
			var filePath = GetTestDataPath(fileName);

			return LoadFileFromPath(filePath);
		}


		protected virtual Mock<IDialogHelper> SetMockDialogHelper(string fileToLoad = null, string fileToSave = null)
		{
			if (_mockDialogHelper == null) {
				_mockDialogHelper = new Mock<IDialogHelper>();
				_mockDialogHelper.Setup(dialogHelper => dialogHelper.ShowMessageBox(It.IsAny<string>(),
						It.IsAny<string>(),
						It.IsAny<MessageBoxButton>(),
						It.IsAny<MessageBoxImage>())).Returns(MessageBoxResult.OK)
					.Callback<string, string, MessageBoxButton, MessageBoxImage>((
						(message, caption, button, image) => {
							TestContext.WriteLine($"{caption}\n {message}");
						}));

				_mockDialogHelper.Setup(dialogHelper =>
						dialogHelper.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>()))
					.Callback<string, string>((message, caption) => 
						TestContext.WriteLine($"{{caption}}\n {message}"));

				_mockDialogHelper.Setup(dialogHelper =>
						dialogHelper.ShowErrorMessage(It.IsAny<string>(), It.IsAny<string>()))
					.Callback<string, string>((message, caption) =>
						TestContext.WriteLine($"{{caption}}\n {message}"));

				_mockDialogHelper.Setup(dialogHelper =>
						dialogHelper.ShowErrorMessage(It.IsAny<string>()))
					.Callback<string>((message) =>
						TestContext.WriteLine($"{{Error}}\n {message}"));


			}
			if (fileToLoad != null) {
				var filePath = fileToLoad;

				Assert.NotNull(filePath);
				_mockDialogHelper.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog(It.IsAny<string>())).Returns(filePath);
				_mockDialogHelper.Setup(dialogHelper => dialogHelper.OpenXMLFileDialog()).Returns(filePath);

				TestContext.WriteLine($"Created MOCKDIALOGHELPER, returns {filePath} for OpenXMLFileDialog()");
			}

			if (fileToSave != null) {
				var filePath = fileToLoad;
				_mockDialogHelper.Setup(dialogHelper =>
					dialogHelper.SaveToXMLDialog(It.IsAny<string>())).Returns(filePath);
				_mockDialogHelper.Setup(dialogHelper =>
					dialogHelper.SaveToXMLDialog(null)).Returns(filePath);

				TestContext.WriteLine($"Created MOCKDIALOGHELPER, returns {filePath} for SaveToXMLFileDialog()");
			}


			return _mockDialogHelper;
		}

		protected Mock<IDialogHelper> GetMockDialogHelper()
		{
			return _mockDialogHelper;
		}

		protected virtual string GetTestDataPath(string fileName)
		{
			var path = Path.Combine(TestDataFullPath + fileName);
			return path;
		}

		protected void Write(string outputMessage)
		{
			TestContext.Write(outputMessage);
		}
		protected void WriteLine(string outputMessage)
		{
			TestContext.WriteLine(outputMessage);
		}
	}
}