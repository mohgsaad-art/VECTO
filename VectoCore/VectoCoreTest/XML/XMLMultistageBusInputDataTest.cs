using System;
using System.IO;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLMultistageBusInputDataTest
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;
		
		const string DirPath = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\";
		const string VehicleInterimStageInput = DirPath + "vecto_vehicle-stage_input_full-sample.xml";
		const string VehicleExemptedInterimStageInput = DirPath + "vecto_vehicle-exempted_input_full-sample.xml";
		const string VehicleExemptedMandatoryOnly = DirPath + "vecto_vehicle-exempted_input_only_mandatory_entries.xml";
		const string VehicleComponentsEntriesNullable = DirPath + "vecto_vehicle-stage_input_only_component_nullable_entries.xml";
		const string VehicleAirdragStandardValue = DirPath + "vecto_vehicle-stage_input_only_mandatory_standard_value_airdrag.xml";

		const string ValdiationDirPath = @"TestData\XML\XMLReaderDeclaration\";
		const string ExemptedInputFullSample = ValdiationDirPath + "vecto_vehicle-exempted_input_full-sample.xml";
		const string ExemptedOnlyCertainEntries01 = ValdiationDirPath + "vecto_vehicle-exempted_input_only_certain_entries01-sample.xml";
		const string ExemptedOnlyCertainEntries02 = ValdiationDirPath + "vecto_vehicle-exempted_input_only_certain_entries02-sample.xml";
		const string ExemptedSample = ValdiationDirPath + "vecto_vehicle-exempted-sample.xml";
		const string NewParamSample = ValdiationDirPath + "vecto_vehicle-new_parameters-sample.xml";
		const string VehicleSample = ValdiationDirPath + "vecto_vehicle-sample.xml";
		const string StageInputCertainEntriesEntries01 = ValdiationDirPath + "vecto_vehicle-stage_input_only_certain_entries01-sample.xml";
		const string StageInputCertainEntriesEntries02 = ValdiationDirPath + "vecto_vehicle-stage_input_only_certain_entries02-sample.xml";
		const string StageInputFullSample = ValdiationDirPath + "vecto_vehicle-stage_input_full-sample.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase]
		public void TestVehicleInterimStageInput()
		{
			var reader = XmlReader.Create(VehicleInterimStageInput);
			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;

			Assert.AreEqual("VEH-1234567890", vehicle.Identifier);
			Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
			Assert.AreEqual("Some Manufacturer Address", vehicle.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567890", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2021-06-30T22:00:00Z").ToUniversalTime(), vehicle.Date);
			Assert.AreEqual("Sample Bus Model", vehicle.Model);
			Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
			Assert.AreEqual(500, vehicle.CurbMassChassis.Value());//CorrectedActualMass
			Assert.AreEqual(3500, vehicle.GrossVehicleMassRating.Value());//TechnicalPermissibleMaximumLadenMass
			Assert.AreEqual(true, vehicle.AirdragModifiedMultistep);
			Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);//NgTankSystem
			Assert.AreEqual(RegistrationClass.II_III, vehicle.RegisteredClass);//ClassBus
			Assert.AreEqual(1, vehicle.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(10, vehicle.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(11, vehicle.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(2, vehicle.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(VehicleCode.CB, vehicle.VehicleCode);
			Assert.AreEqual(false, vehicle.LowEntry);
			Assert.AreEqual(2.5, vehicle.Height.Value());//HeightIntegratedBody
			Assert.AreEqual(9.5, vehicle.Length.Value());
			Assert.AreEqual(2.5, vehicle.Width.Value());
			Assert.AreEqual(2, vehicle.EntranceHeight.Value());
			Assert.AreEqual(ConsumerTechnology.Electrically, vehicle.DoorDriveTechnology);
			Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);

			TestADASInput(vehicle);
			TestComponents(vehicle.Components);
		}

		private void TestADASInput(IVehicleDeclarationInputData vehicle)
		{
			Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.None, vehicle.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.None,  vehicle.ADAS.PredictiveCruiseControl);
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
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvacAux.HeatPumpTypeHeatingDriverCompartment);
			Assert.AreEqual(HeatPumpType.none, hvacAux.HeatPumpTypeCoolingDriverCompartment);
			//Assert.AreEqual(3, hvacAux.HeatPumpPassengerCompartments.Count);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvacAux.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_4_stage , hvacAux.HeatPumpTypeHeatingPassengerCompartment);
			//Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvacAux.HeatPumpPassengerCompartments[1].Item1);
			//Assert.AreEqual(HeatPumpMode.heating, hvacAux.HeatPumpPassengerCompartments[1].Item2);
			//Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvacAux.HeatPumpPassengerCompartments[2].Item1);
			//Assert.AreEqual(HeatPumpMode.cooling, hvacAux.HeatPumpPassengerCompartments[2].Item2);
			Assert.AreEqual(50, hvacAux.AuxHeaterPower.Value());
			Assert.AreEqual(false, hvacAux.DoubleGlazing);
			Assert.AreEqual(true, hvacAux.AdjustableAuxiliaryHeater);
			Assert.AreEqual(false, hvacAux.SeparateAirDistributionDucts);
			Assert.AreEqual(50.SI<Watt>(), hvacAux.AuxHeaterPower);
			Assert.AreEqual(false, hvacAux.DoubleGlazing);
			Assert.AreEqual(true, hvacAux.AdjustableAuxiliaryHeater);
			Assert.AreEqual(false, hvacAux.SeparateAirDistributionDucts);

			//Assert.AreEqual(true, hvacAux.WaterElectricHeater);
			//Assert.AreEqual(false, hvacAux.AirElectricHeater);
			//Assert.AreEqual(false, hvacAux.OtherHeatingTechnology);
		}


		[TestCase]
		public void TestVehicleExemptedInterimStageInput()
		{
			var reader = XmlReader.Create(VehicleExemptedInterimStageInput);
			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;
			
			Assert.AreEqual("VEH-1234567890", vehicle.Identifier);
			Assert.AreEqual("Some Manufacturer", vehicle.Manufacturer);
			Assert.AreEqual("Infinite Loop", vehicle.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567891", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2021-01-09T11:00:00Z").ToUniversalTime(), vehicle.Date);
			Assert.AreEqual("Sample Bus Model 2", vehicle.Model);
			Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
			Assert.AreEqual(7000, vehicle.CurbMassChassis.Value());//CorrectedActualMass
			Assert.AreEqual(10000, vehicle.GrossVehicleMassRating.Value());//TechnicalPermissibleMaximumLadenMass
			Assert.AreEqual(RegistrationClass.A, vehicle.RegisteredClass);//ClassBus
			Assert.AreEqual(10, vehicle.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(42, vehicle.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(20, vehicle.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(13, vehicle.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(VehicleCode.CE, vehicle.VehicleCode);
			Assert.AreEqual(true, vehicle.LowEntry);
			Assert.AreEqual(2.5, vehicle.Height.Value());
		}

		[TestCase]
		public void TestVehicleExemptedMandatoryInput()
		{
			var reader = XmlReader.Create(VehicleExemptedMandatoryOnly);
			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;

			Assert.AreEqual("VEH-1234567890", vehicle.Identifier);
			Assert.AreEqual("Some Manufacturer 3", vehicle.Manufacturer);
			Assert.AreEqual("Some Manufacturer Address 3", vehicle.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567891", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2021-01-09T11:00:00Z").ToUniversalTime(), vehicle.Date);

			Assert.AreEqual(null, vehicle.Model);
			Assert.AreEqual(null, vehicle.LegislativeClass);
			Assert.AreEqual(null, vehicle.CurbMassChassis);
			Assert.AreEqual(null, vehicle.GrossVehicleMassRating);
			Assert.AreEqual(null, vehicle.RegisteredClass);
			Assert.AreEqual(null, vehicle.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(null, vehicle.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(null, vehicle.VehicleCode);
			Assert.AreEqual(null, vehicle.LowEntry);
			Assert.AreEqual(null, vehicle.Height);
			Assert.AreEqual(null, vehicle.Components);
		}

		[TestCase]
		public void TestNullableComponentEntriesInput()
		{
			var reader = XmlReader.Create(VehicleComponentsEntriesNullable);
			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;


			Assert.AreEqual("VEH-1234567890", vehicle.Identifier);
			Assert.AreEqual("Some Manufacturer 4", vehicle.Manufacturer);
			Assert.AreEqual("Some Manufacturer Address 4", vehicle.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567894", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2022-01-09T11:00:00Z").ToUniversalTime(), vehicle.Date);
			Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);

			Assert.AreEqual(null, vehicle.Model);
			Assert.AreEqual(null, vehicle.LegislativeClass);
			Assert.AreEqual(null, vehicle.CurbMassChassis);
			Assert.AreEqual(null, vehicle.GrossVehicleMassRating);
			Assert.AreEqual(null, vehicle.RegisteredClass);
			Assert.AreEqual(null, vehicle.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(null, vehicle.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(null, vehicle.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(null, vehicle.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(null, vehicle.VehicleCode);
			Assert.AreEqual(null, vehicle.LowEntry);
			Assert.AreEqual(null, vehicle.Height);
	
			Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, vehicle.ADAS.PredictiveCruiseControl);
			Assert.AreEqual(null, vehicle.ADAS.ATEcoRollReleaseLockupClutch);
			
			Assert.AreEqual(null , vehicle.Components);
		}
		
		[TestCase]
		public void TestStandardValueAirdragComponent()
		{
			var reader = XmlReader.Create(VehicleAirdragStandardValue);
			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;
			
			Assert.AreEqual("VEH-1234567890", vehicle.Identifier);
			Assert.AreEqual("Some Manufacturer 3", vehicle.Manufacturer);
			Assert.AreEqual("Some Manufacturer Address 3", vehicle.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567890", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2020-01-09T11:00:00Z").ToUniversalTime(), vehicle.Date);
			Assert.AreEqual(VehicleDeclarationType.final, vehicle.VehicleDeclarationType);

			var airdrag = vehicle.Components.AirdragInputData;
			Assert.IsNotNull(airdrag);
		}

		[TestCase(ExemptedInputFullSample),
		TestCase(ExemptedOnlyCertainEntries01),
		TestCase(ExemptedOnlyCertainEntries02),
		TestCase(ExemptedSample),
		TestCase(NewParamSample),
		TestCase(VehicleSample),
		TestCase(StageInputFullSample),
		TestCase(StageInputCertainEntriesEntries01),
		TestCase(StageInputCertainEntriesEntries02),
		]
		public void TestValidateInputMultistageExampleFiles(string fileName)
		{
			var reader = XmlReader.Create(fileName);
			var inputDataProvider = xmlInputReader.CreateDeclaration(reader);
			Assert.IsNotNull(inputDataProvider);

		}
	}
}
