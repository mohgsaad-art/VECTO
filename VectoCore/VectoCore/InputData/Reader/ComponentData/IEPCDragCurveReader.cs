using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class IEPCDragCurveReader
	{
		public static DragCurve Create(DataTable data, int count, double ratio)
		{
			if (data.Columns.Count < 2) {
				throw new VectoException("Drag Curve must contain at least 2 columns", ErrorCodes.DATATABLE_WRONG_COLUMNS,
					$"Make sure the IEPC Drag Curve file contains at least 2 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("Drag Curve must contain at least 2 rows with numeric values", ErrorCodes.DATATABLE_INSUFFICIENT_DATA,
					$"Make sure the IEPC Drag Curve file contains at least 2 rows with numeric values.");
			}

			if (!HeaderIsValid(data.Columns)) {
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.DragTorque;
			}

			var entries = data.AsEnumerable().Cast<DataRow>().Select(x => new DragCurve.DragLoadEntry() {
				MotorSpeed = x.ParseDouble(Fields.MotorSpeed).RPMtoRad() * ratio,
				DragTorque = -x.ParseDouble(Fields.DragTorque).SI<NewtonMeter>() * count / ratio // / efficiency
			}).OrderBy(x => x.MotorSpeed).ToList();
			var invalid = entries.Where(x => x.DragTorque.IsSmaller(0)).ToList();
			if (invalid.Count > 0) {
				throw new VectoException($"Drag torque has to be negative in input: {invalid.Select(x => $"{x.MotorSpeed.AsRPM} rpm").Join()}", ErrorCodes.DATATABLE_INVALID_DATA,
					$"Make sure the IEPC Drag Curve file contains only negative drag torque values.");
			}
			var duplicates = entries.GroupBy(x => x.MotorSpeed).Where(g => g.Count() > 1).Select(x => x.Key.AsRPM / ratio).ToList();
			if (duplicates.Any()) {
				throw new VectoException(
					$"Drag curve contains multiple entries for a single output speed: {duplicates.Join()}", ErrorCodes.DATATABLE_DUPLICATE_ENTRY,
					$"Make sure the IEPC Drag Curve file does not contain multiple entries for a single output speed.");
			}
            return new DragCurve(entries);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.MotorSpeed) && columns.Contains(Fields.DragTorque);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n_out";

			public const string DragTorque = "T_drag_out";
		}
	}
}