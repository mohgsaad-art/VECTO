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
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using DummyDriverInfo = TUGraz.VectoCore.Tests.Utils.DummyDriverInfo;
using MockDrivingCycle = TUGraz.VectoCore.Tests.Utils.MockDrivingCycle;

namespace TUGraz.VectoCore.Tests.Integration.EngineOnlyCycle
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class EngineOnlyCycleTest
	{
		private const string EngineFile = @"TestData/Components/24t Coach.veng";

		[OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(
			"24tCoach_EngineOnly",
			@"TestData/Components/24t Coach.veng",
			@"TestData/Cycles/Coach Engine Only.vdri",
			@"TestData/Results/EngineOnlyCycles/24tCoach_EngineOnly.vmod")]
		[TestCase(
			"24tCoach_EngineOnlyPaux",
			@"TestData/Components/24t Coach.veng",
			@"TestData/Cycles/Coach Engine Only Paux.vdri",
			@"TestData/Results/EngineOnlyCycles/24tCoach_EngineOnlyPaux.vmod")]
		[TestCase(
			"24tCoach_EngineOnlyFullLoad",
			@"TestData/Components/24t Coach.veng",
			@"TestData/Cycles/Coach Engine Only FullLoad.vdri",
			@"TestData/Results/EngineOnlyCycles/24tCoach_EngineOnlyFullLoad.vmod")]
		public void TestEngineOnlyDrivingCycle(string testName, string engineFile, string cycleFile, string modalResultFile)
		{
			var data = DrivingCycleDataReader.ReadFromFile(
				cycleFile, CycleType.EngineOnly,
				false);
			var vehicle = VehicleContainer.CreateVehicleContainer(null, null, null);

			// ReSharper disable once ObjectCreationAsStatement
			new MockDrivingCycle(vehicle, data);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(engineFile, 0);

			var aux = new EngineAuxiliary(vehicle);
			aux.AddCycle(Constants.Auxiliaries.Cycle);

			var engine = new EngineOnlyCombustionEngine(vehicle, engineData);
			engine.Connect(aux);
			new EngineOnlyGearboxInfo(vehicle);
			new ZeroMileageCounter(vehicle);
			new DummyDriverInfo(vehicle);
			//aux.InPort().Connect(engine.OutPort());
			var port = engine.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var modFile = Path.GetFileNameWithoutExtension(modalResultFile);

			var runData = new VectoRunData() {
				EngineData = engineData,
				JobName = modFile,
				JobType = VectoSimulationJobType.EngineOnlySimulation
			};

			//Path.GetFileNameWithoutExtension(Path.GetRandomFileName()); // + ".vmod";
			var fileWriter = new FileOutputWriter(modFile);
			var kernel = new StandardKernel(new VectoNinjectModule());
			var modData = kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, fileWriter, null, null) as ModalDataContainer;
			Assert.NotNull(modData);
			modData.WriteModalResults = true;
			modData.AddAuxiliary(Constants.Auxiliaries.Cycle);
			modData.Data.CreateColumns(ModalResults.TimeCycleSignals);
			modData.Data.CreateCombustionEngineColumns(runData);
			port.Initialize(data.Entries.First().Torque, data.Entries.First().AngularVelocity);
			foreach (var cycleEntry in data.Entries) {
				// ReSharper disable once UnusedVariable
				var response = (ResponseSuccess)port.Request(absTime, dt, cycleEntry.Torque, cycleEntry.AngularVelocity, false);
				foreach (var sc in vehicle.Components) {
					modData[ModalResultField.time] = absTime + dt / 2;
					sc.CommitSimulationStep(absTime, dt, modData);
				}

				modData.CommitSimulationStep();
				absTime += dt;
			}

			modData.Finish(VectoRun.Status.Success);

			ResultFileHelper.TestModFile(modalResultFile, modFile + Constants.FileExtensions.ModDataFile);
		}

		[TestCase]
		public void AssembleEngineOnlyPowerTrain()
		{
			var dataWriter = new MockModalDataContainer();

			var vehicleContainer = VehicleContainer.CreateVehicleContainer(null, null, null);

			var engine = new CombustionEngine(
				vehicleContainer, MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile, 0));
			var gbx = new MockGearbox(vehicleContainer) { Gear = new GearshiftPosition(0) };

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var angularVelocity = 644.4445.RPMtoRad();
			var power = 2329.973.SI<Watt>();

			engine.OutPort().Initialize(power / angularVelocity, angularVelocity);
			engine.OutPort().Request(absTime, dt, power / angularVelocity, angularVelocity, false);

			foreach (var sc in vehicleContainer.Components) {
				sc.CommitSimulationStep(absTime, dt, dataWriter);
			}

			Assert.IsNotNull(dataWriter.CurrentRow);
		}
	}
}
