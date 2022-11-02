namespace TUGraz.VectoCommon.BusAuxiliaries
{
	public interface ISSMTechnologyBenefits
	{
		double HValueVariation { get; }
		double VHValueVariation { get; }
		double VVValueVariation { get; }
		double VCValueVariation { get; }
		double CValueVariation { get; }

	}
}
