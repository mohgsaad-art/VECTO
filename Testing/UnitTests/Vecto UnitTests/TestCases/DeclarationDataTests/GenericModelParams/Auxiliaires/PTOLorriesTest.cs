using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Auxiliaires;

[TestFixture]
public class PTOLorriesTest
{
	[TestCase("only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel", 50),
	TestCase("only the drive shaft of the PTO - multi-disc clutch", 350),
	TestCase("only the drive shaft of the PTO - multi-disc clutch, oil pump", 3000),
	TestCase("drive shaft and/or up to 2 gear wheels - shift claw, synchronizer, sliding gearwheel", 150),
	TestCase("drive shaft and/or up to 2 gear wheels - multi-disc clutch", 400),
	TestCase("drive shaft and/or up to 2 gear wheels - multi-disc clutch, oil pump", 3050),
	TestCase("drive shaft and/or more than 2 gear wheels - shift claw, synchronizer, sliding gearwheel", 200),
	TestCase("drive shaft and/or more than 2 gear wheels - multi-disc clutch", 450),
	TestCase("drive shaft and/or more than 2 gear wheels - multi-disc clutch, oil pump", 3100),
	TestCase("PTO which includes 1 or more additional gearmesh(es), without disconnect clutch", 1500),
	TestCase("only one engaged gearwheel above oil level", 0)]
	public void AuxPTOTransmissionTest(string technology, double value)
	{
		AssertHelper.AreRelativeEqual(value, DeclarationData.PTOTransmission.Lookup(technology).PowerDemand.Value());
	}

	[TestCase("Superfluid")]
	public void AuxPTOTransmission_NotExistingError(string technology)
	{
		AssertHelper.Exception<VectoException>(() => { DeclarationData.PTOTransmission.Lookup(technology); });
	}
}