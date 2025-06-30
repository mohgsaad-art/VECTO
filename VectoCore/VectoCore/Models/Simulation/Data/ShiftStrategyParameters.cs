using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Data {
	public class ShiftStrategyParameters
	{
		public ShiftStrategyParameters()
		{
			PEV_DownshiftMinSpeedFactor = 0.1;
			PEV_TargetSpeedBrakeNorm = 0.7;
			PEV_DeRatedDownshiftSpeedFactor = 1;
			PEV_DownshiftSpeedFactor = 1;
		}

		
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

		// Shift Lines for Voith proposed shift strategy
		public TableData GearshiftLines { get; set; }

		public IEnumerable<Tuple<double, double>> LoadstageThresholds { get; set; }

		public PerSecond MinEngineSpeedPostUpshift { get; set; }
		public Second ATLookAheadTime { get; set; }
		public double[] LoadStageThresoldsUp { get; set; }
		public double[] LoadStageThresoldsDown { get; set; }
		public double[][] ShiftSpeedsTCToLocked { get; set; }
		public double PEV_TargetSpeedBrakeNorm { get; set; }

		public double PEV_DownshiftSpeedFactor { get; set; }
		public double PEV_DeRatedDownshiftSpeedFactor { get; set; }
		public double PEV_DownshiftMinSpeedFactor { get; set; }
	}
}