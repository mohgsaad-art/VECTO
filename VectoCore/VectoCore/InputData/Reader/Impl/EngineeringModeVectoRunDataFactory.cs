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
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

[assembly: InternalsVisibleTo("VectoCoreTest")]

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class EngineeringModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		private static readonly Dictionary<string, Tuple<DrivingCycleData, DateTime>> CyclesCache = new Dictionary<string, Tuple<DrivingCycleData, DateTime>>();

		protected readonly IEngineeringInputDataProvider InputDataProvider;

		internal EngineeringModeVectoRunDataFactory(IEngineeringInputDataProvider dataProvider)
		{
			InputDataProvider = dataProvider;
		}

		/// <summary>
		/// Iterate over all cycles defined in the JobFile and create a container with all data required for creating a simulation run
		/// </summary>
		/// <returns>VectoRunData instance for initializing the powertrain.</returns>
		public virtual IEnumerable<VectoRunData> NextRun()
		{

			switch(InputDataProvider.JobInputData.JobType) {
				case VectoSimulationJobType.ConventionalVehicle:
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.IHPC:
				case VectoSimulationJobType.EngineOnlySimulation:
					return GetConventionalVehicleRunData();
				case VectoSimulationJobType.BatteryElectricVehicle:
					return GetBatteryElectricVehicleRunData();
				case VectoSimulationJobType.SerialHybridVehicle:
					return GetSerialHybridRunData();
				case VectoSimulationJobType.IEPC_E:
					return GetIEPCRunData();
				case VectoSimulationJobType.IEPC_S:
					return GetIEPC_S_RunData();
				default:
					throw new ArgumentOutOfRangeException($"Invalid JobType {InputDataProvider.JobInputData.JobType}");
			}
		}

		private IEnumerable<VectoRunData> GetSerialHybridRunData()
		{
			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				var engineMode = engineModes[modeIdx];
				foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
					var dao = new EngineeringDataAdapter();
					var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);
					if (InputDataProvider.JobInputData.JobType != VectoSimulationJobType.ConventionalVehicle) {
						driver.EngineStopStart.UtilityFactorDriving = 1;
						driver.EngineStopStart.UtilityFactorStandstill = 1;
					}

					var vehicle = InputDataProvider.JobInputData.Vehicle;
					var engineData = dao.CreateEngineData(vehicle, engineMode);
					engineData.FuelMode = modeIdx;

					var battery = dao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
					var superCap = dao.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);

					var averageVoltage = battery != null 
						? CalculateAverageVoltage(battery)
							: null;

					var axlegearData = vehicle.Components.AxleGearInputData != null
						? dao.CreateAxleGearData(vehicle.Components.AxleGearInputData)
						: null;
					var electricMachinesData = dao.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits, averageVoltage);

					GearboxData gearboxData = null;
					ShiftStrategyParameters gearshiftParams = null;
					var angledriveData = dao.CreateAngledriveData(vehicle.Components.AngledriveInputData);
					if (electricMachinesData == null || electricMachinesData.Count == 0)
						throw new ArgumentNullException("Electric machines are missing in vehicle.");
					
					if (electricMachinesData.Any(x => x.Item1 == PowertrainPosition.BatteryElectricE2)) {
						// gearbox required!
						gearshiftParams = dao.CreateGearshiftData(
							InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.Type, InputDataProvider.DriverInputData.GearshiftInputData,
							axlegearData.AxleGear.Ratio * (angledriveData?.Angledrive.Ratio ?? 1.0), null);
						var tmpRunData = new VectoRunData() {
							JobType = VectoSimulationJobType.SerialHybridVehicle,
							GearboxData = new GearboxData() {
								Type = vehicle.Components.GearboxInputData.Type,
							},
							GearshiftParameters = gearshiftParams,
							ElectricMachinesData = electricMachinesData,
							//VehicleData = dao.CreateVehicleData(vehicle)
						};
						var tempVehicle = dao.CreateVehicleData(vehicle);
						var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
						gearboxData = dao.CreateGearboxData(
							InputDataProvider, new VectoRunData() {
								JobType = VectoSimulationJobType.SerialHybridVehicle,
								VehicleData = tempVehicle,
								AxleGearData = axlegearData,
								ElectricMachinesData = electricMachinesData
							}, tmpStrategy);
						angledriveData = dao.CreateAngledriveData(vehicle.Components.AngledriveInputData);

					}

					if (gearshiftParams == null) {
						gearshiftParams = new ShiftStrategyParameters() {
							StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
							StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
							TimeBetweenGearshifts = DeclarationData.Gearbox.MinTimeBetweenGearshifts,
							DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
							UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
							UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
						};
					}

					var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
											CrossWindCorrectionMode.VAirBetaLookupTable;
					var ptoTransmissionData =
						dao.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

					if (InputDataProvider.JobInputData.Vehicle.PTO_DriveGear != null &&
						InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed != null) {
						driver.PTODriveMinSpeed = InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed /
							axlegearData.AxleGear.Ratio /
							gearboxData.Gears[InputDataProvider.JobInputData.Vehicle.PTO_DriveGear.Gear].Ratio /
							(angledriveData?.Angledrive.Ratio ?? 1.0) * vehicle.DynamicTyreRadius;
						driver.PTODriveRoadsweepingGear = InputDataProvider.JobInputData.Vehicle.PTO_DriveGear;
						engineData.PTORoadSweepEngineSpeed =
							InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed;
					}

					var ptoCycleWhileDrive =
						InputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData
							.PTOCycleWhileDriving != null
							? DrivingCycleDataReader.ReadFromDataTable(
								InputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData
									.PTOCycleWhileDriving,
								"PTO During Drive", false)
							: null;


					var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);

					var electricMachines =
						dao.CreateElectricMachines(vehicle.Components.ElectricMachines,
							vehicle.ElectricMotorTorqueLimits, averageVoltage) ??
						new List<Tuple<PowertrainPosition, ElectricMotorData>>();
					var powertrainPosition = electricMachines.First(e => e.Item1 != PowertrainPosition.GEN).Item1;
					var jobType = VectoSimulationJobType.SerialHybridVehicle;
					var vehicleData = dao.CreateVehicleData(vehicle);
					var hybridParameters = dao.CreateHybridStrategyParameters(InputDataProvider.JobInputData, 
						engineData, gearboxData);

					yield return new VectoRunData {
						JobName = InputDataProvider.JobInputData.JobName,
						JobType = jobType,
						EngineData = engineData,
						GearboxData = gearboxData,
						AxleGearData = axlegearData,
						AngledriveData = angledriveData,
						VehicleData = vehicleData,
						AirdragData = dao.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
						DriverData = driver,
						Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
						BusAuxiliaries = dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, 
							vehicleData, jobType),
						Retarder = dao.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition),
						PTO = ptoTransmissionData,
						Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
						ExecutionMode = ExecutionMode.Engineering,
						PTOCycleWhileDrive = ptoCycleWhileDrive,
						ElectricMachinesData = electricMachines,
						HybridStrategyParameters = hybridParameters,
						BatteryData = battery,
						SuperCapData = superCap,
						SimulationType = SimulationType.DistanceCycle
										| SimulationType.MeasuredSpeedCycle 
										| SimulationType.PWheel,
						GearshiftParameters = gearshiftParams,
						ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData
							.Auxiliaries.ElectricPowerDemand,
					};
				}
			}
		
		}

		private DrivingCycleData GetDrivingCycle(ICycleData cycle, bool crossWindRequired)
		{
			var lastModified = File.GetLastWriteTimeUtc(cycle.CycleData.Source);
			if (CyclesCache.TryGetValue(cycle.CycleData.Source, out var lookup)) {
				if (lastModified.Equals(lookup.Item2)) {
					return lookup.Item1;
				}
			}

			lock (CyclesCache) {
				var cycleData =
					DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, crossWindRequired);
				CyclesCache[cycle.CycleData.Source] = Tuple.Create(cycleData, lastModified);
				return cycleData;
			}
		}

		private IEnumerable<VectoRunData> GetBatteryElectricVehicleRunData()
		{
			foreach (var cycle in InputDataProvider.JobInputData.Cycles)
			{
				var dao = new EngineeringDataAdapter();
				var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var axlegearData = vehicle.Components.AxleGearInputData != null
					? dao.CreateAxleGearData(vehicle.Components.AxleGearInputData)
					: null;

				var batteryData = dao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
				var supercapData = dao.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);

				var averageVoltage = batteryData != null ? CalculateAverageVoltage(batteryData) : null;
				var electricMachinesData = dao.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits, averageVoltage);
				var powertrainPosition = electricMachinesData.First(e => e.Item1 != PowertrainPosition.GEN).Item1;
				GearboxData gearboxData = null;
				ShiftStrategyParameters gearshiftParams = null;
				var angledriveData = dao.CreateAngledriveData(vehicle.Components.AngledriveInputData);
				if (electricMachinesData.Any(x => x.Item1 == PowertrainPosition.BatteryElectricE2)) {
					// gearbox required!
					gearshiftParams = dao.CreateGearshiftData(
						InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.Type, InputDataProvider.DriverInputData.GearshiftInputData,
						axlegearData.AxleGear.Ratio * (angledriveData?.Angledrive.Ratio ?? 1.0), null);
					var tmpRunData = new VectoRunData() {
						JobType = VectoSimulationJobType.BatteryElectricVehicle,
						GearboxData = new GearboxData() {
							Type = vehicle.Components.GearboxInputData.Type,
						},
						GearshiftParameters = gearshiftParams,
						ElectricMachinesData = electricMachinesData,
						//VehicleData = dao.CreateVehicleData(vehicle)
					};
					var tempVehicle = dao.CreateVehicleData(vehicle);
					var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
					gearboxData = dao.CreateGearboxData(
						InputDataProvider, new VectoRunData() {
							JobType = VectoSimulationJobType.BatteryElectricVehicle,
							VehicleData = tempVehicle,
							AxleGearData = axlegearData,
							ElectricMachinesData = electricMachinesData
						}, tmpStrategy);
					angledriveData = dao.CreateAngledriveData(vehicle.Components.AngledriveInputData);
					
				}

				if (gearshiftParams == null) {
					gearshiftParams = new ShiftStrategyParameters() {
						StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
						StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
					};
				}

				var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
										CrossWindCorrectionMode.VAirBetaLookupTable;
				var ptoTransmissionData = dao.CreateBatteryElectricPTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

				var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);

				var vehicleData = dao.CreateVehicleData(vehicle);
				yield return new VectoRunData
				{
					JobName = InputDataProvider.JobInputData.JobName,
					JobType = VectoSimulationJobType.BatteryElectricVehicle,
					GearboxData = gearboxData,
					AxleGearData = axlegearData,
					AngledriveData = angledriveData,
					VehicleData = vehicleData,
					AirdragData = dao.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
					DriverData = driver,
					Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
					BusAuxiliaries = dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData, VectoSimulationJobType.BatteryElectricVehicle),
					Retarder = dao.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition),
					PTO = ptoTransmissionData,
					Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
					ExecutionMode = ExecutionMode.Engineering,
					ElectricMachinesData = electricMachinesData,
					//HybridStrategyParameters = dao.CreateHybridStrategyParameters(InputDataProvider.JobInputData.HybridStrategyParameters),
					BatteryData = batteryData,
					SuperCapData = supercapData,
					SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel,
					GearshiftParameters = gearshiftParams,
					ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries.ElectricPowerDemand,
				};
			}
		}

		private IEnumerable<VectoRunData> GetIEPCRunData()
		{
			var axleGearRequired = AxleGearRequired();
			var dao = new EngineeringDataAdapter();
			foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
				yield return GetIEPCVectoRunData(axleGearRequired, cycle, dao);
			}
		}

		private bool AxleGearRequired()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			var iepcInput = vehicle.Components.IEPCEngineeringInputData;
			var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
			if (axleGearRequired && vehicle.Components.AxleGearInputData == null) {
				throw new VectoException(
					$"Axlegear reqhired for selected type of IEPC! DifferentialIncluded: {iepcInput.DifferentialIncluded}, DesignTypeWheelMotor: {iepcInput.DesignTypeWheelMotor}");
			}

			var numGearsPowermap =
				iepcInput.VoltageLevels.Select(x => Tuple.Create(x.VoltageLevel, x.PowerMap.Count)).ToArray();
			var gearCount = iepcInput.Gears.Count;
			var numGearsDrag = iepcInput.DragCurves.Count;

			if (numGearsPowermap.Any(x => x.Item2 != gearCount)) {
				throw new VectoException(
					$"Number of gears for voltage levels does not match! PowerMaps: {numGearsPowermap.Select(x => $"{x.Item1}: {x.Item2}").Join()}; Gear count: {gearCount}");
			}

			if (numGearsDrag > 1 && numGearsDrag != gearCount) {
				throw new VectoException(
					$"Number of gears drag curve does not match gear count! DragCurve {numGearsDrag}; Gear count: {gearCount}");
			}

			return axleGearRequired;
		}

		private IEnumerable<VectoRunData> GetIEPC_S_RunData()
		{
			var axleGearRequired = AxleGearRequired();

			var vehicle = InputDataProvider.JobInputData.Vehicle;
			var engine = vehicle.Components.EngineInputData;
			if (engine == null) {
				throw new VectoException("Combustion Engine is required for IEPC-S vehicle configurations");
			}
			var engineModes = engine.EngineModes;

			var dao = new EngineeringDataAdapter();
			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				var engineMode = engineModes[modeIdx];
				foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
					var engineData = dao.CreateEngineData(vehicle, engineMode);
					engineData.FuelMode = modeIdx;
					
					var runData = GetIEPCVectoRunData(axleGearRequired, cycle, dao);
					var averageVoltage = CalculateAverageVoltage(runData.BatteryData);
					
					var electricMachinesData = dao.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits, averageVoltage);

					var hybridParameters = dao.CreateHybridStrategyParameters(InputDataProvider.JobInputData,
						engineData, runData.GearboxData);

					foreach (var gear in runData.GearboxData.Gears.Keys) {
						engineData.FullLoadCurves[gear] = engineData.FullLoadCurves[0];
					}
					runData.JobType = VectoSimulationJobType.IEPC_S;
					runData.EngineData = engineData;
					runData.ElectricMachinesData.Add(electricMachinesData.First(x => x.Item1 == PowertrainPosition.GEN));
					runData.HybridStrategyParameters = hybridParameters;
					yield return runData;
				}
			}
		}


		private VectoRunData GetIEPCVectoRunData(bool axleGearRequired, ICycleData cycle, EngineeringDataAdapter dao)
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			
			var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);

			var axlegearData = axleGearRequired && vehicle.Components.AxleGearInputData != null
				? dao.CreateAxleGearData(vehicle.Components.AxleGearInputData)
				: null;

			var batteryData = dao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
			var supercapData = dao.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);

			var averageVoltage = batteryData != null ? CalculateAverageVoltage(batteryData) : null;
			var electricMachinesData =
				dao.CreateIEPCElectricMachines(vehicle.Components.IEPCEngineeringInputData, averageVoltage);
			var powertrainPosition = electricMachinesData.First(e => e.Item1 != PowertrainPosition.GEN).Item1;
			var retarderData = axleGearRequired
				? dao.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition)
				: new RetarderData() {
					Type = RetarderType.LossesIncludedInTransmission,
					Ratio = 1.0
				};

			var gearshiftParams = dao.CreateGearshiftData(GearboxType.APTN,
				InputDataProvider.DriverInputData.GearshiftInputData,
				axlegearData?.AxleGear.Ratio ?? 1.0, null);
			var tmpRunData = new VectoRunData() {
				JobType = VectoSimulationJobType.IEPC_E,
				GearboxData = new GearboxData() {
					Type = GearboxType.APTN,
				},
				GearshiftParameters = gearshiftParams,
				ElectricMachinesData = electricMachinesData,
				//VehicleData = dao.CreateVehicleData(vehicle)
			};
			var tempVehicle = dao.CreateVehicleData(vehicle);
			var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
			var gearboxData = dao.CreateIEPCGearboxData(
				InputDataProvider, new VectoRunData() {
					JobType = VectoSimulationJobType.IEPC_E,
					VehicleData = tempVehicle,
					AxleGearData = axlegearData,
					ElectricMachinesData = electricMachinesData
				}, tmpStrategy);

			var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
									CrossWindCorrectionMode.VAirBetaLookupTable;
			//var ptoTransmissionData = dao.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

			var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);

			var vehicleData = dao.CreateVehicleData(vehicle);
			return new VectoRunData {
				JobName = InputDataProvider.JobInputData.JobName,
				JobType = VectoSimulationJobType.IEPC_E,
				GearboxData = gearboxData,
				AxleGearData = axlegearData,
				AngledriveData = null,
				VehicleData = vehicleData,
				AirdragData = dao.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
				DriverData = driver,
				Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
				BusAuxiliaries = dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData,
					VectoSimulationJobType.BatteryElectricVehicle),
				Retarder = retarderData,
				//PTO = ptoTransmissionData,
				Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
				ExecutionMode = ExecutionMode.Engineering,
				ElectricMachinesData = electricMachinesData,
				//HybridStrategyParameters = dao.CreateHybridStrategyParameters(InputDataProvider.JobInputData.HybridStrategyParameters),
				BatteryData = batteryData,
				SuperCapData = supercapData,
				SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel,
				GearshiftParameters = gearshiftParams,
				ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries
					.ElectricPowerDemand,
			};
		}


		protected virtual IEnumerable<VectoRunData> GetConventionalVehicleRunData()
		{
			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			//foreach (var engineMode in engineModes) {
			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				var engineMode = engineModes[modeIdx];
				foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
					var dao = new EngineeringDataAdapter();
					var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);
					if (InputDataProvider.JobInputData.JobType != VectoSimulationJobType.ConventionalVehicle) {
						driver.EngineStopStart.UtilityFactorDriving = 1;
					}
					
					var vehicle = InputDataProvider.JobInputData.Vehicle;
					var engineData = dao.CreateEngineData(vehicle, engineMode);
					engineData.FuelMode = modeIdx;
					var tempVehicle = dao.CreateVehicleData(vehicle);

					var axlegearData = dao.CreateAxleGearData(vehicle.Components.AxleGearInputData);
					var tmpRunData = new VectoRunData() {
						JobType = InputDataProvider.JobInputData.JobType,
						GearboxData = new GearboxData() {
							Type = vehicle.Components.GearboxInputData.Type,
						}
					};
					var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
					
					var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
											CrossWindCorrectionMode.VAirBetaLookupTable;

					var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);
					var drivingCycleProxy = new DrivingCycleProxy(drivingCycle, cycle.Name);

					var gearboxData = dao.CreateGearboxData(
						InputDataProvider, new VectoRunData() {
							EngineData = engineData,
							VehicleData = tempVehicle,
							AxleGearData = axlegearData,
							Cycle = drivingCycleProxy
						}, tmpStrategy);

					var angledriveData = dao.CreateAngledriveData(vehicle.Components.AngledriveInputData);
					var ptoTransmissionData =
						dao.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

					if (InputDataProvider.JobInputData.Vehicle.PTO_DriveGear != null &&
						InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed != null) {
						driver.PTODriveMinSpeed = InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed /
							axlegearData.AxleGear.Ratio /
							gearboxData.Gears[InputDataProvider.JobInputData.Vehicle.PTO_DriveGear.Gear].Ratio /
							(angledriveData?.Angledrive.Ratio ?? 1.0) * vehicle.DynamicTyreRadius;
						driver.PTODriveRoadsweepingGear = InputDataProvider.JobInputData.Vehicle.PTO_DriveGear;
						engineData.PTORoadSweepEngineSpeed =
							InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed;
					}

					var ptoCycleWhileDrive =
						InputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData
							.PTOCycleWhileDriving != null
							? DrivingCycleDataReader.ReadFromDataTable(
								InputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData
									.PTOCycleWhileDriving,
								"PTO During Drive", false)
							: null;

					var battery = dao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
					var superCap = dao.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
					var averageVoltage = battery != null ? CalculateAverageVoltage(battery): null;

					var electricMachines =
						dao.CreateElectricMachines(vehicle.Components.ElectricMachines,
							vehicle.ElectricMotorTorqueLimits, averageVoltage, gearboxData.GearList) ??
						new List<Tuple<PowertrainPosition, ElectricMotorData>>();
					var powertrainPosition = electricMachines.FirstOrDefault(e => e.Item1 != PowertrainPosition.GEN)?.Item1 ?? PowertrainPosition.HybridPositionNotSet;
					
					var jobType = electricMachines.Count > 0 && (battery != null || superCap != null)
						? VectoSimulationJobType.ParallelHybridVehicle
						: VectoSimulationJobType.ConventionalVehicle;

					if (powertrainPosition == PowertrainPosition.HybridP2 && gearboxData.Type.AutomaticTransmission()) {
						throw new VectoException(
							"Powertrain Architecture 'hybrid electric vehicle, P2' with AT transmission not supported");
					}
					var vehicleData = dao.CreateVehicleData(vehicle);

					var gearshiftParams = dao.CreateGearshiftData(
						gearboxData.Type, InputDataProvider.DriverInputData.GearshiftInputData,
						axlegearData.AxleGear.Ratio * (angledriveData?.Angledrive.Ratio ?? 1.0),
						engineData.IdleSpeed);
					if (gearshiftParams == null) {
						gearshiftParams = new ShiftStrategyParameters() {
							StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
							StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
							TimeBetweenGearshifts = DeclarationData.Gearbox.MinTimeBetweenGearshifts,
							DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
							UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
							UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
						};
					}
					
					var hybridParameters = jobType == VectoSimulationJobType.ParallelHybridVehicle
						? dao.CreateHybridStrategyParameters(InputDataProvider.JobInputData, engineData, gearboxData)
						: null;
					yield return new VectoRunData {
						JobName = InputDataProvider.JobInputData.JobName,
						JobType = jobType,
						EngineData = engineData,
						GearboxData = gearboxData,
						AxleGearData = axlegearData,
						AngledriveData = angledriveData,
						VehicleData = vehicleData,
						AirdragData = dao.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
						DriverData = driver,
						Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
						BusAuxiliaries =
							dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData, jobType),
						Retarder = dao.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition),
						PTO = ptoTransmissionData,
						Cycle = drivingCycleProxy,
						ExecutionMode = ExecutionMode.Engineering,
						PTOCycleWhileDrive = ptoCycleWhileDrive,

						ElectricMachinesData = electricMachines,
						HybridStrategyParameters = hybridParameters,
						BatteryData = battery,
						SuperCapData = superCap,
						SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle |
										SimulationType.PWheel,
						GearshiftParameters = gearshiftParams,
						ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData
							.Auxiliaries.ElectricPowerDemand,
					};
				}
			}
		}

		private Volt CalculateAverageVoltage(BatterySystemData batteryData)
		{
			// use the battery system to get min/max SoC of the whole battery system (multiple batteries in series/parallel)
			//   the battery system already contains all necessary models, no need to duplicate here the rather complex calculations
			var tmpBattery = new BatterySystem(null, batteryData);
			var min = tmpBattery.MinSoC;
			var max = tmpBattery.MaxSoC;
			var averageSoC = (min + max) / 2.0;

			tmpBattery.Initialize(averageSoC);
			return tmpBattery.InternalVoltage;
		}
	}
}
