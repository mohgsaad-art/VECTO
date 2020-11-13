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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLManufacturerReportTrailer : AbstractXMLManufacturerReport
	{
		public Dictionary<XMLDeclarationReport.ResultEntry, XMLDeclarationReport.ResultEntry> Mappings = new Dictionary<XMLDeclarationReport.ResultEntry, XMLDeclarationReport.ResultEntry>();

		protected KilogramPerMeter _weightedCo2 = 0.SI<KilogramPerMeter>();
		protected Kilogram _weightedPayload = 0.SI<Kilogram>();
		protected double _weightedEfficiencyRatio = 0;

		public XMLManufacturerReportTrailer()
		{
			tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9";
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + XMLNames.Report_Results);
		}

		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleTrailerType"),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),

				new XElement(tns + XMLNames.Trailer_LegislativeCategory, modelData.VehicleData.InputDataTrailer.LegislativeCategory),
				new XElement(tns + XMLNames.Trailer_AxleCount, modelData.VehicleData.InputDataTrailer.NumberOfAxles),
				new XElement(tns + XMLNames.Trailer_TrailerType, modelData.VehicleData.InputDataTrailer.TrailerType),
				new XElement(tns + XMLNames.Trailer_BodyCode, modelData.VehicleData.InputDataTrailer.BodyworkCode),
				new XElement(tns + XMLNames.Trailer_CurbMass, modelData.VehicleData.InputDataTrailer.CurbMass.ToXMLFormat(3), XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Trailer_TPMLMTotalTrailer, modelData.VehicleData.InputDataTrailer.TPMLMTotalTrailer.ToXMLFormat(3), XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Trailer_ExternalLenghtBody, modelData.VehicleData.InputDataTrailer.ExternalBodyLength.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_ExternalWidthBody, modelData.VehicleData.InputDataTrailer.ExternalBodyWidth.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_ExternalHeightBody, modelData.VehicleData.InputDataTrailer.ExternalBodyHeight.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_TotalHeightTrailer, modelData.VehicleData.InputDataTrailer.TotalTrailerHeight.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_PositionXFirstAxle, modelData.VehicleData.InputDataTrailer.LengthTrailerFrontToFirstAxleCenter.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_PositionXLastAxle, modelData.VehicleData.InputDataTrailer.LengthTrailerFrontToLastAxleCenter.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_InternalHeightBody, modelData.VehicleData.InputDataTrailer.InternalBodyHeight.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_CargoVolume, modelData.VehicleData.InputDataTrailer.CargoVolume.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Trailer_AeroFeatureTechnologies, modelData.VehicleData.InputDataTrailer.AeroFeatureTechnologies),

				//GetADAS(modelData.VehicleData.ADAS),
				GetTorqueLimits(modelData.EngineData)
				//VehicleComponents(modelData, fuelModes)
					
			);
			
			InputDataIntegrity = GetInputDataSignature(modelData);
		}

		public double CalculateFactor(XMLDeclarationReport.ResultEntry result1, XMLDeclarationReport.ResultEntry result2)
		{
			return result2.CO2Total / result1.CO2Total;
		}

		protected override XElement GetSimulationParameters(XMLDeclarationReport.ResultEntry result)
		{
			var ratio = 0.0;
			if (Mappings.Count(x => x.Key.Mission == result.Mission && x.Key.Payload == result.Payload) > 0)
				ratio = CalculateFactor(
					Mappings.FirstOrDefault(x => x.Key.Mission == result.Mission && x.Key.Payload == result.Payload)
						.Key,
					Mappings.FirstOrDefault(x => x.Key.Mission == result.Mission && x.Key.Payload == result.Payload)
						.Value);


			return new XElement(
				tns + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(
					tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(result.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Report_ResultEntry_Payload, XMLHelper.ValueAsUnit(result.Payload, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Report_AirDrag_CdxA, result.CdxA.ToXMLFormat(3)),
				ratio > 0 ? new XElement(tns + XMLNames.Efficiency_Ratio, ratio) : null,
				result.PassengerCount.HasValue && result.PassengerCount.Value > 0 ? new XElement(tns + "PassengerCount", result.PassengerCount.Value.ToMinSignificantDigits(3, 1)) : null,
				new XElement(
					tns + XMLNames.Report_Result_FuelMode,
					result.FuelData.Count > 1 ? XMLNames.Report_Result_FuelMode_Val_Dual : XMLNames.Report_Result_FuelMode_Val_Single)
			);
		}

		protected override XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			if (modelData.VehicleData.AxleConfiguration.AxlegearIncludedInGearbox()) {
				return new XElement(
					tns + XMLNames.Vehicle_Components,
					new XAttribute(xsi + "type", "ComponentsTruckFWDType"),
					GetEngineDescription(modelData.EngineData, fuelModes),
					GetGearboxDescription(modelData.GearboxData, modelData.AxleGearData),
					GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
					GetRetarderDescription(modelData.Retarder),
					GetAngledriveDescription(modelData.AngledriveData),
					GetAirDragDescription(modelData.AirdragData),
					GetAxleWheelsDescription(modelData.VehicleData),
					GetAuxiliariesDescription(modelData)
				);
			}
			return new XElement(
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
				GetAuxiliariesDescription(modelData)
			);
		}

		protected override XElement GetAuxiliariesDescription(VectoRunData modelData)
		{
			var auxData = modelData.Aux.ToDictionary(a => a.ID);
			var auxList = new[] {
				AuxiliaryType.Fan, AuxiliaryType.SteeringPump, AuxiliaryType.ElectricSystem, AuxiliaryType.PneumaticSystem,
				AuxiliaryType.HVAC
			};
			var retVal = new XElement(tns + XMLNames.Component_Auxiliaries);
			foreach (var auxId in auxList) {
				if (!auxData.ContainsKey(auxId.Key())) {
					continue;
				}

				foreach (var entry in auxData[auxId.Key()].Technology) {
					retVal.Add(new XElement(tns + GetTagName(auxId), entry));
				}
			}

			return retVal;
		}

		private string GetTagName(AuxiliaryType auxId)
		{
			return auxId.ToString() + "Technology";
		}

		public override void WriteResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			_allSuccess &= resultEntry.Status == VectoRun.Status.Success;
			if (resultEntry.Status == VectoRun.Status.Success)
			{
				_weightedPayload += resultEntry.Payload * resultEntry.WeightingFactor;
				_weightedCo2 += resultEntry.CO2Total / resultEntry.Distance * resultEntry.WeightingFactor;

				var ratio = 0.0;
				if (Mappings.Count(x => x.Key.Mission == resultEntry.Mission && x.Key.Payload == resultEntry.Payload) > 0)
				{
					var currentWeightedCo2 = resultEntry.CO2Total / resultEntry.Distance * resultEntry.WeightingFactor;
					var genericResultentry = Mappings.FirstOrDefault(x =>
						x.Key.Mission == resultEntry.Mission && x.Key.Payload == resultEntry.Payload).Key;

					var currentGenericWeightedCo2 = genericResultentry.CO2Total / genericResultentry.Distance * resultEntry.WeightingFactor;

					ratio = currentWeightedCo2 / currentGenericWeightedCo2;
				}
				_weightedEfficiencyRatio += ratio;
			}

			Results.Add(
				resultEntry.Status == VectoRun.Status.Success ? GetSuccessResult(resultEntry) : GetErrorResult(resultEntry));
		}

		public override void GenerateReport()
		{
			var mrf = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput");
			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, _allSuccess ? "success" : "error"));
			var summary = _allSuccess && _weightedPayload > 0
				? new XElement(tns + XMLNames.Report_Results_Summary,
					new XElement(tns + "WeightedCO2Emissions",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_gCO2Pertkm),
						(_weightedCo2 / _weightedPayload).ConvertToGrammPerTonKilometer().ToXMLFormat(1)
					),
					new XElement(tns + "WeightedPayload",
						new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_t),
						_weightedPayload.ConvertToTon().ToXMLFormat(3)
					),
					_weightedEfficiencyRatio > 0 ? new XElement(tns + "WeightedEfficiencyRatio", _weightedEfficiencyRatio.ToMinSignificantDigits(2)) : null
				)
				: null;
			results.Add(summary);
			var vehicle = new XElement(VehiclePart);
			vehicle.Add(InputDataIntegrity);
			retVal.Add(
				new XProcessingInstruction(
					"xml-stylesheet", "href=\"https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(
				new XElement(
					mrf + XMLNames.VectoManufacturerReport,

					//new XAttribute("schemaVersion", CURRENT_SCHEMA_VERSION),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
					new XAttribute("xmlns", tns),
					new XAttribute(XNamespace.Xmlns + "di", di),
					//new XAttribute(XNamespace.Xmlns + "tns", tns),
					new XAttribute(XNamespace.Xmlns + "mrf", mrf),
					new XAttribute(
						xsi + "schemaLocation",
						string.Format("{0} {1}/DEV/VectoOutputManufacturer.xsd", mrf, AbstractXMLWriter.SchemaLocationBaseUrl)),
					new XElement(
						mrf + XMLNames.Report_DataWrap,
						new XAttribute(xsi + "type", "VectoOutputDataType"),
						vehicle,
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

		private XElement GetApplicationInfo()
		{
			var versionNumber = VectoSimulationCore.VersionNumber;
#if CERTIFICATION_RELEASE // add nothing to version number
#else
			versionNumber += " !!NOT FOR CERTIFICATION!!";
#endif
			return new XElement(
				tns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(tns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, versionNumber),
				new XElement(
					tns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

	}
}
