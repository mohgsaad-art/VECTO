using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Gearbox for Automatic Power Transmission - No Torque Converter.
	/// </summary>
	public class APTNGearbox : Gearbox
	{
		public APTNGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container, strategy)
		{ }

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

		protected internal override ResponseDryRun Initialize(Second absTime, GearshiftPosition gear,
			NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var oldGear = Gear;
			Gear = gear;
			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear.Gear].Ratio;
			var torqueLossResult = ModelData.Gears[gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			CurrentState.TorqueLossResult = torqueLossResult;
			var inTorque = outTorque / ModelData.Gears[gear.Gear].Ratio + torqueLossResult.Value;

			if (!inAngularVelocity.IsEqual(0)) {
				var alpha = ModelData.Inertia.IsEqual(0) ? 0.SI<PerSquareSecond>() : outTorque / ModelData.Inertia;
				var inertiaPowerLoss = Formulas.InertiaPower(inAngularVelocity, alpha, ModelData.Inertia, Constants.SimulationSettings.TargetTimeInterval);
				inTorque += inertiaPowerLoss / inAngularVelocity;
			}

			var response = NextComponent.Request(absTime, Constants.SimulationSettings.TargetTimeInterval, inTorque, inAngularVelocity, true);

			var eMotor = DataBus.ElectricMotorInfo(DataBus.PowertrainInfo.ElectricMotorPositions[0]);
			var fullLoad = -eMotor.MaxPowerDrive(DataBus.BatteryInfo.InternalVoltage, inAngularVelocity, gear);

			Gear = oldGear;
			return new ResponseDryRun(this, response) {
				ElectricMotor = { PowerRequest = response.ElectricMotor.PowerRequest },
				Gearbox = {
					PowerRequest = outTorque * outAngularVelocity,
					InputSpeed = inAngularVelocity,
					InputTorque = inTorque,
					OutputTorque = outTorque,
					OutputSpeed = outAngularVelocity,
				},
				DeltaFullLoad = response.ElectricMotor.PowerRequest - fullLoad
			};
		}
	}
}