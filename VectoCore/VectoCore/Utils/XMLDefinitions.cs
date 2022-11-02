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

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TUGraz.VectoCore.Utils
{
	[Flags]
	public enum XmlDocumentType
	{
		DeclarationJobData = 1 << 1,
		//PrimaryVehicleBusOutputData = 1 << 2,
		DeclarationComponentData = 1 << 3,
		EngineeringJobData = 1 << 4,
		EngineeringComponentData = 1 << 5,
		ManufacturerReport = 1 << 6,
		CustomerReport = 1 << 7,
		MonitoringReport = 1 << 8,
		VTPReport = 1 << 9,
		MultistepOutputData = 1 << 10
	}

	public static class XmlDocumentTypeExtensions
	{
		public static string GetName(this XmlDocumentType docType)
		{
			switch (docType) {
				case XmlDocumentType.MultistepOutputData:
					return "Multistep output data";
				default:
					return docType.ToString();
			}
		}
	}


	public static class XMLDefinitions
	{
		//public const string SchemaBaseURL = "file:///E:/QUAM/Workspace/VECTO_quam/VectoCore/VectoCore/Resources/XSD/";
		public const string SCHEMA_BASE_LOCATION = "https://citnet.tech.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/XSD/";
		
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

		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V221 = DECLARATION_NAMESPACE + ":v2.2.1";
		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V23 = DECLARATION_NAMESPACE + ":v2.3";


		public const string DECLARATION_DEFINITIONS_NAMESPACE_URI_V24 = DECLARATION_NAMESPACE + ":v2.4";

		public const string DECLARATION_INPUT_NAMESPACE = "urn:tugraz:ivt:VectoAPI:DeclarationInput";

		public const string DECLARATION_INPUT_NAMESPACE_URI_V10 = DECLARATION_INPUT_NAMESPACE + ":v1.0";

		public const string DECLARATION_INPUT_NAMESPACE_URI_V20 = DECLARATION_INPUT_NAMESPACE + ":v2.0";

		//		public const string DECLARATION_COMPONENT_NAMESPACE_URI_V10 = "urn:tugraz:ivt:VectoAPI:DeclarationComponent:v1.0";

		public const string DECLARATION_PRIMARY_BUS_VEHICLE_NAMESPACE = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:PrimaryVehicleInformation";

		public const string DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE =
			"urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile";

		public const string DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1 =
			DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE + ":v0.1";


		public const string XML_SCHEMA_NAMESPACE = "http://www.w3.org/2001/XMLSchema-instance";


		// mapping of document type + version => supported schema files (+version)
		//private static Dictionary<Tuple<XmlDocumentType, string>, IList<string>> schemaFilenames = new Dictionary<Tuple<XmlDocumentType, string>, IList<string>>();

		private static Dictionary<XmlDocumentType, string> schemaFilenames = new Dictionary<XmlDocumentType, string>() {
			{XmlDocumentType.DeclarationJobData, "VectoDeclarationJob.xsd"},
			//{XmlDocumentType.PrimaryVehicleBusOutputData, "VectoOutputPrimaryVehicleInformation.xsd"},
			{XmlDocumentType.DeclarationComponentData, "VectoDeclarationComponent.xsd"},
			{XmlDocumentType.EngineeringJobData, "VectoEngineeringJob.xsd" },
			{XmlDocumentType.EngineeringComponentData, "VectoEngineeringComponent.xsd" },
			{XmlDocumentType.ManufacturerReport, "VectoOutputManufacturer.xsd" },
			{XmlDocumentType.CustomerReport , "VectoOutputCustomer.xsd"},
			{XmlDocumentType.MonitoringReport , "VectoMonitoring.xsd"},
			{XmlDocumentType.VTPReport , "VTPReport.xsd"},
			{XmlDocumentType.MultistepOutputData, "VectoOutputMultistep.0.1.xsd"}
		};

		public static XNamespace DECLARATION_OUTPUT_PRIMARY_HEAVY_BUS = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:PrimaryVehicleInformation:HeavyBus:v0.1";


		public static string GetSchemaFilename(XmlDocumentType type)
		{
			try {
				return schemaFilenames[type];
			} catch (KeyNotFoundException e) {
				throw new Exception($"Invalid argument '{type}' - only use single flags", e);
			}
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
