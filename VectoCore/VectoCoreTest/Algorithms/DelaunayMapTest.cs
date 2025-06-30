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
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Algorithms
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DelaunayMapTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_Simple_DelaunayMap()
		{
			var map = new DelaunayMap("TEST");
			map.AddPoint(0, 0, 0);
			map.AddPoint(1, 0, 0);
			map.AddPoint(0, 1, 0);

			map.Triangulate();

			var result = map.Interpolate(0.25, 0.25);

			AssertHelper.AreRelativeEqual(0, result);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_DelaunayMapTriangle()
		{
			var map = new DelaunayMap("TEST");
			map.AddPoint(0, 0, 0);
			map.AddPoint(1, 0, 1);
			map.AddPoint(0, 1, 2);

			map.Triangulate();

			// fixed points
			AssertHelper.AreRelativeEqual(0, map.Interpolate(0, 0));
			AssertHelper.AreRelativeEqual(1, map.Interpolate(1, 0));
			AssertHelper.AreRelativeEqual(2, map.Interpolate(0, 1));

			// interpolations
			AssertHelper.AreRelativeEqual(0.5, map.Interpolate(0.5, 0));
			AssertHelper.AreRelativeEqual(1, map.Interpolate(0, 0.5));
			AssertHelper.AreRelativeEqual(1.5, map.Interpolate(0.5, 0.5));

			AssertHelper.AreRelativeEqual(0.25, map.Interpolate(0.25, 0));
			AssertHelper.AreRelativeEqual(0.5, map.Interpolate(0, 0.25));
			AssertHelper.AreRelativeEqual(0.75, map.Interpolate(0.25, 0.25));

			AssertHelper.AreRelativeEqual(0.75, map.Interpolate(0.75, 0));
			AssertHelper.AreRelativeEqual(1.5, map.Interpolate(0, 0.75));

			// extrapolation (should fail)
			Assert.IsNaN(map.Interpolate(1, 1));
			Assert.IsNaN(map.Interpolate(-1, -1));
			Assert.IsNaN(map.Interpolate(1, -1));
			Assert.IsNaN(map.Interpolate(-1, 1));
		}

		public void Test_DelaunayMapPlane()
		{
			var map = new DelaunayMap("TEST");
			map.AddPoint(0, 0, 0);
			map.AddPoint(1, 0, 1);
			map.AddPoint(0, 1, 2);
			map.AddPoint(1, 1, 3);

			map.Triangulate();

			// fixed points
			AssertHelper.AreRelativeEqual(0, map.Interpolate(0, 0));
			AssertHelper.AreRelativeEqual(1, map.Interpolate(1, 0));
			AssertHelper.AreRelativeEqual(2, map.Interpolate(0, 1));
			AssertHelper.AreRelativeEqual(3, map.Interpolate(1, 1));

			// interpolations
			AssertHelper.AreRelativeEqual(0.5, map.Interpolate(0.5, 0));
			AssertHelper.AreRelativeEqual(1, map.Interpolate(0, 0.5));
			AssertHelper.AreRelativeEqual(2, map.Interpolate(1, 0.5));
			AssertHelper.AreRelativeEqual(2.5, map.Interpolate(0.5, 1));

			AssertHelper.AreRelativeEqual(1.5, map.Interpolate(0.5, 0.5));

			AssertHelper.AreRelativeEqual(0.75, map.Interpolate(0.25, 0.25));
			AssertHelper.AreRelativeEqual(2.25, map.Interpolate(0.75, 0.75));

			AssertHelper.AreRelativeEqual(1.75, map.Interpolate(0.25, 0.75));
			AssertHelper.AreRelativeEqual(1.25, map.Interpolate(0.75, 0.25));

			// extrapolation (should fail)
			AssertHelper.Exception<VectoException>(() => map.Interpolate(1.5, 0.5), "Interpolation failed.");
			AssertHelper.Exception<VectoException>(() => map.Interpolate(1.5, 1.5), "Interpolation failed.");
			AssertHelper.Exception<VectoException>(() => map.Interpolate(0.5, 1.5), "Interpolation failed.");
			AssertHelper.Exception<VectoException>(() => map.Interpolate(-0.5, 1.5), "Interpolation failed.");
			AssertHelper.Exception<VectoException>(() => map.Interpolate(-0.5, 0.5), "Interpolation failed.");
			AssertHelper.Exception<VectoException>(() => map.Interpolate(-1.5, -1.5), "Interpolation failed.");
			AssertHelper.Exception<VectoException>(() => map.Interpolate(0.5, -0.5), "Interpolation failed.");
			AssertHelper.Exception<VectoException>(() => map.Interpolate(-1.5, -0.5), "Interpolation failed.");
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_Delaunay_LessThan3Points()
		{
			AssertHelper.Exception<ArgumentException>(() => new DelaunayMap("TEST").Triangulate(),
				"TEST: Triangulation needs at least 3 Points. Got 0 Points.");

			AssertHelper.Exception<ArgumentException>(() => {
				var map1 = new DelaunayMap("TEST");
				map1.AddPoint(1, 0, 0);
				map1.Triangulate();
			}, "TEST: Triangulation needs at least 3 Points. Got 1 Points.");

			AssertHelper.Exception<ArgumentException>(() => {
				var map2 = new DelaunayMap("TEST");
				map2.AddPoint(1, 0, 0);
				map2.AddPoint(0, 1, 0);
				map2.Triangulate();
			}, "TEST: Triangulation needs at least 3 Points. Got 2 Points.");

			var map = new DelaunayMap("TEST");
			map.AddPoint(1, 0, 0);
			map.AddPoint(0, 1, 0);
			map.AddPoint(0, 0, 1);
			map.Triangulate();
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_Delaunay_DuplicatePoints()
		{
			var map = new DelaunayMap("TEST");
			map.AddPoint(0, 0, 0);
			map.AddPoint(1, 0, 1);
			map.AddPoint(1, 1, 3);
			map.AddPoint(0, 1, 2);
			map.AddPoint(1, 1, 5);

			AssertHelper.Exception<VectoException>(() => { map.Triangulate(); },
				"TEST: Input Data for Delaunay map contains duplicates! \n1 / 1");
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void Test_Delaunay_NormalOperation()
		{
			foreach (var factors in	new[] {
				Tuple.Create(1.0, 1.0),
				Tuple.Create(1.0, 0.04),
				Tuple.Create(1.0, 0.1),
				Tuple.Create(1.0, 0.01),
				Tuple.Create(1.0, 0.0001)
			}) {
				var xfactor = factors.Item1;
				var yfactor = factors.Item2;

				var map = new DelaunayMap("TEST");
				var points =
					File.ReadAllLines(@"TestData/Components/40t_Long_Haul_Truck.vmap")
						.Skip(1)
						.Select(s => {
							var p = s.Split(',').ToDouble().ToList();
							return new Point(p[0] * xfactor, p[1] * yfactor, p[2]);
						})
						.ToList();

				points.ForEach(p => map.AddPoint(p.X, p.Y, p.Z));
				map.Triangulate();

				// test fixed points
				foreach (var p in points) {
					AssertHelper.AreRelativeEqual(p.Z, map.Interpolate(p.X, p.Y));
				}
				map.DrawGraph();

				// test one arbitrary point in the middle
				AssertHelper.AreRelativeEqual(37681, map.Interpolate(1500 * xfactor, 1300 * yfactor),
					$"{xfactor}, {yfactor}");
			}
		}


		[TestCase()]
		public void Test_Delaunay_ReRaster()
		{
			var mapPoints = new[] {
				Tuple.Create<double, double, double>(0.001538462, -0.032394231, 0.000000000),
				Tuple.Create<double, double, double>(0.155384615, -0.045519615, 0.000000000),
				Tuple.Create<double, double, double>(0.310000000, -0.062632308, 0.000000000),
				Tuple.Create<double, double, double>(0.464461538, -0.082949615, 0.000000000),
				Tuple.Create<double, double, double>(0.618461538, -0.109889231, 0.000000000),
				Tuple.Create<double, double, double>(0.773000000, -0.133641538, 0.000000000),
				Tuple.Create<double, double, double>(0.927615385, -0.162471923, 0.000000000),
				Tuple.Create<double, double, double>(1.081846154, -0.193093077, 0.000000000),
				Tuple.Create<double, double, double>(1.236384615, -0.232284615, 0.000000000),
				Tuple.Create<double, double, double>(1.236461538, -0.012653077, 13.482692308),
				Tuple.Create<double, double, double>(1.081769231, 0.001316923, 12.018076923),
				Tuple.Create<double, double, double>(0.927461538, -0.001039231, 10.260384615),
				Tuple.Create<double, double, double>(0.773615385, 0.001193462, 6.957307692),
				Tuple.Create<double, double, double>(0.618615385, 0.014564615, 8.122692308),
				Tuple.Create<double, double, double>(0.464615385, -0.002257308, 7.198846154),
				Tuple.Create<double, double, double>(0.309846154, 0.001653846, 4.949615385),
				Tuple.Create<double, double, double>(0.155538462, -0.002086154, 2.305000000),
				Tuple.Create<double, double, double>(0.001230769, -0.000385000, 2.300384615),
				Tuple.Create<double, double, double>(0.001538462, 0.051939231, 5.341923077),
				Tuple.Create<double, double, double>(0.155384615, 0.065864231, 7.465769231),
				Tuple.Create<double, double, double>(0.310000000, 0.079249231, 9.391153846),
				Tuple.Create<double, double, double>(0.464307692, 0.098057308, 10.908461538),
				Tuple.Create<double, double, double>(0.618692308, 0.116616923, 13.846153846),
				Tuple.Create<double, double, double>(0.773076923, 0.132415385, 15.488846154),
				Tuple.Create<double, double, double>(0.927461538, 0.147105000, 18.621538462),
				Tuple.Create<double, double, double>(1.081769231, 0.165186538, 21.701923077),
				Tuple.Create<double, double, double>(1.159153846, 0.172157692, 23.980000000),
				Tuple.Create<double, double, double>(1.159153846, 0.343706538, 34.611538462),
				Tuple.Create<double, double, double>(1.081846154, 0.330271154, 32.211538462),
				Tuple.Create<double, double, double>(0.927538462, 0.298645000, 28.569615385),
				Tuple.Create<double, double, double>(0.773000000, 0.264761923, 25.560000000),
				Tuple.Create<double, double, double>(0.618846154, 0.230699231, 21.142692308),
				Tuple.Create<double, double, double>(0.464538462, 0.200092308, 16.688461538),
				Tuple.Create<double, double, double>(0.310000000, 0.165346154, 14.146538462),
				Tuple.Create<double, double, double>(0.155384615, 0.129761923, 10.897307692),
				Tuple.Create<double, double, double>(0.001461538, 0.097684231, 7.566153846),
				Tuple.Create<double, double, double>(0.001538462, 0.150440769, 11.187692308),
				Tuple.Create<double, double, double>(0.155384615, 0.198203077, 15.025769231),
				Tuple.Create<double, double, double>(0.310000000, 0.247090385, 19.126153846),
				Tuple.Create<double, double, double>(0.463923077, 0.295549615, 23.085000000),
				Tuple.Create<double, double, double>(0.618769231, 0.344763846, 27.550769231),
				Tuple.Create<double, double, double>(0.773076923, 0.393946538, 32.051923077),
				Tuple.Create<double, double, double>(0.927384615, 0.442922692, 37.409615385),
				Tuple.Create<double, double, double>(1.081923077, 0.496163846, 43.010000000),
				Tuple.Create<double, double, double>(1.081846154, 0.655856154, 54.189615385),
				Tuple.Create<double, double, double>(0.927461538, 0.593846538, 47.590384615),
				Tuple.Create<double, double, double>(0.772923077, 0.525074231, 39.774615385),
				Tuple.Create<double, double, double>(0.618615385, 0.462357692, 35.888076923),
				Tuple.Create<double, double, double>(0.463923077, 0.391441154, 30.429230769),
				Tuple.Create<double, double, double>(0.310000000, 0.329964231, 24.787692308),
				Tuple.Create<double, double, double>(0.155538462, 0.263354615, 20.846923077),
				Tuple.Create<double, double, double>(0.001384615, 0.194714231, 16.068846154),
				Tuple.Create<double, double, double>(0.155769231, 0.329682308, 25.057692308),
				Tuple.Create<double, double, double>(0.310076923, 0.408988846, 31.324615385),
				Tuple.Create<double, double, double>(0.463923077, 0.494515769, 36.668461538),
				Tuple.Create<double, double, double>(0.618846154, 0.574802692, 42.729615385),
				Tuple.Create<double, double, double>(0.773230769, 0.657451538, 51.768846154),
				Tuple.Create<double, double, double>(0.927384615, 0.737729231, 58.619230769),
				Tuple.Create<double, double, double>(1.082000000, 0.820795385, 66.246538462),
				Tuple.Create<double, double, double>(0.927692308, 0.885024615, 69.555384615),
				Tuple.Create<double, double, double>(0.773153846, 0.792173462, 60.138076923),
				Tuple.Create<double, double, double>(0.618846154, 0.687764231, 50.728076923),
				Tuple.Create<double, double, double>(0.464615385, 0.591986538, 42.919615385),
				Tuple.Create<double, double, double>(0.309923077, 0.495059615, 35.414615385),
				Tuple.Create<double, double, double>(0.155384615, 0.395178846, 29.951538462),
				Tuple.Create<double, double, double>(0.155538462, 0.458480769, 35.030000000),
				Tuple.Create<double, double, double>(0.310000000, 0.574832692, 42.308846154),
				Tuple.Create<double, double, double>(0.463769231, 0.690457308, 50.049615385),
				Tuple.Create<double, double, double>(0.618692308, 0.802535385, 57.395769231),
				Tuple.Create<double, double, double>(0.773000000, 0.922390385, 69.417307692),
				Tuple.Create<double, double, double>(0.927384615, 1.014361923, 79.097692308),
				Tuple.Create<double, double, double>(0.618692308, 0.920111538, 65.989230769),
				Tuple.Create<double, double, double>(0.464615385, 0.793565385, 57.120769231),
				Tuple.Create<double, double, double>(0.309923077, 0.661785769, 46.892692308),
				Tuple.Create<double, double, double>(0.309846154, 0.737497308, 53.649230769),
				Tuple.Create<double, double, double>(0.463846154, 0.886876538, 64.360000000),
				Tuple.Create<double, double, double>(0.464692308, 0.971539231, 70.035384615),
				Tuple.Create<double, double, double>(0.309769231, 0.825154231, 58.071538462),
				Tuple.Create<double, double, double>(1.159076923, 0.478227308, 44.091923077),
				Tuple.Create<double, double, double>(1.082000000, 0.958018846, 75.203076923),
				Tuple.Create<double, double, double>(1.004461538, 1.005195385, 75.920769231),
				Tuple.Create<double, double, double>(0.927538462, 1.014611538, 77.188846154),
				Tuple.Create<double, double, double>(0.850252154, 1.001503846, 76.295000000),
				Tuple.Create<double, double, double>(0.773048615, 0.995304231, 74.519615385),
				Tuple.Create<double, double, double>(0.695852846, 0.984280385, 71.971538462),
				Tuple.Create<double, double, double>(0.618603000, 0.975799231, 71.007692308),
				Tuple.Create<double, double, double>(0.541692308, 0.952781154, 68.597307692),
				Tuple.Create<double, double, double>(0.463923077, 0.970624615, 69.900769231),
				Tuple.Create<double, double, double>(0.387384615, 0.962980385, 68.582692308),
				Tuple.Create<double, double, double>(0.310076923, 0.861072692, 62.247307692),
				Tuple.Create<double, double, double>(0.232615385, 0.684363462, 49.479615385),
				Tuple.Create<double, double, double>(0.155538462, 0.504606154, 38.427692308),
				Tuple.Create<double, double, double>(0.078461538, 0.374907308, 29.214230769),
				Tuple.Create<double, double, double>(0.001384615, 0.244873846, 19.907307692),
				Tuple.Create<double, double, double>(1.236169308, -0.229840769, 0.000000000),
				Tuple.Create<double, double, double>(0.927514769, -0.159189615, 0.000000000),
				Tuple.Create<double, double, double>(0.618461538, -0.105688846, 0.000000000),
				Tuple.Create<double, double, double>(0.309994846, -0.060521923, 0.000000000),
				Tuple.Create<double, double, double>(0.001502462, -0.031145385, 0.000000000),
			};
			

			var gridPoints = new[] {
				Tuple.Create<double, double>(0.0, -0.12),
				Tuple.Create<double, double>(0.0, 0.02),
				Tuple.Create<double, double>(0.0, 0.16),
				Tuple.Create<double, double>(0.12, -0.12),
				Tuple.Create<double, double>(0.12, 0.02),
				Tuple.Create<double, double>(0.12, 0.16),
				Tuple.Create<double, double>(0.12, 0.3),
				Tuple.Create<double, double>(0.24, -0.12),
				Tuple.Create<double, double>(0.24, 0.02),
				Tuple.Create<double, double>(0.24, 0.16),
				Tuple.Create<double, double>(0.24, 0.3),
				Tuple.Create<double, double>(0.24, 0.44),
				Tuple.Create<double, double>(0.36, -0.12),
				Tuple.Create<double, double>(0.36, 0.02),
				Tuple.Create<double, double>(0.36, 0.16),
				Tuple.Create<double, double>(0.36, 0.3),
				Tuple.Create<double, double>(0.36, 0.44),
				Tuple.Create<double, double>(0.36, 0.58),
				Tuple.Create<double, double>(0.36, 0.72),
				Tuple.Create<double, double>(0.36, 0.86),
				Tuple.Create<double, double>(0.48, -0.12),
				Tuple.Create<double, double>(0.48, 0.02),
				Tuple.Create<double, double>(0.48, 0.16),
				Tuple.Create<double, double>(0.48, 0.3),
				Tuple.Create<double, double>(0.48, 0.44),
				Tuple.Create<double, double>(0.48, 0.58),
				Tuple.Create<double, double>(0.48, 0.72),
				Tuple.Create<double, double>(0.48, 0.86),
				Tuple.Create<double, double>(0.48, 1),
				Tuple.Create<double, double>(0.6, -0.12),
				Tuple.Create<double, double>(0.6, 0.02),
				Tuple.Create<double, double>(0.6, 0.16),
				Tuple.Create<double, double>(0.6, 0.3),
				Tuple.Create<double, double>(0.6, 0.44),
				Tuple.Create<double, double>(0.6, 0.58),
				Tuple.Create<double, double>(0.6, 0.72),
				Tuple.Create<double, double>(0.6, 0.86),
				Tuple.Create<double, double>(0.6, 1),
				Tuple.Create<double, double>(0.72, -0.26),
				Tuple.Create<double, double>(0.72, -0.12),
				Tuple.Create<double, double>(0.72, 0.02),
				Tuple.Create<double, double>(0.72, 0.16),
				Tuple.Create<double, double>(0.72, 0.3),
				Tuple.Create<double, double>(0.72, 0.44),
				Tuple.Create<double, double>(0.72, 0.58),
				Tuple.Create<double, double>(0.72, 0.72),
				Tuple.Create<double, double>(0.72, 0.86),
				Tuple.Create<double, double>(0.72, 1),
				Tuple.Create<double, double>(0.84, -0.26),
				Tuple.Create<double, double>(0.84, -0.12),
				Tuple.Create<double, double>(0.84, 0.02),
				Tuple.Create<double, double>(0.84, 0.16),
				Tuple.Create<double, double>(0.84, 0.3),
				Tuple.Create<double, double>(0.84, 0.44),
				Tuple.Create<double, double>(0.84, 0.58),
				Tuple.Create<double, double>(0.84, 0.72),
				Tuple.Create<double, double>(0.84, 0.86),
				Tuple.Create<double, double>(0.84, 1),
				Tuple.Create<double, double>(0.96, -0.26),
				Tuple.Create<double, double>(0.96, -0.12),
				Tuple.Create<double, double>(0.96, 0.02),
				Tuple.Create<double, double>(0.96, 0.16),
				Tuple.Create<double, double>(0.96, 0.3),
				Tuple.Create<double, double>(0.96, 0.44),
				Tuple.Create<double, double>(0.96, 0.58),
				Tuple.Create<double, double>(0.96, 0.72),
				Tuple.Create<double, double>(0.96, 0.86),
				Tuple.Create<double, double>(0.96, 1),
				Tuple.Create<double, double>(1.08, -0.26),
				Tuple.Create<double, double>(1.08, -0.12),
				Tuple.Create<double, double>(1.08, 0.02),
				Tuple.Create<double, double>(1.08, 0.16),
				Tuple.Create<double, double>(1.08, 0.3),
				Tuple.Create<double, double>(1.08, 0.44),
				Tuple.Create<double, double>(1.08, 0.58),
				Tuple.Create<double, double>(1.08, 0.72),
				Tuple.Create<double, double>(1.08, 0.86),
				Tuple.Create<double, double>(1.2, -0.26),
				Tuple.Create<double, double>(1.2, -0.12),
				Tuple.Create<double, double>(1.2, 0.02),
				Tuple.Create<double, double>(1.2, 0.16),
				Tuple.Create<double, double>(1.2, 0.3),
				Tuple.Create<double, double>(1.2, 0.44),
				Tuple.Create<double, double>(1.2, 0.58),
				Tuple.Create<double, double>(0.0, -0.0334),
				Tuple.Create<double, double>(0.06, -0.0375),
				Tuple.Create<double, double>(0.12, -0.04182),
				Tuple.Create<double, double>(0.18, -0.04668),
				Tuple.Create<double, double>(0.24, -0.052),
				Tuple.Create<double, double>(0.3, -0.0577),
				Tuple.Create<double, double>(0.36, -0.06474),
				Tuple.Create<double, double>(0.42, -0.0723),
				Tuple.Create<double, double>(0.48, -0.08052),
				Tuple.Create<double, double>(0.54, -0.08948),
				Tuple.Create<double, double>(0.6, -0.0993),
				Tuple.Create<double, double>(0.66, -0.11024),
				Tuple.Create<double, double>(0.72, -0.12272),
				Tuple.Create<double, double>(0.78, -0.13682),
				Tuple.Create<double, double>(0.84, -0.15078),
				Tuple.Create<double, double>(0.9, -0.1649),
				Tuple.Create<double, double>(0.96, -0.17974),
				Tuple.Create<double, double>(1.02, -0.19602),
				Tuple.Create<double, double>(1.08, -0.21468),
				Tuple.Create<double, double>(1.14, -0.2342),
				Tuple.Create<double, double>(1.2, -0.254),
			};

			var map = new DelaunayMap("re-sample");
			foreach (var mapPoint in mapPoints) {
				map.AddPoint(mapPoint.Item1, mapPoint.Item2, mapPoint.Item3);
			}
			map.Triangulate();

			foreach (var gridPoint in gridPoints) {
				var extrapol = false;
				var val = map.Interpolate(gridPoint.Item1, gridPoint.Item2);
				if (double.IsNaN(val)) {
					extrapol = true;
					val = map.Extrapolate(gridPoint.Item1, gridPoint.Item2);
				}
				Console.WriteLine($"{gridPoint.Item1:F2}, {gridPoint.Item2:F5}, {val:F3} {(extrapol ? ", extrapolated" : "")}");
			}
		}
	}
}