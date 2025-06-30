using TUGraz.Vecto.IntegrationTests.Utils;
using TUGraz.Vecto.IntegrationTests.Utils.DummyRun;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.Vecto.IntegrationTests.TestCases.Declaration.SimulationRuns.FullReportTests;

public class MediumLorryFullReportTests : FullReportTestsBase
{
	
    #region Medium Lorry Testfiles

    protected const string Conventional_mediumLorry_AMT = BasePath + "MediumLorry/Conventional_mediumLorry_AMT.xml";
    protected const string HEV_S_mediumLorry_AMT_S2 = BasePath + "MediumLorry/HEV-S_mediumLorry_AMT_S2.xml";
    protected const string HEV_S_mediumLorry_IEPC_S = BasePath + "MediumLorry/HEV-S_mediumLorry_IEPC-S.xml";
    protected const string HEV_S_mediumLorry_S3 = BasePath + "MediumLorry/HEV-S_mediumLorry_S3.xml";
    protected const string HEV_S_mediumLorry_S3_BatteryStd = BasePath + "MediumLorry/HEV-S_mediumLorry_S3_BatteryStd.xml";
    protected const string HEV_S_mediumLorry_S4 = BasePath + "MediumLorry/HEV-S_mediumLorry_S4.xml";
    protected const string HEV_mediumLorry_AMT_Px = BasePath + "MediumLorry/HEV_mediumLorry_AMT_Px.xml";
    protected const string IEPC_mediumLorry = BasePath + "MediumLorry/IEPC_mediumLorry.xml";
    protected const string PEV_mediumLorry_AMT_E2 = BasePath + "MediumLorry/PEV_mediumLorry_AMT_E2.xml";
    protected const string PEV_mediumLorry_AMT_E2_EM_Std = BasePath + "MediumLorry/PEV_mediumLorry_AMT_E2_EM_Std.xml";
    protected const string PEV_mediumLorry_APT_N_E2 = BasePath + "MediumLorry/PEV_mediumLorry_APT-N_E2.xml";
    protected const string PEV_mediumLorry_E3 = BasePath + "MediumLorry/PEV_mediumLorry_E3.xml";
    protected const string PEV_mediumLorry_E4 = BasePath + "MediumLorry/PEV_mediumLorry_E4.xml";
    protected const string PEV_mediumLorry_E3_BatteryStd = BasePath + "MediumLorry/PEV_mediumLorry_E3_BatteryStd.xml";

    protected const string HEV_mediumLorry_IHPC = BasePath + "MediumLorry/HEV_mediumLorry_IHPC.xml";
    protected const string HEV_mediumLorry_Px_SuperCap = BasePath + "MediumLorry/HEV_mediumLorry_Px_SuperCap.xml";
    protected const string PEV_mediumLorry_AMT_E2_BatStd = BasePath + "MediumLorry/PEV_mediumLorry_AMT_E2_BatteryStd.xml";
    //protected const string HEV_mediumLorry_Px_ADC = BasePathMockup + "MediumLorry/HEV_heavyLorry_IHPC.xml";
    //protected const string HEV_mediumLorry_S3_ADC_GenSetADC = BasePathMockup + "MediumLorry/HEV_heavyLorry_IHPC.xml";

	protected const string ExemptedMediumLorry = BasePath + "MediumLorry/exempted_mediumLorry.xml";
    #endregion

    [OneTimeSetUp]
    public void OneTimeSetup()
	{
        // update all necessary bindings so that no simulation is performed
		SetupNinject();

		//WRITE_REPORTS_TO_FILESYSTEM = true;
  //      WRITE_REPORTS_TO_OUTPUT = true;
	}

	[TestCase(Conventional_mediumLorry_AMT, TestName = "Conventional_Medium_Lorry")]
	[TestCase(HEV_S_mediumLorry_AMT_S2, TestName = "HEV_S2_Medium_Lorry")]
	[TestCase(HEV_S_mediumLorry_IEPC_S, TestName = "HEV_IEPC_Medium_Lorry")]
	[TestCase(HEV_S_mediumLorry_S3, TestName = "HEV_S3_Medium_Lorry")]
	[TestCase(HEV_S_mediumLorry_S3_BatteryStd, TestName = "HEV_S3_Medium_Lorry_BatteryStd")]
	[TestCase(HEV_S_mediumLorry_S4, TestName = "HEV_S4_Medium_Lorry")]
	[TestCase(HEV_mediumLorry_AMT_Px, TestName = "HEV_Px_Medium_Lorry")]
	[TestCase(IEPC_mediumLorry, TestName = "PEV_IEPC_Medium_Lorry")]
	[TestCase(PEV_mediumLorry_AMT_E2, TestName = "PEV_E2_Medium_Lorry")]
	[TestCase(PEV_mediumLorry_AMT_E2_EM_Std, TestName = "PEV_E2_std_Medium_Lorry")]
	[TestCase(PEV_mediumLorry_APT_N_E2, TestName = "PEV_E2_Medium_Lorry_2")]
	[TestCase(PEV_mediumLorry_E3, TestName = "PEV_E3_Medium_Lorry")]
	[TestCase(PEV_mediumLorry_E3_BatteryStd, TestName = "PEV_E3_Medium_Lorry_BatteryStd")]
	[TestCase(PEV_mediumLorry_E4, TestName = "PEV_E4_Medium_Lorry")]
	[TestCase(HEV_mediumLorry_IHPC, TestName = "HEV_IHPC_MediumLorry")]
	[TestCase(HEV_mediumLorry_Px_SuperCap, TestName = "HEV_Px_Medium_Lorry_SuperCap")]
	[TestCase(PEV_mediumLorry_AMT_E2_BatStd, TestName = "PEV_E2_Medium_Lorry_BatteryStd")]

    [TestCase(ExemptedMediumLorry, TestName = "Exempted_MediumLorry")]
    public void MediumLorryFullReportSuccessTest(string fileName)
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

	[TestCase(Conventional_mediumLorry_AMT, TestName = "Conventional_Medium_Lorry Error")]
	[TestCase(HEV_S_mediumLorry_S3, TestName = "HEV_S3_Medium_Lorry Error")]
	[TestCase(HEV_mediumLorry_AMT_Px, TestName = "HEV_Px_Medium_Lorry Error")]
	[TestCase(PEV_mediumLorry_AMT_E2, TestName = "PEV_E2_Medium_Lorry Error")]
    public void MediumLorryFullReportErrorTest(string fileName)
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