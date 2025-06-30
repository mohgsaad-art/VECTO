using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{

    public class NoBattery : VectoSimulationComponent, ISimpleBattery
	{
		public NoBattery(IVehicleContainer container) : base(container) { }

		#region Implementation of ISimpleBatteryInfo

		public double SOC => 0;

		public WattSecond Capacity => 0.SI<WattSecond>();

		#endregion

		#region Implementation of ISimpleBattery

		public WattSecond ConsumedEnergy { get; }
		public void ConsumeEnergy(WattSecond energy, bool dryRun)
		{
			
		}

		public WattSecond MaxChargeEnergy()
		{
			return 0.SI<WattSecond>();
		}

		public WattSecond MaxDischargeEnergy()
		{
			return 0.SI<WattSecond>();
		}

		#endregion

		

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other) => false;

		#endregion
	}

	// ########################################

	public class SimpleBattery : StatefulVectoSimulationComponent<SimpleBattery.State>, ISimpleBattery
	{
		public SimpleBattery(IVehicleContainer container, WattSecond capacity, double storageEfficiency, double soc = 0.9) : base(container)
		{
			Capacity = capacity;
			SOC = soc;
			StorageEfficiency = storageEfficiency;
		}

		public double StorageEfficiency { get; set; }

		#region Implementation of ISimpleBattery

		public double SOC { get; internal set; }
		public WattSecond Capacity { get; }
		public WattSecond ConsumedEnergy => CurrentState.ConsumedEnergy;

		public void ConsumeEnergy(WattSecond energy, bool dryRun)
		{
			var batEnergy = energy * (energy.IsGreater(0) ? StorageEfficiency : 1);

			if (!dryRun) {
				CurrentState.ConsumedEnergy = batEnergy;
			}
			var tmpSoc = SOC + batEnergy / Capacity;
			if (tmpSoc > 1) {
				Log.Warn("SOC would exceed max!");
				
			}
			if (tmpSoc < 0) {
				Log.Warn("SOC would  exceed min!");
			}
		}

		#endregion


		public WattSecond MaxChargeEnergy()
		{
			return -(SOC - 1) * Capacity / StorageEfficiency;
		}

		public WattSecond MaxDischargeEnergy()
		{
			return -SOC * Capacity;
		}

		public class State
		{
			public State()
			{
				ConsumedEnergy = 0.SI<WattSecond>();
			}
			public WattSecond ConsumedEnergy { get; set; }

			public State Clone() => (State)MemberwiseClone();
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			SOC += CurrentState.ConsumedEnergy / Capacity;
			if (SOC > 1) {
				Log.Warn("SOC > 1!");
				SOC = 1;
			}
			if (SOC < 0) {
				Log.Warn("SOC < 0!");
				SOC = 0;
			}
			AdvanceState();
		}
		#endregion

		#region Implementation of IUpdateable
		protected override bool DoUpdateFrom(object other) {
			if (other is SimpleBattery b) {
				PreviousState = b.PreviousState.Clone();
				SOC = b.SOC;
				return true;
			}
			return false;
		}
		#endregion
	}
}
