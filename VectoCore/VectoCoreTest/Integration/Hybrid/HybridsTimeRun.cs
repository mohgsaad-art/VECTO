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
        TestCase(P2_JOB, 0, 575.2666, 8.6809, 7.6311, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, 566.303, 11.2974, 11.1686, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, 652.6198, 30.0121, 30.6825, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P3_JOB, 0, 574.6327, 2.8561, 3.2357, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, 565.9677, 7.4585, 8.3992, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, 645.7506, 27.2941, 28.2386, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P4_JOB, 0, 574.0029, 1.6969, 1.9504, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, 578.1415, 3.2344, 3.9599, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, 747.9616, 8.4186, 9.6123, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P2_5_JOB, 0, 859.5821, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 1, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),

        TestCase(P1_JOB, 0, 496.8873, 10.2371, 10.2191, TestName = "P1 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 1, 497.4313, 14.9897, 15.072, TestName = "P1 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 2, 629.491, 44.975, 44.9965, TestName = "P1 Hybrid TimeRun MeasuredSpeed UrbanDelivery")
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
