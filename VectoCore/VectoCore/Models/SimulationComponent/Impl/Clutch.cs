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
	public class
		Clutch : StatefulProviderComponent<Clutch.ClutchState, ITnOutPort, ITnInPort, ITnOutPort>, IClutch,
		ITnOutPort, ITnInPort
	{
		protected readonly PerSecond _idleSpeed;
		protected readonly PerSecond _ratedSpeed;

		private bool firstInitialize = true;

		public IIdleController IdleController
		{
			get => _idleController;
			set {
				_idleController = value;
				_idleController.RequestPort = NextComponent;
			}
		}

		protected readonly SI _clutchSpeedSlippingFactor;
		private IIdleController _idleController;

		public Clutch(IVehicleContainer container, CombustionEngineData engineData) : base(container)
		{
			_idleSpeed = engineData.IdleSpeed;
			_ratedSpeed = engineData.FullLoadCurves[0].RatedSpeed;
			_clutchSpeedSlippingFactor = Constants.SimulationSettings.ClutchClosingSpeedNorm * (_ratedSpeed - _idleSpeed) /
										(_idleSpeed + Constants.SimulationSettings.ClutchClosingSpeedNorm * (_ratedSpeed - _idleSpeed));
		}

		public virtual IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			NewtonMeter torqueIn;
			PerSecond engineSpeedIn;
			if (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted /*DataBus.VehicleStopped*/) {
				engineSpeedIn = _idleSpeed;
				torqueIn = 0.SI<NewtonMeter>();
			} else {
				AddClutchLoss(outTorque, outAngularVelocity, firstInitialize || DataBus.VehicleInfo.VehicleStopped, out torqueIn, out engineSpeedIn);
			}
			PreviousState.SetState(torqueIn, engineSpeedIn, outTorque, outAngularVelocity);
			//if (!firstInitialize) {
			//	PreviousState.
			//}

			var retVal = NextComponent.Initialize(torqueIn, engineSpeedIn);
			retVal.Clutch.PowerRequest = outTorque * outAngularVelocity;
			retVal.Clutch.OutputSpeed = outAngularVelocity;
			return retVal;
		}

		public virtual void Initialize(Watt clutchLoss)
		{
			PreviousState.ClutchLoss = clutchLoss;
		}


		public virtual IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			firstInitialize = false;
			if ((!DataBus.ClutchInfo.ClutchClosed(absTime) || !DataBus.GearboxInfo.GearEngaged(absTime)) && !dryRun) {
				return HandleClutchOpen(absTime, dt, outTorque, outAngularVelocity, false);
			}

			if (IdleController != null && !dryRun) {
				IdleController.Reset();
			}

			Log.Debug("from Wheels: torque: {0}, angularVelocity: {1}, power {2}", outTorque, outAngularVelocity,
				Formulas.TorqueToPower(outTorque, outAngularVelocity));

			return HandleClutchClosed(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		protected IResponse HandleClutchOpen(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			if (dryRun) {
				var delta = outTorque * avgOutAngularVelocity;
				return new ResponseDryRun(this) {
					Gearbox = { PowerRequest = delta },
					DeltaDragLoad = delta,
					DeltaFullLoad = delta,
					DeltaFullLoadTorque = outTorque,
					DeltaDragLoadTorque = outTorque,
					Clutch = {
						PowerRequest = delta,
						OutputSpeed = outAngularVelocity
					}
				};
			} else {

				if ((outTorque * avgOutAngularVelocity).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
					return new ResponseOverload(this) {
						Delta = outTorque * avgOutAngularVelocity,
						Clutch = {
						PowerRequest = outTorque * avgOutAngularVelocity,
						OutputSpeed = outAngularVelocity
					}
					};
				}

				if ((outTorque * avgOutAngularVelocity).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
					return new ResponseUnderload(this) {
						Delta = outTorque * avgOutAngularVelocity,
						Clutch = {
						PowerRequest = outTorque * avgOutAngularVelocity,
						OutputSpeed = outAngularVelocity
					}
					};
				}

				Log.Debug("Invoking IdleController...");
				var retVal = IdleController.Request(absTime, dt, outTorque, null, false);
				retVal.Clutch.PowerRequest = 0.SI<Watt>();
				retVal.Clutch.OutputSpeed = outAngularVelocity;
				CurrentState.SetState(0.SI<NewtonMeter>(), retVal.Engine.EngineSpeed, outTorque, outAngularVelocity);
				CurrentState.ClutchLoss = 0.SI<Watt>();
				return retVal;
			}
		}

		protected virtual IResponse HandleClutchClosed(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			var startClutch = DataBus.VehicleInfo.VehicleStopped || !PreviousState.ClutchLoss.IsEqual(0, 1e-3) || (outAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineSpeed, 1e-3) && !DataBus.EngineInfo.EngineOn); // || (PreviousState.ClutchLoss.IsEqual(0) && outAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineIdleSpeed));
			var slippingClutchWhenDriving = (DataBus.GearboxInfo.Gear.Gear <= 2 && DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Braking);
			var slippingClutchDuringBraking = DataBus.GearboxInfo.Gear.Gear == 1 && DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking && outTorque > 0 && DataBus.Brakes?.BrakePower == 0.SI<Watt>();
			//var slippingClutchWhenDriving = (DataBus.Gear == 1 && outTorque > 0);
			AddClutchLoss(outTorque, outAngularVelocity,
				slippingClutchWhenDriving || slippingClutchDuringBraking || startClutch || outAngularVelocity.IsEqual(0),
				out var torqueIn, out var angularVelocityIn);

			Log.Debug("to Engine:   torque: {0}, angularVelocity: {1}, power {2}", torqueIn, angularVelocityIn,
				Formulas.TorqueToPower(torqueIn, angularVelocityIn));

			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var iceOn = !DataBus.EngineInfo.EngineOn && DataBus.EngineCtl.CombustionEngineOn;
			var prevInAngularVelocity = iceOn ? DataBus.EngineInfo.EngineSpeed : PreviousState.InAngularVelocity;
			var avgInAngularVelocity = (prevInAngularVelocity + angularVelocityIn) / 2.0;
			var clutchLoss = torqueIn * avgInAngularVelocity - outTorque * avgOutAngularVelocity;
			if (!startClutch && !clutchLoss.IsEqual(0) && (DataBus.GearboxInfo.Gear.Gear != 1 || clutchLoss.IsSmaller(0))) {
				// we don't want to have negative clutch losses, so adapt input torque to match the average output power
				torqueIn = outTorque * avgOutAngularVelocity / avgInAngularVelocity;
			}

			var retVal = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn, dryRun);
			if (!dryRun) {
				CurrentState.SetState(torqueIn, angularVelocityIn, outTorque, outAngularVelocity);
				CurrentState.ClutchLoss = torqueIn * avgInAngularVelocity - outTorque * avgOutAngularVelocity;
				CurrentState.ICEOn = iceOn;
				CurrentState.ICEOnSpeed = DataBus.EngineInfo.EngineSpeed;
			}
			retVal.Clutch.PowerRequest = outTorque *
										((PreviousState.OutAngularVelocity ?? 0.SI<PerSecond>()) + CurrentState.OutAngularVelocity) / 2.0;
			retVal.Clutch.OutputSpeed = outAngularVelocity;
			return retVal;
		}

		protected virtual void AddClutchLoss(NewtonMeter torque, PerSecond angularVelocity, bool allowSlipping, out NewtonMeter torqueIn,
			out PerSecond angularVelocityIn)
		{
			if (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted) {
				angularVelocityIn = _idleSpeed;
				torqueIn = 0.SI<NewtonMeter>();
				return;
			}

			torqueIn = torque;
			angularVelocityIn = angularVelocity;

			var engineSpeedNorm = (angularVelocity - _idleSpeed) / (_ratedSpeed - _idleSpeed);
			if (allowSlipping && engineSpeedNorm < Constants.SimulationSettings.ClutchClosingSpeedNorm) {
				var engineSpeed = VectoMath.Max(_idleSpeed, angularVelocity);
				angularVelocityIn = _clutchSpeedSlippingFactor * engineSpeed + _idleSpeed;
			}
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			if (PreviousState.InAngularVelocity == null || CurrentState.InAngularVelocity == null) {
				container[ModalResultField.P_clutch_out] = 0.SI<Watt>();
				container[ModalResultField.P_clutch_loss] = 0.SI<Watt>();
			} else {
				var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
				var prevInAngularVelocity = CurrentState.ICEOn ? CurrentState.ICEOnSpeed : PreviousState.InAngularVelocity;
				var avgInAngularVelocity = (prevInAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
				container[ModalResultField.P_clutch_out] = CurrentState.OutTorque * avgOutAngularVelocity;
				container[ModalResultField.P_clutch_loss] = CurrentState.InTorque * avgInAngularVelocity -
															CurrentState.OutTorque * avgOutAngularVelocity;
			}
		}

		public virtual bool ClutchClosed(Second absTime)
		{
			return DataBus.GearboxInfo.GearEngaged(absTime);
		}

		public Watt ClutchLosses => PreviousState.ClutchLoss;

		public class ClutchState : SimpleComponentState
		{
			public ClutchState()
			{
				ClutchLoss = 0.SI<Watt>();
			}
			public Watt ClutchLoss { get; set; }
			public bool ICEOn { get; set; }
			public PerSecond ICEOnSpeed { get; set; }
		}
	}
}