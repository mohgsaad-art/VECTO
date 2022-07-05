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
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
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

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	//public interface IPowertrainBuilderFactory
	//{
	//	PowertrainBuilder GetPowerTrainBuilder(IModalDataContainer modData, WriteSumData sumWriter = null);

	//}

	//public class PowertrainBuilderFactory : IPowertrainBuilderFactory
	//{
	//	#region Implementation of IPowertrainBuilderFactory

	//	public PowertrainBuilder GetPowerTrainBuilder(IModalDataContainer modData, WriteSumData sumWriter = null)
	//	{
	//		return new PowertrainBuilder(modData, sumWriter);
	//	}

	//	#endregion
	//}
	/// <summary>
	/// Provides Methods to build a simulator with a powertrain step by step.
	/// </summary>
	public static class PowertrainBuilder
	{

		public static IVehicleContainer Build(VectoRunData data, IModalDataContainer modData, ISumData sumWriter = null)
		{
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					switch (data.JobType) {
						case VectoSimulationJobType.ConventionalVehicle: return BuildFullPowertrainConventional(data, modData, sumWriter);
						case VectoSimulationJobType.ParallelHybridVehicle: return BuildFullPowertrainParallelHybrid(data, modData, sumWriter);
						case VectoSimulationJobType.SerialHybridVehicle: return BuildFullPowertrainSerialHybrid(data, modData, sumWriter);
						case VectoSimulationJobType.BatteryElectricVehicle: return BuildFulPowertrainBatteryElectric(data, modData, sumWriter);
						case VectoSimulationJobType.EngineOnlySimulation: return BuildEngineOnly(data, modData, sumWriter);
						case VectoSimulationJobType.IEPC_E: return BuildFullPowertrainIEPCE(data, modData, sumWriter);
						case VectoSimulationJobType.IEPC_S: return BuildFullPowertrainIEPCSerial(data, modData, sumWriter);
						default: throw new ArgumentOutOfRangeException($"Powertrain Builder cannot build Powertrain for JobType: {data.JobType}");
					}
				case CycleType.EngineOnly: return BuildEngineOnly(data, modData, sumWriter);
				case CycleType.PWheel:
					switch (data.JobType)
					{
						case VectoSimulationJobType.ConventionalVehicle: return BuildPWheelConventional(data, modData, sumWriter);
						case VectoSimulationJobType.BatteryElectricVehicle: return BuildPWheelBatteryElectric(data, modData, sumWriter);
						default: throw new ArgumentOutOfRangeException($"Powertrain Builder cannot build Powertrain for JobType: {data.JobType}");
					}
				case CycleType.VTP: return BuildVTP(data, modData, sumWriter);
				case CycleType.MeasuredSpeed:
					switch (data.JobType)
					{
						case VectoSimulationJobType.ConventionalVehicle: return BuildMeasuredSpeedConventional(data, modData, sumWriter);
						case VectoSimulationJobType.BatteryElectricVehicle: return BuildMeasuredSpeedBatteryElectric(data, modData, sumWriter);
						default: throw new ArgumentOutOfRangeException($"Powertrain Builder cannot build Powertrain for JobType: {data.JobType}");
					}
				case CycleType.MeasuredSpeedGear:
					switch (data.JobType)
					{
						case VectoSimulationJobType.ConventionalVehicle: return BuildMeasuredSpeedGearConventional(data, modData, sumWriter);
						case VectoSimulationJobType.BatteryElectricVehicle: return BuildMeasuredSpeedGearBatteryElectric(data, modData, sumWriter);
						default: throw new ArgumentOutOfRangeException($"Powertrain Builder cannot build Powertrain for JobType: {data.JobType}");
					}
				default: throw new VectoException("Powertrain Builder cannot build Powertrain for CycleType: {0}", data.Cycle.CycleType);
			}
		}

		/// <summary>
		/// Builds an engine only powertrain.
		/// <code>
		/// PowertrainDrivingCycle
		/// └StopStartCombustionEngine
		///  └(Aux)
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildEngineOnly(VectoRunData data, IModalDataContainer modData, ISumData _sumWriter)
		{
			if (_sumWriter == null)
				throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.EngineOnly) {
				throw new VectoException("CycleType must be EngineOnly.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, modData, _sumWriter) { RunData = data };

			var cycle = new PowertrainDrivingCycle(container, data.Cycle);
			var engine = new EngineOnlyCombustionEngine(container, data.EngineData);
			var directAux = new EngineAuxiliary(container);

			cycle.InPort().Connect(engine.OutPort());
			engine.Connect(directAux.Port());
			directAux.AddCycle(Constants.Auxiliaries.Cycle);

			new EngineOnlyGearboxInfo(container);
			new ZeroMileageCounter(container);
			new DummyDriverInfo(container);

			return container;
		}

		/// <summary>
		/// Builds a PWheel powertrain.
		/// <code>
		/// PWheelCycle
		/// └AxleGear
		///  ├(Angledrive)
		///  ├(TransmissionOutputRetarder)
		///  └CycleGearbox
		///   ├(TransmissionInputRetarder)
		///   └Clutch
		///    └StopStartCombustionEngine
		///     └(Aux)
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildPWheelConventional(VectoRunData data, IModalDataContainer modData, ISumData _sumWriter)
		{
			if (_sumWriter == null)
				throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.PWheel) {
				throw new VectoException("CycleType must be PWheel.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, modData, _sumWriter) { RunData = data };
			var engine = new StopStartCombustionEngine(container, data.EngineData, pt1Disabled: true);
			new PWheelCycle(container, data.Cycle)
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(new CycleGearbox(container, data))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(new Clutch(container, data.EngineData))
				.AddComponent(engine, GetIdleController(data.PTO, engine, container))
				.AddAuxiliaries(container, data);

			new ZeroMileageCounter(container);
			return container;
		}

		/// <summary>
		/// Builds a VTP powertrain.
		/// <code>
		/// VTPCycle
		/// └AxleGear
		///  ├(Angledrive)
		///  ├(TransmissionOutputRetarder)
		///  └VTPGearbox
		///   ├(TransmissionInputRetarder)
		///   └Clutch
		///    └VTPCombustionEngine
		///     └(VTPTruckAuxiliaries or VTPBusAuxiliaries)
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildVTP(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.VTP) {
				throw new VectoException("CycleType must be VTP.");
			}

			var container = new VehicleContainer(data.ExecutionMode, modData, sumWriter) { RunData = data };
			var engine = new VTPCombustionEngine(container, data.EngineData, pt1Disabled: true);

			new VTPCycle(container, data.Cycle)
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(new VTPGearbox(container, data))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(new Clutch(container, data.EngineData))
				.AddComponent(engine, new CombustionEngine.CombustionEngineNoDoubleClutchIdleController(engine, container));

			new ZeroMileageCounter(container);

			if (data.VehicleData.VehicleCategory.IsLorry()) {
				AddVTPTruckAuxiliaries(data, container, engine);
			} else if (data.VehicleData.VehicleCategory.IsBus()) {
				AddVTPBusAuxiliaries(data, container, engine);
			}

			return container;
		}

		private static void AddVTPBusAuxiliaries(VectoRunData data, VehicleContainer container, VTPCombustionEngine engine)
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
					default:
						throw new ArgumentOutOfRangeException("AuxiliaryDemandType", auxData.DemandType.ToString());
				}
			}
			engine.Connect(aux.Port());
		}

		private static void AddVTPTruckAuxiliaries(VectoRunData data, VehicleContainer container, VTPCombustionEngine engine)
		{
			var aux = CreateSpeedDependentAuxiliaries(data, container);
			var engineFan = new EngineFanAuxiliary(data.FanDataVTP.FanCoefficients.Take(3).ToArray(), data.FanDataVTP.FanDiameter);
			aux.AddCycle(Constants.Auxiliaries.IDs.Fan, cycleEntry => engineFan.PowerDemand(cycleEntry.FanSpeed));

			if (data.PTO != null) {
				aux.AddConstant(Constants.Auxiliaries.IDs.PTOTransmission,
					DeclarationData.PTOTransmission.Lookup(data.PTO.TransmissionType).PowerDemand,
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOTransmission);

				aux.Add(Constants.Auxiliaries.IDs.PTOConsumer,
					(n, absTime, dt, dryRun) => container.DrivingCycleInfo.PTOActive ? null : data.PTO.LossMap.GetTorqueLoss(n) * n,
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOConsumer);
			}

			engine.Connect(aux.Port());
		}

		/// <summary>
		/// Builds a measured speed powertrain.
		/// <code>
		/// MeasuredSpeedDrivingCycle
		/// └Vehicle
		///  └Wheels
		///   └Brakes
		///    └AxleGear
		///     ├(Angledrive)
		///     ├(TransmissionOutputRetarder)
		///     └Gearbox, ATGearbox, or APTNGearbox
		///      ├(TransmissionInputRetarder)
		///      ├(Clutch)
		///      └StopStartCombustionEngine
		///       └(Aux)
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildMeasuredSpeedConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeed) {
				throw new VectoException("CycleType must be MeasuredSpeed.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, modData, sumWriter) { RunData = data };
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			new MeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(GetGearbox(container))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? new Clutch(container, data.EngineData) : null)
				.AddComponent(engine, GetIdleController(data.PTO, engine, container))
				.AddAuxiliaries(container, data);
			return container;
		}

		/// <summary>
		/// Builds a measured speed (with gear) powertrain.
		/// <code>
		/// MeasuredSpeedDrivingCycle
		/// └Vehicle
		///  └Wheels
		///   └Brakes
		///    └AxleGear
		///     ├(Angledrive)
		///     ├(TransmissionOutputRetarder)
		///     └CycleGearbox
		///      ├(TransmissionInputRetarder)
		///      ├(Clutch)
		///      └StopStartCombustionEngine
		///       └(Aux)
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildMeasuredSpeedGearConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeedGear) {
				throw new VectoException("CycleType must be MeasuredSpeed with Gear.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, modData, sumWriter) { RunData = data };
			new MeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(new CycleGearbox(container, data))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? new Clutch(container, data.EngineData) : null)
				.AddComponent(new StopStartCombustionEngine(container, data.EngineData))
				.AddAuxiliaries(container, data);

			new ATClutchInfo(container);
			return container;
		}

		/// <summary>
		/// Builds a distance-based conventional powertrain.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └Brakes
		///     └AxleGear
		///      ├(Angledrive)
		///      ├(TransmissionOutputRetarder)
		///      └Gearbox, ATGearbox, or APTNGearbox
		///       ├(TransmissionInputRetarder)
		///       ├(Clutch)
		///       └StopStartCombustionEngine
		///        └(Aux)
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildFullPowertrainConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}

			var container = new VehicleContainer(data.ExecutionMode, modData, sumWriter) { RunData = data };
			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var idleController = GetIdleController(data.PTO, engine, container);
			cycle.IdleController = idleController as IdleControllerSwitcher;
			cycle.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(GetGearbox(container))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? new Clutch(container, data.EngineData) : null)
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);
			return container;
		}

		/// <summary>
		/// Builds a distance-based parallel hybrid powertrain.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └HybridController
		///     └Brakes
		///      ├(Engine P4)
		///      └AxleGear
		///       ├(Engine P3)
		///       ├(Angledrive)
		///       ├(TransmissionOutputRetarder)
		///       └Gearbox, ATGearbox, or APTNGearbox
		///        ├(TransmissionInputRetarder)
		///        ├(Engine P2.5)
		///        ├(Engine P2)
		///        ├(SwitchableClutch)
		///        ├(Engine P1)
		///        └StopStartCombustionEngine
		///         └(Aux)
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildFullPowertrainParallelHybrid(VectoRunData data, IModalDataContainer modData, ISumData _sumWriter)
		{
			if (_sumWriter == null)
				throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.GEN)) {
				throw new VectoException("ParallelHybrid does not support GEN set.");
			}
			if (data.ElectricMachinesData.Count != 1) {
				throw new VectoException("ParallelHybrid needs exactly one electric motor.");
			}
			if (data.ElectricMachinesData.Any(e => e.Item1 == PowertrainPosition.HybridP0)) {
				throw new VectoException("P0 Hybrids are modeled as SmartAlternator in the BusAuxiliary model.");
			}

			var container = new VehicleContainer(data.ExecutionMode, modData, _sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);
			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			HybridController ctl;
			SwitchableClutch clutch = null;
			
			if (data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC) {
				ctl = new HybridController(container,new HybridStrategy(data, container), es);
				clutch = new SwitchableClutch(container, data.EngineData);
			} else {
				ctl = new HybridController(container, new HybridStrategyAT(data, container), es);
				new ATClutchInfo(container);
			}
		

			// add engine before gearbox in the container, that gearbox can obtain it
			var engine = new StopStartCombustionEngine(container, data.EngineData);

			var gearbox = GetGearbox(container, ctl.ShiftStrategy);
			if (!(gearbox is IHybridControlledGearbox gbx)) {
				throw new VectoException($"Gearbox can not be used for parallel hybrid: ${gearbox?.GetType()}");
			}

			ctl.Gearbox = gbx;
			ctl.Engine = engine;

			if ((data.SuperCapData != null || data.BatteryData != null) && data.EngineData.WHRType.IsElectrical()) {
				var dcDcConverterEfficiency = DeclarationData.WHRChargerEfficiency;
				var whrCharger = new WHRCharger(container, dcDcConverterEfficiency);
				es.Connect(whrCharger);
				engine.WHRCharger = whrCharger;
			}

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var idleController = GetIdleController(data.PTO, engine, container);
			cycle.IdleController = idleController as IdleControllerSwitcher;
			cycle.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2_5, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.IHPC, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);

			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				// this has to be done _after_ the powertrain is connected together so that the cluch already has its nextComponent set (necessary in the idle controlelr)
				if (gearbox is ATGearbox atGbx) {
					atGbx.IdleController = idleController;
				} else {
					clutch.IdleController = idleController;
				}
			}

			if (data.BusAuxiliaries != null) {
				if (container.BusAux is BusAuxiliariesAdapter busAux) {
					var auxCfg = data.BusAuxiliaries;
					var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
						? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity, auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
						: (ISimpleBattery)new NoBattery(container);
					busAux.ElectricStorage = electricStorage;
					if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
						var dcdc = new DCDCConverter(container,
							data.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency);
						busAux.DCDCConverter = dcdc;
						es.Connect(dcdc);
					}
				} else {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}
			}

			return container;
		}

		/// <summary>
		/// Builds a distance-based serial hybrid powertrain for either E4, E3, or E2.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └SerialHybridController
		///     └Brakes
		///      │ └Engine E4
		///      └AxleGear
		///       │ ├(AxlegearInputRetarder)
		///       │ └Engine E3
		///       ├(AngleDrive)
		///       ├(TransmissionOutputRetarder)
		///       └PEVGearbox or APTNGearbox
		///        ├(TransmissionInputRetarder)
		///        └Engine E2
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildFullPowertrainSerialHybrid(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (sumWriter == null)
				throw new ArgumentNullException(nameof(sumWriter));
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 == PowertrainPosition.GEN) != 1) {
				throw new VectoException("SerialHybrid needs exactly one GEN set.");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 != PowertrainPosition.GEN) != 1) {
				throw new VectoException("SerialHybrid needs exactly one electric motor.");
			}

			var container = new VehicleContainer(data.ExecutionMode, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);
			var strategy = data.GearboxData != null && data.GearboxData.Type.AutomaticTransmission()
				? (IHybridControlStrategy)new SerialHybridStrategyAT(data, container)
				: new SerialHybridStrategy(data, container);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			var ctl = new SerialHybridController(container, strategy, es);

			var engine = new StopStartCombustionEngine(container, data.EngineData);

			var idleController = engine.IdleController;
			ctl.Engine = engine;

			if ((data.SuperCapData != null || data.BatteryData != null) && data.EngineData.WHRType.IsElectrical()) {
				var dcDcConverterEfficiency = DeclarationData.WHRChargerEfficiency;
				var whrCharger = new WHRCharger(container, dcDcConverterEfficiency);
				es.Connect(whrCharger);
				engine.WHRCharger = whrCharger;

			}

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			switch (pos) {
				case PowertrainPosition.BatteryElectricE4:
					//-->Engine E4
					powertrain.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE4,
						data.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container, new GearshiftPosition(0));
					new DummyAxleGearInfo(container);
					new ATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE3:
					//-->AxleGear-->(AxlegearInputRetarder)-->Engine E3
					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container, new GearshiftPosition(0));
					new ATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE2:
					//-->AxleGear-->(AngleDrive)-->(TransmissionOutputRetarder)-->PEVGearbox or APTNGearbox-->(TransmissionInputRetarder)-->Engine E2
					Gearbox gearbox;
					if (data.GearboxData.Type == GearboxType.APTN) {
						gearbox = new APTNGearbox(container, new APTNShiftStrategy(container));
					} else {
						gearbox = new PEVGearbox(container, new PEVAMTShiftStrategy(container));
					}

					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
						.AddComponent(gearbox)
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
						.AddComponent(GetPEVPTO(container, data))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE2,
							data.ElectricMachinesData, container, es, ctl));
					new ATClutchInfo(container);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for serial hybrid vehicle.");
			}

			if (data.BusAuxiliaries != null) {
				if (container.BusAux is BusAuxiliariesAdapter busAux) {
					var auxCfg = data.BusAuxiliaries;
					var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
						? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity, auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
						: (ISimpleBattery)new NoBattery(container);
					busAux.ElectricStorage = electricStorage;
					if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
						var dcdc = new DCDCConverter(container,
							data.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency);
						busAux.DCDCConverter = dcdc;
						es.Connect(dcdc);
					}
				} else {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}
			}

			ctl.GenSet.AddComponent(GetElectricMachine(PowertrainPosition.GEN, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliariesSerialHybrid(container, data);

			return container;
		}


		/// <summary>
		/// Builds a battery electric powertrain for either E4, E3, or E2.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └Brakes
		///     │ └Engine E4
		///     └AxleGear
		///      │ ├(AxlegearInputRetarder)
		///      | └Engine E3
		///      ├(Angledrive)
		///      ├(TransmissionOutputRetarder)
		///      └PEVGearbox or APTNGearbox
		///       ├(TransmissionInputRetarder)
		///       └Engine E2
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildFulPowertrainBatteryElectric(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.GEN)) {
				throw new VectoException("Battery electric vehicle does not support GEN set.");
			}
			if (data.ElectricMachinesData.Count != 1) {
				throw new VectoException("Battery electric vehicle needs exactly one electric motor.");
			}
			
			var container = new VehicleContainer(data.ExecutionMode, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);

			var ctl = new BatteryElectricMotorController(container, es);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First().Item1;
			IElectricMotor em;
			switch (pos) {
				case PowertrainPosition.BatteryElectricE4:
					//-->Engine E4
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);
					powertrain.AddComponent(em);
					new DummyGearboxInfo(container);
					new DummyAxleGearInfo(container);
					new ATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE3:
					//-->AxleGear-->(AxlegearInputRetarder)-->Engine E3
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);
					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
						.AddComponent(em);
					new DummyGearboxInfo(container);
					new ATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE2:
					//-->AxleGear-->(Angledrive)-->(TransmissionOutputRetarder)-->APTNGearbox or PEVGearbox-->(TransmissionInputRetarder)-->Engine E2
					var gearbox = data.GearboxData.Type == GearboxType.APTN
						? (Gearbox)new APTNGearbox(container, new APTNShiftStrategy(container))
						: new PEVGearbox(container, new PEVAMTShiftStrategy(container));
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData, container, es, ctl);
					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
						.AddComponent(gearbox)
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
						.AddComponent(GetPEVPTO(container, data))
						.AddComponent(em);

					new ATClutchInfo(container);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for BatteryElectric Vehicle");
			}

			new DummyEngineInfo(container);

			if (data.BusAuxiliaries != null) {
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;
				var busAux = new BusAuxiliariesAdapter(container, auxCfg);
				var electricStorage = new NoBattery(container);
				busAux.ElectricStorage = electricStorage;
				var dcdc = new DCDCConverter(container, data.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency);
				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			}

			return container;
		}
		
		private static IVehicleContainer BuildPWheelBatteryElectric(VectoRunData data, IModalDataContainer modData, WriteSumData sumWriter)
        {
			if (data.Cycle.CycleType != CycleType.PWheel)
				throw new VectoException("CycleType must be DistanceBased or MeasuredSpeed");
			ValidateBatteryElectric(data);

			var container = new VehicleContainer(data.ExecutionMode, modData, sumWriter) { RunData = data };

			var es = new ElectricSystem(container);

			if (data.BatteryData != null)
			{
				var battery = new BatterySystem(container, data.BatteryData);
				battery.Initialize(data.BatteryData.InitialSoC);
				es.Connect(battery);
			}

			if (data.SuperCapData != null)
			{
				var superCap = new SuperCap(container, data.SuperCapData);
				superCap.Initialize(data.SuperCapData.InitialSoC);
				es.Connect(superCap);
			}

			var ctl = new PWheelBatteryElectricMotorController(container, es);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			var powertrain = new PWheelCycle(container, data.Cycle);
						
            IElectricMotor em;
			
			var position = data.ElectricMachinesData.First().Item1;
			
			switch (position)
			{
				case PowertrainPosition.BatteryElectricE4:
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);
					powertrain.AddComponent(em);
					new DummyGearboxInfo(container);
					new DummyAxleGearInfo(container);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE3:
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);
					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
						.AddComponent(em);
					new DummyGearboxInfo(container);
					new ATClutchInfo(container);
					new DummyEngineInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE2:
					em = GetElectricMachine(position, data.ElectricMachinesData, container, es, ctl);
					new ZeroMileageCounter(container);
					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
						.AddComponent(new CycleGearbox(container, data))
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
						.AddComponent(em);
					new ATClutchInfo(container);
					new DummyEngineInfo(container);
					break;

				default: throw new ArgumentOutOfRangeException(nameof(position), position, null);
			}
			
			if (data.BusAuxiliaries != null)
			{
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS)
				{
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;

				var busAux = new BusAuxiliariesAdapter(container, auxCfg);

				var electricStorage = new NoBattery(container);
				busAux.ElectricStorage = electricStorage;

				var dcdc = new DCDCConverter(container,
					data.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency);
				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			}

			return container;
		}

		private static IPowerTrainComponent GetPEVPTO(VehicleContainer container, VectoRunData data)
		{
			if (data.PTO == null) {
				return null;
			}
			var pto = new PEVPTO(container);

			RoadSweeperAuxiliary rdSwpAux = null;
			PTODriveAuxiliary ptoDrive = null;
			if (data.ExecutionMode == ExecutionMode.Engineering && data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityRoadSweeping)) {
				if (data.DriverData.PTODriveMinSpeed == null) {
					throw new VectoSimulationException("PTO activity 'road sweeping' requested, but no min. engine speed or gear provided");
				}
				rdSwpAux = new RoadSweeperAuxiliary(container);
				pto.Add(Constants.Auxiliaries.IDs.PTORoadsweeping, (nEng, absTime, dt, dryRun) => rdSwpAux.PowerDemand(nEng, absTime, dt, dryRun) / nEng);
				container.AddAuxiliary(Constants.Auxiliaries.IDs.PTORoadsweeping, Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTORoadsweeping);
			}

			if (data.ExecutionMode == ExecutionMode.Engineering &&
				data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityWhileDrive)) {
				if (data.PTOCycleWhileDrive == null) {
					throw new VectoException("PTO activation while drive requested in cycle but no PTO cycle provided");
				}

				ptoDrive = new PTODriveAuxiliary(container, data.PTOCycleWhileDrive);
				pto.Add(Constants.Auxiliaries.IDs.PTODuringDrive, (nEng, absTime, dt, dryRun) => ptoDrive.PowerDemand(nEng, absTime, dt, dryRun) / nEng);
				container.AddAuxiliary(Constants.Auxiliaries.IDs.PTODuringDrive, Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTODuringDrive);
			}
			if (data.PTO != null) {
				pto.AddConstant(Constants.Auxiliaries.IDs.PTOTransmission,
								DeclarationData.PTOTransmission.Lookup(data.PTO.TransmissionType).TorqueLoss);
				container.AddAuxiliary(Constants.Auxiliaries.IDs.PTOTransmission,
												Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOTransmission);

				pto.Add(Constants.Auxiliaries.IDs.PTOConsumer,
						(n, absTime, dt, dryRun) => container.DrivingCycleInfo.PTOActive || (rdSwpAux?.Active(absTime) ?? false) || (ptoDrive?.Active(absTime) ?? false) ? null : data.PTO.LossMap.GetTorqueLoss(n));
				container.AddAuxiliary(Constants.Auxiliaries.IDs.PTOConsumer,
												Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOConsumer);
			}

			return pto;
		}

		private static IVehicleContainer BuildMeasuredSpeedBatteryElectric(VectoRunData data, IModalDataContainer modData, WriteSumData sumWriter)
        {
			if (data.Cycle.CycleType != CycleType.MeasuredSpeed)
				throw new VectoException("CycleType must be DistanceBased or MeasuredSpeed");
			ValidateBatteryElectric(data);

			var container = new VehicleContainer(data.ExecutionMode, modData, sumWriter) { RunData = data };

			var es = new ElectricSystem(container);

			if (data.BatteryData != null)
			{
				var battery = new BatterySystem(container, data.BatteryData);
				battery.Initialize(data.BatteryData.InitialSoC);
				es.Connect(battery);
			}

			if (data.SuperCapData != null)
			{
				var superCap = new SuperCap(container, data.SuperCapData);
				superCap.Initialize(data.SuperCapData.InitialSoC);
				es.Connect(superCap);
			}

			var ctl = new BatteryElectricMotorController(container, es);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			var timeBasedCycle = new MeasuredSpeedDrivingCycle(container, data.Cycle);
			IPowerTrainComponent powertrain = timeBasedCycle
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			IElectricMotor em;
			var position = data.ElectricMachinesData.First().Item1;
			switch (position)
			{
				case PowertrainPosition.BatteryElectricE4:
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);
					powertrain.AddComponent(em);
					new DummyGearboxInfo(container);
					new DummyAxleGearInfo(container);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE3:
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);
					powertrain.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(em);
					new DummyGearboxInfo(container);
					new ATClutchInfo(container);
					new DummyEngineInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE2 when data.GearboxData.Type != GearboxType.APTN:
					var strategy = new PEVAMTShiftStrategy(container);
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData,
						container, es, ctl);
					powertrain.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(new PEVGearbox(container, strategy))
						.AddComponent(em);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE2 when data.GearboxData.Type == GearboxType.APTN:
					var strategyAPTN = new APTNShiftStrategy(container);
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData,
						container, es, ctl);
					powertrain.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(new APTNGearbox(container, strategyAPTN))
						.AddComponent(em);
					new ATClutchInfo(container);
					break;

				default: throw new ArgumentOutOfRangeException(nameof(position), position, null);
			}

			if (data.BusAuxiliaries != null)
			{
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS)
				{
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;

				var busAux = new BusAuxiliariesAdapter(container, auxCfg);

				var electricStorage = new NoBattery(container);
				busAux.ElectricStorage = electricStorage;

				var dcdc = new DCDCConverter(container,
					data.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency);
				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			}

			return container;

		}
		private static IVehicleContainer BuildMeasuredSpeedGearBatteryElectric(VectoRunData data, IModalDataContainer modData, WriteSumData sumWriter)
        {
			if (data.Cycle.CycleType != CycleType.MeasuredSpeedGear)
				throw new VectoException("CycleType must be DistanceBased or MeasuredSpeed");
			ValidateBatteryElectric(data);

			var container = new VehicleContainer(data.ExecutionMode, modData, sumWriter) { RunData = data };

			var es = new ElectricSystem(container);

			if (data.BatteryData != null)
			{
				var battery = new BatterySystem(container, data.BatteryData);
				battery.Initialize(data.BatteryData.InitialSoC);
				es.Connect(battery);
			}

			if (data.SuperCapData != null)
			{
				var superCap = new SuperCap(container, data.SuperCapData);
				superCap.Initialize(data.SuperCapData.InitialSoC);
				es.Connect(superCap);
			}

			var ctl = new BatteryElectricMotorController(container, es);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			var timeBasedCycle = new MeasuredSpeedDrivingCycle(container, data.Cycle);
			IPowerTrainComponent powertrain =  timeBasedCycle
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			IElectricMotor em;
			var position = data.ElectricMachinesData.First().Item1;
			switch (position)
			{
				case PowertrainPosition.BatteryElectricE4:
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);
					powertrain.AddComponent(em);
					new DummyGearboxInfo(container);
					new DummyAxleGearInfo(container);
					new ATClutchInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE3:
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);
					powertrain.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(em);
					new DummyGearboxInfo(container);
					new ATClutchInfo(container);
					new DummyEngineInfo(container);
					break;
				case PowertrainPosition.BatteryElectricE2 when data.Cycle.CycleType == CycleType.MeasuredSpeedGear:
					em = GetElectricMachine(position, data.ElectricMachinesData, container, es, ctl);
					powertrain.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
						.AddComponent(new CycleGearbox(container, data))
						.AddComponent(em);
					new ATClutchInfo(container);
					new DummyEngineInfo(container);
					break;

				default: throw new ArgumentOutOfRangeException(nameof(position), position, null);
			}

			if (data.BusAuxiliaries != null)
			{
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS)
				{
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;

				var busAux = new BusAuxiliariesAdapter(container, auxCfg);

				var electricStorage = new NoBattery(container);
				busAux.ElectricStorage = electricStorage;

				var dcdc = new DCDCConverter(container,
					data.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency);
				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			}

			return container;

		}
		private static void ValidateBatteryElectric(VectoRunData data)
        {
			if (data.ElectricMachinesData.Count > 1)
			{
				throw new VectoException("Electric motors on multiple positions not supported");
			}

			if (data.BatteryData != null && data.SuperCapData != null)
			{
				throw new VectoException("Only one REESS is supported.");
			}

			var position = data.ElectricMachinesData.First().Item1;
			if (position == PowertrainPosition.HybridPositionNotSet)
				throw new VectoException("invalid powertrain position");
			if (position == PowertrainPosition.HybridP0 ||
				position == PowertrainPosition.HybridP1 ||
				position == PowertrainPosition.HybridP2 ||
				position == PowertrainPosition.HybridP3 ||
				position == PowertrainPosition.HybridP4)
			{
				throw new VectoException("BatteryElectric Vehicle does not support parallel powertrain configurations");
			}
		}
		private static Retarder GetRetarder(RetarderType type, RetarderData data, IVehicleContainer container) =>
			type == data.Type ? new Retarder(container, data.LossMap, data.Ratio) : null;

		private static IElectricMotor GetElectricMachine(PowertrainPosition pos, IList<Tuple<PowertrainPosition,
				ElectricMotorData>> electricMachinesData, VehicleContainer container, IElectricSystem es, IHybridController ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData is null) {
				return null;
			}

			//container.ModData?.AddElectricMotor(pos);
			ctl.AddElectricMotor(pos, motorData.Item2);
			var motor = pos == PowertrainPosition.IEPC
				? new IEPC(container, motorData.Item2, ctl.ElectricMotorControl(pos), pos)
				: new ElectricMotor(container, motorData.Item2, ctl.ElectricMotorControl(pos), pos);
			if (pos == PowertrainPosition.GEN) {
				es.Connect(new GensetChargerAdapter(motor));
			} else {
				motor.Connect(es);
			}
			return motor;
		}

		private static IElectricMotor GetElectricMachine(PowertrainPosition pos, IList<Tuple<PowertrainPosition,
				ElectricMotorData>> electricMachinesData, VehicleContainer container, IElectricSystem es, IElectricMotorControl ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData is null) {
				return null;
			}

			//container.ModData?.AddElectricMotor(pos);
			var motor = pos == PowertrainPosition.IEPC
				? new IEPC(container, motorData.Item2, ctl, pos)
				: new ElectricMotor(container, motorData.Item2, ctl, pos);
			motor.Connect(es);
			return motor;
		}

		/// <summary>
		/// Builds a battery electric powertrain for either E4, E3, or E2.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └Brakes
		///     └AxleGear
		///      │ ├(AxlegearInputRetarder)
		///      | ├Singlespeed Gearbox
		///      | └Engine IEPC
		///      └ APTNGearbox
		///       └Engine IEPC
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildFullPowertrainIEPCE(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.GEN)) {
				throw new VectoException("IEPC vehicle does not support GEN set.");
			}
			if (data.ElectricMachinesData.Count != 1) {
				throw new VectoException("IEPC vehicle needs exactly one electric motor.");
			}

			var container = new VehicleContainer(data.ExecutionMode, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);

			var ctl = new BatteryElectricMotorController(container, es);

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First().Item1;
			IElectricMotor em;
			if (pos != PowertrainPosition.IEPC) {
				throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for BatteryElectric Vehicle");
			}
			
			//-->AxleGear-->APTNGearbox or SinglespeedGearbox-->Engine E2
			var gearbox = data.GearboxData.Gears.Count > 1
				? (IGearbox)new IEPCGearbox(container, new APTNShiftStrategy(container))
				: new SingleSpeedGearbox(container, data.GearboxData);
			em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);
			powertrain
				.AddComponent(data.AxleGearData != null ? new AxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);

			new ATClutchInfo(container);
			if (data.AxleGearData == null) {
				new DummyAxleGearInfo(container);
			}
			new DummyEngineInfo(container);

			if (data.BusAuxiliaries != null) {
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;
				var busAux = new BusAuxiliariesAdapter(container, auxCfg);
				var electricStorage = new NoBattery(container);
				busAux.ElectricStorage = electricStorage;
				var dcdc = new DCDCConverter(container, data.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency);
				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			}

			return container;
		}

		/// <summary>
		/// Builds a battery electric powertrain for either E4, E3, or E2.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └Brakes
		///     └AxleGear
		///      │ ├(AxlegearInputRetarder)
		///      | ├Singlespeed Gearbox
		///      | └Engine IEPC
		///      └ APTNGearbox
		///       └Engine IEPC
		/// </code>
		/// </summary>
		private static IVehicleContainer BuildFullPowertrainIEPCSerial(VectoRunData data, IModalDataContainer modData, ISumData _sumWriter)
		{
			if (_sumWriter == null)
				throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 == PowertrainPosition.GEN) != 1) {
				throw new VectoException("IEPC vehicle needs exactly one GEN set.");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 != PowertrainPosition.GEN) != 1) {
				throw new VectoException("IEPC vehicle needs exactly one electric motor.");
			}

			var container = new VehicleContainer(data.ExecutionMode, modData, _sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);

			var strategy = new SerialHybridStrategy(data, container);
			var ctl = new SerialHybridController(container, strategy, es);

			var engine = new StopStartCombustionEngine(container, data.EngineData);

			var idleController = engine.IdleController;
			ctl.Engine = engine;

			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			IElectricMotor em;
			if (pos != PowertrainPosition.IEPC) {
				throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for BatteryElectric Vehicle");
			}

			//-->AxleGear-->APTNGearbox or SinglespeedGearbox-->Engine E2
			var gearbox = data.GearboxData.Gears.Count > 1
				? (IGearbox)new IEPCGearbox(container, new APTNShiftStrategy(container))
				: new SingleSpeedGearbox(container, data.GearboxData);
			em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);
			powertrain
				.AddComponent(data.AxleGearData != null ? new AxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);

			new ATClutchInfo(container);
			if (data.AxleGearData == null) {
				new DummyAxleGearInfo(container);
			}

			if (data.BusAuxiliaries != null) {
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;
				var busAux = new BusAuxiliariesAdapter(container, auxCfg);
				var electricStorage = new NoBattery(container);
				busAux.ElectricStorage = electricStorage;
				var dcdc = new DCDCConverter(container, data.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency);
				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			}

			ctl.GenSet.AddComponent(GetElectricMachine(PowertrainPosition.GEN, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);

			return container;
		}

		/// <summary>
		/// Builds a simple conventional powertrain.
		/// <code>
		///(MeasuredSpeedDrivingCycle)
		/// └Vehicle
		///  └Wheels
		///   └Brakes
		///    └AxleGear
		///     ├(Angledrive)
		///     ├(TransmissionOutputRetarder)
		///     └ATGearbox or Gearbox
		///      ├(TransmissionInputRetarder)
		///      ├(Clutch)
		///      └CombustionEngine
		///       └(Aux)
		/// </code>
		/// </summary>
		public static void BuildSimplePowertrain(VectoRunData data, IVehicleContainer container)
		{
			IVehicle vehicle = new Vehicle(container, data.VehicleData, data.AirdragData);
			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.MeasuredSpeed:
					new MeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
					break;
				case CycleType.DistanceBased:
					container.AddComponent(new DistanceBasedDrivingCycle(container, data.Cycle));
					break;
				case CycleType.EngineOnly:
					break;
				default:
					throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

			var engine = new CombustionEngine(container, data.EngineData);
			vehicle.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(GetSimpleGearbox(container, data))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? new Clutch(container, data.EngineData) : null)
				.AddComponent(engine, GetIdleController(data.PTO, engine, container))
				.AddAuxiliaries(container, data);
		}

		/// <summary>
		/// Builds a simple serial hybrid powertrain with either E4, E3, or E2.
		/// <code>
		/// Vehicle
		/// └Wheels
		///  └SimpleHybridController
		///   └Brakes
		///    │ └Engine E4
		///    └AxleGear
		///     │ ├(AxlegearInputRetarder)
		///     │ └Engine E3
		///     ├(AngleDrive)
		///     ├(TransmissionOutputRetarder)
		///     └Gearbox or APTNGearbox
		///      ├(TransmissionInputRetarder)
		///      └Engine E2
		/// </code>
		/// </summary>
		public static void BuildSimpleSerialHybridPowertrain(VectoRunData data, VehicleContainer container)
		{
			var es = ConnectREESS(data, container);
			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);
			es.Connect(new GensetChargerAdapter(null));

			var ctl = new SimpleHybridController(container, es);

			//Vehicle-->Wheels-->SimpleHybridController-->Brakes
			var powertrain = new Vehicle(container, data.VehicleData, data.AirdragData)
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			switch (pos) {
				case PowertrainPosition.BatteryElectricE4:
					//-->Engine E4
					new DummyGearboxInfo(container);
					new ATClutchInfo(container);

					powertrain.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE4,
						data.ElectricMachinesData, container, es, ctl));
					break;

				case PowertrainPosition.BatteryElectricE3:
					//-->AxleGear-->(AxlegearInputRetarder)-->Engine E3
					new DummyGearboxInfo(container);
					new ATClutchInfo(container);

					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE3,
							data.ElectricMachinesData, container, es, ctl));
					break;

				case PowertrainPosition.BatteryElectricE2:
					//-->AxleGear-->(AngleDrive)-->(TransmissionOutputRetarder)-->APTNGearbox or Gearbox-->(TransmissionInputRetarder)-->Engine E2
					Gearbox gearbox;
					if (data.GearboxData.Type.AutomaticTransmission()) {
						gearbox = new APTNGearbox(container, ctl.ShiftStrategy);
					} else {
						gearbox = new Gearbox(container, ctl.ShiftStrategy);
					}

					ctl.Gearbox = gearbox;
					new DummyEngineInfo(container);

					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
						.AddComponent(gearbox)
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
						.AddComponent(GetPEVPTO(container, data))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE2,
							data.ElectricMachinesData, container, es, ctl));
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for simple serial hybrid vehicles.");
			}
		}

		/// <summary>
		/// Builds a simple serial hybrid powertrain with either E4, E3, or E2.
		/// <code>
		/// Vehicle
		/// └Wheels
		///  └SimpleHybridController
		///   └Brakes
		///    │ └Engine E4
		///    └AxleGear
		///     │ ├(AxlegearInputRetarder)
		///     │ └Engine E3
		///     ├(AngleDrive)
		///     ├(TransmissionOutputRetarder)
		///     └Gearbox or APTNGearbox
		///      ├(TransmissionInputRetarder)
		///      └Engine E2
		/// </code>
		/// </summary>
		public static void BuildSimpleIEPCHybridPowertrain(VectoRunData data, VehicleContainer container)
		{
			var es = ConnectREESS(data, container);
			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);
			es.Connect(new GensetChargerAdapter(null));

			var ctl = new SimpleHybridController(container, es);

			//Vehicle-->Wheels-->SimpleHybridController-->Brakes
			var powertrain = new Vehicle(container, data.VehicleData, data.AirdragData)
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			var gearbox = data.GearboxData.Gears.Count > 1
				? (IGearbox)new APTNGearbox(container, ctl.ShiftStrategy)
				: new SingleSpeedGearbox(container, data.GearboxData);
			var em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);
			powertrain
				.AddComponent(data.AxleGearData != null ? new AxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);
		}

		/// <summary>
		/// Builds a simple genset
		/// <code>
		/// Engine Gen
		///  └CombustionEngine
		/// </code>
		/// </summary>
		public static void BuildSimpleGenSet(VectoRunData data, VehicleContainer container)
		{
			var es = ConnectREESS(data, container);
			var ctl = new GensetMotorController(container, es);

			GetElectricMachine(PowertrainPosition.GEN, data.ElectricMachinesData, container, es, ctl)
				.AddComponent(new StopStartCombustionEngine(container, data.EngineData));

			new ATClutchInfo(container);
			new DummyGearboxInfo(container, new GearshiftPosition(0));
		}

		/// <summary>
		/// Builds a simple hybrid powertrain.
		///<code>
		/// (MeasuredSpeedDrivingCycle)
		///  └Vehicle
		///   └Wheels
		///    └SimpleHybridController
		///     └Brakes
		///      ├(Engine P4)
		///      └AxleGear
		///       ├(Engine P3)
		///       ├(Angledrive)
		///       ├(TransmissionOutputRetarder)
		///       └Gearbox, ATGearbox, or APTNGearbox
		///        ├(TransmissionInputRetarder)
		///        ├(Engine P2.5)
		///        ├(Engine P2)
		///        ├(SwitchableClutch)
		///        ├(Engine P1)
		///        └StopStartCombustionEngine
		///         └(Aux)
		/// </code>
		/// </summary>
		public static void BuildSimpleHybridPowertrain(VectoRunData data, VehicleContainer container)
		{
			var es = ConnectREESS(data, container);
			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);

			//IMPORTANT HINT: add engine BEFORE gearbox to container that gearbox can obtain if an ICE is available
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var gearbox = GetSimpleGearbox(container, data);
			if (!(gearbox is IHybridControlledGearbox gbx)) {
				throw new VectoException("Gearbox can not be used for parallel hybrid");
			}
			var ctl = new SimpleHybridController(container, es) { Gearbox = gbx, Engine = engine };
			var idleController = GetIdleController(data.PTO, engine, container);
			var clutch = (data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC) ? new SwitchableClutch(container, data.EngineData) : null;


			var vehicle = new Vehicle(container, data.VehicleData, data.AirdragData);

			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					container.AddComponent(new DistanceBasedDrivingCycle(container, data.Cycle));
					break;
				case CycleType.MeasuredSpeed:
					new MeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
					break;
				case CycleType.EngineOnly: break;
				default: throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

			vehicle.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2_5, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.IHPC, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);

			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				// this has to be done _after_ the powertrain is connected together so that the cluch already has its nextComponent set (necessary in the idle controlelr)
				if (gearbox is ATGearbox atGbx) {
					atGbx.IdleController = idleController;
					new ATClutchInfo(container);
				} else {
					clutch.IdleController = idleController;
				}
			}

			if (data.BusAuxiliaries != null) {
				if (!(container.BusAux is BusAuxiliariesAdapter busAux)) {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}

				var auxCfg = data.BusAuxiliaries;
				var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
					? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity, auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
					: (ISimpleBattery)new NoBattery(container);
				busAux.ElectricStorage = electricStorage;
				if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					var dcdc = new DCDCConverter(container, data.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency);
					busAux.DCDCConverter = dcdc;
					es.Connect(dcdc);
				}
			}
		}

		/// <summary>
		/// Builds a simple battery electric powertrain.
		/// <code>
		/// (Dummy MeasureSpeedDrivingCycle)
		/// └Vehicle
		///  └Wheels
		///   └Brakes
		///    └AxleGear
		///     └ATGearbox or Gearbox
		///      └Electric Motor
		/// </code>
		/// </summary>
		public static void BuildSimplePowertrainElectric(VectoRunData data, VehicleContainer container)
		{
			var es = ConnectREESS(data, container);
			var aux = new ElectricAuxiliary(container);
			aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
			es.Connect(aux);
			es.Connect(new SimpleCharger());

			var vehicle = new Vehicle(container, data.VehicleData, data.AirdragData);

			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					container.AddComponent(new DistanceBasedDrivingCycle(container, data.Cycle));
					break;
				case CycleType.MeasuredSpeed:
					new MeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
					break;
				case CycleType.EngineOnly:
					break;
				default:
					throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

			vehicle.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(data.AxleGearData is null ? null : new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(data.GearboxData is null ? null : GetSimpleGearbox(container, data))
				.AddComponent(GetElectricMachine(data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1,
					data.ElectricMachinesData, container, es, new DummyElectricMotorControl()));
			if (data.AxleGearData == null) {
				new DummyAxleGearInfo(container); // necessary for certain IEPC configurations
			}
		}

		private static ElectricSystem ConnectREESS(VectoRunData data, VehicleContainer container)
		{
			if (data.BatteryData != null && data.SuperCapData != null) {
				throw new VectoException("Powertrain requires either Battery OR SuperCapacitor, but both are defined.");
			}
			if (data.BatteryData is null && data.SuperCapData is null) {
				throw new VectoException("Powertrain requires either Battery OR SuperCapacitor, but none are defined.");
			}

			var es = new ElectricSystem(container);
			if (data.BatteryData != null) {
				if (data.BatteryData.InitialSoC < data.BatteryData.Batteries.Min(x => x.Item2.MinSOC)) {
					throw new VectoException("Battery: Initial SoC has to be higher than min SoC");
				}
				var battery = new BatterySystem(container, data.BatteryData);
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

			return es;
		}

		private static DrivingCycleData GetMeasuredSpeedDummyCycle() =>
			DrivingCycleDataReader.ReadFromStream((
				"<t>,<v>,<grad>\n" +
				"0, 50, 0\n" +
				"10, 50, 0").ToStream(), CycleType.MeasuredSpeed, "DummyCycle", false);

		private static IIdleController GetIdleController(PTOData pto, ICombustionEngine engine, IVehicleContainer container) =>
			pto?.PTOCycle is null
				? engine.IdleController
				: new IdleControllerSwitcher(engine.IdleController, new PTOCycleController(container, pto.PTOCycle));

		internal static IAuxInProvider CreateAdvancedAuxiliaries(VectoRunData data, IVehicleContainer container)
		{
			var conventionalAux = CreateAuxiliaries(data, container);
			// TODO: MQ 2019-07-30 -- which fuel map for advanced auxiliaries?!
			var busAux = new BusAuxiliariesAdapter(container, data.BusAuxiliaries, conventionalAux);
			var auxCfg = data.BusAuxiliaries;
			var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
				? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity, auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
				: (ISimpleBattery)new NoBattery(container);
			busAux.ElectricStorage = electricStorage;
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
					default:
						throw new ArgumentOutOfRangeException("AuxiliaryDemandType", auxData.DemandType.ToString());
				}
			}

			RoadSweeperAuxiliary rdSwpAux = null;
			PTODriveAuxiliary ptoDrive = null;

			if (data.ExecutionMode == ExecutionMode.Engineering && data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityRoadSweeping)) {
				if (data.DriverData.PTODriveMinSpeed == null) {
					throw new VectoSimulationException("PTO activity 'road sweeping' requested, but no min. engine speed or gear provided");
				}
				rdSwpAux = new RoadSweeperAuxiliary(container);
				aux.Add(Constants.Auxiliaries.IDs.PTORoadsweeping,
					(nEng, absTime, dt, dryRun) => rdSwpAux.PowerDemand(nEng, absTime, dt, dryRun),
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTORoadsweeping);
			}

			if (data.ExecutionMode == ExecutionMode.Engineering &&
				data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityWhileDrive)) {
				if (data.PTOCycleWhileDrive == null) {
					throw new VectoException("PTO activation while drive requested in cycle but no PTO cycle provided");
				}

				ptoDrive = new PTODriveAuxiliary(container, data.PTOCycleWhileDrive);
				aux.Add(Constants.Auxiliaries.IDs.PTODuringDrive,
					(nEng, absTime, dt, dryRun) => ptoDrive.PowerDemand(nEng, absTime, dt, dryRun),
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTODuringDrive);
			}
			if (data.PTO != null) {
				aux.AddConstant(Constants.Auxiliaries.IDs.PTOTransmission,
								DeclarationData.PTOTransmission.Lookup(data.PTO.TransmissionType).PowerDemand, Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOTransmission);

				aux.Add(Constants.Auxiliaries.IDs.PTOConsumer,
					(n, absTime, dt, dryRun) =>
						container.DrivingCycleInfo.PTOActive || (rdSwpAux?.Active(absTime) ?? false) ||
						(ptoDrive?.Active(absTime) ?? false)
							? null
							: data.PTO.LossMap.GetTorqueLoss(n) * n,
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOConsumer);
			}

			return aux;
		}

		internal static EngineAuxiliary CreateAuxiliariesSerialHybrid(VectoRunData data,
			IVehicleContainer container)
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
					default:
						throw new ArgumentOutOfRangeException("AuxiliaryDemandType", auxData.DemandType.ToString());
				}
			}

			return aux;
		}

		private static EngineAuxiliary CreateSpeedDependentAuxiliaries(VectoRunData data, IVehicleContainer container)
		{
			var aux = new EngineAuxiliary(container);
			var auxData = data.Aux.ToArray();
			AddSwitchingAux(aux, Constants.Auxiliaries.IDs.HeatingVentilationAirCondition, auxData);
			AddSwitchingAux(aux, Constants.Auxiliaries.IDs.SteeringPump, auxData);
			AddSwitchingAux(aux, Constants.Auxiliaries.IDs.ElectricSystem, auxData);
			AddSwitchingAux(aux, Constants.Auxiliaries.IDs.PneumaticSystem, auxData);
			return aux;
		}

		private static void AddSwitchingAux(EngineAuxiliary aux, string auxId, VectoRunData.AuxData[] auxData)
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
		}

		private static IGearbox GetGearbox(IVehicleContainer container, IShiftStrategy strategy = null)
		{
			strategy = strategy ?? GetShiftStrategy(container);
			switch (container.RunData.GearboxData.Type) {
				case GearboxType.AMT:
				case GearboxType.MT:
					return new Gearbox(container, strategy);
				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					new ATClutchInfo(container);
					return new ATGearbox(container, strategy);
				case GearboxType.APTN:
					return new APTNGearbox(container, strategy);
				case GearboxType.IHPC:
					return new APTNGearbox(container, strategy);
				default:
					throw new ArgumentOutOfRangeException("Unknown Gearbox Type", container.RunData.GearboxData.Type.ToString());
			}
		}

		public static IShiftStrategy GetShiftStrategy(IVehicleContainer container)
		{
			var runData = container.RunData;

			switch (runData.GearboxData.Type) {
				case GearboxType.AMT:
					switch (runData.JobType) {
						case VectoSimulationJobType.ConventionalVehicle:
						case VectoSimulationJobType.ParallelHybridVehicle:
							runData.ShiftStrategy = AMTShiftStrategyOptimized.Name;
							return new AMTShiftStrategyOptimized(container);
						case VectoSimulationJobType.BatteryElectricVehicle:
						case VectoSimulationJobType.SerialHybridVehicle:
							runData.ShiftStrategy = PEVAMTShiftStrategy.Name;
							return new PEVAMTShiftStrategy(container);
						default:
							throw new VectoException(
								"no default gearshift strategy available for gearbox type {0} and job type {1}",
								runData.GearboxData.Type, runData.JobType);
					}
				case GearboxType.MT:
					runData.ShiftStrategy = MTShiftStrategy.Name;
					return new MTShiftStrategy(container);

				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					switch (runData.JobType) {
						case VectoSimulationJobType.ParallelHybridVehicle:
						case VectoSimulationJobType.ConventionalVehicle:
							runData.ShiftStrategy = ATShiftStrategyOptimized.Name;
							return new ATShiftStrategyOptimized(container);
						case VectoSimulationJobType.SerialHybridVehicle:
						case VectoSimulationJobType.BatteryElectricVehicle:
							runData.ShiftStrategy = APTNShiftStrategy.Name;
							return new APTNShiftStrategy(container);
						default:
							throw new VectoException(
								"no default gearshift strategy available for gearbox type {0} and job type {1}",
								runData.GearboxData.Type, runData.JobType);
					}
				case GearboxType.APTN:
					switch (runData.JobType) {
						case VectoSimulationJobType.ParallelHybridVehicle:
						case VectoSimulationJobType.SerialHybridVehicle:
						case VectoSimulationJobType.BatteryElectricVehicle:
						case VectoSimulationJobType.IEPC_E:
						case VectoSimulationJobType.IEPC_S:
							runData.ShiftStrategy = APTNShiftStrategy.Name;
							return new APTNShiftStrategy(container);
						case VectoSimulationJobType.ConventionalVehicle when container.IsTestPowertrain:
							return null;
						default: 
							throw new ArgumentException("APT-N Gearbox is only applicable on hybrids and battery electric vehicles.");
					}
				case GearboxType.IHPC:
					switch (runData.JobType) {
						case VectoSimulationJobType.IHPC:
							runData.ShiftStrategy = AMTShiftStrategyOptimized.Name;
							return new AMTShiftStrategyOptimized(container);
						default:
							throw new ArgumentException("IHPC Gearbox is only applicable on hybrid vehicle of type IHPC.");
					}
				default:
					throw new ArgumentOutOfRangeException("GearboxType", runData.GearboxData.Type, "VECTO can not automatically derive shift strategy for GearboxType.");
			}
		}

		private static IGearbox GetSimpleGearbox(IVehicleContainer container, VectoRunData runData)
		{
			if (runData.GearboxData.Type.AutomaticTransmission() && runData.GearboxData.Type != GearboxType.APTN && runData.GearboxData.Type != GearboxType.IHPC) {
				new ATClutchInfo(container);
				return new ATGearbox(container, null);
			}
			return new Gearbox(container, null);
		}
	}

	public class SimpleCharger : IElectricChargerPort, IUpdateable
	{
		#region Implementation of IElectricChargerPort
		private Watt _chargingPower;
		public SimpleCharger() => _chargingPower = 0.SI<Watt>();
		public Watt Initialize() => _chargingPower = 0.SI<Watt>();
		public Watt PowerDemand(Second absTime, Second dt, Watt powerDemandEletricMotor, Watt auxPower, bool dryRun) => _chargingPower;
		#endregion

		#region Implementation of IUpdateable
		public bool UpdateFrom(object other)
		{
			if (other is IElectricSystemInfo es) {
				_chargingPower = es.ChargePower;
				return true;
			}

			if (other is Watt w) {
				_chargingPower = w;
				return true;
			}

			return false;
		}
		#endregion
	}

	internal class DummyEngineInfo : VectoSimulationComponent, IEngineInfo, IEngineControl
	{
		public DummyEngineInfo(VehicleContainer container) : base(container)
		{
			EngineIdleSpeed = 100.RPMtoRad();
			EngineSpeed = 100.RPMtoRad();
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
		}

		#endregion

		#region Implementation of IEngineInfo

		public PerSecond EngineSpeed { get; }
		public NewtonMeter EngineTorque { get; }
		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			return null;
		}

		public Watt EngineDynamicFullLoadPower(PerSecond avgEngineSpeed, Second dt)
		{
			return null;
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			return 0.SI<Watt>();
		}

		public Watt EngineAuxDemand(PerSecond avgEngineSpeed, Second dt)
		{
			return null;
		}

		public PerSecond EngineIdleSpeed { get; }
		public PerSecond EngineRatedSpeed { get; }
		public PerSecond EngineN95hSpeed { get; }
		public PerSecond EngineN80hSpeed { get; }
		public bool EngineOn { get; }

		#endregion

		#region Implementation of IEngineControl

		public bool CombustionEngineOn { get => false; set { } }

		#endregion
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
			return (-outTorque).LimitTo(maxDriveTorque ?? 0.SI<NewtonMeter>(), maxRecuperationTorque ?? VectoMath.Max(maxDriveTorque, 0.SI<NewtonMeter>()));
		}
	}

	public class GensetMotorController : IElectricMotorControl
	{
		public GensetMotorController(IVehicleContainer container, ElectricSystem es)
		{

		}

		#region Implementation of IElectricMotorControl

		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity,
			PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
			PowertrainPosition position, bool dryRun)
		{
			return EMTorque;
		}

		public NewtonMeter EMTorque { get; set; }

		#endregion
	}

	public class DummyElectricMotorControl : IElectricMotorControl
	{
		#region Implementation of IElectricMotorControl

		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque, PowertrainPosition position, bool dryRun)
		{
			return EmTorque;
		}

		public NewtonMeter EmTorque;

		#endregion
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

		public DrivingBehavior DriverBehavior => DrivingBehavior.Accelerating;

		public DrivingAction DrivingAction => DrivingAction.Accelerate;

		public MeterPerSquareSecond DriverAcceleration => 0.SI<MeterPerSquareSecond>();
		public PCCStates PCCState => PCCStates.OutsideSegment;
		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();

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

		public GearboxType GearboxType => GearboxType.DrivingCycle;

		public GearshiftPosition Gear => new GearshiftPosition(0);

		public bool TCLocked => true;

		public MeterPerSecond StartSpeed => throw new VectoException("No Gearbox available. StartSpeed unknown.");

		public MeterPerSquareSecond StartAcceleration => throw new VectoException("No Gearbox available. StartAcceleration unknown.");

		public Watt GearboxLoss()
		{
			throw new VectoException("No Gearbox available.");
		}

		public Second LastShift => throw new VectoException("No Gearbox available.");

		public Second LastUpshift => throw new VectoException("No Gearbox available.");

		public Second LastDownshift => throw new VectoException("No Gearbox available.");

		public GearData GetGearData(uint gear)
		{
			throw new VectoException("No Gearbox available.");
		}

		public GearshiftPosition NextGear => throw new VectoException("No Gearbox available.");

		public Second TractionInterruption => throw new NotImplementedException();

		public uint NumGears => throw new NotImplementedException();

		public bool DisengageGearbox => throw new VectoException("No Gearbox available.");

		public bool GearEngaged(Second absTime)
		{
			return true;
		}

		public bool RequestAfterGearshift { get; set; }

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

		public Meter Distance => 0.SI<Meter>();

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

		public MeterPerSecond VehicleSpeed => 0.SI<MeterPerSecond>();

		public bool VehicleStopped => throw new NotImplementedException();

		public Kilogram VehicleMass => throw new NotImplementedException();

		public Kilogram VehicleLoading => throw new NotImplementedException();

		public Kilogram TotalMass => throw new NotImplementedException();

		public CubicMeter CargoVolume => throw new NotImplementedException();

		public Newton AirDragResistance(MeterPerSecond previousVelocity, MeterPerSecond nextVelocity)
		{
			throw new NotImplementedException();
		}

		public Newton RollingResistance(Radian gradient)
		{
			throw new NotImplementedException();
		}

		public Newton SlopeResistance(Radian gradient)
		{
			throw new NotImplementedException();
		}

		public MeterPerSecond MaxVehicleSpeed => throw new NotImplementedException();

		#endregion
	}
}