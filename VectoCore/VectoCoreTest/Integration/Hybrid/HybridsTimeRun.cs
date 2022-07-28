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
        TestCase(P1_JOB, 1, 497.431, 15.0404, 15.1223, 39.3974, 33.9238, TestName = "P1 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 2, 629.9087, 45.0032, 45.0246, 20.7733, 8.9985, TestName = "P1 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        
        TestCase(P2_JOB, 0, 573.9052, 8.7047, 7.6549, 56.8495, 54.8095, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, 557.342, 11.4929, 11.4449, 42.7555, 37.8328, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, 608.0294, 30.607, 31.4384, 20.7954, 9.3377, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 836.8542, 5.4472, 9.1129, 97.6207, 82.1965, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 1, 948.4517, 6.8962, 11.0601, 82.857, 58.0923, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 1653.1938, 13.5281, 18.2762, 55.0623, 17.108, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P3_JOB, 0, 573.7034, 2.8792, 3.2346, 56.8521, 54.8135, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, 557.9873, 7.5073, 8.4417, 42.8139, 37.9032, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, 0, 0, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P4_JOB, 0, 573.5137, 1.733, 1.9888, 56.5505, 54.5212, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, 569.1214, 3.2957, 3.9781, 42.3031, 37.4575, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, 712.0098, 8.3069, 9.4837, 20.3732, 9.1137, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery")
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
        TestCase(P4_JOB, 5, TestName = "P4 Hybrid DistanceRun MeasuredSpeed UrbanDelivery")
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

			//Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
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

			//Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }

    }
}
