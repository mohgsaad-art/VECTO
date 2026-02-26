using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.DrivingAction;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.PCCStates;

namespace TUGraz.VectoCore.Tests.Integration.ADAS
{
	/// <summary>
	/// The engineering ADAS Test generate their segments in the output console.
	/// You can copy and adapt it from there for easier test creation.
	/// </summary>
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ADASTestsHEV
	{
        private StandardKernel _kernel;

        private const string BasePath = @"TestData/Integration/ADAS-HEV/Group5PCCEng/";
		private const double tolerance = 1; //seconds of tolerance. Tolerance distance is calculated dynamically based on speed.

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
            _kernel = new StandardKernel(new VectoNinjectModule());
        }

		private GraphWriter GetGraphWriter()
		{
			var graphWriter = new GraphWriter();
			//#if VECTOTRACE
			graphWriter.Enable();
			//#else
			//graphWriter.Disable();
			//#endif
			graphWriter.Xfields = new[] { ModalResultField.dist };

			graphWriter.Yfields = new[] {
				ModalResultField.v_act, ModalResultField.altitude, ModalResultField.acc, ModalResultField.Gear,
				ModalResultField.P_ice_out, ModalResultField.FCMap
			};
			graphWriter.Series1Label = "ADAS PCC";
			graphWriter.PlotIgnitionState = true;
			return graphWriter;
		}

