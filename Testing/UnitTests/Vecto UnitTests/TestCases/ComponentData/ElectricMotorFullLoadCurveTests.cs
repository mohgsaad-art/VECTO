using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class ElectricMotorFullLoadCurveTests
{
    [TestCase()]
    public void TestEmTorqueLimit_DriveTorqueBelowEM()
    {
        var emFld = new[] {
                "  0, 800, -800",
                "200, 800, -800",
                "400, 400, -400"};

        var maxTq = new[] {
                "  0, 500, -500",
                "200, 400, -500",
                "400, 400, -400"};

        var expected = new[] {
                "  0, 500, -500",
                "200, 400, -500",
                "400, 400, -400"};

        TestIntersectEmMaxTorque(emFld, maxTq, expected);

    }

    [TestCase()]
    public void TestEmTorqueLimit_DriveTorqueIntersects1()
    {
        var emFld = new[] {
                "  0, 800, -800",
                "200, 800, -800",
                "400, 400, -400"};

        var maxTq = new[] {
                "  0, 1200, -500",
                "200, 400, -500",
                "400, 400, -400"};

        var expected = new[] {
                "  0, 800, -500",
                "100, 800, -500",
                "200, 400, -500",
                "400, 400, -400"};

        TestIntersectEmMaxTorque(emFld, maxTq, expected);

    }

    [TestCase()]
    public void TestEmTorqueLimit_GenTorqueIntersects1()
    {
        var emFld = new[] {
                "  0, 800, -800",
                "200, 800, -800",
                "400, 400, -400"};

        var maxTq = new[] {
                "  0, 500, -1200",
                "200, 500, -400",
                "400, 400, -400"};

        var expected = new[] {
                "  0, 500, -800",
                "100, 500, -800",
                "200, 500, -400",
                "400, 400, -400"};

        TestIntersectEmMaxTorque(emFld, maxTq, expected);
    }

    [TestCase()]
    public void TestEmTorqueLimit_DifferentGridpoints1()
    {
        var emFld = new[] {
                "  0, 800, -800",
                "200, 800, -800",
                "400, 400, -400"};

        var maxTq = new[] {
                "  0, 500, -1200",
                "100, 500, -400",
                "300, 400, -400",
                "400, 400, -400"};

        var expected = new[] {
                "  0, 500, -800",
                " 50, 500, -800",
                "100, 500, -400",
                "200, 450, -400",
                "300, 400, -400",
                "400, 400, -400"};

        TestIntersectEmMaxTorque(emFld, maxTq, expected);
    }

    [TestCase()]
    public void TestEmTorqueLimit_Complex1()
    {
        var emFld = new[] {
                "  0, 800, -800",
                "200, 800, -800",
                "400, 400, -400"};

        var maxTq = new[] {
                "  0, 1000, -1200",
                " 50, 1000,  -400",
                "100,  500,  -400",
                "150,  820,  -400",
                "250,  730,  -800",
                "300,  500,  -500",
                "400,  500,  -450",
            };

        var expected = new[] {
                "0, 800, -800",
                "25, 800, -800",
                "50, 800, -400",
                "70, 800, -400",
                "100, 500, -400",
                "146.875, 800, -400",
                "150, 800, -400",
                "172.222, 800, -488.8888",
                "200, 775, -600",
                "222.7272, 754.5455, -690.909",
                "233.3333, 733.333, -733.333",
                "250, 700, -700",
                "261.538, 676.9231, -676.9231",
                "275, 615, -650",
                "300, 500, -500",
                "350, 500, -475",
                "366.666, 466.6667, -466.6667",
                "400, 400, -400",
            };

        TestIntersectEmMaxTorque(emFld, maxTq, expected);
    }

    [TestCase()]
    public void TestEmTorqueLimit_Complex2()
    {
        var emFld = new[] {
                "  0, 1000, -1200",
                " 50, 1000,  -400",
                "100,  500,  -400",
                "150,  820,  -400",
                "250,  730,  -800",
                "300,  500,  -500",
                "400,  500,  -450",
            };

        var maxTq = new[] {
                "  0, 800, -800",
                "200, 800, -800",
                "400, 400, -400"};

        var expected = new[] {
                "0, 800, -800",
                "25, 800, -800",
                "50, 800, -400",
                "70, 800, -400",
                "100, 500, -400",
                "146.875, 800, -400",
                "150, 800, -400",
                "172.222, 800, -488.8888",
                "200, 775, -600",
                "222.7272, 754.5455, -690.909",
                "233.3333, 733.333, -733.333",
                "250, 700, -700",
                "261.538, 676.9231, -676.9231",
                "275, 615, -650",
                "300, 500, -500",
                "350, 500, -475",
                "366.666, 466.6667, -466.6667",
                "400, 400, -400",
            };

        TestIntersectEmMaxTorque(emFld, maxTq, expected);
    }


    private static void TestIntersectEmMaxTorque(string[] emFld, string[] maxTq, string[] expected)
    {
        var emHeader = "n [rpm] , T_drive [Nm] , T_recuperation [Nm]";

        var fld = ElectricFullLoadCurveReader.Read(InputDataHelper.InputDataAsStream(emHeader, emFld), 1);
        var max = ElectricFullLoadCurveReader.Read(InputDataHelper.InputDataAsStream(emHeader, maxTq), 1);

        var intersect = AbstractSimulationDataAdapter.IntersectEMFullLoadCurves(fld, max);

        var exp = ElectricFullLoadCurveReader.Read(InputDataHelper.InputDataAsStream(emHeader, expected), 1);
        foreach (var t in exp.FullLoadEntries.ZipAll(intersect.FullLoadEntries, Tuple.Create)) {
            Assert.AreEqual(t.Item1.MotorSpeed.AsRPM, t.Item2.MotorSpeed.AsRPM, 1e-3, "MotorSpeed");
            Assert.AreEqual(-t.Item1.FullDriveTorque.Value(), -t.Item2.FullDriveTorque.Value(), 1e-3,
                "FullDriveTorque n = {0}", t.Item1.MotorSpeed.AsRPM);
            Assert.AreEqual(-t.Item1.FullGenerationTorque.Value(), -t.Item2.FullGenerationTorque.Value(), 1e-3,
                "FullGenerationTorque n = {0}", t.Item1.MotorSpeed.AsRPM);
        }
    }
}