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

using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATGearbox : AbstractGearbox<ATGearbox.ATGearboxState>
	{
		private readonly IShiftStrategy _strategy;
		protected internal readonly TorqueConverter TorqueConverter;
		private IIdleController _idleController;
		protected bool RequestAfterGearshift;

		private WattSecond _powershiftLossEnergy;
		protected internal KilogramSquareMeter EngineInertia;

		public bool TorqueConverterLocked
		{
			get { return CurrentState.TorqueConverterLocked; }
			//set { CurrentState.TorqueConverterLocked = value; }
		}

		public override bool TCLocked { get { return Gear.TorqueConverterLocked.Value; } }

		public ATGearbox(IVehicleContainer container, IShiftStrategy strategy)
			: base(container)
		{
			_strategy = strategy;
			_strategy.Gearbox = this;
			LastShift = -double.MaxValue.SI<Second>();
			TorqueConverter = new TorqueConverter(this, _strategy, container, ModelData.TorqueConverterData,
				container.RunData);
			EngineInertia = container.RunData.EngineData.Inertia;
		}

		public IIdleController IdleController
		{
			get { return _idleController; }
			set
			{
				_idleController = value;
				_idleController.RequestPort = NextComponent;
			}
		}

		public bool Disengaged
		{
			get { return CurrentState.Disengaged; }
			set { CurrentState.Disengaged = value; }
		}

		public override void Connect(ITnOutPort other)
		{
			base.Connect(other);
			TorqueConverter.NextComponent = other;
		}

		public override GearshiftPosition NextGear
		{
			get { return _strategy.NextGear; }
		}

		#region Overrides of AbstractGearbox<ATGearboxState>

		public override GearshiftPosition Gear { get { return _gear; } protected internal set { _gear = value;
			//if (PreviousState.Gear == value) {
			//	RequestAfterGearshift = false;
			//}
		} }

		#endregion

		public override bool DisengageGearbox { get; set; }

		public override bool ClutchClosed(Second absTime)
		{
			return absTime.IsGreater(DataBus.AbsTime) ||
					!(CurrentState.Disengaged || (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted));
		}

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (CurrentState.Disengaged) {
				Gear = _strategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, outTorque,
					outAngularVelocity);
			}
			var inAngularVelocity = 0.SI<PerSecond>();
			var inTorque = 0.SI<NewtonMeter>();
			var effectiveRatio = ModelData.Gears[Gear.Gear].Ratio;
			var effectiveLossMap = ModelData.Gears[Gear.Gear].LossMap;
			if (!CurrentState.TorqueConverterLocked) {
				effectiveRatio = ModelData.Gears[Gear.Gear].TorqueConverterRatio;
				effectiveLossMap = ModelData.Gears[Gear.Gear].TorqueConverterGearLossMap;
			}
			if (!DataBus.VehicleInfo.VehicleStopped) {
				inAngularVelocity = outAngularVelocity * effectiveRatio;
				var torqueLossResult = effectiveLossMap.GetTorqueLoss(outAngularVelocity, outTorque);
				CurrentState.TorqueLossResult = torqueLossResult;

				inTorque = outTorque / effectiveRatio + torqueLossResult.Value;
			}
			if (CurrentState.Disengaged) {
				return NextComponent.Initialize(0.SI<NewtonMeter>(), null);
			}

			if (!CurrentState.TorqueConverterLocked && !ModelData.Gears[Gear.Gear].HasTorqueConverter) {
				throw new VectoSimulationException(
					"Torque converter requested by strategy for gear without torque converter!");
			}
			var response = CurrentState.TorqueConverterLocked
				? NextComponent.Initialize(inTorque, inAngularVelocity)
				: TorqueConverter.Initialize(inTorque, inAngularVelocity);

			CurrentState.TorqueLossResult = new TransmissionLossMap.LossMapResult() {
				Value = 0.SI<NewtonMeter>(),
				Extrapolated = false
			};

			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.Gear = Gear;
			PreviousState.TorqueConverterLocked = CurrentState.TorqueConverterLocked;
			return response;
		}

		

		internal ResponseDryRun Initialize(GearshiftPosition gear, bool torqueConverterLocked, NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			var effectiveRatio = torqueConverterLocked
				? ModelData.Gears[gear.Gear].Ratio
				: ModelData.Gears[gear.Gear].TorqueConverterRatio;

			var inAngularVelocity = outAngularVelocity * effectiveRatio;
			var torqueLossResult = torqueConverterLocked
				? ModelData.Gears[gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque)
				: ModelData.Gears[gear.Gear].TorqueConverterGearLossMap.GetTorqueLoss(outAngularVelocity, outTorque);

			var inTorque = outTorque / effectiveRatio + torqueLossResult.Value;

			IResponse response;
			if (torqueConverterLocked) {
				response = NextComponent.Initialize(inTorque, inAngularVelocity);
			} else {
				if (!ModelData.Gears[gear.Gear].HasTorqueConverter) {
					throw new VectoSimulationException(
						"Torque converter requested by strategy for gear without torque converter!");
				}
				response = TorqueConverter.Initialize(inTorque, inAngularVelocity);
			}

			response.Switch().
				Case<ResponseSuccess>(). // accept
				Case<ResponseUnderload>(). // accept
				Case<ResponseOverload>(). // accept
				Default(r => {
					throw new UnexpectedResponseException("AT-Gearbox.Initialize", r);
				});

			return new ResponseDryRun(this) {
				Engine = {
					EngineSpeed = response.Engine.EngineSpeed,
					PowerRequest = response.Engine.PowerRequest,
				},
				Gearbox = {
					PowerRequest = outTorque * outAngularVelocity,
				}
			};
		}

		public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			IterationStatistics.Increment(this, "Requests");

			Log.Debug("AT-Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);

			var driveOffSpeed = DataBus.VehicleInfo.VehicleStopped && outAngularVelocity > 0;
			var driveOffTorque = CurrentState.Disengaged && outTorque.IsGreater(0, 1e-1);
			if (!dryRun && (driveOffSpeed || driveOffTorque)) {
				Gear = ModelData.GearList.First();
				CurrentState.TorqueConverterLocked = false;
				LastShift = absTime;
				CurrentState.Disengaged = false;
			}

			IResponse retVal;
			var count = 0;
			var loop = false;
			SetPowershiftLossEnergy(absTime, dt, outTorque, outAngularVelocity);
			do {
				if (CurrentState.Disengaged || (DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Halted)) {
					// only when vehicle is halted or close before halting
					retVal = RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
				} else {
					CurrentState.Disengaged = false;
					retVal = RequestEngaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
					IdleController.Reset();
				}
				if (!(retVal is ResponseGearShift)) {
					continue;
				}
				if (ConsiderShiftLosses(_strategy.NextGear, outTorque) && !RequestAfterGearshift) {
					retVal = new ResponseFailTimeInterval(this) {
						DeltaT = ModelData.PowershiftShiftTime,
						Gearbox = {
							PowerRequest =
							outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0
						}
					};
					RequestAfterGearshift = true;
					LastShift = absTime;
				} else {
					loop = true;
					Gear = _strategy.Engage(absTime, dt, outTorque, outAngularVelocity);
					LastShift = absTime;
				}
			} while (loop && ++count < 2);

			retVal.Gearbox.PowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			return retVal;
		}

		private void SetPowershiftLossEnergy(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (RequestAfterGearshift /*&& Gear != PreviousState.Gear*/) {
				LastShift = absTime;
				Gear = _strategy.Engage(absTime, dt, outTorque, outAngularVelocity);
				_powershiftLossEnergy = ComputeShiftLosses(outTorque, outAngularVelocity, Gear);
			} else {
				if (PreviousState.PowershiftLossEnergy != null && PreviousState.PowershiftLossEnergy.IsGreater(0)) {
					_powershiftLossEnergy = PreviousState.PowershiftLossEnergy;
				} else {
					_powershiftLossEnergy = null;
				}
			}
		}

		private IResponse RequestEngaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			if (!CurrentState.TorqueConverterLocked && !ModelData.Gears[Gear.Gear].HasTorqueConverter) {
				throw new VectoSimulationException(
					"Torque converter requested by strategy for gear without torque converter!");
			}

			var effectiveRatio = ModelData.Gears[Gear.Gear].Ratio;
			var effectiveLossMap = ModelData.Gears[Gear.Gear].LossMap;
			if (!CurrentState.TorqueConverterLocked) {
				effectiveRatio = ModelData.Gears[Gear.Gear].TorqueConverterRatio;
				effectiveLossMap = ModelData.Gears[Gear.Gear].TorqueConverterGearLossMap;
			}

			var inAngularVelocity = outAngularVelocity * effectiveRatio;
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var avgInAngularVelocity = (PreviousState.InAngularVelocity + inAngularVelocity) / 2.0;
			var inTorqueLossResult = effectiveLossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			var inTorque = outTorque * (avgOutAngularVelocity / avgInAngularVelocity) + inTorqueLossResult.Value;

			var inertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
				? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
				avgOutAngularVelocity
				: 0.SI<NewtonMeter>();
			inTorque += inertiaTorqueLossOut / effectiveRatio;
			var powershiftLoss = 0.SI<NewtonMeter>();
			var aliquotEnergyLoss = 0.SI<WattSecond>();
			if (_powershiftLossEnergy != null) {
				var remainingShiftLossLime = ModelData.PowershiftShiftTime - (absTime - LastShift);
				if (remainingShiftLossLime.IsGreater(0)) {
					aliquotEnergyLoss = _powershiftLossEnergy * VectoMath.Min(1.0, VectoMath.Min(dt, remainingShiftLossLime) / ModelData.PowershiftShiftTime);
					var avgEngineSpeed = (DataBus.EngineInfo.EngineSpeed + outAngularVelocity * effectiveRatio) / 2;
					powershiftLoss = aliquotEnergyLoss / dt / avgEngineSpeed;
					inTorque += powershiftLoss;

					//inTorque += CurrentState.PowershiftLossEnergy;
				} else {
					_powershiftLossEnergy = null;
				}
			}

			if (!dryRun) {
				CurrentState.InertiaTorqueLossOut = inertiaTorqueLossOut;
				CurrentState.TorqueLossResult = inTorqueLossResult;
				CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
				CurrentState.Gear = Gear;
				CurrentState.TransmissionTorqueLoss = inTorque * effectiveRatio - outTorque;
				CurrentState.PowershiftLoss = powershiftLoss;
				CurrentState.PowershiftLossEnergy = _powershiftLossEnergy;
				TorqueConverter.Locked(CurrentState.InTorque, CurrentState.InAngularVelocity, CurrentState.InTorque,
					CurrentState.InAngularVelocity);
			}

			if (!CurrentState.TorqueConverterLocked) {
				var response = TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity, dryRun);
				if (response is ResponseGearShift) {
					//RequestAfterGearshift = false;
				}
				return response;
			}
			var retVal = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, dryRun);
			if (!dryRun && retVal is ResponseSuccess &&
				_strategy.ShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, Gear,
					LastShift, retVal)) {
				retVal = new ResponseGearShift(this);
				//RequestAfterGearshift = false;
			}

			return retVal;
		}

		private IResponse RequestDisengaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			var avgAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			if (dryRun) {
				// if gearbox is disengaged the 0[W]-line is the limit for drag and full load.
				return new ResponseDryRun(this) {
					Gearbox = { PowerRequest = outTorque * avgAngularVelocity },
					DeltaDragLoad = outTorque * avgAngularVelocity,
					DeltaFullLoad = outTorque * avgAngularVelocity,
				};
			}
			if ((outTorque * avgAngularVelocity).IsGreater(0.SI<Watt>(),
				Constants.SimulationSettings.LineSearchTolerance)) {
				return new ResponseOverload(this) {
					Delta = outTorque * avgAngularVelocity,
					Gearbox = {PowerRequest = outTorque * avgAngularVelocity }
				};
			}

			if ((outTorque * avgAngularVelocity).IsSmaller(0.SI<Watt>(),
				Constants.SimulationSettings.LineSearchTolerance)) {
				return new ResponseUnderload(this) {
					Delta = outTorque * avgAngularVelocity,
					Gearbox = { PowerRequest = outTorque * avgAngularVelocity }
				};
			}

			Log.Debug("Invoking IdleController...");

			var retval = IdleController.Request(absTime, dt, 0.SI<NewtonMeter>(), null);
			retval.Clutch.PowerRequest = 0.SI<Watt>();

			// no dry-run - update state
			var effectiveRatio = ModelData.Gears[Gear.Gear].Ratio;
			if (!CurrentState.TorqueConverterLocked) {
				effectiveRatio = ModelData.Gears[Gear.Gear].TorqueConverterRatio;
			}
			CurrentState.SetState(0.SI<NewtonMeter>(), outAngularVelocity * effectiveRatio, outTorque,
				outAngularVelocity);
			CurrentState.Gear = ModelData.GearList.First();
			CurrentState.TorqueConverterLocked = !ModelData.Gears[Gear.Gear].HasTorqueConverter;
			CurrentState.TorqueLossResult = new TransmissionLossMap.LossMapResult() {
				Extrapolated = false,
				Value = 0.SI<NewtonMeter>()
			};
			TorqueConverter.Locked(DataBus.VehicleInfo.VehicleStopped ? 0.SI<NewtonMeter>() : CurrentState.InTorque, retval.Engine.EngineSpeed,
				CurrentState.InTorque,
				outAngularVelocity * effectiveRatio);


			return retval;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var avgInAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			var avgOutAngularSpeed = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;

			container[ModalResultField.Gear] =
				CurrentState.Disengaged || DataBus.VehicleInfo.VehicleStopped ||
				(DisengageGearbox && !ModelData.ATEcoRollReleaseLockupClutch)
					? 0
					: Gear.Gear;
			container[ModalResultField.TC_Locked] = DataBus.VehicleInfo.VehicleStopped
				? false
				: !(DisengageGearbox && ModelData.ATEcoRollReleaseLockupClutch) &&
				CurrentState.Gear.TorqueConverterLocked.Value;
			container[ModalResultField.P_gbx_loss] = CurrentState.InTorque * avgInAngularSpeed -
													CurrentState.OutTorque * avgOutAngularSpeed;
			container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgOutAngularSpeed;
			container[ModalResultField.P_gbx_in] = CurrentState.InTorque * avgInAngularSpeed;
			container[ModalResultField.P_gbx_shift_loss] = CurrentState.PowershiftLoss.DefaultIfNull(0) * avgInAngularSpeed;
			container[ModalResultField.n_gbx_out_avg] = avgOutAngularSpeed;
			container[ModalResultField.T_gbx_out] = CurrentState.OutTorque;

			_strategy?.WriteModalResults(container);
		}

		protected override void DoCommitSimulationStep()
		{
			if (!CurrentState.Disengaged && CurrentState.TorqueLossResult != null &&
				CurrentState.TorqueLossResult.Extrapolated) {
				Log.Warn(
					"Gear {0} LossMap data was extrapolated: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
					Gear, CurrentState.OutAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.OutTorque,
					ModelData.Gears[Gear.Gear].Ratio);
				if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
					throw new VectoException(
						"Gear {0} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
						Gear, CurrentState.InAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.InTorque,
						ModelData.Gears[Gear.Gear].Ratio);
				}
			}
			RequestAfterGearshift = false;

			if (DataBus.VehicleInfo.VehicleStopped) {
				CurrentState.Disengaged = true;
			}

			AdvanceState();

			CurrentState.TorqueConverterLocked = PreviousState.TorqueConverterLocked;
			CurrentState.Disengaged = PreviousState.Disengaged;
		}

		public class ATGearboxState : GearboxState
		{
			public bool TorqueConverterLocked;
			public bool Disengaged = true;
			public WattSecond PowershiftLossEnergy;
			public NewtonMeter PowershiftLoss;
		}
	}
}