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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using System.IO;

namespace TUGraz.VectoCore.Tests.Integration.BusAuxiliaries
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class AuxDemandTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[Test]
		[TestCase(12000, 1256, 148, 148, 6086.9321)]
		[TestCase(12000, 1256, -15, -50, 8954.1396)]
		[TestCase(15700, 1319, -35.79263, -144.0441, 9093.9473)]
		public void AuxDemandtest(double vehicleWeight, double engineSpeedRpm, double driveLinePower, double internalPower,
			double expectedPowerDemand)
		{
			MockDriver driver;
			var busAux = CreateBusAuxAdapterForTesting(vehicleWeight, out driver);

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
				(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-2);
		}

		[Test]
		public void AuxFCConsumptionTest()
		{
			var driveLinePower = 148;
			var engineSpeedRpm = 1256;
			var internalPower = 148;

			MockDriver driver;
			var busAux = CreateBusAuxAdapterForTesting(12000, out driver);

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var modalData = new MockModalDataContainer();

			for (int i = 0; i < 10; i++) {
				var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
					(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);
				Assert.AreEqual(6086.9321, (torque * engineSpeed).Value(), 1e-3);
				busAux.DoWriteModalResults(modalData);
			}

		    Assert.AreEqual(79.303.SI(Unit.SI.Gramm).Value(), ((SI)modalData[ModalResultField.AA_TotalCycleFC_Grams]).Value(), 0.0001);

			engineDrivelinePower = -15000.SI<Watt>();
			internalPower = -50;

			for (int i = 0; i < 10; i++) {
				var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
					(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);
				Assert.AreEqual(8954.1396, (torque * engineSpeed).Value(), 1e-3);
				busAux.DoWriteModalResults(modalData);
			}

		    Assert.AreEqual(82.5783.SI(Unit.SI.Gramm).Value(), ((SI)modalData[ModalResultField.AA_TotalCycleFC_Grams]).Value(), 0.0001);

			engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			internalPower = 148;

			for (int i = 0; i < 10; i++) {
				var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed,
					(internalPower * 1000).SI<Watt>() / engineSpeed, engineSpeed);
				Assert.AreEqual(6086.9321, (torque * engineSpeed).Value(), 1e-3);
				busAux.DoWriteModalResults(modalData);
			}

		    Assert.AreEqual(162.4654.SI(Unit.SI.Gramm).Value(), ((SI)modalData[ModalResultField.AA_TotalCycleFC_Grams]).Value(), 0.0001);
		}

		public static BusAuxiliariesAdapter CreateBusAuxAdapterForTesting(double vehicleWeight, out MockDriver driver)
		{
			var auxFilePath = @"TestData\Integration\BusAuxiliaries\AdvAuxTest.aaux";
			var engineFLDFilePath = @"TestData\Integration\BusAuxiliaries\24t Coach.vfld";
			var engineFCMapFilePath = @"TestData\Integration\BusAuxiliaries\24t Coach.vmap";

			var vehicle = new VehicleContainer(ExecutionMode.Engineering, new MockModalDataContainer());
			var fcMap = FuelConsumptionMapReader.ReadFromFile(engineFCMapFilePath);
			var fld = FullLoadCurveReader.ReadFromFile(engineFLDFilePath);
			var modelData = new CombustionEngineData() {
				Fuels = new List<CombustionEngineFuelData>() {
					new CombustionEngineFuelData() {
						ConsumptionMap = fcMap,
					}
				},
				FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() { { 0, fld }, { 1, fld } },
				IdleSpeed = 560.SI<PerSecond>()
			};

			var engine = new CombustionEngine(vehicle, modelData);
			//new Vehicle(vehicle, new VehicleData());
			driver = new MockDriver(vehicle) { VehicleStopped = false, DriverBehavior = DrivingBehavior.Braking };
			var gbx = new MockGearbox(vehicle) { Gear = new GearshiftPosition(1) };
			var brakes = new MockBrakes(vehicle);
			var veh = new MockVehicle(vehicle) { MyVehicleSpeed = 50.KMPHtoMeterPerSecond() };
			var busAux = new BusAuxiliariesAdapter(vehicle, auxFilePath, "Coach", vehicleWeight.SI<Kilogram>(),
				fcMap, modelData.IdleSpeed);
			return busAux;
		}
	}
}
