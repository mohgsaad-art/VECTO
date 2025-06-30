using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc
{
    public class AMTShiftStrategyPolygonCalculator : IShiftPolygonCalculator
    {
        public ShiftPolygon ComputeDeclarationShiftPolygon(
            GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, IList<ITransmissionInputData> gearboxGears,
            CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
        {
            return DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(
                i, engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);
        }

        public ShiftPolygon ComputeDeclarationExtendedShiftPolygon(
            GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, IList<ITransmissionInputData> gearboxGears,
            CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
        {
            return DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygonExtended(
                i, engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);
        }

		public ShiftPolygon ComputeElectricMotorDeclarationShiftPolygon(GearboxType gearboxType, int i,
			IList<ITransmissionInputData> gearboxGears,
			double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData, ElectricMotorData emDataLimited)
		{
			throw new System.NotImplementedException("Not applicable for PEV vehicles");
		}
	}
}