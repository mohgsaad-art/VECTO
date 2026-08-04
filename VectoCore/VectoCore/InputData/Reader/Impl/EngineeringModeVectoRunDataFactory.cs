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
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Configuration;

[assembly: InternalsVisibleTo("VectoCoreTest")]

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
    public class EngineeringModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		public const string Name = "EngineeringModeVectoRunDataFactory";

        private static readonly Dictionary<string, Tuple<DrivingCycleData, DateTime>> CyclesCache = new Dictionary<string, Tuple<DrivingCycleData, DateTime>>();

		protected readonly IEngineeringInputDataProvider InputDataProvider;
		protected IEngineeringDataAdapter DataAdapter { get; }

		/// Used only for debug output
		/// </summary>
		public IOutputDataWriter Writer { get; set; }


        internal EngineeringModeVectoRunDataFactory(IEngineeringInputDataProvider dataProvider, IEngineeringDataAdapter dataAdapter)
		{
			InputDataProvider = dataProvider;
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
					return GetFCHV_RunData(GetMultiplePowertrainsRunData, InputDataProvider.JobInputData.JobType);
				default:
					throw new ArgumentOutOfRangeException($"Invalid JobType {InputDataProvider.JobInputData.JobType}");
			}
		}

		private IEnumerable<VectoRunData> GetFCHV_RunData(Func<IEnumerable<VectoRunData>> baseDataFunc, VectoSimulationJobType jobType)
		{
			var dao = new EngineeringDataAdapter() {
				DebugOutputDataWriter = Writer,
				JobFilePath = InputDataProvider.DataSource.SourceFile
			};

			foreach (var pevRd in baseDataFunc()) {
				if (InputDataProvider.JobInputData.Vehicle.Components.FuelCellSystemInputData != null)
				{
					var fuelCellData =
						dao.CreateFuelCellSystemData(InputDataProvider.JobInputData.Vehicle.Components
							.FuelCellSystemInputData);
					var iterativeRunStrategy = new FCHEVIterativeRunStrategy(new[]{
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


					iterativeRunStrategy.Update = (modData, runData) =>
					{
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
				}

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
						AxleGearSinglePwt = axlegearData,
						AngledriveSinglePwt = angledriveData,
						VehicleData = vehicleData,
						AirdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
						WheelEndData = wheelEndData,
						DriverData = driver,
						Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
						BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData,
							vehicleData, jobType),
						RetarderSinglePwt = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition),
						PTOSinglePwt = ptoTransmissionData,
						Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
						ExecutionMode = ExecutionMode.Engineering,
						PTOCycleWhileDrive = ptoCycleWhileDrive,
						ElectricMachinesSinglePwt = electricMachines,
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
						retVal.GearshiftParametersSinglePwt = gearshiftParams;

                        gearboxData = DataAdapter.CreateGearboxData(
							InputDataProvider, retVal);
						angledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
						retVal.GearboxSinglePwt = gearboxData;
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
						retVal.GearshiftParametersSinglePwt = gearshiftParams;
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

		private VectoRunData GetMultiplePowertrainsCycleRunData(
			ICycleData cycle, 
			IEngineEngineeringInputData engine = null,
			int engineModeIndex = -1)
		{
			var dao = new EngineeringDataAdapter();
			var vehicle = InputDataProvider.JobInputData.Vehicle;

			var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);
			driver.EngineStopStart.UtilityFactorDriving = 1;
			driver.EngineStopStart.UtilityFactorStandstill = 1;

			CombustionEngineData engineData = null;
			if (engine != null)
			{
				engineData = dao.CreateEngineData(vehicle, engine.EngineModes[engineModeIndex]);
				engineData.FuelMode = engineModeIndex;
                engineData.PTORoadSweepEngineSpeed = vehicle.PTO_DriveEngineSpeed;
            }

			var batteryData = dao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
			var averageVoltage = (batteryData != null) ? CalculateAverageVoltage(batteryData) : null;

			var generatorData = (vehicle.Components.GeneratorEngineering != null)
				? dao.CreateElectricMachine(vehicle.Components.GeneratorEngineering, vehicle.ElectricMotorTorqueLimits, averageVoltage)
				: null;

			var emData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();

			if (generatorData != null)
			{
				emData.Add(generatorData);
			}

			var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode == CrossWindCorrectionMode.VAirBetaLookupTable;

			var drivingCycle = GetDrivingCycle(cycle, crossWindRequired);

			var vehicleData = dao.CreateVehicleData(vehicle);

			var cycleProxy = new DrivingCycleProxy(drivingCycle, cycle.Name);

            var axlePtData = dao.CreateAxlePowertrainsData(InputDataProvider, averageVoltage, cycleProxy);

			foreach (var axlePt in axlePtData)
			{
                SetDriverPTOProperties(vehicle, driver, axlePt.AxleGearData, axlePt.GearboxData, axlePt.AngledriveData);
            }

            return new VectoRunData
			{
				JobName = InputDataProvider.JobInputData.JobName,
				JobType = InputDataProvider.JobInputData.JobType,
				VehicleData = vehicleData,
				EngineData = engineData,
				ElectricMachinesSinglePwt = emData,
				WheelEndData = dao.CreateWheelEndData(vehicleData.VehicleClass, vehicle),
				AirdragData = dao.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
				DriverData = driver,
				Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
				BusAuxiliaries = dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData, InputDataProvider.JobInputData.JobType),
				Cycle = cycleProxy,
				ExecutionMode = ExecutionMode.Engineering,
				BatteryData = batteryData,
				SuperCapData = dao.CreateSuperCapData(vehicle.Components.ElectricStorage, vehicle.InitialSOC),
				SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel,
				ElectricAuxDemand = vehicle.Components.AuxiliaryInputData.Auxiliaries.ElectricPowerDemand,
				InMotionCharging = vehicle.InMotionCharging.Enabled,
				InMotionChargingTechnology = vehicle.InMotionCharging.Technology,
				HybridStrategyParameters = null,
				AxlePowertrains = axlePtData
			};
		}

        private void SetDriverPTOProperties(
			IVehicleEngineeringInputData vehicle, 
			DriverData driver, 
			AxleGearData axlegearData,
			GearboxData gearboxData,
			AngledriveData angledriveData
            )
        {
			if (gearboxData == null)
			{
				return;
			}

			if (vehicle.PTO_DriveGear == null || vehicle.PTO_DriveEngineSpeed == null)
			{
				return;
			}
            
			driver.PTODriveMinSpeed = vehicle.PTO_DriveEngineSpeed /
                (axlegearData?.AxleGear.Ratio ?? 1.0) /
                gearboxData.Gears[vehicle.PTO_DriveGear.Gear].Ratio /
                (angledriveData?.Angledrive.Ratio ?? 1.0) * vehicle.DynamicTyreRadius;

            driver.PTODriveRoadsweepingGear = vehicle.PTO_DriveGear;
        }

        private IEnumerable<VectoRunData> GetMultiplePowertrainsRunData()
		{
			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;

			if (engine == null)
			{
				foreach (var cycle in InputDataProvider.JobInputData.Cycles)
				{
					yield return GetMultiplePowertrainsCycleRunData(cycle);
				}
			}			
			else
			{
				for (var modeIdx = 0; modeIdx < engine.EngineModes.Count; modeIdx++)
				{
					foreach (var cycle in InputDataProvider.JobInputData.Cycles)
					{
						yield return GetMultiplePowertrainsCycleRunData(cycle, engine, modeIdx);
					}
				}
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
					GearboxSinglePwt = gearboxData,
					AxleGearSinglePwt = axlegearData,
					VehicleData = vehicleData,
					AirdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
					WheelEndData = wheelEndData,
					DriverData = driver,
					Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
					BusAuxiliaries = DataAdapter.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData, VectoSimulationJobType.BatteryElectricVehicle),
					RetarderSinglePwt = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition),
					PTOSinglePwt = ptoTransmissionData,
					Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
					ExecutionMode = ExecutionMode.Engineering,
					ElectricMachinesSinglePwt = electricMachinesData,
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
                    retVal.GearshiftParametersSinglePwt = gearshiftParams;
                    gearboxData = DataAdapter.CreateGearboxData(InputDataProvider, retVal);
					angledriveData = DataAdapter.CreateAngledriveData(vehicle.Components.AngledriveInputData);
				}

				retVal.AngledriveSinglePwt = angledriveData;

				if (gearshiftParams == null) {
					gearshiftParams = new ShiftStrategyParameters() {
						StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
						StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
					};
					retVal.GearshiftParametersSinglePwt = gearshiftParams;
                }

				retVal.GearboxSinglePwt = gearboxData;
				
				yield return retVal;
			}
		}

		

		private IEnumerable<VectoRunData> GetIEPCRunData()
		{
			var axleGearRequired = EngineeringDataAdapter.CheckIEPCData(
				InputDataProvider.JobInputData.Vehicle.Components.IEPCEngineeringInputData,
				InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);
			
			var dao = new EngineeringDataAdapter();
			foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
				yield return GetIEPCVectoRunData(axleGearRequired, cycle, DataAdapter);
			}
		}

		

		private IEnumerable<VectoRunData> GetIEPC_S_RunData()
		{
			var axleGearRequired = EngineeringDataAdapter.CheckIEPCData(
				InputDataProvider.JobInputData.Vehicle.Components.IEPCEngineeringInputData,
				InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);

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
						engineData, runData.GearboxSinglePwt);

					foreach (var gear in runData.GearboxSinglePwt.Gears.Keys) {
						engineData.FullLoadCurves[gear] = engineData.FullLoadCurves[0];
					}
					runData.JobType = VectoSimulationJobType.IEPC_S;
					runData.EngineData = engineData;
					runData.ElectricMachinesSinglePwt.Add(electricMachinesData.First(x => x.Item1 == PowertrainPosition.GEN));
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
				AxleGearSinglePwt = axlegearData,
				AngledriveSinglePwt = null,
				VehicleData = vehicleData,
				WheelEndData = wheelEndData,
				AirdragData = dao.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
				DriverData = driver,
				Aux = dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
				BusAuxiliaries = dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData,
					VectoSimulationJobType.BatteryElectricVehicle),
				RetarderSinglePwt = retarderData,
				PTOSinglePwt = ptoTransmissionData,
				Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
				ExecutionMode = ExecutionMode.Engineering,
				ElectricMachinesSinglePwt = electricMachinesData,
				BatteryData = batteryData,
				SuperCapData = supercapData,
				SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle | SimulationType.PWheel,
				GearshiftParametersSinglePwt = gearshiftParams,
				ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData.Auxiliaries
					.ElectricPowerDemand,
				InMotionCharging = vehicle.InMotionCharging.Enabled,
				InMotionChargingTechnology = IMCTechnology.NotApplicable,

            };
			
			var gearboxData = dao.CreateIEPCGearboxData(InputDataProvider.JobInputData.Vehicle.Components.IEPCEngineeringInputData, retVal);

			retVal.GearboxSinglePwt = gearboxData;
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
						AxleGearSinglePwt = axlegearData,
						AngledriveSinglePwt = angledriveData,
						VehicleData = vehicleData,
						AirdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, vehicle),
						WheelEndData = wheelEndData,
						DriverData = driver,
						Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData),
						//BusAuxiliaries =
						//	dao.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData, jobType),
						//Retarder = dao.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition),
						PTOSinglePwt = ptoTransmissionData,
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
							(retVal.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (retVal.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
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
					retVal.GearshiftParametersSinglePwt = gearshiftParams;
					retVal.GearboxSinglePwt = DataAdapter.CreateGearboxData(InputDataProvider, retVal);
					
					var electricMachines =
						DataAdapter.CreateElectricMachines(vehicle.Components.ElectricMachines,
							vehicle.ElectricMotorTorqueLimits, averageVoltage, retVal.GearboxSinglePwt.GearList) ??
						new List<Tuple<PowertrainPosition, ElectricMotorData>>();
					var powertrainPosition = electricMachines.FirstOrDefault(e => e.Item1 != PowertrainPosition.GEN)?.Item1 ?? PowertrainPosition.HybridPositionNotSet;

					var jobType = vehicle.VehicleType;
						//electricMachines.Count > 0 && (battery != null || superCap != null)
						//? VectoSimulationJobType.ParallelHybridVehicle
						//: VectoSimulationJobType.ConventionalVehicle;

					retVal.JobType = jobType;
					retVal.ElectricMachinesSinglePwt = electricMachines;
					retVal.RetarderSinglePwt = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData, powertrainPosition);
					retVal.BusAuxiliaries =
						DataAdapter.CreateBusAuxiliariesData(vehicle.Components.AuxiliaryInputData, vehicleData, jobType);
					if (powertrainPosition == PowertrainPosition.HybridP2 && retVal.GearboxSinglePwt.Type.AutomaticTransmission()) {
						throw new VectoException(
							"Powertrain Architecture 'hybrid electric vehicle, P2' with AT transmission not supported");
					}
					retVal.HybridStrategyParameters = jobType.IsOneOf(VectoSimulationJobType.ParallelHybridVehicle, VectoSimulationJobType.IHPC)
						? DataAdapter.CreateHybridStrategyParameters(InputDataProvider.JobInputData, engineData, retVal.GearboxSinglePwt)
						: null;
					if (InputDataProvider.JobInputData.Vehicle.PTO_DriveGear != null &&
						InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed != null) {
						driver.PTODriveMinSpeed = InputDataProvider.JobInputData.Vehicle.PTO_DriveEngineSpeed /
							axlegearData.AxleGear.Ratio /
							retVal.GearboxSinglePwt.Gears[InputDataProvider.JobInputData.Vehicle.PTO_DriveGear.Gear].Ratio /
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
