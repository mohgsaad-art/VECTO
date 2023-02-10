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
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
    public class MeasuredSpeedGearHybridStrategy : AbstractMeasuredSpeedGearHybridStrategy<MeasuredSpeedHybridsCycleGearbox>
    {
        public MeasuredSpeedGearHybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer) : base(runData,vehicleContainer)
        {}

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, 
			GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			//This is based on the corresponding method of the HybridStrategy class.

			TestPowertrain.Gearbox.Gear = new GearshiftPosition(
				DataBus.DrivingCycleInfo.CycleData.RightSample.Gear,
				!DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive);

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

			if (TestPowertrain.WHRCharger != null) {
				TestPowertrain.WHRCharger.PreviousState.GeneratedEnergy =
					DataBus.WHRCharger.PreviousState.GeneratedEnergy;
				TestPowertrain.WHRCharger.PreviousState.ExcessiveEnergy =
					DataBus.WHRCharger.PreviousState.ExcessiveEnergy;
			}

			TestPowertrain.Gearbox.PreviousState.InAngularVelocity =
				(DataBus.GearboxInfo as CycleGearbox).PreviousState.InAngularVelocity;

			TestPowertrain.Clutch.PreviousState.InAngularVelocity =
				(DataBus.ClutchInfo as SwitchableClutch).PreviousState.InAngularVelocity;
			TestPowertrain.Clutch.PreviousState.OutAngularVelocity =
				(DataBus.ClutchInfo as SwitchableClutch).PreviousState.OutAngularVelocity;

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
			//This is based on the corresponding method of the HybridStrategy class.
			
			if (resp.Engine.EngineSpeed != null && resp.Gearbox.Gear.Engaged &&
				GearList.HasSuccessor(resp.Gearbox.Gear) && ModelData.GearboxData.Gears[resp.Gearbox.Gear.Gear]
					.ShiftPolygon.IsAboveUpshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift;
			}

			if (resp.Engine.EngineSpeed != null && GearList.HasPredecessor(resp.Gearbox.Gear) && ModelData.GearboxData
				.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon
				.IsBelowDownshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift;
			}
		}

    }

	//--------------------------------------------------
	public class MeasuredSpeedGearATHybridStrategy : AbstractMeasuredSpeedGearHybridStrategy<MeasuredSpeedHybridsCycleGearbox>
	{

		public MeasuredSpeedGearATHybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer) : base(runData,
			vehicleContainer)
		{}

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			//This is based on the corresponding method of the HybridStrategyAT class.

			TestPowertrain.Gearbox.Gear = PreviousState.GearboxEngaged ? CurrentGear : NextGear;
			TestPowertrain.Gearbox.DisengageGearbox = !nextGear.Engaged;
			TestPowertrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPowertrain.HybridController.ApplyStrategySettings(cfg);
			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
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

			var currentGear = PreviousState.GearboxEngaged ? CurrentGear : NextGear;

			var gearboxInfo = DataBus.GearboxInfo as MeasuredSpeedHybridsCycleGearbox;
			if (nextGear.Engaged && !nextGear.Equals(currentGear)) {
				
				var vDrop = DataBus.DriverInfo.DriverAcceleration * ModelData.GearshiftParameters.ATLookAheadTime;
				var vehicleSpeedPostShift = (DataBus.VehicleInfo.VehicleSpeed + vDrop * ModelData.GearshiftParameters.VelocityDropFactor).LimitTo(
					0.KMPHtoMeterPerSecond(), DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed);

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

				TestPowertrain.Gearbox.Gear = nextGear;
			} 

			if (!nextGear.Engaged) {
				TestPowertrain.Gearbox.DisengageGearbox = !nextGear.Engaged;
			}

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

			if (TestPowertrain.WHRCharger != null) {
				TestPowertrain.WHRCharger.PreviousState.GeneratedEnergy =
					DataBus.WHRCharger.PreviousState.GeneratedEnergy;
				TestPowertrain.WHRCharger.PreviousState.ExcessiveEnergy =
					DataBus.WHRCharger.PreviousState.ExcessiveEnergy;
			}

			TestPowertrain.Gearbox.PreviousState.OutAngularVelocity = gearboxInfo.PreviousState.OutAngularVelocity;
			TestPowertrain.Gearbox.PreviousState.InAngularVelocity = gearboxInfo.PreviousState.InAngularVelocity;
			
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

			var pos = ModelData.ElectricMachinesData.FirstOrDefault().Item1;
			TestPowertrain.ElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(pos) as ElectricMotor).ThermalBuffer;
			TestPowertrain.ElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(pos) as ElectricMotor).DeRatingActive;

			foreach (var emPos in TestPowertrain.ElectricMotorsUpstreamTransmission.Keys) {
				TestPowertrain.ElectricMotorsUpstreamTransmission[pos].PreviousState.EMSpeed =
					DataBus.ElectricMotorInfo(emPos).ElectricMotorSpeed;
			}

			var retVal = TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, false);
			retVal.HybridController.StrategySettings = cfg;

			return retVal;
		}

		protected override void CheckGearshiftLimits(HybridResultEntry tmp, IResponse resp)
		{
			//This is based on the corresponding method of the HybridStrategyAT class.

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
    public abstract class AbstractMeasuredSpeedGearHybridStrategy<T> : AbstractHybridStrategy<T> 
        where T : class, IHybridControlledGearbox, IGearbox
    {
        public AbstractMeasuredSpeedGearHybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer) : 
            base(runData, vehicleContainer)
        {}

        protected override void BuildSimplePowertrain(VectoRunData runData, SimplePowertrainContainer testContainer)
		{
			PowertrainBuilder.BuildSimpleHybridPowertrainGear(runData, testContainer);
        }

        protected override void WarnGearShiftRange()
        {}

        protected override GearshiftPosition CurrentGear => new GearshiftPosition(
			DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear,
			!DataBus.DrivingCycleInfo.CycleData.LeftSample.TorqueConverterActive);

		protected override GearshiftPosition NextGear => new GearshiftPosition(
			DataBus.DrivingCycleInfo.CycleData.RightSample.Gear,
			!DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive);

        protected override void DoEmergencyGearShift(GearshiftPosition currentGear, Second absTime, Second dt, NewtonMeter outTorque, 
            PerSecond outAngularVelocity, HybridResultEntry best)
        {}

        protected override bool Disengaged(IResponse firstResponse)
        {
            return (DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear == 0);
        }

        protected override List<HybridResultEntry> FindSolution(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, 
            bool dryRun)
        {
			//Evaluate configs only for input gear instead of a range of gears (corresponding method in AbstractHybridStrategy class)

            var duringTractionInterruption = (PreviousState.GearshiftTriggerTstmp + ModelData.GearboxData.TractionInterruption)
                .IsGreaterOrEqual(absTime, ModelData.GearboxData.TractionInterruption / 20);

			var allowICEOff = AllowICEOff(absTime) && (!DataBus.EngineInfo.EngineOn || !duringTractionInterruption);

			var emPos = ModelData.ElectricMachinesData.First().Item1;

			var responses = new List<HybridResultEntry>();

			EvaluateConfigsForGear(absTime, dt, outTorque, outAngularVelocity, NextGear, allowICEOff, responses, emPos, dryRun);

			return responses;
        }

        protected override IResponse RepeatDryRunWithDifferentGear(IResponse response, GearshiftPosition currentGear, Second absTime,
			Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, HybridStrategyResponse tmp)
        {
			//Repeat dry run with lower gear if engine speed is under idle.
			if ((response == null) || (response.Engine.EngineSpeed ?? 0.SI<PerSecond>()) <= DataBus.EngineInfo.EngineIdleSpeed) {
				if (CurrentGear.Gear < NextGear.Gear) {
					currentGear = new GearshiftPosition((currentGear.Gear > 0) ? currentGear.Gear - 1 : currentGear.Gear);
					return RequestDryRun(absTime, dt, outTorque, outAngularVelocity, currentGear, tmp);
                }
				else if (CurrentGear.Gear > NextGear.Gear) {
					currentGear = new GearshiftPosition(NextGear.Gear, !NextGear.TorqueConverterLocked);
						
					var hybridStrategyResponse = (NextGear.Gear == 0) ? new HybridStrategyResponse {
							CombustionEngineOn = DataBus.EngineInfo.EngineOn, 
							GearboxInNeutral = true,
							NextGear = currentGear,
							MechanicalAssistPower = ElectricMotorsOff
						} : tmp;

					return RequestDryRun(absTime, dt, outTorque, outAngularVelocity, currentGear, hybridStrategyResponse);
                }
            } 

			return response;
        }

        protected override bool DetermineEngineSpeedTooLow(IResponse firstResponse)
        {
			return !DataBus.GearboxInfo.GearEngaged(DataBus.AbsTime) 
				|| firstResponse.Clutch.OutputSpeed.IsSmaller(ModelData.EngineData.IdleSpeed);
        }

        protected override bool HandleTotalTorqueDemandNull(IResponse resp, HybridResultEntry tmp)
        {
			if (resp.Engine.TotalTorqueDemand == null) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.NoResponseAvailable;
				return true;
			}
			else {
				return false;
            }
        }

    }
}
