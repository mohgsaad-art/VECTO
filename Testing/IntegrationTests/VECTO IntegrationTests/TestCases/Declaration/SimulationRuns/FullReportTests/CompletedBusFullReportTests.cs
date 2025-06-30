using System.Xml;
using TUGraz.Vecto.IntegrationTests.Utils;
using TUGraz.Vecto.IntegrationTests.Utils.DummyRun;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.IntegrationTests.TestCases.Declaration.SimulationRuns.FullReportTests;

public class CompletedBusFullReportTests : FullReportTestsBase
{

    #region PrimaryBus

    protected const string Conventional_PrimaryBus = BasePath + "PrimaryBus/Conventional_primaryBus_AMT.xml";
    protected const string Conventional_PrimaryBus_AT_Angledrive = BasePath + "PrimaryBus/Conventional_primaryBus_AT_Angledrive.xml";
    protected const string Conventional_PrimaryBus_NoRetarder = BasePath + "PrimaryBus/Conventional_primaryBus_AT_NoRetarder.xml";
    protected const string Conventional_PrimaryBus_RetarderMeasured = BasePath + "PrimaryBus/Conventional_primaryBus_AMT_RetarderMeasured.xml";
	protected const string HEV_Px_PrimaryBus_OVC = BasePath + "PrimaryBus/HEV_primaryBus_AMT_Px_OVC.xml";
    protected const string Conventional_PrimaryBus_Tyres = BasePath + "PrimaryBus/Conventional_primaryBus_AMT_DifferentTyres.xml";
    protected const string HEV_Px_PrimaryBus = BasePath + "PrimaryBus/HEV_primaryBus_AMT_Px.xml";
    protected const string HEV_Px_PrimaryBus_BatteryStd = BasePath + "PrimaryBus/HEV_primaryBus_AMT_Px_BatteryStd.xml";
    protected const string HEV_IHPC_PrimaryBus = BasePath + "PrimaryBus/HEV_primaryBus_AMT_IHPC.xml";
    protected const string HEV_IHPC_PrimaryBus_NoRetarder = BasePath + "PrimaryBus/HEV_primaryBus_AMT_IHPC_NoRetarder.xml";
    protected const string HEV_Px_PrimaryBus_SuperCap = BasePath + "PrimaryBus/HEV_primaryBus_AMT_Px_SuperCap.xml";
    protected const string HEV_S2_PrimaryBus = BasePath + "PrimaryBus/HEV-S_primaryBus_AMT_S2.xml";
    protected const string HEV_S2_PrimaryBus_GenSetADC = BasePath + "PrimaryBus/HEV-S_primaryBus_AMT_S2_GenSetADC.xml";
    protected const string HEV_S2_PrimaryBus_ADC = BasePath + "PrimaryBus/HEV-S_primaryBus_AMT_S2_ADC.xml";
    protected const string HEV_S3_PrimaryBus = BasePath + "PrimaryBus/HEV-S_primaryBus_S3.xml";
    protected const string HEV_S4_PrimaryBus = BasePath + "PrimaryBus/HEV-S_primaryBus_S4.xml";
    protected const string HEV_IEPC_S_PrimaryBus = BasePath + "PrimaryBus/HEV-S_primaryBus_IEPC-S.xml";
    protected const string HEV_IEPC_S_PrimaryBus_BatteryStd = BasePath + "PrimaryBus/HEV-S_primaryBus_IEPC-S_BatteryStd.xml";
    protected const string PEV_E2_PrimaryBus = BasePath + "PrimaryBus/PEV_primaryBus_AMT_E2.xml";
    protected const string PEV_E3_PrimaryBus = BasePath + "PrimaryBus/PEV_primaryBus_E3.xml";
    protected const string PEV_E4_PrimaryBus = BasePath + "PrimaryBus/PEV_primaryBus_E4.xml";
    protected const string PEV_IEPC_PrimaryBus = BasePath + "PrimaryBus/IEPC_primaryBus.xml";
    protected const string PEV_IEPC_PrimaryBus_Gbx1 = BasePath + "PrimaryBus/IEPC_primaryBus_Gbx1.xml";
    protected const string PEV_IEPC_PrimaryBus_Gbx1Axl = BasePath + "PrimaryBus/IEPC_primaryBus_Gbx1Axl.xml";
    protected const string PEV_IEPC_PrimaryBus_Gbx1Whl = BasePath + "PrimaryBus/IEPC_primaryBus_Gbx1Whl.xml";
    protected const string PEV_IEPC_PrimaryBus_Gbx2 = BasePath + "PrimaryBus/IEPC_primaryBus_Gbx2.xml";
    protected const string PEV_IEPC_PrimaryBus_Gbx2_drag = BasePath + "PrimaryBus/IEPC_primaryBus_Gbx2_drag.xml";
    protected const string PEV_IEPC_PrimaryBus_Gbx2Axl = BasePath + "PrimaryBus/IEPC_primaryBus_Gbx2Axl.xml";
    protected const string PEV_IEPC_PrimaryBus_Gbx2Axl_drag = BasePath + "PrimaryBus/IEPC_primaryBus_Gbx2Axl_drag.xml";
    protected const string PEV_IEPC_PrimaryBus_Gbx2Whl = BasePath + "PrimaryBus/IEPC_primaryBus_Gbx2Whl.xml";

