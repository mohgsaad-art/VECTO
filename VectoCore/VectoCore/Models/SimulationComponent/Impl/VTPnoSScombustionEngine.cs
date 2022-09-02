using System;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class VTPnoSScombustionEngine : CombustionEngine
    {
        private bool firstInit = true;

        public VTPnoSScombustionEngine(IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(container, modelData, pt1Disabled) { }

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
			// When speed is below idle, clip to idle to avoid crashing.
			// Returning zero causes crashes (divide by zero, etc).
			return (DataBus.DrivingCycleInfo.CycleData.LeftSample.EngineSpeed < EngineIdleSpeed) 
				? EngineIdleSpeed 
				: DataBus.DrivingCycleInfo.CycleData.LeftSample.EngineSpeed;
        }

        protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
        {
			base.DoWriteModalResults(time, simulationInterval, container);

			var avgEngineSpeed = GetEngineSpeed(CurrentState.EngineSpeed);

			foreach (var fuel in ModelData.Fuels) {
				var result = fuel.ConsumptionMap.GetFuelConsumption(
					CurrentState.EngineTorque, avgEngineSpeed,
					DataBus.ExecutionMode != ExecutionMode.Declaration);

				var fuelData = fuel.FuelData;
				
				bool aboveIdleSpeed = (DataBus.DrivingCycleInfo.CycleData.LeftSample.EngineSpeed >= EngineIdleSpeed);
				var fc = aboveIdleSpeed ? result.Value : 0.SI<KilogramPerSecond>();
				var fcNCVcorr = fc * fuelData.HeatingValueCorrection;

				var fcWHTC = fcNCVcorr * WHTCCorrectionFactor(fuel.FuelData);
				
				var advancedAux = EngineAux as BusAuxiliariesAdapter;
				if (advancedAux != null) {
					advancedAux.DoWriteModalResultsICE(time, simulationInterval, container);
					
				}
				var fcFinal = fcWHTC;

				container[ModalResultField.FCMap, fuelData] = fc;
				container[ModalResultField.FCNCVc, fuel.FuelData] = fcNCVcorr;
				container[ModalResultField.FCWHTCc, fuel.FuelData] = fcWHTC;
				container[ModalResultField.FCFinal, fuel.FuelData] = fcFinal;
			}
        }

        protected override double WHTCCorrectionFactor(IFuelProperties fuel)
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
