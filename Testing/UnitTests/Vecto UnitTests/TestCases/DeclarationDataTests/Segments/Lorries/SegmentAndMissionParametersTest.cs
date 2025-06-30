using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.Segments.Lorries;

[TestFixture]
[Parallelizable(ParallelScope.All)]
public class SegmentAndMissionParametersTest
{

    /// <summary>
    /// trailer in longhaul, always pc formula
    /// </summary>
    [TestCase]
    public void Segment2Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            GrossVehicleMassRating = 11900.SI<Kilogram>(),
            CurbWeight = 5850.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class2, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(3, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaul,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.225, 0.325 },
            trailerAxleWeightDistribution: new[] { 0.45 },
            trailerAxleCount: new[] { 2 },
            bodyCurbWeight: 1900,
            trailerCurbWeight: new[] { 3400.0 },
            trailerType: new[] { TrailerType.T1 },
            lowLoad: 1296.8235,
            refLoad: 9450,
            trailerGrossVehicleWeight: new[] { 10500.0 },
            deltaCdA: 1.3,
            maxLoad: 11250);

        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 1900,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 596.8235,
            refLoad: 2984.1176,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 4150);

        AssertMission(segment.Missions[2],
            vehicleData: vehicleData,
            missionType: MissionType.UrbanDelivery,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 1900,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 596.8235,
            refLoad: 2984.1176,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 4150);
    }


    /// <summary>
    /// trailer in longhaul, always pc formula
    /// </summary>
    [TestCase]
    public void Segment2TestHeavy()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            GrossVehicleMassRating = 11990.SI<Kilogram>(),
            CurbWeight = 9500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class2, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(3, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaul,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.225, 0.325 },
            trailerAxleWeightDistribution: new[] { 0.45 },
            trailerAxleCount: new[] { 2 },
            bodyCurbWeight: 1900,
            trailerCurbWeight: new[] { 3400.0 },
            trailerType: new[] { TrailerType.T1 },
            lowLoad: 1290,
            refLoad: 5890,
            trailerGrossVehicleWeight: new[] { 10500.0 },
            deltaCdA: 1.3,
            maxLoad: 7690);

        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 1900,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 590,
            refLoad: 590,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 590);

        AssertMission(segment.Missions[2],
            vehicleData: vehicleData,
            missionType: MissionType.UrbanDelivery,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 1900,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 590,
            refLoad: 590,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 590);
    }


    /// <summary>
    /// normal pc formula, no trailer
    /// </summary>
    [TestCase]
    public void Segment3Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            GrossVehicleMassRating = 14000.SI<Kilogram>(),
            CurbWeight = 5850.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class3, segment.VehicleClass);

        NUnit.Framework.Assert.AreEqual(2, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.4, 0.6 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 2000,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 762.3529,
            refLoad: 3811.7647,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 6150);

        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.UrbanDelivery,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.4, 0.6 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 2000,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 762.3529,
            refLoad: 3811.7647,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 6150);
    }

    /// <summary>
    /// fixed reference weight, trailer only in longhaul
    /// </summary>
    [TestCase]
    public void Segment4Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            GrossVehicleMassRating = 18000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class4, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(5, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaul,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.2, 0.3 },
            trailerAxleWeightDistribution: new[] { 0.5 },
            trailerAxleCount: new[] { 2 },
            bodyCurbWeight: 2100,
            trailerCurbWeight: new[] { 5400.0 },
            trailerType: new[] { TrailerType.T2 },
            lowLoad: 1900,
            refLoad: 14000,
            trailerGrossVehicleWeight: new[] { 18000.0 },
            deltaCdA: 1.5,
            maxLoad: 21000);

        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 2100,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 900,
            refLoad: 4400,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 8400);

        AssertMission(segment.Missions[2],
                    vehicleData: vehicleData,
                    missionType: MissionType.UrbanDelivery,
                    cosswindCorrection: "RigidSolo",
                    axleWeightDistribution: new[] { 0.45, 0.55 },
                    trailerAxleWeightDistribution: new double[] { },
                    trailerAxleCount: new int[] { },
                    bodyCurbWeight: 2100,
                    trailerCurbWeight: new double[] { },
                    trailerType: new TrailerType[] { },
                    lowLoad: 900,
                    refLoad: 4400,
                    trailerGrossVehicleWeight: new double[] { },
                    deltaCdA: 0,
                    maxLoad: 8400);

        AssertMission(segment.Missions[3],
            vehicleData: vehicleData,
            missionType: MissionType.MunicipalUtility,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 6000,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 600,
            refLoad: 3000,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 4500);
    }

    /// <summary>
    /// fixed reference weight, trailer only in longhaul
    /// </summary>
    [TestCase]
    public void Segment4VocationalTest()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            GrossVehicleMassRating = 18000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, true);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class4, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(5, segment.Missions.Length);

        AssertMission(segment.Missions[3],
            vehicleData: vehicleData,
            missionType: MissionType.MunicipalUtility,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 6000,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 600,
            refLoad: 3000,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 4500);

        AssertMission(segment.Missions[4],
                    vehicleData: vehicleData,
                    missionType: MissionType.Construction,
                    cosswindCorrection: "RigidSolo",
                    axleWeightDistribution: new[] { 0.45, 0.55 },
                    trailerAxleWeightDistribution: new double[] { },
                    trailerAxleCount: new int[] { },
                    bodyCurbWeight: 2000,
                    trailerCurbWeight: new double[] { },
                    trailerType: new TrailerType[] { },
                    lowLoad: 900,
                    refLoad: 4400,
                    trailerGrossVehicleWeight: new double[] { },
                    deltaCdA: 0,
                    maxLoad: 8500);
    }

    /// <summary>
    /// Segment 5: fixed reference weight, trailer always used
    /// </summary>
    [TestCase]
    public void Segment5Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.Tractor,
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            GrossVehicleMassRating = 18000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class5, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(6, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaul,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.2, 0.25 },
            trailerAxleWeightDistribution: new[] { 0.55 },
            trailerAxleCount: new[] { 3 }, bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0 },
            trailerType: new[] { TrailerType.ST1 },
            lowLoad: 2600,
            refLoad: 19300,
            trailerGrossVehicleWeight: new[] { 24000.0 },
            deltaCdA: 0,
            maxLoad: 25000);

        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaulEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.15, 0.2 },
            trailerAxleWeightDistribution: new[] { 0.40, 0.25 },
            trailerAxleCount: new[] { 3, 2 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0, 5400 },
            trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
            lowLoad: 3500,
            refLoad: 26500,
            trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
            deltaCdA: 1.5,
            maxLoad: 39600,
            ems: true);

        AssertMission(segment.Missions[2],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.25, 0.25 },
            trailerAxleWeightDistribution: new[] { 0.5 },
            trailerAxleCount: new[] { 3 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0 },
            trailerType: new[] { TrailerType.ST1 },
            lowLoad: 2600,
            refLoad: 12900,
            trailerGrossVehicleWeight: new[] { 24000.0 },
            deltaCdA: 0, maxLoad: 25000);

        AssertMission(segment.Missions[3],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDeliveryEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.175, 0.25 },
            trailerAxleWeightDistribution: new[] { 0.35, 0.225 },
            trailerAxleCount: new[] { 3, 2 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0, 5400 },
            trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
            lowLoad: 3500,
            refLoad: 17500,
            trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
            deltaCdA: 1.5,
            maxLoad: 39600,
            ems: true);

        AssertMission(segment.Missions[4],
            vehicleData: vehicleData,
            missionType: MissionType.UrbanDelivery,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.25, 0.25 },
            trailerAxleWeightDistribution: new[] { 0.5 },
            trailerAxleCount: new[] { 3 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0 },
            trailerType: new[] { TrailerType.ST1 },
            lowLoad: 2600,
            refLoad: 12900,
            trailerGrossVehicleWeight: new[] { 24000.0 },
            deltaCdA: 0, maxLoad: 25000);
    }

    /// <summary>
    /// Segment 5: fixed reference weight, trailer always used
    /// </summary>
    [TestCase]
    public void Segment5VocationalTest()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.Tractor,
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            GrossVehicleMassRating = 18000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, true);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class5, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(6, segment.Missions.Length);

        AssertMission(segment.Missions[5],
            vehicleData: vehicleData,
            missionType: MissionType.Construction,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.25, 0.25 },
            trailerAxleWeightDistribution: new[] { 0.5 },
            trailerAxleCount: new[] { 3 }, bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 6100.0 },
            trailerType: new[] { TrailerType.STT1 },
            lowLoad: 2600,
            refLoad: 12900,
            trailerGrossVehicleWeight: new[] { 24000.0 },
            deltaCdA: 0,
            maxLoad: 26400);

    }

    /// <summary>
    /// Segment 9: fixed reference weight, trailer always used
    /// </summary>
    [TestCase]
    public void Segment9Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
            GrossVehicleMassRating = 24000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class9, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(6, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaul,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.2, 0.3, 0.15 },
            trailerAxleWeightDistribution: new[] { 0.35 },
            trailerAxleCount: new[] { 2 },
            bodyCurbWeight: 2200,
            trailerCurbWeight: new[] { 5400.0 },
            trailerType: new[] { TrailerType.T2 },
            lowLoad: 2600,
            refLoad: 19300,
            trailerGrossVehicleWeight: new[] { 18000.0 },
            deltaCdA: 1.5,
            maxLoad: 24900);

        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaulEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.15, 0.2, 0.1 },
            trailerAxleWeightDistribution: new[] { 0.225, 0.325 },
            trailerAxleCount: new[] { 2, 3 },
            bodyCurbWeight: 2200,
            trailerCurbWeight: new[] { 2500, 7500.0 },
            trailerType: new[] { TrailerType.Dolly, TrailerType.ST1 },
            lowLoad: 3500,
            refLoad: 26500,
            trailerGrossVehicleWeight: new[] { 12000.0, 24000 },
            deltaCdA: 2.1,
            maxLoad: 40300,
            ems: true);

        AssertMission(segment.Missions[2],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.35, 0.4, 0.25 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 2200,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 1400,
            refLoad: 7100,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 14300);

        AssertMission(segment.Missions[3],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDeliveryEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.175, 0.2, 0.1 },
            trailerAxleWeightDistribution: new[] { 0.225, 0.3 },
            trailerAxleCount: new[] { 2, 3 },
            bodyCurbWeight: 2200,
            trailerCurbWeight: new[] { 2500, 7500.0 },
            trailerType: new[] { TrailerType.Dolly, TrailerType.ST1 },
            lowLoad: 3500,
            refLoad: 17500,
            trailerGrossVehicleWeight: new[] { 12000.0, 24000 },
            deltaCdA: 2.1,
            maxLoad: 40300,
            ems: true);

        AssertMission(segment.Missions[4],
            vehicleData: vehicleData,
            missionType: MissionType.MunicipalUtility,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.35, 0.4, 0.25 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 6750,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 1200,
            refLoad: 6000,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 9750);
    }

    /// <summary>
    /// Segment 9: fixed reference weight, trailer always used
    /// </summary>
    [TestCase]
    public void Segment9VocationalTest()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
            GrossVehicleMassRating = 24000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, true);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class9, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(6, segment.Missions.Length);

        AssertMission(segment.Missions[4],
            vehicleData: vehicleData,
            missionType: MissionType.MunicipalUtility,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.35, 0.4, 0.25 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 6750,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 1200,
            refLoad: 6000,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 9750);

        AssertMission(segment.Missions[5],
                    vehicleData: vehicleData,
                    missionType: MissionType.Construction,
                    cosswindCorrection: "RigidSolo",
                    axleWeightDistribution: new[] { 0.35, 0.4, 0.25 },
                    trailerAxleWeightDistribution: new double[] { },
                    trailerAxleCount: new int[] { },
                    bodyCurbWeight: 3230,
                    trailerCurbWeight: new double[] { },
                    trailerType: new TrailerType[] { },
                    lowLoad: 1400,
                    refLoad: 7100,
                    trailerGrossVehicleWeight: new double[] { },
                    deltaCdA: 0,
                    maxLoad: 13270);

    }

    /// <summary>
    /// Segment 10: fixed reference weight, trailer always used
    /// </summary>
    [TestCase]
    public void Segment10Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.Tractor,
            AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
            GrossVehicleMassRating = 24000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class10, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(5, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaul,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.15, 0.1, 0.2 },
            trailerAxleWeightDistribution: new[] { 0.55 },
            trailerAxleCount: new[] { 3 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0 },
            trailerType: new[] { TrailerType.ST1 },
            lowLoad: 2600,
            refLoad: 19300,
            trailerGrossVehicleWeight: new[] { 24000.0 },
            deltaCdA: 0,
            maxLoad: 25000);

        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaulEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.125, 0.15, 0.1 },
            trailerAxleWeightDistribution: new[] { 0.375, 0.25 },
            trailerAxleCount: new[] { 3, 2 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0, 5400 },
            trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
            lowLoad: 3500,
            refLoad: 26500,
            trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
            deltaCdA: 1.5,
            maxLoad: 39600,
            ems: true);

        AssertMission(segment.Missions[2],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.2, 0.1, 0.2 },
            trailerAxleWeightDistribution: new[] { 0.5 },
            trailerAxleCount: new[] { 3 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0 },
            trailerType: new[] { TrailerType.ST1 },
            lowLoad: 2600,
            refLoad: 12900,
            trailerGrossVehicleWeight: new[] { 24000.0 },
            deltaCdA: 0,
            maxLoad: 25000);

        AssertMission(segment.Missions[3],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDeliveryEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.15, 0.15, 0.1 },
            trailerAxleWeightDistribution: new[] { 0.35, 0.25 },
            trailerAxleCount: new[] { 3, 2 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0, 5400 },
            trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
            lowLoad: 3500,
            refLoad: 17500,
            trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
            deltaCdA: 1.5,
            maxLoad: 39600,
            ems: true);
    }

    /// <summary>
    /// Segment 10: fixed reference weight, trailer always used
    /// </summary>
    [TestCase]
    public void Segment10VocationalTest()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.Tractor,
            AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
            GrossVehicleMassRating = 24000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, true);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class10, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(5, segment.Missions.Length);

        AssertMission(segment.Missions[4],
            vehicleData: vehicleData,
            missionType: MissionType.Construction,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.2, 0.1, 0.2 },
            trailerAxleWeightDistribution: new[] { 0.5 },
            trailerAxleCount: new[] { 2 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 5600.0 },
            trailerType: new[] { TrailerType.STT2 },
            lowLoad: 2600,
            refLoad: 12900,
            trailerGrossVehicleWeight: new[] { 18000.0 },
            deltaCdA: 0,
            maxLoad: 26900);

    }

    /// <summary>
    /// Segment 11: fixed reference weight, trailer always used
    /// </summary>
    [TestCase]
    public void Segment11Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_6x4,
            GrossVehicleMassRating = 24000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class11, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(6, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaul,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.2, 0.225, 0.225 },
            trailerAxleWeightDistribution: new[] { 0.35 },
            trailerAxleCount: new[] { 2 },
            bodyCurbWeight: 2200,
            trailerCurbWeight: new[] { 5400.0 },
            trailerType: new[] { TrailerType.T2 },
            lowLoad: 2600,
            refLoad: 19300,
            trailerGrossVehicleWeight: new[] { 18000.0 },
            deltaCdA: 1.5,
            maxLoad: 24900);

        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaulEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.15, 0.2, 0.1 },
            trailerAxleWeightDistribution: new[] { 0.225, 0.325 },
            trailerAxleCount: new[] { 2, 3 },
            bodyCurbWeight: 2200,
            trailerCurbWeight: new[] { 2500, 7500.0 },
            trailerType: new[] { TrailerType.Dolly, TrailerType.ST1 },
            lowLoad: 3500,
            refLoad: 26500,
            trailerGrossVehicleWeight: new[] { 12000.0, 24000 },
            deltaCdA: 2.1,
            maxLoad: 40300,
            ems: true);

        AssertMission(segment.Missions[2],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.35, 0.35, 0.3 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { }, bodyCurbWeight: 2200,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 1400,
            refLoad: 7100,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 14300);

        AssertMission(segment.Missions[3],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDeliveryEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.175, 0.2, 0.1 },
            trailerAxleWeightDistribution: new[] { 0.225, 0.3 },
            trailerAxleCount: new[] { 2, 3 },
            bodyCurbWeight: 2200,
            trailerCurbWeight: new[] { 2500, 7500.0 },
            trailerType: new[] { TrailerType.Dolly, TrailerType.ST1 },
            lowLoad: 3500,
            refLoad: 17500,
            trailerGrossVehicleWeight: new[] { 12000.0, 24000 },
            deltaCdA: 2.1,
            maxLoad: 40300,
            ems: true);

        AssertMission(segment.Missions[4],
            vehicleData: vehicleData,
            missionType: MissionType.MunicipalUtility,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.35, 0.35, 0.3 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 6750,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 1200,
            refLoad: 6000,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 9750);

        AssertMission(segment.Missions[5],
            vehicleData: vehicleData,
            missionType: MissionType.Construction,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.35, 0.35, 0.3 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 3230,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 1400,
            refLoad: 7100,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 13270);
    }

    /// <summary>
    /// Segment 10: fixed reference weight, trailer always used
    /// </summary>
    [TestCase]
    public void Segment12Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.Tractor,
            AxleConfiguration = AxleConfiguration.AxleConfig_6x4,
            GrossVehicleMassRating = 24000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class12, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(5, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaul,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.15, 0.15, 0.15 },
            trailerAxleWeightDistribution: new[] { 0.55 },
            trailerAxleCount: new[] { 3 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0 },
            trailerType: new[] { TrailerType.ST1 },
            lowLoad: 2600,
            refLoad: 19300,
            trailerGrossVehicleWeight: new[] { 24000.0 },
            deltaCdA: 0,
            maxLoad: 25000);

        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.LongHaulEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.125, 0.15, 0.1 },
            trailerAxleWeightDistribution: new[] { 0.375, 0.25 },
            trailerAxleCount: new[] { 3, 2 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0, 5400 },
            trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
            lowLoad: 3500,
            refLoad: 26500,
            trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
            deltaCdA: 1.5,
            maxLoad: 39600,
            ems: true);

        AssertMission(segment.Missions[2],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.2, 0.15, 0.15 },
            trailerAxleWeightDistribution: new[] { 0.5 },
            trailerAxleCount: new[] { 3 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0 },
            trailerType: new[] { TrailerType.ST1 },
            lowLoad: 2600,
            refLoad: 12900,
            trailerGrossVehicleWeight: new[] { 24000.0 },
            deltaCdA: 0,
            maxLoad: 25000);

        AssertMission(segment.Missions[3],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDeliveryEMS,
            cosswindCorrection: "RigidTrailer",
            axleWeightDistribution: new[] { 0.15, 0.15, 0.1 },
            trailerAxleWeightDistribution: new[] { 0.35, 0.25 },
            trailerAxleCount: new[] { 3, 2 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 7500.0, 5400 },
            trailerType: new[] { TrailerType.ST1, TrailerType.T2 },
            lowLoad: 3500,
            refLoad: 17500,
            trailerGrossVehicleWeight: new[] { 24000.0, 18000 },
            deltaCdA: 1.5,
            maxLoad: 39600,
            ems: true);

        AssertMission(segment.Missions[4],
            vehicleData: vehicleData,
            missionType: MissionType.Construction,
            cosswindCorrection: "TractorSemitrailer",
            axleWeightDistribution: new[] { 0.2, 0.15, 0.15 },
            trailerAxleWeightDistribution: new[] { 0.5 },
            trailerAxleCount: new[] { 2 },
            bodyCurbWeight: 0,
            trailerCurbWeight: new[] { 5600.0 },
            trailerType: new[] { TrailerType.STT2 },
            lowLoad: 2600,
            refLoad: 12900,
            trailerGrossVehicleWeight: new[] { 18000.0 },
            deltaCdA: 0,
            maxLoad: 26900,
            ems: false);
    }

    /// <summary>
    /// Segment 9: fixed reference weight, trailer always used
    /// </summary>
    [TestCase]
    public void Segment16Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_8x4,
            GrossVehicleMassRating = 36000.SI<Kilogram>(),
            CurbWeight = 7500.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class16, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(5, segment.Missions.Length);

        AssertMission(segment.Missions[4],
            vehicleData: vehicleData,
            missionType: MissionType.Construction,
            cosswindCorrection: "RigidSolo",
            axleWeightDistribution: new[] { 0.25, 0.25, 0.25, 0.25 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 4355,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 2600,
            refLoad: 12900,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 24145);
    }

    /// <summary>
    /// Segment 53: medium lorry
    /// </summary>
    [TestCase]
    public void Segment53Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.RigidTruck,
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            GrossVehicleMassRating = 5200.SI<Kilogram>(),
            CurbWeight = 1300.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class53, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(2, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "MediumLorriesRigid",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 800,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 149.743589,
            refLoad: 764.3589743,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 3100);
        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.UrbanDelivery,
            cosswindCorrection: "MediumLorriesRigid",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 800,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 149.743589,
            refLoad: 764.3589743,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 3100);
    }

    /// <summary>
    /// Segment 54: medium lorry
    /// </summary>
    [TestCase]
    public void Segment54Test()
    {
        var vehicleData = new
        {
            VehicleCategory = VehicleCategory.Van,
            AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
            GrossVehicleMassRating = 5200.SI<Kilogram>(),
            CurbWeight = 1300.SI<Kilogram>()
        };

        var segment = DeclarationData.TruckSegments.Lookup(vehicleData.VehicleCategory, vehicleData.AxleConfiguration,
            vehicleData.GrossVehicleMassRating, vehicleData.CurbWeight, false);

        NUnit.Framework.Assert.AreEqual(VehicleClass.Class54, segment.VehicleClass);

        var data = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
        TestAcceleration(data);

        NUnit.Framework.Assert.AreEqual(2, segment.Missions.Length);

        AssertMission(segment.Missions[0],
            vehicleData: vehicleData,
            missionType: MissionType.RegionalDelivery,
            cosswindCorrection: "MediumLorriesVan",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 0,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 149.743589,
            refLoad: 764.3589743,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 3900);
        AssertMission(segment.Missions[1],
            vehicleData: vehicleData,
            missionType: MissionType.UrbanDelivery,
            cosswindCorrection: "MediumLorriesVan",
            axleWeightDistribution: new[] { 0.45, 0.55 },
            trailerAxleWeightDistribution: new double[] { },
            trailerAxleCount: new int[] { },
            bodyCurbWeight: 0,
            trailerCurbWeight: new double[] { },
            trailerType: new TrailerType[] { },
            lowLoad: 149.743589,
            refLoad: 764.3589743,
            trailerGrossVehicleWeight: new double[] { },
            deltaCdA: 0,
            maxLoad: 3900);
    }

    public static void AssertMission(Mission m, dynamic vehicleData, MissionType missionType, string cosswindCorrection,
        double[] axleWeightDistribution, double[] trailerAxleWeightDistribution, int[] trailerAxleCount,
        double bodyCurbWeight, double[] trailerCurbWeight, TrailerType[] trailerType, double lowLoad, double refLoad,
        double maxLoad, double[] trailerGrossVehicleWeight, double deltaCdA, bool ems = false)
    {
        NUnit.Framework.Assert.AreEqual(missionType, m.MissionType);
        NUnit.Framework.Assert.AreEqual(cosswindCorrection, m.CrossWindCorrectionParameters);
        NUnit.Framework.CollectionAssert.AreEqual(axleWeightDistribution, m.AxleWeightDistribution,
            "Axle distribution not equal.\nexpected: {0}\nactual: {1}", axleWeightDistribution.Join(),
            m.AxleWeightDistribution.Join());
        NUnit.Framework.CollectionAssert.AreEqual(trailerAxleWeightDistribution, m.Trailer.Select(t => t.TrailerAxleWeightShare),
            "Trailer axle distribution not equal.\nexpected: {0}\nactual: {1}", trailerAxleWeightDistribution.Join(),
            m.Trailer.Select(t => t.TrailerAxleWeightShare).Join());
        NUnit.Framework.Assert.AreEqual(bodyCurbWeight.SI<Kilogram>(), m.BodyCurbWeight);
        NUnit.Framework.CollectionAssert.AreEqual(trailerCurbWeight, m.Trailer.Select(t => t.TrailerCurbWeight.Value()));
        NUnit.Framework.CollectionAssert.AreEqual(trailerType, m.Trailer.Select(t => t.TrailerType));
        NUnit.Framework.CollectionAssert.AreEqual(trailerAxleCount, m.Trailer.Select(t => t.TrailerWheels.Count));
        //Assert.IsNotNull(m.CycleFile);
        //Assert.IsTrue(!string.IsNullOrEmpty(new StreamReader(m.CycleFile).ReadLine()));
        NUnit.Framework.Assert.AreEqual(null, m.MinLoad);
        AssertHelper.AreRelativeEqual(lowLoad, m.LowLoad);
        AssertHelper.AreRelativeEqual(refLoad, m.RefLoad);
        NUnit.Framework.Assert.AreEqual(maxLoad.SI<Kilogram>(), m.MaxPayload);
        NUnit.Framework.CollectionAssert.AreEqual(trailerGrossVehicleWeight, m.Trailer.Select(t => t.TrailerGrossVehicleWeight.Value()));
        NUnit.Framework.Assert.AreEqual(
            VectoMath.Min(
                vehicleData.GrossVehicleMassRating +
                m.Trailer.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0),
                ems ? 60000.SI<Kilogram>() : 40000.SI<Kilogram>())
            - m.BodyCurbWeight - m.Trailer.Sum(t => t.TrailerCurbWeight).DefaultIfNull(0) -
            vehicleData.CurbWeight,
            m.MaxPayload);
        NUnit.Framework.Assert.AreEqual(deltaCdA.SI<SquareMeter>(),
            m.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0));
    }

    private const double Tolerance = 0.0001;

    private static void EqualAcceleration(AccelerationCurveData data, double velocity, double acceleration,
        double deceleration)
    {
        var entry = data.Lookup(velocity.KMPHtoMeterPerSecond());
        Assert.AreEqual(entry.Acceleration.Value(), acceleration, Tolerance);
        Assert.AreEqual(entry.Deceleration.Value(), deceleration, Tolerance);
    }

    private static void TestAcceleration(AccelerationCurveData data)
    {
        // FIXED POINTS
        EqualAcceleration(data, 0, 1, -1);
        EqualAcceleration(data, 25, 1, -1);
        EqualAcceleration(data, 50, 0.642857143, -1);
        EqualAcceleration(data, 60, 0.5, -0.5);
        EqualAcceleration(data, 120, 0.5, -0.5);

        // INTERPOLATED POINTS
        EqualAcceleration(data, 20, 1, -1);
        EqualAcceleration(data, 40, 0.785714286, -1);
        EqualAcceleration(data, 55, 0.571428572, -0.75);
        EqualAcceleration(data, 80, 0.5, -0.5);
        EqualAcceleration(data, 100, 0.5, -0.5);

        // EXTRAPOLATE 
        EqualAcceleration(data, -20, 1, -1);
        EqualAcceleration(data, 140, 0.5, -0.5);
    }
}