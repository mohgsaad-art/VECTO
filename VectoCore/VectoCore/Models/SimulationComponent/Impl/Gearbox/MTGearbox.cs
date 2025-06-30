using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class MTGearbox : AbstractAMTGearbox, IMTGearbox
	{
		public MTGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container, strategy) { }

		protected MTGearbox(IVehicleContainer container, IShiftStrategy strategy, bool dummy) : base(container, strategy, false) { }

	}
}