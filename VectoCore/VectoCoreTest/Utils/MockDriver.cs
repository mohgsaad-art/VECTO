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

using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils
{
    public class MockDriver : VectoSimulationComponent, IDriver, IDrivingCycleOutPort, IDriverDemandInPort, IDriverInfo
	{
		private IDriverDemandOutPort _next;

		public RequestData LastRequest;

		public MockDriver(IVehicleContainer container) : base(container)
		{
			DriverBehavior = DrivingBehavior.Accelerating;
			DrivingAction = DrivingAction.Accelerate;
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container) {}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval) {}

		public IDrivingCycleOutPort OutPort()
		{
			return this;
		}

		public IDriverDemandInPort InPort()
		{
			return this;
		}

		public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			LastRequest = new RequestData { AbsTime = absTime, ds = ds, Gradient = gradient, TargetVelocity = targetVelocity };
			var acc = 0.SI<MeterPerSquareSecond>();
			var dt = 1.SI<Second>();
			return new ResponseSuccess(this) { SimulationInterval = dt, SimulationDistance = ds, };
		}

		public IResponse Request(Second absTime, Second dt, MeterPerSecond targetVelocity, Radian gradient)
		{
			LastRequest = new RequestData { AbsTime = absTime, dt = dt, Gradient = gradient, TargetVelocity = targetVelocity };
			var acc = 0.SI<MeterPerSquareSecond>();
			return new ResponseSuccess(this) { SimulationInterval = dt, };
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient)
		{
			if (_next != null) {
				return _next.Initialize(vehicleSpeed, roadGradient);
			}

			return new ResponseSuccess(this);
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient, MeterPerSquareSecond startAcceleration)
		{
			if (_next != null) {
				return _next.Initialize(vehicleSpeed, roadGradient, startAcceleration);
			}

			return new ResponseSuccess(this);
		}

		public void Connect(IDriverDemandOutPort other)
		{
			_next = other;
		}

		public class RequestData
		{
			public Second AbsTime;
			public Meter ds;
			public Second dt;
			public MeterPerSecond TargetVelocity;
			public Radian Gradient;
		}

		//public MeterPerSquareSecond DriverAcceleration { get; set; }

		public bool VehicleStopped { get; set; }

		public DrivingBehavior DriverBehavior { get; set; }

		public DrivingAction DrivingAction { get; set; }

		public MeterPerSquareSecond DriverAcceleration { get; set; }
		public PCCStates PCCState => PCCStates.OutsideSegment;
		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();
		public MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed) => targetSpeed;

		protected override bool DoUpdateFrom(object other) => false;

	}
}