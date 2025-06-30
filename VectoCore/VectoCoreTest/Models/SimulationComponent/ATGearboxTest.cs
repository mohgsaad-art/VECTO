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
using NUnit.Framework;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;


namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ATGearboxTest
	{
		public const string EngineDataFile = @"TestData/Components/AT_GBX/Engine.veng";
		public const string GearboxDataFile = @"TestData/Components/AT_GBX/GearboxSerial.vgbx";

		public const string GearboxData8SpdFile = @"TestData/Components/AT_GBX/GearboxSerial8Spd.vgbx";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		//[Test,
		//TestCase(0, 100, 1),
		//TestCase(0, 200, 1),
		//TestCase(5, 100, 1),
		//TestCase(5, 300, 1),
		//TestCase(5, 600, 1),
		//TestCase(15, 100, 3),
		//TestCase(15, 300, 3),
		//TestCase(15, 600, 3),
		//TestCase(40, 100, 6),
		//TestCase(40, 300, 6),
		//TestCase(40, 600, 6),
		//TestCase(70, 100, 6),
		//TestCase(70, 300, 6),
		//TestCase(70, 600, 6),
		//	Category(Definitions.TESTCASE_MIGRATED)
		//]
		//public void TestATGearInitialize(double vehicleSpeed, double torque, int expectedGear)
		//{
		//	var gearboxData = MockSimulationDataFactory.CreateGearboxDataFromFile(GearboxDataFile, EngineDataFile, false);
		//	var vehicleContainer = new MockVehicleContainer(); //(ExecutionMode.Engineering);
		//	var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineDataFile, gearboxData.Gears.Count);
		//	vehicleContainer.EngineInfo = new CombustionEngine(vehicleContainer, engineData);
		//	var runData = new VectoRunData() {
		//		GearboxData = gearboxData,
		//		EngineData = new CombustionEngineData() { Inertia = 0.SI<KilogramSquareMeter>() }
		//	};
		//	vehicleContainer.RunData = runData;
		//	var gearbox = new ATGearbox(vehicleContainer, new ATShiftStrategy(vehicleContainer));

		//	vehicleContainer.VehicleSpeed = vehicleSpeed.KMPHtoMeterPerSecond();

		//	var tnPort = new MockTnOutPort();
		//	gearbox.Connect(tnPort);

		//	// r_dyn = 0.465m, i_axle = 6.2
		//	var angularVelocity = vehicleSpeed.KMPHtoMeterPerSecond() / 0.465.SI<Meter>() * 6.2;
		//	var response = gearbox.Initialize(torque.SI<NewtonMeter>(), angularVelocity);

		//	Assert.IsInstanceOf(typeof(ResponseSuccess), response);
		//	Assert.AreEqual(expectedGear, gearbox.Gear.Gear);
		//	Assert.AreEqual(vehicleSpeed.IsEqual(0), gearbox.Disengaged);
		//}

		//[Test,
		//TestCase(GearboxType.ATSerial, TestName = "Drive TorqueConverter - Serial"),
		//TestCase(GearboxType.ATPowerSplit, TestName = "Drive TorqueConverter - PowerSplit")]
		//public void TestATGearboxDriveTorqueConverter(GearboxType gbxType)
		//{
		//	var cycleData = @"   0,  0, 0,    2
		//						20,  8, 0,    0
		//					   200,  0, 0,    2";
		//	var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
		//	var run = ATPowerTrain.CreateEngineeringRun(
		//		cycle, gbxType, null,
		//		$"AT_Vehicle_Drive-TC-{(gbxType == GearboxType.ATSerial ? "ser" : "ps")}.vmod");

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);
		//}

		//[Test,
		//TestCase(GearboxType.ATSerial, TestName = "ShiftUp TorqueConverter - Serial"),
		//TestCase(GearboxType.ATPowerSplit, TestName = "ShiftUp TorqueConverter - PowerSplit")]
		//public void TestATGearboxShiftUp(GearboxType gbxType)
		//{
		//	var cycleData = @"  0,  0, 0,    2
		//					  500, 40, 0,    0";
		//	var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
		//	var run = ATPowerTrain.CreateEngineeringRun(
		//		cycle, gbxType, null,
		//		$"AT_Vehicle_Drive-TC_shiftup-{(gbxType == GearboxType.ATSerial ? "ser" : "ps")}.vmod");

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);
		//}

		//[Test,
		//TestCase(GearboxType.ATSerial, TestName = "ShiftDown TorqueConverter - Serial"),
		//TestCase(GearboxType.ATPowerSplit, TestName = "ShiftDown TorqueConverter - PowerSplit")]
		//public void TestATGearboxShiftDown(GearboxType gbxType)
		//{
		//	var cycleData = @"  0, 70, 0,    0
		//					  500,  0, 0,    2";
		//	var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
		//	var run = ATPowerTrain.CreateEngineeringRun(
		//		cycle, gbxType, null,
		//		$"AT_Vehicle_Drive-TC_shiftdown-{(gbxType == GearboxType.ATSerial ? "ser" : "ps")}.vmod");

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);
		//}


		//[Category("LongRunning")]
		//[Test,
		//TestCase("Urban", GearboxType.ATSerial),
		//TestCase("Suburban", GearboxType.ATSerial),
		//TestCase("Interurban", GearboxType.ATSerial),
		//TestCase("HeavyUrban", GearboxType.ATSerial),
		//TestCase("Urban", GearboxType.ATPowerSplit),
		//TestCase("Suburban", GearboxType.ATPowerSplit),
		//TestCase("Interurban", GearboxType.ATPowerSplit),
		//TestCase("HeavyUrban", GearboxType.ATPowerSplit)
		//]
		//public void TestATGearboxDriveCycle(string cycleName, GearboxType gbxType)
		//{
		//	Assert.IsTrue(gbxType.AutomaticTransmission());
		//	var cycle = SimpleDrivingCycles.ReadDeclarationCycle(cycleName);
		//	var sumWriter =
		//		new SummaryDataContainer(
		//			new FileOutputWriter(
		//				$"AT_Vehicle_Drive-TC_{cycleName}-{(gbxType == GearboxType.ATSerial ? "ser" : "ps")}"));
		//	var run = ATPowerTrain.CreateEngineeringRun(
		//		cycle, gbxType, sumWriter,
		//		$"AT_Vehicle_Drive-TC_{cycleName}-{(gbxType == GearboxType.ATSerial ? "ser" : "ps")}.vmod");

		//	run.Run();
		//	sumWriter.Finish();
		//	Assert.IsTrue(run.FinishedWithoutErrors);
		//}


		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestATGearboxLastGearDisabled()
		{
			var limits = new List<ITorqueLimitInputData>() {
				new TorqueLimitInputData() {
					Gear = 8,
					MaxTorque = 0.SI<NewtonMeter>()
				}
			};

			var gbx = GetATGearbox(GearboxData8SpdFile, limits);
			Assert.AreEqual(7, gbx.Gears.Count);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestATGearboxLastButOneGearDisabled()
		{
			var limits = new List<ITorqueLimitInputData>() {
				new TorqueLimitInputData() {
					Gear = 7,
					MaxTorque = 0.SI<NewtonMeter>()
				}
			};

			AssertHelper.Exception<VectoException>(
				() => {
					var gbx = GetATGearbox(GearboxData8SpdFile, limits);
				}, "Only the last 1 or 2 gears can be disabled. Disabling gear 7 for a 8-speed gearbox is not allowed.");
			
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestATGearboxLastTwoGearsDisabled()
		{
			var limits = new List<ITorqueLimitInputData>() {
				new TorqueLimitInputData() {
					Gear = 7,
					MaxTorque = 0.SI<NewtonMeter>()
				},
				new TorqueLimitInputData() {
					Gear = 8,
					MaxTorque = 0.SI<NewtonMeter>()
				}
			};

			var gbx = GetATGearbox(GearboxData8SpdFile, limits);
			Assert.AreEqual(6, gbx.Gears.Count);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestATGearboxFirstGearDisabled()
		{
			var limits = new List<ITorqueLimitInputData>() {
				new TorqueLimitInputData() {
					Gear = 1,
					MaxTorque = 0.SI<NewtonMeter>()
				},
				
			};

			AssertHelper.Exception<VectoException>(() => GetATGearbox(GearboxData8SpdFile, limits), messageContains: "Only the last 1 or 2 gears can be disabled.");
			//Assert.AreEqual(8, gbx.Gears.Count);
		}

		public GearboxData GetATGearbox(string gbxFile, IList<ITorqueLimitInputData> torqueLimits)
		{
			var gearboxInput = JSONInputDataFactory.ReadGearbox(gbxFile);
			var engineInput = JSONInputDataFactory.ReadEngine(EngineDataFile);

			var dao = new DeclarationDataAdapterHeavyLorry.Conventional();
			var vehicleInput = new MockDeclarationVehicleInputData() {
				EngineInputData = engineInput,
				GearboxInputData = gearboxInput
			};
			var mission = new Mission() {
				MissionType = MissionType.LongHaul
			};
			var engineData = dao.CreateEngineData(
				vehicleInput, engineInput.EngineModes.First(),
				mission); //(engineInput, null, gearboxInput, new List<ITorqueLimitInputData>());
			return dao.CreateGearboxData(
				new MockVehicleInputData() {
					Components = new MockComponents() {
						GearboxInputData = gearboxInput,
						TorqueConverterInputData = (ITorqueConverterDeclarationInputData)gearboxInput,
					},
					TorqueLimits = torqueLimits
				}, new VectoRunData() {
					EngineData = engineData,
					AxleGearData = new AxleGearData() {
						AxleGear = new TransmissionData() { Ratio = ((IAxleGearInputData)gearboxInput).Ratio },
					},
					VehicleData =
						new VehicleData() { VehicleCategory = VehicleCategory.RigidTruck, DynamicTyreRadius = 0.5.SI<Meter>() }
				}, null);
		}
	}
}
