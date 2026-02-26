using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class ElectricPowerJunctionBox : StatefulVectoSimulationComponent<SimpleComponentState>, IElectricPowerJunctionBox
    {
		protected internal IElectricSystem _electricPower;
		private Dictionary<ITnOutPort, Watt> _powerDemands;
		private IElectricSystemResponse _electricPowerResponse;

		public ElectricPowerJunctionBox(IVehicleContainer container) : base(container, Constants.NOT_IN_AXLE_POWERTRAIN)
		{
			_powerDemands = new Dictionary<ITnOutPort, Watt>();
		}

		public ITnOutPort CurrentTorqueSplitterNextComponent {  get; set; }

		public void AddTorqueSplitterNextComponent(ITnOutPort component)
		{
			if (!_powerDemands.ContainsKey(component))
			{
				_powerDemands.Add(component, null);
			}
		}

		public void ClearResults()
		{
			_electricPowerResponse = null;

			foreach (var item in _powerDemands.Keys.ToList())
			{
				_powerDemands[item] = null;
			}
		}

		public IElectricSystemResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false)
		{
			if ((absTime.Value() == 0) && (powerDemand.Value() == 0) && dryRun)
			{
				return _electricPower.Request(absTime, dt, powerDemand, dryRun);
			}

            if (_powerDemands[CurrentTorqueSplitterNextComponent] == null)
			{
				_powerDemands[CurrentTorqueSplitterNextComponent] = powerDemand;	
			}

			return _powerDemands.Any(x => x.Value == null)
				? new ElectricSystemNotReadyResponse(this)
				: _electricPowerResponse = _electricPowerResponse ?? _electricPower.Request(absTime, dt, _powerDemands.Values.Sum(), dryRun);
		}

		public Watt ElectricAuxPower => _electricPower.ElectricAuxPower;
		public Watt ChargePower => _electricPower.ChargePower;
		public Watt BatteryPower => _electricPower.BatteryPower;
		public Watt ConsumerPower => _electricPower.ConsumerPower;
        public Watt FuelCellPower => _electricPower.FuelCellPower;

        public void Connect(IElectricChargerPort charger)
		{
			throw new NotImplementedException();
		}

        public void Connect(IElectricEnergyStorage battery)
		{
			throw new NotImplementedException();
		}

        public void Connect(IElectricAuxPort aux)
		{
			throw new NotImplementedException();
		}

        public void Connect(IElectricSystem powerSupply)
		{
			_electricPower = powerSupply;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
		}

		protected override bool DoUpdateFrom(object other)
		{
			if (other is ElectricPowerJunctionBox jb)
			{
				PreviousState = jb.PreviousState.Clone();
				return true;
			}

			return false;
		}

	}
}
