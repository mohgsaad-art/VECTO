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
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
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

    public class MockVehicleContainer : IVehicleContainer, IEngineInfo, IEngineControl, IVehicleInfo, IClutchInfo, IBrakes, IAxlegearInfo, IWheelsInfo, IDriverInfo, IDrivingCycleInfo, IMileageCounter, IGearboxInfo, IGearboxControl, IPowertainInfo, IUpdateable
	{
		// only CycleData Lookup is set / accessed...

		protected List<VectoSimulationComponent> MyComponents = new List<VectoSimulationComponent>();
		private Watt _axlegearLoss = 0.SI<Watt>();
		private bool _clutchClosed = true;

		public IAxlegearInfo AxlegearInfo => this;

		public IEngineInfo EngineInfo { get; set; }

		public IEngineControl EngineCtl => this;

		public IVehicleInfo VehicleInfo => this;

		public IClutchInfo ClutchInfo => this;

		public IBrakes Brakes => this;

		public IWheelsInfo WheelsInfo => this;

		public IDriverInfo DriverInfo => this;

		public IDrivingCycleInfo DrivingCycleInfo => this;

		public GearboxType GearboxType { get; set; }

		public GearshiftPosition Gear { get; set; }
		public bool TCLocked { get; set; }
		public GearshiftPosition NextGear { get; private set; }

		public Second TractionInterruption => 1.SI<Second>();

		public uint NumGears { get; set; }
		public bool Disengaged { get; }

		public MeterPerSecond StartSpeed { get; set; }
		public MeterPerSquareSecond StartAcceleration { get; set; }
		public NewtonMeter GearMaxTorque { get; set; }

		public FuelType FuelType => FuelType.DieselCI;

		public Second AbsTime { get; set; }

		public IMileageCounter MileageCounter => this;

		public IGearboxInfo GearboxInfo => this;

		public IShiftStrategy Strategy => null;

		public event Action GearShiftTriggered;

		public IGearboxControl GearboxCtl => this;

		public IElectricMotorInfo ElectricMotorInfo(PowertrainPosition pos)
		{
			return null;
		}

		public IRESSInfo BatteryInfo
		{
			get;
			set;
		}

		public IElectricSystemInfo ElectricSystemInfo { get; }

		public ITorqueConverterInfo TorqueConverterInfo => null;

		public ITorqueConverterControl TorqueConverterCtl => null;

		public IPowertainInfo PowertrainInfo => this;

		public IHybridControllerInfo HybridControllerInfo { get; }
		public IHybridControllerCtl HybridControllerCtl { get; }
		public IAngledriveInfo AngledriveInfo { get; }
		public IDCDCConverter DCDCConverter { get; }
		public IWHRCharger WHRCharger { get; }

		public bool IsTestPowertrain => false;

		public Watt GearboxLoss()
		{
			throw new NotImplementedException();
		}

		public Second LastShift { get;  set; }
		public Second LastUpshift => null;

		public Second LastDownshift => null;

		public GearData GetGearData(uint gear)
		{
			return null;
		}

		public PerSecond EngineSpeed { get; set; }
		public NewtonMeter EngineTorque { get; set; }

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			return EngineInfo.EngineStationaryFullPower(angularSpeed);
		}

		public Watt EngineDynamicFullLoadPower(PerSecond avgEngineSpeed, Second dt)
		{
			throw new NotImplementedException();
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			return EngineInfo.EngineStationaryFullPower(angularSpeed);
		}

		public Watt EngineAuxDemand(PerSecond avgEngineSpeed, Second dt)
		{
			throw new NotImplementedException();
		}

		public PerSecond EngineIdleSpeed => EngineInfo.EngineIdleSpeed;

		public PerSecond EngineRatedSpeed => EngineInfo.EngineRatedSpeed;

		public PerSecond EngineN95hSpeed => EngineInfo.EngineN95hSpeed;

		public PerSecond EngineN80hSpeed => EngineInfo.EngineN80hSpeed;

		public bool EngineOn => EngineInfo.EngineOn;

		public MeterPerSecond VehicleSpeed { get; set; }
		public Kilogram VehicleMass { get; set; }
		public Kilogram VehicleLoading { get; set; }
		public Kilogram TotalMass { get; set; }
		public CubicMeter CargoVolume { get; set; }

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

		public Meter Distance { get; set; }

		public bool SetClutchClosed
		{
			set => _clutchClosed = value;
		}

		public bool ClutchClosed(Second absTime)
		{
			return _clutchClosed;
		}

		public Watt ClutchLosses => throw new NotImplementedException();

		public Watt BrakePower { get; set; }
		public Radian RoadGradient { get; set; }
		public MeterPerSecond TargetSpeed { get; set; }
		public Second StopTime { get; set; }
		public Meter CycleStartDistance { get; set; }

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			throw new NotImplementedException();
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			throw new NotImplementedException();
		}

		public SpeedChangeEntry LastTargetspeedChange { get; set; }

		public bool VehicleStopped { get; set; }

		public DrivingBehavior DriverBehavior { get; set; }

		public DrivingAction DrivingAction { get; set; }

		public MeterPerSquareSecond DriverAcceleration { get; set; }
		public PCCStates PCCState => PCCStates.OutsideSegment;
		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();
		public MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed) => targetSpeed;

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
			throw new NotImplementedException();
		}

		public VectoRun.Status RunStatus { get; set; }
		public ISimplePowertrainBuilder SimplePowertrainBuilder => null;
		public IAuxInProvider BusAux => null;

		public bool PTOActive { get; private set; }

		public void AddComponent(VectoSimulationComponent component)
		{
			MyComponents.Add(component);
			ModalData?.RegisterComponent(component);

			//WriteSumData?.RegisterComponent(component, RunData);
		}

		public void AddAuxiliary(string id, string columnName = null)
		{
			ModalData?.AddAuxiliary(id, columnName);
		}

		public void CommitSimulationStep(Second time, Second simulationInterval)
		{
			foreach (var entry in MyComponents) {
				entry.CommitSimulationStep(time, simulationInterval, ModalData);
			}
		}

		public void FinishSimulation() {}

		public void FinishSimulationRun(Exception e) {}
		
		public void StartSimulationRun() { }

		public Watt SetAxlegearLoss
		{
			set => _axlegearLoss = value;
		}

		public Watt AxlegearLoss()
		{
			return _axlegearLoss;
		}

		public Tuple<PerSecond, NewtonMeter> CurrentAxleDemand { get; }
		public double Ratio { get; }

		public Kilogram ReducedMassWheels { get; set; }
		public Meter DynamicTyreRadius { get; }

		#region Implementation of IEngineControl

		public bool CombustionEngineOn { get; set; }

		#endregion

		#region Implementation of IGearboxControl

		public bool DisengageGearbox { get; set; }
		public void TriggerGearshift(Second absTime, Second dt)
		{
			throw new NotImplementedException();
		}

		public bool GearEngaged(Second absTime)
		{
			return ClutchClosed(absTime);
		}

		public bool RequestAfterGearshift { get; set; }

		#endregion

		public IEnumerable<ISimulationPreprocessor> GetPreprocessingRuns { get { return new ISimulationPreprocessor[] { }; } }
		public ISumData SumData { get; }

		public void AddPreprocessor(ISimulationPreprocessor simulationPreprocessor)
		{
			throw new NotImplementedException();
		}

		public IReadOnlyList<VectoSimulationComponent> Components => MyComponents;

		public void ResetComponents()
		{
			throw new NotImplementedException();
		}

		public void FinishSingleSimulationRun(Exception e = null)
		{
			throw new NotImplementedException();
		}


		#region Implementation of IPowertainInfo

		public bool HasCombustionEngine
		{
			get;
			set;
		}

		public bool HasGearbox
		{
			get;
			set;
		}

		public bool HasElectricMotor
		{
			get; set;
		}
		public PowertrainPosition[] ElectricMotorPositions { get; set; }
		public VectoSimulationJobType VehicleArchitecutre { get; }

		#endregion

		#region Implementation of IUpdateable

		public bool UpdateFrom(object other) {
			return false;
		}

		#endregion

		#region Implementation of ITnInProvider

		public ITnInPort InPort()
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of ITnOutProvider

		public ITnOutPort OutPort()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}