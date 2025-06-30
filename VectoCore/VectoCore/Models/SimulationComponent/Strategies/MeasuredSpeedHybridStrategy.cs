using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
    public class MeasuredSpeedHybridStrategy : HybridStrategy
    {
        public MeasuredSpeedHybridStrategy(VectoRunData runData, IVehicleContainer container) : base(runData, container) {}

        protected override List<HybridResultEntry> FindSolution(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
        {
            // Change gear if all responses return an implausible engine operating point.

            var responses = base.FindSolution(absTime, dt, outTorque, outAngularVelocity, dryRun);

            bool engineSpeedTooHigh = responses.All(x => 
                ((x.IgnoreReason & HybridConfigurationIgnoreReason.EngineSpeedTooHigh) != 0) &&
                ((x.IgnoreReason & HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh) != 0));

            bool engineSpeedTooLow = responses.All(x => 
                ((x.IgnoreReason & HybridConfigurationIgnoreReason.EngineSpeedTooLow) != 0) &&
                ((x.IgnoreReason & HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow) != 0));

            if (engineSpeedTooHigh || engineSpeedTooLow) {
                var duringTractionInterruption = (PreviousState.GearshiftTriggerTstmp + ModelData.GearboxData.TractionInterruption).IsGreaterOrEqual(absTime, ModelData.GearboxData.TractionInterruption / 20);
			    var allowICEOff = AllowICEOff(absTime) && (!DataBus.EngineInfo.EngineOn || !duringTractionInterruption);

			    var emPos = ModelData.ElectricMachinesData.First().Item1;

                var gear = PreviousState.GearboxEngaged ? CurrentGear : NextGear;
                var nextGear = engineSpeedTooLow ? GearList.Predecessor(gear, 1) : GearList.Successor(gear, 1);
			   
                EvaluateConfigsForGear(absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, responses, emPos, dryRun);
            }
            
            return responses; 
        }
    }

    public class MeasuredSpeedHybridStrategyAT : HybridStrategyAT 
    { 
        public MeasuredSpeedHybridStrategyAT(VectoRunData runData, IVehicleContainer container) : base(runData, container)
		{ }

        protected override List<HybridResultEntry> FindSolution(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
        {
            // Change gear if all responses return an implausible engine operating point.

            var responses = base.FindSolution(absTime, dt, outTorque, outAngularVelocity, dryRun);

            bool engineSpeedTooHigh = responses.All(x => 
                ((x.IgnoreReason & HybridConfigurationIgnoreReason.EngineSpeedTooHigh) != 0) &&
                ((x.IgnoreReason & HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh) != 0));

            bool engineSpeedTooLow = responses.All(x => 
                ((x.IgnoreReason & HybridConfigurationIgnoreReason.EngineSpeedTooLow) != 0) &&
                ((x.IgnoreReason & HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow) != 0));

            if (engineSpeedTooHigh || engineSpeedTooLow) {
                var duringTractionInterruption = (PreviousState.GearshiftTriggerTstmp + ModelData.GearboxData.TractionInterruption).IsGreaterOrEqual(absTime, ModelData.GearboxData.TractionInterruption / 20);
			    var allowICEOff = AllowICEOff(absTime) && (!DataBus.EngineInfo.EngineOn || !duringTractionInterruption);

			    var emPos = ModelData.ElectricMachinesData.First().Item1;

                var gear = PreviousState.GearboxEngaged ? CurrentGear : NextGear;
                var nextGear = engineSpeedTooLow ? GearList.Predecessor(gear, 1) : GearList.Successor(gear, 1);
			   
                EvaluateConfigsForGear(absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, responses, emPos, dryRun);
            }
            
            return responses;
        }
    }
}
