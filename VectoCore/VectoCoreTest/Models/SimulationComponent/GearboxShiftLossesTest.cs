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

using System;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;
using System.IO;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class GearboxShiftLossesTest
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		private static AxleGearData CreateAxleGearData(GearboxType gbxType)
		{
			var ratio = gbxType == GearboxType.ATSerial ? 6.2 : 5.8;
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMapReader.Create(0.95, ratio, "Axlegear"),
				}
			};
		}

		//[Test,
		//TestCase(200, 2u, 562, 620, 19.5023, double.NaN),
		//TestCase(400, 2u, 562, 620, 39.0046, 3183.0086),
		//TestCase(600, 2u, 562, 620, 58.5069, double.NaN),
		//TestCase(800, 2u, 562, 620, 78.0092, double.NaN),
		//TestCase(200, 2u, 562, 600, 19.7908, double.NaN),
		//TestCase(400, 2u, 562, 600, 39.5816, double.NaN),
		//TestCase(600, 2u, 562, 600, 59.3723, 4774.5128),
		//TestCase(800, 2u, 562, 600, 79.1632, double.NaN),
		//TestCase(400, 3u, 500, 490, 30.7900, double.NaN),
		//TestCase(400, 3u, 550, 490, 32.4643, 2328.0855),
		//TestCase(600, 3u, 550, 490, 48.6965, double.NaN),
		//	Category(Definitions.TESTCASE_MIGRATED)
		//]
		//public void TestShiftLossComputation(double torqueDemand, uint gear, double preShiftRpm,
		//	double postShiftRpm, double expectedShiftLoss, double expectedShiftLossEnergy)
		//{
		//	var cycleDataStr = "0, 0, 0, 2\n100, 20, 0, 0\n1000, 50, 0, 0";
		//	var container = CreateVehicle(cycleDataStr, preShiftRpm, out var axleGear, out var gbx, out var engine);
		//	new ATClutchInfo(container);
		//	gbx.Gear = new GearshiftPosition(gear, true);
			
		//	var absTime = 20.SI<Second>();
		//	var dt = 0.5.SI<Second>();
		//	var response = gbx.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
		//	axleGear.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad(), false);


		//	Assert.IsInstanceOf<ResponseSuccess>(response);
		//	container.CommitSimulationStep(absTime, dt);
		//	absTime += dt;

		//	response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());
		//	Assert.IsInstanceOf<ResponseFailTimeInterval>(response);

		//	Console.WriteLine($"initial gear: {gear}, after GS: {gbx._strategy.NextGear}");
		//	dt = ((ResponseFailTimeInterval)response).DeltaT;
		//	response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());

		//	Assert.IsInstanceOf<ResponseSuccess>(response);
		//	Assert.AreEqual(expectedShiftLoss, gbx.CurrentState.PowershiftLoss.Value(), 1e-3);
		//	Assert.AreEqual(gear + (postShiftRpm > preShiftRpm ? 1 : -1), gbx.Gear.Gear);

		//	if (!double.IsNaN(expectedShiftLossEnergy)) {
		//		var modData = new MockModalDataContainer();
		//		gbx.CommitSimulationStep(absTime, dt, modData);
		//		var shiftLossE = (Watt)modData[ModalResultField.P_gbx_shift_loss] * dt;
		//		Assert.AreEqual(expectedShiftLossEnergy, shiftLossE.Value(), 1e-3);
		//	}
		//}

		//[Test,
		////TestCase(200, 2u, 562, 620, 19.5023),
		//TestCase(400, 2u, 562, 620, 39.0046, 3183.0086),
		////TestCase(600, 2u, 562, 620, 58.5069),
		////TestCase(800, 2u, 562, 620, 78.0092),
		////TestCase(200, 2u, 562, 600, 19.7908),
		////TestCase(400, 2u, 562, 600, 39.5816),
		//TestCase(600, 2u, 562, 600, 59.3723, 4774.5128),
		////TestCase(800, 2u, 562, 600, 79.1632),
		////TestCase(400, 3u, 500, 490, 30.7900),
		//TestCase(400, 3u, 550, 490, 32.4643, 2328.0855),
		////TestCase(600, 3u, 550, 490, 48.6965),
		//Category(Definitions.TESTCASE_MIGRATED)
		//]
		//public void TestSplittingShiftLossesTwoIntervals(double torqueDemand, uint gear, double preShiftRpm,
		//	double postShiftRpm, double expectedShiftLoss, double expectedShiftLossEnergy)
		//{
		//	var cycleDataStr = "0, 0, 0, 2\n100, 20, 0, 0\n1000, 50, 0, 0";
		//	var container = CreateVehicle(cycleDataStr, preShiftRpm, out var axleGear, out var gbx, out var engine);
		//	var modData = new MockModalDataContainer();
			
		//	gbx.Gear = new GearshiftPosition(gear, true);

		//	var absTime = 20.SI<Second>();
		//	var dt = 0.5.SI<Second>();
		//	var response = gbx.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
		//	axleGear.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad(), false);


		//	Assert.IsInstanceOf<ResponseSuccess>(response);
		//	container.CommitSimulationStep(absTime, dt);
		//	absTime += dt;

		//	response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());
		//	Assert.IsInstanceOf<ResponseFailTimeInterval>(response);

		//	var splitFactor = 0.75;
		//	var shiftTime = ((ResponseFailTimeInterval)response).DeltaT;
		//	dt = shiftTime * splitFactor;
		//	response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());

		//	Assert.IsInstanceOf<ResponseSuccess>(response);
		//	Assert.AreEqual(expectedShiftLoss, gbx.CurrentState.PowershiftLoss.Value(), 1e-3);
		//	Assert.AreEqual(gear + (postShiftRpm > preShiftRpm ? 1 : -1), gbx.Gear.Gear);

		//	gbx.CommitSimulationStep(absTime, dt, modData);
		//	engine.CommitSimulationStep(absTime, dt, modData);
		//	var shiftLoss1 = (Watt)modData[ModalResultField.P_gbx_shift_loss] * dt;
		//	Assert.AreEqual(expectedShiftLossEnergy * splitFactor, shiftLoss1.Value(), 1e-3);
		//	axleGear.Request(absTime, dt, 0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad(), false);

		//	absTime += dt;
		//	dt = 0.5.SI<Second>();

		//	response = gbx.Request(absTime, dt, torqueDemand.SI<NewtonMeter>(), postShiftRpm.RPMtoRad());

		//	Assert.IsInstanceOf<ResponseSuccess>(response);
		//	gbx.CommitSimulationStep(absTime, dt, modData);
		//	engine.CommitSimulationStep(absTime, dt, modData);
		//	var shiftLoss2 = (Watt)modData[ModalResultField.P_gbx_shift_loss]  *  dt;
		//	Console.WriteLine("expected shiftloss energy: {0}, sum of shift loss energy: {1} ({2} + {3})", expectedShiftLossEnergy, shiftLoss1 + shiftLoss2, shiftLoss1, shiftLoss2);
		//	Assert.AreEqual(expectedShiftLossEnergy * (1 - splitFactor), shiftLoss2.Value(), 1e-3);

		//	Assert.AreEqual(expectedShiftLossEnergy, (shiftLoss1 + shiftLoss2).Value(), 1e-3);
		//}



		//private static VehicleContainer CreateVehicle(string cycleDataStr, double preShiftRpm, out AxleGear axleGear, out ATGearbox gbx, out CombustionEngine engine)
		//{
		//	var engineInertia = 5.SI<KilogramSquareMeter>();

		//	var gearboxData = ATPowerTrain.CreateGearboxData(GearboxType.ATSerial);

		//	var runData = new VectoRunData() {
		//		GearboxData = gearboxData,
		//		GearshiftParameters = ATPowerTrain.CreateGearshiftData(),
		//		EngineData = new CombustionEngineData() { Inertia = 5.SI<KilogramSquareMeter>() }
		//	};
  //          var container = VehicleContainer.CreateVehicleContainer(runData, null, null) as VehicleContainer;
  //          gearboxData.PowershiftShiftTime = 0.8.SI<Second>();
		//	new ATClutchInfo(container);
			
		//	var cycleData = SimpleDrivingCycles.CreateCycleData(cycleDataStr);
		//	var cycle = new MockDrivingCycle(container, cycleData);

		//	axleGear = new AxleGear(container, CreateAxleGearData(GearboxType.ATSerial));
		//	axleGear.Connect(new MockComponent());

		//	var wheels = new Wheels(container, 0.5.SI<Meter>(), 9.5.SI<KilogramSquareMeter>());

		//	var vehicle = new MockVehicle(container);
		//	container.AbsTime = 100.SI<Second>();
		//	var driver = new MockDriver(container);
		//	driver.DriverAcceleration = 0.SI<MeterPerSquareSecond>();
		//	vehicle.MyVehicleSpeed = 10.KMPHtoMeterPerSecond();
		//	driver.DriverBehavior = DrivingBehavior.Driving;
		//	engine = new CombustionEngine(
		//		container,
		//		MockSimulationDataFactory.CreateEngineDataFromFile(ATPowerTrain.EngineFile, gearboxData.Gears.Count));
		//	container.EngineInfo = engine;
			
			
		//	gbx = new ATGearbox(container, new ATShiftStrategy(container));
		//	gbx.Connect(engine);
		//	gbx.IdleController = new MockIdleController();

		//	var init = gbx.Initialize(0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
		//	axleGear.Initialize(0.SI<NewtonMeter>(), preShiftRpm.RPMtoRad());
		//	return container;
		//}
	}
}