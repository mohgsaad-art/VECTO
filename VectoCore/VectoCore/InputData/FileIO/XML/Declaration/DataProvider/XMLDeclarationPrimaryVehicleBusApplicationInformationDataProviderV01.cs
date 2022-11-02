using System;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationMultistagePrimaryVehicleBusApplicationInformationDataProviderV01 :
		AbstractXMLType, IXMLApplicationInformationData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "ApplicationInformationPrimaryVehicleType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationMultistagePrimaryVehicleBusApplicationInformationDataProviderV01(XmlNode applicationNode)
			: base(applicationNode) { }

		public string SimulationToolVersion => GetString(XMLNames.Report_ApplicationInfo_SimulationToolVersion);

		public DateTime Date => XmlConvert.ToDateTime(GetString(XMLNames.Report_ApplicationInfo_Date), XmlDateTimeSerializationMode.Utc);
	}
}