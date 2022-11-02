using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	// This class is reflective of the stored entries for the combined alternator
	// And is used by the Combined Alternator Form and any related classes.

	public class CombinedAlternatorMapRow : ICombinedAlternatorMapRow
	{
		public string AlternatorName { get; }
		public PerSecond RPM { get; }
		public Ampere Amps { get; }
		public double Efficiency { get; }
		public double PulleyRatio { get; }

		public CombinedAlternatorMapRow(
			string alternatorName, PerSecond rpm, Ampere amps, double efficiency, double pulleyRatio)
		{
			// Sanity Check
			if (alternatorName.Trim().Length == 0) {
				throw new ArgumentException("Alternator name cannot be zero length");
			}
			if (efficiency < 0 | efficiency > 100) {
				throw new ArgumentException("Alternator Efficiency must be between 0 and 100");
			}
			if (pulleyRatio <= 0) {
				throw new ArgumentException("Alternator Pully ratio must be a positive number");
			}

			// Assignments
			AlternatorName = alternatorName;
			RPM = rpm;
			Amps = amps;
			Efficiency = efficiency;
			PulleyRatio = pulleyRatio;
		}
	}
}
