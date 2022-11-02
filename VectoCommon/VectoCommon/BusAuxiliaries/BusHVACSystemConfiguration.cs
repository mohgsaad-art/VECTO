using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries {
	public enum BusHVACSystemConfiguration
	{
		[GuiLabel("Unknown")]
		Unknown,

		[GuiLabel("Configuration 0")]
		Configuration0,

		[GuiLabel("Configuration 1")]
		Configuration1,

		[GuiLabel("Configuration 2")]
		Configuration2,

		[GuiLabel("Configuration 3")]
		Configuration3,

		[GuiLabel("Configuration 4")]
		Configuration4,

		[GuiLabel("Configuration 5")]
		Configuration5,

		[GuiLabel("Configuration 6")]
		Configuration6,

		[GuiLabel("Configuration 7")]
		Configuration7,

		[GuiLabel("Configuration 8")]
		Configuration8,

		[GuiLabel("Configuration 9")]
		Configuration9,

		[GuiLabel("Configuration 10")]
		Configuration10,
	}

	public static class BusHVACSystemConfigurationHelper
	{
		private const string Prefix = "Configuration";

		public static BusHVACSystemConfiguration? Parse(string text)
		{
			return (Prefix + text).ParseEnum<BusHVACSystemConfiguration>();
		}

		public static string GetLabel(this BusHVACSystemConfiguration? hvacConfiguration)
		{
			if (hvacConfiguration == BusHVACSystemConfiguration.Unknown) { 
					return BusHVACSystemConfiguration.Unknown.ToString();
			}

			return Prefix + " " + hvacConfiguration.ToString().Replace(Prefix, "");
		}

		public static string GetName(this BusHVACSystemConfiguration? hvacConfig)
		{
			if (hvacConfig == BusHVACSystemConfiguration.Unknown) {
				return "Unknown";
			}

			return hvacConfig.ToString().Replace(Prefix, "");
		}

		public static string GetXmlFormat(this BusHVACSystemConfiguration? hvacConfiguration)
		{
			if (hvacConfiguration == BusHVACSystemConfiguration.Unknown) {
				return "0";
			}

			return GetName(hvacConfiguration);
		}

		public static bool RequiresDriverAC(this BusHVACSystemConfiguration? hvacConfig)
		{
			if (hvacConfig == null) {
				return false;
			}
			switch (hvacConfig) {
				case BusHVACSystemConfiguration.Configuration2: 
				case BusHVACSystemConfiguration.Configuration4: 
				case BusHVACSystemConfiguration.Configuration7: 
				case BusHVACSystemConfiguration.Configuration9:
					return true;
			}

			return false;
		}

		public static bool RequiresPassengerAC(this BusHVACSystemConfiguration? hvacConfig)
		{
			if (hvacConfig == null) {
				return false;
			}
			switch (hvacConfig) {
				case BusHVACSystemConfiguration.Configuration1:
				case BusHVACSystemConfiguration.Configuration2:
				case BusHVACSystemConfiguration.Configuration3:
				case BusHVACSystemConfiguration.Configuration4:
					return false;
			}

			return true;
		}
	}
}