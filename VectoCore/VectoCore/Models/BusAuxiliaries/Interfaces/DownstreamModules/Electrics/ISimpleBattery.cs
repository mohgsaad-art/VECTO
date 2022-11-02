using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics {

	public interface ISimpleBatteryInfo
	{
		double SOC { get; }
		WattSecond Capacity { get; }

	}

	public interface ISimpleBattery  : ISimpleBatteryInfo
	{
		WattSecond ConsumedEnergy { get; }

		//void Request(WattSecond energy);
		void ConsumeEnergy(WattSecond energy, bool dryRun);
		WattSecond MaxChargeEnergy();
		WattSecond MaxDischargeEnergy();
	}
}