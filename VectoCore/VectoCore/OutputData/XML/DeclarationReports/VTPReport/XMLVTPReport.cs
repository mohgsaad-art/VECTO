/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NLog.Config;
using NLog.Targets;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;
using NLog;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using LogManager = NLog.LogManager;

namespace TUGraz.VectoCore.OutputData.XML
{
	internal class XMLVTPReport : DeclarationReport<XMLVTPReport.ResultEntry>, IVTPReport
	{
		public const string CURRENT_SCHEMA_VERSION = "0.2";

		private const string VTPReportTartetName = "VTPReportTarget";

		protected XElement VehiclePart;
		protected XElement GeneralPart;
		protected XElement DataIntegrityPart;
		protected XElement TestConditionsPart;

		protected XElement ResultsPart;

		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		protected XNamespace rootNS = "urn:tugraz:ivt:VectoAPI:VTPReport";
		protected XNamespace tns = "urn:tugraz:ivt:VectoAPI:VTPReport:v" + CURRENT_SCHEMA_VERSION;

		private static List<string> LogList = new List<string>();
		private LoggingRule cycleChecksRule;

		protected VehicleClass VehicleClass = VehicleClass.Unknown;
		protected VehicleCode? VehicleCode = VectoCommon.Models.VehicleCode.NOT_APPLICABLE;

		//protected XNamespace di;
		//private bool allSuccess = true;

		public class ResultEntry : XMLDeclarationReport.ResultEntry
		{
			public Watt AverageFanPower;
			public Dictionary<FuelType, Kilogram> VTPFcFinalSimulated = new Dictionary<FuelType, Kilogram>();
			public WattSecond VTPWorkPWheelPos;
			public double VTPFcCorrectionFactor;
			public Dictionary<FuelType, Kilogram> VTPFcMeasured = new Dictionary<FuelType, Kilogram>();
			public WattSecond VTPWorPWheelSimPos;
			public TankSystem? TankSystem;

			#region Overrides of ResultEntry

			public override void SetResultData(VectoRunData runData, IModalDataContainer data, double factor)
			{
				base.SetResultData(runData, data, factor);

				if (runData.SimulationType != SimulationType.VerificationTest) {
					return;
				}

				TankSystem = runData.VehicleData.InputData.TankSystem;
				var aux = data.Auxiliaries.FirstOrDefault(x => x.Key == Constants.Auxiliaries.IDs.Fan);
				AverageFanPower = data.AuxiliaryWork(aux.Value) / data.Duration;
				var cycleEntries = runData.Cycle.Entries.Pairwise().Select(
					x => new {
						PWheel = x.Item1.PWheel > 0 ? x.Item1.PWheel : 0.SI<Watt>(),
						dt = x.Item2.Time - x.Item1.Time,
						FC = x.Item1.VTPFuelconsumption
					}).ToArray();
				VTPWorPWheelSimPos = data.WorkWheelsPos();
				VTPWorkPWheelPos = cycleEntries.Sum(x => x.PWheel * x.dt).Cast<WattSecond>();
				foreach (var fuel in cycleEntries.First().FC.Keys) {
					VTPFcMeasured[fuel] = cycleEntries.Sum(x => x.FC[fuel] * x.dt);
				}
				//VTPFcMeasured = cycleEntries.Sum(x => x.FC * x.dt).Cast<Kilogram>();

				foreach (var entry in data.FuelData) {
					var col = data.GetColumnName(entry, ModalResultField.FCFinal);
					var fcSum = data.TimeIntegral<Kilogram>(col);
					VTPFcFinalSimulated[entry.FuelType] = fcSum;
				}

				//VTPFcFinalSimulated = data.TimeIntegral<Kilogram>(ModalResultField.FCFinal);
				VTPFcCorrectionFactor = runData.VTPData.CorrectionFactor;
			}

			#endregion
		}

		public XMLVTPReport(IReportWriter writer) : base(writer)
		{
			//di = "http://www.w3.org/2000/09/xmldsig#";

			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			GeneralPart = new XElement(tns + "General");
			DataIntegrityPart = new XElement(tns + "DataIntegrityCheck");
			TestConditionsPart = new XElement(tns + "TestConditions");
			ResultsPart = new XElement(tns + "Results");

			AddLogging();
		}

