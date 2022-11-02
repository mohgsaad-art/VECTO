using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATShiftStrategyOptimized : ATShiftStrategy
	{
		private List<CombustionEngineFuelData> fcMap;
		private Dictionary<uint, EngineFullLoadCurve> fld;
		private ShiftStrategyParameters shiftStrategyParameters;
		private SimplePowertrainContainer TestContainer;
		private ATGearbox TestContainerGbx;

		private Kilogram vehicleMass;
		private Kilogram MinMass;
		private Kilogram MaxMass;

		private List<SchmittTrigger> LoadStageSteps = new List<SchmittTrigger>();
		private ShiftLineSet UpshiftLineTCLocked = new ShiftLineSet();

		public new static string Name => "AT - EffShift";

		public ATShiftStrategyOptimized(IVehicleContainer dataBus) : base(dataBus)
		{
			var runData = dataBus.RunData;
			if (runData.EngineData == null) {
				return;
			}

			fcMap = runData.EngineData.Fuels;
			fld = runData.EngineData.FullLoadCurves;
			vehicleMass = runData.VehicleData.TotalVehicleMass;
			shiftStrategyParameters = runData.GearshiftParameters;

			MinMass = runData.VehicleData.MinimumVehicleMass;
			MaxMass = runData.VehicleData.MaximumVehicleMass;

			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			InitializeShiftlinesTCToLocked();

			InitializeTestContainer(runData);

		}

		private void InitializeShiftlinesTCToLocked()
		{
			if (shiftStrategyParameters.LoadStageThresoldsUp.Length != shiftStrategyParameters.LoadStageThresoldsDown.Length) {
				throw new VectoException("Thresholds for loadstage definition Up/Down need to be of same length!");
			}

			foreach (var entry in shiftStrategyParameters.LoadStageThresoldsUp.Zip(
				shiftStrategyParameters.LoadStageThresoldsDown, Tuple.Create)) {
				LoadStageSteps.Add(new SchmittTrigger(entry));
			}

			var slopes = new[] {
				VectoMath.InclinationToAngle(DeclarationData.GearboxTCU.DownhillSlope / 100.0),
				VectoMath.InclinationToAngle(0),
				VectoMath.InclinationToAngle(DeclarationData.GearboxTCU.UphillSlope / 100.0)
			};

			if (shiftStrategyParameters.ShiftSpeedsTCToLocked.Length < shiftStrategyParameters.LoadStageThresoldsUp.Length + 1) {
				throw new VectoException(
					"Shift speeds TC -> L need to be defined for all {0} load stages",
					shiftStrategyParameters.LoadStageThresoldsUp.Length + 1);
			}

			for (var loadStage = 1; loadStage <= 6; loadStage++) {
				if (shiftStrategyParameters.ShiftSpeedsTCToLocked[loadStage - 1].Length != 6) {
					throw new VectoException("Exactly 6 shift speeds need to be provided! (downhill/level/uphill)*(a_max/a_min)");
				}

				var shiftLines = new ShiftLines();
				for (var i = 0; i < 6; i++) {
					var t = Tuple.Create(slopes[i % 3], shiftStrategyParameters.ShiftSpeedsTCToLocked[loadStage - 1][i].RPMtoRad());
					if (i < 3) {
						shiftLines.entriesAMax.Add(t);
					} else {
						shiftLines.entriesAMin.Add(t);
					}
				}

				UpshiftLineTCLocked.LoadStages[loadStage] = shiftLines;
			}
		}

		private void InitializeTestContainer(VectoRunData runData)
		{
			// fuel list here has no effect as this is the mod-container for the test-powertrain only
			TestContainer = new SimplePowertrainContainer(runData);

			PowertrainBuilder.BuildSimplePowertrain(runData, TestContainer);
			TestContainerGbx = TestContainer.GearboxCtl as ATGearbox;
			if (TestContainerGbx == null) {
				throw new VectoException("Unknown gearboxtype: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}

			// initialize vehicle so that vehicleStopped of the testcontainer is false (required for test-runs)
			TestContainerGbx.Gear = new GearshiftPosition(2u, true);
			TestContainer.VehiclePort.Initialize(10.KMPHtoMeterPerSecond(), 0.SI<Radian>());

			if (runData.Cycle.CycleType == CycleType.MeasuredSpeed) {
				try {
					TestContainer.GetCycleOutPort().Initialize();
					TestContainer.GetCycleOutPort().Request(0.SI<Second>(), 1.SI<Second>());
				} catch (Exception) { }
			}

			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}
		}

		
		#region Overrides of ATShiftStrategy

		protected override bool? CheckEarlyUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque,
			PerSecond origInAngularVelocity, GearshiftPosition currentGear, Second lastShiftTime, IResponse response)
		{
			var next = Gears.Successor(currentGear);
			
			if (currentGear.Gear == next.Gear && currentGear.TorqueConverterLocked != next.TorqueConverterLocked) {
				return CheckUpshiftFromTC(
					absTime, dt, outTorque, outAngularVelocity, origInTorque, origInAngularVelocity, currentGear, lastShiftTime,
					response);
			}

			return CheckEarlyUpshiftFromLocked(
				absTime, dt, outTorque, outAngularVelocity, origInTorque, origInAngularVelocity, currentGear, lastShiftTime,
				response);
		}

		private bool? CheckUpshiftFromTC(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond origInAngularVelocity, GearshiftPosition currentGear, Second lastShiftTime, IResponse response)
		{
			var accPower = EstimateAccelerrationPower(outAngularVelocity, outTorque);

			var _accMin = (accPower / DataBus.VehicleInfo.VehicleSpeed / (MaxMass + DataBus.WheelsInfo.ReducedMassWheels)).Cast<MeterPerSquareSecond>();
			var _accMax = (accPower / DataBus.VehicleInfo.VehicleSpeed / (MinMass + DataBus.WheelsInfo.ReducedMassWheels)).Cast<MeterPerSquareSecond>();

			var engineLoadPercent = inTorque / response.Engine.DynamicFullLoadTorque;
			var _loadStage = GetLoadStage(engineLoadPercent);

			var shiftSpeed = UpshiftLineTCLocked.LookupShiftSpeed(
				_loadStage, DataBus.DrivingCycleInfo.RoadGradient, DataBus.DriverInfo.DriverAcceleration, _accMin, _accMax);
			var shiftSpeedGbxOut = shiftSpeed / GearboxModelData.Gears[currentGear.Gear].Ratio;
			if (outAngularVelocity > shiftSpeedGbxOut) {
				Upshift(absTime, currentGear);
				return true;
			}

			return false;
		}

		protected Watt EstimateAccelerrationPower(PerSecond gbxOutSpeed, NewtonMeter gbxOutTorque)
		{
			var vehicleSpeed = DataBus.VehicleInfo.VehicleSpeed;
			var avgSlope =
			((DataBus.DrivingCycleInfo.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
			DataBus.DrivingCycleInfo.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleInfo.VehicleSpeed;
			var rollResistanceLoss = DataBus.VehicleInfo.RollingResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;

			var slopeLoss = DataBus.VehicleInfo.SlopeResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var axleLoss = DataBus.AxlegearInfo.AxlegearLoss();

			return gbxOutSpeed * gbxOutTorque - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;
		}

		private int GetLoadStage(double engineLoadPercent)
		{
			var sum = 1;
			foreach (var entry in LoadStageSteps) {
				sum += entry.GetOutput(engineLoadPercent * 100);
			}

			return sum;
		}

		protected bool? CheckEarlyUpshiftFromLocked(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque,
			PerSecond origInAngularVelocity, GearshiftPosition currentGear, Second lastShiftTime, IResponse response1)
		{
			if (outAngularVelocity.IsEqual(0)) {
				return null;
			}

			if (DataBus.DriverInfo.DriverAcceleration < 0) {
				return null;
			}
			if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear.Gear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
				return null;
			}

			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			var fcCurrent = double.NaN;

			var current = currentGear;
			//var currentIdx = GearList.IndexOf(current);

			var vDrop = DataBus.DriverInfo.DriverAcceleration * shiftStrategyParameters.ATLookAheadTime;
			var vehicleSpeedPostShift = (DataBus.VehicleInfo.VehicleSpeed + vDrop * shiftStrategyParameters.VelocityDropFactor).LimitTo(
				0.KMPHtoMeterPerSecond(), DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed);

			var outAngularVelocityEst =
				(outAngularVelocity * vehicleSpeedPostShift / (DataBus.VehicleInfo.VehicleSpeed + DataBus.DriverInfo.DriverAcceleration * dt))
				.Cast<PerSecond>();
			var outTorqueEst = outTorque * outAngularVelocity / outAngularVelocityEst;

			foreach (var next in Gears.IterateGears(Gears.Successor(currentGear), Gears.Successor(currentGear, (uint)shiftStrategyParameters.AllowedGearRangeFC))) {
				
				if (next == null) {
					// no further gear
					continue;
				}

				if (current.TorqueConverterLocked != next.TorqueConverterLocked && current.Gear != next.Gear) {
					// upshift from C to L with skipping gear not allowed
					continue;
				}

				if (!(GearboxModelData.Gears[next.Gear].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC)) {
					continue;
				}

				var inAngularVelocity = GearboxModelData.Gears[next.Gear].Ratio * outAngularVelocity;
				var totalTransmissionRatio = inAngularVelocity / (DataBus.VehicleInfo.VehicleSpeed + DataBus.DriverInfo.DriverAcceleration * dt);
				var estimatedEngineSpeed = (vehicleSpeedPostShift * totalTransmissionRatio).Cast<PerSecond>();
				if (estimatedEngineSpeed.IsSmaller(shiftStrategyParameters.MinEngineSpeedPostUpshift)) {
					continue;
				}

				var pNextGearMax = DataBus.EngineInfo.EngineStationaryFullPower(estimatedEngineSpeed);

				var response = RequestDryRunWithGear(absTime, dt, outTorqueEst, outAngularVelocityEst, next);

				if (!response.Engine.PowerRequest.IsSmaller(pNextGearMax)) {
					continue;
				}

				var inTorque = response.Engine.PowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (GearboxModelData.Gears[next.Gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inAngularVelocity)) {
					continue;
				}

				var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

				if (reserve < GearshiftParams.TorqueReserve) {
					var accelerationFactor = outAngularVelocity * GearboxModelData.Gears[currentGear.Gear].Ratio < fld[0].NTq98hSpeed
						? 1.0
						: VectoMath.Interpolate(
							fld[0].NTq98hSpeed, fld[0].NP98hSpeed, 1.0, shiftStrategyParameters.AccelerationFactor,
							outAngularVelocity * GearboxModelData.Gears[currentGear.Gear].Ratio);
					if (accelerationFactor.IsEqual(1, 1e-9)) {
						continue;
					}

					var accelerationTorque = vehicleMass * DataBus.DriverInfo.DriverAcceleration * DataBus.VehicleInfo.VehicleSpeed / outAngularVelocity;
					var reducedTorque = outTorque - accelerationTorque * (1 - accelerationFactor);

					response = RequestDryRunWithGear(absTime, dt, reducedTorque, outAngularVelocity, next);

					//response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, DataBus.DriverAcceleration * accelerationFactor, next);
					fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
					reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
					if (reserve < GearshiftParams.TorqueReserve) {
						continue;
					}
				}

				var tqdrag = fld[next.Gear].DragLoadStationaryTorque(response.Engine.EngineSpeed);
				var tqmax = fld[next.Gear].FullLoadStationaryTorque(response.Engine.EngineSpeed);
				if (tqmax.IsSmallerOrEqual(tqdrag) || response.Engine.EngineSpeed.IsGreaterOrEqual(DataBus.EngineInfo.EngineN95hSpeed)) {
					// engine speed is to high or
					// extrapolation of max torque curve for high engine speeds may leads to negative max torque 
					continue;
				}

				if (double.IsNaN(fcCurrent)) {
					//var responseCurrent = RequestDryRunWithGear(
					//	absTime, dt, vehicleSpeedForGearRating, DataBus.DriverAcceleration, current);
					//var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, current);
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorqueEst, outAngularVelocityEst, current);
					if (responseCurrent.Engine.EngineSpeed.IsGreaterOrEqual(DataBus.EngineInfo.EngineN95hSpeed)) {
						fcCurrent = double.MaxValue;
					} else {
						var tqCurrent = responseCurrent.Engine.TorqueOutDemand.LimitTo(
							fld[currentGear.Gear].DragLoadStationaryTorque(responseCurrent.Engine.EngineSpeed),
							fld[currentGear.Gear].FullLoadStationaryTorque(responseCurrent.Engine.EngineSpeed));
						fcCurrent = GetFCRating(responseCurrent.Engine.EngineSpeed, tqCurrent);
					}
				}
				var tqNext = response.Engine.TorqueOutDemand.LimitTo(
					fld[next.Gear].DragLoadStationaryTorque(response.Engine.EngineSpeed),
					fld[next.Gear].FullLoadStationaryTorque(response.Engine.EngineSpeed));
				var fcNext = GetFCRating(response.Engine.EngineSpeed, tqNext);

				if (reserve < GearshiftParams.TorqueReserve ||
					!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) || !fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFc = fcNext;
				minFcGear = next;
			}

			if (!minFcGear.Equals(current)) {
				ShiftGear(absTime, current, minFcGear);
				return true;
			}

			return null;
		}

		private double GetFCRating(PerSecond engineSpeed, NewtonMeter tqCurrent)
		{
			var fcCurrent = 0.0;
			foreach (var fuel in fcMap) {
				var fcCurrentRes = fuel.ConsumptionMap.GetFuelConsumption(tqCurrent, engineSpeed, true);
				if (fcCurrentRes.Extrapolated) {
					Log.Warn(
						"EffShift Strategy: Extrapolation of fuel consumption for current gear!n: {1}, Tq: {2}",
						engineSpeed, tqCurrent);
				}
				fcCurrent += fcCurrentRes.Value.Value() * fuel.FuelData.LowerHeatingValueVecto.Value();
			}

			return fcCurrent;
		}


		protected override bool? CheckEarlyDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque,
			PerSecond origInAngularVelocity, GearshiftPosition currentGear, Second lastShiftTime, IResponse response1)
		{
			if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear.Gear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
				return null;
			}

			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			var fcCurrent = double.NaN;

			var current = currentGear;

			foreach (var next in Gears.IterateGears(Gears.Predecessor(current), Gears.Predecessor(current, (uint)shiftStrategyParameters.AllowedGearRangeFC))) {
				if (next == null) {
					// no further gear
					continue;
				}

				if (!next.TorqueConverterLocked.Value) {
					continue;
				}
				if (current.TorqueConverterLocked != next.TorqueConverterLocked && current.Gear != next.Gear) {
					// downshift from C to L with skipping gear not allowed
					continue;
				}

				if (!(GearboxModelData.Gears[next.Gear].Ratio < shiftStrategyParameters.RatioEarlyDownshiftFC)) {
					continue;
				}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, next);

				var inAngularVelocity = GearboxModelData.Gears[next.Gear].Ratio * outAngularVelocity;
				var inTorque = response.Engine.PowerRequest / inAngularVelocity;

				if (!IsAboveUpShiftCurve(next, inTorque, inAngularVelocity, next.TorqueConverterLocked.Value)) {
					if (double.IsNaN(fcCurrent)) {
						var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, current);
						fcCurrent = GetFCRating(responseCurrent.Engine.EngineSpeed, responseCurrent.Engine.TorqueOutDemand.LimitTo(
								fld[currentGear.Gear].DragLoadStationaryTorque(responseCurrent.Engine.EngineSpeed),
								fld[currentGear.Gear].FullLoadStationaryTorque(responseCurrent.Engine.EngineSpeed)));
					}
					var fcNext = GetFCRating(response.Engine.EngineSpeed, response.Engine.TorqueOutDemand.LimitTo(
							fld[next.Gear].DragLoadStationaryTorque(response.Engine.EngineSpeed),
							fld[next.Gear].FullLoadStationaryTorque(response.Engine.EngineSpeed)));

					if (fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) && fcNext.IsSmaller(minFc)) {
						minFcGear = next;
						minFc = fcNext;
					}
				}
			}

			if (!current.Equals(minFcGear)) {
				ShiftGear(absTime, current, minFcGear);
				return true;
			}

			return null;
		}

		protected virtual void ShiftGear(Second absTime, GearshiftPosition currentGear, GearshiftPosition nextGear)
		{
			if (currentGear.TorqueConverterLocked != nextGear.TorqueConverterLocked && currentGear.Gear != nextGear.Gear) {
				throw new VectoException(
					"skipping gear from converter to locked not allowed! {0} -> {1}", currentGear.Name, nextGear.Name);
			}

			_nextGear.SetState(absTime, disengaged: false, gear: nextGear);
		}

		#endregion

		protected ResponseDryRun RequestDryRunWithGear(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition gear)
		{
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = gear;
			//TestContainerGbx.TorqueConverterLocked = gear.TorqueConverterLocked.Value;

			TestContainer.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			var response = (ResponseDryRun)TestContainer.GearboxOutPort.Request(
				0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			return response;
		}

		protected ResponseDryRun RequestDryRunWithGear(
			Second absTime, Second dt, MeterPerSecond vehicleSpeed, MeterPerSquareSecond acceleration, GearshiftPosition gear)
		{
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = gear;
			//TestContainerGbx.TorqueConverterLocked = gear.TorqueConverterLocked.Value;

			TestContainer.VehiclePort.Initialize(vehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient);
			var response = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), dt, acceleration, DataBus.DrivingCycleInfo.RoadGradient, true);
			return response;
		}

		#region Overrides of ATShiftStrategy

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
		{
			var shiftLine = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
				Math.Max(i, 2), engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);

			var upshift = new List<ShiftPolygon.ShiftPolygonEntry>();

			if (i < gearboxGears.Count - 1) {
				var maxDragTorque = engineDataFullLoadCurve.MaxDragTorque * 1.1;
				var maxTorque = engineDataFullLoadCurve.MaxTorque * 1.1;

				var speed = engineData.FullLoadCurves[0].NP98hSpeed / gearboxGears[i].Ratio * gearboxGears[i + 1].Ratio;

				upshift.Add(new ShiftPolygon.ShiftPolygonEntry(maxDragTorque, speed));
				upshift.Add(new ShiftPolygon.ShiftPolygonEntry(maxTorque, speed));
			}

			return new ShiftPolygon(shiftLine.Downshift.ToList(), upshift);
		}

		#endregion
	}
}
