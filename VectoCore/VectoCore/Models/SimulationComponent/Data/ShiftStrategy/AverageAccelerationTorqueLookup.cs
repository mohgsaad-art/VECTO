using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class AverageAccelerationTorqueLookup : Interpolate2D<PerSecond, NewtonMeter, NewtonMeter, KeyValuePair<Tuple<PerSecond, NewtonMeter>, NewtonMeter>> 
	{

		public KeyValuePair<Tuple<PerSecond, NewtonMeter>, NewtonMeter>[] Data
		{
			set => SetData(value);
		}

		#region Overrides of Interpolate2D<PerSecond,NewtonMeter,NewtonMeter,KeyValuePair<Tuple<PerSecond,NewtonMeter>,NewtonMeter>>

		protected override PerSecond GetXValue(KeyValuePair<Tuple<PerSecond, NewtonMeter>, NewtonMeter> entry)
		{
			return entry.Key.Item1;
		}

		protected override NewtonMeter GetYValue(KeyValuePair<Tuple<PerSecond, NewtonMeter>, NewtonMeter> entry)
		{
			return entry.Key.Item2;
		}

		protected override NewtonMeter GetZValue(KeyValuePair<Tuple<PerSecond, NewtonMeter>, NewtonMeter> entry)
		{
			return entry.Value;
		}

		protected override IEnumerable<PerSecond> GetXValuesSorted(KeyValuePair<Tuple<PerSecond, NewtonMeter>, NewtonMeter>[] entries)
		{
			return entries.Select(x => x.Key.Item1).OrderBy(x => x);
		}

		protected override IEnumerable<NewtonMeter> GetYValuesSorted(KeyValuePair<Tuple<PerSecond, NewtonMeter>, NewtonMeter>[] entries)
		{
			return entries.Select(x => x.Key.Item2).OrderBy(x => x);
		}

		#endregion
	}
}