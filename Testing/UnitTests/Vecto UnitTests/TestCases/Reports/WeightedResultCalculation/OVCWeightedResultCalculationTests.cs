using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports.WeightedResultCalculation;

public class OVCWeightedResultCalculationTests
{
	[TestCase()]
	public void TestCalculateOVCWeightedResult(params FuelType[] fuels)
	{
		var jobType = VectoSimulationJobType.ParallelHybridVehicle;
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = OvcHevMode.ChargeDepleting;
		var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, false, ovcmode, fuels);
		var modData = ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, fuels, ovcmode);

		var cdResult = ReportResultTestUtils.GetResultEntry(runData);
		cdResult.SetResultData(runData, modData, 1);

		var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, false, OvcHevMode.ChargeSustaining, fuels);
		var modData2 = ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, fuels, OvcHevMode.ChargeSustaining);
		var csResult = ReportResultTestUtils.GetResultEntry(run2);
		csResult.SetResultData(run2, modData2, 1);

		var weighted = DeclarationData.CalculateWeightedResult(cdResult, csResult);

		Console.WriteLine($"{weighted.ActualChargeDepletingRange.Value().ToXMLFormat(3)} {weighted.EquivalentAllElectricRange.Value().ToXMLFormat(3)} {weighted.ZeroCO2EmissionsRange.Value().ToXMLFormat(3)} {weighted.UtilityFactor.ToXMLFormat(3)}" +
						$" {weighted.ElectricEnergyConsumption.Value().ToXMLFormat(3)} {weighted.FuelConsumption[FuelData.Diesel].Value().ToXMLFormat(3)}");

		//1518.750 1366.875 1366.875 0.004 795230.237 30.890 20.000

		Assert.AreEqual(1518.750, weighted.ActualChargeDepletingRange.Value(), 1e-3);
		Assert.AreEqual(1366.875, weighted.EquivalentAllElectricRange.Value(), 1e-3);
		Assert.AreEqual(1366.875, weighted.ZeroCO2EmissionsRange.Value(), 1e-3);
		Assert.AreEqual(0.004, weighted.UtilityFactor, 1e-3);
		Assert.AreEqual(795230.237, weighted.ElectricEnergyConsumption.Value(), 1e-3);
		Assert.AreEqual(30.890, weighted.FuelConsumption[FuelData.Diesel].Value(), 1e-3);
		//Assert.AreEqual(20.000, weighted.CO2Total.Value(), 1e-3);

	}

    [TestCase()]
    public void TestCalculateOVCWeightedResultIMC(params FuelType[] fuels)
    {
        var jobType = VectoSimulationJobType.ParallelHybridVehicle;
        var vehicleCategory = VehicleCategory.RigidTruck;
        var ovcmode = OvcHevMode.ChargeDepleting;
        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, false, ovcmode, fuels, IMCTechnology.OverheadPantograph);
        var modData = ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, fuels, ovcmode);

        var cdResult = ReportResultTestUtils.GetResultEntry(runData);
        cdResult.SetResultData(runData, modData, 1);

        var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, false, OvcHevMode.ChargeSustaining, fuels, IMCTechnology.OverheadPantograph);
        var modData2 = ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, fuels, OvcHevMode.ChargeSustaining);
        var csResult = ReportResultTestUtils.GetResultEntry(run2);
        csResult.SetResultData(run2, modData2, 1);

        var weighted = DeclarationData.CalculateWeightedResult(cdResult, csResult);

        Console.WriteLine($"{weighted.ActualChargeDepletingRange.Value().ToXMLFormat(3)} {weighted.EquivalentAllElectricRange.Value().ToXMLFormat(3)} {weighted.ZeroCO2EmissionsRange.Value().ToXMLFormat(3)} {weighted.UtilityFactor.ToXMLFormat(3)}" +
                        $" {weighted.ElectricEnergyConsumption.Value().ToXMLFormat(3)} {weighted.FuelConsumption[FuelData.Diesel].Value().ToXMLFormat(3)} {weighted.CO2PerMeter.Value().ToXMLFormat(3)}");

        //1518.750 1366.875 1366.875 0.004 795230.237 30.890 20.000
        //1518.750 1366.875 1366.875 0.504 97381237.429 16.940 20.000

        Assert.AreEqual(1518.750, weighted.ActualChargeDepletingRange.Value(), 1e-3);
        Assert.AreEqual(1366.875, weighted.EquivalentAllElectricRange.Value(), 1e-3);
        Assert.AreEqual(1366.875, weighted.ZeroCO2EmissionsRange.Value(), 1e-3);
        Assert.AreEqual(0.504, weighted.UtilityFactor, 1e-3);
        Assert.AreEqual(50513415.163, weighted.ElectricEnergyConsumption.Value(), 1e-3);
        Assert.AreEqual(16.940, weighted.FuelConsumption[FuelData.Diesel].Value(), 1e-3);
        Assert.AreEqual(30000.0, weighted.Distance.Value(), 1e-3);
        Assert.AreEqual(20.0 / 30000.0, weighted.CO2PerMeter.Value(), 1e-8);

    }
}