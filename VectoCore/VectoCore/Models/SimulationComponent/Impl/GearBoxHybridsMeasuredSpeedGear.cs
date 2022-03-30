using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.CycleGearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class GearBoxHybridsMeasuredSpeedGear : Gearbox
    {
        protected internal readonly TorqueConverterWrapper TorqueConverter;

        protected internal Second DisengagedTime;

        public override GearshiftPosition NextGear => new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear);

        public override bool GearEngaged(Second absTime)
        {
            Disengaged = (GetGearFromCycle() == 0);
            return !Disengaged;
        }

        public override void TriggerGearshift(Second absTime, Second dt)
        {
            throw new Exception("TriggerGearshift should not be called in Measured Speed with Gear Hybrid");
        }

        public GearBoxHybridsMeasuredSpeedGear(IVehicleContainer container, VectoRunData runData) : base(container, null)
        {
            CurrentState = new CycleGearboxState();
            PreviousState = new CycleGearboxState();

            if (!ModelData.Type.AutomaticTransmission()) {
                return;
            }

            var strategy = new CycleShiftStrategy(container);

            TorqueConverter = new TorqueConverterWrapper(runData.Cycle.Entries.All(x => x.EngineSpeed != null),
                new CycleTorqueConverter(container, ModelData.TorqueConverterData),
                new TorqueConverter(this, strategy, container, ModelData.TorqueConverterData, runData));
        }

        public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            if ((TorqueConverter != null) && (Gear.TorqueConverterLocked == null)) {
                throw new VectoSimulationException("Driving cycle does not contain information about TorqueConverter!");
            }

            var inAngularVelocity = DataBus.EngineInfo.EngineIdleSpeed;
            var inTorque = 0.SI<NewtonMeter>();
            IResponse response;

            Gear = Gear ?? new GearshiftPosition(0);

            if (Gear.Gear != 0) {
                var ratio = !Gear.IsLockedGear()
                    ? ModelData.Gears[Gear.Gear].TorqueConverterRatio
                    : ModelData.Gears[Gear.Gear].Ratio;

                inAngularVelocity = outAngularVelocity * ratio;

                var inTorqueLossResult = !Gear.IsLockedGear()
                    ? ModelData.Gears[Gear.Gear].TorqueConverterGearLossMap.GetTorqueLoss(outAngularVelocity, outTorque)
                    : ModelData.Gears[Gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);

                CurrentState.TorqueLossResult = inTorqueLossResult;

                inTorque = (outTorque / ratio) + inTorqueLossResult.Value;

                var dt = Constants.SimulationSettings.TargetTimeInterval;

                var torqueLossInertia = outAngularVelocity.IsEqual(0)
                    ? 0.SI<NewtonMeter>()
                    : Formulas.InertiaPower(inAngularVelocity, PreviousState.InAngularVelocity, ModelData.Inertia, dt) /
                    inAngularVelocity;

                inTorque += torqueLossInertia;

                response = (Gear.TorqueConverterLocked != null) && !Gear.TorqueConverterLocked.Value && (TorqueConverter != null)
                    ? TorqueConverter.Initialize(inTorque, inAngularVelocity, GetEngineSpeedFromCycle())
                    : NextComponent.Initialize(inTorque, inAngularVelocity);
            }
            else {
                response = NextComponent.Initialize(inTorque, inAngularVelocity);
            }

            CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
            PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);

            PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
            PreviousState.Gear = Gear;

            response.Gearbox.PowerRequest = inTorque * inAngularVelocity;

            return response;
        }

        protected PerSecond GetEngineSpeedFromCycle()
        {
            return DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
                ? DataBus.DrivingCycleInfo.CycleData.LeftSample.EngineSpeed
                : DataBus.DrivingCycleInfo.CycleData.RightSample.EngineSpeed;
        }

        protected uint GetGearFromCycle()
        {
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
        public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
            bool dryRun = false)
        {
            var gear = GetGearFromCycle();

            var TorqueConverterActive = DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
                ? DataBus.DrivingCycleInfo.CycleData.LeftSample.TorqueConverterActive
                : DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive;

            if (TorqueConverter != null && TorqueConverterActive == null) {
                throw new VectoSimulationException("Driving cycle does not contain information about TorqueConverter!");
            }

            if (gear != 0 && !ModelData.Gears.ContainsKey(gear)) {
                throw new VectoSimulationException("Requested Gear {0} from driving cycle is not available", gear);
            }

            // mk 2016-11-30: added additional check for outAngularVelocity due to failing test: MeasuredSpeed_Gear_AT_PS_Run
            // mq 2016-12-16: changed check to vehicle halted due to failing test: MeasuredSpeed_Gear_AT_*
            IResponse response = ((gear == 0) || (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted))
                ? RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun)
                : RequestEngaged(absTime, dt, outTorque, outAngularVelocity, dryRun);

            response.Gearbox.PowerRequest = outTorque * ((PreviousState.OutAngularVelocity + outAngularVelocity) / 2);

            return response;
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
        private IResponse RequestEngaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
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

            var gearData = ModelData.Gears[Gear.Gear];
            var effectiveRatio = (tcLocked ?? false) ? gearData.TorqueConverterRatio : gearData.Ratio;
            var effectiveLossMap = (tcLocked ?? false) ? gearData.TorqueConverterGearLossMap : gearData.LossMap;

            CheckModelData(effectiveLossMap, effectiveRatio);

            var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
            var inTorqueLossResult = effectiveLossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);

            CurrentState.TorqueLossResult = inTorqueLossResult;

            var inTorque = outTorque / effectiveRatio + inTorqueLossResult.Value;
            var inAngularVelocity = outAngularVelocity * effectiveRatio;

            CurrentState.InertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
                ? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) / avgOutAngularVelocity
                : 0.SI<NewtonMeter>();

            inTorque += CurrentState.InertiaTorqueLossOut / effectiveRatio;

            if (Gear != PreviousState.Gear && ConsiderShiftLosses(Gear, outTorque)) {
                myCurrentState.PowershiftLosses = ComputeShiftLosses(outTorque, outAngularVelocity, Gear);
            }

            if (myCurrentState.PowershiftLosses != null) {
                var averageEngineSpeed = (DataBus.EngineInfo.EngineSpeed + outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio) / 2;
                inTorque += myCurrentState.PowershiftLosses / dt / averageEngineSpeed;
            }

            if (dryRun) {
                var dryRunResponse = HandleDryRunRequest(absTime, dt, inTorque, inAngularVelocity);
                dryRunResponse.Gearbox.PowerRequest = outTorque * avgOutAngularVelocity;
                return dryRunResponse;
            }

            myCurrentState.TransmissionTorqueLoss = inTorque * effectiveRatio - outTorque;

            CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
            CurrentState.Gear = Gear;
          
            if (TorqueConverter != null && !Gear.TorqueConverterLocked.Value) {
                myCurrentState.TorqueConverterActive = true;
                return TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity, GetEngineSpeedFromCycle());
            }

            if (TorqueConverter != null) {
                TorqueConverter.Locked(
                    CurrentState.InTorque, CurrentState.InAngularVelocity, CurrentState.InTorque,
                    CurrentState.InAngularVelocity);
            }

            var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, false);

            response.Gearbox.PowerRequest = outTorque * avgOutAngularVelocity;
            response.Gearbox.Gear = Gear;
            response.Gearbox.InputSpeed = inAngularVelocity;
            response.Gearbox.InputTorque = inTorque;

            return response;
        }


        private void CheckModelData(TransmissionLossMap effectiveLossMap, double effectiveRatio)
        {
            if (effectiveLossMap == null || double.IsNaN(effectiveRatio)) {
                throw new VectoSimulationException(
                    "Ratio or loss-map for gear {0}{1} invalid. Please check input data", Gear,
                    Gear.TorqueConverterLocked.HasValue ? (Gear.TorqueConverterLocked.Value ? "L" : "C") : "");
            }

            if (Gear.TorqueConverterLocked.HasValue && !Gear.TorqueConverterLocked.Value && !ModelData.Gears[Gear.Gear].HasTorqueConverter) {
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
        private IResponse RequestDisengaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
        {
            if (!Disengaged) {
                DisengagedTime = absTime;
            }

            var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;

            if (dryRun) {
                // if gearbox is disengaged the 0-line is the limit for drag and full load
                return new ResponseDryRun(this)
                {
                    Gearbox = {
                        PowerRequest = outTorque * avgOutAngularVelocity,
                    },
                    DeltaDragLoad = outTorque * avgOutAngularVelocity,
                    DeltaFullLoad = outTorque * avgOutAngularVelocity,
                    DeltaFullLoadTorque = outTorque,
                    DeltaDragLoadTorque = outTorque
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

            if (GearboxType.AutomaticTransmission()) {
                disengagedResponse = EngineIdleRequest(absTime, dt);
            }
            else {
                var nextGear = new GearshiftPosition(GetGearFromCycle());

                disengagedResponse = nextGear.Gear > 0
                    ? NextComponent.Request(
                        absTime, dt, 0.SI<NewtonMeter>(),
                        outAngularVelocity * ModelData.Gears[nextGear.Gear].Ratio, false)
                    : EngineIdleRequest(absTime, dt);
            }

            if (TorqueConverter != null) {
                if (DataBus.VehicleInfo.VehicleStopped) {
                    TorqueConverter.Locked(
                        0.SI<NewtonMeter>(), disengagedResponse.Engine.EngineSpeed, CurrentState.InTorque,
                        outAngularVelocity);
                }
                else {
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
            if (disengagedResponse is ResponseSuccess) {
                return disengagedResponse;
            }

            var motoringSpeed = DataBus.EngineInfo.EngineSpeed;

            if (motoringSpeed.IsGreater(DataBus.EngineInfo.EngineIdleSpeed)) {
                var first = (ResponseDryRun)NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), motoringSpeed, true);
                
                try {
                    motoringSpeed = SearchAlgorithm.Search(
                        motoringSpeed, first.DeltaDragLoad,
                        Constants.SimulationSettings.EngineIdlingSearchInterval,
                        getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
                        evaluateFunction: n => NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), n, true),
                        criterion: result => ((ResponseDryRun)result).DeltaDragLoad.Value());
                }
                catch (VectoException) {
                    Log.Warn("CycleGearbox could not find motoring speed for disengaged state.");
                }

                motoringSpeed = motoringSpeed.LimitTo(DataBus.EngineInfo.EngineIdleSpeed, DataBus.EngineInfo.EngineSpeed);
            }

            return NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), motoringSpeed, false);
        }


        private IResponse HandleDryRunRequest(Second absTime, Second dt, NewtonMeter inTorque,PerSecond inAngularVelocity)
        {
            if (TorqueConverter != null && !Gear.TorqueConverterLocked.Value) {
                return TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity, GetEngineSpeedFromCycle(), true);
            }

            var dryRunResponse = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
            
            dryRunResponse.Gearbox.Gear = Gear;
            dryRunResponse.Gearbox.InputSpeed = inAngularVelocity;
            dryRunResponse.Gearbox.InputTorque = inTorque;

            return dryRunResponse;
        }

        protected override uint GetGearForModalResults()
        { 
			return DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear;
		}

        protected override bool ShouldCheckLossMapRange => DisengagedTime != null;

        protected override void SetDisengaged(bool value)
        { }

    }
}
