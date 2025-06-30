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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class PowerTrainBuilderTest
	{
		protected IPowertrainBuilder PowertrainBuilder;
		protected IEngineeringDataAdapter DataAdapter;

        public const string JobFile = @"TestData/Jobs/24t Coach.vecto";
		public const string JobFileNoAngular = @"TestData/Jobs/24t CoachNoAng.vecto";
		public const string JobFileAngEfficiency = @"TestData/Jobs/24t Coach_Ang_Efficiency.vecto";

		public const string JobFileDecl = @"TestData/Jobs/40t_Long_Haul_Truck.vecto";
		public const string JobFileDeclNoAngular = @"TestData/Jobs/40t_Long_Haul_Truck_NoAng.vecto";
		public const string JobFileDeclAngEfficiency = @"TestData/Jobs/40t_Long_Haul_Truck with AngleEfficiency.vecto";

		public const string JobFileBEVE2 = @"TestData/BatteryElectric/GenericVehicleB2/BEV_ENG.vecto";
		public const string JobFileBEVE3 = @"TestData/BatteryElectric/GenericVehicleB3/BEV_ENG.vecto";
		public const string JobFileBEVE4 = @"TestData/BatteryElectric/GenericVehicleB4/BEV_ENG.vecto";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			var kernel = new StandardKernel(new VectoNinjectModule());
			PowertrainBuilder = kernel.Get<IPowertrainBuilder>();
			DataAdapter = kernel.Get<IEngineeringDataAdapter>();
		}

		[Category("LongRunning")]
		[TestCase(JobFile, 12),
		TestCase(JobFileNoAngular, 11),
		TestCase(JobFileAngEfficiency, 12)]
		public void BuildFullPowerTrain_Engineering(string inputFile, int componentCount)
		{
			var dataProvider = JSONInputDataFactory.ReadJsonJob(inputFile);
			var engineeringProvider = dataProvider as IEngineeringInputDataProvider;
			if (engineeringProvider == null) {
				throw new VectoException("Failed to cast to Engineering InputDataProvider");
			}
			
            var reader = new EngineeringModeVectoRunDataFactory(engineeringProvider, PowertrainBuilder, DataAdapter);
			var runData = reader.NextRun().First();

			var powerTrain = PowertrainBuilder.Build(runData, new MockModalDataContainer()) as VehicleContainer;

			Assert.NotNull(powerTrain);
			Assert.IsInstanceOf<IVehicleContainer>(powerTrain);
			Assert.AreEqual(componentCount, powerTrain.SimulationComponents().Count);

			Assert.IsInstanceOf<CombustionEngine>(powerTrain.EngineInfo);
			Assert.IsInstanceOf<AMTGearbox>(powerTrain.GearboxInfo);
			Assert.IsInstanceOf<ISimulationOutPort>(powerTrain.Cycle);
			Assert.IsInstanceOf<Vehicle>(powerTrain.VehicleInfo);
		}

		[TestCase(JobFileDeclNoAngular, 11, false),
		TestCase(JobFileDecl, 12, false),
		TestCase(JobFileDeclAngEfficiency, 11, true)]
		public void BuildFullPowerTrain_Declaration(string inputFile, int componentCount, bool shouldFail)
		{
			var dataProvider = JSONInputDataFactory.ReadJsonJob(inputFile);
			var provider = dataProvider as IDeclarationInputDataProvider;
			if (provider == null) {
				throw new VectoException("Failed to cast to Declaration InputDataProvider");
			}
			var kernel = new StandardKernel(new VectoNinjectModule());
			var reader = kernel.Get<IVectoRunDataFactoryFactory>()
				.CreateDeclarationRunDataFactory(provider, null, null);

			if (!shouldFail) {
				var runData = reader.NextRun().First();

				var powerTrain = PowertrainBuilder.Build(runData, new MockModalDataContainer()) as VehicleContainer;

				Assert.NotNull(powerTrain);
				Assert.IsInstanceOf<IVehicleContainer>(powerTrain);
				Assert.AreEqual(componentCount, powerTrain.SimulationComponents().Count);

				Assert.IsInstanceOf<CombustionEngine>(powerTrain.EngineInfo);
				Assert.IsInstanceOf<AMTGearbox>(powerTrain.GearboxInfo);
				Assert.IsInstanceOf<ISimulationOutPort>(powerTrain.Cycle);
				Assert.IsInstanceOf<Vehicle>(powerTrain.VehicleInfo);
			} else {
				AssertHelper.Exception<VectoException>(() => { reader.NextRun().ToList(); });
			}
		}

		[TestCase(JobFileBEVE2),
		TestCase(JobFileBEVE3),
		TestCase(JobFileBEVE4)]
		public void BuildFullPowerTrain_Engineering_BEV(string inputFile)
		{
			var dataProvider = JSONInputDataFactory.ReadJsonJob(inputFile);
			var engineeringProvider = dataProvider as IEngineeringInputDataProvider;
			if (engineeringProvider == null)
			{
				throw new VectoException("Failed to cast to Engineering InputDataProvider");
			}
			var reader = new EngineeringModeVectoRunDataFactory(engineeringProvider, PowertrainBuilder, DataAdapter);
			var runData = reader.NextRun().First();

			var writer = new MockModalDataContainer();
			var powerTrain = PowertrainBuilder.Build(runData, new MockModalDataContainer()) as VehicleContainer;

			Assert.NotNull(powerTrain);
		}
	}
}
