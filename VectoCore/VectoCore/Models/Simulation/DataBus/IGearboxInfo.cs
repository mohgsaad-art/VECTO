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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.Simulation.DataBus
{
	/// <summary>
	/// Defines a method to access shared data of the gearbox.
	/// </summary>
	public interface IGearboxInfo
	{
		GearboxType GearboxType { get; }

		/// <summary>
		/// Returns the current gear.
		/// </summary>
		/// <returns></returns>
		GearshiftPosition Gear { get; }

		bool TCLocked { get; }

		//MeterPerSecond StartSpeed { get; }

		//MeterPerSquareSecond StartAcceleration { get; }

		Watt GearboxLoss();

		Second LastShift { get; }

		Second LastUpshift { get; }

		Second LastDownshift { get; }

		GearData GetGearData(uint gear);

		/// <summary>
		/// Returns the next gear during shifting operations
		/// </summary>
		GearshiftPosition NextGear { get; }

		Second TractionInterruption { get; }
		uint NumGears { get; }

		bool Disengaged { get; }

		bool DisengageGearbox { get; }

		bool GearEngaged(Second absTime);

		bool RequestAfterGearshift { get; set; }

		IShiftStrategy Strategy { get; }
	}

	public interface IGearboxControl
	{
		bool DisengageGearbox { set; }
		void TriggerGearshift(Second absTime, Second dt);

		event Action GearShiftTriggered;
	}

	public interface ITorqueConverterInfo
	{
		Tuple<TorqueConverterOperatingPoint, NewtonMeter> CalculateOperatingPoint(PerSecond inSpeed, PerSecond outSpeed);

	}

	public interface ITorqueConverterControl
	{
		
		TorqueConverterOperatingPoint SetOperatingPoint { get; set; }
	}
}