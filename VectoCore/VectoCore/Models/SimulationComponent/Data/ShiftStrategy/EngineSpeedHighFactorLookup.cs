using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;

namespace TUGraz.VectoCore.InputData.Reader.ShiftStrategy
{
	public class EngineSpeedHighFactorLookup : IEngineSpeedHighFactorLookup
	{
		private KeyValuePair<double, double>[] _entries;

		public EngineSpeedHighFactorLookup(KeyValuePair<double, double>[] entries)
		{
			_entries = entries;
		}

		public double Lookup(double torqueRatio)
		{
			return _entries.Interpolate(x => x.Key, y => y.Value, torqueRatio);
		}
	}
}
