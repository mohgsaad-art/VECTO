using System.Linq;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class MeasuredSpeedGearHybridController : HybridController
    {
		public MeasuredSpeedGearHybridController(IVehicleContainer container, IPowertrainContainer powertrainContainer, IHybridControlStrategy strategy, IElectricSystem es) :
            base(container, powertrainContainer, strategy, es)
        {}

        protected override void ApplyStrategySettings(HybridStrategyResponse strategySettings)
		{
			Gearbox.SwitchToNeutral = strategySettings.GearboxInNeutral;

			Engine.CombustionEngineOn = strategySettings.CombustionEngineOn;
			_electricMotorTorque = strategySettings.MechanicalAssistPower;

			var nextGear = new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.RightSample.Gear,
				!DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive);
					
			if (DataBus.VehicleInfo.VehicleStopped && strategySettings.NextGear.Gear != 0) {
				_shiftStrategy.SetNextGear(nextGear);
			}

			var gearbox = DataBus.GearboxesInfo.First(x => x.AxleNumber == AxleNumber);
			strategySettings.ShiftRequired = gearbox.GearEngaged(DataBus.AbsTime) && 
				(DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear != DataBus.DrivingCycleInfo.CycleData.RightSample.Gear);
			
			if (strategySettings.ShiftRequired) {
				_shiftStrategy.SetNextGear(nextGear);
			}
        }

        protected override AbstractResponse ShiftGear(HybridStrategyResponse strategySettings, bool dryRun, Second absTime, Second dt, out bool retry)
		{
			retry = false;
			return null;
        }

        protected override GearshiftPosition GetNextGear(HybridStrategyResponse strategySettings)
        {
			return new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.RightSample.Gear,
				!DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive);
		}

		protected override bool AdjustStrategyWhenExceedingGearMaxSpeed(bool dryRun, IResponse retVal, Second absTime, Second dt, 
			NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return false;
        }

    }
}
