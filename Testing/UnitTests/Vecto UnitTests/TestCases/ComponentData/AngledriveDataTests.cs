using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox.TransmissionLossMap;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class AngledriveDataTests
    {
        static GearLossMapEntry _entry1 = new GearLossMapEntry(0.SI<PerSecond>(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry2 = new GearLossMapEntry(1.SI<PerSecond>(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry3 = new GearLossMapEntry(0.SI<PerSecond>(), 1.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry4 = new GearLossMapEntry(0.SI<PerSecond>(), 0.SI<NewtonMeter>(), 1.SI<NewtonMeter>());

        public static IEnumerable<object[]> EqualityTestCases()
        {
            var dc1 = new AngledriveData() { 
                Type = AngledriveType.None, 
                Angledrive = new TransmissionData() { Ratio = 0, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true) } 
            };
            
            var dc1Equal = new AngledriveData()
            {
                Type = AngledriveType.None,
                Angledrive = new TransmissionData() { Ratio = 0, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true) }
            };

            var dc2 = new AngledriveData()
            {
                Type = AngledriveType.SeparateAngledrive,
                Angledrive = new TransmissionData() { Ratio = 0, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true) }
            };

            var dc3 = new AngledriveData()
            {
                Type = AngledriveType.None,
                Angledrive = new TransmissionData() { Ratio = 0, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry4, _entry2, _entry3 }, 1, "1", true) }
            };

            yield return new object[] { null, null, true };
            yield return new object[] { dc1, null, false };
            yield return new object[] { null, dc1, false };
            yield return new object[] { dc1, dc1, true };
            yield return new object[] { dc1, dc1Equal, true };
            yield return new object[] { dc2, dc1, false };
            yield return new object[] { dc3, dc1, false };
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_AngledriveData_OperatorEqual(AngledriveData a, AngledriveData b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_AngledriveData_OperatorNotEqual(AngledriveData a, AngledriveData b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_AngledriveData_Equals(AngledriveData a, AngledriveData b, bool expected)
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
