using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public interface ISerialHybridController : IHybridController
	{
		ITnInProvider GenSet { get; }
	}

    public class SerialHybridController : StatefulProviderComponent<SerialHybridController.HybridControllerState,
			ITnOutPort, ITnInPort, ITnOutPort>, ISerialHybridController, ITnOutPort, ITnInPort
	{
		protected readonly Dictionary<PowertrainPosition, ElectricMotorController> _electricMotorCtl;

		private Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> _electricMotorTorque =
			new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>();

		protected readonly IHybridControlStrategy _hybridStrategy;

		private HybridStrategyResponse CurrentStrategySettings = null;

		protected DebugData DebugData = new DebugData();

		protected ITnOutPort GenSetPort;

		public SerialHybridController(IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es) : base(container)
		{
			GenSet = new TnInPortWrapper(this);
			_electricMotorCtl = new Dictionary<PowertrainPosition, ElectricMotorController>();
			//_shiftStrategy = container.RunData.GearboxData.Type.AutomaticTransmission()
			//	? new HybridController.HybridCtlATShiftStrategy(this, container)
			//	: new HybridController.HybridCtlShiftStrategy(this, container);
			_hybridStrategy = strategy;
			strategy.Controller = this;

			ElectricSystem = es;
		}

		

		public IHybridControlStrategy Strategy => _hybridStrategy;

		public IElectricSystem ElectricSystem { get; }


		public IHybridControlledGearbox Gearbox { protected get; set; }
		public ICombustionEngine Engine { protected get; set; }

		//public 

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			Strategy.WriteModalResults(time, simulationInterval, container);
		}

		#endregion

		#region Implementation of ITnOutPort

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			var retry = false;
			var retryCount = 0;
			IResponse retVal;
			do {
				if (retryCount > 10) {
					throw new VectoException("SerialHybridStrategy: retry count exceeded! {0}", DebugData);
				}

				retry = false;

				var strategyResponse = Strategy.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);

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
				CurrentStrategySettings = strategySettings;
				if (!dryRun) {
					CurrentState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
					CurrentState.StrategyResponse = strategySettings;
				}

				// Todo: re-think for S2 configuration....
				//if (!dryRun && /*!DataBus.EngineInfo.EngineOn &&*/ strategySettings.ShiftRequired) {
				//	DataBus.GearboxCtl.TriggerGearshift(absTime, dt);
				//	_shiftStrategy.SetNextGear(strategySettings.NextGear);
				//	SelectedGear = strategySettings.NextGear;
				//	if (!DataBus.GearboxInfo.GearboxType.AutomaticTransmission()) {
				//		return new ResponseGearShift(this);
				//	}
				//}

				var gensetResponse = GenSetPort.Request(absTime, dt, 0.SI<NewtonMeter>(),
					strategySettings.GenSetSpeed, dryRun);

				if (!(gensetResponse is ResponseSuccess || gensetResponse is ResponseDryRun)) {
					throw new VectoException("Invalid operating point for Genset provided by strategy! {0}",
						gensetResponse);
				}

				var gear = DataBus.GearboxInfo.Gear;
				retVal = NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
				DebugData.Add("SHC.R", new {
					DrivingAction = DataBus.DriverInfo.DrivingAction,
					StrategySettings = strategySettings,
					Response = retVal,
					DryRun = dryRun
				});

				if (DataBus.GearboxInfo.GearboxType.AutomaticTransmission() && !gear.Equals(DataBus.GearboxInfo.Gear)) {
					retryCount++;
					retry = true;
					Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, dryRun,
						retVal);
					continue;
				}

				if (retVal is ResponseDifferentGearEngaged) {
					retryCount++;
					retry = true;
					Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, dryRun,
						retVal);
					continue;
				}
				retVal.HybridController.StrategySettings = strategySettings;
				
			} while (retry);

			var modifiedResponse =
				Strategy.AmendResponse(retVal, absTime, dt, outTorque, outAngularVelocity, dryRun);

			return modifiedResponse;
		}

		private void ApplyStrategySettings(HybridStrategyResponse strategySettings)
		{
			if (Gearbox != null) {
				Gearbox.SwitchToNeutral = strategySettings.GearboxInNeutral;
			}

			Engine.CombustionEngineOn = strategySettings.CombustionEngineOn;
			_electricMotorTorque = strategySettings.MechanicalAssistPower;
			//if (DataBus.VehicleInfo.VehicleStopped && strategySettings.NextGear.Gear != 0) {
				//_shiftStrategy.SetNextGear(strategySettings.NextGear);
			//}
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			base.DoCommitSimulationStep(time, simulationInterval);
			Strategy.CommitSimulationStep(time, simulationInterval);
			DebugData = new DebugData();
		}

		protected override bool DoUpdateFrom(object other) => false;

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			PreviousState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
			var strategyResponse = Strategy.Initialize(outTorque, outAngularVelocity);
			PreviousState.StrategyResponse = strategyResponse as HybridStrategyResponse;
			_electricMotorTorque = PreviousState.StrategyResponse.MechanicalAssistPower;

			DuringInitialize = true;

			var retVal = NextComponent.Initialize(outTorque, outAngularVelocity);
			if (DataBus.GearboxInfo != null) {
				SelectedGear = DataBus.GearboxInfo.Gear;
			}


			GenSetPort.Initialize(0.SI<NewtonMeter>(), DataBus.EngineInfo.EngineIdleSpeed);
			(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineOn = false;

			DuringInitialize = false;

			return retVal;
		}

		protected bool DuringInitialize { get; set; }

		#endregion

		#region Implementation of IHybridControllerInfo

		public GearshiftPosition SelectedGear { get; protected set; }
		public PerSecond ICESpeed { get; }
		public bool GearboxEngaged { get; }
		public Second SimulationInterval => CurrentStrategySettings.SimulationInterval;
		public PerSecond ElectricMotorSpeed(PowertrainPosition pos)
		{
			return CurrentStrategySettings.MechanicalAssistPower[pos].Item1;
		}

		#endregion

		#region Implementation of IHybridControllerCtl

		public void RepeatDrivingAction(Second absTime)
		{
			Strategy.RepeatDrivingAction(absTime);
		}

		#endregion

		#region Implementation of IHybridController

		public IShiftStrategy ShiftStrategy { get; }
		SimpleComponentState IHybridController.PreviousState => PreviousState;
		public ITnInProvider GenSet { get; }

		public IElectricMotorControl ElectricMotorControl(PowertrainPosition pos)
		{
			return _electricMotorCtl[pos];
		}

		public void AddElectricMotor(PowertrainPosition pos, ElectricMotorData motorData)
		{
			if (_electricMotorCtl.ContainsKey(pos)) {
				throw new VectoException("Electric motor already registered as position {0}", pos);
			}

			_electricMotorCtl[pos] = new ElectricMotorController(this, motorData);
		}



		#endregion

		private NewtonMeter MechanicalAssistPower(PowertrainPosition pos, Second absTime, Second dt,
			NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque, bool dryRun)
		{
			if (DuringInitialize && pos == PowertrainPosition.BatteryElectricE2) {
				return (-outTorque).LimitTo(maxDriveTorque, maxRecuperationTorque ?? VectoMath.Max(maxDriveTorque, 0.SI<NewtonMeter>()));
			}

			return _electricMotorTorque[pos]?.Item2;
		}

		///=======================================================================================

		public class HybridControllerState : SimpleComponentState
		{
			public HybridStrategyResponse StrategyResponse { get; set; }
		}

		///=======================================================================================
		private class TnInPortWrapper : ITnInProvider, ITnInPort
		{
			protected SerialHybridController Controller;

			public TnInPortWrapper(SerialHybridController ctl)
			{
				Controller = ctl;
			}

			#region Implementation of ITnInProvider

			public ITnInPort InPort()
			{
				return this;
			}

			#endregion

			#region Implementation of ITnInPort

			public void Connect(ITnOutPort other)
			{
				Controller.GenSetPort = other;
			}

			#endregion
		}

		///=======================================================================================

		public class ElectricMotorController : IElectricMotorControl
		{
			protected SerialHybridController _controller;
			protected ElectricMotorData ElectricMotorData;

			public ElectricMotorController(SerialHybridController hybridController, ElectricMotorData motorData)
			{
				_controller = hybridController;
				ElectricMotorData = motorData;
			}

			public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity,
				NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque, PowertrainPosition position, bool dryRun)
			{
				return _controller.MechanicalAssistPower(position, absTime, dt, outTorque, prevOutAngularVelocity,
					currOutAngularVelocity, maxDriveTorque, maxRecuperationTorque, dryRun);
			}
		}

	}
}