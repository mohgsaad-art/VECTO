using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IHybridControllerInfo
	{
		GearshiftPosition SelectedGear { get; }
		
		PerSecond ICESpeed { get; }
		bool GearboxEngaged { get; }


		Second SimulationInterval { get; }

		PerSecond ElectricMotorSpeed(PowertrainPosition pos);

		//IList<PowertrainPosition> ElectricMotors { get; }

		//NewtonMeter ElectricMotorTorque(PowertrainPosition pos);
	}

	public interface IHybridControllerCtl
	{
		void RepeatDrivingAction(Second absTime);

		IHybridControlStrategy Strategy { get; }
	}
}