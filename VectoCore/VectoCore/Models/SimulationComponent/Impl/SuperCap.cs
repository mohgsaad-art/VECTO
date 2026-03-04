using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class SuperCap : StatefulVectoSimulationComponent<SuperCap.State>, IElectricEnergyStorage, IElectricEnergyStoragePort, IUpdateable
	{
		private SuperCapData ModelData;

		public SuperCap(IVehicleContainer container, SuperCapData modelData) : base(container)
		{
			ModelData = modelData;
		}

		public IElectricEnergyStoragePort MainBatteryPort => this;

		public Volt InternalVoltage => PreviousState.Charge / ModelData.Capacity;

		public double StateOfCharge => PreviousState.Charge / (ModelData.Capacity * ModelData.MaxVoltage);

		public WattSecond StoredEnergy =>
			// E = 1/2 C * U^2 = 1/2 Q^2/C
			PreviousState.Charge * InternalVoltage / 2.0;

		public Watt MaxChargePower(Second dt)
		{
			var maxChargeCurrent =
				VectoMath.Min((ModelData.Capacity * ModelData.MaxVoltage - PreviousState.Charge) / dt,
					ModelData.MaxCurrentCharge);
			return InternalVoltage * maxChargeCurrent + maxChargeCurrent * ModelData.InternalResistance * maxChargeCurrent;
		}

		public Watt MaxDischargePower(Second dt)
		{
			var maxPower = -InternalVoltage / (4 * ModelData.InternalResistance) * InternalVoltage;
			var maxPowerCurrent = maxPower / InternalVoltage;
			var maxDischargeCurrent =
				VectoMath.Max((ModelData.Capacity * ModelData.MinVoltage - PreviousState.Charge) / dt,
					ModelData.MaxCurrentDischarge);
			maxDischargeCurrent = VectoMath.Max(maxDischargeCurrent, maxPowerCurrent);
			var maxDischargePower = InternalVoltage * maxDischargeCurrent +
									maxDischargeCurrent * ModelData.InternalResistance * maxDischargeCurrent;
			return VectoMath.Max(maxDischargePower, maxPower);
		}

		public double MinSoC => ModelData.MinVoltage / ModelData.MaxVoltage;

		public double MaxSoC => 1;

		/// <summary>
		/// [Warning("Not implemented in super cap, returns null")]
		/// </summary>
		public AmpereSecond Capacity => null;
		/// <summary>
		/// [Warning("Not implemented in super cap, returns null")]
		/// </summary>
		public Volt NominalVoltage => null;

		public void Initialize(double initialSoC)
		{
			PreviousState.Charge = ModelData.Capacity * (ModelData.MaxVoltage *  initialSoC );

		}

		public IRESSResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun)
		{
			var maxChargePower = MaxChargePower(dt);
			var maxDischargePower = MaxDischargePower(dt);

			if (powerDemand.IsGreater(maxChargePower, Constants.SimulationSettings.LineSearchTolerance) ||
				powerDemand.IsSmaller(maxDischargePower, Constants.SimulationSettings.LineSearchTolerance))
			{
				return PowerDemandExceeded(absTime, dt, powerDemand, maxDischargePower, maxChargePower, dryRun);
			}

			var internalResistance = ModelData.InternalResistance;
			var current = 0.SI<Ampere>();
			if (!powerDemand.IsEqual(0))
			{
				var solutions = VectoMath.QuadraticEquationSolver(internalResistance.Value(), InternalVoltage.Value(),
					-powerDemand.Value());
				current = SelectSolution(solutions, powerDemand.Value());
			}
			var internalLoss = current * internalResistance * current;
			var currentCharge = PreviousState.Charge;

			if (dryRun)
			{
				return new RESSDryRunResponse(this)
				{
					AbsTime = absTime,
					SimulationInterval = dt,
					MaxChargePower = maxChargePower,
					MaxDischargePower = maxDischargePower,
					PowerDemand = powerDemand,
					LossPower = internalLoss,
					StateOfCharge = (currentCharge + current * dt) / (ModelData.Capacity * ModelData.MaxVoltage),
					InternalVoltage = InternalVoltage
				};
			}
			CurrentState.SimulationInterval = dt;
			CurrentState.PowerDemand = powerDemand;
			CurrentState.TotalCurrent = current;
			CurrentState.InternalLoss = internalLoss;


			CurrentState.Charge = (currentCharge + current * dt);
			CurrentState.MaxChargePower = maxChargePower;
			CurrentState.MaxDischargePower = maxDischargePower;
			return new RESSResponseSuccess(this)
			{
				AbsTime = absTime,
				SimulationInterval = dt,
				MaxChargePower = maxChargePower,
				MaxDischargePower = maxDischargePower,
				PowerDemand = powerDemand,
				LossPower = internalLoss,
				StateOfCharge = (currentCharge + current * dt) / (ModelData.Capacity * ModelData.MaxVoltage),
				InternalVoltage = InternalVoltage
			};
		}

		private Ampere SelectSolution(double[] solutions, double sign)
		{
			return solutions.FirstOrDefault().SI<Ampere>();

			//return solutions.Where(x => Math.Sign(sign) == Math.Sign(x) /*&& Math.Abs(x).IsSmallerOrEqual(ModelData.MaxCurrent.Value(), 1e-3)*/).MinBy(x => Math.Abs(x)).SI<Ampere>();
		}


		private IRESSResponse PowerDemandExceeded(Second absTime, Second dt, Watt powerDemand, Watt maxDischargePower,
			Watt maxChargePower, bool dryRun)
		{
			var maxPower = powerDemand < 0 ? maxDischargePower : maxChargePower;

			var maxChargeCurrent =
				VectoMath.Min((ModelData.Capacity * ModelData.MaxVoltage - PreviousState.Charge) / dt,
					ModelData.MaxCurrentCharge);
			var maxDischargeCurrent =
				VectoMath.Max((ModelData.Capacity * ModelData.MinVoltage - PreviousState.Charge) / dt,
					ModelData.MaxCurrentDischarge);
			var current = powerDemand < 0 ? maxDischargeCurrent : maxChargeCurrent;

			var batteryLoss = current * ModelData.InternalResistance * current;

			AbstractRESSResponse response;
			if (dryRun) {
				response = new RESSDryRunResponse(this);
			} else {
				if (powerDemand > maxPower) {
					response = new RESSOverloadResponse(this);
				} else {
					response = new RESSUnderloadResponse(this);
				}
			}
			response.AbsTime = absTime;
			response.SimulationInterval = dt;
			response.MaxChargePower = maxChargePower;
			response.MaxDischargePower = maxDischargePower;
			response.PowerDemand = powerDemand;
			response.LossPower = batteryLoss;
			response.InternalVoltage = InternalVoltage;

			return response;
		}


		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var internalVoltage = PreviousState.Charge / ModelData.Capacity;
			container[ModalResultField.U0_reess] = internalVoltage;
			container[ModalResultField.U_reess_terminal] =
				internalVoltage +
				CurrentState.TotalCurrent * ModelData.InternalResistance; // adding both terms because pos. current charges the battery!
			container[ModalResultField.I_reess] = CurrentState.TotalCurrent;
			container[ModalResultField.REESSStateOfCharge] = CurrentState.Charge / (ModelData.Capacity * ModelData.MaxVoltage);
			container[ModalResultField.P_reess_terminal] = CurrentState.PowerDemand;
			container[ModalResultField.P_reess_int] = internalVoltage * CurrentState.TotalCurrent;
			container[ModalResultField.P_reess_loss] = CurrentState.InternalLoss;
			container[ModalResultField.P_reess_charge_max] = CurrentState.MaxChargePower;
			container[ModalResultField.P_reess_discharge_max] = CurrentState.MaxDischargePower;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();
		}

		public class State
		{
			public AmpereSecond Charge;

			public Second SimulationInterval;

			public Watt PowerDemand;

			public Ampere TotalCurrent;
			public Watt MaxChargePower;
			public Watt MaxDischargePower;
			public Watt InternalLoss;

			public State Clone() => (State)MemberwiseClone();
		}

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other) {
			if (other is SuperCap o) {
				PreviousState = o.PreviousState.Clone();
				return true;
			}

			return false;
		}

		#endregion
	}
}