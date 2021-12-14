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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;

namespace TUGraz.VectoCore.Utils
{
	public static class SearchAlgorithm
	{
		/// <summary>
		/// Applies a numerical search over the evaluateFunction until the criterion reaches approximately 0.
		/// <code>
		/// SearchAlgorithm.Search(firstAcceleration, firstDelta, secondAccelerationInterval,
		///		getYValue: result => ((ResponseDryRun)result).Delta,
		///		evaluateFunction: x => NextComponent.Request(absTime, dt, x, gradient, true),
		///		criterion: result => ((ResponseDryRun)result).Delta);
		/// </code>
		/// </summary>
		public static T Search<T>(T x, SI y, T interval, Func<object, SI> getYValue, Func<T, object> evaluateFunction,
			Func<object, double> criterion, bool forceLineSearch = false) where T : SIBase<T>
		{
			var iterationCount = 0;
			return Search(x, y, interval, getYValue, evaluateFunction, criterion, null, ref iterationCount, forceLineSearch);
		}

		/// <summary>
		/// Applies a numerical search over the evaluateFunction until the criterion reaches approximately 0.
		/// <code>
		/// SearchAlgorithm.Search(firstAcceleration, firstDelta, secondAccelerationInterval,
		///		getYValue: result => ((ResponseDryRun)result).Delta,
		///		evaluateFunction: x => NextComponent.Request(absTime, dt, x, gradient, true),
		///		criterion: result => ((ResponseDryRun)result).Delta);
		///     abortCriterion: result => true/false
		/// </code>
		/// </summary>
		public static T Search<T>(T x, SI y, T interval, Func<object, SI> getYValue, Func<T, object> evaluateFunction,
			Func<object, double> criterion, Func<object, int, bool> abortCriterion, bool forceLineSearch = false) where T : SIBase<T>
		{
			var iterationCount = 0;
			return Search(x, y, interval, getYValue, evaluateFunction, criterion, abortCriterion, ref iterationCount, forceLineSearch);
		}

		/// <summary>
		/// Applies a numerical search over the evaluateFunction until the criterion reaches approximately 0.
		/// <code>
		/// SearchAlgorithm.Search(firstAcceleration, firstDelta, secondAccelerationInterval,
		///		getYValue: result => ((ResponseDryRun)result).Delta,
		///		evaluateFunction: x => NextComponent.Request(absTime, dt, x, gradient, true),
		///		criterion: result => ((ResponseDryRun)result).Delta);
		///     abortCriterion: result => true/false
		/// </code>
		/// </summary>
		public static T Search<T>(T x, SI y, T interval, Func<object, SI> getYValue, Func<T, object> evaluateFunction,
			Func<object, double> criterion, Func<object, int, bool> abortCriterion, ref int iterationCount, bool forceLineSearch) where T : SIBase<T>
		{
			T result;
			try {
				if (forceLineSearch) {
					result = LineSearch(x, y, interval, getYValue, evaluateFunction, criterion, abortCriterion, ref iterationCount); 
					
				} else {
					result = InterpolateSearch(
						x, y, interval, getYValue, evaluateFunction, criterion, abortCriterion,
						ref iterationCount);
				}
			} catch (VectoException ex) {
				var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);
				log.Debug("Falling back to InterpolationSearch in reverse. Normal InterpolationSearch failed: " + ex.Message);
				try {
					result = InterpolateSearch(x, y, -interval, getYValue, evaluateFunction, criterion, abortCriterion,
						ref iterationCount);
				} catch (VectoException ex1) {
					log.Debug("Falling back to LineSearch. Reverse InterpolationSearch failed: " + ex1.Message);
					result = LineSearch(x, y, interval, getYValue, evaluateFunction, criterion, abortCriterion, ref iterationCount);
				}
			}
			return result;
		}

		/// <summary>
		/// Line Search Algorithm. 
		/// Phase 1: Linear Bracketing: Search iterative for the area of interest (with fixed step size).
		/// Phase 2: Binary Sectioning: Binary search in the area of interest.
		/// </summary>
		private static T LineSearch<T>(T x, SI y, T interval, Func<object, SI> getYValue, Func<T, object> evaluateFunction,
			Func<object, double> criterion, Func<object, int, bool> abortCriterion, ref int iterationCount) where T : SIBase<T>
		{
			var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);

			var intervalFactor = 1.0;
			var origY = y;
			var debug = new DebugData();
			debug.Add(new { x = x.Value(), y = y.Value() });
			log.Debug("Log Disabled during LineSearch.");
			LogManager.DisableLogging();
			try {
				for (var count = 1; count < 150; count++, iterationCount++) {
					if (origY.Sign() != y.Sign()) {
						intervalFactor = 0.5;
					}

					interval *= intervalFactor;
					x += interval * -y.Sign();
					var result = evaluateFunction(x);
					y = getYValue(result);
					debug.Add(new { x = x.Value(), y = y.Value(), delta = criterion(result), result });
					if (criterion(result).IsEqual(0, Constants.SimulationSettings.LineSearchTolerance / 2)) {
						LogManager.EnableLogging();
						log.Debug("LineSearch found an operating point after {0} function calls.", count);
						//iterationCount += count;
						LogManager.DisableLogging();
						AppendDebug(debug);
						return x;
					}
					if (abortCriterion != null && abortCriterion(result, iterationCount)) {
						LogManager.EnableLogging();
						log.Debug("LineSearch aborted due to abortCriterion: {0}", result);
						LogManager.DisableLogging();
						throw new VectoSearchAbortedException("LineSearch");
					}
				}
			} finally {
				LogManager.EnableLogging();
			}

