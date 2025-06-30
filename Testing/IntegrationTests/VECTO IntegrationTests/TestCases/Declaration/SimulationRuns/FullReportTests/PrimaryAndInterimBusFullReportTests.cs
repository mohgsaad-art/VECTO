using System.Xml;
using TUGraz.Vecto.IntegrationTests.Utils;
using TUGraz.Vecto.IntegrationTests.Utils.DummyRun;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.IntegrationTests.TestCases.Declaration.SimulationRuns.FullReportTests;

public class PrimaryAndInterimBusFullReportTests : FullReportTestsBase
{
    #region PrimaryBus

    protected const string Conventional_PrimaryBus = BasePath + "PrimaryBus/Conventional_primaryBus_AMT.xml";
    protected const string Conventional_PrimaryBus_AT_Angledrive = BasePath + "PrimaryBus/Conventional_primaryBus_AT_Angledrive.xml";
    protected const string Conventional_PrimaryBus_NoRetarder = BasePath + "PrimaryBus/Conventional_primaryBus_AT_NoRetarder.xml";
    protected const string Conventional_PrimaryBus_RetarderMeasured = BasePath + "PrimaryBus/Conventional_primaryBus_AMT_RetarderMeasured.xml";
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

    [OneTimeSetUp]
	public void OneTimeSetup()
	{
		// update all necessary bindings so that no simulation is performed
		SetupNinject();

		//WRITE_REPORTS_TO_FILESYSTEM = true;
		//      WRITE_REPORTS_TO_OUTPUT = true;
	}

