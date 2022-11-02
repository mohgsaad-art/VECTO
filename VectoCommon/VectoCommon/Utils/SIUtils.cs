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

namespace TUGraz.VectoCommon.Utils
{
	public struct SIUtils
	{
		private const int UnitCount = 7;
		public static bool CompareUnits(int[] units1, int[] units2)
		{
			for (var i = 0; i < UnitCount; i++) {
				if (units1[i] != units2[i]) {
					return false;
				}
			}
			return true;
		}

		public static int[] CombineUnits(int[] units1, int[] units2)
		{
			var units = new int[UnitCount];
			for (var i = 0; i < UnitCount; i++) {
				units[i] = units1[i] + units2[i];
			}
			return units;
		}

		public static int[] MultiplyUnits(int[] units, int factor)
		{
			var result = new int[UnitCount];
			for (var i = 0; i < UnitCount; i++) {
				if (units[i] != 0) {
					result[i] = units[i] * factor;
				}
			}
			return result;
		}
	}


	public struct Unit
	{
		// TODO mk-2017-09-14: must be exact the same definition as in the SI class - find a way to de-duplicate this
		private static readonly string[] Unitnames = { "kg", "m", "s", "A", "K", "mol", "cd" };

		public static UnitInstance SI
		{
			get { return new UnitInstance(new[] { 0, 0, 0, 0, 0, 0, 0 }, 1, 1, 1); }
		}

		public static string GetUnitString(int[] siUnitParam)
		{
			var numerator = "";
			var denominator = "";

			for (var i = 0; i < siUnitParam.Length; i++) {
				var currentValue = siUnitParam[i];

				var exponent = Math.Abs(currentValue);
				var exponentStr = "";
				if (currentValue != 0) {
					var currentUnit = Unitnames[i];

					if (exponent > 1) {
						exponentStr = "^" + exponent;
					}

					if (currentValue > 0) {
						numerator += currentUnit + exponentStr;
					} else if (currentValue < 0) {
						denominator += currentUnit + exponentStr;
					}
				}
			}
			if (numerator == "" && denominator == "")
				return "-";
			if (numerator == "")
				return "1/" + denominator;
			if (denominator == "")
				return numerator;
			return numerator + "/" + denominator;
		}
	}

	public struct UnitInstance
	{
		/// <summary>
		/// kg, m, s, A, K, mol, cd
		/// </summary>
		private readonly int[] _units;

		private int _exponent;
		private int _reciproc;

		public double Factor { get; private set; }

		public UnitInstance(int[] units, double factor, int exponent, int reciproc)
		{
			_units = units;
			Factor = factor;
			_exponent = exponent;
			_reciproc = reciproc;
		}

		public int[] GetSIUnits()
		{
			return _units;
		}

		/// <summary>
		/// [g] (to basic unit: [kg])
		/// </summary>
		public UnitInstance Gramm
		{
			get
			{
				_units[0] += 1 * _reciproc * _exponent;

				// division because kg is the base unit (g/1000 = kg)
				Factor /= Math.Pow(1000, _reciproc * _exponent);

				// todo mk-2017-10-02: visual studio displays wrong units - check if still not working
				return this;
			}
		}

		/// <summary>
		/// Takes all following terms as cubic terms (=to the power of 3).
		/// </summary>
		public UnitInstance Cubic
		{
			get
			{
				_exponent = 3;
				return this;
			}
		}

		/// <summary>
		/// [s] Converts to/from Second. Internally everything is stored in seconds.
		/// </summary>
		public UnitInstance Hour
		{
			get
			{
				var reciprocAndExponent = _reciproc * _exponent;
				_units[2] += 1 * reciprocAndExponent;

				Factor *= Math.Pow(3600, reciprocAndExponent);

				return this;
			}
		}

		/// <summary>
		/// Quantifier for Kilo (1000).
		/// </summary>
		public UnitInstance Kilo
		{
			get
			{
				Factor *= Math.Pow(1000, _exponent * _reciproc);

				return this;
			}
		}

		/// <summary>
		/// Quantifier for Mega (1E6).
		/// </summary>
		public UnitInstance Mega
		{
			get {
				Factor *= Math.Pow(1E6, _exponent * _reciproc);

				return this;
			}
		}

