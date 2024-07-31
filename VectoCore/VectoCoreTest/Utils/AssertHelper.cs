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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using TUGraz.VectoCommon.Utils;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Utils
{
	public static class AssertHelper
	{
		/// <summary>
		/// Assert an expected Exception.
		/// </summary>
		[DebuggerHidden]
		public static void Exception<T>(this Action func, string message = null, string messageContains = null) where T : Exception
		{
			try {
				func();
				Assert.Fail("Expected Exception {0}, but no exception occured.", typeof(T));
			} catch (T ex) {
				if (message != null) {
					Assert.AreEqual(message, ex.Message);
				}
				if (messageContains != null) {
					Assert.IsTrue(ex.Message.Contains(messageContains), "Exception message does not contain expected text. Expected: '{1}', Message: '{0}'", ex.Message, messageContains);
				}
			}
		}

		[DebuggerHidden]
		public static void AreRelativeEqual(SI expected, SI actual,
			double toleranceFactor = DoubleExtensionMethods.ToleranceFactor, string message = null)
		{
			if (!actual.HasEqualUnit(expected))
				Assert.IsTrue(actual.HasEqualUnit(expected), "Wrong SI Units: expected: {0}, actual: {1}", expected.ToBasicUnits(), actual.ToBasicUnits());
			AreRelativeEqual(expected.Value(), actual.Value(), toleranceFactor: toleranceFactor, message: message);
		}

		[DebuggerHidden]
		public static void AreRelativeEqual(Scalar expected, Scalar actual,
			double toleranceFactor = DoubleExtensionMethods.ToleranceFactor)
		{
			AreRelativeEqual(expected.Value(), actual.Value(), toleranceFactor: toleranceFactor);
		}

		[DebuggerHidden]
		public static void AreRelativeEqual(double? expected, SI actual,
			double toleranceFactor = DoubleExtensionMethods.ToleranceFactor)
		{
			if (expected.HasValue) {
				AreRelativeEqual(expected.Value, actual.Value(), toleranceFactor: toleranceFactor);
			} else {
				Assert.IsNull(actual, "Both Values have to be null or not null.");
			}
		}

		[DebuggerHidden]
		public static void AreRelativeEqual(
			double? expected,
			double? actual,
			string message = null,
			double toleranceFactor = DoubleExtensionMethods.ToleranceFactor)
		{
			if (!string.IsNullOrWhiteSpace(message)) {
				message = "\n" + message;
			} else {
				message = "";
			}

			Assert.IsFalse(expected.HasValue ^ actual.HasValue, "Both Values have to be null or not null.");

			if (double.IsNaN(expected.Value)) {
				Assert.IsTrue(
					double.IsNaN(actual.Value),
					"Actual value is not NaN. Expected: {0}, Actual: {1}{2}",
					expected,
					actual,
					message);
				return;
			}

			var ratio = expected == 0 ? Math.Abs(actual.Value) : Math.Abs(actual.Value / expected.Value - 1);
			Assert.IsTrue(
				ratio < toleranceFactor,
				"Given values are not equal. Expected: {0}, Actual: {1}, Difference: {3} (Tolerance Factor: {2}){4}",
				expected,
				actual,
				toleranceFactor,
				expected - actual,
				message);
		}

		public static void PublicPropertiesEqual(Type t, object expected, object actual, string[] ignoredProperties = null)
		{
			const BindingFlags flags =
				BindingFlags.Instance | BindingFlags.Public |
				BindingFlags.FlattenHierarchy;
			var properties = t.GetProperties(flags);

			foreach (var prop in properties) {
				if (ignoredProperties != null && ignoredProperties.Contains(prop.Name)) {
					continue;
				}
				object expectedVal = null;
				try {
					expectedVal = prop.GetValue(expected);
				} catch (Exception) {
					try {
						prop.GetValue(actual);
						Assert.Fail("expected value thew exception, but actual value not!");
					} catch (Exception) {
						// both getters threw an exception - at least its the same...
						continue;
					}
				}
				var actualVal = prop.GetValue(actual);
				var propertyType = prop.PropertyType;
				if (expectedVal == null && actualVal == null) {
					continue;
				}
				if (propertyType.IsPrimitive || propertyType == typeof(string)) {
					Assert.AreEqual(expectedVal, actualVal, $"Property {prop.Name}, expected: {expectedVal}, actual: {actualVal}");
				} else if (propertyType == typeof(SI)) {
					Assert.AreEqual((expectedVal as SI).Value(), (actualVal as SI).Value());
					Assert.AreEqual((expectedVal as SI).UnitString, (actualVal as SI).UnitString);
				} else if (expectedVal is IEnumerable<object>) {
					Assert.IsTrue(actualVal is IList);
					var expectedEnumerable = (expectedVal as IEnumerable<object>).ToArray();
					Assert.IsTrue(actualVal is IEnumerable<object>);
					var actualEnumerable = (actualVal as IEnumerable<object>).ToArray();
					Assert.AreEqual(expectedEnumerable.Length, actualEnumerable.Length);
					if (expectedEnumerable.Length > 0) {
						IterateElements(expectedEnumerable, actualEnumerable, ignoredProperties);
					}
				} else if (propertyType == typeof(TableData)) {
					TableDataEquals(expectedVal as TableData, actualVal as TableData);
				} else {
					PublicPropertiesEqual(propertyType, expectedVal, actualVal, ignoredProperties);
				}
			}

		}

        public static void ReportDeviations(String distanceSumPath, int distanceSumRow, SimulatorFactory factory, 
			Dictionary<String, double> metrics)
        { 
			String sumFilePath = WriteSumFile(factory);

			var table = VectoCSVFile.Read(sumFilePath, true, true);
			var row = table.Rows[0];

			var distanceTable = VectoCSVFile.Read(distanceSumPath, true, true);
			var distanceRow = distanceTable.Rows[distanceSumRow];
		
			foreach (var metric in metrics) {
				double result;
				Assert.IsTrue(double.TryParse(row[metric.Key].ToString(), out result));

				double distanceResult;
				Assert.IsTrue(double.TryParse(distanceRow[metric.Key].ToString(), out distanceResult));

				double deviation = ((result - distanceResult) / distanceResult) * 100;

				double expectedDeviation = ((metric.Value - distanceResult) / distanceResult) * 100;

				TestContext.WriteLine($"Distance run deviation of {metric.Key} = {deviation.ToString("N2")} %   (expected = {expectedDeviation.ToString("N2")} %)");
            }

			TestContext.WriteLine();
		}

		public static void AssertMetricsRange(ISimulatorFactory factory, Dictionary<String, double> metrics, double vectoTolerance = DoubleExtensionMethods.VectoToleranceFactor)
		{
			AssertMetrics(factory, metrics, vectoTolerance);
		}

		public static void AssertMetrics(ISimulatorFactory factory, Dictionary<String, double> metrics, double tolerance = DoubleExtensionMethods.ToleranceFactor)
		{
			String sumFilePath = WriteSumFile(factory);

			var table = VectoCSVFile.Read(sumFilePath, true, true);
			var row = table.Rows[0];

			Dictionary<string, double> results = new Dictionary<string, double>();

			foreach (var kvp in metrics)
			{
				double result;
				Assert.IsTrue(double.TryParse(row[kvp.Key].ToString(), out result));

				results.Add(kvp.Key, result);

				TestContext.WriteLine($"{kvp.Key} = {result}   (expected = {kvp.Value})");
			}
			TestContext.WriteLine();
			TestContext.WriteLine(metrics.Keys.Select(x => results[x]).Join());

			foreach (var metric in metrics)
			{
				var expected = metric.Value.SI<Scalar>();
				var actual = results[metric.Key].SI<Scalar>();

				AreRelativeEqual(expected, actual, $"{metric.Key} ({results[metric.Key]}) is other than expected ({metric.Value})", tolerance);
				LogGitlabMetrics(metric.Key, actual);
			}
		}

		/// <summary>
		/// Appends the current test result to the metrics file gitlab_metrics.log.
		/// 
		/// The metrics file stores key value pairs in different lines with format:
		/// MethodName_VehicleInputFile_CycleName(MetricField,ExpectedValue) ActualValue
		/// </summary>
		/// <param name="metricField">Name of the metric field to log.</param>
		/// <param name="actual">Result of the current test to log.</param>
		private static void LogGitlabMetrics(string metricField, double? actual)
		{
			var metricFieldFormatted = metricField.Replace(" ", "");
			
			string vehicleFileRaw = TestContext.CurrentContext.Test.Arguments[0].ToString();
			string cycleName = TestContext.CurrentContext.Test.Arguments[1].ToString();
			string expectedValue = TestContext.CurrentContext.Test.Arguments[2].ToString();

			string vehicleFile =
				Path.GetFileName(vehicleFileRaw)
					.Replace(" ", "_")
					.Replace(".", "_");


			var metricKey = $"{TestContext.CurrentContext.Test.MethodName}.{vehicleFile}.{cycleName}_({metricFieldFormatted},{expectedValue})";

			var filePath = Path.Join(TestContext.CurrentContext.TestDirectory, "gitlab_metrics.log");
			File.AppendAllLines(
				filePath,
				new List<string>() { $"{metricKey} {actual}" });
		}

		public static void ReadMetricsFromVSum(String vsumPath, int vsumRow, Dictionary<String, double> metrics)
		{
			var distanceTable = VectoCSVFile.Read(vsumPath, true, true);
			var distanceRow = distanceTable.Rows[vsumRow];
		
			foreach (var metric in metrics) {
				double distanceResult;
				Assert.IsTrue(double.TryParse(distanceRow[metric.Key].ToString(), out distanceResult));

				metrics[metric.Key] = distanceResult;
			}
        }

		private static string WriteSumFile(SimulatorFactory factory)
        {
			String sumFilePath = ((FileOutputWriter) factory.ReportWriter).SumFileName;

			if (!File.Exists(sumFilePath)) {
				factory.SumData.Finish();
            }

			return sumFilePath;
		}

		private static void TableDataEquals(TableData expected, TableData actual)
		{
			Assert.NotNull(expected);
			Assert.NotNull(actual);

			Assert.AreEqual(expected.Columns.Count, actual.Columns.Count);
			Assert.AreEqual(expected.Rows.Count, actual.Rows.Count);

			foreach (DataColumn expectedCol in expected.Columns) {
				Assert.NotNull(actual.Columns[expectedCol.ColumnName]);
			}

			//foreach (DataRow row in expected.Rows) {
			for (var i = 0; i < expected.Rows.Count; i++) {
				var expectedRow = expected.Rows[i];
				var actualRow = actual.Rows[i];
				foreach (DataColumn col in expected.Columns) {
					var value = expectedRow[col];
					if (value is ConvertedSI) {
						Assert.AreEqual((value as ConvertedSI).Value, (actualRow[col] as ConvertedSI).Value);
					}
					if (value.GetType().IsPrimitive) {
						Assert.AreEqual(value, actualRow[col]);
					}
				}
			}
		}

		private static void IterateElements(IEnumerable<object> expected, IEnumerable<object> actual, string[] ignoredProperties = null)
		{
			foreach (var entry in expected.Zip(actual, Tuple.Create)) {
				PublicPropertiesEqual(entry.Item1.GetType(), entry.Item1, entry.Item2, ignoredProperties);
			}
		}
	}
}
