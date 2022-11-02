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
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationTyreDataProviderV10 : AbstractCommonComponentType, IXMLTyreDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "TyreDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected string _dimension;
		protected double? _rrc;
		protected Newton _fzIso;

		public XMLDeclarationTyreDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of ITyreDeclarationInputData

		public virtual string Dimension => _dimension ?? (_dimension = GetString(XMLNames.AxleWheels_Axles_Axle_Dimension));

		public virtual double RollResistanceCoefficient => _rrc ?? (_rrc = GetDouble(XMLNames.AxleWheels_Axles_Axle_RRCDeclared)).Value;

		public virtual Newton TyreTestLoad => _fzIso ?? (_fzIso = GetDouble(XMLNames.AxleWheels_Axles_Axle_FzISO).SI<Newton>());

		public virtual string FuelEfficiencyClass => DeclarationData.Wheels.TyreClass.Lookup(RollResistanceCoefficient);

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationTyreDataProviderV20 : XMLDeclarationTyreDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "TyreComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationTyreDataProviderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationTyreDataProviderV22 : XMLDeclarationTyreDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V22;

		//public new const string XSD_TYPE = "TyreComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationTyreDataProviderV22(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationTyreDataProviderV23 : XMLDeclarationTyreDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		//public new const string XSD_TYPE = "TyreComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationTyreDataProviderV23(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		public override string FuelEfficiencyClass => GetString("FuelEfficiencyClass");
	}
}
