using System.Xml.Linq;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports.MRF;

public class MRF_ReportResultTests
{
	XNamespace MRF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v1.0");
	private const string REPORT_VERSION = "VectoOutputManufacturer.1.9";


    private StandardKernel _kernel;

	private IResultsWriterFactory _reportResultsFactory;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_reportResultsFactory = _kernel.Get<IResultsWriterFactory>();
	}


    [
        TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle, 2, false, true, typeof(MRFResultsWriter.ExemptedVehicle)),
        TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle, 2, false, false, typeof(MRFResultsWriter.ConventionalLorry)),
        TestCase(VehicleCategory.Tractor, VectoSimulationJobType.ConventionalVehicle, 2, false, false, typeof(MRFResultsWriter.ConventionalLorry)),
        TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, typeof(MRFResultsWriter.HEVNonOVCLorry)),
        TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.SerialHybridVehicle, 2, true, false, typeof(MRFResultsWriter.HEVOVCLorry)),
        TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, typeof(MRFResultsWriter.PEVLorry)),

        TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ConventionalVehicle, 2, false, true, typeof(MRFResultsWriter.ExemptedVehicle)),
        TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ConventionalVehicle, 2, false, false, typeof(MRFResultsWriter.ConventionalBus)),
        TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, typeof(MRFResultsWriter.HEVNonOVCBus)),
        TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.SerialHybridVehicle, 2, true, false, typeof(MRFResultsWriter.HEVOVCBus)),
        TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, typeof(MRFResultsWriter.PEVBus)),
    ]
	[
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle, 3, false, true, typeof(MRFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle, 3, false, false, typeof(MRFResultsWriter.ConventionalLorry)),
		TestCase(VehicleCategory.Tractor, VectoSimulationJobType.ConventionalVehicle, 3, false, false, typeof(MRFResultsWriter.ConventionalLorry)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, typeof(MRFResultsWriter.HEVNonOVCLorry)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.SerialHybridVehicle, 3, true, false, typeof(MRFResultsWriter.HEVOVCLorry)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, typeof(MRFResultsWriter.PEVLorry)),

		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ConventionalVehicle, 3, false, true, typeof(MRFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ConventionalVehicle, 3, false, false, typeof(MRFResultsWriter.ConventionalBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, typeof(MRFResultsWriter.HEVNonOVCBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.SerialHybridVehicle, 3, true, false, typeof(MRFResultsWriter.HEVOVCBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, typeof(MRFResultsWriter.PEVBus)),
	]
    public void Test_MRF_ReportResultInstance(VehicleCategory vehicleCategory, VectoSimulationJobType jobType, int amdm, bool ovc,
        bool exempted, Type expectedResultWriterType)
    {
        var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(ReportResultTestUtils.GetMockInputData(amdm), vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

        Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
    }


    [
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "MRF_ReportResult_WritingResults: Lorry Conv SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults: Lorry Conv DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults: Lorry Conv LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "MRF_ReportResult_WritingResults: Lorry Conv ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "MRF_ReportResult_WritingResults: Lorry HEV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults: Lorry HEV OVC DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.IEPC_S, 2, true, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults: Lorry HEV OVC LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "MRF_ReportResult_WritingResults: Lorry HEV OVC ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "MRF_ReportResult_WritingResults: Lorry HEV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "MRF_ReportResult_WritingResults: Lorry HEV non-OVC ERROR"),

        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "MRF_ReportResult_WritingResults: Lorry PEV SUCCESS"),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "MRF_ReportResult_WritingResults: Lorry PEV ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "MRF_ReportResult_WritingResults: Lorry HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, true, true, true, TestName = "MRF_ReportResult_WritingResults: Lorry Conv exempted"),
    ]
    [
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry Conv SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.H2CI, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry Conv H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry Conv DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry Conv LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, false, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry Conv ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, FuelType.H2CI, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV OVC H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, 3, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV OVC DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.IEPC_S, 3, true, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV OVC LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, false, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV OVC ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, true, FuelType.H2CI, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV non-OVC H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, false, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV non-OVC ERROR"),

        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry PEV SUCCESS"),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, false, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry PEV ERROR"),

        TestCase(VectoSimulationJobType.FCHV, 3, false, false, true, FuelType.H2FC, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry FCHV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.FCHV, 3, true, false, true, FuelType.H2FC, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry FCHV OVC SUCCESS"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, true, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, true, true, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry Conv exempted"),
    ]
    public void Test_MRF_ReportResult_WritingResults_Lorry(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
    {
        var vehicleCategory = VehicleCategory.RigidTruck;
        var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		runData.InputData = ReportResultTestUtils.GetMockInputData(amdm);
		var modData = ReportResultTestUtils.GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

        var resultEntries = new List<IResultEntry>();

        var resultEntry = ReportResultTestUtils.GetResultEntry(runData);
        resultEntry.SetResultData(runData, modData, 1);

        resultEntries.Add(resultEntry);

        if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
            var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
            var res2 = ReportResultTestUtils.GetResultEntry(run2);
            res2.SetResultData(run2, modData, 1);
            resultEntries.Add(res2);
        }
		if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.FuelCell) {
			var fcfuel = DeclarationData.FuelData.Lookup(FuelType.H2FC);
			resultEntry.FuelData.Add(fcfuel);
			resultEntry.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
			resultEntry.AuxHeaterFuel = FuelData.Diesel;
			resultEntry.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
			resultEntry.ZEV_CO2 = resultEntry.ZEV_FuelConsumption_AuxHtr * resultEntry.AuxHeaterFuel.CO2PerFuelWeight;
			if (ovc) {
                resultEntries.Clear();
				var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
				run2.InputData = ReportResultTestUtils.GetMockInputData(amdm);
                var res2 = ReportResultTestUtils.GetResultEntry(run2);
				res2.SetResultData(run2, modData, 1);
				resultEntries.Add(res2);
				res2.FuelData.Add(fcfuel);
				res2.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
				res2.AuxHeaterFuel = FuelData.Diesel;
				res2.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
				res2.ZEV_CO2 = res2.ZEV_FuelConsumption_AuxHtr * res2.AuxHeaterFuel.CO2PerFuelWeight;
			}
		}

        var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(ReportResultTestUtils.GetMockInputData(amdm),
            runData.VehicleData.VehicleCategory.GetVehicleType(),
            runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, MRF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

		ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

        //WriteToFile("MRF", doc, runData, success, exempted);
    }

    [
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV OVC DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.IEPC_S, 2, true, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV OVC LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV OVC ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV non-OVC ERROR"),

        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus PEV SUCCESS"),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "MRF_ReportResult_WritingResults: CompletedBus PEV ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, true, true, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv exempted"),
    ]
    [
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus Conv SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.H2CI, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus Conv H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus Conv DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus Conv LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, false, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus Conv ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, FuelType.H2CI, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, 3, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.IEPC_S, 3, true, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, false, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, true, FuelType.H2CI, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV non-OVC H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, false, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV non-OVC ERROR"),

        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus PEV SUCCESS"),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, false, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus PEV ERROR"),

        TestCase(VectoSimulationJobType.FCHV, 3, false, false, true, FuelType.H2FC, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus FCHV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.FCHV, 3, true, false, true, FuelType.H2FC, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus FCHV OVC SUCCESS"),


        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, true, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, true, true, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus Conv exempted"),
    ]
    public void Test_MRF_ReportResult_WritingResults_Bus(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
    {
        var vehicleCategory = VehicleCategory.HeavyBusCompletedVehicle;
        var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		runData.InputData = ReportResultTestUtils.GetMockInputData(amdm);
        var modData = ReportResultTestUtils.GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

        var resultEntries = new List<IResultEntry>();

        var resultEntry = ReportResultTestUtils.GetResultEntry(runData);
        resultEntry.SetResultData(runData, modData, 1);

        if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.PureElectric ||
			(fuels.All(f => f.IsHydrogenFuel()))) {
            resultEntry.AuxHeaterFuel = FuelData.Diesel;
            resultEntry.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
            resultEntry.ZEV_CO2 = resultEntry.ZEV_FuelConsumption_AuxHtr * resultEntry.AuxHeaterFuel.CO2PerFuelWeight;
        }

        resultEntries.Add(resultEntry);

        if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
            var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
            var res2 = ReportResultTestUtils.GetResultEntry(run2);
            res2.SetResultData(run2, modData, 1);
            resultEntries.Add(res2);
			if (fuels.All(f => f.IsHydrogenFuel())) {
				res2.AuxHeaterFuel = FuelData.Diesel;
				res2.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
				res2.ZEV_CO2 = res2.ZEV_FuelConsumption_AuxHtr * res2.AuxHeaterFuel.CO2PerFuelWeight;
			}
        }
		if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.FuelCell) {
			var fcfuel = DeclarationData.FuelData.Lookup(FuelType.H2FC);
			resultEntry.FuelData.Add(fcfuel);
			resultEntry.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
			resultEntry.AuxHeaterFuel = FuelData.Diesel;
			resultEntry.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
			resultEntry.ZEV_CO2 = resultEntry.ZEV_FuelConsumption_AuxHtr * resultEntry.AuxHeaterFuel.CO2PerFuelWeight;
			if (ovc) {
                //resultEntries.Clear();
				var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
				run2.InputData = ReportResultTestUtils.GetMockInputData(amdm);
                var res2 = ReportResultTestUtils.GetResultEntry(run2);
				res2.SetResultData(run2, modData, 1);
				resultEntries.Add(res2);
				res2.FuelData.Add(fcfuel);
				res2.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
				res2.AuxHeaterFuel = FuelData.Diesel;
				res2.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
				res2.ZEV_CO2 = res2.ZEV_FuelConsumption_AuxHtr * res2.AuxHeaterFuel.CO2PerFuelWeight;
			}
		}

        var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(ReportResultTestUtils.GetMockInputData(amdm),
            runData.VehicleData.VehicleCategory.GetVehicleType(),
            runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, MRF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

		ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

        //WriteToFile("MRF", doc, runData, success, exempted);
    }

}