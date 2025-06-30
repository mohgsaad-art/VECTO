using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data {
	public class HybridStrategyParameters 
	{
		public double EquivalenceFactor { get; set; }

		public double EquivalenceFactorDischarge { get; set; }

		public double EquivalenceFactorCharge { get; set; }

		public double MinSoC { get; set; }

		public double MaxSoC { get; set; }

		public double TargetSoC { get; set; }

		public Second AuxReserveTime { get; set; }

		public Second AuxReserveChargeTime { get; set; }

		public Second MinICEOnTime { get; set; }
		
		public Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque> MaxPropulsionTorque { get; set; }

		public double ICEStartPenaltyFactor { get; set; }

		//public Watt MaxDrivetrainPower { get; set; }

		public double CostFactorSOCExponent { get; internal set; }
		
		// serial hybrid only: factor applied to the max propulsion power which the genset needs to provide in the optimal operating point
		public double GensetMinOptPowerFactor { get; set; }


		public double InitialSoc { get; set; }
	}
}