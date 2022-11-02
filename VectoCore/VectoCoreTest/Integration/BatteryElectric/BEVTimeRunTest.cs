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
        TestCase(E2_JOB, 0, 0, 0.6415, 57.8797, TestName = "E2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E2_JOB, 1, 1, 2.3874, 55.8618, TestName = "E2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E2_JOB, 2, 2, 15.6732, 57.2625, TestName = "E2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E2_JOB, 6, 0, 0.6417, 57.8782, TestName = "E2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(E2_JOB, 7, 1, 2.3936, 55.855, TestName = "E2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(E2_JOB, 8, 2, 15.7181, 57.2289, TestName = "E2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(E2_JOB, 9, 0, 0.6562, 57.8561, TestName = "E2 BEV TimeRun PWheel LongHaul"),
        TestCase(E2_JOB, 10, 1, 2.4716, 55.8024, TestName = "E2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E2_JOB, 11, 2, 16.5514, 57.0518, TestName = "E2 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E3_JOB, 0, 0, 0.4531, 47.3529, TestName = "E3 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E3_JOB, 1, 1, 1.8863, 48.6232, TestName = "E3 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E3_JOB, 2, 2, 15.1783, 58.4318, TestName = "E3 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E3_JOB, 6, 0, 0.4637, 47.3472, TestName = "E3 BEV TimeRun PWheel LongHaul"),
        TestCase(E3_JOB, 7, 1, 1.9485, 48.617, TestName = "E3 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E3_JOB, 8, 2, 15.8621, 58.2931, TestName = "E3 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E4_JOB, 0, 0, 0.5177, 45.1982, TestName = "E4 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E4_JOB, 1, 1, 2.1198, 46.4401, TestName = "E4 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E4_JOB, 2, 2, 16.1847, 56.1445, TestName = "E4 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E4_JOB, 6, 0, 0.5286, 45.1944, TestName = "E4 BEV TimeRun PWheel LongHaul"),
        TestCase(E4_JOB, 7, 1, 2.183, 46.4384, TestName = "E4 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E4_JOB, 8, 2, 16.8807, 56.0423, TestName = "E4 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_JOB, 0, 0, 0.8765, 44.9221, TestName = "IEPC3X BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_JOB, 1, 1, 2.9071, 43.4708, TestName = "IEPC3X BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_JOB, 2, 2, 18.407, 47.2887, TestName = "IEPC3X BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_JOB, 6, 0, 0.8754, 44.9313, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_JOB, 7, 1, 2.9035, 43.494, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_JOB, 8, 2, 18.3679, 47.538, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_JOB, 9, 0, 0.8825, 44.9121, TestName = "IEPC3X BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_JOB, 10, 1, 2.9434, 43.447, TestName = "IEPC3X BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_JOB, 11, 2, 18.8478, 47.1707, TestName = "IEPC3X BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 0, 0, 0.9303, 43.2376, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 1, 1, 3.0557, 41.8951, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 2, 2, 18.9934, 46.2902, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 6, 0, 0.9291, 43.2468, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 7, 1, 3.052, 41.9182, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 8, 2, 18.9533, 46.5396, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 9, 0, 0.9368, 43.2246, TestName = "IEPC3X_AXLE BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 10, 1, 3.089, 41.8728, TestName = "IEPC3X_AXLE BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 11, 2, 19.4281, 46.1668, TestName = "IEPC3X_AXLE BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 0, 0, 0.9399, 43.2495, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 1, 1, 3.0813, 41.9273, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 2, 2, 19.151, 46.4791, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 6, 0, 0.9387, 43.2585, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 7, 1, 3.0779, 41.9502, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 8, 2, 19.1136, 46.7244, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 9, 0, 0.9467, 43.2361, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 10, 1, 3.1139, 41.9059, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 11, 2, 19.5944, 46.3481, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 0, 0, 0.9303, 43.2376, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 1, 1, 3.0557, 41.8951, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 2, 2, 18.9934, 46.2902, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 6, 0, 0.9291, 43.2468, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 7, 1, 3.052, 41.9182, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 8, 2, 18.9533, 46.5396, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 9, 0, 0.9368, 43.2246, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 10, 1, 3.089, 41.8728, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 11, 2, 19.4281, 46.1668, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel UrbanDelivery")
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
        TestCase(E4_JOB, 5, TestName = "E4 BEV DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_JOB, 3, TestName = "IEPC3X BEV DistanceRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_JOB, 4, TestName = "IEPC3X BEV DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_JOB, 5, TestName = "IEPC3X BEV DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 3, TestName = "IEPC3X_AXLE BEV DistanceRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 4, TestName = "IEPC3X_AXLE BEV DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 5, TestName = "IEPC3X_AXLE BEV DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 3, TestName = "IEPC3X_WHEEL1 BEV DistanceRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 4, TestName = "IEPC3X_WHEEL1 BEV DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 5, TestName = "IEPC3X_WHEEL1 BEV DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 3, TestName = "IEPC3X_WHEEL2 BEV DistanceRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 4, TestName = "IEPC3X_WHEEL2 BEV DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 5, TestName = "IEPC3X_WHEEL2 BEV DistanceRun MeasuredSpeed UrbanDelivery"),
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
