using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ShiftLineSet
	{
		public const double DownhillSlope = -5;
		public const double UphillSlope = 5;

		public Dictionary<int, ShiftLines> LoadStages = new Dictionary<int, ShiftLines>();

		public PerSecond LookupShiftSpeed(
			int loadStage, Radian gradient, MeterPerSquareSecond acceleration, MeterPerSquareSecond aMin,
			MeterPerSquareSecond aMax)
		{
			if (!LoadStages.ContainsKey(loadStage)) {
				throw new VectoException("No Shiftlines for load stage {0} found", loadStage);
			}

			var shiftLinesSet = LoadStages[loadStage];

			//var slope = (Math.Tan(gradient.Value()) * 100).LimitTo(
			//	ATShiftStrategyVoith.DownhillSlope, ATShiftStrategyVoith.UphillSlope);

			gradient = gradient.LimitTo(
				VectoMath.InclinationToAngle(DownhillSlope),
				VectoMath.InclinationToAngle(UphillSlope));
			var shiftLine = shiftLinesSet.LookupShiftSpeed(gradient);
			var acc = aMin > aMax ? acceleration.LimitTo(aMax, aMin) : acceleration.LimitTo(aMin, aMax);

			var shiftSpeed = VectoMath.Interpolate(
				aMin, aMax, shiftLine.ShiftSpeedAMin, shiftLine.ShiftSpeedAMax, acc);

			return shiftSpeed;
		}
	}

	public class ShiftLines
	{

		internal readonly List<Tuple<Radian, PerSecond>> entriesAMin = new List<Tuple<Radian, PerSecond>>();
		internal readonly List<Tuple<Radian, PerSecond>> entriesAMax = new List<Tuple<Radian, PerSecond>>();


		public ShiftSpeedTuple LookupShiftSpeed(Radian gradent)
		{
			var sectLow = entriesAMin.GetSection(x => x.Item1 < gradent);
			var sectHigh = entriesAMax.GetSection(x => x.Item1 < gradent);

			return new ShiftSpeedTuple(
				VectoMath.Interpolate(sectLow.Item1.Item1, sectLow.Item2.Item1, sectLow.Item1.Item2, sectLow.Item2.Item2, gradent),
				VectoMath.Interpolate(
					sectHigh.Item1.Item1, sectHigh.Item2.Item1, sectHigh.Item1.Item2, sectHigh.Item2.Item2, gradent));
		}
	}

	public class ShiftSpeedTuple
	{
		public PerSecond ShiftSpeedAMin { get; }
		public PerSecond ShiftSpeedAMax { get; }

		public ShiftSpeedTuple(PerSecond shiftSpeedAMin, PerSecond shiftSpeedAMax)
		{
			ShiftSpeedAMin = shiftSpeedAMin;
			ShiftSpeedAMax = shiftSpeedAMax;
		}
	}
}