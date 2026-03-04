using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell
{
	public class TracingInfinityBatterySystem : StatefulVectoSimulationComponent<TracingInfinityBatterySystem.TracingInfinityBatteryState>, IElectricEnergyStoragePort, IElectricEnergyStorage, IRESSInfo
	{

		public class TracingInfinityBatteryState
		{
			public double StateOfCharge;

			public Volt InternalVoltage;
			public Ampere Current;


		}

		private BatterySystem _infinityBat;

		public IList<double> SoCTrace = new List<double>();


		public TracingInfinityBatterySystem(BatterySystemData batData) : base(null)
		{
			var tmp_batData = batData.Clone();
			tmp_batData.ChargeSustainingBatterySystem = true;
			_infinityBat = new BatterySystem(null, tmp_batData); ;
		}


		/// <summary>
		/// Smallest SoC since last call of <see cref="Initialize"/>
		/// </summary>
		public double MinSocTrace => SoCTrace.Min();
		/// <summary>
		/// Largest SoC since last call of <see cref="Initialize"/>
		/// </summary>
		public double MaxSocTrace => SoCTrace.Max();


		#region Implementation of IElectricEnergyStoragePort

		public static double CalculateDeltaSOC(BatterySystem batterySystem, IRESSResponse response, out Ampere current)
		{
			var totalCapacity = batterySystem.Capacity;

			current = (response.PowerDemand - response.LossPower) / response.InternalVoltage;
			var energy = current * response.SimulationInterval;

			var energy_in_bat = totalCapacity * batterySystem.StateOfCharge;

			var remaining_energy = energy_in_bat + energy;

			var remaining_soc = remaining_energy / batterySystem.Capacity;
			var deltaSoc = remaining_soc - batterySystem.StateOfCharge;


			return deltaSoc;
		}


		public void Initialize(double initialSoC)
		{
			_infinityBat.Initialize(initialSoC);
			PreviousState.StateOfCharge = initialSoC;
			SoCTrace.Clear();
			SoCTrace.Add(initialSoC);
		}

		public IRESSResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun)
		{
			var response = _infinityBat.Request(absTime, dt, powerDemand, dryRun);

			if (response is RESSResponseSuccess responseSuccess)
			{


				var dSoc = CalculateDeltaSOC(batterySystem: _infinityBat, response: responseSuccess, out var current);


				CurrentState.Current = current;
				CurrentState.InternalVoltage = responseSuccess.InternalVoltage;
				CurrentState.StateOfCharge = PreviousState.StateOfCharge + dSoc;
				SoCTrace.Add(CurrentState.StateOfCharge);
			}


			return response;
		}

		#endregion

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_reess_int] = CurrentState.InternalVoltage * CurrentState.Current;

			container[ModalResultField.REESSStateOfCharge] = CurrentState.StateOfCharge.SI<Scalar>();
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();

		}

		protected override bool DoUpdateFrom(object other)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IRESSInfo

		public Volt InternalVoltage => _infinityBat.InternalVoltage;

		public double StateOfCharge => PreviousState.StateOfCharge;

		public WattSecond StoredEnergy => _infinityBat.StoredEnergy;

		public Watt MaxChargePower(Second dt)
		{
			return _infinityBat.MaxChargePower(dt);
		}

		public Watt MaxDischargePower(Second dt)
		{
			return _infinityBat.MaxDischargePower(dt);
		}

		public double MinSoC => _infinityBat.MinSoC;

		public double MaxSoC => _infinityBat.MaxSoC;

		public AmpereSecond Capacity => _infinityBat.Capacity;

		public Volt NominalVoltage => _infinityBat.NominalVoltage;

		#endregion

		#region Implementation of IBatteryProvider

		public IElectricEnergyStoragePort MainBatteryPort => this;

		#endregion
	}
}