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
using System.Diagnostics.CodeAnalysis;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public enum AxleConfiguration
	{
		AxleConfig_Undefined,
		AxleConfig_4x2,
		AxleConfig_4x2F,
		AxleConfig_4x4,
		AxleConfig_6x2,
		AxleConfig_6x4,
		AxleConfig_6x6,
		AxleConfig_8x2,
		AxleConfig_8x4,
		AxleConfig_8x6,
		AxleConfig_8x8,
	}

	public enum AxleType
	{
		VehicleDriven,
		VehicleNonDriven,
		Trailer
	}

	public static class AxleTypeHelper
	{
		public static string GetLabel(this AxleType self)
		{
			switch (self) {
				case AxleType.VehicleDriven:
					return "Vehicle driven";
				case AxleType.VehicleNonDriven:
					return "Vehicle non-driven";
				case AxleType.Trailer:
					return "Trailer";
				default:
					throw new ArgumentOutOfRangeException("self", self, null);
			}
		}
	}

	public static class AxleConfigurationHelper
	{
		private const string Prefix = "AxleConfig_";

		public static string GetName(this AxleConfiguration self)
		{
			return self.ToString().Replace(Prefix, "");
		}

		public static string ToXMLFormat(this AxleConfiguration self)
		{
			return self.GetName();
		}

		public static AxleConfiguration Parse(string typeString)
		{
			return (Prefix + typeString).ParseEnum<AxleConfiguration>();
		}

		public static int NumAxles(this AxleConfiguration self)
		{
			switch (self) {
				case AxleConfiguration.AxleConfig_4x2:
				case AxleConfiguration.AxleConfig_4x4: return 2;
				case AxleConfiguration.AxleConfig_6x2:
				case AxleConfiguration.AxleConfig_6x4:
				case AxleConfiguration.AxleConfig_6x6: return 3;
				case AxleConfiguration.AxleConfig_8x2:
				case AxleConfiguration.AxleConfig_8x4:
				case AxleConfiguration.AxleConfig_8x6:
				case AxleConfiguration.AxleConfig_8x8: return 4;
			}

			return 0;
		}

		public static int NumDrivenAxles(this AxleConfiguration self)
		{
			switch (self) {
				case AxleConfiguration.AxleConfig_4x2:
				case AxleConfiguration.AxleConfig_4x2F:
				case AxleConfiguration.AxleConfig_6x2:
				case AxleConfiguration.AxleConfig_8x2: return 1;
				case AxleConfiguration.AxleConfig_4x4:
				case AxleConfiguration.AxleConfig_6x4:
				case AxleConfiguration.AxleConfig_8x4: return 2;
				case AxleConfiguration.AxleConfig_6x6:
				case AxleConfiguration.AxleConfig_8x6: return 3;
				case AxleConfiguration.AxleConfig_8x8: return 4;
			}

			return 0;
		}

		public static bool AxlegearIncludedInGearbox(this AxleConfiguration self)
		{
			return self == AxleConfiguration.AxleConfig_4x2F;
		}
	}
}