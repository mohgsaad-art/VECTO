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
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCommon.Utils
{
	/// <summary>
	/// Provides helper methods for mathematical functions.
	/// </summary>
	public static class VectoMath
	{
		/// <summary>
		/// Linearly interpolates a value between two points.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TResult">The type of the result.</typeparam>
		/// <param name="x1">First Value on the X-Axis.</param>
		/// <param name="x2">Second Value on the X-Axis.</param>
		/// <param name="y1">First Value on the Y-Axis.</param>
		/// <param name="y2">Second Value on the Y-Axis.</param>
		/// <param name="xint">Value on the X-Axis, for which the Y-Value should be interpolated.</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult Interpolate<T, TResult>(T x1, T x2, TResult y1, TResult y2, T xint) where T : SI
			where TResult : SIBase<TResult>
		{
			return Interpolate(x1.Value(), x2.Value(), y1.Value(), y2.Value(), xint.Value()).SI<TResult>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult Interpolate<TInput, T, TResult>(this Tuple<TInput, TInput> self, Func<TInput, T> x,
			Func<TInput, TResult> y, T xInterpolate) where T : SIBase<T> where TResult : SIBase<TResult>
		{
			return Interpolate(x(self.Item1).Value(), x(self.Item2).Value(), y(self.Item1).Value(), y(self.Item2).Value(),
				xInterpolate.Value()).SI<TResult>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult Interpolate<TInput, TResult>(this (TInput, TInput) self, Func<TInput, double> x,
			Func<TInput, TResult> y, double xInterpolate) where TResult : SIBase<TResult>
		{
			return Interpolate(x(self.Item1), x(self.Item2), y(self.Item1).Value(), y(self.Item2).Value(), xInterpolate).SI<TResult>();
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult Interpolate<TInput, TResult>(this Tuple<TInput, TInput> self, Func<TInput, double> x,
			Func<TInput, TResult> y, double xInterpolate) where TResult : SIBase<TResult>
		{
			return
				Interpolate(x(self.Item1), x(self.Item2), y(self.Item1).Value(), y(self.Item2).Value(), xInterpolate).SI<TResult>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult Interpolate<TInput, TResult>(this IEnumerable<TInput> self, Func<TInput, double> x,
			Func<TInput, TResult> y, double xInterpolate)
			where TResult : SIBase<TResult>
		{
			return self.GetSection(elem => x(elem) < xInterpolate).Interpolate(x, y, xInterpolate);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Interpolate<TInput>(this IEnumerable<TInput> self, Func<TInput, double> x,
			Func<TInput, double> y, double xInterpolate)
		{
			return self.GetSection(elem => x(elem) < xInterpolate).Interpolate(x, y, xInterpolate);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Interpolate<TInput>(this (TInput, TInput) self, Func<TInput, double> x,
			Func<TInput, double> y, double xInterpolate)
		{
			return Interpolate(x(self.Item1), x(self.Item2), y(self.Item1), y(self.Item2), xInterpolate);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Interpolate<TInput>(this Tuple<TInput, TInput> self, Func<TInput, double> x,
			Func<TInput, double> y, double xInterpolate)
		{
			return Interpolate(x(self.Item1), x(self.Item2), y(self.Item1), y(self.Item2), xInterpolate);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Interpolate<T>(T x1, T x2, double y1, double y2, T xint) where T : SI
		{
			return Interpolate(x1.Value(), x2.Value(), y1, y2, xint.Value());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TResult Interpolate<TResult>(double x1, double x2, TResult y1, TResult y2, double xint)
			where TResult : SIBase<TResult>
		{
			return Interpolate(x1, x2, y1.Value(), y2.Value(), xint).SI<TResult>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Interpolate(Point p1, Point p2, double x)
		{
			return Interpolate(p1.X, p2.X, p1.Y, p2.Y, x);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Interpolate(Edge edge, double x)
		{
			return Interpolate(edge.P1, edge.P2, x);
		}

		/// <summary>
		/// Linearly interpolates a value between two points.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static double Interpolate(double x1, double x2, double y1, double y2, double xint)
		{
			if ((x1 - x2).IsEqual(0, 1e-9) && (x1 - xint).IsEqual(0, 1e-9)) {
				return y1;
			}
			return (xint - x1) * (y2 - y1) / (x2 - x1) + y1;
		}

		/// <summary>
		/// Returns the absolute value.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SI Abs(SI si)
		{
			return si.Abs();
		}

		/// <summary>
		/// Returns the minimum of two values.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Min<T>(T c1, T c2) where T : IComparable
		{
			if (c1 == null) {
				return c2;
			}

			if (c2 == null) {
				return c1;
			}

			return c1.CompareTo(c2) <= 0 ? c1 : c2;
		}

		/// <summary>
		/// Returns the maximum of two values.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Max<T>(T c1, T c2) where T : IComparable
		{
			if (c1 == null) {
				return c2;
			}

			if (c2 == null) {
				return c1;
			}
			return c1.CompareTo(c2) > 0 ? c1 : c2;
		}

		/// <summary>
		/// Returns the maximum of two values.
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Max<T>(double c1, T c2) where T : SIBase<T>
		{
			return c1 > c2.Value() ? c1.SI<T>() : c2;
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Max<T>(T c1, T c2, T c3) where T : SIBase<T>
		{
			return Max(Max(c1, c2), c3);
		}

		//[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T LimitTo<T>(this T value, T lowerBound, T upperBound) where T : IComparable
		{
			if (lowerBound.CompareTo(upperBound) > 0) {
				throw new VectoException(
					"VectoMath.LimitTo: lowerBound must not be greater than upperBound. lowerBound: {0}, upperBound: {1}", lowerBound,
					upperBound);
			}

			if (value.CompareTo(upperBound) > 0) {
				return upperBound;
			}

			if (value.CompareTo(lowerBound) < 0) {
				return lowerBound;
			}

			return value;
		}

		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsBetween<T>(this T value, T lowerBound, T upperBound) where T : IComparable
		{
			if (lowerBound.CompareTo(upperBound) > 0) {
				throw new VectoException(
					"VectoMath.LimitTo: lowerBound must not be greater than upperBound. lowerBound: {0}, upperBound: {1}", lowerBound,
					upperBound);
			}

			if (value.CompareTo(upperBound) > 0 || value.CompareTo(lowerBound) < 0) {
				return false;
			}

			return true;
		}

		/// <summary>
		///	converts the given inclination in percent (0-1+) into Radians
		/// </summary>
		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Radian InclinationToAngle(double inclinationPercent)
		{
			return Math.Atan(inclinationPercent).SI<Radian>();
		}


		public static Point Intersect(Edge line1, Edge line2)
		{
			var s10X = line1.P2.X - line1.P1.X;
			var s10Y = line1.P2.Y - line1.P1.Y;
			var s32X = line2.P2.X - line2.P1.X;
			var s32Y = line2.P2.Y - line2.P1.Y;

			var denom = s10X * s32Y - s32X * s10Y;
			if (denom.IsEqual(0)) {
				return null;
			}

			var s02X = line1.P1.X - line2.P1.X;
			var s02Y = line1.P1.Y - line2.P1.Y;
			var sNumer = s10X * s02Y - s10Y * s02X;
			if ((sNumer < 0) == (denom > 0)) {
				return null;
			}
			var tNumer = s32X * s02Y - s32Y * s02X;
			if ((tNumer < 0) == (denom > 0)) {
				return null;
			}
			if (((sNumer > denom) == (denom > 0)) || ((tNumer > denom) == (denom > 0))) {
				return null;
			}
			var t = tNumer / denom;

			return new Point(line1.P1.X + t * s10X, line1.P1.Y + t * s10Y);
		}

		/// <summary>
		/// Computes the time interval for driving the given distance ds with the vehicle's current speed and the given acceleration.
		/// If the distance ds can not be reached (i.e., the vehicle would halt before ds is reached) then the distance parameter is adjusted.
		/// Returns a new operating point (a, ds, dt)
		/// </summary>
		/// <param name="currentSpeed">vehicle's current speed at the beginning of the simulation interval</param>
		/// <param name="acceleration">vehicle's acceleration</param>
		/// <param name="distance">absolute distance at the beginning of the simulation interval (can be 0)</param>
		/// <param name="ds">distance to drive in the current simulation interval</param>
		/// <returns>Operating point (a, ds, dt)</returns>
		public static OperatingPoint ComputeTimeInterval(MeterPerSecond currentSpeed, MeterPerSquareSecond acceleration,
			Meter distance, Meter ds)
		{
			if (!(ds > 0)) {
				throw new VectoSimulationException("ds has to be greater than 0! ds: {0}", ds);
			}

			var retVal = new OperatingPoint() { Acceleration = acceleration, SimulationDistance = ds };
			if (acceleration.IsEqual(0)) {
				if (currentSpeed > 0) {
					retVal.SimulationInterval = ds / currentSpeed;
					return retVal;
				}
				//Log.Error("{2}: vehicle speed is {0}, acceleration is {1}", currentSpeed.Value(), acceleration.Value(),
				//	distance);
				throw new VectoSimulationException(
					"vehicle speed has to be > 0 if acceleration = 0!  v: {0}, a: {1}, distance: {2}", currentSpeed.Value(),
					acceleration.Value(), distance);
			}

			// we need to accelerate / decelerate. solve quadratic equation...
			// ds = acceleration / 2 * dt^2 + currentSpeed * dt   => solve for dt
			var solutions = QuadraticEquationSolver(acceleration.Value() / 2.0, currentSpeed.Value(),
				-ds.Value());

			if (solutions.Length == 0) {
				// no real-valued solutions: acceleration is so negative that vehicle stops already before the required distance can be reached.
				// adapt ds to the halting-point.
				// t = v / a
				var dt = currentSpeed / -acceleration;

				// s = a/2*t^2 + v*t
				var stopDistance = acceleration / 2 * dt * dt + currentSpeed * dt;

				if (stopDistance.IsGreater(ds)) {
					// just to cover everything - does not happen...
					//Log.Error(
					//	"Could not find solution for computing required time interval to drive distance ds: {0}. currentSpeed: {1}, acceleration: {2}, stopDistance: {3}, distance: {4}",
					//	ds, currentSpeed, acceleration, stopDistance,distance);
					throw new VectoSimulationException("Could not find solution for time-interval!  ds: {0}, stopDistance: {1}", ds,
						stopDistance);
				}

				//LoggingObject.Logger<>().Info(
				//	"Adjusted distance when computing time interval: currentSpeed: {0}, acceleration: {1}, distance: {2} -> {3}, timeInterval: {4}",
				//	currentSpeed, acceleration, stopDistance, stopDistance, dt);

				retVal.SimulationInterval = dt;
				retVal.SimulationDistance = stopDistance;
				return retVal;
			}
			// if there are 2 positive solutions (i.e. when decelerating), take the smaller time interval
			// (the second solution means that you reach negative speed)
			retVal.SimulationInterval = solutions.Where(x => x >= 0).Min().SI<Second>();
			return retVal;
		}



		[DebuggerStepThrough]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Ceiling<T>(T si) where T : SIBase<T>
		{
			return Math.Ceiling(si.Value()).SI<T>();
		}


		private static double Cbrt(double x)
		{
			return x < 0 ? -Math.Pow(-x, 1.0 / 3.0) : Math.Pow(x, 1.0 / 3.0);
		}


		public static (double k, double d) LeastSquaresFitting<T>(IEnumerable<T> entries, Func<T, double> getX, Func<T, double> getY) =>
			LeastSquaresFitting(entries?.Select(x => new Point(getX(x), getY(x))));

		public static (double k, double d) LeastSquaresFitting(IEnumerable<Point> entries)
		{
			if (entries == null) {
				return (0, 0);
			}

			// algorithm taken from http://mathworld.wolfram.com/LeastSquaresFitting.html (eqn. 27 & 28)
			var count = 0;
			double sumX = 0, sumY = 0, sumXSquare = 0, sumYSquare = 0, sumXY = 0;
			foreach (var e in entries) {
				sumX += e.X;
				sumY += e.Y;
				sumXSquare += e.X * e.X;
				sumYSquare += e.Y * e.Y;
				sumXY += e.X * e.Y;
				count++;
			}
			switch (count) {
				case 0: return (0, 0);
				case 1: return (0, sumY);
				default:
					var ssxx = sumXSquare - sumX * sumX / count;
					var ssxy = sumXY - sumX * sumY / count;
					var k = ssxy / ssxx;
					var d = (sumY - k * sumX) / count;
					return (k, d);
			}
		}
		
		public static double[] QuadraticEquationSolver(double a, double b, double c)
		{
			return Polynom2Solver(a, b, c);
		}

		public static double[] CubicEquationSolver(double a, double b, double c, double d)
		{
			return Polynom3Solver(a, b, c, d);
		}

		public static double[] Polynom2Solver(double a, double b, double c)
		{
			var d = b * b - 4 * a * c;

			// no real solution
			if (d < 0) {
				return new double[0];
			}

			if (d > 0) {
				// two solutions
				return new[] { (-b + Math.Sqrt(d)) / (2 * a), (-b - Math.Sqrt(d)) / (2 * a) };
			}

			// one real solution
			return new[] { -b / (2 * a) };
		}


		public static double[] Polynom3Solver(double a, double b, double c, double d)
		{
			var solutions = new List<double>();
			if (a.IsEqual(0, 1e-12)) {
				return QuadraticEquationSolver(b, c, d);
			}
			var w = b / (3 * a);
			var p = Math.Pow(c / (3 * a) - w * w, 3);
			var q = -0.5 * (2 * (w * w * w) - (c * w - d) / a);
			var discriminant = q * q + p;
			if (discriminant < 0.0) {
				// 3 real solutions
				var h = q / Math.Sqrt(-p);
				var phi = Math.Acos(Math.Max(-1.0, Math.Min(1.0, h)));
				p = 2 * Math.Pow(-p, 1.0 / 6.0);
				for (var i = 0; i < 3; i++) {
					solutions.Add(p * Math.Cos((phi + 2 * i * Math.PI) / 3.0) - w);
				}
			} else {
				// one real solution
				discriminant = Math.Sqrt(discriminant);
				solutions.Add(Cbrt(q + discriminant) + Cbrt(q - discriminant) - w);
			}

			// 1 Newton iteration step in order to minimize round-off errors
			for (var i = 0; i < solutions.Count; i++) {
				var h = c + solutions[i] * (2 * b + 3 * solutions[i] * a);
				if (!h.IsEqual(0, 1e-12)) {
					solutions[i] -= (d + solutions[i] * (c + solutions[i] * (b + solutions[i] * a))) / h;
				}
			}
			solutions.Sort();
			return solutions.ToArray();
		}

		public static double[] Polynom4Solver(double A, double B, double C, double D, double E)
		{
			// see http://www.mathe.tu-freiberg.de/~hebisch/cafe/viertergrad.pdf

			var a = B / A;
			var b = C / A;
			var c = D / A;
			var d = E / A;
			var p = -3.0 / 8.0 * a * a + b;
			var q = 1.0 / 8.0 * a * a * a - a * b / 2.0 + c;
			var r = -3.0 / 256.0 * a * a * a * a + a * a * b / 16.0 - a * c / 4.0 + d;
			if (q.IsEqual(0, 1e-12)) {
				var solY = QuadraticEquationSolver(1, b, d);
				var retVal = new List<double>();
				foreach (var s in solY) {
					if (s < 0) {
						continue;
					}

					retVal.Add(Math.Sqrt(s));
					retVal.Add(-Math.Sqrt(s));
				}

				return retVal.ToArray();
			}

			var solZ = Polynom3Solver(8.0, 20.0 * p, 16.0 * p * p - 8.0 * r, 4.0 * p * p * p - 4.0 * p * r - q * q);
			if (solZ.Length == 0) {
				return new double[0];

				//throw new VectoException("no solution for polynom grade 4 found");
			}

			var z = solZ.First();
			var u = p + 2.0 * z;
			if (u < 0) {
				// no real-valued solution
				return new double[0];
			}
			var solY1 = QuadraticEquationSolver(1, -Math.Sqrt(u), q / (2.0 * Math.Sqrt(u)) + p + z);
			var solY2 = QuadraticEquationSolver(1, Math.Sqrt(u), -q / (2.0 * Math.Sqrt(u)) + p + z);
			return solY1.Select(s => s - a / 4.0).Concat(solY2.Select(s => s - a / 4.0)).ToArray();
		}
	}

	[DebuggerDisplay("(X:{X}, Y:{Y}, Z:{Z})")]
	public class Point : IEquatable<Point>
	{
		public readonly double X;
		public readonly double Y;
		public readonly double Z;

		public Point(double x, double y, double z = 0) => (X, Y, Z) = (x, y, z);

		public void Deconstruct(out double x, out double y) => (x, y) = (X, Y);

		public static Point operator +(Point p1, Point p2) => new Point(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);

		public static Point operator -(Point p1, Point p2) => new Point(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);

		public static Point operator -(Point p1) => new Point(-p1.X, -p1.Y);

		public static Point operator *(Point p1, double scalar) => new Point(p1.X * scalar, p1.Y * scalar, p1.Z * scalar);

		public static Point operator *(double scalar, Point p1) => p1 * scalar;

		/// <summary>
		/// Returns perpendicular vector for xy-components of this point. P = (-Y, X)
		/// </summary>
		public Point Perpendicular() => new Point(-Y, X);

		/// <summary>
		/// Returns dot product between two 3d-vectors.
		/// </summary>
		public double Dot(Point other) => X * other.X + Y * other.Y + Z * other.Z;

		/// <summary>
		/// Test if point is on the left side of an edge.
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public bool IsLeftOf(Edge e)
		{
			var abX = e.P2.X - e.P1.X;
			var abY = e.P2.Y - e.P1.Y;
			var acX = X - e.P1.X;
			var acY = Y - e.P1.Y;
			var z = abX * acY - abY * acX;
			return z.IsGreater(0);
		}

		#region Equality members
		public bool Equals(Point other) => X.IsEqual(other.X) && Y.IsEqual(other.Y) && Z.IsEqual(other.Z);

		public override bool Equals(object obj) => !ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Point)obj));

		public override int GetHashCode() => unchecked((((X.GetHashCode() * 397) ^ Y.GetHashCode()) * 397) ^ Z.GetHashCode());
		#endregion
	}

	[DebuggerDisplay("Plane({X}, {Y}, {Z}, {W})")]
	public class Plane
	{
		public readonly double X;
		public readonly double Y;
		public readonly double Z;
		public readonly double W;

		public Plane(Triangle tr)
		{
			var abX = tr.P2.X - tr.P1.X;
			var abY = tr.P2.Y - tr.P1.Y;
			var abZ = tr.P2.Z - tr.P1.Z;

			var acX = tr.P3.X - tr.P1.X;
			var acY = tr.P3.Y - tr.P1.Y;
			var acZ = tr.P3.Z - tr.P1.Z;

			X = abY * acZ - abZ * acY;
			Y = abZ * acX - abX * acZ;
			Z = abX * acY - abY * acX;
			W = tr.P1.X * X + tr.P1.Y * Y + tr.P1.Z * Z;
		}
	}

	[DebuggerDisplay("Triangle(({P1.X}, {P1.Y}, {P1.Z}), ({P2.X}, {P2.Y}, {P2.Z}), ({P3.X}, {P3.Y}, {P3.Z}))")]
	public class Triangle
	{
		public readonly Point P1;
		public readonly Point P2;
		public readonly Point P3;

		public Triangle(Point p1, Point p2, Point p3)
		{
			if (p1.X.IsEqual(p2.X) && p2.X.IsEqual(p3.X) || p1.Y.IsEqual(p2.Y) && p2.Y.IsEqual(p3.Y)) {
				throw new VectoException("triangle is not extrapolatable by a plane.");
			}
			(P1, P2, P3) = (p1, p2, p3);
		}

		/// <summary>
		/// Check if Point is inside of Triangle. Barycentric Technique: http://www.blackpawn.com/texts/pointinpoly/default.html
		/// </summary>
		public bool IsInside(double x, double y, bool exact)
		{
			var smallerY = y - DoubleExtensionMethods.Tolerance;
			var biggerY = y + DoubleExtensionMethods.Tolerance;
			var smallerX = x - DoubleExtensionMethods.Tolerance;
			var biggerX = x + DoubleExtensionMethods.Tolerance;

			var aboveTriangle = P1.Y < smallerY && P2.Y < smallerY && P3.Y < smallerY;
			var belowTriangle = P1.Y > biggerY && P2.Y > biggerY && P3.Y > biggerY;
			var leftOfTriangle = P1.X > biggerX && P2.X > biggerX && P3.X > biggerX;
			var rightOfTriangle = P1.X < smallerX && P2.X < smallerX && P3.X < smallerX;

			if (aboveTriangle || rightOfTriangle || leftOfTriangle || belowTriangle) {
				return false;
			}

			var v0X = P3.X - P1.X;
			var v0Y = P3.Y - P1.Y;
			var v1X = P2.X - P1.X;
			var v1Y = P2.Y - P1.Y;
			var v2X = x - P1.X;
			var v2Y = y - P1.Y;

			var dot00 = v0X * v0X + v0Y * v0Y;
			var dot01 = v0X * v1X + v0Y * v1Y;
			var dot02 = v0X * v2X + v0Y * v2Y;
			var dot11 = v1X * v1X + v1Y * v1Y;
			var dot12 = v1X * v2X + v1Y * v2Y;

			var invDenom = 1.0 / (dot00 * dot11 - dot01 * dot01);
			var u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			var v = (dot00 * dot12 - dot01 * dot02) * invDenom;

			if (exact) {
				return u >= 0 && v >= 0 && u + v <= 1;
			}

			return u.IsPositive() && v.IsPositive() && (u + v).IsSmallerOrEqual(1);
		}

		public bool ContainsInCircumcircle(Point p)
		{
			var ax = P1.X - p.X;
			var ay = P1.Y - p.Y;
			var bx = P2.X - p.X;
			var by = P2.Y - p.Y;
			var cx = P3.X - p.X;
			var cy = P3.Y - p.Y;
			var result = (ax * ax + ay * ay) * (bx * cy - cx * by)
						- (bx * bx + by * by) * (ax * cy - cx * ay)
						+ (cx * cx + cy * cy) * (ax * by - bx * ay);
			return result > 0;
		}

		public bool Contains(Point p) => p.Equals(P1) || p.Equals(P2) || p.Equals(P3);

		public bool SharesVertexWith(Triangle t) => Contains(t.P1) || Contains(t.P2) || Contains(t.P3);

		public Edge[] GetEdges() => new[] { new Edge(P1, P2), new Edge(P2, P3), new Edge(P3, P1) };

		#region Equality members
		protected bool Equals(Triangle other) => Equals(P1, other.P1) && Equals(P2, other.P2) && Equals(P3, other.P3);

		public override bool Equals(object obj) =>
			!ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Triangle)obj));

		public override int GetHashCode() => (P1, P2, P3).GetHashCode();
		#endregion
	}

	[DebuggerDisplay("Edge(({P1.X}, {P1.Y},{P1.Z}), ({P2.X}, {P2.Y},{P2.Z}))")]
	public class Edge
	{
		public readonly Point P1;
		public readonly Point P2;

		private Point _vector;

		public Edge(Point p1, Point p2) => (P1, P2) = (p1, p2);

		public Point Vector => _vector ?? (_vector = P2 - P1);

		public double SlopeXY => Vector.Y / Vector.X;

		public double OffsetXY => P2.Y - SlopeXY * P2.X;

		#region Equality members

		protected bool Equals(Edge other) =>
			P1.Equals(other.P1) && Equals(P2, other.P2) || P1.Equals(other.P2) && P2.Equals(other.P1);

		public override bool Equals(object obj) =>
			!ReferenceEquals(null, obj) && (ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((Edge)obj));

		public override int GetHashCode() => P1.GetHashCode() ^ P2.GetHashCode();

		#endregion

		public static Edge Create(Point arg1, Point arg2) => new Edge(arg1, arg2);

		public bool ContainsXY(Point point) =>
			(SlopeXY * point.X + (P1.Y - SlopeXY * P1.X) - point.Y).IsEqual(0, 1E-9);
	}
}