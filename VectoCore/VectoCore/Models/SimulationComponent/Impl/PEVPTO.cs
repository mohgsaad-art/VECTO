using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class PEVPtoTransm : StatefulProviderComponent<PEVPtoTransm.State, ITnOutPort, ITnInPort, ITnOutPort>,
		IPowerTrainComponent, ITnInPort, ITnOutPort
	{
		public class State
		{
			public Dictionary<string, NewtonMeter> PowerDemands { get; set; }
			public PerSecond OutAngularVelocity { get; set; }
		}

		protected readonly Dictionary<string, Func<PerSecond, Second, Second, bool, NewtonMeter>> Auxiliaries =
			new Dictionary<string, Func<PerSecond, Second, Second, bool, NewtonMeter>>();

		public PEVPtoTransm(IVehicleContainer container) : base(container)
		{
			
		}

		public void AddConstant(string auxId, NewtonMeter torqueLoss)
		{
			Add(auxId, (nEng, absTime, dt, dryRun) => torqueLoss);
		}

		public void Add(string auxId, Func<PerSecond, Second, Second, bool, NewtonMeter> torqueLossFunction)
		{
			Auxiliaries[auxId] = torqueLossFunction;
		}

		#region Implementation of ITnOutPort

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			PreviousState.OutAngularVelocity = outAngularVelocity;
			var torqueLoss = outAngularVelocity.IsEqual(0)
				? 0.SI<NewtonMeter>()
				: ComputeTorqueLoss(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, outAngularVelocity,
					false);

			return NextComponent.Initialize(outTorque + torqueLoss, outAngularVelocity);
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			var avgAngularSpeed = PreviousState.OutAngularVelocity != null
				? (outAngularVelocity + PreviousState.OutAngularVelocity) / 2.0
				: outAngularVelocity;

			var torqueLoss = 0.SI<NewtonMeter>();
			var alwaysConsiderPTOTransmLoss = false; //<---- debugging only //TODO: REMOVE
			var gearEngaged = !DataBus.GearboxInfo.GearEngaged(absTime);
			////Always consider PTO_Transm_loss

			if (!DataBus.GearboxInfo.GearEngaged(absTime) && !alwaysConsiderPTOTransmLoss)
            {
                SetZeroPowerDemand(dryRun);
            }
            else
            {
                if (outAngularVelocity != null && !avgAngularSpeed.IsEqual(0))
                {
                    torqueLoss = ComputeTorqueLoss(absTime, dt, avgAngularSpeed, dryRun);
                }
                else
                {
                    SetZeroPowerDemand(dryRun);
                }
            }

            if (!dryRun) {
				CurrentState.OutAngularVelocity = outAngularVelocity;
			}
			
			return NextComponent.Request(absTime, dt, outTorque + torqueLoss, outAngularVelocity, dryRun);
		}


		#endregion

		private void SetZeroPowerDemand(bool dryRun)
		{
			var powerDemands = new Dictionary<string, NewtonMeter>(Auxiliaries.Count);
			foreach (var aux in Auxiliaries)
			{
				powerDemands[aux.Key] = 0.SI<NewtonMeter>();
			}
			if (!dryRun)
			{
				CurrentState.PowerDemands = powerDemands;
			}
		}
		
		private NewtonMeter ComputeTorqueLoss(Second absTime, Second dt, PerSecond avgAngularSpeed, bool dryRun)
		{
			var powerDemands = new Dictionary<string, NewtonMeter>(Auxiliaries.Count);
			foreach (var item in Auxiliaries) {
				var value = item.Value(avgAngularSpeed, absTime, dt, dryRun);
				if (value != null) {
					powerDemands[item.Key] = value;
				}
			}
			if (!dryRun) {
				CurrentState.PowerDemands = powerDemands;
			}

			return powerDemands.Sum(kv => kv.Value) ?? 0.SI<NewtonMeter>();
		}


		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var avgAngularSpeed = PreviousState.OutAngularVelocity != null
				? (CurrentState.OutAngularVelocity + PreviousState.OutAngularVelocity) / 2.0
				: CurrentState.OutAngularVelocity;
			foreach (var kv in CurrentState.PowerDemands) {
				container[kv.Key] = kv.Value * avgAngularSpeed;
			}
		}

		protected override bool DoUpdateFrom(object other) => false;

		#endregion
	}
}