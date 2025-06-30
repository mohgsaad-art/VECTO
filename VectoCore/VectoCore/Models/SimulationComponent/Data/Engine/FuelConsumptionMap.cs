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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
{
    public class FuelConsumptionMap : SimulationComponentData, IFuelConsumptionMap
	{
		[Required, ValidateObject] private readonly DelaunayMap _fuelMap;

		protected internal FuelConsumptionMap(DelaunayMap fuelMap)
		{
			_fuelMap = fuelMap;
		}

		/// <summary>
		/// Calculates the fuel consumption based on the given fuel map, the angularVelocity and the torque.
		/// </summary>
		public FuelConsumptionResult GetFuelConsumption(NewtonMeter torque, PerSecond angularVelocity,
			bool allowExtrapolation = false)
		{
			var result = new FuelConsumptionResult();
			
			var value = _fuelMap.Interpolate(torque, angularVelocity);
			if (!value.IsNaN()) {
			    result.Value = value.SI(Unit.SI.Kilo.Gramm.Per.Second).Cast<KilogramPerSecond>();
				return result;
			}

			if (allowExtrapolation) {
				result.Value =
				    _fuelMap.Extrapolate(torque, angularVelocity).SI(Unit.SI.Kilo.Gramm.Per.Second).Cast<KilogramPerSecond>();
				result.Extrapolated = true;
				return result;
			}

			throw new VectoException("FuelConsumptionMap: Interpolation failed. torque: {0}, n: {1}", torque.Value(),
				angularVelocity.AsRPM);
		}

		[JsonIgnore]
		public IReadOnlyCollection<Entry> Entries
		{
			get {
				var entries = _fuelMap.Entries;
				var retVal = new Entry[entries.Count];
				var i = 0;
				foreach (var entry in entries) {
					retVal[i++] = new Entry(entry.Y.SI<PerSecond>(), entry.X.SI<NewtonMeter>(), entry.Z.SI<KilogramPerSecond>());
				}
				return retVal;
			}
		}

		public string[] EntriesSerialized
		{
			get {
				return _fuelMap.Entries.Select(
									entry => $"{entry.Y.SI<PerSecond>().AsRPM} [rpm], {entry.X.SI<NewtonMeter>()}, {entry.Z.SI<KilogramPerSecond>().ConvertToGrammPerHour()} [g/h]")
								.ToArray();
			}
		}


		public class Entry
		{
			[Required, SIRange(0, 5000 * Constants.RPMToRad)] public readonly PerSecond EngineSpeed;
			[Required] public readonly NewtonMeter Torque;
			[Required, SIRange(0, double.MaxValue)] public readonly KilogramPerSecond FuelConsumption;

			public Entry(PerSecond engineSpeed, NewtonMeter torque, KilogramPerSecond fuelConsumption)
			{
				EngineSpeed = engineSpeed;
				Torque = torque;
				FuelConsumption = fuelConsumption;
			}
		}

		#region Implementation of IFuelConsumptionMap

		public KilogramPerSecond GetFuelConsumptionValue(NewtonMeter torque, PerSecond angularVelocity)
		{
			return GetFuelConsumption(torque, angularVelocity, true).Value;
		}

		#endregion
	}
}