using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Tests.TestUtils
{
	internal static class RunDataHelper
	{
		public static VectoRunData UpdateFequiv(this VectoRunData rd, double f_equiv)
		{
			if (rd.HybridStrategyParameters == null) {
				return rd;
			}

			var factorCharge = rd.HybridStrategyParameters.EquivalenceFactorCharge /
								rd.HybridStrategyParameters.EquivalenceFactor;
			var factorDischarge = rd.HybridStrategyParameters.EquivalenceFactorDischarge /
								rd.HybridStrategyParameters.EquivalenceFactor;
			rd.HybridStrategyParameters.EquivalenceFactor = f_equiv;
			rd.HybridStrategyParameters.EquivalenceFactorDischarge = f_equiv * factorDischarge;
			rd.HybridStrategyParameters.EquivalenceFactorCharge = f_equiv * factorCharge;
			return rd;
		}
	}
}
