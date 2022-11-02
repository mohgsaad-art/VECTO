using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLMultistageBusDataTest
	{
		protected IXMLInputDataReader _xmlInputReader;
		private IKernel _kernel;

		const string VIF01 =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\vecto_multistage_primary_vehicle_stage_2_3.xml";

		const string VIF02 =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\vecto_multistage_primary_vehicle_stage_2_full.xml";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}


		[TestCase]
		public void TestVehicleMultistageBusInput01()
		{
			var reader = XmlReader.Create(VIF01);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;
			TestPrimaryVehicleDataType(inputDataProvider.JobInputData.PrimaryVehicle);
			TestManufacturingStages(inputDataProvider.JobInputData.ManufacturingStages);
		}
		
		#region Primary Vehicle Test

		private void TestPrimaryVehicleDataType(IPrimaryVehicleInformationInputDataProvider primaryVehicle)
		{
			TestVehicleData(primaryVehicle.Vehicle);
			TestADASData(primaryVehicle.Vehicle.ADAS);
			TestTorqueLimitsData(primaryVehicle.Vehicle.TorqueLimits);
			TestComponentsData(primaryVehicle.Vehicle.Components);
			TestInputDataSignature(primaryVehicle.PrimaryVehicleInputDataHash);
			TestManufacturerRecordSignatureData(primaryVehicle.ManufacturerRecordHash);
			TestApplicationInformationData(primaryVehicle.ApplicationInformation);
			TestResultsData(primaryVehicle.ResultsInputData);
			TestSignatureHashData(primaryVehicle.VehicleSignatureHash);
		}


		#region Vehicle Test Methods

		private void TestVehicleData(IVehicleDeclarationInputData vehicleData)
		{
			Assert.AreEqual("Generic Truck Manufacturer", vehicleData.Manufacturer);
			Assert.AreEqual("Street, ZIP City", vehicleData.ManufacturerAddress);
			Assert.AreEqual("Generic Model", vehicleData.Model);
			Assert.AreEqual("VEH-1234567890_nonSmart-ESS", vehicleData.VIN);
			Assert.AreEqual(DateTime.Parse("2017-02-15T11:00:00Z").ToUniversalTime(), vehicleData.Date);
			Assert.AreEqual(LegislativeClass.M3, vehicleData.LegislativeClass);
			Assert.AreEqual("Bus", vehicleData.VehicleCategory.ToXMLFormat());
			Assert.AreEqual(AxleConfiguration.AxleConfig_8x2, vehicleData.AxleConfiguration);
			Assert.AreEqual(true, vehicleData.Articulated);
			Assert.AreEqual(4000, vehicleData.GrossVehicleMassRating.Value());
			Assert.AreEqual(600, vehicleData.EngineIdleSpeed.AsRPM);
			Assert.AreEqual("Transmission Output Retarder", vehicleData.Components.RetarderInputData.Type.ToXMLFormat());
			Assert.AreEqual("None", vehicleData.Components.AngledriveInputData.Type.ToXMLFormat());
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

		#endregion


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

		#region Components Data Tests

		#region Engine Data Test

		private void TestEngineData(IEngineDeclarationInputData engineData)
		{
			Assert.AreEqual("Generic Engine Manufacturer", engineData.Manufacturer);
			Assert.AreEqual("Bus 6x2", engineData.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", engineData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-02-15T11:00:00Z").ToUniversalTime(), engineData.Date);
			Assert.AreEqual("VectoEngine x.y", engineData.AppVersion);
			Assert.AreEqual(12730.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(), engineData.Displacement);
			Assert.AreEqual(1800, engineData.RatedSpeedDeclared.AsRPM);
			Assert.AreEqual(325032.SI<Watt>(), engineData.RatedPowerDeclared);
			Assert.AreEqual(2134, engineData.MaxTorqueDeclared.Value());
			Assert.AreEqual(WHRType.ElectricalOutput, engineData.WHRType);
			TestEngineModes(engineData.EngineModes);
		}

		private void TestEngineModes(IList<IEngineModeDeclarationInputData> engineModes)
		{
			Assert.AreEqual(1, engineModes.Count);
			Assert.AreEqual(600, engineModes[0].IdleSpeed.AsRPM);

			var fullLoadCurve = engineModes[0].FullLoadCurve;
			var entryIndex = 0;
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("600.00", "1188.00", "-138.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("800.00", "1661.00", "-143.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1000.00", "2134.00", "-152.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1200.00", "2134.00", "-165.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1400.00", "2134.00", "-187.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1600.00", "1928.00", "-217.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("1800.00", "1722.00", "-244.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("2000.00", "1253.00", "-278.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("2100.00", "1019.00", "-296.00", fullLoadCurve, ref entryIndex));
			Assert.IsTrue(CheckFullLoadAndDragCurveEntry("2200.00", "0.00", "-314.00", fullLoadCurve, ref entryIndex));

			Assert.AreEqual(1, engineModes[0].Fuels.Count);
			Assert.AreEqual(FuelType.DieselCI, engineModes[0].Fuels.First().FuelType);
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

		#region Transmission Data Test

		private void TestTransmissionData(IGearboxDeclarationInputData transmissionData)
		{
			Assert.AreEqual("Generic Gearbox Manufacturer", transmissionData.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck Gearbox", transmissionData.Model);
			Assert.AreEqual(CertificationMethod.StandardValues, transmissionData.CertificationMethod);
			Assert.AreEqual("Trans-5464sdf6sdf555", transmissionData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T11:00:00Z").ToUniversalTime(), transmissionData.Date);
			Assert.AreEqual("3.0.1", transmissionData.AppVersion);
			Assert.AreEqual(GearboxType.AMT, transmissionData.Type);
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
			Assert.AreEqual("Generic Torque Converter", torqueConvData.Manufacturer);
			Assert.AreEqual("Generic Torque Converter Model", torqueConvData.Model);
			Assert.AreEqual(CertificationMethod.StandardValues, torqueConvData.CertificationMethod);
			Assert.AreEqual("Torq-4546565455", torqueConvData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2018-01-12T12:00:00Z").ToUniversalTime(), torqueConvData.Date);
			Assert.AreEqual("3.0.3", torqueConvData.AppVersion);

			Assert.AreEqual(3, torqueConvData.TCData.Rows.Count);
			var rowIndex = 0;
			CheckTorqueConvertCharacteristics("0.0000", "1.00", "300.00", ref rowIndex, torqueConvData.TCData);
			CheckTorqueConvertCharacteristics("0.5000", "1.00", "200.00", ref rowIndex, torqueConvData.TCData);
			CheckTorqueConvertCharacteristics("0.9000", "0.90", "200.00", ref rowIndex, torqueConvData.TCData);
		}

		private void CheckTorqueConvertCharacteristics(string speedRatio, string torqueRation, string inputTorque,
			ref int rowIndex, TableData tableData)
		{
			Assert.AreEqual(speedRatio, tableData.Rows[rowIndex][0]);
			Assert.AreEqual(torqueRation, tableData.Rows[rowIndex][1]);
			Assert.AreEqual(inputTorque, tableData.Rows[rowIndex][2]);
			rowIndex++;
		}

		#endregion

		#region Angledrive Data Test

		private void TestAngledriveData(IAngledriveInputData angledriveData)
		{
			Assert.AreEqual("Generic Angledrive", angledriveData.Manufacturer);
			Assert.AreEqual("Generic Angledrive Model", angledriveData.Model);
			Assert.AreEqual(CertificationMethod.StandardValues, angledriveData.CertificationMethod);
			Assert.AreEqual("ANG-Z64665456654", angledriveData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2019-01-12T12:00:00Z").ToUniversalTime(), angledriveData.Date);
			Assert.AreEqual("3.2.3", angledriveData.AppVersion);
			Assert.AreEqual(20, angledriveData.Ratio);
		}

		#endregion

		#region Axelgear Test
		private void TestAxelgearData(IAxleGearInputData axelgearData)
		{
			Assert.AreEqual("Generic Gearbox Manufacturer", axelgearData.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck AxleGear", axelgearData.Model);
			Assert.AreEqual(CertificationMethod.StandardValues, axelgearData.CertificationMethod);
			Assert.AreEqual("AX-6654888s5f4", axelgearData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T11:00:00Z").ToUniversalTime(), axelgearData.Date);
			Assert.AreEqual("3.0.1", axelgearData.AppVersion);
			Assert.AreEqual(AxleLineType.SinglePortalAxle, axelgearData.LineType);
			Assert.AreEqual(2.590, axelgearData.Ratio);
		}

		#endregion

		#region AxleWheels Data Test
		private void TestAxelWheelsData(IAxlesDeclarationInputData axelWheels)
		{
			Assert.AreEqual(3, axelWheels.AxlesDeclaration.Count);

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
			Assert.AreEqual("Tyre Generation App 1.1", tyre2.AppVersion);
			Assert.AreEqual("315/70 R22.5", tyre2.Dimension);
			Assert.AreEqual(0.0063, tyre2.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre2.TyreTestLoad.Value());

			Assert.AreEqual("#WHL-5432198760-315-70-R22.5", tyre2.DigestValue.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", tyre2.DigestValue.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", tyre2.DigestValue.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", tyre2.DigestValue.DigestMethod);
			Assert.AreEqual("KljvtvGUUQ/L7MiLVAqU+bckL5PNDNNwdeLH9kUVrfM=", tyre2.DigestValue.DigestValue);

			entry = axelWheels.AxlesDeclaration[2];
			Assert.AreEqual(AxleType.VehicleNonDriven, entry.AxleType);
			Assert.AreEqual(false, entry.TwinTyres);

			var tyre3 = entry.Tyre;
			Assert.AreEqual("Generic Wheels Manufacturer", tyre3.Manufacturer);
			Assert.AreEqual("Generic Wheel", tyre3.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", tyre3.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T14:00:00Z").ToUniversalTime(), tyre3.Date);
			Assert.AreEqual("Tyre Generation App 1.1", tyre3.AppVersion);
			Assert.AreEqual("315/70 R22.5", tyre3.Dimension);
			Assert.AreEqual(0.0055, tyre3.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre3.TyreTestLoad.Value());

			Assert.AreEqual("#WHL-5432198760-315-70-R22.5", tyre3.DigestValue.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", tyre3.DigestValue.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", tyre3.DigestValue.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", tyre3.DigestValue.DigestMethod);
			Assert.AreEqual("4TkUGQTX8tevHOU9Cj9uyCFuI/aqcEYlo/gyVjVQmv0=", tyre3.DigestValue.DigestValue);
		}


		#endregion

		#region Auxiliaries Test
		private void TestAuxiliarieData(IBusAuxiliariesDeclarationData aux)
		{
			Assert.AreEqual("Hydraulic driven - Constant displacement pump", aux.FanTechnology);

			Assert.AreEqual(1, aux.SteeringPumpTechnology.Count);
			Assert.AreEqual("Variable displacement elec. controlled", aux.SteeringPumpTechnology[0]);

			Assert.AreEqual(0, aux.ElectricSupply.Alternators.Count);
			Assert.AreEqual(AlternatorType.Conventional, aux.ElectricSupply.AlternatorTechnology);
			//Assert.AreEqual(true, aux.ElectricSupply.SmartElectrics);
			//Assert.AreEqual(730, aux.ElectricSupply.Alternators.FirstOrDefault().RatedCurrent.Value());
			//Assert.AreEqual(48, aux.ElectricSupply.Alternators.FirstOrDefault().RatedVoltage.Value());
			//Assert.AreEqual(50.SI(Unit.SI.Watt.Hour).Cast<WattSecond>(), aux.ElectricSupply.ElectricStorage.FirstOrDefault().ElectricStorageCapacity);

			Assert.AreEqual("Large Supply 2-stage", aux.PneumaticSupply.CompressorSize);//SizeOfAirSupply
			Assert.AreEqual(CompressorDrive.electrically, aux.PneumaticSupply.CompressorDrive);
			Assert.AreEqual("none", aux.PneumaticSupply.Clutch);
			Assert.AreEqual(1.000, aux.PneumaticSupply.Ratio);
			Assert.AreEqual(true, aux.PneumaticSupply.SmartAirCompression);
			Assert.AreEqual(false, aux.PneumaticSupply.SmartRegeneration);
			Assert.AreEqual(ConsumerTechnology.Electrically, aux.PneumaticConsumers.AirsuspensionControl);
			Assert.AreEqual(ConsumerTechnology.Electrically, aux.PneumaticConsumers.AirsuspensionControl);
			Assert.AreEqual(ConsumerTechnology.Pneumatically, aux.PneumaticConsumers.AdBlueDosing);

			Assert.AreEqual(true, aux.HVACAux.AdjustableCoolantThermostat);
			Assert.AreEqual(true, aux.HVACAux.EngineWasteGasHeatExchanger);
		}

		#endregion

		#endregion

		#region Input Data Signature Test

		private void TestInputDataSignature(DigestData data)
		{
			Assert.AreEqual("#VEH-PrimaryBus_SmartPS", data.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", data.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", data.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", data.DigestMethod);
			Assert.AreEqual("uqcFIXtDYxvl513ruiYYJCrV1fIiyw37t8vJhg4xxoo=", data.DigestValue);
		}

		#endregion

		#region Input Data Manufacturer Signature Test

		private void TestManufacturerRecordSignatureData(DigestData data)
		{
			Assert.AreEqual("#RESULT-5f30c7fe665a47938f6b", data.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", data.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", data.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", data.DigestMethod);
			Assert.AreEqual("KUpFCKh1bu/YPwYj36kJK1uCrv++BTLf2OUZcOt43Os=", data.DigestValue);
		}

		#endregion

		#region Application Information Test

		private void TestApplicationInformationData(IApplicationInformation appInfo)
		{
			Assert.AreEqual("String", appInfo.SimulationToolVersion);
			Assert.AreEqual(DateTime.Parse("2017-01-01T00:00:00Z").ToUniversalTime(), appInfo.Date);
		}

		#endregion

		#region Results Data Test

		private void TestResultsData(IResultsInputData data)
		{
			Assert.AreEqual("success", data.Status);
			Assert.AreEqual(22, data.Results.Count);

			var index = 0;
			CheckResultData(VehicleClass.ClassP33SD, MissionType.HeavyUrban, 21.22359, data.Results[index]);
			CheckSimulationParameters(15527.52, 1352.52, 19.89, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33SD, MissionType.HeavyUrban, 25.40945, data.Results[index]);
			CheckSimulationParameters(20937.60, 6762.60, 99.45, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33SD, MissionType.Urban, 16.74101, data.Results[index]);
			CheckSimulationParameters(15527.52, 1352.52, 19.89, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33SD, MissionType.Urban, 20.24221, data.Results[index]);
			CheckSimulationParameters(20937.60, 6762.60, 99.45, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33SD, MissionType.Suburban, 14.34578, data.Results[index]);
			CheckSimulationParameters(15527.52, 1352.52, 19.89, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33SD, MissionType.Suburban, 17.60016, data.Results[index]);
			CheckSimulationParameters(20937.60, 6762.60, 99.45, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33SD, MissionType.Interurban, 11.82313, data.Results[index]);
			CheckSimulationParameters(15469.51, 1294.51, 18.23, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33SD, MissionType.Interurban, 13.24283, data.Results[index]);
			CheckSimulationParameters(18411.57, 4236.57, 59.67, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33DD, MissionType.HeavyUrban, 22.72091, data.Results[index]);
			CheckSimulationParameters(16303.29, 1578.29, 23.21, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33DD, MissionType.HeavyUrban, 27.95930, data.Results[index]);
			CheckSimulationParameters(22616.43, 7891.43, 116.05, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33DD, MissionType.Urban, 17.90756, data.Results[index]);
			CheckSimulationParameters(16303.29, 1578.29, 23.21, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33DD, MissionType.Urban, 22.23796, data.Results[index]);
			CheckSimulationParameters(22616.43, 7891.43, 116.05, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33DD, MissionType.Suburban, 15.28035, data.Results[index]);
			CheckSimulationParameters(16303.29, 1578.29, 23.21, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP33DD, MissionType.Suburban, 19.26201, data.Results[index]);
			CheckSimulationParameters(22616.43, 7891.43, 116.05, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP34SD, MissionType.Interurban, 11.93701, data.Results[index]);
			CheckSimulationParameters(16467.68, 1254.68, 17.67, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP34SD, MissionType.Interurban, 13.32341, data.Results[index]);
			CheckSimulationParameters(19319.21, 4106.21, 57.83, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP34SD, MissionType.Coach, 8.71847, data.Results[index]);
			CheckSimulationParameters(16490.49, 1277.49, 17.99, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP34SD, MissionType.Coach, 9.20255, data.Results[index]);
			CheckSimulationParameters(18406.72, 3193.72, 44.98, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP34DD, MissionType.Interurban, 13.58335, data.Results[index]);
			CheckSimulationParameters(19588.08, 1738.08, 24.48, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP34DD, MissionType.Interurban, 15.56740, data.Results[index]);
			CheckSimulationParameters(23643.60, 5793.60, 81.60, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP34DD, MissionType.Coach, 9.82937, data.Results[index]);
			CheckSimulationParameters(19703.95, 1853.95, 26.11, "single fuel mode", data.Results[index], ref index);

			CheckResultData(VehicleClass.ClassP34DD, MissionType.Coach, 10.56728, data.Results[index]);
			CheckSimulationParameters(22484.88, 4634.88, 65.28, "single fuel mode", data.Results[index], ref index);
		}

		private void CheckResultData(VehicleClass vGroup, MissionType mission, double energyCon, IResult result)
		{
			Assert.AreEqual(vGroup, result.VehicleGroup);
			Assert.AreEqual(mission, result.Mission);
			Assert.AreEqual(FuelType.DieselCI, result.EnergyConsumption.First().Key);
			energyCon = energyCon * 1000;
			Assert.AreEqual(energyCon.SI<JoulePerMeter>(), result.EnergyConsumption.First().Value);
		}

		private void CheckSimulationParameters(double totalMass, double payload, double passenger, string fuelMode, IResult result, ref int index)
		{
			Assert.AreEqual(totalMass.SI<Kilogram>(), result.SimulationParameter.TotalVehicleMass);
			Assert.AreEqual(payload.SI<Kilogram>(), result.SimulationParameter.Payload);
			Assert.AreEqual(passenger, result.SimulationParameter.PassengerCount);
			Assert.AreEqual(fuelMode, result.SimulationParameter.FuelMode);

			index++;
		}

		#endregion

		#region Signature Test

		private void TestSignatureHashData(DigestData data)
		{
			Assert.AreEqual("#PIF-d10aff76c5d149948046", data.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", data.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", data.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", data.DigestMethod);
			Assert.AreEqual("nI+57QQtWA2rFqJTZ41t0XrXcJbcGmc7j4E66iGJyT0=", data.DigestValue);
		}

		#endregion


		#endregion

		#region Manufacturing Stages Test

		private void TestManufacturingStages(IList<IManufacturingStageInputData> manufacturingStages)
		{
			Assert.AreEqual(2, manufacturingStages.Count);
			TestManufacturingStag01(manufacturingStages[0]);
			TestManufacturingStag02(manufacturingStages[1]);
		}
		

		#region Test Manufacturing Stage 1
		
		private void TestManufacturingStag01(IManufacturingStageInputData manufacturingStage)
		{
			Assert.AreEqual(2, manufacturingStage.StepCount);

			TestDigestData("#PIF-d10aff76c5d149948046", "nI+57QQtWA2rFqJTZ41t0XrXcJbcGmc7j4E66iGJyT0=",
				manufacturingStage.HashPreviousStep);

			TestVehicleDataStage01(manufacturingStage.Vehicle);

			TestApplicationInformationData("!!NOT FOR CERTIFICATION!!", "2021-01-12T07:20:08.0187663Z",
				manufacturingStage.ApplicationInformation);

			TestDigestData("#RESULT-6f30c7fe665a47938f6b", "BMpFCKh1bu/YPwYj37kJK1uCrv++BTLf2OUZcOt43Os=",
				manufacturingStage.Signature);
		}

		private void TestVehicleDataStage01(IVehicleDeclarationInputData vehicleData)
		{
			Assert.AreEqual("MSTG-1-Vehicle", vehicleData.Identifier);
			Assert.AreEqual("Intermediate Manufacturer 1", vehicleData.Manufacturer);
			Assert.AreEqual("Intermediate Manufacturer Address 1", vehicleData.ManufacturerAddress);
			Assert.AreEqual("VEH-1234567890", vehicleData.VIN);
			Assert.AreEqual(DateTime.Parse("2018-02-15T11:00:00Z").ToUniversalTime(), vehicleData.Date);
			Assert.AreEqual(VehicleDeclarationType.interim, vehicleData.VehicleDeclarationType);
			TestEmptyInterimStageInputFields(vehicleData);
		}

		private void TestEmptyInterimStageInputFields(IVehicleDeclarationInputData vehicleData)
		{
			Assert.AreEqual(null, vehicleData.Model);
			Assert.AreEqual(null, vehicleData.LegislativeClass);			
			Assert.AreEqual(null, vehicleData.CurbMassChassis);
			Assert.AreEqual(null, vehicleData.GrossVehicleMassRating);
			Assert.AreEqual(null, vehicleData.AirdragModifiedMultistep);
			Assert.AreEqual(null, vehicleData.TankSystem);
			Assert.AreEqual(null, vehicleData.RegisteredClass);
			Assert.AreEqual(null, vehicleData.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(null, vehicleData.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(null, vehicleData.VehicleCode);
			Assert.AreEqual(null, vehicleData.LowEntry);
			Assert.AreEqual(null, vehicleData.Height);
			Assert.AreEqual(null, vehicleData.Length);
			Assert.AreEqual(null, vehicleData.Width);
			Assert.AreEqual(null, vehicleData.EntranceHeight);
			Assert.AreEqual(null, vehicleData.DoorDriveTechnology);

			Assert.AreEqual(null, vehicleData.ADAS);
			Assert.AreEqual(null, vehicleData.Components);
		}
		
		#endregion


		#region Test manufacturing Stage 2

		private void TestManufacturingStag02(IManufacturingStageInputData manufacturingStage)
		{
			Assert.AreEqual(3, manufacturingStage.StepCount);

			TestDigestData("#RESULT-6f30c7fe665a47938f6b", "BMpFCKh1bu/YPwYj37kJK1uCrv++BTLf2OUZcOt43Os=",
				manufacturingStage.HashPreviousStep);

			TestVehicleDataStage02(manufacturingStage.Vehicle);
		}

		private void TestVehicleDataStage02(IVehicleDeclarationInputData vehicleData)
		{
			Assert.AreEqual("MSTG-2-Vehicle", vehicleData.Identifier);
			Assert.AreEqual("Intermediate Manufacturer 2", vehicleData.Manufacturer);
			Assert.AreEqual("Intermediate Manufacturer Address 2", vehicleData.ManufacturerAddress);
			Assert.AreEqual("VEH-2234567890", vehicleData.VIN);
			Assert.AreEqual(DateTime.Parse("2021-02-13T07:20:08.0187663Z").ToUniversalTime(), vehicleData.Date);
			Assert.AreEqual(null, vehicleData.AirdragModifiedMultistep);
			Assert.AreEqual(VehicleDeclarationType.final, vehicleData.VehicleDeclarationType);

			Assert.AreEqual(true, vehicleData.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.WithEngineStop, vehicleData.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.Option_1_2, vehicleData.ADAS.PredictiveCruiseControl);

			var busAux = vehicleData.Components.BusAuxiliaries;
			Assert.AreEqual(true, busAux.ElectricConsumers.InteriorLightsLED);
			Assert.AreEqual(true, busAux.ElectricConsumers.DayrunninglightsLED);
			Assert.AreEqual(false, busAux.ElectricConsumers.PositionlightsLED);
			Assert.AreEqual(false, busAux.ElectricConsumers.BrakelightsLED);
			Assert.AreEqual(true, busAux.ElectricConsumers.HeadlightsLED);

			var hvacAux = vehicleData.Components.BusAuxiliaries.HVACAux;
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration1, hvacAux.SystemConfiguration);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvacAux.HeatPumpTypeCoolingDriverCompartment);
			Assert.AreEqual(HeatPumpType.none, hvacAux.HeatPumpTypeHeatingDriverCompartment);
			//Assert.AreEqual(1, hvacAux.HeatPumpPassengerCompartments.Count);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvacAux.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.none, hvacAux.HeatPumpTypeHeatingPassengerCompartment);
			Assert.AreEqual(50.SI<Watt>(), hvacAux.AuxHeaterPower);
			Assert.AreEqual(false, hvacAux.DoubleGlazing);
			Assert.AreEqual(true, hvacAux.AdjustableAuxiliaryHeater);
			Assert.AreEqual(false, hvacAux.SeparateAirDistributionDucts);
			//Assert.AreEqual(false, hvacAux.WaterElectricHeater);
			//Assert.AreEqual(false, hvacAux.AirElectricHeater);
			//Assert.AreEqual(true, hvacAux.OtherHeatingTechnology);
		}

		#endregion
		
		private void TestApplicationInformationData(string toolVersion, string date,
			IApplicationInformation applicationInfoData)
		{
			Assert.AreEqual(toolVersion, applicationInfoData.SimulationToolVersion);
			Assert.AreEqual(DateTime.Parse(date).ToUniversalTime(), applicationInfoData.Date);
		}

		private void TestDigestData(string reference, string digestValue, DigestData digestData)
		{
			Assert.AreEqual(reference, digestData.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", digestData.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", digestData.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", digestData.DigestMethod);
			Assert.AreEqual(digestValue, digestData.DigestValue);
		}

		#endregion
		

		[TestCase]
		public void TestVehicleMultistageBusInput02()
		{
			var reader = XmlReader.Create(VIF02);
			var inputDataProvider = _xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;
			TestPrimaryVehicleDataType(inputDataProvider.JobInputData.PrimaryVehicle);
			TestManufacturingStageVIF02(inputDataProvider.JobInputData.ManufacturingStages);
		}
		
		#region Manufacutring Stage Test VIF02

		private void TestManufacturingStageVIF02(IList<IManufacturingStageInputData> manufacturingStages)
		{
			Assert.AreEqual(1, manufacturingStages.Count);
			
			var stageData = manufacturingStages[0];
			Assert.AreEqual(2, stageData.StepCount);

			TestDigestData("#PIF-d11aff76c5d149948046", "nI+67QQtWA2rFqJTZ41t0XrXcJbcGmc7j4E66iGJyT0=",
				stageData.HashPreviousStep);

			TestVehicleDataVIF02(stageData.Vehicle);

			TestApplicationInformationData("!!NOT FOR CERTIFICATION!!", "2021-01-12T07:20:08.0187666Z",
				stageData.ApplicationInformation);

			TestDigestData("#RESULT-6f30c7fe665a47938f6b", "BMpFCKh1bu/YPwYj37kJK1uCrv++BTLf2OUZcOt43Os=", stageData.Signature);
		}

		private void TestVehicleDataVIF02(IVehicleDeclarationInputData vehicleData)
		{
			Assert.AreEqual("MSTG-1-Vehicle", vehicleData.Identifier);
			Assert.AreEqual("Intermediate Manufacturer 1", vehicleData.Manufacturer);
			Assert.AreEqual("Intermediate Manufacturer Address 1", vehicleData.ManufacturerAddress);
			Assert.AreEqual("VIN-132564", vehicleData.VIN);
			Assert.AreEqual(DateTime.Parse("2018-01-15T11:00:00Z").ToUniversalTime(), vehicleData.Date);
			Assert.AreEqual("Intermediate Model 1", vehicleData.Model);
			Assert.AreEqual(LegislativeClass.M3, vehicleData.LegislativeClass);
			Assert.AreEqual(25000.SI<Kilogram>(), vehicleData.CurbMassChassis);
			Assert.AreEqual(30000.SI<Kilogram>(), vehicleData.GrossVehicleMassRating);
			Assert.AreEqual(null, vehicleData.AirdragModifiedMultistep);
			Assert.AreEqual(TankSystem.Liquefied, vehicleData.TankSystem);
			Assert.AreEqual(RegistrationClass.B, vehicleData.RegisteredClass);
			Assert.AreEqual(24, vehicleData.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(30, vehicleData.NumberPassengerSeatsUpperDeck);
			Assert.AreEqual(7, vehicleData.NumberPassengersStandingLowerDeck);
			Assert.AreEqual(4, vehicleData.NumberPassengersStandingUpperDeck);
			Assert.AreEqual(VehicleCode.CA, vehicleData.VehicleCode);
			Assert.AreEqual(true, vehicleData.LowEntry);
			Assert.AreEqual(2.5.SI<Meter>(), vehicleData.Height);
			Assert.AreEqual(12.SI<Meter>(), vehicleData.Length);
			Assert.AreEqual(2.SI<Meter>(), vehicleData.Width);
			Assert.AreEqual(2.05.SI<Meter>(), vehicleData.EntranceHeight);
			Assert.AreEqual(ConsumerTechnology.Pneumatically, vehicleData.DoorDriveTechnology);
			Assert.AreEqual(VehicleDeclarationType.final, vehicleData.VehicleDeclarationType);

			Assert.AreEqual(false, vehicleData.ADAS.EngineStopStart);
			Assert.AreEqual(EcoRollType.WithoutEngineStop, vehicleData.ADAS.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.None, vehicleData.ADAS.PredictiveCruiseControl);
			Assert.AreEqual(true, vehicleData.ADAS.ATEcoRollReleaseLockupClutch);


			TestVehicleComponentDataVIF02(vehicleData.Components);
		}

		private void TestVehicleComponentDataVIF02(IVehicleComponentsDeclaration componentData)
		{
			TestAirdragVIF02(componentData.AirdragInputData);
			TestBusAuxVIF02(componentData.BusAuxiliaries);
		}

		private void TestBusAuxVIF02(IBusAuxiliariesDeclarationData busAux)
		{
			Assert.AreEqual(true, busAux.ElectricConsumers.InteriorLightsLED);
			Assert.AreEqual(false, busAux.ElectricConsumers.DayrunninglightsLED);
			Assert.AreEqual(true, busAux.ElectricConsumers.PositionlightsLED);
			Assert.AreEqual(true, busAux.ElectricConsumers.BrakelightsLED);
			Assert.AreEqual(false, busAux.ElectricConsumers.HeadlightsLED);

			var hvacAux = busAux.HVACAux;
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration0, hvacAux.SystemConfiguration);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, hvacAux.HeatPumpTypeHeatingDriverCompartment);
			Assert.AreEqual(HeatPumpType.none, hvacAux.HeatPumpTypeCoolingDriverCompartment);
			//Assert.AreEqual(1, hvacAux.HeatPumpPassengerCompartments.Count);
			Assert.AreEqual(HeatPumpType.non_R_744_2_stage, hvacAux.HeatPumpTypeHeatingPassengerCompartment);
			Assert.AreEqual(HeatPumpType.none, hvacAux.HeatPumpTypeCoolingPassengerCompartment);
			Assert.AreEqual(40.SI<Watt>(), hvacAux.AuxHeaterPower);
			Assert.AreEqual(false, hvacAux.DoubleGlazing);
			Assert.AreEqual(false, hvacAux.AdjustableAuxiliaryHeater);
			Assert.AreEqual(true, hvacAux.SeparateAirDistributionDucts);
			//Assert.AreEqual(false, hvacAux.WaterElectricHeater);
			//Assert.AreEqual(true, hvacAux.AirElectricHeater);
			//Assert.AreEqual(false, hvacAux.OtherHeatingTechnology);
		}

		private void TestAirdragVIF02(IAirdragDeclarationInputData airdragData)
		{
			Assert.AreEqual("Generic Manufacturer", airdragData.Manufacturer);
			Assert.AreEqual("Generic Model", airdragData.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E1000*00", airdragData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2018-03-24T15:00:00Z").ToUniversalTime(), airdragData.Date);
			Assert.AreEqual("Vecto AirDrag x.y.z", airdragData.AppVersion);
			Assert.AreEqual(6.35, airdragData.AirDragArea.Value());

			TestDigestData("#CabinX23h", "b9SHCfOoVrBxFQ8wwDK32OO+9bd85DuaUdgs6j/29N8=", airdragData.DigestValue);
		}

		#endregion
	}
}
