using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class VelocityRollingLookup : Interpolate2D<MeterPerSecond, Radian, MeterPerSecond,
		VelocitySpeedGearshiftPreprocessor.Entry>
	{
		public VelocitySpeedGearshiftPreprocessor.Entry[] Data
		{
			set {
				Valid = value != null && value.Length > 0;
				if (Valid) {
					SetData(value);
				}
			}
		}

		public bool Valid { get; set; }

		#region Overrides of Interpolate2D<MeterPerSecond,Radian,MeterPerSecond,Entry>

		protected override MeterPerSecond GetXValue(VelocitySpeedGearshiftPreprocessor.Entry entry)
		{
			return entry.StartVelocity;
		}

		protected override Radian GetYValue(VelocitySpeedGearshiftPreprocessor.Entry entry)
		{
			return entry.Gradient;
		}

		protected override MeterPerSecond GetZValue(VelocitySpeedGearshiftPreprocessor.Entry entry)
		{
			return entry.EndVelocity;
		}

		protected override IEnumerable<MeterPerSecond> GetXValuesSorted(VelocitySpeedGearshiftPreprocessor.Entry[] entries)
		{
			return entries.Select(x => x.StartVelocity).OrderBy(x => x);
		}

		protected override IEnumerable<Radian> GetYValuesSorted(VelocitySpeedGearshiftPreprocessor.Entry[] entries)
		{
			return entries.Select(x => x.Gradient).OrderBy(x => x);
		}

		#endregion
	}
}