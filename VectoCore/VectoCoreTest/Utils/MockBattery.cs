using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Tests.Utils {
	public class MockBattery : IElectricEnergyStorage, IElectricEnergyStoragePort, IElectricAuxConnecor
	{
		public Volt InternalVoltage => 640.SI<Volt>();

		public IRESSResponse Request(Second absTime, Second dt, Watt powerdemand, bool dryRun = false)
		{
			return new RESSResponseSuccess(this)
			{
				MaxDischargePower = -InternalVoltage * MaxCurrent,
				AbsTime = absTime,
				LossPower = 0.SI<Watt>(),
				MaxChargePower = InternalVoltage * MaxCurrent,
				PowerDemand = powerdemand,
				SimulationInterval = dt,
			};
		}

		public double StateOfCharge { get; set; }

		public WattSecond StoredEnergy => throw new System.NotImplementedException();

		public Ampere MaxCurrent => 375.SI<Ampere>();

		public Watt MaxChargePower(Second dt)
		{
			throw new System.NotImplementedException();
		}

		public Watt MaxDischargePower(Second dt)
		{
			throw new System.NotImplementedException();
		}

		public double MinSoC => 0;

		public double MaxSoC => 1;
		public AmpereSecond Capacity => null;
		public Volt NominalVoltage => null;

		public IElectricEnergyStoragePort MainBatteryPort => this;

		public IElectricAuxConnecor AuxBatteryPort()
		{
			return this;
		}

		public void Initialize(double initialSoC)
		{
			StateOfCharge = initialSoC;
		}

		public void Connect(IElectricAuxPort aux)
		{
			throw new System.NotImplementedException();
		}

		#region Implementation of IBatteryChargeProvider

		public void Connect(IElectricChargerPort charger)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}