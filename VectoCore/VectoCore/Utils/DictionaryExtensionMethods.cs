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

using Ninject.Planning.Bindings.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Utils
{
	internal static class DictionaryExtensionMethods
	{
		public static object GetValueOrDBNull<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) =>
			dictionary.TryGetValue(key, out var value) ? (object)value : DBNull.Value;

		public static TValue GetValueOrZero<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
			where TValue : SIBase<TValue> => dictionary.TryGetValue(key, out var value) ? value : 0.SI<TValue>();

		public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TKey, TValue> defaultConstructor)
		{
			if (self.TryGetValue(key, out var result))
				return result;

			lock (self) {
				if (!self.TryGetValue(key, out result)) {
					result = defaultConstructor(key);
					self.Add(key, result);
				}
				return result;
			}
		}

		public static TValue GetVECTOValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key) => 
			self.TryGetValue(key, out var value) ? value : default;


		public static TValue GetVECTOValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, TValue defaultValue) =>
			self.TryGetValue(key, out var value) ? value : defaultValue;
	}
}