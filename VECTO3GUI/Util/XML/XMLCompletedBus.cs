using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using VECTO3GUI.Model;
using VECTO3GUI.ViewModel.Interfaces;


namespace VECTO3GUI.Util.XML
{
	public class XMLCompletedBus
	{
		private string _declarationDefinition;
		private string _schemaVersion;
		private XNamespace _xsi;
		private XNamespace _tns;
		private XNamespace _v26;
		private XNamespace _v21;
		private XNamespace _v20;
		private XNamespace _di;
		private XNamespace _rootNamespace;


		public XMLCompletedBus()
		{
			Init();
		}

		private void Init()
		{
			_declarationDefinition = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions";
			_schemaVersion = "2.0";
			_xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

			_tns = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v2.0";
			_v26 = _declarationDefinition + ":DEV:v2.6";
			_v21 = _declarationDefinition + ":v2.1";
			_v20 = _declarationDefinition + ":v2.0";
			_di = "http://www.w3.org/2000/09/xmldsig#";
			_rootNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationJob";
		}

		public XDocument GenerateCompletedBusDocument(Dictionary<Component, object> inputData)
		{
			var doc = new XDocument();

			doc.Add(
				new XElement(_tns + XMLNames.VectoInputDeclaration,
					new XAttribute("schemaVersion", _schemaVersion),
					new XAttribute(XNamespace.Xmlns + "xsi", _xsi.NamespaceName),
					new XAttribute("xmlns", _v21),
					new XAttribute(XNamespace.Xmlns +"tns", _tns),
					new XAttribute(XNamespace.Xmlns +"v2.6", _v26),
					new XAttribute(XNamespace.Xmlns +"v2.1", _v21), 
					new XAttribute(XNamespace.Xmlns +"v2.0", _v20),
					new XAttribute(XNamespace.Xmlns +"di", _di),
					new XAttribute(_xsi + "schemaLocation", 
						$"{_rootNamespace} {AbstractXMLWriter.SchemaLocationBaseUrl}VectoDeclarationJob.xsd"),
					GetVehicle(inputData)
				));
			return doc;
		}
		
