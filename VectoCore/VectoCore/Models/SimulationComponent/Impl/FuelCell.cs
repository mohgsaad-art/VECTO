using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class FuelCell : StatefulVectoSimulationComponent<FuelCell.FuelCellState>
	{
		public class FuelCellState
		{
			public Watt RequestedPower { get; set; }
			public Watt Power { get; set; }
			public KilogramPerSecond FuelConsumption { get; set; }
			public Watt MinEffPower { get; set; }
			public Second TimeShare { get; set; }

			public double ShareOn { get; set; }
		}

		private FuelCellData ModelData;
		private Watt _minEffPower;

		public Watt MaxPower
		{
			get => ModelData.MaxElectricPower;
		}
		public Watt MinPower
		{
			get => ModelData.MinElectricPower;
		}

		public Watt FCSRatedPower => ModelData.FCSRatedPower;

        public FuelCellData.FuelCellId Id { get; private set; }


		public FuelCell(FuelCellData fcData, IVehicleContainer dataBus, FuelCellData.FuelCellId id) : 
			base(null) //provide null here, when registering the component the Id is accessed but is not set in the base constructor
		{
			DataBus = dataBus;
			ModelData = fcData;
			Id = id;
			dataBus.AddComponent(this);
			_minEffPower = ModelData.MassFlowMap.MinEffPower;
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var power = CurrentState.Power;
			container[ModalResultField.P_FCS, Id] = power ?? 0.SI<Watt>();
			container[ModalResultField.FC_FCS, Id] = CurrentState.FuelConsumption ?? 0.SI<KilogramPerSecond>();
			container[ModalResultField.t_FCS_On, Id] = CurrentState.TimeShare;

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		public Watt Request(Watt requestedPower, Second dt, bool dryRun, bool allowTimeSlicing)
		{
			var timeShare = dt;
			var timeShareFactor = 1.0d;
            if (allowTimeSlicing) {
				if (requestedPower.IsSmaller(_minEffPower))
				{
					timeShareFactor = (requestedPower / _minEffPower);
					System.Diagnostics.Debug.Assert(timeShareFactor.IsSmallerOrEqual(1));
					timeShare = timeShareFactor * timeShare;
				}
			}

            var generatedPower = requestedPower; //Handle time slicing


			var h2 = ModelData.MassFlowMap.Lookup(generatedPower, allowTimeSlicing);

            if (!dryRun) {
				CurrentState.RequestedPower = requestedPower;
				CurrentState.Power = generatedPower;
				CurrentState.FuelConsumption = h2;
				CurrentState.TimeShare = timeShare;
				CurrentState.ShareOn = timeShareFactor;
			}


			return generatedPower;
		}

		protected override bool DoUpdateFrom(object other)
		{
			if (other is FuelCell fc) {
				PreviousState = fc.PreviousState;
				return true;
			}

			return false;
		}

		#endregion
	}
}