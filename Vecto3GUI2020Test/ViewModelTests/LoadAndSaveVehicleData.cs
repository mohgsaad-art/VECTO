using System;
using System.IO;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test
{
	[TestFixture]
    public class LoadAndSaveVehicleData : ViewModelTestBase
	{
		[Test]
		public void LoadInputFileMultipleStage()
		{
			Assert.NotNull(LoadFileFromTestDirectory(consolidated_multiple_stages));
		}



		[Test]
		public void LoadPrimaryAndSaveVehicleData()
		{
			//Load Primary Vehicle VIF
			var newMultiStageJob = LoadFileFromTestDirectory(primary_vehicle_only);
			var vehicle = newMultiStageJob.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(vehicle);
			vehicle.Manufacturer = "test1";
			vehicle.ManufacturerAddress = "testAddress2";
			vehicle.VIN = "VIN123456789";



			var multistageJobViewModel =
				newMultiStageJob.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;

			Assert.NotNull(multistageJobViewModel);


			var fileName = primary_vehicle_only.Replace(".xml", "") + "_output.xml";

			multistageJobViewModel.ManufacturingStageViewModel.SaveInputDataExecute(GetFullPath(fileName));
			Assert.True(checkFileNameExists(fileName));
		}

		[Test]
		public void LoadPrimaryAndSaveAsVif()
		{
			//load file
			var newMultiStageJob = LoadFileFromTestDirectory(primary_vehicle_only);
			var vehicle = newMultiStageJob.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;

		}



		[Test]
		public void ReloadInputFile()
		{
			var newMultistageJobViewModel = LoadFileFromTestDirectory(consolidated_multiple_stages_airdrag) as NewMultiStageJobViewModel;

			var vehicle = newMultistageJobViewModel.MultiStageJobViewModel.VehicleInputData as
				InterimStageBusVehicleViewModel_v2_8;

			Assert.NotNull(vehicle);


			Assert.True(vehicle.AirdragModifiedMultistepEditingEnabled);

			newMultistageJobViewModel.AddVifFile(GetTestDataPath(consolidated_multiple_stages_hev));
			Assert.AreEqual(GetTestDataPath(consolidated_multiple_stages_hev), newMultistageJobViewModel.VifPath);
			vehicle = newMultistageJobViewModel.MultiStageJobViewModel.VehicleInputData as InterimStageBusVehicleViewModel_v2_8;
			Assert.IsFalse(vehicle.AirdragModifiedMultistepEditingEnabled);

		}


		[Test]
		public void LoadInputFileMultipleStageAirdrag()
		{
			LoadFileFromTestDirectory(consolidated_multiple_stages_airdrag);
		}

		[Ignore("incomplete")]
		[Test]
		public void LoadAndSaveFullInputDataSample()
		{
			var vm = LoadFileFromTestDirectory(primary_vehicle_only);
			var multiStageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;


			var fileToSave = "loadAndSaveFullInputDataTest.xml";
			var mockDialogHelper = SetMockDialogHelper(stageInputFullSample, fileToSave);
			multiStageJobViewModel.ManufacturingStageViewModel.LoadStageInputData(GetFullPath(stageInputFullSample));


			var manufacturingStageViewModel =
				multiStageJobViewModel.ManufacturingStageViewModel as ManufacturingStageViewModel_v0_1;

			var vehicleViewModel =
				manufacturingStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;





		}

		[Test]
		public void loadVehicleInputDataOnly()
		{
			string inputPath = GetTestDataPath(stageInputFullSample);
			var inputDataReader = _kernel.Get<IXMLInputDataReader>();
			var inputData = (IDeclarationInputDataProvider)inputDataReader.Create(inputPath);
			var vehicleInputData = inputData.JobInputData.Vehicle;
			//_manufacturingStageViewModel.SetInputData(vehicleInputData);


			Assert.AreEqual(ConsumerTechnology.Electrically, vehicleInputData.DoorDriveTechnology);
		}

		[Test]
		public void loadInputFileConsolidatedOneStage()
		{
			LoadFileFromTestDirectory(consolidated_one_stage);
		}

		[Test]
		public void loadInputFilePrimaryOnly()
		{
			var vm = LoadFileFromTestDirectory(primary_vehicle_only);
			Assert.AreEqual(2, vm.MultiStageJobViewModel.ManufacturingStageViewModel.StepCount);

			var primaryVehicle = vm.MultiStageJobViewModel.PrimaryVehicle;
			Assert.NotNull(primaryVehicle);

			var vehicleViewModel =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as IMultistageVehicleViewModel;
			Assert.NotNull(vehicleViewModel);
			Assert.IsTrue(string.IsNullOrEmpty(vehicleViewModel.Manufacturer));
			Assert.IsTrue(string.IsNullOrEmpty(vehicleViewModel.ManufacturerAddress));
			Assert.IsTrue(string.IsNullOrEmpty(vehicleViewModel.VIN));
			Assert.IsNull(vehicleViewModel.Model);

			var vehicleViewModelV28 = vehicleViewModel as InterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(vehicleViewModelV28);

			Assert.Null(vehicleViewModelV28.HeightInMm);
			Assert.Null(vehicleViewModelV28.LengthInMm);
			Assert.Null(vehicleViewModelV28.WidthInMm);
			Assert.Null(vehicleViewModelV28.EntranceHeightInMm);



			Assert.Null(vehicleViewModelV28.ConsolidatedHeightInMm);
			Assert.Null(vehicleViewModelV28.ConsolidatedLengthInMm);
			Assert.Null(vehicleViewModelV28.ConsolidatedWidthInMm);
			Assert.Null(vehicleViewModelV28.ConsolidatedEntranceHeightInMm);


			Assert.IsFalse(vehicleViewModelV28.AirdragModifiedMultistepEditingEnabled);

			Assert.IsNull(vehicleViewModelV28.AirdragModifiedMultistep);
			Assert.IsNull(vehicleViewModelV28.ConsolidatedAirdragModifiedEnum);
			Assert.IsTrue(vehicleViewModelV28.AirdragModifiedEnum == AIRDRAGMODIFIED.UNKNOWN || vehicleViewModelV28.AirdragModifiedEnum == null);
			


			Assert.AreEqual(vehicleViewModelV28.AirdragModifiedMultistepEditingEnabled, false);

			Assert.Null(vehicleViewModelV28.BusAuxiliaries);


			var vifInputData = vm.MultiStageJobViewModel as IMultistageVIFInputData;

			Assert.Null(vifInputData.VehicleInputData.Components);
		}







		[TestCase(consolidated_multiple_stages_airdrag, true, TestName="LoadAirdragComponentConsolidatedMultipleStages")]
		[TestCase(consolidated_multiple_stages, null, TestName="LoadAirdragConsolidatedMultipleStage")]
		[TestCase(consolidated_one_stage, null, TestName="LoadAirdragOneStage")]
		[TestCase(primary_vehicle_only, null, TestName= "LoadAirdragPrimaryVehicle")]
		public void LoadAirdragComponentAndSaveVehicleData(string fileName, object expectedAirdragModifiedValue)
		{
			var vm = LoadFileFromTestDirectory(fileName);

			var vehicleVm =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as
					InterimStageBusVehicleViewModel_v2_8;


			var airdragLoadResult = vehicleVm.MultistageAirdragViewModel.LoadAirdragFile(GetTestDataPath(airdragLoadTestFile));
			Assert.IsTrue(airdragLoadResult, "Airdrag file not loaded");


            //TODO: Set mandatory fields
            vehicleVm.Manufacturer = "TestManufacturer";
			vehicleVm.ManufacturerAddress = "ManufacturerADDRESS";
			vehicleVm.VIN = "1234567890";


			
			var fileToSave = "stageInput.xml";

			var mockDialogHelper = SetMockDialogHelper(null, fileToSave: fileToSave);

			TestContext.Write("Saving file with loaded Airdrag Component ... ");
			var multistageJobViewModel = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			var savePath = GetFullPath($"{TestContext.CurrentContext.Test.Name}.xml");
			multistageJobViewModel.ManufacturingStageViewModel.SaveInputDataExecute(savePath);

			Assert.IsTrue(File.Exists(savePath));
			TestContext.WriteLine("Done!");
			
			TestContext.WriteLine("Checking saved File ... ");
			var inputData = (IDeclarationInputDataProvider)_kernel.Get<IXMLInputDataReader>().Create(savePath);

			Assert.NotNull(inputData.JobInputData.Vehicle.Components.AirdragInputData, "No Airdrag Component loaded");
			var airdragData = inputData.JobInputData.Vehicle.Components.AirdragInputData;
			
			Assert.AreEqual(expectedAirdragModifiedValue, vehicleVm.AirdragModifiedMultistep);

			TestContext.WriteLine("Done!");

			File.Delete(savePath);
		}


		[Test]
		public void LoadVehicleDataTest()
		{
			
			
			TestContext.WriteLine($"Loading {consolidated_multiple_stages}");
			//New Manufacturing Stage
			var newMultistageJobViewModel = LoadFileFromTestDirectory(consolidated_multiple_stages);
			Assert.NotNull(newMultistageJobViewModel.MultiStageJobViewModel);
			var manstageVehicleViewModel = newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as IMultistageVehicleViewModel;
			Assert.NotNull(manstageVehicleViewModel);
			var auxiliariesViewModel = manstageVehicleViewModel.MultistageAuxiliariesViewModel;
			Assert.NotNull(auxiliariesViewModel);
			var multiStageViewModel =
				newMultistageJobViewModel.MultiStageJobViewModel as
					MultiStageJobViewModel_v0_1;
			Assert.NotNull(multiStageViewModel);
			var manStageViewModel = multiStageViewModel.ManufacturingStageViewModel as ManufacturingStageViewModel_v0_1;


			//Load Stage InputData
			var vehicleInputDataFilePath = GetTestDataPath(stageInputFullSample);
			TestContext.WriteLine($"Loading {vehicleInputDataFilePath}");
			Assert.IsTrue(File.Exists(vehicleInputDataFilePath));
			manStageViewModel.LoadStageInputData(vehicleInputDataFilePath);

			var vehicleViewModel = manStageViewModel.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;
			Assert.NotNull(vehicleViewModel);
			//Assert.IsFalse(getMockDialogHelper().Verify);

			Assert.AreEqual("VEH-1234567890", vehicleViewModel.Identifier);
			Assert.AreEqual("Some Manufacturer", vehicleViewModel.Manufacturer);
			Assert.AreEqual("Some Manufacturer Address", vehicleViewModel.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567890", vehicleViewModel.VIN);
			Assert.AreEqual(DateTime.Today, vehicleViewModel.Date);
			Assert.AreEqual("Sample Bus Model", vehicleViewModel.Model);
			Assert.AreEqual(LegislativeClass.M3, vehicleViewModel.LegislativeClass);
			Assert.AreEqual(500, vehicleViewModel.CurbMassChassis.Value());//CorrectedActualMass
			Assert.AreEqual(3500, vehicleViewModel.GrossVehicleMassRating.Value());//TechnicalPermissibleMaximumLadenMass
			Assert.AreEqual(true, vehicleViewModel.AirdragModifiedMultistep);
			Assert.AreEqual(AIRDRAGMODIFIED.TRUE, vehicleViewModel.AirdragModifiedEnum);
			Assert.AreEqual(AIRDRAGMODIFIED.TRUE, vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.AirdragModifiedEnum)].CurrentContent);
			Assert.AreEqual(TankSystem.Compressed, vehicleViewModel.TankSystem);//NgTankSystem
			Assert.AreEqual(RegistrationClass.II_III, vehicleViewModel.RegisteredClass);//ClassBus
			Assert.AreEqual(1, vehicleViewModel.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(10, vehicleViewModel.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(11, vehicleViewModel.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(2, vehicleViewModel.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(VehicleCode.CB, vehicleViewModel.VehicleCode);
			Assert.AreEqual(false, vehicleViewModel.LowEntry);
			Assert.AreEqual(2.5, vehicleViewModel.Height.Value());//HeightIntegratedBody
			Assert.AreEqual(9.5, vehicleViewModel.Length.Value());
			Assert.AreEqual(2.5, vehicleViewModel.Width.Value());

			Assert.AreEqual(2500, (vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.HeightInMm)].CurrentContent as ConvertedSI).Value);
			Assert.AreEqual(9500, (vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.LengthInMm)].CurrentContent as ConvertedSI).Value);
			Assert.AreEqual(2500, (vehicleViewModel.ParameterViewModels[nameof(vehicleViewModel.WidthInMm)].CurrentContent as ConvertedSI).Value);
			

			Assert.AreEqual(2, vehicleViewModel.EntranceHeight.Value());
			Assert.AreEqual(ConsumerTechnology.Electrically, vehicleViewModel.DoorDriveTechnology);
			Assert.AreEqual(VehicleDeclarationType.interim, vehicleViewModel.VehicleDeclarationType);
			Assert.AreEqual("1234567890", vehicleViewModel.VehicleTypeApprovalNumber);


			Assert.AreEqual(newMultistageJobViewModel.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle.DoorDriveTechnology, vehicleViewModel.DoorDriveTechnology);


			var airdrag = vehicleViewModel.MultistageAirdragViewModel;
			Assert.NotNull(airdrag.AirDragViewModel.XMLSource);


			TestAdasInput(vehicleViewModel);
			TestComponents(vehicleViewModel.Components);




		}

		private void TestAdasInput(IVehicleDeclarationInputData vehicle)
		{
			Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.None, vehicle.ADAS.PredictiveCruiseControl);
			Assert.AreEqual(true, vehicle.ADAS.ATEcoRollReleaseLockupClutch);
		}

		private void TestComponents(IVehicleComponentsDeclaration components)
		{
			TestAirdragComponent(components.AirdragInputData);
			TestAuxiliariesComponent(components.BusAuxiliaries);
		}

		private void TestAirdragComponent(IAirdragDeclarationInputData airdrag)
		{
			Assert.AreEqual("Generic Manufacturer", airdrag.Manufacturer);
			Assert.AreEqual("Generic Model", airdrag.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", airdrag.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-03-24T15:00:00Z").ToUniversalTime(), airdrag.Date);
			Assert.AreEqual("Vecto AirDrag x.y", airdrag.AppVersion);
			Assert.AreEqual(6.34, airdrag.AirDragArea.Value());
		}

		private void TestAuxiliariesComponent(IBusAuxiliariesDeclarationData busAux)
		{
			TestLedLightsComponent(busAux.ElectricConsumers);
			TestHVACComponent(busAux.HVACAux);
		}

		private void TestLedLightsComponent(IElectricConsumersDeclarationData electricConsumer)
		{
			Assert.AreEqual(false, electricConsumer.InteriorLightsLED);
			Assert.AreEqual(true, electricConsumer.DayrunninglightsLED);
			Assert.AreEqual(true, electricConsumer.PositionlightsLED);
			Assert.AreEqual(true, electricConsumer.BrakelightsLED);
			Assert.AreEqual(false, electricConsumer.HeadlightsLED);
		}

        private void TestHVACComponent(IHVACBusAuxiliariesDeclarationData hvacAux)
        {
            Assert.AreEqual(BusHVACSystemConfiguration.Configuration0, hvacAux.SystemConfiguration);

			Assert.AreEqual(HeatPumpType.none, hvacAux.HeatPumpTypeCoolingDriverCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvacAux.HeatPumpTypeHeatingDriverCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvacAux.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_4_stage, hvacAux.HeatPumpTypeHeatingPassengerCompartment);

			Assert.AreEqual(50, hvacAux.AuxHeaterPower.Value());
            Assert.AreEqual(false, hvacAux.DoubleGlazing);
            Assert.AreEqual(true, hvacAux.AdjustableAuxiliaryHeater);
            Assert.AreEqual(false, hvacAux.SeparateAirDistributionDucts);
            Assert.AreEqual(null, hvacAux.WaterElectricHeater);
            Assert.AreEqual(null, hvacAux.AirElectricHeater);
            Assert.AreEqual(null, hvacAux.OtherHeatingTechnology);
        }






        #region Helper

        #endregion
    }

}