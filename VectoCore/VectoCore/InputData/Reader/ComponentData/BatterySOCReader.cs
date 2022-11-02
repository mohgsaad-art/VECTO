using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
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
				throw new VectoException("SOC-Map data must contain exactly two columns: {0}, {1}", Fields.StateOfCharge, Fields.BatteryVoltage);
			}
			if (data.Rows.Count < 2)
			{
				throw new VectoException("SoC-Map data must contain at least 2 entries!");
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
			return new SOCMap(data.Rows.Cast<DataRow>().Select(row => new SOCMap.SOCMapEntry
			{
				SOC = row.ParseDouble(Fields.StateOfCharge) / 100,
				BatteryVolts = row.ParseDouble(Fields.BatteryVoltage).SI<Volt>()
			}).OrderBy(e => e.SOC).ToArray());
		}

		public static class Fields
		{
			public const string StateOfCharge = "SoC";

			public const string BatteryVoltage = "V";
		}
	}
}