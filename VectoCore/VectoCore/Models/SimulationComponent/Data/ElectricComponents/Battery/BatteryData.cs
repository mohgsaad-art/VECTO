using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery {

	public class BatterySystemData : ICloneable
	{
		public BatterySystemData()
		{
			Batteries = new List<Tuple<int, BatteryData>>();
		}

        public List<Tuple<int, BatteryData>> Batteries { get; internal set; }

		public double InitialSoC { get; internal set; }

		public Ohm ConnectionSystemResistance { get; internal set; } = 0.SI<Ohm>();

		public AmpereSecond Capacity
		{
			get
			{
				return Batteries.Select(x => x.Item1).Distinct().OrderBy(x => x)
					.Aggregate(0.SI<AmpereSecond>(), 
						(current, s) => current + Batteries.Where(x => x.Item1 == s).Min(x => x.Item2.Capacity));
			}
		}

		public WattSecond TotalStoredEnergy {
			get {
				return Batteries.Select(x => x.Item1).Distinct().OrderBy(x => x).Aggregate(0.SI<WattSecond>(),
					(current, s) => current + Batteries.Where(x => x.Item1 == s).Min(x => x.Item2.TotalStoredEnergy));
			}
		}

        public WattSecond UseableStoredEnergy
		{
			get
			{
				return Batteries.Select(x => x.Item1).Distinct().OrderBy(x => x).Aggregate(0.SI<WattSecond>(),
					(current, s) => current + Batteries.Where(x => x.Item1 == s).Min(x => x.Item2.UseableStoredEnergy));
			}
		}

		public bool ChargeSustainingBatterySystem
		{
			get { return Batteries.All(b => b.Item2.ChargeDepletingBattery); } 
			set { Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = value); }
		}

		#region Implementation of ICloneable

		object ICloneable.Clone()
		{
			return Clone();
		}

		public BatterySystemData Clone()
		{
			var cloned = new BatterySystemData()
			{
				InitialSoC = this.InitialSoC,
				ConnectionSystemResistance = this.ConnectionSystemResistance,
			};

			foreach (var b in Batteries)
			{
				cloned.Batteries.Add(Tuple.Create(b.Item1, b.Item2.Clone()));
			}

			return cloned;
        }

		#endregion
	}

	public class BatteryData : ICloneable
	{
		private WattSecond _totaltoredEnergy;
		private WattSecond _useableStoredEnergy;

		[ValidateObject]
		public SOCMap SOCMap { get; internal set; }

		[Range(0, 1)]
		public double MinSOC { get; internal set; }

		[Range(0, 1)]
		public double MaxSOC { get; internal set; }

		[SIRange(0, 1e9)]
		public InternalResistanceMap InternalResistance { get; internal set; }

		private AmpereSecond _capacity;

		public AmpereSecond Capacity
		{
			get => _capacity;
			internal set
			{
				_capacity = value;
				_totaltoredEnergy = null;
				_useableStoredEnergy = null;
			}
		}

		public MaxCurrentMap MaxCurrent { get; internal set; }
		public int BatteryId { get; internal set; }

		/// <summary>
		/// Infinity Battery
		/// </summary>
		public bool ChargeDepletingBattery { get; internal set; }

		public WattSecond TotalStoredEnergy => _totaltoredEnergy ?? (_totaltoredEnergy = CalculateBatteryEnergy(0, 1));

        public WattSecond UseableStoredEnergy => _useableStoredEnergy ?? (_useableStoredEnergy = CalculateBatteryEnergy(MinSOC, MaxSOC));

		[JsonIgnore]
		public IElectricStorageDeclarationInputData InputData { get; internal set; }

		protected WattSecond CalculateBatteryEnergy(double minSoc, double maxSoc)
		{
			var retVal = 0.SI<WattSecond>();
			foreach (var (low, high) in SOCMap.Entries.Pairwise()) {
				if (low.SOC.IsSmaller(minSoc) && high.SOC.IsSmaller(minSoc)) {
					continue;
				}

				if (low.SOC.IsGreater(maxSoc) && high.SOC.IsGreater(maxSoc)) {
					continue;
				}

				var min = VectoMath.Max(minSoc, low.SOC);
				var max = VectoMath.Min(maxSoc, high.SOC);
				var voltage = SOCMap.Lookup((min + max) / 2.0);
				retVal += (max - min) * Capacity * voltage;
			}
			return retVal;
		}

		#region Implementation of ICloneable

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public BatteryData Clone()
		{
			return new BatteryData() {
				BatteryId = this.BatteryId,
				ChargeDepletingBattery = this.ChargeDepletingBattery,
				MinSOC = this.MinSOC,
				MaxSOC = this.MaxSOC,
				Capacity = this.Capacity,
				InputData = this.InputData,

				InternalResistance = this.InternalResistance,
				MaxCurrent = this.MaxCurrent,
				SOCMap = this.SOCMap,
			};
		}

		#endregion
	}

	public class SuperCapData
	{
		public Farad Capacity { get; internal set; }

		public Ohm InternalResistance { get; internal set; }

		public Volt MinVoltage { get; internal set; }

		public Volt MaxVoltage { get; internal set; }
		public double InitialSoC { get; internal set; }
		public Ampere MaxCurrentCharge { get; internal set; }

		public Ampere MaxCurrentDischarge { get; internal set; }
	}

	public class SOCMap
	{
		protected internal SOCMapEntry[] Entries;

		public SOCMap(SOCMapEntry[] entries)
		{
			Entries = entries;
		}

		public string[] SerializedEntries
		{
			get
			{
				return Entries.Select(x => $"{x.SOC} - {x.BatteryVolts}").ToArray();
			}
		}

		public Volt Lookup(double soc)
		{
			var idx = FindIndex(soc);
			return VectoMath.Interpolate(Entries[idx - 1].SOC, Entries[idx].SOC, Entries[idx - 1].BatteryVolts,
				Entries[idx].BatteryVolts, soc);
		}

		public bool ContainsSoC(double soc)
		{
			return Entries.Any(x => x.SOC.IsEqual(soc));
		}

		protected int FindIndex(double soc)
		{
			if (soc < Entries.First().SOC)
			{
				return 1;
			}
			if (soc > Entries.Last().SOC)
			{
				return Entries.Length - 1;

			}
			for (var index = 1; index < Entries.Length; index++)
			{
				if (soc >= Entries[index - 1].SOC && soc <= Entries[index].SOC)
				{
					return index;
				}
			}
			throw new VectoException($"soc {soc} exceeds battery model data. min: {Entries.First().SOC} max: {Entries.Last().SOC}", ErrorCodes.REESS_INVALID_SOC);
		}

		
		public class SOCMapEntry
		{
			[Required, Range(0, 1)] public double SOC;
			[Required, SIRange(0, double.MaxValue)] public Volt BatteryVolts;
		}
	}

	public class InternalResistanceMap
	{
		protected internal InternalResistanceMapEntry[] Entries;

		public InternalResistanceMap(InternalResistanceMapEntry[] entries)
		{
			Entries = entries;
		}

		public string[] SerializedEntries
		{
			get
			{
				return Entries.Select(x =>
					$"{x.SoC}: " + x.Resistance.OrderBy(r => r.Item1.Value()).Select(r => $"{r.Item1}: {r.Item2}")
						.Join(";")).ToArray();
			}
		}

		public Ohm Lookup(double SoC, Second tPulse)
		{
			var idx = FindIndex(SoC);
			var entry1 = Entries[idx - 1];
			var entry2 = Entries[idx];
			
			var resistance1 = InterpolateResistance(entry1.Resistance, tPulse);
			var resistance2 = InterpolateResistance(entry2.Resistance, tPulse);

			return VectoMath.Interpolate(entry1.SoC, entry2.SoC, resistance1, resistance2, SoC);
		}

		private Ohm InterpolateResistance(List<Tuple<Second, Ohm>> resistance, Second tPulse)
		{
			
			if (tPulse < resistance.First().Item1) {
				return resistance.First().Item2;
			}

			if (tPulse > resistance.Last().Item1) {
				return resistance.Last().Item2;
			}

			Tuple<Second, Ohm> entry1 = null;
			Tuple<Second, Ohm> entry2 = null;
			for (var index = 1; index < resistance.Count; index++) {
				if (tPulse >= resistance[index - 1].Item1 && tPulse <= resistance[index].Item1) {
					entry1 = resistance[index - 1];
					entry2 = resistance[index];
				}
			}

			if (entry1 == null || entry2 == null) {
				throw new VectoException("Failed to lookup internal resistance!", ErrorCodes.REESS_INTERNAL_RESISTANCE_LOOKUP_FAILED);
			}

			return	VectoMath.Interpolate(entry1.Item1, entry2.Item1,
					entry1.Item2, entry2.Item2, tPulse);
		}

		protected int FindIndex(double soc)
		{
			if (soc < Entries.First().SoC) {
				return 1;
			}

			if (soc > Entries.Last().SoC) {
				return Entries.Length - 1;

			}

			for (var index = 1; index < Entries.Length; index++) {
				if (soc >= Entries[index - 1].SoC && soc <= Entries[index].SoC) {
					return index;
				}
			}

			throw new VectoException($"soc {soc} exceeds battery model data. min: {Entries.First().SoC} max: {Entries.Last().SoC}", ErrorCodes.REESS_INVALID_SOC);
		}

		public class InternalResistanceMapEntry
		{
			[Required, Range(0, 1)] public double SoC;
			[Required, SIRange(0, 1e6)] public List<Tuple<Second, Ohm>> Resistance;
		}

	}


	public class MaxCurrentMap
	{
		protected internal MaxCurrentEntry[] Entries;

		public MaxCurrentMap(MaxCurrentEntry[] entries)
		{
			Entries = entries;
		}

		public string[] SerializedEntries
		{
			get
			{
				return Entries.Select(x => $"{x.SoC}: {x.MaxChargeCurrent} / {x.MaxDischargeCurrent}").ToArray();
			}
		}

		public Ampere LookupMaxChargeCurrent(double soc)
		{
			var idx = FindIndex(soc);
			return VectoMath.Interpolate(Entries[idx - 1].SoC, Entries[idx].SoC, Entries[idx - 1].MaxChargeCurrent,
				Entries[idx].MaxChargeCurrent, soc);
		}

		public Ampere LookupMaxDischargeCurrent(double soc)
		{
			var idx = FindIndex(soc);
			return VectoMath.Interpolate(Entries[idx - 1].SoC, Entries[idx].SoC, Entries[idx - 1].MaxDischargeCurrent,
				Entries[idx].MaxDischargeCurrent, soc);
		}

		protected int FindIndex(double soc)
		{
			if (soc < Entries.First().SoC) {
				return 1;
			}

			if (soc > Entries.Last().SoC) {
				return Entries.Length - 1;

			}

			for (var index = 1; index < Entries.Length; index++) {
				if (soc >= Entries[index - 1].SoC && soc <= Entries[index].SoC) {
					return index;
				}
			}

			throw new VectoException($"soc {soc} exceeds battery model data. min: {Entries.First().SoC} max: {Entries.Last().SoC}", ErrorCodes.REESS_INVALID_SOC);
		}

		public class MaxCurrentEntry
		{
			[Required, SIRange(0, 1)] public double SoC;

			[Required, SIRange(0, double.MaxValue)]
			public Ampere MaxChargeCurrent;

			[Required, SIRange(-double.MaxValue, 0)]
			public Ampere MaxDischargeCurrent;
		}
	}
}