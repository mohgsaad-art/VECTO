using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;

namespace TUGraz.VectoCore.Tests.Utils {
	public class MockElectricConsumer : IElectricAuxPort
	{
		private Watt PowwerDemand;

		public MockElectricConsumer(Watt powerDemand)
		{
			PowwerDemand = powerDemand;
		}

		public Watt Initialize()
		{
			return PowwerDemand;
		}

		public Watt PowerDemand(Second absTime, Second dt, bool dryRun)
		{
			return PowwerDemand;
		}

	}
}