    protected const string PEV_IEPC_std_PrimaryBus = BasePath + "PrimaryBus/IEPC_primaryBus_StdValues.xml";

    protected const string PEV_E2_PrimaryBus_StdEM = BasePath + "PrimaryBus/PEV_primaryBus_AMT_E2_EMStd.xml";
    protected const string PEV_E2_PrimaryBus_StdBat = BasePath + "PrimaryBus/PEV_primaryBus_AMT_E2_BatteryStd.xml";
    protected const string Conventional_PrimaryBus_DF = BasePath + "PrimaryBus/Conventional_primaryBus_AMT_DF.xml";

	protected const string Exempted_PrimaryBus = BasePath + "PrimaryBus/exempted_primaryBus.xml";
    #endregion

    #region Interim Bus Input

    protected const string Conventional_InterimBusInput = BasePath + "CompletedBus/Conventional_InterimBus_Min.xml";
	protected const string Conventional_InterimBusInput_AirdragV10 = BasePath + "CompletedBus/Conventional_interimBus_AirdragV10.xml";
	protected const string Conventional_InterimBusInput_AirdragV20 = BasePath + "CompletedBus/Conventional_interimBus_AirdragV20.xml";
	protected const string HEV_InterimBusInput = BasePath + "CompletedBus/HEV_InterimBus_Min.xml";
	protected const string PEV_InterimBusInput = BasePath + "CompletedBus/PEV_InterimBus_Min.xml";
	protected const string PEV_IEPC_InterimBusInput = BasePath + "CompletedBus/IEPC_InterimBus_Min.xml";

	protected const string Exempted_InterimBus = BasePath + "CompletedBus/exempted_completedBus_min.xml";
    #endregion

    #region Complete(d) Bus Input

    protected const string Conventional_CompletedBusInput = BasePath + "CompletedBus/Conventional_completedBus_2.xml";
	protected const string Conventional_CompletedBusInput_NoTankSystem = BasePath + "CompletedBus/Conventional_completedBus_3.xml";
    protected const string Conventional_CompletedBusInputNoAirdrag = BasePath + "CompletedBus/Conventional_completedBus_NoAirdrag.xml";
    protected const string Conventional_CompletedBusInput_TypeApproval = BasePath + "CompletedBus/Conventional_completedBus_2_TypeApprovalNumber.xml";
    protected const string Conventional_CompletedBusInput_AirdragV10 = BasePath + "CompletedBus/Conventional_completedBus_AirdragV10.xml";
    protected const string Conventional_CompletedBusInput_AirdragV20 = BasePath + "CompletedBus/Conventional_completedBus_AirdragV20.xml";
    protected const string HEV_CompletedBusInput = BasePath + "CompletedBus/HEV_completedBus_2.xml";
    protected const string PEV_CompletedBusInput = BasePath + "CompletedBus/PEV_completedBus_2.xml";
    protected const string PEV_IEPC_CompletedBusInput = BasePath + "CompletedBus/IEPC_completedBus_2.xml";

	protected const string Exempted_CompletedBus = BasePath + "CompletedBus/exempted_completedBus_2.xml";
    #endregion

    [OneTimeSetUp]
	public void OneTimeSetup()
	{
		// update all necessary bindings so that no simulation is performed
		SetupNinject();

		//WRITE_REPORTS_TO_FILESYSTEM = true;
		//      WRITE_REPORTS_TO_OUTPUT = true;
	}

