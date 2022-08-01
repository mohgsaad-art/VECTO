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
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

[assembly: InternalsVisibleTo("VectoCoreTest")]

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class EngineeringModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		private static readonly Dictionary<string, DrivingCycleData> CyclesCache = new Dictionary<string, DrivingCycleData>();

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
			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			//foreach (var engineMode in engineModes) {
			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				var engineMode = engineModes[modeIdx];
				foreach (var cycle in InputDataProvider.JobInputData.Cycles) {
					var dao = new EngineeringDataAdapter();
					var driver = dao.CreateDriverData(InputDataProvider.DriverInputData);
					if (InputDataProvider.JobInputData.JobType != VectoSimulationJobType.ConventionalVehicle)
						driver.EngineStopStart.UtilityFactorDriving = 1;
					var vehicle = InputDataProvider.JobInputData.Vehicle;
					var engineData = dao.CreateEngineData(vehicle, engineMode);
					engineData.FuelMode = modeIdx;
					var tempVehicle = dao.CreateVehicleData(vehicle);

					var axlegearData = dao.CreateAxleGearData(vehicle.Components.AxleGearInputData);
					var tmpRunData = new VectoRunData() {
						ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
						GearboxData = new GearboxData() {
							Type = vehicle.Components.GearboxInputData.Type,
						}
					};
					var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
					var gearboxData = dao.CreateGearboxData(
						InputDataProvider, new VectoRunData() {
							EngineData = engineData,
							VehicleData = tempVehicle,
							AxleGearData = axlegearData,
						}, tmpStrategy);

					var crossWindRequired = vehicle.Components.AirdragInputData.CrossWindCorrectionMode ==
											CrossWindCorrectionMode.VAirBetaLookupTable;
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


					var drivingCycle = CyclesCache.GetOrAdd(cycle.CycleData.Source, _ => DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, crossWindRequired));

					
					var jobType = VectoSimulationJobType.ConventionalVehicle;

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
						Retarder = dao.CreateRetarderData(vehicle.Components.RetarderInputData),
						PTO = ptoTransmissionData,
						Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name),
						ExecutionMode = ExecutionMode.Engineering,
						PTOCycleWhileDrive = ptoCycleWhileDrive,
						SimulationType = SimulationType.DistanceCycle | SimulationType.MeasuredSpeedCycle |
										SimulationType.PWheel,
						GearshiftParameters = gearshiftParams,
						ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
						ElectricAuxDemand = InputDataProvider.JobInputData.Vehicle.Components.AuxiliaryInputData
							.Auxiliaries.ElectricPowerDemand,
					};
				}
			}
		}

		public IInputDataProvider DataProvider
		{
			get { return InputDataProvider; }
		}
	}
}
