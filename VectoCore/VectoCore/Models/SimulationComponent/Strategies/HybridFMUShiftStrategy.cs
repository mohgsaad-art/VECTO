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
using TUGraz.VectoCore.Models.SimulationComponent.Impl;


namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
	public class HybridFMUShiftStrategy : HybridController.HybridCtlShiftStrategy
    {
		protected new HybridController _controller;
		protected new readonly GearshiftPosition MaxStartGear;
		protected new GearshiftPosition _nextGear;

		public HybridFMUShiftStrategy(IVehicleContainer dataBus, HybridController hybridController = null) : base(hybridController, dataBus)
		{
			_controller = hybridController != null ? hybridController : null;
			var runData = dataBus.RunData;
			EarlyShiftUp = true;
			SkipGears = true;
			if (runData.EngineData == null)
			{
				return;
			}
			var transmissionRatio = runData.AxleGearData.AxleGear.Ratio *
									(runData.AngledriveData == null ? 1.0 : runData.AngledriveData.Angledrive.Ratio) /
									runData.VehicleData.DynamicTyreRadius;
			var minEngineSpeed = (runData.EngineData.FullLoadCurves[0].RatedSpeed - runData.EngineData.IdleSpeed) *
								Constants.SimulationSettings.ClutchClosingSpeedNorm + runData.EngineData.IdleSpeed;
			MaxStartGear = GearboxModelData.GearList.First();
			foreach (var gear in GearboxModelData.GearList.Reverse())
			{
				var gearData = GearboxModelData.Gears[gear.Gear];
				if (GearshiftParams.StartSpeed * transmissionRatio * gearData.Ratio > minEngineSpeed)
				{
					MaxStartGear = gear;
					break;
				}
			}
		}

		public bool SpeedTooLowForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
		{
			return (outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsSmaller(DataBus.EngineInfo.EngineIdleSpeed);
		}

		public bool SpeedTooHighForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsGreaterOrEqual(VectoMath.Min(GearboxModelData.Gears[gear.Gear].MaxSpeed,
																				DataBus.EngineInfo.EngineN95hSpeed));
		}

		public override GearshiftPosition NextGear
		{
			get { return _nextGear; }
		}

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, IList<ITransmissionInputData> gearboxGears,
			CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
		{
			return DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(
				i, engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);
		}

		public static string Name { get { return "AMT - Classic"; } }

		public override GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear, outAngularVelocity))
			{
				_nextGear = Gears.Predecessor(_nextGear);
			}
			while (Gears.HasSuccessor(_nextGear) && SpeedTooHighForEngine(_nextGear, outAngularVelocity))
			{
				_nextGear = Gears.Successor(_nextGear);
			}

			return _nextGear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (!_controller.ShiftRequired && DataBus.DriverInfo.DrivingAction != DrivingAction.Halt)
			{
				// gearbox disengaged on its own! set next gear!
				var gear = _nextGear;
				while (Gears.HasPredecessor(gear) && SpeedTooLowForEngine(gear, outAngularVelocity))
				{
					gear = Gears.Predecessor(gear);
				}

				while (Gears.HasSuccessor(gear) && SpeedTooHighForEngine(gear, outAngularVelocity))
				{
					gear = Gears.Successor(gear);
				}

				_nextGear = gear;
			}
		}

		public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0))
			{
				return InitStarterGear(absTime, outTorque, outAngularVelocity);
			}

			foreach (var gear in Gears.Reverse())
			{
				var selected = gear;
				var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
				var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
				var inTorque = response.Clutch.PowerRequest / inAngularSpeed;

				// if in shift curve and torque reserve is provided: return the current gear
				if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
					reserve >= GearshiftParams.StartTorqueReserve)
				{
					if ((inAngularSpeed - DataBus.EngineInfo.EngineIdleSpeed) / (DataBus.EngineInfo.EngineRatedSpeed - DataBus.EngineInfo.EngineIdleSpeed) <
						Constants.SimulationSettings.ClutchClosingSpeedNorm && Gears.HasPredecessor(gear))
					{
						selected = Gears.Predecessor(gear);
					}
					_nextGear = selected;
					return selected;
				}

				// if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
				if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && Gears.HasSuccessor(gear))
				{
					selected = Gears.Successor(gear);
					_nextGear = selected;
					return selected;
				}
			}

			// fallback: return first gear
			_nextGear = Gears.First();
			return _nextGear;
		}

		private GearshiftPosition InitStarterGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (!DataBus.EngineCtl.CombustionEngineOn)
			{
				_nextGear = MaxStartGear;
				return _nextGear;
			}
			foreach (var gear in Gears.IterateGears(MaxStartGear, Gears.First()))
			{
				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

				var ratedSpeed = DataBus.EngineInfo.EngineRatedSpeed;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0))
				{
					continue;
				}

				var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

				var fullLoadPower = response.Engine.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

				if (response.Engine.EngineSpeed > DataBus.EngineInfo.EngineIdleSpeed && reserve >= GearshiftParams.StartTorqueReserve)
				{
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
			// no shift when ICE Off
			var log = LogManager.GetLogger(typeof(AMTShiftStrategy).FullName);


			// no shift when vehicle stands
			if (DataBus.VehicleInfo.VehicleStopped)
			{
				return false;
			}

			if (_controller.ShiftRequired)
			{
				_nextGear = _controller.NextGear;
				return _controller.ShiftRequired;

			}

			if (!DataBus.EngineCtl.CombustionEngineOn)
			{

				/*var tooHigh = SpeedTooHighForEngine(gear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio);
				var tooLow = SpeedTooLowForEngine(gear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio);
				var tooHigh2 = SpeedTooHighForEngine(Gears.Successor(gear), inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio);
				var tooLow2 = SpeedTooLowForEngine(Gears.Successor(gear), inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio);*/
				if (DataBus.VehicleInfo.VehicleStopped)
				{
					log.Debug("Vehicle stoppped");
				}
				_nextGear = gear;
				// early up shift to higher gear ---------------------------------------
				var earlyUpshiftGear = _nextGear;
				/*if (EarlyShiftUp && Gears.HasSuccessor(_nextGear))
				{
					earlyUpshiftGear = CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, _nextGear, response);
				}*/
				if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate &&
					(IsAboveUpShiftCurve(_nextGear, inTorque, inAngularVelocity) || SpeedTooHighForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio) ||
					(earlyUpshiftGear != _nextGear)))
				{
					_nextGear = Gears.Successor(gear);
					return true;
				}
				else if (DataBus.DriverInfo.DrivingAction == DrivingAction.Coast &&
					(IsBelowDownShiftCurve(_nextGear, inTorque, inAngularVelocity) || SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) &&
					Gears.HasPredecessor(gear))
				{
					_nextGear = Gears.Predecessor(gear);
					return true;
				}
				else if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake &&
					(IsBelowDownShiftCurve(_nextGear, inTorque, inAngularVelocity) || SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) &&
					Gears.HasPredecessor(gear))
				{
					log.Debug("-->Braking and combustion engine off {0}", absTime);
					//while 70% zwischen idle und nenndrehzahl
					_nextGear = Gears.Predecessor(gear);
					return true;
				}
				else
				{
					return false;
				}
			}

			// emergency shift to not stall the engine ------------------------
			if (Gears.First().Equals(gear) &&
				SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio))
			{
				return true;
			}

			_nextGear = gear;
			while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear,
				inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio))
			{
				_nextGear = Gears.Predecessor(_nextGear);
			}

			while (Gears.HasSuccessor(_nextGear) &&
					SpeedTooHighForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio))
			{
				_nextGear = Gears.Successor(_nextGear);
			}

			if (!_nextGear.Equals(gear))
			{
				return true;
			}

			// normal shift when all requirements are fullfilled ------------------
			var minimumShiftTimePassed =
				(lastShiftTime + GearshiftParams.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed)
			{
				return false;
			}

			_nextGear = CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				response);
			if (!_nextGear.Equals(gear))
			{
				return true;
			}

			_nextGear = CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				response);

			return !_nextGear.Equals(gear);
		}

		protected virtual GearshiftPosition CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Accelerating && DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Driving)
			{
				return currentGear;
			}
			if ((absTime - _gearbox.LastDownshift).IsSmaller(GearshiftParams.UpshiftAfterDownshiftDelay))
			{
				return currentGear;
			}
			var nextGear = DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
			if (nextGear.Equals(currentGear))
			{
				return nextGear;
			}

			// estimate acceleration for selected gear
			if (EstimateAccelerationForGear(nextGear, outAngularVelocity).IsSmaller(GearshiftParams.UpshiftMinAcceleration))
			{
				// if less than 0.1 for next gear, don't shift
				if (nextGear.Gear - currentGear.Gear == 1)
				{
					return currentGear;
				}
				// if a gear is skipped but acceleration is less than 0.1, try for next gear. if acceleration is still below 0.1 don't shift!
				if (nextGear.Gear > currentGear.Gear &&
					EstimateAccelerationForGear(Gears.Successor(currentGear), outAngularVelocity)
						.IsSmaller(GearshiftParams.UpshiftMinAcceleration))
				{
					return currentGear;
				}
				nextGear = Gears.Successor(currentGear);
			}

			return nextGear;
		}

		protected virtual GearshiftPosition CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			if ((absTime - _gearbox.LastUpshift).IsSmaller(GearshiftParams.DownshiftAfterUpshiftDelay))
			{
				return currentGear;
			}
			return DoCheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
		}

		protected virtual GearshiftPosition DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity))
			{
				currentGear = Gears.Successor(currentGear);

				while (SkipGears && Gears.HasSuccessor(currentGear))
				{
					currentGear = Gears.Successor(currentGear);
					var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					inAngularVelocity = response.Engine.EngineSpeed; //ModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.Clutch.PowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(
						response.Engine.DynamicFullLoadPower /
						((DataBus.EngineInfo.EngineSpeed + response.Engine.EngineSpeed) / 2),
						currentGear.Equals(Gears.First())
							? double.MaxValue.SI<NewtonMeter>()
							: GearboxModelData.Gears[currentGear.Gear].ShiftPolygon
								.InterpolateDownshift(response.Engine.EngineSpeed));
					var reserve = 1 - inTorque / maxTorque;

					if (reserve >= 0 /*ModelData.TorqueReserve */ && IsAboveDownShiftCurve(currentGear, inTorque, inAngularVelocity))
					{
						continue;
					}

					currentGear = Gears.Predecessor(currentGear);
					break;
				}
			}

			// early up shift to higher gear ---------------------------------------
			if (EarlyShiftUp && Gears.HasSuccessor(currentGear))
			{
				currentGear = CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
			}
			return currentGear;
		}

		protected virtual GearshiftPosition CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// try if next gear would provide enough torque reserve
			var tryNextGear = Gears.Successor(currentGear);
			var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

			//_controller.Strategy.Request(absTime, dt, outTorque, outAngularVelocity, true);

			var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
			var inTorque = response.Clutch.PowerRequest / inAngularVelocity;

			// if next gear supplied enough power reserve: take it
			// otherwise take
			if (!IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity))
			{
				var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

				if (reserve >= GearshiftParams.TorqueReserve)
				{
					currentGear = tryNextGear;
				}
			}
			return currentGear;
		}


		protected virtual GearshiftPosition DoCheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// down shift
			if (IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity))
			{
				currentGear = Gears.Predecessor(currentGear);
				//while (SkipGears && currentGear > 1) {
				//	currentGear--;
				//	var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

				//	inAngularVelocity = ModelData.Gears[currentGear].Ratio * outAngularVelocity;
				//	inTorque = response.ClutchPowerRequest / inAngularVelocity;
				//	var maxTorque = VectoMath.Min(response.DynamicFullLoadPower / ((DataBus.EngineSpeed + response.EngineSpeed) / 2),
				//		currentGear > 1
				//			? ModelData.Gears[currentGear].ShiftPolygon.InterpolateDownshift(response.EngineSpeed)
				//			: double.MaxValue.SI<NewtonMeter>());
				//	var reserve = maxTorque.IsEqual(0) ? -1 : (1 - inTorque / maxTorque).Value();
				//	if (reserve >= ModelData.TorqueReserve && IsBelowUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				//		continue;
				//	}
				//	currentGear++;
				//	break;
				//}
			}
			return currentGear;
		}

		protected virtual ResponseDryRun RequestDryRunWithGear(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition tryNextGear)
		{
			var tmpGear = Gearbox.Gear;
			_gearbox.Gear = tryNextGear;
			var response = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
			_gearbox.Gear = tmpGear;
			return response;
		}
	}
}
