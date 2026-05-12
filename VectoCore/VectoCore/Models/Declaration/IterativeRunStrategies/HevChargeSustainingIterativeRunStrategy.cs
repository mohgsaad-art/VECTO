using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies
{
	public class
		HevChargeSustainingIterativeRunStrategy : AbstractIterativeRunStrategy<HevChargeSustainingIterativeRunStrategy.OVCHevIterativeRunResult>
	{
		#region Implementation of IIterativeRunStrategy

		public override bool RunAgain(int iteration, IModalDataContainer modData, VectoRunData runData)
		{

			return Enabled && (iteration < 2);
		}

		public override void UpdateRunData(int iteration, IModalDataContainer modData, VectoRunData runData)
		{
			StoreResults(modData, runData, iteration);
			runData.Iteration = (iteration + 1);
			SetEquivalenceFactor(runData, iteration);
		}

		private void StoreResults(IModalDataContainer modData, VectoRunData runData, int iteration)
		{
			_results.Add(iteration, new OVCHevIterativeRunResult() {
				d_soc = modData.REESSDeltaSoc(),
				f_equiv = runData.HybridStrategyParameters.EquivalenceFactor,
			});
		}

		#endregion

		public class OVCHevIterativeRunResult : IIterativeRunResult
		{
			public double f_equiv;
			public double d_soc;
		}
		



		private void SetEquivalenceFactor(VectoRunData runData, int iteration)
		{
			var factorCharge = runData.HybridStrategyParameters.EquivalenceFactorCharge /
								runData.HybridStrategyParameters.EquivalenceFactor;
			var factorDischarge = runData.HybridStrategyParameters.EquivalenceFactorDischarge /
								runData.HybridStrategyParameters.EquivalenceFactor;


			double f_equiv_1, f_equiv_2, f_equiv_3;
			double d_soc_1, d_soc_2;
			switch (iteration) {
				case 0:
					///1. iteration
					var k_0_4 = DeclarationData.HEVStrategyParameters.LookupSlope(runData.Mission.MissionType,
						runData.Mission.BusParameter?.BusGroup ?? runData.VehicleData.VehicleClass, runData.Loading);
					var soc_usable = runData.HybridStrategyParameters.MaxSoC - runData.HybridStrategyParameters.MinSoC;

					var k = k_0_4 + 0.2 * (soc_usable - 0.4); //soc usable < 0.4?

					f_equiv_1 = _results[iteration].f_equiv;
					d_soc_1 = _results[iteration].d_soc;
					f_equiv_2 = f_equiv_1 - (d_soc_1/100 / k);

					f_equiv_2 = f_equiv_2.LimitTo(DeclarationData.HEV_EquivalenceFactor_Min,
						DeclarationData.HEV_EquivalenceFactor_Max);
					runData.HybridStrategyParameters.EquivalenceFactor = f_equiv_2;
					runData.HybridStrategyParameters.EquivalenceFactorCharge = f_equiv_2 * factorCharge;
					runData.HybridStrategyParameters.EquivalenceFactorDischarge = f_equiv_2 * factorDischarge;
					break;
				case 1:

					d_soc_1 = _results[0].d_soc / 100;
					d_soc_2 = _results[1].d_soc / 100;
					f_equiv_1 = _results[0].f_equiv;
					f_equiv_2 = _results[1].f_equiv;

					if (d_soc_1.IsGreater(0) && d_soc_2.IsGreater(0) && d_soc_1.IsEqual(d_soc_2, 1e-5)) {
						f_equiv_3 = DeclarationData.HEV_EquivalenceFactor_Min;
					} else if (d_soc_1.IsSmaller(0) && d_soc_2.IsSmaller(0) && d_soc_1.IsEqual(d_soc_2, 1e-5)) {
						f_equiv_3 = DeclarationData.HEV_EquivalenceFactor_Max;
					} else {
						f_equiv_3 = (((0 - d_soc_1) / (d_soc_2 - d_soc_1)) * (f_equiv_2 - f_equiv_1)) + f_equiv_1;
					}

					f_equiv_3 = f_equiv_3.LimitTo(DeclarationData.HEV_EquivalenceFactor_Min, DeclarationData.HEV_EquivalenceFactor_Max);
					runData.HybridStrategyParameters.EquivalenceFactor = f_equiv_3;
					runData.HybridStrategyParameters.EquivalenceFactorCharge = f_equiv_3 * factorCharge;
					runData.HybridStrategyParameters.EquivalenceFactorDischarge = f_equiv_3 * factorDischarge;

					break;
				default:
					throw new VectoException($"Iteration {iteration} not implemented", ErrorCodes.DECLARATIONDATA_INVALID_ITERATION);
			}
		}

		public HevChargeSustainingIterativeRunStrategy() : base(new PreRunOptions[3] {
			new PreRunOptions(),
			new PreRunOptions(),
			new PreRunOptions()
		})
		{

		}
	}
}