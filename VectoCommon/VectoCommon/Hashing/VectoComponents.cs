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
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCommon.Hashing
{
	public enum VectoComponents
	{
		Engine,
		Gearbox,
		Axlegear,
		Retarder,
		TorqueConverter,
		Angledrive,
		Airdrag,
		Tyre,
		Vehicle,
		VectoOutput,
		VectoCustomerInformation,
		VectoPrimaryVehicleInformation,
		VectoManufacturingStep, 
		BatterySystem,
		CapacitorSystem,
		ElectricMachineSystem,
		IEPC,
		ADC,
		CertifiedAeroReduction,
		ElectricEnergyStorage
	}

	public static class VectoComponentsExtensionMethods
	{
		public static string XMLElementName(this VectoComponents component)
		{
			switch (component) {
				case VectoComponents.Engine:
					return XMLNames.Component_Engine;
				case VectoComponents.Gearbox:
					return XMLNames.Component_Gearbox;
				case VectoComponents.Axlegear:
					return XMLNames.Component_Axlegear;
				case VectoComponents.Retarder:
					return XMLNames.Component_Retarder;
				case VectoComponents.TorqueConverter:
					return XMLNames.Component_TorqueConverter;
				case VectoComponents.Angledrive:
					return XMLNames.Component_Angledrive;
				case VectoComponents.Airdrag:
					return XMLNames.Component_AirDrag;
				case VectoComponents.Tyre:
					return XMLNames.AxleWheels_Axles_Axle_Tyre;
				case VectoComponents.Vehicle:
					return XMLNames.Component_Vehicle;
				case VectoComponents.VectoOutput:
					return "VectoOutput";
				case VectoComponents.VectoCustomerInformation:
					return "VectoCustomerInformation";
				case VectoComponents.VectoPrimaryVehicleInformation:
					return "PrimaryVehicle";
				case VectoComponents.VectoManufacturingStep:
					return XMLNames.ManufacturingStep;
				case VectoComponents.BatterySystem:
					return XMLNames.Component_BatterySystem;
				case VectoComponents.CapacitorSystem:
					return XMLNames.Component_CapacitorSystem;
				case VectoComponents.ElectricMachineSystem:
					return XMLNames.Component_ElectricMachineSystem;
				case VectoComponents.IEPC:
					return XMLNames.Component_IEPC;
				case VectoComponents.ADC:
					return XMLNames.Component_ADC;
				case VectoComponents.CertifiedAeroReduction:
					return XMLNames.Component_CertifiedAeroReduction;
				case VectoComponents.ElectricEnergyStorage:
					return XMLNames.REESS;
				default:
					throw new ArgumentOutOfRangeException("VectoComponents", component, null);
			}
		}

		public static string HashIdPrefix(this VectoComponents component)
		{
			switch (component) {
				case VectoComponents.Engine:
					return "ENG-";
				case VectoComponents.Gearbox:
					return "GBX-";
				case VectoComponents.Axlegear:
					return "AXL-";
				case VectoComponents.Retarder:
					return "RET-";
				case VectoComponents.TorqueConverter:
					return "TC-";
				case VectoComponents.Angledrive:
					return "ANGL-";
				case VectoComponents.Airdrag:
					return "AD-";
				case VectoComponents.Tyre:
					return "TYRE-";
				case VectoComponents.VectoOutput:
					return "RESULT-";
				case VectoComponents.VectoCustomerInformation:
					return "COC-";
				case VectoComponents.VectoPrimaryVehicleInformation:
					return "PIF-";
				case VectoComponents.Vehicle:
					return "VEH-";
				case VectoComponents.VectoManufacturingStep:
					return "MST-";
				case VectoComponents.BatterySystem:
					return "BAT-";
				case VectoComponents.CapacitorSystem:
					return "CAP-";
				case VectoComponents.ElectricMachineSystem:
					return "EM-";
				case VectoComponents.IEPC:
					return "IEPC-";
				case VectoComponents.ADC:
					return "ADC-";
				case VectoComponents.CertifiedAeroReduction:
					return "AERO-";
				case VectoComponents.ElectricEnergyStorage:
					return "REESS-";
				default:
					throw new ArgumentOutOfRangeException("VectoComponents", component, null);
			}
		}

		public static bool IsReport(this VectoComponents component)
		{
			switch (component) {
				case VectoComponents.VectoCustomerInformation:
				case VectoComponents.VectoOutput:
				case VectoComponents.VectoPrimaryVehicleInformation:
				case VectoComponents.VectoManufacturingStep:
					return true;
				default:
					return false;
			}
		}
	}
}