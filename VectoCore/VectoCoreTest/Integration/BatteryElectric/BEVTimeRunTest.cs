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
        TestCase(E2_JOB, 0, 0, 1.3212, 115.8048, TestName = "E2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E2_JOB, 1, 1, 4.8834, 111.8572, TestName = "E2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E2_JOB, 2, 2, 32.1409, 115.5528, TestName = "E2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E2_JOB, 6, 0, 1.3193, 115.7989, TestName = "E2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(E2_JOB, 7, 1, 4.8952, 111.8427, TestName = "E2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(E2_JOB, 8, 2, 32.2101, 115.4526, TestName = "E2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(E2_JOB, 9, 0, 1.3537, 115.7734, TestName = "E2 BEV TimeRun PWheel LongHaul"),
        TestCase(E2_JOB, 10, 1, 5.0829, 111.8072, TestName = "E2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E2_JOB, 11, 2, 34.3508, 116.2004, TestName = "E2 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E3_JOB, 0, 0, 0.9604, 94.7664, TestName = "E3 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E3_JOB, 1, 1, 4.0407, 97.5422, TestName = "E3 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E3_JOB, 2, 2, 32.8012, 119.5899, TestName = "E3 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E3_JOB, 6, 0, 0.9801, 94.7574, TestName = "E3 BEV TimeRun PWheel LongHaul"),
        TestCase(E3_JOB, 7, 1, 4.1574, 97.5316, TestName = "E3 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E3_JOB, 8, 2, 34.082, 119.3069, TestName = "E3 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E4_JOB, 0, 0, 1.0882, 90.4571, TestName = "E4 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E4_JOB, 1, 1, 4.5049, 93.1759, TestName = "E4 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E4_JOB, 2, 2, 34.794, 115.0165, TestName = "E4 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E4_JOB, 6, 0, 1.1099, 90.4511, TestName = "E4 BEV TimeRun PWheel LongHaul"),
        TestCase(E4_JOB, 7, 1, 4.6265, 93.1728, TestName = "E4 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E4_JOB, 8, 2, 36.1207, 114.7989, TestName = "E4 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_JOB, 0, 0, 1.7705, 89.8657, TestName = "IEPC3X BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_JOB, 1, 1, 5.8651, 87.0042, TestName = "IEPC3X BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_JOB, 2, 2, 37.1233, 94.97, TestName = "IEPC3X BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_JOB, 6, 0, 1.7682, 89.8845, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_JOB, 7, 1, 5.8583, 87.0518, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_JOB, 8, 2, 37.0491, 95.4826, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_JOB, 9, 0, 1.7815, 89.8412, TestName = "IEPC3X BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_JOB, 10, 1, 5.9387, 86.9597, TestName = "IEPC3X BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_JOB, 11, 2, 38.0038, 94.7603, TestName = "IEPC3X BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 0, 0, 1.8804, 86.4985, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 1, 1, 6.1565, 83.853, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 2, 2, 38.2845, 92.9826, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 6, 0, 1.8782, 86.5173, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 7, 1, 6.1496, 83.9005, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 8, 2, 38.2085, 93.4968, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 9, 0, 1.8938, 86.4733, TestName = "IEPC3X_AXLE BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 10, 1, 6.2227, 83.8154, TestName = "IEPC3X_AXLE BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 11, 2, 39.1598, 92.7594, TestName = "IEPC3X_AXLE BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 0, 0, 1.9143, 86.5457, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 1, 1, 6.2554, 83.9808, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 2, 2, 38.904, 93.7733, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 6, 0, 1.9121, 86.5647, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 7, 1, 6.2496, 84.0293, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 8, 2, 38.839, 94.2973, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 9, 0, 1.9246, 86.5202, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 10, 1, 6.3208, 83.9357, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 11, 2, 39.7878, 93.5613, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 0, 0, 1.8804, 86.4985, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 1, 1, 6.1565, 83.853, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 2, 2, 38.2845, 92.9826, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 6, 0, 1.8782, 86.5173, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 7, 1, 6.1496, 83.9005, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 8, 2, 38.2085, 93.4968, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 9, 0, 1.8938, 86.4733, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 10, 1, 6.2227, 83.8154, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 11, 2, 39.1598, 92.7594, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel UrbanDelivery")
        ]
        public void TestBEVTimeRunCycle(string jobFile, int cycleIdx, int distanceCycleIdx, double charge, double discharge)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
                { SumDataFields.E_REESS_T_chg, charge },
                { SumDataFields.E_REESS_T_dischg, discharge }
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
				{ SumDataFields.E_REESS_T_chg, double.NaN },
                { SumDataFields.E_REESS_T_dischg, double.NaN }
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
