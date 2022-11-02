using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Tests.Integration.Declaration;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.Tests.Integration.Multistage
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class MultistageVehicleTest
	{
		const string VIFDirPath = @"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\";
		const string InputDirPath = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\";
		private const string PrimaryInputDirPath = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\";


		const string InputFilePath = InputDirPath  + "vecto_vehicle-stage_input_full-sample.xml";
		const string VIFInputFile = VIFDirPath  + "vecto_multistage_primary_vehicle_stage_2_3.xml";


		const string InputFilePathGroup41 = InputDirPath + "vecto_vehicle-stage_input_full-sample_group41.xml";
		const string VIFInputFileGroup41 = VIFDirPath + "vecto_multistage_primary_vehicle_stage_2_3_group41.xml";

		const string VIFExemptedPrimaryBus = VIFDirPath + "exempted_primary_heavyBus.VIF.xml";
		private const string ExepmtedCompletedBusInput = InputDirPath + "vecto_vehicle-exempted_input_full-sample.xml";


		const string vifResult = VIFDirPath + "vif_vehicle-sample.VIF_Report_3.xml";


		public const string PrimaryBus = PrimaryInputDirPath + "vecto_vehicle-primary_heavyBus-sample.xml";
		public const string PrimaryBus_SmartES = PrimaryInputDirPath + "vecto_vehicle-primary_heavyBusSmartES-sample.xml";

		const string PrimaryBusAdasV23 = PrimaryInputDirPath + "vecto_vehicle-primary_heavyBusSmartES_invalid_testdata.xml";


		protected IXMLInputDataReader xmlInputReader;
		protected IXMLInputDataReader xmlVIFInputReader;

		private IKernel _kernel;
		private string _generatedVIFFilepath;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
			xmlVIFInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(VIFInputFile, InputFilePath, 1)]
		public void TestSimulationMultistageVehicle(string vifFilename, string inputFilename, int numRuns)
		{
			//Input files
			var inputReader = XmlReader.Create(inputFilename);
			var inputDataProvider = xmlInputReader.CreateDeclaration(inputReader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;

			var vifReader = XmlReader.Create(vifFilename);
			var vifDataProvider = xmlInputReader.Create(vifReader) as IMultistageBusInputDataProvider;

			var numberOfManufacturingStages = vifDataProvider?.JobInputData.ManufacturingStages?.Count ?? 0;
			var writer = new FileOutputVIFWriter(vifResult, numberOfManufacturingStages);
			_generatedVIFFilepath = writer.XMLMultistageReportFileName;
			
			var inputData = new XMLDeclarationVIFInputData(vifDataProvider, vehicle);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToList();
			Assert.AreEqual(numRuns, runs.Count);
			foreach (var run in runs)
			{
				jobContainer.AddRun(run);
			}

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));

			using (var xmlReader = XmlReader.Create(writer.XMLMultistageReportFileName)) {
				var validator = new XMLValidator(xmlReader);
				Assert.IsTrue(validator.ValidateXML(VectoCore.Utils.XmlDocumentType.MultistepOutputData), validator.ValidationError);
			}
		}

		#region VIF result file test

		[TestCase(VIFInputFile, InputFilePath, 1)]
		public void TestVifGeneration(string vifFilename, string inputFilename, int numRuns)
		{
			//Input files
			var inputReader = XmlReader.Create(inputFilename);
			var inputDataProvider = xmlInputReader.CreateDeclaration(inputReader);
			var vehicle = inputDataProvider.JobInputData.Vehicle;

			var vifReader = XmlReader.Create(vifFilename);
			var vifDataProvider = xmlInputReader.Create(vifReader) as IMultistageBusInputDataProvider;

			var numberOfManufacturingStages = vifDataProvider?.JobInputData.ManufacturingStages?.Count ?? 0;
			var writer = new FileOutputVIFWriter(vifResult, numberOfManufacturingStages);
			
			var inputData = new XMLDeclarationVIFInputData(vifDataProvider, vehicle);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToList();
			Assert.AreEqual(numRuns, runs.Count);
			foreach (var run in runs)
			{
				jobContainer.AddRun(run);
			}

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));

			using (var xmlReader = XmlReader.Create(writer.XMLMultistageReportFileName))
			{
				var validator = new XMLValidator(xmlReader);
				Assert.IsTrue(validator.ValidateXML(VectoCore.Utils.XmlDocumentType.MultistepOutputData), validator.ValidationError);
			}

			TestNewVifData(writer.XMLMultistageReportFileName);
		}
		

		

		private void TestNewVifData(string filePath)
		{
			var vifReader = XmlReader.Create(filePath);
			var vifDataProvider = xmlInputReader.Create(vifReader) as IMultistageBusInputDataProvider;

			Assert.AreEqual(3, vifDataProvider.JobInputData.ManufacturingStages.Count());
			TestVifStage2Data(vifDataProvider.JobInputData.ManufacturingStages[0]);
			TestVifStage3Data(vifDataProvider.JobInputData.ManufacturingStages[1]);
			TestVifStage4Data(vifDataProvider.JobInputData.ManufacturingStages[2]);
		}
		
		private void TestVifStage2Data(IManufacturingStageInputData data)
		{
			Assert.AreEqual(2, data.StepCount);
			TestSignatureData(data.HashPreviousStep, "nI+57QQtWA2rFqJTZ41t0XrXcJbcGmc7j4E66iGJyT0=",
				"#PIF-d10aff76c5d149948046");

			Assert.AreEqual("Intermediate Manufacturer 1", data.Vehicle.Manufacturer);
			Assert.AreEqual("Intermediate Manufacturer Address 1", data.Vehicle.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567890", data.Vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2018-02-15T11:00:00Z").ToUniversalTime(), data.Vehicle.Date);
			Assert.AreEqual(VehicleDeclarationType.interim, data.Vehicle.VehicleDeclarationType);
			Assert.AreEqual(null, data.Vehicle.ADAS);
			Assert.AreEqual(null, data.Vehicle.Components);

			TestApplicationInformation(data.ApplicationInformation, "!!NOT FOR CERTIFICATION!!",
				"2021-01-12T07:20:08.0187663Z");

			TestSignatureData(data.Signature, "BMpFCKh1bu/YPwYj37kJK1uCrv++BTLf2OUZcOt43Os=",
				"#RESULT-6f30c7fe665a47938f6b");
		}

		private void TestVifStage3Data(IManufacturingStageInputData data)
		{
			Assert.AreEqual(3, data.StepCount);
			TestSignatureData(data.HashPreviousStep, "BMpFCKh1bu/YPwYj37kJK1uCrv++BTLf2OUZcOt43Os=",
				"#RESULT-6f30c7fe665a47938f6b");

			Assert.AreEqual("Intermediate Manufacturer 2", data.Vehicle.Manufacturer);
			Assert.AreEqual("Intermediate Manufacturer Address 2", data.Vehicle.ManufacturerAddress);
			Assert.AreEqual("VEH-2234567890", data.Vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2021-02-13T07:20:08.0187663Z").ToUniversalTime(), data.Vehicle.Date);
			Assert.AreEqual(VehicleDeclarationType.final, data.Vehicle.VehicleDeclarationType);

			Assert.AreEqual(true, data.Vehicle.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.WithEngineStop, data.Vehicle.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, data.Vehicle.ADAS.PredictiveCruiseControl);
			Assert.AreEqual(null, data.Vehicle.ADAS.ATEcoRollReleaseLockupClutch);

			var aux = data.Vehicle.Components.BusAuxiliaries;
			Assert.AreEqual(true, aux.ElectricConsumers.InteriorLightsLED);
			Assert.AreEqual(true, aux.ElectricConsumers.DayrunninglightsLED);
			Assert.AreEqual(false, aux.ElectricConsumers.PositionlightsLED);
			Assert.AreEqual(false, aux.ElectricConsumers.BrakelightsLED);
			Assert.AreEqual(true, aux.ElectricConsumers.HeadlightsLED);

			var hvac = data.Vehicle.Components.BusAuxiliaries.HVACAux;
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration1, hvac.SystemConfiguration);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvac.HeatPumpTypeCoolingDriverCompartment);
			Assert.AreEqual(HeatPumpType.none, hvac.HeatPumpTypeHeatingDriverCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvac.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvac.HeatPumpTypeCoolingDriverCompartment);

			Assert.AreEqual(50.SI<Watt>(), hvac.AuxHeaterPower);
			Assert.AreEqual(false, hvac.DoubleGlazing);
			Assert.AreEqual(true, hvac.AdjustableAuxiliaryHeater);
			Assert.AreEqual(false, hvac.SeparateAirDistributionDucts);
			//Assert.AreEqual(false, hvac.WaterElectricHeater);
			//Assert.AreEqual(false, hvac.AirElectricHeater);
			//Assert.AreEqual(true, hvac.OtherHeatingTechnology);

			TestApplicationInformation(data.ApplicationInformation, "!!NOT FOR CERTIFICATION!!!",
				"2021-03-13T07:20:08.0187663Z");

			TestSignatureData(data.Signature, "GHpFCKh1bu/YPwYj37kJK1uCrv++BTLf2OUZcOt43Os=",
				"#RESULT-8f30c7fe665a47938f6b");
		}

		private void TestVifStage4Data(IManufacturingStageInputData data)
		{
			Assert.AreEqual(4, data.StepCount);
			TestSignatureData(data.HashPreviousStep, "GHpFCKh1bu/YPwYj37kJK1uCrv++BTLf2OUZcOt43Os=",
				"#RESULT-8f30c7fe665a47938f6b");
			
			Assert.AreEqual("Some Manufacturer", data.Vehicle.Manufacturer);
			Assert.AreEqual("Some Manufacturer Address", data.Vehicle.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567890", data.Vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2021-06-30T22:00:00Z").ToUniversalTime(), data.Vehicle.Date);
			Assert.AreEqual("Sample Bus Model", data.Vehicle.Model);
			Assert.AreEqual(LegislativeClass.M3, data.Vehicle.LegislativeClass);
			Assert.AreEqual(500, data.Vehicle.CurbMassChassis.Value());
			Assert.AreEqual(3500, data.Vehicle.GrossVehicleMassRating.Value());
			Assert.AreEqual(null, data.Vehicle.AirdragModifiedMultistep);
			Assert.AreEqual(TankSystem.Compressed, data.Vehicle.TankSystem);
			Assert.AreEqual(RegistrationClass.II_III, data.Vehicle.RegisteredClass);
			Assert.AreEqual(1, data.Vehicle.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(10, data.Vehicle.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(11, data.Vehicle.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(2, data.Vehicle.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(VehicleCode.CB, data.Vehicle.VehicleCode);
			Assert.AreEqual(false, data.Vehicle.LowEntry);
			Assert.AreEqual(2.5.SI<Meter>(), data.Vehicle.Height);//HeightIntegratedBody
			Assert.AreEqual(9.5.SI<Meter>(), data.Vehicle.Length);
			Assert.AreEqual(2.5.SI<Meter>(), data.Vehicle.Width);
			Assert.AreEqual(2.SI<Meter>(), data.Vehicle.EntranceHeight);
			Assert.AreEqual(VehicleDeclarationType.interim, data.Vehicle.VehicleDeclarationType);
			
			Assert.AreEqual(true, data.Vehicle.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.None, data.Vehicle.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.None, data.Vehicle.ADAS.PredictiveCruiseControl);
			Assert.AreEqual(true, data.Vehicle.ADAS.ATEcoRollReleaseLockupClutch);
			
			var airdrag = data.Vehicle.Components.AirdragInputData as AbstractCommonComponentType;
			Assert.NotNull(airdrag);
			Assert.AreEqual("Generic Manufacturer", airdrag.Manufacturer);
			Assert.AreEqual("Generic Model", airdrag.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", airdrag.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-03-24T15:00:00Z").ToUniversalTime(), airdrag.Date);
			Assert.AreEqual("Vecto AirDrag x.y", airdrag.AppVersion);
			Assert.AreEqual(6.34.SI<SquareMeter>(), data.Vehicle.Components.AirdragInputData.AirDragArea);
			Assert.AreEqual(6.31.SI<SquareMeter>(), data.Vehicle.Components.AirdragInputData.AirDragArea_0);
			Assert.AreEqual(6.32.SI<SquareMeter>(), data.Vehicle.Components.AirdragInputData.TransferredAirDragArea);
			TestSignatureData(airdrag.DigestValue, "b9SHCfOoVrBxFQ8wwDK32OO+9bd85DuaUdgs6j/29N8=", "#CabinX23h");
			
			var aux = data.Vehicle.Components.BusAuxiliaries;
			Assert.AreEqual(false, aux.ElectricConsumers.InteriorLightsLED);
			Assert.AreEqual(true, aux.ElectricConsumers.DayrunninglightsLED);
			Assert.AreEqual(true, aux.ElectricConsumers.PositionlightsLED);
			Assert.AreEqual(true, aux.ElectricConsumers.BrakelightsLED);
			Assert.AreEqual(false, aux.ElectricConsumers.HeadlightsLED);
			
			var hvac = data.Vehicle.Components.BusAuxiliaries.HVACAux;
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration0, hvac.SystemConfiguration);
			Assert.AreEqual(HeatPumpType.none, hvac.HeatPumpTypeCoolingDriverCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvac.HeatPumpTypeHeatingDriverCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvac.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.non_R_744_4_stage, hvac.HeatPumpTypeHeatingPassengerCompartment);
			Assert.AreEqual(50.SI<Watt>(), hvac.AuxHeaterPower);
			Assert.AreEqual(false, hvac.DoubleGlazing);
			Assert.AreEqual(true, hvac.AdjustableAuxiliaryHeater);
			Assert.AreEqual(false, hvac.SeparateAirDistributionDucts);
			//Assert.AreEqual(true, hvac.WaterElectricHeater);
			//Assert.AreEqual(false, hvac.AirElectricHeater);
			//Assert.AreEqual(false, hvac.OtherHeatingTechnology);
		}
		
		private void TestSignatureData(DigestData digestData, string digestValue, string reference)
		{
			Assert.AreEqual(reference, digestData.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", digestData.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", digestData.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", digestData.DigestMethod);
			Assert.AreEqual(digestValue, digestData.DigestValue);
		}

		private void TestApplicationInformation(IApplicationInformation appInfo, string version, string date)
		{
			Assert.AreEqual(version, appInfo.SimulationToolVersion);
			Assert.AreEqual(DateTime.Parse(date).ToUniversalTime(), appInfo.Date);
		}

		#endregion

		

		[TestCase()]
		public void TestMultistageSimulationRun()
		{
			TestSimulationMultistageVehicle(VIFInputFileGroup41, InputFilePathGroup41, 1);
			
			var vifReader = XmlReader.Create(_generatedVIFFilepath);
			var vifDataProvider = xmlInputReader.Create(vifReader) as IMultistageBusInputDataProvider;

			var inputData = new XMLDeclarationVIFInputData(vifDataProvider, null);
			var writer = new MockDeclarationWriter("vif_vehicle-sample_test.xml");
			

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;

			var jobContainer = new JobContainer(new SummaryDataContainer(writer));
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase()]
		public void TestMultistageExemptedSimulationRun()
		{
			TestSimulationMultistageVehicle(VIFExemptedPrimaryBus, ExepmtedCompletedBusInput, 1);

			var vifReader = XmlReader.Create(_generatedVIFFilepath);
			var vifDataProvider = xmlInputReader.Create(vifReader) as IMultistageBusInputDataProvider;

			var inputData = new XMLDeclarationVIFInputData(vifDataProvider, null);
			var writer = new FileOutputWriter("vif_vehicle-sample_test.xml");


			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;

			var jobContainer = new JobContainer(new SummaryDataContainer(writer));
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat<Exception>(jobContainer.Runs.Select(r => r.ExecException)));
		}


		[NonParallelizable]
		[TestCase(PrimaryBus, TestName = "Multistage Write VIF Primary"),
		TestCase(PrimaryBus_SmartES, TestName = "Multistage Write VIF Primary SmartES")]
		public void TestMultistageWritingVif(string primaryFile)
		{
			var inputData = xmlInputReader.Create(primaryFile);

			var writer = new MockDeclarationWriter("vif_writing_test.xml");
			
			//var xmlreport = new XMLDeclarationReportMultistageBusVehicle(writer);
			var xmlreport = new XMLDeclarationReportPrimaryVehicle(writer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer, xmlreport);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;

			var jobContainer = new JobContainer(new SummaryDataContainer(writer));
			jobContainer.AddRuns(factory);

			xmlreport.DoWriteReport();
		}


		[TestCase(PrimaryBus, "vif_primary_bus_writing_test.xml", TestName = "Multistage Write VIF Primary With Simulation"),
		TestCase(PrimaryBus_SmartES, "vif_primary_bus_smart_writing_test.xml",  TestName = "Multistage Write VIF Primary SmartES With Simulation")]
		public void TestMultistageWritingVifWithSimulation(string primaryFile, string outputFile)
		{
			var inputData = xmlInputReader.Create(primaryFile);

			var writer = new MockDeclarationWriter(outputFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;

			var jobContainer = new JobContainer(new SummaryDataContainer(writer));
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		
		[TestCase(PrimaryBusAdasV23, "vif_primary_bus_writing_test_adasv2.3.xml", TestName = "Multistage Write VIF Primary With ADAS v 2.3")]
		public void TestPrimaryWritingVIF(string primaryFile, string outputFile)
		{
			var inputData = xmlInputReader.Create(primaryFile);

			var writer = new FileOutputWriter(outputFile);

			var xmlreport = new XMLDeclarationReportPrimaryVehicle(writer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer, xmlreport);
			factory.WriteModalResults = true;
			factory.Validate = false;

			var jobContainer = new JobContainer(new SummaryDataContainer(writer));
			jobContainer.AddRuns(factory);
			xmlreport.DoWriteReport();
			
			//Set fake test result for validation and unit tests
			SetTestResults(xmlreport.PrimaryVehicleReport);
			writer.WriteReport(ReportType.DeclarationReportPrimaryVehicleXML, xmlreport.PrimaryVehicleReport);
			
			using (var xmlReader = XmlReader.Create(writer.XMLPrimaryVehicleReportName))
			{
				var validator = new XMLValidator(xmlReader);
				Assert.IsTrue(validator.ValidateXML(VectoCore.Utils.XmlDocumentType.MultistepOutputData), validator.ValidationError);
			}

			ValidateVIFData(writer.XMLPrimaryVehicleReportName);
		}



		private void ValidateVIFData(string vifFilePath)
		{
			var vifReader = XmlReader.Create(vifFilePath);
			var vifDataProvider = xmlInputReader.Create(vifReader) as IMultistageBusInputDataProvider;

			var res = vifDataProvider.JobInputData.PrimaryVehicle;
			TestVehicleData(res.Vehicle);
			TestADASData(res.Vehicle.ADAS);
			TestTorqueLimitsData(res.Vehicle.TorqueLimits);
			TestComponentsData(res.Vehicle.Components);
		}


		#region Test Result VIF Primary Data

		private void TestVehicleData(IVehicleDeclarationInputData vehicleData)
		{
			Assert.AreEqual("Generic Truck Manufacturer", vehicleData.Manufacturer);
			Assert.AreEqual("Street, ZIP City", vehicleData.ManufacturerAddress);
			Assert.AreEqual("Generic Model", vehicleData.Model);
			Assert.AreEqual("VEH-1234567890", vehicleData.VIN);
			Assert.AreEqual(DateTime.Parse("2017-02-15T11:00:00Z").ToUniversalTime(), vehicleData.Date);
			Assert.AreEqual(LegislativeClass.M3, vehicleData.LegislativeClass);
			Assert.AreEqual("Bus", vehicleData.VehicleCategory.ToXMLFormat());
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicleData.AxleConfiguration);
			Assert.AreEqual(false, vehicleData.Articulated);
			Assert.AreEqual(25000, vehicleData.GrossVehicleMassRating.Value());
			Assert.AreEqual(600, vehicleData.EngineIdleSpeed.AsRPM);
			Assert.AreEqual("Transmission Output Retarder", vehicleData.Components.RetarderInputData.Type.ToXMLFormat());
			Assert.AreEqual(1.000, vehicleData.Components.RetarderInputData.Ratio);
			Assert.AreEqual("Separate Angledrive", vehicleData.Components.AngledriveInputData.Type.ToXMLFormat());
			Assert.AreEqual(false, vehicleData.ZeroEmissionVehicle);
		}

		private void TestADASData(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			Assert.AreEqual(true, adas.EngineStopStart);
			Assert.AreEqual(EcoRollType.WithEngineStop, adas.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, adas.PredictiveCruiseControl);
			Assert.AreEqual(false, adas.ATEcoRollReleaseLockupClutch);
		}

		private void TestTorqueLimitsData(IList<ITorqueLimitInputData> torqueLimits)
		{
			Assert.AreEqual(3, torqueLimits.Count);
			Assert.AreEqual(6, torqueLimits[0].Gear);
			Assert.AreEqual(1800, torqueLimits[0].MaxTorque.Value());
			Assert.AreEqual(1, torqueLimits[1].Gear);
			Assert.AreEqual(2500, torqueLimits[1].MaxTorque.Value());
			Assert.AreEqual(12, torqueLimits[2].Gear);
			Assert.AreEqual(1900, torqueLimits[2].MaxTorque.Value());
		}

		private void TestComponentsData(IVehicleComponentsDeclaration vehicleComponents)
		{
			TestEngineData(vehicleComponents.EngineInputData);
			TestTransmissionData(vehicleComponents.GearboxInputData);
			TestTorqueConverterData(vehicleComponents.TorqueConverterInputData);
			TestAngledriveData(vehicleComponents.AngledriveInputData);
			TestAxelgearData(vehicleComponents.AxleGearInputData);
			TestAxelWheelsData(vehicleComponents.AxleWheels);
			TestAuxiliarieData(vehicleComponents.BusAuxiliaries);
		}

		#region Test Engine Data
		private void TestEngineData(IEngineDeclarationInputData engineData)
		{
			Assert.AreEqual("Generic Engine Manufacturer", engineData.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck Engine", engineData.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", engineData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-02-15T11:00:00Z").ToUniversalTime(), engineData.Date);
			Assert.AreEqual("VectoEngine x.y", engineData.AppVersion);
			Assert.AreEqual(12730.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(), engineData.Displacement);
			Assert.AreEqual(2200, engineData.RatedSpeedDeclared.AsRPM);
			Assert.AreEqual(380000.SI<Watt>(), engineData.RatedPowerDeclared);
			Assert.AreEqual(2400, engineData.MaxTorqueDeclared.Value());
			Assert.AreEqual(WHRType.None, engineData.WHRType);// How ?!??!

			TestEngineModes(engineData.EngineModes);
		}

		private void TestEngineModes(IList<IEngineModeDeclarationInputData> engineModes)
		{
			Assert.AreEqual(1, engineModes.Count);
			Assert.AreEqual(560, engineModes[0].IdleSpeed.AsRPM);

			var fullLoadCurve = engineModes[0].FullLoadCurve;
			var entryIndex = 0;
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("560.00", "1180.00", "-149.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("600.00", "1282.00", "-148.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("800.00", "1791.00", "-149.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1000.00", "2300.00", "-160.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1200.00", "2300.00", "-179.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1400.00", "2300.00", "-203.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1600.00", "2079.00", "-235.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1800.00", "1857.00", "-264.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("2000.00", "1352.00", "-301.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("2100.00", "1100.00", "-320.00", fullLoadCurve, ref entryIndex));

			Assert.AreEqual(1, engineModes[0].Fuels.Count);
			Assert.AreEqual(FuelType.NGCI, engineModes[0].Fuels.First().FuelType);
		}
		private bool CheckFullLoadAndDragCurveEntry(string engineSpeed, string maxTorque, string dragTorque, TableData loadCurve, ref int currentRow)
		{
			var result = engineSpeed == loadCurve.Rows[currentRow][0].ToString() &&
						maxTorque == loadCurve.Rows[currentRow][1].ToString() &&
						dragTorque == loadCurve.Rows[currentRow][2].ToString();

			currentRow++;
			return result;
		}

		#endregion
		
		#region Test Transmission Data

		private void TestTransmissionData(IGearboxDeclarationInputData transmissionData)
		{
			Assert.AreEqual("Generic Gearbox Manufacturer", transmissionData.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck Gearbox", transmissionData.Model);
			Assert.AreEqual(CertificationMethod.Option1, transmissionData.CertificationMethod);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", transmissionData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T11:00:00Z").ToUniversalTime(), transmissionData.Date);
			Assert.AreEqual("3.0.1", transmissionData.AppVersion);
			Assert.AreEqual(GearboxType.ATSerial, transmissionData.Type);
			Assert.AreEqual(12, transmissionData.Gears.Count);

			var entryIndex = 0;
			Assert.IsTrue(CheckTransmissionGear(1, 14.930, 1900, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(2, 11.640, 1900, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(3, 9.020, null, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(4, 7.040, null, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(5, 5.640, null, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(6, 4.400, null, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(7, 3.390, null, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(8, 2.650, null, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(9, 2.050, null, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(10, 1.600, null, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(11, 1.280, null, 2000, transmissionData.Gears[entryIndex], ref entryIndex));
			Assert.IsTrue(CheckTransmissionGear(12, 1.000, null, null, transmissionData.Gears[entryIndex], ref entryIndex));
		}

		private bool CheckTransmissionGear(int gear, double ratio, int? maxTorque, double? maxSpeed, ITransmissionInputData entry, ref int entryIndex)
		{
			Assert.AreEqual(gear, entry.Gear);
			Assert.AreEqual(ratio, entry.Ratio);

			if (maxTorque == null)
				Assert.IsNull(entry.MaxTorque);
			else
				Assert.AreEqual(((int)maxTorque).SI<NewtonMeter>(), entry.MaxTorque);

			if (maxSpeed == null)
				Assert.IsNull(entry.MaxInputSpeed);
			else
				Assert.AreEqual((double)maxSpeed, entry.MaxInputSpeed.AsRPM, 1e-6);

			entryIndex++;
			return true;
		}

		#endregion
		
		#region Torque Converter Data Test

		private void TestTorqueConverterData(ITorqueConverterDeclarationInputData torqueConvData)
		{
			Assert.AreEqual("Some Manufacturer", torqueConvData.Manufacturer);
			Assert.AreEqual("Some Model", torqueConvData.Model);
			Assert.AreEqual(CertificationMethod.Measured, torqueConvData.CertificationMethod);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", torqueConvData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-02-15T11:00:00Z").ToUniversalTime(), torqueConvData.Date);
			Assert.AreEqual("TC CalcApp 123", torqueConvData.AppVersion);
			Assert.AreEqual(null, torqueConvData.TCData);
		}

		#endregion

		#region Angledrive Data Test

		private void TestAngledriveData(IAngledriveInputData angledriveData)
		{
			Assert.AreEqual("Generic Gearbox Manufacturer", angledriveData.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck Gearbox", angledriveData.Model);
			Assert.AreEqual(CertificationMethod.Option1, angledriveData.CertificationMethod);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", angledriveData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T11:00:00Z").ToUniversalTime(), angledriveData.Date);
			Assert.AreEqual("3.0.1", angledriveData.AppVersion);
			Assert.AreEqual(2.345, angledriveData.Ratio);
		}

		#endregion

		#region Axelgear Test
		private void TestAxelgearData(IAxleGearInputData axelgearData)
		{
			Assert.AreEqual("Generic Gearbox Manufacturer", axelgearData.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck AxleGear", axelgearData.Model);
			Assert.AreEqual(CertificationMethod.Measured, axelgearData.CertificationMethod);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", axelgearData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T11:00:00Z").ToUniversalTime(), axelgearData.Date);
			Assert.AreEqual("3.0.1", axelgearData.AppVersion);
			Assert.AreEqual(AxleLineType.SinglePortalAxle, axelgearData.LineType);
			Assert.AreEqual(2.590, axelgearData.Ratio);
		}

		#endregion

		#region AxleWheels Data Test
		private void TestAxelWheelsData(IAxlesDeclarationInputData axelWheels)
		{
			Assert.AreEqual(2, axelWheels.AxlesDeclaration.Count);

			var entry = axelWheels.AxlesDeclaration[0];
			Assert.AreEqual(AxleType.VehicleNonDriven, entry.AxleType);
			Assert.AreEqual(false, entry.TwinTyres);

			var tyre1 = entry.Tyre;
			Assert.AreEqual("Generic Wheels Manufacturer", tyre1.Manufacturer);
			Assert.AreEqual("Generic Wheel", tyre1.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", tyre1.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T14:00:00Z").ToUniversalTime(), tyre1.Date);
			Assert.AreEqual("Tyre Generation App 1.0", tyre1.AppVersion);
			Assert.AreEqual("315/70 R22.5", tyre1.Dimension);
			Assert.AreEqual(0.0055, tyre1.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre1.TyreTestLoad.Value());//85% of the maximum tyre payload

			Assert.AreEqual("#WHL-5432198760-315-70-R22.5", tyre1.DigestValue.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", tyre1.DigestValue.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", tyre1.DigestValue.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", tyre1.DigestValue.DigestMethod);
			Assert.AreEqual("4TkUGQTX8tevHOU9Cj9uyCFuI/aqcEYlo/gyVjVQmv0=", tyre1.DigestValue.DigestValue);

			entry = axelWheels.AxlesDeclaration[1];
			Assert.AreEqual(AxleType.VehicleDriven, entry.AxleType);
			Assert.AreEqual(true, entry.TwinTyres);

			var tyre2 = entry.Tyre;
			Assert.AreEqual("Generic Wheels Manufacturer", tyre2.Manufacturer);
			Assert.AreEqual("Generic Wheel", tyre2.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", tyre2.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T14:00:00Z").ToUniversalTime(), tyre2.Date);
			Assert.AreEqual("Tyre Generation App 1.0", tyre2.AppVersion);
			Assert.AreEqual("315/70 R22.5", tyre2.Dimension);
			Assert.AreEqual(0.0063, tyre2.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre2.TyreTestLoad.Value());

			Assert.AreEqual("#WHL-5432198760-315-70-R22.5", tyre2.DigestValue.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", tyre2.DigestValue.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", tyre2.DigestValue.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", tyre2.DigestValue.DigestMethod);
			Assert.AreEqual("KljvtvGUUQ/L7MiLVAqU+bckL5PNDNNwdeLH9kUVrfM=", tyre2.DigestValue.DigestValue);
		}

		#endregion

		#region Auxiliaries Test
		private void TestAuxiliarieData(IBusAuxiliariesDeclarationData aux)
		{
			Assert.AreEqual("Hydraulic driven - Constant displacement pump", aux.FanTechnology);

			Assert.AreEqual(1, aux.SteeringPumpTechnology.Count);
			Assert.AreEqual("Variable displacement elec. controlled", aux.SteeringPumpTechnology[0]);

			//ToDo SupplyFromHEVPossible to interface and reader?
			Assert.AreEqual(AlternatorType.Smart, aux.ElectricSupply.AlternatorTechnology);

			Assert.AreEqual(1, aux.ElectricSupply.Alternators.Count);
			Assert.AreEqual(300.SI<Ampere>(), aux.ElectricSupply.Alternators[0].RatedCurrent);
			Assert.AreEqual(48.SI<Volt>(), aux.ElectricSupply.Alternators[0].RatedVoltage);
			
			Assert.AreEqual(2, aux.ElectricSupply.ElectricStorage.Count);

			var battery = aux.ElectricSupply.ElectricStorage[0] as BusAuxBatteryInputData;
			Assert.IsNotNull(battery);
			Assert.AreEqual("li-ion battery - high power", battery.Technology);
			Assert.AreEqual(5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(), battery.Capacity);
			Assert.AreEqual(48.SI<Volt>(), battery.Voltage);

			var capacitor = aux.ElectricSupply.ElectricStorage[1] as BusAuxCapacitorInputData;
			Assert.IsNotNull(capacitor);
			Assert.AreEqual("with DCDC converter", capacitor.Technology);
			Assert.AreEqual(30.SI<Farad>(), capacitor.Capacity);
			Assert.AreEqual(3.SI<Volt>(), capacitor.Voltage);
			
			Assert.AreEqual("Large Supply 2-stage", aux.PneumaticSupply.CompressorSize);//SizeOfAirSupply
			Assert.AreEqual(CompressorDrive.electrically, aux.PneumaticSupply.CompressorDrive);
			Assert.AreEqual("none", aux.PneumaticSupply.Clutch);
			Assert.AreEqual(1.000, aux.PneumaticSupply.Ratio);
			Assert.AreEqual(false, aux.PneumaticSupply.SmartAirCompression);
			Assert.AreEqual(false, aux.PneumaticSupply.SmartRegeneration);
			Assert.AreEqual(ConsumerTechnology.Electrically, aux.PneumaticConsumers.AirsuspensionControl);
			Assert.AreEqual(ConsumerTechnology.Pneumatically, aux.PneumaticConsumers.AdBlueDosing); //true

			Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
			Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
		}

		#endregion

		#endregion

		#region Set Result Entry For Testing only

		private void SetTestResults(XDocument xDocument)
		{
			var results = xDocument.XPathSelectElement(".//*[local-name()='Results']");
			results.ReplaceWith(GetTempResultElement());
		}
		
		private XElement GetTempResultElement()
		{
			XNamespace tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";

			var results = new XElement(tns + XMLNames.Report_Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, "success"));

			results.Add(new XElement(tns + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "success"),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, "P31SD"),
				new XElement(tns + XMLNames.Report_Result_Mission, "Heavy Urban"),
				new XElement(tns +
							XMLNames.Report_ResultEntry_SimulationParameters,
					new XElement(tns +
								XMLNames.Report_ResultEntry_TotalVehicleMass,
						XMLHelper.ValueAsUnit(13098.63, XMLNames.Unit_kg, 2)),
					new XElement(tns + XMLNames.Report_Result_Payload, XMLHelper.ValueAsUnit(1123.63, XMLNames.Unit_kg, 2)),
					new XElement(tns + XMLNames.Report_ResultEntry_PassengerCount, 16.52),
					new XElement(tns + XMLNames.Report_Result_FuelMode, XMLNames.Report_Result_FuelMode_Val_Single)
				),
				new XElement(tns +
					XMLNames.Report_Results_Fuel,
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, "Diesel CI"),
					new XElement(tns +
								XMLNames.Report_Result_EnergyConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/km"), 16.93598)),

				new XElement(tns+ 
					XMLNames.Report_Results_CO2, new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"), 1862.57
				)));

			return results;

		}

		#endregion


		





	}
}
