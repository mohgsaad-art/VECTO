using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class MultipleGearboxesDriver : Driver
    {
        public MultipleGearboxesDriver(IVehicleContainer container, DriverData driverData, IDriverStrategy strategy) : base(container, driverData, strategy)
        {
        }
    }
}