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
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;

namespace TUGraz.VectoCore.Models.Declaration
{
	public static class AuxiliaryTypeHelper
	{
		private static readonly Dictionary<AuxiliaryType, string> AuxToStr = new Dictionary<AuxiliaryType, string> {
			{ AuxiliaryType.Fan, Constants.Auxiliaries.Names.Fan },
			{ AuxiliaryType.SteeringPump, Constants.Auxiliaries.Names.SteeringPump },
			{ AuxiliaryType.HVAC, Constants.Auxiliaries.Names.HeatingVentilationAirCondition },
			{ AuxiliaryType.PneumaticSystem, Constants.Auxiliaries.Names.PneumaticSystem },
			{ AuxiliaryType.ElectricSystem, Constants.Auxiliaries.Names.ElectricSystem },
		};

		private static readonly Dictionary<AuxiliaryType, string> AuxToKey = new Dictionary<AuxiliaryType, string> {
			{ AuxiliaryType.Fan, Constants.Auxiliaries.IDs.Fan },
			{ AuxiliaryType.SteeringPump, Constants.Auxiliaries.IDs.SteeringPump },
			{ AuxiliaryType.HVAC, Constants.Auxiliaries.IDs.HeatingVentilationAirCondition },
			{ AuxiliaryType.PneumaticSystem, Constants.Auxiliaries.IDs.PneumaticSystem },
			{ AuxiliaryType.ElectricSystem, Constants.Auxiliaries.IDs.ElectricSystem },
		};

		private static readonly Dictionary<string, AuxiliaryType> StrToAux = AuxToStr.ToDictionary(kv => kv.Value,
			kv => kv.Key);

		public static AuxiliaryType Parse(string s)
		{
			return StrToAux.TryGetValue(s, out var aux) ? aux : AuxiliaryType.Fan;
		}

		public static AuxiliaryType ParseKey(string s)
		{
			return AuxToKey.FirstOrDefault(x => x.Value.Equals(s, StringComparison.InvariantCultureIgnoreCase)).Key;
		}

		public static string ToString(AuxiliaryType t)
		{
			return AuxToStr[t];
		}

		public static string GetAuxKey(AuxiliaryType t)
		{
			return AuxToKey[t];
		}

		public static string Key(this AuxiliaryType t)
		{
			return AuxToKey[t];
		}

		public static string Name(this AuxiliaryType t)
		{
			return AuxToStr[t];
		}
	}
}