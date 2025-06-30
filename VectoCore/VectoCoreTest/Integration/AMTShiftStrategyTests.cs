using System;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration
{
	public class AMTShiftStrategyTests
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();

		}

		private const int RunIdx = 0;

		//[TestCase(
		//	@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group2_HEV_IEPC_S.xml", 0)]
		[TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group5_Conv_ES_Standard.xml", 8)]
		//[TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\HeavyLorry_IHPC.xml", 4)]

		[TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\exempted_heavy_lorry.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group2_HEV_IEPC_S.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group2_HEV_S2_ovc.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group5_ PEV_E4.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group5_ PEV_IEPC_E.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group5_Conv_ES_Standard.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group5_HEV_P2_non-ovc.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group5_HEV_P2_ovc.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group5_HEV_P2_supercap.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\HeavyLorry_IHPC.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\PEV_heavyLorry_AMT_E2.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\PrimaryBuses\PEV_primaryBus_AMT_E2.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\PrimaryBuses\PrimaryCoach_S2_Base_AMT.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\Group P31_32\primary_heavyBus group_P31_32_Smart_ES.xml", RunIdx)]
        //[TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Engineering Mode\GenericVehicleE2\BEV_ENG.vecto", 0)]
        //[TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Engineering Mode\GenericVehicle_S2_Job\SerialHybrid_S2.vecto", 0)]
        //[TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Engineering Mode\GenericVehicle_Group5_P2\P2 Group 5.vecto", 0)]
        //[TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Engineering Mode\GenericIEPC\IEPC_Gbx3Speed+Axle\IEPC_ENG_Gbx3Axl.vecto", 0)]
        //[TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Engineering Mode\CityBus_AT\CityBus_AT_PS.vecto", 0)]
        //[TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Engineering Mode\EngineOnly\EngineOnly.vecto", 0)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\Group 53\ML3r.vecto", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\PEV_heavyLorry_APTN_E2..xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group2_HEV_S2_APTN.xml", RunIdx)]
        [TestCase(@"E:\QUAM\Workspace\VECTO_DEV_SW3\Generic Vehicles\Declaration Mode\xEV XML Jobs\Lorries\Group2_HEV_IEPC_S-APTN.xml", RunIdx)]
		public void RunJob_G2_HEV_IEPC_S(string job, int runIdx)
		{
			if (runIdx < 0) {
				RunJob_DeclAll(job);
			} else {
				RunJob_DeclSingle(job, runIdx);
			}
		}
        //[TestCase()]
        //public void VECTO_EffShift()
        //{
        //	var jobName =
        //		@"E:/QUAM/tmp/1a_EffShift_high-engine-rev_UD-cycle_LH-tractor_model/vecto_tractor_4x2_overdr_EffShift-def.vecto";
        //	RunJob_DeclSingle(jobName, 9);
        //}

        public void RunJob_DeclSingle(string jobName, int runIdx)
		{
			if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(jobName), _tmpDir))) {
				Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(jobName), _tmpDir));
			}
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), _tmpDir, Path.GetFileName(jobName)));

            var relativeJobPath = jobName;
			//var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var simFactory = _kernel.Get<ISimulatorFactoryFactory>();
			var factory = simFactory.Factory(ExecutionMode.Declaration, inputData, writer, null, null);
            factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToArray();
			
			runs[runIdx].Run();

			Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);
		}

		private const string _tmpDir = "tmp2";

		public void RunJob_DeclAll(string jobName)
		{
			if (!Directory.Exists(Path.Combine(Path.GetDirectoryName(jobName), _tmpDir))) {
				Directory.CreateDirectory(Path.Combine(Path.GetDirectoryName(jobName), _tmpDir));
			}
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), _tmpDir, Path.GetFileName(jobName)));

            var relativeJobPath = jobName;
			//var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? xmlInputReader.CreateDeclaration(relativeJobPath)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var simFactory = _kernel.Get<ISimulatorFactoryFactory>();
			var factory = simFactory.Factory(ExecutionMode.Declaration, inputData, writer, null, null);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), String.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

	}
}
