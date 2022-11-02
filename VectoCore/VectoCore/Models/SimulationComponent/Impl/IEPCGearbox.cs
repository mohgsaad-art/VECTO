using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class IEPCGearbox : APTNGearbox
	{
		public IEPCGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container, strategy) { }

		protected override void DoWriteModalResults(Second time, Second simulationInterval,
			IModalDataContainer container)
		{
			//var avgInAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			//var avgOutAngularSpeed = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			//var inPower = CurrentState.InTorque * avgInAngularSpeed;
			//var outPower = CurrentState.OutTorque * avgOutAngularSpeed;
			container[ModalResultField.Gear] = Disengaged || DataBus.VehicleInfo.VehicleStopped ? 0 : Gear.Gear;
			//container[ModalResultField.P_gbx_loss] = inPower - outPower;
			//container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgOutAngularSpeed;
			//container[ModalResultField.P_gbx_in] = inPower;
			container[ModalResultField.n_IEPC_out_avg] = (PreviousState.OutAngularVelocity +
														CurrentState.OutAngularVelocity) / 2.0;
			container[ModalResultField.T_IEPC_out] = CurrentState.OutTorque;
			//container[ModalResultField.T_gbx_in] = CurrentState.InTorque;
			_strategy.WriteModalResults(container);
		}
	}
}