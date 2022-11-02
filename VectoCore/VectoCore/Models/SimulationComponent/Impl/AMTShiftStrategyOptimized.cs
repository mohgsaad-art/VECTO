using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AMTShiftStrategyOptimized : AMTShiftStrategy
	{
		private List<CombustionEngineFuelData> fcMap;
		private Dictionary<uint, EngineFullLoadCurve> fld;
		private ShiftStrategyParameters shiftStrategyParameters;
		private SimplePowertrainContainer TestContainer;
		private Gearbox TestContainerGbx;

		protected readonly VelocityRollingLookup VelocityDropData = new VelocityRollingLookup();
		//private AccelerationCurveData accCurve;

		private Kilogram vehicleMass;
		

		

		public AMTShiftStrategyOptimized(IVehicleContainer dataBus) : base(dataBus)
		{
			var runData = dataBus.RunData;
			if (runData.EngineData == null) {
				return;
			}

			fcMap = runData.EngineData.Fuels;
			fld = runData.EngineData.FullLoadCurves;
			shiftStrategyParameters = runData.GearshiftParameters;
			//accCurve = runData.DriverData.AccelerationCurve;
			vehicleMass = runData.VehicleData.TotalVehicleMass;
			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			SetupVelocityDropPreprocessor(dataBus);

			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}
		}

		private void SetupVelocityDropPreprocessor(IVehicleContainer dataBus)
		{
			var runData = dataBus.RunData;
			// MQ: 2019-11-29 - fuel used here has no effect as this is the modDatacontainer for the test-powertrain only!
			TestContainer = new SimplePowertrainContainer(runData);
			PowertrainBuilder.BuildSimplePowertrain(runData, TestContainer);
			TestContainerGbx = TestContainer.GearboxCtl as Gearbox;
			if (TestContainerGbx == null) {
				throw new VectoException("Unknown gearboxtype: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}

			// register pre-processors
			var maxG = runData.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var grad = Convert.ToInt32(maxG / 2) * 2;
			if (grad == 0) {
				grad = 2;
			}

			dataBus.AddPreprocessor(
				new VelocitySpeedGearshiftPreprocessor(VelocityDropData, runData.GearboxData.TractionInterruption,
					TestContainer, -grad, grad, 2));
		}

		#region Overrides of AMTShiftStrategy

		protected override GearshiftPosition CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			IResponse minFCResponse = null;
			var fcCurrent = double.NaN;

			var fcUpshiftPossible = true;

			if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear.Gear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
				return currentGear;
			}

			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				return currentGear;
			}

			var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
			var vehicleSpeedPostShift = DataBus.VehicleInfo.VehicleSpeed - vDrop * shiftStrategyParameters.VelocityDropFactor;

			var totalTransmissionRatio = DataBus.EngineInfo.EngineSpeed / DataBus.VehicleInfo.VehicleSpeed;

			//for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
			foreach (var tryNextGear in Gears.IterateGears(Gears.Successor(currentGear),
				Gears.Successor(currentGear, (uint)shiftStrategyParameters.AllowedGearRangeFC))) {
				//var tryNextGear = (uint)(currentGear.Gear + i);

				if (tryNextGear == null ||
					!(GearboxModelData.Gears[tryNextGear.Gear].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC)) {
					continue;
				}

				fcUpshiftPossible = true;

				//var response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, DataBus.DriverAcceleration, tryNextGear);
				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity,
					tryNextGear);
				
				var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				var inTorque = response.Clutch.PowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				var estimatedEngineSpeed = (vehicleSpeedPostShift * (totalTransmissionRatio /
						GearboxModelData.Gears[currentGear.Gear].Ratio * GearboxModelData.Gears[tryNextGear.Gear].Ratio))
					.Cast<PerSecond>();
				if (estimatedEngineSpeed.IsSmaller(shiftStrategyParameters.MinEngineSpeedPostUpshift)) {
					continue;
				}

				var pNextGearMax = DataBus.EngineInfo.EngineStationaryFullPower(estimatedEngineSpeed);

				if (!response.Engine.PowerRequest.IsSmaller(pNextGearMax)) {
					continue;
				}

				var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

				//var reserve = 1 - response.EngineTorqueDemandTotal / response.EngineStationaryFullLoadTorque;

				if (reserve < GearshiftParams.TorqueReserve /* && reserve > -0.1*/) {
					//var acc = EstimateAcceleration(outAngularVelocity, outTorque);

					var accelerationFactor = outAngularVelocity * GearboxModelData.Gears[currentGear.Gear].Ratio < fld[0].NTq98hSpeed
						? 1.0
						: VectoMath.Interpolate(
							fld[0].NTq98hSpeed, fld[0].NP98hSpeed, 1.0, shiftStrategyParameters.AccelerationFactor,
							outAngularVelocity * GearboxModelData.Gears[currentGear.Gear].Ratio);
					if (accelerationFactor.IsEqual(1, 1e-9)) {
						continue;
					}
					//var minAcc = VectoMath.Min(DataBus.DriverAcceleration, accCurve.Lookup(DataBus.VehicleSpeed).Acceleration * accelerationFactor);
					//var minAcc = DataBus.DriverAcceleration * accelerationFactor;
					//response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, minAcc, tryNextGear);
					var accelerationTorque = vehicleMass * DataBus.DriverInfo.DriverAcceleration * DataBus.VehicleInfo.VehicleSpeed / outAngularVelocity;
					var reducedTorque = outTorque - accelerationTorque * (1 - accelerationFactor);

					response = RequestDryRunWithGear(absTime, dt, reducedTorque, outAngularVelocity, tryNextGear);
					fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
					reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
					if (reserve < GearshiftParams.TorqueReserve) {
						continue;
					} else {
						//Log.Error("foo");
					}
				}

				if (double.IsNaN(fcCurrent)) {
					//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
					var tqCurrent = responseCurrent.Engine.TotalTorqueDemand.LimitTo(
						fld[currentGear.Gear].DragLoadStationaryTorque(responseCurrent.Engine.EngineSpeed),
						fld[currentGear.Gear].FullLoadStationaryTorque(responseCurrent.Engine.EngineSpeed));
					fcCurrent = GetFCRating(responseCurrent.Engine.EngineSpeed, tqCurrent);
				}
				var tqNext = response.Engine.TotalTorqueDemand.LimitTo(
					fld[tryNextGear.Gear].DragLoadStationaryTorque(response.Engine.EngineSpeed),
					fld[tryNextGear.Gear].FullLoadStationaryTorque(response.Engine.EngineSpeed));
				var fcNext = GetFCRating(response.Engine.EngineSpeed, tqNext);
				
				if (reserve < GearshiftParams.TorqueReserve ||
					!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) || !fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFcGear = tryNextGear;
				minFc = fcNext;
				minFCResponse = response;
			}

			if (!currentGear.Equals(minFcGear)) {
				return minFcGear;
			}

			//todo mk20210618 fcUpshiftPossible is always true! Maybe this statement can be simplified?
			return fcUpshiftPossible
				? currentGear
				: base.CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
		}

		private double GetFCRating(PerSecond engineSpeed, NewtonMeter tqCurrent)
		{
			var fcCurrent = 0.0;
			foreach (var fuel in fcMap) {
				var fcCurRes = fuel.ConsumptionMap.GetFuelConsumption(tqCurrent, engineSpeed, true);
				if (fcCurRes.Extrapolated) {
					Log.Warn(
						"EffShift Strategy: Extrapolation of fuel consumption for current gear!n: {0}, Tq: {1}",
						engineSpeed, tqCurrent);
				}
				fcCurrent += fcCurRes.Value.Value() * fuel.FuelData.LowerHeatingValueVecto.Value();
			}
			return fcCurrent;
		}

		
		protected override GearshiftPosition DoCheckDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			var nextGear = base.DoCheckDownshift(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);

			if (Equals(nextGear, currentGear) && currentGear.Gear > GearboxModelData.Gears.Keys.Min()) {
				nextGear = CheckEarlyDownshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response);
			}
			return nextGear;
		}

		protected virtual GearshiftPosition CheckEarlyDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			var fcCurrent = double.NaN;

			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				return currentGear;
			}

			if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear.Gear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
				return currentGear;
			}

			//for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
			foreach (var tryNextGear in Gears.IterateGears(Gears.Predecessor(currentGear),
				Gears.Predecessor(currentGear, (uint)shiftStrategyParameters.AllowedGearRangeFC))) {
				//var tryNextGear = (uint)(currentGear.Gear - i);

				if (tryNextGear == null || !(GearboxModelData.Gears[tryNextGear.Gear].Ratio <= shiftStrategyParameters.RatioEarlyDownshiftFC)) {
					continue;
				}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				//var response = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, tryNextGear);

				var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				var inTorque = response.Clutch.PowerRequest / inAngularVelocity;

				if (IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				if (double.IsNaN(fcCurrent)) {
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
					fcCurrent = GetFCRating(responseCurrent.Engine.EngineSpeed, responseCurrent.Engine.TorqueOutDemand.LimitTo(
							fld[currentGear.Gear].DragLoadStationaryTorque(responseCurrent.Engine.EngineSpeed),
							fld[currentGear.Gear].FullLoadStationaryTorque(responseCurrent.Engine.EngineSpeed)));
				}
				var fcNext = GetFCRating(response.Engine.EngineSpeed, response.Engine.TorqueOutDemand.LimitTo(
						fld[tryNextGear.Gear].DragLoadStationaryTorque(response.Engine.EngineSpeed),
						fld[tryNextGear.Gear].FullLoadStationaryTorque(response.Engine.EngineSpeed)));

				if (!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) ||
					!fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFcGear = tryNextGear;
				minFc = fcNext;
			}

			return minFcGear;
		}

		#endregion

		//protected ResponseDryRun RequestDryRunWithGear(
		//	Second absTime, Second dt, MeterPerSecond vehicleSpeed, MeterPerSquareSecond acceleration, uint tryNextGear)
		//{
		//	LogEnabled = false;
		//	TestContainerGbx.Disengaged = false;
		//	TestContainerGbx.Gear = tryNextGear;

		//	//TestContainer.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
		//	TestContainer.VehiclePort.Initialize(vehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient);
		//	var response = (ResponseDryRun)TestContainer.VehiclePort.Request(
		//		0.SI<Second>(), dt, acceleration, DataBus.DrivingCycleInfo.RoadGradient, true);

		//	//var response = (ResponseDryRun)TestContainer.GearboxOutPort.Request(
		//	//	0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
		//	LogEnabled = true;
		//	return response;
		//}

		protected override ResponseDryRun RequestDryRunWithGear(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition tryNextGear)
		{
			LogEnabled = false;
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = tryNextGear;

			TestContainer.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			var response = (ResponseDryRun)TestContainer.GearboxOutPort.Request(
				0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			LogEnabled = true;
			return response;
		}

		public new static string Name => "AMT - EffShift";

		#region Overrides of AMTShiftStrategy

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
		{
			return DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
				i, engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);
		}

		#endregion
	}
}
