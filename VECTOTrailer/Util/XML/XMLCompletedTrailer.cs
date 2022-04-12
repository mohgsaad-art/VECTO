using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;
using VECTOTrailer.ViewModel.Interfaces;


namespace VECTOTrailer.Util.XML
{

	public class XMLCompletedTrailer
	{
		private string _declarationDefinition;
		private string _declarationComponent;
		private string _schemaVersion;
		private XNamespace _xsi;
		private XNamespace _tns;
		private XNamespace _v27;
		private XNamespace _v23;
		private XNamespace _v22;
		private XNamespace _v21;
		private XNamespace _v20;
		private XNamespace _di;
		private XNamespace _tyre;
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
			_v23 = _declarationDefinition + ":DEV:v2.3";
			_v22 = _declarationDefinition + ":v2.2";
			_v21 = _declarationDefinition + ":v2.1";
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
					new XAttribute(XNamespace.Xmlns + "tns", _tns),
					new XAttribute(XNamespace.Xmlns + "v2.7", _v27),
					new XAttribute(XNamespace.Xmlns + "v2.2", _v22),
					new XAttribute(XNamespace.Xmlns + "v2.0", _v20),
					new XAttribute(XNamespace.Xmlns + "di", _di),
					new XAttribute(XNamespace.Xmlns + "v2.3", _v23),
					new XAttribute("xmlns", _v27),
					new XAttribute(_xsi + "schemaLocation",
						@"urn:tugraz:ivt:VectoAPI:DeclarationJob F:\Vecto\vecto-dev_bt\VectoCore\VectoCore\Resources\XSD/VectoDeclarationJob.xsd"
						//$"{_rootNamespace} {AbstractXMLWriter.SchemaLocationBaseUrl}VectoDeclarationJob.xsd"
					),
					GetTrailer(inputData)
				));

			return doc;
		}

		private XElement GetTrailer(Dictionary<Component, object> inputData)
		{
			var trailerData = (ICompleteTrailer)inputData[Component.CompleteTrailer];
			if (trailerData == null) return null;

			var id = "CB-" + Guid.NewGuid().ToString("n").Substring(0, 20);
			var vehicle = new XElement(_v20 + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, id),
				new XAttribute(_xsi + "type", XMLDeclarationVehicleDataProviderV27.XSD_TYPE),

				new XElement(_v27 + XMLNames.Component_Manufacturer, trailerData.Manufacturer),
				new XElement(_v27 + XMLNames.Component_ManufacturerAddress, trailerData.ManufacturerAddress),
				new XElement(_v27 + XMLNames.Trailer_Model, trailerData.TrailerModel),
				new XElement(_v27 + XMLNames.Vehicle_VIN, trailerData.VIN),
				new XElement(_v27 + XMLNames.Component_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
				new XElement(_v27 + XMLNames.Trailer_LegislativeCategory, trailerData.LegislativeCategory),
				new XElement(_v27 + XMLNames.Trailer_AxleCount, trailerData.NumberOfAxles.GetLabel()),
				new XElement(_v27 + XMLNames.Trailer_TrailerType, trailerData.TrailerType),
				new XElement(_v27 + XMLNames.Trailer_BodyCode, trailerData.BodyCode.GetLabel()));

			vehicle.Add(trailerData.TrailerType == TypeTrailer.DC
				? new XElement(_v27 + XMLNames.Trailer_TrailerCouplingPoint,
					trailerData.TrailerCouplingPoint.GetLabel())
				: null);

				vehicle.Add(new XElement(_v27 + "Masses",
					new XElement(_v27 + XMLNames.Trailer_MassInRunningOrder, trailerData.MassInRunningOrder.ToXMLFormat(0)),
					new XElement(_v27 + XMLNames.Trailer_TPMLMTotalTrailer, trailerData.TPLMTotalTrailer.ToXMLFormat(0)),
					trailerData.TPLMAxleAssembly != null 
						? new XElement(_v27 + XMLNames.Trailer_TPLMAxleAssembly, trailerData.TPLMAxleAssembly.ToXMLFormat(0)) 
						: null),
					new XElement(_v27 + "Dimensions",
							new XElement(_v27 + XMLNames.Trailer_ExternalLengthBody.ToString(CultureInfo.InvariantCulture),
								trailerData.ExternalLengthOfTheBody?.ToXMLFormat(3).ToString(CultureInfo.InvariantCulture)),
							new XElement(_v27 + XMLNames.Trailer_ExternalWidthBody, trailerData.ExternalWidthOfTheBody?.ToXMLFormat(3)),
							new XElement(_v27 + XMLNames.Trailer_ExternalHeightBody.ToString(CultureInfo.InvariantCulture),
								trailerData.ExternalHeightOfTheBody?.ToXMLFormat(3).ToString(CultureInfo.InvariantCulture)),
							new XElement(_v27 + XMLNames.Trailer_TotalHeightTrailer.ToString(CultureInfo.InvariantCulture),
								trailerData.TotalHeightOfTheTrailer?.ToXMLFormat(3).ToString(CultureInfo.InvariantCulture)),
							new XElement(_v27 + XMLNames.Trailer_LengthFromFrontToFirstAxle, trailerData.LengthFromFrontToFirstAxle?.ToXMLFormat(3).ToString(CultureInfo.InvariantCulture)),
							new XElement(_v27 + XMLNames.Trailer_LengthBetweenCentersOfAxles, trailerData.LengthBetweenCentersOfAxles?.ToXMLFormat(3).ToString(CultureInfo.InvariantCulture)),
							new XElement(_v27 + XMLNames.Trailer_CargoVolume, trailerData.CargoVolume?.ToXMLFormat(3).ToString(CultureInfo.InvariantCulture)),
							new XElement(_v27 + XMLNames.Trailer_VolumeOrientation, trailerData.VolumeOrientation)
				)
			);
			
	
			var aeroFeatures = new XElement(_v27 + XMLNames.Trailer_AeroFeatures);
			vehicle.Add(aeroFeatures);
			
			if (trailerData.IsStandard) {
				if (trailerData.SideSkirtsShort) {
					aeroFeatures.Add(new XElement(_v27 + XMLNames.Trailer_AeroFeatureTechnologies,
						AeroFeatureTechnology.SideSkirtsShort.ToXMLFormat()));
				}

				if (trailerData.SideSkirtsLong) {
					aeroFeatures.Add(new XElement(_v27 + XMLNames.Trailer_AeroFeatureTechnologies,
						AeroFeatureTechnology.SideSkirtsLong.ToXMLFormat()));
				}

				if (trailerData.BoatTailShort) {
					aeroFeatures.Add(new XElement(_v27 + XMLNames.Trailer_AeroFeatureTechnologies,
						AeroFeatureTechnology.BoatTailShort.ToXMLFormat()));
				}

				if (trailerData.BoatTailLong) {
					aeroFeatures.Add(new XElement(_v27 + XMLNames.Trailer_AeroFeatureTechnologies, 
						AeroFeatureTechnology.BoatTailLong.ToXMLFormat()));
				}
			} else if (trailerData.IsCertified) {
				vehicle.Element(_v27 + XMLNames.Trailer_AeroFeatures)?.Add(new XElement(_v27 + XMLNames.Trailer_CertifiedAeroDevice));

				if (trailerData.CertifiedAeroDeviceXml != null) {
					var certifiedAeroDevice = XDocument.Parse(trailerData.CertifiedAeroDeviceXml.ToString(CultureInfo.InvariantCulture));

					var data = certifiedAeroDevice.Descendants().Where(x => x.Name.ToString().Contains("Data"));

					if (data != null)
						vehicle.Element(_v27 + XMLNames.Trailer_AeroFeatures)?.Element(_v27 + XMLNames.Trailer_CertifiedAeroDevice)?
							.Add(data);

					var signature = certifiedAeroDevice.Descendants().Where(x => x.Name.ToString().Contains("Signature"));

					aeroFeatures.Element(_v27 + XMLNames.Trailer_CertifiedAeroDevice)?
						.Add(signature);
				}
			} else if (trailerData.IsNone) {
				aeroFeatures.Add(new XElement(_v27 + XMLNames.Trailer_AeroFeatureTechnologies,
					AeroFeatureTechnology.None.ToXMLFormat()));
			} else{
				throw new VectoException("We should not be here aero device has to be either none/standard/certified");
			}


			if (int.Parse(trailerData.NumberOfAxles.GetLabel()) <= 0)
				return vehicle;

			vehicle.Add(new XElement(_v27 + "AxleWheels",
				new XElement(_v27 + "Data", new XAttribute(_xsi + "type", "v2.7:TrailerAxleWheelsDataDeclarationType"),
					new XElement(_v27 + "Axles"))));

			try {
				var tyre = CreateTyre(trailerData.Axle1Xml);
				vehicle.Element(_v27 + "AxleWheels")?.Element(_v27 + "Data")?.Element(_v27 + "Axles")?.Add(
					new XElement(_v27 + "Axle", new XAttribute(_xsi + "type", "v2.7:TrailerAxleType"),
						new XAttribute("axleNumber", "1"),
						new XElement(_v27 + "TwinTyres", trailerData.Axle1IsTwin.ToString().ToLower()),
						new XElement(_v27 + "Liftable", trailerData.Axle1IsLiftable.ToString().ToLower()),
						new XElement(_v27 + "Steered", trailerData.Axle1IsSteered.ToString().ToLower()),
						tyre));

				if (int.Parse(trailerData.NumberOfAxles.GetLabel()) <= 1)
					return vehicle;

				tyre = CreateTyre(trailerData.Axle2Xml);

				vehicle.Element(_v27 + "AxleWheels")?.Element(_v27 + "Data")?.Element(_v27 + "Axles")?.Add(
					new XElement(_v27 + "Axle", new XAttribute(_xsi + "type", "v2.7:TrailerAxleType"),
						new XAttribute("axleNumber", "2"),
						new XElement(_v27 + "TwinTyres", trailerData.Axle2IsTwin.ToString().ToLower()),
						new XElement(_v27 + "Liftable", trailerData.Axle2IsLiftable.ToString().ToLower()),
						new XElement(_v27 + "Steered", trailerData.Axle2IsSteered.ToString().ToLower()),
						tyre));
				if (int.Parse(trailerData.NumberOfAxles.GetLabel()) <= 2)
					return vehicle;

				tyre = CreateTyre(trailerData.Axle3Xml);

				vehicle.Element(_v27 + "AxleWheels")?.Element(_v27 + "Data")?.Element(_v27 + "Axles")?.Add(
					new XElement(_v27 + "Axle", new XAttribute(_xsi + "type", "v2.7:TrailerAxleType"),
						new XAttribute("axleNumber", "3"),
						new XElement(_v27 + "TwinTyres", trailerData.Axle3IsTwin.ToString().ToLower()),
						new XElement(_v27 + "Liftable", trailerData.Axle3IsLiftable.ToString().ToLower()),
						new XElement(_v27 + "Steered", trailerData.Axle3IsSteered.ToString().ToLower()),
						tyre));

			} catch (Exception ex) {
				throw new VectoException("Error writing XML", ex);
			}

			return vehicle;
		}

		private XElement CreateTyre(string axleXml)
		{
			

			var tyre = new XElement(_v27 + XMLNames.AxleWheels_Axles_Axle_Tyre);
			try {
				var sourceXml = XDocument.Parse(axleXml.ToString(CultureInfo.InvariantCulture));
				
				
				var nsDeclarations = sourceXml.Root.Attributes().Where(attr => attr.IsNamespaceDeclaration);
				//tyre.Add(nsDeclarations);
				
				var tyreNode = sourceXml.Root.Elements().Single(e => e.Name.LocalName == XMLNames.AxleWheels_Axles_Axle_Tyre);
				var tyreData = tyreNode.Elements().Single(e => e.Name.LocalName == XMLNames.ComponentDataWrapper);
				tyreData.Name = _v27 + XMLNames.ComponentDataWrapper; //Data is in namespace v27
				

				var tyreSignature = tyreNode.Elements().Single(e => e.Name.LocalName == XMLNames.DI_Signature);
				tyreSignature.Name = _v27 + XMLNames.DI_Signature; //Signature element is in namespace v27


				tyre.Add(tyreData, tyreSignature);

			} catch (Exception ex) {
				throw new VectoException("Invalid tyre xml", ex);
			}
			return tyre;
		}
	}
}
