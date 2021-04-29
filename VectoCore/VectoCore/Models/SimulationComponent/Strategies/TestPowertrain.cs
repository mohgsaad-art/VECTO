using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies {
	public class TestPowertrain<T> where T: class, IHybridControlledGearbox
	{
		public SimplePowertrainContainer Container;
		public T Gearbox;
		
		public SimpleHybridController HybridController;
		public Battery Battery;
		public SuperCap SuperCap;
		public Clutch Clutch;
		public IBrakes Brakes;

		public IDriverInfo Driver;
		public IDrivingCycleInfo DrivingCycle;

		public StopStartCombustionEngine CombustionEngine;
		public ElectricMotor ElectricMotor;
		public ElectricMotor ElectricMotorP2;
		public ElectricMotor ElectricMotorP3;

		public TestPowertrain(SimplePowertrainContainer container, IDataBus realContainer)
		{
			Container = container;
			Gearbox = Container.GearboxCtl as T;
			
			HybridController = Container.HybridController as SimpleHybridController;
			Battery = Container.BatteryInfo as Battery;
			SuperCap = Container.BatteryInfo as SuperCap;
			Clutch = Container.ClutchInfo as Clutch;
			CombustionEngine = Container.EngineInfo as StopStartCombustionEngine;
			ElectricMotor = container.ElectricMotors.FirstOrDefault().Value as ElectricMotor;
			ElectricMotorP2 = container.ElectricMotors.ContainsKey(PowertrainPosition.HybridP2)
				? container.ElectricMotors[PowertrainPosition.HybridP2] as ElectricMotor
				: null;
			ElectricMotorP3 = container.ElectricMotors.ContainsKey(PowertrainPosition.HybridP3)
				? container.ElectricMotors[PowertrainPosition.HybridP3] as ElectricMotor
				: null;
			if (Gearbox == null) {
				throw new VectoException("Unknown gearboxtype in TestContainer: {0}", Container.GearboxCtl.GetType().FullName);
			}

			if (HybridController == null) {
				throw new VectoException("Unknown HybridController in TestContainer: {0}", Container.HybridController.GetType().FullName);
			}

			Driver = new MockDriver(container, realContainer);
			DrivingCycle = new MockDrivingCycle(container, realContainer);
				//Brakes = new MockBrakes(container);

			Brakes = container.Brakes as Brakes;
			if (Brakes == null)
            {
				throw new VectoException("error unknown missing brakes");
            }
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
        public NewtonMeter BrakeTorque { get; set; }

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

		public CycleData CycleData
		{
			get { return realContainer.DrivingCycleInfo.CycleData; }
		}

		public bool PTOActive
		{
			get { return realContainer.DrivingCycleInfo.PTOActive; }
		}

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			return realContainer.DrivingCycleInfo.CycleLookAhead(distance);
		}

		public Meter Altitude
		{
			get { return realContainer.DrivingCycleInfo.Altitude; }
		}

		public Radian RoadGradient
		{
			get { return realContainer.DrivingCycleInfo.RoadGradient; }
		}

		public MeterPerSecond TargetSpeed
		{
			get { return realContainer.DrivingCycleInfo.TargetSpeed; }
		}

		public Second StopTime
		{
			get { return realContainer.DrivingCycleInfo.StopTime; }
		}

		public Meter CycleStartDistance
		{
			get { return realContainer?.DrivingCycleInfo?.CycleStartDistance ?? 0.SI<Meter>(); }
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			return realContainer.DrivingCycleInfo.LookAhead(lookaheadDistance);
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			return realContainer.DrivingCycleInfo.LookAhead(time);
		}

		public SpeedChangeEntry LastTargetspeedChange
		{
			get { return realContainer.DrivingCycleInfo.LastTargetspeedChange; }
		}

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

		public DrivingBehavior DriverBehavior
		{
			get { return realContainer?.DriverInfo?.DriverBehavior ?? DrivingBehavior.Accelerating; }
		}

		public DrivingAction DrivingAction
		{
			get { return realContainer?.DriverInfo?.DrivingAction ?? DrivingAction.Accelerate; }
		}

		public MeterPerSquareSecond DriverAcceleration
		{
			get { return realContainer?.DriverInfo.DriverAcceleration; }
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
}