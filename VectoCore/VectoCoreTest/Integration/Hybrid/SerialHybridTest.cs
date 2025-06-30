using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.Impl.ElectricSystem;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;
using Moq;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;


namespace TUGraz.VectoCore.Tests.Integration.Hybrid
{
    [TestFixture,
	Parallelizable(ParallelScope.All)]
	public class SerialHybridTest
	{
		public const string BatFile = @"TestData/Hybrids/GenericVehicle_Sx/GenericBattery.vreess";

		public const string AccelerationFile = @"TestData/Hybrids/GenericVehicle_Sx/Truck.vacc";
		public const string MotorFile = @"TestData/Hybrids/GenericVehicle_Sx/GenericEMotor.vem";

		public const string GeneratorFile = @"TestData/Hybrids/GenericVehicle_Sx/GenericGen.vem";

		public const string GearboxIndirectLoss = @"TestData/Components/Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData/Components/Direct Gear.vtlm";

		public const string EngineFile = @"TestData/Hybrids/GenericVehicle_Sx/Group2_6l.veng";

		public const string SerialHybrid_S2_3Speed_Job = @"TestData/Hybrids/GenericVehicle_S2_Job/SerialHybrid_S2_3Speed.vecto";
		public const string SerialHybrid_S2_12Speed_Job = @"TestData/Hybrids/GenericVehicle_S2_Job/SerialHybrid_S2.vecto";
		public const string SerialHybrid_S2_APTN_Job = @"TestData/Hybrids/GenericVehicle_S2_APTN/HEV_S2_Group5LH_rl_APTN.vecto";
		public const string SerialHybrid_S2_APTS_Job = @"TestData/Hybrids/GenericVehicle_S2_AT/HEV_S2_Group5LH_rl_APTS.vecto";
		public const string SerialHybrid_S2_APTP_Job = @"TestData/Hybrids/GenericVehicle_S2_AT/HEV_S2_Group5LH_rl_APTP.vecto";

		public const string SerialHybrid_S2_3Speed_PTO_Job = @"TestData/Hybrids/GenericVehicle_S2_Job/SerialHybrid_S2_3Speed_PTO.vecto";

