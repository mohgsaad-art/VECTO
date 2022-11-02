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
	public class CoastingTests
	{
		[OneTimeSetUp]
		public void DisableLogging() { }

		public GraphWriter GetGraphWriter() 
		{
			var graphWriter = new GraphWriter();
			//LogManager.DisableLogging();
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
		TestCase(60, 20, 0),
		TestCase(60, 20, 0.6),
		TestCase(60, 20, 1.4),
		TestCase(60, 20, 2.7),
		TestCase(60, 20, 3.3),
		TestCase(60, 20, 3.7),
		TestCase(60, 35, 5.3),
		TestCase(50, 47.5, -2.1),
		TestCase(65, 62.5, -0.8),
		TestCase(60, 50, 5.6),
		TestCase(80, 40, 4.0),
		TestCase(65, 60, 4.7),
		TestCase(70, 62.5, 4.6),
		TestCase(75, 65, 4.5),
		]
		public void Truck_Coasting_Test(double v1, double v2, double slope)
		{
			Assert.IsTrue(v1 > v2);

			var cycle = new[] {
				// <s>,<v>,<grad>,<stop>
				string.Format(CultureInfo.InvariantCulture, "  0,  {0}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1000, {1}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1100, {1},   0,  0", v1, v2, slope)
			};
			System.IO.Directory.CreateDirectory(string.Format(@"Coast_{0}_{1}", v1, v2, slope));
			var slopePrefix = "";
			if (!slope.IsEqual(0)) {
				slopePrefix = slope > 0 ? "uh_" : "dh_";
			}
			var modFile = string.Format(@"Coast_{0}_{1}\Truck_Coast_{0}_{1}_{3}{2:0.0}.vmod", v1, v2, Math.Abs(slope),
				slopePrefix);
			var cycleData = SimpleDrivingCycles.CreateCycleData(cycle);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycleData, modFile);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GetGraphWriter().Write(modFile);
		}

		[Ignore("no longer relevant"), Test,
		TestCase(40),
		TestCase(45),
		TestCase(50),
		TestCase(55),
		TestCase(60),
		TestCase(65),
		TestCase(70),
		TestCase(75),
		TestCase(80),
		]
		public void Truck_Coasting_Variability_Test(double v1)
		{
			const double vStep = 2.5;
			const double vMin = 40;
			Assert.IsTrue(vMin - vStep > 0);
			for (var v2 = vMin; v2 <= v1 - vStep; v2 += vStep) {
				for (var slope = -6.0; slope <= 6; slope += 0.1) {
					Truck_Coasting_Test(v1, v2, slope);
				}
			}
		}
	}
}