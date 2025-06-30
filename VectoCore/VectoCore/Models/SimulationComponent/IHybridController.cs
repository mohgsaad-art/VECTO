using System.Collections;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
    public interface IHybridController : IPowerTrainComponent, IHybridControllerInfo, IHybridControllerCtl
	{
		IShiftStrategy ShiftStrategy { get; }

		SimpleComponentState PreviousState { get; }

		IHybridControlledGearbox Gearbox { set; }

		ICombustionEngine Engine { set; }

		IElectricMotorControl ElectricMotorControl(PowertrainPosition pos);
		
		void AddElectricMotor(PowertrainPosition pos, ElectricMotorData motorDataItem2);

	}
}