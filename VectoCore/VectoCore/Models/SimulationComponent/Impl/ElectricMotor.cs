using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using NLog.LayoutRenderers;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	
	public class ElectricMotor : StatefulProviderComponent<ElectricMotorState, ITnOutPort, ITnInPort, ITnOutPort>, IPowerTrainComponent, IElectricMotor, ITnOutPort, ITnInPort
	{

		protected IElectricSystem ElectricPower;
		protected IElectricMotorControl Control;
		protected ElectricMotorData ModelData;
		private PerSecond _maxSpeed;

		protected internal Joule ThermalBuffer = 0.SI<Joule>();
		protected internal bool DeRatingActive = false;

		public Joule OverloadBuffer { get; }
		public NewtonMeter ContinuousTorque { get; }

		public Watt ContinuousPowerLoss { get; }

		public ElectricMotor(IVehicleContainer container, ElectricMotorData data, IElectricMotorControl control, PowertrainPosition position) : base(container)
		{
			Control = control;
			ModelData = data;
			Position = position;
			container.AddComponent(this); // We have to do this again because in the base class the position is unknown!

			ContinuousTorque = ModelData.ContinuousPower / ModelData.ContinuousPowerSpeed;
			var contElPwr =
				ModelData.EfficiencyMap.LookupElectricPower(ModelData.ContinuousPowerSpeed, -ContinuousTorque).ElectricalPower ??
				ModelData.EfficiencyMap.LookupElectricPower(ModelData.ContinuousPowerSpeed, ModelData.FullLoadCurve.FullLoadDriveTorque(ModelData.ContinuousPowerSpeed), true).ElectricalPower;
			ContinuousPowerLoss = -contElPwr - ModelData.ContinuousPower; // loss needs to be positive
			var maxTqDrive = ModelData.FullLoadCurve.FullLoadDriveTorque(ModelData.ContinuousPowerSpeed);
			var peakElPwr = ModelData.EfficiencyMap.LookupElectricPower(ModelData.ContinuousPowerSpeed, maxTqDrive, true)
				.ElectricalPower;
			var peakPwrLoss = -peakElPwr + ModelData.ContinuousPowerSpeed * maxTqDrive; // losses need to be positive
			OverloadBuffer = (peakPwrLoss - ContinuousPowerLoss) * ModelData.OverloadTime;
		}

		public PowertrainPosition Position { get; }
		public PerSecond MaxSpeed
		{
			get { return _maxSpeed ?? (_maxSpeed = (ModelData.FullLoadCurve.FullLoadEntries.MaxBy(x => x.MotorSpeed).MotorSpeed) / ModelData.Ratio); }
		}

		public Watt DragPower(PerSecond electricMotorSpeed)
		{
			return ModelData.DragCurve.Lookup(electricMotorSpeed) * electricMotorSpeed;
		}

		public Watt MaxPowerDrive(PerSecond electricMotorSpeed)
		{
			return ModelData.FullLoadCurve.FullLoadDriveTorque(electricMotorSpeed) * electricMotorSpeed;
		}

		public NewtonMeter GetTorqueForElectricPower(Watt electricPower, PerSecond avgEmSpeed, Second dt)
		{
			var maxTorque = electricPower > 0
				? GetMaxRecuperationTorque(dt, avgEmSpeed)
				: GetMaxDriveTorque(dt, avgEmSpeed);
			var tqEmMap = ModelData.EfficiencyMap.LookupTorque(electricPower, avgEmSpeed, maxTorque);
			if (tqEmMap == null) {
				return null;
			}

			var emSpeed = avgEmSpeed * 2 - PreviousState.EMSpeed;

			var tqInertia = Formulas.InertiaPower(emSpeed, PreviousState.EMSpeed, ModelData.Inertia, dt) / avgEmSpeed;
			var tqEm = tqEmMap + tqInertia;
			var tqDt = ConvertEmTorqueToDrivetrain(tqEm);
			return tqDt;

		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var emOutAngularVelocity = outAngularVelocity * ModelData.Ratio;
			var emOutTorque = outTorque / ModelData.Ratio;

			PreviousState.EMSpeed = emOutAngularVelocity;
			PreviousState.EMTorque = 0.SI<NewtonMeter>();

			PreviousState.DrivetrainSpeed = outAngularVelocity;
			PreviousState.DrivetrainOutTorque = outTorque;

			if (NextComponent == null) {
				return new ResponseSuccess(this) {
					Engine = {
						PowerRequest = emOutTorque * emOutAngularVelocity,
						EngineSpeed = emOutAngularVelocity
					}
				};
			}
			if (!DataBus.EngineCtl.CombustionEngineOn) {
				PreviousState.DrivetrainInTorque = 0.SI<NewtonMeter>();
				//PreviousState.InAngularVelocity = emOutAngularVelocity;
			}
			return NextComponent.Initialize(outTorque, outAngularVelocity);
			//return NextComponent.Initialize(PreviousState.InTorque, PreviousState.InAngularVelocity);
		}

		/// <summary>
		/// If the electric motor is operated in generator mode, it adds positive torque to the request for the next component,
		/// if the electric motor is operated in drive mode, it adds negative torque (i.e., supports the next component)
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="outTorque"></param>
		/// <param name="outAngularVelocity"></param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		public IResponse Request(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun = false)
		{
			var avgDtSpeed = (PreviousState.DrivetrainSpeed + outAngularVelocity) / 2;
			var emSpeed = outAngularVelocity * ModelData.Ratio;

			var avgEmSpeed = (PreviousState.EMSpeed + emSpeed) / 2;
			var inertiaTorqueEm = avgEmSpeed.IsEqual(0)
				? 0.SI<NewtonMeter>()
				: Formulas.InertiaPower(avgEmSpeed, PreviousState.EMSpeed, ModelData.Inertia, dt) / avgEmSpeed;

			var maxDriveTorqueEmMap = GetMaxDriveTorque(dt, avgEmSpeed);
			var maxRecuperationTorqueEmMap = GetMaxRecuperationTorque(dt, avgEmSpeed);

			// inertia has to be added here. drive torque is negative, when accelerating inertia is positive and thus 'reduces' drive torque, i.e 'less negative'
			var maxDriveTorqueEm = maxDriveTorqueEmMap == null ? null : maxDriveTorqueEmMap + inertiaTorqueEm;
			// inertia has to be added here. recuperation torque is positive, when accelerating inertia is positive and adds more drag to the drivetrain, 
			var maxRecuperationTorqueEm = maxRecuperationTorqueEmMap == null ? null : maxRecuperationTorqueEmMap + inertiaTorqueEm;

			var maxDriveTorqueDt = maxDriveTorqueEm == null ? null : ConvertEmTorqueToDrivetrain(maxDriveTorqueEm);
			var maxRecuperationTorqueDt = maxRecuperationTorqueEm == null ? null : ConvertEmTorqueToDrivetrain(maxRecuperationTorqueEm);
			
			// control returns torque that shall be applied on the drivetrain. calculate backward to the EM
			var emTorqueDt = Control.MechanicalAssistPower(absTime, dt, outTorque,
				PreviousState.DrivetrainSpeed, outAngularVelocity, maxDriveTorqueDt, maxRecuperationTorqueDt, Position, dryRun);

			var emTorque = emTorqueDt == null ? null : ConvertDrivetrainTorqueToEm(emTorqueDt);
			var emOff = emTorqueDt == null;

			if (!dryRun && emTorqueDt != null && ((emTorque).IsSmaller(maxDriveTorqueEm ?? 0.SI<NewtonMeter>(), 1e-3) ||
									(emTorque).IsGreater(maxRecuperationTorqueEm ?? 0.SI<NewtonMeter>(), 1e-3))) {
				// check if provided EM torque (drivetrain) is valid)
				if (DataBus.HybridControllerInfo != null && (!avgDtSpeed.IsEqual(DataBus.HybridControllerInfo.ElectricMotorSpeed(Position)) ||
															!dt.IsEqual(DataBus.HybridControllerInfo.SimulationInterval))) {
					return new ResponseInvalidOperatingPoint(this);
				}
				throw new VectoException(
					"Invalid operating point provided by strategy! SupportPower: {0}, max Power: {1}, min Power: {2}",
					emTorqueDt, maxDriveTorqueDt, maxRecuperationTorqueDt);
			}


			
			if (Position == PowertrainPosition.HybridP2 && !DataBus.GearboxInfo.GearEngaged(absTime)) {
				// electric motor is between gearbox and clutch, but no gear is engaged...
				if (emTorque != null) {
					if (!DataBus.HybridControllerInfo.GearboxEngaged) {
						return new ResponseInvalidOperatingPoint(this) {
							ElectricMotor = {
								MaxDriveTorque = maxDriveTorqueDt,
								MaxRecuperationTorque = maxRecuperationTorqueDt,
								AngularVelocity = avgDtSpeed,
								PowerRequest = outTorque * avgDtSpeed
							}
						};
					}

					if (!dryRun) {
						throw new VectoSimulationException(
							"electric motor cannot provide torque when gearbox and clutch are disengaged");
					}
				}
				// gearbox is disengaged - ignore em inertia and drag...
				emTorqueDt = 0.SI<NewtonMeter>();
				emTorque = 0.SI<NewtonMeter>();
			}

			if (Position == PowertrainPosition.BatteryElectricE2 && !DataBus.GearboxInfo.GearEngaged(absTime)) {
				// electric motor is after the gearbox but no gear engaged - ignore inertia and drag...
				emTorqueDt = 0.SI<NewtonMeter>();
				emTorque = 0.SI<NewtonMeter>();
			}

			if (Position == PowertrainPosition.HybridP1 && !DataBus.EngineCtl.CombustionEngineOn) {
				// electric motor is directly connected to the ICE, ICE is off and EM is off - do not apply drag loss
				emTorqueDt = 0.SI<NewtonMeter>();
				emTorque = 0.SI<NewtonMeter>();
			}

			if (ElectricPower == null || emTorqueDt == null) {
				// no electric system or EM shall be off - apply drag only
				// if EM is off, calculate EM drag torque 'forward' to be applied on drivetrain
				// add inertia, drag is positive
				emTorque =  ModelData.DragCurve.Lookup(avgEmSpeed) + inertiaTorqueEm;
				emTorqueDt = ConvertEmTorqueToDrivetrain(emTorque);
				emOff = true;
			}

			// inertia torque 'brakes' - electric motor has to provide this torque in addition (T_inertia > 0 when angular speed increases)
			// emTorque < 0 when propelling, emTorqueMap needs to be 'more negative' to provide torque for inertia
			// emTorque > 0 when recuperating, inertia 'brakes' in addition, emTorqueMap is decreased
			var emTorqueMap = emTorque - inertiaTorqueEm ;
			if (emOff) {
				// not used later 
				emTorqueMap = null;
			}

			var electricPower = emOff || (ModelData.DragCurve.Lookup(avgEmSpeed) + inertiaTorqueEm).IsEqual(emTorque)
				? 0.SI<Watt>()
				: ModelData.EfficiencyMap
					.LookupElectricPower(avgEmSpeed, emTorqueMap, DataBus.ExecutionMode != ExecutionMode.Declaration)
					.ElectricalPower;

			var electricSupplyResponse =
				ElectricPower.Request(absTime, dt, electricPower, dryRun);
			if (!dryRun && !emOff && !(electricSupplyResponse is ElectricSystemResponseSuccess)) {
				if (DataBus.HybridControllerInfo != null && !avgEmSpeed.IsEqual(DataBus.HybridControllerInfo.ElectricMotorSpeed(Position))) {
					return new ResponseInvalidOperatingPoint(this);
				}
				throw new VectoException(
					"Invalid operating point provided by strategy! EM Torque: {0}, req. electric Power: {1}, battery demand motor: {3}, max Power from Battery: {2}",
					emTorque, electricPower,
					emTorque < 0 ? electricSupplyResponse.MaxPowerDrive : electricSupplyResponse.MaxPowerDrag, electricSupplyResponse.ConsumerPower);
			}

			var inTorqueDt = outTorque + emTorqueDt;

			IResponse retVal = null;
			if (NextComponent == null) {
				// electric motor only
				var remainingPower = inTorqueDt * avgDtSpeed;
				if (dryRun) {
					retVal = new ResponseDryRun(this) {
						Engine = { EngineSpeed = avgDtSpeed},
						ElectricMotor = {
							ElectricMotorPowerMech = (inTorqueDt - outTorque) * avgDtSpeed,
							TotalTorqueDemand = inTorqueDt,
						},
						DeltaFullLoad =  remainingPower,
						DeltaDragLoad = remainingPower,
					};
				} else {

					if (remainingPower.IsEqual(0, Constants.SimulationSettings.LineSearchTolerance)) {
						if (electricSupplyResponse.MaxPowerDrive.IsGreaterOrEqual(0)) {
							retVal = new ResponseBatteryEmpty(this);
						} else {
							retVal = new ResponseSuccess(this) {
								ElectricMotor = {
									ElectricMotorPowerMech = (inTorqueDt - outTorque) * avgDtSpeed,
									TotalTorqueDemand = inTorqueDt
								},
								Engine = {
									PowerRequest = 0.SI<Watt>(),
									EngineSpeed = outAngularVelocity
								},
							};
						}
					} else {
						if (remainingPower > 0) {
							retVal = new ResponseOverload(this) { Delta = remainingPower };
						} else {
							retVal = new ResponseUnderload(this) { Delta = remainingPower };
						}

						retVal.Engine.EngineSpeed = avgDtSpeed;
					}
				}
			} else {
				retVal = NextComponent.Request(absTime, dt, inTorqueDt, outAngularVelocity, dryRun);
				retVal.ElectricMotor.ElectricMotorPowerMech = (inTorqueDt - outTorque) * avgDtSpeed;
				retVal.ElectricMotor.TotalTorqueDemand = inTorqueDt;
			}

			retVal.ElectricMotor.MaxDriveTorque = maxDriveTorqueDt;
			retVal.ElectricMotor.MaxRecuperationTorque = maxRecuperationTorqueDt;
			retVal.ElectricMotor.AngularVelocity = avgEmSpeed;
			
			retVal.ElectricMotor.PowerRequest = outTorque * outAngularVelocity;
			retVal.ElectricMotor.InertiaPowerDemand = inertiaTorqueEm * avgEmSpeed;
			retVal.ElectricSystem = electricSupplyResponse;
			
			if (!dryRun) {
				CurrentState.EMSpeed = emSpeed;
				CurrentState.EMTorque = emOff ? null : emTorque;
				CurrentState.EmTorqueMap = emTorqueMap;
				CurrentState.DragMax = maxRecuperationTorqueEmMap;
				CurrentState.DriveMax = maxDriveTorqueEmMap;

				CurrentState.InertiaTorqueLoss = inertiaTorqueEm;
				CurrentState.DrivetrainSpeed = outAngularVelocity;
				CurrentState.DrivetrainInTorque = inTorqueDt;
				CurrentState.DrivetrainOutTorque = outTorque;

				CurrentState.TransmissionTorqueLoss = avgDtSpeed.IsEqual(0) ? 0.SI<NewtonMeter>() :
					((inTorqueDt - outTorque) * avgDtSpeed - emTorque * avgEmSpeed) / avgDtSpeed;

				CurrentState.ElectricPowerToBattery = retVal.ElectricSystem?.ConsumerPower;

			}
			return retVal;
		}

		private NewtonMeter GetMaxRecuperationTorque(Second dt, PerSecond avgSpeed)
		{
			var tqContinuousPwr = DeRatingActive ? ContinuousTorque : null;
			if (!avgSpeed.IsEqual(0)) {
				tqContinuousPwr = DeRatingActive ? ModelData.ContinuousPower / avgSpeed : null;
			}
			var maxEmTorque = VectoMath.Min(tqContinuousPwr, ModelData.FullLoadCurve.FullGenerationTorque(avgSpeed));
			var electricSystemResponse = ElectricPower.Request(0.SI<Second>(), dt, 0.SI<Watt>(), true);
			var maxBatPower = electricSystemResponse.MaxPowerDrag;

			if (maxBatPower.IsSmaller(0, 1e-3)) {
				// has to be positive for recuperation - battery is full
				return null;
			}

			var maxBatRecuperationTorque = maxBatPower.IsEqual(0, 1e-3) ? ModelData.DragCurve.Lookup(avgSpeed) : ModelData.EfficiencyMap.LookupTorque(maxBatPower, avgSpeed, maxEmTorque);
			var maxTorqueRecuperate = VectoMath.Min(maxEmTorque, maxBatRecuperationTorque);
			return maxTorqueRecuperate < 0 ? null : maxTorqueRecuperate;
		}

		private NewtonMeter GetMaxDriveTorque(Second dt, PerSecond avgSpeed)
		{
			var tqContinuousPwr = DeRatingActive ? -ContinuousTorque : null;
			if (!avgSpeed.IsEqual(0)) {
				tqContinuousPwr = DeRatingActive ? -ModelData.ContinuousPower / avgSpeed : null;
			}
			var maxEmTorque = VectoMath.Max(tqContinuousPwr ,ModelData.FullLoadCurve.FullLoadDriveTorque(avgSpeed));
			var electricSystemResponse = ElectricPower.Request(0.SI<Second>(), dt, 0.SI<Watt>(), true);
			var maxBatPower = electricSystemResponse.MaxPowerDrive;

			if (maxBatPower.IsGreater(0, 1e-3)) {
				// has to be negative for propelling - so battery is below min SoC
				return null;
			}

			var maxBatDriveTorque = maxBatPower.IsEqual(0, 1e-3) ? ModelData.DragCurve.Lookup(avgSpeed) : ModelData.EfficiencyMap.LookupTorque(maxBatPower, avgSpeed, maxEmTorque);
			//if (maxBatDriveTorque == null) {
			//	return ModelData.DragCurve.Lookup(avgSpeed);
			//}
			var maxTorqueDrive = VectoMath.Max(maxEmTorque, maxBatDriveTorque);
			return maxTorqueDrive > 0 ? null : maxTorqueDrive;
		}


		protected NewtonMeter ConvertEmTorqueToDrivetrain(NewtonMeter emTorque)
		{
			return emTorque * ModelData.Ratio *
					(emTorque < 0 ? ModelData.TransmissionEfficiency : 1 / ModelData.TransmissionEfficiency);
		}

		protected NewtonMeter ConvertDrivetrainTorqueToEm(NewtonMeter dtTorque)
		{
			return dtTorque / ModelData.Ratio *
					(dtTorque < 0 ? 1 / ModelData.TransmissionEfficiency : ModelData.TransmissionEfficiency);
		}


		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var avgEMSpeed = (PreviousState.EMSpeed + CurrentState.EMSpeed) / 2;
			var avgDTSpeed = (PreviousState.DrivetrainSpeed + CurrentState.DrivetrainSpeed) / 2;

			container[ModalResultField.EM_ratio_, Position] = ModelData.Ratio.SI<Scalar>();
			container[ModalResultField.n_EM_electricMotor_, Position] = avgEMSpeed;
			container[ModalResultField.T_EM_electricMotor_, Position] = CurrentState.EMTorque;
			container[ModalResultField.T_EM_electricMotor_map_, Position] = CurrentState.EmTorqueMap;

			container[ModalResultField.T_EM_electricMotor_drive_max_, Position] = CurrentState.DriveMax;
			container[ModalResultField.T_EM_electricMotor_gen_max_, Position] = CurrentState.DragMax;

			container[ModalResultField.P_EM_electricMotor_gen_max_, Position] = (CurrentState.DragMax ?? 0.SI<NewtonMeter>()) * avgEMSpeed;
			container[ModalResultField.P_EM_electricMotor_drive_max_, Position] = (CurrentState.DriveMax ?? 0.SI<NewtonMeter>()) * avgEMSpeed;
			
			container[ModalResultField.P_EM_electricMotor_em_mech_, Position] = (CurrentState.EMTorque ?? 0.SI<NewtonMeter>() ) * avgEMSpeed;
			container[ModalResultField.P_EM_electricMotor_em_mech_map_, Position] = (CurrentState.EmTorqueMap ?? 0.SI<NewtonMeter>()) * avgEMSpeed;


			container[ModalResultField.P_EM_in_, Position] = CurrentState.DrivetrainInTorque * avgDTSpeed;
			container[ModalResultField.P_EM_out_, Position] = CurrentState.DrivetrainOutTorque * avgDTSpeed;
			container[ModalResultField.T_EM_out_, Position] = CurrentState.DrivetrainOutTorque;
			container[ModalResultField.P_EM_mech_, Position] = (CurrentState.DrivetrainInTorque - CurrentState.DrivetrainOutTorque) * avgDTSpeed;
			
			container[ModalResultField.P_EM_electricMotor_el_, Position] = CurrentState.ElectricPowerToBattery;
			
			container[ModalResultField.P_EM_electricMotorLoss_, Position] = (CurrentState.EmTorqueMap ?? 0.SI<NewtonMeter>()) * avgEMSpeed - CurrentState.ElectricPowerToBattery;

			container[ModalResultField.P_EM_TransmissionLoss_, Position] = CurrentState.TransmissionTorqueLoss * avgDTSpeed;

			container[ModalResultField.P_EM_electricMotorInertiaLoss_, Position] = CurrentState.InertiaTorqueLoss * avgEMSpeed;

			container[ModalResultField.P_EM_loss_, Position] = (CurrentState.DrivetrainInTorque - CurrentState.DrivetrainOutTorque) * avgDTSpeed - CurrentState.ElectricPowerToBattery;

			container[ModalResultField.EM_Off_, Position] = CurrentState.EMTorque == null ? 1.SI<Scalar>() : 0.SI<Scalar>();

			var losses = (CurrentState.EmTorqueMap ?? 0.SI<NewtonMeter>()) * avgEMSpeed - CurrentState.ElectricPowerToBattery;
			var contribution = (losses - ContinuousPowerLoss) * simulationInterval;
			container[ModalResultField.ElectricMotor_OvlBuffer_, Position] = VectoMath.Max(0, (ThermalBuffer + contribution) / OverloadBuffer);
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			var avgSpeed = (PreviousState.EMSpeed + CurrentState.EMSpeed) / 2;
			var losses = (CurrentState.EMTorque ?? 0.SI<NewtonMeter>()) * avgSpeed - CurrentState.ElectricPowerToBattery;
			ThermalBuffer += (losses - ContinuousPowerLoss) * simulationInterval;
			if (ThermalBuffer < 0) {
				ThermalBuffer = 0.SI<Joule>();
			}

			if (DeRatingActive) {
				if (ThermalBuffer.IsSmallerOrEqual(OverloadBuffer * ModelData.OverloadRegenerationFactor)) {
					DeRatingActive = false;
				}
			} else {
				if (ThermalBuffer.IsGreater(OverloadBuffer)) {
					DeRatingActive = true;
				}
			}
			base.DoCommitSimulationStep(time, simulationInterval);
		}

		
		//public NewtonMeter ElectricDragTorque(PerSecond electricMotorSpeed, Second dt, DrivingBehavior drivingBehavior)
		//{
		//	return Control.MaxDragTorque(electricMotorSpeed, dt);
		//}



		public PerSecond ElectricMotorSpeed
		{
			get { return PreviousState.EMSpeed; }
		}

		public void Connect(IElectricSystem powersupply)
		{
			ElectricPower = powersupply;
		}
	}

	public class ElectricMotorState // : SimpleComponentState
	{

		public PerSecond DrivetrainSpeed = 0.RPMtoRad();
		public NewtonMeter DrivetrainInTorque = 0.SI<NewtonMeter>();
		public NewtonMeter DrivetrainOutTorque = 0.SI<NewtonMeter>();
		public NewtonMeter TransmissionTorqueLoss;

		public PerSecond EMSpeed = 0.RPMtoRad();
		public NewtonMeter EMTorque;
		public NewtonMeter EmTorqueMap;

		public NewtonMeter DriveMax;
		public NewtonMeter DragMax;
		public NewtonMeter InertiaTorqueLoss;


		public Watt ElectricPowerToBattery;
	}
}