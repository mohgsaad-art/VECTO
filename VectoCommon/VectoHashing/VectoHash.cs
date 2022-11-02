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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoHashing.Impl;
using TUGraz.VectoHashing.Util;

namespace TUGraz.VectoHashing
{
	public class VectoHash : IVectoHash
	{
		protected XmlDocument Document;

		public static VectoHash Load(string filename)
		{
			var doc = new XmlDocument();
			XmlTextReader reader = null;
			try {
				reader = new XmlTextReader(filename);
				doc.Load(reader);
			} catch (Exception e) {
				throw new Exception("failed to read XML document", e);
			} finally {
				if (reader != null) {
					reader.Close();
				}
			}
			return new VectoHash(doc);
		}

		public static VectoHash Load(Stream stream)
		{
			var doc = new XmlDocument();
			try {
				doc.Load(new XmlTextReader(stream));
			} catch (Exception e) {
				throw new Exception("failed to read XML document", e);
			}
			return new VectoHash(doc);
		}

		public static VectoHash Load(XmlDocument doc)
		{
			return new VectoHash(doc);
		}

		protected VectoHash(XmlDocument doc)
		{
			Document = doc;
		}


		/// <summary>
		/// Get a list of all supported digest methods
		/// </summary>
		public static ICollection<string> SupportedDigestMehods => XMLHashProvider.SupportedDigestMethods;

		/// <summary>
		/// get the identifier of the default digest method
		/// </summary>
		public static string DefaultDigestMethod => XMLHashProvider.DefaultDigestMethod;

		/// <summary>
		/// get a list of all supported canonicalization methods
		/// </summary>
		public static ICollection<string> SupportedCanonicalizationMethods => XMLHashProvider.SupportedCanonicalizationMethods;

		/// <summary>
		/// get the sequence of the default canonicalization methods
		/// </summary>
		public static IEnumerable<string> DefaultCanonicalizationMethod => XMLHashProvider.DefaultCanonicalizationMethod;

		public IList<VectoComponents> GetContainigComponents()
		{
			var retVal = new List<VectoComponents>();
			foreach (var component in EnumHelper.GetValues<VectoComponents>()) {
				// special treatment for REESS: can be either supercap or multiple batteries where the component node may contain several sub-components
				var select = component == VectoComponents.ElectricEnergyStorage
					? $"//*[local-name()='{XMLNames.VectoInputDeclaration}']//*[local-name()='{component.XMLElementName()}']//*[local-name()='Data']"
					: $"//*[local-name()='{XMLNames.VectoInputDeclaration}']//*[local-name()='{component.XMLElementName()}']";
                var nodes = Document.SelectNodes(select);
				var count = nodes?.Count ?? 0;
				for (var i = 0; i < count; i++) {
					retVal.Add(component);
				}
			}
			return retVal;
		}

		public XElement ComputeXmlHash(IEnumerable<string> canonicalization = null, string digestMethod = null)
		{
			var nodes = Document.SelectNodes(GetComponentQueryString());
			if (nodes == null || nodes.Count == 0) {
				throw new Exception("No component found");
			}
			var componentId = nodes[0].Attributes[XMLNames.Component_ID_Attr].Value;
			var hash = DoComputeHash(nodes[0], canonicalization, digestMethod);
			return hash.ToXDocument().Root;
		}

		public string ComputeHash(IEnumerable<string> canonicalization = null, string digestMethod = null)
		{
			var nodes = Document.SelectNodes(GetComponentQueryString());
			if (nodes == null || nodes.Count == 0) {
				throw new Exception("No component found");
			}
			var componentId = nodes[0].Attributes[XMLNames.Component_ID_Attr].Value;
			return GetHashValueFromSig(DoComputeHash(nodes[0], canonicalization, digestMethod), componentId);
		}


