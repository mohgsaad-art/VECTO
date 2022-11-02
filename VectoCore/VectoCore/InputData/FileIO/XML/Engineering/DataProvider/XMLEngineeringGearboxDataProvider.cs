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
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringGearboxDataProviderV07 : AbstractEngineeringXMLComponentDataProvider,
		IXMLGearboxData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "GearboxDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringGearboxDataProviderV07(
			IXMLEngineeringVehicleData vehicle,
			XmlNode axlegearNode, string fsBasePath)
			: base(vehicle, axlegearNode, fsBasePath)
		{
			SourceType = (vehicle as IXMLResource).DataSource.SourceFile == fsBasePath ? DataSourceType.XMLEmbedded : DataSourceType.XMLFile;
		}

		public virtual GearboxType Type => GetString(XMLNames.Gearbox_TransmissionType).ParseEnum<GearboxType>();


		public virtual KilogramSquareMeter Inertia => GetDouble(XMLNames.Gearbox_Inertia).SI<KilogramSquareMeter>();

		public virtual Second TractionInterruption => GetDouble(XMLNames.Gearbox_TractionInterruption).SI<Second>();

		public virtual Second PowershiftShiftTime =>
			GetNode(XMLNames.DriverModel_ShiftStrategyParameters_PowershiftShiftTime,
					((XMLEngineeringInputDataProviderV07)Vehicle.Job.InputData).Document.DocumentElement, required: false)
				?.InnerText.ToDouble().SI<Second>() ?? Constants.DefaultPowerShiftTime;

		public virtual IList<ITransmissionInputData> Gears
		{
			get {
				var gearNodes = GetNodes(new[] { XMLNames.Gearbox_Gears, XMLNames.Gearbox_Gears_Gear });
				if (gearNodes == null || gearNodes.Count == 0) {
					return new List<ITransmissionInputData>();
				}

				var retVal = new List<ITransmissionInputData>();
				foreach (XmlNode gearNode in gearNodes) {
					retVal.Add(Reader.CreateGear(gearNode));
				}

				return retVal;
			}
		}

		public virtual bool DifferentialIncluded => false;
		public virtual double AxlegearRatio => double.NaN;


		public IXMLGearboxReader Reader { protected get; set; }

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}


	internal class XMLEngineeringGearboxDataProviderV10 : XMLEngineeringGearboxDataProviderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		//public new const string XSD_TYPE = "GearboxComponentEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringGearboxDataProviderV10(
			IXMLEngineeringVehicleData vehicle, XmlNode axlegearNode, string fsBasePath) : base(
			vehicle, axlegearNode, fsBasePath) { }

		#region Overrides of XMLEngineeringGearboxDataProviderV07

		public override Second PowershiftShiftTime => GetNode(XMLNames.DriverModel_ShiftStrategyParameters_PowershiftShiftTime, required: false)?.InnerText.ToDouble().SI<Second>() ?? Constants.DefaultPowerShiftTime;

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		#endregion
	}

	public abstract class XMLAbstractGearData : AbstractXMLType
	{
		protected string BasePath;

		protected DataSource _dataSource;

		public XMLAbstractGearData(XmlNode node, string basePath) : base(node)
		{
			BasePath = basePath;
		}

		public virtual int Gear =>
			XmlConvert.ToUInt16(
				BaseNode.Attributes?.GetNamedItem(XMLNames.Gearbox_Gear_GearNumber_Attr).InnerText ?? "0");

		public virtual double Ratio =>
			BaseNode.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Gearbox_Gear_Ratio))?.InnerText.ToDouble() ??
			double.NaN;

		public virtual TableData LossMap =>
			XMLHelper.ReadEntriesOrResource(
				BaseNode, BasePath, XMLNames.Gearbox_Gear_TorqueLossMap, XMLNames.Gearbox_Gear_TorqueLossMap_Entry,
				AttributeMappings.TransmissionLossmapMapping);

		public virtual double Efficiency
		{
			get {
				return GetNode(new[] { XMLNames.Gearbox_Gear_TorqueLossMap, XMLNames.Gearbox_Gear_Efficiency })
							?.InnerText.ToDouble() ?? double.NaN;
			}
		}

		public virtual DataSource DataSource => _dataSource ?? (_dataSource = new DataSource() { SourceFile = BasePath, SourceType = DataSourceType.XMLEmbedded, SourceVersion = XMLHelper.GetVersionFromNamespaceUri(SchemaNamespace) });

		protected abstract XNamespace SchemaNamespace { get; }
	}

	internal class XMLGearDataV07 : XMLAbstractGearData, IXMLGearData
	{
		
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "GearEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLGearDataV07(XmlNode gearNode, string basePath) : base(gearNode, basePath) { }

		#region Implementation of ITransmissionInputData

		public virtual NewtonMeter MaxTorque =>
			GetNode(XMLNames.Gearbox_Gears_MaxTorque, required: false)?.InnerText
				.ToDouble().SI<NewtonMeter>();

		public virtual PerSecond MaxInputSpeed => GetNode(XMLNames.Gearbox_Gear_MaxSpeed, required: false)?.InnerText.ToDouble().RPMtoRad();

		public virtual TableData ShiftPolygon =>
			XMLHelper.ReadEntriesOrResource(
				BaseNode, BasePath, XMLNames.Gearbox_Gears_Gear_ShiftPolygon, XMLNames.TorqueConverter_ShiftPolygon_Entry,
				AttributeMappings.ShiftPolygonMapping);

		#endregion

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}

	internal class XMLGearDataV10 : XMLGearDataV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "GearEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLGearDataV10(XmlNode gearNode, string basePath) : base(gearNode, basePath) { }


		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
	}
}
