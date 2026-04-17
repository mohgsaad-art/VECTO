using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.DragCurve;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class ElectricMotorDragCurveTests
    {
        const double Epsilon = 1e-7;
        static DragLoadEntry _entry1 = new DragLoadEntry() { MotorSpeed = 0.SI<PerSecond>(), DragTorque = 0.SI<NewtonMeter>() };
        static DragLoadEntry _entry1Equal = new DragLoadEntry() { MotorSpeed = 0.SI<PerSecond>(), DragTorque = 0.SI<NewtonMeter>() };
        static DragLoadEntry _entry2 = new DragLoadEntry() { MotorSpeed = 1.SI<PerSecond>(), DragTorque = 0.SI<NewtonMeter>() };
        static DragLoadEntry _entry3 = new DragLoadEntry() { MotorSpeed = 0.SI<PerSecond>(), DragTorque = 1.SI<NewtonMeter>() };
        static DragLoadEntry _entry1Close = new DragLoadEntry() { MotorSpeed = 0.001.SI<PerSecond>(), DragTorque = 0.SI<NewtonMeter>() };
        static DragLoadEntry _entry1Epsilon = new DragLoadEntry() { MotorSpeed = Epsilon.SI<PerSecond>(), DragTorque = 0.SI<NewtonMeter>() };

        public static IEnumerable<object[]> DragLoadEntryComparerEqualsTestCases()
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

        [Test, TestCaseSource(nameof(DragLoadEntryComparerEqualsTestCases))]
        public void Test_DragLoadEntryComparer_Equals(DragLoadEntry a, DragLoadEntry b, bool expected)
        {
            var result = (new DragLoadEntryComparer()).Equals(a, b);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> EqualityTestCases()
        {
            var dc1 = new DragCurve(new List<DragLoadEntry>() { _entry1 });
            var dc1Equal = new DragCurve(new List<DragLoadEntry>() { _entry1 });
            var dc2 = new DragCurve(new List<DragLoadEntry>() { _entry2 });
            var dc3 = new DragCurve(new List<DragLoadEntry>() { _entry3 });
            var dc4 = new DragCurve(new List<DragLoadEntry>() { _entry1, _entry1 });

            yield return new object[] { null, null, true };
            yield return new object[] { dc1, null, false };
            yield return new object[] { null, dc1, false };
            yield return new object[] { dc1, dc1, true };
            yield return new object[] { dc1, dc1Equal, true };
            yield return new object[] { dc2, dc1, false };
            yield return new object[] { dc3, dc1, false };
            yield return new object[] { dc4, dc1, false };
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_DragCurve_OperatorEqual(DragCurve a, DragCurve b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_DragCurve_OperatorNotEqual(DragCurve   a, DragCurve b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_DragCurve_Equals(DragCurve a, DragCurve b, bool expected)
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
