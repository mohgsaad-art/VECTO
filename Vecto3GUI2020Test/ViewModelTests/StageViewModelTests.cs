using System.IO;
using System.Runtime.InteropServices;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using VECTO3GUI2020.ViewModel.Implementation;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class StageViewModelTests : ViewModelTestBase
	{


		[TestCase(true, TestName="Exempted")]
		[TestCase(false, TestName="NotExempted")]
		public void updateFilePathsWhenSaved(bool exempted)
		{
			IMultiStageViewModelFactory vmFactory = _kernel.Get<IMultiStageViewModelFactory>();

			var StageInput = vmFactory.GetStageInputViewModel(exempted) as StageInputViewModel;
			var vehicleVm = StageInput.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;
			vehicleVm.Manufacturer = "adsf";
			vehicleVm.ManufacturerAddress = "asdf 123";
			vehicleVm.VIN = "1234567890";

			var fileName = TestHelper.GetMethodName() + ".xml";
			StageInput.SaveInputDataExecute(GetFullPath(fileName));
			Assert.True(checkFileNameExists(fileName));
			Assert.AreEqual(GetFullPath(fileName), StageInput.VehicleInputDataFilePath);

			//Check if title is updated
			StringAssert.Contains(fileName, StageInput.Title);

			//Check datasource
			Assert.NotNull(StageInput.DataSource);

			File.Delete(GetFullPath(fileName));
		}

		[Test]
		public void SaveFullStageInput()
		{
			IMultiStageViewModelFactory vmFactory = _kernel.Get<IMultiStageViewModelFactory>();

			var StageInput = vmFactory.GetStageInputViewModel(false) as StageInputViewModel;
			var vehicleVm = StageInput.VehicleViewModel as InterimStageBusVehicleViewModel_v2_8;
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




			var fileName = TestHelper.GetMethodName() + ".xml";
			StageInput.SaveInputDataExecute(GetFullPath(fileName));
			Assert.True(checkFileNameExists(fileName));
			Assert.AreEqual(GetFullPath(fileName), StageInput.VehicleInputDataFilePath);

			//Check if title is updated
			StringAssert.Contains(fileName, StageInput.Title);

			//Check datasource
			Assert.NotNull(StageInput.DataSource);

			File.Delete(GetFullPath(fileName));
		}





	}
}