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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATShiftStrategy : BaseShiftStrategy
	{
		protected ATGearbox _gearbox;
		protected readonly NextGearState _nextGear = new NextGearState();
		private KilogramSquareMeter EngineInertia;

		public override IGearbox Gearbox
		{
			get => _gearbox;
			set {
				_gearbox = value as ATGearbox;
				if (_gearbox == null) {
					throw new VectoException("AT Shift strategy can only handle AT gearboxes, given: {0}", value.GetType());
				}
			}
		}

		public override GearshiftPosition NextGear => _nextGear.Gear;

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
		{
			return DeclarationData.TorqueConverter.ComputeShiftPolygon(
				engineDataFullLoadCurve, i == 0, i >= gearboxGears.Count - 1);
		}

		public static string Name => "AT - Classic";

		public ATShiftStrategy(IVehicleContainer dataBus) : base(dataBus)
		{
			EngineInertia = dataBus.RunData.EngineData?.Inertia ?? 0.SI<KilogramSquareMeter>();

			if (Gears.Any(x => !x.TorqueConverterLocked.HasValue)) {
				throw new VectoException("Gear list must have TC info for all gears! {0}", Gears.Join());
			}

			MaxStartGear = Gears.Any() ? Gears.First() : new GearshiftPosition(0);
		}

		public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
				// AT always starts in first gear and TC active!
				_gearbox.Disengaged = true;
				return Gears.First();
			}

			foreach (var gear in Gears.Reverse()) {
				var response = _gearbox.Initialize(gear, torque, outAngularVelocity);

				if (response.Engine.EngineSpeed > DataBus.EngineInfo.EngineRatedSpeed || response.Engine.EngineSpeed < DataBus.EngineInfo.EngineIdleSpeed) {
					continue;
				}

				if (!IsBelowDownShiftCurve(gear, response.Engine.PowerRequest / response.Engine.EngineSpeed, response.Engine.EngineSpeed)) {
					_gearbox.Disengaged = false;
					return gear;
				}
			}

			// fallback: start with first gear;
			_gearbox.Disengaged = false;
			return Gears.First();
		}

		public override GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (_nextGear.AbsTime != null && _nextGear.AbsTime.IsEqual(absTime)) {
				//_gearbox.Gear = _nextGear.Gear;
				_gearbox.Disengaged = _nextGear.Disengaged;
				_nextGear.AbsTime = null;
				return _nextGear.Gear;
			}

			_nextGear.AbsTime = null;
			return _gearbox.Gear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			throw new System.NotImplementedException("AT Shift Strategy does not support disengaging.");
		}

		protected override bool DoCheckShiftRequired(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			// ENGAGE ---------------------------------------------------------
			// 0 -> 1C: drive off after disengaged - engage first gear
			if (_gearbox.Disengaged && outAngularVelocity.IsGreater(0.SI<PerSecond>())) {
				Log.Debug("shift required: drive off after vehicle stopped");
				_nextGear.SetState(absTime, disengaged: false, gear: Gears.First());
				return true;
			}

			// DISENGAGE ------------------------------------------------------
			// 1) _ -> 0: disengage before halting
			var braking = DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking;
			var torqueNegative = outTorque.IsSmaller(0);
			var slowerThanDisengageSpeed =
				DataBus.VehicleInfo.VehicleSpeed.IsSmaller(GearboxModelData.DisengageWhenHaltingSpeed);
			var disengageBeforeHalting = braking && torqueNegative && slowerThanDisengageSpeed;

			// 2) L -> 0: disengage if inAngularVelocity == 0
			var disengageAngularVelocityZero = _gearbox.TorqueConverterLocked && inAngularVelocity.IsEqual(0.SI<PerSecond>());

			// 3) 1C -> 0: disengange when negative T_out and positive T_in
			var gear1C = Gears.First().Equals(gear);
			var disengageTOutNegativeAndTInPositive = DataBus.DriverInfo.DriverAcceleration <= 0 && gear1C && outTorque.IsSmaller(0) &&
													inTorque.IsGreater(0);

			var disengageTCEngineSpeedLowerIdle = braking && torqueNegative && gear1C &&
												inAngularVelocity.IsSmallerOrEqual(DataBus.EngineInfo.EngineIdleSpeed);

			if (disengageBeforeHalting || disengageTCEngineSpeedLowerIdle || disengageAngularVelocityZero ||
				disengageTOutNegativeAndTInPositive) {
				_nextGear.SetState(absTime, disengaged: true, gear: Gears.First());
				return true;
			}

			// EMERGENCY SHIFTS ---------------------------------------
			if (CheckEmergencyShift(absTime, outTorque, outAngularVelocity, inAngularVelocity, gear)) {
				return true;
			}

			// UPSHIFT --------------------------------------------------------
			if (CheckUpshift(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				lastShiftTime, response)) {
				return true;
			}

			// DOWNSHIFT ------------------------------------------------------
			if (CheckDownshift(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				lastShiftTime, response)) {
				return true;
			}

			return false;
		}

		protected virtual bool CheckEmergencyShift(
			Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity, PerSecond inAngularVelocity, GearshiftPosition gear)
		{
			// Emergency Downshift: if lower than engine idle speed
			if (inAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineIdleSpeed) && Gears.HasPredecessor(gear)) {
				Log.Debug("engine speed would fall below idle speed - shift down");
				Downshift(absTime, gear);
				return true;
			}

			// Emergency Upshift: if higher than engine rated speed
			if (inAngularVelocity.IsGreaterOrEqual(VectoMath.Min(GearboxModelData.Gears[gear.Gear].MaxSpeed, DataBus.EngineInfo.EngineN95hSpeed))) {
				// check if upshift is possible
				if (!Gears.HasSuccessor(gear)) {
					return false;
				}

				PerSecond nextInAngularSpeed;
				NewtonMeter nextInTorque;
				if (GearboxModelData.Gears[gear.Gear].HasLockedGear) {
					nextInAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
					nextInTorque = outTorque / GearboxModelData.Gears[gear.Gear].Ratio;
				} else {
					nextInAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear + 1].Ratio;
					nextInTorque = outTorque / GearboxModelData.Gears[gear.Gear + 1].Ratio;
				}

				var nextGear = Gears.Successor(gear);
				var acc = EstimateAccelerationForGear(nextGear, outAngularVelocity);
				if ((acc > 0 || _gearbox.TCLocked) && !IsBelowDownShiftCurve(nextGear, nextInTorque, nextInAngularSpeed)) {
					Log.Debug("engine speed would be above max speed / rated speed - shift up");
					Upshift(absTime, gear);
					return true;
				}
			}

			return false;
		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		protected virtual bool CheckUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(GearshiftParams.TimeBetweenGearshifts);
			if (!shiftTimeReached) {
				return false;
			}

			var currentGear = GearboxModelData.Gears[gear.Gear];

			if (_gearbox.TorqueConverterLocked || currentGear.HasLockedGear) {
				var result = CheckUpshiftToLocked(absTime, outAngularVelocity, inTorque, inAngularVelocity, gear);
				if (result.HasValue) {
					return result.Value;
				}
			}

			// UPSHIFT - Special rule for 1C -> 2C
			var nextGear = Gears.Successor(gear);
			if (!gear.TorqueConverterLocked.Value && nextGear != null && !nextGear.TorqueConverterLocked.Value && outAngularVelocity.IsGreater(0)) {
				var result = CheckUpshiftTcTc(
					absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, currentGear, response);
				if (result.HasValue) {
					return result.Value;
				}
			}

			if (Gears.HasSuccessor(gear)) {
				var earlyUpshift = CheckEarlyUpshift(
					absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, response);
				if (earlyUpshift.HasValue) {
					return earlyUpshift.Value;
				}
			}

			return false;
		}

		protected virtual bool? CheckEarlyUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			return null;
		}

		protected virtual bool? CheckUpshiftTcTc(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, GearData currentGear, IResponse response)
		{
			// C -> C+1
			var nextGearPos = Gears.Successor(gear); // GearboxModelData.Gears[gear + 1];
			var nextGear = GearboxModelData.Gears[nextGearPos.Gear];
			var gearRatio = nextGear.TorqueConverterRatio / currentGear.TorqueConverterRatio;
			var minEngineSpeed = VectoMath.Min(700.RPMtoRad(), gearRatio * (DataBus.EngineInfo.EngineN80hSpeed - 150.RPMtoRad()));

			var nextGearboxInSpeed = outAngularVelocity * nextGear.TorqueConverterRatio;
			var nextGearboxInTorque = outTorque / nextGear.TorqueConverterRatio;
			var shiftLosses = _gearbox.ComputeShiftLosses(outTorque, outAngularVelocity, nextGearPos) /
							GearboxModelData.PowershiftShiftTime / nextGearboxInSpeed;
			nextGearboxInTorque += shiftLosses;
			var tcOperatingPoint =
				_gearbox.TorqueConverter.FindOperatingPoint(absTime, dt, nextGearboxInTorque, nextGearboxInSpeed);

			var engineSpeedOverMin = tcOperatingPoint.InAngularVelocity.IsGreater(minEngineSpeed);
			var avgSpeed = (DataBus.EngineInfo.EngineSpeed + tcOperatingPoint.InAngularVelocity) / 2;
			var engineMaxTorque = DataBus.EngineInfo.EngineStationaryFullPower(avgSpeed) / avgSpeed;
			var engineInertiaTorque = Formulas.InertiaPower(
										DataBus.EngineInfo.EngineSpeed, tcOperatingPoint.InAngularVelocity, _gearbox.EngineInertia, dt) / avgSpeed;
			var engineTorqueBelowMax =
				tcOperatingPoint.InTorque.IsSmallerOrEqual(engineMaxTorque - engineInertiaTorque);

			var reachableAcceleration =
				EstimateAcceleration(
					outAngularVelocity, outTorque, inAngularVelocity, inTorque, gear.Gear, response); // EstimateAccelerationForGear(gear + 1, outAngularVelocity);
			var minAcceleration = VectoMath.Min(
				GearboxModelData.TorqueConverterData.CCUpshiftMinAcceleration,
				DataBus.DriverInfo.DriverAcceleration);
			var minAccelerationReachable = reachableAcceleration.IsGreaterOrEqual(minAcceleration);

			if (engineSpeedOverMin && engineTorqueBelowMax && minAccelerationReachable) {
				Upshift(absTime, gear);
				return true;
			}

			return null;
		}

		protected virtual bool? CheckUpshiftToLocked(
			Second absTime, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear)
		{
			// UPSHIFT - General Rule
			// L -> L+1 
			// C -> L
			var nextGear = Gears.Successor(gear); // _gearbox.TorqueConverterLocked ? gear + 1 : gear;
			if (nextGear == null || !GearboxModelData.Gears.ContainsKey(nextGear.Gear)) {
				return false;
			}

			var nextEngineSpeed = outAngularVelocity * GearboxModelData.Gears[nextGear.Gear].Ratio;
			if (nextEngineSpeed.IsEqual(0)) {
				return false;
			}

			var currentEnginePower = inTorque * inAngularVelocity;
			var nextEngineTorque = currentEnginePower / nextEngineSpeed;
			var isAboveUpShift = IsAboveUpShiftCurve(gear, nextEngineTorque, nextEngineSpeed, _gearbox.TorqueConverterLocked);

			var minAccelerationReachable = true;
			if (DataBus.DriverInfo.DriverAcceleration.IsSmaller(0)) {
				return null;
			}
			if (!DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
				var reachableAcceleration = EstimateAccelerationForGear(nextGear, outAngularVelocity);
				var minAcceleration = _gearbox.TorqueConverterLocked
					? GearshiftParams.UpshiftMinAcceleration
					: GearboxModelData.TorqueConverterData.CLUpshiftMinAcceleration;
				minAcceleration = VectoMath.Min(
					minAcceleration, VectoMath.Max(0.SI<MeterPerSquareSecond>(), DataBus.DriverInfo.DriverAcceleration));
				minAccelerationReachable = reachableAcceleration.IsGreaterOrEqual(minAcceleration);
			}

			if (isAboveUpShift && minAccelerationReachable) {
				Upshift(absTime, gear);
				return true;
			}

			return null;
		}

		/// <summary>
		/// Tests if the operating point is above (right of) the up-shift curve.
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <param name="torqueConverterLocked">if true, the regular shift polygon is used, otherwise the shift polygon for the torque converter is used</param>
		/// <returns><c>true</c> if the operating point is above the up-shift curve; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAboveUpShiftCurve(
			GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed,
			bool torqueConverterLocked)
		{
			var shiftPolygon = torqueConverterLocked
				? GearboxModelData.Gears[gear.Gear].ShiftPolygon
				: GearboxModelData.Gears[gear.Gear].TorqueConverterShiftPolygon;

			return Gears.HasSuccessor(gear) && shiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
		}

		protected virtual void Upshift(Second absTime, GearshiftPosition gear)
		{
			if (!Gears.HasSuccessor(gear)) {
				throw new VectoSimulationException(
					"ShiftStrategy wanted to shift up but no higher gear available.");
			}

			_nextGear.SetState(absTime, false, Gears.Successor(gear));

		}

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		protected virtual bool CheckDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(GearshiftParams.TimeBetweenGearshifts);

			if (shiftTimeReached && IsBelowDownShiftCurve(gear, inTorque, inAngularVelocity)) {
				Downshift(absTime, gear);
				return true;
			}

			if (shiftTimeReached && DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate) {
				if (DataBus.VehicleInfo.VehicleSpeed < DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed - 10.KMPHtoMeterPerSecond() &&
					DataBus.DriverInfo.DriverAcceleration < 0.SI<MeterPerSquareSecond>()) {
					var tmpResponseCurr = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
					if (_gearbox.Gear > Gears.First()) {
						// clone current state of _nextgear, set gearbox state to lower gear, issue request, restore old gearbox state
						var tmp = _nextGear.Clone();
						var gbxState = new NextGearState(absTime, _gearbox);
						tmp.Gear = Gears.Predecessor(_gearbox.Gear);
						SetGear(tmp);
						var tmpResponseDs = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
						SetGear(gbxState);
						// done
						if (tmpResponseDs.DeltaFullLoad - Formulas.InertiaPower(
								tmpResponseDs.Engine.EngineSpeed, DataBus.EngineInfo.EngineSpeed, EngineInertia, dt) < tmpResponseCurr.DeltaFullLoad) {
							Downshift(absTime, gear);
							return true;
						}
					}
				}
			}

			if (shiftTimeReached && gear > Gears.First()) {
				var earlyDownshift = CheckEarlyDownshift(
					absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, response);
				if (earlyDownshift.HasValue) {
					return earlyDownshift.Value;
				}
			}

			return false;
		}

		protected virtual bool? CheckEarlyDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			return null;
		}

		protected virtual void SetGear(NextGearState gbxState)
		{
			_gearbox.Gear = gbxState.Gear;
			//_gearbox.TorqueConverterLocked = gbxState.TorqueConverterLocked;
			_gearbox.Disengaged = gbxState.Disengaged;
		}

		/// <summary>
		/// Tests if the operating point is below (left of) the down-shift curve.
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the down-shift curv; otherwise, <c>false</c>.</returns>
		protected virtual bool IsBelowDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			return !Gears.First().Equals(gear) && GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
		}

		protected virtual void Downshift(Second absTime, GearshiftPosition gear)
		{
			if (!Gears.HasPredecessor(gear)) {
				throw new VectoSimulationException(
					"ShiftStrategy wanted to shift down but no lower gear available.");
			}
			_nextGear.SetState(absTime, false, Gears.Predecessor(gear));
		}

		protected MeterPerSquareSecond EstimateAcceleration(
			PerSecond gbxOutSpeed, NewtonMeter gbxOutTorque, PerSecond tcInSpeed, NewtonMeter tcInTorque, uint currentGear, IResponse response)
		{
			var vehicleSpeed = DataBus.VehicleInfo.VehicleSpeed;
			var avgSlope =
			((DataBus.DrivingCycleInfo.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
			DataBus.DrivingCycleInfo.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleInfo.VehicleSpeed;
			var rollResistanceLoss = DataBus.VehicleInfo.RollingResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;

			var slopeLoss = DataBus.VehicleInfo.SlopeResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var axleLoss = DataBus.AxlegearInfo.AxlegearLoss();

			var tcLossesCurrentGear = tcInSpeed * tcInTorque - gbxOutSpeed * gbxOutTorque;

			var nextTcOutSpeed = gbxOutSpeed * GearboxModelData.Gears[currentGear + 1].TorqueConverterRatio;
			
			var tcNext = GearboxModelData.TorqueConverterData.LookupOperatingPointOut(
				nextTcOutSpeed, response.Engine.EngineSpeed, response.Engine.TorqueOutDemand);
			var tcLossesNextGear = tcNext.InAngularVelocity * tcNext.InTorque - tcNext.OutAngularVelocity * tcNext.OutTorque;
			var deltaTcLosses = tcLossesNextGear - tcLossesCurrentGear;

			var accelerationPower = gbxOutSpeed * gbxOutTorque - deltaTcLosses - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;

			var acceleration = accelerationPower / DataBus.VehicleInfo.VehicleSpeed / (DataBus.VehicleInfo.TotalMass + DataBus.WheelsInfo.ReducedMassWheels);

			return acceleration.Cast<MeterPerSquareSecond>();
		}

		protected class NextGearState
		{
			public Second AbsTime { get; internal set; }
			public bool Disengaged { get; private set; }
			public GearshiftPosition Gear { get; internal set; }

			public NextGearState()
			{
				Gear = new GearshiftPosition(0);
			}

			private NextGearState(NextGearState nextGearState)
			{
				AbsTime = nextGearState.AbsTime;
				Disengaged = nextGearState.Disengaged;
				Gear = nextGearState.Gear;
			}

			public NextGearState(Second absTime, ATGearbox gearbox)
			{
				SetState(absTime, gearbox);
			}

			public void SetState(Second absTime, bool disengaged, GearshiftPosition gear)
			{
				AbsTime = absTime;
				Disengaged = disengaged;
				Gear = gear;
			}
			
			public void SetState(Second absTime, ATGearbox gearbox)
			{
				AbsTime = absTime;
				Disengaged = gearbox.Disengaged;
				Gear = gearbox.Gear;
			}

			public NextGearState Clone()
			{
				return new NextGearState(this);
			}
		}
	}
}
