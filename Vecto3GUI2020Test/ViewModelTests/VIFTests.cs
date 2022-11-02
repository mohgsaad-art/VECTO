using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using Moq;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class VIFTests : ViewModelTestBase
	{

		public const string _finalVif = "vecto_multistage_conventional_final_vif.VIF_Report_1.xml";
		public const string _vectoMultistageOneStage = "vecto_multistage_consolidated_one_stage.xml";

		[Test]
		public void loadPrimaryVehicleOnlyAndCreateNewVIF()
		{
			var multistagevm = LoadFileFromTestDirectory(primary_vehicle_only).MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var stage = multistagevm.ManufacturingStageViewModel.StepCount;

			Assert.AreEqual(2, stage);

			//Set Mandatory Fields
			var vehicle =
				multistagevm.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;
			vehicle.ManufacturerAddress = "Address";
			vehicle.Manufacturer = "Manufacturer";
			vehicle.VIN = "VIN12345678";
			
			var writer = GetFileOutputVIFWriter(multistagevm);
			
			deleteFile(writer.XMLMultistageReportFileName);
			SetMockDialogHelper(null, writer.XMLMultistageReportFileName);
			_kernel.Rebind<IDialogHelper>().ToConstant(SetMockDialogHelper(null, writer.XMLMultistageReportFileName).Object);

			multistagevm.SaveVif(multistagevm, writer);

			Assert.IsTrue(File.Exists(writer.XMLMultistageReportFileName));

			var validator = new XMLValidator(XmlReader.Create(writer.XMLMultistageReportFileName));
			Assert.True(validator.ValidateXML(TUGraz.VectoCore.Utils.XmlDocumentType.MultistepOutputData));
		}

		[Test, Combinatorial]
		public void CreateVifAllParameters(
			[Values("manufacturer")] string manufacturer,
			[Values(LegislativeClass.M3)] LegislativeClass legCategory)
		{
			var multistagevm = LoadFileFromTestDirectory(primary_vehicle_only).MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var stage = multistagevm.ManufacturingStageViewModel.StepCount;

			Assert.AreEqual(2, stage);

			//Set Necessary Fields
			var vehicleVm =
				multistagevm.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;

			vehicleVm.Manufacturer = "adsf";
			vehicleVm.ManufacturerAddress = "asdf 123";
			vehicleVm.VIN = "1234567890";
			vehicleVm.Model = "Model";
			vehicleVm.LegislativeClass = LegislativeClass.M3;
			vehicleVm.CurbMassChassis = Kilogram.Create(20000);
			vehicleVm.GrossVehicleMassRating = Kilogram.Create(20000);
			vehicleVm.RegisteredClass = RegistrationClass.I_II;
			vehicleVm.VehicleCode = VehicleCode.CC;
			vehicleVm.LowEntry = true;
			vehicleVm.Height = Meter.Create(2.6);
			vehicleVm.NumberPassengerSeatsUpperDeck = 2;
			vehicleVm.NumberPassengersStandingLowerDeck = 13;
			vehicleVm.NumberPassengerSeatsLowerDeck = 10;
			vehicleVm.NumberPassengersStandingUpperDeck = 12;

			//SETADAS
			vehicleVm.EngineStopStartNullable = true;
			vehicleVm.EcoRollTypeNullable = EcoRollType.WithEngineStop;
			vehicleVm.PredictiveCruiseControlNullable = PredictiveCruiseControlType.Option_1_2_3;
			vehicleVm.ATEcoRollReleaseLockupClutch = false;



			//SETAUxiliaries
			var auxVm = vehicleVm.MultistageAuxiliariesViewModel as MultistageAuxiliariesViewModel;


			auxVm.InteriorLightsLED = true;
			auxVm.DayrunninglightsLED = false;
			auxVm.PositionlightsLED = false;
			auxVm.BrakelightsLED = true;
			auxVm.HeadlightsLED = false;
			auxVm.SystemConfiguration = BusHVACSystemConfiguration.Configuration2;
			auxVm.HeatPumpTypeCoolingDriverCompartment = HeatPumpType.non_R_744_3_stage;
			auxVm.HeatPumpTypeCoolingPassengerCompartment = HeatPumpType.non_R_744_4_stage;
			auxVm.HeatPumpTypeHeatingDriverCompartment = HeatPumpType.non_R_744_2_stage;
			auxVm.HeatPumpTypeHeatingPassengerCompartment = HeatPumpType.non_R_744_continuous;
			auxVm.AuxHeaterPower = SIBase<Watt>.Create(50);
			auxVm.DoubleGlazing = true;
			auxVm.AdjustableAuxiliaryHeater = false;
			auxVm.SeparateAirDistributionDucts = false;

			var resultFile = multistagevm.SaveVif(GetFullPath(
				"completed_final" + ".xml"));


			var jobListVm = _kernel.Get<IJobListViewModel>();
			Assert.That(() => jobListVm.Jobs.Count, Is.EqualTo(2));

		}

		[TestCase(true, 1, TestName="With Airdrag Component v1")]
		[TestCase(true, 2, TestName="With Airdrag Component v2")]
		[TestCase(false, 0, TestName="Without Airdrag Component")]
		public void CreateCompletedFinalVIFWithAirdrag(bool loadAirdrag, int airdragVersion)
		{
			GetMockDialogHelper().Setup(dialogHelper => dialogHelper.ShowMessageBox(It.IsAny<string>(),
				It.IsAny<string>(), MessageBoxButton.YesNo, It.IsAny<MessageBoxImage>())).Returns(MessageBoxResult.No);

			var multistagevm = LoadFileFromTestDirectory(_finalVif);

			var VehicleViewModel = multistagevm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;

			VehicleViewModel.Manufacturer = "Manufacturer";
            VehicleViewModel.ManufacturerAddress = "Manufacturer Address";
			VehicleViewModel.VIN = "1234567";
			VehicleViewModel.Model = "asdf";
			VehicleViewModel.AirdragModifiedEnum = loadAirdrag ? AIRDRAGMODIFIED.TRUE: AIRDRAGMODIFIED.FALSE;
			VehicleViewModel.VehicleDeclarationType = VehicleDeclarationType.final;
			VehicleViewModel.VehicleTypeApprovalNumber = "123456789";
			//SETADAS
			VehicleViewModel.EngineStopStartNullable = true;
			VehicleViewModel.EcoRollTypeNullable = EcoRollType.WithEngineStop;
			VehicleViewModel.PredictiveCruiseControlNullable = PredictiveCruiseControlType.Option_1_2_3;
			VehicleViewModel.ATEcoRollReleaseLockupClutch = false;


			if (loadAirdrag) {
				var airdragTestFile = airdragVersion == 2 ? airdragLoadTestFilev2 : airdragLoadTestFile;
				Assert.IsTrue(VehicleViewModel.MultistageAirdragViewModel.LoadAirdragFile(GetTestDataPath(airdragTestFile)));
			}
		
			var resultFile = multistagevm.MultiStageJobViewModel.SaveVif(GetFullPath(
				"completed_final" + ".xml"));

			
			var jobListVm = _kernel.Get<IJobListViewModel>();
			Assert.That(() => jobListVm.Jobs.Count, Is.EqualTo(2));

			Assert.IsTrue(jobListVm.Jobs[1].CanBeSimulated, String.Join("\n",((AdditionalJobInfoViewModelMultiStage) jobListVm.Jobs[1].AdditionalJobInfoVm).InvalidEntries));
		}

		[Test]
		public void CreateCompletedExemptedVif()
		{
			var multistagevm = LoadFileFromTestDirectory(exempted_primary_vif).MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var jobListVm = _kernel.Get<IJobListViewModel>();

			var vehicleVm =
				multistagevm.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;

			Assert.IsTrue(vehicleVm.ExemptedVehicle);
			vehicleVm.Manufacturer = "Test Manufacturer 1";
			vehicleVm.ManufacturerAddress = "Address";
			vehicleVm.VIN = "123456789";
			vehicleVm.Model = "Model";
			vehicleVm.LegislativeClass = LegislativeClass.M3;
			vehicleVm.CurbMassChassis = Kilogram.Create(20000);
			vehicleVm.GrossVehicleMassRating = Kilogram.Create(20000);
			vehicleVm.RegisteredClass = RegistrationClass.I_II;
			vehicleVm.VehicleCode = VehicleCode.CC;
			vehicleVm.LowEntry = true;
			vehicleVm.Height = Meter.Create(2.6);
			vehicleVm.NumberPassengerSeatsUpperDeck = 2;
			vehicleVm.NumberPassengersStandingLowerDeck = 13;
			vehicleVm.NumberPassengerSeatsLowerDeck = 10;
			vehicleVm.NumberPassengersStandingUpperDeck = 12;


			
			
			var vifName = multistagevm.SaveVif(TestHelper.GetMethodName() + ".xml");

			Assert.NotNull(vifName);
			WriteLine($"Written to {vifName}");
			Assert.IsTrue(checkFileNameExists(vifName));

			Assert.AreEqual(2, jobListVm.Jobs.Count);

			Assert.IsTrue(jobListVm.Jobs[1].CanBeSimulated, string.Join("\n",((AdditionalJobInfoViewModelMultiStage)jobListVm.Jobs[1].AdditionalJobInfoVm).InvalidEntries));
		}


		[Test]
		public void CreateIncompletedExemptedVif()
		{
			var multistagevm = LoadFileFromTestDirectory(exempted_primary_vif).MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var jobListVm = _kernel.Get<IJobListViewModel>();

			var vehicleVm =
				multistagevm.ManufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;

			Assert.IsTrue(vehicleVm.ExemptedVehicle);
			vehicleVm.Manufacturer = "Test Manufacturer 1";
			vehicleVm.ManufacturerAddress = "Address";
			vehicleVm.VIN = "123456789";
			vehicleVm.Model = "Model";
			//vehicleVm.LegislativeClass = LegislativeClass.M3;
			vehicleVm.CurbMassChassis = Kilogram.Create(20000);
			vehicleVm.GrossVehicleMassRating = Kilogram.Create(20000);
			//vehicleVm.RegisteredClass = RegistrationClass.I_II;
			vehicleVm.VehicleCode = VehicleCode.CC;
			vehicleVm.LowEntry = true;
			vehicleVm.Height = Meter.Create(2.6);
			vehicleVm.NumberPassengerSeatsUpperDeck = 2;
			vehicleVm.NumberPassengerSeatsLowerDeck = 10;




			var vifName = multistagevm.SaveVif(TestHelper.GetMethodName() + ".xml");

			Assert.NotNull(vifName);
			WriteLine($"Written to {vifName}");
			Assert.IsTrue(checkFileNameExists(vifName));

			Assert.AreEqual(2, jobListVm.Jobs.Count);

			Assert.IsFalse(jobListVm.Jobs[1].CanBeSimulated);
			var invalidEntries = (jobListVm.Jobs[1].AdditionalJobInfoVm as AdditionalJobInfoViewModelMultiStage).InvalidEntries;
			Assert.Contains(nameof(vehicleVm.LegislativeClass), invalidEntries);
			Assert.Contains(nameof(vehicleVm.RegisteredClass), invalidEntries);
		}


		[Test]
		public void TestAirdragLoadAndSave()
		{
			var newMultistageJobViewModel = LoadFileFromTestDirectory(consolidated_multiple_stages);
			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);

			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as IMultistageVehicleViewModel;
			Assert.NotNull(manstageVehicleViewModel);

			var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
			Assert.NotNull(auxiliariesViewModel);

			var multiStageViewModel = newMultistageJobViewModel.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			Assert.NotNull(multiStageViewModel);

			SetMockDialogHelper(stageInputFullSample, null);

			multiStageViewModel.ManufacturingStageViewModel.LoadStageInputData(GetTestDataPath(stageInputFullSample));
			
			var vehicle =
				multiStageViewModel.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;
			vehicle.ManufacturerAddress = "Address";
			vehicle.Manufacturer = "Manufacturer";
			vehicle.VIN = "VIN12345678";
			vehicle.AirdragModifiedMultistep = true;
			vehicle.VehicleDeclarationType = VehicleDeclarationType.interim;
			
			var writer = GetFileOutputVIFWriter(multiStageViewModel);
			
			deleteFile(writer.XMLMultistageReportFileName);
			SetMockDialogHelper(null, writer.XMLMultistageReportFileName);

			multiStageViewModel.SaveVif(multiStageViewModel, writer);
			
			var validator = new XMLValidator(XmlReader.Create(writer.XMLMultistageReportFileName));
			Assert.True(validator.ValidateXML(TUGraz.VectoCore.Utils.XmlDocumentType.MultistepOutputData));
		}


		private FileOutputVIFWriter GetFileOutputVIFWriter(IMultiStageJobViewModel multistageViewModel)
		{
			var outputFileName = primary_vehicle_only.Replace(".xml", "_vif_output_mandatory_fields.xml");
			var outputFilePath = GetFullPath(outputFileName);

			var currentStageCount = multistageViewModel.MultistageJobInputData.JobInputData.ManufacturingStages?.Count ?? 0;
			return  new FileOutputVIFWriter(outputFilePath, currentStageCount);
		}

	}
}
