#define FULL_SIMULATIONS



using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Tests.Utils;
using Formatting = Newtonsoft.Json.Formatting;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.PrimaryBus;




public class PrimaryBusSimulation
{

	private const string BASE_DIR = @"TestData/Integration/DeclarationMode/2nd_AmendmDeclMode/";

	private const string BASE_DIR_COMPLETED = @"TestData/Integration/DeclarationMode/2nd_AmendmDeclMode/CompletedBus";

	private const string BASE_DIR_FACTOR_METHOD_MODEL_DATA = @"TestData/Integration/DeclarationMode/2nd_AmendmDeclMode/CompletedBus/FactorMethod";
    private const string BASE_DIR_VIF = @"TestData/Integration/DeclarationMode/2nd_AmendmDeclMode/CompletedBus/VIF";
	private ThreadLocal<StandardKernel> _kernel;

	private StandardKernel Kernel => _kernel.Value;
	
	private IXMLInputDataReader _xmlReader;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new ThreadLocal<StandardKernel>(() => new StandardKernel(new VectoNinjectModule()));
		_xmlReader = Kernel.Get<IXMLInputDataReader>();

		
		
	}

	[SetUp]
	public void Setup()
	{
#if !FULL_SIMULATIONS
		Kernel.Rebind<IDeclarationCycleFactory>().To<TestDeclarationCycleFactoryVariant>().InSingletonScope();
		var cycleFactory = Kernel.Get<IDeclarationCycleFactory>() as TestDeclarationCycleFactoryVariant;
		cycleFactory.Variant = "Short_10";
#endif
    }

	private TestMissionFilter TestMissionFilter()
	{
#if !FULL_SIMULATIONS
		Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
#endif
		var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
#if FULL_SIMULATIONS
		Assert.Null(missionFilter);
#else
		Assert.NotNull(missionFilter);
#endif 
		return missionFilter;
	}

	private TestDeclarationCycleFactoryStartPoint StartPointCycleFactory()
	{
#if !FULL_SIMULATIONS
		Kernel.Rebind<IDeclarationCycleFactory>().To<TestDeclarationCycleFactoryStartPoint>().InSingletonScope();
#endif
        var missionFilter = Kernel.Get<IDeclarationCycleFactory>() as TestDeclarationCycleFactoryStartPoint;
#if FULL_SIMULATIONS
		Assert.Null(missionFilter);
		TestContext.Progress.WriteLine("Running full cycle");
#else
		Assert.NotNull(missionFilter);
		
#endif
		return missionFilter;
    }


    [
	TestCase(@"PrimaryBus/Conventional/primary_heavyBus group41_nonSmart.xml", 0, TestName = "2nd Amendment PrimaryBus Conventional"),
	TestCase(@"PrimaryBus/PEV/PEV_primaryBus_AMT_E2.xml", 0, TestName = "2nd Amendment PrimaryBus PEV E2"),

	TestCase(@"PrimaryBus/PEV/PrimaryCoach_E2_Base_AMT.xml", 0, TestName = "2nd Amendment PrimaryBus Coach PEV E2 Base"),
	TestCase(@"PrimaryBus/PEV/PrimaryCityBus_IEPC_Base.xml", 0, TestName = "2nd Amendment PrimaryBus CityBus PEV IEPC Base"),

	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_BD_BCVC_stefan.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 stpr"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_NoAlt.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 no_alt stpr"),
	//TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_Notappl.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 notAppl stpr"), // invalid input for PS
	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_CM_BCVC.xml", 7, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 AMT_CM_BCVC stpr, InterUrban_Ref_Load"),

	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_Conv.xml", 0, TestName="2nd Amendment PrimaryBus Coach P-HEV P2 AMT"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_Base_AMT.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 Base AMT"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_ADC.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 AMT ADC"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_OVC.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 AMT OVC"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_Base_AT.xml", 0, TestName = "2nd Amendment PrimaryBus CityBus P-HEV P1 Base AT"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_Supercap.xml", 0, TestName = "2nd Amendment PrimaryBus CityBus P-HEV P1 Supercap"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_Base_AT.xml", 7, TestName = "2nd Amendment PrimaryBus CityBus P-HEV P1 Base AT - Interurban"),
    //TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_AT_BD_BCVC.xml", 0, TestName= "2nd Amendment PrimaryBus CityBus P-HEV P1 Base AT - no TC"), // Px has to have a TC (in case of APT)
	TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_IHPC.xml", -1, TestName = "2nd Amendment PrimaryBus CityBus P-HEV IHPC Base"),
    TestCase(@"PrimaryBus/S-HEV/PrimaryCoach_S2_Base_AMT.xml", 0, TestName = "2nd Amendment PrimaryBus Coach S-HEV S2 Base"),
	TestCase(@"PrimaryBus/S-HEV/PrimaryCityBus_IEPC-S_Base.xml", 0, TestName = "2nd Amendment PrimaryBus CityBus S-HEV IEPC Base"),

    TestCase(@"PrimaryBus/Exempted/exempted_primary_heavyBus.xml", 0, TestName = "2nd Amendment PrimaryBus Exempted"),

	TestCase(@"FactorMethod/Conventional/P31_32_nonSmartES_elecFan_elecSteer.xml", 0, TestName = "2nd Amendment PrimaryBus FM Conventional nonSmartES_elFan_elSteer"),
	TestCase(@"FactorMethod/Conventional/P31_32_nonSmartES_mechFan_mechSteer.xml", 0, TestName = "2nd Amendment PrimaryBus FM Conventional nonSmartES_mechFan_mechSteer"),
	TestCase(@"FactorMethod/Conventional/P31_32_SmartES_elecFan_elecSteer.xml", 0, TestName = "2nd Amendment PrimaryBus FM Conventional SmartES_elFan_elSteer"),
	TestCase(@"FactorMethod/Conventional/P31_32_SmartES_mechFan_mechSteer.xml", 0, TestName = "2nd Amendment PrimaryBus FM Conventional SmartES_mechFan_mechSteer"),


	TestCase(@"FactorMethod/IEPC/P31_32_IEPC_EDP.xml", 0, TestName = "2nd Amendment PrimaryBus FM IEPC EDP"),
	TestCase(@"FactorMethod/IEPC/P31_32_IEPC_FESG.xml", 0, TestName = "2nd Amendment PrimaryBus FM IEPC FESG"),

	TestCase(@"FactorMethod/IHPC/P31_32_IHPC_nonSmartES_elec_SP_elec_Fan.xml", 0, TestName = "2nd Amendment PrimaryBus FM IHPC nonSmartES_elFan_elSteer"),
	TestCase(@"FactorMethod/IHPC/P31_32_IHPC_nonSmartES_elec_SP_mech_Fan.xml", 0, TestName = "2nd Amendment PrimaryBus FM IHPC nonSmartES_mechFan_elSteer"),
	TestCase(@"FactorMethod/IHPC/P31_32_IHPC_nonSmartES_mechAux.xml", 0, TestName = "2nd Amendment PrimaryBus FM IHPC nonSmartES_mechAux"),
	TestCase(@"FactorMethod/IHPC/P31_32_IHPC_SmartES_elec_SP_elec_Fan.xml", 0, TestName = "2nd Amendment PrimaryBus FM IHPC SmartES_elFan_ElSteer"),

	TestCase(@"FactorMethod/PEV/P31_32_E2_AMT_EDP.xml", 0, TestName = "2nd Amendment PrimaryBus FM PEV E2 EDP"),
	TestCase(@"FactorMethod/PEV/P31_32_E2_AMT_FESG.xml", 0, TestName = "2nd Amendment PrimaryBus FM PEV E2 FESG"),

	TestCase(@"FactorMethod/P-HEV/P1-HEV/P31_32_P1_HEV_AT_nonSmart_ES_elec_SP_elec_PS.xml", 0, TestName = "2nd Amendment PrimaryBus FM P-HEV P1 nonSmartES_elFan_elPS"),
	TestCase(@"FactorMethod/P-HEV/P1-HEV/P31_32_P1_HEV_AT_nonSmart_ES_mech_Aux.xml", -1, TestName = "2nd Amendment PrimaryBus FM P-HEV P1 nonSmartES_mechAux"),
	TestCase(@"FactorMethod/P-HEV/P1-HEV/P31_32_P1_HEV_AT_Smart_ES_elec_SP_elec_PS.xml", 0, TestName = "2nd Amendment PrimaryBus FM P-HEV P1 SmartES_elPS_elSteer"),
	
	TestCase(@"FactorMethod/P-HEV/P2-HEV/P31_32_P2_HEV_nonSmartES_elec_SP_elec_Fan.xml", 0, TestName = "2nd Amendment PrimaryBus FM P-HEV P2 nonSmartES_elFan_elSteer"),
	TestCase(@"FactorMethod/P-HEV/P2-HEV/P31_32_P2_HEV_nonSmartES_elec_SP_mech_Fan.xml", 0, TestName = "2nd Amendment PrimaryBus FM P-HEV P2 nonSmartES_mechFan_elSP"),
	TestCase(@"FactorMethod/P-HEV/P2-HEV/P31_32_P2_HEV_nonSmartES_mechAux.xml", 0, TestName = "2nd Amendment PrimaryBus FM P-HEV P2 nonSmartES_mechAux"),
	TestCase(@"FactorMethod/P-HEV/P2-HEV/P31_32_P2_HEV_SmartES_elec_SP_elec_Fan.xml", 0, TestName = "2nd Amendment PrimaryBus FM P-HEV P2 SmartES_elFan_elSteer"),

    // Fails on almost every cycle except suburban
	TestCase(@"FactorMethod/S-HEV/S2-HEV/P31_32_S2_HEV_nonSmartES_elecSP_mechFan.xml", 0, TestName = "2nd Amendment PrimaryBus FM S-HEV S2 nonSmartES_elecSP_mechFan 0"),
    TestCase(@"FactorMethod/S-HEV/S2-HEV/P31_32_S2_HEV_nonSmartES_elecSP_mechFan.xml", 4, TestName = "2nd Amendment PrimaryBus FM S-HEV S2 nonSmartES_elecSP_mechFan 4"),
	]

	// Testcase Stefan Present
	[TestCase(@"E:\QUAM\Downloads\HybridEquivFactor_IEPCAxle\P32_IEPC_Base.xml", -1, TestName = "PrimaryBus IEPC StefanP")]
	[TestCase(@"E:\QUAM\Downloads\HybridEquivFactor_IEPCAxle\P1_HEV_P32_NonOVC_ICE-I_Base.xml", -1, TestName = "PrimaryBus EffFactor StefanP")]
	[TestCase(@"E:\QUAM\Downloads\StefanP_OVC-SoC_UF\P2_HEV_P32_OVC_ICE-I_Base.xml", 12, TestName = "PrimaryBus StefanP EC_EL in XML")]
    public void PrimaryBusSimulationTest(string jobFile, int runIdx)
	{
		TestMissionFilter()?.SetMissions((MissionType.Interurban, LoadingType.ReferenceLoad));
		RunSimulationPrimary(jobFile, runIdx);
	}
	
	[
	TestCase(@"CompletedBus/VIF/primary_heavyBus group41_nonSmart.RSLT_VIF.xml", @"CompletedBus/Conventional_completedBus_2.xml",                1,                 TestName = "2nd Amendment CompletedBus Conventional"),
	TestCase(@"CompletedBus/VIF/PEV_primaryBus_AMT_E2.RSLT_VIF.xml",             @"CompletedBus/PEV_completedBus_2.xml",                        -1,                 TestName = "2nd Amendment CompletedBus PEV E2"),
	TestCase(@"CompletedBus/VIF/PrimaryCoach_E2_Base_AMT.RSLT_VIF.xml",          @"CompletedBus/PEV_completedBus_2.xml",                         1,                 TestName = "2nd Amendment CompletedBus Coach PEV E2"),
	TestCase(@"CompletedBus/VIF/PrimaryCityBus_IEPC_Base.RSLT_VIF.xml",          @"CompletedBus/PEV_completedBus_2.xml",                         1,                 TestName = "2nd Amendment CompletedBus CityBus PEV IEPC"),
	TestCase(@"CompletedBus/VIF/PrimaryCoach_P2_HEV_Base_AMT.RSLT_VIF.xml",      @"CompletedBus/HEV_completedBus_2.xml",                         1,                 TestName = "2nd Amendment CompletedBus Coach HEV P2"),
	TestCase(@"CompletedBus/VIF/PrimaryCoach_P2_HEV_AMT_OVC.RSLT_VIF.xml",       @"CompletedBus/HEV_completedBus_2.xml",                         1,                 TestName = "2nd Amendment CompletedBus Coach HEV P2 OVC"),
	TestCase(@"CompletedBus/VIF/PrimaryCityBus_P1_HEV_Base_AT.RSLT_VIF.xml",     @"CompletedBus/HEV_completedBus_2.xml",                         1,true,    TestName = "2nd Amendment CompletedBus CityBus HEV P1 - fails on complete cycle"),
	TestCase(@"CompletedBus/VIF/PrimaryCityBus_P1_HEV_Supercap.RSLT_VIF.xml",    @"CompletedBus/HEV_completedBus_2.xml",                         1,false,   TestName = "2nd Amendment CompletedBus CityBus HEV P1 SuperCap"),
	TestCase(@"CompletedBus/VIF/PrimaryCoach_S2_Base_AMT.RSLT_VIF.xml",          @"CompletedBus/HEV_completedBus_2.xml",                         1,                 TestName = "2nd Amendment CompletedBus Coach HEV S2 OVC"),
	TestCase(@"CompletedBus/VIF/PrimaryCityBus_IEPC-S_Base.RSLT_VIF.xml",        @"CompletedBus/HEV_completedBus_2.xml",                         1,                 TestName = "2nd Amendment CompletedBus CityBus HEV IEPC-S"),
	TestCase(@"CompletedBus/VIF/exempted_primary_heavyBus.RSLT_VIF.xml",         @"CompletedBus/exempted_completedBus_input_full.xml",           1,                 TestName = "2nd Amendment CompletedBus Exempted"),
	TestCase(@"CompletedBus/VIF/PrimaryCityBus_IHPC.RSLT_VIF.xml",               @"CompletedBus/HEV_completedBus_2.xml",                        -1,                 TestName="2nd Amendment Completed Bus IHPC"),

	TestCase(@"FactorMethod/VIF/P31_32_nonSmartES_elecFan_elecSteer.RSLT_VIF.xml", @"FactorMethod/Conventional/Conventional_32e_prim_Dim_HVAC.xml", 0,
		TestName = "2nd Amendment CompletedBus Conventional nonSmartES_elFan_elSteer 32e_prim_Dim_HVAC"),
	TestCase(@"FactorMethod/VIF/P31_32_nonSmartES_mechFan_mechSteer.RSLT_VIF.xml", @"FactorMethod/Conventional/Conventional_32e_spez_Dim_HVAC.xml",  0,
		TestName = "2nd Amendment CompletedBus Conventional nonSmartES_mechFan_mechSteer 32e_spez_Dim_HVAC.xml"),
	TestCase(@"FactorMethod/VIF/P31_32_SmartES_elecFan_elecSteer.RSLT_VIF.xml", @"FactorMethod/Conventional/Conventional_32e_prim_Dim_HVAC.xml",0,
		TestName = "2nd Amendment CompletedBus Conventional SmartES_elFan_elSteer 32e_prim_Dim_HVAC"),
	TestCase(@"FactorMethod/VIF/P31_32_SmartES_mechFan_mechSteer.RSLT_VIF.xml", @"FactorMethod/Conventional/Conventional_32e_spez_Dim_HVAC.xml",  0,
		TestName = "2nd Amendment CompletedBus Conventional SmartES_mechFan_mechSteer 32e_spez_Dim_HVAC"),

    ]

	[TestCase(@"E:\QUAM\Downloads\HybridEquivFactor_IEPCAxle\P32_IEPC_Base.RSLT_VIF.xml", @"E:\QUAM\Downloads\HybridEquivFactor_IEPCAxle\IEPC_32c_Base.xml", -1, true, TestName = "CompletedBus IEPC StefanP")]
	[TestCase(@"E:\QUAM\Downloads\HybridEquivFactor_IEPCAxle\P1_HEV_P32_NonOVC_ICE-I_Base.RSLT_VIF.xml", @"E:\QUAM\Downloads\HybridEquivFactor_IEPCAxle\P1_HEV_32c_NonOVC_ICE-I_Base.xml", -1, true, TestName = "CompletedBus EffFactor StefanP")]

    public void CompletedBusSimulationTest(string vifFile, string completed, int runIdx, bool full_sim = false)
	{
		if (full_sim) {
			Kernel.Rebind<IDeclarationCycleFactory>().To<DeclarationCycleFactory>().InSingletonScope();
        }
		var completedJob = GenerateJsonJobCompletedBus(Path.Combine(BASE_DIR, vifFile), Path.Combine(BASE_DIR, completed));

		var missionFilter = TestMissionFilter();
		missionFilter?.SetMissions((MissionType.Coach, LoadingType.ReferenceLoad));

        var finalVif = CreateCompletedVIF(completedJob);
		
		//RunSimulationPrimary(finalVif, runIdx);
	}



	[TestCase(@"PrimaryCityBus_P1_HEV_Base_AT.RSLT_VIF.xml", @"HEV_completedBus_2.xml", 53804,
		TestName = "2nd Amendment CompletedBus CityBus HEV P1 - failing cycle section")]
	public void CompletedBusCycleSection(string vifFile, string completed, double start,
		double? distance = null)
	{
#if FULL_SIMULATIONS
		Assert.Ignore();
#endif
		var cycleFactory = StartPointCycleFactory();

		var missionFilter = TestMissionFilter();
		missionFilter?.SetMissions((MissionType.Interurban, LoadingType.ReferenceLoad));
		cycleFactory?.SetStartPoint(MissionType.Interurban, start.SI<Meter>(), true, distance?.SI<Meter>());
		var completedJob = GenerateJsonJobCompletedBus(Path.Combine(BASE_DIR_VIF, vifFile),
			Path.Combine(BASE_DIR_COMPLETED, completed));

		var finalVif = CreateCompletedVIF(completedJob);
	}


	[
		TestCase(@"PrimaryBus/Conventional/primary_heavyBus group41_nonSmart.xml", @"Conventional_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Conventional"),
		
		TestCase(@"PrimaryBus/PEV/PEV_primaryBus_AMT_E2.xml", @"PEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus PEV E2"),
		TestCase(@"PrimaryBus/PEV/PrimaryCoach_E2_Base_AMT.xml", @"PEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Coach PEV E2 Base"),
		TestCase(@"PrimaryBus/PEV/PrimaryCityBus_IEPC_Base.xml", @"PEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus CityBus PEV IEPC Base"),

		TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_Base_AMT.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Coach P-HEV P2 Base AMT"),
		TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_OVC.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Coach P-HEV P2 AMT OVC"),
		TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_Base_AT.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus CityBus P-HEV P1 Base AT"),

		TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_IHPC.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus CityBus IHPC"),


        TestCase(@"PrimaryBus/S-HEV/PrimaryCoach_S2_Base_AMT.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Coach S-HEV S2 Base"),
		TestCase(@"PrimaryBus/S-HEV/PrimaryCityBus_IEPC-S_Base.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus CityBus S-HEV IEPC Base"),

		//TestCase(@"PrimaryBus/Exempted/exempted_primary_heavyBus.xml", @"exempted_completedBus_input_full.xml", 0, TestName = "2nd Amendment SingleBus Exempted"), // exempted single run not supported!

	]

	[TestCase(@"E:\QUAM\Downloads\HybridEquivFactor_IEPCAxle\P32_IEPC_Base.xml", @"E:\QUAM\Downloads\HybridEquivFactor_IEPCAxle\IEPC_32c_Base.xml", -1, TestName = "SingleBus IEPC StefanP")]

    public void SingleBusSimulationTest(string jobFile, string completed, int runIdx)
	{
		RunSimulationSingle(jobFile, completed, runIdx);
	}

	private const LoadingType LowL = LoadingType.LowLoading;
	private const LoadingType RefL = LoadingType.ReferenceLoad;
	private const MissionType CycleCO = MissionType.Coach;
	private const MissionType CycleHU = MissionType.HeavyUrban;

	[
	//TestCase(@"PrimaryBus/Conventional/primary_heavyBus group41_nonSmart.xml", @"Conventional_completedBus_2.xml", @"primary_heavyBus group41_nonSmart.RSLT_VIF.xml", CycleCO, LowL, 
	//	TestName = "2nd Amendment FactorMethodRunData Conventional CO RL"),
	//TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_Base_AMT.xml",  @"HEV_completedBus_2.xml", @"PrimaryCoach_P2_HEV_Base_AMT.RSLT_VIF.xml", CycleCO, LowL,
	//	TestName = "2nd Amendment FactorMethodRunData Coach HEV P2 CO LL"),
	//TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_ADC.xml", @"HEV_completedBus_2.xml", @"PrimaryCoach_P2_HEV_AMT_ADC.RSLT_VIF.xml", CycleCO, LowL,
	//	TestName = "2nd Amendment FactorMethodRunData Coach HEV P2 ADC CO LL"),
	//TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_Supercap.xml", @"HEV_completedBus_2.xml", @"PrimaryCityBus_P1_HEV_Supercap.RSLT_VIF.xml", CycleCO, LowL, 
	//	TestName = "2nd Amendment FactorMethodRunData CityBus HEV P1 SuperCap"),


	TestCase(@"FactorMethod/Conventional/P31_32_nonSmartES_elecFan_elecSteer.xml", @"FactorMethod/Conventional/Conventional_32e_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_nonSmartES_elecFan_elecSteer.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData Conventional nonSmartES_elFan_elSteer 32e_prim_Dim_HVAC"),
	TestCase(@"FactorMethod/Conventional/P31_32_nonSmartES_mechFan_mechSteer.xml", @"FactorMethod/Conventional/Conventional_32e_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_nonSmartES_mechFan_mechSteer.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData Conventional nonSmartES_mechFan_mechSteer 32e_spez_Dim_HVAC.xml"),
	TestCase(@"FactorMethod/Conventional/P31_32_SmartES_elecFan_elecSteer.xml", @"FactorMethod/Conventional/Conventional_32e_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_SmartES_elecFan_elecSteer.RSLT_VIF.xml", CycleCO, RefL,
		TestName = "2nd Amendment FactorMethodRunData Conventional SmartES_elFan_elSteer 32e_prim_Dim_HVAC"),
	TestCase(@"FactorMethod/Conventional/P31_32_SmartES_mechFan_mechSteer.xml", @"FactorMethod/Conventional/Conventional_32e_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_SmartES_mechFan_mechSteer.RSLT_VIF.xml", CycleCO, RefL, 
		TestName = "2nd Amendment FactorMethodRunData Conventional SmartES_mechFan_mechSteer 32e_spez_Dim_HVAC"),


	TestCase(@"FactorMethod/IEPC/P31_32_IEPC_EDP.xml", @"FactorMethod/IEPC/IEPC_31a_completed_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_IEPC_EDP.RSLT_VIF.xml", CycleHU, LowL, 
		TestName = "2nd Amendment FactorMethodRunData IEPC EDP prim_Dim_HVAC"),
	TestCase(@"FactorMethod/IEPC/P31_32_IEPC_FESG.xml", @"FactorMethod/IEPC/IEPC_31a_completed_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_IEPC_FESG.RSLT_VIF.xml", CycleHU, RefL, 
		TestName = "2nd Amendment FactorMethodRunData IEPC FESG spez_Dim_HVAC"),

	TestCase(@"FactorMethod/IHPC/P31_32_IHPC_nonSmartES_elec_SP_elec_Fan.xml", @"FactorMethod/IHPC/IHPC_32e_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_IHPC_nonSmartES_elec_SP_elec_Fan.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData IHPC nonSmartES_elFan_elSteer prim_Dim_HVAC"),
	TestCase(@"FactorMethod/IHPC/P31_32_IHPC_nonSmartES_elec_SP_mech_Fan.xml", @"FactorMethod/IHPC/IHPC_32e_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_IHPC_nonSmartES_elec_SP_mech_Fan.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData IHPC nonSmartES_mechFan_elSteer spez_Dim_HVAC"),
	TestCase(@"FactorMethod/IHPC/P31_32_IHPC_nonSmartES_mechAux.xml", @"FactorMethod/IHPC/IHPC_32e_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_IHPC_nonSmartES_mechAux.RSLT_VIF.xml", CycleCO, LowL,
		TestName = "2nd Amendment FactorMethodRunData IHPC nonSmartES_mechAux prim_Dim_HVAC"),
	TestCase(@"FactorMethod/IHPC/P31_32_IHPC_SmartES_elec_SP_elec_Fan.xml", @"FactorMethod/IHPC/IHPC_32e_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_IHPC_SmartES_elec_SP_elec_Fan.RSLT_VIF.xml", CycleCO, LowL,
		TestName = "2nd Amendment FactorMethodRunData IHPC SmartES_elFan_ElSteer spez_Dim_HVAC"),

	TestCase(@"FactorMethod/PEV/P31_32_E2_AMT_EDP.xml", @"FactorMethod/PEV/E2_31a_completed_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_E2_AMT_EDP.RSLT_VIF.xml", CycleHU, LowL, 
		TestName = "2nd Amendment FactorMethodRunData PEV E2 EDP prim_Dim_HVAC"),
	TestCase(@"FactorMethod/PEV/P31_32_E2_AMT_FESG.xml", @"FactorMethod/PEV/E2_31a_completed_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_E2_AMT_FESG.RSLT_VIF.xml", CycleHU, LowL, 
		TestName = "2nd Amendment FactorMethodRunData PEV E2 FESG spez_Dim_HVAC"),

	TestCase(@"FactorMethod/P-HEV/P1-HEV/P31_32_P1_HEV_AT_nonSmart_ES_elec_SP_elec_PS.xml", @"FactorMethod/P-HEV/P1-HEV/P1_HEV_32e_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_P1_HEV_AT_nonSmart_ES_elec_SP_elec_PS.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData P-HEV P1 nonSmartES_elFan_elPS prim_Dim_HVAC"),
	TestCase(@"FactorMethod/P-HEV/P1-HEV/P31_32_P1_HEV_AT_nonSmart_ES_mech_Aux.xml", @"FactorMethod/P-HEV/P1-HEV/P1_HEV_32e_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_P1_HEV_AT_nonSmart_ES_mech_Aux.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData P-HEV P1 nonSmartES_mechAux spez_Dim_HVAC"),
	TestCase(@"FactorMethod/P-HEV/P1-HEV/P31_32_P1_HEV_AT_Smart_ES_elec_SP_elec_PS.xml", @"FactorMethod/P-HEV/P1-HEV/P1_HEV_32e_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_P1_HEV_AT_Smart_ES_elec_SP_elec_PS.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData P-HEV P1 SmartES_elPS_elSteer prim_Dim_HVAC"),

	TestCase(@"FactorMethod/P-HEV/P2-HEV/P31_32_P2_HEV_nonSmartES_elec_SP_elec_Fan.xml", @"FactorMethod/P-HEV/P2-HEV/P2_HEV_32e_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_P2_HEV_nonSmartES_elec_SP_elec_Fan.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData P-HEV P2 nonSmartES_elFan_elSteer prim_Dim_HVAC"),
	TestCase(@"FactorMethod/P-HEV/P2-HEV/P31_32_P2_HEV_nonSmartES_elec_SP_mech_Fan.xml", @"FactorMethod/P-HEV/P2-HEV/P2_HEV_32e_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_P2_HEV_nonSmartES_elec_SP_mech_Fan.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData P-HEV P2 nonSmartES_mechFan_elSP spez_Dim_HVAC"),
	TestCase(@"FactorMethod/P-HEV/P2-HEV/P31_32_P2_HEV_nonSmartES_mechAux.xml", @"FactorMethod/P-HEV/P2-HEV/P2_HEV_32e_prim_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_P2_HEV_nonSmartES_mechAux.RSLT_VIF.xml", CycleCO, LowL,
		TestName = "2nd Amendment FactorMethodRunData P-HEV P2 nonSmartES_mechAux prim_Dim_HVAC"),

	TestCase(@"FactorMethod/P-HEV/P2-HEV/P31_32_P2_HEV_SmartES_elec_SP_elec_Fan.xml", @"FactorMethod/P-HEV/P2-HEV/P2_HEV_32e_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_P2_HEV_SmartES_elec_SP_elec_Fan.RSLT_VIF.xml", CycleCO, LowL, 
		TestName = "2nd Amendment FactorMethodRunData P-HEV P2 SmartES_elFan_elSteer spez_Dim_HVAC"),

	TestCase(@"FactorMethod/P-HEV/P2-HEV/P31_32_P2_HEV_SmartES_elec_SP_elec_Fan.xml", @"FactorMethod/P-HEV/P2-HEV/P2_HEV_32e_spez_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_P2_HEV_SmartES_elec_SP_elec_Fan.RSLT_VIF.xml", CycleCO, LowL,
		TestName = "2nd Amendment FactorMethodRunData P-HEV P2 SmartES_elFan_elSteer spez_Dim_HVAC"),

	TestCase(@"FactorMethod/S-HEV/S2-HEV/P31_32_S2_HEV_nonSmartES_elecSP_mechFan.xml", @"FactorMethod/S-HEV/S2-HEV/S2_HEV_32e_spec_Dim_HVAC.xml", @"FactorMethod/VIF/P31_32_S2_HEV_nonSmartES_elecSP_mechFan.RSLT_VIF.xml", MissionType.Suburban, LowL,
		TestName= "2nd Amendment FactorMethodRunData S-HEV S2 nonSmartES_elecSP_mechFan"),
    ]
    public void TestFactorMethodRunData(string primary, string completed, string vifFile, MissionType mission, LoadingType loading)
	{

		var missionFilter = TestMissionFilter();
		missionFilter?.SetMissions((mission, loading));

		var outputPath = Path.Combine("FactorMethod_xEV", TestContext.CurrentContext.Test.Name.Split(' ').Skip(3).Join("_"));
		if (!Directory.Exists(outputPath)) {
			Directory.CreateDirectory(outputPath);
		}

		var singleJob = GenerateJsonJobSingleBus(Path.Combine(BASE_DIR, primary), Path.Combine(BASE_DIR, completed));
		var dataProviderSingle = JSONInputDataFactory.ReadJsonJob(singleJob);
		var fileWriterSingle = new FileOutputWriter(singleJob);
		var simFactorySingle = Kernel.Get<ISimulatorFactoryFactory>();
		var runsFactorySingle = simFactorySingle.Factory(ExecutionMode.Declaration, dataProviderSingle, fileWriterSingle, null, null);

		SerializeRunData(runsFactorySingle, outputPath);


		var filePathPrim = Path.Combine(BASE_DIR, primary);
		var dataProviderPrim = _xmlReader.CreateDeclaration(filePathPrim);
		var fileWriterPrim = new FileOutputWriter(filePathPrim);
		var simFactoryPrim = Kernel.Get<ISimulatorFactoryFactory>();
		var runsFactoryPrim = simFactoryPrim.Factory(ExecutionMode.Declaration, dataProviderPrim, fileWriterPrim, null, null);

		SerializeRunData(runsFactoryPrim, outputPath);


		var completedJob = GenerateJsonJobCompletedBus(Path.Combine(BASE_DIR, vifFile), Path.Combine(BASE_DIR, completed), false);

		var dataProviderComleted = JSONInputDataFactory.ReadJsonJob(completedJob);
		var fileWriterCompleted = new FileOutputWriter(singleJob);
		var simFactoryCompleted = Kernel.Get<ISimulatorFactoryFactory>();
		var runsFactoryCompleted = simFactoryCompleted.Factory(ExecutionMode.Declaration, dataProviderComleted, fileWriterCompleted, null, null);
		var jobContainer = new JobContainer(new SummaryDataContainer(fileWriterCompleted)) { };
		jobContainer.AddRuns(runsFactoryCompleted);
		jobContainer.Execute();
		jobContainer.WaitFinished();

		var vif = fileWriterCompleted.GetWrittenFiles().FirstOrDefault(x => x.Key == ReportType.DeclarationReportMultistageVehicleXML);
		Assert.NotNull(vif);

		var dataProviderVif = _xmlReader.CreateDeclaration(vif.Value) as IMultistepBusInputDataProvider;
		var dataProviderFinal = new XMLDeclarationVIFInputData(dataProviderVif, null, true);
        var fileWriterFinal = new FileOutputWriter(filePathPrim);
		var simFactoryFinal = Kernel.Get<ISimulatorFactoryFactory>();
		var runsFactoryFinal = simFactoryFinal.Factory(ExecutionMode.Declaration, dataProviderFinal, fileWriterFinal, null, null);

		SerializeRunData(runsFactoryFinal, outputPath);
    }

	private static void SerializeRunData(ISimulatorFactory runsFactorySingle, string outputPath)
	{
		var jsonSerializerSettings = new JsonSerializerSettings();
		jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
		foreach (var run in runsFactorySingle.SimulationRuns()) {
			var data = run.GetContainer().RunData;
			File.WriteAllText(
				Path.Combine(
					outputPath,
					$"{data.JobName}_{data.Cycle.Name}{data.ModFileSuffix}.json"),
				JsonConvert.SerializeObject(data, Formatting.Indented, jsonSerializerSettings));
		}
	}

	private void RunSimulationSingle(string jobFile, string completed, int runIdx)
	{
		var singleJob = GenerateJsonJobSingleBus(Path.Combine(BASE_DIR, jobFile), Path.Combine(BASE_DIR_COMPLETED, completed));
		var dataProvider = JSONInputDataFactory.ReadJsonJob(singleJob);
		var fileWriter = new FileOutputWriter(singleJob);
		var simFactory = Kernel.Get<ISimulatorFactoryFactory>();
		var sumData = new SummaryDataContainer(fileWriter);

        var runsFactory = simFactory.Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null);
		//runsFactory.WriteModalResults = true;
		runsFactory.SerializeVectoRunData = true;
		runsFactory.SumData = sumData;
		var jobContainer = new JobContainer(sumData) { };

		if (runIdx < 0) {
			jobContainer.AddRuns(runsFactory);
		} else {
			var run = runsFactory.SimulationRuns().Skip(runIdx).First();
			jobContainer.AddRun(run);
			//if (dataProvider.JobInputData.Vehicle.OvcHev) {
			//	var run2 = runsFactory.SimulationRuns().Skip(runIdx + 1).First();
			//	jobContainer.AddRun(run2);
			//}
		}

		PrintRuns(jobContainer, null);

		jobContainer.Execute();
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));

		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}

	private string GenerateJsonJobSingleBus(string primary, string completed)
	{
		var subDirectory = Path.GetDirectoryName(primary);

		var header = new Dictionary<string, object>() {
			{ "FileVersion", 6 }
		};
		var body = new Dictionary<string, object>() {
			{ "PrimaryVehicle", Path.GetRelativePath(subDirectory, Path.GetFullPath(primary)) },
			{ "CompletedVehicle", Path.GetRelativePath(subDirectory, Path.GetFullPath(completed)) },
			//{ "RunSimulation", true}
		};
		var json = new Dictionary<string, object>() {
			{"Header", header},
			{"Body", body}
		};

		Directory.CreateDirectory(Path.GetFullPath(subDirectory));
		var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), "Single_" + Path.GetFileNameWithoutExtension(primary) + ".vecto");
		var str = JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
		File.WriteAllText(path, str);
		return path;
	}

	public string CreateCompletedVIF(string jobFile)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = JSONInputDataFactory.ReadJsonJob(filePath);
		var fileWriter = new FileOutputWriter(filePath);
		var simFactory = Kernel.Get<ISimulatorFactoryFactory>();
		
		var runsFactory = simFactory.Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null);
		
		runsFactory.WriteModalResults = true;
		runsFactory.SerializeVectoRunData = true;
		var jobContainer = new JobContainer(new SummaryDataContainer(fileWriter)) { };
        //var jobContainer = new JobContainer(new MockSumWriter()) { };

		jobContainer.AddRuns(runsFactory);
		//PrintRuns(jobContainer, null);

		jobContainer.Execute(multithreaded:false);
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));

		//PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
		return fileWriter.GetWrittenFiles()[ReportType.DeclarationReportMultistageVehicleXML];
	}

	public void RunSimulationPrimary(string jobFile, int runIdx, params Action<VectoRunData>[] runDataModifier)
	{
		RunSimulationPrimary(jobFile, runIdx, out var _);
	}
	public void RunSimulationPrimary(string jobFile, int runIdx, out string vifFile, params Action<VectoRunData>[] runDataModifier)
	{
#if FULL_SIMULATIONS
		//runIdx = -1;
#endif

		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		var fileWriter = new FileOutputWriter(filePath);
		var sumData = new SummaryDataContainer(fileWriter);
		var simFactory = Kernel.Get<ISimulatorFactoryFactory>();
		var runsFactory = simFactory.Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null);
		runsFactory.WriteModalResults = true;
		runsFactory.SerializeVectoRunData = true;
		runsFactory.SumData = sumData;
		var jobContainer = new JobContainer(sumData) { };
		//var jobContainer = new JobContainer(new MockSumWriter()) { };
		var runs = runsFactory.SimulationRuns().ToList();
		foreach (var vectoRun in runs) {
			foreach (var action in runDataModifier) {
				action(vectoRun.GetContainer().RunData);
			}
		}
		if (runIdx < 0) {
			foreach (var run in runs) {
				jobContainer.AddRun(run);
			}
			//jobContainer.AddRuns(runsFactory);
		} else {
			var run = runs.Skip(runIdx).First();
			jobContainer.AddRun(run);
			TestContext.Progress.WriteLine($"{run.CycleName} - {run.RunSuffix}");
			var expectedResults = 1;
			if (dataProvider.JobInputData.Vehicle.OVC && !dataProvider.JobInputData.Vehicle.VehicleType.IsOneOf(
					VectoSimulationJobType.BatteryElectricVehicle, 
					VectoSimulationJobType.IEPC_E)) {
				var run2 = runsFactory.SimulationRuns().Skip(runIdx + 1).First();

				jobContainer.AddRun(run2);
				TestContext.Progress.WriteLine($"{run2.CycleName} - {run2.RunSuffix}");

				CompareStrings(run.CycleName + run.RunSuffix, run2.CycleName + run2.RunSuffix, 2);
				expectedResults++;
			}
			
			SetResultCountInReport(expectedResults, run.GetContainer().RunData.Report);
		}

		PrintRuns(jobContainer, null);

		jobContainer.Execute();
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
		vifFile = fileWriter.XMLMultistageReportFileName;
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}

	private void CompareStrings(string expected, string actual, int ignoreEnd)
	{
		var len = expected.Length - ignoreEnd;

		var expectedWithoutSuffix = expected.Substring(0, len);
		var actualWithoutSuffix = actual.Substring(0, len);
		Assert.AreEqual(expectedWithoutSuffix, actualWithoutSuffix);
	}

	private void SetResultCountInReport(int count, IDeclarationReport report)
	{
		if (report == null)
		{
			return; //also used in engineering mode
		}
		if (report is XMLDeclarationReport rep09)
		{
			GetField("_resultCount", rep09.GetType()).SetValue(rep09, count);
			return;
		}
		Assert.Fail("Reflection failed");

	}

	private FieldInfo GetField(string name, Type type)
	{
		bool found = false;
		while (!found)
		{
			FieldInfo[] fields = type.GetFields(
				BindingFlags.NonPublic |
				BindingFlags.Instance);
			var field = fields.FirstOrDefault(f => f.Name == name);
			if (field == null)
			{
				type = type.BaseType;
				if (type == null)
				{
					Assert.Fail("Field not found");
				}
			}
			else
			{
				return field;
			}
		}

		return null;

	}


	private string GenerateJsonJobCompletedBus(string vif, string completeBusInput, bool? runSimulation = null)
	{
		var subDirectory = Path.GetDirectoryName(completeBusInput);

		var header = new Dictionary<string, object>() {
			{ "FileVersion", 7 }
		};
		var body = new Dictionary<string, object>() {
			{ "PrimaryVehicleResults", Path.GetRelativePath(subDirectory, Path.GetFullPath(vif)) },
			{ "CompletedVehicle", Path.GetRelativePath(subDirectory, Path.GetFullPath(completeBusInput)) },
			{ "RunSimulation", runSimulation ?? true }
		};
		var json = new Dictionary<string, object>() {
			{"Header", header},
			{"Body", body}
		};

		Directory.CreateDirectory(Path.GetFullPath(subDirectory));
		var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), Path.GetFileNameWithoutExtension(vif) + ".vecto");
		var str = JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
		File.WriteAllText(path, str);
		return path;
	}

	private void PrintRuns(JobContainer jobContainer, FileOutputWriter fileWriter = null)
	{
		foreach (var keyValuePair in jobContainer.GetProgress()) {
			TestContext.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value.CycleName} {keyValuePair.Value.RunName} {keyValuePair.Value.Error?.Message}");
			//if (fileWriter != null && keyValuePair.Value.Success) {
			//	TestContext.AddTestAttachment(fileWriter.GetModDataFileName(keyValuePair.Value.RunName, keyValuePair.Value.CycleName, keyValuePair.Value.RunSuffix), keyValuePair.Value.RunName);
			//         }

		}
	}

	private void PrintFiles(FileOutputWriter fileWriter)
	{
		//if (fileWriter.GetWrittenFiles().Count == 0) {
		//	Assert.Fail("No files written\n");
		//}
		foreach (var keyValuePair in fileWriter.GetWrittenFiles()) {
			TestContext.WriteLine($"{keyValuePair.Key} written to {keyValuePair.Value}");
			TestContext.AddTestAttachment(keyValuePair.Value, keyValuePair.Key.ToString());
		}
	}

	[TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_Base_AT.xml",
		MissionType.Interurban,
		LoadingType.ReferenceLoad,
		53800,
		0.1,
		TestName = "2nd Amendment PrimaryBus P-HEV P1, Interurban selected section")]
	public void PrimaryBusCycleSection(string primaryFile, MissionType cycle, LoadingType loading,
		double startDistance_m, double f_equiv)
	{

#if FULL_SIMULATIONS
		Assert.Ignore($"");
#endif

        var missionFilter = TestMissionFilter();
		missionFilter.SetMissions((cycle, loading));

		var cycleFactory = StartPointCycleFactory();
		cycleFactory.SetStartPoint(startDistance_m.SI<Meter>());



		RunSimulationPrimary(primaryFile, -1, runData => {
			if (runData.HybridStrategyParameters != null) {
				var factorCharge = runData.HybridStrategyParameters.EquivalenceFactorCharge /
									runData.HybridStrategyParameters.EquivalenceFactor;
				var factorDischarge = runData.HybridStrategyParameters.EquivalenceFactorDischarge /
									runData.HybridStrategyParameters.EquivalenceFactor;
				runData.HybridStrategyParameters.EquivalenceFactor = f_equiv;
				runData.HybridStrategyParameters.EquivalenceFactorDischarge = f_equiv * factorDischarge;
				runData.HybridStrategyParameters.EquivalenceFactorCharge = f_equiv * factorCharge;
            }
		});


    }
}