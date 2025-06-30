using System.Xml;
using TUGraz.Vecto.IntegrationTests.Utils;
using TUGraz.Vecto.IntegrationTests.Utils.DummyRun;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.Vecto.IntegrationTests.TestCases.Declaration.SimulationRuns.FullReportTests;

public class CompleteBusFullReportTests : FullReportTestsBase
{
    #region PrimaryBus

    protected const string Conventional_PrimaryBus = BasePath + "PrimaryBus/Conventional_primaryBus_AMT.xml";
    protected const string Conventional_PrimaryBus_AT_Angledrive = BasePath + "PrimaryBus/Conventional_primaryBus_AT_Angledrive.xml";
    protected const string Conventional_PrimaryBus_NoRetarder = BasePath + "PrimaryBus/Conventional_primaryBus_AT_NoRetarder.xml";
    protected const string Conventional_PrimaryBus_RetarderMeasured = BasePath + "PrimaryBus/Conventional_primaryBus_AMT_RetarderMeasured.xml";
    protected const string Conventional_PrimaryBus_Tyres = BasePath + "PrimaryBus/Conventional_primaryBus_AMT_DifferentTyres.xml";
    protected const string HEV_Px_PrimaryBus = BasePath + "PrimaryBus/HEV_primaryBus_AMT_Px.xml";
	protected const string HEV_Px_PrimaryBus_OVC = BasePath + "PrimaryBus/HEV_primaryBus_AMT_Px_OVC.xml";
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

    #region Complete(d) Bus Input

    protected const string Conventional_CompletedBusInput = BasePath + "CompletedBus/Conventional_completedBus_2.xml";
	protected const string Conventional_CompletedBusInputNoAirdrag = BasePath + "CompletedBus/Conventional_completedBus_NoAirdrag.xml";
	protected const string Conventional_CompletedBusInput_TypeApproval = BasePath + "CompletedBus/Conventional_completedBus_2_TypeApprovalNumber.xml";
	protected const string Conventional_CompletedBusInput_AirdragV10 = BasePath + "CompletedBus/Conventional_completedBus_AirdragV10.xml";
	protected const string Conventional_CompletedBusInput_AirdragV20 = BasePath + "CompletedBus/Conventional_completedBus_AirdragV20.xml";
	protected const string HEV_CompletedBusInput = BasePath + "CompletedBus/HEV_completedBus_2.xml";
	protected const string PEV_CompletedBusInput = BasePath + "CompletedBus/PEV_completedBus_2.xml";
	protected const string PEV_IEPC_CompletedBusInput = BasePath + "CompletedBus/IEPC_completedBus_2.xml";

	protected const string Exempted_CempletedBus = BasePath + "CompletedBus/exempted_completedBus_2.xml";
    #endregion


    [OneTimeSetUp]
	public void OneTimeSetup()
	{
		// update all necessary bindings so that no simulation is performed
		SetupNinject();

        //WRITE_REPORTS_TO_FILESYSTEM = true;
        WRITE_REPORTS_TO_OUTPUT = true;
    }


