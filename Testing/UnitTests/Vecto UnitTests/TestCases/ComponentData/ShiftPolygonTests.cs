using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox.ShiftPolygon;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class ShiftPolygonTests
    {
        const double Epsilon = 1e-7;
        static ShiftPolygonEntry _entry1 = new ShiftPolygonEntry(0.SI<NewtonMeter>(), 0.SI<PerSecond>());
        static ShiftPolygonEntry _entry1Equal = new ShiftPolygonEntry(0.SI<NewtonMeter>(), 0.SI<PerSecond>());
        static ShiftPolygonEntry _entry2 = new ShiftPolygonEntry(1.SI<NewtonMeter>(), 0.SI<PerSecond>());
        static ShiftPolygonEntry _entry3 = new ShiftPolygonEntry(0.SI<NewtonMeter>(), 1.SI<PerSecond>());
        static ShiftPolygonEntry _entry1Close = new ShiftPolygonEntry(0.001.SI<NewtonMeter>(), 0.SI<PerSecond>());
        static ShiftPolygonEntry _entry1Epsilon = new ShiftPolygonEntry(Epsilon.SI<NewtonMeter>(), 0.SI<PerSecond>());

        public static IEnumerable<object[]> ShiftPolygonEntryComparerEqualsTestCases()
        {
            yield return new object[] { null, null, true };
            yield return new object[] { _entry1, null, false };
            yield return new object[] { null, _entry1, false };
            yield return new object[] { _entry1, _entry1, true };
            yield return new object[] { _entry1, _entry1Equal, true };
            yield return new object[] { _entry1, _entry2, false };
            yield return new object[] { _entry1, _entry3, false };
            yield return new object[] { _entry1, _entry1Close, false };
            yield return new object[] { _entry1, _entry1Epsilon, true };
        }

        [Test, TestCaseSource(nameof(ShiftPolygonEntryComparerEqualsTestCases))]
        public void Test_ShiftPolygonEntryComparer_Equals(ShiftPolygonEntry a, ShiftPolygonEntry b, bool expected)
        {
            var result = (new ShiftPolygonEntryComparer()).Equals(a, b);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> EqualityTestCases()
        {
            var dc1 = new ShiftPolygon(new List<ShiftPolygonEntry>() { _entry1 }, new List<ShiftPolygonEntry>() { _entry1 });
            var dc1Equal = new ShiftPolygon(new List<ShiftPolygonEntry>() { _entry1 }, new List<ShiftPolygonEntry>() { _entry1 });
            var dc2 = new ShiftPolygon(new List<ShiftPolygonEntry>() { _entry2 }, new List<ShiftPolygonEntry>() { _entry1 });
            var dc3 = new ShiftPolygon(new List<ShiftPolygonEntry>() { _entry3 }, new List<ShiftPolygonEntry>() { _entry1 });
            var dc4 = new ShiftPolygon(new List<ShiftPolygonEntry>() { _entry1, _entry1 }, new List<ShiftPolygonEntry>() { _entry1 });
            var dc5 = new ShiftPolygon(new List<ShiftPolygonEntry>() { _entry1 }, new List<ShiftPolygonEntry>() { _entry2 });
            var dc6 = new ShiftPolygon(new List<ShiftPolygonEntry>() { _entry1 }, new List<ShiftPolygonEntry>() { _entry3 });
            var dc7 = new ShiftPolygon(new List<ShiftPolygonEntry>() { _entry1 }, new List<ShiftPolygonEntry>() { _entry1, _entry1 });

            yield return new object[] { null, null, true };
            yield return new object[] { dc1, null, false };
            yield return new object[] { null, dc1, false };
            yield return new object[] { dc1, dc1, true };
            yield return new object[] { dc1, dc1Equal, true };
            yield return new object[] { dc2, dc1, false };
            yield return new object[] { dc3, dc1, false };
            yield return new object[] { dc4, dc1, false };
            yield return new object[] { dc5, dc1, false };
            yield return new object[] { dc6, dc1, false };
            yield return new object[] { dc7, dc1, false };
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_ShiftPolygon_OperatorEqual(ShiftPolygon a, ShiftPolygon b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_ShiftPolygon_OperatorNotEqual(ShiftPolygon a, ShiftPolygon b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_ShiftPolygon_Equals(ShiftPolygon a, ShiftPolygon b, bool expected)
        {
            if ((a is null) || (b is null))
            {
                return;
            }

            var result = a.Equals(b);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}