		public const bool PlotGraphs = true;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		// - - - - - - - - - - - - - - - - - - - - - - - - - 
		[
			TestCase(SerialHybrid_S2_3Speed_Job, 0, TestName = "Generic Serial Hybrid S2 AMT 3Speed Job, LongHaul"),
			TestCase(SerialHybrid_S2_3Speed_Job, 1, TestName = "Generic Serial Hybrid S2 AMT 3Speed Job, RegionalDelivery"),
			TestCase(SerialHybrid_S2_3Speed_Job, 2, TestName = "Generic Serial Hybrid S2 AMT 3Speed Job, UrbanDelivery"),
			TestCase(SerialHybrid_S2_3Speed_Job, 3, TestName = "Generic Serial Hybrid S2 AMT 3Speed Job, Construction"),
			TestCase(SerialHybrid_S2_3Speed_Job, 4, TestName = "Generic Serial Hybrid S2 AMT 3Speed Job, Urban"),
			TestCase(SerialHybrid_S2_3Speed_Job, 5, TestName = "Generic Serial Hybrid S2 AMT 3Speed Job, Suburban"),
			TestCase(SerialHybrid_S2_3Speed_Job, 6, TestName = "Generic Serial Hybrid S2 AMT 3Speed Job, Interurban"),
			TestCase(SerialHybrid_S2_3Speed_Job, 7, TestName = "Generic Serial Hybrid S2 AMT 3Speed Job, Coach"),
		]
		[
			TestCase(SerialHybrid_S2_12Speed_Job, 0, TestName = "Generic Serial Hybrid S2 AMT 12speed Job, LongHaul"),
			TestCase(SerialHybrid_S2_12Speed_Job, 1, TestName = "Generic Serial Hybrid S2 AMT 12speed Job, RegionalDelivery"),
			TestCase(SerialHybrid_S2_12Speed_Job, 2, TestName = "Generic Serial Hybrid S2 AMT 12speed Job, UrbanDelivery"),
			TestCase(SerialHybrid_S2_12Speed_Job, 3, TestName = "Generic Serial Hybrid S2 AMT 12speed Job, Construction"),
			TestCase(SerialHybrid_S2_12Speed_Job, 4, TestName = "Generic Serial Hybrid S2 AMT 12speed Job, Urban"),
			TestCase(SerialHybrid_S2_12Speed_Job, 5, TestName = "Generic Serial Hybrid S2 AMT 12speed Job, Suburban"),
			TestCase(SerialHybrid_S2_12Speed_Job, 6, TestName = "Generic Serial Hybrid S2 AMT 12speed Job, Interurban"),
			TestCase(SerialHybrid_S2_12Speed_Job, 7, TestName = "Generic Serial Hybrid S2 AMT 12speed Job, Coach"),
		]
		[
			TestCase(SerialHybrid_S2_APTN_Job, 0, TestName = "Generic Serial Hybrid S2 APT-N 3speed Job, LongHaul"),
			TestCase(SerialHybrid_S2_APTN_Job, 1, TestName = "Generic Serial Hybrid S2 APT-N 3speed Job, RegionalDelivery"),
			TestCase(SerialHybrid_S2_APTN_Job, 2, TestName = "Generic Serial Hybrid S2 APT-N 3speed Job, UrbanDelivery"),
			TestCase(SerialHybrid_S2_APTN_Job, 3, TestName = "Generic Serial Hybrid S2 APT-N 3speed Job, Construction"),
			TestCase(SerialHybrid_S2_APTN_Job, 4, TestName = "Generic Serial Hybrid S2 APT-N 3speed Job, Urban"),
			TestCase(SerialHybrid_S2_APTN_Job, 5, TestName = "Generic Serial Hybrid S2 APT-N 3speed Job, Suburban"),
			TestCase(SerialHybrid_S2_APTN_Job, 6, TestName = "Generic Serial Hybrid S2 APT-N 3speed Job, Interurban"),
			TestCase(SerialHybrid_S2_APTN_Job, 7, TestName = "Generic Serial Hybrid S2 APT-N 3speed Job, Coach"),
		]
		[
			TestCase(SerialHybrid_S2_APTS_Job, 0, TestName = "Generic Serial Hybrid S2 APT-S Job, LongHaul"),
			TestCase(SerialHybrid_S2_APTS_Job, 1, TestName = "Generic Serial Hybrid S2 APT-S Job, RegionalDelivery"),
			TestCase(SerialHybrid_S2_APTS_Job, 2, TestName = "Generic Serial Hybrid S2 APT-S Job, UrbanDelivery"),
			TestCase(SerialHybrid_S2_APTS_Job, 3, TestName = "Generic Serial Hybrid S2 APT-S Job, Construction"),
			TestCase(SerialHybrid_S2_APTS_Job, 4, TestName = "Generic Serial Hybrid S2 APT-S Job, Urban"),
			TestCase(SerialHybrid_S2_APTS_Job, 5, TestName = "Generic Serial Hybrid S2 APT-S Job, Suburban"),
			TestCase(SerialHybrid_S2_APTS_Job, 6, TestName = "Generic Serial Hybrid S2 APT-S Job, Interurban"),
			TestCase(SerialHybrid_S2_APTS_Job, 7, TestName = "Generic Serial Hybrid S2 APT-S Job, Coach"),
		]
		[
			TestCase(SerialHybrid_S2_APTP_Job, 0, TestName = "Generic Serial Hybrid S2 APT-P Job, LongHaul"),
			TestCase(SerialHybrid_S2_APTP_Job, 1, TestName = "Generic Serial Hybrid S2 APT-P Job, RegionalDelivery"),
			TestCase(SerialHybrid_S2_APTP_Job, 2, TestName = "Generic Serial Hybrid S2 APT-P Job, UrbanDelivery"),
			TestCase(SerialHybrid_S2_APTP_Job, 3, TestName = "Generic Serial Hybrid S2 APT-P Job, Construction"),
			TestCase(SerialHybrid_S2_APTP_Job, 4, TestName = "Generic Serial Hybrid S2 APT-P Job, Urban"),
			TestCase(SerialHybrid_S2_APTP_Job, 5, TestName = "Generic Serial Hybrid S2 APT-P Job, Suburban"),
			TestCase(SerialHybrid_S2_APTP_Job, 6, TestName = "Generic Serial Hybrid S2 APT-P Job, Interurban"),
			TestCase(SerialHybrid_S2_APTP_Job, 7, TestName = "Generic Serial Hybrid S2 APT-P Job, Coach"),
		]
		[
			TestCase(SerialHybrid_S2_3Speed_PTO_Job, 0, TestName = "Generic Serial Hybrid S2 AMT 3Speed PTO Job, LongHaul"),
			TestCase(SerialHybrid_S2_3Speed_PTO_Job, 1, TestName = "Generic Serial Hybrid S2 AMT 3Speed PTO Job, RegionalDelivery"),
			TestCase(SerialHybrid_S2_3Speed_PTO_Job, 2, TestName = "Generic Serial Hybrid S2 AMT 3Speed PTO Job, UrbanDelivery"),
			TestCase(SerialHybrid_S2_3Speed_PTO_Job, 3, TestName = "Generic Serial Hybrid S2 AMT 3Speed PTO Job, Construction"),
			TestCase(SerialHybrid_S2_3Speed_PTO_Job, 4, TestName = "Generic Serial Hybrid S2 AMT 3Speed PTO Job, Urban"),
			TestCase(SerialHybrid_S2_3Speed_PTO_Job, 5, TestName = "Generic Serial Hybrid S2 AMT 3Speed PTO Job, Suburban"),
			TestCase(SerialHybrid_S2_3Speed_PTO_Job, 6, TestName = "Generic Serial Hybrid S2 AMT 3Speed PTO Job, Interurban"),
			TestCase(SerialHybrid_S2_3Speed_PTO_Job, 7, TestName = "Generic Serial Hybrid S2 AMT 3Speed PTO Job, Coach"),
		]
		[
			TestCase(@"TestData/Hybrids/GenericVehicle_S2_Job/SerialHybrid_S2_WHR.vecto", 1, TestName = "Generic Serial Hybrid S2 AMT WHR 12speed Job, RegionalDelivery"),
		]
		public void S2SerialHybridJob(string jobFile, int runIdx)
		{
			RunHybridJob(jobFile, runIdx);
		}


