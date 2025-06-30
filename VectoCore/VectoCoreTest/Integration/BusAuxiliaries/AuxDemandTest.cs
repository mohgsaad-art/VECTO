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
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using System.IO;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using Newtonsoft.Json;
using MockDriver = TUGraz.VectoCore.Tests.Utils.MockDriver;

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

		[TestCase(12000, 1256, 148, 148, 5649.8149)]
		[TestCase(12000, 1256, -45, -30, 8516.9257)]
		[TestCase(15700, 1319, -45.79263, -24.0441, 8656.7333),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void AuxDemandtest(double vehicleWeight, double engineSpeedRpm, double driveLinePower, double internalPower,
			double expectedPowerDemand)
		{
			var busAux = CreateBusAuxAdapterForTesting(vehicleWeight, out var driver);

            var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed, engineSpeed);

			Assert.AreEqual(expectedPowerDemand, (torque * engineSpeed).Value(), 1e-2);
		}

		[TestCase,
		Category(Definitions.DUPLICATE)]
		public void AuxFCConsumptionTest()
		{
			var driveLinePower = 148;
			var engineSpeedRpm = 1256;

			var busAux = CreateBusAuxAdapterForTesting(12000, out var driver);

			var engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();
			var engineSpeed = engineSpeedRpm.RPMtoRad();
			busAux.Initialize(engineDrivelinePower / engineSpeed, engineSpeed);

			var modalData = new MockModalDataContainer();

			for (int i = 0; i < 10; i++) {
				var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed, engineSpeed);
				Assert.AreEqual(5649.81493, (torque * engineSpeed).Value(), 1e-3);
				busAux.DoWriteModalResultsICE(0.SI<Second>(), 1.SI<Second>(), modalData);
				busAux.DoCommitSimulationStep();
			}

		    //Assert.AreEqual(79.303.SI(Unit.SI.Gramm).Value(), ((SI)modalData[ModalResultField.AA_TotalCycleFC_Grams]).Value(), 0.0001);

			engineDrivelinePower = -45000.SI<Watt>();

			for (int i = 0; i < 10; i++) {
				var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed, engineSpeed);
				Assert.AreEqual(8516.92571, (torque * engineSpeed).Value(), 1e-3);
				busAux.DoWriteModalResultsICE(0.SI<Second>(), 1.SI<Second>(), modalData);
				busAux.DoCommitSimulationStep();
			}

			//Assert.AreEqual(82.5783.SI(Unit.SI.Gramm).Value(), ((SI)modalData[ModalResultField.AA_TotalCycleFC_Grams]).Value(), 0.0001);

			engineDrivelinePower = (driveLinePower * 1000).SI<Watt>();

			for (int i = 0; i < 10; i++) {
				var torque = busAux.TorqueDemand(0.SI<Second>(), 1.SI<Second>(), engineDrivelinePower / engineSpeed, engineSpeed);
				Assert.AreEqual(5649.81493, (torque * engineSpeed).Value(), 1e-3);
				busAux.DoWriteModalResultsICE(0.SI<Second>(), 1.SI<Second>(), modalData);
				busAux.DoCommitSimulationStep();
			}

			//Assert.AreEqual(162.4654.SI(Unit.SI.Gramm).Value(), ((SI)modalData[ModalResultField.AA_TotalCycleFC_Grams]).Value(), 0.0001);
		}

		public static BusAuxiliariesAdapter CreateBusAuxAdapterForTesting(double vehicleMass, out MockDriver driver)
		{
			var auxFilePath = @"TestData/Integration/BusAuxiliaries/AdvAuxTest.aaux";
			var engineFLDFilePath = @"TestData/Integration/BusAuxiliaries/24t Coach.vfld";
			var engineFCMapFilePath = @"TestData/Integration/BusAuxiliaries/24t Coach.vmap";

			
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
			var runData = new VectoRunData() {
				EngineData = modelData,
				VehicleData = new VehicleData() {
					//Length = 10.655.SI< Meter>(),
					//Width = 2.55.SI<Meter>(),
					//Height = 2.275.SI< Meter>(),
					//FloorType = FloorType.HighFloor,
					//PassengerCount = 47,
					//DoubleDecker = false,
					CurbMass = vehicleMass.SI<Kilogram>()
				}
			};
			var vehicle = VehicleContainer.CreateVehicleContainer(runData, new MockModalDataContainer(), null);
            var engine = new CombustionEngine(vehicle, modelData);
			//new Vehicle(vehicle, new VehicleData());
			driver = new MockDriver(vehicle) { VehicleStopped = false, DriverBehavior = DrivingBehavior.Braking, DrivingAction = DrivingAction.Brake };
			//driver = null;
			var gbx = new MockGearbox(vehicle) { Gear = new GearshiftPosition(1) };
			var brakes = new MockBrakes(vehicle);
			var veh = new MockVehicle(vehicle) { MyVehicleSpeed = 50.KMPHtoMeterPerSecond() };
			var auxConfig = BusAuxiliaryInputData.ReadBusAuxiliaries(auxFilePath, vehicle.RunData.VehicleData);
			var busAux = new BusAuxiliariesAdapter(vehicle, auxConfig);
			var electricStorage = auxConfig.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
				? new SimpleBattery(vehicle, auxConfig.ElectricalUserInputsConfig.ElectricStorageCapacity, auxConfig.ElectricalUserInputsConfig.StoredEnergyEfficiency)
				: (ISimpleBattery)new NoBattery(vehicle);
			busAux.ElectricStorage = electricStorage;
			var str = JsonConvert.SerializeObject(auxConfig);

            return busAux;
		}
	}
}
