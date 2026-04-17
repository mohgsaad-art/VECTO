using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox.ShiftPolygon;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox.TransmissionLossMap;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class GearboxDataTests
    {
        static GearLossMapEntry _entry1 = new GearLossMapEntry(0.SI<PerSecond>(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry2 = new GearLossMapEntry(1.SI<PerSecond>(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry3 = new GearLossMapEntry(0.SI<PerSecond>(), 1.SI<NewtonMeter>(), 0.SI<NewtonMeter>());

        static ShiftPolygonEntry _spEntry1 = new ShiftPolygonEntry(0.SI<NewtonMeter>(), 0.SI<PerSecond>());
        static TorqueConverterEntry _tqEntry1 = new TorqueConverterEntry() { SpeedRatio = 0, Torque = 0.SI<NewtonMeter>(), TorqueRatio = 0 };

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

        private static GearboxData CreateGearboxData()
        {
            return new GearboxData() {
                Type = GearboxType.ATSerial,
                Gears = new Dictionary<uint, GearData>() { { 1, CreateGearData() } },
                TorqueConverterData = new TorqueConverterData(new List<TorqueConverterEntry>() { _tqEntry1 }, 0.SI<PerSecond>(), 0.SI<PerSecond>(), 0.SI<MeterPerSquareSecond>(), 0.SI<MeterPerSquareSecond>()),
                Inertia = 0.SI<KilogramSquareMeter>(),
                TractionInterruption = 0.SI<Second>(),
                PowershiftShiftTime = 0.SI<Second>(),
                ATEcoRollReleaseLockupClutch = false,
                DisengageWhenHaltingSpeed = 0.SI<MeterPerSecond>(),
                ShiftStrategy = "A"
            };
        }

        public static IEnumerable<object[]> EqualityTestCases()
        {
            var dc1 = CreateGearboxData();
            var dc1Equal = CreateGearboxData();
            
            yield return new object[] { null, null, true };
            yield return new object[] { dc1, null, false };
            yield return new object[] { null, dc1, false };
            yield return new object[] { dc1, dc1, true };
            yield return new object[] { dc1, dc1Equal, true };
            
            const int TOTAL_PROPERTIES = 9;

            List<GearboxData> gds = new List<GearboxData>();

            for (int i = 0; i < TOTAL_PROPERTIES; i++)
            {
                gds.Add(CreateGearboxData());
            }

            gds[0].Type = GearboxType.APTN;
            gds[1].Gears.First().Value.Ratio += 1;
            gds[2].TorqueConverterData.CLUpshiftMinAcceleration += 1.SI<MeterPerSquareSecond>();
            gds[3].Inertia += 1.SI<KilogramSquareMeter>();
            gds[4].TractionInterruption += 1.SI<Second>();
            gds[5].PowershiftShiftTime += 1.SI<Second>();
            gds[6].ATEcoRollReleaseLockupClutch = !gds[6].ATEcoRollReleaseLockupClutch;
            gds[7].DisengageWhenHaltingSpeed += 1.SI<MeterPerSecond>();
            gds[8].ShiftStrategy += "A";

            foreach (var g in gds)
            {
                yield return new object[] { g, dc1, false };
            }
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_GearboxData_OperatorEqual(GearboxData a, GearboxData b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_GearboxData_OperatorNotEqual(GearboxData a, GearboxData b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_GearboxData_Equals(GearboxData a, GearboxData b, bool expected)
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
