using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
	public class FCHVAPTNShiftStrategy : APTNShiftStrategy
	{
		public new const string Name = "APT-N - EffShift (FCHV)";

		public FCHVAPTNShiftStrategy(IVehicleContainer container, IPowertrainContainer powertrainContainer)
			: base(container, powertrainContainer, container.HasMultiplePowertrains ? VectoSimulationJobType.Multiple_FCHV : VectoSimulationJobType.FCHV , false) { }
	}


    public class APTNShiftStrategy : PEVAMTShiftStrategy
	{
		public new const string Name = "APT-N - EffShift (BEV)";
		
		public APTNShiftStrategy(IVehicleContainer container, IPowertrainContainer powertrainContainer) : this(container,
			powertrainContainer, 
			container.HasMultiplePowertrains ? VectoSimulationJobType.Multiple_PEV : VectoSimulationJobType.BatteryElectricVehicle, false)
		{}

		public APTNShiftStrategy(IVehicleContainer container, IPowertrainContainer powertrainContainer, VectoSimulationJobType jobType, bool dummy) : base(container, powertrainContainer, jobType,  false)
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