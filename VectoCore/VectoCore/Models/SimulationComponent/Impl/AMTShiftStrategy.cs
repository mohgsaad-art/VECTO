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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// AMTShiftStrategy implements the AMT Shifting Behaviour.
	/// </summary>
	public class AMTShiftStrategy : ShiftStrategy
	{
		protected readonly uint MaxStartGear;
		protected uint _nextGear;
		private uint DesiredGearRoadsweeping;

		public AMTShiftStrategy(VectoRunData runData, IDataBus dataBus) : base(runData.GearboxData, dataBus)
		{
			EarlyShiftUp = true;
			SkipGears = true;

			var transmissionRatio = runData.AxleGearData.AxleGear.Ratio *
									(runData.AngledriveData == null ? 1.0 : runData.AngledriveData.Angledrive.Ratio) /
									runData.VehicleData.DynamicTyreRadius;
			var minEngineSpeed = (runData.EngineData.FullLoadCurves[0].RatedSpeed - runData.EngineData.IdleSpeed) *
								Constants.SimulationSettings.ClutchClosingSpeedNorm + runData.EngineData.IdleSpeed;

			DesiredGearRoadsweeping = runData.DriverData.PTODriveRoadsweepingGear;

			foreach (var gearData in ModelData.Gears.Reverse()) {
				if (ModelData.StartSpeed * transmissionRatio * gearData.Value.Ratio > minEngineSpeed) {
					MaxStartGear = gearData.Key;
					break;
				}
			}
		}

		private bool SpeedTooLowForEngine(uint gear, PerSecond outAngularSpeed)
		{
			return (outAngularSpeed * ModelData.Gears[gear].Ratio).IsSmaller(DataBus.EngineIdleSpeed);
		}

		private bool SpeedTooHighForEngine(uint gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * ModelData.Gears[gear].Ratio).IsGreaterOrEqual(VectoMath.Min(ModelData.Gears[gear].MaxSpeed,
																				DataBus.EngineN95hSpeed));
		}

		public override GearInfo NextGear
		{
			get { return new GearInfo(_nextGear, false); }
		}

		public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			while (_nextGear > 1 && SpeedTooLowForEngine(_nextGear, outAngularVelocity)) {
				_nextGear--;
			}
			while (_nextGear < ModelData.Gears.Count && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
				_nextGear++;
			}

			return _nextGear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed) {}

		public override uint InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleSpeed.IsEqual(0)) {
				return InitStartGear(outTorque, outAngularVelocity);
			}
			for (var gear = (uint)ModelData.Gears.Count; gear > 1; gear--) {
				var response = _gearbox.Initialize(gear, outTorque, outAngularVelocity);

				var inAngularSpeed = outAngularVelocity * ModelData.Gears[gear].Ratio;
				var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;
				var inTorque = response.ClutchPowerRequest / inAngularSpeed;

				// if in shift curve and torque reserve is provided: return the current gear
				if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
					reserve >= ModelData.StartTorqueReserve) {
					if ((inAngularSpeed - DataBus.EngineIdleSpeed) / (DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
						Constants.SimulationSettings.ClutchClosingSpeedNorm && gear > 1) {
						gear--;
					}
					_nextGear = gear;
					return gear;
				}

				// if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
				if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && gear < ModelData.Gears.Count) {
					_nextGear = gear;
					return gear + 1;
				}
			}

			// fallback: return first gear
			_nextGear = 1;
			return 1;
		} 

		private uint InitStartGear(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			for (var gear = MaxStartGear; gear > 1; gear--) {
				var inAngularSpeed = outAngularVelocity * ModelData.Gears[gear].Ratio;

				var ratedSpeed = DataBus.EngineRatedSpeed;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
					continue;
				}

				var response = _gearbox.Initialize(gear, outTorque, outAngularVelocity);

				var fullLoadPower = response.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

				if (response.EngineSpeed > DataBus.EngineIdleSpeed && reserve >= ModelData.StartTorqueReserve) {
					_nextGear = gear;
					return gear;
				}
			}
			_nextGear = 1;
			return 1;
		}

		public override bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{

			//LogShiftData.LogShiftDataToExcel(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, DataBus, _gearbox);

			// no shift when vehicle stands
			if (DataBus.VehicleStopped) {
				return false;
			}

			// emergency shift to not stall the engine ------------------------
			if (gear == 1 && SpeedTooLowForEngine(_nextGear, inAngularVelocity / ModelData.Gears[gear].Ratio)) {
				return true;
			}
			_nextGear = gear;
			while (_nextGear > 1 && SpeedTooLowForEngine(_nextGear, inAngularVelocity / ModelData.Gears[gear].Ratio)) {
				_nextGear--;
			}
			while (_nextGear < ModelData.Gears.Count &&
					SpeedTooHighForEngine(_nextGear, inAngularVelocity / ModelData.Gears[gear].Ratio)) {
				_nextGear++;
			}
			if (_nextGear != gear) {
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
						DesiredGearRoadsweeping, inAngularVelocity / ModelData.Gears[DesiredGearRoadsweeping].Ratio)) {
						_nextGear = DesiredGearRoadsweeping;
						return true;
					}
				}
			}

			// normal shift when all requirements are fullfilled ------------------
			var minimumShiftTimePassed = (lastShiftTime + ModelData.ShiftTime).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return false;
			}

			_nextGear = CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear);
			if (_nextGear != gear) {
				return true;
			}

			_nextGear = CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear);

			//if ((ModelData.Gears[_nextGear].Ratio * outAngularVelocity - DataBus.EngineIdleSpeed) /
			//	(DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
			//	Constants.SimulationSettings.ClutchClosingSpeedNorm && _nextGear > 1) {
			//	_nextGear--;
			//}

			return _nextGear != gear;
		}

		protected virtual uint CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (DataBus.DriverBehavior != DrivingBehavior.Accelerating && DataBus.DriverBehavior != DrivingBehavior.Driving) {
				return currentGear;
			}
			if ((absTime - _gearbox.LastDownshift).IsSmaller(_gearbox.ModelData.UpshiftAfterDownshiftDelay)) {
				return currentGear;
			}
			var nextGear = DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear);
			if (nextGear == currentGear) {
				return nextGear;
			}

			// estimate acceleration for selected gear
			if (EstimateAccelerationForGear(nextGear, outAngularVelocity).IsSmaller(_gearbox.ModelData.UpshiftMinAcceleration)) {
				// if less than 0.1 for next gear, don't shift
				if (nextGear - currentGear == 1) {
					return currentGear;
				}
				// if a gear is skipped but acceleration is less than 0.1, try for next gear. if acceleration is still below 0.1 don't shift!
				if (nextGear > currentGear &&
					EstimateAccelerationForGear(currentGear + 1, outAngularVelocity)
						.IsSmaller(_gearbox.ModelData.UpshiftMinAcceleration)) {
					return currentGear;
				}
				nextGear = currentGear + 1;
			}

			return nextGear;
		}

		protected virtual uint CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			if ((absTime - _gearbox.LastUpshift).IsSmaller(_gearbox.ModelData.DownshiftAfterUpshiftDelay)) {
				return currentGear;
			}
			return DoCheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear);
		}

		protected virtual uint DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear++;

				while (SkipGears && currentGear < ModelData.Gears.Count) {
					currentGear++;
					var tmpGear = Gearbox.Gear;
					_gearbox.Gear = currentGear;
					var response = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
					_gearbox.Gear = tmpGear;

					inAngularVelocity = response.EngineSpeed; //ModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.ClutchPowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(response.DynamicFullLoadPower / ((DataBus.EngineSpeed + response.EngineSpeed) / 2),
						currentGear > 1
							? ModelData.Gears[currentGear].ShiftPolygon.InterpolateDownshift(response.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = 1 - inTorque / maxTorque;

					if (reserve >= ModelData.TorqueReserve && IsAboveDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}

					currentGear--;
					break;
				}
			}

			// early up shift to higher gear ---------------------------------------
			if (EarlyShiftUp && currentGear < ModelData.Gears.Count) {
				// try if next gear would provide enough torque reserve
				var tryNextGear = currentGear + 1;
				var tmpGear = Gearbox.Gear;
				_gearbox.Gear = tryNextGear;
				var response = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
				_gearbox.Gear = tmpGear;

				inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
				inTorque = response.ClutchPowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (!IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

					if (reserve >= ModelData.TorqueReserve) {
						currentGear = tryNextGear;
					}
				}
			}
			return currentGear;
		}

		protected virtual uint DoCheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			// down shift
			if (IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear--;
				while (SkipGears && currentGear > 1) {
					currentGear--;
					var tmpGear = Gearbox.Gear;
					_gearbox.Gear = currentGear;
					var response = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
					_gearbox.Gear = tmpGear;

					inAngularVelocity = ModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.ClutchPowerRequest / inAngularVelocity;
					var maxTorque = VectoMath.Min(response.DynamicFullLoadPower / ((DataBus.EngineSpeed + response.EngineSpeed) / 2),
						currentGear > 1
							? ModelData.Gears[currentGear].ShiftPolygon.InterpolateDownshift(response.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = maxTorque.IsEqual(0) ? -1 : (1 - inTorque / maxTorque).Value();
					if (reserve >= ModelData.TorqueReserve && IsBelowUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}
					currentGear++;
					break;
				}
			}
			return currentGear;
		}
	}
}