using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class SwitchableClutch : Clutch
    {
        public SwitchableClutch(IVehicleContainer container, CombustionEngineData engineData) : base(container, engineData) { }

        #region Overrides of Clutch

        public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            if (ClutchOpen) {
                IResponse response = new ResponseSuccess(this);
                if (outTorque * outAngularVelocity > 0) {
                    response = new ResponseOverload(this) { Delta = outTorque * outAngularVelocity };
                } else if (outTorque * outAngularVelocity < 0) {
                    response = new ResponseUnderload(this) { Delta = outTorque * outAngularVelocity };
                } else {
                    response = NextComponent.Initialize(0.SI<NewtonMeter>(), 0.RPMtoRad());
                }

                response.Clutch.OutputSpeed = outAngularVelocity;
                return response;
            }
            if (DataBus.EngineCtl.CombustionEngineOn)
                return base.Initialize(outTorque, outAngularVelocity);
            PreviousState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
            var retVal = NextComponent.Initialize(outTorque, outAngularVelocity);
            retVal.Clutch.OutputSpeed = outAngularVelocity;
            return retVal;
        }

        public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
        {
            var avgOutSpeed = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
            if (ClutchOpen) {
                if (dryRun) {
                    return new ResponseDryRun(this) {
                        DeltaFullLoad = avgOutSpeed * outTorque,
                        DeltaDragLoad = avgOutSpeed * outTorque,
                        DeltaFullLoadTorque = outTorque,
                        DeltaDragLoadTorque = outTorque,
                        Clutch = {
                            PowerRequest = avgOutSpeed * outTorque,
                            OutputSpeed = outAngularVelocity
                        }
                    };
                }
                var idleResponse = IdleController.Request(absTime, dt, 0.SI<NewtonMeter>(), null, false);
                CurrentState.SetState(0.SI<NewtonMeter>(), idleResponse.Engine.EngineSpeed, outTorque, outAngularVelocity);
                IResponse response = new ResponseSuccess(this);
                if ((outTorque * avgOutSpeed).IsGreater(0, Constants.SimulationSettings.LineSearchTolerance)) {
                    response = new ResponseOverload(this) { Delta = outTorque * avgOutSpeed };
                }
                if ((outTorque * avgOutSpeed).IsSmaller(0, Constants.SimulationSettings.LineSearchTolerance)) {
                    response = new ResponseUnderload(this) { Delta = outTorque * avgOutSpeed };
                }
                response.Engine.EngineSpeed = idleResponse.Engine.EngineSpeed;
                response.Clutch.PowerRequest = outTorque * avgOutSpeed;
                response.Clutch.OutputSpeed = outAngularVelocity;
                return response;
            }

            if (DataBus.EngineCtl.CombustionEngineOn) {
                if (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted && !ClutchOpen) {
                    //return HandleClutchClosed(absTime, dt, outTorque, outAngularVelocity, dryRun);
                    return HandleClutchOpen(absTime, dt, outTorque, outAngularVelocity, dryRun);
                }
                return base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);

            }

            var inAngularVelocity = 0.RPMtoRad();
            var retVal = NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);

            //if (retVal is ResponseEngineSpeedTooLow)
            //{
            //	var delta = ((ResponseEngineSpeedTooLow)retVal).DeltaEngineSpeed;
            //	var engineAngularVelocity = SearchAlgorithm.Search(outAngularVelocity, delta, delta * 0.1,
            //		result => ((ResponseDryRun)result).DeltaEngineSpeed,
            //		engineSpeed => NextComponent.Request(absTime, dt, outTorque, engineSpeed, true),
            //		result => ((ResponseDryRun)result).DeltaEngineSpeed.Value() * 1e3);

            //	retVal = NextComponent.Request(absTime, dt, outTorque, engineAngularVelocity, dryRun);
            //}

            if (!dryRun) {
                CurrentState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
                var avgInAngularVelocity = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2;
                var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2;
                var clutchLoss = outTorque * (avgInAngularVelocity - avgOutAngularVelocity);
                CurrentState.ClutchLoss = clutchLoss;
            }
            retVal.Clutch.PowerRequest = outTorque * avgOutSpeed;
            retVal.Clutch.OutputSpeed = outAngularVelocity;
            return retVal;
        }

        protected override void AddClutchLoss(NewtonMeter torque, PerSecond angularVelocity, bool allowSlipping, out NewtonMeter torqueIn,
            out PerSecond angularVelocityIn)
        {
            if (ClutchOpen) {
                angularVelocityIn = _idleSpeed;
                torqueIn = 0.SI<NewtonMeter>();
                return;
            }

            torqueIn = torque;
            angularVelocityIn = angularVelocity;

            var engineSpeedNorm = (angularVelocity - _idleSpeed) / (_ratedSpeed - _idleSpeed);
            if (allowSlipping && engineSpeedNorm < Constants.SimulationSettings.ClutchClosingSpeedNorm) {
                var engineSpeed = VectoMath.Max(_idleSpeed, angularVelocity);
                angularVelocityIn = _clutchSpeedSlippingFactor * engineSpeed + _idleSpeed;
            }
        }

        public bool ClutchOpen { get; set; }

        #region Overrides of Clutch

        //public override bool ClutchClosed(Second absTime) { return !ClutchOpen; } 

        #endregion

        #endregion

        #region Implementation of IUpdateable

        protected override bool DoUpdateFrom(object other)
        {
            if (other is SwitchableClutch c) {
                PreviousState = c.PreviousState.Clone();
                ClutchOpen = c.ClutchOpen;
                return true;
            }
            return false;
        }

        #endregion
    }
}