		// =================================================

		[
			TestCase(30, 0.7, 0, 0, TestName = "S3 Serial Hybrid ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "S3 Serial Hybrid ConstantSpeed 50km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, 0, TestName = "S3 Serial Hybrid ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "S3 Serial Hybrid ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "S3 Serial Hybrid ConstantSpeed 50km/h SoC: 0.25, level"),
			TestCase(80, 0.25, 0, 0, TestName = "S3 Serial Hybrid ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "S3 Serial Hybrid ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "S3 Serial Hybrid ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			TestCase(80, 0.5, 5, 0, TestName = "S3 Serial Hybrid ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "S3 Serial Hybrid ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "S3 Serial Hybrid ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			TestCase(80, 0.5, -5, 0, TestName = "S3 Serial Hybrid ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "S3 Serial Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "S3 Serial Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		]
		public void S3HybridConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = $"SimpleParallelHybrid-S3_constant_{vmax}-{initialSoC}_{slope}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 12.47, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;

			var hybridController = (SerialHybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3, ModalResultField.P_electricMotor_mech_Gen });
			graphWriter.Write(modFilename);
		}

		[
			TestCase(30, 0.7, 0, TestName = "S3 Serial Hybrid DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "S3 Serial Hybrid DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.22, 0, TestName = "S3 Serial Hybrid DriveOff 30km/h SoC: 0.22, level")
		]
		public void S3HybridDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = $"SimpleParallelHybrid-S3_acc_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 12.47);
			var run = job.Runs.First().Run;

			var hybridController = (SerialHybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3, ModalResultField.P_electricMotor_mech_Gen });
			graphWriter.Write(modFilename);
		}

