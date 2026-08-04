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
using System.Linq;
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
			Func<object, double> criterion, bool forceLineSearch = false, object searcher = null) where T : SIBase<T>
		{
			var iterationCount = 0;
			return Search(x, y, interval, getYValue, evaluateFunction, criterion, null, ref iterationCount,
				forceLineSearch, searcher);
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
			Func<object, double> criterion, Func<object, int, bool> abortCriterion, bool forceLineSearch = false,
			object searcher = null) where T : SIBase<T>
		{
			var iterationCount = 0;
			return Search(x, y, interval, getYValue, evaluateFunction, criterion, abortCriterion, ref iterationCount,
				forceLineSearch, searcher);
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
			Func<object, double> criterion, Func<object, int, bool> abortCriterion, ref int iterationCount,
			bool forceLineSearch, object searcher) where T : SIBase<T>
		{
			T result;
			try {
				if (forceLineSearch) {
					result = LineSearch(x, y, interval, getYValue, evaluateFunction, criterion, abortCriterion,
						ref iterationCount, searcher);
				} else {
					result = InterpolateSearch(x, y, interval, getYValue, evaluateFunction, criterion, abortCriterion,
						ref iterationCount, searcher);
				}
			} catch (VectoException ex) {
				var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);
				log.Debug("Falling back to LineSearch. Reverse InterpolationSearch failed: " + ex.Message);
				result = LineSearch(x, y, interval, getYValue, evaluateFunction, criterion, abortCriterion,
					ref iterationCount, searcher);
			}
			return result;
		}

		/// <summary>
		/// Line Search Algorithm. 
		/// Phase 1: Linear Bracketing: Search iterative for the area of interest (with fixed step size).
		/// Phase 2: Binary Sectioning: Binary search in the area of interest.
		/// </summary>
		private static T LineSearch<T>(T xStart, SI yStart, T intervalStart, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, double> criterion, Func<object, int, bool> abortCriterion,
			ref int iterationCount, object searcher) where T : SIBase<T>
		{
			var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);
			var x = xStart;
			var y = yStart;
			var interval = intervalStart;

			var intervalFactor = 1.0;
			var origY = y;
			var debug = new DebugData();
			debug.Add($"[SA.LS-1-{iterationCount}]", new { x = x.Value(), y = y.Value() });
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
					if (abortCriterion != null && abortCriterion(result, iterationCount)) {
						debug.Add($"[SA.LS-2-{iterationCount}] - aborted", new {
							x = x.Value(),
							y = y.Value(),
							delta = criterion(result),
							result
						});
						LogManager.EnableLogging();
						log.Debug("LineSearch aborted due to abortCriterion: {0}", result);
						LogManager.DisableLogging();
						throw new VectoSearchAbortedException("LineSearch");
					}
					y = getYValue(result);
					var criterionResult = criterion(result);
					debug.Add($"[SA.LS-3-{iterationCount}]", new {
						x = x.Value(),
						y = y.Value(),
						delta = criterionResult,
						result
					});
					if (criterionResult.IsEqual(0, Constants.SimulationSettings.LineSearchTolerance / 2)) {
						LogManager.EnableLogging();
						log.Debug("LineSearch found an operating point after {0} function calls.", count);
						//iterationCount += count;
						LogManager.DisableLogging();
						AppendDebug(debug);
						return x;
					}

				}
			} finally {
				LogManager.EnableLogging();
			}

			//iterationCount += 100;
			log.Debug("LineSearch could not find an operating point.");
#if DEBUG
			log.Error("Exceeded max iterations when searching for operating point!");
