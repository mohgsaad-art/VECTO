using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	// Reflects stored data in pesisted CombinedAlternator Map .AALT
	public interface ICombinedAlternatorMapRow
	{
		string AlternatorName { get;  }
		PerSecond RPM { get;  }
		Ampere Amps { get;  }
		double Efficiency { get;  }
		double PulleyRatio { get;  }
	}
}
