using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData
{
    public class ShiftStrategyParameterTests
    {
        const int TOTAL_PROPERTIES = 26;
        static ShiftStrategyParameters _ssp1 = new ShiftStrategyParameters();
        static ShiftStrategyParameters _ssp1Equal = new ShiftStrategyParameters();
        static ShiftStrategyParameters[] _ssps = new ShiftStrategyParameters[TOTAL_PROPERTIES];
        
        static void PopulateData(ShiftStrategyParameters ssp)
        {
            ssp.TorqueReserve = 0;
            ssp.TimeBetweenGearshifts = 0.SI<Second>();
            ssp.StartTorqueReserve = 0;
            ssp.StartSpeed = 0.SI<MeterPerSecond>();
            ssp.StartAcceleration = 0.SI<MeterPerSquareSecond>();
            ssp.UpshiftAfterDownshiftDelay = 0.SI<Second>();
            ssp.DownshiftAfterUpshiftDelay = 0.SI<Second>();
            ssp.UpshiftMinAcceleration = 0.SI<MeterPerSquareSecond>();
            ssp.RatingFactorCurrentGear = 0;
            ssp.RatioEarlyDownshiftFC = 0;
            ssp.RatioEarlyUpshiftFC = 0;
            ssp.AllowedGearRangeFC = 0;
            ssp.AccelerationFactor = 0;
            ssp.VelocityDropFactor = 0;
            ssp.GearshiftLines = new TableData();
            PopulateTableData(ssp.GearshiftLines, new[] { new[]{"0.0", "0.0", "0.0"} });
            ssp.LoadstageThresholds = new List<Tuple<double, double>>() { Tuple.Create(0.0, 0.0) };
            ssp.MinEngineSpeedPostUpshift = 0.SI<PerSecond>();
            ssp.ATLookAheadTime = 0.SI<Second>();
            ssp.LoadStageThresoldsUp = new double[1] { 0 };
            ssp.LoadStageThresoldsDown = new double[1] { 0 };
            ssp.ShiftSpeedsTCToLocked = new double[1][] { new double[1] { 0 } };
            ssp.PEV_TargetSpeedBrakeNorm = 0;
            ssp.PEV_DownshiftMinSpeedFactor = 0;
            ssp.PEV_DeRatedDownshiftSpeedFactor = 0;
            ssp.PEV_DownshiftSpeedFactor = 0;
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

        public static IEnumerable<object[]> EqualityTestCases()
        {
            PopulateData(_ssp1);
            PopulateData(_ssp1Equal);

            yield return new object[] { null, null, true };
            yield return new object[] { null, _ssp1, false };
            yield return new object[] { _ssp1, null, false };
            yield return new object[] { _ssp1, _ssp1, true };
            yield return new object[] { _ssp1, _ssp1Equal, true };

            for (int i = 0; i < TOTAL_PROPERTIES; i++)
            {
                _ssps[i] = new ShiftStrategyParameters();
                PopulateData(_ssps[i]);
            }

            _ssps[0].TorqueReserve = 1;
            _ssps[1].TimeBetweenGearshifts = 1.SI<Second>();
            _ssps[2].StartTorqueReserve = 1;
            _ssps[3].StartSpeed = 1.SI<MeterPerSecond>();
            _ssps[4].StartAcceleration = 1.SI<MeterPerSquareSecond>();
            _ssps[5].UpshiftAfterDownshiftDelay = 1.SI<Second>();
            _ssps[6].DownshiftAfterUpshiftDelay = 1.SI<Second>();
            _ssps[7].DownshiftAfterUpshiftDelay = 1.SI<Second>();
            _ssps[8].UpshiftMinAcceleration = 1.SI<MeterPerSquareSecond>();
            _ssps[9].RatingFactorCurrentGear = 1;
            _ssps[10].RatioEarlyDownshiftFC = 1;
            _ssps[11].RatioEarlyUpshiftFC = 1;
            _ssps[12].AllowedGearRangeFC = 1;
            _ssps[13].AccelerationFactor = 1;
            _ssps[14].VelocityDropFactor = 1;
            _ssps[15].GearshiftLines = new TableData();
            PopulateTableData(_ssps[15].GearshiftLines, new[] { new[] { "1.0", "0.0", "0.0" } });
            _ssps[16].LoadstageThresholds = new List<Tuple<double, double>>() { Tuple.Create(1.0, 0.0) };
            _ssps[17].MinEngineSpeedPostUpshift = 1.SI<PerSecond>();
            _ssps[18].ATLookAheadTime = 1.SI<Second>();
            _ssps[19].LoadStageThresoldsUp = new double[1] { 1 };
            _ssps[20].LoadStageThresoldsDown = new double[1] { 1 };
            _ssps[21].ShiftSpeedsTCToLocked = new double[1][] { new double[1] { 1 } };
            _ssps[22].PEV_TargetSpeedBrakeNorm = 1;
            _ssps[23].PEV_DownshiftMinSpeedFactor = 1;
            _ssps[24].PEV_DeRatedDownshiftSpeedFactor = 1;
            _ssps[25].PEV_DownshiftSpeedFactor = 1;

            for (int i = 0; i < TOTAL_PROPERTIES; i++)
            {
                yield return new object[] { _ssp1, _ssps[i], false };
            }
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_ShiftStrategyParameters_OperatorNotEqual(ShiftStrategyParameters a, ShiftStrategyParameters b, bool expected)
        {
            var result = (a == b);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_ShiftStrategyParameters_OperatorEqual(ShiftStrategyParameters a, ShiftStrategyParameters b, bool notExpected)
        {
            var result = (a != b);

            Assert.That(result, Is.EqualTo(!notExpected));
        }

        [Test, TestCaseSource(nameof(EqualityTestCases))]
        public void Test_ShiftStrategyParameters_Equals(ShiftStrategyParameters a, ShiftStrategyParameters b, bool expected)
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
