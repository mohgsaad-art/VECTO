using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Moq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports.CIF;

public class CIF_ReportResultTests
{
	XNamespace CIF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput:v1.0");

	private const string REPORT_VERSION = "VectoOutputCustomer.0.9";

    private StandardKernel _kernel;

	private IResultsWriterFactory _reportResultsFactory;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_reportResultsFactory = _kernel.Get<IResultsWriterFactory>();
	}


    [
        TestCase(VehicleCategory.RigidTruck, 2, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(CIFResultsWriter.ExemptedVehicle)),
        TestCase(VehicleCategory.RigidTruck, 2, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalLorry)),
        TestCase(VehicleCategory.Tractor, 2, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalLorry)),
        TestCase(VehicleCategory.RigidTruck, 2, VectoSimulationJobType.ParallelHybridVehicle, false, false, typeof(CIFResultsWriter.HEVNonOVCLorry)),
        TestCase(VehicleCategory.RigidTruck, 2, VectoSimulationJobType.SerialHybridVehicle, true, false, typeof(CIFResultsWriter.HEVOVCLorry)),
        TestCase(VehicleCategory.RigidTruck, 2, VectoSimulationJobType.BatteryElectricVehicle, true, false, typeof(CIFResultsWriter.PEVLorry)),

        TestCase(VehicleCategory.HeavyBusCompletedVehicle, 2, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(CIFResultsWriter.ExemptedVehicle)),
        TestCase(VehicleCategory.HeavyBusCompletedVehicle, 2, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalBus)),
        TestCase(VehicleCategory.HeavyBusCompletedVehicle, 2, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalBus)),
        TestCase(VehicleCategory.HeavyBusCompletedVehicle, 2, VectoSimulationJobType.ParallelHybridVehicle, false, false, typeof(CIFResultsWriter.HEVNonOVCBus)),
        TestCase(VehicleCategory.HeavyBusCompletedVehicle, 2, VectoSimulationJobType.SerialHybridVehicle, true, false, typeof(CIFResultsWriter.HEVOVCBus)),
        TestCase(VehicleCategory.HeavyBusCompletedVehicle, 2, VectoSimulationJobType.BatteryElectricVehicle, true, false, typeof(CIFResultsWriter.PEVBus)),

		TestCase(VehicleCategory.RigidTruck, 3, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(CIFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.RigidTruck, 3, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalLorry)),
		TestCase(VehicleCategory.Tractor, 3, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalLorry)),
		TestCase(VehicleCategory.RigidTruck, 3, VectoSimulationJobType.ParallelHybridVehicle, false, false, typeof(CIFResultsWriter.HEVNonOVCLorry)),
		TestCase(VehicleCategory.RigidTruck, 3, VectoSimulationJobType.SerialHybridVehicle, true, false, typeof(CIFResultsWriter.HEVOVCLorry)),
		TestCase(VehicleCategory.RigidTruck, 3, VectoSimulationJobType.BatteryElectricVehicle, true, false, typeof(CIFResultsWriter.PEVLorry)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, 3, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(CIFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, 3, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, 3, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, 3, VectoSimulationJobType.ParallelHybridVehicle, false, false, typeof(CIFResultsWriter.HEVNonOVCBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, 3, VectoSimulationJobType.SerialHybridVehicle, true, false, typeof(CIFResultsWriter.HEVOVCBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, 3, VectoSimulationJobType.BatteryElectricVehicle, true, false, typeof(CIFResultsWriter.PEVBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, 3, VectoSimulationJobType.FCHV, false, false, typeof(CIFResultsWriter.FCHVNonOVCBus)),

    ]
    public void Test_CIF_ReportResultInstance(VehicleCategory vehicleCategory, int amdm, VectoSimulationJobType jobType, bool ovc,
        bool exempted, Type expectedResultWriterType)
    {
        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(ReportResultTestUtils.GetMockInputData(amdm, ovc), vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

        Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
    }

    [
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "CIF_ReportResult_WritingResults: Lorry Conv SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults: Lorry Conv DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults: Lorry Conv LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "CIF_ReportResult_WritingResults: Lorry Conv ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "CIF_ReportResult_WritingResults: Lorry HEV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults: Lorry HEV OVC DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.IEPC_S, 2, true, false,  true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults: Lorry HEV OVC LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "CIF_ReportResult_WritingResults: Lorry HEV OVC ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "CIF_ReportResult_WritingResults: Lorry HEV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "CIF_ReportResult_WritingResults: Lorry HEV non-OVC ERROR"),

        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "CIF_ReportResult_WritingResults: Lorry PEV SUCCESS"),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "CIF_ReportResult_WritingResults: Lorry PEV ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "CIF_ReportResult_WritingResults: Lorry HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, true, true, true, TestName = "CIF_ReportResult_WritingResults: Lorry Conv exempted"),
    ]
    [
		TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry Conv SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry Conv DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry Conv LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, false, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry Conv ERROR"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.H2CI, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry Conv H2 ICE SUCCESS"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.SerialHybridVehicle, 3, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV OVC DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.IEPC_S, 3, true, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV OVC LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, FuelType.H2CI, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV OVC H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, false, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV OVC ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, true, FuelType.H2CI, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV non-OVC H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, false, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV non-OVC ERROR"),

        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry PEV SUCCESS"),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, false, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry PEV ERROR"),

        TestCase(VectoSimulationJobType.FCHV, 3, true, false, true, FuelType.H2FC, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry FCHV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.FCHV, 3, false, false, true, FuelType.H2FC, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry FCHV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, true, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, true, true, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry Conv exempted"),
    ]
    public void Test_CIF_ReportResult_WritingResults_Lorry(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
    {
        var vehicleCategory = VehicleCategory.RigidTruck;
        var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		runData.InputData = ReportResultTestUtils.GetMockInputData(amdm, ovc);
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
			if (ovc) {
				var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
				run2.InputData = ReportResultTestUtils.GetMockInputData(amdm, ovc);
				run2.Iteration = 1;
                var res2 = ReportResultTestUtils.GetResultEntry(run2);
				res2.SetResultData(run2, modData, 1);
				resultEntries.Add(res2);
				res2.FuelData.Add(fcfuel);
				res2.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
			}
		}

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(ReportResultTestUtils.GetMockInputData(amdm, ovc),
            runData.VehicleData.VehicleCategory.GetVehicleType(),
            runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, CIF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

		ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

        //WriteToFile("CIF", doc, runData, success, exempted);
    }


    [
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus Conv SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults: CompletedBus Conv DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults: CompletedBus Conv LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "CIF_ReportResult_WritingResults: CompletedBus Conv ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV OVC DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV OVC LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV OVC ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV non-OVC ERROR"),

        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus PEV SUCCESS"),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "CIF_ReportResult_WritingResults: CompletedBus PEV ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV exempted"),
    ]
    [
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus Conv SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus Conv DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus Conv LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, false, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus Conv ERROR"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, false, false, true, FuelType.H2CI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus Conv H2 ICE SUCCESS"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC DualFuel SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC LNG SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, true, FuelType.H2CI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, false, false, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV OVC ERROR"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, true, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV non-OVC SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, true, FuelType.H2CI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV non-OVC H2 ICE SUCCESS"),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, false, false, false, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV non-OVC ERROR"),

        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus PEV SUCCESS"),
        TestCase(VectoSimulationJobType.BatteryElectricVehicle, 3, true, false, false, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus PEV ERROR"),

        TestCase(VectoSimulationJobType.FCHV, 3, true, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus FCHV OVC SUCCESS"),
        TestCase(VectoSimulationJobType.FCHV, 3, false, false, true, FuelType.H2FC, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus FCHV non-OVC SUCCESS"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, true, true, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV exempted"),
    ]
    public void TestReportResult_WritingResults_CompletedBus(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
    {
        var vehicleCategory = VehicleCategory.HeavyBusCompletedVehicle;
        var ovcmode = ovc 
            ? (jobType.IsFCHV() ? OvcHevMode.ChargeSustaining : OvcHevMode.ChargeDepleting) 
            : OvcHevMode.NotApplicable;

        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		runData.InputData = ReportResultTestUtils.GetMockInputData(amdm, ovc);
        var modData = ReportResultTestUtils.GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

        var resultEntries = new List<IResultEntry>();

        var resultEntry = ReportResultTestUtils.GetResultEntry(runData);
        resultEntry.SetResultData(runData, modData, 1);
        resultEntries.Add(resultEntry);

        if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.PureElectric ||
			(fuels.All(f => f.IsHydrogenFuel()))) {
            resultEntry.AuxHeaterFuel = FuelData.Diesel;
            resultEntry.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
            resultEntry.ZEV_CO2 = resultEntry.ZEV_FuelConsumption_AuxHtr * resultEntry.AuxHeaterFuel.CO2PerFuelWeight;
        }

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
            resultEntry.HydrogenRange = 1000.SI<Meter>();
            resultEntry.ZeroCO2EmissionsRange = 1000.SI<Meter>();
			if (ovc) {
				var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
				run2.InputData = ReportResultTestUtils.GetMockInputData(amdm, ovc);
				run2.Iteration = 1;
				var res2 = ReportResultTestUtils.GetResultEntry(run2);
				res2.SetResultData(run2, modData, 1);
				resultEntries.Add(res2);
				res2.FuelData.Add(fcfuel);
				res2.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
				res2.AuxHeaterFuel = FuelData.Diesel;
				res2.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
				res2.ZEV_CO2 = res2.ZEV_FuelConsumption_AuxHtr * res2.AuxHeaterFuel.CO2PerFuelWeight;
                res2.HydrogenRange = 1000.SI<Meter>();
                res2.ZeroCO2EmissionsRange = 1000.SI<Meter>();
            }
		}

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(ReportResultTestUtils.GetMockInputData(amdm, ovc),
            runData.VehicleData.VehicleCategory.GetVehicleType(),
            runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, CIF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

		ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

        // WriteToFile("CIF", doc, runData, success, exempted);
    }

    [
        TestCase(VehicleClass.Class11, VehicleCategory.RigidTruck, TestName = "CIF_EHC_Summary: Class11 has 3 summaries"),
        TestCase(VehicleClass.Class12, VehicleCategory.Tractor, TestName = "CIF_EHC_Summary: Class12 has 3 summaries"),
        TestCase(VehicleClass.Class16, VehicleCategory.RigidTruck, TestName = "CIF_EHC_Summary: Class16 has 3 summaries"),
    ]
    public void Test_CIF_EHC_Summary_ThreeSummariesForEHCGroups(VehicleClass vehicleClass, VehicleCategory vehicleCategory)
    {
        var missions = new[]
        {
            (MissionType.LongHaul,            LoadingType.LowLoading),
            (MissionType.LongHaul,            LoadingType.ReferenceLoad),
            (MissionType.RegionalDelivery,    LoadingType.LowLoading),
            (MissionType.RegionalDelivery,    LoadingType.ReferenceLoad),
            (MissionType.Construction,        LoadingType.LowLoading),
            (MissionType.Construction,        LoadingType.ReferenceLoad),
            (MissionType.LongHaulEMS,         LoadingType.LowLoading),
            (MissionType.LongHaulEMS,         LoadingType.ReferenceLoad),
            (MissionType.RegionalDeliveryEMS, LoadingType.LowLoading),
            (MissionType.RegionalDeliveryEMS, LoadingType.ReferenceLoad),
        };

        var resultEntries = missions.Select(m =>
        {
            var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory,
                VectoSimulationJobType.ConventionalVehicle, false, false, OvcHevMode.NotApplicable, null);
            runData.Mission = new Mission() { MissionType = m.Item1 };
            runData.Loading = m.Item2;
            runData.VehicleData.VehicleClass = vehicleClass;
            var entry = ReportResultTestUtils.GetResultEntry(runData);
            entry.SetResultData(runData, ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, null), 1);
            return (IResultEntry)entry;
        }).ToList();

        var firstEntry = resultEntries.First();
        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
            ReportResultTestUtils.GetMockInputData(3, false),
            vehicleCategory.GetVehicleType(),
            VectoSimulationJobType.ConventionalVehicle, false, false);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, CIF_NS);
        var summaryCount = doc.Descendants(CIF_NS + "Summary").Count();

        Assert.AreEqual(3, summaryCount, "Expected 3 Summary elements in CIF report for EHC group");
    }

    /// <summary>
    /// EHC missions (LongHaulEMS) must be excluded from the first weighted summary and only appear
    /// in the EHC summary. With LongHaul + LongHaulEMS entries the XML contains exactly 2 Summary
    /// elements (normal + EHC; vocational is null so not written). Without the EHC filter the
    /// LongHaulEMS payload would pollute the first summary's weighted average.
    /// </summary>
    [
        TestCase(VehicleClass.Class11, VehicleCategory.RigidTruck, TestName = "CIF_EHC_EhcEntriesExcludedFromFirstSummary: Class11"),
        TestCase(VehicleClass.Class16, VehicleCategory.RigidTruck, TestName = "CIF_EHC_EhcEntriesExcludedFromFirstSummary: Class16"),
    ]
    public void Test_CIF_EHC_EhcEntriesExcludedFromFirstSummary(VehicleClass vehicleClass, VehicleCategory vehicleCategory)
    {
        // Arrange: two normal LongHaul entries + two EHC LongHaulEMS entries
        var allMissions = new[]
        {
            (MissionType.LongHaul,    LoadingType.LowLoading),
            (MissionType.LongHaul,    LoadingType.ReferenceLoad),
            (MissionType.LongHaulEMS, LoadingType.LowLoading),
            (MissionType.LongHaulEMS, LoadingType.ReferenceLoad),
        };

        var resultEntries = allMissions.Select(m =>
        {
            var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory,
                VectoSimulationJobType.ConventionalVehicle, false, false, OvcHevMode.NotApplicable, null);
            runData.Mission = new Mission() { MissionType = m.Item1 };
            runData.Loading = m.Item2;
            runData.VehicleData.VehicleClass = vehicleClass;
            var entry = ReportResultTestUtils.GetResultEntry(runData);
            entry.SetResultData(runData, ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, null), 1);
            return (IResultEntry)entry;
        }).ToList();

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
            ReportResultTestUtils.GetMockInputData(3, false),
            vehicleCategory.GetVehicleType(),
            VectoSimulationJobType.ConventionalVehicle, false, false);

        // Act
        var results = resultsWriter.GenerateResults(resultEntries);
        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, CIF_NS);
        var summaries = doc.Descendants(CIF_NS + "Summary").ToList();

        // Assert: 2 summaries generated (normal + EHC); vocational is null so not written
        Assert.AreEqual(2, summaries.Count, "Expected normal + EHC summaries");

        // The EHC summary is the last one — it is NOT vocational
        var ehcSummary = summaries.Last();
        var isVocational = bool.Parse(ehcSummary.Element(CIF_NS + "Vocational")?.Value ?? "false");
        Assert.IsFalse(isVocational, "EHC summary should not be marked as vocational");
    }

    /// <summary>
    /// GetEHCSegment returns the segment row with the heaviest LongHaulEMS reference payload.
    /// Class 16 has rows with ref payloads 26500/34000/26500 kg → expects 34000 kg.
    /// Class 11/12 have rows with ref payloads 26500/31000/26500 kg → expects 31000 kg.
    /// </summary>
    [
        TestCase(VehicleClass.Class16, 34000, TestName = "GetEHCSegment_Class16_Returns34000kgRow"),
        TestCase(VehicleClass.Class11, 31000, TestName = "GetEHCSegment_Class11_Returns31000kgRow"),
        TestCase(VehicleClass.Class12, 31000, TestName = "GetEHCSegment_Class12_Returns31000kgRow"),
    ]
    public void Test_GetEHCSegment_ReturnsHeaviestLongHaulEMSRow(VehicleClass vehicleClass, double expectedRefLoadKg)
    {
        var segment = DeclarationData.TruckSegments.GetEHCSegment(vehicleClass);

        Assert.IsTrue(segment.Found, "Segment should be found for EHC class");
        var lhEmsMission = segment.Missions.FirstOrDefault(m => m.MissionType == MissionType.LongHaulEMS);
        Assert.IsNotNull(lhEmsMission, "Segment should have a LongHaulEMS mission");
        Assert.AreEqual(expectedRefLoadKg, lhEmsMission.RefLoad.Cast<Kilogram>().Value(), 1.0,
            $"Expected EHC ref payload {expectedRefLoadKg} kg for {vehicleClass}");
    }

    /// <summary>
    /// ApplyEHCCO2Interpolation: the weighted EHC result has CO2PerMeter computed as
    /// Σ(weightedFCPerKm * CO2PerFuelWeight) across all fuels.
    /// Verifies that the EHC interpolation path runs without error and yields a positive CO2 value.
    /// </summary>
    [
        TestCase(VehicleClass.Class16, VehicleCategory.RigidTruck, TestName = "CIF_EHC_ApplyInterpolation_CO2PerMeterUsesAllFuels: Class16"),
        TestCase(VehicleClass.Class11, VehicleCategory.RigidTruck, TestName = "CIF_EHC_ApplyInterpolation_CO2PerMeterUsesAllFuels: Class11"),
    ]
    public void Test_CIF_EHC_ApplyInterpolation_CO2PerMeterUsesAllFuels(VehicleClass vehicleClass, VehicleCategory vehicleCategory)
    {
        // Build the full set of missions the EHC weighting group expects, using the standard
        // GetMockRunData/GetMockModData helpers so all modal data fields are properly populated.
        var missions = new[]
        {
            (MissionType.LongHaul,            LoadingType.LowLoading),
            (MissionType.LongHaul,            LoadingType.ReferenceLoad),
            (MissionType.RegionalDelivery,    LoadingType.LowLoading),
            (MissionType.RegionalDelivery,    LoadingType.ReferenceLoad),
            (MissionType.Construction,        LoadingType.LowLoading),
            (MissionType.Construction,        LoadingType.ReferenceLoad),
            (MissionType.LongHaulEMS,         LoadingType.LowLoading),
            (MissionType.LongHaulEMS,         LoadingType.ReferenceLoad),
        };

        var resultEntries = missions.Select(m =>
        {
            var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory,
                VectoSimulationJobType.ConventionalVehicle, false, false, OvcHevMode.NotApplicable, null);
            runData.Mission = new Mission() { MissionType = m.Item1 };
            runData.Loading = m.Item2;
            runData.VehicleData.VehicleClass = vehicleClass;
            // Give LowLoading a lighter payload, ReferenceLoad a heavier one
            runData.VehicleData.Loading = m.Item2 == LoadingType.LowLoading
                ? 3500.SI<Kilogram>()
                : 19300.SI<Kilogram>();
            var entry = ReportResultTestUtils.GetResultEntry(runData);
            entry.SetResultData(runData, ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, null), 1);
            return (IResultEntry)entry;
        }).ToList();

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
            ReportResultTestUtils.GetMockInputData(3, false),
            vehicleCategory.GetVehicleType(),
            VectoSimulationJobType.ConventionalVehicle, false, false);

        // Should complete without exceptions: ApplyEHCCO2Interpolation runs on the LongHaulEMS entries
        var results = resultsWriter.GenerateResults(resultEntries);
        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, CIF_NS);

        Assert.AreEqual(3, doc.Descendants(CIF_NS + "Summary").Count(),
            "EHC group should produce 3 Summary elements");
    }


    private static readonly (MissionType, LoadingType)[] _ehcTestMissions =
    {
        (MissionType.LongHaul,         LoadingType.LowLoading),
        (MissionType.LongHaul,         LoadingType.ReferenceLoad),
        (MissionType.RegionalDelivery, LoadingType.LowLoading),
        (MissionType.RegionalDelivery, LoadingType.ReferenceLoad),
        (MissionType.LongHaulEMS,      LoadingType.LowLoading),
        (MissionType.LongHaulEMS,      LoadingType.ReferenceLoad),
    };

    private List<IResultEntry> BuildEHCEntries(VehicleClass vehicleClass, VehicleCategory vehicleCategory, FuelType[] fuels)
    {
        return _ehcTestMissions.Select(m =>
        {
            var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory,
                VectoSimulationJobType.ConventionalVehicle, false, false, OvcHevMode.NotApplicable, fuels);
            runData.Mission = new Mission() { MissionType = m.Item1 };
            runData.Loading = m.Item2;
            runData.VehicleData.VehicleClass = vehicleClass;
            runData.VehicleData.Loading = m.Item2 == LoadingType.LowLoading
                ? 3500.SI<Kilogram>()
                : 26500.SI<Kilogram>();
            var entry = ReportResultTestUtils.GetResultEntry(runData);
            entry.SetResultData(runData, ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, fuels), 1);
            return (IResultEntry)entry;
        }).ToList();
    }

    /// <summary>
    /// Dual-fuel (Diesel CI + NG PI) Class 11 EHC vehicle: the EHC Summary element must contain
    /// exactly one &lt;Fuel&gt; element per fuel type and pass schema validation.
    /// </summary>
    [
        TestCase(VehicleClass.Class11, VehicleCategory.RigidTruck, TestName = "CIF_EHC_DualFuel_EHCSummaryHasBothFuels: Class11"),
        TestCase(VehicleClass.Class12, VehicleCategory.Tractor,    TestName = "CIF_EHC_DualFuel_EHCSummaryHasBothFuels: Class12"),
    ]
    public void Test_CIF_EHC_DualFuel_EHCSummaryHasBothFuels(VehicleClass vehicleClass, VehicleCategory vehicleCategory)
    {
        var fuels = new[] { FuelType.DieselCI, FuelType.NGPI };
        var resultEntries = BuildEHCEntries(vehicleClass, vehicleCategory, fuels);

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
            ReportResultTestUtils.GetMockInputData(3, false),
            vehicleCategory.GetVehicleType(),
            VectoSimulationJobType.ConventionalVehicle, false, false);

        var results = resultsWriter.GenerateResults(resultEntries);
        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, CIF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

        ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

        // No vocational missions → 2 summaries: normal + EHC
        Assert.AreEqual(2, doc.Descendants(CIF_NS + "Summary").Count(), "Expected normal + EHC summaries");

        var ehcSummary = doc.Descendants(CIF_NS + "Summary").Last();
        var fuelTypes = ehcSummary.Descendants(CIF_NS + "Fuel")
            .Select(f => f.Attribute("type")?.Value)
            .ToList();

        CollectionAssert.Contains(fuelTypes, "Diesel CI", "EHC summary must contain Diesel CI fuel");
        CollectionAssert.Contains(fuelTypes, "NG PI", "EHC summary must contain NG PI fuel");
    }

    /// <summary>
    /// Dual-fuel EHC: each Summary element must contain exactly 2 &lt;Fuel&gt; entries — one per fuel
    /// type. Verifies the fix for the duplication bug where GetElements was called once per fuel entry.
    /// </summary>
    [
        TestCase(VehicleClass.Class11, VehicleCategory.RigidTruck, TestName = "CIF_EHC_DualFuel_FuelCount: Class11"),
        TestCase(VehicleClass.Class12, VehicleCategory.Tractor,    TestName = "CIF_EHC_DualFuel_FuelCount: Class12"),
    ]
    public void Test_CIF_EHC_DualFuel_EHCSummaryFuelCountMatchesFuelTypes(VehicleClass vehicleClass, VehicleCategory vehicleCategory)
    {
        var fuels = new[] { FuelType.DieselCI, FuelType.NGPI };
        var resultEntries = BuildEHCEntries(vehicleClass, vehicleCategory, fuels);

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
            ReportResultTestUtils.GetMockInputData(3, false),
            vehicleCategory.GetVehicleType(),
            VectoSimulationJobType.ConventionalVehicle, false, false);

        var results = resultsWriter.GenerateResults(resultEntries);
        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, CIF_NS);

        ReportResultTestUtils.WriteToConsole(doc);

        var summaries = doc.Descendants(CIF_NS + "Summary").ToList();
        foreach (var summary in summaries)
        {
            var count = summary.Descendants(CIF_NS + "Fuel").Count();
            Assert.AreEqual(2, count, $"Each Summary must have exactly 2 Fuel elements, found {count}");
        }
    }

    /// <summary>
    /// Single-fuel Class 12 EHC vehicle: the EHC Summary must have a positive CO2 value after
    /// ApplyEHCCO2Interpolation runs with distinct low/reference payloads.
    /// </summary>
    [
        TestCase(VehicleClass.Class11, VehicleCategory.RigidTruck, TestName = "CIF_EHC_SingleFuel_PositiveCO2: Class11"),
        TestCase(VehicleClass.Class12, VehicleCategory.Tractor,    TestName = "CIF_EHC_SingleFuel_PositiveCO2: Class12"),
    ]
    public void Test_CIF_EHC_SingleFuel_EHCSummaryHasPositiveCO2(VehicleClass vehicleClass, VehicleCategory vehicleCategory)
    {
        var fuels = new[] { FuelType.DieselCI };
        var resultEntries = BuildEHCEntries(vehicleClass, vehicleCategory, fuels);

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
            ReportResultTestUtils.GetMockInputData(3, false),
            vehicleCategory.GetVehicleType(),
            VectoSimulationJobType.ConventionalVehicle, false, false);

        var results = resultsWriter.GenerateResults(resultEntries);
        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, CIF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

        ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

        var ehcSummary = doc.Descendants(CIF_NS + "Summary").Last();
        var co2Text = ehcSummary.Descendants(CIF_NS + "CO2").FirstOrDefault()?.Value;
        Assert.IsNotNull(co2Text, "EHC summary must contain a CO2 element");
        Assert.IsTrue(double.TryParse(co2Text, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var co2Value), "CO2 value must be numeric");
        Assert.Greater(co2Value, 0, "EHC CO2 value must be positive");
    }

    [Test]
    public void Test_CIF_EHC_Summary_NonEHCGroupHasNoThirdSummary()
    {
        var missions = new[]
        {
            (MissionType.LongHaul,         LoadingType.LowLoading),
            (MissionType.LongHaul,         LoadingType.ReferenceLoad),
            (MissionType.RegionalDelivery, LoadingType.LowLoading),
            (MissionType.RegionalDelivery, LoadingType.ReferenceLoad),
        };

        var resultEntries = missions.Select(m =>
        {
            var runData = ReportResultTestUtils.GetMockRunData(VehicleCategory.RigidTruck,
                VectoSimulationJobType.ConventionalVehicle, false, false, OvcHevMode.NotApplicable, null);
            runData.Mission = new Mission() { MissionType = m.Item1 };
            runData.Loading = m.Item2;
            runData.VehicleData.VehicleClass = VehicleClass.Class5;
            var entry = ReportResultTestUtils.GetResultEntry(runData);
            entry.SetResultData(runData, ReportResultTestUtils.GetMockModData(VectoRun.Status.Success, null), 1);
            return (IResultEntry)entry;
        }).ToList();

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
            ReportResultTestUtils.GetMockInputData(3, false),
            VehicleCategory.RigidTruck.GetVehicleType(),
            VectoSimulationJobType.ConventionalVehicle, false, false);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, REPORT_VERSION, CIF_NS);
        var summaryCount = doc.Descendants(CIF_NS + "Summary").Count();

        Assert.AreEqual(1, summaryCount, "Expected 1 Summary element in CIF report for non-EHC group");
    }


}