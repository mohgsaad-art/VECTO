using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.Tests.Integration
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class ConventionalTimeruns
    {
        private const string GROUP5_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/Group5_Tractor_4x2/Class5_Tractor_ENG.vecto";

		[OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			
		}

		[Category("LongRunning")]
		[Category("Integration")]
        [
			TestCase(GROUP5_JOB, 0, 0, 820.3491, 92.7594, 78.8948, TestName = "Group 5 Conventional TimeRun MeasuredSpeed LongHaul"),
			TestCase(GROUP5_JOB, 1, 1, 946.3962, 80.0195, 56.1634, TestName = "Group 5 Conventional TimeRun MeasuredSpeed RegionalDelivery"),
			TestCase(GROUP5_JOB, 2, 2, 1720.3376, 57.8706, 17.2869, TestName = "Group 5 Conventional TimeRun MeasuredSpeed UrbanDelivery"),
			
			//TestCase(GROUP5_JOB, 6, 0, 814.57, 92.7311, 79.2397, TestName = "Group 5 Conventional TimeRun MeasuredSpeedGear LongHaul"),
			//TestCase(GROUP5_JOB, 7, 1, 943.1614, 80.6104, 56.822, TestName = "Group 5 Conventional TimeRun MeasuredSpeedGear RegionalDelivery"),
			//TestCase(GROUP5_JOB, 8, 2, 1726.7531, 59.1912, 18.0386, TestName = "Group 5 Conventional TimeRun MeasuredSpeedGear UrbanDelivery")
		]
		public void TestTimeRunCycle(string jobFile, int cycleIdx, int distanceCycleIdx, double CO2, double pWheelpos, double pWheel)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SumDataFields.CO2_KM, CO2 },
                { SumDataFields.P_WHEEL_POS, pWheelpos },
                { SumDataFields.P_WHEEL, pWheel }
			};

            RunTimeRunCycle(jobFile, cycleIdx, distanceCycleIdx, metrics);
        }

        [Category("LongRunning")]
		[Category("Integration")]
        [
        TestCase(GROUP5_JOB, 9, 0, 93.0276, 79.3837, 20293.2749, TestName = "Group 5 Conventional TimeRun PWheel LongHaul"),
        TestCase(GROUP5_JOB, 10, 1, 81.1434, 57.0846, 17766.195, TestName = "Group 5 Conventional TimeRun PWheel RegionalDelivery"),
        TestCase(GROUP5_JOB, 11, 2, 60.4794, 18.4924, 13727.1302, TestName = "Group 5 Conventional TimeRun PWheel UrbanDelivery")
        ]
        public void TestPWheelCycle(string jobFile, int cycleIdx, int distanceCycleIdx, double pWheelpos, double pWheel, double fcFinal)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
                { SumDataFields.P_WHEEL_POS, pWheelpos },
                { SumDataFields.P_WHEEL, pWheel },
                { String.Format(SumDataFields.FCFINAL_H, ""), fcFinal }
			};

            RunTimeRunCycle(jobFile, cycleIdx, distanceCycleIdx, metrics);
        }


        [Category("LongRunning")]
        [Category("Integration")]
        [
        TestCase(GROUP5_JOB, 3, TestName = "Group 5 Conventional DistanceRun MeasuredSpeed LongHaul"),
        TestCase(GROUP5_JOB, 4, TestName = "Group 5 Conventional DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(GROUP5_JOB, 5, TestName = "Group 5 Conventional DistanceRun MeasuredSpeed UrbanDelivery")
        ]
        public void TestDistanceRunCycle(string jobFile, int cycleIdx)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SumDataFields.CO2_KM, double.NaN }
			};

            RunDistanceCycle(jobFile, cycleIdx, metrics);
        }

        public void RunTimeRunCycle(string jobFile, int cycleIdx, int distanceCycleIdx, Dictionary<String, double> metrics)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.WriteModalResults = true;
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			
            string distanceSumPath = Path.Combine(Path.GetDirectoryName(jobFile), "distance.vsum");
            AssertHelper.ReportDeviations(distanceSumPath, distanceCycleIdx, factory, metrics);
            
            AssertHelper.AssertMetrics(factory, metrics);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }

        public void RunDistanceCycle(string jobFile, int cycleIdx, Dictionary<String, double> metrics)
        { 
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.WriteModalResults = true;
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			
            string distanceSumPath = Path.Combine(Path.GetDirectoryName(jobFile), "distance.vsum");

            const int DISTANCE_RUN_START_POSITION = 3;

            AssertHelper.ReadMetricsFromVSum(distanceSumPath, cycleIdx - DISTANCE_RUN_START_POSITION, metrics);

            AssertHelper.AssertMetrics(factory, metrics);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }

    }
}
