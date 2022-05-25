using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.RoadSweepers
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class RoadSweeperTests
	{
		public const string RoadSweeperJob =
			@"TestData\RoadSweepers\Class9_RigidTruck_6x2_PTO\Class9_RigidTruck_ENG_PTO.vecto";

		public const string SideLoaderJob =
			@"TestData\RoadSweepers\Class9_RigidTruck_6x2_PTO\Class9_RigidTruck_ENG_PTO-SideLoader.vecto";

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase()]
		public void RoadSweeper_StartDriveStop()
		{
			var cycle = "s,v,grad,stop,PTO, Padd, P_PTO \n" +
						"   0, 0, 0, 5, 0, 3, 0   \n" +
						"   1, 8, 0, 0, 2, 3, 10  \n" +
						"1000, 0, 0, 5, 0, 3, 0   \n" +
						"1001, 8, 0, 0, 0, 3, 0   \n" +
						"1200, 0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_Stop_PTO-active");
		}

		[TestCase()]
		public void RoadSweeper_SwitchPTOOffDuringDrive()
		{
			var cycle = "s,v,grad,stop,PTO, Padd, P_PTO \n" + 
						"   0, 0, 0, 5, 0, 3, 0   \n" +
						"   1, 8, 0, 0, 2, 3, 10  \n" +
						" 800, 8, 0, 0, 0, 3, 10  \n" +
						"1000, 0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_switch_PTO_off_during_drive");
		}

		[TestCase()]
		public void RoadSweeper_SwitchPTOOnDuringDrive()
		{
			var cycle = "s,v,grad,stop,PTO, Padd, P_PTO \n" +
						"   0, 0, 0, 5, 0, 3, 0   \n" +
						"   1, 8, 0, 0, 0, 3, 10  \n" +
						" 200, 8, 0, 0, 2, 3, 10  \n" +
						"1000, 0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_switch_PTO_on_during_drive");
		}

		[TestCase()]
		public void RoadSweeper_SwitchPTOOnDuringDriveFromHigherSpeed()
		{
			var cycle = "s,v,grad,stop,PTO, Padd, P_PTO \n" +
						"   0,  0, 0, 5, 0, 3, 0   \n" +
						"   1, 20, 0, 0, 0, 3, 10  \n" +
						" 200,  8, 0, 0, 2, 3, 10  \n" +
						"1000,  0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_switch_PTO_on_during_drive_from_higher_speed");
		}

		[TestCase()]
		public void RoadSweeper_SwitchPTOOnDuringDriveWithHigherSpeed()
		{
			var cycle = "s,v,grad,stop,PTO, Padd, P_PTO \n" +
						"   0,  0, 0, 5, 0, 3, 0   \n" +
						"   1, 20, 0, 0, 0, 3, 10  \n" +
						" 200, 20, 0, 0, 2, 3, 10  \n" +
						"1000,  0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_switch_PTO_on_during_drive_with_higher_speed");
		}


		[TestCase()]
		public void RoadSweeper_ChangePTOLoadDuringSweeping()
		{
			var cycle = "s,v,grad,stop,PTO, Padd, P_PTO \n" +
						"   0,  0, 0, 5, 0, 3, 0   \n" +
						"   1,  8, 0, 0, 2, 3, 10  \n" +
						" 400,  8, 0, 0, 2, 3, 20  \n" +
						"1000,  0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_change_PTO_load_while_sweeping");
		}


		[TestCase()]
		public void TestReadingPTOCycleDuringDrive()
		{
			var cycle = @"TestData\RoadSweepers\Class9_RigidTruck_6x2_PTO\PTO-cycle.vptor";
			var cyleTbl = VectoCSVFile.Read(cycle);

			var cycleData = DrivingCycleDataReader.ReadFromDataTable(cyleTbl, "PTO During Drive", false);

			Assert.AreEqual(7, cycleData.Entries.Count);
		}


		[TestCase()]
		public void SideLoader_PTOLoadDuringDriving()
		{
			var cycle = "s,v,grad,stop,PTO, Padd \n" +
						"   0,  0, 0, 5, 0, 3 \n" +
						"   1,  50, 0, 0, 0, 3 \n" +
						" 400,  50, 0, 0, 3, 3 \n" +
						"1000,   0, 0, 5, 0, 3 \n";
			EngineeringRunWithCycle(SideLoaderJob, cycle, "PTOLoadDuringDriving");
		}

		[TestCase()]
		public void SideLoader_PTOLoadDuringDrivingMultiple()
		{
			var cycle = "s,v,grad,stop,PTO, Padd \n" +
						"   0,  0, 0, 5, 0, 3 \n" +
						"   1,  50, 0, 0, 0, 3 \n" +
						" 400,  50, 0, 0, 3, 3 \n" +
						"1400,  50, 0, 0, 3, 3 \n" +
						"2400,  50, 0, 0, 3, 3 \n" +
						"3400,  50, 0, 0, 3, 3 \n" +
						"4000,   0, 0, 5, 0, 3 \n";
			EngineeringRunWithCycle(SideLoaderJob, cycle, "PTOLoadDuringDrivingMultiple");
		}

		[TestCase()]
		public void SideLoader_PTOLoadDuringDrivingReActivation()
		{
			var cycle = "s,v,grad,stop,PTO, Padd \n" +
						"   0,  0, 0, 5, 0, 3 \n" +
						"   1,  50, 0, 0, 0, 3 \n" +
						" 400,  50, 0, 0, 3, 3 \n" +
						" 450,  50, 0, 0, 3, 3 \n" +
						"1000,   0, 0, 5, 0, 3 \n";
			try {
				EngineeringRunWithCycle(SideLoaderJob, cycle, "PTOLoadDuringDriving_ReActivation");
				Assert.Fail("Exception expected!");
			} catch (Exception e) {
				Assert.IsTrue(e.InnerException.Message.Contains("Additional PTO activation during drive requested while PTO still active!"));
			}
		}


		[TestCase()]
		public void SideLoader_PTOLoadDuringAcceleration()
		{
			var cycle = "s,v,grad,stop,PTO, Padd \n" +
						"   0,  0, 0, 5, 0, 3 \n" +
						"   1,  50, 0, 0, 0, 3 \n" +
						"  50,  50, 0, 0, 3, 3 \n" +
						"1000,   0, 0, 5, 0, 3 \n";
			EngineeringRunWithCycle(SideLoaderJob, cycle, "PTOLoadDuringAcceleration");
		}

		[TestCase()]
		public void SideLoader_PTOLoadDuringDeceleration()
		{
			var cycle = "s,v,grad,stop,PTO, Padd \n" +
						"   0,  0, 0, 5, 0, 3 \n" +
						"   1,  50, 0, 0, 0, 3 \n" +
						" 900,  50, 0, 0, 3, 3 \n" +
						"1000,   0, 0,60, 0, 3 \n";
			EngineeringRunWithCycle(SideLoaderJob, cycle, "PTOLoadDuringDeceleration");
		}

		[TestCase(RoadSweeperJob, 1, TestName = "RoadSweeper 1")]
		public void RoadSweeperTest(string jobFile, int idx)
		{
			EngineeringSimulationRun(idx, jobFile);
		}

		public void EngineeringSimulationRun(int cycleIdx, string jobName)
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;

			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
		}


		public void EngineeringRunWithCycle(string jobFile, string cycleData, string testName)
		{
			var job = (IEngineeringInputDataProvider)JSONInputDataFactory.ReadJsonJob(jobFile);

			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobFile), Path.GetFileName(testName)));

			var cycle = (ICycleData)new CycleInputData() {
				Name = testName,
				CycleData = VectoCSVFile.ReadStream(cycleData.ToStream(), source: testName)
			};

			var inputData = new EngineeringJobInputData() {
				JobType = job.JobInputData.JobType,
				JobName = job.JobInputData.JobName,
				Vehicle = job.JobInputData.Vehicle,
				DriverInputData = job.DriverInputData,
				Cycles = (new[] {cycle}).ToList(),
				PTOCycleWhileDrive = job.JobInputData.Vehicle.Components.PTOTransmissionInputData.PTOCycleWhileDriving,
			};
			

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, writer);
			factory.WriteModalResults = true; //ActualModalData = true,
			factory.Validate = false;

			factory.SumData = sumContainer;

			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat(progress.Select(r => r.Value.Error)));
		}

		public class EngineeringJobInputData : IEngineeringInputDataProvider, IEngineeringJobInputData
		{
			private bool _savedInDeclarationMode = false;

			#region Implementation of IInputDataProvider

			public DataSource DataSource => new DataSource();

			#endregion

			#region Implementation of IEngineeringInputDataProvider

			public IEngineeringJobInputData JobInputData => this;
			public IDriverEngineeringInputData DriverInputData { get; set; }

			#endregion

			#region Implementation of IDriverDeclarationInputData

			public IVehicleEngineeringInputData Vehicle { get; set; }
			public IHybridStrategyParameters HybridStrategyParameters { get; }
			public IList<ICycleData> Cycles { get; set; }
			public VectoSimulationJobType JobType { get; set; }
			public bool EngineOnlyMode => false;
			public IEngineEngineeringInputData EngineOnly => null;
			public TableData PTOCycleWhileDrive { get; set; }

			IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle => Vehicle;

			public string JobName { get; set; }
			public string ShiftStrategy { get; }

			#endregion

			#region Implementation of IDeclarationJobInputData

			bool IDeclarationJobInputData.SavedInDeclarationMode => _savedInDeclarationMode;

			#endregion
		}
	}
}
