using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

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
	}
}
