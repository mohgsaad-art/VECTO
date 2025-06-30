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
using System.Diagnostics;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	public enum GearboxType
	{
		MT, // Manual Transmission
		AMT, // Automated Manual Transmission
		ATSerial, // Automatic Transmission
		ATPowerSplit,
		APTN, // Automatic Power Transmission - No Torque Converter
		IHPC, // Integrated Hybrid Powertrain Component - Transmission part
		IEPC, // not a real gearbox type, only used for GUI and validation
		NoGearbox
	}

	public static class GearBoxTypeHelper
	{
		public static string GetLabel(this GearboxType type)
		{
			switch (type) {
				case GearboxType.MT:           return "Manual Transmission (MT)";
				case GearboxType.AMT:          return "Automated Transmission (AMT)";
				case GearboxType.ATSerial:     return "Automatic Transmission - Serial (AT-S)"; //todo mk20211210 shouldn't we call it APT-S?
				case GearboxType.ATPowerSplit: return "Automatic Transmission - PowerSplit (AT-P)";//todo mk20211210 shouldn't we call it APT-P?
				case GearboxType.APTN:         return "Automatic Transmission - No Torque Converter (APT-N)";
				case GearboxType.IHPC:         return "IHPC Transmission";
				case GearboxType.IEPC:         return "IEPC Transmission - dummy entry";
				default: throw new ArgumentOutOfRangeException("GearboxType", type, null);
			}
		}

		[DebuggerStepThrough]
		public static string ShortName(this GearboxType type) => type.ToString();

		[DebuggerStepThrough]
		public static bool AutomaticTransmission(this GearboxType type) =>
			type == GearboxType.ATPowerSplit || type == GearboxType.ATSerial || type == GearboxType.APTN;

		[DebuggerStepThrough]
		public static bool ManualTransmission(this GearboxType type) =>
			type == GearboxType.MT || type == GearboxType.AMT;

		public static Second TractionInterruption(this GearboxType type)
		{
			switch (type) {
				case GearboxType.MT: return 2.SI<Second>();
				case GearboxType.AMT: return 1.SI<Second>();
				default: return 0.SI<Second>();
			}
		}

		public static string ToXMLFormat(this GearboxType type)
		{
			switch (type) {
				case GearboxType.MT:           return "SMT";
				case GearboxType.AMT:          return "AMT";
				case GearboxType.ATSerial:     return "APT-S";
				case GearboxType.ATPowerSplit: return "APT-P";
				case GearboxType.APTN:         return "APT-N";
				case GearboxType.IHPC:         return "IHPC Type 1";
				default: throw new ArgumentOutOfRangeException("GearboxType", type, null);
			}
		}
	}
}