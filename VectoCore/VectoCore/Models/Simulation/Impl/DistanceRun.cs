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

using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class DistanceRun : VectoRun
	{
		public DistanceRun(IVehicleContainer container) : base(container) {}

		protected override IResponse DoSimulationStep()
		{
			IterationStatistics.StartIteration();

			// estimate distance to be traveled within the next TargetTimeInterval
			var ds = Container.VehicleInfo.VehicleSpeed.IsEqual(0.KMPHtoMeterPerSecond(), 0.01.SI<MeterPerSecond>())
				? Constants.SimulationSettings.DriveOffDistance
				: VectoMath.Max(
				Constants.SimulationSettings.TargetTimeInterval * Container.VehicleInfo.VehicleSpeed,
					Constants.SimulationSettings.DriveOffDistance);

			var loopCount = 0;
			IResponse response;
			var debug = new DebugData();
			do {
				IterationStatistics.Increment(this, "Iterations");

				Container.Brakes.BrakePower = 0.SI<Watt>();
				response = CyclePort.Request(AbsTime, ds);
				switch (response) {
					case ResponseSuccess r: 
						dt = r.SimulationInterval;
						break;
					case ResponseDrivingCycleDistanceExceeded r:
						if (r.MaxDistance.IsSmallerOrEqual(0)) {
							throw new VectoSimulationException("DistanceExceeded, MaxDistance is invalid: {0}", r.MaxDistance);
						}
						ds = r.MaxDistance;
						break;
					case ResponseCycleFinished _:
						FinishedWithoutErrors = true;
						Log.Info("========= Driving Cycle Finished");
						break;
					case ResponseBatteryEmpty _:
						FinishedWithoutErrors = true;
						Log.Info("========= REESS empty");
						break;
					default:
						throw new VectoException("DistanceRun got an unexpected response: {0}", response);
				}
				if (loopCount++ > Constants.SimulationSettings.MaximumIterationCountForSimulationStep) {
					throw new VectoSimulationException("Maximum iteration count for a single simulation interval reached! Aborting!");
				}
				debug.Add($"[DR.DST-{loopCount}]", response);
			} while (!(response is ResponseSuccess || response is ResponseCycleFinished || response is ResponseBatteryEmpty));

			IterationStatistics.Increment(this, "Distance", Container.MileageCounter.Distance.Value());
			IterationStatistics.Increment(this, "Time", AbsTime.Value());
			IterationStatistics.FinishIteration(AbsTime);
			response.AbsTime = AbsTime;
			return response;
		}

		protected override IResponse Initialize()
		{
			Log.Info("Starting {0}", RunIdentifier);
			return CyclePort.Initialize();
		}
	}
}