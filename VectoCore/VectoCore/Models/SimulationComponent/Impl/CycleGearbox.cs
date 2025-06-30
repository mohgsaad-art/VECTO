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
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class CycleGearbox : AbstractGearbox<CycleGearbox.CycleGearboxState>, ITypedGearbox
	{
		/// <summary>
		/// True if gearbox is disengaged (no gear is set).
		/// </summary>
		protected internal Second DisengagedTstmp;

		public override bool Disengaged
		{
			get { return DisengagedTstmp != null; }
			set {}
		}

		protected internal readonly TorqueConverterWrapper TorqueConverter;

		public CycleGearbox(IVehicleContainer container)
			: base(container)
		{
			if (!ModelData.Type.AutomaticTransmission()) {
				return;
			}

			var runData = container.RunData;

			// Because APTN gearbox does not have a torque converter.
			if (ModelData.Type == GearboxType.APTN) {
				return;
			}
			
			// Because IHPC gearbox does not have a torque converter.
			if (ModelData.Type == GearboxType.IHPC) {
				return;
            }

			var strategy = new CycleShiftStrategy(container);

			TorqueConverter = new TorqueConverterWrapper(runData.Cycle.Entries.All(x => x.EngineSpeed != null),
				new CycleTorqueConverter(container, ModelData.TorqueConverterData),
				new TorqueConverter(this, strategy, container, ModelData.TorqueConverterData, runData));
			if (TorqueConverter == null) {
				throw new VectoException("Torque Converter required for AT transmission!");
			}
		}

		public override void Connect(ITnOutPort other)
		{
			base.Connect(other);
			if (TorqueConverter != null) {
				TorqueConverter.NextComponent = other;
			}
		}

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			Gear = new GearshiftPosition(GetGearFromCycle(), !GetTCActiveFromCycle());

			if (TorqueConverter != null && Gear.TorqueConverterLocked == null) {
				throw new VectoSimulationException("Driving cycle does not contain information about TorqueConverter!");
			}

			var inAngularVelocity = CalculateInAngularSpeed(outAngularVelocity);
			var inTorque = 0.SI<NewtonMeter>();
			IResponse response;

			if (Gear.Gear != 0) {
				var ratio = !Gear.IsLockedGear()
					? ModelData.Gears[Gear.Gear].TorqueConverterRatio
					: ModelData.Gears[Gear.Gear].Ratio;
				inAngularVelocity = outAngularVelocity * ratio;
				var inTorqueLossResult = !Gear.IsLockedGear()
					? ModelData.Gears[Gear.Gear].TorqueConverterGearLossMap.GetTorqueLoss(outAngularVelocity, outTorque)
					: ModelData.Gears[Gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
				CurrentState.TorqueLossResult = inTorqueLossResult;
				inTorque = outTorque / ratio + inTorqueLossResult.Value;

				var torqueLossInertia = outAngularVelocity.IsEqual(0)
					? 0.SI<NewtonMeter>()
					: Formulas.InertiaPower(inAngularVelocity, PreviousState.InAngularVelocity, ModelData.Inertia, dt) /
					inAngularVelocity;

				inTorque += torqueLossInertia;

				response = Gear.TorqueConverterLocked != null && !Gear.TorqueConverterLocked.Value && TorqueConverter != null
					? TorqueConverter.Initialize(inTorque, inAngularVelocity, GetEngineSpeedFromCycle())
					: NextComponent.Initialize(inTorque, inAngularVelocity);
			} else {
				response = NextComponent.Initialize(inTorque, inAngularVelocity);
			}
			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			PreviousState.Gear = Gear;

			response.Gearbox.PowerRequest = inTorque * inAngularVelocity;
			return response;
		}

		protected virtual PerSecond CalculateInAngularSpeed(PerSecond outAngularVelocity)
		{ 
			return DataBus.EngineInfo.EngineIdleSpeed;
		}

		public override bool TCLocked => Gear.TorqueConverterLocked ?? false;

		/// <summary>
		/// Requests the Gearbox to deliver torque and angularVelocity
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>ResponseDryRun</description></item>
		/// <item><description>ResponseOverload</description></item>
		/// <item><description>ResponseGearshift</description></item>
		/// </list>
		/// </returns>
		public override IResponse Request(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);
			var gear = GetGearFromCycle();

			var TorqueConverterActive = DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
				? DataBus.DrivingCycleInfo.CycleData.LeftSample.TorqueConverterActive
				: DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive;

			if (TorqueConverter != null && TorqueConverterActive == null) {
				throw new VectoSimulationException("Driving cycle does not contain information about TorqueConverter!");
			}
			if (gear != 0 && !ModelData.Gears.ContainsKey(gear)) {
				throw new VectoSimulationException("Requested Gear {0} from driving cycle is not available", gear);
			}

			// mk 2016-11-30: added additional check for outAngularVelocity due to failing test: MeasuredSpeed_Gear_AT_PS_Run
			// mq 2016-12-16: changed check to vehicle halted due to failing test: MeasuredSpeed_Gear_AT_*
			var retVal = gear == 0 || DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted

				//|| (outAngularVelocity.IsSmallerOrEqual(0, 1) && outTorque.IsSmallerOrEqual(0, 1))
				? RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun)
				: RequestEngaged(absTime, dt, outTorque, outAngularVelocity, dryRun);

			retVal.Gearbox.PowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2;
			return retVal;
        }

        protected virtual bool? GetTCActiveFromCycle()
        { 
			return DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
				? DataBus.DrivingCycleInfo.CycleData.LeftSample.TorqueConverterActive
				: DataBus.DrivingCycleInfo.CycleData.RightSample.TorqueConverterActive;
		}

        protected virtual uint GetGearFromCycle()
		{
			return DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
				? DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear
				: DataBus.DrivingCycleInfo.CycleData.RightSample.Gear;
		}

		protected virtual PerSecond GetEngineSpeedFromCycle()
		{
			return DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
				? DataBus.DrivingCycleInfo.CycleData.LeftSample.EngineSpeed
				: DataBus.DrivingCycleInfo.CycleData.RightSample.EngineSpeed;
		}

		/// <summary>
		/// Handles requests when a gear is engaged
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="outTorque"></param>
		/// <param name="outAngularVelocity"></param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		private IResponse RequestEngaged(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			DisengagedTstmp = null;

			Gear = new GearshiftPosition(GetGearFromCycle(), !GetTCActiveFromCycle());

			var effectiveRatio = ModelData.Gears[Gear.Gear].Ratio;
			var effectiveLossMap = ModelData.Gears[Gear.Gear].LossMap;
			if (Gear.TorqueConverterLocked.HasValue && !Gear.TorqueConverterLocked.Value) {
				effectiveRatio = ModelData.Gears[Gear.Gear].TorqueConverterRatio;
				effectiveLossMap = ModelData.Gears[Gear.Gear].TorqueConverterGearLossMap;
			}

			CheckModelData(effectiveLossMap, effectiveRatio);

			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var inTorqueLossResult = effectiveLossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			CurrentState.TorqueLossResult = inTorqueLossResult;
			var inTorque = outTorque / effectiveRatio + inTorqueLossResult.Value;
			CurrentState.TorqueLossResult = inTorqueLossResult;
			var inAngularVelocity = outAngularVelocity * effectiveRatio;

			CurrentState.InertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
				? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
				avgOutAngularVelocity
				: 0.SI<NewtonMeter>();
			inTorque += CurrentState.InertiaTorqueLossOut / effectiveRatio;
			if (Gear != PreviousState.Gear &&
				ConsiderShiftLosses(Gear, outTorque)) {
				CurrentState.PowershiftLosses = ComputeShiftLosses(outTorque, outAngularVelocity, Gear);
			}
			if (CurrentState.PowershiftLosses != null) {
				var averageEngineSpeed = (DataBus.EngineInfo.EngineSpeed + outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio) / 2;
				inTorque += CurrentState.PowershiftLosses / dt / averageEngineSpeed;
			}
			if (dryRun) {
				var dryRunResponse = HandleDryRunRequest(absTime, dt, inTorque, inAngularVelocity);
				dryRunResponse.Gearbox.PowerRequest = outTorque * avgOutAngularVelocity;
				return dryRunResponse;
			}

			CurrentState.TransmissionTorqueLoss = inTorque * effectiveRatio - outTorque;

			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.Gear = Gear;

			// end critical section

			if (TorqueConverter != null && !Gear.TorqueConverterLocked.Value) {
				CurrentState.TorqueConverterActive = true;
				return TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity, GetEngineSpeedFromCycle());
			}

			if (TorqueConverter != null) {
				TorqueConverter.Locked(
					CurrentState.InTorque, CurrentState.InAngularVelocity, CurrentState.InTorque,
					CurrentState.InAngularVelocity);
			}
			var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, false);
			response.Gearbox.PowerRequest = outTorque * avgOutAngularVelocity;
			return response;
		}

		private void CheckModelData(TransmissionLossMap effectiveLossMap, double effectiveRatio)
		{
			if (effectiveLossMap == null || double.IsNaN(effectiveRatio)) {
				throw new VectoSimulationException(
					"Ratio or loss-map for gear {0}{1} invalid. Please check input data", Gear,
					Gear.TorqueConverterLocked.HasValue ? (Gear.TorqueConverterLocked.Value ? "L" : "C") : "");
			}
			if (Gear.TorqueConverterLocked.HasValue && !Gear.TorqueConverterLocked.Value && !ModelData.Gears[Gear.Gear].HasTorqueConverter) {
				throw new VectoSimulationException("Torque converter requested by cycle for gear without torque converter!");
			}
		}

		protected virtual IResponse HandleDryRunRequest(
			Second absTime, Second dt, NewtonMeter inTorque,
			PerSecond inAngularVelocity)
		{
			if (TorqueConverter != null && !Gear.TorqueConverterLocked.Value) {
				return TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity, GetEngineSpeedFromCycle(), true);
			}

			// mk 2016-12-13
			//if (outTorque.IsSmaller(0) && inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
			//	//Log.Warn("engine speed would fall below idle speed - disengage! gear from cycle: {0}, vehicle speed: {1}", Gear,
			//	//	DataBus.VehicleSpeed);
			//	Gear = 0;
			//	return RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
			//}
			return NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
		}

		/// <summary>
		/// Handles Requests when no gear is disengaged
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="outTorque"></param>
		/// <param name="outAngularVelocity"></param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		private IResponse RequestDisengaged(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			if (DisengagedTstmp == null) {
				DisengagedTstmp = absTime;
			}

			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			if (dryRun) {
				// if gearbox is disengaged the 0-line is the limit for drag and full load
				return new ResponseDryRun(this)				{
					Gearbox = {
						PowerRequest = outTorque * avgOutAngularVelocity,
					},
					DeltaDragLoad = outTorque * avgOutAngularVelocity,
					DeltaFullLoad = outTorque * avgOutAngularVelocity,
					DeltaFullLoadTorque = outTorque,
					DeltaDragLoadTorque = outTorque,
					Engine = {
						TorqueOutDemand = 0.SI<NewtonMeter>()
					}
				};
			}

			if ((outTorque * avgOutAngularVelocity).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance) &&
				!outAngularVelocity.IsEqual(0)) {
				return new ResponseOverload(this) {
					Delta = outTorque * avgOutAngularVelocity,
					Gearbox = {
						PowerRequest = outTorque * avgOutAngularVelocity
					}
				};
			}

			if ((outTorque * avgOutAngularVelocity).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				return new ResponseUnderload(this) {
					Delta = outTorque * avgOutAngularVelocity,
					Gearbox = {
						PowerRequest = outTorque * avgOutAngularVelocity
					}
				};
			}

			IResponse disengagedResponse = GetDisengagedResponse(absTime, dt, outAngularVelocity);
			
			if (TorqueConverter != null) {
				if (DataBus.VehicleInfo.VehicleStopped) {
					TorqueConverter.Locked(
						0.SI<NewtonMeter>(), disengagedResponse.Engine.EngineSpeed, CurrentState.InTorque,
						outAngularVelocity);
				} else {
					TorqueConverter.Locked(
						CurrentState.InTorque, disengagedResponse.Engine.EngineSpeed, CurrentState.InTorque,
						disengagedResponse.Engine.EngineSpeed);
				}
			}
			disengagedResponse.Gearbox.PowerRequest = outTorque * avgOutAngularVelocity;
			CurrentState.SetState(0.SI<NewtonMeter>(), disengagedResponse.Engine.EngineSpeed, 0.SI<NewtonMeter>(), outAngularVelocity);
			CurrentState.Gear = Gear;

			InvokeGearShiftTriggered();

			return disengagedResponse;
		}

		protected virtual IResponse GetDisengagedResponse(Second absTime, Second dt, PerSecond outAngularVelocity)
		{
			IResponse disengagedResponse;

			if (GearboxType.AutomaticTransmission()) {
				disengagedResponse = EngineIdleRequest(absTime, dt);
			} else {
				disengagedResponse = (NextGear.Gear > 0)
					? NextComponent.Request(
						absTime, dt, 0.SI<NewtonMeter>(),
						outAngularVelocity * ((NextGear.Gear > 0) ? ModelData.Gears[NextGear.Gear].Ratio : 1), false)
					: EngineIdleRequest(absTime, dt);
			}

			return disengagedResponse;
		}

		protected virtual IResponse EngineIdleRequest(Second absTime, Second dt)
		{
			var disengagedResponse = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), DataBus.EngineInfo.EngineIdleSpeed, false);
			if (disengagedResponse is ResponseSuccess) {
				return disengagedResponse;
			}

			var motoringSpeed = DataBus.EngineInfo.EngineSpeed;
			if (motoringSpeed.IsGreater(DataBus.EngineInfo.EngineIdleSpeed)) {
				var first = (ResponseDryRun)NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), motoringSpeed, true);
				try {
					motoringSpeed = SearchAlgorithm.Search(
						motoringSpeed, first.DeltaDragLoad,
						Constants.SimulationSettings.EngineIdlingSearchInterval,
						getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
						evaluateFunction: n => NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), n, true),
						criterion: result => ((ResponseDryRun)result).DeltaDragLoad.Value(),
						searcher: this);
				} catch (VectoException) {
					Log.Warn("CycleGearbox could not find motoring speed for disengaged state.");
				}
				motoringSpeed = motoringSpeed.LimitTo(DataBus.EngineInfo.EngineIdleSpeed, DataBus.EngineInfo.EngineSpeed);
			}
			disengagedResponse = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), motoringSpeed, false);
			return disengagedResponse;
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var avgInAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			var avgOutAngularSpeed = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			var inPower = CurrentState.InTorque * avgInAngularSpeed;
			var outPower = CurrentState.OutTorque * avgOutAngularSpeed;
			container[ModalResultField.Gear] = DisengagedTstmp != null ? 0 : Gear.Gear;
			container[ModalResultField.P_gbx_loss] = inPower - outPower;
			container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgOutAngularSpeed;
			container[ModalResultField.P_gbx_in] = inPower;
			container[ModalResultField.n_gbx_out_avg] = (PreviousState.OutAngularVelocity +
														CurrentState.OutAngularVelocity) / 2.0;
			container[ModalResultField.n_gbx_in_avg] = avgInAngularSpeed;
			container[ModalResultField.T_gbx_out] = CurrentState.OutTorque;
			container[ModalResultField.T_gbx_in] = CurrentState.InTorque;

			if (ModelData.Type.AutomaticTransmission()) {
				container[ModalResultField.TC_Locked] = !CurrentState.TorqueConverterActive;
				container[ModalResultField.P_gbx_shift_loss] = CurrentState.PowershiftLosses == null
					? 0.SI<Watt>()
					: CurrentState.PowershiftLosses * avgInAngularSpeed;
			}

			// torque converter fields are written by TorqueConverter (if present), called from Vehicle container 
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			if (Gear.Gear != 0) {
				if (CurrentState.TorqueLossResult != null && CurrentState.TorqueLossResult.Extrapolated) {
					Log.Warn(
						"Gear {0} LossMap data was extrapolated: range for loss map is not sufficient: n:{1}, torque:{2}",
						Gear, CurrentState.OutAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.OutTorque);
					if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
						throw new VectoException(
							"Gear {0} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{1}, torque:{2}",
							Gear, CurrentState.OutAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.OutTorque);
					}
				}
			}

			base.DoCommitSimulationStep(time, simulationInterval);
		}

		#region ICluchInfo

		public override Second LastUpshift
		{
			get => throw new NotImplementedException();
			protected internal set => throw new NotImplementedException();
		}

		public override Second LastDownshift
		{
			get => throw new NotImplementedException();
			protected internal set => throw new NotImplementedException();
		}

		public override GearshiftPosition NextGear
		{
			get {
				if (DisengagedTstmp == null) {
					return Gear;
				}

				var future = DataBus.DrivingCycleInfo.LookAhead(ModelData.TractionInterruption * 5);
				var nextGear = 0u;
				var torqueConverterLocked = false;
				foreach (var entry in future) {
					if (entry.VehicleTargetSpeed != null && entry.VehicleTargetSpeed.IsEqual(0)) {
						// vehicle is stopped, no next gear, engine should go to idle
						break;
					}
					if (entry.WheelAngularVelocity != null && entry.WheelAngularVelocity.IsEqual(0)) {
						// vehicle is stopped, no next gear, engine should go to idle
						break;
					}

					if (entry.Gear == 0) {
						continue;
					}

					nextGear = entry.Gear;
					torqueConverterLocked = !entry.TorqueConverterActive ?? false;
					break;
				}

				return new GearshiftPosition(nextGear, GearboxType.ManualTransmission() ? (bool?)null : torqueConverterLocked);
			}
		}

		public override Second TractionInterruption
		{
			get {
				if (DisengagedTstmp == null) {
					return ModelData.TractionInterruption;
				}

				var future = DataBus.DrivingCycleInfo.LookAhead(ModelData.TractionInterruption * 5);
				foreach (var entry in future) {
					if (entry.VehicleTargetSpeed != null && entry.VehicleTargetSpeed.IsEqual(0)) {
						// vehicle is stopped, no next gear, engine should go to idle
						break;
					}
					if (entry.WheelAngularVelocity != null && entry.WheelAngularVelocity.IsEqual(0)) {
						// vehicle is stopped, no next gear, engine should go to idle
						break;
					}

					if (entry.Gear == 0) {
						continue;
					}

					return entry.Time - DisengagedTstmp;
				}

				return ModelData.TractionInterruption;
			}
		}

		public override bool GearEngaged(Second absTime)
		{
			return (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
						? DataBus.DrivingCycleInfo.CycleData.LeftSample.Gear
						: DataBus.DrivingCycleInfo.CycleData.RightSample.Gear) != 0;
		}

		public override bool DisengageGearbox
		{
			get => false;
			set => throw new NotImplementedException();
		}

		public override void TriggerGearshift(Second absTime, Second dt)
		{
			throw new NotSupportedException();
		}

		#endregion

		protected override bool DoUpdateFrom(object other) => false;

		public class CycleGearboxState : GearboxState
		{
			public bool TorqueConverterActive;
			public WattSecond PowershiftLosses { get; set; }
		}

		
	}
}