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

using System.Data;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;
using CollectionAssert = NUnit.Framework.CollectionAssert;


namespace TUGraz.Vecto.UnitTests.TestCases.Utils
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class VectoCSVFileTests
	{

		[TestCase, NUnit.Framework.Ignore("Filesystem access")]
		public void VectoCSVFile_Read()
		{
			var table = VectoCSVFile.Read(@"TestData/test.csv");
			Assert.AreEqual(3, table.Columns.Count);
			CollectionAssert.AreEqual(new[] { "a", "b", "c" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
			Assert.AreEqual(2, table.Rows.Count);

			CollectionAssert.AreEqual(new[] { "1", "2", "3" }, table.Rows[0].ItemArray);
			CollectionAssert.AreEqual(new[] { "4", "5", "6" }, table.Rows[1].ItemArray);
		}

		[TestCase, NUnit.Framework.Ignore("Filesystem access")]
		public void VectoCSVFile_Read_RealLossMap()
		{
			var table = VectoCSVFile.Read(@"TestData/Components/Axle.vtlm");
			Assert.AreEqual(3, table.Columns.Count);
			CollectionAssert.AreEqual(
				new[] { "Input Speed", "Input Torque", "Torque Loss" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
			Assert.AreEqual(285, table.Rows.Count);
		}

		[TestCase]
		public void VectoCSVFile_ReadStream_Normal()
		{
			var stream = "a,b,c\n1,2,3\n4,5,6".ToStream();
			var table = VectoCSVFile.ReadStream(stream);

			CollectionAssert.AreEqual(new[] { "a", "b", "c" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
			Assert.AreEqual(2, table.Rows.Count);

			CollectionAssert.AreEqual(new[] { "1", "2", "3" }, table.Rows[0].ItemArray);
			CollectionAssert.AreEqual(new[] { "4", "5", "6" }, table.Rows[1].ItemArray);
		}

		[TestCase]
		public void VectoCSVFile_ReadStream_Escaped()
		{
			var stream = "a,b,c\n\"1,1\",2,3\n4,5,6".ToStream();
			var table = VectoCSVFile.ReadStream(stream);

			CollectionAssert.AreEqual(new[] { "a", "b", "c" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
			Assert.AreEqual(2, table.Rows.Count);

			CollectionAssert.AreEqual(new[] { "1,1", "2", "3" }, table.Rows[0].ItemArray);
			CollectionAssert.AreEqual(new[] { "4", "5", "6" }, table.Rows[1].ItemArray);
		}

		[TestCase]
		public void VectoCSVFile_ReadStream_Comment()
		{
			var stream = "a,b,c\n\"1,1\",2,3#asdf\n4,5,6".ToStream();
			var table = VectoCSVFile.ReadStream(stream);

			CollectionAssert.AreEqual(new[] { "a", "b", "c" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
			Assert.AreEqual(2, table.Rows.Count);

			CollectionAssert.AreEqual(new[] { "1,1", "2", "3" }, table.Rows[0].ItemArray);
			CollectionAssert.AreEqual(new[] { "4", "5", "6" }, table.Rows[1].ItemArray);
		}

		[TestCase]
		public void VectoCSVFile_ReadStream_EscapedComment()
		{
			var stream = "a,b,c\n\"1,1\",2,\"3#asdf\"\n4,5,6".ToStream();
			var table = VectoCSVFile.ReadStream(stream);

			CollectionAssert.AreEqual(new[] { "a", "b", "c" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
			Assert.AreEqual(2, table.Rows.Count);

			CollectionAssert.AreEqual(new[] { "1,1", "2", "3" }, table.Rows[0].ItemArray);
			CollectionAssert.AreEqual(new[] { "4", "5", "6" }, table.Rows[1].ItemArray);
		}

		[TestCase]
		public void VectoCSVFile_ReadStream_No_Header()
		{
			var stream = "1,2,3\n4,5,6".ToStream();
			var table = VectoCSVFile.ReadStream(stream);

			CollectionAssert.AreEqual(new[] { "0", "1", "2" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
			Assert.AreEqual(2, table.Rows.Count);

			CollectionAssert.AreEqual(new[] { "1", "2", "3" }, table.Rows[0].ItemArray);
			CollectionAssert.AreEqual(new[] { "4", "5", "6" }, table.Rows[1].ItemArray);
		}

		[TestCase]
		public void VectoCSVFile_ReadStream_No_Content()
		{
			var stream = "a,b,c".ToStream();
			var table = VectoCSVFile.ReadStream(stream);

			Assert.AreEqual(3, table.Columns.Count);
			CollectionAssert.AreEqual(new[] { "a", "b", "c" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
			Assert.AreEqual(0, table.Rows.Count);
		}

		[TestCase]
		public void VectoCSVFile_ReadStream_Empty()
		{
			AssertHelper.Exception<VectoException>(() => VectoCSVFile.ReadStream("".ToStream()));
		}

		[TestCase]
		public void VectoCSVFile_ReadStream_Comments()
		{
			var stream = @"#a,b,c
						   #21,22,23
						   #674,95,96
						   a,b,c
						   #9,8,7
						   1,2,3
						   4,5,6".ToStream();
			var table = VectoCSVFile.ReadStream(stream);

			CollectionAssert.AreEqual(new[] { "a", "b", "c" }, table.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
			Assert.AreEqual(2, table.Rows.Count);

			CollectionAssert.AreEqual(new[] { "1", "2", "3" }, table.Rows[0].ItemArray);
			CollectionAssert.AreEqual(new[] { "4", "5", "6" }, table.Rows[1].ItemArray);
		}

		[TestCase]
		public void VectoCSVFile_Write_Filename()
		{
			const string fileName = "out_test.csv";

			if (File.Exists(fileName)) {
				File.Delete(fileName);
			}

			var table = new DataTable();
			table.Columns.Add("a");
			table.Columns.Add("b");
			table.Rows.Add("1", "2");

			VectoCSVFile.Write(fileName, table);

			var lines = File.ReadAllLines(fileName);
			Assert.AreEqual("a,b", lines[0]);
			Assert.AreEqual("1,2", lines[1]);
		}

		[TestCase]
		public void VectoCSVFile_Write_StreamWriter()
		{
			var table = new DataTable();
			table.Columns.Add("a");
			table.Columns.Add("b");
			var row = table.NewRow();
			row.ItemArray = new object[] { "1", "2" };
			table.Rows.Add(row);

			using (var stream = new MemoryStream()) {
				using (var sw = new StreamWriter(stream)) {
					VectoCSVFile.Write(sw, table);
					sw.Flush();

					stream.Position = 0;

					using (var sr = new StreamReader(stream)) {
						string line1 = sr.ReadLine();
						Assert.AreEqual("a,b", line1);
						
						string line2 = sr.ReadLine();
						Assert.AreEqual("1,2", line2);
					}
				}
			}
		}
	}
}
