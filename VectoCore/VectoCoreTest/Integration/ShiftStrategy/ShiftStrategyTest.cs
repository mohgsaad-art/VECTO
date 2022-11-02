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
using System.Globalization;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Tests.Utils;
using System.Collections.Generic;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;

namespace TUGraz.VectoCore.Tests.Integration.ShiftStrategy
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ShiftStrategyTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		protected GraphWriter GetGraphWriter() 
		{
			var graphWriter = new GraphWriter();
#if TRACE
			graphWriter.Enable();
#else
			graphWriter.Disable();
#endif

			graphWriter.Xfields = new[] { ModalResultField.dist };

			graphWriter.Yfields = new[] {
				ModalResultField.v_act, ModalResultField.acc, ModalResultField.n_ice_avg, ModalResultField.Gear,
				ModalResultField.P_ice_out, /*ModalResultField.T_eng_fcmap, */ ModalResultField.FCMap,
			};
			graphWriter.PlotDrivingMode = true;
			graphWriter.Series1Label = "Vecto 3";
			return graphWriter;
		}

		[Test,
		TestCase(75, 42.5, 4.5),
		TestCase(75, 42.5, 3.5),
		TestCase(75, 42.5, 2.1),
		TestCase(65, 42.5, 2.3),
		]
		public void Truck_Shifting_Test(double v1, double v2, double slope)
		{
			Assert.IsTrue(v1 > v2);

			var cycle = new[] {
				// <s>,<v>,<grad>,<stop>
				string.Format(CultureInfo.InvariantCulture, "  0,  {0}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1000, {1}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1100, {1},   0,  0", v1, v2, slope)
			};
			Directory.CreateDirectory(string.Format(@"Shiftt_{0}_{1}", v1, v2, slope));
			var slopePrefix = "";
			if (!slope.IsEqual(0)) {
				slopePrefix = slope > 0 ? "uh_" : "dh_";
			}
			var modFile = string.Format(@"Truck_Shift_{0}_{1}_{3}{2:0.0}.vmod", v1, v2, Math.Abs(slope),
				slopePrefix);
			var cycleData = SimpleDrivingCycles.CreateCycleData(cycle);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycleData, modFile, gbxType: GearboxType.MT);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GetGraphWriter().Write(modFile);
		}

		//[TestCase()]
		//public void TestGearshiftTrigger()
		//{
		//	var amtTestcase = @"E:\QUAM\tmp\AT_Vdrop\AMT_normal\MB_Citaro_G_MP156_ZF_Sort.vecto";
		//	var atTestcase = @"E:\QUAM\tmp\AT_Vdrop\AT_normal\MB_Citaro_G_MP156_ZF_Sort.vecto";


		//	var relativeJobPath = amtTestcase;
		//	var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(relativeJobPath), "tmp", Path.GetFileName(relativeJobPath)));
		//	var inputData =  JSONInputDataFactory.ReadJsonJob(relativeJobPath);
		//	var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
		//		WriteModalResults = true,
		//		//ActualModalData = true,
		//		Validate = false
		//	};
		//	var jobContainer = new JobContainer(new MockSumWriter());
		//	var runs = factory.SimulationRuns().ToArray();
		//	var run = runs[0];

		//	var container = run.GetContainer() as VehicleContainer;
		//	var vehicle = container?.VehicleInfo as Vehicle;

		//	Assert.NotNull(container);
		//	Assert.NotNull(vehicle);

		//	foreach (var preprocessor in container.Preprocessors) {
		//		preprocessor.RunPreprocessing();
		//	}

		//	var decision = new List<Tuple<double, IResponse, ResponseDryRun>>();
		//	for (var v = 15.0; v < 20; v += 0.1) {
		//		vehicle.Initialize(v.KMPHtoMeterPerSecond(), 0.SI<Radian>());
		//		container.AbsTime = 0.SI<Second>();
		//		(container.GearboxInfo as Gearbox).Gear = 2;
		//		//(container.Gearbox as ATGearbox)._strategy.NextGear.Gear = 0;
		//		(container.DriverInfo as Driver).DrivingAction = DrivingAction.Accelerate;
		//		(container.DriverInfo as Driver).DriverBehavior = DrivingBehavior.Accelerating;
		//		var response = vehicle.Request(
		//			0.SI<Second>(), 0.5.SI<Second>(), 0.5.SI<MeterPerSquareSecond>(), 0.SI<Radian>(), false);
		//		decision.Add(Tuple.Create(v, response, ((container.GearboxInfo as Gearbox)._strategy as AMTShiftStrategyOptimized).minFCResponse));
		//	}

		//	foreach (var tuple in decision) {
		//		var r = tuple.Item2;
		//		var s = tuple.Item3;
		//		var fc = r.Engine.EngineSpeed != null
		//			? container.RunData.EngineData.Fuels.First().ConsumptionMap.GetFuelConsumption(r.Engine.TotalTorqueDemand, r.Engine.EngineSpeed).Value
		//						.ConvertToGrammPerHour().Value
		//			: 0;
		//		var fc2 = s?.Engine.EngineSpeed != null
		//			? container.RunData.EngineData.Fuels.First().ConsumptionMap.GetFuelConsumption(s.Engine.TotalTorqueDemand, s.Engine.EngineSpeed).Value
		//						.ConvertToGrammPerHour().Value
		//			: 0;
		//		Console.WriteLine(
		//			"{0}; {1}; {2}; {3}; {4}; {5}; {6}; {7}", tuple.Item1, tuple.Item2 is ResponseGearShift ? "1" : "0",
		//			r.Engine.EngineSpeed?.AsRPM ?? 0,
		//			r.Engine.TorqueOutDemand?.Value() ?? 0, fc,
		//			s?.Engine.EngineSpeed?.AsRPM ?? 0,
		//			s?.Engine.TorqueOutDemand?.Value() ?? 0, fc2
		//			);
		//	}
		//}
	}
}