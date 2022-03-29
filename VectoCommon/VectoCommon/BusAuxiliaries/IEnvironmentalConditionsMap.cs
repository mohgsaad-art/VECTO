using System.Collections.Generic;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IEnvironmentalConditionsMap
	{
	
		IReadOnlyList<IEnvironmentalConditionsMapEntry> GetEnvironmentalConditions();
	}
}
