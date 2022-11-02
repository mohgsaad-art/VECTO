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

using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public struct StandardBody
	{
		public readonly Kilogram CurbWeight;
		public readonly Kilogram GrossVehicleWeight;
		public readonly SquareMeter[] DeltaCrossWindArea;
		public readonly string Name;
		public readonly List<Wheels.Entry> Wheels;
		public readonly CubicMeter CargoVolume;

		public Kilogram MaxPayLoad => GrossVehicleWeight - CurbWeight;

		public StandardBody(string name, Kilogram curbWeight, Kilogram grossVehicleWeight, SquareMeter[] deltaCrossWindArea,
			Wheels.Entry? wheels, int axleCount, CubicMeter volume) :
				this(name, curbWeight, grossVehicleWeight, deltaCrossWindArea,
					!wheels.HasValue ? new List<Wheels.Entry>() : Enumerable.Repeat(wheels.Value, axleCount).ToList(), volume) {}

		private StandardBody(string name, Kilogram curbWeight, Kilogram grossVehicleWeight, SquareMeter[] deltaCrossWindArea,
			List<Wheels.Entry> wheels, CubicMeter volume)
		{
			Name = name;
			CurbWeight = curbWeight;
			GrossVehicleWeight = grossVehicleWeight;
			DeltaCrossWindArea = deltaCrossWindArea;
			Wheels = wheels;
			CargoVolume = volume;
		}

		//public static StandardBody operator +(StandardBody first, StandardBody second)
		//{
		//	var wheels = new List<Wheels.Entry>(first.Wheels);
		//	wheels.AddRange(second.Wheels);
		//	return new StandardBody(first.Name + second.Name, first.CurbWeight + second.CurbWeight,
		//		first.GrossVehicleWeight + second.GrossVehicleWeight,
		//		first.DeltaCrossWindArea + second.DeltaCrossWindArea,
		//		wheels, first.CargoVolume + second.TotalCargoVolume);
		//}
	}

	/// <summary>
	/// Lookup Class for Standard Weights of Bodies, Trailers and Semitrailers.
	/// Standard Weights include 
	///		CurbWeight (=Empty Weight), 
	///		Gross Vehicle Weight (=Maximum Allowed Weight),
	///		MaxPayload,
	///     DeltaCrossWindArea,
	///     Wheels
	/// </summary>
	public sealed class StandardBodies : LookupData<string, StandardBody>
	{
		public static readonly StandardBody Empty = new StandardBody("", 0.SI<Kilogram>(), 0.SI<Kilogram>(),
			new[] { 0.SI<SquareMeter>(), 0.SI<SquareMeter>() }, null, 0, 0.SI<CubicMeter>());

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".Body_Trailers_Weights.csv";

		protected override string ErrorMessage => "StandardWeigths Lookup Error: No value found for ID '{0}'";

		public override StandardBody Lookup(string id)
		{
			return string.IsNullOrWhiteSpace(id) ? Empty : base.Lookup(id);
		}

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>().Select(k => {
				var deltaCdxAStr = k.Field<string>("deltacdxafortraileroperationinlonghaul");
				var deltaCdxA = new[] { 0.SI<SquareMeter>(), 0.SI<SquareMeter>() };
				if (!deltaCdxAStr.Equals("-")) {
					deltaCdxA = deltaCdxAStr.Split('/').Select(x => x.ToDouble().SI<SquareMeter>()).ToArray();
				}
				return new StandardBody(
					k.Field<string>("name"),
					k.ParseDoubleOrGetDefault("curbmass").SI<Kilogram>(),
					k.ParseDoubleOrGetDefault("maxgrossmass").SI<Kilogram>(),
					deltaCdxA,
					!string.IsNullOrWhiteSpace(k.Field<string>("wheels"))
						? DeclarationData.Wheels.Lookup(k.Field<string>("wheels"))
						: (Wheels.Entry?)null,
					int.Parse(k.Field<string>("axlecount")),
					k.ParseDouble("cargovolume").SI<CubicMeter>());
			})
				.ToDictionary(kv => kv.Name);
		}
	}
}