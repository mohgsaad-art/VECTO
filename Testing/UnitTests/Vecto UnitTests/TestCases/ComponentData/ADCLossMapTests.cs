using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class ADCLossMapTests
{
    [TestCase(0.95, 1000, 1000, 1052.63157894),   // case EM drag: EM torque is lower than DT torque
         TestCase(0.95, 1000, -1000, -950), // case EM drive: DT torque is lower than EM torque
        ]
    public void TestADCEfficiencyMapLookupFWD(double eff, double emSpeed, double emTorque, double expectedDTTorque)
    {
        var lossMap = TransmissionLossMapReader.CreateEmADCLossMap(eff, 1.0, "EM ADC Eff");

        var outTorque = lossMap.GetOutTorque(emSpeed.RPMtoRad(), emTorque.SI<NewtonMeter>());

        Assert.AreEqual(expectedDTTorque, outTorque.Value(), 1e-6);

        var effValue = emTorque < 0 ? outTorque / emTorque : emTorque / outTorque;

        Assert.AreEqual(eff, effValue.Value(), 1e-6);
    }

    [TestCase(0.95, 1000, 1000, 950),   // case EM drag: EM torque is lower than DT torque
    TestCase(0.95, 1000, -1000, -1052.63157894), // case EM drive: DT torque is lower than EM torque
    ]
    public void TestADCEfficiencyMapLookupBWD(double eff, double dtSpeed, double dtTorque, double expectedEMTorque)
    {
        var lossMap = TransmissionLossMapReader.CreateEmADCLossMap(eff, 1.0, "EM ADC Eff");

        var torqueLoss = lossMap.GetTorqueLoss(dtSpeed.RPMtoRad(), dtTorque.SI<NewtonMeter>());

        var emTorque = dtTorque.SI<NewtonMeter>() + torqueLoss.Value;

        Assert.AreEqual(expectedEMTorque, emTorque.Value(), 1e-6);


    }

    [TestCase(1000, 1000, 1050),
    TestCase(1000, -1000, -950),
    ]
    public void TestADCEfficiencyMapLookupFWD(double emSpeed, double emTorque, double expectedDTTorque)
    {
        var header = "n, T_in, T_loss";
        var mapData = new[] {
                "0, -100000, 5000",
                "0, 0, 0",
                "0, 100000, 5000",
                "10000, -100000, 5000",
                "10000, 0, 0",
                "10000, 100000, 5000"
            };

        var lossMap =
            TransmissionLossMapReader.CreateEmADCLossMap(InputDataHelper.InputDataAsTableData(header, mapData), 1.0,
                "EM ADC Map", false);

        var outTorque = lossMap.GetOutTorque(emSpeed.RPMtoRad(), emTorque.SI<NewtonMeter>());

        Assert.AreEqual(expectedDTTorque, outTorque.Value(), 1e-6);
    }

    [TestCase(1000, 1000, 952.380952),
    TestCase(1000, -1000, -1052.6315789),
    ]
    public void TestADCEfficiencyMapLookupBWD(double dtSpeed, double dtTorque, double expectedEMTorque)
    {
        var header = "n, T_in, T_loss";
        var mapData = new[] {
                "0, -100000, 5000",
                "0, 0, 0",
                "0, 100000, 5000",
                "10000, -100000, 5000",
                "10000, 0, 0",
                "10000, 100000, 5000"
            };

        var lossMap =
            TransmissionLossMapReader.CreateEmADCLossMap(InputDataHelper.InputDataAsTableData(header, mapData), 1.0,
                "EM ADC Map", false);

        var torqueLoss = lossMap.GetTorqueLoss(dtSpeed.RPMtoRad(), dtTorque.SI<NewtonMeter>());

        var emTorque = dtTorque.SI<NewtonMeter>() + torqueLoss.Value;

        Assert.AreEqual(expectedEMTorque, emTorque.Value(), 1e-6);
    }
}