using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using ScottPlot.Drawing.Colormaps;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell;
using TUGraz.VectoCore.Tests.Models.SimulationComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Tests.Utils.RunDataHelper;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.Tests.Integration.FuelCell
{
    [TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class FuelCellVehicleTest
	{

		protected const string FCHV_E2_JOB = @"TestData/H2_FCV/GenericVehicleE2 - FCHV/FCHV_singleFc.vecto";

		protected const string FCHV_E2_JOB_300kW = @"TestData/H2_FCV/GenericVehicleE2 - FCHV/FCHV_singleFc_300kW_fc.vecto";
        protected const string FCHV_E2_JOB_multipleFC = @"TestData/H2_FCV/Group 2 FCHV 100kW FCS/Gr2_FCHV_100kW_FCS_10kWhBat_multipleFc.vecto";

		protected const string FCHV_IEPC_JOB_1 = @"TestData/H2_FCV/FCHV_IEPC/IEPC_Gbx3Speed_FC/IEPC_ENG_Gbx3.vecto";

		private string TEST_WORKING_DIR;

        [OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			//TEST_WORKING_DIR = Directory.GetCurrentDirectory();
			//Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(FCHV_E2_JOB, 0, TestName = "FCHV E2 Job RD single FC")]
		
		[TestCase(FCHV_E2_JOB_multipleFC, 0, 
			TestName="FCHV E2 Job RD multiple FC")]

		[TestCase(FCHV_IEPC_JOB_1, 0, TestName="FCHV_IEPC_Job")]

		public void E2_FCHV_Job(string jobFile, int cycleIdx)
		{
			var run = GetRun(jobFile, cycleIdx, out var pt, out _);
			TestContext.Progress.WriteLine(
				$"Usable energy {pt.RunData.BatteryData.UseableStoredEnergy.ConvertToKiloWattHour()}");
            run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		private static IVectoRun GetRun(string jobFile, int cycleIdx, out IVehicleContainer pt, out IEngineeringInputDataProvider inputProvider, string fileWriterSuffix = "")
		{
			return GetRun(jobFile, cycleIdx, out pt, out inputProvider, null, fileWriterSuffix);
		}

		private static IVectoRun GetRun(string jobFile, int cycleIdx, out IVehicleContainer pt, out IEngineeringInputDataProvider inputProvider, Action<IEngineeringInputDataProvider> modifyInputData,string fileWriterSuffix = "")
		{
			inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile) as IEngineeringInputDataProvider;

			if (modifyInputData != null) {
				modifyInputData(inputProvider);
			}

			//var outputFile = jobFile.Replace(".vecto", fileWriterSuffix + ".vecto");
			var writer = new FileOutputWriter(jobFile);
			TestContext.Progress.WriteLine($"Creating job using {jobFile}");

			
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.Validate = false;
			factory.WriteModalResults = true;

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			var run = factory.SimulationRuns().ToArray()[cycleIdx];

			Assert.NotNull(run);

			pt = run.GetContainer();

			Assert.NotNull(pt);
			return run;
		}





        [TestCase(FCHV_E2_JOB, 0, 100, 10, 300, TestName = "FCHV E2 Job RD single FC, 100kWh  , 300 kW 0")]
		[TestCase(FCHV_E2_JOB, 0, 100, 10, 500, TestName = "FCHV E2 Job RD single FC, 100kWh  , 500 kW 0")]

		[TestCase(FCHV_E2_JOB, 0, 80,  10, 300, TestName = "FCHV E2 Job RD single FC, 80kWh   , 300 kW 0" )]
		[TestCase(FCHV_E2_JOB, 0, 80,  10, 500, TestName = "FCHV E2 Job RD single FC, 80kWh   , 500 kW 0" )]

		[TestCase(FCHV_E2_JOB, 0, 50,  10, 300, TestName = "FCHV E2 Job RD single FC, 50kWh   , 300 kW 0" )]
		[TestCase(FCHV_E2_JOB, 0, 50,  10, 500, TestName = "FCHV E2 Job RD single FC, 50kWh   , 500 kW 0" )]

		[TestCase(FCHV_E2_JOB, 0, 35,  10, 300, TestName = "FCHV E2 Job RD single FC, 35kWh   , 300 kW 0" )]
		[TestCase(FCHV_E2_JOB, 0, 35,  10, 500, TestName = "FCHV E2 Job RD single FC, 35kWh   , 500 kW 0" )]

		[TestCase(FCHV_E2_JOB, 0, 20,  10, 300, TestName = "FCHV E2 Job RD single FC, 20kWh   , 300 kW 0" )]
		[TestCase(FCHV_E2_JOB, 0, 20,  10, 500, TestName = "FCHV E2 Job RD single FC, 20kWh   , 500 kW 0" )]

		[TestCase(FCHV_E2_JOB, 0, 10, 10, 300, TestName = "FCHV E2 Job RD single FC, 10kWh   , 300 kW 0")]
		[TestCase(FCHV_E2_JOB, 0, 10, 10, 500, TestName = "FCHV E2 Job RD single FC, 10kWh   , 500 kW 0")]

		[TestCase(FCHV_E2_JOB, 0, 5, 10, 300, TestName = "FCHV E2 Job RD single FC, 5kWh    , 300 kW 0")]
		[TestCase(FCHV_E2_JOB, 0, 5, 10, 500, TestName = "FCHV E2 Job RD single FC, 5kWh    , 500 kW 0")]

		[TestCase(FCHV_E2_JOB, 0, 1, 10, 300, TestName = "FCHV E2 Job RD single FC, 1kWh    , 300 kW 0", Ignore = "Battery too small")]
		[TestCase(FCHV_E2_JOB, 0, 1, 10, 500, TestName = "FCHV E2 Job RD single FC, 1kWh    , 500 kW 0", Ignore = "Battery too small")]

		//[TestCase(FCHV_E2_JOB, 0, 0.1, 10, 300, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 300 kW 0")]
		//[TestCase(FCHV_E2_JOB, 0, 0.1, 10, 500, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 500 kW 0")]

		[TestCase(FCHV_E2_JOB, 1, 100, 10, 100, TestName = "FCHV E2 Job RD single FC, 100kWh  , 100 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 100, 10, 300, TestName = "FCHV E2 Job RD single FC, 100kWh  , 300 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 100, 10, 500, TestName = "FCHV E2 Job RD single FC, 100kWh  , 500 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 80, 10, 100, TestName = "FCHV E2 Job RD single FC, 80kWh    , 100 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 80, 10, 300, TestName = "FCHV E2 Job RD single FC, 80kWh    , 300 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 80, 10, 500, TestName = "FCHV E2 Job RD single FC, 80kWh    , 500 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 50, 10, 100, TestName = "FCHV E2 Job RD single FC, 50kWh    , 100 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 50, 10, 300, TestName = "FCHV E2 Job RD single FC, 50kWh    , 300 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 50, 10, 500, TestName = "FCHV E2 Job RD single FC, 50kWh    , 500 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 35, 10, 100, TestName = "FCHV E2 Job RD single FC, 35kWh    , 100 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 35, 10, 300, TestName = "FCHV E2 Job RD single FC, 35kWh    , 300 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 35, 10, 500, TestName = "FCHV E2 Job RD single FC, 35kWh    , 500 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 20, 10, 100, TestName = "FCHV E2 Job RD single FC, 20kWh    , 100 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 20, 10, 300, TestName = "FCHV E2 Job RD single FC, 20kWh    , 300 kW 1")]
        [TestCase(FCHV_E2_JOB, 1, 20, 10, 500, TestName = "FCHV E2 Job RD single FC, 20kWh    , 500 kW 1")]
		[TestCase(FCHV_E2_JOB, 1, 10, 10, 100, TestName = "FCHV E2 Job RD single FC, 10kWh    , 100 kW 1")]
		[TestCase(FCHV_E2_JOB, 1, 10, 10, 300, TestName = "FCHV E2 Job RD single FC, 10kWh    , 300 kW 1")]
		[TestCase(FCHV_E2_JOB, 1, 10, 10, 500, TestName = "FCHV E2 Job RD single FC, 10kWh    , 500 kW 1")]
		[TestCase(FCHV_E2_JOB, 1, 5, 10, 100, TestName = "FCHV E2 Job RD single FC, 5kWh      , 100 kW 1", Ignore = "Battery too small")]
		[TestCase(FCHV_E2_JOB, 1, 5, 10, 300, TestName = "FCHV E2 Job RD single FC, 5kWh      , 300 kW 1")]
		[TestCase(FCHV_E2_JOB, 1, 5, 10, 500, TestName = "FCHV E2 Job RD single FC, 5kWh      , 500 kW 1")]
		[TestCase(FCHV_E2_JOB, 1, 1, 10, 100, TestName = "FCHV E2 Job RD single FC, 1kWh      , 100 kW 1", Ignore = "Battery too small")]
        [TestCase(FCHV_E2_JOB, 1, 1, 10, 300, TestName = "FCHV E2 Job RD single FC, 1kWh      , 300 kW 1", Ignore = "Battery too small")]
        [TestCase(FCHV_E2_JOB, 1, 1, 10, 500, TestName = "FCHV E2 Job RD single FC, 1kWh      , 500 kW 1", Ignore = "Battery too small")]
		//[TestCase(FCHV_E2_JOB, 1, 0.1, 10, 100, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 100 kW 1")]
		//[TestCase(FCHV_E2_JOB, 1, 0.1, 10, 300, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 300 kW 1")]
		//[TestCase(FCHV_E2_JOB, 1, 0.1, 10, 500, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 500 kW 1")]

		[TestCase(FCHV_E2_JOB, 2, 100, 10, 100, TestName = "FCHV E2 Job RD single FC, 100kWh  , 100 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 100, 10, 300, TestName = "FCHV E2 Job RD single FC, 100kWh  , 300 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 100, 10, 500, TestName = "FCHV E2 Job RD single FC, 100kWh  , 500 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 80, 10, 100, TestName = "FCHV E2 Job RD single FC, 80kWh    , 100 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 80, 10, 300, TestName = "FCHV E2 Job RD single FC, 80kWh    , 300 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 80, 10, 500, TestName = "FCHV E2 Job RD single FC, 80kWh    , 500 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 50, 10, 100, TestName = "FCHV E2 Job RD single FC, 50kWh    , 100 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 50, 10, 300, TestName = "FCHV E2 Job RD single FC, 50kWh    , 300 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 50, 10, 500, TestName = "FCHV E2 Job RD single FC, 50kWh    , 500 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 35, 10, 100, TestName = "FCHV E2 Job RD single FC, 35kWh    , 100 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 35, 10, 300, TestName = "FCHV E2 Job RD single FC, 35kWh    , 300 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 35, 10, 500, TestName = "FCHV E2 Job RD single FC, 35kWh    , 500 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 20, 10, 100, TestName = "FCHV E2 Job RD single FC, 20kWh    , 100 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 20, 10, 300, TestName = "FCHV E2 Job RD single FC, 20kWh    , 300 kW 2")]
        [TestCase(FCHV_E2_JOB, 2, 20, 10, 500, TestName = "FCHV E2 Job RD single FC, 20kWh    , 500 kW 2")]


		[TestCase(FCHV_E2_JOB, 2, 10, 10, 100, TestName = "FCHV E2 Job RD single FC, 10kWh    , 100 kW 2")]
		[TestCase(FCHV_E2_JOB, 2, 10, 10, 300, TestName = "FCHV E2 Job RD single FC, 10kWh    , 300 kW 2")]
		[TestCase(FCHV_E2_JOB, 2, 10, 10, 500, TestName = "FCHV E2 Job RD single FC, 10kWh    , 500 kW 2")]
		[TestCase(FCHV_E2_JOB, 2, 5, 10, 100, TestName = "FCHV E2 Job RD single FC, 5kWh      , 100 kW 2")]
		[TestCase(FCHV_E2_JOB, 2, 5, 10, 300, TestName = "FCHV E2 Job RD single FC, 5kWh      , 300 kW 2")]
		[TestCase(FCHV_E2_JOB, 2, 5, 10, 500, TestName = "FCHV E2 Job RD single FC, 5kWh      , 500 kW 2")]



		[TestCase(FCHV_E2_JOB, 1, 100, 10, 500, 150, TestName = "FCHV E2 Job RD single FC, 100kWh  , 500 kW ltd charge")]
		[TestCase(FCHV_E2_JOB, 2, 80, 10, 300, 150, TestName = "FCHV E2 Job RD single FC, 80kWh    , 300 kW ltd charge")]
		[TestCase(FCHV_E2_JOB, 1, 20, 10, 500, 100, TestName = "FCHV E2 Job RD single FC, 20kWh    , 500 kW ltd charge")]

        [Parallelizable(ParallelScope.Children)]
		//[TestCase(FCHV_E2_JOB, 2, 1, 10, 100, TestName = "FCHV E2 Job RD single FC, 1kWh      , 100 kW 2")]
		//[TestCase(FCHV_E2_JOB, 2, 1, 10, 300, TestName = "FCHV E2 Job RD single FC, 1kWh      , 300 kW 2")]
		//[TestCase(FCHV_E2_JOB, 2, 1, 10, 500, TestName = "FCHV E2 Job RD single FC, 1kWh      , 500 kW 2")]
		//Battery safety margin
		//[TestCase(FCHV_E2_JOB, 2, 0.1, 10, 100, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 100 kW 2")]
		//[TestCase(FCHV_E2_JOB, 2, 0.1, 10, 300, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 300 kW 2")]
		//[TestCase(FCHV_E2_JOB, 2, 0.1, 10, 500, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 500 kW 2")]
		public void E2_FCHV_Job_var_capacity(string jobFile, int cycleIdx, double usable_energy_kWh, double min_fcPower_kW, double max_fcPower_kW, double? maxChargingPower_kW = null)
		{
			var targetDir = @$"TestResults/FCHV_bat_{usable_energy_kWh}_kWh_fc_{max_fcPower_kW}_kW_{cycleIdx}";
			CopyToOutputDirectory(Path.GetDirectoryName(jobFile), targetDir);
			jobFile = Path.Combine(targetDir, Path.GetFileName(jobFile));




			var run = GetRun(jobFile, cycleIdx, out var pt, out var inputData, provider => {
				var components = provider.JobInputData.Vehicle.Components;
				//components.FuelCellSystemInputData = new Mock<IFuelCellSystemEngineeringInputData>().Object;

				var fcProperty = components.GetType().GetField("_fuelCellSystem", System.Reflection.BindingFlags.NonPublic
					| System.Reflection.BindingFlags.Instance);
				fcProperty.SetValue(components, GetFuelCellSystemInputData((max_fcPower_kW * 1E3).SI<Watt>(), (min_fcPower_kW * 1E3).SI<Watt>()));
				TestContext.Progress.WriteLine(
					$"FC ({components.FuelCellSystemInputData.FuelCellStrings.Single().FuelCellComponent.MinElectricPower}/" +
					$"{components.FuelCellSystemInputData.FuelCellStrings.Single().FuelCellComponent.MaxElectricPower}) ");

            }, fileWriterSuffix: $"fc_{min_fcPower_kW}_kW_usable_bat_{usable_energy_kWh}_kWh");

    



			
			var engineeringDao = new EngineeringDataAdapter();
			var vehicle = inputData.JobInputData.Vehicle;

		

			var batData = engineeringDao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
			var rd = pt.RunData;

			var usableWs = usable_energy_kWh.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();
			batData = batData.SetUsableCapacity(usableWs);
			if (maxChargingPower_kW.HasValue) {
				var maxChargingPower = maxChargingPower_kW.Value.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
				batData = batData.SetMaxChargingPower(maxChargingPower);
            }
	
			rd.BatteryData =
				engineeringDao.CreateFuelCellPreProcessingBattery(vehicle.Components.FuelCellSystemInputData, batData.Clone(), out _);


			Assert.AreEqual(usable_energy_kWh, batData.UseableStoredEnergy.ConvertToKiloWattHour().Value, 1E-3);

			try {
				run.Run();
			} catch (Exception ex) {
				rd = run.GetContainer().RunData;
				var postProcessing = rd.FuelCellSystemData.PreRunPostProcessing;
				TestContext.Progress.WriteLine(ex.Message);
				WritePostprocessingInfo(rd.FuelCellSystemData?.PreRunPostProcessing);

				throw;
			}
			Assert.AreEqual(max_fcPower_kW, run.GetContainer().RunData.FuelCellSystemData.FuelCellStrings.First().MaxPower.ConvertToKiloWatt().Value);
            Assert.IsTrue(run.FinishedWithoutErrors);
			

			rd = run.GetContainer().RunData;

			var fcs = rd.FuelCellSystemData;
			var pP = fcs.PreRunPostProcessing;
			

			WritePostprocessingInfo(pP);
			Assert.That(rd.BatteryData.InitialSoC, Is.EqualTo(pP.StartSoC));
		}

		
		public void CopyToOutputDirectory(string source, string target)
		{
			Directory.CreateDirectory(target);


			var sourcDirInfo = new DirectoryInfo(source);
			var targetDirInfo = new DirectoryInfo(target);

				
			foreach (var fileInfo in sourcDirInfo.EnumerateFiles()) {

				fileInfo.CopyTo(Path.Combine(target, fileInfo.Name), true);
			}

			foreach (var dirInfo in sourcDirInfo.EnumerateDirectories()) {
				CopyToOutputDirectory(dirInfo.FullName, targetDirInfo.CreateSubdirectory(dirInfo.Name).FullName);
			}
		}

		[TestCase(FCHV_E2_JOB, 0, 100, 10, 20, TestName = "FCHV E2 Job RD single FC, 100kWh  , 20 kW 0")]
		[TestCase(FCHV_E2_JOB, 0, 50, 10, 100, TestName = "FCHV E2 Job RD single FC, 100kWh  , 100 kW 0")]
		[TestCase(FCHV_E2_JOB, 0, 5, 9, 10, TestName = "FCHV E2 Job RD single FC, 100kWh  , 10 kW 0")]
		[TestCase(FCHV_E2_JOB, 0, 5, 10, 115, TestName = "FCHV E2 Job RD single FC, 100kWh  , 115 kW 0")]
        //[TestCase(FCHV_E2_JOB, 0, 80,  10, 100, TestName = "FCHV E2 Job RD single FC, 80kWh   , 100 kW 0")]
        //[TestCase(FCHV_E2_JOB, 0, 50,  10, 100, TestName = "FCHV E2 Job RD single FC, 50kWh   , 100 kW 0")]
        //[TestCase(FCHV_E2_JOB, 0, 35,  10, 100, TestName = "FCHV E2 Job RD single FC, 35kWh   , 100 kW 0")]
        //[TestCase(FCHV_E2_JOB, 0, 20,  10, 100, TestName = "FCHV E2 Job RD single FC, 20kWh   , 100 kW 0")]
        //[TestCase(FCHV_E2_JOB, 0, 10,  10, 100, TestName = "FCHV E2 Job RD single FC, 10kWh   , 100 kW 0")]
        //[TestCase(FCHV_E2_JOB, 0, 5,   10, 100, TestName = "FCHV E2 Job RD single FC, 5kWh    , 100 kW 0")]
        //[TestCase(FCHV_E2_JOB, 0, 1,   10, 100, TestName = "FCHV E2 Job RD single FC, 1kWh    , 100 kW 0")]

        public void E2_FCHV_FuelCellPowerNotSufficient(string jobFile, int cycleIdx, double usable_energy_kWh, double min_fcPower_kW, double max_fcPower_kW)
		{

			var exception = Assert.Throws<VectoException>(() => {
				E2_FCHV_Job_var_capacity(jobFile, cycleIdx, usable_energy_kWh, min_fcPower_kW, max_fcPower_kW);
			});
			Assert.NotNull(exception);
			Assert.That(exception.Message.Contains("FuelCell power not sufficient"), Is.True);


		}

        public void WritePostprocessingInfo(IFuelCellPreRunInfo pP)
		{
			if (pP == null) {
				TestContext.WriteLine("PostProcessingInfo not available");
				return;
			}
			TestContext.Progress.WriteLine($"Window Size {pP.WindowSize} \n" +
											$"found after {pP.BinarySearchIterations} iterations \n" +
											$"SoC {pP.StartSoC}");

        }




        IFuelCellSystemEngineeringInputData GetFuelCellSystemInputData(Watt maxPower, Watt minPower)
		{
			var massFlowMap = new TableData() { };
			massFlowMap.Columns.AddRange(new DataColumn[] {
				new DataColumn() {
					ColumnName = FuelCellMassFlowMapReader.Fields.ElectricPower,
					DataType = typeof(string),
                },
				new DataColumn() {
					ColumnName = FuelCellMassFlowMapReader.Fields.m_H2,
					DataType = typeof(string),
				}
			});

			var nrOfEntries = 10;
			var maxPower_kW = maxPower.Value() / 1000;
			var minPower_kW = minPower.Value() / 1000;
			var step = (maxPower_kW - minPower_kW) / nrOfEntries;


			for (int i = 0; i <= nrOfEntries; i++) {
				var row = massFlowMap.NewRow();
				var power = minPower_kW + step * i;
				row[0] = power.ToString(CultureInfo.InvariantCulture);
				row[1] = (power * 100).ToString(CultureInfo.InvariantCulture);


				massFlowMap.Rows.Add(row);
			}

			var fuelCellSystemMock = new Mock<IFuelCellSystemEngineeringInputData>();


			var fuelCellComponentMock = new Mock<IFuelCellComponentEngineeringInputData>();
			var fuelCellComponentEntry = new FuelCellStringEntry<IFuelCellComponentEngineeringInputData>() {
				Count = 1,
				FuelCellComponent = fuelCellComponentMock.Object,
			};
			fuelCellSystemMock.Setup(fcs => fcs.FuelCellStrings).Returns(
				new List<FuelCellStringEntry<IFuelCellComponentEngineeringInputData>>() {
					fuelCellComponentEntry
				});

			fuelCellComponentMock.Setup(fcc => fcc.MaxElectricPower)
				.Returns(maxPower);
			fuelCellComponentMock.Setup(fcc => fcc.MinElectricPower)
				.Returns(minPower);
			fuelCellComponentMock.Setup(fcc => fcc.MassFlowMap)
				.Returns(massFlowMap);

            return fuelCellSystemMock.Object;
		}
	}

}
