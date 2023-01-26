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
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class VTPCombustionEngine : CombustionEngine
    {
		private bool firstInit = true;

        public VTPCombustionEngine(IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(container, modelData, pt1Disabled) { }

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (outAngularVelocity == null) {
				outAngularVelocity = EngineIdleSpeed;
			}
			var auxDemand = EngineAux == null ? 0.SI<NewtonMeter>() : EngineAux.Initialize(outTorque, outAngularVelocity);
			if (firstInit) {
				PreviousState = new EngineState {
					EngineSpeed = outAngularVelocity,
					dt = 1.SI<Second>(),
					InertiaTorqueLoss = 0.SI<NewtonMeter>(),
					StationaryFullLoadTorque = ModelData.FullLoadCurves[DataBus.GearboxInfo.Gear.Gear].FullLoadStationaryTorque(outAngularVelocity),
					FullDragTorque = ModelData.FullLoadCurves[DataBus.GearboxInfo.Gear.Gear].DragLoadStationaryTorque(outAngularVelocity),
					EngineTorque = outTorque + auxDemand,
					EnginePower = (outTorque + auxDemand) * outAngularVelocity,
				};
				PreviousState.DynamicFullLoadTorque = PreviousState.StationaryFullLoadTorque;
			}

			return new ResponseSuccess(this) {
				Engine = {
					PowerRequest = PreviousState.EnginePower,
					EngineSpeed = outAngularVelocity
				}
			};
		}

		protected override IResponse DoHandleRequest(Second absTime, Second dt, NewtonMeter torqueReq,
			PerSecond angularVelocity, bool dryRun)
		{
			firstInit = false;
			var powerDemand = angularVelocity * torqueReq;

			var avgEngineSpeed = GetEngineSpeed(angularVelocity);
			var torqueOut = powerDemand / avgEngineSpeed;


			var fullDragTorque = ModelData.FullLoadCurves[DataBus.GearboxInfo.Gear.Gear].DragLoadStationaryTorque(avgEngineSpeed);
			var fullLoadTorque = ModelData.FullLoadCurves[DataBus.GearboxInfo.Gear.Gear].FullLoadStationaryTorque(avgEngineSpeed);
			
			var inertiaTorqueLoss =
				Formulas.InertiaPower(angularVelocity, PreviousState.EngineSpeed, ModelData.Inertia, dt) /
				avgEngineSpeed;

			if (EngineAux != null) {
				EngineAux.Initialize(0.SI<NewtonMeter>(), avgEngineSpeed);
			}
			var auxTorqueDemand = EngineAux == null
				? 0.SI<NewtonMeter>()
				: EngineAux.TorqueDemand(absTime, dt, torqueOut, avgEngineSpeed, dryRun);
			// compute the torque the engine has to provide. powertrain + aux + its own inertia
			var totalTorqueDemand = torqueOut + auxTorqueDemand + inertiaTorqueLoss;

			Log.Debug("EngineInertiaTorque: {0}", inertiaTorqueLoss);
			Log.Debug("Drag Curve: torque: {0}, power: {1}", fullDragTorque, fullDragTorque * avgEngineSpeed);

			var deltaFull = totalTorqueDemand - fullLoadTorque;
			var deltaDrag = totalTorqueDemand - fullDragTorque;

			if (dryRun) {
				return new ResponseDryRun(this) {
					DeltaFullLoad = deltaFull * avgEngineSpeed,
					DeltaDragLoad = deltaDrag * avgEngineSpeed,
					DeltaDragLoadTorque = deltaDrag,
					DeltaFullLoadTorque = deltaFull,
					DeltaEngineSpeed = 0.RPMtoRad(),
					Engine = {
						PowerRequest = torqueOut * avgEngineSpeed,
						DynamicFullLoadPower = fullLoadTorque * avgEngineSpeed,
						DragPower = fullDragTorque * avgEngineSpeed,
						EngineSpeed = angularVelocity,
						AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					},
				};
			}
			CurrentState.dt = dt;
			CurrentState.EngineSpeed = angularVelocity;
			CurrentState.EngineTorqueOut = torqueOut;
			CurrentState.FullDragTorque = fullDragTorque;
			CurrentState.DynamicFullLoadTorque = fullLoadTorque;
			CurrentState.StationaryFullLoadTorque = fullLoadTorque;
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
				return new ResponseOverload(this) {
					AbsTime = absTime,
					Delta = deltaFull * avgEngineSpeed,
					Engine = {
						PowerRequest = totalTorqueDemand * avgEngineSpeed,
						DynamicFullLoadPower = fullLoadTorque * avgEngineSpeed,
						DragPower = CurrentState.FullDragTorque * avgEngineSpeed,
						EngineSpeed = angularVelocity,
						AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					},
				};
			}

			if (totalTorqueDemand.IsSmaller(0) &&
				(deltaDrag * avgEngineSpeed).IsSmaller(0, Constants.SimulationSettings.LineSearchTolerance)) {
				Log.Debug("requested engine power is below drag power: delta: {0}", deltaDrag);
				return new ResponseUnderload(this) {
					AbsTime = absTime,
					Delta = deltaDrag * avgEngineSpeed,
					Engine = {
						PowerRequest = totalTorqueDemand * avgEngineSpeed,
						DynamicFullLoadPower = fullLoadTorque * avgEngineSpeed,
						DragPower = CurrentState.FullDragTorque * avgEngineSpeed,
						EngineSpeed = angularVelocity,
						AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
					},
				};
			}

			//UpdateEngineState(CurrentState.EnginePower, avgEngineSpeed);

			return new ResponseSuccess(this) {
				Engine = {
					PowerRequest = totalTorqueDemand * avgEngineSpeed,
					DynamicFullLoadPower = fullLoadTorque * avgEngineSpeed,
					DragPower = CurrentState.FullDragTorque * avgEngineSpeed,
					EngineSpeed = angularVelocity,
					AuxiliariesPowerDemand = auxTorqueDemand * avgEngineSpeed,
				},
			};
		}


		protected override PerSecond GetEngineSpeed(PerSecond angularSpeed)
        {
            return DataBus.DrivingCycleInfo.CycleData.LeftSample.EngineSpeed;
        }


		// TODO: MQ 2019-07-30
		protected override double WHTCCorrectionFactor (IFuelProperties fuel)
		{
			var selected = ModelData.Fuels.First(x => x.FuelData.FuelType == fuel.FuelType);
			
			if (DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed >= Constants.SimulationSettings.HighwaySpeedThreshold) {
				return selected.WHTCMotorway;
			}
			if (DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed >= Constants.SimulationSettings.RuralSpeedThreshold) {
				return selected.WHTCRural;
			}
			return selected.WHTCUrban;
		}
		
	}
}