using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Util.XML;
using VECTO3GUI2020.Util.XML.Interfaces;


namespace VECTO3GUI2020.Helper
{
	public static class XMLExtension
	{
		
		public static XElement CreateSignatureElement(this IXMLComponentWriter componentWriter, XNamespace nameSpace, string uri, DigestData digestData, bool hash = false)
		{
			var di = XMLNamespaces.Di;
			var signatureElement = new XElement(nameSpace + XMLNames.DI_Signature);
			signatureElement.Add(new XElement(di + XMLNames.DI_Signature_Reference, new XAttribute(XMLNames.DI_Signature_Reference_URI_Attr, digestData.Reference)));

			var refElement = signatureElement.FirstNode as XElement;
			refElement?.Add(new XElement(di + XMLNames.DI_Signature_Reference_Transforms));

			var transformsElement = refElement?.FirstNode as XElement;
			foreach (var digestDataCanonicalizationMethod in digestData.CanonicalizationMethods) {
				transformsElement?.Add(new XElement(di + XMLNames.DI_Signature_Reference_Transforms_Transform,
					new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, digestDataCanonicalizationMethod)));
			}
			
			refElement?.Add(new XElement(di + XMLNames.DI_Signature_Reference_DigestMethod, 
				new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, digestData.DigestMethod)));
			if (hash) {
				throw new NotImplementedException("Hashing not implemented");
			}
			refElement?.Add(new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, digestData.DigestValue));


			return signatureElement;
		}


		public static XmlDocument ToXmlDocument(this XDocument xDocument)
		{
			var xmlDocument = new XmlDocument();
			using (var reader = xDocument.CreateReader())
			{
				xmlDocument.Load(reader);
			}

			var xDeclaration = xDocument.Declaration;
			if (xDeclaration != null)
			{
				var xmlDeclaration = xmlDocument.CreateXmlDeclaration(
					xDeclaration.Version,
					xDeclaration.Encoding,
					xDeclaration.Standalone);

				xmlDocument.InsertBefore(xmlDeclaration, xmlDocument.FirstChild);
			}

			return xmlDocument;
		}

		public static XmlNode ToXmlNode(this XElement element)
		{
			using (XmlReader xmlReader = element.CreateReader())
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(xmlReader);
				return xmlDocument;
			}
		}

		public static string GetVersion(this XElement element)
		{
			return element.ToXmlNode().GetVersion();
		}

		public static string GetVersion(this XmlNode node)
		{
			if (node == null)
			{
				return null;
			}
			var version = XMLHelper.GetXsdType(node.SchemaInfo.SchemaType);
			if (string.IsNullOrWhiteSpace(version))
			{
				version = XMLHelper.GetVersionFromNamespaceUri((node.SchemaInfo.SchemaType?.Parent as XmlSchemaElement)?.QualifiedName.Namespace);
			}

			return version;
		}


		public static string ToXmlFormat(this DateTime dateTime)
		{
			return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Utc);
		}

}
}