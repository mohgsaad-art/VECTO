using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IElectricMachinesDataAdapter
    {
        IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(
            IElectricMachinesDeclarationInputData electricMachines,
            IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage, GearList gearlist = null);

		List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCDeclarationInputData iepc,
			Volt averageVoltage);
	}
}