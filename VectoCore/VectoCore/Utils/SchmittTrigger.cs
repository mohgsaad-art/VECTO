

using System;

namespace TUGraz.VectoCore.Utils
{
	public class SchmittTrigger
	{
		protected int Value;

		public SchmittTrigger(double thLow, double thHigh)
		{
			ThresholdLow = thLow;
			ThresholdHigh = thHigh;
		}

		public SchmittTrigger(Tuple<double, double> thresholds)
		{
			ThresholdLow = thresholds.Item1;
			ThresholdHigh = thresholds.Item2;
		}

		public double ThresholdLow { get;  }

		public double ThresholdHigh { get;  }

		public int GetOutput(double input)
		{
			if (Value == 0 && input > ThresholdHigh) {
				Value = 1;
			}
			if (Value == 1 && input < ThresholdLow) {
				Value = 0;
			}
			return Value;
		}
	}
}
