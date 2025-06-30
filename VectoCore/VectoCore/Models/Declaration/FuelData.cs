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
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
    public sealed class FuelData : LookupData
	{
		private static FuelData _instance;

		private List<Entry> _data = new List<Entry>();

		public static FuelData Instance()
		{
			return _instance ?? (_instance = new FuelData());
		}

		private FuelData() { }

		protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".FuelTypes.csv";

		protected override string ErrorMessage => "FuelType {0} {1} not found!";

		public IFuelProperties Lookup(FuelType fuelType, TankSystem? tankSystem = null)
		{
			var entries = _data.FindAll(x => x.FuelType == fuelType);
			if (entries.Count == 0) {
				throw new VectoException(ErrorMessage, fuelType.ToString(), tankSystem?.ToString() ?? "<no tank system specified>");
			}

			if (entries.Count > 1) {
				entries = entries.FindAll(x => x.TankSystem == tankSystem);
			}
			if (entries.Count == 0) {
				throw new VectoException(ErrorMessage, fuelType.ToString(), tankSystem?.ToString() ?? "<no tank system specified>");
			}

			return entries.First();
		}

		public static IFuelProperties Diesel => Instance().Lookup(FuelType.DieselCI);

		/// <summary>
		/// Used for Fuel Cell
		/// </summary>
		public static IFuelProperties H2 => Instance().Lookup(FuelType.H2FC);


		protected override void ParseData(DataTable table)
		{
			foreach (DataRow row in table.Rows) {
				var density = row.Field<string>("fueldensity");
				var tankSystem = row.Field<string>("tanksystem");
				_data.Add(
					new Entry(
						row.Field<string>(0).ParseEnum<FuelType>(),
						string.IsNullOrWhiteSpace(tankSystem) ? (TankSystem?)null : tankSystem.ParseEnum<TankSystem>(),
						string.IsNullOrWhiteSpace(density) ? null : density.ToDouble(0).SI<KilogramPerCubicMeter>(),
						row.ParseDouble("co2perfuelweight"), row.ParseDouble("co2perfuelweightVTP"),
						row.ParseDouble("ncv_stdvecto").SI(Unit.SI.Kilo.Joule.Per.Kilo.Gramm).Cast<JoulePerKilogramm>(),
						row.ParseDouble("ncv_stdengine").SI(Unit.SI.Kilo.Joule.Per.Kilo.Gramm).Cast<JoulePerKilogramm>()));
			}
		}

		public class Entry : IFuelProperties
		{
			#region Equality members

			protected bool Equals(Entry other)
			{
				return FuelType == other.FuelType && TankSystem == other.TankSystem && Equals(FuelDensity, other.FuelDensity) &&
						CO2PerFuelWeight.Equals(other.CO2PerFuelWeight) && Equals(LowerHeatingValueVecto, other.LowerHeatingValueVecto) &&
						Equals(LowerHeatingValueVectoEngine, other.LowerHeatingValueVectoEngine);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj))
					return false;
				if (ReferenceEquals(this, obj))
					return true;
				if (obj.GetType() != GetType())
					return false;

				return Equals((Entry)obj);
			}

			public override int GetHashCode()
			{
				unchecked {
					var hashCode = (int)FuelType;
					hashCode = (hashCode * 397) ^ TankSystem.GetHashCode();
					hashCode = (hashCode * 397) ^ (FuelDensity != null ? FuelDensity.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ CO2PerFuelWeight.GetHashCode();
					hashCode = (hashCode * 397) ^ (LowerHeatingValueVecto != null ? LowerHeatingValueVecto.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (LowerHeatingValueVectoEngine != null ? LowerHeatingValueVectoEngine.GetHashCode() : 0);
					return hashCode;
				}
			}

			#endregion

			public Entry(FuelType type, TankSystem? tankSystem, KilogramPerCubicMeter density, double weightCO2, double weightCO2VTP, JoulePerKilogramm heatingValueVecto, JoulePerKilogramm heatingValueAnnex)
			{
				FuelType = type;
				TankSystem = tankSystem;
				FuelDensity = density;
				CO2PerFuelWeight = weightCO2;
				CO2PerFuelWeightVTP = weightCO2VTP;
				LowerHeatingValueVecto = heatingValueVecto;
				LowerHeatingValueVectoEngine = heatingValueAnnex;
			}


			public FuelType FuelType { get; }

			public TankSystem? TankSystem { get; }

			public KilogramPerCubicMeter FuelDensity { get; }

			public double CO2PerFuelWeight { get; }

			public double CO2PerFuelWeightVTP { get; }

			public JoulePerKilogramm LowerHeatingValueVecto { get; }

			public JoulePerKilogramm LowerHeatingValueVectoEngine { get; }

			public double HeatingValueCorrection => LowerHeatingValueVectoEngine / LowerHeatingValueVecto;

			public string GetLabel()
			{
				return (TankSystem != null ? (TankSystem == VectoCommon.InputData.TankSystem.Liquefied ? "L" : "C") : "") +
						FuelType.GetLabel();
			}

			
		}
	}
}
