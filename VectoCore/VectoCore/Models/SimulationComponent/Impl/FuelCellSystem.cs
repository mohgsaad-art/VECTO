using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TestpowertrainFuelCellSystem : FuelCellSystem
    {
		protected FuelCellSystemData ModelData { get; set; }
		
		
		public TestpowertrainFuelCellSystem(IVehicleContainer container, FuelCellSystemData modelData) : base(container, modelData, false)
		{
			if (!container.IsTestPowertrain) {
				throw new VectoException("Component shall not be used in real powertrain!");
			}
        }

		#region Overrides of VectoSimulationComponent

		public override bool UpdateFrom(object other)
		{
			return true;
		}

		#endregion
	}


	public class FuelCellSystem : StatefulVectoSimulationComponent<FuelCellSystem.State>, IFuelCellSystem
	{
		private readonly IList<FuelCellString> _fuelCellStrings;

		public class State
		{
			public Watt Power { get; set; }

			public FuelCellSystemShareMap.FuelCellShare Share { get; set; }
		}
		private readonly IMileageCounter _mileageCounter;
		private readonly FuelCellSystemShareMap _fuelCellShareMap;

		public FuelCellSystem(IVehicleContainer container, FuelCellSystemData modelData) : this(container, modelData,
			false)
		{
			if (container.IsTestPowertrain) {
				throw new VectoException("Component shall not be used in testpowertrain!");
			}
        }

		protected FuelCellSystem(IVehicleContainer container, FuelCellSystemData modelData, bool dummy) : 
			base(container, Constants.NOT_IN_AXLE_POWERTRAIN)
		{
			

			_fuelCellShareMap = modelData.FuelCellShareMap;
			_mileageCounter = container.MileageCounter;
			_fuelCellStrings = new List<FuelCellString>();
			ModelData = modelData;

			var id = 1;
			foreach (var fuelCell in modelData.FuelCellStrings) {
				var fcs = new FuelCellString(fuelCell, id++, dataBus: container);
				AddFuelCellString(fcs);
			}

            Initialize();
		}

		public IReadOnlyCollection<FuelCellString> FuelCellStrings => new ReadOnlyCollection<FuelCellString>(_fuelCellStrings);

		private FuelCellSystemData ModelData { get; set; }

		#region Implementation of IElectricChargerPort

		public Watt Initialize()
		{

			//var distance = _mileageCounter.Distance;
			var power = ModelData.FuelCellPowerMap.InitPower;
			PreviousState.Power = power;
			return power;
		}

		public Watt PowerDemand(Second absTime, Second dt, Watt maxPower, bool dryRun)
		{
			var targetPower = ModelData.ChargingPower(_mileageCounter.Distance).LimitTo(0.SI<Watt>(), maxPower);

			var shareResult = _fuelCellShareMap.Lookup(targetPower, PreviousState?.Share);

			var generatedPower = 0.SI<Watt>();

			generatedPower += _fuelCellStrings[0].Request(targetPower * shareResult.Share.ShareA, dryRun, dt);

			if (_fuelCellStrings.Count > 1) {
				generatedPower += _fuelCellStrings[1].Request(targetPower * shareResult.Share.ShareB, dryRun, dt);
			}

			if (!dryRun) {
				CurrentState.Share = shareResult.Share;
				CurrentState.Power = generatedPower;
			}

			return generatedPower;
        }


		public void AddFuelCellString(FuelCellString fuelCellString)
		{
			_fuelCellStrings.Add(fuelCellString);
		}

		#endregion


		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_FCSystem] = CurrentState.Power;
			container[ModalResultField.FC_FCSystem] = _fuelCellStrings.Sum(x => x.PreviousState.FuelConsumption);
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		protected override bool DoUpdateFrom(object other)
		{
			if (other is FuelCellSystem fc) {
				PreviousState = fc.PreviousState;
				return true;
			}

			return false;
		}

		#endregion
	}


}