		private void AddLogging()
		{
			LogList.Clear();
			var target = new MethodCallTarget {
				ClassName = typeof(XMLVTPReport).AssemblyQualifiedName,
				MethodName = "LogMethod"
			};
			target.Parameters.Add(new MethodCallParameter("${level}"));
			target.Parameters.Add(new MethodCallParameter("${message}"));
			var config = LogManager.Configuration;
			if (config == null) {
				config = new LoggingConfiguration();
				LogManager.Configuration = config;
			}
			cycleChecksRule = new LoggingRule(typeof(VTPCycle).FullName, LogLevel.Error, target);
			config.AddTarget(VTPReportTartetName, target);
			config.LoggingRules.Add(cycleChecksRule);
			LogManager.Configuration.Reload();
		}

		// ReSharper disable once UnusedMember.Global -- see AddLogging Method
		public static void LogMethod(string level, string message)
		{
			LogList.Add(message);
		}

		#region Overrides of DeclarationReport<ResultEntry>

		protected override void DoStoreResult(
			ResultEntry entry, VectoRunData runData, IModalDataContainer modData)
		{
			entry.SetResultData(runData, modData, 0.0);
		}

		protected internal override void DoWriteReport()
		{
			var config = LogManager.Configuration;
			config.LoggingRules.Remove(cycleChecksRule);
			config.RemoveTarget(VTPReportTartetName);
			LogManager.Configuration.Reload();

			GenerateResults();

			var report = GenerateReport();
			if (Writer != null) {
				Writer.WriteReport(ReportType.DeclarationVTPReportXML, report);
			}
		}

		protected override void OutputReports()
		{
			throw new NotImplementedException();
		}

		protected override void GenerateReports()
		{
			throw new NotImplementedException();
		}

		protected override void WriteResult(ResultEntry result)
		{
			throw new NotImplementedException();
		}

