using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;
using static TUGraz.VectoCore.Models.Declaration.DeclarationData;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{

    public class SimplePowertrainBuilder : PowertrainBuilderBase, ISimplePowertrainBuilder
    {
	    private delegate IElectricMotor PevAxlePowertrainBuilder(VectoRunData data, AxlePowertrainData axlePt, IElectricPowerJunctionBox junctionBox, IVehicleContainer container, IPowertrainContainer powertrainContainer, IPowerTrainComponent powertrain, IElectricSystem es);

	    private readonly Dictionary<PowertrainPosition, PevAxlePowertrainBuilder> _axlePowertrainPEVBuilders;

		
        public SimplePowertrainBuilder(IPowertrainComponentFactory componentFactory, IShiftStrategyFactory shiftStrategyFactory) : base(componentFactory, shiftStrategyFactory)
        {
            _axlePowertrainPEVBuilders = new Dictionary<PowertrainPosition, PevAxlePowertrainBuilder>()
            {
                { PowertrainPosition.BatteryElectricE4, CreateAxlePowertrainForE4 },
                { PowertrainPosition.BatteryElectricE3, CreateAxlePowertrainForE3 },
                { PowertrainPosition.BatteryElectricE2, CreateAxlePowertrainForE2 },
                { PowertrainPosition.IEPC, CreateAxlePowertrainForIEPC}
            };
        }

		public ITestPowertrain CreateTestPowertrain(IVehicleContainer realContainer, bool createDriver, VectoSimulationJobType overrideJobType, string torqueSplitterType)
		{
			var testContainer = BuildSimplePowertrain(realContainer, overrideJobType, torqueSplitterType);
			return new TestPowertrain(testContainer, realContainer, createDriver);
		}

		public ITestPowertrain CreateTestPowertrain(IVehicleContainer realContainer, bool createDriver, string torqueSplitterType)
		{
			var testContainer = BuildSimplePowertrain(realContainer, null, torqueSplitterType);
			return new TestPowertrain(testContainer, realContainer, createDriver);
		}

        public ITestGenset CreateTestGenset(IVehicleContainer realContainer)
		{
			var testContainer = BuildSimpleGenSet(realContainer);
			return new TestGenset(testContainer, realContainer);
		}

		public ISimplePowertrainContainer CreateSimplePowertrainContainer(ISimpleVehicleContainer simpleContainer,
			AxleNumber axleNumber, bool useSimpleTorqueSplitter = false)
		{
			return ComponentFactory.CreateSimplePowertrainContainer(simpleContainer, axleNumber, useSimpleTorqueSplitter);
		}

		public ISimplePowertrainContainer CreateSimplePowertrainContainer(ISimpleVehicleContainer simpleContainer)
		{
			return ComponentFactory.CreateSimplePowertrainContainer(simpleContainer, AxleNumber.NOT_IN_AXLE_POWERTRAIN);
		}


		protected ISimpleVehicleContainer BuildSimplePowertrain(IVehicleContainer realContainer,
			VectoSimulationJobType? overrideJobType, string torqueSplitterType)
		{
			var jobType = overrideJobType.HasValue ? overrideJobType.Value : realContainer.RunData.JobType;
			var runData = realContainer.RunData;
			switch (jobType) {
				case VectoSimulationJobType.ConventionalVehicle:
					return BuildSimplePowertrainConventional(realContainer);
				case VectoSimulationJobType.ParallelHybridVehicle when runData.BatteryOnlyHybridMode:
					return BuildSimpleHybridBatteryOnlyPowertrain(realContainer);
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.IHPC:
					return runData.Cycle.CycleType == CycleType.MeasuredSpeedGear
						? BuildSimpleHybridPowertrainGear(realContainer)
						: BuildSimpleHybridPowertrain(realContainer);
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
					return BuildSimplePowertrainElectric(realContainer);
				case VectoSimulationJobType.FCHV:
				case VectoSimulationJobType.FCHV_IEPC:
					return BuildSimplePowertrainFCHV(realContainer);
				case VectoSimulationJobType.SerialHybridVehicle:
					return BuildSimpleSerialHybridPowertrain(realContainer);
				case VectoSimulationJobType.IEPC_S:
					return BuildSimpleIEPCHybridPowertrain(realContainer);
				case VectoSimulationJobType.Multiple_PEV:
					return BuildSimplePowertrainMultiplePEV(realContainer, torqueSplitterType);
				case VectoSimulationJobType.Multiple_FCHV:
					return BuildSimplePowertrainMultipleFCHV(realContainer, torqueSplitterType);
				default:
					throw new ArgumentOutOfRangeException();
			}
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
        public ISimpleVehicleContainer BuildSimplePowertrainConventional(IVehicleContainer realContainer)
		{
			
			var container = GetVehicleContainer(realContainer);
			var powertrainContainer =
				ComponentFactory.CreateSimplePowertrainContainer(container);
			var data = container.RunData;
			
			
			IVehicle vehicle = ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData);
			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.MeasuredSpeed:
					ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
					break;
				case CycleType.DistanceBased:
					ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);
					break;
				case CycleType.EngineOnly:
					break;
				default:
					throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, powertrainContainer, data.EngineData);
			var gearbox = GetSimpleGearbox(container, powertrainContainer, data);
			var idleController = GetIdleController(data.PTOSinglePwt, engine, container);

			vehicle.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius,
					data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateAxleGear(container, powertrainContainer, data.AxleGearSinglePwt))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(data.AngledriveSinglePwt != null ? ComponentFactory.CreateAngledrive(container, powertrainContainer, data.AngledriveSinglePwt) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.RetarderSinglePwt, container, powertrainContainer))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.RetarderSinglePwt, container, powertrainContainer))
				.AddComponent(
					data.GearboxSinglePwt.Type.ManualTransmission() ? ComponentFactory.CreateClutch(data.JobType, container, powertrainContainer, data.EngineData) : null)
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data, powertrainContainer);

			if (gearbox is IAPTGearbox atGbx) {
				atGbx.IdleController = idleController;
			}
			return container;
        }

		public ISimpleVehicleContainer BuildSimpleHybridPowertrainGear(IVehicleContainer realContainer)
		{
			var data = realContainer.RunData;
			VerifyCycleType(data, CycleType.MeasuredSpeedGear);
			var container = GetVehicleContainer(realContainer);
			var powertrainContainer = ComponentFactory.CreateSimplePowertrainContainer(container);
            var es = ConnectREESS(data, container);

			// add engine before gearbox so that gearbox can obtain if an ICE is available already in constructor
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, powertrainContainer, data.EngineData);
			var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxSinglePwt.Type,
				container, powertrainContainer, null);
			

			var ctl = ComponentFactory.CreateHybridController(data.Cycle.CycleType, container, powertrainContainer, null, es);
			ctl.Gearbox = gearbox as IHybridControlledGearbox;
			ctl.Engine = engine;

			var position = data.ElectricMachinesSinglePwt[0].Item1;

			TimeRunHybridComponents components = new TimeRunHybridComponents() {
				Cycle = ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, data.Cycle),
				Engine = engine,
				Gearbox = gearbox,
				Clutch = AddClutch(data, container, powertrainContainer),
				IdleController = GetIdleController(data.PTOSinglePwt, engine, container),
				ElectricMotor =
					GetElectricMachine<MeasuredSpeedGearHybridsElectricMotor>(position, data.ElectricMachinesSinglePwt,
						container, es, ctl, powertrainContainer),
				HybridController = ctl
			};

			_timerunGearHybridBuilders[position].Invoke(data, container, components, powertrainContainer);

			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddHighVoltageAuxiliaries(data, container, es, dcdc);
			AddHybridBusAuxiliaries(data, container, es, dcdc);
			return container;
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
		public ISimpleVehicleContainer BuildSimpleSerialHybridPowertrain(IVehicleContainer realContainer)
		{
			var data = realContainer.RunData;
			var container = GetVehicleContainer(realContainer);
			var powertrainContainer =
				ComponentFactory.CreateSimplePowertrainContainer(container);
			var es = ConnectREESS(data, container);
			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddElectricAuxiliaries(data, container, powertrainContainer, es, null, dcdc);
			es.Connect(ComponentFactory.CreateGensetChargerAdapter(null));

			var ctl = ComponentFactory.CreateHybridController(data.Cycle.CycleType, container, powertrainContainer, null, es);

            //Vehicle-->Wheels-->SimpleHybridController-->Brakes
            var powertrain = ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData)
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateBrakes(container));

            var pos = data.ElectricMachinesSinglePwt.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			switch (pos) {
				case PowertrainPosition.BatteryElectricE4:
                    //-->Engine E4
					ComponentFactory.CreateDummyGearboxInfo(false, container, powertrainContainer);
					ComponentFactory.CreateATClutchInfo(container, powertrainContainer);

					powertrain.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE4,
						data.ElectricMachinesSinglePwt, container, es, ctl, powertrainContainer));
					break;

				case PowertrainPosition.BatteryElectricE3:
                    //-->AxleGear-->(AxlegearInputRetarder)-->Engine E3
					ComponentFactory.CreateDummyGearboxInfo(false, container, powertrainContainer);
					ComponentFactory.CreateATClutchInfo(container, powertrainContainer);

					powertrain
						.AddComponent(ComponentFactory.CreateAxleGear(container, powertrainContainer, data.AxleGearSinglePwt))
						.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.RetarderSinglePwt, container, powertrainContainer))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE3,
							data.ElectricMachinesSinglePwt, container, es, ctl, powertrainContainer));
					break;

				case PowertrainPosition.BatteryElectricE2:
					//-->AxleGear-->(AngleDrive)-->(TransmissionOutputRetarder)-->APTNGearbox or Gearbox-->(TransmissionInputRetarder)-->Engine E2
					var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxSinglePwt.Type, container, powertrainContainer, ctl.ShiftStrategy);

					ctl.Gearbox = gearbox as IHybridControlledGearbox;
					ComponentFactory.CreateDummyEngineInfo(container);

					powertrain
						.AddComponent(ComponentFactory.CreateAxleGear(container, powertrainContainer, data.AxleGearSinglePwt))
						.AddComponent(data.AngledriveSinglePwt != null
							? ComponentFactory.CreateAngledrive(container, powertrainContainer, data.AngledriveSinglePwt)
							: null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.RetarderSinglePwt, container, powertrainContainer))
						.AddComponent(gearbox)
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.RetarderSinglePwt, container, powertrainContainer))
						.AddComponent(GetPEVPTO(container, powertrainContainer, data))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE2,
							data.ElectricMachinesSinglePwt, container, es, ctl, powertrainContainer));
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos,
						"Invalid engine powertrain position for simple serial hybrid vehicles.");
			}
			return container;
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
		public ISimpleVehicleContainer BuildSimpleIEPCHybridPowertrain(IVehicleContainer realContainer)
		{
			var data = realContainer.RunData;
			var container = GetVehicleContainer(realContainer);
			var powertrainContainer =
				ComponentFactory.CreateSimplePowertrainContainer(container);
			var es = ConnectREESS(data, container);
			es.Connect(ComponentFactory.CreateGensetChargerAdapter(null));

			var ctl = ComponentFactory.CreateHybridController(data.Cycle.CycleType, container, powertrainContainer, null, es);

			//Vehicle-->Wheels-->SimpleHybridController-->Brakes
            var powertrain = ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData)
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData));

			var gearbox = ComponentFactory.CreateGearbox(data.JobType, data.Cycle.CycleType, data.GearboxSinglePwt.Type,
				container, powertrainContainer, ctl.ShiftStrategy);

			var em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesSinglePwt, container, es, ctl, powertrainContainer);
			powertrain
				.AddComponent(data.AxleGearSinglePwt != null ? ComponentFactory.CreateAxleGear(container, powertrainContainer, data.AxleGearSinglePwt) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.RetarderSinglePwt, container, powertrainContainer))
				.AddComponent(gearbox)
				.AddComponent(em);
			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddElectricAuxiliaries(data, container, powertrainContainer, es, null, dcdc);
			return container;
        }

		/// <summary>
		/// Builds a simple genset
		/// <code>
		/// Engine Gen
		///  └CombustionEngine
		/// </code>
		/// </summary>
		public ISimpleVehicleContainer BuildSimpleGenSet(IVehicleContainer realContainer)
		{
			var data = realContainer.RunData;
			var container = GetVehicleContainer(realContainer);
			var powertrainContainer = ComponentFactory.CreateSimplePowertrainContainer(container);
			var es = ConnectREESS(data, container);
			var ctl = new GensetMotorController(container, es);

			var ice = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, powertrainContainer, data.EngineData);
			AddAuxiliariesSerialHybrid(ice, container, data, powertrainContainer);

			GetElectricMachine(PowertrainPosition.GEN, data.ElectricMachinesSinglePwt, container, powertrainContainer, es, ctl)
				.AddComponent(ice);

			ComponentFactory.CreateATClutchInfo(container, powertrainContainer);
			ComponentFactory.CreateDummyGearboxInfo(false, container, powertrainContainer, gear: new GearshiftPosition(0));
			new DummyVehicleInfo(container);

			if (data.BusAuxiliaries != null) {
				if (!(container.BusAux is BusAuxiliariesAdapter busAux)) {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}

				var auxCfg = data.BusAuxiliaries;
				var electricStorage = 
					ComponentFactory.CreateSimpleBattery(auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart, container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity,
						auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency);
				busAux.ElectricStorage = electricStorage;
				if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
					busAux.DCDCConverter = dcdc;
					es.Connect(dcdc);
				}
			}
			return container;
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
		public ISimpleVehicleContainer BuildSimpleHybridPowertrain(IVehicleContainer realContainer)
		{
			var data = realContainer.RunData;
			var container = GetVehicleContainer(realContainer);
			var powertrainContainer =
				ComponentFactory.CreateSimplePowertrainContainer(container);
			var es = ConnectREESS(data, container);
			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddHighVoltageAuxiliaries(data, container, es, dcdc);

			//IMPORTANT HINT: add engine BEFORE gearbox to container that gearbox can obtain if an ICE is available
			var engine = ComponentFactory.CreateCombustionEngine(data.Cycle.CycleType, container, powertrainContainer, data.EngineData);
			var gearbox = GetSimpleGearbox(container, powertrainContainer, data);
			if (!(gearbox is IHybridControlledGearbox gbx)) {
				throw new VectoException("Gearbox can not be used for parallel hybrid");
			}

			var ctl = ComponentFactory.CreateHybridController(data.Cycle.CycleType, container, powertrainContainer, null, es);
			ctl.Gearbox = gbx;
			ctl.Engine = engine;

			var idleController = GetIdleController(data.PTOSinglePwt, engine, container);
			var clutch = (data.GearboxSinglePwt.Type.ManualTransmission() || data.GearboxSinglePwt.Type == GearboxType.IHPC)
				? ComponentFactory.CreateClutch(data.JobType, container, powertrainContainer, data.EngineData) 
				: null;


			var vehicle = ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData);

			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);
					break;
				case CycleType.MeasuredSpeed:
					ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
					break;
				case CycleType.EngineOnly: break;
				default: throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

			if ((data.SuperCapData != null || data.BatteryData != null) && data.EngineData.WHRType.IsElectrical()) {
				var dcDcConverterEfficiency = DeclarationData.WHRChargerEfficiency;
				var whrCharger = ComponentFactory.CreateWHRCharger(container, dcDcConverterEfficiency);
				es.Connect(whrCharger);
				engine.WHRCharger = whrCharger;
			}

			vehicle.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius,
					data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(
					GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesSinglePwt, container, es, ctl, powertrainContainer))
				.AddComponent(ComponentFactory.CreateAxleGear(container, powertrainContainer, data.AxleGearSinglePwt))
				.AddComponent(
					GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesSinglePwt, container, es, ctl, powertrainContainer))
				.AddComponent(data.AngledriveSinglePwt != null ? ComponentFactory.CreateAngledrive(container, powertrainContainer, data.AngledriveSinglePwt) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.RetarderSinglePwt, container, powertrainContainer))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.RetarderSinglePwt, container, powertrainContainer))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2_5, data.ElectricMachinesSinglePwt, container,
					es,
					ctl, powertrainContainer))
				.AddComponent(
					GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesSinglePwt, container, es, ctl, powertrainContainer))
				.AddComponent(
					GetElectricMachine(PowertrainPosition.IHPC, data.ElectricMachinesSinglePwt, container, es, ctl, powertrainContainer))
				.AddComponent(clutch)
				.AddComponent(
					GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesSinglePwt, container, es, ctl, powertrainContainer))
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data, powertrainContainer);

			if (data.ElectricMachinesSinglePwt.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				// this has to be done _after_ the powertrain is connected together so that the cluch already has its nextComponent set (necessary in the idle controlelr)
				if (gearbox is IAPTGearbox atGbx) {
					atGbx.IdleController = idleController;
				} else {
					clutch.IdleController = idleController;
				}
			}

			if (data.BusAuxiliaries != null) {
				if (!(container.BusAux is BusAuxiliariesAdapter busAux)) {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}

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
			}
			return container;
		}

        public ISimpleVehicleContainer BuildSimpleHybridBatteryOnlyPowertrain(IVehicleContainer realContainer)
        {
	        var data = realContainer.RunData;
            var container = GetVehicleContainer(realContainer);
            var axlePtContainer = ComponentFactory.CreateSimplePowertrainContainer(container);
            var es = ConnectREESS(data, container);
            var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);
            AddHighVoltageAuxiliaries(data, container, es, dcdc);

            //IMPORTANT HINT: add engine BEFORE gearbox to container that gearbox can obtain if an ICE is available
            var engine = ComponentFactory.CreateCombustionEngineBatteryOnlyHybrid(data.Cycle.CycleType, container, axlePtContainer, data.EngineData);
            var gearbox = ComponentFactory.CreateGearboxBatteryOnlyHybrid(data.JobType, data.Cycle.CycleType, data.GearboxSinglePwt.Type, container.RunData.ElectricMachinesSinglePwt.First().Item1
			,container, axlePtContainer, null);
            
            var idleController = GetIdleController(data.PTOSinglePwt, engine, container);
            var clutch = (data.GearboxSinglePwt.Type.ManualTransmission() || data.GearboxSinglePwt.Type == GearboxType.IHPC)
                ? ComponentFactory.CreateClutch(data.JobType, container, axlePtContainer, data.EngineData)
                : null;


            var vehicle = ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData);

            // TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
            //       should be a reference/proxy to the main driving cyle. but how to access it?
            switch (data.Cycle.CycleType) {
                case CycleType.DistanceBased:
                    ComponentFactory.CreateDistanceBasedDrivingCycle( container, data.Cycle);
                    break;
                case CycleType.MeasuredSpeed:
					ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
                    break;
                case CycleType.EngineOnly:
                    break;
                default:
                    throw new VectoException("Wrong CycleType for SimplePowertrain");
            }

            if ((data.SuperCapData != null || data.BatteryData != null) && data.EngineData.WHRType.IsElectrical()) {
                var dcDcConverterEfficiency = DeclarationData.WHRChargerEfficiency;
                var whrCharger = ComponentFactory.CreateWHRCharger(container, dcDcConverterEfficiency);
                es.Connect(whrCharger);
                engine.WHRCharger = whrCharger;
            }

			var ctl = ComponentFactory.CreateElectricMotorControllerBatteryOnlyHybrid(data.Cycle.CycleType, container, axlePtContainer, es);
            vehicle.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius,
                    data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
                .AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
                .AddComponent(
                    GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesSinglePwt, container, axlePtContainer, es, ctl))
                .AddComponent(ComponentFactory.CreateAxleGear(container, axlePtContainer, data.AxleGearSinglePwt))
                .AddComponent(
                    GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesSinglePwt, container, axlePtContainer, es, ctl))
                .AddComponent(data.AngledriveSinglePwt != null ? ComponentFactory.CreateAngledrive(container, axlePtContainer, data.AngledriveSinglePwt) : null)
                .AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.RetarderSinglePwt, container, axlePtContainer))
                .AddComponent(gearbox)
                .AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.RetarderSinglePwt, container, axlePtContainer))
                .AddComponent(GetElectricMachine(PowertrainPosition.HybridP2_5, data.ElectricMachinesSinglePwt, container, axlePtContainer, es, ctl))
                .AddComponent(
                    GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesSinglePwt, container, axlePtContainer, es, ctl))
                .AddComponent(
                    GetElectricMachine(PowertrainPosition.IHPC, data.ElectricMachinesSinglePwt, container, axlePtContainer, es, ctl))
                .AddComponent(clutch)
                .AddComponent(
                    GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesSinglePwt, container, axlePtContainer, es, ctl))
                .AddComponent(engine, idleController);
            AddAuxiliaries(engine, container, data, axlePtContainer);

            if (data.ElectricMachinesSinglePwt.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
                // this has to be done _after_ the powertrain is connected together so that the cluch already has its nextComponent set (necessary in the idle controlelr)
                clutch.IdleController = idleController;
                
            }

            if (data.BusAuxiliaries != null) {
                if (!(container.BusAux is BusAuxiliariesAdapter busAux)) {
                    throw new VectoException("BusAux data set but no BusAux component found!");
                }

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
            }
            return container;
        }

        /// <summary>
        /// Builds a simple battery electric powertrain for PEVs.
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
        public ISimpleVehicleContainer BuildSimplePowertrainElectric(IVehicleContainer realContainer)
        {
	        var axleNumber = AxleNumber.NOT_IN_AXLE_POWERTRAIN;
	        
			var container = GetVehicleContainer(realContainer);
			var powertrainContainer =
				ComponentFactory.CreateSimplePowertrainContainer(container, axleNumber);
			var ptData = realContainer.RunData.AxlePowertrainsDict[axleNumber];
			var data = realContainer.RunData;
			var vehicle = ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData);

			// TODO: MQ 2018-11-19: engineering mode needs AUX power from cycle, use face cycle...
			//       should be a reference/proxy to the main driving cyle. but how to access it?
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);
					break;
				case CycleType.MeasuredSpeed:
					ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
					break;
				case CycleType.EngineOnly:
					break;
				default:
					throw new VectoException("Wrong CycleType for SimplePowertrain");
			}

			var es = ConnectREESS(data, container);
			es.Connect(new SimpleCharger());

			
			
			var em = data.BatteryOnlyHybridMode ?
				GetElectricMachineBatteryOnlyP2(ptData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1,
					ptData.ElectricMachinesData, container, es, ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, powertrainContainer, es), powertrainContainer) 
				: GetElectricMachine(ptData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1,
					ptData.ElectricMachinesData, container, powertrainContainer, es, ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, powertrainContainer, es));

			vehicle.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius,
                    data.VehicleData.WheelsInertia))
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ptData.AxleGearData is null ? null : ComponentFactory.CreateAxleGear(container, powertrainContainer, ptData.AxleGearData))
				.AddComponent(ptData.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, powertrainContainer, ptData.AngledriveData) : null)
				.AddComponent(ptData.GearboxData is null ? null : GetSimpleGearbox(container, powertrainContainer, data))
				.AddComponent(em);
			var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);

			AddElectricAuxiliaries(data, container, powertrainContainer, es, null, dcdc);
			if (ptData.AxleGearData == null) {
				ComponentFactory.CreateDummyAxleGearInfo(container, powertrainContainer); // necessary for certain IEPC configurations
			}

			if (data.BatteryOnlyHybridMode && data.EngineData != null)
			{
				new AlwaysOffCombustionEngine(container, powertrainContainer, data.EngineData);
			}

			return container;
		}

        public ISimpleVehicleContainer BuildSimplePowertrainMultiplePEV(IVehicleContainer realContainer, string torqueSplitterType)
		{			
			var data = realContainer.RunData;
            var container = GetVehicleContainer(realContainer);
            var vehicle = ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData);

            switch (data.Cycle.CycleType)
            {
                case CycleType.DistanceBased:
                    ComponentFactory.CreateDistanceBasedDrivingCycle(container, data.Cycle);
                    break;
                case CycleType.MeasuredSpeed:
                    ComponentFactory.CreateMeasuredSpeedDrivingCycle(container, GetMeasuredSpeedDummyCycle()).AddComponent(vehicle);
                    break;
                case CycleType.EngineOnly:
                    break;
                default:
                    throw new VectoException("Wrong CycleType for SimplePowertrain");
            }

            var es = ConnectREESS(data, container);
            es.Connect(new SimpleCharger());

            var junctionBox = ComponentFactory.CreateElectricPowerJunctionBox(container);
            junctionBox.Connect(es);

            var powertrain = vehicle
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
                .AddComponent(ComponentFactory.CreateBrakes(container))
                .AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
                .AddComponent(ComponentFactory.CreateTorqueSplitter(torqueSplitterType, container, junctionBox));

            var dcdc = ComponentFactory.CreateDCDCConverter(container, data.DCDCData.DCDCEfficiency);

            bool addedAuxiliaries = false;

            var axlePtWithPTO = data.AxlePowertrains.FirstOrDefault(x => x.PTO != null);

			foreach (var axlePt in data.AxlePowertrains)
			{
				var powertrainContainer = ComponentFactory.CreateSimplePowertrainContainer(container, axlePt.AxleNumber);
				
				var ptBuilder = _axlePowertrainPEVBuilders[axlePt.ElectricMachineData.Item1];
				IElectricMotor em = ptBuilder(data, axlePt, junctionBox, container, powertrainContainer, powertrain, es);

                if (!addedAuxiliaries)
                {
                    if ((axlePtWithPTO == null) || (axlePt == axlePtWithPTO))
                    {
                        AddElectricAuxiliaries(data, container, powertrainContainer, es, null, dcdc);
                        addedAuxiliaries = true;
                    }
                }
            }

            return container;
        }

        private IElectricMotor CreateAxlePowertrainForIEPC(VectoRunData data,
	        AxlePowertrainData axlePt,
	        IElectricPowerJunctionBox junctionBox,
	        IVehicleContainer container,
	        IPowertrainContainer powertrainContainer,
	        IPowerTrainComponent powertrain, 
			IElectricSystem es)
        {
            var em = ComponentFactory.CreateElectricMotor(
                true,
                container, 
				powertrainContainer, 
				axlePt.ElectricMachineData.Item2, 
				ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, powertrainContainer, es, axlePt: axlePt), 
				axlePt.ElectricMachineData.Item1);

            em.Connect(junctionBox.GetElectricPowerJunctionBoxPort());

            powertrain
                .AddComponent(axlePt.AxleGearData != null ? ComponentFactory.CreateAxleGear(container, powertrainContainer, axlePt.AxleGearData) : null)
                .AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, axlePt.Retarder, container, powertrainContainer))
                .AddComponent(GetSimpleGearbox(container, powertrainContainer, data))    
                .AddComponent(em);

            ComponentFactory.CreateDummyAxleGearInfo(container, powertrainContainer);
            
            return em;
        }

        private IElectricMotor CreateAxlePowertrainForE2(
            VectoRunData data,
            AxlePowertrainData axlePt,
            IElectricPowerJunctionBox junctionBox,
            IVehicleContainer container,
            IPowertrainContainer powertrainContainer,
            IPowerTrainComponent powertrain, 
			IElectricSystem es)
        {
            var em = ComponentFactory.CreateElectricMotor(
                false,
                container, 
				powertrainContainer, 
				axlePt.ElectricMachineData.Item2, 
				ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, powertrainContainer, es), 
				axlePt.ElectricMachineData.Item1);

            em.Connect(junctionBox.GetElectricPowerJunctionBoxPort());

            powertrain
                .AddComponent(ComponentFactory.CreateAxleGear(container, powertrainContainer, axlePt.AxleGearData))
                .AddComponent(axlePt.AngledriveData != null ? ComponentFactory.CreateAngledrive(container, powertrainContainer, axlePt.AngledriveData) : null)
                .AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, axlePt.Retarder, container, powertrainContainer))
                .AddComponent(GetSimpleGearbox(container, powertrainContainer, data))
                .AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, axlePt.Retarder, container, powertrainContainer))
                .AddComponent(GetPEVPTO(container, powertrainContainer, data))
                .AddComponent(em);

            return em;
        }

        private IElectricMotor CreateAxlePowertrainForE3(
            VectoRunData data,
            AxlePowertrainData axlePt,
            IElectricPowerJunctionBox junctionBox,
            IVehicleContainer container,
            IPowertrainContainer powertrainContainer,
            IPowerTrainComponent powertrain, 
			IElectricSystem es)
        {
            var em = ComponentFactory.CreateElectricMotor(
                false,
                container, 
				powertrainContainer, 
				axlePt.ElectricMachineData.Item2, 
				ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, powertrainContainer, es, axlePt: axlePt), 
				axlePt.ElectricMachineData.Item1);

            em.Connect(junctionBox.GetElectricPowerJunctionBoxPort());

            powertrain
                .AddComponent(ComponentFactory.CreateAxleGear(container, powertrainContainer, axlePt.AxleGearData))
                .AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, axlePt.Retarder, container, powertrainContainer))
                .AddComponent(em);

            ComponentFactory.CreateDummyGearboxInfo(false, container, powertrainContainer);
            ComponentFactory.CreateATClutchInfo(container, powertrainContainer);

            return em;
        }

        private IElectricMotor CreateAxlePowertrainForE4(
            VectoRunData data,
            AxlePowertrainData axlePt,
            IElectricPowerJunctionBox junctionBox,
            IVehicleContainer container,
            IPowertrainContainer powertrainContainer,
            IPowerTrainComponent powertrain, 
			IElectricSystem es)
		{
            var em = ComponentFactory.CreateElectricMotor(
                false,
                container, 
				powertrainContainer, 
				axlePt.ElectricMachineData.Item2, 
				ComponentFactory.CreateElectricMotorController(data.Cycle.CycleType, container, powertrainContainer, es, axlePt: axlePt), 
				axlePt.ElectricMachineData.Item1);

            em.Connect(junctionBox.GetElectricPowerJunctionBoxPort());

            powertrain.AddComponent(em);

            ComponentFactory.CreateDummyAxleGearInfo(container, powertrainContainer);
            ComponentFactory.CreateDummyGearboxInfo(false, container, powertrainContainer);
            ComponentFactory.CreateATClutchInfo(container, powertrainContainer);

            return em;
        }

        public ISimpleVehicleContainer BuildSimplePowertrainFCHV(IVehicleContainer realContainer)
		{
			var data = realContainer.RunData;
			var container = BuildSimplePowertrainElectric(realContainer);
			var es = container.ElectricSystemInfo as ElectricSystem;

			if (data.FuelCellSystemData != null) {
				var fuelCellSystem = ComponentFactory.CreateFuelCellSystem(container, data.FuelCellSystemData);
				es.Connect(fuelCellSystem);
			}

			return container;
		}

		public ISimpleVehicleContainer BuildSimplePowertrainMultipleFCHV(IVehicleContainer realContainer, string torqueSplitterType)
		{
			var data = realContainer.RunData;
			var container = BuildSimplePowertrainMultiplePEV(realContainer, torqueSplitterType);

            var es = container.ElectricSystemInfo as ElectricSystem;

            if (data.FuelCellSystemData != null)
            {
                var fuelCellSystem = ComponentFactory.CreateFuelCellSystem(container, data.FuelCellSystemData);
                es.Connect(fuelCellSystem);
            }

            return container;
        }

        protected ISimpleVehicleContainer GetVehicleContainer(IVehicleContainer realContainer)
		{
			var runData =  realContainer.RunData;
			var container = ComponentFactory.CreateSimpleVehicleContainer(runData);
			return container;
		}

		protected IGearbox GetSimpleGearbox(IVehicleContainer container, IPowertrainContainer powertrainContainer, VectoRunData runData)
		{
			var axleNumber = powertrainContainer.AxleNumber;
			var gearboxData = runData.GetGearboxData().First(x => x.Item1 == axleNumber).Item2;

			if (gearboxData.Type.AutomaticTransmission() && (gearboxData.Type != GearboxType.APTN) && (gearboxData.Type != GearboxType.IHPC)) 
			{
				ComponentFactory.CreateATClutchInfo(container, powertrainContainer);
			}

			return ComponentFactory.CreateGearbox(runData.JobType, runData.Cycle.CycleType, gearboxData.Type, container, powertrainContainer, null);
		}

		// used for battery electric powertrains
        protected IElectricMotor GetElectricMachine(PowertrainPosition pos,
	        IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, IVehicleContainer container,
	        IPowertrainContainer powertrainContainer, IElectricSystem es,
	        IElectricMotorControl ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData is null) {
				return null;
			}

			var motor = ComponentFactory.CreateElectricMotor(pos == PowertrainPosition.IEPC, container, powertrainContainer, motorData.Item2, ctl, pos);

            motor.Connect(es);
			return motor;
		}

		// used for hybrid electric powertrains
        protected IElectricMotor GetElectricMachine(PowertrainPosition pos,
	        IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, IVehicleContainer container,
	        IElectricSystem es,
	        IHybridController ctl, IPowertrainContainer powertrainContainer)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData is null) {
				return null;
			}

			ctl.AddElectricMotor(pos, motorData.Item2);
            var motor = ComponentFactory.CreateElectricMotor(pos == PowertrainPosition.IEPC, container, powertrainContainer, motorData.Item2, ctl.ElectricMotorControl(pos), pos);

            if (pos == PowertrainPosition.GEN) {
				es.Connect(ComponentFactory.CreateGensetChargerAdapter(motor));
			} else {
				motor.Connect(es);
			}

			return motor;
		}

	}
}

public class SimpleElectricMotorControl : ITestPowertrainElectricMotorControl
{
	public bool EmOff { get; set; }

	public NewtonMeter EMTorque { get; set; }

	public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity,
		PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
		PowertrainPosition position, bool dryRun)
	{
		if (EmOff) {
			return null;
		}

		if (EMTorque != null) {
			return EMTorque;
		}


		if (dryRun) {
			return -outTorque;
		}
		return (-outTorque).LimitTo(maxDriveTorque ?? 0.SI<NewtonMeter>(), maxRecuperationTorque ?? VectoMath.Max(maxDriveTorque, 0.SI<NewtonMeter>()));
	}
}

public class GensetMotorController : IGensetMotorController
{
	public GensetMotorController(IVehicleContainer container, IElectricSystem es)
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
