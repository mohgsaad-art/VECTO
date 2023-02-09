using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class MeasuredSpeedHybridsGearbox : Gearbox
    {
        public MeasuredSpeedHybridsGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container, strategy) 
        {}

        protected override void DoNotEngageWhenBraking(NewtonMeter outTorque, Second absTime, Second dt, PerSecond outAngularVelocity)
        {
            if ((DataBus.PowertrainInfo.ElectricMotorPositions[0] == PowertrainPosition.HybridP3 || 
				DataBus.PowertrainInfo.ElectricMotorPositions[0] == PowertrainPosition.HybridP4)
				&& (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking)
				&& Disengaged
				&& ShouldNotEngage(outTorque, outAngularVelocity, dt)) {
				
				EngageTime = absTime + dt;
				_overrideDisengage = null;
            }
        }

        private bool ShouldNotEngage(NewtonMeter outTorque, PerSecond outAngularVelocity, Second dt)
		{
			//Do not allow gear engagement below idle speed and below clutch disengage threshold.

			var gear = Disengaged ? NextGear.Gear : Gear.Gear;
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var inTorqueLossResult = ModelData.Gears[gear].LossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			if (avgOutAngularVelocity.IsEqual(0, 1e-9)) {
				inTorqueLossResult.Value = 0.SI<NewtonMeter>();
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
			var avgInAngularVelocity = (PreviousState.InAngularVelocity + inAngularVelocity) / 2.0;
			var inTorque = !avgInAngularVelocity.IsEqual(0)
				? outTorque * (avgOutAngularVelocity / avgInAngularVelocity)
				: outTorque / ModelData.Gears[Gear.Gear].Ratio;
			inTorque += inTorqueLossResult.Value;
			
			var inertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
				? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
				avgOutAngularVelocity
				: 0.SI<NewtonMeter>();
			inTorque += inertiaTorqueLossOut / ModelData.Gears[gear].Ratio;

			var driverDeceleratingNegTorque = DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking &&
											DataBus.DriverInfo.DrivingAction == DrivingAction.Brake &&
											(DataBus.DrivingCycleInfo.RoadGradient.IsSmaller(0) ||
											(ICEAvailable && inAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineIdleSpeed))) &&
											(DataBus.Brakes.BrakePower.IsGreater(0) || inTorque.IsSmaller(0));
			var vehiclespeedBelowThreshold =
				DataBus.VehicleInfo.VehicleSpeed.IsSmaller(Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed);

			return driverDeceleratingNegTorque && vehiclespeedBelowThreshold;
        }
    }

}