		private XElement GetVehicle(Dictionary<Component, object> inputData)
		{
			var vehicleData = (ICompleteVehicleBus)inputData[Component.CompleteBusVehicle];


			var id = "CB-" + Guid.NewGuid().ToString("n").Substring(0, 20);
			return new XElement(_v20 + XMLNames.Component_Vehicle,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					new XAttribute(_xsi + "type", XMLDeclarationCompletedBusDataProviderV26.XSD_TYPE),// "CompletedVehicleDeclarationType"
					new XAttribute("xmlns", _v26),

					new XElement(_v26 + XMLNames.Component_Manufacturer, vehicleData?.Manufacturer),
					new XElement(_v26 + XMLNames.Component_ManufacturerAddress, vehicleData?.ManufacturerAddress),
					new XElement(_v26 + XMLNames.Component_Model, vehicleData?.Model),
					new XElement(_v26 + XMLNames.Vehicle_VIN, vehicleData?.VIN),
					new XElement(_v26 + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
					new XElement(_v26 + XMLNames.Vehicle_LegislativeClass, vehicleData?.LegislativeClass.GetLabel()),
					new XElement(_v26 + XMLNames.Vehicle_RegisteredClass, vehicleData?.RegisteredClass.GetLabel()),
					new XElement(_v26 + XMLNames.Vehicle_VehicleCode, vehicleData?.VehicleCode.GetLabel()),
					new XElement(_v26 + XMLNames.Vehicle_CurbMassChassis, vehicleData?.CurbMassChassis?.ToXMLFormat(0)),
					new XElement(_v26 + XMLNames.TPMLM, vehicleData?.TechnicalPermissibleMaximumLadenMass?.ToXMLFormat(0)),
					vehicleData?.NgTankSystem == null ? null : new XElement(_v26 + XMLNames.Vehicle_NgTankSystem, vehicleData?.NgTankSystem),
					new XElement(_v26 + "RegisteredPassengers",
						new XElement(_v26 + XMLNames.Bus_LowerDeck, vehicleData?.NumberOfPassengersLowerDeck),
						new XElement(_v26 + XMLNames.Bus_UpperDeck, vehicleData?.NumberOfPassengersUpperDeck)
					),
					new XElement(_v26 + XMLNames.Bus_LowEntry, vehicleData?.LowEntry),
					new XElement(_v26 + XMLNames.Bus_HeighIntegratedBody, vehicleData?.HeightIntegratedBody?.ToXMLFormat(3)),
					new XElement(_v26 + XMLNames.Bus_VehicleLength, vehicleData?.VehicleLength?.ToXMLFormat(3)),
					new XElement(_v26 + XMLNames.Bus_VehicleWidth, vehicleData?.VehicleWidth?.ToXMLFormat(3)),
					new XElement(_v26 + XMLNames.Bus_EntranceHeight, vehicleData?.EntranceHeight?.ToXMLFormat(3)),
					new XElement(_v26 + XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology, vehicleData?.DoorDriveTechnology.GetLabel()?.ToLower()),

			new XElement(_v26 + XMLNames.Vehicle_Components,
					new XAttribute(_xsi + "type", XMLDeclarationCompleteBusComponentsDataProviderV26.XSD_TYPE),//"CompletedVehicleComponentsDeclarationType"
					GetComponentXElements(inputData)
			)
			);
		}

		private XElement[] GetComponentXElements(Dictionary<Component, object> inputData)
		{
			var result = inputData.ContainsKey(Component.Airdrag) ? new XElement[2] : new XElement[1];

			if (result.Length == 2) {
				result[0] = GetAirdrag((IAirdrag)inputData[Component.Airdrag]);
				result[1] = GetAuxiliary((IAuxiliariesBus)inputData[Component.Auxiliaries]);
			} else {
				result[0] = GetAuxiliary((IAuxiliariesBus)inputData[Component.Auxiliaries]); 
			}
		
			return result;
		}
		
		private XElement GetAirdrag(IAirdrag airdrag)
		{
			if (airdrag.NoAirdragData)
				return null;

			return new XElement(_v26 + XMLNames.Component_AirDrag, 
				new XAttribute("xmlns", _v20),
				
				new XElement(_v20 + XMLNames.ComponentDataWrapper, 
					new XAttribute(XMLNames.Component_ID_Attr, airdrag?.DigestValue.Reference.Replace("#","")),
					new XAttribute(_xsi + "type", XMLDeclarationAirdragDataProviderV10.XSD_TYPE), // "AirDragDataDeclarationType"
					
					new XElement(_v20 + XMLNames.Component_Manufacturer, airdrag?.Manufacturer),
					new XElement(_v20 + XMLNames.Component_Model, airdrag?.Model),
					new XElement(_v20 + XMLNames.Component_CertificationNumber, airdrag?.CertificationNumber),
					new XElement(_v20 + XMLNames.Component_Date, airdrag?.Date),
					new XElement(_v20 + XMLNames.Component_AppVersion, airdrag?.AppVersion),
					new XElement(_v20 + "CdxA_0", airdrag?.CdxA_0.ToXMLFormat()),
					new XElement(_v20 + "TransferredCdxA", airdrag?.TransferredCdxA.ToXMLFormat()),
					new XElement(_v20 + XMLNames.AirDrag_DeclaredCdxA, airdrag?.DeclaredCdxA.ToXMLFormat())),
				
				new XElement(_v20 + XMLNames.DI_Signature,
					new XElement(_di +XMLNames.DI_Signature_Reference,
						new XAttribute(XMLNames.DI_Signature_Reference_URI_Attr, airdrag?.DigestValue.Reference),
						new XElement(_di + XMLNames.DI_Signature_Reference_Transforms,
							new XElement(_di + XMLNames.DI_Signature_Reference_Transforms_Transform, 
								new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, airdrag?.DigestValue.CanonicalizationMethods[0])),
							new XElement(_di + XMLNames.DI_Signature_Reference_Transforms_Transform,
								new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, airdrag?.DigestValue.CanonicalizationMethods[1]))
						),
						new XElement(_di + XMLNames.DI_Signature_Reference_DigestMethod,
							new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, airdrag?.DigestValue.DigestMethod)),
						new XElement(_di + XMLNames.DI_Signature_Reference_DigestValue, airdrag?.DigestValue.DigestValue))
						)
				);
		}

		private XElement GetAuxiliary(IAuxiliariesBus auxBus)
		{
			return new XElement(_v26 + XMLNames.Component_Auxiliaries,
					new XElement(_v26 + XMLNames.ComponentDataWrapper,
						new XAttribute(_xsi +"type", XMLDeclarationCompleteBusAuxiliariesDataProviderV26.XSD_TYPE),//"CompletedVehicleAuxiliaryDataDeclarationType"

						new XElement(_v26 + XMLNames.BusAux_ElectricSystem,
							GetAlternatorTechnology(auxBus?.AlternatorTechnologies),
							new XElement(_v26 + "LEDLights",
								new XElement(_v26 + XMLNames.Bus_Dayrunninglights, auxBus?.DayrunninglightsLED),
								new XElement(_v26 + XMLNames.Bus_Headlights, auxBus?.HeadlightsLED),
								new XElement(_v26 + XMLNames.Bus_Positionlights, auxBus?.PositionlightsLED),
								new XElement(_v26 + XMLNames.Bus_Brakelights, auxBus?.BrakelightsLED),
								new XElement(_v26 + XMLNames.Bus_Interiorlights, auxBus?.InteriorLightsLED))),
							new XElement(_v26 + "HVAC",
								new XElement(_v26 + XMLNames.Bus_SystemConfiguration, auxBus?.SystemConfiguration.GetXmlFormat()),
								new XElement(_v26 + XMLNames.Bus_CompressorType,
									new XElement(_v26 + XMLNames.Bus_DriverAC, auxBus?.CompressorTypeDriver.GetLabel()),
									new XElement(_v26 + XMLNames.Bus_PassengerAC, auxBus?.CompressorTypePassenger.GetLabel())),
								new XElement(_v26 + XMLNames.Bus_AuxiliaryHeaterPower, Convert.ToInt32(auxBus?.AuxHeaterPower?.Value())), 
								new XElement(_v26 + XMLNames.Bus_DoubleGlazing, auxBus?.DoubleGlasing),
								new XElement(_v26 + XMLNames.Bus_HeatPump, auxBus?.HeatPump),
								new XElement(_v26 + XMLNames.Bus_AdjustableAuxiliaryHeater, auxBus?.AdjustableAuxiliaryHeater), 
								new XElement(_v26 + XMLNames.Bus_SeparateAirDistributionDucts, auxBus?.SeparateAirDistributionDucts)
								)
							)
			);
		}
		
		private XElement[] GetAlternatorTechnology(IList<AlternatorTechnologyModel> alternatorTechnologies)
		{
			if (alternatorTechnologies.IsNullOrEmpty())
				return null;

			var result = new List<XElement>();

			for (int i = 0; i < alternatorTechnologies.Count; i++) {

				if(string.IsNullOrWhiteSpace(alternatorTechnologies[i].AlternatorTechnology))
					continue;
				
				result.Add(new XElement(_v26 + XMLNames.BusAux_ElectricSystem_AlternatorTechnology, 
					alternatorTechnologies[i].AlternatorTechnology));	
			}

			return result.Count > 0 ? result.ToArray() : null;
		}
	}
}
