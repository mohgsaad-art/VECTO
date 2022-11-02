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
				throw new VectoException("Drag Curve must contain at least 2 columns");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException("Drag Curve must contain at least 2 rows with numeric values");
			}

			if (!HeaderIsValid(data.Columns)) {
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.DragTorque;
			}
			return new DragCurve(data.AsEnumerable().Cast<DataRow>().Select(x => new DragCurve.DragLoadEntry() {
				MotorSpeed = x.ParseDouble(Fields.MotorSpeed).RPMtoRad(), // / ratio,
				DragTorque = -x.ParseDouble(Fields.DragTorque).SI<NewtonMeter>() * count, // * ratio / efficiency
			}).ToList());
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