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
        TestCase(E2_JOB, 0, 0, 1.1651, 120.0786, TestName = "E2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E2_JOB, 1, 1, 4.409, 117.1787, TestName = "E2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E2_JOB, 2, 2, 29.5192, 126.2702, TestName = "E2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E2_JOB, 6, 0, 1.1634, 120.0728, TestName = "E2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(E2_JOB, 7, 1, 4.4193, 117.1627, TestName = "E2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(E2_JOB, 8, 2, 29.585, 126.1664, TestName = "E2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(E2_JOB, 9, 0, 1.1957, 120.0453, TestName = "E2 BEV TimeRun PWheel LongHaul"),
        TestCase(E2_JOB, 10, 1, 4.6028, 117.123, TestName = "E2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E2_JOB, 11, 2, 31.7001, 126.8887, TestName = "E2 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E3_JOB, 0, 0, 0.8445, 101.6789, TestName = "E3 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E3_JOB, 1, 1, 3.6425, 105.0844, TestName = "E3 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E3_JOB, 2, 2, 30.5744, 131.0605, TestName = "E3 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E3_JOB, 6, 0, 0.8669, 101.6725, TestName = "E3 BEV TimeRun PWheel LongHaul"),
        TestCase(E3_JOB, 7, 1, 3.7693, 105.0839, TestName = "E3 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E3_JOB, 8, 2, 31.9715, 130.8939, TestName = "E3 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(E4_JOB, 0, 0, 0.9694, 97.3673, TestName = "E4 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(E4_JOB, 1, 1, 4.0547, 100.6667, TestName = "E4 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(E4_JOB, 2, 2, 32.5519, 126.4729, TestName = "E4 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(E4_JOB, 6, 0, 0.9925, 97.3627, TestName = "E4 BEV TimeRun PWheel LongHaul"),
        TestCase(E4_JOB, 7, 1, 4.1832, 100.6705, TestName = "E4 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(E4_JOB, 8, 2, 33.9662, 126.343, TestName = "E4 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_JOB, 0, 0, 1.6143, 94.1388, TestName = "IEPC3X BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_JOB, 1, 1, 5.3288, 92.2672, TestName = "IEPC3X BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_JOB, 2, 2, 34.2658, 105.4006, TestName = "IEPC3X BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_JOB, 6, 0, 1.612, 94.1575, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_JOB, 7, 1, 5.3216, 92.3144, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_JOB, 8, 2, 34.1897, 105.9113, TestName = "IEPC3X BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_JOB, 9, 0, 1.6306, 94.1195, TestName = "IEPC3X BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_JOB, 10, 1, 5.4244, 92.2446, TestName = "IEPC3X BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_JOB, 11, 2, 35.3498, 105.3943, TestName = "IEPC3X BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 0, 0, 1.7091, 90.7572, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 1, 1, 5.6007, 89.0999, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 2, 2, 35.3919, 103.3803, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 6, 0, 1.7067, 90.776, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 7, 1, 5.5932, 89.1469, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 8, 2, 35.3138, 103.8925, TestName = "IEPC3X_AXLE BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_AXLE_JOB, 9, 0, 1.7283, 90.738, TestName = "IEPC3X_AXLE BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_AXLE_JOB, 10, 1, 5.6919, 89.0874, TestName = "IEPC3X_AXLE BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_AXLE_JOB, 11, 2, 36.4718, 103.3618, TestName = "IEPC3X_AXLE BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 0, 0, 1.7431, 90.8044, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 1, 1, 5.6987, 89.2263, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 2, 2, 36.0026, 104.1617, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 6, 0, 1.7407, 90.8231, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 7, 1, 5.6917, 89.2737, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 8, 2, 35.9317, 104.6797, TestName = "IEPC3X_WHEEL1 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL1_JOB, 9, 0, 1.7585, 90.784, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL1_JOB, 10, 1, 5.7864, 89.2035, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL1_JOB, 11, 2, 37.0538, 104.117, TestName = "IEPC3X_WHEEL1 BEV TimeRun PWheel UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 0, 0, 1.7091, 90.7572, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 1, 1, 5.6007, 89.0999, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 2, 2, 35.3919, 103.3803, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 6, 0, 1.7067, 90.776, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 7, 1, 5.5932, 89.1469, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 8, 2, 35.3138, 103.8925, TestName = "IEPC3X_WHEEL2 BEV TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IEPC3X_WHEEL2_JOB, 9, 0, 1.7283, 90.738, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel LongHaul"),
        TestCase(IEPC3X_WHEEL2_JOB, 10, 1, 5.6919, 89.0874, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel RegionalDelivery"),
        TestCase(IEPC3X_WHEEL2_JOB, 11, 2, 36.4718, 103.3618, TestName = "IEPC3X_WHEEL2 BEV TimeRun PWheel UrbanDelivery")
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
