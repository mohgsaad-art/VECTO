using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class MeasuredSpeedHybridsCycleGearbox : CycleGearbox, IHybridControlledGearbox
    {
        public MeasuredSpeedHybridsCycleGearbox(IVehicleContainer container, VectoRunData runData) : base(container, runData)
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
            //Since the engine can be off, we back-calculate in angular speed from out angular speed and ratio, instead of setting it to engine idle speed.
			return outAngularVelocity * ((Gear.Gear == 0) ? 0 : ModelData.Gears[Gear.Gear].Ratio);
		}

        protected override IResponse EngineIdleRequest(Second absTime, Second dt)
        {
            //For EM positions: P2.5, P2, IHPC, the next component is the electric motor, so we use the EM speed in the request.
            var pos = DataBus.PowertrainInfo.ElectricMotorPositions[0];
            var useEMSpeed = (pos == PowertrainPosition.HybridP2_5 || pos == PowertrainPosition.HybridP2 || pos == PowertrainPosition.IHPC);
            
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
            var response = base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);

            response.Gearbox.Gear = Gear;
            response.Gearbox.InputSpeed = CalculateInAngularSpeed(outAngularVelocity);
            response.Gearbox.OutputSpeed = outAngularVelocity;
            response.Gearbox.OutputTorque = outTorque;

            return response;
        }

    }
}
