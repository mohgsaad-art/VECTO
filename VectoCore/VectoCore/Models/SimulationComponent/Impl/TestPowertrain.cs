using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TestPowertrain : ITestPowertrain
    {
        public ISimpleVehicleContainer Container { get; }
        public IDataBus RealContainer;

        public ITestPowertrainVehicle Vehicle { get; }
        public ITestPowertrainTransmission Gearbox { get; }

        public ISimpleHybridController HybridController { get; }
        public IRESSInfo BatterySystem { get; }
        public IClutch Clutch { get; }
        public IBrakes Brakes { get; }

        public ITestpowertrainCombustionEngine CombustionEngine { get; }
        public IAuxPort EngineAux { get; }
        public ITestpowertrainElectricMotor ElectricMotor { get; }
        public ITestpowertrainGensetChargerAdapter Charger { get; }
        public Dictionary<PowertrainPosition, IElectricMotor> ElectricMotorsUpstreamTransmission { get; } = new Dictionary<PowertrainPosition, IElectricMotor>();
        public Dictionary<PowertrainPosition, ITestpowertrainElectricMotor> ElectricMotors { get; } = new Dictionary<PowertrainPosition, ITestpowertrainElectricMotor>();
        public ITorqueConverter TorqueConverter { get; }
        public IDCDCConverter DCDCConverter { get; }
        public IWHRCharger WHRCharger;

        public TestPowertrain(ISimpleVehicleContainer container, IDataBus realContainer, bool createDriver)
        {
            Container = container;
            RealContainer = realContainer;

            Vehicle = Container.VehicleInfo as ITestPowertrainVehicle;
            Gearbox = Container.GearboxCtl as ITestPowertrainTransmission;

            HybridController = Container.HybridController as ISimpleHybridController;
            BatterySystem = container.BatteryInfo;

            Clutch = Container.ClutchInfo as IClutch;
            CombustionEngine = Container.EngineInfo as ITestpowertrainCombustionEngine;
            EngineAux = CombustionEngine?.GetEngineAux;
            ElectricMotor = container.ElectricMotors.FirstOrDefault().Value as ITestpowertrainElectricMotor;
            Charger =
                ((ElectricMotor?.GetElectricSystem as ITestpowertrainElectricSystem)?.Charger.FirstOrDefault(x =>
                    x is ITestpowertrainGensetChargerAdapter)) as ITestpowertrainGensetChargerAdapter;
            foreach (var pos in container.ElectricMotorPositions) {
                var em = container.ElectricMotors[pos] as ITestpowertrainElectricMotor;
                if (em != null) {
                    ElectricMotors[pos] = em;
                }
                if (pos == PowertrainPosition.HybridP1 || pos == PowertrainPosition.HybridP2 || pos == PowertrainPosition.IHPC ||
                    pos == PowertrainPosition.HybridP2_5 || pos == PowertrainPosition.HybridP3) {
                    ElectricMotorsUpstreamTransmission[pos] = container.ElectricMotors[pos] as IElectricMotor;
                }
            }

            if (Gearbox != null && Gearbox.GearboxType.AutomaticTransmission() && Gearbox.GearboxType != GearboxType.APTN && Gearbox.GearboxType != GearboxType.IHPC) {
                TorqueConverter = Container.TorqueConverterInfo as ITorqueConverter;
                if (TorqueConverter == null) {
                    throw new VectoException("Torque converter missing for automatic transmission: {0}", Container.TorqueConverterInfo?.GetType().FullName);
                }
            }

            //if (HybridController == null) {
            //	throw new VectoException("Unknown HybridController in TestContainer: {0}", Container.HybridController?.GetType().FullName);
            //}

            var busAux = container.RunData.BusAuxiliaries;
            if (busAux != null && busAux.ElectricalUserInputsConfig.ConnectESToREESS) {
                DCDCConverter = container.DCDCConverter as DCDCConverter;
            }

            var whrCharger = container.SimulationComponents().FirstOrDefault(x => x is IWHRCharger);
            if (whrCharger != null) {
                WHRCharger = whrCharger as IWHRCharger;
            }

            if (createDriver) {
                var driver = new MockDriver(container, realContainer);
            }

            var cycle = new MockDrivingCycle(container, realContainer);

            Brakes = container.Brakes;
            if (Brakes == null) {
                throw new VectoException("Unknown or missing brakes in TestContainer: {0}", Container.Brakes?.GetType().FullName);
            }
            //Brakes = new MockBrakes(container);
        }

        public void UpdateComponents() => Container.UpdateComponents(RealContainer);
    }

    public class MockDrivingCycle : VectoSimulationComponent, IDrivingCycleInfo
    {
        private IDataBus realContainer;

        public MockDrivingCycle(ISimpleVehicleContainer container, IDataBus rcontainer) : base(container)
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

        protected override bool DoUpdateFrom(object other) => false;

        #endregion
    }

    public class MockDriver : VectoSimulationComponent, IDriverInfo
    {
        private IDataBus realContainer;

        public MockDriver(ISimpleVehicleContainer container, IDataBus rcontainer) : base(container)
        {
            realContainer = rcontainer;
        }

        #region Implementation of IDriverInfo

        public DrivingBehavior DriverBehavior => realContainer?.DriverInfo?.DriverBehavior ?? DrivingBehavior.Accelerating;

        public DrivingAction DrivingAction => realContainer?.DriverInfo?.DrivingAction ?? DrivingAction.Accelerate;

        public MeterPerSquareSecond DriverAcceleration => realContainer?.DriverInfo.DriverAcceleration;
        public PCCStates PCCState => PCCStates.OutsideSegment;

        public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();
        public MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed) => targetSpeed;

        #endregion

        #region Overrides of VectoSimulationComponent

        protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
        {

        }

        protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
        {

        }

        protected override bool DoUpdateFrom(object other) => false;

        #endregion
    }
}