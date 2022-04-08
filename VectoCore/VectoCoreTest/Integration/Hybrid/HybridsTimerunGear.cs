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
    public class HybridsTimerunGear
    {
        private const string P2_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericVehicle_Group2_P2_EM\Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P3_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericVehicle_Group2_P3_EM\Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P4_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericVehicle_Group2_P4_EM\Class2_RigidTruck_ParHyb_ENG.vecto";
        private const string P2_5_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\GenericVehicle_Group5_P2.5\P2.5 Group 5 2.vecto";
        private const string P1_JOB = @"TestData\Integration\TimeRun\MeasuredSpeedGear\CityBus_AT\CityBus_AT_PS.vecto";
        
        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [Category("JRC")]
		[Category("LongRunning")]
		[Category("Integration")]
        [
        TestCase(P2_JOB, 0, 572.3747, 2.7045, 1.7181, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaulNoGrad"),
        TestCase(P2_JOB, 1, 581.0263, 7.956, 8.8775, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear SmallUrbanDeliveryNoGrad"),
        TestCase(P2_JOB, 2, 572.3749, 2.6878, 1.7033, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear CycleTimeNoGrad"),
        TestCase(P2_JOB, 3, 0, 0, 0, TestName = "P2 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P3_JOB, 0, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear LongHaulNoGrad"),
        TestCase(P3_JOB, 1, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeedGear UrbanDeliveryNoGrad"),
        TestCase(P4_JOB, 0, 0, 0, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear LongHaulNoGrad"),
        TestCase(P4_JOB, 1, 0, 0, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeedGear UrbanDeliveryNoGrad"),
        TestCase(P2_5_JOB, 0, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear LongHaul"),
        TestCase(P2_5_JOB, 1, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),
        TestCase(P1_JOB, 0, 511.6089, 0, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear RegionalDelivery"),
        TestCase(P1_JOB, 1, 562.479, 0, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeedGear UrbanDelivery"),
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
			AssertHelper.AssertMetrics(factory, metrics);
            
			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
        }
    }
}
