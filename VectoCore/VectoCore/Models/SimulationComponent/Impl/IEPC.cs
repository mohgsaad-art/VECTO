using System;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

	public class TestpowertrainIEPC : IEPC, ITestpowertrainElectricMotor
	{
		public TestpowertrainIEPC(IVehicleContainer container, IPowertrainContainer powertrainContainer, ElectricMotorData data, IElectricMotorControl control,
			PowertrainPosition position) : base(container, powertrainContainer, data, control, position, false)
		{
			if (!container.IsTestPowertrain) {
				throw new Exception("TestpowertrainIEPC component must not be used in real powertrain - use dedicated component instead");
			}

        }

		public IElectricSystem GetElectricSystem => ElectricPower;
		public ElectricMotorState GetPreviousState { get => PreviousState; }
		public Joule SetThermalBuffer
		{
			set { ThermalBuffer = value; }
		}
		public bool SetDeRatingActive
		{
			set { DeRatingActive = value; }
		}
	}

	public class IEPC : ElectricMotor
	{
		public IEPC(IVehicleContainer container, IPowertrainContainer powertrainContainer, ElectricMotorData data, IElectricMotorControl control,
			PowertrainPosition position) : base(container, powertrainContainer, data, control, position)
		{
			if (container.IsTestPowertrain) {
				throw new Exception("IEPC component must not be used in test powertrain - use dedicated component instead");
			}
		}

		protected IEPC(IVehicleContainer container, IPowertrainContainer powertrainContainer, ElectricMotorData data, IElectricMotorControl control,
			PowertrainPosition position, bool dummy) 
			: base(container, powertrainContainer, data, control, position, false) { }

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var prevDtSpeed = CurrentState.IceSwitchedOn ? CurrentState.ICEOnSpeed : PreviousState.DrivetrainSpeed;
			var prevEmSpeed = CurrentState.IceSwitchedOn ? prevDtSpeed * ModelData.RatioADC : PreviousState.EMSpeed;

			var avgEMSpeed = (prevEmSpeed + CurrentState.EMSpeed) / 2;
			var avgDTSpeed = (prevDtSpeed + CurrentState.DrivetrainSpeed) / 2;

			//container[ModalResultField.EM_ratio_, Position] = ModelData.RatioADC.SI<Scalar>();
			container[ModalResultField.n_IEPC_int_, EMPosition, AxleNumber] = avgEMSpeed;
			container[ModalResultField.T_IEPC_, EMPosition, AxleNumber] = CurrentState.EMTorque;
			container[ModalResultField.T_IEPC_map_, EMPosition, AxleNumber] = CurrentState.EmTorqueMap;

			container[ModalResultField.T_IEPC_int_drive_max_, EMPosition, AxleNumber] = CurrentState.DriveMax;
			container[ModalResultField.T_IEPC_int_gen_max_, EMPosition, AxleNumber] = CurrentState.DragMax;

			container[ModalResultField.P_IEPC_int_gen_max_, EMPosition, AxleNumber] = (CurrentState.DragMax ?? 0.SI<NewtonMeter>()) * avgEMSpeed;
			container[ModalResultField.P_IEPC_int_drive_max_, EMPosition, AxleNumber] = (CurrentState.DriveMax ?? 0.SI<NewtonMeter>()) * avgEMSpeed;

			//container[ModalResultField.P_EM_electricMotor_em_mech_, Position] = (CurrentState.EMTorque ?? 0.SI<NewtonMeter>()) * avgEMSpeed;
			container[ModalResultField.P_IEPC_int_mech_map_, EMPosition, AxleNumber] = (CurrentState.EmTorqueMap ?? 0.SI<NewtonMeter>()) * avgEMSpeed;


			//container[ModalResultField.P_EM_in_, Position] = CurrentState.DrivetrainInTorque * avgDTSpeed;
			container[ModalResultField.P_IEPC_out_, EMPosition, AxleNumber] = CurrentState.DrivetrainOutTorque * avgDTSpeed;
			//container[ModalResultField.P_EM_mech_, Position] = (CurrentState.DrivetrainInTorque - CurrentState.DrivetrainOutTorque) * avgDTSpeed;

			container[ModalResultField.P_IEPC_el_, EMPosition, AxleNumber] = CurrentState.ElectricPowerToBattery;

			container[ModalResultField.P_IEPC_electricMotorLoss_, EMPosition, AxleNumber] = (CurrentState.DrivetrainInTorque - CurrentState.DrivetrainOutTorque) * avgDTSpeed - CurrentState.ElectricPowerToBattery;

			//container[ModalResultField.P_EM_TransmissionLoss_, Position] = CurrentState.TransmissionTorqueLoss * avgDTSpeed;

			container[ModalResultField.P_IEPC_electricMotorInertiaLoss_, EMPosition, AxleNumber] = CurrentState.InertiaTorqueLoss * avgEMSpeed;

			//container[ModalResultField.P_EM_loss_, Position] = (CurrentState.DrivetrainInTorque - CurrentState.DrivetrainOutTorque) * avgDTSpeed - CurrentState.ElectricPowerToBattery;

			container[ModalResultField.IEPC_Off_, EMPosition, AxleNumber] = CurrentState.EMTorque == null ? 1.SI<Scalar>() : 0.SI<Scalar>();

			var losses = (CurrentState.EmTorqueMap ?? 0.SI<NewtonMeter>()) * avgEMSpeed - CurrentState.ElectricPowerToBattery;
			var contribution = (losses - ModelData.Overload.ContinuousPowerLoss) * simulationInterval;
			if (DeRatingActive && contribution.IsGreater(0)) {
				contribution = 0.SI<WattSecond>();
			}

			if (ThermalBuffer + contribution > ModelData.Overload.OverloadBuffer) {
				contribution = (ModelData.Overload.OverloadBuffer - ThermalBuffer).Cast<WattSecond>();
			}
			if (ModelData.Overload.OverloadBuffer.Value() != 0) { // mk2021-08-03 overloadbuffer was 0 in Test Case: "ADASTestPEV.TestPCCEngineeringSampleCases G5Eng PCC12 Case A"
				container[ModalResultField.IEPC_OvlBuffer_, EMPosition, AxleNumber] = VectoMath.Max(0, (ThermalBuffer + contribution) / ModelData.Overload.OverloadBuffer);
			}

			if (NextComponent == null && BusAux != null) {
				BusAux.DoWriteModalResultsICE(time, simulationInterval, container);
			}
		}
	}
}