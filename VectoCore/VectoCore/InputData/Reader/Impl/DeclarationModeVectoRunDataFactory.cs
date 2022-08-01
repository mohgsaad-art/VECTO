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
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModeVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		private static readonly object CyclesCacheLock = new object();

		private static readonly Dictionary<MissionType, DrivingCycleData> CyclesCache =
			new Dictionary<MissionType, DrivingCycleData>();

		protected readonly IDeclarationInputDataProvider InputDataProvider;

		protected IDeclarationReport Report;
		private DeclarationDataAdapter _dao;
		private Segment _segment;
		private bool allowVocational;
		private DriverData _driverdata;
		private AirdragData _airdragData;
		private AxleGearData _axlegearData;
		private AngledriveData _angledriveData;
		private GearboxData _gearboxData;
		private RetarderData _retarderData;
		private PTOData _ptoTransmissionData;
		private PTOData _municipalPtoTransmissionData;
		private Exception InitException;
		protected ShiftStrategyParameters _gearshiftData;

		internal DeclarationModeVectoRunDataFactory(IDeclarationInputDataProvider dataProvider, IDeclarationReport report)
		{
			InputDataProvider = dataProvider;
			Report = report;
			allowVocational = true;
			try {
				Initialize();
				if (Report != null) {
					InitializeReport();
				}
			} catch (Exception e) {
				InitException = e;
			}
		}

		private void Initialize()
		{
			_dao = new DeclarationDataAdapter();
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			if (vehicle.ExemptedVehicle) {
				if (vehicle.AxleConfiguration != AxleConfiguration.AxleConfig_Undefined) {
					_segment = GetVehicleClassification(vehicle.VehicleCategory,
						vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating, vehicle.CurbMassChassis, false);
				}
				return;
			}

			try {
				_segment = GetVehicleClassification(
					vehicle.VehicleCategory,
					vehicle.AxleConfiguration,
					vehicle.GrossVehicleMassRating,
					vehicle.CurbMassChassis,
					vehicle.VocationalVehicle);
			} catch (VectoException) {
				allowVocational = false;
				_segment = GetVehicleClassification(
					vehicle.VehicleCategory,
					vehicle.AxleConfiguration,
					vehicle.GrossVehicleMassRating,
					vehicle.CurbMassChassis,
					false);
			}
			if (!_segment.Found) {
				throw new VectoException(
					"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, GVMR: {2}",
					vehicle.VehicleCategory, vehicle.AxleConfiguration,
					vehicle.GrossVehicleMassRating);
			}
			_driverdata = _dao.CreateDriverData();
			_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);
			var tempVehicle = _dao.CreateVehicleData(vehicle, _segment.Missions.First(),
				_segment.Missions.First().Loadings.First().Value, allowVocational);
			_airdragData = _dao.CreateAirdragData(vehicle.Components.AirdragInputData,
				_segment.Missions.First(), _segment);
			var tmpEngine = _dao.CreateEngineData(
				vehicle, vehicle.Components.EngineInputData.EngineModes[0], _segment.Missions.First());
			_axlegearData = _dao.CreateAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);
			_angledriveData = _dao.CreateAngledriveData(InputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData);
			
			var tmpRunData = new VectoRunData() {
				ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
				GearboxData = new GearboxData() {
					Type = vehicle.Components.GearboxInputData.Type,
				}
			};
			var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));
			_gearboxData = _dao.CreateGearboxData(
				vehicle, new VectoRunData() { EngineData = tmpEngine, AxleGearData = _axlegearData, VehicleData = tempVehicle },
				tmpStrategy);
			_retarderData = _dao.CreateRetarderData(vehicle.Components.RetarderInputData);

			_ptoTransmissionData = _dao.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

			_municipalPtoTransmissionData = CreateDefaultPTOData();
			_gearshiftData = _dao.CreateGearshiftData(
				_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), tmpEngine.IdleSpeed);

		}

		private void InitializeReport()
		{
			VectoRunData powertrainConfig;
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
				powertrainConfig = new VectoRunData() {
					Exempted = true,
					VehicleData = _dao.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, null, null, allowVocational),
					InputDataHash = InputDataProvider.XMLHash
				};
				powertrainConfig.VehicleData.VehicleClass = _segment.Found ? _segment.VehicleClass : VehicleClass.Class0;
			} else {
				powertrainConfig = _segment.Missions.Select(
						mission => CreateVectoRunData(
							vehicle, 0, mission, mission.Loadings.First()))
					.FirstOrDefault(x => x != null);
			}
			Report.InitializeReport(powertrainConfig);
		}

		public IEnumerable<VectoRunData> NextRun()
		{
			if (InitException != null) {
				throw InitException;
			}

			if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
				yield return new VectoRunData {
						Exempted = true,
						Report = Report,
						Mission = new Mission() { MissionType = MissionType.ExemptedMission},
						VehicleData = _dao.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, null, null, allowVocational),
						InputDataHash = InputDataProvider.XMLHash
					};
			} else {
				foreach (var vectoRunData in VectoRunDataNonExempted())
					yield return vectoRunData;
			}
		}

		public IInputDataProvider DataProvider
		{
			get { return InputDataProvider; }
		}

		private IEnumerable<VectoRunData> VectoRunDataNonExempted()
		{
			
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var adasCombination = DeclarationData.ADASCombinations.Lookup(vehicle.ADAS);
			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				foreach (var mission in _segment.Missions) {
					if (mission.MissionType.IsEMS() &&
						engine.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS)) {
						continue;
					}

					foreach (var loading in mission.Loadings) {
						var simulationRunData = CreateVectoRunData(vehicle, modeIdx, mission, loading);
						yield return simulationRunData;
					}
				}
			}
		}

		private VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Kilogram> loading)
		{
			if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
				return new VectoRunData {
					Exempted = true,
					Report = Report,
					Mission = new Mission() { MissionType = MissionType.ExemptedMission },
					VehicleData = _dao.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, null, 0.SI<Kilogram>(), allowVocational),
					InputDataHash = InputDataProvider.XMLHash
				};
			}

			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];

			DrivingCycleData cycle;
			lock (CyclesCacheLock) {
				if (CyclesCache.ContainsKey(mission.MissionType)) {
					cycle = CyclesCache[mission.MissionType];
				} else {
					cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased,
						"", false);
					CyclesCache.Add(mission.MissionType, cycle);
				}
			}

			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleDesignSpeed = _segment.DesignSpeed,
				VehicleData = _dao.CreateVehicleData(vehicle, mission, loading.Value, allowVocational),
				AirdragData =
					_dao.CreateAirdragData(vehicle.Components.AirdragInputData, mission, _segment),
				EngineData = _dao.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = _dao.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData, mission.MissionType,
					_segment.VehicleClass, vehicle.Components.AxleWheels.NumSteeredAxles),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.JobName,
				ModFileSuffix = loading.Key.ToString(),
				Report = Report,
				Mission = mission,
				PTO = mission.MissionType == MissionType.MunicipalUtility
					? _municipalPtoTransmissionData
					: _ptoTransmissionData,
				InputDataHash = InputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
				ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
			return simulationRunData;
		}

		private PTOData CreateDefaultPTOData()
		{
			return new PTOData() {
				TransmissionType = DeclarationData.PTO.DefaultPTOTechnology,
				LossMap = PTOIdleLossMapReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOIdleLosses)),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOActivationCycle),
						CycleType.PTO, "PTO", false)
			};
		}

		internal Segment GetVehicleClassification(VehicleCategory category, AxleConfiguration axles, Kilogram grossMassRating,
			Kilogram curbWeight, bool vocational)
		{
			return DeclarationData.Segments.Lookup(category, axles, grossMassRating, curbWeight, vocational);
		}
	}
}
