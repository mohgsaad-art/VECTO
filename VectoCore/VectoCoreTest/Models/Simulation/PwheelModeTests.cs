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
using System.Text;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Ninject;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class PwheelModeTests
	{
		protected IPowertrainBuilder PowertrainBuilder;

        [OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			var kernel = new StandardKernel(new VectoNinjectModule());
			PowertrainBuilder = kernel.Get<IPowertrainBuilder>();
        }


		/// <summary>
		/// Test if the cycle file can be read.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Pwheel_ReadCycle_Test()
		{
			var runData = new VectoRunData() {
				GearboxData = new GearboxData {
					Gears = new Dictionary<uint, GearData> {
						{ 1, new GearData { Ratio = 2.0 } },
						{ 2, new GearData { Ratio = 3.5 } }
					}
				},
				VehicleData = new VehicleData {
                    DynamicTyreRadius = 0.5.SI<Meter>()
				},
				AxleGearData = new AxleGearData {
					AxleGear = new TransmissionData {
						Ratio = 2.3
					}
				},
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>()
			};

			var container = VehicleContainer.CreateVehicleContainer(runData, null, null);
			var inputData = @"<t>,<Pwheel>,<gear>,<n>,<Padd>
							   1,89,2,1748,1.300
							   2,120,2,1400,0.4";

			var cycleFile = new MemoryStream(Encoding.UTF8.GetBytes(inputData));
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycleFile, CycleType.PWheel, "", false);

			var gearbox = new CycleGearbox(container);

			var cycle = new PWheelCycle(container, drivingCycle);
			cycle.Connect(new MockTnOutPort());

			cycle.Initialize();
			
			Assert.AreEqual(container.DrivingCycleInfo.CycleData.LeftSample.Time, 1.SI<Second>());
			Assert.AreEqual(container.DrivingCycleInfo.CycleData.RightSample.Time, 2.SI<Second>());

			Assert.AreEqual(1748.RPMtoRad() / (2.3 * 3.5), container.DrivingCycleInfo.CycleData.LeftSample.WheelAngularVelocity);
			Assert.AreEqual(1400.RPMtoRad() / (2.3 * 3.5), container.DrivingCycleInfo.CycleData.RightSample.WheelAngularVelocity);

			Assert.AreEqual(89.SI(Unit.SI.Kilo.Watt), container.DrivingCycleInfo.CycleData.LeftSample.PWheel);
			Assert.AreEqual(120.SI(Unit.SI.Kilo.Watt), container.DrivingCycleInfo.CycleData.RightSample.PWheel);

			Assert.AreEqual(2u, container.DrivingCycleInfo.CycleData.LeftSample.Gear);
			Assert.AreEqual(2u, container.DrivingCycleInfo.CycleData.RightSample.Gear);

			Assert.AreEqual(1300.SI<Watt>(), container.DrivingCycleInfo.CycleData.LeftSample.AdditionalAuxPowerDemand);
			Assert.AreEqual(400.SI<Watt>(), container.DrivingCycleInfo.CycleData.RightSample.AdditionalAuxPowerDemand);

			Assert.AreEqual(89.SI(Unit.SI.Kilo.Watt) / (1748.RPMtoRad() / (2.3 * 3.5)), container.DrivingCycleInfo.CycleData.LeftSample.Torque);
			Assert.AreEqual(120.SI(Unit.SI.Kilo.Watt) / (1400.RPMtoRad() / (2.3 * 3.5)), container.DrivingCycleInfo.CycleData.RightSample.Torque);
		}

		/// <summary>
		/// Tests if the powertrain can be created in P_wheel_in mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestCase]
		public void Pwheel_CreatePowertrain_Test()
		{
			// prepare input data
			var inputData = @"<t>,<Pwheel>,<gear>,<n>,  <Padd>
							   1,  89,      2,     1748, 1.3
							   2,  120,     2,     1400, 0.4";

			var cycleFile = new MemoryStream(Encoding.UTF8.GetBytes(inputData));
			var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycleFile, CycleType.PWheel, "", false);

			var fuelConsumption = new DataTable();
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Rows.Add("1", "1", "1");
			fuelConsumption.Rows.Add("2", "2", "2");
			fuelConsumption.Rows.Add("3", "3", "3");

			var fullLoad = new DataTable();
			fullLoad.Columns.Add("Engine speed");
			fullLoad.Columns.Add("max torque");
			fullLoad.Columns.Add("drag torque");
			fullLoad.Columns.Add("PT1");
			fullLoad.Rows.Add("0", "5000", "-5000", "0");
			fullLoad.Rows.Add("3000", "5000", "-5000", "0");

			var fullLoadCurve = FullLoadCurveReader.Create(fullLoad);
			var data = new VectoRunData {
				Cycle = drivingCycle,
				AxleGearData = new AxleGearData { AxleGear = new GearData { Ratio = 2.3 } },
				EngineData =
					new CombustionEngineData {
						IdleSpeed = 560.RPMtoRad(),
						Inertia = 1.SI<KilogramSquareMeter>(),
						EngineStartTime = DeclarationData.Engine.DefaultEngineStartTime,
						FullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>() { { 0, fullLoadCurve }, { 1, fullLoadCurve } },
						Fuels = new List<CombustionEngineFuelData>() {
							new CombustionEngineFuelData() {
								FuelData = FuelData.Diesel
							}
						}
					},
				GearboxData = new GearboxData { Gears = new Dictionary<uint, GearData> { { 2, new GearData { Ratio = 3.5 } } } },
				Retarder = new RetarderData(),
				DriverData = new DriverData() {
					EngineStopStart = new DriverData.EngineStopStartData() {
						UtilityFactorStandstill = DeclarationData.Driver.GetEngineStopStartLorry().UtilityFactor,
						EngineOffStandStillActivationDelay = DeclarationData.Driver.GetEngineStopStartLorry().ActivationDelay,
						MaxEngineOffTimespan = DeclarationData.Driver.GetEngineStopStartLorry().MaxEngineOffTimespan
					}
				}
			};

			// call builder (actual test)
			var jobContainer = PowertrainBuilder.Build(data, new MockModalDataContainer(), new MockSumWriter());
		}

		/// <summary>
		/// Tests if the simulation works and the modfile and sumfile are correct in P_wheel_in mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[TestCase]
		public void Pwheel_Run_Test()
		{
			var jobFile = @"TestData/Pwheel/Pwheel.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			ResultFileHelper.TestSumFile(@"TestData/Pwheel/Results/Pwheel.vsum", @"TestData/Pwheel/Pwheel.vsum");

			ResultFileHelper.TestModFile(@"TestData/Pwheel/Results/Pwheel_Gear2_pt1_rep1_actual.vmod",
				@"TestData/Pwheel/Pwheel_Gear2_pt1_rep1_actual.vmod");
		}

		/// <summary>
		/// Tests if the simulation works and the modfile and sumfile are correct in P_wheel_in mode.
		/// </summary>
		/// <remarks>VECTO-177</remarks>
		[Category("LongRunning")]
		[TestCase]
		public void Pwheel_ultimate_Run_Test()
		{
			var jobFile = @"TestData/Pwheel/Pwheel_ultimate.vecto";
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();

			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));

			ResultFileHelper.TestSumFile(@"TestData/Pwheel/Results/Pwheel_ultimate.vsum", @"TestData/Pwheel/Pwheel_ultimate.vsum");

			ResultFileHelper.TestModFile(@"TestData/Pwheel/Results/Pwheel_ultimate_RD_#1_Pwheel_AuxStd.vmod",
				@"TestData/Pwheel/Pwheel_ultimate_RD_#1_Pwheel_AuxStd.vmod");
		}
	}
}