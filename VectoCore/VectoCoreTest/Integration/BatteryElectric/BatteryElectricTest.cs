using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
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
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies.ShiftPolygonCalc;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.SimulationComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.Impl.ElectricSystem;


namespace TUGraz.VectoCore.Tests.Integration.BatteryElectric
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class BatteryElectricTest
	{

		protected const string BEV_E4_Job = @"TestData/BatteryElectric/GenericVehicleB4/BEV_ENG.vecto";
		protected const string BEV_E4_Job_Cont30kW = @"TestData/BatteryElectric/GenericVehicleB4/BEV_ENG_Cont30kW.vecto";

		protected const string BEV_E3_Job = @"TestData/BatteryElectric/GenericVehicleB3/BEV_ENG.vecto";
		protected const string BEV_E3_Job_Cont30kW = @"TestData/BatteryElectric/GenericVehicleB3/BEV_ENG_Cont30kW.vecto";

		protected const string BEV_E2_Job = @"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG.vecto";
		protected const string BEV_E2_Job_3Speed = @"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG_3speed.vecto";
		protected const string BEV_E2_Job_BusAux = @"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG_BusAux.vecto";
		protected const string BEV_E2_Job_Cont30kW = @"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG_Cont30kW.vecto";

		protected const string BEV_E2_APTN_Job = @"TestData/BatteryElectric/GenericVehicleB2_APTN/BEV_B2_Group5LH_rl_APTN.vecto";
		
		protected const string BEV_E2_APTS_Job = @"TestData/BatteryElectric/GenericVehicleB2_AT/BEV_B2_Group5LH_rl_APTS.vecto";
		protected const string BEV_E2_APTP_Job = @"TestData/BatteryElectric/GenericVehicleB2_AT/BEV_B2_Group5LH_rl_APTP.vecto";

		protected const string BEV_E2_3Speed_PTO_Job = @"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG_3speed_PTO.vecto";

		protected const string BEV_E2_Job_TqLimit = @"TestData\BatteryElectric\GenericVehicleB2\BEV_ENG_3speed_GbxTqLimit.vecto";
		protected const string BEV_E2_Job_SpdLimit = @"TestData\BatteryElectric\GenericVehicleB2\BEV_ENG_3speed_GbxSpdLimit.vecto";
		protected const string BEV_E2_Job_SpdTqLimit = @"TestData\BatteryElectric\GenericVehicleB2\BEV_ENG_3speed_GbxSpdTqLimit.vecto";


		public const string MotorFile = @"TestData\BatteryElectric\GenericVehicleB4\GenericEMotor_125kW_485Nm.vem";
		public const string BatFile = @"TestData\BatteryElectric\GenericVehicleB4\GenericBattery_243kWh_750V.vbat";

		public const string AccelerationFile = @"TestData/Components/Truck.vacc";
		//public const string MotorFile240kW = @"TestData/Hybrids/ElectricMotor/GenericEMotor240kW.vem";

		public const string GearboxIndirectLoss = @"TestData/Components/Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData/Components/Direct Gear.vtlm";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		private GraphWriter GetGraphWriter(ModalResultField[] yFields)
		{
			var graphWriter = new GraphWriter();
			//#if VECTOTRACE
			graphWriter.Enable();

			//#else
			//graphWriter.Disable();
			//#endif


			var Yfields = new[] {
				ModalResultField.v_act, ModalResultField.altitude, ModalResultField.acc, ModalResultField.Gear,
				ModalResultField.REESSStateOfCharge,
			}.Concat(yFields).ToArray();
			graphWriter.Xfields = new[] { ModalResultField.dist };
			graphWriter.Yfields = yFields;
			graphWriter.Series1Label = "BEV";
			graphWriter.PlotIgnitionState = true;

			return graphWriter;
		}


		[
			TestCase(30, 0.7, 0, 0, TestName = "PEV E4 ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "PEV E4 ConstantSpeed 50km/h SoC: 0.7, level"),
			//TestCase(80, 0.7, 0, 0, TestName = "PEV E4 ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "PEV E4 ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "PEV E4 ConstantSpeed 50km/h SoC: 0.25, level"),
			//TestCase(80, 0.25, 0, 0, TestName = "PEV E4 ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "PEV E4 ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "PEV E4 ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			//TestCase(80, 0.5, 5, 0, TestName = "PEV E4 ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "PEV E4 ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "PEV E4 ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			//TestCase(80, 0.5, -5, 0, TestName = "PEV E4 ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "PEV E4 ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "PEV E4 ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		] // the vehicle can drive max. 56km/h!! 80km/h testcase makes no sense
		public void B4PEVConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B4_constant_{vmax}-{initialSoC}_{slope}_{pAuxEl}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4 });
			graphWriter.Write(modFilename + ".vmod");
		}


		[
			TestCase(30, 0.7, 0, TestName = "PEV E4 DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "PEV E4 DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.25, 0, TestName = "PEV E4 DriveOff 30km/h SoC: 0.25, level")
		]
		public void B4PEVDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B4_acc_{vmax}-{initialSoC}_{slope}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4 });
			graphWriter.Write(modFilename + ".vmod");
		}

		[
			TestCase(30, 0.5, 0, TestName = "PEV E4 Halt 30km/h SoC: 0.5, level"),
			TestCase(50, 0.5, 0, TestName = "PEV E4 Halt 50km/h SoC: 0.5, level"),
			//TestCase(80, 0.5, 0, TestName = "PEV E4 Halt 80km/h SoC: 0.5, level"), // max vehicle speed: 56
			TestCase(30, 0.25, 0, TestName = "PEV E4 Halt 30km/h SoC: 0.25, level"),

		TestCase(50, 0.5, 12, TestName = "PEV E4 Halt 50km/h SoC: 0.5, uphill 12"),

		]
		public void B4PEVHalt(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1}, 0
				   900,   0, {1}, 3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B4_stop_{vmax}-{initialSoC}_{slope}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			if (slope.IsSmallerOrEqual(0) && vmax > 30) {
				((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data = modData;
				var selected = ((VehicleContainer)run.GetContainer()).ModData.GetValues(r => new {
					speed = (SI)r[ModalResultField.v_act.GetName()],
					brakePwr = (SI)r[ModalResultField.P_brake_loss.GetName()]
				}).Where(x => x.speed.IsSmaller(7.KMPHtoMeterPerSecond()) && x.speed.IsGreater(0));
				Assert.IsTrue(selected.All(x => x.brakePwr.IsGreater(0)));
			}

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4 });
			graphWriter.Write(modFilename + ".vmod");
		}

		[
			TestCase("LongHaul", 2000, 0.8, 0, TestName = "PEV E4 DriveCycle LongHaul, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.8, 0, TestName = "PEV E4 DriveCycle RegionalDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.8, 0, TestName = "PEV E4 DriveCycle UrbanDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.8, 0, TestName = "PEV E4 DriveCycle Construction, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.8, 0, TestName = "PEV E4 DriveCycle Urban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.8, 0, TestName = "PEV E4 DriveCycle SubUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.8, 0, TestName = "PEV E4 DriveCycle InterUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.8, 0, TestName = "PEV E4 DriveCycle Coach, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
		]
		public void B4PEVDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			var modFilename = $"SimpleParallelHybrid-B4_cycle_{declarationMission}-{initialSoC}_{payload}_{pAuxEl}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE4;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B4 });
			graphWriter.Write(modFilename + ".vmod");
		}

		[
			TestCase(BEV_E4_Job, 0, TestName = "PEV E4 Job RD"),
			TestCase(BEV_E4_Job_Cont30kW, 0, TestName = "PEV E4 Job Cont. 30kW RD")
		]
		public void B4PEVRunJob(string jobFile, int cycleIdx)
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


		// - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - -  - - - 

		[
			TestCase(30, 0.7, 0, 0, TestName = "PEV E3 ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "PEV E3 ConstantSpeed 50km/h SoC: 0.7, level"),
			//TestCase(80, 0.7, 0, 0, TestName = "PEV E4 ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "PEV E3 ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "PEV E3 ConstantSpeed 50km/h SoC: 0.25, level"),
			//TestCase(80, 0.25, 0, 0, TestName = "PEV E4 ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "PEV E3 ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "PEV E3 ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			//TestCase(80, 0.5, 5, 0, TestName = "PEV E4 ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "PEV E3 ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "PEV E3 ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			//TestCase(80, 0.5, -5, 0, TestName = "PEV E4 ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "PEV E3 ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "PEV E3 ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		] // the vehicle can drive max. 56km/h!! 80km/h testcase makes no sense
		public void B3PEVConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B3_constant_{vmax}-{initialSoC}_{slope}_{pAuxEl}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3 });
			graphWriter.Write(modFilename + ".vmod");
		}


		[
			TestCase(30, 0.7, 0, TestName = "PEV E3 DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "PEV E3 DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.25, 0, TestName = "PEV E3 DriveOff 30km/h SoC: 0.25, level")
		]
		public void B3PEVDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B3_acc_{vmax}-{initialSoC}_{slope}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3 });
			graphWriter.Write(modFilename + ".vmod");
		}

		[
			TestCase(30, 0.5, 0, TestName = "PEV E3 Halt 30km/h SoC: 0.5, level"),
			TestCase(50, 0.5, 0, TestName = "PEV E3 Halt 50km/h SoC: 0.5, level"),
			//TestCase(80, 0.5, 0, TestName = "PEV E3 Halt 80km/h SoC: 0.5, level"),  // max vehicle speed: 56 km/h
			TestCase(30, 0.25, 0, TestName = "PEV E3 Halt 30km/h SoC: 0.25, level"),

			TestCase(50, 0.5, 12, TestName = "PEV E3 Halt 50km/h SoC: 0.5, uphill 12"),

		]
		public void B3PEVHalt(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   900,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B3_halt_{vmax}-{initialSoC}_{slope}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			if (slope.IsSmallerOrEqual(0) && vmax > 30) {
				((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data = modData;
				var selected = ((VehicleContainer)run.GetContainer()).ModData.GetValues(r => new {
					speed = (SI)r[ModalResultField.v_act.GetName()],
					brakePwr = (SI)r[ModalResultField.P_brake_loss.GetName()]
				}).Where(x => x.speed.IsSmaller(7.KMPHtoMeterPerSecond()) && x.speed.IsGreater(0));
				Assert.IsTrue(selected.All(x => x.brakePwr.IsGreater(0)));
			}

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3 });
			graphWriter.Write(modFilename + ".vmod");
		}

		[
			TestCase("LongHaul", 2000, 0.8, 0, TestName = "PEV E3 DriveCycle LongHaul, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.8, 0, TestName = "PEV E3 DriveCycle RegionalDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.8, 0, TestName = "PEV E3 DriveCycle UrbanDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.8, 0, TestName = "PEV E3 DriveCycle Construction, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.8, 0, TestName = "PEV E3 DriveCycle Urban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.8, 0, TestName = "PEV E3 DriveCycle SubUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.8, 0, TestName = "PEV E3 DriveCycle InterUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.8, 0, TestName = "PEV E3 DriveCycle Coach, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
		]
		public void B3PEVDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			var modFilename = $"SimpleParallelHybrid-B3_cycle_{declarationMission}-{initialSoC}_{payload}_{pAuxEl}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE3;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 22.6, largeMotor: true);

			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B3 });
			graphWriter.Write(modFilename + ".vmod");
		}


		[
			TestCase(BEV_E3_Job, 0, TestName = "PEV E3 Job RD"),
			TestCase(BEV_E3_Job_Cont30kW, 0, TestName = "PEV E3 Job Cont. 30kW RD")
		]
		public void B3PEVRunJob(string jobFile, int cycleIdx)
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

		// - - - - - - - - - - - - - - - - - - -  - - - - - - - - - - -  - - - 

		[
			TestCase(30, 0.7, 0, 0, TestName = "PEV E2 ConstantSpeed 30km/h SoC: 0.7, level"),
			TestCase(50, 0.7, 0, 0, TestName = "PEV E2 ConstantSpeed 50km/h SoC: 0.7, level"),
			//TestCase(80, 0.7, 0, 0, TestName = "PEV E2 ConstantSpeed 80km/h SoC: 0.7, level"),

			TestCase(30, 0.25, 0, 0, TestName = "PEV E2 ConstantSpeed 30km/h SoC: 0.25, level"),
			TestCase(50, 0.25, 0, 0, TestName = "PEV E2 ConstantSpeed 50km/h SoC: 0.25, level"),
			//TestCase(80, 0.25, 0, 0, TestName = "PEV E2 ConstantSpeed 80km/h SoC: 0.25, level"),

			TestCase(30, 0.5, 5, 0, TestName = "PEV E2 ConstantSpeed 30km/h SoC: 0.5, UH 5%"),
			TestCase(50, 0.5, 5, 0, TestName = "PEV E2 ConstantSpeed 50km/h SoC: 0.5, UH 5%"),
			//TestCase(80, 0.5, 5, 0, TestName = "PEV E2 ConstantSpeed 80km/h SoC: 0.5, UH 5%"),

			TestCase(30, 0.5, -5, 0, TestName = "PEV E2 ConstantSpeed 30km/h SoC: 0.5, DH 5%"),
			TestCase(50, 0.5, -5, 0, TestName = "PEV E2 ConstantSpeed 50km/h SoC: 0.5, DH 5%"),
			//TestCase(80, 0.5, -5, 0, TestName = "PEV E2 ConstantSpeed 80km/h SoC: 0.5, DH 5%"),

			TestCase(30, 0.25, 0, 1000, TestName = "PEV E2 ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 1kW"),
			TestCase(30, 0.25, 0, 5000, TestName = "PEV E2 ConstantSpeed 30km/h SoC: 0.25, level P_auxEl: 5kW"),
		] // the vehicle can drive max. 56km/h!! 80km/h testcase makes no sense
		public void B2PEVConstantSpeed(double vmax, double initialSoC, double slope, double pAuxEl)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B2_constant_{vmax}-{initialSoC}_{slope}_{pAuxEl}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 2, largeMotor: true, pAuxEl: pAuxEl);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B2 });
			graphWriter.Write(modFilename + ".vmod");
		}


		[
			TestCase(30, 0.7, 0, TestName = "PEV E2 Halt 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "PEV E2 Halt 80km/h SoC: 0.7, level"),
			TestCase(30, 0.25, 0, TestName = "PEV E2 Halt 30km/h SoC: 0.25, level"),
			TestCase(80, 0.5, -5, TestName = "PEV E2 Halt 80km/h SoC: 0.5, DH 5%"),
		]
		public void B2PEVStop(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				   699, {0}, {1},    0
				   700,   0, {1},    3", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B2_stop_{vmax}-{initialSoC}_{slope}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 2, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B2 });
			graphWriter.Write(modFilename + ".vmod");
		}

		[
			TestCase(30, 0.7, 0, TestName = "PEV E2 DriveOff 30km/h SoC: 0.7, level"),
			TestCase(80, 0.7, 0, TestName = "PEV E2 DriveOff 80km/h SoC: 0.7, level"),
			TestCase(30, 0.25, 0, TestName = "PEV E2 DriveOff 30km/h SoC: 0.25, level")
		]
		public void B2PEVDriveOff(double vmax, double initialSoC, double slope)
		{
			var cycleData = string.Format(
				@"   0,   0, {1},    3
				   700, {0}, {1},    0", vmax, slope);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B2_acc_{vmax}-{initialSoC}_{slope}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 2, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B2 });
			graphWriter.Write(modFilename + ".vmod");
		}

		[
			TestCase("LongHaul", 2000, 0.8, 0, TestName = "PEV E2 DriveCycle LongHaul, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("RegionalDelivery", 2000, 0.8, 0, TestName = "PEV E2 DriveCycle RegionalDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("UrbanDelivery", 2000, 0.8, 0, TestName = "PEV E2 DriveCycle UrbanDelivery, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Construction", 2000, 0.8, 0, TestName = "PEV E2 DriveCycle Construction, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Urban", 2000, 0.8, 0, TestName = "PEV E2 DriveCycle Urban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Suburban", 2000, 0.8, 0, TestName = "PEV E2 DriveCycle SubUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Interurban", 2000, 0.8, 0, TestName = "PEV E2 DriveCycle InterUrban, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
			TestCase("Coach", 2000, 0.8, 0, TestName = "PEV E2 DriveCycle Coach, SoC: 0.8 Payload: 2t P_auxEl: 0kW"),
		]
		public void B2PEVDriveCycle(string declarationMission, double payload, double initialSoC, double pAuxEl)
		{
			var cycleData = RessourceHelper.ReadStream(
				DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
				declarationMission +
				Constants.FileExtensions.CycleFile);
			var cycle = DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.DistanceBased, "", false);

			var modFilename = $"SimpleBatteryElectric-B2_cycle_{declarationMission}-{initialSoC}_{payload}_{pAuxEl}";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE2;
			var job = CreateEngineeringRun(
				cycle, modFilename, initialSoC, pos, 2, 2, largeMotor: true);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			//run.Run();
			job.Execute();
			job.WaitFinished();
			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);

			var graphWriter = GetGraphWriter(new[] { ModalResultField.P_electricMotor_mech_B2 });
			graphWriter.Write(modFilename + ".vmod");
		}

        [
			TestCase(BEV_E2_Job, 0, TestName = "PEV E2 Job LongHaul"),
			TestCase(BEV_E2_Job, 1, TestName = "PEV E2 Job Coach"),
			TestCase(BEV_E2_Job, 2, TestName = "PEV E2 Job Construction"),
			TestCase(BEV_E2_Job, 3, TestName = "PEV E2 Job HeavyUrban"),
			TestCase(BEV_E2_Job, 4, TestName = "PEV E2 Job Interurban"),
			TestCase(BEV_E2_Job, 5, TestName = "PEV E2 Job MunicipalUtility"),
			TestCase(BEV_E2_Job, 6, TestName = "PEV E2 Job RegionalDelivery"),
			TestCase(BEV_E2_Job, 7, TestName = "PEV E2 Job Suburban"),
			TestCase(BEV_E2_Job, 8, TestName = "PEV E2 Job Urban"),
			TestCase(BEV_E2_Job, 9, TestName = "PEV E2 Job UrbanDelivery"),

			TestCase(BEV_E2_Job_3Speed, 0, TestName = "PEV E2 3Speed Job LongHaul"),
			TestCase(BEV_E2_Job_3Speed, 1, TestName = "PEV E2 3Speed Job Coach"),
			TestCase(BEV_E2_Job_3Speed, 2, TestName = "PEV E2 3Speed Job Construction"),
			TestCase(BEV_E2_Job_3Speed, 3, TestName = "PEV E2 3Speed Job HeavyUrban"),
			TestCase(BEV_E2_Job_3Speed, 4, TestName = "PEV E2 3Speed Job Interurban"),
			TestCase(BEV_E2_Job_3Speed, 5, TestName = "PEV E2 3Speed Job MunicipalUtility"),
			TestCase(BEV_E2_Job_3Speed, 6, TestName = "PEV E2 3Speed Job RegionalDelivery"),
			TestCase(BEV_E2_Job_3Speed, 7, TestName = "PEV E2 3Speed Job Suburban"),
			TestCase(BEV_E2_Job_3Speed, 8, TestName = "PEV E2 3Speed Job Urban"),
			TestCase(BEV_E2_Job_3Speed, 9, TestName = "PEV E2 3Speed Job UrbanDelivery"),

			TestCase(BEV_E2_Job_BusAux, 0, TestName = "PEV E2 BusAux Job LongHaul"),
			TestCase(BEV_E2_Job_BusAux, 1, TestName = "PEV E2 BusAux Job Coach"),
			TestCase(BEV_E2_Job_BusAux, 2, TestName = "PEV E2 BusAux Job Construction"),
			TestCase(BEV_E2_Job_BusAux, 3, TestName = "PEV E2 BusAux Job HeavyUrban"),
			TestCase(BEV_E2_Job_BusAux, 4, TestName = "PEV E2 BusAux Job Interurban"),
			TestCase(BEV_E2_Job_BusAux, 5, TestName = "PEV E2 BusAux Job MunicipalUtility"),
			TestCase(BEV_E2_Job_BusAux, 6, TestName = "PEV E2 BusAux Job RegionalDelivery"),
			TestCase(BEV_E2_Job_BusAux, 7, TestName = "PEV E2 BusAux Job Suburban"),
			TestCase(BEV_E2_Job_BusAux, 8, TestName = "PEV E2 BusAux Job Urban"),
			TestCase(BEV_E2_Job_BusAux, 9, TestName = "PEV E2 BusAux Job UrbanDelivery"),

			TestCase(BEV_E2_Job_Cont30kW, 0, TestName = "PEV E2 Cont. 30kW Job LongHaul"),
			TestCase(BEV_E2_Job_Cont30kW, 1, TestName = "PEV E2 Cont. 30kW Job Coach"),
			TestCase(BEV_E2_Job_Cont30kW, 2, TestName = "PEV E2 Cont. 30kW Job Construction"),
			TestCase(BEV_E2_Job_Cont30kW, 3, TestName = "PEV E2 Cont. 30kW Job HeavyUrban"),
			TestCase(BEV_E2_Job_Cont30kW, 4, TestName = "PEV E2 Cont. 30kW Job Interurban"),
			TestCase(BEV_E2_Job_Cont30kW, 5, TestName = "PEV E2 Cont. 30kW Job MunicipalUtility"),
			TestCase(BEV_E2_Job_Cont30kW, 6, TestName = "PEV E2 Cont. 30kW Job RegionalDelivery"),
			TestCase(BEV_E2_Job_Cont30kW, 7, TestName = "PEV E2 Cont. 30kW Job Suburban"),
			TestCase(BEV_E2_Job_Cont30kW, 8, TestName = "PEV E2 Cont. 30kW Job Urban"),
			TestCase(BEV_E2_Job_Cont30kW, 9, TestName = "PEV E2 Cont. 30kW Job UrbanDelivery"),
			//TestCase(BEV_Job_Cont30kW, 0, TestName = "PEV E2 Job Cont. 80kW RD")

			TestCase(BEV_E2_APTN_Job, 0, TestName = "PEV E2 APT-N Job LongHaul"),
			TestCase(BEV_E2_APTN_Job, 1, TestName = "PEV E2 APT-N Job RegionalDelivery"),
			TestCase(BEV_E2_APTN_Job, 2, TestName = "PEV E2 APT-N Job UrbanDelivery"),
			TestCase(BEV_E2_APTN_Job, 3, TestName = "PEV E2 APT-N Job Construction"),
			TestCase(BEV_E2_APTN_Job, 4, TestName = "PEV E2 APT-N Job Urban"),
			TestCase(BEV_E2_APTN_Job, 5, TestName = "PEV E2 APT-N Job Suburban"),
			TestCase(BEV_E2_APTN_Job, 6, TestName = "PEV E2 APT-N Job Interurban"),
			TestCase(BEV_E2_APTN_Job, 7, TestName = "PEV E2 APT-N Job Coach"),

			TestCase(BEV_E2_APTS_Job, 0, TestName = "PEV E2 APT-S Job LongHaul"),
			TestCase(BEV_E2_APTS_Job, 1, TestName = "PEV E2 APT-S Job RegionalDelivery"),
			TestCase(BEV_E2_APTS_Job, 2, TestName = "PEV E2 APT-S Job UrbanDelivery"),
			TestCase(BEV_E2_APTS_Job, 3, TestName = "PEV E2 APT-S Job Construction"),
			TestCase(BEV_E2_APTS_Job, 4, TestName = "PEV E2 APT-S Job Urban"),
			TestCase(BEV_E2_APTS_Job, 5, TestName = "PEV E2 APT-S Job Suburban"),
			TestCase(BEV_E2_APTS_Job, 6, TestName = "PEV E2 APT-S Job Interurban"),
			TestCase(BEV_E2_APTS_Job, 7, TestName = "PEV E2 APT-S Job Coach"),

			TestCase(BEV_E2_APTP_Job, 0, TestName = "PEV E2 APT-P Job LongHaul"),
			TestCase(BEV_E2_APTP_Job, 1, TestName = "PEV E2 APT-P Job RegionalDelivery"),
			TestCase(BEV_E2_APTP_Job, 2, TestName = "PEV E2 APT-P Job UrbanDelivery"),
			TestCase(BEV_E2_APTP_Job, 3, TestName = "PEV E2 APT-P Job Construction"),
			TestCase(BEV_E2_APTP_Job, 4, TestName = "PEV E2 APT-P Job Urban"),
			TestCase(BEV_E2_APTP_Job, 5, TestName = "PEV E2 APT-P Job Suburban"),
			TestCase(BEV_E2_APTP_Job, 6, TestName = "PEV E2 APT-P Job Interurban"),
			TestCase(BEV_E2_APTP_Job, 7, TestName = "PEV E2 APT-P Job Coach"),

			TestCase(BEV_E2_3Speed_PTO_Job, 0, TestName = "PEV E2 3speed PTO Job LongHaul"),
			TestCase(BEV_E2_3Speed_PTO_Job, 1, TestName = "PEV E2 3speed PTO Job Coach"),
			TestCase(BEV_E2_3Speed_PTO_Job, 2, TestName = "PEV E2 3speed PTO Job Construction"),
			TestCase(BEV_E2_3Speed_PTO_Job, 3, TestName = "PEV E2 3speed PTO Job HeavyUrban"),
			TestCase(BEV_E2_3Speed_PTO_Job, 4, TestName = "PEV E2 3speed PTO Job Interurban"),
			TestCase(BEV_E2_3Speed_PTO_Job, 5, TestName = "PEV E2 3speed PTO Job MunicipalUtility"),
			TestCase(BEV_E2_3Speed_PTO_Job, 6, TestName = "PEV E2 3speed PTO Job RegionalDelivery"),
			TestCase(BEV_E2_3Speed_PTO_Job, 7, TestName = "PEV E2 3speed PTO Job Suburban"),
			TestCase(BEV_E2_3Speed_PTO_Job, 8, TestName = "PEV E2 3speed PTO Job Urban"),
			TestCase(BEV_E2_3Speed_PTO_Job, 9, TestName = "PEV E2 3speed PTO Job UrbanDelivery"),

			TestCase(BEV_E2_Job_Cont30kW, 9, TestName = "PEV E2 Cont. 30kW Job UrbanDelivery"),

			TestCase(BEV_E2_Job_TqLimit, 0, TestName = "PEV E2 Job 3speed Gbx TqLimit LongHaul"),
			TestCase(BEV_E2_Job_TqLimit, 1, TestName = "PEV E2 Job 3speed Gbx TqLimit Coach"),
			TestCase(BEV_E2_Job_TqLimit, 2, TestName = "PEV E2 Job 3speed Gbx TqLimit Construction"),
			TestCase(BEV_E2_Job_TqLimit, 3, TestName = "PEV E2 Job 3speed Gbx TqLimit HeavyUrban"),
			TestCase(BEV_E2_Job_TqLimit, 4, TestName = "PEV E2 Job 3speed Gbx TqLimit Interurban"),
			TestCase(BEV_E2_Job_TqLimit, 5, TestName = "PEV E2 Job 3speed Gbx TqLimit MunicipalUtility"),
			TestCase(BEV_E2_Job_TqLimit, 6, TestName = "PEV E2 Job 3speed Gbx TqLimit RegionalDelivery"),
			TestCase(BEV_E2_Job_TqLimit, 7, TestName = "PEV E2 Job 3speed Gbx TqLimit Suburban"),
			TestCase(BEV_E2_Job_TqLimit, 8, TestName = "PEV E2 Job 3speed Gbx TqLimit Urban"),
			TestCase(BEV_E2_Job_TqLimit, 9, TestName = "PEV E2 Job 3speed Gbx TqLimit UrbanDelivery"),

			TestCase(BEV_E2_Job_SpdLimit, 0, TestName = "PEV E2 Job 3speed Gbx SpeedLimit LongHaul"),
			TestCase(BEV_E2_Job_SpdLimit, 1, TestName = "PEV E2 Job 3speed Gbx SpeedLimit Coach"),
			TestCase(BEV_E2_Job_SpdLimit, 2, TestName = "PEV E2 Job 3speed Gbx SpeedLimit Construction"),
			TestCase(BEV_E2_Job_SpdLimit, 3, TestName = "PEV E2 Job 3speed Gbx SpeedLimit HeavyUrban"),
			TestCase(BEV_E2_Job_SpdLimit, 4, TestName = "PEV E2 Job 3speed Gbx SpeedLimit Interurban"),
			TestCase(BEV_E2_Job_SpdLimit, 5, TestName = "PEV E2 Job 3speed Gbx SpeedLimit MunicipalUtility"),
			TestCase(BEV_E2_Job_SpdLimit, 6, TestName = "PEV E2 Job 3speed Gbx SpeedLimit RegionalDelivery"),
			TestCase(BEV_E2_Job_SpdLimit, 7, TestName = "PEV E2 Job 3speed Gbx SpeedLimit Suburban"),
			TestCase(BEV_E2_Job_SpdLimit, 8, TestName = "PEV E2 Job 3speed Gbx SpeedLimit Urban"),
			TestCase(BEV_E2_Job_SpdLimit, 9, TestName = "PEV E2 Job 3speed Gbx SpeedLimit UrbanDelivery"),

			TestCase(BEV_E2_Job_SpdTqLimit, 0, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit LongHaul"),
			TestCase(BEV_E2_Job_SpdTqLimit, 1, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit Coach"),
			TestCase(BEV_E2_Job_SpdTqLimit, 2, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit Construction"),
			TestCase(BEV_E2_Job_SpdTqLimit, 3, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit HeavyUrban"),
			TestCase(BEV_E2_Job_SpdTqLimit, 4, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit Interurban"),
			TestCase(BEV_E2_Job_SpdTqLimit, 5, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit MunicipalUtility"),
			TestCase(BEV_E2_Job_SpdTqLimit, 6, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit RegionalDelivery"),
			TestCase(BEV_E2_Job_SpdTqLimit, 7, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit Suburban"),
			TestCase(BEV_E2_Job_SpdTqLimit, 8, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit Urban"),
			TestCase(BEV_E2_Job_SpdTqLimit, 9, TestName = "PEV E2 3speed Job Gbx SpeedTqLimit UrbanDelivery"),
		]
		public void B2PEVRunJob(string jobFile, int cycleIdx)
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

		[TestCase]
		public void Run_E3_AxlegearInputRetarder()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData("0, 80, 0, 0\n500, 80, 0, 0");
			var job = CreateEngineeringRun(cycle, $"{MethodBase.GetCurrentMethod()}.vmod", 0.5,
				PowertrainPosition.BatteryElectricE3, 2, 2, largeMotor: true, retarderType: RetarderType.AxlegearInputRetarder);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);
			Assert.That(modData.Columns.Contains(ModalResultField.P_ret_loss.GetName()));
			Assert.That(modData.Columns.Contains(ModalResultField.P_retarder_in.GetName()));
			Assert.That(modData.Sum(r => r.Field<Watt>(ModalResultField.P_ret_loss.GetName()).Value()), Is.GreaterThan(0));
			Assert.That(modData.Sum(r => r.Field<Watt>(ModalResultField.P_retarder_in.GetName()).Value()), Is.GreaterThan(0));
		}

		[TestCase]
		public void Run_E3_WithoutAxlegearInputRetarder()
		{
			var cycleData = string.Format(
				@"   0, {0}, {1},    0
				  7000, {0}, {1},    0", 80, 0);
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);

			var modFilename = $"SimpleBatteryElectric-B3_constant";
			const PowertrainPosition pos = PowertrainPosition.BatteryElectricE3;
			var job = CreateEngineeringRun(
				cycle, modFilename, 0.5, pos, 2, 22.6, largeMotor: true, pAuxEl: 0);
			var run = job.Runs.First().Run;
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			var data = run.GetContainer().RunData;
			//File.WriteAllText(
			//	$"{modFilename}.json",
			//	JsonConvert.SerializeObject(data, Formatting.Indented));

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			Assert.IsTrue(modData.Rows.Count > 0);



			//var cycle = SimpleDrivingCycles.CreateCycleData("0, 80, 0, 0\n7000, 80, 0, 0");
			//var job = CreateEngineeringRun(cycle, $"{MethodBase.GetCurrentMethod()}.vmod", 0.5,
			//	PowertrainPosition.BatteryElectricE3, 2, 2, largeMotor: true, retarderType: RetarderType.None, pAuxEl:0);
			//var run = job.Runs.First().Run;
			//var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			//run.Run();

			//Assert.IsTrue(run.FinishedWithoutErrors);
			//Assert.IsTrue(modData.Rows.Count > 0);
			//Assert.That(modData.Columns.Contains(ModalResultField.P_ret_loss.GetName()));
			//Assert.That(modData.Columns.Contains(ModalResultField.P_retarder_in.GetName()));
			//Assert.That(modData.Rows.Cast<DataRow>().All(r => r.Field<Watt>(ModalResultField.P_ret_loss.GetName()) is null));
			//Assert.That(modData.Rows.Cast<DataRow>().All(r => r.Field<Watt>(ModalResultField.P_retarder_in.GetName()) is null));
		}

		[TestCase]
		public void RunJob_E3_AxlegearInputRetarder()
		{
			var jobFile = @"TestData/Components/Retarder/E3/E3WithAxlegearInputRetarder.vecto";
			var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var writer = new FileOutputWriter(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.Validate = false;
			factory.WriteModalResults = true;
			factory.SumData = new SummaryDataContainer(writer);
			var run = factory.SimulationRuns().ToArray()[0];
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);
			Assert.That(modData.Columns.Contains(ModalResultField.P_ret_loss.GetName()));
			Assert.That(modData.Columns.Contains(ModalResultField.P_retarder_in.GetName()));
			Assert.That(modData.Sum(r => r.Field<Watt>(ModalResultField.P_ret_loss.GetName()).Value()), Is.GreaterThan(0));
			Assert.That(modData.Sum(r => r.Field<Watt>(ModalResultField.P_retarder_in.GetName()).Value()), Is.GreaterThan(0));
		}

		[TestCase]
		public void RunJob_E3_NoAxlegearInputRetarder()
		{
			var jobFile = @"TestData/Components/Retarder/E3/E3WithoutAxlegearInputRetarder.vecto";
			var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
			var writer = new FileOutputWriter(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.Validate = false;
			factory.WriteModalResults = true;
			factory.SumData = new SummaryDataContainer(writer);
			var run = factory.SimulationRuns().First();
			var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.IsTrue(modData.Rows.Count > 0);
            Assert.IsFalse(modData.Columns.Contains(ModalResultField.P_ret_loss.GetName()));
            Assert.IsFalse(modData.Columns.Contains(ModalResultField.P_retarder_in.GetName()));
            //Assert.That(modData.Rows.Cast<DataRow>().All(r => r.Field<Watt>(ModalResultField.P_ret_loss.GetName()) is null));
            //Assert.That(modData.Rows.Cast<DataRow>().All(r => r.Field<Watt>(ModalResultField.P_retarder_in.GetName()) is null));
        }



		// =================================================

		public static JobContainer CreateEngineeringRun(DrivingCycleData cycleData, string modFileName, double initialSoc, 
			PowertrainPosition pos, int count, double ratio, bool largeMotor = false, double pAuxEl = 0, Kilogram payload = null,
			RetarderType retarderType = RetarderType.None)
		{
			var fileWriter = new FileOutputWriter(Path.GetFileNameWithoutExtension(modFileName));
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			var container = CreateBatteryElectricPowerTrain(cycleData, modFileName, fileWriter, sumData, initialSoc, 
				count, ratio, largeMotor, pAuxEl, pos, payload, retarderType);

			var run = new DistanceRun(container);
			jobContainer.AddRun(run);
			return jobContainer;
		}

		public static IVehicleContainer CreateBatteryElectricPowerTrain(DrivingCycleData cycleData, string modFileName, 
			FileOutputWriter fileWriter, SummaryDataContainer sumData, double initialBatCharge, int count, double ratio, 
			bool largeMotor, double pAuxEl, PowertrainPosition pos, Kilogram payload = null, RetarderType retarderType = RetarderType.None)
		{
			var gearboxData = CreateGearboxData_2Speed();
			var axleGearData = CreateAxleGearData();

			var vehicleData = CreateVehicleData(payload ?? 3300.SI<Kilogram>());
			var airdragData = CreateAirdragData();
			var driverData = CreateDriverData(AccelerationFile, true);

			var electricMotorData = MockSimulationDataFactory.CreateElectricMotorData(MotorFile, count, pos, 
				ratio / (pos == PowertrainPosition.BatteryElectricE3 ? 2.59 : 1.0), 0.97);

			var batteryData = MockSimulationDataFactory.CreateBatteryData(BatFile, initialBatCharge);

			//var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(
			//Truck40tPowerTrain.EngineFile, gearboxData.Gears.Count);

			var retarderLossMapEntries = new RetarderLossMap.RetarderLossEntry[] {
				new RetarderLossMap.RetarderLossEntry(){ RetarderSpeed = 0.RPMtoRad(), TorqueLoss = 10.SI<NewtonMeter>()},
				new RetarderLossMap.RetarderLossEntry(){ RetarderSpeed = 1000.RPMtoRad(), TorqueLoss = 12.SI<NewtonMeter>()},
				new RetarderLossMap.RetarderLossEntry(){ RetarderSpeed = 2000.RPMtoRad(), TorqueLoss = 18.SI<NewtonMeter>()},
				new RetarderLossMap.RetarderLossEntry(){ RetarderSpeed = 2300.RPMtoRad(), TorqueLoss = 20.58.SI<NewtonMeter>()},
			};
			var retarderData = new RetarderData { Type = retarderType, LossMap = new RetarderLossMap(retarderLossMapEntries), Ratio = 1 };
			
			var runData = new VectoRunData() {
				JobRunId = 0,
				JobType = VectoSimulationJobType.BatteryElectricVehicle,
				SimulationType = SimulationType.DistanceCycle,
				DriverData = driverData,
				//AxleGearData = axleGearData,
				//GearboxData = gearboxData,
				VehicleData = vehicleData,
				AirdragData = airdragData,
				JobName = modFileName,
				Cycle = cycleData,
				Retarder = retarderData,
				Aux = new List<VectoRunData.AuxData>(),
				ElectricMachinesData = electricMotorData,
				//EngineData = engineData,
				BatteryData = batteryData,
				GearshiftParameters = CreateGearshiftData(gearboxData, axleGearData.AxleGear.Ratio),
				ElectricAuxDemand = pAuxEl.SI<Watt>(),
				ExecutionMode = ExecutionMode.Engineering,
			};

			var modDataFilter = new IModalDataFilter[] { }; //new IModalDataFilter[] { new ActualModalDataFilter(), };
			var modData = new ModalDataContainer(runData, fileWriter, null, modDataFilter) {
				WriteModalResults = true,
			};
			if (pos == PowertrainPosition.BatteryElectricE3) {
				runData.AxleGearData = axleGearData;
			}

			if (pos == PowertrainPosition.BatteryElectricE2) {
				runData.AxleGearData = axleGearData;
				runData.GearboxData = gearboxData;
			}

			var container = VehicleContainer.CreateVehicleContainer(runData, modData, sumData);



			var es = new ElectricSystem(container, runData.BatteryData);
			var battery = new BatterySystem(container, batteryData);
			battery.Initialize(initialBatCharge);

			var ctl = new BatteryElectricMotorController(container, es);

			es.Connect(battery);

			var cycle = new DistanceBasedDrivingCycle(container, cycleData);

			var aux = new HighVoltageElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", pAuxEl.SI<Watt>());
			es.Connect(aux);

			var powertrain = cycle
				.AddComponent(new Driver(container, runData.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, runData.VehicleData, runData.AirdragData))
				.AddComponent(new VectoCore.Models.SimulationComponent.Impl.Wheels(container, runData.VehicleData.DynamicTyreRadius,
					runData.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			switch (pos) {
				case PowertrainPosition.HybridPositionNotSet:
					throw new VectoException("invalid powertrain position");
				case PowertrainPosition.HybridP0:
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
					throw new VectoException("testcase does not support parallel powertrain configurations");
				case PowertrainPosition.BatteryElectricE4:
					powertrain.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE4, runData.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container);
					//new MockEngineInfo(container);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE3:
					powertrain
						.AddComponent(new AxleGear(container, runData.AxleGearData))
						.AddComponent(runData.Retarder.Type == RetarderType.AxlegearInputRetarder ? new Retarder(container, runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE3, runData.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container);
					//new MockEngineInfo(container);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE2:
					var strategy = new PEVAMTShiftStrategyPolygonCreator(runData.GearshiftParameters);

					foreach (var entry in gearboxData.Gears) {
						entry.Value.ShiftPolygon = strategy.ComputeDeclarationShiftPolygon(GearboxType.AMT,
							(int)entry.Key, null, new TransmissionInputData().Repeat(gearboxData.Gears.Count + 1).Cast<ITransmissionInputData>().ToList(), null, axleGearData.AxleGear.Ratio,
							vehicleData.DynamicTyreRadius, electricMotorData.First().Item2);
					}

					powertrain
						.AddComponent(new AxleGear(container, runData.AxleGearData))
						.AddComponent(runData.AngledriveData != null ? new Angledrive(container, runData.AngledriveData) : null)
						.AddComponent(runData.Retarder.Type == RetarderType.TransmissionOutputRetarder ? new Retarder(container, runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
						.AddComponent(new PEVGearbox(container, new PEVAMTShiftStrategy(container)))
						.AddComponent(runData.Retarder.Type == RetarderType.TransmissionInputRetarder ? new Retarder(container, runData.Retarder.LossMap, runData.Retarder.Ratio) : null)
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE2, runData.ElectricMachinesData, container, es, ctl));
					new ATClutchInfo(container);
					break;
				//throw new VectoException("Battery Electric configuration B2 currently not supported");
				default: throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
			}

			return container;
		}

		private static IElectricMotor GetElectricMachine(PowertrainPosition pos,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, IVehicleContainer container,
			IElectricSystem es, IElectricMotorControl ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null) {
				return null;
			}

			//container.ModData.AddElectricMotor(pos);
			//ctl.AddElectricMotor(pos, motorData.Item2);
			var motor = new ElectricMotor(container, motorData.Item2, ctl, pos);
			motor.Connect(es);
			return motor;
		}

		private static GearboxData CreateGearboxData_2Speed()
		{
			var ratios = new[] { 3.86, 1.93 };

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
							//ShiftPolygon = shiftStrategy.ComputeDeclarationShiftPolygon(GearboxType.AMT, i, null, )
						})).ToDictionary(k => k.Item1 + 1, v => v.Item2),

				Inertia = 0.SI<KilogramSquareMeter>(),
				TractionInterruption = 1.SI<Second>(),
				InputData = new DummyGearboxData() {
					Gears = new List<ITransmissionInputData>() {
						new TransmissionInputData(),
						new TransmissionInputData(),
					}
				}
			};
		}

		private static AxleGearData CreateAxleGearData()
		{
			var ratio = 2.59;
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
			var wheels = DeclarationData.Wheels.Lookup("275/70 R22.5");
			var inputdata = new Mock<IVehicleDeclarationInputData>();
			inputdata.Setup(v => v.VehicleType).Returns(VectoSimulationJobType.BatteryElectricVehicle);
			return new VehicleData {
				AirDensity = DeclarationData.AirDensity,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				CurbMass = 11500.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = wheels.DynamicTyreRadius,
				AxleData = axles,
				SavedInDeclarationMode = false,
				InputData = inputdata.Object,
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
				},
			};
		}

		public static ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio/*, PerSecond engineIdlingSpeed*/)
		{
			var retVal = new ShiftStrategyParameters {
				TorqueReserve = 0.2,
				StartTorqueReserve = 0.2,
				DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
				UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
				UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
				StartSpeed = 2.SI<MeterPerSecond>(),
				//StartAcceleration = 0.6.SI<MeterPerSquareSecond>(),
				TimeBetweenGearshifts = 2.SI<Second>(),

				StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
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
				//ShiftSpeedsTCToLocked = DeclarationData.GearboxTCU.ShiftSpeedsTCToLocked
				//.Select(x => x.Select(y => y + engineIdlingSpeed.AsRPM).ToArray()).ToArray(),
			};

			return retVal;
		}
	}

	public class MockEngineInfo : VectoSimulationComponent, IEngineInfo
	{
		public MockEngineInfo(IVehicleContainer container) : base(container)
		{
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{ }

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{ }

		protected override bool DoUpdateFrom(object other) => false;

		public PerSecond EngineSpeed
		{
			get;
			set;
		}
		public NewtonMeter EngineTorque => null;

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			throw new NotImplementedException();
		}

		public Watt EngineDynamicFullLoadPower(PerSecond avgEngineSpeed, Second dt)
		{
			throw new NotImplementedException();
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			throw new NotImplementedException();
		}

		public Watt EngineAuxDemand(PerSecond avgEngineSpeed, Second dt)
		{
			throw new NotImplementedException();
		}

		public PerSecond EngineIdleSpeed { get; }
		public PerSecond EngineRatedSpeed { get; }
		public PerSecond EngineN95hSpeed { get; }
		public PerSecond EngineN80hSpeed { get; }
		public bool EngineOn => true;
	}


}
