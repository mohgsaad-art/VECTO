using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation.Data;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class PreRunOptions
	{
		/// <summary>
		/// Determines if the Mod data should be written for this iteration AND if an entry should be added to the sumfile
		/// default true
		/// </summary>
		public bool WriteModAndSumData { get; set; } = true;
	}




	public interface IFollowUpRunCreator
	{
		bool RunAgain(Action<VectoRunData> runAgainAction, IVectoRun run, Action<PreRunOptions> beforeNextRun);

		// in case a follow-up run is indicated by RunAgain, this property needs to provide the Powertrain builder to create the 
		// follow-up run
		IPowertrainBuilder PowertrainBuilder { get; }
	}


	public class NoFollowUpRunCreator : LoggingObject, IFollowUpRunCreator
	{
		#region Implementation of IFollowUpRunCreator

		public bool RunAgain(Action<VectoRunData> runAgainAction, IVectoRun run, Action<PreRunOptions> beforeNextRun)
		{
			return false;
		}

		// as no follow-up run is created the powertrain builder is not necessary
		public IPowertrainBuilder PowertrainBuilder => null;

		#endregion
	}

	/// <summary>
	/// This class decides if a run is executed another time, stores the relevant data from the current run and prepares the VehicleContainer for the next run
	/// </summary>
	public class FollowUpRunCreator : LoggingObject, IFollowUpRunCreator
	{
		private string _originalModfileSuffix = null;
		private int _iteration = 0;

		private readonly IIterativeRunStrategy _strategy;

		public IPowertrainBuilder PowertrainBuilder { get; private set; }

		public FollowUpRunCreator(IIterativeRunStrategy strategy, IPowertrainBuilder ptBuilder)
		{
			_strategy = strategy;
			PowertrainBuilder = ptBuilder;
		}

		/// <summary>
		/// Determines if a run should be simulated again, if the run should be simulated again,
		/// necessary simulation components are reset, the rundata is adjusted and the runAgainAction is executed
		/// </summary>
		/// <param name="runAgainAction"></param>
		/// <param name="run"></param>
		/// <return>true if the run is executed again, false otherwise</return>
		public bool RunAgain(Action<VectoRunData> runAgainAction, IVectoRun run, Action<PreRunOptions> beforeNextRun)
		{
			var modalDataContainer = run.GetContainer().ModalData;
			var vectoRunData = run.GetContainer().RunData;
			if (!_strategy.RunAgain(_iteration, modalDataContainer, vectoRunData)) {
				return false;
			}
			_originalModfileSuffix = _originalModfileSuffix ?? (_originalModfileSuffix = vectoRunData.ModFileSuffix);

			Log.Info(string.Format("Run {0} again!", run.RunName));

			var options = _strategy.GetPreRunOptions(_iteration);
			beforeNextRun(options);

			vectoRunData.ModFileSuffix = _originalModfileSuffix + (_iteration + 1);
			_strategy.UpdateRunData(_iteration, modalDataContainer, vectoRunData);
			_iteration++;
			runAgainAction(vectoRunData);
			return true;
		}
	}
}