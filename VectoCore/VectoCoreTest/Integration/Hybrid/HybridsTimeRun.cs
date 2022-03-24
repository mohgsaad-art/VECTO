using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;

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
        TestCase(P2_JOB, 0, 584.9698, 5.519, 4.4734, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, 558.7214, 9.6829, 9.6138, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, 0, 0, 0, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        TestCase(P3_JOB, 0, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, 0, 0, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        TestCase(P4_JOB, 0, 0, 0, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, 0, 0, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, 0, 0, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        TestCase(P2_5_JOB, 0, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 1, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 2, 0, 0, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        TestCase(P1_JOB, 0, 0, 0, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 1, 0, 0, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 2, 0, 0, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeed UrbanDelivery")
        ]
        public void TestHybridTimerunCycle(string jobFile, int cycleIdx, double CO2, double charge, double discharge)
        {
            Dictionary<string, double> metrics = new Dictionary<string, double>()
			{
				//{ "FC-SoC [g/km]", fcSoC },
				//{ "FC-SoC_Corr [g/km]", fcSocCorr },
				{ SummaryDataContainer.Fields.CO2_KM, CO2 },
                { SummaryDataContainer.Fields.E_REESS_T_chg, charge },
                { SummaryDataContainer.Fields.E_REESS_T_dischg, discharge }
			};

            RunHybridDriveCycle(jobFile, cycleIdx, metrics);
        }

        public void RunHybridDriveCycle(string jobFile, int cycleIdx, Dictionary<String, double> metrics)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			(string outputFile, Guid guid) = GetUniqueOutputSubfolder(jobFile);
			var writer = new FileOutputWriter(outputFile);

			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputProvider, writer) {
				Validate = false,
				WriteModalResults = true,
			};
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			AssertMetricsAreWithinLimits(factory, metrics);

			DeleteOutputSubFolder(outputFile, guid);
        }

        private void DeleteOutputSubFolder(string outputFile, Guid guid)
        { 
			var folderName = new FileInfo(outputFile).Directory.Name;
			
			if (!folderName.Equals(guid.ToString())) {
				return;
            }

			Directory.Delete(Path.GetDirectoryName(outputFile), recursive: true);
		}

        private (string outputFile, Guid guid) GetUniqueOutputSubfolder(String jobFile)
        { 
			Guid guid = Guid.NewGuid();

			string outputFile = Path.Combine(Path.GetDirectoryName(jobFile), guid.ToString(), Path.GetFileName(jobFile));

			if (!Directory.Exists(Path.GetDirectoryName(outputFile))) {
				Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
            }

			return (outputFile, guid);
		}

        private void AssertMetricsAreWithinLimits(SimulatorFactory factory, Dictionary<String, double> metrics)
        {
			factory.SumData.Finish();

			var table = VectoCSVFile.Read(((FileOutputWriter) factory.ModWriter).SumFileName, true, true);
			var row = table.Rows[0];

			Dictionary<string, double> results = new Dictionary<string, double>();

			foreach (var kvp in metrics) {
				double result;
				Assert.IsTrue(double.TryParse(row[kvp.Key].ToString(), out result));

				results.Add(kvp.Key, result);

				TestContext.WriteLine($"{kvp.Key} = {result}   (expected = {kvp.Value})");
            }

			foreach (var kvp in metrics) {
				Assert.IsTrue(results[kvp.Key].IsEqual(kvp.Value, 1e-3), $"{kvp.Key} ({results[kvp.Key]}) is other than expected ({kvp.Value})");
			}
        }

    }
}
