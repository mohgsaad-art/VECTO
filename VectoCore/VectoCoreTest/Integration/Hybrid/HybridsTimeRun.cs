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
        TestCase(P2_JOB, 0, 584.7318, 6.3064, 5.2622, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, 561.3724, 11.1849, 10.9652, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, 0, 0, 0, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P3_JOB, 0, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P4_JOB, 0, 580.585, 1.491, 1.3954, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, 567.6795, 3.1631, 3.7575, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, 0, 0, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 853.946, 3.0619, 7.0072, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 1, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P1_JOB, 0, 504.81, 8.5547, 8.54, TestName = "P1 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 1, 496.9319, 14.6776, 14.7593, TestName = "P1 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 2, 628.967, 45.0138, 45.0353, TestName = "P1 Hybrid TimeRun MeasuredSpeed UrbanDelivery")
        ]
        public void TestHybridTimerunCycle(string jobFile, int cycleIdx, double CO2, double charge, double discharge)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				{ SummaryDataContainer.Fields.CO2_KM, CO2 },
                { SummaryDataContainer.Fields.E_REESS_T_chg, charge },
                { SummaryDataContainer.Fields.E_REESS_T_dischg, discharge }
			};

            RunHybridDriveCycle(jobFile, cycleIdx, metrics);
        }

        public void RunHybridDriveCycle(string jobFile, int cycleIdx, Dictionary<String, double> metrics)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			string outputFile = InputDataHelper.CreateUniqueSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputProvider, writer) {
				Validate = false,
				WriteModalResults = true,
			};
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			
            string distanceSumPath = Path.Combine(Path.GetDirectoryName(jobFile), "distance.vsum");
            AssertHelper.ReportDeviations(distanceSumPath, cycleIdx, factory, metrics);
            
            AssertHelper.AssertMetrics(factory, metrics);

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }

    }
}
