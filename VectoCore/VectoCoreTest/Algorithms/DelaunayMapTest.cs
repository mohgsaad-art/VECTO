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
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
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


		[TestCase]
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

		[TestCase]
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

		[TestCase]
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

		[TestCase]
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

		[TestCase]
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
					File.ReadAllLines(@"TestData\Components\40t_Long_Haul_Truck.vmap")
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
	}
}