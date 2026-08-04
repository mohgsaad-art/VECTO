using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

	public class FuelCellString : StatefulVectoSimulationComponent<FuelCellString.FuelCellStringState>
	{
		public class FuelCellStringState
		{
			public KilogramPerSecond FuelConsumption { get; set; }
			public Watt Power { get; set; }
			public Watt RequestedPower { get; set; }
		}

		private readonly FuelCellStringMassFlowMap _fcStringMap;
		private readonly int _stringId;

		public string StringId => _stringId.ToString(CultureInfo.InvariantCulture);
		private IList<FuelCell> _fuelCells = new List<FuelCell>();

		public IReadOnlyCollection<FuelCell> FuelCells => new ReadOnlyCollection<FuelCell>(_fuelCells);

		public FuelCellString(FuelCellStringData fcData, int stringId, IVehicleContainer dataBus) : 
			base(null, Constants.NOT_IN_AXLE_POWERTRAIN) //provide null here, when registering the component the Id is accessed but is not set in the base constructor
        {
			_fcStringMap = fcData.MassFlowMap;
			_stringId = stringId;

			for (var i = 0; i < fcData.FcCount; i++) {
				var fc = new FuelCell(fcData.FcData, dataBus,
					new FuelCellData.FuelCellId() { Id = stringId, SubId = (i + 1) });
				_fuelCells.Add(fc);
			}

			DataBus = dataBus;
			dataBus.AddComponent(this);
		}



		public Watt Request(Watt requestedPower, bool dryRun, Second dt)
		{
			var fcCount = _fcStringMap.GetActiveFuelCellCount(requestedPower);
			var generatedPower = 0.SI<Watt>();
			var fc = 0.SI<KilogramPerSecond>();
			foreach (var fuelCell in _fuelCells.Take(fcCount)) {
				generatedPower += fuelCell.Request(requestedPower / fcCount, dt, dryRun, (fcCount == 1) && (requestedPower.IsSmallerOrEqual(_fcStringMap.MinPower)));
				if (!dryRun) {
					fc += (fuelCell.CurrentState.FuelConsumption);
#if DEBUG
					if (fcCount != 1) {
						System.Diagnostics.Debug.Assert(fuelCell.CurrentState.TimeShare.IsEqual(dt));
					}
#endif
				}

			}

			if (!dryRun) {
				CurrentState.RequestedPower = requestedPower;
				CurrentState.Power = generatedPower;
				CurrentState.FuelConsumption = fc;
			}

			return generatedPower;
		}


		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval,
			IModalDataContainer container)
		{
			container[ModalResultField.P_FCS, StringId] = CurrentState.Power ?? 0.SI<Watt>();
			container[ModalResultField.FC_FCS, StringId] = CurrentState.FuelConsumption ?? 0.SI<KilogramPerSecond>();
        }

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		protected override bool DoUpdateFrom(object other)
		{
			if (other is FuelCellString fcs) {
				PreviousState = fcs.PreviousState;
				return true;
			}

			return false;

			#endregion
		}
	}
}