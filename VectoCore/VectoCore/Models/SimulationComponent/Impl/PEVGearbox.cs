using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class PEVGearbox : AbstractAMTGearbox, IPEVGearbox
    {
		public PEVGearbox(IVehicleContainer container, IShiftStrategy strategy, int axleNumber) : base(container, strategy, axleNumber)
		{
			_gear = new GearshiftPosition(0);
		}

	}
}