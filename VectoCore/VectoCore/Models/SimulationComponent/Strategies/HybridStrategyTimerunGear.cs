using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;
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

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			TestPowertrain.Gearbox.Gear = new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.RightSample.Gear);
			TestPowertrain.Gearbox.Disengaged = (DataBus.DrivingCycleInfo.CycleData.RightSample.Gear == 0);
			TestPowertrain.Gearbox.DisengageGearbox = (DataBus.DrivingCycleInfo.CycleData.RightSample.Gear == 0);
			TestPowertrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPowertrain.HybridController.ApplyStrategySettings(cfg);
			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
			TestPowertrain.Clutch.Initialize(DataBus.ClutchInfo.ClutchLosses);
			TestPowertrain.Battery?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			TestPowertrain.SuperCap?.Initialize(DataBus.BatteryInfo.StateOfCharge);

			TestPowertrain.Brakes.BrakePower = DataBus.Brakes.BrakePower;

			var currentGear = new GearshiftPosition(this.DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear, false);

			if (nextGear.Engaged && !nextGear.Equals(currentGear)) {
				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio > ModelData.GearshiftParameters.RatioEarlyUpshiftFC) {
					return null;
				}

				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio >= ModelData.GearshiftParameters.RatioEarlyDownshiftFC) {
					return null;
				}
			}

			var combustionEngineInfo = DataBus.EngineInfo as CombustionEngine;
			var enginePrevious = combustionEngineInfo.PreviousState;
			TestPowertrain.CombustionEngine.Initialize(enginePrevious.EngineTorque, enginePrevious.EngineSpeed);
			var testPreviousState = TestPowertrain.CombustionEngine.PreviousState;
			testPreviousState.EngineOn = enginePrevious.EngineOn;
			testPreviousState.EnginePower = enginePrevious.EnginePower;
			testPreviousState.dt = enginePrevious.dt;
			testPreviousState.EngineSpeed = enginePrevious.EngineSpeed;
			testPreviousState.EngineTorque = enginePrevious.EngineTorque;
			testPreviousState.EngineTorqueOut = enginePrevious.EngineTorqueOut;
			testPreviousState.DynamicFullLoadTorque = enginePrevious.DynamicFullLoadTorque;

			switch (TestPowertrain.CombustionEngine.EngineAux) {
				case EngineAuxiliary engineAux:
					engineAux.PreviousState.AngularSpeed =
						(combustionEngineInfo.EngineAux as EngineAuxiliary).PreviousState
						.AngularSpeed;
					break;
				case BusAuxiliariesAdapter busAux:
					busAux.PreviousState.AngularSpeed =
						(combustionEngineInfo.EngineAux as BusAuxiliariesAdapter).PreviousState
						.AngularSpeed;
					if (busAux.ElectricStorage is SimpleBattery bat) {
						bat.SOC = (combustionEngineInfo.EngineAux as BusAuxiliariesAdapter)
							.ElectricStorage
							.SOC;
					}
					break;
			}

			if (TestPowertrain.DCDCConverter != null) {
				TestPowertrain.DCDCConverter.PreviousState.ConsumedEnergy =
					(DataBus.DCDCConverter as DCDCConverter).PreviousState.ConsumedEnergy;
			}

			TestPowertrain.Gearbox.PreviousState.InAngularVelocity =
				(DataBus.GearboxInfo as Gearbox).PreviousState.InAngularVelocity;

			TestPowertrain.Clutch.PreviousState.InAngularVelocity =
				(DataBus.ClutchInfo as SwitchableClutch).PreviousState.InAngularVelocity;
			TestPowertrain.Clutch.PreviousState.OutAngularVelocity =
				(DataBus.ClutchInfo as SwitchableClutch).PreviousState.OutAngularVelocity;

			//}

			var pos = ModelData.ElectricMachinesData.FirstOrDefault().Item1;
			TestPowertrain.ElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(pos) as ElectricMotor).ThermalBuffer;
			TestPowertrain.ElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(pos) as ElectricMotor).DeRatingActive;

			foreach (var emPos in TestPowertrain.ElectricMotorsUpstreamTransmission.Keys) {
				TestPowertrain.ElectricMotorsUpstreamTransmission[pos].PreviousState.EMSpeed =
					DataBus.ElectricMotorInfo(emPos).ElectricMotorSpeed;
			}

			var retVal = TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, true);
			retVal.HybridController.StrategySettings = cfg;
			return retVal;
		}

		protected override void CheckGearshiftLimits(HybridResultEntry tmp, IResponse resp)
		{
			if (resp.Engine.EngineSpeed != null && resp.Gearbox.Gear.Engaged &&
				GearList.HasSuccessor(resp.Gearbox.Gear) && ModelData.GearboxData.Gears[resp.Gearbox.Gear.Gear]
					.ShiftPolygon.IsAboveUpshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				//lastShiftTime = absTime;
				//tmp.FuelCosts = double.NaN; // Tuple.Create(true, response.Gearbox.Gear + 1);
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift;
			}

			if (resp.Engine.EngineSpeed != null && GearList.HasPredecessor(resp.Gearbox.Gear) && ModelData.GearboxData
				.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon
				.IsBelowDownshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				//lastShiftTime = absTime;
				//tmp.FuelCosts = double.NaN; // = Tuple.Create(true, response.Gearbox.Gear - 1);
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift;
			}
		}

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
            BatteryDischargeEnergyThreshold = 0.SI<WattSecond>();

			CalculateBatteryDischargeEnergyThresholdWithAuxEnergyReserve();
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

        protected override void InitializeBatteryDischargeEnergyThreshold()
        {
        }

        protected override void CalculateCosts(IResponse resp, Second dt, HybridResultEntry tmp, bool allowIceOff, bool dryRun)
        {
            tmp.IgnoreReason = 0;
			if (resp == null) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.NoResponseAvailable;
				return;
			}

			if (resp.Engine.TotalTorqueDemand == null)
			{
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.NoResponseAvailable;
				return;
			}

			var iceOff = allowIceOff && resp.Engine.TorqueOutDemand.IsEqual(0, 1e-3);
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && dryRun) {
				// this means we search for an acceleration the vehicle is capable to do - so treat ICE overload regularly
				iceOff = false;
			}


			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
				StrategyParameters.MaxPropulsionTorque != null) {
				var tqRequest = resp.Gearbox.InputTorque;
				var maxTorque =
					StrategyParameters.MaxPropulsionTorque.FullLoadDriveTorque(resp.Gearbox.InputSpeed);
				if (((tqRequest - maxTorque) * resp.Gearbox.InputSpeed).IsGreater(0, Constants.SimulationSettings.LineSearchTolerance)) {
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.MaxPropulsionTorqueExceeded;
				}
			}

			try
			{
				if (!iceOff && /*!resp.Engine.TotalTorqueDemand.IsBetween(resp.Engine.DragTorque, resp.Engine.DynamicFullLoadTorque)*/
					(resp.Engine.TotalTorqueDemand.IsSmaller(resp.Engine.DragTorque) || resp.Engine.TotalTorqueDemand.IsGreater(resp.Engine.DynamicFullLoadTorque))) {
					tmp.FuelCosts = double.NaN;
					tmp.IgnoreReason |= resp.Engine.TotalTorqueDemand.IsGreater(resp.Engine.DynamicFullLoadTorque)
						? HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh
						: HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow;
				}
			}
			catch
			{
				//Log.Info("Error during Hybrid strategy TotalTorqueDemand!");
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.NotEvaluated;
			}


            try
            {
				if (resp.Engine.EngineSpeed.IsSmallerOrEqual(ModelData.EngineData.IdleSpeed)) {
					tmp.FuelCosts = double.NaN;
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooLow;
				}
			}
			catch
			{
				Log.Info("Error during Hybrid strategy Gearbox!");
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.NotEvaluated;
			}

			//if (resp.Engine.EngineSpeed != null && resp.Gearbox.Gear.Engaged && GearList.HasSuccessor(resp.Gearbox.Gear) && ModelData.GearboxData.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon.IsAboveUpshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
			//	//lastShiftTime = absTime;
			//	//tmp.FuelCosts = double.NaN; // Tuple.Create(true, response.Gearbox.Gear + 1);
			//	tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift;
			//}
			//if (resp.Engine.EngineSpeed != null && GearList.HasPredecessor(resp.Gearbox.Gear) && ModelData.GearboxData.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon.IsBelowDownshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
			//	//lastShiftTime = absTime;
			//	//tmp.FuelCosts = double.NaN; // = Tuple.Create(true, response.Gearbox.Gear - 1);
			//	tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift;
			//}

			if (resp.Source is TorqueConverter) {
				if (resp is ResponseUnderload) {
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow;
				}

				if (resp is ResponseOverload) {
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh;
				}
			}

			SetBatteryCosts(resp, dt, tmp);
			var absTime = DataBus.AbsTime; // todo!
			if (DataBus.GearboxInfo.GearEngaged(absTime)) {

				if (iceOff) {
					// no torque from ICE requested, ICE could be turned off
					tmp.FuelCosts = 0;
					tmp.ICEOff = true;
				} else {
					if (!double.IsNaN(tmp.FuelCosts)) {
						//if (!allowIceOff || !resp.Engine.TorqueOutDemand.IsEqual(0)) {
						tmp.FuelCosts = ModelData.EngineData.Fuels.Sum(
							x => (x.ConsumptionMap.GetFuelConsumptionValue(resp.Engine.TotalTorqueDemand, resp.Engine.EngineSpeed)
								* x.FuelData.LowerHeatingValueVecto * dt).Value());

						//}
					}
				}
			} else {
				if (!resp.Engine.TorqueOutDemand.IsEqual(0, 1e-3)) {
					tmp.FuelCosts = double.NaN;
					tmp.IgnoreReason |= resp.Engine.TorqueOutDemand.IsGreater(0)
						? HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh
						: HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow;
				}
				if (iceOff) {
					// no torque from ICE requested, ICE could be turned off
					tmp.FuelCosts = 0;
					tmp.ICEOff = true;
				} else {
					if (!double.IsNaN(tmp.FuelCosts)) {
						tmp.FuelCosts = ModelData.EngineData.Fuels.Sum(
							x => (x.ConsumptionMap.GetFuelConsumptionValue(0.SI<NewtonMeter>(), resp.Engine.EngineSpeed)
								* x.FuelData.LowerHeatingValueVecto * dt).Value());
					}
				}
			}

			var maxSoC = Math.Min(DataBus.BatteryInfo.MaxSoC, StrategyParameters.MaxSoC);
			var minSoC = Math.Max(DataBus.BatteryInfo.MinSoC, StrategyParameters.MinSoC);
			tmp.SoCPenalty = 1 - Math.Pow((DataBus.BatteryInfo.StateOfCharge - StrategyParameters.TargetSoC) / (0.5 * (maxSoC - minSoC)), StrategyParameters.CostFactorSOCExponent);

			var socthreshold = StrategyParameters.MinSoC + (StrategyParameters.MaxSoC - StrategyParameters.MinSoC) * 0.1;
			var minSoCPenalty = 10.0;
			if (DataBus.BatteryInfo.StateOfCharge.IsSmaller(socthreshold)) {
				var k = minSoCPenalty / (minSoC - socthreshold);
				var d = minSoCPenalty - k * minSoC;
				var extraSoCPenalty = k * DataBus.BatteryInfo.StateOfCharge + d;
				tmp.SoCPenalty += extraSoCPenalty;
			}

			tmp.EquivalenceFactor = resp.ElectricSystem.RESSPowerDemand.IsSmaller(0)
				? StrategyParameters.EquivalenceFactorDischarge
				: StrategyParameters.EquivalenceFactorCharge;
			tmp.GearshiftPenalty = resp.Gearbox.Gear.Engaged && !resp.Gearbox.Gear.Equals(DataBus.GearboxInfo.Gear)
				? ModelData.GearshiftParameters.RatingFactorCurrentGear
				: 1;

			if (!DataBus.EngineCtl.CombustionEngineOn && !tmp.ICEOff && DataBus.BatteryInfo.StateOfCharge.IsGreater(socthreshold)) {
				tmp.ICEStartPenalty1 = IceRampUpCosts / 10;
				tmp.ICEStartPenalty2 = IceIdlingCosts * 0;
			} else {
				tmp.ICEStartPenalty1 = 0;
				tmp.ICEStartPenalty2 = 0;
			}

			if (!DataBus.EngineCtl.CombustionEngineOn && !tmp.ICEOff) {
				var engineRampUpEnergy = Formulas.InertiaPower(DataBus.EngineInfo.EngineSpeed, ModelData.EngineData.IdleSpeed,
					ModelData.EngineData.Inertia, ModelData.EngineData.EngineStartTime) * ModelData.EngineData.EngineStartTime;
				var avgRampUpSpeed = (ModelData.EngineData.IdleSpeed + DataBus.EngineInfo.EngineSpeed) / 2.0;
				var engineDragEnergy =
					VectoMath.Abs(ModelData.EngineData.FullLoadCurves[0].DragLoadStationaryTorque(avgRampUpSpeed)) *
					avgRampUpSpeed * 0.5.SI<Second>();
				tmp.RampUpPenalty = (engineRampUpEnergy + engineDragEnergy).Value() * StrategyParameters.ICEStartPenaltyFactor;
			} else {
				tmp.RampUpPenalty = 0;
			}
			if (!double.IsNaN(tmp.FuelCosts) && tmp.IgnoreReason == 0) {
				tmp.IgnoreReason = HybridConfigurationIgnoreReason.Evaluated;
			}
        }

    }
}
