using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class PWheelBatteryElectricMotorController : BatteryElectricMotorController
    {
        public PWheelBatteryElectricMotorController(IVehicleContainer container,
            IPowertrainContainer powertrainContainer, IElectricSystem es) : base(container, powertrainContainer, es)
        {
            
        }

        protected override bool CannotProvideRecuperationAtLowSpeed(NewtonMeter outTorque)
        {
           return DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed.IsSmallerOrEqual(
				GearboxModelData?.DisengageWhenHaltingSpeed ?? Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed)
					&& outTorque.IsSmaller(0);
        }
    }
}
