using System.Data;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.GenericModelData
{

	public class RatedPoint
	{
		public Watt PRated { get; }
		public PerSecond NRated { get; }
		public NewtonMeter TRated { get; }
		
		public RatedPoint(PerSecond nRated, NewtonMeter tRated)
		{
			NRated = nRated;
			TRated = tRated;
			PRated = GetPRated();
		}

		private Watt GetPRated()
		{
			return NRated * TRated;
		}
	}


	public static class GenericRatedPointHelper
	{
		//struct FullLoadCurveEntry
		//{
		//	public PerSecond MotorSpeed { get; }
		//	public NewtonMeter TorqueDrive { get; }
		//	public NewtonMeter TorqueDrag { get; }
		//	public Watt PowerDrive { get; }

		//	public FullLoadCurveEntry(PerSecond motorSpeed, NewtonMeter torqueDrive, NewtonMeter torqueDrag)
		//	{
		//		MotorSpeed = motorSpeed;
		//		TorqueDrive = torqueDrive;
		//		TorqueDrag = torqueDrag;
		//		PowerDrive = motorSpeed * torqueDrive * Math.PI / 30000;
		//	}
		//}


		//struct SlopeValueEntry
		//{
		//	public double Slope { get; }
		//	public double Delta { get; }

		//	public SlopeValueEntry(double slope, double delta)
		//	{
		//		Slope = slope;
		//		Delta = delta;
		//	}
		//}


		#region Constants

		public const string MotorSpeedColumn = "outShaftSpeed";
		public const string TorqueDriveColumn = "maxTorque";
		public const string TorqueDragColumn = "minTorque";

		#endregion

		//private static List<FullLoadCurveEntry> fullLoadCurveEntries;
		//private static List<SlopeValueEntry> slopeValueEntries;
		

		public static RatedPoint GetRatedPointOfFullLoadCurveAtEM(TableData fullLoadCurve)
		{
            var n = ElectricMotorRatedSpeedHelper.GetRatedSpeed(fullLoadCurve.AsEnumerable(),
                row => row.ParseDouble(MotorSpeedColumn).RPMtoRad(), row => row.ParseDouble(TorqueDriveColumn).SI<NewtonMeter>());

            var tDrive = fullLoadCurve.AsEnumerable()
                .Max(row => row.ParseDouble(TorqueDriveColumn).SI<NewtonMeter>());

            var tDrag = fullLoadCurve.AsEnumerable()
                .Min(row => row.ParseDouble(TorqueDragColumn).SI<NewtonMeter>());

            if (tDrive.IsGreaterOrEqual(-tDrag / 1.05))
            {
                return new RatedPoint(n, tDrive);
            }
            else
            {
                return new RatedPoint(n, -tDrag);
            }

            return new RatedPoint(n, tDrive);

            //SetCurveValues(fullLoadCurve);
            //var ratedIndex = FindRowOfRatedPoint();
            //var n = fullLoadCurveEntries[ratedIndex].MotorSpeed;
            //var tDrive = GetHighestTorque();

            //return new RatedPoint(n, tDrive);
        }

		public static RatedPoint GetRatedPointOfFullLoadCurveAtIEPC(TableData fullLoadCurve,
			double axleRatio, double gearRatio,
			double gearEfficiency, double axleEfficiency)
		{

			var n = ElectricMotorRatedSpeedHelper.GetRatedSpeed(fullLoadCurve.AsEnumerable(),
				row => row.ParseDouble(MotorSpeedColumn).RPMtoRad(), row => row.ParseDouble(TorqueDriveColumn).SI<NewtonMeter>());
			var tDrive = fullLoadCurve.AsEnumerable().Max(row => row.ParseDouble(TorqueDriveColumn).SI<NewtonMeter>());
			//SetCurveValues(fullLoadCurve);
			//var ratedIndex = FindRowOfRatedPoint();
			


			var nRated = GetNRatedAtIEPC(n, axleRatio, gearRatio);
			var tRated = GetTRatedAtIEPC(tDrive, gearRatio, gearEfficiency, axleRatio, axleEfficiency);

			return new RatedPoint(nRated, tRated);
		}

		//private static void SetCurveValues(TableData fullLoadCurve)
		//{
		//	fullLoadCurveEntries = new List<FullLoadCurveEntry>();
		//	slopeValueEntries = new List<SlopeValueEntry>();

		//	for (int r = 0; r < fullLoadCurve.Rows.Count; r++) {

		//		var motorSpeed = fullLoadCurve.Rows[r].ParseDouble(MotorSpeedColumn).SI<PerSecond>();
		//		var torqueDrive = fullLoadCurve.Rows[r].ParseDouble(TorqueDriveColumn).SI<NewtonMeter>();
		//		var torqueDrag = fullLoadCurve.Rows[r].ParseDouble(TorqueDragColumn).SI<NewtonMeter>();

		//		fullLoadCurveEntries.Add(new FullLoadCurveEntry(motorSpeed, torqueDrive, torqueDrag));

		//		if (r == 0)
		//			continue;
				
		//		var slopeValue = (fullLoadCurveEntries[r].PowerDrive.Value() - fullLoadCurveEntries[r - 1].PowerDrive.Value()) /
		//						 (fullLoadCurveEntries[r].MotorSpeed.Value() - fullLoadCurveEntries[r - 1].MotorSpeed.Value());

		//		var deltaValue = slopeValue / ((slopeValueEntries.Count == 0) ? slopeValue  : slopeValueEntries[0].Slope ) -1;

		//		slopeValueEntries.Add(new SlopeValueEntry(slopeValue, deltaValue));
		//	}
		//}
		

		//private static NewtonMeter GetHighestTorque()
		//{
		//	var value = double.MinValue;

		//	foreach (var entry in fullLoadCurveEntries)
		//	{
		//		var currentValue = entry.TorqueDrive.Value();
		//		if (value < currentValue)
		//			value = currentValue;
		//	}

		//	return value.SI<NewtonMeter>();
		//}


		//private static int FindRowOfRatedPoint()
		//{
		//	for (int i = 0; i < slopeValueEntries.Count; i++)
		//	{
		//		var deltaValue = slopeValueEntries[i].Delta;

		//		if (Math.Abs(deltaValue) > 0.2)
		//			return i;
		//	}

		//	return -1;
		//}


		private static PerSecond GetNRatedAtIEPC(PerSecond n, double axleRatio, double gearRatio)
		{
			return (n.Value() * axleRatio * gearRatio).SI<PerSecond>();
		}

		private static NewtonMeter GetTRatedAtIEPC(NewtonMeter tDrive, double gearRatio, double gearEfficiency,
			double axleRatio, double axleEfficiency)
		{
			return (tDrive.Value() / gearRatio / gearEfficiency / axleRatio / axleEfficiency).SI<NewtonMeter>();
		}
	}
}
