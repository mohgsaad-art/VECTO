using System;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ShiftStrategyV2Test
	{
		public const string Class9Decl =
			@"TestData\Generic Vehicles\Declaration Mode\Class9_RigidTruck_6x2\Class9_RigidTruck_DECL.vecto";
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();

		}


		[TestCase(@"TestData\Integration\ShiftStrategyV2\Class5_Tractor_4x2\Class5_Tractor_ENG_TCU.vecto"),
		TestCase(@"TestData\Integration\ShiftStrategyV2\Class5_Tractor_4x2\Class5_Tractor_ENG_FC.vecto")]
		public void TestShiftStrategyEngineering(string jobFile, int runIdx = 0)
		{
			var relativeJobPath = jobFile;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml" ? xmlInputReader.CreateDeclaration(relativeJobPath) : JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;

			Assert.NotNull(((IEngineeringInputDataProvider)inputData).DriverInputData.GearshiftInputData);
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToArray();
			//jobContainer.AddRun(runs[runIdx]);
			runs[runIdx].Run();

			Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);
			//jobContainer.Execute();
			//jobContainer.WaitFinished();
			//var progress = jobContainer.GetProgress();
			//Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
			//Assert.IsTrue(jobContainer.Runs.All(r => r.Success), String.Concat<Exception>(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[//TestCase(@"Rigid Truck_4x2_vehicle-class-1_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group1 ALL"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-1_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group1 RD Low"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-1_EURO6_2018.xml", 1, TestName = "AMT ShiftV2 Group1 RD Ref"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-1_EURO6_2018.xml", 2, TestName = "AMT ShiftV2 Group1 UD Low"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-1_EURO6_2018.xml", 3, TestName = "AMT ShiftV2 Group1 UD Ref"),
		]
		public void RunDeclarationTestNewShiftStrategy_Group1(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;

			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}

		[//TestCase(@"Rigid Truck_4x2_vehicle-class-2_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group2 ALL"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-2_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group2 LH Low"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-2_EURO6_2018.xml", 1, TestName = "AMT ShiftV2 Group2 LH Ref"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-2_EURO6_2018.xml", 2, TestName = "AMT ShiftV2 Group2 RD Low"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-2_EURO6_2018.xml", 3, TestName = "AMT ShiftV2 Group2 RD Ref"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-2_EURO6_2018.xml", 4, TestName = "AMT ShiftV2 Group2 UD Low"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-2_EURO6_2018.xml", 5, TestName = "AMT ShiftV2 Group2 UD Ref"),

		]
		public void RunDeclarationTestNewShiftStrategy_Group2(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;

			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}

		[//TestCase(@"Rigid Truck_4x2_vehicle-class-3_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group3 ALL"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-3_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group3 RD Low"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-3_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group3 RD Low"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-3_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group3 UD Low"),
		TestCase(@"Rigid Truck_4x2_vehicle-class-3_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group3 UD Low"),
		]
		public void RunDeclarationTestNewShiftStrategy_Group3(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;

			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}

		[TestCase(@"Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group4 ALL"), 
		//TestCase(@"Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group4 LH Low"),
		//TestCase(@"Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml", 1, TestName = "AMT ShiftV2 Group4 LH Ref"),
		//TestCase(@"Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml", 2, TestName = "AMT ShiftV2 Group4 RD Low"),
		//TestCase(@"Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml", 3, TestName = "AMT ShiftV2 Group4 RD Ref"),
		//TestCase(@"Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml", 4, TestName = "AMT ShiftV2 Group4 UD Low"),
		//TestCase(@"Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml", 5, TestName = "AMT ShiftV2 Group4 UD Ref"),
		//TestCase(@"Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml", 6, TestName = "AMT ShiftV2 Group4 MU Low"),
		//TestCase(@"Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml", 7, TestName = "AMT ShiftV2 Group4 MU Ref"),
			]
		public void RunDeclarationTestNewShiftStrategy_Group4(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;

			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}

		[TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group5 ALL"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group5 LH Low"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 1, TestName = "AMT ShiftV2 Group5 LH Ref"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 2, TestName = "AMT ShiftV2 Group5 LH EMS Low"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 3, TestName = "AMT ShiftV2 Group5 LH EMS Ref"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 4, TestName = "AMT ShiftV2 Group5 RD Low"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 5, TestName = "AMT ShiftV2 Group5 RD Ref"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 6, TestName = "AMT ShiftV2 Group5 RD EMS Low"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 7, TestName = "AMT ShiftV2 Group5 RD EMS Ref"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 8, TestName = "AMT ShiftV2 Group5 UD Low"),
		//TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018.xml", 9, TestName = "AMT ShiftV2 Group5 UD Ref"),
			]
		public void RunDeclarationTestNewShiftStrategy_Group5(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;

			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}

		[TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group9 ALL"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group9 LH Low"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 1, TestName = "AMT ShiftV2 Group9 LH Ref"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 2, TestName = "AMT ShiftV2 Group9 LH EMS Low"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 3, TestName = "AMT ShiftV2 Group9 LH EMS Ref"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 4, TestName = "AMT ShiftV2 Group9 RD Low"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 5, TestName = "AMT ShiftV2 Group9 RD Ref"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 6, TestName = "AMT ShiftV2 Group9 RD EMS Low"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 7, TestName = "AMT ShiftV2 Group9 RD EMS Ref"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 8, TestName = "AMT ShiftV2 Group9 MU Low"),
			//TestCase("Rigid Truck_6x2_vehicle-class-9_EURO6_2018.xml", 9, TestName = "AMT ShiftV2 Group9 MU Ref"),
			]
		public void RunDeclarationTestNewShiftStrategy_Group9(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;

			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}

		[TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group 10 ALL"),
			//TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group10 LH Low"),
			//TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 1, TestName = "AMT ShiftV2 Group10 LH Ref"),
			//TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 2, TestName = "AMT ShiftV2 Group10 LH EMS Los"),
			//TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 3, TestName = "AMT ShiftV2 Group10 LH EMS Ref"),
			//TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 4, TestName = "AMT ShiftV2 Group10 RD Low"),
			TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 5, TestName = "AMT ShiftV2 Group10 RD Ref"),
			//TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 6, TestName = "AMT ShiftV2 Group10 RD EMS Low"),
			//TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 7, TestName = "AMT ShiftV2 Group10 RD EMS Ref"),
		
			// Testing Class 10 on UD cycle - not for production!
			//TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 8, TestName = "AMT ShiftV2 Group10 UD Low"),
			//TestCase("Tractor_6x2_vehicle-class-10_EURO6_2018.xml", 9, TestName = "AMT ShiftV2 Group10 UD Ref"),
			]
		public void RunDeclarationTestNewShiftStrategy_Group10(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;


			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}

		[//TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group 11 ALL"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group11 LH Low"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 1, TestName = "AMT ShiftV2 Group11 LH Ref"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 2, TestName = "AMT ShiftV2 Group11 LH EMS Low"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 3, TestName = "AMT ShiftV2 Group11 LH EMS Ref"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 4, TestName = "AMT ShiftV2 Group11 RD Low"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 5, TestName = "AMT ShiftV2 Group11 RD Ref"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 6, TestName = "AMT ShiftV2 Group11 RD EMS Low"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 7, TestName = "AMT ShiftV2 Group11 RD EMS Ref"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 8, TestName = "AMT ShiftV2 Group11 MU Low"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 9, TestName = "AMT ShiftV2 Group11 MU Ref"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 10, TestName = "AMT ShiftV2 Group11 CO Low"),
		TestCase("Rigid Truck_6x4_vehicle-class-11_EURO6_2018.xml", 11, TestName = "AMT ShiftV2 Group11 CO Ref"),
		]
		public void RunDeclarationTestNewShiftStrategy_Group11(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;


			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}

		[//TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group 12 ALL"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group12 LH Low"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 1, TestName = "AMT ShiftV2 Group12 LH Ref"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 2, TestName = "AMT ShiftV2 Group12 LH EMS Low"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 3, TestName = "AMT ShiftV2 Group12 LH EMS Ref"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 4, TestName = "AMT ShiftV2 Group12 RD Low"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 5, TestName = "AMT ShiftV2 Group12 RD Ref"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 6, TestName = "AMT ShiftV2 Group12 RD EMS Low"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 7, TestName = "AMT ShiftV2 Group12 RD EMS Ref"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 8, TestName = "AMT ShiftV2 Group12 CO Low"),
			TestCase("Tractor_6x4_vehicle-class-12_EURO6_2018.xml", 9, TestName = "AMT ShiftV2 Group12 CO Ref"),
		]
		public void RunDeclarationTestNewShiftStrategy_Group12(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;


			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}


		[//TestCase("Rigid Truck_8x4_vehicle-class-16_EURO6_2018.xml", null, TestName = "AMT ShiftV2 Group16 ALL"),
		TestCase("Rigid Truck_8x4_vehicle-class-16_EURO6_2018.xml", 0, TestName = "AMT ShiftV2 Group16 CO Low"),
		TestCase("Rigid Truck_8x4_vehicle-class-16_EURO6_2018.xml", 1, TestName = "AMT ShiftV2 Group16 CO Ref"),
		]
		public void RunDeclarationTestNewShiftStrategy_Group16(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;


			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}


		[TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018_FlatFCMap.xml", null, TestName = "AMT ShiftV2 Group5 FlatMap ALL"),
		TestCase("Tractor_4x2_vehicle-class-5_EURO6_2018_IncreasingFCMap.xml", null, TestName = "AMT ShiftV2 Group5 IncreasingMap ALL"),
		]
		public void RunDeclarationTestNewShiftStrategy_Group5_FCMapInfluence(string filename, int? idx)
		{
			var job = @"TestData\Integration\ShiftStrategyV2\SampleVehicles\" + filename;

			if (idx.HasValue) {
				RunJob_DeclSingle(job, idx.Value);
			} else {
				RunJob_DeclAll(job);
			}
		}


		[TestCase(@"TestData\Integration\ShiftStrategyV2\CityBus_AT_GSVoith\CityBus_AT_PS.vecto"),
		TestCase(@"TestData\Integration\ShiftStrategyV2\CityBus_AT_GSVoith\CityBus_AT_Ser.vecto"),
		Ignore("Voith Shift strategy no longer maintained")]
		public void RunEngineeringVoith(string jobName)
		{
			RunJob_Engineering(jobName);
		}

		[TestCase(@"TestData\Integration\ShiftStrategyV2\CityBus_AT_FCOpt\CityBus_AT_PS.vecto"),
		TestCase(@"TestData\Integration\ShiftStrategyV2\CityBus_AT_FCOpt\CityBus_AT_Ser.vecto")]
		[Ignore("Review before release.")]
		public void RunEngineeringFCoptimized(string jobName)
		{
			RunJob_Engineering(jobName);
		}


		public void RunJob_Engineering(string jobName)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData =  JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;
			var sumWriter = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumWriter);

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

		}

		public void RunJob_DeclSingle(string jobName, int runIdx)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml" ? xmlInputReader.CreateDeclaration(relativeJobPath) : JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToArray();

			runs[runIdx].Run();

			Assert.IsTrue(runs[runIdx].FinishedWithoutErrors);
		}

		public void RunJob_DeclAll(string jobName)
		{
			var relativeJobPath = jobName;
			var writer = new FileOutputWriter(relativeJobPath);
			var inputData = Path.GetExtension(relativeJobPath) == ".xml" ? xmlInputReader.CreateDeclaration(relativeJobPath) : JSONInputDataFactory.ReadJsonJob(relativeJobPath);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
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
