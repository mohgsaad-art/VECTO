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

using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.Exceptions;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public static class JsonExtensionMethods
	{
		public static JToken GetEx(this JObject jobject, string property)
		{
			var val = jobject[property];
			if (val == null) {
				throw new InvalidFileFormatException("Key {0} not found", property);
			}
			return val;
		}

		public static JToken GetEx(this JToken jtoken, string property)
		{
			var val = jtoken[property];
			if (val == null) {
				throw new InvalidFileFormatException("Key {0} not found", property);
			}
			return val;
		}

		public static T GetEx<T>(this JObject value, string property)
		{
			return GetEx(value, property).Value<T>();
		}

		public static T GetEx<T>(this JToken value, string property)
		{
			return GetEx(value, property).Value<T>();
		}


		//public static T GetValueOrDefault<T>(this JToken jtoken, string property, T defaultVal = null) where T : class 
		//{
		//	var val = jtoken[property];
		//	return val == null ? defaultVal : val.Value<T>();
		//}

		public static T? GetValueOrDefault<T>(this JToken jtoken, string property) where T : struct 
		{
			var val = jtoken[property];
			return val?.Value<T>();
		}
	}
}