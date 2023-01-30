using System;

using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IHybridControlStrategy
	{
		IHybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity);

		IHybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun);

		IResponse AmendResponse(IResponse response, Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun);

		
		void CommitSimulationStep(Second time, Second simulationInterval);

		VelocityRollingLookup VelocityDropData { get; }

		event Action GearShiftTriggered;
		
		IHybridController Controller { set; }
		PerSecond MinICESpeed { get; }

		bool AllowEmergencyShift { set; }

		void WriteModalResults(Second time, Second simulationInterval, IModalDataContainer container);
		void OperatingpointChangedDuringRequest(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun, IResponse retVal);

		void RepeatDrivingAction(Second absTime);
	}
}