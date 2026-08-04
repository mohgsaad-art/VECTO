using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Auxiliaries;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{

    public abstract class PowertrainBuilderBase
	{
		protected readonly
			Dictionary<PowertrainPosition, Action<VectoRunData, IVehicleContainer, TimeRunHybridComponents>>
			_timerunGearHybridBuilders;

		protected readonly IShiftStrategyFactory ShiftStrategyFactory;
		protected readonly IPowertrainComponentFactory ComponentFactory;

		protected PowertrainBuilderBase(IPowertrainComponentFactory powertrainComponentFactory,
			IShiftStrategyFactory shiftStrategyFactory)
		{
			ShiftStrategyFactory = shiftStrategyFactory;
			ComponentFactory = powertrainComponentFactory;
			_timerunGearHybridBuilders =
				new Dictionary<PowertrainPosition, Action<VectoRunData, IVehicleContainer, TimeRunHybridComponents>>() {
					{ PowertrainPosition.HybridP1, BuildTimerunGearHybridForP1 },
					{ PowertrainPosition.HybridP2, BuildTimerunGearHybridForP2 },
					{ PowertrainPosition.HybridP2_5, BuildTimerunGearHybridForP2 },
					{ PowertrainPosition.HybridP3, BuildTimerunGearHybridForP3 },
					{ PowertrainPosition.HybridP4, BuildTimerunGearHybridForP4 },
					{ PowertrainPosition.IHPC, BuildTimerunGearHybridForP2 }
				};
		}

		private void BuildTimerunGearHybridForP1(VectoRunData data, IVehicleContainer container,
			TimeRunHybridComponents components)
		{
			components.Cycle
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(components.HybridController)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearSinglePwt))
				.AddComponent(data.AngledriveSinglePwt != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveSinglePwt) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.RetarderSinglePwt, container))
				.AddComponent(components.Gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.RetarderSinglePwt, container))
				.AddComponent(components.Clutch)
				.AddComponent(components.ElectricMotor)
				.AddComponent(components.Engine, components.IdleController);
			AddAuxiliaries(components.Engine, container, data);

			SetIdleControllerForHybridP1(data, components.Gearbox, components.IdleController, components.Clutch);
		}

		private void BuildTimerunGearHybridForP2(VectoRunData data, IVehicleContainer container,
			TimeRunHybridComponents components)
		{
			components.Cycle
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(components.HybridController)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearSinglePwt))
				.AddComponent(data.AngledriveSinglePwt != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveSinglePwt) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.RetarderSinglePwt, container))
				.AddComponent(components.Gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.RetarderSinglePwt, container))
				.AddComponent(components.ElectricMotor)
				.AddComponent(components.Clutch)
				.AddComponent(components.Engine, components.IdleController);
			AddAuxiliaries(components.Engine, container, data);
		}

		private void BuildTimerunGearHybridForP3(VectoRunData data, IVehicleContainer container,
			TimeRunHybridComponents components)
		{
			components.Cycle
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(components.HybridController)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearSinglePwt))
				.AddComponent(components.ElectricMotor)
				.AddComponent(data.AngledriveSinglePwt != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveSinglePwt) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.RetarderSinglePwt, container))
				.AddComponent(components.Gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.RetarderSinglePwt, container))
				.AddComponent(components.Clutch)
				.AddComponent(components.Engine, components.IdleController);
			AddAuxiliaries(components.Engine, container, data);
		}

		private void BuildTimerunGearHybridForP4(VectoRunData data, IVehicleContainer container,
			TimeRunHybridComponents components)
		{
			components.Cycle
				.AddComponent(ComponentFactory.CreateVehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(ComponentFactory.CreateWheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(components.HybridController)
				.AddComponent(ComponentFactory.CreateBrakes(container))
				.AddComponent(ComponentFactory.CreateWheelEnd(container, data.WheelEndData))
				.AddComponent(components.ElectricMotor)
				.AddComponent(ComponentFactory.CreateAxleGear(container, data.AxleGearSinglePwt))
				.AddComponent(data.AngledriveSinglePwt != null ? ComponentFactory.CreateAngledrive(container, data.AngledriveSinglePwt) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.RetarderSinglePwt, container))
				.AddComponent(components.Gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.RetarderSinglePwt, container))
				.AddComponent(components.Clutch)
				.AddComponent(components.Engine, components.IdleController);
			AddAuxiliaries(components.Engine, container, data);
		}

		protected void AddVTPBusAuxiliaries(VectoRunData data, IVehicleContainer container, ICombustionEngine engine)
		{
			var aux = ComponentFactory.CreateEngineAuxiliary(container);
			foreach (var auxData in data.Aux) {
				// id's in upper case
				var id = auxData.ID.ToUpper();

				switch (auxData.DemandType) {
					case AuxiliaryDemandType.Constant:
						aux.AddConstant(id, auxData.PowerDemandMech);
						break;
					case AuxiliaryDemandType.Direct:
						if (auxData.PowerDemandMechCycleFunc == null) {
							aux.AddCycle(id);
						} else {
							aux.AddCycle(id, auxData.PowerDemandMechCycleFunc);
						}

						break;
					default:
						throw new ArgumentOutOfRangeException("AuxiliaryDemandType", auxData.DemandType.ToString());
				}
			}

			engine.Connect(aux.Port());
		}

		protected void AddVTPTruckAuxiliaries(VectoRunData data, IVehicleContainer container,
			ICombustionEngine engine)
		{
			var aux = CreateSpeedDependentAuxiliaries(data, container);
			var engineFan = new EngineFanAuxiliary(data.FanDataVTP.FanCoefficients.Take(3).ToArray(),
				data.FanDataVTP.FanDiameter);
			aux.AddCycle(Constants.Auxiliaries.IDs.Fan, cycleEntry => engineFan.PowerDemand(cycleEntry));

			if (data.PTOSinglePwt != null) {
				aux.AddConstant(Constants.Auxiliaries.IDs.PTOTransmission,
					DeclarationData.PTOTransmission.Lookup(data.PTOSinglePwt.TransmissionType).PowerDemand,
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOTransmission);

				aux.Add(Constants.Auxiliaries.IDs.PTOConsumer,
					(n, absTime, dt, dryRun) =>
						container.DrivingCycleInfo.PTOActive ? null : data.PTOSinglePwt.LossMap.GetTorqueLoss(n) * n,
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOConsumer);
			}

			engine.Connect(aux.Port());
		}

		protected IClutch AddClutch(VectoRunData data, IVehicleContainer container)
		{
			IClutch clutch = null;

			if (data.GearboxSinglePwt.Type.ManualTransmission() || (data.GearboxSinglePwt.Type == GearboxType.IHPC)) {
				clutch = ComponentFactory.CreateClutch(data.JobType, container, data.EngineData);
			} else {
				ComponentFactory.CreateATClutchInfo(container);
			}

			return clutch;
		}

		protected void AddHybridBusAuxiliaries(VectoRunData data, IVehicleContainer container, IElectricSystem es,
			IDCDCConverter dcdc)
		{
			if (data.BusAuxiliaries == null) {
				return;
			}

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

		protected void SetIdleControllerForHybridP1(VectoRunData data, IGearbox gearbox, IIdleController idleController,
			IClutch clutch)
		{
			if (data.ElectricMachinesSinglePwt.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				if (gearbox is IAPTGearbox atGbx) {
					atGbx.IdleController = idleController;
				} else {
					if (clutch != null) {
						clutch.IdleController = idleController;
					}
				}
			}
		}

		/// <summary>
		/// Adds electric auxilaries and EPTO to the powertrain
		/// </summary>
		/// <param name="data"></param>
		/// <param name="container"></param>
		/// <param name="es"></param>
		/// <param name="cycle">Only for EPTO controller.</param>
		/// <param name="dcdc"></param>
		protected void AddElectricAuxiliaries(
			VectoRunData data,
			IVehicleContainer container,
			IElectricSystem es,
			IDistanceBasedDrivingCycle cycle,
			IDCDCConverter dcdc,
			AxlePowertrainData axlePt = null)
		{
			var elAux = new ElectricAuxiliaries(container);

            var dataPTO = (axlePt != null) ? axlePt.PTO : data.PTOSinglePwt;

            IEPTO epto = null;
			if (dataPTO?.PTOCycle != null) {
				var pevPTOController = GetPEV_SHEVIdleController(dataPTO, container);
				if (cycle != null) {
					cycle.IdleController = pevPTOController;
				}

				var eptoAux = new EPTO(pevPTOController, container);
				container.AddComponent(eptoAux);
				elAux.AddAuxiliary(eptoAux);
				epto = eptoAux;
			}

			elAux.AddAuxiliaries(data.Aux.Where(x => x.ConnectToREESS && x.ID != Constants.Auxiliaries.IDs.Cond));
			if (data.Aux.Any(aux => aux.ID == Constants.Auxiliaries.IDs.Cond))
			{
				var conditioningAux = data.Aux.FirstOrDefault(aux => aux.ID == Constants.Auxiliaries.IDs.Cond);
				var emConditioning = data.JobType.IsFCHV() 
					? DeclarationData.Conditioning.LookupPowerDemand(
						data.VehicleData.VehicleClass,
						VectoSimulationJobType.BatteryElectricVehicle,
						data.Mission.MissionType.GetNonEMSMissionType())
					: null;

				elAux.AddAuxiliary(new Conditioning(conditioningAux, epto, emConditioning));
			}

			var hvElectricAuxiliaries = ConfigureHVElectricAuxilariesData(data);
			elAux.AddAuxiliaries(hvElectricAuxiliaries);

			dcdc.Connect(elAux);
			dcdc.Initialize();
			es.Connect(dcdc);
		}

		protected void AddHighVoltageAuxiliaries(
			VectoRunData data,
			IVehicleContainer container,
			IElectricSystem es,
			IDCDCConverter dcdc)
		{
			var elAux = new ElectricAuxiliaries(container);

			var hvElectricAuxiliaries = ConfigureHVElectricAuxilariesData(data);
			elAux.AddAuxiliaries(hvElectricAuxiliaries);

			dcdc.Connect(elAux);
			dcdc.Initialize();
			es.Connect(dcdc);
		}

		protected IEnumerable<VectoRunData.AuxData> ConfigureHVElectricAuxilariesData(VectoRunData data)
		{
			VectoRunData.AuxData additionalPowerData = new VectoRunData.AuxData() {
				ID = Constants.Auxiliaries.IDs.PowerAdditonalHighVoltage,
				IsFullyElectric = true,
				ConnectToREESS = true,
				DemandType = AuxiliaryDemandType.Dynamic,
				PowerDemandDataBusFunc = (d, m) =>
					d.DrivingCycleInfo.CycleData.LeftSample.PowerAdditonalHighVoltage ?? 0.SI<Watt>()
			};

			VectoRunData.AuxData auxElectricPowerData = new VectoRunData.AuxData() {
				ID = Constants.Auxiliaries.IDs.PowerAuxiliaryElectric,
				IsFullyElectric = true,
				ConnectToREESS = true,
				DemandType = AuxiliaryDemandType.Constant,
				PowerDemandElectric = data.ElectricAuxDemand ?? 0.SI<Watt>(),
			};

			return new List<VectoRunData.AuxData> { additionalPowerData, auxElectricPowerData };
		}

		protected void VerifyCycleType(VectoRunData data, CycleType cycleType)
		{
			if (data.Cycle.CycleType != cycleType) {
				throw new VectoException($"CycleType must be {cycleType}.");
			}
		}

		protected IPowerTrainComponent GetPEVPTO(IVehicleContainer container, VectoRunData data, AxlePowertrainData axlePt = null)
		{
            var ptoData = axlePt?.PTO ?? data.PTOSinglePwt;
			var ptoCycleWhileDrive = axlePt?.PTOCycleWhileDrive ?? data.PTOCycleWhileDrive;

            if (ptoData == null)
            {
                return null;
            }

            var pto = new PEVPtoTransm(container, axlePt?.AxleNumber ?? Constants.NOT_IN_AXLE_POWERTRAIN);

            if ((data.ExecutionMode == ExecutionMode.Engineering) && data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityRoadSweeping)) 
			{
				if (data.DriverData.PTODriveMinSpeed == null) 
				{
					throw new VectoSimulationException("PTO activity 'road sweeping' requested, but no min. engine speed or gear provided");
				}

                RoadSweeperAuxiliary rdSwpAux = new RoadSweeperAuxiliary(container);
				pto.Add(Constants.Auxiliaries.IDs.PTORoadsweeping, (nEng, absTime, dt, dryRun) => rdSwpAux.PowerDemand(nEng, absTime, dt, dryRun) / nEng);
				container.AddAuxiliary(Constants.Auxiliaries.IDs.PTORoadsweeping, Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTORoadsweeping);
			}

			if ((data.ExecutionMode == ExecutionMode.Engineering) && data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityWhileDrive)) 
			{
				if (ptoCycleWhileDrive == null) {
					throw new VectoException("PTO activation while drive requested in cycle but no PTO cycle provided");
				}

                PTODriveAuxiliary ptoDrive = new PTODriveAuxiliary(container, ptoCycleWhileDrive);
				pto.Add(Constants.Auxiliaries.IDs.PTODuringDrive,(nEng, absTime, dt, dryRun) => ptoDrive.PowerDemand(nEng, absTime, dt, dryRun) / nEng);
				container.AddAuxiliary(Constants.Auxiliaries.IDs.PTODuringDrive, Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTODuringDrive);
			}

			if (ptoData.TransmissionType != null) 
			{
				pto.AddConstant(Constants.Auxiliaries.IDs.PTOTransmission, DeclarationData.PTOTransmission.Lookup(ptoData.TransmissionType).TorqueLoss);
				container.AddAuxiliary(Constants.Auxiliaries.IDs.PTOTransmission, Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOTransmission + " [kW]");
			}

			return pto;
		}

		protected double GetDCDCEfficiency(VectoRunData rd)
		{
			double dcdcEff = rd.BusAuxiliaries?.ElectricalUserInputsConfig?.DCDCEfficiency ??
							rd.DCDCData?.DCDCEfficiency ?? 0;

			return dcdcEff;
		}

		protected IPowerTrainComponent GetRetarder(RetarderType type, RetarderData data, IVehicleContainer container, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN) =>
			type == data.Type ? ComponentFactory.CreateRetarder(container, data.LossMap, data.Ratio, axleNumber) : null;


		protected IElectricMotor GetElectricMachineBatteryOnlyP2(PowertrainPosition pos, IList<Tuple<PowertrainPosition,
				ElectricMotorData>> electricMachinesData, IVehicleContainer container, IElectricSystem es,
			IElectricMotorControl ctl)
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData is null) {
				return null;
			}

			//container.ModData?.AddElectricMotor(pos);
			var motor = ComponentFactory.CreateElectricMotor(false, container, motorData.Item2, ctl,
				PowertrainPosition.BatteryElectricE2);
			//var motor = new ElectricMotor(container, motorData.Item2, ctl, PowertrainPosition.BatteryElectricE2);
			motor.Connect(es);
			return motor;
		}

        protected IElectricMotor GetElectricMachine<TElectricMotor>(PowertrainPosition pos,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> electricMachinesData, IVehicleContainer container,
			IElectricSystem es, IHybridController ctl) where TElectricMotor : ElectricMotor
		{
			var motorData = electricMachinesData.FirstOrDefault(x => x.Item1 == pos);
			if (motorData == null) {
				return null;
			}

			ctl.AddElectricMotor(pos, motorData.Item2);

			var motor = (TElectricMotor)Activator.CreateInstance(typeof(TElectricMotor),
				new object[] { container, motorData.Item2, ctl.ElectricMotorControl(pos), pos });

			motor.Connect(es);

			return motor;
		}


		protected IElectricSystem ConnectREESS(VectoRunData data, IVehicleContainer container)
		{
			if (data.BatteryData != null && data.SuperCapData != null) {
				throw new VectoException("Powertrain requires either Battery OR SuperCapacitor, but both are defined.");
			}

			if (data.BatteryData is null && data.SuperCapData is null) {
				throw new VectoException("Powertrain requires either Battery OR SuperCapacitor, but none are defined.");
			}

			var es = ComponentFactory.CreateElectricSystem(container, data.BatteryData);
			if (data.BatteryData != null) {
				if (data.BatteryData.InitialSoC < data.BatteryData.Batteries.Min(x => x.Item2.MinSOC)) {
					throw new VectoException("Battery: Initial SoC has to be higher than min SoC");
				}

				var battery = ComponentFactory.CreateREESS(REESSType.Battery, container, data.BatteryData);
				battery.Initialize(data.BatteryData.InitialSoC);
				es.Connect(battery);
			}

			if (data.SuperCapData != null) {
				if (data.SuperCapData.InitialSoC < data.SuperCapData.MinVoltage / data.SuperCapData.MaxVoltage) {
					throw new VectoException("SuperCap: Initial SoC has to be higher than min SoC");
				}

				var superCap = ComponentFactory.CreateREESS(REESSType.SuperCap, container, data.SuperCapData);
				superCap.Initialize(data.SuperCapData.InitialSoC);
				es.Connect(superCap);
			}

			return es;
		}

		protected DrivingCycleData GetMeasuredSpeedDummyCycle() =>
			DrivingCycleDataReader.ReadFromStream((
				"<t>,<v>,<grad>\n" +
				"0, 50, 0\n" +
				"10, 50, 0").ToStream(), CycleType.MeasuredSpeed, "DummyCycle", false);

		protected IIdleController
			GetIdleController(PTOData pto, ICombustionEngine engine, IVehicleContainer container) =>
			pto?.PTOCycle is null
				? engine.IdleController
				: new IdleControllerSwitcher(engine.IdleController, new PTOCycleController(container, pto.PTOCycle));

		protected IIdleControllerSwitcher GetPEV_SHEVIdleController(PTOData pto,
			IVehicleContainer container) =>
			pto?.PTOCycle is null ? null : new EPTOCycleController(container, pto?.PTOCycle);

		protected internal void AddAuxiliaries(ICombustionEngine engine, IVehicleContainer container,
			VectoRunData data)
		{
			// aux --> engine
			if (data.BusAuxiliaries != null) {
				engine.Connect(CreateAdvancedAuxiliaries(data, container).Port());
			} else {
				if (data.Aux != null) {
					engine.Connect(CreateAuxiliaries(data, container).Port());
				}
			}
		}

		protected void AddAuxiliariesSerialHybrid(ICombustionEngine engine, IVehicleContainer container,
			VectoRunData data, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
		{
			// aux --> engine
			if (data.BusAuxiliaries != null) {
				engine.Connect(CreateAdvancedAuxiliaries(data, container, axleNumber).Port());
			} else {
				if (data.Aux != null) {
					engine.Connect(CreateAuxiliariesSerialHybrid(data, container).Port());
				}
			}
		}

		protected IAuxInProvider CreateAdvancedAuxiliaries(VectoRunData data, IVehicleContainer container, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
		{
			var conventionalAux = CreateAuxiliaries(data, container, axleNumber);
			// TODO: MQ 2019-07-30 -- which fuel map for advanced auxiliaries?!
			var busAux = new BusAuxiliariesAdapter(container, data.BusAuxiliaries, conventionalAux, axleNumber);
			var auxCfg = data.BusAuxiliaries;
			var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
				? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity,
					auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
				: (ISimpleBattery)new NoBattery(container);
			busAux.ElectricStorage = electricStorage;
			return busAux;
		}

		protected IEngineAuxiliary CreateAuxiliaries(VectoRunData data, IVehicleContainer container, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
		{
			var aux = ComponentFactory.CreateEngineAuxiliary(container);
			foreach (var auxData in data.Aux) {
				// id's in upper case
				var id = auxData.ID.ToUpper();
				if (auxData.ConnectToREESS) {
					continue;
				}

				switch (auxData.DemandType) {
					case AuxiliaryDemandType.Constant:
						aux.AddConstant(id, auxData.PowerDemandMech);
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

			if ((data.ExecutionMode == ExecutionMode.Engineering) && data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityRoadSweeping)) 
			{
				if (data.DriverData.PTODriveMinSpeed == null) 
				{
					throw new VectoSimulationException("PTO activity 'road sweeping' requested, but no min. engine speed or gear provided");
				}

				rdSwpAux = new RoadSweeperAuxiliary(container);
				aux.Add(Constants.Auxiliaries.IDs.PTORoadsweeping,
					(nEng, absTime, dt, dryRun) => rdSwpAux.PowerDemand(nEng, absTime, dt, dryRun),
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTORoadsweeping);
			}

			var axlePt = data.AxlePowertrains.FirstOrDefault(x => x.AxleNumber == axleNumber);
            var ptoData = axlePt?.PTO ?? data.PTOSinglePwt;
            var ptoCycleWhileDrive = axlePt?.PTOCycleWhileDrive ?? data.PTOCycleWhileDrive;

            if ((data.ExecutionMode == ExecutionMode.Engineering) && data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityWhileDrive)) 
			{
				if (ptoCycleWhileDrive == null) 
				{
					throw new VectoException("PTO activation while drive requested in cycle but no PTO cycle provided");
				}

				ptoDrive = new PTODriveAuxiliary(container, ptoCycleWhileDrive);
				aux.Add(Constants.Auxiliaries.IDs.PTODuringDrive,
					(nEng, absTime, dt, dryRun) => ptoDrive.PowerDemand(nEng, absTime, dt, dryRun),
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTODuringDrive);
			}

            if (ptoData != null) 
			{
                aux.AddConstant(Constants.Auxiliaries.IDs.PTOTransmission,
                    ptoData.TransmissionPowerDemand,
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOTransmission);

				aux.Add(Constants.Auxiliaries.IDs.PTOConsumer,
					(n, absTime, dt, dryRun) =>
						container.DrivingCycleInfo.PTOActive || (rdSwpAux?.Active(absTime) ?? false) || (ptoDrive?.Active(absTime) ?? false)
							? null
							: ptoData.LossMap.GetTorqueLoss(n) * n,
					Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOConsumer);
			}

			return aux;
		}

		protected IEngineAuxiliary CreateAuxiliariesSerialHybrid(VectoRunData data,
			IVehicleContainer container)
		{
			var aux = ComponentFactory.CreateEngineAuxiliary(container);
			foreach (var auxData in data.Aux) {
				if (auxData.ConnectToREESS == true) {
					continue;
				}

				// id's in upper case
				var id = auxData.ID.ToUpper();


				switch (auxData.DemandType) {
					case AuxiliaryDemandType.Constant:
						aux.AddConstant(id, auxData.PowerDemandMech);
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

		protected IEngineAuxiliary CreateSpeedDependentAuxiliaries(VectoRunData data, IVehicleContainer container)
		{
			var aux = ComponentFactory.CreateEngineAuxiliary(container);
			var auxData = data.Aux.ToArray();
			AddSwitchingAux(aux, Constants.Auxiliaries.IDs.HeatingVentilationAirCondition, auxData);
			AddSwitchingAux(aux, Constants.Auxiliaries.IDs.SteeringPump, auxData);
			AddSwitchingAux(aux, Constants.Auxiliaries.IDs.ElectricSystem, auxData);
			AddSwitchingAux(aux, Constants.Auxiliaries.IDs.PneumaticSystem, auxData);
			return aux;
		}

		protected void AddSwitchingAux(IEngineAuxiliary aux, string auxId, VectoRunData.AuxData[] auxData)
		{
			var urban = auxData.First(x => x.ID == auxId && x.MissionType == MissionType.UrbanDelivery);
			var rural = auxData.First(x => x.ID == auxId && x.MissionType == MissionType.RegionalDelivery);
			var motorway = auxData.First(x => x.ID == auxId && x.MissionType == MissionType.LongHaul);

			aux.AddCycle(auxId, entry => {
				if (entry.VehicleTargetSpeed >= Constants.SimulationSettings.HighwaySpeedThreshold) {
					return motorway.PowerDemandMech;
				}

				if (entry.VehicleTargetSpeed >= Constants.SimulationSettings.RuralSpeedThreshold) {
					return rural.PowerDemandMech;
				}

				return urban.PowerDemandMech;
			});
		}

		public IShiftStrategy GetShiftStrategy(IVehicleContainer container)
		{
			var runData = container.RunData;
			
			return ShiftStrategyFactory.GetShiftStrategy(runData.GetShiftStrategy(), container);
		}

	}
}