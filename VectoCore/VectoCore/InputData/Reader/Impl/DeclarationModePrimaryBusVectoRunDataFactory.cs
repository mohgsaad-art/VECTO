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
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModePrimaryBusVectoRunDataFactory : AbstractDeclarationVectoRunDataFactory
	{
		protected DeclarationDataAdapterPrimaryBus _dao = new DeclarationDataAdapterPrimaryBus();

		public DeclarationModePrimaryBusVectoRunDataFactory(IDeclarationInputDataProvider dataProvider, IDeclarationReport report) :
			base(dataProvider, report)
		{ }

		#region Overrides of AbstractDeclarationVectoRunDataFactory

		protected override IDeclarationDataAdapter DataAdapter => _dao;

		#endregion

		protected override IEnumerable<VectoRunData> GetNextRun()
		{
			if (InputDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle) {
				if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
					yield return CreateVectoRunData(InputDataProvider.JobInputData.Vehicle, 0, null,
						new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>());
				} else {
					foreach (var vectoRunData in VectoRunDataHeavyBusPrimary()) {
						yield return vectoRunData;
					}
				}
			}

			foreach (var entry in new List<VectoRunData>()) {
				yield return entry;
			}
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
			if (InputDataProvider.JobInputData.Vehicle.ExemptedVehicle) {
				return new VectoRunData {
					Exempted = true,
					Report = Report,
					Mission = new Mission { MissionType = MissionType.ExemptedMission },
					VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, new Segment(),
						null,
						new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad,
							Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null)), _allowVocational),
					InputDataHash = InputDataProvider.XMLHash
				};
			}

			var engine = vehicle.Components.EngineInputData;
			var engineModes = engine.EngineModes;
			var engineMode = engineModes[modeIdx];

			var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));

			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleData = DataAdapter.CreateVehicleData(vehicle, _segment, mission, loading, _allowVocational),
				VehicleDesignSpeed = _segment.DesignSpeed,
				AirdragData = _dao.CreateAirdragData(null, mission, new Segment()),
				EngineData = DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode, mission),
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapter.CreateAuxiliaryData(
					vehicle.Components.AuxiliaryInputData,
					vehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					vehicle.Length ?? mission.BusParameter.VehicleLength,
					vehicle.Components.AxleWheels.NumSteeredAxles),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverdata,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.JobName,
				ModFileSuffix = $"{(engineModes.Count > 1 ? $"_EngineMode{modeIdx}_" : "")}" +
								$"_{mission.BusParameter.BusGroup.GetClassNumber()}_{loading.Key}",
				Report = Report,
				Mission = mission,
				InputDataHash = InputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				GearshiftParameters = _gearshiftData,
			};
			simulationRunData.EngineData.FuelMode = modeIdx;
			simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
			simulationRunData.BusAuxiliaries = _dao.CreateBusAuxiliariesData(
				mission, InputDataProvider.JobInputData.Vehicle, simulationRunData);
			return simulationRunData;
		}
	}
}
