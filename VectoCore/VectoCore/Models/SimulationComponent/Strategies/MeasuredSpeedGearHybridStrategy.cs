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
    public class MeasuredSpeedGearHybridStrategy : AbstractMeasuredSpeedGearHybridStrategy
    {
        public MeasuredSpeedGearHybridStrategy(VectoRunData runData, IVehicleContainer container) : base(runData, container)
        {}

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, 
			GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			//This is based on the corresponding method of the HybridStrategy class.
			
			TestPowertrain.UpdateComponents();
			var useNextGear = new GearshiftPosition(
				DataBus.DrivingCycleInfo.CycleData.RightSample.Gear,
				!DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive);

			TestPowertrain.Gearbox.SetGear = useNextGear; // DataBus.VehicleInfo.VehicleStopped ? NextGear : PreviousState.GearboxEngaged ? CurrentGear : NextGear;
			TestPowertrain.Gearbox.SetDisengageGearbox = !useNextGear.Engaged;
			TestPowertrain.Vehicle.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPowertrain.HybridController.ApplyStrategySettings(cfg);

			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);

			if (!PreviousState.GearboxEngaged || (useNextGear.Engaged && useNextGear.Equals(CurrentGear)) || !nextGear.Engaged) {
				TestPowertrain.CombustionEngine.UpdateFrom(DataBus.EngineInfo);
				TestPowertrain.Gearbox.UpdateFrom(DataBus.GearboxInfo);
				TestPowertrain.Clutch.UpdateFrom(DataBus.ClutchInfo);
				var pos = ModelData.ElectricMachinesData.FirstOrDefault().Item1;
				TestPowertrain.ElectricMotor.UpdateFrom(DataBus.ElectricMotorInfo(pos));
				// TODO: MQ 2025-02-05: is this really necessary? EM is updated in the line above anyways
				foreach (var emPos in TestPowertrain.ElectricMotorsUpstreamTransmission.Keys) {
					//TestPowertrain.ElectricMotorsUpstreamTransmission[pos].PreviousState.EMSpeed = DataBus.ElectricMotorInfo(emPos).ElectricMotorSpeed;
					TestPowertrain.ElectricMotorsUpstreamTransmission[pos].UpdateFrom(DataBus.ElectricMotorInfo(emPos));
				}
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
	public class MeasuredSpeedGearATHybridStrategy : AbstractMeasuredSpeedGearHybridStrategy
	{

		public MeasuredSpeedGearATHybridStrategy(VectoRunData runData, IVehicleContainer container) : base(runData, container)
		{}

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			//This is based on the corresponding method of the HybridStrategyAT class.
			
			TestPowertrain.UpdateComponents();
			
			TestPowertrain.Gearbox.SetGear = PreviousState.GearboxEngaged ? CurrentGear : NextGear;
			TestPowertrain.Gearbox.SetDisengageGearbox = !nextGear.Engaged;
			TestPowertrain.Vehicle.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPowertrain.HybridController.ApplyStrategySettings(cfg);
			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
			
			TestPowertrain.Brakes.BrakePower = DataBus.Brakes.BrakePower;
			TestPowertrain.DCDCConverter?.UpdateFrom(DataBus.DCDCConverter);
			
			if (nextGear.Engaged && !nextGear.Equals(TestPowertrain.Gearbox.Gear)) {
				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio > ModelData.GearshiftParameters.RatioEarlyUpshiftFC) {
					return null;
				}

				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio >= ModelData.GearshiftParameters.RatioEarlyDownshiftFC) {
					return null;
				}

				var vDrop = DataBus.DriverInfo.DriverAcceleration * ModelData.GearshiftParameters.ATLookAheadTime;
				var vehicleSpeedPostShift = (DataBus.VehicleInfo.VehicleSpeed + vDrop * ModelData.GearshiftParameters.VelocityDropFactor).LimitTo(0.KMPHtoMeterPerSecond(), DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed);
				if (nextGear.TorqueConverterLocked.HasValue && nextGear.TorqueConverterLocked.Value) {
					var inAngularVelocity = ModelData.GearboxData.Gears[nextGear.Gear].Ratio * outAngularVelocity;
					if (inAngularVelocity.IsEqual(0)) {
						return null;
					}

					var totalTransmissionRatio = inAngularVelocity / (DataBus.VehicleInfo.VehicleSpeed + DataBus.DriverInfo.DriverAcceleration * dt);
					var estimatedEngineSpeed = (vehicleSpeedPostShift * totalTransmissionRatio).Cast<PerSecond>();
					if (estimatedEngineSpeed.IsSmaller(ModelData.GearshiftParameters.MinEngineSpeedPostUpshift)) {
						return null;
					}
				}

				TestPowertrain.Gearbox.SetGear = nextGear;
				TestPowertrain.Gearbox.RequestAfterGearshift = true;
			} else {
				TestPowertrain.Gearbox.RequestAfterGearshift = DataBus.GearboxInfo.RequestAfterGearshift;
			}
			
			if (!nextGear.Engaged) {
				TestPowertrain.Gearbox.SetDisengageGearbox = !nextGear.Engaged;
			}

			TestPowertrain.CombustionEngine.UpdateFrom(DataBus.EngineInfo);
			TestPowertrain.Gearbox.UpdateFrom(DataBus.GearboxInfo);
			if (nextGear.TorqueConverterLocked.HasValue && !nextGear.TorqueConverterLocked.Value) {
				TestPowertrain.TorqueConverter.UpdateFrom(DataBus.TorqueConverterInfo);
			}

			var pos = ModelData.ElectricMachinesData.FirstOrDefault().Item1;
			TestPowertrain.ElectricMotor.UpdateFrom(DataBus.ElectricMotorInfo(pos));
			// TODO: MQ 2025-02-05: is this really necessary? EM is updated in the line above anyways
			foreach (var emPos in TestPowertrain.ElectricMotorsUpstreamTransmission.Keys) {
				//TestPowertrain.ElectricMotorsUpstreamTransmission[pos].PreviousState.EMSpeed = DataBus.ElectricMotorInfo(emPos).ElectricMotorSpeed;
				TestPowertrain.ElectricMotorsUpstreamTransmission[pos].UpdateFrom(DataBus.ElectricMotorInfo(emPos));
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
    public abstract class AbstractMeasuredSpeedGearHybridStrategy : AbstractHybridStrategy
	{
        public AbstractMeasuredSpeedGearHybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer) : 
            base(runData, vehicleContainer)
        {}

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
