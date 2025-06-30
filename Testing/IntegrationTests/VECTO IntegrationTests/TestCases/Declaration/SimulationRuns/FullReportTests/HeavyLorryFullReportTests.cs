using TUGraz.Vecto.IntegrationTests.Utils;
using TUGraz.Vecto.IntegrationTests.Utils.DummyRun;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.Vecto.IntegrationTests.TestCases.Declaration.SimulationRuns.FullReportTests;

public class HeavyLorryFullReportTests : FullReportTestsBase
{
	
    #region Heavy Lorry Testfiles
    protected const string ConventionalHeavyLorry = BasePath + "HeavyLorry/Conventional_heavyLorry_AMT.xml";
    protected const string ConventionalHeavyLorry_DifferentTyres = BasePath + "HeavyLorry/Conventional_heavyLorry_AMT_DifferentTyres.xml";
    protected const string ConventionalHeavyLorry_AT_Angledrive = BasePath + "HeavyLorry/Conventional_heavyLorry_AT_Angledrive.xml";
    protected const string ConventionalHeavyLorry_NoRetarder = BasePath + "HeavyLorry/Conventional_heavyLorry_AMT_NoRetarder.xml";
    protected const string ConventionalHeavyLorry_NoAirdrag = BasePath + "HeavyLorry/Conventional_heavyLorry_AMT_NoAirdrag.xml";
	protected const string ConventionalHeavyLorry_DualFuel = BasePath + "HeavyLorry/Conventional_heavyLorry_AMT_DF.xml";
	protected const string ConventionalHeavyLorry_WHR = BasePath + "HeavyLorry/Conventional_heavyLorry_AMT_WHR.xml";

    protected const string ConventionalHeavyLorry_Vocational = BasePath + "HeavyLorry/Conventional_heavyLorry_AMT_Vocational.xml";

    protected const string HEV_Px_HeavyLorry = BasePath + "HeavyLorry/HEV_heavyLorry_AMT_Px.xml";
    protected const string HEV_Px_HeavyLorry_BatteryStd = BasePath + "HeavyLorry/HEV_heavyLorry_Px_ADC_BatteryStd.xml";
    protected const string HEV_S2_HeavyLorry = BasePath + "HeavyLorry/HEV-S_heavyLorry_AMT_S2.xml";
    protected const string HEV_S2_HeavyLorry_NoRetarder = BasePath + "HeavyLorry/HEV-S_heavyLorry_AMT_S2_NoRetarder.xml";
    protected const string HEV_S3_HeavyLorry = BasePath + "HeavyLorry/HEV-S_heavyLorry_S3.xml";
    protected const string HEV_S3_HeavyLorry_ovc = BasePath + "HeavyLorry/HEV-S_heavyLorry_S3_ovc.xml";
    protected const string HEV_S4_HeavyLorry = BasePath + "HeavyLorry/HEV-S_heavyLorry_S4.xml";
    protected const string HEV_IEPC_S_HeavyLorry = BasePath + "HeavyLorry/HEV-S_heavyLorry_IEPC-S.xml";
    protected const string PEV_E2_HeavyLorry = BasePath + "HeavyLorry/PEV_heavyLorry_AMT_E2.xml";
    protected const string PEV_E2_HeavyLorry_NoRetarder = BasePath + "HeavyLorry/PEV_heavyLorry_AMT_E2_NoRetarder.xml";
    protected const string PEV_E2_HeavyLorry_NoAirdrag = BasePath + "HeavyLorry/PEV_heavyLorry_AMT_E2_NoAirdrag.xml";
    protected const string PEV_E2_HeavyLorry_Vocational = BasePath + "HeavyLorry/PEV_heavyLorry_AMT_E2_Vocational.xml";
    protected const string PEV_E2_HeavyLorry_BatteryStd = BasePath + "HeavyLorry/PEV_heavyLorry_AMT_E2_BatteryStd.xml";
    protected const string PEV_E3_HeavyLorry = BasePath + "HeavyLorry/PEV_heavyLorry_E3.xml";
    protected const string PEV_E4_HeavyLorry = BasePath + "HeavyLorry/PEV_heavyLorry_E4.xml";
    protected const string PEV_IEPC_HeavyLorry = BasePath + "HeavyLorry/IEPC_heavyLorry.xml";

    protected const string PEV_IEPC_HeavyLorry_Gbx1 = BasePath + "HeavyLorry/PEV_heavyLorry_IEPC_Gbx1.xml";
    protected const string PEV_IEPC_HeavyLorry_Gbx1Axl = BasePath + "HeavyLorry/PEV_heavyLorry_IEPC_Gbx1Axl.xml";
    protected const string PEV_IEPC_HeavyLorry_Gbx1Whl = BasePath + "HeavyLorry/PEV_heavyLorry_IEPC_Gbx1Axl.xml";

