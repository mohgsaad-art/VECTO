using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.OutputData
{
    public class OvcResultEntry : IOVCResultEntry
	{
		#region Implementation of IOVCResultEntry

		public IResultEntry ChargeDepletingResult { get; internal set; }
		public IResultEntry ChargeSustainingResult { get; internal set; }

		public IWeightedResult Weighted { get; internal set; }

		#endregion
	}

	public class WeightedResult : IWeightedResult
	{
		#region Implementation of IWeightedResult

		public Meter Distance  { get; internal set; }
		
		public Kilogram Payload { get; internal set; }
		
		public CubicMeter CargoVolume { get; internal set; }
		
		public double? PassengerCount { get; internal set; }

		public VectoSimulationJobType JobType { get; internal set; }

		public bool OffVehicleCharging { get; internal set; }

		public MeterPerSecond AverageSpeed { get; internal set; }
		
		public MeterPerSecond AverageDrivingSpeed { get; internal set; }

		public IDictionary<IFuelProperties, Kilogram> FuelConsumption { get; internal set; }
		
		public IDictionary<IFuelProperties, KilogramPerMeter> FuelConsumptionPerMeter { get; internal set; }

		public WattSecond ElectricEnergyConsumption { get; internal set; }
		
		public KilogramPerMeter CO2PerMeter { get; internal set; }
		
		public Meter ActualChargeDepletingRange { get; internal set; }
		
		public Meter EquivalentAllElectricRange { get; internal set; }
		
		public Meter ZeroCO2EmissionsRange { get; internal set; }

		public Meter HydrogenRange { get; internal set; }
		
		public double UtilityFactor { get; internal set; }
		
		public IFuelProperties AuxHeaterFuel { get; set; }
		
		public KilogramPerMeter ZEV_FuelConsumption_AuxHtr { get; set; }
		
		public KilogramPerMeter ZEV_CO2 { get; set; }
		
		public VectoRun.Status Status { get; set; }

		#endregion
	}
}