using NUnit.Framework;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils.ExtensionMethods;

public class DoubleExtensionMethodTests
{
    const double Epsilon = 1e-7;

    [TestCase]
    public void DoubleExtensions_SI()
    {
        var val = 600.RPMtoRad();
        Assert.AreEqual(600 / 60 * 2 * Math.PI, val.Value());

        Assert.IsTrue(0.SI<PerSecond>().HasEqualUnit(val));

        var val2 = 1200.SI(Unit.SI.Rounds.Per.Minute).Cast<PerSecond>();
        val = val * 2;
        Assert.AreEqual(val.Value(), val2.Value(), 1e-12);
        Assert.IsTrue(val.IsEqual(val2, 1e-9.RPMtoRad()));

        val2 = val2 / 2;
        val = val / 2;
        Assert.AreEqual(val.Value(), val2.Value(), 1e-12);
		Assert.IsTrue(val.IsEqual(val2, 1e-9.RPMtoRad()));
        Assert.AreEqual(600.SI(Unit.SI.Rounds.Per.Minute).Cast<PerSecond>(), val2);
        Assert.AreEqual(600.SI(Unit.SI.Rounds.Per.Minute).Cast<PerSecond>().Value(), val2.Value());
    }

    [TestCase(0.0, 0.0, true),
    TestCase(0.0, Epsilon, true),
    TestCase(Epsilon, 0.0, true),
    TestCase(0.0, -Epsilon, true),
    TestCase(-Epsilon, 0.0, true),
    TestCase(0.0, 0.1, false),
    TestCase(0.1, 0.0, false),
    TestCase(0.0, -0.1, false),
    TestCase(-0.1, 0.0, false),]
    public void DoubleExtension_ComparisonOperators_IsEqual(double value, double expected, bool isTrue)
    {
        if (isTrue)
        {
            Assert.IsTrue(value.IsEqual(expected));
        }
        else
        {
            Assert.IsFalse(value.IsEqual(expected));
        }
    }

    [TestCase(1.0, 0.0, true),
    TestCase(1.002, 1.0, true),
    TestCase(1.001, 1.0, true),
    TestCase(1.0, 1.0, false),
    TestCase(0.999, 1.0, false),]
    public void DoubleExtension_ComparisonOperators_IsGreater(double value, double expected, bool isTrue)
    {
        if (isTrue)
        {
            Assert.IsTrue(value.IsGreater(expected));
        }
        else
        {
            Assert.IsFalse(value.IsGreater(expected));
        }
    }

    [TestCase(1.0, 1.0, true),
    TestCase(1.001, 1.0, true),
    TestCase(0.999, 1.0, false),
    TestCase(0.998, 1.0, false),]
    public void DoubleExtension_ComparisonOperators_IsGreaterOrEqual(double value, double expected, bool isTrue)
    {
        if (isTrue)
        {
            Assert.IsTrue(value.IsGreaterOrEqual(expected));
        }
        else
        {
            Assert.IsFalse(value.IsGreaterOrEqual(expected));
        }
    }

    [TestCase(0.0, 1.0, true),
    TestCase(0.998, 1.0, true),
    TestCase(0.999, 1.0, true),
    TestCase(1.0, 1.0, false),
    TestCase(1.0011, 1.0, false),]
    public void DoubleExtension_ComparisonOperators_IsSmaller(double value, double expected, bool isTrue)
    {
        if (isTrue)
        {
            Assert.IsTrue(value.IsSmaller(expected));
        }
        else
        {
            Assert.IsFalse(value.IsSmaller(expected));
        }
    }

    [TestCase(1.0, 1.0, true),
    TestCase(1 + Epsilon, 1.0, true),
    TestCase(1.001, 1.0, false),
    TestCase(0.999, 1.0, true),
    TestCase(0.998, 1.0, true),]
    public void DoubleExtension_ComparisonOperators_IsSmallerOrEqual(double value, double expected, bool isTrue)
    {
        if (isTrue)
        {
            Assert.IsTrue(value.IsSmallerOrEqual(expected));
        }
        else
        {
            Assert.IsFalse(value.IsSmallerOrEqual(expected));
        }
    }

    [TestCase(1.0, true),
    TestCase(0.001, true),
    TestCase(0.0, true),
    TestCase(-Epsilon, true),
    TestCase(-0.001, false),
    TestCase(-0.002, false),]
    public void DoubleExtension_ComparisonOperators_IsPositive(double value, bool isTrue)
    {
        if (isTrue)
        {
            Assert.IsTrue(value.IsPositive());
        }
        else
        {
            Assert.IsFalse(value.IsPositive());
        }
    }

    [TestCase(0.452345, 3u, 1u, "0.452"),
    TestCase(4.52345, 3u, 1u, "4.52"),
    TestCase(45.2345, 3u, 1u, "45.2"),
    TestCase(0.0452345, 3u, 1u, "0.0452"),
    TestCase(0.00452345, 3u, 1u, "0.00452"),
    TestCase(-0.452345, 3u, 1u, "-0.452"),
    TestCase(-4.52345, 3u, 1u, "-4.52"),
    TestCase(-45.2345, 3u, 1u, "-45.2"),
    TestCase(-0.0452345, 3u, 1u, "-0.0452"),
    TestCase(-0.00452345, 3u, 1u, "-0.00452"),
    TestCase(0.0, 3u, 1u, "0.0"),]
    public void DoubleExtensions_ToMinSignificantDigits(double value, uint significant, uint decimals, string expected)
    {
        Assert.AreEqual(expected, value.ToMinSignificantDigits(significant, decimals));
    }
}