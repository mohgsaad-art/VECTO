using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;

namespace TUGraz.VectoCore.InputData.Reader.ShiftStrategy {


	public class ShareIdleLowLookup : IShareIdleLowLookup
	{
		private readonly KeyValuePair<MeterPerSecond, double>[] _entries;


		public ShareIdleLowLookup(KeyValuePair<MeterPerSecond, double>[] entries)
		{
			_entries = entries;
		}

		public double Lookup(MeterPerSecond velocity)
		{
			return _entries.Interpolate(x => x.Key.Value(), y => y.Value, velocity.Value());
		}

		public double MaxValue { get { return _entries.Max(x => x.Value); } }

		public double MinValue { get { return _entries.Min(x => x.Value); } }
	}
}