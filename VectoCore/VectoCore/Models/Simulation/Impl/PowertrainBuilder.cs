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
using System.ComponentModel;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Auxiliaries;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
    /// <summary>
    /// Provides Methods to build a simulator with a powertrain step by step.
    /// </summary>
    public class PowertrainBuilder : PowertrainBuilderBase, IPowertrainBuilder
	{
		private readonly Dictionary<CycleType, Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>> _builders;
		
		private readonly Dictionary<PowertrainPosition, Func<VectoRunData, IVehicleContainer, IElectricSystem, IPowerTrainComponent, IElectricMotor>> _MeasuredSpeedBEVBuilders;
		private readonly Dictionary<PowertrainPosition, Func<VectoRunData, IVehicleContainer, IElectricSystem, IPWheelCycle, IElectricMotor>> _PWheelBEVBuilders;

		public PowertrainBuilder(IPowertrainComponentFactory componentFactory, IShiftStrategyFactory shiftStrategyFactory) : base(componentFactory, shiftStrategyFactory)
		{

			var distanceBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildFullPowertrainConventional },
				{ VectoSimulationJobType.ParallelHybridVehicle, BuildFullPowertrainParallelHybrid },
				{ VectoSimulationJobType.IHPC, BuildFullPowertrainParallelHybrid },
				{ VectoSimulationJobType.SerialHybridVehicle, BuildFullPowertrainSerialHybrid },
				{ VectoSimulationJobType.BatteryElectricVehicle, BuildFullPowertrainBatteryElectric },
				{ VectoSimulationJobType.EngineOnlySimulation, BuildEngineOnly },
				{ VectoSimulationJobType.IEPC_E, BuildFullPowertrainIEPCE },
				{ VectoSimulationJobType.IEPC_S, BuildFullPowertrainIEPCSerial },
				{ VectoSimulationJobType.FCHV, BuildFullPowertrainFCHV },
				{ VectoSimulationJobType.FCHV_IEPC, BuildFullPowertrainFCHV_IEPC }
			};

			var pWheelBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildPWheelConventional },
				{ VectoSimulationJobType.BatteryElectricVehicle, BuildPWheelBatteryElectric },
				{ VectoSimulationJobType.IEPC_E, BuildPWheelBatteryElectric }
			};
			
			var measuredSpeedGearBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildMeasuredSpeedGearConventional },
				{ VectoSimulationJobType.BatteryElectricVehicle, BuildMeasuredSpeedGearBatteryElectric },
				{ VectoSimulationJobType.IEPC_E, BuildMeasuredSpeedGearIEPC },
				{ VectoSimulationJobType.ParallelHybridVehicle, BuildMeasuredSpeedGearHybrid }
			};
			
			var measuredSpeedBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildMeasuredSpeedConventional },
				{ VectoSimulationJobType.BatteryElectricVehicle, BuildMeasuredSpeedBatteryElectric },
				{ VectoSimulationJobType.IEPC_E, BuildMeasuredSpeedBatteryElectric },
				{ VectoSimulationJobType.ParallelHybridVehicle, BuildMeasuredSpeedHybrid }
			};
			
			var vtpBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildVTPConventional } 
			};
			
			var engineOnlyBuilders =  new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildEngineOnly },
				{ VectoSimulationJobType.EngineOnlySimulation, BuildEngineOnly }
			};
			
			_builders = new Dictionary<CycleType, Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>>()
			{
				{ CycleType.DistanceBased, distanceBuilders	},
				{ CycleType.PWheel, pWheelBuilders },
				{ CycleType.MeasuredSpeed, measuredSpeedBuilders },
				{ CycleType.MeasuredSpeedGear, measuredSpeedGearBuilders }, 
				{ CycleType.VTP, vtpBuilders },
				{ CycleType.EngineOnly, engineOnlyBuilders }
			};

			_MeasuredSpeedBEVBuilders = new Dictionary<PowertrainPosition, Func<VectoRunData, IVehicleContainer, IElectricSystem, IPowerTrainComponent, IElectricMotor>>()
			{
				{ PowertrainPosition.BatteryElectricE2, BuildMeasuredSpeedForE2 },
				{ PowertrainPosition.BatteryElectricE3, BuildMeasuredSpeedForE3 },
				{ PowertrainPosition.BatteryElectricE4, BuildMeasuredSpeedForE4 },
				{ PowertrainPosition.IEPC, BuildMeasuredSpeedForIEPC }
			};
			
			_PWheelBEVBuilders = new Dictionary<PowertrainPosition, Func<VectoRunData, IVehicleContainer, IElectricSystem, IPWheelCycle, IElectricMotor>>()
			{
				{ PowertrainPosition.BatteryElectricE2, BuildPWheelForE2 },
				{ PowertrainPosition.BatteryElectricE3, BuildPWheelForE3 },
				{ PowertrainPosition.BatteryElectricE4, BuildPWheelForE4 },
				{ PowertrainPosition.IEPC, BuildPWheelForIEPC }
			};
			
			
        }

		// todo amogoda: m9. understand how declaration fc and engineering fc work together. should fc engineering be deprecated.
		// todo amogoda: m9. implement IFuelCellPort.
		private IVehicleContainer BuildFullPowertrainFCHV_IEPC(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			var container = BuildFullPowertrainIEPCE(data, modData, sumWriter);

			ConnectFuelCellSystem(container.ElectricSystemInfo as ElectricSystem, data, container);

			return container;
		}

		private IVehicleContainer BuildFullPowertrainFCHV(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			var container = BuildFullPowertrainBatteryElectric(data, modData, sumWriter);
			var es = container.ElectricSystemInfo as ElectricSystem;
			ConnectFuelCellSystem(es, data, container);

			return container;
		}

		public IVehicleContainer Build(VectoRunData data, IModalDataContainer modData, ISumData sumWriter = null)
		{
			var cycleType = data.Cycle.CycleType;	
			var jobType = data.JobType;

			if (!_builders.ContainsKey(cycleType) || !_builders[cycleType].ContainsKey(jobType)) {
				throw new ArgumentException($"Powertrain Builder: cannot build {cycleType} powertrain for job type: {jobType}");
            }

			return _builders[cycleType][jobType].Invoke(data, modData, sumWriter);
		}

		public IExemptedVehicleContainer BuildExempted(VectoRunData data)
		{
			var container = ComponentFactory.CreateExemptedVehicleContainer(data, null, null);
			//var container = _vehicleContainerFactory.CreateExemptedVehicleContainer(data, null, null);
			return container;
		}

        /// <summary>
        /// Builds an engine only powertrain.
        /// <code>
        /// PowertrainDrivingCycle
        /// └StopStartCombustionEngine
        ///  └(Aux)
        /// </code>
        /// </summary>
		private IVehicleContainer BuildEngineOnly(VectoRunData data, IModalDataContainer modData, ISumData _sumWriter)
		{
			if (_sumWriter == null)
				throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.EngineOnly) {
				throw new VectoException("CycleType must be EngineOnly.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, _sumWriter);
			
			var cycle = ComponentFactory.CreatePowertrainDrivingCycle(container, data.Cycle);
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData);
			var directAux = ComponentFactory.CreateEngineAuxiliary(container);

			cycle.InPort().Connect(engine.OutPort());
			engine.Connect(directAux.Port());
			directAux.AddCycle(Constants.Auxiliaries.Cycle);

			ComponentFactory.CreateDummyGearboxInfo(true, container);
			ComponentFactory.CreateDummyMileageCounter(container);
			ComponentFactory.CreateDummyDriverInfo(container);

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
        private IVehicleContainer BuildPWheelConventional(VectoRunData data, IModalDataContainer modData, ISumData _sumWriter)
		{
			if (_sumWriter == null)
				throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.PWheel) {
				throw new VectoException("CycleType must be PWheel.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, _sumWriter);

			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData, pt1Disabled: true);
			ComponentFactory.CreatePWheelCycle(container, data.Cycle)
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type, container, null))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(ComponentFactory.CreateClutch(data.JobType, container, data.EngineData))
				.AddComponent(engine, GetIdleController(data.PTO, engine, container));
			AddAuxiliaries(engine, container, data);

			ComponentFactory.CreateDummyMileageCounter(container);
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
		private IVehicleContainer BuildVTPConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.VTP) {
				throw new VectoException("CycleType must be VTP.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData, true);
			ComponentFactory.CreateVTPCycle(container, data.Cycle)
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type, container, null))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(ComponentFactory.CreateClutch(data.JobType, container, data.EngineData))
				.AddComponent(engine, engine.IdleController);

			ComponentFactory.CreateDummyMileageCounter(container);

			if (data.VehicleData.VehicleCategory.IsLorry()) {
				AddVTPTruckAuxiliaries(data, container, engine);
			} else if (data.VehicleData.VehicleCategory.IsBus()) {
				AddVTPBusAuxiliaries(data, container, engine);
			}

			return container;
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
        private IVehicleContainer BuildMeasuredSpeedConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeed) {
				throw new VectoException("CycleType must be MeasuredSpeed.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
			
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData);
			ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(GetGearbox(container))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? ComponentFactory.CreateClutch(data.JobType, container, data.EngineData) : null)
				.AddComponent(engine, GetIdleController(data.PTO, engine, container));
			AddAuxiliaries(engine, container, data);
			return container;
        }

		private IVehicleContainer BuildMeasuredSpeedGearHybrid(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			VerifyCycleType(data, CycleType.MeasuredSpeedGear);
			
			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
			
			var es = ConnectREESS(data, container);

			var isATTransmission =
				!(data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC);
			var strategy = ComponentFactory.CreateHybridStrategy(data.JobType, data.Cycle.CycleType, isATTransmission, data, container);
			
			// add engine before gearbox so that gearbox can obtain if an ICE is available already in constructor
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData);
			var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type,
				container, null);
			
			var ctl = ComponentFactory.CreateHybridController(data.Cycle.CycleType, container, strategy, es);
			ctl.Gearbox = gearbox as IHybridControlledGearbox;
			ctl.Engine = engine;

			var position = data.ElectricMachinesData[0].Item1;

			TimeRunHybridComponents components = new TimeRunHybridComponents() 
			{
				Cycle = ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, data.Cycle),
				Engine = engine,
				Gearbox =  gearbox,
				Clutch = AddClutch(data, container),
				IdleController = GetIdleController(data.PTO, engine, container),
				ElectricMotor = GetElectricMachine<MeasuredSpeedGearHybridsElectricMotor>(position, data.ElectricMachinesData, container, es, ctl),
				HybridController = ctl
			};

			_timerunGearHybridBuilders[position].Invoke(data, container, components);

			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);

            AddHighVoltageAuxiliaries(data, container, es, dcdc);
			AddHybridBusAuxiliaries(data, container, es, dcdc);

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
		private IVehicleContainer BuildMeasuredSpeedGearConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeedGear) {
				throw new VectoException("CycleType must be MeasuredSpeed with Gear.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData);
			ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type, container, null))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? ComponentFactory.CreateClutch(data.JobType, container, data.EngineData) : null)
				.AddComponent(engine);
			AddAuxiliaries(engine, container, data);

			ComponentFactory.CreateATClutchInfo(container);
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
		private IVehicleContainer BuildFullPowertrainConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			
			var cycle = ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData);
			var idleController = GetIdleController(data.PTO, engine, container);
			cycle.IdleController = idleController as IdleControllerSwitcher;

			var gearbox = GetGearbox(container);

			cycle.AddComponent(ComponentFactory.CreateDriver(container, data.DriverData, ComponentFactory.CreateDriverStrategy(container)))
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? ComponentFactory.CreateClutch(data.JobType, container, data.EngineData) : null)
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			if (gearbox is IAPTGearbox atGbx) {
				atGbx.IdleController = idleController;
			}

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
		private IVehicleContainer BuildFullPowertrainParallelHybrid(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.SavedInDeclarationMode) {
				throw new NotImplementedException();
			}
			//if (sumWriter == null)
			//	throw new ArgumentNullException(nameof(sumWriter));
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

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			var es = ConnectREESS(data, container);

			IClutch clutch = null;
			
			var isAtTransmission = !(data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC);
			if (!isAtTransmission) {
				clutch = ComponentFactory.CreateClutch(data.JobType, container, data.EngineData);
            }
			var strategy = ComponentFactory.CreateHybridStrategy(data.JobType, data.Cycle.CycleType, isAtTransmission, data, container);
			var ctl = ComponentFactory.CreateHybridController(data.Cycle.CycleType, container, strategy, es);
			
            // add engine before gearbox in the container, that gearbox can obtain it
            var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData);

			var gearbox = GetGearbox(container, ctl.ShiftStrategy);
			if (!(gearbox is IHybridControlledGearbox gbx)) {
				throw new VectoException($"Gearbox can not be used for parallel hybrid: ${gearbox?.GetType()}");
			}

			ctl.Gearbox = gbx;
			ctl.Engine = engine;

			if ((data.SuperCapData != null || data.BatteryData != null) && data.EngineData.WHRType.IsElectrical()) {
				var dcDcConverterEfficiency = DeclarationData.WHRChargerEfficiency;
				var whrCharger = ComponentFactory.CreateWHRCharger(container, dcDcConverterEfficiency);
				es.Connect(whrCharger);
				engine.WHRCharger = whrCharger;
			}

			var cycle = ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);
			var idleController = GetIdleController(data.PTO, engine, container);
			cycle.IdleController = idleController as IIdleControllerSwitcher;
			cycle.AddComponent(ComponentFactory.CreateDriver(container, data.DriverData, ComponentFactory.CreateDriverStrategy(container)))
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2_5, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.IHPC, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				// this has to be done _after_ the powertrain is connected together so that the cluch already has its nextComponent set (necessary in the idle controlelr)
				if (gearbox is IAPTGearbox atGbx) {
					atGbx.IdleController = idleController;
				} else {
					clutch.IdleController = idleController;
				}
			}

			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);

            if (data.BusAuxiliaries != null) {
				if (container.BusAux is BusAuxiliariesAdapter busAux) {
					var auxCfg = data.BusAuxiliaries;
					var electricStorage = ComponentFactory.CreateSimpleBattery(
							auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart, container,
							auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity,
							auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency);
					busAux.ElectricStorage = electricStorage;
					if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
						busAux.DCDCConverter = dcdc;
						es.Connect(dcdc);
					}
				} else {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}
			}
						
			AddHighVoltageAuxiliaries(data, container, es, dcdc);

			///TODO: remove
			data.ElectricAuxDemand = 0.SI<Watt>();

			return container;
		}

        private IVehicleContainer BuildMeasuredSpeedHybrid(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			VerifyCycleType(data, CycleType.MeasuredSpeed);

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
			var es = ConnectREESS(data, container);

			var clutch = AddClutch(data, container);

			var isATTransmission = !(data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC);
			var strategy = ComponentFactory.CreateHybridStrategy(data.JobType, data.Cycle.CycleType, isATTransmission, data, container);

            var ctl = ComponentFactory.CreateHybridController(data.Cycle.CycleType, container, strategy, es);

			// add engine before gearbox so that gearbox can obtain if an ICE is available already in constructor
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData);
			var gearbox = GetGearbox(container, ctl.ShiftStrategy);

			var idleController = GetIdleController(data.PTO, engine, container);
			
			ctl.Gearbox = gearbox as IHybridControlledGearbox;
			ctl.Engine = engine;

			var cycle = ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, data.Cycle);

			cycle
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2_5, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.IHPC, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			SetIdleControllerForHybridP1(data, gearbox, idleController, clutch);

			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddHighVoltageAuxiliaries(data, container, es, dcdc);
			AddHybridBusAuxiliaries(data, container, es, dcdc);

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
		private IVehicleContainer BuildFullPowertrainSerialHybrid(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			//if (sumWriter == null)
			//	throw new ArgumentNullException(nameof(sumWriter));
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 == PowertrainPosition.GEN) != 1) {
				throw new VectoException("SerialHybrid needs exactly one GEN set.");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 != PowertrainPosition.GEN) != 1) {
				throw new VectoException("SerialHybrid needs exactly one electric motor.");
			}

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			
			var es = ConnectREESS(data, container);
			
			var atTransmission = data.GearboxData != null && data.GearboxData.Type.AutomaticTransmission();
			var strategy = ComponentFactory.CreateHybridStrategy(data.JobType, data.Cycle.CycleType, atTransmission,
				data, container);

			var ctl = ComponentFactory.CreateSerialHybridController(data.Cycle.CycleType, container, strategy,
				es); 
			
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData);

			var idleController = engine.IdleController;
			ctl.Engine = engine;

			if ((data.SuperCapData != null || data.BatteryData != null) && data.EngineData.WHRType.IsElectrical()) {
				var dcDcConverterEfficiency = DeclarationData.WHRChargerEfficiency;
				var whrCharger = ComponentFactory.CreateWHRCharger(container, dcDcConverterEfficiency);
				es.Connect(whrCharger);
				engine.WHRCharger = whrCharger;

			}

			var cycle = ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(ComponentFactory.CreateDriver(container, data.DriverData, ComponentFactory.CreateDriverStrategy(container)))
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData));

			var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			switch (pos) {
				case PowertrainPosition.BatteryElectricE4:
					//-->Engine E4
					powertrain.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE4,
						data.ElectricMachinesData, container, es, ctl));
					ComponentFactory.CreateDummyGearboxInfo(false, container, new GearshiftPosition(0));
					ComponentFactory.CreateDummyAxleGearInfo(container);
					ComponentFactory.CreateATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE3:
					//-->AxleGear-->(AxlegearInputRetarder)-->Engine E3
					powertrain
						.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
						.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl));
					ComponentFactory.CreateDummyGearboxInfo(false, container, new GearshiftPosition(0));
					ComponentFactory.CreateATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE2:
                    //-->AxleGear-->(AngleDrive)-->(TransmissionOutputRetarder)-->PEVGearbox or APTNGearbox-->(TransmissionInputRetarder)-->Engine E2
					var shiftStrategy = ShiftStrategyFactory.GetShiftStrategy(data.ShiftStrategy, container);
					var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type, container, shiftStrategy);

					powertrain
						.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
						.AddComponent(gearbox)
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
						.AddComponent(GetPEVPTO(container, data))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE2,
							data.ElectricMachinesData, container, es, ctl));
					ComponentFactory.CreateATClutchInfo(container);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for serial hybrid vehicle.");
			}

			ctl.GenSet.AddComponent(GetElectricMachine(PowertrainPosition.GEN, data.ElectricMachinesData, container, es,
					ctl))
				.AddComponent(engine, idleController);
			AddAuxiliariesSerialHybrid(engine,container, data);

			var dcdc = ComponentFactory.CreateDCDCConverter(container,
				data.DCDCData.DCDCEfficiency);
            if (data.BusAuxiliaries != null) {
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					throw new VectoException("BusAux must be supplied from REESS!");
				}
                if (container.BusAux is BusAuxiliariesAdapter busAux) {
					var auxCfg = data.BusAuxiliaries;
					var electricStorage = ComponentFactory.CreateSimpleBattery(
						auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart, container,
						auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity,
						auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency);
					busAux.ElectricStorage = electricStorage;
					if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
						busAux.DCDCConverter = dcdc;
						es.Connect(dcdc);
					}
				} else {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}
			}

			AddElectricAuxiliaries(data, container, es, cycle, dcdc);

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
		private IVehicleContainer BuildFullPowertrainBatteryElectric(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
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
			
			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			
			var es = ConnectREESS(data, container);

			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);

			var cycle = ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);

			var powertrain = cycle
				.AddComponent(ComponentFactory.CreateDriver(container, data.DriverData, ComponentFactory.CreateDriverStrategy(container)))
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData));

			var pos = data.ElectricMachinesData.First().Item1;
			IElectricMotor em;
			switch (pos) {
				case PowertrainPosition.BatteryElectricE4:
					//-->Engine E4
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);
					powertrain.AddComponent(em);
					ComponentFactory.CreateDummyGearboxInfo(false, container);
					ComponentFactory.CreateDummyAxleGearInfo(container);
					ComponentFactory.CreateATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE3:
					//-->AxleGear-->(AxlegearInputRetarder)-->Engine E3
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);
					powertrain
						.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
						.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
						.AddComponent(em);
					ComponentFactory.CreateDummyGearboxInfo(false, container);
					ComponentFactory.CreateATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE2:
					var shiftStrategy = ShiftStrategyFactory.GetShiftStrategy(data.ShiftStrategy, container);
					var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType,
						data.GearboxData.Type, container, shiftStrategy);
					//-->AxleGear-->(Angledrive)-->(TransmissionOutputRetarder)-->APTNGearbox or PEVGearbox-->(TransmissionInputRetarder)-->Engine E2
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData, container, es, ctl);
					powertrain
						.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
						.AddComponent(gearbox)
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
						.AddComponent(GetPEVPTO(container, data))
						.AddComponent(em);

					ComponentFactory.CreateATClutchInfo(container);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for BatteryElectric Vehicle");
			}

			ComponentFactory.CreateDummyEngineInfo(container);

			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
            if (data.BusAuxiliaries != null) {
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;
				var busAux = ComponentFactory.CreateBusAuxiliariesAdapter(container, auxCfg);
				var electricStorage = ComponentFactory.CreateSimpleBattery(false, container, null, double.NaN);
				busAux.ElectricStorage = electricStorage;
	
				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			} 

			AddElectricAuxiliaries(data, container, es, cycle, dcdc);
			//TODO: Just add to BatteryElectric???
			//Move to FCHV

			return container;

		}

		//private static void ConnectFuelCellSystem(ElectricSystem es, FuelCellSystemData fcSystemData, IVehicleContainer container)
		private static void ConnectFuelCellSystem(ElectricSystem es, VectoRunData runData, IVehicleContainer container)
		{
			if(runData.FuelCellSystemData == null)
			{
				return;
			}

			//if(runData.FuelCellSystemData.FuelCellPowerMap == null)
			//{
			//	runData.FuelCellSystemData.SetFuelCellSystemPowerMap(runData.BatteryData, container.ModalData);
			//}
			
			//if (runData.FuelCellSystemData.FuelCellShareMap == null)
			//{
			//	runData.FuelCellSystemData.SetFuelCellSystemSharedMap();
			//}

			var id = 1;
			var fuelCellSystem = new FuelCellSystem(runData.FuelCellSystemData, container);
			foreach (var fuelCell in runData.FuelCellSystemData.FuelCellStrings) {
				var fcs = new FuelCellString(fuelCell, id++, dataBus:container);
				fuelCellSystem.AddFuelCellString(fcs);
			}
				
			es.Connect(fuelCellSystem);
		}
		
		private IVehicleContainer BuildPWheelBatteryElectric(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
        {
			VerifyCycleType(data, CycleType.PWheel);
			ValidateBatteryElectric(data);

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			
			var es = ConnectREESS(data, container);
			
			var powertrain = ComponentFactory.CreatePWheelCycle(container, data.Cycle);
			
			var position = data.ElectricMachinesData.First().Item1;
			
			IElectricMotor em = _PWheelBEVBuilders[position].Invoke(data, container, es, powertrain);

			var dcdc = ComponentFactory.CreateDCDCConverter(container, GetDCDCEfficiency(data));
            if (data.BusAuxiliaries != null) {
				AddBEVBusAuxiliaries(data, container, es, em, dcdc);
			}
			else {
				AddElectricAuxiliaries(data, container, es, null, dcdc);
			}

			return container;
        }

        private IElectricMotor BuildPWheelForIEPC(VectoRunData data, IVehicleContainer container, IElectricSystem es, IPWheelCycle cycle)
        { 
			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);
			
			IElectricMotor em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);

			ITnInProvider powertrain = cycle;

			cycle.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData));

			if (data.AxleGearData != null) {
				powertrain = cycle
					.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
					.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container));
            }

			powertrain
				.AddComponent(ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type, container, null))
				.AddComponent(em);

			ComponentFactory.CreateATClutchInfo(container);
			ComponentFactory.CreateDummyEngineInfo(container);

			if (data.AxleGearData == null) {
				ComponentFactory.CreateDummyAxleGearInfo(container);
			}

			return em;
		}

        private IElectricMotor BuildPWheelForE2(VectoRunData data, IVehicleContainer container, IElectricSystem es, IPWheelCycle powertrain)
		{
			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);
			
			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData, container, es, ctl);
			
			powertrain
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type, container, null))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetPEVPTO(container, data))
				.AddComponent(em);

			ComponentFactory.CreateATClutchInfo(container);
			ComponentFactory.CreateDummyEngineInfo(container);
			ComponentFactory.CreateDummyMileageCounter(container);

			return em;
        }

		private IElectricMotor BuildPWheelForE3(VectoRunData data, IVehicleContainer container, IElectricSystem es, IPWheelCycle powertrain)
		{
			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);

			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);
			
			powertrain
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(em);

			ComponentFactory.CreateDummyGearboxInfo(false, container);
			ComponentFactory.CreateATClutchInfo(container);
			ComponentFactory.CreateDummyEngineInfo(container);

			return em;
        }

		private IElectricMotor BuildPWheelForE4(VectoRunData data, IVehicleContainer container, IElectricSystem es, IPWheelCycle powertrain)
		{
			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);

			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);
			
			powertrain.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData));
			powertrain.AddComponent(em);
			ComponentFactory.CreateDummyGearboxInfo(false,container);
			ComponentFactory.CreateDummyAxleGearInfo(container);
			ComponentFactory.CreateATClutchInfo(container);

			return em;
        }

		private IVehicleContainer BuildMeasuredSpeedBatteryElectric(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
        {
			VerifyCycleType(data, CycleType.MeasuredSpeed);
			ValidateBatteryElectric(data);

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			
			var es = ConnectREESS(data, container);

			IPowerTrainComponent powertrain = ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData));

			var position = data.ElectricMachinesData.First().Item1;
			
			IElectricMotor em = _MeasuredSpeedBEVBuilders[position].Invoke(data, container, es, powertrain);

			
			var dcdc = ComponentFactory.CreateDCDCConverter(container,GetDCDCEfficiency(data));
            if (data.BusAuxiliaries != null) {
				AddBEVBusAuxiliaries(data, container, es, em, dcdc);
			}
			else {
				AddElectricAuxiliaries(data, container, es, null, dcdc);
			}

			return container;
        }

		private IElectricMotor BuildMeasuredSpeedForIEPC(VectoRunData data, IVehicleContainer container,IElectricSystem es, IPowerTrainComponent powertrain)
        { 
			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);

			var strategy = GetShiftStrategy(container);
			var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type,
				container, strategy);

			IElectricMotor em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);

			powertrain.AddComponent((data.AxleGearData != null) ? ComponentFactory.CreateAxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);

			ComponentFactory.CreateATClutchInfo(container);
			ComponentFactory.CreateDummyEngineInfo(container);

			if (data.AxleGearData == null) {
				ComponentFactory.CreateDummyAxleGearInfo(container);
			}

			return em;
		}

        private IElectricMotor BuildMeasuredSpeedForE2(VectoRunData data, IVehicleContainer container, IElectricSystem es, IPowerTrainComponent powertrain)
        {
			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);

			var strategy = ShiftStrategyFactory.GetShiftStrategy(data.ShiftStrategy, container);

			var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type,
				container, strategy);

			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData, container, es, ctl);

			powertrain.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetPEVPTO(container, data))
				.AddComponent(em);

			ComponentFactory.CreateATClutchInfo(container);
				
			return em;
        }

        private IElectricMotor BuildMeasuredSpeedForE3(VectoRunData data, IVehicleContainer container, IElectricSystem es, IPowerTrainComponent powertrain)
        { 
			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);
			
			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);

			powertrain.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(em);

			ComponentFactory.CreateDummyGearboxInfo(false, container);
			ComponentFactory.CreateATClutchInfo(container);
			ComponentFactory.CreateDummyEngineInfo(container);

			return em;
        }

        private IElectricMotor BuildMeasuredSpeedForE4(VectoRunData data, IVehicleContainer container, IElectricSystem es, IPowerTrainComponent powertrain)
        {
			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);
			
			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);

			powertrain.AddComponent(em);

			ComponentFactory.CreateDummyGearboxInfo(false, container);
			ComponentFactory.CreateDummyAxleGearInfo(container);
			ComponentFactory.CreateATClutchInfo(container);

			return em;
		}

        private void AddBEVBusAuxiliaries(VectoRunData data, IVehicleContainer container, IElectricSystem es, IElectricMotor em, IDCDCConverter dcdc)
        {
			if (data.BusAuxiliaries == null) {
				return;
			}
			
			if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
				throw new VectoException("BusAux must be supplied from REESS!");
			}

			var busAux = ComponentFactory.CreateBusAuxiliariesAdapter(container, data.BusAuxiliaries);			


			busAux.DCDCConverter = dcdc;
			busAux.ElectricStorage = ComponentFactory.CreateSimpleBattery(false, container, null, double.NaN);

			es.Connect(dcdc);
			em.BusAux = busAux;
		}

        private IVehicleContainer BuildMeasuredSpeedGearBatteryElectric(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
        {
			VerifyCycleType(data, CycleType.MeasuredSpeedGear);
			ValidateBatteryElectric(data);

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			
			var es = ConnectREESS(data, container);
			
			var position = data.ElectricMachinesData.First().Item1;

			if (position != PowertrainPosition.BatteryElectricE2) {
				throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }

			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);
			IElectricMotor em = GetElectricMachine(position, data.ElectricMachinesData, container, es, ctl);

			var timeBasedCycle = ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, data.Cycle);
			
			timeBasedCycle
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type, container, null))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetPEVPTO(container, data))
				.AddComponent(em);

			ComponentFactory.CreateATClutchInfo(container);
			ComponentFactory.CreateDummyEngineInfo(container);

			var dcdc = ComponentFactory.CreateDCDCConverter(container, GetDCDCEfficiency(data));
            if (data.BusAuxiliaries != null) {
				AddBEVBusAuxiliaries(data, container, es, em, dcdc);
			}
			else {
				AddElectricAuxiliaries(data, container, es, null, dcdc);
			}

			return container;
		}

		private IVehicleContainer BuildMeasuredSpeedGearIEPC(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
        {
			VerifyCycleType(data, CycleType.MeasuredSpeedGear);
			ValidateBatteryElectric(data);

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			
			var es = ConnectREESS(data, container);
			
			var position = data.ElectricMachinesData.First().Item1;

			if (position != PowertrainPosition.IEPC) {
				throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }

			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);
			IElectricMotor em = GetElectricMachine(position, data.ElectricMachinesData, container, es, ctl);

			var timeBasedCycle = ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, data.Cycle);
			
			timeBasedCycle
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(data.AxleGearData != null ? ComponentFactory.CreateAxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type, container, null))
				.AddComponent(em);

			ComponentFactory.CreateATClutchInfo(container);
			ComponentFactory.CreateDummyEngineInfo(container);

			if (data.AxleGearData == null) {
				ComponentFactory.CreateDummyAxleGearInfo(container);
			}

			var dcdc = ComponentFactory.CreateDCDCConverter(container, GetDCDCEfficiency(data));
            if (data.BusAuxiliaries != null) {
				AddBEVBusAuxiliaries(data, container, es, em, dcdc);
			}
			else {
				AddElectricAuxiliaries(data, container, es, null,dcdc);
			}

			return container;
		}

		private void ValidateBatteryElectric(VectoRunData data)
        {
			if (data.ElectricMachinesData.Count > 1) {
				throw new VectoException("Electric motors on multiple positions not supported");
			}

			if (data.BatteryData != null && data.SuperCapData != null) {
				throw new VectoException("Only one REESS is supported.");
			}

			var position = data.ElectricMachinesData.First().Item1;

			if (position == PowertrainPosition.HybridPositionNotSet) {
				throw new VectoException("invalid powertrain position");
            }

			if (position == PowertrainPosition.HybridP0 ||
				position == PowertrainPosition.HybridP1 ||
				position == PowertrainPosition.HybridP2 ||
				position == PowertrainPosition.HybridP3 ||
				position == PowertrainPosition.HybridP4) {

				throw new VectoException("BatteryElectric Vehicle does not support parallel powertrain configurations");
			}
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
		private IVehicleContainer BuildFullPowertrainIEPCE(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
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

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			
			var es = ConnectREESS(data, container);

			var ctl = ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, es);

			var cycle = ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(ComponentFactory.CreateDriver(container, data.DriverData, ComponentFactory.CreateDriverStrategy(container)))
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData));

			var pos = data.ElectricMachinesData.First().Item1;
			IElectricMotor em;
			if (pos != PowertrainPosition.IEPC) {
				throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for BatteryElectric Vehicle");
			}
			
			//-->AxleGear-->APTNGearbox or SinglespeedGearbox-->Engine E2
			var shiftstrategy = GetShiftStrategy(container);
			var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type,
				container, shiftstrategy);

			em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);
			powertrain
				.AddComponent(data.AxleGearData != null ? ComponentFactory.CreateAxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);

			ComponentFactory.CreateATClutchInfo(container);
			if (data.AxleGearData == null) {
				ComponentFactory.CreateDummyAxleGearInfo(container);
			}
			ComponentFactory.CreateDummyEngineInfo(container);

			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
			if (data.BusAuxiliaries != null) {
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;
				var busAux = ComponentFactory.CreateBusAuxiliariesAdapter(container, auxCfg);
				var electricStorage = ComponentFactory.CreateSimpleBattery(false, container, null, double.NaN);
				busAux.ElectricStorage = electricStorage;

				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			}

			AddElectricAuxiliaries(data, container, es, cycle, dcdc);

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
		private IVehicleContainer BuildFullPowertrainIEPCSerial(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			//if (_sumWriter == null)
			//	throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 == PowertrainPosition.GEN) != 1) {
				throw new VectoException("IEPC vehicle needs exactly one GEN set.");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 != PowertrainPosition.GEN) != 1) {
				throw new VectoException("IEPC vehicle needs exactly one electric motor.");
			}

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			
			var es = ConnectREESS(data, container);

			var strategy = ComponentFactory.CreateHybridStrategy(data.JobType, data.Cycle.CycleType, false, data, container);
			var ctl = ComponentFactory.CreateSerialHybridController(data.Cycle.CycleType, container, strategy, es);

			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, data.EngineData);

			var idleController = engine.IdleController;
			ctl.Engine = engine;

            if (data.BusAuxiliaries != null)
            {
                var aux = new HighVoltageElectricAuxiliary(container);
                aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
                es.Connect(aux);
            }
                
			var cycle = ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(ComponentFactory.CreateDriver(container, data.DriverData, ComponentFactory.CreateDriverStrategy(container)))
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData));

			var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			IElectricMotor em;
			if (pos != PowertrainPosition.IEPC) {
				throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for BatteryElectric Vehicle");
			}

			//-->AxleGear-->APTNGearbox or SinglespeedGearbox-->Engine E2
			var shiftStrategy = GetShiftStrategy(container);
			var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxData.Type,
				container, shiftStrategy);
			
			em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);
			powertrain
				.AddComponent(data.AxleGearData != null ? ComponentFactory.CreateAxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);

			ComponentFactory.CreateATClutchInfo(container);
			if (data.AxleGearData == null) {
				ComponentFactory.CreateDummyAxleGearInfo(container);
			}

			ctl.GenSet.AddComponent(GetElectricMachine(PowertrainPosition.GEN, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
            if (data.BusAuxiliaries != null) {
				if (container.BusAux is BusAuxiliariesAdapter busAux) {
					if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
						throw new VectoException("BusAux must be supplied from REESS!");
					}

					var auxCfg = data.BusAuxiliaries;
					var electricStorage = ComponentFactory.CreateSimpleBattery(false, container, null, double.NaN);
					busAux.ElectricStorage = electricStorage;
					busAux.DCDCConverter = dcdc;
					es.Connect(dcdc);
					//em.BusAux = busAux;
				}
			} else {
                AddElectricAuxiliaries(data, container, es, cycle,dcdc);
			}
						
			return container;
		}

		protected IVehicleContainer GetVehicleContainer(ExecutionMode mode, VectoRunData runData, IModalDataContainer modData, ISumData sumWriter)
		{
			var container = ComponentFactory.CreateVehicleContainer(runData, modData, sumWriter);
			return container;
		}

		protected IGearbox GetGearbox(IVehicleContainer container, IShiftStrategy strategy = null)
		{
			strategy = strategy ?? GetShiftStrategy(container);

			if (container.RunData.GearboxData.Type.IsOneOf(GearboxType.ATSerial, GearboxType.ATPowerSplit)) {
				ComponentFactory.CreateATClutchInfo(container);
            }

			return ComponentFactory.CreateGearbox(container.RunData.JobType, container.RunData.Cycle.CycleType, container.RunData.GearboxData.Type, container,
				strategy);
		}

		// used for battery electric powertrains
		protected IElectricMotor GetElectricMachine(PowertrainPosition pos, IList<Tuple<PowertrainPosition,
				ElectricMotorData>> electricMachinesData, IVehicleContainer container, IElectricSystem es,
			IElectricMotorControl ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData is null) {
				return null;
			}

			var motor = ComponentFactory.CreateElectricMotor(pos == PowertrainPosition.IEPC, container, motorData.Item2, ctl, pos);
            motor.Connect(es);
			return motor;
		}

		// used for hybrid electric powertrains
		protected IElectricMotor GetElectricMachine(PowertrainPosition pos, IList<Tuple<PowertrainPosition,
				ElectricMotorData>> electricMachinesData, IVehicleContainer container, IElectricSystem es,
			IHybridController ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData is null) {
				return null;
			}

			ctl.AddElectricMotor(pos, motorData.Item2);
			var motor = ComponentFactory.CreateElectricMotor(pos == PowertrainPosition.IEPC, container, motorData.Item2, ctl.ElectricMotorControl(pos), pos);

            if (pos == PowertrainPosition.GEN) {
				es.Connect(ComponentFactory.CreateGensetChargerAdapter(motor));
			} else {
				motor.Connect(es);
			}

			return motor;
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

	interface IDummyEngineInfo : IEngineInfo {}

	internal class DummyEngineInfo : VectoSimulationComponent, IEngineInfo, IEngineControl, IDummyEngineInfo
	{
		public DummyEngineInfo(IVehicleContainer container) : base(container)
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
		public bool EngineOn { get; private set; }

		#endregion

		#region Implementation of IEngineControl

		public bool CombustionEngineOn { get => false; set { } }

		#endregion

		protected override bool DoUpdateFrom(object other)
		{
			if (other is IEngineInfo info) {
				EngineOn = info.EngineOn;
				return true;
			} else {
				return false;
			}

		}
	}

	internal class DummyDriverInfo : VectoSimulationComponent, IDriverInfo
	{
		public DummyDriverInfo(IVehicleContainer container) : base(container)
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
		public MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed) => targetSpeed;

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}

	internal class EngineOnlyGearboxInfo : VectoSimulationComponent, IGearboxInfo
	{
		public EngineOnlyGearboxInfo(IVehicleContainer container) : base(container)
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

		public IShiftStrategy Strategy => throw new VectoException("No Gearbox available.");

		public GearboxType GearboxType => GearboxType.NoGearbox;

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
		public bool Disengaged { get; }

		public bool DisengageGearbox => throw new VectoException("No Gearbox available.");

		public bool GearEngaged(Second absTime)
		{
			return true;
		}

		public bool RequestAfterGearshift { get; set; }

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}

	internal class ZeroMileageCounter : VectoSimulationComponent, IMileageCounter
	{
		public ZeroMileageCounter(IVehicleContainer container) : base(container)
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

		protected override bool DoUpdateFrom(object other) => false;
	}

	public class DummyVehicleInfo : VectoSimulationComponent, IVehicleInfo
	{
		public DummyVehicleInfo(IVehicleContainer container) : base(container)
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

		public bool VehicleStopped => false;

		public Kilogram VehicleMass => throw new NotImplementedException();

		public Kilogram VehicleLoading => throw new NotImplementedException();

		public Kilogram TotalMass => throw new NotImplementedException();

		public CubicMeter CargoVolume => throw new NotImplementedException();

		public AirDragLossResult AirDragResistance(MeterPerSecond previousVelocity, MeterPerSecond nextVelocity)
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

		protected override bool DoUpdateFrom(object other) => false;
	}

	public class TimeRunHybridComponents
    {
		internal IMeasuredSpeedDrivingCycle Cycle { get; set; }

		public ICombustionEngine Engine { get; set; }

		public IGearbox Gearbox { get; set; }

		public IClutch Clutch { get; set; }

		public IIdleController IdleController { get; set; }

		public IElectricMotor ElectricMotor	{ get; set; }

		public IHybridController HybridController { get; set; }
	}
}