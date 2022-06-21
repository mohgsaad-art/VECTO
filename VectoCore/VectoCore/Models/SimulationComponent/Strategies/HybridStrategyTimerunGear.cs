using System;
using System.Collections.Generic;
using System.Linq;

using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
    public class HybridStrategyTimerunGear : AbstractHybridStrategyTimerunGear<GearBoxHybridsMeasuredSpeedGear>
    {
        public HybridStrategyTimerunGear(VectoRunData runData, IVehicleContainer vehicleContainer) : base(runData,vehicleContainer)
        { }

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, 
			GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			TestPowertrain.Gearbox.Gear = new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.RightSample.Gear);
			TestPowertrain.Gearbox.Disengaged = (DataBus.DrivingCycleInfo.CycleData.RightSample.Gear == 0);
			TestPowertrain.Gearbox.DisengageGearbox = (DataBus.DrivingCycleInfo.CycleData.RightSample.Gear == 0);
			TestPowertrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPowertrain.HybridController.ApplyStrategySettings(cfg);
			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
			TestPowertrain.Clutch.Initialize(DataBus.ClutchInfo.ClutchLosses);
			TestPowertrain.Battery?.Initialize(DataBus.BatteryInfo.StateOfCharge);

			if (TestPowertrain.Battery != null) {
				TestPowertrain.Battery.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as Battery).PreviousState.PulseDuration;
			}
			if (TestPowertrain.BatterySystem != null) {
				var batSystem = DataBus.BatteryInfo as BatterySystem;
				foreach (var bsKey in batSystem.Batteries.Keys) {
					for (var i = 0; i < batSystem.Batteries[bsKey].Batteries.Count; i++) {
						TestPowertrain.BatterySystem.Batteries[bsKey].Batteries[i]
							.Initialize(batSystem.Batteries[bsKey].Batteries[i].StateOfCharge);
					}
				}
				TestPowertrain.BatterySystem.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as BatterySystem).PreviousState.PulseDuration;
			}
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

			TestPowertrain.Gearbox._nextGear = Controller.ShiftStrategy.NextGear;

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
	public class HybridStrategyATTimerunGear : AbstractHybridStrategyTimerunGear<ATGearbox> //ATGearBoxHybridsMeasuredSpeedGear ?
	{

		public HybridStrategyATTimerunGear(VectoRunData runData, IVehicleContainer vehicleContainer) : base(runData,
			vehicleContainer)
		{ }

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			TestPowertrain.Gearbox.Gear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;
			TestPowertrain.Gearbox.Disengaged = !nextGear.Engaged;
			TestPowertrain.Gearbox.DisengageGearbox = !nextGear.Engaged;
			TestPowertrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPowertrain.HybridController.ApplyStrategySettings(cfg);
			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
			//	TestPowertrain.Clutch.Initialize(DataBus.ClutchInfo.ClutchLosses);
			TestPowertrain.Battery?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			if (TestPowertrain.Battery != null) {
				TestPowertrain.Battery.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as Battery).PreviousState.PulseDuration;
			}
			if (TestPowertrain.BatterySystem != null) {
				var batSystem = DataBus.BatteryInfo as BatterySystem;
				foreach (var bsKey in batSystem.Batteries.Keys) {
					for (var i = 0; i < batSystem.Batteries[bsKey].Batteries.Count; i++) {
						TestPowertrain.BatterySystem.Batteries[bsKey].Batteries[i]
							.Initialize(batSystem.Batteries[bsKey].Batteries[i].StateOfCharge);
					}
				}
				TestPowertrain.BatterySystem.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as BatterySystem).PreviousState.PulseDuration;
			}
			TestPowertrain.SuperCap?.Initialize(DataBus.BatteryInfo.StateOfCharge);

			
			TestPowertrain.Brakes.BrakePower = DataBus.Brakes.BrakePower;

			var currentGear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;

			var gearboxInfo = DataBus.GearboxInfo as ATGearbox;
			if (nextGear.Engaged && !nextGear.Equals(currentGear)) {
				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio > ModelData.GearshiftParameters.RatioEarlyUpshiftFC) {
					return null;
				}

				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio >= ModelData.GearshiftParameters.RatioEarlyDownshiftFC) {
					return null;
				}

				var vDrop = DataBus.DriverInfo.DriverAcceleration * ModelData.GearshiftParameters.ATLookAheadTime;
				var vehicleSpeedPostShift = (DataBus.VehicleInfo.VehicleSpeed + vDrop * ModelData.GearshiftParameters.VelocityDropFactor).LimitTo(
					0.KMPHtoMeterPerSecond(), DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed);

				//var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
				//if (!AllowEmergencyShift && !estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				//	return null;
				//}

				if (nextGear.TorqueConverterLocked.HasValue && nextGear.TorqueConverterLocked.Value) {
					var inAngularVelocity = ModelData.GearboxData.Gears[nextGear.Gear].Ratio * outAngularVelocity;

					if (inAngularVelocity.IsEqual(0)) {
						return null;
					}

					var totalTransmissionRatio = inAngularVelocity /
												(DataBus.VehicleInfo.VehicleSpeed +
												DataBus.DriverInfo.DriverAcceleration * dt);

					var estimatedEngineSpeed = (vehicleSpeedPostShift * totalTransmissionRatio).Cast<PerSecond>();
					if (estimatedEngineSpeed.IsSmaller(ModelData.GearshiftParameters.MinEngineSpeedPostUpshift)) {
						return null;
					}
				}

				//var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
				//var vehicleSpeedPostShift = estimatedVelocityPostShift; // DataBus.VehicleInfo.VehicleSpeed - vDrop * ModelData.GearshiftParameters.VelocityDropFactor;
				TestPowertrain.Gearbox.Gear = nextGear;
				TestPowertrain.Gearbox.RequestAfterGearshift = true;
				//var init = TestPowertrain.Container.VehiclePort.Initialize(
				//	vehicleSpeedPostShift, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
				//if (!AllowEmergencyShift && init.Engine.EngineSpeed.IsSmaller(ModelData.EngineData.IdleSpeed)) {
				//	return null;
				//}
			} else {
				TestPowertrain.Gearbox.RequestAfterGearshift = gearboxInfo.RequestAfterGearshift;
			}
			//TestPowertrain.Gearbox.ShiftToLocked = (DataBus.GearboxInfo as ATGearbox).ShiftToLocked;

			if (!nextGear.Engaged) {
				//TestPowertrain.Gearbox._nextGear = Controller.ShiftStrategy.NextGear;
				TestPowertrain.Gearbox.Disengaged = !nextGear.Engaged;
			}

			//if (!PreviousState.GearboxEngaged) {
			var engineInfo = DataBus.EngineInfo as CombustionEngine;
			var enginePrevious = engineInfo.PreviousState;
			TestPowertrain.CombustionEngine.Initialize(enginePrevious.EngineTorque, enginePrevious.EngineSpeed);
			var testEnginePrevious = TestPowertrain.CombustionEngine.PreviousState;
			testEnginePrevious.EngineOn = enginePrevious.EngineOn;
			testEnginePrevious.EnginePower = enginePrevious.EnginePower;
			testEnginePrevious.dt = enginePrevious.dt;
			testEnginePrevious.EngineSpeed = enginePrevious.EngineSpeed;
			testEnginePrevious.EngineTorque = enginePrevious.EngineTorque;
			testEnginePrevious.EngineTorqueOut = enginePrevious.EngineTorqueOut;
			testEnginePrevious.DynamicFullLoadTorque = enginePrevious.DynamicFullLoadTorque;

			switch (TestPowertrain.CombustionEngine.EngineAux) {
				case EngineAuxiliary engineAux:
					engineAux.PreviousState.AngularSpeed = (engineInfo.EngineAux as EngineAuxiliary).PreviousState.AngularSpeed;
					break;
				case BusAuxiliariesAdapter busAux:
					busAux.PreviousState.AngularSpeed = (engineInfo.EngineAux as BusAuxiliariesAdapter).PreviousState.AngularSpeed;
					if (busAux.ElectricStorage is SimpleBattery bat) {
						bat.SOC = (engineInfo.EngineAux as BusAuxiliariesAdapter)
							.ElectricStorage
							.SOC;
					}
					break;
			}

			if (TestPowertrain.DCDCConverter != null) {
				TestPowertrain.DCDCConverter.PreviousState.ConsumedEnergy =
					(DataBus.DCDCConverter as DCDCConverter).PreviousState.ConsumedEnergy;
			}

			TestPowertrain.Gearbox.PreviousState.OutAngularVelocity = gearboxInfo.PreviousState.OutAngularVelocity;
			TestPowertrain.Gearbox.PreviousState.InAngularVelocity = gearboxInfo.PreviousState.InAngularVelocity;
			TestPowertrain.Gearbox._powershiftLossEnergy = gearboxInfo._powershiftLossEnergy;
			TestPowertrain.Gearbox.PreviousState.PowershiftLossEnergy = gearboxInfo.PreviousState.PowershiftLossEnergy;
			TestPowertrain.Gearbox.LastShift = gearboxInfo.LastShift;
			TestPowertrain.Gearbox.PreviousState.Gear = gearboxInfo.PreviousState.Gear;

			if (nextGear.TorqueConverterLocked.HasValue && !nextGear.TorqueConverterLocked.Value) {
				var dataBusTorqueConverterInfo = DataBus.TorqueConverterInfo as TorqueConverter;
				var prev = dataBusTorqueConverterInfo.PreviousState;
				var testTCPrevious = TestPowertrain.TorqueConverter.PreviousState;
				testTCPrevious.InAngularVelocity = prev.InAngularVelocity;
				testTCPrevious.InTorque = prev.InTorque;
				testTCPrevious.OutAngularVelocity = prev.OutAngularVelocity;
				testTCPrevious.IgnitionOn = prev.IgnitionOn;
			}

			//TestPowertrain.Clutch.PreviousState.InAngularVelocity =
			//	(DataBus.ClutchInfo as SwitchableClutch).PreviousState.InAngularVelocity;

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

			try {
				var retVal = TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque,
					outAngularVelocity, false);

				//if (retVal.Source is TorqueConverter) {
				//	return null;
				//}

				retVal.HybridController.StrategySettings = cfg;
				return retVal;
			} catch (Exception ex) {
				Log.Debug(ex);
				return null;
			}
		}

		protected override void CheckGearshiftLimits(HybridResultEntry tmp, IResponse resp)
		{
			if (resp.Engine.EngineSpeed == null) {
				return;
			}
			if (resp.Gearbox.Gear.Engaged && GearList.HasSuccessor(resp.Gearbox.Gear)) {
				var current = resp.Gearbox.Gear;
				var successor = GearList.Successor(current);
				if (successor.IsLockedGear()) {
					// C/L -> L shift
					var nextEngineSpeed = resp.Gearbox.OutputSpeed * ModelData.GearboxData.Gears[successor.Gear].Ratio;
					if (nextEngineSpeed.IsEqual(0)) {
						return;
					}
					var nextEngineTorque = resp.Engine.EngineSpeed * resp.Engine.TotalTorqueDemand / nextEngineSpeed;
					if (ModelData.GearboxData.Gears[resp.Gearbox.Gear.Gear]
						.ShiftPolygon.IsAboveUpshiftCurve(nextEngineTorque, nextEngineSpeed)) {
						tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift;
					}
				} else {
					// C -> C shift
					//throw new NotImplementedException("TC-TC upshift not implemented");
				}
			}

			if (GearList.HasPredecessor(resp.Gearbox.Gear) && ModelData.GearboxData
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
        { }

        protected override void BuildSimplePowertrain(VectoRunData runData, SimplePowertrainContainer testContainer)
		{
			PowertrainBuilder.BuildSimpleHybridPowertrainGear(runData, testContainer);
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
        { }

        protected override GearshiftPosition CurrentGear => new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear, false);

		protected override GearshiftPosition NextGear => new GearshiftPosition(DataBus.DrivingCycleInfo.CycleData.RightSample.Gear, false);

        protected override void DoEmergencyGearShift(GearshiftPosition currentGear, Second absTime, Second dt, NewtonMeter outTorque, 
            PerSecond outAngularVelocity, HybridResultEntry best)
        { }

        protected override bool Disengaged(IResponse firstResponse)
        {
            return (DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear == 0);
        }

        protected override List<HybridResultEntry> FindSolution(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, 
            bool dryRun)
        {
            var duringTractionInterruption = (PreviousState.GearshiftTriggerTstmp + ModelData.GearboxData.TractionInterruption)
                .IsGreaterOrEqual(absTime, ModelData.GearboxData.TractionInterruption / 20);

			var allowICEOff = AllowICEOff(absTime) && (!DataBus.EngineInfo.EngineOn || !duringTractionInterruption);

			var emPos = ModelData.ElectricMachinesData.First().Item1;

			var responses = new List<HybridResultEntry>();

			EvaluateConfigsForGear(absTime, dt, outTorque, outAngularVelocity, NextGear, allowICEOff, responses, emPos, dryRun);

			return responses;
        }

    }
}
