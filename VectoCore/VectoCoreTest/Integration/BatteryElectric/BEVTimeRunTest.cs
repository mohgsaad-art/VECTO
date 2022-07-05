using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;

namespace TUGraz.VectoCore.Tests.Integration.BatteryElectric
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class BEVTimeRunTest 
    {
        private const string E2_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicleB2\BEV_ENG.vecto";
        private const string E3_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicleB3\BEV_ENG.vecto";
        private const string E4_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicleB4\BEV_ENG.vecto";

        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Category("JRC")]
		[Category("Integration")]
        [
        TestCase(E2_JOB, 0, 0, 1.1667, 120.2219, TestName = "E2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E2_JOB, 1, 1, 3.9969, 120.1189, TestName = "E2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E2_JOB, 2, 2, 26.2759, 143.7661, TestName = "E2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E2_JOB, 6, 0, 1.128, 120.0374, TestName = "E2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(E2_JOB, 7, 1, 4.3124, 117.0889, TestName = "E2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(E2_JOB, 8, 2, 28.7993, 125.4735, TestName = "E2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(E2_JOB, 9, 0, 1.1661, 120.0129, TestName = "E2 BEV TimeRun MeasuredSpeedPWheel LongHaul"),
        TestCase(E2_JOB, 10, 1, 4.6925, 117.3748, TestName = "E2 BEV TimeRun MeasuredSpeedPWheel RegionalDelivery"),
        TestCase(E2_JOB, 11, 2, 31.7336, 127.926, TestName = "E2 BEV TimeRun MeasuredSpeedPWheel UrbanDelivery"),

        TestCase(E3_JOB, 0, 0, 0.7916, 101.6194, TestName = "E3 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E3_JOB, 1, 1, 3.3777, 104.792, TestName = "E3 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E3_JOB, 2, 2, 28.1637, 128.3685, TestName = "E3 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E3_JOB, 6, 0, 0.8476, 101.5911, TestName = "E3 BEV TimeRun MeasuredSpeedPWheel LongHaul"),
        TestCase(E3_JOB, 7, 1, 3.611, 104.7354, TestName = "E3 BEV TimeRun MeasuredSpeedPWheel RegionalDelivery"),
        TestCase(E3_JOB, 8, 2, 30.5738, 127.6631, TestName = "E3 BEV TimeRun MeasuredSpeedPWheel UrbanDelivery"),

        TestCase(E4_JOB, 0, 0, 0.9165, 97.3067, TestName = "E4 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E4_JOB, 1, 1, 3.7896, 100.3712, TestName = "E4 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E4_JOB, 2, 2, 30.1342, 123.7528, TestName = "E4 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E4_JOB, 6, 0, 0.9741, 97.2793, TestName = "E4 BEV TimeRun MeasuredSpeedPWheel LongHaul"),
        TestCase(E4_JOB, 7, 1, 4.0286, 100.3147, TestName = "E4 BEV TimeRun MeasuredSpeedPWheel RegionalDelivery"),
        TestCase(E4_JOB, 8, 2, 32.6304, 123.0436, TestName = "E4 BEV TimeRun MeasuredSpeedPWheel UrbanDelivery")
        ]
        public void TestBEVTimeRunCycle(string jobFile, int cycleIdx, int distanceCycleIdx, double charge, double discharge)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
                { SummaryDataContainer.Fields.E_REESS_T_chg, charge },
                { SummaryDataContainer.Fields.E_REESS_T_dischg, discharge }
			};

            RunBEVTimeRunCycle(jobFile, cycleIdx, distanceCycleIdx, metrics);
        }

        [Category("TUG-update")]
        [Category("JRC")]
        [Category("LongRunning")]
        [Category("Integration")]
        [
        TestCase(E2_JOB, 3, TestName = "E2 BEV DistanceRun MeasuredSpeed LongHaul"),
        TestCase(E2_JOB, 4, TestName = "E2 BEV DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(E2_JOB, 5, TestName = "E2 BEV DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(E3_JOB, 3, TestName = "E3 BEV DistanceRun MeasuredSpeed LongHaul"),
        TestCase(E3_JOB, 4, TestName = "E3 BEV DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(E3_JOB, 5, TestName = "E3 BEV DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(E4_JOB, 3, TestName = "E4 BEV DistanceRun MeasuredSpeed LongHaul"),
        TestCase(E4_JOB, 4, TestName = "E4 BEV DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(E4_JOB, 5, TestName = "E4 BEV DistanceRun MeasuredSpeed UrbanDelivery")
        ]
        public void TestBEVDistanceRunCycle(string jobFile, int cycleIdx)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SummaryDataContainer.Fields.E_REESS_T_chg, double.NaN },
                { SummaryDataContainer.Fields.E_REESS_T_dischg, double.NaN }
			};

            RunBEVDistanceRunCycle(jobFile, cycleIdx, metrics);
        }

        public void RunBEVTimeRunCycle(string jobFile, int cycleIdx, int distanceCycleIdx, Dictionary<String, double> metrics)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = new SimulatorFactoryEngineering(inputProvider, writer, false) { WriteModalResults = true };
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			
            string distanceSumPath = Path.Combine(Path.GetDirectoryName(jobFile), "distance.vsum");
            AssertHelper.ReportDeviations(distanceSumPath, distanceCycleIdx, factory, metrics);
            
            AssertHelper.AssertMetrics(factory, metrics);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }

        public void RunBEVDistanceRunCycle(string jobFile, int cycleIdx, Dictionary<String, double> metrics)
        { 
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = new SimulatorFactoryEngineering(inputProvider, writer, false) { WriteModalResults = true };
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			
            string distanceSumPath = Path.Combine(Path.GetDirectoryName(jobFile), "distance.vsum");

            const int DISTANCE_RUN_START_POSITION = 3;

            AssertHelper.ReadMetricsFromVSum(distanceSumPath, cycleIdx - DISTANCE_RUN_START_POSITION, metrics);

            AssertHelper.AssertMetrics(factory, metrics);

			//Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }
    }
}
