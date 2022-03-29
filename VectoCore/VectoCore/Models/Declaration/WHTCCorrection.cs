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
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class WHTCCorrection
	{
		private readonly WHTCCorrectionData _data = new WHTCCorrectionData();

		public double Lookup(MissionType mission, double rural, double urban, double motorway)
		{
			var entry = _data.Lookup(mission);
			return rural * entry.Rural + urban * entry.Urban + motorway * entry.Motorway;
		}

		private sealed class WHTCCorrectionData : LookupData<MissionType, Entry>
		{
			protected override string ResourceId
			{
				get { return DeclarationData.DeclarationDataResourcePrefix + ".WHTC-Weighting-Factors.csv"; }
			}

			protected override string ErrorMessage
			{
				get { return "WHTC Correction Lookup Error: no value found. Mission: '{0}'"; }
			}

			protected override void ParseData(DataTable table)
			{
				foreach (MissionType mission in Enum.GetValues(typeof(MissionType))) {
					if (mission.IsEMS() || !mission.IsDeclarationMission() || mission == MissionType.ExemptedMission) {
						continue;
					}
					var values = table.Columns[mission.ToString().ToLower()].Values<string>().ToDouble().ToArray();
					Data[mission] = new Entry { Urban = values[0], Rural = values[1], Motorway = values[2] };
				}
			}
		}

		private struct Entry
		{
			public double Rural;
			public double Urban;
			public double Motorway;
		}
	}
}