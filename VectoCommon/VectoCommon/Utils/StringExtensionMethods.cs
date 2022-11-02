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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TUGraz.VectoCommon.Utils
{
	public static class StringExtensionMethods
	{
		public static string Slice(this string source, int start, int end) {
			if (start < 0) start = source.Length + start;
			if (end < 0) end = source.Length + end;
			return source.Substring(start, end - start);
		}

		public static double ToDouble(this string self, double? defaultValue = null)
		{
			if (string.IsNullOrWhiteSpace(self)) {
				if (defaultValue.HasValue) {
					return defaultValue.Value;
				}
				throw new FormatException("Cannot convert an empty string to a number.");
			}

			var success = double.TryParse(self, NumberStyles.Float, CultureInfo.InvariantCulture, out var retVal);
			if (success) {
				return retVal;
			}

			if (defaultValue.HasValue) {
				return defaultValue.Value;
			}

			// throws an exception
			return double.Parse(self, CultureInfo.InvariantCulture);
		}

		public static int ToInt(this string self, int? defaultValue = null)
		{
			try {
				return int.Parse(self, CultureInfo.InvariantCulture);
			} catch (FormatException) {
				if (defaultValue.HasValue) {
					return defaultValue.Value;
				}
				throw;
			}
		}

		public static bool ToBoolean(this string self)
		{
			if (string.IsNullOrEmpty(self)) {
				return false;
			}
			return int.Parse(self) != 0;
		}

		public static double IndulgentParse(this string self) =>
			double.Parse(new string(self.Trim().TakeWhile(c => char.IsDigit(c) || c == '.').ToArray()),
				CultureInfo.InvariantCulture);

		public static Stream ToStream(this string self) => 
			new MemoryStream(Encoding.UTF8.GetBytes(self));

		public static string RemoveWhitespace(this string self) => 
			string.Concat(self.Split());
	}
}