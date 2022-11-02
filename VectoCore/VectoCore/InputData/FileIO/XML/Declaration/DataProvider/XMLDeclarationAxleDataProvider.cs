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
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationAxleDataProviderV10 : AbstractCommonComponentType, IXMLAxleDeclarationInputData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "AxleDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected ITyreDeclarationInputData _tyre;
		protected bool? _twinTyre;
		protected AxleType? _axleType;
		private bool? _steered;

		public XMLDeclarationAxleDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Implementation of IAxleDeclarationInputData

		public virtual bool TwinTyres => _twinTyre ?? (_twinTyre = XmlConvert.ToBoolean(GetString(XMLNames.AxleWheels_Axles_Axle_TwinTyres))).Value;

		public virtual AxleType AxleType => _axleType ?? (_axleType = GetString(XMLNames.AxleWheels_Axles_Axle_AxleType).ParseEnum<AxleType>()).Value;

		public virtual ITyreDeclarationInputData Tyre => _tyre ?? (_tyre = Reader.Tyre);

		public bool Steered =>
			_steered ?? (_steered = XmlConvert.ToBoolean(GetString(XMLNames.AxleWheels_Axles_Axle_Steered))).Value;

		#endregion


		#region Implementation of IXMLAxleDeclarationInputData

		public virtual IXMLAxleReader Reader { protected get; set; }

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationAxleDataProviderV20 : XMLDeclarationAxleDataProviderV10
	{
		public new static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "AxleDataDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		static XMLDeclarationAxleDataProviderV20()
		{
			NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;
		}

		public XMLDeclarationAxleDataProviderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}
