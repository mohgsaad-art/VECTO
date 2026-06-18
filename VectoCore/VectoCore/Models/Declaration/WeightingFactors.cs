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
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class WeightingFactors : LookupData
	{
		private const double VocationalFactorsSum = 2.0;
		private const double NonVocationalFactorsSum = 1.0;

		private readonly Dictionary<WeightingGroup, Dictionary<Tuple<MissionType, LoadingType>, double>> Data = new Dictionary<WeightingGroup, Dictionary<Tuple<MissionType, LoadingType>, double>>();

		public IDictionary<Tuple<MissionType, LoadingType>, double> Lookup(WeightingGroup group)
		{
			try {
				return new ReadOnlyDictionary<Tuple<MissionType, LoadingType>, double>(Data[group]);
			} catch (Exception e) {
				throw new VectoException(string.Format(ErrorMessage, group), ErrorCodes.SYSTEM_KEY_NOT_FOUND, e);
			}
		}

		#region Overrides of LookupData

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".CO2Standards.MissionProfileWeights.csv";
		protected override string ErrorMessage => "No Weighting Factors found for Weighting Group {0}";

		protected override void ParseData(DataTable table)
		{
			var loadingTypes = new[] { LoadingType.LowLoading, LoadingType.ReferenceLoad };
			var missions = new[] {
				MissionType.LongHaul, MissionType.LongHaulEMS, MissionType.RegionalDelivery, MissionType.RegionalDeliveryEMS,
				MissionType.UrbanDelivery, MissionType.MunicipalUtility, MissionType.Construction, MissionType.HeavyUrban,
				MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach
			};
			foreach (DataRow row in table.Rows) {
				var weightingGroup = WeightingGroupHelper.Parse(row.Field<string>("weightinggroup"));
				var weightingGroupValues = Data.GetOrAdd(weightingGroup, _=>new Dictionary<Tuple<MissionType, LoadingType>, double>());
				
				foreach (var missionType in missions) {
					var factors = row.Field<string>(missionType.GetName());
					if (string.IsNullOrWhiteSpace(factors)) {
						continue;
					}

					var factorsPerLoading = factors.Split('/');
					if (factorsPerLoading.Length != loadingTypes.Length) {
						throw new VectoException("Number of entries in MissionProfileWeights does not match expected payloads", ErrorCodes.DATATABLE_INVALID_DATA);
					}
					for (var i = 0; i < loadingTypes.Length; i++) {
						var cycleAndPayload = Tuple.Create(missionType, loadingTypes[i]);
						weightingGroupValues[cycleAndPayload] = factorsPerLoading[i].ToDouble(0);
					}
				}
			}

			foreach (var entry in Data) {
				var sum = entry.Value.Sum(item => item.Value);

				bool hasVocationalWeights = sum.IsEqual(VocationalFactorsSum, 1e-12);
				bool isNormalWeights = sum.IsEqual(NonVocationalFactorsSum, 1e-12);

				if (!isNormalWeights && !hasVocationalWeights)
				{
					throw new VectoException($"Weighting Factors for {entry.Key} do not sum up to 1.0! sum: {sum}", ErrorCodes.DATATABLE_INVALID_DATA);
				}
			}
		}

		#endregion
	}
}
