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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Tests.Utils;
using System.IO;

namespace TUGraz.VectoCore.Tests.Integration.BusAuxiliaries
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DriverStrategyTestCoachAux
	{
		[OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		private GraphWriter GetGraphWriter() 
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
			graphWriter.Series2Label = "Vecto 2.0_aux";
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

		[Category("LongRunning")]
		[Category("ComparisonAAUX"),
		TestCase(0, 20, -5), TestCase(0, 20, 0),
		TestCase(0, 40, 25), TestCase(0, 40, 20), TestCase(0, 40, 15),
		TestCase(0, 40, 10), TestCase(0, 40, 5), TestCase(0, 40, 1),
		TestCase(0, 40, 0), TestCase(0, 40, -3),
		TestCase(0, 40, -1), TestCase(0, 40, -5), TestCase(0, 40, -10),
		TestCase(0, 40, -15), TestCase(0, 40, -20), TestCase(0, 40, -25),
		TestCase(0, 60, 25), TestCase(0, 60, 20), TestCase(0, 60, 15),
		TestCase(0, 60, 10), TestCase(0, 60, 5), TestCase(0, 60, 1),
		TestCase(0, 60, 0), TestCase(0, 60, -3),
		TestCase(0, 60, -1), TestCase(0, 60, -5), TestCase(0, 60, -10),
		TestCase(0, 60, -15), TestCase(0, 60, -20), TestCase(0, 60, -25),
		TestCase(0, 85, 25), TestCase(0, 85, 20), TestCase(0, 85, 15),
		TestCase(0, 85, 10), TestCase(0, 85, 5), TestCase(0, 85, 1),
		TestCase(0, 85, 0), TestCase(0, 85, -3), TestCase(0, 85, 2),
		TestCase(0, 85, -1), TestCase(0, 85, -5), TestCase(0, 85, -10),
		TestCase(0, 85, -15), TestCase(0, 85, -20), TestCase(0, 85, -25),
		TestCase(20, 40, 25), TestCase(20, 40, 20), TestCase(20, 40, 15),
		TestCase(20, 40, 10), TestCase(20, 40, 5), TestCase(20, 40, 1),
		TestCase(20, 40, 0),
		TestCase(20, 40, -1), TestCase(20, 40, -5), TestCase(20, 40, -10),
		TestCase(20, 40, -15), TestCase(20, 40, -20), TestCase(20, 40, -25),
		TestCase(20, 60, 25), TestCase(20, 60, 20), TestCase(20, 60, 15),
		TestCase(20, 60, 10), TestCase(20, 60, 5), TestCase(20, 60, 1),
		TestCase(20, 60, 0),
		TestCase(20, 60, -1), TestCase(20, 60, -5), TestCase(20, 60, -10),
		TestCase(20, 60, -15), TestCase(20, 60, -20), TestCase(20, 60, -25),
		TestCase(20, 85, 25), TestCase(20, 85, 20), TestCase(20, 85, 15),
		TestCase(20, 85, 10), TestCase(20, 85, 5), TestCase(20, 85, 1),
		TestCase(20, 85, 0),
		TestCase(20, 85, -1), TestCase(20, 85, -5), TestCase(20, 85, -10),
		TestCase(20, 85, -15), TestCase(20, 85, -20), TestCase(20, 85, -25),
		TestCase(20, 22, 5),
		]
		public void Coach_Accelerate_AAux(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			var slopeStr = GetSlopeString(slope);
			var modFileName = string.Format(CultureInfo.InvariantCulture, @"CoachAAUX_{0}_{1}_{2}.vmod", v1, v2, slopeStr);

			Coach_BusAuxiliaries(cycle, modFileName,
				string.Format(CultureInfo.InvariantCulture, "24t Coach_AAux_Cycle_Accelerate_{0}_{1}_{2}.vmod", v1, v2, slopeStr));
		}

		[Category("LongRunning")]
		[Category("ComparisonAAUX"),
		TestCase(40, 0, 20), TestCase(40, 0, 15),
		TestCase(40, 0, 10), TestCase(40, 0, 5), TestCase(40, 0, 1),
		TestCase(40, 0, 0),
		TestCase(40, 0, -1), TestCase(40, 0, -5), TestCase(40, 0, -10),
		TestCase(40, 0, -15), TestCase(40, 0, -20), TestCase(40, 0, -25),
		TestCase(60, 0, 20), TestCase(60, 0, 15),
		TestCase(60, 0, 10), TestCase(60, 0, 5), TestCase(60, 0, 1),
		TestCase(60, 0, 0),
		TestCase(60, 0, -1), TestCase(60, 0, -5), TestCase(60, 0, -10),
		TestCase(60, 0, -15), TestCase(60, 0, -20), TestCase(60, 0, -25),
		TestCase(85, 0, 25), TestCase(85, 0, 20), TestCase(85, 0, 15),
		TestCase(85, 0, 10), TestCase(85, 0, 5), TestCase(85, 0, 1),
		TestCase(85, 0, 0),
		TestCase(85, 0, -1), TestCase(85, 0, -5), TestCase(85, 0, -10),
		TestCase(85, 0, -15), TestCase(85, 0, -20), TestCase(85, 0, -25),
		TestCase(80, 0, 3), TestCase(80, 0, 5), TestCase(80, 0, 15),
		TestCase(22, 20, -5),
		]
		public void Coach_Decelerate_AAux(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			var slopeStr = GetSlopeString(slope);
			var modFileName = string.Format(CultureInfo.InvariantCulture, @"CoachAAUX_{0}_{1}_{2}.vmod", v1, v2, slopeStr);

			Coach_BusAuxiliaries(cycle, modFileName,
				string.Format(CultureInfo.InvariantCulture, "24t Coach_AAux_Cycle_Decelerate_{0}_{1}_{2}.vmod", v1, v2, slopeStr));
		}

		[Category("ComparisonAAUX"),
		TestCase(40, 0, 25),
		TestCase(60, 0, 25),
		TestCase(80, 0, 25)]
		public void Coach_Decelerate_AAux_Low(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			var slopeStr = GetSlopeString(slope);
			var modFileName = string.Format(CultureInfo.InvariantCulture, @"CoachAAUX_{0}_{1}_{2}.vmod", v1, v2, slopeStr);

			Coach_BusAuxiliaries(cycle, modFileName,
				string.Format(CultureInfo.InvariantCulture, "24t Coach_AAux_Cycle_Decelerate_{0}_{1}_{2}.vmod", v1, v2, slopeStr),
				false);
		}

		[Category("LongRunning")]
		[Category("ComparisonAAUX"),
		TestCase(10, 10, 20), TestCase(10, 10, 15),
		TestCase(10, 10, 10), TestCase(10, 10, 5), TestCase(10, 10, 1),
		TestCase(10, 10, 0),
		TestCase(10, 10, -1), TestCase(10, 10, -5), TestCase(10, 10, -10),
		TestCase(10, 10, -15), TestCase(10, 10, -20), TestCase(10, 10, -25),
		TestCase(20, 20, 25), TestCase(20, 20, 20), TestCase(20, 20, 15),
		TestCase(20, 20, 10), TestCase(20, 20, 5), TestCase(20, 20, 1),
		TestCase(20, 20, 0),
		TestCase(20, 20, -1), TestCase(20, 20, -5), TestCase(20, 20, -10),
		TestCase(20, 20, -15), TestCase(20, 20, -20), TestCase(20, 20, -25),
		TestCase(30, 30, 25), TestCase(30, 30, 20), TestCase(30, 30, 15),
		TestCase(30, 30, 10), TestCase(30, 30, 5), TestCase(30, 30, 1),
		TestCase(30, 30, 0),
		TestCase(30, 30, -1), TestCase(30, 30, -5), TestCase(30, 30, -10),
		TestCase(30, 30, -15), TestCase(30, 30, -20), TestCase(30, 30, -25),
		TestCase(40, 40, 20), TestCase(40, 40, 15),
		TestCase(40, 40, 10), TestCase(40, 40, 5), TestCase(40, 40, 1),
		TestCase(40, 40, 0),
		TestCase(40, 40, -1), TestCase(40, 40, -5), TestCase(40, 40, -10),
		TestCase(40, 40, -15), TestCase(40, 40, -20), TestCase(40, 40, -25),
		TestCase(50, 50, 25), TestCase(50, 50, 20), TestCase(50, 50, 15),
		TestCase(50, 50, 10), TestCase(50, 50, 5), TestCase(50, 50, 1),
		TestCase(50, 50, 0),
		TestCase(50, 50, -1), TestCase(50, 50, -5), TestCase(50, 50, -10),
		TestCase(50, 50, -15), TestCase(50, 50, -20), TestCase(50, 50, -25),
		TestCase(60, 60, 20), TestCase(60, 60, 15),
		TestCase(60, 60, 10), TestCase(60, 60, 5), TestCase(60, 60, 1),
		TestCase(60, 60, 0),
		TestCase(60, 60, -1), TestCase(60, 60, -5), TestCase(60, 60, -10),
		TestCase(60, 60, -15), TestCase(60, 60, -20), TestCase(60, 60, -25),
		TestCase(80, 80, 20), TestCase(80, 80, 15),
		TestCase(80, 80, 10), TestCase(80, 80, 5), TestCase(80, 80, 1),
		TestCase(80, 80, 0),
		TestCase(80, 80, -1), TestCase(80, 80, -5), TestCase(80, 80, -10),
		TestCase(80, 80, -15), TestCase(80, 80, -20), TestCase(80, 80, -25),
		TestCase(85, 85, 25), TestCase(85, 85, 20), TestCase(85, 85, 15),
		TestCase(85, 85, 10), TestCase(85, 85, 5), TestCase(85, 85, 1),
		TestCase(85, 85, 0),
		TestCase(85, 85, -1), TestCase(85, 85, -5), TestCase(85, 85, -10),
		TestCase(85, 85, -15), TestCase(85, 85, -20), TestCase(85, 85, -25),
		]
		public void Coach_Drive_AAux(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			var slopeStr = GetSlopeString(slope);
			var modFileName = string.Format(CultureInfo.InvariantCulture, @"CoachAAUX_{0}_{1}_{2}.vmod", v1, v2, slopeStr);

			Coach_BusAuxiliaries(cycle, modFileName,
				string.Format(CultureInfo.InvariantCulture, "24t Coach_AAux_Cycle_Drive_{0}_{1}_{2}.vmod", v1, v2, slopeStr));
		}

		[Category("ComparisonAAUX"),
		TestCase(10, 10, 25),
		TestCase(40, 40, 25),
		TestCase(60, 60, 25),
		TestCase(80, 80, 25),
		]
		public void Coach_Drive_AAux_Low(double v1, double v2, double slope)
		{
			var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
				v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

			var slopeStr = GetSlopeString(slope);
			var modFileName = string.Format(CultureInfo.InvariantCulture, @"CoachAAUX_{0}_{1}_{2}.vmod", v1, v2, slopeStr);

			Coach_BusAuxiliaries(cycle, modFileName,
				string.Format(CultureInfo.InvariantCulture, "24t Coach_AAux_Cycle_Drive_{0}_{1}_{2}.vmod", v1, v2, slopeStr),
				false);
		}

		[Category("LongRunning")]
		[Category("ComparisonAAUX"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Increasing_Slope,
			"Coach_AAux_Drive_80_slope_inc.vmod", "24t Coach_AAux_Cycle_Drive_80_Increasing_Slope.vmod",
			TestName = "CoachBusAux CycleDrive_80_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Increasing_Slope,
			"Coach_AAux_Drive_50_slope_inc.vmod", "24t Coach_AAux_Cycle_Drive_50_Increasing_Slope.vmod",
			TestName = "CoachBusAux CycleDrive_50_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Increasing_Slope,
			"Coach_AAux_Drive_30_slope_inc.vmod", "24t Coach_AAux_Cycle_Drive_30_Increasing_Slope.vmod",
			TestName = "CoachBusAux CycleDrive_30_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Decreasing_Slope,
			"Coach_AAux_Drive_80_slope_dec.vmod", "24t Coach_AAux_Cycle_Drive_80_Decreasing_Slope.vmod",
			TestName = "CoachBusAux CycleDrive_80_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Decreasing_Slope,
			"Coach_AAux_Drive_50_slope_dec.vmod", "24t Coach_AAux_Cycle_Drive_50_Decreasing_Slope.vmod",
			TestName = "CoachBusAux CycleDrive_50_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Decreasing_Slope,
			"Coach_AAux_Drive_30_slope_dec.vmod", "24t Coach_AAux_Cycle_Drive_30_Decreasing_Slope.vmod",
			TestName = "CoachBusAux CycleDrive_30_Decreasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_80_Dec_Increasing_Slope,
			"Coach_AAux_Drive_80_slope_dec-inc.vmod", "24t Coach_AAux_Cycle_Drive_80_Dec_Increasing_Slope.vmod",
			TestName = "CoachBusAux CycleDrive_80_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_50_Dec_Increasing_Slope,
			"Coach_AAux_Drive_50_slope_dec-inc.vmod", "24t Coach_AAux_Cycle_Drive_50_Dec_Increasing_Slope.vmod",
			TestName = "CoachBusAux CycleDrive_50_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDrive_30_Dec_Increasing_Slope,
			"Coach_AAux_Drive_30_slope_dec-inc.vmod", "24t Coach_AAux_Cycle_Drive_30_Dec_Increasing_Slope.vmod",
			TestName = "CoachBusAux CycleDrive_30_Dec_Increasing_Slope"),
		TestCase(SimpleDrivingCycles.CycleDecelerateWhileBrake_80_0_level,
			"Coach_AAux_DecelerateWhileBrake_80_0_level.vmod", "24t Coach_AAux_Cycle_DecelerateWhileBrake_80_0_level.vmod",
			TestName = "CoachBusAux CycleDecelerateWhileBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateWhileBrake_80_0_level,
			"Coach_AAux_AccelerateWhileBrake_80_0_level.vmod", "24t Coach_AAux_Cycle_AccelerateWhileBrake_80_0_level.vmod",
			TestName = "CoachBusAux CycleAccelerateWhileBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateAtBrake_80_0_level,
			"Coach_AAux_AccelerateAtBrake_80_0_level.vmod", "24t Coach_AAux_Cycle_AccelerateAtBrake_80_0_level.vmod",
			TestName = "CoachBusAux CycleAccelerateAtBrake_80_0_level"),
		TestCase(SimpleDrivingCycles.CycleAccelerateBeforeBrake_80_0_level,
			"Coach_AAux_AccelerateBeforeBrake_80_0_level.vmod", "24t Coach_AAux_Cycle_AccelerateBeforeBrake_80_0_level.vmod",
			TestName = "CoachBusAux CycleAccelerateBeforeBrake_80_0_level"
			),
		TestCase(SimpleDrivingCycles.CycleDrive_stop_85_stop_85_level,
			"Coach_AAux_Drive_stop_85_stop_85_level.vmod", "24t Coach_AAux_Cycle_Drive_stop_85_stop_85_level.vmod",
			TestName = "CoachBusAux CycleDrive_stop_85_stop_85_level"),
		]
		public void Coach_AAux_Special(string cycleData, string modFileName, string compareFileName)
		{
			Coach_BusAuxiliaries(cycleData, modFileName, compareFileName);
		}

		private void Coach_BusAuxiliaries(string cycleData, string modFileName, string compareFileName,
			bool highEnginePower = true)
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, modFileName, highEnginePower: highEnginePower);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GetGraphWriter().Write(modFileName, @"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\" + compareFileName);
		}
	}
}