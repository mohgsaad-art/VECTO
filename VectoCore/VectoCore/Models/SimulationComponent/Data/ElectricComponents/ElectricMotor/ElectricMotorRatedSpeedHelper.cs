using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor
{
	public static class ElectricMotorRatedSpeedHelper
	{
		private class FullLoadCurveEntry
		{
			public PerSecond speed;
			public NewtonMeter torque;
			public Watt power;
		}

		public static PerSecond GetRatedSpeed<TEntry>(IEnumerable<TEntry> entries,
			Func<TEntry, PerSecond> getSpeed, Func<TEntry, NewtonMeter> getTorque)
		{
			var enumEntries = entries as TEntry[] ?? entries.ToArray();
			AssertDifferentSpeeds(enumEntries.Select(getSpeed));
			AssertZeroRPM(enumEntries.Select(getSpeed));
			//speed power
			var orderedEntries = enumEntries.OrderBy(getSpeed).Select(e => new FullLoadCurveEntry() {
				speed = getSpeed(e),
				torque = getTorque(e),
				power = getSpeed(e) * getTorque(e),
			}).ToArray();

			

			double[] gradients = new double[orderedEntries.Length];
			gradients[0] = 0;
			double[] deltaStartGradient = new double[orderedEntries.Length];
			deltaStartGradient[0] = 0;

			for (var i = 1; i < orderedEntries.Length; i++) {
				gradients[i] = (orderedEntries[i].power - orderedEntries[i - 1].power).Value() /
								(orderedEntries[i].speed - orderedEntries[i - 1].speed).Value();
				if (gradients[1].IsEqual(0)) {
					throw new VectoException("Invalid map", ErrorCodes.ELECTRICMOTOR_INVALID_RATED_SPEED);
				}
				deltaStartGradient[i] = gradients[i] / gradients[1] - 1;
				if(Math.Abs(deltaStartGradient[i]) > Constants.EMFullLoadCurveSettings.RatedSpeedGradientDelta) {
					return orderedEntries[i - 1].speed;
				}
			}
			throw new VectoException("Rated speed not found", ErrorCodes.ELECTRICMOTOR_RATED_SPEED_NOT_FOUND);
			
		}

		private static void AssertDifferentSpeeds(IEnumerable<PerSecond> speeds)
		{
			var speedsArray = speeds as PerSecond[] ?? speeds.ToArray();
			HashSet<PerSecond> difSpeeds = new HashSet<PerSecond>(speedsArray);
			if (difSpeeds.Count != speedsArray.Count()) {
				throw new VectoException("Only one entry per speed allowed", ErrorCodes.ELECTRICMOTOR_INVALID_RATED_SPEED);
			}
		}

		private static void AssertZeroRPM(IEnumerable<PerSecond> speeds)
		{
			if (!speeds.Min().IsEqual(0)) {
				throw new VectoException("Entries at 0 rpm missing", ErrorCodes.ELECTRICMOTOR_INVALID_RATED_SPEED);
			}
		}
	}
}