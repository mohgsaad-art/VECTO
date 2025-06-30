using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration.Components;

public class AirdragDataAdapterTests
{
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7450, 5000, VehicleClass.Class1s, 3.6, null, 7.1, 7.1)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 9000, 5000, VehicleClass.Class1, 3.6, null, 7.1, 7.1)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 5000, VehicleClass.Class2, 3.75, null, 8.5, 7.2, 7.2)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 14000, 5000, VehicleClass.Class3, 3.9, null, 7.4, 7.4)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 17000, 5000, VehicleClass.Class4, 4, null, 9.9, 8.4, 8.4, 8.4, 6.6)]
    [TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 17000, 5000, VehicleClass.Class5, 4, null, 8.7, 10.2, 8.7, 10.2, 8.7, 7.2)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 17000, 5000, VehicleClass.Class9, 4, null, 10, 10.6, 8.5, 10.6, 8.5, 6.7)]
    [TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 12000, 5000, VehicleClass.Class10, 4, null, 8.8, 10.3, 8.8, 10.3, 7.3)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 12000, 5000, VehicleClass.Class11, 4, null, 10, 10.6, 8.5, 10.6, 8.5, 6.7)]
    [TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 12000, 5000, VehicleClass.Class12, 4, null, 8.8, 10.3, 8.8, 10.3, 7.3)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 12000, 5000, VehicleClass.Class16, 3.6, null, 10.5, 11.1, 9, 11.1, 7)]

    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 6000, 5000, VehicleClass.Class53, 3.5, null, 5.8, 5.8)]
    [TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 6000, 5000, VehicleClass.Class54, 2.9, null, 2.5, 2.5)]

    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7450, 5000, VehicleClass.Class1s, 3.6, 5.123, 5.123, 5.123)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 9000, 5000, VehicleClass.Class1, 3.6, 5.123, 5.123, 5.123)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 5000, VehicleClass.Class2, 3.75, 5.123, 6.423, 5.123, 5.123)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 14000, 5000, VehicleClass.Class3, 3.9, 5.123, 5.123, 5.123)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 17000, 5000, VehicleClass.Class4, 4, 5.123, 6.623, 5.123, 5.123, 5.123, 6.6)]
    [TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 17000, 5000, VehicleClass.Class5, 4, 5.123, 5.123, 6.623, 5.123, 6.623, 5.123, 7.2)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 17000, 5000, VehicleClass.Class9, 4, 5.123, 6.623, 7.223, 5.123, 7.223, 5.123, 6.7)]
    [TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 12000, 5000, VehicleClass.Class10, 4, 5.123, 5.123, 6.623, 5.123, 6.623, 7.3)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 12000, 5000, VehicleClass.Class11, 4, 5.123, 6.623, 7.223, 5.123, 7.223, 5.123, 6.7)]
    [TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 12000, 5000, VehicleClass.Class12, 4, 5.123, 5.123, 6.623, 5.123, 6.623, 7.3)]
    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 12000, 5000, VehicleClass.Class16, 3.6, 5.123, 6.623, 7.223, 5.123, 7.223, 7)]

    [TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 6000, 5000, VehicleClass.Class53, 3.5, 5.123, 5.123, 5.123)]
    [TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 6000, 5000, VehicleClass.Class54, 2.9, 5.123, 5.123, 5.123)]

    public void TestHeightAndAirdragArea_Lorry(VehicleCategory category, AxleConfiguration axleConfiguration,
        double grossVehicleMassRating, double curbWeight, VehicleClass expectedVehicleClass, double expectedHeight, double? cdxA, params double[] expectedCdxA)
    {
        var segment =
            DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossVehicleMassRating.SI<Kilogram>(), curbWeight.SI<Kilogram>(), false);
        //var mission = segment.Missions.First();

        Assert.AreEqual(expectedVehicleClass, segment.VehicleClass);

        var heights = new List<Meter>();
        var areas = new List<SquareMeter>();
        var realDao = new AirdragDataAdapter();
        var dao = new Mock<AirdragDataAdapter>() { CallBase = true };
        dao.Setup(a =>
            a.GetDeclarationAirResistanceCurve(It.IsAny<string>(), It.IsAny<SquareMeter>(), It.IsAny<Meter>())).Returns(
            (string cw, SquareMeter cdxA, Meter h) => {
                heights.Add(h);
                areas.Add(cdxA);
                return realDao.GetDeclarationAirResistanceCurve(cw, cdxA, h);
            });

        var imcData = new Mock<IVehicleInMotionChargingDeclaration>();
        imcData.Setup(i => i.Technology).Returns(IMCTechnology.GroundRail);

        var vehicle = new Mock<IVehicleDeclarationInputData>();
        vehicle.Setup(v => v.InMotionCharging).Returns(imcData.Object);
        vehicle.Setup(v => v.Components).Returns(new Mock<IVehicleComponentsDeclaration>().Object);
        if (cdxA.HasValue) {
            vehicle.Setup(v => v.Components.AirdragInputData.AirDragArea).Returns(cdxA.Value.SI<SquareMeter>());
        }

        var data = segment.Missions.Select(m => dao.Object.CreateAirdragData(vehicle.Object, m, segment, OvcHevMode.NotApplicable)).ToArray();

        Console.WriteLine(heights.Select(x => x.Value()).Join(", "));
        Console.WriteLine(areas.Select(x => x.Value()).Join(", "));

        Assert.IsTrue(heights.All(x => x.IsEqual(expectedHeight)));
        CollectionAssert.AreEqual(expectedCdxA.Select(x => $"{x:F5}"), areas.Select(a => $"{a.Value():F5}"));
    }

    [TestCase(AxleConfiguration.AxleConfig_4x2, false, VehicleClass.ClassP31SD, VehicleClass.ClassP31_32, 2.8 + 0.3, 4.9, 4.9, 4.9, 4.9)]
    [TestCase(AxleConfiguration.AxleConfig_4x2, false, VehicleClass.ClassP31DD, VehicleClass.ClassP31_32, 3.8 + 0.3, 6.2, 6.2, 6.2)]
    [TestCase(AxleConfiguration.AxleConfig_4x2, false, VehicleClass.ClassP32SD, VehicleClass.ClassP31_32, 3.15 + 0.3, 3.45, 3.45)]
    [TestCase(AxleConfiguration.AxleConfig_4x2, false, VehicleClass.ClassP32DD, VehicleClass.ClassP31_32, 3.7 + 0.3, 3.9, 3.9)]

    [TestCase(AxleConfiguration.AxleConfig_6x2, false, VehicleClass.ClassP33SD, VehicleClass.ClassP33_34, 2.8 + 0.3, 5, 5, 5, 5)]
    [TestCase(AxleConfiguration.AxleConfig_6x2, false, VehicleClass.ClassP33DD, VehicleClass.ClassP33_34, 3.8 + 0.3, 6.3, 6.3, 6.3)]
    [TestCase(AxleConfiguration.AxleConfig_6x2, false, VehicleClass.ClassP34SD, VehicleClass.ClassP33_34, 3.15 + 0.3, 3.53, 3.53)]
    [TestCase(AxleConfiguration.AxleConfig_6x2, false, VehicleClass.ClassP34DD, VehicleClass.ClassP33_34, 3.7 + 0.3, 3.98, 3.98)]

    [TestCase(AxleConfiguration.AxleConfig_6x2, true, VehicleClass.ClassP35SD, VehicleClass.ClassP35_36, 2.8 + 0.3, 5.1, 5.1, 5.1, 5.1)]
    [TestCase(AxleConfiguration.AxleConfig_6x2, true, VehicleClass.ClassP35DD, VehicleClass.ClassP35_36, 3.8 + 0.3, 6.4, 6.4, 6.4)]
    [TestCase(AxleConfiguration.AxleConfig_6x2, true, VehicleClass.ClassP36SD, VehicleClass.ClassP35_36, 3.15 + 0.3, 3.6, 3.6)]
    [TestCase(AxleConfiguration.AxleConfig_6x2, true, VehicleClass.ClassP36DD, VehicleClass.ClassP35_36, 3.7 + 0.3, 4.05, 4.05)]

    [TestCase(AxleConfiguration.AxleConfig_8x2, false, VehicleClass.ClassP37SD, VehicleClass.ClassP37_38, 2.8 + 0.3, 5.1, 5.1, 5.1, 5.1)]
    [TestCase(AxleConfiguration.AxleConfig_8x2, false, VehicleClass.ClassP37DD, VehicleClass.ClassP37_38, 3.8 + 0.3, 6.4, 6.4, 6.4)]
    [TestCase(AxleConfiguration.AxleConfig_8x2, false, VehicleClass.ClassP38SD, VehicleClass.ClassP37_38, 3.15 + 0.3, 3.6, 3.6)]
    [TestCase(AxleConfiguration.AxleConfig_8x2, false, VehicleClass.ClassP38DD, VehicleClass.ClassP37_38, 3.7 + 0.3, 4.05, 4.05)]

    [TestCase(AxleConfiguration.AxleConfig_8x2, true, VehicleClass.ClassP39SD, VehicleClass.ClassP39_40, 2.6 + 0.3, 5.2, 5.2, 5.2, 5.2)]
    [TestCase(AxleConfiguration.AxleConfig_8x2, true, VehicleClass.ClassP39DD, VehicleClass.ClassP39_40, 3.8 + 0.3, 6.5, 6.5, 6.5)]
    [TestCase(AxleConfiguration.AxleConfig_8x2, true, VehicleClass.ClassP40SD, VehicleClass.ClassP39_40, 3.15 + 0.3, 3.68, 3.68)]
    [TestCase(AxleConfiguration.AxleConfig_8x2, true, VehicleClass.ClassP40DD, VehicleClass.ClassP39_40, 3.7 + 0.3, 4.13, 4.13)]

    public void TestHeightAndAirdragArea_PrimaryBus(AxleConfiguration axleConfiguration, bool articulated, VehicleClass testClass, VehicleClass expectedVehicleClass, double expectedHeight, params double[] expectedCdxA)
    {
        var segment = DeclarationData.PrimaryBusSegments.Lookup(VehicleCategory.HeavyBusPrimaryVehicle, axleConfiguration, articulated);
        Assert.AreEqual(expectedVehicleClass, segment.VehicleClass);

        var heights = new List<Meter>();
        var areas = new List<SquareMeter>();
        var realDao = new AirdragDataAdapter();
        var dao = new Mock<AirdragDataAdapter>() { CallBase = true };
        dao.Setup(a =>
            a.GetDeclarationAirResistanceCurve(It.IsAny<string>(), It.IsAny<SquareMeter>(), It.IsAny<Meter>())).Returns(
            (string cw, SquareMeter cdxA, Meter h) => {
                heights.Add(h);
                areas.Add(cdxA);
                return realDao.GetDeclarationAirResistanceCurve(cw, cdxA, h);
            });

        var imcData = new Mock<IVehicleInMotionChargingDeclaration>();
        imcData.Setup(i => i.Technology).Returns(IMCTechnology.GroundRail);

        var vehicle = new Mock<IVehicleDeclarationInputData>();
        vehicle.Setup(v => v.InMotionCharging).Returns(imcData.Object);
        vehicle.Setup(v => v.Components).Returns(new Mock<IVehicleComponentsDeclaration>().Object);

        var data = segment.Missions.Where(m => m.BusParameter.BusGroup == testClass).Select(m => dao.Object.CreateAirdragData(vehicle.Object, m, segment, OvcHevMode.NotApplicable)).ToArray();

        Console.WriteLine(heights.Select(x => x.Value()).Join(", "));
        Console.WriteLine(areas.Select(x => x.Value()).Join(", "));

        Assert.IsTrue(heights.All(x => x.IsEqual(expectedHeight)));
        CollectionAssert.AreEqual(expectedCdxA, areas.Select(a => a.Value()));
    }

    [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CE, RegistrationClass.I, 0, 3, false, null, VehicleClass.Class31a, 3 + 0.3, 4.9, 4.9, 4.9)]
    [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.III, 0, 3, false, null, VehicleClass.Class32d, 3 + 0.3, 4.6, 4.6)]
    [TestCase(AxleConfiguration.AxleConfig_6x2, VehicleCode.CG, RegistrationClass.A, 0, 3, true, null, VehicleClass.Class35b1, 3 + 0.3, 5.1, 5.1, 5.1)]
    [TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CC, RegistrationClass.II_III, 0, 3, false, null, VehicleClass.Class40b, 3 + 0.3, 4.9, 4.9)]

    [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CE, RegistrationClass.I, 0, 3, false, 5.123, VehicleClass.Class31a, 3 + 0.3, 4.9, 4.9, 4.9)]
    [TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.III, 0, 3, false, 5.123, VehicleClass.Class32d, 3 + 0.3, 5.123, 5.123)]
    [TestCase(AxleConfiguration.AxleConfig_6x2, VehicleCode.CG, RegistrationClass.A, 0, 3, true, 5.123, VehicleClass.Class35b1, 3 + 0.3, 5.1, 5.1, 5.1)]
    [TestCase(AxleConfiguration.AxleConfig_8x2, VehicleCode.CC, RegistrationClass.II_III, 0, 3, false, 5.123, VehicleClass.Class40b, 3 + 0.3, 5.123, 5.123)]
    public void TestHeightAndAirdragArea_CompleteSpecificBus(AxleConfiguration axleConfiguration, VehicleCode? vehicleCode,
        RegistrationClass? registrationClass, int? passengerCnt, double bodyHeight, bool? lowEntry, double? cdxA,
        VehicleClass expectedVehicleClass, double expectedHeight, params double[] expectedCdxA)
    {
        var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfiguration.NumAxles(), vehicleCode,
            registrationClass, passengerCnt, bodyHeight.SI<Meter>(), lowEntry);
        Assert.AreEqual(expectedVehicleClass, segment.VehicleClass);

        var heights = new List<Meter>();
        var areas = new List<SquareMeter>();
        var realDao = new AirdragDataAdapter();
        var dao = new Mock<AirdragDataAdapter>() { CallBase = true };
        dao.Setup(a =>
            a.GetDeclarationAirResistanceCurve(It.IsAny<string>(), It.IsAny<SquareMeter>(), It.IsAny<Meter>())).Returns(
            (string cw, SquareMeter cdxA, Meter h) => {
                heights.Add(h);
                areas.Add(cdxA);
                return realDao.GetDeclarationAirResistanceCurve(cw, cdxA, h);
            });
        var imcData = new Mock<IVehicleInMotionChargingDeclaration>();
        imcData.Setup(i => i.Technology).Returns(IMCTechnology.GroundRail);

        var vehicle = new Mock<IVehicleDeclarationInputData>();
        vehicle.Setup(v => v.InMotionCharging).Returns(imcData.Object);
        vehicle.Setup(v => v.Height).Returns(bodyHeight.SI<Meter>());
        vehicle.Setup(v => v.Components).Returns(new Mock<IVehicleComponentsDeclaration>().Object);
		if (cdxA.HasValue) {
            vehicle.Setup(v => v.Components.AirdragInputData.AirDragArea).Returns(cdxA.Value.SI<SquareMeter>());
        }

        var data = segment.Missions.Select(m => dao.Object.CreateAirdragData(vehicle.Object, m, segment, OvcHevMode.NotApplicable)).ToArray();

        Console.WriteLine(heights.Select(x => x.Value()).Join(", "));
        Console.WriteLine(areas.Select(x => x.Value()).Join(", "));

        Assert.IsTrue(heights.All(x => x.IsEqual(expectedHeight)));
        CollectionAssert.AreEqual(expectedCdxA, areas.Select(a => a.Value()));

    }
}