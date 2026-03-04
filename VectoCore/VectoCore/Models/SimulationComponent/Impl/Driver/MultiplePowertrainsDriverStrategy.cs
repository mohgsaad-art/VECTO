using System;
using System.Linq;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public abstract class MultiplePowertrainsDriverStrategy : DefaultDriverStrategy
    {
        public MultiplePowertrainsDriverStrategy(IVehicleContainer container) : base(container)
        {
        }
    }
    
    public class MultiplePowertrainsDriverStrategyNoSwitchableGearbox : DefaultDriverStrategy
    {
        public override IPowertrainContainer PowertrainContainer => DataBus.Powertrains.Values.First();

        public MultiplePowertrainsDriverStrategyNoSwitchableGearbox(IVehicleContainer container) : base(container)
        {
        }
    }
    
    public class MultiplePowertrainsDriverStrategySingleSwitchableGearbox : DefaultDriverStrategy
    {
        public override IPowertrainContainer PowertrainContainer =>
            DataBus.Powertrains.Values.Where(p => p.GearboxInfo.NumGears > 1).Single();
        public MultiplePowertrainsDriverStrategySingleSwitchableGearbox(IVehicleContainer container) : base(container)
        {
        }
    }
    
    public class MultiplePowertrainsDriverStrategyMultipleSwitchableGearboxes : DefaultDriverStrategy
    {
        public override IPowertrainContainer PowertrainContainer =>
            throw new NotImplementedException("Driver Strategy for multiple switchable gearboxes not implemented");
        public MultiplePowertrainsDriverStrategyMultipleSwitchableGearboxes(IVehicleContainer container) : base(container)
        {
        }
    }
}