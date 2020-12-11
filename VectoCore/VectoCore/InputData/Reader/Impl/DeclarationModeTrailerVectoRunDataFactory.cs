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
using System.Net;
using System.ServiceModel.Channels;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Bson;
using Ninject;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using IXMLGearData = TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces.IXMLGearData;
using Wheels = TUGraz.VectoCore.Models.Declaration.Wheels;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{

	public class DeclarationModeTrailerVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected static readonly object CyclesCacheLock = new object();
		protected static readonly Dictionary<MissionType, DrivingCycleData> CyclesCache = new Dictionary<MissionType, DrivingCycleData>();
		protected readonly IDeclarationTrailerInputDataProvider TrailerInputDataProvider;
		protected readonly IDeclarationInputDataProvider InputDataProvider;
		protected IDeclarationReport Report; 
		protected IDeclarationDataAdapter DataAdapter => _da;
		protected DeclarationTrailerDataAdapter TrailerDataAdapter = new DeclarationTrailerDataAdapter();

		private readonly DeclarationDataAdapterHeavyLorry _da = new DeclarationDataAdapterHeavyLorry();

		private Segment _segment;
		private DriverData _driverdata;
		private AirdragData _airdragData;
		private AxleGearData _axlegearData;
		private AngledriveData _angledriveData;
		private GearboxData _gearboxData;
		private RetarderData _retarderData;
		private PTOData _ptoTransmissionData;
		private ShiftStrategyParameters _gearshiftData;

		private IDeclarationInputDataProvider _genericInputDataProvider;
		private ITrailerDeclarationInputData _trailer;
		private TrailerSegment _trailerSegment;
		private TrailerData _specificTrailerData;

		[Inject]
		public IDeclarationInjectFactory DeclarationFactory { protected get; set; }

		internal DeclarationModeTrailerVectoRunDataFactory(
			IDeclarationTrailerInputDataProvider dataProvider, IDeclarationReport report)
		{
			TrailerInputDataProvider = dataProvider;
			Report = report;
		}

		protected Segment GetSegment(IVehicleDeclarationInputData vehicle)
		{
			var segment = DeclarationData.TruckSegments.Lookup(
				vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.GrossVehicleMassRating, vehicle.CurbMassChassis,
				vehicle.VocationalVehicle);

			return segment;
		}

		protected IEnumerable<VectoRunData> GetNextRun()
		{
			return VectoRunDataNonExempted();
		}

		private IEnumerable<VectoRunData> VectoRunDataNonExempted()
		{
			var vehicle = _genericInputDataProvider.JobInputData.Vehicle;
			var engine = vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				foreach (var mission in _segment.Missions.Where(genericMission => _trailerSegment.Missions.Count(trailerMission => trailerMission.MissionType == genericMission.MissionType) > 0)) {
					foreach (var loading in mission.Loadings) {
						yield return CreateTrailerVectoRunData(vehicle, modeIdx, mission, loading);
						yield return CreateVectoRunData(vehicle, modeIdx, mission, loading);
					}
				}
			}
		}

		protected virtual void InitializeReport()
		{
			VectoRunData powertrainConfig;
			List<List<FuelData.Entry>> fuels;

			powertrainConfig = _segment.Missions.Select(
					mission => CreateTrailerVectoRunData(_genericInputDataProvider.JobInputData.Vehicle, 0, mission, mission.Loadings.FirstOrDefault())
					).FirstOrDefault(x => x != null);
			fuels = _genericInputDataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, null)).ToList())
					.ToList();
		
			Report.InitializeReport(powertrainConfig, fuels);
		}

		protected virtual void Initialize()
		{
			//DoTests();
			#region Get trailer specific data from trailerAdapter
			_trailer = TrailerInputDataProvider.JobInputData.Trailer;
			TrailerDataAdapter.ValidateTrailerInput(_trailer);
			var volumeOrientation = (_trailer.InternalBodyHeight >= 2.9 && _trailer.TotalTrailerHeight <= 4);
			_trailerSegment = DeclarationData.TrailerSegments.Lookup(_trailer.NumberOfAxles, _trailer.TrailerType, _trailer.BodyworkCode, volumeOrientation);
			_genericInputDataProvider = TrailerDataAdapter.GetGenericVehicleData(_trailerSegment.GenericCode);
			_specificTrailerData = TrailerDataAdapter.GetSpecificTrailerData(_trailerSegment, _trailer, _genericInputDataProvider.JobInputData.Vehicle);
			#endregion

			_segment = GetSegment(_genericInputDataProvider.JobInputData.Vehicle);
			_driverdata = DataAdapter.CreateDriverData();
			_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segment.AccelerationFile);

			var tempVehicle = DataAdapter.CreateVehicleData(_genericInputDataProvider.JobInputData.Vehicle, _segment, _segment.Missions.First(), _segment.Missions.First().Loadings.First());

			_airdragData = DataAdapter.CreateAirdragData(_genericInputDataProvider.JobInputData.Vehicle.Components.AirdragInputData, _segment.Missions.First(), _segment);

			_axlegearData = _genericInputDataProvider.JobInputData.Vehicle.AxleConfiguration.AxlegearIncludedInGearbox() ?
				DataAdapter.CreateDummyAxleGearData(_genericInputDataProvider.JobInputData.Vehicle.Components.GearboxInputData) : DataAdapter.CreateAxleGearData(_genericInputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);
			
			_angledriveData = DataAdapter.CreateAngledriveData(_genericInputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData);

			var tmpRunData = new VectoRunData {
				ShiftStrategy = _genericInputDataProvider.JobInputData.ShiftStrategy,
				GearboxData =  new GearboxData {
					Type = _genericInputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.Type,
				}
			};
			var tmpStrategy = PowertrainBuilder.GetShiftStrategy(tmpRunData, new SimplePowertrainContainer(tmpRunData));
			var tmpEngine = DataAdapter.CreateEngineData(_genericInputDataProvider.JobInputData.Vehicle, 
				 _genericInputDataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes[0], _segment.Missions.First());

			_gearboxData = DataAdapter.CreateGearboxData(
				_genericInputDataProvider.JobInputData.Vehicle, new VectoRunData() { EngineData = tmpEngine, AxleGearData = _axlegearData, VehicleData = tempVehicle },
				tmpStrategy);
				
			_retarderData = DataAdapter.CreateRetarderData(_genericInputDataProvider.JobInputData.Vehicle.Components.RetarderInputData);

			_ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(_genericInputDataProvider.JobInputData.Vehicle.Components.PTOTransmissionInputData);

			_gearshiftData = DataAdapter.CreateGearshiftData(_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), tmpEngine.IdleSpeed);
		}

		protected VectoRunData CreateVectoRunData(
			IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var engine = _genericInputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];

			DrivingCycleData cycle;
			lock (CyclesCacheLock)
			{
				if (CyclesCache.ContainsKey(mission.MissionType))
					cycle = CyclesCache[mission.MissionType];
				else
				{
					cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
					CyclesCache.Add(mission.MissionType, cycle);
				}
			}
			var simulationRunData = new VectoRunData
			{
				Loading = loading.Key,
				VehicleData = TrailerDataAdapter.CreateVehicleData(vehicle, _segment, mission, loading),
				VehicleDesignSpeed = _segment.DesignSpeed,
				AirdragData = TrailerDataAdapter.CreateAirdragData(_genericInputDataProvider.JobInputData.Vehicle.Components.AirdragInputData, mission, _segment),
				EngineData = TrailerDataAdapter.CreateEngineData(_genericInputDataProvider.JobInputData.Vehicle, engineMode, mission), 
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = TrailerDataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, vehicle.Length),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = TrailerInputDataProvider.JobInputData.JobName,
				ModFileSuffix = (engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key,
				Report = Report,
				Mission = mission,
				InputDataHash = TrailerInputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
				ShiftStrategy = TrailerInputDataProvider.JobInputData.ShiftStrategy
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;

			return simulationRunData;
		}

		protected VectoRunData CreateTrailerVectoRunData(
			IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var engine = _genericInputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];
			var trailerMission = mission;

			DrivingCycleData cycle;
			lock (CyclesCacheLock)
			{
				if (CyclesCache.ContainsKey(mission.MissionType))
					cycle = CyclesCache[mission.MissionType];
				else
				{
					cycle = DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false);
					CyclesCache.Add(mission.MissionType, cycle);
				}
			}

			//Switch generic loadings with trailer loadings for this mission from trailerSegment
			var minLoad = _trailerSegment.Missions.FirstOrDefault(x => x.MissionType == mission.MissionType)?.MinLoad;
			var refLoad = _trailerSegment.Missions.FirstOrDefault(x => x.MissionType == mission.MissionType)?.MaxLoad;

			if (loading.Key == LoadingType.LowLoading)
				loading = new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.LowLoading, new Tuple<Kilogram, double?>(minLoad, loading.Value.Item2));

			if (loading.Key == LoadingType.ReferenceLoad)
				loading = new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad, new Tuple<Kilogram, double?>(refLoad, loading.Value.Item2));


			var simulationRunData = new VectoRunData
			{
				Loading = loading.Key,
				VehicleData = TrailerDataAdapter.CreateTrailerData(vehicle, _trailer, _trailerSegment, _segment, mission, loading, _specificTrailerData),
				VehicleDesignSpeed = _segment.DesignSpeed,
				AirdragData = TrailerDataAdapter.CreateTrailerAirdragData(_genericInputDataProvider.JobInputData.Vehicle.Components.AirdragInputData, mission, _trailerSegment, _specificTrailerData),
				EngineData = TrailerDataAdapter.CreateEngineData(_genericInputDataProvider.JobInputData.Vehicle, engineMode, mission),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = TrailerDataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, vehicle.Length),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = TrailerInputDataProvider.JobInputData.JobName,
				ModFileSuffix = (engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key + "_trailer",
				Report = Report,
				Mission = trailerMission,
				InputDataHash = TrailerInputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
				ShiftStrategy = TrailerInputDataProvider.JobInputData.ShiftStrategy
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;

			return simulationRunData;
		}

		public IEnumerable<VectoRunData> NextRun()
		{
		
			Initialize();
			if (Report != null) 
				InitializeReport();

			return GetNextRun();
		}
	}
}
