using System.IO;
using System.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.PCCStates;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.DrivingAction;

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
		private const string BasePath = @"TestData\Integration\ADAS-HEV\Group5PCCEng\";
		private const double tolerance = 1; //seconds of tolerance. Tolerance distance is calculated dynamically based on speed.

		[OneTimeSetUp]
		public void RunBeforeAnyTests() => Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

		private GraphWriter GetGraphWriter()
		{
			var graphWriter = new GraphWriter();
			//#if TRACE
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

		[TestCase(@"TestData\Integration\ADAS-HEV\VECTO-1493\P1_CityBus.vecto", TestName = "VECTO-1493_P1Citybus")]
		[TestCase(@"TestData\Integration\ADAS-HEV\VECTO-1484\P2_Group5_s2c0_rep_Payload.vecto", TestName = "VECTO-1484_P2Group5")]
		public static void RunEngineeringJob(string jobName)
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));
			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
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
			var jobName = @"TestData\Integration\ADAS-HEV\Group5EcoRollEng\Class5_Tractor_ENG.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
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
			var jobName = @"TestData\Integration\ADAS-HEV\Group9_RigidTruck_AT\Class_9_RigidTruck_AT_Eng_Neutral.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
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
			var jobName = @"TestData\Integration\ADAS-HEV\Group9_RigidTruck_AT\Class_9_RigidTruck_AT_Eng_TC.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
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
			(500, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 7252, UseCase1, Coast),                // len: 1756m
			(7252, 7486, OutsideSegment, Coast),          // len: 234m
			(7486, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 6104, UseCase1, Coast),                // len: 1168m
			(6104, 6298, WithinSegment, Coast),           // len: 194m
			(6298, 7139, WithinSegment, Brake),           // len: 841m
			(7139, 7151, WithinSegment, Coast),           // len: 12m
			(7151, 7563, OutsideSegment, Coast),          // len: 412m
			(7563, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2487, UseCase1, Coast),                // len: 585m
			(2487, 2511, OutsideSegment, Coast),          // len: 24m
			(2511, 4039, OutsideSegment, Accelerate),     // len: 1528m
			(4039, 4961, WithinSegment, Accelerate),      // len: 922m
			(4961, 6211, UseCase1, Coast),                // len: 1250m
			(6211, 6624, WithinSegment, Coast),           // len: 413m
			(6624, 6673, WithinSegment, Brake),           // len: 49m
			(6673, 6686, WithinSegment, Coast),           // len: 13m
			(6686, 7086, OutsideSegment, Coast),          // len: 400m
			(7086, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2378, UseCase1, Coast),                // len: 301m
			(2378, 2996, WithinSegment, Accelerate),      // len: 618m
			(2996, 3875, UseCase1, Coast),                // len: 879m
			(3875, 3970, WithinSegment, Coast),           // len: 95m
			(3970, 4170, OutsideSegment, Coast),          // len: 200m
			(4170, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2390, UseCase1, Coast),                // len: 301m
			(2390, 2647, WithinSegment, Accelerate),      // len: 257m
			(2647, 3267, UseCase1, Coast),                // len: 620m
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
			(500, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4563, WithinSegment, Accelerate),      // len: 1004m
			(5146, 5342, WithinSegment, Accelerate),      // len: 196m
			(5342, 5669, UseCase2, Coast),                // len: 327m
			(5669, 5923, WithinSegment, Coast),           // len: 254m
			(5923, 6109, WithinSegment, Brake),           // len: 186m
			(6109, 6121, WithinSegment, Coast),           // len: 12m
			(6121, 6509, OutsideSegment, Coast),          // len: 388m
			(6509, 1e6, OutsideSegment, Accelerate));

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
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5809, UseCase1, Coast),                // len: 337m
			(5809, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 7252, UseCase1, Coast),                // len: 1756m
			(7252, 7486, OutsideSegment, Coast),          // len: 234m
			(7486, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC12_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 6104, UseCase1, Coast),                // len: 1168m
			(6104, 6164, WithinSegment, Coast),           // len: 60m
			(6164, 7149, WithinSegment, Brake),           // len: 985m
			(7149, 7447, OutsideSegment, Coast),          // len: 298m
			(7447, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2487, UseCase1, Coast),                // len: 585m
			(2487, 2511, OutsideSegment, Coast),          // len: 24m
			(2511, 4039, OutsideSegment, Accelerate),     // len: 1528m
			(4039, 4961, WithinSegment, Accelerate),      // len: 922m
			(4961, 6211, UseCase1, Coast),                // len: 1250m
			(6211, 6306, WithinSegment, Coast),           // len: 95m
			(6306, 6690, WithinSegment, Brake),           // len: 384m
			(6690, 6976, OutsideSegment, Coast),          // len: 286m
			(6976, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC12_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2378, UseCase1, Coast),                // len: 301m
			(2378, 2996, WithinSegment, Accelerate),      // len: 618m
			(2996, 3875, UseCase1, Coast),                // len: 879m
			(3875, 3970, WithinSegment, Coast),           // len: 95m
			(3970, 4170, OutsideSegment, Coast),          // len: 200m
			(4170, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2390, UseCase1, Coast),                // len: 301m
			(2390, 2647, WithinSegment, Accelerate),      // len: 257m
			(2647, 3267, UseCase1, Coast),                // len: 620m
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
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4563, WithinSegment, Accelerate),      // len: 1004m
			(4563, 4582, WithinSegment, Roll),            // len: 19m
			(4582, 4709, WithinSegment, Accelerate),      // len: 127m
			(4709, 4723, WithinSegment, Roll),            // len: 14m
			(4723, 4789, WithinSegment, Accelerate),      // len: 66m
			(4789, 4800, WithinSegment, Roll),            // len: 11m
			(4800, 4861, WithinSegment, Accelerate),      // len: 61m
			(4861, 4870, WithinSegment, Roll),            // len: 9m
			(4870, 4931, WithinSegment, Accelerate),      // len: 61m
			(4931, 4940, WithinSegment, Roll),            // len: 9m
			(4940, 5028, WithinSegment, Accelerate),      // len: 88m
			(5028, 5040, WithinSegment, Roll),            // len: 12m
			(5040, 5079, WithinSegment, Accelerate),      // len: 39m
			(5079, 5093, WithinSegment, Roll),            // len: 14m
			(5093, 5131, WithinSegment, Accelerate),      // len: 38m
			(5131, 5147, WithinSegment, Roll),            // len: 16m
			(5147, 5344, WithinSegment, Accelerate),      // len: 197m
			(5344, 5671, UseCase2, Coast),                // len: 327m
			(5671, 5730, WithinSegment, Coast),           // len: 59m
			(5730, 6115, WithinSegment, Brake),           // len: 385m
			(6115, 6400, OutsideSegment, Coast),          // len: 285m
			(6400, 1e6, OutsideSegment, Accelerate));


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
			(0, 5612, OutsideSegment, Accelerate),        // len: 5612m
			(5612, 5916, OutsideSegment, Coast),          // len: 304m
			(5916, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 6114, OutsideSegment, Accelerate),        // len: 6114m
			(6114, 6787, OutsideSegment, Coast),          // len: 673m
			(6787, 7255, OutsideSegment, Brake),          // len: 468m
			(7255, 7731, OutsideSegment, Coast),          // len: 476m
			(7731, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5461, OutsideSegment, Accelerate),        // len: 5461m
			(5461, 5802, OutsideSegment, Coast),          // len: 341m
			(5802, 7148, OutsideSegment, Brake),          // len: 1346m
			(7148, 7446, OutsideSegment, Coast),          // len: 298m
			(7446, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2147, OutsideSegment, Accelerate),        // len: 2147m
			(2147, 2301, OutsideSegment, Coast),          // len: 154m
			(2301, 2481, OutsideSegment, Brake),          // len: 180m
			(2481, 2600, OutsideSegment, Coast),          // len: 119m
			(2600, 5516, OutsideSegment, Accelerate),     // len: 2916m
			(5516, 5870, OutsideSegment, Coast),          // len: 354m
			(5870, 6687, OutsideSegment, Brake),          // len: 817m
			(6687, 6973, OutsideSegment, Coast),          // len: 286m
			(6973, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2182, OutsideSegment, Accelerate),        // len: 2182m
			(2182, 2464, OutsideSegment, Coast),          // len: 282m
			(2464, 3327, OutsideSegment, Accelerate),     // len: 863m
			(3327, 3575, OutsideSegment, Coast),          // len: 248m
			(3575, 3971, OutsideSegment, Brake),          // len: 396m
			(3971, 4233, OutsideSegment, Coast),          // len: 262m
			(4233, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2194, OutsideSegment, Accelerate),        // len: 2194m
			(2194, 2499, OutsideSegment, Coast),          // len: 305m
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
			(0, 5297, OutsideSegment, Accelerate),        // len: 5297m
			(5297, 5651, OutsideSegment, Coast),          // len: 354m
			(5651, 5987, OutsideSegment, Brake),          // len: 336m
			(5987, 6249, OutsideSegment, Coast),          // len: 262m
			(6249, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

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
			(1902, 2487, UseCase1, Coast),                // len: 585m
			(2487, 2511, OutsideSegment, Coast),          // len: 24m
			(2511, 4039, OutsideSegment, Accelerate),     // len: 1528m
			(4039, 4961, WithinSegment, Accelerate),      // len: 922m
			(4961, 6211, UseCase1, Coast),                // len: 1250m
			(6211, 6624, WithinSegment, Coast),           // len: 413m
			(6624, 6673, WithinSegment, Brake),           // len: 49m
			(6673, 6686, WithinSegment, Coast),           // len: 13m
			(6686, 7086, OutsideSegment, Coast),          // len: 400m
			(7086, 1e6, OutsideSegment, Accelerate));

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
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5099, WithinSegment, Accelerate),      // len: 1155m
			(5099, 5901, UseCase1, Coast),                // len: 802m
			(5901, 6339, WithinSegment, Coast),           // len: 438m
			(6339, 6537, WithinSegment, Brake),           // len: 198m
			(6537, 6561, WithinSegment, Coast),           // len: 24m
			(6561, 7288, OutsideSegment, Coast),          // len: 727m
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
			(3559, 4563, WithinSegment, Accelerate),      // len: 1004m
			(4563, 4582, WithinSegment, Roll),            // len: 19m
			(4582, 4700, WithinSegment, Accelerate),      // len: 118m
			(4700, 4715, WithinSegment, Roll),            // len: 15m
			(4715, 4774, WithinSegment, Accelerate),      // len: 59m
			(4774, 4785, WithinSegment, Roll),            // len: 11m
			(4785, 4826, WithinSegment, Accelerate),      // len: 41m
			(4826, 4834, WithinSegment, Roll),            // len: 8m
			(4834, 4873, WithinSegment, Accelerate),      // len: 39m
			(4873, 4879, WithinSegment, Roll),            // len: 6m
			(4879, 4923, WithinSegment, Accelerate),      // len: 44m
			(4923, 4929, WithinSegment, Roll),            // len: 6m
			(4929, 5005, WithinSegment, Accelerate),      // len: 76m
			(5005, 5014, WithinSegment, Roll),            // len: 9m
			(5014, 5056, WithinSegment, Accelerate),      // len: 42m
			(5056, 5068, WithinSegment, Roll),            // len: 12m
			(5068, 5100, WithinSegment, Accelerate),      // len: 32m
			(5100, 5114, WithinSegment, Roll),            // len: 14m
			(5114, 5151, WithinSegment, Accelerate),      // len: 37m
			(5151, 5167, WithinSegment, Roll),            // len: 16m
			(5167, 5371, WithinSegment, Accelerate),      // len: 204m
			(5371, 5708, UseCase2, Coast),                // len: 337m
			(5708, 6123, WithinSegment, Coast),           // len: 415m
			(6123, 6510, OutsideSegment, Coast),          // len: 387m
			(6510, 1e6, OutsideSegment, Accelerate));

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
			(5184, 5397, WithinSegment, Accelerate),      // len: 213m
			(5397, 5736, UseCase2, Coast),                // len: 339m
			(5736, 6126, WithinSegment, Coast),           // len: 390m
			(6126, 6501, OutsideSegment, Coast),          // len: 375m
			(6501, 1e6, OutsideSegment, Accelerate));

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
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5612, WithinSegment, Accelerate),      // len: 1482m
			(5612, 5810, UseCase1, Coast),                // len: 198m
			(5810, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5682, WithinSegment, Accelerate),      // len: 1027m
			(5682, 7253, UseCase1, Coast),                // len: 1571m
			(7253, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4994, WithinSegment, Accelerate),      // len: 1015m
			(4994, 6176, UseCase1, Coast),                // len: 1182m
			(6176, 6394, WithinSegment, Coast),           // len: 218m
			(6394, 7086, WithinSegment, Brake),           // len: 692m
			(7086, 7160, WithinSegment, Coast),           // len: 74m
			(7160, 7523, OutsideSegment, Coast),          // len: 363m
			(7523, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1949, WithinSegment, Accelerate),       // len: 1295m
			(1949, 2481, UseCase1, Coast),                // len: 532m
			(2481, 2504, OutsideSegment, Coast),          // len: 23m
			(2504, 4044, OutsideSegment, Accelerate),     // len: 1540m
			(4044, 5012, WithinSegment, Accelerate),      // len: 968m
			(5012, 6353, UseCase1, Coast),                // len: 1341m
			(6353, 6688, WithinSegment, Coast),           // len: 335m
			(6688, 6938, OutsideSegment, Coast),          // len: 250m
			(6938, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2112, WithinSegment, Accelerate),       // len: 1423m
			(2112, 2379, UseCase1, Coast),                // len: 267m
			(2379, 3032, WithinSegment, Accelerate),      // len: 653m
			(3032, 3974, UseCase1, Coast),                // len: 942m
			(3974, 4056, OutsideSegment, Coast),          // len: 82m
			(4056, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2124, WithinSegment, Accelerate),       // len: 1424m
			(2124, 2391, UseCase1, Coast),                // len: 267m
			(2391, 2741, WithinSegment, Accelerate),      // len: 350m
			(2741, 3273, UseCase1, Coast),                // len: 532m
			(3273, 1e6, OutsideSegment, Accelerate));

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
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4982, WithinSegment, Accelerate),      // len: 1167m
			(4982, 5992, UseCase1, Coast),                // len: 1010m
			(5992, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));
		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4519, WithinSegment, Accelerate),      // len: 960m
			(4519, 4538, WithinSegment, Roll),            // len: 19m
			(4538, 4735, WithinSegment, Accelerate),      // len: 197m
			(4735, 4748, WithinSegment, Roll),            // len: 13m
			(4748, 4780, WithinSegment, Accelerate),      // len: 32m
			(4780, 4791, WithinSegment, Roll),            // len: 11m
			(4791, 4994, WithinSegment, Accelerate),      // len: 203m
			(4994, 5006, WithinSegment, Roll),            // len: 12m
			(5006, 5037, WithinSegment, Accelerate),      // len: 31m
			(5037, 5050, WithinSegment, Roll),            // len: 13m
			(5050, 5085, WithinSegment, Accelerate),      // len: 35m
			(5085, 5100, WithinSegment, Roll),            // len: 15m
			(5100, 5329, WithinSegment, Accelerate),      // len: 229m
			(5329, 5679, UseCase2, Coast),                // len: 350m
			(5679, 6117, WithinSegment, Coast),           // len: 438m
			(6117, 6455, OutsideSegment, Coast),          // len: 338m
			(6455, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast1_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 4012, OutsideSegment, Accelerate),      // len: 3636m
			(4012, 5192, OutsideSegment, Coast),          // len: 1180m
			(5192, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_P25_PCC123EcoRollEngineStop_CrestCoast2_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 3711, OutsideSegment, Accelerate),      // len: 3335m
			(3711, 4636, OutsideSegment, Coast),          // len: 925m
			(4636, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseA_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5612, OutsideSegment, Accelerate),        // len: 5612m
			(5612, 5916, OutsideSegment, Coast),          // len: 304m
			(5916, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 6114, OutsideSegment, Accelerate),        // len: 6114m
			(6114, 6787, OutsideSegment, Coast),          // len: 673m
			(6787, 7255, OutsideSegment, Brake),          // len: 468m
			(7255, 7731, OutsideSegment, Coast),          // len: 476m
			(7731, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5461, OutsideSegment, Accelerate),        // len: 5461m
			(5461, 5802, OutsideSegment, Coast),          // len: 341m
			(5802, 7148, OutsideSegment, Brake),          // len: 1346m
			(7148, 7446, OutsideSegment, Coast),          // len: 298m
			(7446, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2147, OutsideSegment, Accelerate),        // len: 2147m
			(2147, 2301, OutsideSegment, Coast),          // len: 154m
			(2301, 2481, OutsideSegment, Brake),          // len: 180m
			(2481, 2600, OutsideSegment, Coast),          // len: 119m
			(2600, 5516, OutsideSegment, Accelerate),     // len: 2916m
			(5516, 5870, OutsideSegment, Coast),          // len: 354m
			(5870, 6687, OutsideSegment, Brake),          // len: 817m
			(6687, 6973, OutsideSegment, Coast),          // len: 286m
			(6973, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2182, OutsideSegment, Accelerate),        // len: 2182m
			(2182, 2464, OutsideSegment, Coast),          // len: 282m
			(2464, 3327, OutsideSegment, Accelerate),     // len: 863m
			(3327, 3575, OutsideSegment, Coast),          // len: 248m
			(3575, 3971, OutsideSegment, Brake),          // len: 396m
			(3971, 4233, OutsideSegment, Coast),          // len: 262m
			(4233, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2194, OutsideSegment, Accelerate),        // len: 2194m
			(2194, 2499, OutsideSegment, Coast),          // len: 305m
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
			(0, 5297, OutsideSegment, Accelerate),        // len: 5297m
			(5297, 5651, OutsideSegment, Coast),          // len: 354m
			(5651, 5987, OutsideSegment, Brake),          // len: 336m
			(5987, 6249, OutsideSegment, Coast),          // len: 262m
			(6249, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4563, OutsideSegment, Accelerate),        // len: 4563m
			(6400, 1e6, OutsideSegment, Accelerate));

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
			(0, 5612, OutsideSegment, Accelerate),        // len: 5612m
			(5612, 5916, OutsideSegment, Coast),          // len: 304m
			(5916, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 6114, OutsideSegment, Accelerate),        // len: 6114m
			(6114, 6787, OutsideSegment, Coast),          // len: 673m
			(6787, 7255, OutsideSegment, Brake),          // len: 468m
			(7255, 7731, OutsideSegment, Coast),          // len: 476m
			(7731, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5461, OutsideSegment, Accelerate),        // len: 5461m
			(5461, 5802, OutsideSegment, Coast),          // len: 341m
			(5802, 7148, OutsideSegment, Brake),          // len: 1346m
			(7148, 7446, OutsideSegment, Coast),          // len: 298m
			(7446, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2147, OutsideSegment, Accelerate),        // len: 2147m
			(2147, 2301, OutsideSegment, Coast),          // len: 154m
			(2301, 2481, OutsideSegment, Brake),          // len: 180m
			(2481, 2600, OutsideSegment, Coast),          // len: 119m
			(2600, 5516, OutsideSegment, Accelerate),     // len: 2916m
			(5516, 5870, OutsideSegment, Coast),          // len: 354m
			(5870, 6687, OutsideSegment, Brake),          // len: 817m
			(6687, 6973, OutsideSegment, Coast),          // len: 286m
			(6973, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2182, OutsideSegment, Accelerate),        // len: 2182m
			(2182, 2464, OutsideSegment, Coast),          // len: 282m
			(2464, 3327, OutsideSegment, Accelerate),     // len: 863m
			(3327, 3575, OutsideSegment, Coast),          // len: 248m
			(3575, 3971, OutsideSegment, Brake),          // len: 396m
			(3971, 4233, OutsideSegment, Coast),          // len: 262m
			(4233, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2194, OutsideSegment, Accelerate),        // len: 2194m
			(2194, 2499, OutsideSegment, Coast),          // len: 305m
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
			(0, 5297, OutsideSegment, Accelerate),        // len: 5297m
			(5297, 5651, OutsideSegment, Coast),          // len: 354m
			(5651, 5987, OutsideSegment, Brake),          // len: 336m
			(5987, 6249, OutsideSegment, Coast),          // len: 262m
			(6249, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4563, OutsideSegment, Accelerate),        // len: 4563m
			(5146, 5443, OutsideSegment, Accelerate),     // len: 297m
			(5443, 5538, OutsideSegment, Coast),          // len: 95m
			(5538, 6115, OutsideSegment, Brake),          // len: 577m
			(6115, 6400, OutsideSegment, Coast),          // len: 285m
			(6400, 1e6, OutsideSegment, Accelerate));


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
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5809, UseCase1, Coast),                // len: 337m
			(5809, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 7252, UseCase1, Coast),                // len: 1756m
			(7252, 7486, OutsideSegment, Coast),          // len: 234m
			(7486, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 6104, UseCase1, Coast),                // len: 1168m
			(6104, 6298, WithinSegment, Coast),           // len: 194m
			(6298, 7139, WithinSegment, Brake),           // len: 841m
			(7139, 7151, WithinSegment, Coast),           // len: 12m
			(7151, 7563, OutsideSegment, Coast),          // len: 412m
			(7563, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2487, UseCase1, Coast),                // len: 585m
			(2487, 2511, OutsideSegment, Coast),          // len: 24m
			(2511, 4039, OutsideSegment, Accelerate),     // len: 1528m
			(4039, 4961, WithinSegment, Accelerate),      // len: 922m
			(4961, 6211, UseCase1, Coast),                // len: 1250m
			(6211, 6624, WithinSegment, Coast),           // len: 413m
			(6624, 6673, WithinSegment, Brake),           // len: 49m
			(6673, 6686, WithinSegment, Coast),           // len: 13m
			(6686, 7086, OutsideSegment, Coast),          // len: 400m
			(7086, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2378, UseCase1, Coast),                // len: 301m
			(2378, 2996, WithinSegment, Accelerate),      // len: 618m
			(2996, 3875, UseCase1, Coast),                // len: 879m
			(3875, 3970, WithinSegment, Coast),           // len: 95m
			(3970, 4170, OutsideSegment, Coast),          // len: 200m
			(4170, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2390, UseCase1, Coast),                // len: 301m
			(2390, 2647, WithinSegment, Accelerate),      // len: 257m
			(2647, 3267, UseCase1, Coast),                // len: 620m
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
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4500, WithinSegment, Accelerate),      // len: 1004m
			(5146, 5342, WithinSegment, Accelerate),      // len: 196m
			(5342, 5669, UseCase2, Coast),                // len: 327m
			(5669, 5923, WithinSegment, Coast),           // len: 254m
			(5923, 6109, WithinSegment, Brake),           // len: 186m
			(6109, 6121, WithinSegment, Coast),           // len: 12m
			(6121, 6509, OutsideSegment, Coast),          // len: 388m
			(6509, 1e6, OutsideSegment, Accelerate));


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
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5472, WithinSegment, Accelerate),      // len: 1342m
			(5472, 5809, UseCase1, Coast),                // len: 337m
			(5809, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseB_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5496, WithinSegment, Accelerate),      // len: 841m
			(5496, 7252, UseCase1, Coast),                // len: 1756m
			(7252, 7486, OutsideSegment, Coast),          // len: 234m
			(7486, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseC_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4936, WithinSegment, Accelerate),      // len: 957m
			(4936, 6104, UseCase1, Coast),                // len: 1168m
			(6104, 6298, WithinSegment, Coast),           // len: 194m
			(6298, 7139, WithinSegment, Brake),           // len: 841m
			(7139, 7151, WithinSegment, Coast),           // len: 12m
			(7151, 7563, OutsideSegment, Coast),          // len: 412m
			(7563, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseD_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1902, WithinSegment, Accelerate),       // len: 1248m
			(1902, 2487, UseCase1, Coast),                // len: 585m
			(2487, 2511, OutsideSegment, Coast),          // len: 24m
			(2511, 4039, OutsideSegment, Accelerate),     // len: 1528m
			(4039, 4961, WithinSegment, Accelerate),      // len: 922m
			(4961, 6211, UseCase1, Coast),                // len: 1250m
			(6211, 6624, WithinSegment, Coast),           // len: 413m
			(6624, 6673, WithinSegment, Brake),           // len: 49m
			(6673, 6686, WithinSegment, Coast),           // len: 13m
			(6686, 7086, OutsideSegment, Coast),          // len: 400m
			(7086, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseE_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2077, WithinSegment, Accelerate),       // len: 1388m
			(2077, 2378, UseCase1, Coast),                // len: 301m
			(2378, 2996, WithinSegment, Accelerate),      // len: 618m
			(2996, 3875, UseCase1, Coast),                // len: 879m
			(3875, 3970, WithinSegment, Coast),           // len: 95m
			(3970, 4170, OutsideSegment, Coast),          // len: 200m
			(4170, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseF_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2089, WithinSegment, Accelerate),       // len: 1389m
			(2089, 2390, UseCase1, Coast),                // len: 301m
			(2390, 2647, WithinSegment, Accelerate),      // len: 257m
			(2647, 3267, UseCase1, Coast),                // len: 620m
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
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4854, WithinSegment, Accelerate),      // len: 1039m
			(4854, 5987, UseCase1, Coast),                // len: 1133m
			(5987, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseI_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(500, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseJ_HEV() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4563, WithinSegment, Accelerate),      // len: 1004m
			(5146, 5342, WithinSegment, Accelerate),      // len: 196m
			(5342, 5669, UseCase2, Coast),                // len: 327m
			(5669, 5923, WithinSegment, Coast),           // len: 254m
			(5923, 6109, WithinSegment, Brake),           // len: 186m
			(6109, 6121, WithinSegment, Coast),           // len: 12m
			(6121, 6509, OutsideSegment, Coast),          // len: 388m
			(6509, 1e6, OutsideSegment, Accelerate));

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
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
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
