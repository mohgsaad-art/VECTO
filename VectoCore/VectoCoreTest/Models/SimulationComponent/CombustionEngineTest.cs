/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using System.Data;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using System.IO;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation;
using DummyDriverInfo = TUGraz.VectoCore.Tests.Utils.DummyDriverInfo;
using MockDriver = TUGraz.VectoCore.Tests.Utils.MockDriver;

// ReSharper disable UnusedVariable
// ReSharper disable NotAccessedVariable
// ReSharper disable CollectionNeverQueried.Local
// ReSharper disable RedundantAssignment

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class CombustionEngineTest
	{
		protected double Tolerance = 1E-3;

		private const string CoachEngine = @"TestData/Components/24t Coach.veng";

		private const string TruckEngine = @"TestData/Components/40t_Long_Haul_Truck.veng";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestEngineHasOutPort()
		{
			var vehicle = VehicleContainer.CreateVehicleContainer(null, null, null);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine, 0);
			var engine = new CombustionEngine(vehicle, engineData);

			var port = engine.OutPort();
			Assert.IsNotNull(port);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestOutPortRequestNotFailing()
		{
			var vehicle = VehicleContainer.CreateVehicleContainer(null, null, null);
            var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine, 0);
			var engine = new CombustionEngine(vehicle, engineData);
			var gearbox = new MockGearbox(vehicle) { Gear = new GearshiftPosition(0) };

			var port = engine.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();
			var torque = 400.SI<NewtonMeter>();
			var engineSpeed = 1500.RPMtoRad();

			port.Initialize(torque, engineSpeed);
			port.Request(absTime, dt, torque, engineSpeed, false);
		}

		[TestCase]
		public void TestSimpleModalData()
		{
			var vehicle = VehicleContainer.CreateVehicleContainer(null, null, null);
            var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine, 0);
			var engine = new CombustionEngine(vehicle, engineData);
			var gearbox = new MockGearbox(vehicle) { Gear = new GearshiftPosition(0) };
			var port = engine.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var torque = 0.SI<NewtonMeter>();
			var engineSpeed = 600.RPMtoRad();
			var dataWriter = new MockModalDataContainer();

			port.Initialize(torque, engineSpeed);
			for (var i = 0; i < 21; i++) {
				port.Request(absTime, dt, torque, engineSpeed, false);
				engine.CommitSimulationStep(absTime, dt, dataWriter);
				if (i > 0) {
					dataWriter.CommitSimulationStep(absTime, dt);
				}
				absTime += dt;
			}

			engineSpeed = 644.4445.RPMtoRad();
			port.Request(absTime, dt, Formulas.PowerToTorque(2329.973.SI<Watt>(), engineSpeed), engineSpeed, false);
			engine.CommitSimulationStep(absTime, dt, dataWriter);

			AssertHelper.AreRelativeEqual(1152.40304, ((SI)dataWriter[ModalResultField.P_ice_inertia]).Value());

			dataWriter.CommitSimulationStep(absTime, dt);
			absTime += dt;

			var power = new[] { 569.3641, 4264.177 };
			for (var i = 0; i < 2; i++) {
				port.Request(absTime, dt, Formulas.PowerToTorque(power[i].SI<Watt>(), engineSpeed), engineSpeed, false);
				engine.CommitSimulationStep(absTime, dt, dataWriter);
				dataWriter.CommitSimulationStep(absTime, dt);
				absTime += dt;
			}

			engineSpeed = 869.7512.RPMtoRad();
			port.Request(absTime, dt, Formulas.PowerToTorque(7984.56.SI<Watt>(), engineSpeed), engineSpeed, false);
			engine.CommitSimulationStep(absTime, dt, dataWriter);

			Assert.AreEqual(7108.32, ((SI)dataWriter[ModalResultField.P_ice_inertia]).Value(), 0.001);
			dataWriter.CommitSimulationStep(absTime, dt);
			absTime += dt;

			engineSpeed = 644.4445.RPMtoRad();
			port.Request(absTime, dt, Formulas.PowerToTorque(1351.656.SI<Watt>(), engineSpeed), engineSpeed, false);
			engine.CommitSimulationStep(absTime, dt, dataWriter);

			Assert.AreEqual(-7108.32, ((SI)dataWriter[ModalResultField.P_ice_inertia]).Value(), 0.001);
			dataWriter.CommitSimulationStep(absTime, dt);

			VectoCSVFile.Write(@"test1.csv", dataWriter.Data, true);
		}

		[TestCase("Test1Hz", @"TestData/Components/24t Coach.veng", 1000, 50, 50,
			@"TestData/Results/EngineFullLoadJumps/EngineFLJ_1000rpm_1Hz.csv")]
		[TestCase("Test10Hz", @"TestData/Components/24t Coach.veng", 1000, 50, 50,
			@"TestData/Results/EngineFullLoadJumps/EngineFLJ_1000rpm_10Hz.csv")]
		[TestCase("TestvarHz", @"TestData/Components/24t Coach.veng", 1000, 50, 50,
			@"TestData/Results/EngineFullLoadJumps/EngineFLJ_1000rpm_varHz.csv")]
		[TestCase("Test10Hz", @"TestData/Components/24t Coach_IncPT1.veng", 1000, 50, 50,
			@"TestData/Results/EngineFullLoadJumps/EngineFLJ_1000rpm_10Hz_IncPT1.csv"),
		Category(Definitions.DUPLICATE)]
		public void TestEngineOnlyEngineFullLoadJump(string testName, string engineFile, double rpm, double initialIdleLoad,
			double finalIdleLoad, string resultFile)
		{
			var vehicleContainer = VehicleContainer.CreateVehicleContainer(null, null, null);
            var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(engineFile, 0);
			var engine = new EngineOnlyCombustionEngine(vehicleContainer, engineData);
			new EngineOnlyGearboxInfo(vehicleContainer);
			new ZeroMileageCounter(vehicleContainer);
			new DummyDriverInfo(vehicleContainer);

			var expectedResults = VectoCSVFile.Read(resultFile);

			var requestPort = engine.OutPort();

			//var modalData = new ModalDataWriter(string.Format("load_jump_{0}.csv", TestContext.DataRow["TestName"].ToString()));
			var modalData = new MockModalDataContainer();

			var idlePower = initialIdleLoad.SI<Watt>();

			var angularSpeed = rpm.RPMtoRad();

			var t = 0.SI<Second>();
			var dt = 0.1.SI<Second>();
			requestPort.Initialize(Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed);
			for (; t < 2; t += dt) {
				requestPort.Request(t, dt, Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed, false);
				engine.CommitSimulationStep(t, dt, modalData);
			}

			var i = 0;
			// dt = TimeSpan.FromSeconds(double.Parse(TestContext.DataRow["dt"].ToString(), CultureInfo.InvariantCulture));
			// dt = TimeSpan.FromSeconds(expectedResults.Rows[i].ParseDouble(0)) - t;
			var engineLoadPower = engineData.FullLoadCurves[0].FullLoadStationaryPower(angularSpeed);
			idlePower = finalIdleLoad.SI<Watt>();
			for (; t < 25; t += dt, i++) {
				dt = (expectedResults.Rows[i + 1].ParseDouble(0) - expectedResults.Rows[i].ParseDouble(0)).SI<Second>();
				if (t >= 10.SI<Second>()) {
					engineLoadPower = idlePower;
				}
				requestPort.Request(t, dt, Formulas.PowerToTorque(engineLoadPower, angularSpeed), angularSpeed, false);
				modalData[ModalResultField.time] = t;
				modalData[ModalResultField.simulationInterval] = dt;
				engine.CommitSimulationStep(t, dt, modalData);
				Assert.AreEqual(expectedResults.Rows[i].ParseDouble(0), t.Value(), 0.001, "Time");
				Assert.AreEqual(expectedResults.Rows[i].ParseDouble(1), ((SI)modalData[ModalResultField.P_ice_full]).Value(), 0.1,
					$"Load in timestep {t}");
				modalData.CommitSimulationStep();
			}
			modalData.Finish(VectoRun.Status.Success);
		}

		[TestCase("Test1Hz", @"TestData/Components/24t Coach.veng", 1000, 50, 50,
			@"TestData/Results/EngineFullLoadJumps/EngineFLJ_1000rpm_1Hz.csv")]
		[TestCase("Test10Hz", @"TestData/Components/24t Coach.veng", 1000, 50, 50,
			@"TestData/Results/EngineFullLoadJumps/EngineFLJ_1000rpm_10Hz.csv")]
		[TestCase("TestvarHz", @"TestData/Components/24t Coach.veng", 1000, 50, 50,
			@"TestData/Results/EngineFullLoadJumps/EngineFLJ_1000rpm_varHz.csv")]
		[TestCase("Test10Hz", @"TestData/Components/24t Coach_IncPT1.veng", 1000, 50, 50,
			@"TestData/Results/EngineFullLoadJumps/EngineFLJ_1000rpm_10Hz_IncPT1.csv")]
		[Category(Definitions.TESTCASE_MIGRATED)]
		public void TestEngineFullLoadJump(string testName, string engineFile, double rpm, double initialIdleLoad,
			double finalIdleLoad, string resultFile)
		{
			var vehicleContainer = VehicleContainer.CreateVehicleContainer(null, null, null);
            var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(engineFile, 0);
			var engine = new CombustionEngine(vehicleContainer, engineData);
			var gearbox = new MockGearbox(vehicleContainer) { Gear = new GearshiftPosition(0) };

			var expectedResults = VectoCSVFile.Read(resultFile);

			var requestPort = engine.OutPort();

			//var modalData = new ModalDataWriter(string.Format("load_jump_{0}.csv", TestName));
			var modalData = new MockModalDataContainer();

			var idlePower = initialIdleLoad.SI<Watt>();

			var angularSpeed = rpm.RPMtoRad();

			var t = 0.SI<Second>();
			var dt = 0.1.SI<Second>();
			requestPort.Initialize(Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed);
			for (; t < 2; t += dt) {
				requestPort.Request(t, dt, Formulas.PowerToTorque(idlePower, angularSpeed), angularSpeed, false);
				engine.CommitSimulationStep(t, dt, modalData);
			}

			var i = 0;
			// dt = TimeSpan.FromSeconds(double.Parse(TestContext.DataRow["dt"].ToString(), CultureInfo.InvariantCulture));
			// dt = TimeSpan.FromSeconds(expectedResults.Rows[i].ParseDouble(0)) - t;
			var engineLoadPower = engineData.FullLoadCurves[0].FullLoadStationaryPower(angularSpeed);
			idlePower = finalIdleLoad.SI<Watt>();
			for (; t < 25; t += dt, i++) {
				dt = (expectedResults.Rows[i + 1].ParseDouble(0) - expectedResults.Rows[i].ParseDouble(0)).SI<Second>();
				if (t >= 10.SI<Second>()) {
					engineLoadPower = idlePower;
				}
				requestPort.Request(t, dt, Formulas.PowerToTorque(engineLoadPower, angularSpeed), angularSpeed, false);
				modalData[ModalResultField.time] = t;
				modalData[ModalResultField.simulationInterval] = dt;
				engine.CommitSimulationStep(t, dt, modalData);
				Assert.AreEqual(expectedResults.Rows[i].ParseDouble(0), t.Value(), 0.001, "Time");
				Assert.AreEqual(expectedResults.Rows[i].ParseDouble(1), ((SI)modalData[ModalResultField.P_ice_full]).Value(), 0.1,
					$"Load in timestep {t}");
				modalData.CommitSimulationStep();
			}
			modalData.Finish(VectoRun.Status.Success);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void EngineIdleJump()
		{
			var dataWriter = new MockModalDataContainer();
            var container = VehicleContainer.CreateVehicleContainer(null, dataWriter, null);
            var gearbox = new MockGearbox(container);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine, 1);
			var vehicle = new MockVehicle(container);
			vehicle.MyVehicleSpeed = 0.SI<MeterPerSecond>();
			var engine = new CombustionEngine(container, engineData);
			var clutch = new Clutch(container, engineData);

			var d = new MockDriver(container);

			var aux = new EngineAuxiliary(container);
			aux.AddConstant("CONST", 5000.SI<Watt>());

			gearbox.Gear = new GearshiftPosition(1);

			//gearbox.InPort().Connect(engine.OutPort());
			gearbox.InPort().Connect(clutch.OutPort());
			clutch.InPort().Connect(engine.OutPort());
			engine.Connect(aux.Port());
			clutch.IdleController = engine.IdleController;

			//			var expectedResults = VectoCSVFile.Read(TestContext.DataRow["ResultFile"].ToString());

			var requestPort = gearbox.OutPort();

			//vehicleContainer.DataWriter = new ModalDataWriter("engine_idle_test.csv");
			
			container.AddAuxiliary("CONST");

			var torque = 1200.SI<NewtonMeter>();
			var angularVelocity = 800.RPMtoRad();

			// initialize engine...

			gearbox.Initialize(torque, angularVelocity);

			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			var response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity, false);

			container.CommitSimulationStep(absTime, dt);
			var row = dataWriter.Data.Rows.Cast<DataRow>().Last();
			Assert.AreEqual(100530.96491487339.SI<Watt>().Value(), ((SI)row[ModalResultField.P_ice_out.GetName()]).Value());
			Assert.AreEqual(105530.96491487339.SI<Watt>().Value(), ((SI)row[ModalResultField.P_ice_fcmap.GetName()]).Value());
			Assert.AreEqual(5000.SI<Watt>(), row[ModalResultField.P_aux_mech.GetName()]);
			Assert.AreEqual(800.RPMtoRad(), row[ModalResultField.n_ice_avg.GetName()]);

			absTime += dt;

			// actual test...

			gearbox.Gear = new GearshiftPosition(0);
			gearbox.SetClutch(false);
			torque = 0.SI<NewtonMeter>();

			response = (ResponseSuccess)gearbox.Request(absTime, dt, torque, angularVelocity);

			container.CommitSimulationStep(absTime, dt);
			row = dataWriter.Data.Rows.Cast<DataRow>().Last();

			Assert.AreEqual(0.SI<Watt>(), row[ModalResultField.P_ice_out.GetName()]);
			Assert.AreEqual(5000.SI<Watt>(), row[ModalResultField.P_aux_mech.GetName()]);
			Assert.AreEqual(680.RPMtoRad(), row[ModalResultField.n_ice_avg.GetName()]);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void EngineIdleControllerTestCoach()
		{
			GetVehicleContainer(CoachEngine, out var container, out var engine, out var requestPort, out var gearbox);

			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			var angularVelocity = 800.RPMtoRad();
			var torque = 100000.SI<Watt>() / angularVelocity;

			var response = (ResponseSuccess)requestPort.Initialize(torque, angularVelocity);

			response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity, false);
			Assert.AreEqual(105000, response.Engine.PowerRequest.Value(), Tolerance);
			container.CommitSimulationStep(absTime, dt);
			absTime += dt;

			var engineSpeed = new[] { 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad() };
			var enginePower = new[] { -8601.6308.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>() };

			gearbox.SetClutch(false);
			for (var i = 0; i < engineSpeed.Length; i++) {
				torque = 0.SI<NewtonMeter>();

				response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, 0.RPMtoRad(), false);
				container.CommitSimulationStep(absTime, dt);
				absTime += dt;

				Assert.AreEqual(engineSpeed[i].Value(), engine.PreviousState.EngineSpeed.Value(), Tolerance, "i: {0}", i);
				Assert.AreEqual(enginePower[i].Value(), engine.PreviousState.EnginePower.Value(), Tolerance, "i: {0}", i);
			}
		}

		/*
		 * VECTO 2.2
		| time [s] | P_eng_out [kW] | n_eng_avg [1/min] | T_eng_fcmap [Nm] | Gear [-] |
		| 59.5     | 349.981     | 1679.281  | 1990.181    | 8        |
		| 60.5     | 5           | 1679.281  | 28.43269    | 0        |
		| 61.5     | -19.47213   | 1397.271  | -133.0774   | 0        |
		| 62.5     | -18.11888   | 1064.296  | -162.5699   | 0        |
		| 63.5     | -11.11163   | 714.1923  | -148.571    | 0        |
		| 64.5     | -0.5416708  | 560       | -9.236741   | 0        |
		| 65.5     | 5           | 560       | 85.26157    | 0        |
		| 66.5     | 5           | 560       | 85.26157    | 0        |
		| 67.5     | 5           | 560       | 85.26157    | 0        |
		| 68.5     | 5           | 560       | 85.26157    | 0        |
		| 69.5     | 5           | 560       | 85.26157    | 0        |
		| 70.5     | 308.729     | 1284.139  | 2295.815    | 9        |
				*/

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void EngineIdleControllerTestTruck()
		{
			GetVehicleContainer(TruckEngine, out var container, out var engine, out var requestPort, out var gearbox);

			//var dataWriter = new ModalDataWriter("EngineIdle.vmod");
			//container.DataWriter = dataWriter;

			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			var angularVelocity = 1680.RPMtoRad();
			var torque = 345000.SI<Watt>() / angularVelocity;

			var response = (ResponseSuccess)requestPort.Initialize(torque, angularVelocity);

			response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity, false);
			Assert.AreEqual(350000, response.Engine.PowerRequest.Value(), Tolerance);
			container.CommitSimulationStep(absTime, dt);
			absTime += dt;

			var engineSpeed = new[] {
				1439.5773, 1225.5363, 1026.6696, 834.1936, 641.1360, 560, 560, 560, 560, 560, 560, 560, 560, 560, 560, 560, 560, 560,
				560, 560
			};

			var enginePower = new[] {
				-37334.1588, -27198.2777, -20280.7550, -15216.7221, -11076.6656, -500.7991, 5000, 5000, 5000, 5000, 5000, 5000, 5000,
				5000, 5000, 5000, 5000, 5000, 5000, 5000
			};

			var fld = engine.ModelData.FullLoadCurves;

			gearbox.SetClutch(false);
			var engSpeedResults = new List<dynamic>();
			for (var i = 0; i < 20; i++) {
				torque = 0.SI<NewtonMeter>();

				response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, 0.RPMtoRad(), false);

				container.CommitSimulationStep(absTime, dt);

				engSpeedResults.Add(new { absTime, engine.PreviousState.EngineSpeed, engine.PreviousState.EnginePower });
				Assert.AreEqual(engineSpeed[i], engine.PreviousState.EngineSpeed.AsRPM, Tolerance, $"entry {i}");
				Assert.AreEqual(enginePower[i], engine.PreviousState.EnginePower.Value(), Tolerance, $"entry {i}");
				absTime += dt;
			}
			//dataWriter.Finish();
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void EngineIdleControllerTest2Truck()
		{
			GetVehicleContainer(TruckEngine, out var container, out var engine, out var requestPort, out var gearbox);

			//var dataWriter = new ModalDataWriter("EngienIdle.vmod");
			//container.DataWriter = dataWriter;

			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			var angularVelocity = 95.5596.SI<PerSecond>();

			var torque = (engine.ModelData.FullLoadCurves[0].DragLoadStationaryPower(angularVelocity) - 5000.SI<Watt>()) /
						angularVelocity;

			var response = (ResponseSuccess)requestPort.Initialize(torque, angularVelocity);

			response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, angularVelocity, false);
			Assert.AreEqual(-14829.79713, response.Engine.PowerRequest.Value(), Tolerance);
			container.CommitSimulationStep(absTime, dt);
			absTime += dt;

			var engineSpeed = new[] {
				1680.RPMtoRad(), 1680.RPMtoRad(), 1467.014.RPMtoRad(), 1272.8658.RPMtoRad(), 1090.989.RPMtoRad(),
				915.3533.RPMtoRad(), 738.599.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(),
				560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(), 560.RPMtoRad(),
				560.RPMtoRad()
			};

			//var enginePower = new[] {
			//	5000.SI<Watt>(), 5000.SI<Watt>(), -32832.8834.SI<Watt>(), -25025.1308.SI<Watt>(), -19267.0360.SI<Watt>(),
			//	-14890.1962.SI<Watt>(), -11500.7991.SI<Watt>(), -8091.0577.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(),
			//	5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(),
			//	5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>(), 5000.SI<Watt>()
			//};

			gearbox.SetClutch(false);
			var engSpeedResults = new List<dynamic>();
			torque = 0.SI<NewtonMeter>();
			for (var i = 0; i < engineSpeed.Length; i++) {
				response = (ResponseSuccess)requestPort.Request(absTime, dt, torque, 0.RPMtoRad(), false);

				container.CommitSimulationStep(absTime, dt);

				engSpeedResults.Add(new { absTime, engine.PreviousState.EngineSpeed, engine.PreviousState.EnginePower });
				//Assert.AreEqual(engineSpeed[i].Value(), engine.PreviousState.EngineSpeed.Value(), Tolerance);
				//Assert.AreEqual(enginePower[i].Value(), engine.PreviousState.EnginePower.Value(), Tolerance);

				absTime += dt;
			}
			//dataWriter.Finish();
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_EngineData()
		{
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(CoachEngine, 0);
			var motorway = engineData.Fuels.First().WHTCMotorway;
			Assert.AreEqual(motorway, 1);

			var rural = engineData.Fuels.First().WHTCRural;
			Assert.AreEqual(rural, 1);

			var urban = engineData.Fuels.First().WHTCUrban;
			Assert.AreEqual(urban, 1);

			var displace = engineData.Displacement;
			Assert.AreEqual(0.01273, displace.Value());
			Assert.IsTrue(displace.HasEqualUnit(new SI(Unit.SI.Cubic.Meter)));

			var inert = engineData.Inertia;
			Assert.AreEqual(3.8, inert.Value(), 0.00001);
			Assert.IsTrue(inert.HasEqualUnit(new SI(Unit.SI.Kilo.Gramm.Square.Meter)));

			var idle = engineData.IdleSpeed;
			Assert.AreEqual(58.6430628670095, idle.Value(), 0.000001);
			Assert.IsTrue(idle.HasEqualUnit(0.SI<PerSecond>()));
		}

		
		private static void GetVehicleContainer(string engineFile, out IVehicleContainer container, out CombustionEngine engine,
			out ITnOutPort requestPort, out MockGearbox gearbox)
		{
			var dataWriter = new MockModalDataContainer();
			container = VehicleContainer.CreateVehicleContainer(null, dataWriter, null);
			gearbox = new MockGearbox(container);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(engineFile, 1);

			engine = new CombustionEngine(container, engineData);
			var clutch = new Clutch(container, engineData);

			var d = new MockDriver(container);

			var aux = new EngineAuxiliary(container);
			aux.AddConstant("CONST", 5000.SI<Watt>());

			gearbox.Gear = new GearshiftPosition(1);
			var vehicle = new MockVehicle(container);
			vehicle.MyVehicleSpeed = 0.SI<MeterPerSecond>();
			//gearbox.InPort().Connect(engine.OutPort());
			gearbox.InPort().Connect(clutch.OutPort());
			clutch.InPort().Connect(engine.OutPort());
			engine.Connect(aux.Port());

			// has to be done after connecting components!
			clutch.IdleController = engine.IdleController;

			requestPort = gearbox.OutPort();

			//vehicleContainer.DataWriter = new ModalDataWriter("engine_idle_test.csv");
			
			container.AddAuxiliary("CONST");
		}
	}
}