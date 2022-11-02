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
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class TimeRun : VectoRun
	{
		public TimeRun(IVehicleContainer container) : base(container) { }

		protected override IResponse DoSimulationStep()
		{
			dt = Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval;

			var debug = new DebugData();
			var loopCount = 0;
			IResponse response;
			do {
				response = CyclePort.Request(AbsTime, dt);
				debug.Add("[TR.DST]", response);
				switch (response) {
					case ResponseSuccess r:
						dt = r.SimulationInterval;
						break;
					case ResponseFailTimeInterval r:
						dt = r.DeltaT;
						break;
					case ResponseCycleFinished r:
						FinishedWithoutErrors = true;
						Log.Info("========= Driving Cycle Finished");
						break;
					default:
						throw new VectoException("TimeRun got an unexpected response: {0}", response);
				}
				if (loopCount++ > Constants.SimulationSettings.MaximumIterationCountForSimulationStep) {
					throw new VectoSimulationException("Maximum iteration count for a single simulation interval reached! Aborting!");
				}
			} while (!(response is ResponseSuccess || response is ResponseCycleFinished));

			return response;
		}

		protected override IResponse Initialize()
		{
			Log.Info("Starting {0}", RunIdentifier);
			var response = CyclePort.Initialize();
			AbsTime = response.AbsTime;
			return response;
		}
	}
}