    [TestCase(Conventional_PrimaryBus_Tyres, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim Conventional Bus Different Tyres")]
    [TestCase(Conventional_PrimaryBus_NoRetarder, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim_ConventionalPrimaryBus_NoRetarder")]
    [TestCase(Conventional_PrimaryBus_DF, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim ConventionalPrimaryBus_DualFuel")]
    [TestCase(Conventional_PrimaryBus_AT_Angledrive, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim ConventionalPrimaryBus_AT_Angledrive")]
    [TestCase(HEV_IEPC_S_PrimaryBus, HEV_InterimBusInput, "IEPC-S", "HEV", TestName = "PrimaryAndInterim HEV_IEPC_S_PrimaryBus")]
    [TestCase(HEV_IEPC_S_PrimaryBus_BatteryStd, HEV_InterimBusInput, "IEPC-S", "HEV", TestName = "PrimaryAndInterim HEV_IEPC_S_PrimaryBus_BatteryStd")]
    [TestCase(HEV_Px_PrimaryBus, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_Px_PrimaryBus")]
    [TestCase(HEV_Px_PrimaryBus_BatteryStd, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_Px_PrimaryBus_BatteryStd")]
    [TestCase(HEV_IHPC_PrimaryBus, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_IHPC_PrimaryBus")]
    [TestCase(HEV_IHPC_PrimaryBus_NoRetarder, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_IHPC_PrimaryBus_NoRetarder")]
    [TestCase(HEV_S2_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "PrimaryAndInterim HEV_S2_PrimaryBus")]
    [TestCase(HEV_S3_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "PrimaryAndInterim HEV_S3_PrimaryBus")]
    [TestCase(HEV_S4_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "PrimaryAndInterim HEV_S4_PrimaryBus")]
    [TestCase(PEV_E2_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E2_PrimaryBus")]
    [TestCase(PEV_E3_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E3_PrimaryBus")]
    [TestCase(PEV_E4_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E4_PrimaryBus")]
    [TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterim PEV_IEPC_PrimaryBus")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx1")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1Axl, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx1Axl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1Whl, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx1Whl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2_drag, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2_drag")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2Axl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl_drag, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2Axl_drag")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Whl, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterimPEV_IEPC_PrimaryBus_Gbx2Whl")]
    [TestCase(PEV_IEPC_std_PrimaryBus, PEV_IEPC_InterimBusInput, "IEPC", TestName = "PrimaryAndInterim PEV_IEPC-std_PrimaryBus")]
    [TestCase(PEV_E2_PrimaryBus_StdEM, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E2_PrimaryBus_EM-Std")]

    [TestCase(Exempted_PrimaryBus, Exempted_InterimBus, "Exempted", TestName = "PrimaryAndInterim Exempted")]
    public void PrimaryWithInterimFullReportSuccessTest(string primaryBusInput, string interimInput, params string[] expectedType)
    {
        var copied = CopyInputFile(primaryBusInput, interimInput);
        // complete: primary input + complete input (full) => MRF Primary, VIF (step 1), MRF Complete, CIF Complete
        // (approach: first simulate primary on its own to have an up-to-date VIF
        // (no need to maintain this in the testfiles)


        var job = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput), XmlReader.Create(interimInput), _inputDataReader);
        var fileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, interimInput);
        var sumWriter = new SummaryDataContainer(null);
        var jobContainer = new JobContainer(sumWriter);

        var completedSimulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, job, fileWriter, null, null, true);

        Clearfiles(fileWriter); //remove files from previous test runs
        jobContainer.AddRuns(completedSimulatorFactory);
        jobContainer.Execute(false);
        jobContainer.WaitFinished();

        // assertions

        CheckReportExists(fileWriter, VifShouldExist: true, CifShouldExist: false, MrfShouldExist: true);

        CheckElementTypeNameContains(fileWriter.XMLMultistageReport, "Vehicle", expectedType);
    }

	[TestCase(Conventional_PrimaryBus_Tyres, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim Conventional Bus Different Tyres Error")]
	[TestCase(HEV_Px_PrimaryBus, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_Px_PrimaryBus Error")]
	[TestCase(HEV_S2_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "PrimaryAndInterim HEV_S2_PrimaryBus Error")]
	[TestCase(PEV_E3_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E3_PrimaryBus Error")]
	public void PrimaryWithInterimFullReportErrorTest(string primaryBusInput, string interimInput, params string[] expectedType)
	{
		var copied = CopyInputFile(primaryBusInput, interimInput);
		// complete: primary input + complete input (full) => MRF Primary, VIF (step 1), MRF Complete, CIF Complete
		// (approach: first simulate primary on its own to have an up-to-date VIF
		// (no need to maintain this in the testfiles)


		var job = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput), XmlReader.Create(interimInput), _inputDataReader);
		var reportWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, interimInput);
		var sumWriter = new SummaryDataContainer(null);
		var jobContainer = new JobContainer(sumWriter);

		var completedSimulatorFactory =
			_simFactoryFactory.Factory(ExecutionMode.Declaration, job, reportWriter, null, null, true);

		Clearfiles(reportWriter); //remove files from previous test runs
		jobContainer.AddRuns(completedSimulatorFactory);
		(jobContainer.Runs[0].Run as DummyRunNonExemptedRun).FinishedWithError = true;
        jobContainer.Execute(false);
		AssertHelper.Exception<Exception>(() => jobContainer.WaitFinished());

        // assertions

        CheckReportExists(reportWriter, VifShouldExist: false, CifShouldExist: false, MrfShouldExist: true);

		//CheckElementTypeNameContains(reportWriter.XMLMultistageReport, "Vehicle", expectedType);

		Assert.IsTrue(CheckElementExists(XMLNames.Report_Results_Error, reportWriter.XMLManufacturerReport));
		AssertElementValue(reportWriter.XMLManufacturerReport, XMLNames.Report_Results_Status_Error_Val,
			XMLNames.Report_Results, XMLNames.Report_Result_Status);

    }

    [TestCase(Conventional_PrimaryBus_Tyres, Conventional_InterimBusInput, "Conventional", TestName = "PrimaryAndInterim Conventional Bus Different Tyres Ignore")]
    [TestCase(HEV_Px_PrimaryBus, HEV_InterimBusInput, "Px", "HEV", TestName = "PrimaryAndInterim HEV_Px_PrimaryBus Ignore")]
    [TestCase(HEV_S2_PrimaryBus, HEV_InterimBusInput, "Sx", "HEV", TestName = "PrimaryAndInterim HEV_S2_PrimaryBus Ignore")]
    [TestCase(PEV_E3_PrimaryBus, PEV_InterimBusInput, "Ex", "PEV", TestName = "PrimaryAndInterim PEV_E3_PrimaryBus Ignore")]
    public void PrimaryWithInterimFullReportIgnoreTest(string primaryBusInput, string interimInput, params string[] expectedType)
    {
        var copied = CopyInputFile(primaryBusInput, interimInput);
        // complete: primary input + complete input (full) => MRF Primary, VIF (step 1), MRF Complete, CIF Complete
        // (approach: first simulate primary on its own to have an up-to-date VIF
        // (no need to maintain this in the testfiles)


        var job = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput), XmlReader.Create(interimInput), _inputDataReader);
        var reportWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, interimInput);
        var sumWriter = new SummaryDataContainer(null);
        var jobContainer = new JobContainer(sumWriter);

        var completedSimulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, job, reportWriter, null, null, true);

        Clearfiles(reportWriter); //remove files from previous test runs
        jobContainer.AddRuns(completedSimulatorFactory);
        (jobContainer.Runs[0].Run as DummyRunNonExemptedRun).IgnoreSimulationRun = true;
        jobContainer.Execute(false);
        jobContainer.WaitFinished();

        // assertions

        CheckReportExists(reportWriter, VifShouldExist: true, CifShouldExist: false, MrfShouldExist: true);

        //CheckElementTypeNameContains(reportWriter.XMLMultistageReport, "Vehicle", expectedType);

        AssertElementValue(reportWriter.XMLManufacturerReport, XMLNames.Report_Results_Status_Success_Val,
            XMLNames.Report_Results, XMLNames.Report_Result_Status);

		if (GetElements(reportWriter.XMLMultistageReport, XMLNames.Report_Results,
				XMLNames.Report_Results_FuelConsumption).Any()) {
			Assert.IsTrue(
				GetElements(reportWriter.XMLManufacturerReport, XMLNames.Report_Results,
					XMLNames.Report_Results_FuelConsumption).Any(x => x.Value == double.NaN.ToString()));
		}

		if (GetElements(reportWriter.XMLMultistageReport, XMLNames.Report_Results, XMLNames.Report_Results_CO2).Any()) {
			Assert.IsTrue(
				GetElements(reportWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_Results_CO2)
					.Any(x => x.Value == double.NaN.ToString()));
		}

		if (GetElements(reportWriter.XMLMultistageReport, XMLNames.Report_Results, XMLNames.Report_ResultEntry_ElectricEnergyConsumption).Any()) {
			Assert.IsTrue(
				GetElements(reportWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_ResultEntry_VIF_ElectricEnergyConsumption)
					.Any(x => x.Value == double.NaN.ToString()));
		}
    }
}