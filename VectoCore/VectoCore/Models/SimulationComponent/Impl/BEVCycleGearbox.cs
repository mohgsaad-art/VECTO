using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class BEVCycleGearbox : CycleGearbox
    {
        public BEVCycleGearbox(IVehicleContainer container, VectoRunData runData) : base(container, runData)
        {}

        protected override IResponse GetDisengagedResponse(Second absTime, Second dt, PerSecond outAngularVelocity)
        {
            var outVelocity = outAngularVelocity * ((NextGear.Gear > 0) ? ModelData.Gears[NextGear.Gear].Ratio : 1);
            
            return NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), outVelocity, false);
        }
    }
}