			//iterationCount += 100;
			log.Debug("LineSearch could not find an operating point.");
			log.Error("Exceeded max iterations when searching for operating point!");
			log.Error("debug: {0}", debug);

			WriteSearch(debug, "LineSearch.csv");
			throw new VectoSearchFailedException("Failed to find operating point! points: {0}", debug);
		}

		[Conditional("TRACE")]
		private static void AppendDebug(DebugData debug)
		{
			var xmin = debug.Data.Min(d => d.x);
			var xmax = debug.Data.Max(d => d.x);
			var ymin = debug.Data.Min(d => d.y);
			var ymax = debug.Data.Max(d => d.y);

			var rand = new Random().Next();
			using (
				var f =
					new StreamWriter(File.Open("LineSearch-" + Thread.CurrentThread.ManagedThreadId + "-statistics.csv",
						FileMode.Append))) {
				foreach (var d in debug.Data) {
					f.WriteLine(string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}",
						rand,
						(d.x - xmin) / (xmax - xmin),
						(d.y - ymin) / (ymax - ymin),
						d.x / Math.Max(Math.Abs(xmax), Math.Abs(xmin)),
						d.y / Math.Max(Math.Abs(ymax), Math.Abs(ymin)),
						d.x, d.y));
				}
			}
		}

		/// <summary>
		/// Interpolating Search algorithm.
		/// Calculates linear equation of 2 points and jumps directly to root-point.
		/// </summary>
		private static T InterpolateSearch<T>(T x1SI, SI y1SI, T intervalSI, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, double> criterion, Func<object, int, bool> abortCriterion,
			ref int iterationCount) where T : SIBase<T>
		{
			var x1 = x1SI.Value();
			var interval = intervalSI.Value();
			var y1 = y1SI.Value();

			var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);
			var debug = new DebugData();
			debug.Add(new { x = x1, y = y1 });
			log.Debug("Log Disabled during InterpolateSearch.");
			LogManager.DisableLogging();

			try {
				var x2 = x1 + interval;
				var result = evaluateFunction(x2.SI<T>());
				if (criterion(result).IsEqual(0, Constants.SimulationSettings.InterpolateSearchTolerance)) {
					LogManager.EnableLogging();
					log.Debug("InterpolateSearch found an operating point after 1 function call.");
					AppendDebug(debug);
					LogManager.DisableLogging();
					iterationCount++;
					return x2.SI<T>();
				}

				for (var count = 2; count < 30; count++, iterationCount++) {
					var y2 = getYValue(result).Value();
					debug.Add(new { x = x2, y = y2, delta = criterion(result), result });

					var k = (y2 - y1) / (x2 - x1);
					var d = y2 - k * x2;
					x1 = x2;
					x2 = -d / k;
					if (double.IsInfinity(x2) || double.IsNaN(x2)) {
						debug.Add(new { x = x2, y = getYValue(result).Value(), delta = criterion(result), result });
						LogManager.EnableLogging();
						log.Debug("InterpolateSearch could not get more exact. Aborting after {0} function calls.", count);
						LogManager.DisableLogging();
						AppendDebug(debug);
						//iterationCount += count;

						//return x1.SI<T>();
						throw new VectoSearchAbortedException("InterpolateLinearSearch");
					}

					result = evaluateFunction(x2.SI<T>());
					if (criterion(result).IsEqual(0, Constants.SimulationSettings.InterpolateSearchTolerance)) {
						debug.Add(new { x = x2, y = getYValue(result).Value(), delta = criterion(result), result });
						LogManager.EnableLogging();
						log.Debug("InterpolateSearch found an operating point after {0} function calls.", count);
						LogManager.DisableLogging();
						AppendDebug(debug);
						return x2.SI<T>();
					}
					if (abortCriterion != null && abortCriterion(result, iterationCount)) {
						LogManager.EnableLogging();
						log.Debug("LineSearch aborted due to abortCriterion: {0}", result);
						LogManager.DisableLogging();
						throw new VectoSearchAbortedException("InterpolateLinearSearch");
					}
					y1 = y2;
				}
			} finally {
				LogManager.EnableLogging();
			}

			log.Debug("InterpolateSearch could not find an operating point.");
#if DEBUG
			log.Error("InterpolateSearch exceeded max iterations when searching for operating point!");
			log.Error("debug: {0}", debug);
#endif
			WriteSearch(debug, "InterpolateSearch.csv");
			throw new VectoSearchFailedException("Failed to find operating point! points: {0}", debug);
		}

		[Conditional("TRACE")]
		private static void WriteSearch(DebugData debug, string filename)
		{
			var table = new DataTable();
			table.Columns.Add("x", typeof(double));
			table.Columns.Add("y", typeof(double));
			table.Columns.Add("delta", typeof(double));
			table.Columns.Add("AuxPower", typeof(double));
			table.Columns.Add("engineSpeed", typeof(double));
			table.Columns.Add("enginePower", typeof(double));

			foreach (var entry in debug.Data.Skip(1)) {
				var response = entry.result as ResponseDryRun;
				if (response == null) {
					continue;
				}
				var row = table.NewRow();
				row["x"] = entry.x.Value();
				row["y"] = entry.y.Value();
				row["delta"] = entry.delta;
				row["AuxPower"] = response.AuxiliariesPowerDemand == null ? -1 : response.AuxiliariesPowerDemand.Value();
				row["engineSpeed"] = response.EngineSpeed == null ? -1 : response.EngineSpeed.Value();
				row["enginePower"] = response.EnginePowerRequest == null ? -1 : response.EnginePowerRequest.Value();

				table.Rows.Add(row);
			}
			VectoCSVFile.Write(filename, table);
		}
	}
}