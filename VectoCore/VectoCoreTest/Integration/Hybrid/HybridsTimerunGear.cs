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
        private const string IHPC_6SPEED_JOB= @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericIHPC\6SpeedGbx\IHPC Group 5.vecto";
        private const string IHPC_12SPEED_JOB= @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericIHPC\12SpeedGbx\IHPC Group 5.vecto";
        
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
        TestCase(P1_JOB, 0, 495.9162, 10.109, 10.0911, 51.3798, 49.0387, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P1_JOB, 1, 496.4705, 15.0474, 15.1196, 39.3967, 33.9231, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"), //corrected (2)
        TestCase(P1_JOB, 2, 624.5117, 47.7872, 47.7917, 20.7414, 8.9727, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_JOB, 0, 575.0292, 8.7344, 7.6996, 56.8462, 54.8112, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_JOB, 1, 560.7134, 11.3702, 11.4443, 42.7787, 37.8862, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_JOB, 2, 627.07, 28.9551, 29.9004, 20.8454, 9.4736, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"), //corrected (1)

        TestCase(P2_5_JOB, 0, 848.7992, 5.7746, 9.4119, 99.8514, 84.4142, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 1, 964.503, 6.8655, 11.1103, 85.1731, 60.2621, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"), //corrected (1)
        TestCase(P2_5_JOB, 2, 1673.2243, 14.9028, 19.5998, 57.487, 18.4409, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"), //corrected (1)

        TestCase(P3_JOB, 0, 573.9488, 2.8993, 3.2519, 56.8509, 54.8181, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 1, 559.0593, 7.4798, 8.4667, 42.8132, 37.9181, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P3_JOB, 2, 607.6912, 27.3724, 28.3928, 20.8754, 9.4923, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"), //corrected (1)

        TestCase(P4_JOB, 0, 573.613, 1.7104, 1.9642, 56.5541, 54.5304, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P4_JOB, 1, 569.8255, 3.1632, 3.8582, 42.3671, 37.5376, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P4_JOB, 2, 701.2659, 7.8066, 8.9675, 20.5767, 9.3336, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IHPC_6SPEED_JOB, 0, 782.1167, 33.9944, 32.7456, 101.6309, 86.5873, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 1, 835.1911, 46.3719, 44.8218, 88.4838, 62.5003, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 2, 1237.9412, 114.4095, 107.3087, 64.9372, 20.13, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IHPC_12SPEED_JOB, 0, 812.8371, 21.4298, 21.4595, 101.4736, 86.4456, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 1, 901.6363, 29.6849, 30.099, 87.7325, 62.1546, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"), // corrected (1)
        TestCase(IHPC_12SPEED_JOB, 2, 1503.9847, 71.9527, 70.3088, 63.7561, 19.8885, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery")
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
