using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	public interface IDCDCConverter : IElectricAuxPort
	{
		void ConsumerEnergy(WattSecond electricConsumerPower, bool dryRun);
	}
}