using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Data {
	public class ShiftStrategyParameters
	{

		public MeterPerSecond StartVelocity { get; internal set; }

		//public MeterPerSquareSecond StartAcceleration { get; internal set; }

		//public Second GearResidenceTime { get; internal set; }


		//public IPredictionDurationLookup PredictionDurationLookup { get; internal set; }
		//public IShareTorque99lLookup ShareTorque99L { get; internal set; }

		//public IShareIdleLowLookup ShareIdleLow { get; internal set; }

		//public IEngineSpeedHighFactorLookup ShareEngineHigh { get; internal set; }

		//public IAccelerationReserveLookup AccelerationReserveLookup { get; internal set; }

		[Required, Range(0, 0.5)]
		public double TorqueReserve { get; internal set; } 

		/// <summary>
		/// Gets the minimum time between shifts.
		/// </summary>
		[Required, SIRange(0, 5)]
		public Second TimeBetweenGearshifts { get; internal set; } 

		/// <summary>
		/// [%] (0-1) The starting torque reserve for finding the starting gear after standstill.
		/// </summary>
		[Required, Range(0, 0.5)]
		public double StartTorqueReserve { get; internal set; }

		// MQ: TODO: move to Driver Data ?
		[Required, SIRange(double.Epsilon, 5)]
		public MeterPerSecond StartSpeed { get; internal set; } 

		// MQ: TODO: move to Driver Data ?
		[Required, SIRange(double.Epsilon, 2)]
		public MeterPerSquareSecond StartAcceleration { get; internal set; } 

		[Required, SIRange(0, double.MaxValue)]
		public Second UpshiftAfterDownshiftDelay { get; internal set; }

		[Required, SIRange(0, double.MaxValue)]
		public Second DownshiftAfterUpshiftDelay { get; internal set; } 

		[Required, SIRange(0, double.MaxValue)]
		public MeterPerSquareSecond UpshiftMinAcceleration { get; internal set; }

		public double RatingFactorCurrentGear { get; set; }
		
		public double RatioEarlyDownshiftFC { get; set; }
		public double RatioEarlyUpshiftFC { get; set; }

		public int AllowedGearRangeFC { get; set; }

		public double AccelerationFactor { get; set; }

		public double VelocityDropFactor { get; internal set; }

		public PerSecond MinEngineSpeedPostUpshift { get; set; }
		public Second ATLookAheadTime { get; set; }
		public double[] LoadStageThresoldsUp { get; set; }
		public double[] LoadStageThresoldsDown { get; set; }
		public double[][] ShiftSpeedsTCToLocked { get; set; }
		
	}
}