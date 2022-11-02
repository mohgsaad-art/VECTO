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
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.PCCStates;
using static TUGraz.VectoCore.Models.SimulationComponent.Impl.DrivingAction;

namespace TUGraz.VectoCore.Tests.Integration.ADAS
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ADASTestsPEV
	{
		private const string BasePath = @"TestData\Integration\ADAS-PEV\Group5PCCEng\";
		private const double tolerance = 1; //seconds of tolerance. Tolerance distance is calculated dynamically based on speed.

		[OneTimeSetUp]
		public void RunBeforeAnyTests() => Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

		[TestCase]
		public void TestVECTO_1483()
		{
			var jobName = @"TestData\Integration\ADAS-PEV\VECTO-1483\E4_Group 5 LH_ll.vecto";
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



		#region E2

		[TestCase]
		public void Class5_E2_NoADAS_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5647, OutsideSegment, Accelerate),        // len: 5647m
			(5647, 5846, OutsideSegment, Coast),          // len: 199m
			(5846, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_NoADAS_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 6196, OutsideSegment, Accelerate),        // len: 6196m
			(6196, 7628, OutsideSegment, Coast),          // len: 1432m
			(7628, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5496, OutsideSegment, Accelerate),        // len: 5496m
			(5496, 5849, OutsideSegment, Coast),          // len: 353m
			(5849, 7111, OutsideSegment, Brake),          // len: 1262m
			(7111, 7420, OutsideSegment, Coast),          // len: 309m
			(7420, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_NoADAS_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2147, OutsideSegment, Accelerate),        // len: 2147m
			(2147, 2300, OutsideSegment, Coast),          // len: 153m
			(2300, 2469, OutsideSegment, Brake),          // len: 169m
			(2469, 2599, OutsideSegment, Coast),          // len: 130m
			(2599, 5551, OutsideSegment, Accelerate),     // len: 2952m
			(5551, 5917, OutsideSegment, Coast),          // len: 366m
			(5917, 6649, OutsideSegment, Brake),          // len: 732m
			(6649, 6947, OutsideSegment, Coast),          // len: 298m
			(6947, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2194, OutsideSegment, Accelerate),        // len: 2194m
			(2194, 2440, OutsideSegment, Coast),          // len: 246m
			(2440, 3350, OutsideSegment, Accelerate),     // len: 910m
			(3350, 3598, OutsideSegment, Coast),          // len: 248m
			(3598, 3946, OutsideSegment, Brake),          // len: 348m
			(3946, 4208, OutsideSegment, Coast),          // len: 262m
			(4208, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 2206, OutsideSegment, Accelerate),        // len: 2206m
			(2206, 2463, OutsideSegment, Coast),          // len: 257m
			(2463, 2907, OutsideSegment, Accelerate),     // len: 444m
			(2907, 3400, OutsideSegment, Coast),          // len: 493m
			(3400, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5437, OutsideSegment, Accelerate),        // len: 5437m
			(5437, 5614, OutsideSegment, Coast),          // len: 177m
			(5614, 6467, OutsideSegment, Brake),          // len: 853m
			(6467, 6990, OutsideSegment, Coast),          // len: 523m
			(6990, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 5332, OutsideSegment, Accelerate),        // len: 5332m
			(5332, 5722, OutsideSegment, Coast),          // len: 390m
			(5722, 5962, OutsideSegment, Brake),          // len: 240m
			(5962, 6224, OutsideSegment, Coast),          // len: 262m
			(6224, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4794, OutsideSegment, Accelerate),        // len: 4794m
			(4794, 4805, OutsideSegment, Roll),           // len: 11m
			(4805, 4837, OutsideSegment, Accelerate),     // len: 32m
			(4837, 4845, OutsideSegment, Roll),           // len: 8m
			(4845, 4881, OutsideSegment, Accelerate),     // len: 36m
			(4881, 4887, OutsideSegment, Roll),           // len: 6m
			(4887, 5088, OutsideSegment, Accelerate),     // len: 201m
			(5088, 5103, OutsideSegment, Roll),           // len: 15m
			(5103, 5143, OutsideSegment, Accelerate),     // len: 40m
			(5143, 5159, OutsideSegment, Roll),           // len: 16m
			(5159, 5431, OutsideSegment, Accelerate),     // len: 272m
			(5431, 5526, OutsideSegment, Coast),          // len: 95m
			(5526, 6090, OutsideSegment, Brake),          // len: 564m
			(6090, 6364, OutsideSegment, Coast),          // len: 274m
			(6364, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_NoADAS_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 13, OutsideSegment, Accelerate),          // len: 13m
			//(13, 18, OutsideSegment, Roll),               // len: 5m
			//(18, 36, OutsideSegment, Accelerate),         // len: 18m
			//(36, 43, OutsideSegment, Roll),               // len: 7m
			//(43, 78, OutsideSegment, Accelerate),         // len: 35m
			//(78, 87, OutsideSegment, Roll),               // len: 9m
			//(87, 169, OutsideSegment, Accelerate),        // len: 82m
			//(169, 182, OutsideSegment, Roll),             // len: 13m
			//(182, 278, OutsideSegment, Accelerate),       // len: 96m
			//(278, 295, OutsideSegment, Roll),             // len: 17m
			(500, 3809, OutsideSegment, Accelerate),      // len: 3514m
			(3809, 4568, OutsideSegment, Coast),          // len: 759m
			(4568, 5001, OutsideSegment, Brake),          // len: 433m
			(5001, 5408, OutsideSegment, Coast),          // len: 407m
			(5408, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_NoADAS_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 13, OutsideSegment, Accelerate),          // len: 13m
			//(13, 18, OutsideSegment, Roll),               // len: 5m
			//(18, 36, OutsideSegment, Accelerate),         // len: 18m
			//(36, 43, OutsideSegment, Roll),               // len: 7m
			//(43, 78, OutsideSegment, Accelerate),         // len: 35m
			//(78, 87, OutsideSegment, Roll),               // len: 9m
			//(87, 169, OutsideSegment, Accelerate),        // len: 82m
			//(169, 182, OutsideSegment, Roll),             // len: 13m
			//(182, 278, OutsideSegment, Accelerate),       // len: 96m
			//(278, 295, OutsideSegment, Roll),             // len: 17m
			(500, 3615, OutsideSegment, Accelerate),      // len: 3320m
			(3615, 4248, OutsideSegment, Coast),          // len: 633m
			(4248, 4509, OutsideSegment, Brake),          // len: 261m
			(4509, 4717, OutsideSegment, Coast),          // len: 208m
			(4717, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4107, OutsideSegment, Accelerate),        // len: 4107m
			(4107, 5566, WithinSegment, Accelerate),      // len: 1459m
			(5566, 5834, UseCase1, Coast),                // len: 268m
			(5834, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4585, OutsideSegment, Accelerate),        // len: 4585m
			(4585, 5682, WithinSegment, Accelerate),      // len: 1097m
			(5682, 7315, UseCase1, Coast),                // len: 1633m
			(7315, 7385, OutsideSegment, Coast),          // len: 70m
			(7385, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3955, OutsideSegment, Accelerate),        // len: 3955m
			(3955, 4982, WithinSegment, Accelerate),      // len: 1027m
			(4982, 6112, UseCase1, Coast),                // len: 1130m
			(6112, 6330, WithinSegment, Coast),           // len: 218m
			(6330, 7096, WithinSegment, Brake),           // len: 766m
			(7096, 7183, WithinSegment, Coast),           // len: 87m
			(7183, 7533, OutsideSegment, Coast),          // len: 350m
			(7533, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1949, WithinSegment, Accelerate),       // len: 1295m
			(1949, 2483, UseCase1, Coast),                // len: 534m
			(2483, 2541, OutsideSegment, Coast),          // len: 58m
			(2541, 4011, OutsideSegment, Accelerate),     // len: 1470m
			(4011, 5003, WithinSegment, Accelerate),      // len: 992m
			(5003, 6235, UseCase1, Coast),                // len: 1232m
			(6235, 6707, WithinSegment, Coast),           // len: 472m
			(6707, 7007, OutsideSegment, Coast),          // len: 300m
			(7007, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2112, WithinSegment, Accelerate),       // len: 1423m
			(2112, 2380, UseCase1, Coast),                // len: 268m
			(2380, 3033, WithinSegment, Accelerate),      // len: 653m
			(3033, 3840, UseCase1, Coast),                // len: 807m
			(3840, 3982, WithinSegment, Coast),           // len: 142m
			(3982, 4147, OutsideSegment, Coast),          // len: 165m
			(4147, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2124, WithinSegment, Accelerate),       // len: 1435m
			(2124, 2403, UseCase1, Coast),                // len: 279m
			(2403, 2741, WithinSegment, Accelerate),      // len: 338m
			(2741, 3286, UseCase1, Coast),                // len: 545m
			(3286, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5134, WithinSegment, Accelerate),      // len: 1202m
			(5134, 5917, UseCase1, Coast),                // len: 783m
			(5917, 6621, WithinSegment, Coast),           // len: 704m
			(6621, 7124, OutsideSegment, Coast),          // len: 503m
			(7124, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3804, OutsideSegment, Accelerate),        // len: 3804m
			(3804, 4982, WithinSegment, Accelerate),      // len: 1178m
			(4982, 6011, UseCase1, Coast),                // len: 1029m
			(6011, 6105, OutsideSegment, Coast),          // len: 94m
			(6105, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4794, WithinSegment, Accelerate),      // len: 1247m
			(4794, 4805, WithinSegment, Roll),            // len: 11m
			(4805, 4837, WithinSegment, Accelerate),      // len: 32m
			(4837, 4845, WithinSegment, Roll),            // len: 8m
			(4845, 4881, WithinSegment, Accelerate),      // len: 36m
			(4881, 4887, WithinSegment, Roll),            // len: 6m
			(4887, 5088, WithinSegment, Accelerate),      // len: 201m
			(5088, 5103, WithinSegment, Roll),            // len: 15m
			(5103, 5143, WithinSegment, Accelerate),      // len: 40m
			(5143, 5159, WithinSegment, Roll),            // len: 16m
			(5159, 5330, WithinSegment, Accelerate),      // len: 171m
			(5330, 5679, UseCase2, Coast),                // len: 349m
			(5679, 6143, WithinSegment, Coast),           // len: 464m
			(6143, 6469, OutsideSegment, Coast),          // len: 326m
			(6469, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 13, OutsideSegment, Accelerate),          // len: 13m
			//(13, 18, OutsideSegment, Roll),               // len: 5m
			//(18, 36, OutsideSegment, Accelerate),         // len: 18m
			//(36, 43, OutsideSegment, Roll),               // len: 7m
			//(43, 78, OutsideSegment, Accelerate),         // len: 35m
			//(78, 87, OutsideSegment, Roll),               // len: 9m
			//(87, 169, OutsideSegment, Accelerate),        // len: 82m
			//(169, 182, OutsideSegment, Roll),             // len: 13m
			//(182, 278, OutsideSegment, Accelerate),       // len: 96m
			//(278, 295, OutsideSegment, Roll),             // len: 17m
			(500, 3809, OutsideSegment, Accelerate),      // len: 3514m
			(3809, 4568, OutsideSegment, Coast),          // len: 759m
			(4568, 5001, OutsideSegment, Brake),          // len: 433m
			(5001, 5408, OutsideSegment, Coast),          // len: 407m
			(5408, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 13, OutsideSegment, Accelerate),          // len: 13m
			//(13, 18, OutsideSegment, Roll),               // len: 5m
			//(18, 36, OutsideSegment, Accelerate),         // len: 18m
			//(36, 43, OutsideSegment, Roll),               // len: 7m
			//(43, 78, OutsideSegment, Accelerate),         // len: 35m
			//(78, 87, OutsideSegment, Roll),               // len: 9m
			//(87, 169, OutsideSegment, Accelerate),        // len: 82m
			//(169, 182, OutsideSegment, Roll),             // len: 13m
			//(182, 278, OutsideSegment, Accelerate),       // len: 96m
			//(278, 295, OutsideSegment, Roll),             // len: 17m
			(500, 3615, OutsideSegment, Accelerate),      // len: 3320m
			(3615, 4248, OutsideSegment, Coast),          // len: 633m
			(4248, 4509, OutsideSegment, Brake),          // len: 261m
			(4509, 4717, OutsideSegment, Coast),          // len: 208m
			(4717, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E2_PCC12_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4107, OutsideSegment, Accelerate),        // len: 4107m
			(4107, 5566, WithinSegment, Accelerate),      // len: 1459m
			(5566, 5834, UseCase1, Coast),                // len: 268m
			(5834, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4585, OutsideSegment, Accelerate),        // len: 4585m
			(4585, 5682, WithinSegment, Accelerate),      // len: 1097m
			(5682, 7315, UseCase1, Coast),                // len: 1633m
			(7315, 7385, OutsideSegment, Coast),          // len: 70m
			(7385, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3955, OutsideSegment, Accelerate),        // len: 3955m
			(3955, 4982, WithinSegment, Accelerate),      // len: 1027m
			(4982, 6112, UseCase1, Coast),                // len: 1130m
			(6112, 6183, WithinSegment, Coast),           // len: 71m
			(6183, 7108, WithinSegment, Brake),           // len: 925m
			(7108, 7181, WithinSegment, Coast),           // len: 73m
			(7181, 7418, OutsideSegment, Coast),          // len: 237m
			(7418, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1949, WithinSegment, Accelerate),       // len: 1295m
			(1949, 2483, UseCase1, Coast),                // len: 534m
			(2483, 2541, OutsideSegment, Coast),          // len: 58m
			(2541, 4011, OutsideSegment, Accelerate),     // len: 1470m
			(4011, 5003, WithinSegment, Accelerate),      // len: 992m
			(5003, 6235, UseCase1, Coast),                // len: 1232m
			(6235, 6367, WithinSegment, Coast),           // len: 132m
			(6367, 6643, WithinSegment, Brake),           // len: 276m
			(6643, 6715, WithinSegment, Coast),           // len: 72m
			(6715, 6941, OutsideSegment, Coast),          // len: 226m
			(6941, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2112, WithinSegment, Accelerate),       // len: 1423m
			(2112, 2380, UseCase1, Coast),                // len: 268m
			(2380, 3033, WithinSegment, Accelerate),      // len: 653m
			(3033, 3840, UseCase1, Coast),                // len: 807m
			(3840, 3982, WithinSegment, Coast),           // len: 142m
			(3982, 4147, OutsideSegment, Coast),          // len: 165m
			(4147, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2124, WithinSegment, Accelerate),       // len: 1435m
			(2124, 2403, UseCase1, Coast),                // len: 279m
			(2403, 2741, WithinSegment, Accelerate),      // len: 338m
			(2741, 3286, UseCase1, Coast),                // len: 545m
			(3286, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC12_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3932, OutsideSegment, Accelerate),        // len: 3932m
			(3932, 5134, WithinSegment, Accelerate),      // len: 1202m
			(5134, 5917, UseCase1, Coast),                // len: 783m
			(5917, 6036, WithinSegment, Coast),           // len: 119m
			(6036, 6468, WithinSegment, Brake),           // len: 432m
			(6468, 6624, WithinSegment, Coast),           // len: 156m
			(6624, 6980, OutsideSegment, Coast),          // len: 356m
			(6980, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3804, OutsideSegment, Accelerate),        // len: 3804m
			(3804, 4982, WithinSegment, Accelerate),      // len: 1178m
			(4982, 6011, UseCase1, Coast),                // len: 1029m
			(6011, 6105, OutsideSegment, Coast),          // len: 94m
			(6105, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E2_PCC12_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3547, OutsideSegment, Accelerate),        // len: 3547m
			(3547, 4794, WithinSegment, Accelerate),      // len: 1247m
			(4794, 4805, WithinSegment, Roll),            // len: 11m
			(4805, 4837, WithinSegment, Accelerate),      // len: 32m
			(4837, 4845, WithinSegment, Roll),            // len: 8m
			(4845, 4881, WithinSegment, Accelerate),      // len: 36m
			(4881, 4887, WithinSegment, Roll),            // len: 6m
			(4887, 5088, WithinSegment, Accelerate),      // len: 201m
			(5088, 5103, WithinSegment, Roll),            // len: 15m
			(5103, 5143, WithinSegment, Accelerate),      // len: 40m
			(5143, 5159, WithinSegment, Roll),            // len: 16m
			(5159, 5330, WithinSegment, Accelerate),      // len: 171m
			(5330, 5679, UseCase2, Coast),                // len: 349m
			(5679, 5751, WithinSegment, Coast),           // len: 72m
			(5751, 6087, WithinSegment, Brake),           // len: 336m
			(6087, 6135, WithinSegment, Coast),           // len: 48m
			(6135, 6373, OutsideSegment, Coast),          // len: 238m
			(6373, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E2_PCC12_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 13, OutsideSegment, Accelerate),          // len: 13m
			//(13, 18, OutsideSegment, Roll),               // len: 5m
			//(18, 36, OutsideSegment, Accelerate),         // len: 18m
			//(36, 43, OutsideSegment, Roll),               // len: 7m
			//(43, 78, OutsideSegment, Accelerate),         // len: 35m
			//(78, 87, OutsideSegment, Roll),               // len: 9m
			//(87, 169, OutsideSegment, Accelerate),        // len: 82m
			//(169, 182, OutsideSegment, Roll),             // len: 13m
			//(182, 278, OutsideSegment, Accelerate),       // len: 96m
			//(278, 295, OutsideSegment, Roll),             // len: 17m
			(500, 3809, OutsideSegment, Accelerate),      // len: 3514m
			(3809, 4568, OutsideSegment, Coast),          // len: 759m
			(4568, 5001, OutsideSegment, Brake),          // len: 433m
			(5001, 5408, OutsideSegment, Coast),          // len: 407m
			(5408, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E2_PCC12_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			//(0, 0, OutsideSegment, Halt),                 // len: 0m
			//(0, 13, OutsideSegment, Accelerate),          // len: 13m
			//(13, 18, OutsideSegment, Roll),               // len: 5m
			//(18, 36, OutsideSegment, Accelerate),         // len: 18m
			//(36, 43, OutsideSegment, Roll),               // len: 7m
			//(43, 78, OutsideSegment, Accelerate),         // len: 35m
			//(78, 87, OutsideSegment, Roll),               // len: 9m
			//(87, 169, OutsideSegment, Accelerate),        // len: 82m
			//(169, 182, OutsideSegment, Roll),             // len: 13m
			//(182, 278, OutsideSegment, Accelerate),       // len: 96m
			//(278, 295, OutsideSegment, Roll),             // len: 17m
			(500, 3615, OutsideSegment, Accelerate),      // len: 3320m
			(3615, 4248, OutsideSegment, Coast),          // len: 633m
			(4248, 4509, OutsideSegment, Brake),          // len: 261m
			(4509, 4717, OutsideSegment, Coast),          // len: 208m
			(4717, 1e6, OutsideSegment, Accelerate));


		#endregion

		#region E3

		[TestCase]
		public void Class5_E3_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5531, WithinSegment, Accelerate),      // len: 1401m
			(5531, 5810, UseCase1, Coast),                // len: 279m
			(5810, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5612, WithinSegment, Accelerate),      // len: 957m
			(5612, 7240, UseCase1, Coast),                // len: 1628m
			(7240, 7498, OutsideSegment, Coast),          // len: 258m
			(7498, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4971, WithinSegment, Accelerate),      // len: 992m
			(4971, 6090, UseCase1, Coast),                // len: 1119m
			(6090, 6296, WithinSegment, Coast),           // len: 206m
			(6296, 7112, WithinSegment, Brake),           // len: 816m
			(7112, 7149, WithinSegment, Coast),           // len: 37m
			(7149, 7548, OutsideSegment, Coast),          // len: 399m
			(7548, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1926, WithinSegment, Accelerate),       // len: 1272m
			(1926, 2479, UseCase1, Coast),                // len: 553m
			(2479, 2526, OutsideSegment, Coast),          // len: 47m
			(2526, 4043, OutsideSegment, Accelerate),     // len: 1517m
			(4043, 4988, WithinSegment, Accelerate),      // len: 945m
			(4988, 6198, UseCase1, Coast),                // len: 1210m
			(6198, 6684, WithinSegment, Coast),           // len: 486m
			(6684, 7046, OutsideSegment, Coast),          // len: 362m
			(7046, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2101, WithinSegment, Accelerate),       // len: 1401m
			(2101, 2379, UseCase1, Coast),                // len: 278m
			(2379, 2391, WithinSegment, Coast),           // len: 12m
			(2391, 3009, WithinSegment, Accelerate),      // len: 618m
			(3009, 3868, UseCase1, Coast),                // len: 859m
			(3868, 3975, WithinSegment, Coast),           // len: 107m
			(3975, 4152, OutsideSegment, Coast),          // len: 177m
			(4152, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2112, WithinSegment, Accelerate),       // len: 1412m
			(2112, 2391, UseCase1, Coast),                // len: 279m
			(2391, 2426, WithinSegment, Coast),           // len: 35m
			(2426, 2718, WithinSegment, Accelerate),      // len: 292m
			(2718, 3262, UseCase1, Coast),                // len: 544m
			(3262, 3321, OutsideSegment, Coast),          // len: 59m
			(3321, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3944, OutsideSegment, Accelerate),        // len: 3944m
			(3944, 5122, WithinSegment, Accelerate),      // len: 1178m
			(5122, 5893, UseCase1, Coast),                // len: 771m
			(5893, 6551, WithinSegment, Coast),           // len: 658m
			(6551, 7204, OutsideSegment, Coast),          // len: 653m
			(7204, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4947, WithinSegment, Accelerate),      // len: 1132m
			(4947, 5996, UseCase1, Coast),                // len: 1049m
			(5996, 6125, OutsideSegment, Coast),          // len: 129m
			(6125, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CaseJ() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3559, OutsideSegment, Accelerate),        // len: 3559m
			(3559, 5374, WithinSegment, Accelerate),      // len: 1815m
			(5374, 5713, UseCase2, Coast),                // len: 339m
			(5713, 6114, WithinSegment, Coast),           // len: 401m
			(6114, 6477, OutsideSegment, Coast),          // len: 363m
			(6477, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E3_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3410, OutsideSegment, Accelerate),        // len: 3410m
			(3410, 4441, OutsideSegment, Coast),          // len: 1031m
			(4441, 5005, OutsideSegment, Brake),          // len: 564m
			(5005, 5422, OutsideSegment, Coast),          // len: 417m
			(5422, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E3_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3614, OutsideSegment, Accelerate),        // len: 3614m
			(3614, 4148, OutsideSegment, Coast),          // len: 534m
			(4148, 4510, OutsideSegment, Brake),          // len: 362m
			(4510, 4718, OutsideSegment, Coast),          // len: 208m
			(4718, 1e6, OutsideSegment, Accelerate));

		#endregion

		#region E4

		[TestCase]
		public void Class5_E4_PCC123_CaseA() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4130, OutsideSegment, Accelerate),        // len: 4130m
			(4130, 5531, WithinSegment, Accelerate),      // len: 1401m
			(5531, 5810, UseCase1, Coast),                // len: 279m
			(5810, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E4_PCC123_CaseB() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 4655, OutsideSegment, Accelerate),        // len: 4655m
			(4655, 5601, WithinSegment, Accelerate),      // len: 946m
			(5601, 7252, UseCase1, Coast),                // len: 1651m
			(7252, 7522, OutsideSegment, Coast),          // len: 270m
			(7522, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseC() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3979, OutsideSegment, Accelerate),        // len: 3979m
			(3979, 4959, WithinSegment, Accelerate),      // len: 980m
			(4959, 6088, UseCase1, Coast),                // len: 1129m
			(6088, 6293, WithinSegment, Coast),           // len: 205m
			(6293, 7121, WithinSegment, Brake),           // len: 828m
			(7121, 7158, WithinSegment, Coast),           // len: 37m
			(7158, 7523, OutsideSegment, Coast),          // len: 365m
			(7523, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseD() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 654, OutsideSegment, Accelerate),         // len: 654m
			(654, 1926, WithinSegment, Accelerate),       // len: 1272m
			(1926, 2480, UseCase1, Coast),                // len: 554m
			(2480, 2538, OutsideSegment, Coast),          // len: 58m
			(2538, 4040, OutsideSegment, Accelerate),     // len: 1502m
			(4040, 4983, WithinSegment, Accelerate),      // len: 943m
			(4983, 6193, UseCase1, Coast),                // len: 1210m
			(6193, 6691, WithinSegment, Coast),           // len: 498m
			(6691, 7054, OutsideSegment, Coast),          // len: 363m
			(7054, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E4_PCC123_CaseE() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 689, OutsideSegment, Accelerate),         // len: 689m
			(689, 2101, WithinSegment, Accelerate),       // len: 1412m
			(2101, 2379, UseCase1, Coast),                // len: 278m
			(2379, 2403, WithinSegment, Coast),           // len: 24m
			(2403, 3008, WithinSegment, Accelerate),      // len: 605m
			(3008, 3888, UseCase1, Coast),                // len: 880m
			(3888, 3971, WithinSegment, Coast),           // len: 83m
			(3971, 4147, OutsideSegment, Coast),          // len: 176m
			(4147, 1e6, OutsideSegment, Accelerate));


		[TestCase]
		public void Class5_E4_PCC123_CaseF() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 700, OutsideSegment, Accelerate),         // len: 700m
			(700, 2112, WithinSegment, Accelerate),       // len: 1412m
			(2112, 2391, UseCase1, Coast),                // len: 279m
			(2391, 2438, WithinSegment, Coast),           // len: 47m
			(2438, 2706, WithinSegment, Accelerate),      // len: 268m
			(2706, 3273, UseCase1, Coast),                // len: 567m
			(3273, 3320, OutsideSegment, Coast),          // len: 47m
			(3320, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseG() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3941, OutsideSegment, Accelerate),        // len: 3941m
			(3941, 5117, WithinSegment, Accelerate),      // len: 1176m
			(5117, 5888, UseCase1, Coast),                // len: 771m
			(5888, 6559, WithinSegment, Coast),           // len: 671m
			(6559, 7224, OutsideSegment, Coast),          // len: 665m
			(7224, 1e6, OutsideSegment, Accelerate));



		[TestCase]
		public void Class5_E4_PCC123_CaseH() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3815, OutsideSegment, Accelerate),        // len: 3815m
			(3815, 4936, WithinSegment, Accelerate),      // len: 1121m
			(4936, 5994, UseCase1, Coast),                // len: 1058m
			(5994, 6135, OutsideSegment, Coast),          // len: 141m
			(6135, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseI() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CaseJE4() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 3430, OutsideSegment, Accelerate),        // len: 3430m
			(3430, 5227, WithinSegment, Accelerate),      // len: 1797m
			(5227, 6181, UseCase2, Coast),                // len: 954m
			(6181, 6431, WithinSegment, Coast),           // len: 250m
			(6431, 6538, OutsideSegment, Coast),          // len: 107m
			(6538, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CrestCoast1() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3417, OutsideSegment, Accelerate),        // len: 3417m
			(3417, 4400, OutsideSegment, Coast),          // len: 983m
			(4400, 5004, OutsideSegment, Brake),          // len: 604m
			(5004, 5422, OutsideSegment, Coast),          // len: 418m
			(5422, 1e6, OutsideSegment, Accelerate));

		[TestCase]
		public void Class5_E4_PCC123_CrestCoast2() => TestPCC(MethodBase.GetCurrentMethod().Name.Split('_').Slice(0, -1).Join("_"), MethodBase.GetCurrentMethod().Name.Split('_').Last(),
			(0, 0, OutsideSegment, Halt),                 // len: 0m
			(0, 3612, OutsideSegment, Accelerate),        // len: 3612m
			(3612, 4126, OutsideSegment, Coast),          // len: 514m
			(4126, 4509, OutsideSegment, Brake),          // len: 383m
			(4509, 4727, OutsideSegment, Coast),          // len: 218m
			(4727, 1e6, OutsideSegment, Accelerate));

		#endregion

		private void TestPCC(string jobName, string cycleName, params (double start, double end, PCCStates pcc, DrivingAction action)[] data)
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
