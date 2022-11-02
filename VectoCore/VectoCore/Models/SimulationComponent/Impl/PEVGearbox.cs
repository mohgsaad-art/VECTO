using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class PEVGearbox : Gearbox
	{
		public PEVGearbox(IVehicleContainer container, IShiftStrategy strategy) : base(container, strategy) { }

		protected internal override ResponseDryRun Initialize(Second absTime, GearshiftPosition gear, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var oldGear = Gear;
			Gear = gear;
			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear.Gear].Ratio;
			var torqueLossResult = ModelData.Gears[gear.Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			CurrentState.TorqueLossResult = torqueLossResult;
			var inTorque = outTorque / ModelData.Gears[gear.Gear].Ratio + torqueLossResult.Value;

			if (!inAngularVelocity.IsEqual(0)) {
				var alpha = ModelData.Inertia.IsEqual(0)
					? 0.SI<PerSquareSecond>()
					: outTorque / ModelData.Inertia;

				var inertiaPowerLoss = Formulas.InertiaPower(inAngularVelocity, alpha, ModelData.Inertia,
					Constants.SimulationSettings.TargetTimeInterval);
				inTorque += inertiaPowerLoss / inAngularVelocity;
			}

			var response =
				NextComponent.Request(absTime, Constants.SimulationSettings.TargetTimeInterval, inTorque,
					inAngularVelocity, true); //NextComponent.Initialize(inTorque, inAngularVelocity);

			var fullLoad = -DataBus.ElectricMotorInfo(PowertrainPosition.BatteryElectricE2).MaxPowerDrive(DataBus.BatteryInfo.InternalVoltage, inAngularVelocity, Gear);

			Gear = oldGear;
			return new ResponseDryRun(this, response) {
				ElectricMotor = {
					PowerRequest = response.ElectricMotor.PowerRequest
				},
				Gearbox = {
					PowerRequest = outTorque * outAngularVelocity,
				},
				DeltaFullLoad = response.ElectricMotor.PowerRequest - fullLoad
			};
		}
	}
}