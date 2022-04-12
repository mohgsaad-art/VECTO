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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter.Xml;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML
{
	public static class XMLReportHelperTrailer
	{
		public static XElement GetFactorWithAttributes(XName xmlName, double val, string typeAttrVal,
			uint? decimals = null,
			string unitAttrVal = "-")
		{
			return new XElement(xmlName, val.ToXMLFormat(decimals), new XAttribute("type", typeAttrVal),
				new XAttribute("unit", unitAttrVal));
		}

		public static double CalculateFactor(XMLDeclarationReport.ResultEntry specificEntry, XMLDeclarationReport.ResultEntry genericEntry)
		{
			return DoCalculateFactor(specificEntry.CO2Total, specificEntry.Distance, genericEntry.CO2Total, genericEntry.Distance);
		}
		public static double DoCalculateFactor(Kilogram specificCo2, CubicMeter specificCargoVolume, Kilogram genericCo2, CubicMeter genericCargoVolume)
		{
			return ((specificCo2 * 1000) / specificCargoVolume.Value()) /
					((genericCo2 * 1000) / genericCargoVolume.Value());
			//g/m3-km
		}

		public static double DoCalculateFactor(Kilogram specificCo2, Meter specificDistance, Kilogram genericCo2, Meter genericDistance)
		{
			return ((specificCo2 * 1000) / (specificDistance / 1000)) / 
					((genericCo2 * 1000) / (genericDistance / 1000));
			//g/km
		}

		public static double DoCalculateFactor(Kilogram specificCo2, Meter specificDistance, Kilogram specificPayload, Kilogram genericCo2, Meter genericDistance, Kilogram genericPayload)
		{
			return (((specificCo2 * 1000) / (specificDistance / 1000)) / (specificPayload.Value())) /
					(((genericCo2 * 1000) / (genericDistance / 1000)) / (genericPayload.Value()));
			//g/t-km
		}

		public static IEnumerable<XElement> GetResults(XMLDeclarationReport.ResultEntry result, XNamespace tns)
		{

			var retVal = new List<XElement>();

			//var cargoVolume = result.PassengerCount == 1 ? SpecificCargoVolume : ReferenceCargoVolume;

			foreach (var fuel in result.FuelData) {
				var fcResult = new XElement(tns + XMLNames.Report_Results_Fuel,
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()));
				//liter
				fcResult.Add(
					new XElement(
						tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/100km"),
						(result.FuelConsumptionFinal[fuel.FuelType].ConvertToGramm() / fuel.FuelDensity /
							result.Distance.ConvertToKiloMeter() * 100)
						.Value().ToMinSignificantDigits(3, 1)),
					new XElement(
						tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/t-km"),
						(result.FuelConsumptionFinal[fuel.FuelType].ConvertToGramm() / fuel.FuelDensity /
						result.Distance.ConvertToKiloMeter() /
						result.Payload.ConvertToTon()).Value().ToMinSignificantDigits(3, 1)));
				if (result.CargoVolume > 0) {
					fcResult.Add(
						new XElement(
							tns + XMLNames.Report_Results_FuelConsumption,
							new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/m³-km"),
							(result.FuelConsumptionFinal[fuel.FuelType].ConvertToGramm() / fuel.FuelDensity /
							result.Distance.ConvertToKiloMeter() /
							result.CargoVolume).Value().ToMinSignificantDigits(3, 1)));
				}
                //g/x

				fcResult.Add(
					new XElement(
						tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
						(result.FuelConsumptionFinal[fuel.FuelType].ConvertToGramm() /
							result.Distance.ConvertToKiloMeter()).ToMinSignificantDigits(3, 1)),

                    new XElement(
                        tns + XMLNames.Report_Results_FuelConsumption,
                        new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
                        (result.FuelConsumptionFinal[fuel.FuelType].ConvertToGramm() /
                        result.Distance.ConvertToKiloMeter() /
                        result.Payload.ConvertToTon()).ToMinSignificantDigits(3, 1)));
                if (result.CargoVolume > 0)
                {
                    fcResult.Add(
                        new XElement(
                            tns + XMLNames.Report_Results_FuelConsumption,
                            new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
                            (result.FuelConsumptionFinal[fuel.FuelType].ConvertToGramm() /
                            result.Distance.ConvertToKiloMeter() /
                            result.CargoVolume).Value().ToMinSignificantDigits(3, 1)));
                }

				retVal.Add(fcResult);
			}

			//CO2
			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2, new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter()).ToMinSignificantDigits(3,
						2)));
			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
					(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter() /
					result.Payload.ConvertToTon()).ToMinSignificantDigits(3, 2)));
			if (result.CargoVolume > 0)
				retVal.Add(
					new XElement(
						tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter() / result.CargoVolume)
						.Value()
						.ToMinSignificantDigits(3, 2)));
			return retVal;
		}

		public static XElement GetSimulationParameters(XMLDeclarationReport.ResultEntry result, TrailerSegment trailerSegment, XNamespace tns, bool includeYawAngles = true)
		{
			SquareMeter a1, a2, a3;

			if (result.PassengerCount != null && result.PassengerCount == 1)
			{
				a1 = trailerSegment.A1Cor;
				a2 = trailerSegment.A2Cor;
				a3 = trailerSegment.A3Cor;
			}
			else
			{
				a1 = trailerSegment.A1RefCor;
				a2 = trailerSegment.A2RefCor;
				a3 = trailerSegment.A3RefCor;
			}

			var cdxAYawAngle3 = result.CdxA +
								a1 * 3 +
								a2 * 3 * 3 +
								a3 * 3 * 3 * 3;

			var cdxAYawAngle6 = result.CdxA +
								a1 * 6 +
								a2 * 6 * 6 +
								a3 * 6 * 6 * 6;

			var cdxAYawAngle9 = result.CdxA +
								a1 * 9 +
								a2 * 9 * 9 +
								a3 * 9 * 9 * 9;

			var resultXElement = new XElement(
				tns + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(tns + XMLNames.Report_ResultEntry_Payload, XMLHelper.ValueAsUnit(result.Payload, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Report_ResultEntry_TotalVehicleMass, XMLHelper.ValueAsUnit(result.TotalVehicleMass, XMLNames.Unit_kg))
			);
			if (includeYawAngles) {
				resultXElement.Add(
					result.CdxA != null
						? new XElement(tns + XMLNames.Report_AirDrag_CdxA + "yawAngle0", new XAttribute("unit", "m²"),
							result.CdxA.ToXMLFormat(3))
						: null,
					result.CdxA != null
						? new XElement(tns + XMLNames.Report_AirDrag_CdxA + "yawAngle3", new XAttribute("unit", "m²"),
							cdxAYawAngle3.ToXMLFormat(3))
						: null,
					result.CdxA != null
						? new XElement(tns + XMLNames.Report_AirDrag_CdxA + "yawAngle6", new XAttribute("unit", "m²"),
							cdxAYawAngle6.ToXMLFormat(3))
						: null,
					result.CdxA != null
						? new XElement(tns + XMLNames.Report_AirDrag_CdxA + "yawAngle9", new XAttribute("unit", "m²"),
							cdxAYawAngle9.ToXMLFormat(3))
						: null
				);
			}



			resultXElement.Add(new XElement(tns + XMLNames.Report_ResultEntry_AverageSpeed, new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_kmph), result.AverageSpeed.ConvertToKiloMeterPerHour().Value.ToXMLFormat(1)));
			return resultXElement;
		}
		public static XElement[] GetWeightedFuelConsumption(IDictionary<FuelType, WeightedFc> WeightedFcPerFuelType,
			Kilogram WeightedPayloadTrailer, 
			CubicMeter SpecificCargoVolume, XNamespace tns)
		{
			var fuels = new List<XElement>();
			foreach (var weightedFuelConsumption in WeightedFcPerFuelType)
			{
				var weightedfcTrailerLiter = weightedFuelConsumption.Value.WeightedFcLitrePerKilometer;
				var weightedfcTrailerGram = weightedFuelConsumption.Value.WeightedFcGramPerKilometer;

				var weightedFcTrailerLiterTon = weightedfcTrailerLiter / WeightedPayloadTrailer.ConvertToTon();
				var weightedFcTrailerGrammTon = weightedfcTrailerGram / WeightedPayloadTrailer.ConvertToTon();
				var weighteFcTrailerLiterCubicMeter = weightedfcTrailerLiter / SpecificCargoVolume.Value();
				var weightedFcTrailerGrammPerCubicMeter = weightedfcTrailerGram / SpecificCargoVolume.Value();



				var fuel = new XElement(tns + XMLNames.Engine_FuelModes_Fuel,
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, weightedFuelConsumption.Key.ToXMLFormat()));
				fuels.Add(fuel);
				fuel.Add(

					new XElement(tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/100km"),
						(weightedfcTrailerLiter * 100).ToXMLFormat(2)),
					new XElement(tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/t-km"),
						weightedFcTrailerLiterTon.ToXMLFormat(4)),
					new XElement(tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/m³-km"),
						weighteFcTrailerLiterCubicMeter.ToXMLFormat(5)),

					new XElement(tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
						weightedfcTrailerGram.ToXMLFormat(2)),
					new XElement(tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
						weightedFcTrailerGrammTon.ToXMLFormat(2)),
					new XElement(tns + XMLNames.Report_Results_FuelConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						weightedFcTrailerGrammPerCubicMeter.ToXMLFormat(2))
					);
			}

			return fuels.ToArray();
		}

		public class WeightedFc
		{
			public double WeightedFcLitrePerKilometer;
			public double WeightedFcGramPerKilometer;
		}
	}

	public class XMLManufacturerReportTrailer : AbstractXMLManufacturerReport
	{

		public Dictionary<XMLDeclarationReport.ResultEntry, XMLDeclarationReport.ResultEntry> Mappings = new Dictionary<XMLDeclarationReport.ResultEntry, XMLDeclarationReport.ResultEntry>();

		protected Kilogram ReferenceWeightedPayload = 0.SI<Kilogram>();
		protected double ReferenceWeightedCo2 = 0;
		protected CubicMeter ReferenceCargoVolume;

		protected double WeightedCo2Trailer = 0;
		protected double WeightedFcTrailer = 0;
		private Dictionary<FuelType, XMLReportHelperTrailer.WeightedFc> WeightedFcPerFuelType = new Dictionary<FuelType, XMLReportHelperTrailer.WeightedFc>();
		protected Kilogram WeightedPayloadTrailer = 0.SI<Kilogram>();
		protected CubicMeter SpecificCargoVolume;
		private string _genericTowingVehicle;

		private TrailerSegment _trailerSegment;

		public XMLManufacturerReportTrailer()
		{
			tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:Trailer:v0.1";
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + XMLNames.Report_Results);
		}

		public override void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			_trailerSegment = modelData.VehicleData.TrailerSegment;

			SpecificCargoVolume = modelData.Mission.TotalCargoVolume + modelData.VehicleData.InputDataTrailer.CargoVolume;
			ReferenceCargoVolume = modelData.Mission.TotalCargoVolume + _trailerSegment.ReferenceTrailerSegment.CargoVolume;

			_genericTowingVehicle = modelData.VehicleData.ModelName;

			var aeroFeatures = new XElement(tns + XMLNames.Trailer_AeroFeatures);
			var trailerData = modelData.VehicleData.InputDataTrailer;
			var standardTechnologies = new XElement(tns + "StandardTechnologies");
			if (trailerData.AeroFeatureTechnologies.Count == 0 && modelData.VehicleData.InputDataTrailer.CertifiedAeroDeviceApplicableVehicleGroup == null) {
				//No Aero Device
				standardTechnologies.Add(new XElement(tns + XMLNames.Trailer_AeroFeatureTechnologies, AeroFeatureTechnology.None.ToXMLFormat()));
				aeroFeatures.Add(standardTechnologies);

			} else if(trailerData.AeroFeatureTechnologies.Count > 0 || trailerData.CertifiedAeroDeviceApplicableVehicleGroup != null) {


				if (trailerData.AeroFeatureTechnologies.Count > 0)
				{
					aeroFeatures.Add(standardTechnologies);
				}
				foreach (var trailerDataAeroFeatureTechnology in trailerData.AeroFeatureTechnologies) {
					standardTechnologies.Add(new XElement(tns + XMLNames.Trailer_AeroFeatureTechnologies, trailerDataAeroFeatureTechnology.ToXMLFormat()));
				}

				var aeroReductionElement = new XElement(tns + XMLNames.Trailer_AeroReduction);
				
				if (trailerData.AeroFeatureTechnologies.Count > 0)
				{
					//Standard Aero Device
					double[] aeroReductions;
					var specificTrailerSegment = trailerData.GetTrailerSegment().SpecificTrailerSegment;
					switch (trailerData.GetAeroFeatureCombination()) {
						case 0:
							aeroReductions = specificTrailerSegment.Aero0;
							break;
						case 1:
							aeroReductions = specificTrailerSegment.Aero1;
							break;
						case 2:
							aeroReductions = specificTrailerSegment.Aero2;
							break;
						case 3:
							aeroReductions = specificTrailerSegment.Aero3;
							break;
						case 4:
							aeroReductions = specificTrailerSegment.Aero4;
							break;
						case 5:
							aeroReductions = specificTrailerSegment.Aero5;
							break;
						case 6:
							aeroReductions = specificTrailerSegment.Aero6;
							break;
						case 7:
							aeroReductions = specificTrailerSegment.Aero7;
							break;
						case 8:
							aeroReductions = specificTrailerSegment.Aero8;
							break;
						default:
							throw new VectoException("Invalid aero combination");
					}


					aeroReductionElement.Add(new XElement(tns + XMLNames.Trailer_YawAngle0, (aeroReductions[0]*(-1)).ToXMLFormat(2), new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.UnitPercent)));
					aeroReductionElement.Add(new XElement(tns + XMLNames.Trailer_YawAngle3, (aeroReductions[1]*(-1)).ToXMLFormat(2), new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.UnitPercent)));
					aeroReductionElement.Add(new XElement(tns + XMLNames.Trailer_YawAngle6, (aeroReductions[2]*(-1)).ToXMLFormat(2), new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.UnitPercent)));
					aeroReductionElement.Add(new XElement(tns + XMLNames.Trailer_YawAngle9, (aeroReductions[3]*(-1)).ToXMLFormat(2), new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.UnitPercent)));


				}
				else
				{
					
					var certifiedAeroDevice = new XElement(tns + "CertifiedAeroDevice");
					aeroFeatures.Add(certifiedAeroDevice);
					certifiedAeroDevice.Add(new XElement(tns + XMLNames.Component_CertificationNumber, trailerData.CertifiedAeroCertificationNumber));

					var aeroSignature = XElement.Parse(trailerData.CertifiedAeroSignatureXml);
					certifiedAeroDevice.Add(new XElement(tns + XMLNames.DI_Signature, aeroSignature.Elements()));

					//Certified Aero Device
					aeroReductionElement.Add(new XElement(tns + XMLNames.Trailer_YawAngle0, modelData.VehicleData.InputDataTrailer.YawAngle0.ToXMLFormat(2), new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.UnitPercent)));
					aeroReductionElement.Add(new XElement(tns + XMLNames.Trailer_YawAngle3, modelData.VehicleData.InputDataTrailer.YawAngle3.ToXMLFormat(2), new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.UnitPercent)));
					aeroReductionElement.Add(new XElement(tns + XMLNames.Trailer_YawAngle6, modelData.VehicleData.InputDataTrailer.YawAngle6.ToXMLFormat(2), new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.UnitPercent)));
					aeroReductionElement.Add(new XElement(tns + XMLNames.Trailer_YawAngle9, modelData.VehicleData.InputDataTrailer.YawAngle9.ToXMLFormat(2), new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.UnitPercent)));
					

				}
				aeroFeatures.Add(aeroReductionElement);
			}

			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehicleType"),
				new XElement(tns + XMLNames.Component_Manufacturer,
					modelData.VehicleData.InputDataTrailer.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress,
					modelData.VehicleData.InputDataTrailer.ManufacturerAddress),
				new XElement(tns + XMLNames.Trailer_Model,
					modelData.VehicleData.InputDataTrailer.TrailerModel),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.InputDataTrailer.VIN),
				new XElement(tns + XMLNames.Component_Date, modelData.VehicleData.InputDataTrailer.Date),
				new XElement(tns + XMLNames.Trailer_LegislativeCategory,
					modelData.VehicleData.InputDataTrailer.LegislativeCategory),
				new XElement(tns + XMLNames.Trailer_AxleCount,
					int.Parse(modelData.VehicleData.InputDataTrailer.NumberOfAxles.GetLabel())),
				new XElement(tns + XMLNames.Trailer_TrailerType, modelData.VehicleData.InputDataTrailer.TrailerType),
				new XElement(tns + XMLNames.Trailer_BodyCode,
					modelData.VehicleData.InputDataTrailer.BodyworkCode.GetLabel()),
				new XElement(tns + XMLNames.Trailer_TrailerCouplingPoint,
					modelData.VehicleData.InputDataTrailer.TrailerCouplingPoint.GetLabel()),
				new XElement(tns + XMLNames.Trailer_Masses,
					new XElement(tns + XMLNames.Trailer_MassInRunningOrder,
						modelData.VehicleData.InputDataTrailer.MassInRunningOrder.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Trailer_TPMLMTotalTrailer,
						modelData.VehicleData.InputDataTrailer.TPMLMTotalTrailer.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Trailer_TPLMAxleAssembly,
						modelData.VehicleData.InputDataTrailer.TPMLMAxleAssembly?.ToXMLFormat(0) ?? "n/a")
					
				),
				new XElement(tns + XMLNames.Trailer_VehicleGroupAnnexI, modelData.VehicleData.TrailerSegment.VehicleGroupAnnex),
				new XElement(tns + XMLNames.Trailer_VehicleGroupToolInternal, modelData.VehicleData.TrailerSegment.VehicleGroup),
				new XElement(tns + XMLNames.Trailer_Dimensions,
					new XElement(tns + XMLNames.Trailer_ExternalLengthBody, 
						modelData.VehicleData.InputDataTrailer.ExternalBodyLength.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Trailer_ExternalWidthBody, 
						modelData.VehicleData.InputDataTrailer.ExternalBodyWidth.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Trailer_ExternalHeightBody, 
						modelData.VehicleData.InputDataTrailer.ExternalBodyHeight.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Trailer_TotalHeightTrailer, 
						modelData.VehicleData.InputDataTrailer.TotalTrailerHeight.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Trailer_LengthFromFrontToFirstAxle, 
						modelData.VehicleData.InputDataTrailer.LengthFromFrontToFirstAxle.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Trailer_LengthBetweenCentersOfAxles, 
						modelData.VehicleData.InputDataTrailer.LengthBetweenCentersOfAxles.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Trailer_CargoVolume, 
						modelData.VehicleData.InputDataTrailer.CargoVolume.ToXMLFormat(3)),
					new XElement(tns + XMLNames.Trailer_VolumeOrientation, modelData.VehicleData.InputDataTrailer.VolumeOrientation)
				),
				aeroFeatures
			);

			var axles = new XElement(tns + "AxlesAndTyreFeatures");
			var axleCount = 1;
			foreach (var axle in modelData.VehicleData.InputDataTrailer.Axles) {
				axles.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle,
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, axleCount++),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Steered, axle.Steered),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Liftable, axle.Liftable),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_TwinTyres, axle.TwinTyres),
						new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Tyre,
							new XElement(tns + "Data",
								new XElement(tns + XMLNames.Component_Model,
									axle.Tyre.Model),
								new XElement(tns + XMLNames.Component_CertificationNumber,
										axle.Tyre
										.CertificationNumber),
								new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Dimension,
										axle.Tyre.Dimension),
								new XElement(tns + XMLNames.AxleWheels_Axles_Axle_RRCDeclared,
										axle.Tyre
										.RollResistanceCoefficient),
								axle.Tyre.FuelEfficiencyClass != null ? new XElement(tns + "FuelEfficiencyClass",
										axle.Tyre
										.FuelEfficiencyClass) : null,
								new XElement(tns + "Hash",
										axle.Tyre.GetHashCode())
						))
					));
			}

			VehiclePart.Add(axles);

			InputDataIntegrity = GetInputDataSignature(modelData);
		}

		protected override XElement GetSimulationParameters(XMLDeclarationReport.ResultEntry result)
		{
			return XMLReportHelperTrailer.GetSimulationParameters(result, _trailerSegment, tns);
		}

		protected override XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			return null;
		}



		public override void WriteResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			_allSuccess &= resultEntry.Status == VectoRun.Status.Success;
			if (resultEntry.Status == VectoRun.Status.Success) {
				//TODO: identifies trailer missions with PassengerCount - need to find a better way to identify trailer missions
				if (resultEntry.PassengerCount != null && resultEntry.PassengerCount == 1) {
					WeightedPayloadTrailer += resultEntry.Payload * resultEntry.WeightingFactor;
					WeightedCo2Trailer += (resultEntry.CO2Total.ConvertToGramm() / resultEntry.Distance.ConvertToKiloMeter()) * resultEntry.WeightingFactor;
					//WeightedFcTrailer +=
					//	(resultEntry.FuelConsumptionFinal[resultEntry.FuelData.FirstOrDefault().FuelType]
					//			.ConvertToGramm() / resultEntry.FuelData.FirstOrDefault().FuelDensity /
					//		resultEntry.Distance.ConvertToKiloMeter() * 100).Value() * resultEntry.WeightingFactor;
					foreach (var finalFuelConsumption in resultEntry.FuelConsumptionFinal) {
						var fuelData = resultEntry.FuelData.Single((properties =>
							properties.FuelType == finalFuelConsumption.Key));
						var entryExists = WeightedFcPerFuelType.TryGetValue(finalFuelConsumption.Key, out var prevValue);
						if (!entryExists) {
							WeightedFcPerFuelType.Add(finalFuelConsumption.Key,
								new XMLReportHelperTrailer.WeightedFc());
						}
						WeightedFcPerFuelType[finalFuelConsumption.Key].WeightedFcLitrePerKilometer = 
							(entryExists ? prevValue.WeightedFcLitrePerKilometer : 0) 
							+ (finalFuelConsumption.Value
								/ fuelData.FuelDensity 
								/ resultEntry.Distance.ConvertToKiloMeter()).Value()
								* resultEntry.WeightingFactor;
						WeightedFcPerFuelType[finalFuelConsumption.Key].WeightedFcGramPerKilometer =
							(entryExists ? prevValue.WeightedFcGramPerKilometer : 0) + (finalFuelConsumption.Value.ConvertToGramm() / resultEntry.Distance.ConvertToKiloMeter()) * resultEntry.WeightingFactor;
					}
				} 
				else {
					ReferenceWeightedPayload += resultEntry.Payload * resultEntry.WeightingFactor;
					ReferenceWeightedCo2 += (resultEntry.CO2Total.ConvertToGramm() / resultEntry.Distance.ConvertToKiloMeter()) * resultEntry.WeightingFactor;
					return;
				}

			}

			Results.Add(
				resultEntry.Status == VectoRun.Status.Success ? GetSuccessResult(resultEntry) : GetErrorResult(resultEntry));
		}

		protected override XElement GetSuccessResult(XMLDeclarationReport.ResultEntry result)
		{
			var ratio = 0.0;
			var ratioTons = 0.0;
			var ratioCubicMeter = 0.0;

			if (Mappings.Count(x => x.Key.Mission == result.Mission && x.Key.LoadingType == result.LoadingType) > 0)
			{
				var key = Mappings
					.FirstOrDefault(x => x.Key.Mission == result.Mission && x.Key.LoadingType == result.LoadingType)
					.Key;
				var value = Mappings
					.FirstOrDefault(x => x.Key.Mission == result.Mission && x.Key.LoadingType == result.LoadingType)
					.Value;
				ratio = XMLReportHelperTrailer.CalculateFactor(key, value);
				ratioTons = XMLReportHelperTrailer.DoCalculateFactor(key.CO2Total, key.Distance, key.Payload, value.CO2Total, value.Distance,
					value.Payload);
				ratioCubicMeter = XMLReportHelperTrailer.DoCalculateFactor(key.CO2Total, SpecificCargoVolume, value.CO2Total, ReferenceCargoVolume);
			}
			
			return new XElement(
				tns + XMLNames.Report_Result_Result,
				new XAttribute(
					XMLNames.Report_Result_Status_Attr, "success"),
				new XAttribute(xsi + "type", "ResultSuccessType"),
				new XElement(tns + XMLNames.Report_Result_Mission, result.Mission.ToXMLFormat()),
				GetSimulationParameters(result), XMLReportHelperTrailer.GetResults(result, tns).Cast<object>().ToArray(), 
				XMLReportHelperTrailer.GetFactorWithAttributes(tns + XMLNames.Efficiency_Ratio, ratio, XMLNames.Result_RatioType_KilometreBased, 3), 
				XMLReportHelperTrailer.GetFactorWithAttributes(tns+XMLNames.Efficiency_Ratio, ratioTons, XMLNames.Result_RatioType_TonKilometreBased, 3), 
				XMLReportHelperTrailer.GetFactorWithAttributes(tns + XMLNames.Efficiency_Ratio, ratioCubicMeter, XMLNames.Result_RatioType_CubicMetre_KilometreBased, 3)
			);
		}


		public override void GenerateReport()
		{
			var weightedCo2Ton = WeightedCo2Trailer / WeightedPayloadTrailer.ConvertToTon();
			var weightedReferenceCo2Ton = ReferenceWeightedCo2 / ReferenceWeightedPayload.ConvertToTon();
			var weightedCo2CubicMeter = (WeightedCo2Trailer / SpecificCargoVolume);
			var weightedReferenceCo2CubicMeter = (ReferenceWeightedCo2 / ReferenceCargoVolume);


			var mrf = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput");
			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + "TowingVehicle", _genericTowingVehicle),
				new XElement(tns + XMLNames.Report_Result_Status, _allSuccess ? "success" : "error"));

			var summary = _allSuccess && WeightedPayloadTrailer > 0
				? new XElement(tns + XMLNames.Report_Results_WeightedResults,
					new XElement(tns + XMLNames.Report_Result_Payload,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_kg),
						WeightedPayloadTrailer.ToXMLFormat(0)
					),
					XMLReportHelperTrailer.GetWeightedFuelConsumption(WeightedFcPerFuelType,WeightedPayloadTrailer,SpecificCargoVolume, tns),
					//new XElement(tns + XMLNames.Report_Results_FuelConsumption,
					//	new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/100km"),
					//	WeightedFcTrailer.ToXMLFormat(4)
					//),
					//new XElement(tns + XMLNames.Report_Results_FuelConsumption,
					//	new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/t-km"),
					//	WeightedFcTrailerTon.ToXMLFormat(4)
					//),
					//new XElement(tns + XMLNames.Report_Results_FuelConsumption,
					//	new XAttribute(XMLNames.Report_Results_Unit_Attr, "l/m³-km"),
					//	WeightedFcTrailerCubicMeter.ToXMLFormat(4)
					//),
					new XElement(tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
						WeightedCo2Trailer.ToXMLFormat(1)
					),
					new XElement(tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/t-km"),
						weightedCo2Ton.ToXMLFormat(2)
					),
					new XElement(tns + XMLNames.Report_Results_CO2,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/m³-km"),
						weightedCo2CubicMeter.ToXMLFormat(3)
					),
					XMLReportHelperTrailer.GetFactorWithAttributes(tns + XMLNames.Efficiency_Ratio,
						(WeightedCo2Trailer / ReferenceWeightedCo2),
						XMLNames.Result_RatioType_KilometreBased, 4),
					XMLReportHelperTrailer.GetFactorWithAttributes(tns + XMLNames.Efficiency_Ratio,
						(weightedCo2Ton / weightedReferenceCo2Ton),
						XMLNames.Result_RatioType_TonKilometreBased, 4),
					XMLReportHelperTrailer.GetFactorWithAttributes(tns + XMLNames.Efficiency_Ratio,
						(weightedCo2CubicMeter / weightedReferenceCo2CubicMeter).Value(),
						XMLNames.Result_RatioType_CubicMetre_KilometreBased, 4)
				) : null;
			results.Add(summary);
			var vehicle = new XElement(VehiclePart);
			vehicle.Add(InputDataIntegrity);
			retVal.Add(
				new XProcessingInstruction(
					"xml-stylesheet", "href=\"VectoReportsTrailer.css\""));
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
