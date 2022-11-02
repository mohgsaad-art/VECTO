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
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCommon.Utils
{
	public static class EnumerableExtensionMethods
	{
		/// <summary>
		/// Joins the items of the enumerable into a string.
		/// </summary>
		public static string Join<T>(this IEnumerable<T> list, string separator = ", ") =>
			string.Join(separator, list ?? Enumerable.Empty<T>());

		public static IEnumerable<(T value, int index)> Select<T>(this IEnumerable<T> self) =>
			self.Select((value, index) => (value, index));

		public static T[] Slice<T>(this T[] source, int start, int end)
		{
			if (start < 0) start = source.Length + start;
			if (end < 0) end = source.Length + end;
			var dest = new T[end - start];
			Array.ConstrainedCopy(source, start, dest, 0, end - start);
			return dest;
		}

		public static IEnumerable<double> ToDouble(this IEnumerable<string> self, double? defaultValue = null)
		{
			return self.Select(s => s.ToDouble(defaultValue));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SequenceEqualFast<T>(this T[] self, T[] other) where T : IComparable
		{
			if (self.Equals(other)) {
				return true;
			}

			if (self.Length != other.Length) {
				return false;
			}

			// ReSharper disable once LoopCanBeConvertedToQuery
			for (var i = 0; i < self.Length; i++) {
				if (self[i].CompareTo(other[i]) != 0) {
					return self.OrderBy(x => x).SequenceEqual(other.OrderBy(x => x));
				}
			}

			return true;
		}

		public static IList<double> ToDouble(this IEnumerable<SI> self)
		{
			return self.Select(x => x.Value()).ToList();
		}

		/// <summary>
		/// Wraps this object instance into an IEnumerable.
		/// </summary>
		public static IEnumerable<T> ToEnumerable<T>(this T item)
		{
			yield return item;
		}

		/// <summary>
		/// Zips all elements of two enumerable together. If the enumerables dont have the same length an exception is thrown.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Enumeration already finished. Thrown if the enumerables dont have the same length.</exception>
		public static IEnumerable<(TFirst,TSecond)> ZipAll<TFirst, TSecond>(this IEnumerable<TFirst> firstEnumerable,
			IEnumerable<TSecond> secondEnumerable) =>
			firstEnumerable.ZipAll(secondEnumerable, ValueTuple.Create);


		/// <summary>
		/// Zips all elements of two enumerable together. If the enumerables dont have the same length an exception is thrown.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Enumeration already finished. Thrown if the enumerables dont have the same length.</exception>
		public static IEnumerable<TResult> ZipAll<TFirst, TSecond, TResult>(this IEnumerable<TFirst> firstEnumerable,
			IEnumerable<TSecond> secondEnumerable, Func<TFirst, TSecond, TResult> resultSelector)
		{
			using (var first = firstEnumerable.GetEnumerator()) {
				using (var second = secondEnumerable.GetEnumerator()) {
					while (first.MoveNext() | second.MoveNext()) {
						yield return resultSelector(first.Current, second.Current);
					}
				}
			}
		}

		/// <summary>
		/// Zips all elements of two enumerable together. If the enumerables dont have the same length an exception is thrown.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Enumeration already finished. Thrown if the enumerables dont have the same length.</exception>
		public static IEnumerable<(T1 Item1, T2 Item2, T3 Item3)> Zip<T1, T2, T3>(this IEnumerable<T1> item1, IEnumerable<T2> item2, IEnumerable<T3> item3)
		{
			using (var first = item1.GetEnumerator()) {
				using (var second = item2.GetEnumerator()) {
					using (var third = item3.GetEnumerator()) {
						while (first.MoveNext() | second.MoveNext() | third.MoveNext()) {
							yield return (first.Current, second.Current, third.Current);
						}
					}
				}
			}
		}

		/// <summary>
		/// Zips all elements of two enumerable together. If the enumerables dont have the same length an exception is thrown.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">Enumeration already finished. Thrown if the enumerables dont have the same length.</exception>
		public static IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4)> Zip<T1, T2, T3, T4>(this IEnumerable<T1> item1, IEnumerable<T2> item2, IEnumerable<T3> item3, IEnumerable<T4> item4)
		{
			using (var first = item1.GetEnumerator()) {
				using (var second = item2.GetEnumerator()) {
					using (var third = item3.GetEnumerator()) {
						using (var fourth = item4.GetEnumerator()) {
							while (first.MoveNext() | second.MoveNext() | third.MoveNext() | fourth.MoveNext()) {
								yield return (first.Current, second.Current, third.Current, fourth.Current);
							}
						}
					}
				}
			}
		}

		public static T Sum<T>(this IEnumerable<T> values) where T : SIBase<T> =>
			values.Sum(x => x);

		public static TResult Sum<TU, TResult>(this IEnumerable<TU> values, Func<TU, TResult> selector)
			where TResult : SIBase<TResult> =>
			values.Select(selector).DefaultIfEmpty().Aggregate((sum, current) => sum + current);

		public static T Average<T>(this IEnumerable<T> values) where T : SIBase<T> =>
			values.Average(v => v.Value()).SI<T>();

		/// <summary>
		/// Get the first two items where the predicate changes from true to false.
		/// If the predicate is always true, the last 2 elements are returned.
		/// If the predicate is always false, the first 2 elements are returned.
		/// </summary>
		/// <example>values.GetSection(x => x &lt; X); // returns the pair (x_1, x2) where (x_1 &lt; X, x_2 &gt;= X)</example>
		public static (T, T) GetSection<T>(this IEnumerable<T> self, Func<T, bool> skip, out int index, string message = null)
		{
			using (var enumerator = self.GetEnumerator()) {
				index = 1;
				enumerator.MoveNext();
				var first = enumerator.Current;
				enumerator.MoveNext();
				var second = enumerator.Current;
				while (skip(enumerator.Current) && enumerator.MoveNext()) {
					index++;
					(first, second) = (second, enumerator.Current);
				}

				if (!string.IsNullOrWhiteSpace(message) && (!skip(first) || skip(second))) {
					LogManager.GetLogger(typeof(T).ToString()).Warn(message);
				}

				return (first, second);
			}
		}

		/// <summary>
		/// Get the first two adjacent items where the predicate changes from true to false.
		/// If the predicate never gets true, the last 2 elements are returned.
		/// </summary>
		/// <example>values.GetSection(x => x &lt; X); // returns the pair (x_1, x2) where (x_1 &lt; X, x_2 &gt;= X)</example>
		public static (T, T) GetSection<T>(this IEnumerable<T> self, Func<T, bool> predicate, string message = null) =>
			self.GetSection(predicate, out _, message);

		public static TSource MinBy<TSource>(this IEnumerable<TSource> source,
			Func<TSource, IComparable> projectionToComparable)
		{
			using (var e = source.GetEnumerator()) {
				if (!e.MoveNext()) {
					throw new InvalidOperationException("Sequence is empty.");
				}

				var min = e.Current;
				var minProjection = projectionToComparable(e.Current);

				while (e.MoveNext()) {
					var currentProjection = projectionToComparable(e.Current);
					if (currentProjection.CompareTo(minProjection) < 0) {
						min = e.Current;
						minProjection = currentProjection;
					}
				}

				return min;
			}
		}

		public static TSource MaxBy<TSource>(this IEnumerable<TSource> source,
			Func<TSource, IComparable> projectionToComparable)
		{
			using (var e = source.GetEnumerator()) {
				if (!e.MoveNext()) {
					throw new InvalidOperationException("Sequence is empty.");
				}

				var max = e.Current;
				var maxProjection = projectionToComparable(e.Current);

				while (e.MoveNext()) {
					var currentProjection = projectionToComparable(e.Current);
					if (currentProjection.CompareTo(maxProjection) > 0) {
						max = e.Current;
						maxProjection = currentProjection;
					}
				}

				return max;
			}
		}

		public static IEnumerable<TResult> Pairwise<TSource, TResult>(this IEnumerable<TSource> source,
			Func<TSource, TSource, TResult> resultSelector)
		{
			using (var it = source.GetEnumerator()) {
				if (it.MoveNext()) {
					var previous = it.Current;
					while (it.MoveNext()) {
						yield return resultSelector(previous, previous = it.Current);
					}
				}
			}
		}

		public static IEnumerable<(TSource, TSource)> Pairwise<TSource>(this IEnumerable<TSource> source)
		{
			using (var it = source.GetEnumerator()) {
				if (it.MoveNext()) {
					var previous = it.Current;
					while (it.MoveNext()) {
						yield return (previous, previous = it.Current);
					}
				}
			}
		}

		/// <summary>
		/// Repeats the element and returns an Enumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="element"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public static IEnumerable<T> Repeat<T>(this T element, int count)
		{
			return Enumerable.Repeat(element, count);
		}

		/// <summary>
		/// Deconstruct an IEnumerable into individual variables. (Tuple Unpacking)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="values"></param>
		/// <param name="item1"></param>
		/// <param name="item2"></param>
		/// <param name="item3"></param>
		public static void Deconstruct<T>(this IEnumerable<T> values, out T item1, out T item2, out T item3)
		{
			using (var enumerator = values.GetEnumerator()) {
				enumerator.MoveNext();
				item1 = enumerator.Current;
				enumerator.MoveNext();
				item2 = enumerator.Current;
				enumerator.MoveNext();
				item3 = enumerator.Current;
			}
		}

		/// <summary>
		/// Deconstruct an IEnumerable into individual variables. (Tuple Unpacking)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="values"></param>
		/// <param name="item1"></param>
		/// <param name="item2"></param>
		/// <param name="item3"></param>
		/// <param name="item4"></param>
		/// <param name="item5"></param>
		/// <param name="item6"></param>
		/// <param name="item7"></param>
		public static void Deconstruct<T>(this IEnumerable<T> values, out T item1, out T item2,
			out T item3, out T item4, out T item5, out T item6, out T item7)
		{
			using (var enumerator = values.GetEnumerator()) {
				enumerator.MoveNext();
				item1 = enumerator.Current;
				enumerator.MoveNext();
				item2 = enumerator.Current;
				enumerator.MoveNext();
				item3 = enumerator.Current;
				enumerator.MoveNext();
				item4 = enumerator.Current;
				enumerator.MoveNext();
				item5 = enumerator.Current;
				enumerator.MoveNext();
				item6 = enumerator.Current;
				enumerator.MoveNext();
				item7 = enumerator.Current;
			}
		}

		/// <summary>
		/// Checks if a value is one of the candidate values.
		/// </summary>
		public static bool IsOneOf<T>(this T self, params T[] candidates) => candidates.Contains(self);
	}
}