using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TestpowertrainCombustionEngine : StopStartCombustionEngine, ITestpowertrainCombustionEngine
	{
		public TestpowertrainCombustionEngine(IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(container, modelData, pt1Disabled) { }

		#region Implementation of ITestpowertrainCombustionEngine

		public IAuxPort GetEngineAux => EngineAux;

		#endregion
	}

	public class StopStartCombustionEngine : CombustionEngine
	{
		protected WattSecond EngineStartEnergy;

		public StopStartCombustionEngine(
			IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(
			container, modelData, pt1Disabled)
		{
			CombustionEngineOn = true;

			var engineRampUpEnergy = Formulas.InertiaPower(modelData.IdleSpeed, 0.RPMtoRad(), modelData.Inertia, modelData.EngineStartTime) * modelData.EngineStartTime;
			var engineDragEnergy = VectoMath.Abs(modelData.FullLoadCurves[0].DragLoadStationaryTorque(modelData.IdleSpeed)) *
									modelData.IdleSpeed / 2.0 * modelData.EngineStartTime;

			EngineStartEnergy = (engineRampUpEnergy + engineDragEnergy) / DeclarationData.AlternatorEfficiency / DeclarationData.AlternatorEfficiency;
		}

		public override bool CombustionEngineOn { get; set; }
		
		#region Overrides of CombustionEngine

		public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			var retVal = CombustionEngineOn
				? base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun)
				: HandleEngineOffRequest(absTime, dt, outTorque, outAngularVelocity, dryRun);
			retVal.Engine.EngineOn = CombustionEngineOn;
			return retVal;
		}

		#endregion

		protected virtual IResponse HandleEngineOffRequest(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			if (!outTorque.IsEqual(0, 1e-2)) {
				if (dryRun) {
					return new ResponseDryRun(this) {
						DeltaFullLoad = outTorque * ModelData.IdleSpeed,
						DeltaDragLoad = outTorque * ModelData.IdleSpeed,
						DeltaDragLoadTorque = outTorque,
						DeltaFullLoadTorque = outTorque,
						Engine = {
							TorqueOutDemand = outTorque,
							TotalTorqueDemand = outTorque,
							PowerRequest = outTorque * 0.RPMtoRad(), //outAngularVelocity,
							DynamicFullLoadPower = 0.SI<Watt>(),
							DragPower = 0.SI<Watt>(),
							DragTorque = 0.SI<NewtonMeter>(),
							EngineSpeed =  0.RPMtoRad(), // outAngularVelocity, //
							AuxiliariesPowerDemand = 0.SI<Watt>(),
						},
						DeltaEngineSpeed = 0.RPMtoRad(),
					};
				}


				var retVal = outTorque > 0
					? new ResponseOverload(this) { Delta = outTorque * ModelData.IdleSpeed }
					: (AbstractResponse)new ResponseUnderload(this) { Delta = outTorque * ModelData.IdleSpeed };
				retVal.Engine.TotalTorqueDemand = outTorque;
				retVal.Engine.TorqueOutDemand = outTorque;
				retVal.Engine.PowerRequest = outTorque * 0.RPMtoRad(); // outAngularVelocity;
				retVal.Engine.DynamicFullLoadPower = 0.SI<Watt>();
				retVal.Engine.DragPower = 0.SI<Watt>();
				retVal.Engine.DragTorque = 0.SI<NewtonMeter>();
				retVal.Engine.EngineSpeed = 0.RPMtoRad();
				retVal.Engine.AuxiliariesPowerDemand = 0.SI<Watt>();
				return retVal;

				//throw new VectoSimulationException("Combustion engine cannot supply outtorque when switched off (T_out: {0})", outTorque);
			}
			CurrentState.EngineOn = false;
			CurrentState.EngineSpeed = DataBus.VehicleInfo.VehicleStopped || outAngularVelocity.IsEqual(0) 
				? ModelData.IdleSpeed 
				: VectoMath.Max(outAngularVelocity, ModelData.IdleSpeed);

			CurrentState.EngineTorque = 0.SI<NewtonMeter>();
			CurrentState.EngineTorqueOut = 0.SI<NewtonMeter>();
			CurrentState.EnginePower = 0.SI<Watt>();
			CurrentState.dt = dt;

			//if (!dryRun) {
			//EngineAux.TorqueDemand(absTime, dt, 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>(), ModelData.IdleSpeed);
			//CurrentState.AuxPowerEngineOff = EngineAux.PowerDemandEngineOff(absTime, dt);
			//} else {
			if (dryRun) {
				return new ResponseDryRun(this) {
					DeltaFullLoad = 0.SI<Watt>(),
					DeltaDragLoad = 0.SI<Watt>(),
					DeltaDragLoadTorque = 0.SI<NewtonMeter>(),
					DeltaFullLoadTorque = 0.SI<NewtonMeter>(),
					Engine = {
						TorqueOutDemand = outTorque,
						PowerRequest = outTorque * 0.RPMtoRad(), // outAngularVelocity,
						DynamicFullLoadPower = 0.SI<Watt>(),
						DynamicFullLoadTorque = 0.SI<NewtonMeter>(),
						DragPower = 0.SI<Watt>(),
						EngineSpeed = 0.RPMtoRad(), // outAngularVelocity, // 0.RPMtoRad(),
						AuxiliariesPowerDemand = 0.SI<Watt>(),
						TotalTorqueDemand = 0.SI<NewtonMeter>(),
						DragTorque = 0.SI<NewtonMeter>()
					},
					DeltaEngineSpeed = 0.RPMtoRad(),
				};
			}

			EngineAux?.TorqueDemand(absTime, dt, outTorque, outAngularVelocity);
			return new ResponseSuccess(this) {
				Engine = {
					TorqueOutDemand = outTorque,
					PowerRequest = 0.SI<Watt>(),
					DynamicFullLoadPower = 0.SI<Watt>(),
					TotalTorqueDemand = 0.SI<NewtonMeter>(),
					DynamicFullLoadTorque = 0.SI<NewtonMeter>(),
					DragPower = 0.SI<Watt>(),
					DragTorque = 0.SI<NewtonMeter>(),
					EngineSpeed = 0.RPMtoRad(),
					AuxiliariesPowerDemand = 0.SI<Watt>(),
				},
			};
		}

		#region Overrides of CombustionEngine

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			if (CombustionEngineOn) {
				base.DoWriteModalResults(time, simulationInterval, container);
				var engineStart = !PreviousState.EngineOn && CurrentState.EngineOn;

				//var engineRampUpEnergy = Formulas.InertiaPower(modelData.IdleSpeed, 0.RPMtoRad(), modelData.Inertia, modelData.EngineStartTime) * modelData.EngineStartTime;
				//var engineDragEnergy = VectoMath.Abs(modelData.FullLoadCurves[0].DragLoadStationaryTorque(modelData.IdleSpeed)) *
				//	modelData.IdleSpeed / 2.0 * modelData.EngineStartTime;

				if (engineStart) {
					var engineRampUpEnergy = Formulas.InertiaPower(PreviousState.EngineSpeed, ModelData.IdleSpeed,
						ModelData.Inertia, ModelData.EngineStartTime) * ModelData.EngineStartTime;
					var avgRampUpSpeed = (ModelData.IdleSpeed + PreviousState.EngineSpeed) / 2.0;
					var engineDragEnergy =
						VectoMath.Abs(ModelData.FullLoadCurves[0].DragLoadStationaryTorque(avgRampUpSpeed)) *
						avgRampUpSpeed * 0.5.SI<Second>();

					container[ModalResultField.P_ice_start] =
						(EngineStartEnergy + (engineRampUpEnergy + engineDragEnergy)) /
						CurrentState.dt;
				} else {
					container[ModalResultField.P_ice_start] = 0.SI<Watt>();
				}

				container[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
				container[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();
			} else {
				container[ModalResultField.P_ice_start] = 0.SI<Watt>();
				DoWriteEngineOffResults(time, simulationInterval, container);
			}

		}

		#endregion

		protected virtual void DoWriteEngineOffResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
			container[ModalResultField.P_ice_out] = 0.SI<Watt>();
			container[ModalResultField.P_ice_inertia] = 0.SI<Watt>();

			container[ModalResultField.n_ice_avg] = 0.RPMtoRad();
			container[ModalResultField.T_ice_fcmap] = 0.SI<NewtonMeter>();

			container[ModalResultField.P_ice_full] = 0.SI<Watt>();
			container[ModalResultField.P_ice_full_stat] = 0.SI<Watt>();
			container[ModalResultField.P_ice_drag] = 0.SI<Watt>();
			container[ModalResultField.T_ice_full] = 0.SI<NewtonMeter>();
			container[ModalResultField.T_ice_drag] = 0.SI<NewtonMeter>();

			container[ModalResultField.ICEOn] = CurrentState.EngineOn;

			var auxDemandPwrICEOn = EngineAux.PowerDemandESSEngineOn(time, simulationInterval, ModelData.IdleSpeed);
			var auxDemandPwrICEOff = EngineAux.PowerDemandESSEngineOff(time, simulationInterval);
			var auxDemandTq = auxDemandPwrICEOn / ModelData.IdleSpeed;

			container[ModalResultField.P_aux_ESS_mech_ice_off] = (auxDemandPwrICEOff ?? 0.SI<Watt>());
			container[ModalResultField.P_aux_ESS_mech_ice_on] = (auxDemandPwrICEOn ?? 0.SI<Watt>());

			WriteWHRPowerEngineOff(container, ModelData.IdleSpeed, auxDemandTq);

			foreach (var fuel in ModelData.Fuels) {
				var fc = 0.SI<KilogramPerSecond>();
				var fcNCVcorr = fc * fuel.FuelData.HeatingValueCorrection; // TODO: wird fcNCVcorr

				var fcWHTC = fcNCVcorr * WHTCCorrectionFactor(fuel.FuelData);
				//var fcAAUX = fcWHTC;
				var advancedAux = EngineAux as BusAuxiliariesAdapter;
				if (advancedAux != null) {
					//throw new VectoException("Engine Stop/Start with advanced auxiliaries not supported!");
					advancedAux.DoWriteModalResultsICE(time, simulationInterval, container);
					//fcAAUX = advancedAux.AAuxFuelConsumption;
				}

				var fcESS = fcWHTC;
				var fcFinal = fcESS;

				container[ModalResultField.FCMap, fuel.FuelData] = fc;
				container[ModalResultField.FCNCVc, fuel.FuelData] = fcNCVcorr;
				container[ModalResultField.FCWHTCc, fuel.FuelData] = fcWHTC;
				//container[ModalResultField.FCAAUX, fuel.FuelData] = fcAAUX;
				//container[ModalResultField.FCICEStopStart, fuel.FuelData] = fcESS;
				container[ModalResultField.FCFinal, fuel.FuelData] = fcFinal;
			}
		}

		protected virtual void WriteWHRPowerEngineOff(IModalDataContainer container, PerSecond engineSpeed, NewtonMeter engineTorque)
		{
			container[ModalResultField.P_WHR_el_map] = 0.SI<Watt>();
			container[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();

			WHRCharger?.GeneratedEnergy(0.SI<WattSecond>());

			container[ModalResultField.P_WHR_mech_map] = 0.SI<Watt>();
			container[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();
		}

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other)
		{
			if (other is CombustionEngine e) {
				PreviousState = e.PreviousState;
				return EngineAux.UpdateFrom(e.EngineAux);
			}
			return false;
		}

		#endregion
	}

}