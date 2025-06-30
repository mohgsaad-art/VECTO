using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
    public class HybridCtlShiftStrategy : BaseShiftStrategy, IHybridControlShiftStrategy
    {
        protected IHybridControllerInternal Controller;

        protected ITestPowertrain TestPowertrain;

		protected IGearbox _gearbox;

        public HybridCtlShiftStrategy(IHybridControllerInternal hybridController, IVehicleContainer container) : base(
            container)
        {
            Controller = hybridController;

            if (RunData?.EngineData == null) {
                return;
            }

            var transmissionRatio = RunData.AxleGearData.AxleGear.Ratio *
                                    (RunData.AngledriveData?.Angledrive.Ratio ?? 1.0) /
                                    RunData.VehicleData.DynamicTyreRadius;
            var minEngineSpeed = (RunData.EngineData.FullLoadCurves[0].RatedSpeed - RunData.EngineData.IdleSpeed) *
                Constants.SimulationSettings.ClutchClosingSpeedNorm + RunData.EngineData.IdleSpeed;
            MaxStartGear = Gears.First();
            foreach (var gear in Gears.Reverse()) {
                var gearData = GearboxModelData.Gears[gear.Gear];
                if (gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value) {
                    continue;
                }
                if (GearshiftParams.StartSpeed * transmissionRatio * gearData.Ratio <= minEngineSpeed)
                    continue;
                MaxStartGear = gear;
                break;
            }

			TestPowertrain = PowertrainBuilder.CreateTestPowertrain(Container, true);
        }

		public override IGearbox Gearbox {
			get => _gearbox;
			set {
				if (value is IAMTGearbox || value is MeasuredSpeedHybridsGearbox) {
					_gearbox = value;
					return;
				}
				throw new VectoException("This shift strategy can't handle gearbox of type {0}, expected {1} or {2}", value.GetType().Name, nameof(IAMTGearbox), nameof(MeasuredSpeedHybridsGearbox));
			}
		}

        protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
            PerSecond outAngularVelocity, NewtonMeter inTorque,
            PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
        {
            if (Controller.ShiftRequired) {
                _nextGear = Controller.NextGear;
            }

            return Controller.ShiftRequired;
        }

        #region Overrides of BaseShiftStrategy

        public override void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            base.Request(absTime, dt, outTorque, outAngularVelocity);
            if (Container.DriverInfo.DrivingAction == DrivingAction.Halt) {
                _nextGear = MaxStartGear;
            }
        }

        #endregion

        public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter outTorque,
            PerSecond outAngularVelocity)
        {
            if (Container.VehicleInfo.VehicleSpeed.IsEqual(0)) {
                return InitStartGear(absTime, outTorque, outAngularVelocity);
            }

            foreach (var entry in Gears.Reverse()) {
                var gear = entry;
                //for (var gear = (uint)GearboxModelData.Gears.Count; gear > 1; gear--) {

                TestPowertrain.UpdateComponents();
                TestPowertrain.Gearbox.SetGear = gear;
                TestPowertrain.Gearbox.SetNextGear = gear;
                if (Controller.CurrentStrategySettings != null) {
                    TestPowertrain.HybridController.ApplyStrategySettings(Controller.CurrentStrategySettings);
                }
                var response = TestPowertrain.Gearbox.Initialize(outTorque, outAngularVelocity);
                response = TestPowertrain.Gearbox.Request(absTime,
                    Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval, outTorque, outAngularVelocity,
                    true);

                var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
                var fullLoadPower = TestPowertrain.CombustionEngine.EngineStationaryFullPower(response.Engine.EngineSpeed);
                var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
                var inTorque = response.Clutch.PowerRequest / inAngularSpeed;

                // if in shift curve and torque reserve is provided: return the current gear
                if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) &&
                    !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
                    reserve >= GearshiftParams.StartTorqueReserve) {
                    if ((inAngularSpeed - Container.EngineInfo.EngineIdleSpeed) /
                        (Container.EngineInfo.EngineRatedSpeed - Container.EngineInfo.EngineIdleSpeed) <
                        Constants.SimulationSettings.ClutchClosingSpeedNorm && Gears.HasPredecessor(gear)) {
                        gear = Gears.Predecessor(gear);
                    }

                    _nextGear = gear;
                    return gear;
                }

                // if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
                if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && Gears.HasSuccessor(gear)) {
                    _nextGear = gear;
                    return Gears.Successor(gear);
                }
            }

            // fallback: return first gear
            _nextGear = Gears.First();
            return _nextGear;
        }

        protected virtual GearshiftPosition InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
			foreach (var gear in Gears.IterateGears(MaxStartGear, Gears.First())) {
                //for (var gear = MaxStartGear; gear > 1; gear--) {
                var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

                var ratedSpeed = Container.EngineInfo.EngineRatedSpeed;
                if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
                    continue;
                }
				
				TestPowertrain.UpdateComponents();

                TestPowertrain.Gearbox.SetGear = gear;
                TestPowertrain.Gearbox.SetNextGear = gear;
                if (Controller.CurrentStrategySettings != null) {
                    TestPowertrain.HybridController.ApplyStrategySettings(Controller.CurrentStrategySettings);
                }

                TestPowertrain.CombustionEngine.CombustionEngineOn = true;

                var response = TestPowertrain.Gearbox.Initialize(outTorque, outAngularVelocity);
                response = TestPowertrain.Gearbox.Request(absTime,
                    Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval, outTorque, outAngularVelocity,
                    true);

                var fullLoadPower =
                    response.Engine.DynamicFullLoadTorque; //EnginePowerRequest - response.DeltaFullLoad;
                var reserve = 1 - response.Engine.TorqueOutDemand / fullLoadPower;

                if (RunData != null && RunData.HybridStrategyParameters.MaxPropulsionTorque?.GetVECTOValueOrDefault(gear) != null) {
                    var tqRequest = response.Gearbox.InputTorque;
                    var maxTorque = RunData.HybridStrategyParameters.MaxPropulsionTorque[gear].FullLoadDriveTorque(response.Gearbox.InputSpeed);
                    reserve = 1 - VectoMath.Min(response.Engine.TorqueOutDemand / fullLoadPower, tqRequest / maxTorque);
                }

                if (response.Engine.EngineSpeed > Container.EngineInfo.EngineIdleSpeed &&
                    reserve.IsGreaterOrEqual(0)) {
                    //reserve >= GearshiftParams.StartTorqueReserve) {
                    _nextGear = gear;
                    return gear;
                }
            }

            _nextGear = Gears.First();
            return _nextGear;
        }

        public override GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            if (Container.EngineCtl.CombustionEngineOn) {
                while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear, outAngularVelocity)) {
                    _nextGear = Gears.Predecessor(_nextGear);
                }
				
                while (Gears.HasSuccessor(_nextGear) && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
                    _nextGear = Gears.Successor(_nextGear);
                }
            }

            return _nextGear;
        }

        public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque,
            PerSecond outAngularVelocity)
        {
            if (!Controller.ShiftRequired && !Controller.CurrentStrategySettings.GearboxInNeutral && Container.DriverInfo.DrivingAction != DrivingAction.Halt) {
                // gearbox disengaged on its own! set next gear!
                var gear = _nextGear;
                while (Gears.HasPredecessor(gear) && SpeedTooLowForEngine(gear, outAngularVelocity)) {
                    gear = Gears.Predecessor(gear);
                }

                while (Gears.HasSuccessor(gear) && SpeedTooHighForEngine(gear, outAngularVelocity)) {
                    gear = Gears.Successor(gear);
                }

                _nextGear = gear;
            }
        }

        public void SetNextGear(GearshiftPosition nextGear) => _nextGear = nextGear;
    }
}