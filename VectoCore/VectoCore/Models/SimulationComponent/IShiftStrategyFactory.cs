using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IShiftStrategyFactory
	{
		IShiftStrategy GetShiftStrategy(string name, IVehicleContainer container,
			IPowertrainContainer powertrainContainer);

		string GetShiftStrategyName(GearboxType gearboxType, VectoSimulationJobType jobType, bool batteryOnlyHybridMode);

		IShiftPolygonCalculator CreateShiftPolygonCalculator(string shiftStrategyName, ShiftStrategyParameters shiftParams);

	}

	public interface IInternalShiftStrategyFactory
	{
		IShiftStrategy CreateShiftStrategy(string name, IVehicleContainer container,
			IPowertrainContainer powertrainContainer);

		IShiftPolygonCalculator CreateShiftPolygonCalculator(string shiftStrategyName,
			ShiftStrategyParameters shiftParams);

	}
}