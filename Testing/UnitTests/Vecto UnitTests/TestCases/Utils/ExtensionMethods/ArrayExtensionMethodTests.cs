using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils.ExtensionMethods
{
    public class ArrayExtensionMethodTests
    {
        const double Epsilon = 1e-7;

        public static IEnumerable<object[]> OneDimensionalArrayIsEqualTestCases()
        {
            yield return new object[] { null, null, new DoubleComparer(), true };
            yield return new object[] { null, new double[1] { 0 }, new DoubleComparer(), false };
            yield return new object[] { new double[1] { 0 }, null, new DoubleComparer(), false };
            yield return new object[] { new double[1] { 0 }, new double[1] { 0 }, new DoubleComparer(), true };
            yield return new object[] { new double[2] { 0, 1 }, new double[1] { 0 }, new DoubleComparer(), false };
            yield return new object[] { new double[1] { 0.001 }, new double[1] { 0 }, new DoubleComparer(), false };
            yield return new object[] { new double[1] { Epsilon }, new double[1] { 0 }, new DoubleComparer(), true };
        }

        [Test, TestCaseSource(nameof(OneDimensionalArrayIsEqualTestCases))]
        public void Test_OneDimensionalArrayIsEqual(double[] a, double[] b, IEqualityComparer<double> comparer, bool expected)
        {
            var result = a.IsEqual(b, comparer);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> TwoDimensionalJaggedArrayIsEqualTestCases()
        {
            yield return new object[] { null, null, new DoubleComparer(), true };
            yield return new object[] { null, new double[1][] { new double[1] { 0 } }, new DoubleComparer(), false };
            yield return new object[] { new double[1][] { new double[1] { 0 } }, null, new DoubleComparer(), false };
            yield return new object[] { new double[1][] { new double[1] { 0 } }, new double[1][] { new double[1] { 0 } }, new DoubleComparer(), true };
            yield return new object[] { new double[2][] { new double[1] { 0 }, new double[1] { 1 } }, new double[1][] { new double[1] { 0 } }, new DoubleComparer(), false };
            yield return new object[] { new double[1][] { new double[2] { 0, 1 } }, new double[1][] { new double[1] { 0 } }, new DoubleComparer(), false };
            yield return new object[] { new double[1][] { new double[1] { 0.001 } }, new double[1][] { new double[1] { 0 } }, new DoubleComparer(), false };
            yield return new object[] { new double[1][] { new double[1] { Epsilon } }, new double[1][] { new double[1] { 0 } }, new DoubleComparer(), true };
        }

        [Test, TestCaseSource(nameof(TwoDimensionalJaggedArrayIsEqualTestCases))]
        public void Test_TwoDimensionalJaggedArrayIsEqual(double[][] a, double[][] b, IEqualityComparer<double> comparer, bool expected)
        {
            var result = a.IsEqual(b, comparer);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
