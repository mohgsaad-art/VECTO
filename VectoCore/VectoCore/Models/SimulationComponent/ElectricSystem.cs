using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public class ElectricSystem : StatefulVectoSimulationComponent<ElectricSystem.State>, IElectricSystem, IElectricAuxConnecor, IElectricChargerConnector, IBatteryConnector
	{

		protected readonly List<IElectricAuxPort> Consumers = new List<IElectricAuxPort>();

		public IList<IElectricChargerPort> Charger { get;  }

		protected IElectricEnergyStorage Battery;

		public ElectricSystem(IVehicleContainer container) : base(container)
		{
			Charger = new List<IElectricChargerPort>();
		}

		public IElectricSystemResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false)
		{
			var auxDemand = Consumers.Sum(x => x.PowerDemand(absTime, dt, dryRun)).DefaultIfNull(0);
			var chargePower = Charger.Count == 0 ? 0.SI<Watt>() : Charger.Sum(x => x.PowerDemand(absTime, dt, powerDemand, auxDemand, dryRun));
			var totalPowerDemand = powerDemand + chargePower - auxDemand;

			var batResponse = Battery.MainBatteryPort.Request(absTime, dt, totalPowerDemand, dryRun);

			var response = dryRun
				? (AbstractElectricSystemResponse)new ElectricSystemDryRunResponse(this)
				: new ElectricSystemResponseSuccess(this);

			if (batResponse is RESSOverloadResponse)
			{
				response = new ElectricSystemOverloadResponse(this);
			}
			if (batResponse is RESSUnderloadResponse)
			{
				response = new ElectricSystemUnderloadResponse(this);
			}

			if (!dryRun)
			{
				CurrentState.SetState(powerDemand, auxDemand, chargePower, batResponse.PowerDemand);
			}

			response.AbsTime = absTime;
			response.SimulationInterval = dt;
			response.RESSResponse = batResponse;
			response.RESSPowerDemand = totalPowerDemand;
			response.ConsumerPower = powerDemand;
			response.AuxPower = auxDemand;
			response.ChargingPower = chargePower;
			return response;
		}

		public Watt ElectricAuxPower => PreviousState.AuxPower;
		public Watt ChargePower => PreviousState.ChargePower;
		public Watt BatteryPower => PreviousState.BatteryPower;
		public Watt ConsumerPower => PreviousState.ConsumerPower;

		protected override void DoWriteModalResults(Second absTime, Second dt, IModalDataContainer container)
		{
			container[ModalResultField.P_aux_el] = CurrentState.AuxPower;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}


		#region Implementation of IBatteryChargeProvider

		public void Connect(IElectricChargerPort charger)
		{
			Charger.Add(charger);
		}

		#endregion

		#region Implementation of IBatteryAuxOutProvider

		public void Connect(IElectricAuxPort aux)
		{
			if (Consumers.Contains(aux)) { return; }
			Consumers.Add(aux);
		}

		#endregion

		#region Implementation of IBatteryConnector

		public void Connect(IElectricEnergyStorage battery)
		{
			if (Battery != null)
			{
				throw new VectoException("Battery is already connected!");
			}
			Battery = battery;
		}

		#endregion

		#region Implementation of IRESSInfo

		public Volt InternalCellVoltage => Battery.InternalVoltage;

		public double StateOfCharge => Battery.StateOfCharge;

		public Watt MaxChargePower(Second dt)
		{
			return Battery.MaxChargePower(dt);
		}

		public Watt MaxDischargePower(Second dt)
		{
			return Battery.MaxDischargePower(dt);
		}


		#endregion

		public class State
		{
			public Watt AuxPower = 0.SI<Watt>();
			public Watt ChargePower = 0.SI<Watt>();
			public Watt ConsumerPower = 0.SI<Watt>();
			public Watt BatteryPower = 0.SI<Watt>();

			public void SetState(Watt powerDemand, Watt auxDemand, Watt chargePower, Watt batteryPower)
			{
				AuxPower = auxDemand;
				ChargePower = chargePower;
				ConsumerPower = powerDemand;
				BatteryPower = batteryPower;
			}
		}

	}
}