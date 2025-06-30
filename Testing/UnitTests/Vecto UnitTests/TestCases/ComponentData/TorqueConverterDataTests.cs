using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class TorqueConverterDataTests
{
    [TestCase]
    public void TestTorqueConverterDataDeclarationMode()
    {
        var tqInput = new[] {
                "0.0,1.80,377.80		",
                "0.1,1.71,365.21		",
                "0.2,1.61,352.62		",
                "0.3,1.52,340.02		",
                "0.4,1.42,327.43		",
                "0.5,1.33,314.84		",
                "0.6,1.23,302.24		",
                "0.7,1.14,264.46		",
                "0.8,1.04,226.68		",
                "0.9,0.95,188.90		",
                "1.0,0.95,0.00			",
                "1.100,0.99,-40.34		",
                "1.222,0.98,-80.34		",
                "1.375,0.97,-136.11	",
                "1.571,0.96,-216.52	",
                "1.833,0.95,-335.19	",
                "2.200,0.94,-528.77	",
                "2.750,0.93,-883.40	",
                "4.400,0.92,-2462.17	",
                "11.000,0.91,-16540.98	",
            };
        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio, MP1000",
                tqInput), 1000.RPMtoRad(), 1600.RPMtoRad(), ExecutionMode.Declaration, 1, 0.1.SI<MeterPerSquareSecond>(),
                0.1.SI<MeterPerSquareSecond>());

        // check input data
        Assert.AreEqual(1.8, tqData.TorqueConverterEntries[0].TorqueRatio, 1e-9);
        Assert.AreEqual(377.8, tqData.TorqueConverterEntries[0].Torque.Value(), 1e-9);
        Assert.AreEqual(0, tqData.TorqueConverterEntries[0].SpeedRatio, 1e-9);

        Assert.AreEqual(0.95, tqData.TorqueConverterEntries[9].TorqueRatio, 1e-9);
        Assert.AreEqual(188.90, tqData.TorqueConverterEntries[9].Torque.Value(), 1e-9);
        Assert.AreEqual(0.9, tqData.TorqueConverterEntries[9].SpeedRatio, 1e-9);

        // check appended data

        var entry = tqData.TorqueConverterEntries.Reverse().Skip(1).First();
        Assert.AreEqual(0.9, entry.TorqueRatio, 1e-9);
        Assert.AreEqual(-1511.2, entry.Torque.Value(), 1e-9);
        Assert.AreEqual(5, entry.SpeedRatio, 1e-9);

        entry = tqData.TorqueConverterEntries.Last();
        Assert.AreEqual(0.85, entry.TorqueRatio, 1e-9);
        Assert.AreEqual(-1511.2 * 4.1 / 4, entry.Torque.Value(), 1e-9);
        Assert.AreEqual(15, entry.SpeedRatio, 1e-9);
    }

    [TestCase]
    public void TestTorqueConverterDataExtendPowersplit()
    {
        var tqInput = new[] {
                "0.0,1.80,377.80		",
                "0.1,1.71,365.21		",
                "0.2,1.61,352.62		",
                "0.3,1.52,340.02		",
                "0.4,1.42,327.43		",
                "0.5,1.33,314.84		",
                "0.6,1.23,302.24		",
                "0.7,1.14,264.46		",
                "0.8,1.04,226.68		",
                "0.9,0.95,188.90		",
                "1.0,0.95,0.00			",
                "1.100,0.99,-40.34		",
                "1.222,0.98,-80.34		",
                "1.375,0.97,-136.11	",
                "1.571,0.96,-216.52	",
                "1.833,0.95,-335.19	",
                "2.200,0.94,-528.77	",
                "2.750,0.93,-883.40	",
                "4.400,0.92,-2462.17	",
                "11.000,0.91,-16540.98	",
            };

        var ratio = 0.75;
        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio, MP1000",
                tqInput), 1000.RPMtoRad(), 1600.RPMtoRad(), ExecutionMode.Declaration, ratio, 0.1.SI<MeterPerSquareSecond>(),
                0.1.SI<MeterPerSquareSecond>());

        // check input data
        Assert.AreEqual(1.8, tqData.TorqueConverterEntries[0].TorqueRatio, 1e-9);
        Assert.AreEqual(377.8, tqData.TorqueConverterEntries[0].Torque.Value(), 1e-9);
        Assert.AreEqual(0, tqData.TorqueConverterEntries[0].SpeedRatio, 1e-9);

        Assert.AreEqual(1.14, tqData.TorqueConverterEntries[7].TorqueRatio, 1e-9);
        Assert.AreEqual(264.46, tqData.TorqueConverterEntries[7].Torque.Value(), 1e-9);
        Assert.AreEqual(0.7, tqData.TorqueConverterEntries[7].SpeedRatio, 1e-9);

        // check appended data

        var entry = tqData.TorqueConverterEntries.Reverse().Skip(1).First();
        Assert.AreEqual(0.9 / ratio, entry.TorqueRatio, 1e-9);
        Assert.AreEqual(-1511.2, entry.Torque.Value(), 1e-9);
        Assert.AreEqual(5 * ratio, entry.SpeedRatio, 1e-9);

        entry = tqData.TorqueConverterEntries.Last();
        Assert.AreEqual(0.85 / ratio, entry.TorqueRatio, 1e-9);
        Assert.AreEqual(-1511.2 * 4.1 / 4, entry.Torque.Value(), 1e-9);
        Assert.AreEqual(15 * ratio, entry.SpeedRatio, 1e-9);
    }

    [TestCase]
    public void TestTorqueConverterDataEngneeringMode_Append()
    {
        var tqInput = new[] {
                "0.0,1.80,377.80		",
                "0.1,1.71,365.21		",
                "0.2,1.61,352.62		",
                "0.3,1.52,340.02		",
                "0.4,1.42,327.43		",
                "0.5,1.33,314.84		",
                "0.6,1.23,302.24		",
                "0.7,1.14,264.46		",
                "0.8,1.04,226.68		",
                "0.9,0.95,188.90		",
            };
        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio, MP1000",
                tqInput), 1000.RPMtoRad(), 1600.RPMtoRad(), ExecutionMode.Engineering, 1, 0.1.SI<MeterPerSquareSecond>(),
                0.1.SI<MeterPerSquareSecond>());

        // check input data
        Assert.AreEqual(1.8, tqData.TorqueConverterEntries[0].TorqueRatio, 1e-9);
        Assert.AreEqual(377.8, tqData.TorqueConverterEntries[0].Torque.Value(), 1e-9);
        Assert.AreEqual(0, tqData.TorqueConverterEntries[0].SpeedRatio, 1e-9);

        Assert.AreEqual(0.95, tqData.TorqueConverterEntries[9].TorqueRatio, 1e-9);
        Assert.AreEqual(188.90, tqData.TorqueConverterEntries[9].Torque.Value(), 1e-9);
        Assert.AreEqual(0.9, tqData.TorqueConverterEntries[9].SpeedRatio, 1e-9);

        // check appended data
        Assert.AreEqual(0.95 * 0.99, tqData.TorqueConverterEntries[10].TorqueRatio, 1e-9);
        Assert.AreEqual(0, tqData.TorqueConverterEntries[10].Torque.Value(), 1e-9);
        Assert.AreEqual(1, tqData.TorqueConverterEntries[10].SpeedRatio, 1e-9);

        var entry = tqData.TorqueConverterEntries.Reverse().Skip(1).First();
        Assert.AreEqual(0.9, entry.TorqueRatio, 1e-9);
        Assert.AreEqual(-1511.2, entry.Torque.Value(), 1e-9);
        Assert.AreEqual(5, entry.SpeedRatio, 1e-9);

        entry = tqData.TorqueConverterEntries.Last();
        Assert.AreEqual(0.85, entry.TorqueRatio, 1e-9);
        Assert.AreEqual(-1511.2 * 4.1 / 4, entry.Torque.Value(), 1e-9);
        Assert.AreEqual(15, entry.SpeedRatio, 1e-9);
    }

    [TestCase]
    public void TestTorqueConverterDataEngineeringMode_Unmodified()
    {
        var tqInput = new[] {
                "0.0,1.80,377.80		",
                "0.1,1.71,365.21		",
                "0.2,1.61,352.62		",
                "0.3,1.52,340.02		",
                "0.4,1.42,327.43		",
                "0.5,1.33,314.84		",
                "0.6,1.23,302.24		",
                "0.7,1.14,264.46		",
                "0.8,1.04,226.68		",
                "0.9,0.95,188.90		",
                "1.0,0.95,0.00			",
                "1.100,0.99,-40.34		",
                "1.222,0.98,-80.34		",
                "1.375,0.97,-136.11	",
                "1.571,0.96,-216.52	",
                "1.833,0.95,-335.19	",
                "2.200,0.94,-528.77	",
                "2.750,0.93,-883.40	",
                "4.400,0.92,-2462.17	",
                "11.000,0.91,-16540.98	",
            };
        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio, MP1000",
                tqInput), 1000.RPMtoRad(), 1600.RPMtoRad(), ExecutionMode.Engineering, 1, 0.1.SI<MeterPerSquareSecond>(),
                0.1.SI<MeterPerSquareSecond>());

        // check input data
        Assert.AreEqual(1.8, tqData.TorqueConverterEntries[0].TorqueRatio, 1e-9);
        Assert.AreEqual(377.8, tqData.TorqueConverterEntries[0].Torque.Value(), 1e-9);
        Assert.AreEqual(0, tqData.TorqueConverterEntries[0].SpeedRatio, 1e-9);

        Assert.AreEqual(0.95, tqData.TorqueConverterEntries[9].TorqueRatio, 1e-9);
        Assert.AreEqual(188.90, tqData.TorqueConverterEntries[9].Torque.Value(), 1e-9);
        Assert.AreEqual(0.9, tqData.TorqueConverterEntries[9].SpeedRatio, 1e-9);

        Assert.AreEqual(0.95, tqData.TorqueConverterEntries[10].TorqueRatio, 1e-9);
        Assert.AreEqual(0, tqData.TorqueConverterEntries[10].Torque.Value(), 1e-9);
        Assert.AreEqual(1, tqData.TorqueConverterEntries[10].SpeedRatio, 1e-9);

        Assert.AreEqual(0.91, tqData.TorqueConverterEntries[19].TorqueRatio, 1e-9);
        Assert.AreEqual(-16540.98, tqData.TorqueConverterEntries[19].Torque.Value(), 1e-9);
        Assert.AreEqual(11, tqData.TorqueConverterEntries[19].SpeedRatio, 1e-9);
    }

    [Test,
    TestCase(30, 10, 227.8707, 1780.6630),
    TestCase(40, 10, 197.9536, 1340.4737),
    TestCase(50, 10, 177.6759, 1076.9007),
    TestCase(60, 10, 162.8219, 901.5900),
    TestCase(70, 10, 151.3747, 776.6864),
    TestCase(70, 30, 259.9515, 2306.1578),
    TestCase(90, 10, 134.7344, 610.8267),
    TestCase(90, 30, 230.4896, 1805.2978),
    TestCase(170, 30, 172.2875, 987.4020),
    TestCase(170, 50, 220.2397, 1626.0613),
    TestCase(190, 30, 164.3260, 892.7507),
    TestCase(190, 50, 209.6016, 1465.6611),
    TestCase(190, 70, 246.4829, 2036.7399),
    TestCase(530.517, 1.930, 59.1582, 36.7119),
    ]
    public void TestTorqueConverterOperatingPoint(double nOut, double Pout, double nInExpected, double tqInExpected)
    {
        var tqLimit = 1600;
        //Assert.IsTrue(nInExpected < tqLimit);
        var tqInput = new[] {
                "0.0,1.80,377.80",
                "0.1,1.71,365.21",
                "0.2,1.61,352.62",
                "0.3,1.52,340.02",
                "0.4,1.42,327.43",
                "0.5,1.33,314.84",
                "0.6,1.23,302.24",
                "0.7,1.14,264.46",
                "0.8,1.04,226.68",
                "0.9,0.95,188.90",
                "1.0,0.95,0.00",
            };

        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
                tqInput), 1000.RPMtoRad(), tqLimit.RPMtoRad(), ExecutionMode.Engineering, 1, 0.1.SI<MeterPerSquareSecond>(),
                0.1.SI<MeterPerSquareSecond>());

        var outAngularSpeed = nOut.RPMtoRad();
        var outTorque = (Pout * 1000).SI<Watt>() / outAngularSpeed;
        var resultList = tqData.FindOperatingPoint(outTorque, outAngularSpeed, 0.SI<PerSecond>());
        Assert.AreEqual(1, resultList.Count);
        var result = resultList[0];

        Assert.AreEqual(outAngularSpeed.Value(), result.OutAngularVelocity.Value(), 1e-3);
        Assert.AreEqual(outTorque.Value(), result.OutTorque.Value(), 1e-3);

        //Debug.WriteLine("n_in: {0}, tq_in: {1}", result.InAngularVelocity.AsRPM, result.InTorque.Value());
        //Assert.IsTrue(result.InAngularVelocity.Value() < 1600.RPMtoRad().Value());
        Assert.AreEqual(nInExpected, result.InAngularVelocity.Value(), 1e-3);
        Assert.AreEqual(tqInExpected, result.InTorque.Value(), 1e-3);
    }

    [Test,
    TestCase(898, 463)]
    public void TestTorqueConverterOperatingPointForward(double nIn, double tqIn)
    {
        var tqLimit = 1600;

        var tqInput = new[] {
                "0.0,  4.5, 700",
                "0.1,  3.5, 640	",
                "0.2,  2.7, 560	",
                "0.3,  2.2, 460	",
                "0.4,  1.6, 350	",
                "0.5,  1.2, 250	",
                "0.6,  0.9, 160	",
                "0.74, 0.9,   1",
            };

        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
                tqInput), 1000.RPMtoRad(), tqLimit.RPMtoRad(), ExecutionMode.Engineering, 1, 0.1.SI<MeterPerSquareSecond>(),
                0.1.SI<MeterPerSquareSecond>());

        var operatingPoint = tqData.FindOperatingPointForward(tqIn.SI<NewtonMeter>(), nIn.RPMtoRad(), null);

        Assert.AreEqual(operatingPoint.InTorque.Value(), tqIn, 1e-6);
        Assert.AreEqual(operatingPoint.InAngularVelocity.Value(), nIn.RPMtoRad().Value(), 1e-6);

        var resultList = tqData.FindOperatingPoint(operatingPoint.OutTorque, operatingPoint.OutAngularVelocity,
            0.SI<PerSecond>());
        Assert.AreEqual(1, resultList.Count);
        var reverseOP = resultList[0];

        Assert.AreEqual(operatingPoint.InTorque.Value(), reverseOP.InTorque.Value(), 1e-6);
        Assert.AreEqual(operatingPoint.OutTorque.Value(), reverseOP.OutTorque.Value(), 1e-6);
        Assert.AreEqual(operatingPoint.InAngularVelocity.Value(), reverseOP.InAngularVelocity.Value(), 1e-6);
        Assert.AreEqual(operatingPoint.OutAngularVelocity.Value(), reverseOP.OutAngularVelocity.Value(), 1e-6);
    }

    [TestCase]
    public void TestTorqueConverterComparisonV2()
    {
        var tqLimit = 1600;

        var tqInput = new[] {
                "0.0,1.80,377.80",
                "0.1,1.71,365.21",
                "0.2,1.61,352.62",
                "0.3,1.52,340.02",
                "0.4,1.42,327.43",
                "0.5,1.33,314.84",
                "0.6,1.23,302.24",
                "0.7,1.14,264.46",
                "0.8,1.04,226.68",
                "0.9,0.95,188.90",
                "1.0,0.95,0.00",
            };
        var testData = new List<Tuple<double, double>>() {
                Tuple.Create(139.4087374, 72.74847642),
                Tuple.Create(2275.286998, 453.9413043),
                Tuple.Create(1507.256216, 1012.44118),
                Tuple.Create(26.58522419, 29.25312925),
                Tuple.Create(1752.83589, 328.6386397),
            };
        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
                tqInput), 1000.RPMtoRad(), tqLimit.RPMtoRad(), ExecutionMode.Engineering, 1, 0.1.SI<MeterPerSquareSecond>(),
                0.1.SI<MeterPerSquareSecond>());

        foreach (var entry in testData)
        {
            var torqueTCOut = entry.Item1.SI<NewtonMeter>();
            var angularSpeedOut = entry.Item2.RPMtoRad();
            var result = tqData.FindOperatingPoint(torqueTCOut, angularSpeedOut, 0.SI<PerSecond>()).First();
            //Debug.WriteLine("n_out: {0}, tq_out: {1}, n_in: {2}, Tq_in: {3}", angularSpeedOut.Value() / Constants.RPMToRad,
            //	torqueTCOut.Value(), result.InAngularVelocity.Value() / Constants.RPMToRad, result.InTorque.Value());
        }
    }

    [Test,
    TestCase(10, 110),
    TestCase(20, 130),
    TestCase(50, 90),
    TestCase(50, 190),
    TestCase(60, 150),
    TestCase(70, 70),
    TestCase(70, 90),
    TestCase(70, 190),
    TestCase(80, 50),
    TestCase(80, 130),
    TestCase(90, 70),
    TestCase(100, 150),
    TestCase(130, 70),
    TestCase(150, 80),
    TestCase(170, 80),
    ]
    public void TestTorqueConverterInvalidOperatingPoint(double nOut, double Pout)
    {
        var tqLimit = 1600.RPMtoRad();

        var tqInput = new[] {
                "0,3.935741,563.6598  ",
                "0.1,3.296827,534.1364",
                "0.2,2.701476,504.6129",
                "0.3,2.265852,472.1372",
                "0.4,1.931875,421.9474",
                "0.5,1.554335,354.0435",
                "0.6,1.249399,268.4255",
                "0.7,1.075149,114.9037",
            };

        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
                tqInput), 1000.RPMtoRad(), tqLimit, ExecutionMode.Engineering, 1, 0.1.SI<MeterPerSquareSecond>(),
                0.1.SI<MeterPerSquareSecond>());

        var outAngularSpeed = nOut.RPMtoRad();
        var outTorque = (Pout * 1000).SI<Watt>() / outAngularSpeed;
        var resultList = tqData.FindOperatingPoint(outTorque, outAngularSpeed, 0.SI<PerSecond>());
        Assert.AreEqual(1, resultList.Count);
        var result = resultList[0];

        Assert.IsTrue(result.InAngularVelocity.Value() > tqLimit.Value());
    }

    [TestCase()]
    public void TestTorqueConverterOperatingPointForwardBackward()
    {
        var tqLimit = 1600.RPMtoRad();

        var tqInput = new[] {
                "0,4.6,708		  ",
                "0.1,3.7,641	  ",
                "0.2,2.9,560	  ",
                "0.3,2.4,465	  ",
                "0.4,1.8,356	  ",
                "0.5,1.4,251	  ",
                "0.6,0.9,166	  ",
                "0.735,0.9,1	  ",
                "0.738,1.361,0	  ",
                "0.808,1.351,-40  ",
                "0.898,1.349,-80  ",
                "1.01,1.338,-136  ",
                "1.154,1.327,-217 ",
                "1.347,1.316,-335 ",
                "1.616,1.305,-529 ",
                "3    ,1.294,-729 ",
            };

        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
                tqInput), 1000.RPMtoRad(), tqLimit, ExecutionMode.Engineering, 1, 0.1.SI<MeterPerSquareSecond>(),
                0.1.SI<MeterPerSquareSecond>());

        var operatingPoint = tqData.FindOperatingPointForPowerDemand(20000.SI<Watt>(), 113.5.SI<PerSecond>(),
            1200.RPMtoRad(), 4.SI<KilogramSquareMeter>(), 0.5.SI<Second>(), 20000.SI<Watt>());

        var tmp = tqData.FindOperatingPoint(operatingPoint.OutTorque, operatingPoint.OutAngularVelocity, 0.RPMtoRad());
        var backward = tmp.First();

        Console.WriteLine(operatingPoint);
        Console.WriteLine(operatingPoint.InAngularVelocity * operatingPoint.InTorque);

        Console.WriteLine(backward);
        Console.WriteLine(backward.InAngularVelocity * backward.InTorque);

        Assert.AreEqual(backward.OutAngularVelocity.Value(), operatingPoint.OutAngularVelocity.Value(), 1e-9);
        Assert.AreEqual(backward.OutTorque.Value(), operatingPoint.OutTorque.Value(), 1e-9);
        Assert.AreEqual(backward.InAngularVelocity.Value(), operatingPoint.InAngularVelocity.Value(), 1e-9);
        Assert.AreEqual(backward.InTorque.Value(), operatingPoint.InTorque.Value(), 1e-9);
    }


    [TestCase]
    public void TestTorqueConverterBehavior()
    {
        var tqLimit = 1600;

        var tqInput = new[] {
                "0.0,1.80,377.80",
                "0.1,1.71,365.21",
                "0.2,1.61,352.62",
                "0.3,1.52,340.02",
                "0.4,1.42,327.43",
                "0.5,1.33,314.84",
                "0.6,1.23,302.24",
                "0.7,1.14,264.46",
                "0.8,1.04,226.68",
                "0.9,0.95,188.90",
                "1.0,0.95,0.00",
            };

        var tqInput2 = new[] {
                "0.0000,1.80,554.30",
                "0.6000,1.23,443.44",
                "0.9000,0.95,277.15",
                "1.0000,0.95,0.00",
                "5,0.959,-2290"
            };

        var tqData =
            TorqueConverterDataReader.ReadFromStream(InputDataHelper.InputDataAsStream("Speed Ratio, Torque Ratio,MP1000",
                                                                                        tqInput), 1000.RPMtoRad(), tqLimit.RPMtoRad(), ExecutionMode.Engineering, 1, 0.1.SI<MeterPerSquareSecond>(),
                                                    0.1.SI<MeterPerSquareSecond>());

        var outSpeed = 1200.RPMtoRad();
        var results = new Dictionary<NewtonMeter, IList<TorqueConverterOperatingPoint>>();
        for (var t = -2000; t < 2000; t += 10)
        {
            var op = tqData.FindOperatingPoint(t.SI<NewtonMeter>(), outSpeed, 0.RPMtoRad());
            results[t.SI<NewtonMeter>()] = op;
        }

        foreach (var result in results)
        {
            var tmp = result.Value.FirstOrDefault();
            Console.WriteLine("speed_out: {0},  torque_out: {1}, speed_in: {2},  torque_in: {3}", outSpeed.AsRPM, result.Key, tmp?.InAngularVelocity.AsRPM, tmp?.InTorque);
        }
    }
}