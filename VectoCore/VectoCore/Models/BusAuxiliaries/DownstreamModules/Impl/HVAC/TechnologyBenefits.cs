using TUGraz.VectoCommon.BusAuxiliaries;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC {
	public class TechnologyBenefits : ISSMTechnologyBenefits
	{
		#region Implementation of ISSMTechList

		public double HValueVariation { get; internal set; }
		public double VHValueVariation { get; internal set; }
		public double VVValueVariation { get; internal set; }
		public double VCValueVariation { get; internal set; }
		public double CValueVariation { get; internal set; }

		#endregion
	}
}