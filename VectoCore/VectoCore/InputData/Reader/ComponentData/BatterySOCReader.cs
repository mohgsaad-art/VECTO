using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData {
	public static class BatterySOCReader
	{
		public static SOCMap Create(Stream data)
		{
			return Create(VectoCSVFile.ReadStream(data));
		}


		public static SOCMap Create(DataTable data)
		{
			if (data.Columns.Count != 2)
			{
				throw new VectoException($"SOC-Map data must contain exactly two columns: {Fields.StateOfCharge}, {Fields.BatteryVoltage}", ErrorCodes.DATATABLE_WRONG_COLUMNS,
					$"Make sure the SOC-Map contains the correct number of columns.");
			}
			if (data.Rows.Count < 2)
			{
				throw new VectoException("SoC-Map data must contain at least 2 entries!", ErrorCodes.DATATABLE_INSUFFICIENT_DATA,
					$"Make sure the SOC-Map contains at least two entries.");
			}
			if (!data.Columns.Contains(Fields.StateOfCharge) || !data.Columns.Contains(Fields.BatteryVoltage))
			{
				data.Columns[0].ColumnName = Fields.StateOfCharge;
				data.Columns[1].ColumnName = Fields.BatteryVoltage;
				LoggingObject.Logger<SOCMap>().Warn(
					"SoC-Map Header is invalid. Expected: '{0}, {1}', Got: '{2}'. Falling back to column index.",
					Fields.StateOfCharge, 
					Fields.BatteryVoltage,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
			}

			var entries = data.Rows.Cast<DataRow>().Select(row => new SOCMap.SOCMapEntry {
				SOC = row.ParseDouble(Fields.StateOfCharge) / 100,
				BatteryVolts = row.ParseDouble(Fields.BatteryVoltage).SI<Volt>()
			}).OrderBy(e => e.SOC).ToList();
			var first = entries.FirstOrDefault();
			var last = entries.LastOrDefault();
			if (first.SOC > 0) {
				entries.Add(new SOCMap.SOCMapEntry() {
					SOC = 0,
					BatteryVolts = first.BatteryVolts
				});
			}

			if (last.SOC < 1) {
				entries.Add(new SOCMap.SOCMapEntry() {
					SOC = 1,
					BatteryVolts = last.BatteryVolts
				});
			}
            return new SOCMap(entries.OrderBy(x => x.SOC).ToArray());
		}

		public static class Fields
		{
			public const string StateOfCharge = "SoC";

			public const string BatteryVoltage = "V";
		}
	}

}