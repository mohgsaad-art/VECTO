using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TestpowertrainGensetChargerAdapter : GensetChargerAdapter, ITestpowertrainGensetChargerAdapter
    {
		public TestpowertrainGensetChargerAdapter(IElectricMotor motor) : base(motor, false)
		{
			// check that it is used in testpowertrain
		}
	}


    public class GensetChargerAdapter : IElectricChargerPort
	{
		protected IElectricSystem es;
		protected Watt PowerGenerated;

		public GensetChargerAdapter(IElectricMotor motor) : this(motor, false)
		{
			// check if not used in testpowertrain
		}

		protected GensetChargerAdapter(IElectricMotor motor, bool dummy)
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
						InternalVoltage = 0.SI<Volt>()
					},
					ConsumerPower = powerDemand,
					ConnectionSystemResistance = 0.SI<Ohm>(),
				};
			}

			public void Connect(IElectricChargerPort charger)
			{
				throw new System.NotImplementedException();
			}

			#endregion

			#region Implementation of IBatteryConnector

			public void Connect(IElectricEnergyStorage battery)
			{
				throw new System.NotImplementedException();
			}

			#endregion

			#region Implementation of IElectricAuxConnector

			public void Connect(IElectricAuxPort aux)
			{
				throw new System.NotImplementedException();
			}

			#endregion
		}

		#region Implementation of IUpdateable

		public bool UpdateFrom(object other) {
			if (other is GenSetOperatingPoint p) {
				ChargingPower = p.ElectricPower;
				return true;
			}
			return false;
		}

		#endregion
	}
}