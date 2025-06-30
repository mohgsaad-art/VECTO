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
using System.Data;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML
{
	public abstract class AbstractXMLWriter
	{
		//protected const string SchemaLocationBaseUrl = "http://markus.quaritsch.at/VECTO/";
		public const string SchemaLocationBaseUrl = "https://citnet.tech.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/XSD/";

		protected string SchemaVersion; // = "1.0";

		protected XNamespace tns;
		protected XNamespace rootNamespace;
		protected readonly XNamespace di;

		protected const string Creator = "TU Graz, IVT-EM XML Exporter";
		protected readonly string Vendor;

		protected readonly string BasePath;


		protected AbstractXMLWriter(string basePath, string vendor)
		{
			BasePath = basePath;
			Vendor = vendor;

			di = "http://www.w3.org/2000/09/xmldsig#";
		}

		protected XElement CreateTorqueLimits(IVehicleDeclarationInputData vehicle)
		{
			return vehicle.TorqueLimits.Count == 0
				? null
				: new XElement(tns + XMLNames.Vehicle_TorqueLimits,
					vehicle.TorqueLimits
						.OrderBy(x => x.Gear)
						.Select(entry => new XElement(tns + XMLNames.Vehicle_TorqueLimits_Entry,
							new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, entry.Gear),
							new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr, entry.MaxTorque.ToXMLFormat(0))
							)
						)
					);
		}

		protected object[] EmbedDataTable(DataTable table, Dictionary<string, string> mapping, string tagName = "Entry",
			Dictionary<string, uint> precision = null)
		{
			return (from DataRow row in table.Rows
				select
					new XElement(tns + tagName,
						table.Columns.Cast<DataColumn>()
							.Where(c => mapping.ContainsKey(c.ColumnName))
							.Select(c => new XAttribute(mapping[c.ColumnName], 
								row.Field<string>(c).ToDouble().ToXMLFormat(precision?.GetVECTOValueOrDefault(c.ColumnName, 2u) ?? 2u)))))
				.Cast<object>().ToArray();
		}
	}
}