		private void GenerateResults()
		{
			//var vtpResult = Missions.First().Value.FirstOrDefault(x => x.Key == MissionType.VerificationTest).Value.ResultEntry
			//						.FirstOrDefault().Value;
			var vtpResult = Results.OrderBy(x => x.FuelMode).FirstOrDefault(x => x.Mission == MissionType.VerificationTest);

			if (vtpResult == null) {
				throw new VectoException("no vtp result found for generating vtp report");
			}

			var selectedMission = VehicleClass.IsBus()
				? (VehicleCode.GetFloorType() == FloorType.LowFloor
					? DeclarationData.VTPMode.SelectedMissionLowFloorBus
					: DeclarationData.VTPMode.SelectedMissionHighFloorBus)
				: (vtpResult.VehicleClass.IsMediumLorry()
					? DeclarationData.VTPMode.SelectedMissionMediumLorry
					: DeclarationData.VTPMode.SelectedMissionHeavyLorry);
			const LoadingType selectedLoading = DeclarationData.VTPMode.SelectedLoading;
			//var result = Results.OrderBy(x => x.FuelMode).FirstOrDefault(x => x.Mission == selectedMission && x.LoadingType == selectedLoading);
			//var vtpFcSimulated = vtpResult.VTPFcFinalSimulated / vtpResult.FuelData.HeatingValueCorrection / vtpResult.VTPWorPWheelSimPos;
			//var declaredCO2 = result.FuelConsumptionTotal * result.FuelData.CO2PerFuelWeight / result.Distance / result.Payload;


			var result = ManufacturerRecord.Results.Results.Where(x => x.Mission == selectedMission)
											.MaxBy(x => x.SimulationParameter.Payload);
			var key = VehicleClass.IsBus() ? "g/p-km" : "g/t-km";
			var declaredCO2 = result.CO2[key];

			if (result == null) {
				throw new VectoException("no corresponding simulation result found for generating vtp report");
			}

			var vtpFcMeasured = vtpResult.VTPFcMeasured.Select(x => Tuple.Create(x.Key, x.Value / vtpResult.VTPWorkPWheelPos)).ToDictionary(x => x.Item1, x => x.Item2);
			var vtpFcMeasuredCorr = vtpResult.VTPFcMeasured
				.Select(x =>
					Tuple.Create(x.Key, x.Value / vtpResult.VTPWorkPWheelPos * vtpResult.VTPFcCorrectionFactor))
				.ToDictionary(x => x.Item1, x => x.Item2);
			var vtpFcSimulated = vtpResult.VTPFcFinalSimulated
				.Select(x => Tuple.Create(x.Key, x.Value / vtpResult.VTPWorPWheelSimPos))
				.ToDictionary(x => x.Item1, x => x.Item2);
			var fuels = DeclarationData.FuelData;
			var cVtp =
				vtpFcMeasuredCorr.Sum(e => e.Value * fuels.Lookup(e.Key, vtpResult.TankSystem).CO2PerFuelWeightVTP) /
				vtpFcSimulated.Sum(e => e.Value * fuels.Lookup(e.Key, vtpResult.TankSystem).CO2PerFuelWeightVTP);

			//var declaredCO2 =
			//	result.FuelConsumptionFinal.Sum(x => x.Value * fuels.Lookup(x.Key, vtpResult.TankSystem).CO2PerFuelWeightVTP) /
			//	result.Distance / result.Payload;
			var verifiedCO2 = declaredCO2 * cVtp.Value();

			ResultsPart.Add(
				new XElement(tns + "Status", cVtp < 1.075 ? "Passed" : "Failed"),
				new XElement(
					tns + "AverageFanPower",
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "kW"),
					vtpResult.AverageFanPower.ConvertToKiloWatt().ToXMLFormat(3)),
				new XElement(
					tns + "WorkPosVT", new XAttribute(XMLNames.Report_Results_Unit_Attr, "kWh"),
					vtpResult.VTPWorkPWheelPos.ConvertToKiloWattHour().ToXMLFormat(3)),
				vtpFcMeasured.Select(x =>
				new XElement(
					tns + "FuelConsumption",
					new XAttribute("fuelType", x.Key.ToXMLFormat()),
					new XElement(
						tns + "Measured",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/kWh"),
						vtpFcMeasured[x.Key].ConvertToGramPerKiloWattHour().ToXMLFormat(3)
					),
					new XElement(
						tns + "MeasuredCorrected",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/kWh"),
						vtpFcMeasuredCorr[x.Key].ConvertToGramPerKiloWattHour().ToXMLFormat(3)
					),
					new XElement(
						tns + "Simulated",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/kWh"),
						vtpFcSimulated[x.Key].ConvertToGramPerKiloWattHour().ToXMLFormat(3)
					)
				)),
				new XElement(
					tns + "CO2",
					new XElement(
						tns + "Mission",
						$"{selectedMission.ToXMLFormat()}, {selectedLoading.ToString()}"
					),
					new XElement(
						tns + "Declared", new XAttribute(XMLNames.Report_Results_Unit_Attr, key),
						declaredCO2.ToMinSignificantDigits(3, 2)
					),
					new XElement(
						tns + "Verified", new XAttribute(XMLNames.Report_Results_Unit_Attr, key),
						verifiedCO2.ToMinSignificantDigits(3, 2)
					)
				),
				new XElement(tns + "C_VTP", cVtp.ToXMLFormat(4)));
			if (LogList.Any()) {
				ResultsPart.Add(new XElement(tns + "Warnings", LogList.Select(x => new XElement(tns + "Warning", x))));
			}
		}

