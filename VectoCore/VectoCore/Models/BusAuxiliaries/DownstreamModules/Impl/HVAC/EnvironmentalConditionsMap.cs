using System.Collections.Generic;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.BusAuxiliaries;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class EnvironmentalConditionsMap : IEnvironmentalConditionsMap
	{
		private readonly IReadOnlyList<IEnvironmentalConditionsMapEntry> _map;

		public EnvironmentalConditionsMap(IList<IEnvironmentalConditionsMapEntry> entries)
		{
			_map = new ReadOnlyCollection<IEnvironmentalConditionsMapEntry>(entries);
		}

		
		public IReadOnlyList<IEnvironmentalConditionsMapEntry> GetEnvironmentalConditions()
		{
			return _map;
		}
	}
}
