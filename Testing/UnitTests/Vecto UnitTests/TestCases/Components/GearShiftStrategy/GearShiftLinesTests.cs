using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.Vecto.UnitTests.TestCases.Components.GearShiftStrategy;

public class GearShiftLinesTests
{
    [TestCase]
    public void IntersectShiftLines1()
    {
        var upShift = new[] {
                new Point(10, 0),
                new Point(10, 10),
                new Point(20, 20),
            };

        var transformed = new[] {
                new Point(8, 0),
                new Point(8, 8),
                new Point(18, 22)
            };

        var expected = new[] {
                new Point(10, 0),
                new Point(10, 10),
                new Point(20, 20),
            };

        var result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(upShift, transformed);

        Assert.AreEqual(expected.Length, result.Length);

        foreach (var tuple in expected.Zip(result, Tuple.Create)) {
            Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
            Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
        }
    }

    [TestCase]
    public void IntersectShiftLines2()
    {
        var upShift = new[] {
                new Point(10, 0),
                new Point(10, 10),
                new Point(20, 20),
            };

        var transformed = new[] {
                new Point(8, 0),
                new Point(8, 6),
                new Point(18, 14)
            };

        var expected = new[] {
                new Point(10, 0),
                new Point(10, 7.6),
                new Point(20, 15.6),
            };

        var result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(upShift, transformed);

        Assert.AreEqual(expected.Length, result.Length);

        foreach (var tuple in expected.Zip(result, Tuple.Create)) {
            Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
            Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
        }

        result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(transformed, upShift);

        Assert.AreEqual(expected.Length, result.Length);

        foreach (var tuple in expected.Zip(result, Tuple.Create)) {
            Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
            Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
        }
    }

