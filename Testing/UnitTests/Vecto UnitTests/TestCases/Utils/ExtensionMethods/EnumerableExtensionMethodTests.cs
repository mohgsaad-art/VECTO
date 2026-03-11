using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.Utils.ExtensionMethods
{
    public class EnumerableExtensionMethodTests
    {
        const double Epsilon = 1e-7;

        static readonly IEnumerable<Tuple<double, double>> listZero = new List<Tuple<double, double>>() { Tuple.Create(0.0, 0.0) };
        static readonly IEnumerable<Tuple<double, double>> listZeroEqual = new List<Tuple<double, double>>() { Tuple.Create(0.0, 0.0) };
        static readonly IEnumerable<Tuple<double, double>> listZeroOne = new List<Tuple<double, double>>() { Tuple.Create(0.0, 1.0) };
        static readonly IEnumerable<Tuple<double, double>> listCloseToZero = new List<Tuple<double, double>>() { Tuple.Create(0.001, 0.001) };
        static readonly IEnumerable<Tuple<double, double>> listEpsilon = new List<Tuple<double, double>>() { Tuple.Create(Epsilon, Epsilon) };

        public static IEnumerable<object[]> EnumerableOfTuplesIsEqualTestCases()
        { 
            yield return new object[] { null, null, new DoubleComparer(), true };
            yield return new object[] { null, listZero, new DoubleComparer(), false };
            yield return new object[] { listZero, null, new DoubleComparer(), false };
            yield return new object[] { listZero, listZero, new DoubleComparer(), true };
            yield return new object[] { listZero, listZeroEqual, new DoubleComparer(), true };
            yield return new object[] { listZeroOne, listZero, new DoubleComparer(), false };
            yield return new object[] { listCloseToZero, listZero, new DoubleComparer(), false };
            yield return new object[] { listEpsilon, listZero, new DoubleComparer(), true };
        }

        [Test, TestCaseSource(nameof(EnumerableOfTuplesIsEqualTestCases))]
        public void Test_EnumerableOfTuplesIsEqual(IEnumerable<Tuple<double, double>> a, IEnumerable<Tuple<double, double>> b, IEqualityComparer<double> comparer, bool expected)
        {
            var result = a.IsEqual(b, comparer);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
