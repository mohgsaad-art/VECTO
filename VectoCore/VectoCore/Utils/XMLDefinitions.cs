using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TUGraz.VectoCore.Utils
{
	[Flags]
	public enum XmlDocumentType
	{
		DeclarationJobData = 1 << 1,
		PrimaryVehicleBusOutputData = 1 << 2,
		DeclarationComponentData = 1 << 3,
		EngineeringJobData = 1 << 4,
		EngineeringComponentData = 1 << 5,
		ManufacturerReport = 1 << 6,
		CustomerReport = 1 << 7,
		MonitoringReport = 1 << 8,
		VTPReport = 1 << 9,
		DeclarationTrailerJobData = 1 << 10,
	}

	
	public static class XMLDefinitions
	{
		//public const string SchemaBaseURL = "file:///E:/QUAM/Workspace/VECTO_quam/VectoCore/VectoCore/Resources/XSD/";
		public const string SCHEMA_BASE_LOCATION = "https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/XSD/";

		public const string DEV = ":DEV";

		public const string ENGINEERING_NAMESPACE = "urn:tugraz:ivt:VectoAPI:EngineeringDefinitions"; 

		public const string ENGINEERING_INPUT_NAMESPACE_URI_V07 = "urn:tugraz:ivt:VectoAPI:EngineeringInput:v0.7";

		public const string ENGINEERING_INPUT_NAMESPACE_URI_V10 = "urn:tugraz:ivt:VectoAPI:EngineeringInput:v1.0";

		public const string ENGINEERING_DEFINITONS_NAMESPACE_V07 = ENGINEERING_NAMESPACE + ":v0.7";
		
		public const string ENGINEERING_DEFINITONS_NAMESPACE_V10 = ENGINEERING_NAMESPACE + ":v1.0";

		public const string ENGINEERING_DEFINITONS_NAMESPACE_V11 = ENGINEERING_NAMESPACE + ":v1.1";

		public const string ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST = ENGINEERING_NAMESPACE + ":v1.0TEST";


		public const string DECLARATION_NAMESPACE = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions";
			
		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V10 = DECLARATION_NAMESPACE + ":v1.0";

		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V20 = DECLARATION_NAMESPACE + ":v2.0";

		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V21 = DECLARATION_NAMESPACE + ":v2.1";

		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V22 = DECLARATION_NAMESPACE + ":v2.2";

		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V23 = DECLARATION_NAMESPACE + ":v2.3";

		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V26 = DECLARATION_NAMESPACE + ":DEV:v2.6";

		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V27 = DECLARATION_NAMESPACE + ":DEV:v2.7";

		public const string DECLARATION_INPUT_NAMESPACE = "urn:tugraz:ivt:VectoAPI:DeclarationInput";

		public const string DECLARATION_INPUT_NAMESPACE_URI_V10 = DECLARATION_INPUT_NAMESPACE + ":v1.0";

		public const string DECLARATION_INPUT_NAMESPACE_URI_V20 = DECLARATION_INPUT_NAMESPACE + ":v2.0";

        public const string DECLARATION_COMPONENT_NAMESPACE_URI_V10 = "urn:tugraz:ivt:VectoAPI:DeclarationComponent:v1.0";

		public const string DECLARATION_COMPONENT_NAMESPACE_URI_V20 = "urn:tugraz:ivt:VectoAPI:DeclarationComponent:v2.0";

		public const string DECLARATION_PRIMARY_BUS_VEHICLE_NAMESPACE = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:PrimaryVehicleInformation";

		public const string DECLARATION_PRIMARY_BUS_VEHICLE_URI_V01 =
			DECLARATION_PRIMARY_BUS_VEHICLE_NAMESPACE + ":HeavyBus:v0.1";


		public const string DECLARATION_MANUFACTURER_REPORT_V05 = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.5";

		public const string DECLARATION_CUSTOMER_REPORT_V05 = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.5";

		public const string DECLARATION_VTP_REPORT_V01 = "urn:tugraz:ivt:VectoAPI:VTPReport:v0.1";


		public const string XML_SCHEMA_NAMESPACE = "http://www.w3.org/2001/XMLSchema-instance";


		// mapping of document type + version => supported schema files (+version)
		//private static Dictionary<Tuple<XmlDocumentType, string>, IList<string>> schemaFilenames = new Dictionary<Tuple<XmlDocumentType, string>, IList<string>>();

		private static Dictionary<XmlDocumentType, string> schemaFilenames = new Dictionary<XmlDocumentType, string>() {
			{XmlDocumentType.DeclarationJobData, "VectoDeclarationJob.xsd"},
			{XmlDocumentType.DeclarationTrailerJobData, "VectoDeclarationJob.xsd"},
			{XmlDocumentType.PrimaryVehicleBusOutputData, "VectoOutputPrimaryVehicleInformation.xsd"},
			{XmlDocumentType.DeclarationComponentData, "VectoDeclarationComponent.xsd"},
			{XmlDocumentType.EngineeringJobData, "VectoEngineeringJob.xsd" },
			{XmlDocumentType.EngineeringComponentData, "VectoEngineeringComponent.xsd" },
			{XmlDocumentType.ManufacturerReport, "VectoOutputManufacturer.xsd" },
			{XmlDocumentType.CustomerReport , "VectoOutputCustomer.xsd"},
			{XmlDocumentType.MonitoringReport , "VectoMonitoring.xsd"},
			{XmlDocumentType.VTPReport , "VTPReport.xsd"},
		};

		public static XNamespace DECLARATION_OUTPUT_PRIMARY_HEAVY_BUS = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:PrimaryVehicleInformation:HeavyBus:v0.1";


		public static string GetSchemaFilename(XmlDocumentType type)
		{
			if (!schemaFilenames.ContainsKey(type)) {
				throw new Exception(string.Format("Invalid argument {0} - only use single flags", type));
			}
			var entry = schemaFilenames[type];
			
			return entry;
		}


		public static string GetSchemaVersion(string nodeType)
		{
			var parts = nodeType?.Split(':');
			if (parts?.Length == 2) {
				return XMLHelper.GetVersionFromNamespaceUri(parts[0]);
			}

			return null;
		}
	}
}
