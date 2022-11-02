using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface IPneumaticUserInputsConfig
	{
		ICompressorMap CompressorMap { get; }
		double CompressorGearEfficiency { get; }
		double CompressorGearRatio { get; }
		bool SmartAirCompression { get; }

		bool SmartRegeneration { get; }

		//bool RetarderBrake { get;  }
		Meter KneelingHeight { get; }

		ConsumerTechnology AirSuspensionControl { get; } // mechanical or electrical
		ConsumerTechnology AdBlueDosing { get; } // pnmeumatic or electric
		ConsumerTechnology? Doors { get; } // pneumatic or electric
	}

	public enum ConsumerTechnology
	{
		Unknown,
		Mechanically,
		Electrically,
		Pneumatically,
		Mixed
	}

	
	public static class ConsumerTechnologyHelper
	{
		public static ConsumerTechnology Parse(string technology)
		{
			switch (technology.ToLowerInvariant())
			{
				case "mechanically":
				case "mechanic":
					return ConsumerTechnology.Mechanically;
				case "electrically":
				case "electric":
				case "electronically":
					return ConsumerTechnology.Electrically;
				case "pneumatically":
				case "pneumatic":
					return ConsumerTechnology.Pneumatically;
				case "mixed":
					return ConsumerTechnology.Mixed;
				default:
					return ConsumerTechnology.Unknown;
			}
		}

		public static string GetLabel(this ConsumerTechnology? technology)
		{
			switch (technology) {
				case ConsumerTechnology.Electrically:
					return "Electric";
				case ConsumerTechnology.Mechanically:
					return "Mechanic";
				case ConsumerTechnology.Pneumatically:
					return "Pneumatic";
				case ConsumerTechnology.Mixed:
					return "Mixed";
				default:
					return ConsumerTechnology.Unknown.ToString();
			}
		}

		public static string ToXMLFormat(this ConsumerTechnology? technology)
		{
			return technology.GetLabel().ToLowerInvariant();
		}
	}
}
