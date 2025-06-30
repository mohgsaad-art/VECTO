using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
    public interface IElectricMotor : IPowerTrainComponent, ITnOutPort, IElectricMotorInfo, IUpdateable
	{
		void Connect(IElectricSystem powersupply);

		IBusAuxiliariesAdapter BusAux { set; }
	}

	public interface ITestpowertrainElectricMotor : IElectricMotor, ITnOutPort
	{
		IElectricMotorControl Control { get; }
		IElectricSystem GetElectricSystem { get; }
		ElectricMotorState GetPreviousState { get; }
		Joule SetThermalBuffer { set; }
		bool SetDeRatingActive { set; }
		NewtonMeter ConvertEmTorqueToDrivetrain(PerSecond emSpeed, NewtonMeter tq, bool dryRun);
		PerSecond ConvertEmSpeedToDrivetrain(PerSecond emSpeed);
	}

	public interface ITestPowertrainElectricMotorControl : IElectricMotorControl
	{
		bool EmOff { get; set; }
	}
}