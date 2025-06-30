using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.Simulation;

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

        /*
         * How to correct vdri data for time runs with gear
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
        TestCase(P1_JOB, 0, 0, 496.9016, 10.2037, 10.1858, 51.3796, 49.0389, TestName = "P1 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 1, 1, 497.4142, 15.0151, 15.0975, 39.3944, 33.922, TestName = "P1 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 2, 2, 630.2149, 44.726, 44.747, 20.752, 8.9981, TestName = "P1 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        
		TestCase(P1_JOB, 6, 0, 495.9188, 10.1443, 10.1264, 51.3734, 49.0327, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P1_JOB, 7, 1, 496.4581, 14.8994, 14.972, 39.3881, 33.9157, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P1_JOB, 8, 2, 0, 0, 0, 0, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery", Ignore="Hard to fix gear selection in vdri file"),
		
        TestCase(P2_JOB, 0, 0, 579.6284, 8.866, 7.6864, 56.9987, 54.8431, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, 1, 559.4666, 10.4123, 9.6562, 43.0757, 37.8983, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, 2, 593.7086, 26.8625, 26.0191, 21.3762, 9.4808, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

		TestCase(P2_JOB, 6, 0, 580.5758, 8.6123, 7.4419, 56.9719, 54.9039, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_JOB, 7, 1, 559.5154, 9.8914, 9.0774, 42.9582, 37.9712, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_JOB, 8, 2, 584.4209, 25.146, 24.3054, 21.2145, 9.5953, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 0, 889.6599, 3.9393, 7.5211, 102.1342, 85.6607, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 1, 1, 1009.2965, 6.0588, 9.3752, 86.9533, 60.6924, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 2, 1776.1712, 13.0147, 17.3303, 59.5787, 18.004, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

		TestCase(P2_5_JOB, 6, 0, 902.5276, 6.3532, 7.9834, 101.9656, 85.8464, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 7, 1, 1020.9232, 7.3171, 10.216, 88.4035, 61.8954, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"), 
        TestCase(P2_5_JOB, 8, 2, 1799.1259, 15.9136, 20.116, 61.3833, 19.0588, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P3_JOB, 0, 0, 588.5828, 8.0242, 7.0238, 56.9783, 54.8303, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, 1, 566.8818, 9.8751, 9.0682, 43.06, 37.8773, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, 2, 592.0671, 24.547, 23.7381, 21.2811, 9.4139, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

		TestCase(P3_JOB, 6, 0, 588.6146, 7.8562, 6.856, 56.9761, 54.8913, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 7, 1, 566.349, 9.75, 8.939, 43.0201, 37.9452, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P3_JOB, 8, 2, 574.3199, 23.911, 22.9435, 21.2638, 9.5601, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P4_JOB, 0, 0, 577.104, 2.7537, 1.8072, 56.9749, 54.843, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, 1, 567.6796, 3.2454, 2.3199, 42.9384, 37.8463, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, 2, 688.5758, 6.081, 5.1662, 21.1642, 9.3694, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

		TestCase(P4_JOB, 6, 0, 575.9543, 2.7665, 1.8168, 56.9579, 54.8866, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P4_JOB, 7, 1, 566.0504, 3.3263, 2.3911, 42.8919, 37.9241, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P4_JOB, 8, 2, 677.443, 5.9588, 4.9939, 21.1298, 9.5309, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),
		
        TestCase(IHPC_6SPEED_JOB, 0, 0, 826.9347, 21.3906, 21.4437, 101.691, 86.3408, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_6SPEED_JOB, 1, 1, 936.5872, 29.4015, 29.4103, 88.8407, 62.5671, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_6SPEED_JOB, 2, 2, 1468.0661, 73.9576, 71.7535, 64.2939, 19.9436, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeed UrbanDelivery", Ignore="Respective distance run fails"),

		TestCase(IHPC_6SPEED_JOB, 6, 0, 0, 0, 0, 0, 0, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul", Ignore="Hard to fix gear selection in vdri file"),
        TestCase(IHPC_6SPEED_JOB, 7, 1, 0, 0, 0, 0, 0, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery", Ignore="Hard to fix gear selection in vdri file"),
        TestCase(IHPC_6SPEED_JOB, 8, 2, 0, 0, 0, 0, 0, TestName = "IHPC_6SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery", Ignore="Respective distance run fails"),

        TestCase(IHPC_12SPEED_JOB, 0, 0, 819.8468, 21.6502, 21.6253, 102.5448, 87.3208, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(IHPC_12SPEED_JOB, 1, 1, 914.4771, 30.8441, 30.6636, 89.2142, 62.8425, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(IHPC_12SPEED_JOB, 2, 2, 1521.4502, 74.2185, 71.7417, 65.6317, 20.1404, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        
		TestCase(IHPC_12SPEED_JOB, 6, 0, 0, 0, 0, 0, 0, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear LongHaul", Ignore="Hard to fix gear selection in vdri file"),
        TestCase(IHPC_12SPEED_JOB, 7, 1, 0, 0, 0, 0, 0, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear RegionalDelivery", Ignore="Hard to fix gear selection in vdri file"),
        TestCase(IHPC_12SPEED_JOB, 8, 2, 0, 0, 0, 0, 0, TestName = "IHPC_12SPEED IHPC Hybrid TimeRun MeasuredSpeedGear UrbanDelivery", Ignore="Hard to fix gear selection in vdri file")
		]
        public void TestHybridTimeRunCycle(string jobFile, int cycleIdx, int distanceCycleIdx, double CO2, double charge, double discharge, double pWheelpos,
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

            RunHybridTimeRunCycle(jobFile, cycleIdx, distanceCycleIdx, metrics);
        }

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
        TestCase(IHPC_6SPEED_JOB, 5, TestName = "IHPC_6SPEED IHPC Hybrid DistanceRun MeasuredSpeed UrbanDelivery", Ignore="Distance run is currently(?) broken"),

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

        public void RunHybridTimeRunCycle(string jobFile, int cycleIdx, int distanceCycleIdx, Dictionary<String, double> metrics)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer, validate: false);
			factory.WriteModalResults = true;
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			
            string distanceSumPath = Path.Combine(Path.GetDirectoryName(jobFile), "distance.vsum");
            AssertHelper.ReportDeviations(distanceSumPath, distanceCycleIdx, factory, metrics);
            
            AssertHelper.AssertMetrics(factory, metrics);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }

        public void RunHybridDistanceRunCycle(string jobFile, int cycleIdx, Dictionary<String, double> metrics)
        { 
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer, validate: false);
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
