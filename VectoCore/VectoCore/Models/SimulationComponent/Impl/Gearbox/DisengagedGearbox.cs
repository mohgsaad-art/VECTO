using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class DisengagedGearbox : AbstractGearbox<GearboxState>, IGearbox, IHybridControlledGearbox
	{

		public DisengagedGearbox(IVehicleContainer container, int axleNumber) : base(container, axleNumber)
		{
			Gear = ModelData.GearList.Last();
			LastDownshift = -double.MaxValue.SI<Second>();
			LastUpshift = -double.MaxValue.SI<Second>();
		}

		#region Overrides of Gearbox

		public override Second LastUpshift { get; protected internal set; }

		public override Second LastDownshift { get; protected internal set; }

		public override GearshiftPosition NextGear => Gear;

		public override bool Disengaged { get; set; }

		public override bool DisengageGearbox { get; set; }

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio;
			var gearboxTorqueLoss = GetGearLoss(outAngularVelocity, outTorque);
			CurrentState.TorqueLossResult = gearboxTorqueLoss;

			var inTorque = outTorque / ModelData.Gears[Gear.Gear].Ratio
							+ gearboxTorqueLoss.Value;

			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			PreviousState.Gear = Gear;
			
			var response = NextComponent.Initialize(inTorque, inAngularVelocity);

			return response;
		}

		public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun = false)
		{
			var gear = Gear.Gear;
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var inTorqueLossResult = GetGearLoss(avgOutAngularVelocity, outTorque);
			if (avgOutAngularVelocity.IsEqual(0, 1e-9)) {
				inTorqueLossResult.Value = 0.SI<NewtonMeter>();
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
			var avgInAngularVelocity = (PreviousState.InAngularVelocity + inAngularVelocity) / 2.0;
			var inTorque = !avgInAngularVelocity.IsEqual(0)
				? outTorque * (avgOutAngularVelocity / avgInAngularVelocity)
				: outTorque / ModelData.Gears[Gear.Gear].Ratio;
			inTorque += inTorqueLossResult.Value;
			//var inTorque = outTorque / ModelData.Gears[gear].Ratio + inTorqueLossResult.Value;

			var inertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
				? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
				avgOutAngularVelocity
				: 0.SI<NewtonMeter>();
			inTorque += inertiaTorqueLossOut / ModelData.Gears[gear].Ratio;

			if (dryRun) {
				// gearbox is disengaged the 0[W]-line is the limit for drag and full load.
				var delta = inTorque * avgInAngularVelocity;
				var remainingPowerTrain = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
				return new ResponseDryRun(this, remainingPowerTrain) {
					Gearbox = {
						PowerRequest =  delta,
						Gear = new GearshiftPosition(0),
						InputSpeed = inAngularVelocity,
						InputTorque = inTorque,
						OutputTorque = outTorque,
						OutputSpeed = outAngularVelocity,
					},
					DeltaDragLoad = delta,
					DeltaFullLoad = delta,
					DeltaDragLoadTorque = inTorque,
					DeltaFullLoadTorque = inTorque,
				};
			}

			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.Gear = Gear;
			CurrentState.TransmissionTorqueLoss = inTorque * ModelData.Gears[gear].Ratio - outTorque;

			var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, false);

			response.Gearbox.PowerRequest = outTorque * avgOutAngularVelocity;
			response.Gearbox.Gear = new GearshiftPosition(0);
			response.Gearbox.InputSpeed = inAngularVelocity;
			response.Gearbox.InputTorque = inTorque;
			response.Gearbox.OutputTorque = outTorque;
			response.Gearbox.OutputSpeed = outAngularVelocity;
			return response;
		}

		protected TransmissionLossMap.LossMapResult GetGearLoss(PerSecond outAngularVelocity, NewtonMeter outTorque)
		{
			var loss = ModelData.Gears[Gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			loss.Value *= 0.5;
			return loss;
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval,
			IModalDataContainer container)
		{
			var avgInAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			var avgOutAngularSpeed = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			var inPower = CurrentState.InTorque * avgInAngularSpeed;
			var outPower = CurrentState.OutTorque * avgOutAngularSpeed;
			container[ModalResultField.Gear, AxleNumber.FormatAxleNumber()] =  0;
			container[ModalResultField.P_gbx_loss, AxleNumber.FormatAxleNumber()] = inPower - outPower;
			container[ModalResultField.P_gbx_inertia, AxleNumber.FormatAxleNumber()] = CurrentState.InertiaTorqueLossOut * avgOutAngularSpeed;
			container[ModalResultField.P_gbx_in, AxleNumber.FormatAxleNumber()] = inPower;
			container[ModalResultField.n_gbx_out_avg, AxleNumber.FormatAxleNumber()] = (PreviousState.OutAngularVelocity +
														CurrentState.OutAngularVelocity) / 2.0;
			container[ModalResultField.n_gbx_in_avg, AxleNumber.FormatAxleNumber()] = avgInAngularSpeed;

			container[ModalResultField.T_gbx_out, AxleNumber.FormatAxleNumber()] = CurrentState.OutTorque;
			container[ModalResultField.T_gbx_in, AxleNumber.FormatAxleNumber()] = CurrentState.InTorque;
		}

		public override bool GearEngaged(Second absTime)
		{
			return true;
		}


		public override bool TCLocked => true;


		public override void TriggerGearshift(Second absTime, Second dt)
		{
			
		}

		#region Overrides of VectoSimulationComponent

		protected override bool DoUpdateFrom(object other)
		{
			if (other is DisengagedGearbox g) {
				PreviousState = g.PreviousState.Clone();
				LastShift = g.LastShift;
				return true;
			}
			return false;
		}

		#endregion

		#endregion

		#region Implementation of IHybridControlledGearbox

		public bool SwitchToNeutral { get; set; }

		#endregion
	}
}