		public string ComputeHash(VectoComponents component, int index = 0, IEnumerable<string> canonicalization = null,
			string digestMethod = null)
		{
			var nodes = Document.SelectNodes(GetComponentQueryString(component));

			if (nodes == null || nodes.Count == 0) {
				throw new Exception($"Component {component.XMLElementName()} not found");
			}
			if (index >= nodes.Count) {
				throw new Exception($"index exceeds number of components found! index: {index}, #components: {nodes.Count}");
			}
			var sortedNodes = SortComponentNodes(component, nodes);
			var componentId = sortedNodes[index].Attributes[XMLNames.Component_ID_Attr].Value;
			return GetHashValueFromSig(DoComputeHash(sortedNodes[index], canonicalization, digestMethod), componentId);
		}

		private static XmlDocument DoComputeHash(XmlNode dataNode, IEnumerable<string> canonicalization, string digestMethod)
		{
			var parent = dataNode.ParentNode;

			if (parent == null) {
				throw new Exception("Invalid structure of input XML!");
			}

			if (canonicalization == null) {
				canonicalization = ReadCanonicalizationMethods(parent);
			}
			if (digestMethod == null) {
				digestMethod = ReadDigestMethod(parent);
			}

			canonicalization = canonicalization ?? XMLHashProvider.DefaultCanonicalizationMethod;
			digestMethod = digestMethod ?? XMLHashProvider.DefaultDigestMethod;

			// copy the provided data Node to a new document before computing the hash
			// required if the same component (e.g. tire) is used multiple times
			var newDoc = new XmlDocument();
			var node = newDoc.CreateElement("Dummy");
			newDoc.AppendChild(node);
			var newNode = newDoc.ImportNode(parent, true);
			node.AppendChild(newNode);

			var componentId = dataNode.Attributes[XMLNames.Component_ID_Attr].Value;
			return XMLHashProvider.ComputeHash(newDoc, componentId, canonicalization, digestMethod);
		}

		private static string ReadDigestMethod(XmlNode rootNode)
		{
			var nodes = rootNode.SelectNodes("./*[local-name()='Signature']//*[local-name() = 'DigestMethod']/@Algorithm");
			if (nodes == null || nodes.Count == 0) {
				return null;
			}
			if (nodes.Count > 1) {
				throw new Exception("Multiple DigestValue elements found!");
			}
			return nodes[0].InnerText;
		}

		private static IEnumerable<string> ReadCanonicalizationMethods(XmlNode rootNode)
		{
			var nodes = rootNode.SelectNodes("./*[local-name()='Signature']//*[local-name() = 'Transform']/@Algorithm");
			if (nodes == null || nodes.Count == 0) {
				return null;
			}
			return (from XmlNode node in nodes select node.InnerText).ToArray();
		}

		public XDocument AddHash()
		{
			var component = GetComponentToHash();
			var query = $"//*[local-name()='{component.XMLElementName()}']/*[local-name()='Data']";
			var node = Document.SelectSingleNode(query);
			if (node == null) {
				throw new Exception($"'Data' element for component '{component.XMLElementName()}' not found!");
			}
			query = $"//*[local-name()='{component.XMLElementName()}']/*[local-name()='Signature']";
			var sigNodes = Document.SelectNodes(query);
			if (sigNodes != null && sigNodes.Count > 0) {
				throw new Exception("input data already contains a signature element");
			}

			var attributes = node.Attributes;
			var id = component.HashIdPrefix() + Guid.NewGuid().ToString("n").Substring(0, 20);
			var idSet = false;
			if (attributes != null && attributes[XMLNames.Component_ID_Attr] != null) {
				if (attributes[XMLNames.Component_ID_Attr].Value.Length < 5) {
					attributes[XMLNames.Component_ID_Attr].Value = id;
				} else {
					id = attributes[XMLNames.Component_ID_Attr].Value;
				}
				idSet = true;
			}
			if (!idSet) {
				var attr = Document.CreateAttribute(XMLNames.Component_ID_Attr);
				attr.Value = id;
				if (node.Attributes == null) {
					throw new Exception("failed to add 'id' attribute");
				}
				node.Attributes.Append(attr);
			}

			query = component.IsReport()
				? "*/*[local-name()='Data']/*[local-name()='ApplicationInformation']/*[local-name()='Date']"
				: $"*/*[local-name()='{component.XMLElementName()}']/*/*[local-name()='Date']";
			var dateNode = Document.SelectSingleNode(query);
			if (dateNode == null) {
				throw new Exception("Date-Element not found in input!");
			}
			dateNode.InnerText = XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc);


