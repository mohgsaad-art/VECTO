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
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public abstract class XMLAbstractGearData : AbstractXMLType
	{
		protected TableData _lossmap;
		protected DataSource _dataSource;

		protected XMLAbstractGearData(XmlNode gearNode, string sourceFile) : base(gearNode)
		{
			SourceFile = sourceFile;
		}

		public virtual string SourceFile { get; }


		public virtual DataSource DataSource =>
			_dataSource ?? (_dataSource = new DataSource() {
				SourceFile = SourceFile,
				SourceType = DataSourceType.XMLEmbedded,
				SourceVersion = XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace)
			});

		protected abstract XNamespace SchemaNamespace { get; }

		#region Implementation of ITransmissionInputData

		public virtual int Gear =>
			XmlConvert.ToUInt16(
				BaseNode.Attributes?.GetNamedItem(XMLNames.Gearbox_Gear_GearNumber_Attr).InnerText ?? "0");

		public virtual double Ratio => GetString(XMLNames.Gearbox_Gear_Ratio).ToDouble(double.NaN);

		public virtual TableData LossMap
		{
			get {
				return _lossmap ?? (_lossmap = XMLHelper.ReadTableData(
							AttributeMappings.TransmissionLossmapMapping,
							GetNodes(new[] { XMLNames.Gearbox_Gear_TorqueLossMap, XMLNames.Gearbox_Gear_TorqueLossMap_Entry })));
			}
		}

		public virtual double Efficiency => double.NaN;

		public virtual NewtonMeter MaxTorque => GetNode(XMLNames.Gearbox_Gears_MaxTorque, required: false)?.InnerText.ToDouble().SI<NewtonMeter>();

		public virtual PerSecond MaxInputSpeed => GetNode(XMLNames.Gearbox_Gear_MaxSpeed, required: false)?.InnerText.ToDouble().RPMtoRad();

		public virtual TableData ShiftPolygon => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLGearDataV10 : XMLAbstractGearData, IXMLGearData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "GearDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLGearDataV10(XmlNode gearNode, string sourceFile) : base(gearNode, sourceFile) { }

		#region Overrides of XMLAbstractGearData

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLGearDataV20 : XMLGearDataV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "GearDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		public XMLGearDataV20(XmlNode gearNode, string sourceFile) : base(gearNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}


	// ---------------------------------------------------------------------------------------
	
	public class XMLMultistagePrimaryVehicleBusTransmissionDataV01 : XMLGearDataV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "TransmissionGearVIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLMultistagePrimaryVehicleBusTransmissionDataV01(XmlNode gearNode, string sourceFile)
			: base(gearNode, sourceFile) { }

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

}
