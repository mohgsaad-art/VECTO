using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies 
{

	public class TestGenset
	{

		public SimplePowertrainContainer Container;
		public StopStartCombustionEngine CombustionEngine;
		public ElectricMotor ElectricMotor;
		public GensetMotorController ElectricMotorCtl;

		public Battery Battery;
		public BatterySystem BatterySystem;
		public SuperCap SuperCap;

		public TestGenset(SimplePowertrainContainer container, IDataBus realContainer)
		{
			Container = container;
			CombustionEngine = Container.EngineInfo as StopStartCombustionEngine;
			ElectricMotor = container.ElectricMotors.FirstOrDefault(x => x.Key == PowertrainPosition.GEN).Value as ElectricMotor;
			ElectricMotorCtl = ElectricMotor.Control as GensetMotorController;

			Battery = Container.BatteryInfo as Battery;
			BatterySystem = container.BatteryInfo as BatterySystem;

			SuperCap = Container.BatteryInfo as SuperCap;
		}
	}

	public class TestPowertrain<T> where T: class, IHybridControlledGearbox, IGearbox
	{
		public SimplePowertrainContainer Container;
		public T Gearbox;
		
		public SimpleHybridController HybridController;
		public Battery Battery;
		public BatterySystem BatterySystem;
		public SuperCap SuperCap;
		public Clutch Clutch;
		public IBrakes Brakes;

		public StopStartCombustionEngine CombustionEngine;
		public ElectricMotor ElectricMotor;
		public GensetChargerAdapter Charger;
		public Dictionary<PowertrainPosition, ElectricMotor> ElectricMotorsUpstreamTransmission = new Dictionary<PowertrainPosition, ElectricMotor>();
		public TorqueConverter TorqueConverter;
		public DCDCConverter DCDCConverter;
		public WHRCharger WHRCharger;

		public TestPowertrain(SimplePowertrainContainer container, IDataBus realContainer)
		{
			Container = container;
			Gearbox = Container.GearboxCtl as T;
			
			HybridController = Container.HybridController as SimpleHybridController;
			Battery = Container.BatteryInfo as Battery;
			BatterySystem = container.BatteryInfo as BatterySystem;
			
			SuperCap = Container.BatteryInfo as SuperCap;
			Clutch = Container.ClutchInfo as Clutch;
			CombustionEngine = Container.EngineInfo as StopStartCombustionEngine;
			ElectricMotor = container.ElectricMotors.FirstOrDefault().Value as ElectricMotor;
			Charger = ((ElectricMotor?.ElectricPower as ElectricSystem)?.Charger.FirstOrDefault(x => x is GensetChargerAdapter)) as GensetChargerAdapter;
			foreach (var pos in container.ElectricMotorPositions) {
				if (pos == PowertrainPosition.HybridP1 || pos == PowertrainPosition.HybridP2 || pos == PowertrainPosition.IHPC ||
					pos == PowertrainPosition.HybridP2_5 || pos == PowertrainPosition.HybridP3) {
					ElectricMotorsUpstreamTransmission[pos] = container.ElectricMotors[pos] as ElectricMotor;
				}
			}
			
			if (Gearbox != null && Gearbox.GearboxType.AutomaticTransmission() && Gearbox.GearboxType != GearboxType.APTN && Gearbox.GearboxType != GearboxType.IHPC) {
				TorqueConverter = Container.TorqueConverterInfo as TorqueConverter;
				if (TorqueConverter == null) {
					throw new VectoException("Torque converter missing for automatic transmission: {0}", Container.TorqueConverterInfo?.GetType().FullName);
				}
			}

			if (HybridController == null) {
				throw new VectoException("Unknown HybridController in TestContainer: {0}", Container.HybridController?.GetType().FullName);
			}

			var busAux = container.RunData.BusAuxiliaries;
			if (busAux != null && busAux.ElectricalUserInputsConfig.ConnectESToREESS) {
				DCDCConverter = container.DCDCConverter as DCDCConverter;
			}

			var whrCharger = container.SimulationComponents().FirstOrDefault(x => x is WHRCharger);
			if (whrCharger != null) {
				WHRCharger = whrCharger as WHRCharger;
			}
			var driver = new MockDriver(container, realContainer);
			var cycle = new MockDrivingCycle(container, realContainer);

			Brakes = container.Brakes as Brakes;
			if (Brakes == null) {
				throw new VectoException("Unknown or missing brakes in TestContainer: {0}", Container.Brakes?.GetType().FullName);
			}
			//Brakes = new MockBrakes(container);
		}
	}

	public class MockBrakes : VectoSimulationComponent, IBrakes
	{
		public MockBrakes(IVehicleContainer container) : base(container)
		{
			BrakePower = 0.SI<Watt>();
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		#endregion

		#region Implementation of IBrakes

		public Watt BrakePower { get; set; }

		#endregion
	}

	public class MockDrivingCycle : VectoSimulationComponent, IDrivingCycleInfo
	{
		private IDataBus realContainer;

		public MockDrivingCycle(VehicleContainer container, IDataBus rcontainer) : base(container)
		{
			realContainer = rcontainer;
		}

		#region Implementation of IDrivingCycleInfo

		public CycleData CycleData => realContainer.DrivingCycleInfo.CycleData;

		public bool PTOActive => realContainer.DrivingCycleInfo.PTOActive;

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			return realContainer.DrivingCycleInfo.CycleLookAhead(distance);
		}

		public Meter Altitude => realContainer.DrivingCycleInfo.Altitude;

		public Radian RoadGradient => realContainer.DrivingCycleInfo.RoadGradient;

		public MeterPerSecond TargetSpeed => realContainer.DrivingCycleInfo.TargetSpeed;

		public Second StopTime => realContainer.DrivingCycleInfo.StopTime;

		public Meter CycleStartDistance => realContainer?.DrivingCycleInfo?.CycleStartDistance ?? 0.SI<Meter>();

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			return realContainer.DrivingCycleInfo.LookAhead(lookaheadDistance);
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			return realContainer.DrivingCycleInfo.LookAhead(time);
		}

		public SpeedChangeEntry LastTargetspeedChange => realContainer.DrivingCycleInfo.LastTargetspeedChange;

		public void FinishSimulation()
		{
		}

		#endregion

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		#endregion
	}

	public class MockDriver : VectoSimulationComponent, IDriverInfo
	{
		private IDataBus realContainer;

		public MockDriver(VehicleContainer container, IDataBus rcontainer) : base(container)
		{
			realContainer = rcontainer;
		}

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior => realContainer?.DriverInfo?.DriverBehavior ?? DrivingBehavior.Accelerating;

		public DrivingAction DrivingAction => realContainer?.DriverInfo?.DrivingAction ?? DrivingAction.Accelerate;

		public MeterPerSquareSecond DriverAcceleration => realContainer?.DriverInfo.DriverAcceleration;
		public PCCStates PCCState => PCCStates.OutsideSegment;

		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();
		#endregion

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		#endregion
	}
}