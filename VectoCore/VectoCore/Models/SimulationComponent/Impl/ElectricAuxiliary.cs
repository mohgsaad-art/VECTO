using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	internal class ElectricAuxiliary : IElectricAuxPort
	{
		protected readonly Dictionary<string, Func<Watt>> Auxiliaries = new Dictionary<string, Func<Watt>>();

		private IDataBus DataBus;

		public ElectricAuxiliary(VehicleContainer container)
		{
			DataBus = container;
		}

		#region Implementation of IElectricAuxPort

		public Watt Initialize()
		{
			return Auxiliaries.Sum(a => a.Value());
		}

		public Watt PowerDemand(Second absTime, Second dt, bool dryRun)
		{
			return Auxiliaries.Sum(a => a.Value());
		}

		#endregion

		public void AddConstant(string auxId, Watt powerDemand)
		{
			Add(auxId, () => powerDemand);
		}

		private void Add(string auxId, Func<Watt> powerLossFunction)
		{
			Auxiliaries[auxId] = powerLossFunction;
		}

		internal void AddConstant(string auxId, Watt powerDemand, Watt powerdemandStandstill)
		{
			Add(auxId, () => DataBus.VehicleInfo.VehicleStopped && DataBus.DrivingCycleInfo.StopTime.IsGreater(Constants.SimulationSettings.ThresholdStandstillOff) && DataBus.DrivingCycleInfo.TargetSpeed.IsEqual(0) ? powerdemandStandstill : powerDemand);
		}
	}
}