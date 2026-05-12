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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Utils
{
	public static class XMLHelper
	{
		public static XmlDocumentType? GetDocumentTypeFromRootElement(string rootElement)
		{
			switch (rootElement) {
				case "VectoInputDeclaration": return XmlDocumentType.DeclarationJobData;
				case "VectoInputEngineering": return XmlDocumentType.EngineeringJobData;
				case "VectoComponentEngineering": return XmlDocumentType.EngineeringComponentData;
				//case "VectoOutputPrimaryVehicle": return XmlDocumentType.PrimaryVehicleBusOutputData;
				case "VectoOutputMultistep": return XmlDocumentType.MultistepOutputData;
			}

			return null;
		}

		public static XmlDocumentType? GetDocumentTypeFromFile(string filePath)
		{
			var xElement = new System.Xml.XmlDocument();
			xElement.Load(filePath);
			return XMLHelper.GetDocumentTypeFromRootElement(xElement?.DocumentElement?.LocalName);

        }


        //internal static string GetSchemaVersion(XmlSchemaType type)
        //{
        //	return GetVersionFromNamespaceUri(type.QualifiedName.Namespace);
        //}

        //public static string GetSchemaVersion(XmlElement node)
        //{
        //	return GetVersionFromNamespaceUri(node.NamespaceURI);
        //}

        public static string GetVersionFromNamespaceUri(this XNamespace namespaceUri)
		{
			const string versionPrefix = "v";
			return namespaceUri.NamespaceName.Split(':').Last(x => x.StartsWith(versionPrefix))
				.Replace(versionPrefix, string.Empty);
		}


		public static object[] ValueAsUnit(this Kilogram mass, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "t": return GetValueAsUnit(mass.ConvertToTon(), unit, decimals);
				case "kg": return GetValueAsUnit(mass.Value(), unit, decimals);
			}

			throw new NotImplementedException($"unknown unit '{unit}'");
		}

		public static object[] ValueAsUnit(this Watt power, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "kW": return GetValueAsUnit(power?.ConvertToKiloWatt(), unit, decimals);
				case "W": return GetValueAsUnit(power?.Value(), unit, decimals);
			}

			throw new NotImplementedException($"unknown unit '{unit}'");
		}

		public static object[] ValueAsUnit(this WattSecond energy, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "kWh":
					return GetValueAsUnit(energy?.ConvertToKiloWattHour(), unit, decimals);
				case "Wh":
					return GetValueAsUnit(energy?.ConvertToWattHour(), unit, decimals);
			}

			throw new NotImplementedException($"unknown unit '{unit}'");
		}

		public static object[] ValueAsUnit(KilogramPerWattSecond kpws, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "mg/kWh":
					return GetValueAsUnit(kpws?.ConvertToMilliGramPerKiloWattHour(), unit, decimals);
				case "g/kWh":
					return GetValueAsUnit(kpws?.ConvertToGramPerKiloWattHour(), unit, decimals);
			}

			throw new NotImplementedException(string.Format("unknown unit '{0}'", unit));
		}

		public static object[] ValueAsUnit(PerWattSecond pws, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "#/kWh":
					return GetValueAsUnit(pws?.ConvertToPerKiloWattHour(), unit, decimals);
			}

			throw new NotImplementedException(string.Format("unknown unit '{0}'", unit));
		}
		
		public static object[] ValueAsUnit(this AmpereSecond capacity, string unit, uint? decimals = 0)
		{
			switch (unit)
			{
				case "As": return GetValueAsUnit(capacity.Value(), unit, decimals);
				case "Ah": return GetValueAsUnit(capacity?.AsAmpHour, unit, decimals);
			}

			throw new NotImplementedException($"unknown unit '{unit}'");
		}



		public static object[] ValueAsUnit(this CubicMeter volume, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "l": 
				case "ltr": 
					return GetValueAsUnit(volume.ConvertToCubicDeziMeter(), unit, decimals);
				case "ccm":
					return GetValueAsUnit(volume.ConvertToCubicCentiMeter(), unit, decimals);
				case "m3":
					return GetValueAsUnit(volume.Value(), unit, decimals);
			}

			throw new NotImplementedException($"unknown unit '{unit}'");
		}

		public static object[] ValueAsUnit(this PerSecond angSpeed, string unit, uint? decimals = 0)
		{
			switch (unit) {
				case "rpm": return GetValueAsUnit(angSpeed.ConvertToRoundsPerMinute(), unit, decimals);
			}

			throw new NotImplementedException($"unknown unit '{unit}'");
		}


		public static object[] ValueAsUnit(this MeterPerSecond speed, string unit, uint? decimals)
		{
			switch (unit) {
				case "km/h": return GetValueAsUnit(speed.ConvertToKiloMeterPerHour(), unit, decimals);
			}

			throw new NotImplementedException($"unknown unit '{unit}'");
		}

		public static object[] ValueAsUnit(this MeterPerSquareSecond acc, string unit, uint? decimals)
		{
			switch (unit) {
				case "m/s²": return GetValueAsUnit(acc.Value(), unit, decimals);
			}

			throw new NotImplementedException($"unknown unit '{unit}'");
		}

		public static object[] ValueAsUnit(this Meter m, string unit, uint? decimals)
		{
			switch (unit) {
				case "m": return GetValueAsUnit(m.Value(), unit, decimals);
				case "km": return GetValueAsUnit(m.ConvertToKiloMeter(), unit, decimals);
			}

			throw new NotImplementedException($"unknown unit '{unit}'");
		}

		public static object[] ValueAsUnit(this double value, string unit, uint? decimals)
		{
			switch (unit) {
				case "%": return GetValueAsUnit(value * 100, unit, decimals);
				default: return GetValueAsUnit(value, unit, decimals);
			}
		}

		public static object[] ValueAsUnit(this ConvertedSI value, uint? significant = null, uint? decimals = null)
		{
			//return GetValueAsUnit, value.Units, decimals);
			return new object[] {
				new XAttribute(XMLNames.Report_Results_Unit_Attr, value.Units),
				value.Value.ToMinSignificantDigits(significant, decimals)
			};
		}

		public static object[] ValueAsUnit(this ConvertedSI value, uint decimals)
		{
			return GetValueAsUnit(value, value.Units, decimals);
		}

        private static object[] GetValueAsUnit(this double? value, string unit, uint? decimals)
		{
			if (value == null) {
				return new object[0];
			}

			return new object[] {
				new XAttribute(XMLNames.Report_Results_Unit_Attr, unit),
				value.Value.ToXMLFormat(decimals)
			};
		}


		public static string ToXmlStr(FuelData.Entry fuelData)
		{
			var prefix = "";
			if (fuelData.FuelType == FuelType.NGPI || fuelData.FuelType == FuelType.NGCI) {
				if (fuelData.TankSystem == null) {
					throw new VectoException("No TankSystem specified!", ErrorCodes.FUEL_MISSING_TANKSYSTEM);
				}

				prefix = fuelData.TankSystem.Value == TankSystem.Liquefied ? "L" : "C";
			}

			return prefix + fuelData.FuelType.ToXMLFormat();
		}

		public static string ToXmlFormat(this DateTime dateTime)
		{
			return XmlConvert.ToString(dateTime, XmlDateTimeSerializationMode.Utc);
		}

		public static string QueryLocalName(string nodeName)
		{
			return $".//*[local-name()='{nodeName}']";
		}

		public static string QueryLocalName(params string[] nodePath)
		{
			return "./" + string.Join("/",
				nodePath.Where(x => x != null).Select(x => $"/*[local-name()='{x}']").ToArray());
		}


		public static TableData ReadTableData(Dictionary<string, string> attributeMapping, XmlNodeList entryNodes)
		{
			var table = new TableData();
			var entries = Shim<XmlNode>(entryNodes).ToArray();
			foreach (var mapping in attributeMapping) {
				if (entries.All(x => x.Attributes?.GetNamedItem(mapping.Value) != null)) {
					table.Columns.Add(mapping.Key);
				}
			}

			foreach (var entry in entries) {
				var row = table.NewRow();
				foreach (var mapping in attributeMapping) {
					if (entry.Attributes?.GetNamedItem(mapping.Value) != null) {
						row[mapping.Key] = entry.Attributes?.GetNamedItem(mapping.Value).InnerText;
					}
				}

				table.Rows.Add(row);
			}

			return table;
		}

		public static TableData ReadEntriesOrResource(XmlNode baseNode, string basePath, string baseElement,
			string entryElement, Dictionary<string, string> mapping)
		{
			var entries = baseNode.SelectNodes(
				QueryLocalName(baseElement, entryElement));
			if (entries != null && entries.Count > 0) {
				return ReadTableData(mapping, entries);
			}

			return ReadCSVResource(baseNode, baseElement, basePath);
		}

		public static TableData ReadCSVResource(XmlNode baseNode, string xmlElement, string basePath)
		{
			var resourceNode = baseNode.SelectSingleNode(
				QueryLocalName(xmlElement) + ExtCSVResourceQuery);
			var filename = string.Empty;
			if (resourceNode != null) {
				filename = resourceNode.Attributes?.GetNamedItem(XMLNames.ExtResource_File_Attr).InnerText;
				if (filename == null) {
					throw new VectoException($"{xmlElement} No filename provided!", ErrorCodes.XML_MISSING_ATTRIBUTE);
				}

				if (basePath == null) {
					throw new VectoException("cannot read referenced file - job passed as stream!", ErrorCodes.FILE_FAILED_TO_READ);
				}

				var fullFilename = Path.Combine(basePath, filename);
				if (!File.Exists(fullFilename)) {
					throw new FileNotFoundException($"{xmlElement} file not found: {filename}");
				}

				return VectoCSVFile.Read(fullFilename);
			}

			return null; // new TableData(Path.Combine(basePath ?? "", filename), DataSourceType.Missing);
		}

		private static string ExtCSVResourceQuery =>
			$"/*[local-name()='{XMLNames.ExternalResource}' and @{XMLNames.ExtResource_Type_Attr}='{XMLNames.ExtResource_Type_Value_CSV}']";

		private static IEnumerable<T> Shim<T>(XmlNodeList nodes)
		{
			foreach (var node in nodes) {
				yield return (T)node;
			}
		}


		public static string CombineNamespace(XNamespace xmlNamespace, string type)
		{
			return string.Join(":", xmlNamespace.NamespaceName, type);
		}

		public static string GetXsdType(XmlSchemaType schemaInfoSchemaType)
		{
			return string.Join(":", schemaInfoSchemaType.QualifiedName.Namespace,
				schemaInfoSchemaType.QualifiedName.Name);
		}

		public static double GetVersion(XmlNode node)
		{
			const string versionPrefix = "v";
			var namesp = node.SchemaInfo.SchemaType.QualifiedName.Namespace;
			var versionPart = namesp.Split(':').Last(x => x.StartsWith(versionPrefix))
				.Replace(versionPrefix, string.Empty);
			if (versionPart.Split('.').Length > 2) {
				versionPart = string.Join(".", versionPart.Split('.').Take(2));
			}

			return versionPart.ToDouble();
		}

		public static XElement CreateDummySig(XNamespace di)
		{
			return new XElement(di + XMLNames.DI_Signature_Reference,
				new XElement(di + XMLNames.DI_Signature_Reference_DigestMethod,
					new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "null")),
				new XElement(di + XMLNames.DI_Signature_Reference_DigestValue, "NOT AVAILABLE")
			);
		}

		public static bool IsAnyNull(this IComponentInputData inputData, params object[] checkForNull)
		{
			foreach (var o in checkForNull) {
				if (o == null) {
					return true;
				}
			}

			return false;
		}


		public static void AddIfContentNotNull(this XElement xElement, XElement xElementToAdd)
		{
			if (!string.IsNullOrEmpty(xElementToAdd.Value)){
				xElement.Add(xElementToAdd);
			}
		}

		public static void AddIfContentNotNull(this XElement xElement, params XElement[] xElementsToAdd)
		{
			foreach (var element in xElementsToAdd) {
				xElement.AddIfContentNotNull(element);
			}
		}

		public static XElement WithXName(this XElement xElement, XName xName)
		{
			xElement.Name = xName;
			return xElement;
		}

		public static XElement GetApplicationInfo(XNamespace ns)
		{
			var versionNumber = VectoSimulationCore.VersionNumber;
#if CERTIFICATION_RELEASE
			// add nothing to version number
#else
			versionNumber += " !!NOT FOR CERTIFICATION!!";
#endif
			return new XElement(ns + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(ns + XMLNames.Report_ApplicationInfo_SimulationToolVersion, versionNumber),
				new XElement(ns + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}

		public static string GetGUID()
		{
			return Guid.NewGuid().ToString("n").Substring(0, 20);
		}


		public static XmlSchemaType GetSchemaType(XmlNode componentNode)
		{
			var dataNode =
				componentNode?.SelectSingleNode($"./*[local-name()='{XMLNames.ComponentDataWrapper}']");
			
			var type = (dataNode ?? componentNode)?.SchemaInfo.SchemaType;
			return type;
        }
		
		public static XmlDocument SecureLoadXML(string filePath)
		{
            var document = new XmlDocument();
            MemoryStream stream = new MemoryStream(File.ReadAllBytes(filePath));
			
			XmlReaderSettings settings = new XmlReaderSettings() { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null };
            
			document.Load(XmlReader.Create(stream, settings));

			stream.Close();
			stream.Dispose();

			return document;
		}

	}
}