    [TestCase]
    public void IntersectShiftLines3()
    {
        var upShift = new[] {
                new Point(10, 0),
                new Point(10, 10),
                new Point(20, 20),
            };

        var transformed = new[] {
                new Point(8, 0),
                new Point(8, 4),
                new Point(18, 22)
            };

        var expected = new[] {
                new Point(10, 0),
                new Point(10, 7.6),
                new Point(13, 13),
                new Point(20, 20),
            };

        var result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(upShift, transformed);

        Assert.AreEqual(expected.Length, result.Length);

        foreach (var tuple in expected.Zip(result, Tuple.Create)) {
            Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
            Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
        }

        result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(transformed, upShift);

        Assert.AreEqual(expected.Length, result.Length);

        foreach (var tuple in expected.Zip(result, Tuple.Create)) {
            Assert.AreEqual(tuple.Item1.X, tuple.Item2.X);
            Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y);
        }
    }

    [TestCase]
    public void IntersectShiftLines4()
    {
        var upShift = new[] {
                new Point(10, 0),
                new Point(10, 10),
                new Point(20, 20),
            };

        var transformed = new[] {
                new Point(8, 0),
                new Point(8, 12),
                new Point(18, 16)
            };

        var expected = new[] {
                new Point(10, 0),
                new Point(10, 10),
                new Point(14.6666, 14.6666),
                new Point(20, 16.8),
            };

        var result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(upShift, transformed);

        Assert.AreEqual(expected.Length, result.Length);

        foreach (var tuple in expected.Zip(result, Tuple.Create)) {
            Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
            Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
        }

        result = DeclarationData.Gearbox.IntersectTakeHigherShiftLine(transformed, upShift);

        Assert.AreEqual(expected.Length, result.Length);

        foreach (var tuple in expected.Zip(result, Tuple.Create)) {
            Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
            Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
        }
    }

	[TestCase]
	public void LimitShiftlines1()
	{
		var upShift = new[] {
			new Point(10, 0),
			new Point(10, 10),
			new Point(20, 20),
		};

		var limit = new[] {
			new Point(8, 0),
			new Point(8, 20)
		};

		var expected = new[] {
			new Point(8, 0),
			new Point(8, 20)
		};

		var result = DeclarationData.Gearbox.IntersectTakeLowerShiftLine(upShift, limit);

		Assert.AreEqual(expected.Length, result.Length);

		foreach (var tuple in expected.Zip(result, Tuple.Create)) {
			Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
			Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
		}
	}

	[TestCase]
	public void LimitShiftlines2()
	{
		var upShift = new[] {
			new Point(10, 0),
			new Point(10, 10),
			new Point(20, 20),
		};

		var limit = new[] {
			new Point(15, 0),
			new Point(15, 20)
		};

		var expected = new[] {
			new Point(10, 0),
			new Point(10, 10),
			new Point(15, 15),
			new Point(15, 20),
		};

		var result = DeclarationData.Gearbox.IntersectTakeLowerShiftLine(upShift, limit);

		Assert.AreEqual(expected.Length, result.Length);

		foreach (var tuple in expected.Zip(result, Tuple.Create)) {
			Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
			Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
		}
	}

	[TestCase]
	public void LimitShiftlines3()
	{
		var upShift = new[] {
			new Point(10, 0),
			new Point(10, 10),
			new Point(20, 20),
		};

		var limit = new[] {
			new Point(25, 0),
			new Point(25, 20)
		};

		var expected = new[] {
			new Point(10, 0),
			new Point(10, 10),
			new Point(20, 20),
		};

		var result = DeclarationData.Gearbox.IntersectTakeLowerShiftLine(upShift, limit);

		Assert.AreEqual(expected.Length, result.Length);

		foreach (var tuple in expected.Zip(result, Tuple.Create)) {
			Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
			Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
		}
	}

	[TestCase]
	public void ShiftPolygonFldMarginTest()
	{
		var engineFld = new[] {
			new Point(8, 10),
			new Point(9, 11),
			new Point(10, 11.5),
			new Point(11, 12),
			new Point(12, 12.5),
			new Point(13, 13),
			new Point(14, 13.5),
			new Point(15, 14),
			new Point(16, 14.5),
			new Point(17, 15),
			new Point(18, 16.5),
			new Point(19, 18),
			new Point(20, 19.5),
			new Point(21, 21),
			new Point(22, 22.5),
		};

		var expected = new[] {
			new Point(8, 9.8),
			new Point(9, 10.78),
			new Point(10, 11.27),
			new Point(11, 11.76),
			new Point(12, 12.25),
			new Point(13, 12.74),
			new Point(14, 13.23),
			new Point(15, 13.72),
			new Point(16, 14.21),
			new Point(17, 14.7),
			new Point(18, 16.17),
			new Point(19, 17.64),
			new Point(20, 19.11),
			new Point(21, 20.58),
			new Point(22, 22.05),
		};

		var result =
			DeclarationData.Gearbox.ShiftPolygonFldMargin(
				engineFld.Select(
					p =>
						new EngineFullLoadCurve.FullLoadCurveEntry() {
							EngineSpeed = p.X.SI<PerSecond>(),
							TorqueFullLoad = p.Y.SI<NewtonMeter>()
						}).ToList(),
				23.SI<PerSecond>());

		foreach (var tuple in expected.Zip(result, Tuple.Create)) {
			Assert.AreEqual(tuple.Item1.X, tuple.Item2.X, 1e-3);
			Assert.AreEqual(tuple.Item1.Y, tuple.Item2.Y, 1e-3);
		}
	}

	[
		TestCase(false, 650, 400),
		TestCase(true, 400, 500),
		TestCase(false, 900, 400),
		TestCase(false, 1200, 400),
		TestCase(true, 600, 900),
		TestCase(false, 1000, 900),
		TestCase(false, 1200, 900),
		TestCase(true, 300, 1300),
		TestCase(true, 900, 1300),
		TestCase(false, 1200, 1250),
		TestCase(false, 1200, 1600),
	]
	public void IsLeftOf_Test(bool result, double speed, double torque)
	{
		var segment = (new ShiftPolygon.ShiftPolygonEntry(550.SI<NewtonMeter>(), 685.RPMtoRad()),
			new ShiftPolygon.ShiftPolygonEntry(1200.SI<NewtonMeter>(), 1080.RPMtoRad()));

		Assert.AreEqual(result, ShiftPolygon.IsLeftOf(speed.RPMtoRad(), torque.SI<NewtonMeter>(), segment));
	}

    [TestCase]
	public void CorrectDownShiftByEngineFldTest()
	{
		var downshift = Edge.Create(new Point(10, 10), new Point(22, 20));
		var engineFldCorr = new[] {
			new Point(8, 9.8),
			new Point(9, 10.78),
			new Point(10, 11.27),
			new Point(11, 11.76),
			new Point(12, 12.25),
			new Point(13, 12.74),
			new Point(14, 13.23),
			new Point(15, 13.72),
			new Point(16, 14.21),
			new Point(17, 14.7),
			new Point(18, 16.17),
			new Point(19, 17.64),
			new Point(20, 19.11),
			new Point(21, 20.58),
			new Point(22, 22.05),
		};

		var corrected = DeclarationData.Gearbox.MoveDownshiftBelowFld(downshift, engineFldCorr, 20.SI<NewtonMeter>());

		Assert.AreEqual(10, corrected.P1.X, 1e-3);
		Assert.AreEqual(8.86666, corrected.P1.Y, 1e-3);
		Assert.AreEqual(23.36, corrected.P2.X, 1e-3);
		Assert.AreEqual(20, corrected.P2.Y, 1e-3);
	}
}