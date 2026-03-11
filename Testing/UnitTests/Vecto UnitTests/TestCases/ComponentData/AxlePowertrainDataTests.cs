using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox.ShiftPolygon;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox.TransmissionLossMap;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.PTOLossMap;
using static TUGraz.VectoCore.Models.SimulationComponent.Data.RetarderLossMap;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class AxlePowertrainDataTests
    {
        static GearLossMapEntry _entry1 = new GearLossMapEntry(0.SI<PerSecond>(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry2 = new GearLossMapEntry(1.SI<PerSecond>(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>());
        static GearLossMapEntry _entry3 = new GearLossMapEntry(0.SI<PerSecond>(), 1.SI<NewtonMeter>(), 0.SI<NewtonMeter>());

        static ShiftPolygonEntry _spEntry1 = new ShiftPolygonEntry(0.SI<NewtonMeter>(), 0.SI<PerSecond>());
        static TorqueConverterEntry _tqEntry1 = new TorqueConverterEntry() { SpeedRatio = 0, Torque = 0.SI<NewtonMeter>(), TorqueRatio = 0 };
        static Entry _ptoEntry1 = new Entry() { EngineSpeed = 0.SI<PerSecond>(), PTOTorque = 0.SI<NewtonMeter>() };

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
            return new GearboxData()
            {
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

        static void PopulateTableData(TableData td, string[][] values)
        {
            foreach (var col in values.First())
            {
                td.Columns.Add(new DataColumn());
            }

            foreach (var row in values)
            {
                td.Rows.Add(td.NewRow().ItemArray = row);
            }
        }

        private static ShiftStrategyParameters CreateShiftStrategyParameters()
        {
            var ssp = new ShiftStrategyParameters()
            {
                TorqueReserve = 0,
                TimeBetweenGearshifts = 0.SI<Second>(),
                StartTorqueReserve = 0,
                StartSpeed = 0.SI<MeterPerSecond>(),
                StartAcceleration = 0.SI<MeterPerSquareSecond>(),
                UpshiftAfterDownshiftDelay = 0.SI<Second>(),
                DownshiftAfterUpshiftDelay = 0.SI<Second>(),
                UpshiftMinAcceleration = 0.SI<MeterPerSquareSecond>(),
                RatingFactorCurrentGear = 0,
                RatioEarlyDownshiftFC = 0,
                RatioEarlyUpshiftFC = 0,
                AllowedGearRangeFC = 0,
                AccelerationFactor = 0,
                VelocityDropFactor = 0,
                GearshiftLines = new TableData(),
                LoadstageThresholds = new List<Tuple<double, double>>() { Tuple.Create(0.0, 0.0) },
                MinEngineSpeedPostUpshift = 0.SI<PerSecond>(),
                ATLookAheadTime = 0.SI<Second>(),
                LoadStageThresoldsUp = new double[1] { 0 },
                LoadStageThresoldsDown = new double[1] { 0 },
                ShiftSpeedsTCToLocked = new double[1][] { new double[1] { 0 } },
                PEV_TargetSpeedBrakeNorm = 0,
                PEV_DownshiftMinSpeedFactor = 0,
                PEV_DeRatedDownshiftSpeedFactor = 0,
                PEV_DownshiftSpeedFactor = 0
            };

            PopulateTableData(ssp.GearshiftLines, new[] { new[] { "0.0", "0.0", "0.0" } });

            return ssp;
        }

        private static AxlePowertrainData CreateAxlePowertrainData()
        {
            var axpt = new AxlePowertrainData()
            {
                Architecture = ArchitectureID.F4,
                GearboxData = CreateGearboxData(),
                AxleGearData = new AxleGearData()
                {
                    LineType = AxleLineType.SinglePortalAxle,
                    AxleGear = new TransmissionData() { Ratio = 0, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true) }
                },
                AngledriveData = new AngledriveData()
                {
                    Type = AngledriveType.None,
                    Angledrive = new TransmissionData() { Ratio = 0, LossMap = new TransmissionLossMap(new List<GearLossMapEntry>() { _entry1, _entry2, _entry3 }, 1, "1", true) }
                },
                Retarder = new RetarderData()
                {
                    Ratio = 0,
                    Type = RetarderType.None,
                    LossMap = new RetarderLossMap(new RetarderLossEntry[1] { new RetarderLossEntry() { RetarderSpeed = 0.SI<PerSecond>(), TorqueLoss = 0.SI<NewtonMeter>() } })
                },
                PTO = new PTOData()
                {
                    TransmissionType = "A",
                    TransmissionPowerDemand = 0.SI<Watt>(),
                    TransmissionPowerDemandElectrical = 0.SI<Watt>(),
                    LossMap = new PTOLossMap(new Entry[1] { _ptoEntry1 }),
                    ConsumerType = PTOConsumerType.electrical
                },
                GearshiftParameters = CreateShiftStrategyParameters(),
                ShiftStrategy = "A",
            };

            var em = new ElectricMotorData();
            em.Inertia = 0.SI<KilogramSquareMeter>();
            axpt.ElectricMachinesData.Add(Tuple.Create(PowertrainPosition.BatteryElectricE4, em));

            return axpt;
        }

        public static IEnumerable<object[]> EqualityTestCases()
        {
            var dc1 = CreateAxlePowertrainData();
            var dc1Equal = CreateAxlePowertrainData();

            yield return new object[] { null, null, true };
            yield return new object[] { dc1, null, false };
            yield return new object[] { null, dc1, false };
            yield return new object[] { dc1, dc1, true };
            yield return new object[] { dc1, dc1Equal, true };

            const int TOTAL_PROPERTIES = 9;

            List<AxlePowertrainData> diffs = new List<AxlePowertrainData>();

            for (int i = 0; i < TOTAL_PROPERTIES; i++)
            {
                diffs.Add(CreateAxlePowertrainData());
            }

            diffs[0].Architecture = ArchitectureID.F3;
            diffs[1].GearboxData.DisengageWhenHaltingSpeed += 1.SI<MeterPerSecond>();
            diffs[2].AxleGearData.LineType = AxleLineType.HubReductionTandemAxle;
            diffs[3].AngledriveData.Type = AngledriveType.LossesIncludedInGearbox;
            diffs[4].Retarder.Ratio += 1;
            diffs[5].PTO.ConsumerType = PTOConsumerType.mechanical;
            diffs[6].GearshiftParameters.StartTorqueReserve += 1;
            diffs[7].ShiftStrategy += "A";
            diffs[8].ElectricMachinesData.First().Item2.Inertia += 1.SI<KilogramSquareMeter>();

            foreach (var g in diffs)
            {
                yield return new object[] { g, dc1, false };
            }
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_AxlePowertrainData_OperatorEqual(AxlePowertrainData a, AxlePowertrainData b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_AxlePowertrainData_OperatorNotEqual(AxlePowertrainData a, AxlePowertrainData b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_AxlePowertrainData_Equals(AxlePowertrainData a, AxlePowertrainData b, bool expected)
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