    [TestCase(Conventional_PrimaryBus_Tyres, Conventional_CompletedBusInput, "Conventional", TestName = "Complete Conventional Bus Different Tyres")]
    [TestCase(Conventional_PrimaryBus_Tyres, Conventional_CompletedBusInputNoAirdrag, "Conventional", TestName = "Complete Conventional Bus NoAirdrag")]
    [TestCase(Conventional_PrimaryBus_NoRetarder, Conventional_CompletedBusInput, "Conventional", TestName = "Complete_ConventionalPrimaryBus_NoRetarder")]
    [TestCase(Conventional_PrimaryBus_DF, Conventional_CompletedBusInput, "Conventional", TestName = "Complete ConventionalPrimaryBus_DualFuel")]
    [TestCase(Conventional_PrimaryBus_AT_Angledrive, Conventional_CompletedBusInput, "Conventional", TestName = "Complete ConventionalPrimaryBus_AT_Angledrive")]
    [TestCase(Conventional_PrimaryBus, Conventional_CompletedBusInput_TypeApproval, "Conventional", TestName = "Complete Conventional Bus Type Approval")]
    [TestCase(HEV_IEPC_S_PrimaryBus, HEV_CompletedBusInput, "IEPC-S", "HEV", TestName = "Complete HEV_IEPC_S_PrimaryBus")]
    [TestCase(HEV_IEPC_S_PrimaryBus_BatteryStd, HEV_CompletedBusInput, "IEPC-S", "HEV", TestName = "Complete HEV_IEPC_S_PrimaryBus_BatteryStd")]
    [TestCase(HEV_Px_PrimaryBus, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_Px_PrimaryBus")]
	[TestCase(HEV_Px_PrimaryBus_OVC, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_Px_PrimaryBus_OVC")]
    [TestCase(HEV_Px_PrimaryBus_BatteryStd, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_Px_PrimaryBus_BatteryStd")]
    [TestCase(HEV_IHPC_PrimaryBus, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_IHPC_PrimaryBus")]
    [TestCase(HEV_IHPC_PrimaryBus_NoRetarder, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_IHPC_PrimaryBus_NoRetarder")]
    [TestCase(HEV_S2_PrimaryBus, HEV_CompletedBusInput, "S2", "HEV", TestName = "Complete HEV_S2_PrimaryBus")]
    [TestCase(HEV_S3_PrimaryBus, HEV_CompletedBusInput, "S3", "HEV", TestName = "Complete HEV_S3_PrimaryBus")]
    [TestCase(HEV_S4_PrimaryBus, HEV_CompletedBusInput, "S4", "HEV", TestName = "Complete HEV_S4_PrimaryBus")]
    [TestCase(PEV_E2_PrimaryBus, PEV_CompletedBusInput, "E2", "PEV", TestName = "Complete PEV_E2_PrimaryBus")]
    [TestCase(PEV_E3_PrimaryBus, PEV_CompletedBusInput, "E3", "PEV", TestName = "Complete PEV_E3_PrimaryBus")]
    [TestCase(PEV_E4_PrimaryBus, PEV_CompletedBusInput, "E4", "PEV", TestName = "Complete PEV_E4_PrimaryBus")]
    [TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx1")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1Axl, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx1Axl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1Whl, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx1Whl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2_drag, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2_drag")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2Axl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl_drag, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2Axl_drag")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Whl, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus_Gbx2Whl")]
    [TestCase(PEV_IEPC_std_PrimaryBus, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC-std_PrimaryBus")]
    [TestCase(PEV_E2_PrimaryBus_StdEM, PEV_CompletedBusInput, "E2", "PEV", TestName = "Complete PEV_E2_PrimaryBus_EM-Std")]

    [TestCase(Exempted_PrimaryBus, Exempted_CempletedBus, "Exempted", TestName = "Complete ExemptedBus")]
    public void CompleteBusFullReportSuccessTest(string primaryBusInput, string completeBusInput, params string[] expectedType)
    {
        var copied = CopyInputFile(primaryBusInput, completeBusInput);
        // complete: primary input + complete input (full) => MRF Primary, VIF (step 1), MRF Complete, CIF Complete
        // (approach: first simulate primary on its own to have an up-to-date VIF
        // (no need to maintain this in the testfiles)


		var completeJob = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput),
			XmlReader.Create(completeBusInput), _inputDataReader, true);
        var completeFileWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, completeBusInput);
        var completeSumWriter = new SummaryDataContainer(null);
        var completeJobContainer = new JobContainer(completeSumWriter);

        var completedSimulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completeJob, completeFileWriter, null, null, true);

        Clearfiles(completeFileWriter); //remove files from previous test runs
        completeJobContainer.AddRuns(completedSimulatorFactory);
        completeJobContainer.Execute(false);
        completeJobContainer.WaitFinished();

        // assertions

        CheckReportExists(completeFileWriter, PrimaryMrfShouldExist: true, VifShouldExist: true, CifShouldExist: true, MrfShouldExist: true);

		ValidateAndPrint(completeFileWriter.XMLManufacturerReport, XmlDocumentType.ManufacturerReport);
		ValidateAndPrint(completeFileWriter.XMLCustomerReport, XmlDocumentType.CustomerReport);

        CheckElementTypeNameContains(completeFileWriter.XMLMultistageReport, "Vehicle", expectedType);

	}

	[TestCase(Conventional_PrimaryBus, Conventional_CompletedBusInput_TypeApproval, "Conventional", TestName = "Complete Conventional Bus Type Approval Ignore")]
	[TestCase(HEV_Px_PrimaryBus_BatteryStd, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_Px_PrimaryBus_BatteryStd Ignore")]
	[TestCase(HEV_S3_PrimaryBus, HEV_CompletedBusInput, "S3", "HEV", TestName = "Complete HEV_S3_PrimaryBus Ignore")]
	[TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus Ignore")]
	public void CompleteBusFullReportIgnoreTest(string primaryBusInput, string completeBusInput, params string[] expectedType)
	{
		var copied = CopyInputFile(primaryBusInput, completeBusInput);
		// complete: primary input + complete input (full) => MRF Primary, VIF (step 1), MRF Complete, CIF Complete
		// (approach: first simulate primary on its own to have an up-to-date VIF
		// (no need to maintain this in the testfiles)


		var completeJob = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput),
			XmlReader.Create(completeBusInput), _inputDataReader, true);
		var completeReportWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, completeBusInput);
		var completeSumWriter = new SummaryDataContainer(null);
		var completeJobContainer = new JobContainer(completeSumWriter);

		var completedSimulatorFactory =
			_simFactoryFactory.Factory(ExecutionMode.Declaration, completeJob, completeReportWriter, null, null, true);

		Clearfiles(completeReportWriter); //remove files from previous test runs
		completeJobContainer.AddRuns(completedSimulatorFactory);
		(completeJobContainer.Runs[0].Run as DummyRunNonExemptedRun).IgnoreSimulationRun = true;
        completeJobContainer.Execute(false);
		completeJobContainer.WaitFinished();

		// assertions

		CheckReportExists(completeReportWriter, PrimaryMrfShouldExist: true, VifShouldExist: true, CifShouldExist: true, MrfShouldExist: true);

		if (WRITE_REPORTS_TO_FILESYSTEM) {
			completeReportWriter.WriteAllReports();
		}

        CheckElementTypeNameContains(completeReportWriter.XMLMultistageReport, "Vehicle", expectedType);

		AssertElementValue(completeReportWriter.XMLManufacturerReport, XMLNames.Report_Results_Status_Success_Val,
			XMLNames.Report_Results, XMLNames.Report_Result_Status);

		if (GetElements(completeReportWriter.XMLMultistageReport, XMLNames.Report_Results,
				XMLNames.Report_Results_FuelConsumption).Any()) {
			Assert.IsTrue(
				GetElements(completeReportWriter.XMLManufacturerReport, XMLNames.Report_Results,
					XMLNames.Report_Results_FuelConsumption).Any(x => x.Value == double.NaN.ToString()));
		}

		if (GetElements(completeReportWriter.XMLMultistageReport, XMLNames.Report_Results, XMLNames.Report_Results_CO2).Any()) {
			Assert.IsTrue(
				GetElements(completeReportWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_Results_CO2)
					.Any(x => x.Value == double.NaN.ToString()));
		}

		if (GetElements(completeReportWriter.XMLMultistageReport, XMLNames.Report_Results, XMLNames.Report_ResultEntry_ElectricEnergyConsumption).Any()) {
			Assert.IsTrue(
				GetElements(completeReportWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_ResultEntry_VIF_ElectricEnergyConsumption)
					.Any(x => x.Value == double.NaN.ToString()));
		}
    }

    [TestCase(Conventional_PrimaryBus, Conventional_CompletedBusInput_TypeApproval, "Conventional", TestName = "Complete Conventional Bus Type Approval IgnoreAndError")]
    [TestCase(HEV_Px_PrimaryBus_BatteryStd, HEV_CompletedBusInput, "Px", "HEV", TestName = "Complete HEV_Px_PrimaryBus_BatteryStd IgnoreAndError")]
    [TestCase(HEV_S3_PrimaryBus, HEV_CompletedBusInput, "S3", "HEV", TestName = "Complete HEV_S3_PrimaryBus IgnoreAndError")]
    [TestCase(PEV_IEPC_PrimaryBus, PEV_IEPC_CompletedBusInput, "IEPC", TestName = "Complete PEV_IEPC_PrimaryBus IgnoreAndError")]
    public void CompleteBusFullReportIgnoreErrorTest(string primaryBusInput, string completeBusInput, params string[] expectedType)
    {
        var copied = CopyInputFile(primaryBusInput, completeBusInput);
        // complete: primary input + complete input (full) => MRF Primary, VIF (step 1), MRF Complete, CIF Complete
        // (approach: first simulate primary on its own to have an up-to-date VIF
        // (no need to maintain this in the testfiles)


        var completeJob = new DummyRunPrimaryWithCompletedBusInputDataProvider(XmlReader.Create(primaryBusInput),
            XmlReader.Create(completeBusInput), _inputDataReader, true);
        var completeReportWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, completeBusInput);
        var completeSumWriter = new SummaryDataContainer(null);
        var completeJobContainer = new JobContainer(completeSumWriter);

        var completedSimulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, completeJob, completeReportWriter, null, null, true);

        Clearfiles(completeReportWriter); //remove files from previous test runs
		completeJobContainer.AddRuns(completedSimulatorFactory);
		completeJobContainer.Runs.ForEach(x => (x.Run as DummyRunNonExemptedRun).IgnoreSimulationRun = true);

		completeJobContainer.Execute(false);
		AssertHelper.Exception<Exception>(() => completeJobContainer.WaitFinished());


    }
}