using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries {
	public class Actuations : IActuations
	{
		public int Braking { get; internal set; }

		public int ParkBrakeAndDoors { get; internal set; }

		public int Kneeling { get; internal set; }

		public Second CycleTime { get; internal set; }
	}
}