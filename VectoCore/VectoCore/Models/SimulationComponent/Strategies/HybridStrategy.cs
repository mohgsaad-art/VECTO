using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
    public class HybridStrategy : AbstractHybridStrategy
	{
		public HybridStrategy(VectoRunData runData, IVehicleContainer container) : base(runData, container)
		{
            // register pre-processors
			container.AddPreprocessor(GetGearshiftPreprocessor());
		}

		protected ISimulationPreprocessor GetGearshiftPreprocessor()
		{
			var runData = ModelData;
			var maxG = runData.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var grad = Convert.ToInt32(maxG / 2) * 2;
			if (grad == 0) {
				grad = 2;
			}
			
			return new VelocitySpeedGearshiftPreprocessor(VelocityDropData, runData.GearboxData.TractionInterruption,
				TestPowertrain, -grad, grad);
		}

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			TestPowertrain.UpdateComponents();
			var useNextGear = nextGear;
			if (nextGear.Gear == 0) {
				useNextGear = NextGear;
			}

			TestPowertrain.Gearbox.SetGear = useNextGear; // DataBus.VehicleInfo.VehicleStopped ? NextGear : PreviousState.GearboxEngaged ? CurrentGear : NextGear;
			TestPowertrain.Gearbox.SetDisengaged = !useNextGear.Engaged;
			TestPowertrain.Gearbox.SetDisengageGearbox = !useNextGear.Engaged;
			TestPowertrain.Gearbox.SetNextGear = NextGear;
			if (ModelData.GearboxData.TractionInterruption.IsGreater(0)) {
				TestPowertrain.Vehicle.Initialize(DataBus.VehicleInfo.VehicleSpeed,
					DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			}

			if (TestPowertrain.EngineAux is BusAuxiliariesAdapter busAux) {
				busAux.CurrentState.ExcessiveDragPower =
					((DataBus.EngineInfo as CombustionEngine)?.EngineAux as BusAuxiliariesAdapter)?.CurrentState
					.ExcessiveDragPower ?? 0.SI<Watt>();
			}
			
            TestPowertrain.HybridController.ApplyStrategySettings(cfg);
			

			if (useNextGear.Engaged && !useNextGear.Equals(TestPowertrain.Gearbox.Gear)) {
				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[useNextGear.Gear].Ratio > ModelData.GearshiftParameters.RatioEarlyUpshiftFC) {
					return null;
				}

				if (!AllowEmergencyShift && ModelData.GearboxData.Gears[useNextGear.Gear].Ratio >= ModelData.GearshiftParameters.RatioEarlyDownshiftFC) {
					return null;
				}
				var estimatedVelocityPostShift = !VelocityDropData.Valid ? DataBus.VehicleInfo.VehicleSpeed : VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
				if (!AllowEmergencyShift && !estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
					return null;
				}

				TestPowertrain.Gearbox.SetGear = useNextGear;
				var init = TestPowertrain.Vehicle.Initialize(estimatedVelocityPostShift, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
				if (!AllowEmergencyShift && init.Engine.EngineSpeed.IsSmaller(ModelData.EngineData.IdleSpeed)) {
					return null;
				}
			}

			if ((!DataBus.GearboxInfo.GearboxType.IsOneOf(GearboxType.APTN, GearboxType.IHPC))) {
				TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque,
					Controller.PreviousState.OutAngularVelocity);
			}

			if (!PreviousState.GearboxEngaged || (useNextGear.Engaged && useNextGear.Equals(CurrentGear)) || !nextGear.Engaged) {
				TestPowertrain.CombustionEngine.UpdateFrom(DataBus.EngineInfo);
				TestPowertrain.Gearbox.UpdateFrom(DataBus.GearboxInfo);
				TestPowertrain.Clutch.UpdateFrom(DataBus.ClutchInfo);
				TestPowertrain.Brakes.UpdateFrom(DataBus.Brakes);
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

	public class HybridStrategyAT : AbstractHybridStrategy
	{
		public HybridStrategyAT(VectoRunData runData, IVehicleContainer container) : base(runData, container)
		{ }

		protected override IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition nextGear, HybridStrategyResponse cfg)
		{
			TestPowertrain.UpdateComponents();
			
			TestPowertrain.Gearbox.SetGear = PreviousState.GearboxEngaged ? CurrentGear : NextGear;
			TestPowertrain.Gearbox.SetDisengaged = !nextGear.Engaged;
			TestPowertrain.Gearbox.SetDisengageGearbox = !nextGear.Engaged;
			TestPowertrain.Vehicle.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPowertrain.HybridController.ApplyStrategySettings(cfg);
			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
			
			TestPowertrain.Brakes.BrakePower = DataBus.Brakes.BrakePower;
			TestPowertrain.DCDCConverter?.UpdateFrom(DataBus.DCDCConverter);
			
			if (TestPowertrain.EngineAux is BusAuxiliariesAdapter busAux) {
				busAux.CurrentState.ExcessiveDragPower =
					((DataBus.EngineInfo as CombustionEngine)?.EngineAux as BusAuxiliariesAdapter)?.CurrentState
					.ExcessiveDragPower ?? 0.SI<Watt>();
			}
			
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
				TestPowertrain.Gearbox.SetDisengaged = !nextGear.Engaged;
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
			
			try {
				var retVal = TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, false);
				retVal.HybridController.StrategySettings = cfg;
				return retVal;
			} catch (Exception e) {
				Log.Debug(e);
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

	public abstract class AbstractHybridStrategy : LoggingObject, IHybridControlStrategy
	{
		public class StrategyState
		{
			public PerSecond AngularVelocity { get; set; }
			public HybridStrategyResponse Response { get; set; }
			public List<HybridResultEntry> Evaluations;
			public HybridResultEntry Solution { get; set; }
			public bool GearboxEngaged { get; set; }
			public Second ICEStartTStmp { get; set; }
			public Second GearshiftTriggerTstmp { get; set; }
			public NewtonMeter MaxGbxTq { get; set; }
			public bool ICEOn { get; set; }
		}

		public class DryRunSolutionState
		{
			public DryRunSolutionState(DrivingAction drivingAction, HybridResultEntry setting,
				List<HybridResultEntry> hybridResultEntries)
			{
				DrivingAction = drivingAction;
				Solution = setting;
				EvaluatedConfigs = hybridResultEntries;
			}


			public DrivingAction DrivingAction { get; }

			public HybridResultEntry Solution { get; }

			public List<HybridResultEntry> EvaluatedConfigs { get; }
		}

		protected VectoRunData ModelData;
		protected IVehicleContainer DataBus;

		protected Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> ElectricMotorsOff;

		protected bool ElectricMotorCanPropellDuringTractionInterruption;

		//private Second lastShiftTime;

		protected ISimplePowertrainBuilder PowertrainBuilder { get; private set; }

		protected ITestPowertrain TestPowertrain;

		public VelocityRollingLookup VelocityDropData { get; } = new VelocityRollingLookup();

		protected StrategyState CurrentState = new StrategyState();
		protected StrategyState PreviousState = new StrategyState();
		protected double IceRampUpCosts;
		protected double IceIdlingCosts;

		protected HybridStrategyParameters StrategyParameters;

		protected DebugData DebugData = new DebugData();
		protected WattSecond BatteryDischargeEnergyThreshold;

		protected DryRunSolutionState DryRunSolution { get; set; }

		protected readonly GearList GearList;
		protected bool LimitedGbxTorque;

		public Second EngineOffTimestamp { get; set; }

		public Second VehicleHaltTimestamp { get; set; }


		public AbstractHybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer)
		{
			DataBus = vehicleContainer;
			PowertrainBuilder = vehicleContainer.SimplePowertrainBuilder;
			ModelData = runData;
			if (ModelData.ElectricMachinesData.Select(x => x.Item1).Distinct().Count() > 1) {
				throw new VectoException("More than one electric motors are currently not supported");
			}
			StrategyParameters = ModelData.HybridStrategyParameters;
			if (StrategyParameters == null) {
				throw new VectoException("Model parameters for hybrid strategy required!");
			}

			GearList = runData.GearboxData.GearList;

			ElectricMotorsOff = ModelData.ElectricMachinesData
										.Select(x => new KeyValuePair<PowertrainPosition, NewtonMeter>(x.Item1, null))
										.ToDictionary(x => x.Key, x => new Tuple<PerSecond, NewtonMeter>(null, x.Value));
			var emPos = ModelData.ElectricMachinesData.First().Item1;
			ElectricMotorCanPropellDuringTractionInterruption =
				emPos == PowertrainPosition.HybridP4 || emPos == PowertrainPosition.HybridP3;

			var engineRampUpEnergy = Formulas.InertiaPower(ModelData.EngineData.IdleSpeed, 0.RPMtoRad(), ModelData.EngineData.Inertia, ModelData.EngineData.EngineStartTime) * ModelData.EngineData.EngineStartTime;
			var engineDragEnergy = VectoMath.Abs(ModelData.EngineData.FullLoadCurves[0].DragLoadStationaryTorque(ModelData.EngineData.IdleSpeed)) *
									ModelData.EngineData.IdleSpeed / 2.0 * ModelData.EngineData.EngineStartTime;

			IceRampUpCosts = (engineRampUpEnergy + engineDragEnergy).Value() / DeclarationData.AlternatorEfficiency / DeclarationData.AlternatorEfficiency;

			IceIdlingCosts = ModelData.EngineData.Fuels.Sum(
				x => (x.ConsumptionMap.GetFuelConsumptionValue(0.SI<NewtonMeter>(), ModelData.EngineData.IdleSpeed)
					* x.FuelData.LowerHeatingValueVecto * StrategyParameters.MinICEOnTime).Value());

            // create testcontainer
            TestPowertrain = PowertrainBuilder.CreateTestPowertrain(DataBus, true);

			var shiftStrategyParameters = runData.GearshiftParameters;
			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}
			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				WarnGearShiftRange();
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}

			// TODO: MQ 20210712 how to handle with batterysystem
			//var auxEnergyReserve = ModelData.ElectricAuxDemand * StrategyParameters.AuxReserveTime;
			BatteryDischargeEnergyThreshold = 0.SI<WattSecond>();
			//if (auxEnergyReserve > 0) {
			//	var minSoc = Math.Max(ModelData.BatteryData?.MinSOC ?? ModelData.SuperCapData.MinVoltage / ModelData.SuperCapData.MaxVoltage,
			//		StrategyParameters.MinSoC);
			//	BatteryDischargeEnergyThreshold =
			//		ModelData.BatteryData.Capacity * minSoc * ModelData.BatteryData.SOCMap.Lookup(minSoc) +
			//		auxEnergyReserve;
			//}
			AllowEmergencyShift = false;
		}


		protected virtual void WarnGearShiftRange()
		{
			Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
        }

		public virtual IHybridController Controller { protected get; set; }

		public PerSecond MinICESpeed => ModelData.EngineData.IdleSpeed;

		public bool AllowEmergencyShift { protected get; set; }

		public event Action GearShiftTriggered;

		public virtual IHybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			GearshiftPosition nextGear;
			if (DataBus.VehicleInfo.VehicleStopped) {
				nextGear = NextGear;
			}
			else if (!DataBus.GearboxInfo.GearEngaged(absTime)) {
				nextGear = NextGear;
			}
			else if (PreviousState.GearboxEngaged) {
				nextGear = CurrentGear;
			} else {
				nextGear = NextGear;
			}

			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && (nextGear != null) 
				&& StrategyParameters.MaxPropulsionTorque?.GetVECTOValueOrDefault(nextGear) != null && DataBus.GearboxInfo.TCLocked) {
				
				var emOff = new HybridStrategyResponse {
					CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
					GearboxInNeutral = false,
					NextGear = nextGear,
					MechanicalAssistPower = ElectricMotorsOff
				};
				if (ElectricMotorCanPropellDuringTractionInterruption) {
					// for P3 or P4 let the EM compensate its own drag to get correct gearbox in torque
					emOff.MechanicalAssistPower = ElectricMotorsOff.ToDictionary(x => x.Key,
						x => Tuple.Create(x.Value.Item1, 0.SI<NewtonMeter>()));
					//emOff.MechanicalAssistPower[emOff.MechanicalAssistPower.First().Key] =
					//	Tuple.Create((PerSecond)null, 0.SI<NewtonMeter>());
				}
				var testRequest = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, emOff);
				if (testRequest.Gearbox.InputSpeed < DataBus.EngineInfo.EngineN95hSpeed) {
					var emPos = ModelData.ElectricMachinesData.First().Item1;
					var tqRequest = GetGearboxInTorqueLimitedVehiclePorpTorque(testRequest, emPos); //testRequest.Gearbox.InputTorque;
					var maxTorque = StrategyParameters.MaxPropulsionTorque[nextGear].FullLoadDriveTorque(testRequest.Gearbox.InputSpeed);

					if (!dryRun) {
						CurrentState.MaxGbxTq = maxTorque;
					}

					if (((tqRequest - maxTorque) * testRequest.Gearbox.InputSpeed).IsGreater(0,
						Constants.SimulationSettings.LineSearchTolerance)) {
						LimitedGbxTorque = true;
						DryRunSolution = null; // reset already found/selected solution - search may jump to a significantly different operating point
						return HandleRequestExceedsMaxPower(absTime, dt, outTorque, outAngularVelocity, dryRun,
							testRequest);
					}
                    else {
                        LimitedGbxTorque = false;
                    }
                }
			}

			VehicleHaltTimestamp = DataBus.VehicleInfo.VehicleStopped ? VehicleHaltTimestamp : null;
			EngineOffTimestamp = null;

			var currentGear = PreviousState.GearboxEngaged ? CurrentGear : NextGear;

			if (DryRunSolution != null && DryRunSolution.DrivingAction != DataBus.DriverInfo.DrivingAction) {
				DryRunSolution = null;
				LimitedGbxTorque = false;
			}

			if (!dryRun && DryRunSolution != null && !DryRunSolution.Solution.IgnoreReason.AllOK()) {
				DryRunSolution = null;
				LimitedGbxTorque = false;
			}

            if (dryRun && DryRunSolution != null && DryRunSolution.DrivingAction == DataBus.DriverInfo.DrivingAction && DataBus.GearboxInfo.TCLocked && !DryRunSolution.Solution.IgnoreReason.EngineSpeedBelowDownshift()) {
				var tmp = CreateResponse(DryRunSolution.Solution, currentGear);
				return tmp;
			}


			var eval = new List<HybridResultEntry>();

			switch (DataBus.DriverInfo.DrivingAction) {
				case DrivingAction.Accelerate:
					HandleAccelerateAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				case DrivingAction.Coast:
					HandleCoastAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				case DrivingAction.Roll:
					HandleRollAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				case DrivingAction.Brake:
					HandleBrakeAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				case DrivingAction.Halt:
					HandleHaltAction(absTime, dt, outTorque, outAngularVelocity, dryRun, eval);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}


			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake && eval.Count == 0) {
				eval.Add(MaxRecuperationSetting(absTime, dt, outTorque, outAngularVelocity, dryRun));
			}

			var best = SelectBestOption(eval, absTime, dt, outTorque, outAngularVelocity, dryRun, currentGear);

			if (best == null) {
				best = ResponseEmOff;
				best.ICEOff = false;
				DoEmergencyGearShift(currentGear, absTime, dt, outTorque, outAngularVelocity, best);
			}

			var retVal = CreateResponse(best, currentGear);

			retVal.GearboxEngaged = DataBus.GearboxInfo.GearEngaged(absTime);
			//if (!DataBus.EngineInfo.EngineOn && !best.ICEOff && retVal.ShiftRequired) {
			//	CurrentState.ICEStartTStmp = absTime + dt;
			//} else {
			//	CurrentState.ICEStartTStmp = PreviousState.ICEStartTStmp;
			//}
			CurrentState.Response = dryRun ? null : retVal;
			if (!dryRun) {
				CurrentState.Solution = best;
				CurrentState.AngularVelocity = outAngularVelocity;
				CurrentState.Evaluations = eval;
				CurrentState.GearboxEngaged = DataBus.GearboxInfo.GearEngaged(absTime) || DataBus.GearboxInfo.GearboxType.AutomaticTransmission();
				//if (!DataBus.EngineCtl.CombustionEngineOn && !best.ICEOff && !retVal.ShiftRequired) {
				//	CurrentState.ICEStartTStmp = absTime;
				//} else {
				//	CurrentState.ICEStartTStmp = PreviousState.ICEStartTStmp;
				//}
			}

			DryRunSolution = new DryRunSolutionState(DataBus.DriverInfo.DrivingAction, best, eval);

			if (retVal.ShiftRequired) {
				DryRunSolution = null;
				CurrentState.GearshiftTriggerTstmp = absTime;

				GearShiftTriggered?.Invoke();
			}

			DebugData.Add("AHS.R", new {
				DataBus.DriverInfo.DrivingAction,
				Evaluations = eval,
				Best = best,
				RetVal = retVal,
				DryRun = dryRun
			});
			return retVal;
		}

		protected virtual GearshiftPosition CurrentGear => DataBus.GearboxInfo.Gear;

		protected virtual void DoEmergencyGearShift(GearshiftPosition currentGear, Second absTime, Second dt, NewtonMeter outTorque, 
			PerSecond outAngularVelocity, HybridResultEntry best)
		{
			if (AllowEmergencyShift && GearList.HasSuccessor(currentGear)) {
				var testResponse = GetEmOffResultEntry(absTime, dt, outTorque, outAngularVelocity, currentGear);
				if (testResponse.Response.Engine.EngineSpeed.IsGreaterOrEqual(ModelData.EngineData.FullLoadCurves[0]
					.RatedSpeed)) {
					best.Gear = GearList.Successor(currentGear);
				}
			}
        }

		public IResponse AmendResponse(IResponse response, Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			if (response is ResponseSuccess) {
				if (!DataBus.EngineInfo.EngineOn && !CurrentState.Solution.ICEOff) {
					//CurrentState.ICEStartTStmp = absTime;
				}
			}
			//if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
			//	(outTorque * outAngularVelocity).IsEqual(StrategyParameters.MaxDrivetrainPower,
			//		Constants.SimulationSettings.LineSearchTolerance.SI<Watt>())) {
			//	if (dryRun && response is ResponseDryRun responseDryRun) {
			//		if (responseDryRun.DeltaFullLoad.IsSmaller(0)) {
			//			return new ResponseDryRun(this, responseDryRun) { DeltaFullLoad = 0.SI<Watt>() };
			//		}
			//	}
			//}

			if (dryRun && DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && StrategyParameters.MaxPropulsionTorque?.GetVECTOValueOrDefault(response.Gearbox.Gear) != null) {
				var dryRunResponse = response as ResponseDryRun;
				if (response.Engine.EngineOn && dryRunResponse.DeltaFullLoad.IsSmallerOrEqual(0) &&
					dryRunResponse.DeltaDragLoad.IsGreaterOrEqual(0) && LimitedGbxTorque) {
					// during this request the torque at gbx-in was limited - engine is ok and a seaerch operation is going on
					// overwrite delta value...
					var maxTorque =
						StrategyParameters.MaxPropulsionTorque[response.Gearbox.Gear].FullLoadDriveTorque(response.Gearbox.InputSpeed);

					var emPos = ModelData.ElectricMachinesData.First().Item1;

					var gbxInTq = GetGearboxInTorqueLimitedVehiclePorpTorque(dryRunResponse, emPos);
					dryRunResponse.DeltaFullLoad = (gbxInTq - maxTorque) *
													dryRunResponse.Gearbox.InputSpeed;
					dryRunResponse.DeltaFullLoadTorque = gbxInTq - maxTorque;
					dryRunResponse.DeltaDragLoad = (gbxInTq - maxTorque) *
													dryRunResponse.Gearbox.InputSpeed;
					dryRunResponse.DeltaDragLoadTorque = gbxInTq - maxTorque;

				}
			}

			return response;
		}

		/// <summary>
		///  get the input torque at the gearbox to calculate overload limit. in case of P3 or P4 compensate the EM torque
		/// to get the 'virtual' gearbox input torque
		/// </summary>
		/// <param name="dryRunResponse"></param>
		/// <param name="emPos"></param>
		/// <returns></returns>
		private NewtonMeter GetGearboxInTorqueLimitedVehiclePorpTorque(IResponse dryRunResponse, PowertrainPosition emPos)
		{
			var gbxInTq = dryRunResponse.Gearbox.InputTorque;
			switch (emPos) {
				case PowertrainPosition.HybridP3 when dryRunResponse.Gearbox.Gear.Gear != 0: {
					var gear = dryRunResponse.Gearbox.Gear;
					var emTq = dryRunResponse.ElectricMotor.ElectricMotorPowerMech /
								dryRunResponse.ElectricMotor.AvgDrivetrainSpeed;
					var gbxOutTqV = dryRunResponse.Gearbox.OutputTorque - emTq;

					var prevGbxSpeed = GetPrevGbxSpeed();
					var lossMap = gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value
						? ModelData.GearboxData.Gears[gear.Gear].TorqueConverterGearLossMap
						: ModelData.GearboxData.Gears[gear.Gear].LossMap;
					var gbxLoss = lossMap
						.GetTorqueLoss((dryRunResponse.Gearbox.OutputSpeed + prevGbxSpeed) / 2.0, gbxOutTqV);
					var ratio = gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value
						? ModelData.GearboxData.Gears[gear.Gear].TorqueConverterRatio
						: ModelData.GearboxData.Gears[gear.Gear].Ratio;
                        gbxInTq = gbxOutTqV / ratio + gbxLoss.Value;
					break;
				}
				case PowertrainPosition.HybridP4 when dryRunResponse.Gearbox.Gear.Gear != 0: {
					var emTq = dryRunResponse.ElectricMotor.ElectricMotorPowerMech /
								dryRunResponse.ElectricMotor.AvgDrivetrainSpeed;
					var axlOutTorque = dryRunResponse.Axlegear.OutputTorque - emTq;
					var prevAxlSpeed = (DataBus.AxlegearInfo as TransmissionComponent).PreviousState.OutAngularVelocity;
					var axleLoss = ModelData.AxleGearData.AxleGear.LossMap.GetTorqueLoss((dryRunResponse.Axlegear.OutputSpeed + prevAxlSpeed) / 2.0, axlOutTorque);
					var axlInTorque = axlOutTorque / ModelData.AxleGearData.AxleGear.Ratio + axleLoss.Value;
					var gbxInTorque = axlInTorque;
					if (ModelData.AngledriveData != null && DataBus.AxlegearInfo != null) {
						var prevAnglSpeed = (DataBus.AxlegearInfo as TransmissionComponent).PreviousState
							.OutAngularVelocity;
						var anglLoss =
								ModelData.AngledriveData.Angledrive.LossMap.GetTorqueLoss(
									(dryRunResponse.Angledrive.OutputSpeed + prevAnglSpeed) / 2.0, axlInTorque);
						var anglInTorque = axlInTorque / ModelData.AngledriveData.Angledrive.Ratio + anglLoss.Value;
						gbxInTorque = anglInTorque;
					}
					var gear = dryRunResponse.Gearbox.Gear;
					var prevGbxSpeed = GetPrevGbxSpeed();
					var lossMap = gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value
						? ModelData.GearboxData.Gears[gear.Gear].TorqueConverterGearLossMap
						: ModelData.GearboxData.Gears[gear.Gear].LossMap;
					var gbxLoss = lossMap
						.GetTorqueLoss((dryRunResponse.Gearbox.OutputSpeed + prevGbxSpeed) / 2.0, gbxInTorque);
					var ratio = gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value
						? ModelData.GearboxData.Gears[gear.Gear].TorqueConverterRatio
						: ModelData.GearboxData.Gears[gear.Gear].Ratio;
					gbxInTq = gbxInTorque / ratio + gbxLoss.Value;
					break;
				}
			}

			return gbxInTq;
		}

		private PerSecond GetPrevGbxSpeed()
		{
			switch (DataBus.GearboxInfo) {
				case IAMTGearbox gbx:
					return gbx.GetPreviousState.OutAngularVelocity;
				case IAPTGearbox atGbx:
					return atGbx.GetPreviousState.OutAngularVelocity;
				default: throw new VectoException("Unsupported gearbox type!");
			}
		}

		public void OperatingpointChangedDuringRequest(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity,
			bool dryRun, IResponse retVal)
		{
			DryRunSolution = null;
			LimitedGbxTorque = false;
		}

		public void RepeatDrivingAction(Second absTime)
		{
			DryRunSolution = null;
			LimitedGbxTorque = false;
		}

		protected abstract IResponse RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition nextGear, HybridStrategyResponse cfg);

		private IHybridStrategyResponse HandleRequestExceedsMaxPower(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun, IResponse emOffResponse)
		{
			// issue dry-run to get max available power from EM and ICE,
			// Search PWheel with max available EM power with ICE operating point on MaxTorque
			// return overload with Delta as P_out - PWheelMax

			//var responses = new List<HybridResultEntry>();

			var gearRange = GetGearRange(absTime, dryRun);

			var gear = DataBus.VehicleInfo.VehicleStopped
				? NextGear
				: PreviousState.GearboxEngaged ? CurrentGear : NextGear; // CurrentGear;

			var firstGear = GearList.Predecessor(gear, Math.Min(1, gearRange.Item1));
			var lastGear = gear; // GearList.Successor(gear, (uint)gearRange.Item2);

			var candidates = new Dictionary<GearshiftPosition, Tuple<Watt, IResponse>>();
			var maxTorqueGbxIn =
				StrategyParameters.MaxPropulsionTorque[gear].FullLoadDriveTorque(emOffResponse.Gearbox.InputSpeed);
			candidates[emOffResponse.Gearbox.Gear] = Tuple.Create(maxTorqueGbxIn * emOffResponse.Gearbox.InputSpeed, emOffResponse);
			foreach (var nextGear in GearList.IterateGears(firstGear, lastGear)) {
				if (candidates.ContainsKey(nextGear)) {
					continue;
				}
				var emOff = new HybridStrategyResponse {
					CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
					GearboxInNeutral = false,
					NextGear = nextGear,
					MechanicalAssistPower = ElectricMotorsOff
				};
				if (ElectricMotorCanPropellDuringTractionInterruption) {
					// for P3 or P4 let the EM compensate its own drag to get correct gearbox in torque
					emOff.MechanicalAssistPower = ElectricMotorsOff.ToDictionary(x => x.Key,
						x => Tuple.Create(x.Value.Item1, 0.SI<NewtonMeter>()));
					//emOff.MechanicalAssistPower[emOff.MechanicalAssistPower.First().Key] =
					//	Tuple.Create((PerSecond)null, 0.SI<NewtonMeter>());
				}
				var testRequest = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, emOff);
				if (testRequest != null && testRequest.Engine.EngineSpeed < ModelData.EngineData.FullLoadCurves[0].NTq99hSpeed) {
					var maxGbxTorque = StrategyParameters.MaxPropulsionTorque?.GetVECTOValueOrDefault(nextGear)?.FullLoadDriveTorque(testRequest.Gearbox.InputSpeed);
					candidates[nextGear] = maxGbxTorque != null
						? Tuple.Create(maxGbxTorque * testRequest.Gearbox.InputSpeed, testRequest)
						: Tuple.Create(testRequest.Gearbox.InputTorque * testRequest.Gearbox.InputSpeed, testRequest);
				}
			}

			var maxPwr = candidates.MaxBy(x => x.Value.Item1);
			if (!emOffResponse.Gearbox.Gear.Equals(maxPwr.Key)) {
				return new HybridStrategyResponse {
					ShiftRequired = true,
					NextGear = maxPwr.Key,
					//CombustionEngineOn = 
					EvaluatedSolution = new HybridResultEntry {
						Gear = maxPwr.Key,
						Response = maxPwr.Value.Item2
					},
					MechanicalAssistPower = maxPwr.Value.Item2.HybridController.StrategySettings.MechanicalAssistPower
				};
			}

			var emPos = ModelData.ElectricMachinesData.First().Item1;
			var currentGear = DataBus.VehicleInfo.VehicleStopped
				? NextGear
				: PreviousState.GearboxEngaged ? CurrentGear : NextGear;

			var maxEmDriveSetting = new HybridStrategyResponse {
				CombustionEngineOn = true,
				GearboxInNeutral = false,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
					{emPos , Tuple.Create(emOffResponse.ElectricMotor.AngularVelocity, emOffResponse.ElectricMotor.MaxDriveTorque)}
				},
			};
			var maxEmDriveResponse =
				RequestDryRun(absTime, dt, outTorque, outAngularVelocity, currentGear, maxEmDriveSetting);
			var deltaFullLoadTq = maxEmDriveResponse.Engine.TotalTorqueDemand -
								maxEmDriveResponse.Engine.DynamicFullLoadTorque;
			var maxEngineSpeed =
				maxEmDriveResponse.Gearbox.Gear.Gear == 0 || !DataBus.ClutchInfo.ClutchClosed(absTime) ||
				!DataBus.GearboxInfo.TCLocked
					? ModelData.EngineData.FullLoadCurves[0].N95hSpeed :
					VectoMath.Min(DataBus.GearboxInfo.GetGearData(CurrentGear.Gear).MaxSpeed, ModelData.EngineData.FullLoadCurves[0].N95hSpeed);


			var gbxInTq = GetGearboxInTorqueLimitedVehiclePorpTorque(emOffResponse, emPos);
			if (deltaFullLoadTq.IsSmallerOrEqual(0)) {
				// the engine is not overloaded if EM boosts, limit to max gearbox torque
				return new HybridStrategyLimitedResponse {
					//Delta = outTorque * outAngularVelocity - StrategyParameters.MaxDrivetrainPower,
					Delta = (gbxInTq - maxTorqueGbxIn) * emOffResponse.Gearbox.InputSpeed,
					DeltaEngineSpeed = maxEmDriveResponse.Engine.EngineSpeed - maxEngineSpeed, // .DeltaEngineSpeed
					GearboxResponse = maxEmDriveResponse.Gearbox,
				};
			}

			var deltaMaxTorque = gbxInTq - maxTorqueGbxIn;
			if (deltaMaxTorque.IsGreater(deltaFullLoadTq)) {
				// gearbox in torque is much higher above limit than ice operating point above full-load curve (with em boosting)
				// search to max torque curve
				// i.e. when limiting to max torque the ICE operating point is below full-load curve
				return new HybridStrategyLimitedResponse {
					Delta = (gbxInTq - maxTorqueGbxIn) * emOffResponse.Gearbox.InputSpeed,
					DeltaEngineSpeed = maxEmDriveResponse.Engine.EngineSpeed - maxEngineSpeed, // .DeltaEngineSpeed
					GearboxResponse = maxEmDriveResponse.Gearbox,
				};
			}

			// ICE operating point (with EM boosting) is higher above full-load curve than gearbox in-torque above max torque limit
			// (i.e. going to max torque point would still overload the ICE with max EM boosting)

			var avgEngineSpeed = (maxEmDriveResponse.Engine.EngineSpeed + DataBus.EngineInfo.EngineSpeed) / 2;
			var maxTorque = SearchAlgorithm.Search(outTorque, deltaFullLoadTq, -outTorque * 0.1,
				getYValue: resp => {
					var r = (IResponse)resp;
					var deltaMaxTq = r.Engine.TotalTorqueDemand - r.Engine.DynamicFullLoadTorque;
					return deltaMaxTq * avgEngineSpeed;
				},
				evaluateFunction: x => RequestDryRun(absTime, dt, x, outAngularVelocity, currentGear, maxEmDriveSetting),
				criterion: resp => {
					var r = (IResponse)resp;
					var deltaMaxTq = r.Engine.TotalTorqueDemand - r.Engine.DynamicFullLoadTorque;
					return (deltaMaxTq * avgEngineSpeed).Value();
				},
				searcher: this);
			var rqMaxTorque = RequestDryRun(absTime, dt, maxTorque, outAngularVelocity, currentGear, maxEmDriveSetting);
			// limiting to ICE FLD with max propulsion - delta gearbox torque
			var rqMaxGbxInTq = GetGearboxInTorqueLimitedVehiclePorpTorque(rqMaxTorque, emPos);
			var delta1 = (rqMaxGbxInTq - maxTorqueGbxIn) * emOffResponse.Gearbox.InputSpeed;
			var delta2 = (outTorque - maxTorque) * outAngularVelocity;
			var delta = VectoMath.Max(delta1, delta2);
			return new HybridStrategyLimitedResponse {
				Delta = delta,
				DeltaEngineSpeed = maxEmDriveResponse.Engine.EngineSpeed - maxEngineSpeed,
				GearboxResponse = maxEmDriveResponse.Gearbox,

			};
		}

		protected HybridResultEntry ResponseEmOff
		{
			get {
				return new HybridResultEntry {
					U = double.NaN,
					Response = null,
					Setting = new HybridStrategyResponse {
						GearboxInNeutral = false,
						CombustionEngineOn = DataBus.EngineInfo.EngineOn,
						MechanicalAssistPower = ElectricMotorsOff.ToDictionary(x => x.Key,
							x => Tuple.Create(x.Value.Item1, x.Value.Item2))
					},
					FuelCosts = double.NaN,
					ICEOff = !DataBus.EngineInfo.EngineOn,
					Gear = new GearshiftPosition(0),
				};
			}
		}

		protected virtual bool AllowICEOff(Second absTime)
		{
			//VECTO-1493 special case for P1: only allow turning ICE off if ESS is activated and PCC is currently active
			var isPCC = DataBus.DriverInfo.PCCState.IsOneOf(PCCStates.UseCase1, PCCStates.UseCase2);
			var isP1 = DataBus.PowertrainInfo.ElectricMotorPositions.Contains(PowertrainPosition.HybridP1);
			if (isP1 && (!ModelData.VehicleData.ADAS.EngineStopStart || !isPCC))
				return false;

			//normal case: only turn of ICE if ESS is activated and the minimal ICE on time is exceeded.
			var MinICEonTimeExceeded = PreviousState.ICEStartTStmp is null
				|| absTime.IsGreaterOrEqual(PreviousState.ICEStartTStmp + StrategyParameters.MinICEOnTime);
			return ModelData.VehicleData.ADAS.EngineStopStart && MinICEonTimeExceeded;
		}

		protected virtual void HandleBrakeAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			if (DataBus.GearboxInfo.GearboxType.AutomaticTransmission() &&
				CurrentGear.Equals(GearList.First())) {
				eval.Add(ResponseEmOff);
				return;
			}

			var debug = new DebugData();

			var emPos = ModelData.ElectricMachinesData.First().Item1;
			var disengageSpeedThreshold = ModelData.GearboxData.DisengageWhenHaltingSpeed;

            // hint: only check for halting speed if vehicle is actually braking to halt.
			var vehicleEndSpeed = DataBus.VehicleInfo.VehicleSpeed +
							DataBus.DriverInfo.DriverAcceleration * ModelData.GearboxData.TractionInterruption; 
            var vehiclespeedBelowThreshold = vehicleEndSpeed.IsSmaller(disengageSpeedThreshold)
											&& (DataBus.DriverInfo.NextBrakeTriggerSpeed?.IsEqual(0) ?? false);

			if (!ElectricMotorCanPropellDuringTractionInterruption && !DataBus.GearboxInfo.GearEngaged(absTime)) {
				var off = ResponseEmOff;
				if (vehiclespeedBelowThreshold &&
					emPos.IsOneOf(PowertrainPosition.HybridP2, PowertrainPosition.HybridP1, PowertrainPosition.IHPC, PowertrainPosition.HybridP2_5)) {
					off.Setting.GearboxInNeutral = true;
				} else {
					off.Setting.GearboxInNeutral = PreviousState.Solution?.Setting.GearboxInNeutral ?? false;
				}

				eval.Add(off);
				return;
			}

			GearshiftPosition nextGear;
			if (!DataBus.GearboxInfo.GearEngaged(absTime)) {
				nextGear = new GearshiftPosition(0);
			} else if (PreviousState.GearboxEngaged) {
				nextGear = CurrentGear;
			} else {
				nextGear = NextGear;
			}

			if (vehiclespeedBelowThreshold && emPos.IsOneOf(PowertrainPosition.HybridP2,
					PowertrainPosition.HybridP1, PowertrainPosition.IHPC, PowertrainPosition.HybridP2_5)) {
				if (DataBus.GearboxInfo.GearboxType.AutomaticTransmission()) {
					var firstgear = ResponseEmOff;
					firstgear.Gear = GearList.First();
					eval.Add(firstgear);
					return;
				}

				var testDisengage = new HybridStrategyResponse {
					CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
					GearboxInNeutral = false,
					NextGear = nextGear,
					MechanicalAssistPower = ElectricMotorsOff
				};
				var testDisengageResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear,
					testDisengage);

				var off = ResponseEmOff;

				if (testDisengageResponse.Clutch.PowerRequest.IsSmaller(0)) {
					// only disengage if the torque at the clutch is negative - i.e. no propulsion needed
					off.Setting.GearboxInNeutral = true;
				}

				eval.Add(off);
				return;
			}

			if (!nextGear.IsLockedGear()) {
				var off = ResponseEmOff;
				var offResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, off.Setting);
				if (offResponse.Source is IAPTGearbox && offResponse is ResponseOverload && GearList.HasPredecessor(nextGear)) {
					off.Gear = GearList.Predecessor(nextGear);
				}
				eval.Add(off);
				return;
			}

			var disengaged = nextGear.Gear == 0;
			//if (!disengaged && outAngularVelocity.IsEqual(0)) {
			//	var stop = ResponseEmOff;
			//	stop.Gear = new GearshiftPosition(0);
			//	eval.Add(stop);
			//	return;
			//}
			var currentGear = nextGear;
			var tmp = new HybridStrategyResponse {
				CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
				GearboxInNeutral = false,
				NextGear = nextGear,
				MechanicalAssistPower = ElectricMotorsOff
			};
			var response = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, tmp);
			debug.Add($"[AHS.HBA-0] DryRun Gear={nextGear}", response);

			response = RepeatDryRunWithDifferentGear(response, currentGear, absTime, dt, outTorque, outAngularVelocity, tmp);

			var endSpeed = DataBus.VehicleInfo.VehicleSpeed +
							DataBus.DriverInfo.DriverAcceleration * ModelData.GearboxData.TractionInterruption;
			if (EngineSpeedTooLow(response) && DataBus.EngineInfo.EngineOn
				&& (DataBus.GearboxInfo.GearboxType.ManualTransmission() ||
					DataBus.GearboxInfo.GearboxType == GearboxType.IHPC)
				&& endSpeed.IsSmallerOrEqual(disengageSpeedThreshold, 0.1.KMPHtoMeterPerSecond())) {
				var responseEmOff = ResponseEmOff;
				// if the engine speed is too low but the gear is engaged in this timestep, use the gear as calculated before. 
				// on re-engage the engine speed is checked and a lower gear is engaged. then the hybrid strategy is called again.
				responseEmOff.Gear = (!PreviousState.GearboxEngaged && DataBus.GearboxInfo.GearEngaged(absTime))
					? currentGear
					: new GearshiftPosition(0);
				responseEmOff.Setting.GearboxEngaged = false;
				responseEmOff.Setting.GearboxInNeutral =
					(!PreviousState.GearboxEngaged && DataBus.GearboxInfo.GearEngaged(absTime)) ? false : true;
				eval.Add(responseEmOff);
				return;
			}

			if (GearList.HasPredecessor(nextGear)
				&& EngineSpeedTooLow(response)
				&& (!vehiclespeedBelowThreshold || AllowEmergencyShift)) {
				// engine speed would fall below idling speed - consider downshift
				var estimatedVelocityPostShift = VelocityDropData.Valid
					? VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed,
						DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>())
					: DataBus.VehicleInfo.VehicleSpeed;
				var postShiftBelowThreshold = estimatedVelocityPostShift.IsSmaller(disengageSpeedThreshold);
				if (postShiftBelowThreshold) {
					var downshift = ResponseEmOff;
					downshift.Gear = GearList.Predecessor(nextGear);
					eval.Add(downshift);
					return;
				}

				do {
					nextGear = GearList.Predecessor(nextGear);
					response = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, tmp);
					debug.Add($"[AHS.HBA-1] DryRun Gear={nextGear}", response);
				} while (GearList.HasPredecessor(nextGear) && response == null);
			}

			if (nextGear.Equals(GearList.First()) && EngineSpeedTooLow(response)) {
				// disengage gearbox...
				var responseEmOff = ResponseEmOff;
				responseEmOff.Gear = new GearshiftPosition(0);
				responseEmOff.Setting.GearboxEngaged = false;
				responseEmOff.Setting.GearboxInNeutral = true;
				eval.Add(responseEmOff);
				return;
			}

			if (DataBus.GearboxInfo.GearboxType.AutomaticTransmission() && response == null &&
				nextGear.Equals(GearList.First())) {
				var downshift = ResponseEmOff;
				downshift.Gear = nextGear;
				eval.Add(downshift);
				return;
			}

			if (tmp.CombustionEngineOn) {
				var firstEntry = new HybridResultEntry();
				CalculateCosts(response, dt, firstEntry, AllowICEOff(absTime), dryRun);
				var minimumShiftTimePassed =
					(DataBus.GearboxInfo.LastShift + ModelData.GearshiftParameters.TimeBetweenGearshifts)
					.IsSmallerOrEqual(absTime);
				if (DataBus.GearboxInfo.GearEngaged(absTime) && !vehiclespeedBelowThreshold) {
					var reEngaged = !PreviousState.GearboxEngaged && DataBus.GearboxInfo.GearEngaged(absTime);
					if ((firstEntry.IgnoreReason.EngineSpeedBelowDownshift() &&
						!firstEntry.IgnoreReason.EngineTorqueDemandTooHigh() ||
						firstEntry.IgnoreReason.EngineSpeedTooLow()) && !reEngaged) {
						// ICE torque below FLD is OK as EM may regenerate and shift ICE operating point on drag line
						// for negative torques the shift line is vertical anyway ;-)
						var best = FindBestGearForBraking(nextGear, response);
						if (!best.Equals(currentGear)) {
							// downshift required!
							var downshift = ResponseEmOff;
							//downshift.Gear = GearList.Predecessor(nextGear);
							downshift.Gear = best; // GearList.Predecessor(nextGear);
							downshift.Setting.GearboxInNeutral = best.Gear == 0;
							downshift.Setting.ShiftRequired = best.Gear == 0;
							eval.Add(downshift);
							return;
						}
					}
				}

				if (!nextGear.Equals(currentGear) && !firstEntry.IgnoreReason.InvalidEngineSpeed()) {
					firstEntry.Gear = nextGear;
					firstEntry.Setting = tmp;
					eval.Add(firstEntry);
				}
			}

			if (response == null) {
				var responseEmOff = ResponseEmOff;
				eval.Add(responseEmOff);
				return;
			}

			var deltaDragTqFirst = disengaged
				? (response as ResponseDryRun).Gearbox.InputTorque ?? 0.SI<NewtonMeter>() //.DeltaDragLoadTorque
				: response.Engine.TotalTorqueDemand - response.Engine.DragTorque;

			if (!response.Engine.EngineOn && DataBus.GearboxInfo.GearboxType.AutomaticTransmission()) {
				deltaDragTqFirst = response.Gearbox.InputTorque;
			}

			if (deltaDragTqFirst.IsGreater(0)) {
				// braking requested but engine operating point is not below drag curve.
				if (ElectricMotorCanPropellDuringTractionInterruption) {
					if (DataBus.GearboxInfo.GearEngaged(absTime)) {
						eval.AddRange(FindSolution(absTime, dt, outTorque, outAngularVelocity, dryRun));
					} else {
						EvaluateConfigsForGear(
							absTime, dt, outTorque, outAngularVelocity, nextGear, AllowICEOff(absTime), eval, emPos,
							dryRun);
					}
				} else if (DataBus.GearboxInfo.GearEngaged(absTime)) {
					eval.AddRange(FindSolution(absTime, dt, outTorque, outAngularVelocity, dryRun));
				} else {
					eval.Add(ResponseEmOff);
				}

				return;
			}

			if (Disengaged(response) && !ElectricMotorCanPropellDuringTractionInterruption) {
				// we are disengaged and EM cannot recuperate - switch EM off
				eval.Add(ResponseEmOff);
				return;
			}

			if (response.ElectricMotor.MaxRecuperationTorque == null) {
				var retVal = ResponseEmOff;
				retVal.Gear = disengaged ? new GearshiftPosition(0) : response.Gearbox.Gear;
				eval.Add(retVal);
				return;
			}

			var maxRecuperation = new HybridStrategyResponse {
				CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
				GearboxInNeutral = false,
				NextGear = nextGear,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
					{
						emPos,
						Tuple.Create(response.ElectricMotor.AngularVelocity,
							VectoMath.Max(response.ElectricMotor.MaxRecuperationTorque, 0.SI<NewtonMeter>()))
					}
				}
			};
			var maxRecuperationResponse =
				RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, maxRecuperation);
			debug.Add("[AHS.HBA-2] DryRun maxRecuperationResponse", maxRecuperationResponse);
			var deltaDragTqMaxRecuperation = disengaged
				? (maxRecuperationResponse as ResponseDryRun).Gearbox.InputTorque ?? 0.SI<NewtonMeter>() //.DeltaDragLoadTorque
				: maxRecuperationResponse.Engine.TotalTorqueDemand - maxRecuperationResponse.Engine.DragTorque;

			var isAPTWithTorqueConverter = DataBus.GearboxInfo.GearboxType.AutomaticTransmission() &&
											DataBus.GearboxInfo.GearboxType != GearboxType.IHPC;
			if (!maxRecuperationResponse.Engine.EngineOn && isAPTWithTorqueConverter) {
				deltaDragTqMaxRecuperation = maxRecuperationResponse.Gearbox.InputTorque;
			}

			if (deltaDragTqMaxRecuperation.IsEqual(0)) {
				// with max recuperation we are already at the drag curve (e.g. because search braking power was invoked before
				eval.Add(
					new HybridResultEntry {
						ICEOff = !DataBus.EngineInfo.EngineOn,
						Gear = nextGear,
						Setting = maxRecuperation
					});
				return;
			}

			if (deltaDragTqMaxRecuperation.IsSmaller(0) &&
				maxRecuperationResponse.ElectricSystem.RESSPowerDemand.IsBetween(
					maxRecuperationResponse.ElectricSystem.MaxPowerDrag,
					maxRecuperationResponse.ElectricSystem.MaxPowerDrive)) {
				// even with full recuperation (and no braking) the operating point is below the drag curve (and the battery can handle it) - use full recuperation
				eval.Add(
					new HybridResultEntry {
						ICEOff = !DataBus.EngineInfo.EngineOn,
						Gear = nextGear,
						Setting = new HybridStrategyResponse {
							CombustionEngineOn = DataBus.EngineInfo.EngineOn,
							GearboxInNeutral = false,
							NextGear = nextGear,
							MechanicalAssistPower =
								new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
									{
										emPos,
										Tuple.Create(response.ElectricMotor.AngularVelocity,
											response.ElectricMotor.MaxRecuperationTorque)
									}
								}
						}
					});
				return;
			}

			// full recuperation is not possible - ICE would need to propel - search max possible EM torque
			var emRecuperationTq = SearchAlgorithm.Search(
				maxRecuperationResponse.ElectricMotor.ElectricMotorPowerMech /
				maxRecuperationResponse.ElectricMotor.AngularVelocity,
				maxRecuperationResponse.Engine.TorqueOutDemand,
				maxRecuperationResponse.ElectricMotor.MaxRecuperationTorque * 0.1,
				getYValue: resp => {
					var r = resp as IResponse;
					var deltaDragLoad = disengaged
						? (r as ResponseDryRun).Gearbox.InputTorque //.DeltaDragLoadTorque
						: r.Engine.TotalTorqueDemand - r.Engine.DragTorque;
					if (!r.Engine.EngineOn && isAPTWithTorqueConverter) {
						deltaDragLoad = r.Gearbox.InputTorque;
					}

					return deltaDragLoad;
				},
				evaluateFunction: emTq => {
					var cfg = new HybridStrategyResponse {
						CombustionEngineOn = DataBus.EngineInfo.EngineOn,
						GearboxInNeutral = false,
						NextGear = nextGear,
						MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
							{ emPos, Tuple.Create(response.ElectricMotor.AngularVelocity, emTq) }
						}
					};
					return RequestDryRun(absTime, dt, outTorque, outAngularVelocity,
						DataBus.GearboxInfo.GearEngaged(absTime) ? nextGear : new GearshiftPosition(0), cfg);
				},
				criterion: resp => {
					var r = resp as IResponse;
					var deltaDragLoad = disengaged
						? (r as ResponseDryRun).Gearbox.InputTorque //.DeltaDragLoadTorque
						: r.Engine.TotalTorqueDemand - r.Engine.DragTorque;
					if (!r.Engine.EngineOn && isAPTWithTorqueConverter) {
						deltaDragLoad = r.Gearbox.InputTorque;
					}

					return deltaDragLoad.Value();
				},
				searcher: this
			);
			if (emRecuperationTq.IsBetween(
					response.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
					response.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>())) {
				var entry = new HybridResultEntry {
					ICEOff = !DataBus.EngineInfo.EngineOn,
					Gear = nextGear,
					Setting = new HybridStrategyResponse {
						CombustionEngineOn = DataBus.EngineInfo.EngineOn,
						GearboxInNeutral = false,
						NextGear = nextGear,
						MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
							{ emPos, Tuple.Create(response.ElectricMotor.AngularVelocity, emRecuperationTq) }
						}
					}
				};
				entry.Response = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, entry.Setting);
				if (entry.Response.ElectricSystem.ConsumerPower.IsGreater(0)) {
					eval.Add(entry);
				} else {
					// for the found operating point, although recuperating no electric energy is generated. leave EM off
					var off = ResponseEmOff;
					if (vehiclespeedBelowThreshold &&
						emPos.IsOneOf(PowertrainPosition.HybridP2, PowertrainPosition.HybridP1, PowertrainPosition.HybridP2_5, PowertrainPosition.IHPC)) {
						off.Setting.GearboxInNeutral = true;
					} else {
						off.Setting.GearboxInNeutral = PreviousState.Solution?.Setting.GearboxInNeutral ?? false;
					}

					eval.Add(off);
				}
			} else {
				if (emRecuperationTq.IsGreater(0)) {
					var voltage = DataBus.BatteryInfo.InternalVoltage;
					var maxbatDragTq = DataBus.ElectricMotorInfo(emPos).GetTorqueForElectricPower(voltage,
						response.ElectricSystem.MaxPowerDrag, response.ElectricMotor.AngularVelocity, dt, nextGear,
						false);
					eval.Add(
						new HybridResultEntry {
							ICEOff = !DataBus.EngineInfo.EngineOn,
							Gear = nextGear,
							Setting = new HybridStrategyResponse {
								CombustionEngineOn = DataBus.EngineInfo.EngineOn,
								GearboxInNeutral = false,
								NextGear = nextGear,
								MechanicalAssistPower =
									new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
										{
											emPos,
											Tuple.Create(response.ElectricMotor.AngularVelocity,
												VectoMath.Min(maxbatDragTq,
													response.ElectricMotor.MaxRecuperationTorque))
										}
									}
							}
						});
				} else {
					eval.Add(ResponseEmOff);
				}
			}
		}

		protected virtual IResponse RepeatDryRunWithDifferentGear(IResponse response, GearshiftPosition currentGear, Second absTime,
			Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, HybridStrategyResponse tmp)
		{
			return response;
        }

		protected virtual GearshiftPosition NextGear => Controller.ShiftStrategy.NextGear;

		protected virtual bool Disengaged(IResponse response)
		{
			return !response.Gearbox.Gear.Engaged;
        }

		private bool EngineSpeedTooLow(IResponse firstResponse)
		{
			if (firstResponse == null) {
				return false;
			}

			if (DataBus.GearboxInfo.GearboxType.AutomaticTransmission() && DataBus.GearboxInfo.GearboxType != GearboxType.IHPC) {
				return firstResponse.Engine.EngineOn
					? firstResponse.Engine.EngineSpeed.IsSmaller(ModelData.EngineData.IdleSpeed)
					: (firstResponse.Gearbox.InputSpeed?.IsSmaller(ModelData.EngineData.IdleSpeed) ?? false);
			}
			
			return DetermineEngineSpeedTooLow(firstResponse);
        }

		protected virtual bool DetermineEngineSpeedTooLow(IResponse firstResponse)
        {
			return firstResponse.Clutch.OutputSpeed.IsSmaller(ModelData.EngineData.IdleSpeed); //  firstResponse.Gearbox.InputSpeed.IsSmaller(ModelData.EngineData.IdleSpeed));
		}

		private GearshiftPosition FindBestGearForBraking(GearshiftPosition nextGear, IResponse firstResponse)
		{
			var endSpeed = DataBus.VehicleInfo.VehicleSpeed +
							DataBus.DriverInfo.DriverAcceleration * ModelData.GearboxData.TractionInterruption;

			// only disengage if we are actually braking for halting (meaning: next brake trigger speed is 0).
			if (DataBus.GearboxInfo.GearboxType.ManualTransmission()
				&& (DataBus.DriverInfo.NextBrakeTriggerSpeed?.IsEqual(0) ?? false)
				&& endSpeed.IsSmallerOrEqual(ModelData.GearboxData.DisengageWhenHaltingSpeed, 0.1.KMPHtoMeterPerSecond())) {
				return new GearshiftPosition(0);
			}

			var tmpGear = new GearshiftPosition(nextGear.Gear, nextGear.TorqueConverterLocked);
			var candidates = new Dictionary<GearshiftPosition, PerSecond>();
			var gbxOutSpeed = firstResponse.Engine.EngineSpeed /
							ModelData.GearboxData.Gears[tmpGear.Gear].Ratio;
			var firstGear = GearList.Predecessor(nextGear, 1);
			var lastGear = GearList.Predecessor(nextGear, (uint)ModelData.GearshiftParameters.AllowedGearRangeFC);
			//while (GearList.HasPredecessor(tmpGear)) {
			foreach (var gear in GearList.IterateGears(firstGear, lastGear)) {
				var ratio = gear.IsLockedGear()
					? ModelData.GearboxData.Gears[gear.Gear].Ratio
					: ModelData.GearboxData.Gears[gear.Gear].TorqueConverterRatio;
				candidates[gear] = gbxOutSpeed * ratio;
				//tmpGear = GearList.Predecessor(tmpGear);
			}

			var targetEngineSpeed = ModelData.EngineData.IdleSpeed +
									0.7 * (ModelData.EngineData.FullLoadCurves[0].NP98hSpeed - ModelData.EngineData.IdleSpeed);
			var best = candidates.MinBy(x => VectoMath.Abs(x.Value - targetEngineSpeed)).Key;
			return best;
		}

		protected virtual void HandleCoastAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			var nextGear = !DataBus.GearboxInfo.GearEngaged(absTime)
				? new GearshiftPosition(0)
				: PreviousState.GearboxEngaged
					? CurrentGear
					: NextGear;
			var tmp = new HybridStrategyResponse {
				CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
				GearboxInNeutral = false,
				NextGear = nextGear,
				MechanicalAssistPower = ElectricMotorsOff
			};
			var resp = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, tmp);
			if (!dryRun && resp != null) {
				// resp.Engine.EngineSpeed != null && resp.Gearbox.Gear > 1 && ModelData.GearboxData
				//.Gears[resp.Gearbox.Gear].ShiftPolygon
				//.IsBelowDownshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				var engineSpeed = resp.Gearbox.InputSpeed;
				if (resp.Gearbox.Gear.IsLockedGear() && GearList.HasPredecessor(resp.Gearbox.Gear) && ModelData.GearboxData
					.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon
					.IsBelowDownshiftCurve(resp.Engine.TorqueOutDemand, engineSpeed)) {
					// consider downshift
					var downshift = ResponseEmOff;
					downshift.Gear = GearList.Predecessor(nextGear);
					eval.Add(downshift);
					return;
				}
				if (resp.Gearbox.Gear.IsLockedGear() && !resp.Gearbox.Gear.Equals(new GearshiftPosition(0)) && 
					GearList.HasSuccessor(resp.Gearbox.Gear) && 
					ModelData.GearboxData
					.Gears[resp.Gearbox.Gear.Gear].ShiftPolygon
					.IsAboveUpshiftCurve(resp.Engine.TorqueOutDemand, engineSpeed)) {
					// consider upshift
					if (DataBus.DriverInfo.DriverAcceleration.IsGreaterOrEqual(0)) {
						var upshift = ResponseEmOff;
						upshift.Gear = GearList.Successor(nextGear);
						eval.Add(upshift);
						return;
					}
				}
			}

			var result = ResponseEmOff;
			if (DataBus.DriverInfo.PCCState.IsOneOf(PCCStates.UseCase1, PCCStates.UseCase2)) {
				result.ICEOff = AllowICEOff(absTime);

				if (DataBus.PowertrainInfo.ElectricMotorPositions.Contains(PowertrainPosition.HybridP1)) {
					// special logic for HybridP1 (VECTO-1493)
					result.Setting.GearboxInNeutral = true;
					result.ICEOff &= ModelData.VehicleData.ADAS.EngineStopStart;
				}
				result.Setting.CombustionEngineOn = !result.ICEOff;
			}
			eval.Add(result);
		}

		protected virtual void HandleRollAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			var tmp = ResponseEmOff;
			if (AllowEmergencyShift) {
				tmp.Setting.GearboxInNeutral = true;
			}
			eval.Add(tmp);

			// in case of P3 or P4 the EM could propell/recuperate during roll acttion. but as we have no information
			// what the real vehicle does lets skip this

			//if (ElectricMotorCanPropellDuringTractionInterruption) {
			//	//eval = FindSolutionDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
			//	eval.Add(responseEmOff);
			//} else {
			//	eval.Add(responseEmOff);
			//}
		}

		protected virtual void HandleHaltAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			var iceOn = ModelData.VehicleData.ADAS.EngineStopStart ? DataBus.EngineInfo.EngineOn : true;

			if (ModelData.VehicleData.ADAS.EngineStopStart) {
				if (VehicleHaltTimestamp == null) {
					VehicleHaltTimestamp = absTime;
				}

				if ((absTime - VehicleHaltTimestamp).IsGreaterOrEqual(
					ModelData.DriverData.EngineStopStart.EngineOffStandStillActivationDelay)) {
					if (EngineOffTimestamp == null) {
						EngineOffTimestamp = absTime;
						iceOn = false;
					}
				}

				if (EngineOffTimestamp != null &&
					(absTime - EngineOffTimestamp).IsGreaterOrEqual(ModelData.DriverData.EngineStopStart
						.MaxEngineOffTimespan)) {
					iceOn = true;
				}
			}

			var tmp = ResponseEmOff;
			tmp.Setting.GearboxInNeutral = false;
			//tmp.Setting.NextGear = startg
			tmp.Setting.CombustionEngineOn = iceOn;
			tmp.ICEOff = !iceOn;

			eval.Add(tmp);
		}

		protected virtual void HandleAccelerateAction(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun, List<HybridResultEntry> eval)
		{
			if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime) ||
				DataBus.GearboxInfo.GearboxType.AutomaticTransmission()) {
				eval.AddRange(FindSolution(absTime, dt, outTorque, outAngularVelocity, dryRun));
			} else {
				eval.Add(ResponseEmOff);
			}
		}

		private HybridResultEntry SelectBestOption(
			List<HybridResultEntry> eval, Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun, GearshiftPosition currentGear)
		{
			var eval2 = eval.Where(x =>
					(x.IgnoreReason & HybridConfigurationIgnoreReason.VehicleSpeedBelowMinSpeedAfterGearshift) == 0)
				.ToList();
			var prohibitGearshift = eval.Where(x => !x.Gear.Equals(currentGear)).All(x =>
				(x.IgnoreReason & HybridConfigurationIgnoreReason.VehicleSpeedBelowMinSpeedAfterGearshift) != 0);
			var best = DoSelectBestOption(eval2, absTime, dt, outTorque, outAngularVelocity, dryRun, currentGear);
			if (best == null) {
				return null;
			}
			if (!best.IgnoreReason.InvalidEngineSpeed() || /*best.ICEOff ||*/
				eval.Select(x => x.Gear).Distinct().Count() <= 1) {
				best.SimulationInterval = dt;
				return best;
			}

			if (!prohibitGearshift) {
				// selected solution has invalid engine speed and engine is on and evaluation contains only one gear - allow emergency shift
				if (best.IgnoreReason.EngineSpeedAboveUpshift()) {
					//try upshift
					var newEval = new List<HybridResultEntry>();
					EvaluateConfigsForGear(
						absTime, dt, outTorque, outAngularVelocity, GearList.Successor(best.Gear), AllowICEOff(absTime),
						newEval,
						best.Setting.MechanicalAssistPower.First().Key, dryRun);
					if (newEval.Count > 0) {
						var newBest = DoSelectBestOption(newEval, absTime, dt, outTorque, outAngularVelocity, dryRun,
							currentGear);
						if ((newBest != null) && !newBest.IgnoreReason.EngineSpeedTooHigh()) {
							best = newBest;
						}
					}
				}
			}

			if ((!prohibitGearshift || AllowEmergencyShift) && (best.IgnoreReason.EngineSpeedBelowDownshift() || best.IgnoreReason.EngineSpeedTooLow())) {
				best = DoSelectBestOption(eval, absTime, dt, outTorque, outAngularVelocity, dryRun, currentGear);
				if (best.IgnoreReason.EngineSpeedBelowDownshift()) {
					//try downshift
					var newEval = new List<HybridResultEntry>();
					EvaluateConfigsForGear(
						absTime, dt, outTorque, outAngularVelocity, GearList.Predecessor(best.Gear),
						AllowICEOff(absTime), newEval,
						best.Setting.MechanicalAssistPower.First().Key, dryRun);
					if (newEval.Count > 0) {
						var oldBest = best;
						best = DoSelectBestOption(newEval, absTime, dt, outTorque, outAngularVelocity, dryRun,
							currentGear);
						if (best == null) {
							best = oldBest;
						}
					}
				}
			}

			if ((!prohibitGearshift || AllowEmergencyShift) && (best.IgnoreReason.EngineSpeedAboveUpshift() || best.IgnoreReason.EngineSpeedTooHigh())) {
				best = DoSelectBestOption(eval, absTime, dt, outTorque, outAngularVelocity, dryRun, currentGear);
				if (best.IgnoreReason.EngineSpeedAboveUpshift()) {
					//try upshift
					var newEval = new List<HybridResultEntry>();
					EvaluateConfigsForGear(
						absTime, dt, outTorque, outAngularVelocity, GearList.Successor(best.Gear),
						AllowICEOff(absTime), newEval,
						best.Setting.MechanicalAssistPower.First().Key, dryRun);
					if (newEval.Count > 0) {
						best = DoSelectBestOption(newEval, absTime, dt, outTorque, outAngularVelocity, dryRun,
							currentGear);
					}
				}
			}

			if (best != null) {
				best.SimulationInterval = dt;
				best.ProhibitGearshift = prohibitGearshift;
			}

			return best;
		}

		private HybridResultEntry DoSelectBestOption(
			List<HybridResultEntry> eval, Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun, GearshiftPosition currentGear)
		{
			if (eval.Count == 0) {
				return null;
			}
			HybridResultEntry best;

			if (DataBus.VehicleInfo.VehicleSpeed.IsSmallerOrEqual(ModelData.GearshiftParameters.StartSpeed) &&
				DataBus.VehicleInfo.VehicleSpeed.IsSmaller(DataBus.DrivingCycleInfo.TargetSpeed, 1.KMPHtoMeterPerSecond())) {
				best = eval.Where(x => !double.IsNaN(x.Score)).Where(x => !x.IgnoreReason.EngineSpeedTooHigh())
							.OrderBy(x => x.Score).FirstOrDefault();
			} else {
				best = eval.Where(x => !double.IsNaN(x.Score)).Where(x => x.IgnoreReason.AllOK()).OrderBy(x => x.Score)
								.FirstOrDefault();
			}
			if (best != null) {
				return best;
			}

			if (!DataBus.Brakes.BrakePower.IsEqual(0)) {
				// if the brakes are activated we already searched for a valid operating point - do not allow solutions below ice max or ice min torque
				best = eval.Where(x =>
						!double.IsNaN(x.Score) && !x.IgnoreReason.EngineSpeedTooLow() &&
						!x.IgnoreReason.EngineSpeedTooHigh() && x.IgnoreReason.EngineTorqueOK()).OrderBy(x => x.Score)
					.FirstOrDefault();
			} else {
				best = eval.Where(x => !double.IsNaN(x.Score) && !x.IgnoreReason.InvalidEngineSpeed())
					.OrderBy(x => x.Score)
					.FirstOrDefault();
			}
			//best = eval.Where(x => !double.IsNaN(x.Score) && !x.IgnoreReason.InvalidEngineSpeed()).OrderBy(x => x.Score)
			//			.FirstOrDefault();
			if (best != null) {
				return best;
			}

			best = eval.Where(x => !double.IsNaN(x.Score)).OrderBy(x => x.Score).FirstOrDefault();
			if (best != null && !(best.Gear != currentGear && best.IgnoreReason.EngineSpeedTooHigh())) {
				return best;
			}

			var validResponses = eval.Where(x => x.Response != null).ToArray();
			var allOverload = validResponses.Where(x => !(x.IgnoreReason.BatteryDemandExceeded() || x.IgnoreReason.BatterySoCTooLow()))
								.All(x => x.IgnoreReason.EngineTorqueDemandTooHigh());
			var allUnderload = validResponses.All(x => x.IgnoreReason.EngineTorqueDemandTooLow());
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && allOverload) {
				if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime)) {
					// overload, EM can support - use solution with max EM power
					var filtered = eval.Where(x => !x.IgnoreReason.BatteryDemandExceeded() && !x.IgnoreReason.BatterySoCTooLow())
										.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
					if (filtered.Length == 0) {
						best = eval.Where(x => !x.IgnoreReason.BatteryDemandExceeded())
							.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear)))
							.ThenBy(x => -x.Response.ElectricSystem.RESSPowerDemand.Value()).FirstOrDefault();
						return best;
					}

					best = filtered
						.Where(x => !x.IgnoreReason.InvalidEngineSpeed())
						.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear)))
						.ThenBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()))
						.FirstOrDefault();
					if (best == null) {
						best = filtered
							.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear)))
							.ThenBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()))
							.FirstOrDefault();
					}
					return best;
				}
			}
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && allUnderload) {
				var emPos = DataBus.PowertrainInfo.ElectricMotorPositions.First(x => x != PowertrainPosition.GEN);
				if (DataBus.GearboxInfo.GearboxType.AutomaticTransmission() && DataBus.VehicleInfo.VehicleStopped &&
					emPos == PowertrainPosition.HybridP1) {
					best = eval.First(x => double.IsNaN(x.U));
					return best;
				}
				if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime)) {
					var filtered = eval.Where(x => !x.IgnoreReason.InvalidEngineSpeed())
										.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
					if (!filtered.Any()) {
						filtered = eval.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
					}
					best = filtered.Where(x => !x.IgnoreReason.BatteryDemandExceeded()).MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
					if (best != null) {
						return best;
					}
				}
			}

			var emEngaged = !ElectricMotorCanPropellDuringTractionInterruption ||
							DataBus.GearboxInfo.GearEngaged(absTime) && (eval.First().Response?.Gearbox.Gear.Engaged ?? true);
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && emEngaged) {
				//var filtered = eval.Where(x => !x.IgnoreReason.InvalidEngineSpeed()).ToArray();
				var batOK = eval.Where(x => !x.IgnoreReason.BatteryDemandExceeded()).ToArray();
				if (!batOK.Any()) {
					batOK = eval.ToArray();
				}
				var filtered = batOK.Where(x => !x.IgnoreReason.EngineSpeedTooLow() && !x.IgnoreReason.EngineSpeedTooHigh()).ToArray();
				if (filtered.Length == 0) {
					filtered = eval.Where(x => !x.IgnoreReason.EngineSpeedTooLow() && !x.IgnoreReason.EngineSpeedTooHigh()).ToArray();
				}
				if (filtered.Length == 0) {
					filtered = eval.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
				}
				var filtered2 = filtered.Where(x => !x.IgnoreReason.EngineTorqueDemandTooLow()).ToArray();
				if (filtered2.Length == 0) {
					filtered2 = filtered.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
				}

				var filtered3 = filtered2
					.Where(x => (x.IgnoreReason & HybridConfigurationIgnoreReason.BatteryBelowMinSoC) == 0)
					.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
				if (filtered3.Length == 0) {
					filtered3 = filtered2;
				}
				var filteredCurrentGear = filtered3.Where(x => x.Gear.Equals(currentGear)).ToArray();
				if (filteredCurrentGear.Length > 0) {
					if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake) {
						best = filteredCurrentGear.MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
					} else {
						best = filteredCurrentGear.MinBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
					}
					return best;
				}

				if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake) {
					best = filtered3.MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
				} else {
					best = filtered3.MinBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
				}
				if (best != null) {
					return best;
				}
			}
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake && emEngaged) {
				best = eval.Where(x => !x.IgnoreReason.BatteryDemandExceeded()).MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
				if (best != null && !best.IgnoreReason.InvalidEngineSpeed()) {
					return best;
				}
				var filtered = eval.Where(x => !x.IgnoreReason.BatteryDemandExceeded()).ToArray();
				var filtered2 = filtered.Where(x => !x.IgnoreReason.EngineSpeedTooLow() && !x.IgnoreReason.EngineSpeedTooHigh()).ToArray();
				if (filtered2.Length == 0) {
					filtered2 = filtered.OrderBy(x => Math.Abs(GearList.Distance(currentGear, x.Gear))).ToArray();
				}

				best = filtered2.MaxBy(x =>
					x.Setting.MechanicalAssistPower.Sum(e => e.Value?.Item2 ?? 0.SI<NewtonMeter>()));
				if (best != null) {
					return best;
				}
			}
			return eval.FirstOrDefault();
		}


		private HybridStrategyResponse CreateResponse(HybridResultEntry best, GearshiftPosition currentGear)
		{
			var retVal = new HybridStrategyResponse {
				CombustionEngineOn = !best.ICEOff,
				GearboxInNeutral = best.Setting.GearboxInNeutral,
				MechanicalAssistPower = best.Setting.MechanicalAssistPower,
				ShiftRequired = best.Gear.Engaged && !best.Gear.Equals(currentGear), //  gs?.Item1 ?? false,
				NextGear = best.Gear, // gs?.Item2 ?? 0,
				EvaluatedSolution = best,
				SimulationInterval = best.SimulationInterval,
				ProhibitGearshift = best.ProhibitGearshift,
			};
			//var pos = retVal.MechanicalAssistPower.Keys.First();
			//if (retVal.MechanicalAssistPower[pos].Item1 == null) {
			//	retVal.MechanicalAssistPower[pos] = Tuple.Create(best.Response.ElectricMotor.AngularVelocity, retVal.MechanicalAssistPower[pos].Item2);
			//}
			if (best.IgnoreReason.EngineSpeedTooHigh() && !DataBus.EngineInfo.EngineOn) {
				// ICE is off, selected solution has a too low or too high engine speed - keep ICE off
				retVal.CombustionEngineOn = false;
			}
			if (best.IgnoreReason.EngineSpeedTooLow() && !DataBus.EngineInfo.EngineOn
				&& DataBus.VehicleInfo.VehicleSpeed.IsGreater(ModelData.GearshiftParameters.StartSpeed)) {
				// ICE is off, selected solution has a too low or too high engine speed - keep ICE off
				retVal.CombustionEngineOn = false;
			}
			return retVal;
		}


		private HybridResultEntry MaxRecuperationSetting(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			var first = new HybridStrategyResponse {
				CombustionEngineOn = DataBus.EngineInfo.EngineOn, // AllowICEOff(absTime), 
				GearboxInNeutral = false,
				MechanicalAssistPower = ElectricMotorsOff
			};
			var currentGear = PreviousState.GearboxEngaged ? CurrentGear : NextGear;
			var firstResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, currentGear, first);

			var emPos = ModelData.ElectricMachinesData.First().Item1;

			var emTorque = !ElectricMotorCanPropellDuringTractionInterruption && (!firstResponse.Gearbox.Gear.Engaged || !DataBus.GearboxInfo.GearEngaged(absTime)) ? null : firstResponse.ElectricMotor.MaxRecuperationTorque;
			return TryConfiguration(absTime, dt, outTorque, outAngularVelocity, currentGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorque), double.NaN, AllowICEOff(absTime), dryRun);
		}

		protected virtual List<HybridResultEntry> FindSolution(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			var duringTractionInterruption = (PreviousState.GearshiftTriggerTstmp + ModelData.GearboxData.TractionInterruption).IsGreaterOrEqual(absTime, ModelData.GearboxData.TractionInterruption / 20);
			var allowICEOff = AllowICEOff(absTime) && (!DataBus.EngineInfo.EngineOn || !duringTractionInterruption); //DataBus.GearboxInfo.GearEngaged(absTime);

			var emPos = ModelData.ElectricMachinesData.First().Item1;

			var gearRange = GetGearRange(absTime, dryRun);

			var gear = PreviousState.GearboxEngaged ? CurrentGear : NextGear; // CurrentGear;

			var firstGear = GearList.Predecessor(gear, gearRange.Item1);
			var lastGear = GearList.Successor(gear, gearRange.Item2);

			var responses = new List<HybridResultEntry>();

			var allowEmergencyUpshift = false;
			var allowEmergencyDownshift = false;
			foreach (var nextGear in GearList.IterateGears(firstGear, lastGear)) {

				var emOffEntry = EvaluateConfigsForGear(absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, responses, emPos, dryRun);

				if (emOffEntry == null) {
					continue;
				}
				if (nextGear.Equals(gear) && gearRange.Item2 == 0 && (emOffEntry.IgnoreReason & HybridConfigurationIgnoreReason.EngineSpeedTooHigh) != 0) {
					allowEmergencyUpshift = true;
				}
				if (nextGear.Equals(gear) && gearRange.Item1 == 0 && (emOffEntry.IgnoreReason & HybridConfigurationIgnoreReason.EngineSpeedTooLow) != 0) {
					allowEmergencyDownshift = true;
				}
			}

			var tmpBest = responses.Where(x => !double.IsNaN(x.Score)).OrderBy(x => x.Score).FirstOrDefault();
			if (allowEmergencyUpshift && tmpBest != null && !tmpBest.ICEOff) {
				var nextGear = GearList.Successor(gear);
				var emOffEntry = EvaluateConfigsForGear(
					absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, responses, emPos, dryRun);
			}
			if (allowEmergencyDownshift && tmpBest != null && !tmpBest.ICEOff) {
				var nextGear = GearList.Predecessor(gear);
				if (nextGear != null) {
					var emOffEntry = EvaluateConfigsForGear(
						absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, responses, emPos, dryRun);
                }
			}

			return responses;
		}

		protected Tuple<uint, uint> GetGearRange(Second absTime, bool dryRun)
		{
			var minimumShiftTimePassed =
				(DataBus.GearboxInfo.LastShift + ModelData.GearshiftParameters.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
			var gearRangeUpshift = ModelData.GearshiftParameters.AllowedGearRangeFC;
			var gearRangeDownshift = ModelData.GearshiftParameters.AllowedGearRangeFC;
			if (!AllowEmergencyShift) {
				if (dryRun || !minimumShiftTimePassed ||
					(absTime - DataBus.GearboxInfo.LastUpshift).IsSmaller(
						ModelData.GearshiftParameters.DownshiftAfterUpshiftDelay /*, 0.1*/)
					|| DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
					DataBus.VehicleInfo.VehicleSpeed.IsSmaller(5.KMPHtoMeterPerSecond())) {
					gearRangeDownshift = 0;
				}

				if (dryRun || !minimumShiftTimePassed ||
					(absTime - DataBus.GearboxInfo.LastDownshift).IsSmaller(
						ModelData.GearshiftParameters.UpshiftAfterDownshiftDelay /*,0.1*/)
					|| DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
					DataBus.VehicleInfo.VehicleSpeed.IsSmaller(5.KMPHtoMeterPerSecond())) {
					gearRangeUpshift = 0;
				}
			}

			return Tuple.Create((uint)gearRangeDownshift, (uint)gearRangeUpshift);
		}

		protected HybridResultEntry EvaluateConfigsForGear(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition nextGear, bool allowICEOff,
			List<HybridResultEntry> responses, PowertrainPosition emPos, bool dryRun)
		{
			var emOffEntry = GetEmOffResultEntry(absTime, dt, outTorque, outAngularVelocity, nextGear);
			if (emOffEntry == null) {
				return null;
			}

			//if (StrategyParameters.MaxPropulsionTorque != null && ModelData.GearboxData.Type.AutomaticTransmission()) {
			//	var maxTqNextGear =
			//		StrategyParameters.MaxPropulsionTorque.FullLoadDriveTorque(emOffEntry.Response.Gearbox.InputSpeed);
			//	if (!AllowEmergencyShift && ((emOffEntry.Response.Gearbox.InputTorque - maxTqNextGear) * emOffEntry.Response.Gearbox.InputSpeed).IsGreater(0, Constants.SimulationSettings.LineSearchTolerance)) {
			//		return null;
			//	}
			//}

			CalculateCosts(emOffEntry.Response, dt, emOffEntry, allowICEOff, dryRun);

			responses.Add(emOffEntry);

			var emTqReq = emOffEntry.Response.ElectricMotor.TorqueRequest; // +
																		   //emOffEntry.Response.ElectricMotor.InertiaTorque;

			IterateEMTorque(
				absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, emOffEntry.Response, emTqReq, emPos, responses, dryRun);
			return emOffEntry;
		}

		private HybridResultEntry GetEmOffResultEntry(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition nextGear)
		{
			var emOffSetting = new HybridStrategyResponse {
				CombustionEngineOn = true,
				GearboxInNeutral = false,
				MechanicalAssistPower = ElectricMotorsOff
			};
			var emOffResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, emOffSetting);

			if (emOffResponse == null) {
				return null;
			}

			var entry = new HybridResultEntry {
				U = double.NaN,
				Response = emOffResponse,
				Setting = emOffSetting,
				Gear = nextGear
			};
			return entry;
		}

		private void IterateEMTorque(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			GearshiftPosition nextGear, bool allowIceOff, IResponse firstResponse, NewtonMeter emTqReq, PowertrainPosition emPos,
			List<HybridResultEntry> responses, bool dryRun)
		{
			const double stepSize = 0.1;

			// iterate over 'EM provides torque'. allow EM to provide more torque in order to overcome ICE inertia
			var maxEmTorque = firstResponse.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>();
			var maxU = allowIceOff
				? -1.0
				: Math.Min(maxEmTorque.Value() / emTqReq.Value(), -1.0);
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake || double.IsNaN(maxU)) {
				maxU = 0;
			}

			if (emTqReq.IsEqual(0, 1)) {
				maxU = 0;
			}

			var voltage = DataBus.BatteryInfo.InternalVoltage;
			if (firstResponse.ElectricMotor.MaxDriveTorque != null && (ElectricMotorCanPropellDuringTractionInterruption || firstResponse.Gearbox.Gear.Engaged)) {
				for (var u = -stepSize; u >= maxU; u -= stepSize * (u < -10 ? 100 : u < -4 ? 10 : u < -2 ? 5 : 1)) {
					var emTorque = emTqReq.Abs() * u;
					if (!emTorque.IsBetween(
						0.SI<NewtonMeter>(), firstResponse.ElectricMotor.MaxDriveTorque)) {
						continue;
					}

					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorque), u, allowIceOff, dryRun);
					responses.Add(tmp);
				}

				// make sure the max drive point is also covered.
				var batEnergyAvailable = (DataBus.BatteryInfo.StoredEnergy - BatteryDischargeEnergyThreshold) / dt;
				var emDrivePower = -(batEnergyAvailable - ModelData.ElectricAuxDemand);
				var emTorqueM = emTqReq * maxU;
				if (!responses.Any(x => x.Gear.Equals(nextGear) && x.U.IsEqual(maxU)) && emTorqueM.IsBetween(
					VectoMath.Min(0.SI<NewtonMeter>(), firstResponse.ElectricMotor.MaxRecuperationTorque), firstResponse.ElectricMotor.MaxDriveTorque)) {
					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorqueM), maxU, allowIceOff, dryRun);
					responses.Add(tmp);
				}
				if (maxEmTorque.IsSmaller(0) && emTqReq.IsGreater(-maxEmTorque)) {
					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos,
						Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, maxEmTorque), maxEmTorque / emTqReq,
						allowIceOff, dryRun);
					if (!tmp.Response?.ElectricSystem.ConsumerPower.IsSmaller(emDrivePower) ?? false) {
						responses.Add(tmp);
					}
				}
				// if battery is getting empty try to set EM-torque to discharge battery to lower SoC boundary

				if (maxEmTorque.IsSmaller(0) && emDrivePower.IsGreaterOrEqual(maxEmTorque * firstResponse.ElectricMotor.AngularVelocity)) {
					// maxEmTorque < 0  ==> EM can still propel
					// (-emDrivePower).IsGreaterOrEqual(maxEmTorque * firstResponse.ElectricMotor.AngularVelocity) ==> power available from battery for driving does not exceed max EM power (otherwise torque lookup may fail) 
					//var emDriveTorque = ModelData.ElectricMachinesData.Where(x => x.Item1 == emPos).First().Item2.EfficiencyMap
					//							.LookupTorque(emDrivePower, firstResponse.ElectricMotor.AngularVelocity, maxEmTorque);

					var emDriveTorque = DataBus.ElectricMotorInfo(emPos).GetTorqueForElectricPower(voltage, emDrivePower, firstResponse.ElectricMotor.AngularVelocity, dt, new GearshiftPosition(1), dryRun);
					var emDragTorque = ModelData.ElectricMachinesData.First(x => x.Item1 == emPos).Item2
												.DragCurveLookup(firstResponse.ElectricMotor.AngularVelocity, nextGear);
					if (emDriveTorque != null &&
						emDriveTorque.IsBetween(
							firstResponse.ElectricMotor.MaxRecuperationTorque, firstResponse.ElectricMotor.MaxDriveTorque) &&
						!emDriveTorque.IsEqual(emDragTorque, 1.SI<NewtonMeter>())) {
						var tmp = TryConfiguration(
							absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emDriveTorque), emDriveTorque / emTqReq,
							allowIceOff, dryRun);
						responses.Add(tmp);
					}
				}

				if (ElectricMotorCanPropellDuringTractionInterruption && (allowIceOff || !DataBus.GearboxInfo.GearEngaged(absTime)) /*&& (DataBus.DriverInfo.DrivingAction != DrivingAction.Brake || !DataBus.EngineInfo.EngineOn)*/) {
					// this means that the EM is between wheels and transmission
					// search EM Torque that results in 0 torque at ICE out
					try {
						var emTorqueICEOff = SearchAlgorithm.Search(
							firstResponse.ElectricMotor.ElectricMotorPowerMech / firstResponse.ElectricMotor.AngularVelocity,
							firstResponse.Engine.TorqueOutDemand, firstResponse.ElectricMotor.MaxDriveTorque * 0.1,
							getYValue: r => {
								var response = r as IResponse;
								return response.Engine.TorqueOutDemand;
							},
							evaluateFunction: emTq => {
								var cfg = new HybridStrategyResponse {
									CombustionEngineOn = nextGear.IsLockedGear() ? true : false,
									GearboxInNeutral = false,
									MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
										{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTq) }
									}
								};
								return RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, cfg);
							},
							criterion: r => {
								var response = r as IResponse;
								return response.Engine.TorqueOutDemand.Value();
							},
							abortCriterion: (r, c) => r == null,
							searcher: this
						);
						if (emTorqueICEOff.IsBetween(
							firstResponse.ElectricMotor.MaxDriveTorque, VectoMath.Min(0.SI<NewtonMeter>(), firstResponse.ElectricMotor.MaxRecuperationTorque))) {
							// only consider when within allowed EM torque range
							var tmp = TryConfiguration(
								absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorqueICEOff), emTorqueICEOff / emTqReq,
								allowIceOff, dryRun);
							responses.Add(tmp);
						}
					} catch (Exception) {
						Log.Debug("Failed to find EM torque to compensate drag losses of next components.");
					}
				}

				if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate) {
					// search EM torque for ICE to be on FLD
					try {
						var emTorqueICEMax = SearchAlgorithm.Search(
							firstResponse.ElectricMotor.ElectricMotorPowerMech / firstResponse.ElectricMotor.AngularVelocity,
							firstResponse.Engine.TorqueOutDemand, firstResponse.ElectricMotor.MaxDriveTorque * 0.1,
							getYValue: r => {
								var response = r as IResponse;
								return response.Engine.TotalTorqueDemand - response.Engine.DynamicFullLoadTorque;
							},
							evaluateFunction: emTq => {
								var cfg = new HybridStrategyResponse {
									CombustionEngineOn = nextGear.IsLockedGear() ? true : false,
									GearboxInNeutral = false,
									MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
										{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTq) }
									}
								};
								return RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, cfg);
							},
							criterion: r => {
								var response = r as IResponse;
								return (response.Engine.TotalTorqueDemand - response.Engine.DynamicFullLoadTorque).Value();
							},
							abortCriterion: (r, c) => r == null,
							searcher: this
						);
						if (emTorqueICEMax.IsBetween(maxEmTorque, 0.SI<NewtonMeter>())) {
							// only consider where EM is recuperating
							var tmp = TryConfiguration(
								absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorqueICEMax),
								emTorqueICEMax / emTqReq,
								allowIceOff, dryRun);
							responses.Add(tmp);
						}
					} catch (Exception) {
						Log.Debug("Failed to find EM torque to compensate drag losses of next components.");
					}
				}
			}

			// iterate over 'EM recuperates' up to max available recuperation potential
			if (firstResponse.ElectricMotor.MaxRecuperationTorque != null && (ElectricMotorCanPropellDuringTractionInterruption || firstResponse.Gearbox.Gear.Engaged)) {
				for (var u = stepSize; u <= 1.0; u += stepSize) {
					var emTorque = firstResponse.ElectricMotor.MaxRecuperationTorque * u;
					if (!emTorque.IsBetween(
						firstResponse.ElectricMotor.MaxRecuperationTorque, firstResponse.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>())) {
						continue;
					}

					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorque), u, allowIceOff, dryRun);
					responses.Add(tmp);
				}
				var maxEmTorqueRecuperate = firstResponse.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>();

				if (maxEmTorqueRecuperate.IsGreater(0) && allowIceOff && DataBus.DriverInfo.DrivingAction != DrivingAction.Brake) {
					if (ElectricMotorCanPropellDuringTractionInterruption) {
						// this means that the EM is between wheels and transmission
						// search EM Torque that results in 0 torque at ICE out
						try {
							var emTorqueICEOff = SearchAlgorithm.Search(
								firstResponse.ElectricMotor.ElectricMotorPowerMech / firstResponse.ElectricMotor.AngularVelocity,
								firstResponse.Engine.TorqueOutDemand, firstResponse.ElectricMotor.MaxRecuperationTorque * 0.1,
								getYValue: r => {
									var response = r as IResponse;
									return response.Engine.TorqueOutDemand;
								},
								evaluateFunction: emTq => {
									var cfg = new HybridStrategyResponse {
										CombustionEngineOn = nextGear.IsLockedGear() ? true : false,
										GearboxInNeutral = false,
										MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
											{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTq) }
										}
									};
									return RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, cfg);
								},
								criterion: r => {
									var response = r as IResponse;
									return response.Engine.TorqueOutDemand.Value();
								},
								abortCriterion: (r, c) => r == null,
								searcher: this
							);
							if (emTorqueICEOff.IsBetween(maxEmTorqueRecuperate, 0.SI<NewtonMeter>())) {
								// only consider where EM is recuperating
								var tmp = TryConfiguration(
									absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorqueICEOff),
									emTorqueICEOff / maxEmTorqueRecuperate,
									allowIceOff, dryRun);
								responses.Add(tmp);
							}
						} catch (Exception) {
							Log.Debug("Failed to find EM torque to compensate drag losses of next components.");
						}
					} else {
						if (maxEmTorqueRecuperate.IsGreater(0) && (-emTqReq).IsBetween(maxEmTorqueRecuperate, 0.SI<NewtonMeter>())) {
							var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos,
								Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, -emTqReq), -emTqReq / maxEmTorqueRecuperate, allowIceOff, dryRun);
							responses.Add(tmp);
						}
					}
				}

				if (maxEmTorqueRecuperate.IsGreater(0) && DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate) {
					// search EM torque for ICE to be on FLD
					try {
						var emTorqueICEMax = SearchAlgorithm.Search(
							firstResponse.ElectricMotor.ElectricMotorPowerMech / firstResponse.ElectricMotor.AngularVelocity,
							firstResponse.Engine.TorqueOutDemand, firstResponse.ElectricMotor.MaxRecuperationTorque * 0.1,
							getYValue: r => {
								var response = r as IResponse;
								return response.Engine.TotalTorqueDemand - response.Engine.DynamicFullLoadTorque;
							},
							evaluateFunction: emTq => {
								var cfg = new HybridStrategyResponse {
									CombustionEngineOn = nextGear.IsLockedGear() ? true : false,
									GearboxInNeutral = false,
									MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
										{ emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTq) }
									}
								};
								return RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, cfg);
							},
							criterion: r => {
								var response = r as IResponse;
								return (response.Engine.TotalTorqueDemand - response.Engine.DynamicFullLoadTorque).Value();
							},
							abortCriterion: (r, c) => r == null,
							searcher: this
						);
						if (emTorqueICEMax.IsBetween(0.SI<NewtonMeter>(), maxEmTorqueRecuperate)) {
							// only consider where EM is recuperating
							var tmp = TryConfiguration(
								absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, Tuple.Create(firstResponse.ElectricMotor.AngularVelocity, emTorqueICEMax),
								emTorqueICEMax / maxEmTorqueRecuperate,
								allowIceOff, dryRun);
							responses.Add(tmp);
						}
					} catch (Exception) {
						Log.Debug("Failed to find EM torque to compensate drag losses of next components.");
					}
                }
			}
		}

		private HybridResultEntry TryConfiguration(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition nextGear, PowertrainPosition emPos, Tuple<PerSecond, NewtonMeter> emTorque, double u,
			bool allowIceOff, bool dryRun)
		{
			var cfg = new HybridStrategyResponse {
				CombustionEngineOn = true,
				GearboxInNeutral = false,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> {
					{ emPos,  emTorque }
				}
			};
			var resp = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, cfg);

			var tmp = new HybridResultEntry {
				U = u,
				Setting = cfg,
				Response = resp,
				Gear = nextGear,
			};

			CalculateCosts(resp, dt, tmp, allowIceOff, dryRun);
			return tmp;
		}

		private void CalculateCosts(IResponse resp, Second dt, HybridResultEntry tmp, bool allowIceOff, bool dryRun)
		{
			tmp.IgnoreReason = 0;
			if (resp == null) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.NoResponseAvailable;
				return;
			}

			if (HandleTotalTorqueDemandNull(resp, tmp)) {
				return;
			}

			var iceOff = allowIceOff && resp.Engine.TorqueOutDemand.IsEqual(0, 1e-3);
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && dryRun) {
				// this means we search for an acceleration the vehicle is capable to do - so treat ICE overload regularly
				iceOff = false;
			}

			if (!(!PreviousState.GearboxEngaged && resp.Gearbox.Gear.Engaged)) {
				if (VelocityDropData.Valid && resp.Gearbox.Gear.Engaged &&
					!resp.Gearbox.Gear.Equals(CurrentGear)) {
					var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed,
						DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
					if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU
						.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
						tmp.IgnoreReason |= HybridConfigurationIgnoreReason.VehicleSpeedBelowMinSpeedAfterGearshift;
					}
				}
			}

			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
				StrategyParameters.MaxPropulsionTorque?.GetVECTOValueOrDefault(resp.Gearbox.Gear) != null) {
				var tqRequest = resp.Gearbox.InputTorque;
				var maxTorque =
					StrategyParameters.MaxPropulsionTorque[resp.Gearbox.Gear].FullLoadDriveTorque(resp.Gearbox.InputSpeed);
				if (((tqRequest - maxTorque) * resp.Gearbox.InputSpeed).IsGreater(0, Constants.SimulationSettings.LineSearchTolerance)) {
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.MaxPropulsionTorqueExceeded;
				}
			}

			var avgEngineSpeed = resp.Engine.DynamicFullLoadTorque.IsEqual(0)
				? resp.Engine.EngineSpeed // dynamic full load may be 0 if engine speed is too high, use this as estimate for now
				: resp.Engine.DynamicFullLoadPower / resp.Engine.DynamicFullLoadTorque;
			if (!iceOff && /*!resp.Engine.TotalTorqueDemand.IsBetween(resp.Engine.DragTorque, resp.Engine.DynamicFullLoadTorque)*/
				((resp.Engine.TotalTorqueDemand * avgEngineSpeed).IsSmaller(resp.Engine.DragPower, Constants.SimulationSettings.LineSearchTolerance) || 
				(resp.Engine.TotalTorqueDemand * avgEngineSpeed).IsGreater(resp.Engine.DynamicFullLoadPower, Constants.SimulationSettings.LineSearchTolerance))) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= resp.Engine.TotalTorqueDemand.IsGreater(resp.Engine.DynamicFullLoadTorque)
					? HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh
					: HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow;
			}
			
			if (resp.Gearbox.Gear.Engaged && resp.Engine.EngineSpeed.IsGreaterOrEqual(
					VectoMath.Min(
						ModelData.GearboxData.Gears[resp.Gearbox.Gear.Gear].MaxSpeed,
						DataBus.EngineInfo.EngineN95hSpeed))) {
				tmp.FuelCosts = iceOff ? 0 : double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooHigh;
			}

			if (!ModelData.GearboxData.Type.AutomaticTransmission()) {
				if ((resp.Engine.EngineSpeed != null) && resp.Engine.EngineSpeed.IsSmallerOrEqual(ModelData.EngineData.IdleSpeed)) {
					tmp.FuelCosts = iceOff ? 0 : double.NaN;
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooLow;
				}
			} else {
				if ((resp.Engine.EngineSpeed != null) && resp.Engine.EngineSpeed.IsSmaller(ModelData.EngineData.IdleSpeed)) {
					tmp.FuelCosts = iceOff ? 0 : double.NaN;
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooLow;
				}
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
			if (!tmp.IgnoreReason.EngineTorqueDemandTooHigh()) {
				CheckGearshiftLimits(tmp, resp);
			}

			if (resp.Source is TorqueConverter) {
				if (resp is ResponseUnderload) {
					tmp.FuelCosts = double.NaN;
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow;
				}

				if (resp is ResponseOverload) {
					tmp.FuelCosts = double.NaN;
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh;
				}
			}

			SetBatteryCosts(resp, dt, tmp);
			var absTime = DataBus.AbsTime; // todo!
			if (resp.Gearbox.Gear.Engaged) {

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
			tmp.GearshiftPenalty = resp.Gearbox.Gear.Engaged && !resp.Gearbox.Gear.Equals(CurrentGear)
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

		protected virtual bool HandleTotalTorqueDemandNull(IResponse resp, HybridResultEntry tmp)
		{
			return false;
        }

		protected abstract void CheckGearshiftLimits(HybridResultEntry tmp, IResponse resp);

		protected void SetBatteryCosts(IResponse resp, Second dt, HybridResultEntry tmp)
		{
			var batEnergyStored = DataBus.BatteryInfo.StoredEnergy;
			var batEnergy = resp.ElectricSystem.RESSPowerDemand * dt;
			var batPower = resp.ElectricSystem.RESSResponse.PowerDemand;

			if (batPower.IsSmaller(resp.ElectricSystem.RESSResponse.MaxDischargePower) || batPower.IsGreater(resp.ElectricSystem.RESSResponse.MaxChargePower)) {
				// battery power demand too high - would discharge below min SoC / charge above max SoC
				tmp.BatCosts = double.NaN;
				tmp.IgnoreReason |= batPower.IsSmaller(
						resp.ElectricSystem.RESSResponse.MaxDischargePower)
						? HybridConfigurationIgnoreReason.BatteryBelowMinSoC
						: HybridConfigurationIgnoreReason.BatteryAboveMaxSoc;
			}
			if ((batEnergyStored + batEnergy).IsSmaller(BatteryDischargeEnergyThreshold)) {
				// battery level would go below buffer for auxiliary power - do not alow at 
				tmp.BatCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.BatterySoCTooLow;
			}
			if (batEnergyStored.IsSmaller(BatteryDischargeEnergyThreshold)) {
				var missingBatCharge = BatteryDischargeEnergyThreshold - batEnergyStored;
				var minChargePower = missingBatCharge / StrategyParameters.AuxReserveChargeTime;
				if (batPower.IsSmaller(minChargePower)) {
					tmp.BatCosts = double.NaN;
					tmp.IgnoreReason |= HybridConfigurationIgnoreReason.BatterySoCTooLow;
				} else {
					tmp.BatCosts = 0;
					tmp.IgnoreReason &= ~HybridConfigurationIgnoreReason.BatterySoCTooLow;
				}
			}
			if (!double.IsNaN(tmp.BatCosts)) {
				tmp.BatCosts = -batEnergy.Value();
			}
		}


		public virtual IHybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var retVal = new HybridStrategyResponse { MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() };

			foreach (var em in ModelData.ElectricMachinesData) {
				retVal.MechanicalAssistPower[em.Item1] = null;
			}

			// TODO: MQ 20210712 how to handle with batterysystem
			var auxEnergyReserve = ModelData?.ElectricAuxDemand * StrategyParameters.AuxReserveTime;
			if (auxEnergyReserve > 0) {
				var minSoc = Math.Max(DataBus.BatteryInfo.MinSoC, StrategyParameters.MinSoC);
				BatteryDischargeEnergyThreshold =
					DataBus.BatteryInfo.Capacity * minSoc * DataBus.BatteryInfo.NominalVoltage +
					auxEnergyReserve;
			}

			PreviousState.AngularVelocity = outAngularVelocity;
			PreviousState.GearboxEngaged = true;
			PreviousState.GearshiftTriggerTstmp = -double.MaxValue.SI<Second>();
			CurrentState.GearshiftTriggerTstmp = -double.MaxValue.SI<Second>();
			return retVal;
		}

		public virtual void CommitSimulationStep(Second time, Second simulationInterval)
		{
			var iceStart = PreviousState.ICEStartTStmp;
			PreviousState = CurrentState;
			if (!DataBus.EngineCtl.CombustionEngineOn) {
				PreviousState.ICEStartTStmp = iceStart;
			} else {
				// ice is on, set start-timestamp if it was off
				if (!PreviousState.ICEOn) {
					PreviousState.ICEStartTStmp = time;
				}
			}

			CurrentState = new StrategyState {
				ICEStartTStmp = PreviousState.ICEStartTStmp,
				GearshiftTriggerTstmp = PreviousState.GearshiftTriggerTstmp,
				ICEOn = DataBus.EngineCtl.CombustionEngineOn
			};
			AllowEmergencyShift = false;
			DebugData = new DebugData();
			DryRunSolution = null;
			LimitedGbxTorque = false;
		}

		public void WriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.HybridStrategyScore] = (CurrentState.Solution?.Score ?? 0) / 1e3;
			container[ModalResultField.HybridStrategySolution] = CurrentState.Solution?.U ?? -100;
			container[ModalResultField.MaxPropulsionTorqe] = CurrentState.MaxGbxTq ?? 0.SI<NewtonMeter>();

			//if (CurrentState.Evaluations != null) {
			//    container.SetDataValue(
			//        "HybridStrategyEvaluation",
			//        string.Join(
			//            " | ", CurrentState.Evaluations.Select(
			//                x => {
			//                    var foo = string.Join(" ", x.Setting.MechanicalAssistPower.Select(e => $"{e.Key.GetName()} - {e.Value}"));
			//                    var ice = "====";
			//                    if (x.Response != null) {
			//                        ice =
			//                            $"{x.Response.Engine.TorqueOutDemand}, {x.Response.Engine.TotalTorqueDemand}, {x.Response.Engine.DynamicFullLoadTorque}";
			//                    }
			//                    return
			//                        $"{x.U:F2}: {x.Score:F2}; G{x.Gear}; ({x.FuelCosts:F2} + {x.EquivalenceFactor:F2} * ({x.BatCosts:F2} + {x.ICEStartPenalty1:F2}) * {x.SoCPenalty:F2} + {x.ICEStartPenalty2:F2} + {x.RampUpPenalty:F2}) / {x.GearshiftPenalty:F2} = {x.Score:F2} ({foo} ICE: {ice}); {x.IgnoreReason.HumanReadable()}";
			//                })
			//            )
			//        );
			//}
		}


	}

}