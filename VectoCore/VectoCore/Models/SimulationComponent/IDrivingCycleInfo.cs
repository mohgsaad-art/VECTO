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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	/// <summary>
	/// Defines a method to access shared data of the driving cycle.
	/// </summary>
	/// 
	public interface IDrivingCycleInfo
	{
		/// <summary>
		/// Returns the data samples for the current position in the cycle.
		/// </summary>
		CycleData CycleData { get; }

		bool PTOActive { get; }

		/// <summary>
		/// get a single driving-cycle entry at a certain distance ahead
		/// altitude is interpolated between sampling points, slope is averaged
		/// </summary>
		/// <param name="distance"></param>
		/// <returns></returns>
		DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance);

		Meter Altitude { get; }

		Radian RoadGradient { get; }

		MeterPerSecond TargetSpeed { get; }

		Second StopTime { get; }

		Meter CycleStartDistance { get; }

		IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance);

		IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time);

		SpeedChangeEntry LastTargetspeedChange { get; }

		void FinishSimulation();
	}

	public class SpeedChangeEntry
	{
		public MeterPerSecond PreviousTargetSpeed;
		public MeterPerSecond NewTargetSpeed;
		public Meter Distance;
		public Second AbsTime;
		public Meter Altitude;

	}
}