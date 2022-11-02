using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class ElectricFullLoadCurveReader
	{
		public static ElectricMotorFullLoadCurve Read(Stream str, int count)
		{
			return Create(VectoCSVFile.ReadStream(str), count);
		}

		public static ElectricMotorFullLoadCurve Create(DataTable data, int count)
		{
			if (data.Columns.Count < 3) {
				throw new VectoException("Motor FullLoadCurve Data must contain at least 3 columns");
			}
			if (data.Rows.Count < 2) {
				throw new VectoException("Motor FullLoadCurve Data must contain at least 2 rows with numeric values");
			}

			if (!HeaderIsValid(data.Columns)) {
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.DrivingTorque;
				data.Columns[2].ColumnName = Fields.GenerationTorque;
			}

			return new ElectricMotorFullLoadCurve(
				(from DataRow row in data.Rows
				select new ElectricMotorFullLoadCurve.FullLoadEntry {
					MotorSpeed = row.ParseDouble(Fields.MotorSpeed).RPMtoRad(), // / ratio,
					FullDriveTorque = -row.ParseDouble(Fields.DrivingTorque).SI<NewtonMeter>() * count, // * ratio * efficiency,
					FullGenerationTorque = -row.ParseDouble(Fields.GenerationTorque).SI<NewtonMeter>() * count, //* ratio / efficiency
				}).ToList());
		}

		private static bool HeaderIsValid(DataColumnCollection dataColumns)
		{
			return dataColumns.Contains(Fields.MotorSpeed) && dataColumns.Contains(Fields.DrivingTorque) &&
					dataColumns.Contains(Fields.GenerationTorque);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n";

			public const string DrivingTorque = "T_drive";

			public const string GenerationTorque = "T_recuperation";
		}
	}
}
