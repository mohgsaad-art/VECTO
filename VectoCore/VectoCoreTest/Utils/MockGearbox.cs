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
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils
{
    public class MockGearbox : VectoSimulationComponent, IGearbox, ITnInPort, ITnOutPort, IClutchInfo
	{
		private ITnOutPort _outPort;
		private bool _clutchClosed;

		public event Action GearShiftTriggered;

		public MockGearbox(IVehicleContainer cockpit) : base(cockpit)
		{
			_clutchClosed = true;
		}

		public ITnInPort InPort()
		{
			return this;
		}

		public ITnOutPort OutPort()
		{
			return this;
		}

		public GearboxType GearboxType { get; set; }
		public GearshiftPosition Gear { get; set; }
		public bool TCLocked { get; set; }
		public GearshiftPosition NextGear { get; private set; }

		public Second TractionInterruption => 1.SI<Second>();

		public uint NumGears { get; set; }
		public bool Disengaged { get; }

		public MeterPerSecond StartSpeed => 2.SI<MeterPerSecond>();

		public MeterPerSquareSecond StartAcceleration => 0.6.SI<MeterPerSquareSecond>();

		public NewtonMeter GearMaxTorque => null;

		public IShiftStrategy Strategy => null;

		public Watt GearboxLoss()
		{
			return 0.SI<Watt>();
		}

		public Second LastShift { get; private set; }

		public Second LastUpshift => throw new NotImplementedException();

		public Second LastDownshift => throw new NotImplementedException();

		public GearData GetGearData(uint gear)
		{
			return new GearData() { Ratio = 1.0 };
		}

		public void Connect(ITnOutPort other)
		{
			_outPort = other;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			if (_outPort != null) {
				//if (Gear > 0) {
				return _outPort.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
				//}
				//return _outPort.Request(absTime, dt, 0.SI<NewtonMeter>(), 0.RPMtoRad(), dryRun);
			}
			throw new NotImplementedException();
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (_outPort != null) {
				return _outPort.Initialize(outTorque, outAngularVelocity);
			}
			throw new NotImplementedException();
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			// nothing to write
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval) {}

		public bool GearEngaged(Second absTime)
		{
			return _clutchClosed;
		}

		public bool RequestAfterGearshift { get; set; }

		public bool ClutchClosed(Second absTime)
		{
			return _clutchClosed;
		}

		public Watt ClutchLosses => throw new NotImplementedException();

		public void SetClutch(bool closed)
		{
			_clutchClosed = closed;
		}

		public void Connect(IAuxPort aux)
		{
			throw new NotImplementedException();
		}


		public bool DisengageGearbox { get; set; }
		public void TriggerGearshift(Second absTime, Second dt)
		{
			throw new NotImplementedException();
		}

		protected override bool DoUpdateFrom(object other) => false;

	}

	public class MockAxlegear : VectoSimulationComponent, IAxlegear
	{
		public MockAxlegear(IVehicleContainer vehicleContainer) : base(vehicleContainer)
		{
			
		}

		public ITnInPort InPort()
		{
			throw new NotImplementedException();
		}

		public ITnOutPort OutPort()
		{
			throw new NotImplementedException();
		}

		public Watt AxlegearLoss()
		{
			throw new NotImplementedException();
		}

		public Tuple<PerSecond, NewtonMeter> CurrentAxleDemand { get; }
		public double Ratio => 1;

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		protected override bool DoUpdateFrom(object other) => false;

	}
}