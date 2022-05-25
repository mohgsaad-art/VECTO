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
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	public enum RetarderType
	{
		None,
		TransmissionInputRetarder,
		TransmissionOutputRetarder,
		EngineRetarder,
		LossesIncludedInTransmission,
		AxlegearInputRetarder
	}

	public static class RetarderTypeHelper
	{
		public static RetarderType Parse(string retarderType)
		{
			switch (retarderType.ToLowerInvariant())
			{
				case "primary": return RetarderType.TransmissionInputRetarder;
				case "secondary": return RetarderType.TransmissionOutputRetarder;
				case "losses included in gearbox": return RetarderType.LossesIncludedInTransmission;
				case "axlegear input retarder": return RetarderType.AxlegearInputRetarder;
				default: return retarderType.ParseEnum<RetarderType>();
			}
		}

		public static string GetName(this RetarderType retarder)
		{
			switch (retarder)
			{
				case RetarderType.TransmissionInputRetarder: return "primary";
				case RetarderType.TransmissionOutputRetarder: return "secondary";
				default: return retarder.ToString();
			}
		}

		public static string GetLabel(this RetarderType retarderType)
		{
			switch (retarderType)
			{
				case RetarderType.None: return "None";
				case RetarderType.TransmissionInputRetarder: return "Transmission Input Retarder";
				case RetarderType.TransmissionOutputRetarder: return "Transmission Output Retarder";
				case RetarderType.EngineRetarder: return "Engine Retarder";
				case RetarderType.LossesIncludedInTransmission: return "Included in Transmission Loss Maps";
				case RetarderType.AxlegearInputRetarder: return "Axlegear Input Retarder";
				default: throw new ArgumentOutOfRangeException(nameof(retarderType), retarderType, null);
			}
		}

		public static string ToXMLFormat(this RetarderType retarderType)
		{
			switch (retarderType)
			{
				case RetarderType.None: return "None";
				case RetarderType.TransmissionInputRetarder: return "Transmission Input Retarder";
				case RetarderType.TransmissionOutputRetarder: return "Transmission Output Retarder";
				case RetarderType.EngineRetarder: return "Engine Retarder";
				case RetarderType.LossesIncludedInTransmission: return "Losses included in Gearbox";
				case RetarderType.AxlegearInputRetarder: return "Axlegear Input Retarder";
				default: throw new ArgumentOutOfRangeException(nameof(retarderType), retarderType, null);
			}
		}

		public static bool IsDedicatedComponent(this RetarderType retarder) =>
			retarder.IsOneOf(
				RetarderType.TransmissionInputRetarder,
				RetarderType.TransmissionOutputRetarder,
				RetarderType.AxlegearInputRetarder);
	}
}
