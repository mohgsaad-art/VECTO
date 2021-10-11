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
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// AMTShiftStrategy implements the AMT Shifting Behaviour.
	/// </summary>
	public class AMTShiftStrategy : ShiftStrategy
	{
		//protected readonly GearshiftPosition MaxStartGear;
		protected GearshiftPosition _nextGear;
		private GearshiftPosition DesiredGearRoadsweeping;

		public AMTShiftStrategy(IVehicleContainer dataBus) : base(dataBus)
		{
			var runData = dataBus.RunData;
			EarlyShiftUp = true;
			SkipGears = true;

			var transmissionRatio = runData.AxleGearData.AxleGear.Ratio *
									(runData.AngledriveData == null ? 1.0 : runData.AngledriveData.Angledrive.Ratio) /
									runData.VehicleData.DynamicTyreRadius;
			var minEngineSpeed = (runData.EngineData.FullLoadCurves[0].RatedSpeed - runData.EngineData.IdleSpeed) *
								Constants.SimulationSettings.ClutchClosingSpeedNorm + runData.EngineData.IdleSpeed;

			DesiredGearRoadsweeping = runData.DriverData.PTODriveRoadsweepingGear;

			MaxStartGear = GearboxModelData.GearList.First();
			foreach (var gear in GearboxModelData.GearList.Reverse()) {
				var gearData = GearboxModelData.Gears[gear.Gear];
				if (GearboxModelData.StartSpeed * transmissionRatio * gearData.Ratio > minEngineSpeed) {
					MaxStartGear = gear;
					break;
				}
			}
		}

		private bool SpeedTooLowForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
		{
			return (outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsSmaller(DataBus.EngineIdleSpeed);
		}

		private bool SpeedTooHighForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsGreaterOrEqual(VectoMath.Min(GearboxModelData.Gears[gear.Gear].MaxSpeed,
																				DataBus.EngineN95hSpeed));
		}

		public override GearshiftPosition NextGear
		{
			get { return _nextGear; }
		}

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, IList<ITransmissionInputData> gearboxGears,
			CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius)
		{
			return DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(
				i, engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);
		}


		public static string Name => "AMT - Classic";

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

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed) {}

		public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleSpeed.IsEqual(0)) {
				return InitStartGear(outTorque, outAngularVelocity);
			}

			foreach (var gear in Gears.Reverse()) {
				var selected = gear;
				//for (var gear = (uint)GearboxModelData.Gears.Count; gear > 1; gear--) {
				var response = _gearbox.Initialize(gear, outTorque, outAngularVelocity);

				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
				var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;
				var inTorque = response.ClutchPowerRequest / inAngularSpeed;

				// if in shift curve and torque reserve is provided: return the current gear
				if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
					reserve >= GearboxModelData.StartTorqueReserve) {
					if ((inAngularSpeed - DataBus.EngineIdleSpeed) / (DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
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

		private GearshiftPosition InitStartGear(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			foreach (var gear in Gears.IterateGears(MaxStartGear, Gears.First())) {
				//for (var gear = MaxStartGear; gear > 1; gear--) {
				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

				var ratedSpeed = DataBus.EngineRatedSpeed;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
					continue;
				}

				var response = _gearbox.Initialize(gear, outTorque, outAngularVelocity);

				var fullLoadPower = response.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

				if (response.EngineSpeed > DataBus.EngineIdleSpeed && reserve >= GearboxModelData.StartTorqueReserve) {
					_nextGear = gear;
					return gear;
				}
			}
			_nextGear = Gears.First();
			return _nextGear;
		}

		protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			// no shift when vehicle stands
			if (DataBus.VehicleStopped) {
				return false;
			}

			// emergency shift to not stall the engine ------------------------
			if (Gears.First().Equals(gear) && SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				return true;
			}
			_nextGear = gear;
			while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = Gears.Predecessor(_nextGear);
			}
			while (Gears.HasSuccessor(_nextGear) && SpeedTooHighForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = Gears.Successor(_nextGear);
			}
			if (!_nextGear.Equals(gear)) {
				return true;
			}

			// PTO Active while drive (roadsweeping) shift rules
			if (DataBus.CycleData.LeftSample.PTOActive == PTOActivity.PTOActivityRoadSweeping) {
				if (gear == DesiredGearRoadsweeping) {
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
			var minimumShiftTimePassed = (lastShiftTime + GearboxModelData.ShiftTime).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return false;
			}

			_nextGear = CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, response);
			if (_nextGear != gear) {
				return true;
			}

			_nextGear = CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, response);

			//if ((GearboxModelData.Gears[_nextGear].Ratio * outAngularVelocity - DataBus.EngineIdleSpeed) /
			//	(DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
			//	Constants.SimulationSettings.ClutchClosingSpeedNorm && _nextGear > 1) {
			//	_nextGear--;
			//}

			return _nextGear != gear;
		}

		protected virtual GearshiftPosition CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Accelerating && DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Driving) {
				return currentGear;
			}
			if ((absTime - _gearbox.LastDownshift).IsSmaller(_gearbox.ModelData.UpshiftAfterDownshiftDelay)) {
				return currentGear;
			}
			var nextGear = DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
			if (nextGear.Equals(currentGear)) {
				return nextGear;
			}

			// estimate acceleration for selected gear
			if (EstimateAccelerationForGear(nextGear, outAngularVelocity).IsSmaller(_gearbox.ModelData.UpshiftMinAcceleration)) {
				// if less than 0.1 for next gear, don't shift
				if (nextGear.Gear - currentGear.Gear == 1) {
					return currentGear;
				}
				// if a gear is skipped but acceleration is less than 0.1, try for next gear. if acceleration is still below 0.1 don't shift!
				if (nextGear > currentGear &&
					EstimateAccelerationForGear(Gears.Successor(currentGear), outAngularVelocity)
						.IsSmaller(_gearbox.ModelData.UpshiftMinAcceleration)) {
					return currentGear;
				}
				nextGear = Gears.Successor(currentGear);
			}

			return nextGear;
		}

		protected virtual GearshiftPosition CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			if ((absTime - _gearbox.LastUpshift).IsSmaller(_gearbox.ModelData.DownshiftAfterUpshiftDelay)) {
				return currentGear;
			}
			return DoCheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
		}

		protected virtual GearshiftPosition DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear = Gears.Successor(currentGear);

				while (SkipGears && Gears.HasSuccessor(currentGear)) {
					currentGear = Gears.Successor(currentGear);
					var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					inAngularVelocity = response.EngineSpeed; //GearboxModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.ClutchPowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(response.DynamicFullLoadPower / ((DataBus.EngineSpeed + response.EngineSpeed) / 2),
						currentGear.Equals(Gears.First())
							? GearboxModelData.Gears[currentGear.Gear].ShiftPolygon.InterpolateDownshift(response.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = 1 - inTorque / maxTorque;

					if (reserve >= GearboxModelData.TorqueReserve && IsAboveDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}

					currentGear = Gears.Predecessor(currentGear);
					break;
				}
			}

			// early up shift to higher gear ---------------------------------------
			if (EarlyShiftUp && Gears.HasSuccessor(currentGear)) {
				currentGear = CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
			}
			return currentGear;
		}

		protected virtual GearshiftPosition CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			GearshiftPosition currentGear, IResponse response1)
		{
			NewtonMeter inTorque;
			PerSecond inAngularVelocity;
			// try if next gear would provide enough torque reserve
			var tryNextGear = Gears.Successor(currentGear);
			var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

			inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
			inTorque = response.ClutchPowerRequest / inAngularVelocity;

			// if next gear supplied enough power reserve: take it
			// otherwise take
			if (!IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
				var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

				if (reserve >= GearboxModelData.TorqueReserve) {
					currentGear = tryNextGear;
				}
			}

			return currentGear;
		}

		protected virtual ResponseDryRun RequestDryRunWithGear(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GearshiftPosition tryNextGear)
		{
			var tmpGear = Gearbox.Gear;
			_gearbox.Gear = tryNextGear;
			var response = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
			_gearbox.Gear = tmpGear;
			return response;
		}

		protected virtual GearshiftPosition DoCheckDownshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// down shift
			if (IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear = Gears.Predecessor(currentGear);
				while (SkipGears && Gears.HasPredecessor(currentGear)) {
					currentGear = Gears.Predecessor(currentGear);
					var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					inAngularVelocity = GearboxModelData.Gears[currentGear.Gear].Ratio * outAngularVelocity;
					inTorque = response.ClutchPowerRequest / inAngularVelocity;
					var maxTorque = VectoMath.Min(response.DynamicFullLoadPower / ((DataBus.EngineSpeed + response.EngineSpeed) / 2),
						Gears.First().Equals(currentGear)
							? GearboxModelData.Gears[currentGear.Gear].ShiftPolygon.InterpolateDownshift(response.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = maxTorque.IsEqual(0) ? -1 : (1 - inTorque / maxTorque).Value();
					if (reserve >= GearboxModelData.TorqueReserve && IsBelowUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}
					currentGear = Gears.Successor(currentGear);
					break;
				}
			}
			return currentGear;
		}
	}
}