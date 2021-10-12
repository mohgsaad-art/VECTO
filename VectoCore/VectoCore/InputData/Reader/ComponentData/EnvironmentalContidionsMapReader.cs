using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class EnvironmentalContidionsMapReader
	{
		public static readonly string[] Header = new[] { Fields.ID, Fields.EnvTemp, Fields.Solar, Fields.WeightingFactor };

		public static IEnvironmentalConditionsMap ReadFile(string fileName)
		{
			if ((string.IsNullOrWhiteSpace(fileName))) {
				return null;
			}

			if (!File.Exists(fileName)) {
				throw new FileNotFoundException(fileName);
			}

			return Create(VectoCSVFile.Read(fileName));
		}

		public static IEnvironmentalConditionsMap ReadStream(Stream stream)
		{
			return Create(VectoCSVFile.ReadStream(stream));
		}

		public static IEnvironmentalConditionsMap Create(TableData data)
		{
			var entries = new List<EnvironmentalConditionMapEntry>();

			if (!HeaderIsValid(data.Columns)) {
				throw new VectoException(
					"Invalid Header for environmental conditions. Got: {0}, expected: {1}",
					string.Join(", ", data.Columns.Cast<DataColumn>().Select(x => x.ColumnName)), 
					string.Join(", ", Header));
			}

			foreach (DataRow row in data.Rows) {
				entries.Add(
					new EnvironmentalConditionMapEntry(
						row.ParseDouble(Fields.EnvTemp).DegCelsiusToKelvin(),
						row.ParseDouble(Fields.Solar).SI<WattPerSquareMeter>(),
						row.ParseDouble(Fields.WeightingFactor)));
			}

			var sum = entries.Sum(e => e.Weighting);
			foreach (var entry in entries) {
				entry.Weighting = entry.Weighting / sum;
			}

			return new EnvironmentalConditionsMap(entries.Cast<IEnvironmentalConditionsMapEntry>().ToList());
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return Header.All(h => columns.Contains(h));
		}


		public static class Fields
		{
			public const string ID = "ID";
			public const string EnvTemp = "EnvTemp";
			public const string Solar = "Solar";
			public const string WeightingFactor = "WeightingFactor";
		}
	}
}
