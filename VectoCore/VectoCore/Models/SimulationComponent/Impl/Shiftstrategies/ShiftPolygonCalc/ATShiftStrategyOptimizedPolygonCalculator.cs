using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc
{

	public class ATShiftStrategyOptimizedPolygonCalculator : IShiftPolygonCalculator
	{
		public virtual ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
			Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
		{
			var shiftLine = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
				Math.Max(i, 2), engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);

			var upshift = new List<ShiftPolygon.ShiftPolygonEntry>();

			if (i < gearboxGears.Count - 1) {
				var maxDragTorque = engineDataFullLoadCurve.MaxDragTorque * 1.1;
				var maxTorque = engineDataFullLoadCurve.MaxTorque * 1.1;

				var speed = engineData.FullLoadCurves[0].NP98hSpeed / gearboxGears[i].Ratio * gearboxGears[i + 1].Ratio;

				upshift.Add(new ShiftPolygon.ShiftPolygonEntry(maxDragTorque, speed));
				upshift.Add(new ShiftPolygon.ShiftPolygonEntry(maxTorque, speed));
			}

			return new ShiftPolygon(shiftLine.Downshift.ToList(), upshift);
		}

		public virtual ShiftPolygon ComputeDeclarationExtendedShiftPolygon(
			GearboxType gearboxType,
			int i,
			EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears,
			CombustionEngineData engineData,
			double axlegearRatio,
			Meter dynamicTyreRadius,
			ElectricMotorData electricMotorData = null)
		{
			throw new NotImplementedException("Not applicable to AT transmissions.");
		}

		public ShiftPolygon ComputeElectricMotorDeclarationShiftPolygon(GearboxType gearboxType, int i,
			IList<ITransmissionInputData> gearboxGears,
			double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData, ElectricMotorData emDataLimited)
		{
			throw new NotImplementedException("Not applicable for PEV vehicles");
		}
	}

}