using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
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
            if (Gear < NextGear) {
                LastUpshift = absTime;
            }
            if (Gear > NextGear) {
                LastDownshift = absTime;
            }

            return base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
        }

    }
}
