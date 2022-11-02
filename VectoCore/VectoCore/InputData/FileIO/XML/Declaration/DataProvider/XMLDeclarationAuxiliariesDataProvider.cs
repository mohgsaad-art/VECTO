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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationAuxiliariesDataProviderV10 : AbstractXMLType, IXMLAuxiliariesDeclarationInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "AuxiliariesDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IList<IAuxiliaryDeclarationInputData> _auxiliaries;


		public XMLDeclarationAuxiliariesDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		#region Implementation of IAuxiliariesDeclarationInputData

		public virtual bool SavedInDeclarationMode => true;

		public virtual IList<IAuxiliaryDeclarationInputData> Auxiliaries
		{
			get {
				if (_auxiliaries != null) {
					return _auxiliaries;
				}

				_auxiliaries = new List<IAuxiliaryDeclarationInputData>();

				//var auxNodes = GetNodes(XMLNames.Auxiliaries_Auxiliary);
				var auxNodes = BaseNode.SelectNodes(XMLHelper.QueryLocalName(XMLNames.Auxiliaries_Auxiliary_Technology) + "/..");
				if (auxNodes == null) {
					return _auxiliaries;
				}

				foreach (XmlNode auxNode in auxNodes) {
					_auxiliaries.Add(Reader.CreateAuxiliary(auxNode));
				}

				return _auxiliaries;
			}
		}

		#endregion

		#region Implementation of IXMLAuxiliariesDeclarationInputData

		public virtual IXMLAuxiliaryReader Reader { protected get; set; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationAuxiliariesDataProviderV20 : XMLDeclarationAuxiliariesDataProviderV10
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "AuxiliariesComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationAuxiliariesDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }
	}

	
	// ---------------------------------------------------------------------------------------

	
	public class XMLDeclarationAuxiliariesDataProviderV24_Lorry: XMLDeclarationAuxiliariesDataProviderV20
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

        public new const string XSD_TYPE = "AUX_Conventional_LorryDataType";
		public const string  XSD_HEV_P_TYPE = "AUX_HEV-P_LorryDataType";
		public const string XSD_HEV_S_TYPE = "AUX_HEV-S_LorryDataType";
		public const string XSD_PEV_TYPE = "AUX_PEV_LorryDataType";
		public const string XSD_IEPC_TYPE = "AUX_IEPC_LorryDataType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		public static readonly string QUALIFIED_XSD_HEV_P_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_P_TYPE);
		public static readonly string QUALIFIED_XSD_HEV_S_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_HEV_S_TYPE);
		public static readonly string QUALIFIED_XSD_PEV_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_PEV_TYPE);
		public static readonly string QUALIFIED_XSD_IEPC_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_IEPC_TYPE);
		
		public XMLDeclarationAuxiliariesDataProviderV24_Lorry(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(vehicle, componentNode, sourceFile) { }
	}
}
