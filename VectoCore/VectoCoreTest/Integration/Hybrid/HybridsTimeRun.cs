using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;

namespace TUGraz.VectoCore.Tests.Integration.Hybrid
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HybridsTimeRunTest
    {
        private const string P2_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P3_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicle_Group2_P3_EM\Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P4_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicle_Group2_P4_EM\Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P2_5_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicle_Group5_P2.5\P2.5 Group 5 2.vecto";
        private const string P1_JOB = @"TestData\Integration\TimeRun\MeasuredSpeed\GenericVehicle_P1-APT\CityBus_AT_PS.vecto";
        private const string IHPC_6SPEED_JOB= @"TestData\Integration\TimeRun\MeasuredSpeed\GenericIHPC\6SpeedGbx\IHPC Group 5.vecto";
        private const string IHPC_12SPEED_JOB= @"TestData\Integration\TimeRun\MeasuredSpeed\GenericIHPC\12SpeedGbx\IHPC Group 5.vecto";

        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Category("JRC")]
		[Category("LongRunning")]
		[Category("Integration")]
        [
        TestCase(P1_JOB, 0, 496.8756, 10.1923, 10.1745, 51.3798, 49.0388, TestName = "P1 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 1, 497.4272, 15.0003, 15.0825, 39.3949, 33.9218, TestName = "P1 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 2, 630.0769, 44.916, 44.9378, 20.7742, 8.998, TestName = "P1 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        
        TestCase(P2_JOB, 0, 573.8802, 8.708, 7.6547, 56.8497, 54.8098, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, 557.427, 11.5132, 11.4639, 42.7538, 37.8312, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, 610.4948, 30.6064, 31.4296, 20.7994, 9.3408, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 836.1743, 5.5229, 9.1872, 96.8081, 81.5287, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 1, 946.8806, 6.7719, 10.9654, 81.7536, 57.6873, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 1643.2916, 11.7918, 16.4455, 53.3112, 16.591, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P3_JOB, 0, 573.7057, 2.8913, 3.2429, 56.8516, 54.8131, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, 557.9404, 7.5087, 8.443, 42.8131, 37.9026, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, 0, 0, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P4_JOB, 0, 572.5051, 1.7167, 1.9718, 56.365, 54.331, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, 568.7838, 3.2977, 4.0094, 42.2992, 37.4581, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, 711.7185, 8.2921, 9.4681, 20.3727, 9.1141, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IHPC_6SPEED_JOB, 0, 789.4683, 31.611, 30.6306, 100.8837, 85.7417, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 1, 858.1059, 44.0771, 42.8824, 88.1826, 62.2659, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 2, 1238.5376, 109.9816, 103.3343, 64.9494, 20.1109, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IHPC_12SPEED_JOB, 0, 823.8629, 20.1096, 20.2614, 101.152, 86.1182, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 1, 921.1086, 29.1406, 29.5595, 87.7955, 62.2168, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 2, 1517.1778, 65.9818, 64.9079, 63.7205, 19.8704, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed UrbanDelivery")
        ]
        public void TestHybridTimeRunCycle(string jobFile, int cycleIdx, double CO2, double charge, double discharge, double pWheelpos,
            double pWheel)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SumDataFields.CO2_KM, CO2 },
                { SumDataFields.E_REESS_T_chg, charge },
                { SumDataFields.E_REESS_T_dischg, discharge },
                { SumDataFields.P_WHEEL_POS, pWheelpos },
                { SumDataFields.P_WHEEL, pWheel }
			};

            RunHybridTimeRunCycle(jobFile, cycleIdx, metrics);
        }

        [Category("TUG-update")]
        [Category("JRC")]
        [Category("LongRunning")]
        [Category("Integration")]
        [
        TestCase(P1_JOB, 3, TestName = "P1 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 4, TestName = "P1 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 5, TestName = "P1 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_JOB, 3, TestName = "P2 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 4, TestName = "P2 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 5, TestName = "P2 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_5_JOB, 3, TestName = "P2_5 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 4, TestName = "P2_5 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 5, TestName = "P2_5 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(P3_JOB, 3, TestName = "P3 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 4, TestName = "P3 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 5, TestName = "P3 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(P4_JOB, 3, TestName = "P4 Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 4, TestName = "P4 Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 5, TestName = "P4 Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(IHPC_6SPEED_JOB, 3, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 4, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 5, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeed UrbanDelivery"),

        TestCase(IHPC_12SPEED_JOB, 3, TestName = "IHPC_12SPEED IHPC Hybrid DistanceRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 4, TestName = "IHPC_12SPEED IHPC Hybrid DistanceRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 5, TestName = "IHPC_12SPEED IHPC Hybrid DistanceRun MeasuredSpeed UrbanDelivery")
        ]
        public void TestHybridDistanceRunCycle(string jobFile, int cycleIdx)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SumDataFields.CO2_KM, double.NaN },
                { SumDataFields.E_REESS_T_chg, double.NaN },
                { SumDataFields.E_REESS_T_dischg, double.NaN }
			};

            RunHybridDistanceRunCycle(jobFile, cycleIdx, metrics);
        }

        public void RunHybridTimeRunCycle(string jobFile, int cycleIdx, Dictionary<String, double> metrics)
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
            AssertHelper.ReportDeviations(distanceSumPath, cycleIdx, factory, metrics);
            
            AssertHelper.AssertMetrics(factory, metrics);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }

        public void RunHybridDistanceRunCycle(string jobFile, int cycleIdx, Dictionary<String, double> metrics)
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
