using System;
using System.Collections.Generic;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation
{
	public interface ISimulatorFactory
	{
		bool WriteModalResults { get; set; }
		bool ModalResults1Hz { get; set; }
		bool ActualModalData { get; set; }

		bool Validate { get; set; }
		SummaryDataContainer SumData { get; set; }
		int JobNumber { get; set; }
		IVectoRunDataFactory DataReader { get; }

		/// <summary>
		/// Creates powertrain and initializes it with the component's data.
		/// </summary>
		/// <returns>new VectoRun Instance</returns>
		IEnumerable<IVectoRun> SimulationRuns(Func<VectoRunData, bool> filter = null);
	}
}
