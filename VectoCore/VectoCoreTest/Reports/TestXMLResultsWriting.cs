using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.ResultWriter;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.Reports;

[TestFixture]
public class TestXMLResultsWriting
{
	private StandardKernel _kernel;

	private IResultsWriterFactory _reportResultsFactory;
	//private IXMLInputDataReader _xmlReader;

	XNamespace CIF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput:v1.0");
	XNamespace MRF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v1.0");
	XNamespace VIF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1");

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		//_xmlReader = _kernel.Get<IXMLInputDataReader>();
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
		Category(Definitions.TESTCASE_MIGRATED),
	]
    public void Test_CIF_ReportResultInstance(VehicleCategory vehicleCategory, int amdm, VectoSimulationJobType jobType,  bool ovc,
		bool exempted, Type expectedResultWriterType)
	{
		var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(GetMockInputData(amdm) ,vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

		Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
	}


	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.SerialHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.IEPC_S, 2, true, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry HEV exempted"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, true, true, true, TestName = "CIF_ReportResult_WritingResults_2Amd: Lorry Conv exempted"),
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

		TestCase(VectoSimulationJobType.FCHV, 3, true, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry FCHV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.FCHV, 3, false, false, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry FCHV non-OVC SUCCESS"),
		Category(Definitions.TESTCASE_MIGRATED),
        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, true, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, true, true, true, TestName = "CIF_ReportResult_WritingResults_3Amd: Lorry Conv exempted"),
		Category(Definitions.TESTCASE_MIGRATED)
    ]
    public void Test_CIF_ReportResult_WritingResults_Lorry(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);
		resultEntries.Add(resultEntry);

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
			res2.SetResultData(run2, modData, 1);
			resultEntries.Add(res2);
		}

		if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.FuelCell) {
			var fcfuel = DeclarationData.FuelData.Lookup(FuelType.H2FC);
			resultEntry.FuelData.Add(fcfuel);
			resultEntry.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
			if (ovc) {
				var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
				var res2 = GetResultEntry(run2);
				res2.SetResultData(run2, modData, 1);
				resultEntries.Add(res2);
				res2.FuelData.Add(fcfuel);
				res2.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
            }
        }
		
        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(GetMockInputData(amdm),
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputCustomer.0.9", CIF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

		WriteToFile("CIF", doc, amdm, runData, success, exempted);
	}


	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "CIF_ReportResult_WritingResult_2Amd: CompletedBus HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "CIF_ReportResult_WritingResults_2Amd: CompletedBus HEV exempted"),
		Category(Definitions.TESTCASE_MIGRATED)
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
		TestCase(VectoSimulationJobType.FCHV, 3, false, false, true, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus FCHV non-OVC SUCCESS"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, true, true, TestName = "CIF_ReportResult_WritingResults_3Amd: CompletedBus HEV exempted"),
		Category(Definitions.TESTCASE_MIGRATED)
    ]
    public void TestReportResult_WritingResults_CompletedBus(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.HeavyBusCompletedVehicle;
		var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		runData.InputData = GetMockInputData(amdm);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);
		resultEntries.Add(resultEntry);

		if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.PureElectric ||
			(fuels.All(f => f.IsHydrogenFuel()))) {
			resultEntry.AuxHeaterFuel = FuelData.Diesel;
			resultEntry.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
			resultEntry.ZEV_CO2 = resultEntry.ZEV_FuelConsumption_AuxHtr * resultEntry.AuxHeaterFuel.CO2PerFuelWeight;
		}

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
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
				var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
				var res2 = GetResultEntry(run2);
				res2.SetResultData(run2, modData, 1);
				resultEntries.Add(res2);
				res2.FuelData.Add(fcfuel);
				res2.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
				res2.AuxHeaterFuel = FuelData.Diesel;
				res2.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
				res2.ZEV_CO2 = res2.ZEV_FuelConsumption_AuxHtr * res2.AuxHeaterFuel.CO2PerFuelWeight;
            }
		}

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(GetMockInputData(amdm),
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputCustomer.1.0", CIF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

		WriteToFile("CIF", doc, amdm, runData, success, exempted);
	}


	// ---------------------------
	// MRF Tests

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
		Category(Definitions.TESTCASE_MIGRATED)
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
		Category(Definitions.TESTCASE_MIGRATED)
	]
    public void Test_MRF_ReportResultInstance(VehicleCategory vehicleCategory, VectoSimulationJobType jobType, int amdm, bool ovc,
		bool exempted, Type expectedResultWriterType)
	{
		var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(GetMockInputData(amdm), vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

		Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
	}


	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.SerialHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.IEPC_S, 2, true, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry HEV exempted"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, true, true, true, TestName = "MRF_ReportResult_WritingResults_2Amd: Lorry Conv exempted"),
		Category(Definitions.TESTCASE_MIGRATED)
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

		TestCase(VectoSimulationJobType.FCHV, 3, false, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry FCHV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.FCHV, 3, true, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry FCHV OVC SUCCESS"),

        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, true, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, true, true, true, TestName = "MRF_ReportResult_WritingResults_3Amd: Lorry Conv exempted"),
		Category(Definitions.TESTCASE_MIGRATED)
    ]
    public void Test_MRF_ReportResult_WritingResults_Lorry(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);

		resultEntries.Add(resultEntry);

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
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
				var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
				var res2 = GetResultEntry(run2);
				res2.SetResultData(run2, modData, 1);
				resultEntries.Add(res2);
				res2.FuelData.Add(fcfuel);
				res2.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
				res2.AuxHeaterFuel = FuelData.Diesel;
				res2.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
				res2.ZEV_CO2 = res2.ZEV_FuelConsumption_AuxHtr * res2.AuxHeaterFuel.CO2PerFuelWeight;
			}
		}

        var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(GetMockInputData(amdm),
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputManufacturer.0.9", MRF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

		WriteToFile("MRF", doc, amdm, runData, success, exempted);
	}

	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.SerialHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.IEPC_S, 2, true, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus HEV exempted"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, true, true, true, TestName = "MRF_ReportResult_WritingResults_2Amd: CompletedBus Conv exempted"),
		Category(Definitions.TESTCASE_MIGRATED)
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

		TestCase(VectoSimulationJobType.FCHV, 3, false, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus FCHV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.FCHV, 3, true, false, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus FCHV OVC SUCCESS"),


        TestCase(VectoSimulationJobType.ParallelHybridVehicle, 3, true, true, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus HEV exempted"),
        TestCase(VectoSimulationJobType.ConventionalVehicle, 3, true, true, true, TestName = "MRF_ReportResult_WritingResults_3Amd: CompletedBus Conv exempted"),
		Category(Definitions.TESTCASE_MIGRATED)
    ]
    public void Test_MRF_ReportResult_WritingResults_Bus(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.HeavyBusCompletedVehicle;
		var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);
		
		if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.PureElectric ||
			fuels.All(f => f.IsHydrogenFuel())) {
			resultEntry.AuxHeaterFuel = FuelData.Diesel;
			resultEntry.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
			resultEntry.ZEV_CO2 = resultEntry.ZEV_FuelConsumption_AuxHtr * resultEntry.AuxHeaterFuel.CO2PerFuelWeight;
		}

        resultEntries.Add(resultEntry);

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
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
				var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
				var res2 = GetResultEntry(run2);
				res2.SetResultData(run2, modData, 1);
				resultEntries.Add(res2);
				res2.FuelData.Add(fcfuel);
				res2.CorrectedFinalFuelConsumption[FuelType.H2FC] = new FuelCellFuelConsumptionCorrection(fcfuel, 0.SI<KilogramPerWattSecond>(), 1.SI<Kilogram>(), 0.SI<Kilogram>(), modData.Duration, modData.Distance);
				res2.AuxHeaterFuel = FuelData.Diesel;
				res2.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
				res2.ZEV_CO2 = res2.ZEV_FuelConsumption_AuxHtr * res2.AuxHeaterFuel.CO2PerFuelWeight;
			}
		}

        var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(GetMockInputData(amdm),
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputManufacturer.0.9", MRF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

		WriteToFile("MRF", doc, amdm, runData, success, exempted);
	}

	// ------------
	// VIF Tests

	[
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.ConventionalVehicle, 2, false, true, typeof(VIFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.ConventionalVehicle, 2, false, false, typeof(VIFResultsWriter.ConventionalBus)),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, typeof(VIFResultsWriter.HEVNonOVCBus)),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.SerialHybridVehicle, 2, true, false, typeof(VIFResultsWriter.HEVOVCBus)),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, typeof(VIFResultsWriter.PEVBus)),
		Category(Definitions.TESTCASE_MIGRATED)
	]
	public void Test_VIF_ReportResultInstance(VehicleCategory vehicleCategory, VectoSimulationJobType jobType, int amdm, bool ovc,
		bool exempted, Type expectedResultWriterType)
	{
		
        var resultsWriter = _reportResultsFactory.GetVIFResultsWriter(GetMockInputData(amdm), vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

		Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
	}

	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, true, FuelType.NGPI, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, false, false, false, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, true, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.SerialHybridVehicle, 2, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.IEPC_S, 2, true, false, true, FuelType.NGPI, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, false, false, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, true, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, false, false, false, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, true, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, 2, true, false, false, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, 2, true, true, true, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus HEV exempted"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, 2, true, true, true, TestName = "VIF_ReportResult_WritingResults_2Amd: PrimaryBus Conv exempted"),
		Category(Definitions.TESTCASE_MIGRATED)
	]
	public void Test_VIF_ReportResult_WritingResults_Bus(VectoSimulationJobType jobType, int amdm, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle;
		var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);

		resultEntries.Add(resultEntry);

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
			res2.SetResultData(run2, modData, 1);
			resultEntries.Add(res2);
		}
		
        var resultsWriter = _reportResultsFactory.GetVIFResultsWriter(GetMockInputData(amdm),
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputMultistep.0.1", VIF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.MultistepOutputData), validator.ValidationError);

		WriteToFile("VIF", doc, amdm, runData, success, exempted);
	}

	// ---------

	[TestCase(),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void TestCalculateOVCWeightedResult(params FuelType[] fuels)
	{
		var jobType = VectoSimulationJobType.ParallelHybridVehicle;
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = OvcHevMode.ChargeDepleting;
		var runData = GetMockRunData(vehicleCategory, jobType, true, false, ovcmode, fuels);
		var modData = GetMockModData(VectoRun.Status.Success, fuels, ovcmode);

		var cdResult = GetResultEntry(runData);
		cdResult.SetResultData(runData, modData, 1);

		var run2 = GetMockRunData(vehicleCategory, jobType, true, false, OvcHevMode.ChargeSustaining, fuels);
		var modData2 = GetMockModData(VectoRun.Status.Success, fuels, OvcHevMode.ChargeSustaining);
		var csResult = GetResultEntry(run2);
		csResult.SetResultData(run2, modData2, 1);

		var weighted = DeclarationData.CalculateWeightedResult(cdResult, csResult);

		Console.WriteLine($"{weighted.ActualChargeDepletingRange.Value().ToXMLFormat(3)} {weighted.EquivalentAllElectricRange.Value().ToXMLFormat(3)} {weighted.ZeroCO2EmissionsRange.Value().ToXMLFormat(3)} {weighted.UtilityFactor.ToXMLFormat(3)}" +
						$" {weighted.ElectricEnergyConsumption.Value().ToXMLFormat(3)} {weighted.FuelConsumption[FuelData.Diesel].Value().ToXMLFormat(3)} {weighted.CO2PerMeter.Value().ToXMLFormat(3)}");

		//1518.750 1366.875 1366.875 0.004 795230.237 30.890 20.000

		Assert.AreEqual(1518.750, weighted.ActualChargeDepletingRange.Value(), 1e-3);
		Assert.AreEqual(1366.875, weighted.EquivalentAllElectricRange.Value(), 1e-3);
		Assert.AreEqual(1366.875, weighted.ZeroCO2EmissionsRange.Value(), 1e-3);
		Assert.AreEqual(0.004, weighted.UtilityFactor, 1e-3);
		Assert.AreEqual(795230.237, weighted.ElectricEnergyConsumption.Value(), 1e-3);
		Assert.AreEqual(30.890, weighted.FuelConsumption[FuelData.Diesel].Value(), 1e-3);
		Assert.AreEqual(30000.0, weighted.Distance.Value(), 1e-3);
		Assert.AreEqual(20.0/30000.0, weighted.CO2PerMeter.Value(), 1e-8);

	}

    // ---------

	[TestCase(),
	Category(Definitions.TESTCASE_MIGRATED)]
	public void TestCalculateOVCWeightedResultIMC(params FuelType[] fuels)
	{
		var jobType = VectoSimulationJobType.ParallelHybridVehicle;
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = OvcHevMode.ChargeDepleting;
		var runData = GetMockRunData(vehicleCategory, jobType, true, false, ovcmode, fuels, IMCTechnology.OverheadPantograph);
		var modData = GetMockModData(VectoRun.Status.Success, fuels, ovcmode);

		var cdResult = GetResultEntry(runData);
		cdResult.SetResultData(runData, modData, 1);

		var run2 = GetMockRunData(vehicleCategory, jobType, true, false, OvcHevMode.ChargeSustaining, fuels, IMCTechnology.OverheadPantograph);
		var modData2 = GetMockModData(VectoRun.Status.Success, fuels, OvcHevMode.ChargeSustaining);
		var csResult = GetResultEntry(run2);
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

	// ---------

    [TestCase()]
	public void TestCalculatePEVRanges(params FuelType[] fuels)
	{
		var jobType = VectoSimulationJobType.ParallelHybridVehicle;
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = OvcHevMode.ChargeDepleting;
		var runData = GetMockRunData(vehicleCategory, jobType, true, false, ovcmode, fuels);
		var modData = GetMockModData(VectoRun.Status.Success, fuels, ovcmode);

		//var result = GetResultEntry(runData);
		//result.SetResultData(runData, modData, 1);


		var weighted = DeclarationData.CalculateElectricRangesPEV(runData, modData);

		Console.WriteLine($"{weighted.ActualChargeDepletingRange.Value().ToXMLFormat(3)} {weighted.EquivalentAllElectricRange.Value().ToXMLFormat(3)} {weighted.ZeroCO2EmissionsRange.Value().ToXMLFormat(3)} {weighted.ElectricEnergyConsumption.ConvertToKiloWattHour().ToXMLFormat(3)}");

		//1518.750 1366.875 1366.875 0.004 797877.345 30.890 20.000

		Assert.AreEqual(1518.750, weighted.ActualChargeDepletingRange.Value(), 1e-3);
		Assert.AreEqual(1518.750, weighted.EquivalentAllElectricRange.Value(), 1e-3);
		Assert.AreEqual(1518.750, weighted.ZeroCO2EmissionsRange.Value(), 1e-3);

		Assert.AreEqual(55.584, weighted.ElectricEnergyConsumption.ConvertToKiloWattHour(), 1e-3);
    }

	[TestCase()]
	public void TestCalculatePEVRangesIMC(params FuelType[] fuels)
	{
		var jobType = VectoSimulationJobType.ParallelHybridVehicle;
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = OvcHevMode.ChargeDepleting;
		var runData = GetMockRunData(vehicleCategory, jobType, true, false, ovcmode, fuels, IMCTechnology.OverheadPantograph);
		var modData = GetMockModData(VectoRun.Status.Success, fuels, ovcmode);

		//var result = GetResultEntry(runData);
		//result.SetResultData(runData, modData, 1);


		var weighted = DeclarationData.CalculateElectricRangesPEV(runData, modData);

		Console.WriteLine($"{weighted.ActualChargeDepletingRange.Value().ToXMLFormat(3)} {weighted.EquivalentAllElectricRange.Value().ToXMLFormat(3)} {weighted.ZeroCO2EmissionsRange.Value().ToXMLFormat(3)} {weighted.ElectricEnergyConsumption.ConvertToKiloWattHour().ToXMLFormat(3)}");

		//1518.750 1366.875 1366.875 0.004 797877.345 30.890 20.000

		Assert.AreEqual(1518.750, weighted.ActualChargeDepletingRange.Value(), 1e-3);
		Assert.AreEqual(1518.750, weighted.EquivalentAllElectricRange.Value(), 1e-3);
		Assert.AreEqual(1518.750, weighted.ZeroCO2EmissionsRange.Value(), 1e-3);

		Assert.AreEqual(53.676, weighted.ElectricEnergyConsumption.ConvertToKiloWattHour(), 1e-3);

	}

    // ===================================

    private static void WriteToConsole(XDocument doc)
	{
		var m = new MemoryStream();
		var writer = new XmlTextWriter(m, Encoding.UTF8) { Formatting = Formatting.Indented };
		doc.WriteTo(writer);
		writer.Flush();
		m.Flush();
		m.Seek(0, SeekOrigin.Begin);
		Console.WriteLine(new StreamReader(m).ReadToEnd());
	}

    
	private void WriteToFile(string prefix, XDocument doc, int amdm, VectoRunData runData, bool success, bool exempted)
	{
		lock (this) {
			var fileName = GetFilename(prefix, amdm, runData, success, exempted);
			var filePath = Path.Combine("TestDummyResults", fileName);
			if (!Directory.Exists(Path.GetDirectoryName(filePath))) {
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			}

			if (File.Exists(filePath)) {
				File.Delete(filePath);
			}

			var writer = new XmlTextWriter(filePath, Encoding.UTF8) { Formatting = Formatting.Indented };
			doc.WriteTo(writer);
			writer.Flush();
		}
	}

	private string GetFilename(string prefix, int amdm, VectoRunData runData, bool success, bool exempted)
	{
		var arch = string.Empty;
		switch (runData.JobType.GetPowertrainArchitectureType()) {
			case VectoSimulationJobTypeHelper.Hybrid:
				arch = (runData.VehicleData.OffVehicleCharging ? "OVC" : "non-OVC") + "-HEV";
				break;
			case VectoSimulationJobTypeHelper.Conventional:
				arch = "Conv";
				break;
			case VectoSimulationJobTypeHelper.PureElectric:
				arch = "PEV";
				break;
			case VectoSimulationJobTypeHelper.FuelCell:
				arch = (runData.VehicleData.OffVehicleCharging ? "OVC" : "non-OVC") + "FCHV";
				break;
		}

		var category = runData.VehicleData.VehicleCategory.IsLorry() ? "Lorry" : "Bus";
		var suffix = success ? null : "_ERR";
		var exept = exempted ? "_exempted" : null;

		var fuelSuffix = "";
		var fuels = runData.EngineData?.Fuels;
		if (fuels?.Any(x => x.FuelData.FuelType != FuelType.DieselCI) ?? false) {
			fuelSuffix = "_" + fuels.Select(x => x.FuelData.FuelType.ToXMLFormat().Replace(' ', '-')).Join("_");
		}
		return $"{prefix}_Amd{amdm}_MockupResults_{arch}_{category}{fuelSuffix}{suffix}{exept}.xml";
	}


	private static XMLDeclarationReport.ResultEntry GetResultEntry(VectoRunData runData)
	{
		var resultEntry = new XMLDeclarationReport.ResultEntry();
		resultEntry.Initialize(runData);
		return resultEntry;
	}

	private XMLValidator GetValidator(XDocument doc)
	{
		var ms = new MemoryStream();
		var writer = new XmlTextWriter(ms, Encoding.UTF8);
		doc.WriteTo(writer);
		writer.Flush();
		ms.Flush();
		ms.Seek(0, SeekOrigin.Begin);
		return new XMLValidator(new XmlTextReader(ms));
	}

	private XDocument CreateXmlDocument(XElement results, string reportType, XNamespace ns)
	{
		var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		
		var doc = new XDocument();
		doc.Add(new XElement(ns + "VectoMockResults",
			new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
			new XAttribute(xsi + "schemaLocation", $@"{ns.NamespaceName} V:/VectoCore/VectoCore/Resources/XSD/{reportType}.xsd"),
			results));

		return doc;
	}

	private IModalDataContainer GetMockModData(VectoRun.Status runStatus, FuelType[] fuelTypes, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
	{
		var fuels = fuelTypes == null || fuelTypes.Length == 0 ? new[] { FuelType.DieselCI } : fuelTypes;

		var modData = new Mock<IModalDataContainer>();
		modData.Setup(x => x.RunStatus).Returns(runStatus);
		modData.Setup(x => x.Duration).Returns(3600.SI<Second>());
		modData.Setup(x => x.Distance).Returns(30000.SI<Meter>());
		modData.Setup(x => x.GetValues<MeterPerSecond>(ModalResultField.v_act)).Returns(new[] { 0.KMPHtoMeterPerSecond(), 50.KMPHtoMeterPerSecond() });
		modData.Setup(x => x.GetValues<MeterPerSquareSecond>(ModalResultField.acc)).Returns(new[] { -1.SI<MeterPerSquareSecond>(), 0.SI<MeterPerSquareSecond>(), 1.SI<MeterPerSquareSecond>()});
		modData.Setup(x => x.GetValues<uint>(ModalResultField.Gear)).Returns(new[] { 0u, 2u, 0u, 3u, 0u });

		var e_gbxIn = 1000.SI<WattSecond>();
		var gbxEff = 0.98;
		var axlEff = 0.97;
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn);
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff);
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff * axlEff);

		var batChgEff = 0.95;
		var batDischgEff = 0.93;
		var batEnergy = 200.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();
		var factorChg = ovcMode.IsOneOf(OvcHevMode.ChargeSustaining, OvcHevMode.NotApplicable) ? 1 : 0.1;
        var batteryEntries = new[] {
			// internal , terminal
			Tuple.Create(batEnergy * factorChg, batEnergy * factorChg / batChgEff),
			Tuple.Create(-batEnergy, -batEnergy * batDischgEff)
		};
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_int, It.IsAny<Func<SI, bool>>()))
			.Returns<ModalResultField, Func<SI, bool>>((_, f) =>
				batteryEntries.Select(x => x.Item1).Where(x => f == null || f(x)).Sum());
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_terminal, It.IsAny<Func<SI, bool>>()))
			.Returns<ModalResultField,
				Func<SI, bool>>((_, f) => batteryEntries.Select(x => x.Item2).Where(x => f(x)).Sum());
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_terminal_ES, It.IsAny<Func<SI, bool>>()))
			.Returns<ModalResultField,
				Func<SI, bool>>((_, f) => batteryEntries.Select(x => x.Item2).Where(x => f(x)).Sum());
		modData.Setup(x => x.GetValues<SI>(ModalResultField.REESSStateOfCharge)).Returns(() => new[] { 50.SI(), 50.SI() });

        if (runStatus != VectoRun.Status.Success) {
			modData.Setup(x => x.Error).Returns("TestCase Error!");
			modData.Setup(x => x.StackTrace).Returns("Testcase Stacktrace");
		}

		var mc = new Mock<ICorrectedModalData>();
		modData.Setup(x => x.CorrectedModalData).Returns(mc.Object);

		var fcCorrected = new Dictionary<FuelType, IFuelConsumptionCorrection>();
		var ovcFactor = ovcMode == OvcHevMode.ChargeDepleting ? 0.1 : 1.0;
		foreach (var fuelType in fuels) {
			var factor = fcCorrected.Count == 0 ? 1 : 0.1;
			var fc = new Mock<IFuelConsumptionCorrection>();
			fc.Setup(x => x.Fuel).Returns(DeclarationData.FuelData.Lookup(fuelType, TankSystem.Liquefied));
			fc.Setup(x => x.TotalFuelConsumptionCorrected).Returns(31.SI<Kilogram>() * factor * ovcFactor);
			fc.Setup(x => x.EnergyDemand).Returns(31.SI<Kilogram>() * factor * ovcFactor * FuelData.Diesel.LowerHeatingValueVecto);
			fc.Setup(x => x.FC_AUXHTR_KM).Returns(0.SI<KilogramPerMeter>());
			fcCorrected.Add(fuelType, fc.Object);
		}
		mc.Setup(x => x.FuelCorrection).Returns(fcCorrected);

		mc.Setup(x => x.CO2Total).Returns(20.SI<Kilogram>());
		mc.Setup(x => x.FuelEnergyConsumptionTotal).Returns(1e9.SI<Joule>());

		var elOvcFactor = ovcMode == OvcHevMode.ChargeSustaining ? 0 : 1.0;
		mc.Setup(x => x.ElectricEnergyConsumption_Final).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);
        mc.Setup(x => x.ElectricEnergyConsumption_SoC).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);
		mc.Setup(x => x.ElectricEnergyConsumption_SoC_Corr).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);

        return modData.Object;
	}

	private VectoRunData GetMockRunData(VehicleCategory vehicleCategory, VectoSimulationJobType jobType,
		bool offVehicleCharging, bool exempted, OvcHevMode ovcMode, FuelType[] fuelTypes, IMCTechnology? imcTech = null)
	{
		var fuels = fuelTypes == null || fuelTypes.Length == 0 ? new [] { FuelType.DieselCI } : fuelTypes;
		var retVal = new VectoRunData() {
			Mission = new Mission() {
				MissionType = MissionType.LongHaul
			},
			OVCMode = ovcMode,
			InMotionCharging = imcTech.HasValue && imcTech != IMCTechnology.NotApplicable,
			InMotionChargingTechnology = imcTech ?? IMCTechnology.NotApplicable, 
			Exempted = exempted,
			JobType = jobType,
			Loading = LoadingType.LowLoading,
			MaxChargingPower = 250.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
			VehicleData = new VehicleData() {
				CurbMass = 7600.SI<Kilogram>(),
				Loading = 5000.SI<Kilogram>(),
				CargoVolume = 20.SI<CubicMeter>(),
				PassengerCount = 20,
				VehicleClass = VehicleClass.Class5,
				VehicleCategory = vehicleCategory,
				OffVehicleCharging = offVehicleCharging,
				H2StorageUsableCapacity = fuels.Any(x => x.IsHydrogenFuel()) ? 80.SI<Kilogram>() : null
			},
			EngineData = new CombustionEngineData() {
				FuelMode = 0,
				Fuels = fuels.Select(x => new CombustionEngineFuelData()
					{ FuelData = DeclarationData.FuelData.Lookup(x, TankSystem.Liquefied) }).ToList(),
			},
			Retarder = new RetarderData() {
				Type = RetarderType.None,
			},
			BatteryData = new BatterySystemData() {
				Batteries = new List<Tuple<int, BatteryData>>() {
					Tuple.Create(1, new BatteryData() {
						BatteryId = 0,
						Capacity = 7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						ChargeDepletingBattery = true,
						MinSOC = 0.2,
						MaxSOC = 0.8,
						SOCMap = BatterySOCReader.Create("SoC, V\n0, 600\n100, 650\n".ToStream()),
						InternalResistance = BatteryInternalResistanceReader.Create("SoC, Ri-2, Ri-10, Ri-20\n0, 20, 20, 20\n100, 20, 20, 20\n".ToStream(), true),
						MaxCurrent = BatteryMaxCurrentReader.Create("SoC, I_charge, I_discharge\n0, 300, 300\n100, 500, 500\n".ToStream())
						
					})
				}
			}
		};
		if (jobType.IsBatteryElectric() || jobType.IsFCHV()) {
			retVal.EngineData = null;
		}
		return retVal;
	}

    private IDeclarationInputDataProvider GetMockInputData(int amdm)
	{
		var xmlType = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24 + ":FOO";

		var mock = new Mock<IDeclarationInputDataProvider>();
		var inputDataSource = new DataSource() {
			SourceType = DataSourceType.XMLFile
		};
		mock.Setup(i => i.DataSource).Returns(inputDataSource);
		var xmlNS = "";
		switch (amdm) {
			case 2:
				xmlNS = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
				break;
			case 3:
				xmlNS = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;
				break;

        }

		var vehicleDataSource = new DataSource() {
			TypeVersion = xmlNS,
		};
		mock.Setup(i => i.JobInputData.Vehicle.DataSource).Returns(vehicleDataSource);

		var pack = new Mock<IBatteryPackDeclarationInputData>();
		var b1 = new Mock<IElectricStorageDeclarationInputData>();
		var bat = new Mock<IElectricStorageSystemDeclarationInputData>();
		pack.Setup(p => p.StorageType).Returns(REESSType.Battery);
		pack.Setup(p => p.MinSOC).Returns(0.2);
		pack.Setup(p => p.MaxSOC).Returns(0.8);
		pack.Setup(p => p.MaxCurrentMap).Returns(InputDataHelper.InputDataAsTableData("SoC, I_charge, I_discharge", new[] { "0, 300, 300", "100, 500, 500" }));
		pack.Setup(p => p.Capacity).Returns(7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>());
		pack.Setup(p => p.InternalResistanceCurve)
			.Returns(InputDataHelper.InputDataAsTableData("SoC, Ri-2, Ri-10, Ri-20",
				new[] { "0, 20, 20, 20", "100, 20, 20, 20" }));
		pack.Setup(p => p.VoltageCurve)
			.Returns(InputDataHelper.InputDataAsTableData("SoC, V", new[] { "0, 600", "100, 650" }));
		pack.Setup(p => p.DataSource).Returns(new DataSource() {
			SourceType = DataSourceType.XMLFile
		});
		b1.Setup(b => b.Count).Returns(1);
		b1.Setup(b => b.StringId).Returns(0);
		b1.Setup(b => b.REESSPack).Returns(pack.Object);
		bat.Setup(b => b.ElectricStorageElements).Returns(new[] { b1.Object });

		mock.Setup(i => i.JobInputData.Vehicle.Components.ElectricStorage).Returns(bat.Object);

        return mock.Object;
	}
}