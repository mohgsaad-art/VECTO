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
        TestCase(P2_JOB, 0, 580.2063, 6.3558, 5.3125, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, 566.4735, 11.5669, 11.4186, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, 652.6895, 30.1909, 31.0123, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P3_JOB, 0, 578.3128, 2.8593, 2.6273, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, 565.3111, 7.4483, 8.3917, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, 646.0486, 27.2443, 28.2147, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P4_JOB, 0, 579.3321, 1.4022, 1.4869, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, 576.4058, 3.4684, 4.2249, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, 749.1922, 8.6428, 9.8394, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
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

    }
}
