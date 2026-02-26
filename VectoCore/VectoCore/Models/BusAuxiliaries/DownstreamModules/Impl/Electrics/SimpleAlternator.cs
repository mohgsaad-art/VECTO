using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class SimpleAlternator : IAlternatorMap
	{
		protected double _efficiency;

		

		public SimpleAlternator(double efficiency)
		{
			_efficiency = efficiency;
		}

		#region Implementation of IAlternatorMap

		public double GetEfficiency(PerSecond rpm, Ampere currentDemand)
		{
			return _efficiency;
		}

		public string Source => null;

        #endregion

        public override bool Equals(object obj)
        {
			var other = obj as SimpleAlternator;
            return (other != null) && (other._efficiency == _efficiency);
        }

        public override int GetHashCode()
        {
            return _efficiency.GetHashCode();
        }

    }
}
