using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class HybridControllerTimerunGear : HybridController
    {
		protected override int MaxRequestRetries => 20;
        
		public HybridControllerTimerunGear(IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es) :
            base(container, strategy, es)
        {
        }

        protected override void ApplyStrategySettings(Second absTime, HybridStrategyResponse strategySettings)
		{
			Gearbox.SwitchToNeutral = strategySettings.GearboxInNeutral;

			Engine.CombustionEngineOn = strategySettings.CombustionEngineOn;
			_electricMotorTorque = strategySettings.MechanicalAssistPower;

			var nextGear = new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.RightSample.Gear);
					
			if (DataBus.VehicleInfo.VehicleStopped && strategySettings.NextGear.Gear != 0) {
				_shiftStrategy.SetNextGear(nextGear);
			}
			
			strategySettings.ShiftRequired = DataBus.GearboxInfo.GearEngaged(absTime) && 
				(DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear != DataBus.DrivingCycleInfo.CycleData.RightSample.Gear);
			
			if (strategySettings.ShiftRequired) {
				_shiftStrategy.SetNextGear(nextGear);
			}
        }

        protected override AbstractResponse ShiftGear(HybridStrategyResponse strategySettings, bool dryRun, Second absTime, Second dt)
        {
			return null;
        }

        protected override GearshiftPosition GetNextGear(HybridStrategyResponse strategySettings)
        {
			return new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear, false);
		}

		protected override bool AdjustStrategyWhenExceedingGearMaxSpeed(bool dryRun, IResponse retVal, Second absTime, Second dt, 
			NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return false;
        }

    }
}
