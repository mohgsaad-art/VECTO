using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public class ElectricStorageAdapter : IElectricStorageAdapter
	{
		public BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData batteryInputData,
			VectoSimulationJobType jobType,
			bool ovc)
		{
			if (batteryInputData == null) {
				return null;
			}

			var batteries = batteryInputData.ElectricStorageElements.Where(x => x.REESSPack.StorageType == REESSType.Battery).ToArray();

			if (batteries.Length == 0) {
				return null;
			}

			var addJunctionBoxResistance = false;
			var addConnectorSystemResistance = false;
			var retVal = new BatterySystemData();
			var genericSOC = DeclarationData.Battery.GenericSOC.Lookup(jobType, ovc);
			foreach (var entry in batteries)
			{
				var b = entry.REESSPack as IBatteryPackDeclarationInputData;
				if (b == null)
				{
					continue;
				}

				if (b.JunctionboxIncluded != null && !b.JunctionboxIncluded.Value) {
					addJunctionBoxResistance = true;
				}

				if (b.ConnectorsSubsystemsIncluded != null && !b.ConnectorsSubsystemsIncluded.Value) {
					addConnectorSystemResistance = true;
				}

				if (jobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.IEPC_E) ||
					(jobType == VectoSimulationJobType.ParallelHybridVehicle) && ovc)
				{
					if (b.MinSOC == null)
					{
						throw new VectoException("Battery SOCmin is undefined");
					}

					if (b.MaxSOC == null)
					{
                        throw new VectoException("Battery SOCmax is undefined");
                    }

					if (b.DeteriorationPerformanceRatio == null)
					{
						//TODO: This should only apply to version v2.7 XSD
                        //throw new VectoException("Battery DeteriorationPerformanceRatio is undefined");
                    }
				}

				var minSoc = genericSOC.SOCMin;
				if (b.MinSOC != null && b.MinSOC > minSoc) {
					minSoc = b.MinSOC.Value;
				}

				var maxSoc = genericSOC.SOCMax;
				if (b.MaxSOC != null && b.MaxSOC < maxSoc && b.MaxSOC > b.MinSOC) {
					maxSoc = b.MaxSOC.Value;
				}
			
				var batteryData = new BatteryData() {
					MinSOC = maxSoc  * ((1d/2) * DeclarationData.Battery.GenericDeterioration)
							+ minSoc * (1 - (1d/2) * DeclarationData.Battery.GenericDeterioration),
					MaxSOC = (maxSoc * (1 - (1d/2) * DeclarationData.Battery.GenericDeterioration)
							+ minSoc * ((1d/2) * DeclarationData.Battery.GenericDeterioration)),
					MaxCurrent = BatteryMaxCurrentReader.Create(b.MaxCurrentMap),
					Capacity = b.Capacity,
					InternalResistance =
						BatteryInternalResistanceReader.Create(b.InternalResistanceCurve, entry.REESSPack.DataSource.SourceType != DataSourceType.JSONFile),
					SOCMap = BatterySOCReader.Create(b.VoltageCurve),
					InputData = entry
				};

#if DEBUG
				if (!entry.REESSPack.DataSource.SourceType.IsOneOf(DataSourceType.JSONFile, DataSourceType.XMLFile,
						DataSourceType.XMLEmbedded)) {
					throw new VectoException(
						$"Expected Datasource type to be JSONFile, XMLFile or XMLEmbedded but was {entry.REESSPack.DataSource.SourceType}");
				};
#endif

				retVal.Batteries.Add(Tuple.Create(entry.StringId, batteryData));
			}

			if (addJunctionBoxResistance) {
				retVal.ConnectionSystemResistance += DeclarationData.Battery.JunctionBoxResistance;
			}

			if (addConnectorSystemResistance) {
				retVal.ConnectionSystemResistance += DeclarationData.Battery.CablesAndConnectorsResistance;
			}

			retVal.InitialSoC = CalculateInitialSoc(retVal);
			return retVal;
		}

		private double CalculateInitialSoc(BatterySystemData battery)
		{
			var socLimits = battery.GetSocLimits();
			return (socLimits.MaxSoc + socLimits.MinSoc) / 2;
		}

		public SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData reessInputData)
		{
			if (reessInputData == null)
			{
				return null;
			}

			var superCaps = reessInputData.ElectricStorageElements.Where(x => x.REESSPack.StorageType == REESSType.SuperCap).ToArray();

			var superCap = superCaps.FirstOrDefault()?.REESSPack as ISuperCapDeclarationInputData;

			if (superCap == null)
			{
				return null;
			}
			

			var r = new SuperCapData()
			{
				Capacity = superCaps.First().Count * superCap.Capacity,
				InternalResistance = superCap.InternalResistance / superCaps.First().Count,
				MinVoltage = VectoMath.Max(superCap.MaxVoltage * DeclarationData.SuperCap.SocMin, superCap.MinVoltage),
				MaxVoltage = superCap.MaxVoltage,
				MaxCurrentCharge = superCap.MaxCurrentCharge,
				MaxCurrentDischarge = -superCap.MaxCurrentDischarge,
				
			};

			r.InitialSoC =
				Math.Sqrt(Math.Pow(r.MaxVoltage.Value(), 2) - Math.Pow(r.MinVoltage.Value(), 2)) /
				r.MaxVoltage.Value();
			return r;
		}


	}

	public class GenericElectricStorageDataAdapter : IElectricStorageAdapter
	{
		protected GenericBusBatteryData busBattery = new GenericBusBatteryData();
		protected GenericBusSuperCapData busSuperCap = new GenericBusSuperCapData();

		#region Implementation of IElectricStorageAdapter

		public BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData batteryInputData,
			VectoSimulationJobType jobType, bool ovc)
		{
			if (batteryInputData == null) {
				return null;
			}
            return busBattery.CreateBatteryData(batteryInputData, jobType, ovc);
		}

		public SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData reessInputData)
		{
			if (reessInputData == null) {
				return null;
			}
			var superCaps = reessInputData.ElectricStorageElements.Where(x => x.REESSPack.StorageType == REESSType.SuperCap).ToArray();

			var superCap = superCaps.FirstOrDefault()?.REESSPack as ISuperCapDeclarationInputData;

			if (superCap == null) {
				return null;
			}

			return busSuperCap.CreateGenericSuperCapData(superCap);
		}

		#endregion
	}

	public static class BatterySystemHelper
	{
		public static Volt CalculateVoltageCenterSoc(this BatterySystemData battery)
		{
			if (battery == null) {
				return null;
			}

			var tmpBattery = new BatterySystem(null, battery);
			var min = tmpBattery.MinSoC;
			var max = tmpBattery.MaxSoC;
			var averageSoC = (min + max) / 2.0;

			tmpBattery.Initialize(averageSoC);
			return tmpBattery.InternalVoltage;
		}


		public static (double MinSoc, double MaxSoc) GetSocLimits(this BatterySystemData battery)
		{
			if (battery == null) {
				throw new ArgumentNullException();
			}

			var tmpBattery = new BatterySystem(null, battery);
			return (tmpBattery.MinSoC, tmpBattery.MaxSoC);
		}
	}
	
}