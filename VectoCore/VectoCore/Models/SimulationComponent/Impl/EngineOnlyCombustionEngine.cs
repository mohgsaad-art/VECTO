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
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class EngineOnlyCombustionEngine : CombustionEngine
	{
		public EngineOnlyCombustionEngine(IVehicleContainer container, CombustionEngineData modelData)
			: base(container, modelData) {}

		// the behavior in engine-only mode differs a little bit from normal driving cycle simulation: in engine-only mode
		// certain amount of overload is tolerated.
		protected override IResponse DoHandleRequest(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun)
		{
			CurrentState.dt = dt;
			CurrentState.EngineSpeed = angularVelocity;
			CurrentState.EngineTorqueOut = torque;
			//			var requestedEnginePower = ComputeRequestedEnginePower(absTime, dt, torque, angularVelocity);

			var avgEngineSpeed = (PreviousState.EngineSpeed + CurrentState.EngineSpeed) / 2.0;

			CurrentState.InertiaTorqueLoss =
				Formulas.InertiaPower(angularVelocity, PreviousState.EngineSpeed, ModelData.Inertia, dt) /
				avgEngineSpeed;

			var auxTorqueDemand = EngineAux == null
				? 0.SI<NewtonMeter>()
				: EngineAux.TorqueDemand(absTime, dt, CurrentState.EngineTorqueOut, angularVelocity, dryRun);

			var totalTorqueDemand = CurrentState.EngineTorqueOut + auxTorqueDemand + CurrentState.InertiaTorqueLoss;
			CurrentState.EngineTorque = totalTorqueDemand;

			CurrentState.FullDragTorque = ModelData.FullLoadCurves[0].DragLoadStationaryTorque(avgEngineSpeed);
			var stationaryFullLoadTorque = ModelData.FullLoadCurves[DataBus.GearboxInfo.Gear.Gear].FullLoadStationaryTorque(avgEngineSpeed);
			var dynamicFullLoadPower = ComputeFullLoadPower(avgEngineSpeed, stationaryFullLoadTorque, dt, dryRun);
			CurrentState.StationaryFullLoadTorque = stationaryFullLoadTorque;
			CurrentState.DynamicFullLoadTorque = dynamicFullLoadPower / avgEngineSpeed;

			ValidatePowerDemand(totalTorqueDemand, CurrentState.DynamicFullLoadTorque, CurrentState.FullDragTorque);

			CurrentState.EngineTorque = LimitEnginePower(CurrentState.EngineTorque, avgEngineSpeed, absTime);

			CurrentState.EnginePower = CurrentState.EngineTorque * avgEngineSpeed;
			if (dryRun) {
				return new ResponseDryRun(this) {
					DeltaFullLoad = CurrentState.EnginePower - CurrentState.DynamicFullLoadTorque * avgEngineSpeed,
					DeltaDragLoad = CurrentState.EnginePower - CurrentState.FullDragTorque * avgEngineSpeed
				};
			}

			//UpdateEngineState(CurrentState.EnginePower, avgEngineSpeed);

			return new ResponseSuccess(this);
		}

		protected NewtonMeter LimitEnginePower(NewtonMeter requestedEngineTorque, PerSecond avgEngineSpeed, Second absTime)
		{
			if (requestedEngineTorque > CurrentState.DynamicFullLoadTorque) {
				if (requestedEngineTorque / CurrentState.DynamicFullLoadTorque > MaxTorqueExceededThreshold) {
					Log.Warn("t: {0}  requested power > P_engine_full * 1.05 - corrected. P_request: {1}  P_engine_full: {2}",
						absTime, requestedEngineTorque * avgEngineSpeed, CurrentState.DynamicFullLoadTorque * avgEngineSpeed);
				}
				return CurrentState.DynamicFullLoadTorque;
			}
			if (requestedEngineTorque < CurrentState.FullDragTorque) {
				if (requestedEngineTorque / CurrentState.FullDragTorque > MaxTorqueExceededThreshold &&
					requestedEngineTorque > -99999) {
					Log.Warn("t: {0}  requested power < P_engine_drag * 1.05 - corrected. P_request: {1}  P_engine_drag: {2}",
						absTime, requestedEngineTorque * avgEngineSpeed, CurrentState.FullDragTorque * avgEngineSpeed);
				}
				return CurrentState.FullDragTorque;
			}
			return requestedEngineTorque;
		}

		protected override PerSecond GetEngineSpeedLimit(Second absTime)
		{
			return ModelData.FullLoadCurves[0].N95hSpeed;
		}
	}
}