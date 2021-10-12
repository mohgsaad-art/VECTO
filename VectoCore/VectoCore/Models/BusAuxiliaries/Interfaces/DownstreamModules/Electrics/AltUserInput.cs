using System;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	// Used by the Combined Alternator Form/Classes to accept user input for the combined alternators efficiency
	// At different Current Demands
	public class AltUserInput<T> where T: SI
	{
		public T Amps;
		public double Eff;

		// Constructor
		public AltUserInput(T amps, double eff)
		{
			Amps = amps;
			Eff = eff;
		}

		// Equality
		public bool IsEqual(AltUserInput<T> other, int rounding = 7)
		{
			return Amps != null && other != null && Amps.IsEqual(other.Amps, Math.Pow(10, -rounding).SI<Ampere>()) &&
					Eff.IsEqual(other.Eff, Math.Pow(10, -rounding));
		}
	}
}
