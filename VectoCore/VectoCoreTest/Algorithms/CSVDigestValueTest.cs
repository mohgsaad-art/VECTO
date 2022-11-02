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
using NUnit.Framework.Internal;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Algorithms
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class CSVDigestValueTest
	{

		[TestCase]
		public void TestDigestValueCreation()
		{
			var tbl = CreateDataTable(new[] { "t", "dt", "v" }, 5);

			var str = new MemoryStream();
			var writer = new StreamWriter(str);
			VectoCSVFile.Write(writer, tbl, true, true);

			writer.Flush();
			str.Flush();
			str.Seek(0, SeekOrigin.Begin);

			var reader = new StreamReader(str);
			var lines = new List<string>();
			while (!reader.EndOfStream)
				lines.Add(reader.ReadLine());

			var last = lines.Last();

			Assert.IsTrue(last.StartsWith("#@"), "Digest Identifier not found");
			Assert.IsTrue(last.Contains("SHA256"), "Digest descriptor SHA256 not found");
		}

		[TestCase]
		public void TestDigestValueValidation()
		{
			var tbl = CreateDataTable(new[] { "t", "dt", "v" }, 5);

			var str = new MemoryStream();
			var writer = new StreamWriter(str);
			VectoCSVFile.Write(writer, tbl, true, true);

			writer.Flush();
			str.Flush();
			str.Seek(0, SeekOrigin.Begin);

			var reader = new StreamReader(str);
			var lines = new List<string>();
			while (!reader.EndOfStream)
				lines.Add(reader.ReadLine());

			var last = lines.Last();

			Assert.IsTrue(last.StartsWith("#@"), "Digest Identifier not found");
			Assert.IsTrue(last.Contains("SHA256"), "Digest descriptor SHA256 not found");

			var otherLines = lines.Where(x => !x.StartsWith("#@")).ToArray();

			var digest = DataIntegrityHelper.ComputeDigestValue(otherLines);

			Assert.AreEqual($"#@ {digest}", last);
		}

		private static DataTable CreateDataTable(string[] cols, int numRows)
		{
			var tbl = new DataTable();
			
			foreach (var col in cols) {
				tbl.Columns.Add(col, typeof(double));
			}

			var rnd = new Randomizer(873);
			for (var i = 0; i < numRows; i++) {
				var row = tbl.NewRow();
				foreach (var col in cols) {
					row[col] = rnd.NextDouble(100);
				}

				tbl.Rows.Add(row);
			}

			return tbl;
		}


		
	}
}
