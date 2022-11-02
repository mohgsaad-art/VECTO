using System;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {

	internal enum GearRatingCase
	{
		A = 1, // inside preferred speed range and inside torque range
		B, // inside engine speed limits, torque demand too high
		C, // valid gear, gear residience time below threshold
		D, // inside engine speed limits, outside preferred speed range
		E, // outside engine speed limits
		Z = 10 // no rating calculated
	}


	internal struct GearRating : IComparable
	{
		private const double CaseSeparationInterval = 1e5;

		public GearRating(GearRatingCase ratingCase, double rating, PerSecond maxEngineSpeed)
		{
			RatingCase = ratingCase;
			Rating = rating;
			MaxEngineSpeed = maxEngineSpeed;
		}

		public double Rating { get; }

		public GearRatingCase RatingCase { get; }
		public PerSecond MaxEngineSpeed { get;  }

		public double NumericValue => ((int)RatingCase - 1) * CaseSeparationInterval + Rating.LimitTo(0, CaseSeparationInterval-1);

		public static bool operator <(GearRating first, GearRating second)
		{
			return first.RatingCase < second.RatingCase || (first.RatingCase == second.RatingCase && first.Rating < second.Rating);
		}

		public static bool operator >(GearRating first, GearRating second)
		{
			return first.RatingCase > second.RatingCase || (first.RatingCase == second.RatingCase && first.Rating > second.Rating);
		}

		public override string ToString()
		{
			return $"{RatingCase} / {Rating} ({NumericValue})";
		}

		public int CompareTo(object obj)
		{
			var other = (GearRating)obj;
			if (other > this) {
				return -1;
			}

			if (other < this) {
				return 1;
			}

			return 0;
		}
	}
}