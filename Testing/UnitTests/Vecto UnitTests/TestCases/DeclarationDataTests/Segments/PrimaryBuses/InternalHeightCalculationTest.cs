using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.Segments.PrimaryBuses;

public class InternalHeightCalculationTest
{

    [TestCase(VehicleCode.CE, RegistrationClass.I, 2.134, 2.134),
    TestCase(VehicleCode.CE, RegistrationClass.II, 2.134, 2.134),
    TestCase(VehicleCode.CE, RegistrationClass.I_II, 2.134, 2.134),
    TestCase(VehicleCode.CE, RegistrationClass.A, 2.134, 2.134),

    TestCase(VehicleCode.CF, RegistrationClass.I, 2.134, 1.8),
    TestCase(VehicleCode.CF, RegistrationClass.II, 2.134, 1.8),
    TestCase(VehicleCode.CF, RegistrationClass.I_II, 2.134, 1.8),
    TestCase(VehicleCode.CF, RegistrationClass.A, 2.134, 1.8),

    TestCase(VehicleCode.CI, RegistrationClass.I, 2.134, 2.134),
    TestCase(VehicleCode.CI, RegistrationClass.II, 2.134, 2.134),
    TestCase(VehicleCode.CI, RegistrationClass.I_II, 2.134, 2.134),
    TestCase(VehicleCode.CI, RegistrationClass.A, 2.134, 2.134),
    TestCase(VehicleCode.CI, RegistrationClass.II_III, 2.134, 2.134),
    TestCase(VehicleCode.CI, RegistrationClass.III, 2.134, 2.134),
    TestCase(VehicleCode.CI, RegistrationClass.B, 2.134, 2.134),

    TestCase(VehicleCode.CJ, RegistrationClass.I, 2.134, 1.8),
    TestCase(VehicleCode.CJ, RegistrationClass.II, 2.134, 1.8),
    TestCase(VehicleCode.CJ, RegistrationClass.I_II, 2.134, 1.8),
    TestCase(VehicleCode.CJ, RegistrationClass.A, 2.134, 1.8),
    TestCase(VehicleCode.CJ, RegistrationClass.II_III, 2.134, 1.8),
    TestCase(VehicleCode.CJ, RegistrationClass.III, 2.134, 1.8),
    TestCase(VehicleCode.CJ, RegistrationClass.B, 2.134, 1.8),

    TestCase(VehicleCode.CA, RegistrationClass.II, 2.134, 2.134 - 0.5),
    TestCase(VehicleCode.CA, RegistrationClass.II_III, 2.134, 2.134 - 0.5),
    TestCase(VehicleCode.CA, RegistrationClass.II_III, 3.134, 1.8),
    TestCase(VehicleCode.CA, RegistrationClass.III, 2.134, 1.8),
    TestCase(VehicleCode.CA, RegistrationClass.B, 2.134, 1.8),

    TestCase(VehicleCode.CB, RegistrationClass.II, 2.134, 1.8),
    TestCase(VehicleCode.CB, RegistrationClass.II_III, 2.134, 1.8),
    TestCase(VehicleCode.CB, RegistrationClass.III, 2.134, 1.8),
    TestCase(VehicleCode.CB, RegistrationClass.B, 2.134, 1.8),
    ]
    public void TestInternalHeightCalculation(
        VehicleCode vehicleCode, RegistrationClass regClass, double bodyHeight, double expectedInternalHeight)
    {
        var internalHeight =
            DeclarationData.BusAuxiliaries.CalculateInternalHeight(vehicleCode, regClass, bodyHeight.SI<Meter>());

        Assert.AreEqual(expectedInternalHeight, internalHeight.Value(), 1e-6);
    }
}