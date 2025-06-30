using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Auxiliaires;

[TestFixture]
public class AuxESLorriesTests
{
    //Heavy Lorry
    [TestCase(VehicleClass.Class6, MissionType.LongHaul, "Standard technology", 1200, 0.7),
    TestCase(VehicleClass.Class6, MissionType.RegionalDelivery, "Standard technology", 1000, 0.7),
    TestCase(VehicleClass.Class6, MissionType.UrbanDelivery, "Standard technology", 1000, 0.7),
    TestCase(VehicleClass.Class6, MissionType.MunicipalUtility, "Standard technology", 1000, 0.7),
    TestCase(VehicleClass.Class6, MissionType.Construction, "Standard technology", 1000, 0.7),
    TestCase(VehicleClass.Class6, MissionType.LongHaul, "Standard technology - LED headlights, all", 1150, 0.7),
    TestCase(VehicleClass.Class6, MissionType.RegionalDelivery, "Standard technology - LED headlights, all", 950,
        0.7),
    TestCase(VehicleClass.Class6, MissionType.UrbanDelivery, "Standard technology - LED headlights, all", 950, 0.7),
    TestCase(VehicleClass.Class6, MissionType.MunicipalUtility, "Standard technology - LED headlights, all", 950,
        0.7),
    TestCase(VehicleClass.Class6, MissionType.Construction, "Standard technology - LED headlights, all", 950, 0.7),]

    //Medium Lorry
    [TestCase(VehicleClass.Class51, MissionType.RegionalDelivery, "Standard technology", 600, 0.7),
    TestCase(VehicleClass.Class52, MissionType.UrbanDelivery, "Standard technology", 600, 0.7),
    TestCase(VehicleClass.Class53, MissionType.RegionalDelivery, "Standard technology - LED headlights, all", 550,
        0.7),
    TestCase(VehicleClass.Class55, MissionType.UrbanDelivery, "Standard technology - LED headlights, all", 550,
        0.7)]
    [TestCase(VehicleClass.Class55, MissionType.LongHaul, "Standard technology", 720, 0.7),
    TestCase(VehicleClass.Class55, MissionType.LongHaul, "Standard technology - LED headlights, all", 660, 0.7)]
    public void AuxElectricSystemTest(VehicleClass hdvClass, MissionType mission, string technology, double value,
        double efficiency)
    {
        AssertHelper.AreRelativeEqual(value / efficiency,
            DeclarationData.ElectricSystem.Lookup(hdvClass, mission, technology).PowerDemand.Value());
    }

    //Heavy Lorry
    [TestCase(VehicleClass.Class6, MissionType.Interurban, "Standard technology"),
    TestCase(VehicleClass.Class6, MissionType.LongHaul, "Standard technology - Flux-Compensator")]
    //Medium Lorry
    //[TestCase(VehicleClass.Class55, MissionType.LongHaul, "Standard technology"),
    [TestCase(VehicleClass.Class55, MissionType.UrbanDelivery, "Standard technology - Flux-Compensator")]
    //TestCase(VehicleClass.Class55, MissionType.LongHaul, "Standard technology - LED headlights, all")]

    public void AuxElectricSystem_NotExistingError(VehicleClass hdvClass, MissionType mission, string technology)
    {
        AssertHelper.Exception<VectoException>(() => { DeclarationData.ElectricSystem.Lookup(hdvClass, mission, technology); });
    }
}