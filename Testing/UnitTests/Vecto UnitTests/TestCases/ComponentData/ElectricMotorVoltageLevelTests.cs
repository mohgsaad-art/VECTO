using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor.ElectricMotorFullLoadCurve;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class ElectricMotorVoltageLevelTests
    {
        const double Epsilon = 1e-7;

        static ElectricMotorVoltageLevelData _entry1 = new ElectricMotorVoltageLevelData();
        static ElectricMotorVoltageLevelData _entry1Equal = new ElectricMotorVoltageLevelData();
        static ElectricMotorVoltageLevelData _entry2 = new ElectricMotorVoltageLevelData();
        static ElectricMotorVoltageLevelData _entry3 = new ElectricMotorVoltageLevelData();
        static ElectricMotorVoltageLevelData _entry4 = new ElectricMotorVoltageLevelData();
        static ElectricMotorVoltageLevelData _entry1Close = new ElectricMotorVoltageLevelData();
        static ElectricMotorVoltageLevelData _entry1Epsilon = new ElectricMotorVoltageLevelData();

        static FullLoadEntry _fdEntry1 = new FullLoadEntry() { MotorSpeed = 0.SI<PerSecond>(), FullDriveTorque = 0.SI<NewtonMeter>(), FullGenerationTorque = 0.SI<NewtonMeter>() };
        static FullLoadEntry _fdEntry2 = new FullLoadEntry() { MotorSpeed = 1.SI<PerSecond>(), FullDriveTorque = 0.SI<NewtonMeter>(), FullGenerationTorque = 0.SI<NewtonMeter>() };

        static DelaunayMap _dmap1 = new DelaunayMap("map");
        static DelaunayMap _dmap2 = new DelaunayMap("map");

        static ElectricMotorVoltageLevelTests()
        {
            _dmap1.AddPoint(0, 0, 0);
            _dmap2.AddPoint(0, 0, 1);

            PopulateElectricMotorVoltageLevelData(_entry1);
            PopulateElectricMotorVoltageLevelData(_entry1Equal);
            
            PopulateElectricMotorVoltageLevelData(_entry2);
            _entry2.Voltage += 1.SI<Volt>();

            PopulateElectricMotorVoltageLevelData(_entry3);
            _entry3.FullLoadCurve = new ElectricMotorFullLoadCurve(new List<FullLoadEntry>() { _fdEntry2 });

            PopulateElectricMotorVoltageLevelData(_entry4);
            _entry4.EfficiencyMap = new EfficiencyMap(_dmap2);

            PopulateElectricMotorVoltageLevelData(_entry1Close);
            _entry1Close.Voltage = 0.001.SI<Volt>();

            PopulateElectricMotorVoltageLevelData(_entry1Epsilon);
            _entry1Epsilon.Voltage = Epsilon.SI<Volt>();
        }

        static void PopulateElectricMotorVoltageLevelData(ElectricMotorVoltageLevelData ev)
        {
            ev.Voltage = 0.SI<Volt>();

            ev.FullLoadCurve = new ElectricMotorFullLoadCurve(new List<FullLoadEntry>() { _fdEntry1 });

            ev.EfficiencyMap = new EfficiencyMap(_dmap1);
        }

        public static IEnumerable<object[]> ElectricMotorVoltageLevelDataComparerEqualsTestCases()
        {
            yield return new object[] { null, null, true };
            yield return new object[] { _entry1, null, false };
            yield return new object[] { null, _entry1, false };
            yield return new object[] { _entry1, _entry1, true };
            yield return new object[] { _entry1, _entry1Equal, true };
            yield return new object[] { _entry1, _entry2, false };
            yield return new object[] { _entry1, _entry3, false };
            yield return new object[] { _entry1, _entry4, false };
            yield return new object[] { _entry1, _entry1Close, false };
            yield return new object[] { _entry1, _entry1Epsilon, true };
        }

        [Test, TestCaseSource(nameof(ElectricMotorVoltageLevelDataComparerEqualsTestCases))]
        public void Test_ElectricMotorVoltageLevelDataComparer_Equals(ElectricMotorVoltageLevelData a, ElectricMotorVoltageLevelData b, bool expected)
        {
            var result = (new ElectricMotorVoltageLevelDataComparer()).Equals(a, b);

            Assert.That(result, Is.EqualTo(expected));
        }

        public static IEnumerable<object[]> VoltageLevelDataEqualityTestCases()
        {
            var dc1 = new VoltageLevelData() { VoltageLevels = new List<ElectricMotorVoltageLevelData>() { _entry1 } };

            var dc1Equal = new VoltageLevelData() { VoltageLevels = new List<ElectricMotorVoltageLevelData>() { _entry1 } };
            var dc2 = new VoltageLevelData() { VoltageLevels = new List<ElectricMotorVoltageLevelData>() { _entry2 } };
            var dc3 = new VoltageLevelData() { VoltageLevels = new List<ElectricMotorVoltageLevelData>() { _entry3 } };
            var dc4 = new VoltageLevelData() { VoltageLevels = new List<ElectricMotorVoltageLevelData>() { _entry4 } };

            yield return new object[] { null, null, true };
            yield return new object[] { dc1, null, false };
            yield return new object[] { null, dc1, false };
            yield return new object[] { dc1, dc1, true };
            yield return new object[] { dc1, dc1Equal, true };
            yield return new object[] { dc2, dc1, false };
            yield return new object[] { dc3, dc1, false };
            yield return new object[] { dc4, dc1, false };
        }

        [Test, TestCaseSource(nameof(VoltageLevelDataEqualityTestCases))]
        public void Test_VoltageLevelData_OperatorEqual(VoltageLevelData a, VoltageLevelData b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(VoltageLevelDataEqualityTestCases))]
        public void Test_VoltageLevelData_OperatorNotEqual(VoltageLevelData a, VoltageLevelData b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(VoltageLevelDataEqualityTestCases))]
        public void Test_VoltageLevelData_Equals(VoltageLevelData a, VoltageLevelData b, bool expected)
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
