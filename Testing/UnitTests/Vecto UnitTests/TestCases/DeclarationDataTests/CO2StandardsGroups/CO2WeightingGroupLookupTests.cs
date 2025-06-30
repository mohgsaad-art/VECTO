using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.CO2StandardsGroups;

public class CO2WeightingGroupLookupTests
{
    [
       TestCase(VehicleClass.Class1, true, 169.9, WeightingGroup.Group1),
       TestCase(VehicleClass.Class1, false, 265, WeightingGroup.Group1),
       TestCase(VehicleClass.Class2, true, 169.9, WeightingGroup.Group2),
       TestCase(VehicleClass.Class2, false, 265, WeightingGroup.Group2),
       TestCase(VehicleClass.Class3, true, 169.9, WeightingGroup.Group3),
       TestCase(VehicleClass.Class3, false, 265, WeightingGroup.Group3),


       TestCase(VehicleClass.Class4, true, 169.9, WeightingGroup.Group4UD),
       TestCase(VehicleClass.Class4, false, 169.9, WeightingGroup.Group4UD),
       TestCase(VehicleClass.Class4, false, 170, WeightingGroup.Group4RD),
       TestCase(VehicleClass.Class4, true, 170, WeightingGroup.Group4RD),
       TestCase(VehicleClass.Class4, true, 264.9, WeightingGroup.Group4RD),
       TestCase(VehicleClass.Class4, true, 265, WeightingGroup.Group4LH),

       TestCase(VehicleClass.Class5, false, 169.9, WeightingGroup.Group5RD),
       TestCase(VehicleClass.Class5, false, 170, WeightingGroup.Group5RD),
       TestCase(VehicleClass.Class5, false, 264.9, WeightingGroup.Group5RD),
       TestCase(VehicleClass.Class5, false, 265, WeightingGroup.Group5RD),
       TestCase(VehicleClass.Class5, true, 264.9, WeightingGroup.Group5RD),
       TestCase(VehicleClass.Class5, true, 265, WeightingGroup.Group5LH),

       TestCase(VehicleClass.Class9, false, 169.9, WeightingGroup.Group9RD),
       TestCase(VehicleClass.Class9, false, 264.9, WeightingGroup.Group9RD),
       TestCase(VehicleClass.Class9, false, 265, WeightingGroup.Group9RD),
       TestCase(VehicleClass.Class9, true, 169.9, WeightingGroup.Group9LH),
       TestCase(VehicleClass.Class9, true, 264.9, WeightingGroup.Group9LH),
       TestCase(VehicleClass.Class9, true, 265, WeightingGroup.Group9LH),

       TestCase(VehicleClass.Class10, false, 169.9, WeightingGroup.Group10RD),
       TestCase(VehicleClass.Class10, false, 264.9, WeightingGroup.Group10RD),
       TestCase(VehicleClass.Class10, false, 265, WeightingGroup.Group10RD),
       TestCase(VehicleClass.Class10, true, 169.9, WeightingGroup.Group10LH),
       TestCase(VehicleClass.Class10, true, 264.9, WeightingGroup.Group10LH),
       TestCase(VehicleClass.Class10, true, 265, WeightingGroup.Group10LH),

       TestCase(VehicleClass.Class11, true, 169.9, WeightingGroup.Group11),
       TestCase(VehicleClass.Class11, false, 265, WeightingGroup.Group11),
       TestCase(VehicleClass.Class12, true, 169.9, WeightingGroup.Group12),
       TestCase(VehicleClass.Class12, false, 265, WeightingGroup.Group12),
       TestCase(VehicleClass.Class16, true, 169.9, WeightingGroup.Group16),
       TestCase(VehicleClass.Class16, false, 265, WeightingGroup.Group16),
           ]
    public void TestWeightingGroupLookup(
           VehicleClass vehicleGroup, bool sleeperCab, double ratedPowerkWm, WeightingGroup expectedWeightingGroup)
    {
        var wGroup = DeclarationData.WeightingGroup.Lookup(
            vehicleGroup, sleeperCab, ratedPowerkWm.SI(Unit.SI.Kilo.Watt).Cast<Watt>());
        Assert.AreEqual(expectedWeightingGroup, wGroup);
    }
}