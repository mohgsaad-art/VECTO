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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering
{
	internal class XMLEngineeringWriter : IXMLEngineeringWriter
	{
		protected readonly Dictionary<string, string> NamespaceLocationMap = new Dictionary<string, string>() {
			{ XMLDefinitions.ENGINEERING_INPUT_NAMESPACE_URI_V10, XMLDefinitions.SCHEMA_BASE_LOCATION + "VectoEngineeringInput.1.0.xsd" },
			{ XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10, XMLDefinitions.SCHEMA_BASE_LOCATION + "VectoEngineeringDefinitions.1.0.xsd"},
			{ XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST, XMLDefinitions.SCHEMA_BASE_LOCATION + "VectoEngineeringDefinitionsTEST.1.1.xsd"},
		};

		protected Dictionary<string, XNamespace> _namespaces = new Dictionary<string, XNamespace>();



		[Inject]
		public IEngineeringWriterFactory WriterFactory { protected get; set; }

		public XMLEngineeringWriter()
		{
			RegisterNamespace(XMLDefinitions.XML_SCHEMA_NAMESPACE);
			RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
			_namespaces.Add("", XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);

			Configuration = new WriterConfiguration { SingleFile = true, BasePath = "" };
		}

		public XDocument Write(IInputDataProvider inputData)
		{
			return Write(inputData as IEngineeringInputDataProvider);
		}

		public XDocument Write(IEngineeringInputDataProvider inputData)
		{
			var version = "1.0";
			var jobWriter = WriterFactory.CreateJobWriter(version, this, inputData);

			var retVal = new XDocument();
			var v10 = RegisterNamespace(XMLDefinitions.ENGINEERING_INPUT_NAMESPACE_URI_V10);

			var jobXML = jobWriter.WriteXML();
			retVal.Add(
				new XElement(
					v10 + XMLNames.VectoInputEngineering,
					GetNamespaceAttributes(),
					GetSchemaLocations(),
					jobXML
				)
			);
			return retVal;
		}

		public XDocument WriteComponent<T>(T componentInputData) where T : class, IComponentInputData
		{
			var retVal = new XDocument();

			var v10Inp = RegisterNamespace(XMLDefinitions.ENGINEERING_INPUT_NAMESPACE_URI_V10);
			var v10Def = RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
			var xsns = RegisterNamespace(XMLDefinitions.XML_SCHEMA_NAMESPACE);
			var writer = WriterFactory.GetWriter(componentInputData, this);

			// wrap every component except the vehicle in a Data element
			var componentXML = writer?.WriteXML(componentInputData);
			if (typeof(T) != typeof(IVehicleEngineeringInputData)) {
				componentXML = new object[] {new XElement(
					v10Def + XMLNames.ComponentDataWrapper,
					componentXML)};
			}
			retVal.Add(
				new XElement(
					v10Inp + XMLNames.VectoComponentEngineering,
					new XAttribute(
						xsns + "type", $"{GetNSPrefix(v10Def.NamespaceName)}:VectoComponentEngineeringType"),
					GetNamespaceAttributes(),
					GetSchemaLocations(),
					new XElement(
						v10Def + XMLWriterMapping.GetXMLTag(typeof(T)),
						 componentXML )
				)
			);
			return retVal;
		}

		private XAttribute GetSchemaLocations()
		{
			var xsns = RegisterNamespace(XMLDefinitions.XML_SCHEMA_NAMESPACE);
			return new XAttribute(xsns + "schemaLocation", 
				NamespaceLocationMap.Where(x => _namespaces.ContainsKey(x.Key)).Select(x => $"{x.Key} {x.Value}").Join(" "));
		}

		public WriterConfiguration Configuration { get; set; }

		public string GetFilename<T>(T componentData, string suffix = null) where T : IComponentInputData
		{
			var formatString = XMLWriterMapping.GetFilenameTemplate(typeof(T));

			return
				Path.Combine(
					Configuration.BasePath,
					RemoveInvalidFileCharacters(
						string.Format(
							formatString, componentData.Manufacturer ?? "N.A.", componentData.Model ?? "N.A.",
							suffix != null ? "_" + suffix : null)));
		}


		public XNamespace RegisterNamespace(string namespaceUri) => _namespaces.GetOrAdd(namespaceUri, XNamespace.Get);

		#region Implementation of IXMLEngineeringWriter

		#endregion

		protected virtual object[] GetNamespaceAttributes()
		{
			return _namespaces
				.Select(x => new XAttribute(x.Key == "" ? "xmlns" : XNamespace.Xmlns + GetNSPrefix(x.Key), x.Value)).Cast<object>()
				.ToArray();
		}

		public string GetNSPrefix(string xmlns)
		{
			if (xmlns == XMLDefinitions.XML_SCHEMA_NAMESPACE) {
				return "xsi";
			}
			if (xmlns == XMLDefinitions.ENGINEERING_INPUT_NAMESPACE_URI_V10) {
				return "tns";
			}

			if (xmlns.StartsWith("urn:tugraz:ivt:VectoAPI")) {
				var parts = xmlns.Split(':');
				return parts.Last();
			}

			throw new VectoException("unknown namespace!");
		}

		public string GetComponentFilename(IComponentInputData component)
		{
			var formatString = component is IEngineEngineeringInputData ? "ENG_{0}.xml" : "{0}";
			return RemoveInvalidFileCharacters(string.Format(formatString, component.Model));
		}

		public string RemoveInvalidFileCharacters(string filename)
		{
			var regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			var r = new Regex($"[{Regex.Escape(regexSearch)}]");
			return r.Replace(filename, "");
		}
	}
}
