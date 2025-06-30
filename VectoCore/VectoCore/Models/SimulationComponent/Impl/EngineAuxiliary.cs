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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    /// <summary>
    /// Container Class for Auxiliaries which are connected to the Engine.
    /// </summary>
    public class EngineAuxiliary : StatefulVectoSimulationComponent<EngineAuxiliary.State>, IEngineAuxiliary
	{
		protected readonly Dictionary<string, Func<PerSecond, Second, Second, bool, Watt>> Auxiliaries =
			new Dictionary<string, Func<PerSecond, Second, Second, bool, Watt>>();

		protected double EngineStopStartUtilityFactor;
		private bool _writePTO;

		public EngineAuxiliary(IVehicleContainer container) : base(container)
		{
			EngineStopStartUtilityFactor = 1; // container.RunData?.DriverData?.EngineStopStart?.UtilityFactorStandstill ?? double.NaN;
			_writePTO = container.RunData?.PTO?.ConsumerType == PTOConsumerType.mechanical;
		}

		public IAuxPort Port()
		{
			return this;
		}

		/// <summary>
		/// Adds a constant power demand auxiliary.
		/// </summary>
		/// <param name="auxId"></param>
		/// <param name="powerDemand"></param>
		/// <param name="columnName"></param>
		public void AddConstant(string auxId, Watt powerDemand, string columnName = null)
		{
			Add(auxId, (nEng, absTime, dt, dryRun) => powerDemand, columnName);
		}

		/// <summary>
		/// Adds an auxiliary which gets its power demand from the driving cycle.
		/// </summary>
		/// <param name="auxId"></param>
		public void AddCycle(string auxId)
		{
            Add(auxId, (nEng, absTime, dt, dryRun) => DataBus.DrivingCycleInfo?.CycleData.LeftSample.AdditionalAuxPowerDemand ?? 0.SI<Watt>());
		}

		public void AddCycle(string auxId, Func<DrivingCycleData.DrivingCycleEntry, Watt> powerLossFunc, string columnName = null)
		{
			Add(auxId, (nEng, absTime, dt, dryRun) => powerLossFunc(DataBus.DrivingCycleInfo.CycleData.LeftSample), columnName);
		}



		/// <summary>
		/// Adds an auxiliary with a function returning the power demand based on the engine speed.
		/// </summary>
		/// <param name="auxId"></param>
		/// <param name="powerLossFunction"></param>
		/// <param name="columnName"></param>
		public void Add(string auxId, Func<PerSecond, Second, Second, bool, Watt> powerLossFunction, string columnName = null)
		{
			Auxiliaries[auxId] = powerLossFunction;
			(DataBus as IVehicleContainer)?.AddAuxiliary(auxId, columnName);
			//(DataBus as IVehicleContainer)?.SumData?.AddAuxiliary(auxId);
		}

		public NewtonMeter Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			PreviousState.AngularSpeed = angularSpeed;
			if (angularSpeed.IsEqual(0)) {
				return 0.SI<NewtonMeter>();
			}

			return ComputePowerDemand(angularSpeed, 0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, false) / angularSpeed;
		}

		/// <summary>
		/// Calculates the torque demand for all registered auxiliaries for the the current engine 
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="torquePowerTrain"></param>
		/// <param name="angularSpeed"></param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		public NewtonMeter TorqueDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, PerSecond angularSpeed, bool dryRun = false)
		{
			var iceOn = !DataBus.EngineInfo.EngineOn && DataBus.EngineCtl.CombustionEngineOn;
			var prevAngularSpeed = iceOn ? DataBus.EngineInfo.EngineSpeed : PreviousState.AngularSpeed;

			var avgAngularSpeed = prevAngularSpeed != null
				? (angularSpeed + prevAngularSpeed) / 2.0
				: angularSpeed;
			if (!dryRun) {
				CurrentState.AngularSpeed = angularSpeed;
			}
			if (avgAngularSpeed.IsGreater(0)) {
				return ComputePowerDemand(avgAngularSpeed, absTime, dt, dryRun) / avgAngularSpeed;
			}
			return 0.SI<NewtonMeter>();
		}

		public Watt PowerDemandESSEngineOff(Second absTime, Second dt)
		{

			var auxiliarieIgnoredDuringVehicleStop = new[] {
				Constants.Auxiliaries.IDs.SteeringPump, 
				Constants.Auxiliaries.IDs.Fan,
				Constants.Auxiliaries.IDs.PTOConsumer, 
				Constants.Auxiliaries.IDs.PTOTransmission,
				Constants.Auxiliaries.IDs.ENGMode_AUX_MECH_FAN, 
				Constants.Auxiliaries.IDs.ENGMode_AUX_MECH_STP
			};
			var auxiliarieIgnoredDuringDrive = new[] {
				Constants.Auxiliaries.IDs.Fan,
				Constants.Auxiliaries.IDs.ENGMode_AUX_MECH_FAN
			};
			var powerDemands = new Dictionary<string, Watt>(Auxiliaries.Count);
			var engineOffDemand = 0.SI<Watt>();
			foreach (var item in Auxiliaries) {

				var value =  item.Value(DataBus.EngineInfo.EngineIdleSpeed, absTime, dt, true) ;
				if (value == null) {
					continue;
				}

				powerDemands[item.Key] = value * (1-EngineStopStartUtilityFactor);
				if (DataBus.VehicleInfo.VehicleStopped) {
					engineOffDemand += auxiliarieIgnoredDuringVehicleStop.Contains(item.Key)
						? 0.SI<Watt>()
						: value * EngineStopStartUtilityFactor;
				} else {
					engineOffDemand += auxiliarieIgnoredDuringDrive.Contains(item.Key)
						? 0.SI<Watt>()
						: value * EngineStopStartUtilityFactor;
				}
			}
			CurrentState.PowerDemands = powerDemands;
			return engineOffDemand;  //powerDemands.Sum(kv => kv.Value); 
		}

		public Watt PowerDemandESSEngineOn(Second absTime, Second dt, PerSecond engineSpeed)
		{
			return ComputePowerDemand(engineSpeed, absTime, dt, true);
		}

		protected Watt ComputePowerDemand(PerSecond engineSpeed, Second absTime, Second dt, bool dryRun)
		{
			var powerDemands = new Dictionary<string, Watt>(Auxiliaries.Count);
			foreach (var item in Auxiliaries) {
				var value = item.Value(engineSpeed, absTime, dt, dryRun);
				if (value != null) {
					powerDemands[item.Key] = value;
				}
			}
			if (!dryRun) {
				CurrentState.PowerDemands = powerDemands;
			}
			return powerDemands.Sum(kv => kv.Value) ?? 0.SI<Watt>();
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var auxPowerDemand = 0.SI<Watt>();
			var excludedFromAuxSum = new[] {
				Constants.Auxiliaries.IDs.PTOTransmission, Constants.Auxiliaries.IDs.PTOConsumer,
				Constants.Auxiliaries.IDs.PTODuringDrive, Constants.Auxiliaries.IDs.PTORoadsweeping
			};
			var ptoConsumerAggregation = new[] {
				Constants.Auxiliaries.IDs.PTOConsumer,
				Constants.Auxiliaries.IDs.PTODuringDrive, Constants.Auxiliaries.IDs.PTORoadsweeping
			};
			if (CurrentState.PowerDemands != null) {
				foreach (var kv in CurrentState.PowerDemands) {
					container[kv.Key] = kv.Value;
					// mk 2016-10-11: pto's should not be counted in sum auxiliary power demand
					if (!excludedFromAuxSum.Contains(kv.Key)) {
						auxPowerDemand += kv.Value;
					}
				}

				if (_writePTO) {
					var ptoConsumer = 0.SI<Watt>();
					foreach (var kv in CurrentState.PowerDemands) {
						if (ptoConsumerAggregation.Contains(kv.Key)) {
							ptoConsumer += kv.Value;
						}
					}

					if (container.ContainsColumn(Constants.Auxiliaries.IDs.PTOConsumer) && (container[Constants.Auxiliaries.IDs.PTOConsumer] == null ||
						container[Constants.Auxiliaries.IDs.PTOConsumer] == DBNull.Value)) {
						container[Constants.Auxiliaries.IDs.PTOConsumer] = ptoConsumer;
					}
				}
			}

			if (container[ModalResultField.P_aux_mech] == null || container[ModalResultField.P_aux_mech] == DBNull.Value) {


				// only overwrite if nobody else already wrote the total aux power
				container[ModalResultField.P_aux_mech] = auxPowerDemand;
			}
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		public class State
		{
			public PerSecond AngularSpeed;
			public Dictionary<string, Watt> PowerDemands;

			public State Clone() => (State)MemberwiseClone();
		}

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other) {
			if (other is EngineAuxiliary a) {
				PreviousState = a.PreviousState.Clone();
				return true;
			}

			return false;
		}

		#endregion
	}
}