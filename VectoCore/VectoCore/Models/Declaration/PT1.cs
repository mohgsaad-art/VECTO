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
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class PT1 : LookupData<PerSecond, PT1.PT1Result>
	{
		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".PT1.csv";

		protected override string ErrorMessage => throw new InvalidOperationException("ErrorMessage not applicable.");

		private List<KeyValuePair<PerSecond, Second>> _entries;

		public PT1()
		{
			ParseData(ReadCsvResource(ResourceId));
		}

		public PT1(DataTable data)
		{
			ParseDataFromFld(data);
		}

		protected override void ParseData(DataTable table)
		{
			_entries = table.Rows.Cast<DataRow>()
				.Select(r => new KeyValuePair<PerSecond, Second>(r.ParseDouble("rpm").RPMtoRad(), r.ParseDouble("PT1").SI<Second>()))
				.OrderBy(x => x.Key)
				.ToList();
		}

		private void ParseDataFromFld(DataTable data)
		{
			if (data.Columns.Count < 3) {
				throw new VectoException("FullLoadCurve/PT1 Data File must consist of at least 4 columns.");
			}

			if (data.Rows.Count < 2) {
				throw new VectoException(
					"FullLoadCurve/PT1 must consist of at least two lines with numeric values (below file header)");
			}

			if (data.Columns.Contains(Fields.EngineSpeed) && data.Columns.Contains(Fields.PT1)) {
				_entries = data.Rows.Cast<DataRow>()
					.Select(
						r => new KeyValuePair<PerSecond, Second>(r.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
							r.ParseDouble(Fields.PT1).SI<Second>()))
					.OrderBy(x => x.Key).ToList();
			} else {
				_entries = data.Rows.Cast<DataRow>()
					.Select(r => new KeyValuePair<PerSecond, Second>(r.ParseDouble(0).RPMtoRad(), r.ParseDouble(3).SI<Second>()))
					.OrderBy(x => x.Key).ToList();
			}
		}

		public override PT1Result Lookup(PerSecond key)
		{
			var extrapolated = key.IsSmaller(_entries[0].Key) || key.IsGreater(_entries.Last().Key);

			var index = _entries.FindIndex(x => x.Key.IsGreater(key));
			if (index <= 0) {
				index = key.IsGreater(_entries[0].Key) ? _entries.Count - 1 : 1;
			}

			var pt1 = VectoMath.Interpolate(_entries[index - 1].Key, _entries[index].Key, _entries[index - 1].Value,
				_entries[index].Value, key);
			if (pt1 < 0) {
				pt1 = 0.SI<Second>();
				extrapolated = true;
				//throw new VectoException("The calculated pt1 value must not be smaller than 0. Value: " + pt1);
			}
			return new PT1Result() { Value = pt1, Extrapolated = extrapolated };
		}

		private static class Fields
		{
			public const string PT1 = "PT1";
			public const string EngineSpeed = "engine speed";
		}

		public struct PT1Result
		{
			public Second Value;
			public bool Extrapolated;
		}
	}
}