using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{

    public class HybridCtlATShiftStrategy : BaseShiftStrategy, IHybridControlShiftStrategy
    {
		protected APTGearbox _gearbox;


        public HybridCtlATShiftStrategy(IHybridControllerInternal hybridController, IVehicleContainer container) :
			base(container)
		{ }

        public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
        {
            if (Container.VehicleInfo.VehicleSpeed.IsEqual(0)) {
                // AT always starts in first gear and TC active!
                _gearbox.Disengaged = true;
                return base.Gears.First();
            }

            foreach (var gear in base.Gears.Reverse()) {
                var response = _gearbox.Initialize(gear, torque, outAngularVelocity);

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
            return base.Gears.First();
        }

		public override IGearbox Gearbox {
			get => _gearbox;
			set {
				if (value is APTGearbox gbx) {
					_gearbox = gbx;
					return;
				}
				throw new VectoException("This shift strategy can't handle gearbox of type {0}, expected {1}", value.GetType().Name, nameof(APTGearbox));
			}
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
                    if (_gearbox != null) {
                        _gearbox.Disengaged = true;
                        return tmpGear;
                    }
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

        public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) =>
            throw new NotImplementedException("AT Shift Strategy does not support disengaging.");

        protected override bool SpeedTooLowForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
        {
            if (gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value) {
                return false;
            }

            return base.SpeedTooLowForEngine(gear, outAngularSpeed);
        }

        protected override bool SpeedTooHighForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
        {
            if (gear.TorqueConverterLocked.HasValue && !gear.TorqueConverterLocked.Value) {
                return false;
            }

            return base.SpeedTooHighForEngine(gear, outAngularSpeed);
        }

		public void SetNextGear(GearshiftPosition nextGear) => _nextGear = nextGear;
    }
}