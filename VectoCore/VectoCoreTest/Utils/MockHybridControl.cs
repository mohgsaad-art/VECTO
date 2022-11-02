using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Tests.Utils {
	public class MockHybridControl : IElectricMotorControl
	{
		public NewtonMeter ElectricShare { get; set; }

		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond prevOutAngularVelocity, PerSecond curOutAngularVelocity,
			NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
			PowertrainPosition position, bool dryRun)
		{
			return ElectricShare;
		}

		public NewtonMeter MaxDriveTorque(PerSecond avgSpeed, Second dt)
		{
			return -100000.SI<Watt>() / avgSpeed;
		}

		public NewtonMeter MaxDragTorque(PerSecond avgSpeed, Second dt)
		{
			return 100.SI<NewtonMeter>();
		}
	}
}