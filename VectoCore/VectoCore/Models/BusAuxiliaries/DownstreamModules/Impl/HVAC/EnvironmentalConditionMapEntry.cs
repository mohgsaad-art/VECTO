using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class EnvironmentalConditionMapEntry : IEnvironmentalConditionsMapEntry
	{
		
		public EnvironmentalConditionMapEntry(Kelvin temperature, WattPerSquareMeter solar, double weight)
		{
			Temperature = temperature;
			Solar = solar;
			Weighting = weight;
		}

		public Kelvin Temperature { get; }

		public WattPerSquareMeter Solar { get; }

		public double Weighting { get; internal set; }

		public override bool Equals(object other)
		{
			var myOther = other as EnvironmentalConditionMapEntry;
			if (myOther == null) {
				return false;
			}

			return Temperature.IsEqual(myOther.Temperature) && Solar.IsEqual(myOther.Solar) &&
					Weighting.IsEqual(myOther.Weighting);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
