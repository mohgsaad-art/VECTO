using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.ResultWriter;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports.VIF;

public class VIF_ReportResultTests
{
	XNamespace VIF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1");

    private StandardKernel _kernel;

	private IResultsWriterFactory _reportResultsFactory;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_reportResultsFactory = _kernel.Get<IResultsWriterFactory>();
	}


    [
        TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.ConventionalVehicle, 2, false, true, typeof(VIFResultsWriter.ExemptedVehicle)),
        TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.ConventionalVehicle, 2, false, false, typeof(VIFResultsWriter.ConventionalBus)),
        TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, typeof(VIFResultsWriter.HEVNonOVCBus)),
        TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.SerialHybridVehicle, 2, true, false, typeof(VIFResultsWriter.HEVOVCBus)),
        TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, typeof(VIFResultsWriter.PEVBus)),
    ]
    public void Test_VIF_ReportResultInstance(VehicleCategory vehicleCategory, VectoSimulationJobType jobType, int amdm, bool ovc,
        bool exempted, Type expectedResultWriterType)
    {
        var resultsWriter = _reportResultsFactory.GetVIFResultsWriter(ReportResultTestUtils.GetMockInputData(amdm), vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

        Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
    }

    [
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV OVC DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.IEPC_S, 2, true, false, true, FuelType.NGPI, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV OVC LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV OVC ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV non-OVC ERROR"),

        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus PEV SUCCESS"),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "VIF_ReportResult_WritingResults: PrimaryBus PEV ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, true, true, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv exempted"),
    ]
    public void Test_VIF_ReportResult_WritingResults_Bus(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
    {
        var vehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle;
        var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
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

        var resultsWriter = _reportResultsFactory.GetVIFResultsWriter(ReportResultTestUtils.GetMockInputData(amdm),
            runData.VehicleData.VehicleCategory.GetVehicleType(),
            runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, "VectoOutputMultistep.0.1", VIF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

		ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.MultistepOutputData), validator.ValidationError);

        //WriteToFile("VIF", doc, runData, success, exempted);
    }
}