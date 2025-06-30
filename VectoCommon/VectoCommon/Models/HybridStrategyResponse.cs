using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models {

	public interface IHybridStrategyResponse {}

	public class HybridStrategyLimitedResponse : IHybridStrategyResponse
	{
		public Watt Delta { get; set; }
		public PerSecond DeltaEngineSpeed { get; set; }
		
		public GearboxResponse GearboxResponse { get; set; }
	}

	[DebuggerDisplay("HybridStrategyResponse(Gear: {NextGear})")]
	public class HybridStrategyResponse : AbstractComponentResponse, IHybridStrategyResponse
	{
		public Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> MechanicalAssistPower;

        public Second SimulationInterval;
        public bool ShiftRequired { get; set; }
		public GearshiftPosition NextGear { get; set; }
		public bool GearboxInNeutral { get; set; }
		public bool CombustionEngineOn { get; set; }

		public HybridResultEntry EvaluatedSolution { get; set; }
		public bool GearboxEngaged { get; set; }
		public bool ProhibitGearshift { get; set; }
		public PerSecond GenSetSpeed { get; set; }
	}

	[DebuggerDisplay("{U.ToString(\"F4\"),nq}: {Score,nq} - G{Gear,nq} - {IgnoreReason,nq}")]
	public class HybridResultEntry
	{
		public Second SimulationInterval;
		public double U { get; set; }

		public HybridStrategyResponse Setting { get; set; }

		public IResponse Response { get; set; }


		public double Score
		{
			get
			{
				Cost = (FuelCosts + EquivalenceFactor * (BatCosts + ICEStartPenalty1) * SoCPenalty + ICEStartPenalty2 +
				RampUpPenalty);
				var gearshift = Cost.IsSmaller(0) ? GearshiftPenalty : 1 / GearshiftPenalty;
				return Cost * gearshift;
			}
		}

		public double Cost { get; set; }

		public double FuelCosts { get; set; }

		public double BatCosts { get; set; }

		public double SoCPenalty { get; set; }

		public double EquivalenceFactor { get; set; }

		public double GearshiftPenalty { get; set; }

		public double ICEStartPenalty1 { get; set; }

		public double ICEStartPenalty2 { get; set; }

		public GearshiftPosition Gear { get; set; }

		public bool ICEOff { get; set; }

		public HybridConfigurationIgnoreReason IgnoreReason { get; set; }
		public double RampUpPenalty { get; set; }
		public bool ProhibitGearshift { get; set; }

		public bool IsEqual(HybridResultEntry other)
		{
			return ToString().Equals(other.ToString(), StringComparison.InvariantCultureIgnoreCase);
		}

		public override string ToString() {
			var setting = Setting.MechanicalAssistPower.Select(x => $"{x.Key}, {x.Value}").Join();
			return $"{U}: {setting} {Score} G{Gear}";
		}
	}

	[Flags]
	public enum HybridConfigurationIgnoreReason
	{
		NotEvaluated = 0,
		EngineSpeedTooLow = 1 << 2,
		EngineSpeedTooHigh = 1 << 3,
		EngineTorqueDemandTooHigh = 1 << 4,
		EngineTorqueDemandTooLow = 1 << 5,
		EngineSpeedAboveUpshift = 1 << 6,
		EngineSpeedBelowDownshift = 1 << 7,
		BatteryBelowMinSoC = 1 << 8,
		BatteryAboveMaxSoc = 1 << 9,
		BatterySoCTooLow = 1 << 10,
		VehicleSpeedBelowMinSpeedAfterGearshift = 1 << 11,
		MaxPropulsionTorqueExceeded = 1 << 12,
		NoResponseAvailable = 1 << 13,
		Evaluated = 1 << 14,
	}

	public static class HybridConfigurationIgnoreReasonHelper
	{
		public static string HumanReadable(this HybridConfigurationIgnoreReason x)
		{
			var retVal = new List<string>();
			foreach (var entry in EnumHelper.GetValues<HybridConfigurationIgnoreReason>()) {

				var tmp = x & entry;
				switch (tmp) {

					case HybridConfigurationIgnoreReason.Evaluated:
						retVal.Add("OK");
						break;
					case HybridConfigurationIgnoreReason.NotEvaluated:
						//retVal.Add("not evaluated");
						break;
					case HybridConfigurationIgnoreReason.EngineSpeedTooLow:
						retVal.Add("engine speed too low");
						break;
					case HybridConfigurationIgnoreReason.EngineSpeedTooHigh:
						retVal.Add("engine speed too high");
						break;
					case HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh:
						retVal.Add("engine torque demand too high");
						break;
					case HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow:
						retVal.Add("engine torque demand too low");
						break;
					case HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift: retVal.Add("engine speed above upshift"); break;
					case HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift:
						retVal.Add("engine speed below downshift");
						break;
					case HybridConfigurationIgnoreReason.NoResponseAvailable: return "no response available";

					case HybridConfigurationIgnoreReason.BatteryBelowMinSoC:
						retVal.Add("battery below MinSoC");
						break;
					case HybridConfigurationIgnoreReason.BatteryAboveMaxSoc:
						retVal.Add("battery above MaxSoC");
						break;
					case HybridConfigurationIgnoreReason.BatterySoCTooLow:
						retVal.Add("battery SoC too low");
						break;
					default: throw new ArgumentOutOfRangeException(nameof(x), x, null);
				}
			}

			return retVal.Join("/");
		}

		public static bool InvalidEngineSpeed(this HybridConfigurationIgnoreReason x)
		{
			return (x & (HybridConfigurationIgnoreReason.EngineSpeedTooLow |
					HybridConfigurationIgnoreReason.EngineSpeedTooHigh |
					HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift |
					 HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift)) != 0;
		}

		public static bool BatteryDemandExceeded(this HybridConfigurationIgnoreReason x)
		{
			return (x & (HybridConfigurationIgnoreReason.BatteryAboveMaxSoc |
						HybridConfigurationIgnoreReason.BatteryBelowMinSoC)) != 0;
		}

		public static bool EngineSpeedTooHigh(this HybridConfigurationIgnoreReason x)
		{
			return (x & HybridConfigurationIgnoreReason.EngineSpeedTooHigh) != 0;
		}

		public static bool EngineSpeedTooLow(this HybridConfigurationIgnoreReason x)
		{
			return (x & HybridConfigurationIgnoreReason.EngineSpeedTooLow) != 0;
		}

		public static bool EngineSpeedBelowDownshift(this HybridConfigurationIgnoreReason x)
		{
			return (x & HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift) != 0;
		}

		public static bool EngineSpeedAboveUpshift(this HybridConfigurationIgnoreReason x)
		{
			return (x & HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift) != 0;
		}

		public static bool EngineTorqueDemandTooHigh(this HybridConfigurationIgnoreReason x)
		{
			return (x & HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh) != 0;
		}

		public static bool EngineTorqueDemandTooLow(this HybridConfigurationIgnoreReason x)
		{
			return (x & HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow) != 0;
		}

		public static bool EngineTorqueOK(this HybridConfigurationIgnoreReason x)
		{
			return (x & (HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh |
						HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow)) == 0;
		}

		public static bool BatterySoCTooLow(this HybridConfigurationIgnoreReason x)
		{
			return (x & HybridConfigurationIgnoreReason.BatterySoCTooLow) != 0;
		}

		public static bool BatteryBelowMinSoC(this HybridConfigurationIgnoreReason x)
		{
			return (x & HybridConfigurationIgnoreReason.BatteryBelowMinSoC) != 0;
		}

		public static bool AllOK(this HybridConfigurationIgnoreReason x)
		{
			return (x & HybridConfigurationIgnoreReason.Evaluated) == HybridConfigurationIgnoreReason.Evaluated;
		}

		public static bool Evaluated(this HybridConfigurationIgnoreReason x)
		{
			return x != HybridConfigurationIgnoreReason.NotEvaluated;
		}
	}

}