	[TestCase(Conventional_PrimaryBus, Conventional_InterimBusInput, Conventional_CompletedBusInput_NoTankSystem, "Conventional", TestName = "Completed Conventional Bus")]
    [TestCase(Conventional_PrimaryBus_Tyres, Conventional_InterimBusInput, Conventional_CompletedBusInput, "Conventional", TestName = "Completed Conventional Bus Different Tyres")]
    [TestCase(Conventional_PrimaryBus_DF, Conventional_InterimBusInput, Conventional_CompletedBusInput, "Conventional", TestName = "Completed ConventionalPrimaryBus_DualFuel")]
    [TestCase(Conventional_PrimaryBus_AT_Angledrive, Conventional_InterimBusInput, Conventional_CompletedBusInput, "Conventional", TestName = "Completed Conventional Bus_AT_Angledrive")]
    [TestCase(HEV_IEPC_S_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "IEPC-S", "HEV", TestName = "Completed HEV_IEPC_S_PrimaryBus")]
    [TestCase(HEV_IEPC_S_PrimaryBus_BatteryStd, HEV_InterimBusInput, HEV_CompletedBusInput, "IEPC-S", "HEV", TestName = "Completed HEV_IEPC_S_PrimaryBus_BatteryStd")]
    [TestCase(HEV_Px_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_Px_PrimaryBus")]
	[TestCase(HEV_Px_PrimaryBus_OVC, HEV_InterimBusInput, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_Px_PrimaryBus")]
    [TestCase(HEV_Px_PrimaryBus_BatteryStd, HEV_InterimBusInput, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_Px_PrimaryBus_BatteryStd")]
    [TestCase(HEV_IHPC_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_IHPC_PrimaryBus")]
    [TestCase(HEV_IHPC_PrimaryBus_NoRetarder, HEV_InterimBusInput, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_IHPC_PrimaryBus_NoRetarder")]
    [TestCase(HEV_S2_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput,"Sx", "HEV", TestName = "Completed HEV_S2_PrimaryBus")]
    [TestCase(HEV_S3_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput,"Sx", "HEV", TestName = "Completed HEV_S3_PrimaryBus")]
    [TestCase(HEV_S4_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "Sx", "HEV", TestName = "Completed HEV_S4_PrimaryBus")]
    [TestCase(PEV_E2_PrimaryBus, PEV_InterimBusInput, PEV_CompletedBusInput, "Ex", "PEV", TestName = "Completed PEV_E2_PrimaryBus")]
    [TestCase(PEV_E3_PrimaryBus, PEV_InterimBusInput, PEV_CompletedBusInput,"Ex", "PEV", TestName = "Completed PEV_E3_PrimaryBus")]
    [TestCase(PEV_E4_PrimaryBus, PEV_InterimBusInput, PEV_CompletedBusInput,"Ex", "PEV", TestName = "Completed PEV_E4_PrimaryBus")]
    [TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx1")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1Axl, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx1Axl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1Whl, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx1Whl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2_drag, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2_drag")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2Axl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl_drag, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2Axl_drag")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Whl, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus_Gbx2Whl")]
    [TestCase(PEV_IEPC_std_PrimaryBus, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC-std_PrimaryBus")]
    [TestCase(PEV_E2_PrimaryBus_StdEM, PEV_InterimBusInput, PEV_CompletedBusInput, "Ex", "PEV", TestName = "Completed PEV_E2_PrimaryBus_EM-Std")]

    [TestCase(Exempted_PrimaryBus, Exempted_InterimBus, Exempted_CompletedBus, "Exempted", TestName = "Completed Exempted Bus")]
    public void CompletedBusFullReportSuccessTest(string primaryBusInput, string completeBusInput, string completedBusInput, params string[] expectedType)
    {
        var completeCopy = CopyInputFile(completeBusInput);
        CopyInputFile(primaryBusInput);
        // completed: VIF + complete input (full) =>  VIF , MRF Completed, CIF Completed
        // (approach: first simulate primary on its own to have an up-to-date VIF
        // (no need to maintain this in the testfiles)

        // setting up testcase 
        // run primary simulation
		var completeInputData = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput),
			XmlReader.Create(completeBusInput), _inputDataReader);
        var fileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, primaryBusInput);
        var sumWriter = new SummaryDataContainer(null);
        var jobContainer = new JobContainer(sumWriter);

        var _simulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completeInputData, fileWriter, null, null, true);

        Clearfiles(fileWriter); //remove files from previous test runs
        jobContainer.AddRuns(_simulatorFactory);
        jobContainer.Execute(false);
        jobContainer.WaitFinished();

        CheckReportExists(fileWriter, CifShouldExist: false, MrfShouldExist: true, VifShouldExist: true);
        //File.Delete(fileWriter.XMLFullReportName);
        var primaryVif = CopyInputFile(fileWriter.XMLPrimaryVehicleReportName);
        // done preparing testcase...

        // this is the actual test: run completed simulation

		var completedInputData = new DummyRunVIFWithInterimBusInputDataProvider(fileWriter.XMLMultistageReport,
			XmlReader.Create(completedBusInput), _inputDataReader, true);
        var completedFileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, completedBusInput);
        var completedSumWriter = new SummaryDataContainer(null);
        var completedJobContainer = new JobContainer(completedSumWriter);

        var completedSimulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completedInputData, completedFileWriter, null, null, true);

        Clearfiles(completedFileWriter); //remove files from previous test runs
        completedJobContainer.AddRuns(completedSimulatorFactory);
        completedJobContainer.Execute(false);
        completedJobContainer.WaitFinished();

        // assertions
        //File.Delete(fileWriter.XMLPrimaryVehicleReportName);

        CheckReportExists(completedFileWriter, CifShouldExist: true, MrfShouldExist: true, VifShouldExist: true);

        CheckElementTypeNameContains(completedFileWriter.XMLMultistageReport, "Vehicle", expectedType);
    }


	[TestCase(Conventional_PrimaryBus_Tyres, Conventional_InterimBusInput, Conventional_CompletedBusInput, "Conventional", TestName = "Completed Conventional Bus Different Tyres Error")]
	[TestCase(HEV_Px_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_Px_PrimaryBus Error")]
	[TestCase(HEV_Px_PrimaryBus_OVC, HEV_InterimBusInput, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_Px_PrimaryBus Error")]
	[TestCase(HEV_S4_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "Sx", "HEV", TestName = "Completed HEV_S4_PrimaryBus Error")]
	[TestCase(PEV_E2_PrimaryBus, PEV_InterimBusInput, PEV_CompletedBusInput, "Ex", "PEV", TestName = "Completed PEV_E2_PrimaryBus Error")]
    public void CompletedBusFullReportErrorTest(string primaryBusInput, string completeBusInput, string completedBusInput, params string[] expectedType)
    {
        var completeCopy = CopyInputFile(completeBusInput);
        CopyInputFile(primaryBusInput);
        // completed: VIF + complete input (full) =>  VIF , MRF Completed, CIF Completed
        // (approach: first simulate primary on its own to have an up-to-date VIF
        // (no need to maintain this in the testfiles)

        // setting up testcase 
        // run primary simulation
        var completeInputData = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput),
            XmlReader.Create(completeBusInput), _inputDataReader);
        var fileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, primaryBusInput);
        var sumWriter = new SummaryDataContainer(null);
        var jobContainer = new JobContainer(sumWriter);

        var _simulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completeInputData, fileWriter, null, null, true);

        Clearfiles(fileWriter); //remove files from previous test runs
        jobContainer.AddRuns(_simulatorFactory);
        jobContainer.Execute(false);
        jobContainer.WaitFinished();

        CheckReportExists(fileWriter, CifShouldExist: false, MrfShouldExist: true, VifShouldExist: true);
        //File.Delete(fileWriter.XMLFullReportName);
        var primaryVif = CopyInputFile(fileWriter.XMLPrimaryVehicleReportName);
        // done preparing testcase...

        var completedInputData = new DummyRunVIFWithInterimBusInputDataProvider(fileWriter.XMLMultistageReport,
            XmlReader.Create(completedBusInput), _inputDataReader, false);
        var completedFileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, completedBusInput);
        var completedSumWriter = new SummaryDataContainer(null);
        var completedJobContainer = new JobContainer(completedSumWriter);

        var completedSimulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completedInputData, completedFileWriter, null, null, true);

        Clearfiles(completedFileWriter); //remove files from previous test runs
        completedJobContainer.AddRuns(completedSimulatorFactory);
		completedJobContainer.Execute(false);
        completedJobContainer.WaitFinished();

        CheckReportExists(completedFileWriter, CifShouldExist: false, MrfShouldExist: false, VifShouldExist: true);
		CheckElementTypeNameContains(completedFileWriter.XMLMultistageReport, "Vehicle", expectedType);

		// this is the actual test: run completed simulation
		var completedInputData2 =
			new DummyRunVIFWithInterimBusInputDataProvider(completedFileWriter.XMLMultistageReport, null,
				_inputDataReader, true);
		var completedFileWriter2 = GetReportWriter(TestContext.CurrentContext.Test.Name, completedBusInput);
		var completedSumWriter2 = new SummaryDataContainer(null);
		var completedJobContainer2 = new JobContainer(completedSumWriter2);

		var completedSimulatorFactory2 =
			_simFactoryFactory.Factory(ExecutionMode.Declaration, completedInputData2, completedFileWriter2, null, null, true);

		Clearfiles(completedFileWriter2); //remove files from previous test runs
		completedJobContainer2.AddRuns(completedSimulatorFactory2);
		(completedJobContainer2.Runs[0].Run as DummyRunNonExemptedRun).FinishedWithError = true;
        completedJobContainer2.Execute(false);
		AssertHelper.Exception<Exception>(() => completedJobContainer2.WaitFinished());
    }

    [TestCase(Conventional_PrimaryBus_DF, Conventional_InterimBusInput, Conventional_CompletedBusInput, "Conventional", TestName = "Completed ConventionalPrimaryBus_DualFuel Ignore")]
	[TestCase(HEV_Px_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_Px_PrimaryBus Ignore")]
	[TestCase(HEV_S4_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "Sx", "HEV", TestName = "Completed HEV_S4_PrimaryBus Ignore")]
	[TestCase(PEV_E4_PrimaryBus, PEV_InterimBusInput, PEV_CompletedBusInput, "Ex", "PEV", TestName = "Completed PEV_E4_PrimaryBus Ignore")]
	[TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus Ignore")]
    public void CompletedBusFullReportIgnoreTest(string primaryBusInput, string completeBusInput, string completedBusInput, params string[] expectedType)
    {
        var completeCopy = CopyInputFile(completeBusInput);
        CopyInputFile(primaryBusInput);
        // completed: VIF + complete input (full) =>  VIF , MRF Completed, CIF Completed
        // (approach: first simulate primary on its own to have an up-to-date VIF
        // (no need to maintain this in the testfiles)

        // setting up testcase 
        // run primary simulation
        var completeInputData = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput),
            XmlReader.Create(completeBusInput), _inputDataReader);
        var fileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, primaryBusInput);
        var sumWriter = new SummaryDataContainer(null);
        var jobContainer = new JobContainer(sumWriter);

        var _simulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completeInputData, fileWriter, null, null, true);

        Clearfiles(fileWriter); //remove files from previous test runs
        jobContainer.AddRuns(_simulatorFactory);
		(jobContainer.Runs[0].Run as DummyRunNonExemptedRun).IgnoreSimulationRun = true;
        jobContainer.Execute(false);
        jobContainer.WaitFinished();

        CheckReportExists(fileWriter, CifShouldExist: false, MrfShouldExist: true, VifShouldExist: true);
        //File.Delete(fileWriter.XMLFullReportName);
        var primaryVif = CopyInputFile(fileWriter.XMLPrimaryVehicleReportName);
        // done preparing testcase...

        // this is the actual test: run completed simulation

        var completedInputData = new DummyRunVIFWithInterimBusInputDataProvider(fileWriter.XMLMultistageReport,
            XmlReader.Create(completedBusInput), _inputDataReader, true);
        var completedFileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, completedBusInput);
        var completedSumWriter = new SummaryDataContainer(null);
        var completedJobContainer = new JobContainer(completedSumWriter);

        var completedSimulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completedInputData, completedFileWriter, null, null, true);

