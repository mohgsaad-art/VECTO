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
*   Vasileios Kouliaridis, vasilis.k@emisia.com, EMISIA
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	public class XMLCustomerReportTrailer : AbstractXMLCustomerReport
	{
		public const string CURRENT_SCHEMA_VERSION = "0.9";

		protected readonly XElement VehiclePart;

		protected XElement InputDataIntegrity;

		protected readonly XElement Results;

		protected readonly XNamespace rootNS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput");
		protected readonly XNamespace tns = XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput:v" + CURRENT_SCHEMA_VERSION);
		protected readonly XNamespace di = XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");
		protected readonly XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected bool _allSuccess = true;

		protected Kilogram WeightedPayloadGeneric = 0.SI<Kilogram>();
		protected double GenericWeightedCo2 = 0;
		protected CubicMeter CargoVolumeGeneric;

		protected double WeightedCo2Trailer = 0;
		protected Kilogram WeightedPayloadTrailer = 0.SI<Kilogram>();
		protected CubicMeter CargoVolumeTrailer;
		

		public Dictionary<XMLDeclarationReport.ResultEntry, XMLDeclarationReport.ResultEntry> Mappings = new Dictionary<XMLDeclarationReport.ResultEntry, XMLDeclarationReport.ResultEntry>();

		public XMLCustomerReportTrailer()
		{
			
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + XMLNames.Report_Results);
		}

		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			var exempted = modelData.Exempted;
			CargoVolumeTrailer = modelData.VehicleData.InputDataTrailer.CargoVolume;
			CargoVolumeGeneric = modelData.Mission.TotalCargoVolume;

			VehiclePart.Add(
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),

				new XElement(tns + XMLNames.Trailer_LegislativeCategory, modelData.VehicleData.InputDataTrailer.LegislativeCategory),
				new XElement(tns + XMLNames.Trailer_AxleCount, modelData.VehicleData.InputDataTrailer.NumberOfAxles),
				new XElement(tns + XMLNames.Trailer_TrailerType, modelData.VehicleData.InputDataTrailer.TrailerType),
				new XElement(tns + XMLNames.Trailer_BodyCode, modelData.VehicleData.InputDataTrailer.BodyworkCode),
				new XElement(tns + XMLNames.Trailer_CurbMass, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg, 3)),
				new XElement(tns + XMLNames.Trailer_TPMLMTotalTrailer, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg, 3)),
				new XElement(tns + XMLNames.Trailer_ExternalLenghtBody, modelData.VehicleData.InputDataTrailer.ExternalBodyLength.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_ExternalWidthBody, modelData.VehicleData.InputDataTrailer.ExternalBodyWidth.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_ExternalHeightBody, modelData.VehicleData.InputDataTrailer.ExternalBodyHeight.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_TotalHeightTrailer, modelData.VehicleData.InputDataTrailer.TotalTrailerHeight.ToXMLFormat(3)),
				//new XElement(tns + XMLNames.Trailer_PositionXFirstAxle, modelData.VehicleData.InputDataTrailer.LengthTrailerFrontToFirstAxleCenter.ToXMLFormat(3)),
				//new XElement(tns + XMLNames.Trailer_PositionXLastAxle, modelData.VehicleData.InputDataTrailer.LengthTrailerFrontToLastAxleCenter.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_InternalHeightBody, modelData.VehicleData.InputDataTrailer.InternalBodyHeight.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_CargoVolume, modelData.VehicleData.InputDataTrailer.CargoVolume.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_AeroFeatureTechnologies, modelData.VehicleData.InputDataTrailer.AeroFeatureTechnologies)
	
			);

			if (exempted) {
				VehiclePart.Add(new XAttribute(xsi + "type", "ExemptedVehicleType"), 
					ExemptedData(modelData));
				Results.Add(new XElement(tns + XMLNames.Report_ExemptedVehicle));
			} else {
				VehiclePart.Add(
					new XAttribute(xsi + "type", "VehicleType"),
					new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
					new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
					new XElement(tns + XMLNames.Vehicle_VocationalVehicle, modelData.VehicleData.VocationalVehicle),
					new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab)
					//GetADAS(modelData.VehicleData.ADAS),
					//ComponentData(modelData, fuelModes)
					);
			}
			InputDataIntegrity = new XElement(tns + XMLNames.Report_InputDataSignature,
				modelData.InputDataHash == null ? XMLHelper.CreateDummySig(di) : new XElement(modelData.InputDataHash));
		}

		private object[] ExemptedData(VectoRunData modelData)
		{
			return new object[] {
				modelData.VehicleData.HybridElectricHDV ? new XElement(tns + XMLNames.Vehicle_MaxNetPower1, XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower1, XMLNames.Unit_W)) : null,
				modelData.VehicleData.HybridElectricHDV ? new XElement(tns + XMLNames.Vehicle_MaxNetPower2, XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower2, XMLNames.Unit_W)) : null
			};
		}

		protected XElement GetADAS(VehicleData.ADASData adasData)
		{
			return new XElement(tns + XMLNames.Vehicle_ADAS,
								new XElement(tns + XMLNames.Vehicle_ADAS_EngineStopStart, adasData.EngineStopStart),
								new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adasData.EcoRoll.WithoutEngineStop()),
								new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adasData.EcoRoll.WithEngineStop()),
								new XElement(tns + XMLNames.Vehicle_ADAS_PCC, adasData.PredictiveCruiseControl != PredictiveCruiseControlType.None)
			);
		}

		protected virtual XElement[] ComponentData(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			return new[] {
				new XElement(
					tns + XMLNames.Report_Vehicle_EngineRatedPower,
					XMLHelper.ValueAsUnit(modelData.EngineData.RatedPowerDeclared, XMLNames.Unit_kW)),
				new XElement(
					tns + XMLNames.Report_Vehicle_EngineDisplacement,
					XMLHelper.ValueAsUnit(modelData.EngineData.Displacement, XMLNames.Unit_ltr, 1)),
				new XElement(
					tns + XMLNames.Report_Vehicle_FuelTypes,
					fuelModes.SelectMany(x => x.Select(f => f.FuelType.ToXMLFormat())).Distinct()
							.Select(x => new XElement(tns + XMLNames.Engine_FuelType, x))
				),
				new XElement(
					tns + XMLNames.Report_Vehicle_TransmissionCertificationMethod,
					modelData.GearboxData.CertificationMethod.ToXMLFormat()),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, modelData.GearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, modelData.GearboxData.Gears.Count),
				new XElement(tns + XMLNames.Report_Vehicle_Retarder, modelData.Retarder.Type.IsDedicatedComponent()),
				new XElement(tns + XMLNames.Report_Vehicle_AxleRatio, modelData.AxleGearData.AxleGear.Ratio.ToXMLFormat(3)),
				new XElement(
					tns + XMLNames.Report_Vehicle_AverageRRC, modelData.VehicleData.AverageRollingResistanceTruck.ToXMLFormat(4)),

				//new XElement(
				//	tns + XMLNames.Report_Vehicle_AverageRRCLabel,
				//	DeclarationData.Wheels.TyreClass.Lookup(modelData.VehicleData.AverageRollingResistanceTruck))
			}.Concat(
				modelData.VehicleData.AxleData.Where(x => x.AxleType != AxleType.Trailer).Select(
					(x, idx) => new XElement(tns + "FuelEfficiencyLabelMotorVehicleTyre",
					new XAttribute("axleNbr", idx+1),
					x.FuelEfficiencyClass))).ToArray();

		}

		public override void WriteResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			//foreach (var resultEntry in entry.ResultEntry) {
			_allSuccess &= resultEntry.Status == VectoRun.Status.Success;
			if (resultEntry.Status == VectoRun.Status.Success) {
				//TODO: identifies trailer missions with PassengerCount  - need to change this probably
				if (resultEntry.PassengerCount != null && resultEntry.PassengerCount == 1)
				{
					WeightedPayloadTrailer += resultEntry.Payload * resultEntry.WeightingFactor;
					WeightedCo2Trailer += (resultEntry.CO2Total.ConvertToGramm() / resultEntry.Distance.ConvertToKiloMeter()) * resultEntry.WeightingFactor;
				}
				else {
					WeightedPayloadGeneric += resultEntry.Payload * resultEntry.WeightingFactor;
					GenericWeightedCo2 += (resultEntry.CO2Total.ConvertToGramm() / resultEntry.Distance.ConvertToKiloMeter()) * resultEntry.WeightingFactor;
				}

			}
			Results.Add(resultEntry.Status == VectoRun.Status.Success ? GetSuccessResult(resultEntry) : GetErrorResult(resultEntry));
		}

		private XElement GetErrorResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			var content = new object[] { };
			switch (resultEntry.Status) {
				case VectoRun.Status.Pending:
				case VectoRun.Status.Running:
					content = null; // should not happen!
					break;
				case VectoRun.Status.Canceled:
				case VectoRun.Status.Aborted:
					content =  new object[] {
						new XElement(tns + "Error", resultEntry.Error)
					};
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return new XElement(tns + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				new XElement(tns + XMLNames.Report_Result_Mission, resultEntry.Mission.ToXMLFormat()),
				content);
		}

		private XElement GetSuccessResult(XMLDeclarationReport.ResultEntry result)
		{
			var ratio = 0.0;
			var ratioTons = 0.0;
			var ratioCubicMeter = 0.0;
			var man = new XMLManufacturerReportTrailer();

			if (Mappings.Count(x => x.Key.Mission == result.Mission && x.Key.Payload == result.Payload) > 0) {
				var key = Mappings
					.FirstOrDefault(x => x.Key.Mission == result.Mission && x.Key.Payload == result.Payload)
					.Key;
				var value = Mappings
					.FirstOrDefault(x => x.Key.Mission == result.Mission && x.Key.Payload == result.Payload)
					.Value;
				ratio = man.CalculateFactor(key, value);
				ratioTons = man.DoCalculateFactor(key.CO2Total, key.Distance, key.Payload, value.CO2Total, value.Distance,
					value.Payload);
				ratioCubicMeter = man.DoCalculateFactor(key.CO2Total, key.Distance, CargoVolumeTrailer, value.CO2Total,
					value.Distance, CargoVolumeGeneric);
			}

			var showRatio = result.PassengerCount != null && result.PassengerCount == 1 && result.Mission == MissionType.LongHaul ||
							result.Mission == MissionType.RegionalDelivery;

			return new XElement(
				tns + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "success"),
				new XAttribute(xsi + "type", "ResultSuccessType"),
				new XElement(tns + XMLNames.Report_Result_Mission, result.Mission.ToXMLFormat()),
				//new XElement(tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
				//	XMLHelper.ValueAsUnit(result.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Report_ResultEntry_Payload, XMLHelper.ValueAsUnit(result.Payload, XMLNames.Unit_kg)),
				//result.CdxA != null ? new XElement(tns + XMLNames.Report_AirDrag_CdxA, result.CdxA.ToXMLFormat(3)) : null,
				showRatio ? new XElement(tns + XMLNames.Efficiency_Ratio, new XAttribute("unit", "g/km"), ratio.ToXMLFormat(2)) : null,
				showRatio ? new XElement(tns + XMLNames.Efficiency_Ratio, new XAttribute("unit", "g/t-km"), ratioTons.ToXMLFormat(2)) : null,
				showRatio ? new XElement(tns + XMLNames.Efficiency_Ratio, new XAttribute("unit", "g/m³-km"), ratioCubicMeter.ToXMLFormat(2)) : null
				//new XElement(tns + XMLNames.Report_Result_FuelMode,
				//	result.FuelData.Count > 1 ? XMLNames.Report_Result_FuelMode_Val_Dual : XMLNames.Report_Result_FuelMode_Val_Single),
				//new XElement(tns + XMLNames.Report_Results_AverageSpeed, XMLHelper.ValueAsUnit(result.AverageSpeed, XMLNames.Unit_kmph, 1)),
				//XMLDeclarationReport.GetResults(result, tns, false).Cast<object>().ToArray()
			);
		}

		protected XElement GetApplicationInfo()
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

		public override void GenerateReport(XElement resultSignature)
		{
			var weightedCo2Ton = WeightedCo2Trailer / WeightedPayloadTrailer.ConvertToTon();
			var weightedGenericCo2Ton = GenericWeightedCo2 / WeightedPayloadGeneric.ConvertToTon();
			var weightedCo2CubicMeter = (WeightedCo2Trailer / CargoVolumeTrailer);
			var weightedGenericCo2CubicMeter = (GenericWeightedCo2 / CargoVolumeGeneric);

			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, _allSuccess ? "success" : "error"));
			var summary = _allSuccess && WeightedPayloadTrailer > 0
				? new XElement(tns + XMLNames.Report_Results_Summary,
					new XElement(tns + "WeightedPayload",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_kg),
						WeightedPayloadGeneric.ToXMLFormat()
					),
					new XElement(tns + "WeightedPayloadTrailer",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_kg),
						WeightedPayloadTrailer.ToXMLFormat()
					),
					new XElement(tns + "WeightedCO2Emissions",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
						GenericWeightedCo2.ToXMLFormat(3)
					),
					new XElement(tns + "WeightedCO2EmissionsTrailer",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
						WeightedCo2Trailer.ToXMLFormat(3)
					),
					new XElement(tns + "WeightedCO2Emissions",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
						weightedGenericCo2Ton.ToXMLFormat(1)
					),
					new XElement(tns + "WeightedCO2EmissionsTrailer",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
						weightedCo2Ton.ToXMLFormat(1)
					),
					new XElement(tns + "WeightedCO2Emissions",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						weightedGenericCo2CubicMeter.ToXMLFormat(3)
					),
					new XElement(tns + "WeightedCO2EmissionsTrailer",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						weightedCo2CubicMeter.ToXMLFormat(3)
					),
					new XElement(tns + "WeightedEfficiencyRatio",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
						(WeightedCo2Trailer / GenericWeightedCo2).ToXMLFormat(2)),
					new XElement(tns + "WeightedEfficiencyRatio",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
						(weightedCo2Ton / weightedGenericCo2Ton).ToXMLFormat(2)),
					new XElement(tns + "WeightedEfficiencyRatio",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						(weightedCo2CubicMeter / weightedGenericCo2CubicMeter).ToXMLFormat(2))
				)
				: null;
			results.Add(summary);
			var vehicle = new XElement(VehiclePart);
			vehicle.Add(InputDataIntegrity);
			retVal.Add(new XProcessingInstruction("xml-stylesheet", "href=\"https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(new XElement(rootNS + XMLNames.VectoCustomerReport,
				//new XAttribute("schemaVersion", CURRENT_SCHEMA_VERSION),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "tns", rootNS),
				new XAttribute(XNamespace.Xmlns + "di", di),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}DEV/VectoOutputCustomer.xsd", rootNS, AbstractXMLWriter.SchemaLocationBaseUrl)),
				new XElement(rootNS + XMLNames.Report_DataWrap,
					new XAttribute(xsi + "type", "VectoOutputDataType"),
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
	}
}
