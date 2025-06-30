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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
    /// <summary>
    /// AMTShiftStrategy implements the AMT Shifting Behaviour.
    /// </summary>
    [Obsolete("no longer maintained - use AMTShiftStrategyOptimized")]
	public class AMTShiftStrategy : BaseShiftStrategy
	{
		public const string Name = "AMT - Classic";

        //protected readonly GearshiftPosition MaxStartGear;
        protected GearshiftPosition DesiredGearRoadsweeping;
		
		protected ITestPowertrain TestPowertrain;

		protected IAMTGearbox _gearbox;

        public AMTShiftStrategy(IVehicleContainer container) : base(container)
		{
			var runData = container.RunData;
			
			if (runData.EngineData == null) {
				return;
			}

			var transmissionRatio = runData.AxleGearData.AxleGear.Ratio *
									(runData.AngledriveData?.Angledrive.Ratio ?? 1.0) /
									runData.VehicleData.DynamicTyreRadius;
			var minEngineSpeed = (runData.EngineData.FullLoadCurves[0].RatedSpeed - runData.EngineData.IdleSpeed) *
								Constants.SimulationSettings.ClutchClosingSpeedNorm + runData.EngineData.IdleSpeed;

			DesiredGearRoadsweeping = runData.DriverData?.PTODriveRoadsweepingGear;

			MaxStartGear = GearboxModelData.GearList.First();
			foreach (var gear in GearboxModelData.GearList.Reverse()) {
				var gearData = GearboxModelData.Gears[gear.Gear];
				if (GearshiftParams.StartSpeed * transmissionRatio * gearData.Ratio > minEngineSpeed) {
					MaxStartGear = gear;
					break;
				}
			}

			// create testcontainer
			TestPowertrain = PowertrainBuilder.CreateTestPowertrain(Container, true);
		}

		public override IGearbox Gearbox {
			get => _gearbox;
			set {
				if (value is IAMTGearbox gbx) {
					_gearbox = gbx;
					return;
				}
				throw new VectoException("This shift strategy can't handle gearbox of type {0}, expected {1}", value.GetType().Name, nameof(IAMTGearbox));
			}
		}

        public override GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear, outAngularVelocity)) {
				_nextGear = Gears.Predecessor(_nextGear);
			}
			while (Gears.HasSuccessor(_nextGear) && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
				_nextGear = Gears.Successor(_nextGear);
			}

			return _nextGear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) {}

		public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (Container.VehicleInfo.VehicleSpeed.IsEqual(0)) {
				return InitStartGear(absTime, outTorque, outAngularVelocity);
			}

			foreach (var gear in Gears.Reverse()) {
				var selected = gear;
				//var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

				TestPowertrain.UpdateComponents();
				TestPowertrain.Gearbox.SetGear = gear;
				TestPowertrain.Gearbox.SetNextGear = gear;
				
				var response = TestPowertrain.Gearbox.Initialize(outTorque, outAngularVelocity);
				response = TestPowertrain.Gearbox.Request(absTime,
					Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval, outTorque, outAngularVelocity,
					true);

				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
				var fullLoadPower = TestPowertrain.CombustionEngine.EngineStationaryFullPower(response.Engine.EngineSpeed);
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
				var inTorque = response.Clutch.PowerRequest / inAngularSpeed;

				// if in shift curve and torque reserve is provided: return the current gear
				if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
					reserve >= GearshiftParams.StartTorqueReserve) {
					if ((inAngularSpeed - Container.EngineInfo.EngineIdleSpeed) / (Container.EngineInfo.EngineRatedSpeed - Container.EngineInfo.EngineIdleSpeed) <
						Constants.SimulationSettings.ClutchClosingSpeedNorm && Gears.HasPredecessor(gear)) {
						selected = Gears.Predecessor(gear);
					}
					_nextGear = selected;
					return selected;
				}

				// if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
				if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && Gears.HasSuccessor(gear)) {
					selected = Gears.Successor(gear);
					_nextGear = selected;
					return selected;
				}
			}

			// fallback: return first gear
			_nextGear = Gears.First();
			return _nextGear;
		}

		private GearshiftPosition InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			foreach (var gear in Gears.IterateGears(MaxStartGear, Gears.First())) {
				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

				var ratedSpeed = Container.EngineInfo.EngineRatedSpeed;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
					continue;
				}

				//var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);
				TestPowertrain.UpdateComponents();
				TestPowertrain.Gearbox.SetGear = gear;
				TestPowertrain.Gearbox.SetNextGear = gear;

				var response = TestPowertrain.Gearbox.Initialize(outTorque, outAngularVelocity);
				response = TestPowertrain.Gearbox.Request(absTime,
					Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval, outTorque, outAngularVelocity,
					true);

				var reserve = 1 - response.Engine.TotalTorqueDemand / response.Engine.DynamicFullLoadTorque; //response.Engine.PowerRequest/response.Engine.DynamicFullLoadPower does not contain auxiliary power

				if (response.Engine.EngineSpeed > Container.EngineInfo.EngineIdleSpeed && reserve >= GearshiftParams.StartTorqueReserve) {
					_nextGear = gear;
					return gear;
				}
			}
			_nextGear = Gears.First();
			return _nextGear;
		}

		protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response)
		{
			// no shift when vehicle stands
			if (Container.VehicleInfo.VehicleStopped) {
				return false;
			}

			// emergency shift to not stall the engine ------------------------
			if (Gears.First().Equals(gear) &&
				SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				return true;
			}

			_nextGear = gear;
			while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear,
				inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = Gears.Predecessor(_nextGear);
			}

			while (Gears.HasSuccessor(_nextGear) &&
					SpeedTooHighForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = Gears.Successor(_nextGear);
			}

			if (!_nextGear.Equals(gear)) {
				return true;
			}

			// PTO Active while drive (roadsweeping) shift rules
			if (Container.DrivingCycleInfo.CycleData.LeftSample.PTOActive == PTOActivity.PTOActivityRoadSweeping) {
				if (gear.Equals(DesiredGearRoadsweeping)) {
					return false;
				}

				if (gear > DesiredGearRoadsweeping) {
					if (IsAboveDownShiftCurve(DesiredGearRoadsweeping, inTorque, inAngularVelocity)) {
						_nextGear = DesiredGearRoadsweeping;
						return true;
					}
				}

				if (gear < DesiredGearRoadsweeping) {
					if (!SpeedTooHighForEngine(
						DesiredGearRoadsweeping, inAngularVelocity / GearboxModelData.Gears[DesiredGearRoadsweeping.Gear].Ratio)) {
						_nextGear = DesiredGearRoadsweeping;
						return true;
					}
				}
			}

			// normal shift when all requirements are fullfilled ------------------
			var minimumShiftTimePassed =
				(lastShiftTime + GearshiftParams.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return false;
			}

			_nextGear = CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				response);
			if (!_nextGear.Equals(gear)) {
				return true;
			}

			_nextGear = CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				response);

			return !_nextGear.Equals(gear);
		}

		protected virtual GearshiftPosition CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (Container.DriverInfo.DriverBehavior != DrivingBehavior.Accelerating && Container.DriverInfo.DriverBehavior != DrivingBehavior.Driving) {
				return currentGear;
			}
			if ((absTime - _gearbox.LastDownshift).IsSmaller(GearshiftParams.UpshiftAfterDownshiftDelay)) {
				return currentGear;
			}
			var nextGear = DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
			if (nextGear.Equals(currentGear)) {
				return nextGear;
			}

			// estimate acceleration for selected gear
			if (EstimateAccelerationForGear(nextGear, outAngularVelocity).IsSmaller(GearshiftParams.UpshiftMinAcceleration)) {
				// if less than 0.1 for next gear, don't shift
				if (nextGear.Gear - currentGear.Gear == 1) {
					return currentGear;
				}
				// if a gear is skipped but acceleration is less than 0.1, try for next gear. if acceleration is still below 0.1 don't shift!
				if (nextGear.Gear > currentGear.Gear &&
					EstimateAccelerationForGear(Gears.Successor(currentGear), outAngularVelocity)
						.IsSmaller(GearshiftParams.UpshiftMinAcceleration)) {
					return currentGear;
				}
				nextGear = Gears.Successor(currentGear);
			}

			return nextGear;
		}

		protected virtual GearshiftPosition CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			if ((absTime - _gearbox.LastUpshift).IsSmaller(GearshiftParams.DownshiftAfterUpshiftDelay)) {
				return currentGear;
			}
			return DoCheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
		}

		protected virtual GearshiftPosition DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear = Gears.Successor(currentGear);

				while (Gears.HasSuccessor(currentGear)) {
					currentGear = Gears.Successor(currentGear);
					var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					inAngularVelocity = response.Engine.EngineSpeed; //ModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.Clutch.PowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(
						response.Engine.DynamicFullLoadPower /
						((Container.EngineInfo.EngineSpeed + response.Engine.EngineSpeed) / 2),
						currentGear.Equals(Gears.First())
							? double.MaxValue.SI<NewtonMeter>()
							: GearboxModelData.Gears[currentGear.Gear].ShiftPolygon
								.InterpolateDownshift(response.Engine.EngineSpeed));
					var reserve = 1 - inTorque.Value() / maxTorque.Value();

					if (reserve >= 0 /*ModelData.TorqueReserve */ && IsAboveDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}

					currentGear = Gears.Predecessor(currentGear);
					break;
				}
			}

			// early up shift to higher gear ---------------------------------------
			if (Gears.HasSuccessor(currentGear)) {
				currentGear = CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
			}
			return currentGear;
		}

		protected virtual GearshiftPosition CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// try if next gear would provide enough torque reserve
			var tryNextGear = Gears.Successor(currentGear);
			var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

			var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
			var inTorque = response.Clutch.PowerRequest / inAngularVelocity;

			// if next gear supplied enough power reserve: take it
			// otherwise take
			if (!IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
				var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

				if (reserve >= GearshiftParams.TorqueReserve) {
					currentGear = tryNextGear;
				}
			}
			return currentGear;
		}

		
		protected virtual GearshiftPosition DoCheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// down shift
			if (IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear = Gears.Predecessor(currentGear);
			}
			return currentGear;
		}

		protected virtual ResponseDryRun RequestDryRunWithGear(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition tryNextGear)
		{
			// var tmpGear = Gearbox.Gear;
			// _gearbox.Gear = tryNextGear;
			// var response = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
			// _gearbox.Gear = tmpGear;
			
			
			
			LogEnabled = false;
			TestPowertrain.UpdateComponents();
			TestPowertrain.Gearbox.SetDisengaged = false;
			TestPowertrain.Gearbox.SetGear = tryNextGear;

			TestPowertrain.Container.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			var response = (ResponseDryRun)TestPowertrain.Container.GearboxOutPort.Request(
				0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			LogEnabled = true;
			return response;
		}
	}
}