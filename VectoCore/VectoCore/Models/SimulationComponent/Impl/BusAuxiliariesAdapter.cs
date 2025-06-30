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
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class BusAuxiliariesAdapter : VectoSimulationComponent, IBusAuxiliariesAdapter
	{
		protected internal BusAuxState CurrentState;
		protected internal BusAuxState PreviousState;

		protected internal IAuxPort AdditionalAux;

		protected IBusAuxiliaries Auxiliaries;

		private bool SmartElectricSystem;
		private IAuxiliaryConfig AuxCfg;

		//private readonly FuelConsumptionAdapter _fcMapAdapter;

		public BusAuxiliariesAdapter(
			IVehicleContainer container, IAuxiliaryConfig auxiliaryConfig, IAuxPort additionalAux = null) : base(container)
		{
			//container.AddComponent(this);

			CurrentState = new BusAuxState();
			PreviousState = new BusAuxState { AngularSpeed = container.EngineInfo.EngineIdleSpeed };

			AdditionalAux = additionalAux;
			AuxCfg = auxiliaryConfig;
			DataBus = container;

			if (container.ModalData != null) {
				container.ModalData.AuxHeaterDemandCalc = AuxHeaterDemandCalculation;
			}

			var electricStorage =
				AuxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart &&
				AuxCfg.ElectricalUserInputsConfig.ConnectESToREESS
					// in case of smart alternator with Px hybrid take electric power from P0 REESS first, then from HEV REESS.
					// do not use alternator to generate demanded power if P0 REESS is empty. so trick busaux that there is always
					// energy in the battery.
					? (ISimpleBatteryInfo)new InfinityBattery(AuxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity, new ElectricStorageWrapper(this)) 
					: new ElectricStorageWrapper(this);
			var tmpAux = AuxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.None
				? new BusAuxiliariesNoAlternator(electricStorage)
				: new BusAuxiliaries.BusAuxiliaries(electricStorage);


			//'Set Signals
			tmpAux.Signals.EngineIdleSpeed = DataBus.EngineInfo.EngineIdleSpeed;
			tmpAux.Initialise(AuxCfg);

			SmartElectricSystem = AuxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart;

			Auxiliaries = tmpAux;
		}

		
		public IDCDCConverter DCDCConverter { get; set; }

		public ISimpleBattery ElectricStorage { get; set; }

		public virtual HeaterDemandResult AuxHeaterDemandCalculation(Second cycleTime, Joule engineWasteHeatTotal, Joule electricMotorWasteHeatTotal)
		{
			return Auxiliaries.AuxHeaterDemandCalculation(cycleTime, engineWasteHeatTotal, electricMotorWasteHeatTotal);
		}

		public IAuxPort Port()
		{
			return this;
		}

		public NewtonMeter Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			//PreviousState.TotalFuelConsumption = 0.SI<Kilogram>();
			PreviousState.AngularSpeed = angularSpeed;
			CurrentState.AngularSpeed = angularSpeed;
			AdditionalAux?.Initialize(torque, angularSpeed);
			DCDCConverter?.Initialize();
			PreviousState.PowerDemand = GetBusAuxPowerDemand(0.SI<Second>(), 1.SI<Second>(), torque, angularSpeed) +
										(AdditionalAux?.PowerDemandESSEngineOn(0.SI<Second>(), 1.SI<Second>(), angularSpeed) ?? 0.SI<Watt>());
			if (angularSpeed.IsEqual(0)) {
				return 0.SI<NewtonMeter>();
			}

			return PreviousState.PowerDemand / angularSpeed;
		}


		public virtual NewtonMeter TorqueDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, PerSecond angularSpeed, bool dryRun = false)
		{
			CurrentState.AngularSpeed = angularSpeed;
			CurrentState.dt = dt;

			var signals = Auxiliaries.Signals;
			// trick bus auxiliaries that ice is on - all auxiliaries are considered. ESS is corrected in post-processing
			signals.EngineStopped = !DataBus.EngineCtl.CombustionEngineOn; //false;
			signals.InNeutral = !DataBus.EngineCtl.CombustionEngineOn;																		   
			signals.VehicleStopped = DataBus.VehicleInfo.VehicleStopped; // false; 

			CurrentState.PowerDemand = GetBusAuxPowerDemand(absTime, dt, torquePowerTrain, angularSpeed, dryRun) +
										(AdditionalAux?.PowerDemandESSEngineOn(0.SI<Second>(), 1.SI<Second>(), angularSpeed) ?? 0.SI<Watt>());

			var avgAngularSpeed = (CurrentState.AngularSpeed + PreviousState.AngularSpeed) / 2.0;
			if (avgAngularSpeed.IsEqual(0)) {
				return 0.SI<NewtonMeter>();
            }
			return CurrentState.PowerDemand / avgAngularSpeed;
		}

		public virtual Watt PowerDemandESSEngineOn(Second time, Second simulationInterval, PerSecond engineSpeed)
		{
			var signals = Auxiliaries.Signals;
			signals.EngineStopped = false;
			signals.VehicleStopped = DataBus.VehicleInfo.VehicleStopped;

			var busAuxPwrICEOn = GetBusAuxPowerDemand(time, simulationInterval, 0.SI<NewtonMeter>(), engineSpeed, true);
			var esICEOnLoad = Auxiliaries.ElectricPowerConsumerSum;

			signals.EngineStopped = true;
			var busAuxPwrICEOff = GetBusAuxPowerDemand(time, simulationInterval, 0.SI<NewtonMeter>(), engineSpeed, true);
			var esICEOffLoad = Auxiliaries.ElectricPowerConsumerSum;

			// if busAuxPwrICEOn and busAuxPwrICEOff are different the battery is empty and the mechanical power is the difference
			// if both are equal we need to add the difference between ES ICE On and ES ICE Off power demand (the latter is corrected by ES correction) 

			var esSupplyNotFromICE = AuxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.None ||
									(AuxCfg.ElectricalUserInputsConfig.ConnectESToREESS);

			var esMech = (busAuxPwrICEOn - busAuxPwrICEOff).IsEqual(0) && !esSupplyNotFromICE
				? (esICEOnLoad - esICEOffLoad) / AuxCfg.ElectricalUserInputsConfig.AlternatorGearEfficiency /
				AuxCfg.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>())
				: 0.SI<Watt>();

			return busAuxPwrICEOff - Auxiliaries.PSPowerDemandAirGenerated + esMech - Auxiliaries.ElectricPowerDemandMech + (busAuxPwrICEOn - busAuxPwrICEOff) +
					(AdditionalAux?.PowerDemandESSEngineOn(0.SI<Second>(), 1.SI<Second>(), engineSpeed) ??
					0.SI<Watt>());
		}

		protected virtual Watt ComputePowerDemand(Second time, Second simulationInterval, PerSecond engineSpeed, bool includeESBaseLoad = true)
		{
			var signals = Auxiliaries.Signals;
			//signals.EngineStopped = true;
			//signals.VehicleStopped = DataBus.VehicleInfo.VehicleStopped;
			var esBaseLoad = Auxiliaries.ElectricPowerConsumerSum;

			//signals.EngineStopped = false; 
			//signals.VehicleStopped = false;
			var retVal =  GetBusAuxPowerDemand(time, simulationInterval, 0.SI<NewtonMeter>(), engineSpeed, true) +
						(AdditionalAux?.PowerDemandESSEngineOn(0.SI<Second>(), 1.SI<Second>(), engineSpeed) ?? 0.SI<Watt>());

			if (!SmartElectricSystem) {
				return retVal;
			}

			//var batteryPwr = ElectricStorage.SOC * AuxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity / simulationInterval;
			var esSum = Auxiliaries.ElectricPowerConsumerSum - esBaseLoad;
			//if (batteryPwr < esSum) {
				retVal += (esSum ) / AuxCfg.ElectricalUserInputsConfig.AlternatorGearEfficiency /
						AuxCfg.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>());
			//}

			return retVal;
		}

		public virtual Watt PowerDemandESSEngineOff(Second absTime, Second dt)
		{
			
			//CurrentState.AngularSpeed = DataBus.EngineInfo.EngineIdleSpeed;
			//CurrentState.dt = dt;

			var signals = Auxiliaries.Signals;

			signals.EngineStopped = !DataBus.EngineCtl.CombustionEngineOn;
			signals.InNeutral = !DataBus.EngineCtl.CombustionEngineOn;
			signals.VehicleStopped = DataBus.VehicleInfo.VehicleStopped;

			var busAuxPowerDemand  = GetBusAuxPowerDemand(
				absTime, dt, 0.SI<NewtonMeter>(), DataBus.EngineInfo.EngineIdleSpeed, true) - Auxiliaries.ElectricPowerDemandMech;
            //AdditionalAux = conventionalAux;

            CurrentState.PowerDemand = 0.SI<Watt>();
            //CurrentState.ESPowerGeneratedICE_On = Auxiliaries.ElectricPowerGenerated;
            //CurrentState.ESPowerMech = Auxiliaries.ElectricPowerDemandMech;
            // 


            //busAuxPowerDemand = GetBusAuxPowerDemand(
            //	absTime, dt, 0.SI<NewtonMeter>(), DataBus.EngineInfo.EngineIdleSpeed);
            //AdditionalAux = conventionalAux;

            return (busAuxPowerDemand - Auxiliaries.PSPowerDemandAirGenerated + (AdditionalAux?.PowerDemandESSEngineOff(absTime, dt) ?? 0.SI<Watt>()));
		}



		public virtual void DoWriteModalResultsICE(Second absTime, Second dt, IModalDataContainer container)
		{
			// called from ICE - write modal results there

			var essUtilityFactor = 1.0;
			if (!DataBus.EngineCtl.CombustionEngineOn) {
				essUtilityFactor = 0.0;
			}

            var signals = Auxiliaries.Signals;
            signals.EngineStopped = !DataBus.EngineCtl.CombustionEngineOn;
			signals.InNeutral = !DataBus.EngineCtl.CombustionEngineOn;
            signals.VehicleStopped = DataBus.VehicleInfo.VehicleStopped;

            // cycleStep has to be called here and not in DoCommit, write is called before Commit!
            //var oldSOC = Auxiliaries.BatterySOC;
            Auxiliaries.CycleStep(CurrentState.dt);
			//var newSOC = Auxiliaries.BatterySOC;

			//CurrentState.TotalFuelConsumption = Auxiliaries.TotalFuel;
			if (container.HasCombustionEngine) {
				container[ModalResultField.P_aux_mech] = CurrentState.PowerDemand;
			}

			container[ModalResultField.P_busAux_ES_HVAC] = /*essUtilityFactor **/ Auxiliaries.HVACElectricalPowerConsumer;
			container[ModalResultField.P_busAux_ES_other] = /*essUtilityFactor **/ Auxiliaries.ElectricPowerConsumer;
			container[ModalResultField.P_busAux_ES_consumer_sum] = /*essUtilityFactor **/ Auxiliaries.ElectricPowerConsumerSum;
			container[ModalResultField.P_busAux_ES_sum_mech] = essUtilityFactor * Auxiliaries.ElectricPowerDemandMech;
			container[ModalResultField.P_busAux_ES_generated] = essUtilityFactor * Auxiliaries.ElectricPowerGenerated;

			if (AuxCfg.ElectricalUserInputsConfig.ConnectESToREESS) {
				container[ModalResultField.P_busAux_ES_HVAC] = Auxiliaries.HVACElectricalPowerConsumer;
				container[ModalResultField.P_busAux_ES_other] = Auxiliaries.ElectricPowerConsumer;
				container[ModalResultField.P_busAux_ES_consumer_sum] = Auxiliaries.ElectricPowerConsumerSum;
				container[ModalResultField.P_busAux_ES_sum_mech] = Auxiliaries.ElectricPowerDemandMech;
				container[ModalResultField.P_busAux_ES_generated] = Auxiliaries.ElectricPowerGenerated;
				if (SmartElectricSystem) {
					container[ModalResultField.BatterySOC] = ElectricStorage.SOC * 100.0;
					container[ModalResultField.P_busAux_bat] = ElectricStorage.ConsumedEnergy / dt;
				}
			} else {
				if (SmartElectricSystem) {
					var batteryPwr = ElectricStorage.ConsumedEnergy / dt;

					container[ModalResultField.BatterySOC] = ElectricStorage.SOC * 100.0;
					container[ModalResultField.P_busAux_bat] = ElectricStorage.ConsumedEnergy / dt;

					container[ModalResultField.P_busAux_ES_generated] = essUtilityFactor *
																		(DataBus.VehicleInfo.VehicleStopped &&
																		!DataBus.EngineCtl.CombustionEngineOn
																			? Auxiliaries.ElectricPowerConsumerSum
																			: Auxiliaries.ElectricPowerGenerated);
					container[ModalResultField.P_busAux_ES_sum_mech] =
						essUtilityFactor * (Auxiliaries.ElectricPowerConsumerSum + batteryPwr) /
						AuxCfg.ElectricalUserInputsConfig.AlternatorGearEfficiency /
						AuxCfg.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>());

				}
			}

			if (AuxCfg.PneumaticUserInputsConfig.CompressorMap != null) {
				container[ModalResultField.Nl_busAux_PS_consumer] = Auxiliaries.PSDemandConsumer;
				container[ModalResultField.Nl_busAux_PS_generated] = essUtilityFactor * Auxiliaries.PSAirGenerated;
				container[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = essUtilityFactor * Auxiliaries.PSAirGeneratedAlwaysOn;
			} else {
				// electric compressor
				container[ModalResultField.Nl_busAux_PS_consumer] = Auxiliaries.PSDemandConsumer;
				container[ModalResultField.Nl_busAux_PS_generated] = Auxiliaries.PSAirGenerated;
				container[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = Auxiliaries.PSAirGeneratedAlwaysOn;
			}

			//container[ModalResultField.Nl_busAux_PS_generated_dragOnly] = Auxiliaries.PSAirGeneratedDrag;
			container[ModalResultField.P_busAux_PS_generated] = essUtilityFactor * Auxiliaries.PSPowerDemandAirGenerated;
			container[ModalResultField.P_busAux_PS_generated_alwaysOn] = essUtilityFactor * Auxiliaries.PSPowerCompressorAlwaysOn;
			container[ModalResultField.P_busAux_PS_generated_dragOnly] = essUtilityFactor * Auxiliaries.PSPowerCompressorDragOnly;

			container[ModalResultField.P_busAux_HVACmech_consumer] = essUtilityFactor * Auxiliaries.HVACMechanicalPowerConsumer;
			container[ModalResultField.P_busAux_HVACmech_gen] = essUtilityFactor *  Auxiliaries.HVACMechanicalPowerGenerated;
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			// called from base class - do nothing here
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			// called from base class - do nothing here
		}

		protected internal virtual void DoCommitSimulationStep()
		{
			// called from combustion engine - do commit here
			PreviousState = CurrentState;
			CurrentState = new BusAuxState();
		}

		protected virtual Watt GetBusAuxPowerDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, PerSecond angularSpeed, bool dryRun = false)
		{
			Auxiliaries.ResetCalculations();

			var signals = Auxiliaries.Signals;

			signals.SimulationInterval = dt;
			signals.ClutchEngaged = DataBus.ClutchInfo.ClutchClosed(absTime) && DataBus.GearboxInfo.GearEngaged(absTime);
			signals.EngineDrivelineTorque = torquePowerTrain;
			
			signals.EngineSpeed = angularSpeed;
			var avgAngularSpeed = (PreviousState.AngularSpeed + CurrentState.AngularSpeed) / 2;


			var preExistingAuxPower = AdditionalAux != null
				? AdditionalAux.TorqueDemand(absTime, dt, torquePowerTrain, angularSpeed, dryRun) * avgAngularSpeed
				: 0.SI<Watt>();

			var drivetrainPower = torquePowerTrain * avgAngularSpeed;
			if (!dryRun && !DataBus.IsTestPowertrain && DataBus.DriverInfo.DrivingAction == DrivingAction.Brake && CurrentState.ExcessiveDragPower.IsEqual(0)) {
				CurrentState.ExcessiveDragPower = drivetrainPower -
												(DataBus.EngineInfo.EngineDragPower(avgAngularSpeed) - preExistingAuxPower) - DataBus.Brakes.BrakePower;
			}

			if (!dryRun && DataBus.DriverInfo.DrivingAction == DrivingAction.Brake && torquePowerTrain.IsGreater(0) &&
				DataBus.GearboxInfo.Gear.TorqueConverterLocked.HasValue &&
				!DataBus.GearboxInfo.Gear.TorqueConverterLocked.Value) {
				CurrentState.ExcessiveDragPower = 0.SI<Watt>();
			}
			if (!dryRun && DataBus.DriverInfo.DrivingAction != DrivingAction.Brake) {
				CurrentState.ExcessiveDragPower = 0.SI<Watt>();
			}

			
			signals.ExcessiveDragPower = CurrentState.ExcessiveDragPower;
			signals.Idle = DataBus.VehicleInfo.VehicleStopped;
			signals.InNeutral = DataBus.GearboxInfo.Gear.Gear == 0 || !DataBus.EngineCtl.CombustionEngineOn;



			var maxChg = ElectricStorage.MaxChargeEnergy();
			var maxDischg = ElectricStorage.MaxDischargeEnergy();
			
			var essFactor = DataBus.EngineCtl.CombustionEngineOn ? 0.0 : 1.0;
			var elPwrGen = Auxiliaries.ElectricPowerGenerated;
			var elPwrConsumed = Auxiliaries.ElectricPowerConsumerSum;

			var energyDemand = (elPwrGen * (1 - essFactor) - elPwrConsumed)  * dt;

			var batEnergy = energyDemand.LimitTo(maxDischg, maxChg);

			if (SmartElectricSystem) {
				ElectricStorage.ConsumeEnergy(batEnergy, dryRun);
			}

			var missingEnergy = energyDemand - batEnergy;

			if (AuxCfg.ElectricalUserInputsConfig.ConnectESToREESS) {
				if (DCDCConverter is null) {
					throw new VectoException("DCDCConverter is missing: The current configuration for the bus auxiliaries " +
										     "requires a DCDCConverter (ES supply from HEV REESS is activated).");
				}
				DCDCConverter.ConsumerEnergy(-missingEnergy, dryRun);
			} else {
				if (!dryRun) {
					CurrentState.MissingElectricEnergy = missingEnergy;
				}
			}

			return Auxiliaries.AuxiliaryPowerAtCrankWatts;
		}

		public class BusAuxState
		{
			public Second dt;
			public PerSecond AngularSpeed;
			public Watt PowerDemand { get; set; }
			public WattSecond MissingElectricEnergy { get; set; }

			public Watt ExcessiveDragPower = 0.SI<Watt>();

			public BusAuxState Clone() => (BusAuxState)MemberwiseClone();
		}

		public class ElectricStorageWrapper : ISimpleBatteryInfo
		{
			private BusAuxiliariesAdapter busAuxAdapter;

			public ElectricStorageWrapper(BusAuxiliariesAdapter busAuxiliariesAdapter)
			{
				busAuxAdapter = busAuxiliariesAdapter;
			}

			#region Implementation of ISimpleBatteryInfo

			public double SOC => busAuxAdapter.ElectricStorage.SOC;

			public WattSecond Capacity => busAuxAdapter.ElectricStorage.Capacity;

			#endregion
		}

		public class InfinityBattery : ISimpleBatteryInfo
		{
			private ISimpleBatteryInfo ElectricStorage;

			public InfinityBattery(WattSecond electricStorageCapacity, ElectricStorageWrapper electricStorageWrapper)
			{
				Capacity = electricStorageCapacity;
				ElectricStorage = electricStorageWrapper;
			}

			#region Implementation of ISimpleBatteryInfo

			public double SOC => ElectricStorage.SOC.IsEqual(1, 1e-2) ? 1 : 0.5;
			public WattSecond Capacity { get; }

			#endregion
		}

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other) {
			if (other is BusAuxiliariesAdapter b) {
				PreviousState = b.PreviousState.Clone();
				return ElectricStorage.UpdateFrom(b.ElectricStorage);
			}

			return false;
		}

		#endregion
	}
}
