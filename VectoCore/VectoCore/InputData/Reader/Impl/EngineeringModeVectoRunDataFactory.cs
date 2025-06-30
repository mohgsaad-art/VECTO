#define TRACE_FC
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
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.Utils;

[assembly: InternalsVisibleTo("VectoCoreTest")]

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
    public class EngineeringModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		public const string Name = "EngineeringModeVectoRunDataFactory";

        private static readonly Dictionary<string, Tuple<DrivingCycleData, DateTime>> CyclesCache = new Dictionary<string, Tuple<DrivingCycleData, DateTime>>();

		protected readonly IEngineeringInputDataProvider InputDataProvider;
		protected IEngineeringDataAdapter DataAdapter { get; }

		protected IPowertrainBuilder PowertrainBuilder { get; }
		/// Used only for debug output
		/// </summary>
		public IOutputDataWriter Writer { get; set; }


        internal EngineeringModeVectoRunDataFactory(IEngineeringInputDataProvider dataProvider,
			IPowertrainBuilder ptBuilder, IEngineeringDataAdapter dataAdapter)
		{
			InputDataProvider = dataProvider;
			PowertrainBuilder = ptBuilder;
			DataAdapter = dataAdapter;
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
				case VectoSimulationJobType.FCHV:
					return GetFCHV_RunData(GetBatteryElectricVehicleRunData, VectoSimulationJobType.FCHV);
				case VectoSimulationJobType.FCHV_IEPC:
					return GetFCHV_RunData(GetIEPCRunData, VectoSimulationJobType.FCHV_IEPC);
				case VectoSimulationJobType.Multiple_FCHV:
				case VectoSimulationJobType.Multiple_PEV:
				case VectoSimulationJobType.Multiple_SHEV:
					return GetMultiplePowertrainsRunData();
				default:
					throw new ArgumentOutOfRangeException($"Invalid JobType {InputDataProvider.JobInputData.JobType}");
			}
		}

		private IEnumerable<VectoRunData> GetFCHV_RunData(Func<IEnumerable<VectoRunData>> baseDataFunc, VectoSimulationJobType jobType)
		{
			var dao = new EngineeringDataAdapter() {

				DebugOutputDataWriter = Writer,
			};

			foreach (var pevRd in baseDataFunc()) {
				var fuelCellData = 
					dao.CreateFuelCellSystemData(InputDataProvider.JobInputData.Vehicle.Components
						.FuelCellSystemInputData);
                var iterativeRunStrategy = new FCHEVIterativeRunStrategy( new []{
					//Prerun, iteration 0
					new PreRunOptions() {
#if TRACE_FC
						WriteModAndSumData = true,
#else
						WriteModAndSumData = false
#endif
					},
					//Real run, iteration 1
					new PreRunOptions() {
						WriteModAndSumData = true
					}
				});

				//pevRd.BatteryData.ConnectionSystemResistance = 0.SI<Ohm>();
				pevRd.BatteryData = dao.CreateFuelCellPreProcessingBattery(InputDataProvider.JobInputData.Vehicle.Components.FuelCellSystemInputData, pevRd.BatteryData, out var fcBat);
				//pevRd.SimulationType = jobType;
#if TRACE_FC
				pevRd.ModFileSuffix += "pre";
#else

#endif

				iterativeRunStrategy.Update = (modData, runData) => {
					runData.JobType = jobType;
					runData.ModFileSuffix = "";
					modData.PostProcessingCorrection = new FCHVPostProcessingCorrection();
					//In case the battery is modified after creating the rundata (testing, do not create new battery data)
					pevRd.BatteryData.Batteries =
						pevRd.BatteryData.Batteries.Where(b => b.Item1 != fcBat.Item1).ToList();


                    //runData.BatteryData = pevBat;
                    //runData.BatteryData =
                    //	dao.CreateBatteryData(InputDataProvider.JobInputData.Vehicle.Components.ElectricStorage, 0.5);
					runData.FuelCellSystemData = fuelCellData;
					runData.FuelCellSystemData.FuelCellPowerMap =
						dao.CreateFuelCellPowerMap(modData, runData.FuelCellSystemData, runData.BatteryData);
					runData.FuelCellSystemData.FuelCellShareMap = dao.CreateFuelCellShareMap(fuelCellData);
					pevRd.BatteryData.ChargeSustainingBatterySystem = false; //In the real run we don't use a chargesustaining battery

                };
				pevRd.IterativeRunStrategy = iterativeRunStrategy;
				yield return pevRd;
			}
		}

		
		public IInputDataProvider DataProvider => InputDataProvider;

		public IVehicleDeclarationInputData CompletedVehicle { get; set; }

        private IEnumerable<VectoRunData> GetSerialHybridRunData()
		{
			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				var engineMode = engineModes[modeIdx];
				foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
					var driver = DataAdapter.CreateDriverData(InputDataProvider.DriverInputData);
					if (InputDataProvider.JobInputData.JobType != VectoSimulationJobType.ConventionalVehicle) {
						driver.EngineStopStart.UtilityFactorDriving = 1;
						driver.EngineStopStart.UtilityFactorStandstill = 1;
					}

					var vehicle = InputDataProvider.JobInputData.Vehicle;
					var engineData = DataAdapter.CreateEngineData(vehicle, engineMode);
					engineData.FuelMode = modeIdx;

					var battery = DataAdapter.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
					var superCap = DataAdapter.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);

					var averageVoltage = battery != null 
						? CalculateAverageVoltage(battery)
							: null;

					var axlegearData = vehicle.Components.AxleGearInputData != null
						? DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData)
						: null;
					var electricMachinesData = DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits, averageVoltage);

					//GearboxData gearboxData = null;
					//ShiftStrategyParameters gearshiftParams = null;
					var angledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
					if (electricMachinesData == null || electricMachinesData.Count == 0)
						throw new ArgumentNullException("Electric machines are missing in vehicle.");

					var ptoCycleWhileDrive =
						InputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData
							.PTOCycleWhileDriving != null
							? DrivingCycleDataReader.ReadFromDataTable(
								InputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData
									.PTOCycleWhileDriving,
								"PTO During Drive", false)
							: null;

					var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
											CrossWindCorrectionMode.VAirBetaLookupTable;
					var ptoTransmissionData =
						DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

                    var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);

					var electricMachines =
						DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines,
							vehicle.ElectricMotorTorqueLimits, averageVoltage) ??
						new List<Tuple<PowertrainPosition, ElectricMotorData>>();
					var powertrainPosition = electricMachines.First(e => e.Item1 != PowertrainPosition.GEN).Item1;
					var jobType = VectoSimulationJobType.SerialHybridVehicle;
					var vehicleData = DataAdapter.CreateVehicleData(vehicle);
					var wheelEndData = DataAdapter.CreateWheelEndData(vehicleData.VehicleClass, vehicle);
					
                    var retVal = new VectoRunData {
						JobName = InputDataProvider.JobInputData.JobName,
						JobType = jobType,
						EngineData = engineData,
						//GearboxData = gearboxData,
						AxleGearData = axlegearData,
						AngledriveData = angledriveData,
						VehicleData = vehicleData,
						AirdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
						WheelEndData = wheelEndData,
						DriverData = driver,
						Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
						BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData,
							vehicleData, jobType),
						Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition),
						PTO = ptoTransmissionData,
						Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
						ExecutionMode = ExecutionMode.Engineering,
						PTOCycleWhileDrive = ptoCycleWhileDrive,
						ElectricMachinesData = electricMachines,
						BatteryData = battery,
						SuperCapData = superCap,
						SimulationType = SimulationType.DistanceCycle
										| SimulationType.MeasuredSpeedCycle
										| SimulationType.PWheel,
						//GearshiftParameters = gearshiftParams,
						ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData
							.Auxiliaries.ElectricPowerDemand,
						InMotionCharging = vehicle.InMotionCharging.Enabled,
						InMotionChargingTechnology = IMCTechnology.NotApplicable,

                    };

					GearboxData gearboxData = null;
					ShiftStrategyParameters gearshiftParams = null;
                    if (electricMachinesData.Any(x => x.Item1 == PowertrainPosition.BatteryElectricE2)) {
						// gearbox required!
						gearshiftParams = DataAdapter.CreateGearshiftData(
							InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.Type, InputDataProvider.DriverInputData.GearshiftInputData,
							axlegearData.AxleGear.Ratio * (angledriveData?.Angledrive.Ratio ?? 1.0), null);
						retVal.GearshiftParameters = gearshiftParams;

                        gearboxData = DataAdapter.CreateGearboxData(
							InputDataProvider, retVal);
						angledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
						retVal.GearboxData = gearboxData;
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
						retVal.GearshiftParameters = gearshiftParams;
                    }

					var hybridParameters = DataAdapter.CreateHybridStrategyParameters(InputDataProvider.JobInputData,
						engineData, gearboxData);
					retVal.HybridStrategyParameters = hybridParameters;

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

					

					yield return retVal;
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

		private IEnumerable<VectoRunData> GetMultiplePowertrainsRunData()
		{
			foreach (var cycle in InputDataProvider.JobInputData.Cycles)
			{
				var dao = new EngineeringDataAdapter();
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var batteryData = dao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
				var averageVoltage = (batteryData != null) ? CalculateAverageVoltage(batteryData) : null;

				var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
										CrossWindCorrectionMode.VAirBetaLookupTable;

				var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);

				var vehicleData = dao.CreateVehicleData(vehicle);

				yield return new VectoRunData {
					JobName = InputDataProvider.JobInputData.JobName,
					JobType = InputDataProvider.JobInputData.JobType,
					VehicleData = vehicleData,
					WheelEndData = dao.CreateWheelEndData(vehicleData.VehicleClass, vehicle),
					AirdragData = dao.CreateAirdragData(
						vehicle.Components.AirdragInputData, 
						vehicle),
					DriverData = dao.CreateDriverData(InputDataProvider.DriverInputData),
					Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
					BusAuxiliaries = dao.CreateBusAuxiliariesData(
						vehicle.Components.AuxiliaryInputData, 
						vehicleData,
                        InputDataProvider.JobInputData.JobType),
					Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
					ExecutionMode = ExecutionMode.Engineering,
					BatteryData = batteryData,
					SuperCapData = dao.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC),
					SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel,
					ElectricAuxDemand = vehicle.Components.AuxiliaryInputData.Auxiliaries.ElectricPowerDemand,
					InMotionCharging = vehicle.InMotionCharging.Enabled,
					InMotionChargingTechnology = IMCTechnology.NotApplicable,
					AxlePowertrainsData = dao.CreateAxlePowertrainsData(InputDataProvider, averageVoltage, PowertrainBuilder)
				};
			}
		}

		private IEnumerable<VectoRunData> GetBatteryElectricVehicleRunData()
		{
			foreach (var cycle in InputDataProvider.JobInputData.Cycles)
			{
				var driver = DataAdapter.CreateDriverData(InputDataProvider.DriverInputData);
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var axlegearData = vehicle.Components.AxleGearInputData != null
					? DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData)
					: null;

				var batteryData = DataAdapter.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
				var supercapData = DataAdapter.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);

				var averageVoltage = batteryData != null ? CalculateAverageVoltage(batteryData) : null;
				var electricMachinesData = DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits, averageVoltage);
				var powertrainPosition = electricMachinesData.First(e => e.Item1 != PowertrainPosition.GEN).Item1;
				GearboxData gearboxData = null;
				ShiftStrategyParameters gearshiftParams = null;
				var vehicleData = DataAdapter.CreateVehicleData(vehicle);
                var angledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData); 
				var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
										CrossWindCorrectionMode.VAirBetaLookupTable;
				var ptoTransmissionData = DataAdapter.CreateBatteryElectricPTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

				var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);
				var wheelEndData = DataAdapter.CreateWheelEndData(vehicleData.VehicleClass, vehicle);

                var retVal = new VectoRunData {
				

					JobName = InputDataProvider.JobInputData.JobName,
					JobType = VectoSimulationJobType.BatteryElectricVehicle,
					GearboxData = gearboxData,
					AxleGearData = axlegearData,
					VehicleData = vehicleData,
					AirdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
					WheelEndData = wheelEndData,
					DriverData = driver,
					Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
					BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData, VectoSimulationJobType.BatteryElectricVehicle),
					Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition),
					PTO = ptoTransmissionData,
					Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
					ExecutionMode = ExecutionMode.Engineering,
					ElectricMachinesData = electricMachinesData,
					BatteryData = batteryData,
					SuperCapData = supercapData,
					SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel,
					//GearshiftParameters = gearshiftParams,
					ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries.ElectricPowerDemand,
					InMotionCharging = vehicle.InMotionCharging.Enabled,
					InMotionChargingTechnology = IMCTechnology.NotApplicable,
				};
                if (electricMachinesData.Any(x => x.Item1 == PowertrainPosition.BatteryElectricE2)) {
					// gearbox required!
					gearshiftParams = DataAdapter.CreateGearshiftData(
						InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.Type, InputDataProvider.DriverInputData.GearshiftInputData,
						//Is the Axlegear obligatory for E2 Vehicles?
						axlegearData?.AxleGear.Ratio ?? 1.0 * (angledriveData?.Angledrive.Ratio ?? 1.0), null);
                    retVal.GearshiftParameters = gearshiftParams;
                    gearboxData = DataAdapter.CreateGearboxData(InputDataProvider, retVal);
					angledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
				}

				retVal.AngledriveData = angledriveData;

				if (gearshiftParams == null) {
					gearshiftParams = new ShiftStrategyParameters() {
						StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
						StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
					};
					retVal.GearshiftParameters = gearshiftParams;
                }

				retVal.GearboxData = gearboxData;
				
				yield return retVal;
			}
		}

		

		private IEnumerable<VectoRunData> GetIEPCRunData()
		{
			var axleGearRequired = AxleGearRequired();
			foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
				yield return GetIEPCVectoRunData(axleGearRequired, cycle, DataAdapter);
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

			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				var engineMode = engineModes[modeIdx];
				foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
					var engineData = DataAdapter.CreateEngineData(vehicle, engineMode);
					engineData.FuelMode = modeIdx;
					
					var runData = GetIEPCVectoRunData(axleGearRequired, cycle, DataAdapter);
					var averageVoltage = CalculateAverageVoltage(runData.BatteryData);
					
					var electricMachinesData = DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines, vehicle.ElectricMotorTorqueLimits, averageVoltage);

					var hybridParameters = DataAdapter.CreateHybridStrategyParameters(InputDataProvider.JobInputData,
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


		private VectoRunData GetIEPCVectoRunData(bool axleGearRequired, ICycleData cycle, IEngineeringDataAdapter dao)
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

			var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
									CrossWindCorrectionMode.VAirBetaLookupTable;
			var ptoTransmissionData = dao.CreateBatteryElectricPTOTransmissionData(vehicle.Components.PTOTransmissionInputData);
			var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);
			var vehicleData = dao.CreateVehicleData(vehicle);

            var gearshiftParams = dao.CreateGearshiftData(GearboxType.APTN,
				InputDataProvider.DriverInputData.GearshiftInputData,
				axlegearData?.AxleGear.Ratio ?? 1.0, null);
			var wheelEndData = dao.CreateWheelEndData(vehicleData.VehicleClass, vehicle);

            var retVal = new VectoRunData {
				JobName = InputDataProvider.JobInputData.JobName,
				JobType = VectoSimulationJobType.IEPC_E,
				//GearboxData = gearboxData,
				AxleGearData = axlegearData,
				AngledriveData = null,
				VehicleData = vehicleData,
				WheelEndData = wheelEndData,
				AirdragData = dao.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
				DriverData = driver,
				Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
				BusAuxiliaries = dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData,
					VectoSimulationJobType.BatteryElectricVehicle),
				Retarder = retarderData,
				PTO = ptoTransmissionData,
				Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
				ExecutionMode = ExecutionMode.Engineering,
				ElectricMachinesData = electricMachinesData,
				BatteryData = batteryData,
				SuperCapData = supercapData,
				SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel,
				GearshiftParameters = gearshiftParams,
				ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries
					.ElectricPowerDemand,
				InMotionCharging = vehicle.InMotionCharging.Enabled,
				InMotionChargingTechnology = IMCTechnology.NotApplicable,

            };
			
			var gearboxData = dao.CreateIEPCGearboxData(InputDataProvider, retVal);

			retVal.GearboxData = gearboxData;
			return retVal;
		}


		protected virtual IEnumerable<VectoRunData> GetConventionalVehicleRunData()
		{
			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			//foreach (var engineMode in engineModes) {
			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				var engineMode = engineModes[modeIdx];
				foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
					var driver = DataAdapter.CreateDriverData(InputDataProvider.DriverInputData);
					if (InputDataProvider.JobInputData.JobType != VectoSimulationJobType.ConventionalVehicle) {
						driver.EngineStopStart.UtilityFactorDriving = 1;
					}

					var vehicle = InputDataProvider.JobInputData.Vehicle;
					var engineData = DataAdapter.CreateEngineData(vehicle, engineMode);
					engineData.FuelMode = modeIdx;
					
					var axlegearData = DataAdapter.CreateAxleGearData(vehicle.Components.AxleGearInputData);
					
					var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
											CrossWindCorrectionMode.VAirBetaLookupTable;
					var angledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
					var ptoTransmissionData =
						DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);


					var ptoCycleWhileDrive =
						InputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData
							.PTOCycleWhileDriving != null
							? DrivingCycleDataReader.ReadFromDataTable(
								InputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData
									.PTOCycleWhileDriving,
								"PTO During Drive", false)
							: null;


					var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);

					var battery = DataAdapter.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
					var superCap = DataAdapter.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
					var averageVoltage = battery != null ? CalculateAverageVoltage(battery): null;

					
					var vehicleData = DataAdapter.CreateVehicleData(vehicle);
					var wheelEndData = DataAdapter.CreateWheelEndData(vehicleData.VehicleClass, vehicle);

					//var gearshiftParams = dao.CreateGearshiftData(
					//	gearboxData.Type, InputDataProvider.DriverInputData.GearshiftInputData,
					//	axlegearData.AxleGear.Ratio * (angledriveData?.Angledrive.Ratio ?? 1.0),
					//	engineData.IdleSpeed);
					//if (gearshiftParams == null) {
					//	gearshiftParams = new ShiftStrategyParameters() {
					//		StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					//		StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration,
					//		TimeBetweenGearshifts = DeclarationData.Gearbox.MinTimeBetweenGearshifts,
					//		DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay,
					//		UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay,
					//		UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration,
					//	};
					//}
					
					
					var retVal =  new VectoRunData {
						JobName = InputDataProvider.JobInputData.JobName,
						//JobType = jobType,
						EngineData = engineData,
						//GearboxData = gearboxData,
						AxleGearData = axlegearData,
						AngledriveData = angledriveData,
						VehicleData = vehicleData,
						AirdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
						WheelEndData = wheelEndData,
						DriverData = driver,
						Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
						//BusAuxiliaries =
						//	dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData, jobType),
						//Retarder = dao.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition),
						PTO = ptoTransmissionData,
						Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
						ExecutionMode = ExecutionMode.Engineering,
						PTOCycleWhileDrive = ptoCycleWhileDrive,

						//ElectricMachinesData = electricMachines,
						//HybridStrategyParameters = hybridParameters,
						InMotionCharging = false, // vehicle.InMotionCharging.Enabled,
						InMotionChargingTechnology = IMCTechnology.NotApplicable,
						BatteryData = battery,
						SuperCapData = superCap,
						SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle |
										SimulationType.PWheel,
						//GearshiftParameters = gearshiftParams,
						ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData
							.Auxiliaries.ElectricPowerDemand,
					};
					
					var gearshiftParams =
						DataAdapter.CreateGearshiftData(
							vehicle.Components.GearboxInputData.Type, InputDataProvider.DriverInputData.GearshiftInputData,
							(retVal.AxleGearData?.AxleGear.Ratio ?? 1.0) * (retVal.AngledriveData?.Angledrive.Ratio ?? 1.0),
							engineData.IdleSpeed
						);
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
					retVal.GearshiftParameters = gearshiftParams;
					retVal.GearboxData = DataAdapter.CreateGearboxData(InputDataProvider, retVal);
					
					var electricMachines =
						DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines,
							vehicle.ElectricMotorTorqueLimits, averageVoltage, retVal.GearboxData.GearList) ??
						new List<Tuple<PowertrainPosition, ElectricMotorData>>();
					var powertrainPosition = electricMachines.FirstOrDefault(e => e.Item1 != PowertrainPosition.GEN)?.Item1 ?? PowertrainPosition.HybridPositionNotSet;

					var jobType = vehicle.VehicleType;
						//electricMachines.Count > 0 && (battery != null || superCap != null)
						//? VectoSimulationJobType.ParallelHybridVehicle
						//: VectoSimulationJobType.ConventionalVehicle;

					retVal.JobType = jobType;
					retVal.ElectricMachinesData = electricMachines;
					retVal.Retarder = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition);
					retVal.BusAuxiliaries =
						DataAdapter.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData, jobType);
					if (powertrainPosition == PowertrainPosition.HybridP2 && retVal.GearboxData.Type.AutomaticTransmission()) {
						throw new VectoException(
							"Powertrain Architecture 'hybrid electric vehicle, P2' with AT transmission not supported");
					}
					retVal.HybridStrategyParameters = jobType.IsOneOf(VectoSimulationJobType.ParallelHybridVehicle, VectoSimulationJobType.IHPC)
						? DataAdapter.CreateHybridStrategyParameters(InputDataProvider.JobInputData, engineData, retVal.GearboxData)
						: null;
					if (InputDataProvider.JobInputData.Vehicle.PTO_DriveGear != null &&
						InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed != null) {
						driver.PTODriveMinSpeed = InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed /
							axlegearData.AxleGear.Ratio /
							retVal.GearboxData.Gears[InputDataProvider.JobInputData.Vehicle.PTO_DriveGear.Gear].Ratio /
							(angledriveData?.Angledrive.Ratio ?? 1.0) * vehicle.DynamicTyreRadius;
						driver.PTODriveRoadsweepingGear = InputDataProvider.JobInputData.Vehicle.PTO_DriveGear;
						engineData.PTORoadSweepEngineSpeed =
							InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed;
					}

					yield return retVal;
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

		private class DummyVehicleContainer : VehicleContainer
		{
			public DummyVehicleContainer(VectoRunData runData) : base(runData, null,
				null, null)
			{ }
		}
    }
}
