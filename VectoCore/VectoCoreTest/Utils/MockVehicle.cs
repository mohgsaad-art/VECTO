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
using System.Reflection.Metadata.Ecma335;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils
{
    public class MockVehicle : VectoSimulationComponent, IVehicle, IFvInPort, IDriverDemandOutPort, IMileageCounter
	{
		internal MeterPerSecond MyVehicleSpeed;
		internal IFvOutPort NextComponent;

		internal RequestData LastRequest = new RequestData();

		public MockVehicle(IVehicleContainer cockpit) : base(cockpit)
		{
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container) {}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval) {}

		public IFvInPort InPort()
		{
			return this;
		}

		public IDriverDemandOutPort OutPort()
		{
			return this;
		}

		public MeterPerSecond VehicleSpeed => MyVehicleSpeed;

		public bool VehicleStopped => MyVehicleSpeed.IsEqual(0.SI<MeterPerSecond>(), 0.01.SI<MeterPerSecond>());

		public Kilogram VehicleMass => 7500.SI<Kilogram>();

		public Kilogram VehicleLoading => 0.SI<Kilogram>();

		public Kilogram TotalMass => VehicleMass;

		public CubicMeter CargoVolume { get;  set; }

		public AirDragLossResult AirDragResistance(MeterPerSecond previousVelocity, MeterPerSecond nextVelocity)
		{
			return new AirDragLossResult(0.SI<Watt>(), 0.SI<SquareMeter>(), (previousVelocity + nextVelocity) / 2.0);
		}

		public Newton RollingResistance(Radian gradient)
		{
			return 0.SI<Newton>();
		}

		public Newton SlopeResistance(Radian gradient)
		{
			return 0.SI<Newton>();
		}

		public MeterPerSecond MaxVehicleSpeed => null;

		public void Connect(IFvOutPort other)
		{
			NextComponent = other;
		}

		public IResponse Request(Second absTime, Second dt, MeterPerSquareSecond acceleration, Radian gradient, bool b)
		{
			LastRequest = new RequestData {
				abstime = absTime,
				dt = dt,
				acceleration = acceleration,
				gradient = gradient
			};
			return new ResponseSuccess(this);
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient)
		{
			throw new NotImplementedException();
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient, MeterPerSquareSecond startAcceleration)
		{
			throw new NotImplementedException();
		}

		public class RequestData
		{
			public Second abstime;
			public Second dt;
			public MeterPerSquareSecond acceleration;
			public Radian gradient;
		}

		public Meter Distance => 0.SI<Meter>();

		protected override bool DoUpdateFrom(object other) => false;

	}
}