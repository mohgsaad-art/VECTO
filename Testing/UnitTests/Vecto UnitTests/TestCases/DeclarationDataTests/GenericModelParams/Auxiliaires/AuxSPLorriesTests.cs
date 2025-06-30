using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.Auxiliaires;

public class AuxSPLorriesTests
{
    [
           TestCase(MissionType.LongHaul, VehicleClass.Class2, 370, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.LongHaul, VehicleClass.Class4, 610, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.LongHaul, VehicleClass.Class5, 720, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.LongHaul, VehicleClass.Class9, 720, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.LongHaul, VehicleClass.Class10, 570, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.LongHaul, VehicleClass.Class11, 720, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.LongHaul, VehicleClass.Class12, 570, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class1s, 280, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class1, 280, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 340, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class3, 370, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class4, 570, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class5, 670, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class9, 590, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class10, 570, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class11, 590, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class12, 570, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.UrbanDelivery, VehicleClass.Class1s, 270, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.UrbanDelivery, VehicleClass.Class1, 270, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.UrbanDelivery, VehicleClass.Class2, 310, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.UrbanDelivery, VehicleClass.Class3, 350, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.UrbanDelivery, VehicleClass.Class5, 620, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.MunicipalUtility, VehicleClass.Class4, 510, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.MunicipalUtility, VehicleClass.Class9, 510, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.MunicipalUtility, VehicleClass.Class11, 510, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.Construction, VehicleClass.Class11, 770, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.Construction, VehicleClass.Class12, 770, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.Construction, VehicleClass.Class16, 770, 0, "Fixed displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 325.5, 0, "Fixed displacement with elec. control", null,
               null,
               null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 289, 0, "Dual displacement", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 255, 0, "Variable displacement mech. controlled", null,
               null,
               null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 204, 0, "Variable displacement elec. controlled", null,
               null,
               null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 0, 32.87, "Full electric steering gear", null, null, null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 665, 0, "Fixed displacement", "Fixed displacement", null,
               null),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 1295, 0, "Fixed displacement", "Fixed displacement",
               "Fixed displacement", "Fixed displacement"),
           TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, 1021.5, 0,
               "Dual displacement",
               "Variable displacement mech. controlled",
               "Fixed displacement with elec. control",
               "Variable displacement elec. controlled"),

           //Electric and Mechanic
           TestCase(MissionType.UrbanDelivery,
               VehicleClass.Class5,
               852.048,
               768.72,
               "Fixed displacement",
               "Dual displacement",
               "Electric driven pump",
               "Full electric steering gear"),
           TestCase(MissionType.UrbanDelivery,
               VehicleClass.Class5,
               0,
               262.386,
               "Full electric steering gear",
               "Full electric steering gear",
               "Full electric steering gear",
               "Full electric steering gear"),
       ]
    public void Aux_SteeringPumpLookupValues(MissionType mission, VehicleClass hdvClass, double expectedMech, double expectedElectric, string axle1,
           string axle2, string axle3, string axle4)
    {
        var result = DeclarationData.SteeringPump.Lookup(mission, hdvClass,
            new[] { axle1, axle2, axle3, axle4 }.TakeWhile(a => a != null).ToArray());
        AssertHelper.AreRelativeEqual(expectedMech, result.mechanicalPumps);
        AssertHelper.AreRelativeEqual(expectedElectric, result.electricPumps);
    }

    [TestCase]
    public void Aux_SteeringpumpMultipleLookups()
    {
        // testcase to illustrate modification of lookup-data for steering pump
        const string axle1 = "Full electric steering gear";
        const MissionType mission = MissionType.LongHaul;
        const VehicleClass hdvClass = VehicleClass.Class5;
        var first = DeclarationData.SteeringPump.Lookup(mission, hdvClass,
            new[] { axle1 }.TakeWhile(a => a != null).ToArray()).mechanicalPumps;

        for (var i = 0; i < 10; i++) {
            DeclarationData.SteeringPump.Lookup(mission, hdvClass,
                new[] { axle1 }.TakeWhile(a => a != null).ToArray());
        }

        var last = DeclarationData.SteeringPump.Lookup(mission, hdvClass,
            new[] { axle1 }.TakeWhile(a => a != null).ToArray()).mechanicalPumps;

        Assert.AreEqual(first.Value(), last.Value(), 1e-3);
    }

    [TestCase(MissionType.LongHaul, VehicleClass.Class1s,
        TestName = "Aux_SteeringPumpLookupFail class 1s ( No Value )"),
    TestCase(MissionType.LongHaul, VehicleClass.Class1,
        TestName = "Aux_SteeringPumpLookupFail class 1 ( No Value )"),
    TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, "Super displacement",
        TestName = "Aux_SteeringPumpLookupFail class 2 ( Wrong Tech )"),
    TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, "Dual displacement", "Dual displacement",
        "Dual displacement", "Dual displacement", "Dual displacement", TestName = "Aux_SteeringPumpLookupFail class 2( >4 Techs )"),
    TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, TestName = "Aux_SteeringPumpLookupFail class 2( Null Techs )"),
    TestCase(MissionType.RegionalDelivery, VehicleClass.Class2, new string[0],
        TestName = "Aux_SteeringPumpLookupFail class 2 ( 0 Techs )"),
    ]
    public void Aux_SteeringPumpLookupFail(MissionType mission, VehicleClass hdvClass, params string[] tech)
    {
        AssertHelper.Exception<VectoException>(() => DeclarationData.SteeringPump.Lookup(mission, hdvClass, tech));
    }
}