			var hash = XMLHashProvider.ComputeHash(Document, id, XMLHashProvider.DefaultCanonicalizationMethod,
				XMLHashProvider.DefaultDigestMethod);
			var sig = Document.CreateElement(XMLNames.DI_Signature, node.NamespaceURI);

			if (node.ParentNode == null || hash.DocumentElement == null) {
				throw new Exception("Invalid format of document and/or created hash");
			}
			sig.AppendChild(Document.ImportNode(hash.DocumentElement, true));
			node.ParentNode.AppendChild(sig);
			return Document.ToXDocument();
		}

		private VectoComponents GetComponentToHash()
		{
			if (Document.DocumentElement == null) {
				throw new Exception("invalid input document");
			}
			if (Document.DocumentElement.LocalName.Equals(XMLNames.VectoInputDeclaration)) {
				var components = GetContainigComponents();
				if (components.Contains(VectoComponents.Vehicle)) {
					throw new Exception("adding hash for Vehicle is not supported");
				}
				if (components.Count > 1) {
					throw new Exception("input must not contain multiple components!");
				}
				if (components.Count == 0) {
					throw new Exception("input does not contain a known component!");
				}
				return components.First();
			}
			if (Document.DocumentElement.LocalName.Equals("VectoOutput")) {
				return VectoComponents.VectoOutput;
			}
			if (Document.DocumentElement.LocalName.Equals("VectoCustomerInformation")) {
				return VectoComponents.VectoCustomerInformation;
			}
			if (Document.DocumentElement.LocalName.Equals("VectoOutputPrimaryVehicle")) {
				return VectoComponents.VectoPrimaryVehicleInformation;
			}
			if (Document.DocumentElement.LocalName.Equals(XMLNames.ManufacturingStep)) {
				return VectoComponents.VectoManufacturingStep;
			}
			throw new Exception("unknown document structure! neither input data nor output data format");
		}

		public string GetDigestMethod()
		{
			return DoGetDigestMethod(null, 0);
		}


		public string GetDigestMethod(VectoComponents component, int index = 0)
		{
			return DoGetDigestMethod(component, index);
		}

		private string DoGetDigestMethod(VectoComponents? component, int index)
		{
			var nodes = GetNodes(component, index);
			var digestmethod = ReadDigestMethod(nodes[index].ParentNode);
			digestmethod = digestmethod ?? XMLHashProvider.DefaultDigestMethod;
			return digestmethod;
		}

		public IEnumerable<string> GetCanonicalizationMethods()
		{
			return DoGetCanonicalizationMethods(null, 0);
		}

		public IEnumerable<string> GetCanonicalizationMethods(VectoComponents component, int index = 0)
		{
			return DoGetCanonicalizationMethods(component, index);
		}

		private IEnumerable<string> DoGetCanonicalizationMethods(VectoComponents? component, int index)
		{
			var nodes = GetNodes(component, index);
			var c14N = ReadCanonicalizationMethods(nodes[index].ParentNode);
			c14N = c14N ?? XMLHashProvider.DefaultCanonicalizationMethod;
			return c14N;
		}

		public string ReadHash()
		{
			return DoReadHash(null, 0);
		}

		public string ReadHash(VectoComponents component, int index = 0)
		{
			return DoReadHash(component, index);
		}

		public string GetCertificationNumber(VectoComponents component, int idx)
		{
			var nodes = GetNodes(component, idx);
			return ReadElementValue(nodes[idx], XMLNames.Component_CertificationNumber);
		}

		public DateTime GetCertificationDate(VectoComponents component, int idx)
		{
			var nodes = GetNodes(component, idx);
			return XmlConvert.ToDateTime(ReadElementValue(nodes[idx], XMLNames.Component_Date),
				XmlDateTimeSerializationMode.Local);
		}

