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
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class FuelConsumptionMapReader
	{
		public static FuelConsumptionMap ReadFromFile(string fileName)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data);
			} catch (Exception e) {
				throw new VectoException($"File {fileName}: {e.Message}", e);
			}
		}

		public static FuelConsumptionMap ReadFromStream(Stream stream)
		{
			try {
				var data = VectoCSVFile.ReadStream(stream);
				return Create(data);
			} catch (Exception e) {
				throw new VectoException($"Failed reading stream: {e.Message}", e);
			}
		}

		public static FuelConsumptionMap Create(DataTable data)
		{
			var headerValid = HeaderIsValid(data.Columns);
			if (!headerValid) {
				LoggingObject.Logger<FuelConsumptionMap>().Warn(
					"FuelConsumptionMap: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}. Falling back to column index.",
					Fields.EngineSpeed, 
					Fields.Torque, 
					Fields.FuelConsumption,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
			}
			var delaunayMap = new DelaunayMap("FuelConsumptionMap");

			foreach (DataRow row in data.Rows) {
				try {
					var entry = headerValid ? CreateFromColumNames(row) : CreateFromColumnIndizes(row);
					delaunayMap.AddPoint(entry.Torque.Value(),
						(headerValid ? row.ParseDouble(Fields.EngineSpeed) : row.ParseDouble(0)).RPMtoRad().Value(),
						entry.FuelConsumption.Value());
				} catch (Exception e) {
					throw new VectoException($"FuelConsumptionMap - Line {data.Rows.IndexOf(row)}: {e.Message}", e);
				}
			}

			delaunayMap.Triangulate();
			return new FuelConsumptionMap(delaunayMap);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.EngineSpeed) && columns.Contains(Fields.Torque) &&
					columns.Contains(Fields.FuelConsumption);
		}

		private static FuelConsumptionMap.Entry CreateFromColumnIndizes(DataRow row)
		{
			return new FuelConsumptionMap.Entry(
				engineSpeed: row.ParseDouble(0).RPMtoRad(),
				torque: row.ParseDouble(1).SI<NewtonMeter>(),
				fuelConsumption: row.ParseDouble(2).SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>()
				);
		}

		private static FuelConsumptionMap.Entry CreateFromColumNames(DataRow row)
		{
			return new FuelConsumptionMap.Entry(
				engineSpeed: row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
				torque: row.SI<NewtonMeter>(Fields.Torque),
				fuelConsumption: row.ParseDouble(Fields.FuelConsumption).SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>()
				);
		}

		public static class Fields
		{
			/// <summary>
			/// [rpm]
			/// </summary>
			public const string EngineSpeed = "engine speed";

			/// <summary>
			/// [Nm]
			/// </summary>
			public const string Torque = "torque";

			/// <summary>
			/// [g/h]
			/// </summary>
			public const string FuelConsumption = "fuel consumption";
		}
	}
}