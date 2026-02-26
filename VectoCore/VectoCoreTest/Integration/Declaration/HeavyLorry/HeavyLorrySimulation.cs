

//#define singlethreaded

using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.HeavyLorry;



[TestFixture]
public class HeavyLorrySimulation
{


	private const string BASE_DIR = @"TestData/Integration/DeclarationMode/2nd_AmendmDeclMode/";
	private StandardKernel _kernel;
	private IXMLInputDataReader _xmlReader;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_xmlReader = _kernel.Get<IXMLInputDataReader>();
	}

	[TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_AMT_E2_realistic.xml"),
	TestCase(@"HeavyLorry/Conventional/Conventional_heavyLorry_AMT.xml"),
	TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_E3_realistic.xml"),
	TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_E3_realistic_TorqueLimits.xml"),
	TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_E3_realistic_municipal.xml"),
	TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_AMT_E2_pto_transm.xml"),
	TestCase(@"HeavyLorry/PEV/PEV_heavyLorry_E4.xml"),
	TestCase(@"HeavyLorry/S-HEV/Group2_HEV_S2.xml"),
	TestCase(@"HeavyLorry/S-HEV/Group5_HEV_P2_.xml"),
		Ignore("Testdata not intended for simulation - mostly dummy data")
	]
	public void HeavyLorrySimulationTest(string jobFile)
	{
#if singlethreaded
		RunSimulation(jobFile, false);
#else
		RunSimulation(jobFile, true);
#endif
	}

	public void RunSimulation(string jobFile, bool multiThreaded = true)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		var fileWriter = new FileOutputWriter(filePath);
		var runsFactory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null, false);
        runsFactory.WriteModalResults = true;
		var jobContainer = new JobContainer(new MockSumWriter()){};
		//var jobContainer = new JobContainer(new MockSumWriter()) { };
		jobContainer.AddRuns(runsFactory);
		PrintRuns(jobContainer, null);
		
		jobContainer.Execute(multiThreaded);
		if (multiThreaded) {
			jobContainer.WaitFinished();
		}
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}

	

	








	private void PrintRuns(JobContainer jobContainer, FileOutputWriter fileWriter = null)
	{
		foreach (var keyValuePair in jobContainer.GetProgress()) {
			TestContext.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value.CycleName} {keyValuePair.Value.RunName} {keyValuePair.Value.Error?.Message}" );
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
}
