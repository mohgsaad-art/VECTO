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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TUGraz.VectoCommon.Utils
{
	/// <summary>
	/// Extension methods for double.
	/// </summary>
	public static class DoubleExtensionMethods
	{
		/// <summary>
		/// The tolerance.
		/// </summary>
		public const double Tolerance = 1e-6;

		/// <summary>
		/// The tolerancefactor for relative comparisons.
		/// </summary>
		public const double ToleranceFactor = 1e-6;

		/// <summary>
		/// The tolerancefactor for relative comparisons.
		/// </summary>
		public const double VectoToleranceFactor = 0.003;

		/// <summary>
		/// Determines whether the specified other is equal within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsEqual(this double self, double other, double tolerance = Tolerance)
		{
			return Math.Abs(self - other) < tolerance;
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRelativeEqual(this SI expected, SI actual, double toleranceFactor = ToleranceFactor)
		{
			return IsRelativeEqual(expected.Value(), actual.Value(), toleranceFactor: toleranceFactor);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsRelativeEqual(this double expected, double actual,
			double toleranceFactor = ToleranceFactor)
		{
			if (double.IsNaN(expected)) {
				return double.IsNaN(actual);
			}

			var ratio = expected.IsEqual(0, toleranceFactor) ? Math.Abs(actual) : Math.Abs(actual / expected - 1);
			return ratio < toleranceFactor;
		}

		/// <summary>
		/// Determines whether the specified other is smaller within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSmaller(this double self, double other, double tolerance = Tolerance)
		{
			return self < other - tolerance;
		}

		/// <summary>
		/// Determines whether the specified other is smaller or equal within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsSmallerOrEqual(this double self, double other, double tolerance = Tolerance)
		{
			return self <= other + tolerance;
		}

		/// <summary>
		/// Determines whether the specified other is greater within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsGreater(this double self, double other, double tolerance = Tolerance)
		{
			return self > other + tolerance;
		}

		/// <summary>
		/// Determines whether the specified other is greater or equal within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="other">The other.</param>
		/// <param name="tolerance">The tolerance.</param>
		/// <returns></returns>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsGreaterOrEqual(this double self, double other, double tolerance = Tolerance)
		{
			return self >= other - tolerance;
		}

		/// <summary>
		/// Determines whether the specified tolerance is positive within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="tolerance">The tolerance.</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsPositive(this double self, double tolerance = Tolerance)
		{
			return self >= -tolerance;
		}

		/// <summary>
		/// Determines whether the specified tolerance is positive within tolerance.
		/// </summary>
		/// <param name="self">The self.</param>
		/// <param name="tolerance">The tolerance.</param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNegative(this double self, double tolerance = Tolerance)
		{
			return self <= tolerance;
		}

		/// <summary>
		/// Checks if a value is between min and max (min &lt;= value &lt;= max)
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBetween(this double self, double min, double max)
		{
			return Math.Min(min, max) <= self && self <= Math.Max(min, max);
		}

		/// <summary>
		/// Checks if a value is between min and max (min &lt;= value &lt;= max)
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBetween(this double self, SI min, SI max)
		{
			return VectoMath.Min(min, max) <= self && self <= VectoMath.Max(min, max);
		}

		/// <summary>
		/// Converts the double-value from RPM (rounds per minute) to the SI Unit PerSecond.
		/// </summary>
		/// <param name="self"></param>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PerSecond RPMtoRad(this double self)
		{
			return SI<PerSecond>(self * 2 * Math.PI / 60.0);
		}

		public static Kelvin DegCelsiusToKelvin(this double self)
		{
			return SI<Kelvin>(self + 273.16);
		}

		public static Kelvin DegCelsiusToKelvin(this int self)
		{
			return SI<Kelvin>(self + 273.16);
		}

		/// <summary>
		/// Converts the double-value from RPM (rounds per minute) to the SI Unit PerSecond.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PerSecond RPMtoRad(this float self)
		{
			return SI<PerSecond>(self * 2 * Math.PI / 60.0);
		}

		/// <summary>
		/// Converts the value from rounds per minute to the SI Unit PerSecond
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static PerSecond RPMtoRad(this int self)
		{
			return SI<PerSecond>(self * 2.0 * Math.PI / 60.0);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MeterPerSecond KMPHtoMeterPerSecond(this double self)
		{
			return SI<MeterPerSecond>(self / 3.6);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MeterPerSecond KMPHtoMeterPerSecond(this int self)
		{
			return SI<MeterPerSecond>(self / 3.6);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double ToDegree(this double self)
		{
			return self * 180.0 / Math.PI;
		}

		/// <summary>
		/// Creates an SI object for the number (unit-less: [-]).
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SI SI(this double value)
		{
			return SIBase<Scalar>.Create(value);
		}

		public static SI SI(this double value, UnitInstance si)
		{
			return new SI(si, value);
		}

		/// <summary>
		/// Creates an templated SI object for the number.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T SI<T>(this double value) where T : SIBase<T>
		{
			return SIBase<T>.Create(value);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IEnumerable<T> SI<T>(this IEnumerable<double> self) where T : SIBase<T>
		{
			return self.Select(x => x.SI<T>());
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToGUIFormat(this double self, int decimals = 12)
		{
			return Math.Round(self, decimals).ToString(CultureInfo.InvariantCulture);
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToXMLFormat(this double self, uint? decimals = null)
		{
			decimals = decimals ?? 2;
			return self.ToString("F" + decimals.Value, CultureInfo.InvariantCulture);
		}

		public static string ToXMLFormat(this ConvertedSI self, uint? decimals = null)
		{
			decimals = decimals ?? 2;
			return ((double)self).ToString("F" + decimals.Value, CultureInfo.InvariantCulture);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToMinSignificantDigits(this ConvertedSI self, uint? significant = null, uint? decimals = null)
		{
			return ToMinSignificantDigits((double)self, significant, decimals);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToMinSignificantDigits(this double self, uint? significant = null, uint? decimals = null)
		{
			significant = significant ?? 3;
			decimals = decimals ?? 1;
			var scale = Math.Ceiling(Math.Log10(Math.Abs(self)));

			if (double.IsInfinity(scale) || double.IsNaN(scale)) {
				return self.ToString("F" + decimals.Value, CultureInfo.InvariantCulture);
			}

			return self.ToString("F" + Math.Max(significant.Value - scale, decimals.Value), CultureInfo.InvariantCulture);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNaN(this double self) => double.IsNaN(self);
	}

	public static class FloatExtensionMethods
	{
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T SI<T>(this float value) where T : SIBase<T>
		{
			return SIBase<T>.Create(value);
		}
	}

	public static class IntegerExtensionMethods
	{
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToGUIFormat(this int self)
		{
			return self.ToString();
		}
	}
}