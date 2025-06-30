using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class HybridCtlElectricMotorController : IElectricMotorControl
	{
		protected IHybridControllerInternal _controller;
		protected ElectricMotorData ElectricMotorData;

		public HybridCtlElectricMotorController(IHybridControllerInternal hybridController, ElectricMotorData motorData)
		{
			_controller = hybridController;
			ElectricMotorData = motorData;
		}

		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity,
			NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque, PowertrainPosition position, bool dryRun)
		{
			return _controller.MechanicalAssistPower(position, absTime, dt, outTorque, prevOutAngularVelocity,
				currOutAngularVelocity, dryRun);
		}
	}
}