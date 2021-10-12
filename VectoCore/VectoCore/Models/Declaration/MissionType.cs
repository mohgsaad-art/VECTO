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
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.Models.Declaration
{
	
	public static class MissionTypeHelper
	{
		public static string GetName(this MissionType self)
		{
			return self.ToString().ToLowerInvariant();
		}

		public static bool IsEMS(this MissionType self)
		{
			return self == MissionType.LongHaulEMS || self == MissionType.RegionalDeliveryEMS;
		}

		public static bool IsDeclarationMission(this MissionType self)
		{
			return self != MissionType.VerificationTest;
		}

		public static MissionType GetNonEMSMissionType(this MissionType self)
		{
			if (self == MissionType.LongHaulEMS) {
				return MissionType.LongHaul;
			}
			if (self == MissionType.RegionalDeliveryEMS) {
				return MissionType.RegionalDelivery;
			}
			return self;
		}

		public static string GetLabel(this MissionType self)
		{
			return self.ToXMLFormat();
		}

		public static string ToXMLFormat(this MissionType self)
		{
			switch (self) {
				case MissionType.LongHaul:
					return "Long Haul";
				case MissionType.LongHaulEMS:
					return "Long Haul EMS";
				case MissionType.RegionalDelivery:
					return "Regional Delivery";
				case MissionType.RegionalDeliveryEMS:
					return "Regional Delivery EMS";
				case MissionType.UrbanDelivery:
					return "Urban Delivery";
				case MissionType.MunicipalUtility:
					return "Municipal Utility";
				case MissionType.Construction:
					return "Construction";
				//case MissionType.HeavyUrban:
				//	return "";
				//case MissionType.Urban:
				//	return "";
				//case MissionType.Suburban:
				//	return "";
				//case MissionType.Interurban:
				//	return "";
				//case MissionType.Coach:
				//	return "";
				default:
					throw new ArgumentOutOfRangeException("MissionType", self, null);
			}
		}

		public static double GetLowLoadFactorBus(this MissionType self)
		{
			switch (self) {
				case MissionType.HeavyUrban:
				case MissionType.Urban:
				case MissionType.Suburban:
					return 0.2;
				case MissionType.Interurban:
					return 0.25;
				case MissionType.Coach:
					return 0.4;
				default:
					return 0.1;
			}
		}
	}
}