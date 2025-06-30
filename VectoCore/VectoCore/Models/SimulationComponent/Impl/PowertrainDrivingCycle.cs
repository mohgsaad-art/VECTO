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
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public interface IPowertrainDrivingCycle : ITnInProvider
	{

	}

    /// <summary>
    /// Represents a driving cycle which directly is connected to the powertrain (e.g. engine, or axle gear).
    /// </summary>
    public class PowertrainDrivingCycle :
		StatefulProviderComponent<SimpleComponentState, ISimulationOutPort, ITnInPort, ITnOutPort>,
		IDrivingCycleInfo, ISimulationOutPort, ITnInProvider, ITnInPort, IPowertrainDrivingCycle
	{
		internal readonly IDrivingCycleData Data;
		protected internal readonly DrivingCycleEnumerator CycleIterator;

		protected Second AbsTime
		{
			get; 
			set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PowertrainDrivingCycle"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="cycle">The cycle.</param>
		public PowertrainDrivingCycle(IVehicleContainer container, IDrivingCycleData cycle) : base(container)
		{
			Data = cycle;
			CycleIterator = new DrivingCycleEnumerator(Data);

			AbsTime = -1.SI<Second>();
			
		}

		public virtual IResponse Initialize()
		{
			var first = Data.Entries[0];
			if (AbsTime < 0) {
				AbsTime = first.Time;
				CycleIterator.MoveNext();
			}

			var response = NextComponent.Initialize(first.Torque, first.AngularVelocity);
			response.AbsTime = AbsTime;
			return response;
		}

		#region ISimulationOutPort

		public IResponse Request(Second absTime, Meter ds)
		{
			throw new VectoSimulationException("Powertrain Only Simulation can not handle distance request.");
		}

		public virtual IResponse Request(Second absTime, Second dt)
		{
			// cycle finished (no more entries in cycle)
			if (CycleIterator.LastEntry && CycleIterator.RightSample.Time == absTime) {
				return new ResponseCycleFinished(this);
			}

			// interval exceeded
			if (CycleIterator.RightSample != null && (absTime + dt).IsGreater(CycleIterator.RightSample.Time)) {
				return new ResponseFailTimeInterval(this) {
					AbsTime = absTime,
					DeltaT = CycleIterator.RightSample.Time - absTime
				};
			}

			return DoHandleRequest(absTime, dt, CycleIterator.LeftSample.AngularVelocity);
		}

		protected IResponse DoHandleRequest(Second absTime, Second dt, PerSecond angularVelocity)
		{
			var debug = new DebugData();

			IResponse response;
			var responseCount = 0;
			do {
				response = NextComponent.Request(absTime, dt, CycleIterator.LeftSample.Torque, angularVelocity, false);
				CurrentState.InAngularVelocity = angularVelocity;
				CurrentState.InTorque = CycleIterator.LeftSample.Torque;
				
				debug.Add("PDC.DHR-0", response);
				switch (response) {
					case ResponseGearShift _:
						response = NextComponent.Request(absTime, dt, CurrentState.InTorque, angularVelocity, false);
						break;
					case ResponseUnderload r:
						var torqueInterval = -r.Delta / (angularVelocity.IsEqual(0) ? 10.RPMtoRad() : angularVelocity);
						var torque = SearchAlgorithm.Search(CycleIterator.LeftSample.Torque, r.Delta, torqueInterval,
							getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
							evaluateFunction: t => NextComponent.Request(absTime, dt, t, angularVelocity, true),
							criterion: y => ((ResponseDryRun)y).DeltaDragLoad.Value(),
							searcher: this);
						response = NextComponent.Request(absTime, dt, torque, angularVelocity, false);
						CurrentState.InTorque = torque;
						break;
					case ResponseOverload r:
						var torque2 = SearchAlgorithm.Search(CycleIterator.LeftSample.Torque, r.Delta, 50.SI<NewtonMeter>(),
							getYValue: result => ((ResponseDryRun)result).DeltaFullLoad,
							evaluateFunction: t => NextComponent.Request(absTime, dt, t, angularVelocity, true),
							criterion: y => ((ResponseDryRun)y).DeltaFullLoad.Value(),
							searcher: this);
						response = NextComponent.Request(absTime, dt, torque2, angularVelocity, false);
						CurrentState.InAngularVelocity = angularVelocity;
						CurrentState.InTorque = torque2;
						break;
					case ResponseEngineSpeedTooHigh r:
						angularVelocity = SearchAlgorithm.Search(angularVelocity, r.DeltaEngineSpeed,
							1.RPMtoRad(),
							getYValue: result => ((ResponseDryRun)result).DeltaEngineSpeed,
							evaluateFunction: x => NextComponent.Request(absTime, dt, CurrentState.InTorque, x, true),
							criterion: y => ((ResponseDryRun)y).DeltaEngineSpeed.Value(),
							searcher: this);
						break;
					case ResponseFailTimeInterval r:
						dt = r.DeltaT;
						break;
					case ResponseSuccess _:
						break;
					default:
						throw new UnexpectedResponseException("PowertrainDrivingCycle received an unexpected response.", response);
				}
			} while (!(response is ResponseSuccess || response is ResponseFailTimeInterval) && (++responseCount < 10));

			AbsTime = absTime + dt;
			response.SimulationInterval = dt;
			debug.Add("PDC.DHR-1", response);
			return response;
		}

		public double Progress => Math.Max(0, AbsTime.Value() / Data.Entries.Last().Time.Value());

		#endregion

		#region VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{ }

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			CycleIterator.MoveNext();
			
			AdvanceState();
		}

		protected override bool DoUpdateFrom(object other) => false;

		#endregion

		public CycleData CycleData =>
			new CycleData {
				AbsTime = CycleIterator.LeftSample.Time,
				AbsDistance = null,
				LeftSample = CycleIterator.LeftSample,
				RightSample = CycleIterator.RightSample,
			};

		public bool PTOActive => true;

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			return new DrivingCycleData.DrivingCycleEntry {
				Altitude = 0.SI<Meter>()
			};
		}

		public Meter Altitude => 0.SI<Meter>();

		public Radian RoadGradient => 0.SI<Radian>();

		public MeterPerSecond TargetSpeed => throw new NotImplementedException("Targetspeed in Powertrain not available?");

		public Second StopTime => CycleIterator.LeftSample.StoppingTime;

		public Meter CycleStartDistance => 0.SI<Meter>();

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance) => throw new NotImplementedException();

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			var retVal = new List<DrivingCycleData.DrivingCycleEntry>();

			var iterator = CycleIterator.Clone();
			do {
				retVal.Add(iterator.RightSample);
			} while (iterator.MoveNext() && iterator.RightSample.Time < AbsTime + time);

			return retVal;
		}

		public SpeedChangeEntry LastTargetspeedChange => null;

		public void FinishSimulation() => Data.Finish();
	}
}