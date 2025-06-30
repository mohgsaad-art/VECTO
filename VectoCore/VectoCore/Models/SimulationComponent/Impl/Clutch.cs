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
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class Clutch : StatefulProviderComponent<Clutch.ClutchState, ITnOutPort, ITnInPort, ITnOutPort>, IClutch, 
		ITnOutPort, ITnInPort, IUpdateable
	{
		protected readonly PerSecond _idleSpeed;
		protected readonly PerSecond _ratedSpeed;
		protected readonly Dictionary<uint, EngineFullLoadCurve> _engineFullLoadCurves;

		protected readonly MeterPerSquareSecond _startAcceleration;
        protected readonly Watt _auxPower;

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
			_engineFullLoadCurves = engineData.FullLoadCurves;
			_idleSpeed = engineData.IdleSpeed;
			_ratedSpeed = _engineFullLoadCurves[0].RatedSpeed;
			_clutchSpeedSlippingFactor = Constants.SimulationSettings.ClutchClosingSpeedNorm * (_ratedSpeed - _idleSpeed) /
										(_idleSpeed + Constants.SimulationSettings.ClutchClosingSpeedNorm * (_ratedSpeed - _idleSpeed));
			_startAcceleration = container.RunData.GearshiftParameters?.StartAcceleration ?? 1.0.SI<MeterPerSquareSecond>();
			_auxPower = 0.0.SI<Watt>();
			foreach (var item in container.RunData.Aux)
			{
				_auxPower = _auxPower + (item.PowerDemandMech == null ? 0.0.SI<Watt>() : item.PowerDemandMech);
			}
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

			var response = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn, dryRun);
			if (!dryRun) {
				CurrentState.SetState(torqueIn, angularVelocityIn, outTorque, outAngularVelocity);
				CurrentState.ClutchLoss = torqueIn * avgInAngularVelocity - outTorque * avgOutAngularVelocity;
				CurrentState.ICEOn = iceOn;
				CurrentState.ICEOnSpeed = DataBus.EngineInfo.EngineSpeed;
			}

			response.Clutch.PowerRequest = outTorque * ((PreviousState.OutAngularVelocity ?? 0.SI<PerSecond>()) + outAngularVelocity) / 2.0;
			response.Clutch.OutputSpeed = outAngularVelocity;
			return response;
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

			/* clutch slipping part */
			var engMaxTorque = _engineFullLoadCurves[0].MaxTorque;
			var acc = _startAcceleration;
			var engTorque = 0.SI<NewtonMeter>();
			var auxPower = _auxPower;

            /* gear used for clutch slipping: saturation to avoid case where Gear==0 (where GetGearData crash) and Gear>2 (allowslipping does not allow slipping at speed higher than 2) */

            if ((DataBus.DrivingCycleInfo.RoadGradient != null) && (DataBus.WheelsInfo != null))
			{
				var slipGear = Math.Min(Math.Max(DataBus.GearboxInfo?.Gear?.Gear ?? 1, 1), 2);
				var ratioGB = DataBus.GearboxInfo?.GetGearData(slipGear)?.Ratio ?? 1.0;
				var ratioAxl = DataBus.AxlegearInfo?.Ratio ?? 1.0;
				var axlegearlossP = DataBus.AxlegearInfo?.AxlegearLoss() ?? 0.0.SI<Watt>();
				var gearboxlossP = DataBus.GearboxInfo?.GearboxLoss() ?? 0.0.SI<Watt>();

				var fWheel = DataBus.VehicleInfo.RollingResistance(DataBus.DrivingCycleInfo.RoadGradient)
                    + DataBus.VehicleInfo.SlopeResistance(DataBus.DrivingCycleInfo.RoadGradient)
                    + DataBus.VehicleInfo.TotalMass*acc;
				engTorque = (fWheel*DataBus.WheelsInfo.DynamicTyreRadius) / ratioGB / ratioAxl;
				if (angularVelocity.IsGreater(0))
				{
					engTorque += (axlegearlossP + gearboxlossP + auxPower) / angularVelocity; // adding transmission losses
				}

				/* additional 1% safety for overloaded vehicles (no effect on Tractors even if concerned, because engine is big enough, 
				 * but effective on small primary buses with oversized default body.
				 * It allows to solve some issues. This line needs to be removed if primary load mass becomes saturated to TPMLM.
				*/
				if (DataBus.VehicleInfo.VehicleLoading.IsGreater(0.9*DataBus.VehicleInfo.VehicleMass))
				{
					engTorque /= 0.99; 
				}
			}

			var angularVelocityForTorque = GetSpeedForTorque(VectoMath.Min(engMaxTorque*0.95, VectoMath.Max(engTorque, 0.0.SI<NewtonMeter>())));
			var slipVelocity = VectoMath.Max(_idleSpeed + _clutchSpeedSlippingFactor * _idleSpeed, angularVelocityForTorque) ;
			if (allowSlipping && angularVelocity < slipVelocity)
			{
				angularVelocityIn = slipVelocity;
			}
			
		}

		private PerSecond GetSpeedForTorque(NewtonMeter torque)
		{
			NewtonMeter previousFullLoadTorque = 0.SI<NewtonMeter>();
			PerSecond previousSpeed = 0.SI<PerSecond>();
			PerSecond engineSpeed = 0.SI<PerSecond>();
			NewtonMeter maxTorque = 0.SI<NewtonMeter>();
			PerSecond engineSpeedAtMaxTorque = 0.SI<PerSecond>();

			if (torque.IsEqual(0.0))
			{
				return engineSpeed;
			}

			foreach (var curvePoint in _engineFullLoadCurves[0].FullLoadEntries)
			{
				if (previousFullLoadTorque < torque && curvePoint.TorqueFullLoad >= torque)
				{
					engineSpeed = curvePoint.EngineSpeed + (previousSpeed-curvePoint.EngineSpeed)/(previousFullLoadTorque-curvePoint.TorqueFullLoad)*(torque-curvePoint.TorqueFullLoad);
					break;
				}
				if (curvePoint.TorqueFullLoad > maxTorque)
				{
					maxTorque = curvePoint.TorqueFullLoad;
					engineSpeedAtMaxTorque = curvePoint.EngineSpeed;
				}
				previousFullLoadTorque = curvePoint.TorqueFullLoad;
				previousSpeed = curvePoint.EngineSpeed;
			}

			if ((engineSpeed <= 0.0) || (engineSpeed == null))
			{
				return engineSpeedAtMaxTorque;
			}
			else
			{
				return engineSpeed;
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
			public new ClutchState Clone() => (ClutchState)base.Clone();
		}

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other) {
			if (other is Clutch c) {
				PreviousState = c.PreviousState.Clone();
				
				return true;
			}
			return false;
		}

		#endregion
	}
}