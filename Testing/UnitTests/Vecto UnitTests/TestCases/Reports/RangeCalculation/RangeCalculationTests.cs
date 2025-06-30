using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports.RangeCalculation;

public class RangeCalculationTests
{
    [TestCase()]
    public void TestCalculatePEVRanges(params FuelType[] fuels)
    {
        var jobType = VectoSimulationJobType.ParallelHybridVehicle;
        var vehicleCategory = VehicleCategory.RigidTruck;
        var ovcmode = OvcHevMode.ChargeDepleting;
        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, false, ovcmode, fuels);
        var modData = ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, fuels, ovcmode);

        //var result = GetResultEntry(runData);
        //result.SetResultData(runData, modData, 1);


        var weighted = DeclarationData.CalculateElectricRangesPEV(runData, modData);

        Console.WriteLine($"{weighted.ActualChargeDepletingRange.Value().ToXMLFormat(3)} {weighted.EquivalentAllElectricRange.Value().ToXMLFormat(3)} {weighted.ZeroCO2EmissionsRange.Value().ToXMLFormat(3)}");

        //1518.750 1366.875 1366.875 0.004 797877.345 30.890 20.000

        Assert.AreEqual(1518.750, weighted.ActualChargeDepletingRange.Value(), 1e-3);
        Assert.AreEqual(1518.750, weighted.EquivalentAllElectricRange.Value(), 1e-3);
        Assert.AreEqual(1518.750, weighted.ZeroCO2EmissionsRange.Value(), 1e-3);

    }
}