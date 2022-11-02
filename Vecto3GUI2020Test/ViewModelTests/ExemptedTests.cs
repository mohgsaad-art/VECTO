using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class ExemptedTests : ViewModelTestBase
	{

		public const string _exemptedCompleted = "exempted_completed.VIF_Report_2.xml";

		[Test]
		public void LoadAndSaveExemptedPrimary()
		{
			var newMultiStageJob = LoadFileFromTestDirectory(exempted_primary_vif);
			Assert.IsTrue(newMultiStageJob.MultiStageJobViewModel.Exempted);


			var multistageJobViewModel = newMultiStageJob.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;

			var outputPath = GetFullPath("test1.xml");

			var vehicleVm =
				multistageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as
					InterimStageBusVehicleViewModel_v2_8;

			Assert.IsTrue(vehicleVm.ExemptedVehicle);


			var manufacturer = "TestManufacturer";
			var manufacturerAddress = "TestManufacturerAddress";
			var vehicleIdentificationNumber = "12345678";
			var model = "model";
			var curbMassChassis = Kilogram.Create(123456);
			var technicalpermissableLadenMass = Kilogram.Create(654321);
			var registeredClass = RegistrationClass.II_III;
			var legislativeCategory = LegislativeClass.M3;
			var bodyWorkCode = VehicleCode.CG;
			var lowEntry = false;
			var height = Meter.Create(1.5).ConvertToMilliMeter();
			var PassengerSeatsLowerDeck = 4;
			var passengerSeatsUpperDeck = 12;

			vehicleVm.ParameterViewModels[nameof(vehicleVm.Manufacturer)].CurrentContent = manufacturer;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.ManufacturerAddress)].CurrentContent = manufacturerAddress;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.HeightInMm)].CurrentContent = height;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.VIN)].CurrentContent = vehicleIdentificationNumber;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.Model)].CurrentContent = model;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.LegislativeClass)].CurrentContent = legislativeCategory;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.CurbMassChassis)].CurrentContent = curbMassChassis;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.GrossVehicleMassRating)].CurrentContent = technicalpermissableLadenMass;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.RegisteredClass)].CurrentContent = registeredClass;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.VehicleCode)].CurrentContent = bodyWorkCode;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.LowEntry)].CurrentContent = lowEntry;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.NumberPassengerSeatsLowerDeck)].CurrentContent = PassengerSeatsLowerDeck;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.NumberPassengerSeatsUpperDeck)].CurrentContent = passengerSeatsUpperDeck;


			multistageJobViewModel.ManufacturingStageViewModel.SaveInputDataExecute(outputPath);


			Assert.AreEqual(outputPath, multistageJobViewModel.VehicleInputDataFilePath);

			Assert.AreEqual(manufacturer, vehicleVm.Manufacturer);
			Assert.AreEqual(manufacturerAddress, vehicleVm.ManufacturerAddress);
			Assert.AreEqual(height, vehicleVm.HeightInMm);
			Assert.AreEqual(model, vehicleVm.Model);
			Assert.AreEqual(legislativeCategory, vehicleVm.LegislativeClass);
			Assert.AreEqual(curbMassChassis, vehicleVm.CurbMassChassis);
			Assert.AreEqual(technicalpermissableLadenMass, vehicleVm.GrossVehicleMassRating);
			Assert.AreEqual(registeredClass, vehicleVm.RegisteredClass);
			Assert.AreEqual(bodyWorkCode, vehicleVm.VehicleCode);
			Assert.AreEqual(lowEntry, vehicleVm.LowEntry);
			Assert.AreEqual(passengerSeatsUpperDeck, vehicleVm.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(PassengerSeatsLowerDeck, vehicleVm.NumberPassengerSeatsLowerDeck);
		}

		[Test]
		public async Task SaveAsNewVifAndSimulate()
		{
			var newMultiStageJob = LoadFileFromTestDirectory(exempted_primary_vif);
			Assert.IsTrue(newMultiStageJob.MultiStageJobViewModel.Exempted);


			var multistageJobViewModel = newMultiStageJob.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;

			var outputFile = GetFullPath("exemptedNewVif/test1.xml");

			var vehicleVm =
				multistageJobViewModel.ManufacturingStageViewModel.VehicleViewModel as
					InterimStageBusVehicleViewModel_v2_8;

			Assert.IsTrue(vehicleVm.ExemptedVehicle);


			var manufacturer = "TestManufacturer";
			var manufacturerAddress = "TestManufacturerAddress";
			var vehicleIdentificationNumber = "12345678";
			var model = "model";
			var curbMassChassis = Kilogram.Create(123456);
			var technicalpermissableLadenMass = Kilogram.Create(654321);
			var registeredClass = RegistrationClass.II_III;
			var bodyWorkCode = VehicleCode.CG;
			var legislativeCategory = LegislativeClass.M3;
			var lowEntry = false;
			var height = Meter.Create(1.5).ConvertToMilliMeter();
			var PassengerSeatsLowerDeck = 4;
			var passengerSeatsUpperDeck = 12;

			vehicleVm.ParameterViewModels[nameof(vehicleVm.Manufacturer)].CurrentContent = manufacturer;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.ManufacturerAddress)].CurrentContent = manufacturerAddress;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.HeightInMm)].CurrentContent = height;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.VIN)].CurrentContent = vehicleIdentificationNumber;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.Model)].CurrentContent = model;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.LegislativeClass)].CurrentContent = legislativeCategory;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.CurbMassChassis)].CurrentContent = curbMassChassis;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.GrossVehicleMassRating)].CurrentContent = technicalpermissableLadenMass;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.RegisteredClass)].CurrentContent = registeredClass;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.VehicleCode)].CurrentContent = bodyWorkCode;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.LowEntry)].CurrentContent = lowEntry;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.NumberPassengerSeatsLowerDeck)].CurrentContent = PassengerSeatsLowerDeck;
			vehicleVm.ParameterViewModels[nameof(vehicleVm.NumberPassengerSeatsUpperDeck)].CurrentContent = passengerSeatsUpperDeck;

			try {
				Directory.Delete(Path.GetDirectoryName(outputFile), true);
			} catch (Exception e) {
				WriteLine(e.Message);
			}
			var result = multistageJobViewModel.SaveVif(outputFile);

			//Check that file exists
			Assert.IsTrue(File.Exists(result));

			//Check that file was added to JobList
			var jobListVm = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			Assert.AreEqual(2, jobListVm.Jobs.Count);
			Assert.AreEqual(result, jobListVm.Jobs[1].DataSource.SourceFile);

			var inputDataProvider = _testHelper.GetInputDataProvider(result) as IMultistageBusInputDataProvider;
			Assert.NotNull(inputDataProvider);

			//Check added Manufacturing Stage
			var lastManStage = inputDataProvider.JobInputData.ManufacturingStages.Last();


			Assert.AreEqual(manufacturer, lastManStage.Vehicle.Manufacturer);
			Assert.AreEqual(manufacturerAddress, lastManStage.Vehicle.ManufacturerAddress);
			Assert.AreEqual(height.ConvertToMeter(), lastManStage.Vehicle.Height);
			Assert.AreEqual(model, lastManStage.Vehicle.Model);
			Assert.AreEqual(legislativeCategory, lastManStage.Vehicle.LegislativeClass);
			Assert.AreEqual(curbMassChassis, lastManStage.Vehicle.CurbMassChassis);
			Assert.AreEqual(technicalpermissableLadenMass, lastManStage.Vehicle.GrossVehicleMassRating);
			Assert.AreEqual(registeredClass, lastManStage.Vehicle.RegisteredClass);
			Assert.AreEqual(bodyWorkCode, lastManStage.Vehicle.VehicleCode);
			Assert.AreEqual(lowEntry, lastManStage.Vehicle.LowEntry);
			Assert.AreEqual(passengerSeatsUpperDeck, lastManStage.Vehicle.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(PassengerSeatsLowerDeck, lastManStage.Vehicle.NumberPassengerSeatsLowerDeck);


			Write("Starting simulation ...");
			jobListVm.Jobs[1].Selected = true;
			await jobListVm.RunSimulationExecute();


			Write("Done!");
		}

		[Test]
		public async Task SimulateMinimalExemptedVif()
		{
			//Setup
			var jobListViewModel = _kernel.Get<IJobListViewModel>() as JobListViewModel;
			await jobListViewModel.AddJobAsync(GetTestDataPath(_exemptedCompleted));
			Assert.AreEqual(1, jobListViewModel.Jobs.Count);

			jobListViewModel.Jobs[0].Selected = true;
			await jobListViewModel.RunSimulationExecute();


		}



	}
}