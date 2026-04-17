using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.RetarderLossMap;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class RetarderLossMapTests
    {
        const double Epsilon = 1e-7;
        static RetarderLossEntry _entry1 = new RetarderLossEntry() { RetarderSpeed = 0.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() };
        static RetarderLossEntry _entry1Equal = new RetarderLossEntry() { RetarderSpeed = 0.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() };
        static RetarderLossEntry _entry2 = new RetarderLossEntry() { RetarderSpeed = 1.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() };
        static RetarderLossEntry _entry3 = new RetarderLossEntry() { RetarderSpeed = 0.SI<PerSecond>(), TorqueLoss = 1.SI<NewtonMeter>() };
        static RetarderLossEntry _entry1Close = new RetarderLossEntry() { RetarderSpeed = 0.001.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() };
        static RetarderLossEntry _entry1Epsilon = new RetarderLossEntry() { RetarderSpeed = Epsilon.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() };

        public static IEnumerable<object[]> RetarderLossEntryComparerEqualsTestCases()
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

        [Test, TestCaseSource(nameof(RetarderLossEntryComparerEqualsTestCases))]
        public void Test_RetarderLossEntryComparer_Equals(RetarderLossEntry a, RetarderLossEntry b, bool expected)
        {
            var result = (new RetarderLossEntryComparer()).Equals(a, b);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> EqualityTestCases()
        {
            var map1 = new RetarderLossMap(new RetarderLossEntry[1] { _entry1 });
            var map1Equal = new RetarderLossMap(new RetarderLossEntry[1] { _entry1 });
            var map2 = new RetarderLossMap(new RetarderLossEntry[1] { _entry2 });
            var map3 = new RetarderLossMap(new RetarderLossEntry[1] { _entry3 });
            var map4 = new RetarderLossMap(new RetarderLossEntry[2] { _entry1, _entry1 });

            yield return new object[] { null, null, true };
            yield return new object[] { map1, null, false };
            yield return new object[] { null, map1, false };
            yield return new object[] { map1, map1, true };
            yield return new object[] { map1, map1Equal, true };
            yield return new object[] { map2, map1, false };
            yield return new object[] { map3, map1, false };
            yield return new object[] { map4, map1, false };
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_RetarderLossMap_OperatorEqual(RetarderLossMap a, RetarderLossMap b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_RetarderLossMap_OperatorNotEqual(RetarderLossMap a, RetarderLossMap b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_RetarderLossMap_Equals(RetarderLossMap a, RetarderLossMap b, bool expected)
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
