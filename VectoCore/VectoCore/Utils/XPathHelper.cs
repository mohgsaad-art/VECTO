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

using System.Linq;
using System.Xml;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Utils
{
	public class XPathHelper
	{
		protected readonly ExecutionMode Mode;
		protected readonly string DefaultPrefix;

		public XPathHelper(ExecutionMode mode)
		{
			Mode = mode;
			DefaultPrefix = Mode == ExecutionMode.Declaration
				? Constants.XML.DeclarationNSPrefix
				: Constants.XML.EngineeringNSPrefix;
		}

		public string Query(params string[] xpathSections) {
			return xpathSections.Select(
				x => string.IsNullOrWhiteSpace(x) || x.StartsWith("..") || x.Contains(":") || x.StartsWith("@") ? x : NSPrefix(x)).Join("/");
		}

		public string QueryConstraint(string elementName, string name, string value,
			string prefix = "@")
		{
			return value == null
				? string.Format("{0}[{2}{1}]", elementName, name, prefix)
				: string.Format("{0}[{3}{1}='{2}']", elementName, name, value, prefix);
		}

		public string QueryAbs(params string[] xPathSections)
		{
			return $"/{Query(xPathSections)}";
		}

		public string NSPrefix(string element, string prefix = null)
		{
			if (prefix == null) {
				prefix = DefaultPrefix;
			}
			return string.Format("{1}:{0}", element, prefix);
		}

		public void AddNamespaces(XmlNamespaceManager manager)
		{
			manager.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
			if (Mode == ExecutionMode.Declaration) {
				manager.AddNamespace(Constants.XML.DeclarationNSPrefix, Constants.XML.VectoDeclarationDefinitionsNS);
				manager.AddNamespace(Constants.XML.RootNSPrefix, Constants.XML.VectoDeclarationInputNS);
			} else {
				manager.AddNamespace(Constants.XML.EngineeringNSPrefix, Constants.XML.VectoEngineeringDefinitionsNS);
				manager.AddNamespace(Constants.XML.RootNSPrefix, Constants.XML.VectoEngineeringInputNS);
			}
		}
	}
}