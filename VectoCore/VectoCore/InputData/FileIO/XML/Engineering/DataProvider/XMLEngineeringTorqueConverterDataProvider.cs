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
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Impl
{
	internal class XMLEngineeringTorqueConverterDataProviderV07 : AbstractEngineeringXMLComponentDataProvider,
		IXMLTorqueconverterData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "TorqueConverterDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringTorqueConverterDataProviderV07(
			IXMLEngineeringVehicleData vehicle, XmlNode node, string source) : base(vehicle, node, source)
		{
			SourceType = (vehicle as IXMLResource).DataSource.SourceFile == source ? DataSourceType.XMLEmbedded : DataSourceType.XMLFile;
		}

		#region Implementation of ITorqueConverterDeclarationInputData

		public virtual TableData TCData =>
			XMLHelper.ReadEntriesOrResource(
				BaseNode, DataSource.SourcePath, XMLNames.TorqueConverter_Characteristics, XMLNames.TorqueConverter_Characteristics_Entry,
				AttributeMappings.TorqueConverterDataMapping);

		#endregion

		#region Implementation of ITorqueConverterEngineeringInputData

		public virtual PerSecond ReferenceRPM =>
			GetNode(XMLNames.TorqueConverter_ReferenceRPM)?.InnerText.ToDouble().RPMtoRad() ??
			DeclarationData.TorqueConverter.ReferenceRPM;

		public virtual KilogramSquareMeter Inertia =>
			GetNode(XMLNames.TorqueConverter_Inertia)?.InnerText.ToDouble().SI<KilogramSquareMeter>() ??
			0.SI<KilogramSquareMeter>();

		public virtual TableData ShiftPolygon =>
			XMLHelper.ReadEntriesOrResource(
				BaseNode, DataSource.SourcePath, XMLNames.TorqueConverter_ShiftPolygon, XMLNames.TorqueConverter_ShiftPolygon_Entry,
				AttributeMappings.ShiftPolygonMapping);

		public virtual PerSecond MaxInputSpeed => GetNode(XMLNames.TorqueConverter_MaxInputSpeed)?.InnerText.ToDouble().RPMtoRad();

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLEngineeringTorqueConverterDataProviderV10 : XMLEngineeringTorqueConverterDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "TorqueConverterDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringTorqueConverterDataProviderV10(IXMLEngineeringVehicleData vehicle, XmlNode node, string source) :
			base(vehicle, node, source) { }

		#region Overrides of XMLEngineeringTorqueConverterDataProviderV07

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#endregion
	}
}
