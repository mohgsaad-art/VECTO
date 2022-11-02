using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class AverageAccelerationTorquePreprocessor : ISimulationPreprocessor
	{
		protected readonly AverageAccelerationTorqueLookup AverageAccelerationTorqueLookup;
		protected readonly VectoRunData Data;
		protected readonly PerSecond UpperLimit;

		public AverageAccelerationTorquePreprocessor(AverageAccelerationTorqueLookup averageAccelerationTorqueLookup, VectoRunData data, PerSecond engineSpeedLimitHighMin)
		{
			AverageAccelerationTorqueLookup = averageAccelerationTorqueLookup;
			Data = data;
			UpperLimit = engineSpeedLimitHighMin;
		}



		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			var fld = Data.EngineData.FullLoadCurves[0];

			var numSpeedSteps = 120;
			var numTorqueSteps = 100;
			var entries = new List<KeyValuePair<Tuple<PerSecond, NewtonMeter>, NewtonMeter>>();
			var speedStepSize = (UpperLimit - Data.EngineData.IdleSpeed) / numSpeedSteps;
			var maxTorque = new Dictionary<PerSecond, NewtonMeter>();
			for (var engineSpeed = UpperLimit;
				engineSpeed >= Data.EngineData.IdleSpeed;
				engineSpeed -= speedStepSize) {
				maxTorque[engineSpeed] = fld.FullLoadStationaryTorque(engineSpeed);
			}

			for (var torque = 0.SI<NewtonMeter>(); torque < fld.MaxTorque; torque += fld.MaxTorque / numTorqueSteps) {
				var sum = 0.SI<NewtonMeter>();

				for (var engineSpeed = UpperLimit ;
					engineSpeed >= Data.EngineData.IdleSpeed;
					engineSpeed -= speedStepSize) {

					sum += VectoMath.Min(torque, maxTorque[engineSpeed]);
					var tmp = engineSpeed.IsEqual(UpperLimit, speedStepSize / 2)
						? 0.SI<NewtonMeter>()
						: sum * speedStepSize / (UpperLimit - engineSpeed);
					entries.Add(new KeyValuePair<Tuple<PerSecond, NewtonMeter>, NewtonMeter>(Tuple.Create(engineSpeed, torque), tmp));
				
				}
			}

			AverageAccelerationTorqueLookup.Data = entries.ToArray();
		}

		#endregion
	}
}