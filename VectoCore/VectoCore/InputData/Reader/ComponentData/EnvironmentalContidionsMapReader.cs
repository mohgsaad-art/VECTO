using System;
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
					$"Invalid Header for environmental conditions. Expected: {Header.Join()}, Got: {data.Columns.Cast<DataColumn>().Select(x => x.ColumnName).Join()}", ErrorCodes.DATATABLE_WRONG_COLUMNS);
			}

			var heatpumps = new List<Tuple<string, HeatPumpType>>();
			var heater = new List<Tuple<string, HeaterType>>();
			foreach (DataColumn column in data.Columns) {
				var heatPump = HeatPumpTypeHelper.TryParse(column.ColumnName);
				if (heatPump != null) {
					heatpumps.Add(Tuple.Create(column.ColumnName, heatPump.Value));
				}

				var heaterCol = HeaterTypeHelper.TryParse(column.ColumnName);
				if (heaterCol != null) {
					heater.Add(Tuple.Create(column.ColumnName, heaterCol.Value));
				}
			}

			foreach (DataRow row in data.Rows) {
				var cooling = row.Field<string>(Fields.HeatingCooling)
					.Equals("c", StringComparison.InvariantCultureIgnoreCase);
				var heatPumpCoP = new Dictionary<HeatPumpType, double>();
				foreach (var entry in heatpumps) {
					var val = row.ParseDoubleOrGetDefault(entry.Item1, double.NaN);
					if (double.IsNaN(val)) { continue; }
					heatPumpCoP.Add(entry.Item2, val);
				}

				var heaterEfficiency = new Dictionary<HeaterType, double>();
				foreach (var entry in heater) {
					var val = row.ParseDoubleOrGetDefault(entry.Item1, double.NaN);
					if (double.IsNaN(val)) { continue; }
					heaterEfficiency.Add(entry.Item2, val);
				}
				entries.Add(
					new EnvironmentalConditionMapEntry(
						row.Field<string>(Fields.ID).ToInt(),
						row.ParseDouble(Fields.EnvTemp).DegCelsiusToKelvin(),
						row.ParseDouble(Fields.Solar).SI<WattPerSquareMeter>(),
						row.ParseDouble(Fields.WeightingFactor),
						heatPumpCoP,
						heaterEfficiency));
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

			public const string HeatingCooling = "heating/cooling";
		}
	}
}
