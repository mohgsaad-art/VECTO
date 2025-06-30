using TUGraz.Vecto.IntegrationTests.Utils;
using TUGraz.Vecto.IntegrationTests.Utils.DummyRun;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.Vecto.IntegrationTests.TestCases.Declaration.SimulationRuns.FullReportTests;

public class PrimaryBusFullReportTests : FullReportTestsBase
{
	
   
    #region PrimaryBus

    protected const string Conventional_PrimaryBus = BasePath + "PrimaryBus/Conventional_primaryBus_AMT.xml";
	protected const string Conventional_PrimaryBus_TyreNS = BasePath + "PrimaryBus/Conventional_primaryBus_AMT_TyreNS-1.xml";
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

    [OneTimeSetUp]
    public void OneTimeSetup()
	{
		// update all necessary bindings so that no simulation is performed
		SetupNinject();

        //WRITE_REPORTS_TO_FILESYSTEM = true;
        WRITE_REPORTS_TO_OUTPUT = true;
    }


    [TestCase(Conventional_PrimaryBus, TestName = "FullReportTest_ConventionalPrimaryBus")]
	[TestCase(Conventional_PrimaryBus_TyreNS, TestName = "FullReportTest_ConventionalPrimaryBus_TyreNamespace")]
    [TestCase(Conventional_PrimaryBus_NoRetarder, TestName = "FullReportTest_ConventionalPrimaryBus_NoRetarder")]
    [TestCase(Conventional_PrimaryBus_RetarderMeasured, TestName = "FullReportTest_ConventionalPrimaryBus_RetarderMeasured")]
    [TestCase(Conventional_PrimaryBus_AT_Angledrive, TestName = "FullReportTest_ConventionalPrimaryBus_AT_Angledrive")]
    [TestCase(Conventional_PrimaryBus_Tyres, TestName = "FullReportTest_ConventionalPrimaryBus Tyres")]
    [TestCase(HEV_IEPC_S_PrimaryBus, TestName = "FullReportTest_HEV_IEPC_S_PrimaryBus")]
    [TestCase(HEV_IEPC_S_PrimaryBus_BatteryStd, TestName = "FullReportTest_HEV_IEPC_S_PrimaryBus_BatteryStd")]
    [TestCase(HEV_Px_PrimaryBus, TestName = "FullReportTest_HEV_Px_PrimaryBus")]
	[TestCase(HEV_Px_PrimaryBus_OVC, TestName = "FullReportTest_HEV_Px_PrimaryBus OVC")]
    [TestCase(HEV_Px_PrimaryBus_BatteryStd, TestName = "FullReportTest_HEV_Px_PrimaryBus_BatteryStd")]
    [TestCase(HEV_IHPC_PrimaryBus, TestName = "FullReportTest_HEV_IHPC_PrimaryBus")]
    [TestCase(HEV_IHPC_PrimaryBus_NoRetarder, TestName = "FullReportTest_HEV_IHPC_PrimaryBus_NoRetarder")]
    [TestCase(HEV_Px_PrimaryBus_SuperCap, TestName = "FullReportTest_HEV_Px_PrimaryBus_SuperCap")]
    [TestCase(HEV_S2_PrimaryBus, TestName = "FullReportTest_HEV_S2_PrimaryBus")]
    [TestCase(HEV_S2_PrimaryBus_GenSetADC, TestName = "FullReportTest_HEV_S2_PrimaryBus_GenSetADC")]
    [TestCase(HEV_S2_PrimaryBus_ADC, TestName = "FullReportTest_HEV_S2_PrimaryBus_ADC")]
    [TestCase(HEV_S3_PrimaryBus, TestName = "FullReportTest_HEV_S3_PrimaryBus")]
    [TestCase(HEV_S4_PrimaryBus, TestName = "FullReportTest_HEV_S4_PrimaryBus")]
    [TestCase(PEV_E2_PrimaryBus, TestName = "FullReportTest_PEV_E2_PrimaryBus")]
    [TestCase(PEV_E3_PrimaryBus, TestName = "FullReportTest_PEV_E3_PrimaryBus")]
    [TestCase(PEV_E4_PrimaryBus, TestName = "FullReportTest_PEV_E4_PrimaryBus")]
    [TestCase(PEV_IEPC_PrimaryBus, TestName = "FullReportTest_PEV_IEPC_PrimaryBus")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1, TestName = "FullReportTest_PEV_IEPC_PrimaryBus_Gbx1")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1Axl, TestName = "FullReportTest_PEV_IEPC_PrimaryBus_Gbx1Axl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx1Whl, TestName = "FullReportTest_PEV_IEPC_PrimaryBus_Gbx1Whl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2, TestName = "FullReportTest_PEV_IEPC_PrimaryBus_Gbx2")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2_drag, TestName = "FullReportTest_PEV_IEPC_PrimaryBus_Gbx2_drag")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl, TestName = "FullReportTest_PEV_IEPC_PrimaryBus_Gbx2Axl")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Axl_drag, TestName = "FullReportTest_PEV_IEPC_PrimaryBus_Gbx2Axl_drag")]
    [TestCase(PEV_IEPC_PrimaryBus_Gbx2Whl, TestName = "FullReportTest_PEV_IEPC_PrimaryBus_Gbx2Whl")]
    [TestCase(PEV_IEPC_std_PrimaryBus, TestName = "FullReportTest_PEV_IEPC-std_PrimaryBus")]
    [TestCase(PEV_E2_PrimaryBus_StdEM, TestName = "FullReportTest_PEV_E2_PrimaryBus_EM-Std")]
    [TestCase(PEV_E2_PrimaryBus_StdBat, TestName = "FullReportTest_PEV_E2_PrimaryBus_BatteryStd")]
    [TestCase(Conventional_PrimaryBus_DF, TestName = "FullReportTest_ConventionalPrimaryBus_DualFuel")]

    [TestCase(Exempted_PrimaryBus, TestName = "FullReportTest_ExemptedPrimaryBus")]
    public void PrimaryBusFullReportSuccessTest(string fileName)
    {
        CopyInputFile(fileName);
        var inputProvider = _inputDataReader.CreateDeclaration(fileName);
        var reportWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, fileName);
        var sumWriter = new SummaryDataContainer(null);
        var jobContainer = new JobContainer(sumWriter);

        var _simulatorFactory =
            _simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, reportWriter, null, null, true);
        Clearfiles(reportWriter);
        jobContainer.AddRuns(_simulatorFactory);
        jobContainer.Execute(false);
        jobContainer.WaitFinished();

		if (WRITE_REPORTS_TO_FILESYSTEM) {
			reportWriter.WriteAllReports();
		}
        CheckReportExists(reportWriter, CifShouldExist: false, VifShouldExist: true);
        Assert.IsTrue(ValidateAndPrint(reportWriter.XMLMultistageReport, XmlDocumentType.MultistepOutputData), "VIF invalid");
        Assert.IsTrue(ValidateAndPrint(reportWriter.XMLManufacturerReport, XmlDocumentType.ManufacturerReport), "MRF invalid");

		AssertElementValue(reportWriter.XMLManufacturerReport, XMLNames.Report_Results_Status_Success_Val,
			XMLNames.Report_Results, XMLNames.Report_Result_Status);


		//Assert.IsTrue(CheckElementExists(XMLNames.Report_Results_Summary, reportWriter.XMLCustomerReport));
		//CheckElementCount(XMLNames.Report_Results_Summary, reportWriter.XMLCustomerReport, 2);

	}

    [TestCase(Conventional_PrimaryBus, TestName = "FullReportTest_ConventionalPrimaryBus Error")]
	[TestCase(HEV_Px_PrimaryBus, TestName = "FullReportTest_HEV_Px_PrimaryBus Error")]
	[TestCase(HEV_S2_PrimaryBus_GenSetADC, TestName = "FullReportTest_HEV_S2_PrimaryBus_GenSetADC Error")]
	[TestCase(PEV_E3_PrimaryBus, TestName = "FullReportTest_PEV_E3_PrimaryBus Error")]
	[TestCase(HEV_Px_PrimaryBus_OVC, TestName = "FullReportTest_HEV_Px_PrimaryBus OVC")]
	public void PrimaryBusFullReportErrorTest(string fileName)
	{
		CopyInputFile(fileName);
		var inputProvider = _inputDataReader.CreateDeclaration(fileName);
		var reportWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, fileName);
		var sumWriter = new SummaryDataContainer(null);
		var jobContainer = new JobContainer(sumWriter);

		var _simulatorFactory =
			_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, reportWriter, null, null, true);
		Clearfiles(reportWriter);
		jobContainer.AddRuns(_simulatorFactory);
		(jobContainer.Runs[0].Run as DummyRunNonExemptedRun).FinishedWithError = true;
        jobContainer.Execute(false);
		AssertHelper.Exception<Exception>(() => jobContainer.WaitFinished());

        if (WRITE_REPORTS_TO_FILESYSTEM) {
			reportWriter.WriteAllReports();
		}
		CheckReportExists(reportWriter, CifShouldExist: false, VifShouldExist: true);
		Assert.IsTrue(ValidateAndPrint(reportWriter.XMLMultistageReport, XmlDocumentType.MultistepOutputData), "VIF invalid");
		Assert.IsTrue(ValidateAndPrint(reportWriter.XMLManufacturerReport, XmlDocumentType.ManufacturerReport), "MRF invalid");

		Assert.IsTrue(CheckElementExists(XMLNames.Report_Results_Error, reportWriter.XMLManufacturerReport));
		AssertElementValue(reportWriter.XMLManufacturerReport, XMLNames.Report_Results_Status_Error_Val,
			XMLNames.Report_Results, XMLNames.Report_Result_Status);
	}

	

	[TestCase(Conventional_PrimaryBus, TestName = "FullReportTest_ConventionalPrimaryBus Ignore")]
	[TestCase(HEV_Px_PrimaryBus, TestName = "FullReportTest_HEV_Px_PrimaryBus Ignore")]
	[TestCase(HEV_S2_PrimaryBus_GenSetADC, TestName = "FullReportTest_HEV_S2_PrimaryBus_GenSetADC Ignore")]
	[TestCase(PEV_E3_PrimaryBus, TestName = "FullReportTest_PEV_E3_PrimaryBus Ignore")]
	public void PrimaryBusFullReportIgnoreTest(string fileName)
	{
		CopyInputFile(fileName);
		var inputProvider = _inputDataReader.CreateDeclaration(fileName);
		var reportWriter = GetReportWriter(TestContext.CurrentContext.Test.Name, fileName);
		var sumWriter = new SummaryDataContainer(null);
		var jobContainer = new JobContainer(sumWriter);

		var _simulatorFactory =
			_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, reportWriter, null, null, true);
		Clearfiles(reportWriter);
		jobContainer.AddRuns(_simulatorFactory);
		(jobContainer.Runs[0].Run as DummyRunNonExemptedRun).IgnoreSimulationRun = true;
		jobContainer.Execute(false);
		jobContainer.WaitFinished();

		if (WRITE_REPORTS_TO_FILESYSTEM) {
			reportWriter.WriteAllReports();
		}
		CheckReportExists(reportWriter, CifShouldExist: false, VifShouldExist: true);
		Assert.IsTrue(ValidateAndPrint(reportWriter.XMLMultistageReport, XmlDocumentType.MultistepOutputData), "VIF invalid");
		Assert.IsTrue(ValidateAndPrint(reportWriter.XMLManufacturerReport, XmlDocumentType.ManufacturerReport), "MRF invalid");

		AssertElementValue(reportWriter.XMLManufacturerReport, XMLNames.Report_Results_Status_Success_Val,
			XMLNames.Report_Results, XMLNames.Report_Result_Status);

		if (GetElements(reportWriter.XMLManufacturerReport, XMLNames.Report_Results,
				XMLNames.Report_Results_FuelConsumption).Any()) {
			Assert.IsTrue(
				GetElements(reportWriter.XMLManufacturerReport, XMLNames.Report_Results,
					XMLNames.Report_Results_FuelConsumption).Any(x => x.Value == double.NaN.ToString()));
		}

		if (GetElements(reportWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_Results_CO2).Any()) {
			Assert.IsTrue(
				GetElements(reportWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_Results_CO2)
					.Any(x => x.Value == double.NaN.ToString()));
		}

		if (GetElements(reportWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_ResultEntry_ElectricEnergyConsumption).Any()) {
			Assert.IsTrue(
				GetElements(reportWriter.XMLManufacturerReport, XMLNames.Report_Results, XMLNames.Report_Result_EnergyConsumption)
					.Any(x => x.Value == double.NaN.ToString()));
		}
    }
}