using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class IdealBattery : Battery
    {
        public const double IDEAL_STATE_OF_CHARGE = 0.5;

        public IdealBattery(IVehicleContainer container, BatteryData modelData, int idx = -1) : 
			base(container, modelData, idx)
        {
        }

        public override void Initialize(double initialSoC)
        { 
            base.Initialize(initialSoC);

            PreviousState.CalculatedStateOfCharge = initialSoC;
            PreviousState.StateOfCharge = IDEAL_STATE_OF_CHARGE;
        }

        public override IRESSResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun = false)
		{
			var tPulse = PreviousState.PowerDemand.Sign() == powerDemand.Sign()
				? PreviousState.PulseDuration
				: 0.SI<Second>();

			var maxChargePower = MaxChargePower(dt);
			var maxDischargePower = MaxDischargePower(dt);

			if (powerDemand.IsGreater(maxChargePower, Constants.SimulationSettings.InterpolateSearchTolerance) ||
				powerDemand.IsSmaller(maxDischargePower, Constants.SimulationSettings.InterpolateSearchTolerance))
			{
				return PowerDemandExceeded(absTime, dt, powerDemand, maxDischargePower, maxChargePower, tPulse, dryRun);
			}

			var batteryState = ComputeInternalResistanceAndCurrent(PreviousState.StateOfCharge, powerDemand, tPulse);
			var batteryLoss = batteryState.current * batteryState.internalResistance * batteryState.current;
			
			if (dryRun) {
				return new RESSDryRunResponse(this) {
					AbsTime = absTime,
					SimulationInterval = dt,
					MaxChargePower = maxChargePower,
					MaxDischargePower = maxDischargePower,
					PowerDemand = powerDemand,
					LossPower = batteryLoss,
					StateOfCharge = IDEAL_STATE_OF_CHARGE
				};
			}

			var calcBatteryState = ComputeInternalResistanceAndCurrent(PreviousState.CalculatedStateOfCharge, powerDemand, tPulse);
			var calcStateOfCharge = CalculateStateOfCharge(ModelData.Capacity * PreviousState.CalculatedStateOfCharge, 
				calcBatteryState.current, dt);

			CurrentState.SimulationInterval = dt;
			CurrentState.PowerDemand = powerDemand;
			CurrentState.TotalCurrent = batteryState.current;
			CurrentState.BatteryLoss = batteryLoss;
			CurrentState.StateOfCharge = IDEAL_STATE_OF_CHARGE;
			CurrentState.CalculatedStateOfCharge = calcStateOfCharge;
			CurrentState.MaxChargePower = maxChargePower;
			CurrentState.MaxDischargePower = maxDischargePower;

			return new RESSResponseSuccess(this) {
				AbsTime = absTime,
				SimulationInterval = dt,
				MaxChargePower = maxChargePower,
				MaxDischargePower = maxDischargePower,
				PowerDemand = powerDemand,
				LossPower = batteryLoss,
				StateOfCharge = IDEAL_STATE_OF_CHARGE
			};
        }

		protected (Ohm internalResistance, Ampere current) ComputeInternalResistanceAndCurrent(double stateOfCharge, Watt powerDemand, 
			Second tPulse)
		{
			var internalResistance = ModelData.InternalResistance.Lookup(stateOfCharge, tPulse);
			
			var current = powerDemand.IsEqual(0) 
				? 0.SI<Ampere>() 
				: SelectSolution(
					VectoMath.QuadraticEquationSolver(internalResistance.Value(), InternalVoltage.Value(),-powerDemand.Value()),
					powerDemand.Value());

			return (internalResistance: internalResistance, current: current);
        }

		protected double CalculateStateOfCharge(AmpereSecond currentCharge, Ampere current, Second dt)
		{
			return (currentCharge + current * dt) / ModelData.Capacity;
        }

        protected override void DoWriteModalResults(Second absTime, Second dt, IModalDataContainer container)
        {
            base.DoWriteModalResults(absTime, dt, container);
            container[ModalResultField.REESSStateOfCharge, BatteryId] = CurrentState.CalculatedStateOfCharge.SI();
        }
    }
}