		private XDocument GenerateReport()
		{

			var retVal = new XDocument();
			retVal.Add(
				new XProcessingInstruction(
					"xml-stylesheet", "href=\"https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(
				new XElement(
					rootNS + "VectoVTPReport",
					//new XAttribute("schemaVersion", CURRENT_SCHEMA_VERSION),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
					new XAttribute("xmlns", tns),
					new XAttribute(XNamespace.Xmlns + "tns", rootNS),

					//new XAttribute(XNamespace.Xmlns + "di", di),
					new XAttribute(
						xsi + "schemaLocation",
						$"{rootNS} {AbstractXMLWriter.SchemaLocationBaseUrl}VTPReport.xsd"),
					new XElement(
						rootNS + "Data",
						new XAttribute(xsi + "type", "VTPReportDataType"),
						new XElement(GeneralPart),
						new XElement(VehiclePart),
						new XElement(DataIntegrityPart),
						new XElement(TestConditionsPart),
						new XElement(ResultsPart),
						GetApplicationInfo()
					)
				)
			);

			return retVal;
		}

		public override void InitializeReport(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehicleClass = modelData.VehicleData.VehicleClass;
			if (VehicleClass.IsBus()) {
				VehicleCode = modelData.VehicleData.VehicleCode;
			}
			GeneralPart.Add(
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress));
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleType"),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, modelData.VehicleData.GrossVehicleMass.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_CurbMassChassis, modelData.VehicleData.CurbMass.ToXMLFormat(0)),
				modelData.Retarder.Type.IsDedicatedComponent()
					? new XElement(tns + XMLNames.Vehicle_RetarderRatio, modelData.Retarder.Ratio.ToXMLFormat(3))
					: null,
				new XElement(tns + XMLNames.Vehicle_PTO, modelData.PTO != null));
			if (modelData.VehicleData.AxleConfiguration.AxlegearIncludedInGearbox()) {
				VehiclePart.Add(
					new XElement(
						tns + XMLNames.Vehicle_Components,
						new XAttribute(xsi + "type", "ComponentsTruckFWDType"),
						GetEngineDescription(modelData.EngineData, fuelModes),
						GetGearboxDescription(modelData.GearboxData, modelData.AxleGearData.AxleGear.Ratio),
						GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
						GetRetarderDescription(modelData.Retarder),
						GetAngledriveDescription(modelData.AngledriveData),
						GetAirDragDescription(modelData.AirdragData),
						GetAxleWheelsDescription(modelData.VehicleData),
						GetAuxiliariesDescription(modelData.Aux)
					));
			} else {
				VehiclePart.Add(
					new XElement(
						tns + XMLNames.Vehicle_Components,
						new XAttribute(xsi + "type", "ComponentsTruckType"),
						GetEngineDescription(modelData.EngineData, fuelModes),
						GetGearboxDescription(modelData.GearboxData),
						GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
						GetRetarderDescription(modelData.Retarder),
						GetAngledriveDescription(modelData.AngledriveData),
						GetAxlegearDescription(modelData.AxleGearData),
						GetAirDragDescription(modelData.AirdragData),
						GetAxleWheelsDescription(modelData.VehicleData),
						GetAuxiliariesDescription(modelData.Aux)
					));
			}

			if (InputDataHash == null) {
				return;
			}

			var allSuccess = true;

			var componentChecks = ComponentIntegrityChecks(ref allSuccess);
			var jobIntegrity = JobIntegrityChecks(ref allSuccess);
			var manufacturerReportIntegrity = ManufacturerReportIntegrityChecks(ref allSuccess);

