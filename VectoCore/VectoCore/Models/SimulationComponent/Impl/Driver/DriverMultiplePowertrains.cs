using TUGraz.VectoCore.Models.Simulation;
using DriverData = TUGraz.VectoCore.Models.SimulationComponent.Data.DriverData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class DriverMultiplePowertrains : Driver
    {
        
        public DriverMultiplePowertrains(IVehicleContainer container, DriverData driverData, IDriverStrategy strategy) :
            base(container)
        {
            
            Init(container, driverData, strategy);
        }
        
    }

    public class DriverNoSwitchableGearbox : DriverMultiplePowertrains
    {
        public DriverNoSwitchableGearbox(IVehicleContainer container, DriverData driverData, IDriverStrategy strategy) : base(container, driverData, strategy)
        {
        }
    }

    public class DriverSingleSwitchableGearbox : DriverMultiplePowertrains
    {
        public DriverSingleSwitchableGearbox(IVehicleContainer container, DriverData driverData, IDriverStrategy strategy) : base(container, driverData, strategy)
        {
        }
    }
    
    public class DriverMultipleSwitchableGearbox : DriverMultiplePowertrains
    {
        public DriverMultipleSwitchableGearbox(IVehicleContainer container, DriverData driverData, IDriverStrategy strategy) : base(container, driverData, strategy)
        {
        }
    }
}