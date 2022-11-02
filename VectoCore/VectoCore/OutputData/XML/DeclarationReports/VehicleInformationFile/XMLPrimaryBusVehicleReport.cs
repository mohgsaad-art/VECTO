using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile
{
	public class XMLPrimaryBusVehicleReport : IXMLPrimaryVehicleReport
	{
		protected XNamespace tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";
		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";
		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected XNamespace v20 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0";
		protected XNamespace v21 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.1";
		protected XNamespace v23 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3";
		protected XNamespace v24 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.4";
		protected XNamespace v10 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
		
		protected XElement VehiclePart;
		protected XElement InputDataIntegrity;
		protected XElement Results;

		protected bool _allSuccess = true;


		public XMLPrimaryBusVehicleReport()
		{
			VehiclePart = new XElement(tns + XMLNames.Component_Vehicle);
			Results = new XElement(tns + XMLNames.Report_Results);
		}

		public XDocument Report { get; protected set; }


		public virtual void GenerateReport(XElement resultSignature)
		{
			var retVal = new XDocument();
			retVal.Add(
				new XElement(tns + XMLNames.VectoOutputMultistep,
					new XAttribute(XNamespace.Xmlns + "di", di),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "vif0.1", tns),
					new XAttribute(XNamespace.Xmlns + "v2.0", v20),
					new XAttribute(XNamespace.Xmlns + "v2.1", v21),
					new XAttribute(XNamespace.Xmlns + "v2.3", v23),
					new XAttribute(XNamespace.Xmlns + "v2.8", v24),
					new XAttribute(xsi + "schemaLocation", $"{tns.NamespaceName} "+ @"V:\VectoCore\VectoCore\Resources\XSD/VectoOutputMultistep.0.1.xsd"),
					new XAttribute("xmlns", tns),

					GeneratePrimaryVehicle(resultSignature))
				);
			
			Debug.WriteLine(retVal.ToString());
			Report = retVal;
		}
		
		protected virtual XElement GeneratePrimaryVehicle(XElement resultSignature)
		{
			var results = new XElement(Results);
			results.AddFirst(new XElement(tns + XMLNames.Report_Result_Status, _allSuccess ? "success" : "error"));
			var vehicleId = $"{VectoComponents.Vehicle.HashIdPrefix()}{GetGUID()}";

			var primaryVehicle = new XElement(tns + XMLNames.Bus_PrimaryVehicle,
				new XElement(tns + XMLNames.Report_DataWrap,
					new XAttribute(XMLNames.Component_ID_Attr, vehicleId),
					new XAttribute(xsi + "type", "PrimaryVehicleDataType"),
					new XAttribute("xmlns", tns),
					VehiclePart,
					InputDataIntegrity,
					new XElement(tns + "ManufacturerRecordSignature", resultSignature),
					results,
					GetApplicationInfo())
			);

			var sigXElement = GetSignatureElement(primaryVehicle);
			primaryVehicle.LastNode.Parent.Add(sigXElement);
			return primaryVehicle;
		}

		protected XElement GetSignatureElement(XElement stage)
		{
			var stream = new MemoryStream();
			//var writer = new StreamWriter(stream);
			//writer.Write(stage);
			var writer = new XmlTextWriter(stream, Encoding.UTF8);
			stage.WriteTo(writer);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			return new XElement(tns + XMLNames.DI_Signature,
				VectoHash.Load(stream).ComputeXmlHash
					(VectoHash.DefaultCanonicalizationMethod, VectoHash.DefaultDigestMethod));
		}

		protected XElement GetApplicationInfo()
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

		public virtual void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XAttribute(xsi + "type", "VehiclePIFType"),
				new XElement(tns + XMLNames.ManufacturerPrimaryVehicle, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.ManufacturerAddressPrimaryVehicle, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(modelData.VehicleData.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Vehicle_LegislativeCategory, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(tns + XMLNames.ChassisConfiguration, modelData.VehicleData.VehicleCategory.ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_Articulated, modelData.VehicleData.InputData.Articulated),
				new XElement(tns + XMLNames.TPMLM, modelData.VehicleData.InputData.GrossVehicleMassRating.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_IdlingSpeed, modelData.EngineData.IdleSpeed.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Vehicle_RetarderType, modelData.Retarder.Type.ToXMLFormat()),
				modelData.Retarder.Type.IsDedicatedComponent()
					? new XElement(tns + XMLNames.Vehicle_RetarderRatio, modelData.Retarder.Ratio.ToXMLFormat(3))
					: null,
				new XElement(tns + XMLNames.Vehicle_AngledriveType, (modelData.AngledriveData?.Type ?? AngledriveType.None).ToXMLFormat()),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				GetADAS(modelData.VehicleData.ADAS),
				GetTorqueLimits(modelData),
				VehicleComponents(modelData, fuelModes)
			);
			
			InputDataIntegrity = new XElement(tns + XMLNames.Report_InputDataSignature,
											modelData.InputDataHash == null ? XMLHelper.CreateDummySig(di) : new XElement(modelData.InputDataHash));
		}

		private XElement GetADAS(VehicleData.ADASData adasData)
		{
			var ns = XNamespace.Get(adasData.InputData.XMLSource.SchemaInfo.SchemaType.QualifiedName.Namespace);
			//const string adasPrefix = "adas";
			return new XElement(
				tns + XMLNames.Vehicle_ADAS,
				new XAttribute(
					xsi + "type",
					$"{adasData.InputData.XMLSource.SchemaInfo.SchemaType.QualifiedName.Name}"),
				new XAttribute("xmlns", ns),
				XElement.Parse(adasData.InputData.XMLSource.OuterXml).Elements()
			);
		}

		private XElement GetTorqueLimits(VectoRunData modelData)
		{
			var inputData = modelData.VehicleData.InputData.XMLSource;
			var tcLimits = inputData.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Vehicle_TorqueLimits));
			if (tcLimits == null) {
				return null;
			}

			var ns = XNamespace.Get(tcLimits.SchemaInfo.SchemaType.QualifiedName.Namespace);
			//const string adasPrefix = "tcl";
			return new XElement(
				tns + XMLNames.Vehicle_TorqueLimits,
				new XAttribute(
					xsi + "type", $"{tcLimits.SchemaInfo.SchemaType.QualifiedName.Name}"),
				new XAttribute("xmlns", ns.NamespaceName),
				XElement.Parse(tcLimits.OuterXml).Elements()
			);
		}

		private XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			return new XElement(
				tns + XMLNames.Vehicle_Components,
				new XAttribute(xsi + "type", "VehicleComponentsPIFType"),
				GetEngineDescription(modelData.EngineData, fuelModes),
				GetGearboxDescription(modelData.GearboxData),
				GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
				GetAngledriveDescription(modelData.AngledriveData),
				GetAxlegearDescription(modelData.AxleGearData),
				GetAxleWheelsDescription(modelData),
				GetAuxiliariesDescription(modelData)
			);
		}

		private XElement GetTorqueConverterDescription(TorqueConverterData torqueConverter)
		{
			if (torqueConverter == null)
				return null;
			
			return new XElement(tns + XMLNames.Component_TorqueConverter,
				new XElement(tns + XMLNames.Report_DataWrap,
					new XAttribute(xsi + "type", "TorqueConverterDataPIFType"),
					GetCommonDescription(torqueConverter),
					new XElement(tns + XMLNames.Component_AppVersion, torqueConverter.AppVersion)
				));
		}
		
		protected virtual XElement GetAxleWheelsDescription(VectoRunData modeldData)
		{
			var axles = modeldData.VehicleData.InputData.Components.AxleWheels.XMLSource;
			var axlesNode = axles.SelectSingleNode(".//*[local-name()='Axles']");
			var axlesNodes = axlesNode.SelectNodes(".//*[local-name()='Axle']");
			
			return new XElement(tns + XMLNames.Component_AxleWheels,
					new XElement(tns + XMLNames.Report_DataWrap, 
						new XAttribute(xsi + "type", "AxleWheelsDataPIFType"),
						new XElement(tns + XMLNames.AxleWheels_Axles,
							GetAxlesNodes(axlesNodes)
						)));
		}


		private List<XElement> GetAxlesNodes(XmlNodeList nodes)
		{
			var axles = new List<XElement>();
			foreach (XmlNode axleNode in nodes) {

				var axleNumber = axleNode.Attributes[XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr].Value;
				var axle = new XElement(tns + XMLNames.AxleWheels_Axles_Axle,
							new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, axleNumber),
							new XAttribute(XNamespace.Xmlns + "v2.0", v20),
							new XAttribute(xsi + "type", "v2.0:AxleDataDeclarationType"),
								XElement.Parse(axleNode.OuterXml.Replace("xsi:type=\"TyreDataDeclarationType\"", "xsi:type=\"v2.0:TyreDataDeclarationType\"")).Elements());
				
				axles.Add(axle);
			}

			return axles;
		}



		private XElement GetAuxiliariesDescription(VectoRunData modelData)
		{
			var aux = modelData.BusAuxiliaries.InputData;
			//var supplyHevPossible = XmlConvert.ToBoolean(
			//	aux.XMLSource.SelectSingleNode(
			//		$".//*[local-name()='{XMLNames.BusAux_ElectricSystem_SupplyFromHEVPossible}']")?.InnerText);


			var result =  new XElement(tns + XMLNames.Component_Auxiliaries,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + "type", "AuxiliaryDataPIFType"),
					//new XAttribute("xmlns", tns.NamespaceName), //automically created
					new XElement(tns + XMLNames.BusAux_Fan, new XElement(tns + XMLNames.BusAux_Technology,  aux.FanTechnology)),
					GetSteeringPumpElement(aux.SteeringPumpTechnology),
					GetElectricSystem(aux.ElectricSupply),
					GetPneumaticSystem(aux.PneumaticSupply, aux.PneumaticConsumers),
					GetHvac(aux.HVACAux))
				);


			return result;
		}


		private XElement GetSteeringPumpElement(IList<string> steeringPumps)
		{
			var technologies = new List<XElement>();

			for (int i = 0; i < steeringPumps.Count; i++)
			{
				var technology = new XElement(tns + XMLNames.BusAux_Technology,
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, i + 1), steeringPumps[i]);
				technologies.Add(technology);
			}

			return new XElement(tns + XMLNames.BusAux_SteeringPump,
				technologies
			);
		}

		private XElement GetElectricSystem(IElectricSupplyDeclarationData electricSupply)
		{
			var alternatorTech = new XElement(tns + XMLNames.Bus_AlternatorTechnology, electricSupply.AlternatorTechnology.ToXMLFormat());

			List<XElement> smartAlternators = null;
			List<XElement> auxBattery = null;
			List<XElement> auxCapacitor = null;

			if (electricSupply.Alternators?.Any() == true) {
				smartAlternators = new List<XElement>();

				foreach (var alternator in electricSupply.Alternators) {
					smartAlternators.Add(new XElement(tns + XMLNames.BusAux_ElectricSystem_SmartAlternator,
									new XElement(tns + XMLNames.BusAux_ElectricSystem_RatedCurrent, alternator.RatedCurrent.Value()),
									new XElement(tns + XMLNames.BusAux_ElectricSystem_RatedRatedVoltage, alternator.RatedVoltage.Value())));
				}
			}

			if (electricSupply.ElectricStorage?.Any() == true) {
				auxBattery = new List<XElement>();
				auxCapacitor = new List<XElement>();

				foreach (var electricStorage in electricSupply.ElectricStorage) {
					if (electricStorage is BusAuxBatteryInputData) {
						var battery = electricStorage as BusAuxBatteryInputData;
						auxBattery.Add(new XElement(tns + XMLNames.BusAux_ElectricSystem_Battery,
							new XElement(tns + XMLNames.BusAux_ElectricSystem_BatteryTechnology, battery.Technology),
							new XElement(tns + XMLNames.BusAux_ElectricSystem_RatedCapacity, battery.Capacity.AsAmpHour),
							new XElement(tns + XMLNames.BusAux_ElectricSystem_NominalVoltage, battery.Voltage.Value())));
					}
					else if (electricStorage is BusAuxCapacitorInputData) {
						var capacitor = electricStorage as BusAuxCapacitorInputData;
						auxCapacitor.Add(new XElement(tns + XMLNames.BusAux_ElectricSystem_Capacitor,
							new XElement(tns + XMLNames.BusAux_ElectricSystem_CapacitorTechnology, capacitor.Technology),
							new XElement(tns + XMLNames.BusAux_ElectricSystem_RatedCapacitance, capacitor.Capacity.Value()),
							new XElement(tns + XMLNames.BusAux_ElectricSystem_RatedVoltage, capacitor.Voltage.Value())));
					}
				}

				auxBattery = auxBattery.Any() ? auxBattery : null;
				auxCapacitor = auxCapacitor.Any() ? auxCapacitor : null;
			}
			
			return new XElement(tns + XMLNames.BusAux_ElectricSystem,
						alternatorTech,
						smartAlternators,
						auxBattery,
						auxCapacitor//,
						//new XElement(tns + XMLNames.BusAux_ElectricSystem_SupplyFromHEVPossible, supplyHevPossible)
			);
		}


		private XElement GetPneumaticSystem(IPneumaticSupplyDeclarationData supply, IPneumaticConsumersDeclarationData consumer)
		{
			return new XElement(tns + XMLNames.BusAux_PneumaticSystem,
				new XElement(tns + XMLNames.Bus_SizeOfAirSupply, supply.CompressorSize),
				new XElement(tns + XMLNames.CompressorDrive, supply.CompressorDrive.GetLabel()),
				new XElement(tns + XMLNames.Vehicle_Clutch, supply.Clutch),
				new XElement(tns + XMLNames.Bus_CompressorRatio, supply.Ratio.ToXMLFormat(3)),
				new XElement(tns + XMLNames.Bus_SmartCompressionSystem, supply.SmartAirCompression),
				new XElement(tns + XMLNames.Bus_SmartRegenerationSystem, supply.SmartRegeneration),
				new XElement(tns + XMLNames.Bus_AirsuspensionControl, GetXMLAirsuspensionControl(consumer.AirsuspensionControl)),
				new XElement(tns + XMLNames.BusAux_PneumaticSystem_SCRReagentDosing, consumer.AdBlueDosing == ConsumerTechnology.Pneumatically)
			);
		}

		private string GetXMLAirsuspensionControl(ConsumerTechnology airsuspensionControl)
		{
			switch (airsuspensionControl) {
				case ConsumerTechnology.Electrically:
					return "electronically";
				case ConsumerTechnology.Mechanically:
					return "mechanically";
				default:
					throw new VectoException("Unknown AirsuspensionControl!");
			}
		}


		private XElement GetHvac(IHVACBusAuxiliariesDeclarationData hvac)
		{
			return new XElement(new XElement(tns + XMLNames.BusAux_HVAC,
				new XElement(tns + XMLNames.Bus_AdjustableCoolantThermostat, hvac.AdjustableCoolantThermostat),
				new XElement(tns + XMLNames.Bus_EngineWasteGasHeatExchanger, hvac.EngineWasteGasHeatExchanger)));
		}


		private XElement GetAxlegearDescription(AxleGearData axleGearData)
		{
			return WrapComponent(
				XMLNames.Component_Axlegear, "AxlegearDataPIFType",
				GetCommonDescription(axleGearData),
				new XElement(tns + XMLNames.Component_AppVersion, axleGearData.InputData.AppVersion),
				new XElement(tns + XMLNames.Axlegear_LineType, axleGearData.LineType.ToXMLFormat()),
				new XElement(tns + XMLNames.Axlegear_Ratio, axleGearData.AxleGear.Ratio.ToXMLFormat(3)));
		}

		private XElement GetAngledriveDescription(AngledriveData angledriveData)
		{
			if (angledriveData == null) 
				return null;
			
			return WrapComponent(XMLNames.Component_Angledrive, "AngledriveDataPIFType",
				GetCommonDescription(angledriveData),
					new XElement(tns + XMLNames.Component_AppVersion, angledriveData.InputData.AppVersion),
					new XElement(tns + XMLNames.AngleDrive_Ratio, angledriveData.Angledrive.Ratio.ToXMLFormat(3))
				);
		}

		private XElement GetGearboxDescription(GearboxData gearboxData)
		{
			var retVal = WrapComponent(
				"Transmission", "TransmissionDataPIFType",
				GetCommonDescription(gearboxData),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, gearboxData.Type.ToXMLFormat()),
				new XElement(
					tns + XMLNames.Gearbox_Gears,
					new XAttribute(xsi + "type", "TransmissionGearsPIFType"),
					gearboxData.Gears.Select(
						x =>
							new XElement(
								tns + XMLNames.Gearbox_Gears_Gear,
								new XAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, x.Key),
								new XElement(tns + XMLNames.Gearbox_Gear_Ratio, x.Value.Ratio.ToXMLFormat(3)),
								x.Value.MaxTorque != null
									? new XElement(tns + XMLNames.Gearbox_Gears_MaxTorque, x.Value.MaxTorque.ToXMLFormat(0))
									: null,
								x.Value.MaxSpeed != null
									? new XElement(tns + XMLNames.Gearbox_Gear_MaxSpeed, x.Value.MaxSpeed.AsRPM.ToXMLFormat(0))
									: null)))
			);
			return retVal;

		}

		private XElement GetEngineDescription(CombustionEngineData engineData, List<List<FuelData.Entry>> fuelModes)
		{
			var fuels = new List<XElement>();
			foreach (var mode in engineData.InputData.EngineModes) {
				fuels.Add(
					new XElement(
						tns + XMLNames.Engine_FuelModes,
						new XElement(tns + XMLNames.Engine_IdlingSpeed, mode.IdleSpeed.AsRPM.ToXMLFormat(0)),
						new XElement(
							tns + XMLNames.Engine_FullLoadAndDragCurve,
							FullLoadCurveReader.Create(mode.FullLoadCurve, true).FullLoadEntries.Select(
								x => new XElement(
									tns + XMLNames.Engine_FullLoadCurve_Entry,
									new XAttribute(XMLNames.Engine_EngineFullLoadCurve_EngineSpeed_Attr, x.EngineSpeed.AsRPM.ToXMLFormat(2)),
									new XAttribute(XMLNames.Engine_FullLoadCurve_MaxTorque_Attr, x.TorqueFullLoad.ToXMLFormat(2)),
									new XAttribute(XMLNames.Engine_FullLoadCurve_DragTorque_Attr, x.TorqueDrag.ToXMLFormat(2)))
							)),
						new XElement(
							tns + "Fuels",
							mode.Fuels.Select(x => new XElement(tns + XMLNames.Engine_FuelType, x.FuelType.ToXMLFormat())))
					)
				);
			}

			var retVal = WrapComponent(
				XMLNames.Component_Engine, "EngineDataPIFType",
				GetCommonDescription(engineData),
				new XElement(tns + XMLNames.Engine_Displacement, engineData.Displacement.ConvertToCubicCentiMeter().ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_RatedSpeed, engineData.RatedSpeedDeclared.AsRPM.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_RatedPower, engineData.RatedPowerDeclared.ToXMLFormat(0)),
				new XElement(tns + XMLNames.Engine_MaxTorque, engineData.InputData.MaxTorqueDeclared.ToXMLFormat(0)),
				new XElement(
					tns + XMLNames.Engine_WHRType,
					new XElement(
						v23 + XMLNames.Engine_WHR_MechanicalOutputICE, (engineData.WHRType & WHRType.MechanicalOutputICE) != 0),
					new XElement(
						v23 + XMLNames.Engine_WHR_MechanicalOutputIDrivetrain,
						(engineData.WHRType & WHRType.MechanicalOutputDrivetrain) != 0),
					new XElement(v23 + XMLNames.Engine_WHR_ElectricalOutput, (engineData.WHRType & WHRType.ElectricalOutput) != 0)
				),
				fuels
			);
			return retVal;
		}

		private XElement WrapComponent(
			string elementName, string dataType, object[] commonElements, params object[] specificElements)
		{
			return new XElement(
				tns + elementName,
				new XElement(
					tns + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + "type", dataType),
					commonElements,
					specificElements));
		}


		private object[] GetCommonDescription(CombustionEngineData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Manufacturer, data.Manufacturer),
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(data.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Component_AppVersion, data.InputData.AppVersion)
			};
		}

		protected virtual object[] GetCommonDescription(GearboxData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Manufacturer, data.Manufacturer),
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Component_Gearbox_CertificationMethod, data.CertificationMethod.ToXMLFormat()),
				data.CertificationMethod == CertificationMethod.StandardValues
					? null
					: new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(data.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Component_AppVersion, data.InputData.AppVersion)
			};
		}

		protected virtual object[] GetCommonDescription(SimulationComponentData data)
		{
			return new object[] {
				new XElement(tns + XMLNames.Component_Manufacturer, data.Manufacturer),
				new XElement(tns + XMLNames.Component_Model, data.ModelName),
				new XElement(tns + XMLNames.Report_Component_CertificationMethod, data.CertificationMethod.ToXMLFormat()),
				data.CertificationMethod == CertificationMethod.StandardValues
					? null
					: new XElement(tns + XMLNames.Report_Component_CertificationNumber, data.CertificationNumber),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(data.Date, XmlDateTimeSerializationMode.Utc)),
			};
		}

		public virtual void WriteResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			_allSuccess &= resultEntry.Status == VectoRun.Status.Success;

			//if (resultEntry.Status == VectoRun.Status.Success) {
			//	_weightedPayload += resultEntry.Payload * resultEntry.WeightingFactor;
			//	_weightedCo2 += resultEntry.CO2Total / resultEntry.Distance * resultEntry.WeightingFactor;
			//}
			Results.Add(
				new XElement(
					tns + XMLNames.Report_Result_Result,
					new XAttribute(
						XMLNames.Report_Result_Status_Attr,
						resultEntry.Status == VectoRun.Status.Success ? "success" : "error"),
					new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, resultEntry.VehicleClass.GetClassNumber()),
					new XElement(tns + XMLNames.Report_Result_Mission, resultEntry.Mission.ToXMLFormat()),
					new XElement(
						tns + XMLNames.Report_ResultEntry_SimulationParameters,
						new XElement(
							tns + XMLNames.Report_ResultEntry_TotalVehicleMass,
							XMLHelper.ValueAsUnit(resultEntry.TotalVehicleMass, XMLNames.Unit_kg, 2)),
						new XElement(
							tns + XMLNames.Report_Result_Payload, XMLHelper.ValueAsUnit(resultEntry.Payload, XMLNames.Unit_kg, 2)),
						new XElement(
							tns + XMLNames.Report_ResultEntry_PassengerCount,
							resultEntry.PassengerCount?.ToXMLFormat(2) ?? "NaN"),
						new XElement(
							tns + XMLNames.Report_Result_FuelMode,
							resultEntry.FuelData.Count > 1
								? XMLNames.Report_Result_FuelMode_Val_Dual
								: XMLNames.Report_Result_FuelMode_Val_Single)
					),
					GetResults(resultEntry)));
		}

		private object[] GetResults(XMLDeclarationReport.ResultEntry resultEntry)
		{
			switch (resultEntry.Status) {
				case VectoRun.Status.Pending:
				case VectoRun.Status.Running: return null; // should not happen!
				case VectoRun.Status.Success: return GetSuccessResultEntry(resultEntry);
				case VectoRun.Status.Canceled:
				case VectoRun.Status.Aborted:
					return new object[] {
						new XElement(tns + "Error", resultEntry.Error),
						new XElement(tns + "ErrorDetails", resultEntry.StackTrace), 
					};
				default: throw new ArgumentOutOfRangeException();
			}
		}

		private object[] GetSuccessResultEntry(XMLDeclarationReport.ResultEntry result)
		{
			var retVal = new List<XElement>();

			foreach (var fuel in result.FuelData) {
				var fcResult = new XElement(
					tns + XMLNames.Report_Results_Fuel,
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()));
				fcResult.Add(
					new XElement(
						tns + XMLNames.Report_Result_EnergyConsumption,
						new XAttribute(XMLNames.Report_Results_Unit_Attr, "MJ/km"),
						(result.FuelConsumptionFinal[fuel.FuelType].EnergyDemand /
						result.Distance.ConvertToKiloMeter() / 1e6)
						.Value().ToMinSignificantDigits(5, 5)));
				retVal.Add(fcResult);
			}

			retVal.Add(
				new XElement(
					tns + XMLNames.Report_Results_CO2, new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					(result.CO2Total.ConvertToGramm() / result.Distance.ConvertToKiloMeter()).ToMinSignificantDigits(3, 2)));

			return retVal.Cast<object>().ToArray();
		}

		protected string GetGUID()
		{
			return Guid.NewGuid().ToString("n").Substring(0, 20);
		}
	}
}
