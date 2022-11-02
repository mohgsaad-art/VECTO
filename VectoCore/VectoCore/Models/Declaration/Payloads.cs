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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class Payloads : LookupData<Kilogram, Payloads.PayloadEntry>
	{
		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Payloads.csv";

		protected override string ErrorMessage => throw new InvalidOperationException("ErrorMessage not applicable.");

		public Kilogram Lookup10Percent(Kilogram grossVehicleWeight)
		{
            var section = Data.GetSection(d => d.Key < grossVehicleWeight);
			return VectoMath.Interpolate(section.Item1.Key, section.Item2.Key,
				section.Item1.Value.Payload10Percent, section.Item2.Value.Payload10Percent,
				grossVehicleWeight);
		}

		public Kilogram Lookup50Percent(Kilogram grossVehicleWeight)
		{
			var section = Data.GetSection(d => d.Key < grossVehicleWeight);
			return VectoMath.Interpolate(section.Item1.Key, section.Item2.Key,
				section.Item1.Value.Payload50Percent, section.Item2.Value.Payload50Percent,
				grossVehicleWeight);
		}

		public Kilogram Lookup75Percent(Kilogram grossVehicleWeight)
		{
			var section = Data.GetSection(d => d.Key < grossVehicleWeight);
			return VectoMath.Interpolate(section.Item1.Key, section.Item2.Key,
				section.Item1.Value.Payload75Percent, section.Item2.Value.Payload75Percent,
				grossVehicleWeight);
		}

		public Kilogram LookupTrailer(Kilogram grossVehicleWeight, Kilogram curbWeight)
		{
			return 0.75 * (grossVehicleWeight - curbWeight);
		}

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>()
				.ToDictionary(
					kv => kv.ParseDouble("grossvehicleweight").SI<Kilogram>(),
					kv => new PayloadEntry {
						Payload10Percent = kv.ParseDouble("payload10%").SI<Kilogram>(),
						Payload50Percent = kv.ParseDouble("payload50%").SI<Kilogram>(),
						Payload75Percent = kv.ParseDouble("payload75%").SI<Kilogram>()
					});
		}

		public struct PayloadEntry
		{
			public Kilogram Payload10Percent;
			public Kilogram Payload50Percent;
			public Kilogram Payload75Percent;
		}
	}
}