		[TestCase(@"TestData/Integration/ADAS-HEV/VECTO-1493/P1_CityBus.vecto", TestName = "VECTO-1493_P1Citybus")]
		[TestCase(@"TestData/Integration/ADAS-HEV/VECTO-1484/P2_Group5_s2c0_rep_Payload.vecto", TestName = "VECTO-1484_P2Group5")]
		public void RunEngineeringJob(string jobName)
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));
			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, inputData, writer, null, null, false);
            factory.WriteModalResults = true;
			factory.Validate = false;
			factory.SumData = sumContainer;
			
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
		}


		[
		TestCase(0, TestName = "EcoRoll DH1.1 const"),
		TestCase(1, TestName = "EcoRoll DH1.1 UH0.1"),
		TestCase(2, TestName = "EcoRoll DH1.4 const"),
		TestCase(3, TestName = "EcoRoll DH0.8 const - too flat"),
		TestCase(4, TestName = "EcoRoll DH1.7 const - too steep"),
		TestCase(5, TestName = "EcoRoll DH1.1 const - Stop"),
		TestCase(6, TestName = "EcoRoll DH1.1 const - TS60"),
		TestCase(7, TestName = "EcoRoll DH1.1 const - TS68"),
		TestCase(8, TestName = "EcoRoll DH1.1 const - TS72"),
		TestCase(9, TestName = "EcoRoll DH1.1 const - TS80"),
		]
		public void TestEcoRoll(int cycleIdx)
		{
			var jobName = @"TestData/Integration/ADAS-HEV/Group5EcoRollEng/Class5_Tractor_ENG.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, inputData, writer, null, null, false);
            factory.WriteModalResults = true;
			factory.Validate = false;
			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			var modFilename = writer.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);
			GetGraphWriter().Write(modFilename);
		}

		[TestCase(0, TestName = "AT EcoRoll Neutral DH1.8 const"),
		TestCase(1, TestName = "AT EcoRoll Neutral DH1.8 UH0.1"),
		TestCase(2, TestName = "AT EcoRoll Neutral DH1.9 const"),
		TestCase(3, TestName = "AT EcoRoll Neutral DH1.2 const - too flat"),
		TestCase(4, TestName = "AT EcoRoll Neutral DH2.5 const - too steep"),
		TestCase(5, TestName = "AT EcoRoll Neutral DH1.9 const - Stop"),
		TestCase(6, TestName = "AT EcoRoll Neutral DH1.9 const - TS60"),
		TestCase(7, TestName = "AT EcoRoll Neutral DH1.9 const - TS68"),
		TestCase(8, TestName = "AT EcoRoll Neutral DH1.9 const - TS72"),
		TestCase(9, TestName = "AT EcoRoll Neutral DH1.9 const - TS80"),
		TestCase(10, TestName = "AT EcoRoll Neutral DH1.2 const"),
		]
		public void TestEcoRollAT_Neutral(int cycleIdx)
		{
			var jobName = @"TestData/Integration/ADAS-HEV/Group9_RigidTruck_AT/Class_9_RigidTruck_AT_Eng_Neutral.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, inputData, writer, null, null, false);
            factory.WriteModalResults = true;
			factory.Validate = false;
			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			var modFilename = writer.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);
			GetGraphWriter().Write(modFilename);
		}


		[TestCase(0, TestName = "AT EcoRoll TC DH1.8 const"),
		TestCase(1, TestName = "AT EcoRoll TC DH1.8 UH0.1"),
		TestCase(2, TestName = "AT EcoRoll TC DH1.9 const"),
		TestCase(3, TestName = "AT EcoRoll TC DH1.2 const - too flat"),
		TestCase(4, TestName = "AT EcoRoll TC DH2.5 const - too steep"),
		TestCase(5, TestName = "AT EcoRoll TC DH1.9 const - Stop"),
		TestCase(6, TestName = "AT EcoRoll TC DH1.9 const - TS60"),
		TestCase(7, TestName = "AT EcoRoll TC DH1.9 const - TS68"),
		TestCase(8, TestName = "AT EcoRoll TC DH1.9 const - TS72"),
		TestCase(9, TestName = "AT EcoRoll TC DH1.9 const - TS80"),
		]
		public void TestEcoRollAT_TC(int cycleIdx)
		{
			var jobName = @"TestData/Integration/ADAS-HEV/Group9_RigidTruck_AT/Class_9_RigidTruck_AT_Eng_TC.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, inputData, writer, null, null, false);
			factory.WriteModalResults = true;
			factory.Validate = false;
			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			var modFilename = writer.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix);
			GetGraphWriter().Write(modFilename);
		}

		[TestCase]
		public void Class5_PCC123_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5809, UseCase1, Coast),                // len: 337m
			(5809, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(595, 4655, OutsideSegment, Accelerate),      // len: 4060m
			(4655, 5495, WithinSegment, Accelerate),      // len: 840m
			(5495, 7251, UseCase1, Coast),                // len: 1756m
			(7251, 7486, OutsideSegment, Coast),          // len: 235m
			(7486, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(234, 3979, OutsideSegment, Accelerate),      // len: 3745m
			(3979, 4935, WithinSegment, Accelerate),      // len: 956m
			(4935, 6104, UseCase1, Coast),                // len: 1169m
			(6104, 6298, WithinSegment, Coast),           // len: 194m
			(6298, 7138, WithinSegment, Brake),           // len: 840m
			(7138, 7151, WithinSegment, Coast),           // len: 13m
			(7151, 7563, OutsideSegment, Coast),          // len: 412m
			(7563, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(549, 654, OutsideSegment, Accelerate),       // len: 105m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2487, UseCase1, Coast),                // len: 585m
			(2487, 2510, OutsideSegment, Coast),          // len: 23m
			(2510, 4038, OutsideSegment, Accelerate),     // len: 1528m
			(4038, 4960, WithinSegment, Accelerate),      // len: 922m
			(4960, 6210, UseCase1, Coast),                // len: 1250m
			(6210, 6636, WithinSegment, Coast),           // len: 426m
			(6636, 6673, WithinSegment, Brake),           // len: 37m
			(6673, 6685, WithinSegment, Coast),           // len: 12m
			(6685, 7085, OutsideSegment, Coast),          // len: 400m
			(7085, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 688, OutsideSegment, Accelerate),       // len: 116m
			(688, 2088, WithinSegment, Accelerate),       // len: 1400m
			(2088, 2379, UseCase1, Coast),                // len: 291m
			(2379, 2402, WithinSegment, Coast),           // len: 23m
			(2402, 2997, WithinSegment, Accelerate),      // len: 595m
			(2997, 3876, UseCase1, Coast),                // len: 879m
			(3876, 3970, WithinSegment, Coast),           // len: 94m
			(3970, 4171, OutsideSegment, Coast),          // len: 201m
			(4171, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 700, OutsideSegment, Accelerate),       // len: 128m
			(700, 2088, WithinSegment, Accelerate),       // len: 1388m
			(2088, 2390, UseCase1, Coast),                // len: 302m
			(2390, 2646, WithinSegment, Accelerate),      // len: 256m
			(2646, 3267, UseCase1, Coast),                // len: 621m
			(3267, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5901, UseCase1, Coast),                // len: 802m
			(5901, 6339, WithinSegment, Coast),           // len: 438m
			(6339, 6537, WithinSegment, Brake),           // len: 198m
			(6537, 6561, WithinSegment, Coast),           // len: 24m
			(6561, 7288, OutsideSegment, Coast),          // len: 727m
			(7288, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5987, UseCase1, Coast),                // len: 1133m
			(5987, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(537, 3558, OutsideSegment, Accelerate),      // len: 3021m
			(3558, 4200, WithinSegment, Accelerate),      // len: 642m
			(4200, 4223, WithinSegment, Roll),            // len: 23m
			(4223, 4586, WithinSegment, Accelerate),      // len: 363m
			(4586, 4605, WithinSegment, Roll),            // len: 19m
			(4605, 4747, WithinSegment, Accelerate),      // len: 142m
			(4747, 4762, WithinSegment, Roll),            // len: 15m
			(4762, 4888, WithinSegment, Accelerate),      // len: 126m
			(4888, 4900, WithinSegment, Roll),            // len: 12m
			(4900, 4981, WithinSegment, Accelerate),      // len: 81m
			(4981, 4994, WithinSegment, Roll),            // len: 13m
			(4994, 5116, WithinSegment, Accelerate),      // len: 122m
			(5116, 5133, WithinSegment, Roll),            // len: 17m
			(5133, 5280, WithinSegment, Accelerate),      // len: 147m
			(5280, 5410, UseCase2, Coast),                // len: 130m
			(5410, 5421, UseCase2, Accelerate),           // len: 11m
			(5421, 5433, UseCase2, Roll),                 // len: 12m
			(5433, 5629, UseCase2, Coast),                // len: 196m
			(5629, 5969, WithinSegment, Coast),           // len: 340m
			(5969, 6043, WithinSegment, Brake),           // len: 74m
			(6043, 6117, WithinSegment, Coast),           // len: 74m
			(6117, 6456, OutsideSegment, Coast),          // len: 339m
			(6456, 6747, OutsideSegment, Accelerate),     // len: 291m
			(6747, 6771, OutsideSegment, Roll),           // len: 24m
			(6771, 1e6, OutsideSegment, Accelerate));

        [TestCase]
		public void Class5_PCC123_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3804, OutsideSegment, Accelerate),      // len: 3393m
			(3804, 4533, OutsideSegment, Coast),          // len: 729m
			(4533, 5006, OutsideSegment, Brake),          // len: 473m
			(5006, 5413, OutsideSegment, Coast),          // len: 407m
			(5413, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC123_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3609, OutsideSegment, Accelerate),      // len: 3198m
			(3609, 4202, OutsideSegment, Coast),          // len: 593m
			(4202, 4504, OutsideSegment, Brake),          // len: 302m
			(4504, 4722, OutsideSegment, Coast),          // len: 218m
			(4722, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(525, 4130, OutsideSegment, Accelerate),      // len: 3605m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5808, UseCase1, Coast),                // len: 336m
			(5808, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(595, 4655, OutsideSegment, Accelerate),      // len: 4060m
			(4655, 5495, WithinSegment, Accelerate),      // len: 840m
			(5495, 7251, UseCase1, Coast),                // len: 1756m
			(7251, 7486, OutsideSegment, Coast),          // len: 235m
			(7486, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC12_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(234, 3979, OutsideSegment, Accelerate),      // len: 3745m
			(3979, 4935, WithinSegment, Accelerate),      // len: 956m
			(4935, 6104, UseCase1, Coast),                // len: 1169m
			(6104, 6164, WithinSegment, Coast),           // len: 60m
			(6164, 7149, WithinSegment, Brake),           // len: 985m
			(7149, 7446, OutsideSegment, Coast),          // len: 297m
			(7446, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(549, 654, OutsideSegment, Accelerate),       // len: 105m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2487, UseCase1, Coast),                // len: 585m
			(2487, 2510, OutsideSegment, Coast),          // len: 23m
			(2510, 4038, OutsideSegment, Accelerate),     // len: 1528m
			(4038, 4960, WithinSegment, Accelerate),      // len: 922m
			(4960, 6210, UseCase1, Coast),                // len: 1250m
			(6210, 6305, WithinSegment, Coast),           // len: 95m
			(6305, 6690, WithinSegment, Brake),           // len: 385m
			(6690, 6975, OutsideSegment, Coast),          // len: 285m
			(6975, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC12_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 688, OutsideSegment, Accelerate),       // len: 116m
			(688, 2088, WithinSegment, Accelerate),       // len: 1400m
			(2088, 2379, UseCase1, Coast),                // len: 291m
			(2379, 2402, WithinSegment, Coast),           // len: 23m
			(2402, 2997, WithinSegment, Accelerate),      // len: 595m
			(2997, 3876, UseCase1, Coast),                // len: 879m
			(3876, 3970, WithinSegment, Coast),           // len: 94m
			(3970, 4171, OutsideSegment, Coast),          // len: 201m
			(4171, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 700, OutsideSegment, Accelerate),       // len: 128m
			(700, 2088, WithinSegment, Accelerate),       // len: 1388m
			(2088, 2390, UseCase1, Coast),                // len: 302m
			(2390, 2646, WithinSegment, Accelerate),      // len: 256m
			(2646, 3267, UseCase1, Coast),                // len: 621m
			(3267, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5901, UseCase1, Coast),                // len: 802m
			(5901, 5997, WithinSegment, Coast),           // len: 96m
			(5997, 6561, WithinSegment, Brake),           // len: 564m
			(6561, 7073, OutsideSegment, Coast),          // len: 512m
			(7073, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5987, UseCase1, Coast),                // len: 1133m
			(5987, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(537, 3558, OutsideSegment, Accelerate),      // len: 3021m
			(3558, 4200, WithinSegment, Accelerate),      // len: 642m
			(4200, 4223, WithinSegment, Roll),            // len: 23m
			(4223, 4586, WithinSegment, Accelerate),      // len: 363m
			(4586, 4605, WithinSegment, Roll),            // len: 19m
			(4605, 4747, WithinSegment, Accelerate),      // len: 142m
			(4747, 4762, WithinSegment, Roll),            // len: 15m
			(4762, 4888, WithinSegment, Accelerate),      // len: 126m
			(4888, 4900, WithinSegment, Roll),            // len: 12m
			(4900, 4981, WithinSegment, Accelerate),      // len: 81m
			(4981, 4994, WithinSegment, Roll),            // len: 13m
			(4994, 5116, WithinSegment, Accelerate),      // len: 122m
			(5116, 5133, WithinSegment, Roll),            // len: 17m
			(5133, 5280, WithinSegment, Accelerate),      // len: 147m
			(5280, 5410, UseCase2, Coast),                // len: 130m
			(5410, 5421, UseCase2, Accelerate),           // len: 11m
			(5421, 5433, UseCase2, Roll),                 // len: 12m
			(5433, 5629, UseCase2, Coast),                // len: 196m
			(5629, 5712, WithinSegment, Coast),           // len: 83m
			(5712, 6061, WithinSegment, Brake),           // len: 349m
			(6061, 6121, WithinSegment, Coast),           // len: 60m
			(6121, 6346, OutsideSegment, Coast),          // len: 225m
			(6346, 6510, OutsideSegment, Accelerate),     // len: 164m
			(6510, 6533, OutsideSegment, Roll),           // len: 23m
			(6533, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3804, OutsideSegment, Accelerate),      // len: 3393m
			(3804, 4533, OutsideSegment, Coast),          // len: 729m
			(4533, 5006, OutsideSegment, Brake),          // len: 473m
			(5006, 5413, OutsideSegment, Coast),          // len: 407m
			(5413, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC12_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3609, OutsideSegment, Accelerate),      // len: 3198m
			(3609, 4202, OutsideSegment, Coast),          // len: 593m
			(4202, 4504, OutsideSegment, Brake),          // len: 302m
			(4504, 4722, OutsideSegment, Coast),          // len: 218m
			(4722, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(525, 5612, OutsideSegment, Accelerate),      // len: 5087m
			(5612, 5916, OutsideSegment, Coast),          // len: 304m
			(5916, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(595, 6113, OutsideSegment, Accelerate),      // len: 5518m
			(6113, 6786, OutsideSegment, Coast),          // len: 673m
			(6786, 7255, OutsideSegment, Brake),          // len: 469m
			(7255, 7731, OutsideSegment, Coast),          // len: 476m
			(7731, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(234, 5460, OutsideSegment, Accelerate),      // len: 5226m
			(5460, 5802, OutsideSegment, Coast),          // len: 342m
			(5802, 7148, OutsideSegment, Brake),          // len: 1346m
			(7148, 7445, OutsideSegment, Coast),          // len: 297m
			(7445, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(549, 2147, OutsideSegment, Accelerate),      // len: 1598m
			(2147, 2300, OutsideSegment, Coast),          // len: 153m
			(2300, 2481, OutsideSegment, Brake),          // len: 181m
			(2481, 2599, OutsideSegment, Coast),          // len: 118m
			(2599, 5516, OutsideSegment, Accelerate),     // len: 2917m
			(5516, 5870, OutsideSegment, Coast),          // len: 354m
			(5870, 6687, OutsideSegment, Brake),          // len: 817m
			(6687, 6972, OutsideSegment, Coast),          // len: 285m
			(6972, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 2182, OutsideSegment, Accelerate),      // len: 1610m
			(2182, 2464, OutsideSegment, Coast),          // len: 282m
			(2464, 3327, OutsideSegment, Accelerate),     // len: 863m
			(3327, 3575, OutsideSegment, Coast),          // len: 248m
			(3575, 3971, OutsideSegment, Brake),          // len: 396m
			(3971, 4232, OutsideSegment, Coast),          // len: 261m
			(4232, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 2193, OutsideSegment, Accelerate),      // len: 1621m
			(2193, 2499, OutsideSegment, Coast),          // len: 306m
			(2499, 2884, OutsideSegment, Accelerate),     // len: 385m
			(2884, 3450, OutsideSegment, Coast),          // len: 566m
			(3450, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5437, OutsideSegment, Accelerate),        // len: 5437m
			(5437, 5603, OutsideSegment, Coast),          // len: 166m
			(5603, 6564, OutsideSegment, Brake),          // len: 961m
			(6564, 7075, OutsideSegment, Coast),          // len: 511m
			(7075, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(385, 5297, OutsideSegment, Accelerate),      // len: 4912m
			(5297, 5651, OutsideSegment, Coast),          // len: 354m
			(5651, 5987, OutsideSegment, Brake),          // len: 336m
			(5987, 6249, OutsideSegment, Coast),          // len: 262m
			(6249, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(35, 420, OutsideSegment, Accelerate),        // len: 385m
			(420, 444, OutsideSegment, Roll),             // len: 24m
			(444, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4563, OutsideSegment, Accelerate),        // len: 4563m
			(5146, 5443, OutsideSegment, Accelerate),     // len: 297m
			(5443, 5538, OutsideSegment, Coast),          // len: 95m
			(5538, 6115, OutsideSegment, Brake),          // len: 577m
			(6115, 6400, OutsideSegment, Coast),          // len: 285m
			(6400, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3804, OutsideSegment, Accelerate),      // len: 3393m
			(3804, 4533, OutsideSegment, Coast),          // len: 729m
			(4533, 5006, OutsideSegment, Brake),          // len: 473m
			(5006, 5413, OutsideSegment, Coast),          // len: 407m
			(5413, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3609, OutsideSegment, Accelerate),      // len: 3198m
			(3609, 4202, OutsideSegment, Coast),          // len: 593m
			(4202, 4504, OutsideSegment, Brake),          // len: 302m
			(4504, 4722, OutsideSegment, Coast),          // len: 218m
			(4722, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5809, UseCase1, Coast),                // len: 337m
			(5809, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 7252, UseCase1, Coast),                // len: 1756m
			(7252, 7486, OutsideSegment, Coast),          // len: 234m
			(7486, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 6104, UseCase1, Coast),                // len: 1168m
			(6104, 6298, WithinSegment, Coast),           // len: 194m
			(6298, 7139, WithinSegment, Brake),           // len: 841m
			(7139, 7151, WithinSegment, Coast),           // len: 12m
			(7151, 7563, OutsideSegment, Coast),          // len: 412m
			(7563, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2475, UseCase1, Coast),                // len: 573m
			(2475, 2511, OutsideSegment, Coast),          // len: 36m
			(2511, 4051, OutsideSegment, Accelerate),     // len: 1540m
			(4051, 4949, WithinSegment, Accelerate),      // len: 898m
			(4949, 6218, UseCase1, Coast),                // len: 1269m
			(6218, 6680, WithinSegment, Coast),           // len: 462m
			(6680, 7079, OutsideSegment, Coast),          // len: 399m
			(7079, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2378, UseCase1, Coast),                // len: 301m
			(2378, 2996, WithinSegment, Accelerate),      // len: 618m
			(2996, 3875, UseCase1, Coast),                // len: 879m
			(3875, 3970, WithinSegment, Coast),           // len: 95m
			(3970, 4170, OutsideSegment, Coast),          // len: 200m
			(4170, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2390, UseCase1, Coast),                // len: 301m
			(2390, 2647, WithinSegment, Accelerate),      // len: 257m
			(2647, 3267, UseCase1, Coast),                // len: 620m
			(3267, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(280, 3944, OutsideSegment, Accelerate),      // len: 3664m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5901, UseCase1, Coast),                // len: 802m
			(5901, 6339, WithinSegment, Coast),           // len: 438m
			(6339, 6536, WithinSegment, Brake),           // len: 197m
			(6536, 6549, WithinSegment, Coast),           // len: 13m
			(6549, 7288, OutsideSegment, Coast),          // len: 739m
			(7288, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5987, UseCase1, Coast),                // len: 1133m
			(5987, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4200, WithinSegment, Accelerate),      // len: 641m
			(4200, 4224, WithinSegment, Roll),            // len: 24m
			(4224, 4583, WithinSegment, Accelerate),      // len: 359m
			(4583, 4602, WithinSegment, Roll),            // len: 19m
			(4602, 4714, WithinSegment, Accelerate),      // len: 112m
			(4714, 4730, WithinSegment, Roll),            // len: 16m
			(4730, 4813, WithinSegment, Accelerate),      // len: 83m
			(4813, 4825, WithinSegment, Roll),            // len: 12m
			(4825, 5020, WithinSegment, Accelerate),      // len: 195m
			(5020, 5033, WithinSegment, Roll),            // len: 13m
			(5033, 5083, WithinSegment, Accelerate),      // len: 50m
			(5083, 5099, WithinSegment, Roll),            // len: 16m
			(5099, 5202, WithinSegment, Accelerate),      // len: 103m
			(5202, 5221, WithinSegment, Roll),            // len: 19m
			(5221, 5322, WithinSegment, Accelerate),      // len: 101m
			(5322, 5627, UseCase2, Coast),                // len: 305m
			(5627, 5845, WithinSegment, Coast),           // len: 218m
			(5845, 6105, WithinSegment, Brake),           // len: 260m
			(6105, 6117, WithinSegment, Coast),           // len: 12m
			(6117, 6505, OutsideSegment, Coast),          // len: 388m
			(6505, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3803, OutsideSegment, Accelerate),      // len: 3344m
			(3803, 4533, OutsideSegment, Coast),          // len: 730m
			(4533, 5006, OutsideSegment, Brake),          // len: 473m
			(5006, 5413, OutsideSegment, Coast),          // len: 407m
			(5413, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_P3_PCC123EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3609, OutsideSegment, Accelerate),      // len: 3150m
			(3609, 4202, OutsideSegment, Coast),          // len: 593m
			(4202, 4504, OutsideSegment, Brake),          // len: 302m
			(4504, 4722, OutsideSegment, Coast),          // len: 218m
			(4722, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5461, WithinSegment, Accelerate),      // len: 1331m
			(5461, 5808, UseCase1, Coast),                // len: 347m
			(5808, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5472, WithinSegment, Accelerate),      // len: 817m
			(5472, 7247, UseCase1, Coast),                // len: 1775m
			(7247, 7493, OutsideSegment, Coast),          // len: 246m
			(7493, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 6094, UseCase1, Coast),                // len: 1158m
			(6094, 6288, WithinSegment, Coast),           // len: 194m
			(6288, 7141, WithinSegment, Brake),           // len: 853m
			(7141, 7153, WithinSegment, Coast),           // len: 12m
			(7153, 7566, OutsideSegment, Coast),          // len: 413m
			(7566, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1891, WithinSegment, Accelerate),       // len: 1237m
			(1891, 2485, UseCase1, Coast),                // len: 594m
			(2485, 2497, OutsideSegment, Coast),          // len: 12m
			(2497, 4037, OutsideSegment, Accelerate),     // len: 1540m
			(4037, 4947, WithinSegment, Accelerate),      // len: 910m
			(4947, 6205, UseCase1, Coast),                // len: 1258m
			(6205, 6606, WithinSegment, Coast),           // len: 401m
			(6606, 6680, WithinSegment, Brake),           // len: 74m
			(6680, 6692, WithinSegment, Coast),           // len: 12m
			(6692, 7092, OutsideSegment, Coast),          // len: 400m
			(7092, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2378, UseCase1, Coast),                // len: 301m
			(2378, 2985, WithinSegment, Accelerate),      // len: 607m
			(2985, 3906, UseCase1, Coast),                // len: 921m
			(3906, 3977, WithinSegment, Coast),           // len: 71m
			(3977, 4153, OutsideSegment, Coast),          // len: 176m
			(4153, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2077, WithinSegment, Accelerate),       // len: 1377m
			(2077, 2389, UseCase1, Coast),                // len: 312m
			(2389, 2634, WithinSegment, Accelerate),      // len: 245m
			(2634, 3266, UseCase1, Coast),                // len: 632m
			(3266, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5890, UseCase1, Coast),                // len: 791m
			(5890, 6291, WithinSegment, Coast),           // len: 401m
			(6291, 6538, WithinSegment, Brake),           // len: 247m
			(6538, 6563, WithinSegment, Coast),           // len: 25m
			(6563, 7290, OutsideSegment, Coast),          // len: 727m
			(7290, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4842, WithinSegment, Accelerate),      // len: 1027m
			(4842, 5997, UseCase1, Coast),                // len: 1155m
			(5997, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4142, WithinSegment, Accelerate),      // len: 583m
			(4142, 4165, WithinSegment, Roll),            // len: 23m
			(4165, 4591, WithinSegment, Accelerate),      // len: 426m
			(4591, 4608, WithinSegment, Roll),            // len: 17m
			(4608, 4688, WithinSegment, Accelerate),      // len: 80m
			(4688, 4702, WithinSegment, Roll),            // len: 14m
			(4702, 4753, WithinSegment, Accelerate),      // len: 51m
			(4753, 4765, WithinSegment, Roll),            // len: 12m
			(4765, 4816, WithinSegment, Accelerate),      // len: 51m
			(4816, 4825, WithinSegment, Roll),            // len: 9m
			(4825, 4986, WithinSegment, Accelerate),      // len: 161m
			(4986, 4996, WithinSegment, Roll),            // len: 10m
			(4996, 5054, WithinSegment, Accelerate),      // len: 58m
			(5054, 5068, WithinSegment, Roll),            // len: 14m
			(5068, 5119, WithinSegment, Accelerate),      // len: 51m
			(5119, 5135, WithinSegment, Roll),            // len: 16m
			(5135, 5329, WithinSegment, Accelerate),      // len: 194m
			(5329, 5340, WithinSegment, Roll),            // len: 11m
			(5340, 5350, UseCase2, Roll),                 // len: 10m
			(5350, 5666, UseCase2, Coast),                // len: 316m
			(5666, 5909, WithinSegment, Coast),           // len: 243m
			(5909, 6107, WithinSegment, Brake),           // len: 198m
			(6107, 6119, WithinSegment, Coast),           // len: 12m
			(6119, 6507, OutsideSegment, Coast),          // len: 388m
			(6507, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(600, 3415, OutsideSegment, Accelerate),      // len: 2887m
			(3415, 4504, OutsideSegment, Coast),          // len: 1089m
			(4504, 5008, OutsideSegment, Brake),          // len: 504m
			(5008, 5415, OutsideSegment, Coast),          // len: 407m
			(5415, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P4_PCC123EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(600, 3610, OutsideSegment, Accelerate),      // len: 3082m
			(3610, 4173, OutsideSegment, Coast),          // len: 563m
			(4173, 4505, OutsideSegment, Brake),          // len: 332m
			(4505, 4723, OutsideSegment, Coast),          // len: 218m
			(4723, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(35, 4130, OutsideSegment, Accelerate),       // len: 4095m
			(4130, 5810, WithinSegment, Accelerate),      // len: 1680m
			(5810, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(35, 4655, OutsideSegment, Accelerate),       // len: 4620m
			(4655, 7257, WithinSegment, Accelerate),      // len: 2602m
			(7257, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(35, 3979, OutsideSegment, Accelerate),       // len: 3944m
			(3979, 5040, WithinSegment, Accelerate),      // len: 1061m
			(5040, 6289, UseCase1, Coast),                // len: 1249m
			(6289, 6568, WithinSegment, Coast),           // len: 279m
			(6568, 7000, WithinSegment, Brake),           // len: 432m
			(7000, 7149, WithinSegment, Coast),           // len: 149m
			(7149, 7462, OutsideSegment, Coast),          // len: 313m
			(7462, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(35, 654, OutsideSegment, Accelerate),        // len: 619m
			(654, 1995, WithinSegment, Accelerate),       // len: 1341m
			(1995, 2486, UseCase1, Coast),                // len: 491m
			(2486, 2497, OutsideSegment, Coast),          // len: 11m
			(2497, 4037, OutsideSegment, Accelerate),     // len: 1540m
			(4037, 5099, WithinSegment, Accelerate),      // len: 1062m
			(5099, 6692, UseCase1, Coast),                // len: 1593m
			(6692, 6716, OutsideSegment, Coast),          // len: 24m
			(6716, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(35, 689, OutsideSegment, Accelerate),        // len: 654m
			(689, 2182, WithinSegment, Accelerate),       // len: 1493m
			(2182, 2380, UseCase1, Coast),                // len: 198m
			(2380, 3103, WithinSegment, Accelerate),      // len: 723m
			(3103, 3979, UseCase1, Coast),                // len: 876m
			(3979, 4014, OutsideSegment, Coast),          // len: 35m
			(4014, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(35, 700, OutsideSegment, Accelerate),        // len: 665m
			(700, 2252, WithinSegment, Accelerate),       // len: 1552m
			(2252, 2369, WithinSegment, Coast),           // len: 117m
			(2369, 3010, WithinSegment, Accelerate),      // len: 641m
			(3010, 3197, WithinSegment, Coast),           // len: 187m
			(3197, 3267, WithinSegment, Accelerate),      // len: 70m
			(3267, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5134, WithinSegment, Accelerate),      // len: 1190m
			(5134, 5994, UseCase1, Coast),                // len: 860m
			(5994, 6559, WithinSegment, Coast),           // len: 565m
			(6559, 6999, OutsideSegment, Coast),          // len: 440m
			(6999, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(35, 3815, OutsideSegment, Accelerate),       // len: 3780m
			(3815, 5122, WithinSegment, Accelerate),      // len: 1307m
			(5122, 5995, UseCase1, Coast),                // len: 873m
			(5995, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 35, OutsideSegment, Roll),                // len: 35m
			(35, 3559, OutsideSegment, Accelerate),       // len: 3524m
			(3559, 4798, WithinSegment, Accelerate),      // len: 1239m
			(4798, 4813, WithinSegment, Roll),            // len: 15m
			(4813, 4840, WithinSegment, Accelerate),      // len: 27m
			(4840, 4852, WithinSegment, Roll),            // len: 12m
			(4852, 5069, WithinSegment, Accelerate),      // len: 217m
			(5069, 5085, WithinSegment, Roll),            // len: 16m
			(5085, 5274, WithinSegment, Accelerate),      // len: 189m
			(5274, 5657, UseCase2, Coast),                // len: 383m
			(5657, 6117, WithinSegment, Coast),           // len: 460m
			(6117, 6368, OutsideSegment, Coast),          // len: 251m
			(6368, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 4504, OutsideSegment, Accelerate),      // len: 4310m
			(4504, 4787, OutsideSegment, Coast),          // len: 283m
			(4787, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(194, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(35, 502, OutsideSegment, Accelerate),        // len: 467m
			(502, 525, OutsideSegment, Roll),             // len: 23m
			(525, 5612, OutsideSegment, Accelerate),      // len: 5087m
			(5612, 5916, OutsideSegment, Coast),          // len: 304m
			(5916, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(595, 6113, OutsideSegment, Accelerate),      // len: 5518m
			(6113, 6786, OutsideSegment, Coast),          // len: 673m
			(6786, 7255, OutsideSegment, Brake),          // len: 469m
			(7255, 7731, OutsideSegment, Coast),          // len: 476m
			(7731, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(234, 5460, OutsideSegment, Accelerate),      // len: 5226m
			(5460, 5802, OutsideSegment, Coast),          // len: 342m
			(5802, 7148, OutsideSegment, Brake),          // len: 1346m
			(7148, 7445, OutsideSegment, Coast),          // len: 297m
			(7445, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(549, 2147, OutsideSegment, Accelerate),      // len: 1598m
			(2147, 2300, OutsideSegment, Coast),          // len: 153m
			(2300, 2481, OutsideSegment, Brake),          // len: 181m
			(2481, 2599, OutsideSegment, Coast),          // len: 118m
			(2599, 5516, OutsideSegment, Accelerate),     // len: 2917m
			(5516, 5870, OutsideSegment, Coast),          // len: 354m
			(5870, 6687, OutsideSegment, Brake),          // len: 817m
			(6687, 6972, OutsideSegment, Coast),          // len: 285m
			(6972, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 2182, OutsideSegment, Accelerate),      // len: 1610m
			(2182, 2464, OutsideSegment, Coast),          // len: 282m
			(2464, 3327, OutsideSegment, Accelerate),     // len: 863m
			(3327, 3575, OutsideSegment, Coast),          // len: 248m
			(3575, 3971, OutsideSegment, Brake),          // len: 396m
			(3971, 4232, OutsideSegment, Coast),          // len: 261m
			(4232, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 2193, OutsideSegment, Accelerate),      // len: 1621m
			(2193, 2499, OutsideSegment, Coast),          // len: 306m
			(2499, 2884, OutsideSegment, Accelerate),     // len: 385m
			(2884, 3450, OutsideSegment, Coast),          // len: 566m
			(3450, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5437, OutsideSegment, Accelerate),        // len: 5437m
			(5437, 5603, OutsideSegment, Coast),          // len: 166m
			(5603, 6564, OutsideSegment, Brake),          // len: 961m
			(6564, 7075, OutsideSegment, Coast),          // len: 511m
			(7075, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(385, 5297, OutsideSegment, Accelerate),      // len: 4912m
			(5297, 5651, OutsideSegment, Coast),          // len: 354m
			(5651, 5987, OutsideSegment, Brake),          // len: 336m
			(5987, 6249, OutsideSegment, Coast),          // len: 262m
			(6249, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(537, 4200, OutsideSegment, Accelerate),      // len: 3663m
			(4200, 4223, OutsideSegment, Roll),           // len: 23m
			(4223, 4586, OutsideSegment, Accelerate),     // len: 363m
			(4586, 4605, OutsideSegment, Roll),           // len: 19m
			(4605, 4747, OutsideSegment, Accelerate),     // len: 142m
			(4747, 4762, OutsideSegment, Roll),           // len: 15m
			(4762, 4888, OutsideSegment, Accelerate),     // len: 126m
			(4888, 4900, OutsideSegment, Roll),           // len: 12m
			(4900, 4981, OutsideSegment, Accelerate),     // len: 81m
			(4981, 4994, OutsideSegment, Roll),           // len: 13m
			(4994, 5116, OutsideSegment, Accelerate),     // len: 122m
			(5116, 5133, OutsideSegment, Roll),           // len: 17m
			(5133, 5301, OutsideSegment, Accelerate),     // len: 168m
			(5301, 5323, OutsideSegment, Roll),           // len: 22m
			(5323, 5392, OutsideSegment, Accelerate),     // len: 69m
			(5392, 5475, OutsideSegment, Coast),          // len: 83m
			(5475, 6111, OutsideSegment, Brake),          // len: 636m
			(6111, 6397, OutsideSegment, Coast),          // len: 286m
			(6397, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3804, OutsideSegment, Accelerate),      // len: 3393m
			(3804, 4533, OutsideSegment, Coast),          // len: 729m
			(4533, 5006, OutsideSegment, Brake),          // len: 473m
			(5006, 5413, OutsideSegment, Coast),          // len: 407m
			(5413, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3609, OutsideSegment, Accelerate),      // len: 3198m
			(3609, 4202, OutsideSegment, Coast),          // len: 593m
			(4202, 4504, OutsideSegment, Brake),          // len: 302m
			(4504, 4722, OutsideSegment, Coast),          // len: 218m
			(4722, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 35, OutsideSegment, Roll),                // len: 35m
			(35, 502, OutsideSegment, Accelerate),        // len: 467m
			(502, 525, OutsideSegment, Roll),             // len: 23m
			(525, 5612, OutsideSegment, Accelerate),      // len: 5087m
			(5612, 5916, OutsideSegment, Coast),          // len: 304m
			(5916, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 35, OutsideSegment, Roll),                // len: 35m
			(35, 572, OutsideSegment, Accelerate),        // len: 537m
			(572, 595, OutsideSegment, Roll),             // len: 23m
			(595, 6113, OutsideSegment, Accelerate),      // len: 5518m
			(6113, 6786, OutsideSegment, Coast),          // len: 673m
			(6786, 7255, OutsideSegment, Brake),          // len: 469m
			(7255, 7731, OutsideSegment, Coast),          // len: 476m
			(7731, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 36, OutsideSegment, Accelerate),          // len: 36m
			(36, 59, OutsideSegment, Roll),               // len: 23m
			(59, 210, OutsideSegment, Accelerate),        // len: 151m
			(210, 234, OutsideSegment, Roll),             // len: 24m
			(234, 5460, OutsideSegment, Accelerate),      // len: 5226m
			(5460, 5802, OutsideSegment, Coast),          // len: 342m
			(5802, 7148, OutsideSegment, Brake),          // len: 1346m
			(7148, 7445, OutsideSegment, Coast),          // len: 297m
			(7445, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 35, OutsideSegment, Roll),                // len: 35m
			(35, 525, OutsideSegment, Accelerate),        // len: 490m
			(525, 549, OutsideSegment, Roll),             // len: 24m
			(549, 2147, OutsideSegment, Accelerate),      // len: 1598m
			(2147, 2300, OutsideSegment, Coast),          // len: 153m
			(2300, 2481, OutsideSegment, Brake),          // len: 181m
			(2481, 2599, OutsideSegment, Coast),          // len: 118m
			(2599, 5516, OutsideSegment, Accelerate),     // len: 2917m
			(5516, 5870, OutsideSegment, Coast),          // len: 354m
			(5870, 6687, OutsideSegment, Brake),          // len: 817m
			(6687, 6972, OutsideSegment, Coast),          // len: 285m
			(6972, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 35, OutsideSegment, Roll),                // len: 35m
			(35, 549, OutsideSegment, Accelerate),        // len: 514m
			(549, 572, OutsideSegment, Roll),             // len: 23m
			(572, 2182, OutsideSegment, Accelerate),      // len: 1610m
			(2182, 2464, OutsideSegment, Coast),          // len: 282m
			(2464, 3327, OutsideSegment, Accelerate),     // len: 863m
			(3327, 3575, OutsideSegment, Coast),          // len: 248m
			(3575, 3971, OutsideSegment, Brake),          // len: 396m
			(3971, 4232, OutsideSegment, Coast),          // len: 261m
			(4232, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 35, OutsideSegment, Roll),                // len: 35m
			(35, 549, OutsideSegment, Accelerate),        // len: 514m
			(549, 572, OutsideSegment, Roll),             // len: 23m
			(572, 2193, OutsideSegment, Accelerate),      // len: 1621m
			(2193, 2499, OutsideSegment, Coast),          // len: 306m
			(2499, 2884, OutsideSegment, Accelerate),     // len: 385m
			(2884, 3450, OutsideSegment, Coast),          // len: 566m
			(3450, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5437, OutsideSegment, Accelerate),        // len: 5437m
			(5437, 5603, OutsideSegment, Coast),          // len: 166m
			(5603, 6564, OutsideSegment, Brake),          // len: 961m
			(6564, 7075, OutsideSegment, Coast),          // len: 511m
			(7075, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 35, OutsideSegment, Roll),                // len: 35m
			(35, 362, OutsideSegment, Accelerate),        // len: 327m
			(362, 385, OutsideSegment, Roll),             // len: 23m
			(385, 5297, OutsideSegment, Accelerate),      // len: 4912m
			(5297, 5651, OutsideSegment, Coast),          // len: 354m
			(5651, 5987, OutsideSegment, Brake),          // len: 336m
			(5987, 6249, OutsideSegment, Coast),          // len: 262m
			(6249, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(537, 4200, OutsideSegment, Accelerate),      // len: 3663m
			(4200, 4223, OutsideSegment, Roll),           // len: 23m
			(4223, 4586, OutsideSegment, Accelerate),     // len: 363m
			(4586, 4605, OutsideSegment, Roll),           // len: 19m
			(4605, 4747, OutsideSegment, Accelerate),     // len: 142m
			(4747, 4762, OutsideSegment, Roll),           // len: 15m
			(4762, 4888, OutsideSegment, Accelerate),     // len: 126m
			(4888, 4900, OutsideSegment, Roll),           // len: 12m
			(4900, 4981, OutsideSegment, Accelerate),     // len: 81m
			(4981, 4994, OutsideSegment, Roll),           // len: 13m
			(4994, 5116, OutsideSegment, Accelerate),     // len: 122m
			(5116, 5133, OutsideSegment, Roll),           // len: 17m
			(5133, 5301, OutsideSegment, Accelerate),     // len: 168m
			(5301, 5323, OutsideSegment, Roll),           // len: 22m
			(5323, 5392, OutsideSegment, Accelerate),     // len: 69m
			(5392, 5475, OutsideSegment, Coast),          // len: 83m
			(5475, 6111, OutsideSegment, Brake),          // len: 636m
			(6111, 6397, OutsideSegment, Coast),          // len: 286m
			(6397, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3804, OutsideSegment, Accelerate),      // len: 3393m
			(3804, 4533, OutsideSegment, Coast),          // len: 729m
			(4533, 5006, OutsideSegment, Brake),          // len: 473m
			(5006, 5413, OutsideSegment, Coast),          // len: 407m
			(5413, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3609, OutsideSegment, Accelerate),      // len: 3198m
			(3609, 4202, OutsideSegment, Coast),          // len: 593m
			(4202, 4504, OutsideSegment, Brake),          // len: 302m
			(4504, 4722, OutsideSegment, Coast),          // len: 218m
			(4722, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(525, 4130, OutsideSegment, Accelerate),      // len: 3605m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5808, UseCase1, Coast),                // len: 336m
			(5808, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(595, 4655, OutsideSegment, Accelerate),      // len: 4060m
			(4655, 5495, WithinSegment, Accelerate),      // len: 840m
			(5495, 7251, UseCase1, Coast),                // len: 1756m
			(7251, 7486, OutsideSegment, Coast),          // len: 235m
			(7486, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(234, 3979, OutsideSegment, Accelerate),      // len: 3745m
			(3979, 4935, WithinSegment, Accelerate),      // len: 956m
			(4935, 6104, UseCase1, Coast),                // len: 1169m
			(6104, 6298, WithinSegment, Coast),           // len: 194m
			(6298, 7138, WithinSegment, Brake),           // len: 840m
			(7138, 7151, WithinSegment, Coast),           // len: 13m
			(7151, 7563, OutsideSegment, Coast),          // len: 412m
			(7563, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(549, 654, OutsideSegment, Accelerate),       // len: 105m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2487, UseCase1, Coast),                // len: 585m
			(2487, 2510, OutsideSegment, Coast),          // len: 23m
			(2510, 4038, OutsideSegment, Accelerate),     // len: 1528m
			(4038, 4960, WithinSegment, Accelerate),      // len: 922m
			(4960, 6210, UseCase1, Coast),                // len: 1250m
			(6210, 6636, WithinSegment, Coast),           // len: 426m
			(6636, 6673, WithinSegment, Brake),           // len: 37m
			(6673, 6685, WithinSegment, Coast),           // len: 12m
			(6685, 7085, OutsideSegment, Coast),          // len: 400m
			(7085, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 688, OutsideSegment, Accelerate),       // len: 116m
			(688, 2088, WithinSegment, Accelerate),       // len: 1400m
			(2088, 2379, UseCase1, Coast),                // len: 291m
			(2379, 2402, WithinSegment, Coast),           // len: 23m
			(2402, 2997, WithinSegment, Accelerate),      // len: 595m
			(2997, 3876, UseCase1, Coast),                // len: 879m
			(3876, 3970, WithinSegment, Coast),           // len: 94m
			(3970, 4171, OutsideSegment, Coast),          // len: 201m
			(4171, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 700, OutsideSegment, Accelerate),       // len: 128m
			(700, 2088, WithinSegment, Accelerate),       // len: 1388m
			(2088, 2390, UseCase1, Coast),                // len: 302m
			(2390, 2646, WithinSegment, Accelerate),      // len: 256m
			(2646, 3267, UseCase1, Coast),                // len: 621m
			(3267, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5901, UseCase1, Coast),                // len: 802m
			(5901, 6339, WithinSegment, Coast),           // len: 438m
			(6339, 6537, WithinSegment, Brake),           // len: 198m
			(6537, 6561, WithinSegment, Coast),           // len: 24m
			(6561, 7288, OutsideSegment, Coast),          // len: 727m
			(7288, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5987, UseCase1, Coast),                // len: 1133m
			(5987, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(537, 3558, OutsideSegment, Accelerate),      // len: 3021m
			(3558, 4200, WithinSegment, Accelerate),      // len: 642m
			(4200, 4223, WithinSegment, Roll),            // len: 23m
			(4223, 4586, WithinSegment, Accelerate),      // len: 363m
			(4586, 4605, WithinSegment, Roll),            // len: 19m
			(4605, 4747, WithinSegment, Accelerate),      // len: 142m
			(4747, 4762, WithinSegment, Roll),            // len: 15m
			(4762, 4888, WithinSegment, Accelerate),      // len: 126m
			(4888, 4900, WithinSegment, Roll),            // len: 12m
			(4900, 4981, WithinSegment, Accelerate),      // len: 81m
			(4981, 4994, WithinSegment, Roll),            // len: 13m
			(4994, 5116, WithinSegment, Accelerate),      // len: 122m
			(5116, 5133, WithinSegment, Roll),            // len: 17m
			(5133, 5280, WithinSegment, Accelerate),      // len: 147m
			(5280, 5410, UseCase2, Coast),                // len: 130m
			(5410, 5421, UseCase2, Accelerate),           // len: 11m
			(5421, 5433, UseCase2, Roll),                 // len: 12m
			(5433, 5629, UseCase2, Coast),                // len: 196m
			(5629, 5969, WithinSegment, Coast),           // len: 340m
			(5969, 6043, WithinSegment, Brake),           // len: 74m
			(6043, 6117, WithinSegment, Coast),           // len: 74m
			(6117, 6456, OutsideSegment, Coast),          // len: 339m
			(6456, 6747, OutsideSegment, Accelerate),     // len: 291m
			(6747, 6771, OutsideSegment, Roll),           // len: 24m
			(6771, 1e6, OutsideSegment, Accelerate));


        [TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3804, OutsideSegment, Accelerate),      // len: 3393m
			(3804, 4533, OutsideSegment, Coast),          // len: 729m
			(4533, 5006, OutsideSegment, Brake),          // len: 473m
			(5006, 5413, OutsideSegment, Coast),          // len: 407m
			(5413, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3609, OutsideSegment, Accelerate),      // len: 3198m
			(3609, 4202, OutsideSegment, Coast),          // len: 593m
			(4202, 4504, OutsideSegment, Brake),          // len: 302m
			(4504, 4722, OutsideSegment, Coast),          // len: 218m
			(4722, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(525, 4130, OutsideSegment, Accelerate),      // len: 3605m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5808, UseCase1, Coast),                // len: 336m
			(5808, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(595, 4655, OutsideSegment, Accelerate),      // len: 4060m
			(4655, 5495, WithinSegment, Accelerate),      // len: 840m
			(5495, 7251, UseCase1, Coast),                // len: 1756m
			(7251, 7486, OutsideSegment, Coast),          // len: 235m
			(7486, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(234, 3979, OutsideSegment, Accelerate),      // len: 3745m
			(3979, 4935, WithinSegment, Accelerate),      // len: 956m
			(4935, 6104, UseCase1, Coast),                // len: 1169m
			(6104, 6298, WithinSegment, Coast),           // len: 194m
			(6298, 7138, WithinSegment, Brake),           // len: 840m
			(7138, 7151, WithinSegment, Coast),           // len: 13m
			(7151, 7563, OutsideSegment, Coast),          // len: 412m
			(7563, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(549, 654, OutsideSegment, Accelerate),       // len: 105m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2487, UseCase1, Coast),                // len: 585m
			(2487, 2510, OutsideSegment, Coast),          // len: 23m
			(2510, 4038, OutsideSegment, Accelerate),     // len: 1528m
			(4038, 4960, WithinSegment, Accelerate),      // len: 922m
			(4960, 6210, UseCase1, Coast),                // len: 1250m
			(6210, 6636, WithinSegment, Coast),           // len: 426m
			(6636, 6673, WithinSegment, Brake),           // len: 37m
			(6673, 6685, WithinSegment, Coast),           // len: 12m
			(6685, 7085, OutsideSegment, Coast),          // len: 400m
			(7085, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 688, OutsideSegment, Accelerate),       // len: 116m
			(688, 2088, WithinSegment, Accelerate),       // len: 1400m
			(2088, 2379, UseCase1, Coast),                // len: 291m
			(2379, 2402, WithinSegment, Coast),           // len: 23m
			(2402, 2997, WithinSegment, Accelerate),      // len: 595m
			(2997, 3876, UseCase1, Coast),                // len: 879m
			(3876, 3970, WithinSegment, Coast),           // len: 94m
			(3970, 4171, OutsideSegment, Coast),          // len: 201m
			(4171, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(572, 700, OutsideSegment, Accelerate),       // len: 128m
			(700, 2088, WithinSegment, Accelerate),       // len: 1388m
			(2088, 2390, UseCase1, Coast),                // len: 302m
			(2390, 2646, WithinSegment, Accelerate),      // len: 256m
			(2646, 3267, UseCase1, Coast),                // len: 621m
			(3267, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseG_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5901, UseCase1, Coast),                // len: 802m
			(5901, 6339, WithinSegment, Coast),           // len: 438m
			(6339, 6537, WithinSegment, Brake),           // len: 198m
			(6537, 6561, WithinSegment, Coast),           // len: 24m
			(6561, 7288, OutsideSegment, Coast),          // len: 727m
			(7288, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseH_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(385, 3815, OutsideSegment, Accelerate),      // len: 3430m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5987, UseCase1, Coast),                // len: 1133m
			(5987, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(537, 3558, OutsideSegment, Accelerate),      // len: 3021m
			(3558, 4200, WithinSegment, Accelerate),      // len: 642m
			(4200, 4223, WithinSegment, Roll),            // len: 23m
			(4223, 4586, WithinSegment, Accelerate),      // len: 363m
			(4586, 4605, WithinSegment, Roll),            // len: 19m
			(4605, 4747, WithinSegment, Accelerate),      // len: 142m
			(4747, 4762, WithinSegment, Roll),            // len: 15m
			(4762, 4888, WithinSegment, Accelerate),      // len: 126m
			(4888, 4900, WithinSegment, Roll),            // len: 12m
			(4900, 4981, WithinSegment, Accelerate),      // len: 81m
			(4981, 4994, WithinSegment, Roll),            // len: 13m
			(4994, 5116, WithinSegment, Accelerate),      // len: 122m
			(5116, 5133, WithinSegment, Roll),            // len: 17m
			(5133, 5280, WithinSegment, Accelerate),      // len: 147m
			(5280, 5410, UseCase2, Coast),                // len: 130m
			(5410, 5421, UseCase2, Accelerate),           // len: 11m
			(5421, 5433, UseCase2, Roll),                 // len: 12m
			(5433, 5629, UseCase2, Coast),                // len: 196m
			(5629, 5969, WithinSegment, Coast),           // len: 340m
			(5969, 6043, WithinSegment, Brake),           // len: 74m
			(6043, 6117, WithinSegment, Coast),           // len: 74m
			(6117, 6456, OutsideSegment, Coast),          // len: 339m
			(6456, 6747, OutsideSegment, Accelerate),     // len: 291m
			(6747, 6771, OutsideSegment, Roll),           // len: 24m
			(6771, 1e6, OutsideSegment, Accelerate));

        [TestCase]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3804, OutsideSegment, Accelerate),      // len: 3393m
			(3804, 4533, OutsideSegment, Coast),          // len: 729m
			(4533, 5006, OutsideSegment, Brake),          // len: 473m
			(5006, 5413, OutsideSegment, Coast),          // len: 407m
			(5413, 1e6, OutsideSegment, Accelerate));




		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3609, OutsideSegment, Accelerate),      // len: 3198m
			(3609, 4202, OutsideSegment, Coast),          // len: 593m
			(4202, 4504, OutsideSegment, Brake),          // len: 302m
			(4504, 4722, OutsideSegment, Coast),          // len: 218m
			(4722, 1e6, OutsideSegment, Accelerate));



		private void TestPCC(string testname,
			params (double start, double end, PCCStates pcc, DrivingAction action)[] data)
		{
			var jobName = testname.Split('_').Slice(0, -2).Join("_");
			var cycleName = testname.Split('_').Reverse().Skip(1).First();
			DoTestPCC(jobName, cycleName, data);
		}

		private void DoTestPCC(string jobName, string cycleName,
			params (double start, double end, PCCStates pcc, DrivingAction action)[] data)
		{
			jobName = Path.Combine(BasePath, jobName + ".vecto");

			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));
			var sumContainer = new SummaryDataContainer(writer);
			var factory = _kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Engineering, inputData, writer, null, null, false);
            factory.WriteModalResults = true;
			factory.Validate = false;
			factory.SumData = sumContainer;

			var run = factory.SimulationRuns().First(r => r.CycleName == cycleName);
			var mod = (run.GetContainer().ModalData as ModalDataContainer).Data;
			run.Run();
			PrintPCCSections(mod);
			Assert.IsTrue(run.FinishedWithoutErrors);

			var expected = data;

			var segmentWasTested = false;

			var distances = mod.Columns[ModalResultField.dist.GetName()].Values<Meter>();
			var pccStates = mod.Columns["PCCState"].Values<PCCStates>();
			var actions = mod.Columns["DriverAction"].Values<DrivingAction>();
			var vActs = mod.Columns[ModalResultField.v_act.GetName()].Values<MeterPerSecond>();

			using (var exp = expected.AsEnumerable().GetEnumerator()) {
				exp.MoveNext();
				foreach (var (dist, pcc, action, vAct) in distances.Zip(pccStates, actions, vActs)) {
					if (dist > exp.Current.end) {
						Assert.IsTrue(segmentWasTested, $"dist {dist}: Expected Segment was not tested. Maybe distance range to narrow?");
						if (!exp.MoveNext())
							break;
						segmentWasTested = false;
					}

					if (dist.IsBetween(exp.Current.start, exp.Current.end)) {
						// if the segment is very short, at least one of the entries should have the expected values
						if (exp.Current.pcc == pcc && exp.Current.action == action)
							segmentWasTested = true;
					}

					if (dist.IsBetween(exp.Current.start.SI<Meter>() + vAct * tolerance.SI<Second>(), exp.Current.end.SI<Meter>() - vAct * tolerance.SI<Second>())) {
						Assert.AreEqual(exp.Current.pcc, pcc, $"dist {dist}: Wrong PCC state: {pcc} instead of {exp.Current.pcc}.");
						Assert.AreEqual(exp.Current.action, action, $"dist {dist}: Wrong DriverAction: {action} instead of {exp.Current.action}.");
						segmentWasTested = true;
					}
				}
			}

			Assert.IsTrue(segmentWasTested);
		}


		private void PrintPCCSections(ModalResults mod)
		{
			var sCol = mod.Columns[ModalResultField.dist.GetName()];
			var pccCol = mod.Columns["PCCState"];
			var driverActionCol = mod.Columns["DriverAction"];

			var pccStates = pccCol.Values<PCCStates>();
			var driverAction = driverActionCol.Values<DrivingAction>();
			var distances = sCol.Values<Meter>();
			var sections = GetDistancesOfStateChanges(pccStates.ZipAll(driverAction), distances).ToArray();

			Console.WriteLine("Start-End Segments:");
			if (sections.Any()) {
				var start = 0d;
				foreach (var section in sections) {
					Console.WriteLine($"{$"({start}, {(int)section.Distance.Value()}, {section.Before.Item1}, {section.Before.Item2}),",-45} // len: {(int)section.Distance.Value() - start}m");
					start = (int)section.Distance.Value();
				}
				Console.WriteLine($"({(int)sections.Last().Distance.Value()}, 1e6, {sections.Last().After.Item1}, {sections.Last().After.Item2}));");
			} else {
				Console.WriteLine("(0, 1e6, OutsideSegment, Accelerate));");
			}
		}

		IEnumerable<(T2 Distance, T1 Before, T1 After, T2[] SegmentValues)> GetDistancesOfStateChanges<T1, T2>(IEnumerable<T1> states, IEnumerable<T2> distances)
		{
			using (var values = states.GetEnumerator()) {
				using (var distance = distances.GetEnumerator()) {
					distance.MoveNext();
					values.MoveNext();
					var value = values.Current;
					var segmentValues = new List<T2> { distance.Current };
					while (values.MoveNext() | distance.MoveNext()) {
						if (!value.Equals(values.Current)) {
							yield return (distance.Current, value, values.Current, segmentValues.ToArray());
							segmentValues.Clear();
							value = values.Current;
							segmentValues.Add(distance.Current);
						}
					}
				}
			}
		}
	}
}
