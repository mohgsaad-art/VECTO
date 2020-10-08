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
					GetTrailer(inputData)
				));

			return doc;
		}
		
		private XElement GetTrailer(Dictionary<Component, object> inputData)
		{
			var trailerData = (ICompleteTrailer)inputData[Component.CompleteTrailer];


			var id = "CB-" + Guid.NewGuid().ToString("n").Substring(0, 20);
			var vehicle = new XElement(_v20 + XMLNames.Component_Vehicle,
					new XAttribute(XMLNames.Component_ID_Attr, id),
					new XAttribute(_xsi + "type", XMLDeclarationVehicleDataProviderV27.XSD_TYPE),

					new XElement(_v27 + XMLNames.Component_Manufacturer, trailerData?.Manufacturer),
					new XElement(_v27 + XMLNames.Component_ManufacturerAddress, trailerData?.ManufacturerAddress),
					new XElement(_v27 + XMLNames.Trailer_CommercialName, trailerData?.Model),
					new XElement(_v27 + XMLNames.Vehicle_VIN, trailerData?.VIN),
					new XElement(_v27 + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
					new XElement(_v27 + XMLNames.Trailer_LegislativeCategory, trailerData?.LegislativeCategory),
					new XElement(_v27 + XMLNames.Trailer_AxleCount, trailerData?.NumberOfAxles.GetLabel()),
					new XElement(_v27 + XMLNames.Trailer_TrailerType, trailerData?.TrailerType),
					new XElement(_v27 + XMLNames.Trailer_BodyCode, trailerData?.BodyCode.GetLabel()),
					new XElement(_v27 + "Masses", 
						new XElement(_v27 + XMLNames.Trailer_CurbMass, trailerData?.CurbMassTrailer?.ToXMLFormat()), 
						new XElement(_v27 + XMLNames.Trailer_TPMLMTotalTrailer, trailerData?.TPLMTotalTrailer?.ToXMLFormat())),
					new XElement(_v27 + "Dimensions", 
						new XElement(_v27 + XMLNames.Trailer_ExternalLenghtBody, trailerData?.ExternalLengthOfTheBody?.ToXMLFormat()),
						new XElement(_v27 + XMLNames.Trailer_ExternalWidthBody, trailerData?.ExternalWidthOfTheBody?.ToXMLFormat()),
						new XElement(_v27 + XMLNames.Trailer_ExternalHeightBody, trailerData?.ExternalHeightOfTheBody?.ToXMLFormat()),
						new XElement(_v27 + XMLNames.Trailer_TotalHeightTrailer, trailerData?.TotalHeightOfTheTrailer?.ToXMLFormat()),
						new XElement(_v27 + XMLNames.Trailer_PositionXFirstAxle, trailerData?.PositionXFirstAxle?.ToXMLFormat()),
						new XElement(_v27 + XMLNames.Trailer_PositionXLastAxle, trailerData?.PositionXLastAxle?.ToXMLFormat()),
						new XElement(_v27 + XMLNames.Trailer_InternalHeightBody, trailerData?.InternalHeightOfTheBody?.ToXMLFormat()),
						new XElement(_v27 + XMLNames.Trailer_CargoVolume, trailerData?.CargoVolume?.ToXMLFormat())
						)
			);

			if (trailerData != null && (trailerData.SideSkirtsShort || trailerData.SideSkirtsLong ||
										trailerData.BoatTailShort || trailerData.BoatTailLong)) {
				vehicle.Add(new XElement(_v27 + "AeroFeatures"));

				if (trailerData.SideSkirtsShort)
					vehicle.Element(_v27 + "AeroFeatures")?.Add(new XElement(_v27 + XMLNames.Trailer_AeroFeatureTechnologies, "Side skirt short"));
				if (trailerData.SideSkirtsLong)
					vehicle.Element(_v27 + "AeroFeatures")?.Add(new XElement(_v27 + XMLNames.Trailer_AeroFeatureTechnologies, "Side skirt long"));
				if (trailerData.BoatTailShort)
					vehicle.Element(_v27 + "AeroFeatures")?.Add(new XElement(_v27 + XMLNames.Trailer_AeroFeatureTechnologies, "Boat tail short"));
				if (trailerData.BoatTailLong)
					vehicle.Element(_v27 + "AeroFeatures")?.Add(new XElement(_v27 + XMLNames.Trailer_AeroFeatureTechnologies, "Boat tail long"));
			}

			if (trailerData != null && (int.Parse(trailerData.NumberOfAxles.GetLabel()) > 0)) {
				vehicle.Add(new XElement(_v27 + "AxleWheels", new XElement(_v27 + "Data", new XAttribute(_xsi + "type", "TrailerAxleWheelsDataDeclarationType"), new XElement(_v27 + "Axles"))));

				if (int.Parse(trailerData.NumberOfAxles.GetLabel()) > 0)
					vehicle.Element(_v27 + "Axles")?.Add(new XElement(_v27 + "Axle", new XAttribute(_xsi + "type", "TrailerAxleType"), new XAttribute("axleNumber", "1"), new XElement(_v27 + "TwinTyres", trailerData.Axle1IsTwin.ToString())));
				if (int.Parse(trailerData.NumberOfAxles.GetLabel()) > 1)
					vehicle.Element(_v27 + "Axles")?.Add(new XElement(_v27 + "Axle", new XAttribute(_xsi + "type", "TrailerAxleType"), new XAttribute("axleNumber", "2"), new XElement(_v27 + "TwinTyres", trailerData.Axle2IsTwin.ToString())));
				if (int.Parse(trailerData.NumberOfAxles.GetLabel()) > 2)
					vehicle.Element(_v27 + "Axles")?.Add(new XElement(_v27 + "Axle", new XAttribute(_xsi + "type", "TrailerAxleType"), new XAttribute("axleNumber", "3"), new XElement(_v27 + "TwinTyres", trailerData.Axle3IsTwin.ToString())));
			}

			return vehicle;
		}
	}
}
