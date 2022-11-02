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

using System.Xml;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	public abstract class AbstractXMLType
	{
		protected readonly XmlNode BaseNode;

		protected AbstractXMLType(XmlNode node)
		{
			BaseNode = node;
		}

		protected bool ElementExists(string nodeName)
		{
			return GetNode(nodeName, BaseNode, required: false) != null;
		}

		protected string GetString(string nodeName, XmlNode basenode = null, bool required = true)
		{
			return GetNode(nodeName, basenode, required)?.InnerText;
		}

		protected double GetDouble(string nodeName, double? fallbackValue = null)
		{
			var node = GetNode(nodeName, required: fallbackValue != null);

			return node?.InnerText.ToDouble() ?? fallbackValue.Value;
		}

		protected double GetDouble(string[] nodePath, double? fallbackValue = null)
		{
			var node = GetNode(nodePath, required: fallbackValue == null);

			return node?.InnerText.ToDouble() ?? fallbackValue.Value;
		}

		protected bool GetBool(string nodeName) 
		{
			return XmlConvert.ToBoolean(GetNode(nodeName).InnerText);
		}

		protected XmlNode GetNode(string[] nodeName, XmlNode baseNode = null, bool required = true)
		{
			return DoGetNode(XMLHelper.QueryLocalName(nodeName), baseNode, required);
		}

		protected XmlNode GetNode(string nodeName, XmlNode baseNode = null, bool required = true)
		{
			return DoGetNode(XMLHelper.QueryLocalName(nodeName), baseNode, required);
		}

		private XmlNode DoGetNode(string xpathQuery, XmlNode baseNode, bool required)
		{
			var node = (baseNode ?? BaseNode)?.SelectSingleNode(xpathQuery);
			if (required && node == null) {
				throw new VectoException("Node {0} not found", xpathQuery);
			}

			return node;
		}

		protected XmlNodeList GetNodes(string nodeName, XmlNode baseNode = null)
		{
			return (baseNode ?? BaseNode).SelectNodes(XMLHelper.QueryLocalName(nodeName));
		}

		protected XmlNodeList GetNodes(string[] nodeName, XmlNode baseNode = null)
		{
			return (baseNode ?? BaseNode).SelectNodes(XMLHelper.QueryLocalName(nodeName));
		}

		protected string GetAttribute(XmlNode node, string attribute)
		{
			return node?.Attributes?.GetNamedItem(attribute)?.InnerText;
		}
	}
}
