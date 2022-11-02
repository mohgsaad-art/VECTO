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
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class SteeringPump : IDeclarationAuxiliaryTable
	{
		private readonly SteeringPumpBaseLine _baseLookup = new SteeringPumpBaseLine();
		private readonly SteeringPumpAxles _axleLookup = new SteeringPumpAxles();
		private readonly SteeringPumpTechnologies _techLookup = new SteeringPumpTechnologies();

		public Watt Lookup(MissionType mission, VehicleClass hdvClass, IEnumerable<string> technologies)
		{
			var baseLine = _baseLookup.Lookup(mission, hdvClass);
			var power = new SteeringPumpValues<Watt>(0.SI<Watt>(), 0.SI<Watt>(), 0.SI<Watt>());
			var factors = new SteeringPumpValues<double>(0, 0, 0);
			var i = 0;
			foreach (var technology in technologies) {
				i++;
				var axles = _axleLookup.Lookup(mission, i);
				power.UnloadedFriction += baseLine.UnloadedFriction * axles.UnloadedFriction;
				power.Banking += baseLine.Banking * axles.Banking;
				power.Steering += baseLine.Steering * axles.Steering;

				var f = _techLookup.Lookup(technology, mission);
				factors.UnloadedFriction += f.UnloadedFriction;
				factors.Banking += f.Banking;
				factors.Steering += f.Steering;
			}

			power.UnloadedFriction *= factors.UnloadedFriction / i;
			power.Banking *= factors.Banking / i;
			power.Steering *= factors.Steering / i;

			return power.UnloadedFriction + power.Banking + power.Steering;
		}

		private sealed class SteeringPumpBaseLine : LookupData<MissionType, VehicleClass, SteeringPumpValues<Watt>>
		{
			protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUX.SP-Table.csv";

			protected override string ErrorMessage => "Auxiliary Lookup Error: No value found for Steering Pump. Mission: '{0}', HDVClass: '{1}'";

			protected override void ParseData(DataTable table)
			{
				foreach (DataRow row in table.Rows) {
					var hdvClass = VehicleClassHelper.Parse(row.Field<string>("hdvgroup"));
					foreach (DataColumn col in table.Columns) {
						if (col.Caption == "hdvgroup" || string.IsNullOrWhiteSpace(row.Field<string>(col.Caption))) {
							continue;
						}
						var values = row.Field<string>(col.Caption).Split('/')
							.Select(v => v.ToDouble() / 100.0).Concat(0.0.Repeat(3)).SI<Watt>().ToList();
						Data[Tuple.Create(col.Caption.ParseEnum<MissionType>(), hdvClass)] = new SteeringPumpValues<Watt>(values[0],
							values[1], values[2]);
					}
				}
			}
		}

		private sealed class SteeringPumpTechnologies : LookupData<string, SteeringPumpValues<double>>
		{
			protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUX.SP-Tech.csv";

			protected override string ErrorMessage => "Auxiliary Lookup Error: No value found for SteeringPump Technology. Key: '{0}'";

			protected override void ParseData(DataTable table)
			{
				Data = table.Rows.Cast<DataRow>().ToDictionary(
					key => key.Field<string>("Technology"),
					value => new SteeringPumpValues<double>(value.ParseDouble("UF"), value.ParseDouble("B"), value.ParseDouble("S")));
			}

			public override SteeringPumpValues<double> Lookup(string key)
			{
				throw new InvalidOperationException("Standard lookup is not supported. Use Lookup(string, MissionType) instead.");
			}

			/// <summary>
			/// Lookup for Steering Pump Technologies.
			/// </summary>
			/// <param name="tech">The technology string.</param>
			/// <param name="mission">Only used when Tech is Electric System.</param>
			/// <returns></returns>
			public SteeringPumpValues<double> Lookup(string tech, MissionType mission)
			{
				var values = base.Lookup(tech);
				return values;
			}

			public string[] GetTechnologies()
			{
				return Data.Keys.Distinct().ToArray();
			}
		}

		private sealed class SteeringPumpAxles : LookupData<MissionType, int, SteeringPumpValues<double>>
		{
			protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".VAUX.SP-Axles.csv";

			protected override string ErrorMessage => "Auxiliary Lookup Error: No value found for SteeringPump Axle. Mission: '{0}', Axle Count: '{1}'";

			protected override void ParseData(DataTable table)
			{
				foreach (DataRow row in table.Rows) {
					var axleNumber = int.Parse(row.Field<string>("steeredaxles"));
					foreach (DataColumn col in table.Columns) {
						if (col.Caption == "steeredaxles") {
							continue;
						}
						var field = row.Field<string>(col.Caption);
						if (string.IsNullOrWhiteSpace(field)) {
							continue;
						}
						var values = field.Split('/').ToDouble().Concat(0.0.Repeat(3)).ToList();
						Data[Tuple.Create(col.Caption.ParseEnum<MissionType>(), axleNumber)] = new SteeringPumpValues<double>(values[0],
							values[1], values[2]);
					}
				}
			}
		}

		[DebuggerDisplay("UnloadedFriction = {UnloadedFriction,nq}, Banking = {Banking,nq}, Steering = {Steering,nq}")]
		private struct SteeringPumpValues<T>
		{
			public T UnloadedFriction;
			public T Banking;
			public T Steering;

			public SteeringPumpValues(T unloadedFriction, T banking, T steering)
			{
				UnloadedFriction = unloadedFriction;
				Banking = banking;
				Steering = steering;
			}
		}

		public string[] GetTechnologies()
		{
			return _techLookup.GetTechnologies();
		}
	}
}