using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;

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

		public MeterPerSecond StartVelocity { get; internal set; }

		//public MeterPerSquareSecond StartAcceleration { get; internal set; }

		public Second GearResidenceTime { get; internal set; }


		public IPredictionDurationLookup PredictionDurationLookup { get; internal set; }
		public IShareTorque99lLookup ShareTorque99L { get; internal set; }

		public IShareIdleLowLookup ShareIdleLow { get; internal set; }

		public IEngineSpeedHighFactorLookup ShareEngineHigh { get; internal set; }

		public IAccelerationReserveLookup AccelerationReserveLookup { get; internal set; }

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

		//% Max.acceptable engine speed for current gear
		//% Low limit, if demanded cardan torque for /constant/ velocity is not above
		//% the max.available cardan torque
		//% Min.ratio of distance between
		//% min.speed where 99 % of the max.engine torque are reached, and
		//% max.speed where 99 % of the max.engine power are reached.
		public double DnT99L_highMin1 { get; internal set; }

		//% Max.acceptable engine speed for current gear
		//% Low limit, if demanded cardan torque for /constant/ velocity is not above
		//% the max.available cardan torque
		//% Max.ratio of distance between
		//% min.speed where 99 % of the max.engine torque are reached, and
		//% max.speed where 99 % of the max.engine power are reached.
		public double DnT99L_highMin2 { get; internal set; }

		public double EngineSpeedHighDriveOffFactor { get; set; }
		public int AllowedGearRangeDown { get; set; }
		public int AllowedGearRangeUp { get; set; }
		public Second LookBackInterval { get; set; }
		public Second DriverAccelerationLookBackInterval { get; set; }
		public Watt AverageCardanPowerThresholdPropulsion { get; set; }
		public Watt CurrentCardanPowerThresholdPropulsion { get; set; }
		public double TargetSpeedDeviationFactor { get; set; }
		public double RatingFactorCurrentGear { get; set; }
		public MeterPerSquareSecond DriverAccelerationThresholdLow { get; set; }
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