using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public interface IDriverMode
    {
        DefaultDriverStrategy DriverStrategy { get; set; }

        IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient);

        void ResetMode();
    }
    
    public abstract class AbstractDriverMode : LoggingObject, IDriverMode
    {
        private IDriverActions _driver;
        private DriverData _driverData;
        private IDataBus _dataBus;
        private IPowertrainContainer _powertrainContainer;
        
        
        public DefaultDriverStrategy DriverStrategy { get; set; }

        protected IPowertrainContainer PowertrainContainer =>
            _powertrainContainer ?? (_powertrainContainer = DriverStrategy.PowertrainContainer);
        protected IDriverActions Driver => _driver ?? (_driver = DriverStrategy.Driver);
        protected DriverData DriverData => _driverData ?? (_driverData = Driver.DriverData);
        protected IDataBus DataBus => _dataBus ?? (_dataBus = Driver.DataBus);
        
        public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
        {
            var response = DoHandleRequest(absTime, ds, targetVelocity, gradient);

            if (DriverStrategy.NextDrivingAction == null || !(response is ResponseSuccess))
            {
                return response;
            }

            // if we accelerate in the current simulation interval the ActionDistance of the next action
            // changes and we might pass the ActionDistance - check again...
            if (response.Driver.Acceleration <= 0)
            {
                return response;
            }

            // if the speed at the end of the simulation interval is below the next target speed 
            // we are fine (no need to brake right now)
            var v2 = DataBus.VehicleInfo.VehicleSpeed + response.Driver.Acceleration * response.SimulationInterval;
            if (v2 <= DriverStrategy.NextDrivingAction.NextTargetSpeed)
            {
                return response;
            }

            response = CheckRequestDoesNotExceedNextAction(absTime, ds, targetVelocity, gradient, response,
                out var newds);

            if (ds.IsEqual(newds, 1e-3.SI<Meter>()))
            {
                return response;
            }

            if (newds.IsSmallerOrEqual(0, 1e-3))
            {
                newds = ds / 2.0;

                //DriverStrategy.CurrentDrivingMode = DefaultDriverStrategy.DrivingMode.DrivingModeBrake;
                //DriverStrategy.BrakeTrigger = DriverStrategy.NextDrivingAction;
            }

            var newOperatingPoint = VectoMath.ComputeTimeInterval(
                DataBus.VehicleInfo.VehicleSpeed, response.Driver.Acceleration, DataBus.MileageCounter.Distance,
                newds);
            if (newOperatingPoint.SimulationInterval.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval))
            {
                // the next time interval will be too short, this may lead to issues with inertia etc. 
                // instead of accelerating, drive at constant speed.
                response = DoHandleRequest(absTime, ds, DataBus.VehicleInfo.VehicleSpeed, gradient, true);
                return response;
            }

            Log.Debug(
                "Exceeding next ActionDistance at {0}. Reducing max Distance from {2} to {1}",
                DriverStrategy.NextDrivingAction.ActionDistance, newds, ds);
            return new ResponseDrivingCycleDistanceExceeded(this)
            {
                MaxDistance = newds,
            };
        }

        protected abstract IResponse DoHandleRequest(
            Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
            bool prohibitOverspeed = false);

        protected abstract IResponse CheckRequestDoesNotExceedNextAction(
            Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient, IResponse response,
            out Meter newSimulationDistance);

        public abstract void ResetMode();
    }
    public class DriverModeDrive : AbstractDriverMode
    {
        public DriverModeDrive()
        {
            
        }

        protected override IResponse DoHandleRequest(Second absTime, Meter ds, MeterPerSecond targetVelocity,
            Radian gradient, bool prohibitOverspeed = false)
        {
            var debug = new DebugData();

            Driver.DriverBehavior = DrivingBehavior.Driving;
            var velocity = targetVelocity;
            if (DriverStrategy.IsOverspeedAllowed(targetVelocity, prohibitOverspeed))
            {
                velocity = DriverStrategy.ApplyOverspeed(velocity);
            }

            if (DataBus.GearboxesInfo.All(g => g.GearboxType.AutomaticTransmission()) ||
                (DataBus.ClutchesInfo.All(c => c.ClutchClosed(absTime)) &&
                 DataBus.GearboxesInfo.All(g => g.GearEngaged(absTime))))
            {
                if (DataBus.DrivingCycleInfo.CycleData.LeftSample.PTOActive == PTOActivity.PTOActivityRoadSweeping &&
                    targetVelocity < DriverStrategy.PTODriveMinSpeed)
                {
                    velocity = DriverStrategy.PTODriveMinSpeed;
                    targetVelocity = velocity;
                }

                for (var i = 0; i < 3; i++)
                {
                    var retVal = HandleRequestEngaged(absTime, ds, targetVelocity, gradient, prohibitOverspeed,
                        velocity, debug);
                    if (retVal != null)
                    {
                        return retVal;
                    }
                }

                throw new VectoException("HandleRequestEngaged found no operating point.", ErrorCodes.DRIVER_OPERATING_POINT_NOT_FOUND);
            }

            var response = HandleRequestDisengaged(absTime, ds, gradient, velocity, debug);
            if (response is ResponseDrivingCycleDistanceExceeded)
            {
                return response;
            }

            if (!(response is ResponseSuccess) && DataBus.ClutchesInfo.First().ClutchClosed(absTime))
            {
                response = HandleRequestEngaged(absTime, ds, targetVelocity, gradient, prohibitOverspeed, velocity,
                    debug);
            }

            return response;
        }

        private IResponse HandleRequestDisengaged(Second absTime, Meter ds, Radian gradient, MeterPerSecond velocity,
            DebugData debug)
        {
            if (DataBus.VehicleInfo.VehicleSpeed.IsSmallerOrEqual(0.SI<MeterPerSecond>()))
            {
                // the clutch is disengaged, and the vehicle stopped - we can't perform a roll action. wait for the clutch to be engaged
                // todo mk 2016-08-23: is this still needed?
                var remainingShiftTime = Constants.SimulationSettings.TargetTimeInterval;
                while (!DataBus.ClutchesInfo.First().ClutchClosed(absTime + remainingShiftTime))
                {
                    remainingShiftTime += Constants.SimulationSettings.TargetTimeInterval;
                }

                return new ResponseFailTimeInterval(this)
                {
                    DeltaT = remainingShiftTime,
                };
            }

            var response = Driver.DrivingActionRoll(absTime, ds, velocity, gradient);
            debug.Add("[DMD.HRD-0] ClutchOpen -> Roll", response);
            switch (response)
            {
                case ResponseUnderload _ when DataBus.ClutchesInfo.First().ClutchClosed(absTime):
                    response = HandleRequestEngaged(absTime, ds, velocity, gradient, false, velocity, debug);
                    break;
                case ResponseUnderload _ when !DataBus.ClutchesInfo.First().ClutchClosed(absTime):
                    response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient, response);
                    debug.Add("[DMD.HRD-1] Roll:Underload -> Brake", response);
                    break;
                case ResponseSpeedLimitExceeded _:
                    response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
                    debug.Add("[DMD.HRD-2] Roll:SpeedLimitExceeded -> Brake", response);
                    break;
            }

            return response;
        }

        private IResponse HandleRequestEngaged(
            Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
            bool prohibitOverspeed, MeterPerSecond velocityWithOverspeed, DebugData debug)
        {
            // drive along
            var first = FirstAccelerateOrCoast(absTime, ds, targetVelocity, gradient, prohibitOverspeed,
                velocityWithOverspeed, debug);

            var second = first;
            switch (first)
            {
                case ResponseUnderload _:
                    if (PowertrainContainer.GearboxInfo.GearboxType.AutomaticTransmission() &&
                        !DataBus.ClutchesInfo.First().ClutchClosed(absTime))
                    {
                        //TODO mk20210616 the whole statement could be de-nested to switch-pattern matching (with "where") if this first "if" would not be here.
                        var debugResponse = Driver.DrivingActionRoll(absTime, ds, velocityWithOverspeed, gradient);
                        debug.Add("[DMD.HRE-0] (Underload&Overspeed)->Roll", second);
                    }

                    if (DataBus.VehicleInfo.VehicleSpeed.IsGreater(0) &&
                        DriverStrategy.IsOverspeedAllowed(targetVelocity, prohibitOverspeed))
                    {
                        second = Driver.DrivingActionCoast(absTime, ds, velocityWithOverspeed, gradient);
                        debug.Add("[DMD.HRE-1] (Underload&Overspeed)->Coast", second);
                        second = HandleCoastAfterUnderloadWithOverspeed(absTime, ds, gradient, velocityWithOverspeed,
                            debug, second);
                    }
                    else
                    {
                        // overrideAction: in case of hybrids with an AT gearbox after an underload we search for braking power because the torque converter
                        // may be in creeping condition (e.g. engine propels but torque converter is below drag. -> engine propells and remaining power is 
                        // dissipated in brakes.
                        // in order that the hybrid controller still selects a solution where the EM propells as well, the brake action announces the 
                        // accelerate action.
                        // unfortunately, this causes issues for P1 hybrid configurations with AT gearbox in torque converter gear. If the EM propells, the torque
                        // converter cannot find an operating point close to the drag point. therefore, do not announce the different action except for driving off from
                        // standstill.
                        var gearbox = DataBus.GearboxesInfo.First();
                        var overrideAction = gearbox.GearboxType.AutomaticTransmission()
                                             && (gearbox.Gear.TorqueConverterLocked.HasValue &&
                                                 !gearbox.Gear.TorqueConverterLocked.Value)
                                             && (DataBus.ElectricMotorsInfo.Count(x =>
                                                     x.Position == PowertrainPosition.HybridP1) == 0 ||
                                                 DataBus.VehicleInfo.VehicleStopped)
                            ? DrivingAction.Accelerate
                            : (DrivingAction?)null;
                        second = Driver.DrivingActionBrake(absTime, ds, velocityWithOverspeed, gradient,
                            overrideAction: overrideAction);
                        debug.Add("[DMD.HRE-2] (Underload&!Overspeed)->Brake", second);
                    }

                    break;
                case ResponseEngineSpeedTooHigh _:
                    second = Driver.DrivingActionBrake(absTime, ds, targetVelocity, gradient, first);
                    break;
                case ResponseSpeedLimitExceeded _:
                    if (DriverStrategy.EcoRollState.State == EcoRollStates.EcoRollOn &&
                        PowertrainContainer.GearboxInfo.GearboxType.AutomaticTransmission() &&
                        DriverStrategy.ATEcoRollReleaseLockupClutch)
                    {
                        DriverStrategy.EcoRollState.State = EcoRollStates.EcoRollOff;
                        DataBus.GearboxesCtl.First(x => x.AxleNumber == AxleNumber.NOT_IN_AXLE_POWERTRAIN).DisengageGearbox = false;
                    }

                    second = Driver.DrivingActionBrake(absTime, ds, velocityWithOverspeed, gradient);
                    debug.Add("[DMD.HRE-3] SpeedLimitExceeded->Brake", second);
                    break;
            }

            if (second == null)
            {
                return null;
            }

            var third = second;

            switch (second)
            {
                case ResponseGearShift _:
                    third = Driver.DrivingActionRoll(absTime, ds, velocityWithOverspeed, gradient);
                    debug.Add("[DMD.HRE-4] second: GearShift -> Roll", third);
                    switch (third)
                    {
                        case ResponseOverload _:
                            third = Driver.DrivingActionCoast(absTime, ds, velocityWithOverspeed, gradient);
                            debug.Add("[DMD.HRE-5] third:Overload -> try again Coast", third);
                            break;
                        case ResponseUnderload _:
                            // underload may happen if driver limits acceleration when rolling downhill
                            third = Driver.DrivingActionBrake(absTime, ds, velocityWithOverspeed, gradient);
                            debug.Add("[DMD.HRE-6] third:Underload -> Brake", third);
                            break;
                        case ResponseSpeedLimitExceeded _:
                            third = Driver.DrivingActionBrake(absTime, ds, velocityWithOverspeed, gradient);
                            debug.Add("[DMD.HRE-7] third:SpeedLimitExceeded -> Brake", third);

                            if (third is ResponseOverload)
                            {
                                third = Driver.DrivingActionAccelerate(absTime, ds, velocityWithOverspeed, gradient);
                                debug.Add("[DMD.HRE-7-1] third:Overload after Brake after Gearshift --> Accelerate",
                                    third);
                            }

                            break;
                    }

                    break;
                case ResponseOverload _ when DataBus.VehicleInfo.VehicleSpeed.IsGreater(0):
                    third = Driver.DrivingActionCoast(absTime, ds, velocityWithOverspeed, gradient);
                    debug.Add("[DMD.HRE-8] second:Overload -> Coast", third);

                    switch (third)
                    {
                        case ResponseGearShift _:
                            third = Driver.DrivingActionCoast(absTime, ds, velocityWithOverspeed, gradient);
                            debug.Add("[DMD.HRE-9] third:GearShift -> try again Coast", third);
                            break;
                        case ResponseSpeedLimitExceeded _:
                            if (PowertrainContainer.GearboxInfo.GearboxType.AutomaticTransmission() &&
                                !PowertrainContainer.GearboxInfo.Gear.IsLockedGear())
                            {
                                third = Driver.DrivingActionCoast(absTime, ds,
                                    velocityWithOverspeed + 1.KMPHtoMeterPerSecond(), gradient);
                                debug.Add("[DMD.HRE-10] third:Overload (AT, Converter gear) -> Coast", third);
                            }
                            else
                            {
                                third = Driver.DrivingActionBrake(absTime, ds, velocityWithOverspeed, gradient);
                                debug.Add("[DMD.HRE-11] third:SpeedLimitExceeded -> Brake", third);
                            }

                            break;
                        case ResponseOverload _:
                            // for cases with IEPC while first is overspeed, second ist overload due to brake + downshift, third is coast with overload
                            // also for case with xEV having to accelerate before stop due to positive gradient
                            if (PowertrainContainer.GearboxInfo.GearboxType is GearboxType.APTN)
                            {
                                third = Driver.DrivingActionAccelerate(absTime, ds, velocityWithOverspeed, gradient);
                                debug.Add("[DMD.HRE-12] third:Overload (APTN,IEPC) -> Accelerate", third);
                            }

                            break;
                    }

                    break;
            }

            return third;
        }

        private IResponse HandleCoastAfterUnderloadWithOverspeed(Second absTime, Meter ds, Radian gradient,
            MeterPerSecond velocity, DebugData debug, IResponse second)
        {
            if (second is ResponseUnderload || second is ResponseSpeedLimitExceeded)
            {
                second = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
                debug.Add("[DMD.HCAUWO-0] second:(Underload|SpeedLimitExceeded) -> Brake", second);
            }

            if (second is ResponseEngineSpeedTooHigh)
            {
                second = Driver.DrivingActionBrake(absTime, ds, velocity, gradient, second);
                debug.Add(
                    "[DMD.HCAUWO-1] second:(EngineSpeedTooHigh|SpeedLimitExceeded) -> Brake with reduced acceleration",
                    second);
            }

            return second;
        }

        private IResponse FirstAccelerateOrCoast(Second absTime, Meter ds, MeterPerSecond targetVelocity,
            Radian gradient,
            bool prohibitOverspeed, MeterPerSecond velocityWithOverspeed, DebugData debug)
        {
            if (DriverStrategy.pccState == PCCStates.UseCase1 || DriverStrategy.pccState == PCCStates.UseCase2)
            {
                var response = Driver.DrivingActionCoast(absTime, ds, velocityWithOverspeed, gradient);
                debug.Add("[DMD.FAOC-0] Coast", response);
                if (response is ResponseSuccess)
                {
                    return response;
                }
            }

            var isOverspeedAllowed = DriverStrategy.IsOverspeedAllowed(targetVelocity, prohibitOverspeed);
            var isDrivingWithOverspeed = DataBus.VehicleInfo.VehicleSpeed.IsGreaterOrEqual(targetVelocity);
            if (isOverspeedAllowed && isDrivingWithOverspeed)
            {
                //we are driving in overspeed (VehicleSpeed >= targetVelocity)

                var response = Driver.DrivingActionCoast(absTime, ds, velocityWithOverspeed, gradient);
                debug.Add("[DMD.FAOC-1] Coast", response);

                if (response is ResponseSuccess
                    && response.Driver.Acceleration < 0 && response.Vehicle.VehicleSpeed <= targetVelocity)
                {
                    //do accelerate action if we would come below targetVelocity due to coasting
                    response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
                    debug.Add("[DMD.FAOC-2] Accelerate(Success && Acc<0 && VehSpeed <= targetVelocity)", response);
                }

                if (response is ResponseOverload
                    && DataBus.PowertrainInfo.HasCombustionEngine && !DataBus.EngineInfo.EngineOn)
                {
                    response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
                    debug.Add("[DMD.FAOC-3] Accelerate(Overload && ICE off)", response);
                }

                if (response is ResponseOverload
                    && !DataBus.PowertrainInfo.HasCombustionEngine)
                {
                    response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
                    debug.Add("[DMD.FAOC-4] Accelerate(Overload && BEV)", response);
                }

                if (response is ResponseOverload)
                {
                    response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
                    debug.Add("[DMD.FAOC-5] Accelerate(Overload)", response);
                }

                return response;
            }
            else
            {
                //we are not driving in overspeed (VehicleSpeed < targetVelocity)

                if (DataBus.GearboxesInfo.All(g => g.GearboxType.AutomaticTransmission()) &&
                    DataBus.GearboxesInfo.All(g => g.DisengageGearbox))
                {
                    var response = Driver.DrivingActionCoast(absTime, ds, velocityWithOverspeed, gradient);
                    debug.Add("[DMD.FAOC-6] Coast", response);
                    return response;
                }
                else
                {
                    var response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
                    debug.Add("[DMD.FAOC-7] Accelerate", response);
                    return response;
                }
            }
        }

        protected override IResponse CheckRequestDoesNotExceedNextAction(
            Second absTime, Meter ds,
            MeterPerSecond targetVelocity, Radian gradient, IResponse response, out Meter newds)
        {
            var nextAction = DriverStrategy.NextDrivingAction;
            newds = ds;
            if (nextAction == null)
            {
                return response;
            }

            var v2 = DataBus.VehicleInfo.VehicleSpeed + response.Driver.Acceleration * response.SimulationInterval;
            var newBrakingDistance = Driver.DriverData.AccelerationCurve.ComputeDecelerationDistance(v2,
                nextAction.NextTargetSpeed) + DefaultDriverStrategy.BrakingSafetyMargin;
            switch (DriverStrategy.NextDrivingAction.Action)
            {
                case DrivingBehavior.Coasting:
                    var coastingDistance = DriverStrategy.ComputeCoastingDistance(v2, nextAction.CycleEntry);
                    var newActionDistance = coastingDistance;
                    var safetyFactor = 4.0;
                    if (newBrakingDistance > coastingDistance)
                    {
                        newActionDistance = newBrakingDistance;
                        safetyFactor = 0.5;
                    }

                    // if the distance at the end of the simulation interval is smaller than the new ActionDistance
                    // we are safe - go ahead...
                    if ((DataBus.MileageCounter.Distance + ds).IsSmallerOrEqual(
                            nextAction.TriggerDistance - newActionDistance,
                            Constants.SimulationSettings.DriverActionDistanceTolerance * safetyFactor) &&
                        (DataBus.MileageCounter.Distance + ds).IsSmallerOrEqual(nextAction.TriggerDistance -
                            newBrakingDistance))
                    {
                        return response;
                    }

                    newds = ds / 2; //EstimateAccelerationDistanceBeforeBrake(response, nextAction) ?? ds;
                    break;
                case DrivingBehavior.Braking:
                    if ((DataBus.MileageCounter.Distance + ds).IsSmaller(
                            nextAction.TriggerDistance - newBrakingDistance))
                    {
                        return response;
                    }

                    newds = nextAction.TriggerDistance - newBrakingDistance - DataBus.MileageCounter.Distance -
                            Constants.SimulationSettings.DriverActionDistanceTolerance / 2;
                    break;
                default: return response;
            }

            return response;
        }

        public override void ResetMode()
        {
        }
    }
    public class DriverModeBrake : AbstractDriverMode
    {
        protected enum BrakingPhase
        {
            Coast,
            Brake
        }

        protected BrakingPhase Phase;
        protected bool RetryDistanceExceeded;

        public DriverModeBrake()
        {
        }

        protected override IResponse DoHandleRequest(
            Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
            bool prohibitOverspeed = false)
        {
            //If we have reached the target speed exactly we stop breaking

            //if (DataBus.VehicleInfo.VehicleSpeed.IsSmaller(DriverStrategy.BrakeTrigger.NextTargetSpeed, Constants.SimulationSettings.BrakeTriggerSpeedTolerance)
            if (DataBus.VehicleInfo.VehicleSpeed <= DriverStrategy.BrakeTrigger.NextTargetSpeed
                && !DataBus.VehicleInfo.VehicleStopped)
            {
                var retVal = HandleTargetspeedReached(absTime, ds, targetVelocity, gradient);
                for (var i = 0; i < 3 && retVal == null; i++)
                {
                    retVal = HandleTargetspeedReached(absTime, ds, targetVelocity, gradient);
                }

                if (retVal == null)
                {
                    throw new VectoException("Failed to find operating point!", ErrorCodes.DRIVER_OPERATING_POINT_NOT_FOUND);
                }

                return retVal;
            }

            var currentDistance = DataBus.MileageCounter.Distance;

            var brakingDistance = Driver.ComputeDecelerationDistance(DriverStrategy.BrakeTrigger.NextTargetSpeed) +
                                  DefaultDriverStrategy.BrakingSafetyMargin;
            DriverStrategy.BrakeTrigger.BrakingStartDistance =
                DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance;
            if (DriverStrategy.BrakeTrigger.Action == DrivingBehavior.Braking)
            {
                Phase = BrakingPhase.Brake;
            }

            if (Phase == BrakingPhase.Coast)
            {
                var resp = CheckSwitchingToBraking(ds, currentDistance);
                if (resp != null)
                {
                    return resp;
                }
            }

            switch (Phase)
            {
                case BrakingPhase.Coast:
                    for (var i = 1; i < 3; i++)
                    {
                        var retVal = DoCoast(absTime, ds, targetVelocity, gradient, currentDistance);
                        if (retVal != null)
                        {
                            return retVal;
                        }
                    }

                    throw new VectoException("No valid operating point found", ErrorCodes.DRIVER_OPERATING_POINT_NOT_FOUND);
                case BrakingPhase.Brake:
                    return DoBrake(absTime, ds, targetVelocity, gradient, brakingDistance, currentDistance);
                default: throw new VectoException("Invalid Phase in DriverModeBrake", ErrorCodes.DRIVER_MODE_INVALID_PHASE);
            }
        }

        private IResponse DoBrake(
            Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
            Meter brakingDistance, Meter currentDistance)
        {
            IResponse response;
            var debug = new DebugData();

            Log.Debug("Phase: BRAKE. breaking distance: {0} start braking @ {1}", brakingDistance,
                DriverStrategy.BrakeTrigger.BrakingStartDistance);

            if (DriverStrategy.BrakeTrigger.BrakingStartDistance.IsSmaller(
                    currentDistance, Constants.SimulationSettings.DriverActionDistanceTolerance / 2))
            {
                Log.Info("Expected Braking Deceleration could not be reached! {0}",
                    DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance);
            }

            Meter targetDistance = null;
            if (DataBus.VehicleInfo.VehicleSpeed < Constants.SimulationSettings.MinVelocityForCoast)
            {
                targetDistance = DriverStrategy.BrakeTrigger.TriggerDistance;
            }

            if (targetDistance == null && DriverStrategy.BrakeTrigger.NextTargetSpeed.IsEqual(0.SI<MeterPerSecond>()))
            {
                targetDistance = DriverStrategy.BrakeTrigger.TriggerDistance -
                                 DefaultDriverStrategy.BrakingSafetyMargin;
            }

            Driver.DriverBehavior = DrivingBehavior.Braking;

            if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0) && DriverStrategy.BrakeTrigger.NextTargetSpeed.IsEqual(0))
            {
                if (ds.IsEqual(targetDistance - currentDistance, 1e-4.SI<Meter>()))
                {
                    return new ResponseDrivingCycleDistanceExceeded(this)
                    {
                        MaxDistance = ds / 2
                    };
                }

                var tmpTargetVelocity = Math.Max(0.001.KMPHtoMeterPerSecond().Value(),
                    Math.Min(
                        1.KMPHtoMeterPerSecond().Value(),
                        (ds / 0.1).Value()
                    )
                ).SI<MeterPerSecond>();
                //var tmpTargetVelocity = 1.KMPHtoMeterPerSecond();
                response = Driver.DrivingActionAccelerate(absTime, ds, tmpTargetVelocity, gradient);
                debug.Add("[DMB-DB-1] Accelerate", response);

                if (response is ResponseUnderload)
                {
                    response = Driver.DrivingActionBrake(absTime, ds, tmpTargetVelocity, gradient, response,
                        overrideAction: DrivingAction.Accelerate);
                    debug.Add("[DMB-DB-2] Brake", response);
                }
            }
            else
            {
                response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
                    gradient, targetDistance: targetDistance);
                debug.Add("[DMB-DB-3] Brake", response);
            }

            if ((DataBus.GearboxesInfo.All(g => GearBoxTypeHelper.AutomaticTransmission(g.GearboxType)) ||
                 DataBus.HybridControllerInfo != null)
                && response == null)
            {
                for (var i = 0; i < 3 && response == null; i++)
                {
                    DataBus.Brakes.BrakePower = 0.SI<Watt>();
                    response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
                        gradient, targetDistance: targetDistance);
                    debug.Add($"[DMB-DB-4-{i}] Brake", response);
                }

                if (response == null)
                {
                    throw new VectoException("No valid operating point found", ErrorCodes.DRIVER_OPERATING_POINT_NOT_FOUND);
                }
            }

            switch (response)
            {
                case ResponseOverload r:
                    Log.Info(
                        "Brake -> Got OverloadResponse during brake action - desired deceleration could not be reached! response: {0}",
                        r);
                    if (!DataBus.ClutchesInfo.All(c => c.ClutchClosed(absTime)))
                    {
                        Log.Info("Brake -> Overload -> Clutch is open - Trying roll action");
                        response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
                        debug.Add("[DMB-DB-5] Roll", response);
                        if (response is ResponseSpeedLimitExceeded)
                        {
                            response = Driver.DrivingActionBrake(absTime, ds, targetVelocity, gradient);
                            debug.Add("[DMB-DB-6] Brake", response);
                        }

                        if (DataBus.ClutchesInfo.All(c => c.ClutchClosed(absTime)))
                        {
                            response = Driver.DrivingActionAccelerate(absTime, ds,
                                    DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                            debug.Add("[DMB-DB-6-1] Accelerate", response);

                            if (response is ResponseGearShift)
                            {
                                response = Driver.DrivingActionAccelerate(absTime, ds,
                                    DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                debug.Add("[DMB-DB-6-2] Accelerate", response);
                            }

                            if ((response is ResponseUnderload) || (response is ResponseDrivingCycleDistanceExceeded))
                            {
                                response = Driver.DrivingActionCoast(absTime, ds,
                                DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                debug.Add("[DMB-DB-6-3] Coast", response);
                            }
                        }
                    }
                    else
                    {
                        Log.Info("Brake -> Overload -> Clutch is closed - Trying brake action again");
                        DataBus.Brakes.BrakePower = 0.SI<Watt>();
                        DataBus.HybridControllerCtl?.RepeatDrivingAction(absTime);
                        response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
                            gradient, targetDistance: targetDistance);
                        debug.Add("[DMB-DB-7] Brake", response);
                        if (response is ResponseOverload)
                        {
                            Log.Info("Brake -> Overload -> 2nd Brake -> Overload -> Trying accelerate action");
                            var gear = PowertrainContainer.GearboxInfo.Gear;
                            if (PowertrainContainer.GearboxInfo.GearEngaged(absTime))
                            {
                                response = Driver.DrivingActionAccelerate(absTime, ds,
                                    DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                debug.Add("[DMB-DB-8] Accelerate", response);
                                if (response is ResponseOverload)
                                {
                                    Log.Info("Brake -> Overload --> Accelerate -> Overload -> trying coast action");
                                    response = Driver.DrivingActionCoast(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                    debug.Add("[DMB-DB-8-1] Coast", response);
                                }
                            }
                            else
                            {
                                response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
                                debug.Add("[DMB-DB-9] Roll", response);
                                if (!(response is ResponseSuccess) && DataBus.HybridControllerCtl != null)
                                {
                                    response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
                                    debug.Add("[DMB-DB-10] Roll", response);
                                }
                            }

                            switch (response)
                            {
                                case ResponseGearShift _:
                                    Log.Info(
                                        "Brake -> Overload -> 2nd Brake -> Accelerate or Roll -> Got GearShift response, performing roll action");
                                    response = Driver.DrivingActionRoll(absTime, ds,
                                        DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                    debug.Add("[DMB-DB-12] Roll", response);
                                    break;
                                case ResponseUnderload _:
                                    if (gear.Gear != PowertrainContainer.GearboxInfo.Gear.Gear ||
                                        absTime.IsEqual(PowertrainContainer.GearboxInfo.LastDownshift) ||
                                        (absTime.IsEqual(PowertrainContainer.GearboxInfo.LastShift) && gear.Gear==1.0))
                                    {
                                        // AT Gearbox switched gears, shift losses are no longer applied, try once more...
                                        response = Driver.DrivingActionAccelerate(absTime, ds,
                                            DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                        debug.Add("[DMB-DB-13] Accelerate", response);
                                        if (response is ResponseUnderload)
                                        {
                                            Log.Info(
                                                "Brake -> Overload --> Accelerate -> Gearshift -> Accelerate --> Underload --> Accelerate --> Underload --> trying brake action");
                                            response = Driver.DrivingActionBrake(absTime, ds,
                                                DriverStrategy.BrakeTrigger.NextTargetSpeed,
                                                gradient, targetDistance: targetDistance);
                                            debug.Add("[DMB-DB-14] Brake", response);
                                        }

                                        if (response is ResponseOverload &&
                                            PowertrainContainer.GearboxInfo.GearboxType is GearboxType.APTN)
                                        {
                                            Log.Info(
                                                "Brake -> Overload --> Accelerate -> Gearshift -> Accelerate --> Underload --> Accelerate --> Underload --> Brake -> Overload -> trying coast action");
                                            response = Driver.DrivingActionCoast(absTime, ds,
                                                DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                            debug.Add("[DMB-DB-15] Coast", response);
                                        }
                                    }

                                    break;
                            }
                        }
                    }

                    break;
                case ResponseGearShift _:
                    Log.Info("Brake -> Got GearShift response, performing roll action + brakes");

                    //response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                    DataBus.Brakes.BrakePower = 0.SI<Watt>();
                    response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
                        gradient, targetDistance: targetDistance);
                    debug.Add("[DMB-DB-14] Brake", response);
                    if (response is ResponseOverload)
                    {
                        Log.Info("Brake -> Gearshift -> Overload -> trying roll action (no gear engaged)");
                        response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
                            gradient);
                        debug.Add("[DMB-DB-15] Roll", response);
                    }

                    break;
            }

            return response;
        }

        private IResponse DoCoast(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
            Meter currentDistance)
        {
            Driver.DriverBehavior = DrivingBehavior.Coasting;
            var response = DataBus.ClutchesInfo.First().ClutchClosed(absTime)
                ? Driver.DrivingActionCoast(absTime, ds,
                    VectoMath.Max(targetVelocity, DataBus.VehicleInfo.VehicleSpeed), gradient)
                : Driver.DrivingActionRoll(absTime, ds, VectoMath.Max(targetVelocity, DataBus.VehicleInfo.VehicleSpeed),
                    gradient);
            switch (response)
            {
                case ResponseUnderload r:
                    // coast would decelerate more than driver's max deceleration => issue brakes to decelerate with driver's max deceleration
                    response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
                        gradient, r);
                    if ((DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance).IsSmallerOrEqual(
                            Constants.SimulationSettings.DriverActionDistanceTolerance))
                    {
                        Phase = BrakingPhase.Brake;
                    }

                    break;
                case ResponseOverload _:
                    // limiting deceleration while coast may result in an overload => issue brakes to decelerate with driver's max deceleration
                    response = DataBus.ClutchesInfo.First().ClutchClosed(absTime)
                        ? Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient)
                        : Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
                    //Phase = BrakingPhase.Brake;
                    break;
                case ResponseDrivingCycleDistanceExceeded r:
                    if (!ds.IsEqual(r.MaxDistance))
                    {
                        // distance has been reduced due to vehicle stop in coast/roll action => use brake action to get exactly to the stop-distance
                        // TODO what if no gear is enaged (and we need driveline power to get to the stop-distance?
                        response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
                            gradient);
                    }

                    break;
                case ResponseEngineSpeedTooHigh r:
                    response = Driver.DrivingActionBrake(absTime, ds, targetVelocity, gradient, r);
                    break;
            }

            if (response == null)
            {
                return null;
            }

            var gbxInfo = PowertrainContainer.GearboxInfo;
            var gearShiftCondition = ((gbxInfo.GearboxType is GearboxType.MT || gbxInfo.GearboxType is GearboxType.AMT || gbxInfo.GearboxType is GearboxType.IHPC ||
                                       gbxInfo.GearboxType is GearboxType.APTN || gbxInfo.GearboxType is GearboxType.ATSerial)
                                      && ((gbxInfo.LastDownshift?.IsGreaterOrEqual(absTime) ?? false) || (gbxInfo.LastShift?.IsGreaterOrEqual(absTime) ?? false)));

            // handle the SpeedLimitExceeded Response and Gearshift Response separately in case it occurs in one of the requests in the second try
            for (var i = 0;
                 i < 3 && (response is ResponseGearShift || response is ResponseSpeedLimitExceeded ||
                           (response is ResponseOverload && gearShiftCondition) || (response is ResponseUnderload && gearShiftCondition));
                 i++)
            {
                switch (response)
                {
                    case ResponseGearShift _:
                        response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
                        break;
                    case ResponseSpeedLimitExceeded _:
                        response = Driver.DrivingActionBrake(absTime, ds, DataBus.VehicleInfo.VehicleSpeed, gradient);
                        if (response is ResponseOverload && !DataBus.ClutchesInfo.First().ClutchClosed(absTime))
                        {
                            response = Driver.DrivingActionRoll(absTime, ds, DataBus.VehicleInfo.VehicleSpeed,
                                gradient);
                        }

                        if (response is ResponseGearShift)
                        {
                            response = Driver.DrivingActionBrake(absTime, ds, DataBus.VehicleInfo.VehicleSpeed,
                                gradient);
                        }

                        break;
                    case ResponseOverload _:
                        // add a condition for APTN/IEPC as ResponseGearShift does not exist for them
                        // add a condition for ATSerial/ICE as ResponseGearShift does not exist for them
                        response = Driver.DrivingActionAccelerate(absTime, ds, DataBus.VehicleInfo.VehicleSpeed,
                            gradient);
                        break;
                    case ResponseUnderload _:
                        // add a condition for IHPC as ResponseGearShift does not exist for them in case of downshift
                        response = Driver.DrivingActionBrake(absTime, ds, DataBus.VehicleInfo.VehicleSpeed,
                            gradient);
                        break;
                }

                gearShiftCondition = ((gbxInfo.GearboxType is GearboxType.MT || gbxInfo.GearboxType is GearboxType.AMT || gbxInfo.GearboxType is GearboxType.IHPC ||
                                       gbxInfo.GearboxType is GearboxType.APTN || gbxInfo.GearboxType is GearboxType.ATSerial)
                                      && ((gbxInfo.LastDownshift?.IsGreaterOrEqual(absTime) ?? false) || (gbxInfo.LastShift?.IsGreaterOrEqual(absTime) ?? false)));

            }

            return response;
        }

        private IResponse CheckSwitchingToBraking(Meter ds, Meter currentDistance)
        {
            var nextBrakeAction = DriverStrategy.GetNextDrivingAction(ds);
            if (nextBrakeAction != null &&
                !DriverStrategy.BrakeTrigger.TriggerDistance.IsEqual(nextBrakeAction.TriggerDistance) &&
                nextBrakeAction.BrakingStartDistance.IsSmaller(DriverStrategy.BrakeTrigger.BrakingStartDistance))
            {
                DriverStrategy.BrakeTrigger = nextBrakeAction;
                Log.Debug(
                    "setting brake trigger to new trigger: trigger distance: {0}, start braking @ {1}",
                    nextBrakeAction.TriggerDistance, nextBrakeAction.BrakingStartDistance);
            }

            Log.Debug("start braking @ {0}", DriverStrategy.BrakeTrigger.BrakingStartDistance);
            var remainingDistanceToBrake = DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance;
            var estimatedTimeInterval = remainingDistanceToBrake / DataBus.VehicleInfo.VehicleSpeed;
            if (estimatedTimeInterval.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval) ||
                currentDistance + Constants.SimulationSettings.DriverActionDistanceTolerance >
                DriverStrategy.BrakeTrigger.BrakingStartDistance)
            {
                Phase = BrakingPhase.Brake;
                Log.Debug("Switching to BRAKE Phase. currentDistance: {0}", currentDistance);
            }
            else
            {
                if ((currentDistance + ds).IsGreater(DriverStrategy.BrakeTrigger.BrakingStartDistance))
                {
                    return new ResponseDrivingCycleDistanceExceeded(this)
                    {
                        MaxDistance = DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance
                    };
                }
            }

            if (DataBus.VehicleInfo.VehicleSpeed < Constants.SimulationSettings.MinVelocityForCoast)
            {
                Phase = BrakingPhase.Brake;
                Log.Debug(
                    "Switching to BRAKE Phase. currentDistance: {0}  v: {1}", currentDistance,
                    DataBus.VehicleInfo.VehicleSpeed);
            }

            return null;
        }

        private IResponse HandleTargetspeedReached(Second absTime, Meter ds, MeterPerSecond targetVelocity,
            Radian gradient)
        {
            var response = TargetSpeedReachedDriveAlong(absTime, ds, targetVelocity, gradient);

            //var i = 0;
            //do {
            switch (response)
            {
                case ResponseGearShift _:
                    response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
                    switch (response)
                    {
                        case ResponseUnderload r:
                            // under-load may happen if driver limits acceleration when rolling downhill
                            response = Driver.DrivingActionBrake(absTime, ds,
                                DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient, r);
                            break;
                        case ResponseSpeedLimitExceeded _:
                            response = Driver.DrivingActionBrake(absTime, ds, DataBus.VehicleInfo.VehicleSpeed,
                                gradient);
                            break;
                    }

                    break;
                case ResponseSpeedLimitExceeded _:
                    response = Driver.DrivingActionBrake(absTime, ds, DataBus.VehicleInfo.VehicleSpeed, gradient);
                    break;
                case ResponseUnderload r:
                    //response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
                    //	gradient, r);
                    response = Driver.DrivingActionBrake(absTime, ds,
                        DataBus.VehicleInfo.VehicleSpeed + r.Driver.Acceleration * r.SimulationInterval,
                        gradient, DataBus.HybridControllerInfo == null ? r : null);
                    if (response != null)
                    {
                        switch (response)
                        {
                            case ResponseGearShift _:
                                DataBus.Brakes.BrakePower = 0.SI<Watt>();
                                response = Driver.DrivingActionBrake(absTime, ds,
                                    DriverStrategy.BrakeTrigger.NextTargetSpeed,
                                    gradient, DataBus.HybridControllerInfo == null ? r : null);
                                if (response is ResponseOverload)
                                {
                                    response = Driver.DrivingActionRoll(absTime, ds,
                                        DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                }

                                break;
                            case ResponseOverload _:
                                DataBus.Brakes.BrakePower = 0.SI<Watt>();
                                if (PowertrainContainer.GearboxInfo.GearboxType.AutomaticTransmission() ||
                                    DataBus.ClutchesInfo.First().ClutchClosed(absTime))
                                {
                                    if (DataBus.VehicleInfo.VehicleSpeed.IsGreater(0))
                                    {
                                        response = Driver.DrivingActionAccelerate(absTime, ds,
                                            DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                    }
                                    else
                                    {
                                        if (RetryDistanceExceeded)
                                        {
                                            response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity,
                                                gradient);
                                        }
                                        else
                                        {
                                            RetryDistanceExceeded = true;
                                            response = new ResponseDrivingCycleDistanceExceeded(this)
                                                { MaxDistance = ds / 2 };
                                        }
                                    }
                                }
                                else
                                {
                                    response = Driver.DrivingActionRoll(absTime, ds,
                                        DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
                                }

                                break;
                        }
                    }

                    break;
            }

            //} while (!(response is ResponseSuccess) && i++ < 3);
            return response;
        }

        private IResponse TargetSpeedReachedDriveAlong(
            Second absTime, Meter ds, MeterPerSecond targetVelocity,
            Radian gradient)
        {
            IResponse response;
            if (PowertrainContainer.GearboxInfo.GearboxType.AutomaticTransmission() ||
                (DataBus.ClutchesInfo.First().ClutchClosed(absTime) && PowertrainContainer.GearboxInfo.GearEngaged(absTime)))
            {
                if (DataBus.VehicleInfo.VehicleSpeed.IsGreater(0))
                {
                    response = Driver.DrivingActionAccelerate(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
                        gradient);
                }
                else
                {
                    if (RetryDistanceExceeded)
                    {
                        response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
                    }
                    else
                    {
                        RetryDistanceExceeded = true;
                        response = new ResponseDrivingCycleDistanceExceeded(this) { MaxDistance = ds / 2 };
                    }
                }
            }
            else
            {
                response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
            }

            return response;
        }

        protected override IResponse CheckRequestDoesNotExceedNextAction(
            Second absTime, Meter ds,
            MeterPerSecond targetVelocity, Radian gradient, IResponse response, out Meter newds)
        {
            var nextAction = DriverStrategy.BrakeTrigger;
            newds = ds;
            if (nextAction == null)
            {
                return response;
            }

            switch (nextAction.Action)
            {
                case DrivingBehavior.Coasting:
                    var v2 = DataBus.VehicleInfo.VehicleSpeed +
                             response.Driver.Acceleration * response.SimulationInterval;
                    var newBrakingDistance = Driver.DriverData.AccelerationCurve.ComputeDecelerationDistance(
                        v2,
                        nextAction.NextTargetSpeed);
                    if ((DataBus.MileageCounter.Distance + ds).IsSmaller(
                            nextAction.TriggerDistance - newBrakingDistance))
                    {
                        return response;
                    }

                    newds = nextAction.TriggerDistance - newBrakingDistance - DataBus.MileageCounter.Distance -
                            Constants.SimulationSettings.DriverActionDistanceTolerance / 2;
                    break;
                default: return response;
            }

            return response;
        }

        public override void ResetMode()
        {
            RetryDistanceExceeded = false;
            Phase = BrakingPhase.Coast;
        }
    }
}