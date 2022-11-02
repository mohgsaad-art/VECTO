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
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLADASReaderV10 : AbstractComponentReader, IXMLADASReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "ADASType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IXMLDeclarationVehicleData Vehicle;
		protected IAdvancedDriverAssistantSystemDeclarationInputData _adas;


		public XMLADASReaderV10(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode) : base(
			vehicle, vehicleNode)
		{
			Vehicle = vehicle;
		}

		#region Implementation of IXMLADASReader

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADASInputData => _adas ?? (_adas = CreateComponent(XMLNames.Vehicle_ADAS, ADASCreator));

		#endregion

		protected virtual IAdvancedDriverAssistantSystemDeclarationInputData ADASCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				version = XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace);
			}
			return Factory.CreateADASData(version, Vehicle, componentNode, sourceFile);
		}

		public virtual XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLADASReaderV20 : XMLADASReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "ADASType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		public XMLADASReaderV20(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode) : base(
			vehicle, vehicleNode) { }

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADASInputData => _adas ?? (_adas = CreateComponent(XMLNames.Vehicle_ADAS, ADASCreator, true));

		public override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLADASReaderV21 : XMLADASReaderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "AdvancedDriverAssistantSystemsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLADASReaderV21(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode) : base(
			vehicle, vehicleNode) { }

		public override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	// ---------------------------------------------------------------------------------------

	public class XMLADASReaderV24 : XMLADASReaderV21
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;

		public const string XSD_TYPE_CONVENTIONAL = "ADAS_Conventional_Type";
		public const string XSD_TYPE_HEV = "ADAS_HEV_Type";
		public const string XSD_TYPE_PEV = "ADAS_PEV_Type";
		public const string XSD_TYPE_IEPC = "ADAS_IEPC_Type";

		public static readonly string QUALIFIED_XSD_TYPE_CONVENTIONAL = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE_CONVENTIONAL);

		public static readonly string QUALIFIED_XSD_TYPE_HEV = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE_HEV);

		public static readonly string QUALIFIED_XSD_TYPE_PEV = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE_PEV);

		public static readonly string QUALIFIED_XSD_TYPE_IEPC = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE_IEPC);

		public XMLADASReaderV24(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode) : base(
			vehicle, vehicleNode)
		{ }

		public override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

}
