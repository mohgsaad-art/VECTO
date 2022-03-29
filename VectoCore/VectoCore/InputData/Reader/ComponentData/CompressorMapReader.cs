using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public class CompressorMapReader
	{
		public static readonly string[] Header = new[] { Fields.RPM, Fields.FlowRate, Fields.PowerOn, Fields.PowerOff };

		public static ICompressorMap ReadFile(string filename, double dragCurveFactorClutch, string technology)
		{
			return new CompressorMap(Create(VectoCSVFile.Read(filename), dragCurveFactorClutch), technology, Path.GetFullPath(filename));
		}

		public static ICompressorMap ReadStream(Stream stream, double dragCurveFactorClutch, string technology, string source = null)
		{
			return new CompressorMap(Create(VectoCSVFile.ReadStream(stream), dragCurveFactorClutch), technology, source);
		}

		public static IList<CompressorMapValues> Create(DataTable data, double dragCurveFactorClutch)
		{
			if (!HeaderIsValid(data.Columns)) {
				throw new VectoException(
					"Invalid column header. expected: {0}, got: {1}", 
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(x => x.ColumnName)),
					string.Join(", ", Header));
			}

			if (data.Rows.Count < 3) {
				throw new VectoException("Insufficient rows in csv to build a usable map");
			}

			var entries = new List<CompressorMapValues>();
			foreach (DataRow row in data.Rows) {
				entries.Add(new CompressorMapValues(
					row.ParseDouble(Fields.RPM).RPMtoRad(),
					row.ParseDouble(Fields.FlowRate).SI(Unit.SI.Liter.Per.Minute).Cast<NormLiterPerSecond>(),
					row.ParseDouble(Fields.PowerOn).SI<Watt>(),
					row.ParseDouble(Fields.PowerOff).SI<Watt>() * dragCurveFactorClutch
					));
			}
			return entries;
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return Header.All(h => columns.Contains(h));		
		}

		public static class Fields
		{
			public const string RPM = "rpm";
			public const string FlowRate = "flowRate";
			public const string PowerOn = "power on";
			public const string PowerOff = "power off";
		}
	}
}
