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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TUGraz.VectoCore.Utils
{
	/// <summary>
	/// Accepts following date formats:
	/// * German: d.M.yyyy HH:mm:ss
	/// * English: M/d/yyyy HH:mm:ss tt
	/// * ISO8601: yyyy-MM-DDTHH:mm:ssZ
	/// * Local culture format (based on current localization of the user)
	/// Output is in ISO8601 format.
	/// </summary>
	public class DateTimeFallbackDeserializer : IsoDateTimeConverter
	{
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Date) {
				return reader.Value;
			}
			if (DateTime.TryParseExact((string)reader.Value, new[] { "d.M.yyyy HH:mm:ss", "M/d/yyyy HH:mm:ss tt" },
				CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime)) {
				return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			}

			if (DateTime.TryParse((string)reader.Value, out dateTime)) {
				return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
			}
			return base.ReadJson(reader, objectType, existingValue, serializer);
		}
	}
}