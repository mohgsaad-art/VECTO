using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IActuations
	{
		int Braking { get; }
		Second CycleTime { get; }
		int Kneeling { get; }
		int ParkBrakeAndDoors { get; }
	}
}