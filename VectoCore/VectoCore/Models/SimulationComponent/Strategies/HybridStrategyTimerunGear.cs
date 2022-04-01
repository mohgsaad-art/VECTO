using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils.MatrixRecorder;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
    public class HybridStrategyTimerunGear : AbstractHybridStrategyTimerunGear<GearBoxHybridsMeasuredSpeedGear>
    {
        public HybridStrategyTimerunGear(VectoRunData runData, IVehicleContainer vehicleContainer) : base(runData,vehicleContainer)
        { }

    }

    //--------------------------------------------------
    public abstract class AbstractHybridStrategyTimerunGear<T> : AbstractHybridStrategy<T> 
        where T : class, IHybridControlledGearbox, IGearbox
    {
        public AbstractHybridStrategyTimerunGear(VectoRunData runData, IVehicleContainer vehicleContainer) : 
            base(runData, vehicleContainer)
        {
        }

        protected override void BuildSimplePowertrain(PowertrainBuilder builder, VectoRunData runData, 
            SimplePowertrainContainer testContainer)
		{
			builder.BuildSimpleHybridPowertrainGear(runData, testContainer);
        }

        protected override void CalculateBatteryDischargeEnergyThreshold()
        {
            var auxEnergyReserve = ModelData.ElectricAuxDemand * StrategyParameters.AuxReserveTime;
            var minSoc = Math.Max(DataBus.BatteryInfo.MinSoC, StrategyParameters.MinSoC);
            
            BatteryDischargeEnergyThreshold = (auxEnergyReserve > 0) 
                ? (DataBus.BatteryInfo.Capacity * minSoc * DataBus.BatteryInfo.NominalVoltage) + auxEnergyReserve
                : 0.SI<WattSecond>();
        }

        protected override void WarnGearShiftRange()
        {
        }

        protected override bool Accelerating => (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate) 
			&& (StrategyParameters.MaxPropulsionTorque != null);

        protected override GearshiftPosition CurrentGear => new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear, false);

        protected override void LogDataForInvestigation(Second absTime, List<HybridResultEntry> eval)
        {
            if (absTime.IsGreaterOrEqual(0))
			{
				Log.Debug($"Investigate absTime: {absTime} eval.count: {eval.Count()}");
			
                foreach(var ev in eval)
                {
					Dictionary<string, object> dict = new Dictionary<string, object>()
					{
						["Cost"] = ev.Cost,
						["FuelCosts"] = ev.FuelCosts,
						["BatCosts"] = ev.BatCosts,
						["ICEStartPenalty1"] = ev.ICEStartPenalty1,
						["ICEStartPenalty2"] = ev.ICEStartPenalty2,
						["SoCPenalty"] = ev.SoCPenalty,
						["RampUpPenalty"] = ev.RampUpPenalty,
						["GearshiftPenalty"] = ev.GearshiftPenalty,
						["IgnoreReason"] = ev.IgnoreReason.HumanReadable()
					};

					MatRec.NewRow(dict);
				}
			}
        }

        protected override void DoEmergencyGearShift(GearshiftPosition currentGear, Second absTime, Second dt, NewtonMeter outTorque, 
            PerSecond outAngularVelocity, HybridResultEntry best)
        {
        }

        protected override bool CheckEngineSpeed(IResponse testRequest)
        {
            return true;
        }

        protected override GearshiftPosition NextGear => new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.RightSample.Gear, false);

        protected override Second ShiftTimePassed => 0.SI<Second>();

        protected override bool Disengaged(IResponse firstResponse)
        {
            return (DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear == 0);
        }

        protected override (bool mustReturn, bool iceOn) HandleBreakingActionWhenEngineAndElectricMotorOff(PowertrainPosition emPos, 
            IResponse firstResponse)
        {
            return (false, DataBus.EngineInfo.EngineOn);
        }

        protected override void CheckDriverPCCStateAndAddResponseEmOff(List<HybridResultEntry> eval)
        {
            eval.Add(ResponseEmOff);
        }

        protected override bool Foo(HybridConfigurationIgnoreReason reason)
        {
            return reason.EngineSpeedBelowDownshift();
        }

        protected override List<HybridResultEntry> FindSolution(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, 
            bool dryRun)
        {
            var duringTractionInterruption = (PreviousState.GearshiftTriggerTstmp + ModelData.GearboxData.TractionInterruption)
                .IsGreaterOrEqual(absTime, ModelData.GearboxData.TractionInterruption / 20);

			var allowICEOff = AllowICEOff(absTime) && (!DataBus.EngineInfo.EngineOn || !duringTractionInterruption);

			var emPos = ModelData.ElectricMachinesData.First().Item1;

			var gear = new GearshiftPosition(this.DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear, false);

			var responses = new List<HybridResultEntry>();

			var nextGear = new GearshiftPosition(this.DataBus.DrivingCycleInfo.CycleData.RightSample.Gear, false);

			var emOffEntry = EvaluateConfigsForGear(absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, responses, emPos, dryRun);

			var tmpBest = responses.Where(x => !double.IsNaN(x.Score)).OrderBy(x => x.Score).FirstOrDefault();

			return responses;
        }

        protected override Tuple<uint, uint> GetGearRange(Second absTime, bool dryRun)
        {
            Tuple<uint, uint> gearRange = new Tuple<uint, uint>(0, 1);

            try {
                gearRange = base.GetGearRange(absTime, dryRun);
            }
            catch {}

            return gearRange;
        }

        protected override bool CannotEvaluate(HybridResultEntry emOffEntry)
        {
            if (StrategyParameters.MaxPropulsionTorque != null && ModelData.GearboxData.Type.AutomaticTransmission()) {
				var maxTqNextGear = StrategyParameters.MaxPropulsionTorque.FullLoadDriveTorque(emOffEntry.Response.Gearbox.InputSpeed);
				
                if (((emOffEntry.Response.Gearbox.InputTorque - maxTqNextGear) * emOffEntry.Response.Gearbox.InputSpeed)
                    .IsGreater(0, Constants.SimulationSettings.LineSearchTolerance)) {
					
                    return true;
				}
			}

            return false;
        }

        protected override double CalculateMaxU(bool allowIceOff, NewtonMeter maxEmTorque, NewtonMeter emTqReq)
        {
            var maxU = -1.0;

			if (!allowIceOff && !emTqReq.IsEqual(0)) {
				maxU = Math.Min((maxEmTorque) / emTqReq, -1.0);
			}

			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake) {
				maxU = 0;
			}

            return maxU;
        }
    }
}
