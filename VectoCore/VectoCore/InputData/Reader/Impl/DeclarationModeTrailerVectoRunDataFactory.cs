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
using System.Linq;
using Ninject;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;

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
		private DriverData _driverdata;
		private AxleGearData _axlegearData;
		private AngledriveData _angledriveData;
		private GearboxData _gearboxData;
		private RetarderData _retarderData;
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
				foreach (var mission in _trailerSegment.Missions) {
					foreach (var loading in mission.Loadings) {
						yield return CreateSpecificTrailerVectoRunData(vehicle, modeIdx, mission, loading);
						yield return CreateVectoRunData(vehicle, modeIdx, mission, loading);
					}
				}
			}
		}

		protected virtual void InitializeReport()
		{
			var powertrainConfig = _trailerSegment.Missions.Select(
				mission => CreateSpecificTrailerVectoRunData(_genericInputDataProvider.JobInputData.Vehicle, 0, mission, mission.Loadings.FirstOrDefault())
			).FirstOrDefault(x => x != null);
			var fuels = _genericInputDataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, null)).ToList())
				.ToList();
		
			Report.InitializeReport(powertrainConfig, fuels);
		}

		protected virtual void Initialize()
		{
			#region Get trailer specific data from trailerAdapter
			_trailer = TrailerInputDataProvider.JobInputData.Trailer;
			TrailerDataAdapter.ValidateTrailerInput(_trailer);
			var volumeOrientation = _trailer.VolumeOrientation;
			_trailerSegment = DeclarationData.TrailerSegments.Lookup(_trailer.NumberOfAxles, _trailer.TrailerType, _trailer.BodyworkCode, volumeOrientation, _trailer.TPMLMAxleAssembly);
			_genericInputDataProvider = DeclarationData.TrailerSegments.GetGenericVehicleData(_trailerSegment.GenericCode);

			_specificTrailerData = TrailerDataAdapter.GetTrailerData(_trailerSegment, _trailer, _genericInputDataProvider.JobInputData.Vehicle);

			//Add Corrected A1, A2, A3
			_trailerSegment.A1Cor = _specificTrailerData.TotalCorrection.A1;
			_trailerSegment.A2Cor = _specificTrailerData.TotalCorrection.A2;
			_trailerSegment.A3Cor = _specificTrailerData.TotalCorrection.A3;

			_trailerSegment.A1RefCor = _specificTrailerData.TotalRefCorrection.A1;
			_trailerSegment.A2RefCor = _specificTrailerData.TotalRefCorrection.A2;
			_trailerSegment.A3RefCor = _specificTrailerData.TotalRefCorrection.A3;
			#endregion

			_driverdata = DataAdapter.CreateDriverData();
			_driverdata.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_trailerSegment.AccelerationFile);

			var _tempData = TrailerDataAdapter.CreateVehicleData(_genericInputDataProvider.JobInputData.Vehicle, _trailerSegment.Missions.First(), _trailerSegment.Missions.First().Loadings.First(), _trailerSegment);
			_tempData.DynamicTyreRadius = _genericInputDataProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration.Where(axle => axle.AxleType == AxleType.VehicleDriven)
					.Select(da => DeclarationData.Wheels.Lookup(da.Tyre.Dimension).DynamicTyreRadius).Average();

			TrailerDataAdapter.CreateAirdragData(_genericInputDataProvider.JobInputData.Vehicle.Components.AirdragInputData, _trailerSegment.Missions.First(), _specificTrailerData, _trailerSegment);

			_axlegearData = _genericInputDataProvider.JobInputData.Vehicle.AxleConfiguration.AxlegearIncludedInGearbox() ?
				DataAdapter.CreateDummyAxleGearData(_genericInputDataProvider.JobInputData.Vehicle.Components.GearboxInputData) : 
				DataAdapter.CreateAxleGearData(_genericInputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);

			_angledriveData = DataAdapter.CreateAngledriveData(_genericInputDataProvider.JobInputData.Vehicle.Components.AngledriveInputData);

			var tmpRunData = new VectoRunData
			{
				ShiftStrategy = _genericInputDataProvider.JobInputData.ShiftStrategy,
				GearboxData = new GearboxData
				{
					Type = _genericInputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.Type,
				}
			};
			var tmpStrategy = PowertrainBuilder.GetShiftStrategy(tmpRunData, new SimplePowertrainContainer(tmpRunData));
			var tmpEngine = DataAdapter.CreateEngineData(_genericInputDataProvider.JobInputData.Vehicle,
				 _genericInputDataProvider.JobInputData.Vehicle.Components.EngineInputData.EngineModes[0], _trailerSegment.Missions.First());

			_gearboxData = DataAdapter.CreateGearboxData(
				_genericInputDataProvider.JobInputData.Vehicle, new VectoRunData() { EngineData = tmpEngine, AxleGearData = _axlegearData, VehicleData = _tempData },
				tmpStrategy);

			_retarderData = DataAdapter.CreateRetarderData(_genericInputDataProvider.JobInputData.Vehicle.Components.RetarderInputData);

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
				VehicleData = TrailerDataAdapter.CreateVehicleData(vehicle, mission, loading, _trailerSegment),
				VehicleDesignSpeed = _trailerSegment.DesignSpeed,
				AirdragData = TrailerDataAdapter.CreateAirdragData(_genericInputDataProvider.JobInputData.Vehicle.Components.AirdragInputData, mission, _specificTrailerData, _trailerSegment),
				EngineData = TrailerDataAdapter.CreateEngineData(_genericInputDataProvider.JobInputData.Vehicle, engineMode, mission), 
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = TrailerDataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType,
					_trailerSegment.Class, vehicle.Length),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = TrailerInputDataProvider.JobInputData.JobName,
				ModFileSuffix = (engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key + "_referenceTrailer",
				Report = Report,
				Mission = mission,
				InputDataHash = TrailerInputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
				ShiftStrategy = TrailerInputDataProvider.JobInputData.ShiftStrategy
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _trailerSegment.Class; 

			return simulationRunData;
		}

		protected VectoRunData CreateSpecificTrailerVectoRunData(
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

			//Switch reference loadings with specific loadings for specific missions
			if (loading.Key == LoadingType.LowLoading)
				loading = new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.LowLoading, new Tuple<Kilogram, double?>(mission.SpecificTrailerLowLoad, loading.Value.Item2));

			if (loading.Key == LoadingType.ReferenceLoad)
				loading = new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad, new Tuple<Kilogram, double?>(mission.SpecificTrailerRefLoad, loading.Value.Item2));


			var simulationRunData = new VectoRunData
			{
				Loading = loading.Key,
				VehicleData = TrailerDataAdapter.CreateTrailerData(vehicle, _trailer, _trailerSegment, mission, loading, _specificTrailerData),
				VehicleDesignSpeed = _trailerSegment.DesignSpeed,
				AirdragData = TrailerDataAdapter.CreateTrailerAirdragData(_genericInputDataProvider, mission, _specificTrailerData, _trailerSegment),
				EngineData = TrailerDataAdapter.CreateTrailerEngineData(_genericInputDataProvider.JobInputData.Vehicle, engineMode, mission, _trailer, loading.Key),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = TrailerDataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType,
					_trailerSegment.Class, vehicle.Length),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = TrailerInputDataProvider.JobInputData.JobName,
				ModFileSuffix = (engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key + "_specificTrailer",
				Report = Report,
				Mission = trailerMission,
				InputDataHash = TrailerInputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
				ShiftStrategy = TrailerInputDataProvider.JobInputData.ShiftStrategy
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _trailerSegment.Class;

			return simulationRunData;
		}

		public IEnumerable<VectoRunData> NextRun()
		{
		
			Initialize();
			if (Report != null) 
				InitializeReport();

			return GetNextRun();
		}

		public IInputDataProvider DataProvider => TrailerInputDataProvider;
	}
}
