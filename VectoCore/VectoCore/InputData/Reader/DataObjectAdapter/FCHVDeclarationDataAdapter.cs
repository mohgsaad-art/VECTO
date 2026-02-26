using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class FCHVDeclarationDataAdapter : AbstractSimulationDataAdapter
	{
		private string _jobFilePath;

		public IOutputDataWriter DebugOutputDataWriter
		{
			get;
			set;
		}

		public string JobFilePath
		{
			get
			{
				if (string.IsNullOrEmpty(_jobFilePath))
				{
					return DebugOutputDataWriter?.JobFile;
				}
				return _jobFilePath;
			}
			set
			{
				_jobFilePath = value;
			}
		}

		public FCHVDeclarationDataAdapter(DataSource dataSource)
		{
			JobFilePath = dataSource?.SourceFile;
		}

		public FuelCellPowerMap CreateFuelCellPowerMap(
			IModalDataContainer modalData,
			FuelCellSystemData fcsData,
			BatterySystemData batterySystemData)
		{
			var fcPostProcessor = new FuelCellPreRunPostprocessor(modalData)
			{
				WriterBasePath = JobFilePath
			};

			fcsData.PreRunPostProcessing = fcPostProcessor;
			var result = fcPostProcessor.CalculateFuelCellPowerDemand(fcsData, batterySystemData.Clone(), modalData.WriteModalResults);
			if (result.Success)
			{
				batterySystemData.InitialSoC = result.InitSoc;
			}

			return new FuelCellPowerMap(result.Entries, result.Reason);
		}

		public FuelCellSystemShareMap CreateFuelCellShareMap(FuelCellSystemData fuelCellSystemData)
		{
			var fcSystemMassFlowMap = new FuelCellSystemMassFlowMap(
				fuelCellSystemData.FuelCellStrings.ElementAt(0).MassFlowMap,
				fuelCellSystemData.FuelCellStrings.ElementAtOrDefault(1)?.MassFlowMap);

			return new FuelCellSystemShareMap(fcSystemMassFlowMap);
		}

		public BatterySystemData CreateFuelCellPreProcessingBattery(
			FuelCellSystemData fuelCellSystemData,
			BatterySystemData batterySystemData,
			out Tuple<int, BatteryData> fuelCellBattery)
		{
			Watt fcP = fuelCellSystemData.FuelCellStrings.Sum(fc => fc.MaxPower);

			return CreateFuelCellPreProcessingBattery(fcP, batterySystemData, out fuelCellBattery);
		}

		private BatterySystemData CreateFuelCellPreProcessingBattery(
			Watt fuelCellStringSumPower, BatterySystemData batterySystemData, out Tuple<int, BatteryData> fuelCellBattery)
		{
			var pevBat = batterySystemData;
			pevBat.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
			var fcP = fuelCellStringSumPower;
			var V = pevBat.CalculateVoltageCenterSoc();
			var I = fcP / V;

			var resistance = 1E-12.SI<Ohm>();

			var batteryData = new BatteryData()
			{
				BatteryId = FuelCellSystemData.FuelCellBatID,
				ChargeDepletingBattery = true,
				MinSOC = 0,
				MaxSOC = 1,
				InputData = null,
				Capacity = 1E5.SI<AmpereSecond>(), //?
				MaxCurrent = new MaxCurrentMap(new[] {
					new MaxCurrentMap.MaxCurrentEntry() {
						SoC = 0,
						MaxDischargeCurrent = -I,
						MaxChargeCurrent = 0.SI<Ampere>()
					},
					new MaxCurrentMap.MaxCurrentEntry() {
						SoC = 0.5,
						MaxDischargeCurrent = -I,
						MaxChargeCurrent = 0.SI<Ampere>()
					},
					new MaxCurrentMap.MaxCurrentEntry() {
						SoC = 1,
						MaxDischargeCurrent = -I,
						MaxChargeCurrent = 0.SI<Ampere>()
					}
				}),
				SOCMap = new SOCMap(new[] {
					new SOCMap.SOCMapEntry() {
						SOC = 0,
						BatteryVolts = V
					},
					new SOCMap.SOCMapEntry() {
						SOC = 0.5,
						BatteryVolts = V
					},
					new SOCMap.SOCMapEntry(){
						SOC = 1,
						BatteryVolts = V
					}
				}),
				InternalResistance = new InternalResistanceMap(new[] {
					new InternalResistanceMap.InternalResistanceMapEntry() {
						SoC = 0,
						Resistance = new List<Tuple<Second, Ohm>>() {
							Tuple.Create(0.SI<Second>(), resistance),
							Tuple.Create(1e9.SI<Second>(), resistance)
						}
					},
					new InternalResistanceMap.InternalResistanceMapEntry() {
						SoC = 0.5,
						Resistance = new List<Tuple<Second, Ohm>>() {
							Tuple.Create(0.SI<Second>(), resistance),
							Tuple.Create(1e9.SI<Second>(), resistance)
						}
					},
					new InternalResistanceMap.InternalResistanceMapEntry() {
						SoC = 1,
						Resistance = new List<Tuple<Second, Ohm>>() {
							Tuple.Create(0.SI<Second>(), resistance),
							Tuple.Create(1e9.SI<Second>(), resistance)
						}
					}
				})
			};
			fuelCellBattery = Tuple.Create(0xFCB, batteryData);
			batterySystemData.Batteries.Add(fuelCellBattery);
			return batterySystemData;
		}
	}
}
