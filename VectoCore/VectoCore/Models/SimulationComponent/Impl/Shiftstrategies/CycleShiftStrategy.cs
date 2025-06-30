using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
    public class CycleShiftStrategy : BaseShiftStrategy
    {
		protected IGearbox _gearbox;

        public CycleShiftStrategy(IVehicleContainer container) : base(container) { }

		public override IGearbox Gearbox {
			get => _gearbox;
			set {
				if (value is CycleGearbox) {
					_gearbox = value;
					return;
				}
				throw new VectoException("This shift strategy can't handle gearbox of type {0}, expected {1}", value.GetType().Name, nameof(CycleGearbox));
			}
		}

        protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
            PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
            Second lastShiftTime, IResponse response)
        {
            return false;
        }

        public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
        {
            throw new NotImplementedException();
        }

        public override GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            throw new NotImplementedException();
        }

        public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            throw new NotImplementedException();
        }

        public override GearshiftPosition NextGear => throw new NotImplementedException();


    }
}