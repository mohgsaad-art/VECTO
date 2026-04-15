using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.PTOLossMap;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class PTODataTests
    {
        static Entry _entry1 = new Entry() { EngineSpeed = 0.SI<PerSecond>(), PTOTorque = 0.SI<NewtonMeter>() };
        static Entry _entry2 = new Entry() { EngineSpeed = 1.SI<PerSecond>(), PTOTorque = 0.SI<NewtonMeter>() };

        private static PTOData CreatePTOData()
        {
            return new PTOData() 
            {
                TransmissionType = "A",
                TransmissionPowerDemand = 0.SI<Watt>(),
                TransmissionPowerDemandElectrical = 0.SI<Watt>(),
                LossMap = new PTOLossMap(new Entry[1] { _entry1 }),
                ConsumerType = PTOConsumerType.electrical
            };
        }

        public static IEnumerable<object[]> EqualityTestCases()
        {
            var dc1 = CreatePTOData();
            var dc1Equal = CreatePTOData();

            yield return new object[] { null, null, true };
            yield return new object[] { dc1, null, false };
            yield return new object[] { null, dc1, false };
            yield return new object[] { dc1, dc1, true };
            yield return new object[] { dc1, dc1Equal, true };

            const int TOTAL_PROPERTIES = 5;

            List<PTOData> diffs = new List<PTOData>();

            for (int i = 0; i < TOTAL_PROPERTIES; i++)
            {
                diffs.Add(CreatePTOData());
            }

            diffs[0].TransmissionType += "A";
            diffs[1].TransmissionPowerDemand += 1.SI<Watt>();
            diffs[2].TransmissionPowerDemandElectrical += 1.SI<Watt>();
            diffs[3].LossMap = new PTOLossMap(new Entry[1] { _entry2 });
            diffs[4].ConsumerType = PTOConsumerType.mechanical;

            foreach (var g in diffs)
            {
                yield return new object[] { g, dc1, false };
            }
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_PTOData_OperatorEqual(PTOData a, PTOData b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_PTOData_OperatorNotEqual(PTOData a, PTOData b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_PTOData_Equals(PTOData a, PTOData b, bool expected)
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
