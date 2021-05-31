using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class StopStartCombustionEngine : CombustionEngine
	{
		protected double EngineStopStartUtilityFactor;
		private WattSecond EngineStartEnergy;

		public StopStartCombustionEngine(
			IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(
			container, modelData, pt1Disabled)
		{
			CombustionEngineOn = true;
			EngineStopStartUtilityFactor = container.RunData.DriverData.EngineStopStart.UtilityFactor;

			var engineRampUpEnergy = Formulas.InertiaPower(modelData.IdleSpeed, 0.RPMtoRad(), modelData.Inertia, modelData.EngineStartTime) * modelData.EngineStartTime;
			var engineDragEnergy = VectoMath.Abs(modelData.FullLoadCurves[0].DragLoadStationaryTorque(modelData.IdleSpeed)) *
									modelData.IdleSpeed / 2.0 * modelData.EngineStartTime;

			EngineStartEnergy = (engineRampUpEnergy + engineDragEnergy) * EngineStopStartUtilityFactor / DeclarationData.AlternaterEfficiency / DeclarationData.AlternaterEfficiency;
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
			if (!outTorque.IsEqual(0, 1e-3)) {
				if (dryRun) {
					return new ResponseDryRun(this) {
						DeltaFullLoad = outTorque * ModelData.IdleSpeed,
						DeltaDragLoad = outTorque * ModelData.IdleSpeed,
						Engine = {
							TorqueOutDemand = outTorque,
							TotalTorqueDemand = outTorque,
							PowerRequest = outTorque * outAngularVelocity,
							DynamicFullLoadPower = 0.SI<Watt>(),
							DragPower = 0.SI<Watt>(),
							EngineSpeed = outAngularVelocity, // 0.RPMtoRad(),
							AuxiliariesPowerDemand = 0.SI<Watt>(),
							InertiaTorqueLoss = 0.SI<NewtonMeter>(),
							AuxTorqueDemand = 0.SI<NewtonMeter>()
						},
						DeltaEngineSpeed = 0.RPMtoRad(),
					};
				}


				var retVal = outTorque > 0
					? new ResponseOverload(this) { Delta = outTorque * ModelData.IdleSpeed }
					: (AbstractResponse)new ResponseUnderload(this) { Delta = outTorque * ModelData.IdleSpeed };
				retVal.Engine.TotalTorqueDemand = outTorque;
				retVal.Engine.TorqueOutDemand = outTorque;
				retVal.Engine.PowerRequest = outTorque * outAngularVelocity;
				retVal.Engine.DynamicFullLoadPower = 0.SI<Watt>();
				retVal.Engine.DragPower = 0.SI<Watt>();
				retVal.Engine.EngineSpeed = 0.RPMtoRad();
				retVal.Engine.AuxiliariesPowerDemand = 0.SI<Watt>();
				retVal.Engine.InertiaTorqueLoss = 0.SI<NewtonMeter>();
				retVal.Engine.AuxTorqueDemand = 0.SI<NewtonMeter>();
				return retVal;

				//throw new VectoSimulationException("Combustion engine cannot supply outtorque when switched off (T_out: {0})", outTorque);
			}
			CurrentState.EngineOn = false;
			CurrentState.EngineSpeed = ModelData.IdleSpeed;
			CurrentState.EngineTorque = 0.SI<NewtonMeter>();
			CurrentState.EngineTorqueOut = 0.SI<NewtonMeter>();
			CurrentState.EnginePower = 0.SI<Watt>();
			CurrentState.dt = dt;

			if (!dryRun) {
				//EngineAux.TorqueDemand(absTime, dt, 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>(), ModelData.IdleSpeed);
				CurrentState.AuxPowerEngineOff = EngineAux.PowerDemandEngineOff(absTime, dt);
			} else {
				return new ResponseDryRun(this) {
					DeltaFullLoad = 0.SI<Watt>(),
					DeltaDragLoad = 0.SI<Watt>(),
					Engine = {
						TorqueOutDemand = outTorque,
						PowerRequest = outTorque * outAngularVelocity,
						DynamicFullLoadPower = 0.SI<Watt>(),
						DragPower = 0.SI<Watt>(),
						EngineSpeed = outAngularVelocity, // 0.RPMtoRad(),
						AuxiliariesPowerDemand = 0.SI<Watt>(),
						TotalTorqueDemand = 0.SI<NewtonMeter>(),
						DragTorque = 0.SI<NewtonMeter>(),
						InertiaTorqueLoss = 0.SI<NewtonMeter>(),
						AuxTorqueDemand = 0.SI<NewtonMeter>()
					},
					DeltaEngineSpeed = 0.RPMtoRad(),
				};
			}

			return new ResponseSuccess(this) {
				Engine = {
					TorqueOutDemand = outTorque,
					PowerRequest = 0.SI<Watt>(),
					DynamicFullLoadPower = 0.SI<Watt>(),
					TotalTorqueDemand = 0.SI<NewtonMeter>(),
					DragPower = 0.SI<Watt>(),
					EngineSpeed = 0.RPMtoRad(),
					AuxiliariesPowerDemand = 0.SI<Watt>(),
					InertiaTorqueLoss = 0.SI<NewtonMeter>(),
					AuxTorqueDemand = 0.SI<NewtonMeter>()
				},
			};
		}

		#region Overrides of CombustionEngine

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			if (CombustionEngineOn) {
				base.DoWriteModalResults(time, simulationInterval, container);
				var engineStart = !PreviousState.EngineOn && CurrentState.EngineOn;
				container[ModalResultField.P_ice_start] = engineStart ? EngineStartEnergy / CurrentState.dt : 0.SI<Watt>();
				container[ModalResultField.P_aux_ice_off] = 0.SI<Watt>();
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
			container[ModalResultField.P_aux_ice_off] = (CurrentState.AuxPowerEngineOff ?? 0.SI<Watt>());


			var auxDemand = EngineAux.PowerDemandEngineOn(time, simulationInterval, ModelData.IdleSpeed) / ModelData.IdleSpeed;

			WriteWHRPowerEngineOff(container, ModelData.IdleSpeed, auxDemand);

			foreach (var fuel in ModelData.Fuels) {
				var fc = 0.SI<KilogramPerSecond>();
				var fcNCVcorr = fc * fuel.FuelData.HeatingValueCorrection; // TODO: wird fcNCVcorr

				var fcWHTC = fcNCVcorr * WHTCCorrectionFactor(fuel.FuelData);
				//var fcAAUX = fcWHTC;
				var advancedAux = EngineAux as BusAuxiliariesAdapter;
				if (advancedAux != null) {
					//throw new VectoException("Engine Stop/Start with advanced auxiliaries not supported!");
					advancedAux.DoWriteModalResults(time, simulationInterval, container);
					//fcAAUX = advancedAux.AAuxFuelConsumption;
				}

				
				var result = fuel.ConsumptionMap.GetFuelConsumption(auxDemand, ModelData.IdleSpeed);

				var fcESS = result.Value * (1 - EngineStopStartUtilityFactor) * fuel.FuelData.HeatingValueCorrection * WHTCCorrectionFactor(fuel.FuelData);
				var fcFinal = fcESS;

				container[ModalResultField.FCMap, fuel.FuelData] = fc;
				container[ModalResultField.FCNCVc, fuel.FuelData] = fcNCVcorr;
				container[ModalResultField.FCWHTCc, fuel.FuelData] = fcWHTC;
				//container[ModalResultField.FCAAUX, fuel.FuelData] = fcAAUX;
				container[ModalResultField.FCICEStopStart, fuel.FuelData] = fcESS;
				container[ModalResultField.FCFinal, fuel.FuelData] = fcFinal;
			}
		}

		protected virtual void WriteWHRPowerEngineOff(IModalDataContainer container, PerSecond engineSpeed, NewtonMeter engineTorque)
		{
			var pWHRelMap = 0.SI<Watt>();
			var pWHRelCorr = 0.SI<Watt>();
			var pWHRmechMap = 0.SI<Watt>();
			var pWHRmechCorr = 0.SI<Watt>();
			GetWHRPower(ModelData.ElectricalWHR, engineSpeed, engineTorque, ref pWHRelMap, ref pWHRelCorr);
			GetWHRPower(ModelData.MechanicalWHR, engineSpeed, engineTorque, ref pWHRmechMap, ref pWHRmechCorr);

			container[ModalResultField.P_WHR_el_map] = (1 - EngineStopStartUtilityFactor) * pWHRelMap;
			container[ModalResultField.P_WHR_el_corr] = (1 - EngineStopStartUtilityFactor) * pWHRelCorr;

			container[ModalResultField.P_WHR_mech_map] = (1 - EngineStopStartUtilityFactor) * pWHRmechMap;
			container[ModalResultField.P_WHR_mech_corr] = (1 - EngineStopStartUtilityFactor) * pWHRmechCorr;
		}
	}

	public class SimplePowerrtrainCombustionEngine : StopStartCombustionEngine
	{
		public SimplePowerrtrainCombustionEngine(
			IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(
			container, modelData, pt1Disabled) { }

		public EngineState EnginePreviousState { get { return PreviousState; } }
	}
}