using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    internal class ATClutchInfo : VectoSimulationComponent, IClutchInfo
	{
		public ATClutchInfo(IVehicleContainer container) : base(container)
		{
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion

		#region Implementation of IClutchInfo

		public bool ClutchClosed(Second absTime)
		{
			return true;
		}

		public Watt ClutchLosses => 0.SI<Watt>();

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}
}