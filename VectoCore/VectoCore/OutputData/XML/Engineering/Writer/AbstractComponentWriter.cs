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
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	public abstract class AbstractXMLWriter
	{
		public string XMLDataType { get; }

		protected AbstractXMLWriter(string xmlType)
		{
			XMLDataType = xmlType;
		}

		protected XElement[] GetDefaultComponentElements(IComponentInputData data)
		{
			var tns = Writer.RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
			return new[] {
				new XElement(tns + XMLNames.Component_Manufacturer, string.IsNullOrWhiteSpace(data.Manufacturer) ? "N.A." : data.Manufacturer),
				new XElement(tns + XMLNames.Component_Model,  string.IsNullOrWhiteSpace(data.Model) ? "N.A." : data.Model),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Component_AppVersion, "VECTO " + VectoSimulationCore.VersionNumber),
			};
		}

		protected XElement[] GetDefaultComponentElements(IVehicleEngineeringInputData data)
		{
			var tns = Writer.RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
			return new[] {
				new XElement(tns + XMLNames.Component_Manufacturer, data.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, data.ManufacturerAddress),
				new XElement(tns + XMLNames.Component_Model, data.Model),
				new XElement(tns + XMLNames.Vehicle_VIN, data.VIN),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
			};
		}

		protected object[] EmbedDataTable(
			DataTable table, Dictionary<string, string> mapping, string tagName = "Entry",
			Dictionary<string, uint> precision = null)
		{
			var tns = Writer.RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);

			return (from DataRow row in table.Rows
					select
						new XElement(
							tns + tagName,
							table.Columns.Cast<DataColumn>()
								.Where(c => mapping.ContainsKey(c.ColumnName))
								.Select(c => new XAttribute(mapping[c.ColumnName], 
									row.Field<string>(c).ToDouble().ToXMLFormat(precision?.GetVECTOValueOrDefault(c.ColumnName, 2u) ?? 2u)))))
				.Cast<object>().ToArray();
		}

		protected object[] ExtCSVResource(DataTable data, string filename)
		{
			var tns = Writer.RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
			VectoCSVFile.Write(filename, data);
			return new object[] {
				new XElement(
					tns + XMLNames.ExternalResource,
					new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
					new XAttribute(XMLNames.ExtResource_File_Attr, filename))
			};
		}

		protected virtual XElement ExtComponent(XDocument document, string component, string filename)
		{
			var writer = new XmlTextWriter(filename, Encoding.UTF8) {
				Formatting = Formatting.Indented
			};
			document.WriteTo(writer);
			writer.Flush();
			writer.Close();
			var tns = Writer.RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
			var retVal = new XElement(
				tns + XMLNames.ExternalResource,
				new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_XML),
				new XAttribute(XMLNames.ExtResource_Component_Attr, component),
				new XAttribute(XMLNames.ExtResource_File_Attr, filename));
			return retVal;
		}

		public virtual IXMLEngineeringWriter Writer { protected get; set; }

		public virtual object[] WriteXML(IAxleEngineeringInputData axle, int idx, Meter dynamicTyreRadius)
		{
			return null;
		}

		public virtual object[] WriteXML(ITransmissionInputData inputData, int i)
		{
			return null;
		}

		public virtual object[] WriteXML(IDriverModelData inputData)
		{
			return null;
		}

		public virtual object[] WriteXML(IComponentInputData inputData)
		{
			return null;
		}

		
		public virtual object[] WriteXML(IEngineeringInputDataProvider inputData)
		{
			return null;
		}

		public virtual object[] WriteXML(IAuxiliaryEngineeringInputData inputData)
		{
			return null;
		}

		public abstract XNamespace ComponentDataNamespace { get; }

		public virtual XAttribute GetXMLTypeAttribute()
		{
			var xsns = Writer.RegisterNamespace(XMLDefinitions.XML_SCHEMA_NAMESPACE);
			return new XAttribute(
				xsns + "type", $"{Writer.GetNSPrefix(ComponentDataNamespace.NamespaceName)}:{XMLDataType}");
		}

		public virtual object[] WriteXML(IAdvancedDriverAssistantSystemsEngineering inputData)
		{
			return null;
		}
	}


	public abstract class AbstractComponentWriter<T> : AbstractXMLWriter where T : class, IComponentInputData
	{
		protected AbstractComponentWriter(string xmlType) : base(xmlType) { }

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IComponentInputData inputData)
		{
			var componentData = inputData as T;
			if (Writer == null) {
				throw new VectoException("no main writer set!");
			}

			if (componentData == null) {
				throw new VectoException("input Data is not of type IEngineEngineeringInputData");
			}

			return DoWriteXML(componentData);
		}

		#endregion

		protected abstract object[] DoWriteXML(T inputData);
	}
}
