using System;
using System.Collections.Generic;
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
    public class BatterySystem : StatefulVectoSimulationComponent<BatterySystem.State>, IElectricEnergyStorage, IElectricEnergyStoragePort, IUpdateable
	{
		public class BatteryString: IUpdateable
		{
			protected readonly List<Battery> _batteries;
			
			private AmpereSecond _capacity;
			private AmpereSecond _capacityMinSoc;
			private AmpereSecond _capacityMaxSoc;

			public BatteryString()
            {
				_batteries = new List<Battery>();
            }

			//public IReadOnlyList<Battery> Batteries { get; }
			public IReadOnlyList<Battery> Batteries => _batteries;

			public void AddBattery(Battery bat)
			{
				_batteries.Add(bat);
				// Todo: update some properties?
				_capacity = null;
			}

			public Volt OpenCircuitVoltage => _batteries.Sum(x => x.InternalVoltage);

			public Ohm InternalResistance(Second tPulse) => _batteries.Sum(x => x.InternalResistance(tPulse));

			public AmpereSecond Capacity => _capacity ?? (_capacity = _batteries.Min(x => x.Capacity));

			public double SoC => _batteries.Min(x => x.StateOfCharge * x.Capacity) / Capacity;
			public WattSecond StoredEnergy => _batteries.Min(x => x.StateOfCharge * x.Capacity) * OpenCircuitVoltage;

			public Watt MaxDischargePower(Second dt, Second tPulse)
			{
				var maxDischargeCurrent = MaxDischargeCurrent(dt);
				var internalResistance = InternalResistance(tPulse);
				var maxDischargePower = OpenCircuitVoltage * maxDischargeCurrent +
										maxDischargeCurrent * internalResistance* maxDischargeCurrent;
				var maxPower = -OpenCircuitVoltage / (4 * internalResistance) * OpenCircuitVoltage;
				return VectoMath.Max(maxDischargePower, maxPower);

			}

			public Ampere MaxChargeCurrent(Second dt)
			{
				return _batteries.Min(x => x.MaxChargeCurrent(dt));
			}

			public Ampere MaxDischargeCurrent(Second dt)
			{
				return _batteries.Max(x => x.MaxDischargeCurrent(dt));
			}

			public Watt MaxChargePower(Second dt, Second tPulse)
			{
				var maxCurrent = MaxChargeCurrent(dt);
				return OpenCircuitVoltage * maxCurrent +
						maxCurrent * InternalResistance(tPulse) * maxCurrent;
			}

			public IList<IRESSResponse> Request(Second absTime, Second dt, Watt powerDemand, Second tPulse, bool dryRun)
			{
				var current = 0.SI<Ampere>();
				if (!powerDemand.IsEqual(0, 1e-3)) {
					var R_int = InternalResistance(tPulse);
					double[] solutions;
					if (R_int.IsRelativeEqual(0.SI<Ohm>())) {
						//Linear solution, quadratic equation solver would become unstable if a is very close to zero
						solutions = new[] {
							(powerDemand / OpenCircuitVoltage).Value()
						};
					} else {
						solutions = VectoMath.QuadraticEquationSolver(InternalResistance(tPulse).Value(), OpenCircuitVoltage.Value(),
							-powerDemand.Value());
                    }
			
					current = SelectSolution(solutions, powerDemand.Value(), dt);
				}

				if (!dryRun) {
					Current = current;
				}

				return _batteries.Select(x => {
					var demand = (x.InternalVoltage + x.InternalResistance(tPulse) * current) * current;
					return x.Request(absTime, dt, demand, dryRun);
				}).ToList();
			}

			public Ampere Current { get; set; }

			public AmpereSecond CapacityMinSoc =>
				_capacityMinSoc ?? (_capacityMinSoc = _batteries.Min(x => x.Capacity * x.MinSoC));

			public AmpereSecond CapacityMaxSoc =>
				_capacityMaxSoc ?? (_capacityMaxSoc = _batteries.Min(x => x.Capacity * x.MaxSoC));

			public Volt NominalVoltage => _batteries.Sum(x => x.NominalVoltage);

			private Ampere SelectSolution(double[] solutions, double sign, Second dt)
			{
				var maxCurrent = Math.Sign(sign) < 0
					? MaxDischargeCurrent(dt)
					: MaxChargeCurrent(dt);
				return solutions.Where(x => Math.Sign(sign) == Math.Sign(x) && Math.Abs(x).IsSmallerOrEqual(Math.Abs(maxCurrent.Value()), 1e-3)).Min().SI<Ampere>();
			}

			#region Implementation of IUpdateable

			public bool UpdateFrom(object other) {
				if (other is BatteryString bs) {
					return _batteries.ZipAll(bs._batteries).All(ts => ts.Item1.UpdateFrom(ts.Item2));
				}
				return false;
			}

			#endregion
		}

		protected internal readonly Dictionary<int, BatteryString> Batteries = new Dictionary<int, BatteryString>();
		
		private AmpereSecond _totalCapacity;
		private Scalar _minSoc;
		private Scalar _maxSoc;

		public BatterySystem(IVehicleContainer container, BatterySystemData batterySystemData) : base(container)
		{
			foreach (var entry in batterySystemData.Batteries) {
				var bat = new Battery(null, entry.Item2);
				if (!Batteries.ContainsKey(entry.Item1)) {
					Batteries[entry.Item1] = new BatteryString();
				}
				Batteries[entry.Item1].AddBattery(bat);
			}

			PreviousState.PowerDemand = 0.SI<Watt>();
		}

		#region Overrides of VectoSimulationComponent

		public override void CommitSimulationStep(Second time, Second simulationInterval, IModalDataContainer container)
		{
			base.CommitSimulationStep(time, simulationInterval, container);
			foreach (var battery in Batteries) {
				foreach (var b in battery.Value.Batteries) {
					b.CommitSimulationStep(time, simulationInterval, container);
				}
			}
			// write battery system average SoC after all battery components - then the SoC property changes from current to
			// previous state and thus the average matches the individual batteries
			if (container != null) {
				container[ModalResultField.REESSStateOfCharge] = StateOfCharge.SI();
			}
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var cellVoltage = InternalVoltage;
			var tPulse = PreviousState.PowerDemand.Sign() == CurrentState.PowerDemand.Sign()
				? PreviousState.PulseDuration
				: 0.SI<Second>();
			container[ModalResultField.U0_reess] = cellVoltage;
			container[ModalResultField.U_reess_terminal] =
				cellVoltage +
				CurrentState.TotalCurrent *
				InternalResistance(tPulse); // adding both terms because pos. current charges the battery!
			container[ModalResultField.I_reess] = CurrentState.TotalCurrent;
			//container[ModalResultField.REESSStateOfCharge] = CurrentState.StateOfCharge.SI();
			container[ModalResultField.P_reess_terminal] = CurrentState.PowerDemand;
			container[ModalResultField.P_reess_int] = cellVoltage * CurrentState.TotalCurrent;
			container[ModalResultField.P_reess_loss] = CurrentState.BatteryLoss;
			container[ModalResultField.P_reess_charge_max] = CurrentState.MaxChargePower;
			container[ModalResultField.P_reess_discharge_max] = CurrentState.MaxDischargePower;
		}

		public Ohm InternalResistance(Second tPulse) => (1 / Batteries.Sum(bs => 1 / bs.Value.InternalResistance(tPulse).Value())).SI<Ohm>();
		
		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			var tPulse = PreviousState.PowerDemand.Sign() == CurrentState.PowerDemand.Sign()
				? PreviousState.PulseDuration
				: 0.SI<Second>();
			CurrentState.PulseDuration = tPulse + simulationInterval;
			AdvanceState();
		}


		#endregion

		#region Implementation of IBatteryProvider

		public IElectricEnergyStoragePort MainBatteryPort => this;

		#endregion

		#region Implementation of IRESSInfo

		public Volt InternalVoltage => Batteries.Values.Select(x => x.OpenCircuitVoltage).Average();

		public double StateOfCharge => Batteries.Values.Sum(bs => bs.SoC * bs.Capacity) / TotalCapacity;

		public AmpereSecond TotalCapacity =>
			_totalCapacity ?? (_totalCapacity = Batteries.Values.Sum(bs => bs.Capacity));

		public WattSecond StoredEnergy => Batteries.Values.Sum(bs => bs.StoredEnergy);
		
		public Watt MaxChargePower(Second dt)
		{
			var tPulse = PreviousState.PowerDemand.Sign() > 0 // keep on charging?
				? PreviousState.PulseDuration
				: 0.SI<Second>();
			return Batteries.Values.Sum(bs => bs.MaxChargePower(dt, tPulse));

		}

		public Watt MaxDischargePower(Second dt)
		{
			var tPulse = PreviousState.PowerDemand.Sign() < 0 // keep on discharging?
				? PreviousState.PulseDuration
				: 0.SI<Second>();
			return Batteries.Values.Sum(bs => bs.MaxDischargePower(dt, tPulse));
		}

		public double MinSoC => _minSoc ?? (_minSoc = Batteries.Values.Sum(x => x.CapacityMinSoc) / TotalCapacity);
		public double MaxSoC => _maxSoc ?? (_maxSoc = Batteries.Values.Sum(x => x.CapacityMaxSoc) / TotalCapacity);
		public AmpereSecond Capacity => TotalCapacity;

		public Volt NominalVoltage => Batteries.Values.Select(x => x.NominalVoltage).Average();

		#endregion

		#region Implementation of IElectricEnergyStoragePort

		public void Initialize(double initialSoC)
		{
			CurrentState.PulseDuration = 0.SI<Second>();
			PreviousState.PulseDuration = 0.SI<Second>();
			PreviousState.PowerDemand = 0.SI<Watt>();
			foreach (var b in Batteries.Values.SelectMany(bs => bs.Batteries)) {
				b.Initialize(initialSoC);
			}
		}

		public IRESSResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun)
		{
			var maxChargePower = MaxChargePower(dt);
			var maxDischargePower = MaxDischargePower(dt);

			var tPulse = PreviousState.PowerDemand.Sign() == powerDemand.Sign()
				? PreviousState.PulseDuration
				: 0.SI<Second>();

			if (powerDemand.IsGreater(maxChargePower, Constants.SimulationSettings.InterpolateSearchTolerance) ||
				powerDemand.IsSmaller(maxDischargePower, Constants.SimulationSettings.InterpolateSearchTolerance)) {
				return PowerDemandExceeded(absTime, dt, powerDemand, maxDischargePower, maxChargePower, tPulse, dryRun);
			}

			var tPulseChg = PreviousState.PowerDemand.Sign() > 0 // keep on charging?
				? PreviousState.PulseDuration
				: 0.SI<Second>();
			var tPulseDischg = PreviousState.PowerDemand.Sign() < 0 // keep on discharging?
				? PreviousState.PulseDuration
				: 0.SI<Second>();

			var powerDemands = new Dictionary<int, Watt>();
			var limitBB = new Dictionary<int, Watt>();
			var distributedPower = 0.SI<Watt>();
			do {
				distributedPower = 0.SI<Watt>();
				var remainingPower = powerDemand - (limitBB.Sum(x => x.Value) ?? 0.SI<Watt>());
				powerDemands.Clear();
				var totalCapacity = (Batteries.Where(x => !limitBB.ContainsKey(x.Key)).Sum(x => x.Value.Capacity) ?? 0.SI<AmpereSecond>());
				var averageSoC = (Batteries.Where(x => !limitBB.ContainsKey(x.Key)).Sum(x => x.Value.SoC * x.Value.Capacity) / totalCapacity).Value();
				foreach (var bs in Batteries) {
					if (limitBB.ContainsKey(bs.Key)) {
						continue;
					}
					var delta = 1 - Math.Sign(remainingPower.Value()) * (bs.Value.SoC - averageSoC) / averageSoC;
					var power = bs.Value.Capacity / totalCapacity * delta * remainingPower;
					
					if (!power.IsBetween(bs.Value.MaxDischargePower(dt, tPulseDischg), bs.Value.MaxChargePower(dt, tPulseChg))) {
						limitBB[bs.Key] = power.LimitTo(bs.Value.MaxDischargePower(dt, tPulseDischg), bs.Value.MaxChargePower(dt, tPulseChg));
					} else {
						powerDemands[bs.Key] = power;
						distributedPower += power;
					}
				}

				if (limitBB.Count > 0) {
					Log.Debug($"BB ${limitBB.Keys.Join()} are at max - recalculating power distribution");
				}
			} while (!(distributedPower + (limitBB.Sum(x => x.Value) ?? 0.SI<Watt>())) .IsEqual(powerDemand, 1e-3.SI<Watt>()));

			powerDemands = powerDemands.Concat(limitBB).ToDictionary(x => x.Key, x=> x.Value);

			var responses = new Dictionary<int, IList<IRESSResponse>>();
			foreach (var entry in powerDemands) {
				responses[entry.Key] = Batteries[entry.Key].Request(absTime, dt, entry.Value, tPulse, dryRun);
			}

			if (dryRun) {
				return new RESSDryRunResponse(this) {
					AbsTime = absTime,
					SimulationInterval = dt,
					MaxChargePower = maxChargePower,
					MaxDischargePower = maxDischargePower,
					PowerDemand = powerDemand,
					LossPower = responses.Sum(x => x.Value.Sum(b => b.LossPower)),
					//StateOfCharge = (currentCharge + current * dt) / ModelData.Capacity
					InternalVoltage = InternalVoltage,
					StateOfCharge = StateOfCharge,
				};
			}

			var current = Batteries.Values.Sum(x => x.Current);
			
			CurrentState.StateOfCharge = StateOfCharge;
			CurrentState.PowerDemand = powerDemand;
			CurrentState.MaxChargePower = maxChargePower;
			CurrentState.MaxDischargePower = maxDischargePower;
			CurrentState.TotalCurrent = current;
			CurrentState.BatteryLoss = current * InternalResistance(tPulse) * current;

			if (responses.All(bb => bb.Value.All(b => b is RESSResponseSuccess))) {
				return new RESSResponseSuccess(this) {
					AbsTime = absTime,
					SimulationInterval = dt,
					MaxChargePower = maxChargePower,
					MaxDischargePower = maxDischargePower,
					PowerDemand = powerDemand,
					LossPower = responses.Sum(x => x.Value.Sum(b => b.LossPower)),
                    //StateOfCharge = (currentCharge + current * dt) / ModelData.Capacity
					InternalVoltage = InternalVoltage,
					StateOfCharge = StateOfCharge,
                };
			}
			throw new NotImplementedException("batterypack no success");
		}


		private IRESSResponse PowerDemandExceeded(Second absTime, Second dt, Watt powerDemand, Watt maxDischargePower,
			Watt maxChargePower, Second tPulse, bool dryRun)
		{
			var maxPower = powerDemand < 0 ? maxDischargePower : maxChargePower;

			var maxChargeCurrent = Batteries.Values.Sum(bs => bs.MaxChargeCurrent(dt));
			var maxDischargeCurrent = Batteries.Values.Sum(bs => bs.MaxDischargeCurrent(dt));
			var current = powerDemand < 0 ? maxDischargeCurrent : maxChargeCurrent;
			var internalResistance = (1 / Batteries.Values.Sum(bs => 1 / bs.InternalResistance(tPulse).Value())).SI<Ohm>();

			var batteryLoss = current * internalResistance * current;

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

		#endregion

		public class State
		{
			public double StateOfCharge;

			public Second SimulationInterval;

			public Watt PowerDemand;

			public Ampere TotalCurrent;
			public Watt MaxChargePower;
			public Watt MaxDischargePower;
			public Watt BatteryLoss;
			public Second PulseDuration;
			public State Clone() => (State)MemberwiseClone();
		}

		#region Implementation of IUpdateable

		protected override bool DoUpdateFrom(object other) {
			if (other is BatterySystem b) {
				PreviousState = b.PreviousState.Clone();
				return Batteries.All(kv => kv.Value.UpdateFrom(b.Batteries[kv.Key]));
			}

			return false;
		}

		#endregion

	}
}