    protected const string PEV_IEPC_HeavyLorry_Gbx2 = BasePath + "HeavyLorry/PEV_heavyLorry_IEPC_Gbx3.xml";
    protected const string PEV_IEPC_HeavyLorry_Gbx2_drag = BasePath + "HeavyLorry/PEV_heavyLorry_IEPC_Gbx3_drag.xml";
    protected const string PEV_IEPC_HeavyLorry_Gbx2Axl = BasePath + "HeavyLorry/PEV_heavyLorry_IEPC_Gbx3Axl.xml";
    protected const string PEV_IEPC_HeavyLorry_Gbx2Axl_drag = BasePath + "HeavyLorry/PEV_heavyLorry_IEPC_Gbx3Axl_drag.xml";
    protected const string PEV_IEPC_HeavyLorry_Gbx2Whl = BasePath + "HeavyLorry/PEV_heavyLorry_IEPC_Gbx3Axl.xml";

    protected const string HEV_IHPC_HeavyLorry = BasePath + "HeavyLorry/HEV_heavyLorry_IHPC.xml";
    protected const string HEV_Px_HeavyLorry_NoRetarder = BasePath + "HeavyLorry/HEV_heavyLorry_AMT_Px_NoRetarder.xml";
    protected const string HEV_Px_HeavyLorry_NoAirDrag = BasePath + "HeavyLorry/HEV_heavyLorry_AMT_Px_NoAirdrag.xml";
    protected const string HEV_Px_HeavyLorry_ADC = BasePath + "HeavyLorry/HEV_heavyLorry_Px_ADC.xml";
    protected const string HEV_S3_HeavyLorry_ADC = BasePath + "HeavyLorry/HEV-S_heavyLorry_S3_ADC_GenSetADC.xml";
    protected const string HEV_Px_HeavyLorry_SuperCap = BasePath + "HeavyLorry/HEV_heavyLorry_Px_SuperCap.xml";

	protected const string ExemptedHeavyLorry = BasePath + "HeavyLorry/exempted_heavyLorry.xml";
    #endregion

    [OneTimeSetUp]
    public void OneTimeSetup()
	{
		// update all necessary bindings so that no simulation is performed
		SetupNinject();

        //WRITE_REPORTS_TO_FILESYSTEM = true;
        WRITE_REPORTS_TO_OUTPUT = true;
    }

