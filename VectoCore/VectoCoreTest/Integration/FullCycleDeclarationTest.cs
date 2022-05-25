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

using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using NUnit.Framework;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class FullCycleDeclarationTest
	{
		public const string LongHaulTruckDeclarationJob =
			@"TestData\Integration\DeclarationMode\40t Truck\40t_Long_Haul_Truck.vecto";

		public const string DeliveryTruckDeclarationJob =
			@"TestData\Integration\DeclarationMode\12t Truck\12t Delivery Truck.vecto";

		public const string DeliveryTruck8GearDeclarationJob =
			@"TestData\Integration\DeclarationMode\12t Truck\12t Delivery Truck_8gear.vecto";

		public const string Class9RigidTruckPTOJob =
			@"TestData\Integration\DeclarationMode\Class9_RigidTruck_6x2\Class9_RigidTruck_DECL.vecto";

		public const string Class5TractorDeclPrimaryRetarder =
			@"TestData\Integration\DeclarationMode\Class5_Tractor_4x2\Class5_Tractor_DECL_primRet.vecto";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[Category("LongRunning")]
		[TestCase]
		public void Truck40t_LongHaulCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("LongHaul");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_LongHaulCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Category("LongRunning")]
		[TestCase]
		public void Truck40t_RegionalDeliveryCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("RegionalDelivery");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_RegionalDeliveryCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>(), true);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Category("LongRunning")]
		[TestCase]
		public void Truck40t_UrbanDeliveryCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("UrbanDelivery");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_UrbanDeliveryCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Category("LongRunning")]
		[TestCase]
		public void Truck40t_MunicipalCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("MunicipalUtility");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_MunicipalCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Category("LongRunning")]
		[TestCase]
		public void Truck40t_ConstructionCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Construction");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_ConstructionCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[Category("LongRunning")]
		[TestCase]
		public void Truck40t_HeavyUrbanCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("HeavyUrban");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_HeavyUrbanCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			//((DistanceBasedDrivingCycle)((VehicleContainer)run.GetContainer()).Cycle).SetDriveOffDistance(26700.SI<Meter>());
			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestCase]
		public void Truck40t_SubUrbanCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Suburban");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_SubUrbanCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestCase, Category("LongRunning")]
		public void Truck40t_InterUrbanCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Interurban");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_InterUrbanCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());
			//((DistanceBasedDrivingCycle)((VehicleContainer)run.GetContainer()).Cycle).SetDriveOffDistance(33000.SI<Meter>());
			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestCase, Category("LongRunning")]
		public void Truck40t_CoachCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Coach");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_CoachCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			//((DistanceBasedDrivingCycle)((VehicleContainer)run.GetContainer()).Cycle).SetDriveOffDistance(152429.9.SI<Meter>());
			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestCase, Category("LongRunning")]
		public void Truck40t_DeclarationTest()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(LongHaulTruckDeclarationJob);
			var fileWriter = new FileOutputWriter(LongHaulTruckDeclarationJob);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory.WriteModalResults = true;
			factory.Validate = false;
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);

			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase, Category("LongRunning")]
		public void Truck40t_Mod1Hz_Test()
		{
			var modFileName = "40t_Long_Haul_Truck_RegionalDeliveryReferenceLoad.vmod";
			var modFileName1Hz = "40t_Long_Haul_Truck_RegionalDeliveryReferenceLoad_1Hz.vmod";

			if (File.Exists(modFileName)) {
				File.Delete(modFileName);
			}

			if (File.Exists(modFileName1Hz)) {
				File.Delete(modFileName1Hz);
			}

			var inputData = JSONInputDataFactory.ReadJsonJob(LongHaulTruckDeclarationJob);
			var fileWriter = new FileOutputWriter("Truck40t_Mod1Hz_Test.vecto");
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory.WriteModalResults = true;
			factory.ModalResults1Hz = false;
			var factory1Hz = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory1Hz.WriteModalResults = true;
			factory1Hz.ModalResults1Hz = true;
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			var i = 5;
			jobContainer.Runs[i].Run.Run();
			Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors,
				$"{jobContainer.Runs[i].ExecException}");

			jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory1Hz);

			jobContainer.Runs[i].Run.Run();
			Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors,
				$"{jobContainer.Runs[i].ExecException}");

			var modFile = VectoCSVFile.Read(modFileName);
			var modFile1Hz = VectoCSVFile.Read(modFileName1Hz);

			// test if line count matches the second count
			var maxSeconds = modFile.Rows.Cast<DataRow>().Last().ParseDouble(ModalResultField.time.GetShortCaption());
			var lineCount1Hz = modFile1Hz.Rows.Count;

			AssertHelper.AreRelativeEqual(lineCount1Hz, maxSeconds,
				$"LineCount must equal max seconds. Lines={lineCount1Hz}, MaxSeconds={maxSeconds}", 1);

			// test max distance
			var maxDistance = modFile.Rows.Cast<DataRow>().Last().ParseDouble(ModalResultField.dist.GetShortCaption());
			var maxDistance1Hz = modFile1Hz.Rows.Cast<DataRow>().Last().ParseDouble(ModalResultField.dist.GetShortCaption());
			AssertHelper.AreRelativeEqual(maxDistance, maxDistance1Hz);

			// test if interval in 1Hz always is 1
			Assert.IsTrue(
				modFile1Hz.Rows.Cast<DataRow>()
					.All(r => r.ParseDouble(ModalResultField.simulationInterval.GetShortCaption()).IsEqual(1)),
				"dt must always be 1 second");

			// test if absTime in 1Hz always is a whole number and consecutive
			Assert.IsTrue(
				modFile1Hz.Rows.Cast<DataRow>()
					.Select((r, index) => r.ParseDouble(ModalResultField.time.GetName()).IsEqual(index + 1))
					.All(p => p), "time must be whole numbers and consecutive");

			// test sum of fuel consumption
			var sumFuelConsumption = modFile.Rows.Cast<DataRow>().Select(r =>
				r.ParseDoubleOrGetDefault(ModalResultField.FCWHTCc.GetShortCaption()) *
				r.ParseDouble(ModalResultField.simulationInterval.GetShortCaption())).Sum();
			var sumFuelConsumption1Hz =
				modFile1Hz.Rows.Cast<DataRow>()
					.Select(r => r.ParseDoubleOrGetDefault(ModalResultField.FCWHTCc.GetShortCaption()))
					.Sum();
			AssertHelper.AreRelativeEqual(sumFuelConsumption, sumFuelConsumption1Hz, "Fuel Consumption is not equal", 1e-4);

			// test speed not negative
			Assert.IsTrue(
				modFile1Hz.Rows.Cast<DataRow>()
					.All(r => r.ParseDouble(ModalResultField.v_act.GetShortCaption()).IsGreaterOrEqual(0)),
				"v_act must not be negative.");

			// test fuel consumption not negative
			Assert.IsTrue(
				modFile1Hz.Rows.Cast<DataRow>()
					.All(r => r.ParseDouble(string.Format(ModalResultField.FCWHTCc.GetShortCaption(), "")).IsGreaterOrEqual(0)),
				"fuel consumption must not be negative.");

			// last v_act entry must be the same as original
			var vAct = modFile.Rows.Cast<DataRow>().Last().ParseDouble(ModalResultField.v_act.GetName()).SI<MeterPerSecond>();
			var vAct1Hz = modFile1Hz.Rows.Cast<DataRow>().Last().ParseDouble(ModalResultField.v_act.GetName()).SI<MeterPerSecond>();
			AssertHelper.AreRelativeEqual(vAct, vAct1Hz, 1e-4, "end velocity is not equal");
		}

		[TestCase, Category("LongRunning")]
		public void Truck12t_DeclarationTest()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(DeliveryTruckDeclarationJob);
			var fileWriter = new FileOutputWriter(DeliveryTruckDeclarationJob);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory.WriteModalResults = true;
			factory.Validate = false;
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			//var i = 2;
			//jobContainer.Runs[i].Run.Run();
			//Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors);

			//var i = 0;
			//foreach (var runEntry in jobContainer.Runs) {
			//	runEntry.Run.Run();
			//	Assert.IsTrue(runEntry.Run.FinishedWithoutErrors, "run {0} failed", i);
			//	i++;
			//}

			jobContainer.Execute();
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success),
				$"folowing runs failed: {string.Concat(jobContainer.Runs.Where(r => !r.Success).Select(r => r.Run.RunName + " - " + r.Run.CycleName))}\n" +
				$"{string.Concat(jobContainer.Runs.Select(r => r.ExecException))}");
		}

		[TestCase, Category("LongRunning")]
		public void Truck12t8Gear_DeclarationTest()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(DeliveryTruck8GearDeclarationJob);
			var fileWriter = new FileOutputWriter(DeliveryTruck8GearDeclarationJob);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory.WriteModalResults = true;
			factory.Validate = false;
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			//var runs = jobContainer.Runs;

			//runs[8].Run.Run();

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}


		[TestCase]
		public void DeclarationClass9PTOTest()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(Class9RigidTruckPTOJob);
			var fileWriter = new FileOutputWriter(Class9RigidTruckPTOJob);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory.WriteModalResults = true;
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			//var runs = jobContainer.Runs;

			//runs[8].Run.Run();

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase]
		public void DeclarationClass5PrimaryRetarder()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(Class5TractorDeclPrimaryRetarder);
			var fileWriter = new FileOutputWriter(Class5TractorDeclPrimaryRetarder);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory.WriteModalResults = true;
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			//var runs = jobContainer.Runs;
			//runs[0].Run.Run();

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}
	}
}