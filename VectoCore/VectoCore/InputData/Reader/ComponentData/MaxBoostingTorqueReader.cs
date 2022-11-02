using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class MaxBoostingTorqueReader
	{
		public static VehicleMaxPropulsionTorque Read(Stream str)
		{
			return Create(VectoCSVFile.ReadStream(str));
		}

		public static VehicleMaxPropulsionTorque Create(DataTable data)
		{
			if (data.Columns.Count < 2) {
				throw new VectoException("Vehicle max. propulsion data must contain at least 2 columns");
			}
			if (data.Rows.Count < 2) {
				throw new VectoException("Vehicle max. propulsion data must contain at least 2 rows with numeric values");
			}

			if (!HeaderIsValid(data.Columns)) {
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.DrivingTorque;
			}

			return new VehicleMaxPropulsionTorque(
				(from DataRow row in data.Rows
					select new VehicleMaxPropulsionTorque.FullLoadEntry {
						MotorSpeed = row.ParseDouble(Fields.MotorSpeed).RPMtoRad(),
						FullDriveTorque = row.ParseDouble(Fields.DrivingTorque).SI<NewtonMeter>()
					}).ToList());
		}

		private static bool HeaderIsValid(DataColumnCollection dataColumns)
		{
			return dataColumns.Contains(Fields.MotorSpeed) && dataColumns.Contains(Fields.DrivingTorque);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n";

			public const string DrivingTorque = "T_drive";
		}
	}
}