using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies
{
    public class FCHEVIterativeRunStrategy : AbstractIterativeRunStrategy<FCHEVIterativeRunStrategy.FCHEVIterativeRunResult>
	{
		public delegate void DoUpdateRunData(IModalDataContainer modData, VectoRunData runData); 
		
		public DoUpdateRunData Update { get; set; }

		private bool firstRunPerformed = false;
		
		public class FCHEVIterativeRunResult : IIterativeRunResult
		{

		}

		public FCHEVIterativeRunStrategy(PreRunOptions[] preRunOptions) : base(preRunOptions) { }


        #region Overrides of AbstractIterativeRunStrategy<FCHEVIterativeRunResult>

        public override bool RunAgain(int iteration, IModalDataContainer modData, VectoRunData runData)
		{
			if (!firstRunPerformed) {
				firstRunPerformed = true;
				
				return true;
			}

			return false;

		}

		public override void UpdateRunData(int iteration, IModalDataContainer modData, VectoRunData runData)
		{
			if (Update != null) {
				Update(modData, runData);

			}




            //Perform necessary changes to the rundata for the next run
        }

		#endregion

	}
}
