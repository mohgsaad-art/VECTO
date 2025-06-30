using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	/// <summary>
    /// Gearbox for Automatic Power Transmission - No Torque Converter.
    /// </summary>
    public class APTNGearbox : AbstractAMTGearbox, IAPTNGearbox
    {
        public APTNGearbox(IVehicleContainer container, IShiftStrategy strategy) : this(container, strategy, false)
        {
            if (container.IsTestPowertrain) {
                throw new VectoException(
                    "This class shall not be used in a testpowertrain - use the dedicated class instead!");
            }
        }

        protected APTNGearbox(IVehicleContainer container, IShiftStrategy strategy, bool dummy) : base(container,
            strategy, dummy)
        {
            // common initialization goes here
        }


        public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            var absTime = 0.SI<Second>();
            var dt = Constants.SimulationSettings.TargetTimeInterval;

            EngageTime = -double.MaxValue.SI<Second>();

            if (_strategy != null && (Disengaged || DisengageGearbox)) {
                Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
            }

            var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio;
            var gearboxTorqueLoss = ModelData.Gears[Gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
            CurrentState.TorqueLossResult = gearboxTorqueLoss;

            var inTorque = outTorque / ModelData.Gears[Gear.Gear].Ratio
                            + gearboxTorqueLoss.Value;

            PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
            PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
            PreviousState.Gear = Gear;
            Disengaged = false;

            var response = NextComponent.Initialize(inTorque, inAngularVelocity);

            return response;
        }

        public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun = false)
        {
            var response = base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
            if (response is ResponseGearShift) {
                response = base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
            }
            return response;
        }

    }
}