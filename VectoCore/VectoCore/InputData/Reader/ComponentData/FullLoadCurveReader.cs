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
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class FullLoadCurveReader
	{
		public static EngineFullLoadCurve ReadFromFile(string fileName, bool declarationMode = false)
		{
			try {
				var data = VectoCSVFile.Read(fileName);
				return Create(data, declarationMode);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading FullLoadCurve File: " + ex.Message, ex);
			}
		}

		public static EngineFullLoadCurve Create(DataTable data, bool declarationMode = false)
		{
			if (data.Columns.Count < 3) {
				throw new VectoException("Engine FullLoadCurve Data File must consist of at least 3 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException(
					"FullLoadCurve must consist of at least two lines with numeric values (below file header)");
			}

			List<EngineFullLoadCurve.FullLoadCurveEntry> entriesFld;
			if (HeaderIsValid(data.Columns)) {
				entriesFld = CreateFromColumnNames(data);
			} else {
				LoggingObject.Logger<EngineFullLoadCurve>().Warn(
					"FullLoadCurve: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: '{3}'. Falling back to column index.",
					Fields.EngineSpeed, 
					Fields.TorqueFullLoad,
					Fields.TorqueDrag, 
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());

				entriesFld = CreateFromColumnIndizes(data);
			}

			LookupData<PerSecond, PT1.PT1Result> tmp;
			if (declarationMode) {
				tmp = new PT1();
			} else {
				tmp = data.Columns.Count > 3 ? new PT1(data) : new PT1();
			}
			entriesFld.Sort((entry1, entry2) => entry1.EngineSpeed.Value().CompareTo(entry2.EngineSpeed.Value()));
			var duplicates = entriesFld.Select(x => x.EngineSpeed.AsRPM).GroupBy(x => x).Where(g => g.Count() > 1)
										.Select(g => g.Key).ToList();
			if (duplicates.Count > 0) {
				throw new VectoException(
					"Error reading full-load curve: multiple entries for engine speeds {0}", duplicates.Join());
			}
			return new EngineFullLoadCurve(entriesFld, tmp);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.EngineSpeed)
					&& columns.Contains(Fields.TorqueFullLoad)
					&& columns.Contains(Fields.TorqueDrag);
		}

		private static List<EngineFullLoadCurve.FullLoadCurveEntry> CreateFromColumnNames(DataTable data)
		{
			return (from DataRow row in data.Rows
				select new EngineFullLoadCurve.FullLoadCurveEntry {
					EngineSpeed = row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
					TorqueFullLoad = row.ParseDouble(Fields.TorqueFullLoad).SI<NewtonMeter>(),
					TorqueDrag = row.ParseDouble(Fields.TorqueDrag).SI<NewtonMeter>()
				}).ToList();
		}

		private static List<EngineFullLoadCurve.FullLoadCurveEntry> CreateFromColumnIndizes(DataTable data)
		{
			return (from DataRow row in data.Rows
				select new EngineFullLoadCurve.FullLoadCurveEntry {
					EngineSpeed = row.ParseDouble(0).RPMtoRad(),
					TorqueFullLoad = row.ParseDouble(1).SI<NewtonMeter>(),
					TorqueDrag = row.ParseDouble(2).SI<NewtonMeter>()
				}).ToList();
		}

		public static class Fields
		{
			/// <summary>
			/// [rpm] engine speed
			/// </summary>
			public const string EngineSpeed = "engine speed";

			/// <summary>
			/// [Nm] full load torque
			/// </summary>
			public const string TorqueFullLoad = "full load torque";

			/// <summary>
			/// [Nm] motoring torque
			/// </summary>
			public const string TorqueDrag = "motoring torque";
		}
	}
}