		[TestCase(50, 0.79, 0, TestName = "S3 Serial Hybrid Brake Standstill 50km/h SoC: 0.79, level"),
		TestCase(50, 0.25, 0, TestName = "S3 Serial Hybrid Brake Standstill 50km/h SoC: 0.25, level"),
		TestCase(50, 0.65, 0, TestName = "S3 Serial Hybrid Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void S3HybridBrakeStandstill(double vmax, double initialSoC, double slope)
		{
			//var dst =
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = $"SimpleParallelHybrid-S3_stop_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 12.47);
			var run = job.Runs.First().Run;

			var hybridController = (SerialHybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3, ModalResultField.P_electricMotor_mech_Gen });
			graphWriter.Write(modFilename);
		}


		[
			TestCase("LongHaul", 2000, 0.5, 0, TestName = "S3 Serial Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, TestName = "S3 Serial Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, TestName = "S3 Serial Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.5, 0, TestName = "S3 Serial Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.5, 0, TestName = "S3 Serial Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.5, 0, TestName = "S3 Serial Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.5, 0, TestName = "S3 Serial Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.5, 0, TestName = "S3 Serial Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
		]
		public void S3HybridDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			const bool largeMotor = true;

			var modFilename = $"SimpleParallelHybrid-S3_cycle_{declarationMission}-{initialSoC}_{payload}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 12.47, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (SerialHybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3, ModalResultField.P_electricMotor_mech_Gen });
			graphWriter.Write(modFilename);
		}

		// =================================================

		[
			TestCase(30, 0.7, 0, 0, TestName = "S4 Serial Hybrid ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "S4 Serial Hybrid ConstantSpeed 50km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, 0, TestName = "S4 Serial Hybrid ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "S4 Serial Hybrid ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "S4 Serial Hybrid ConstantSpeed 50km/h SoC: 0.25, level"),
			TestCase(80, 0.25, 0, 0, TestName = "S4 Serial Hybrid ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "S4 Serial Hybrid ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "S4 Serial Hybrid ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			TestCase(80, 0.5, 5, 0, TestName = "S4 Serial Hybrid ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "S4 Serial Hybrid ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "S4 Serial Hybrid ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			TestCase(80, 0.5, -5, 0, TestName = "S4 Serial Hybrid ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "S4 Serial Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "S4 Serial Hybrid ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		]
		public void S4HybridConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = $"SimpleParallelHybrid-S4_constant_{vmax}-{initialSoC}_{slope}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 12.47, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;

			var hybridController = (SerialHybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);
			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4, ModalResultField.P_electricMotor_mech_Gen });
			graphWriter.Write(modFilename);
		}

		[
			TestCase(30, 0.7, 0, TestName = "S4 Serial Hybrid DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "S4 Serial Hybrid DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.22, 0, TestName = "S4 Serial Hybrid DriveOff 30km/h SoC: 0.22, level"),
			TestCase(80, 0.22, 0, TestName = "S4 Serial Hybrid DriveOff 80km/h SoC: 0.22, level"),
		]
		public void S4HybridDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = $"SimpleParallelHybrid-S4_acc_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 12.47);
			var run = job.Runs.First().Run;

			var hybridController = (SerialHybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4, ModalResultField.P_electricMotor_mech_Gen });
			graphWriter.Write(modFilename);
		}

		[TestCase(50, 0.79, 0, TestName = "S4 Serial Hybrid Brake Standstill 50km/h SoC: 0.79, level"),
		TestCase(50, 0.25, 0, TestName = "S4 Serial Hybrid Brake Standstill 50km/h SoC: 0.25, level"),
		TestCase(50, 0.65, 0, TestName = "S4 Serial Hybrid Brake Standstill 50km/h SoC: 0.65, level")
		]
		public void S4HybridBrakeStandstill(double vmax, double initialSoC, double slope)
		{
			//var dst =
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   200,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			const bool largeMotor = true;

			var modFilename = $"SimpleParallelHybrid-S4_stop_{vmax}-{initialSoC}_{slope}.vmod";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 12.47);
			var run = job.Runs.First().Run;

			var hybridController = (SerialHybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);
			//var strategy = (DelegateParallelHybridStrategy)hybridController.Strategy;
			//Assert.NotNull(strategy);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4, ModalResultField.P_electricMotor_mech_Gen });
			graphWriter.Write(modFilename);
		}




		[
			TestCase("LongHaul", 2000, 0.5, 0, TestName = "S4 Serial Hybrid DriveCycle LongHaul, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.5, 0, TestName = "S4 Serial Hybrid DriveCycle RegionalDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.5, 0, TestName = "S4 Serial Hybrid DriveCycle UrbanDelivery, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.5, 0, TestName = "S4 Serial Hybrid DriveCycle Construction, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.5, 0, TestName = "S4 Serial Hybrid DriveCycle Urban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.5, 0, TestName = "S4 Serial Hybrid DriveCycle SubUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.5, 0, TestName = "S4 Serial Hybrid DriveCycle InterUrban, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.5, 0, TestName = "S4 Serial Hybrid DriveCycle Coach, SoC: 0.5 Payload: 2t P_auxEl: 0kW"),
		]
		public void S4HybridDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			var modFilename = $"SimpleSerialHybrid-S4_cycle_{declarationMission}-{initialSoC}_{payload}_{pAuxEl}.vmod";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 12.47, pAuxEl: pAuxEl, payload: payload.SI<Kilogram>());
			var run = job.Runs.First().Run;

			var hybridController = (SerialHybridController)((VehicleContainer)run.GetContainer()).HybridController;
			Assert.NotNull(hybridController);

			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4, ModalResultField.P_electricMotor_mech_Gen });
			graphWriter.Write(modFilename);
		}



		// =================================================

		[
			TestCase(@"TestData/Hybrids/GenericVehicle_Sx_Job/SerialHybrid_S4.vecto", 0, TestName = "Generic Serial Hybrid S4 Job, LongHaul"),
			TestCase(@"TestData/Hybrids/GenericVehicle_Sx_Job/SerialHybrid_S4.vecto", 1, TestName = "Generic Serial Hybrid S4 Job, RegionalDelivery"),
			TestCase(@"TestData/Hybrids/GenericVehicle_Sx_Job/SerialHybrid_S4.vecto", 2, TestName = "Generic Serial Hybrid S4 Job, UrbanDelivery"),
			TestCase(@"TestData/Hybrids/GenericVehicle_Sx_Job/SerialHybrid_S4.vecto", 3, TestName = "Generic Serial Hybrid S4 Job, Construction"),
			TestCase(@"TestData/Hybrids/GenericVehicle_Sx_Job/SerialHybrid_S4.vecto", 4, TestName = "Generic Serial Hybrid S4 Job, Urban"),
			TestCase(@"TestData/Hybrids/GenericVehicle_Sx_Job/SerialHybrid_S4.vecto", 5, TestName = "Generic Serial Hybrid S4 Job, Suburban"),
			TestCase(@"TestData/Hybrids/GenericVehicle_Sx_Job/SerialHybrid_S4.vecto", 6, TestName = "Generic Serial Hybrid S4 Job, Interurban"),
			TestCase(@"TestData/Hybrids/GenericVehicle_Sx_Job/SerialHybrid_S4.vecto", 7, TestName = "Generic Serial Hybrid S4 Job, Coach"),
		]
		public void S4SerialHybridJob(string jobFile, int runIdx)
		{
			RunHybridJob(jobFile, runIdx);
		}


		// =================================================

		private ModalResults RunHybridJob(string jobFile, int runIdx, int? startDistance = null)
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			var writer = new FileOutputWriter(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.Validate = false;
			factory.WriteModalResults = true;

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			var run = factory.SimulationRuns().ToArray()[runIdx];

			if (startDistance != null) {
				//(run.GetContainer().BatteryInfo as Battery).PreviousState.StateOfCharge = 0.317781;
				(run.GetContainer().RunData.Cycle as DrivingCycleProxy).Entries = run.GetContainer().RunData.Cycle
					.Entries.Where(x => x.Distance >= startDistance.Value).ToList();
				//run.GetContainer().MileageCounter.Distance = startDistance;
				//run.GetContainer().DrivingCycleInfo.CycleStartDistance = startDistance;
			}

			Assert.NotNull(run);

			var pt = run.GetContainer();
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;
			Assert.NotNull(pt);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
			return modData;
		}
		
		[TestCase]
		public void Run_S3_AxlegearInputRetarder()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData("0, 80, 0, 0\n100, 80, 0, 0");
			var job = CreateEngineeringRun(cycle, $"{MethodBase.GetCurrentMethod()}.vmod", 0.5, 
				PowertrainPosition.BatteryElectricE3, 12.47, retarderType:RetarderType.AxlegearInputRetarder);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);
			Assert.That(modData.Columns.Contains(ModalResultField.P_ret_loss.GetName()));
			Assert.That(modData.Columns.Contains(ModalResultField.P_retarder_in.GetName()));
			Assert.That(modData.Sum(r=>r.Field<Watt>(ModalResultField.P_ret_loss.GetName()).Value()), Is.GreaterThan(0));
			Assert.That(modData.Sum(r => r.Field<Watt>(ModalResultField.P_retarder_in.GetName()).Value()), Is.GreaterThan(0));
		}

		[TestCase]
		public void Run_S3_WithoutAxlegearInputRetarder()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData("0, 80, 0, 0\n100, 80, 0, 0");
			var job = CreateEngineeringRun(cycle, $"{MethodBase.GetCurrentMethod()}.vmod", 0.5,
				PowertrainPosition.BatteryElectricE3, 12.47, retarderType: RetarderType.None);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);
			Assert.IsFalse(modData.Columns.Contains(ModalResultField.P_ret_loss.GetName()));
			Assert.IsFalse(modData.Columns.Contains(ModalResultField.P_retarder_in.GetName()));
			//Assert.That(modData.Rows.Cast<DataRow>().All(r => r.Field<Watt>(ModalResultField.P_ret_loss.GetName()) is null));
			//Assert.That(modData.Rows.Cast<DataRow>().All(r => r.Field<Watt>(ModalResultField.P_retarder_in.GetName()) is null));
		}

		[TestCase]
		public void RunJob_S3_AxlegearInputRetarder()
		{
			var modData = RunHybridJob(@"TestData/Components/Retarder/S3/S3WithAxlegearInputRetarder.vecto", 0);
			Assert.IsTrue(modData.Rows.Count > 0);
			Assert.That(modData.Columns.Contains(ModalResultField.P_ret_loss.GetName()));
			Assert.That(modData.Columns.Contains(ModalResultField.P_retarder_in.GetName()));
			Assert.That(modData.Sum(r => r.Field<Watt>(ModalResultField.P_ret_loss.GetName()).Value()), Is.GreaterThan(0));
			Assert.That(modData.Sum(r => r.Field<Watt>(ModalResultField.P_retarder_in.GetName()).Value()), Is.GreaterThan(0));
		}

		[TestCase]
		public void RunJob_S3_NoAxlegearInputRetarder()
		{
			var modData = RunHybridJob(@"TestData/Components/Retarder/S3/S3WithoutAxlegearInputRetarder.vecto", 0);
			Assert.IsTrue(modData.Rows.Count > 0);
            Assert.IsFalse(modData.Columns.Contains(ModalResultField.P_ret_loss.GetName()));
            Assert.IsFalse(modData.Columns.Contains(ModalResultField.P_retarder_in.GetName()));
            //Assert.That(modData.Rows.Cast<DataRow>().All(r => r.Field<Watt>(ModalResultField.P_ret_loss.GetName()) is null));
            //Assert.That(modData.Rows.Cast<DataRow>().All(r => r.Field<Watt>(ModalResultField.P_retarder_in.GetName()) is null));
        }



		// =================================================

		public static JobContainer CreateEngineeringRun(DrivingCycleData cycleData, string modFileName, double initialSoc, 
			PowertrainPosition pos, double ratio, double pAuxEl = 0, Kilogram payload = null, Watt maxDriveTrainPower = null,
			GearboxType gearboxType = GearboxType.NoGearbox, RetarderType retarderType = RetarderType.None)
		{
			var fileWriter = new FileOutputWriter(Path.GetFileNameWithoutExtension(modFileName));
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			var container = CreateSerialHybridPowerTrain(cycleData, modFileName, initialSoc, sumData, pAuxEl, pos, ratio,
				payload, maxDriveTrainPower, gearboxType, retarderType);
			var run = new DistanceRun(container);
			jobContainer.AddRun(run);
			return jobContainer;
		}
		public static IVehicleContainer CreateSerialHybridPowerTrain(DrivingCycleData cycleData, string modFileName,
			double initialBatCharge, SummaryDataContainer sumData, double pAuxEl,
			PowertrainPosition pos, double ratio, Kilogram payload = null, Watt maxDriveTrainPower = null, 
			GearboxType gearboxType = GearboxType.NoGearbox, RetarderType retarderType = RetarderType.None)
		{
			
			var gearboxData = CreateGearboxData(gearboxType);
			var axleGearData = CreateAxleGearData(gearboxType);
			
			var vehicleData = CreateVehicleData(payload ?? 3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var emFile = MotorFile;
			var correctedRatio = pos == PowertrainPosition.BatteryElectricE3
				? ratio / axleGearData.AxleGear.Ratio
				: ratio;
			var electricMotorData = MockSimulationDataFactory.CreateElectricMotorData(emFile, 2, pos, correctedRatio, 1);

			var genFile = GeneratorFile;

			foreach (var md in MockSimulationDataFactory.CreateElectricMotorData(genFile, 1,
						PowertrainPosition.GEN, 2.6, 1)) {
				electricMotorData.Add(md);
			}
			//electricMotorData.AddRange(
			//	MockSimulationDataFactory.CreateElectricMotorData(genFile, 1, PowertrainPosition.GEN, 2.6, 1));

			var batFile = BatFile;
			var batteryData = MockSimulationDataFactory.CreateBatteryData(batFile, initialBatCharge);
			//batteryData.TargetSoC = 0.5;

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(
				 EngineFile, gearboxData.Gears.Count);

			foreach (var entry in gearboxData.Gears) {
				entry.Value.ShiftPolygon = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
					(int)entry.Key, engineData.FullLoadCurves[entry.Key], new TransmissionInputData().Repeat(gearboxData.Gears.Count + 1)
						.Cast<ITransmissionInputData>().ToList(), engineData, axleGearData.AxleGear.Ratio,
					vehicleData.DynamicTyreRadius);
			}

			var retarderLossMapEntries = new RetarderLossMap.RetarderLossEntry[] {
				new RetarderLossMap.RetarderLossEntry(){ RetarderSpeed = 0.RPMtoRad(), TorqueLoss = 10.SI<NewtonMeter>()},
				new RetarderLossMap.RetarderLossEntry(){ RetarderSpeed = 1000.RPMtoRad(), TorqueLoss = 12.SI<NewtonMeter>()},
				new RetarderLossMap.RetarderLossEntry(){ RetarderSpeed = 2000.RPMtoRad(), TorqueLoss = 18.SI<NewtonMeter>()},
				new RetarderLossMap.RetarderLossEntry(){ RetarderSpeed = 2300.RPMtoRad(), TorqueLoss = 20.58.SI<NewtonMeter>()},
			};
			var retarderData = new RetarderData { Type = retarderType, LossMap=new RetarderLossMap(retarderLossMapEntries), Ratio=1 };
			var runData = new VectoRunData {
				//PowertrainConfiguration = PowertrainConfiguration.ParallelHybrid,
				JobRunId = 0,
				JobType = VectoSimulationJobType.SerialHybridVehicle,
				SimulationType = SimulationType.DistanceCycle,
				DriverData = driverData,
				AxleGearData = axleGearData,
				GearboxData = gearboxData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				JobName = Path.GetFileNameWithoutExtension(modFileName),
				Cycle = cycleData,
				Retarder = retarderData,
				Aux = new List<VectoRunData.AuxData>(),
				ElectricMachinesData = electricMotorData,
				EngineData = engineData,
				BatteryData = batteryData,
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio, engineData.IdleSpeed),
				HybridStrategyParameters = CreateHybridStrategyData(),
				ElectricAuxDemand = pAuxEl.SI<Watt>()
			};
			var fileWriter = new FileOutputWriter(modFileName);
			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(runData, fileWriter, null, modDataFilter) {
				WriteModalResults = true,
			};
			var container = VehicleContainer.CreateVehicleContainer(runData, modData, 
				sumData);

			var strategy = new SerialHybridStrategy(runData, container);
			var es = new ElectricSystem(container, batteryData);
			var battery = new BatterySystem(container, batteryData);
			battery.Initialize(initialBatCharge);

			//var clutch = gearboxType.AutomaticTransmission() ? null : new SwitchableClutch(container, runData.EngineData);
			var ctl = new SerialHybridController(container, strategy, es);

			es.Connect(battery);

			var engine = new StopStartCombustionEngine(container, runData.EngineData);
			
			//var hybridStrategy = new DelegateParallelHybridStrategy();

			var idleController = engine.IdleController;
			ctl.Engine = engine;

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var aux = new HighVoltageElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", pAuxEl.SI<Watt>());
			es.Connect(aux);

			var powertrain = cycle
				.AddComponent(new Driver(container, runData.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, runData.VehicleData, runData.AirdragData))
				.AddComponent(new Wheels(container, runData.VehicleData.DynamicTyreRadius, runData.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container));

			switch (pos) {
				case PowertrainPosition.HybridPositionNotSet:
					throw new VectoException("invalid powertrain position");
				case PowertrainPosition.BatteryElectricE2:
					var gearbox = gearboxType.AutomaticTransmission()
						? (IHybridControlledGearbox)new APTGearbox(container, ctl.ShiftStrategy)
						: new AMTGearbox(container, ctl.ShiftStrategy);
					powertrain = powertrain.AddComponent(new AxleGear(container, runData.AxleGearData))
						.AddComponent(runData.AngledriveData != null ? new Angledrive(container, runData.AngledriveData) : null)
						.AddComponent(runData.Retarder.Type == RetarderType.TransmissionOutputRetarder 
							? new Retarder(container, runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
						.AddComponent((IGearbox)gearbox)
						.AddComponent(runData.Retarder.Type == RetarderType.TransmissionInputRetarder 
							? new Retarder(container, runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE2, 
							runData.ElectricMachinesData, container, es, ctl));
					ctl.Gearbox = gearbox;

					break;
					
				case PowertrainPosition.BatteryElectricE3:
					powertrain = powertrain.AddComponent(new AxleGear(container, runData.AxleGearData))
						.AddComponent(runData.Retarder.Type == RetarderType.AxlegearInputRetarder 
							? new Retarder(container, runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE3, 
							runData.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container, new GearshiftPosition(0));
					//new MockEngineInfo(container);
					new ATClutchInfo(container);
					runData.GearboxData = null;
					break;
				case PowertrainPosition.BatteryElectricE4:
					powertrain = powertrain.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE4, 
						runData.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container, new GearshiftPosition(0));
					//new MockEngineInfo(container);
					new ATClutchInfo(container);
					runData.GearboxData = null;
					break;
				case PowertrainPosition.HybridP0:
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2_5:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
				
					throw new VectoException("testcase does not support parallel powertrain configurations");
				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
			}

			ctl.GenSet.AddComponent(GetElectricMachine(PowertrainPosition.GEN, runData.ElectricMachinesData, container,
					es, ctl))
				.AddComponent(engine, idleController);
			//PowertrainBuilderBase.AddAuxiliaries(engine, container, runData);

			return container;
		}

		private static HybridStrategyParameters CreateHybridStrategyData()
		{
			return new HybridStrategyParameters() {
				MinSoC = 0.24,
				TargetSoC = 0.7,
				GensetMinOptPowerFactor = 0,
			};
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
			ElectricSystem es, IHybridController ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null) {
				return null;
			}

			//container.ModData.AddElectricMotor(pos);
			ctl.AddElectricMotor(pos, motorData.Item2);
			var motor = new ElectricMotor(container, motorData.Item2, ctl.ElectricMotorControl(pos), pos);
			if (pos == PowertrainPosition.GEN) {
				es.Connect(new GensetChargerAdapter(motor));
			} else {
				motor.Connect(es);
			}

			return motor;
		}

		private static GearboxData CreateGearboxData(GearboxType gearboxType = GearboxType.NoGearbox)
		{
			switch (gearboxType) {

				case GearboxType.NoGearbox:
				case GearboxType.AMT:
					return CreateAMTGearbox();
				//case GearboxType.ATSerial:
				//	return CreateATSerial();
				//case GearboxType.ATPowerSplit:
				//	return CreateATPowerSplit();
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
			inputData.Setup(v => v.VehicleType).Returns(VectoSimulationJobType.SerialHybridVehicle);
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


		// ======================

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

	}
}