		private string ReadElementValue(XmlNode xmlNode, string elementName)
		{
			var node = xmlNode.SelectSingleNode($"./*[local-name()='{elementName}']");
			if (node == null) {
				throw new Exception($"Node '{elementName}' not found!");
			}
			return node.InnerText;
		}

		private string DoReadHash(VectoComponents? component, int index)
		{
			var nodes = GetNodes(component, index);
			return ReadHashValue(nodes[index]);
		}

		private XmlNode[] GetNodes(VectoComponents? component, int index)
		{
			var nodes = Document.SelectNodes(GetComponentQueryString(component));
			if (nodes == null || nodes.Count == 0) {
				throw new Exception(component == null
					? "No component found"
					: $"Component {component.Value.XMLElementName()} not found");
			}
			if (index >= nodes.Count) {
				throw new Exception($"index exceeds number of components found! index: {index}, #components: {nodes.Count}");
			}
			return SortComponentNodes(component, nodes);
		}

		private XmlNode[] SortComponentNodes(VectoComponents? component, XmlNodeList nodes)
		{
			switch (component) {
				case VectoComponents.Tyre:
					return nodes.Cast<XmlNode>()
						.OrderBy(x => x.SelectSingleNode("./ancestor-or-self::*[@axleNumber]/@axleNumber")?.Value.ToInt() ?? 0).ToArray();
				case VectoComponents.ElectricEnergyStorage:
					return nodes.Cast<XmlNode>()
						.OrderBy(x => x.SelectSingleNode("./ancestor-or-self::*[local-name()='Battery']/*[local-name()='StringID']")?.InnerText.ToInt() ?? 0)
						.ThenBy(x => x.SelectSingleNode("./*[local-name()='Model']")?.InnerText ?? "")
						.ThenBy(x => x.SelectSingleNode("./*[local-name()='BatteryType']")?.InnerText ?? "")
						.ThenBy(x => x.SelectSingleNode("./*[local-name()='RatedCapacity']")?.InnerText ?? "")
						.ToArray();
				default:
					return nodes.Cast<XmlNode>().ToArray();
			}
		}

		public bool ValidateHash()
		{
			return ReadHash().Equals(ComputeHash(), StringComparison.Ordinal);
		}

		public bool ValidateHash(VectoComponents component, int index = 0)
		{
			return StructuralComparisons.StructuralEqualityComparer.Equals(ReadHash(component, index),
				ComputeHash(component, index));
		}


		protected static string GetComponentQueryString(VectoComponents? component = null)
		{
			switch (component) {
				case null:
					return "(//*[@id])[1]";
				case VectoComponents.Vehicle:
					return $"//*[local-name()='{component.Value.XMLElementName()}']";
				case VectoComponents.ElectricEnergyStorage:
					return $"//*[local-name()='{component.Value.XMLElementName()}']//*[local-name()='Data']";
				default:
					return $"//*[local-name()='{component.Value.XMLElementName()}']/*[local-name()='Data']";
			}
		}

		private static string GetHashValueFromSig(XmlDocument hashed, string elementId)
		{
			var nodes = hashed.SelectNodes("//*[@URI='#" + elementId + "']/*[local-name() = 'DigestValue']");
			if (nodes == null || nodes.Count == 0) {
				return null;
			}
			if (nodes.Count > 1) {
				throw new Exception("Multiple DigestValue elements found!");
			}
			return nodes[0].InnerText;
		}

		private static string ReadHashValue(XmlNode dataNode)
		{
			var parent = dataNode.ParentNode;
			if (parent == null) {
				throw new Exception("Invalid structure of input XML!");
			}
			var elementToHash = dataNode.Attributes[XMLNames.Component_ID_Attr].Value;
			var nodes = parent.SelectNodes(".//*[@URI='#" + elementToHash + "']/*[local-name() = 'DigestValue']");
			if (nodes == null || nodes.Count == 0) {
				return null;
			}
			if (nodes.Count > 1) {
				throw new Exception("Multiple DigestValue elements found!");
			}
			return nodes[0].InnerText;
		}
	}
}
