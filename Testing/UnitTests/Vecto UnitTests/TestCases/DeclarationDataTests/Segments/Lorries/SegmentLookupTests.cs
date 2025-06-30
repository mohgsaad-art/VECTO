using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.Segments.Lorries
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class SegmentLookupTests
    {

        [Test,
       //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2F, 5000.01, 0, false, VehicleClass.Class51),
       //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2F, 5000.01, 0, false, VehicleClass.Class52),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 5000.01, 0, false, VehicleClass.Class53),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 5000.01, 0, false, VehicleClass.Class53),
       TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 5000.01, 0, false, VehicleClass.Class54),
       //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 5000.01, 0, false, VehicleClass.Class55),
       //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 5000.01, 0, false, VehicleClass.Class55),
       //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x4, 5000.01, 0, false, VehicleClass.Class56),

       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7400.01, 0, false, VehicleClass.Class1s),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7400.01, 0, false, VehicleClass.Class1s),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500.01, 0, false, VehicleClass.Class1),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500.01, 0, false, VehicleClass.Class1),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1),

       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10001, 0, false, VehicleClass.Class2),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10001, 0, false, VehicleClass.Class2),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2),

       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12001, 0, false, VehicleClass.Class3),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12001, 0, false, VehicleClass.Class3),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3),

       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16001, 0, false, VehicleClass.Class4),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 99000, 0, false, VehicleClass.Class4),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16001, 0, true, VehicleClass.Class4),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 99000, 0, true, VehicleClass.Class4),

       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16001, 0, false, VehicleClass.Class5),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 99000, 0, false, VehicleClass.Class5),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16001, 0, true, VehicleClass.Class5),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 99000, 0, true, VehicleClass.Class5),

       //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 7500, 0, VehicleClass.Class6),
       //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 16000, 0, VehicleClass.Class6),
       //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 16001, 0, VehicleClass.Class7),
       //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 99000, 0, VehicleClass.Class7),
       //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 16000, 0, VehicleClass.Class8),
       //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 99000, 0, VehicleClass.Class8),

       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 7500, 0, false, VehicleClass.Class9),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class9),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class9),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 99000, 0, false, VehicleClass.Class9),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 7500, 0, true, VehicleClass.Class9),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 99000, 0, true, VehicleClass.Class9),

       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 7500, 0, false, VehicleClass.Class10),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class10),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class10),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 99000, 0, false, VehicleClass.Class10),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 7500, 0, true, VehicleClass.Class10),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 99000, 0, true, VehicleClass.Class10),

       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 7500, 0, false, VehicleClass.Class11),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 40000, 0, false, VehicleClass.Class11),

       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 7500, 0, false, VehicleClass.Class12),
       TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 99000, 0, false, VehicleClass.Class12),

       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 7500, 0, false, VehicleClass.Class16),
       TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 99000, 0, false, VehicleClass.Class16),
       ]
        public void SegmentLookupTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
           double curbWeight, bool vocational, VehicleClass expectedClass)
        {
            var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);
            Assert.AreEqual(expectedClass, segment.VehicleClass);
        }

		[
			TestCase(0),
			TestCase(1000),
			TestCase(3500),
			//TestCase(7500)
		]
		public void SegmentWeightOutOfRange4X2(double weight)
		{
			var segment = DeclarationData.TruckSegments.Lookup(
				VehicleCategory.RigidTruck,
				AxleConfiguration.AxleConfig_4x2,
				weight.SI<Kilogram>(),
				0.SI<Kilogram>(),
				false);
            Assert.IsFalse(segment.Found);
				//$"ERROR: Could not find the declaration segment for vehicle. " +
				//$"Category: {VehicleCategory.RigidTruck}, " +
				//$"AxleConfiguration: {AxleConfiguration.AxleConfig_4x2.GetName()}, " +
				//$"GrossVehicleWeight: {weight.SI<Kilogram>()}");
		}

		[
			TestCase(0),
			TestCase(1000),
			TestCase(3500),
			TestCase(7500)
		]
		public void SegmentWeightOutOfRange4X4(double weight)
		{
			var segment = DeclarationData.TruckSegments.Lookup(
				VehicleCategory.RigidTruck,
				AxleConfiguration.AxleConfig_4x4,
				weight.SI<Kilogram>(),
				0.SI<Kilogram>(),
				false);
            Assert.IsFalse(segment.Found);
				//$"ERROR: Could not find the declaration segment for vehicle. " +
				//$"Category: {VehicleCategory.RigidTruck}, " +
				//$"AxleConfiguration: {AxleConfiguration.AxleConfig_4x4.GetName()}, " +
				//$"GrossVehicleWeight: {weight.SI<Kilogram>()}");
		}

        [
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2F, 5001, 0, false, VehicleClass.Class51, 85),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2F, 5001, 0, false, VehicleClass.Class52, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 5001, 0, false, VehicleClass.Class53, 85),
        TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 5001, 0, false, VehicleClass.Class54, 85),
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 5001, 0, false, VehicleClass.Class55, 85),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x4, 5001, 0, false, VehicleClass.Class56, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7401, 0, false, VehicleClass.Class1s, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7501, 0, false, VehicleClass.Class1, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10001, 0, false, VehicleClass.Class2, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12001, 0, false, VehicleClass.Class3, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16001, 0, false, VehicleClass.Class4, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16001, 0, true, VehicleClass.Class4, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16001, 0, false, VehicleClass.Class5, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16001, 0, true, VehicleClass.Class5, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 7501, 0, false, VehicleClass.Class9, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 7501, 0, true, VehicleClass.Class9, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 7500, 0, false, VehicleClass.Class10, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 7500, 0, true, VehicleClass.Class10, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 7500, 0, false, VehicleClass.Class11, 85),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 7500, 0, false, VehicleClass.Class12, 85),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 7500, 0, false, VehicleClass.Class16, 85),
        ]
        public void SegmentDesignSpeedTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
    double curbWeight, bool vocational, VehicleClass expectedClass, double speed)
        {
            var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);

            Assert.AreEqual(speed.KMPHtoMeterPerSecond(), segment.DesignSpeed);
        }

        [Test,
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class51, 800, null,
        //    TestName = "SegmentLookupBodyWeight Class51 Rigid"),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class51, 800, null,
        //    TestName = "SegmentLookupBodyWeight Class51 Tractor"),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class52, 0, null,
        //    TestName = "SegmentLookupBodyWeight Class52 Van"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class53, 800, null,
            TestName = "SegmentLookupBodyWeight Class53 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class53, 800, null,
            TestName = "SegmentLookupBodyWeight Class53 Tractor"),
        TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class54, 0, null,
            TestName = "SegmentLookupBodyWeight Class54 Van"),
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class55, 800, null,
        //    TestName = "SegmentLookupBodyWeight Class55 Rigid"),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class55, 800, null,
        //    TestName = "SegmentLookupBodyWeight Class55 Tractor"),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class56, 0, null,
        //    TestName = "SegmentLookupBodyWeight ClassML4rvan Van"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s, 1600, null,
            TestName = "SegmentLookupBodyWeight Class1s Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s, 1600, null,
            TestName = "SegmentLookupBodyWeight Class1s Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1, 1600, null,
            TestName = "SegmentLookupBodyWeight Class1 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1, 1600, null,
            TestName = "SegmentLookupBodyWeight Class1 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2, 1900, 3400,
            TestName = "SegmentLookupBodyWeight Class2 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2, 1900, 3400,
            TestName = "SegmentLookupBodyWeight Class2 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3, 2000, null,
            TestName = "SegmentLookupBodyWeight Class3 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3, 2000, null,
            TestName = "SegmentLookupBodyWeight Class3 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class4, 2100, 5400,
            TestName = "SegmentLookupBodyWeight Class4"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class5, null, 7500,
            TestName = "SegmentLookupBodyWeight Class5"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class9, 2200, 5400,
            TestName = "SegmentLookupBodyWeight Class9"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class10, null, 7500,
            TestName = "SegmentLookupBodyWeight Class10"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 12000, 0, false, VehicleClass.Class11, 2200, 5400,
            TestName = "SegmentLookupBodyWeight Class11"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 12000, 0, false, VehicleClass.Class12, null, 7500,
            TestName = "SegmentLookupBodyWeight Class12"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 12000, 0, false, VehicleClass.Class16, null, null,
            TestName = "SegmentLookupBodyWeight Class16")]
        public void SegmentLookupBodyTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
    double curbWeight, bool vocational, VehicleClass expectedClass, int? expectedBodyWeight, int? expectedTrailerWeight)
        {
            var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);
            Assert.AreEqual(expectedClass, segment.VehicleClass);

            if (expectedBodyWeight.HasValue)
            {
                Assert.AreEqual(expectedBodyWeight, segment.Missions[0].BodyCurbWeight.Value());
            }
            if (expectedTrailerWeight.HasValue)
            {
                var trailerMission = segment.Missions.Where(m => m.Trailer.Count > 0).ToList();
                if (trailerMission.Count > 0)
                {
                    Assert.AreEqual(expectedTrailerWeight, trailerMission.First().Trailer.First().TrailerCurbWeight.Value());
                }
            }
        }

        [Test,
        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class51, 3.5,
        //    TestName = "SegmentLookupHeight Class51 Rigid"),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class51, 3.5,
        //    TestName = "SegmentLookupHeight Class51 Tractor"),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2F, 7400, 0, false, VehicleClass.Class52, 2.9,
        //    TestName = "SegmentLookupHeight Class52 Van"),

        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class53, 3.5,
            TestName = "SegmentLookupHeight Class53 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class53, 3.5,
            TestName = "SegmentLookupHeight Class53 Tractor"),
        TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x2, 7400, 0, false, VehicleClass.Class54, 2.9,
            TestName = "SegmentLookupHeight Class54 Van"),

        //TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class55, 3.5,
        //    TestName = "SegmentLookupHeight Class55 Rigid"),
        //TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class55, 3.5,
        //    TestName = "SegmentLookupHeight Class55 Tractor"),
        //TestCase(VehicleCategory.Van, AxleConfiguration.AxleConfig_4x4, 7400, 0, false, VehicleClass.Class56, 2.9,
        //    TestName = "SegmentLookupHeight Class56 Van"),


        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s, 3.6,
            TestName = "SegmentLookupHeight Class1s Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7500, 0, false, VehicleClass.Class1s, 3.6,
            TestName = "SegmentLookupHeight Class1s Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1, 3.6,
            TestName = "SegmentLookupHeight Class1 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 10000, 0, false, VehicleClass.Class1, 3.6,
            TestName = "SegmentLookupHeight Class1 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2, 3.75,
            TestName = "SegmentLookupHeight Class2 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2, 3.75,
            TestName = "SegmentLookupHeight Class2 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3, 3.9,
            TestName = "SegmentLookupHeight Class3 Rigid"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3, 3.9,
            TestName = "SegmentLookupHeight Class3 Tractor"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class4, 4.0,
            TestName = "SegmentLookupHeight Class4"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, true, VehicleClass.Class4, 4.0,
            TestName = "SegmentLookupHeight Class4v"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class5, 4.0,
            TestName = "SegmentLookupHeight Class5"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, true, VehicleClass.Class5, 4.0,
            TestName = "SegmentLookupHeight Class5v"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 10000, 0, false, VehicleClass.Class9, 3.6,
            TestName = "SegmentLookupHeight Class9 - 1"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 12000, 0, false, VehicleClass.Class9, 3.75,
            TestName = "SegmentLookupHeight Class9 - 2"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class9, 3.9,
            TestName = "SegmentLookupHeight Class9 - 3"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 18000, 0, false, VehicleClass.Class9, 4.0,
            TestName = "SegmentLookupHeight Class9 - 4"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class9, 4.0,
            TestName = "SegmentLookupHeight Class9 - other"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 40000, 0, true, VehicleClass.Class9, 4.0,
            TestName = "SegmentLookupHeight Class9v - other"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, false, VehicleClass.Class10, 4.0,
            TestName = "SegmentLookupHeight Class10"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 40000, 0, true, VehicleClass.Class10, 4.0,
            TestName = "SegmentLookupHeight Class10v"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 12000, 0, false, VehicleClass.Class11, 4.0,
            TestName = "SegmentLookupHeight Class11"),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 12000, 0, false, VehicleClass.Class12, 4.0,
            TestName = "SegmentLookupHeight Class12"),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 12000, 0, false, VehicleClass.Class16, 3.6,
            TestName = "SegmentLookupHeight Class16")]
        public void SegmentLookupHeightTest(VehicleCategory category, AxleConfiguration axleConfiguration, double grossWeight,
            double curbWeight, bool vocational, VehicleClass expectedClass, double expectedHeight)
        {
            var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);
            Assert.AreEqual(expectedClass, segment.VehicleClass);
            AssertHelper.AreRelativeEqual(expectedHeight, segment.Missions.First().VehicleHeight);
        }

        [Test,
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7401, 0, false, VehicleClass.Class1s,
            new[] { 36.5, 36.5 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7401, 0, false, VehicleClass.Class1s,
            new[] { 36.5, 36.5 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 7501, 0, false, VehicleClass.Class1,
            new[] { 36.5, 36.5 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 7501, 0, false, VehicleClass.Class1,
            new[] { 36.5, 36.5 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2,
            new[] { 85.0, 45.2, 45.2 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 12000, 0, false, VehicleClass.Class2,
            new[] { 85.0, 45.2, 45.2 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3,
            new[] { 47.7, 47.7 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 16000, 0, false, VehicleClass.Class3,
            new[] { 47.7, 47.7 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class4,
            new[] { 98.9, 49.4, 49.4, 0.0, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_4x2, 18000, 0, true, VehicleClass.Class4,
            new[] { 98.9, 49.4, 49.4, 0.0, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, false, VehicleClass.Class5,
            new[] { 91.0, 140.5, 91.0, 140.5, 91.0, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_4x2, 18000, 0, true, VehicleClass.Class5,
            new[] { 91.0, 140.5, 91.0, 140.5, 91.0, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class9,
            new[] { 101.4, 142.9, 51.9, 142.9, 0.0, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x2, 16000, 0, true, VehicleClass.Class9,
            new[] { 101.4, 142.9, 51.9, 142.9, 0.0, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 16000, 0, false, VehicleClass.Class10,
            new[] { 91.0, 140.5, 91.0, 140.5, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x2, 16000, 0, true, VehicleClass.Class10,
            new[] { 91.0, 140.5, 91.0, 140.5, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_6x4, 40000, 0, false, VehicleClass.Class11,
            new[] { 101.4, 142.9, 51.9, 142.9, 0.0, 0.0 }),
        TestCase(VehicleCategory.Tractor, AxleConfiguration.AxleConfig_6x4, 99000, 0, false, VehicleClass.Class12,
            new[] { 91.0, 140.5, 91.0, 140.5, 0.0 }),
        TestCase(VehicleCategory.RigidTruck, AxleConfiguration.AxleConfig_8x4, 99000, 0, false, VehicleClass.Class16,
            new[] { 101.4, 142.9, 51.9, 142.9, 0.0 })
        ]
        public void SegmentLookupCargoVolumeTest(VehicleCategory category, AxleConfiguration axleConfiguration,
            double grossWeight,
            double curbWeight, bool vocational, VehicleClass expectedClass, double[] expectedCargoVolume)
        {
            var segment = DeclarationData.TruckSegments.Lookup(category, axleConfiguration, grossWeight.SI<Kilogram>(),
                curbWeight.SI<Kilogram>(), vocational);
            Assert.AreEqual(expectedClass, segment.VehicleClass);
            Assert.AreEqual(expectedCargoVolume.Length, segment.Missions.Length);
            for (var i = 0; i < expectedCargoVolume.Length; i++)
            {
                Assert.AreEqual(expectedCargoVolume[i], segment.Missions[i].TotalCargoVolume.Value());
            }
        }
    }
}
