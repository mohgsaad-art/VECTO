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
using TUGraz.VectoCommon.Models;
using System.Text.RegularExpressions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public enum WeightingGroup
	{
		Group1s = 1,
		Group1,
		Group2,
		Group3,
        Group4UD,
		Group4RD,
		Group4LH,
		Group5RD,
		Group5LH,
		Group9RD,
		Group9LH,
		Group10RD,
		Group10LH,

		Group11,
		Group12,
		Group16,

		Group51,
		Group52,
		Group53,
		Group54,
		Group55,
		Group56,

        Unknown
    }

	public static class WeightingGroupHelper
	{
		public const string Prefix = "Group";
		public static WeightingGroup Parse(string groupStr)
		{
			return (Prefix + groupStr.Replace("-", "")).ParseEnum<WeightingGroup>();
		}

		public static string ToXMLFormat(this WeightingGroup group)
		{
			switch (group) {
				case WeightingGroup.Group1:
				case WeightingGroup.Group2:
				case WeightingGroup.Group3:
				case WeightingGroup.Group11:
				case WeightingGroup.Group12:
				case WeightingGroup.Group16:
				case WeightingGroup.Unknown:
					return Constants.NOT_AVAILABLE;
				case WeightingGroup.Group4UD:
				case WeightingGroup.Group4RD:
				case WeightingGroup.Group4LH:
				case WeightingGroup.Group5RD:
				case WeightingGroup.Group5LH:
				case WeightingGroup.Group9RD:
				case WeightingGroup.Group9LH:
				case WeightingGroup.Group10RD:
				case WeightingGroup.Group10LH:
					return Regex.Split(group.ToString().Replace(Prefix, ""), @"(\d+|\w+)").Where(x => !string.IsNullOrWhiteSpace(x)).Join("-");
				default:
					return Constants.NOT_AVAILABLE;
			}
		}
	}

	public class WeightingGroups : LookupData<VehicleClass, bool, Watt, WeightingGroup>
	{
		protected readonly List<Entry> Entries = new List<Entry>();

		#region Overrides of LookupData

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".CO2Standards.WeightingGroups.csv";

		protected override string ErrorMessage => "WeightingGroup Lookup Error: no entry found for group {0}, sleeper cab: {1}, engine rated power {2}";

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				Entries.Add(new Entry() {
					VehicleGroup = VehicleClassHelper.Parse(row.Field<string>("vehiclegroup")),
					SleeperCab = "SleeperCab".Equals(row.Field<string>("cabintype"), StringComparison.InvariantCultureIgnoreCase),
					RatedPowerMin = row.ParseDouble("engineratedpowermin").SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
					RatedPowerMax = row.ParseDouble("engineratedpowermax").SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
					WeightingGroup = WeightingGroupHelper.Parse(row.Field<string>("weightinggroup"))
				});
			}
		}


		public override WeightingGroup Lookup(VehicleClass group, bool sleeperCab, Watt engineRatedPower)
		{
			var rows = Entries.FindAll(
				x => x.VehicleGroup == group && x.SleeperCab == sleeperCab && engineRatedPower >= x.RatedPowerMin &&
					engineRatedPower < x.RatedPowerMax);
			return rows.Count == 0 ? WeightingGroup.Unknown : rows.First().WeightingGroup;
		}
		#endregion

		protected class Entry
		{
			public VehicleClass VehicleGroup;
			public bool SleeperCab;
			public Watt RatedPowerMin;
			public Watt RatedPowerMax;
			public WeightingGroup WeightingGroup;
		}
	}
}
