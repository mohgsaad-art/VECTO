using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.RetarderLossMap;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class RetarderDataTests
    {
        public static IEnumerable<object[]> EqualityTestCases()
        {
            var dc1 = new RetarderData() { 
                Ratio = 0, 
                Type = RetarderType.None, 
                LossMap = new RetarderLossMap(new RetarderLossEntry[1] { new RetarderLossEntry() { RetarderSpeed = 0.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() } }) 
            };

            var dc1Equal = new RetarderData()
            {
                Ratio = 0,
                Type = RetarderType.None,
                LossMap = new RetarderLossMap(new RetarderLossEntry[1] { new RetarderLossEntry() { RetarderSpeed = 0.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() } })
            };

            var dc2 = new RetarderData()
            {
                Ratio = 1,
                Type = RetarderType.None,
                LossMap = new RetarderLossMap(new RetarderLossEntry[1] { new RetarderLossEntry() { RetarderSpeed = 0.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() } })
            };
            var dc3 = new RetarderData()
            {
                Ratio = 0,
                Type = RetarderType.EngineRetarder,
                LossMap = new RetarderLossMap(new RetarderLossEntry[1] { new RetarderLossEntry() { RetarderSpeed = 0.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() } })
            }; 

            var dc4 = new RetarderData()
            {
                Ratio = 0,
                Type = RetarderType.None,
                LossMap = new RetarderLossMap(new RetarderLossEntry[1] { new RetarderLossEntry() { RetarderSpeed = 1.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() } })
            };

            yield return new object[] { null, null, true };
            yield return new object[] { dc1, null, false };
            yield return new object[] { null, dc1, false };
            yield return new object[] { dc1, dc1, true };
            yield return new object[] { dc1, dc1Equal, true };
            yield return new object[] { dc1, dc2, false };
            yield return new object[] { dc1, dc3, false };
            yield return new object[] { dc1, dc4, false };
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_RetarderData_OperatorEqual(RetarderData a, RetarderData b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_RetarderData_OperatorNotEqual(RetarderData a, RetarderData b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_RetarderData_Equals(RetarderData a, RetarderData b, bool expected)
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
