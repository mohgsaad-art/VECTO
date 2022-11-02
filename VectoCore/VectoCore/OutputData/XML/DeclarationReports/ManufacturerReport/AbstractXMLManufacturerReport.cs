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
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport
{
	public abstract class AbstractXMLManufacturerReport : IXMLManufacturerReport
	{
		protected XElement VehiclePart;
		protected XElement Results;
		protected XElement InputDataIntegrity;

		protected bool _allSuccess = true;

		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		protected XNamespace tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.8";
		protected XNamespace vns;
		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";

		protected AbstractXMLManufacturerReport()
		{
			vns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.8";
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + XMLNames.Report_Results);
		}

		public virtual XDocument Report { get; protected set; }

		public abstract void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes);

		public virtual void GenerateReport()
		{
			var mrf = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput");
			var retVal = new XDocument();
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, _allSuccess ? "success" : "error"));
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
					new XAttribute("xmlns", vns),
					new XAttribute(XNamespace.Xmlns + "di", di),
                    new XAttribute(XNamespace.Xmlns + "tns", tns),
					new XAttribute(XNamespace.Xmlns + "vns", vns),
                    new XAttribute(XNamespace.Xmlns + "mrf", mrf),
					new XAttribute(
						xsi + "schemaLocation",
						$"{mrf} {AbstractXMLWriter.SchemaLocationBaseUrl}/DEV/VectoOutputManufacturer.xsd"),
					new XElement(
						mrf + XMLNames.Report_DataWrap,
						new XAttribute(xsi + "type", "tns:VectoOutputDataType"),
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


		public virtual void WriteResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			_allSuccess &= resultEntry.Status == VectoRun.Status.Success;
			Results.Add(
				resultEntry.Status == VectoRun.Status.Success ? GetSuccessResult(resultEntry) : GetErrorResult(resultEntry));
		}

		protected virtual XElement GetErrorResult( XMLDeclarationReport.ResultEntry resultEntry)
		{
			var content = new object[] {};
			switch (resultEntry.Status) {
				case VectoRun.Status.Pending:
				case VectoRun.Status.Running:
					content = new object[] {
						GetSimulationParameters(resultEntry),
						new XElement(
							tns + XMLNames.Report_Results_Error,
							$"Simulation not finished! Status: {resultEntry.Status}"),
						new XElement(tns + XMLNames.Report_Results_ErrorDetails, ""),
					}; // should not happen!
					break;
				case VectoRun.Status.Canceled:
				case VectoRun.Status.Aborted:
					content =  new object[] {
						GetSimulationParameters(resultEntry),
						new XElement(tns + XMLNames.Report_Results_Error, resultEntry.Error),
						new XElement(tns + XMLNames.Report_Results_ErrorDetails, resultEntry.StackTrace),
					};
					break;
				default: throw new ArgumentOutOfRangeException();
			}

			return new XElement(
				tns + XMLNames.Report_Result_Result,
				new XAttribute(
					XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				new XElement(tns + XMLNames.Report_Result_Mission, resultEntry.Mission.ToXMLFormat()),
				content);
		}

		protected virtual XElement GetSuccessResult(XMLDeclarationReport.ResultEntry result)
		{
			return new XElement(
				tns + XMLNames.Report_Result_Result,
				new XAttribute(
					XMLNames.Report_Result_Status_Attr, "success"),
				new XAttribute(xsi + "type", "ResultSuccessType"),
				new XElement(tns + XMLNames.Report_Result_Mission, result.Mission.ToXMLFormat()), 
				new XElement(
					tns + XMLNames.Report_ResultEntry_Distance, new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_km),
					result.Distance.ConvertToKiloMeter().ToXMLFormat(3)),
				GetSimulationParameters(result),
				GetVehiclePerformance(result),
				//FC
				XMLDeclarationReport.GetResults(result, tns, true).Cast<object>().ToArray()
			);
		}

		private XElement GetVehiclePerformance(XMLDeclarationReport.ResultEntry result)
		{
			return new XElement(
				tns + XMLNames.Report_ResultEntry_VehiclePerformance,
				new XElement(
					tns + XMLNames.Report_ResultEntry_AverageSpeed,
					XMLHelper.ValueAsUnit(result.AverageSpeed, XMLNames.Unit_kmph, 1)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_AvgDrivingSpeed,
					XMLHelper.ValueAsUnit(result.AverageDrivingSpeed, XMLNames.Unit_kmph, 1)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_MinSpeed, XMLHelper.ValueAsUnit(result.MinSpeed, XMLNames.Unit_kmph, 1)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_MaxSpeed, XMLHelper.ValueAsUnit(result.MaxSpeed, XMLNames.Unit_kmph, 1)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_MaxDeceleration,
					XMLHelper.ValueAsUnit(result.MaxDeceleration, XMLNames.Unit_mps2, 2)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_MaxAcceleration,
					XMLHelper.ValueAsUnit(result.MaxAcceleration, XMLNames.Unit_mps2, 2)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_FullLoadDrivingtimePercentage,
					result.FullLoadPercentage.ToXMLFormat(2)),
				new XElement(tns + XMLNames.Report_ResultEntry_GearshiftCount, result.GearshiftCount.ToXMLFormat(0)),
				new XElement(
					tns + XMLNames.Report_ResultEntry_EngineSpeedDriving,
					new XElement(
						tns + XMLNames.Report_ResultEntry_EngineSpeedDriving_Min,
						XMLHelper.ValueAsUnit(result.EngineSpeedDrivingMin, XMLNames.Unit_RPM, 1)),
					new XElement(
						tns + XMLNames.Report_ResultEntry_EngineSpeedDriving_Avg,
						XMLHelper.ValueAsUnit(result.EngineSpeedDrivingAvg, XMLNames.Unit_RPM, 1)),
					new XElement(
						tns + XMLNames.Report_ResultEntry_EngineSpeedDriving_Max,
						XMLHelper.ValueAsUnit(result.EngineSpeedDrivingMax, XMLNames.Unit_RPM, 1))
				),
				new XElement(
					tns + XMLNames.Report_Results_AverageGearboxEfficiency,
					XMLHelper.ValueAsUnit(result.AverageGearboxEfficiency, XMLNames.UnitPercent, 2)),
				new XElement(
					tns + XMLNames.Report_Results_AverageAxlegearEfficiency,
					XMLHelper.ValueAsUnit(result.AverageAxlegearEfficiency, XMLNames.UnitPercent, 2))
			);
		}

		protected virtual XElement GetSimulationParameters(XMLDeclarationReport.ResultEntry result)
		{
			return new XElement(
				tns + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(
					tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(result.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Report_ResultEntry_Payload, XMLHelper.ValueAsUnit(result.Payload, XMLNames.Unit_kg)),
				result.PassengerCount.HasValue && result.PassengerCount.Value > 0 ? new XElement(tns + "PassengerCount", result.PassengerCount.Value.ToMinSignificantDigits(3,1)) : null,
				new XElement(
					tns + XMLNames.Report_Result_FuelMode,
					result.FuelData.Count > 1 ? XMLNames.Report_Result_FuelMode_Val_Dual : XMLNames.Report_Result_FuelMode_Val_Single)
			);
		}

		protected virtual XElement GetInputDataSignature(VectoRunData modelData)
		{
			return new XElement(
				vns + XMLNames.Report_Input_Signature,
				modelData.InputDataHash == null ? CreateDummySig() : new XElement(modelData.InputDataHash));
		}

		protected virtual XElement CreateDummySig()
		{
			return new XElement(
				di + XMLNames.DI_Signature_Reference,
				new XElement(
					di + XMLNames.DI_Signature_Reference_DigestMethod,
					new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "null")),
				new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, "NOT AVAILABLE")
			);
		}

		protected virtual XElement GetADAS(VehicleData.ADASData adasData)
		{
			if (adasData.InputData.XMLSource == null || XMLHelper.GetVersion(adasData.InputData.XMLSource) < 2.0) {
				return CreateADAS(adasData);
			}

			var ns = XNamespace.Get(adasData.InputData.XMLSource.SchemaInfo.SchemaType.QualifiedName.Namespace);
			var type = adasData.InputData.XMLSource.SchemaInfo.SchemaType.QualifiedName.Name;
				
			const string adasPrefix = "adas";
			return new XElement(
				tns + XMLNames.Vehicle_ADAS,
				new XAttribute(XNamespace.Xmlns + adasPrefix, ns.NamespaceName),
				new XAttribute(xsi + "type", $"{adasPrefix}:{type}"),
				XElement.Parse(adasData.InputData.XMLSource.OuterXml).Elements()
			);
		}

		protected virtual XElement CreateADAS(VehicleData.ADASData adasData)
		{
			var ns = XMLADASReaderV21.NAMESPACE_URI;
			var type = XMLADASReaderV21.XSD_TYPE;

			const string adasPrefix = "adas";
			return new XElement(
				tns + XMLNames.Vehicle_ADAS,
				new XAttribute(XNamespace.Xmlns + adasPrefix, ns.NamespaceName),
				new XAttribute(xsi + "type", $"{adasPrefix}:{type}"),
					new XElement(ns + XMLNames.Vehicle_ADAS_EngineStopStart, adasData.EngineStopStart),
					new XElement(ns + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adasData.EcoRoll.WithoutEngineStop()),
					new XElement(ns + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adasData.EcoRoll.WithEngineStop()),
					new XElement(ns + XMLNames.Vehicle_ADAS_PCC, adasData.PredictiveCruiseControl.ToXMLFormat())
					//new XElement(ns + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch, adasData.InputData.ATEcoRollReleaseLockupClutch ?? false)
			);
		}

		protected virtual XElement GetTorqueLimits(CombustionEngineData modelData)
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

		protected abstract XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes);

		protected virtual XElement GetEngineDescription(CombustionEngineData engineData, List<List<FuelData.Entry>> fuelModes)
		{
			return new XElement(
				tns + XMLNames.Component_Engine,
				GetCommonDescription(engineData),
				new XElement(
					tns + XMLNames.Engine_RatedPower, XMLHelper.ValueAsUnit(engineData.RatedPowerDeclared, XMLNames.Unit_kW)),
				new XElement(tns + XMLNames.Engine_IdlingSpeed, XMLHelper.ValueAsUnit(engineData.IdleSpeed, XMLNames.Unit_RPM)),
				new XElement(
					tns + XMLNames.Engine_RatedSpeed, XMLHelper.ValueAsUnit(engineData.RatedSpeedDeclared, XMLNames.Unit_RPM)),
				new XElement(
					tns + XMLNames.Engine_Displacement, XMLHelper.ValueAsUnit(engineData.Displacement, XMLNames.Unit_ltr, 1)),
				new XElement(tns + XMLNames.Engine_WHRType, engineData.WHRType.ToXMLFormat()),
				fuelModes.Select(
					x => new XElement(
						tns + XMLNames.Report_Engine_FuelMode,
						x.Select(f => new XElement(tns + XMLNames.Engine_FuelType, f.FuelType.ToXMLFormat()))))
			);
		}

		protected virtual XElement GetGearboxDescription(GearboxData gearboxData)
		{
			return new XElement(
				tns + XMLNames.Component_Gearbox,
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, gearboxData.Gears.Count),
				new XElement(
					tns + XMLNames.Report_Gearbox_TransmissionRatioFinalGear,
					gearboxData.Gears[gearboxData.Gears.Keys.Max()].Ratio.ToXMLFormat(3))
			);
		}

		protected virtual XElement GetGearboxDescription(GearboxData gearboxData, AxleGearData axlegearData)
		{
			return new XElement(
				tns + XMLNames.Component_Gearbox,
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Report_GetGearbox_GearsCount, gearboxData.Gears.Count),
				new XElement(tns + XMLNames.Gearbox_AxlegearRatio, axlegearData.AxleGear.Ratio.ToXMLFormat(3)),
				new XElement(
					tns + XMLNames.Report_Gearbox_TransmissionRatioFinalGear,
					gearboxData.Gears[gearboxData.Gears.Keys.Max()].Ratio.ToXMLFormat(3))
			);
		}


		protected virtual XElement GetTorqueConverterDescription(TorqueConverterData torqueConverterData)
		{
			if (torqueConverterData == null) {
				return null;
			}

			return new XElement(
				tns + XMLNames.Component_TorqueConverter,
				GetCommonDescription(torqueConverterData));
		}

		protected virtual XElement GetRetarderDescription(RetarderData retarder)
		{
			return new XElement(
				tns + XMLNames.Component_Retarder,
				new XElement(tns + XMLNames.Vehicle_RetarderType, retarder.Type.ToXMLFormat()),
				retarder.Type.IsDedicatedComponent() ? GetCommonDescription(retarder) : null);
		}

		protected virtual XElement GetAngledriveDescription(AngledriveData angledriveData)
		{
			if (angledriveData == null) {
				return null;
			}

			return new XElement(
				tns + XMLNames.Component_Angledrive,
				GetCommonDescription(angledriveData),
				new XElement(tns + XMLNames.AngleDrive_Ratio, angledriveData.Angledrive.Ratio.ToXMLFormat(3)));
		}

		protected virtual XElement GetAxlegearDescription(AxleGearData axleGearData)
		{
			return new XElement(
				tns + XMLNames.Component_Axlegear,
				GetCommonDescription(axleGearData),
				new XElement(tns + XMLNames.Axlegear_LineType, axleGearData.LineType.ToXMLFormat()),
				new XElement(tns + XMLNames.Axlegear_Ratio, axleGearData.AxleGear.Ratio.ToXMLFormat(3)));
		}

		protected virtual XElement GetAirDragDescription(AirdragData airdragData)
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
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, airdragData.DigestValueInput),
				new XElement(tns + XMLNames.Report_AirDrag_CdxA, airdragData.DeclaredAirdragArea.ToXMLFormat(2))
			);
		}

		protected virtual XElement GetAxleWheelsDescription(VehicleData vehicleData)
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

		protected virtual XElement GetAxleDescription(int i, Axle axle)
		{
			return new XElement(
				tns + XMLNames.AxleWheels_Axles_Axle,
				new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, i),
				new XElement(tns + XMLNames.Report_Tyre_TyreDimension, axle.WheelsDimension),
				new XElement(tns + XMLNames.Report_Tyre_TyreCertificationNumber, axle.CertificationNumber),
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, axle.DigestValueInput),
				new XElement(tns + XMLNames.Report_Tyre_TyreRRCDeclared, axle.RollResistanceCoefficient.ToXMLFormat(4)),
				new XElement(tns + XMLNames.AxleWheels_Axles_Axle_TwinTyres, axle.TwinTyres));
		}

		protected virtual object[] GetCommonDescription(CombustionEngineData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.DI_Signature_Reference_DigestValue, data.DigestValueInput)
			};
		}

		protected virtual object[] GetCommonDescription(SimulationComponentData data)
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

		protected virtual XElement GetAuxiliariesDescription(VectoRunData modelData)
		{
			return null;
		}
	}
}
