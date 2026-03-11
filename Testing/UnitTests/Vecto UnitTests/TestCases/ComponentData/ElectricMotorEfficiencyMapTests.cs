using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class ElectricMotorEfficiencyMapTests
    {
        static DelaunayMap _dmap1 = new DelaunayMap("map");
        static DelaunayMap _dmap2 = new DelaunayMap("map");
        static DelaunayMap _dmap3 = new DelaunayMap("map");
        static DelaunayMap _dmap4 = new DelaunayMap("map");
        static DelaunayMap _dmap1Equal = new DelaunayMap("map");

        static ElectricMotorEfficiencyMapTests()
        {
            _dmap1.AddPoint(0, 0, 0);
            _dmap1Equal.AddPoint(0, 0, 0);
            _dmap2.AddPoint(0, 0, 1);
            _dmap3.AddPoint(0, 1, 0);
            _dmap4.AddPoint(1, 0, 0);
        }

        public static IEnumerable<object[]> EfficiencyMapNewEqualityTestCases()
        {
            var map1 = new EfficiencyMapNew(_dmap1);
            var map1Equal = new EfficiencyMapNew(_dmap1Equal);
            var map2 = new EfficiencyMapNew(_dmap2);
            var map3 = new EfficiencyMapNew(_dmap3);
            var map4 = new EfficiencyMapNew(_dmap4);

            yield return new object[] { null, null, true };
            yield return new object[] { map1, null, false };
            yield return new object[] { null, map1, false };
            yield return new object[] { map1, map1, true };
            yield return new object[] { map1, map1Equal, true };
            yield return new object[] { map2, map1, false };
            yield return new object[] { map3, map1, false };
            yield return new object[] { map4, map1, false };
        }

        [Test, TestCaseSource(nameof(EfficiencyMapNewEqualityTestCases))]
        public void Test_EfficiencyMapNew_OperatorEqual(EfficiencyMapNew a, EfficiencyMapNew b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EfficiencyMapNewEqualityTestCases))]
        public void Test_EfficiencyMapNew_OperatorNotEqual(EfficiencyMapNew a, EfficiencyMapNew b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EfficiencyMapNewEqualityTestCases))]
        public void Test_EfficiencyMapNew_Equals(EfficiencyMapNew a, EfficiencyMapNew b, bool expected)
        {
            if ((a is null) || (b is null))
            {
                return;
            }

            var result = a.Equals(b);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> EfficiencyMapEqualityTestCases()
        {
            var map1 = new EfficiencyMap(_dmap1);
            var map1Equal = new EfficiencyMap(_dmap1Equal);
            var map2 = new EfficiencyMap(_dmap2);
            var map3 = new EfficiencyMap(_dmap3);
            var map4 = new EfficiencyMap(_dmap4);

            yield return new object[] { null, null, true };
            yield return new object[] { map1, null, false };
            yield return new object[] { null, map1, false };
            yield return new object[] { map1, map1, true };
            yield return new object[] { map1, map1Equal, true };
            yield return new object[] { map2, map1, false };
            yield return new object[] { map3, map1, false };
            yield return new object[] { map4, map1, false };
        }

        [Test, TestCaseSource(nameof(EfficiencyMapEqualityTestCases))]
        public void Test_EfficiencyMap_OperatorEqual(EfficiencyMap a, EfficiencyMap b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EfficiencyMapEqualityTestCases))]
        public void Test_EfficiencyMap_OperatorNotEqual(EfficiencyMap a, EfficiencyMap b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EfficiencyMapEqualityTestCases))]
        public void Test_EfficiencyMap_Equals(EfficiencyMap a, EfficiencyMap b, bool expected)
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
