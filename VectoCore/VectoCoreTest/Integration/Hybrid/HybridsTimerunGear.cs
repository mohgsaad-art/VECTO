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

namespace TUGraz.VectoCore.Tests.Integration.Hybrid
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class HybridsTimerunGear
    {
        private const string P2_JOB = @"TestData/Integration/TimeRun/MeasuredSpeedGear/GenericVehicle_Group2_P2_EM/Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P3_JOB = @"TestData/Integration/TimeRun/MeasuredSpeedGear/GenericVehicle_Group2_P3_EM/Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P4_JOB = @"TestData/Integration/TimeRun/MeasuredSpeedGear/GenericVehicle_Group2_P4_EM/Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P2_5_JOB = @"TestData/Integration/TimeRun/MeasuredSpeedGear/GenericVehicle_Group5_P2.5/P2.5 Group 5 2.vecto";
        private const string P1_JOB = @"TestData/Integration/TimeRun/MeasuredSpeedGear/GenericVehicle_P1-APT/CityBus_AT_PS.vecto";
        private const string IHPC_6SPEED_JOB= @"TestData/Integration/TimeRun/MeasuredSpeedGear/GenericIHPC/6SpeedGbx/IHPC Group 5.vecto";
        private const string IHPC_12SPEED_JOB= @"TestData/Integration/TimeRun/MeasuredSpeedGear/GenericIHPC/12SpeedGbx/IHPC Group 5.vecto";
        
        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        /*
         * How to correct vdri data for time runs
         * ---------------------------------------
         * 1. If VECTO fails with retry count exceeded and ResponseOverload, check the vdri at that time step and 
         * correct the gear by a) "unifying" consecutive gear shifts with the same gear, or b) lowering the gear at the
         * adjacent steps.
         * 
         * 2. If VECTO fails with retry count exceeded and ResponseOverload at the first step(s), check the vdri at the 
         * beginning for consecutive steps with zeroes in vehicle speed, gear, etc. Leave only one of those steps and 
         * delete the rest.
         */
        [Category("JRC")]
		[Category("LongRunning")]
		[Category("Integration")]
        [
        TestCase(P1_JOB, 0, 495.9084, 10.1283, 10.1104, 51.3742, 49.033, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P1_JOB, 1, 496.4165, 14.922, 14.995, 39.393, 33.9201, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P1_JOB, 2, 623.4541, 46.4335, 46.442, 20.6765, 8.9351, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_JOB, 0, 575.0719, 8.7721, 7.7352, 56.839, 54.8047, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_JOB, 1, 560.5021, 11.2928, 11.3766, 42.7819, 37.889, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_JOB, 2, 627.5877, 28.9301, 29.8455, 20.8317, 9.4637, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 848.6429, 5.7706, 9.4096, 99.8342, 84.3938, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 1, 0, 0, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"), 
        TestCase(P2_5_JOB, 2, 1675.0347, 14.8485, 19.5503, 57.5327, 18.4334, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P3_JOB, 0, 573.951, 2.8769, 3.2305, 56.8495, 54.8168, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 1, 559.3447, 7.5742, 8.5562, 42.8089, 37.9156, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P3_JOB, 2, 608.4345, 27.3547, 28.3752, 20.8738, 9.4915, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P4_JOB, 0, 573.6315, 1.7025, 1.9567, 56.5956, 54.5714, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P4_JOB, 1, 569.2729, 3.1507, 3.8596, 42.3881, 37.5599, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P4_JOB, 2, 701.3478, 7.8252, 8.985, 20.5114, 9.294, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IHPC_6SPEED_JOB, 0, 782.1722, 34.0905, 32.8447, 101.6486, 86.6075, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 1, 834.0826, 45.6237, 44.1385, 88.4057, 62.4774, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 2, 1236.7767, 114.404, 107.2995, 64.889, 20.1226, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IHPC_12SPEED_JOB, 0, 812.8371, 21.4298, 21.4595, 101.4736, 86.4456, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 1, 902.3367, 29.6994, 30.1398, 87.83, 62.2189, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 2, 1504.771, 72.0097, 70.3563, 63.813, 19.8842, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery")
        ]
        public void TestHybridTimeRunCycle(string jobFile, int cycleIdx, double CO2, double charge, double discharge, double pWheelpos,
            double pWheel)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SummaryDataContainer.Fields.CO2_KM, CO2 },
                { SummaryDataContainer.Fields.E_REESS_T_chg, charge },
                { SummaryDataContainer.Fields.E_REESS_T_dischg, discharge },
                { SummaryDataContainer.Fields.P_WHEEL_POS, pWheelpos },
                { SummaryDataContainer.Fields.P_WHEEL, pWheel }
			};

            RunHybridTimeRunCycle(jobFile, cycleIdx, metrics);
        }

        [Category("JRC")]
        [Category("LongRunning")]
        [Category("Integration")]
        [
        TestCase(P1_JOB, 3, TestName = "P1 Hybrid DistanceRun MeasuredSpeedGear LongHaul"),
        TestCase(P1_JOB, 4, TestName = "P1 Hybrid DistanceRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P1_JOB, 5, TestName = "P1 Hybrid DistanceRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_JOB, 3, TestName = "P2 Hybrid DistanceRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_JOB, 4, TestName = "P2 Hybrid DistanceRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_JOB, 5, TestName = "P2 Hybrid DistanceRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_5_JOB, 3, TestName = "P2_5 Hybrid DistanceRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 4, TestName = "P2_5 Hybrid DistanceRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_5_JOB, 5, TestName = "P2_5 Hybrid DistanceRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P3_JOB, 3, TestName = "P3 Hybrid DistanceRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 4, TestName = "P3 Hybrid DistanceRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P3_JOB, 5, TestName = "P3 Hybrid DistanceRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P4_JOB, 3, TestName = "P4 Hybrid DistanceRun MeasuredSpeedGear LongHaul"),
        TestCase(P4_JOB, 4, TestName = "P4 Hybrid DistanceRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P4_JOB, 5, TestName = "P4 Hybrid DistanceRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IHPC_6SPEED_JOB, 3, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 4, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 5, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IHPC_12SPEED_JOB, 3, TestName = "IHPC_12SPEED IHPC Hybrid DistanceRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 4, TestName = "IHPC_12SPEED IHPC Hybrid DistanceRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 5, TestName = "IHPC_12SPEED IHPC Hybrid DistanceRun MeasuredSpeedGear UrbanDelivery")
        ]
        public void TestHybridDistanceRunCycle(string jobFile, int cycleIdx)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SummaryDataContainer.Fields.CO2_KM, double.NaN },
                { SummaryDataContainer.Fields.E_REESS_T_chg, double.NaN },
                { SummaryDataContainer.Fields.E_REESS_T_dischg, double.NaN }
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
