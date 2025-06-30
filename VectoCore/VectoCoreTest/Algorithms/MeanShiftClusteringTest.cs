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
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Algorithms
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class MeanShiftClusteringTest
	{
		
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestClusteringClusters()
		{
			var centers = new[] { 100.0, 200, 300, 400 };
			var entries = new List<double>();
			var rnd = new Random(centers.Length);
			foreach (var center in centers) {
				var sum = 0.0;
				for (var i = 0; i < 30; i++) {
					var val = center + rnd.NextDouble() * 5 - 2.5;
					sum += val;
					entries.Add(val);
				}
				//Console.WriteLine(sum / 30);
			}

			//Console.WriteLine(string.Join(", ", entries));

			var clusterer = new MeanShiftClustering {
				ClusterCount = 8
			};
			var clusters = clusterer.FindClusters(entries.ToArray(), 10);

			//Console.WriteLine(clusterer.IterationCount);
			Console.WriteLine(centers.Join());
			Console.WriteLine(clusters.Join());

			Assert.AreEqual(centers.Length, clusters.Length);
			foreach (var center in centers) {
				Assert.IsTrue(clusters.Any(x => Math.Abs(center - x) < 2.5));
			}
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestClusteringRandom()
		{
			var entries = new double[100];
			var rnd = new Random(0);
			for (var i = 0; i < entries.Length; i++) {
				entries[i] = rnd.NextDouble() * 100;
			}

			var clusterer = new MeanShiftClustering();
			var clusters = clusterer.FindClusters(entries.ToArray(), 0.1);

			Console.WriteLine(clusters.Join());
		}
	}
}
