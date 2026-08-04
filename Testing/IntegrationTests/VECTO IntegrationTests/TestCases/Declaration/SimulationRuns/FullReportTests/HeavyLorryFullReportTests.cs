using TUGraz.Vecto.IntegrationTests.Utils;
using TUGraz.Vecto.IntegrationTests.Utils.DummyRun;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
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

	[TestCase(ConventionalHeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(ConventionalHeavyLorry)")]
	[TestCase(ConventionalHeavyLorry_DualFuel, TestName = "HeavyLorryFullReportSuccessTest_v24(ConventionalHeavyLorry_DualFuel)")]
	[TestCase(ConventionalHeavyLorry_WHR, TestName = "HeavyLorryFullReportSuccessTest_v24(ConventionalHeavyLorry_WHR)")]
    [TestCase(ConventionalHeavyLorry_NoRetarder, TestName = "HeavyLorryFullReportSuccessTest_v24(ConventionalHeavyLorry_NoRetarder)")]
    [TestCase(ConventionalHeavyLorry_NoAirdrag, TestName = "HeavyLorryFullReportSuccessTest_v24(ConventionalHeavyLorry_NoAirdrag)")]
    [TestCase(ConventionalHeavyLorry_DifferentTyres, TestName = "HeavyLorryFullReportSuccessTest_v24(ConventionalHeavyLorry_DifferentTyres)")]
    [TestCase(ConventionalHeavyLorry_AT_Angledrive, TestName = "HeavyLorryFullReportSuccessTest_v24(ConventionalHeavyLorry_AT_Angledrive)")]
    [TestCase(ConventionalHeavyLorry_Vocational, TestName = "HeavyLorryFullReportSuccessTest_v24(ConventionalHeavyLorry_Vocational)")]
    //[TestCase(ConventionalHeavyLorry, false, TestName = "HeavyLorryFullReportSuccessTest_v24(ConventionalHeavyLorryNoMockup)")]
    [TestCase(HEV_S2_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_S2_HeavyLorry)")]
    [TestCase(HEV_S2_HeavyLorry_NoRetarder, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_S2_HeavyLorry_NoRetarder)")]
    [TestCase(HEV_S3_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_S3_HeavyLorry)")]
    [TestCase(HEV_S3_HeavyLorry_ovc, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_S3_HeavyLorry_ovc)")]
    [TestCase(HEV_S4_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_S4_HeavyLorry)")]
    [TestCase(HEV_Px_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_Px_HeavyLorry)")]
    [TestCase(HEV_Px_HeavyLorry_BatteryStd, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_Px_HeavyLorry_BatteryStd)")]
    [TestCase(PEV_E2_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_E2_HeavyLorry)")]
    [TestCase(PEV_E2_HeavyLorry_BatteryStd, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_E2_HeavyLorry_BatteryStd)")]
    [TestCase(PEV_E2_HeavyLorry_NoRetarder, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_E2_HeavyLorry_NoRetarder)")]
    [TestCase(PEV_E2_HeavyLorry_NoAirdrag, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_E2_HeavyLorry_NoAirdrag)")]
    [TestCase(PEV_E2_HeavyLorry_Vocational, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_E2_HeavyLorry_Vocational)")]
    //[TestCase(PEV_E2_HeavyLorry, false, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_E2_HeavyLorryNoMockup)")]
    [TestCase(PEV_E3_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_E3_HeavyLorry)")]
    [TestCase(PEV_E4_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_E4_HeavyLorry)")]
    [TestCase(PEV_IEPC_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_IEPC_HeavyLorry)")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx1, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_IEPC_HeavyLorry_Gbx1)")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx1Axl, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_IEPC_HeavyLorry_Gbx1Axl)")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx1Whl, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_IEPC_HeavyLorry_Gbx1Whl)")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_IEPC_HeavyLorry_Gbx2)")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2_drag, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_IEPC_HeavyLorry_Gbx2_drag)")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2Axl, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_IEPC_HeavyLorry_Gbx2Axl)")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2Axl_drag, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_IEPC_HeavyLorry_Gbx2Axl_drag)")]
    [TestCase(PEV_IEPC_HeavyLorry_Gbx2Whl, TestName = "HeavyLorryFullReportSuccessTest_v24(PEV_IEPC_HeavyLorry_Gbx2Whl)")]

    [TestCase(HEV_IEPC_S_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_IEPC_S_HeavyLorry)")]
    [TestCase(HEV_IHPC_HeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_IHPC_HeavyLorry)")]
    [TestCase(HEV_Px_HeavyLorry_ADC, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_Px_HeavyLorry_ADC)")]
    [TestCase(HEV_Px_HeavyLorry_NoRetarder, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_Px_HeavyLorry_NoRetarder)")]
    [TestCase(HEV_Px_HeavyLorry_NoAirDrag, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_Px_HeavyLorry_NoAirDrag)")]
    [TestCase(HEV_S3_HeavyLorry_ADC, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_S3_HeavyLorry_ADC)")]
    [TestCase(HEV_Px_HeavyLorry_SuperCap, TestName = "HeavyLorryFullReportSuccessTest_v24(HEV_Px_HeavyLorry_SuperCap)")]

    [TestCase(ExemptedHeavyLorry, TestName = "HeavyLorryFullReportSuccessTest_v24(ExemptedHeavyLorry)")]


    [TestCase(v27LorryPath + "Conventional_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Conventional_HeavyLorry)")]
    [TestCase(v27LorryPath + "Conventional_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Conventional_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "FCHV_F2_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(FCHV_F2_HeavyLorry)")]
    [TestCase(v27LorryPath + "FCHV_F2_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(FCHV_F2_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "FCHV_F3_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(FCHV_F3_HeavyLorry)")]
    [TestCase(v27LorryPath + "FCHV_F3_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(FCHV_F3_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "FCHV_F4_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(FCHV_F4_HeavyLorry)")]
    [TestCase(v27LorryPath + "FCHV_F4_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(FCHV_F4_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "FCHV_IEPC_2xFC_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(FCHV_IEPC_2xFC_HeavyLorry)")]
    [TestCase(v27LorryPath + "FCHV_IEPC_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(FCHV_IEPC_HeavyLorry)")]
    [TestCase(v27LorryPath + "FCHV_IEPC_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(FCHV_IEPC_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "H2_ICE_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(H2_ICE_HeavyLorry)")]
    [TestCase(v27LorryPath + "HEV_IHPC_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(HEV_IHPC_HeavyLorry)")]
    [TestCase(v27LorryPath + "HEV_P2_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(HEV_P2_HeavyLorry)")]
    [TestCase(v27LorryPath + "HEV_P2_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(HEV_P2_HeavyLorry_requiredOnly)")]
	[TestCase(v27LorryPath + "HEV_P2_supercap_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(HEV_P2_supercap_HeavyLorry)")]
    [TestCase(v27LorryPath + "PEV_E2_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_E2_HeavyLorry)")]
    [TestCase(v27LorryPath + "PEV_E2_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_E2_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "PEV_E3_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_E3_HeavyLorry)")]
    [TestCase(v27LorryPath + "PEV_E3_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_E3_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "PEV_E4_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_E4_HeavyLorry)")]
    [TestCase(v27LorryPath + "PEV_E4_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_E4_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "PEV_IEPC_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_IEPC_HeavyLorry)")]
    [TestCase(v27LorryPath + "PEV_IEPC_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_IEPC_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "PEV_IEPC_multiCurve_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_IEPC_multiCurve_HeavyLorry)")]
    [TestCase(v27LorryPath + "PEV_IEPC_stdValues_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(PEV_IEPC_stdValues_HeavyLorry)")]
    [TestCase(v27LorryPath + "SHEV_IEPC_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(SHEV_IEPC_HeavyLorry)")]
    [TestCase(v27LorryPath + "SHEV_IEPC_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(SHEV_IEPC_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "SHEV_S2_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(SHEV_S2_HeavyLorry)")]
    [TestCase(v27LorryPath + "SHEV_S2_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(SHEV_S2_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "SHEV_S3_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(SHEV_S3_HeavyLorry)")]
    [TestCase(v27LorryPath + "SHEV_S3_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(SHEV_S3_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "SHEV_S4_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(SHEV_S4_HeavyLorry)")]
    [TestCase(v27LorryPath + "SHEV_S4_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(SHEV_S4_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "Multiple_FCHV_F2_IEPC_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_FCHV_F2_IEPC_HeavyLorry)")]
    [TestCase(v27LorryPath + "Multiple_FCHV_F2_IEPC_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_FCHV_F2_IEPC_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "Multiple_FCHV_F3_F4_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_FCHV_F3_F4_HeavyLorry)")]
    [TestCase(v27LorryPath + "Multiple_FCHV_F3_F4_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_FCHV_F3_F4_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "Multiple_PEV_E2_IEPC_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_PEV_E2_IEPC_HeavyLorry)")]
    [TestCase(v27LorryPath + "Multiple_PEV_E2_IEPC_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_PEV_E2_IEPC_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "Multiple_PEV_E3_E4_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_PEV_E3_E4_HeavyLorry)")]
    [TestCase(v27LorryPath + "Multiple_PEV_E3_E4_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_PEV_E3_E4_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "Multiple_SHEV_S2_IEPC_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_SHEV_S2_IEPC_HeavyLorry)")]
    [TestCase(v27LorryPath + "Multiple_SHEV_S2_IEPC_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_SHEV_S2_IEPC_HeavyLorry_requiredOnly)")]
    [TestCase(v27LorryPath + "Multiple_SHEV_S3_S4_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_SHEV_S3_S4_HeavyLorry)")]
    [TestCase(v27LorryPath + "Multiple_SHEV_S3_S4_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Multiple_SHEV_S3_S4_HeavyLorry_requiredOnly)")]

	[TestCase(v27LorryPath + "Exempted_HeavyLorry.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Exempted_HeavyLorry)")]
	[TestCase(v27LorryPath + "Exempted_HeavyLorry_requiredOnly.xml", TestName = "HeavyLorryFullReportSuccessTest_v27(Exempted_HeavyLorry_requiredOnly)")]

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

		if (!inputProvider.JobInputData.Vehicle.ExemptedVehicle && !inputProvider.JobInputData.JobType.IsOneOf(VectoSimulationJobType.FCHV, VectoSimulationJobType.FCHV_IEPC, VectoSimulationJobType.Multiple_FCHV)) {
			Assert.IsTrue(CheckElementExists(XMLNames.Report_Results_Summary, reportWriter.XMLCustomerReport));
			//CheckElementCount(XMLNames.Report_Results_Summary, reportWriter.XMLCustomerReport, 2);
		}

	}

	[TestCase(ConventionalHeavyLorry, TestName = "HeavyLorryFullReportErrorTest_v24(ConventionalHeavyLorry_Error)")]
	[TestCase(HEV_Px_HeavyLorry, TestName = "HeavyLorryFullReportErrorTest_v24(HEV_Px_HeavyLorry Error)")]
	[TestCase(HEV_S2_HeavyLorry, TestName = "HeavyLorryFullReportErrorTest_v24(HEV_S2_HeavyLorry Error)")]
    [TestCase(PEV_E2_HeavyLorry, TestName = "HeavyLorryFullReportErrorTest_v24(PEV_E2_HeavyLorry Error)")]
	[TestCase(HEV_S3_HeavyLorry_ovc, TestName = "HeavyLorryFullReportErrorTest_v24(HEV_S3_HeavyLorry_ovc)")]
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