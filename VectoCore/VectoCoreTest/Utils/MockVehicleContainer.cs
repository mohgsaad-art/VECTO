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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockVehicleContainer : IVehicleContainer, IDriverInfo, IPowertainInfo, IEngineControl
	{
		// only CycleData Lookup is set / accessed...

		public List<VectoSimulationComponent> Components = new List<VectoSimulationComponent>();
		private Watt _axlegearLoss = 0.SI<Watt>();
		private bool _clutchClosed = true;

		public IWheelsInfo WheelsInfo { get; }
		public IDriverInfo DriverInfo => this;
		public IDrivingCycleInfo DrivingCycleInfo { get { return this; } }
		public IPowertainInfo PowertrainInfo { get { return this; } }
		public IAngledriveInfo AngledriveInfo { get; }
		public bool IsTestPowertrain { get; }

		public IEngineInfo Engine { get; set; }

		public GearboxType GearboxType { get; set; }

		public GearshiftPosition Gear { get; set; }
		public bool TCLocked { get; set; }
		public GearshiftPosition NextGear { get; private set; }

		public Second TractionInterruption
		{
			get { return 1.SI<Second>(); }
		}

		public uint NumGears { get; set; }

		public MeterPerSecond StartSpeed { get; set; }
		public MeterPerSquareSecond StartAcceleration { get; set; }
		public NewtonMeter GearMaxTorque { get; set; }

		public FuelType FuelType
		{
			get { return FuelType.DieselCI; }
		}

		public Second AbsTime { get; set; }
		public IMileageCounter MileageCounter { get { return this; } }
		public IGearboxInfo GearboxInfo { get { return this; } }
		public IGearboxControl GearboxCtl { get; }
		public IAxlegearInfo AxlegearInfo { get; }
		public IEngineInfo EngineInfo { get; }
		public IEngineControl EngineCtl
		{
			get { return this; }
		}
		public IVehicleInfo VehicleInfo { get { return this; } }
		public IClutchInfo ClutchInfo { get { return this; } }
		public IBrakes Brakes { get { return this; } }

		public Watt GearboxLoss()
		{
			throw new System.NotImplementedException();
		}

		public Second LastShift { get;  set; }

		public GearData GetGearData(uint gear)
		{
			throw new System.NotImplementedException();
		}

		public PerSecond EngineSpeed { get; set; }
		public NewtonMeter EngineTorque { get; set; }

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			return Engine.EngineStationaryFullPower(angularSpeed);
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			return Engine.EngineStationaryFullPower(angularSpeed);
		}

		public PerSecond EngineIdleSpeed
		{
			get { return Engine.EngineIdleSpeed; }
		}

		public PerSecond EngineRatedSpeed
		{
			get { return Engine.EngineRatedSpeed; }
		}

		public PerSecond EngineN95hSpeed
		{
			get { return Engine.EngineN95hSpeed; }
		}

		public PerSecond EngineN80hSpeed
		{
			get { return Engine.EngineN80hSpeed; }
		}

		public MeterPerSecond VehicleSpeed { get; set; }
		public Kilogram VehicleMass { get; set; }
		public Kilogram VehicleLoading { get; set; }
		public Kilogram TotalMass { get; set; }
		public CubicMeter CargoVolume { get; set; }

		public Newton AirDragResistance(MeterPerSecond previousVelocity, MeterPerSecond nextVelocity)
		{
			return 0.SI<Newton>();
		}

		public Newton RollingResistance(Radian gradient)
		{
			return 0.SI<Newton>();
		}

		public Newton SlopeResistance(Radian gradient)
		{
			return 0.SI<Newton>();
		}

		public MeterPerSecond MaxVehicleSpeed { get { return null; } }

		public Meter Distance { get; set; }

		public bool SetClutchClosed
		{
			set { _clutchClosed = value; }
		}

		public bool ClutchClosed(Second absTime)
		{
			return _clutchClosed;
		}

		public Watt BrakePower { get; set; }
		public Radian RoadGradient { get; set; }
		public Meter CycleStartDistance { get; set; }

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			throw new System.NotImplementedException();
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			throw new System.NotImplementedException();
		}

		public bool VehicleStopped { get; set; }

		public DrivingBehavior DriverBehavior { get; set; }

		public DrivingAction DrivingAction { get; set; }

		public MeterPerSquareSecond DriverAcceleration { get; set; }

		public CycleData CycleData { get; set; }

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			return new DrivingCycleData.DrivingCycleEntry() {
				RoadGradient = 0.SI<Radian>(),
				Altitude = 0.SI<Meter>()
			};
		}

		public Meter Altitude { get; set; }
		public ExecutionMode ExecutionMode { get; set; }
		public IModalDataContainer ModalData { get; set; }
		public VectoRunData RunData { get; set; }

		public ISimulationOutPort GetCycleOutPort()
		{
			throw new System.NotImplementedException();
		}

		public VectoRun.Status RunStatus { get; set; }

		public bool PTOActive { get; private set; }

		public void AddComponent(VectoSimulationComponent component)
		{
			Components.Add(component);
		}

		public void CommitSimulationStep(Second time, Second simulationInterval)
		{
			foreach (var entry in Components) {
				entry.CommitSimulationStep(ModalData);
			}
		}

		public void FinishSimulation() {}

		public void FinishSimulationRun(Exception e) {}
		public void StartSimulationRun()
		{
			
		}

		public IEnumerable<ISimulationPreprocessor> GetPreprocessingRuns { get { return new ISimulationPreprocessor[] {};} }
		public void AddPreprocessor(ISimulationPreprocessor simulationPreprocessor)
		{
			throw new NotImplementedException();
		}

		public Watt SetAxlegearLoss
		{
			set { _axlegearLoss = value; }
		}

		public Watt AxlegearLoss()
		{
			return _axlegearLoss;
		}

		public Kilogram ReducedMassWheels { get; set; }
		public bool HasCombustionEngine { get; set; }
		public bool CombustionEngineOn { get; set; }
	}
}