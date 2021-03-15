using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
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
    public class FMUSimulinkModelHybridStrategy : IHybridControlStrategy
    {
        public class StrategyState
        {
            public PerSecond AngularVelocity { get; set; }
            public HybridStrategyResponse Response { get; set; }
            public List<HybridResultEntry> Evaluations;
            public HybridResultEntry Solution { get; set; }

            public bool GearboxEngaged;

            public Second ICEStartTStmp { get; set; }

            public Second GearshiftTriggerTstmp { get; set; }
        }

        public class DryRunSolutionState
        {
            public DryRunSolutionState(DrivingAction drivingAction, HybridResultEntry setting)
            {
                DrivingAction = drivingAction;
                Solution = setting;
            }

            public DrivingAction DrivingAction { get; set; }

            public HybridResultEntry Solution { get; set; }
        }

        protected VectoRunData ModelData;
        protected IDataBus DataBus;

        protected TestPowertrain<Gearbox> TestPowertrain;

        protected StrategyState CurrentState = new StrategyState();
        protected StrategyState PreviousState = new StrategyState();

        protected DryRunSolutionState DryRunSolution;

        protected readonly VelocityRollingLookup VelocityDropData = new VelocityRollingLookup();

        protected Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> ElectricMotorsOff;


        public FMUSimulinkModelHybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer)
        {
            DataBus = vehicleContainer;
            ModelData = runData;

            // create testcontainer
            var modData = new ModalDataContainer(runData, null, null);
            var builder = new PowertrainBuilder(modData);
            var testContainer = new SimplePowertrainContainer(runData);
            builder.BuildSimpleHybridPowertrain(runData, testContainer);

            TestPowertrain = new TestPowertrain<Gearbox>(testContainer, DataBus);
            AllowEmergencyShift = false;

            ElectricMotorsOff = ModelData.ElectricMachinesData
                                        .Select(x => new KeyValuePair<PowertrainPosition, NewtonMeter>(x.Item1, null))
                                        .ToDictionary(x => x.Key, x => new Tuple<PerSecond, NewtonMeter>(null, x.Value));
        }

        public IHybridController Controller { protected get; set; }

        public PerSecond MinICESpeed
        {
            get
            {
                return ModelData.EngineData.IdleSpeed;
            }
        }

        public bool AllowEmergencyShift { protected get;  set; }

        protected ResponseDryRun RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition nextGear, HybridStrategyResponse cfg)
        {
            TestPowertrain.Gearbox.Gear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;
            TestPowertrain.Gearbox.Disengaged = !nextGear.Engaged;
            TestPowertrain.Gearbox.DisengageGearbox = !nextGear.Engaged;
            TestPowertrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
            TestPowertrain.HybridController.ApplyStrategySettings(cfg);
            TestPowertrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
            TestPowertrain.Clutch.Initialize(DataBus.ClutchInfo.ClutchLosses);
            TestPowertrain.Battery?.Initialize(DataBus.BatteryInfo.StateOfCharge);
            TestPowertrain.SuperCap?.Initialize(DataBus.BatteryInfo.StateOfCharge);

            var currentGear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;

            if (nextGear.Engaged && !nextGear.Equals(currentGear))
            {
                if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio > ModelData.GearshiftParameters.RatioEarlyUpshiftFC)
                {
                    return null;
                }

                if (!AllowEmergencyShift && ModelData.GearboxData.Gears[nextGear.Gear].Ratio >= ModelData.GearshiftParameters.RatioEarlyDownshiftFC)
                {
                    return null;
                }

                var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
                if (!AllowEmergencyShift && !estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION))
                {
                    return null;
                }


                var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
                var vehicleSpeedPostShift = estimatedVelocityPostShift; // DataBus.VehicleInfo.VehicleSpeed - vDrop * ModelData.GearshiftParameters.VelocityDropFactor;
                TestPowertrain.Gearbox.Gear = nextGear;
                var init = TestPowertrain.Container.VehiclePort.Initialize(
                    vehicleSpeedPostShift, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
                if (!AllowEmergencyShift && init.Engine.EngineSpeed.IsSmaller(ModelData.EngineData.IdleSpeed))
                {
                    return null;
                }
            }

            if (!nextGear.Engaged)
            {
                TestPowertrain.Gearbox._nextGear = Controller.ShiftStrategy.NextGear;
                TestPowertrain.Gearbox.Disengaged = !nextGear.Engaged;
            }

            //if (!PreviousState.GearboxEngaged) {
            TestPowertrain.CombustionEngine.Initialize(
                (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque,
                (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed);
            TestPowertrain.CombustionEngine.PreviousState.EngineOn = //true;
                    (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineOn;
            TestPowertrain.CombustionEngine.PreviousState.EnginePower =
                    (DataBus.EngineInfo as CombustionEngine).PreviousState.EnginePower;
            TestPowertrain.CombustionEngine.PreviousState.dt = (DataBus.EngineInfo as CombustionEngine).PreviousState.dt;
            TestPowertrain.CombustionEngine.PreviousState.EngineSpeed =
                (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed;
            TestPowertrain.CombustionEngine.PreviousState.EngineTorque =
                (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque;
            TestPowertrain.CombustionEngine.PreviousState.EngineTorqueOut =
                (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorqueOut;
            TestPowertrain.CombustionEngine.PreviousState.DynamicFullLoadTorque =
                (DataBus.EngineInfo as CombustionEngine).PreviousState.DynamicFullLoadTorque;
            (TestPowertrain.CombustionEngine.EngineAux as EngineAuxiliary).PreviousState.AngularSpeed =
                ((DataBus.EngineInfo as CombustionEngine).EngineAux as EngineAuxiliary).PreviousState.AngularSpeed;

            TestPowertrain.Gearbox.PreviousState.InAngularVelocity =
                (DataBus.GearboxInfo as Gearbox).PreviousState.InAngularVelocity;

            TestPowertrain.Clutch.PreviousState.InAngularVelocity =
                (DataBus.ClutchInfo as SwitchableClutch).PreviousState.InAngularVelocity;

            //}

            var pos = ModelData.ElectricMachinesData.FirstOrDefault().Item1;
            TestPowertrain.ElectricMotor.ThermalBuffer =
                (DataBus.ElectricMotorInfo(pos) as ElectricMotor).ThermalBuffer;
            TestPowertrain.ElectricMotor.DeRatingActive =
                (DataBus.ElectricMotorInfo(pos) as ElectricMotor).DeRatingActive;

            if (/*nextGear != DataBus.GearboxInfo.Gear && */TestPowertrain.ElectricMotorP2 != null)
            {
                TestPowertrain.ElectricMotorP2.PreviousState.EMSpeed =
                    DataBus.ElectricMotorInfo(PowertrainPosition.HybridP2).ElectricMotorSpeed;
            }
            if (/*nextGear != DataBus.GearboxInfo.Gear && */TestPowertrain.ElectricMotorP3 != null)
            {
                TestPowertrain.ElectricMotorP3.PreviousState.EMSpeed =
                    DataBus.ElectricMotorInfo(PowertrainPosition.HybridP3).ElectricMotorSpeed;
            }

            var retVal = TestPowertrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, true);

            return retVal as ResponseDryRun;
        }

        public IHybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
        {
            var cfg = new HybridStrategyResponse()
            {
                CombustionEngineOn = true,
                GearboxInNeutral = false,
                MechanicalAssistPower = ElectricMotorsOff
            };
            var currentGear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear;
            ResponseDryRun response = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, currentGear, cfg);




            return null;
        }

        public IResponse AmendResponse(IResponse response, Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
        {
            return response;
        }

        public void CommitSimulationStep(Second time, Second simulationInterval)
        {
            throw new NotImplementedException();
        }

        public IHybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            throw new NotImplementedException();
        }

        public void OperatingpointChangedDuringRequest(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, IResponse retVal)
        {
            throw new NotImplementedException();
        }

        public void RepeatDrivingAction(Second absTime)
        {
            throw new NotImplementedException();
        }

    

        public void WriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
        {
            
        }
    }
}