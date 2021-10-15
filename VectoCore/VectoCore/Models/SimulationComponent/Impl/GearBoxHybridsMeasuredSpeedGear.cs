using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.CycleGearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

    public class GearBoxHybridsMeasuredSpeedGear : Gearbox
    {

        protected internal bool IsPreProcessing { get; set; }

        protected internal readonly TorqueConverterWrapper TorqueConverter;

        public override GearshiftPosition Gear { get; protected internal set; }

        protected internal Second DisengagedTime;

        // FIXME: check if needed
        private Second _overrideDisengage = 0.SI<Second>();

        protected internal new GearshiftPosition _nextGear;

        public override GearshiftPosition NextGear
        {
            get
            {
                if (IsPreProcessing)
                {
                    return _nextGear;
                }

                if (Disengaged == false)
                {
                    return Gear;
                }

                var future = DataBus.DrivingCycleInfo.LookAhead(ModelData.TractionInterruption * 5);
                var nextGear = 0u;
                var torqueConverterLocked = false;
                foreach (var entry in future)
                {
                    if (entry.VehicleTargetSpeed != null && entry.VehicleTargetSpeed.IsEqual(0))
                    {
                        // vehicle is stopped, no next gear, engine should go to idle
                        break;
                    }
                    if (entry.WheelAngularVelocity != null && entry.WheelAngularVelocity.IsEqual(0))
                    {
                        // vehicle is stopped, no next gear, engine should go to idle
                        break;
                    }

                    if (entry.Gear == 0)
                    {
                        continue;
                    }

                    nextGear = entry.Gear;
                    torqueConverterLocked = !entry.TorqueConverterActive ?? false;
                    break;
                }

                return new GearshiftPosition(nextGear, GearboxType.ManualTransmission() ? (bool?)null : torqueConverterLocked);
            }
        }


        public override bool GearEngaged(Second absTime)
        {
            //Disengaged = (GetGearFromCycle() <= 0){
            Disengaged = (this.DataBus.DrivingCycleInfo.CycleData.RightSample.Gear <= 0);
            return !Disengaged;
            //return !DisengageGearbox && (_overrideDisengage == null || !_overrideDisengage.IsEqual(absTime)) &&
            //EngageTime.IsSmallerOrEqual(absTime, ModelData.TractionInterruption / 20) && !postponeEngage;
        }


        public override void TriggerGearshift(Second absTime, Second dt)
        {
            throw new Exception("TriggerGearshift should not be called in Measured Speed with Gear Hybrid");
        }

        public GearBoxHybridsMeasuredSpeedGear(IVehicleContainer container, VectoRunData runData) : base(container, null)
        {
            CurrentState = new CycleGearboxState();
            PreviousState = new CycleGearboxState();

            if (!ModelData.Type.AutomaticTransmission())
            {
                return;
            }

            var strategy = new CycleShiftStrategy(container);


            TorqueConverter = new TorqueConverterWrapper(runData.Cycle.Entries.All(x => x.EngineSpeed != null),
                new CycleTorqueConverter(container, ModelData.TorqueConverterData),
                new TorqueConverter(this, strategy, container, ModelData.TorqueConverterData, runData));
            if (TorqueConverter == null)
            {
                throw new VectoException("Torque Converter required for AT transmission!");
            }
            #region old
            /* old
			EngageTime = 0.SI<Second>();
			_strategy = strategy;
			if (_strategy != null)
			{
				_strategy.Gearbox = this;
			}

			ICEAvailable = container.PowertrainInfo.HasCombustionEngine;
			LastDownshift = -double.MaxValue.SI<Second>();
			LastUpshift = -double.MaxValue.SI<Second>();
			Disengaged = true;
			*/
            #endregion
        }

        protected new void AdvanceState()
        {
            PreviousState = CurrentState;
            CurrentState = new CycleGearboxState();
        }

        public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            var dt = Constants.SimulationSettings.TargetTimeInterval;

            var tcLocked = DataBus.DrivingCycleInfo.CycleData.LeftSample.TorqueConverterActive != null
                ? !DataBus.DrivingCycleInfo.CycleData.LeftSample.TorqueConverterActive
                : null;

            if (TorqueConverter != null && Gear.TorqueConverterLocked == null)
            {
                throw new VectoSimulationException("Driving cycle does not contain information about TorqueConverter!");
            }

            var inAngularVelocity = DataBus.EngineInfo.EngineIdleSpeed;
            var inTorque = 0.SI<NewtonMeter>();
            IResponse response;

            if (IsPreProcessing)
            {
                if (_nextGear == null)
                {
                    _nextGear = new GearshiftPosition(0);
                }
                Gear =  new GearshiftPosition(_nextGear.Gear);
            }
            if (Gear == null)
            {
                Gear = new GearshiftPosition(0);
            }
            if (Gear.Gear != 0)
            {
                var ratio = !Gear.IsLockedGear()
                    ? ModelData.Gears[Gear.Gear].TorqueConverterRatio
                    : ModelData.Gears[Gear.Gear].Ratio;
                inAngularVelocity = outAngularVelocity * ratio;
                var inTorqueLossResult = !Gear.IsLockedGear()
                    ? ModelData.Gears[Gear.Gear].TorqueConverterGearLossMap.GetTorqueLoss(outAngularVelocity, outTorque)
                    : ModelData.Gears[Gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
                CurrentState.TorqueLossResult = inTorqueLossResult;
                inTorque = outTorque / ratio + inTorqueLossResult.Value;

                var torqueLossInertia = outAngularVelocity.IsEqual(0)
                    ? 0.SI<NewtonMeter>()
                    : Formulas.InertiaPower(inAngularVelocity, PreviousState.InAngularVelocity, ModelData.Inertia, dt) /
                    inAngularVelocity;

                inTorque += torqueLossInertia;

                response = Gear.TorqueConverterLocked != null && !Gear.TorqueConverterLocked.Value && TorqueConverter != null
                    ? TorqueConverter.Initialize(inTorque, inAngularVelocity, GetEngineSpeedFromCycle())
                    : NextComponent.Initialize(inTorque, inAngularVelocity);
            }
            else
            {
                response = NextComponent.Initialize(inTorque, inAngularVelocity);
            }
            CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
            PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
            PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
            PreviousState.Gear = Gear;

            response.Gearbox.PowerRequest = inTorque * inAngularVelocity;
            return response;
            #region old
            /*
            var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			EngageTime = -double.MaxValue.SI<Second>();

			if (_strategy != null && (Disengaged || DisengageGearbox))
			{
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
			*/
            #endregion old
        }

        protected virtual PerSecond GetEngineSpeedFromCycle()
        {
            return DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
                ? DataBus.DrivingCycleInfo.CycleData.LeftSample.EngineSpeed
                : DataBus.DrivingCycleInfo.CycleData.RightSample.EngineSpeed;
        }

  
        protected virtual uint GetGearFromCycle()
        {
            /*return DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
                ? DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear
                : DataBus.DrivingCycleInfo.CycleData.RightSample.Gear;
                */
            return DataBus.DrivingCycleInfo.CycleData.RightSample.Gear;

        }
        /// <summary>
        /// Requests the Gearbox to deliver torque and angularVelocity
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item><description>ResponseDryRun</description></item>
        /// <item><description>ResponseOverload</description></item>
        /// <item><description>ResponseGearshift</description></item>
        /// </list>
        /// </returns>
        public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque,
            PerSecond outAngularVelocity,
            bool dryRun = false)
        {
            ////////Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);
            var gear = GetGearFromCycle();

            if (IsPreProcessing)
            {
                gear =  _nextGear.Gear;
            }

            var TorqueConverterActive = DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
                ? DataBus.DrivingCycleInfo.CycleData.LeftSample.TorqueConverterActive
                : DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive;

            if (TorqueConverter != null && TorqueConverterActive == null)
            {
                throw new VectoSimulationException("Driving cycle does not contain information about TorqueConverter!");
            }
            if (gear != 0 && !ModelData.Gears.ContainsKey(gear))
            {
                throw new VectoSimulationException("Requested Gear {0} from driving cycle is not available", gear);
            }


            IResponse retVal;
            // mk 2016-11-30: added additional check for outAngularVelocity due to failing test: MeasuredSpeed_Gear_AT_PS_Run
            // mq 2016-12-16: changed check to vehicle halted due to failing test: MeasuredSpeed_Gear_AT_*
            if (gear == 0 || DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted)  //|| (outAngularVelocity.IsSmallerOrEqual(0, 1) && outTorque.IsSmallerOrEqual(0, 1))
            {
                if (absTime.IsGreaterOrEqual(273.9851.SI<Second>()) && dryRun) //273.4851 //273.4851 //13.0332 //274.5037,13.0332,3
                {
                    ////////Log.Debug($"DRYRUN: Gearbox RequestDisengaged with : {gear}");
                }
                retVal = RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
            }
            else
            {
                if (absTime.IsGreaterOrEqual(273.9851.SI<Second>()) && dryRun) //273.4851 //273.4851 //13.0332 //274.5037,13.0332,3
                {
                    ////////Log.Debug($"DRYRUN: Gearbox RequestEngaged with gear: {gear}");
                }
                retVal = RequestEngaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
            }


            retVal.Gearbox.PowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2;
            if (absTime.IsGreaterOrEqual(273.9851.SI<Second>()) && dryRun) //273.4851 //273.4851 //13.0332 //274.5037,13.0332,3
            {
                ////////Log.Debug($"DRYRUN: Gearbox PowerRequest: {retVal.Gearbox.PowerRequest}");
            }
            return retVal;
            #region this is old
            /* this is old
			IterationStatistics.Increment(this, "Requests");

			_strategy?.Request(absTime, dt, outTorque, outAngularVelocity);

			////////Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);
			if (DataBus.VehicleInfo.VehicleStopped)
			{
				EngageTime = absTime;
				LastDownshift = -double.MaxValue.SI<Second>();
				LastUpshift = -double.MaxValue.SI<Second>();
			}

			if (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted)
			{
				EngageTime = absTime + dt;
			}

			postponeEngage = false;
			var reEngaging = false;
			if (GearEngaged(absTime) && Disengaged && !outAngularVelocity.IsEqual(0))
			{
				if (dt.IsSmaller(Constants.SimulationSettings.TargetTimeInterval / 10))
				{
					////////Log.Debug("postponing re-engage due to small simulation interval {0}", dt);
					postponeEngage = true;
				}
				else
				{
					ReEngageGear(absTime, dt, outTorque, outAngularVelocity);
					reEngaging = true;
					////////Log.Debug("Gearbox engaged gear {0}", Gear);
				}
			}

			if (_overrideDisengage != null && (!_strategy?.CheckGearshiftRequired ?? false) && !dryRun)
			{
				var changeGear = _strategy?.ShiftRequired(absTime, dt, outTorque, outAngularVelocity,
					outTorque / ModelData.Gears[Gear.Gear].Ratio,
					outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio, Gear,
					EngageTime, null) ?? false;
				if (changeGear)
				{
					ReEngageGear(absTime, dt, outTorque, outAngularVelocity);
					reEngaging = true;
				}
			}

			if (reEngaging && DataBus.HybridControllerInfo != null &&
				!Equals(Gear, DataBus.HybridControllerInfo.SelectedGear))
			{
				return new ResponseDifferentGearEngaged(this);
			}

			var gear = Disengaged ? NextGear.Gear : Gear.Gear;
			//var gear = this.DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear;

			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var inTorqueLossResult = ModelData.Gears[gear].LossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			if (avgOutAngularVelocity.IsEqual(0, 1e-9))
			{
				inTorqueLossResult.Value = 0.SI<NewtonMeter>();
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
			var avgInAngularVelocity = (PreviousState.InAngularVelocity + inAngularVelocity) / 2.0;
			var inTorque = !avgInAngularVelocity.IsEqual(0)
				? outTorque * (avgOutAngularVelocity / avgInAngularVelocity)
				: outTorque / ModelData.Gears[Gear.Gear].Ratio;
			inTorque += inTorqueLossResult.Value;
			//var inTorque = outTorque / ModelData.Gears[gear].Ratio + inTorqueLossResult.Value;

			var inertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
				? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
				avgOutAngularVelocity
				: 0.SI<NewtonMeter>();
			inTorque += inertiaTorqueLossOut / ModelData.Gears[gear].Ratio;

			var halted = DataBus.DriverInfo.DrivingAction == DrivingAction.Halt;
			var driverDeceleratingNegTorque = DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking &&
											DataBus.DriverInfo.DrivingAction == DrivingAction.Brake &&
											(DataBus.DrivingCycleInfo.RoadGradient.IsSmaller(0) ||
											(ICEAvailable && inAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineIdleSpeed))) &&
											(DataBus.Brakes.BrakePower.IsGreater(0) || inTorque.IsSmaller(0));
			var vehiclespeedBelowThreshold =
				DataBus.VehicleInfo.VehicleSpeed.IsSmaller(Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed);
			if (halted || (driverDeceleratingNegTorque && vehiclespeedBelowThreshold))
			{
				EngageTime = VectoMath.Max(EngageTime, absTime + dt);
				_strategy?.Disengage(absTime, dt, outTorque, outAngularVelocity);
				//if (_strategy != null && DataBus.HybridControllerInfo != null &&
				//	DataBus.HybridControllerInfo.SelectedGear.Gear > 0 &&
				//	NextGear.Gear != DataBus.HybridControllerInfo.SelectedGear.Gear) {
				//	return new ResponseDifferentGearEngaged(this);
				//}

				return RequestGearDisengaged(absTime, dt, outTorque, outAngularVelocity, inTorque, dryRun);
			}

			if (!dryRun)
			{
				CurrentState.DrivingBehavior = DataBus.DriverInfo.DriverBehavior;
			}

			return GearEngaged(absTime)
				? RequestGearEngaged(absTime, dt, outTorque, outAngularVelocity, inTorque, inTorqueLossResult,
					inertiaTorqueLossOut, dryRun)
				: RequestGearDisengaged(absTime, dt, outTorque, outAngularVelocity, inTorque, dryRun);
			*/
            #endregion
        }

        /// <summary>
        /// Handles requests when a gear is engaged
        /// </summary>
        /// <param name="absTime"></param>
        /// <param name="dt"></param>
        /// <param name="outTorque"></param>
        /// <param name="outAngularVelocity"></param>
        /// <param name="dryRun"></param>
        /// <returns></returns>
        private IResponse RequestEngaged(
            Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
            bool dryRun)
        {
            CycleGearboxState myCurrentState = new CycleGearboxState();
            myCurrentState.OutTorque = CurrentState.OutTorque;
            myCurrentState.OutAngularVelocity = CurrentState.OutAngularVelocity;
            myCurrentState.InertiaTorqueLossOut = CurrentState.InertiaTorqueLossOut;
            myCurrentState.TransmissionTorqueLoss = CurrentState.TransmissionTorqueLoss;
            myCurrentState.TorqueLossResult = CurrentState.TorqueLossResult;
            myCurrentState.DrivingBehavior = CurrentState.DrivingBehavior;

            DisengagedTime = null;

            var tcLocked = DataBus.DrivingCycleInfo.CycleData.LeftSample.TorqueConverterActive != null
                ? !DataBus.DrivingCycleInfo.CycleData.LeftSample.TorqueConverterActive
                : null;
            Gear = new GearshiftPosition(GetGearFromCycle(), tcLocked);


            var effectiveRatio = ModelData.Gears[Gear.Gear].Ratio;
            var effectiveLossMap = ModelData.Gears[Gear.Gear].LossMap;
            if (Gear.TorqueConverterLocked.HasValue && !Gear.TorqueConverterLocked.Value)
            {
                effectiveRatio = ModelData.Gears[Gear.Gear].TorqueConverterRatio;
                effectiveLossMap = ModelData.Gears[Gear.Gear].TorqueConverterGearLossMap;
            }

            CheckModelData(effectiveLossMap, effectiveRatio);

            var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
            var inTorqueLossResult = effectiveLossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
            CurrentState.TorqueLossResult = inTorqueLossResult;
            var inTorque = outTorque / effectiveRatio + inTorqueLossResult.Value;
            CurrentState.TorqueLossResult = inTorqueLossResult;
            var inAngularVelocity = outAngularVelocity * effectiveRatio;

            CurrentState.InertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
                ? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
                avgOutAngularVelocity
                : 0.SI<NewtonMeter>();
            inTorque += CurrentState.InertiaTorqueLossOut / effectiveRatio;
            if (Gear != PreviousState.Gear &&
                ConsiderShiftLosses(Gear, outTorque))
            {
                myCurrentState.PowershiftLosses = ComputeShiftLosses(outTorque, outAngularVelocity, Gear);
            }
            if (myCurrentState.PowershiftLosses != null)
            {
                var averageEngineSpeed = (DataBus.EngineInfo.EngineSpeed + outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio) / 2;
                inTorque += myCurrentState.PowershiftLosses / dt / averageEngineSpeed;
            }
            if (dryRun)
            {
                var dryRunResponse = HandleDryRunRequest(absTime, dt, inTorque, inAngularVelocity);
                dryRunResponse.Gearbox.PowerRequest = outTorque * avgOutAngularVelocity;
                return dryRunResponse;
            }

            myCurrentState.TransmissionTorqueLoss = inTorque * effectiveRatio - outTorque;

            CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
            CurrentState.Gear = Gear;

            // end critical section
          

            if (TorqueConverter != null && !Gear.TorqueConverterLocked.Value)
            {
                myCurrentState.TorqueConverterActive = true;
                return TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity, GetEngineSpeedFromCycle());
            }

            if (TorqueConverter != null)
            {
                TorqueConverter.Locked(
                    CurrentState.InTorque, CurrentState.InAngularVelocity, CurrentState.InTorque,
                    CurrentState.InAngularVelocity);
            }
            if (absTime.IsGreaterOrEqual(273.9851.SI<Second>()) && dryRun) //273.4851 //273.4851 //13.0332 //274.5037,13.0332,3
            {
                ////////Log.Debug($"DRYRUN: Gearbox inTorque: {inTorque}");
            }
            var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, false);
            response.Gearbox.PowerRequest = outTorque * avgOutAngularVelocity;
            return response;
        }


        private void CheckModelData(TransmissionLossMap effectiveLossMap, double effectiveRatio)
        {
            if (effectiveLossMap == null || double.IsNaN(effectiveRatio))
            {
                throw new VectoSimulationException(
                    "Ratio or loss-map for gear {0}{1} invalid. Please check input data", Gear,
                    Gear.TorqueConverterLocked.HasValue ? (Gear.TorqueConverterLocked.Value ? "L" : "C") : "");
            }
            if (Gear.TorqueConverterLocked.HasValue && !Gear.TorqueConverterLocked.Value && !ModelData.Gears[Gear.Gear].HasTorqueConverter)
            {
                throw new VectoSimulationException("Torque converter requested by cycle for gear without torque converter!");
            }
        }


        /// <summary>
        /// Handles Requests when no gear is disengaged
        /// </summary>
        /// <param name="absTime"></param>
        /// <param name="dt"></param>
        /// <param name="outTorque"></param>
        /// <param name="outAngularVelocity"></param>
        /// <param name="dryRun"></param>
        /// <returns></returns>
        private IResponse RequestDisengaged(
            Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
            bool dryRun)
        {
            if (Disengaged == false)
            {
                DisengagedTime = absTime;
            }

            var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
            if (dryRun)
            {
                // if gearbox is disengaged the 0-line is the limit for drag and full load
                return new ResponseDryRun(this)
                {
                    Gearbox = {
                        PowerRequest = outTorque * avgOutAngularVelocity,
                    },
                    DeltaDragLoad = outTorque * avgOutAngularVelocity,
                    DeltaFullLoad = outTorque * avgOutAngularVelocity,
                    DeltaFullLoadTorque = outTorque,
                    DeltaDragLoadTorque = outTorque,

                };
            }

            if ((outTorque * avgOutAngularVelocity).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance) &&
                !outAngularVelocity.IsEqual(0))
            {
                return new ResponseOverload(this)
                {
                    Delta = outTorque * avgOutAngularVelocity,
                    Gearbox = {
                        PowerRequest = outTorque * avgOutAngularVelocity
                    }
                };
            }

            if ((outTorque * avgOutAngularVelocity).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance))
            {
                return new ResponseUnderload(this)
                {
                    Delta = outTorque * avgOutAngularVelocity,
                    Gearbox = {
                        PowerRequest = outTorque * avgOutAngularVelocity
                    }
                };
            }

            IResponse disengagedResponse;
            if (GearboxType.AutomaticTransmission())
            {
                disengagedResponse = EngineIdleRequest(absTime, dt);
            }
            else
            {
                disengagedResponse = NextGear.Gear > 0
                    ? NextComponent.Request(
                        absTime, dt, 0.SI<NewtonMeter>(),
                        outAngularVelocity * ModelData.Gears[NextGear.Gear].Ratio, false)
                    : EngineIdleRequest(absTime, dt);
            }
            if (TorqueConverter != null)
            {
                if (DataBus.VehicleInfo.VehicleStopped)
                {
                    TorqueConverter.Locked(
                        0.SI<NewtonMeter>(), disengagedResponse.Engine.EngineSpeed, CurrentState.InTorque,
                        outAngularVelocity);
                }
                else
                {
                    TorqueConverter.Locked(
                        CurrentState.InTorque, disengagedResponse.Engine.EngineSpeed, CurrentState.InTorque,
                        disengagedResponse.Engine.EngineSpeed);
                }
            }
            disengagedResponse.Gearbox.PowerRequest = outTorque * avgOutAngularVelocity;
            CurrentState.SetState(0.SI<NewtonMeter>(), disengagedResponse.Engine.EngineSpeed, 0.SI<NewtonMeter>(), outAngularVelocity);
            CurrentState.Gear = Gear;

            return disengagedResponse;
        }


        private IResponse EngineIdleRequest(Second absTime, Second dt)
        {
            var disengagedResponse = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), DataBus.EngineInfo.EngineIdleSpeed, false);
            if (disengagedResponse is ResponseSuccess)
            {
                return disengagedResponse;
            }

            var motoringSpeed = DataBus.EngineInfo.EngineSpeed;
            if (motoringSpeed.IsGreater(DataBus.EngineInfo.EngineIdleSpeed))
            {
                var first = (ResponseDryRun)NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), motoringSpeed, true);
                try
                {
                    motoringSpeed = SearchAlgorithm.Search(
                        motoringSpeed, first.DeltaDragLoad,
                        Constants.SimulationSettings.EngineIdlingSearchInterval,
                        getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
                        evaluateFunction: n => NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), n, true),
                        criterion: result => ((ResponseDryRun)result).DeltaDragLoad.Value());
                }
                catch (VectoException)
                {
                    Log.Warn("CycleGearbox could not find motoring speed for disengaged state.");
                }
                motoringSpeed = motoringSpeed.LimitTo(DataBus.EngineInfo.EngineIdleSpeed, DataBus.EngineInfo.EngineSpeed);
            }
            disengagedResponse = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), motoringSpeed, false);
            return disengagedResponse;
        }


        private IResponse HandleDryRunRequest(Second absTime, Second dt, NewtonMeter inTorque,PerSecond inAngularVelocity)
        {
            if (TorqueConverter != null && !Gear.TorqueConverterLocked.Value)
            {
                return TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity, GetEngineSpeedFromCycle(), true);
            }

            // mk 2016-12-13
            //if (outTorque.IsSmaller(0) && inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
            //	//Log.Warn("engine speed would fall below idle speed - disengage! gear from cycle: {0}, vehicle speed: {1}", Gear,
            //	//	DataBus.VehicleSpeed);
            //	Gear = 0;
            //	return RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
            //}
            return NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
        }

        protected override void DoWriteModalResults(Second time, Second simulationInterval,
        IModalDataContainer container)
        {
            var avgInAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
            var avgOutAngularSpeed = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
            var inPower = CurrentState.InTorque * avgInAngularSpeed;
            var outPower = CurrentState.OutTorque * avgOutAngularSpeed;
            //container[ModalResultField.Gear] = Disengaged || DataBus.VehicleInfo.VehicleStopped ? 0 : Gear.Gear;
            container[ModalResultField.Gear] = this.DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear;//Gear.Gear;

            container[ModalResultField.P_gbx_loss] = inPower - outPower;
            container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgOutAngularSpeed;
            container[ModalResultField.P_gbx_in] = inPower;
            container[ModalResultField.n_gbx_out_avg] = (PreviousState.OutAngularVelocity +
                                                        CurrentState.OutAngularVelocity) / 2.0;
            container[ModalResultField.T_gbx_out] = CurrentState.OutTorque;
            container[ModalResultField.T_gbx_in] = CurrentState.InTorque;

            //container[ModalResultField.DisengageGearbox] = this.DisengageGearbox;
            //container[ModalResultField.Disengaged] = this.Disengaged;
            //container[ModalResultField.GearEngaged] = this.Gear.Engaged;
            //_strategy.WriteModalResults(container);
        }

        protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
        {
            if (DisengagedTime > 0)
            {
                if (CurrentState.TorqueLossResult != null && CurrentState.TorqueLossResult.Extrapolated)
                {
                    Log.Warn(
                        "Gear {0} LossMap data was extrapolated: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
                        Gear, CurrentState.OutAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.OutTorque,
                        ModelData.Gears[Gear.Gear].Ratio);
                    if (DataBus.ExecutionMode == ExecutionMode.Declaration)
                    {
                        throw new VectoException(
                            "Gear {0} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
                            Gear, CurrentState.InAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.InTorque,
                            ModelData.Gears[Gear.Gear].Ratio);
                    }
                }
            }

            if (DataBus.VehicleInfo.VehicleStopped)
            {
                //Disengaged = 0;
                EngageTime = -double.MaxValue.SI<Second>();
            }

            if (GearEngaged(DataBus.AbsTime))
            {
                _overrideDisengage = null;
            }

            if (PreviousState.DrivingBehavior != CurrentState.DrivingBehavior)
            {
                ////////Log.Debug("driving action changed...");
                if (CurrentState.DrivingBehavior == DrivingBehavior.Driving ||
                    CurrentState.DrivingBehavior == DrivingBehavior.Braking)
                {
                    if (GearEngaged(time))
                    {
                        ////////Log.Debug("resetting gearshift time interval");
                        LastDownshift = -double.MaxValue.SI<Second>();
                        LastUpshift = -double.MaxValue.SI<Second>();
                    }
                    else
                    {
                        CurrentState.DrivingBehavior = PreviousState.DrivingBehavior;
                    }
                }
            }

            base.DoCommitSimulationStep(time, simulationInterval);
        }

    }
}
