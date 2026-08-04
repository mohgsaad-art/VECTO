using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class MTGearbox : AbstractAMTGearbox, IMTGearbox
	{
		public MTGearbox(IVehicleContainer container, IShiftStrategy strategy, int axleNumber) : base(container, strategy, axleNumber) { }

		protected MTGearbox(IVehicleContainer container, IShiftStrategy strategy, bool dummy, int axleNumber) : base(container, strategy, false, axleNumber) { }

	}
}