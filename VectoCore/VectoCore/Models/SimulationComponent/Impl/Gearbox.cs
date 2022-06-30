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

using System.Linq;
using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Gearbox : AbstractGearbox<GearboxState>, IHybridControlledGearbox, IUpdateable
	{
		/// <summary>
		/// The shift strategy.
		/// </summary>
		internal readonly IShiftStrategy _strategy;

		/// <summary>
		/// Time when a gearbox shift engages a new gear (shift is finished). Is set when shifting is needed.
		/// </summary>
		protected internal Second EngageTime { get; set; } //= 0.SI<Second>();

		/// <summary>
		/// True if gearbox is disengaged (no gear is set).
		/// </summary>
		protected internal bool Disengaged { get; set; }

		protected internal GearshiftPosition _nextGear;
		private Second _overrideDisengage;
		private bool postponeEngage;

		private bool ICEAvailable;

		public override Second LastUpshift { get; protected internal set; }

		public override Second LastDownshift { get; protected internal set; }

		public override GearshiftPosition NextGear => _strategy?.NextGear ?? _nextGear;

		public override bool GearEngaged(Second absTime)
		{
			return !DisengageGearbox && (_overrideDisengage == null || !_overrideDisengage.IsEqual(absTime)) &&
					EngageTime.IsSmallerOrEqual(absTime, ModelData.TractionInterruption / 20) && !postponeEngage;
		}

		// controlled by driver (PCC / EcoRoll)
		public override bool DisengageGearbox { get; set; }

		public override void TriggerGearshift(Second absTime, Second dt)
		{
			EngageTime = absTime + ModelData.TractionInterruption;
			Disengaged = true;
			_strategy.Disengage(absTime, dt, null, null);
			Log.Info("Gearshift triggered - Gearbox disengaged");
		}

		public Gearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container)
		{
			EngageTime = 0.SI<Second>();
			_strategy = strategy;
			if (_strategy != null) {
				_strategy.Gearbox = this;
			}

			ICEAvailable = container.PowertrainInfo.HasCombustionEngine;
			LastDownshift = -double.MaxValue.SI<Second>();
			LastUpshift = -double.MaxValue.SI<Second>();
			Disengaged = true;
		}

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			EngageTime = -double.MaxValue.SI<Second>();

			if (_strategy != null && (Disengaged || DisengageGearbox)) {
				Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio;
			var gearboxTorqueLoss = ModelData.Gears[Gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			CurrentState.TorqueLossResult = gearboxTorqueLoss;

			var inTorque = outTorque / ModelData.Gears[Gear.Gear].Ratio
							+ gearboxTorqueLoss.Value;

			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			PreviousState.Gear = Gear;
			Disengaged = false;

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);

			return response;
		}

		public override bool TCLocked => true;

		protected internal virtual ResponseDryRun Initialize(Second absTime, GearshiftPosition gear, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var oldGear = Gear;
			Gear = gear;
			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear.Gear].Ratio;
			var torqueLossResult = ModelData.Gears[gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			
			CurrentState.TorqueLossResult = torqueLossResult;
			var inTorque = outTorque / ModelData.Gears[gear.Gear].Ratio + torqueLossResult.Value;

			if (DataBus.PowertrainInfo.ElectricMotorPositions.Any(x => x.IsOneOf(PowertrainPosition.HybridP2, PowertrainPosition.HybridP2_5))) {
				// if there is an electric motor after the transmission, initialize the EM first
				NextComponent.Initialize(inTorque, inAngularVelocity);
			}

			if (!inAngularVelocity.IsEqual(0)) {
				var alpha = ModelData.Inertia.IsEqual(0)
					? 0.SI<PerSquareSecond>()
					: outTorque / ModelData.Inertia;

				var inertiaPowerLoss = Formulas.InertiaPower(inAngularVelocity, alpha, ModelData.Inertia,
					Constants.SimulationSettings.TargetTimeInterval);
				inTorque += inertiaPowerLoss / inAngularVelocity;
			}

			var response = NextComponent.Request(absTime, Constants.SimulationSettings.TargetTimeInterval,
				inTorque, inAngularVelocity, true);
			//NextComponent.Initialize(inTorque, inAngularVelocity);
			//response.Switch().
			//	Case<ResponseSuccess>().
			//	Case<ResponseOverload>().
			//	Case<ResponseUnderload>().
			//	Default(r => { throw new UnexpectedResponseException("Gearbox.Initialize", r); });

			var fullLoad = DataBus.EngineInfo.EngineStationaryFullPower(inAngularVelocity);

			Gear = oldGear;
			return new ResponseDryRun(this) {
				Engine = {
					PowerRequest = response.Engine.PowerRequest,
					EngineSpeed = response.Engine.EngineSpeed,
					DynamicFullLoadPower = response.Engine.DynamicFullLoadPower,
					TorqueOutDemand = response.Engine.TorqueOutDemand,
					DynamicFullLoadTorque = response.Engine.DynamicFullLoadTorque
				},
				Clutch = {
					PowerRequest = response.Clutch.PowerRequest,
				},
				Gearbox = {
					PowerRequest = outTorque * outAngularVelocity,
					InputSpeed = inAngularVelocity,
					InputTorque = inTorque,
					OutputTorque = outTorque,
					OutputSpeed = outAngularVelocity,
				},
				DeltaFullLoad = response.Engine.PowerRequest - fullLoad
			};
		}

		/// <summary>
		/// Requests the Gearbox to deliver torque and angularVelocity
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>ResponseDryRun</description></item>
		/// <item><description>ResponseOverload</description></item>
		/// <item><description>ResponseGearshift</description></item>
		/// </list>
		/// </returns>
		public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun = false)
		{
			IterationStatistics.Increment(this, "Requests");

			_strategy?.Request(absTime, dt, outTorque, outAngularVelocity);

			Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);
			if (DataBus.VehicleInfo.VehicleStopped) {
				EngageTime = absTime;
				LastDownshift = -double.MaxValue.SI<Second>();
				LastUpshift = -double.MaxValue.SI<Second>();
			}

			if (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted) {
				EngageTime = absTime + dt;
			}

			postponeEngage = false;
			var reEngaging = false;
			if (GearEngaged(absTime) && Disengaged && !outAngularVelocity.IsEqual(0)) {
				if (dt.IsSmaller(Constants.SimulationSettings.TargetTimeInterval / 10)) {
					Log.Debug("postponing re-engage due to small simulation interval {0}", dt);
					postponeEngage = true;
				} else {
					ReEngageGear(absTime, dt, outTorque, outAngularVelocity);
					reEngaging = true;
					Log.Debug("Gearbox engaged gear {0}", Gear);
				}
			}

			if (_overrideDisengage != null && (!_strategy?.CheckGearshiftRequired ?? false) && !dryRun) {
				var changeGear = _strategy?.ShiftRequired(absTime, dt, outTorque, outAngularVelocity,
					outTorque / ModelData.Gears[Gear.Gear].Ratio,
					outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio, Gear,
					EngageTime, null) ?? false;
				if (changeGear) {
					ReEngageGear(absTime, dt, outTorque, outAngularVelocity);
					reEngaging = true;
				}
			}

			if (reEngaging && DataBus.HybridControllerInfo != null &&
				!Equals(Gear, DataBus.HybridControllerInfo.SelectedGear)) {
				return new ResponseDifferentGearEngaged(this);
			}

			var gear = Disengaged ? NextGear.Gear : Gear.Gear;
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var inTorqueLossResult = ModelData.Gears[gear].LossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			if (avgOutAngularVelocity.IsEqual(0, 1e-9)) {
				inTorqueLossResult.Value = 0.SI<NewtonMeter>();
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
			var avgInAngularVelocity = (PreviousState.InAngularVelocity + inAngularVelocity) / 2.0;
			var inTorque = !avgInAngularVelocity.IsEqual(0)
				? outTorque * (avgOutAngularVelocity / avgInAngularVelocity)
				: outTorque / ModelData.Gears[Gear.Gear].Ratio;
			inTorque += inTorqueLossResult.Value;
			//var inTorque = outTorque / ModelData.Gears[gear].Ratio + inTorqueLossResult.Value;

			var inertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
				? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
				avgOutAngularVelocity
				: 0.SI<NewtonMeter>();
			inTorque += inertiaTorqueLossOut / ModelData.Gears[gear].Ratio;

			var halted = DataBus.DriverInfo.DrivingAction == DrivingAction.Halt;
			var driverDeceleratingNegTorque = DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking &&
											DataBus.DriverInfo.DrivingAction == DrivingAction.Brake &&
											(DataBus.DrivingCycleInfo.RoadGradient.IsSmaller(0) ||
											(ICEAvailable && inAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineIdleSpeed))) &&
											(DataBus.Brakes.BrakePower.IsGreater(0) || inTorque.IsSmaller(0));
			var vehicleSpeedBelowThreshold =
				DataBus.VehicleInfo.VehicleSpeed.IsSmaller(ModelData.DisengageWhenHaltingSpeed);
			if (!dryRun) {
				CurrentState.DrivingBehavior = DataBus.DriverInfo.DriverBehavior;
			}
			
			if (halted || (driverDeceleratingNegTorque && vehicleSpeedBelowThreshold)) {
				EngageTime = VectoMath.Max(EngageTime, absTime + dt);
				_strategy?.Disengage(absTime, dt, outTorque, outAngularVelocity);
				//if (_strategy != null && DataBus.HybridControllerInfo != null &&
				//	DataBus.HybridControllerInfo.SelectedGear.Gear > 0 &&
				//	NextGear.Gear != DataBus.HybridControllerInfo.SelectedGear.Gear) {
				//	return new ResponseDifferentGearEngaged(this);
				//}

				return RequestGearDisengaged(absTime, dt, outTorque, outAngularVelocity, inTorque, dryRun);
			}

			if (GearEngaged(absTime)) {
				return RequestGearEngaged(absTime, dt, outTorque, outAngularVelocity, inTorque, inTorqueLossResult, inertiaTorqueLossOut, dryRun);
			} else {
				return RequestGearDisengaged(absTime, dt, outTorque, outAngularVelocity, inTorque, dryRun);
			}
		}

		/// <summary>
		/// Requests the Gearbox in Disengaged mode
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		/// <item><term>ResponseDryRun</term><description>if dryRun, immediate return!</description></item>
		/// <item><term>ResponseFailTimeInterval</term><description>if shiftTime would be exceeded by current step</description></item>
		/// <item><term>ResponseOverload</term><description>if torque &gt; 0</description></item>
		/// <item><term>ResponseUnderload</term><description>if torque &lt; 0</description></item>
		/// <item><term>else</term><description>Response from NextComponent</description></item>
		/// </list>
		/// </returns>
		private IResponse RequestGearDisengaged(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, bool dryRun)
		{
			Disengaged = true;
			Log.Debug("Current Gear: Neutral");
			var avgAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var gear = NextGear;
			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear.Gear].Ratio;
			var avgInAngularVelocity = (PreviousState.InAngularVelocity + inAngularVelocity) / 2.0;

			if (dryRun) {
				// if gearbox is disengaged the 0[W]-line is the limit for drag and full load.
				var delta = inTorque * avgInAngularVelocity;
				var remainingPowerTrain = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
				return new ResponseDryRun(this, remainingPowerTrain) {
					Gearbox = {
						PowerRequest =  delta,
						Gear = new GearshiftPosition(0),
						InputSpeed = inAngularVelocity,
						InputTorque = inTorque,
						OutputTorque = outTorque,
						OutputSpeed = outAngularVelocity,
					},
					DeltaDragLoad = delta,
					DeltaFullLoad = delta,
					DeltaDragLoadTorque = inTorque,
					DeltaFullLoadTorque = inTorque,
				};
			}

			var shiftTimeExceeded = absTime.IsSmaller(EngageTime) &&
									EngageTime.IsSmaller(absTime + dt, Constants.SimulationSettings.LowerBoundTimeInterval);
			// allow 5% tolerance of shift time
			if (shiftTimeExceeded && EngageTime - absTime > Constants.SimulationSettings.LowerBoundTimeInterval / 2) {
				return new ResponseFailTimeInterval(this) {
					DeltaT = EngageTime - absTime,
					Gearbox = {
						PowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0,
						Gear = new GearshiftPosition(0)
					}
				};
			}

			var remainingTime = EngageTime - (absTime + dt);
			var withinTractionInterruption = absTime.IsSmaller(EngageTime) && (absTime + dt).IsSmaller(EngageTime);
			if (withinTractionInterruption &&
				remainingTime.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval) &&
				remainingTime.IsSmaller(ModelData.TractionInterruption * 0.1)) {
				// interval has already been prolonged, but has been overruled. if remaining time is less than 10%, reduce traction interruption time 
				EngageTime = absTime + dt;
			}

			//var inTorque = 0.SI<NewtonMeter>();
			if (avgInAngularVelocity.Equals(0.SI<PerSecond>())) {
				inTorque = 0.SI<NewtonMeter>();
			}

			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.Gear = gear;
			CurrentState.TransmissionTorqueLoss = inTorque * ModelData.Gears[gear.Gear].Ratio - outTorque;

			var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, false);

			InvokeGearShiftTriggered();

			response.Gearbox.PowerRequest = outTorque * avgAngularVelocity;
			response.Gearbox.Gear = new GearshiftPosition(0);
			response.Gearbox.InputSpeed = inAngularVelocity;
			response.Gearbox.InputTorque = inTorque;
			response.Gearbox.OutputTorque = outTorque;
			response.Gearbox.OutputSpeed = outAngularVelocity;
			return response;
		}

		/// <summary>
		/// Requests the gearbox in engaged mode. Sets the gear if no gear was set previously.
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		/// <item><term>ResponseGearShift</term><description>if a shift is needed.</description></item>
		/// <item><term>else</term><description>Response from NextComponent.</description></item>
		/// </list>
		/// </returns>
		private IResponse RequestGearEngaged(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, TransmissionLossMap.LossMapResult inTorqueLossResult,
			NewtonMeter inertiaTorqueLossOut, bool dryRun)
		{
			// Set a Gear if no gear was set and engineSpeed is not zero
			//if (!Disengaged && DataBus.VehicleStopped && !outAngularVelocity.IsEqual(0))
			//{
			//	Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
			//}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio;

			if (dryRun) {
				var dryRunResponse = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
				dryRunResponse.Gearbox.PowerRequest =
					outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
				dryRunResponse.Gearbox.Gear = Gear;
				dryRunResponse.Gearbox.InputSpeed = inAngularVelocity;
				dryRunResponse.Gearbox.InputTorque = inTorque;
				dryRunResponse.Gearbox.OutputTorque = outTorque;
				dryRunResponse.Gearbox.OutputSpeed = outAngularVelocity;
				return dryRunResponse;
			} else {
				var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, false);
				response.Gearbox.InputSpeed = inAngularVelocity;
				response.Gearbox.InputTorque = inTorque;
				response.Gearbox.OutputTorque = outTorque;
				response.Gearbox.OutputSpeed = outAngularVelocity;

				var shiftAllowed = !inAngularVelocity.IsEqual(0) && !DataBus.VehicleInfo.VehicleSpeed.IsEqual(0);

				if (response is ResponseSuccess && shiftAllowed) {
					var shiftRequired = _strategy?.ShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque,
						response.Engine.EngineSpeed, Gear, EngageTime, response) ?? false;

					if (shiftRequired) {
						if (_overrideDisengage != null) {
							EngageTime = absTime;
							return RequestGearEngaged(absTime, dt, outTorque, outAngularVelocity, inTorque,
								inTorqueLossResult, inertiaTorqueLossOut, false);
						}

						EngageTime = absTime + ModelData.TractionInterruption;

						Log.Debug(
							"Gearbox is shifting. absTime: {0}, dt: {1}, interuptionTime: {2}, out: ({3}, {4}), in: ({5}, {6})",
							absTime,
							dt, EngageTime, outTorque, outAngularVelocity, inTorque, inAngularVelocity);

						Disengaged = true;
						_strategy.Disengage(absTime, dt, outTorque, outAngularVelocity);
						Log.Info("Gearbox disengaged");

						return new ResponseGearShift(this, response) {
							SimulationInterval = ModelData.TractionInterruption,
							Gearbox = {
							PowerRequest =
								outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0,
							Gear = Gear
						},

						};
					}
				}

				// this code has to be _after_ the check for a potential gear-shift!
				// (the above block issues dry-run requests and thus may update the CurrentState!)
				// begin critical section
				CurrentState.TransmissionTorqueLoss = inTorque * ModelData.Gears[Gear.Gear].Ratio - outTorque;
				// MQ 19.2.2016: check! inertia is related to output side, torque loss accounts to input side
				CurrentState.InertiaTorqueLossOut = inertiaTorqueLossOut;


				CurrentState.TorqueLossResult = inTorqueLossResult;
				CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
				CurrentState.Gear = Gear;
				// end critical section


				response.Gearbox.PowerRequest =
					outTorque * (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
				response.Gearbox.Gear = Gear;

				return response;
			}
		}

		private void ReEngageGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			Disengaged = false;
			var lastGear = Gear;
			if (DataBus.VehicleInfo.VehicleStopped) {
				Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
			} else {
				Gear = _strategy.Engage(absTime, dt, outTorque, VectoMath.Min(PreviousState.OutAngularVelocity, outAngularVelocity));
			}

			if (!DataBus.VehicleInfo.VehicleStopped) {
				if (Gear > lastGear) {
					LastUpshift = absTime;
				}

				if (Gear < lastGear) {
					LastDownshift = absTime;
				}
			}
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval,
			IModalDataContainer container)
		{
			var avgInAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			var avgOutAngularSpeed = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			var inPower = CurrentState.InTorque * avgInAngularSpeed;
			var outPower = CurrentState.OutTorque * avgOutAngularSpeed;
			container[ModalResultField.Gear] = Disengaged || DataBus.VehicleInfo.VehicleStopped ? 0 : Gear.Gear;
			container[ModalResultField.P_gbx_loss] = inPower - outPower;
			container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgOutAngularSpeed;
			container[ModalResultField.P_gbx_in] = inPower;
			container[ModalResultField.n_gbx_out_avg] = (PreviousState.OutAngularVelocity +
														CurrentState.OutAngularVelocity) / 2.0;
			container[ModalResultField.T_gbx_out] = CurrentState.OutTorque;
			container[ModalResultField.T_gbx_in] = CurrentState.InTorque;
			_strategy.WriteModalResults(container);
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			postponeEngage = false;
			if (!Disengaged) {
				if (CurrentState.TorqueLossResult != null && CurrentState.TorqueLossResult.Extrapolated) {
					Log.Warn(
						"Gear {0} LossMap data was extrapolated: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
						Gear, CurrentState.OutAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.OutTorque,
						ModelData.Gears[Gear.Gear].Ratio);
					if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
						throw new VectoException(
							"Gear {0} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
							Gear, CurrentState.InAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.InTorque,
							ModelData.Gears[Gear.Gear].Ratio);
					}
				}
			}

			if (DataBus.VehicleInfo.VehicleStopped) {
				Disengaged = true;
				EngageTime = -double.MaxValue.SI<Second>();
			}

			if (GearEngaged(DataBus.AbsTime)) {
				_overrideDisengage = null;
			}

			if (PreviousState.DrivingBehavior != CurrentState.DrivingBehavior) {
				Log.Debug("driving action changed...");
				if (CurrentState.DrivingBehavior == DrivingBehavior.Driving ||
					CurrentState.DrivingBehavior == DrivingBehavior.Braking) {
					if (GearEngaged(time)) {
						Log.Debug("resetting gearshift time interval");
						LastDownshift = -double.MaxValue.SI<Second>();
						LastUpshift = -double.MaxValue.SI<Second>();
					} else {
						CurrentState.DrivingBehavior = PreviousState.DrivingBehavior;
					}
				}
			}

			base.DoCommitSimulationStep(time, simulationInterval);
		}

		public bool SwitchToNeutral
		{
			set => _overrideDisengage = value ? DataBus.AbsTime : null;
			//Disengaged = value;
		}

		public override Second LastShift => EngageTime;

		#region Implementation of IUpdateable

		public bool UpdateFrom(object other) {
			if (other is Gearbox g) {
				PreviousState = g.PreviousState.Clone();
				return true;
			}
			return false;
		}

		#endregion
	}
}