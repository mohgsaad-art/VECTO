using System.Collections.Generic;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	public interface ICombinedAlternator
	{
		// Alternators List
		List<IAlternator> Alternators { get; set; }

		// Test Equality
		bool IsEqualTo(ICombinedAlternator other);
	}
}
