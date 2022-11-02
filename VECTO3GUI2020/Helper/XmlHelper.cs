using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Util.XML;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace VECTO3GUI2020.Helper
{
	public static class XmlHelper
	{
		public static XmlDocument ReadXmlDocument(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				return null;

			var xmlDocument = new XmlDocument();
				
			xmlDocument.Load(filePath);	

			return xmlDocument;
		}


		public static XmlNodeList GetComponentNodes(XmlDocument xmlDocument, string parentNode, string nodeName)
		{
			if (xmlDocument == null || string.IsNullOrEmpty(parentNode) || string.IsNullOrEmpty(nodeName))
				return null;

			return xmlDocument.SelectNodes($"//*[local-name()='{parentNode}']//*[local-name()='{nodeName}']");
		}

		public static bool ValidateXDocument(XDocument xDocument, Action<bool> resultAction = null,
			Action<XmlSeverityType, ValidationEvent> validationErrorAction = null)
		{
			var xmlDocument = xDocument.ToXmlDocument();
			if (xmlDocument == null)
				return false;

			var documentType = XMLHelper.GetDocumentType(xmlDocument.DocumentElement.LocalName);
			if (documentType == null)
			{
				throw new VectoException("unknown xml file! {0}", xmlDocument.DocumentElement.LocalName);
			}

			var validator = new XMLValidator(xmlDocument, resultAction, validationErrorAction);
			return validator.ValidateXML(documentType.Value); ;
		}

		public static string GetXmlAbsoluteFilePath(string baseUri)
		{
			if (baseUri == null)
				return null;

			return Uri.UnescapeDataString(new Uri(baseUri).AbsolutePath);
		}

		public static XDocument CreateWrapperDocument(this XElement xElement, XNamespace defaultNamespace,
			XmlDocumentType docType = XmlDocumentType.DeclarationJobData, string schemaVersion = "2.0")
		{
			var prefixMap = new Dictionary<string, XNamespace>();

			var xDocument = new XDocument();
			var rootElement = new XElement(XMLNamespaces.Tns_v20 + XMLNames.VectoInputDeclaration, new XAttribute(XNamespace.Xmlns + "tns",
				XMLNamespaces.Tns_v20));
			Debug.WriteLine(rootElement.ToString());

			rootElement.Add(new XAttribute("xmlns", defaultNamespace));
			rootElement.Add(new XAttribute("schemaVersion", schemaVersion));

			xDocument.Add(rootElement);



			Dictionary<string, XNamespace> nsAttributes = new Dictionary<string, XNamespace> {
				["xsi"] = XMLNamespaces.Xsi
			};

			foreach (var element in xElement.DescendantsAndSelf()) {
				var ns = element.Name.Namespace;
				if (ns != defaultNamespace) {
					var prefix = XMLNamespaces.GetPrefix(ns);
					if(prefix != null)
						nsAttributes[prefix] = ns;
				}
			}

			
			foreach (var nsAttribute in nsAttributes) {
				rootElement.Add(new XAttribute(XNamespace.Xmlns + nsAttribute.Key, nsAttribute.Value));
			}

			var LocalSchemaLocation = @"V:\VectoCore\VectoCore\Resources\XSD\";

			rootElement.Add(new XAttribute(XMLNamespaces.Xsi + "schemaLocation",
				$"{XMLNamespaces.DeclarationRootNamespace} {LocalSchemaLocation}VectoDeclarationJob.xsd"));


			rootElement.Add(xElement);

			return xDocument;
		}
	}
}
