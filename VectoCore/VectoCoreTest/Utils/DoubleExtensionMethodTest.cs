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
using TUGraz.VectoCommon.Utils;
using NUnit.Framework;

namespace TUGraz.VectoCore.Tests.Utils
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class DoubleExtensionMethodTest
	{
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void DoubleExtensions_SI()
		{
			var val = 600.RPMtoRad();
			Assert.AreEqual(600 / 60 * 2 * Math.PI, val.Value());

			Assert.IsTrue(0.SI<PerSecond>().HasEqualUnit(val));

            var val2 = 1200.SI(Unit.SI.Rounds.Per.Minute).Cast<PerSecond>();
			val = val * 2;
			Assert.AreEqual(val, val2);

			val2 = val2 / 2;
			val = val / 2;
			Assert.AreEqual(val, val2);
		    Assert.AreEqual(600.SI(Unit.SI.Rounds.Per.Minute).Cast<PerSecond>(), val2);
		    Assert.AreEqual(600.SI(Unit.SI.Rounds.Per.Minute).Cast<PerSecond>().Value(), val2.Value());
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void DoubleExtension_ComparisonOperators()
		{
			Assert.IsTrue(0.0.IsEqual(0.0));
			Assert.IsTrue(1.0.IsGreater(0.0));
			Assert.IsTrue(1.0.IsGreaterOrEqual(1.0));
			Assert.IsTrue(1.0.IsPositive());
			Assert.IsTrue(0.0.IsSmaller(1.0));
			Assert.IsTrue(1.0.IsSmallerOrEqual(1.0));

			const double inTolerance = 1e-7;
			Assert.IsTrue(0.0.IsEqual(inTolerance));
			Assert.IsTrue(inTolerance.IsEqual(0.0));
			Assert.IsTrue(0.0.IsEqual(-inTolerance));
			Assert.IsTrue((-inTolerance).IsEqual(0.0));

			Assert.IsFalse(0.0.IsEqual(0.1));
			Assert.IsFalse(0.1.IsEqual(0.0));
			Assert.IsFalse(0.0.IsEqual(-0.1));
			Assert.IsFalse((-0.1).IsEqual(0.0));

			Assert.IsTrue(1.002.IsGreater(1.0));
			Assert.IsTrue(1.001.IsGreater(1.0));
			Assert.IsFalse(1.0.IsGreater(1.0));
			Assert.IsFalse(0.999.IsGreater(1.0));

			Assert.IsTrue(1.001.IsGreaterOrEqual(1.0));
			Assert.IsFalse(0.999.IsGreaterOrEqual(1.0));
			Assert.IsFalse(0.998.IsGreaterOrEqual(1.0));

			Assert.IsTrue(0.001.IsPositive());
			Assert.IsTrue(0.0.IsPositive());
			Assert.IsTrue((-inTolerance).IsPositive());
			Assert.IsFalse((-0.001).IsPositive());
			Assert.IsFalse((-0.002).IsPositive());

			Assert.IsTrue(0.998.IsSmaller(1.0));
			Assert.IsTrue(0.999.IsSmaller(1.0));
			Assert.IsFalse(1.0.IsSmaller(1.0));
			Assert.IsFalse(1.0011.IsSmaller(1.0));

			Assert.IsTrue((1 + inTolerance).IsSmallerOrEqual(1.0));
			Assert.IsFalse(1.001.IsSmallerOrEqual(1.0));
			Assert.IsTrue(0.999.IsSmallerOrEqual(1.0));
			Assert.IsTrue(0.998.IsSmallerOrEqual(1.0));
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestStringFormatting()
		{
			Assert.AreEqual("0.452", 0.452345.ToMinSignificantDigits(3, 1));
			Assert.AreEqual("4.52", 4.52345.ToMinSignificantDigits(3, 1));
			Assert.AreEqual("45.2", 45.2345.ToMinSignificantDigits(3, 1));

			Assert.AreEqual("0.0452", 0.0452345.ToMinSignificantDigits(3, 1));
			Assert.AreEqual("0.00452", 0.00452345.ToMinSignificantDigits(3, 1));


			Assert.AreEqual("-0.452", (-0.452345).ToMinSignificantDigits(3, 1));
			Assert.AreEqual("-4.52", (-4.52345).ToMinSignificantDigits(3, 1));
			Assert.AreEqual("-45.2", (-45.2345).ToMinSignificantDigits(3, 1));

			Assert.AreEqual("-0.0452", (-0.0452345).ToMinSignificantDigits(3, 1));
			Assert.AreEqual("-0.00452", (-0.00452345).ToMinSignificantDigits(3, 1));

			Assert.AreEqual("0.0", 0.0.ToMinSignificantDigits(3, 1));
			//Assert.AreEqual("45.2", 45.2345.ToMinSignificantDigits(3, 1));
		}
	}
}