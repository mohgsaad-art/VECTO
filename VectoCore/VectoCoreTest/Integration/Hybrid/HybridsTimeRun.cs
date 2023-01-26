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
        private const string P2_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_Group2_P2_EM/Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P3_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_Group2_P3_EM/Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P4_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_Group2_P4_EM/Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P2_5_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_Group5_P2.5/P2.5 Group 5 2.vecto";
        private const string P1_JOB = @"TestData/Integration/TimeRun/MeasuredSpeed/GenericVehicle_P1-APT/CityBus_AT_PS.vecto";
        private const string IHPC_6SPEED_JOB= @"TestData/Integration/TimeRun/MeasuredSpeed/GenericIHPC/6SpeedGbx/IHPC Group 5.vecto";
        private const string IHPC_12SPEED_JOB= @"TestData/Integration/TimeRun/MeasuredSpeed/GenericIHPC/12SpeedGbx/IHPC Group 5.vecto";

        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Category("JRC")]
		[Category("LongRunning")]
		[Category("Integration")]
        [
        TestCase(P1_JOB, 0, 496.8873, 10.2371, 10.2191, 51.3803, 49.0392, TestName = "P1 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 1, 497.4313, 14.9897, 15.072, 39.3967, 33.9238, TestName = "P1 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 2, 629.491, 44.975, 44.9965, 20.7555, 8.9994, TestName = "P1 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        
        TestCase(P2_JOB, 0, 573.947, 8.7646, 7.7124, 56.8413, 54.8021, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, 557.0427, 11.5103, 11.4645, 42.7029, 37.7794, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, 608.2527, 30.5006, 31.2549, 20.789, 9.3313, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 836.7834, 5.5221, 9.1817, 97.6105, 82.1992, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 1, 946.3114, 6.8857, 11.0556, 82.4354, 57.6871, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 1654.5214, 13.5674, 18.3792, 55.1309, 17.1532, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P3_JOB, 0, 573.7316, 2.8828, 3.2344, 56.8513, 54.8142, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, 558.1475, 7.5458, 8.4801, 42.8088, 37.8984, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, 0, 0, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P4_JOB, 0, 573.1113, 1.7022, 1.957, 56.5289, 54.4965, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, 568.8486, 3.2313, 3.9296, 42.349, 37.4994, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, 711.7234, 8.2956, 9.4808, 20.3771, 9.1163, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IHPC_6SPEED_JOB, 0, 791.3489, 32.2891, 31.2579, 100.9517, 85.8919, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 1, 861.6339, 43.5531, 42.3948, 88.29, 62.3502, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 2, 1233.2673, 110.0654, 103.4252, 64.8424, 20.0724, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(IHPC_12SPEED_JOB, 0, 821.4219, 21.1183, 21.1915, 101.4752, 86.4461, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 1, 921.6429, 29.1447, 29.5643, 87.8293, 62.2116, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 2, 1518.4682, 66.2813, 65.1601, 63.7544, 19.8701, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed UrbanDelivery")
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
