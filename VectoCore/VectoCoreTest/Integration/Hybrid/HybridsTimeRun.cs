using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using Assert = NUnit.Framework.Assert;
using TestContext = NUnit.Framework.TestContext;

namespace TUGraz.VectoCore.Tests.Integration.Hybrid
{
    [TestClass]
    public class HybridsTimeRun
    {
        public const string CycleFile = @"TestData\Integration\FullPowerTrain\1-Gear-Test-dist.vdri";
        public const string CoachCycleFile = @"TestData\Integration\FullPowerTrain\Coach.vdri";
        public const string EngineFile = @"TestData\Components\24t Coach.veng";
        public const string AccelerationFile = @"TestData\Components\Coach.vacc";
        public const string GearboxLossMap = @"TestData\Components\Indirect Gear.vtlm";
        public const string AxleLossMap = @"TestData\Components\Axle.vtlm";
        public const string GearboxShiftPolygonFile = @"TestData\Components\ShiftPolygons.vgbs";

        const string TestJobP1_APTS = @"TestData\Hybrids\GenericVehicle_P1-APT\CityBus_AT_Ser.vecto";
        const string TestJobP1_APTP = @"TestData\Hybrids\GenericVehicle_P1-APT\CityBus_AT_PS.vecto";

        private const string TestJobCityBusP1_APTP = @"TestData\Hybrids\Citybus_P1-APT-P-220kW-7.7l\CityBus_AT-P.vecto";

        [OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
        }

        [
            TestCase(TestJobP1_APTS, 0, TestName = "P1 Hybrid APT-S, DriveCycle LongHaul"),
            TestCase(TestJobP1_APTS, 1, TestName = "P1 Hybrid APT-S, DriveCycle Coach"),
            TestCase(TestJobP1_APTS, 2, TestName = "P1 Hybrid APT-S, DriveCycle Construction"),
            TestCase(TestJobP1_APTS, 3, TestName = "P1 Hybrid APT-S, DriveCycle HeavyUrban"),
            TestCase(TestJobP1_APTS, 4, TestName = "P1 Hybrid APT-S, DriveCycle Interurban"),
            TestCase(TestJobP1_APTS, 5, TestName = "P1 Hybrid APT-S, DriveCycle MunicipalUtility"),
            TestCase(TestJobP1_APTS, 6, TestName = "P1 Hybrid APT-S, DriveCycle RegionalDelivery"),
            TestCase(TestJobP1_APTS, 7, TestName = "P1 Hybrid APT-S, DriveCycle Suburban"),
            TestCase(TestJobP1_APTS, 8, TestName = "P1 Hybrid APT-S, DriveCycle Urban"),
            TestCase(TestJobP1_APTS, 9, TestName = "P1 Hybrid APT-S, DriveCycle UrbanDelivery"),

            TestCase(TestJobP1_APTP, 0, TestName = "P1 Hybrid APT-P, DriveCycle LongHaul"),
            TestCase(TestJobP1_APTP, 1, TestName = "P1 Hybrid APT-P, DriveCycle Coach"),
            TestCase(TestJobP1_APTP, 2, TestName = "P1 Hybrid APT-P, DriveCycle Construction"),
            TestCase(TestJobP1_APTP, 3, TestName = "P1 Hybrid APT-P, DriveCycle HeavyUrban"),
            TestCase(TestJobP1_APTP, 4, TestName = "P1 Hybrid APT-P, DriveCycle Interurban"),
            TestCase(TestJobP1_APTP, 5, TestName = "P1 Hybrid APT-P, DriveCycle MunicipalUtility"),
            TestCase(TestJobP1_APTP, 6, TestName = "P1 Hybrid APT-P, DriveCycle RegionalDelivery"),
            TestCase(TestJobP1_APTP, 7, TestName = "P1 Hybrid APT-P, DriveCycle Suburban"),
            TestCase(TestJobP1_APTP, 8, TestName = "P1 Hybrid APT-P, DriveCycle Urban"),
            TestCase(TestJobP1_APTP, 9, TestName = "P1 Hybrid APT-P, DriveCycle UrbanDelivery"),

            TestCase(TestJobCityBusP1_APTP, 0, TestName = "P1 CityBus Hybrid APT-P, DriveCycle LongHaul"),
            TestCase(TestJobCityBusP1_APTP, 1, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Coach"),
            TestCase(TestJobCityBusP1_APTP, 2, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Construction"),
            TestCase(TestJobCityBusP1_APTP, 3, TestName = "P1 CityBus Hybrid APT-P, DriveCycle HeavyUrban"),
            TestCase(TestJobCityBusP1_APTP, 4, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Interurban"),
            TestCase(TestJobCityBusP1_APTP, 5, TestName = "P1 CityBus Hybrid APT-P, DriveCycle MunicipalUtility"),
            TestCase(TestJobCityBusP1_APTP, 6, TestName = "P1 CityBus Hybrid APT-P, DriveCycle RegionalDelivery"),
            TestCase(TestJobCityBusP1_APTP, 7, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Suburban"),
            TestCase(TestJobCityBusP1_APTP, 8, TestName = "P1 CityBus Hybrid APT-P, DriveCycle Urban"),
            TestCase(TestJobCityBusP1_APTP, 9, TestName = "P1 CityBus Hybrid APT-P, DriveCycle UrbanDelivery"),
]

        public void P1APTHybridDriveCycle(string jobFile, int cycleIdx)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

            var writer = new FileOutputWriter(jobFile);
            var factory = new SimulatorFactory(ExecutionMode.Engineering, inputProvider, writer)
            {
                Validate = false,
                WriteModalResults = true,
            };

            var sumContainer = new SummaryDataContainer(writer);
            var jobContainer = new JobContainer(sumContainer);

            factory.SumData = sumContainer;

            var run = factory.SimulationRuns().ToArray()[cycleIdx];

            Assert.NotNull(run);

            var pt = run.GetContainer();

            Assert.NotNull(pt);

            run.Run();
            Assert.IsTrue(run.FinishedWithoutErrors);

            //jobContainer.AddRuns(factory);
            //jobContainer.Execute();
            //jobContainer.WaitFinished();
            //Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
        }
        [TestMethod]
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
        [TestMethod]
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


        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
