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
*   Vasileios Kouliaridis, vasilis.k@emisia.com, EMISIA
*/

using System;
using System.Collections.Generic;
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
using System.Linq;


namespace TUGraz.VectoCore.InputData.Reader.Impl
{

	public class DeclarationModeTrailerVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected static readonly object CyclesCacheLock = new object();

		protected static readonly Dictionary<MissionType, DrivingCycleData> CyclesCache =
			new Dictionary<MissionType, DrivingCycleData>();

		protected readonly IDeclarationTrailerInputDataProvider InputDataProvider;

		protected IDeclarationReport Report;

		protected Segment _segment;
		protected DriverData _driverdata;
		protected AirdragData _airdragData;
		protected AxleGearData _axlegearData;
		protected AngledriveData _angledriveData;
		protected GearboxData _gearboxData;
		protected RetarderData _retarderData;
		protected PTOData _ptoTransmissionData;
		protected PTOData _municipalPtoTransmissionData;
		//protected Exception InitException;
		protected ShiftStrategyParameters _gearshiftData;

		DeclarationTrailerDataAdapter _dao = new DeclarationTrailerDataAdapter();

		internal DeclarationModeTrailerVectoRunDataFactory(
			IDeclarationTrailerInputDataProvider dataProvider, IDeclarationReport report)
		{
			InputDataProvider = dataProvider;
			Report = report;
		}

		#region Overrides of AbstractDeclarationVectoRunDataFactory

		protected IDeclarationTrailerDataAdapter DataAdapter { get { return null; } }
		#endregion

		protected Segment GetSegment(ITrailerDeclarationInputData vehicle)
		{
			/*if (!vehicle.VehicleCategory.IsLorry()) {
				throw new VectoException("Invalid vehicle category for truck factory! {0}", vehicle.VehicleCategory.GetCategoryName());
			}
			var segment = DeclarationData.TruckSegments.Lookup(
				vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating, vehicle.CurbMassChassis,
				vehicle.VocationalVehicle);

			if (!segment.Found) {
				throw new VectoException(
					"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, GVMR: {2}",
					vehicle.VehicleCategory, vehicle.AxleConfiguration,
					vehicle.GrossVehicleMassRating);
			}*/

			return new Segment();
		}

		protected IEnumerable<VectoRunData> GetNextRun()
		{
			//if (InitException != null) {
			//	throw InitException;
			//}

			//if (TrailerInputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
			//	yield return CreateVectoRunData(InputDataProvider.JobInputData.Vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
			//} else {
			foreach (var vectoRunData in VectoRunDataTruckNonExempted()) {
				yield return vectoRunData;
			}
			//}
		}

		private IEnumerable<VectoRunData> VectoRunDataTruckNonExempted()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;

			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			for(var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				
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

		protected VectoRunData CreateVectoRunData(
			ITrailerDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			/*if (TrailerInputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
				return new VectoRunData {
					Exempted = true,
					Report = Report,
					Mission = new Mission() { MissionType = MissionType.ExemptedMission },
					VehicleData = DataAdapter.CreateVehicleData(TrailerInputDataProvider.JobInputData.Vehicle, new Segment(), null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad, Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null))),
					InputDataHash = TrailerInputDataProvider.XMLHash
				};
			}*/

			var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];
			DrivingCycleData cycle;
			lock (CyclesCacheLock) {
				if (CyclesCache.ContainsKey(mission.MissionType)) {
					cycle = CyclesCache[mission.MissionType];
				} else {
					cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
					CyclesCache.Add(mission.MissionType, cycle);
				}
			}
			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading),
				VehicleDesignSpeed = _segment.DesignSpeed,
				AirdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData, mission, _segment),
				EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission), // _engineData.Copy(), // a copy is necessary because every run has a different correction factor!
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, vehicle.ExternalBodyLength), //TODO: Bill
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.JobName,
				ModFileSuffix = (engineModes.Count > 1 ? string.Format("_EngineMode{0}_", modeIdx) : "") + loading.Key.ToString(),
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


		protected virtual PTOData CreateDefaultPTOData()
		{
			return new PTOData() {
				TransmissionType = DeclarationData.PTO.DefaultPTOTechnology,
				LossMap = PTOIdleLossMapReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOIdleLosses)),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOActivationCycle),
						CycleType.PTO, "PTO", false)
			};
		}

		protected virtual void InitializeReport()
		{
			VectoRunData powertrainConfig;
			List<List<FuelData.Entry>> fuels;
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			if (vehicle.ExemptedVehicle) {
				powertrainConfig = CreateVectoRunData(vehicle, 0, null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
				fuels = new List<List<FuelData.Entry>>();
			} else {
				powertrainConfig = _segment.Missions.Select(
						mission => CreateVectoRunData(
							vehicle, 0, mission, mission.Loadings.First()))
					.FirstOrDefault(x => x != null);
				fuels = vehicle.Components.EngineInputData.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, null)).ToList())
					.ToList();
			}
			Report.InitializeReport(powertrainConfig, fuels);
		}

		protected virtual void Initialize()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			if (vehicle.ExemptedVehicle) {
				return;
			}

			_segment = GetSegment(vehicle);
			_driverdata = DataAdapter.CreateDriverData();
			_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);
			var tempVehicle = DataAdapter.CreateVehicleData(vehicle, _segment, _segment.Missions.First(),
													_segment.Missions.First().Loadings.First());
			_airdragData = DataAdapter.CreateAirdragData(vehicle.Components.AirdragInputData,
												_segment.Missions.First(), _segment);
			if (InputDataProvider.JobInputData.Vehicle.AxleConfiguration.AxlegearIncludedInGearbox()) {
				_axlegearData = DataAdapter.CreateDummyAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData);
			} else { 
				_axlegearData = DataAdapter.CreateAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);
			} 
			_angledriveData = DataAdapter.CreateAngledriveData(InputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData);
			var tmpRunData = new VectoRunData() {
				ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy,
				GearboxData =  new GearboxData() {
					Type = vehicle.Components.GearboxInputData.Type,
				}
			};
			var tmpStrategy = PowertrainBuilder.GetShiftStrategy(tmpRunData, new SimplePowertrainContainer(tmpRunData));
			var tmpEngine = DataAdapter.CreateEngineData(
				vehicle, vehicle.Components.EngineInputData.EngineModes[0], _segment.Missions.First());
			_gearboxData = DataAdapter.CreateGearboxData(
				vehicle, new VectoRunData() { EngineData = tmpEngine, AxleGearData = _axlegearData, VehicleData = tempVehicle },
				tmpStrategy);
				
			_retarderData = DataAdapter.CreateRetarderData(vehicle.Components.RetarderInputData);

			_ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(vehicle.Components.PTOTransmissionInputData);

			_municipalPtoTransmissionData = CreateDefaultPTOData();
			_gearshiftData = DataAdapter.CreateGearshiftData(
				_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), tmpEngine.IdleSpeed);

		}

		public IEnumerable<VectoRunData> NextRun()
		{
		
			Initialize();
			if (Report != null) {
				InitializeReport();
			}

			return GetNextRun();
		}
	}
}
