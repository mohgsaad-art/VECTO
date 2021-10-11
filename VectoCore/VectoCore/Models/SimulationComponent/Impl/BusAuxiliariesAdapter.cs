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
using System.IO;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.OutputData;
using VectoAuxiliaries;
using VectoAuxiliaries.Pneumatics;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class BusAuxiliariesAdapter : LoggingObject, IAuxInProvider, IAuxPort
	{
		protected readonly IDataBus DataBus;
		protected internal BusAuxState CurrentState;
		protected internal BusAuxState PreviousState;

		protected internal readonly IAuxPort AdditionalAux;

		protected IAdvancedAuxiliaries Auxiliaries;
		private readonly FuelConsumptionAdapter _fcMapAdapter;

		public BusAuxiliariesAdapter(IDataBus container, string aauxFile, string cycleName, Kilogram vehicleWeight,
			FuelConsumptionMap fcMap, PerSecond engineIdleSpeed, IAuxPort additionalAux = null)
		{
			//	mAAUX_Global.advancedAuxModel.Signals.DeclarationMode = Cfg.DeclMode
			//	mAAUX_Global.advancedAuxModel.Signals.WHTC = Declaration.WHTCcorrFactor
			CurrentState = new BusAuxState();
			PreviousState = new BusAuxState { AngularSpeed = engineIdleSpeed };

			AdditionalAux = additionalAux;

			DataBus = container;
			var tmpAux = new AdvancedAuxiliaries {
				VectoInputs = {
					Cycle = DetermineCycle(cycleName),
					VehicleWeightKG = vehicleWeight
				}
			};

			_fcMapAdapter = new FuelConsumptionAdapter() { FcMap = fcMap };
			tmpAux.VectoInputs.FuelMap = _fcMapAdapter;
			tmpAux.VectoInputs.FuelDensity = FuelData.Instance().Lookup(container.FuelType).FuelDensity;

			//'Set Signals
			tmpAux.Signals.EngineIdleSpeed = engineIdleSpeed;
			tmpAux.Initialise(Path.GetFileName(aauxFile), Path.GetDirectoryName(Path.GetFullPath(aauxFile)) + @"\");

			tmpAux.Signals.TotalCycleTimeSeconds =
				tmpAux.actuationsMap.GetNumActuations(new ActuationsKey("CycleTime", tmpAux.VectoInputs.Cycle));

			// call initialize again _after_ setting the cycle time to get the correct consumtions
			tmpAux.Initialise(Path.GetFileName(aauxFile), Path.GetDirectoryName(Path.GetFullPath(aauxFile)) + @"\");


			Auxiliaries = tmpAux;
		}

		private static string DetermineCycle(string cycleName)
		{
			var cycle = cycleName.ToLower();

			if (cycle.Contains("bus")) {
				if (cycle.Contains("heavy_urban")) {
					return "Heavy urban";
				}
				if (cycle.Contains("suburban")) {
					return "Suburban";
				}
				if (cycle.Contains("interurban")) {
					return "Interurban";
				}
				if (cycle.Contains("urban")) {
					return "Urban";
				}
			}
			if (cycle.Contains("coach")) {
				return "Coach";
			}
			Logger<BusAuxiliariesAdapter>()
				.Warn("UnServiced Cycle Name '{0}' in Pneumatics Actuations Map 0 Actuations returned", cycleName);
			return cycleName;
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
			if (AdditionalAux != null) {
				AdditionalAux.Initialize(torque, angularSpeed);
			}
			PreviousState.PowerDemand = GetBusAuxPowerDemand(0.SI<Second>(), 1.SI<Second>(), torque, torque, angularSpeed);
			return PreviousState.PowerDemand / angularSpeed;
		}


		public NewtonMeter TorqueDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			CurrentState.AngularSpeed = angularSpeed;
			CurrentState.dt = dt;
			CurrentState.PowerDemand = GetBusAuxPowerDemand(absTime, dt, torquePowerTrain, torqueEngine, angularSpeed, dryRun);

			var avgAngularSpeed = (CurrentState.AngularSpeed + PreviousState.AngularSpeed) / 2.0;
			return CurrentState.PowerDemand / avgAngularSpeed;
		}


		protected internal void DoWriteModalResults(IModalDataContainer container)
		{
			_fcMapAdapter.AllowExtrapolation = true;
			// cycleStep has to be called here and not in DoCommit, write is called before Commit!
			var message = String.Empty;
			Auxiliaries.CycleStep(CurrentState.dt, ref message);
			Log.Warn(message);

			CurrentState.TotalFuelConsumption = Auxiliaries.TotalFuelGRAMS;
			container[ModalResultField.P_aux] = CurrentState.PowerDemand;

			container[ModalResultField.AA_NonSmartAlternatorsEfficiency] = Auxiliaries.AA_NonSmartAlternatorsEfficiency;
			if (Auxiliaries.AA_SmartIdleCurrent_Amps != null) {
				container[ModalResultField.AA_SmartIdleCurrent_Amps] = Auxiliaries.AA_SmartIdleCurrent_Amps;
			}
			container[ModalResultField.AA_SmartIdleAlternatorsEfficiency] = Auxiliaries.AA_SmartIdleAlternatorsEfficiency;
			if (Auxiliaries.AA_SmartTractionCurrent_Amps != null) {
				container[ModalResultField.AA_SmartTractionCurrent_Amps] =
					Auxiliaries.AA_SmartTractionCurrent_Amps;
			}
			container[ModalResultField.AA_SmartTractionAlternatorEfficiency] = Auxiliaries.AA_SmartTractionAlternatorEfficiency;
			if (Auxiliaries.AA_SmartOverrunCurrent_Amps != null) {
				container[ModalResultField.AA_SmartOverrunCurrent_Amps] = Auxiliaries.AA_SmartOverrunCurrent_Amps;
			}
			container[ModalResultField.AA_SmartOverrunAlternatorEfficiency] = Auxiliaries.AA_SmartOverrunAlternatorEfficiency;
			if (Auxiliaries.AA_CompressorFlowRate_LitrePerSec != null) {
				container[ModalResultField.AA_CompressorFlowRate_LitrePerSec] =
					Auxiliaries.AA_CompressorFlowRate_LitrePerSec;
			}
			container[ModalResultField.AA_OverrunFlag] = Auxiliaries.AA_OverrunFlag;
			container[ModalResultField.AA_EngineIdleFlag] = Auxiliaries.AA_EngineIdleFlag;
			container[ModalResultField.AA_CompressorFlag] = Auxiliaries.AA_CompressorFlag;
			if (Auxiliaries.AA_TotalCycleFC_Grams != null) {
				container[ModalResultField.AA_TotalCycleFC_Grams] = Auxiliaries.AA_TotalCycleFC_Grams;
			}
			if (Auxiliaries.AA_TotalCycleFC_Litres != null) {
				container[ModalResultField.AA_TotalCycleFC_Litres] = Auxiliaries.AA_TotalCycleFC_Litres;
			}
			if (Auxiliaries.AA_AveragePowerDemandCrankHVACMechanicals != null) {
				container[ModalResultField.AA_AveragePowerDemandCrankHVACMechanicals] =
					Auxiliaries.AA_AveragePowerDemandCrankHVACMechanicals;
			}
			if (Auxiliaries.AA_AveragePowerDemandCrankHVACElectricals != null) {
				container[ModalResultField.AA_AveragePowerDemandCrankHVACElectricals] =
					Auxiliaries.AA_AveragePowerDemandCrankHVACElectricals;
			}
			if (Auxiliaries.AA_AveragePowerDemandCrankElectrics != null) {
				container[ModalResultField.AA_AveragePowerDemandCrankElectrics] =
					Auxiliaries.AA_AveragePowerDemandCrankElectrics;
			}
			if (Auxiliaries.AA_AveragePowerDemandCrankPneumatics != null) {
				container[ModalResultField.AA_AveragePowerDemandCrankPneumatics] =
					Auxiliaries.AA_AveragePowerDemandCrankPneumatics;
			}
			if (Auxiliaries.AA_TotalCycleFuelConsumptionCompressorOff != null) {
				container[ModalResultField.AA_TotalCycleFuelConsumptionCompressorOff] =
					Auxiliaries.AA_TotalCycleFuelConsumptionCompressorOff;
			}
			container[ModalResultField.AA_TotalCycleFuelConsumptionCompressorOn] =
				Auxiliaries.AA_TotalCycleFuelConsumptionCompressorOn;
		}

		protected internal void DoCommitSimulationStep()
		{
			PreviousState = CurrentState;
			CurrentState = new BusAuxState();
		}

		protected internal KilogramPerSecond AAuxFuelConsumption
		{
			get { return (CurrentState.TotalFuelConsumption - PreviousState.TotalFuelConsumption) / CurrentState.dt; }
		}

		private Watt GetBusAuxPowerDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			_fcMapAdapter.AllowExtrapolation = true;

			Auxiliaries.Signals.ClutchEngaged = DataBus.ClutchInfo.ClutchClosed(absTime);
			Auxiliaries.Signals.EngineDrivelinePower = torquePowerTrain * angularSpeed;
			Auxiliaries.Signals.EngineDrivelineTorque = torquePowerTrain;
			Auxiliaries.Signals.InternalEnginePower = torqueEngine * angularSpeed - DataBus.Brakes.BrakePower;
			if (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Coasting) {
				// make sure smart aux are _not_ enabled for now
				// set internal_engine_power a little bit lower so there is no excessive power for smart aux
				Auxiliaries.Signals.InternalEnginePower = 0.9 * torqueEngine * angularSpeed /*- DataBus.BrakePower*/;
				// if smart aux should be on during coasting use the following line
				// set internal_engine_power to a large value (*10) so that there's excessive power for smart aux (alreadin during search operating point)
				//(float)DataBus.EngineDragPower(angularSpeed).Value() / 100;
			} else {
				if (DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Braking) {
					Auxiliaries.Signals.InternalEnginePower = 0.SI<Watt>();
					//(float)((0.9 * torqueEngine * angularSpeed - DataBus.BrakePower) / 1000).Value();
				} else {
					// smart aux should be on during braking
				}
			}
			Auxiliaries.Signals.EngineMotoringPower = -DataBus.EngineInfo.EngineDragPower(angularSpeed);
			Auxiliaries.Signals.EngineSpeed = angularSpeed;
			var avgAngularSpeed = (PreviousState.AngularSpeed + CurrentState.AngularSpeed) / 2;
			Auxiliaries.Signals.PreExistingAuxPower = AdditionalAux != null
				? AdditionalAux.TorqueDemand(absTime, dt, torquePowerTrain, torqueEngine, angularSpeed, dryRun) * avgAngularSpeed
				: 0.SI<Watt>();
			//mAAUX_Global.PreExistingAuxPower;
			Auxiliaries.Signals.Idle = DataBus.VehicleInfo.VehicleStopped;
			Auxiliaries.Signals.InNeutral = DataBus.GearboxInfo.Gear.Gear == 0;
			Auxiliaries.Signals.RunningCalc = true;

			//mAAUX_Global.Internal_Engine_Power;
			//'Power coming out of Advanced Model is in Watts.

			return Auxiliaries.AuxiliaryPowerAtCrankWatts + Auxiliaries.Signals.PreExistingAuxPower;
		}

		protected class FuelConsumptionAdapter : IFuelConsumptionMap
		{
			protected internal FuelConsumptionMap FcMap;

			public bool AllowExtrapolation { get; set; }

			public KilogramPerSecond GetFuelConsumption(NewtonMeter torque, PerSecond angularVelocity)
			{
				return FcMap.GetFuelConsumption(torque, angularVelocity, AllowExtrapolation).Value;
			}
		}

		public class BusAuxState
		{
			public Second dt;
			public PerSecond AngularSpeed;
			public Watt PowerDemand;
			public Kilogram TotalFuelConsumption = 0.SI<Kilogram>();
		}
	}
}