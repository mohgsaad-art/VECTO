using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Driver;

public class LookAheadCoastingDataTests
{
	[TestCase]
	public void LAC_DF_Test()
	{
		var lac = new LACDecisionFactor();

        for (var vVehicle = 0.SI<MeterPerSecond>();
			vVehicle < 100.KMPHtoMeterPerSecond();
			vVehicle += 1.KMPHtoMeterPerSecond()) {
			for (var vTarget = vVehicle; vTarget > 0; vTarget -= 1.KMPHtoMeterPerSecond()) {
				var df_coast = lac.Lookup(vTarget, vVehicle - vTarget);
				if (vTarget < 48.KMPHtoMeterPerSecond()) {
					AssertHelper.AreRelativeEqual(df_coast, 2.5, $"vVehicle: {vVehicle}, vTarget: {vTarget}");
				}

				if (vVehicle - vTarget > 11) {
					AssertHelper.AreRelativeEqual(df_coast, 2.5, $"vVehicle: {vVehicle}, vTarget: {vTarget}");
				}

				if (vTarget > 52.KMPHtoMeterPerSecond() && vVehicle - vTarget < 9.KMPHtoMeterPerSecond()) {
					AssertHelper.AreRelativeEqual(df_coast, 1.0, $"vVehicle: {vVehicle}, vTarget: {vTarget}");
				}
			}
		}
	}
}