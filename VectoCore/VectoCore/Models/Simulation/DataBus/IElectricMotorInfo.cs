using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public interface IElectricMotorInfo : IHasAxleNumber
	{
		//NewtonMeter ElectricDragTorque(PerSecond electricMotorSpeed, Second simulationInterval, DrivingBehavior drivingBehavior);

		PerSecond ElectricMotorSpeed { get; }
		NewtonMeter ElectricMotorTorque { get; }
		PowertrainPosition Position { get; }
		PerSecond MaxSpeedDt { get; }
		PerSecond RatedSpeedDt { get; }
		Watt DragPower(Volt volt, PerSecond electricMotorSpeed, GearshiftPosition gear);
		Watt MaxPowerDrive(Volt volt, PerSecond inAngularVelocity, GearshiftPosition gear);
		NewtonMeter GetTorqueForElectricPower(Volt volt, Watt electricPower, PerSecond avgEmSpeed, Second dt, GearshiftPosition gear, bool allowExtrapolation);

		bool EmOffPrev { get; }
		bool? EmOffCurr { get; }
		bool DeRatingActive { get; }
		
	}
}