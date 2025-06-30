using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
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
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
    public class ATShiftStrategyOptimized : BaseShiftStrategy
	{
		public const string Name = "AT - EffShift";

		protected new readonly NextGearState _nextGear = new NextGearState();

		protected KilogramSquareMeter EngineInertia;
		protected List<CombustionEngineFuelData> fcMap;
		protected Dictionary<uint, EngineFullLoadCurve> fld;

		protected ITestPowertrain TestPowertrain;

		protected Kilogram vehicleMass;
		protected Kilogram MinMass;
		protected Kilogram MaxMass;

		protected List<SchmittTrigger> LoadStageSteps = new List<SchmittTrigger>();
		protected ShiftLineSet UpshiftLineTCLocked = new ShiftLineSet();

		protected IAPTGearbox _gearbox;

        public ATShiftStrategyOptimized(IVehicleContainer container) : base(container)
		{
			EngineInertia = container.RunData.EngineData?.Inertia ?? 0.SI<KilogramSquareMeter>();

            if (RunData.EngineData == null) {
				return;
			}

			fcMap = RunData.EngineData.Fuels;
			fld = RunData.EngineData.FullLoadCurves;
			vehicleMass = RunData.VehicleData.TotalVehicleMass;
			
			MinMass = RunData.VehicleData.MinimumVehicleMass;
			MaxMass = RunData.VehicleData.MaximumVehicleMass;

			if (GearshiftParams == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			InitializeShiftlinesTCToLocked();

			InitializeTestContainer(RunData);

		}

		public override IGearbox Gearbox {
			get => _gearbox;
			set {
				if (value is IAPTGearbox gbx) {
					_gearbox = gbx;
					return;
				}
				throw new VectoException("This shift strategy can't handle gearbox of type {0}, expected {1}", value.GetType().Name, nameof(APTGearbox));
			}
		}

        public override GearshiftPosition NextGear => _nextGear.Gear;

        private void InitializeShiftlinesTCToLocked()
		{
			if (GearshiftParams.LoadStageThresoldsUp.Length != GearshiftParams.LoadStageThresoldsDown.Length) {
				throw new VectoException("Thresholds for loadstage definition Up/Down need to be of same length!");
			}

			foreach (var entry in GearshiftParams.LoadStageThresoldsUp.Zip(
				GearshiftParams.LoadStageThresoldsDown, Tuple.Create)) {
				LoadStageSteps.Add(new SchmittTrigger(entry));
			}

			var slopes = new[] {
				VectoMath.InclinationToAngle(DeclarationData.GearboxTCU.DownhillSlope / 100.0),
				VectoMath.InclinationToAngle(0),
				VectoMath.InclinationToAngle(DeclarationData.GearboxTCU.UphillSlope / 100.0)
			};

			if (GearshiftParams.ShiftSpeedsTCToLocked.Length < GearshiftParams.LoadStageThresoldsUp.Length + 1) {
				throw new VectoException(
					"Shift speeds TC -> L need to be defined for all {0} load stages",
					GearshiftParams.LoadStageThresoldsUp.Length + 1);
			}

			for (var loadStage = 1; loadStage <= 6; loadStage++) {
				if (GearshiftParams.ShiftSpeedsTCToLocked[loadStage - 1].Length != 6) {
					throw new VectoException("Exactly 6 shift speeds need to be provided! (downhill/level/uphill)*(a_max/a_min)");
				}

				var shiftLines = new ShiftLines();
				for (var i = 0; i < 6; i++) {
					var t = Tuple.Create(slopes[i % 3], GearshiftParams.ShiftSpeedsTCToLocked[loadStage - 1][i].RPMtoRad());
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
			TestPowertrain = Container.SimplePowertrainBuilder.CreateTestPowertrain(Container, false);

			// initialize vehicle so that vehicleStopped of the testcontainer is false (required for test-runs)
			TestPowertrain.Gearbox.SetGear = new GearshiftPosition(2u, true);
			TestPowertrain.Vehicle.Initialize(10.KMPHtoMeterPerSecond(), 0.SI<Radian>());

			if (runData.Cycle.CycleType == CycleType.MeasuredSpeed) {
				try {
					TestPowertrain.Container.GetCycleOutPort().Initialize();
					TestPowertrain.Container.GetCycleOutPort().Request(0.SI<Second>(), 1.SI<Second>());
				} catch (Exception) { }
			}

			if (GearshiftParams.AllowedGearRangeFC > 2 || GearshiftParams.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				GearshiftParams.AllowedGearRangeFC = GearshiftParams.AllowedGearRangeFC.LimitTo(1, 2);
			}
		}

		public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
		{
			if (Container.VehicleInfo.VehicleSpeed.IsEqual(0)) {
				// AT always starts in first gear and TC active!
				_gearbox.Disengaged = true;
				return Gears.First();
			}

			foreach (var gear in Gears.Reverse()) {
				var response = RequestDryRunWithGear(absTime, dt, torque, outAngularVelocity, gear);
				//var response = _gearbox.Initialize(gear, torque, outAngularVelocity);

				if (response.Engine.EngineSpeed > Container.EngineInfo.EngineRatedSpeed || response.Engine.EngineSpeed < Container.EngineInfo.EngineIdleSpeed) {
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
            var braking = Container.DriverInfo.DriverBehavior == DrivingBehavior.Braking;
            var torqueNegative = outTorque.IsSmaller(0);
            var vehicleSpeed = Container.VehicleInfo.VehicleSpeed + Container.DriverInfo.DriverAcceleration * dt;
            var slowerThanDisengageSpeed =
                vehicleSpeed.IsSmaller(GearboxModelData.DisengageWhenHaltingSpeed);
            var disengageBeforeHalting = braking && torqueNegative && slowerThanDisengageSpeed;

            // 2) L -> 0: disengage if inAngularVelocity == 0
            var disengageAngularVelocityZero = _gearbox.TorqueConverterLocked && inAngularVelocity.IsEqual(0.SI<PerSecond>());

            // 3) 1C -> 0: disengange when negative T_out and positive T_in
            var gear1C = Gears.First().Equals(gear);
            var disengageTOutNegativeAndTInPositive = Container.DriverInfo.DriverAcceleration < 0 && gear1C && outTorque.IsSmaller(0) &&
                                                    inTorque.IsGreater(0);

            var disengageTCEngineSpeedLowerIdle = braking && torqueNegative && gear1C &&
                                                inAngularVelocity.IsSmallerOrEqual(Container.EngineInfo.EngineIdleSpeed);

            if (disengageBeforeHalting
                || disengageTCEngineSpeedLowerIdle
                || disengageAngularVelocityZero
                || disengageTOutNegativeAndTInPositive) {
                // In order to make it to the halting distance do not allow disengaging if propulsion from engine is needed.
                bool allowDisengageGear = braking && !torqueNegative && slowerThanDisengageSpeed;

                _nextGear.SetState(absTime, disengaged: !allowDisengageGear, gear: Gears.First());
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
			if (inAngularVelocity.IsSmaller(Container.EngineInfo.EngineIdleSpeed) && Gears.HasPredecessor(gear)) {
				Log.Debug("engine speed would fall below idle speed - shift down");
				Downshift(absTime, gear);
				return true;
			}

			// Emergency Upshift: if higher than engine rated speed
			if (inAngularVelocity.IsGreaterOrEqual(VectoMath.Min(GearboxModelData.Gears[gear.Gear].MaxSpeed, Container.EngineInfo.EngineN95hSpeed))) {
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

#region UpshiftChecks

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
		protected virtual bool CheckUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(base.GearshiftParams.TimeBetweenGearshifts);
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
            if (Container.DriverInfo.DriverAcceleration.IsSmaller(0)) {
                return null;
            }
            if (!Container.VehicleInfo.VehicleSpeed.IsEqual(0)) {
                var reachableAcceleration = EstimateAccelerationForGear(nextGear, outAngularVelocity);
                var minAcceleration = _gearbox.TorqueConverterLocked
                    ? base.GearshiftParams.UpshiftMinAcceleration
                    : GearboxModelData.TorqueConverterData.CLUpshiftMinAcceleration;
                minAcceleration = VectoMath.Min(
                    minAcceleration, VectoMath.Max(0.SI<MeterPerSquareSecond>(), Container.DriverInfo.DriverAcceleration));
                minAccelerationReachable = reachableAcceleration.IsGreaterOrEqual(minAcceleration);
            }

            if (isAboveUpShift && minAccelerationReachable) {
                Upshift(absTime, gear);
                return true;
            }

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
            var minEngineSpeed = VectoMath.Min(700.RPMtoRad(), gearRatio * (Container.EngineInfo.EngineN80hSpeed - 150.RPMtoRad()));

            var nextGearboxInSpeed = outAngularVelocity * nextGear.TorqueConverterRatio;
            var nextGearboxInTorque = outTorque / nextGear.TorqueConverterRatio;
            var shiftLosses = _gearbox.ComputeShiftLosses(outTorque, outAngularVelocity, nextGearPos) /
                            GearboxModelData.PowershiftShiftTime / nextGearboxInSpeed;
            nextGearboxInTorque += shiftLosses;
            var tcOperatingPoint =
                _gearbox.TorqueConverter.FindOperatingPoint(absTime, dt, nextGearboxInTorque, nextGearboxInSpeed);

            var engineSpeedOverMin = tcOperatingPoint.InAngularVelocity.IsGreater(minEngineSpeed);
            var avgSpeed = (Container.EngineInfo.EngineSpeed + tcOperatingPoint.InAngularVelocity) / 2;
            var engineMaxTorque = Container.EngineInfo.EngineStationaryFullPower(avgSpeed) / avgSpeed;
            var engineInertiaTorque = Formulas.InertiaPower(
                                        Container.EngineInfo.EngineSpeed, tcOperatingPoint.InAngularVelocity, _gearbox.EngineInertia, dt) / avgSpeed;
            var engineTorqueBelowMax =
                tcOperatingPoint.InTorque.IsSmallerOrEqual(engineMaxTorque - engineInertiaTorque);

            var reachableAcceleration =
                EstimateAcceleration(
                    outAngularVelocity, outTorque, inAngularVelocity, inTorque, gear.Gear, response); // EstimateAccelerationForGear(gear + 1, outAngularVelocity);
            var minAcceleration = VectoMath.Min(
                GearboxModelData.TorqueConverterData.CCUpshiftMinAcceleration,
                Container.DriverInfo.DriverAcceleration);
            var minAccelerationReachable = reachableAcceleration.IsGreaterOrEqual(minAcceleration);

            if (engineSpeedOverMin && engineTorqueBelowMax && minAccelerationReachable) {
                Upshift(absTime, gear);
                return true;
            }

            return null;
        }

        protected virtual bool? CheckEarlyUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque,
			PerSecond origInAngularVelocity, GearshiftPosition currentGear, Second lastShiftTime, IResponse response)
		{
			var next = Gears.Successor(currentGear);

			if (currentGear.Gear == next.Gear && currentGear.TorqueConverterLocked != next.TorqueConverterLocked) {
				return CheckUpshiftFromTC(
					absTime, dt, outTorque, outAngularVelocity, origInTorque, origInAngularVelocity, currentGear, lastShiftTime,
					response);
			}

			if (currentGear.TorqueConverterLocked==false) {
				return false;
			}

            return CheckEarlyUpshiftFromLocked(
				absTime, dt, outTorque, outAngularVelocity, origInTorque, origInAngularVelocity, currentGear, lastShiftTime,
				response);
		}

		private bool? CheckUpshiftFromTC(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond origInAngularVelocity, GearshiftPosition currentGear, Second lastShiftTime, IResponse response)
		{
			var accPower = EstimateAccelerationPower(outAngularVelocity, outTorque);

			var _accMin = (accPower / Container.VehicleInfo.VehicleSpeed / (MaxMass + Container.WheelsInfo.ReducedMassWheels)).Cast<MeterPerSquareSecond>();
			var _accMax = (accPower / Container.VehicleInfo.VehicleSpeed / (MinMass + Container.WheelsInfo.ReducedMassWheels)).Cast<MeterPerSquareSecond>();

			var engineLoadPercent = inTorque / response.Engine.DynamicFullLoadTorque;
			var _loadStage = GetLoadStage(engineLoadPercent);

			var shiftSpeed = UpshiftLineTCLocked.LookupShiftSpeed(
				_loadStage, Container.DrivingCycleInfo.RoadGradient, Container.DriverInfo.DriverAcceleration, _accMin, _accMax);
			var shiftSpeedGbxOut = shiftSpeed / GearboxModelData.Gears[currentGear.Gear].Ratio;
			if (outAngularVelocity > shiftSpeedGbxOut) {
				Upshift(absTime, currentGear);
				return true;
			}

			return false;
		}

		protected Watt EstimateAccelerationPower(PerSecond gbxOutSpeed, NewtonMeter gbxOutTorque)
		{
			var vehicleSpeed = Container.VehicleInfo.VehicleSpeed;
			var avgSlope =
				((Container.DrivingCycleInfo.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
				Container.DrivingCycleInfo.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = Container.VehicleInfo.AirDragResistance(vehicleSpeed, vehicleSpeed).AirdragForce * Container.VehicleInfo.VehicleSpeed;
			var rollResistanceLoss = Container.VehicleInfo.RollingResistance(avgSlope) * Container.VehicleInfo.VehicleSpeed;

			var slopeLoss = Container.VehicleInfo.SlopeResistance(avgSlope) * Container.VehicleInfo.VehicleSpeed;
			var axleLoss = Container.AxlegearInfo.AxlegearLoss();

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

            if (Container.DriverInfo.DriverAcceleration < 0) {
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

            var vDrop = Container.DriverInfo.DriverAcceleration * GearshiftParams.ATLookAheadTime;
            var vehicleSpeedPostShift = (Container.VehicleInfo.VehicleSpeed + vDrop * GearshiftParams.VelocityDropFactor).LimitTo(
                0.KMPHtoMeterPerSecond(), Container.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed);

            var outAngularVelocityEst =
                (outAngularVelocity * vehicleSpeedPostShift / (Container.VehicleInfo.VehicleSpeed + Container.DriverInfo.DriverAcceleration * dt))
                .Cast<PerSecond>();
            var outTorqueEst = outTorque * outAngularVelocity / outAngularVelocityEst;

			var dtLow = dt.IsSmaller(Constants.SimulationSettings.TargetTimeInterval * 0.8);
			var isGear1 = currentGear.Gear.Equals(1);
			var beforeReachingTargetSpeed = Container.DrivingCycleInfo.TargetSpeed.IsSmallerOrEqual(Container.VehicleInfo.VehicleSpeed + Container.DriverInfo.DriverAcceleration * dt) && Container.DriverInfo.DriverAcceleration.IsGreater(0.0);
			var rightIsStop = Container.DrivingCycleInfo.CycleData.RightSample.VehicleTargetSpeed.IsEqual(0.0);

            foreach (var next in Gears.IterateGears(Gears.Successor(currentGear), Gears.Successor(currentGear, (uint)GearshiftParams.AllowedGearRangeFC))) {

				if (dtLow && isGear1 && !beforeReachingTargetSpeed && rightIsStop) {
					continue;
				}

                if (next == null) {
                    // no further gear
                    continue;
                }

                if (current.TorqueConverterLocked != next.TorqueConverterLocked && current.Gear != next.Gear) {
                    // upshift from C to L with skipping gear not allowed
                    continue;
                }

                if (!(GearboxModelData.Gears[next.Gear].Ratio < GearshiftParams.RatioEarlyUpshiftFC)) {
                    continue;
                }

                var inAngularVelocity = GearboxModelData.Gears[next.Gear].Ratio * outAngularVelocity;
                var totalTransmissionRatio = inAngularVelocity / (Container.VehicleInfo.VehicleSpeed + Container.DriverInfo.DriverAcceleration * dt);
                var estimatedEngineSpeed = vehicleSpeedPostShift * totalTransmissionRatio;
                if (estimatedEngineSpeed.IsSmaller(GearshiftParams.MinEngineSpeedPostUpshift)) {
                    continue;
                }

                var pNextGearMax = Container.EngineInfo.EngineStationaryFullPower(estimatedEngineSpeed);

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

                if (reserve < base.GearshiftParams.TorqueReserve) {
                    var ratio = currentGear.TorqueConverterLocked.HasValue && !currentGear.TorqueConverterLocked.Value
                        ? GearboxModelData.Gears[currentGear.Gear].TorqueConverterRatio
                        : GearboxModelData.Gears[currentGear.Gear].Ratio;
                    var accelerationFactor = outAngularVelocity * ratio < fld[0].NTq98hSpeed
                        ? 1.0
                        : VectoMath.Interpolate(
                            fld[0].NTq98hSpeed, fld[0].NP98hSpeed, 1.0, GearshiftParams.AccelerationFactor,
                            outAngularVelocity * GearboxModelData.Gears[currentGear.Gear].Ratio);
                    if (accelerationFactor.IsEqual(1, 1e-9)) {
                        continue;
                    }

                    var accelerationTorque = vehicleMass * Container.DriverInfo.DriverAcceleration * Container.VehicleInfo.VehicleSpeed / outAngularVelocity;
                    var reducedTorque = outTorque - accelerationTorque * (1 - accelerationFactor);

                    response = RequestDryRunWithGear(absTime, dt, reducedTorque, outAngularVelocity, next);

                    //response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, DataBus.DriverAcceleration * accelerationFactor, next);
                    fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
                    reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
                    if (reserve < base.GearshiftParams.TorqueReserve) {
                        continue;
                    }
                }

                var tqdrag = fld[next.Gear].DragLoadStationaryTorque(response.Engine.EngineSpeed);
                var tqmax = fld[next.Gear].FullLoadStationaryTorque(response.Engine.EngineSpeed);
                if (tqmax.IsSmallerOrEqual(tqdrag) || response.Engine.EngineSpeed.IsGreaterOrEqual(Container.EngineInfo.EngineN95hSpeed)) {
                    // engine speed is to high or
                    // extrapolation of max torque curve for high engine speeds may leads to negative max torque 
                    continue;
                }

                if (double.IsNaN(fcCurrent)) {
                    //var responseCurrent = RequestDryRunWithGear(
                    //	absTime, dt, vehicleSpeedForGearRating, DataBus.DriverAcceleration, current);
                    //var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, current);
                    var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorqueEst, outAngularVelocityEst, current);
                    if (responseCurrent.Engine.EngineSpeed.IsGreaterOrEqual(Container.EngineInfo.EngineN95hSpeed)) {
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

                if (reserve < base.GearshiftParams.TorqueReserve ||
                    !fcNext.IsSmaller(fcCurrent * GearshiftParams.RatingFactorCurrentGear) || !fcNext.IsSmaller(minFc)) {
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



#endregion


#region DownshiftChecks

		[SuppressMessage("ReSharper", "UnusedParameter.Local")]
        protected virtual bool CheckDownshift(
            Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
            PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
        {
            var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(base.GearshiftParams.TimeBetweenGearshifts);

            if (shiftTimeReached && IsBelowDownShiftCurve(gear, inTorque, inAngularVelocity)) {
				var next_gear = Gears.Predecessor(gear);

				if (!(next_gear.TorqueConverterLocked.Equals(true)) || !(IsAboveUpShiftCurve(next_gear, outTorque / GearboxModelData.Gears[next_gear.Gear].Ratio, outAngularVelocity * GearboxModelData.Gears[next_gear.Gear].Ratio, true))) {
					Downshift(absTime, gear);
					return true;
				}
            }

            if (shiftTimeReached && Container.DriverInfo.DrivingAction == DrivingAction.Accelerate) {
                if (Container.VehicleInfo.VehicleSpeed < Container.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed - 10.KMPHtoMeterPerSecond() &&
                    Container.DriverInfo.DriverAcceleration < 0.SI<MeterPerSquareSecond>()) {
					var currentGear = gear;
					var tmpResponseCurr = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, gear);
					
                    // var tmpResponseCurr = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
                    if (_gearbox.Gear > Gears.First()) {
						// var tmpGear = 
						
                        // clone current state of _nextgear, set gearbox state to lower gear, issue request, restore old gearbox state
                        // var tmp = _nextGear.Clone();
                        // var gbxState = new NextGearState(absTime, _gearbox);
                        // tmp.Gear = Gears.Predecessor(_gearbox.Gear);
                        // SetGear(tmp);
                        // var tmpResponseDs = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
                        // SetGear(gbxState);
						var tmpGear = Gears.Predecessor(currentGear);
						var tmpResponseDs = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tmpGear, true);
						
                        // done
                        if (tmpResponseDs.Engine.EngineSpeed.IsSmaller(Container.EngineInfo.EngineN95hSpeed) && tmpResponseDs.DeltaFullLoad - Formulas.InertiaPower(
                                tmpResponseDs.Engine.EngineSpeed, Container.EngineInfo.EngineSpeed, EngineInertia, dt) < tmpResponseCurr.DeltaFullLoad) {
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
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque,
			PerSecond origInAngularVelocity, GearshiftPosition currentGear, Second lastShiftTime, IResponse response1)
		{
			if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor *
															fld[currentGear.Gear]
																.DragLoadStationaryTorque(response1.Engine
																	.EngineSpeed))) {
				return null;
			}

			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			var fcCurrent = double.NaN;

			var current = currentGear;

			var vDrop = Container.DriverInfo.DriverAcceleration * GearshiftParams.ATLookAheadTime;
			var vehicleSpeedPostShift = (Container.VehicleInfo.VehicleSpeed + vDrop * GearshiftParams.VelocityDropFactor).LimitTo(
				0.KMPHtoMeterPerSecond(), Container.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed);

			var outAngularVelocityEst =
				(outAngularVelocity * vehicleSpeedPostShift / (Container.VehicleInfo.VehicleSpeed + Container.DriverInfo.DriverAcceleration * dt))
				.Cast<PerSecond>();
			var outTorqueEst = outTorque * outAngularVelocity / outAngularVelocityEst;

            var dtLow = dt.IsSmaller(Constants.SimulationSettings.TargetTimeInterval*0.8);
            var isGear1 = currentGear.Gear.Equals(1);
			var beforeReachingTargetSpeed = Container.DrivingCycleInfo.TargetSpeed.IsSmallerOrEqual(Container.VehicleInfo.VehicleSpeed + Container.DriverInfo.DriverAcceleration * dt) && Container.DriverInfo.DriverAcceleration.IsGreater(0.0);
			var rightIsStop = Container.DrivingCycleInfo.CycleData.RightSample.VehicleTargetSpeed.IsEqual(0.0);

            foreach (var next in Gears.IterateGears(Gears.Predecessor(currentGear), Gears.Predecessor(currentGear, (uint)GearshiftParams.AllowedGearRangeFC))) {
				
				if (dtLow && isGear1 && !beforeReachingTargetSpeed && rightIsStop) {
                    continue;
                }

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

				if (!(GearboxModelData.Gears[next.Gear].Ratio < GearshiftParams.RatioEarlyDownshiftFC)) {
					continue;
				}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, next);

				var inAngularVelocity = GearboxModelData.Gears[next.Gear].Ratio * outAngularVelocity;
				var inTorque = response.Engine.PowerRequest / inAngularVelocity;

				if (!IsAboveUpShiftCurve(next, inTorque, inAngularVelocity, next.TorqueConverterLocked.Value)) {
					if (double.IsNaN(fcCurrent)) {
						var responseCurrent =
							RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, current);
						fcCurrent = GetFCRating(responseCurrent.Engine.EngineSpeed,
							responseCurrent.Engine.TorqueOutDemand.LimitTo(
								fld[currentGear.Gear].DragLoadStationaryTorque(responseCurrent.Engine.EngineSpeed),
								fld[currentGear.Gear].FullLoadStationaryTorque(responseCurrent.Engine.EngineSpeed)));
					}

					var fcNext = GetFCRating(response.Engine.EngineSpeed, response.Engine.TorqueOutDemand.LimitTo(
						fld[next.Gear].DragLoadStationaryTorque(response.Engine.EngineSpeed),
						fld[next.Gear].FullLoadStationaryTorque(response.Engine.EngineSpeed)));

					if (fcNext.IsSmaller(fcCurrent * GearshiftParams.RatingFactorCurrentGear) &&
						fcNext.IsSmaller(minFc)) {
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


		#endregion

        protected virtual void Upshift(Second absTime, GearshiftPosition gear)
		{
			if (!Gears.HasSuccessor(gear)) {
				throw new VectoSimulationException(
					"ShiftStrategy wanted to shift up but no higher gear available.");
			}

			_nextGear.SetState(absTime, false, Gears.Successor(gear));

		}

		protected virtual void Downshift(Second absTime, GearshiftPosition gear)
		{
			if (!Gears.HasPredecessor(gear)) {
				throw new VectoSimulationException(
					"ShiftStrategy wanted to shift down but no lower gear available.");
			}
			_nextGear.SetState(absTime, false, Gears.Predecessor(gear));
		}

		// protected virtual void SetGear(NextGearState gbxState)
		// {
		// 	_gearbox.Gear = gbxState.Gear;
		// 	//_gearbox.TorqueConverterLocked = gbxState.TorqueConverterLocked;
		// 	_gearbox.Disengaged = gbxState.Disengaged;
		// }

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

		/// <summary>
		/// Tests if the operating point is below (left of) the down-shift curve.
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the down-shift curv; otherwise, <c>false</c>.</returns>
		protected override bool IsBelowDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			return !Gears.First().Equals(gear) && GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
		}

        protected virtual void ShiftGear(Second absTime, GearshiftPosition currentGear, GearshiftPosition nextGear)
		{
			if (currentGear.TorqueConverterLocked != nextGear.TorqueConverterLocked && currentGear.Gear != nextGear.Gear) {
				throw new VectoException(
					"skipping gear from converter to locked not allowed! {0} -> {1}", currentGear.Name, nextGear.Name);
			}

			_nextGear.SetState(absTime, disengaged: false, gear: nextGear);
		}


		protected ResponseDryRun RequestDryRunWithGear(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition gear, bool updateComponents = false)
		{
			TestPowertrain.Gearbox.SetDisengaged = false;
			TestPowertrain.Gearbox.SetGear = gear;

			TestPowertrain.Container.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			if (updateComponents)
				TestPowertrain.UpdateComponents();
			var response = (ResponseDryRun)TestPowertrain.Container.GearboxOutPort.Request(
				0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			return response;
		}

		protected MeterPerSquareSecond EstimateAcceleration(
			PerSecond gbxOutSpeed, NewtonMeter gbxOutTorque, PerSecond tcInSpeed, NewtonMeter tcInTorque, uint currentGear, IResponse response)
		{
			var vehicleSpeed = Container.VehicleInfo.VehicleSpeed;
			var avgSlope =
				((Container.DrivingCycleInfo.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
				Container.DrivingCycleInfo.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = Container.VehicleInfo.AirDragResistance(vehicleSpeed, vehicleSpeed).AirdragForce * Container.VehicleInfo.VehicleSpeed;
			var rollResistanceLoss = Container.VehicleInfo.RollingResistance(avgSlope) * Container.VehicleInfo.VehicleSpeed;

			var slopeLoss = Container.VehicleInfo.SlopeResistance(avgSlope) * Container.VehicleInfo.VehicleSpeed;
			var axleLoss = Container.AxlegearInfo.AxlegearLoss();

			var tcLossesCurrentGear = tcInSpeed * tcInTorque - gbxOutSpeed * gbxOutTorque;

			var nextTcOutSpeed = gbxOutSpeed * GearboxModelData.Gears[currentGear + 1].TorqueConverterRatio;

			var tcNext = GearboxModelData.TorqueConverterData.LookupOperatingPointOut(
				nextTcOutSpeed, response.Engine.EngineSpeed, response.Engine.TorqueOutDemand);
			var tcLossesNextGear = tcNext.InAngularVelocity * tcNext.InTorque - tcNext.OutAngularVelocity * tcNext.OutTorque;
			var deltaTcLosses = tcLossesNextGear - tcLossesCurrentGear;

			var accelerationPower = gbxOutSpeed * gbxOutTorque - deltaTcLosses - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;

			var acceleration = accelerationPower / Container.VehicleInfo.VehicleSpeed / (Container.VehicleInfo.TotalMass + Container.WheelsInfo.ReducedMassWheels);

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

			// public NextGearState(Second absTime, IAPTGearbox gearbox)
			// {
			// 	SetState(absTime, gearbox);
			// }

			public void SetState(Second absTime, bool disengaged, GearshiftPosition gear)
			{
				AbsTime = absTime;
				Disengaged = disengaged;
				Gear = gear;
			}

			// public void SetState(Second absTime, IAPTGearbox gearbox)
			// {
			// 	AbsTime = absTime;
			// 	Disengaged = gearbox.Disengaged;
			// 	Gear = gearbox.Gear;
			// }
			//
			// public NextGearState Clone()
			// {
			// 	return new NextGearState(this);
			// }
		}
    }
}
