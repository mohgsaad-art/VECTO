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
		TrailerAerodynamicReduction
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
					return "VectoOutputPrimaryVehicle";
				case VectoComponents.TrailerAerodynamicReduction:
					return "CertifiedAeroDevice";
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
				case VectoComponents.TrailerAerodynamicReduction:
					return "AERO-";
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
					return true;
				default:
					return false;
			}
		}
	}
}