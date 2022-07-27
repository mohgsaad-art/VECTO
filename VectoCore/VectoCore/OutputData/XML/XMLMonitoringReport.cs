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
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLMonitoringReport
	{
		public const string SCHEMA_VERSION_STRING = "0.7";

		public const string CURRENT_SCHEMA_VERSION = "0.7.2";

		private XMLManufacturerReport _manufacturerReport;

		protected XNamespace tns;
		protected XNamespace di;
		private XElement _additionalFields;


		public XMLMonitoringReport(XMLManufacturerReport manufacturerReport)
		{
			di = "http://www.w3.org/2000/09/xmldsig#";
			tns = "urn:tugraz:ivt:VectoAPI:MonitoringOutput:v" + CURRENT_SCHEMA_VERSION;
			_manufacturerReport = manufacturerReport;
		}

		public XDocument Report
		{
			get {
				var mrf = _manufacturerReport.Report;
				if (mrf == null) {
					return null;
				}

				var retVal = GenerateReport();

				retVal.Root?.Add(
					new XElement(
						tns + "ManufacturerRecord",
						GetManufacturerData(mrf)),
					_additionalFields
				);
				return retVal;
			}
		}

		
		private object[] GetManufacturerData(XDocument mrf)
		{
			return mrf.Root?.XPathSelectElements("./*").ToArray<object>();
		}

		private XDocument GenerateReport()
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var retVal = new XDocument();

			//retVal.Add(
			//	new XProcessingInstruction(
			//		"xml-stylesheet", "href=\"https://citnet.tech.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/CSS/VectoReports.css\""));
			retVal.Add(
				new XElement(
					tns + "VectoMonitoring",
					new XAttribute("schemaVersion", SCHEMA_VERSION_STRING),
					new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
					new XAttribute("xmlns", tns),
					new XAttribute(XNamespace.Xmlns + "di", di),
					new XAttribute(
						xsi + "schemaLocation",
						string.Format(
							"{0} {1}VectoMonitoring.{2}.xsd", tns, AbstractXMLWriter.SchemaLocationBaseUrl, CURRENT_SCHEMA_VERSION))
				)
			);
			return retVal;
		}

		public void Initialize(VectoRunData modelData)
		{
			var numAxles = modelData.VehicleData.AxleData?.Count(x => x.AxleType != AxleType.Trailer) ?? 0;
			var axleData = new object[numAxles];
			for (var i = 0; i < axleData.Length; i++) {
				axleData[i] = new XElement(tns + "Axle",
					new XAttribute("axleNumber", i+1),
					new XElement(tns + "Tyre", GetStandardFields(string.Format("TYRE_{0}", i+1))
					));
			}

			var components = new object[0];
			if (!modelData.Exempted) {
				components = new object[] {
					new XElement(
						tns + "Engine",
						new XElement(
							tns + "WHTC",
							new XElement(tns + "CO2", XMLHelper.ValueAsUnit(double.NaN, "g/kWh", 0)),
							new XElement(tns + "FuelConsumption", XMLHelper.ValueAsUnit(double.NaN, "g/kWh", 0))
						),
						new XElement(
							tns + "WHSC",
							new XElement(tns + "CO2", XMLHelper.ValueAsUnit(double.NaN, "g/kWh", 0)),
							new XElement(tns + "FuelConsumption", XMLHelper.ValueAsUnit(double.NaN, "g/kWh", 0))
						)
					),
					new XElement(tns + "Gearbox", GetStandardFields("GEARBOX")),
					new XElement(tns + "Axlegear", GetStandardFields("AXLEGEAR")),
					new XElement(tns + "AxleWheels", axleData),
				};
			}
			_additionalFields = new XElement(
				tns + "AdditionalData",
				new XElement(tns + "Vehicle",
							new XElement(tns + "Make", "##VEHICLE_MAKE##")),
				components,
				new XElement(tns + "AdvancedReducingTechnologies", new XComment(GetReducingTechnologiesExample())),
				new XElement(tns + "VectoLicenseNbr", "##VECTO_LICENSE_NUMBER##")
			);
		}

		private object[] GetStandardFields(string prefix)
		{
			return new[] {
				new XElement(tns + "Manufacturer", string.Format("##{0}_MANUFACTURER##", prefix)),
				new XElement(tns + "ManufacturerAddress", string.Format("##{0}_MANUFACTURERADDRESS##", prefix)),
				new XElement(tns + "Make", string.Format("##{0}_MAKE##", prefix))
			};
		}

		private  string GetReducingTechnologiesExample()
		{
			var categories = new[] {
				"advanced aerodynamic measures",
				"advanced rolling resistance measures",
				"advanced drivetrain technologies",
				"advanced engine technologies",
				"advanced auxiliary technologies",
				"additional ADAS technologies",
				"advanced powertrain integration and hybridisation",
				"other"
			};
			var retVal = new object[categories.Length];
			//var tmp = new XElement(tns + "foo");
			for (var i = 0; i < retVal.Length; i++) { 
				retVal[i] = new XElement("Entry",
					new XAttribute("category", categories[i]),
					"##TECHNOLOGY_BRAND_NAME##"
					);
			}

			return Environment.NewLine + string.Join(Environment.NewLine, retVal.Select(x => x.ToString())) + Environment.NewLine;
		}
	}
}
