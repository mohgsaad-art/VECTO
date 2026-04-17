using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox.ShiftPolygon;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox.TransmissionLossMap;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class GearDataTests
    {
        static GearLossMapEntry _entry1 = new GearLossMapEntry(0.SI<PerSecond>(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry2 = new GearLossMapEntry(1.SI<PerSecond>(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry3 = new GearLossMapEntry(0.SI<PerSecond>(), 1.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry4 = new GearLossMapEntry(0.SI<PerSecond>(), 0.SI<NewtonMeter>(), 1.SI<NewtonMeter>());

        static ShiftPolygonEntry _spEntry1 = new ShiftPolygonEntry(0.SI<NewtonMeter>(), 0.SI<PerSecond>());
        static ShiftPolygonEntry _spEntry2 = new ShiftPolygonEntry(1.SI<NewtonMeter>(), 0.SI<PerSecond>());

        public static IEnumerable<object[]> TransmissionDataEqualityTestCases()
        {
            var dc1 = new TransmissionData() { Ratio = 0, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true) };
            var dc1Equal = new TransmissionData() { Ratio = 0, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true) };
            var dc2 = new TransmissionData() { Ratio = 1, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true) };
            var dc3 = new TransmissionData() { Ratio = 0, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry4, _entry2, _entry3 }, 1, "1", true) };

            yield return new object[] { null, null, true };
            yield return new object[] { dc1, null, false };
            yield return new object[] { null, dc1, false };
            yield return new object[] { dc1, dc1, true };
            yield return new object[] { dc1, dc1Equal, true };
            yield return new object[] { dc2, dc1, false };
            yield return new object[] { dc3, dc1, false };
        }

        [Test, TestCaseSource(nameof(TransmissionDataEqualityTestCases))]
        public void Test_TransmissionData_OperatorEqual(TransmissionData a, TransmissionData b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(TransmissionDataEqualityTestCases))]
        public void Test_TransmissionData_OperatorNotEqual(TransmissionData a, TransmissionData b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(TransmissionDataEqualityTestCases))]
        public void Test_TransmissionData_Equals(TransmissionData a, TransmissionData b, bool expected)
        {
            if ((a is null) || (b is null))
            {
                return;
            }

            var result = a.Equals(b);

            Assert.That(result, Is.EqualTo(expected));
        }

        private static GearData CreateGearData()
        {
            return new GearData()
            {
                Ratio = 0,
                LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true),
                TorqueConverterRatio = 0,
                TorqueConverterGearLossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true),
                MaxSpeed = 0.SI<PerSecond>(),
                MaxTorque = 0.SI<NewtonMeter>(),
                ShiftPolygon = new ShiftPolygon(new List<ShiftPolygonEntry>() { _spEntry1 }, new List<ShiftPolygonEntry>() { _spEntry1 }),
                ExtendedShiftPolygon = new ShiftPolygon(new List<ShiftPolygonEntry>() { _spEntry1 }, new List<ShiftPolygonEntry>() { _spEntry1 }),
                TorqueConverterShiftPolygon = new ShiftPolygon(new List<ShiftPolygonEntry>() { _spEntry1 }, new List<ShiftPolygonEntry>() { _spEntry1 }),
                DeRatedEmShiftPolygon = new ShiftPolygon(new List<ShiftPolygonEntry>() { _spEntry1 }, new List<ShiftPolygonEntry>() { _spEntry1 })
            };
        }

        public static IEnumerable<object[]> GearDataEqualityTestCases()
        {
            var gd1 = CreateGearData();

            var gd1Equal = CreateGearData();

            yield return new object[] { null, null, true };
            yield return new object[] { gd1, null, false };
            yield return new object[] { null, gd1, false };
            yield return new object[] { gd1, gd1, true };
            yield return new object[] { gd1, gd1Equal, true };

            const int TOTAL_PROPERTIES = 10;

            List<GearData> gearDatas = new List<GearData>(TOTAL_PROPERTIES);

            for (int i = 0; i < TOTAL_PROPERTIES; i++)
            {
                gearDatas.Add(CreateGearData());
            }

            gearDatas[0].Ratio = 1;
            gearDatas[1].LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry4, _entry2, _entry3 }, 1, "1", true);
            gearDatas[2].TorqueConverterRatio = 1;
            gearDatas[3].TorqueConverterGearLossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry4, _entry2, _entry3 }, 1, "1", true);
            gearDatas[4].MaxSpeed = 1.SI<PerSecond>();
            gearDatas[5].MaxTorque = 1.SI<NewtonMeter>();
            gearDatas[6].ShiftPolygon = new ShiftPolygon(new List<ShiftPolygonEntry>() { _spEntry2 }, new List<ShiftPolygonEntry>() { _spEntry2 });
            gearDatas[7].ExtendedShiftPolygon = new ShiftPolygon(new List<ShiftPolygonEntry>() { _spEntry2 }, new List<ShiftPolygonEntry>() { _spEntry2 });
            gearDatas[8].TorqueConverterShiftPolygon = new ShiftPolygon(new List<ShiftPolygonEntry>() { _spEntry2 }, new List<ShiftPolygonEntry>() { _spEntry2 });
            gearDatas[9].DeRatedEmShiftPolygon = new ShiftPolygon(new List<ShiftPolygonEntry>() { _spEntry2 }, new List<ShiftPolygonEntry>() { _spEntry2 });

            foreach (var gd in gearDatas)
            {
                yield return new object[] { gd1, gd, false };
            }
        }

        [Test, TestCaseSource(nameof(GearDataEqualityTestCases))]
        public void Test_GearData_OperatorEqual(GearData a, GearData b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(GearDataEqualityTestCases))]
        public void Test_GearData_OperatorNotEqual(GearData a, GearData b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(GearDataEqualityTestCases))]
        public void Test_GearData_Equals(GearData a, GearData b, bool expected)
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
