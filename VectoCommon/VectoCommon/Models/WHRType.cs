using System;
using System.Collections.Generic;

namespace TUGraz.VectoCore.Models.Declaration {

	[Flags]
	public enum WHRType
	{
		None = 0,
		MechanicalOutputICE = 1,
		MechanicalOutputDrivetrain = 2,
		ElectricalOutput = 4,
	}

	public static class WHRTypeHelper
	{
		public static string ToXMLFormat(this WHRType whrType)
		{
			var options = new List<string>();
			if ((whrType & WHRType.MechanicalOutputICE) != 0) {
				options.Add("mechanical output to ICE");
			}
			if ((whrType & WHRType.MechanicalOutputDrivetrain) != 0) {
				options.Add("mechanical output to drivetrain");
			}
			if ((whrType & WHRType.ElectricalOutput) != 0) {
				options.Add("electrical output");
			}

			return options.Count == 0 ? "none" : string.Join(", ", options);
		}
		
		public static bool IsElectrical(this WHRType whrType)
		{
			return (whrType & WHRType.ElectricalOutput) != 0;
		}

		public static bool IsMechanical(this WHRType whrType)
		{
			return (whrType & WHRType.MechanicalOutputICE) != 0 || (whrType & WHRType.MechanicalOutputDrivetrain) != 0;
		}
	}
}