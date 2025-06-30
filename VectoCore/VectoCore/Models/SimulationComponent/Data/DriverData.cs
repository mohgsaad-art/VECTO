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
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class DriverData
	{
		[Required, ValidateObject] public OverSpeedData OverSpeed;

		[Required, ValidateObject] public LACData LookAheadCoasting;

		[Required, ValidateObject] public AccelerationCurveData AccelerationCurve;

		[Required, ValidateObject] public EngineStopStartData EngineStopStart;

		[Required, ValidateObject] public EcoRollData EcoRoll;
		public MeterPerSecond PTODriveMinSpeed { get; set; }
		public GearshiftPosition PTODriveRoadsweepingGear { get; set; }


		public PCCData PCC;

		public static bool ParseDriverMode(string mode)
		{
			if (mode == null) {
				return false;
			}
			var txt = mode.Replace("-", "");
			if (txt.Equals("Off", StringComparison.InvariantCultureIgnoreCase)) {
				return false;
			}

			if (txt.Equals("Overspeed", StringComparison.InvariantCultureIgnoreCase)) {
				return true;
			}

			if (txt.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase)) {
				return true;
			}

			return false;
		}

		public class OverSpeedData
		{
			public bool Enabled;

			[Required, SIRange(0, 120 / Constants.MeterPerSecondToKMH)] public MeterPerSecond MinSpeed;

			[Required, SIRange(0, 50 / Constants.MeterPerSecondToKMH)] public MeterPerSecond OverSpeed;
		}

		public class EcoRollData
		{
			public MeterPerSecond MinSpeed;

			public MeterPerSquareSecond AccelerationLowerLimit;

			public MeterPerSquareSecond AccelerationUpperLimit;

			public MeterPerSecond UnderspeedThreshold;

			public Second ActivationPhaseDuration;
		}

		public class LACData
		{
			public bool Enabled;

			//public MeterPerSquareSecond Deceleration;

			[Required, SIRange(0, 120 / Constants.MeterPerSecondToKMH)] public MeterPerSecond MinSpeed;

			[Required, Range(0, 20)] public double LookAheadDistanceFactor;

			[Required, ValidateObject] public LACDecisionFactor LookAheadDecisionFactor;
		}

		public class EngineStopStartData
		{
			[Required, SIRange(0, Double.MaxValue)] public Second EngineOffStandStillActivationDelay;

			[Required, SIRange(0, double.MaxValue)] public Second MaxEngineOffTimespan;

			[Required, Range(0.0, 1.0)] public double UtilityFactorStandstill;

			[Required, Range(0.0, 1.0)] public double UtilityFactorDriving;
		}

		public class PCCData
		{
			public MeterPerSecond MinSpeed;

			public Meter PreviewDistanceUseCase1;

			public Meter PreviewDistanceUseCase2;

			public MeterPerSecond UnderSpeed;

			public MeterPerSecond PCCEnableSpeed;

			public MeterPerSecond OverspeedUseCase3;
		}
	}
}