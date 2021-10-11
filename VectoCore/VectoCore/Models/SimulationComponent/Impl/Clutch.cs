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
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Clutch : StatefulProviderComponent<Clutch.ClutchState, ITnOutPort, ITnInPort, ITnOutPort>, IClutch,
		ITnOutPort, ITnInPort
	{
		private readonly PerSecond _idleSpeed;
		private readonly PerSecond _ratedSpeed;

		private bool firstInitialize = true;

		public IIdleController IdleController
		{
			get { return _idleController; }
			set
			{
				_idleController = value;
				_idleController.RequestPort = NextComponent;
			}
		}

		private readonly SI _clutchSpeedSlippingFactor;
		private IIdleController _idleController;

		public Clutch(IVehicleContainer container, CombustionEngineData engineData) : base(container)
		{
			_idleSpeed = engineData.IdleSpeed;
			_ratedSpeed = engineData.FullLoadCurves[0].RatedSpeed;
			_clutchSpeedSlippingFactor = Constants.SimulationSettings.ClutchClosingSpeedNorm * (_ratedSpeed - _idleSpeed) /
										(_idleSpeed + Constants.SimulationSettings.ClutchClosingSpeedNorm * (_ratedSpeed - _idleSpeed));
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			NewtonMeter torqueIn;
			PerSecond engineSpeedIn;
			if (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted /*DataBus.VehicleStopped*/) {
				engineSpeedIn = _idleSpeed;
				torqueIn = 0.SI<NewtonMeter>();
			} else {
				AddClutchLoss(outTorque, outAngularVelocity, firstInitialize || DataBus.VehicleStopped, out torqueIn, out engineSpeedIn);
			}
			PreviousState.SetState(torqueIn, engineSpeedIn, outTorque, outAngularVelocity);
			//if (!firstInitialize) {
			//	PreviousState.
			//}

			var retVal = NextComponent.Initialize(torqueIn, engineSpeedIn);
			retVal.ClutchPowerRequest = outTorque * outAngularVelocity;
			return retVal;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			firstInitialize = false;
			var startClutch = DataBus.VehicleStopped || !PreviousState.ClutchLoss.IsEqual(0);
			if (!DataBus.ClutchClosed(absTime) && !dryRun) {
				Log.Debug("Invoking IdleController...");
				var retval = IdleController.Request(absTime, dt, outTorque, null, dryRun);
				retval.ClutchPowerRequest = 0.SI<Watt>();
				CurrentState.SetState(0.SI<NewtonMeter>(), retval.Engine.EngineSpeed, outTorque, outAngularVelocity);
				CurrentState.ClutchLoss = 0.SI<Watt>();
				return retval;
			}
			if (IdleController != null) {
				IdleController.Reset();
			}

			Log.Debug("from Wheels: torque: {0}, angularVelocity: {1}, power {2}", outTorque, outAngularVelocity,
				Formulas.TorqueToPower(outTorque, outAngularVelocity));

			NewtonMeter torqueIn;
			PerSecond angularVelocityIn;

			var slippingClutchWhenDriving = (DataBus.Gear.Gear <= 2 && DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Braking);
			var slippingClutchDuringBraking = DataBus.Gear.Gear == 1 && DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking && outTorque > 0 && DataBus.BrakePower.IsEqual(0);
			//var slippingClutchWhenDriving = (DataBus.Gear == 1 && outTorque > 0);
			AddClutchLoss(outTorque, outAngularVelocity,
				slippingClutchWhenDriving || slippingClutchDuringBraking || startClutch || outAngularVelocity.IsEqual(0),
				out torqueIn, out angularVelocityIn);

			Log.Debug("to Engine:   torque: {0}, angularVelocity: {1}, power {2}", torqueIn, angularVelocityIn,
				Formulas.TorqueToPower(torqueIn, angularVelocityIn));

			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var avgInAngularVelocity = (PreviousState.InAngularVelocity + angularVelocityIn) / 2.0;
			var clutchLoss = torqueIn * avgInAngularVelocity - outTorque * avgOutAngularVelocity;
			if (!startClutch && !clutchLoss.IsEqual(0) && (DataBus.Gear.Gear != 1 || clutchLoss.IsSmaller(0))) {
				// we don't want to have negative clutch losses, so adapt input torque to match the average output power
				torqueIn = outTorque * avgOutAngularVelocity / avgInAngularVelocity;
			}

			var retVal = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn, dryRun);
			if (!dryRun) {
				CurrentState.SetState(torqueIn, angularVelocityIn, outTorque, outAngularVelocity);
				CurrentState.ClutchLoss = torqueIn * avgInAngularVelocity - outTorque * avgOutAngularVelocity;
			}
			retVal.ClutchPowerRequest = outTorque *
										((PreviousState.OutAngularVelocity ?? 0.SI<PerSecond>()) + CurrentState.OutAngularVelocity) / 2.0;
			return retVal;
		}

		private void AddClutchLoss(NewtonMeter torque, PerSecond angularVelocity, bool allowSlipping, out NewtonMeter torqueIn,
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

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			if (PreviousState.InAngularVelocity == null || CurrentState.InAngularVelocity == null) {
				container[ModalResultField.P_clutch_out] = 0.SI<Watt>();
				container[ModalResultField.P_clutch_loss] = 0.SI<Watt>();
			} else {
				var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
				var avgInAngularVelocity = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
				container[ModalResultField.P_clutch_out] = CurrentState.OutTorque * avgOutAngularVelocity;
				container[ModalResultField.P_clutch_loss] = CurrentState.InTorque * avgInAngularVelocity -
															CurrentState.OutTorque * avgOutAngularVelocity;
			}
		}

		public class ClutchState : SimpleComponentState
		{
			public Watt ClutchLoss = 0.SI<Watt>();
		}
	}
}