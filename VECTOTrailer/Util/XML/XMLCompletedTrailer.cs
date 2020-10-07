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
using VECTOTrailer.Model;
using VECTOTrailer.ViewModel.Interfaces;


namespace VECTOTrailer.Util.XML
{
	public class XMLCompletedTrailer
	{
		private string _declarationDefinition;
		private string _schemaVersion;
		private XNamespace _xsi;
		private XNamespace _tns;
		private XNamespace _v27;
		private XNamespace _v22;
		private XNamespace _v20;
		private XNamespace _di;
		private XNamespace _rootNamespace;


		public XMLCompletedTrailer()
		{
			Init();
		}

		private void Init()
		{
			_declarationDefinition = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions";
			_schemaVersion = "2.7";
			_xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

			_tns = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v2.0";
			_v27 = _declarationDefinition + ":DEV:v2.7";
			_v22 = _declarationDefinition + ":v2.2";
			_v20 = _declarationDefinition + ":v2.0";
			_di = "http://www.w3.org/2000/09/xmldsig#";
			_rootNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationJob";
		}

		public XDocument GenerateCompletedTrailerDocument(Dictionary<Component, object> inputData)
		{
			var doc = new XDocument(
				new XDeclaration("1.0", "utf-8", "no"),
				new XElement(_tns + XMLNames.VectoInputDeclaration,
					new XAttribute("schemaVersion", _schemaVersion),
					new XAttribute(XNamespace.Xmlns + "xsi", _xsi.NamespaceName),
					new XAttribute("xmlns", _v27),
					new XAttribute(XNamespace.Xmlns +"tns", _tns),
					new XAttribute(XNamespace.Xmlns +"v2.7", _v27),
					new XAttribute(XNamespace.Xmlns +"v2.2", _v22), 
					new XAttribute(XNamespace.Xmlns +"v2.0", _v20),
					//new XAttribute(XNamespace.Xmlns +"di", _di),
					new XAttribute(_xsi + "schemaLocation", 
						@"urn:tugraz:ivt:VectoAPI:DeclarationJob  F:\Vecto\vecto-dev_bt\VectoCore\VectoCore\Resources\XSD/VectoDeclarationJob.xsd"
						//$"{_rootNamespace} {AbstractXMLWriter.SchemaLocationBaseUrl}VectoDeclarationJob.xsd"
						),
					GetVehicle(inputData)
				));

			return doc;
		}
		
		private XElement GetVehicle(Dictionary<Component, object> inputData)
		{
			var vehicleData = (ICompleteTrailer)inputData[Component.CompleteTrailer];


			var id = "CB-" + Guid.NewGuid().ToString("n").Substring(0, 20);
			return new XElement(_v20 + XMLNames.Component_Vehicle,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					new XAttribute(_xsi + "type", XMLDeclarationVehicleDataProviderV27.XSD_TYPE),// "VehicleTrailerType"

					new XElement(_v27 + XMLNames.Component_Manufacturer, vehicleData?.Manufacturer),
					new XElement(_v27 + XMLNames.Component_ManufacturerAddress, vehicleData?.ManufacturerAddress),
					new XElement(_v27 + XMLNames.Trailer_CommercialName, vehicleData?.Model),
					new XElement(_v27 + XMLNames.Vehicle_VIN, vehicleData?.VIN),
					new XElement(_v27 + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
					new XElement(_v27 + XMLNames.Trailer_LegislativeCategory, vehicleData?.LegislativeCategory),
					new XElement(_v27 + XMLNames.Trailer_AxleCount, vehicleData?.NumberOfAxles.GetLabel()),
					new XElement(_v27 + XMLNames.Trailer_TrailerType, vehicleData?.TrailerType),
					new XElement(_v27 + XMLNames.Trailer_BodyCode, vehicleData?.BodyCode.GetLabel()),
					new XElement(_v27 + "Masses", 
						new XElement(_v27 + XMLNames.Trailer_CurbMass, vehicleData?.CurbMassTrailer), 
						new XElement(_v27 + XMLNames.Trailer_TPMLMTotalTrailer, vehicleData?.TPLMTotalTrailer)),
					new XElement(_v27 + "Dimensions", 
						new XElement(_v27 + XMLNames.Trailer_ExternalLenghtBody, vehicleData?.ExternalLengthOfTheBody),
						new XElement(_v27 + XMLNames.Trailer_ExternalWidthBody, vehicleData?.ExternalWidthOfTheBody),
						new XElement(_v27 + XMLNames.Trailer_ExternalHeightBody, vehicleData?.ExternalHeightOfTheBody),
						new XElement(_v27 + XMLNames.Trailer_TotalHeightTrailer, vehicleData?.TotalHeightOfTheTrailer),
						new XElement(_v27 + XMLNames.Trailer_PositionXFirstAxle, vehicleData?.PositionXFirstAxle),
						new XElement(_v27 + XMLNames.Trailer_PositionXLastAxle, vehicleData?.PositionXLastAxle),
						new XElement(_v27 + XMLNames.Trailer_InternalHeightBody, vehicleData?.InternalHeightOfTheBody),
						new XElement(_v27 + XMLNames.Trailer_CargoVolume, vehicleData?.CargoVolume)
						)
			);
		}
	}
}
