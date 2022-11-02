using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class GensetChargerAdapter : IElectricChargerPort
	{
		protected IElectricSystem es;
		protected Watt PowerGenerated;

		public GensetChargerAdapter(ElectricMotor motor)
		{
			es = new ChargeElectricSystem(this);
			motor?.Connect(es);
			PowerGenerated = 0.SI<Watt>();
		}

		#region Implementation of IElectricChargerPort

		public Watt Initialize()
		{
			PowerGenerated = 0.SI<Watt>();
			return PowerGenerated;
		}

		public Watt PowerDemand(Second absTime, Second dt, Watt powerDemandEletricMotor, Watt auxPower, bool dryRun)
		{
			return PowerGenerated;
		}

		public Watt ChargingPower
		{
			set { PowerGenerated = value; }
		}

		#endregion

		public class ChargeElectricSystem : IElectricSystem
		{
			protected GensetChargerAdapter Adapter;


			public ChargeElectricSystem(GensetChargerAdapter adapter)
			{
				Adapter = adapter;
			}

			#region Implementation of IElectricSystemInfo

			public Watt ElectricAuxPower => 0.SI<Watt>();
			public Watt ChargePower => 0.SI<Watt>();
			public Watt BatteryPower => 0.SI<Watt>();
			public Watt ConsumerPower => 0.SI<Watt>();
			public IElectricSystemResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false)
			{
				if (!dryRun) {
					Adapter.PowerGenerated = powerDemand;
				}

				return new ElectricSystemResponseSuccess(this) {
					RESSResponse = new RESSResponseSuccess(this) {
						MaxChargePower = double.MaxValue.SI<Watt>(),
						MaxDischargePower = -double.MaxValue.SI<Watt>(),
						AbsTime = absTime,
					},
					ConsumerPower = powerDemand
				};
			}

			public void Connect(IElectricChargerPort charger)
			{
				throw new System.NotImplementedException();
			}

			#endregion
		}
	}
}