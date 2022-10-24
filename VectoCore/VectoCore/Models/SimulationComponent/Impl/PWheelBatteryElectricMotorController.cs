using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class PWheelBatteryElectricMotorController : BatteryElectricMotorController
    {
        public PWheelBatteryElectricMotorController(VehicleContainer container, ElectricSystem es) : base(container, es)
        {}

        protected override bool CannotProvideRecuperationAtLowSpeed(NewtonMeter outTorque)
        {
           return DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed.IsSmallerOrEqual(
				GearboxModelData?.DisengageWhenHaltingSpeed ?? Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed)
					&& outTorque.IsSmaller(0);
        }
    }
}
