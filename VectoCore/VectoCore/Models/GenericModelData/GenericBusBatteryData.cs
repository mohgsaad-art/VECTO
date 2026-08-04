using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericBusBatteryData
	{
		public BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData batteryData, VectoSimulationJobType jobType,
			bool ovc, bool batteryOnlyMode)
		{
			var currentBatteryData = batteryData.ElectricStorageElements
				.Where(x => x.REESSPack.StorageType == REESSType.Battery).ToList();

			if (currentBatteryData.Count == 0)
				return null;

			var retVal = new BatterySystemData {
				Batteries = GetBatteries(currentBatteryData, jobType, ovc, batteryOnlyMode),
				ConnectionSystemResistance = 0.SI<Ohm>()
			};
			var limits = retVal.GetSocLimits();
			retVal.InitialSoC = (limits.MaxSoc + limits.MinSoc) / 2;
			return retVal;
		}

		private List<Tuple<int, BatteryData>> GetBatteries(List<IElectricStorageDeclarationInputData> currentBatteries, VectoSimulationJobType jobType,
			bool ovc, bool batteryOnlyMode)
		{
			var result = new List<Tuple<int, BatteryData>>();

			foreach (var currentBattery in currentBatteries) {
				var entry = new Tuple<int, BatteryData>(currentBattery.StringId,
					GetBatteryData(currentBattery.REESSPack as IBatteryPackDeclarationInputData, jobType, ovc, batteryOnlyMode));
				result.Add(entry);
			}
			
			return result;
		}
		
		private BatteryData GetBatteryData(IBatteryPackDeclarationInputData battery, VectoSimulationJobType jobType,
			bool ovc, bool batteryOnlyMode)
		{
			var genericSOC = DeclarationData.Battery.GenericSOC.Lookup(jobType, ovc);

            var windowEqualOrSmallerThanDefault = !ovc || !batteryOnlyMode;

            var minSoc = battery.MinSOC.HasValue
                ? ((windowEqualOrSmallerThanDefault && (battery.MinSOC.Value < genericSOC.SOCMin)) ? genericSOC.SOCMin : battery.MinSOC.Value)
                : genericSOC.SOCMin;

            var maxSoc = battery.MaxSOC.HasValue
                ? ((windowEqualOrSmallerThanDefault && (battery.MaxSOC.Value > genericSOC.SOCMax)) ? genericSOC.SOCMax : battery.MaxSOC.Value)
                : genericSOC.SOCMax;

            if (maxSoc <= minSoc)
            {
                throw new VectoException($"Battery: min SoC ({minSoc}) must be less than max SoC ({maxSoc}).");
            }

            return new BatteryData {
				MinSOC = maxSoc * ((1d / 2) * DeclarationData.Battery.GenericDeterioration)
						+ minSoc * (1 - (1d / 2) * DeclarationData.Battery.GenericDeterioration),
				MaxSOC = (maxSoc * (1 - (1d / 2) * DeclarationData.Battery.GenericDeterioration)
						+ minSoc * ((1d / 2) * DeclarationData.Battery.GenericDeterioration)),
                MaxCurrent = BatteryMaxCurrentReader.Create(battery.MaxCurrentMap),
				Capacity = battery.Capacity,
				InternalResistance = GetGenericInternalResistanceMap(battery),
				SOCMap = BatterySOCReader.Create(battery.VoltageCurve)
			};
		}
		
		private InternalResistanceMap GetGenericInternalResistanceMap(IBatteryPackDeclarationInputData battery)
		{
			var vNominal = GetNominalVoltage(battery.VoltageCurve);
			var resistance = 0.0;

			if (battery.BatteryType == BatteryType.HPBS)
				resistance = GetHPBSResistance(battery.Capacity.AsAmpHour, vNominal);
			else if (battery.BatteryType == BatteryType.HEBS)
				resistance = GetHEBSResistance(battery.Capacity.AsAmpHour, vNominal);

			return BatteryInternalResistanceReader.Create(GetGenericResistanceDataTable(battery.BatteryType, resistance), true);
		}

		private DataTable GetGenericResistanceDataTable(BatteryType batteryType, double resistance)
		{
			var result = new DataTable();
			result.Columns.Add(BatteryInternalResistanceReader.Fields.StateOfCharge);
			result.Columns.Add(BatteryInternalResistanceReader.Fields.InternalResistance_2);
			result.Columns.Add(BatteryInternalResistanceReader.Fields.InternalResistance_10);
			result.Columns.Add(BatteryInternalResistanceReader.Fields.InternalResistance_20);
			
			if (batteryType == BatteryType.HEBS)
				result.Columns.Add(BatteryInternalResistanceReader.Fields.InternalResistance_120);

			for (int r = 0; r < 2; r++) {

				var soc = r == 0 ? 0 : 100;
				result.Rows.Add(result.NewRow());
				result.Rows[r][BatteryInternalResistanceReader.Fields.StateOfCharge] = soc;
				result.Rows[r][BatteryInternalResistanceReader.Fields.InternalResistance_2] = resistance.ToXMLFormat(6);
				result.Rows[r][BatteryInternalResistanceReader.Fields.InternalResistance_10] = resistance.ToXMLFormat(6);
				result.Rows[r][BatteryInternalResistanceReader.Fields.InternalResistance_20] = resistance.ToXMLFormat(6);
				if (batteryType == BatteryType.HEBS)
					result.Rows[r][BatteryInternalResistanceReader.Fields.InternalResistance_120] = resistance.ToXMLFormat(6);
			}
			
			return result;
		}

		private Volt GetNominalVoltage(TableData ocvData)
		{
			var sortedOcvData = ocvData.AsEnumerable().OrderBy(x => x.ParseDouble(XMLNames.REESS_OCV_SoC)).ToList();

			for (int i = 0; i < sortedOcvData.Count; i++) {

				var soc = sortedOcvData[i].ParseDouble(XMLNames.REESS_OCV_SoC);
				if (soc >= 50) {

					int fstIndex;
					int secIndex;
					if (i < sortedOcvData.Count - 1) {
						fstIndex = i;
						secIndex = i + 1;
					} else {
						fstIndex = i - 1;
						secIndex = i;
					}

					var fstSoC = sortedOcvData[fstIndex].ParseDouble(BatterySOCReader.Fields.StateOfCharge);
					var secSoC = sortedOcvData[secIndex].ParseDouble(BatterySOCReader.Fields.StateOfCharge);
					var fstOCV = sortedOcvData[fstIndex].ParseDouble(BatterySOCReader.Fields.BatteryVoltage);
					var secOCV = sortedOcvData[secIndex].ParseDouble(BatterySOCReader.Fields.BatteryVoltage);

					return VectoMath.Interpolate(fstSoC, secSoC, fstOCV, secOCV, 50).SI<Volt>();
				}
			}

			return null;
		}
		
		private double GetHPBSResistance(double ratedCapacity, Volt vNominal)
		{
			return (25 / ratedCapacity ) * (vNominal.Value() / 3.3);
		}
		
		private double GetHEBSResistance(double ratedCapacity, Volt vNominal)
		{
			return (140 / ratedCapacity) * (vNominal.Value() / 3.7);
		}
	}
}
