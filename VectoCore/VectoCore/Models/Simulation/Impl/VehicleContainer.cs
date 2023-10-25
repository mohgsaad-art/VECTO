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
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public sealed class VehicleContainer : LoggingObject, IVehicleContainer
	{
		private List<Tuple<int, VectoSimulationComponent>> _components =
			new List<Tuple<int, VectoSimulationComponent>>();

		internal IEngineInfo Engine;
		internal IGearboxInfo Gearbox;
		internal IAxlegearInfo Axlegear;
		internal IVehicleInfo Vehicle;
		internal IBrakes Brakes;
		internal IWheelsInfo Wheels;
		internal IDriverInfo Driver;

		internal IMileageCounter MilageCounter;

		internal IClutchInfo Clutch;

		internal IDrivingCycleInfo DrivingCycle;

		internal ISimulationOutPort Cycle;

		internal IModalDataContainer ModData;

		internal WriteSumData WriteSumData;

		#region IGearCockpit

		public GearboxType GearboxType
		{
			get { return Gearbox == null ? GearboxType.MT : Gearbox.GearboxType; }
		}

		public uint Gear
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				if (Gearbox == null) {
					return 0; // throw new VectoException("no gearbox available!");
				}
				return Gearbox.Gear;
			}
		}

		public bool TCLocked
		{
			get {
				if (Gearbox == null) {
					return true;
				}
				return  Gearbox.TCLocked;
			}
		}

		public MeterPerSecond StartSpeed
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				if (Gearbox == null) {
					throw new VectoException("No Gearbox available. StartSpeed unkown");
				}
				return Gearbox.StartSpeed;
			}
		}

		public MeterPerSquareSecond StartAcceleration
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				if (Gearbox == null) {
					throw new VectoException("No Gearbox available. StartAcceleration unknown.");
				}
				return Gearbox.StartAcceleration;
			}
		}

		public Watt GearboxLoss()
		{
			return Gearbox.GearboxLoss();
		}

		public Second LastShift
		{
			get { return Gearbox.LastShift; }
		}

		public GearData GetGearData(uint gear)
		{
			return Gearbox.GetGearData(gear);
		}

		public GearInfo NextGear
		{
			get { return Gearbox.NextGear; }
		}

		public Second TractionInterruption
		{
			get { return Gearbox.TractionInterruption; }
		}

		public uint NumGears
		{
			get { return Gearbox.NumGears; }
		}

		#endregion

		#region IEngineCockpit

		public PerSecond EngineSpeed
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				if (Engine == null) {
					throw new VectoException("no engine available!");
				}
				return Engine.EngineSpeed;
			}
		}

		public NewtonMeter EngineTorque
		{
			get { return Engine.EngineTorque; }
		}

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			return Engine.EngineStationaryFullPower(angularSpeed);
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			return Engine.EngineDragPower(angularSpeed);
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

		public Watt PreviousStateEnginePower
		{
			get { return Engine.PreviousStateEnginePower; }
		}

		public NewtonMeter PreviousStateEngineTorque
		{
			get { return Engine.PreviousStateEngineTorque; }
		}
		public NewtonMeter PreviousStateEngineTorqueOut
		{
			get { return Engine.PreviousStateEngineTorqueOut; }
		}

		public NewtonMeter PreviousStateFullDragTorque
		{
			get { return Engine.PreviousStateFullDragTorque; }
		}

		public NewtonMeter PreviousStateInertiaTorqueLoss
		{
			get { return Engine.PreviousStateInertiaTorqueLoss; }
		}

		public SimulationComponent.Impl.CombustionEngine.EngineOperationMode PreviousStateOperationMode
		{
			get { return Engine.PreviousStateOperationMode; }
		}

		public NewtonMeter PreviousStateStationaryFullLoadTorque
		{
			get { return Engine.PreviousStateStationaryFullLoadTorque; }
		}

		public Second PreviousStateDt
		{
			get { return Engine.PreviousStateDt; }
		}

		#endregion

		#region IVehicleCockpit

		public MeterPerSecond VehicleSpeed
		{
			get { return Vehicle != null ? Vehicle.VehicleSpeed : 0.SI<MeterPerSecond>(); }
		}

		public Kilogram VehicleMass
		{
			get { return Vehicle != null ? Vehicle.VehicleMass : 0.SI<Kilogram>(); }
		}

		public Kilogram VehicleLoading
		{
			get { return Vehicle != null ? Vehicle.VehicleLoading : 0.SI<Kilogram>(); }
		}

		public Kilogram TotalMass
		{
			get { return Vehicle != null ? Vehicle.TotalMass : 0.SI<Kilogram>(); }
		}

		public CubicMeter CargoVolume
		{
			get { return Vehicle != null ? Vehicle.CargoVolume : 0.SI<CubicMeter>(); }
		}

		public Newton AirDragResistance(MeterPerSecond previousVelocity, MeterPerSecond nextVelocity)
		{
			return Vehicle.AirDragResistance(previousVelocity, nextVelocity);
		}

		public Newton RollingResistance(Radian gradient)
		{
			return Vehicle.RollingResistance(gradient);
		}

		public Newton SlopeResistance(Radian gradient)
		{
			return Vehicle.SlopeResistance(gradient);
		}

		public MeterPerSecond MaxVehicleSpeed
		{
			get { return Vehicle.MaxVehicleSpeed; }
		}

		#endregion

		public VehicleContainer(ExecutionMode executionMode, IModalDataContainer modData = null,
			WriteSumData writeSumData = null)
		{
			ModData = modData;
			WriteSumData = writeSumData ?? delegate { };
			ExecutionMode = executionMode;
		}

		#region IVehicleContainer

		public IModalDataContainer ModalData
		{
			get { return ModData; }
		}

		public ISimulationOutPort GetCycleOutPort()
		{
			return Cycle;
		}

		public FuelType FuelType
		{
			get { return ModData.FuelData.FuelType; }
		}

		public Second AbsTime { get; set; }

		public void AddComponent(VectoSimulationComponent component)
		{
			var commitPriority = 0;

			component.Switch()
				.If<IEngineInfo>(c => {
					Engine = c;
					commitPriority = 2;
				})
				.If<IDriverInfo>(c => Driver = c)
				.If<IGearboxInfo>(c => {
					Gearbox = c;
					commitPriority = 4;
				})
				.If<IAxlegearInfo>(c => Axlegear = c)
				.If<IWheelsInfo>(c => Wheels = c)
				.If<IVehicleInfo>(c => {
					Vehicle = c;
					commitPriority = 5;
				})
				.If<ISimulationOutPort>(c => Cycle = c)
				.If<IMileageCounter>(c => MilageCounter = c)
				.If<IBrakes>(c => Brakes = c)
				.If<IClutchInfo>(c => Clutch = c)
				.If<IDrivingCycleInfo>(c => {
					DrivingCycle = c;
					commitPriority = 6;
				})
				.If<PTOCycleController>(c => { commitPriority = 99; })
				.If<VTPCycle>(_ => { commitPriority = 0; });

			_components.Add(Tuple.Create(commitPriority, component));
			_components = _components.OrderBy(x => x.Item1).Reverse().ToList();
		}

		public void CommitSimulationStep(Second time, Second simulationInterval)
		{
			Log.Info("VehicleContainer committing simulation. time: {0}, dist: {1}, speed: {2}", time,
				Distance, VehicleSpeed);


			foreach (var component in _components) {
				component.Item2.CommitSimulationStep(ModData);
			}

			if (ModData != null) {
				ModData[ModalResultField.drivingBehavior] = DriverBehavior;
				ModData[ModalResultField.time] = time + simulationInterval / 2;
				ModData[ModalResultField.simulationInterval] = simulationInterval;
				ModData.CommitSimulationStep();
			}
		}

		public void FinishSimulationRun(Exception e = null)
		{
			Log.Info("VehicleContainer finishing simulation.");
			ModData?.Finish(RunStatus, e);

			WriteSumData(ModData);

			ModData?.FinishSimulation();
			DrivingCycle?.FinishSimulation();
		}

		public void FinishSimulation()
		{
			throw new NotImplementedException();
		}

		public VectoRun.Status RunStatus { get; set; }

		#endregion

		public IReadOnlyCollection<VectoSimulationComponent> SimulationComponents()
		{
			return new ReadOnlyCollection<VectoSimulationComponent>(_components.Select(x => x.Item2).ToList());
		}

		public Meter Distance
		{
			get {
				if (RunData == null || (RunData.SimulationType & SimulationType.DistanceCycle) == 0 ) {
					return 0.SI<Meter>();
				}
				if (MilageCounter != null) {
					return MilageCounter.Distance;
				}

				Log.Warn("No MileageCounter in VehicleContainer. Distance cannot be measured.");
				return 0.SI<Meter>();
			}
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			return DrivingCycle.LookAhead(lookaheadDistance);
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			return DrivingCycle.LookAhead(time);
		}


		public Watt BrakePower
		{
			get { return Brakes.BrakePower; }
			set { Brakes.BrakePower = value; }
		}

		public bool ClutchClosed(Second absTime)
		{
			if (Clutch == null) {
				Log.Warn("No Clutch in VehicleContainer. ClutchClosed set to constant true!");
				return true;
			}
			return Clutch.ClutchClosed(absTime);
		}

		public bool VehicleStopped
		{
			get { return Vehicle.VehicleStopped; }
		}

		public DrivingBehavior DriverBehavior
		{
			get { return Driver != null ? Driver.DriverBehavior : DrivingBehavior.Driving; }
		}

		public DrivingAction DrivingAction
		{
			get { return Driver.DrivingAction; }
		}

		public MeterPerSquareSecond DriverAcceleration
		{
			get { return Driver != null ? Driver.DriverAcceleration : 0.SI<MeterPerSquareSecond>(); }
		}

		public Radian RoadGradient
		{
			get { return DrivingCycle.RoadGradient; }
		}

		public Meter CycleStartDistance
		{
			get { return DrivingCycle == null ? 0.SI<Meter>() : DrivingCycle.CycleStartDistance; }
		}

		public VectoRunData RunData { get; set; }
		public ExecutionMode ExecutionMode { get; }



		public CycleData CycleData
		{
			get { return DrivingCycle.CycleData; }
		}

		public bool PTOActive
		{
			get { return DrivingCycle.PTOActive; }
		}

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			return DrivingCycle.CycleLookAhead(distance);
		}

		public Meter Altitude
		{
			get { return DrivingCycle.Altitude; }
		}

		public Watt AxlegearLoss()
		{
			return Axlegear.AxlegearLoss();
		}

		public Kilogram ReducedMassWheels
		{
			get { return Wheels.ReducedMassWheels; }
		}
	}
}