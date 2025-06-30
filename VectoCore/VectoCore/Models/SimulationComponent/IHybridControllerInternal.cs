using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
    public interface IHybridControllerInternal
    {
        bool ShiftRequired { get; }

        GearshiftPosition NextGear { get; }

        HybridStrategyResponse CurrentStrategySettings { get; }

        NewtonMeter MechanicalAssistPower(PowertrainPosition pos, Second absTime, Second dt,
            NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity, bool dryRun);

    }
}