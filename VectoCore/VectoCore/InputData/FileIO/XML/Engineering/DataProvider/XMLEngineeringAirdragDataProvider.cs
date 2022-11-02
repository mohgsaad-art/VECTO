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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringAirdragDataProviderV07 : AbstractEngineeringXMLComponentDataProvider,
		IXMLAirdragData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "AirDragDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringAirdragDataProviderV07(
			IXMLEngineeringVehicleData vehicle,
			XmlNode axlegearNode, string fsBasePath)
			: base(vehicle, axlegearNode, fsBasePath)
		{
			SourceType = (vehicle as IXMLResource).DataSource.SourceFile == fsBasePath ? DataSourceType.XMLEmbedded : DataSourceType.XMLFile;
		}

		public virtual SquareMeter AirDragArea => GetDouble(XMLNames.Vehicle_AirDragArea).SI<SquareMeter>();

		public virtual SquareMeter TransferredAirDragArea => AirDragArea;

		public virtual SquareMeter AirDragArea_0 => AirDragArea;

		public virtual CrossWindCorrectionMode CrossWindCorrectionMode => GetString(XMLNames.Vehicle_CrossWindCorrectionMode).ParseEnum<CrossWindCorrectionMode>();

		public virtual TableData CrosswindCorrectionMap =>
			XMLHelper.ReadEntriesOrResource(
				BaseNode, DataSource.SourcePath, XMLNames.Vehicle_CrosswindCorrectionData, XMLNames.Vehicle_CrosswindCorrectionData_Entry,
				AttributeMappings.CrossWindCorrectionMapping);

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLEngineeringAirdragDataProviderV10 : XMLEngineeringAirdragDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "AirDragDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringAirdragDataProviderV10(
			IXMLEngineeringVehicleData vehicle, XmlNode axlegearNode, string fsBasePath) : base(
			vehicle, axlegearNode, fsBasePath) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}
