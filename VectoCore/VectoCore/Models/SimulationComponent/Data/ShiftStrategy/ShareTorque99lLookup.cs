using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;

namespace TUGraz.VectoCore.InputData.Reader.ShiftStrategy {
	

	public class ShareTorque99lLookup : IShareTorque99lLookup
	{
		private KeyValuePair<MeterPerSecond, double>[] _entries;

		public ShareTorque99lLookup(KeyValuePair<MeterPerSecond, double>[] entries)
		{
			_entries = entries;
		}

		public double Lookup(MeterPerSecond velocity)
		{
			return _entries.Interpolate(x => x.Key.Value(), y => y.Value, velocity.Value());
		}
	}
}