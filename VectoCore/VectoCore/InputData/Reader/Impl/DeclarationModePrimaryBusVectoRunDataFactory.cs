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
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModePrimaryBusVectoRunDataFactory : AbstractDeclarationVectoRunDataFactory
	{
		protected DeclarationDataAdapterPrimaryBus _dao = new DeclarationDataAdapterPrimaryBus();

		public DeclarationModePrimaryBusVectoRunDataFactory(IDeclarationInputDataProvider dataProvider, IDeclarationReport report) :
			base(dataProvider, report) { }

		#region Overrides of AbstractDeclarationVectoRunDataFactory

		protected override IDeclarationDataAdapter DataAdapter
		{
			get { return _dao; }
		}
		#endregion

		protected override IEnumerable<VectoRunData> GetNextRun()
		{
			if (InputDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle) {
				return VectoRunDataHeavyBusPrimary();
			}

			return new List<VectoRunData>();
		}

		protected override Segment GetSegment(IVehicleDeclarationInputData vehicle)
		{
			if (vehicle.VehicleCategory != VehicleCategory.HeavyBusPrimaryVehicle) {
				throw new VectoException(
					"Invalid vehicle category for bus factory! {0}", vehicle.VehicleCategory.GetCategoryName());
			}

			var segment = DeclarationData.PrimaryBusSegments.Lookup(
				vehicle.VehicleCategory, vehicle.AxleConfiguration, vehicle.Articulated);
			if (!segment.Found) {
				throw new VectoException(
					"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, primary",
					vehicle.VehicleCategory, vehicle.AxleConfiguration,
					vehicle.Articulated);
			}

			return segment;
		}

		private IEnumerable<VectoRunData> VectoRunDataHeavyBusPrimary()
		{
			var vehicle = InputDataProvider.JobInputData.Vehicle;
			var engine = vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;

			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
				foreach (var mission in _segment.Missions) {
					foreach (var loading in mission.Loadings) {
						var simulationRunData = CreateVectoRunData(vehicle, modeIdx, mission, loading);
						if (simulationRunData == null) {
							continue;
						}
						yield return simulationRunData;
					}
				}
			}
		}


		protected override VectoRunData CreateVectoRunData(
			IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			var engine = vehicle.Components.EngineInputData;
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
				AirdragData = _dao.CreateAirdragData(null, mission, new Segment()),
				EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					vehicle.Length ?? mission.BusParameter.VehicleLength),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.JobName,
				ModFileSuffix = (engineModes.Count > 1 ? string.Format("_EngineMode{0}_", modeIdx) : "") + "_" + mission.BusParameter.BusGroup.GetClassNumber() + "_" + loading.Key.ToString(),
				Report = Report,
				Mission = mission,
				InputDataHash = InputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
				ShiftStrategy = InputDataProvider.JobInputData.ShiftStrategy
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
			simulationRunData.BusAuxiliaries = _dao.CreateBusAuxiliariesData(
				mission, InputDataProvider.JobInputData.Vehicle, simulationRunData);
			return simulationRunData;
		}
	}
}
