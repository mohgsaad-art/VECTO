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
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	public static class IterationStatistics
	{
		private static readonly ThreadLocal<List<DataEntry>> DataLocal =
			new ThreadLocal<List<DataEntry>>(() => new List<DataEntry>());

		public static List<DataEntry> Data => DataLocal.Value;

		private static readonly ThreadLocal<Stopwatch> TimerLocal = new ThreadLocal<Stopwatch>(Stopwatch.StartNew);

		private static Stopwatch Timer => TimerLocal.Value;

		private static readonly ThreadLocal<Dictionary<string, Dictionary<string, double>>> CurrentLocal =
			new ThreadLocal<Dictionary<string, Dictionary<string, double>>>(
				() => new Dictionary<string, Dictionary<string, double>>());

		private static Dictionary<string, Dictionary<string, double>> Current
		{
			get => CurrentLocal.Value;
			set => CurrentLocal.Value = value;
		}

		[Conditional("VECTOTRACE")]
		public static void Increment<T>(T o, string key, double? value = null)
		{
			var t = typeof(T).Name;
			var c = Current.GetOrAdd(t, _ => new Dictionary<string, double>());
			c[key] = c.GetVECTOValueOrDefault(key, 0) + value ?? 1;
		}

		[Conditional("VECTOTRACE")]
		public static void StartIteration()
		{
			Timer.Restart();
		}

		[Conditional("TRACE"), MethodImpl(MethodImplOptions.Synchronized)]
		public static void FinishIteration(Second absTime)
		{
			Timer.Stop();
			Data.Add(new DataEntry(absTime, Timer.Elapsed, Current));
			Current = new Dictionary<string, Dictionary<string, double>>();
		}

		[Conditional("VECTOTRACE")]
		public static void FinishSimulation(string runName)
		{
			var table = new DataTable();
			table.Columns.Add("Distance", typeof(double));
			table.Columns.Add("Time", typeof(double));
			table.Columns.Add("StepDuration", typeof(double));
			table.Columns.Add("StepIterationCount", typeof(double));
			table.Columns.Add("NumDriverRequests", typeof(double));
			table.Columns.Add("NumAccelActions", typeof(double));
			table.Columns.Add("NumBrakeActions", typeof(double));
			table.Columns.Add("NumCoastActions", typeof(double));
			table.Columns.Add("NumRollActions", typeof(double));
			table.Columns.Add("SearchOPIterations", typeof(double));
			table.Columns.Add("SearchBrakeIterations", typeof(double));
			table.Columns.Add("NumGearboxRequests", typeof(double));
			table.Columns.Add("NumEngineRequests", typeof(double));

			foreach (var entry in Data) {
				var row = table.NewRow();
				row["StepDuration"] = entry.Duration.TotalMilliseconds;

				if (entry.Values.ContainsKey("DistanceRun")) {
					row["Distance"] = entry.Values["DistanceRun"].GetValueOrDBNull("Distance");
					row["Time"] = entry.Values["DistanceRun"].GetValueOrDBNull("Time");
					row["StepIterationCount"] = entry.Values["DistanceRun"].GetValueOrDBNull("Iterations");
				}
				if (entry.Values.ContainsKey("Driver")) {
					row["NumDriverRequests"] = entry.Values["Driver"].GetValueOrDBNull("Requests");
					row["NumAccelActions"] = entry.Values["Driver"].GetValueOrDBNull("Accelerate");
					row["NumBrakeActions"] = entry.Values["Driver"].GetValueOrDBNull("Brake");
					row["NumCoastActions"] = entry.Values["Driver"].GetValueOrDBNull("Coast");
					row["NumRollActions"] = entry.Values["Driver"].GetValueOrDBNull("Roll");
					row["SearchOPIterations"] = entry.Values["Driver"].GetValueOrDBNull("SearchOperatingPoint");
					row["SearchBrakeIterations"] = entry.Values["Driver"].GetValueOrDBNull("SearchBrakingPower");
				}
				if (entry.Values.ContainsKey("Gearbox")) {
					row["NumGearboxRequests"] = entry.Values["Gearbox"].GetValueOrDBNull("Requests");
				}
				if (entry.Values.ContainsKey("CombustionEngine")) {
					row["NumEngineRequests"] = entry.Values["CombustionEngine"].GetValueOrDBNull("Requests");
				}
				table.Rows.Add(row);
			}

			using (var writer = new StreamWriter("statistics_" + runName + ".csv"))
			{
                VectoCSVFile.Write(writer, table);
            }
		}

		public sealed class DataEntry
		{
			public readonly TimeSpan Duration;
			public readonly Second Time;
			public readonly Dictionary<string, Dictionary<string, double>> Values;

			public DataEntry(Second time, TimeSpan duration, Dictionary<string, Dictionary<string, double>> values)
			{
				Time = time;
				Duration = duration;
				Values = values;
			}
		}
	}
}