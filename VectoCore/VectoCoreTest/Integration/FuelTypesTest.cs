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
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class FuelTypesTest
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(FuelType.DieselCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 0,
			0.00021674714805162135, 0.00021674714805162135, 25.926692350672408, 0.0006784185734015748, 9255.103221804231,
			TestName = "TestFuelTypesCO2 Diesel LH Low"),
		TestCase(FuelType.EthanolCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 0,
			0.00021674714805162135, 0.00021930715373727074, 26.74477484600863, 0.00039694594826446007, 5570.401704926677,
			TestName = "TestFuelTypesCO2 Ethanol/CI LH Low"),

		TestCase(FuelType.DieselCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.00024815363205920937, 0.00024815363205920937, 29.683448810910214, 0.0007767208683453253, 10596.16008892824,
			TestName = "TestFuelTypesCO2 Diesel LH Ref"),
		TestCase(FuelType.EthanolCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.00024815363205920937, 0.00025108458046935726, 30.620070788946006, 0.00045446309064953665, 6377.548343921674,
			TestName = "TestFuelTypesCO2 Ethanol/CI LH Ref"),
		TestCase(FuelType.EthanolPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.00024815363205920937, 0.00024645975061170444, 31.356202367901332, 0.0005175654762845794, 7221.27069292294,
			TestName = "TestFuelTypesCO2 Ethanol/PI LH Ref"),
		TestCase(FuelType.PetrolPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.00024815363205920937, 0.00024815363205920937, 33.17561925925259, 0.0007543870414599965, 10298.375730457188,
			TestName = "TestFuelTypesCO2 Petrol/PI LH Ref"),
		TestCase(FuelType.LPGPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.00024815363205920937, 0.00024815363205920937, null, 0.0007494239688188123, 11415.06707472363,
			TestName = "TestFuelTypesCO2 LPG/PI LH Ref"),
		TestCase(FuelType.NGPI, TankSystem.Liquefied,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.00024815363205920937, 0.00022793745022139266, null, 0.0006313867371132576, 11191.72880587038,
			TestName = "TestFuelTypesCO2 LNG/PI LH Ref"),
		TestCase(FuelType.NGPI, TankSystem.Compressed,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.00024815363205920937, 0.00023316101678896636, null, 0.0006272031351623195, 11191.728805870385,
			TestName = "TestFuelTypesCO2 CNG/PI LH Ref"),
			]
		public void TestFuelTypesCO2(FuelType fuelType, TankSystem? tankSystem, string jobName, int runIdx, 
			double expectedFCMap, double expectedFCFinal, double? expectedFCperkm, double expectedCo2, double expectedMJ)
		{
			// the same engine fc-map is used for different fuel types, thus the difference in expected results
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			runsFactory.WriteModalResults = true;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			// store a reference to the data table here because it is going to be deleted
			var run = jobContainer.Runs[runIdx];
			var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
			var modData = modContainer.Data;

			var fuelData = FuelData.Instance().Lookup(fuelType, tankSystem);
			
			// change the fuel entry in mod data and engine data
			var origFuel = modContainer.FuelColumns.Keys.First();
			modContainer.FuelColumns[fuelData] = modContainer.FuelColumns[origFuel];
			if (fuelData.FuelType != origFuel.FuelType) {
				modContainer.FuelColumns.Remove(origFuel);
			}
			((VehicleContainer)run.Run.GetContainer()).RunData.EngineData.Fuels.First().FuelData = fuelData;

			run.Run.Run();

			// restore data table before assertions
			modContainer.Data = modData;
			var fuel = modContainer.FuelData.First();
			var distance = modContainer.Distance;
			var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
			var fcVolumePerMeter = fuel.FuelDensity == null ? null :
				(fcFinal / fuel.FuelDensity)
				.Cast<VolumePerMeter>();
			var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
			var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

//			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
			Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
				modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
				fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
				co2PerMeter?.Value(), energyPerMeter?.Value());

			AssertHelper.AreRelativeEqual(expectedFCMap, modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel), 1e-3);
			AssertHelper.AreRelativeEqual(expectedFCFinal, fcFinal, 1e-3);
			AssertHelper.AreRelativeEqual(expectedFCperkm, fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value.SI(), 1e-3);

			AssertHelper.AreRelativeEqual(expectedCo2, co2PerMeter, 1e-3);
			AssertHelper.AreRelativeEqual(expectedMJ, energyPerMeter, 1e-3);
		}
	}
}
