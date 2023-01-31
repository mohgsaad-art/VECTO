using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class HybridController :
		StatefulProviderComponent<HybridController.HybridControllerState, ITnOutPort, ITnInPort, ITnOutPort>,
		IHybridController, ITnInPort, ITnOutPort
	{
		protected readonly Dictionary<PowertrainPosition, ElectricMotorController> _electricMotorCtl;
		protected readonly HybridCtlShiftStrategy _shiftStrategy;
		protected readonly IHybridControlStrategy _hybridStrategy;

		protected Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> _electricMotorTorque =
			new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>();

		private HybridStrategyResponse CurrentStrategySettings;

		protected DebugData DebugData = new DebugData();


		public HybridController(IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es) : base(container)
		{
			_electricMotorCtl = new Dictionary<PowertrainPosition, ElectricMotorController>();

			switch (container.RunData.GearboxData.Type) {
				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					_shiftStrategy = new HybridCtlATShiftStrategy(this, container);
					break;
				case GearboxType.AMT:
					_shiftStrategy = new HybridCtlShiftStrategy(this, container);
					break;
				case GearboxType.APTN:
				case GearboxType.IHPC:
					_shiftStrategy = new HybridCtlIHPCShiftStrategy(this, container);
					break;
				default: throw new ArgumentException($"Unsupported Gearbox type for Hybrid Controller: {container.RunData.GearboxData.Type}");
			}
			
			_hybridStrategy = strategy;
			strategy.Controller = this;

			ElectricSystem = es;
		}

		public IHybridControlStrategy Strategy => _hybridStrategy;

		public IElectricSystem ElectricSystem { get; }

		public virtual void AddElectricMotor(PowertrainPosition pos, ElectricMotorData motorData)
		{
			if (_electricMotorCtl.ContainsKey(pos)) {
				throw new VectoException("Electric motor already registered as position {0}", pos);
			}

			_electricMotorCtl[pos] = new ElectricMotorController(this, motorData);
		}

		protected virtual void ApplyStrategySettings(HybridStrategyResponse strategySettings)
		{
			Gearbox.SwitchToNeutral = strategySettings.GearboxInNeutral;
			Engine.CombustionEngineOn = strategySettings.CombustionEngineOn;
			_electricMotorTorque = strategySettings.MechanicalAssistPower;
			if (DataBus.VehicleInfo.VehicleStopped && strategySettings.NextGear.Gear != 0) {
				_shiftStrategy.SetNextGear(strategySettings.NextGear);
			}
		}

		SimpleComponentState IHybridController.PreviousState => PreviousState;

		public virtual IElectricMotorControl ElectricMotorControl(PowertrainPosition pos) => _electricMotorCtl[pos];

		public virtual IShiftStrategy ShiftStrategy => _shiftStrategy;

		public GearshiftPosition SelectedGear { get; protected set; }

		public bool GearboxEngaged => CurrentStrategySettings.GearboxEngaged;

		public PerSecond ElectricMotorSpeed(PowertrainPosition pos) => CurrentStrategySettings.MechanicalAssistPower[pos].Item1;

		public Second SimulationInterval => CurrentStrategySettings.SimulationInterval;

		public PerSecond ICESpeed => CurrentStrategySettings.EvaluatedSolution.Response?.Engine.EngineSpeed;


		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			var retry = false;
			var retryCount = 0;
			IResponse retVal;
			do {
				if (retryCount > 10) {
					throw new VectoException("HybridStrategy: retry count exceeded! {0}", DebugData);
				}

				retry = false;
				var strategyResponse = Strategy.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
				DebugData.Add($"[HC-R-0-{retryCount}]", strategyResponse);
				if (strategyResponse is HybridStrategyLimitedResponse ovl) {
					if (dryRun) {
						return new ResponseDryRun(this) {
							DeltaDragLoad = ovl.Delta,
							DeltaFullLoad = ovl.Delta,
							// TODO! delta full/drag torque
							DeltaEngineSpeed = ovl.DeltaEngineSpeed,
							Gearbox = {
								InputTorque = ovl.GearboxResponse?.InputTorque,
								InputSpeed = ovl.GearboxResponse?.InputSpeed,
								OutputTorque = ovl.GearboxResponse?.OutputTorque,
								OutputSpeed = ovl.GearboxResponse?.OutputSpeed,
								PowerRequest = ovl.GearboxResponse?.PowerRequest,
								Gear = ovl.GearboxResponse?.Gear
							}

						};
					}

					return new ResponseOverload(this) {
						Delta = ovl.Delta
					};
				}

				var strategySettings = strategyResponse as HybridStrategyResponse;
				ApplyStrategySettings(strategySettings);
				if (!dryRun) {
					CurrentState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
					CurrentState.StrategyResponse = strategySettings;
				}

				var gearShiftResponse = ShiftGear(strategySettings, dryRun, absTime, dt);
				if (gearShiftResponse != null) {
					return gearShiftResponse; 
                }

				if (!dryRun /*&& DataBus.VehicleInfo.VehicleStopped*/) {
					SelectedGear = GetNextGear(strategySettings);
				}

				CurrentStrategySettings = strategySettings;
				retVal = NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
				DebugData.Add($"HC.R-1-{retryCount}", new {
					DataBus.DriverInfo.DrivingAction,
					StrategySettings = strategySettings,
					Response = retVal,
					DryRun = dryRun
				});

				if (!dryRun && strategySettings.CombustionEngineOn && retVal is ResponseSuccess &&
					retVal.Engine.EngineSpeed.IsSmaller(Strategy.MinICESpeed)) {
					Strategy.AllowEmergencyShift = true;
					retryCount++;
					retry = true;
					Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, false, retVal);
					continue;
				}

				if (AdjustStrategyWhenExceedingGearMaxSpeed(dryRun, retVal, absTime, dt, outTorque, outAngularVelocity)) {
					retryCount++;
					retry = true;
					continue;
                }

				if (!dryRun && strategySettings.CombustionEngineOn && retVal is ResponseEngineSpeedTooHigh && !strategySettings.ProhibitGearshift) {
					retryCount++;
					retry = true;
					Strategy.AllowEmergencyShift = true;
					Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, false, retVal);
					continue;
				}

				if (retVal is ResponseDifferentGearEngaged) {
					retryCount++;
					retry = true;
					Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, dryRun,
						retVal);
					continue;
				}

				if (retVal is ResponseInvalidOperatingPoint) {
					retryCount++;
					retry = true;
					Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, dryRun,
						retVal);
					continue;
				}

				retVal.HybridController.StrategySettings = strategySettings;
				if (!(retVal is ResponseSuccess) && strategySettings.EvaluatedSolution.Gear.Engaged &&
					retVal.Gearbox.Gear != strategySettings.EvaluatedSolution.Gear && retryCount < 3) {
					retryCount++;
					retry = true;
				}
			} while (retry);

			var modifiedResponse = Strategy.AmendResponse(retVal, absTime, dt, outTorque, outAngularVelocity, dryRun);

			return modifiedResponse;
        }

        protected virtual AbstractResponse ShiftGear(HybridStrategyResponse strategySettings, bool dryRun, Second absTime, Second dt)
        {
			if (!dryRun && strategySettings.ShiftRequired) {
				DataBus.GearboxCtl.TriggerGearshift(absTime, dt);

				_shiftStrategy.SetNextGear(strategySettings.NextGear);
				SelectedGear = strategySettings.NextGear;

				if (!DataBus.GearboxInfo.GearboxType.AutomaticTransmission()) {
					return new ResponseGearShift(this);
				}
			}

			return null;
        }

        protected virtual GearshiftPosition GetNextGear(HybridStrategyResponse strategySettings)
        {
			return strategySettings.NextGear;
		}

		protected virtual bool AdjustStrategyWhenExceedingGearMaxSpeed(bool dryRun, IResponse retVal, Second absTime, Second dt, 
			NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var gear = DataBus.GearboxInfo.Gear;
			var maxSpeed = VectoMath.Min(DataBus.GearboxInfo.GetGearData(gear.Gear).MaxSpeed, DataBus.EngineInfo.EngineN95hSpeed);

			if (!dryRun 
				&& retVal is ResponseSuccess 
				&& DataBus.GearboxInfo.GearEngaged(absTime) 
				&& (retVal.Gearbox.InputSpeed != null) 
				&& retVal.Gearbox.InputSpeed.IsGreater(maxSpeed)) {
				
				Strategy.AllowEmergencyShift = true;
				Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, false, retVal);
				
				return true;
			}

			return false;
        }

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			PreviousState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
			var strategyResponse = Strategy.Initialize(outTorque, outAngularVelocity);
			PreviousState.StrategyResponse = strategyResponse as HybridStrategyResponse;
			_electricMotorTorque = PreviousState.StrategyResponse.MechanicalAssistPower;
			var retVal = NextComponent.Initialize(outTorque, outAngularVelocity);
			SelectedGear = DataBus.GearboxInfo.Gear;
			return retVal;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			base.DoCommitSimulationStep(time, simulationInterval);
			Strategy.CommitSimulationStep(time, simulationInterval);
			DebugData = new DebugData();
		}

		protected override void DoWriteModalResults(
			Second time, Second simulationInterval,
			IModalDataContainer container)
		{
			Strategy.WriteModalResults(time, simulationInterval, container);
		}

		private NewtonMeter MechanicalAssistPower(PowertrainPosition pos, Second absTime, Second dt,
			NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity, bool dryRun) =>
			_electricMotorTorque[pos]?.Item2;

		public GearshiftPosition NextGear => CurrentState.StrategyResponse.NextGear;

		public bool ShiftRequired => CurrentState.StrategyResponse.ShiftRequired;

		public IHybridControlledGearbox Gearbox { protected get; set; }
		public ICombustionEngine Engine { protected get; set; }

		///=======================================================================================
		public class HybridControllerState : SimpleComponentState
		{
			public HybridStrategyResponse StrategyResponse;
		}

		///=======================================================================================
		public class ElectricMotorController : IElectricMotorControl
		{
			protected HybridController _controller;
			protected ElectricMotorData ElectricMotorData;

			public ElectricMotorController(HybridController hybridController, ElectricMotorData motorData)
			{
				_controller = hybridController;
				ElectricMotorData = motorData;
			}

			public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity,
				NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque, PowertrainPosition position, bool dryRun)
			{
				return _controller.MechanicalAssistPower(position, absTime, dt, outTorque, prevOutAngularVelocity,
					currOutAngularVelocity, dryRun);
			}
		}

		public void RepeatDrivingAction(Second absTime)
		{
			Strategy.RepeatDrivingAction(absTime);
		}

		///=======================================================================================

		public class HybridCtlShiftStrategy : ShiftStrategy
		{
			protected HybridController _controller;


			//protected readonly GearshiftPosition MaxStartGear;
			protected GearshiftPosition _nextGear { get; set; }

			protected readonly GearList GearList;
			protected readonly VectoRunData _runData;

			public HybridCtlShiftStrategy(HybridController hybridController, IVehicleContainer container) : base(
				container)
			{
				_controller = hybridController;

				_runData = container.RunData;
				if (_runData?.EngineData == null) {
					return;
				}

				GearList = GearboxModelData.GearList;
				var transmissionRatio = _runData.AxleGearData.AxleGear.Ratio *
										(_runData.AngledriveData?.Angledrive.Ratio ?? 1.0) /
										_runData.VehicleData.DynamicTyreRadius;
				var minEngineSpeed = (_runData.EngineData.FullLoadCurves[0].RatedSpeed - _runData.EngineData.IdleSpeed) *
					Constants.SimulationSettings.ClutchClosingSpeedNorm + _runData.EngineData.IdleSpeed;
				MaxStartGear = GearList.First();
				foreach (var gear in GearList.Reverse()) {
					var gearData = GearboxModelData.Gears[gear.Gear];
					if (gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value) {
						continue;
					}
					if (GearshiftParams.StartSpeed * transmissionRatio * gearData.Ratio <= minEngineSpeed)
						continue;
					MaxStartGear = gear;
					break;
				}
			}

			public override ShiftPolygon ComputeDeclarationShiftPolygon(GearboxType gearboxType, int i,
				EngineFullLoadCurve engineDataFullLoadCurve,
				IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
				Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
			{
				return DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
					i, engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);
			}

			protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond outAngularVelocity, NewtonMeter inTorque,
				PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
			{
				if (_controller.ShiftRequired) {
					_nextGear = _controller.NextGear;
				}

				return _controller.ShiftRequired;
			}

			#region Overrides of BaseShiftStrategy

			public override void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				base.Request(absTime, dt, outTorque, outAngularVelocity);
				if (DataBus.DriverInfo.DrivingAction == DrivingAction.Halt) {
					_nextGear = MaxStartGear;
				}
			}

			#endregion

			public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond outAngularVelocity)
			{
				if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
					return InitStartGear(absTime, outTorque, outAngularVelocity);
				}

				foreach (var entry in GearList.Reverse()) {
					var gear = entry;
					//for (var gear = (uint)GearboxModelData.Gears.Count; gear > 1; gear--) {
					var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

					var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
					var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
					var inTorque = response.Clutch.PowerRequest / inAngularSpeed;

					// if in shift curve and torque reserve is provided: return the current gear
					if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) &&
						!IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
						reserve >= GearshiftParams.StartTorqueReserve) {
						if ((inAngularSpeed - DataBus.EngineInfo.EngineIdleSpeed) /
							(DataBus.EngineInfo.EngineRatedSpeed - DataBus.EngineInfo.EngineIdleSpeed) <
							Constants.SimulationSettings.ClutchClosingSpeedNorm && GearList.HasPredecessor(gear)) {
							gear = GearList.Predecessor(gear);
						}

						_nextGear = gear;
						return gear;
					}

					// if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
					if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && GearList.HasSuccessor(gear)) {
						_nextGear = gear;
						return GearList.Successor(gear);
					}
				}

				// fallback: return first gear
				_nextGear = GearList.First();
				return _nextGear;
			}

			protected GearshiftPosition InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				if (!DataBus.EngineCtl.CombustionEngineOn) {
					return _nextGear;
				}

				foreach (var gear in GearList.IterateGears(MaxStartGear, GearList.First())) {
					//for (var gear = MaxStartGear; gear > 1; gear--) {
					var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

					var ratedSpeed = DataBus.EngineInfo.EngineRatedSpeed;
					if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
						continue;
					}

					var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

					var fullLoadPower =
						response.Engine.DynamicFullLoadTorque; //EnginePowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.Engine.TorqueOutDemand / fullLoadPower;

					if (_runData != null && _runData.HybridStrategyParameters.MaxPropulsionTorque?.GetVECTOValueOrDefault(gear) != null) {
						var tqRequest = response.Gearbox.InputTorque;
						var maxTorque = _runData.HybridStrategyParameters.MaxPropulsionTorque[gear].FullLoadDriveTorque(response.Gearbox.InputSpeed);
						reserve = 1 - VectoMath.Min(response.Engine.TorqueOutDemand / fullLoadPower, tqRequest / maxTorque);
					}

					if (response.Engine.EngineSpeed > DataBus.EngineInfo.EngineIdleSpeed &&
						reserve.IsGreaterOrEqual(0)) {
						//reserve >= GearshiftParams.StartTorqueReserve) {
						_nextGear = gear;
						return gear;
					}
				}

				_nextGear = GearList.First();
				return _nextGear;
			}


			protected virtual bool SpeedTooLowForEngine(GearshiftPosition gear, PerSecond outAngularSpeed) => 
				(outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsSmaller(DataBus.EngineInfo.EngineIdleSpeed);

			protected virtual bool SpeedTooHighForEngine(GearshiftPosition gear, PerSecond outAngularSpeed) =>
				(outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsGreaterOrEqual(
					VectoMath.Min(GearboxModelData.Gears[gear.Gear].MaxSpeed, DataBus.EngineInfo.EngineN95hSpeed));

			public override GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				var tmpGear = new GearshiftPosition(_nextGear.Gear);
				if (DataBus.EngineCtl.CombustionEngineOn) {
					while (GearList.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear, outAngularVelocity)) {
						_nextGear = GearList.Predecessor(_nextGear);
					}

					//if (!tmpGear.Equals(_nextGear)) {
					//	if (GearList.HasPredecessor(_nextGear) && IsBelowDownShiftCurve(_nextGear,)) {
					//		_nextGear = GearList.Predecessor(_nextGear);
					//	}
					//}

					while (GearList.HasSuccessor(_nextGear) && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
						_nextGear = GearList.Successor(_nextGear);
					}
				}

				return _nextGear;
			}

			public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond outAngularVelocity)
			{
				if (!_controller.ShiftRequired && !_controller.CurrentStrategySettings.GearboxInNeutral && DataBus.DriverInfo.DrivingAction != DrivingAction.Halt) {
					// gearbox disengaged on its own! set next gear!
					var gear = _nextGear;
					while (GearList.HasPredecessor(gear) && SpeedTooLowForEngine(gear, outAngularVelocity)) {
						gear = GearList.Predecessor(gear);
					}

					while (GearList.HasSuccessor(gear) && SpeedTooHighForEngine(gear, outAngularVelocity)) {
						gear = GearList.Successor(gear);
					}

					_nextGear = gear;
				}
			}

			public override IGearbox Gearbox
			{
				get => _gearbox;
				set => _gearbox = value as Gearbox ?? throw new VectoException("This shift strategy can't handle gearbox of type {0}", value.GetType());
			}

			public override GearshiftPosition NextGear => _nextGear;

			public void SetNextGear(GearshiftPosition nextGear) => _nextGear = nextGear;
		}


		///=======================================================================================

		public class HybridCtlATShiftStrategy : HybridCtlShiftStrategy
		{
			protected new ATGearbox _gearbox;

			public HybridCtlATShiftStrategy(HybridController hybridController, IVehicleContainer container) : base(
				hybridController, container)
			{ }

			public override IGearbox Gearbox
			{
				get => _gearbox;
				set => _gearbox = value as ATGearbox ?? throw new VectoException("This shift strategy can't handle gearbox of type {0}", value.GetType());
			}

			public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
			{
				if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
					// AT always starts in first gear and TC active!
					_gearbox.Disengaged = true;
					return Gears.First();
				}

				foreach (var gear in Gears.Reverse()) {
					var response = _gearbox.Initialize(gear, torque, outAngularVelocity);

					if (response.Engine.EngineSpeed > DataBus.EngineInfo.EngineRatedSpeed || response.Engine.EngineSpeed < DataBus.EngineInfo.EngineIdleSpeed) {
						continue;
					}

					if (!IsBelowDownShiftCurve(gear, response.Engine.PowerRequest / response.Engine.EngineSpeed, response.Engine.EngineSpeed)) {
						_gearbox.Disengaged = false;
						return gear;
					}
				}

				// fallback: start with first gear;
				_gearbox.Disengaged = false;
				return Gears.First();
			}

			protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, 
				PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear, 
				Second lastShiftTime, IResponse response) =>
				false;

			public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) => 
				throw new NotImplementedException("AT Shift Strategy does not support disengaging.");

			protected override bool SpeedTooLowForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
			{
				if (gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value) {
					return false;
				}

				return base.SpeedTooLowForEngine(gear, outAngularSpeed);
			}

			protected override bool SpeedTooHighForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
			{
				if (gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value) {
					return false;
				}

				return base.SpeedTooHighForEngine(gear, outAngularSpeed);
			}
		}

		///=======================================================================================

		public class HybridCtlIHPCShiftStrategy : HybridCtlATShiftStrategy
		{
			protected new APTNGearbox _gearbox;

			public HybridCtlIHPCShiftStrategy(HybridController hybridController, IVehicleContainer container) : base(hybridController, container) { }

			public override IGearbox Gearbox {
				get => _gearbox;
				set => _gearbox = value as APTNGearbox ?? throw new VectoException("This shift strategy can't handle gearbox of type {0}", value.GetType());
			}

			public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
			{
				if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
					return InitStartGear(absTime, torque, outAngularVelocity);
				}

				foreach (var gear in Gears.Reverse()) {
					var response = _gearbox.Initialize(absTime, gear, torque, outAngularVelocity);

					if (response.Engine.EngineSpeed > DataBus.EngineInfo.EngineRatedSpeed || response.Engine.EngineSpeed < DataBus.EngineInfo.EngineIdleSpeed) {
						continue;
					}

					if (!IsBelowDownShiftCurve(gear, response.Engine.PowerRequest / response.Engine.EngineSpeed, response.Engine.EngineSpeed)) {
						_gearbox.Disengaged = false;
						return gear;
					}
				}

				// fallback: start with first gear;
				_gearbox.Disengaged = false;
				return Gears.First();
			}

			public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

			protected GearshiftPosition InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				if (!DataBus.EngineCtl.CombustionEngineOn) {
					return _nextGear;
				}

				foreach (var gear in GearList.IterateGears(MaxStartGear, GearList.First())) {
					//for (var gear = MaxStartGear; gear > 1; gear--) {
					var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

					var ratedSpeed = DataBus.EngineInfo.EngineRatedSpeed;
					if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
						continue;
					}

					var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

					var fullLoadPower =
						response.Engine.DynamicFullLoadTorque; //EnginePowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.Engine.TorqueOutDemand / fullLoadPower;

					if (_runData != null && _runData.HybridStrategyParameters.MaxPropulsionTorque?.GetVECTOValueOrDefault(gear) != null) {
						var tqRequest = response.Gearbox.InputTorque;
						var maxTorque = _runData.HybridStrategyParameters.MaxPropulsionTorque[gear].FullLoadDriveTorque(response.Gearbox.InputSpeed);
						reserve = 1 - VectoMath.Min(response.Engine.TorqueOutDemand / fullLoadPower, tqRequest / maxTorque);
					}

					if (response.Engine.EngineSpeed > DataBus.EngineInfo.EngineIdleSpeed &&
						reserve.IsGreaterOrEqual(0)) {
						//reserve >= GearshiftParams.StartTorqueReserve) {
						_nextGear = gear;
						return gear;
					}
				}

				_nextGear = GearList.First();
				return _nextGear;
			}
		}
	}
}