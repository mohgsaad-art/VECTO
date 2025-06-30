using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.BusAux;

public class BusAuxTests
{
    protected CompletedBusSegments segments;

    [OneTimeSetUp]
    public void RunBeforeTest()
    {
        segments = new CompletedBusSegments();
    }

    [TestCase(2, false, VehicleCode.CE, RegistrationClass.I, false, null, null, VehicleClass.Class31a, 0.0),
    TestCase(2, false, VehicleCode.CE, RegistrationClass.I, true, null, null, VehicleClass.Class31b1, 1.0),
    TestCase(2, false, VehicleCode.CE, RegistrationClass.II, true, null, null, VehicleClass.Class31b2, 1.0),
    TestCase(2, false, VehicleCode.CF, RegistrationClass.I, null, null, null, VehicleClass.Class31c, 0.0),
    TestCase(2, false, VehicleCode.CI, RegistrationClass.I, null, null, null, VehicleClass.Class31d, 0.0),
    TestCase(2, false, VehicleCode.CJ, RegistrationClass.I, null, null, null, VehicleClass.Class31e, 0.0),
    TestCase(2, false, VehicleCode.CA, RegistrationClass.II, null, null, null, VehicleClass.Class32a, 0.0),
    TestCase(2, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class32b, 0.0),
    TestCase(2, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class32c, 0.0),
    TestCase(2, false, VehicleCode.CA, RegistrationClass.III, null, null, null, VehicleClass.Class32d, 0.0),
    TestCase(2, false, VehicleCode.CB, RegistrationClass.II, null, 5, null, VehicleClass.Class32e, 0.0),
    TestCase(2, false, VehicleCode.CB, RegistrationClass.II, null, 10, null, VehicleClass.Class32f, 0.0),
    TestCase(3, false, VehicleCode.CE, RegistrationClass.I, false, null, null, VehicleClass.Class33a, 0.0),
    TestCase(3, false, VehicleCode.CE, RegistrationClass.I, true, null, null, VehicleClass.Class33b1, 1.25),
    TestCase(3, false, VehicleCode.CE, RegistrationClass.II, true, null, null, VehicleClass.Class33b2, 1.25),
    TestCase(3, false, VehicleCode.CF, RegistrationClass.I, null, null, null, VehicleClass.Class33c, 0.0),
    TestCase(3, false, VehicleCode.CI, RegistrationClass.I, null, null, null, VehicleClass.Class33d, 0.0),
    TestCase(3, false, VehicleCode.CJ, RegistrationClass.I, null, null, null, VehicleClass.Class33e, 0.0),
    TestCase(3, false, VehicleCode.CA, RegistrationClass.II, null, null, null, VehicleClass.Class34a, 0.0),
    TestCase(3, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class34b, 0.0),
    TestCase(3, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class34c, 0.0),
    TestCase(3, false, VehicleCode.CA, RegistrationClass.III, null, null, null, VehicleClass.Class34d, 0.0),
    TestCase(3, false, VehicleCode.CB, RegistrationClass.II, null, 5, null, VehicleClass.Class34e, 0.0),
    TestCase(3, false, VehicleCode.CB, RegistrationClass.II, null, 10, null, VehicleClass.Class34f, 0.0),
    TestCase(3, true, VehicleCode.CG, RegistrationClass.I, false, null, null, VehicleClass.Class35a, 0.0),
    TestCase(3, true, VehicleCode.CG, RegistrationClass.I, true, null, null, VehicleClass.Class35b1, 1.0),
    TestCase(3, true, VehicleCode.CG, RegistrationClass.II, true, null, null, VehicleClass.Class35b2, 1.0),
    TestCase(3, true, VehicleCode.CH, RegistrationClass.I, null, null, null, VehicleClass.Class35c, 0.0),
    TestCase(3, true, VehicleCode.CC, RegistrationClass.II, null, null, null, VehicleClass.Class36a, 0.0),
    TestCase(3, true, VehicleCode.CC, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class36b, 0.0),
    TestCase(3, true, VehicleCode.CC, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class36c, 0.0),
    TestCase(3, true, VehicleCode.CC, RegistrationClass.III, null, null, null, VehicleClass.Class36d, 0.0),
    TestCase(3, true, VehicleCode.CD, RegistrationClass.II, null, 5, null, VehicleClass.Class36e, 0.0),
    TestCase(3, true, VehicleCode.CD, RegistrationClass.II, null, 10, null, VehicleClass.Class36f, 0.0),
    TestCase(4, false, VehicleCode.CE, RegistrationClass.I, false, null, null, VehicleClass.Class37a, 0.0),
    TestCase(4, false, VehicleCode.CE, RegistrationClass.I, true, null, null, VehicleClass.Class37b1, 1.25),
    TestCase(4, false, VehicleCode.CE, RegistrationClass.II, true, null, null, VehicleClass.Class37b2, 1.25),
    TestCase(4, false, VehicleCode.CF, RegistrationClass.I, null, null, null, VehicleClass.Class37c, 0.0),
    TestCase(4, false, VehicleCode.CI, RegistrationClass.I, null, null, null, VehicleClass.Class37d, 0.0),
    TestCase(4, false, VehicleCode.CJ, RegistrationClass.I, null, null, null, VehicleClass.Class37e, 0.0),
    TestCase(4, false, VehicleCode.CA, RegistrationClass.II, null, null, null, VehicleClass.Class38a, 0.0),
    TestCase(4, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class38b, 0.0),
    TestCase(4, false, VehicleCode.CA, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class38c, 0.0),
    TestCase(4, false, VehicleCode.CA, RegistrationClass.III, null, null, null, VehicleClass.Class38d, 0.0),
    TestCase(4, false, VehicleCode.CB, RegistrationClass.II, null, 5, null, VehicleClass.Class38e, 0.0),
    TestCase(4, false, VehicleCode.CB, RegistrationClass.II, null, 10, null, VehicleClass.Class38f, 0.0),
    TestCase(4, true, VehicleCode.CG, RegistrationClass.I, false, null, null, VehicleClass.Class39a, 0.0),
    TestCase(4, true, VehicleCode.CG, RegistrationClass.I, true, null, null, VehicleClass.Class39b1, 1.25),
    TestCase(4, true, VehicleCode.CG, RegistrationClass.II, true, null, null, VehicleClass.Class39b2, 1.25),
    TestCase(4, true, VehicleCode.CH, RegistrationClass.I, null, null, null, VehicleClass.Class39c, 0.0),
    TestCase(4, true, VehicleCode.CC, RegistrationClass.II, null, null, null, VehicleClass.Class40a, 0.0),
    TestCase(4, true, VehicleCode.CC, RegistrationClass.II_III, null, null, 3.0, VehicleClass.Class40b, 0.0),
    TestCase(4, true, VehicleCode.CC, RegistrationClass.II_III, null, null, 3.2, VehicleClass.Class40c, 0.0),
    TestCase(4, true, VehicleCode.CC, RegistrationClass.III, null, null, null, VehicleClass.Class40d, 0.0),
    TestCase(4, true, VehicleCode.CD, RegistrationClass.II, null, 5, null, VehicleClass.Class40e, 0.0),
    TestCase(4, true, VehicleCode.CD, RegistrationClass.II, null, 10, null, VehicleClass.Class40f, 0.0),
	]
    public void TestDrivetrainCorrectionLengthDrivetrain(int numAxles, bool articulated, VehicleCode vc, RegistrationClass regCode, bool? lowEntry, int? passCntLow, double? height,
        VehicleClass expecteClass, double expectedLength)
    {
        var semgent = segments.Lookup(numAxles, vc, regCode, passCntLow, height?.SI<Meter>(), lowEntry);
        NUnit.Framework.Assert.AreEqual(semgent.VehicleClass, expecteClass);

        var len = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(vc, lowEntry, numAxles, articulated);
        NUnit.Framework.Assert.AreEqual(expectedLength, len.Value());
    }
}