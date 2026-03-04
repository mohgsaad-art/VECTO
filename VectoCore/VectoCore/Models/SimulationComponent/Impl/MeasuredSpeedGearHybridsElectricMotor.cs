using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class MeasuredSpeedGearHybridsElectricMotor : ElectricMotor
    {
        public MeasuredSpeedGearHybridsElectricMotor(IVehicleContainer container, IPowertrainContainer powertrainContainer, ElectricMotorData data, IElectricMotorControl control, 
            PowertrainPosition position) : base(container, powertrainContainer, data, control, position, false)
        {}

        public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun = false)
        {
            if (TransmissionRatioPerGear == null) {
				return DoHandleRequest(absTime, dt, outTorque, outAngularVelocity, dryRun, 1.0);
			}

			var gearbox = DataBus.GearboxesInfo.First(x => x.AxleNumber == AxleNumber);
			var gear = gearbox.Gear;
			if (gear.Gear == 0) {
				gear = gearbox.NextGear;
			}

			var ratio = (gear.Gear > 0) ? TransmissionRatioPerGear[gear.Gear - 1] : 1;

			return DoHandleRequest(absTime, dt, outTorque * ratio, outAngularVelocity / ratio, dryRun, ratio);
        }
    }
}
