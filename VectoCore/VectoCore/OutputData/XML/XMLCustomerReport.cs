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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLCustomerReport
	{
		public const string SCHEMA_VERSION_STRING = "0.7";
		public const string CURRENT_SCHEMA_VERSION = "0.7.2";

		protected readonly XElement VehiclePart;

		protected XElement InputDataIntegrity;

		protected readonly XElement Results;

		protected readonly XNamespace tns;
		protected readonly XNamespace di;

		private bool _allSuccess = true;

		private KilogramPerMeter _weightedCo2 = 0.SI<KilogramPerMeter>();

		private Kilogram _weightedPayload = 0.SI<Kilogram>();


		public XMLCustomerReport(XMLDeclarationReport xmlDeclarationReport)
		{
			DeclarationReport = xmlDeclarationReport;
			di = "http://www.w3.org/2000/09/xmldsig#";
			tns = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v" + CURRENT_SCHEMA_VERSION;
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + XMLNames.Report_Results);
		}

		public XMLDeclarationReport DeclarationReport { get; }

		public void Initialize(VectoRunData modelData)
		{
			var exempted = modelData.Exempted;
			VehiclePart.Add(
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_t, 1)),
				new XElement(tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				new XElement(tns + XMLNames.Vehicle_HybridElectricHDV, modelData.VehicleData.HybridElectricHDV),
				new XElement(tns + "WasteHeatRecovery", modelData.EngineData.WHRType != WHRType.None),
				new XElement(tns + XMLNames.Vehicle_DualFuelVehicle, modelData.EngineData.Fuels.Count > 1),

				exempted ? ExemptedData(modelData) : new[] {
					new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
					new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
					new XElement(tns + XMLNames.Report_CO2StandardsGroup, DeclarationReport.WeightingGroup.ToXMLFormat()),
					new XElement(tns + XMLNames.Vehicle_VocationalVehicle, modelData.VehicleData.VocationalVehicle),
					new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab),
					GetADAS(modelData.VehicleData.InputData.ADAS)
				}.Concat(ComponentData(modelData))
				);
			if (exempted) {
				Results.Add(new XElement(tns + XMLNames.Report_ExemptedVehicle));
			}
			InputDataIntegrity = new XElement(tns + XMLNames.Report_InputDataSignature,
				modelData.InputDataHash == null ? CreateDummySig() : new XElement(modelData.InputDataHash));
		}

		private XElement[] ExemptedData(VectoRunData modelData)
		{
			return new[] {
				modelData.VehicleData.HybridElectricHDV ? new XElement(tns + XMLNames.Vehicle_MaxNetPower1, XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower1, XMLNames.Unit_W)) : null,
				modelData.VehicleData.HybridElectricHDV ? new XElement(tns + XMLNames.Vehicle_MaxNetPower2, XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower2, XMLNames.Unit_W)) : null
			};
		}

		private XElement GetADAS(IAdvancedDriverAssistantSystemDeclarationInputData adasData)
		{
			return new XElement(tns + XMLNames.Vehicle_ADAS,
								new XElement(tns + XMLNames.Vehicle_ADAS_EngineStopStart, adasData.EngineStopStart),
								new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adasData.EcoRoll.WithoutEngineStop()),
								new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adasData.EcoRoll.WithEngineStop()),
								new XElement(tns + XMLNames.Vehicle_ADAS_PCC, adasData.PredictiveCruiseControl != PredictiveCruiseControlType.None)
			);
		}

		private XElement[] ComponentData(VectoRunData modelData)
		{
			var retVal = new List<XElement>() {
				new XElement(
					tns + XMLNames.Report_Vehicle_EngineRatedPower,
					XMLHelper.ValueAsUnit(modelData.EngineData.RatedPowerDeclared, XMLNames.Unit_kW)),
				new XElement(
					tns + XMLNames.Report_Vehicle_EngineDisplacement,
					XMLHelper.ValueAsUnit(modelData.EngineData.Displacement, XMLNames.Unit_ltr, 1)),
				// TODO: MQ 20211004 (merging ADAS idl, WHR, DF) - should be the following, but is not supported by XML schema
				//new XElement(
				//	tns + XMLNames.Report_Vehicle_FuelTypes,
				//	fuelModes.SelectMany(x => x.Select(f => f.FuelType.ToXMLFormat())).Distinct()
				//		.Select(x => new XElement(tns + XMLNames.Engine_FuelType, x))
				//),
			};
			retVal.AddRange(modelData.EngineData.Fuels.Select(x =>
				new XElement(tns + XMLNames.Engine_FuelType, x.FuelData.FuelType.ToXMLFormat())));
			retVal.AddRange(new [] {
				new XElement(
					tns + XMLNames.Report_Vehicle_TransmissionCertificationMethod,
					modelData.GearboxData.CertificationMethod.ToXMLFormat()),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, modelData.GearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, modelData.GearboxData.Gears.Count),
				new XElement(tns + XMLNames.Report_Vehicle_Retarder, modelData.Retarder.Type.IsDedicatedComponent()),
				new XElement(tns + XMLNames.Report_Vehicle_AxleRatio, modelData.AxleGearData.AxleGear.Ratio.ToXMLFormat(3)),
				new XElement(
					tns + XMLNames.Report_Vehicle_AverageRRC, modelData.VehicleData.AverageRollingResistanceTruck.ToXMLFormat(4)),
				new XElement(
					tns + XMLNames.Report_Vehicle_AverageRRCLabel,
					DeclarationData.Wheels.TyreClass.Lookup(modelData.VehicleData.AverageRollingResistanceTruck))
				});
			return retVal.ToArray();
		}

		private XElement CreateDummySig()
		{
			return new XElement(di + XMLNames.DI_Signature_Reference,
				new XElement(di + XMLNames.DI_Signature_Reference_DigestMethod,
					new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "null")),
				new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, "NOT AVAILABLE")
				);
		}

		public void WriteResult(
			DeclarationReport<XMLDeclarationReport.ResultEntry>.ResultContainer<XMLDeclarationReport.ResultEntry> entry)
		{
			foreach (var resultEntry in entry.ResultEntry) {
				_allSuccess &= resultEntry.Value.Status == VectoRun.Status.Success;
				_weightedPayload += resultEntry.Value.Payload * resultEntry.Value.WeightingFactor;
				_weightedCo2 += resultEntry.Value.CO2Total / resultEntry.Value.Distance * resultEntry.Value.WeightingFactor;
				Results.Add(new XElement(tns + XMLNames.Report_Result_Result,
					new XAttribute(XMLNames.Report_Result_Status_Attr,
						resultEntry.Value.Status == VectoRun.Status.Success ? "success" : "error"),
					new XElement(tns + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat()),
					GetResults(resultEntry)));
			}
		}

		private object[] GetResults(KeyValuePair<LoadingType, XMLDeclarationReport.ResultEntry> resultEntry)
		{
			switch (resultEntry.Value.Status) {
				case VectoRun.Status.Pending:
				case VectoRun.Status.Running:
					return null; // should not happen!
				case VectoRun.Status.Success:
					return GetSuccessResultEntry(resultEntry.Value);
				case VectoRun.Status.Canceled:
				case VectoRun.Status.Aborted:
					return new object[] {
						new XElement(tns + "Error", resultEntry.Value.Error)
					};
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private object[] GetSuccessResultEntry(XMLDeclarationReport.ResultEntry result)
		{
			return new object[] {
				new XElement(tns + XMLNames.Report_Result_Payload, XMLHelper.ValueAsUnit(result.Payload, XMLNames.Unit_kg, 0)),
				result.FuelData.Select(x => new XElement(tns + XMLNames.Report_Results_FuelType, XMLHelper.ToXmlStr(DeclarationData.FuelData.Lookup(x.FuelType, x.TankSystem)))),
				new XElement(tns + XMLNames.Report_Results_AverageSpeed, XMLHelper.ValueAsUnit(result.AverageSpeed, XMLNames.Unit_kmph, 1)),
				XMLDeclarationReport.GetResults(result, tns, false).Cast<object>().ToArray()
			};
		}

		private XElement GetApplicationInfo()
		{
			var versionNumber = VectoSimulationCore.VersionNumber;
#if CERTIFICATION_RELEASE
			// add nothing to version number
#else
			versionNumber += " !!NOT FOR CERTIFICATION!!";
#endif
			return new XElement(tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, versionNumber),
				new XElement(tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		public void GenerateReport(XElement resultSignature)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, _allSuccess ? "success" : "error"));
			var summary = _weightedPayload > 0
				? new XElement(tns + XMLNames.Report_Results_Summary,
					new XElement(tns + XMLNames.Report_SpecificCO2Emissions,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_gCO2Pertkm),
						(_weightedCo2 / _weightedPayload).ConvertToGrammPerTonKilometer().ToXMLFormat(1)
					),
					new XElement(tns + XMLNames.Report_AveragePayload,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_t),
						_weightedPayload.ConvertToTon().ToXMLFormat(3)
					)
				)
				: null;
			results.Add(summary);
			var vehicle = new XElement(VehiclePart);
			vehicle.Add(InputDataIntegrity);
			retVal.Add(new XProcessingInstruction("xml-stylesheet", "href=\"https://citnet.tech.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(new XElement(tns + XMLNames.VectoCustomerReport,
				new XAttribute("schemaVersion", SCHEMA_VERSION_STRING),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "di", di),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}VectoOutputCustomer.{2}.xsd", tns, AbstractXMLWriter.SchemaLocationBaseUrl, CURRENT_SCHEMA_VERSION)),
				new XElement(tns + XMLNames.Report_DataWrap,
					vehicle,
					new XElement(tns + XMLNames.Report_ResultData_Signature, resultSignature),
					results,
					GetApplicationInfo())
				)
				);
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(retVal);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);
			var h = VectoHash.Load(stream);
			Report = h.AddHash();
		}

		public XDocument Report { get; private set; }
	}
}
