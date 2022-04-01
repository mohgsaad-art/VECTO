using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils.MatrixRecorder;

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

        protected override GearshiftPosition GetCurrentGear => new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear, false);

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

    }
}