		/// <summary>
		/// [m]
		/// </summary>
		public UnitInstance Meter
		{
			get
			{
				_units[1] += 1 * _reciproc * _exponent;
				return this;
			}
		}

		/// <summary>
		/// Quantifier for milli (1/1000).
		/// </summary>
		public UnitInstance Milli
		{
			get
			{
				Factor /= Math.Pow(1000, _exponent * _reciproc);
				return this;
			}
		}

		/// <summary>
		/// Quantifier for Centi (1/100)
		/// </summary>
		public UnitInstance Centi
		{
			get
			{
				Factor /= Math.Pow(100, _exponent * _reciproc);
				return this;
			}
		}

		/// <summary>
		/// Quantifier for Dezi (1/10)
		/// </summary>
		public UnitInstance Dezi
		{
			get
			{
				Factor /= Math.Pow(10, _exponent * _reciproc);
				return this;
			}
		}

		/// <summary>
		/// [s] Converts to/from Second. Internally everything is stored in seconds.
		/// </summary>
		public UnitInstance Minute
		{
			get
			{
				var reciprocAndExponent = _reciproc * _exponent;
				_units[2] += 1 * reciprocAndExponent;
				Factor *= Math.Pow(60, reciprocAndExponent);
				return this;
			}
		}

		/// <summary>
		/// [N]
		/// </summary>
		public UnitInstance Newton
		{
			get
			{
				var reciprocAndExponent = _reciproc * _exponent;
				_units[0] += 1 * reciprocAndExponent;
				_units[1] += 1 * reciprocAndExponent;
				_units[2] -= 2 * reciprocAndExponent;

				return this;
			}
		}

		/// <summary>
		/// Defines the denominator by the terms following after the Per.
		/// </summary>
		public UnitInstance Per
		{
			get
			{
				_exponent = 1;
				_reciproc = _reciproc * -1;
				return this;
			}
		}

		/// <summary>
		/// [-]. Defines radian. Only virtual. Has no real SI unit.
		/// </summary>
		public UnitInstance Radian => this;

		/// <summary>
		/// [-]. Converts to/from Radiant. Internally everything is stored in radian.
		/// </summary>
		public UnitInstance Rounds
		{
			get
			{
				Factor *= Math.Pow(2 * Math.PI, _exponent * _reciproc);
				return this;
			}
		}

		/// <summary>
		/// [s]
		/// </summary>
		public UnitInstance Second
		{
			get
			{
				_units[2] += 1 * _reciproc * _exponent;
				return this;
			}
		}

		public UnitInstance Ampere
		{
			get
			{
				_units[3] += 1 * _reciproc * _exponent;
				return this;
			}
		}

		/// <summary>
		/// Takes all following terms as quadratic terms (=to the power of 2).
		/// </summary>
		public UnitInstance Square
		{
			get
			{
				_exponent = 2;
				return this;
			}
		}

		/// <summary>
		/// [t] (to basic unit: [kg])
		/// </summary>
		public UnitInstance Ton
		{
			get
			{
				var reciprocAndExponent = _reciproc * _exponent;
				_units[0] += 1 * reciprocAndExponent;
				Factor *= Math.Pow(1000, reciprocAndExponent);

				return this;
			}
		}

		/// <summary>
		/// [W]
		/// </summary>
		public UnitInstance Watt
		{
			get
			{
				var reciprocAndExponent = _reciproc * _exponent;
				_units[0] += 1 * reciprocAndExponent;
				_units[1] += 2 * reciprocAndExponent;
				_units[2] -= 3 * reciprocAndExponent;

				return this;
			}
		}

		public UnitInstance Joule
		{
			get
			{
				var reciprocAndExponent = _reciproc * _exponent;
				_units[0] += 1 * reciprocAndExponent;
				_units[1] += 2 * reciprocAndExponent;
				_units[2] -= 2 * reciprocAndExponent;

				return this;
			}
		}

		public UnitInstance Liter
		{
			get
			{
				var reciprocAndExponent = _reciproc * _exponent;
				_units[1] += 3 * reciprocAndExponent;
				Factor /= Math.Pow(1000, reciprocAndExponent);

				return this;
			}
		}
	}
}