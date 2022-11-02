using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public interface IElectricMotorInfo
	{
		//NewtonMeter ElectricDragTorque(PerSecond electricMotorSpeed, Second simulationInterval, DrivingBehavior drivingBehavior);

		PerSecond ElectricMotorSpeed { get; }
		PowertrainPosition Position { get; }
		PerSecond MaxSpeed { get; }
		Watt DragPower(Volt volt, PerSecond electricMotorSpeed, GearshiftPosition gear);
		Watt MaxPowerDrive(Volt volt, PerSecond inAngularVelocity, GearshiftPosition gear);
		NewtonMeter GetTorqueForElectricPower(Volt volt, Watt electricPower, PerSecond avgEmSpeed, Second dt, GearshiftPosition gear, bool allowExtrapolation);

		bool DeRatingActive { get; }
	}
}