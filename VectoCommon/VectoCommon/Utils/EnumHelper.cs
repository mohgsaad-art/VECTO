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
using System.Text.RegularExpressions;

namespace TUGraz.VectoCommon.Utils
{
	public static class EnumHelper
	{
		public static T ParseEnum<T>(this string s, bool ignoreCase = true) =>
			(T)Enum.Parse(typeof(T),
				s.StartsWith("-")
					? Regex.Replace(s, "[^a-zA-Z0-9_-]", "")
					: Regex.Replace(s, "[^a-zA-Z0-9_]", ""), ignoreCase);

		public static T ParseEnum<T>(this object o, bool ignoreCase = true) =>
			o is string s ? ParseEnum<T>(s, ignoreCase) : (T)o;


		public static T[] GetValues<T>() =>
			Enum.GetValues(typeof(T)).Cast<T>().ToArray();

	    public static KeyValuePair<T, string>[] GetKeyValuePairs<T>(Func<T, string> displaySelector = null, Func<T, bool> predicate = null) {
			if (displaySelector is null)
				displaySelector = x => x.ToString();
			if (predicate is null)
				predicate = x => true;
			return GetValues<T>().Where(predicate).Select(t => new KeyValuePair<T, string>(t, displaySelector(t))).ToArray();
		}
	}
}