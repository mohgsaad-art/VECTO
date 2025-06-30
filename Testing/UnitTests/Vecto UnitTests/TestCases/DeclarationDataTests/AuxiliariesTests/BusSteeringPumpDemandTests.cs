using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.AuxiliariesTests;

public class BusSteeringPumpDemandTests
{
	const string SP_FD = "Fixed displacement";
	const string SP_FD_EL = "Fixed displacement with elec. control";
	const string SP_DD = "Dual displacement";
	const string SP_DD_EL = "Dual displacement with elec. control";
	const string SP_VD_M = "Variable displacement mech. controlled";
	const string SP_VD_E = "Variable displacement elec. controlled";
	const string SP_EDP = "Electric driven pump";
	const string SP_FESG = "Full electric steering gear";

	private const MissionType SU = MissionType.Suburban;
	private const MissionType U = MissionType.Urban;
	private const MissionType HU = MissionType.HeavyUrban;
	private const MissionType CO = MissionType.Coach;
	private const MissionType IU = MissionType.Interurban;

    [TestCase(HU, 1084.544, 0, 12, SP_FD_EL),
	TestCase(HU, 1424.544, 0, 12, SP_FD_EL, SP_DD),
	TestCase(HU, 1084.544, 100, 12, SP_FD_EL, SP_EDP),
	TestCase(HU, 0, 374.880, 12, SP_EDP, SP_EDP),
	TestCase(HU, 0, 274.880, 12, SP_EDP),
	
	TestCase(HU, 0, 254.080, 9, SP_EDP),
	TestCase(HU, 1405.504, 0, 9, SP_FD_EL, SP_FD_EL),

	TestCase(SU, 1305.504, 0, 9, SP_FD_EL, SP_FD_EL),
	TestCase(SU, 1245.504, 0, 9, SP_FD_EL, SP_DD_EL),
	TestCase(SU, 905.504, 40, 9, SP_FD_EL, SP_FESG),
	TestCase(SU, 0, 70, 9, SP_FESG),
	TestCase(SU, 0, 110, 9, SP_FESG, SP_FESG),
    ]
	public void SteeringPumpPowerDemandTest(MissionType mission, double expPwrMech, double expPwrEl, double length, string sp1, string sp2 = null,
		string sp3 = null, string sp4 = null)
	{
		var technologies = new[] { sp1, sp2, sp3, sp4 }.Where(x => x != null).ToList();
		var powerdemandMech = DeclarationData.SteeringPumpBus.LookupMechanicalPowerDemand(mission, technologies, length.SI<Meter>());
		var powerdemandEl = DeclarationData.SteeringPumpBus.LookupElectricalPowerDemand(mission, technologies, length.SI<Meter>());

        Assert.AreEqual(expPwrMech, powerdemandMech.Value(), 1e-3);
		Assert.AreEqual(expPwrEl, powerdemandEl.Value(), 1e-3);
	}
}