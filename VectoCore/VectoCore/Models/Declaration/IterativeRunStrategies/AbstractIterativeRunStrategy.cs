using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies
{
	public abstract class AbstractIterativeRunStrategy<T> : IIterativeRunStrategy where T : IIterativeRunResult, new()
	{

		protected Dictionary<int, T> _results = new Dictionary<int, T>(3);
		private PreRunOptions[] _preRunOptions;

		#region Implementation of IIterativeRunStrategy

		protected AbstractIterativeRunStrategy(PreRunOptions[] preRunOptions)
		{
			_preRunOptions = preRunOptions;
		}

		public abstract bool RunAgain(int iteration, IModalDataContainer modData, VectoRunData runData);

		public abstract void UpdateRunData(int iteration, IModalDataContainer modData, VectoRunData runData);
		public bool Enabled { get; set; } = true;
		public PreRunOptions GetPreRunOptions(int iteration)
		{
			if (_preRunOptions == null || _preRunOptions.Length < iteration + 1) {
				throw new VectoException($"No {nameof(PreRunOptions)} defined for iteration {iteration}", ErrorCodes.DECLARATIONDATA_MISSING_DATA);
			}
			return _preRunOptions[iteration];
		}

		#endregion
	}
}