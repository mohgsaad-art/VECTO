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
using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLManufacturerReportReader
	{
		public static IList<VectoComponents> GetContainingComponents(XmlDocument xmlDocument)
		{
			var retVal = new List<VectoComponents>();
			foreach (var component in EnumHelper.GetValues<VectoComponents>()) {
				var nodes = xmlDocument.SelectNodes(string.Format("//*[local-name()='{0}']//*[local-name()='{1}']/*[local-name()='Model']",
																XMLNames.VectoManufacturerReport, component.XMLElementName()));
				var count = nodes?.Count ?? 0;
				for (var i = 0; i < count; i++) {
					retVal.Add(component);
				}
			}
			foreach (var component in new[] { XMLNames.AxleWheels_Axles_Axle }) {
				var nodes = xmlDocument.SelectNodes(string.Format("//*[local-name()='{0}']//*[local-name()='{1}']",
																XMLNames.VectoManufacturerReport, component));
				var count = nodes?.Count ?? 0;
				for (var i = 0; i < count; i++) {
					retVal.Add(VectoComponents.Tyre);
				}
			}
			return retVal;
		}

		public static string GetComponentDataDigestValue(XmlDocument xmlDocument, VectoComponents component, int i)
		{
			var node = GetNodes(xmlDocument, component, i);

			return ReadElementValue(node, XMLNames.DI_Signature_Reference_DigestValue);
		}

		public static XmlNode GetNodes(XmlDocument xmlDocument, VectoComponents component, int index)
		{
			var nodes = xmlDocument.SelectNodes(GetComponentQueryString(component == VectoComponents.Tyre ? "Axle" : component.XMLElementName()));
			if (nodes == null || nodes.Count == 0) {
				throw new Exception($"Component {component} not found");
			}
			if (index >= nodes.Count) {
				throw new Exception($"index exceeds number of components found! index: {index}, " +
									$"#components: {nodes.Count}");
			}
			return nodes[index];
		}

		static string GetComponentQueryString(string component = null)
		{
			if (component == null) {
				return "(//*[@id])[1]";
			}
			return $"//*[local-name()='{component}']";
		}

		static string ReadElementValue(XmlNode xmlNode, string elementName)
		{
			var node = xmlNode.SelectSingleNode($"./*[local-name()='{elementName}']");
			if (node == null) {
				return null;
			}
			return node.InnerText;
		}
	}
}
