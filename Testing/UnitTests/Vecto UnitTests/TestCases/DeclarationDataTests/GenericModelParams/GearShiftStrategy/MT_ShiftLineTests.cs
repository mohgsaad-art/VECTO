using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.Vecto.UnitTests.TestCases.DeclarationDataTests.GenericModelParams.GearShiftStrategy;

public class MT_ShiftLineTests
{

	[TestCase]
    public void ComputeShiftPolygonDeclarationTest()
    {
        var rdyn = 0.4882675.SI<Meter>();
        var axlegearRatio = 2.59;

        var expectedDownshift = new[] {
				// Gear 1
				new[] {
                    new Point(64.50736915, -352),
                    new Point(64.50736915, 970.37),
                    new Point(121.059, 2530),
                },
				// Gear 2
				new[] {
                    new Point(64.50736915, -352),
                    new Point(64.50736915, 970.37),
                    new Point(121.059, 2530),
                },
				// Gear 3
				new[] {
                    new Point(64.50736915, -352),
                    new Point(64.50736915, 970.37),
                    new Point(121.059, 2530),
                },
            };
        var expectedUpshift = new[] {
				// Gear 1
				new[] {
                    new Point(136.9338946, -352),
                    new Point(136.9338946, 1281.30911),
                    new Point(201.7326, 2427.6748),
                },
				// Gear 2
				new[] {
                    new Point(136.9338946, -352),
                    new Point(136.9338946, 1281.30911),
                    new Point(203.9530, 2466.9558),
                },
				// Gear 3
				new[] {
                    new Point(136.9338946, -352),
                    new Point(136.9338946, 1281.30911),
					//new Point(153.606666, 1893.19273),
					new Point(201.3375, 2420.6842),
                },
            };

		var fld = CombustionEngineFLDData.Select(x => new EngineFullLoadCurve.FullLoadCurveEntry() {
            EngineSpeed = x[0].RPMtoRad(),
            TorqueFullLoad = x[1].SI<NewtonMeter>(),
            TorqueDrag = x[2].SI<NewtonMeter>(),
		}).ToList();

		var engineData = new CombustionEngineData() {
            IdleSpeed = 560.RPMtoRad(),
            FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() {
				{0u, new EngineFullLoadCurve(fld, null)}
			}
		};

		var ratios = new[] { 14.93, 11.64, 9.02, 7.04, 5.64, 4.4, 3.39, 2.65, 2.05, 1.6, 1.28, 1.0 };
		var gears = new List<ITransmissionInputData>();
		foreach (var ratio in ratios) {
			var gear = new Mock<ITransmissionInputData>();
			gear.Setup(g => g.Ratio).Returns(ratio);
            gears.Add(gear.Object);
			engineData.FullLoadCurves[(uint)gears.Count] = new EngineFullLoadCurve(fld, null);
		}

		var gearboxData = new Mock<IGearboxDeclarationInputData>().Object;
		Mock.Get(gearboxData).Setup(g => g.Gears).Returns(gears);

        var shiftPolygons = new List<ShiftPolygon>();
        for (var i = 0; i < gearboxData.Gears.Count; i++) {
            shiftPolygons.Add(DeclarationData.Gearbox.ComputeManualTransmissionShiftPolygon(i,
                engineData.FullLoadCurves[(uint)(i + 1)],
                gearboxData.Gears,
                engineData, axlegearRatio, rdyn));
        }

        for (var i = 0; i < Math.Min(gearboxData.Gears.Count, expectedDownshift.Length); i++) {
            foreach (var tuple in expectedDownshift[i].Zip(shiftPolygons[i].Downshift, Tuple.Create)) {
                Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
                Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
            }

            foreach (var tuple in expectedUpshift[i].Zip(shiftPolygons[i].Upshift, Tuple.Create)) {
                Assert.AreEqual(tuple.Item1.X, tuple.Item2.AngularSpeed.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
                Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Torque.Value(), 1e-3, "gear: {0} entry: {1}", i + 1, tuple);
            }
        }

        Assert.AreEqual(0, shiftPolygons.First().Downshift.Count);
        Assert.AreEqual(0, shiftPolygons.Last().Upshift.Count);
    }

	protected double[][] CombustionEngineFLDData = new[] {
		new[] { 560.0, 1180.0, -149.0 },
		new[] { 600.0, 1282.0, -148.0 },
		new[] { 800.0, 1791.0, -149.0 },
		new[] { 1000.0, 2300.0, -160.0 },
		new[] { 1200.0, 2300.0, -179.0 },
		new[] { 1400.0, 2300.0, -203.0 },
		new[] { 1600.0, 2079.0, -235.0 },
		new[] { 1800.0, 1857.0, -264.0 },
		new[] { 2000.0, 1352.0, -301.0 },
		new[] { 2100.0, 1100.0, -320.0 },
	};

   
}