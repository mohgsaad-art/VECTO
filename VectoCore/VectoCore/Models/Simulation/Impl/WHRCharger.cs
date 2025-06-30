using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public interface IWHRCharger : IElectricChargerPort
    {
		void GeneratedEnergy(WattSecond eWHR);
	}

    public class WHRCharger : StatefulVectoSimulationComponent<WHRCharger.State>, IElectricChargerPort, IWHRCharger
    {
		public double Efficiency { get; }

		public WHRCharger(IVehicleContainer container, double efficiency) : base(container)
		{
			Efficiency = efficiency;
			PreviousState.GeneratedEnergy = 0.SI<WattSecond>();
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{ }

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
			if (PreviousState.GeneratedEnergy == null && DataBus.IsTestPowertrain) {
				// the method GeneratedEnergy is not called because there is no moddata to write and we are in a testpowertrain
				// make sure the value is not null...
				PreviousState.GeneratedEnergy = 0.SI<WattSecond>();
			}
		}

		#endregion

		#region Implementation of IElectricChargerPort

		public Watt Initialize()
		{
			PreviousState.GeneratedEnergy = 0.SI<WattSecond>();
			return 0.SI<Watt>();
		}

		public Watt PowerDemand(Second absTime, Second dt, Watt consumerPowerDemand, Watt auxPower, bool dryRun)
		{
			var maxDischargeEnergy = (-DataBus.BatteryInfo.MaxDischargePower(dt) * dt);
			var maxChargeEnergy = (-DataBus.BatteryInfo.MaxChargePower(dt) * dt);

			var chargeEnergy = PreviousState.GeneratedEnergy + PreviousState.ExcessiveEnergy;
			var efficiency = chargeEnergy > 0 ?  Efficiency : 1 / Efficiency;
			if ((chargeEnergy * efficiency).IsBetween(maxChargeEnergy, maxDischargeEnergy)) {
				return chargeEnergy / dt * efficiency;
			}

			if (!dryRun) {
				CurrentState.ExcessiveEnergy = PreviousState.GeneratedEnergy;
			}

			return 0.SI<Watt>();
		}

		public void GeneratedEnergy(WattSecond electricEnergy)
		{
			CurrentState.GeneratedEnergy = electricEnergy;
			
		}

		#endregion

		public class State
		{
			public State()
			{
				ExcessiveEnergy = 0.SI<WattSecond>();
			}

			public WattSecond GeneratedEnergy { get; set; }

			public WattSecond ExcessiveEnergy { get; set; }

			public State Clone() => (State)MemberwiseClone();
		}

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other) {
			if (other is WHRCharger c) {
				PreviousState = c.PreviousState.Clone();
				return true;
			}
			return false;
		}

		#endregion
	}
}