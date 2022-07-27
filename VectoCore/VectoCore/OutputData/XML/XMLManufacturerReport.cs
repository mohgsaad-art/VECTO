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
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLManufacturerReport
	{
		public const string SCHEMA_VERSION_STRING = "0.7";
		public const string CURRENT_SCHEMA_VERSION = "0.7.2";
		
		protected XElement VehiclePart;
		
		protected XElement InputDataIntegrity;

		protected XElement Results;

		protected XNamespace tns;
		protected XNamespace di;

		private bool _allSuccess = true;

		public XMLManufacturerReport(XMLDeclarationReport xmlDeclarationReport)
		{
			DeclarationReport = xmlDeclarationReport;
			di = "http://www.w3.org/2000/09/xmldsig#";
			tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v" + CURRENT_SCHEMA_VERSION;
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
				new XElement(tns + XMLNames.Vehicle_DualFuelVehicle, exempted ? modelData.VehicleData.DualFuelVehicle : modelData.EngineData.Fuels.Count > 1),
				exempted 
					? ExemptedData(modelData) 
					: new[] {
						new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
						new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
						new XElement(tns + XMLNames.Report_CO2StandardsGroup, DeclarationReport.WeightingGroup.ToXMLFormat()), 
						new XElement(tns + XMLNames.Vehicle_VocationalVehicle, modelData.VehicleData.VocationalVehicle),
						new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab),
						new XElement(tns + XMLNames.Vehicle_PTO, modelData.PTO != null),
						GetADAS(modelData.VehicleData.InputData.ADAS),
						GetTorqueLimits(modelData.EngineData),
						VehicleComponents(modelData)
					}
				);
			if (exempted) {
				Results.Add(new XElement(tns + XMLNames.Report_ExemptedVehicle));
			}
			InputDataIntegrity = new XElement(tns + XMLNames.Report_Input_Signature,
				modelData.InputDataHash == null ? CreateDummySig() : new XElement(modelData.InputDataHash));
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

		private XElement VehicleComponents(VectoRunData modelData)
		{
			return new XElement(tns + XMLNames.Vehicle_Components,
								GetEngineDescription(modelData.EngineData),
								GetGearboxDescription(modelData.GearboxData),
								GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
								GetRetarderDescription(modelData.Retarder),
								GetAngledriveDescription(modelData.AngledriveData),
								GetAxlegearDescription(modelData.AxleGearData),
								GetAirDragDescription(modelData.AirdragData),
								GetAxleWheelsDescription(modelData.VehicleData),
								GetAuxiliariesDescription(modelData.Aux)
			);
		}

		private XElement[] ExemptedData(VectoRunData modelData)
		{
			var retVal = new List<XElement>();

			if (modelData.VehicleData.AxleConfiguration != AxleConfiguration.AxleConfig_Undefined) {
				retVal.Add(new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()));
				retVal.Add(new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()));
			}

			if (modelData.VehicleData.SleeperCab.HasValue) {
				retVal.Add(new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab.Value));
			}

			if (modelData.VehicleData.MaxNetPower1 != null) 
			 retVal.Add(new XElement(tns + XMLNames.Vehicle_MaxNetPower1,
				XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower1, XMLNames.Unit_W)));
			if (modelData.VehicleData.MaxNetPower2 != null) {
				retVal.Add(new XElement(tns + XMLNames.Vehicle_MaxNetPower2,
					XMLHelper.ValueAsUnit(modelData.VehicleData.MaxNetPower2, XMLNames.Unit_W)));
			}

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

		private XElement GetTorqueLimits(CombustionEngineData modelData)
		{
			var limits = new List<XElement>();
			var maxTorque = modelData.FullLoadCurves[0].MaxTorque;
			for (uint i = 1; i < modelData.FullLoadCurves.Count; i++) {
				if (!maxTorque.IsEqual(modelData.FullLoadCurves[i].MaxTorque, 1e-3.SI<NewtonMeter>())) {
					limits.Add(
						new XElement(
							tns + XMLNames.Vehicle_TorqueLimits_Entry,
							new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, i),
							new XAttribute(
								XMLNames.XMLManufacturerReport_torqueLimit,
								modelData.FullLoadCurves[i].MaxTorque.ToXMLFormat(0)),
							new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_Nm),
							new XAttribute(
								XMLNames.XMLManufacturerReport_torqueLimitPercent,
								(modelData.FullLoadCurves[i].MaxTorque / maxTorque * 100).ToXMLFormat(1))));
				}
			}

			return limits.Count == 0
				? null
				: new XElement(tns + XMLNames.Vehicle_TorqueLimits, limits.Cast<object>().ToArray());
		}

		private XElement GetEngineDescription(CombustionEngineData engineData)
		{
			var retVal = new XElement(tns + XMLNames.Component_Engine,
				GetCommonDescription(engineData),
				new XElement(tns + XMLNames.Engine_RatedPower, XMLHelper.ValueAsUnit(engineData.RatedPowerDeclared, XMLNames.Unit_kW)),
				new XElement(tns + XMLNames.Engine_IdlingSpeed, XMLHelper.ValueAsUnit(engineData.IdleSpeed, XMLNames.Unit_RPM)),
				new XElement(tns + XMLNames.Engine_RatedSpeed, XMLHelper.ValueAsUnit(engineData.RatedSpeedDeclared, XMLNames.Unit_RPM)),
				new XElement(tns + XMLNames.Engine_Displacement, XMLHelper.ValueAsUnit(engineData.Displacement, XMLNames.Unit_ltr, 1)),
				new XElement(tns + "FuelTypes",
					engineData.Fuels.Select(x => new XElement(tns + XMLNames.Engine_FuelType, x.FuelData.FuelType.ToXMLFormat()))
					),
				new XElement(tns + "WasteHeatRecoverySystem", engineData.WHRType != WHRType.None)
			);

			foreach (WHRType whrType in Enum.GetValues(typeof(WHRType))) {
				if (whrType == WHRType.None) {
					continue;
				}

				if ((whrType & engineData.WHRType) == whrType) {
					retVal.Add(new XElement(tns + "WasteHeatRecoverySystemType", whrType.ToXMLFormat()));
				}
			}
			return retVal;
		}

		private XElement GetGearboxDescription(GearboxData gearboxData)
		{
			return new XElement(tns + XMLNames.Component_Gearbox,
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, gearboxData.Gears.Count),
				new XElement(tns + XMLNames.Report_Gearbox_TransmissionRatioFinalGear,
					gearboxData.Gears[gearboxData.Gears.Keys.Max()].Ratio.ToXMLFormat(3))
				);
		}

		private XElement GetTorqueConverterDescription(TorqueConverterData torqueConverterData)
		{
			if (torqueConverterData == null) {
				return null;
			}
			return new XElement(tns + XMLNames.Component_TorqueConverter,
				GetCommonDescription(torqueConverterData));
		}

		private XElement GetRetarderDescription(RetarderData retarder)
		{
			return new XElement(tns + XMLNames.Component_Retarder,
				new XElement(tns + XMLNames.Vehicle_RetarderType, retarder.Type.ToXMLFormat()),
				retarder.Type.IsDedicatedComponent() ? GetCommonDescription(retarder) : null);
		}

		private object GetAngledriveDescription(AngledriveData angledriveData)
		{
			if (angledriveData == null) {
				return null;
			}
			return new XElement(tns + XMLNames.Component_Angledrive,
				GetCommonDescription(angledriveData),
				new XElement(tns + XMLNames.AngleDrive_Ratio, angledriveData.Angledrive.Ratio.ToXMLFormat(3)));
		}

		private XElement GetAxlegearDescription(AxleGearData axleGearData)
		{
			return new XElement(tns + XMLNames.Component_Axlegear,
				GetCommonDescription(axleGearData),
				new XElement(tns + XMLNames.Axlegear_LineType, axleGearData.LineType.ToXMLFormat()),
				new XElement(tns + XMLNames.Axlegear_Ratio, axleGearData.AxleGear.Ratio.ToXMLFormat(3)));
		}

		private XElement GetAirDragDescription(AirdragData airdragData)
		{
			if (airdragData.CertificationMethod == CertificationMethod.StandardValues) {
				return new XElement(tns + XMLNames.Component_AirDrag,
					new XElement(tns + XMLNames.Report_Component_CertificationMethod, airdragData.CertificationMethod.ToXMLFormat()),
					new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragAreaInput.ToXMLFormat(2))
					);
			}
			return new XElement(tns + XMLNames.Component_AirDrag,
				new XElement(tns + XMLNames.Component_Model, airdragData.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationMethod, airdragData.CertificationMethod.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, airdragData.CertificationNumber),
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, airdragData.DigestValueInput),
				new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragAreaInput.ToXMLFormat(2))
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
			return new XElement(tns + XMLNames.AxleWheels_Axles_Axle,
				new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, i),
				new XElement(tns + XMLNames.Report_Tyre_TyreDimension, axle.WheelsDimension),
				new XElement(tns + XMLNames.Report_Tyre_TyreCertificationNumber, axle.CertificationNumber),
				new XElement(tns+XMLNames.DI_Signature_Reference_DigestValue, axle.DigestValueInput),
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

		private object[] GetCommonDescription(CombustionEngineData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, data.DigestValueInput)
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
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, data.DigestValueInput)
			};
		}

		public void WriteResult(
			DeclarationReport<XMLDeclarationReport.ResultEntry>.ResultContainer<XMLDeclarationReport.ResultEntry> entry)
		{
			foreach (var resultEntry in entry.ResultEntry) {
				_allSuccess &= resultEntry.Value.Status == VectoRun.Status.Success;
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
						new XElement(tns + XMLNames.Report_Results_Error, resultEntry.Value.Error),
						new XElement(tns + XMLNames.Report_Results_ErrorDetails, resultEntry.Value.StackTrace),
					};
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private object[] GetSuccessResultEntry(XMLDeclarationReport.ResultEntry result)
		{
			return new object[] {
				new XElement(tns + XMLNames.Report_ResultEntry_Distance, new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_km),
					result.Distance.ConvertToKiloMeter().ToXMLFormat(3)),
				new XElement(tns + XMLNames.Report_ResultEntry_SimulationParameters,
					new XElement(tns + XMLNames.Report_ResultEntry_TotalVehicleMass, XMLHelper.ValueAsUnit(result.TotalVehicleWeight, XMLNames.Unit_kg)),
					new XElement(tns + XMLNames.Report_ResultEntry_Payload, XMLHelper.ValueAsUnit(result.Payload, XMLNames.Unit_kg)),
					result.FuelData.Select(x => new XElement(tns + XMLNames.Report_ResultEntry_FuelType, XMLHelper.ToXmlStr(DeclarationData.FuelData.Lookup(x.FuelType, x.TankSystem))))
					),
				new XElement(tns + XMLNames.Report_ResultEntry_VehiclePerformance,
					new XElement(tns + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(result.AverageSpeed, XMLNames.Unit_kmph, 1)),
					new XElement(tns + XMLNames.Report_ResultEntry_AvgDrivingSpeed, XMLHelper.ValueAsUnit(result.AverageDrivingSpeed, XMLNames.Unit_kmph, 1)),
					new XElement(tns + XMLNames.Report_ResultEntry_MinSpeed, XMLHelper.ValueAsUnit(result.MinSpeed, XMLNames.Unit_kmph, 1)),
					new XElement(tns + XMLNames.Report_ResultEntry_MaxSpeed, XMLHelper.ValueAsUnit(result.MaxSpeed, XMLNames.Unit_kmph, 1)),
					new XElement(tns + XMLNames.Report_ResultEntry_MaxDeceleration, XMLHelper.ValueAsUnit(result.MaxDeceleration, XMLNames.Unit_mps2, 2)),
					new XElement(tns + XMLNames.Report_ResultEntry_MaxAcceleration, XMLHelper.ValueAsUnit(result.MaxAcceleration, XMLNames.Unit_mps2, 2)),
					new XElement(tns + XMLNames.Report_ResultEntry_FullLoadDrivingtimePercentage,
						result.FullLoadPercentage.ToXMLFormat(2)),
					new XElement(tns + XMLNames.Report_ResultEntry_GearshiftCount, result.GearshiftCount.ToXMLFormat(0)),
					new XElement(tns + XMLNames.Report_ResultEntry_EngineSpeedDriving, 
						new XElement(tns + XMLNames.Report_ResultEntry_EngineSpeedDriving_Min, XMLHelper.ValueAsUnit(result.EngineSpeedDrivingMin, XMLNames.Unit_RPM, 1)),
						new XElement(tns + XMLNames.Report_ResultEntry_EngineSpeedDriving_Avg, XMLHelper.ValueAsUnit(result.EngineSpeedDrivingAvg, XMLNames.Unit_RPM, 1)),
						new XElement(tns + XMLNames.Report_ResultEntry_EngineSpeedDriving_Max, XMLHelper.ValueAsUnit(result.EngineSpeedDrivingMax, XMLNames.Unit_RPM, 1))
						),
					new XElement(tns + XMLNames.Report_Results_AverageGearboxEfficiency, XMLHelper.ValueAsUnit(result.AverageGearboxEfficiency, XMLNames.UnitPercent, 2)),
					new XElement(tns + XMLNames.Report_Results_AverageAxlegearEfficiency, XMLHelper.ValueAsUnit(result.AverageAxlegearEfficiency, XMLNames.UnitPercent, 2))
					),
				//FC
				XMLDeclarationReport.GetResults(result, tns, true).Cast<object>().ToArray()
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

		public void GenerateReport()
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, _allSuccess ? "success" : "error"));
			var vehicle = new XElement(VehiclePart);
			vehicle.Add(InputDataIntegrity);
			retVal.Add(new XProcessingInstruction("xml-stylesheet", "href=\"https://citnet.tech.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(new XElement(tns + XMLNames.VectoManufacturerReport,
				new XAttribute("schemaVersion", SCHEMA_VERSION_STRING),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "di", di),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}VectoOutputManufacturer.{2}.xsd", tns, AbstractXMLWriter.SchemaLocationBaseUrl, CURRENT_SCHEMA_VERSION)),
				new XElement(tns + XMLNames.Report_DataWrap,
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

		public XDocument Report { get; private set; }
	}
}
