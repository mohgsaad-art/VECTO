using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class BatteryInternalResistanceReader
	{
		public static InternalResistanceMap Create(DataTable data, bool inmOhm)
		{
			if (!(data.Columns.Count == 2 || data.Columns.Count == 4 || data.Columns.Count == 5)) {
				throw new VectoException(
					"Internal Resistance Map data must contain either two, four or five columns: {0}, {1}",
					Fields.StateOfCharge, Fields.InternalResistance);
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("Internal Resistance Map data must contain at least 2 entries!");
			}

			if (data.Columns.Count == 2) {
				if (!data.Columns.Contains(Fields.StateOfCharge) || !data.Columns.Contains(Fields.InternalResistance)) {
					LoggingObject.Logger<InternalResistanceMap>().Warn(
						"Internal Resistance Map Header is invalid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
						Fields.StateOfCharge, 
						Fields.InternalResistance,
						data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
					data.Columns[0].ColumnName = Fields.StateOfCharge;
					data.Columns[1].ColumnName = Fields.InternalResistance;
				}

				return new InternalResistanceMap(data.Rows.Cast<DataRow>().Select(row => {
					var resistance = row.ParseDouble(Fields.InternalResistance).SI<Ohm>();
					return new InternalResistanceMap.InternalResistanceMapEntry() {
						SoC = row.ParseDouble(Fields.StateOfCharge) / 100,
						Resistance = new List<Tuple<Second, Ohm>>() {
							Tuple.Create(0.SI<Second>(), resistance),
							Tuple.Create(1e9.SI<Second>(), resistance)
						}
					};
				}).OrderBy(e => e.SoC).ToArray());
			}

			var col1 = new[] {
				Tuple.Create(2.SI<Second>(), Fields.InternalResistance_2),
				Tuple.Create(10.SI<Second>(), Fields.InternalResistance_10),
				Tuple.Create(20.SI<Second>(), Fields.InternalResistance_20),
			};
			if (data.Columns.Count == col1.Length + 1) {
				return ReadInternalResistanceMap(data, col1, inmOhm);
			}

			var col2 = new[] { 
				Tuple.Create(2.SI<Second>(), Fields.InternalResistance_2), 
				Tuple.Create(10.SI<Second>(), Fields.InternalResistance_10),
				Tuple.Create(20.SI<Second>(), Fields.InternalResistance_20),
				Tuple.Create(120.SI<Second>(), Fields.InternalResistance_120)
			};
			if (data.Columns.Count == col2.Length + 1) {
				return ReadInternalResistanceMap(data, col2, inmOhm);
			}


			throw new VectoException("Failed to read InternalResistanceMap");
		}

		private static InternalResistanceMap ReadInternalResistanceMap(DataTable data, Tuple<Second, string>[] col1, bool inmOhm)
		{
			if ((!data.Columns.Contains(Fields.StateOfCharge) || !col1.All(x => data.Columns.Contains(x.Item2)))) {
				for (var i = 0; i < col1.Length; i++) {
					data.Columns[i].ColumnName = col1[i].Item2;
				}
				LoggingObject.Logger<InternalResistanceMap>().Warn(
					"Internal Resistance Map Header is invalid. Expected: '{0}', Got: '{1}'. Falling back to column index.",
					col1.Select(x => x.Item2).Join(),
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
			}

			var factor = inmOhm ? 1000 : 1;
			return new InternalResistanceMap(data.Rows.Cast<DataRow>().Select(row => {
				var values = col1.Select(x =>
						row.Table.Columns.Contains(x.Item2) ? Tuple.Create(x.Item1, row.ParseDouble(x.Item2).SI<Ohm>() * factor) : null)
					.Where(x => x != null).ToList();
				return new InternalResistanceMap.InternalResistanceMapEntry() {
					SoC = row.ParseDouble(Fields.StateOfCharge) / 100,
					Resistance = values
				};
			}).OrderBy(e => e.SoC).ToArray());
		}

		public static class Fields
		{
			public const string StateOfCharge = "SoC";

			public const string InternalResistance = "Ri";

			public const string InternalResistance_2 = "Ri-2";

			public const string InternalResistance_10 = "Ri-10";

			public const string InternalResistance_20 = "Ri-20";

			public const string InternalResistance_120 = "Ri-120";

		}

		public static InternalResistanceMap Create(Stream data, bool inmOhm)
		{
			return Create(VectoCSVFile.ReadStream(data), inmOhm);
		}
	}
}