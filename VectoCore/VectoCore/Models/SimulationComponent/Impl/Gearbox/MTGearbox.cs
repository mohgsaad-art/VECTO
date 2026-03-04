using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class MTGearbox : AbstractAMTGearbox, IMTGearbox
	{
		public MTGearbox(IVehicleContainer container, IPowertrainContainer powertrainContainer, IShiftStrategy strategy) : base(container, powertrainContainer, strategy) { }

		protected MTGearbox(IVehicleContainer container, IPowertrainContainer powertrainContainer, IShiftStrategy strategy, bool dummy) : base(container, powertrainContainer, strategy, false) { }

	}
}