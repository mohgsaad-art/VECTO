using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class BatteryElectricMotorController : IElectricMotorControl
	{
		private VehicleContainer DataBus;
		private ElectricSystem ElectricSystem;
		protected ElectricMotorData ElectricMotorData;

		protected readonly GearboxData GearboxModelData;

		public BatteryElectricMotorController(VehicleContainer container, ElectricSystem es)
		{
			DataBus = container;
			ElectricMotorData = container.RunData.ElectricMachinesData.FirstOrDefault()?.Item2;
			ElectricSystem = es;
			GearboxModelData = container.RunData.GearboxData;
		}

		#region Implementation of IElectricMotorControl

		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, 
			PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, 
			NewtonMeter maxRecuperationTorque, PowertrainPosition position, bool dryRun)
		{
			if (!DataBus.GearboxInfo.GearEngaged(absTime) && DataBus.DriverInfo.DrivingAction == DrivingAction.Roll) {
				var avgSpeed = (prevOutAngularVelocity + currOutAngularVelocity) / 2;
				var inertiaTorqueLoss = avgSpeed.IsEqual(0)
					? 0.SI<NewtonMeter>()
					: Formulas.InertiaPower(currOutAngularVelocity, prevOutAngularVelocity, 
						ElectricMotorData.Inertia, dt) / avgSpeed;
				//var dragTorque = ElectricMotorData.DragCurve.Lookup()
				return (-inertiaTorqueLoss); //.LimitTo(maxDriveTorque, maxRecuperationTorque);
			}
            if (DataBus.DriverInfo.DrivingAction == DrivingAction.Coast ||
                DataBus.DriverInfo.DrivingAction == DrivingAction.Roll) {
                return null;
            }
			
            if ((DataBus.VehicleInfo.VehicleSpeed ?? 0.SI<MeterPerSecond>()).IsSmallerOrEqual(GearboxModelData?.DisengageWhenHaltingSpeed ?? Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed)
				&& outTorque.IsSmaller(0)) {
                return null;
            }

            if (maxDriveTorque == null) {
				return null;
			}

			return (-outTorque).LimitTo(maxDriveTorque, maxRecuperationTorque ?? VectoMath.Max(maxDriveTorque, 0.SI<NewtonMeter>()));
		}

		#endregion
	}
}