using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IPneumaticsConsumersDemand
	{
		double OverrunUtilisationForCompressionFraction { get; }

		//NormLiterPerKilogram BrakingWithRetarderNIperKG { get;  }
		NormLiterPerKilogram Braking { get; }

		NormLiterPerKilogramMeter BreakingWithKneeling { get; }
		NormLiter DoorOpening { get; }
		NormLiterPerKilogram StopBrakeActuation { get; }
		NormLiterPerSecond AirControlledSuspension { get; }
		NormLiterPerSecond AdBlueInjection { get; }
		double NonSmartRegenFractionTotalAirDemand { get; }
		double SmartRegenFractionTotalAirDemand { get; }

		NormLiter DeadVolume { get; }

		// Nl / Nl / h => 1/h
		PerSecond DeadVolBlowOuts { get; }
	}
}
