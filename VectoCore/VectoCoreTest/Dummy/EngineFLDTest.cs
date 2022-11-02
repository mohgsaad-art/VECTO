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
using System.Diagnostics;
using System.IO;
using TUGraz.VectoCommon.Utils;
using NUnit.Framework;
using TUGraz.VectoCore.InputData.Reader.ComponentData;

namespace TUGraz.VectoCore.Tests.Dummy
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class EngineFLDTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void CompareFLDLookupPerformance()
		{
			var engineFile1 = @"TestData\Components\40t_Long_Haul_Truck.vfld";
			var engineFile2 = @"E:\QUAM\Downloads\EngineFLD\Map_375c_BB1390_modTUG_R49_375c_BB1386.vfld";

			if (!File.Exists(engineFile2)) {
				Assert.Inconclusive("Confidential File not found. Test cannot run without file.");
			}

			var map1 = FullLoadCurveReader.ReadFromFile(engineFile1, true);
			var map2 = FullLoadCurveReader.ReadFromFile(engineFile2, true);

			map1.FullLoadStationaryTorque(1000.RPMtoRad());
			map2.FullLoadStationaryTorque(1000.RPMtoRad());

			foreach (var map in new[] { map1, map2 }) {
				var rand = new Random();
				var stopWatch = Stopwatch.StartNew();


				for (var i = 0; i < 500000; i++) {
					var angularVelocity = rand.Next(1000, 1400).RPMtoRad();

					var tqMax = map.FullLoadStationaryTorque(angularVelocity);
				}

				stopWatch.Stop();
				Debug.Print("{0}", stopWatch.ElapsedMilliseconds);
			}
		}

		[TestCase]
		public void LookupTest()
		{
			var engineFile2 = @"E:\QUAM\Downloads\EngineFLD\Map_375c_BB1390_modTUG_R49_375c_BB1386.vfld";

			if (!File.Exists(engineFile2)) {
				Assert.Inconclusive("Confidential File not found. Test cannot run without file.");
			}

			var map = FullLoadCurveReader.ReadFromFile(engineFile2, true);

			Assert.AreEqual(1208, map.FullLoadStationaryTorque(500.RPMtoRad()).Value(), 1e-3);

			Assert.AreEqual(27, map.FullLoadStationaryTorque(2202.RPMtoRad()).Value(), 1e-3);

			Assert.AreEqual(2341.5714, map.FullLoadStationaryTorque(1500.RPMtoRad()).Value(), 1e-3);
			Assert.AreEqual(376.7142, map.FullLoadStationaryTorque(2175.RPMtoRad()).Value(), 1e-3);

			Assert.AreEqual(1544, map.FullLoadStationaryTorque(628.RPMtoRad()).Value(), 1e-3);
		}
	}
}
