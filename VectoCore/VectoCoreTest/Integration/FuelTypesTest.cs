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
			0.000219921110821829, 0.000219921110821829, 26.3063529691184, 0.000688353076872326, 9390.63143209212,
			TestName = "Diesel LH Low"),
		TestCase(FuelType.EthanolCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 0,
			0.000219921110821829, 0.000222518604256733, 27.1364151532601, 0.000402758673704687, 5651.97254812102,
			TestName = "Ethanol/CI LH Low"),

		TestCase(FuelType.DieselCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254735249582656, 0.000254735249582656, 30.4707236342891, 0.000797321331193715, 10877.1951571794,
			TestName = "Diesel LH Ref"),
		TestCase(FuelType.EthanolCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254735249582656, 0.000257743933632844, 31.4321870283957, 0.000466516519875449, 6546.69591427425,
			TestName = "Ethanol/CI LH Ref"),
		TestCase(FuelType.EthanolPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254735249582656, 0.00025299644241827, 32.1878425468536, 0.000531292529078366, 7412.7957628553,
			TestName = "Ethanol/PI LH Ref"),
		TestCase(FuelType.PetrolPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254735249582656, 0.000254735249582656, 34.0555146500878, 0.000774395158731276, 10571.5128576802,
			TestName = "Petrol/PI LH Ref"),
		TestCase(FuelType.LPGPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254735249582656, 0.000254735249582656, null, 0.000769300453739622, 11717.8214808022,
			TestName = "LPG/PI LH Ref"),
		TestCase(FuelType.NGPI, TankSystem.Liquefied,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254735249582656, 0.000233982887091198, null, 0.000648132597242618, 11488.5597561778,
			TestName = "LNG/PI LH Ref"),
		TestCase(FuelType.NGPI, TankSystem.Compressed,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254735249582656, 0.000239344994920373, null, 0.000643838036335802, 11488.5597561779,
			TestName = "CNG/PI LH Ref"),
			]
		public void TestFuelTypesCO2(FuelType fuelType, TankSystem? tankSystem, string jobName, int runIdx, 
			double expectedFCMap, double expectedFCFinal, double? expectedFCperkm, double expectedCo2, double expectedMJ)
		{
			// the same engine fc-map is used for different fuel types, thus the difference in expected results
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter) {
				WriteModalResults = true,
				Validate = false
			};

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
