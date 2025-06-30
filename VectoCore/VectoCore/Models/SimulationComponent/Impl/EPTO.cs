using Microsoft.VisualBasic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Auxiliaries;
using TUGraz.VectoCore.OutputData;
using Constants = TUGraz.VectoCore.Configuration.Constants;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public interface IEPTO
	{
		bool EPTOOn(IDataBus dataBus);
	}

	public class EPTO : VectoSimulationComponent, IAuxDemand, IEPTO, IUpdateable
	{
		private readonly IPTOCycleController _ptoCycleController;
		private readonly IDataBus _dataBus;

		public EPTO(IPTOCycleController cycleController, IVehicleContainer dataBus) : base(dataBus)
		{
			_dataBus = dataBus;
			_ptoCycleController = cycleController;
		}

		#region Implementation of IAuxDemand
		public Watt PowerDemand(IDataBus dataBus)
		{
			if (dataBus.DrivingCycleInfo.PTOActive) {
				return _ptoCycleController.CycleData.LeftSample.PTOElectricalPowerDemand ?? 0.SI<Watt>();
			}

			return 0.SI<Watt>();
		}

		public bool EPTOOn(IDataBus dataBus)
		{
			return !PowerDemand(dataBus).IsEqual(0);
		}


		public string AuxID => Constants.Auxiliaries.IDs.PTOConsumer;

		#endregion

		#region Implementation of IUpdateable

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		protected override bool DoUpdateFrom(object other)
		{
			if (other is EPTO epto) {
				if (this._ptoCycleController is IUpdateable updateablePtoCycle) {
					return updateablePtoCycle.UpdateFrom(epto._ptoCycleController);
				}
			}

			return false;
		}

		#endregion
	}
}