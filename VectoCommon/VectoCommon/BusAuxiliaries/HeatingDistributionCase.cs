using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
    public enum HeatingDistributionCase
    {
        HeatingDistribution1 = 1,
        HeatingDistribution2,
        HeatingDistribution3,
        HeatingDistribution4,
        HeatingDistribution5,
        HeatingDistribution6,
        HeatingDistribution7,
        HeatingDistribution8,
        HeatingDistribution9,
        HeatingDistribution10,
        HeatingDistribution11,
        HeatingDistribution12,

        HeatingDistribution_NotAvailable_,
    }

	public static class HeatingDistributionCaseHelper
	{

		public static HeatingDistributionCase Parse(string parse)
		{
			return parse.Replace("HD", "HeatingDistribution").ParseEnum<HeatingDistributionCase>();
		}

		public static int GetID(this HeatingDistributionCase hd)
		{
			return hd.ToString().Replace("HeatingDistribution", "").ToInt();
		}
	}
}