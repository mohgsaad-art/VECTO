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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class ADASBenefits : LookupData<VehicleClass, ADASCombination, MissionType, LoadingType, double>
	{
		private readonly Dictionary<Tuple<VehicleClass, ADASCombination>, Dictionary<Tuple<MissionType, LoadingType>, double>> _data = new Dictionary<Tuple<VehicleClass, ADASCombination>, Dictionary<Tuple<MissionType, LoadingType>, double>>();
		
		#region Overrides of LookupData

		public override double Lookup(VehicleClass group, ADASCombination adasCombination, MissionType mission, LoadingType loading)
		{
			var groupAndAdas = Tuple.Create(group, adasCombination);
			var missionAndLoading = Tuple.Create(mission, loading);
			if (!_data.ContainsKey(groupAndAdas)) {
				return 1.0;
			}
			if (!_data[groupAndAdas].ContainsKey(missionAndLoading)) {
				return 1.0;
			}
			return _data[groupAndAdas][missionAndLoading];
		}

		protected override string ResourceId { get { return DeclarationData.DeclarationDataResourcePrefix + ".ADAS.ADAS_Benefits.csv"; } }
		protected override string ErrorMessage { get { return "ADAS Benefits Lookup Error: No value found."; } }
		protected override void ParseData(DataTable table)
		{
			var loadingTypes = new[] { LoadingType.LowLoading, LoadingType.ReferenceLoad };
			foreach (DataRow row in table.Rows) {
				var groupAndAdas = Tuple.Create(VehicleClassHelper.Parse(row.Field<string>("vehiclegroup")), new ADASCombination() { ID = row.Field<string>("adascombination")});
				if (!_data.ContainsKey(groupAndAdas)) {
					_data[groupAndAdas] = new Dictionary<Tuple<MissionType, LoadingType>, double>();
				}
				foreach (var missionType in new[] { MissionType.LongHaul, MissionType.LongHaulEMS, MissionType.RegionalDelivery, MissionType.RegionalDeliveryEMS, MissionType.UrbanDelivery}) {
					var benefits = row.Field<string>(missionType.GetName());
					if (string.IsNullOrWhiteSpace(benefits)) {
						continue;
					}

					var benefitsPerLoading = benefits.Split('/');
					for (var i = 0; i < loadingTypes.Length; i++) {
						var cycleAndPayload = Tuple.Create(missionType, loadingTypes[i]);
						_data[groupAndAdas][cycleAndPayload] = BenefitFacor(benefitsPerLoading[i]);
					}
				}
			}
		}

		private double BenefitFacor(string benefitStr)
		{
			var benefit = benefitStr.Replace("%", "").ToDouble();
			return 1 + benefit / 100.0;
		}

		#endregion

	}
}
