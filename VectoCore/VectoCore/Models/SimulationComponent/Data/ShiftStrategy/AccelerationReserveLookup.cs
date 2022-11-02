using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy {
	public class AccelerationReserveLookup : IAccelerationReserveLookup
	{
		private KeyValuePair<MeterPerSecond, Tuple<MeterPerSquareSecond, MeterPerSquareSecond>>[] _entries;

		public AccelerationReserveLookup(KeyValuePair<MeterPerSecond, Tuple<MeterPerSquareSecond, MeterPerSquareSecond>>[] entries)
		{
			_entries = entries;
		}

		public MeterPerSquareSecond LookupLow(MeterPerSecond velocity)
		{
			return _entries.Interpolate(x => x.Key.Value(), y => y.Value.Item1, velocity.Value());
		}

		public MeterPerSquareSecond LookupHigh(MeterPerSecond velocity)
		{
			return _entries.Interpolate(x => x.Key.Value(), y => y.Value.Item2, velocity.Value());
		}
	}
}