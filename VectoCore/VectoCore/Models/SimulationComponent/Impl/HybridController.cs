using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class HybridController :
		StatefulProviderComponent<HybridController.HybridControllerState, ITnOutPort, ITnInPort, ITnOutPort>,
		IHybridController, ITnInPort, ITnOutPort, IHybridControllerInternal
	{
		protected readonly Dictionary<PowertrainPosition, HybridCtlElectricMotorController> _electricMotorCtl;
		protected readonly IHybridControlShiftStrategy _shiftStrategy;
		protected readonly IHybridControlStrategy _hybridStrategy;

		protected Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> _electricMotorTorque =
			new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>();

		public HybridStrategyResponse CurrentStrategySettings { get; protected set; }

		protected DebugData DebugData = new DebugData();
		private readonly IVehicleContainer _vehicleContainer;


		public HybridController(IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es) : base(container)
		{
			_electricMotorCtl = new Dictionary<PowertrainPosition, HybridCtlElectricMotorController>();
			_vehicleContainer = container;
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

			_electricMotorCtl[pos] = new HybridCtlElectricMotorController(this, motorData);
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
				var engaged = DataBus.GearboxInfo.GearEngaged(absTime);
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

				var gearShiftResponse = ShiftGear(strategySettings, dryRun, absTime, dt, out var retryAfterGearshift);
				if (gearShiftResponse != null) {
					return gearShiftResponse; 
                }

				if (retryAfterGearshift) {
					retryCount++;
				}
				retry = retryAfterGearshift;

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

				if (retVal is ResponseOverload && DataBus.DriverInfo.DrivingAction == DrivingAction.Brake &&
					engaged != DataBus.GearboxInfo.GearEngaged(absTime)) {
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

        protected virtual AbstractResponse ShiftGear(HybridStrategyResponse strategySettings, bool dryRun, Second absTime, Second dt, out bool retry)
        {
			retry = false;
            if (!dryRun && strategySettings.ShiftRequired) {
				var oldGear = DataBus.GearboxInfo.Gear;
                DataBus.GearboxCtl.TriggerGearshift(absTime, dt);

				_shiftStrategy.SetNextGear(strategySettings.NextGear);
				SelectedGear = strategySettings.NextGear;

				if (DataBus.GearboxInfo.GearboxType == GearboxType.IHPC) {
					retry = true;
					return null;
				}

				if (!DataBus.GearboxInfo.GearboxType.AutomaticTransmission()) {
		
					return new ResponseGearShift(this);
				}
				else if (_vehicleContainer.RunData.HybridStrategyParameters.MaxPropulsionTorque
							?.GetVECTOValueOrDefault(oldGear) != null)
				{
					retry = true;
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

		protected override bool DoUpdateFrom(object other) => false;

		protected override void DoWriteModalResults(
			Second time, Second simulationInterval,
			IModalDataContainer container)
		{
			Strategy.WriteModalResults(time, simulationInterval, container);
		}

		public NewtonMeter MechanicalAssistPower(PowertrainPosition pos, Second absTime, Second dt,
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

		public void RepeatDrivingAction(Second absTime)
		{
			Strategy.RepeatDrivingAction(absTime);
		}
	}
}