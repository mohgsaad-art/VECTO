/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Component for a combustion engine.
	/// </summary>
	public class CombustionEngine : StatefulVectoSimulationComponent<CombustionEngine.EngineState>, ICombustionEngine,
		ITnOutPort
	{
		public bool PT1Disabled { get; set; }

		public enum EngineOperationMode
		{
			Idle,
			Drag,
			FullDrag,
			Load,
			FullLoad,
			Stopped,
			Undef
		}

		protected const int EngineIdleSpeedStopThreshold = 100;
		protected const double MaxTorqueExceededThreshold = 1.05;
		protected const double ZeroThreshold = 0.0001;
		protected const double FullLoadMargin = 0.01;

		protected readonly Watt StationaryIdleFullLoadPower;

		internal readonly CombustionEngineData ModelData;

		protected IAuxPort EngineAux;

		public CombustionEngine(IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false)
			: base(container)
		{
			PT1Disabled = pt1Disabled;
			ModelData = modelData;

			PreviousState.OperationMode = EngineOperationMode.Undef;
			PreviousState.EnginePower = 0.SI<Watt>();
			PreviousState.EngineSpeed = ModelData.IdleSpeed;
			PreviousState.dt = 1.SI<Second>();

			StationaryIdleFullLoadPower = ModelData.FullLoadCurves[0].FullLoadStationaryTorque(ModelData.IdleSpeed) *
										ModelData.IdleSpeed;
		}

		#region IEngineCockpit

		public PerSecond EngineSpeed
		{
			get { return PreviousState.EngineSpeed; }
		}

		public NewtonMeter EngineTorque
		{
			get { return PreviousState.EngineTorque; }
		}

		public Watt PreviousStateEnginePower
		{
			get { return PreviousState.EnginePower; }
		}

		public NewtonMeter PreviousStateEngineTorque
		{
			get { return PreviousState.EngineTorque; }
		}
		public NewtonMeter PreviousStateEngineTorqueOut
		{
			get { return PreviousState.EngineTorqueOut; }
		}

		public NewtonMeter PreviousStateFullDragTorque
		{
			get { return PreviousState.FullDragTorque; }
		}

		public NewtonMeter PreviousStateInertiaTorqueLoss
		{
			get { return PreviousState.InertiaTorqueLoss; }
		}

		public EngineOperationMode PreviousStateOperationMode
		{
			get { return PreviousState.OperationMode; }
		}

		public NewtonMeter PreviousStateStationaryFullLoadTorque
		{
			get { return PreviousState.StationaryFullLoadTorque; }
		}

		public Second PreviousStateDt
		{
			get { return PreviousState.dt; }
		}

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			return ModelData.FullLoadCurves[DataBus.Gear].FullLoadStationaryTorque(angularSpeed) * angularSpeed;
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			return ModelData.FullLoadCurves[DataBus.Gear].DragLoadStationaryPower(angularSpeed);
		}

		public PerSecond EngineIdleSpeed
		{
			get { return ModelData.IdleSpeed; }
		}

		public PerSecond EngineRatedSpeed
		{
			get { return ModelData.FullLoadCurves[0].RatedSpeed; }
		}

		public PerSecond EngineN95hSpeed
		{
			get { return ModelData.FullLoadCurves[0].N95hSpeed; }
		}

		public PerSecond EngineN80hSpeed

		{
			get { return ModelData.FullLoadCurves[0].N80hSpeed; }
		}

		public IIdleController IdleController
		{
			get { return EngineIdleController ?? (EngineIdleController = new CombustionEngineIdleController(this, DataBus)); }
		}

		protected CombustionEngineIdleController EngineIdleController { get; set; }

		#endregion

		#region ITnOutProvider

		public ITnOutPort OutPort()
		{
			return this;
		}

		#endregion

		public void Connect(IAuxPort aux)
		{
			EngineAux = aux;
		}

		#region ITnOutPort

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			IterationStatistics.Increment(this, "Requests");

			Log.Debug("Engine Powertrain Power Request: torque: {0}, angularVelocity: {1}, power: {2}", outTorque,
				outAngularVelocity, outTorque * outAngularVelocity);

			return DoHandleRequest(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		protected virtual IResponse DoHandleRequest(Second absTime, Second dt, NewtonMeter torqueOut,
			PerSecond angularVelocity, bool dryRun)
		{
			if (angularVelocity == null) {
				// if the clutch disengages the idle controller should take over!
				throw new VectoSimulationException("angular velocity is null! Clutch open without IdleController?");
			}
			//if (angularVelocity < ModelData.IdleSpeed.Value() - EngineIdleSpeedStopThreshold) {
			//	CurrentState.OperationMode = EngineOperationMode.Stopped;
			//}

			var avgEngineSpeed = GetEngineSpeed(angularVelocity);

			var engineSpeedLimit = GetEngineSpeedLimit(absTime);
			if (!dryRun && avgEngineSpeed.IsGreater(engineSpeedLimit, Constants.SimulationSettings.LineSearchTolerance)) {
				return new ResponseEngineSpeedTooHigh() {
					DeltaEngineSpeed = avgEngineSpeed - engineSpeedLimit,
					Source = this,
					EngineSpeed = angularVelocity
				};
			}

			var fullDragTorque = ModelData.FullLoadCurves[DataBus.Gear].DragLoadStationaryTorque(avgEngineSpeed);
			var dynamicFullLoadPower = ComputeFullLoadPower(avgEngineSpeed, dt, dryRun);

			var dynamicFullLoadTorque = dynamicFullLoadPower / avgEngineSpeed;
			var inertiaTorqueLoss =
				Formulas.InertiaPower(angularVelocity, PreviousState.EngineSpeed, ModelData.Inertia, dt) /
				avgEngineSpeed;

			var auxTorqueDemand = EngineAux == null
				? 0.SI<NewtonMeter>()
				: EngineAux.TorqueDemand(absTime, dt, torqueOut,
					torqueOut + inertiaTorqueLoss, angularVelocity, dryRun);
			// compute the torque the engine has to provide. powertrain + aux + its own inertia
			var totalTorqueDemand = torqueOut + auxTorqueDemand + inertiaTorqueLoss;

			Log.Debug("EngineInertiaTorque: {0}", inertiaTorqueLoss);
			Log.Debug("Drag Curve: torque: {0}, power: {1}", fullDragTorque, fullDragTorque * avgEngineSpeed);

			Log.Debug("Dynamic FullLoad: torque: {0}, power: {1}", dynamicFullLoadTorque, dynamicFullLoadPower);

			//ValidatePowerDemand(totalTorqueDemand, dynamicFullLoadTorque, fullDragTorque); 

			// get max. torque as limited by gearbox. gearbox only limits torqueOut!

			var deltaFull = totalTorqueDemand - dynamicFullLoadTorque;
			//ComputeDelta(torqueOut, totalTorqueDemand, dynamicFullLoadTorque, gearboxFullLoad, true);
			var deltaDrag = totalTorqueDemand - fullDragTorque; //ComputeDelta(torqueOut, totalTorqueDemand, fullDragTorque,
			//gearboxFullLoad != null ? -gearboxFullLoad : null, false);

			if (dryRun) {
				return new ResponseDryRun {
					DeltaFullLoad = deltaFull * avgEngineSpeed,
					DeltaDragLoad = deltaDrag * avgEngineSpeed,
					DeltaEngineSpeed = avgEngineSpeed - engineSpeedLimit,
					EnginePowerRequest = torqueOut * avgEngineSpeed,
					DynamicFullLoadPower = dynamicFullLoadPower,
					DragPower = fullDragTorque * avgEngineSpeed,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					EngineSpeed = angularVelocity,
					Source = this,
				};
			}
			CurrentState.dt = dt;
			CurrentState.EngineSpeed = angularVelocity;
			CurrentState.EngineTorqueOut = torqueOut;
			CurrentState.FullDragTorque = fullDragTorque;
			CurrentState.DynamicFullLoadTorque = dynamicFullLoadTorque;
			CurrentState.InertiaTorqueLoss = inertiaTorqueLoss;

			if ((deltaFull * avgEngineSpeed).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance) &&
				(deltaDrag * avgEngineSpeed).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				//throw new VectoSimulationException(
				Log.Error(
					"Unexpected condition: requested torque_out is above gearbox full-load and engine is below drag load! deltaFull: {0}, deltaDrag: {1}",
					deltaFull, deltaDrag);
			}

			var minTorque = CurrentState.FullDragTorque;
			var maxTorque = CurrentState.DynamicFullLoadTorque;

			try {
				CurrentState.EngineTorque = totalTorqueDemand.LimitTo(minTorque, maxTorque);
			} catch (Exception) {
				var extrapolated = avgEngineSpeed > ModelData.FullLoadCurves[0].FullLoadEntries.Last().EngineSpeed;
				Log.Error("Engine full-load torque is below drag torque. max_torque: {0}, drag_torque: {1}, extrapolated: {2}", maxTorque, minTorque, extrapolated);
				throw;
			}
			CurrentState.EnginePower = CurrentState.EngineTorque * avgEngineSpeed;

			if (totalTorqueDemand.IsGreater(0) &&
				(deltaFull * avgEngineSpeed).IsGreater(0, Constants.SimulationSettings.LineSearchTolerance)) {
				Log.Debug("requested engine power exceeds fullload power: delta: {0}", deltaFull);
				return new ResponseOverload {
					AbsTime = absTime,
					Delta = deltaFull * avgEngineSpeed,
					EnginePowerRequest = totalTorqueDemand * avgEngineSpeed,
					DynamicFullLoadPower = dynamicFullLoadPower,
					DragPower = CurrentState.FullDragTorque * avgEngineSpeed,
					Source = this,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					EngineSpeed = angularVelocity,
				};
			}

			if (totalTorqueDemand.IsSmaller(0) &&
				(deltaDrag * avgEngineSpeed).IsSmaller(0, Constants.SimulationSettings.LineSearchTolerance)) {
				Log.Debug("requested engine power is below drag power: delta: {0}", deltaDrag);
				return new ResponseUnderload {
					AbsTime = absTime,
					Delta = deltaDrag * avgEngineSpeed,
					EnginePowerRequest = totalTorqueDemand * avgEngineSpeed,
					DynamicFullLoadPower = dynamicFullLoadPower,
					DragPower = CurrentState.FullDragTorque * avgEngineSpeed,
					Source = this,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					EngineSpeed = angularVelocity,
				};
			}

			//UpdateEngineState(CurrentState.EnginePower, avgEngineSpeed);

			return new ResponseSuccess {
				EnginePowerRequest = totalTorqueDemand * avgEngineSpeed,
				DynamicFullLoadPower = dynamicFullLoadPower,
				DragPower = CurrentState.FullDragTorque * avgEngineSpeed,
				AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
				EngineSpeed = angularVelocity,
				Source = this
			};
		}

		protected virtual PerSecond GetEngineSpeed(PerSecond angularVelocity)
		{
			return (PreviousState.EngineSpeed + angularVelocity) / 2.0;
		}

		protected virtual PerSecond GetEngineSpeedLimit(Second absTime)
		{
			return DataBus.Gear == 0 || !DataBus.ClutchClosed(absTime)
				? ModelData.FullLoadCurves[0].N95hSpeed
				: VectoMath.Min(DataBus.GetGearData(DataBus.Gear).MaxSpeed, ModelData.FullLoadCurves[0].N95hSpeed);
		}

		public virtual IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (outAngularVelocity == null) {
				outAngularVelocity = EngineIdleSpeed;
			}
			var auxDemand = EngineAux == null ? 0.SI<NewtonMeter>() : EngineAux.Initialize(outTorque, outAngularVelocity);
			PreviousState = new EngineState {
				EngineSpeed = outAngularVelocity,
				dt = 1.SI<Second>(),
				InertiaTorqueLoss = 0.SI<NewtonMeter>(),
				StationaryFullLoadTorque = ModelData.FullLoadCurves[DataBus.Gear].FullLoadStationaryTorque(outAngularVelocity),
				FullDragTorque = ModelData.FullLoadCurves[DataBus.Gear].DragLoadStationaryTorque(outAngularVelocity),
				EngineTorque = outTorque + auxDemand,
				EnginePower = (outTorque + auxDemand) * outAngularVelocity,
			};
			PreviousState.DynamicFullLoadTorque = PreviousState.StationaryFullLoadTorque;

			return new ResponseSuccess {
				Source = this,
				EnginePowerRequest = PreviousState.EnginePower,
				EngineSpeed = outAngularVelocity
			};
		}

		/// <summary>
		/// Validates the requested power demand [W].
		/// </summary>
		protected virtual void ValidatePowerDemand(NewtonMeter torqueDemand, NewtonMeter dynamicFullLoadTorque,
			NewtonMeter fullDragTorque)
		{
			if (fullDragTorque.IsGreater(0) && torqueDemand < 0) {
				throw new VectoSimulationException("P_engine_drag > 0! Tq_drag: {0}, Tq_eng: {1},  n_eng_avg: {2} [1/min] ",
					fullDragTorque, torqueDemand, CurrentState.EngineSpeed.AsRPM);
			}

			if (dynamicFullLoadTorque <= 0 && torqueDemand > 0) {
				throw new VectoSimulationException("P_engine_full < 0! Tq_full: {0}, Tq_eng: {1},  n_eng_avg: {2} [1/min] ",
					dynamicFullLoadTorque, torqueDemand, CurrentState.EngineSpeed.AsRPM);
			}
		}

		#endregion

		#region VectoSimulationComponent

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			ValidatePowerDemand(CurrentState.EngineTorque, CurrentState.DynamicFullLoadTorque, CurrentState.FullDragTorque);

			var avgEngineSpeed = GetEngineSpeed(CurrentState.EngineSpeed);
			if (avgEngineSpeed.IsSmaller(EngineIdleSpeed,
				DataBus.ExecutionMode == ExecutionMode.Engineering ? 20.RPMtoRad() : 1e-3.RPMtoRad())) {
				Log.Warn("EngineSpeed below idling speed! n_eng_avg: {0}, n_idle: {1}", avgEngineSpeed.AsRPM, EngineIdleSpeed.AsRPM);
			}
			container[ModalResultField.P_eng_fcmap] = CurrentState.EngineTorque * avgEngineSpeed;
			container[ModalResultField.P_eng_out] = container[ModalResultField.P_eng_out] is DBNull
				? CurrentState.EngineTorqueOut * avgEngineSpeed
				: container[ModalResultField.P_eng_out];
			container[ModalResultField.P_eng_inertia] = CurrentState.InertiaTorqueLoss * avgEngineSpeed;

			container[ModalResultField.n_eng_avg] = avgEngineSpeed;
			container[ModalResultField.T_eng_fcmap] = CurrentState.EngineTorque;

			container[ModalResultField.P_eng_full] = CurrentState.DynamicFullLoadTorque * avgEngineSpeed;
			container[ModalResultField.P_eng_full_stat] = CurrentState.StationaryFullLoadTorque * avgEngineSpeed;
			container[ModalResultField.P_eng_drag] = CurrentState.FullDragTorque * avgEngineSpeed;
			container[ModalResultField.Tq_full] = CurrentState.DynamicFullLoadTorque;
			container[ModalResultField.Tq_drag] = CurrentState.FullDragTorque;

			var result = ModelData.ConsumptionMap.GetFuelConsumption(CurrentState.EngineTorque, avgEngineSpeed,
				DataBus.ExecutionMode != ExecutionMode.Declaration);
			if (DataBus.ExecutionMode != ExecutionMode.Declaration && result.Extrapolated) {
				Log.Warn("FuelConsumptionMap was extrapolated: range for FC-Map is not sufficient: n: {0}, torque: {1}",
					avgEngineSpeed.Value(), CurrentState.EngineTorque.Value());
			}
			var pt1 = ModelData.FullLoadCurves[DataBus.Gear].PT1(avgEngineSpeed);
			if (DataBus.ExecutionMode == ExecutionMode.Declaration && pt1.Extrapolated) {
				Log.Error("requested rpm below minimum rpm in pt1 - extrapolating. n_eng_avg: {0}",
					avgEngineSpeed);
			}

			var fc = result.Value;
			var fcNCVcorr = fc * ModelData.FuelData.HeatingValueCorrection;

			var fcWHTC = fcNCVcorr * WHTCCorrectionFactor;
			var fcAAUX = fcWHTC;
			var advancedAux = EngineAux as BusAuxiliariesAdapter;
			if (advancedAux != null) {
				advancedAux.DoWriteModalResults(container);
				fcAAUX = advancedAux.AAuxFuelConsumption;
			}
			var fcADAS = fcAAUX * ModelData.ADASCorrectionFactor;
			var fcFinal = fcADAS;

			container[ModalResultField.FCMap] = fc;
			container[ModalResultField.FCNCVc] = fcNCVcorr;
			container[ModalResultField.FCWHTCc] = fcWHTC;
			container[ModalResultField.FCAAUX] = fcAAUX;
			container[ModalResultField.FCADAS] = fcADAS;
			container[ModalResultField.FCFinal] = fcFinal;
		}

		protected virtual double WHTCCorrectionFactor
		{
			get { return ModelData.FuelConsumptionCorrectionFactor; }
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
			var advancedAux = EngineAux as BusAuxiliariesAdapter;
			if (advancedAux != null) {
				advancedAux.DoCommitSimulationStep();
			}
		}

		#endregion

		/// <summary>
		///     computes full load power from gear [-], angularVelocity [rad/s] and dt [s].
		/// </summary>
		protected Watt ComputeFullLoadPower(PerSecond angularVelocity, Second dt, bool dryRun)
		{
			if (dt <= 0) {
				throw new VectoException("ComputeFullLoadPower cannot compute for simulation interval length 0.");
			}

			var tStatFull = ModelData.FullLoadCurves[DataBus.Gear].FullLoadStationaryTorque(angularVelocity);
			var stationaryFullLoadPower = tStatFull * angularVelocity;
			if (!dryRun) {
				CurrentState.StationaryFullLoadTorque = tStatFull;
			}
			Watt dynFullPowerCalculated;

			// disable pt1 behaviour if PT1Disabled is true, or if the previous enginepower is greater than the current stationary fullload power (in this case the pt1 calculation fails)
			if (PT1Disabled || PreviousState.EnginePower.IsGreaterOrEqual(stationaryFullLoadPower)) {
				dynFullPowerCalculated = stationaryFullLoadPower;
			} else {
				try {
					var pt1 = ModelData.FullLoadCurves[DataBus.Gear].PT1(angularVelocity).Value.Value();
					var powerRatio = (PreviousState.EnginePower / stationaryFullLoadPower).Value();
					var tStarPrev = pt1 * Math.Log(1.0 / (1 - powerRatio), Math.E).SI<Second>();
					var tStar = tStarPrev + PreviousState.dt;
					dynFullPowerCalculated = stationaryFullLoadPower * (pt1.IsEqual(0) ? 1 : 1 - Math.Exp((-tStar / pt1).Value()));
				} catch (VectoException e) {
					Log.Warn("PT1 calculation failed (dryRun: {0}): {1}", dryRun, e.Message);
					if (dryRun) {
						dynFullPowerCalculated = stationaryFullLoadPower;
					} else {
						throw;
					}
				}

				dynFullPowerCalculated = VectoMath.Max(PreviousState.EnginePower, dynFullPowerCalculated);
			}

			// new check in vecto 3.x (according to Martin Rexeis)
			if (dynFullPowerCalculated < StationaryIdleFullLoadPower) {
				dynFullPowerCalculated = StationaryIdleFullLoadPower;
			}
			if (dynFullPowerCalculated > stationaryFullLoadPower) {
				dynFullPowerCalculated = stationaryFullLoadPower;
			}

			if (dynFullPowerCalculated < 0) {
				return 0.SI<Watt>();
			}
			return dynFullPowerCalculated;
		}

		protected bool IsFullLoad(Watt requestedPower, Watt maxPower)
		{
			var testValue = requestedPower / maxPower - 1.0;
			return testValue.Abs() < FullLoadMargin;
		}

		public class EngineState
		{
			public EngineOperationMode OperationMode { get; set; }

			// ReSharper disable once InconsistentNaming
			public Second dt { get; set; }

			public PerSecond EngineSpeed { get; set; }

			public NewtonMeter EngineTorque { get; set; }

			public NewtonMeter EngineTorqueOut { get; set; }

			public Watt EnginePower { get; set; }

			public NewtonMeter InertiaTorqueLoss { get; set; }

			public NewtonMeter StationaryFullLoadTorque { get; set; }

			public NewtonMeter DynamicFullLoadTorque { get; set; }

			public NewtonMeter FullDragTorque { get; set; }
		}

		protected internal class CombustionEngineIdleController : LoggingObject, IIdleController
		{
			private const double PeDropSlope = -5;
			private const double PeDropOffset = 1.0;

			private readonly CombustionEngine _engine;
			private readonly IDataBus _dataBus;

			private Second _idleStart;
			private Watt _lastEnginePower;
			private PerSecond _engineTargetSpeed;

			public ITnOutPort RequestPort { private get; set; }

			public CombustionEngineIdleController(CombustionEngine combustionEngine, IDataBus dataBus)
			{
				_engine = combustionEngine;
				_dataBus = dataBus;
			}

			public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				return new ResponseSuccess { Source = this };
			}

			public void Reset()
			{
				_idleStart = null;
			}

			public virtual IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
				bool dryRun = false)
			{
				if (!_dataBus.VehicleStopped && _dataBus.Gear != _dataBus.NextGear.Gear && _dataBus.Gear != 0 &&
					_dataBus.NextGear.Gear != 0) {
					return RequestDoubleClutch(absTime, dt, outTorque, outAngularVelocity);
				}
				return RequestIdling(absTime, dt, outTorque, outAngularVelocity);
			}

			private IResponse RequestDoubleClutch(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				if (outAngularVelocity != null) {
					throw new VectoException("IdleController can only handle idle requests, i.e. angularVelocity == null!");
				}
				if (!outTorque.IsEqual(0)) {
					throw new VectoException("Torque has to be 0 for idle requests!");
				}
				if (_idleStart == null) {
					_idleStart = absTime;
					_engineTargetSpeed = _engine.PreviousState.EngineSpeed / _dataBus.GetGearData(_dataBus.Gear).Ratio *
										_dataBus.GetGearData(_dataBus.NextGear.Gear).Ratio;
				}


				var velocitySlope = (_dataBus.TractionInterruption - (absTime - _idleStart)).IsEqual(0)
					? 0.SI<PerSquareSecond>()
					: (_engineTargetSpeed - _engine.PreviousState.EngineSpeed) /
					(_dataBus.TractionInterruption - (absTime - _idleStart));

				var nextAngularSpeed = velocitySlope * dt + _engine.PreviousState.EngineSpeed;

				var engineMaxSpeed = VectoMath.Min(_dataBus.GetGearData(_dataBus.NextGear.Gear).MaxSpeed,
					_engine.ModelData.FullLoadCurves[0].N95hSpeed);
				nextAngularSpeed = velocitySlope < 0
					? VectoMath.Max(_engineTargetSpeed, nextAngularSpeed).LimitTo(_engine.EngineIdleSpeed, engineMaxSpeed)
					: VectoMath.Min(_engineTargetSpeed, nextAngularSpeed).LimitTo(_engine.EngineIdleSpeed, engineMaxSpeed);


				var retVal = RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), nextAngularSpeed);
				retVal.Switch().
					Case<ResponseSuccess>().
					Case<ResponseUnderload>(r => {
						var angularSpeed = SearchAlgorithm.Search(nextAngularSpeed, r.Delta,
							Constants.SimulationSettings.EngineIdlingSearchInterval,
							getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
							evaluateFunction: n => RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), n, true),
							criterion: result => ((ResponseDryRun)result).DeltaDragLoad.Value());
						Log.Debug("Found operating point for idling. absTime: {0}, dt: {1}, torque: {2}, angularSpeed: {3}", absTime, dt,
							0.SI<NewtonMeter>(), angularSpeed);
						if (angularSpeed < _engine.ModelData.IdleSpeed) {
							angularSpeed = _engine.ModelData.IdleSpeed;
						}

						retVal = RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), angularSpeed);
					}).
					Case<ResponseOverload>(r => {
						var angularSpeed = SearchAlgorithm.Search(nextAngularSpeed, r.Delta,
							Constants.SimulationSettings.EngineIdlingSearchInterval,
							getYValue: result => ((ResponseDryRun)result).DeltaFullLoad,
							evaluateFunction: n => RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), n, true),
							criterion: result => ((ResponseDryRun)result).DeltaFullLoad.Value());
						Log.Debug("Found operating point for idling. absTime: {0}, dt: {1}, torque: {2}, angularSpeed: {3}", absTime, dt,
							0.SI<NewtonMeter>(), angularSpeed);
						angularSpeed = angularSpeed.LimitTo(_engine.ModelData.IdleSpeed, engineMaxSpeed);
						retVal = RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), angularSpeed);
					}).
					Default(r => { throw new UnexpectedResponseException("searching Idling point", r); });

				return retVal;
			}

			protected IResponse RequestIdling(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				if (outAngularVelocity != null) {
					throw new VectoException("IdleController can only handle idle requests, i.e. angularVelocity == null!");
				}
				if (!outTorque.IsEqual(0)) {
					throw new VectoException("Torque has to be 0 for idle requests!");
				}
				if (_idleStart == null) {
					_idleStart = absTime;
					_lastEnginePower = _engine.PreviousState.EnginePower;
					_engineTargetSpeed = _engine.EngineIdleSpeed;
				}
				if (_lastEnginePower == null) {
					_lastEnginePower = _engine.PreviousState.EnginePower;
				}
				IResponse retVal;

				var idleTime = absTime - _idleStart + dt;
				var prevEngineSpeed = _engine.PreviousState.EngineSpeed;
				var dragLoad = _engine.ModelData.FullLoadCurves[0].DragLoadStationaryPower(prevEngineSpeed);

				var nextEnginePower = (_lastEnginePower - dragLoad) * Math.Max(0, idleTime.Value() * PeDropSlope + PeDropOffset) +
									dragLoad;

				var auxDemandResponse = RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), prevEngineSpeed, true);

				var deltaEnginePower = nextEnginePower - (auxDemandResponse.AuxiliariesPowerDemand ?? 0.SI<Watt>());
				var deltaTorque = deltaEnginePower / prevEngineSpeed;
				var deltaAngularSpeed = deltaTorque / _engine.ModelData.Inertia * dt;

				var nextAngularSpeed = prevEngineSpeed;
				if (deltaAngularSpeed > 0) {
					retVal = RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), nextAngularSpeed);
					return retVal;
				}

				nextAngularSpeed = (prevEngineSpeed + deltaAngularSpeed)
					.LimitTo(_engine.ModelData.IdleSpeed, _engine.EngineRatedSpeed);

				retVal = RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), nextAngularSpeed);
				retVal.Switch().
					Case<ResponseSuccess>().
					Case<ResponseUnderload>(r => {
						var angularSpeed = SearchAlgorithm.Search(nextAngularSpeed, r.Delta,
							Constants.SimulationSettings.EngineIdlingSearchInterval,
							getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
							evaluateFunction: n => RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), n, true),
							criterion: result => ((ResponseDryRun)result).DeltaDragLoad.Value());
						Log.Debug("Found operating point for idling. absTime: {0}, dt: {1}, torque: {2}, angularSpeed: {3}", absTime, dt,
							0.SI<NewtonMeter>(), angularSpeed);
						retVal = RequestPort.Request(absTime, dt, 0.SI<NewtonMeter>(), angularSpeed);
					}).
					Default(r => { throw new UnexpectedResponseException("searching Idling point", r); });

				return retVal;
			}
		}

		protected internal class CombustionEngineNoDubleclutchIdleController : CombustionEngineIdleController
		{
			public CombustionEngineNoDubleclutchIdleController(CombustionEngine combustionEngine, IDataBus dataBus) : base(combustionEngine, dataBus)
			{
			}

			public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
				bool dryRun = false)
			{
				
				return RequestIdling(absTime, dt, outTorque, outAngularVelocity);
			}
		}
	}
}