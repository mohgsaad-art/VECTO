using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IEnvironmentalConditionsMapEntry
	{
		Kelvin Temperature { get; }
		WattPerSquareMeter Solar { get; }

		// already normalized weighting factor
		double Weighting { get; }


		//double GetNormalisedWeighting(IList<IEnvironmentalConditionsMapEntry> map);
	}
}
