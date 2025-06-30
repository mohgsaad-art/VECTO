using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class DistanceBasedCycleReaderTests
{
    public const string CycleHdr = "<s>,<v>,<grad>,<stop>";

    [TestCase]
    public void FilterRedundantEntries()
    {
        var data = new[] {
            " 0, 20, 0, 0",
            " 1, 20, 0, 0",
            " 2, 20, 0, 0",
            " 3, 20, 0, 0",
            " 4, 20, 0, 0",
            " 5, 20, 1, 0",
            " 6, 20, 1, 0",
            " 6, 20, 1, 0",
            " 8, 20, 1, 0",
            " 9, 20, 1, 0",
        };

        var cycleInputData = InputDataHelper.InputDataAsStream(CycleHdr, data);
        var cycleData = DrivingCycleDataReader.ReadFromStream(cycleInputData, CycleType.DistanceBased, "", false);
        Assert.AreEqual(3, cycleData.Entries.Count);

        Assert.AreEqual(0, cycleData.Entries[0].Distance.Value());
        Assert.AreEqual(5, cycleData.Entries[1].Distance.Value());
        Assert.AreEqual(9, cycleData.Entries[2].Distance.Value());
    }

    [TestCase]
    public void HandleStopTimes()
    {
        var data = new[] {
            " 0, 20, 0, 0",
            " 1, 20, 0, 0",
            " 2, 20, 0, 0",
            " 3, 20, 0, 0",
            " 4, 20, 0, 0",
            " 5,  0, 0, 5",
            " 6, 20, 0, 0",
            " 6, 20, 0, 0",
            " 8, 20, 0, 0",
            " 9, 20, 0, 0",
        };

        var cycleInputData = InputDataHelper.InputDataAsStream(CycleHdr, data);
        var cycleData = DrivingCycleDataReader.ReadFromStream(cycleInputData, CycleType.DistanceBased, "", false);
        Assert.AreEqual(4, cycleData.Entries.Count);

        Assert.AreEqual(0, cycleData.Entries[0].Distance.Value());
        Assert.AreEqual(5, cycleData.Entries[1].Distance.Value());
        Assert.AreEqual(5, cycleData.Entries[2].Distance.Value());
        Assert.AreEqual(9, cycleData.Entries[3].Distance.Value());

        Assert.AreEqual(5, cycleData.Entries[1].StoppingTime.Value());
    }

    [TestCase]
    public void StopTimeWhenVehicleSpeedIsNotZero()
    {
        var data = new[] {
            " 0, 20, 0, 0",
            "20, 20, 0, 5",
            "90, 20, 0, 0"
        };

        var cycleInputData = InputDataHelper.InputDataAsStream(CycleHdr, data);

        AssertHelper.Exception<VectoException>(() => DrivingCycleDataReader.ReadFromStream(cycleInputData, CycleType.DistanceBased, "", false));
    }

    [TestCase]
    public void DistanceNotStrictlyIncreasing()
    {
        var data = new[] {
            " 0, 20, 0, 0",
            "90, 20, 0, 0",
            "80, 20, 0, 0"
        };

        var cycleInputData = InputDataHelper.InputDataAsStream(CycleHdr, data);

        AssertHelper.Exception<VectoException>(() => DrivingCycleDataReader.ReadFromStream(cycleInputData, CycleType.DistanceBased, "", false));
    }
}