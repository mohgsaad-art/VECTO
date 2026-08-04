using System;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.InputData.RunDataFactory;



[TestFixture]
public class RunDataFactoryTest
{
	private const string BASE_DIR = @"TestData/Integration/DeclarationMode/2nd_AmendDeclMode/";
	private const string HEAVY_LORRY_DIR = BASE_DIR + @"HeavyLorry/";
	protected IXMLInputDataReader xmlInputReader;
	private IKernel _kernel;

	[OneTimeSetUp]
	public void RunBeforeAnyTests()
	{
		Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

		_kernel = new StandardKernel(new VectoNinjectModule());
		xmlInputReader = _kernel.Get<IXMLInputDataReader>();
	}


	//[TestCase(@"HeavyLorry/Conventional_heavyLorry_AMT.xml"),
	//TestCase(@"HeavyLorry/HEV-S_heavyLorry_AMT_S2.xml"),
	//TestCase(@"HeavyLorry/HEV-S_heavyLorry_AMT_S2_ovc.xml"),
	//TestCase(@"HeavyLorry/HEV-S_heavyLorry_IEPC-S.xml"),
	//TestCase(@"HeavyLorry/HEV-S_heavyLorry_S3.xml"),
	//TestCase(@"HeavyLorry/HEV-S_heavyLorry_S4.xml"),
	//TestCase(@"HeavyLorry/HEV_heavyLorry_AMT_Px.xml"),
	//TestCase(@"HeavyLorry/IEPC_heavyLorry.xml"),

	//[TestCase(@"HeavyLorry/PEV_heavyLorry_AMT_E2_realistic.xml"),
	//TestCase(@"HeavyLorry/PEV_heavyLorry_E3_realistic.xml"),
	//TestCase(@"HeavyLorry/PEV_heavyLorry_E4.xml")]
	//TestCase(@"MediumLorry/Conventional_mediumLorry_AMT.xml"),
	//TestCase(@"MediumLorry/HEV-S_mediumLorry_AMT_S2.xml"),
	//TestCase(@"MediumLorry/HEV-S_mediumLorry_AMT_S2_ovc.xml"),
	//TestCase(@"MediumLorry/HEV-S_mediumLorry_IEPC-S.xml"),
	//TestCase(@"MediumLorry/HEV-S_mediumLorry_S3.xml"),
	//TestCase(@"MediumLorry/HEV-S_mediumLorry_S4.xml"),
	//TestCase(@"MediumLorry/HEV_mediumLorry_AMT_Px.xml"),
	//TestCase(@"MediumLorry/IEPC_mediumLorry.xml"),
	//TestCase(@"MediumLorry/PEV_mediumLorry_AMT_E2.xml"),
	//TestCase(@"MediumLorry/PEV_mediumLorry_APT-N_E2.xml"),
	//TestCase(@"MediumLorry/PEV_mediumLorry_E3.xml"),
	//TestCase(@"MediumLorry/PEV_mediumLorry_E4.xml"),
	//TestCase(@"PrimaryBus/Conventional_primaryBus_AMT.xml"),
	//TestCase(@"PrimaryBus/HEV-S_primaryBus_AMT_S2.xml"),
	//TestCase(@"PrimaryBus/HEV-S_primaryBus_IEPC-S.xml"),
	//TestCase(@"PrimaryBus/HEV-S_primaryBus_S3.xml"),
	//TestCase(@"PrimaryBus/HEV-S_primaryBus_S4.xml"),
	//TestCase(@"PrimaryBus/HEV_primaryBus_AMT_Px.xml"),
	//TestCase(@"PrimaryBus/IEPC_primaryBus.xml"),
	//TestCase(@"PrimaryBus/PEV_primaryBus_AMT_E2.xml"),
	//TestCase(@"PrimaryBus/PEV_primaryBus_E3.xml"),
	//TestCase(@"PrimaryBus/PEV_primaryBus_E4.xml"),
	////TestCase(@"CompletedBus/Conventional_completedBus_1.xml"),
	////TestCase(@"CompletedBus/HEV_completedBus_1.xml"),
	////TestCase(@"CompletedBus/IEPC_completedBus_1.xml"),
	////TestCase(@"CompletedBus/PEV_completedBus_1.xml"),
	//TestCase(@"ExemptedVehicles/exempted_completedBus_input_full.xml"),
	//TestCase(@"ExemptedVehicles/exempted_completedBus_input_only_mandatory_entries.xml"),
	//TestCase(@"ExemptedVehicles/exempted_heavyLorry.xml"),
	//TestCase(@"ExemptedVehicles/exempted_mediumLorry.xml"),
	//TestCase(@"ExemptedVehicles/exempted_primaryBus.xml"),
	public void PEVHeavyLorryTest(string jobFile)
	{
		var runData = CreateRundata(jobFile,BASE_DIR,
			
			PEV_E3Checks, HasInifinityBattery
			);
	}

	private void HasInifinityBattery(IDeclarationInputDataProvider inputData, IVectoRun[] runs)
	{
		Assert.IsTrue(runs.All((run => run.GetContainer().RunData.BatteryData.Batteries.Select(bd => bd.Item2)
			.All(b => b.ChargeDepletingBattery))), "PEVs should use the infinity battery");
	}

	private void PEV_E2Checks(IDeclarationInputDataProvider inputData, IVectoRun[] runs)
	{
		if (inputData.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E2) {
			return;
		}

		foreach (var vectoRun in runs)
		{
			Assert.IsTrue(vectoRun.GetContainer().RunData.GearboxSinglePwt != null);
		}

	}

	private void PEV_E3Checks(IDeclarationInputDataProvider inputData, IVectoRun[] runs)
	{
		if (inputData.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E3) {
			return;
		}

		foreach (var vectoRun in runs) {
			Assert.IsTrue(vectoRun.GetContainer().RunData.GearboxSinglePwt == null);
		}
	}

	public IVectoRun[] CreateRundata(string jobFile, string basePath, params Action<IDeclarationInputDataProvider, IVectoRun[]>[] runAsserts)
	{
		var filename = Path.Combine(basePath, jobFile);

		var fileWriter = new FileOutputWriter(filename);
		//var sumWriter = new SummaryDataContainer(fileWriter);
		//var jobContainer = new JobContainer(sumWriter);
		var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
		var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null, false);
        runsFactory.ModalResults1Hz = false;
		runsFactory.WriteModalResults = false;
		runsFactory.ActualModalData = false;
		runsFactory.Validate = false;



		var runs = runsFactory.SimulationRuns().ToArray();
		Assert.IsTrue(runs.Length > 0);
		foreach (var runAssert in runAsserts) {
			runAssert(dataProvider, runs);
		}

		



		return runs;
	}
}