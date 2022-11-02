using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace Vecto3GUI2020Test.ViewModelTests
{
    [TestFixture]
    public class VehicleViewModelTests : ViewModelTestBase
    {
		[Test]
		public void restoreValuesWhenEditingAgain()
		{
			var vm = LoadFileFromTestDirectory(primary_vehicle_only);
			var vehicleVM =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel_v2_8;
			var enteredString = "test";
			var modelParam = vehicleVM.ParameterViewModels[nameof(vehicleVM.Model)];
			//Null after loading
			Assert.IsNull(vehicleVM.Model);


			//Enable Editing
			modelParam.EditingEnabled = true;
			Assert.IsNotNull(vehicleVM.Model);

			//Enter Value 
			modelParam.CurrentContent = enteredString;

			//Stored To VM
			Assert.AreEqual(vehicleVM.Model, enteredString);

			//DisableEditing
			modelParam.EditingEnabled = false;
			//Currentvalue stored in storedcontent
			Assert.AreEqual(enteredString, modelParam.StoredContent);


			//Value is null again
			Assert.IsNull(vehicleVM.Model);

			modelParam.EditingEnabled = true;
			Assert.AreEqual(modelParam.CurrentContent, enteredString);

			//Change value 
			var enteredString2 = "test2";
            modelParam.CurrentContent = enteredString2;
			modelParam.EditingEnabled = false;
			modelParam.EditingEnabled = true;
			Assert.AreEqual(enteredString2, modelParam.CurrentContent);


			//modify through CurrentContentProperty
			modelParam.EditingEnabled = false;
			modelParam.CurrentContent = enteredString;
			Assert.AreEqual(modelParam.CurrentContent, enteredString);
			Assert.IsTrue(modelParam.EditingEnabled);

		}

		[Test]
		public void SIDummyCreation()
		{

			var vm = LoadFileFromTestDirectory(primary_vehicle_only);
			var vehicleVM =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel_v2_8;

			Assert.IsNull(vehicleVM.CurbMassChassis?.Value());
			var curbMassParameter = vehicleVM.ParameterViewModels[nameof(vehicleVM.CurbMassChassis)];

			Assert.IsNotNull(curbMassParameter.DummyContent);
			Assert.IsTrue(curbMassParameter.DummyContent is Kilogram);

			curbMassParameter.EditingEnabled = true;
			Assert.IsNotNull(vehicleVM.CurbMassChassis);
		}



		[Test]
		public void NoErrorAfterDataLoading()
		{
			var vm = LoadFileFromTestDirectory(primary_vehicle_only);
			var vehicleVM =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel_v2_8;
			var vmConc = vm.MultiStageJobViewModel as MultiStageJobViewModel_v0_1;
			vmConc.ManufacturingStageViewModel.LoadStageInputData(stageInputFullSample);
			Assert.IsFalse(vmConc.ManufacturingStageViewModel.VehicleViewModel.HasErrors);
			
		}



		#region ADAS
		[Test]
		public void LoadPrimaryAndEdit()
		{
			var vm = LoadFileFromTestDirectory(primary_vehicle_only);
			Assert.NotNull(vm);

			var vehicleViewModel =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as
					InterimStageBusVehicleViewModel_v2_8;

			var vehicleData = vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle;

			vehicleViewModel.EcoRollTypeNullable = EcoRollType.WithEngineStop;
			Assert.NotNull(vehicleData.ADAS);

			vehicleViewModel.EcoRollTypeNullable = null;
			Assert.Null(vehicleData.ADAS);

		}




		#endregion


		[Test]
		public void ConsolidatedADASandTPMLMFromPrimaryVehicle()
		{

			var vm = LoadFileFromTestDirectory(primary_vehicle_only);
			Assert.NotNull(vm);

			var vehicleViewModel =
				vm.MultiStageJobViewModel.ManufacturingStageViewModel.Vehicle as InterimStageBusVehicleViewModel_v2_8;

			Assert.NotNull(vm.MultiStageJobViewModel.ManufacturingStages);
			Assert.IsNotEmpty(vm.MultiStageJobViewModel.ManufacturingStages);

			Assert.NotNull(vehicleViewModel.ConsolidatedVehicleData);
			var consolidatedADAS = vehicleViewModel.ConsolidatedVehicleData.ADAS;
			Assert.NotNull(consolidatedADAS);

			Assert.NotNull(vehicleViewModel.ConsolidatedVehicleData.GrossVehicleMassRating);
		}


	}
}
