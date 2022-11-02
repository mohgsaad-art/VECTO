using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
	//public class DelegateParallelHybridStrategy : IHybridControlStrategy
	//{
	//	public Func<NewtonMeter, PerSecond, HybridStrategyResponse> InitializeFunc { get; set; }
	//	public Func<Second, Second, NewtonMeter, PerSecond, bool, HybridStrategyResponse> RequestFunc { get; set; }

	//	public Action CommitFunc { get; set; }

	//	public HybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque,
	//		PerSecond outAngularVelocity, bool dryRun)
	//	{
	//		return RequestFunc(absTime, dt, outTorque, outAngularVelocity, dryRun);
	//	}

	//	public HybridStrategyResponse Initialize(NewtonMeter outTorque,
	//		PerSecond outAngularVelocity)
	//	{
	//		return InitializeFunc(outTorque, outAngularVelocity);
	//	}

	//	public void CommitSimulationStep()
	//	{
	//		CommitFunc();
	//	}
	//}
}