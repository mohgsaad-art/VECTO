using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
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
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ADASTestsConventional
	{
		private const string BasePath = @"TestData\Integration\ADAS-Conventional\Group5PCCEng\";
		private const double tolerance = 1; //seconds of tolerance. Tolerance distance is calculated dynamically based on speed.

		private IXMLInputDataReader _xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			_kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

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


		[TestCase(@"TestData\Integration\ADAS-Conventional\Group5_EngineStopStart.xml")]
		public void TestVehicleWithADASEngineStopStart(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 4);
		}

		[TestCase(@"TestData\Integration\ADAS-Conventional\Group5_EcoRoll.xml")]
		public void TestVehicleWithADASEcoRoll(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 4);
		}

		[TestCase(@"TestData\Integration\ADAS-Conventional\Group5_EcoRollEngineStop.xml")]
		public void TestVehicleWithADASEcoRollEngineStopStart(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 1);
		}


		[TestCase(0, TestName = "EcoRoll DH1.1 const"),
		TestCase(1, TestName = "EcoRoll DH1.1 UH0.1"),
		TestCase(2, TestName = "EcoRoll DH1.3 const"),
		TestCase(3, TestName = "EcoRoll DH0.8 const - too flat"),
		TestCase(4, TestName = "EcoRoll DH1.5 const - too steep"),
		TestCase(5, TestName = "EcoRoll DH1.1 const - Stop"),
		TestCase(6, TestName = "EcoRoll DH1.1 const - TS60"),
		TestCase(7, TestName = "EcoRoll DH1.1 const - TS68"),
		TestCase(8, TestName = "EcoRoll DH1.1 const - TS72"),
		TestCase(9, TestName = "EcoRoll DH1.1 const - TS80"),
		]
		public void TestEcoRoll(int cycleIdx)
		{
			var jobName = @"TestData\Integration\ADAS-Conventional\Group5EcoRollEng\Class5_Tractor_ENG.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
			factory.WriteModalResults = true;
			//ActualModalData = true,
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
			const string jobName = @"TestData\Integration\ADAS-Conventional\Group9_RigidTruck_AT\Class_9_RigidTruck_AT_Eng_Neutral.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
			factory.WriteModalResults = true;
				//ActualModalData = true,
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
			const string jobName = @"TestData\Integration\ADAS-Conventional\Group9_RigidTruck_AT\Class_9_RigidTruck_AT_Eng_TC.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
			factory.WriteModalResults = true;
				//ActualModalData = true,
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

		[TestCase(@"TestData\Integration\ADAS-Conventional\Group9_AT_EngineStopStart.xml")]
		public void TestATVehicleWithADASEngineStopStart(string filename)
		{
			//var container = RunAllDeclarationJob(filename);
			var container = RunSingleDeclarationJob(filename, 5);
		}

		[TestCase(@"TestData\Integration\ADAS-Conventional\Group9_AT_EcoRoll.xml")]
		public void TestATVehicleWithADASEcoRoll(string filename)
		{
			var container = RunAllDeclarationJob(filename);
			//var container = RunSingleDeclarationJob(filename, 1);
		}

		[TestCase(5, TestName = "PCC Group5 RD RefLoad"),
		TestCase(1, TestName = "PCC Group5 LH RefLoad")]
		public void TestTCCDeclaration(int runIdx)
		{
			var jobName = @"TestData\Integration\ADAS-Conventional\Group5PCCDecl\Tractor_4x2_vehicle-class-5_5_t_0.xml";
			RunSingleDeclarationJob(jobName, runIdx);
		}

		public JobContainer RunAllDeclarationJob(string jobName)
		{
			var relativeJobPath = jobName;

			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(relativeJobPath), Path.GetFileName(relativeJobPath)));
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? _xmlInputReader.CreateDeclaration(relativeJobPath)
				//? new XMLDeclarationInputDataProvider(relativeJobPath, true)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			//ActualModalData = true,
			factory.Validate = false;

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();

			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));

			return jobContainer;
		}


		public JobContainer RunSingleDeclarationJob(string jobName, int runIdx)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(relativeJobPath), Path.GetFileName(relativeJobPath)));
			var inputData = Path.GetExtension(relativeJobPath) == ".xml"
				? _xmlInputReader.CreateDeclaration(relativeJobPath)
				//? new XMLDeclarationInputDataProvider(relativeJobPath, true)
				: JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			//ActualModalData = true,
			factory.Validate = false;
			
			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();
			var run = runs[runIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);

			return jobContainer;
		}

		[TestCase]
		public void Class5_PCC123_CaseA_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4119, OutsideSegment, Accelerate),        // len: 4119m
			(4119, 5426, WithinSegment, Accelerate),      // len: 1307m
			(5426, 5830, UseCase1, Coast),                // len: 404m
			(5830, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseB_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4609, OutsideSegment, Accelerate),        // len: 4609m
			(4609, 5414, WithinSegment, Accelerate),      // len: 805m
			(5414, 7291, UseCase1, Coast),                // len: 1877m
			(7291, 7490, OutsideSegment, Coast),          // len: 199m
			(7490, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseC_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3967, OutsideSegment, Accelerate),        // len: 3967m
			(3967, 4912, WithinSegment, Accelerate),      // len: 945m
			(4912, 6089, UseCase1, Coast),                // len: 1177m
			(6089, 6283, WithinSegment, Coast),           // len: 194m
			(6283, 7148, WithinSegment, Brake),           // len: 865m
			(7148, 7173, WithinSegment, Coast),           // len: 25m
			(7173, 7573, OutsideSegment, Coast),          // len: 400m
			(7573, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseD_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1867, WithinSegment, Accelerate),       // len: 1213m
			(1867, 2481, UseCase1, Coast),                // len: 614m
			(2481, 4021, OutsideSegment, Accelerate),     // len: 1540m
			(4021, 4919, WithinSegment, Accelerate),      // len: 898m
			(4919, 6217, UseCase1, Coast),                // len: 1298m
			(6217, 6593, WithinSegment, Coast),           // len: 376m
			(6593, 6692, WithinSegment, Brake),           // len: 99m
			(6692, 6704, WithinSegment, Coast),           // len: 12m
			(6704, 7092, OutsideSegment, Coast),          // len: 388m
			(7092, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123_CaseE_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2066, WithinSegment, Accelerate),       // len: 1377m
			(2066, 2377, UseCase1, Coast),                // len: 311m
			(2377, 2984, WithinSegment, Accelerate),      // len: 607m
			(2984, 3871, UseCase1, Coast),                // len: 887m
			(3871, 3978, WithinSegment, Coast),           // len: 107m
			(3978, 4179, OutsideSegment, Coast),          // len: 201m
			(4179, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123_CaseF_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2066, WithinSegment, Accelerate),       // len: 1366m
			(2066, 2400, UseCase1, Coast),                // len: 334m
			(2400, 2587, WithinSegment, Accelerate),      // len: 187m
			(2587, 3283, UseCase1, Coast),                // len: 696m
			(3283, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseG_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5076, WithinSegment, Accelerate),      // len: 1132m
			(5076, 5899, UseCase1, Coast),                // len: 823m
			(5899, 6275, WithinSegment, Coast),           // len: 376m
			(6275, 6571, WithinSegment, Brake),           // len: 296m
			(6571, 6596, WithinSegment, Coast),           // len: 25m
			(6596, 7323, OutsideSegment, Coast),          // len: 727m
			(7323, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123_CaseH_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3804, OutsideSegment, Accelerate),        // len: 3804m
			(3804, 4772, WithinSegment, Accelerate),      // len: 968m
			(4772, 6003, UseCase1, Coast),                // len: 1231m
			(6003, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123_CaseI_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CaseJ_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4470, WithinSegment, Accelerate),      // len: 911m
			(4470, 4488, WithinSegment, Roll),            // len: 18m
			(4488, 4591, WithinSegment, Accelerate),      // len: 103m
			(4591, 4606, WithinSegment, Roll),            // len: 15m
			(4606, 4667, WithinSegment, Accelerate),      // len: 61m
			(4667, 4678, WithinSegment, Roll),            // len: 11m
			(4678, 4721, WithinSegment, Accelerate),      // len: 43m
			(4721, 4730, WithinSegment, Roll),            // len: 9m
			(4730, 4758, WithinSegment, Accelerate),      // len: 28m
			(4758, 4765, WithinSegment, Roll),            // len: 7m
			(4765, 4799, WithinSegment, Accelerate),      // len: 34m
			(4799, 4804, WithinSegment, Roll),            // len: 5m
			(4804, 4932, WithinSegment, Accelerate),      // len: 128m
			(4932, 4939, WithinSegment, Roll),            // len: 7m
			(4939, 5050, WithinSegment, Accelerate),      // len: 111m
			(5050, 5062, WithinSegment, Roll),            // len: 12m
			(5062, 5095, WithinSegment, Accelerate),      // len: 33m
			(5095, 5109, WithinSegment, Roll),            // len: 14m
			(5109, 5231, WithinSegment, Accelerate),      // len: 122m
			(5231, 5250, WithinSegment, Roll),            // len: 19m
			(5250, 5369, WithinSegment, Accelerate),      // len: 119m
			(5369, 5674, UseCase2, Coast),                // len: 305m
			(5674, 5929, WithinSegment, Coast),           // len: 255m
			(5929, 6114, WithinSegment, Brake),           // len: 185m
			(6114, 6127, WithinSegment, Coast),           // len: 13m
			(6127, 6515, OutsideSegment, Coast),          // len: 388m
			(6515, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123_CrestCoast1_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3410, OutsideSegment, Accelerate),      // len: 2936m
			(3410, 4441, OutsideSegment, Coast),          // len: 1031m
			(4441, 5005, OutsideSegment, Brake),          // len: 564m
			(5005, 5422, OutsideSegment, Coast),          // len: 417m
			(5422, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123_CrestCoast2_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3614, OutsideSegment, Accelerate),      // len: 3140m
			(3614, 4138, OutsideSegment, Coast),          // len: 524m
			(4138, 4510, OutsideSegment, Brake),          // len: 372m
			(4510, 4718, OutsideSegment, Coast),          // len: 208m
			(4718, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseA_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4119, OutsideSegment, Accelerate),        // len: 4119m
			(4119, 5426, WithinSegment, Accelerate),      // len: 1307m
			(5426, 5830, UseCase1, Coast),                // len: 404m
			(5830, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseB_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4609, OutsideSegment, Accelerate),        // len: 4609m
			(4609, 5414, WithinSegment, Accelerate),      // len: 805m
			(5414, 7291, UseCase1, Coast),                // len: 1877m
			(7291, 7490, OutsideSegment, Coast),          // len: 199m
			(7490, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseC_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3967, OutsideSegment, Accelerate),        // len: 3967m
			(3967, 4912, WithinSegment, Accelerate),      // len: 945m
			(4912, 6089, UseCase1, Coast),                // len: 1177m
			(6089, 6161, WithinSegment, Coast),           // len: 72m
			(6161, 7170, WithinSegment, Brake),           // len: 1009m
			(7170, 7467, OutsideSegment, Coast),          // len: 297m
			(7467, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseD_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1867, WithinSegment, Accelerate),       // len: 1213m
			(1867, 2481, UseCase1, Coast),                // len: 614m
			(2481, 4021, OutsideSegment, Accelerate),     // len: 1540m
			(4021, 4919, WithinSegment, Accelerate),      // len: 898m
			(4919, 6217, UseCase1, Coast),                // len: 1298m
			(6217, 6312, WithinSegment, Coast),           // len: 95m
			(6312, 6696, WithinSegment, Brake),           // len: 384m
			(6696, 6708, WithinSegment, Coast),           // len: 12m
			(6708, 6982, OutsideSegment, Coast),          // len: 274m
			(6982, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseE_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2066, WithinSegment, Accelerate),       // len: 1377m
			(2066, 2377, UseCase1, Coast),                // len: 311m
			(2377, 2984, WithinSegment, Accelerate),      // len: 607m
			(2984, 3871, UseCase1, Coast),                // len: 887m
			(3871, 3978, WithinSegment, Coast),           // len: 107m
			(3978, 4179, OutsideSegment, Coast),          // len: 201m
			(4179, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseF_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2066, WithinSegment, Accelerate),       // len: 1366m
			(2066, 2400, UseCase1, Coast),                // len: 334m
			(2400, 2587, WithinSegment, Accelerate),      // len: 187m
			(2587, 3283, UseCase1, Coast),                // len: 696m
			(3283, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseG_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5076, WithinSegment, Accelerate),      // len: 1132m
			(5076, 5899, UseCase1, Coast),                // len: 823m
			(5899, 5994, WithinSegment, Coast),           // len: 95m
			(5994, 6595, WithinSegment, Brake),           // len: 601m
			(6595, 7118, OutsideSegment, Coast),          // len: 523m
			(7118, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseH_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3804, OutsideSegment, Accelerate),        // len: 3804m
			(3804, 4772, WithinSegment, Accelerate),      // len: 968m
			(4772, 6003, UseCase1, Coast),                // len: 1231m
			(6003, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CaseI_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC12_CaseJ_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 4470, WithinSegment, Accelerate),      // len: 911m
			(4470, 4488, WithinSegment, Roll),            // len: 18m
			(4488, 4591, WithinSegment, Accelerate),      // len: 103m
			(4591, 4606, WithinSegment, Roll),            // len: 15m
			(4606, 4667, WithinSegment, Accelerate),      // len: 61m
			(4667, 4678, WithinSegment, Roll),            // len: 11m
			(4678, 4721, WithinSegment, Accelerate),      // len: 43m
			(4721, 4730, WithinSegment, Roll),            // len: 9m
			(4730, 4758, WithinSegment, Accelerate),      // len: 28m
			(4758, 4765, WithinSegment, Roll),            // len: 7m
			(4765, 4799, WithinSegment, Accelerate),      // len: 34m
			(4799, 4804, WithinSegment, Roll),            // len: 5m
			(4804, 4932, WithinSegment, Accelerate),      // len: 128m
			(4932, 4939, WithinSegment, Roll),            // len: 7m
			(4939, 5050, WithinSegment, Accelerate),      // len: 111m
			(5050, 5062, WithinSegment, Roll),            // len: 12m
			(5062, 5095, WithinSegment, Accelerate),      // len: 33m
			(5095, 5109, WithinSegment, Roll),            // len: 14m
			(5109, 5231, WithinSegment, Accelerate),      // len: 122m
			(5231, 5250, WithinSegment, Roll),            // len: 19m
			(5250, 5369, WithinSegment, Accelerate),      // len: 119m
			(5369, 5674, UseCase2, Coast),                // len: 305m
			(5674, 5746, WithinSegment, Coast),           // len: 72m
			(5746, 6130, WithinSegment, Brake),           // len: 384m
			(6130, 6404, OutsideSegment, Coast),          // len: 274m
			(6404, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC12_CrestCoast1_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3410, OutsideSegment, Accelerate),      // len: 2936m
			(3410, 4441, OutsideSegment, Coast),          // len: 1031m
			(4441, 5005, OutsideSegment, Brake),          // len: 564m
			(5005, 5422, OutsideSegment, Coast),          // len: 417m
			(5422, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_PCC12_CrestCoast2_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3614, OutsideSegment, Accelerate),      // len: 3140m
			(3614, 4138, OutsideSegment, Coast),          // len: 524m
			(4138, 4510, OutsideSegment, Brake),          // len: 372m
			(4510, 4718, OutsideSegment, Coast),          // len: 208m
			(4718, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseA_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5601, OutsideSegment, Accelerate),        // len: 5601m
			(5601, 5940, OutsideSegment, Coast),          // len: 339m
			(5940, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseB_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 6079, OutsideSegment, Accelerate),        // len: 6079m
			(6079, 6704, OutsideSegment, Coast),          // len: 625m
			(6704, 7293, OutsideSegment, Brake),          // len: 589m
			(7293, 7757, OutsideSegment, Coast),          // len: 464m
			(7757, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseC_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5449, OutsideSegment, Accelerate),        // len: 5449m
			(5449, 5779, OutsideSegment, Coast),          // len: 330m
			(5779, 7160, OutsideSegment, Brake),          // len: 1381m
			(7160, 7458, OutsideSegment, Coast),          // len: 298m
			(7458, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseD_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2147, OutsideSegment, Accelerate),        // len: 2147m
			(2147, 2289, OutsideSegment, Coast),          // len: 142m
			(2289, 2481, OutsideSegment, Brake),          // len: 192m
			(2481, 2612, OutsideSegment, Coast),          // len: 131m
			(2612, 5505, OutsideSegment, Accelerate),     // len: 2893m
			(5505, 5847, OutsideSegment, Coast),          // len: 342m
			(5847, 6700, OutsideSegment, Brake),          // len: 853m
			(6700, 6985, OutsideSegment, Coast),          // len: 285m
			(6985, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_NoADAS_CaseE_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2182, OutsideSegment, Accelerate),        // len: 2182m
			(2182, 2476, OutsideSegment, Coast),          // len: 294m
			(2476, 3328, OutsideSegment, Accelerate),     // len: 852m
			(3328, 3563, OutsideSegment, Coast),          // len: 235m
			(3563, 3972, OutsideSegment, Brake),          // len: 409m
			(3972, 4234, OutsideSegment, Coast),          // len: 262m
			(4234, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_NoADAS_CaseF_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2182, OutsideSegment, Accelerate),        // len: 2182m
			(2182, 2511, OutsideSegment, Coast),          // len: 329m
			(2511, 2873, OutsideSegment, Accelerate),     // len: 362m
			(2873, 3475, OutsideSegment, Coast),          // len: 602m
			(3475, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseG_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5426, OutsideSegment, Accelerate),        // len: 5426m
			(5426, 5591, OutsideSegment, Coast),          // len: 165m
			(5591, 6600, OutsideSegment, Brake),          // len: 1009m
			(6600, 7111, OutsideSegment, Coast),          // len: 511m
			(7111, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseH_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5286, OutsideSegment, Accelerate),        // len: 5286m
			(5286, 5640, OutsideSegment, Coast),          // len: 354m
			(5640, 6000, OutsideSegment, Brake),          // len: 360m
			(6000, 6250, OutsideSegment, Coast),          // len: 250m
			(6250, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseI_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_NoADAS_CaseJ_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4470, OutsideSegment, Accelerate),        // len: 4470m
			(4470, 4488, OutsideSegment, Roll),           // len: 18m
			(4488, 4591, OutsideSegment, Accelerate),     // len: 103m
			(4591, 4606, OutsideSegment, Roll),           // len: 15m
			(4606, 4667, OutsideSegment, Accelerate),     // len: 61m
			(4667, 4678, OutsideSegment, Roll),           // len: 11m
			(4678, 4721, OutsideSegment, Accelerate),     // len: 43m
			(4721, 4730, OutsideSegment, Roll),           // len: 9m
			(4730, 4758, OutsideSegment, Accelerate),     // len: 28m
			(4758, 4765, OutsideSegment, Roll),           // len: 7m
			(4765, 4799, OutsideSegment, Accelerate),     // len: 34m
			(4799, 4804, OutsideSegment, Roll),           // len: 5m
			(4804, 4932, OutsideSegment, Accelerate),     // len: 128m
			(4932, 4939, OutsideSegment, Roll),           // len: 7m
			(4939, 5050, OutsideSegment, Accelerate),     // len: 111m
			(5050, 5062, OutsideSegment, Roll),           // len: 12m
			(5062, 5095, OutsideSegment, Accelerate),     // len: 33m
			(5095, 5109, OutsideSegment, Roll),           // len: 14m
			(5109, 5231, OutsideSegment, Accelerate),     // len: 122m
			(5231, 5250, OutsideSegment, Roll),           // len: 19m
			(5250, 5459, OutsideSegment, Accelerate),     // len: 209m
			(5459, 5554, OutsideSegment, Coast),          // len: 95m
			(5554, 6131, OutsideSegment, Brake),          // len: 577m
			(6131, 6404, OutsideSegment, Coast),          // len: 273m
			(6404, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_NoADAS_CrestCoast1_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3410, OutsideSegment, Accelerate),      // len: 2936m
			(3410, 4441, OutsideSegment, Coast),          // len: 1031m
			(4441, 5005, OutsideSegment, Brake),          // len: 564m
			(5005, 5422, OutsideSegment, Coast),          // len: 417m
			(5422, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_NoADAS_CrestCoast2_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3614, OutsideSegment, Accelerate),      // len: 3140m
			(3614, 4138, OutsideSegment, Coast),          // len: 524m
			(4138, 4510, OutsideSegment, Brake),          // len: 372m
			(4510, 4718, OutsideSegment, Coast),          // len: 208m
			(4718, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseA_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5601, OutsideSegment, Accelerate),        // len: 5601m
			(5601, 5671, OutsideSegment, Coast),          // len: 70m
			(5671, 6072, OutsideSegment, Roll),           // len: 401m
			(6072, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseB_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 6079, OutsideSegment, Accelerate),        // len: 6079m
			(6079, 6149, OutsideSegment, Coast),          // len: 70m
			(6149, 6433, OutsideSegment, Roll),           // len: 284m
			(6433, 6445, OutsideSegment, Brake),          // len: 12m
			(6445, 6469, OutsideSegment, Coast),          // len: 24m
			(6469, 7286, OutsideSegment, Brake),          // len: 817m
			(7286, 7750, OutsideSegment, Coast),          // len: 464m
			(7750, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseC_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5449, OutsideSegment, Accelerate),        // len: 5449m
			(5449, 5519, OutsideSegment, Coast),          // len: 70m
			(5519, 5708, OutsideSegment, Roll),           // len: 189m
			(5708, 7162, OutsideSegment, Brake),          // len: 1454m
			(7162, 7460, OutsideSegment, Coast),          // len: 298m
			(7460, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseD_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2147, OutsideSegment, Accelerate),        // len: 2147m
			(2147, 2289, OutsideSegment, Coast),          // len: 142m
			(2289, 2481, OutsideSegment, Brake),          // len: 192m
			(2481, 2612, OutsideSegment, Coast),          // len: 131m
			(2612, 5505, OutsideSegment, Accelerate),     // len: 2893m
			(5505, 5575, OutsideSegment, Coast),          // len: 70m
			(5575, 5764, OutsideSegment, Roll),           // len: 189m
			(5764, 6701, OutsideSegment, Brake),          // len: 937m
			(6701, 6987, OutsideSegment, Coast),          // len: 286m
			(6987, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseE_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2182, OutsideSegment, Accelerate),        // len: 2182m
			(2182, 2252, OutsideSegment, Coast),          // len: 70m
			(2252, 2548, OutsideSegment, Roll),           // len: 296m
			(2548, 3330, OutsideSegment, Accelerate),     // len: 782m
			(3330, 3400, OutsideSegment, Coast),          // len: 70m
			(3400, 3518, OutsideSegment, Roll),           // len: 118m
			(3518, 3975, OutsideSegment, Brake),          // len: 457m
			(3975, 4236, OutsideSegment, Coast),          // len: 261m
			(4236, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseF_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2182, OutsideSegment, Accelerate),        // len: 2182m
			(2182, 2252, OutsideSegment, Coast),          // len: 70m
			(2252, 2666, OutsideSegment, Roll),           // len: 414m
			(2666, 2876, OutsideSegment, Accelerate),     // len: 210m
			(2876, 2946, OutsideSegment, Coast),          // len: 70m
			(2946, 3147, OutsideSegment, Roll),           // len: 201m
			(3147, 3159, OutsideSegment, Brake),          // len: 12m
			(3159, 3183, OutsideSegment, Coast),          // len: 24m
			(3183, 3280, OutsideSegment, Brake),          // len: 97m
			(3280, 3517, OutsideSegment, Coast),          // len: 237m
			(3517, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseG_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5426, OutsideSegment, Accelerate),        // len: 5426m
			(5426, 5591, OutsideSegment, Coast),          // len: 165m
			(5591, 6600, OutsideSegment, Brake),          // len: 1009m
			(6600, 7111, OutsideSegment, Coast),          // len: 511m
			(7111, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseH_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5286, OutsideSegment, Accelerate),        // len: 5286m
			(5286, 5356, OutsideSegment, Coast),          // len: 70m
			(5356, 5545, OutsideSegment, Roll),           // len: 189m
			(5545, 5557, OutsideSegment, Brake),          // len: 12m
			(5557, 5569, OutsideSegment, Coast),          // len: 12m
			(5569, 6002, OutsideSegment, Brake),          // len: 433m
			(6002, 6252, OutsideSegment, Coast),          // len: 250m
			(6252, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseI_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CaseJ_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4470, OutsideSegment, Accelerate),        // len: 4470m
			(4470, 4488, OutsideSegment, Roll),           // len: 18m
			(4488, 4591, OutsideSegment, Accelerate),     // len: 103m
			(4591, 4606, OutsideSegment, Roll),           // len: 15m
			(4606, 4667, OutsideSegment, Accelerate),     // len: 61m
			(4667, 4678, OutsideSegment, Roll),           // len: 11m
			(4678, 4721, OutsideSegment, Accelerate),     // len: 43m
			(4721, 4730, OutsideSegment, Roll),           // len: 9m
			(4730, 4758, OutsideSegment, Accelerate),     // len: 28m
			(4758, 4765, OutsideSegment, Roll),           // len: 7m
			(4765, 4799, OutsideSegment, Accelerate),     // len: 34m
			(4799, 4804, OutsideSegment, Roll),           // len: 5m
			(4804, 4932, OutsideSegment, Accelerate),     // len: 128m
			(4932, 4939, OutsideSegment, Roll),           // len: 7m
			(4939, 5050, OutsideSegment, Accelerate),     // len: 111m
			(5050, 5062, OutsideSegment, Roll),           // len: 12m
			(5062, 5095, OutsideSegment, Accelerate),     // len: 33m
			(5095, 5109, OutsideSegment, Roll),           // len: 14m
			(5109, 5231, OutsideSegment, Accelerate),     // len: 122m
			(5231, 5250, OutsideSegment, Roll),           // len: 19m
			(5250, 5459, OutsideSegment, Accelerate),     // len: 209m
			(5459, 5554, OutsideSegment, Coast),          // len: 95m
			(5554, 6131, OutsideSegment, Brake),          // len: 577m
			(6131, 6404, OutsideSegment, Coast),          // len: 273m
			(6404, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast1_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3410, OutsideSegment, Accelerate),      // len: 2936m
			(3410, 3468, OutsideSegment, Coast),          // len: 58m
			(3468, 3854, OutsideSegment, Roll),           // len: 386m
			(3854, 3864, OutsideSegment, Brake),          // len: 10m
			(3864, 3915, OutsideSegment, Coast),          // len: 51m
			(3915, 5012, OutsideSegment, Brake),          // len: 1097m
			(5012, 5420, OutsideSegment, Coast),          // len: 408m
			(5420, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollWithoutEngineStop_CrestCoast2_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3614, OutsideSegment, Accelerate),      // len: 3140m
			(3614, 3672, OutsideSegment, Coast),          // len: 58m
			(3672, 3890, OutsideSegment, Roll),           // len: 218m
			(3890, 3900, OutsideSegment, Brake),          // len: 10m
			(3900, 3910, OutsideSegment, Coast),          // len: 10m
			(3910, 4505, OutsideSegment, Brake),          // len: 595m
			(4505, 4723, OutsideSegment, Coast),          // len: 218m
			(4723, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CaseA_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5601, OutsideSegment, Accelerate),        // len: 5601m
			(5601, 5671, OutsideSegment, Coast),          // len: 70m
			(5671, 6072, OutsideSegment, Roll),           // len: 401m
			(6072, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseB_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 6079, OutsideSegment, Accelerate),        // len: 6079m
			(6079, 6149, OutsideSegment, Coast),          // len: 70m
			(6149, 6433, OutsideSegment, Roll),           // len: 284m
			(6433, 6445, OutsideSegment, Brake),          // len: 12m
			(6445, 6457, OutsideSegment, Coast),          // len: 12m
			(6457, 7286, OutsideSegment, Brake),          // len: 829m
			(7286, 7750, OutsideSegment, Coast),          // len: 464m
			(7750, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseC_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5449, OutsideSegment, Accelerate),        // len: 5449m
			(5449, 5519, OutsideSegment, Coast),          // len: 70m
			(5519, 5708, OutsideSegment, Roll),           // len: 189m
			(5708, 7162, OutsideSegment, Brake),          // len: 1454m
			(7162, 7460, OutsideSegment, Coast),          // len: 298m
			(7460, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CaseD_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2147, OutsideSegment, Accelerate),        // len: 2147m
			(2147, 2289, OutsideSegment, Coast),          // len: 142m
			(2289, 2481, OutsideSegment, Brake),          // len: 192m
			(2481, 2612, OutsideSegment, Coast),          // len: 131m
			(2612, 5505, OutsideSegment, Accelerate),     // len: 2893m
			(5505, 5575, OutsideSegment, Coast),          // len: 70m
			(5575, 5764, OutsideSegment, Roll),           // len: 189m
			(5764, 6701, OutsideSegment, Brake),          // len: 937m
			(6701, 6987, OutsideSegment, Coast),          // len: 286m
			(6987, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseE_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2182, OutsideSegment, Accelerate),        // len: 2182m
			(2182, 2252, OutsideSegment, Coast),          // len: 70m
			(2252, 2548, OutsideSegment, Roll),           // len: 296m
			(2548, 3330, OutsideSegment, Accelerate),     // len: 782m
			(3330, 3400, OutsideSegment, Coast),          // len: 70m
			(3400, 3518, OutsideSegment, Roll),           // len: 118m
			(3518, 3975, OutsideSegment, Brake),          // len: 457m
			(3975, 4236, OutsideSegment, Coast),          // len: 261m
			(4236, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseF_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 2182, OutsideSegment, Accelerate),        // len: 2182m
			(2182, 2252, OutsideSegment, Coast),          // len: 70m
			(2252, 2666, OutsideSegment, Roll),           // len: 414m
			(2666, 2876, OutsideSegment, Accelerate),     // len: 210m
			(2876, 2946, OutsideSegment, Coast),          // len: 70m
			(2946, 3147, OutsideSegment, Roll),           // len: 201m
			(3147, 3159, OutsideSegment, Brake),          // len: 12m
			(3159, 3171, OutsideSegment, Coast),          // len: 12m
			(3171, 3280, OutsideSegment, Brake),          // len: 109m
			(3280, 3517, OutsideSegment, Coast),          // len: 237m
			(3517, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseG_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5426, OutsideSegment, Accelerate),        // len: 5426m
			(5426, 5591, OutsideSegment, Coast),          // len: 165m
			(5591, 6600, OutsideSegment, Brake),          // len: 1009m
			(6600, 7111, OutsideSegment, Coast),          // len: 511m
			(7111, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseH_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 5286, OutsideSegment, Accelerate),        // len: 5286m
			(5286, 5356, OutsideSegment, Coast),          // len: 70m
			(5356, 5545, OutsideSegment, Roll),           // len: 189m
			(5545, 6002, OutsideSegment, Brake),          // len: 457m
			(6002, 6252, OutsideSegment, Coast),          // len: 250m
			(6252, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseI_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CaseJ_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4470, OutsideSegment, Accelerate),        // len: 4470m
			(4470, 4488, OutsideSegment, Roll),           // len: 18m
			(4488, 4591, OutsideSegment, Accelerate),     // len: 103m
			(4591, 4606, OutsideSegment, Roll),           // len: 15m
			(4606, 4667, OutsideSegment, Accelerate),     // len: 61m
			(4667, 4678, OutsideSegment, Roll),           // len: 11m
			(4678, 4721, OutsideSegment, Accelerate),     // len: 43m
			(4721, 4730, OutsideSegment, Roll),           // len: 9m
			(4730, 4758, OutsideSegment, Accelerate),     // len: 28m
			(4758, 4765, OutsideSegment, Roll),           // len: 7m
			(4765, 4799, OutsideSegment, Accelerate),     // len: 34m
			(4799, 4804, OutsideSegment, Roll),           // len: 5m
			(4804, 4932, OutsideSegment, Accelerate),     // len: 128m
			(4932, 4939, OutsideSegment, Roll),           // len: 7m
			(4939, 5050, OutsideSegment, Accelerate),     // len: 111m
			(5050, 5062, OutsideSegment, Roll),           // len: 12m
			(5062, 5095, OutsideSegment, Accelerate),     // len: 33m
			(5095, 5109, OutsideSegment, Roll),           // len: 14m
			(5109, 5231, OutsideSegment, Accelerate),     // len: 122m
			(5231, 5250, OutsideSegment, Roll),           // len: 19m
			(5250, 5459, OutsideSegment, Accelerate),     // len: 209m
			(5459, 5554, OutsideSegment, Coast),          // len: 95m
			(5554, 6131, OutsideSegment, Brake),          // len: 577m
			(6131, 6404, OutsideSegment, Coast),          // len: 273m
			(6404, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_EcoRollEngineStop_CrestCoast1_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3410, OutsideSegment, Accelerate),      // len: 2936m
			(3410, 3468, OutsideSegment, Coast),          // len: 58m
			(3468, 3854, OutsideSegment, Roll),           // len: 386m
			(3854, 3864, OutsideSegment, Brake),          // len: 10m
			(3864, 3915, OutsideSegment, Coast),          // len: 20m
			(3915, 5012, OutsideSegment, Brake),          // len: 1128m
			(5012, 5420, OutsideSegment, Coast),          // len: 408m
			(5420, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_EcoRollEngineStop_CrestCoast2_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3614, OutsideSegment, Accelerate),      // len: 3140m
			(3614, 3672, OutsideSegment, Coast),          // len: 58m
			(3672, 3890, OutsideSegment, Roll),           // len: 218m
			(3890, 4505, OutsideSegment, Brake),          // len: 615m
			(4505, 4723, OutsideSegment, Coast),          // len: 218m
			(4723, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseA_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4025, OutsideSegment, Accelerate),        // len: 4025m
			(4025, 5309, WithinSegment, Accelerate),      // len: 1284m
			(5309, 5908, UseCase1, Roll),                 // len: 599m
			(5908, 5920, OutsideSegment, Coast),          // len: 12m
			(5920, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseB_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4352, OutsideSegment, Accelerate),        // len: 4352m
			(4352, 5216, WithinSegment, Accelerate),      // len: 864m
			(5216, 6698, UseCase1, Roll),                 // len: 1482m
			(6698, 6769, WithinSegment, Coast),           // len: 71m
			(6769, 7169, WithinSegment, Roll),            // len: 400m
			(7169, 7182, WithinSegment, Brake),           // len: 13m
			(7182, 7551, WithinSegment, Coast),           // len: 369m
			(7551, 7911, OutsideSegment, Coast),          // len: 360m
			(7911, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseC_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3850, OutsideSegment, Accelerate),        // len: 3850m
			(3850, 4807, WithinSegment, Accelerate),      // len: 957m
			(4807, 5935, UseCase1, Roll),                 // len: 1128m
			(5935, 6141, WithinSegment, Coast),           // len: 206m
			(6141, 7154, WithinSegment, Brake),           // len: 1013m
			(7154, 7278, WithinSegment, Coast),           // len: 124m
			(7278, 7579, OutsideSegment, Coast),          // len: 301m
			(7579, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseD_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 630, OutsideSegment, Accelerate),         // len: 630m
			(630, 1786, WithinSegment, Accelerate),       // len: 1156m
			(1786, 2500, UseCase1, Roll),                 // len: 714m
			(2500, 2547, OutsideSegment, Coast),          // len: 47m
			(2547, 3912, OutsideSegment, Accelerate),     // len: 1365m
			(3912, 4834, WithinSegment, Accelerate),      // len: 922m
			(4834, 5994, UseCase1, Roll),                 // len: 1160m
			(5994, 6272, WithinSegment, Coast),           // len: 278m
			(6272, 6693, WithinSegment, Brake),           // len: 421m
			(6693, 6804, WithinSegment, Coast),           // len: 111m
			(6804, 7093, OutsideSegment, Coast),          // len: 289m
			(7093, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseE_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 2007, WithinSegment, Accelerate),       // len: 1353m
			(2007, 2411, UseCase1, Roll),                 // len: 404m
			(2411, 2434, WithinSegment, Coast),           // len: 23m
			(2434, 2947, WithinSegment, Accelerate),      // len: 513m
			(2947, 3722, UseCase1, Roll),                 // len: 775m
			(3722, 4060, WithinSegment, Coast),           // len: 338m
			(4060, 4299, OutsideSegment, Coast),          // len: 239m
			(4299, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseF_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2007, WithinSegment, Accelerate),       // len: 1342m
			(2007, 3172, UseCase1, Roll),                 // len: 1165m
			(3172, 3243, WithinSegment, Coast),           // len: 71m
			(3243, 3350, WithinSegment, Roll),            // len: 107m
			(3350, 3539, OutsideSegment, Roll),           // len: 189m
			(3539, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseG_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3920, OutsideSegment, Accelerate),        // len: 3920m
			(3920, 4994, WithinSegment, Accelerate),      // len: 1074m
			(4994, 5774, UseCase1, Roll),                 // len: 780m
			(5774, 6053, WithinSegment, Coast),           // len: 279m
			(6053, 6572, WithinSegment, Brake),           // len: 519m
			(6572, 6904, WithinSegment, Coast),           // len: 332m
			(6904, 7323, OutsideSegment, Coast),          // len: 419m
			(7323, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseH_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3722, OutsideSegment, Accelerate),        // len: 3722m
			(3722, 4609, WithinSegment, Accelerate),      // len: 887m
			(4609, 5819, UseCase1, Roll),                 // len: 1210m
			(5819, 5890, WithinSegment, Coast),           // len: 71m
			(5890, 6094, WithinSegment, Roll),            // len: 204m
			(6094, 6331, OutsideSegment, Roll),           // len: 237m
			(6331, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseI_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CaseJ_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4470, WithinSegment, Accelerate),      // len: 923m
			(4470, 4488, WithinSegment, Roll),            // len: 18m
			(4488, 4591, WithinSegment, Accelerate),      // len: 103m
			(4591, 4606, WithinSegment, Roll),            // len: 15m
			(4606, 4667, WithinSegment, Accelerate),      // len: 61m
			(4667, 4678, WithinSegment, Roll),            // len: 11m
			(4678, 4721, WithinSegment, Accelerate),      // len: 43m
			(4721, 4730, WithinSegment, Roll),            // len: 9m
			(4730, 4758, WithinSegment, Accelerate),      // len: 28m
			(4758, 4765, WithinSegment, Roll),            // len: 7m
			(4765, 4799, WithinSegment, Accelerate),      // len: 34m
			(4799, 4804, WithinSegment, Roll),            // len: 5m
			(4804, 4932, WithinSegment, Accelerate),      // len: 128m
			(4932, 4939, WithinSegment, Roll),            // len: 7m
			(4939, 5050, WithinSegment, Accelerate),      // len: 111m
			(5050, 5062, WithinSegment, Roll),            // len: 12m
			(5062, 5095, WithinSegment, Accelerate),      // len: 33m
			(5095, 5109, WithinSegment, Roll),            // len: 14m
			(5109, 5231, WithinSegment, Accelerate),      // len: 122m
			(5231, 5250, WithinSegment, Roll),            // len: 19m
			(5250, 5369, WithinSegment, Accelerate),      // len: 119m
			(5369, 5618, UseCase2, Roll),                 // len: 249m
			(5618, 5836, WithinSegment, Coast),           // len: 218m
			(5836, 6120, WithinSegment, Brake),           // len: 284m
			(6120, 6231, WithinSegment, Coast),           // len: 111m
			(6231, 6520, OutsideSegment, Coast),          // len: 289m
			(6520, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast1_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3410, OutsideSegment, Accelerate),      // len: 2936m
			(3410, 3468, OutsideSegment, Coast),          // len: 58m
			(3468, 3854, OutsideSegment, Roll),           // len: 386m
			(3854, 3864, OutsideSegment, Brake),          // len: 10m
			(3864, 3915, OutsideSegment, Coast),          // len: 51m
			(3915, 5012, OutsideSegment, Brake),          // len: 1097m
			(5012, 5420, OutsideSegment, Coast),          // len: 408m
			(5420, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollWithoutEngineStop_CrestCoast2_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3614, OutsideSegment, Accelerate),      // len: 3140m
			(3614, 3672, OutsideSegment, Coast),          // len: 58m
			(3672, 3890, OutsideSegment, Roll),           // len: 218m
			(3890, 3900, OutsideSegment, Brake),          // len: 10m
			(3900, 3910, OutsideSegment, Coast),          // len: 10m
			(3910, 4505, OutsideSegment, Brake),          // len: 595m
			(4505, 4723, OutsideSegment, Coast),          // len: 218m
			(4723, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseA_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4025, OutsideSegment, Accelerate),        // len: 4025m
			(4025, 5309, WithinSegment, Accelerate),      // len: 1284m
			(5309, 5920, UseCase1, Roll),                 // len: 599m
			//(5920, 5932, OutsideSegment, Coast),          // len: 24m
			(5920, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseB_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 4352, OutsideSegment, Accelerate),        // len: 4352m
			(4352, 5216, WithinSegment, Accelerate),      // len: 864m
			(5216, 6698, UseCase1, Roll),                 // len: 1482m
			(6698, 6757, WithinSegment, Coast),           // len: 59m
			(6757, 7158, WithinSegment, Roll),            // len: 401m
			//(7158, 7170, WithinSegment, Brake),           // len: 12m
			(7170, 7552, WithinSegment, Coast),           // len: 382m
			(7552, 7912, OutsideSegment, Coast),          // len: 360m
			(7912, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseC_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3850, OutsideSegment, Accelerate),        // len: 3850m
			(3850, 4807, WithinSegment, Accelerate),      // len: 957m
			(4807, 5935, UseCase1, Roll),                 // len: 1128m
			(5935, 6141, WithinSegment, Coast),           // len: 206m
			(6141, 7155, WithinSegment, Brake),           // len: 1014m
			(7155, 7278, WithinSegment, Coast),           // len: 123m
			(7278, 7579, OutsideSegment, Coast),          // len: 301m
			(7579, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseD_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 630, OutsideSegment, Accelerate),         // len: 630m
			(630, 1786, WithinSegment, Accelerate),       // len: 1156m
			(1786, 2500, UseCase1, Roll),                 // len: 714m
			(2500, 2559, OutsideSegment, Coast),          // len: 59m
			(2559, 3912, OutsideSegment, Accelerate),     // len: 1353m
			(3912, 4834, WithinSegment, Accelerate),      // len: 922m
			(4834, 5994, UseCase1, Roll),                 // len: 1160m
			(5994, 6273, WithinSegment, Coast),           // len: 279m
			(6273, 6693, WithinSegment, Brake),           // len: 420m
			(6693, 6804, WithinSegment, Coast),           // len: 111m
			(6804, 7093, OutsideSegment, Coast),          // len: 289m
			(7093, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseE_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 2007, WithinSegment, Accelerate),       // len: 1353m
			(2007, 2411, UseCase1, Roll),                 // len: 404m
			(2411, 2434, WithinSegment, Coast),           // len: 23m
			(2434, 2947, WithinSegment, Accelerate),      // len: 513m
			(2947, 3722, UseCase1, Roll),                 // len: 775m
			(3722, 4061, WithinSegment, Coast),           // len: 339m
			(4061, 4299, OutsideSegment, Coast),          // len: 238m
			(4299, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseF_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 665, OutsideSegment, Accelerate),         // len: 665m
			(665, 2007, WithinSegment, Accelerate),       // len: 1342m
			(2007, 3172, UseCase1, Roll),                 // len: 1165m
			(3172, 3231, WithinSegment, Coast),           // len: 59m
			(3231, 3350, WithinSegment, Roll),            // len: 119m
			(3350, 3539, OutsideSegment, Roll),           // len: 189m
			(3539, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseG_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3920, OutsideSegment, Accelerate),        // len: 3920m
			(3920, 4994, WithinSegment, Accelerate),      // len: 1074m
			(4994, 5774, UseCase1, Roll),                 // len: 780m
			(5774, 6040, WithinSegment, Coast),           // len: 266m
			(6040, 6572, WithinSegment, Brake),           // len: 532m
			(6572, 6904, WithinSegment, Coast),           // len: 332m
			(6904, 7323, OutsideSegment, Coast),          // len: 419m
			(7323, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseH_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3722, OutsideSegment, Accelerate),        // len: 3722m
			(3722, 4609, WithinSegment, Accelerate),      // len: 887m
			(4609, 5819, UseCase1, Roll),                 // len: 1210m
			(5819, 5890, WithinSegment, Coast),           // len: 71m
			(5890, 6094, WithinSegment, Roll),            // len: 204m
			(6094, 6332, OutsideSegment, Roll),           // len: 238m
			(6332, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseI_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CaseJ_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4470, WithinSegment, Accelerate),      // len: 923m
			(4470, 4488, WithinSegment, Roll),            // len: 18m
			(4488, 4591, WithinSegment, Accelerate),      // len: 103m
			(4591, 4606, WithinSegment, Roll),            // len: 15m
			(4606, 4667, WithinSegment, Accelerate),      // len: 61m
			(4667, 4678, WithinSegment, Roll),            // len: 11m
			(4678, 4721, WithinSegment, Accelerate),      // len: 43m
			(4721, 4730, WithinSegment, Roll),            // len: 9m
			(4730, 4758, WithinSegment, Accelerate),      // len: 28m
			(4758, 4765, WithinSegment, Roll),            // len: 7m
			(4765, 4799, WithinSegment, Accelerate),      // len: 34m
			(4799, 4804, WithinSegment, Roll),            // len: 5m
			(4804, 4932, WithinSegment, Accelerate),      // len: 128m
			(4932, 4939, WithinSegment, Roll),            // len: 7m
			(4939, 5050, WithinSegment, Accelerate),      // len: 111m
			(5050, 5062, WithinSegment, Roll),            // len: 12m
			(5062, 5095, WithinSegment, Accelerate),      // len: 33m
			(5095, 5109, WithinSegment, Roll),            // len: 14m
			(5109, 5231, WithinSegment, Accelerate),      // len: 122m
			(5231, 5250, WithinSegment, Roll),            // len: 19m
			(5250, 5369, WithinSegment, Accelerate),      // len: 119m
			(5369, 5618, UseCase2, Roll),                 // len: 249m
			(5618, 5836, WithinSegment, Coast),           // len: 218m
			(5836, 6120, WithinSegment, Brake),           // len: 284m
			(6120, 6231, WithinSegment, Coast),           // len: 111m
			(6231, 6520, OutsideSegment, Coast),          // len: 289m
			(6520, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast1_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3410, OutsideSegment, Accelerate),      // len: 2936m
			(3410, 3468, OutsideSegment, Coast),          // len: 58m
			(3468, 3854, OutsideSegment, Roll),           // len: 386m
			(3854, 3864, OutsideSegment, Brake),          // len: 10m
			(3864, 3915, OutsideSegment, Coast),          // len: 20m
			(3915, 5012, OutsideSegment, Brake),          // len: 1128m
			(5012, 5420, OutsideSegment, Coast),          // len: 408m
			(5420, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_PCC123EcoRollEngineStop_CrestCoast2_Conventional() => TestPCC(MethodBase.GetCurrentMethod().Name,
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 7, OutsideSegment, Accelerate),           // len: 7m
			(7, 10, OutsideSegment, Roll),                // len: 3m
			(10, 22, OutsideSegment, Accelerate),         // len: 12m
			(22, 27, OutsideSegment, Roll),               // len: 5m
			(27, 44, OutsideSegment, Accelerate),         // len: 17m
			(44, 52, OutsideSegment, Roll),               // len: 8m
			(52, 153, OutsideSegment, Accelerate),        // len: 101m
			(153, 166, OutsideSegment, Roll),             // len: 13m
			(166, 454, OutsideSegment, Accelerate),       // len: 288m
			(454, 474, OutsideSegment, Roll),             // len: 20m
			(474, 3614, OutsideSegment, Accelerate),      // len: 3140m
			(3614, 3672, OutsideSegment, Coast),          // len: 58m
			(3672, 3890, OutsideSegment, Roll),           // len: 218m
			(3890, 4505, OutsideSegment, Brake),          // len: 615m
			(4505, 4723, OutsideSegment, Coast),          // len: 218m
			(4723, 1e6, OutsideSegment, Accelerate));


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
						Assert.AreEqual(exp.Current.pcc, pcc, $"dist {dist}: Wrong PCC state: expected {exp.Current.pcc} instead of {pcc}.");
						Assert.AreEqual(exp.Current.action, action, $"dist {dist}: Wrong DriverAction: expected {exp.Current.action} instead of {action}.");
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