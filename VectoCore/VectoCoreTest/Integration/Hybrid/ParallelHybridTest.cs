using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.Impl.ElectricSystem;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Integration.Hybrid
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ParallelHybridTest
	{
		//private ModalResultField[] Yfields;
		public const string MotorFile = @"TestData/Hybrids/ElectricMotor/GenericEMotor.vem";
		public const string BatFile = @"TestData/Hybrids/Battery/GenericBattery.vbat";

		public const string AccelerationFile = @"TestData/Components/Truck.vacc";
		public const string MotorFile240kW = @"TestData/Hybrids/ElectricMotor/GenericEMotor240kW.vem";

		public const string P1HybridMotor = @"TestData/Hybrids/GenericVehicle_P1-APT/GenericEMotor20kW.vem";
		public const string P1BatteryFile = @"TestData/Hybrids/GenericVehicle_P1-APT/GenericBattery.vbat";

		public const string GearboxIndirectLoss = @"TestData/Components/Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData/Components/Direct Gear.vtlm";


		public const string TorqueConverterSerial = @"TestData/Hybrids/GenericVehicle_P1-APT/TorqueConverter.vtcc";
		public const string TorqueConverterPowerSplit = @"TestData/Hybrids/GenericVehicle_P1-APT/TorqueConverterPowerSplit.vtcc";

		public const bool PlotGraphs = true;

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;
		protected IPowertrainBuilder _powertrainBuilder;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
			_powertrainBuilder = _kernel.Get<IPowertrainBuilder>();
			//InitGraphWriter();
		}


		private GraphWriter GetGraphWriter(ModalResultField[] emYFields)
		{
			var Yfields = new[] {
				ModalResultField.v_act, ModalResultField.altitude, ModalResultField.acc, ModalResultField.Gear,
				ModalResultField.P_ice_out, ModalResultField.REESSStateOfCharge, ModalResultField.FCMap
			}.Concat(emYFields).ToArray();

			var graphWriter = new GraphWriter();
			graphWriter.Xfields = new[] { ModalResultField.dist };
			graphWriter.Yfields = Yfields;
			graphWriter.Series1Label = "Hybrid";
			graphWriter.PlotIgnitionState = true;

			if (PlotGraphs) {
				graphWriter.Enable();
			} else {
				graphWriter.Disable();
			}

			return graphWriter;
		}

		// --------------------------------------

		[
		TestCase(30, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 30km/h SoC: 0.7, level"),
		TestCase(50, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 50km/h SoC: 0.7, level"),
		TestCase(70, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 70km/h SoC: 0.7, level"),

		TestCase(30, 0.25, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 30km/h SoC: 0.25, level"),
		TestCase(50, 0.25, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 50km/h SoC: 0.25, level"),
		TestCase(70, 0.25, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 70km/h SoC: 0.25, level"),

		TestCase(30, 0.5, 5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
		TestCase(50, 0.5, 5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
		TestCase(70, 0.5, 5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 70km/h SoC: 0.5, UH 5%"),

		TestCase(30, 0.5, -5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
		TestCase(50, 0.5, -5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
		TestCase(70, 0.5, -5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S ConstantSpeed 70km/h SoC: 0.5, DH 5%"),

		//------

		TestCase(30, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 30km/h SoC: 0.7, level"),
		TestCase(50, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 50km/h SoC: 0.7, level"),
		TestCase(70, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 70km/h SoC: 0.7, level"),

		TestCase(30, 0.25, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 30km/h SoC: 0.25, level"),
		TestCase(50, 0.25, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 50km/h SoC: 0.25, level"),
		TestCase(70, 0.25, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 70km/h SoC: 0.25, level"),

		TestCase(30, 0.5, 5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
		TestCase(50, 0.5, 5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
		TestCase(70, 0.5, 5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 70km/h SoC: 0.5, UH 5%"),

		TestCase(30, 0.5, -5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
		TestCase(50, 0.5, -5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
		TestCase(70, 0.5, -5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P ConstantSpeed 70km/h SoC: 0.5, DH 5%"),
		]
		public void P1HybridConstantSpeed(double vmax, double initialSoC, double slope, GearboxType gbxType)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P1_constant_{vmax}-{initialSoC}_{slope}_{gbxType.ToXMLFormat()}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP1;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: false, gearboxType: gbxType);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P1 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase(30, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S DriveOff 80km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 5, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S DriveOff 80km/h SoC: 0.7, UH 5"),
			TestCase(30, 0.22, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S DriveOff 30km/h SoC: 0.22, level"),

			TestCase(30, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P DriveOff 80km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 5, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P DriveOff 80km/h SoC: 0.7, UH 5"),
			TestCase(30, 0.22, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P DriveOff 30km/h SoC: 0.22, level"),

		]
		public void P1HybridDriveOff(double vmax, double initialSoC, double slope, GearboxType gbxType)
		{
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P1 });
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P1_acc_{vmax}-{initialSoC}_{slope}_{gbxType.ToXMLFormat()}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP1;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: false, gearboxType: gbxType);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			graphWriter.Write(modFilename);
		}

		[
			TestCase(50, 0.79, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S Brake Standstill 50km/h SoC: 0.79, level"),
			TestCase(50, 0.25, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S Brake Standstill 50km/h SoC: 0.25, level"),
			TestCase(50, 0.65, 0, GearboxType.ATSerial, TestName = "P1 Hybrid APT-S Brake Standstill 50km/h SoC: 0.65, level"),

			TestCase(50, 0.79, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P Brake Standstill 50km/h SoC: 0.79, level"),
			TestCase(50, 0.25, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P Brake Standstill 50km/h SoC: 0.25, level"),
			TestCase(50, 0.65, 0, GearboxType.ATPowerSplit, TestName = "P1 Hybrid APT-P Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void P1HybridBrakeStandstill(double vmax, double initialSoC, double slope, GearboxType gbxType)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P1_stop_{vmax}-{initialSoC}_{slope}_{gbxType.ToXMLFormat()}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP1;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: false, gearboxType: gbxType);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P1 });
			graphWriter.Write(modFilename);
		}

		const string TestJobP1_APTS = @"TestData/Hybrids/GenericVehicle_P1-APT/CityBus_AT_Ser.vecto";
		const string TestJobP1_APTP = @"TestData/Hybrids/GenericVehicle_P1-APT/CityBus_AT_PS.vecto";

		//private const string TestJobP1 = @"E:/QUAM/tmp/Citybus_P1-APT-S-175kW-6.8l_C1/CityBus_AT_Ser.vecto";

		private const string TestJobCityBusP1_APTP = @"TestData/Hybrids/Citybus_P1-APT-P-220kW-7.7l/CityBus_AT-P.vecto";

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
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.Validate = false;
			factory.WriteModalResults = true;

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

		private const string BASE_PATH = @"E:/QUAM/Workspace/VECTO-Bugreports_DEV/Bugreport Jobs/2022/";
        [TestCase(BASE_PATH + @"VECTO-1660_2022_11_02_VectoDaten_Iveco_CrosswayLE_MH/IVECO_CRW_LE_C9D_360hp_DNXT_7G_MH.vecto", 4),
        TestCase(BASE_PATH + @"VECTO-1660_2022_11_02_VectoDaten_Iveco_CrosswayLE_MH/IVECO_CRW_LE_C9D_360hp_DNXT_7G_MH.vecto", 3),
		TestCase(BASE_PATH + @"VECTO-1660_2022_11_02_VectoDaten_Iveco_CrosswayLE_MH/IVECO_CRW_LE_C9D_360hp_DNXT_7G_MH.vecto", 6),
        ]
		public void Vecto1660_BusAux_SmartPS_with_Hybrid(string jobFile, int cycleIdx)
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			var writer = new FileOutputWriter(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.Validate = false;
			factory.WriteModalResults = true;

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			var run = factory.SimulationRuns().ToArray()[cycleIdx];

			Assert.NotNull(run);

			var pt = run.GetContainer();

			Assert.NotNull(pt);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		// =======================================================================================

		[
			TestCase(30, 0.7, 0, TestName = "P2 Hybrid DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "P2 Hybrid DriveOff 80km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 5, TestName = "P2 Hybrid DriveOff 80km/h SoC: 0.7, UH 5"),
		TestCase(30, 0.22, 0, TestName = "P2 Hybrid DriveOff 30km/h SoC: 0.22, level"),
		]
		public void P2HybridDriveOff(double vmax, double initialSoC, double slope)
		{
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				  1200, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P2_acc_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			graphWriter.Write(modFilename);
		}

		[
			TestCase(80, 0.7, 5, TestName = "P2 Hybrid GbxTqLimit DriveOff 80km/h SoC: 0.7, UH 5%"),
		]
		public void P2HybridDriveOffGbxTqLimit(double vmax, double initialSoC, double slope)
		{
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				  1200, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P2_acc_{vmax}-{initialSoC}_{slope}_gbxLimit-2300.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, maxGearboxTorque: 2370.SI<NewtonMeter>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			graphWriter.Write(modFilename);
		}

		[
			TestCase(80, 0.7, 5, 0, TestName = "P2 Hybrid BoostingLimit 0Nm DriveOff 80km/h SoC: 0.7, UH 5%"),
			TestCase(80, 0.7, 5, 100, TestName = "P2 Hybrid BoostingLimit 100Nm DriveOff 80km/h SoC: 0.7, UH 5%"),
		]
		public void P2HybridDriveOffBoostingLimitLimit(double vmax, double initialSoC, double slope, double boostingLimit)
		{
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				  1200, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P2_acc_{vmax}-{initialSoC}_{slope}_boostingLimit-{boostingLimit}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, boostingLimit: boostingLimit.SI<NewtonMeter>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			graphWriter.Write(modFilename);
		}


		[
			TestCase(80, 0.7, 5, 100, TestName = "P2 Hybrid BoostingAndGbxLimit 150Nm DriveOff 80km/h SoC: 0.7, UH 5%"),
		]
		public void P2HybridDriveOffBoostingAndGbxLimit(double vmax, double initialSoC, double slope, double boostingLimit)
		{
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				  1200, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P2_acc_{vmax}-{initialSoC}_{slope}_boostingAndGbxLimit-{boostingLimit}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, boostingLimit: boostingLimit.SI<NewtonMeter>(), maxGearboxTorque: 2370.SI<NewtonMeter>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			graphWriter.Write(modFilename);
		}

		[
			TestCase(80, 0.7, 5, 100, TestName = "P2 Hybrid BoostingAndGbxLimit TopTorque 150Nm DriveOff 80km/h SoC: 0.7, UH 5%"),
		]
		public void P2HybridDriveOffBoostingAndGbxLimitTopTorque(double vmax, double initialSoC, double slope, double boostingLimit)
		{
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				  1200, {0}, {1},    0
				  1201, {0}, {2},    0
				  2000, {0}, {2},    0", vmax, slope, 7.2);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P2_acc_{vmax}-{initialSoC}_{slope}_boostingAndGbxLimitTopTorque-{boostingLimit}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true,
				boostingLimit: boostingLimit.SI<NewtonMeter>(), maxGearboxTorque: 2370.SI<NewtonMeter>(),
				topTorque: 2000.SI<NewtonMeter>());
			
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			graphWriter.Write(modFilename);
		}

		[
		TestCase(30, 0.7, 0, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.7, level"),
		TestCase(50, 0.7, 0, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.7, level"),
		TestCase(80, 0.7, 0, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.7, level"),

		TestCase(30, 0.25, 0, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.25, level"),
		TestCase(50, 0.25, 0, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.25, level"),
		TestCase(80, 0.25, 0, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.25, level"),

		TestCase(30, 0.5, 5, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
		TestCase(50, 0.5, 5, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
		TestCase(80, 0.5, 5, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

		TestCase(30, 0.5, -5, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
		TestCase(50, 0.5, -5, 0, TestName = "P2 Hybrid ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
		TestCase(80, 0.5, -5, 0, TestName = "P2 Hybrid ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

		TestCase(30, 0.25, 0, 1000, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 0, TestName = "P2 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 0kW"),
		]
		public void P2HybridConstantSpeed(double vmax, double initialSoC, double slope, double  pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P2_constant_{vmax}-{initialSoC}_{slope}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase("LongHaul", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Construction", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Urban", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Suburban", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Interurban", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
			TestCase("Coach", 2000, 0.5, 0, 320, TestName = "P2 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW maxPwr: 320kW"),
		]
		public void P2HybriDriveCycleOffLimitPwr(string declarationMission, double payload, double initialSoC, double pAuxEl, double maxPwrkW)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			var modFilename = string.Format("SimpleParallelHybrid-P2_cycle_{0}-{1}_{2}_{3}_maxPwr-{3}.vmod", declarationMission, initialSoC, payload, pAuxEl, maxPwrkW);
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase("LongHaul", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.5, 0, TestName = "P2 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),

			TestCase("LongHaul", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Construction", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Urban", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Suburban", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Interurban", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
			TestCase("Coach", 2000, 0.5, 2000, TestName = "P2 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 2kW"),
		]
		public void P2HybriDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			var modFilename = $"SimpleParallelHybrid-P2_cycle_{declarationMission}-{initialSoC}_{payload}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			graphWriter.Write(modFilename);
		}



		public const string Group2TestJob180kW = @"TestData/Hybrids/Hyb_P2_Group2/Class2_RigidTruck_ParHyb_ENG.vecto";


		public const string Group5TestJob = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5.vecto";

		public const string Group5TestJob_GbxTqLimit = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_GbxTqLimit.vecto";

		public const string Group5TestJob_BoostingLimit = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_BoostingLimit.vecto";

		public const string Group5TestJob_BoostingAndGbxLimit = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_BoostingAndGbxLimit.vecto";

		public const string Group5TestJob_BoostingAndGbxLimitTopTorque = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_BoostingAndGbxLimitTopTorque.vecto";

		public const string Group5TestJob_BoostingLimitTopTorque = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_BoostingLimitTopTorque.vecto";

		public const string Group5TestJob_BatterySystem = @"TestData/Hybrids/GenericVehicle_Group5_P2_BatSystem/P2 Group 5.vecto";

		public const string Group5TestJob_BatterySystem2 = @"TestData/Hybrids/GenericVehicle_Group5_P2_BatSystem/P2 Group 5_2.vecto";

		[
			TestCase(Group5TestJob, 0, TestName = "P2 Hybrid Group 5 DriveCycle LongHaul"),
			TestCase(Group5TestJob, 1, TestName = "P2 Hybrid Group 5 DriveCycle Coach"),  
			TestCase(Group5TestJob, 2, TestName = "P2 Hybrid Group 5 DriveCycle Construction"),  
			TestCase(Group5TestJob, 3, TestName = "P2 Hybrid Group 5 DriveCycle HeavyUrban"),
			TestCase(Group5TestJob, 4, TestName = "P2 Hybrid Group 5 DriveCycle Interurban"),  
			TestCase(Group5TestJob, 5, TestName = "P2 Hybrid Group 5 DriveCycle MunicipalUtility"),
			TestCase(Group5TestJob, 6, TestName = "P2 Hybrid Group 5 DriveCycle RegionalDelivery"),
			TestCase(Group5TestJob, 7, TestName = "P2 Hybrid Group 5 DriveCycle Suburban"),  
			TestCase(Group5TestJob, 8, TestName = "P2 Hybrid Group 5 DriveCycle Urban"), 
			TestCase(Group5TestJob, 9, TestName = "P2 Hybrid Group 5 DriveCycle UrbanDelivery"), 
		]
		[
			TestCase(Group5TestJob_GbxTqLimit, 0, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle LongHaul"),
			TestCase(Group5TestJob_GbxTqLimit, 1, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle Coach"),
			TestCase(Group5TestJob_GbxTqLimit, 2, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle Construction"),
			TestCase(Group5TestJob_GbxTqLimit, 3, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle HeavyUrban"),
			TestCase(Group5TestJob_GbxTqLimit, 4, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle Interurban"),
			TestCase(Group5TestJob_GbxTqLimit, 5, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle MunicipalUtility"),
			TestCase(Group5TestJob_GbxTqLimit, 6, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle RegionalDelivery"),
			TestCase(Group5TestJob_GbxTqLimit, 7, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle Suburban"),
			TestCase(Group5TestJob_GbxTqLimit, 8, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle Urban"),
			TestCase(Group5TestJob_GbxTqLimit, 9, TestName = "P2 Hybrid Group 5 GbxTqLimit DriveCycle UrbanDelivery"),
		]
		[
			TestCase(Group5TestJob_BoostingLimit, 0, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle LongHaul"),
			TestCase(Group5TestJob_BoostingLimit, 1, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle Coach"),
			TestCase(Group5TestJob_BoostingLimit, 2, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle Construction"),
			TestCase(Group5TestJob_BoostingLimit, 3, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle HeavyUrban"),
			TestCase(Group5TestJob_BoostingLimit, 4, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle Interurban"),
			TestCase(Group5TestJob_BoostingLimit, 5, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle MunicipalUtility"),
			TestCase(Group5TestJob_BoostingLimit, 6, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle RegionalDelivery"),
			TestCase(Group5TestJob_BoostingLimit, 7, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle Suburban"),
			TestCase(Group5TestJob_BoostingLimit, 8, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle Urban"),
			TestCase(Group5TestJob_BoostingLimit, 9, TestName = "P2 Hybrid Group 5 BoostingLimit DriveCycle UrbanDelivery"),
		]
		[
			TestCase(Group5TestJob_BoostingAndGbxLimit, 0, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle LongHaul"),
			TestCase(Group5TestJob_BoostingAndGbxLimit, 1, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle Coach"),
			TestCase(Group5TestJob_BoostingAndGbxLimit, 2, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle Construction"),
			TestCase(Group5TestJob_BoostingAndGbxLimit, 3, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle HeavyUrban"),
			TestCase(Group5TestJob_BoostingAndGbxLimit, 4, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle Interurban"),
			TestCase(Group5TestJob_BoostingAndGbxLimit, 5, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle MunicipalUtility"),
			TestCase(Group5TestJob_BoostingAndGbxLimit, 6, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle RegionalDelivery"),
			TestCase(Group5TestJob_BoostingAndGbxLimit, 7, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle Suburban"),
			TestCase(Group5TestJob_BoostingAndGbxLimit, 8, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle Urban"),
			TestCase(Group5TestJob_BoostingAndGbxLimit, 9, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit DriveCycle UrbanDelivery"),
		]
		[
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 0, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle LongHaul"),
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 1, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle Coach"),
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 2, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle Construction"),
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 3, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle HeavyUrban"),
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 4, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle Interurban"),
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 5, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle MunicipalUtility"),
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 6, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle RegionalDelivery"),
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 7, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle Suburban"),
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 8, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle Urban"),
			TestCase(Group5TestJob_BoostingAndGbxLimitTopTorque, 9, TestName = "P2 Hybrid Group 5 BoostingAndGbxLimit TopTorque DriveCycle UrbanDelivery"),
		]
		[
			TestCase(Group5TestJob_BoostingLimitTopTorque, 0, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle LongHaul"),
			TestCase(Group5TestJob_BoostingLimitTopTorque, 1, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle Coach"),
			TestCase(Group5TestJob_BoostingLimitTopTorque, 2, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle Construction"),
			TestCase(Group5TestJob_BoostingLimitTopTorque, 3, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle HeavyUrban"),
			TestCase(Group5TestJob_BoostingLimitTopTorque, 4, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle Interurban"),
			TestCase(Group5TestJob_BoostingLimitTopTorque, 5, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle MunicipalUtility"),
			TestCase(Group5TestJob_BoostingLimitTopTorque, 6, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle RegionalDelivery"),
			TestCase(Group5TestJob_BoostingLimitTopTorque, 7, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle Suburban"),
			TestCase(Group5TestJob_BoostingLimitTopTorque, 8, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle Urban"),
			TestCase(Group5TestJob_BoostingLimitTopTorque, 9, TestName = "P2 Hybrid Group 5 BoostingLimit TopTorque DriveCycle UrbanDelivery"),
		]
		public void P2HybridGroup5DriveCycle(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }



		[
			TestCase(Group5TestJob_BatterySystem, 0, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle LongHaul"),
			TestCase(Group5TestJob_BatterySystem, 1, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle Coach"),
			TestCase(Group5TestJob_BatterySystem, 2, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle Construction"),
			TestCase(Group5TestJob_BatterySystem, 3, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle HeavyUrban"),
			TestCase(Group5TestJob_BatterySystem, 4, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle Interurban"),
			TestCase(Group5TestJob_BatterySystem, 5, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle MunicipalUtility"),
			TestCase(Group5TestJob_BatterySystem, 6, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle RegionalDelivery"),
			TestCase(Group5TestJob_BatterySystem, 7, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle Suburban"),
			TestCase(Group5TestJob_BatterySystem, 8, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle Urban"),
			TestCase(Group5TestJob_BatterySystem, 9, TestName = "P2 Hybrid Group 5 BatterySystem DriveCycle UrbanDelivery"),

			TestCase(Group5TestJob_BatterySystem2, 0, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle LongHaul"),
			TestCase(Group5TestJob_BatterySystem2, 1, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle Coach"),
			TestCase(Group5TestJob_BatterySystem2, 2, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle Construction"),
			TestCase(Group5TestJob_BatterySystem2, 3, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle HeavyUrban"),
			TestCase(Group5TestJob_BatterySystem2, 4, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle Interurban"),
			TestCase(Group5TestJob_BatterySystem2, 5, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle MunicipalUtility"),
			TestCase(Group5TestJob_BatterySystem2, 6, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle RegionalDelivery"),
			TestCase(Group5TestJob_BatterySystem2, 7, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle Suburban"),
			TestCase(Group5TestJob_BatterySystem2, 8, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle Urban"),
			TestCase(Group5TestJob_BatterySystem2, 9, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle UrbanDelivery"),
			TestCase(Group5TestJob_BatterySystem2, 10, TestName = "P2 Hybrid Group 5 BatterySystem 2 DriveCycle Interurban Short"),

		]
		public void P2HybridGroup5DriveCycle_BatterySystem(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }


		public const string Group5TestJob325kW = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_325kW.vecto";

		public const string Group5TestJob325kW_WhrEl = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_325kW_WHR.vecto";

		[
		TestCase(Group5TestJob325kW, 0, TestName = "P2 Hybrid Group 5 325kW DriveCycle LongHaul"),
		TestCase(Group5TestJob325kW, 1, TestName = "P2 Hybrid Group 5 325kW DriveCycle Coach"),  
		TestCase(Group5TestJob325kW, 2, TestName = "P2 Hybrid Group 5 325kW DriveCycle Construction"),  
		TestCase(Group5TestJob325kW, 3, TestName = "P2 Hybrid Group 5 325kW DriveCycle HeavyUrban"), 
		TestCase(Group5TestJob325kW, 4, TestName = "P2 Hybrid Group 5 325kW DriveCycle Interurban"),  
		TestCase(Group5TestJob325kW, 5, TestName = "P2 Hybrid Group 5 325kW DriveCycle MunicipalUtility"), 
		TestCase(Group5TestJob325kW, 6, TestName = "P2 Hybrid Group 5 325kW DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob325kW, 7, TestName = "P2 Hybrid Group 5 325kW DriveCycle Suburban"), 
		TestCase(Group5TestJob325kW, 8, TestName = "P2 Hybrid Group 5 325kW DriveCycle Urban"), 
		TestCase(Group5TestJob325kW, 9, TestName = "P2 Hybrid Group 5 325kW DriveCycle UrbanDelivery"),

		TestCase(Group5TestJob325kW_WhrEl, 6, TestName = "P2 Hybrid Group 5 325kW WHR DriveCycle RegionalDelivery"),
		]
		public void P2HybridGroup5DriveCycle_325kW(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }

		public const string Group2TestJob = @"TestData/Hybrids/Hyb_P2_Group2/Class2_RigidTruck_ParHyb_ENG.vecto";

		[
		TestCase(Group2TestJob, 0, TestName = "P2 Hybrid Group 2 DriveCycle LongHaul"),
		TestCase(Group2TestJob, 1, TestName = "P2 Hybrid Group 2 DriveCycle RegionalDelivery"),
		TestCase(Group2TestJob, 2, TestName = "P2 Hybrid Group 2 DriveCycle UrbanDelivery"),  
		TestCase(Group2TestJob, 3, TestName = "P2 Hybrid Group 2 DriveCycle Coach"), 
		TestCase(Group2TestJob, 4, TestName = "P2 Hybrid Group 2 DriveCycle Construction"), 
		TestCase(Group2TestJob, 5, TestName = "P2 Hybrid Group 2 DriveCycle HeavyUrban"),
		TestCase(Group2TestJob, 6, TestName = "P2 Hybrid Group 2 DriveCycle Interurban"), 
		TestCase(Group2TestJob, 7, TestName = "P2 Hybrid Group 2 DriveCycle MunicipalUtility"),
		TestCase(Group2TestJob, 8, TestName = "P2 Hybrid Group 2 DriveCycle Suburban"),
		TestCase(Group2TestJob, 9, TestName = "P2 Hybrid Group 2 DriveCycle Urban"),]
		public void P2HybridGroup2DriveCycle(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }




		public const string Group5_80kWh_TestJob = @"TestData/Hybrids/Hyb_P2_Group5/Hyb_P2_Group5_80kWh.vecto";

		[
		TestCase(Group5_80kWh_TestJob, 0, TestName = "P2 Hybrid Group 5 80kWh DriveCycle LongHaul"),
		TestCase(Group5_80kWh_TestJob, 1, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Coach"), 
		TestCase(Group5_80kWh_TestJob, 2, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Construction"), 
		TestCase(Group5_80kWh_TestJob, 3, TestName = "P2 Hybrid Group 5 80kWh DriveCycle HeavyUrban"), 
		TestCase(Group5_80kWh_TestJob, 4, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Interurban"), 
		TestCase(Group5_80kWh_TestJob, 5, TestName = "P2 Hybrid Group 5 80kWh DriveCycle MunicipalUtility"),
		TestCase(Group5_80kWh_TestJob, 6, TestName = "P2 Hybrid Group 5 80kWh DriveCycle RegionalDelivery"),
		TestCase(Group5_80kWh_TestJob, 7, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Suburban"),
		TestCase(Group5_80kWh_TestJob, 8, TestName = "P2 Hybrid Group 5 80kWh DriveCycle Urban"),  
		TestCase(Group5_80kWh_TestJob, 9, TestName = "P2 Hybrid Group 5 80kWh DriveCycle UrbanDelivery"), ]
		public void P2HybridGroup5DriveCycle_80kWh(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }


		public const string Group5TestJob2 = @"TestData/Hybrids/Hyb_P2_Group5/Hyb_P2_Group5.vecto";

		[
		TestCase(Group5TestJob2, 0, TestName = "P2 Hybrid Group 5 B DriveCycle LongHaul"),
		TestCase(Group5TestJob2, 1, TestName = "P2 Hybrid Group 5 B DriveCycle Coach"), 
		TestCase(Group5TestJob2, 2, TestName = "P2 Hybrid Group 5 B DriveCycle Construction"), 
		TestCase(Group5TestJob2, 3, TestName = "P2 Hybrid Group 5 B DriveCycle HeavyUrban"),  
		TestCase(Group5TestJob2, 4, TestName = "P2 Hybrid Group 5 B DriveCycle Interurban"),  
		TestCase(Group5TestJob2, 5, TestName = "P2 Hybrid Group 5 B DriveCycle MunicipalUtility"),
		TestCase(Group5TestJob2, 6, TestName = "P2 Hybrid Group 5 B DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob2, 7, TestName = "P2 Hybrid Group 5 B DriveCycle Suburban"), 
		TestCase(Group5TestJob2, 8, TestName = "P2 Hybrid Group 5 B DriveCycle Urban"),  
		TestCase(Group5TestJob2, 9, TestName = "P2 Hybrid Group 5 B DriveCycle UrbanDelivery"),  
		]
		public void P2HybridGroup5DriveCycle2(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }


		public const string Group5TestJob_noESS = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5.vecto";

		[
		//// P2 without ESS is not really relevant
		TestCase(Group5TestJob_noESS, 0, TestName = "P2 Hybrid Group 5 NoESS DriveCycle LongHaul"),
		TestCase(Group5TestJob_noESS, 1, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Coach"),  
		TestCase(Group5TestJob_noESS, 2, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Construction"),   
		TestCase(Group5TestJob_noESS, 3, TestName = "P2 Hybrid Group 5 NoESS DriveCycle HeavyUrban"),
		TestCase(Group5TestJob_noESS, 4, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Interurban"),  
		TestCase(Group5TestJob_noESS, 5, TestName = "P2 Hybrid Group 5 NoESS DriveCycle MunicipalUtility"),
		TestCase(Group5TestJob_noESS, 6, TestName = "P2 Hybrid Group 5 NoESS DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob_noESS, 7, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Suburban"),   
		TestCase(Group5TestJob_noESS, 8, TestName = "P2 Hybrid Group 5 NoESS DriveCycle Urban"), 
		TestCase(Group5TestJob_noESS, 9, TestName = "P2 Hybrid Group 5 NoESS DriveCycle UrbanDelivery"),  
		]
		public void P2HybridGroup5DriveCycle_noESS(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }


		public const string Group5TestJob325kW_noESS = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_325kW.vecto";

		[
		//// P2 without ESS is not really relevant
		TestCase(Group5TestJob325kW_noESS, 0, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle LongHaul"),
		TestCase(Group5TestJob325kW_noESS, 1, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Coach"), 
		 TestCase(Group5TestJob325kW_noESS, 2, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Construction"),  
		 TestCase(Group5TestJob325kW_noESS, 3, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle HeavyUrban"), 
		 TestCase(Group5TestJob325kW_noESS, 4, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Interurban"),  
		TestCase(Group5TestJob325kW_noESS, 5, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle MunicipalUtility"), 
		TestCase(Group5TestJob325kW_noESS, 6, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob325kW_noESS, 7, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Suburban"), 
		TestCase(Group5TestJob325kW_noESS, 8, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle Urban"), 
		TestCase(Group5TestJob325kW_noESS, 9, TestName = "P2 Hybrid Group 5 325kW NoESS DriveCycle UrbanDelivery"),
		] 
		public void P2HybridGroup5DriveCycle_325kW_noESS(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }


		//public const string CityBus6x2 = @"TestData/Hybrids/Input CityBus 6x2_HEV_P2/CityBus_6x2_HEV_P2.vecto";

		//[
		//TestCase(CityBus6x2, 0, TestName = "P2 Hybrid CityBus DriveCycle Coach"),  // error
  //      TestCase(CityBus6x2, 1, TestName = "P2 Hybrid CityBus DriveCycle HeavyUrban"),  // error
  //      TestCase(CityBus6x2, 2, TestName = "P2 Hybrid CityBus DriveCycle Interurban"),  // error
  //      TestCase(CityBus6x2, 3, TestName = "P2 Hybrid CityBus DriveCycle LongHaul"),  // error
  //      TestCase(CityBus6x2, 4, TestName = "P2 Hybrid CityBus DriveCycle RegionalDelivery"),  // error
  //      TestCase(CityBus6x2, 5, TestName = "P2 Hybrid CityBus DriveCycle Suburban"),  // error
  //      TestCase(CityBus6x2, 6, TestName = "P2 Hybrid CityBus DriveCycle Urban"),  // error
  //      TestCase(CityBus6x2, 7, TestName = "P2 Hybrid CityBus DriveCycle UrbanDelivery"),  // error
  //      ]
		//public void P2HybridGroup5DriveCycle_CityBus6x2(string jobFile, int cycleIdx)
		//{ RunHybridJob(jobFile, cycleIdx); }


		public const string Group2TestJobSuperCapOvl = @"TestData/Hybrids/Hyb_P2_Group2SuperCapOvl/Class2_RigidTruck_ParHyb_SuperCap_Ovl_ENG.vecto";

		[
		TestCase(Group2TestJobSuperCapOvl, 0, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle LongHaul"),
		TestCase(Group2TestJobSuperCapOvl, 1, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle RegionalDelivery"), 
		TestCase(Group2TestJobSuperCapOvl, 2, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle UrbanDelivery"), 
		TestCase(Group2TestJobSuperCapOvl, 3, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Coach"), 
		TestCase(Group2TestJobSuperCapOvl, 4, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Construction"),  
		TestCase(Group2TestJobSuperCapOvl, 5, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle HeavyUrban"),
		TestCase(Group2TestJobSuperCapOvl, 6, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Interurban"),
		TestCase(Group2TestJobSuperCapOvl, 7, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle MunicipalUtility"), 
		TestCase(Group2TestJobSuperCapOvl, 8, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Suburban"),
		TestCase(Group2TestJobSuperCapOvl, 9, TestName = "P2 Hybrid Group 2 SuperCap Ovl, DriveCycle Urban"),
		]
		public void P2HybridGroup5DriveCycle_SuperCapOvl(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }


		public const string Group5_EMTorqueLimit_TestJob = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_LimitEMTorqueDrive.vecto";

		[
		TestCase(Group5_EMTorqueLimit_TestJob, 0, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle LongHaul"),
		TestCase(Group5_EMTorqueLimit_TestJob, 1, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Coach "),
		TestCase(Group5_EMTorqueLimit_TestJob, 2, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Construction"),
		TestCase(Group5_EMTorqueLimit_TestJob, 3, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle HeavyUrban"),
		TestCase(Group5_EMTorqueLimit_TestJob, 4, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Interurban"),
		TestCase(Group5_EMTorqueLimit_TestJob, 5, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle MunicipalUtility"),
		TestCase(Group5_EMTorqueLimit_TestJob, 6, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle RegionalDelivery"),
		TestCase(Group5_EMTorqueLimit_TestJob, 7, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Suburban"),
		TestCase(Group5_EMTorqueLimit_TestJob, 8, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle Urban"),
		TestCase(Group5_EMTorqueLimit_TestJob, 9, TestName = "P2 Hybrid Group 5 EM TorqueLimit DriveCycle UrbanDelivery"),
		]
		public void P2HybridGroup5DriveCycle_EmTorqueLmit(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }


		public const string Group5_LimitPropTq_TestJob = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_LimitVehiclePropTq.vecto";

		[
		TestCase(Group5_LimitPropTq_TestJob, 0, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle LongHaul"),
		TestCase(Group5_LimitPropTq_TestJob, 1, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Coach "),
		TestCase(Group5_LimitPropTq_TestJob, 2, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Construction"),
		TestCase(Group5_LimitPropTq_TestJob, 3, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle HeavyUrban"),
		TestCase(Group5_LimitPropTq_TestJob, 4, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Interurban"),
		TestCase(Group5_LimitPropTq_TestJob, 5, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle MunicipalUtility"),
		TestCase(Group5_LimitPropTq_TestJob, 6, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle RegionalDelivery"),
		TestCase(Group5_LimitPropTq_TestJob, 7, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Suburban"),
		TestCase(Group5_LimitPropTq_TestJob, 8, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle Urban"),
		TestCase(Group5_LimitPropTq_TestJob, 9, TestName = "P2 Hybrid Group 5 Vehicle Prop TorqueLimit DriveCycle UrbanDelivery"),
			]
		public void P2HybridGroup5DriveCycle_LimitPropTq(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }

		public const string Group5TestJob_EMLossMap_1 = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_EMLossMap_1.vecto"; // non-linear EM Loss-Map

		[
		TestCase(Group5TestJob_EMLossMap_1, 0, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle LongHaul"),
		TestCase(Group5TestJob_EMLossMap_1, 1, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle Coach"),  
		TestCase(Group5TestJob_EMLossMap_1, 2, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle Construction"),  
		TestCase(Group5TestJob_EMLossMap_1, 3, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle HeavyUrban"),
		TestCase(Group5TestJob_EMLossMap_1, 4, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle Interurban"),  
		TestCase(Group5TestJob_EMLossMap_1, 5, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle MunicipalUtility"),
		TestCase(Group5TestJob_EMLossMap_1, 6, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob_EMLossMap_1, 7, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle Suburban"),  
		TestCase(Group5TestJob_EMLossMap_1, 8, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle Urban"),
		TestCase(Group5TestJob_EMLossMap_1, 9, TestName = "P2 Hybrid Group 5 EM-LossMap 1 DriveCycle UrbanDelivery"), 
			]
		public void P2HybridGroup5DriveCycle_EMLossMap_1(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }


		public const string Group5TestJob_EMLossMap_2 = @"TestData/Hybrids/GenericVehicle_Group5_P2/P2 Group 5_EMLossMap_2.vecto"; // linear EM Loss-Map

		[TestCase(Group5TestJob_EMLossMap_2, 0, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle LongHaul"),
		TestCase(Group5TestJob_EMLossMap_2, 1, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle Coach"), 
		TestCase(Group5TestJob_EMLossMap_2, 2, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle Construction"),  
		TestCase(Group5TestJob_EMLossMap_2, 3, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle HeavyUrban"),
		TestCase(Group5TestJob_EMLossMap_2, 4, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle Interurban"), 
		TestCase(Group5TestJob_EMLossMap_2, 5, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle MunicipalUtility"),
		TestCase(Group5TestJob_EMLossMap_2, 6, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle RegionalDelivery"),
		TestCase(Group5TestJob_EMLossMap_2, 7, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle Suburban"),  
		TestCase(Group5TestJob_EMLossMap_2, 8, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle Urban"),
		TestCase(Group5TestJob_EMLossMap_2, 9, TestName = "P2 Hybrid Group 5 EM-LossMap 2 DriveCycle UrbanDelivery"), 
			]
		public void P2HybridGroup5DriveCycle_EMLossMap_2(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx);}


		public const string Group2_5TestJob = @"TestData/Hybrids/GenericVehicle_Group5_P2.5/P2.5 Group 5.vecto";
		public const string Group2_5TestJob_2 = @"TestData/Hybrids/GenericVehicle_Group5_P2.5/P2.5 Group 5 2.vecto";

		[
			TestCase(Group2_5TestJob, 0, TestName = "P2.5 Hybrid Group 2 DriveCycle LongHaul"),
			TestCase(Group2_5TestJob, 1, TestName = "P2.5 Hybrid Group 2 DriveCycle Coach"),
			TestCase(Group2_5TestJob, 2, TestName = "P2.5 Hybrid Group 2 DriveCycle Construction"),
			TestCase(Group2_5TestJob, 3, TestName = "P2.5 Hybrid Group 2 DriveCycle HeavyUrban"),
			TestCase(Group2_5TestJob, 4, TestName = "P2.5 Hybrid Group 2 DriveCycle Interurban"),
			TestCase(Group2_5TestJob, 5, TestName = "P2.5 Hybrid Group 2 DriveCycle MunicipalUtility"),
			TestCase(Group2_5TestJob, 6, TestName = "P2.5 Hybrid Group 2 DriveCycle RegionalDelivery"),
			TestCase(Group2_5TestJob, 7, TestName = "P2.5 Hybrid Group 2 DriveCycle Suburban"),
			TestCase(Group2_5TestJob, 8, TestName = "P2.5 Hybrid Group 2 DriveCycle Urban"),
			TestCase(Group2_5TestJob, 9, TestName = "P2.5 Hybrid Group 2 DriveCycle UrbanDelivery"),

			TestCase(Group2_5TestJob_2, 0, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle LongHaul"),
			TestCase(Group2_5TestJob_2, 1, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle Coach"),
			TestCase(Group2_5TestJob_2, 2, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle Construction"),
			TestCase(Group2_5TestJob_2, 3, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle HeavyUrban"),
			TestCase(Group2_5TestJob_2, 4, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle Interurban"),
			TestCase(Group2_5TestJob_2, 5, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle MunicipalUtility"),
			TestCase(Group2_5TestJob_2, 6, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle RegionalDelivery"),
			TestCase(Group2_5TestJob_2, 7, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle Suburban"),
			TestCase(Group2_5TestJob_2, 8, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle Urban"),
			TestCase(Group2_5TestJob_2, 9, TestName = "P2.5 Hybrid Group 2 Even/Odd DriveCycle UrbanDelivery"),

		]
		public void P2HybridGroup2_5DriveCycle(string jobFile, int cycleIdx)
		{ RunHybridJob(jobFile, cycleIdx); }


		public void RunHybridJob(string jobFile, int cycleIdx, int? startDistance = null, ExecutionMode mode = ExecutionMode.Engineering)
		{
			var inputProvider = Path.GetExtension(jobFile) == ".xml"
				? xmlInputReader.CreateDeclaration(jobFile)
				: JSONInputDataFactory.ReadJsonJob(jobFile);
			
			var writer = new FileOutputWriter(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(mode, inputProvider, writer);
			factory.Validate = false;
			factory.WriteModalResults = true;

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			var run = factory.SimulationRuns().ToArray()[cycleIdx];

			if (startDistance != null) {
				//(run.GetContainer().BatteryInfo as Battery).PreviousState.StateOfCharge = 0.317781;
				(run.GetContainer().RunData.Cycle as DrivingCycleProxy).Entries = run.GetContainer().RunData.Cycle
					.Entries.Where(x => x.Distance >= startDistance.Value ).ToList();
				//run.GetContainer().MileageCounter.Distance = startDistance;
				//run.GetContainer().DrivingCycleInfo.CycleStartDistance = startDistance;
			}

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

		[
			TestCase(80, 0, TestName = "Conventional DriveOff 80km/h  level"),
		]
		public void ConventionalVehicleDriveOff(double vmax, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			//const bool largeMotor = true;

			//const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var run = CreateConventionalEngineeringRun(
				cycle, $"ConventionalVehicle_acc_{vmax}_{slope}.vmod");

			//var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			//Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
		}


		[TestCase(50, 0.79, 0, TestName = "P2 Hybrid Brake Standstill 50km/h SoC: 0.79, level"),
		TestCase(50, 0.25, 0, TestName = "P2 Hybrid Brake Standstill 50km/h SoC: 0.25, level"),
		TestCase(50, 0.65, 0, TestName = "P2 Hybrid Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void P2HybridBrakeStandstill(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P2_stop_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P2 });
			graphWriter.Write(modFilename);
		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - 


		[
			TestCase(30, 0.7, 0, 0, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "P3 Hybrid ConstantSpeed 50km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, 0, TestName = "P3 Hybrid ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "P3 Hybrid ConstantSpeed 50km/h SoC: 0.25, level"),
			TestCase(80, 0.25, 0, 0, TestName = "P3 Hybrid ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "P3 Hybrid ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			TestCase(80, 0.5, 5, 0, TestName = "P3 Hybrid ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "P3 Hybrid ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			TestCase(80, 0.5, -5, 0, TestName = "P3 Hybrid ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "P3 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		]
		public void P3HybridConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P3_constant_{vmax}-{initialSoC}_{slope}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P3 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase(30, 0.7, 0, TestName = "P3 Hybrid DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "P3 Hybrid DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.22, 0, TestName = "P3 Hybrid DriveOff 30km/h SoC: 0.22, level")
		]
		public void P3HybridDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P3_acc_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P3 });
			graphWriter.Write(modFilename);
		}

		[TestCase(50, 0.79, 0, TestName = "P3 Hybrid Brake Standstill 50km/h SoC: 0.79, level"),
		TestCase(50, 0.25, 0, TestName = "P3 Hybrid Brake Standstill 50km/h SoC: 0.25, level"),
		TestCase(50, 0.65, 0, TestName = "P3 Hybrid Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void P3HybridBrakeStandstill(double vmax, double initialSoC, double slope)
		{
			//var dst =
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P3_stop_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP3;
			var job = CreateEngineeringRun(cycle, modFilename, initialSoC, pos, 1.0, true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P3 });
			graphWriter.Write(modFilename);
		}


		[
			TestCase("LongHaul", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.5, 0, TestName = "P3 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
		]
		public void P3HybriDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			var modFilename = $"SimpleParallelHybrid-P3_cycle_{declarationMission}-{initialSoC}_{payload}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P3 });
			graphWriter.Write(modFilename);
		}

		public const string Group5_P3_AMT = @"TestData/Hybrids/GenericVehicle_Group5_P3/P3 Group 5.vecto";
			
		[
			TestCase(Group5_P3_AMT, 0, TestName = "P3 Hybrid DriveCycle LongHaul"),
			TestCase(Group5_P3_AMT, 1, TestName = "P3 Hybrid DriveCycle Coach"),
			TestCase(Group5_P3_AMT, 2, TestName = "P3 Hybrid DriveCycle Construction"),
			TestCase(Group5_P3_AMT, 3, TestName = "P3 Hybrid DriveCycle HeavyUrban"),
			TestCase(Group5_P3_AMT, 4, TestName = "P3 Hybrid DriveCycle Interurban"),
			TestCase(Group5_P3_AMT, 5, TestName = "P3 Hybrid DriveCycle MunicipalUtility"),
			TestCase(Group5_P3_AMT, 6, TestName = "P3 Hybrid DriveCycle RegionalDelivery"),
			TestCase(Group5_P3_AMT, 7, TestName = "P3 Hybrid DriveCycle Suburban"),
			TestCase(Group5_P3_AMT, 8, TestName = "P3 Hybrid DriveCycle Urban"),
			TestCase(Group5_P3_AMT, 9, TestName = "P3 Hybrid DriveCycle UrbanDelivery"),

		]
		public void P3HybridGroup5_DriveCycle(string jobFile, int cycleIdx)
		{
			RunHybridJob(jobFile, cycleIdx);
		}

		public const string Group5_P3_AMT_VehiclePropLimit = @"TestData/Hybrids/GenericVehicle_Group5_P3/P3 Group 5_LimitVehiclePropTq.vecto";

		[
			TestCase(Group5_P3_AMT_VehiclePropLimit, 0, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle LongHaul"),
			TestCase(Group5_P3_AMT_VehiclePropLimit, 1, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle Coach"),
			TestCase(Group5_P3_AMT_VehiclePropLimit, 2, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle Construction"),
			TestCase(Group5_P3_AMT_VehiclePropLimit, 3, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle HeavyUrban"),
			TestCase(Group5_P3_AMT_VehiclePropLimit, 4, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle Interurban"),
			TestCase(Group5_P3_AMT_VehiclePropLimit, 5, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle MunicipalUtility"),
			TestCase(Group5_P3_AMT_VehiclePropLimit, 6, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle RegionalDelivery"),
			TestCase(Group5_P3_AMT_VehiclePropLimit, 7, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle Suburban"),
			TestCase(Group5_P3_AMT_VehiclePropLimit, 8, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle Urban"),
			TestCase(Group5_P3_AMT_VehiclePropLimit, 9, TestName = "P3 Hybrid Limit Vehicle Prop.Tq DriveCycle UrbanDelivery"),

		]
		public void P3HybridGroup5_VehilcePropLimit_DriveCycle(string jobFile, int cycleIdx)
		{
			RunHybridJob(jobFile, cycleIdx);
		}
		
		public const string Group5_P3_APT = @"TestData/Hybrids/GenericVehicle_Group5_P3_APT/P3 APT Group 5.vecto";
		public const string Group5_P4_APT = @"TestData/Hybrids/GenericVehicle_Group5_P4_APT/P4 APT Group 5.vecto";

		public const string Group5_P4_APT_VehiclePropLimit = @"TestData/Hybrids/GenericVehicle_Group5_P4_APT/P4 APT Group 5_LimitVehiclePropTq.vecto";


        [
            TestCase(Group5_P3_APT, 0, TestName = "P3 APT Hybrid Group 5 DriveCycle LongHaul"),
			TestCase(Group5_P3_APT, 1, TestName = "P3 APT Hybrid Group 5 DriveCycle Coach"),
			TestCase(Group5_P3_APT, 2, TestName = "P3 APT Hybrid Group 5 DriveCycle Construction"),
			TestCase(Group5_P3_APT, 3, TestName = "P3 APT Hybrid Group 5 DriveCycle HeavyUrban"),
			TestCase(Group5_P3_APT, 4, TestName = "P3 APT Hybrid Group 5 DriveCycle Interurban"),
			TestCase(Group5_P3_APT, 5, TestName = "P3 APT Hybrid Group 5 DriveCycle MunicipalUtility"),
			TestCase(Group5_P3_APT, 6, TestName = "P3 APT Hybrid Group 5 DriveCycle RegionalDelivery"),
			TestCase(Group5_P3_APT, 7, TestName = "P3 APT Hybrid Group 5 DriveCycle Suburban"),
			TestCase(Group5_P3_APT, 8, TestName = "P3 APT Hybrid Group 5 DriveCycle Urban"),
			TestCase(Group5_P3_APT, 9, TestName = "P3 APT Hybrid Group 5 DriveCycle UrbanDelivery"),

			TestCase(Group5_P4_APT, 0, TestName = "P4 APT Hybrid Group 5 DriveCycle LongHaul"),
			TestCase(Group5_P4_APT, 1, TestName = "P4 APT Hybrid Group 5 DriveCycle Coach"),
			TestCase(Group5_P4_APT, 2, TestName = "P4 APT Hybrid Group 5 DriveCycle Construction"),
			TestCase(Group5_P4_APT, 3, TestName = "P4 APT Hybrid Group 5 DriveCycle HeavyUrban"),
			TestCase(Group5_P4_APT, 4, TestName = "P4 APT Hybrid Group 5 DriveCycle Interurban"),
			TestCase(Group5_P4_APT, 5, TestName = "P4 APT Hybrid Group 5 DriveCycle MunicipalUtility"),
			TestCase(Group5_P4_APT, 6, TestName = "P4 APT Hybrid Group 5 DriveCycle RegionalDelivery"),
			TestCase(Group5_P4_APT, 7, TestName = "P4 APT Hybrid Group 5 DriveCycle Suburban"),
			TestCase(Group5_P4_APT, 8, TestName = "P4 APT Hybrid Group 5 DriveCycle Urban"),
			TestCase(Group5_P4_APT, 9, TestName = "P4 APT Hybrid Group 5 DriveCycle UrbanDelivery"),

			TestCase(Group5_P4_APT_VehiclePropLimit, 6, TestName = "P4 APT Hybrid Group 5 BoostingLimit DriveCycle RegionalDelivery"),

        ]
        public void P4APTHybridGroup5DriveCycle(string jobFile, int cycleIdx)
		{
			RunHybridJob(jobFile, cycleIdx);
		}


		

		// - - - - - - - - - - - - - - - - - - - - - - - - - 

		[
			TestCase(30, 0.7, 0, 0, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "P4 Hybrid ConstantSpeed 50km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, 0, TestName = "P4 Hybrid ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "P4 Hybrid ConstantSpeed 50km/h SoC: 0.25, level"),
			TestCase(80, 0.25, 0, 0, TestName = "P4 Hybrid ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "P4 Hybrid ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			TestCase(80, 0.5, 5, 0, TestName = "P4 Hybrid ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "P4 Hybrid ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			TestCase(80, 0.5, -5, 0, TestName = "P4 Hybrid ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 3000, TestName = "P4 Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 3kW"),
		]
		public void P4HybridConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P4_constant_{vmax}-{initialSoC}_{slope}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P4 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase(30, 0.7, 0, TestName = "P4 Hybrid DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "P4 Hybrid DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.22, 0, TestName = "P4 Hybrid DriveOff 30km/h SoC: 0.22, level")
		]
		public void P4HybridDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P4_acc_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P4 });
			graphWriter.Write(modFilename);
		}

		[TestCase(50, 0.79, 0, TestName = "P4 Hybrid Brake Standstill 50km/h SoC: 0.79, level"),
		TestCase(50, 0.25, 0, TestName = "P4 Hybrid Brake Standstill 50km/h SoC: 0.25, level"),
		TestCase(50, 0.65, 0, TestName = "P4 Hybrid Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void P4HybridBrakeStandstill(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleParallelHybrid-P4_stop_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true);
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P4 });
			graphWriter.Write(modFilename);
		}

		[
			TestCase("LongHaul", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.5, 0, TestName = "P4 Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
		]
		public void P4HybriDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			var modFilename = $"SimpleParallelHybrid-P4_cycle_{declarationMission}-{initialSoC}_{payload}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.HybridP4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 1.0, largeMotor: true, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (HybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_P4 });
			graphWriter.Write(modFilename);
		}

		public const string Group5_P4_AMT = @"TestData/Hybrids/GenericVehicle_Group5_P4/P4 Group 5.vecto";

		[
			TestCase(Group5_P4_AMT, 0, TestName = "P4 Hybrid DriveCycle LongHaul"),
			TestCase(Group5_P4_AMT, 1, TestName = "P4 Hybrid DriveCycle Coach"),
			TestCase(Group5_P4_AMT, 2, TestName = "P4 Hybrid DriveCycle Construction"),
			TestCase(Group5_P4_AMT, 3, TestName = "P4 Hybrid DriveCycle HeavyUrban"),
			TestCase(Group5_P4_AMT, 4, TestName = "P4 Hybrid DriveCycle Interurban"),
			TestCase(Group5_P4_AMT, 5, TestName = "P4 Hybrid DriveCycle MunicipalUtility"),
			TestCase(Group5_P4_AMT, 6, TestName = "P4 Hybrid DriveCycle RegionalDelivery"),
			TestCase(Group5_P4_AMT, 7, TestName = "P4 Hybrid DriveCycle Suburban"),
			TestCase(Group5_P4_AMT, 8, TestName = "P4 Hybrid DriveCycle Urban"),
			TestCase(Group5_P4_AMT, 9, TestName = "P4 Hybrid DriveCycle UrbanDelivery"),

		]
		public void P4HybridGroup5_DriveCycle(string jobFile, int cycleIdx)
		{
			RunHybridJob(jobFile, cycleIdx);
		}

		public const string Group5_P4_AMT_VehiclePropLimit = @"TestData/Hybrids/GenericVehicle_Group5_P4/P4 Group 5_LimitVehiclePropTq.vecto";

		[
			TestCase(Group5_P4_AMT_VehiclePropLimit, 0, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle LongHaul"),
			TestCase(Group5_P4_AMT_VehiclePropLimit, 1, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle Coach"),
			TestCase(Group5_P4_AMT_VehiclePropLimit, 2, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle Construction"),
			TestCase(Group5_P4_AMT_VehiclePropLimit, 3, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle HeavyUrban"),
			TestCase(Group5_P4_AMT_VehiclePropLimit, 4, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle Interurban"),
			TestCase(Group5_P4_AMT_VehiclePropLimit, 5, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle MunicipalUtility"),
			TestCase(Group5_P4_AMT_VehiclePropLimit, 6, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle RegionalDelivery"),
			TestCase(Group5_P4_AMT_VehiclePropLimit, 7, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle Suburban"),
			TestCase(Group5_P4_AMT_VehiclePropLimit, 8, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle Urban"),
			TestCase(Group5_P4_AMT_VehiclePropLimit, 9, TestName = "P4 Hybrid Limit Vehicle Prop.Tq DriveCycle UrbanDelivery"),

		]
		public void P4HybridGroup5_VehilcePropLimit_DriveCycle(string jobFile, int cycleIdx)
		{
			RunHybridJob(jobFile, cycleIdx);
		}


        [TestCase(@"E:/QUAM/tmp/HybridStrategy/P1_Group31aU_ll/P1_CityBusU_ll.vecto"),
		 TestCase(@"E:/QUAM/tmp/HybridStrategy/P1_Group31aU_rl/P1_CityBusU_rl.vecto"),
		]
		public void HybridTestGerard(string jobfile)
		{
			RunHybridJob(jobfile, 0);
		}

		// =================================================

		public static JobContainer CreateEngineeringRun(DrivingCycleData cycleData, string modFileName,
			double initialSoc, PowertrainPosition pos, double ratio, bool largeMotor = false, double pAuxEl = 0,
			Kilogram payload = null, GearboxType gearboxType = GearboxType.NoGearbox,
			NewtonMeter maxGearboxTorque = null, NewtonMeter boostingLimit = null, NewtonMeter topTorque = null)
		{
			var fileWriter = new FileOutputWriter(Path.GetFileNameWithoutExtension(modFileName));
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			var container = CreateParallelHybridPowerTrain(cycleData, modFileName, initialSoc, largeMotor, sumData, 
				pAuxEl, pos, ratio, payload, gearboxType, maxGearboxTorque, boostingLimit, topTorque);
			var run = new DistanceRun(container);
			jobContainer.AddRun(run);
			return jobContainer;
		}

		public static VectoRun CreateConventionalEngineeringRun(DrivingCycleData cycleData, string modFileName, 
			SummaryDataContainer sumData = null, double pAuxEl = 0)
		{
			var container = CreateConventionalPowerTrain(
				cycleData, Path.GetFileNameWithoutExtension(modFileName), sumData, pAuxEl);
			return new DistanceRun(container);
		}

		public static IVehicleContainer CreateParallelHybridPowerTrain(DrivingCycleData cycleData, string modFileName,
			double initialBatCharge, bool largeMotor, SummaryDataContainer sumData, double pAuxEl,
			PowertrainPosition pos, double ratio, Kilogram payload = null,
			GearboxType gearboxType = GearboxType.NoGearbox, NewtonMeter maxGearboxTorque = null,
			NewtonMeter boostingLimit = null, NewtonMeter topTorque = null)
		{ 
			var gearboxData = CreateGearboxData(gearboxType);
			var axleGearData = CreateAxleGearData(gearboxType);

			var vehicleData = CreateVehicleData(payload ??3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var emFile = pos == PowertrainPosition.HybridP1 ? P1HybridMotor : (largeMotor ? MotorFile240kW : MotorFile);
			var electricMotorData = MockSimulationDataFactory.CreateElectricMotorData(emFile, 1, pos, ratio, 1);

			var batFile = pos == PowertrainPosition.HybridP1 ? P1BatteryFile : BatFile;
			var batteryData = MockSimulationDataFactory.CreateBatteryData(batFile, initialBatCharge);
			//batteryData.TargetSoC = 0.5;

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(
				 Truck40tPowerTrain.EngineFile, gearboxData.Gears.Count, topTorque);

			foreach (var entry in gearboxData.Gears) {
				entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
					(int)entry.Key, engineData.FullLoadCurves[entry.Key], new TransmissionInputData().Repeat(gearboxData.Gears.Count + 1)
						.Cast<ITransmissionInputData>().ToList(), engineData, axleGearData.AxleGear.Ratio,
					vehicleData.DynamicTyreRadius);
			}

			var runData = new VectoRunData() {
				//PowertrainConfiguration = PowertrainConfiguration.ParallelHybrid,
				JobRunId = 0,
				JobType = VectoSimulationJobType.ParallelHybridVehicle,
				SimulationType = SimulationType.DistanceCycle,
				DriverData = driverData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				JobName = Path.GetFileNameWithoutExtension(modFileName),
				Cycle = cycleData,
				Retarder = new RetarderData() { Type = RetarderType.None },
				Aux = new List<VectoRunData.AuxData>(),
				ElectricMachinesData = electricMotorData,
				EngineData = engineData,
				BatteryData = batteryData,
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio, engineData.IdleSpeed),
				HybridStrategyParameters = CreateHybridStrategyData(),
				ElectricAuxDemand = pAuxEl.SI<Watt>()
			};
			var mockVehicleInput = new MockEngineeringVehicleInputData() { };
			mockVehicleInput.ElectricMachines = new MockElectricMachinesInputData() {
				Entries = new List<ElectricMachineEntry<IElectricMotorEngineeringInputData>>() {
					new ElectricMachineEntry<IElectricMotorEngineeringInputData>() {
						Position = pos
					}
				}
			};
			if (boostingLimit != null) {
				mockVehicleInput.BoostingLimitations = VectoCSVFile.ReadStream(
					$"n, T_drive\n0, {boostingLimit.Value()}\n1000, {boostingLimit.Value()}".ToStream());
				runData.HybridStrategyParameters.MaxPropulsionTorque =
					EngineeringDataAdapter.CreateMaxPropulsionTorque(mockVehicleInput, engineData,
						gearboxData);
			}
			if (maxGearboxTorque != null) {
				foreach (var gear in gearboxData.Gears) {
					gear.Value.MaxTorque = maxGearboxTorque;
				}
				runData.HybridStrategyParameters.MaxPropulsionTorque =
					EngineeringDataAdapter.CreateMaxPropulsionTorque(mockVehicleInput, engineData,
						gearboxData);
			}
			var fileWriter = new FileOutputWriter(modFileName);
			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(runData, fileWriter, null, modDataFilter)
			{
				WriteModalResults = true,
			};
			var container = VehicleContainer.CreateVehicleContainer(runData, modData, sumData); 
			
			var strategy = gearboxType.AutomaticTransmission()
				? (IHybridControlStrategy) new HybridStrategyAT(runData, container)
				: new HybridStrategy(runData, container);
			var es = new ElectricSystem(container, batteryData);
			var battery = new BatterySystem(container, batteryData);
			battery.Initialize(initialBatCharge);

			var clutch = gearboxType.AutomaticTransmission() ? null :  new SwitchableClutch(container, runData.EngineData);
			var ctl = new HybridController(container, strategy, es);

			es.Connect(battery);

			var engine = new StopStartCombustionEngine(container, runData.EngineData);
			var gearbox = gearboxType.AutomaticTransmission()
				? (IHybridControlledGearbox)new APTGearbox(container, ctl.ShiftStrategy)
				: new AMTGearbox(container, ctl.ShiftStrategy);
			//var hybridStrategy = new DelegateParallelHybridStrategy();
			ctl.Gearbox = gearbox;

			var idleController = engine.IdleController;
			ctl.Engine = engine;

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var aux = new HighVoltageElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", pAuxEl.SI<Watt>());
			es.Connect(aux);

			cycle
				.AddComponent(new Driver(container, runData.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, runData.VehicleData, runData.AirdragData))
				.AddComponent(new Wheels(container, runData.VehicleData.DynamicTyreRadius, runData.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, runData.ElectricMachinesData, container, es, ctl))
				.AddComponent(new AxleGear(container, runData.AxleGearData))
				.AddComponent(runData.Retarder.Type == RetarderType.AxlegearInputRetarder ? new Retarder(container, 
					runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, runData.ElectricMachinesData, container, es, ctl))
				.AddComponent(runData.AngledriveData != null ? new Angledrive(container, runData.AngledriveData) : null)
				.AddComponent(runData.Retarder.Type == RetarderType.TransmissionOutputRetarder ? new Retarder(container, 
					runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
				.AddComponent((IGearbox)gearbox)
				.AddComponent(runData.Retarder.Type == RetarderType.TransmissionInputRetarder ? new Retarder(container, 
					runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, runData.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, runData.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController);
			//AddAuxiliaries(engine, container, runData);

			if (runData.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				if (gearbox is IAPTGearbox atGbx) {
					atGbx.IdleController = idleController;
					new ATClutchInfo(container);
				}
			}

			return container;
		}

		private static HybridStrategyParameters CreateHybridStrategyData()
		{
			return new HybridStrategyParameters() {
				EquivalenceFactorDischarge = 2.5,
				EquivalenceFactorCharge = 2.5,
				MinSoC = 0.22,
				MaxSoC = 0.8,
				TargetSoC = 0.5,
				AuxReserveTime = 5.SI<Second>(),
				AuxReserveChargeTime = 2.SI<Second>(),
				MinICEOnTime = 3.SI<Second>(), 
				ICEStartPenaltyFactor = 0,
				//MaxDrivetrainPower = maxDriveTrainPower ?? 1e12.SI<Watt>(),
				CostFactorSOCExponent = 5,
			};
		}


		public static IVehicleContainer CreateConventionalPowerTrain(DrivingCycleData cycleData, string modFileName,
			SummaryDataContainer sumData, double pAuxEl)
		{
			//var strategySettings = GetHybridStrategyParameters(largeMotor);

			//strategySettings.StrategyName = "SimpleParallelHybridStrategy";

			var gearboxData = CreateGearboxData();
			var axleGearData = CreateAxleGearData(GearboxType.AMT);

			var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(
				 Truck40tPowerTrain.EngineFile, gearboxData.Gears.Count);

			foreach (var entry in gearboxData.Gears) {
				entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
					(int)entry.Key, engineData.FullLoadCurves[entry.Key], new TransmissionInputData().Repeat(gearboxData.Gears.Count + 1)
						.Cast<ITransmissionInputData>().ToList(), engineData, axleGearData.AxleGear.Ratio,
					vehicleData.DynamicTyreRadius);
			}

			var runData = new VectoRunData() {
				//PowertrainConfiguration = PowertrainConfiguration.ParallelHybrid,
				JobRunId = 0,
				SimulationType = SimulationType.DistanceCycle,
				DriverData = driverData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				JobName = modFileName,
				Cycle = cycleData,
				Retarder = new RetarderData() { Type = RetarderType.None },
				Aux = new List<VectoRunData.AuxData>(),
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				EngineData = engineData,
				//BatteryData = batteryData,
				//HybridStrategy = strategySettings
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio, engineData.IdleSpeed)
			};
			var fileWriter = new FileOutputWriter(modFileName);
			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(runData, fileWriter, null, modDataFilter)
			{
				WriteModalResults = true,
			};

			var container = VehicleContainer.CreateVehicleContainer(runData, modData, sumData);
			
			var engine = new StopStartCombustionEngine(container, runData.EngineData);
			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var aux = new HighVoltageElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", pAuxEl.SI<Watt>());
			cycle
				.AddComponent(new Driver(container, runData.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, runData.VehicleData, runData.AirdragData))
				.AddComponent(new Wheels(container, runData.VehicleData.DynamicTyreRadius, runData.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, runData.AxleGearData))
				.AddComponent(runData.AngledriveData != null ? new Angledrive(container, runData.AngledriveData) : null)
				.AddComponent(runData.Retarder.Type == RetarderType.TransmissionOutputRetarder ? new Retarder(container, 
					runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
				.AddComponent(new AMTGearbox(container, new AMTShiftStrategyOptimized(container)))
				.AddComponent(runData.Retarder.Type == RetarderType.TransmissionInputRetarder ? new Retarder(container, 
					runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
				.AddComponent(new SwitchableClutch(container, runData.EngineData))
				.AddComponent(engine, engine.IdleController);
			//PowertrainBuilderBase.AddAuxiliaries(engine, container, runData);

            return container;
		}

		public static ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
		{
			var retVal = new ShiftStrategyParameters {
				TimeBetweenGearshifts = 2.SI<Second>(),
				TorqueReserve = 0.2,
				StartTorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
				StartSpeed = 2.SI<MeterPerSecond>(),
				StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),

				RatingFactorCurrentGear = gbx.Type.AutomaticTransmission()
					? DeclarationData.GearboxTCU.RatingFactorCurrentGearAT
					: DeclarationData.GearboxTCU.RatingFactorCurrentGear,
				
				//--------------------
				RatioEarlyUpshiftFC = DeclarationData.GearboxTCU.RatioEarlyUpshiftFC / axleRatio,
				RatioEarlyDownshiftFC = DeclarationData.GearboxTCU.RatioEarlyDownshiftFC / axleRatio,
				AllowedGearRangeFC = gbx.Type.AutomaticTransmission()
					? (gbx.Gears.Count > DeclarationData.GearboxTCU.ATSkipGearsThreshold
						? DeclarationData.GearboxTCU.AllowedGearRangeFCATSkipGear
						: DeclarationData.GearboxTCU.AllowedGearRangeFCAT)
					: DeclarationData.GearboxTCU.AllowedGearRangeFCAMT,
				VelocityDropFactor = DeclarationData.GearboxTCU.VelocityDropFactor,
				AccelerationFactor = DeclarationData.GearboxTCU.AccelerationFactor,
				MinEngineSpeedPostUpshift = 0.RPMtoRad(),
				ATLookAheadTime = DeclarationData.GearboxTCU.ATLookAheadTime,

				LoadStageThresoldsUp = DeclarationData.GearboxTCU.LoadStageThresholdsUp,
				LoadStageThresoldsDown = DeclarationData.GearboxTCU.LoadStageThresoldsDown,
				ShiftSpeedsTCToLocked = DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked
														.Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
			};

			return retVal;
		}

		private static IElectricMotor GetElectricMachine(PowertrainPosition pos,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, IVehicleContainer container,
			IElectricSystem es, IHybridController ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null) {
				return null;
			}

			//container.ModData.AddElectricMotor(pos);
			ctl.AddElectricMotor(pos, motorData.Item2);
			var motor = new ElectricMotor(container, motorData.Item2, ctl.ElectricMotorControl(pos), pos);
			motor.Connect(es);
			return motor;
		}

		private static GearboxData CreateGearboxData(GearboxType gearboxType = GearboxType.NoGearbox)
		{
			switch (gearboxType) {
				
				case GearboxType.NoGearbox:
				case GearboxType.AMT:
					return CreateAMTGearbox();
				case GearboxType.ATSerial:
					return CreateATSerial();
				case GearboxType.ATPowerSplit:
					return CreateATPowerSplit();
				default:
					throw new ArgumentOutOfRangeException(nameof(gearboxType), gearboxType, null);
			}
		}

		private static GearboxData CreateAMTGearbox()
		{
			var ratios = new[] { 14.93, 11.64, 9.02, 7.04, 5.64, 4.4, 3.39, 2.65, 2.05, 1.6, 1.28, 1.0 };

			return new GearboxData {
				Gears = ratios.Select(
					(ratio, i) => Tuple.Create(
						(uint)i, new GearData {
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap =
								TransmissionLossMapReader.ReadFromFile(
									ratio.IsEqual(1) ? GearboxIndirectLoss : GearboxDirectLoss, ratio,
									$"Gear {i}"),
							Ratio = ratio,
							//ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(i,)
						})).ToDictionary(k => k.Item1 + 1, v => v.Item2),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
				Type = GearboxType.AMT
			};
		}

		private static GearboxData CreateATSerial()
		{
			//var shiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(
			//		gearbox.Type, (int)i, engine.FullLoadCurves[i + 1],
			//		gearsInput, engine,
			//		axlegearRatio, dynamicTyreRadius, null);

			var ratios = new[] { 3.4, 1.9, 1.42, 1.0, 0.7, 0.62 };
			return new GearboxData {
				Type = GearboxType.ATSerial,
				Gears = ratios.Select(
					(ratio, i) => Tuple.Create(
						(uint)i, new GearData {
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap =
								TransmissionLossMapReader.ReadFromFile(
									ratio.IsEqual(1) ? GearboxIndirectLoss : GearboxDirectLoss, ratio,
									$"Gear {i}"),
							Ratio = ratio,
							//ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile),
							TorqueConverterRatio = i == 0 ? ratio : double.NaN,
							TorqueConverterGearLossMap = i == 0
								? TransmissionLossMapReader.Create( 0.98, ratio, $"Gear {i}")
								: null,
							//TorqueConverterShiftPolygon = i == 0 ? ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile) : null
						})).ToDictionary(k => k.Item1 + 1, v => v.Item2),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
				TorqueConverterData = TorqueConverterDataReader.ReadFromFile(TorqueConverterSerial, 1000.RPMtoRad(),
					5000.RPMtoRad(), ExecutionMode.Engineering, 1.0, DeclarationData.Gearbox.UpshiftMinAcceleration,
					DeclarationData.Gearbox.UpshiftMinAcceleration), 
				PowershiftShiftTime = DeclarationData.Gearbox.PowershiftShiftTime
		};
		}

		private static GearboxData CreateATPowerSplit()
		{
			var ratios = new[] { 1.35, 1.0, 0.73 };
			return new GearboxData {
				Type = GearboxType.ATPowerSplit,
				Gears = ratios.Select(
					(ratio, i) => Tuple.Create(
						(uint)i, new GearData {
							//MaxTorque = 2300.SI<NewtonMeter>(),
							LossMap =
								TransmissionLossMapReader.ReadFromFile(
									ratio.IsEqual(1) ? GearboxIndirectLoss : GearboxDirectLoss, ratio,
									$"Gear {i}"),
							Ratio = ratio,
							//ShiftPolygon = ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile),
							TorqueConverterRatio = i == 0 ? 1.0 : double.NaN,
							TorqueConverterGearLossMap = i == 0
								? TransmissionLossMapReader.Create(1.0, ratio, $"Gear {i}")
								: null,
							//TorqueConverterShiftPolygon = i == 0 ? ShiftPolygonReader.ReadFromFile(GearboxShiftPolygonFile) : null
						})).ToDictionary(k => k.Item1 + 1, v => v.Item2),
				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
				TorqueConverterData = TorqueConverterDataReader.ReadFromFile(TorqueConverterSerial, 1000.RPMtoRad(),
					5000.RPMtoRad(), ExecutionMode.Engineering, 1.0 / ratios[0], DeclarationData.Gearbox.UpshiftMinAcceleration,
					DeclarationData.Gearbox.UpshiftMinAcceleration),
				PowershiftShiftTime = DeclarationData.Gearbox.PowershiftShiftTime
			};
		}

		private static AxleGearData CreateAxleGearData(GearboxType gearboxType)
		{
			var ratio = 2.59;
			switch (gearboxType) {
				case GearboxType.ATSerial:
					ratio = 6.2;
					break;
				case GearboxType.ATPowerSplit:
					ratio = 5.8;
					break;
			}
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.Create(0.95, ratio, "Axlegear"),
				}
			};
		}

		private static VehicleData CreateVehicleData(Kilogram loading)
		{
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.38,
					Inertia = 20.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = false,
					TyreTestLoad = 30436.0.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.62,
					Inertia = 18.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.007,
					TwinTyres = true,
					TyreTestLoad = 30436.SI<Newton>()
				},
			};
			var inputData = new Mock<IVehicleDeclarationInputData>();
			inputData.Setup(v => v.VehicleType).Returns(VectoSimulationJobType.ParallelHybridVehicle);
			return new VehicleData {
				AirDensity = DeclarationData.AirDensity,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				CurbMass = 11500.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = 0.465.SI<Meter>(),
				AxleData = axles,
				SavedInDeclarationMode = false,
				ADAS = new VehicleData.ADASData() {
					EngineStopStart = true
				},
				InputData = inputData.Object,
			};
		}

		private static AirdragData CreateAirdragData()
		{
			return new AirdragData() {
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(
						3.2634.SI<SquareMeter>(), 0.SI<SquareMeter>(), 
                        CrossWindCorrectionCurveReader.GetNoCorrectionCurve(3.2634.SI<SquareMeter>()),
						CrossWindCorrectionMode.NoCorrection),
			};
		}

		private static DriverData CreateDriverData(string accelerationFile, bool overspeed = false)
		{
			return new DriverData {
				AccelerationCurve = AccelerationCurveReader.ReadFromFile(accelerationFile),
				LookAheadCoasting = new DriverData.LACData {
					Enabled = true,
					MinSpeed = 50.KMPHtoMeterPerSecond(),

					//Deceleration = -0.5.SI<MeterPerSquareSecond>()
					LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
					LookAheadDecisionFactor = new LACDecisionFactor()
				},
				OverSpeed = new DriverData.OverSpeedData() {
					Enabled = true,
					MinSpeed = 50.KMPHtoMeterPerSecond(),
					OverSpeed = 5.KMPHtoMeterPerSecond()
				},
				EngineStopStart = new DriverData.EngineStopStartData() {
					EngineOffStandStillActivationDelay = DeclarationData.Driver.GetEngineStopStartLorry().ActivationDelay,
					MaxEngineOffTimespan = DeclarationData.Driver.GetEngineStopStartLorry().MaxEngineOffTimespan,
					UtilityFactorStandstill = DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor
				}
			};
		}
	}
}