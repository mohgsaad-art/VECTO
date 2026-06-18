using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class ElectricMotorDragCurveReader
	{
		public static DragCurve Create(DataTable data, int count)
		{
			if (data.Columns.Count < 2) {
				throw new VectoException("Drag Curve must contain at least 2 columns", ErrorCodes.DATATABLE_WRONG_COLUMNS);
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("Drag Curve must contain at least 2 rows with numeric values", ErrorCodes.DATATABLE_INSUFFICIENT_DATA);
			}

			if (!HeaderIsValid(data.Columns)) {
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.DragTorque;
			}

			var entries = data.AsEnumerable().Cast<DataRow>().Select(x => new DragCurve.DragLoadEntry() {
				MotorSpeed = x.ParseDouble(Fields.MotorSpeed).RPMtoRad(), // / ratio,
				DragTorque = -x.ParseDouble(Fields.DragTorque).SI<NewtonMeter>() * count, // * ratio / efficiency
			}).OrderBy(x => x.MotorSpeed).ToList();
			var invalid = entries.Where(x => x.DragTorque.IsSmaller(0)).ToList();
			if (invalid.Count > 0) {
				throw new VectoException($"Drag torque has to be negative in input: {invalid.Select(x => x.MotorSpeed.AsRPM).Join()}", ErrorCodes.DATATABLE_INVALID_DATA);
			}
            var duplicates = entries.GroupBy(x => x.MotorSpeed).Where(g => g.Count() > 1).Select(x => x.Key.AsRPM).ToList();
			if (duplicates.Any()) {
				throw new VectoException(
					$"Drag curve contains multiple entries for a single motor speed: {duplicates.Join()}", ErrorCodes.DATATABLE_DUPLICATE_ENTRY);
			}
            return new DragCurve(entries);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.MotorSpeed) && columns.Contains(Fields.DragTorque);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n";

			public const string DragTorque = "T_drag";
		}
	}
}