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
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Tests.Utils;
using System.IO;

namespace TUGraz.VectoCore.Tests.Integration.DriverStrategy
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class SimpleCycles
	{
		[OneTimeSetUp]
		public void Init()
		{
			//LogManager.DisableLogging();

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
			graphWriter.Xfields = new[] { ModalResultField.time, ModalResultField.dist };

			graphWriter.Yfields = new[] {
				ModalResultField.v_act, ModalResultField.acc, ModalResultField.n_ice_avg, ModalResultField.Gear,
				ModalResultField.P_ice_out, ModalResultField.T_ice_fcmap, ModalResultField.FCMap
			};
			graphWriter.Series1Label = "Vecto 3";
			graphWriter.Series2Label = "Vecto 2.2";
			return graphWriter;
		}

		private static string GetSlopeString(double slope)
		{
			var slopeStr = slope > 0
				? Math.Abs(slope).ToString("uphill_#")
				: slope < 0
					? Math.Abs(slope).ToString("downhill_#")
					: "level";
			return slopeStr;
		}

		[TestCase, Category("ComparisonV2")]
		public void TestGraph()
		{
			var imgV3 = @"TestData\Results\Integration\40t_Long_Haul_Truck_Cycle_Drive_50_Dec_Increasing_Slope_v3.vmod";
			var imgv22 =
				@"TestData\Results\Integration\40t_Long_Haul_Truck_Cycle_Drive_50_Dec_Increasing_Slope_v22.vmod";

			GetGraphWriter().Write(imgV3, imgv22);
		}

		[TestCase, Category("ComparisonV2")]
		public void TestSingleGraph()
		{
			var imgV3 = @"TestData\Results\Integration\40t_Long_Haul_Truck_Cycle_Drive_50_Dec_Increasing_Slope_v3.vmod";

			GetGraphWriter().Write(imgV3);
		}

		[Category("ComparisonV2"),
		TestCase(0, 20, -5),
		TestCase(0, 20, 0),
		TestCase(0, 85, -15),
		TestCase(0, 85, -25),
		TestCase(0, 85, -5),
		TestCase(0, 85, 0),
		TestCase(0, 85, 1),
		TestCase(0, 85, 10),
		TestCase(0, 85, 2),
		TestCase(0, 85, 25),
		TestCase(0, 85, 5),
		TestCase(20, 22, 5),
		TestCase(20, 60, -15),
		TestCase(20, 60, -25),
		TestCase(20, 60, -5),
		TestCase(20, 60, 0),
		TestCase(20, 60, 15),
		TestCase(20, 60, 25),
		TestCase(20, 60, 5),
		]
		public void Truck_Accelerate(double v1, double v2, double slope)
		{
			Assert.IsTrue(v2 > v1);
			var cycle = string.Format(CultureInfo.InvariantCulture,
				"0, {0}, {1}, {2}\n100, {3}, {4}, 0\n1000, {3}, {4}, {5}", v1,
				slope, v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			Truck_Special(cycle,
				string.Format(CultureInfo.InvariantCulture, "40t_Long_Haul_Truck_Cycle_Accelerate_{0}_{1}_{2}.vmod",
					v1, v2, GetSlopeString(slope)));
		}

		[Test,
		TestCase(20, 60, 25),
		]
		public void Truck_Accelerate_MT(double v1, double v2, double slope)
		{
			Assert.IsTrue(v2 > v1);
			var cycle = string.Format(CultureInfo.InvariantCulture,
				"0, {0}, {1}, {2}\n100, {3}, {4}, 0\n1000, {3}, {4}, {5}", v1,
				slope, v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			Truck_Special(cycle,
				string.Format(CultureInfo.InvariantCulture, "40t_Long_Haul_Truck_Cycle_Accelerate_MT_{0}_{1}_{2}.vmod",
					v1, v2, GetSlopeString(slope)), gbxType: GearboxType.MT);
		}

		[Category("ComparisonV2"),
		TestCase(22, 20, -5),
		TestCase(45, 0, -5),
		TestCase(45, 0, 0),
		TestCase(45, 0, 5),
		TestCase(60, 20, -15),
		TestCase(60, 20, -25),
		TestCase(60, 20, -5),
		TestCase(60, 20, 0),
		TestCase(60, 20, 15),
		TestCase(60, 20, 5),
		TestCase(80, 0, -15),
		TestCase(80, 0, -25),
		TestCase(80, 0, -5),
		TestCase(80, 0, 0),
		//TestCase(80, 0, 18),
		TestCase(80, 0, 15),
		TestCase(80, 0, 3),
		TestCase(80, 0, 5),
		]
		public void Truck_Decelerate(double v1, double v2, double slope)
		{
			Assert.IsTrue(v2 < v1);
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			Truck_Special(cycle,
				string.Format(CultureInfo.InvariantCulture, "40t_Long_Haul_Truck_Cycle_Decelerate_{0}_{1}_{2}.vmod",
					v1, v2, GetSlopeString(slope)));
		}

		[Category("ComparisonV2"),
		TestCase(10, 10, -15),
		TestCase(10, 10, -25),
		TestCase(10, 10, -5),
		TestCase(10, 10, 0),
		TestCase(10, 10, 15),
		TestCase(10, 10, 25),
		TestCase(10, 10, 5),
		TestCase(20, 20, -15),
		TestCase(30, 30, -15),
		TestCase(50, 50, -15),
		TestCase(80, 80, -15),
		TestCase(80, 80, -5),
		TestCase(80, 80, 0),
		TestCase(80, 80, 15),
		TestCase(80, 80, 5),
		]
		public void Truck_Drive(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			Truck_Special(cycle,
				string.Format(CultureInfo.InvariantCulture, "40t_Long_Haul_Truck_Cycle_Drive_{0}_{1}_{2}.vmod",
					v1, v2, GetSlopeString(slope)));
		}

		[Category("ComparisonV2"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Increasing_Slope,
			"40t_Long_Haul_Truck_Cycle_Drive_80_Increasing_Slope.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDrive_80_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Increasing_Slope,
			"40t_Long_Haul_Truck_Cycle_Drive_50_Increasing_Slope.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDrive_50_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Increasing_Slope,
			"40t_Long_Haul_Truck_Cycle_Drive_30_Increasing_Slope.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDrive_30_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Decreasing_Slope,
			"40t_Long_Haul_Truck_Cycle_Drive_80_Decreasing_Slope.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDrive_80_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Decreasing_Slope,
			"40t_Long_Haul_Truck_Cycle_Drive_50_Decreasing_Slope.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDrive_50_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Decreasing_Slope,
			"40t_Long_Haul_Truck_Cycle_Drive_30_Decreasing_Slope.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDrive_30_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Dec_Increasing_Slope,
			"40t_Long_Haul_Truck_Cycle_Drive_80_Dec_Increasing_Slope.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDrive_80_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Dec_Increasing_Slope,
			"40t_Long_Haul_Truck_Cycle_Drive_50_Dec_Increasing_Slope.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDrive_50_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Dec_Increasing_Slope,
			"40t_Long_Haul_Truck_Cycle_Drive_30_Dec_Increasing_Slope.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDrive_30_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDecelerateWhileBrake_80_0_level,
			"40t_Long_Haul_Truck_Cycle_DecelerateWhileBrake_80_0_level.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleDecelerateWhileBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateWhileBrake_80_0_level,
			"40t_Long_Haul_Truck_Cycle_AccelerateWhileBrake_80_0_level.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleAccelerateWhileBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateAtBrake_80_0_level,
			"40t_Long_Haul_Truck_Cycle_AccelerateAtBrake_80_0_level.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleAccelerateAtBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateBeforeBrake_80_0_level,
			"40t_Long_Haul_Truck_Cycle_AccelerateBeforeBrake_80_0_level.vmod", GearboxType.AMT,
			TestName = "TruckSpecial CycleAccelerateBeforeBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleDrive_stop_85_stop_85_level,
			"24t Truck_Cycle_Drive_stop_85_stop_85_level.vmod",
			GearboxType.AMT, TestName = "TruckSpecial CycleDrive_stop_85_stop_85_level"),
		TestCase(SimpleDrivingCycles.CycleDrive_SlopeChangeBeforeStop,
			"Truck_DriverStrategy_SlopeChangeBeforeStop.vmod",
			GearboxType.AMT, TestName = "TruckSpecial CycleDrive_SlopeChangeBeforeStop"),
		TestCase(SimpleDrivingCycles.CycleDriver_FrequentSlopChange, "Truck_DriverStrategy_SlopeChangeBeforeStop.vmod",
			GearboxType.AMT, TestName = "TruckSpecial CycleDriver_FrequentSlopChange"),
		]
		public void Truck_Special(string cycleData, string modFileName, GearboxType gbxType = GearboxType.AMT)
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, modFileName, gbxType: gbxType);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GetGraphWriter().Write(modFileName,
				@"..\..\TestData\Integration\DriverStrategy\Vecto2.2\40t Truck\" + modFileName);
		}

		[Category("ComparisonV2"),
		TestCase(0, 40, -1),
		TestCase(0, 40, -3),
		TestCase(0, 40, -5),
		TestCase(0, 60, -1),
		TestCase(0, 60, -3),
		TestCase(0, 60, -5),
		TestCase(0, 85, -1),
		TestCase(0, 85, -3),
		TestCase(0, 85, -5),
		]
		public void Truck_Accelerate_Overspeed(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			var slopeStr = slope > 0 ? "uhpill" : slope < 0 ? "downhill" : "level";
			string modFileName = string.Format(CultureInfo.InvariantCulture,
				"40t_Long_Haul_Truck_Cycle_Accelerate_{0}_{1}_{2}.vmod", v1, v2, slopeStr);

			var cycleData = SimpleDrivingCycles.CreateCycleData(cycle);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycleData, modFileName, true);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GetGraphWriter().Write(modFileName,
				@"..\..\TestData\Integration\DriverStrategy\Vecto2.2\40t Truck_Overspeed\" + modFileName);
		}

		[Category("ComparisonV2"),
		TestCase(0, 20, -5),
		TestCase(0, 20, 0),
		TestCase(0, 85, -15),
		TestCase(0, 85, -25),
		TestCase(0, 85, -5),
		TestCase(0, 85, 0),
		TestCase(0, 85, 1),
		TestCase(0, 85, 10),
		TestCase(0, 85, 2),
		//TestCase(0, 85, 25),
		TestCase(0, 85, 5),
		TestCase(20, 22, 5),
		TestCase(20, 60, -15),
		TestCase(20, 60, -25),
		TestCase(20, 60, -5),
		TestCase(20, 60, 0),
		TestCase(20, 60, 15),
		TestCase(20, 60, 25),
		TestCase(20, 60, 5),
		]
		public void Coach_Accelerate(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			Coach_Special(cycle,
				string.Format(CultureInfo.InvariantCulture, "24t Coach_Cycle_Accelerate_{0}_{1}_{2}.vmod",
					v1, v2, GetSlopeString(slope)));
		}

		[Category("ComparisonV2"),
		TestCase(22, 20, -5),
		TestCase(45, 0, -5),
		TestCase(45, 0, 0),
		TestCase(45, 0, 5),
		TestCase(60, 20, -15),
		TestCase(60, 20, -25),
		TestCase(60, 20, -5),
		TestCase(60, 20, 0),
		TestCase(60, 20, 15),
		TestCase(60, 20, 5),
		TestCase(80, 0, -15),
		TestCase(80, 0, -25),
		TestCase(80, 0, -5),
		TestCase(80, 0, 0),
		TestCase(80, 0, 20),
		TestCase(80, 0, 15),
		TestCase(80, 0, 3),
		TestCase(80, 0, 5),
		]
		public void Coach_Decelerate(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			Coach_Special(cycle,
				string.Format(CultureInfo.InvariantCulture, "24t Coach_Cycle_Decelerate_{0}_{1}_{2}.vmod",
					v1, v2, GetSlopeString(slope)));
		}

		[Category("ComparisonV2"),
		TestCase(10, 10, -15),
		TestCase(10, 10, -25),
		TestCase(10, 10, -5),
		TestCase(10, 10, 0),
		TestCase(10, 10, 15),
		TestCase(10, 10, 5),
		TestCase(20, 20, -15),
		TestCase(30, 30, -15),
		TestCase(50, 50, -15),
		TestCase(80, 80, -15),
		TestCase(80, 80, -5),
		TestCase(80, 80, 0),
		TestCase(80, 80, 15),
		TestCase(80, 80, 5),
		]
		public void Coach_Drive(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			Coach(cycle, string.Format(CultureInfo.InvariantCulture, "24t Coach_Cycle_Drive_{0}_{1}_{2}.vmod",
				v1, v2, GetSlopeString(slope)), true);
		}

		[Category("ComparisonV2"),
		TestCase(10, 10, 25)
		]
		public void Coach_Drive_low(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			Coach(cycle, string.Format(CultureInfo.InvariantCulture, "24t Coach_Cycle_Drive_{0}_{1}_{2}.vmod",
				v1, v2, GetSlopeString(slope)), false);
		}

		[Category("ComparisonV2"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Increasing_Slope,
			"24t Coach_Cycle_Drive_80_Increasing_Slope.vmod", TestName = "CoachSpecial CycleDrive_80_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Increasing_Slope,
			"24t Coach_Cycle_Drive_50_Increasing_Slope.vmod", TestName = "CoachSpecial CycleDrive_50_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Increasing_Slope,
			"24t Coach_Cycle_Drive_30_Increasing_Slope.vmod", TestName = "CoachSpecial CycleDrive_30_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Decreasing_Slope,
			"24t Coach_Cycle_Drive_80_Decreasing_Slope.vmod", TestName = "CoachSpecial CycleDrive_80_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Decreasing_Slope,
			"24t Coach_Cycle_Drive_50_Decreasing_Slope.vmod", TestName = "CoachSpecial CycleDrive_50_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Decreasing_Slope,
			"24t Coach_Cycle_Drive_30_Decreasing_Slope.vmod", TestName = "CoachSpecial CycleDrive_30_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Dec_Increasing_Slope,
			"24t Coach_Cycle_Drive_80_Dec_Increasing_Slope.vmod",
			TestName = "CoachSpecial CycleDrive_80_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Dec_Increasing_Slope,
			"24t Coach_Cycle_Drive_50_Dec_Increasing_Slope.vmod",
			TestName = "CoachSpecial CycleDrive_50_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Dec_Increasing_Slope,
			"24t Coach_Cycle_Drive_30_Dec_Increasing_Slope.vmod",
			TestName = "CoachSpecialCycleDrive_30_Dec_Increasing_Slope "),
		TestCase(SimpleDrivingCycles.CycleDecelerateWhileBrake_80_0_level,
			"24t Coach_Cycle_DecelerateWhileBrake_80_0_level.vmod",
			TestName = "CoachSpecial CycleDecelerateWhileBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateWhileBrake_80_0_level,
			"24t Coach_Cycle_AccelerateWhileBrake_80_0_level.vmod",
			TestName = "CoachSpecial CycleAccelerateWhileBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateAtBrake_80_0_level,
			"24t Coach_Cycle_AccelerateAtBrake_80_0_level.vmod",
			TestName = "CoachSpecial CycleAccelerateAtBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateBeforeBrake_80_0_level,
			"24t Coach_Cycle_AccelerateBeforeBrake_80_0_level.vmod",
			TestName = "CoachSpecial CycleAccelerateBeforeBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleDrive_stop_85_stop_85_level,
			"24t Coach_Cycle_Drive_stop_85_stop_85_level.vmod",
			TestName = "CoachSpecial CycleDrive_stop_85_stop_85_level"),
		TestCase(SimpleDrivingCycles.CycleDrive_SlopeChangeBeforeStop,
			"24t Coach_DriverStrategy_SlopeChangeBeforeStop.vmod",
			TestName = "CoachSpecial CycleDrive_SlopeChangeBeforeStop"),
		TestCase(SimpleDrivingCycles.CycleDriver_FrequentSlopChange,
			"24t Coach_DriverStrategy_SlopeChangeBeforeStop.vmod",
			TestName = "CoachSpecial CycleDriver_FrequentSlopChange"),
		]
		public void Coach_Special(string cycleData, string modFileName)
		{
			Coach(cycleData, modFileName, true);
		}

		private void Coach(string cycleData, string modFileName, bool highEnginePower)
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = CoachPowerTrain.CreateEngineeringRun(cycle, modFileName, highEnginePower: highEnginePower);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GetGraphWriter().Write(modFileName, @"..\..\TestData\Integration\DriverStrategy\Vecto2.2\Coach\" + modFileName);
		}

		[Category("ComparisonV2"),
		TestCase(0, 40, -1),
		TestCase(0, 40, -3),
		TestCase(0, 40, -5),
		TestCase(0, 60, -1),
		TestCase(0, 60, -3),
		TestCase(0, 60, -5),
		TestCase(0, 85, -1),
		TestCase(0, 85, -3),
		TestCase(0, 85, -5),
		]
		public void Coach_Accelerate_Overspeed(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			var slopeStr = slope > 0 ? "uhpill" : slope < 0 ? "downhill" : "level";
			string modFileName = string.Format(CultureInfo.InvariantCulture,
				"24t Coach_Cycle_Accelerate_{0}_{1}_{2}.vmod", v1, v2, slopeStr);

			var cycleData = SimpleDrivingCycles.CreateCycleData(cycle);
			var run = CoachPowerTrain.CreateEngineeringRun(cycleData, modFileName, true);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GetGraphWriter().Write(modFileName,
				@"..\..\TestData\Integration\DriverStrategy\Vecto2.2\Coach_Overspeed\" + modFileName);
		}

		// ####################################################

		[Category("AT Gearbox"),
		TestCase(0, 20, -5),
		TestCase(0, 20, 0),
		TestCase(0, 85, -15),
		TestCase(0, 85, -25),
		TestCase(0, 85, -5),
		TestCase(0, 85, 0),
		TestCase(0, 85, 1),
		TestCase(0, 85, 10),
		TestCase(0, 85, 2),
		//TestCase(0, 85, 25),
		TestCase(0, 85, 5),
		TestCase(20, 22, 5),
		TestCase(20, 60, -15),
		TestCase(20, 60, -25),
		TestCase(20, 60, -5),
		TestCase(20, 60, 0),
		TestCase(20, 60, 15),
		//TestCase(20, 60, 25),
		TestCase(20, 60, 5),
		]
		public void AT_Gearbox_Accelerate(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture,
				"0, {0}, {1}, {2}\n100, {3}, {4}, {5}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			AT_Gearbox_Special(cycle,
				string.Format(CultureInfo.InvariantCulture, "AT-Gbx Accelerate_{0}_{1}_{2}.vmod",
					v1, v2, GetSlopeString(slope)));
		}

		[Category("AT Gearbox"),
		TestCase(22, 20, -5),
		TestCase(45, 0, -5),
		TestCase(45, 0, 0),
		TestCase(45, 0, 5),
		TestCase(60, 20, -15),
		TestCase(60, 20, -25),
		TestCase(60, 20, -5),
		TestCase(60, 20, 0),
		//TestCase(60, 20, 15),
		TestCase(60, 20, 5),
		TestCase(80, 0, -15),
		TestCase(80, 0, -25),
		TestCase(80, 0, -5),
		TestCase(80, 0, 0),
		//TestCase(80, 0, 20),
		//TestCase(80, 0, 15),
		TestCase(80, 0, 3),
		TestCase(80, 0, 5),
		]
		public void AT_Gearbox_Decelerate(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			AT_Gearbox_Special(cycle,
				string.Format(CultureInfo.InvariantCulture, "AT-Gbx Decelerate_{0}_{1}_{2}.vmod",
					v1, v2, GetSlopeString(slope)));
		}

		[Category("AT Gearbox"),
		TestCase(10, 10, -15),
		TestCase(10, 10, -25),
		TestCase(10, 10, -5),
		TestCase(10, 10, 0),
		TestCase(10, 10, 15),
		TestCase(10, 10, 25),
		TestCase(10, 10, 5),
		TestCase(20, 20, -15),
		TestCase(30, 30, -15),
		TestCase(50, 50, -15),
		TestCase(80, 80, -15),
		TestCase(80, 80, -5),
		TestCase(80, 80, 0),
		//TestCase(80, 80, 15),
		TestCase(80, 80, 5),
		]
		public void AT_Gearbox_Drive(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			AT_Gearbox_Special(cycle, string.Format(CultureInfo.InvariantCulture, "AT-Gbx Drive_{0}_{1}_{2}.vmod",
				v1, v2, GetSlopeString(slope)));
		}

		[Category("AT Gearbox"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Increasing_Slope,
			"AT-Gbx Drive_80_Increasing_Slope.vmod", TestName = "AT-Gearbox CycleDrive_80_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Increasing_Slope,
			"AT-Gbx Drive_50_Increasing_Slope.vmod", TestName = "AT-Gearbox CycleDrive_50_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Increasing_Slope,
			"AT-Gbx Drive_30_Increasing_Slope.vmod", TestName = "AT-Gearbox CycleDrive_30_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Decreasing_Slope,
			"AT-Gbx Drive_80_Decreasing_Slope.vmod", TestName = "AT-Gearbox CycleDrive_80_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Decreasing_Slope,
			"AT-Gbx Drive_50_Decreasing_Slope.vmod", TestName = "AT-Gearbox CycleDrive_50_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Decreasing_Slope,
			"AT-Gbx Drive_30_Decreasing_Slope.vmod", TestName = "AT-Gearbox CycleDrive_30_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Dec_Increasing_Slope,
			"AT-Gbx Drive_80_Dec_Increasing_Slope.vmod", TestName = "AT-Gearbox CycleDrive_80_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Dec_Increasing_Slope,
			"AT-Gbx Drive_50_Dec_Increasing_Slope.vmod", TestName = "AT-Gearbox CycleDrive_50_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Dec_Increasing_Slope,
			"AT-Gbx Drive_30_Dec_Increasing_Slope.vmod", TestName = "AT-GearboxCycleDrive_30_Dec_Increasing_Slope "),
		TestCase(SimpleDrivingCycles.CycleDecelerateWhileBrake_80_0_level,
			"AT-Gbx DecelerateWhileBrake_80_0_level.vmod",
			TestName = "AT-Gearbox CycleDecelerateWhileBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateWhileBrake_80_0_level,
			"AT-Gbx AccelerateWhileBrake_80_0_level.vmod",
			TestName = "AT-Gearbox CycleAccelerateWhileBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateAtBrake_80_0_level,
			"AT-Gbx AccelerateAtBrake_80_0_level.vmod", TestName = "AT-Gearbox CycleAccelerateAtBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateBeforeBrake_80_0_level,
			"AT-Gbx AccelerateBeforeBrake_80_0_level.vmod",
			TestName = "AT-Gearbox CycleAccelerateBeforeBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleDrive_stop_85_stop_85_level, "AT-Gbx Drive_stop_85_stop_85_level.vmod",
			TestName = "AT-Gearbox CycleDrive_stop_85_stop_85_level"),
		TestCase(SimpleDrivingCycles.CycleDrive_SlopeChangeBeforeStop,
			"24t Coach_DriverStrategy_SlopeChangeBeforeStop.vmod",
			TestName = "AT-Gearbox CycleDrive_SlopeChangeBeforeStop"),
		TestCase(SimpleDrivingCycles.CycleDriver_FrequentSlopChange,
			"24t Coach_DriverStrategy_SlopeChangeBeforeStop.vmod",
			TestName = "AT-Gearbox CycleDriver_FrequentSlopChange"),
		]
		public void AT_Gearbox_Special(string cycleData, string modFileName)
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = ATPowerTrain.CreateEngineeringRun(cycle, GearboxType.ATSerial, modFileName);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			//GraphWriter.Write(modFileName, @"..\..\TestData\Integration\DriverStrategy\Vecto2.2\Coach\" + modFileName);
		}

		//[TestCase()]
		//public void HugoTest()
		//{
		//	//var source = @"E:\QUAM\Workspace\Projekt HUGO\Jobs generated\Tractor_4x4_vehicle-class-8_EURO6_2018_CO.xml";
		//	var source = @"E:\QUAM\Workspace\Projekt HUGO\Jobs generated\Rigid Truck_4x2_vehicle-class-4_EURO1_LH.xml";
		//	//@"E:\QUAM\Workspace\Projekt HUGO\Jobs generated\Rigid Truck_4x2_vehicle-class-4_EURO1_LH.xml";
		//	var writer = new FileOutputWriter(source);
		//	var apiRun = VectoEngineeringApi.VectoInstance(source, writer);
		//	try {
		//		apiRun.RunSimulation();
		//		var status = apiRun.GetProgress();
		//		foreach (var progressEntry in status) {
		//			if (!progressEntry.Value.Success) {
		//				Console.WriteLine("error executing run {0} in job {1}: error: {2}", progressEntry.Key, source,
		//					progressEntry.Value.Error.Message);
		//				Console.WriteLine(progressEntry.Value.Error.StackTrace);
		//			}
		//		}
		//		Assert.IsFalse(apiRun.GetProgress().Any(x => !x.Value.Success));
		//	} catch (Exception e) {
		//		Console.WriteLine("Simulation failed: " + e.Message);
		//	}
		//}
	}
}