using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class DummyAxleGearInfo : VectoSimulationComponent, IAxlegearInfo
	{
		public DummyAxleGearInfo(IVehicleContainer container) : base(container) { }

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container) { }

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval) { }

		#endregion

		#region Implementation of IAxlegearInfo

		public Watt AxlegearLoss() => 0.SI<Watt>();

		public Tuple<PerSecond, NewtonMeter> CurrentAxleDemand => null;
		public double Ratio => 1;

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}
}