#endif
			WriteSearch(debug, "LineSearch.csv");
			throw new VectoSearchFailedException("Failed to find operating point! points: {0}", debug.LocalData.Select(d => d.b).Join());
		}

		[Conditional("VECTOTRACE")]
		private static void AppendDebug(DebugData debug)
		{
#if TRACE
			//try {
			//	var xmin = debug.LocalData.Min(d => d.x);
			//	var xmax = debug.LocalData.Max(d => d.x);
			//	var ymin = debug.LocalData.Min(d => d.y);
			//	var ymax = debug.LocalData.Max(d => d.y);

			//	var rand = new Random().Next();
			//	using (
			//		var f = new StreamWriter(File.Open(
			//			"LineSearch-" + Thread.CurrentThread.ManagedThreadId + "-statistics.csv",
			//			FileMode.Append))) {
			//		foreach (var d in debug.LocalData) {
			//			f.WriteLine($"{rand}, " +
			//						$"{(d.x - xmin) / (xmax - xmin)}, " +
			//						$"{(d.y - ymin) / (ymax - ymin)}, " +
			//						$"{d.x / Math.Max(Math.Abs(xmax), Math.Abs(xmin))}, " +
			//						$"{d.y / Math.Max(Math.Abs(ymax), Math.Abs(ymin))}, " +
			//						$"{d.x}, " +
			//						$"{d.y}");
			//		}
			//	}
			//} catch (Exception ex) {
			//	//Silently ignore
			//}

#endif
		}

		/// <summary>
		/// Interpolating Search algorithm.
		/// Calculates linear equation of 2 points and jumps directly to root-point.
		/// </summary>
		private static T InterpolateSearch<T>(T x1SI, SI y1SI, T intervalSI, Func<object, SI> getYValue,
			Func<T, object> evaluateFunction, Func<object, double> criterion, Func<object, int, bool> abortCriterion,
			ref int iterationCount, object searcher) where T : SIBase<T>
		{
			var (x1, y1) = (x1SI.Value(), y1SI.Value());
			var interval = intervalSI.Value();

			var log = LogManager.GetLogger(typeof(SearchAlgorithm).FullName);
			log.Debug("Log Disabled during InterpolateSearch.");
			LogManager.DisableLogging();

			var debug = new DebugData();
			debug.Add($"[SA.IS-1-{iterationCount}]", new { x = x1, y = y1 });

			try {
				var x2 = x1 + interval;
				var result = evaluateFunction(x2.SI<T>());
				if (abortCriterion != null && abortCriterion(result, iterationCount)) {
					LogManager.EnableLogging();
					log.Debug("InterpolateSearch aborted due to abortCriterion: {0}", result);
					LogManager.DisableLogging();
					throw new VectoSearchAbortedException("InterpolateLinearSearch");
				}
				if (criterion(result).IsEqual(0, Constants.SimulationSettings.InterpolateSearchTolerance)) {
					LogManager.EnableLogging();
					log.Debug("InterpolateSearch found an operating point after 1 function call.");
					AppendDebug(debug);
					LogManager.DisableLogging();
					iterationCount++;
					return x2.SI<T>();
				}

				for (var count = 2; count < 35; count++, iterationCount++) {
					var y2 = getYValue(result).Value();
					debug.Add($"[SA.IS-2-{iterationCount}]", new { x = x2, y = y2, delta = criterion(result), result });

					var k = (y2 - y1) / (x2 - x1);
					if (count == 2 && k.IsEqual(0)) {
						x2 = x1 - interval;
						y2 = y1SI.Value();
					} else {
						var d = y2 - k * x2;
						x1 = x2;
						x2 = -d / k;
					}
					if (double.IsInfinity(x2) || double.IsNaN(x2)) {
						debug.Add($"[SA.IS-3-{iterationCount}] - infinity or NaN",
							new { x = x2, y = getYValue(result).Value(), delta = criterion(result), result });
						LogManager.EnableLogging();
						log.Debug("InterpolateSearch could not get more exact. Aborting after {0} function calls.", count);
						LogManager.DisableLogging();
						AppendDebug(debug);
						throw new VectoSearchAbortedException("InterpolateLinearSearch: y-Value constant");
					}

					result = evaluateFunction(x2.SI<T>());
					if (abortCriterion != null && abortCriterion(result, iterationCount)) {
						debug.Add($"[SA.IS-4-{iterationCount}] - abort",
							new { x = x2, y = getYValue(result).Value(), delta = criterion(result), result });
						LogManager.EnableLogging();
						log.Debug("InterpolateSearch aborted due to abortCriterion: {0}", result);
						LogManager.DisableLogging();
						throw new VectoSearchAbortedException("InterpolateLinearSearch: AbortCriterion true");
					}
					if (criterion(result).IsEqual(0, Constants.SimulationSettings.InterpolateSearchTolerance)) {
						debug.Add($"[SA.IS-5-{iterationCount}] - success",
							new { x = x2, y = getYValue(result).Value(), delta = criterion(result), result });
						LogManager.EnableLogging();
						log.Debug("InterpolateSearch found an operating point after {0} function calls.", count);
						LogManager.DisableLogging();
						AppendDebug(debug);
						return x2.SI<T>();
					}

					y1 = y2;
				}
			} finally {
				LogManager.EnableLogging();
			}

			log.Debug("InterpolateSearch could not find an operating point.");
#if DEBUG
			log.Error("InterpolateSearch exceeded max iterations when searching for operating point!");
#endif
			WriteSearch(debug, "InterpolateSearch.csv");
			throw new VectoSearchFailedException("Failed to find operating point! points: {0}", debug.LocalData.Select(d => d.b).Join());
		}

		[Conditional("VECTOTRACE")]
		private static void WriteSearch(DebugData debug, string filename)
		{
			var table = new DataTable();
			table.Columns.Add("x", typeof(double));
			table.Columns.Add("y", typeof(double));
			table.Columns.Add("delta", typeof(double));
			table.Columns.Add("AuxPower", typeof(double));
			table.Columns.Add("engineSpeed", typeof(double));
			table.Columns.Add("enginePower", typeof(double));

			foreach (var entry in debug.LocalData.Skip(1)) {
				var response = entry.result as ResponseDryRun;
				if (response == null) {
					continue;
				}
				var row = table.NewRow();
				row["x"] = entry.x.Value();
				row["y"] = entry.y.Value();
				row["delta"] = entry.delta;
				row["AuxPower"] = response.Engine.AuxiliariesPowerDemand == null ? -1 : response.Engine.AuxiliariesPowerDemand.Value();
				row["engineSpeed"] = response.Engine.EngineSpeed == null ? -1 : response.Engine.EngineSpeed.Value();
				row["enginePower"] = response.Engine.PowerRequest == null ? -1 : response.Engine.PowerRequest.Value();

				table.Rows.Add(row);
			}
			VectoCSVFile.Write(filename, table);
		}


        /// <summary>
		/// Tries to find the closest element in a sorted array using binary search
        /// </summary>
        /// <typeparam name="TArray"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="sortedArray">The SORTED array where the search should be performed</param>
        /// <param name="accessor"></param>
        /// <param name="searchValue"></param>
        /// <param name="equal">Should return a - b</param>
		/// <returns></returns>
        public static int FindClosest<TValue>(TValue[] sortedArray, TValue searchValue, Func<(TValue a, TValue b), double> diff, out (int start, int end) interval)
		{
			//TODO handle accuracy
			//Func<(TValue a, TValue b), bool> greater = t => diff(t) > 0;
			//Func<(TValue a, TValue b), bool> less = t => diff(t) < 0;
			//Func<(TValue a, TValue b), bool> equal = t => diff(t) == 0;

			Func<(TValue a, TValue b), bool> greater = t => diff(t).IsGreater(0, 1E-12);
			Func<(TValue a, TValue b), bool> less = t => diff(t).IsSmaller(0, 1E-12);
			Func<(TValue a, TValue b), bool> equal = t => diff(t).IsEqual(0, 1E-12);




            var len = sortedArray.Length; //O(1)
			int first = 0;
			int last = len - 1;
			int i = 0;

			if (less((searchValue, sortedArray.First()))) {
				interval = (first, first);
				return first;
			}

			if (greater((searchValue, sortedArray.Last()))) {
				interval = (last, last);
				return last;
			}


            while (first <= last) {
				i = (first + last) / 2;

				var current_value = sortedArray[i];
				//Debug.WriteLine($"Searching {searchValue}");
				//Debug.WriteLine($"first {first}:{sortedArray[first]} last {last}:{sortedArray[last]} array[{i}] == {current_value} ");
				//for (int j = first; j <= last; j++) {
			
				//	if (j == i) {
				//		Debug.Write($"[{sortedArray[j]}]");
				//	} else {
				//		Debug.Write($"{sortedArray[j]} ");
    //                }
				//}
				//Debug.WriteLine("\n");
                //Debug.WriteLine(sortedArray.Select(accessor).Range(first).Take(last - (first + 1)).Join(","));

				if (equal((current_value, searchValue))) {
					interval = (i, i);
					return i;
				}
				if (last - first == 1)
				{
                    //Only two elements left
					double diffLeft = Math.Abs(diff((searchValue, sortedArray[first])));
					double diffRight = Math.Abs(diff((searchValue, sortedArray[last])));

					interval = (first, last);
					return diffLeft < diffRight ? first : last;

				}





                if (greater((searchValue, current_value)))
				{
					first = i;
				}else {
					last = i;
				}
			}



			throw new VectoException("Binary search failed");
		}

		public static void BinarySearch<T>(T xStart, T xEnd, Func<T, object> evaluateFunction,
			Func<T, object, bool> acceptFunction, Func<T, object, bool> abortCriterion, ref int iterationCount, object searcher) where T : SIBase<T>
		{
			var intervals = new List<Tuple<T, T>> {
				Tuple.Create(xStart, xEnd)
			};
			const int maxIterations = 100;
			for (var i = 0; i < maxIterations; i++) {
				var current = intervals.Last();
				var newX = (current.Item1 + current.Item2) / 2;
				var result = evaluateFunction(newX);
				if (acceptFunction(newX, result)) {
					intervals.Add(Tuple.Create(newX, current.Item2));
				} else {
					intervals.Add(Tuple.Create(current.Item1, newX));
				}

				iterationCount = i;
                if (abortCriterion(newX, result)) {
					//iterationCount = i;
					return;
				}
			}

			throw new VectoSearchFailedException("Binary Search did not find a solution after {0} iterations", maxIterations);
		}
	}
}