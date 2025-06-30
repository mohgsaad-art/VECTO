using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent
{

	public interface IShiftPolygonCalculator
	{
		ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null);

		ShiftPolygon ComputeDeclarationExtendedShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null);

		ShiftPolygon ComputeElectricMotorDeclarationShiftPolygon(GearboxType gearboxType, int i,
			IList<ITransmissionInputData> gearboxGears, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData, ElectricMotorData emDataLimited);

	}
}