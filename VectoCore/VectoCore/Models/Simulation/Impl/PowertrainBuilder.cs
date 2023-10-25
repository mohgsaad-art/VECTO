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
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.ElectricSystem;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;
using StrategyCreator = System.Func<TUGraz.VectoCore.Models.Simulation.IVehicleContainer,
		TUGraz.VectoCore.Models.SimulationComponent.Impl.BaseShiftStrategy>;
using GbxTypeList = System.Collections.Generic.List<TUGraz.VectoCommon.Models.GearboxType>;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	/// <summary>
	/// Provides Methods to build a simulator with a powertrain step by step.
	/// </summary>
	public class PowertrainBuilder
	{
		private readonly IModalDataContainer _modData;
		private readonly WriteSumData _sumWriter;

		private static List<Tuple<GbxTypeList, string, string, StrategyCreator>> ShiftStrategies =
			new List<Tuple<GbxTypeList, string, string, StrategyCreator>> {
				Tuple.Create<GbxTypeList, string, string, StrategyCreator>(new GbxTypeList { GearboxType.MT },
					typeof(MTShiftStrategy).FullName, MTShiftStrategy.Name, c => new MTShiftStrategy(c)),
				Tuple.Create<GbxTypeList, string, string, StrategyCreator>(new GbxTypeList { GearboxType.AMT },
					typeof(AMTShiftStrategy).FullName, AMTShiftStrategy.Name, c => new AMTShiftStrategy(c)),
				Tuple.Create<GbxTypeList, string, string, StrategyCreator>(new GbxTypeList { GearboxType.AMT },
					typeof(AMTShiftStrategyOptimized).FullName, AMTShiftStrategyOptimized.Name,
					c => new AMTShiftStrategyOptimized(c)),
				//Tuple.Create<GbxTypeList, string, string, StrategyCreator>(new GbxTypeList { GearboxType.AMT },
				//	typeof(AMTShiftStrategyACEA).FullName, AMTShiftStrategyACEA.Name,
				//	c => new AMTShiftStrategyACEA(c)),
				Tuple.Create<GbxTypeList, string, string, StrategyCreator>(
					new GbxTypeList { GearboxType.ATPowerSplit, GearboxType.ATSerial },
					typeof(ATShiftStrategy).FullName, ATShiftStrategy.Name, c => new ATShiftStrategy(c)),
				//Tuple.Create<GbxTypeList, string, string, StrategyCreator>(
				//	new GbxTypeList { GearboxType.ATPowerSplit, GearboxType.ATSerial },
				//	typeof(ATShiftStrategyVoith).FullName, ATShiftStrategyVoith.Name,
				//	c => new ATShiftStrategyVoith(c)),
				Tuple.Create<GbxTypeList, string, string, StrategyCreator>(
					new GbxTypeList { GearboxType.ATPowerSplit, GearboxType.ATSerial },
					typeof(ATShiftStrategyOptimized).FullName, ATShiftStrategyOptimized.Name,
					c => new ATShiftStrategyOptimized(c)),
			};


		public PowertrainBuilder(IModalDataContainer modData, WriteSumData sumWriter = null)
		{
			if (modData == null) {
				throw new VectoException("Modal Data Container can't be null");
			}

			_modData = modData;
			_sumWriter = sumWriter;
		}

		public IVehicleContainer Build(VectoRunData data)
		{
			switch (data.Cycle.CycleType) {
				case CycleType.EngineOnly:
					return BuildEngineOnly(data);
				case CycleType.PWheel:
					return BuildPWheel(data);
				case CycleType.VTP:
					return BuildVTP(data);
				case CycleType.MeasuredSpeed:
					return BuildMeasuredSpeed(data);
				case CycleType.MeasuredSpeedGear:
					return BuildMeasuredSpeedGear(data);
				case CycleType.DistanceBased:
					return BuildFullPowertrain(data);
				default:
					throw new VectoException("Powertrain Builder cannot build Powertrain for CycleType: {0}",
						data.Cycle.CycleType);
			}
		}

		private IVehicleContainer BuildEngineOnly(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.EngineOnly) {
				throw new VectoException("CycleType must be EngineOnly.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, _modData, _sumWriter) { RunData = data };
			var cycle = new PowertrainDrivingCycle(container, data.Cycle);

			var directAux = new EngineAuxiliary(container);
			directAux.AddCycle(Constants.Auxiliaries.Cycle);
			container.ModalData.AddAuxiliary(Constants.Auxiliaries.Cycle);
			var engine = new EngineOnlyCombustionEngine(container, data.EngineData);
			new EngineOnlyGearboxInfo(container);
			new ZeroMileageCounter(container);
			new DummyDriverInfo(container);
			engine.Connect(directAux.Port());

			cycle.InPort().Connect(engine.OutPort());
			return container;
		}

		private IVehicleContainer BuildPWheel(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.PWheel) {
				throw new VectoException("CycleType must be PWheel.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, _modData, _sumWriter) { RunData = data };
			var gearbox = new CycleGearbox(container, data);

			// PWheelCycle --> AxleGear --> Clutch --> Engine <-- Aux
			var powertrain = new PWheelCycle(container, data.Cycle)
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(gearbox, data.Retarder, container)
				.AddComponent(new Clutch(container, data.EngineData));
			new ZeroMileageCounter(container);
			var engine = new StopStartCombustionEngine(container, data.EngineData, pt1Disabled: true);
			var idleController = GetIdleController(data.PTO, engine, container);

			powertrain.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);

			return container;
		}

		private IVehicleContainer BuildVTP(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.VTP) {
				throw new VectoException("CycleType must be VTP.");
			}

			var container = new VehicleContainer(data.ExecutionMode, _modData, _sumWriter) { RunData = data };
			var gearbox = new VTPGearbox(container, data);

			// VTPCycle --> AxleGear --> Clutch --> Engine <-- Aux
			var powertrain = new VTPCycle(container, data.Cycle)
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(gearbox, data.Retarder, container)
				.AddComponent(new Clutch(container, data.EngineData));
			new ZeroMileageCounter(container);
			var engine = new VTPCombustionEngine(container, data.EngineData, pt1Disabled: true);

			if (data.VehicleData.VehicleCategory.IsLorry()) {
				AddVTPTruckAuxiliaries(data, container, engine);
			} else if (data.VehicleData.VehicleCategory.IsBus()) {
				AddVTPBusAuxiliaries(data, container, engine);
			}

			var idleController = new CombustionEngine.CombustionEngineNoDubleclutchIdleController(engine, container);
			//if (data.PTO != null && data.PTO.PTOCycle != null) {
			//    var ptoController = new PTOCycleController(container, data.PTO.PTOCycle);
			//    idleController = new IdleControllerSwitcher(engine.IdleController, ptoController);
			//}

			powertrain.AddComponent(engine, idleController);
			//.AddAuxiliaries(container, data);

			return container;
		}

		private void AddVTPBusAuxiliaries(VectoRunData data, VehicleContainer container, VTPCombustionEngine engine)
		{
			var aux = new EngineAuxiliary(container);
			foreach (var auxData in data.Aux) {
				// id's in upper case
				var id = auxData.ID.ToUpper();

				switch (auxData.DemandType) {
					case AuxiliaryDemandType.Constant:
						aux.AddConstant(id, auxData.PowerDemand);
						break;
					case AuxiliaryDemandType.Direct:
						if (auxData.PowerDemandFunc == null) {
							aux.AddCycle(id);
						} else {
							aux.AddCycle(id, auxData.PowerDemandFunc);
						}
						break;
					case AuxiliaryDemandType.Mapping:
						aux.AddMapping(id, auxData.Data);
						break;
					default:
						throw new ArgumentOutOfRangeException("AuxiliaryDemandType", auxData.DemandType.ToString());
				}
				container.ModalData?.AddAuxiliary(id);
			}
			

			engine.Connect(aux.Port());
		}

		private void AddVTPTruckAuxiliaries(VectoRunData data, VehicleContainer container, VTPCombustionEngine engine)
		{
			var aux = CreateSpeedDependentAuxiliaries(data, container);
			var engineFan = new EngineFanAuxiliary(data.FanDataVTP.FanCoefficients.Take(3).ToArray(), data.FanDataVTP.FanDiameter);
			aux.AddCycle(Constants.Auxiliaries.IDs.Fan, cycleEntry => engineFan.PowerDemand(cycleEntry.FanSpeed));
			container.ModalData.AddAuxiliary(Constants.Auxiliaries.IDs.Fan);

			engine.Connect(aux.Port());
		}

		private IVehicleContainer BuildMeasuredSpeed(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeed) {
				throw new VectoException("CycleType must be MeasuredSpeed.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, _modData, _sumWriter) { RunData = data };

			// MeasuredSpeedDrivingCycle --> vehicle --> wheels --> brakes 
			// --> axleGear --> (retarder) --> GearBox --> (retarder) --> Clutch --> engine <-- Aux
			var cycle = new MeasuredSpeedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetGearbox(container), data.Retarder, container);
			if (data.GearboxData.Type.ManualTransmission()) {
				powertrain = powertrain.AddComponent(new Clutch(container, data.EngineData));
			}

			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var idleController = GetIdleController(data.PTO, engine, container);

			powertrain.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);

			return container;
		}

		private IVehicleContainer BuildMeasuredSpeedGear(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeedGear) {
				throw new VectoException("CycleType must be MeasuredSpeed with Gear.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, _modData, _sumWriter) { RunData = data };

			// MeasuredSpeedDrivingCycle --> vehicle --> wheels --> brakes 
			// --> axleGear --> (retarder) --> CycleGearBox --> (retarder) --> CycleClutch --> engine <-- Aux
			var powertrain = new MeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(new CycleGearbox(container, data));
			new ATClutchInfo(container);
			if (data.GearboxData.Type.ManualTransmission()) {
				powertrain = powertrain.AddComponent(new Clutch(container, data.EngineData));
			}

			powertrain.AddComponent(new StopStartCombustionEngine(container, data.EngineData))
				.AddAuxiliaries(container, data);

			
			return container;
		}

		private IVehicleContainer BuildFullPowertrain(VectoRunData data)
		{
			var isHybridVehicle = data.BatteryData != null && data.ElectricMachinesData.Count > 0;
			switch (data.JobType) {
				case VectoSimulationJobType.ConventionalVehicle:
					return BuildFullPowertrainConventional(data);
				case VectoSimulationJobType.ParallelHybridVehicle:
					return BuildFullPowertrainHybrid(data);
				case VectoSimulationJobType.BatteryElectricVehicle:
					return BuildBatteryElectricPowertrain(data);
				case VectoSimulationJobType.EngineOnlySimulation:
					return BuildEngineOnly(data);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private IVehicleContainer BuildFullPowertrainConventional(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}

			var container = new VehicleContainer(data.ExecutionMode, _modData, _sumWriter) { RunData = data };

			// DistanceBasedDrivingCycle --> driver --> vehicle --> wheels 
			// --> axleGear --> (retarder) --> gearBox --> (retarder) --> clutch --> engine <-- Aux
			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetGearbox(container), data.Retarder, container);
			if (data.GearboxData.Type.ManualTransmission()) {
				powertrain = powertrain.AddComponent(new Clutch(container, data.EngineData));
			}

			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var idleController = GetIdleController(data.PTO, engine, container);
			cycle.IdleController = idleController as IdleControllerSwitcher;

			powertrain.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);

			
			return container;
		}


		private IVehicleContainer BuildFullPowertrainHybrid(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}

			var container = new VehicleContainer(data.ExecutionMode, _modData, _sumWriter) { RunData = data };
			var es = new ElectricSystem(container);

			if (data.BatteryData != null) {
				if (data.BatteryData.InitialSoC < data.BatteryData.MinSOC) {
					throw new VectoException("Battery: Initial SoC has to be higher than min SoC");
				}
				var battery = new Battery(container, data.BatteryData);
				battery.Initialize(data.BatteryData.InitialSoC);
				es.Connect(battery);
			}

			if (data.SuperCapData != null) {
				if (data.SuperCapData.InitialSoC < data.SuperCapData.MinVoltage / data.SuperCapData.MaxVoltage) {
					throw new VectoException("SuperCap: Initial SoC has to be higher than min SoC");
				}
				var superCap = new SuperCap(container, data.SuperCapData);
				superCap.Initialize(data.SuperCapData.InitialSoC);
				es.Connect(superCap);
			}

			//var battery = new Battery(container, data.BatteryData);
			//battery.Initialize(data.BatteryData.InitialSoC);
			//es.Connect(battery);
			
			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			HybridController ctl;
			SwitchableClutch clutch = null;
			var strategy = new FMUSimulinkModelHybridStrategy(data, container);
			if (data.GearboxData.Type.ManualTransmission()) {
				//var strategy = new HybridStrategy(data, container);
				clutch = new SwitchableClutch(container, data.EngineData);
				ctl = new HybridController(container, strategy, es);
			} else {
				//var strategy = new HybridStrategyAT(data, container);
				ctl = new HybridController(container, strategy, es);
			}

			ctl = new HybridController(container, strategy, es, true);

			// add engine before gearbox so that gearbox can obtain if an ICE is available already in constructor
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var gearbox = GetGearbox(container, ctl.ShiftStrategy);
			var gbx = gearbox as IHybridControlledGearbox;
			if (gbx == null) {
				throw new VectoException("Gearbox can not be used for parallel hybrid");
			}

			var idleController = GetIdleController(data.PTO, engine, container);

			ctl.Gearbox = gbx;
			ctl.Engine = engine;
			
			// DistanceBasedDrivingCycle --> driver --> vehicle --> wheels 
			// --> axleGear --> (retarder) --> gearBox --> (retarder) --> clutch --> engine <-- Aux
			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(gearbox, data.Retarder, container)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);

			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				if (gearbox is ATGearbox atGbx) {
					atGbx.IdleController = idleController;
				}
			}
			cycle.IdleController = idleController as IdleControllerSwitcher;

			
			return container;
		}

		private IVehicleContainer BuildBatteryElectricPowertrain(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}

			if (data.ElectricMachinesData.Count > 1) {
				throw new VectoException("Electric motors on multiple positions not supported");
			}

			var container = new VehicleContainer(data.ExecutionMode, _modData, _sumWriter) { RunData = data };

			if (data.BatteryData != null && data.SuperCapData != null) {
				throw new VectoException("Only one REESS is supported.");
			}

			var es = new ElectricSystem(container);

			if (data.BatteryData != null) {
				var battery = new Battery(container, data.BatteryData);
				battery.Initialize(data.BatteryData.InitialSoC);
				es.Connect(battery);
			}

			if (data.SuperCapData != null) {
				var superCap = new SuperCap(container, data.SuperCapData);
				superCap.Initialize(data.SuperCapData.InitialSoC);
				es.Connect(superCap);
			}

			var ctl = new BatteryElectricMotorController(container, es);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius,
					data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First().Item1;
			switch (pos)
			{
				case PowertrainPosition.HybridPositionNotSet:
					throw new VectoException("invalid powertrain position");
				case PowertrainPosition.HybridP0:
				case PowertrainPosition.HybridP1:
				case PowertrainPosition.HybridP2:
				case PowertrainPosition.HybridP3:
				case PowertrainPosition.HybridP4:
					throw new VectoException("testcase does not support parallel powertrain configurations");
				case PowertrainPosition.BatteryElectricE4:
					powertrain.AddComponent(
						GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container);
					//new MockEngineInfo(container);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE3:
					powertrain.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(
							GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container);
					//new MockEngineInfo(container);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE2:
					var strategy = new PEVAMTShiftStrategy(container);
					powertrain.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(new PEVGearbox(container, strategy))
						.AddComponent(
							GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData, container, es, ctl));
					new ATClutchInfo(container);
					break;
				default: throw new ArgumentOutOfRangeException(nameof(pos), pos, null);
			}

			return container;
		}

		private IElectricMotor GetElectricMachine(PowertrainPosition pos,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, VehicleContainer container,
			IElectricSystem es, IHybridController ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null) {
				return null;
			}

			container.ModData?.AddElectricMotor(pos);
			ctl.AddElectricMotor(pos, motorData.Item2);
			var motor = new ElectricMotor(container, motorData.Item2, ctl.ElectricMotorControl(pos), pos);
			motor.Connect(es);
			return motor;
		}

		private IElectricMotor GetElectricMachine(PowertrainPosition pos,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, VehicleContainer container,
			IElectricSystem es, IElectricMotorControl ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null)
			{
				return null;
			}

			container.ModData?.AddElectricMotor(pos);
			var motor = new ElectricMotor(container, motorData.Item2, ctl, pos);
			motor.Connect(es);
			return motor;
		}

		public void BuildSimplePowertrain(VectoRunData data, IVehicleContainer container)
		{
			//if (data.Cycle.CycleType != CycleType.DistanceBased) {
			//	throw new VectoException("CycleType must be DistanceBased");
			//}

			var vehicle = new Vehicle(container, data.VehicleData, data.AirdragData);
			//var dummyDriver = new Driver(container, data.DriverData, new DefaultDriverStrategy(container));
			var powertrain = vehicle
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetSimpleGearbox(container, data), data.Retarder, container);
			if (data.GearboxData.Type.ManualTransmission()) {
				powertrain = powertrain.AddComponent(new Clutch(container, data.EngineData));
			}
			// DistanceBasedDrivingCycle --> driver --> vehicle --> wheels 
			// --> axleGear --> (retarder) --> gearBox --> (retarder) --> clutch --> engine <-- Aux

			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					container.AddComponent(new DistanceBasedDrivingCycle(container, data.Cycle));
					break;
				case CycleType.MeasuredSpeed:
					var dummyData = GetMeasuredSpeedDummnCycle();
					var msCycle = new MeasuredSpeedDrivingCycle(container, dummyData);
					msCycle.AddComponent(vehicle);
					break;
				case CycleType.EngineOnly: break;
				default: throw new VectoException("Wrong CycleType for SimplePowertrain");
			}


			var engine = new CombustionEngine(container, data.EngineData);
			var idleController = GetIdleController(data.PTO, engine, container);
			//cycle.IdleController = idleController as IdleControllerSwitcher;

			powertrain.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);
		}

		public void BuildSimpleHybridPowertrain(VectoRunData data, VehicleContainer container)
		{
			//if (data.Cycle.CycleType != CycleType.DistanceBased) {
			//	throw new VectoException("CycleType must be DistanceBased");
			//}

			var es = new ElectricSystem(container);
			if (data.BatteryData != null) {
				var battery = new Battery(container, data.BatteryData);
				battery.Initialize(data.BatteryData.InitialSoC);
				es.Connect(battery);
			}

			if (data.SuperCapData != null) {
				var superCap = new SuperCap(container, data.SuperCapData);
				superCap.Initialize(data.SuperCapData.InitialSoC);
				es.Connect(superCap);
			}

			//var battery = new Battery(container, data.BatteryData);
			//battery.Initialize(data.BatteryData.InitialSoC);
			//es.Connect(battery);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);
			
			var clutch = data.GearboxData.Type.ManualTransmission() ? new SwitchableClutch(container, data.EngineData) : null;

			// add engine before gearbox so that gearbox can obtain if an ICE is available already in constructor
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var gearbox = GetSimpleGearbox(container, data);
			var gbx = gearbox as IHybridControlledGearbox;
			if (gbx == null) {
				throw new VectoException("Gearbox can not be used for parallel hybrid");
			}

			var ctl = new SimpleHybridController(container, es, clutch);

			ctl.Gearbox = gbx;
			ctl.Engine = engine;

			var vehicle = new Vehicle(container, data.VehicleData, data.AirdragData);
			
			//var dummyDriver = new Driver(container, data.DriverData, new DefaultDriverStrategy(container));
			var powertrain = vehicle
				.AddComponent(
					new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(gearbox, data.Retarder, container)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesData, container, es, ctl));
			
			// DistanceBasedDrivingCycle --> driver --> vehicle --> wheels 
			// --> axleGear --> (retarder) --> gearBox --> (retarder) --> clutch --> engine <-- Aux

			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					container.AddComponent(new DistanceBasedDrivingCycle(container, data.Cycle));
					break;
				case CycleType.MeasuredSpeed:
					var dummyData = GetMeasuredSpeedDummnCycle();
					var msCycle = new MeasuredSpeedDrivingCycle(container, dummyData);
					msCycle.AddComponent(vehicle);
					break;
				case CycleType.EngineOnly: break;
				default: throw new VectoException("Wrong CycleType for SimplePowertrain");
			}
			
			var idleController = GetIdleController(data.PTO, engine, container);
			//cycle.IdleController = idleController as IdleControllerSwitcher;

			powertrain.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);
			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				if (gearbox is ATGearbox atGbx) {
					atGbx.IdleController = idleController;
				}
			}
		}

		public void BuildSimplePowertrainE2(VectoRunData data, VehicleContainer container)
		{
			//if (data.Cycle.CycleType != CycleType.DistanceBased) {
			//	throw new VectoException("CycleType must be DistanceBased");
			//}
			var vehicle = new Vehicle(container, data.VehicleData, data.AirdragData);

			var es = new ElectricSystem(container);
			if (data.BatteryData != null) {
				var battery = new Battery(container, data.BatteryData);
				battery.Initialize(data.BatteryData.InitialSoC);
				es.Connect(battery);
			}	

			if (data.SuperCapData != null) {
				var superCap = new SuperCap(container, data.SuperCapData);
				superCap.Initialize(data.SuperCapData.InitialSoC);
				es.Connect(superCap);
			}

			//var battery = new Battery(container, data.BatteryData);
			//battery.Initialize(data.BatteryData.InitialSoC);
			//es.Connect(battery);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			//var dummyDriver = new Driver(container, data.DriverData, new DefaultDriverStrategy(container));

			var ctl = new SimpleElectricMotorControl();
			var powertrain = vehicle
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetSimpleGearbox(container, data), data.Retarder, container)
				.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData, container, es, ctl));
			
			// DistanceBasedDrivingCycle --> driver --> vehicle --> wheels 
			// --> axleGear --> (retarder) --> gearBox --> (retarder) --> clutch --> engine <-- Aux

			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					container.AddComponent(new DistanceBasedDrivingCycle(container, data.Cycle));
					break;
				case CycleType.MeasuredSpeed:
					var dummyData = GetMeasuredSpeedDummnCycle();
					var msCycle = new MeasuredSpeedDrivingCycle(container, dummyData);
					msCycle.AddComponent(vehicle);
					break;
				case CycleType.EngineOnly:
					break;
				default:
					throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

		}

		private DrivingCycleData GetMeasuredSpeedDummnCycle()
		{
			var header = "<t>,<v>,<grad>";
			var entries = new[] { "0, 50, 0", "10, 50, 0" };
			var cycleData = new MemoryStream();
			var writer = new StreamWriter(cycleData);
			writer.WriteLine(header);
			foreach (var entry in entries) {
				writer.WriteLine(entry);
			}

			writer.Flush();
			cycleData.Seek(0, SeekOrigin.Begin);
			return DrivingCycleDataReader.ReadFromStream(cycleData, CycleType.MeasuredSpeed, "DummyCycle", false);
		}

		private static IIdleController GetIdleController(PTOData pto, ICombustionEngine engine,
			IVehicleContainer container)
		{
			var controller = engine.IdleController;

			if (pto != null && pto.PTOCycle != null) {
				var ptoController = new PTOCycleController(container, pto.PTOCycle);
				controller = new IdleControllerSwitcher(engine.IdleController, ptoController);
			}

			return controller;
		}

		internal static IAuxInProvider CreateAdvancedAuxiliaries(VectoRunData data, IVehicleContainer container)
		{
			var conventionalAux = CreateAuxiliaries(data, container);
			// TODO: MQ 2019-07-30 -- which fuel map for advanced auxiliaries?!
			var busAux = new BusAuxiliariesAdapter(container, data.BusAuxiliaries, conventionalAux);
			return busAux;
		}

		internal static EngineAuxiliary CreateAuxiliaries(VectoRunData data, IVehicleContainer container)
		{
			var aux = new EngineAuxiliary(container);
			foreach (var auxData in data.Aux) {
				// id's in upper case
				var id = auxData.ID.ToUpper();

				switch (auxData.DemandType) {
					case AuxiliaryDemandType.Constant:
						aux.AddConstant(id, auxData.PowerDemand);
						break;
					case AuxiliaryDemandType.Direct:
						aux.AddCycle(id);
						break;
					case AuxiliaryDemandType.Mapping:
						aux.AddMapping(id, auxData.Data);
						break;
					default:
						throw new ArgumentOutOfRangeException("AuxiliaryDemandType", auxData.DemandType.ToString());
				}

				container.ModalData?.AddAuxiliary(id);
			}

			if (data.PTO != null) {
				aux.AddConstant(Constants.Auxiliaries.IDs.PTOTransmission,
					DeclarationData.PTOTransmission.Lookup(data.PTO.TransmissionType).PowerDemand);
				container.ModalData?.AddAuxiliary(Constants.Auxiliaries.IDs.PTOTransmission,
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOTransmission);

				aux.Add(Constants.Auxiliaries.IDs.PTOConsumer,
					n => container.DrivingCycleInfo.PTOActive ? null : data.PTO.LossMap.GetTorqueLoss(n) * n);
				container.ModalData?.AddAuxiliary(Constants.Auxiliaries.IDs.PTOConsumer,
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOConsumer);
			}

			return aux;
		}

		private EngineAuxiliary CreateSpeedDependentAuxiliaries(VectoRunData data, IVehicleContainer container)
		{
			var aux = new EngineAuxiliary(container);

			var auxData = data.Aux.ToArray();
			AddSwitchingAux(aux, container.ModalData, Constants.Auxiliaries.IDs.HeatingVentilationAirCondition,
				auxData);
			AddSwitchingAux(aux, container.ModalData, Constants.Auxiliaries.IDs.SteeringPump, auxData);
			AddSwitchingAux(aux, container.ModalData, Constants.Auxiliaries.IDs.ElectricSystem, auxData);
			AddSwitchingAux(aux, container.ModalData, Constants.Auxiliaries.IDs.PneumaticSystem, auxData);

			return aux;
		}

		private void AddSwitchingAux(EngineAuxiliary aux, IModalDataContainer modData, string auxId,
			VectoRunData.AuxData[] auxData)
		{
			var urban = auxData.First(x => x.ID == auxId && x.MissionType == MissionType.UrbanDelivery);
			var rural = auxData.First(x => x.ID == auxId && x.MissionType == MissionType.RegionalDelivery);
			var motorway = auxData.First(x => x.ID == auxId && x.MissionType == MissionType.LongHaul);

			aux.AddCycle(auxId, entry => {
				if (entry.VehicleTargetSpeed >= Constants.SimulationSettings.HighwaySpeedThreshold) {
					return motorway.PowerDemand;
				}

				if (entry.VehicleTargetSpeed >= Constants.SimulationSettings.RuralSpeedThreshold) {
					return rural.PowerDemand;
				}

				return urban.PowerDemand;
			});
			modData.AddAuxiliary(auxId);
		}


		private static IGearbox GetGearbox(IVehicleContainer container)
		{
			var strategy = GetShiftStrategy( container);
			return GetGearbox(container, strategy);
		}

		private static IGearbox GetGearbox(IVehicleContainer container, IShiftStrategy strategy)
		{
			switch (container.RunData.GearboxData.Type)
			{
				case GearboxType.AMT:
				case GearboxType.MT:
					return new Gearbox(container, strategy);
				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					new ATClutchInfo(container);
					return new ATGearbox(container, strategy);
				default:
					throw new ArgumentOutOfRangeException("Unknown Gearbox Type", container.RunData.GearboxData.Type.ToString());
			}
		}

		public static IShiftStrategy GetShiftStrategy(IVehicleContainer container)
		{
			var runData = container.RunData;
			var shiftStrategy = runData.ShiftStrategy;
			if (string.IsNullOrWhiteSpace(shiftStrategy)) {
				shiftStrategy = DeclarationData.GearboxTCU.DefaultShiftStrategy;
			}

			if (string.IsNullOrWhiteSpace(shiftStrategy)) {
				switch (runData.GearboxData.Type) {
					case GearboxType.AMT:
						if (runData.JobType == VectoSimulationJobType.ConventionalVehicle) {
							runData.ShiftStrategy = AMTShiftStrategyOptimized.Name;
							return new AMTShiftStrategyOptimized(container);
						}

						if (runData.JobType == VectoSimulationJobType.BatteryElectricVehicle) {
							runData.ShiftStrategy = PEVAMTShiftStrategy.Name;
							return new PEVAMTShiftStrategy(container);
						}
						throw new VectoException("no default gearshift strategy available for gearbox type {0} and job type {1}", runData.GearboxData.Type, runData.JobType);
					//return new AMTShiftStrategy(runData, container);
					case GearboxType.MT:
						runData.ShiftStrategy = MTShiftStrategy.Name;
						return new MTShiftStrategy(container);
					case GearboxType.ATPowerSplit:
					case GearboxType.ATSerial:
						runData.ShiftStrategy = ATShiftStrategyOptimized.Name;
						return new ATShiftStrategyOptimized(container);
					//return new ATShiftStrategy(runData, container);
					default:
						throw new ArgumentOutOfRangeException("GearboxType",
							string.Format("Unknown Gearbox Type {0}", runData.GearboxData.Type.ToString()));
				}
			}

			var selected = ShiftStrategies.FirstOrDefault(x =>
				x.Item1.Contains(runData.GearboxData.Type) &&
				x.Item2.Equals(shiftStrategy, StringComparison.InvariantCultureIgnoreCase));
			if (selected == null) {
				throw new ArgumentOutOfRangeException("ShiftStrategy",
					string.Format("Unknown Shiftstrategy {0} for Gearbox Type {1}", shiftStrategy,
						runData.GearboxData.Type.ToString()));
			}

			runData.ShiftStrategy = selected.Item3;
			return selected.Item4(container);
		}

		private static IGearbox GetSimpleGearbox(IVehicleContainer container, VectoRunData runData)
		{
			if (runData.GearboxData.Type.AutomaticTransmission()) {
				new ATClutchInfo(container);
				return new ATGearbox(container, null);
			}
			return new Gearbox(container, null);
		}


		public static IEnumerable<Tuple<string, string>> GetRegisteredShiftStrategies(GearboxType? type)
		{
			if (!type.HasValue) {
				return new List<Tuple<string, string>>();
			}

			return ShiftStrategies.Where(x => x.Item1.Contains(type.Value)).Select(x => Tuple.Create(x.Item2, x.Item3))
				.ToList();
		}
	}

	public class SimpleElectricMotorControl : IElectricMotorControl
	{
		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity,
			PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
			PowertrainPosition position, bool dryRun)
		{
			if (dryRun) {
				return -outTorque;
			}
			return (-outTorque).LimitTo(maxDriveTorque, maxRecuperationTorque);
		}
	}


	
	internal class DummyDriverInfo : VectoSimulationComponent, IDriverInfo
	{
		public DummyDriverInfo(VehicleContainer container) : base(container)
		{
			
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior
		{
			get { return DrivingBehavior.Accelerating; }
		}

		public DrivingAction DrivingAction
		{
			get { return DrivingAction.Accelerate; }
		}

		public MeterPerSquareSecond DriverAcceleration
		{
			get { return 0.SI<MeterPerSquareSecond>(); }
		}

		#endregion
	}

	internal class EngineOnlyGearboxInfo : VectoSimulationComponent, IGearboxInfo
	{
		public EngineOnlyGearboxInfo(VehicleContainer container) : base(container)
		{
			
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion

		#region Implementation of IGearboxInfo

		public GearboxType GearboxType
		{
			get {return GearboxType.DrivingCycle; }
		}

		public GearshiftPosition Gear
		{
			get { return new GearshiftPosition(0); }
		}

		public bool TCLocked
		{
			get { return true; }
		}

		public MeterPerSecond StartSpeed
		{
			get { throw new VectoException("No Gearbox available. StartSpeed unknown."); }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { throw new VectoException("No Gearbox available. StartAcceleration unknown."); }
		}

		public Watt GearboxLoss()
		{
			throw new VectoException("No Gearbox available.");
		}

		public Second LastShift
		{
			get { throw new VectoException("No Gearbox available."); }
		}

		public Second LastUpshift
		{
			get { throw new VectoException("No Gearbox available."); }
		}

		public Second LastDownshift
		{
			get { throw new VectoException("No Gearbox available."); }
		}

		public GearData GetGearData(uint gear)
		{
			throw new VectoException("No Gearbox available.");
		}

		public GearshiftPosition NextGear
		{
			get { throw new VectoException("No Gearbox available."); }
		}

		public Second TractionInterruption
		{
			get { throw new NotImplementedException(); }
		}

		public uint NumGears
		{
			get { throw new NotImplementedException(); }
		}

		public bool DisengageGearbox
		{
			get { throw new VectoException("No Gearbox available."); }
		}

		public bool GearEngaged(Second absTime)
		{
			return true;
		}

		#endregion
	}

	internal class ZeroMileageCounter : VectoSimulationComponent, IMileageCounter
	{
		public ZeroMileageCounter(VehicleContainer container) : base(container)
		{
			
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion

		#region Implementation of IMileageCounter

		public Meter Distance
		{
			get { return 0.SI<Meter>(); }
		}

		#endregion
	}

	public class DummyVehicleInfo : VectoSimulationComponent, IVehicleInfo
	{
		public DummyVehicleInfo(VehicleContainer container) : base(container)
		{

		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		#endregion

		#region Implementation of IVehicleInfo

		public MeterPerSecond VehicleSpeed
		{
			get { return 0.SI<MeterPerSecond>(); }
		}

		public bool VehicleStopped
		{
			get { throw new System.NotImplementedException(); }
		}

		public Kilogram VehicleMass
		{
			get { throw new System.NotImplementedException(); }
		}

		public Kilogram VehicleLoading
		{
			get { throw new System.NotImplementedException(); }
		}

		public Kilogram TotalMass
		{
			get { throw new System.NotImplementedException(); }
		}

		public CubicMeter CargoVolume
		{
			get { throw new System.NotImplementedException(); }
		}

		public Newton AirDragResistance(MeterPerSecond previousVelocity, MeterPerSecond nextVelocity)
		{
			throw new System.NotImplementedException();
		}

		public Newton RollingResistance(Radian gradient)
		{
			throw new System.NotImplementedException();
		}

		public Newton SlopeResistance(Radian gradient)
		{
			throw new System.NotImplementedException();
		}

		public MeterPerSecond MaxVehicleSpeed
		{
			get { throw new System.NotImplementedException(); }
		}

		#endregion
	}

}