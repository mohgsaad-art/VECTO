using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class GearBoxHybridsMeasuredSpeedGear : CycleGearbox, IHybridControlledGearbox
    {
        public GearBoxHybridsMeasuredSpeedGear(IVehicleContainer container, VectoRunData runData) : base(container, runData)
        {
            LastDownshift = -double.MaxValue.SI<Second>();
			LastUpshift = -double.MaxValue.SI<Second>();
        }

        public override Second LastUpshift { get; protected internal set; }

		public override Second LastDownshift { get; protected internal set; }

        public override bool DisengageGearbox { get; set; }

        public bool SwitchToNeutral
		{
			set => Disengaged = value ? DataBus.AbsTime : null;
		}

        protected override PerSecond CalculateInAngularSpeed(PerSecond outAngularVelocity)
		{ 
			return outAngularVelocity * ((Gear.Gear == 0) ? 0 : ModelData.Gears[Gear.Gear].Ratio);
		}

        protected override IResponse EngineIdleRequest(Second absTime, Second dt)
        {
            var pos = DataBus.PowertrainInfo.ElectricMotorPositions[0];
            var useEMSpeed = (pos == PowertrainPosition.HybridP2_5 || pos == PowertrainPosition.HybridP2);
            
            return NextComponent.Request(
                absTime, 
                dt, 
                0.SI<NewtonMeter>(), 
                useEMSpeed 
                    ? DataBus.ElectricMotorInfo(pos).ElectricMotorSpeed * ((NextGear.Gear == 0) ? 0 : ModelData.Gears[NextGear.Gear].Ratio)
                    : DataBus.EngineInfo.EngineIdleSpeed, 
                false);
        }

        protected override uint GetGearFromCycle()
        {
            return DataBus.DrivingCycleInfo.CycleData.RightSample.Gear;
        }

		protected override IResponse HandleDryRunRequest(
			Second absTime, Second dt, NewtonMeter inTorque,
			PerSecond inAngularVelocity)
		{
			var dryRunResponse = base.HandleDryRunRequest(absTime, dt, inTorque, inAngularVelocity);
            
            dryRunResponse.Gearbox.Gear = Gear;
            dryRunResponse.Gearbox.InputSpeed = inAngularVelocity;
            dryRunResponse.Gearbox.InputTorque = inTorque;

            return dryRunResponse;
        }

        public override bool GearEngaged(Second absTime)
        {
            return (GetGearFromCycle() != 0);
        }

        public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun = false)
        {
            if (!DataBus.VehicleInfo.VehicleStopped) {
                var nextStepGear = new GearshiftPosition(
                    DataBus.DrivingCycleInfo.CycleData.RightSample.Gear,
                    !DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive);

                if (Gear < nextStepGear) {
                    LastUpshift = absTime;
                }
                if (Gear > nextStepGear) {
                    LastDownshift = absTime;
                }
            }
            else {
            	LastDownshift = -double.MaxValue.SI<Second>();
				LastUpshift = -double.MaxValue.SI<Second>();
			}

            if (PreviousState.DrivingBehavior != CurrentState.DrivingBehavior) {
				Log.Debug("driving action changed...");
				if (CurrentState.DrivingBehavior == DrivingBehavior.Driving ||
					CurrentState.DrivingBehavior == DrivingBehavior.Braking) {
					if (GearEngaged(absTime)) {
						Log.Debug("resetting gearshift time interval");
						LastDownshift = -double.MaxValue.SI<Second>();
						LastUpshift = -double.MaxValue.SI<Second>();
					} else {
						CurrentState.DrivingBehavior = PreviousState.DrivingBehavior;
					}
				}
			}

            var response = base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);

            response.Gearbox.Gear = Gear;
            response.Gearbox.InputSpeed = outAngularVelocity * ((Gear.Gear > 0)  ? ModelData.Gears[Gear.Gear].Ratio : 0);
            response.Gearbox.OutputSpeed = outAngularVelocity;
            response.Gearbox.OutputTorque = outTorque;

            return response;
        }

    }
}
