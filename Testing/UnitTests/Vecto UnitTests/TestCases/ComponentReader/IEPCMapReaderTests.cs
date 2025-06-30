using System.Data;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class IEPCMapReaderTests
{
    [TestCase(413.75, -1396.825, -61756.61)]
    [TestCase(827.50, 161.085, 13679.79)]
    public void TestIEPC_EfficiencyCorrection(double rpm, double tq, double expectedPel)
	{
		var ratio = 21.938;

        var input = new Mock<IElectricMotorPowerMap>();
		input.Setup(p => p.PowerMap).Returns(InputDataHelper.InputDataAsTableData(IEPCMapHdr, IEPCMapData));
		var fld = IEPCFullLoadCurveReader.Create(InputDataHelper.InputDataAsTableData(FldHdr, FldData), 1, ratio);
		var map = IEPCMapReader.Create(input.Object.PowerMap, 1, ratio, fld, ExecutionMode.Declaration);

        var entry = map.Entries.First(x => x.Torque.IsEqual(tq, 0.1) && x.MotorSpeed.AsRPM.IsEqual(rpm, 0.1));

        var inputRow = input.Object.PowerMap.AsEnumerable().First(r => r.ParseDouble(IEPCMapReader.Fields.MotorSpeed).IsEqual(rpm / ratio, 0.1) &&
                                                                r.ParseDouble(IEPCMapReader.Fields.Torque).IsEqual(-tq * ratio, 0.1));
        var inputPwrEl = inputRow.ParseDouble(IEPCMapReader.Fields.PowerElectrical);

        // check that in the input the efficiency is greater than 1
        Assert.IsTrue(-inputPwrEl > expectedPel);
        if (tq < 0) {
            // propulsion
            Assert.IsTrue(rpm.RPMtoRad() * -tq.SI<NewtonMeter>() / inputPwrEl > 1);
        } else {
            // recuperation
            Assert.IsTrue(inputPwrEl / (rpm.RPMtoRad() * -tq.SI<NewtonMeter>()) > 1);
        }

        // calculate electric power as the raw map contains a virtual 'torque loss' of the EM.
        var elPower = entry.MotorSpeed * entry.Torque + entry.PowerElectrical.Value().SI<NewtonMeter>() * entry.MotorSpeed;

        // < 0 means propulsion, hence the electric power needs to be 'more negative'
        Assert.IsTrue(entry.MotorSpeed * entry.Torque > elPower);

        Assert.AreEqual(expectedPel, elPower.Value(), 0.1);
    }

	private const string IEPCMapHdr = "n_out  , T_out     , P_el";

	private static readonly string[] IEPCMapData = new[] {
		"0.00, -35338.92, 0.00",
		"0.00, -30038.08, 0.00",
		"0.00, -24737.24, -130.00",
		"0.00, -19436.40, -1190.00",
		"0.00, -14135.57, -1390.00",
		"0.00, -8834.73, -1130.00",
		"0.00, -3533.89, -530.00",
		"0.00, 306.44, 130.00",
		"0.00, 4596.53, 1700.00",
		"0.00, 9193.07, 3580.00",
		"0.00, 13789.60, 5640.00",
		"0.00, 18386.13, 7880.00",
		"0.00, 22982.67, 10290.00",
		"0.00, 27579.20, 12870.00",
		"0.00, 30643.56, 14670.00",
		"18.86, -35338.92, -58100.00",
		"18.86, -30038.08, -49850.00",
		"18.86, -24737.24, -41420.00",
		"18.86, -19436.40, -32810.00",
		"18.86, -14135.57, -24030.00",
		"18.86, -8834.73, -15080.00",
		"18.86, -3533.89, -5960.00",
		"18.86, -353.39, -370.00",
		"18.86, 306.44, 900.00",
		"18.86, 3064.36, 7160.00",
		"18.86, 7660.89, 17710.00",
		"18.86, 12257.42, 28430.00",
		"18.86, 16853.96, 39320.00",
		"18.86, 21450.49, 50380.00",
		"18.86, 26047.02, 61610.00",
		"18.86, 30643.56, 60010.00", // efficiency > 1, this is corrected in VECTO on reading the map
		"37.72, -35338.92, -120570.00",
		"37.72, -30038.08, -102990.00",
		"37.72, -24737.24, -85200.00",
		"37.72, -19436.40, -67210.00",
		"37.72, -14135.57, -49010.00",
		"37.72, -8834.73, -30610.00",
		"37.72, -3533.89, -15010.00", //efficiency > 1, this is corrected in VECTO on reading the map
		"37.72, -353.39, -660.00",
		"37.72, 306.44, 1840.00",
		"37.72, 3064.36, 14220.00",
		"37.72, 7660.89, 34960.00",
		"37.72, 12257.42, 55900.00",
		"37.72, 16853.96, 77050.00",
		"37.72, 21450.49, 98410.00",
		"37.72, 26047.02, 119970.00",
		"37.72, 30643.56, 141740.00",
		"56.59, -35338.92, -182440.00",
		"56.59, -30038.08, -155670.00",
		"56.59, -24737.24, -128650.00",
		"56.59, -19436.40, -101360.00",
		"56.59, -14135.57, -73820.00",
		"56.59, -8834.73, -46020.00",
		"56.59, -3533.89, -17960.00",
		"56.59, -353.39, -790.00",
		"56.59, 306.44, 2860.00",
		"56.59, 3064.36, 21390.00",
		"56.59, 4596.53, 31680.00",
		"56.59, 9193.07, 62710.00",
		"56.59, 13789.60, 94010.00",
		"56.59, 18386.13, 125570.00",
		"56.59, 22982.67, 157400.00",
		"56.59, 27579.20, 189480.00",
		"56.59, 30643.56, 211020.00",
	};

	private const string FldHdr = "n_out   , T_drive_out , T_recuperation_out";

	private static readonly string[] FldData = new[] {
		"0.00, 10314.39, -11414.26",
		"5.67, 10314.39, -11414.26",
		"57.23, 10314.39, -11414.26",
		"114.47, 10314.39, -11414.26",
		"171.56, 10314.39, -11414.26",
		"228.79, 10314.39, -11414.26",
		"286.03, 10314.39, -11414.26",
		"343.26, 8595.33, -9511.89",
		"400.49, 7367.36, -8152.98",
		"457.59, 6446.49, -7133.92",
		"514.82, 5730.26, -6341.31",
		"572.05, 5157.20, -5707.13",
		"629.29, 4688.37, -5188.32",
		"686.52, 4297.66, -4755.94",
		"743.61, 3967.05, -4390.08",
		"800.85, 3683.75, -4076.57",
		"858.08, 3438.13, -3804.75",
		"915.31, 3223.32, -3567.03",
		"972.54, 3033.67, -3357.16",
		"1029.64, 2865.06, -3170.58",
		"1086.87, 2714.33, -3003.78",
		"1144.11, 2578.60, -2853.57",
	};


}