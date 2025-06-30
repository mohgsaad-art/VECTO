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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.SimulationComponent;
using TUGraz.VectoCore.Tests.Utils;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using DummyDriverInfo = TUGraz.VectoCore.Tests.Utils.DummyDriverInfo;
using MockDrivingCycle = TUGraz.VectoCore.Tests.Utils.MockDrivingCycle;

// ReSharper disable ObjectCreationAsStatement

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class AuxTests
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase]
		public void AuxWriteModFileSumFile()
		{
			var hdvClass = VehicleClass.Class5;
			var mission = MissionType.LongHaul;

			var fileWriter = new FileOutputWriter("AuxWriteModFileSumFile");
			var runData = new VectoRunData() {
				JobName = "AuxWriteModFileSumFile",
				EngineData = new CombustionEngineData() {
					Fuels = new[] {new CombustionEngineFuelData {
						FuelData = FuelData.Diesel,
						ConsumptionMap = FuelConsumptionMapReader.ReadFromFile(@"TestData/Components/12t Delivery Truck.vmap"),
					}}.ToList(),
					IdleSpeed = 600.RPMtoRad(),
					RatedPowerDeclared = 300e3.SI<Watt>(),
					RatedSpeedDeclared = 2000.RPMtoRad(),
					Displacement = 7.SI(Unit.SI.Cubic.Dezi.Meter).Cast<CubicMeter>()
				},
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				Cycle = new DrivingCycleData() {
					Name = "MockCycle",
					CycleType = CycleType.DistanceBased
				},
				DriverData = new DriverData() {
					EngineStopStart = new DriverData.EngineStopStartData(),
				},
				Aux = new List<VectoRunData.AuxData>() {
					new() {
						ID = Constants.Auxiliaries.IDs.Fan,
						Technology = new[] {"Hydraulic driven - Constant displacement pump"}.ToList(),
						PowerDemandMech = DeclarationData.Fan.LookupPowerDemand(hdvClass,MissionType.LongHaul, "Hydraulic driven - Constant displacement pump")
					},
					new() {
						ID= Constants.Auxiliaries.IDs.PneumaticSystem,
						Technology = new[] {"Medium Supply 1-stage"}.ToList(),
						PowerDemandMech = DeclarationData.PneumaticSystem.Lookup(mission, "Medium Supply 1-stage").PowerDemand
					},
					new() {
						ID = Constants.Auxiliaries.IDs.SteeringPump,
						Technology = new[] {"Variable displacement mech. controlled"}.ToList(),
						PowerDemandMech = DeclarationData.SteeringPump.Lookup(MissionType.LongHaul, hdvClass,
							new[] { "Variable displacement mech. controlled" }).mechanicalPumps
					},
					new() {
						ID = Constants.Auxiliaries.IDs.ElectricSystem,
						Technology = new[] {"Hydraulic driven - Constant displacement pump"}.ToList(),
						PowerDemandMech = DeclarationData.ElectricSystem.Lookup(hdvClass, mission).PowerDemand
					},
					new() {
						ID = Constants.Auxiliaries.IDs.HeatingVentilationAirCondition,
						Technology = new[] {"Default"}.ToList(),
						PowerDemandMech = DeclarationData.HeatingVentilationAirConditioning.Lookup(mission, "Default", hdvClass).PowerDemand
					},
				},
				ExecutionMode = ExecutionMode.Engineering,
            };
			var kernel = new StandardKernel(new VectoNinjectModule());
			var modData = kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, fileWriter, null, null) as ModalDataContainer;
			Assert.NotNull(modData);
			modData.WriteModalResults = true;
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
			
			modData.Data.CreateCombustionEngineColumns(runData);

			var sumWriter = new SummaryDataContainer(fileWriter);
			sumWriter.UpdateTableColumns(runData.EngineData);
			var container = VehicleContainer.CreateVehicleContainer(runData, modData,
				sumWriter);
			var data = DrivingCycleDataReader.ReadFromFile(@"TestData/Cycles/LongHaul_short.vdri", CycleType.DistanceBased, false);
			new MockDrivingCycle(container, data);
			new ZeroMileageCounter(container);
			new DummyDriverInfo(container);
			var aux = new EngineAuxiliary(container);
			new MockEngine(container);

			foreach (var auxData in runData.Aux) {
				modData.AddAuxiliary(auxData.ID);
				aux.AddConstant(auxData.ID, auxData.PowerDemandMech);
			}

			var speed = 1400.RPMtoRad();
			var torque = 500.SI<NewtonMeter>();
			var t = 0.SI<Second>();
			var dt = 1.SI<Second>();

			new MockEngine(container);

			aux.Initialize(torque, speed);
			for (var i = 0; i < 11; i++) {
				aux.TorqueDemand(t, dt, torque, speed);
				modData[ModalResultField.dist] = i.SI<Meter>();
				modData[ModalResultField.P_ice_out] = 0.SI<Watt>();
				modData[ModalResultField.acc] = 0.SI<MeterPerSquareSecond>();
				modData[ModalResultField.ICEOn] = false;
				modData[ModalResultField.v_act] = 0.KMPHtoMeterPerSecond();
				container.CommitSimulationStep(t, dt);
				t += dt;
			}

			container.FinishSimulationRun();
			sumWriter.Finish();

			var testColumns = new[] { "P_aux_FAN", "P_aux_STP", "P_aux_AC", "P_aux_ES", "P_aux_PS", "P_aux_mech" };

			ResultFileHelper.TestModFile(@"TestData/Results/EngineOnlyCycles/AuxWriteModFileSumFile.vmod",
				@"AuxWriteModFileSumFile_MockCycle.vmod", testColumns);
			ResultFileHelper.TestSumFile(@"TestData/Results/EngineOnlyCycles/AuxWriteModFileSumFile.vsum",
				@"AuxWriteModFileSumFile.vsum");
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void AuxConstant()
		{
			var dataWriter = new MockModalDataContainer();
			var container = VehicleContainer.CreateVehicleContainer(null, dataWriter, null);
			//var port = new MockTnOutPort();
			var aux = new EngineAuxiliary(container);
			new MockEngine(container);

			var constPower = 1200.SI<Watt>();
			aux.AddConstant("CONSTANT", constPower);

			var speed = 2358.RPMtoRad();
			var torque = 500.SI<NewtonMeter>();
			var t = 0.SI<Second>();

			aux.Initialize(torque, speed);
			var auxDemand = aux.TorqueDemand(t, t, torque, speed);
			AssertHelper.AreRelativeEqual(constPower / speed, auxDemand);

			speed = 2358.RPMtoRad();
			torque = 1500.SI<NewtonMeter>();
			aux.Initialize(torque, speed);
			auxDemand = aux.TorqueDemand(t, t, torque, speed);
			AssertHelper.AreRelativeEqual(constPower / speed, auxDemand);

			speed = 1500.RPMtoRad();
			torque = 1500.SI<NewtonMeter>();
			aux.Initialize(torque, speed);
			auxDemand = aux.TorqueDemand(t, t, torque, speed);
			AssertHelper.AreRelativeEqual(constPower / speed, auxDemand);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void AuxDirect()
		{
			var dataWriter = new MockModalDataContainer();
			var container = VehicleContainer.CreateVehicleContainer(null, dataWriter, null);
			var data = DrivingCycleDataReader.ReadFromFile(@"TestData/Cycles/Coach time based short.vdri",
				CycleType.MeasuredSpeed, false);
			var cycle = new MockDrivingCycle(container, data);

			var aux = new EngineAuxiliary(container);
			new MockEngine(container);

			aux.AddCycle("CYCLE");
			container.AddAuxiliary("CYCLE");

			var speed = 2358.RPMtoRad();
			var torque = 500.SI<NewtonMeter>();

			var t = 0.SI<Second>();

			var expected = new[] { 6100, 3100, 2300, 4500, 6100 };
			foreach (var e in expected) {
				aux.Initialize(torque, speed);
				var auxDemand = aux.TorqueDemand(t, t, torque, speed);

				AssertHelper.AreRelativeEqual((e.SI<Watt>() / speed).Value(), auxDemand.Value());
				cycle.CommitSimulationStep(t, t, null);
			}
		}

		[TestCase] 
		public void AuxAllCombined()
		{
			var dataWriter = new MockModalDataContainer();
			dataWriter.AddAuxiliary("CONSTANT");

			var container = VehicleContainer.CreateVehicleContainer(null, dataWriter, null);
			var data = DrivingCycleDataReader.ReadFromFile(@"TestData/Cycles/Coach time based short.vdri",
				CycleType.MeasuredSpeed, false);
			// cycle ALT1 is set to values to equal the first few fixed points in the auxiliary file.
			// ALT1.aux file: nAuxiliary speed 2358: 0, 0.38, 0.49, 0.64, ...
			// ALT1 in cycle file: 0, 0.3724 (=0.38*0.96), 0.4802 (=0.49*0.96), 0.6272 (0.64*0.96), ...

			var cycle = new MockDrivingCycle(container, data);

			var aux = new EngineAuxiliary(container);
			new MockEngine(container);

			aux.AddCycle("CYCLE");
			var constPower = 1200.SI<Watt>();
			aux.AddConstant("CONSTANT", constPower);

			var speed = 578.22461991.RPMtoRad(); // = 2358 (nAuxiliary) * ratio
			var torque = 500.SI<NewtonMeter>();
			var t = 0.SI<Second>();
			// MQ 2021-03-03: updated expected values - mapping auxiliary is no longer supported
			var expected = new[] {
				1200 + 6100, // + 72.9166666666667,
				// = 1000 * 0.07 (nAuxiliary=2358 and psupply=0) / 0.98 (efficiency_supply)
				1200 + 3100, // + 677.083333333333,
				// = 1000 * 0.65 (nAuxiliary=2358 and psupply=0.38) / 0.98 (efficiency_supply)
				1200 + 2300, // + 822.916666666667,
				// = 1000 * 0.79 (nAuxiliary=2358 and psupply=0.49) / 0.98 (efficiency_supply)
				1200 + 4500, // + 1031.25, // = ...
				1200 + 6100, // + 1166.66666666667,
				1200 + 6100, // + 1656.25,
				1200 + 6100, // + 2072.91666666667,
				1200 + 6100, // + 2510.41666666667,
				1200 + 6100, // + 2979.16666666667,
				1200 + 6100, // + 3322.91666666667,
				1200 + 6100, // + 3656.25
			};

			foreach (var e in expected) {
				aux.Initialize(torque, speed);
				var auxDemand = aux.TorqueDemand(t, t, torque, speed);

				AssertHelper.AreRelativeEqual((e.SI<Watt>() / speed).Value(), auxDemand.Value());

				cycle.CommitSimulationStep(t, t, null);
			}
		}

		
		[Category("LongRunning")]
		[TestCase]
		public void AuxReadJobFileDeclarationMode()
		{
			var fileWriter = new FileOutputWriter("AuxReadJobFileDeclarationMode");
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);

			var inputData = JSONInputDataFactory.ReadJsonJob(
				@"TestData/Generic Vehicles/Declaration Mode/40t Long Haul Truck/40t_Long_Haul_Truck.vecto");
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
		}

		[Category("LongRunning")]
		[TestCase]
		public void AuxReadJobFileEngineeringMode()
		{
			var fileWriter = new FileOutputWriter("AuxReadJobFileEngineeringMode");
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);

			var inputData =
				JSONInputDataFactory.ReadJsonJob(@"TestData/Generic Vehicles/Engineering Mode/24t Coach/24t Coach.vecto");
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
		}

		[TestCase]
		public void AuxDeclarationWrongConfiguration()
		{
			var fileWriter = new FileOutputWriter("AuxReadJobFileDeclarationMode");
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);

			var inputData = JSONInputDataFactory.ReadJsonJob(@"TestData/Jobs/40t_Long_Haul_Truck_wrong_AUX.vecto");
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);

			AssertHelper.Exception<VectoException>(() => jobContainer.AddRuns(runsFactory));
		}
	}
}
