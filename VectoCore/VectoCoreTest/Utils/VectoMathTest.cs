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
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;


namespace TUGraz.VectoCore.Tests.Utils
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class VectoMathTest
	{
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void VectoMath_Min()
		{
			var smaller = 0.SI();
			var bigger = 5.SI();
			var negative = -1 * 10.SI();
			var positive = 10.SI();
			Assert.AreEqual(smaller, VectoMath.Min(smaller, bigger));

			Assert.AreEqual(bigger, VectoMath.Max(smaller, bigger));

			Assert.AreEqual(positive, VectoMath.Abs(negative));
			Assert.AreEqual(positive, VectoMath.Abs(positive));

			var smallerWatt = 0.SI<Watt>();
			var biggerWatt = 5.SI<Watt>();
			var negativeWatt = -10.SI<Watt>();
			var positiveWatt = 10.SI<Watt>();
			Assert.AreEqual(smallerWatt, VectoMath.Min(smallerWatt, biggerWatt));
			Assert.AreEqual(smallerWatt, VectoMath.Min(biggerWatt, smallerWatt));

			Assert.AreEqual(biggerWatt, VectoMath.Max(smallerWatt, biggerWatt));

			Assert.AreEqual(positiveWatt, VectoMath.Abs(negativeWatt));
			Assert.AreEqual(positiveWatt, VectoMath.Abs(positiveWatt));
		}

		[TestCase(0, -1, 0, 1, -1, 0, 1, 0, 0, 0),
		TestCase(0, 0, 10, 0, 0, 5, 10, 5, double.NaN, double.NaN),
		TestCase(0, 0, 0, 10, 5, 0, 10, 5, double.NaN, double.NaN),
		TestCase(0, 0, 1, 1, 1, 0, 0, 1, 0.5, 0.5),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void IntersectionTest(double p0x, double p0y, double p1x, double p1y, double p2x, double p2y, double p3x,
			double p3y, double isx, double isy)
		{
			var line1 = new Edge(new Point(p0x, p0y), new Point(p1x, p1y));
			var line2 = new Edge(new Point(p2x, p2y), new Point(p3x, p3y));

			var intersect = VectoMath.Intersect(line1, line2);
			if (intersect != null) {
				Assert.AreEqual(isx, intersect.X);
				Assert.AreEqual(isy, intersect.Y);
			} else {
				Assert.IsTrue(double.IsNaN(isx));
				Assert.IsTrue(double.IsNaN(isy));
			}
		}

		[TestCase(1, 2, 0, 5, new[] { -2.6906474480286136 }),
		TestCase(5, -3, 2, 10, new[] { -1.0 }),
		TestCase(5, -30, 2, 10, new[] { -0.5238756689475912, 0.6499388479175676, 5.873936821030023 }),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void CubicSolverTest(double a, double b, double c, double d, double[] expected)
		{
			var results = VectoMath.CubicEquationSolver(a, b, c, d);

			Assert.AreEqual(expected.Length, results.Length);
			var sorted = expected.ToList();
			sorted.Sort();
			Array.Sort(results);

			var comparison = sorted.Zip(results, (exp, result) => exp - result);
			foreach (var cmp in comparison) {
				Assert.AreEqual(0, cmp, 1e-15);
			}
		}

		[TestCase(1, 6, 18, 30, 25, new double[] { }),  // only complex-valued solutions
		TestCase(1, 6, -18, 30, -25, new[] { 1.31684387048749, -8.55416218029519 }), // two complex, two real-valued
		TestCase(1, 11, 41, 61, 30, new double[] { -5, -3, -2, -1 }),
		TestCase(1, 0, -4, 0, 3, new[] { 1.73205080756888, 1, -1.73205080756888, -1 }), // biquadratic
		TestCase(5, 0, -20, 0, 15, new[] { 1.73205080756888, 1, -1.73205080756888, -1 }),
		TestCase(1, 1, 1, 1, 1, new double[] { }), // only complex solutions
		TestCase(1, 2, -14, 2, 1, new[] { 2.76090563295441601, 0.362199992663244539, -0.203258341626567109, -4.91984728399109344 }),
		TestCase(16, 8, -16, -8, 1, new[] { 0.1045284632676534713998341548025, 0.9781476007338056379285667478696, -0.91354545764260089550212757198532, -0.66913060635885821382627333068678 }),
		Category(Definitions.TESTCASE_MIGRATED)
        ]
		public void Polynom4SolverTest(double a, double b, double c, double d, double e, double[] expected)
		{
			var results = VectoMath.Polynom4Solver(a, b, c, d, e);

			Console.WriteLine(results.Join());

			Assert.AreEqual(expected.Length, results.Length);

			Array.Sort(expected);
			Array.Sort(results);
			var comparison = expected.Zip(results, (exp, result) => exp - result);
			foreach (var comp in comparison) {
				Assert.AreEqual(0, comp, 1e-12);
			}
		}


		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestLeastSquaresFittingExact()
		{
			var entries = new[] {
				new { X = 0, Y = 4 },
				new { X = 10, Y = 8 },
				new { X = 20, Y = 12 }
			};

			var (k, d) = VectoMath.LeastSquaresFitting(entries, x => x.X, x => x.Y);

			Assert.AreEqual(4, d, 1e-6);
			Assert.AreEqual(0.4, k, 1e-6);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestLeastSquaresFittingEx1()
		{
			var entries = new[] {
				new Point(12, 34.12),
				new Point(19, 40.94),
				new Point(23, 33.58),
				new Point(30, 38.95),
				new Point(22, 35.42),
				new Point(11, 32.12),
				new Point(13, 28.57),
				new Point(28, 40.97),
				new Point(10, 32.06),
				new Point(11, 30.55),
			};

			var (k, d) = VectoMath.LeastSquaresFitting(entries);

			Assert.AreEqual(27.003529, d, 1e-6);
			Assert.AreEqual(0.431535, k, 1e-6);

		}
	}
}
