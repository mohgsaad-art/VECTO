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

using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	/// Class for Reading and Writing VECTO CSV Files.
	/// </summary>
	/// <remarks>
	///     The following format applies to all CSV (Comma-separated values) Input Files used in VECTO:
	///     List DELIMITER: Comma ","
	///     Decimal-Mark: Dot "."
	///     Comments: "#" at the beginning of the comment line. Number and position of comment lines is not limited.
	///     Header: One header line (not a comment line) at the beginning of the file.
	///     All Combinations between max-format and min-format possible. Only "id"-field is used.
	///     max: id (name) [unit], id (name) [unit], ...
	///     min: id,id,...
	/// </remarks>
	public static class VectoCSVFile
	{
		private static readonly Regex HeaderFilter = new Regex(@"\[.*?\]|\<|\>", RegexOptions.Compiled);
		private const string Delimiter = ",";
		private const string Comment = "#";
		private const string DigestValuePrefix = "#@";

		/// <summary>
		/// Reads a CSV file which is stored in Vecto-CSV-Format.
		/// </summary>
		/// <param name="fileName">the filename</param>
		/// <param name="ignoreEmptyColumns">set true, if empty columns should be ignored. default: false.</param>
		/// <param name="fullHeader">set true is column names should be preserved. Otherwise units are trimed away. default: false.</param>
		/// <returns>A DataTable which represents the CSV File.</returns>
		public static TableData Read(string fileName, bool ignoreEmptyColumns = false, bool fullHeader = false)
		{
			try {
				using (var fs = File.OpenText(fileName)) {
					var retVal = new TableData(fileName);
					ReadCSV(retVal, fs, ignoreEmptyColumns, fullHeader);
					return retVal;
				}
			} catch (Exception e) {
				LogManager.GetLogger(typeof(VectoCSVFile).FullName).Error(e);
				throw new VectoException("Error reading file {0}: {1}", fileName, e.Message);
			}
		}

		/// <summary>
		/// Reads a CSV file which is stored in Vecto-CSV-Format.
		/// </summary>
		/// <param name="stream">the stream to read</param>
		/// <param name="ignoreEmptyColumns">set true, if empty columns should be ignored. default: false.</param>
		/// <param name="fullHeader">set true is column names should be preserved. Otherwise units are trimed away. default: false.</param>
		/// <param name="source"></param>
		/// <returns>A DataTable which represents the CSV File.</returns>
		public static TableData ReadStream(Stream stream, bool ignoreEmptyColumns = false, bool fullHeader = false,
			string source = null)
		{
			var retVal = new TableData(source, DataSourceType.Embedded);
			ReadCSV(retVal, new StreamReader(stream), ignoreEmptyColumns, fullHeader);
			return retVal;
		}

		private static void ReadCSV(DataTable table, StreamReader stream, bool ignoreEmptyColumns, bool fullHeader)
		{
			var p = new TextFieldParser(stream) {
				TextFieldType = FieldType.Delimited,
				Delimiters = new[] { Delimiter },
				CommentTokens = new[] { Comment },
				HasFieldsEnclosedInQuotes = true,
				TrimWhiteSpace = true
			};

			var hdrFields = p.ReadFields();
			if (hdrFields == null) {
				throw new CSVReadException("CSV Read Error: File was empty.");
			}
			var colsWithoutComment = hdrFields
				.Select(l => l.Contains(Comment) ? l.Substring(0, l.IndexOf(Comment, StringComparison.Ordinal)) : l)
				.ToArray();

			var columns = colsWithoutComment
				.Select(l => fullHeader ? l : HeaderFilter.Replace(l, ""))
				.Select(l => l.Trim())
				.Where(col => !double.TryParse(col, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
				.Distinct()
				.ToList();

			var firstLineIsData = columns.Count == 0;

			if (firstLineIsData) {
				LogManager.GetLogger(typeof(VectoCSVFile).FullName)
					.Warn("No valid Data Header found. Interpreting the first line as data line.");
				// set the validColumns to: {"0", "1", "2", "3", ...} for all columns in first line.
				columns = colsWithoutComment.Select((_, i) => i.ToString()).ToList();
			}

			columns.ForEach(col => table.Columns.Add(col));

			var lineNumber = 1;
			while (!p.EndOfData) {
				string[] cells = { };
				if (firstLineIsData) {
					cells = colsWithoutComment;
				} else {
					var fields = p.ReadFields();
					if (fields != null) {
						cells = fields.Select(l => l.Contains(Comment) ? l.Substring(0, l.IndexOf(Comment, StringComparison.Ordinal)) : l)
							.Select(s => s.Trim())
							.ToArray();
					}
				}
				firstLineIsData = false;
				if (table.Columns.Count != cells.Length && !ignoreEmptyColumns) {
					throw new CSVReadException(
						$"Line {lineNumber}: The number of values is not correct. " +
						$"Expected {table.Columns.Count} Columns, Got {cells.Length} Columns");
				}

				try {
					// ReSharper disable once CoVariantArrayConversion
					table.Rows.Add(cells);
				} catch (InvalidCastException e) {
					throw new CSVReadException(
						$"Line {lineNumber}: The data format of a value is not correct. {e.Message}", e);
				}
				lineNumber++;
			}
		}

		/// <summary>
		/// Writes the datatable to the csv file.
		/// Uses the column caption as header (with fallback to column name) for the csv header.
		/// </summary>
		/// <param name="fileName">Path to the file.</param>
		/// <param name="table">The Datatable.</param>
		/// <param name="addVersionHeader"></param>
		/// <param name="addDigest"></param>
		public static void Write(string fileName, DataTable table, bool addVersionHeader = false, bool addDigest = false)
		{
			using (var sw = new StreamWriter(new FileStream(fileName, FileMode.Create), Encoding.UTF8)) {
				Write(sw, table, addVersionHeader, addDigest);
			}
		}

		/// <summary>
		/// writes the datatable to a csv file.
		/// Uses the column caption as header (with fallback to column name) for the csv header.
		/// <remarks>Note: the callee has to make suree to close the stream after use.</remarks>
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="table"></param>
		/// <param name="addVersionHeader"></param>
		/// <param name="addDigest"></param>
		public static void Write(StreamWriter writer, DataTable table, bool addVersionHeader = false, bool addDigest = false)
		{
			if (writer == null) {
				return;
			}

			var entries = new List<string>();
			if (addVersionHeader) {
				try {
					entries.Add($"# VECTO{VectoSimulationCore.BranchSuffix} {VectoSimulationCore.VersionNumber} - {DateTime.Now:dd.MM.yyyy HH:mm}");
				} catch (Exception) {
					entries.Add($"# VECTO Unknown - {DateTime.Now:dd.MM.yyyy HH:mm}");
				}
			}
			var header = table.Columns.Cast<DataColumn>().Select(col => col.Caption ?? col.ColumnName);
			entries.Add(header.Join(Delimiter));

			var columnFormatter = new Func<ConvertedSI, string>[table.Columns.Count];
			for (var i = 0; i < table.Columns.Count; i++) {
				var col = table.Columns[i];
				var decimals = (uint?)col.ExtendedProperties["decimals"];
				var outputFactor = (double?)col.ExtendedProperties["outputFactor"];
				var showUnit = (bool?)col.ExtendedProperties["showUnit"];

				columnFormatter[i] = item => item.ToOutputFormat(decimals, outputFactor, showUnit);
			}

			foreach (DataRow row in table.Rows) {
				var items = row.ItemArray;
				var formattedList = new string[items.Length];
				for (var i = 0; i < items.Length; i++) {

					if (items[i] is SI) {
						formattedList[i] = columnFormatter[i]((SI)items[i]);
					} else if (items[i] is ConvertedSI) {
						// todo mk-2017-10-02: maybe we also have to use decimals and showUnit from columnFormatter here?
						formattedList[i] = columnFormatter[i]((ConvertedSI)items[i]);
					} else if(items[i] is bool) {
						formattedList[i] = (bool)items[i] ? "1" : "0";
					} else {
						formattedList[i] = string.Format(CultureInfo.InvariantCulture, "{0}", items[i]);
					}

					// if a string contains a "," then it has to be contained in quotes in order to be correctly recognized in a CSV file.
					if (formattedList[i].Contains(Delimiter)) {
						formattedList[i] = $"\"{formattedList[i]}\"";
					}
				}
				entries.Add(formattedList.Join(Delimiter));
			}

			if (addDigest) {
				var digest = DataIntegrityHelper.ComputeDigestValue(entries.Where(x => !x.StartsWith(DigestValuePrefix)).ToArray());
				entries.Add($"{DigestValuePrefix} {digest}");
			}

			foreach (var entry in entries) {
				writer.WriteLine(entry);
			}
		}
	}
}
