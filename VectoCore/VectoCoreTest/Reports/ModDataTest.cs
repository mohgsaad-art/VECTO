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
using System.Data;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;
using System.IO;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Reports
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ModDataTest
	{

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(80, 0),
			TestCase(80, -0.1),
			TestCase(10, 0.1),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SumDataTest(double initialSpeedVal, double accVal)
		{
			var rundata = new VectoRunData() {
				JobName = "sumDataTest",
				Cycle = new DrivingCycleData() {
					CycleType = CycleType.DistanceBased
				}
			};
			var modData = new ModalDataContainer(rundata, null, null);
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
			modData.Data.CreateColumns(ModalResults.DriverSignals);
			var initialSpeed = initialSpeedVal.KMPHtoMeterPerSecond();
			var speed = initialSpeed;
			var dist = 0.SI<Meter>();
			var dt = 0.5.SI<Second>();
			var acc = accVal.SI<MeterPerSquareSecond>();
			for (var i = 0; i < 100; i++) {
				modData[ModalResultField.v_act] = speed;
				modData[ModalResultField.simulationInterval] = dt;
				modData[ModalResultField.acc] = acc;
				dist += speed * dt + acc * dt * dt / 2.0;
				speed += acc * dt;
				modData[ModalResultField.dist] = dist;
				modData.CommitSimulationStep();
			}

			// distance = 80km/h * 50s + acc/2 * 50s * 50s
			var totalTime = 50.SI<Second>();
			var expected = initialSpeed * totalTime + acc / 2.0 * totalTime * totalTime;

			Assert.AreEqual(expected.Value(), modData.Distance.Value(), 1e-6);
		}


		[TestCase()]
		public void ModDataIntegritySimpleTest()
		{
			var cycleData = new[] {
				// <s>,<v>,<grad>,<stop>
				"  0,  20, 0,    0",
				" 100, 60, 0,    0",
				"1000, 60, 0,    0",
				"1500, 40, 1,    0",
				"2000, 50,-1,    0",
				"2500,  0, 0,    2"
			};
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var sumData = new SummaryDataContainer(null);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck_ModDataIntegrity.vmod");

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(Truck40tPowerTrain.EngineFile, 0);

			// get a reference to the mod-data because the modaldata container clears it after simulation
			var modData = ((ModalDataContainer)run.GetContainer().ModalData).Data;
			var auxKeys = ((ModalDataContainer)run.GetContainer().ModalData).Auxiliaries;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			AssertModDataIntegrity(modData, auxKeys, cycle.Entries.Last().Distance.Value(), engineData.Fuels.First().ConsumptionMap, true, run.GetContainer().RunData);
		}

		[Category("LongRunning")]
		[TestCase(@"TestData/Integration/DeclarationMode/Class2_RigidTruck_4x2/Class2_RigidTruck_DECL.vecto")]
		public void TestFullCycleModDataIntegrityDeclMT(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Declaration);
		}

		[Category("LongRunning")]
		[TestCase(@"TestData/Integration/DeclarationMode/Class2_RigidTruck_4x2_ESS/Class2_RigidTruck_DECL.vecto")]
		public void TestFullCycleModDataIntegrityDeclESS(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Declaration);
		}

		[Category("LongRunning")]
		[TestCase(@"TestData/Integration/EngineeringMode/P1_Group5_AMT/P1_Group5_s2c0_rep_Payload.vecto")]
		public void TestFullCycleModDataIntegrityDecl_P1ESS(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Engineering);
		}

		[Category("LongRunning")]
		[TestCase]
		public void Test_P1_PCC_ESSOn_EssOff()
		{
			var jobName = @"TestData/Integration/EngineeringMode/P1_Group5_AMT/P1_Group5_s2c0_rep_Payload.vecto";
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);
			runsFactory.WriteModalResults = false;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			var run = runsFactory.SimulationRuns().First();
			var modESSon = (run.GetContainer().ModalData as ModalDataContainer).Data;
			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			jobName = @"TestData/Integration/EngineeringMode/P1_Group5_AMT/P1_Group5_s2c0_rep_Payload_ESSoff.vecto";
			fileWriter = new FileOutputWriter(jobName);
			inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);
			runsFactory.WriteModalResults = false;
			runsFactory.Validate = false;

			jobContainer.AddRuns(runsFactory);

			run = runsFactory.SimulationRuns().First();
			var modESSoff = (run.GetContainer().ModalData as ModalDataContainer).Data;
			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			// ICEOn is sometimes 0, therefore the sum must be lower than the row count
			Assert.That(modESSon.Sum(r => (bool)r[ModalResultField.ICEOn.GetName()] ? 1 : 0) < modESSon.Rows.Count);

			// ICEOn must always be 1, therefore the sum should equal the row count
			Assert.That(modESSoff.Sum(r => (bool)r[ModalResultField.ICEOn.GetName()] ? 1 : 0) == modESSoff.Rows.Count);
		}





		[TestCase(@"TestData/Integration/DeclarationMode/Class2_RigidTruck_4x2/Class2_RigidTruck_DECL.vecto")]
		public void TestVSUM_VMOD_FormatDecl(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Declaration);

			var tmpWriter = new FileOutputWriter(jobName);

			AssertModDataFormat(tmpWriter.GetModDataFileName(Path.GetFileNameWithoutExtension(jobName), "LongHaul", "ReferenceLoad"));
			AssertSumDataFormat(tmpWriter.SumFileName);
		}

		[Category("LongRunning")]
		[TestCase(@"TestData/Integration/EngineeringMode/Class2_RigidTruck_4x2/Class2_RigidTruck_ENG.vecto"),
		TestCase(@"TestData/Integration/EngineeringMode/Class5_Tractor_4x2/Class5_Tractor_ENG.vecto"),
		TestCase(@"TestData/Integration/EngineeringMode/Class9_RigidTruck_6x2_PTO/Class9_RigidTruck_ENG_PTO.vecto"),]
		public void TestFullCycleModDataIntegrityMT(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Engineering);
		}

		[TestCase(@"TestData/XML/XMLReaderDeclaration/Tractor_4x2_vehicle-class-5_5_t_0.xml", 1, 1.0),
		TestCase(@"TestData/XML/XMLReaderDeclaration/Tractor_4x2_vehicle-class-5_5_t_0.xml", 7, 1.0)
			]
		public void TractionInterruptionTest(string filename, int idx, double expectedTractionInterruption)
		{
			var writer = new FileOutputWriter(filename);
			var inputData = xmlInputReader.CreateDeclaration(filename);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, writer);
			factory.WriteModalResults = true;
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			var run = jobContainer.Runs[idx];
			var modData = ((ModalDataContainer)run.Run.GetContainer().ModalData).Data;

			run.Run.Run();

			//Assert.IsTrue(run.Done);
			//Assert.IsTrue(run.Success);
			Assert.IsTrue(modData.Rows.Count > 0);

			var tractionInterruptionTimes = ExtractTractionInterruptionTimes(modData);

			var min = tractionInterruptionTimes.Values.Min();
			//var max = tractionInterruptionTimes.Values.Max();

			Console.WriteLine("number of traction interruption intervals: {0}", tractionInterruptionTimes.Count);
			var exceedingHigh = tractionInterruptionTimes.Where(x => x.Value.IsGreater(expectedTractionInterruption, 0.1)).ToList();
			var exceedingLow = tractionInterruptionTimes.Where(x => x.Value.IsSmaller(expectedTractionInterruption, 0.1)).ToList();
			Console.WriteLine("number of traction interruption times exceeding specified interval: {0}", exceedingHigh.Count());
			if (exceedingHigh.Count > 0) {
				foreach (var e in exceedingHigh) {
					Console.WriteLine("{0} : {1}", e.Key, e.Value);
				}
			}
			if (exceedingLow.Count > 0) {
				foreach (var e in exceedingLow) {
					Console.WriteLine("{0} : {1}", e.Key, e.Value);
				}
			}

			Assert.IsTrue(exceedingHigh.Count < 5);
			var max2 = tractionInterruptionTimes.Values.OrderBy(x => x.Value()).Reverse().Skip(5).Max();
			var min2 = tractionInterruptionTimes.Values.OrderBy(x => x.Value()).Skip(5).Min();

			Assert.IsTrue(min2.IsEqual(expectedTractionInterruption, 0.1), "minimum traction interruption time: {0}", min);
			Assert.IsTrue(max2.IsEqual(expectedTractionInterruption, 0.1), "maximum traction interruption time: {0}", max2);


		}

		private Dictionary<Second, Second> ExtractTractionInterruptionTimes(ModalResults modData)
		{
			var retVal = new Dictionary<Second, Second>();

			Second tracStart = null;
			foreach (DataRow row in modData.Rows) {
				var velocity = (MeterPerSecond)row[ModalResultField.v_act.GetName()];
				if (velocity.IsEqual(0)) {
					tracStart = null;
					continue;
				}

				var gear = (uint)row[ModalResultField.Gear.GetName()];
				var absTime = (Second)row[ModalResultField.time.GetName()];
				var dt = (Second)row[ModalResultField.simulationInterval.GetName()];
				if (gear == 0 && tracStart == null) {
					tracStart = absTime - dt / 2.0;
				}
				if (gear != 0 && tracStart != null) {
					var tracEnd = absTime - dt / 2.0;
					retVal[absTime] = (tracEnd - tracStart);
					tracStart = null;
				}
			}

			return retVal;
		}


		[Category("LongRunning")]
		[TestCase(@"TestData/Integration/VTPMode/GenericVehicle/class_5_generic vehicle.vecto")]
		public void TestVTPModeDataIntegrity(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Engineering);
		}

		private void AssertModDataFormat(string modFilename)
		{
			var lineCnt = 0;
			var columnCount = -1;
			var ignoreColumns = new HashSet<int>();
			foreach (var line in File.ReadLines(modFilename)) {
				lineCnt++;
				if (lineCnt == 2) {
					var header = line.Split(',').ToList();
					columnCount = header.Count;
					ignoreColumns.Add(header.FindIndex(x => x.StartsWith("Gear")));
					ignoreColumns.Add(header.FindIndex(x => x.StartsWith("ICE On")));
					ignoreColumns.Add(header.FindIndex(x => x.StartsWith("DriverAction")));
					ignoreColumns.Add(header.FindIndex(x => x.StartsWith("EcoRollConditionsMet")));
					ignoreColumns.Add(header.FindIndex(x => x.StartsWith("PCCSegment")));
					ignoreColumns.Add(header.FindIndex(x => x.StartsWith("PCCState")));
				}
				if (lineCnt <= 2) {
					continue;
				}
				var parts = line.Split(',');
				for (var i = 0; i < columnCount; i++) {
					if (ignoreColumns.Contains(i) || string.IsNullOrWhiteSpace(parts[i])) {
						continue;
					}
					var numParts = parts[i].Split('.');
					Assert.AreEqual(2, numParts.Length, $"Line {lineCnt}: column {i}: value {parts[i]}");
					Assert.IsTrue(numParts[0].Length > 0);
					Assert.AreEqual(4, numParts[1].Length);
				}
			}
		}

		private void AssertSumDataFormat(string sumFilename)
		{
			//var first = 2;
			//var sumContainer = new SummaryDataContainer(null);
			var ranges = new[] {
				Tuple.Create(SumDataFields.SPEED, SumDataFields.BRAKING_TIME_SHARE)
			};
			var lineCnt = 0;
			List<string> header = new List<string>();
			foreach (var line in File.ReadLines(sumFilename)) {
				if (lineCnt == 2) {
					header = line.Split(',').ToList();
				}
				if (lineCnt <= 2) {
					continue;
				}
				var parts = line.Split(',');
				foreach (var range in ranges) {
					for (var i = header.FindIndex(x => x.StartsWith(range.Item1));
						i <= header.FindIndex(x => x. StartsWith(range.Item2));
						i++) {
						if (i >= parts.Length || string.IsNullOrWhiteSpace(parts[i])) {
							continue;
						}

						var numParts = parts[i].Split('.');
						Assert.AreEqual(2, numParts.Length);
						Assert.IsTrue(numParts[0].Length > 0);
						Assert.AreEqual(4, numParts[1].Length);
					}
				}
			}
		}

		private static void RunSimulation(string jobName, ExecutionMode mode)
		{
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

			var runsFactory = SimulatorFactory.CreateSimulatorFactory(mode, inputData, fileWriter);
			runsFactory.WriteModalResults = true;

			jobContainer.AddRuns(runsFactory);
			var modData = new List<Tuple<ModalResults, double>>();
			VectoRunData runData = null;
			foreach (var run in jobContainer.Runs) {
				var distanceCycle = ((VehicleContainer)run.Run.GetContainer()).DrivingCycleInfo as DistanceBasedDrivingCycle;
				if (distanceCycle != null) {
					modData.Add(Tuple.Create(((ModalDataContainer)run.Run.GetContainer().ModalData).Data,
						distanceCycle.Data.Entries.Last().Distance.Value()));
				}
				var cycle = ((VehicleContainer)run.Run.GetContainer()).DrivingCycleInfo as PowertrainDrivingCycle;
				if (cycle != null)
					modData.Add(Tuple.Create(((ModalDataContainer)run.Run.GetContainer().ModalData).Data,
						cycle.Data.Entries.Last().Time.Value()));
				if (runData == null) {
					runData = run.Run.GetContainer().RunData;
				}
			}
			var auxKeys =
				new Dictionary<string, DataColumn>(
					((ModalDataContainer)jobContainer.Runs.First().Run.GetContainer().ModalData).Auxiliaries);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			// mod files will be stored in e.g. 
			// VectoCoreTest/bin/Debug/TestData/Integration/EngineeringMode/Class2_RigidTruck_4x2/Class2_RigidTruck_ENG.vecto_00.vmod
			//fileWriter.WriteModData(Path.GetFileName(jobName), "0", "0", modData[0].Item1);
			//fileWriter.WriteModData(Path.GetFileName(jobName), "1", "1", modData[1].Item1);

			var engInput = inputData as IEngineeringInputDataProvider;
			FuelConsumptionMap fcMap = null;
			if (engInput != null) {
				fcMap = FuelConsumptionMapReader.Create(engInput.JobInputData.Vehicle
				   .Components.EngineInputData.EngineModes.First().Fuels.First().FuelConsumptionMap);
			}
			var vtpInput = inputData as IVTPEngineeringInputDataProvider;
			if (vtpInput != null) {
				fcMap = FuelConsumptionMapReader.Create(vtpInput.JobInputData.Vehicle
					.Components.EngineInputData.EngineModes.First().Fuels.First().FuelConsumptionMap);
			}
			var disatanceBased =
				((VehicleContainer)(jobContainer.Runs.First().Run.GetContainer())).DrivingCycleInfo is DistanceBasedDrivingCycle;
			var em = jobContainer.Runs.First().Run.GetContainer().RunData.ElectricMachinesData;
			foreach (var modalResults in modData) {
				if (em.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
					AssertModDataIntegrityP1(modalResults.Item1, auxKeys, modalResults.Item2, fcMap, disatanceBased, runData);
				} else {
					AssertModDataIntegrity(modalResults.Item1, auxKeys, modalResults.Item2, fcMap, disatanceBased, runData);
				}
			}

			AssertSumDataIntegrity(sumData, mode, disatanceBased, runData);
		}

		private static void AssertSumDataIntegrity(SummaryDataContainer sumData, ExecutionMode mode,
			bool distanceBased, VectoRunData runData)
		{
			Assert.IsTrue(sumData.Table.Rows.Count > 0);

			var ptoTransmissionColumn =
				sumData.Table.Columns.Contains(string.Format(SumDataFields.E_FORMAT,
					Constants.Auxiliaries.IDs.PTOTransmission))
					? string.Format(SumDataFields.E_FORMAT, Constants.Auxiliaries.IDs.PTOTransmission)
					: null;
			var ptoConsumerColumn =
				sumData.Table.Columns.Contains(string.Format(SumDataFields.E_FORMAT, Constants.Auxiliaries.IDs.PTOConsumer))
					? string.Format(SumDataFields.E_FORMAT, Constants.Auxiliaries.IDs.PTOConsumer)
					: null;

			var emPos = EnumHelper.GetValues<PowertrainPosition>().FirstOrDefault(p =>
				sumData.Table.Columns.Contains(string.Format(SumDataFields.EM_AVG_SPEED_FORMAT, p.GetName())));
			var emDriveCol = emPos != PowertrainPosition.HybridPositionNotSet
				? string.Format(string.Format(SumDataFields.E_EM_DRIVE_FORMAT, emPos.GetName()))
				: null;
			var emRecupCol = emPos != PowertrainPosition.HybridPositionNotSet
				? string.Format(SumDataFields.E_EM_GENERATE_FORMAT, emPos.GetName())
				: null;
			var emDragCol = emPos != PowertrainPosition.HybridPositionNotSet
				? string.Format(SumDataFields.E_EM_OFF_Loss_Format, emPos.GetName())
				: null;

			foreach (DataRow row in sumData.Table.Rows) {
				var inputFile = row[SumDataFields.INPUTFILE].ToString();
				var cycle = row[SumDataFields.CYCLE].ToString();
				var loading = row[SumDataFields.LOADING].ToString();
				var eFcMapPos = ((ConvertedSI)row[SumDataFields.E_FCMAP_POS]);
				var eFcMapNeg = ((ConvertedSI)row[SumDataFields.E_FCMAP_NEG]);
				var ePowertrainInertia = distanceBased ? ((ConvertedSI)row[SumDataFields.E_POWERTRAIN_INERTIA]) : new ConvertedSI(0, "");
				var eAux = ((ConvertedSI)row[SumDataFields.E_AUX]);
				var eClutchLoss = runData.GearboxData.Type.AutomaticTransmission() ? new ConvertedSI(0, "") : ((ConvertedSI)row[SumDataFields.E_CLUTCH_LOSS]);
				var eTcLoss = row.Table.Columns.Contains(SumDataFields.E_TC_LOSS) ? ((ConvertedSI)row[SumDataFields.E_TC_LOSS]) : new ConvertedSI(0, "");
				//var eShiftLoss = ((SI)row[SummaryDataContainer.E_SHIFT_LOSS]);
				var eGbxLoss = ((ConvertedSI)row[SumDataFields.E_GBX_LOSS]);
				var eRetLoss = runData.Retarder.Type.IsDedicatedComponent() ? ((ConvertedSI)row[SumDataFields.E_RET_LOSS]) : new ConvertedSI(0, "");
				var eAngleLoss = row.Table.Columns.Contains(SumDataFields.E_ANGLE_LOSS) ? ((ConvertedSI)row[SumDataFields.E_ANGLE_LOSS]) : new ConvertedSI(0, "");
				var eAxlLoss = ((ConvertedSI)row[SumDataFields.E_AXL_LOSS]);
				var eBrakeLoss = distanceBased ? ((ConvertedSI)row[SumDataFields.E_BRAKE]) : new ConvertedSI(0, "");
				var eVehInertia = distanceBased ? ((ConvertedSI)row[SumDataFields.E_VEHICLE_INERTIA]): new ConvertedSI(0, "");
				var eWheel = !distanceBased ? ((ConvertedSI)row[SumDataFields.E_WHEEL]) : null;
				var eAir = distanceBased ? ((ConvertedSI)row[SumDataFields.E_AIR]) : new ConvertedSI(0, "");
				var eRoll = distanceBased ?((ConvertedSI)row[SumDataFields.E_ROLL]) : new ConvertedSI(0, "");
				var eGrad = distanceBased ? ((ConvertedSI)row[SumDataFields.E_GRAD]) : new ConvertedSI(0, "");
				var cargoVolume = mode == ExecutionMode.Engineering ? 0.0 : ((ConvertedSI)row[SumDataFields.CARGO_VOLUME]);

				var loadingValue = ((ConvertedSI)row[SumDataFields.LOADING]) / 1000;
				var fcPer100km = distanceBased ? ((ConvertedSI)row[string.Format(SumDataFields.FCFINAL_LITERPER100KM, "")]) : null;
				var fcPerVolume = mode == ExecutionMode.Engineering
					? 0.0
					: ((ConvertedSI)row[string.Format(SumDataFields.FCFINAL_LiterPer100M3KM, "")]);
				var fcPerLoad = loadingValue > 0 ? ((ConvertedSI)row[string.Format(SumDataFields.FCFINAL_LITERPER100TKM, "")]) : 0.0;
				var co2PerKm = distanceBased ? ((ConvertedSI)row[SumDataFields.CO2_KM]) : null;
				var co2PerVolume = mode == ExecutionMode.Engineering ? 0.0 : ((ConvertedSI)row[SumDataFields.CO2_M3KM]);
				var co2PerLoad = loadingValue > 0 ? ((ConvertedSI)row[SumDataFields.CO2_TKM]) : 0.0;

				var ePTOtransm = ptoTransmissionColumn != null ? ((ConvertedSI)row[ptoTransmissionColumn]) : 0.0;
				var ePTOconsumer = ptoConsumerColumn != null ? ((ConvertedSI)row[ptoConsumerColumn]) : 0.0;

				var eEmDrive = emDriveCol != null ? ((ConvertedSI)row[emDriveCol]) : 0.0;
				var eEmRecup = emRecupCol != null ? ((ConvertedSI)row[emRecupCol]) : 0.0;
				var eEmDrag = emDragCol != null ? ((ConvertedSI)row[emDragCol]) : 0.0;


				if (distanceBased) {
					// E_fcmap_pos = E_fcmap_neg + E_powertrain_inertia + E_aux_xxx + E_aux_sum + E_clutch_loss + E_tc_loss + E_gbx_loss + E_shift_loss + E_ret_loss + E_angle_loss + E_axl_loss + E_brake + E_vehicle_inertia + E_air + E_roll + E_grad + E_PTO_CONSUM + E_PTO_TRANSM
					Assert.AreEqual(eFcMapPos,
						eFcMapNeg + ePowertrainInertia + eAux + eClutchLoss + eTcLoss + eGbxLoss + eRetLoss + eAngleLoss +
						eAxlLoss + eBrakeLoss + eVehInertia + eAir + eRoll + eGrad + ePTOconsumer + ePTOtransm - eEmDrive + eEmRecup + eEmDrag, 1e-5,
						"input file: {0}  cycle: {1} loading: {2}",
						inputFile, cycle, loading);
				} else {
					// E_fcmap_pos = E_fcmap_neg + E_powertrain_inertia + E_aux_xxx + E_aux_sum + E_clutch_loss + E_tc_loss + E_gbx_loss + E_shift_loss + E_ret_loss + E_angle_loss + E_axl_loss + E_brake + E_vehicle_inertia + E_wheel + E_PTO_CONSUM + E_PTO_TRANSM
					Assert.AreEqual(eFcMapPos,
						eFcMapNeg + ePowertrainInertia + eAux + eClutchLoss + eTcLoss + eGbxLoss + eRetLoss + eAngleLoss +
						eAxlLoss + eBrakeLoss + eVehInertia + eWheel + ePTOconsumer + ePTOtransm, 1e-5,
						"input file: {0}  cycle: {1} loading: {2}",
						inputFile, cycle, loading);
				}
				var pFcmapPos = ((ConvertedSI)row[SumDataFields.P_FCMAP_POS]);
				var time = ((ConvertedSI)row[SumDataFields.TIME]);

				// E_fcmap_pos = P_fcmap_pos * t
				Assert.AreEqual(eFcMapPos, pFcmapPos * (time / 3600), 1e-3, "input file: {0}  cycle: {1} loading: {2}", inputFile,
					cycle, loading);

				if (distanceBased && cargoVolume > 0) {
					Assert.AreEqual(fcPerVolume, fcPer100km / cargoVolume, 1e-3, "input file: {0}  cycle: {1} loading: {2}", inputFile,
						cycle, loading);

					Assert.AreEqual(co2PerVolume, co2PerKm / cargoVolume, 1e-3, "input file: {0}  cycle: {1} loading: {2}",
						inputFile,
						cycle, loading);
				}

				if (distanceBased && loadingValue > 0) {
					Assert.AreEqual(co2PerLoad, co2PerKm / loadingValue, 1e-3, "input file: {0}  cycle: {1} loading: {2}",
						inputFile, cycle, loading);
					Assert.AreEqual(fcPerLoad, fcPer100km / loadingValue, 1e-3, "input file: {0}  cycle: {1} loading: {2}",
						inputFile, cycle, loading);
				}

				var stopTimeShare = ((ConvertedSI)row[SumDataFields.STOP_TIMESHARE]);
				var accTimeShare = ((ConvertedSI)row[SumDataFields.ACC_TIMESHARE]);
				var decTimeShare = ((ConvertedSI)row[SumDataFields.DEC_TIMESHARE]);
				var cruiseTimeShare = ((ConvertedSI)row[SumDataFields.CRUISE_TIMESHARE]);

				Assert.AreEqual(100, stopTimeShare + accTimeShare + decTimeShare + cruiseTimeShare, 1e-3,
					"input file: {0}  cycle: {1} loading: {2}", inputFile, cycle, loading);

				if (distanceBased) {
					Assert.IsTrue(((ConvertedSI)row[SumDataFields.ACC_POS]) > 0);
					Assert.IsTrue(((ConvertedSI)row[SumDataFields.ACC_NEG]) < 0);
				}
				var gearshifts = ((ConvertedSI)row[SumDataFields.NUM_GEARSHIFTS]);
				Assert.IsTrue(gearshifts > 0);

				//var acc = ((SI)row[SummaryDataContainer.ACC]).Value();
			}
		}

		private static void AssertModDataIntegrity(ModalResults modData, Dictionary<string, DataColumn> auxKeys,
			double totalDistance, IFuelConsumptionMap consumptionMap, bool distanceBased, VectoRunData runData)
		{
			Assert.IsTrue(modData.Rows.Count > 0);

			var ptoTransmissionColumn = auxKeys.GetVECTOValueOrDefault(Constants.Auxiliaries.IDs.PTOTransmission);
			var ptoConsumerColumn = auxKeys.GetVECTOValueOrDefault(Constants.Auxiliaries.IDs.PTOConsumer);
			foreach (DataRow row in modData.Rows) {
				if (distanceBased && totalDistance.IsEqual(((Meter)row[ModalResultField.dist.GetName()]).Value())) {
					continue;
				}
				var gear = (uint)row[ModalResultField.Gear.GetName()];
				var time = (Second)row[ModalResultField.time.GetName()];

				Meter distance = 0.SI<Meter>();
				if (distanceBased) {
					distance = (Meter)row[ModalResultField.dist.GetName()];
				}
				var tqEngFcmap = (NewtonMeter)row[ModalResultField.T_ice_fcmap.GetName()];
				var nEngFcMap = (PerSecond)row[ModalResultField.n_ice_avg.GetName()];

				// check fuel consumption interpolation
				var fuelConsumption = (SI)row[ModalResultField.FCMap.GetName()];
				Assert.AreEqual(fuelConsumption.Value(),
					consumptionMap.GetFuelConsumption(tqEngFcmap, nEngFcMap, true).Value.Value(), 1E-3, "time: {0}  distance: {1}",
					time, distance);

				// check P_eng_FCmap = T_eng_fcmap * n_eng
				var pEngFcmap = (SI)row[ModalResultField.P_ice_fcmap.GetName()];
				Assert.AreEqual(pEngFcmap.Value(), (tqEngFcmap * nEngFcMap).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				var pWheelIn = (Watt)row[ModalResultField.P_wheel_in.GetName()];
				var pAir = distanceBased ? (Watt)row[ModalResultField.P_air.GetName()] : 0.SI<Watt>();
				var pRoll = distanceBased ? (Watt)row[ModalResultField.P_roll.GetName()] : 0.SI<Watt>();
				var pGrad = distanceBased ? (Watt)row[ModalResultField.P_slope.GetName()] : 0.SI<Watt>();
				var pVehInertia = distanceBased ? (Watt)row[ModalResultField.P_veh_inertia.GetName()] : 0.SI<Watt>();
				var pTrac = distanceBased ? (Watt)row[ModalResultField.P_trac.GetName()] : pWheelIn;

				// P_﻿eng_out = P﻿_wheel + P_loss﻿gearbox + P_loss﻿axle + P_loss﻿retarder + P_a﻿gbx + Pa_﻿eng + P_aux_mech - P_brake_loss
				var pEngOut = (Watt)row[ModalResultField.P_ice_out.GetName()];
				var pLossGbx = (Watt)row[ModalResultField.P_gbx_loss.GetName()];
				var pGbxIn = (Watt)row[ModalResultField.P_gbx_in.GetName()];
				var pLossAxle = (Watt)row[ModalResultField.P_axle_loss.GetName()];
				var pLossAngle = !row.Table.Columns.Contains(ModalResultField.P_angle_loss.GetName()) || row[ModalResultField.P_angle_loss.GetName()] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ModalResultField.P_angle_loss.GetName()];
				var pAxleIn = (Watt)row[ModalResultField.P_axle_in.GetName()];
				var pLossRet = !row.Table.Columns.Contains(ModalResultField.P_ret_loss.GetName()) || row[ModalResultField.P_ret_loss.GetName()] is DBNull ? 0.SI<Watt>() : (Watt)row[ModalResultField.P_ret_loss.GetName()];
				var pRetIn = !row.Table.Columns.Contains(ModalResultField.P_ret_loss.GetName()) || row[ModalResultField.P_retarder_in.GetName()] is DBNull ? pAxleIn : (Watt)row[ModalResultField.P_retarder_in.GetName()];
				var pGbxInertia = (Watt)row[ModalResultField.P_gbx_inertia.GetName()];
				var pShiftLoss = runData.GearboxData.Type.ManualTransmission() || row[ModalResultField.P_gbx_shift_loss.GetName()] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ModalResultField.P_gbx_shift_loss.GetName()];
				var pEngInertia = (Watt)row[ModalResultField.P_ice_inertia.GetName()];
				var pAux =
					(Watt)(row[ModalResultField.P_aux_mech.GetName()] != DBNull.Value ? row[ModalResultField.P_aux_mech.GetName()] : 0.SI<Watt>());
				var pBrakeLoss = distanceBased ? (Watt)row[ModalResultField.P_brake_loss.GetName()] : 0.SI<Watt>();
				var pBrakeIn = distanceBased ? (Watt)row[ModalResultField.P_brake_in.GetName()] : pWheelIn;

				var pWheelInertia = distanceBased ? (Watt)row[ModalResultField.P_wheel_inertia.GetName()] : 0.SI<Watt>();
				var pPTOconsumer = ptoConsumerColumn == null || row[ptoConsumerColumn.ColumnName] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoConsumerColumn.ColumnName];
				var pPTOtransm = ptoTransmissionColumn == null || row[ptoTransmissionColumn.ColumnName] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoTransmissionColumn.ColumnName];

				if (distanceBased) {
					// P_trac = P_veh_inertia + P_roll + P_air + P_slope
					Assert.AreEqual(pTrac.Value(), (pAir + pRoll + pGrad + pVehInertia).Value(), 1E-3, "time: {0}  distance: {1}",
						time,
						distance);
				}

				if (distanceBased) {
					// P_wheel_in = P_trac + P_wheel_inertia
					Assert.AreEqual(pWheelIn.Value(), (pTrac + pWheelInertia).Value(), 1E-3, "time: {0}  distance: {1}", time,
						distance);
				}
				Assert.AreEqual(pBrakeIn.Value(), (pWheelIn + pBrakeLoss).Value(), 1E-3, "time: {0}  distance: {1}", time,
						distance);

				Assert.AreEqual(pAxleIn.Value(), (pBrakeIn + pLossAxle).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pRetIn.Value(), (pAxleIn + pLossRet).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				var pClutchLoss = (Watt)(row[ModalResultField.P_clutch_loss.GetName()] != DBNull.Value
					? row[ModalResultField.P_clutch_loss.GetName()]
					: 0.SI<Watt>());

				var pClutchOut = row[ModalResultField.P_clutch_out.GetName()];
				if (pClutchOut != DBNull.Value) {
					Assert.AreEqual(pGbxIn.Value(), (pClutchOut as Watt).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);
					Assert.AreEqual(pEngOut.Value(), ((pClutchOut as Watt) + pClutchLoss).Value(), 1E-3, "time: {0}  distance: {1}",
						time, distance);
				}

				var pTC_Loss = row.Table.Columns.Contains(ModalResultField.P_TC_loss.GetName()) && row[ModalResultField.P_TC_loss.GetName()] != DBNull.Value
					? (Watt)row[ModalResultField.P_TC_loss.GetName()]
					: 0.SI<Watt>();

				var pTCOut = row[ModalResultField.P_clutch_out.GetName()];
				if (pTCOut != DBNull.Value) {
					Assert.AreEqual(pGbxIn.Value(), (pTCOut as Watt).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);
					//Assert.AreEqual(pEngOut.Value(), (pTCOut as Watt + pTC_Loss).Value(), 1E-3, "time: {0}  distance: {1}",
					//	time, distance);
				}

				Assert.IsTrue(pLossGbx.IsGreaterOrEqual(pShiftLoss + pGbxInertia), "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pGbxIn.Value(), (pRetIn + pLossGbx + pGbxInertia).Value(), gear != 0 ? 1E-3 : 0.5,
					"time: {0}  distance: {1}", time,
					distance);

				// P_eng_fcmap = P_eng_out + P_AUX + P_eng_inertia ( + P_PTO_Transm + P_PTO_Consumer )
				Assert.AreEqual(pEngFcmap.Value(), (pEngOut + pAux + pEngInertia + pPTOtransm + pPTOconsumer).Value(), 0.5,
					"time: {0}  distance: {1}", time, distance);

				// P_eng_fcmap = sum(Losses Powertrain)
				var pLossTot = pClutchLoss + pTC_Loss + pLossGbx + pLossRet + pGbxInertia + pLossAngle + pLossAxle + (distanceBased ? (pBrakeLoss +
								pWheelInertia + pAir + pRoll + pGrad + pVehInertia) : pTrac) + pPTOconsumer + pPTOtransm;
				var pEngFcmapCalc = (pLossTot + pEngInertia + pAux).Value();
				Assert.AreEqual(pEngFcmap.Value(), pEngFcmapCalc, 0.5, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pEngFcmap.Value(),
					(pTrac + pWheelInertia + pBrakeLoss + pLossAxle + pLossRet + pLossGbx + pGbxInertia + pEngInertia + pAux +
					pClutchLoss + pTC_Loss + pPTOtransm + pPTOconsumer).Value(), 0.5, "time: {0}  distance: {1}", time, distance);
			}
		}


		private static void AssertModDataIntegrityP1(ModalResults modData, Dictionary<string, DataColumn> auxKeys,
			double totalDistance, FuelConsumptionMap consumptionMap, bool distanceBased, VectoRunData runData)
		{
			Assert.IsTrue(modData.Rows.Count > 0);

			var ptoTransmissionColumn = auxKeys.GetVECTOValueOrDefault(Constants.Auxiliaries.IDs.PTOTransmission);
			var ptoConsumerColumn = auxKeys.GetVECTOValueOrDefault(Constants.Auxiliaries.IDs.PTOConsumer);

			var p1OutColumn =
				modData.Columns[string.Format(ModalResultField.P_EM_out_.GetCaption(), PowertrainPosition.HybridP1.GetName())];
			Assert.NotNull(p1OutColumn);
			var p1InColumn =
				modData.Columns[string.Format(ModalResultField.P_EM_in_.GetCaption(), PowertrainPosition.HybridP1.GetName())];
			Assert.NotNull(p1InColumn);
			var p1LossColumn =
				modData.Columns[string.Format(ModalResultField.P_EM_loss_.GetCaption(), PowertrainPosition.HybridP1.GetName())];
			Assert.NotNull(p1LossColumn);
			var p1MechColumn =
				modData.Columns[string.Format(ModalResultField.P_EM_mech_.GetCaption(), PowertrainPosition.HybridP1.GetName())];
			Assert.NotNull(p1MechColumn);
			var p1ElColumn =
				modData.Columns[string.Format(ModalResultField.P_EM_electricMotor_el_.GetCaption(), PowertrainPosition.HybridP1.GetName())];
			Assert.NotNull(p1ElColumn);


			foreach (DataRow row in modData.Rows) {
				if (distanceBased && totalDistance.IsEqual(((Meter)row[ModalResultField.dist.GetName()]).Value())) {
					continue;
				}
				var gear = (uint)row[ModalResultField.Gear.GetName()];
				var time = (Second)row[ModalResultField.time.GetName()];

				Meter distance = 0.SI<Meter>();
				if (distanceBased) {
					distance = (Meter)row[ModalResultField.dist.GetName()];
				}
				var tqEngFcmap = (NewtonMeter)row[ModalResultField.T_ice_fcmap.GetName()];
				var nEngFcMap = (PerSecond)row[ModalResultField.n_ice_avg.GetName()];

				// check fuel consumption interpolation
				var fuelConsumption = (SI)row[ModalResultField.FCMap.GetName()];
				Assert.AreEqual(fuelConsumption.Value(),
					consumptionMap.GetFuelConsumption(tqEngFcmap, nEngFcMap, true).Value.Value(), 1E-3, "time: {0}  distance: {1}",
					time, distance);

				// check P_eng_FCmap = T_eng_fcmap * n_eng
				var pEngFcmap = (SI)row[ModalResultField.P_ice_fcmap.GetName()];
				Assert.AreEqual(pEngFcmap.Value(), (tqEngFcmap * nEngFcMap).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				var pWheelIn = (Watt)row[ModalResultField.P_wheel_in.GetName()];
				var pAir = distanceBased ? (Watt)row[ModalResultField.P_air.GetName()] : 0.SI<Watt>();
				var pRoll = distanceBased ? (Watt)row[ModalResultField.P_roll.GetName()] : 0.SI<Watt>();
				var pGrad = distanceBased ? (Watt)row[ModalResultField.P_slope.GetName()] : 0.SI<Watt>();
				var pVehInertia = distanceBased ? (Watt)row[ModalResultField.P_veh_inertia.GetName()] : 0.SI<Watt>();
				var pTrac = distanceBased ? (Watt)row[ModalResultField.P_trac.GetName()] : pWheelIn;

				// P_﻿eng_out = P﻿_wheel + P_loss﻿gearbox + P_loss﻿axle + P_loss﻿retarder + P_a﻿gbx + Pa_﻿eng + P_aux_mech - P_brake_loss
				var pEngOut = (Watt)row[ModalResultField.P_ice_out.GetName()];
				var pLossGbx = (Watt)row[ModalResultField.P_gbx_loss.GetName()];
				var pGbxIn = (Watt)row[ModalResultField.P_gbx_in.GetName()];
				var pLossAxle = (Watt)row[ModalResultField.P_axle_loss.GetName()];
				var pLossAngle = !row.Table.Columns.Contains(ModalResultField.P_angle_loss.GetName()) || row[ModalResultField.P_angle_loss.GetName()] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ModalResultField.P_angle_loss.GetName()];
				var pAxleIn = (Watt)row[ModalResultField.P_axle_in.GetName()];
				var pLossRet = (Watt)row[ModalResultField.P_ret_loss.GetName()];
				var pRetIn = (Watt)row[ModalResultField.P_retarder_in.GetName()];
				var pGbxInertia = (Watt)row[ModalResultField.P_gbx_inertia.GetName()];
				var pShiftLoss = runData.GearboxData.Type.ManualTransmission() || row[ModalResultField.P_gbx_shift_loss.GetName()] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ModalResultField.P_gbx_shift_loss.GetName()];
				var pEngInertia = (Watt)row[ModalResultField.P_ice_inertia.GetName()];
				var pAux =
					(Watt)(row[ModalResultField.P_aux_mech.GetName()] != DBNull.Value ? row[ModalResultField.P_aux_mech.GetName()] : 0.SI<Watt>());
				var pBrakeLoss = distanceBased ? (Watt)row[ModalResultField.P_brake_loss.GetName()] : 0.SI<Watt>();
				var pBrakeIn = distanceBased ? (Watt)row[ModalResultField.P_brake_in.GetName()] : pWheelIn;

				var pWheelInertia = distanceBased ? (Watt)row[ModalResultField.P_wheel_inertia.GetName()] : 0.SI<Watt>();
				var pPTOconsumer = ptoConsumerColumn == null || row[ptoConsumerColumn.ColumnName] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoConsumerColumn.ColumnName];
				var pPTOtransm = ptoTransmissionColumn == null || row[ptoTransmissionColumn.ColumnName] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoTransmissionColumn.ColumnName];

				if (distanceBased) {
					// P_trac = P_veh_inertia + P_roll + P_air + P_slope
					Assert.AreEqual(pTrac.Value(), (pAir + pRoll + pGrad + pVehInertia).Value(), 1E-3, "time: {0}  distance: {1}",
						time,
						distance);
				}

				if (distanceBased) {
					// P_wheel_in = P_trac + P_wheel_inertia
					Assert.AreEqual(pWheelIn.Value(), (pTrac + pWheelInertia).Value(), 1E-3, "time: {0}  distance: {1}", time,
						distance);
				}
				Assert.AreEqual(pBrakeIn.Value(), (pWheelIn + pBrakeLoss).Value(), 1E-3, "time: {0}  distance: {1}", time,
						distance);

				Assert.AreEqual(pAxleIn.Value(), (pBrakeIn + pLossAxle).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pRetIn.Value(), (pAxleIn + pLossRet).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				var pClutchLoss = (Watt)(row[ModalResultField.P_clutch_loss.GetName()] != DBNull.Value
					? row[ModalResultField.P_clutch_loss.GetName()]
					: 0.SI<Watt>());

				var pClutchOut = row[ModalResultField.P_clutch_out.GetName()];
				//if (pClutchOut != DBNull.Value) {
				Assert.AreEqual(pGbxIn.Value(), (pClutchOut as Watt).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);
				var pP1Out = row[p1OutColumn] as Watt;
				Assert.AreEqual(pP1Out.Value(), ((pClutchOut as Watt) + pClutchLoss).Value(), 1E-3, "time: {0}  distance: {1}",
					time, distance);

				var pP1Loss = row[p1LossColumn] as Watt;
				var pP1Mech = row[p1MechColumn] as Watt;
				var pP1In = row[p1InColumn] as Watt;
				var pP1El = row[p1ElColumn] as Watt;
				Assert.AreEqual(pP1In.Value(), (pP1Out + pP1Mech).Value(), 1E-3, "time: {0}  distance: {1}",
					time, distance);

				Assert.AreEqual(pP1Loss.Value(), (pP1In - pP1Out - pP1El).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				//}

				var pTC_Loss = row.Table.Columns.Contains(ModalResultField.P_TC_loss.GetName()) && row[ModalResultField.P_TC_loss.GetName()] != DBNull.Value
					? (Watt)row[ModalResultField.P_TC_loss.GetName()]
					: 0.SI<Watt>();

				var pTCOut = row[ModalResultField.P_clutch_out.GetName()];
				if (pTCOut != DBNull.Value) {
					Assert.AreEqual(pGbxIn.Value(), (pTCOut as Watt).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);
					//Assert.AreEqual(pEngOut.Value(), (pTCOut as Watt + pTC_Loss).Value(), 1E-3, "time: {0}  distance: {1}",
					//	time, distance);
				}

				Assert.IsTrue(pLossGbx.IsGreaterOrEqual(pShiftLoss + pGbxInertia), "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pGbxIn.Value(), (pRetIn + pLossGbx + pGbxInertia).Value(), gear != 0 ? 1E-3 : 0.5,
					"time: {0}  distance: {1}", time,
					distance);

				// P_eng_fcmap = P_eng_out + P_AUX + P_eng_inertia ( + P_PTO_Transm + P_PTO_Consumer )
				Assert.AreEqual(pEngFcmap.Value(), (pEngOut + pAux + pEngInertia + pPTOtransm + pPTOconsumer).Value(), 0.5,
					"time: {0}  distance: {1}", time, distance);

				// P_eng_fcmap = sum(Losses Powertrain)
				var pLossTot = pClutchLoss + pTC_Loss + pLossGbx + pLossRet + pGbxInertia + pLossAngle + pLossAxle + (distanceBased ? (pBrakeLoss +
								pWheelInertia + pAir + pRoll + pGrad + pVehInertia) : pTrac) + pPTOconsumer + pPTOtransm + pP1Mech;
				var pEngFcmapCalc = (pLossTot + pEngInertia + pAux).Value();
				Assert.AreEqual(pEngFcmap.Value(), pEngFcmapCalc, 0.5, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pEngFcmap.Value(),
					(pTrac + pWheelInertia + pBrakeLoss + pLossAxle + pLossRet + pLossGbx + pGbxInertia + pEngInertia + pAux +
					pClutchLoss + pTC_Loss + pPTOtransm + pPTOconsumer + pP1Mech).Value(), 0.5, "time: {0}  distance: {1}", time, distance);
			}
		}



		[Category("LongRunning")]
		[
			TestCase(@"TestData/Integration/EngineeringMode/CityBus_AT/CityBus_AT_Ser.vecto"),
			TestCase(@"TestData/Integration/EngineeringMode/CityBus_AT/CityBus_AT_PS.vecto")]
		public void TestFullCycleModDataIntegrityAT(string jobName)
		{
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

			var runsFactory =
				SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);
			runsFactory.WriteModalResults = true;

			jobContainer.AddRuns(runsFactory);
			var modData = new List<Tuple<ModalResults, Meter>>();
			VectoRunData runData = null;
			foreach (var run in jobContainer.Runs) {
				modData.Add(Tuple.Create(((ModalDataContainer)run.Run.GetContainer().ModalData).Data,
					((DistanceBasedDrivingCycle)((VehicleContainer)run.Run.GetContainer()).DrivingCycleInfo).Data.Entries.Last()
						.Distance));
				if (runData == null) {
					runData = run.Run.GetContainer().RunData;
				}
			}
			var auxKeys =
				new Dictionary<string, DataColumn>(
					((ModalDataContainer)jobContainer.Runs.First().Run.GetContainer().ModalData).Auxiliaries);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			foreach (var modalResults in modData) {
				AssertModDataIntegrityAT(
					modalResults.Item1, auxKeys, modalResults.Item2,
					FuelConsumptionMapReader.Create(
						((IEngineeringInputDataProvider)inputData)
						.JobInputData.Vehicle.Components.EngineInputData.EngineModes.First().Fuels.First().FuelConsumptionMap), true, runData);
			}

			AssertSumDataIntegrity(sumData, ExecutionMode.Engineering, true, runData);
		}

		private static void AssertModDataIntegrityAT(ModalResults modData, Dictionary<string, DataColumn> auxKeys,
			Meter totalDistance, FuelConsumptionMap consumptionMap, bool atGbx, VectoRunData runData)
		{
			Assert.IsTrue(modData.Rows.Count > 0);

			var ptoTransmissionColumn = auxKeys.GetVECTOValueOrDefault(Constants.Auxiliaries.IDs.PTOTransmission);
			var ptoConsumerColumn = auxKeys.GetVECTOValueOrDefault(Constants.Auxiliaries.IDs.PTOConsumer);
			foreach (DataRow row in modData.Rows) {
				if (totalDistance.IsEqual(((Meter)row[ModalResultField.dist.GetName()]))) {
					continue;
				}
				var gear = (uint)row[ModalResultField.Gear.GetName()];
				var time = (Second)row[ModalResultField.time.GetName()];

				var distance = (Meter)row[ModalResultField.dist.GetName()];
				var tqEngFcmap = (NewtonMeter)row[ModalResultField.T_ice_fcmap.GetName()];
				var nEngFcMap = (PerSecond)row[ModalResultField.n_ice_avg.GetName()];

				// check fuel consumption interpolation
				var fuelConsumption = (SI)row[ModalResultField.FCMap.GetName()];
				Assert.AreEqual(fuelConsumption.Value(),
					consumptionMap.GetFuelConsumption(tqEngFcmap, nEngFcMap, false).Value.Value(), 1E-3, "time: {0}  distance: {1}",
					time, distance);

				// check P_eng_FCmap = T_eng_fcmap * n_eng
				var pEngFcmap = (SI)row[ModalResultField.P_ice_fcmap.GetName()];
				Assert.AreEqual(pEngFcmap.Value(), (tqEngFcmap * nEngFcMap).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				var pWheelIn = (Watt)row[ModalResultField.P_wheel_in.GetName()];
				var pAir = (Watt)row[ModalResultField.P_air.GetName()];
				var pRoll = (Watt)row[ModalResultField.P_roll.GetName()];
				var pGrad = (Watt)row[ModalResultField.P_slope.GetName()];
				var pVehInertia = (Watt)row[ModalResultField.P_veh_inertia.GetName()];
				var pTrac = (Watt)row[ModalResultField.P_trac.GetName()];

				// Pe_﻿eng = P﻿_wheel + P_loss﻿gearbox + P_loss﻿axle + P_loss﻿retarder + P_a﻿gbx + Pa_﻿eng + P_aux_mech - P_brake_loss
				var pEngOut = (Watt)row[ModalResultField.P_ice_out.GetName()];
				var pLossGbx = (Watt)row[ModalResultField.P_gbx_loss.GetName()];
				var pGbxIn = (Watt)row[ModalResultField.P_gbx_in.GetName()];
				var pLossAxle = (Watt)row[ModalResultField.P_axle_loss.GetName()];
				var pLossAngle = runData.AngledriveData?.Type != AngledriveType.SeparateAngledrive
					? 0.SI<Watt>()
					: (Watt)row[ModalResultField.P_angle_loss.GetName()];
				var pAxleIn = (Watt)row[ModalResultField.P_axle_in.GetName()];
				var pLossRet = runData.Retarder.Type.IsDedicatedComponent() ? (Watt)row[ModalResultField.P_ret_loss.GetName()] : 0.SI<Watt>();
				var pRetIn = runData.Retarder.Type.IsDedicatedComponent() ? (Watt)row[ModalResultField.P_retarder_in.GetName()] : pAxleIn;
				var pGbxInertia = (Watt)row[ModalResultField.P_gbx_inertia.GetName()];
				var pShiftLoss = row[ModalResultField.P_gbx_shift_loss.GetName()] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ModalResultField.P_gbx_shift_loss.GetName()];
				var pEngInertia = (Watt)row[ModalResultField.P_ice_inertia.GetName()];
				var pAux =
					(Watt)(row[ModalResultField.P_aux_mech.GetName()] != DBNull.Value ? row[ModalResultField.P_aux_mech.GetName()] : 0.SI<Watt>());
				var pBrakeLoss = (Watt)row[ModalResultField.P_brake_loss.GetName()];
				var pBrakeIn = (Watt)row[ModalResultField.P_brake_in.GetName()];
				var pTcLoss = (Watt)row[ModalResultField.P_TC_loss.GetName()];
				var pTcOut = (Watt)row[ModalResultField.P_TC_out.GetName()];
				var pWheelInertia = (Watt)row[ModalResultField.P_wheel_inertia.GetName()];
				var pPTOconsumer = ptoConsumerColumn == null || row[ptoConsumerColumn] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoConsumerColumn];
				var pPTOtransm = ptoTransmissionColumn == null || row[ptoTransmissionColumn] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoTransmissionColumn];
				// P_trac = P_veh_inertia + P_roll + P_air + P_slope
				Assert.AreEqual(pTrac.Value(), (pAir + pRoll + pGrad + pVehInertia).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				// P_wheel_in = P_trac + P_wheel_inertia
				Assert.AreEqual(pWheelIn.Value(), (pTrac + pWheelInertia).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pBrakeIn.Value(), (pWheelIn + pBrakeLoss).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pAxleIn.Value(), (pBrakeIn + pLossAxle).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pRetIn.Value(), (pAxleIn + pLossRet).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pGbxIn.Value(), pTcOut.Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pEngOut.Value(), (pTcOut + pTcLoss).Value(), 1E-3,
					"time: {0}  distance: {1}", time, distance);

				// P_eng_fcmap = P_eng_out + P_AUX + P_eng_inertia ( + P_PTO_Transm + P_PTO_Consumer )
				Assert.AreEqual(pEngFcmap.Value(), (pEngOut + pAux + pEngInertia + pPTOtransm + pPTOconsumer).Value(), atGbx ? 1E-1 : 1e-3,
					"time: {0}  distance: {1}", time,
					distance);

				// P_eng_fcmap = sum(Losses Powertrain)
				var pLossTot = pTcLoss + pLossGbx + pLossRet + pGbxInertia + pLossAngle + pLossAxle + pBrakeLoss +
								pWheelInertia + pAir + pRoll + pGrad + pVehInertia + pPTOconsumer + pPTOtransm;

				Assert.AreEqual(pEngFcmap.Value(), (pLossTot + pEngInertia + pAux).Value(), atGbx ? 1E-1 : 1e-3, "time: {0}  distance: {1}", time,
					distance);

				Assert.IsTrue(pLossGbx.IsGreaterOrEqual(pShiftLoss + pGbxInertia, 0.5.SI<Watt>()), "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pGbxIn.Value(), (pRetIn + pLossGbx + pGbxInertia).Value(), gear != 0 ? 1E-3 : 0.5,
					"time: {0}  distance: {1}", time,
					distance);
				Assert.AreEqual(pEngFcmap.Value(),
					(pTrac + pWheelInertia + pBrakeLoss + pLossAxle + pLossRet + pLossGbx + pGbxInertia + pEngInertia + pAux +
					pTcLoss + pPTOtransm + pPTOconsumer).Value(), 0.5, "time: {0}  distance: {1}", time, distance);
			}
		}
	}
}
