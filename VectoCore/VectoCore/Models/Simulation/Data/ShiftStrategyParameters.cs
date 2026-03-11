using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Data {
	public class ShiftStrategyParameters : IEquatable<ShiftStrategyParameters> 
	{
		public ShiftStrategyParameters()
		{
			PEV_DownshiftMinSpeedFactor = 0.1;
			PEV_TargetSpeedBrakeNorm = 0.7;
			PEV_DeRatedDownshiftSpeedFactor = 1;
			PEV_DownshiftSpeedFactor = 1;
		}

        public override bool Equals(object obj) => Equals(obj as ShiftStrategyParameters);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(ShiftStrategyParameters other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }
			
            return TorqueReserve.IsEqual(other.TorqueReserve)
				&& (TimeBetweenGearshifts == other.TimeBetweenGearshifts)
				&& StartTorqueReserve.IsEqual(other.StartTorqueReserve)
				&& (StartSpeed == other.StartSpeed)
				&& (StartAcceleration == other.StartAcceleration)
				&& (UpshiftAfterDownshiftDelay == other.UpshiftAfterDownshiftDelay)
				&& (DownshiftAfterUpshiftDelay == other.DownshiftAfterUpshiftDelay)
				&& (UpshiftMinAcceleration == other.UpshiftMinAcceleration)
				&& RatingFactorCurrentGear.IsEqual(other.RatingFactorCurrentGear)
				&& RatioEarlyDownshiftFC.IsEqual(other.RatioEarlyDownshiftFC)
				&& RatioEarlyUpshiftFC.IsEqual(other.RatioEarlyUpshiftFC)
				&& (AllowedGearRangeFC == other.AllowedGearRangeFC)
				&& AccelerationFactor.IsEqual(other.AccelerationFactor)
				&& VelocityDropFactor.IsEqual(other.VelocityDropFactor)
				&& (GearshiftLines == other.GearshiftLines)
                && LoadstageThresholds.IsEqual(other.LoadstageThresholds, new DoubleComparer())
                && (MinEngineSpeedPostUpshift == other.MinEngineSpeedPostUpshift)
                && (ATLookAheadTime == other.ATLookAheadTime)
                && LoadStageThresoldsUp.IsEqual(other.LoadStageThresoldsUp, new DoubleComparer())
				&& LoadStageThresoldsDown.IsEqual(other.LoadStageThresoldsDown, new DoubleComparer())
                && ShiftSpeedsTCToLocked.IsEqual(other.ShiftSpeedsTCToLocked, new DoubleComparer())
                && PEV_TargetSpeedBrakeNorm.IsEqual(other.PEV_TargetSpeedBrakeNorm)
				&& PEV_DownshiftMinSpeedFactor.IsEqual(other.PEV_DownshiftMinSpeedFactor)
				&& PEV_DeRatedDownshiftSpeedFactor.IsEqual(other.PEV_DeRatedDownshiftSpeedFactor)
				&& PEV_DownshiftSpeedFactor.IsEqual(other.PEV_DownshiftSpeedFactor);
        }

        public static bool operator ==(ShiftStrategyParameters param1, ShiftStrategyParameters param2)
        {
            if (ReferenceEquals(param1, param2))
            {
                return true;
            }

            if (ReferenceEquals(param1, null) || ReferenceEquals(param2, null))
            {
                return false;
            }

            return param1.Equals(param2);
        }

        public static bool operator !=(ShiftStrategyParameters param1, ShiftStrategyParameters param2) => !(param1 == param2);

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