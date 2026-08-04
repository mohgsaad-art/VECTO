using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
	public class FCHVAPTNShiftStrategy : APTNShiftStrategy
	{
		public new const string Name = "APT-N - EffShift (FCHV)";

		public FCHVAPTNShiftStrategy(IVehicleContainer container) : base(container, VectoSimulationJobType.FCHV, false) { }
	}


    public class APTNShiftStrategy : PEVAMTShiftStrategy
	{
		public new const string Name = "APT-N - EffShift (BEV)";
		
		public APTNShiftStrategy(IVehicleContainer container) : this(container, VectoSimulationJobType.BatteryElectricVehicle, false)
		{}

		public APTNShiftStrategy(IVehicleContainer container, VectoSimulationJobType jobType, bool dummy) : base(container, jobType,  false)
		{
			if (container.RunData.VehicleData == null) {
				return;
			}

			if (!container.IsTestPowertrain) {
				SetupVelocityDropPreprocessor(container.SimplePowertrainBuilder);
			}
		}

		public override bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
            PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
        {
            var shiftAllowed = !dt.IsSmaller(Constants.SimulationSettings.TargetTimeInterval / 10);
			return shiftAllowed && base.ShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, response) ;
        }
	}
}