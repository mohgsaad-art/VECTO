using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{

	public class SerialHybridStrategyAT : AbstractSerialHybridStrategy<APTNGearbox>
	{
		public SerialHybridStrategyAT(VectoRunData runData, IVehicleContainer container) : base(runData, container) { }

		#region Overrides of AbstractSerialHybridStrategy<ATGearbox>

		protected override DrivetrainDemand GetDrivetrainPowerDemand(Second absTime, Second dt, NewtonMeter outTorque,
		PerSecond outAngularVelocity, GenSetOperatingPoint maxPowerGenset)
		{
			if (TestPowertrain.Gearbox != null) {
				var gearboxInfo = DataBus.GearboxInfo as APTNGearbox;
				if (gearboxInfo == null) {
					throw new VectoException("AT Gearbox Required!");
				}
				var currentGear = DataBus.VehicleInfo.VehicleStopped
					? gearboxInfo.NextGear
					: DataBus.GearboxInfo.Gear;

				TestPowertrain.Gearbox.PreviousState.InAngularVelocity =
					gearboxInfo.PreviousState.InAngularVelocity;
				TestPowertrain.Gearbox.Disengaged = gearboxInfo.Disengaged;
				TestPowertrain.Gearbox.DisengageGearbox = gearboxInfo.DisengageGearbox;
				TestPowertrain.Gearbox.Gear = currentGear;
				TestPowertrain.Gearbox._nextGear = gearboxInfo.NextGear;
			}
			TestPowertrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			(TestPowertrain.Container.VehicleInfo as Vehicle).PreviousState.Velocity =
				(DataBus.VehicleInfo as Vehicle).PreviousState.Velocity;

			TestPowertrain.ElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(EmPosition) as ElectricMotor).ThermalBuffer;
			TestPowertrain.ElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(EmPosition) as ElectricMotor).DeRatingActive;

			TestPowertrain.Battery?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			if (TestPowertrain.Battery != null) {
				TestPowertrain.Battery.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as Battery).PreviousState.PulseDuration;
				TestPowertrain.Battery.PreviousState.PowerDemand =
					(DataBus.BatteryInfo as Battery).PreviousState.PowerDemand;
			}
			if (TestPowertrain.BatterySystem != null) {
				var batSystem = DataBus.BatteryInfo as BatterySystem;
				foreach (var bsKey in batSystem.Batteries.Keys) {
					for (var i = 0; i < batSystem.Batteries[bsKey].Batteries.Count; i++) {
						TestPowertrain.BatterySystem.Batteries[bsKey].Batteries[i]
							.Initialize(batSystem.Batteries[bsKey].Batteries[i].StateOfCharge);
					}
				}
				TestPowertrain.BatterySystem.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as BatterySystem).PreviousState.PulseDuration;
				TestPowertrain.BatterySystem.PreviousState.PowerDemand = (DataBus.BatteryInfo as BatterySystem).PreviousState.PowerDemand;
			}

			TestPowertrain.Charger.ChargingPower = maxPowerGenset.ElectricPower;

			if (TestPowertrain.WHRCharger != null) {
				TestPowertrain.WHRCharger.PreviousState.GeneratedEnergy =
					DataBus.WHRCharger.PreviousState.GeneratedEnergy;
				TestPowertrain.WHRCharger.PreviousState.ExcessiveEnergy =
					DataBus.WHRCharger.PreviousState.ExcessiveEnergy;
			}

			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque,
				Controller.PreviousState.OutAngularVelocity);

			if (TestPowertrain.Gearbox != null) {
				var gearboxInfo = DataBus.GearboxInfo as APTNGearbox;
				TestPowertrain.Gearbox.PreviousState.OutAngularVelocity = gearboxInfo.PreviousState.OutAngularVelocity;
				TestPowertrain.Gearbox.PreviousState.InAngularVelocity = gearboxInfo.PreviousState.InAngularVelocity;
			}

			TestPowertrain.Brakes.BrakePower = DataBus.Brakes.BrakePower;
			var testResponse =
				TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, false);

			TestPowertrain.HybridController.ApplyStrategySettings(new HybridStrategyResponse() {
				CombustionEngineOn = false,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
					{
						EmPosition,
						Tuple.Create(testResponse.ElectricMotor.AvgDrivetrainSpeed, -testResponse.ElectricMotor.TorqueRequest)
					}
				}
			});
			var testResponse2 =
				TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity,
					false);
			return new DrivetrainDemand() {
				AvgEmDrivetrainSpeed = testResponse2.ElectricMotor.AvgDrivetrainSpeed,
				EmTorqueDemand = testResponse2.ElectricMotor.TorqueRequest,
				ElectricPowerDemand = testResponse2.ElectricSystem.ConsumerPower,
				Response = testResponse2
			};
		}


		#endregion
	}

	// =======================================================================


	public class SerialHybridStrategy : AbstractSerialHybridStrategy<Gearbox>
	{

		public SerialHybridStrategy(VectoRunData runData, IVehicleContainer container) : base(runData, container) { }

		protected override DrivetrainDemand GetDrivetrainPowerDemand(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond outAngularVelocity, GenSetOperatingPoint maxPowerGenset)
		{
			if (TestPowertrain.Gearbox != null) {
				var currentGear = DataBus.VehicleInfo.VehicleStopped
					? (DataBus.GearboxInfo as Gearbox).NextGear
					: DataBus.GearboxInfo.Gear;

				TestPowertrain.Gearbox.PreviousState.InAngularVelocity =
					(DataBus.GearboxInfo as Gearbox).PreviousState.InAngularVelocity;
				TestPowertrain.Gearbox.Disengaged = (DataBus.GearboxInfo as Gearbox).Disengaged;
				TestPowertrain.Gearbox.DisengageGearbox = (DataBus.GearboxInfo as Gearbox).DisengageGearbox;
				TestPowertrain.Gearbox.Gear = currentGear;
				TestPowertrain.Gearbox._nextGear = (DataBus.GearboxInfo as Gearbox).NextGear;
			}
			TestPowertrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());

			TestPowertrain.ElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(EmPosition) as ElectricMotor).ThermalBuffer;
			TestPowertrain.ElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(EmPosition) as ElectricMotor).DeRatingActive;

			TestPowertrain.Battery?.Initialize(DataBus.BatteryInfo.StateOfCharge);
			if (TestPowertrain.Battery != null) {
				TestPowertrain.Battery.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as Battery).PreviousState.PulseDuration;
				TestPowertrain.Battery.PreviousState.PowerDemand =
					(DataBus.BatteryInfo as Battery).PreviousState.PowerDemand;
			}
			if (TestPowertrain.BatterySystem != null) {
				var batSystem = DataBus.BatteryInfo as BatterySystem;
				foreach (var bsKey in batSystem.Batteries.Keys) {
					for (var i = 0; i < batSystem.Batteries[bsKey].Batteries.Count; i++) {
						TestPowertrain.BatterySystem.Batteries[bsKey].Batteries[i]
							.Initialize(batSystem.Batteries[bsKey].Batteries[i].StateOfCharge);
					}
				}
				TestPowertrain.BatterySystem.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as BatterySystem).PreviousState.PulseDuration;
				TestPowertrain.BatterySystem.PreviousState.PowerDemand = (DataBus.BatteryInfo as BatterySystem).PreviousState.PowerDemand;
			}

			TestPowertrain.Charger.ChargingPower = maxPowerGenset.ElectricPower;

			if (TestPowertrain.WHRCharger != null) {
				TestPowertrain.WHRCharger.PreviousState.GeneratedEnergy =
					DataBus.WHRCharger.PreviousState.GeneratedEnergy;
				TestPowertrain.WHRCharger.PreviousState.ExcessiveEnergy =
					DataBus.WHRCharger.PreviousState.ExcessiveEnergy;
			}

			TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque,
				Controller.PreviousState.OutAngularVelocity);
			TestPowertrain.Brakes.BrakePower = DataBus.Brakes.BrakePower;
			if (TestPowertrain.Gearbox != null) {
				TestPowertrain.Gearbox.PreviousState.InAngularVelocity =
					(DataBus.GearboxInfo as Gearbox).PreviousState.InAngularVelocity;
			}
			var testResponse =
				TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, false);

			TestPowertrain.HybridController.ApplyStrategySettings(new HybridStrategyResponse() {
				CombustionEngineOn = false,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
					{
						EmPosition,
						Tuple.Create(testResponse.ElectricMotor.AvgDrivetrainSpeed, -testResponse.ElectricMotor.TorqueRequest)
					}
				}
			});
			var testResponse2 =
				TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity,
					false);
			return new DrivetrainDemand() {
				AvgEmDrivetrainSpeed = testResponse2.ElectricMotor.AvgDrivetrainSpeed,
				EmTorqueDemand = testResponse2.ElectricMotor.TorqueRequest,
				ElectricPowerDemand = testResponse2.ElectricSystem.ConsumerPower,
				Response = testResponse2
			};
		}

	}

	// =======================================================================

	public abstract class AbstractSerialHybridStrategy<T>  : LoggingObject, IHybridControlStrategy where T : class, IHybridControlledGearbox, IGearbox
	{

		public enum StateMachineState
		{
			Undefined,
			Acc_S0, // GEN = 0
			Acc_S1, // P_GEN = P_opt, SoC <= SoC_min && P_demand < P_opt || SoC >= SoC_min && SoC <= SoC_target && P_demand <= P_opt
			Acc_S2, // P_GEN = P_max, SoC <= S
			Acc_S3, // P_GEN = P_max, P_drive = P_GEN

			Break_S0,
			Break_S1,
			Break_S2,
		}

		public enum GensetState
		{
			Off = 1,
			OptimalPoint,
			MaximalPoint,
			OptimalPointDeRated,
			MaximalPointDeRated,
		}

		protected VectoRunData ModelData;
		protected IDataBus DataBus;

		protected HybridStrategyParameters StrategyParameters;

		//protected Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> ElectricMotorsOff;

		protected StrategyState CurrentState = new StrategyState();
		protected StrategyState PreviousState = new StrategyState();

		protected TestPowertrain<T> TestPowertrain;
		protected TestGenset TestGenSet;
		protected GenSetCharacteristics GenSetCharacteristics;
			
		protected PowertrainPosition EmPosition;

		protected DryRunSolutionState DryRunSolution { get; set; }

		public VelocityRollingLookup VelocityDropData { get; }

		public event Action GearShiftTriggered;

		public AbstractSerialHybridStrategy (VectoRunData runData, IVehicleContainer container)
		{
			DataBus = container;
			ModelData = runData;
			if (ModelData.ElectricMachinesData.Select(x => x.Item1).Where(x => x != PowertrainPosition.GEN).Distinct().Count() > 1) {
				throw new VectoException("More than one electric motors are currently not supported");
			}
			StrategyParameters = ModelData.HybridStrategyParameters;
			if (StrategyParameters == null) {
				throw new VectoException("Model parameters for hybrid strategy required!");
			}

			//ElectricMotorsOff = ModelData.ElectricMachinesData
			//	.Select(x => new KeyValuePair<PowertrainPosition, NewtonMeter>(x.Item1, null))
			//	.ToDictionary(x => x.Key, x => new Tuple<PerSecond, NewtonMeter>(null, x.Value));

			var emDtData = runData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item2;
			var minGensetPower = emDtData.EfficiencyData.VoltageLevels.First().FullLoadCurve.MaxPower * StrategyParameters.GensetMinOptPowerFactor;

			GenSetCharacteristics = new GenSetCharacteristics(minGensetPower);
			runData.GenSet = new VectoRunData.GenSetData() { GenSetCharacteristics = GenSetCharacteristics };

			// create testcontainer
			var testContainer = new SimplePowertrainContainer(runData);
			if (runData.JobType == VectoSimulationJobType.IEPC_S) {
				PowertrainBuilder.BuildSimpleIEPCHybridPowertrain(runData, testContainer);
			} else {
				PowertrainBuilder.BuildSimpleSerialHybridPowertrain(runData, testContainer);
			}

			TestPowertrain = new TestPowertrain<T>(testContainer, DataBus);

			var gensetContainer = new SimplePowertrainContainer(runData);
			PowertrainBuilder.BuildSimpleGenSet(runData, gensetContainer);
			TestGenSet = new TestGenset(gensetContainer, DataBus);

			
			container.AddPreprocessor(new GensetPreprocessor(GenSetCharacteristics ,TestGenSet, runData.EngineData,
				runData.ElectricMachinesData.FirstOrDefault(x => x.Item1 == PowertrainPosition.GEN)?.Item2));
		}


		public virtual IHybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			EmPosition = DataBus.PowertrainInfo.ElectricMotorPositions.FirstOrDefault(x =>
				x != PowertrainPosition.GEN);

			var retVal = new HybridStrategyResponse() { MechanicalAssistPower = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() };

			foreach (var em in ModelData.ElectricMachinesData) {
				retVal.MechanicalAssistPower[em.Item1] = null;
			}

			GenSetCharacteristics.ContinuousTorque = ModelData.ElectricMachinesData
				.FirstOrDefault(x => x.Item1 == EmPosition)?.Item2.Overload.ContinuousTorque ?? 0.SI<NewtonMeter>(); 

			PreviousState.AngularVelocity = outAngularVelocity;
			PreviousState.SMState = DataBus.BatteryInfo.StateOfCharge > StrategyParameters.TargetSoC
				? StateMachineState.Acc_S0
				: DataBus.BatteryInfo.StateOfCharge > StrategyParameters.MinSoC
					? StateMachineState.Acc_S1
					: StateMachineState.Acc_S2;
			CurrentState.SMState = PreviousState.SMState;
			return retVal;
		}

		public virtual IHybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity,
			bool dryRun)
		{

			if (DryRunSolution != null && DryRunSolution.DrivingAction != DataBus.DriverInfo.DrivingAction) {
				DryRunSolution = null;
			}

			if (dryRun && DryRunSolution != null && DryRunSolution.DrivingAction == DataBus.DriverInfo.DrivingAction) {
				var tmp = new HybridStrategyResponse() {
					SimulationInterval = dt,
					CombustionEngineOn = DryRunSolution.GenSet.ICEOn,
					MechanicalAssistPower = DryRunSolution.Settings,
					GenSetSpeed = DryRunSolution.GenSet.ICESpeed,
				};
				return tmp;
			}
			var gensetDeRated = DataBus.ElectricMotorInfo(PowertrainPosition.GEN).DeRatingActive;

			var maxPowerGenset = gensetDeRated
				? ApproachGensetOperatingPoint(absTime, dt, GenSetCharacteristics.MaxPowerDeRated,
					GensetState.MaximalPointDeRated)
				: ApproachGensetOperatingPoint(absTime, dt, GenSetCharacteristics.MaxPower, GensetState.MaximalPoint); //GetMaxElectricPowerGenerated(absTime, dt, TODO);

			var drivetrainDemand = GetDrivetrainPowerDemand(absTime, dt, outTorque, outAngularVelocity, maxPowerGenset);
			var emResponse = drivetrainDemand.Response.ElectricMotor;

			switch (DataBus.DriverInfo.DrivingAction) {
				case DrivingAction.Halt:
				case DrivingAction.Roll:
				case DrivingAction.Coast:
				case DrivingAction.Accelerate:
					CurrentState.SMState = GetStateAccelerate(drivetrainDemand, maxPowerGenset, dt);
					break;
				case DrivingAction.Brake:
					CurrentState.SMState = GetStateBrake(drivetrainDemand);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			GenSetOperatingPoint genSetOperatingPoint;
			var emTorque = GetMechanicalAssistPower(absTime, dt, emResponse.TorqueRequest, emResponse, emResponse.AngularVelocity /* potentially not correct! */);
			GensetState gensetState;

			switch (CurrentState.SMState) {
				case StateMachineState.Acc_S0:
					genSetOperatingPoint = GensetOff;
					gensetState = GensetState.Off;
					// update drivetrain demand if genset uses a different operating point - we are above target SoC, battery might get full
					var tmp = GensetOff;
					tmp.ElectricPower = 0.SI<Watt>();
					drivetrainDemand = GetDrivetrainPowerDemand(absTime, dt, outTorque, outAngularVelocity, tmp);
					emResponse = drivetrainDemand.Response.ElectricMotor;
					//emTorque = (-emResponse.TorqueRequest).LimitTo(emResponse.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
					//	emResponse.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
					emTorque = GetMechanicalAssistPower(absTime, dt, emResponse.TorqueRequest, emResponse, emResponse.AngularVelocity /* potentially not correct! */);
					break;
				case StateMachineState.Acc_S1:
					var optimalPoint = gensetDeRated ?
						GenSetCharacteristics.OptimalPointDeRated
						: GenSetCharacteristics.OptimalPoint;
					gensetState = gensetDeRated ?
						GensetState.OptimalPointDeRated
						: GensetState.OptimalPoint;
					if (drivetrainDemand.Response.ElectricSystem.MaxPowerDrag.IsSmallerOrEqual(0)) {
						genSetOperatingPoint = GensetOff;
						gensetState = GensetState.Off;
						break;
					}

					genSetOperatingPoint = ApproachGensetOperatingPoint(absTime, dt, optimalPoint, gensetState);
					break;
				case StateMachineState.Acc_S2:
					gensetState = gensetDeRated ?
						GensetState.MaximalPointDeRated
						: GensetState.MaximalPoint;
					if (drivetrainDemand.Response.ElectricSystem.MaxPowerDrag.IsSmallerOrEqual(0)) {
						genSetOperatingPoint = GensetOff;
						gensetState = GensetState.Off;
						break;
					}
					genSetOperatingPoint = MaxGensetPower(absTime, dt, drivetrainDemand, maxPowerGenset, gensetState);
					break;
				case StateMachineState.Acc_S3:
					gensetState = gensetDeRated ?
						GensetState.MaximalPointDeRated
						: GensetState.MaximalPoint;
					genSetOperatingPoint = MaxGensetPower(absTime, dt, drivetrainDemand, maxPowerGenset, gensetState);
					emTorque = TestPowertrain.ElectricMotor.GetTorqueForElectricPower(
						DataBus.BatteryInfo.InternalVoltage, drivetrainDemand.Response.ElectricSystem.MaxPowerDrive,
						drivetrainDemand.Response.ElectricMotor.AngularVelocity, dt, DataBus.GearboxInfo?.Gear ?? new GearshiftPosition(0), dryRun);
					if (emTorque == null) {
						emTorque = -emResponse.MaxDriveTorque;
					} else {
						// multiply with -1 here because the following method again changes the sign...
						emTorque *= -1;
					}
					emTorque = GetMechanicalAssistPower(absTime, dt, emTorque, emResponse, emResponse.AngularVelocity /* potentially not correct! */);
					break;
				case StateMachineState.Break_S0:
				case StateMachineState.Break_S1:
				case StateMachineState.Break_S2:
					if (DataBus.BatteryInfo.StateOfCharge >= StrategyParameters.TargetSoC) {
						genSetOperatingPoint = GensetOff;
						gensetState = GensetState.Off;
						break;
					}

					var optimalPointBr = gensetDeRated ?
						GenSetCharacteristics.OptimalPointDeRated
						: GenSetCharacteristics.OptimalPoint;
					gensetState = DataBus.EngineInfo.EngineOn
						? (gensetDeRated
							? GensetState.OptimalPointDeRated
							: GensetState.OptimalPoint)
						: GensetState.Off;

					genSetOperatingPoint = DataBus.EngineInfo.EngineOn ? ApproachGensetOperatingPoint(absTime, dt, optimalPointBr, gensetState) : GensetOff;

					// update drivetrain demand if genset uses a different operating point - we are above target SoC, battery might get full
					var tmpBr = GensetOff;
					tmpBr.ElectricPower = 0.SI<Watt>();
					drivetrainDemand = GetDrivetrainPowerDemand(absTime, dt, outTorque, outAngularVelocity, tmpBr);
					emResponse = drivetrainDemand.Response.ElectricMotor;

					emTorque = GetMechanicalAssistPower(absTime, dt, emResponse.TorqueRequest, emResponse, emResponse.AngularVelocity);

					if (emTorque != null && genSetOperatingPoint.ElectricPower != null && (genSetOperatingPoint.ElectricPower + drivetrainDemand.ElectricPowerDemand).IsGreater(drivetrainDemand.Response.ElectricSystem.MaxPowerDrag)) {
						// em recuperates and genset is still on, but battery cannot be charged with both (probably full) - switch off genset.
						gensetState = GensetState.Off;
						genSetOperatingPoint = GensetOff;
					}

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var setting = new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>() {
					{
						EmPosition,
						Tuple.Create(drivetrainDemand.AvgEmDrivetrainSpeed, emTorque)
					}, {
						PowertrainPosition.GEN,
						Tuple.Create(genSetOperatingPoint.AvgEmDrivetrainSpeed, genSetOperatingPoint.ICETorque)
					},
				}
				;

			DryRunSolution = new DryRunSolutionState(DataBus.DriverInfo.DrivingAction, setting, genSetOperatingPoint);



			var retVal = new HybridStrategyResponse() {
				SimulationInterval = dt,
				CombustionEngineOn = genSetOperatingPoint.ICEOn,
				MechanicalAssistPower = setting,
				GenSetSpeed = genSetOperatingPoint.ICESpeed
			};

			CurrentState.Response = retVal;
			CurrentState.GensetState = gensetState;
			return retVal;
		}

		protected abstract DrivetrainDemand GetDrivetrainPowerDemand(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, GenSetOperatingPoint maxPowerGenset);

		protected virtual NewtonMeter GetMechanicalAssistPower(Second absTime, Second dt, NewtonMeter emOutTorque, ElectricMotorResponse emResponse, PerSecond currOutAngularVelocity)
		{
			if (!DataBus.GearboxInfo.GearEngaged(absTime) && DataBus.DriverInfo.DrivingAction == DrivingAction.Roll) {
				var em = DataBus.ElectricMotorInfo(EmPosition);
				var avgSpeed = (em.ElectricMotorSpeed + currOutAngularVelocity) / 2;
				var inertiaTorqueLoss = avgSpeed.IsEqual(0)
					? 0.SI<NewtonMeter>()
					: Formulas.InertiaPower(currOutAngularVelocity, em.ElectricMotorSpeed, ModelData.ElectricMachinesData.First(x => x.Item1 == EmPosition).Item2.Inertia, dt) / avgSpeed;
				//var dragTorque = ElectricMotorData.DragCurve.Lookup()
				return (-inertiaTorqueLoss); //.LimitTo(maxDriveTorque, maxRecuperationTorque);
			}

			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Coast ||
				DataBus.DriverInfo.DrivingAction == DrivingAction.Roll) {
				return null;
			}

			if (DataBus.DriverInfo.DrivingAction != DrivingAction.Halt && DataBus.VehicleInfo.VehicleSpeed.IsSmallerOrEqual(ModelData.GearboxData?.DisengageWhenHaltingSpeed ?? Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed) && emOutTorque.IsSmaller(0)) {
				return null;
			}

			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake && emResponse.MaxRecuperationTorque == null) {
				// cannot recuperate any more...
				return null;
			}

			return (-emOutTorque).LimitTo(emResponse.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
				emResponse.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
		}

		#region Implementation of IHybridControlStrategy

		protected GenSetOperatingPoint MaxGensetPower(Second absTime, Second dt,
			DrivetrainDemand drivetrainDemand,
			GenSetOperatingPoint maxPowerGenset, GensetState gensetState)
		{
			var electricPowerDemand = drivetrainDemand.ElectricPowerDemand;

			var gensetLimit = DataBus.ElectricMotorInfo(PowertrainPosition.GEN).DeRatingActive
				? GenSetCharacteristics.MaxPowerDeRated
				: GenSetCharacteristics.MaxPower;
			if (maxPowerGenset.ElectricPower.IsSmaller(gensetLimit.ElectricPower)) {
				gensetLimit = maxPowerGenset;
			}

			return ApproachGensetOperatingPoint(absTime, dt, gensetLimit, gensetState);
			
		}

		public GenSetOperatingPoint GensetOff => new
			GenSetOperatingPoint
			{
				ICEOn = false,
				ICESpeed = ModelData.EngineData.IdleSpeed,
				ICETorque = null,
				EMTorque =  null
			};

		public GenSetOperatingPoint GensetIdle => new GenSetOperatingPoint() {
			ICEOn =  true,
			ICESpeed = ModelData.EngineData.IdleSpeed,
			ICETorque =  0.SI<NewtonMeter>(),
			EMTorque = null,
		};


		public class DrivetrainDemand
		{
			public PerSecond AvgEmDrivetrainSpeed { get; set; }
			public NewtonMeter EmTorqueDemand { get; set; }

			public Watt ElectricPowerDemand { get; set; }
			public IResponse Response { get; set; }
		}

		protected virtual StateMachineState GetStateAccelerate(DrivetrainDemand drivetrainDemand,
	GenSetOperatingPoint maxPowerGenset, Second dt)
		{
			var reqBatteryPower = maxPowerGenset.ElectricPower + drivetrainDemand.ElectricPowerDemand;
			if (DataBus.BatteryInfo.StateOfCharge.IsEqual(StrategyParameters.MinSoC, 0.01) && reqBatteryPower < 0 &&
				drivetrainDemand.ElectricPowerDemand < drivetrainDemand.Response.ElectricSystem.MaxPowerDrive) {
				return StateMachineState.Acc_S3;
			}

			var optimalGensetPoint = DataBus.ElectricMotorInfo(PowertrainPosition.GEN).DeRatingActive
				? GenSetCharacteristics.OptimalPointDeRated
				: GenSetCharacteristics.OptimalPoint;
			switch (PreviousState.SMState) {
				case StateMachineState.Acc_S0:
					if (DataBus.BatteryInfo.StateOfCharge < StrategyParameters.MinSoC) {
						return -drivetrainDemand.ElectricPowerDemand <
								optimalGensetPoint.ElectricPower
							? StateMachineState.Acc_S1
							: StateMachineState.Acc_S2;
					}

					break;
				case StateMachineState.Acc_S1:
					if (/*DataBus.BatteryInfo.StateOfCharge >= StrategyParameters.MinSoC &&*/
						DataBus.BatteryInfo.StateOfCharge < StrategyParameters.MinSoC
						&& -drivetrainDemand.ElectricPowerDemand >
						optimalGensetPoint.ElectricPower) {
						return StateMachineState.Acc_S2;
					}

					if (DataBus.BatteryInfo.StateOfCharge >= StrategyParameters.TargetSoC) {
						return StateMachineState.Acc_S0;
					}

					break;
				case StateMachineState.Acc_S2:
					if (DataBus.BatteryInfo.StateOfCharge > StrategyParameters.MinSoC) {
						return StateMachineState.Acc_S1;
					}
					if (DataBus.BatteryInfo.StateOfCharge < StrategyParameters.MinSoC &&
						-drivetrainDemand.ElectricPowerDemand < optimalGensetPoint.ElectricPower) {
						return StateMachineState.Acc_S1;
					}
					break;
				case StateMachineState.Break_S0:
					return StateMachineState.Acc_S0;
				case StateMachineState.Break_S1:
					return StateMachineState.Acc_S1;
				case StateMachineState.Break_S2:
					return StateMachineState.Acc_S2;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return PreviousState.SMState;
		}

		protected virtual StateMachineState GetStateBrake(DrivetrainDemand drivetrainDemand)
		{
			switch (PreviousState.SMState) {
				case StateMachineState.Acc_S0:
					return StateMachineState.Break_S0;
				case StateMachineState.Acc_S1:
					return StateMachineState.Break_S1;
				case StateMachineState.Acc_S2:
					return StateMachineState.Break_S2;
				case StateMachineState.Acc_S3:
					return StateMachineState.Break_S2;
				case StateMachineState.Break_S0:
					break;
				case StateMachineState.Break_S1:
					break;
				case StateMachineState.Break_S2:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return PreviousState.SMState;
		}


		protected GenSetOperatingPoint ApproachGensetOperatingPoint(Second absTime, Second dt, GenSetOperatingPoint op,
			GensetState gensetState)
		{
			TestGenSet.CombustionEngine.Initialize(
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque,
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed);
			TestGenSet.CombustionEngine.PreviousState.EngineOn = //true;
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineOn;
			TestGenSet.CombustionEngine.PreviousState.EnginePower =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EnginePower;
			TestGenSet.CombustionEngine.PreviousState.dt = (DataBus.EngineInfo as CombustionEngine).PreviousState.dt;
			TestGenSet.CombustionEngine.PreviousState.EngineSpeed =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed;
			TestGenSet.CombustionEngine.PreviousState.EngineTorque =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque;
			TestGenSet.CombustionEngine.PreviousState.EngineTorqueOut =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorqueOut;
			TestGenSet.CombustionEngine.PreviousState.DynamicFullLoadTorque =
				(DataBus.EngineInfo as CombustionEngine).PreviousState.DynamicFullLoadTorque;

			switch (TestGenSet.CombustionEngine.EngineAux) {
				case EngineAuxiliary engineAux:
					engineAux.PreviousState.AngularSpeed =
						((DataBus.EngineInfo as CombustionEngine).EngineAux as EngineAuxiliary).PreviousState
						.AngularSpeed;
					break;
				case BusAuxiliariesAdapter busAux:
					busAux.PreviousState.AngularSpeed =
						((DataBus.EngineInfo as CombustionEngine).EngineAux as BusAuxiliariesAdapter).PreviousState
						.AngularSpeed;
					if (busAux.ElectricStorage is SimpleBattery bat) {
						bat.SOC = ((DataBus.EngineInfo as CombustionEngine).EngineAux as BusAuxiliariesAdapter)
							.ElectricStorage
							.SOC;
					}

					break;
			}
			TestGenSet.ElectricMotor.ThermalBuffer =
				(DataBus.ElectricMotorInfo(PowertrainPosition.GEN) as ElectricMotor).ThermalBuffer;
			TestGenSet.ElectricMotor.DeRatingActive =
				(DataBus.ElectricMotorInfo(PowertrainPosition.GEN) as ElectricMotor).DeRatingActive;
			TestGenSet.ElectricMotor.PreviousState.DrivetrainSpeed = (DataBus.ElectricMotorInfo(PowertrainPosition.GEN) as ElectricMotor).PreviousState.DrivetrainSpeed;
			TestGenSet.ElectricMotor.PreviousState.EMSpeed = (DataBus.ElectricMotorInfo(PowertrainPosition.GEN) as ElectricMotor).PreviousState.EMSpeed;

			if (TestPowertrain.BatterySystem != null) {
				var batSystem = DataBus.BatteryInfo as BatterySystem;
				foreach (var bsKey in batSystem.Batteries.Keys) {
					for (var i = 0; i < batSystem.Batteries[bsKey].Batteries.Count; i++) {
						TestPowertrain.BatterySystem.Batteries[bsKey].Batteries[i]
							.Initialize(batSystem.Batteries[bsKey].Batteries[i].StateOfCharge);
					}
				}
				TestPowertrain.BatterySystem.PreviousState.PulseDuration =
					(DataBus.BatteryInfo as BatterySystem).PreviousState.PulseDuration;
				TestPowertrain.BatterySystem.PreviousState.PowerDemand = (DataBus.BatteryInfo as BatterySystem).PreviousState.PowerDemand;
			}

			var iceSpeed = op.ICESpeed;
			var emTqDt = op.ICETorque;
			
			TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
			var r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);

			if (op.EMTorque != null && !op.EMTorque.IsBetween(r1.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
				r1.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>())) {
				emTqDt = emTqDt.LimitTo(r1.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
					r1.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
				TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
				r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);
			}

			if (r1 is ResponseOverload || r1 is ResponseUnderload) {
				var rampUp = PreviousState.ICEOn && PreviousState.Response.GenSetSpeed.IsSmaller(op.ICESpeed) || (!PreviousState.ICEOn && DataBus.EngineInfo.EngineSpeed.IsSmaller(op.ICESpeed));
				var rampDown = PreviousState.ICEOn && PreviousState.Response.GenSetSpeed.IsGreater(op.ICESpeed);
				// increase/decrease ICE speed in case of under- or overload, keeping EM torque constant (or EM off in case ICE was off before)
				//emTqDt = DataBus.EngineInfo.EngineOn
				//	? PreviousState.Response?.MechanicalAssistPower[PowertrainPosition.GEN].Item2 == null ? null :
				//	VectoMath.Min(op.ICETorque,
				//		PreviousState.Response?.MechanicalAssistPower[PowertrainPosition.GEN].Item2)
				//	: null;
				emTqDt = DataBus.EngineInfo.EngineOn
					? op.ICETorque
					: null;

				if (emTqDt == null || !op.ICETorque.IsEqual(emTqDt)) {
					TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
					r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);
				}
				if (rampUp && r1 is ResponseOverload) {
					emTqDt = null;
					TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
					r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);
				}
				if (r1 is ResponseOverload ovl && (!op.ICESpeed.IsEqual(iceSpeed) || rampUp)) {
					(emTqDt, iceSpeed, r1) = SearchICESpeed(absTime, dt, iceSpeed, emTqDt, ovl.Delta, true);
				}

				//if (r1 is ResponseOverload ovl3 && !op.EMTorque.IsEqual(emTqDt)) {
				//	(emTqDt, iceSpeed, r1) = SearchICESpeed(absTime, dt, iceSpeed, emTqDt, ovl3.Delta, true);
				//}

				if (r1 is ResponseUnderload udl) {
					(emTqDt, iceSpeed, r1) = SearchICESpeed(absTime, dt, iceSpeed, emTqDt, udl.Delta, false);
				}

				if (r1 is ResponseOverload ovl2 && op.ICESpeed.IsEqual(iceSpeed)) {
					(emTqDt, iceSpeed, r1) = SearchEMTorque(absTime, dt, iceSpeed, r1.Engine.DynamicFullLoadTorque * 0.1, ovl2.Delta);
				}

				if (rampUp && r1 is ResponseSuccess && r1.Engine.TotalTorqueDemand.IsSmaller(r1.Engine.DynamicFullLoadTorque)) {
					TestGenSet.ElectricMotorCtl.EMTorque = r1.Engine.DynamicFullLoadTorque * 0.1;
					var tmp = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed, true) as ResponseDryRun;
					(emTqDt, iceSpeed, r1) = SearchEMTorque(absTime, dt, iceSpeed, r1.Engine.DynamicFullLoadTorque * 0.1, tmp.DeltaFullLoad);
				}

				if (r1 is ResponseSuccess && emTqDt != null && !emTqDt.IsBetween(
					r1.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
					r1.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>())) {
					emTqDt = emTqDt.LimitTo(
						r1.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
						r1.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
					TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
					r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);
				}
			}

			return new GenSetOperatingPoint() {
				ICEOn = true,
				ElectricPower = r1.ElectricSystem.ConsumerPower,
				ICETorque = emTqDt,
				ICESpeed = iceSpeed,
				AvgEmDrivetrainSpeed = r1.ElectricMotor.AvgDrivetrainSpeed,
				EMTorque = r1.ElectricMotor.TorqueRequestEmMap
			};
		}

		protected (NewtonMeter, PerSecond, IResponse) SearchICESpeed(Second absTime, Second dt, PerSecond iceSpeed,
			NewtonMeter emTqDt, Watt delta, bool searchFullLoad)
		{
			var tmpEmTqDt = emTqDt;
			var tqDt = emTqDt;
			if (delta == null) {
				TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
				var tmp = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed, true) as ResponseDryRun;
				delta = tmp.DeltaFullLoad;
			}
			iceSpeed = SearchAlgorithm.Search(iceSpeed, delta, iceSpeed * 0.01,
				getYValue: r => {
					var dryRun = r as ResponseDryRun;
					return searchFullLoad ? dryRun.DeltaFullLoad : dryRun.DeltaDragLoad;
				},
				evaluateFunction: x => {
					var tmp = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), x, true);
					tmpEmTqDt = tqDt?.LimitTo(tmp.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
						tmp.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
					TestGenSet.ElectricMotorCtl.EMTorque = tmpEmTqDt;
					return TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), x, true);
				},
				criterion: r => {
					var dryRun = r as ResponseDryRun;
					return searchFullLoad ? dryRun.DeltaFullLoad.Value() : dryRun.DeltaDragLoad.Value();
				},
				searcher: this);
			emTqDt = tmpEmTqDt;
			TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
			var r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);
			return (tqDt, iceSpeed, r1);
		}

		protected (NewtonMeter, PerSecond, IResponse) SearchEMTorque(Second absTime, Second dt, PerSecond iceSpeed, NewtonMeter emTqDt, Watt delta)
		{
			var tqDt = SearchAlgorithm.Search(emTqDt, delta, emTqDt * 0.1,
				getYValue: r => {
					var dryRun = r as ResponseDryRun;
					return dryRun.DeltaFullLoad;
				},
				evaluateFunction:
				x => {
					TestGenSet.ElectricMotorCtl.EMTorque = x;
					return TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed, true);
				},
				criterion: r => {
					var dryRun = r as ResponseDryRun;
					return dryRun.DeltaFullLoad.Value();
				},
				searcher: this);
			TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
			var r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);

			if (emTqDt != null && !emTqDt.IsBetween(r1.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
				r1.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>())) {
				emTqDt = emTqDt.LimitTo(r1.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>(),
					r1.ElectricMotor.MaxRecuperationTorque ?? 0.SI<NewtonMeter>());
				TestGenSet.ElectricMotorCtl.EMTorque = emTqDt;
				r1 = TestGenSet.ElectricMotor.Request(absTime, dt, 0.SI<NewtonMeter>(), iceSpeed);
			}

			return (tqDt, iceSpeed, r1);
		}
		
		
		public IResponse AmendResponse(IResponse response, Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			return response;
		}

		public void CommitSimulationStep(Second time, Second simulationInterval)
		{
			CurrentState.ICEOn = DataBus.EngineCtl.CombustionEngineOn;
			PreviousState = CurrentState;
			CurrentState = new StrategyState();
			
			AllowEmergencyShift = false;
		}

		public IHybridController Controller { get; set; }
		public PerSecond MinICESpeed { get; }
		public bool AllowEmergencyShift { get; set; }
		public void WriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			//throw new NotImplementedException();
		}

		public void OperatingpointChangedDuringRequest(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun, IResponse retVal)
		{
			
		}

		public void RepeatDrivingAction(Second absTime)
		{
			
		}

		#endregion

		public class StrategyState
		{
			public PerSecond AngularVelocity { get; set; }
			public HybridStrategyResponse Response { get; set; }
			//public List<HybridResultEntry> Evaluations;
			//public HybridResultEntry Solution { get; set; }

			//public bool GearboxEngaged { get; set; }

			//public Second ICEStartTStmp { get; set; }

			//public Second GearshiftTriggerTstmp { get; set; }
			//public NewtonMeter MaxGbxTq { get; set; }
			public bool ICEOn { get; set; }

			public StateMachineState SMState { get; set; }

			public GensetState GensetState { get; set; }
		}

		public class DryRunSolutionState
		{
			public DryRunSolutionState(DrivingAction drivingAction, Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> settings, GenSetOperatingPoint genSet)
			{
				DrivingAction = drivingAction;
				Settings = settings;
				GenSet = genSet;
			}


			public DrivingAction DrivingAction { get; }

			public GenSetOperatingPoint GenSet { get; }

			public Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> Settings { get; }
		}
	}

	public class GenSetCharacteristics : LoggingObject
	{
		public Dictionary<Watt, List<GenSetOperatingPoint>> OptimalPoints = new Dictionary<Watt, List<GenSetOperatingPoint>>();

		public GenSetOperatingPoint MaxPower;

		public GenSetOperatingPoint MaxPowerDeRated;
		private GenSetOperatingPoint _optimalPoint;
		private GenSetOperatingPoint _optimalPointDerated;
		private Watt MinGensetPower;

		public GenSetCharacteristics(Watt minGensetPower)
		{
			MinGensetPower = minGensetPower;
		}

		public GenSetOperatingPoint OptimalPoint
		{
			get
			{
				if (_optimalPoint == null) {
					var tmp = OptimalPoints.Values.SelectMany(x => x).Where(x => x.ElectricPower > MinGensetPower).ToArray();
					if (!tmp.Any()) {
						Log.Error($"No GenSet operating point with a power greater than {MinGensetPower} found!");
					}
					_optimalPoint = tmp.MinBy(x => x.FuelConsumption / x.ElectricPower);
				}

				return _optimalPoint;
			}
		}

		public GenSetOperatingPoint OptimalPointDeRated
		{
			get
			{
				if (_optimalPointDerated == null) {
					var tmp = OptimalPoints.Values.SelectMany(x => x).Where(x => x.EMTorque.IsSmaller(ContinuousTorque)).Where(x => x.ElectricPower > MinGensetPower).ToArray();
					if (!tmp.Any()) {
						Log.Warn($"No de-rated GenSet operating point with a power greater than {MinGensetPower} found! Ignoring min power threshold");
						tmp = OptimalPoints.Values.SelectMany(x => x)
							.Where(x => x.EMTorque.IsSmaller(ContinuousTorque)).ToArray();

					}
					
					_optimalPointDerated = tmp.MinBy(x => x.FuelConsumption / x.ElectricPower);
				}

				return _optimalPointDerated;
			}
		}

		public NewtonMeter ContinuousTorque { get; set; }
	}

	public class GenSetOperatingPoint
	{
		public bool ICEOn;

		public Watt ElectricPower;

		public PerSecond ICESpeed;

		public NewtonMeter ICETorque;

		public KilogramPerSecond FuelConsumption;

		public PerSecond AvgEmDrivetrainSpeed;

		public NewtonMeter EMTorque;
	}
}