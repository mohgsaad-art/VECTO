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
        TestCase(P1_JOB, 0, 495.9162, 10.109, 10.0911, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P1_JOB, 1, 496.4631, 15.0433, 15.1161, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"), //corrected (2)
        TestCase(P1_JOB, 2, 624.0423, 47.7612, 47.7657, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"), //corrected (1)

        TestCase(P2_JOB, 0, 575.0719, 8.7721, 7.7352, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_JOB, 1, 560.5059, 11.2928, 11.3766, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_JOB, 2, 626.9736, 28.9615, 29.8771, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"), //corrected (1)

        TestCase(P2_5_JOB, 0, 848.6453, 5.7706, 9.4096, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 1, 965.2296, 6.8907, 11.1147, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"), //corrected (1)
        TestCase(P2_5_JOB, 2, 1675.0363, 14.8485, 19.5503, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P3_JOB, 0, 573.9547, 2.8769, 3.2305, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 1, 559.3485, 7.5742, 8.5562, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P3_JOB, 2, 608.1537, 27.3906, 28.4109, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"), //corrected (1)

        TestCase(P4_JOB, 0, 573.6356, 1.7025, 1.9567, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P4_JOB, 1, 569.2767, 3.1507, 3.8596, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P4_JOB, 2, 700.6706, 7.7799, 8.9399, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery") //corrected (1)
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
