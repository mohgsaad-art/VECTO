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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class ShiftPolygonReader
	{
		public static ShiftPolygon ReadFromFile(string fileName)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading ShiftPolygon: " + ex.Message, ex);
			}
		}

		public static ShiftPolygon Create(DataTable data)
		{
			if (data.Columns.Count != 3) {
				throw new VectoException("ShiftPolygon Data File must contain exactly 3 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("ShiftPolygon must have at least two entries");
			}

			List<ShiftPolygon.ShiftPolygonEntry> entriesDown, entriesUp;
			if (HeaderIsValid(data.Columns)) {
				entriesDown = CreateFromColumnNames(data, Fields.AngularSpeedDown);
				entriesUp = CreateFromColumnNames(data, Fields.AngularSpeedUp);
			} else {
				LoggingObject.Logger<ShiftPolygon>().Warn(
						"ShiftPolygon: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: '{3}'. Falling back to column index",
						Fields.Torque, 
						Fields.AngularSpeedUp, 
						Fields.AngularSpeedDown,
						data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
				entriesDown = CreateFromColumnIndizes(data, 1);
				entriesUp = CreateFromColumnIndizes(data, 2);
			}
			return new ShiftPolygon(entriesDown, entriesUp);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.Torque) && columns.Contains(Fields.AngularSpeedUp) &&
					columns.Contains(Fields.AngularSpeedDown);
		}

		private static List<ShiftPolygon.ShiftPolygonEntry> CreateFromColumnNames(DataTable data, string columnName)
		{
			return (from DataRow row in data.Rows
				select new ShiftPolygon.ShiftPolygonEntry(row.SI<NewtonMeter>(Fields.Torque),
					row.ParseDouble(columnName).RPMtoRad())).ToList();
		}

		private static List<ShiftPolygon.ShiftPolygonEntry> CreateFromColumnIndizes(DataTable data, int column)
		{
			return (from DataRow row in data.Rows
					select new ShiftPolygon.ShiftPolygonEntry(row.SI<NewtonMeter>(0), row.ParseDouble(column).RPMtoRad()))
				.ToList();
		}

		public static class Fields
		{
			public const string Torque = "engine torque";
			public const string AngularSpeedUp = "upshift rpm";
			public const string AngularSpeedDown = "downshift rpm";
		}
	}
}