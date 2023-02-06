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
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.ModelbasedTests.DriverStrategy
{
	[TestFixture]
	public class SimpleCycles
	{
		[OneTimeSetUp]
		public void Init()
		{
			//LogManager.DisableLogging();
		}

		public GraphWriter GetGraphWriter() 
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

		[Category("LongRunning"),
		TestCase(0, 80), TestCase(80, 0), TestCase(80, 80),
		TestCase(0, 60), TestCase(60, 0), TestCase(60, 60),
		TestCase(0, 40), TestCase(40, 0), TestCase(40, 40),
		TestCase(0, 20), TestCase(20, 0), TestCase(20, 20),
		]
		public void Truck_AllSlopes(double v1, double v2)
		{
			var slopes = new[] { 25, 20, 15, 10, 5, 3, 1, 0, -1, -3, -5, -10, -15, -20, -25 };

			foreach (var slope in slopes) {
				try {
					var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
						v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

					var modFileName = string.Format(CultureInfo.InvariantCulture, "Truck_{0}_{1}_{2}.vmod", v1, v2,
						GetSlopeString(slope));
					var cycleData = SimpleDrivingCycles.CreateCycleData(cycle);
					var run = Truck40tPowerTrain.CreateEngineeringRun(cycleData, modFileName);

					run.Run();

					Assert.IsTrue(run.FinishedWithoutErrors, "Truck Run (v1: {0}, v2: {1}, slope: {2}) failed.", v1, v2, slope);
					GetGraphWriter().Write(modFileName);
				} catch (Exception ex) {
					Assert.Fail("Truck Run (v1: {0}, v2: {1}, slope: {2}) failed: {3}", v1, v2, slope, ex);
				}
			}
		}

		[Category("LongRunning"),
		TestCase(0, 80), TestCase(80, 0), TestCase(80, 80),
		TestCase(0, 60), TestCase(60, 0), TestCase(60, 60),
		TestCase(0, 40), TestCase(40, 0), TestCase(40, 40),
		TestCase(0, 20), TestCase(20, 0), TestCase(20, 20),
		]
		public void Coach_AllSlopes(double v1, double v2)
		{
			var slopes = new[] { 15, 12, 10, 7, 5, 3, 1, 0, -1, -3, -5, -7, -10, -12, -15 };

			foreach (var slope in slopes) {
				try {
					var cycle = string.Format(CultureInfo.InvariantCulture, "0, {0}, {1}, {2}\n1000, {3}, {4}, {5}", v1, slope,
						v1.IsEqual(0) ? 2 : 0, v2, slope, v2.IsEqual(0) ? 2 : 0);

					var modFileName = string.Format(CultureInfo.InvariantCulture, "Coach_{0}_{1}_{2}.vmod", v1, v2,
						GetSlopeString(slope));
					var cycleData = SimpleDrivingCycles.CreateCycleData(cycle);
					var run = CoachPowerTrain.CreateEngineeringRun(cycleData, modFileName);

					run.Run();

					Assert.IsTrue(run.FinishedWithoutErrors, "Truck Run (v1: {0}, v2: {1}, slope: {2}) failed.", v1, v2, slope);
					GetGraphWriter().Write(modFileName);
				} catch (Exception ex) {
					Assert.Fail("Truck Run (v1: {0}, v2: {1}, slope: {2}) failed: {3}", v1, v2, slope, ex);
				}
			}
		}
	}
}