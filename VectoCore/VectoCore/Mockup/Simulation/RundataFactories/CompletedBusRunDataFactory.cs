using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Mockup.Simulation.RundataFactories
{
	internal class MockupMultistageCompletedBusRunDataFactory : DeclarationModeCompletedBusRunDataFactory.CompletedBusBase
    {
		
		public MockupMultistageCompletedBusRunDataFactory(IMultistageVIFInputData dataProvider,
            IDeclarationReport report,
			ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
			IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory, IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) 
			: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter, ptBuilder)
        {

        }

        protected override void Initialize()
        {
			_segment = GetCompletedSegment();
		}

		protected override IEnumerable<VectoRunData> GetNextRun()
		{
			var InputDataProvider = DataProvider.MultistageJobInputData;
			if (InputDataProvider.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle) {
				return new[] { GetExemptedVectoRunData() };
			}
			return VectoRunDataHeavyBusCompleted();
		}

		protected virtual VectoRunData GetExemptedVectoRunData()
		{
			var InputDataProvider = DataProvider.MultistageJobInputData;
            return new VectoRunData() {
				Exempted = true,
				VehicleData = new VehicleData() {
					ModelName = CompletedVehicle.Model,
					Manufacturer = CompletedVehicle.Manufacturer,
					ManufacturerAddress = CompletedVehicle.ManufacturerAddress,
					VIN = CompletedVehicle.VIN,
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
				},
				InputData = InputDataProvider
			};
		}

        protected virtual IEnumerable<VectoRunData> VectoRunDataHeavyBusCompleted()
		{
            if (PrimaryVehicle.VehicleType.IsOneOf(VectoSimulationJobType.IEPC_E, VectoSimulationJobType.BatteryElectricVehicle)) {
				foreach (var vectoRunData in CreateVectoRunDataForMissions(0, ""))
					yield return vectoRunData;
			} else {
				var engineModes = PrimaryVehicle.Components.EngineInputData
					?.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					var fuelMode = "single fuel mode";
					if (engineModes[modeIdx].Fuels.Count > 1) {
						fuelMode = "dual fuel mode";
					}

					foreach (var vectoRunData in CreateVectoRunDataForMissions(modeIdx, fuelMode))
						yield return vectoRunData;
				}
			}
		}


        protected override VectoRunData CreateVectoRunDataSpecific(Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx,
			OvcHevMode ovcMode = OvcHevMode.NotApplicable)
		{
			var cycle = CycleFactory.GetDeclarationCycle(mission);

            var simulationRunData = new VectoRunData
            {
                Loading = loading.Key,
                VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segment,
                    mission, loading),
                Retarder = PrimaryBusMockupRunDataFactory.CreateMockupRetarder(PrimaryVehicle),
                AirdragData = PrimaryBusMockupRunDataFactory.CreateMockupAirdragData(CompletedVehicle),
                EngineData = PrimaryBusMockupRunDataFactory.CreateMockupEngineData(PrimaryVehicle, modeIdx, CompletedVehicle.TankSystem),
                //ElectricMachinesData = PrimaryBusMockupRunDataFactory.CreateMockupElectricMachineData()
                AngledriveData = PrimaryBusMockupRunDataFactory.CreateMockupAngleDriveData(PrimaryVehicle.Components.AngledriveInputData),
                AxleGearData = PrimaryBusMockupRunDataFactory.CreateMockupAxleGearData(PrimaryVehicle.Components.AxleGearInputData),
                Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                Mission = mission,
                GearboxData = PrimaryBusMockupRunDataFactory.CreateMockupGearboxData(PrimaryVehicle.Components.GearboxInputData),
                InputData = DataProvider.MultistageJobInputData,
                SimulationType = SimulationType.DistanceCycle,
                ExecutionMode = ExecutionMode.Declaration,
                JobName = DataProvider.MultistageJobInputData.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,
                Report = Report,
				//Aux = PrimaryBusMockupRunDataFactory.CreateMockupBusAux(CompletedVehicle),

                //            //AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission),
                //            //EngineData = DataAdapterSpecific.CreateEngineData(PrimaryVehicle, modeIdx, mission),
                //            //ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
                //            //GearboxData = _gearboxData,
                //            //AxleGearData = _axlegearData,
                //            //AngledriveData = _angledriveData,
                //            Aux = DataAdapterSpecific.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
                //                PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segmentCompletedBus.VehicleClass, CompletedVehicle.Length,
                //                PrimaryVehicle.Components.AxleWheels.NumSteeredAxles),
                //Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                //Retarder = _retarderData,
                ////DriverData = _driverData,
                //ExecutionMode = ExecutionMode.Declaration,
                //JobName = InputDataProvider.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,//?!? Jobname
                ModFileSuffix = $"_{_segment.VehicleClass.GetClassNumber()}-Specific_{loading.Key}",
                //Report = Report,
                //Mission = mission,
                //InputDataHash = InputDataProvider.XMLHash,// right hash?!?
                //SimulationType = SimulationType.DistanceCycle,
                //VehicleDesignSpeed = _segmentCompletedBus.DesignSpeed,
                //GearshiftParameters = _gearshiftData,
            };
            if (simulationRunData.EngineData != null)
            {
                simulationRunData.EngineData.FuelMode = 0;
            }
            simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
            simulationRunData.BusAuxiliaries = DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, simulationRunData);

            return simulationRunData;

            //return base.CreateVectoRunDataSpecific(mission, loading, modeIdx);
        }

		protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
		{
			throw new NotImplementedException();
		}

		private IEnumerable<VectoRunData> CreateVectoRunDataForMissions(int modeIdx, string fuelMode)
        {
			var InputDataProvider = DataProvider.MultistageJobInputData;
            foreach (var mission in _segment.Missions) {
                foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {
                    var simulationRunData = CreateVectoRunDataSpecific(mission, loading, modeIdx);
                    if (simulationRunData != null) {
                        yield return simulationRunData;
                    }

                    var primarySegment = GetPrimarySegment();
                    var primaryMission = primarySegment.Missions.Where(
                        m => {
                            return m.BusParameter.DoubleDecker ==
                                    CompletedVehicle.VehicleCode.IsDoubleDeckerBus() &&
                                    m.MissionType == mission.MissionType &&
                                    m.BusParameter.FloorType == CompletedVehicle.VehicleCode.GetFloorType();
                        }).First();
                    simulationRunData = CreateVectoRunDataGeneric(
                        primaryMission,
                        new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(loading.Key,
							primaryMission.Loadings[loading.Key]),
                        primarySegment, modeIdx);

                    var primaryResult = InputDataProvider.JobInputData.PrimaryVehicle.GetResult(
                        simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
                        simulationRunData.VehicleData.Loading, OvcHevMode.NotApplicable);
					if (primaryResult == null) {
						throw new VectoException(
							"Failed to find results in PrimaryVehicleReport for vehicle group: {0},  mission: {1}, fuel mode: '{2}', payload: {3}. Make sure PIF and completed vehicle data match!",
							simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
							simulationRunData.VehicleData.Loading);
					}
					if (primaryResult.ResultStatus == ResultStatus.PrimaryRunIgnored) {
						throw new VectoException(
                            "The vehicle group of the complete(d) vehicle falls into a primary vehicle sub-group for which no result could be calculated due to the criterion of insufficient powertrain power.   mission: {1}, fuel mode: '{2}', payload: {3}.",
							simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
							simulationRunData.VehicleData.Loading);
					}
					if (primaryResult.ResultStatus != ResultStatus.Success) {
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
        protected override VectoRunData CreateVectoRunDataGeneric(Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx,
			OvcHevMode ovcHevMode = OvcHevMode.NotApplicable)
		{
			var cycle = CycleFactory.GetDeclarationCycle(mission);

            return new VectoRunData()
            {
                Mission = mission,
                Loading = loading.Key,
                VehicleData = new VehicleData()
                {
                    Loading = loading.Value.Item1,
                    VehicleClass = primarySegment.VehicleClass,
                },
                EngineData = PrimaryBusMockupRunDataFactory.CreateMockupEngineData(PrimaryVehicle, modeIdx, CompletedVehicle.TankSystem),
                JobName = DataProvider.MultistageJobInputData.JobInputData.ManufacturingStages.Last().Vehicle.Identifier,
                ExecutionMode = ExecutionMode.Declaration,
                SimulationType = SimulationType.DistanceCycle,
                Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
                Report = Report,
                ModFileSuffix = $"_{_segment.VehicleClass.GetClassNumber()}-Generic_{loading.Key}",
				InputData = DataProvider.MultistageJobInputData,
                GearboxData = PrimaryBusMockupRunDataFactory.CreateMockupGearboxData(PrimaryVehicle.Components.GearboxInputData),
                AxleGearData = PrimaryBusMockupRunDataFactory.CreateMockupAxleGearData(PrimaryVehicle.Components.AxleGearInputData)
            };
        }

        //#endregion
    }
}

