using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Models.Connector.Ports.Impl
{
	public interface IBatteryProvider
	{
		IElectricEnergyStoragePort MainBatteryPort { get; }
	}

	public interface IElectricEnergyStoragePort
	{

		void Initialize(double initialSoC);

		/// <summary>
		/// Convention: positive powerdemand charges the battery, 
		///             negative powerdemand discharges
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="powerDemand"></param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		IRESSResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun);

	}

	public interface IElectricAuxConnector
	{
		void Connect(IElectricAuxPort aux);
	}

	public interface IElectricChargerConnector
	{
		void Connect(IElectricChargerPort charger);
	}

	public interface IBatteryConnector
	{
		void Connect(IElectricEnergyStorage battery);
	}

	public interface IElectricAuxPort
	{
		Watt Initialize();

		Watt PowerDemand(Second absTime, Second dt, bool dryRun);
	}

	public interface IElectricChargerPort : IUpdateable
	{
		Watt Initialize();

		Watt PowerDemand(Second absTime, Second dt, Watt powerDemandEletricMotor, Watt auxPower, bool dryRun);
	}

	public interface ITestpowertrainGensetChargerAdapter : IElectricChargerPort
	{

	}
	
	public interface IFuelCellPort
	{
		Watt Initialize();
		Watt PowerDemand(Second absTime, Second dt, Watt maxPower, bool dryRun);
	}
}