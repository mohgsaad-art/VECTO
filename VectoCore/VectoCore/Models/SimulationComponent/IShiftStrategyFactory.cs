using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IShiftStrategyFactory
	{
		IShiftStrategy GetShiftStrategy(string name, IVehicleContainer container);

		string GetShiftStrategyName(GearboxType gearboxType, VectoSimulationJobType jobType);

		IShiftPolygonCalculator CreateShiftPolygonCalculator(string shiftStrategyName, ShiftStrategyParameters shiftParams);

	}

	public interface IInternalShiftStrategyFactory
	{
		IShiftStrategy CreateShiftStrategy(string name, IVehicleContainer container);

		IShiftPolygonCalculator CreateShiftPolygonCalculator(string shiftStrategyName,
			ShiftStrategyParameters shiftParams);

	}
}