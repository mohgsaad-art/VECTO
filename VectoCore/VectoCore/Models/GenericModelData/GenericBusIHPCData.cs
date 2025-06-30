using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericBusIHPCData : GenericBusEMBase
	{
		// public const double GenericGearEfficiency = 0.925; //Considered in gearbox

		public GenericBusIHPCData()
		{
			GenericEfficiencyMap_ASM =
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_ASM_normalized.vmap";

			GenericEfficiencyMap_PSM =
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_PSM_normalized.vmap";
		}
		

		public ElectricMotorData CreateGenericBusIHPCData(
			ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineEntry,
			ElectricMachineType electricMachineType, IGearboxDeclarationInputData gearboxData)
		{
			var electricMotorData = new ElectricMotorData {
				EfficiencyData = GetVoltageLevels(electricMachineEntry, electricMachineType, gearboxData),
				Inertia = electricMachineEntry.ElectricMachine.Inertia * electricMachineEntry.Count,
				RatioADC = electricMachineEntry.RatioADC
			};

			return electricMotorData;
		}
		

		private VoltageLevelData GetVoltageLevels(
			ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineEntry,
			ElectricMachineType electricMachineType, IGearboxDeclarationInputData gearboxData)
		{
			var voltageLevels = electricMachineEntry.ElectricMachine.VoltageLevels;
			var count = electricMachineEntry.Count;
			var normalizedMap = GetNormalizedEfficiencyMap(electricMachineType);

			return new VoltageLevelData {
				VoltageLevels = GetElectricMotorVoltageLevelData(voltageLevels, count, normalizedMap, gearboxData)
			};
		}


		private List<ElectricMotorVoltageLevelData> GetElectricMotorVoltageLevelData(
			IList<IElectricMotorVoltageLevel> voltageLevels, int count, TableData normalizedMap, IGearboxDeclarationInputData gearboxData)
		{
			var result = new List<ElectricMotorVoltageLevelData>();

			foreach (var voltageLevel in voltageLevels) {

				var ratedPoint = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtEM(voltageLevels[0].FullLoadCurve.First().LoadCurve);
				
				var ihpcVoltageLevel = new IHPCVoltageLevelData {
					Voltage = voltageLevel.VoltageLevel,
					FullLoadCurve = GetElectricMotorFullLoadCurve(voltageLevel.FullLoadCurve.First().LoadCurve),
					EfficiencyMaps = GetEfficiencyMaps(ratedPoint, normalizedMap, gearboxData,count)
				};

				result.Add(ihpcVoltageLevel);
			}
			
			return result;
		}


		private Dictionary<uint, EfficiencyMap> GetEfficiencyMaps(RatedPoint ratedPoint, TableData normalizedMap,
			IGearboxDeclarationInputData gearboxData, int count)
		{
			var result = new Dictionary<uint, EfficiencyMap>();

			var efficiencyMap = DeNormalizeData(normalizedMap, ratedPoint);

			foreach (var gearData in gearboxData.Gears.OrderBy(x => x.Gear)) {
				result.Add((uint)gearData.Gear, ElectricMotorMapReader.Create(efficiencyMap, count, ExecutionMode.Declaration));
			}

			return result;
		}
		

		private ElectricMotorFullLoadCurve GetElectricMotorFullLoadCurve(TableData fullLoadCurve)
		{
			var entries = new List<ElectricMotorFullLoadCurve.FullLoadEntry>();

			foreach (DataRow row in fullLoadCurve.Rows)
			{
				entries.Add(new ElectricMotorFullLoadCurve.FullLoadEntry
				{
					MotorSpeed = row.ParseDouble("outShaftSpeed").SI<PerSecond>(),
					FullGenerationTorque = row.ParseDouble("minTorque").SI<NewtonMeter>(),
					FullDriveTorque = row.ParseDouble("maxTorque").SI<NewtonMeter>()
				});
			}

			return new ElectricMotorFullLoadCurve(entries);
		}


		private DataTable DeNormalizeData(TableData normalizedMap, RatedPoint ratedPoint)
		{
			var result = new DataTable();
			result.Columns.Add(ElectricMotorMapReader.Fields.MotorSpeed);
			result.Columns.Add(ElectricMotorMapReader.Fields.Torque);
			result.Columns.Add(ElectricMotorMapReader.Fields.PowerElectrical);

			///Efficiency maps are saved in rpm
            foreach (DataRow row in normalizedMap.Rows)
			{
				var motorSpeed = row.ParseDouble(MotorSpeedNorm) * ratedPoint.NRated;
				var torque = row.ParseDouble(TorqueNorm) * ratedPoint.TRated;
				var powerElectrical = row.ParseDouble(PowerElectricalNorm) * ratedPoint.PRated;

				var newRow = result.NewRow();
				newRow[ElectricMotorMapReader.Fields.MotorSpeed] = Math.Round(motorSpeed.AsRPM, 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.Torque] = Math.Round(torque.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.PowerElectrical] = Math.Round(powerElectrical.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				result.Rows.Add(newRow);
			}

			return result;
		}
	}
}