			DataIntegrityPart.Add(
				new XAttribute("status", allSuccess ? "success" : "failed"),
				new XElement(
					tns + "Components",
					componentChecks.ToArray()
				),
				manufacturerReportIntegrity,
				jobIntegrity
			);
		}

		private List<object> ComponentIntegrityChecks(ref bool allSuccess)
		{
			var componentChecks = new List<object>();
			var components = InputDataHash.GetContainigComponents().GroupBy(s => s)
										.Select(g => new { Entry = g.Key, Count = g.Count() });
			foreach (var component in components) {
				if (component.Entry == VectoComponents.Vehicle) {
					continue;
				}

				for (var i = 0; i < component.Count; i++) {
					componentChecks.Add(CheckComponent(ref allSuccess, component.Entry, i, component.Count));
				}
			}

			return componentChecks;
		}

		private XElement CheckComponent(ref bool allSuccess, VectoComponents component, int i, int count)
		{
			bool status;
			var componentName = count == 1
				? VectoComponentsExtensionMethods.XMLElementName(component)
				: $"{VectoComponentsExtensionMethods.XMLElementName(component)} ({i + 1})";
			XElement retVal;
			try {
				var recomputed = InputDataHash.ComputeHash(component, i);
				var readJob = InputDataHash.ReadHash(component, i);
				var read = ManufacturerRecord.ComponentDigests[component][i];
				status = string.Equals(readJob, recomputed) && string.Equals(recomputed, read);
				retVal = new XElement(
						tns + "Component",
						new XAttribute("componentName", componentName),
						new XAttribute("status", status ? "success" : "failed"),
						new XElement(tns + "DigestValueRecomputed", recomputed),
						new XElement(
							tns + "DigestValueRead",
							new XAttribute("source", "JobData"),
							readJob
						),
						new XElement(
							tns + "DigestValueRead",
							new XAttribute("source", "ManufacturerRecord"),
							read
						)
					);
			} catch (Exception e) {
				status = false;
				retVal = new XElement(
						tns + "Component",
						new XAttribute("componentName", componentName),
						new XAttribute("status", "failed"),
						new XElement(tns + "Error", e.Message));
			}
			allSuccess = allSuccess && status;
			return retVal;
		}

		private XElement ManufacturerReportIntegrityChecks(ref bool allSuccess)
		{
			bool mrStatus;
			XElement manufacturerReportIntegrity;
			try {
				var mrHashRead = ManufacturerRecordHash.ReadHash();
				var mrHashRecomputed = ManufacturerRecordHash.ComputeHash();
				mrStatus = ManufacturerRecordHash.ValidateHash();
				manufacturerReportIntegrity = new XElement(
					tns + "ManufacturerReport",
					new XAttribute("status", mrStatus ? "success" : "failed"),
					new XElement(tns + "DigestValueRecomputed", mrHashRecomputed),
					new XElement(
						tns + "DigestValueRead",
						new XAttribute("source", "ManufacturerRecord"), mrHashRead)
				);
			} catch (Exception e) {
				mrStatus = false;
				var mrError = e.Message;
				manufacturerReportIntegrity = new XElement(
					tns + "ManufacturerReport",
					new XAttribute("status", "failed"),
					new XElement(tns + "Error", mrError)
				);
			}
			allSuccess = allSuccess && mrStatus;
			return manufacturerReportIntegrity;
		}

		private XElement JobIntegrityChecks(ref bool allSuccess)
		{
			bool jobStatus;
			XElement jobIntegrity;
			try {
				var jobHashMethods = ManufacturerRecord.JobDigest;
				var jobHashRecomputed = InputDataHash.ComputeHash(
					jobHashMethods.CanonicalizationMethods, jobHashMethods.DigestMethod);
				var jobHashRead = jobHashMethods.DigestValue;
				jobStatus = string.Equals(jobHashRecomputed, jobHashRead);
				jobIntegrity = new XElement(
					tns + "JobData",
					new XAttribute("status", jobStatus ? "success" : "failed"),
					new XElement(
						tns + "DigestValueRecomputed",
						jobHashRecomputed),
					new XElement(
						tns + "DigestValueRead",
						new XAttribute("source", "ManufacturerRecord"),
						jobHashRead)
				);
			} catch (Exception e) {
				jobStatus = false;
				var jobError = e.Message;
				jobIntegrity = new XElement(
					tns + "JobData",
					new XAttribute("status", "failed"),
					new XElement(tns + "Error", jobError)
				);
			}
			allSuccess = allSuccess && jobStatus;
			return jobIntegrity;
		}

		#endregion

		private XElement GetApplicationInfo()
		{
			return new XElement(
				tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, VectoSimulationCore.VersionNumber),
				new XElement(
					tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		private XElement GetEngineDescription(CombustionEngineData engineData, List<List<FuelData.Entry>> fuelModes)
		{
			return new XElement(
				tns + XMLNames.Component_Engine,
				GetCommonDescription(engineData),
				new XElement(tns + XMLNames.Engine_RatedPower, engineData.RatedPowerDeclared.ToXMLFormat(0)),
				new XElement(
					tns + XMLNames.Engine_Displacement,
					engineData.Displacement.ConvertToCubicCentiMeter().ToXMLFormat(0)),
				fuelModes.SelectMany(x => x.Select(f => f.FuelType.ToXMLFormat())).Distinct().Select(
					x => new XElement(tns + XMLNames.Engine_FuelType, x))
			);
		}

		private XElement GetGearboxDescription(GearboxData gearboxData)
		{
			return new XElement(
				tns + XMLNames.Component_Gearbox,
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, gearboxData.Gears.Count),
				new XElement(
					tns + XMLNames.Report_Gearbox_TransmissionRatioFinalGear,
					gearboxData.Gears.Last().Value.Ratio.ToXMLFormat(3))
			);
		}

		private XElement GetGearboxDescription(GearboxData gearboxData, double axlegearRatio)
		{
			return new XElement(
				tns + XMLNames.Component_Gearbox,
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, gearboxData.Gears.Count),
				new XElement(tns + XMLNames.Gearbox_AxlegearRatio, axlegearRatio.ToXMLFormat(3)),
				new XElement(
					tns + XMLNames.Report_Gearbox_TransmissionRatioFinalGear,
					gearboxData.Gears.Last().Value.Ratio.ToXMLFormat(3))
			);
		}

		private XElement GetTorqueConverterDescription(TorqueConverterData torqueConverterData)
		{
			if (torqueConverterData == null) {
				return null;
			}

			return new XElement(
				tns + XMLNames.Component_TorqueConverter,
				GetCommonDescription(torqueConverterData));
		}

		private XElement GetRetarderDescription(RetarderData retarder)
		{
			return new XElement(
				tns + XMLNames.Component_Retarder,
				new XElement(tns + XMLNames.Vehicle_RetarderType, retarder.Type.ToXMLFormat()),
				retarder.Type.IsDedicatedComponent() ? GetCommonDescription(retarder) : null);
		}

		private object GetAngledriveDescription(AngledriveData angledriveData)
		{
			if (angledriveData == null) {
				return null;
			}

			return new XElement(
				tns + XMLNames.Component_Angledrive,
				GetCommonDescription(angledriveData),
				new XElement(tns + XMLNames.AngleDrive_Ratio, angledriveData.Angledrive.Ratio));
		}

		private XElement GetAxlegearDescription(AxleGearData axleGearData)
		{
			return new XElement(
				tns + XMLNames.Component_Axlegear,
				GetCommonDescription(axleGearData),
				new XElement(tns + XMLNames.Axlegear_LineType, axleGearData.LineType.ToXMLFormat()),
				new XElement(tns + XMLNames.Axlegear_Ratio, axleGearData.AxleGear.Ratio.ToXMLFormat(3)));
		}

		private XElement GetAirDragDescription(AirdragData airdragData)
		{
			if (airdragData.CertificationMethod == CertificationMethod.StandardValues) {
				return new XElement(
					tns + XMLNames.Component_AirDrag,
					new XElement(tns + XMLNames.Report_Component_CertificationMethod, airdragData.CertificationMethod.ToXMLFormat()),
					new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragArea.ToXMLFormat(2))
				);
			}

			return new XElement(
				tns + XMLNames.Component_AirDrag,
				new XElement(tns + XMLNames.Component_Model, airdragData.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationMethod, airdragData.CertificationMethod.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, airdragData.CertificationNumber),
				new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragArea.ToXMLFormat(2))
			);
		}

		private XElement GetAxleWheelsDescription(VehicleData vehicleData)
		{
			var retVal = new XElement(tns + XMLNames.Component_AxleWheels);
			var axleData = vehicleData.AxleData;
			for (var i = 0; i < axleData.Count; i++) {
				if (axleData[i].AxleType == AxleType.Trailer) {
					continue;
				}

				retVal.Add(GetAxleDescription(i + 1, axleData[i]));
			}

			return retVal;
		}

		private XElement GetAxleDescription(int i, Axle axle)
		{
			return new XElement(
				tns + XMLNames.AxleWheels_Axles_Axle,
				new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, i),
				new XElement(tns + XMLNames.Report_Tyre_TyreDimension, axle.WheelsDimension),
				new XElement(tns + XMLNames.Component_CertificationNumber, axle.CertificationNumber),
				new XElement(tns + XMLNames.Report_Tyre_TyreRRCDeclared, axle.RollResistanceCoefficient.ToXMLFormat(4)),
				new XElement(tns + XMLNames.AxleWheels_Axles_Axle_TwinTyres, axle.TwinTyres));
		}

		private XElement GetAuxiliariesDescription(IEnumerable<VectoRunData.AuxData> aux)
		{
			var auxData = aux.ToDictionary(a => a.ID);
			var auxList = new[] {
				AuxiliaryType.Fan, AuxiliaryType.SteeringPump, AuxiliaryType.ElectricSystem, AuxiliaryType.PneumaticSystem,
				AuxiliaryType.HVAC
			};
			var retVal = new XElement(tns + XMLNames.Component_Auxiliaries);
			foreach (var auxId in auxList) {
				if (auxData.TryGetValue(auxId.Key(), out var auxValue)) {
					foreach (var entry in auxValue.Technology) {
						retVal.Add(new XElement(tns + GetTagName(auxId), entry));
					}
				}
			}
			return retVal;
		}

		private string GetTagName(AuxiliaryType auxId)
		{
			return auxId + "Technology";
		}

		private object[] GetCommonDescription(CombustionEngineData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
			};
		}

		private object[] GetCommonDescription(SimulationComponentData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationMethod, data.CertificationMethod.ToXMLFormat()),
				data.CertificationMethod == CertificationMethod.StandardValues
					? null
					: new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
			};
		}

		#region Implementation of IVTPReport

		public IVectoHash InputDataHash { protected get; set; }

		public IManufacturerReport ManufacturerRecord { protected get; set; }

		public IVectoHash ManufacturerRecordHash { protected get; set; }

		#endregion
	}
}
