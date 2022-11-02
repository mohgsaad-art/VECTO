using System;
using System.IO;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLMultistageBusConsolidatedInputDataTest
	{
		protected IXMLInputDataReader _xmlInputReader;
		private IKernel _kernel;

		const string _dirPath = @"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\";
		const string _consolidatedInputData = _dirPath + "vecto_multistage_consolidated_multiple_stages.xml";
		const string _primaryOnlyInputData = _dirPath + "vecto_multistage_primary_vehicle_only.xml";
		const string _oneStageInputData = _dirPath + "vecto_multistage_consolidated_one_stage.xml";
		const string _twoStagesInputData = _dirPath + "vecto_multistage_consolidated_two_stages.xml";

		const string _consolidatedInputDataAirdrag = _dirPath + "vecto_multistage_consolidated_multiple_stages_airdrag.xml";
		const string _consolidatedInputDataHeatPump = _dirPath + "vecto_multistage_consolidated_multiple_stages_heatPump.xml";
		const string _consolidatedInputDataHeatHev = _dirPath + "vecto_multistage_consolidated_multiple_stages_hev.xml";
		const string _consolidatedInputDataHeatNgTank = _dirPath + "vecto_multistage_consolidated_multiple_stages_NGTankSystem.xml";

		
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}


		[TestCase]
		public void TestConsolidateMultistageVehicle()
		{
			var reader = XmlReader.Create(_consolidatedInputData);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;
			Assert.AreEqual(true, inputDataProvider.JobInputData.InputComplete);
			TestConsolidateManufacturingStage(inputDataProvider.JobInputData.ConsolidateManufacturingStage);
		}

		private void TestConsolidateManufacturingStage(IManufacturingStageInputData consolidateStage)
		{
			var vehicle = consolidateStage.Vehicle;
			Assert.AreEqual("Intermediate Manufacturer 3", vehicle.Manufacturer);
			Assert.AreEqual("Intermediate Manufacturer Address 3", vehicle.ManufacturerAddress);
			Assert.AreEqual("Intermediate Model 1", vehicle.Model);
			Assert.AreEqual("VEH-2234567866", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2021-02-13T07:20:08.0187663Z").ToUniversalTime(), vehicle.Date);
			Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
			Assert.AreEqual(15000, vehicle.CurbMassChassis.Value());//CorrectedActualMass
			Assert.AreEqual(20000, vehicle.GrossVehicleMassRating.Value());//TechnicalPermissibleMaximumLadenMass
			Assert.AreEqual(null, vehicle.AirdragModifiedMultistep);
			Assert.AreEqual(TankSystem.Liquefied, vehicle.TankSystem);//NgTankSystem
			Assert.AreEqual(RegistrationClass.B, vehicle.RegisteredClass);//ClassBus
			Assert.AreEqual(11, vehicle.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(31, vehicle.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(3, vehicle.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(1, vehicle.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(VehicleCode.CB, vehicle.VehicleCode);
			Assert.AreEqual(true, vehicle.LowEntry);
			Assert.AreEqual(3, vehicle.Height.Value());//HeightIntegratedBody
			Assert.AreEqual(12, vehicle.Length.Value());
			Assert.AreEqual(2.5, vehicle.Width.Value());
			Assert.AreEqual(2.5, vehicle.EntranceHeight.Value());
			Assert.AreEqual(ConsumerTechnology.Mixed, vehicle.DoorDriveTechnology);
			Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);

			Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
			Assert.AreEqual(true, vehicle.ADAS.ATEcoRollReleaseLockupClutch);

			Assert.AreEqual(null, vehicle.Components.AirdragInputData);

			var busAux = vehicle.Components.BusAuxiliaries;
			Assert.AreEqual(true, busAux.ElectricConsumers.InteriorLightsLED);
			Assert.AreEqual(true, busAux.ElectricConsumers.DayrunninglightsLED);
			Assert.AreEqual(false, busAux.ElectricConsumers.PositionlightsLED);
			Assert.AreEqual(false, busAux.ElectricConsumers.BrakelightsLED);
			Assert.AreEqual(true, busAux.ElectricConsumers.HeadlightsLED);

			var busHVACAux = vehicle.Components.BusAuxiliaries.HVACAux;
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration1, busHVACAux.SystemConfiguration);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, busHVACAux.HeatPumpTypeCoolingDriverCompartment);
			Assert.AreEqual(HeatPumpType.none, busHVACAux.HeatPumpTypeHeatingDriverCompartment);
			//Assert.AreEqual(1, busHVACAux.HeatPumpPassengerCompartments.Count);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, busHVACAux.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.none, busHVACAux.HeatPumpTypeHeatingPassengerCompartment);
			Assert.AreEqual(50, busHVACAux.AuxHeaterPower.Value());
			Assert.AreEqual(false, busHVACAux.DoubleGlazing);
			Assert.AreEqual(true, busHVACAux.AdjustableAuxiliaryHeater);
			Assert.AreEqual(false, busHVACAux.SeparateAirDistributionDucts);
			//Assert.AreEqual(false, busHVACAux.WaterElectricHeater);
			//Assert.AreEqual(false, busHVACAux.AirElectricHeater);
			//Assert.AreEqual(true, busHVACAux.OtherHeatingTechnology);
		}


		[TestCase]
		public void TestPrimaryOnlyConsolidateMultistageVehicle()
		{ 
			var reader = XmlReader.Create(_primaryOnlyInputData);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;

			Assert.AreEqual(null, inputDataProvider.JobInputData.ManufacturingStages);
			//The consolidated ManufacturingStage is now always created, and can hold some values from the primary vehicle (i.e. TPMLM)
			//Assert.AreEqual(null, inputDataProvider.JobInputData.ConsolidateManufacturingStage);
			Assert.AreEqual(false, inputDataProvider.JobInputData.InputComplete);
		}

		[TestCase]
		public void TestOneStageConsolidateMultistageVehicle()
		{
			var reader = XmlReader.Create(_oneStageInputData);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;
			Assert.AreEqual(false, inputDataProvider.JobInputData.InputComplete);

			
			var vehicle = inputDataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle;
			Assert.AreEqual("Intermediate Manufacturer 1", vehicle.Manufacturer);
			Assert.AreEqual("Intermediate Manufacturer Address 1", vehicle.ManufacturerAddress);
			Assert.AreEqual(null, vehicle.Model);
			Assert.AreEqual("VEH-1234567890", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2018-02-15T11:00:00Z").ToUniversalTime(), vehicle.Date);
			Assert.AreEqual(null, vehicle.LegislativeClass);
			Assert.AreEqual(null, vehicle.CurbMassChassis);//CorrectedActualMass
			Assert.AreEqual(4000, vehicle.GrossVehicleMassRating.Value());//TechnicalPermissibleMaximumLadenMass
			Assert.AreEqual(null, vehicle.AirdragModifiedMultistep);
			Assert.AreEqual(null, vehicle.TankSystem);//NgTankSystem
			Assert.AreEqual(null, vehicle.RegisteredClass);//ClassBus
			Assert.AreEqual(null, vehicle.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(null, vehicle.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(null, vehicle.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(null, vehicle.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(null, vehicle.VehicleCode);
			Assert.AreEqual(null, vehicle.LowEntry);
			Assert.AreEqual(null, vehicle.Height);//HeightIntegratedBody
			Assert.AreEqual(null, vehicle.Length);
			Assert.AreEqual(null, vehicle.Width);
			Assert.AreEqual(null, vehicle.EntranceHeight);
			Assert.AreEqual(null, vehicle.DoorDriveTechnology);
			Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);

			Assert.NotNull(vehicle.ADAS);

			Assert.AreEqual(null, vehicle.Components);

		}

		[TestCase]
		public void TestTwoStagesConsolidateMultistageVehicle()
		{
			var reader = XmlReader.Create(_twoStagesInputData);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;
			Assert.AreEqual(false, inputDataProvider.JobInputData.InputComplete);
			
			var vehicle = inputDataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle;
			Assert.AreEqual("Intermediate Manufacturer 2", vehicle.Manufacturer);
			Assert.AreEqual("Intermediate Manufacturer Address 2", vehicle.ManufacturerAddress);
			Assert.AreEqual("Intermediate Model 1", vehicle.Model);
			Assert.AreEqual("VEH-2234567890", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2021-02-13T07:20:08.0187663Z").ToUniversalTime(), vehicle.Date);
			Assert.AreEqual(null, vehicle.LegislativeClass);
			Assert.AreEqual(15000, vehicle.CurbMassChassis.Value());//CorrectedActualMass
			Assert.AreEqual(4000, vehicle.GrossVehicleMassRating.Value());//TechnicalPermissibleMaximumLadenMass
			Assert.AreEqual(null, vehicle.AirdragModifiedMultistep);
			Assert.AreEqual(TankSystem.Compressed, vehicle.TankSystem);//NgTankSystem
			Assert.AreEqual(null, vehicle.RegisteredClass);//ClassBus
			Assert.AreEqual(12, vehicle.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(5, vehicle.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(30, vehicle.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(4, vehicle.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(VehicleCode.CB, vehicle.VehicleCode);
			Assert.AreEqual(true, vehicle.LowEntry);
			Assert.AreEqual(3, vehicle.Height.Value());//HeightIntegratedBody
			Assert.AreEqual(12, vehicle.Length.Value());
			Assert.AreEqual(2.5, vehicle.Width.Value());
			Assert.AreEqual(2.5, vehicle.EntranceHeight.Value());
			Assert.AreEqual(ConsumerTechnology.Mixed, vehicle.DoorDriveTechnology);
			Assert.AreEqual(VehicleDeclarationType.interim, vehicle.VehicleDeclarationType);

			Assert.AreEqual(true, vehicle.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.WithEngineStop, vehicle.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicle.ADAS.PredictiveCruiseControl);
			Assert.AreEqual(null, vehicle.ADAS.ATEcoRollReleaseLockupClutch);
			
			Assert.AreEqual(null, vehicle.Components);
		}


		[TestCase]
		public void TestConsolidateMultistageAirdrag()
		{
			var reader = XmlReader.Create(_consolidatedInputDataAirdrag);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;

			var hvaux = inputDataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries.HVACAux;
			Assert.NotNull(hvaux);
			Assert.AreEqual(HeatPumpType.none, hvaux.HeatPumpTypeCoolingDriverCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvaux.HeatPumpTypeHeatingDriverCompartment);
			//Assert.AreEqual(2, hvaux.HeatPumpPassengerCompartments.Count);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvaux.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.none, hvaux.HeatPumpTypeHeatingPassengerCompartment);
			//Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvaux.HeatPumpPassengerCompartments[1].Item1);
			//Assert.AreEqual(HeatPumpMode.heating, hvaux.HeatPumpPassengerCompartments[1].Item2);

			Assert.AreEqual(true, inputDataProvider.JobInputData.InputComplete);
		}

		[TestCase]
		public void TestConsolidateMultistageHeatPump()
		{
			var reader = XmlReader.Create(_consolidatedInputDataHeatPump);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;

			var hvaux = inputDataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries.HVACAux;
			Assert.NotNull(hvaux);
			Assert.AreEqual(HeatPumpType.not_applicable, hvaux.HeatPumpTypeHeatingDriverCompartment);
			Assert.AreEqual(HeatPumpType.none, hvaux.HeatPumpTypeCoolingDriverCompartment);
			//Assert.AreEqual(1, hvaux.HeatPumpPassengerCompartments.Count);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvaux.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.none, hvaux.HeatPumpTypeHeatingPassengerCompartment);

			Assert.AreEqual(true, inputDataProvider.JobInputData.InputComplete);
		}
		
		[TestCase]
		public void TestConsolidateMultistageHeatHev()
		{
			var reader = XmlReader.Create(_consolidatedInputDataHeatHev);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;

			var hvaux = inputDataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries.HVACAux;
			Assert.NotNull(hvaux);
			Assert.AreEqual(HeatPumpType.none, hvaux.HeatPumpTypeCoolingDriverCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvaux.HeatPumpTypeHeatingDriverCompartment);
			//Assert.AreEqual(1, hvaux.HeatPumpPassengerCompartments.Count);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvaux.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.none, hvaux.HeatPumpTypeHeatingPassengerCompartment);

			Assert.AreEqual(true, inputDataProvider.JobInputData.InputComplete);
		}

		[TestCase]
		public void TestConsolidateMultistageHeatNgTankSystem()
		{
			var reader = XmlReader.Create(_consolidatedInputDataHeatNgTank);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;

			var hvaux = inputDataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle.Components.BusAuxiliaries.HVACAux;
			Assert.NotNull(hvaux);
			Assert.AreEqual(HeatPumpType.none, hvaux.HeatPumpTypeHeatingDriverCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvaux.HeatPumpTypeCoolingDriverCompartment);
			//Assert.AreEqual(1, hvaux.HeatPumpPassengerCompartments.Count);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvaux.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.none, hvaux.HeatPumpTypeHeatingPassengerCompartment);


			Assert.AreEqual(true, inputDataProvider.JobInputData.InputComplete);
		}
	}
}
