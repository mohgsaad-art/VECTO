using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
    public class HybridCtlIHPCShiftStrategy : BaseShiftStrategy, IHybridControlShiftStrategy
    {
		protected IHybridControllerInternal Controller;

		protected ITestPowertrain TestPowertrain;

		protected APTNGearbox _gearbox;

        public HybridCtlIHPCShiftStrategy(IHybridControllerInternal hybridController, IVehicleContainer container) :
			base(container)
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
				if (value is APTNGearbox gbx) {
					_gearbox = gbx;
					return;
				}
				throw new VectoException("This shift strategy can't handle gearbox of type {0}, expected {1}", value.GetType().Name, nameof(APTNGearbox));
			}
		}

        public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
        {
            if (Container.VehicleInfo.VehicleSpeed.IsEqual(0)) {
                return InitStartGear(absTime, torque, outAngularVelocity);
            }

            foreach (var gear in Gears.Reverse()) {
                TestPowertrain.UpdateComponents();
                TestPowertrain.Gearbox.SetGear = gear;
                TestPowertrain.Gearbox.SetNextGear = gear;
                if (Controller.CurrentStrategySettings != null) {
                    TestPowertrain.HybridController.ApplyStrategySettings(Controller.CurrentStrategySettings);
                }

                var response = TestPowertrain.Gearbox.Initialize(torque, outAngularVelocity);
                response = TestPowertrain.Gearbox.Request(absTime,
                    Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval, torque, outAngularVelocity,
                    true);
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

        protected virtual GearshiftPosition InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            if (!Container.EngineCtl.CombustionEngineOn) {
                return _nextGear;
            }

            foreach (var gear in Gears.IterateGears(MaxStartGear, Gears.First())) {
                //for (var gear = MaxStartGear; gear > 1; gear--) {
                var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

                var ratedSpeed = Container.EngineInfo.EngineRatedSpeed;
                if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
                    continue;
                }

                //var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);
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
			var tmpGear = new GearshiftPosition(_nextGear.Gear, false);
			if (Container.EngineCtl.CombustionEngineOn) {
				//  GX -> 0: disengage before halting
				var vehicleSpeed = Container.VehicleInfo.VehicleSpeed + Container.DriverInfo.DriverAcceleration * dt;
				var isSlowerThanDisengageSpeed = vehicleSpeed.IsSmaller(GearboxModelData.DisengageWhenHaltingSpeed);
				var isNegativeTorque = outTorque.IsSmaller(0);
				var isBraking = Container.DriverInfo.DriverBehavior == DrivingBehavior.Braking;
				var disengageBeforeHalting = isBraking && isSlowerThanDisengageSpeed && isNegativeTorque;

				if (disengageBeforeHalting) {
					//if (_gearbox != null) {
					//	_gearbox.Disengaged = true;
					//	return tmpGear;
					//}
				}

				while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear, outAngularVelocity)) {
					_nextGear = Gears.Predecessor(_nextGear);
				}

				while (Gears.HasSuccessor(_nextGear) && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
					_nextGear = Gears.Successor(_nextGear);
				}
			}

			return _nextGear;
		}

        protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response) =>
			false;

        public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

		public void SetNextGear(GearshiftPosition nextGear) => _nextGear = nextGear;
    }
}