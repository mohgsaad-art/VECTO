using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox
{
	public class IEPCGearboxMultipleGears : AbstractAMTGearbox, IIEPCGearbox
	{
		public IEPCGearboxMultipleGears(IVehicleContainer container, IShiftStrategy strategy, int axleNumber) : this(container, strategy, false, axleNumber)
		{
			if (container.IsTestPowertrain) {
				throw new VectoException(
					"This class shall not be used in a testpowertrain - use the dedicated class instead!");
			}
		}

		protected IEPCGearboxMultipleGears(IVehicleContainer container, IShiftStrategy strategy, bool dummy, int axleNumber) 
			: base(container, strategy, dummy, axleNumber)
		{
			_gear = new GearshiftPosition(0);
		}

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			EngageTime = -double.MaxValue.SI<Second>();

			if (_strategy != null && (Disengaged || DisengageGearbox)) {
				Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear.Gear].Ratio;
			var gearboxTorqueLoss = ModelData.Gears[Gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			CurrentState.TorqueLossResult = gearboxTorqueLoss;

			var inTorque = outTorque / ModelData.Gears[Gear.Gear].Ratio
							+ gearboxTorqueLoss.Value;

			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			PreviousState.Gear = Gear;
			Disengaged = false;

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);

			return response;
		}

		public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun = false)
		{
			var response = base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
			if (response is ResponseGearShift) {
				response = base.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
			}
			return response;
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval,
			IModalDataContainer container)
		{
			container[ModalResultField.Gear, AxleNumber.FormatAxleNumber()] = Disengaged || DataBus.VehicleInfo.VehicleStopped ? 0 : Gear.Gear;
			container[ModalResultField.n_IEPC_out_avg, AxleNumber.FormatAxleNumber()] = (PreviousState.OutAngularVelocity +
														CurrentState.OutAngularVelocity) / 2.0;
			container[ModalResultField.T_IEPC_out, AxleNumber.FormatAxleNumber()] = CurrentState.OutTorque;
			_strategy.WriteModalResults(container);
		}
	}
}