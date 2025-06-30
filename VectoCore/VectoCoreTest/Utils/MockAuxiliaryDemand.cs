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
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils
{
    public class MockDrivingCycle : VectoSimulationComponent, IDrivingCycleInfo
	{
		private readonly IEnumerator<DrivingCycleData.DrivingCycleEntry> _left;
		private readonly IEnumerator<DrivingCycleData.DrivingCycleEntry> _right;

		public MockDrivingCycle(IVehicleContainer container, DrivingCycleData data) : base(container)
		{
			if (data != null) {
				_left = data.Entries.GetEnumerator();
				_right = data.Entries.GetEnumerator();
			} else {
				_left = Enumerable.Empty<DrivingCycleData.DrivingCycleEntry>().GetEnumerator();
				_right = Enumerable.Empty<DrivingCycleData.DrivingCycleEntry>().GetEnumerator();
			}
			_left.MoveNext();
			_right.MoveNext();
			_right.MoveNext();
		}


		public CycleData CycleData =>
			new CycleData {
				AbsTime = 0.SI<Second>(),
				AbsDistance = 0.SI<Meter>(),
				LeftSample = _left.Current,
				RightSample = _right.Current
			};

		public bool PTOActive { get; set; }

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			return new DrivingCycleData.DrivingCycleEntry() {
				RoadGradient = 0.SI<Radian>(),
				Altitude = 0.SI<Meter>()
			};
		}

		public Meter Altitude => 0.SI<Meter>();

		public Radian RoadGradient => 0.SI<Radian>();
		public MeterPerSecond TargetSpeed { get; set; }
		public Second StopTime { get; set; }

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.dist] = 0.SI<Meter>();
			container[ModalResultField.v_targ] = 0.KMPHtoMeterPerSecond();
			container[ModalResultField.grad] = 0.SI<Scalar>();
			container[ModalResultField.altitude] = 0.SI<Meter>();
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			_left.MoveNext();
			_right.MoveNext();
		}

		public Meter CycleStartDistance => 0.SI<Meter>();

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			return new List<DrivingCycleData.DrivingCycleEntry>();
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			return new List<DrivingCycleData.DrivingCycleEntry>();
		}

		public SpeedChangeEntry LastTargetspeedChange { get; set; }

		public void FinishSimulation() {}

		protected override bool DoUpdateFrom(object other) => false;

	}
}