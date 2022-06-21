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

        TestCase(P2_5_JOB, 0, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 1, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P2_JOB, 0, 581.4683, 6.5757, 5.5439, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_JOB, 1, 0, 0, 0, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P2_JOB, 2, 0, 0, 0, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P3_JOB, 0, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 1, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P3_JOB, 2, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),

        TestCase(P4_JOB, 0, 0, 0, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P4_JOB, 1, 0, 0, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P4_JOB, 1, 0, 0, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery")
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
