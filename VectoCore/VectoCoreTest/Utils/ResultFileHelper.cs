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
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public static class ResultFileHelper
	{
		public static void TestModFile(string expectedFile, string actualFile, string[] testColumns = null,
			bool testRowCount = true)
		{
			TestModFiles(new[] { expectedFile }, new[] { actualFile }, testColumns, testRowCount);
		}

		public static void TestModFiles(IEnumerable<string> expectedFiles, IEnumerable<string> actualFiles,
			string[] testColumns = null, bool testRowcount = true)
		{
			var resultFiles = expectedFiles.ZipAll(actualFiles, (expectedFile, actualFile) => new { expectedFile, actualFile });
			foreach (var result in resultFiles) {
				Assert.IsTrue(File.Exists(result.actualFile), "MOD File is missing: " + result);
				Assert.IsTrue(File.Exists(result.expectedFile), "Expected File is missing: " + result);

				var expected = VectoCSVFile.Read(result.expectedFile);
				var actual = VectoCSVFile.Read(result.actualFile);

				if (actual.Columns.Contains(ModalResultField.v_act.GetShortCaption()) &&
					!double.IsNaN(((string)actual.Rows[0][ModalResultField.v_act.GetShortCaption()]).ToDouble(double.NaN))) {
					// test v_act >= 0
					Assert.IsTrue(actual.Rows.Cast<DataRow>()
						.All(r => r.ParseDouble(ModalResultField.v_act.GetShortCaption()).IsGreaterOrEqual(0)),
						"v_act must not be negative.");

					// test v_targ >= 0
					Assert.IsTrue(actual.Rows.Cast<DataRow>()
						.All(r => r.ParseDouble(ModalResultField.v_targ.GetShortCaption()).IsGreaterOrEqual(0)),
						"v_targ must not be negative.");
				}

				if (actual.Columns.Contains(ModalResultField.dist.GetShortCaption()) &&
					!double.IsNaN(((string)actual.Rows[0][ModalResultField.dist.GetShortCaption()]).ToDouble(double.NaN))) {
					// test distance monotonous increasing

					var distPrev = actual.Rows[0].ParseDouble(ModalResultField.dist.GetShortCaption());
					for (var i = 1; i < actual.Rows.Count; i++) {
						var dist = actual.Rows[i].ParseDouble(ModalResultField.dist.GetShortCaption());
						Assert.IsTrue(distPrev.IsSmallerOrEqual(dist), "distance must not decrease.");
						distPrev = dist;
					}
				}

				if (testRowcount) {
					Assert.AreEqual(expected.Rows.Count, actual.Rows.Count,
						$"Moddata: Row count differs.\n" +
						$"Expected {expected.Rows.Count} Rows in {result.expectedFile}\n" +
						$"Got {actual.Rows.Count} Rows in {result.actualFile}");
				}

				var actualCols = actual.Columns.Cast<DataColumn>().Select(x => x.ColumnName).OrderBy(x => x).ToList();
				var expectedCols = expected.Columns.Cast<DataColumn>().Select(x => x.ColumnName).OrderBy(x => x).ToList();

				if (testColumns != null) {
					actualCols =
						actual.Columns.Cast<DataColumn>()
							.Where(col => testColumns.Any(req => req.Equals(col.ColumnName))).Select(x => x.ColumnName)
							.OrderBy(x => x)
							.ToList();
					expectedCols = expected.Columns.Cast<DataColumn>()
						.Where(col => testColumns.Any(req => req.Equals(col.ColumnName))).Select(x => x.ColumnName)
						.OrderBy(x => x)
						.ToList();
				}

				CollectionAssert.AreEqual(expectedCols, actualCols,
					"Moddata {0}: Columns differ:\nExpected: {1}\nMissing:{2},\nToo Much:{3}",
					result.actualFile,
					expectedCols.Join(),
					expectedCols.Except(actualCols).Join(),
					actualCols.Except(expectedCols).Join());

				for (var i = 0; testRowcount && i < expected.Rows.Count; i++) {
					var expectedRow = expected.Rows[i];
					var actualRow = actual.Rows[i];

					foreach (var field in testColumns ?? new string[0]) {
						Assert.AreEqual(expectedRow.ParseDoubleOrGetDefault(field), actualRow.ParseDoubleOrGetDefault(field), 1e-4,
							$"t: {i}  field: {field}");
					}
				}
			}
		}

		public static void TestSumFile(string expectedFile, string actualFile, string[] testColumns = null)
		{
			Assert.IsTrue(File.Exists(actualFile), "SUM File is missing: " + actualFile);

			var expected = VectoCSVFile.Read(expectedFile, fullHeader: true);
			var actual = VectoCSVFile.Read(actualFile, fullHeader: true);

			Assert.AreEqual(expected.Rows.Count, actual.Rows.Count,
				$"SUM File row count differs.\nExpected {expected.Rows.Count} Rows in {expectedFile}\nGot {actual.Rows.Count} Rows in {actualFile}");

			var actualCols = actual.Columns.Cast<DataColumn>().Select(x => x.ColumnName).OrderBy(x => x).ToList();
			var expectedCols = expected.Columns.Cast<DataColumn>().Select(x => x.ColumnName).OrderBy(x => x).ToList();

			CollectionAssert.AreEqual(expectedCols, actualCols,
				"SUM FILE {0}: Columns differ:\nExpected: {1}\nMissing:{2},\nToo Much:{3}",
				actualFile,
				expectedCols.Join(),
				expectedCols.Except(actualCols).Join(),
				actualCols.Except(expectedCols).Join());

			for (var i = 0; i < expected.Rows.Count; i++) {
				var expectedRow = expected.Rows[i];
				var actualRow = actual.Rows[i];

				foreach (var field in testColumns ?? new string[0]) {
					AssertHelper.AreRelativeEqual(expectedRow.ParseDoubleOrGetDefault(field), actualRow.ParseDoubleOrGetDefault(field),
						$"t: {i}  field: {field}");
				}
			}
		}
	}
}
