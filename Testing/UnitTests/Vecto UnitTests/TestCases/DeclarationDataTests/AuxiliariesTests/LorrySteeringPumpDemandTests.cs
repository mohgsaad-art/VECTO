using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.AuxiliariesTests;

public class LorrySteeringPumpDemandTests
{
	private const MissionType LH = MissionType.LongHaul;
	private const MissionType RD = MissionType.RegionalDelivery;
	private const MissionType UD = MissionType.UrbanDelivery;
	private const MissionType MU = MissionType.MunicipalUtility;
	private const MissionType CO = MissionType.Construction;

	private const VehicleClass G5 = VehicleClass.Class5;
	private const VehicleClass G9 = VehicleClass.Class9;

    const string SP_FD = "Fixed displacement";
	const string SP_FD_EL = "Fixed displacement with elec. control";
	const string SP_DD = "Dual displacement";
	const string SP_DD_EL = "Dual displacement with elec. control";
	const string SP_VD_M = "Variable displacement mech. controlled";
	const string SP_VD_E = "Variable displacement elec. controlled";
	const string SP_EDP = "Electric driven pump";
	const string SP_FESG = "Full electric steering gear";

    [TestCase(LH, G5, 720, 0, SP_FD),
	TestCase(LH, G5, 1374, 0, SP_FD, SP_FD_EL),
	TestCase(LH, G5, 1228.5, 0, SP_FD, SP_VD_M),
	TestCase(LH, G5, 0, 17.16, SP_FESG),
	TestCase(RD, G5, 670, 0, SP_FD),
	TestCase(RD, G5, 1176.425, 0, SP_FD, SP_DD_EL),
	TestCase(RD, G5, 370.015, 340.011, SP_FD, SP_FESG),
	TestCase(RD, G5, 0, 70.03, SP_FESG),

    TestCase(LH, G9, 720, 0, SP_FD),
    TestCase(LH, G9, 1228.5, 0, SP_FD, SP_VD_M),
    TestCase(LH, G9, 368.58, 348.006, SP_FD, SP_FESG),
    TestCase(LH, G9, 0, 17.16, SP_FESG),

	TestCase(UD, G9, 518, 0, SP_FD_EL),
	TestCase(UD, G9, 1006, 0, SP_FD_EL, SP_FD_EL),
	TestCase(UD, G9, 949.25, 0, SP_FD_EL, SP_DD),
	TestCase(UD, G9, 298.3, 271.51, SP_FD_EL, SP_FESG),
	TestCase(UD, G9, 0, 78.6, SP_FESG),
	TestCase(UD, G9, 913.03, 339.4, SP_FESG, SP_DD_EL, SP_FD_EL, SP_VD_E),


    ]
    public void SteeringPumpPowerDemandTest(MissionType mission, VehicleClass vehGrp, double expPwrMech, double expPwrEl, string sp1, string sp2 = null,
		string sp3 = null, string sp4 = null)
	{
		var technologies = new[] { sp1, sp2, sp3, sp4 }.Where(x => x != null).ToList();
		var powerdemand = DeclarationData.SteeringPump.Lookup(mission, vehGrp, technologies);

		Assert.AreEqual(expPwrMech, powerdemand.mechanicalPumps.Value(), 1e-3);
		Assert.AreEqual(expPwrEl, powerdemand.electricPumps.Value(), 1e-3);
	}
}