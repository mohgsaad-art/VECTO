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
        TestCase(E2_JOB, 0, 0, 1.2831, 115.7593, TestName = "E2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E2_JOB, 1, 1, 4.7747, 111.7235, TestName = "E2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E2_JOB, 2, 2, 31.3464, 114.525, TestName = "E2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E2_JOB, 6, 0, 1.2834, 115.7564, TestName = "E2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(E2_JOB, 7, 1, 4.7871, 111.7101, TestName = "E2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(E2_JOB, 8, 2, 31.4362, 114.4577, TestName = "E2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(E2_JOB, 9, 0, 1.3124, 115.7122, TestName = "E2 BEV TimeRun PWheel LongHaul"),
        TestCase(E2_JOB, 10, 1, 4.9432, 111.6047, TestName = "E2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E2_JOB, 11, 2, 33.1027, 114.1036, TestName = "E2 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E3_JOB, 0, 0, 0.9063, 94.7057, TestName = "E3 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E3_JOB, 1, 1, 3.7725, 97.2465, TestName = "E3 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E3_JOB, 2, 2, 30.3565, 116.8636, TestName = "E3 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E3_JOB, 6, 0, 0.9274, 94.6945, TestName = "E3 BEV TimeRun PWheel LongHaul"),
        TestCase(E3_JOB, 7, 1, 3.8971, 97.2341, TestName = "E3 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E3_JOB, 8, 2, 31.7242, 116.5862, TestName = "E3 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E4_JOB, 0, 0, 1.0353, 90.3965, TestName = "E4 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E4_JOB, 1, 1, 4.2396, 92.8802, TestName = "E4 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E4_JOB, 2, 2, 32.3693, 112.2891, TestName = "E4 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E4_JOB, 6, 0, 1.0572, 90.3887, TestName = "E4 BEV TimeRun PWheel LongHaul"),
        TestCase(E4_JOB, 7, 1, 4.366, 92.8769, TestName = "E4 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E4_JOB, 8, 2, 33.7614, 112.0846, TestName = "E4 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_JOB, 0, 0, 1.753, 89.8441, TestName = "IEPC3X BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_JOB, 1, 1, 5.8143, 86.9416, TestName = "IEPC3X BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_JOB, 2, 2, 36.8139, 94.5774, TestName = "IEPC3X BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_JOB, 6, 0, 1.7507, 89.8625, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_JOB, 7, 1, 5.8071, 86.9881, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_JOB, 8, 2, 36.7358, 95.0761, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_JOB, 9, 0, 1.765, 89.8242, TestName = "IEPC3X BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_JOB, 10, 1, 5.8868, 86.8941, TestName = "IEPC3X BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_JOB, 11, 2, 37.6957, 94.3415, TestName = "IEPC3X BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 0, 0, 1.8607, 86.4752, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 1, 1, 6.1114, 83.7902, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 2, 2, 37.9868, 92.5805, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 6, 0, 1.8583, 86.4936, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 7, 1, 6.1039, 83.8365, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 8, 2, 37.9066, 93.0792, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 9, 0, 1.8736, 86.4491, TestName = "IEPC3X_AXLE BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 10, 1, 6.1779, 83.7456, TestName = "IEPC3X_AXLE BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 11, 2, 38.8563, 92.3336, TestName = "IEPC3X_AXLE BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 0, 0, 1.8797, 86.4989, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 1, 1, 6.1626, 83.8546, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 2, 2, 38.3021, 92.9582, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 6, 0, 1.8775, 86.517, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 7, 1, 6.1559, 83.9005, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 8, 2, 38.2273, 93.4489, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 9, 0, 1.8935, 86.4722, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 10, 1, 6.2278, 83.8119, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 11, 2, 39.1888, 92.6962, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 0, 0, 1.8607, 86.4752, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 1, 1, 6.1114, 83.7902, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 2, 2, 37.9868, 92.5805, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 6, 0, 1.8583, 86.4936, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 7, 1, 6.1039, 83.8365, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 8, 2, 37.9066, 93.0792, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 9, 0, 1.8736, 86.4491, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 10, 1, 6.1779, 83.7456, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 11, 2, 38.8563, 92.3336, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel UrbanDelivery")
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
