using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.InputData {
	public enum PowertrainPosition
	{
		HybridPositionNotSet, // this has to be the first entry so that it is used as default for not initialized fields!
		HybridP0,
		HybridP1,
		HybridP2,
		HybridP2_5,
		HybridP3,
		HybridP4,

		GEN,

		BatteryElectricE4,
		BatteryElectricE3,
		BatteryElectricE2,

		IEPC,
		IHPC
	}

	public static class PowertrainPositionHelper
	{
		public const string HybridPrefix = "Hybrid";
		public const string BatteryElectricPrefix = "BatteryElectric";

		public static int GetPositionNumber(this PowertrainPosition pos)
		{
			switch (pos) {
				case PowertrainPosition.HybridP0:
					return 0;
				case PowertrainPosition.HybridP1:
					return 1;
				case PowertrainPosition.BatteryElectricE2:
				case PowertrainPosition.IEPC:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP2_5:
				case PowertrainPosition.IHPC:
					return 2;
				case PowertrainPosition.BatteryElectricE3:
				case PowertrainPosition.HybridP3:
					return 3;
				case PowertrainPosition.BatteryElectricE4:
				case PowertrainPosition.HybridP4:
					return 4;
				default:
					return -1;
			}
		}

		public static PowertrainPosition Parse(string prefix, string pos)
		{
			if (pos.Equals(nameof(PowertrainPosition.GEN))) {
				return PowertrainPosition.GEN;
			}
			if (pos.Equals(nameof(PowertrainPosition.IHPC), StringComparison.InvariantCultureIgnoreCase)) {
				return PowertrainPosition.IHPC;
			}

			if (prefix.Equals("P", StringComparison.InvariantCultureIgnoreCase)) {
				return (HybridPrefix + prefix + pos).Replace(".", "_").ParseEnum<PowertrainPosition>();
			}

			var supportedPrefixes = new[] { "B", "E", "F", "S" };
			if (supportedPrefixes.Contains(prefix, StringComparer.InvariantCultureIgnoreCase)) {
				return $"{BatteryElectricPrefix}E{pos}".ParseEnum<PowertrainPosition>();
			}
			
			throw new VectoException("invalid powertrain position {0}", pos);
		}

		public static PowertrainPosition Parse(string pos)
		{
			if (pos.Length > 1 && pos[0].IsOneOf('B', 'P', 'E', 'F')) {
				return Parse(pos.Substring(0, 1), pos.Substring(1));
			}

			return Parse("", pos);
		}

		public static string GetName(this PowertrainPosition pos)
		{
			return pos.ToString().Replace(HybridPrefix, "").Replace(BatteryElectricPrefix, "").Replace("_", ".");
		}

		public static string GetLabel(this PowertrainPosition pos)
		{
			switch (pos) {
				case PowertrainPosition.HybridP0:
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
					return pos.ToString().Replace(HybridPrefix, "");
				case PowertrainPosition.HybridP2_5:
					return pos.ToString().Replace(HybridPrefix, "").Replace("_", ".");
				case PowertrainPosition.GEN:
					return nameof(PowertrainPosition.GEN);
			}
			return pos.ToString().Replace(BatteryElectricPrefix, "").Replace("B", "E");
		}

		public static bool IsBatteryElectric(this PowertrainPosition pos)
		{
			switch (pos) {
				case PowertrainPosition.BatteryElectricE2:
				case PowertrainPosition.BatteryElectricE3:
				case PowertrainPosition.BatteryElectricE4:
				case PowertrainPosition.IEPC:
					return true;
				default:
					return false;
			}
		}

		public static bool IsParallelHybrid(this PowertrainPosition pos)
		{
			switch (pos) {
				//case PowertrainPosition.HybridP0: // special case currently modelled in BusAuxiliary as SmartAlternator.
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP2_5:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
					return true;
				default:
					return false;
			}
		}

		public static string ToXmlFormat(this PowertrainPosition pos)
		{
			switch (pos) {
				case PowertrainPosition.HybridPositionNotSet:
					throw new ArgumentException("Hybrid position not set");
				case PowertrainPosition.HybridP0: 
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP2_5:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
				case PowertrainPosition.BatteryElectricE4:
				case PowertrainPosition.BatteryElectricE3:
				case PowertrainPosition.BatteryElectricE2:
					return GetPositionWithoutPrefix(pos.ToString());
				case PowertrainPosition.IHPC:
					return "IHPC";
				case PowertrainPosition.GEN:
					return "GEN";
				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
			}
		}

		private static string GetPositionWithoutPrefix(string position)
		{
			return position.Replace(BatteryElectricPrefix, "").Replace("E", "").Replace(HybridPrefix, "").Replace("P", "")
				.Replace("_", ".");
		}
		public static bool IsSerialHybrid(this PowertrainPosition pos)
		{
			return IsBatteryElectric(pos);
		}
	}

    public class EMPlacement
    {
        public EMPlacement(PowertrainPosition position, AxleNumber axleNumber)
        {
            Position = position;
            AxleNumber = axleNumber;
        }

        public PowertrainPosition Position { get; private set; }

        public AxleNumber AxleNumber { get; private set; }
    }
}