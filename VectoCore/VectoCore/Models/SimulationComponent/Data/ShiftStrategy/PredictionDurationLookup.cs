using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy
{
	public class PredictionDurationLookup : IPredictionDurationLookup
	{
		private readonly KeyValuePair<double, double>[] _entries;

		protected internal PredictionDurationLookup(KeyValuePair<double, double>[] entries)
		{
			_entries = entries;
		}

		public double Lookup(double speedRatio)
		{
			return _entries.Interpolate(x => x.Key, y => y.Value, speedRatio);
		}
	}
}
