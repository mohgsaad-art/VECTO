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
        TestCase(P1_JOB, 0, 495.9363, 10.1142, 10.0963, 51.3793, 49.0383, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P1_JOB, 1, 496.5059, 15.0087, 15.0812, 39.3921, 33.919, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P1_JOB, 2, 624.8247, 47.7661, 47.7707, 20.7409, 8.9707, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_JOB, 0, 575.0411, 8.7404, 7.7021, 56.8461, 54.8111, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_JOB, 1, 560.7571, 11.348, 11.4211, 42.7785, 37.8861, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_JOB, 2, 628.2087, 28.6962, 29.6438, 20.8458, 9.4734, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 842.1234, 5.7451, 9.3868, 97.3054, 81.8595, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 1, 965.0922, 6.8904, 11.1521, 84.3985, 59.7151, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"), 
        TestCase(P2_5_JOB, 2, 1680.7238, 14.3424, 19.0983, 56.1582, 18.0483, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P3_JOB, 0, 573.9568, 2.9116, 3.2659, 56.8505, 54.8178, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 1, 559.1748, 7.4636, 8.4506, 42.8126, 37.9177, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P3_JOB, 2, 608.3104, 27.3321, 28.3515, 20.8775, 9.4939, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P4_JOB, 0, 573.529, 1.6467, 1.9006, 56.7053, 54.6756, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P4_JOB, 1, 569.2049, 3.1577, 3.8553, 42.3667, 37.5374, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P4_JOB, 2, 701.268, 7.8089, 8.9693, 20.5765, 9.3335, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IHPC_6SPEED_JOB, 0, 781.1672, 33.4656, 32.2608, 101.6617, 86.5373, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 1, 835.3289, 45.8498, 44.3575, 88.4131, 62.491, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 2, 1241.8914, 114.4288, 107.3267, 64.9614, 20.1278, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(IHPC_12SPEED_JOB, 0, 812.964, 21.2955, 21.3362, 101.459, 86.4264, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 1, 901.6716, 29.6316, 30.0512, 87.7192, 62.1457, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 2, 1507.2931, 71.7565, 70.1488, 63.7699, 19.8855, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery")
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
