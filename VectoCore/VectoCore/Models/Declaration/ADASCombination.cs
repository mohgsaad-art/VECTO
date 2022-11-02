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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{

	public struct ADASCombination
	{
		public string ID;
		public bool AllowedForAT;
		public bool AllowedForAMT;
		public bool AllowedForMT;
	}
	public sealed class ADASCombinations : LookupData<bool, EcoRollType, PredictiveCruiseControlType, ADASCombination>
	{
		private readonly List<Entry> _combinations = new List<Entry>();

		#region Overrides of LookupData

		public override ADASCombination Lookup(bool enginestopstart, EcoRollType ecoRoll, PredictiveCruiseControlType pcc)
		{
			try {
				var entry = _combinations.First(
					x => x.EngineStopStart == enginestopstart && x.EcoRoll == ecoRoll &&
						x.PCCType == pcc);
				return new ADASCombination {
					ID = entry.ADASCombination,
					AllowedForAT = entry.AllowedForAT,
					AllowedForMT = entry.AllowedForMT,
					AllowedForAMT = entry.AllowedForAMT
				};
			} catch (Exception) {
				throw new VectoException(string.Format(ErrorMessage, enginestopstart, ecoRoll, pcc));
			}
		}

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".ADAS.ADAS_Combinations.csv";

		protected override string ErrorMessage => "ADAS Combination Lookup Error: No entry found for engine stop/start: {0}, eco roll: {1}, PCC: {2}";

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				_combinations.Add(new Entry() {
					EngineStopStart = row.ParseBoolean("enginestopstart"),
					EcoRoll = EcorollTypeHelper.Get(row.ParseBoolean("ecorollwithoutenginestop"), row.ParseBoolean("ecorollwithenginestop")),
					PCCType = PredictiveCruiseControlTypeHelper.Parse(row.Field<string>("predictivecruisecontrol")),
					ADASCombination = row.Field<string>("adascombination"),
					AllowedForAT = row.ParseBoolean("allowedat"),
					AllowedForAMT = row.ParseBoolean("allowedamt"),
					AllowedForMT = row.ParseBoolean("allowedmt")
				});
			}
		}

		#endregion

		public struct Entry
		{
			public bool EngineStopStart;
			public EcoRollType EcoRoll;
			public PredictiveCruiseControlType PCCType;
			public string ADASCombination;
			public bool AllowedForAMT;
			public bool AllowedForAT;
			public bool AllowedForMT;
		}

		public ADASCombination Lookup(IAdvancedDriverAssistantSystemDeclarationInputData adas, GearboxType gbxType)
		{
			var entry = Lookup(
				adas.EngineStopStart, adas.EcoRoll, adas.PredictiveCruiseControl);
			if (gbxType.AutomaticTransmission() && !entry.AllowedForAT) {
				throw new VectoException("ADAS combination {0} not allowed for AT transmissions", entry.ID);
			}

			if (gbxType == GearboxType.MT && !entry.AllowedForMT) {
				throw new VectoException("ADAS combination {0} not allowed for MT transmissions", entry.ID);
			}

			if (gbxType == GearboxType.AMT && !entry.AllowedForAMT) {
				throw new VectoException("ADAS combination {0} not allowed for AMT transmissions", entry.ID);
			}

			return entry;
		}

		internal ADASCombination Lookup(VehicleData.ADASData adas, GearboxType gbxType)
		{
			var entry = Lookup(
				adas.EngineStopStart, adas.EcoRoll, adas.PredictiveCruiseControl);
			if (gbxType.AutomaticTransmission() && !entry.AllowedForAT) {
				throw new VectoException("ADAS combination {0} not allowed for AT transmissions", entry.ID);
			}

			if (gbxType == GearboxType.MT && !entry.AllowedForMT) {
				throw new VectoException("ADAS combination {0} not allowed for MT transmissions", entry.ID);
			}

			if (gbxType == GearboxType.AMT && !entry.AllowedForAMT) {
				throw new VectoException("ADAS combination {0} not allowed for AMT transmissions", entry.ID);
			}
			return entry;
		}
	}
}
