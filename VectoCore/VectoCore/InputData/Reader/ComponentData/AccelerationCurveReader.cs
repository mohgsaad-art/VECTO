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
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class AccelerationCurveReader
	{
		public static AccelerationCurveData ReadFromStream(Stream stream)
		{
			var data = VectoCSVFile.ReadStream(stream);
			return Create(data);
		}

		public static AccelerationCurveData ReadFromFile(string fileName)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading AccelerationCurve File: " + ex.Message);
			}
		}

		internal static AccelerationCurveData Create(DataTable data)
		{
			if (data.Columns.Count != 3) {
				throw new VectoException("Acceleration Limiting File must consist of 3 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("Acceleration Limiting File must consist of at least two entries.");
			}

			if (HeaderIsValid(data.Columns)) {
				return CreateFromColumnNames(data);
			}
			LoggingObject.Logger<AccelerationCurveData>().Warn(
				"Acceleration Curve: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}. Falling back to column index.",
				Fields.Velocity, 
				Fields.Acceleration,
				Fields.Deceleration,
				data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
			return CreateFromColumnIndizes(data);
		}

		private static AccelerationCurveData CreateFromColumnIndizes(DataTable data)
		{
			return new AccelerationCurveData(data.Rows.Cast<DataRow>()
				.Select(r => new KeyValuePair<MeterPerSecond, AccelerationCurveData.AccelerationEntry>(
					r.ParseDouble(0).KMPHtoMeterPerSecond(),
					new AccelerationCurveData.AccelerationEntry {
						Acceleration = r.ParseDouble(1).SI<MeterPerSquareSecond>(),
						Deceleration = r.ParseDouble(2).SI<MeterPerSquareSecond>()
					}))
				.OrderBy(x => x.Key)
				.ToList());
		}

		private static AccelerationCurveData CreateFromColumnNames(DataTable data)
		{
			return new AccelerationCurveData(
				data.Rows.Cast<DataRow>()
					.Select(r => new KeyValuePair<MeterPerSecond, AccelerationCurveData.AccelerationEntry>(
						r.ParseDouble(Fields.Velocity).KMPHtoMeterPerSecond(),
						new AccelerationCurveData.AccelerationEntry {
							Acceleration = r.ParseDouble(Fields.Acceleration).SI<MeterPerSquareSecond>(),
							Deceleration = r.ParseDouble(Fields.Deceleration).SI<MeterPerSquareSecond>()
						}))
					.OrderBy(x => x.Key)
					.ToList());
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.Velocity) &&
					columns.Contains(Fields.Acceleration) &&
					columns.Contains(Fields.Deceleration);
		}

		public static class Fields
		{
			public const string Velocity = "v";

			public const string Acceleration = "acc";

			public const string Deceleration = "dec";
		}
	}
}