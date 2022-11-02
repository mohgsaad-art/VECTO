using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public class DCDCConverter : StatefulVectoSimulationComponent<DCDCConverter.State>, IDCDCConverter
	{
		public double Efficiency { get; protected set; }

		public DCDCConverter(IVehicleContainer container, double efficiency) : base(container)
		{
			Efficiency = efficiency;
			PreviousState.ConsumedEnergy = 0.SI<WattSecond>();
		}


		#region Implementation of IElectricAuxPort

		public Watt Initialize()
		{
			PreviousState.ConsumedEnergy = 0.SI<WattSecond>();
			CurrentState.ConsumedEnergy = 0.SI<WattSecond>();

			return 0.SI<Watt>();
		}

		public Watt PowerDemand(Second absTime, Second dt, bool dryRun)
		{
			var dischargeEnergy = (-DataBus.BatteryInfo.MaxDischargePower(dt) * dt);
			var chargeEnergy = (-DataBus.BatteryInfo.MaxChargePower(dt) * dt);
			var efficiency = PreviousState.ConsumedEnergy > 0 ? 1 / Efficiency : Efficiency;
			if ((PreviousState.ConsumedEnergy * efficiency).IsBetween(chargeEnergy, dischargeEnergy)) {
				return PreviousState.ConsumedEnergy / dt * efficiency;
			}

			// write in mod-file for post-processing correction
			if (!dryRun) {
				CurrentState.MissingEnergy = PreviousState.ConsumedEnergy;
			}

			return 0.SI<Watt>();
		}

		#endregion

		

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			if (CurrentState.MissingEnergy.IsEqual(0)) {
				container[ModalResultField.P_DCDC_In] =
					PreviousState.ConsumedEnergy / simulationInterval / Efficiency;
				container[ModalResultField.P_DCDC_Out] =
					PreviousState.ConsumedEnergy / simulationInterval;
				container[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
			} else {
				container[ModalResultField.P_DCDC_In] = 0.SI<Watt>();
				container[ModalResultField.P_DCDC_Out] = 0.SI<Watt>();
				container[ModalResultField.P_DCDC_missing] = CurrentState.MissingEnergy / simulationInterval;
			}
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		#endregion

		public void ConsumerEnergy(WattSecond electricConsumerEnergy, bool dryRun)
		{
			if (!dryRun) {
				CurrentState.ConsumedEnergy = electricConsumerEnergy;
			}
		}

		public class State
		{
			public State()
			{
				MissingEnergy = 0.SI<WattSecond>();
			}

			public WattSecond ConsumedEnergy { get; set; }
			public WattSecond MissingEnergy { get; set; }
		}
	}
}