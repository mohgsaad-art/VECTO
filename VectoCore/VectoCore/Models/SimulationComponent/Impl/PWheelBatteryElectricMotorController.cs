using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class PWheelBatteryElectricMotorController : BatteryElectricMotorController
    {
        public PWheelBatteryElectricMotorController(VehicleContainer container, ElectricSystem es) : base(container, es)
        {}

        protected override bool CannotProvideMechanicalAssistAtLowSpeed(NewtonMeter outTorque)
        {
            return false;
        }
    }
}
