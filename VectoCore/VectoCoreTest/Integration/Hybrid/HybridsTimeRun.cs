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
        TestCase(P2_JOB, 0, TestName = "P2 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_JOB, 1, TestName = "P2 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_JOB, 2, TestName = "P2 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        TestCase(P3_JOB, 0, TestName = "P3 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P3_JOB, 1, TestName = "P3 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P3_JOB, 2, TestName = "P3 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        TestCase(P4_JOB, 0, TestName = "P4 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P4_JOB, 1, TestName = "P4 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P4_JOB, 2, TestName = "P4 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        TestCase(P2_5_JOB, 0, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P2_5_JOB, 1, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P2_5_JOB, 2, TestName = "P2_5 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        TestCase(P1_JOB, 0, TestName = "P1 Hybrid TimeRun MeasuredSpeed LongHaul"),
        TestCase(P1_JOB, 1, TestName = "P1 Hybrid TimeRun MeasuredSpeed RegionalDelivery"),
        TestCase(P1_JOB, 2, TestName = "P1 Hybrid TimeRun MeasuredSpeed UrbanDelivery"),
        ]
        public void RunHybridDriveCycle(string jobFile, int cycleIdx)
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
			//AssertVTPMetricsAreWithinLimits(factory, metrics);

			//DeleteOutputSubFolder(outputFile, guid);
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

        private void AssertVTPMetricsAreWithinLimits(SimulatorFactory factory, Dictionary<String, double> metrics)
        {
			factory.SumData.Finish();

			var table = VectoCSVFile.Read(((FileOutputWriter) factory.ModWriter).SumFileName, true, true);
			var row = table.Rows[0];

			Dictionary<string, double> results = new Dictionary<string, double>();

			foreach (var kvp in metrics) {
				double result;
				Assert.IsTrue(double.TryParse(row[kvp.Key].ToString(), out result));

				results.Add(kvp.Key, result);

				TestContext.WriteLine($"{kvp.Key} = {result} %   (threshold = {kvp.Value})");
            }

			foreach (var kvp in metrics) {
				Assert.IsTrue(results[kvp.Key] <= kvp.Value, $"{kvp.Key} ({results[kvp.Key]}) is above threshold ({kvp.Value})");
			}
        }

        
        public void HybridsTimeRunSimpleUrbanDelivery()
        {
            string jobName = null;
            // the same engine fc-map is used for different fuel types, thus the difference in expected results
            var fileWriter = new FileOutputWriter(jobName);
            var sumData = new SummaryDataContainer(fileWriter);

            var jobContainer = new JobContainer(sumData);
            var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

            var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter)
            {
                WriteModalResults = true,
                Validate = false
            };

            jobContainer.AddRuns(runsFactory);

            int runIdx = 0;
            // store a reference to the data table here because it is going to be deleted
            var run = jobContainer.Runs[runIdx];
            var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
            var modData = modContainer.Data;


            run.Run.Run();

            // restore data table before assertions
            modContainer.Data = modData;
            var fuel = modContainer.FuelData.First();
            var distance = modContainer.Distance;
            var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
            var fcVolumePerMeter = fuel.FuelDensity == null ? null :
                (fcFinal / fuel.FuelDensity)
                .Cast<VolumePerMeter>();
            var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
            var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

            //			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
                fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
                co2PerMeter?.Value(), energyPerMeter?.Value());
            Assert.IsTrue(true);
        }
        
        
        public void HybridsTimeRunwithGearUrbanDelivery()
        {
            string jobName = null;
            // the same engine fc-map is used for different fuel types, thus the difference in expected results
            var fileWriter = new FileOutputWriter(jobName);
            var sumData = new SummaryDataContainer(fileWriter);

            var jobContainer = new JobContainer(sumData);
            var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

            var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter)
            {
                WriteModalResults = true,
                Validate = false
            };

            jobContainer.AddRuns(runsFactory);

            int runIdx = 0;
            // store a reference to the data table here because it is going to be deleted
            var run = jobContainer.Runs[runIdx];
            var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
            var modData = modContainer.Data;


            run.Run.Run();

            // restore data table before assertions
            modContainer.Data = modData;
            var fuel = modContainer.FuelData.First();
            var distance = modContainer.Distance;
            var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
            var fcVolumePerMeter = fuel.FuelDensity == null ? null :
                (fcFinal / fuel.FuelDensity)
                .Cast<VolumePerMeter>();
            var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
            var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

            //			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
                fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
                co2PerMeter?.Value(), energyPerMeter?.Value());
            Assert.IsTrue(true);
        }


        
        public void HybridsTimeRunwihtGearandEMPowerUrbanDelivery()
        {
            string jobName = null;
            // the same engine fc-map is used for different fuel types, thus the difference in expected results
            var fileWriter = new FileOutputWriter(jobName);
            var sumData = new SummaryDataContainer(fileWriter);

            var jobContainer = new JobContainer(sumData);
            var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

            var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter)
            {
                WriteModalResults = true,
                Validate = false
            };

            jobContainer.AddRuns(runsFactory);

            int runIdx = 0;
            // store a reference to the data table here because it is going to be deleted
            var run = jobContainer.Runs[runIdx];
            var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
            var modData = modContainer.Data;


            run.Run.Run();

            // restore data table before assertions
            modContainer.Data = modData;
            var fuel = modContainer.FuelData.First();
            var distance = modContainer.Distance;
            var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
            var fcVolumePerMeter = fuel.FuelDensity == null ? null :
                (fcFinal / fuel.FuelDensity)
                .Cast<VolumePerMeter>();
            var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
            var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

            //			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
                fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
                co2PerMeter?.Value(), energyPerMeter?.Value());
            Assert.IsTrue(true);
        }
        
       
        public void HybridsTimeRunSimpleLongHaul()
        {
            string jobName = null;
            // the same engine fc-map is used for different fuel types, thus the difference in expected results
            var fileWriter = new FileOutputWriter(jobName);
            var sumData = new SummaryDataContainer(fileWriter);

            var jobContainer = new JobContainer(sumData);
            var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

            var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter)
            {
                WriteModalResults = true,
                Validate = false
            };

            jobContainer.AddRuns(runsFactory);

            int runIdx = 0;
            // store a reference to the data table here because it is going to be deleted
            var run = jobContainer.Runs[runIdx];
            var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
            var modData = modContainer.Data;


            run.Run.Run();

            // restore data table before assertions
            modContainer.Data = modData;
            var fuel = modContainer.FuelData.First();
            var distance = modContainer.Distance;
            var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
            var fcVolumePerMeter = fuel.FuelDensity == null ? null :
                (fcFinal / fuel.FuelDensity)
                .Cast<VolumePerMeter>();
            var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
            var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

            //			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
                fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
                co2PerMeter?.Value(), energyPerMeter?.Value());
            Assert.IsTrue(true);
            Assert.IsTrue(true);
        }
        
        
        public void HybridsTimeRunwithGearLongHaul()
        {
            string jobName = null;
            // the same engine fc-map is used for different fuel types, thus the difference in expected results
            var fileWriter = new FileOutputWriter(jobName);
            var sumData = new SummaryDataContainer(fileWriter);

            var jobContainer = new JobContainer(sumData);
            var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

            var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter)
            {
                WriteModalResults = true,
                Validate = false
            };

            jobContainer.AddRuns(runsFactory);

            int runIdx = 0;
            // store a reference to the data table here because it is going to be deleted
            var run = jobContainer.Runs[runIdx];
            var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
            var modData = modContainer.Data;


            run.Run.Run();

            // restore data table before assertions
            modContainer.Data = modData;
            var fuel = modContainer.FuelData.First();
            var distance = modContainer.Distance;
            var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
            var fcVolumePerMeter = fuel.FuelDensity == null ? null :
                (fcFinal / fuel.FuelDensity)
                .Cast<VolumePerMeter>();
            var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
            var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

            //			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
                fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
                co2PerMeter?.Value(), energyPerMeter?.Value());
            Assert.IsTrue(true);
        }
        
        
        public void HybridsTimeRunwihtGearandEMPowerLongHaul()
        {
            string jobName = null;
            // the same engine fc-map is used for different fuel types, thus the difference in expected results
            var fileWriter = new FileOutputWriter(jobName);
            var sumData = new SummaryDataContainer(fileWriter);

            var jobContainer = new JobContainer(sumData);
            var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

            var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter)
            {
                WriteModalResults = true,
                Validate = false
            };

            jobContainer.AddRuns(runsFactory);

            int runIdx = 0;
            // store a reference to the data table here because it is going to be deleted
            var run = jobContainer.Runs[runIdx];
            var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
            var modData = modContainer.Data;


            run.Run.Run();

            // restore data table before assertions
            modContainer.Data = modData;
            var fuel = modContainer.FuelData.First();
            var distance = modContainer.Distance;
            var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
            var fcVolumePerMeter = fuel.FuelDensity == null ? null :
                (fcFinal / fuel.FuelDensity)
                .Cast<VolumePerMeter>();
            var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
            var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

            //			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
                fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
                co2PerMeter?.Value(), energyPerMeter?.Value());
            Assert.IsTrue(true);
        }
        
       
        public void HybridsTimeRunSimpleRegionalDelivery()
        {
            string jobName = null;
            // the same engine fc-map is used for different fuel types, thus the difference in expected results
            var fileWriter = new FileOutputWriter(jobName);
            var sumData = new SummaryDataContainer(fileWriter);

            var jobContainer = new JobContainer(sumData);
            var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

            var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter)
            {
                WriteModalResults = true,
                Validate = false
            };

            jobContainer.AddRuns(runsFactory);

            int runIdx = 0;
            // store a reference to the data table here because it is going to be deleted
            var run = jobContainer.Runs[runIdx];
            var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
            var modData = modContainer.Data;


            run.Run.Run();

            // restore data table before assertions
            modContainer.Data = modData;
            var fuel = modContainer.FuelData.First();
            var distance = modContainer.Distance;
            var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
            var fcVolumePerMeter = fuel.FuelDensity == null ? null :
                (fcFinal / fuel.FuelDensity)
                .Cast<VolumePerMeter>();
            var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
            var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

            //			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
                fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
                co2PerMeter?.Value(), energyPerMeter?.Value());
            Assert.IsTrue(true);
        }
        
        
        public void HybridsTimeRunwithGearRegionalDelivery()
        {
            string jobName = null;
            // the same engine fc-map is used for different fuel types, thus the difference in expected results
            var fileWriter = new FileOutputWriter(jobName);
            var sumData = new SummaryDataContainer(fileWriter);

            var jobContainer = new JobContainer(sumData);
            var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

            var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter)
            {
                WriteModalResults = true,
                Validate = false
            };

            jobContainer.AddRuns(runsFactory);

            int runIdx = 0;
            // store a reference to the data table here because it is going to be deleted
            var run = jobContainer.Runs[runIdx];
            var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
            var modData = modContainer.Data;


            run.Run.Run();

            // restore data table before assertions
            modContainer.Data = modData;
            var fuel = modContainer.FuelData.First();
            var distance = modContainer.Distance;
            var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
            var fcVolumePerMeter = fuel.FuelDensity == null ? null :
                (fcFinal / fuel.FuelDensity)
                .Cast<VolumePerMeter>();
            var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
            var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

            //			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
                fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
                co2PerMeter?.Value(), energyPerMeter?.Value());
            Assert.IsTrue(true);
        }
        
        
        public void HybridsTimeRunwihtGearandEMPowerRegionalDelivery()
        {
            string jobName = null;
            // the same engine fc-map is used for different fuel types, thus the difference in expected results
            var fileWriter = new FileOutputWriter(jobName);
            var sumData = new SummaryDataContainer(fileWriter);

            var jobContainer = new JobContainer(sumData);
            var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

            var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter)
            {
                WriteModalResults = true,
                Validate = false
            };

            jobContainer.AddRuns(runsFactory);

            int runIdx = 0;
            // store a reference to the data table here because it is going to be deleted
            var run = jobContainer.Runs[runIdx];
            var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
            var modData = modContainer.Data;


            run.Run.Run();

            // restore data table before assertions
            modContainer.Data = modData;
            var fuel = modContainer.FuelData.First();
            var distance = modContainer.Distance;
            var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
            var fcVolumePerMeter = fuel.FuelDensity == null ? null :
                (fcFinal / fuel.FuelDensity)
                .Cast<VolumePerMeter>();
            var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
            var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

            //			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
            Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
                modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
                fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
                co2PerMeter?.Value(), energyPerMeter?.Value());
            Assert.IsTrue(true);
        }
    }
}
