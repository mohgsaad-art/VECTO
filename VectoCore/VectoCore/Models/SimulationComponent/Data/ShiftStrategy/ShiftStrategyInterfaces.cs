using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy
{
	public interface IAccelerationReserveLookup
	{
		MeterPerSquareSecond LookupLow(MeterPerSecond velocity);
		MeterPerSquareSecond LookupHigh(MeterPerSecond velocity);
	}

	public interface IShareTorque99lLookup
	{
		double Lookup(MeterPerSecond velocity);
	}

	public interface IPredictionDurationLookup
	{
		double Lookup(double speedRatio);
	}

	public interface IShareIdleLowLookup
	{
		double Lookup(MeterPerSecond velocity);
		double MaxValue { get; }
		double MinValue { get; }
	}

	public interface IEngineSpeedHighFactorLookup
	{
		double Lookup(double torqueRatio);
	}
}
