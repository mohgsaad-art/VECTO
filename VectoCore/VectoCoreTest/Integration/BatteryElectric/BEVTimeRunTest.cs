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
        private const string E2_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicleE2\BEV_ENG.vecto";
        private const string E3_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicleE3\BEV_ENG.vecto";
        private const string E4_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicleE4\BEV_ENG.vecto";
        private const string IEPC3X_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericIEPC\IEPC_Gbx3Speed\IEPC_ENG_Gbx3.vecto";
        private const string IEPC3X_AXLE_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericIEPC\IEPC_Gbx3Speed+Axle\IEPC_ENG_Gbx3Axl.vecto";
        private const string IEPC3X_WHEEL1_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericIEPC\IEPC_Gbx3Speed-Whl1\IEPC_ENG_Gbx3Whl1.vecto";
        private const string IEPC3X_WHEEL2_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericIEPC\IEPC_Gbx3Speed-Whl2\IEPC_ENG_Gbx3Whl2.vecto";

        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Category("JRC")]
		[Category("Integration")]
        [
        TestCase(E2_JOB, 0, 0, 1.1278, 120.0339, TestName = "E2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E2_JOB, 1, 1, 4.2989, 117.0439, TestName = "E2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E2_JOB, 2, 2, 28.742, 125.2617, TestName = "E2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E2_JOB, 6, 0, 1.128, 120.0309, TestName = "E2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(E2_JOB, 7, 1, 4.309, 117.0281, TestName = "E2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(E2_JOB, 8, 2, 28.8188, 125.1815, TestName = "E2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(E2_JOB, 9, 0, 1.155, 119.9847, TestName = "E2 BEV TimeRun PWheel LongHaul"),
        TestCase(E2_JOB, 10, 1, 4.4591, 116.936, TestName = "E2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E2_JOB, 11, 2, 30.4812, 124.8283, TestName = "E2 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E3_JOB, 0, 0, 0.7916, 101.6194, TestName = "E3 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E3_JOB, 1, 1, 3.3777, 104.792, TestName = "E3 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E3_JOB, 2, 2, 28.1637, 128.3685, TestName = "E3 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E3_JOB, 6, 0, 0.8144, 101.6097, TestName = "E3 BEV TimeRun PWheel LongHaul"),
        TestCase(E3_JOB, 7, 1, 3.5103, 104.7876, TestName = "E3 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E3_JOB, 8, 2, 29.6221, 128.1818, TestName = "E3 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E4_JOB, 0, 0, 0.9165, 97.3067, TestName = "E4 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E4_JOB, 1, 1, 3.7896, 100.3712, TestName = "E4 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E4_JOB, 2, 2, 30.1342, 123.7528, TestName = "E4 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E4_JOB, 6, 0, 0.94, 97.3005, TestName = "E4 BEV TimeRun PWheel LongHaul"),
        TestCase(E4_JOB, 7, 1, 3.9236, 100.3755, TestName = "E4 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E4_JOB, 8, 2, 31.6158, 123.6378, TestName = "E4 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_JOB, 0, 0, 1.5963, 94.1169, TestName = "IEPC3X BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_JOB, 1, 1, 5.2793, 92.2062, TestName = "IEPC3X BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_JOB, 2, 2, 33.9595, 105.0117, TestName = "IEPC3X BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_JOB, 6, 0, 1.594, 94.1353, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_JOB, 7, 1, 5.272, 92.2526, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_JOB, 8, 2, 33.8808, 105.5098, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_JOB, 9, 0, 1.6143, 94.1029, TestName = "IEPC3X BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_JOB, 10, 1, 5.3745, 92.1815, TestName = "IEPC3X BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_JOB, 11, 2, 35.0582, 104.9996, TestName = "IEPC3X BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 0, 0, 1.691, 90.7354, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 1, 1, 5.549, 89.0315, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 2, 2, 35.0967, 102.9804, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 6, 0, 1.6886, 90.7538, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 7, 1, 5.5414, 89.0777, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 8, 2, 35.0158, 103.4783, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 9, 0, 1.7094, 90.7148, TestName = "IEPC3X_AXLE BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 10, 1, 5.6428, 89.0134, TestName = "IEPC3X_AXLE BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 11, 2, 36.1883, 102.9508, TestName = "IEPC3X_AXLE BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 0, 0, 1.709, 90.7583, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 1, 1, 5.5985, 89.0938, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 2, 2, 35.4083, 103.3546, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 6, 0, 1.7066, 90.7762, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 7, 1, 5.591, 89.1389, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 8, 2, 35.3302, 103.842, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 9, 0, 1.7283, 90.7371, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 10, 1, 5.6898, 89.0766, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 11, 2, 36.49, 103.2827, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 0, 0, 1.691, 90.7354, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 1, 1, 5.549, 89.0315, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 2, 2, 35.0967, 102.9804, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 6, 0, 1.6886, 90.7538, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 7, 1, 5.5414, 89.0777, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 8, 2, 35.0158, 103.4783, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 9, 0, 1.7094, 90.7148, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 10, 1, 5.6428, 89.0134, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 11, 2, 36.1883, 102.9508, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel UrbanDelivery")
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

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }
    }
}
