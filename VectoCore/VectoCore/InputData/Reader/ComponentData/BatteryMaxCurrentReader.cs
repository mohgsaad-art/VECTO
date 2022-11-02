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
	public static class BatteryMaxCurrentReader
	{
		public static MaxCurrentMap Create(DataTable data)
		{
			if (data.Columns.Count != 3) {
				throw new VectoException("Max Current Map data must contain exactly three columns: {0}, {1}, {2}", Fields.StateOfCharge, Fields.MaxChargeCurrent, Fields.MaxDischargeCurrent);
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("Max Current  Map data must contain at least 2 entries!");
			}

			if (!data.Columns.Contains(Fields.StateOfCharge) || !data.Columns.Contains(Fields.MaxDischargeCurrent) || !data.Columns.Contains(Fields.MaxChargeCurrent)) {
				LoggingObject.Logger<InternalResistanceMap>().Warn(
					"Max Current  Map Header is invalid. Expected: '{0}, {1}, {2}', Got: '{3}'. Falling back to column index.",
					Fields.StateOfCharge,
					Fields.MaxChargeCurrent,
					Fields.MaxDischargeCurrent,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
				data.Columns[0].ColumnName = Fields.StateOfCharge;
				data.Columns[1].ColumnName = Fields.MaxChargeCurrent;
				data.Columns[2].ColumnName = Fields.MaxDischargeCurrent;
			}
			return new MaxCurrentMap(data.Rows.Cast<DataRow>().Select(row => new MaxCurrentMap.MaxCurrentEntry() {
				SoC = row.ParseDouble(Fields.StateOfCharge) / 100,
				MaxChargeCurrent = row.ParseDouble(Fields.MaxChargeCurrent).SI<Ampere>(),
				MaxDischargeCurrent = -1 * row.ParseDouble(Fields.MaxDischargeCurrent).SI<Ampere>()

			}).OrderBy(e => e.SoC).ToArray());
		}

		public static class Fields
		{
			public const string StateOfCharge = "SoC";

			public const string MaxChargeCurrent = "I_charge";

			public const string MaxDischargeCurrent = "I_discharge";

		}

		public static MaxCurrentMap Create(Stream data)
		{
			return Create(VectoCSVFile.ReadStream(data));
		}
	}
}