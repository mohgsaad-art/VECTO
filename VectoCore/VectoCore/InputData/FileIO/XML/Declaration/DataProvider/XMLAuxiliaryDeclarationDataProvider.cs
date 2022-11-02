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
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLAuxiliaryDeclarationDataProviderV10 : AbstractXMLType, IXMLAuxiliaryDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "AuxiliariesComponentDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected AuxiliaryType? _type;
		private IList<string> _technology;

		public XMLAuxiliaryDeclarationDataProviderV10(XmlNode auxNode, IXMLDeclarationVehicleData vehicle) : base(auxNode) { }

		#region Implementation of IAuxiliaryDeclarationInputData


		public virtual AuxiliaryType Type => _type ?? (_type = BaseNode.LocalName.ParseEnum<AuxiliaryType>()).Value;

		public virtual IList<string> Technology
		{
			get {
				if (_technology != null) {
					return _technology;
				}

				_technology = new List<string>();
				var techNodes = GetNodes(XMLNames.Auxiliaries_Auxiliary_Technology);
				foreach (XmlNode techNode in techNodes) {
					_technology.Add(techNode.InnerText);
				}

				return _technology;
			}
		}

		#endregion
	}

	// =============================

	public class XMLAuxiliaryDeclarationDataProviderV20 : XMLAuxiliaryDeclarationDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "AuxiliariesComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLAuxiliaryDeclarationDataProviderV20(XmlNode auxNode, IXMLDeclarationVehicleData vehicle) : base(
			auxNode, vehicle) { }
	}

	
	// =============================

	public class XMLAuxiliaryDeclarationDataProviderV24_Lorry : XMLAuxiliaryDeclarationDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public new const string XSD_TYPE = "AUX_Component_Conventional_Lorry_Type";
		public const string XSD_HEV_P_TYPE = "AUX_Component_HEV-P_Lorry_Type";
		public const string XSD_HEV_S_TYPE = "AUX_Component_HEV-S_Lorry_Type";
		public const string XSD_PEV_E2_TYPE = "AUX_Component_PEV_Lorry_Type";
		public const string XSD_IEPC_TYPE = "AUX_Component_IEPC_Lorry_Type";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string QUALIFIED_XSD_HEV_P_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_P_TYPE);
		public static readonly string QUALIFIED_XSD_HEV_S_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_S_TYPE);
		public static readonly string QUALIFIED_XSD_PEV_E2_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_PEV_E2_TYPE);
		public static readonly string QUALIFIED_XSD_IEPC_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_IEPC_TYPE);

		public XMLAuxiliaryDeclarationDataProviderV24_Lorry(XmlNode auxNode, IXMLDeclarationVehicleData vehicle) : base(
			auxNode, vehicle)
		{ }
	}
}
