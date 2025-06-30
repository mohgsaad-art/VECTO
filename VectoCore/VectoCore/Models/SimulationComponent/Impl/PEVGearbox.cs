using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class PEVGearbox : AbstractAMTGearbox, IPEVGearbox
    {
		public PEVGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container, strategy)
		{
			_gear = new GearshiftPosition(0);
		}

	}
}