        Clearfiles(completedFileWriter); //remove files from previous test runs
        completedJobContainer.AddRuns(completedSimulatorFactory);
        completedJobContainer.Execute(false);
        completedJobContainer.WaitFinished();

        // assertions
        //File.Delete(fileWriter.XMLPrimaryVehicleReportName);

        CheckReportExists(completedFileWriter, CifShouldExist: true, MrfShouldExist: true, VifShouldExist: true);

        CheckElementTypeNameContains(completedFileWriter.XMLMultistageReport, "Vehicle", expectedType);

		AssertElementValue(completedFileWriter.XMLManufacturerReport, XMLNames.Report_Results_Status_Success_Val,
			XMLNames.Report_Results, XMLNames.Report_Result_Status);

		if (GetElements(completedFileWriter.XMLMultistageReport, XMLNames.Report_Results,
				XMLNames.Report_Results_FuelConsumption).Any()) {
			Assert.IsTrue(
				GetElements(completedFileWriter.XMLManufacturerReport, XMLNames.Report_Results,
					XMLNames.Report_Results_FuelConsumption).Any(x => x.Value == double.NaN.ToString()));
		}

		if (GetElements(completedFileWriter.XMLMultistageReport, XMLNames.Report_Results, XMLNames.Report_Results_CO2).Any()) {
			Assert.IsTrue(
				GetElements(completedFileWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_Results_CO2)
					.Any(x => x.Value == double.NaN.ToString()));
		}

