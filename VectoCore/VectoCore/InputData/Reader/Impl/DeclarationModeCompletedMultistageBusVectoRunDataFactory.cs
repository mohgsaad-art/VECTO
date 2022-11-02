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

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationModeCompletedMultistageBusVectoRunDataFactory : LoggingObject, IVectoRunDataFactory
	{
		protected readonly IMultistageBusInputDataProvider InputDataProvider;
		protected IDeclarationReport Report;

		protected Segment _segmentCompletedBus;
		protected AxleGearData _axlegearData;
		protected AngledriveData _angledriveData;
		protected GearboxData _gearboxData;
		protected RetarderData _retarderData;
		protected ShiftStrategyParameters _gearshiftData;
		private DriverData _driverData;

		protected DeclarationDataAdapterMultistageBus DataAdapterSpecific = new DeclarationDataAdapterMultistageBus();

		protected DeclarationDataAdapterCompletedBusGeneric DataAdapterGeneric = new DeclarationDataAdapterCompletedBusGeneric();

		public DeclarationModeCompletedMultistageBusVectoRunDataFactory(
			IMultistageBusInputDataProvider dataProvider, IDeclarationReport report)
		{

			InputDataProvider = dataProvider;
			Report = report;
		}



		protected IVehicleDeclarationInputData PrimaryVehicle => InputDataProvider.JobInputData.PrimaryVehicle.Vehicle;

		protected IVehicleDeclarationInputData CompletedVehicle => InputDataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle;


		public IEnumerable<VectoRunData> NextRun()
		{
			Initialize();
			if (Report != null)
			{
				InitializeReport();
			}

			return GetNextRun();
		}

		protected virtual void InitializeReport()
		{
			var powertrainConfig = CompletedVehicle.ExemptedVehicle || PrimaryVehicle.ExemptedVehicle
				? GetExemptedVectoRunData()
				:_segmentCompletedBus.Missions.Select(
						mission => CreateVectoRunDataSpecific(
							mission, mission.Loadings.First(), 0))
					.FirstOrDefault(x => x != null);

			Report.InitializeReport(powertrainConfig, new List<List<FuelData.Entry>>());
		}

		private VectoRunData GetExemptedVectoRunData()
		{
			return new VectoRunData() {
				Exempted = true,
				VehicleData = new VehicleData() {
					ModelName = CompletedVehicle.Model,
					Manufacturer = CompletedVehicle.Manufacturer,
					ManufacturerAddress = CompletedVehicle.ManufacturerAddress,
					VIN =  CompletedVehicle.VIN,
					LegislativeClass = CompletedVehicle.LegislativeClass,
					RegisteredClass = CompletedVehicle.RegisteredClass,
					VehicleCode = CompletedVehicle.VehicleCode,
					CurbMass = CompletedVehicle.CurbMassChassis,
					GrossVehicleMass = CompletedVehicle.GrossVehicleMassRating,
					ZeroEmissionVehicle = PrimaryVehicle.ZeroEmissionVehicle,
					MaxNetPower1 = PrimaryVehicle.MaxNetPower1,
					InputData = CompletedVehicle
				},
				Report = Report,
				Mission = new Mission() {
					MissionType = MissionType.ExemptedMission
				}
			};
		}


		protected virtual void Initialize()
		{
			if (PrimaryVehicle.ExemptedVehicle || CompletedVehicle.ExemptedVehicle)
			{
				return;
			}

			_segmentCompletedBus = GetCompletedSegment(CompletedVehicle, PrimaryVehicle.AxleConfiguration);

			var tmpVehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segmentCompletedBus, _segmentCompletedBus.Missions.First(),
				_segmentCompletedBus.Missions.First().Loadings.First());
			tmpVehicleData.VehicleCategory = VehicleCategory.GenericBusVehicle;

			var combustionEngineData = DataAdapterGeneric.CreateEngineData(PrimaryVehicle, 0, _segmentCompletedBus.Missions.First());

			_axlegearData = DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);

			_angledriveData = DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);

			var tmpRunData = new VectoRunData()
			{
				ShiftStrategy = null,//ToDo missing ShiftStrategy InputDataProvider.JobInputData.ShiftStrategy
				GearboxData = new GearboxData()
				{
					Type = PrimaryVehicle.Components.GearboxInputData.Type,
				}
			};
			var tmpStrategy = PowertrainBuilder.GetShiftStrategy(new SimplePowertrainContainer(tmpRunData));

			_gearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, new VectoRunData() { EngineData = combustionEngineData, AxleGearData = _axlegearData, VehicleData = tmpVehicleData },
				tmpStrategy);

			_gearshiftData = DataAdapterGeneric.CreateGearshiftData(
				_gearboxData, _axlegearData.AxleGear.Ratio * (_angledriveData?.Angledrive.Ratio ?? 1.0), combustionEngineData.IdleSpeed);

			_retarderData = DataAdapterGeneric.CreateRetarderData(PrimaryVehicle.Components.RetarderInputData);
			
			_driverData = DataAdapterGeneric.CreateDriverData();
			_driverData.AccelerationCurve = AccelerationCurveReader.ReadFromStream(_segmentCompletedBus.AccelerationFile);
		}
		
		protected virtual IEnumerable<VectoRunData> GetNextRun()
		{
			if (InputDataProvider.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle) {
				return new[] { GetExemptedVectoRunData() };
			}
			return VectoRunDataHeavyBusCompleted();
		}


		private IEnumerable<VectoRunData> VectoRunDataHeavyBusCompleted()
		{
			var engineModes = InputDataProvider.JobInputData.PrimaryVehicle.Vehicle.Components.EngineInputData.EngineModes;

			for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
			{
				var fuelMode = "single fuel mode";
				if (engineModes[modeIdx].Fuels.Count > 1)
				{
					fuelMode = "dual fuel mode";
				}
				foreach (var mission in _segmentCompletedBus.Missions)
				{
					foreach (var loading in mission.Loadings)
					{
						var simulationRunData = CreateVectoRunDataSpecific(mission, loading, modeIdx);
						if (simulationRunData != null)
						{
							yield return simulationRunData;
						}

						var primarySegment = GetPrimarySegment(PrimaryVehicle);
						var primaryMission = primarySegment.Missions.Where(
							m => {
								return m.BusParameter.DoubleDecker ==
										CompletedVehicle.VehicleCode.IsDoubleDeckerBus() &&
										m.MissionType == mission.MissionType &&
										m.BusParameter.FloorType == CompletedVehicle.VehicleCode.GetFloorType();
							}).First();
						simulationRunData = CreateVectoRunDataGeneric(
							primaryMission,
							new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(loading.Key, primaryMission.Loadings[loading.Key]),
							primarySegment, modeIdx);

						var primaryResult = InputDataProvider.JobInputData.PrimaryVehicle.GetResult(
							simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
							simulationRunData.VehicleData.Loading);
						if (primaryResult == null || !primaryResult.ResultStatus.Equals("success"))
						{
							throw new VectoException(
								"Failed to find results in PrimaryVehicleReport for vehicle group: {0},  mission: {1}, fuel mode: '{2}', payload: {3}. Make sure PIF and completed vehicle data match!",
								simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
								simulationRunData.VehicleData.Loading);
						}

						if (primaryResult.ResultStatus != "success")
						{
							throw new VectoException(
								"Simulation results in PrimaryVehicleReport for vehicle group: {0},  mission: {1}, fuel mode: '{2}', payload: {3} not finished successfully.",
								simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
								simulationRunData.VehicleData.Loading);
						}

						simulationRunData.PrimaryResult = primaryResult;

						yield return simulationRunData;
					}
				}
			}
		}

		protected virtual Segment GetPrimarySegment(IVehicleDeclarationInputData primaryVehicle)
		{
			var primarySegment = DeclarationData.PrimaryBusSegments.Lookup(
				primaryVehicle.VehicleCategory, primaryVehicle.AxleConfiguration, primaryVehicle.Articulated);

			return primarySegment;
		}


		protected virtual Segment GetCompletedSegment(IVehicleDeclarationInputData vehicle, AxleConfiguration axleConfiguration)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(
				axleConfiguration.NumAxles(), vehicle.VehicleCode, vehicle.RegisteredClass, vehicle.NumberPassengerSeatsLowerDeck,
				vehicle.Height, vehicle.LowEntry);
			if (!segment.Found)
			{
				throw new VectoException(
					"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, vehicle code: {3}, registered class: {4}, passengersLowerDeck: {5}, height: {6}, lowEntry: {7}. completed",
					vehicle.VehicleCategory, axleConfiguration,
					vehicle.Articulated, vehicle.VehicleCode, vehicle.RegisteredClass.GetLabel(), vehicle.NumberPassengerSeatsLowerDeck,
					vehicle.Height, vehicle.LowEntry);
			}

			return segment;
		}


		protected VectoRunData CreateVectoRunDataSpecific(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int modeIdx)
		{
			var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));
			
			var simulationRunData = new VectoRunData
			{
				Loading = loading.Key,
				VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segmentCompletedBus,
					mission, loading),
				AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission),
				EngineData = DataAdapterSpecific.CreateEngineData(PrimaryVehicle, modeIdx, mission),
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapterSpecific.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segmentCompletedBus.VehicleClass, CompletedVehicle.Length,
					PrimaryVehicle.Components.AxleWheels.NumSteeredAxles),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverData,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,//?!? Jobname
				ModFileSuffix = $"_{_segmentCompletedBus.VehicleClass.GetClassNumber()}-Specific_{loading.Key}",
				Report = Report,
				Mission = mission,
				InputDataHash = InputDataProvider.XMLHash,// right hash?!?
				SimulationType = SimulationType.DistanceCycle,
				VehicleDesignSpeed = _segmentCompletedBus.DesignSpeed,
				GearshiftParameters = _gearshiftData,
			};
			simulationRunData.EngineData.FuelMode = 0;
			simulationRunData.VehicleData.VehicleClass = _segmentCompletedBus.VehicleClass;
			simulationRunData.BusAuxiliaries = DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, simulationRunData);

			return simulationRunData;
		}

		
		protected VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int modeIdx)
		{
			var cycle = DeclarationData.CyclesCache.GetOrAdd(mission.MissionType, _ => DrivingCycleDataReader.ReadFromStream(mission.CycleFile, CycleType.DistanceBased, "", false));
			
			var primaryBusAuxiliaries = PrimaryVehicle.Components.BusAuxiliaries;

			var simulationRunData = new VectoRunData {
				Loading = loading.Key,
				VehicleData = DataAdapterGeneric.CreateVehicleData(PrimaryVehicle, primarySegment, mission, loading, false),
				AirdragData = DataAdapterGeneric.CreateAirdragData(null, mission, new Segment()),
				EngineData = DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx, mission),
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				GearboxData = _gearboxData,
				AxleGearData = _axlegearData,
				AngledriveData = _angledriveData,
				Aux = DataAdapterGeneric.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
					primaryBusAuxiliaries, mission.MissionType, primarySegment.VehicleClass,
					mission.BusParameter.VehicleLength,
					PrimaryVehicle.Components.AxleWheels.NumSteeredAxles),
				Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
				Retarder = _retarderData,
				DriverData = _driverData,
				ExecutionMode = ExecutionMode.Declaration,
				JobName = InputDataProvider.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,
				ModFileSuffix = $"_{_segmentCompletedBus.VehicleClass.GetClassNumber()}-Generic_{loading.Key}",
				Report = Report,
				Mission = mission,
				InputDataHash = InputDataProvider.XMLHash,
				SimulationType = SimulationType.DistanceCycle,
				VehicleDesignSpeed = _segmentCompletedBus.DesignSpeed,
				GearshiftParameters = _gearshiftData,
			};
			simulationRunData.EngineData.FuelMode = 0;
			simulationRunData.VehicleData.VehicleClass = _segmentCompletedBus.VehicleClass;
			simulationRunData.BusAuxiliaries =
				DataAdapterGeneric.CreateBusAuxiliariesData(mission, PrimaryVehicle, simulationRunData);

			return simulationRunData;
		}


	}
}
