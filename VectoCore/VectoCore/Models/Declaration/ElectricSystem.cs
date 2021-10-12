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
	public sealed class ElectricSystem : LookupData<MissionType, string, AuxDemandEntry>, IDeclarationAuxiliaryTable
	{
		private readonly Alternator _alternator = new Alternator();

		protected override string ResourceId
		{
			get { return DeclarationData.DeclarationDataResourcePrefix + ".VAUX.ES-Tech.csv"; }
		}

		protected override string ErrorMessage
		{
			get { return "Auxiliary Lookup Error: No value found for Electric System. Mission: '{0}', Technology: '{1}'"; }
		}

		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				var name = row.Field<string>("technology");
				foreach (DataColumn col in table.Columns) {
					if (col.Caption != "technology") {
						Data[Tuple.Create(col.Caption.ParseEnum<MissionType>(), name)] =
							new AuxDemandEntry() { PowerDemand = row.ParseDouble(col).SI<Watt>() };
					}
				}
			}
		}

		public override AuxDemandEntry Lookup(MissionType missionType, string technology = null)
		{
			if (string.IsNullOrWhiteSpace(technology)) {
				technology = "Standard technology";
			}
			var value = base.Lookup(missionType, technology);
			return new AuxDemandEntry() { PowerDemand = value.PowerDemand / _alternator.Lookup(missionType) };
		}

		internal sealed class Alternator : LookupData<MissionType, string, double>
		{
			protected override string ResourceId
			{
				get { return DeclarationData.DeclarationDataResourcePrefix + ".VAUX.ALT-Tech.csv"; }
			}

			protected override string ErrorMessage
			{
				get { return "Auxiliary Lookup Error: No value found for Alternator. Mission: '{0}', Technology: '{1}'"; }
			}

			protected override void ParseData(DataTable table)
			{
				foreach (DataRow row in table.Rows) {
					var name = row.Field<string>("technology");
					foreach (DataColumn col in table.Columns) {
						if (col.Caption != "technology") {
							Data[Tuple.Create(col.Caption.ParseEnum<MissionType>(), name)] = row.ParseDouble(col);
						}
					}
				}
			}

			public override double Lookup(MissionType missionType, string technology = null)
			{
				if (string.IsNullOrWhiteSpace(technology)) {
					technology = "Standard alternator efficiency";
				}
				return base.Lookup(missionType, technology);
			}
		}

		public string[] GetTechnologies()
		{
			return Data.Keys.Select(x => x.Item2).Distinct().ToArray();
		}
	}
}