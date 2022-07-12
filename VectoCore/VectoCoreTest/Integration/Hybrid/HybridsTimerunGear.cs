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
        private const string P2_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P3_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericVehicle_Group2_P3_EM\Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P4_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericVehicle_Group2_P4_EM\Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P2_5_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericVehicle_Group5_P2.5\P2.5 Group 5 2.vecto";
        private const string P1_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericVehicle_P1-APT\CityBus_AT_PS.vecto";
        
        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Category("JRC")]
		[Category("LongRunning")]
		[Category("Integration")]
        [
        TestCase(P1_JOB, 0, 0, 0, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P1_JOB, 1, 0, 0, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P1_JOB, 2, 0, 0, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_JOB, 0, 574.8186, 8.7811, 7.7352, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_JOB, 1, 559.6623, 11.3467, 11.391, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_JOB, 2, 623.6956, 29.5137, 30.411, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 848.5331, 5.7871, 9.4176, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 1, 964.8595, 6.9504, 11.1678, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 1671.819, 15.0947, 19.77, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P3_JOB, 0, 574.4831, 2.868, 3.2367, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 1, 562.3555, 7.3867, 8.404, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P3_JOB, 2, 641.3701, 25.7719, 26.8174, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P4_JOB, 0, 573.8993, 1.6987, 1.9567, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P4_JOB, 1, 570.5898, 3.1314, 3.8596, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P4_JOB, 2, 708.9572, 7.7111, 8.8806, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery")
        ]
        public void TestHybridTimerunCycle(string jobFile, int cycleIdx, double CO2, double charge, double discharge)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SummaryDataContainer.Fields.CO2_KM, CO2 },
                { SummaryDataContainer.Fields.E_REESS_T_chg, charge },
                { SummaryDataContainer.Fields.E_REESS_T_dischg, discharge }
			};

            RunHybridTimeRunCycle(jobFile, cycleIdx, metrics);
        }

        [Category("TUG-update")]
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
        TestCase(P4_JOB, 5, TestName = "P4 Hybrid DistanceRun MeasuredSpeedGear UrbanDelivery")
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
