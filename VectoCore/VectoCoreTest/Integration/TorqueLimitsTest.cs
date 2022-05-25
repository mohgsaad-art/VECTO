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
using System.Data;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class TorqueLimitsTest
	{
		const string GearboxLimitJobDecl_865 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_GbxTorqueLimits\Class2_RigidTruck_gbxTqLimit-865_DECL.vecto";

		const string GearboxLimitJobDecl_800 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_GbxTorqueLimits\Class2_RigidTruck_gbxTqLimit-800_DECL.vecto";


		const string VehicleLimitJobDecl_910 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-910_DECL.vecto";

		const string VehicleLimitJobDecl_850 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-850_DECL.vecto";

		const string GearboxSpeedLimitJobDecl =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_GbxSpeedLimits\Class2_RigidTruck_DECL.vecto";

		const string EngineSpeedLimitJobDecl =
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2_engineSpeedlimit\Class2_RigidTruck_DECL.vecto";

		const string EngineSpeedLimitJobATDecl =
			@"TestData\Integration\EngineeringMode\TruckAT_GbxSpeedLimit\TruckAT.vecto";

		private const string DeclarationVehicle9GearsFord =
			@"TestData\Integration\DeclarationMode\EngineSpeedTooHigh\vecto_vehicle-sample_9gears.xml";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}


		[TestCase()]
		public void TestGearboxTorqueLimitsAbove90FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(GearboxLimitJobDecl_865);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.RunDataFactory.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by gbx
			Assert.AreEqual(865, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by gbx but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by gbx but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase()]
		public void TestGearboxTorqueLimitsBelow90FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(GearboxLimitJobDecl_800);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.RunDataFactory.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by gbx
			Assert.AreEqual(800, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by gbx
			Assert.AreEqual(800, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by gbx
			Assert.AreEqual(800, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase()]
		public void TestVehicleTorqueLimitsAbove95FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(VehicleLimitJobDecl_910);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.RunDataFactory.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase()]
		public void TestVehicleTorqueLimitsBelow95FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(VehicleLimitJobDecl_850);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.RunDataFactory.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by vehicle
			Assert.AreEqual(850, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by vehicle 
			Assert.AreEqual(850, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase(GearboxLimitJobDecl_800),
		TestCase(GearboxLimitJobDecl_865),
		TestCase(VehicleLimitJobDecl_850),
		TestCase(VehicleLimitJobDecl_910), Category("LongRunning")]
		public void TestRunTorqueLimitedSimulations(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter);
			factory.WriteModalResults = true;


			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[Category("LongRunning"), TestCase(GearboxSpeedLimitJobDecl)]
		[Ignore("Too long, run only before release.")]
		public void TestRunGbxSpeedLimitedSimulations(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter);
			factory.WriteModalResults = true;
			factory.ActualModalData = true;


			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase(EngineSpeedLimitJobDecl), Category("LongRunning")]
		public void TestRunEngineSpeedLimitedSimulations(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter);
			factory.WriteModalResults = true; //ActualModalData = true


			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			//jobContainer.Runs[1].RunWorkerAsync().Wait();

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
			var view = new DataView(sumData.Table, "", SumDataFields.SORT, DataViewRowState.CurrentRows).ToTable();
			Console.WriteLine(string.Join("; ", view.Rows.Cast<DataRow>().Select(x => x[string.Format(SumDataFields.FCMAP_KM, "")].ToString().ToDouble())));
			//199.78724745793005; 235.80023117081666; 169.7345262485087; 182.825905694444; 219.40786682729086; 250.54711559861258
			Assert.AreEqual(199.78724745793005, view.Rows[0][string.Format(SumDataFields.FCMAP_KM, "")].ToString().ToDouble(), 1e-3);
			Assert.AreEqual(235.80023117081666, view.Rows[1][string.Format(SumDataFields.FCMAP_KM, "")].ToString().ToDouble(), 1e-3);
			Assert.AreEqual(169.7345262485087, view.Rows[2][string.Format(SumDataFields.FCMAP_KM, "")].ToString().ToDouble(), 1e-3);
			Assert.AreEqual(182.825905694444, view.Rows[3][string.Format(SumDataFields.FCMAP_KM, "")].ToString().ToDouble(), 1e-3);
			Assert.AreEqual(219.40786682729086, view.Rows[4][string.Format(SumDataFields.FCMAP_KM, "")].ToString().ToDouble(), 1e-3);
			Assert.AreEqual(250.54711559861258, view.Rows[5][string.Format(SumDataFields.FCMAP_KM, "")].ToString().ToDouble(), 1e-3);
		}

		[TestCase(EngineSpeedLimitJobATDecl)]
		public void EngineSpeedSpeedLimitAT(string jobFile)
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var fileWriter = new FileOutputWriter(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory.WriteModalResults = true;
			factory.Validate = false;
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			//var runs = jobContainer.Runs;
			//runs[2].Run.Run();

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase(DeclarationVehicle9GearsFord)]
		public void EngineSpeedTooHigh9SpeedGearbox(string jobFile)
		{
			var inputData = xmlInputReader.CreateDeclaration(jobFile);
			var fileWriter = new FileOutputWriter(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);
			factory.WriteModalResults = true;
			factory.Validate = false;
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			//var runs = jobContainer.Runs;
			//runs[2].Run.Run();

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[Category("LongRunning"),
		TestCase(@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear6.vecto"),
		TestCase(@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear6and5.vecto")]
		public void TestRunDisableGearsWithTorqueLimitSimulations(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase(@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear5invalid.vecto"),
		 TestCase(@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear4invalid.vecto"),
		 TestCase(@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear6and4invalid.vecto"),
		 TestCase(@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear1invalid.vecto")]
		public void TestRunDisableGearsInvalid(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter);
			var jobContainer = new JobContainer(sumData);
			AssertHelper.Exception<VectoException>(() => jobContainer.AddRuns(factory), messageContains: "Only the last 1 or 2 gears can be disabled. Disabling gear ");
		}

		[Category("LongRunning"),
		TestCase(@"Testdata\Integration\EngineeringMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear6.vecto"),
		TestCase(@"Testdata\Integration\EngineeringMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear6and5.vecto")]
		public void TestRunDisableGearsWithTorqueLimitSimulationsEngineering(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputDataProvider, fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase(@"Testdata\Integration\EngineeringMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear5invalid.vecto"),
		 TestCase(@"Testdata\Integration\EngineeringMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear4invalid.vecto"),
		 TestCase(@"Testdata\Integration\EngineeringMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear6and4invalid.vecto"),
		 TestCase(@"Testdata\Integration\EngineeringMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-disableGear1invalid.vecto")]
		public void TestRunDisableGearsInvalidEngineering(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputDataProvider, fileWriter);
			var jobContainer = new JobContainer(sumData);
			AssertHelper.Exception<VectoException>(() => jobContainer.AddRuns(factory), messageContains: "Only the last 1 or 2 gears can be disabled. Disabling gear ");
		}


	}
}
