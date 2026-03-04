using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class PEVGearbox : AbstractAMTGearbox, IPEVGearbox
    {
		public PEVGearbox(IVehicleContainer container, IPowertrainContainer powertrainContainer, IShiftStrategy strategy) : base(container, powertrainContainer, strategy)
		{
			_gear = new GearshiftPosition(0);
		}

	}
}