	[TestCase(ConventionalHeavyLorry, TestName = "ConventionalHeavyLorry")]
	[TestCase(ConventionalHeavyLorry_DualFuel, TestName = "ConventionalHeavyLorry_DualFuel")]
	[TestCase(ConventionalHeavyLorry_WHR, TestName = "ConventionalHeavyLorry_WHR")]
    [TestCase(ConventionalHeavyLorry_NoRetarder, TestName = "ConventionalHeavyLorry_NoRetarder")]
    [TestCase(ConventionalHeavyLorry_NoAirdrag, TestName = "ConventionalHeavyLorry_NoAirdrag")]
    [TestCase(ConventionalHeavyLorry_DifferentTyres, TestName = "ConventionalHeavyLorry_DifferentTyres")]
    [TestCase(ConventionalHeavyLorry_AT_Angledrive, TestName = "ConventionalHeavyLorry_AT_Angledrive")]
    [TestCase(ConventionalHeavyLorry_Vocational, TestName = "ConventionalHeavyLorry_Vocational")]
    //[TestCase(ConventionalHeavyLorry, false, TestName = "ConventionalHeavyLorryNoMockup")]
    [TestCase(HEV_S2_HeavyLorry, TestName = "HEV_S2_HeavyLorry")]
    [TestCase(HEV_S2_HeavyLorry_NoRetarder, TestName = "HEV_S2_HeavyLorry_NoRetarder")]
    [TestCase(HEV_S3_HeavyLorry, TestName = "HEV_S3_HeavyLorry")]
    [TestCase(HEV_S3_HeavyLorry_ovc, TestName = "HEV_S3_HeavyLorry_ovc")]
    [TestCase(HEV_S4_HeavyLorry, TestName = "HEV_S4_HeavyLorry")]
    [TestCase(HEV_Px_HeavyLorry, TestName = "HEV_Px_HeavyLorry")]
    [TestCase(HEV_Px_HeavyLorry_BatteryStd, TestName = "HEV_Px_HeavyLorry_BatteryStd")]
    [TestCase(PEV_E2_HeavyLorry, TestName = "PEV_E2_HeavyLorry")]
    [TestCase(PEV_E2_HeavyLorry_BatteryStd, TestName = "PEV_E2_HeavyLorry_BatteryStd")]
    [TestCase(PEV_E2_HeavyLorry_NoRetarder, TestName = "PEV_E2_HeavyLorry_NoRetarder")]
    [TestCase(PEV_E2_HeavyLorry_NoAirdrag, TestName = "PEV_E2_HeavyLorry_NoAirdrag")]
    [TestCase(PEV_E2_HeavyLorry_Vocational, TestName = "PEV_E2_HeavyLorry_Vocational")]
    //[TestCase(PEV_E2_HeavyLorry, false, TestName = "PEV_E2_HeavyLorryNoMockup")]
    [TestCase(PEV_E3_HeavyLorry, TestName = "PEV_E3_HeavyLorry")]
    [TestCase(PEV_E4_HeavyLorry, TestName = "PEV_E4_HeavyLorry")]
    [TestCase(PEV_IEPC_HeavyLorry, TestName = "PEV_IEPC_HeavyLorry")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx1, TestName = "PEV_IEPC_HeavyLorry_Gbx1")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx1Axl, TestName = "PEV_IEPC_HeavyLorry_Gbx1Axl")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx1Whl, TestName = "PEV_IEPC_HeavyLorry_Gbx1Whl")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2, TestName = "PEV_IEPC_HeavyLorry_Gbx2")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2_drag, TestName = "PEV_IEPC_HeavyLorry_Gbx2_drag")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2Axl, TestName = "PEV_IEPC_HeavyLorry_Gbx2Axl")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2Axl_drag, TestName = "PEV_IEPC_HeavyLorry_Gbx2Axl_drag")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2Whl, TestName = "PEV_IEPC_HeavyLorry_Gbx2Whl")]

    [TestCase(HEV_IEPC_S_HeavyLorry, TestName = "HEV_IEPC_S_HeavyLorry")]
    [TestCase(HEV_IHPC_HeavyLorry, TestName = "HEV_IHPC_HeavyLorry")]
    [TestCase(HEV_Px_HeavyLorry_ADC, TestName = "HEV_Px_HeavyLorry_ADC")]
    [TestCase(HEV_Px_HeavyLorry_NoRetarder, TestName = "HEV_Px_HeavyLorry_NoRetarder")]
    [TestCase(HEV_Px_HeavyLorry_NoAirDrag, TestName = "HEV_Px_HeavyLorry_NoAirDrag")]
    [TestCase(HEV_S3_HeavyLorry_ADC, TestName = "HEV_S3_HeavyLorry_ADC")]
    [TestCase(HEV_Px_HeavyLorry_SuperCap, TestName = "HEV_Px_HeavyLorry_SuperCap")]

    [TestCase(ExemptedHeavyLorry, TestName = "ExemptedHeavyLorry")]
    public void HeavyLorryFullReportSuccessTest(string fileName)
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
        CheckReportExists(reportWriter);
        Assert.IsTrue(ValidateAndPrint(reportWriter.XMLManufacturerReport, XmlDocumentType.ManufacturerReport), "MRF invalid");
        Assert.IsTrue(ValidateAndPrint(reportWriter.XMLCustomerReport, XmlDocumentType.CustomerReport), "CIF invalid");

		if (!inputProvider.JobInputData.Vehicle.ExemptedVehicle) {
			Assert.IsTrue(CheckElementExists(XMLNames.Report_Results_Summary, reportWriter.XMLCustomerReport));
			//CheckElementCount(XMLNames.Report_Results_Summary, reportWriter.XMLCustomerReport, 2);
		}

	}

	[TestCase(ConventionalHeavyLorry, TestName = "ConventionalHeavyLorry_Error")]
	[TestCase(HEV_Px_HeavyLorry, TestName = "HEV_Px_HeavyLorry Error")]
	[TestCase(HEV_S2_HeavyLorry, TestName = "HEV_S2_HeavyLorry Error")]
    [TestCase(PEV_E2_HeavyLorry, TestName = "PEV_E2_HeavyLorry Error")]
	[TestCase(HEV_S3_HeavyLorry_ovc, TestName = "HEV_S3_HeavyLorry_ovc")]
	public void HeavyLorryFullReportErrorTest(string fileName)
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
		CheckReportExists(reportWriter);
		Assert.IsTrue(ValidateAndPrint(reportWriter.XMLManufacturerReport, XmlDocumentType.ManufacturerReport), "MRF invalid");
		Assert.IsTrue(ValidateAndPrint(reportWriter.XMLCustomerReport, XmlDocumentType.CustomerReport), "CIF invalid");

	    Assert.IsTrue(CheckElementExists(XMLNames.Report_Results_Error, reportWriter.XMLCustomerReport));
		AssertElementValue(reportWriter.XMLManufacturerReport, XMLNames.Report_Results_Status_Error_Val,
			XMLNames.Report_Results, XMLNames.Report_Result_Status);


	}


}