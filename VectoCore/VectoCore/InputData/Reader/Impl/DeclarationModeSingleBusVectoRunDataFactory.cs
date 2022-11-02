using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	internal class DeclarationModeSingleBusVectoRunDataFactory : DeclarationModePrimaryBusVectoRunDataFactory
	{
		protected new DeclarationDataAdapterSingleBus _dao = new DeclarationDataAdapterSingleBus();
		private ISingleBusInputDataProvider _singleBusInputData;

		public DeclarationModeSingleBusVectoRunDataFactory(ISingleBusInputDataProvider singleBusInputData, IDeclarationReport report) : base(singleBusInputData, report)
		{
			_singleBusInputData = singleBusInputData;
			_dao.SingleBusInputData = singleBusInputData;
		}

		
		protected override Segment GetSegment(IVehicleDeclarationInputData vehicle)
		{
			//if (vehicle.VehicleCategory != VehicleCategory.HeavyBusCompletedVehicle) {
			//	throw new VectoException(
			//		"Invalid vehicle category for bus factory! {0}", vehicle.VehicleCategory.GetCategoryName());
			//}

			var completedVehicle = _singleBusInputData.CompletedVehicle;

			var segment = DeclarationData.CompletedBusSegments.Lookup(
				_singleBusInputData.PrimaryVehicle.AxleConfiguration.NumAxles(), completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.NumberPassengerSeatsLowerDeck,
				completedVehicle.Height, completedVehicle.LowEntry);
			if (!segment.Found) {
				throw new VectoException(
					"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, vehicle code: {3}, registered class: {4}, passengersLowerDeck: {5}, height: {6}, lowfloor: {7}. completed",
					vehicle.VehicleCategory, _singleBusInputData.PrimaryVehicle.AxleConfiguration,
					vehicle.Articulated, completedVehicle.VehicleCode, completedVehicle.RegisteredClass.GetLabel(), completedVehicle.NumberPassengerSeatsLowerDeck,
					completedVehicle.Height, completedVehicle.LowEntry);
			}
			foreach (var mission in segment.Missions) {
				mission.VehicleHeight = completedVehicle.Height + mission.BusParameter.DeltaHeight;
				mission.BusParameter.VehicleLength = completedVehicle.Length;
			}

			return segment;
		}


		protected override IDeclarationDataAdapter DataAdapter => _dao;


		protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var doubleDecker = _singleBusInputData.CompletedVehicle.NumberPassengerSeatsUpperDeck > 0;
			if (mission.BusParameter.DoubleDecker != doubleDecker) {
				return null;
			}

			var engine = vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];

			var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational),
				AirdragData = _dao.CreateAirdragData(_singleBusInputData.CompletedVehicle, mission),
				EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission),
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapter.CreateAuxiliaryData(vehicle.Components.AuxiliaryInputData,
													vehicle.Components.BusAuxiliaries, mission.MissionType, 
													_segment.VehicleClass, vehicle.Length ?? mission.BusParameter.VehicleLength,
													vehicle.Components.AxleWheels.NumSteeredAxles),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.JobName,
				ModFileSuffix = $"{(engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "")}" +
								$"_{mission.BusParameter.BusGroup.GetClassNumber()}-Single_{loading.Key}",
				Report = Report,
				Mission = mission,
				InputDataHash = InputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
				VehicleDesignSpeed = _segment.DesignSpeed,
				//ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
			simulationRunData.BusAuxiliaries = _dao.CreateBusAuxiliariesData(mission, _singleBusInputData.PrimaryVehicle, _singleBusInputData.CompletedVehicle, simulationRunData);
			return simulationRunData;
		}

		protected override void InitializeReport()
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
				fuels = vehicle.Components.EngineInputData.EngineModes.Select(x => x.Fuels.Select(f => DeclarationData.FuelData.Lookup(f.FuelType, _singleBusInputData.CompletedVehicle.TankSystem)).ToList())
					.ToList();
			}
			Report.InitializeReport(powertrainConfig, fuels);
		}
	}

	
}
