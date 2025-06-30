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

using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class FactoryTest
	{
		public const string DeclarationJobFile = @"TestData/Jobs/12t Delivery Truck.vecto";

		public const string EngineeringJobFile = @"TestData/Jobs/24t Coach.vecto";
		

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void CreateDeclarationSimulationRun()
		{
			var fileWriter = new FileOutputWriter(DeclarationJobFile);

			var inputData = JSONInputDataFactory.ReadJsonJob(DeclarationJobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);

			//factory.DataReader.SetJobFile(DeclarationJobFile);

			var run = factory.SimulationRuns().First();
			var vehicleContainer = (VehicleContainer)run.GetContainer();

			Assert.AreEqual(10, vehicleContainer.SimulationComponents().Count);

			Assert.IsInstanceOf<AMTGearbox>(vehicleContainer.GearboxInfo, "gearbox not installed");
			Assert.IsInstanceOf<CombustionEngine>(vehicleContainer.EngineInfo, "engine not installed");
			Assert.IsInstanceOf<Vehicle>(vehicleContainer.VehicleInfo, "vehicle not installed");

			var gearbox = vehicleContainer.GearboxInfo as AMTGearbox;
			Assert.IsNotNull(gearbox);

			// -- shiftpolygon downshift 

			// no downshift curve in first gear!
			Assert.AreEqual(660.RPMtoRad().Value(), gearbox.ModelData.Gears[2].ShiftPolygon.Downshift[0].AngularSpeed.Value(),
				0.0001);
			Assert.AreEqual(-163.9, gearbox.ModelData.Gears[2].ShiftPolygon.Downshift[0].Torque.Value(), 0.0001);

			Assert.AreEqual(660.RPMtoRad().Value(), gearbox.ModelData.Gears[2].ShiftPolygon.Downshift[1].AngularSpeed.Value(),
				0.0001);
			Assert.AreEqual(623.966, gearbox.ModelData.Gears[2].ShiftPolygon.Downshift[1].Torque.Value(), 0.1);

			Assert.AreEqual(800.RPMtoRad().Value(),
				gearbox.ModelData.Gears[2].ShiftPolygon.Downshift[2].AngularSpeed.Value(),
				0.1);
			Assert.AreEqual(725.102, gearbox.ModelData.Gears[2].ShiftPolygon.Downshift[2].Torque.Value(), 0.0001);

			// -- shiftpolygon upshift

			Assert.AreEqual(2318.2812.RPMtoRad().Value(),
				gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[0].AngularSpeed.Value(),
				0.1);
			Assert.AreEqual(-163.9, gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[0].Torque.Value(), 0.0001);

			Assert.AreEqual(2318.2812.RPMtoRad().Value(),
				gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[1].AngularSpeed.Value(),
				0.1);
			Assert.AreEqual(988.9, gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[1].Torque.Value(), 0.1);
		}

		[TestCase]
		public void CreateEngineeringSimulationRun()
		{
			var fileWriter = new FileOutputWriter(EngineeringJobFile);

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			var run = factory.SimulationRuns().First();

			var vehicleContainer = (VehicleContainer)run.GetContainer();
			Assert.AreEqual(12, vehicleContainer.SimulationComponents().Count);

			Assert.IsInstanceOf<AMTGearbox>(vehicleContainer.GearboxInfo, "gearbox not installed");
			Assert.IsInstanceOf<CombustionEngine>(vehicleContainer.EngineInfo,  "engine not installed");
			Assert.IsInstanceOf<Vehicle>(vehicleContainer.VehicleInfo,  "vehicle not installed");
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestDistanceCycleInVTPEngineering()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(@"TestData/Jobs/VTPModeWithDistanceCycle.vecto");
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, null);

			AssertHelper.Exception<VectoException>(() => factory.SimulationRuns().ToArray(), "first cycle is not a VTP cycle!");
		}

		[Category("LongRunning")]
		[TestCase]
		public void TestDistanceCycleInEngineOnly()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(@"TestData/Jobs/EngineOnlyJobWithDistanceCycle.vecto");
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, null);

			AssertHelper.Exception<VectoException>(() => factory.SimulationRuns().ToArray(), "Distance-based cycle can not be simulated in EngineOnly mode");
		}

		[TestCase]
		public void TestMeasuredSpeedCycleInEngineOnly()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(@"TestData/Jobs/EngineOnlyJobWithMeasuredCycle.vecto");
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, null);

			AssertHelper.Exception<VectoException>(() => {
				var container = new JobContainer(null);
				container.AddRuns(factory);
				container.Execute();
				//factory.SimulationRuns().ToArray();
			}, "MeasuredSpeed-cycle can not be simulated in EngineOnly mode");
		}
	}
}