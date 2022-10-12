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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
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

		public VehicleContainer Build(VectoRunData data)
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
					throw new VectoException("Powertrain Builder cannot build Powertrain for CycleType: {0}", data.Cycle.CycleType);
			}
		}

		private VehicleContainer BuildEngineOnly(VectoRunData data)
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

		private VehicleContainer BuildPWheel(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.PWheel) {
				throw new VectoException("CycleType must be PWheel.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, _modData, _sumWriter) { RunData = data };
			var gearbox = new CycleGearbox(container);

			// PWheelCycle --> AxleGear --> Clutch --> Engine <-- Aux
			var powertrain = new PWheelCycle(container, data.Cycle)
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(gearbox, data.Retarder, container)
				.AddComponent(new Clutch(container, data.EngineData));
			var engine = new StopStartCombustionEngine(container, data.EngineData, pt1Disabled: true);
			var idleController = GetIdleController(data.PTO, engine, container);

			powertrain.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);

			return container;
		}

		private VehicleContainer BuildVTP(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.VTP) {
				throw new VectoException("CycleType must be VTP.");
			}

			var container = new VehicleContainer(data.ExecutionMode, _modData, _sumWriter) { RunData = data };
			var gearbox = new VTPGearbox(container);

			// VTPCycle --> AxleGear --> Clutch --> Engine <-- Aux
			var powertrain = new VTPCycle(container, data.Cycle)
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(gearbox, data.Retarder, container)
				.AddComponent(new Clutch(container, data.EngineData));
			var engine = new VTPCombustionEngine(container, data.EngineData, pt1Disabled: true);
			
			var aux = CreateSpeedDependentAuxiliaries(data, container);
			var engineFan = new EngineFanAuxiliary(data.FanData.FanCoefficients, data.FanData.FanDiameter);
			aux.AddCycle(Constants.Auxiliaries.IDs.Fan, cycleEntry => engineFan.PowerDemand(cycleEntry));
			container.ModalData.AddAuxiliary(Constants.Auxiliaries.IDs.Fan);

            if (data.PTO != null) {
                aux.AddConstant(Constants.Auxiliaries.IDs.PTOTransmission,
                    DeclarationData.PTOTransmission.Lookup(data.PTO.TransmissionType).PowerDemand);
                container.ModalData.AddAuxiliary(Constants.Auxiliaries.IDs.PTOTransmission,
                    Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOTransmission);

                aux.Add(Constants.Auxiliaries.IDs.PTOConsumer,
                    (n, absTime, dt, dryRun) => container.DrivingCycleInfo.PTOActive ? null : data.PTO.LossMap.GetTorqueLoss(n) * n);
                container.ModalData.AddAuxiliary(Constants.Auxiliaries.IDs.PTOConsumer,
                    Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOConsumer);
            }

            engine.Connect(aux.Port());

			var idleController = new CombustionEngine.CombustionEngineNoDubleclutchIdleController(engine, container);
			//if (data.PTO != null && data.PTO.PTOCycle != null) {
			//    var ptoController = new PTOCycleController(container, data.PTO.PTOCycle);
			//    idleController = new IdleControllerSwitcher(engine.IdleController, ptoController);
			//}

			powertrain.AddComponent(engine, idleController);
				//.AddAuxiliaries(container, data);

			return container;
		}



		private VehicleContainer BuildMeasuredSpeed(VectoRunData data)
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
			//_modData.HasTorqueConverter = data.GearboxData.Type.AutomaticTransmission();

			return container;
		}

		private VehicleContainer BuildMeasuredSpeedGear(VectoRunData data)
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
				.AddComponent(new CycleGearbox(container));
			if (data.GearboxData.Type.ManualTransmission()) {
				powertrain = powertrain.AddComponent(new Clutch(container, data.EngineData));
			} else {
				new ATClutchInfo(container);
			}
			powertrain.AddComponent(new StopStartCombustionEngine(container, data.EngineData))
				.AddAuxiliaries(container, data);

			//_modData.HasTorqueConverter = data.GearboxData.Type.AutomaticTransmission();

			return container;
		}

		private VehicleContainer BuildFullPowertrain(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}

			var container = new VehicleContainer(data.ExecutionMode, _modData, _sumWriter) { RunData = data };

			// DistanceBasedDrivingCycle --> driver --> vehicle --> wheels 
			// --> axleGear --> (retarder) --> gearBox --> (retarder) --> clutch --> engine <-- Aux
			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
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

			//_modData.HasTorqueConverter = data.GearboxData.Type.AutomaticTransmission();

			return container;
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
				case CycleType.EngineOnly:
					break;
				default:
					throw new VectoException("Wrong CycleType for SimplePowertrain");
			}


			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var idleController = GetIdleController(data.PTO, engine, container);
			//cycle.IdleController = idleController as IdleControllerSwitcher;

			powertrain.AddComponent(engine, idleController)
				.AddAuxiliaries(container, data);
		}

		private static IGearbox GetSimpleGearbox(IVehicleContainer container, VectoRunData runData)
		{
			if (runData.GearboxData.Type.AutomaticTransmission()) {
				new ATClutchInfo(container);
				return new ATGearbox(container, null);
			}
			return new Gearbox(container, null);
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


		private static IIdleController GetIdleController(PTOData pto, ICombustionEngine engine, IVehicleContainer container)
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
				container.ModalData?.AddAuxiliary(id);
			}

			RoadSweeperAuxiliary rdSwpAux = null;
			PTODriveAuxiliary ptoDrive = null;
			

			if (data.ExecutionMode == ExecutionMode.Engineering && data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityRoadSweeping)) {
				if (data.DriverData.PTODriveMinSpeed == null) {
					throw new VectoSimulationException("PTO activity 'road sweeping' requested, but no min. engine speed or gear provided");
				}
				rdSwpAux = new RoadSweeperAuxiliary(container);
				aux.Add(Constants.Auxiliaries.IDs.PTORoadsweeping, (nEng, absTime, dt, dryRun) => rdSwpAux.PowerDemand(nEng, absTime, dt, dryRun));
				container.ModalData?.AddAuxiliary(Constants.Auxiliaries.IDs.PTORoadsweeping, Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTORoadsweeping);
			}

			if (data.ExecutionMode == ExecutionMode.Engineering &&
				data.Cycle.Entries.Any(x => x.PTOActive == PTOActivity.PTOActivityWhileDrive)) {
				if (data.PTOCycleWhileDrive == null) {
					throw new VectoException("PTO activation while drive requested in cycle but no PTO cycle provided");
				}

				ptoDrive = new PTODriveAuxiliary(container, data.PTOCycleWhileDrive);
				aux.Add(Constants.Auxiliaries.IDs.PTODuringDrive, (nEng, absTime, dt, dryRun) => ptoDrive.PowerDemand(nEng, absTime, dt, dryRun));
				container.ModalData?.AddAuxiliary(Constants.Auxiliaries.IDs.PTODuringDrive, Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTODuringDrive);
			}
			if (data.PTO != null) {
				aux.AddConstant(Constants.Auxiliaries.IDs.PTOTransmission,
								DeclarationData.PTOTransmission.Lookup(data.PTO.TransmissionType).PowerDemand);
				container.ModalData?.AddAuxiliary(Constants.Auxiliaries.IDs.PTOTransmission,
												Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOTransmission);

				aux.Add(Constants.Auxiliaries.IDs.PTOConsumer,
						(n, absTime, dt, dryRun) => container.DrivingCycleInfo.PTOActive || (rdSwpAux?.Active(absTime) ?? false) || (ptoDrive?.Active(absTime) ?? false) ? null : data.PTO.LossMap.GetTorqueLoss(n) * n);
				container.ModalData?.AddAuxiliary(Constants.Auxiliaries.IDs.PTOConsumer,
												Constants.Auxiliaries.PowerPrefix + Constants.Auxiliaries.IDs.PTOConsumer);
			}
			return aux;
		}

		private EngineAuxiliary CreateSpeedDependentAuxiliaries(VectoRunData data, IVehicleContainer container)
		{
			var aux = new EngineAuxiliary(container);
 
			var auxData = data.Aux.ToArray();
			AddSwitchingAux(aux,container.ModalData,Constants.Auxiliaries.IDs.HeatingVentilationAirCondition, auxData);
			AddSwitchingAux(aux,container.ModalData,Constants.Auxiliaries.IDs.SteeringPump, auxData);
			AddSwitchingAux(aux,container.ModalData,Constants.Auxiliaries.IDs.ElectricSystem, auxData);
			AddSwitchingAux(aux, container.ModalData, Constants.Auxiliaries.IDs.PneumaticSystem, auxData);

			return aux;
		}

		private void AddSwitchingAux(EngineAuxiliary aux, IModalDataContainer modData, string auxId, VectoRunData.AuxData[] auxData)
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
			var strategy = GetShiftStrategy(container);
			return GetGearbox(container, strategy);
		}

		private static IGearbox GetGearbox(IVehicleContainer container, IShiftStrategy strategy)
		{
			switch (container.RunData.GearboxData.Type) {
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
			//var shiftStrategy = runData.ShiftStrategy;
			//if (string.IsNullOrWhiteSpace(shiftStrategy)) {
			//	shiftStrategy = DeclarationData.GearboxTCU.DefaultShiftStrategy;
			//}

			//if (string.IsNullOrWhiteSpace(shiftStrategy)) {
				switch (runData.GearboxData.Type) {
					case GearboxType.AMT:
						//if (runData.JobType == VectoSimulationJobType.ConventionalVehicle) {
							runData.ShiftStrategy = AMTShiftStrategyOptimized.Name;
							return new AMTShiftStrategyOptimized(container);
						//}

						//if (runData.JobType == VectoSimulationJobType.BatteryElectricVehicle) {
						//	runData.ShiftStrategy = PEVAMTShiftStrategy.Name;
						//	return new PEVAMTShiftStrategy(container);
						//}
						//throw new VectoException("no default gearshift strategy available for gearbox type {0} and job type {1}", runData.GearboxData.Type, runData.JobType);
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
							$"Unknown Gearbox Type {runData.GearboxData.Type.ToString()}");
				}
			//}

			//var selected = ShiftStrategies.FirstOrDefault(x =>
			//	x.Item1.Contains(runData.GearboxData.Type) &&
			//	x.Item2.Equals(shiftStrategy, StringComparison.InvariantCultureIgnoreCase));
			//if (selected == null) {
			//	throw new ArgumentOutOfRangeException("ShiftStrategy",
			//		$"Unknown Shiftstrategy {shiftStrategy} for Gearbox Type {runData.GearboxData.Type.ToString()}");
			//}

			//runData.ShiftStrategy = selected.Item3;
			//return selected.Item4(container);
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
}