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

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace TUGraz.VectoCommon.InputData {
	public class DigestData
	{
		private const string ReferenceElementXPath = ".//*[local-name()='Reference']";
		private const string TransformElementXPath = ".//*[local-name()='Transform']";
		private const string DigestMethodElementXPath = ".//*[local-name()='DigestMethod']";

		private const string UriAttr = "URI";
		private const string AlgorithmAttr = "Algorithm";

		private const string ReferenceUriAttrXPath = ReferenceElementXPath + "/@" + UriAttr;
		private const string TransformAlgorithmAttrXPath = TransformElementXPath + "/@" + AlgorithmAttr;
		private const string DigestMethodAlgorithmAttrXPath = DigestMethodElementXPath + "/@" + AlgorithmAttr;
		private const string DigestValueElementXPath = ".//*[local-name()='DigestValue']";

		public DigestData(string reference, string[] c14n, string digestMethod, string digestValue)
		{
			Reference = reference;
			CanonicalizationMethods = c14n;
			DigestMethod = digestMethod;
			DigestValue = digestValue;
		}

		public DigestData(XPathNavigator navigator)
		{
			Reference = navigator.SelectSingleNode(ReferenceUriAttrXPath)?.InnerXml;
			var nodes = navigator.Select(TransformAlgorithmAttrXPath);
			var c14n = new List<string>();
			while (nodes.MoveNext()) {
				c14n.Add(nodes.Current.InnerXml);
			}
			CanonicalizationMethods = c14n.ToArray();
			DigestMethod = navigator.SelectSingleNode(DigestMethodAlgorithmAttrXPath)?.InnerXml;
			DigestValue = navigator.SelectSingleNode(DigestValueElementXPath)?.InnerXml;
		}

		public DigestData(XmlNode xmlNode)
		{
			Reference = xmlNode?.SelectSingleNode(ReferenceUriAttrXPath)?.InnerXml;
			var nodes = xmlNode?.SelectNodes(TransformAlgorithmAttrXPath);
			var c14n = new List<string>();
			if (nodes != null) {
				for (var i = 0; i < nodes.Count; i++) {
					c14n.Add(nodes[i].InnerXml);
				}
			}
			CanonicalizationMethods = c14n.ToArray();
			DigestMethod = xmlNode?.SelectSingleNode(DigestMethodAlgorithmAttrXPath)?.InnerXml;
			DigestValue = xmlNode?.SelectSingleNode(DigestValueElementXPath)?.InnerXml;
		}

		public DigestData(XNode xmlNode)
		{
			Reference = xmlNode.XPathSelectElement(ReferenceElementXPath)?.Attribute(XName.Get(UriAttr))?.Value;
			var nodes = xmlNode.XPathSelectElements(TransformElementXPath);
			var c14n = new List<string>();

			foreach (var node in nodes) {
				c14n.Add(node.Attribute(XName.Get(AlgorithmAttr))?.Value);
			}

			CanonicalizationMethods = c14n.ToArray();
			DigestMethod = xmlNode.XPathSelectElement(DigestMethodElementXPath)?.Attribute(XName.Get(AlgorithmAttr))?.Value;
			DigestValue = xmlNode.XPathSelectElement(DigestValueElementXPath)?.Value;
		}

		public string DigestValue { get; }

		public string Reference { get; }

		public string[] CanonicalizationMethods { get; }
		public string DigestMethod { get; }

		public XElement ToXML(XNamespace di)
		{
			return new XElement(di + "Reference",
				new XAttribute("URI", Reference),
				new XElement(di + "Transforms", 
				CanonicalizationMethods.Select(x => new XElement(di + "Transform", new XAttribute("Algorithm", x)))
				),
				new XElement(di + "DigestMethod", new XAttribute("Algorithm",  DigestMethod)),
				new XElement(di + "DigestValue", DigestValue)
				);
		}
	}
}