		if (GetElements(completedFileWriter.XMLMultistageReport, XMLNames.Report_Results, XMLNames.Report_ResultEntry_ElectricEnergyConsumption).Any()) {
			Assert.IsTrue(
				GetElements(completedFileWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_ResultEntry_VIF_ElectricEnergyConsumption)
					.Any(x => x.Value == double.NaN.ToString()));
		}
    }

    [TestCase(Conventional_PrimaryBus_DF, Conventional_InterimBusInput, Conventional_CompletedBusInput, "Conventional", TestName = "Completed ConventionalPrimaryBus_DualFuel IgnoreError")]
    [TestCase(HEV_Px_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "Px", "HEV", TestName = "Completed HEV_Px_PrimaryBus IgnoreError")]
    [TestCase(HEV_S4_PrimaryBus, HEV_InterimBusInput, HEV_CompletedBusInput, "Sx", "HEV", TestName = "Completed HEV_S4_PrimaryBus IgnoreError")]
    [TestCase(PEV_E4_PrimaryBus, PEV_InterimBusInput, PEV_CompletedBusInput, "Ex", "PEV", TestName = "Completed PEV_E4_PrimaryBus IgnoreError")]
    [TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_InterimBusInput, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Completed PEV_IEPC_PrimaryBus IgnoreError")]
    public void CompletedBusFullReportIgnoreErrorTest(string primaryBusInput, string completeBusInput, string completedBusInput, params string[] expectedType)
    {
        var completeCopy = CopyInputFile(completeBusInput);
        CopyInputFile(primaryBusInput);
        // completed: VIF + complete input (full) =>  VIF , MRF Completed, CIF Completed
        // (approach: first simulate primary on its own to have an up-to-date VIF
        // (no need to maintain this in the testfiles)

        // setting up testcase 
        // run primary simulation
        var completeInputData = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput),
            XmlReader.Create(completeBusInput), _inputDataReader);
        var fileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, primaryBusInput);
        var sumWriter = new SummaryDataContainer(null);
        var jobContainer = new JobContainer(sumWriter);

        var _simulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completeInputData, fileWriter, null, null, true);

        Clearfiles(fileWriter); //remove files from previous test runs
        jobContainer.AddRuns(_simulatorFactory);
        jobContainer.Runs.ForEach(x => (x.Run as DummyRunNonExemptedRun).IgnoreSimulationRun = true);
        jobContainer.Execute(false);
        jobContainer.WaitFinished();

        CheckReportExists(fileWriter, CifShouldExist: false, MrfShouldExist: true, VifShouldExist: true);
        //File.Delete(fileWriter.XMLFullReportName);
        var primaryVif = CopyInputFile(fileWriter.XMLPrimaryVehicleReportName);
        // done preparing testcase...

        // this is the actual test: run completed simulation

        var completedInputData = new DummyRunVIFWithInterimBusInputDataProvider(fileWriter.XMLMultistageReport,
            XmlReader.Create(completedBusInput), _inputDataReader, true);
        var completedFileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, completedBusInput);
        var completedSumWriter = new SummaryDataContainer(null);
        var completedJobContainer = new JobContainer(completedSumWriter);

        var completedSimulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completedInputData, completedFileWriter, null, null, true);

        Clearfiles(completedFileWriter); //remove files from previous test runs
        completedJobContainer.AddRuns(completedSimulatorFactory);
		completedJobContainer.Execute(false);
		AssertHelper.Exception<Exception>(() => completedJobContainer.WaitFinished());

    }
}