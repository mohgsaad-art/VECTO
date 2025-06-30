using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IElectricSystemInfo 
	{


		Watt ElectricAuxPower { get; }

		Watt ChargePower { get; }

		Watt BatteryPower { get; }

		Watt ConsumerPower { get; }
	}

	public interface IElectricSystem : IElectricSystemInfo, IBatteryConnector, IElectricChargerConnector, IElectricAuxConnector
    {
		IElectricSystemResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false);

		void Connect(IElectricChargerPort charger);
	}

	public interface IElectricEnergyStorage : IBatteryProvider, IRESSInfo, IElectricEnergyStoragePort
    {
		
	}

	public interface ITestpowertrainElectricSystem : IElectricSystem
	{
		IList<IElectricChargerPort> Charger { get; }
	}

}