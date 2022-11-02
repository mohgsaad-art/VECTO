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

namespace TUGraz.VectoCommon.Models
{
	public enum VehicleCategory
	{
		Unknown,
        Van,
		RigidTruck,
		Tractor,
		CityBus,
		//InterurbanBus,
		Coach,
		HeavyBusPrimaryVehicle,
		HeavyBusCompletedVehicle,
		GenericBusVehicle,
		HeavyBusInterimVehicle
	}

	public static class VehicleCategoryHelper
	{
		public static string GetLabel(this VehicleCategory category)
		{
			switch (category) {
                case VehicleCategory.Van:
                    return "Van";
                case VehicleCategory.RigidTruck:
					return "Rigid Truck";
				case VehicleCategory.Tractor:
					return "Tractor";
				//case VehicleCategory.CityBus:
				//	return "City Bus";
				//case VehicleCategory.InterurbanBus:
				//	return "Interurban Bus";
				//case VehicleCategory.Coach:
				//	return "Coach";
				case VehicleCategory.HeavyBusPrimaryVehicle:
					return "Heavy Bus, Primary Vehicle";
				case VehicleCategory.HeavyBusCompletedVehicle:
					return "Heavy Bus, Completed Vehicle";
				default:
					return category.ToString();
			}
		}

		public static string GetCategoryName(this VehicleCategory category)
		{
			switch (category) {
                case VehicleCategory.Van:
                    return "Van";
                case VehicleCategory.RigidTruck:
					return "Rigid Truck";
				case VehicleCategory.Tractor:
					return "Semitrailer Truck";
				//case VehicleCategory.CityBus:
				//	return "Citybus";
				//case VehicleCategory.InterurbanBus:
				//	return "Interurban Bus";
				//case VehicleCategory.Coach:
				//	return "Coach";
				case VehicleCategory.HeavyBusPrimaryVehicle:
					return "Heavy Bus, Primary Vehicle";
				case VehicleCategory.HeavyBusCompletedVehicle:
					return "Heavy Bus, Completed Vehicle";
				default:
					return category.ToString();
			}
		}

		public static string ToXMLFormat(this VehicleCategory vehicleCategory)
		{
			switch (vehicleCategory) {
				//case VehicleCategory.Coach:
				case VehicleCategory.Tractor:
					return vehicleCategory.ToString();
				//case VehicleCategory.CityBus:
				//	return "City Bus";
				//case VehicleCategory.InterurbanBus:
				//	return "Interurban Bus";
				case VehicleCategory.RigidTruck:
					return "Rigid Lorry";
				case VehicleCategory.HeavyBusPrimaryVehicle:
					return "Bus";
				case VehicleCategory.HeavyBusCompletedVehicle:
					return "Bus";
				default:
					throw new ArgumentOutOfRangeException("vehicleCategory", vehicleCategory, null);
			}
		}

		public static bool IsLorry(this VehicleCategory category)
		{
			switch (category) {
                case VehicleCategory.Van:
                case VehicleCategory.RigidTruck:
				case VehicleCategory.Tractor:
					return true;
				case VehicleCategory.HeavyBusCompletedVehicle:
				case VehicleCategory.GenericBusVehicle:
				default:
					return false;
			}
		}

		public static bool IsBus(this VehicleCategory category)
		{
			switch (category) {
				case VehicleCategory.Coach:
				case VehicleCategory.CityBus:
				case VehicleCategory.HeavyBusPrimaryVehicle:
				case VehicleCategory.HeavyBusCompletedVehicle:
				case VehicleCategory.GenericBusVehicle: return true;
				default: return false;
			}
		}

		public static VehicleCategory Parse(string vehicleCategory)
		{
			switch (vehicleCategory) {
				case "Bus":
					return VehicleCategory.HeavyBusPrimaryVehicle;
				case "Tractor":
					return VehicleCategory.Tractor;
				case "Rigid Lorry":
					return VehicleCategory.RigidTruck;
				case "Van":
					return VehicleCategory.Van;
				default:
					return VehicleCategory.Unknown;
			}
		}
	}
}