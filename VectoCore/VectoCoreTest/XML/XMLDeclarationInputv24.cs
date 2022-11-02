using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	public class XMLDeclarationInputv24
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		private const string BASE_DIR = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\";
		private const string ADDITONAL_TESTS_DIR = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\";
		private const string Optional_TESTS_DIR = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\WithoutOptionalEntries";


		[TestCase(@"HeavyLorry\Conventional_heavyLorry_AMT.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_S3.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_S4.xml"),
		TestCase(@"HeavyLorry\HEV_heavyLorry_AMT_Px.xml"),
		TestCase(@"HeavyLorry\IEPC_heavyLorry.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_AMT_E2.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_APT-N_E2.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_E3.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_E4.xml"),
		TestCase(@"MediumLorry\Conventional_mediumLorry_AMT.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_AMT_S2.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_IEPC-S.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_S3.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_S4.xml"),
		TestCase(@"MediumLorry\HEV_mediumLorry_AMT_Px.xml"),
		TestCase(@"MediumLorry\IEPC_mediumLorry.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_AMT_E2.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_APT-N_E2.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_E3.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_E4.xml"),
		TestCase(@"PrimaryBus\Conventional_primaryBus_AMT.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_AMT_S2.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_IEPC-S.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_S3.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_S4.xml"),
		TestCase(@"PrimaryBus\HEV_primaryBus_AMT_Px.xml"),
		TestCase(@"PrimaryBus\IEPC_primaryBus.xml"),
		TestCase(@"PrimaryBus\PEV_primaryBus_AMT_E2.xml"),
		TestCase(@"PrimaryBus\PEV_primaryBus_E3.xml"),
		TestCase(@"PrimaryBus\PEV_primaryBus_E4.xml"),
		//TestCase(@"CompletedBus\Conventional_completedBus_1.xml"),
		//TestCase(@"CompletedBus\HEV_completedBus_1.xml"),
		//TestCase(@"CompletedBus\IEPC_completedBus_1.xml"),
		//TestCase(@"CompletedBus\PEV_completedBus_1.xml"),
		TestCase(@"ExemptedVehicles\exempted_completedBus_input_full.xml"),
		TestCase(@"ExemptedVehicles\exempted_completedBus_input_only_mandatory_entries.xml"),
		TestCase(@"ExemptedVehicles\exempted_heavyLorry.xml"),
		TestCase(@"ExemptedVehicles\exempted_mediumLorry.xml"),
		TestCase(@"ExemptedVehicles\exempted_primaryBus.xml"),
		]
		public void TestReadingJobVersion_V24(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"CompletedBus\Conventional_completedBus_1.xml"),
		TestCase(@"CompletedBus\HEV_completedBus_1.xml"),
		TestCase(@"CompletedBus\IEPC_completedBus_1.xml"),
		TestCase(@"CompletedBus\PEV_completedBus_1.xml"),
		]
		public void TestReadingCompletedBus_V24(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);
			Assert.NotNull(dataProvider.JobInputData.Vehicle);

			var veh = dataProvider.JobInputData.Vehicle;
			Assert.AreEqual(1, veh.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(9.5, veh.Length.Value());

			Assert.NotNull(veh.Components);
			Assert.NotNull(veh.Components.BusAuxiliaries);

			var busAux = veh.Components.BusAuxiliaries;
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration0, busAux.HVACAux.SystemConfiguration);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, busAux.HVACAux.HeatPumpTypeCoolingPassengerCompartment);

			Assert.IsTrue(busAux.ElectricConsumers.BrakelightsLED);
		}

		public IVectoRun[] ReadDeclarationJob(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);

			var fileWriter = new FileOutputWriter(filename);
			//var sumWriter = new SummaryDataContainer(fileWriter);
			//var jobContainer = new JobContainer(sumWriter);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.ModalResults1Hz = false;
			runsFactory.WriteModalResults = false;
			runsFactory.ActualModalData = false;
			runsFactory.Validate = false;

			var runs = runsFactory.SimulationRuns().ToArray();
			Assert.IsTrue(runs.Length > 0);

			return runs;
		}

		[TestCase(@"HeavyLorry\Conventional_heavyLorry_AMT.xml", BASE_DIR)]
		[TestCase(@"Conventional_heavyLorry_AMT_n_opt.xml", Optional_TESTS_DIR)]
		public void TestConventionalHeavyLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			TestADASData(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNull(vehicle.Components.ElectricMachines);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);

			if (testDir == Optional_TESTS_DIR) {//optional test
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.AngledriveInputData);//optional
				Assert.IsNull(vehicle.Components.RetarderInputData);//optional
				Assert.IsNull(vehicle.Components.AirdragInputData);//optional
				Assert.IsEmpty(vehicle.TorqueLimits);//optional
			} else {
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);//optional
				Assert.IsNotNull(vehicle.Components.RetarderInputData);//optional
				Assert.IsNotNull(vehicle.Components.AirdragInputData);//optional
				Assert.IsNotNull(vehicle.TorqueLimits);//optional
				Assert.IsNotEmpty(vehicle.TorqueLimits);//optional
			}
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.ElectricStorage);
			TestPTOData(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNull(vehicle.BoostingLimitations);//Vehicle Max Prop. Limits
		}

		private void TestADASData(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			Assert.IsNotNull(adas);
			Assert.AreEqual(true, adas.EngineStopStart);
			Assert.AreEqual(EcoRollType.WithEngineStop, adas.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.Option_1_2_3, adas.PredictiveCruiseControl);
		}

		private void TestPTOData(IPTOTransmissionInputData pto)
		{
			Assert.IsNotNull(pto);
			Assert.AreEqual("None", pto.PTOTransmissionType);
		}

		[TestCase(@"MediumLorry\Conventional_mediumLorry_AMT.xml", BASE_DIR) ]
		[TestCase(@"Conventional_mediumLorry_AMT_n_opt.xml", Optional_TESTS_DIR)]
		public void TestConventionalMediumLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			
			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			TestADASData(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNull(vehicle.Components.ElectricMachines);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);

			//optional test
			if (testDir == Optional_TESTS_DIR) {
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.CargoVolume);
				Assert.IsEmpty(vehicle.TorqueLimits);
			}
			else {
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.AreEqual(20.300.SI<CubicMeter>(), vehicle.CargoVolume);
				Assert.IsNotNull(vehicle.TorqueLimits);
				Assert.IsNotEmpty(vehicle.TorqueLimits);
			}

			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNull(vehicle.BoostingLimitations);//Vehicle Max Prop. Limit
		}

		[TestCase(@"PrimaryBus\Conventional_primaryBus_AMT.xml", BASE_DIR)]
		[TestCase(@"Conventional_primaryBus_AMT_n_opt.xml", Optional_TESTS_DIR)]
		public void TestConventionalPrimaryHeavyBus(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNull(vehicle.Components.ElectricMachines);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			
			//optional test
			if (testDir == Optional_TESTS_DIR) {
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsEmpty(vehicle.TorqueLimits);
			}
			else {
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.TorqueLimits);
				Assert.IsNotEmpty(vehicle.TorqueLimits);
			}

			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNull(vehicle.BoostingLimitations);//Vehicle Max Prop. Limit
		}
		
		[TestCase(@"HeavyLorry\HEV_heavyLorry_AMT_Px.xml", BASE_DIR)]
		[TestCase(@"HEV_heavyLorry_AMT_Px_Capacitor.xml", ADDITONAL_TESTS_DIR)]
		[TestCase(@"HEV_heavyLorry_AMT_Px_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVHeavyLorryPx(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);

			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			
			//optional test
			if (testDir == Optional_TESTS_DIR)
			{
				TestElectricMachinesData(vehicle.Components.ElectricMachines.Entries.First(), true);
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsEmpty(vehicle.TorqueLimits);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
				Assert.IsNull(vehicle.BoostingLimitations);
			}
			else
			{
				TestElectricMachinesData(vehicle.Components.ElectricMachines.Entries.First());
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.TorqueLimits);
				Assert.IsNotEmpty(vehicle.TorqueLimits);
				TestElectricMotorTorqueLimits(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
				TestBoostingLimitations(vehicle.BoostingLimitations);//Vehicle Max Prop. Limit
			}

			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels); 
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			TestElectricStorageElements(vehicle.Components.ElectricStorage.ElectricStorageElements);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
		}

		#region Test Electric Machines Reader

		private void TestElectricMachinesData(ElectricMachineEntry<IElectricMotorDeclarationInputData> eMachineEntry, bool optional = false)
		{
			Assert.IsNotNull(eMachineEntry);
			var eMachine = eMachineEntry.ElectricMachine;

			Assert.IsNotNull(eMachine);
			Assert.AreEqual("a", eMachine.Manufacturer);
			Assert.AreEqual("a", eMachine.Model);
			Assert.AreEqual("token", eMachine.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-01T00:00:00Z").ToUniversalTime(), eMachine.Date);
			Assert.AreEqual("aaaaa", eMachine.AppVersion);
			Assert.AreEqual(ElectricMachineType.ASM, eMachine.ElectricMachineType);
			Assert.AreEqual(CertificationMethod.Measured, eMachine.CertificationMethod);
			Assert.AreEqual(1.SI<Watt>(), eMachine.R85RatedPower);
			Assert.AreEqual(0.10.SI<KilogramSquareMeter>(), eMachine.Inertia);//RotationalInertia
			//Assert.AreEqual(200.00.SI<NewtonMeter>(), eMachine.ContinuousTorque);
			//Assert.AreEqual(2000.00.SI<PerSecond>(), eMachine.ContinuousTorqueSpeed);//TestSpeedContinuousTorque
			//Assert.AreEqual(400.00.SI<NewtonMeter>(), eMachine.OverloadTorque);
			//Assert.AreEqual(2000.00.SI<PerSecond>(), eMachine.OverloadTestSpeed);//TestSpeedOverloadTorque
			//Assert.AreEqual(30.00.SI<Second>(), eMachine.OverloadTime);
			Assert.AreEqual(true, eMachine.DcDcConverterIncluded);
			Assert.AreEqual("None", eMachine.IHPCType);

			TestVoltageLevel(eMachine.VoltageLevels);
			TestDragCurve(eMachine.DragCurve);
			TestConditioning(eMachine.Conditioning, optional);
		}

		private void TestVoltageLevel(IList<IElectricMotorVoltageLevel> voltageLevels)
		{
			Assert.AreEqual(2, voltageLevels.Count);
			var voltageLevel = voltageLevels[0];
			Assert.AreEqual(400.SI<Volt>(), voltageLevel.VoltageLevel);
			
			TestOverloadValues(voltageLevel);
			TestMaxTorqueCurve(voltageLevel.FullLoadCurve);
			TestPowerMap(voltageLevel.PowerMap);

			voltageLevel = voltageLevels[1];
			Assert.AreEqual(600.SI<Volt>(), voltageLevel.VoltageLevel);
			
			TestOverloadValues(voltageLevel);
			TestMaxTorqueCurve(voltageLevel.FullLoadCurve);
			TestPowerMap(voltageLevel.PowerMap);

		}

		private void TestOverloadValues(IElectricMotorVoltageLevel voltageLevel)
		{
			Assert.AreEqual(200.00.SI<NewtonMeter>(), voltageLevel.ContinuousTorque);
			Assert.AreEqual(2000.00.SI<PerSecond>(), voltageLevel.ContinuousTorqueSpeed);//TestSpeedContinuousTorque
			Assert.AreEqual(400.00.SI<NewtonMeter>(), voltageLevel.OverloadTorque);
			Assert.AreEqual(2000.00.SI<PerSecond>(), voltageLevel.OverloadTestSpeed);//TestSpeedOverloadTorque
			Assert.AreEqual(30.00.SI<Second>(), voltageLevel.OverloadTime);
		}

		private void TestPowerMap(IList<IElectricMotorPowerMap> powerMap)
		{

			if (powerMap.Count >= 1) 
				TestPowerMapData01(powerMap[0]);
			
			if (powerMap.Count >= 2) 
				TestPowerMapData02(powerMap[1]);
			
			if (powerMap.Count == 3) 
				TestPowerMapData03(powerMap[2]);
		}

		private void TestPowerMapData01(IElectricMotorPowerMap powerMap)
		{
			TestPowerMapEntry("0.00", "400.00", "1000.00", powerMap.PowerMap.Rows[0]);
			TestPowerMapEntry("0.00", "-400.00", "-1000.00", powerMap.PowerMap.Rows[1]);
			TestPowerMapEntry("4000.00", "4000.00", "20000.00", powerMap.PowerMap.Rows[2]);
			TestPowerMapEntry("4000.00", "-4000.00", "-20000.00", powerMap.PowerMap.Rows[3]);
		}

		private void TestPowerMapData02(IElectricMotorPowerMap powerMap)
		{
			TestPowerMapEntry("0.00", "500.00", "1500.00", powerMap.PowerMap.Rows[0]);
			TestPowerMapEntry("0.00", "-500.00", "-1500.00", powerMap.PowerMap.Rows[1]);
			TestPowerMapEntry("5000.00", "5000.00", "25000.00", powerMap.PowerMap.Rows[2]);
			TestPowerMapEntry("5000.00", "-5000.00", "-25000.00", powerMap.PowerMap.Rows[3]);
		}

		private void TestPowerMapData03(IElectricMotorPowerMap powerMap)
		{
			TestPowerMapEntry("0.00", "600.00", "1200.00", powerMap.PowerMap.Rows[0]);
			TestPowerMapEntry("0.00", "-600.00", "-1200.00", powerMap.PowerMap.Rows[1]);
			TestPowerMapEntry("6000.00", "6000.00", "40000.00", powerMap.PowerMap.Rows[2]);
			TestPowerMapEntry("6000.00", "-6000.00", "-40000.00", powerMap.PowerMap.Rows[3]);
		}


		private void TestMaxTorqueCurve(TableData torqueCurve)
		{
			TestMaxTorqueCurveEntry("0.00", "450.00", "-450.00", torqueCurve.Rows[0]);
			TestMaxTorqueCurveEntry("4000.00", "100.00", "-100.00", torqueCurve.Rows[1]);
		}
		
		private void TestMaxTorqueCurveEntry(string outShaftSpeed, string maxTorque, string minTorque, DataRow row)
		{
			Assert.AreEqual(outShaftSpeed, row[XMLNames.MaxTorqueCurve_OutShaftSpeed]);
			Assert.AreEqual(maxTorque, row[XMLNames.MaxTorqueCurve_MaxTorque]);
			Assert.AreEqual(minTorque, row[XMLNames.MaxTorqueCurve_MinTorque]);
		}

		private void TestPowerMapEntry(string outShaftSpeed, string torque, string electricPower, DataRow row)
		{
			Assert.AreEqual(outShaftSpeed, row[XMLNames.PowerMap_OutShaftSpeed]);
			Assert.AreEqual(torque, row[XMLNames.PowerMap_Torque]);
			Assert.AreEqual(electricPower, row[XMLNames.PowerMap_ElectricPower]);
		}

		private void TestDragCurve(TableData dragCurve)
		{
			TestDragCurveData01(dragCurve);
		}

		private void TestDragCurveData01(TableData dragCurve)
		{
			TestDragCurveEntry("0.00", "10.00", dragCurve.Rows[0]);
			TestDragCurveEntry("4000.00", "30.00", dragCurve.Rows[1]);
		}

		private void TestDragCurveData02(TableData dragCurve)
		{
			TestDragCurveEntry("0.00", "15.00", dragCurve.Rows[0]);
			TestDragCurveEntry("4500.00", "35.00", dragCurve.Rows[1]);
		}


		private void TestDragCurveEntry(string outShaftSpeed, string dragTorque, DataRow row)
		{
			Assert.AreEqual(outShaftSpeed, row[XMLNames.DragCurve_OutShaftSpeed]);
			Assert.AreEqual(dragTorque, row[XMLNames.DragCurve_DragTorque]);
		}

		private void TestConditioning(TableData eMachineConditioning , bool optional)
		{
			if(optional)
				Assert.IsNull(eMachineConditioning);
			else
				TestConditioningEntry("30", "5000", eMachineConditioning.Rows[0]);
		}

		private void TestConditioningEntry(string coolantTempInlet, string coolingPower, DataRow row)
		{
			Assert.AreEqual(coolantTempInlet, row[XMLNames.Conditioning_CoolantTempInlet]);
			Assert.AreEqual(coolingPower, row[XMLNames.Conditioning_CoolingPower]);
		}

		#endregion

		#region Test ElectricStorage Element Reader

		private void TestElectricStorageElements(IList<IElectricStorageDeclarationInputData> elements)
		{
			Assert.IsNotNull(elements);
			switch (elements.Count) {
				case 1:
					TestSuperCapEntry(elements.First());
					break;
				case 2:
					Assert.AreEqual(2, elements.Count);
					TestFirstBatterySystemEntry(elements[0]);
					TestSecondBatterySystemEntry(elements[1]);
					break;
			}
		}

		private void TestSuperCapEntry(IElectricStorageDeclarationInputData entry)
		{
			Assert.AreEqual(REESSType.SuperCap, entry.REESSPack.StorageType);
			Assert.AreEqual("Capacitor Manufacturer", entry.REESSPack.Manufacturer);
			Assert.AreEqual("Capacitor Model", entry.REESSPack.Model);
			Assert.AreEqual("ccccccc", entry.REESSPack.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-02-03T00:00:00Z").ToUniversalTime(), entry.REESSPack.Date);
			Assert.AreEqual("ccccc", entry.REESSPack.AppVersion);
			Assert.AreEqual(CertificationMethod.Measured, entry.REESSPack.CertificationMethod);
			Assert.IsNotNull(entry.REESSPack.DigestValue);
			
			var supercap = (ISuperCapDeclarationInputData)entry.REESSPack;
			Assert.AreEqual(100.00.SI<Farad>(), supercap.Capacity);
			Assert.AreEqual(20.00.SI<Ohm>() / 1000, supercap.InternalResistance);
			Assert.AreEqual(12.00.SI<Volt>(), supercap.MinVoltage);
			Assert.AreEqual(100.00.SI<Volt>(), supercap.MaxVoltage);
			Assert.AreEqual(80.00.SI<Ampere>(), supercap.MaxCurrentCharge);
			Assert.AreEqual(20.00.SI<Ampere>(), supercap.MaxCurrentDischarge);
		}

		private void TestFirstBatterySystemEntry(IElectricStorageDeclarationInputData entry)
		{
			Assert.AreEqual(REESSType.Battery, entry.REESSPack.StorageType);
			Assert.AreEqual(0, entry.StringId);
			Assert.AreEqual("a", entry.REESSPack.Manufacturer);
			Assert.AreEqual("a", entry.REESSPack.Model);
			Assert.AreEqual("tokena", entry.REESSPack.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-01T00:00:00Z").ToUniversalTime(), entry.REESSPack.Date);
			Assert.AreEqual("aaaaa", entry.REESSPack.AppVersion);
			Assert.AreEqual(CertificationMethod.Measured, entry.REESSPack.CertificationMethod);
			Assert.IsNotNull(entry.REESSPack.DigestValue);

			var battery = (IBatteryPackDeclarationInputData)entry.REESSPack;
			Assert.AreEqual(20, battery.MinSOC);
			Assert.AreEqual(80, battery.MaxSOC);
			Assert.AreEqual(BatteryType.HPBS, battery.BatteryType);
			Assert.AreEqual(72.00.SI<AmpereSecond>() * 3600, battery.Capacity);
			Assert.AreEqual(true, battery.ConnectorsSubsystemsIncluded);
			Assert.AreEqual(true, battery.JunctionboxIncluded);
			Assert.AreEqual(20.0.DegCelsiusToKelvin(), battery.TestingTemperature);

			Assert.IsNotNull(battery.VoltageCurve);//OVC Data
			TestOCVTableRow("0", "620.00", battery.VoltageCurve.Rows[0]);
			TestOCVTableRow("100", "640.00", battery.VoltageCurve.Rows[1]);
			Assert.IsNotNull(battery.InternalResistanceCurve);
			TestInternalResistanceTableRow("0", "10.00", "11.00", "12.00", battery.InternalResistanceCurve.Rows[0]);
			TestInternalResistanceTableRow("100","12.00" ,"14.00","16.00", battery.InternalResistanceCurve.Rows[1]);
			Assert.IsNotNull(battery.MaxCurrentMap);//CurrentLimits Data
			TestCurrentLimitsTableRow("0", "50.00", "0.00", battery.MaxCurrentMap.Rows[0]);
			TestCurrentLimitsTableRow("100", "0.00", "50.00", battery.MaxCurrentMap.Rows[1]);
		}

		private void TestSecondBatterySystemEntry(IElectricStorageDeclarationInputData entry)
		{
			Assert.AreEqual(REESSType.Battery, entry.REESSPack.StorageType);
			Assert.AreEqual(1, entry.StringId);
			Assert.AreEqual("b", entry.REESSPack.Manufacturer);
			Assert.AreEqual("b", entry.REESSPack.Model);
			Assert.AreEqual("tokenb", entry.REESSPack.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-02-02T00:00:00Z").ToUniversalTime(), entry.REESSPack.Date);
			Assert.AreEqual("bbbbb", entry.REESSPack.AppVersion);
			Assert.AreEqual(CertificationMethod.Measured, entry.REESSPack.CertificationMethod);
			Assert.IsNotNull(entry.REESSPack.DigestValue);

			var battery = (IBatteryPackDeclarationInputData)entry.REESSPack;
			Assert.IsNull(battery.MinSOC);
			Assert.IsNull(battery.MaxSOC);
			Assert.AreEqual(BatteryType.HPBS, battery.BatteryType);
			Assert.AreEqual(73.00.SI<AmpereSecond>() * 3600, battery.Capacity);
			Assert.AreEqual(true, battery.ConnectorsSubsystemsIncluded);
			Assert.AreEqual(true, battery.JunctionboxIncluded);
			Assert.AreEqual(20.0.DegCelsiusToKelvin(), battery.TestingTemperature);

			Assert.IsNotNull(battery.VoltageCurve);//OVC Data
			TestOCVTableRow("0", "621.00", battery.VoltageCurve.Rows[0]);
			TestOCVTableRow("100", "641.00", battery.VoltageCurve.Rows[1]);
			Assert.IsNotNull(battery.InternalResistanceCurve);
			TestInternalResistanceTableRow("0", "11.00", "12.00", "13.00", battery.InternalResistanceCurve.Rows[0]);
			TestInternalResistanceTableRow("100", "12.00", "14.00", "16.00", battery.InternalResistanceCurve.Rows[1]);
			Assert.IsNotNull(battery.MaxCurrentMap);//CurrentLimits Data
			TestCurrentLimitsTableRow("0", "51.00", "0.00", battery.MaxCurrentMap.Rows[0]);
			TestCurrentLimitsTableRow("100", "0.00", "50.00", battery.MaxCurrentMap.Rows[1]);
		}

		private void TestOCVTableRow(string soc, string ocv, DataRow row)
		{
			Assert.AreEqual(soc, row[XMLNames.REESS_OCV_SoC]);
			Assert.AreEqual(ocv, row[XMLNames.REESS_OCV_OCV]);
		}

		private void TestInternalResistanceTableRow(string soc, string r2, string r10, string r20, DataRow row)
		{
			Assert.AreEqual(soc, row[XMLNames.REESS_InternalResistanceCurve_SoC]);
			Assert.AreEqual(r2, row[XMLNames.REESS_InternalResistanceCurve_R2]);
			Assert.AreEqual(r10, row[XMLNames.REESS_InternalResistanceCurve_R10]);
			Assert.AreEqual(r20, row[XMLNames.REESS_InternalResistanceCurve_R20]);
		}
		
		private void TestCurrentLimitsTableRow(string soc, string maxChargingCurrent, string maxDischargingCurrent,
			DataRow row)
		{
			Assert.AreEqual(soc, row[XMLNames.REESS_CurrentLimits_SoC]);
			Assert.AreEqual(maxChargingCurrent, row[XMLNames.REESS_CurrentLimits_MaxChargingCurrent]);
			Assert.AreEqual(maxDischargingCurrent, row[XMLNames.REESS_CurrentLimits_MaxDischargingCurrent]);
		}


		#endregion

		#region Test Electric Motor TorqueLimits Reader

		private void TestElectricMotorTorqueLimits(Dictionary<PowertrainPosition, List<Tuple<Volt, TableData>>> limits)
		{
			Assert.IsNotNull(limits);
			Assert.AreEqual(1, limits.Count);
			Assert.AreEqual(2, limits.First().Value.Count);
			Assert.AreEqual(PowertrainPosition.HybridP2, limits.First().Key);
			
			Assert.AreEqual(100, limits.First().Value[0].Item1.Value());
			TestMaxTorqueCurveEntry("0.00", "200.00", "-200.00", limits.First().Value[0].Item2.Rows[0]);
			TestMaxTorqueCurveEntry("1000.00", "300.00", "-300.00", limits.First().Value[0].Item2.Rows[1]);

			Assert.AreEqual(500, limits.First().Value[1].Item1.Value());
			TestMaxTorqueCurveEntry("0.00", "200.00", "-200.00", limits.First().Value[1].Item2.Rows[0]);
			TestMaxTorqueCurveEntry("1000.00", "300.00", "-300.00", limits.First().Value[1].Item2.Rows[1]);
		}

		#endregion
		
		#region Test Max Propulsion Torque Reader / BoostingLimitations

		private void TestBoostingLimitations(TableData boostingLimitations)
		{
			Assert.IsNotNull(boostingLimitations);
			Assert.AreEqual(2, boostingLimitations.Rows.Count);

			TestBoostingLimitationsEntry("0.00", "0.00", boostingLimitations.Rows[0]);
			TestBoostingLimitationsEntry("1000.00", "0.00", boostingLimitations.Rows[1]);
		}

		private void TestBoostingLimitationsEntry(string rotationalSpeed, string boostingTorque, DataRow row)
		{
			Assert.AreEqual(rotationalSpeed, row[XMLNames.BoostingLimitation_RotationalSpeed]);
			Assert.AreEqual(boostingTorque, row[XMLNames.BoostingLimitation_BoostingTorque]);
		}

		#endregion

		
		[TestCase(@"MediumLorry\HEV_mediumLorry_AMT_Px.xml", BASE_DIR)]
		[TestCase(@"HEV_mediumLorry_AMT_Px_n_opt.xml", Optional_TESTS_DIR )]
		public void TestHEVMediumLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);

			//optional test
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.CargoVolume);
				Assert.IsEmpty(vehicle.TorqueLimits);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
				Assert.IsNull(vehicle.BoostingLimitations);//Vehicle Max Prop. Limit
			}
			else
			{
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.AreEqual(20.300.SI<CubicMeter>(), vehicle.CargoVolume);
				Assert.IsNotNull(vehicle.TorqueLimits);
				Assert.IsNotEmpty(vehicle.TorqueLimits);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
				Assert.IsNotNull(vehicle.BoostingLimitations);//Vehicle Max Prop. Limit
			}
			
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
		}
		

		[TestCase(@"PrimaryBus\HEV_primaryBus_AMT_Px.xml", BASE_DIR)]
		[TestCase(@"HEV_primaryBus_AMT_Px_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVPrimaryBusAMTPx(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);

			//optional test
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsEmpty(vehicle.TorqueLimits);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
				Assert.IsNull(vehicle.BoostingLimitations);//Vehicle Max Prop. Limit
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.TorqueLimits);
				Assert.IsNotEmpty(vehicle.TorqueLimits);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
				Assert.IsNotNull(vehicle.BoostingLimitations);//Vehicle Max Prop. Limit
			}

			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
		}


		[TestCase(@"HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml", BASE_DIR)]
		[TestCase(@"HEV-S_heavyLorry_AMT_S2_ADC.xml", ADDITONAL_TESTS_DIR)]
		[TestCase(@"HEV-S_heavyLorry_AMT_S2_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVHeavyLorryAMTS2(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			TestElectricMachines(vehicle.Components.ElectricMachines.Entries);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.TorqueLimits);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			}
			else
			{
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.TorqueLimits);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			}

			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.BoostingLimitations);//Vehicle Max Prop. Limit
		}

		private void TestElectricMachines(IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> eMachines)
		{
			foreach (var eMachine in eMachines) {
				switch (eMachine.Position) {
					case PowertrainPosition.GEN:
						TestElectricMachineGEN(eMachine);
						break;
					case PowertrainPosition.BatteryElectricE2:
						TestElectricMachine(eMachine);
						break;
				}
			}
		}

		private void TestElectricMachineGEN(ElectricMachineEntry<IElectricMotorDeclarationInputData> eMachine)
		{
			Assert.AreEqual(1, eMachine.Count);
			TestElectricMachinesData(eMachine);
			if (eMachine.ADC != null)
				TestADC(eMachine.ADC);
		}

		private void TestElectricMachine(ElectricMachineEntry<IElectricMotorDeclarationInputData> eMachine)
		{
			Assert.AreEqual(1, eMachine.Count);
			TestElectricMachinesData(eMachine);
			if(eMachine.ADC != null)
				TestADC(eMachine.ADC);
		}

		private void TestADC(IADCDeclarationInputData adcData)
		{
			Assert.AreEqual("ADC Manufacturer", adcData.Manufacturer);
			Assert.AreEqual("ADC Model", adcData.Model);
			Assert.AreEqual("adcadc", adcData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-01T00:00:00Z").ToUniversalTime(), adcData.Date);
			Assert.AreEqual("adccda", adcData.AppVersion);
			Assert.AreEqual(12.123, adcData.Ratio);
			Assert.AreEqual(CertificationMethod.Option1, adcData.CertificationMethod);
			Assert.IsNotNull(adcData.DigestValue);

			TestTorqueLossMapEntry("10.00","40.00" ,"30.00", adcData.LossMap.Rows[0]);
			TestTorqueLossMapEntry("11.00", "41.00", "31.00", adcData.LossMap.Rows[1]);
			TestTorqueLossMapEntry("12.00", "41.00", "32.00", adcData.LossMap.Rows[2]);
			TestTorqueLossMapEntry("13.00", "42.00", "33.00", adcData.LossMap.Rows[3]);
		}

		private void TestTorqueLossMapEntry(string inputSpeed, string inputTorque, string torqueLoss, DataRow row)
		{
			Assert.AreEqual(inputSpeed, row[XMLNames.ADC_TorqueLossMap_InputSpeed]);
			Assert.AreEqual(inputTorque, row[XMLNames.ADC_TorqueLossMap_InputTorque]);
			Assert.AreEqual(torqueLoss, row[XMLNames.ADC_TorqueLossMap_TorqueLoss]);
		}


		[TestCase(@"HeavyLorry\HEV-S_heavyLorry_S3.xml", BASE_DIR)]
		[TestCase(@"HEV-S_heavyLorry_S3_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVHeavyLorryS3(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2 ,vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE3, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN,vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			}
			else
			{
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			}

			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			//Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"HeavyLorry\HEV-S_heavyLorry_S4.xml", BASE_DIR)]
		[TestCase(@"HEV-S_heavyLorry_S4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVHeavyLorryS4(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE4, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			}
			else
			{
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"MediumLorry\HEV-S_mediumLorry_AMT_S2.xml", BASE_DIR)]
		[TestCase(@"HEV-S_mediumLorry_AMT_S2_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVMediumLorryS2(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE2, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.CargoVolume);
				Assert.IsNull(vehicle.TorqueLimits);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.CargoVolume);
				Assert.IsNull(vehicle.TorqueLimits);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"MediumLorry\HEV-S_mediumLorry_S3.xml", BASE_DIR)]
		[TestCase(@"HEV-S_mediumLorry_S3_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHevMediumLorryS3(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE3, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.CargoVolume);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.CargoVolume);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"MediumLorry\HEV-S_mediumLorry_S4.xml", BASE_DIR)]
		[TestCase(@"HEV-S_mediumLorry_S4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVMediumLorryS4(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE4, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.CargoVolume);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.CargoVolume);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"PrimaryBus\HEV-S_primaryBus_AMT_S2.xml", BASE_DIR)]
		[TestCase(@"HEV-S_primaryBus_AMT_S2_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVPrimaryBusS2(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE2, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.TorqueLimits);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.TorqueLimits);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"PrimaryBus\HEV-S_primaryBus_S3.xml", BASE_DIR)]
		[TestCase(@"HEV-S_primaryBus_S3_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVPrimaryBusS3(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE3, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"PrimaryBus\HEV-S_primaryBus_S4.xml", BASE_DIR)]
		[TestCase(@"HEV-S_primaryBus_S4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVPrimaryBusS4(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE4, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			//Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml", BASE_DIR)]
		[TestCase(@"HEV-S_heavyLorry_IEPC-S_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVIEPCHeavyLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[0].Position);
			TestIEPCData(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AxleGearInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
			}
			else
			{
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AxleGearInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		#region Test IEPC reader data

		private void TestIEPCData(IIEPCDeclarationInputData iepcData)
		{
			Assert.IsNotNull(iepcData);
			Assert.AreEqual("a", iepcData.Manufacturer);
			Assert.AreEqual("a", iepcData.Model);
			Assert.AreEqual("token", iepcData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-01T00:00:00Z").ToUniversalTime(), iepcData.Date);
			Assert.AreEqual("aaaaa", iepcData.AppVersion);
			Assert.AreEqual(ElectricMachineType.ASM, iepcData.ElectricMachineType);
			Assert.AreEqual(CertificationMethod.Measured, iepcData.CertificationMethod);
			Assert.AreEqual(1.SI<Watt>(), iepcData.R85RatedPower);
			Assert.AreEqual(0.10.SI<KilogramSquareMeter>(), iepcData.Inertia);//RotationalInertia
			Assert.AreEqual(false, iepcData.DifferentialIncluded);
			Assert.AreEqual(false, iepcData.DesignTypeWheelMotor);
			Assert.IsNull(iepcData.NrOfDesignTypeWheelMotorMeasured);
			Assert.IsNotNull(iepcData.DigestValue);

			TestGearsData(iepcData.Gears);
			TestVoltageLevel(iepcData.VoltageLevels);
			TestDragCurves(iepcData.DragCurves);
		}

		private void TestDragCurves(IList<IDragCurve> dragCurves)
		{
			Assert.AreEqual(1, dragCurves[0].Gear);
			TestDragCurveData01(dragCurves[0].DragCurve);
			
			Assert.AreEqual(2, dragCurves[1].Gear);
			TestDragCurveData02(dragCurves[1].DragCurve);
		}
		
		private void TestGearsData(IList<IGearEntry> gears)
		{
			Assert.IsNotNull(gears);
			Assert.AreEqual(2, gears.Count);

			Assert.AreEqual(1, gears[0].GearNumber);
			Assert.AreEqual(3.000, gears[0].Ratio);
			Assert.IsNull(gears[0].MaxOutputShaftSpeed);
			Assert.IsNull(gears[0].MaxOutputShaftTorque);

			Assert.AreEqual(2, gears[1].GearNumber);
			Assert.AreEqual(1.000, gears[1].Ratio);
			Assert.IsNull(gears[1].MaxOutputShaftSpeed);
			Assert.AreEqual(2000.00.SI<NewtonMeter>(), gears[1].MaxOutputShaftTorque);
		}

		#endregion


		[TestCase(@"MediumLorry\HEV-S_mediumLorry_IEPC-S.xml", BASE_DIR)]
		[TestCase(@"HEV-S_mediumLorry_IEPC-S_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVIEPCSMediumLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[0].Position);
			TestIEPCData(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.TankSystem);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AxleGearInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.CargoVolume);
			}
			else
			{
				Assert.IsNotNull(vehicle.TankSystem);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData.LossMap);
				Assert.IsNotNull(vehicle.Components.AxleGearInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.CargoVolume);
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels.AxlesDeclaration);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"PrimaryBus\HEV-S_primaryBus_IEPC-S.xml", BASE_DIR)]
		[TestCase(@"HEV-S_primaryBus_IEPC-S_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVIEPCSPrimaryBus(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[0].Position);
			TestIEPCData(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AxleGearInputData);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData.LossMap);
				Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"HeavyLorry\PEV_heavyLorry_AMT_E2.xml", BASE_DIR)]
		[TestCase(@"PEV_heavyLorry_AMT_E2_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"HeavyLorry\PEV_heavyLorry_APT-N_E2.xml", BASE_DIR)]
		[TestCase(@"PEV_heavyLorry_APT-N_E2_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVE2HeavyLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE2, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			
			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.TorqueLimits);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.TorqueLimits);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"HeavyLorry\PEV_heavyLorry_E3.xml", BASE_DIR)]
		[TestCase(@"PEV_heavyLorry_E3_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVE3HeavyLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE3, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData.LossMap);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"HeavyLorry\PEV_heavyLorry_E4.xml", BASE_DIR)]
		[TestCase(@"PEV_heavyLorry_E4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVE4HeavyLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE4, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData.LossMap);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"MediumLorry\PEV_mediumLorry_AMT_E2.xml", BASE_DIR)]
		[TestCase(@"PEV_mediumLorry_AMT_E2_n_opt.xml", Optional_TESTS_DIR)]
		[TestCase(@"MediumLorry\PEV_mediumLorry_APT-N_E2.xml", BASE_DIR)]
		[TestCase(@"PEV_mediumLorry_APT-N_E2_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVE2MediumLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE2, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				if (vehicle.VehicleCategory == VehicleCategory.Van) {
					Assert.AreEqual(20, vehicle.CargoVolume.Value());
				} else {
					Assert.IsNull(vehicle.CargoVolume);
				}

				Assert.IsNull(vehicle.TorqueLimits);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.CargoVolume);
				Assert.IsNotNull(vehicle.TorqueLimits);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"MediumLorry\PEV_mediumLorry_E3.xml", BASE_DIR)]
		[TestCase(@"PEV_mediumLorry_E3_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVE3MediumLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE3, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.CargoVolume);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.CargoVolume);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"MediumLorry\PEV_mediumLorry_E4.xml", BASE_DIR)]
		[TestCase(@"PEV_mediumLorry_E4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVE4MediumLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE4, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.IsNull(vehicle.CargoVolume);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.CargoVolume);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"PrimaryBus\PEV_primaryBus_AMT_E2.xml", BASE_DIR)]
		[TestCase(@"PEV_primaryBus_AMT_E2_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVPrimaryBusE2(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE2, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.AngledriveInputData);
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsEmpty(vehicle.TorqueLimits);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.AngledriveInputData);
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.TorqueLimits);
				Assert.IsNotEmpty(vehicle.TorqueLimits);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"PrimaryBus\PEV_primaryBus_E3.xml", BASE_DIR)]
		[TestCase(@"PEV_primaryBus_E3_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVPrimaryBusE3(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE3, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"PrimaryBus\PEV_primaryBus_E4.xml", BASE_DIR)]
		[TestCase(@"PEV_primaryBus_E4_n_opt.xml", Optional_TESTS_DIR)]
		public void TestPEVPrimaryBusE4(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE4, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.IsNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			}

			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			// Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		[TestCase(@"HeavyLorry\IEPC_heavyLorry.xml", BASE_DIR)]
		[TestCase(@"IEPC_heavyLorry_n_opt.xml", Optional_TESTS_DIR)]
		public void TestIEPCHeayLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNull(vehicle.Components.ElectricMachines);
			Assert.IsNotNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AxleGearInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AxleGearInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"MediumLorry\IEPC_mediumLorry.xml", BASE_DIR)]
		[TestCase(@"IEPC_mediumLorry_n_opt.xml", Optional_TESTS_DIR)]
		public void TestIEPCMediumLorry(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNull(vehicle.Components.ElectricMachines);
			Assert.IsNotNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AxleGearInputData);
				Assert.IsNull(vehicle.Components.AirdragInputData);
				Assert.AreEqual(20, vehicle.CargoVolume.Value());
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AxleGearInputData);
				Assert.IsNotNull(vehicle.Components.AirdragInputData);
				Assert.IsNotNull(vehicle.CargoVolume);
			}
			
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData.Auxiliaries);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}


		[TestCase(@"PrimaryBus\IEPC_primaryBus.xml", BASE_DIR)]
		[TestCase(@"IEPC_primaryBus_n_opt.xml", Optional_TESTS_DIR)]
		public void TestIEPCPrimaryBus(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.ADAS);
			Assert.IsNull(vehicle.Components.EngineInputData);
			Assert.IsNull(vehicle.Components.ElectricMachines);
			Assert.IsNotNull(vehicle.Components.IEPC);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);

			if (testDir == Optional_TESTS_DIR)
			{
				Assert.IsNull(vehicle.Components.RetarderInputData);
				Assert.IsNull(vehicle.Components.AxleGearInputData);
			}
			else
			{
				Assert.IsNotNull(vehicle.Components.RetarderInputData);
				Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			}

			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);
			Assert.IsNull(vehicle.BoostingLimitations);
		}

		//[TestCase(@"PrimaryBus\HEV-S_primaryBus_IEPC-S_IEPC_Std.xml", BASE_DIR)]
		//public void TestHEVSPrimaryBusIEPCStd(string jobfile, string testDir)
		//{
		//	var filename = Path.Combine(testDir, jobfile);
		//	var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
		//	Assert.NotNull(dataProvider.JobInputData);
		//	var vehicle = dataProvider.JobInputData.Vehicle;

		//	var iepc = vehicle.Components.IEPC;
		//	Assert.IsNotNull(iepc);
		//	Assert.AreEqual(ElectricMachineType.ASM, iepc.ElectricMachineType);
		//	Assert.AreEqual(CertificationMethod.StandardValues, vehicle.Components.IEPC.CertificationMethod);

		//	Assert.AreEqual(1.SI<Watt>(), iepc.R85RatedPower);
		//	Assert.AreEqual(0.10.SI<KilogramSquareMeter>(), iepc.Inertia);//RotationalInertia
		//	Assert.AreEqual(200.00.SI<NewtonMeter>(), iepc.ContinuousTorque);
		//	Assert.AreEqual(2000.00.SI<PerSecond>(), iepc.ContinuousTorqueSpeed);//TestSpeedContinuousTorque
		//	Assert.AreEqual(400.00.SI<NewtonMeter>(), iepc.OverloadTorque);
		//	Assert.AreEqual(2000.00.SI<PerSecond>(), iepc.OverloadTestSpeed);//TestSpeedOverloadTorque
		//	Assert.AreEqual(30.00.SI<Second>(), iepc.OverloadTime);//OverloadDuration
		//	Assert.AreEqual(false, iepc.DifferentialIncluded);
		//	Assert.AreEqual(false, iepc.DesignTypeWheelMotor);
		//	Assert.AreEqual(1, iepc.NrOfDesignTypeWheelMotorMeasured);

		//	TestGearsData(iepc.Gears);
		//	Assert.AreEqual(1, iepc.VoltageLevels.Count);
		//	TestMaxTorqueCurve(iepc.VoltageLevels[0].FullLoadCurve);
		//	TestPowerMapData01(iepc.VoltageLevels[0].PowerMap[0]);
		//	TestDragCurve(iepc.DragCurves[0].DragCurve);
		//}



		[TestCase(@"HeavyLorry\HEV_heavyLorry_AMT_Px_IHPC.xml", BASE_DIR)]
		public void TestHEVHeavyLorryAMTPxIHPC(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;


			var eMachine = vehicle.Components.ElectricMachines.Entries.First();
			Assert.IsNotNull(eMachine);
			
			Assert.AreEqual(PowertrainPosition.HybridP2, eMachine.Position);
			Assert.AreEqual(1, eMachine.Count);

			Assert.AreEqual(ElectricMachineType.ASM, eMachine.ElectricMachine.ElectricMachineType);
			Assert.AreEqual(1.SI<Watt>(), eMachine.ElectricMachine.R85RatedPower);
			Assert.AreEqual(0.10.SI<KilogramSquareMeter>(), eMachine.ElectricMachine.Inertia);
			Assert.AreEqual(true, eMachine.ElectricMachine.DcDcConverterIncluded);
			Assert.AreEqual("IHPC Type 1", eMachine.ElectricMachine.IHPCType);

			Assert.AreEqual(2, eMachine.ElectricMachine.VoltageLevels.Count);
			Assert.AreEqual(400.SI<Volt>(), eMachine.ElectricMachine.VoltageLevels[0].VoltageLevel);
			Assert.AreEqual(2, eMachine.ElectricMachine.VoltageLevels[0].PowerMap.Count);
			Assert.AreEqual(1, eMachine.ElectricMachine.VoltageLevels[0].PowerMap[0].Gear);
			Assert.AreEqual(2, eMachine.ElectricMachine.VoltageLevels[0].PowerMap[1].Gear);

			Assert.AreEqual(600.SI<Volt>(), eMachine.ElectricMachine.VoltageLevels[1].VoltageLevel);
			Assert.AreEqual(2, eMachine.ElectricMachine.VoltageLevels[1].PowerMap.Count);
			Assert.AreEqual(1, eMachine.ElectricMachine.VoltageLevels[1].PowerMap[0].Gear);
			Assert.AreEqual(2, eMachine.ElectricMachine.VoltageLevels[1].PowerMap[1].Gear);

			Assert.AreEqual(2, eMachine.ElectricMachine.DragCurve.Rows.Count);
			Assert.AreEqual(1, eMachine.ElectricMachine.Conditioning.Rows.Count);
		}


		[TestCase(@"MediumLorry\PEV_mediumLorry_AMT_E2_EM_Std.xml", BASE_DIR)]
		public void TestPEVMediumLorryAMTE2EMStd(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			var eMachine = vehicle.Components.ElectricMachines.Entries[0];
			Assert.AreEqual(1, eMachine.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE2, eMachine.Position);
			
			Assert.AreEqual(ElectricMachineType.ASM, eMachine.ElectricMachine.ElectricMachineType);
			Assert.AreEqual(CertificationMethod.StandardValues, eMachine.ElectricMachine.CertificationMethod);
			Assert.AreEqual(1.SI<Watt>(), eMachine.ElectricMachine.R85RatedPower);
			Assert.AreEqual(0.10.SI<KilogramSquareMeter>(), eMachine.ElectricMachine.Inertia);//RotationalInertia
			Assert.AreEqual(true, eMachine.ElectricMachine.DcDcConverterIncluded);
			Assert.AreEqual("None", eMachine.ElectricMachine.IHPCType);
			
			Assert.AreEqual(1, eMachine.ElectricMachine.VoltageLevels.Count);
			TestMaxTorqueCurve(eMachine.ElectricMachine.VoltageLevels[0].FullLoadCurve);
			Assert.IsNotNull(eMachine.ElectricMachine.VoltageLevels[0].PowerMap);
			TestPowerMapData01(eMachine.ElectricMachine.VoltageLevels[0].PowerMap[0]);
			Assert.IsNull(eMachine.ElectricMachine.Conditioning);

			Assert.IsNotNull(eMachine.RatioPerGear);
		}



		


		#region Test existence of torque converter

		private void TestTorqueConverter(IVehicleDeclarationInputData vehicle)
		{
			var torqueConverter = vehicle.Components.TorqueConverterInputData;
			switch (vehicle.Components?.GearboxInputData?.Type)
			{
				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					Assert.IsNotNull(torqueConverter);
					break;
			}
		}

		#endregion
		
	}
}