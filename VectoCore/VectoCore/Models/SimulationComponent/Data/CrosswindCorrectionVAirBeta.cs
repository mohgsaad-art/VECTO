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
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	internal class CrosswindCorrectionVAirBeta : LoggingObject, ICrossWindCorrection
	{
		public SquareMeter AirDragArea { get; protected set; }

		protected List<CrossWindCorrectionCurveReader.AirDragBetaEntry> AirDragEntries;
		protected IDataBus DataBus;

		public CrosswindCorrectionVAirBeta(SquareMeter airDragArea,
			List<CrossWindCorrectionCurveReader.AirDragBetaEntry> entries)
		{
			AirDragArea = airDragArea;
			AirDragEntries = entries;
		}

		public void SetDataBus(IDataBus dataBus)
		{
			DataBus = dataBus;
		}

		public CrossWindCorrectionMode CorrectionMode => CrossWindCorrectionMode.VAirBetaLookupTable;

		public Watt AverageAirDragPowerLoss(MeterPerSecond v1, MeterPerSecond v2, KilogramPerCubicMeter airDensity)
		{
			if (DataBus == null) {
				throw new VectoException("Databus is not set - can't access vAir, beta!");
			}
			var vAir = DataBus.DrivingCycleInfo.CycleData.LeftSample.AirSpeedRelativeToVehicle;
			var beta = DataBus.DrivingCycleInfo.CycleData.LeftSample.WindYawAngle;

			// F_air(t) = k * CdA_korr * v_air^2   // assumption: v_air = const for the current interval
			// P(t) = F_air(t) * v(t) , v(t) = v1 + a * t
			// P_avg = 1/T * Integral P(t) dt
			// P_avg = k * CdA_korr * v_air^2 * (v1 + v2) / 2
			var airDragForce = (AirDragArea + DeltaCdxA(Math.Abs(beta))) * Physics.AirDensity / 2.0 * vAir * vAir;
			var vAverage = (v1 + v2) / 2;

			return (airDragForce * vAverage).Cast<Watt>();
		}

		protected SquareMeter DeltaCdxA(double beta)
		{
			var (first, second) = FindIndex(beta);
            return VectoMath.Interpolate(first.Beta, second.Beta, first.DeltaCdA,
                second.DeltaCdA, beta);
		}

		protected (CrossWindCorrectionCurveReader.AirDragBetaEntry, CrossWindCorrectionCurveReader.AirDragBetaEntry) FindIndex(double beta)
		{
			if (beta < AirDragEntries.First().Beta) {
				throw new VectoSimulationException("Beta / CdxA Lookup table does not cover beta={0}", beta);
			}
			if (beta > AirDragEntries.Last().Beta) {
				throw new VectoSimulationException("Beta / CdxA Lookup table does not cover beta={0}", beta);
			}

			return AirDragEntries.GetSection(x => x.Beta < beta);
			
		}
	}
}