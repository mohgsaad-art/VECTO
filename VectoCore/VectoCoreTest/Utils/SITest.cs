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
using System.Diagnostics.CodeAnalysis;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using NUnit.Framework;

namespace TUGraz.VectoCore.Tests.Utils
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class SITest
	{
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		[SuppressMessage("ReSharper", "UnusedVariable")]
		public void SI_TypicalUsageTest()
		{
			//mult
			var angularVelocity = 600.RPMtoRad();
			var torque = 1500.SI<NewtonMeter>();
			var power = angularVelocity * torque;
			Assert.IsInstanceOf<Watt>(power);
			Assert.AreEqual(600.0 / 60 * 2 * Math.PI * 1500, power.Value());

			var siStandardMult = power * torque;
			Assert.IsInstanceOf<SI>(siStandardMult);
			Assert.AreEqual(600.0 / 60 * 2 * Math.PI * 1500 * 1500, siStandardMult.Value());
            Assert.IsTrue(siStandardMult.HasEqualUnit(new SI(Unit.SI.Watt.Newton.Meter)));

			//div
			var torque2 = power / angularVelocity;
			Assert.IsInstanceOf<NewtonMeter>(torque2);
			Assert.AreEqual(1500, torque2.Value());

			var siStandardDiv = power / power;
			Assert.IsInstanceOf<SI>(siStandardMult);
            Assert.IsTrue(siStandardDiv.HasEqualUnit(SIBase<Scalar>.Create(0)));
			Assert.AreEqual(600.0 / 60 * 2 * Math.PI * 1500 * 1500, siStandardMult.Value());

			var force = torque / 100.SI<Meter>();
			Assert.IsInstanceOf<Newton>(force);
			Assert.AreEqual(15, force.Value());

			var test = 2.0.SI<PerSecond>();
			var reziprok = 1.0 / test;
			Assert.AreEqual(0.5, reziprok.Value());
			Assert.IsTrue(1.SI<Second>().HasEqualUnit(reziprok));

			//add
			PerSecond angVeloSum = 600.RPMtoRad() + 400.SI<PerSecond>();
			AssertHelper.AreRelativeEqual(600 * 2 * Math.PI / 60 + 400, angVeloSum);
            AssertHelper.Exception<VectoException>(() => { var x = 500.SI(Unit.SI.Watt) + 300.SI(Unit.SI.Newton); });

			//subtract
			PerSecond angVeloDiff = 600.RPMtoRad() - 400.SI<PerSecond>();
			AssertHelper.AreRelativeEqual(600 * 2 * Math.PI / 60 - 400, angVeloDiff);


			//type conversion
			var engineSpeed = 600.0;
			PerSecond angularVelocity3 = engineSpeed.RPMtoRad();

			// convert between units measures
            var angularVelocity4 = engineSpeed.SI(Unit.SI.Rounds.Per.Minute);
			Assert.IsInstanceOf<SI>(angularVelocity4);

			// cast SI to specialized unit classes.
			PerSecond angularVelocity5 = angularVelocity4.Cast<PerSecond>();
			Assert.AreEqual(angularVelocity3, angularVelocity5);
			Assert.AreEqual(angularVelocity3.Value(), angularVelocity4.Value());

			// Cast only allows the cast if the units are correct.
            AssertHelper.Exception<VectoException>(() => { var x = 40.SI(Unit.SI.Newton).Cast<Watt>(); });
            var res2 = 40.SI(Unit.SI.Newton).Cast<Newton>();
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Test()
		{
			var si = 0.SI();
			Assert.AreEqual(0.0, si.Value());
			Assert.AreEqual("0.0000 [-]", si.ToString());
            Assert.IsTrue(si.HasEqualUnit(SIBase<Scalar>.Create(0)));

            var si2 = 5.SI(Unit.SI.Watt);
            Assert.AreEqual("5.0000 [kgm^2/s^3]", si2.ToString());

            var si3 = 2.SI(Unit.SI.Radian.Per.Second);
			Assert.AreEqual("2.0000 [1/s]", si3.ToString());

			var si4 = si2 * si3;
            Assert.AreEqual("10.0000 [kgm^2/s^4]", si4.ToString());
            Assert.IsTrue(si4.HasEqualUnit(new SI(Unit.SI.Watt.Per.Second)));
			Assert.AreEqual("10.0000 [kgm^2/s^4]", si4.ToBasicUnits().ToString());

			var x = 5.SI();
			Assert.AreEqual((2.0 / 5.0).SI(), 2 / x);
			Assert.AreEqual((5.0 / 2.0).SI(), x / 2);
			Assert.AreEqual((2.0 * 5.0).SI(), 2 * x);
			Assert.AreEqual((5.0 * 2.0).SI(), x * 2);

			Assert.AreEqual((2.0 / 5.0).SI(), 2.0 / x);
			Assert.AreEqual((5.0 / 2.0).SI(), x / 2.0);
			Assert.AreEqual((2 * 5).SI(), 2.0 * x);
			Assert.AreEqual((5 * 2).SI(), x * 2.0);

			//var y = 2.SI();
			//Assert.AreEqual((2 * 5).SI(), y * x);

			//var percent = 10.SI<Radian>().ConvertTo().GradientPercent;
			//Assert.AreEqual(67.975.ToString("F3") + " [Percent]", percent.ToString("F3"));
			//Assert.AreEqual(67.975, percent.Value(), 0.001);

			Assert.AreEqual(45.0 / 180.0 * Math.PI, VectoMath.InclinationToAngle(1).Value(), 0.000001);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		[SuppressMessage("ReSharper", "UnusedVariable")]
		public void SI_Comparison_Operators()
		{
			var v1 = 600.SI<NewtonMeter>();
			var v2 = 455.SI<NewtonMeter>();
			var v3 = 600.SI<NewtonMeter>();
			var v4 = 100.SI<Watt>();
			var d = 700;

			v1.ToString();

			Assert.IsTrue(v1 > v2);
			Assert.IsFalse(v1 < v2);
			AssertHelper.Exception<VectoException>(() => { var x = v1 < v4; },
                "Operator '<' can only operate on SI Objects with the same unit. Got: 600.0000 [Nm] < 100.0000 [W]");
			AssertHelper.Exception<VectoException>(() => { var x = v1 > v4; },
                "Operator '>' can only operate on SI Objects with the same unit. Got: 600.0000 [Nm] > 100.0000 [W]");
			AssertHelper.Exception<VectoException>(() => { var x = v1 <= v4; },
				"Operator '<=' can only operate on SI Objects with the same unit. Got: 600.0000 [Nm] <= 100.0000 [W]");
			AssertHelper.Exception<VectoException>(() => { var x = v1 >= v4; },
				"Operator '>=' can only operate on SI Objects with the same unit. Got: 600.0000 [Nm] >= 100.0000 [W]");

			SI si = null;
			Assert.IsFalse(si > 3);
			Assert.IsFalse(si < 3);
			Assert.IsFalse(si >= 3);
			Assert.IsFalse(si <= 3);

			Assert.IsFalse(3 > si);
			Assert.IsFalse(3 < si);
			Assert.IsFalse(si >= 3);
			Assert.IsFalse(si <= 3);

			Assert.IsTrue(v2 < v1);
			Assert.IsFalse(v2 > v1);

			Assert.IsTrue(v1 >= v2);
			Assert.IsFalse(v1 <= v2);

			Assert.IsTrue(v2 <= v1);
			Assert.IsFalse(v2 >= v1);

			Assert.IsTrue(v1 <= v3);
			Assert.IsTrue(v1 >= v3);

			Assert.IsTrue(v1 < d);
			Assert.IsFalse(v1 > d);
			Assert.IsFalse(v1 >= d);
			Assert.IsTrue(v1 <= d);

            Assert.AreEqual(1, 0.SI().CompareTo(null));
            Assert.AreEqual(1, 0.SI().CompareTo("not an SI"));
			Assert.AreEqual(-1, new SI(Unit.SI.Meter).CompareTo(new SI(Unit.SI.Kilo.Meter.Per.Hour)));
			Assert.AreEqual(1, new SI(Unit.SI.Newton.Meter).CompareTo(new SI(Unit.SI.Meter)));

			Assert.AreEqual(0, 1.SI().CompareTo(1.SI()));
			Assert.AreEqual(-1, 1.SI().CompareTo(2.SI()));
			Assert.AreEqual(1, 2.SI().CompareTo(1.SI()));
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		[SuppressMessage("ReSharper", "UnusedVariable")]
		public void SI_Test_Addition_Subtraction()
		{
			AssertHelper.AreRelativeEqual(3.SI(), 1.SI() + 2.SI());
			AssertHelper.AreRelativeEqual(-1.SI(), 1.SI() - 2.SI());

			AssertHelper.AreRelativeEqual(3.SI<Scalar>(), 1.SI<Scalar>() + 2.SI<Scalar>());
			AssertHelper.AreRelativeEqual(3.SI<Scalar>(), 1 + 2.SI<Scalar>());
			AssertHelper.AreRelativeEqual(3.SI<Scalar>(), 1.SI<Scalar>() + 2);
			AssertHelper.AreRelativeEqual(-1.SI<Scalar>(), 1.SI<Scalar>() - 2.SI<Scalar>());
			AssertHelper.AreRelativeEqual(-1.SI<Scalar>(), 1 - 2.SI<Scalar>());
			AssertHelper.AreRelativeEqual(-1.SI<Scalar>(), 1.SI<Scalar>() - 2);

			AssertHelper.AreRelativeEqual(3.SI<NewtonMeter>(), 1.SI<NewtonMeter>() + 2.SI<NewtonMeter>());
			AssertHelper.AreRelativeEqual(-1.SI<NewtonMeter>(), 1.SI<NewtonMeter>() - 2.SI<NewtonMeter>());

            AssertHelper.AreRelativeEqual(3.SI<NewtonMeter>(), 1.SI(Unit.SI.Newton.Meter) + 2.SI<NewtonMeter>());
            AssertHelper.AreRelativeEqual(-1.SI<NewtonMeter>(), 1.SI(Unit.SI.Newton.Meter) - 2.SI<NewtonMeter>());

            AssertHelper.AreRelativeEqual(3.SI<NewtonMeter>(), 1.SI<NewtonMeter>() + 2.SI(Unit.SI.Newton.Meter));
            AssertHelper.AreRelativeEqual(-1.SI<NewtonMeter>(), 1.SI<NewtonMeter>() - 2.SI(Unit.SI.Newton.Meter));

            AssertHelper.Exception<VectoException>(() => { var x = 1.SI(Unit.SI.Second) - 1.SI<Meter>(); },
				"Operator '-' can only operate on SI Objects with the same unit. Got: 1.0000 [s] - 1.0000 [m]");
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_SpecialUnits()
		{
			Scalar scalar = 3.SI<Scalar>();
			AssertHelper.AreRelativeEqual(3.SI(), scalar);
			double scalarDouble = scalar;
			AssertHelper.AreRelativeEqual(3, scalarDouble);

			MeterPerSecond meterPerSecond = 2.SI<MeterPerSecond>();
            AssertHelper.AreRelativeEqual(2.SI(Unit.SI.Meter.Per.Second), meterPerSecond);

			Second second = 1.SI<Second>();
            AssertHelper.AreRelativeEqual(1.SI(Unit.SI.Second), second);

			Watt watt = 2.SI<Watt>();
            AssertHelper.AreRelativeEqual(2.SI(Unit.SI.Watt), watt);

			PerSecond perSecond = 1.SI<PerSecond>();
            AssertHelper.AreRelativeEqual(1.SI(Unit.SI.Per.Second), perSecond);

            SI rpm = 20.SI(Unit.SI.Rounds.Per.Minute);
            AssertHelper.AreRelativeEqual(20.SI(Unit.SI.Rounds.Per.Minute), rpm);
			AssertHelper.AreRelativeEqual(20.RPMtoRad(), rpm);
			AssertHelper.AreRelativeEqual(2.0943951023931953, rpm);

			Radian radian = 30.SI<Radian>();
            AssertHelper.AreRelativeEqual(30.SI(Unit.SI.Radian), radian);
			AssertHelper.AreRelativeEqual(30, radian);

			Newton newton = 3.SI<Newton>();
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Newton), newton);

			NewtonMeter newtonMeter = 5.SI<NewtonMeter>();
            AssertHelper.AreRelativeEqual(5.SI(Unit.SI.Newton.Meter), newtonMeter);
            AssertHelper.AreRelativeEqual(5.SI(Unit.SI.Meter.Newton), newtonMeter);

			MeterPerSquareSecond meterPerSquareSecond = 3.SI<MeterPerSquareSecond>();
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Meter.Per.Square.Second), meterPerSquareSecond);

			Kilogram kilogram = 3.SI<Kilogram>();
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Kilo.Gramm), kilogram);
			AssertHelper.AreRelativeEqual(3, kilogram);

			SquareMeter squareMeter = 3.SI<SquareMeter>();
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Square.Meter), squareMeter);

			CubicMeter cubicMeter = 3.SI<CubicMeter>();
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Cubic.Meter), cubicMeter);

			KilogramSquareMeter kilogramSquareMeter = 3.SI<KilogramSquareMeter>();
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Kilo.Gramm.Square.Meter), kilogramSquareMeter);

			KilogramPerWattSecond kilogramPerWattSecond = 3.SI<KilogramPerWattSecond>();
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Kilo.Gramm.Per.Watt.Second), kilogramPerWattSecond);

			WattPerSecond wattPerSecond = 5.SI<WattPerSecond>();
			AssertHelper.AreRelativeEqual(5.SI(Unit.SI.Watt.Per.Second), wattPerSecond);
		}

		/// <summary>
		/// VECTO-111
		/// </summary>
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_ReziprokDivision()
		{
			var test = 2.0.SI<Second>();

			var actual = 1.0 / test;
			var expected = 0.5.SI<PerSecond>();

			AssertHelper.AreRelativeEqual(expected, actual);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Multiplication_Division()
		{
			AssertHelper.AreRelativeEqual(12.SI(), 3.SI() * 4.SI());
			AssertHelper.AreRelativeEqual(12.SI(), 3 * 4.SI());
			AssertHelper.AreRelativeEqual(12.SI(), 3.SI() * 4);

			AssertHelper.AreRelativeEqual(12.SI<NewtonMeter>(), 3.SI<Newton>() * 4.SI<Meter>());
			AssertHelper.AreRelativeEqual(12.SI<NewtonMeter>(), 3 * 4.SI<NewtonMeter>());
			AssertHelper.AreRelativeEqual(12.SI<NewtonMeter>(), 3.SI<NewtonMeter>() * 4);
            AssertHelper.AreRelativeEqual(12.SI(Unit.SI.Square.Newton.Meter), 3.SI<NewtonMeter>() * 4.SI<NewtonMeter>());

			AssertHelper.AreRelativeEqual(3.SI(), 12.SI() / 4);
			AssertHelper.AreRelativeEqual(3.SI(), 12.SI() / 4.SI());
			AssertHelper.AreRelativeEqual(3.SI(), 12.SI<NewtonMeter>() / 4.SI<NewtonMeter>());

			AssertHelper.AreRelativeEqual(3.SI<NewtonMeter>(), 12.SI<NewtonMeter>() / 4);
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Per.Newton.Meter), 12 / 4.SI<NewtonMeter>());

			var newtonMeter = 10.SI<NewtonMeter>();
			var perSecond = 5.SI<PerSecond>();
			var watt = (10 * 5).SI<Watt>();
			var second = (1.0 / 5.0).SI<Second>();

			AssertHelper.AreRelativeEqual(watt, newtonMeter * perSecond);
			AssertHelper.AreRelativeEqual(watt, perSecond * newtonMeter);

			AssertHelper.AreRelativeEqual(newtonMeter, watt / perSecond);
			AssertHelper.AreRelativeEqual(perSecond, watt / newtonMeter);

			AssertHelper.AreRelativeEqual(second, newtonMeter / watt);


			AssertHelper.AreRelativeEqual(2.SI<NormLiterPerSecond>(), 2.SI<NormLiterPerSecond>() / 1);

			AssertHelper.AreRelativeEqual(2.SI<NormLiterPerSecond>(), 2.SI<NormLiterPerSecond>() * 1);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_MeterPerSecond_Div_Meter()
		{
			PerSecond actual = 6.SI<MeterPerSecond>() / 2.SI<Meter>();
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Per.Second), actual);
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_SimplifyUnits()
		{
            AssertHelper.AreRelativeEqual(3.SI(), 18.SI(Unit.SI.Kilo.Gramm) / 6.SI(Unit.SI.Kilo.Gramm));
			AssertHelper.AreRelativeEqual(3.SI(), 18.SI<NewtonMeter>() / 6.SI<NewtonMeter>());

            AssertHelper.AreRelativeEqual(18.SI(), 3.SI(Unit.SI.Kilo.Gramm) * 6.SI(Unit.SI.Per.Kilo.Gramm));
            AssertHelper.AreRelativeEqual(18.SI<Meter>(), 3.SI(Unit.SI.Kilo.Gramm.Meter) * 6.SI(Unit.SI.Per.Kilo.Gramm));

            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Kilo.Gramm.Square.Meter.Per.Cubic.Second), 3.SI<Watt>());
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Kilo.Gramm.Meter.Per.Square.Second), 3.SI<Newton>());
            AssertHelper.AreRelativeEqual(3000.SI(Unit.SI.Kilo.Gramm), 3.SI(Unit.SI.Ton));
            AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Kilo.Kilo.Gramm).Cast<Kilogram>().ConvertToTon(), 3000.SI(Unit.SI.Kilo.Gramm).Cast<Kilogram>().ConvertToTon());

            AssertHelper.AreRelativeEqual(3.SI<Meter>(), 3000.SI(Unit.SI.Milli.Meter));

            AssertHelper.AreRelativeEqual(36.SI(Unit.SI.Square.Newton.Meter), 6.SI<NewtonMeter>() * 6.SI<NewtonMeter>());
            AssertHelper.AreRelativeEqual(36.SI(Unit.SI.Newton.Newton.Meter.Meter), 6.SI<NewtonMeter>() * 6.SI<NewtonMeter>());

            // todo not testable !!!
            // AssertHelper.AreRelativeEqual(3.SI(Unit.SI.Meter.Per.Second), 3.SI<Newton>(Unit.SI.Second.Per.Kilo.Gramm));
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Math()
		{
			AssertHelper.AreRelativeEqual(-3, -3.SI().Value());
			AssertHelper.AreRelativeEqual(3.SI(), (-3).SI().Abs());
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		[SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
		public void SI_Equality()
		{
			Assert.AreEqual(3.SI(), 3.SI());
			Assert.AreEqual(3.SI<NewtonMeter>(), 3.SI<NewtonMeter>());

			Assert.IsFalse(3.SI<NewtonMeter>().IsEqual(4.SI<NewtonMeter>()));
			Assert.IsFalse(3.SI<NewtonMeter>().IsEqual(3.SI<Meter>()));

			Assert.IsTrue(3.SI().IsEqual(4, 10));

			var x = 4.SI();
			var y = x;
			var z = 4.SI();
			Assert.IsTrue(x.Equals(y));

			Assert.IsFalse(3.SI().Equals(null));
			Assert.IsFalse(3.SI().IsEqual(4.SI()));
			Assert.IsTrue(z.Equals(x));
			Assert.IsFalse(3.SI().Equals(3.SI<Newton>()));

			var newton1 = 3.SI<Newton>();
			var newton2 = 3.SI<Newton>();
			Assert.IsTrue(newton1.Equals(newton2));

			Assert.IsTrue(3.SI().IsEqual(3.SI()));
			Assert.IsTrue(3.SI().IsEqual(3));

			Assert.IsFalse(3.SI().IsEqual(2.9.SI()));
			Assert.IsFalse(3.SI().IsEqual(2.9));

			// just calling to test wether the functions are not throwing an exception.
			3.SI().GetHashCode();
			3.0.SI().GetHashCode();
			4.SI<NewtonMeter>().GetHashCode();
		}

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Output()
		{
			Assert.AreEqual("3.0000", 3.SI().ToOutputFormat());
			Assert.AreEqual("3.0000 [-]", 3.SI().ToOutputFormat(showUnit: true));
			Assert.AreEqual("3.5000", 3.5.SI().ToOutputFormat());
			Assert.AreEqual("3.5000", 3.5.SI<Newton>().ToOutputFormat());
            Assert.AreEqual("3.50 [N]", 3.5.SI<Newton>().ToOutputFormat(2, showUnit: true));
			Assert.AreEqual("18.00 [m/s]", 5.SI<MeterPerSecond>().ToOutputFormat(2, 3.6, true));
			Assert.AreEqual("18.0000", 5.SI<MeterPerSecond>().ToOutputFormat(outputFactor: 3.6));

			Assert.AreEqual("10.0000 [m^2]", 10.SI<SquareMeter>().ToOutputFormat(showUnit: true));

			Assert.AreEqual("10.0000 [m^3]", 10.SI<CubicMeter>().ToOutputFormat(showUnit: true));

			Assert.AreEqual("0.5000 [m/s^2]", 0.5.SI<MeterPerSquareSecond>().ToOutputFormat(showUnit: true));
		}
		
		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
        public void SI_Hash()
        {
            3.SI().GetHashCode();
            3.0.SI().GetHashCode();
            4.SI<NewtonMeter>().GetHashCode();
        }

        [TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
        public void SI_ConstructorPerformance()
        {
            for (var i = 0; i < 5e5; i++)
            {
                var si = i.SI();
                var meter = i.SI<Meter>();
                var watt = i.SI<Watt>();
                var perSecond = i.SI<PerSecond>();
                var meterPerSecond = i.SI<MeterPerSecond>();
                var second = i.SI<Second>();
                var newton = i.SI<Newton>();
                var kilogram = i.SI<Kilogram>();
                var squareMeter = i.SI<SquareMeter>();
                var scalar = i.SI<Scalar>();
                var compound = i.SI(Unit.SI.Kilo.Gramm.Square.Meter.Per.Cubic.Second).Cast<Watt>();
            }

        }

        [TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
        public void SI_CheckForEqualUnitPerformance()
        {

            var si1 = 5.SI<NewtonMeter>();
            var si2 = 5.SI(Unit.SI.Newton.Meter);
            for (var i = 0; i < 1e7; i++)
            {
                si1.HasEqualUnit(si2);
            }


            var si3 = 5.SI<NewtonMeter>();
            var si4 = 5.SI<Kilogram>();
            for (var i = 0; i < 2e6; i++)
            {
                si3.HasEqualUnit(si4);
            }

        }


        [TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
        public void SI_NeutralArithmeticPerformance()
        {

            var transmissionCoefficient = 2.8;
            for (var i = 0; i < 2e5; i++)
            {
                var angularVelocity = 1.5.SI(Unit.SI.Per.Second);
                var torque = 50.SI(Unit.SI.Newton);
                var power = torque * angularVelocity;

                var outAngularVelocity = angularVelocity / transmissionCoefficient;
                var outTorque = torque * transmissionCoefficient;

                var outPower = outTorque * outAngularVelocity;

                var averagePower = (power + outPower) / 2;
                AssertHelper.AreRelativeEqual(outPower, power);
                AssertHelper.AreRelativeEqual(averagePower, power);
            }
        }

        [TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
        public void SI_SpecialArithmeticPerformance()
        {

            var transmissionCoefficient = 2.8;
            for (var i = 0; i < 2e5; i++)
            {
                var angularVelocity = 1.5.SI<PerSecond>();
                var torque = 50.SI<Newton>();
                var power = torque * angularVelocity;

                var outAngularVelocity = angularVelocity / transmissionCoefficient;
                var outTorque = torque * transmissionCoefficient;

                var outPower = outTorque * outAngularVelocity;

                var averagePower = (power + outPower) / 2;
                AssertHelper.AreRelativeEqual(outPower, power);
                AssertHelper.AreRelativeEqual(averagePower, power);
            }
        }

        [TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
        public void SI_NewTests()
        {
            UnitInstance sikg = Unit.SI.Kilo.Gramm;
            Assert.AreEqual("kg", SI.GetUnitString(sikg.GetSIUnits()));

            UnitInstance ui1 = Unit.SI.Kilo.Gramm.Meter.Per.Square.Second;
            Assert.AreEqual("kgm/s^2", SI.GetUnitString(ui1.GetSIUnits()));

            UnitInstance ui3 = Unit.SI.Kilo.Gramm.Per.Watt.Second;
            Assert.AreEqual("s^2/m^2", SI.GetUnitString(ui3.GetSIUnits()));
            Assert.AreEqual(1, ui3.Factor);

            var kg = 3000.SI(Unit.SI.Kilo.Gramm).Cast<Kilogram>();
            Assert.AreEqual("3000.0000 [kg]", kg.ToOutputFormat(showUnit: true));

            var ton = 3.SI(Unit.SI.Ton);
            Assert.AreEqual("3000.0000 [kg]", ton.ToOutputFormat(showUnit: true));

            var val1 = 7.SI(Unit.SI.Cubic.Dezi.Meter);
            Assert.AreEqual("0.0070 [m^3]", val1.ToOutputFormat(showUnit: true));

            var uni = Unit.SI.Cubic.Dezi.Meter;
            Assert.AreEqual("m^3", SI.GetUnitString(uni.GetSIUnits()));
            AssertHelper.AreRelativeEqual(0.001, uni.Factor);

            var uni2 = Unit.SI.Cubic.Centi.Meter;
            Assert.AreEqual("m^3", SI.GetUnitString(uni2.GetSIUnits()));
            AssertHelper.AreRelativeEqual(0.000001, uni2.Factor);

           var uni1 = Unit.SI.Kilo.Meter.Per.Hour;
            Assert.AreEqual("m/s", SI.GetUnitString(uni1.GetSIUnits()));
            AssertHelper.AreRelativeEqual(0.2777777777, uni1.Factor);

            NewtonMeter newtonMeter = 5.SI<NewtonMeter>();
            AssertHelper.AreRelativeEqual(5.SI(Unit.SI.Newton.Meter), newtonMeter);
            AssertHelper.AreRelativeEqual(5.SI(Unit.SI.Meter.Newton), newtonMeter);

			AssertHelper.AreRelativeEqual(5.SI<Liter>(), 1.SI<Second>() * 5.SI<LiterPerSecond>());

			AssertHelper.AreRelativeEqual((6.0/3600).SI<Liter>(), (2.SI<Second>() * 3.SI(Unit.SI.Liter.Per.Hour)).Cast<Liter>());

			AssertHelper.AreRelativeEqual(2.13093, 2.13093.SI(Unit.SI.Liter).Cast<Liter>().Value());
			Assert.AreEqual("m^3", 2.13093.SI(Unit.SI.Liter).UnitString);
		}

		//[TestCase]
  //      public void SI_ConvertValues()
  //      {
  //          var sig1 = 5.SI(Unit.SI.Gramm);
  //          Assert.AreEqual(0.005, sig1.Value());

		//	var val2 = 7.SI(Unit.SI.Cubic.Dezi.Meter).Cast<CubicMeter>().ConvertToCubicDeziMeter();
		//	AssertHelper.AreRelativeEqual(7, val2);

		//	var val3 = 5.SI(Unit.SI.Cubic.Dezi.Meter).Cast<CubicMeter>().ConvertToCubicCentiMeter();
		//	AssertHelper.AreRelativeEqual(5000, val3); // 5000 cm^3

		//	var val4 = 5.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>().ConvertToCubicDeziMeter();
		//	AssertHelper.AreRelativeEqual(0.005, val4); // 0.005 dm^3
		//}

		[TestCase(1, 1000),
		TestCase(1e-3, 1),
		TestCase(2.65344, 2653.44),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToGramm(double val, double converted)
		{
			var siVal = val.SI<Kilogram>();
			var siConv = siVal.ConvertToGramm();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("g", siConv.Units);
		}

		[TestCase(1000, 1),
		TestCase(1, 1e-3),
		TestCase(5243, 5.243),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToTon(double val, double converted)
		{
			var siVal = val.SI<Kilogram>();
			var siConv = siVal.ConvertToTon();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("t", siConv.Units);
		}

		[TestCase(1, 3.6),
		TestCase(0.2777777777777777, 1),
		TestCase(13.7603, 49.53708),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToKiloMeterPerHour(double val, double converted)
		{
			var siVal = val.SI<MeterPerSecond>();
			var siConv = siVal.ConvertToKiloMeterPerHour();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("km/h", siConv.Units);
		}

		[TestCase(1, 1e6),
		TestCase(1e-6, 1),
		TestCase(7.54214451, 7542144.51),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToGrammPerKiloMeter(double val, double converted)
		{
			var siVal = val.SI<KilogramPerMeter>();
			var siConv = siVal.ConvertToGrammPerKiloMeter();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("g/km", siConv.Units);
		}

		[TestCase(1, 1e8),
		TestCase(1e-8, 1),
		TestCase(0.00935934235, 935934.235),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToLiterPer100Kilometer(double val, double converted)
		{
			var siVal = val.SI<VolumePerMeter>();
			var siConv = siVal.ConvertToLiterPer100Kilometer();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("l/100km", siConv.Units);
		}

		[TestCase(1, 1e11),
		TestCase(1e-11, 1),
		TestCase(0.00013243241234, 13243241.234),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToLiterPer100TonKiloMeter(double val, double converted)
		{
			var siVal = val.SI<VolumePerMeterMass>();
			var siConv = siVal.ConvertToLiterPer100TonKiloMeter();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("l/100tkm", siConv.Units);
		}

		[TestCase(1, 1e8),
		TestCase(1e-8, 1),
		TestCase(0.13243241234, 13243241.234),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToLiterPerCubicMeter100KiloMeter(double val, double converted)
		{
			var siVal = val.SI<VolumePerMeterVolume>();
			var siConv = siVal.ConvertToLiterPerCubicMeter100KiloMeter();
			Assert.AreEqual(converted, siConv, 1e-6);
			Assert.AreEqual("l/100m³-km", siConv.Units);
		}

		[TestCase(1,3.6e6),
		TestCase(0.277777777777777777e-6,1),
		TestCase(0.0135897845, 13589.7845*3.6),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToGrammPerHour(double val, double converted)
		{
			var siVal = val.SI<KilogramPerSecond>();
			var siConv = siVal.ConvertToGrammPerHour();
			Assert.AreEqual(converted, siConv, 1e-6);
			Assert.AreEqual("g/h", siConv.Units);
		}

		[TestCase(1, 1e-3),
		TestCase(1e3, 1),
		TestCase(4353.32, 4.35332),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToKiloMeter(double val, double converted)
		{
			var siVal = val.SI<Meter>();
			var siConv = siVal.ConvertToKiloMeter();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("km", siConv.Units);
		}

		[TestCase(1, 1e6),
		TestCase(1e-6, 1),
		TestCase(0.053798513789, 53798.513789),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToCubicCentiMeter(double val, double converted)
		{
			var siVal = val.SI<CubicMeter>();
			var siConv = siVal.ConvertToCubicCentiMeter();
			Assert.AreEqual(converted, siConv, 1e-6);
			Assert.AreEqual("cm³", siConv.Units);
		}

		[TestCase(1, 1e6),
		TestCase(1e-6, 1),
		TestCase(7.54214451, 7542144.51),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToGrammPerCubicMeterKiloMeter(double val, double converted)
		{
			var siVal = val.SI<KilogramPerMeterCubicMeter>();
			var siConv = siVal.ConvertToGrammPerCubicMeterKiloMeter();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("g/m³-km", siConv.Units);
		}

		[TestCase(1, 1e9),
		TestCase(1e-9, 1),
		TestCase(7.54214451, 7542144510),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToGrammPerTonKilometer(double val, double converted)
		{
			var siVal = val.SI<KilogramPerMeterMass>();
			var siConv = siVal.ConvertToGrammPerTonKilometer();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("g/t-km", siConv.Units);
		}

		[TestCase(1, 0.277777777777e-6),
		TestCase(3600e3, 1),
		TestCase(135890, 0.0377472222),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToKiloWattHour(double val, double converted)
		{
			var siVal = val.SI<WattSecond>();
			var siConv = siVal.ConvertToKiloWattHour();
			Assert.AreEqual(converted, siConv, 1e-6);
			Assert.AreEqual("kWh", siConv.Units);
		}

		[TestCase(1, 1e-3),
		TestCase(1e3, 1),
		TestCase(23453, 23.453),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToKiloWatt(double val, double converted)
		{
			var siVal = val.SI<Watt>();
			var siConv = siVal.ConvertToKiloWatt();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("kW", siConv.Units);
		}

		[TestCase(1, 9.549296586),
		TestCase(0.104719755, 1),
		TestCase(62.83185307, 600),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToRoundsPerMinute(double val, double converted)
		{
			var siVal = val.SI<PerSecond>();
			var siConv = siVal.ConvertToRoundsPerMinute();
			Assert.AreEqual(converted, siConv, 1e-6);
			Assert.AreEqual(siVal.Value(), ((double)siConv).RPMtoRad().Value());
			Assert.AreEqual("rpm", siConv.Units);
		}

		[TestCase(1, 1e3),
		TestCase(1e-3, 1),
		TestCase(123.780, 123780),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToCubicDeziMeter(double val, double converted)
		{
			var siVal = val.SI<CubicMeter>();
			var siConv = siVal.ConvertToCubicDeziMeter();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("dm^3", siConv.Units);
		}

		[TestCase(1, 1e3),
		TestCase(1e-3, 1),
		TestCase(0.255, 255),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToMilliMeter(double val, double converted)
		{
			var siVal = val.SI<Meter>();
			var siConv = siVal.ConvertToMilliMeter();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("mm", siConv.Units);
		}


		[TestCase(0.2/1000/3600, 200),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void SI_Convert_ConvertToGramPerKiloWattHour(double val, double converted)
		{
			var siVal = val.SI<SpecificFuelConsumption>();
			var siConv = siVal.ConvertToGramPerKiloWattHour();
			Assert.AreEqual(converted, siConv, 1e-12);
			Assert.AreEqual("g/kWh", siConv.Units);
		}

	}

}