using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory
{
	public abstract class DeclarationModeCompletedBusRunDataFactory
	{
		public abstract class CompletedBusBase : AbstractDeclarationVectoRunDataFactory
		{
			protected const string _modSuffixSpecific = "Specific";
			protected const string _modSuffixGeneric = "Generic";

			public IGenericCompletedBusDeclarationDataAdapter DataAdapterGeneric { get; }
			public ISpecificCompletedBusDeclarationDataAdapter DataAdapterSpecific { get; }
			protected IMultistageVIFInputData DataProvider { get; }

			//protected IDeclarationReport Report { get; set; }
			protected virtual IVehicleDeclarationInputData PrimaryVehicle =>
				DataProvider.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle;

			protected virtual  IVehicleDeclarationInputData CompletedVehicle => DataProvider.MultistageJobInputData.JobInputData
				.ConsolidateManufacturingStage.Vehicle;

			protected override IVehicleDeclarationInputData Vehicle => throw new NotImplementedException();

			public CompletedBusBase(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder) : base(null, report, cycleFactory, missionFilter, false, ptBuilder)
			{
				DataAdapterSpecific = dataAdapterSpecific;
				DataAdapterGeneric = dataAdapterGeneric;
				DataProvider = dataProvider;
			}

			#region Implementation of AbstractDeclarationVectoRunDataFactory

			protected override void Initialize()
			{
				if (CompletedVehicle.ExemptedVehicle || PrimaryVehicle.ExemptedVehicle) {
					return;
				}

				_segment = GetCompletedSegment();
			}

			protected override DriverData CreateDriverData(Segment segment)
			{
				CompressorDrive compressorDrive =
					PrimaryVehicle.Components.BusAuxiliaries.PneumaticSupply.CompressorDrive;
				ArchitectureID arch = PrimaryVehicle.ArchitectureID;
				VectoSimulationJobType jobType = PrimaryVehicle.VehicleType;
				return DataAdapterGeneric.CreateBusDriverData(segment, jobType, arch, compressorDrive);
			}

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				return GetNextRun().First(x => x != null);
			}


			protected abstract VectoRunData CreateVectoRunDataGeneric(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx,
				OvcHevMode ovcHevMode);

			protected abstract VectoRunData CreateVectoRunDataSpecific(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable);

			protected virtual VectoRunData CreateCommonRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, string modSuffix, OvcHevMode ovcMode)
			{
				var cycle = CycleFactory.GetDeclarationCycle(mission);

				CheckSuperCap(PrimaryVehicle);
				var simulationRunData = new VectoRunData {
					JobType = PrimaryVehicle.VehicleType,
					InputData = DataProvider.MultistageJobInputData,
					Loading = loading.Key,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					DriverData = DriverData,
					ExecutionMode = ExecutionMode.Declaration,
					JobName = DataProvider.MultistageJobInputData.JobInputData.ManufacturingStages.Last().Vehicle
						.Identifier, //?!? Jobname
					ModFileSuffix = $"_{_segment.VehicleClass.GetClassNumber()}-{modSuffix}_{loading.Key}",
					Report = Report,
					Mission = mission,
					InputDataHash = DataProvider.MultistageJobInputData.XMLHash, // right hash?!?
					SimulationType = SimulationType.DistanceCycle,
					VehicleDesignSpeed = _segment.DesignSpeed,
					MaxChargingPower = PrimaryVehicle.MaxChargingPower,
					OVCMode = ovcMode,
				};

				return simulationRunData;
			}


			protected virtual Segment GetPrimarySegment()
			{
				var primarySegment = DeclarationData.PrimaryBusSegments.Lookup(
					PrimaryVehicle.VehicleCategory, PrimaryVehicle.AxleConfiguration, PrimaryVehicle.Articulated);

				return primarySegment;
			}

			protected virtual VehicleClass GetPrimaryGroup()
			{
				var segment = GetPrimarySegment();
				return DeclarationData.PrimaryBusSegments.Lookup(hdvSuperGroup: segment.VehicleClass,
					vehicleCode: CompletedVehicle.VehicleCode.Value);
			}

			protected virtual IEnumerable<VectoRunData> CreateVectoRunData(
				Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, string fuelMode = null,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				// create specific run data
				var simulationRunData = CreateVectoRunDataSpecific(mission, loading, modeIdx, ovcMode);
				if (simulationRunData != null) {
					yield return simulationRunData;
				}

				// create generic run data
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
					primarySegment, modeIdx, ovcMode);
				simulationRunData.PrimaryResult = GetPrimaryResult(fuelMode, simulationRunData, ovcMode);

				yield return simulationRunData;
			}


			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				throw new NotImplementedException("Not applicable for completed buses");
			}

			protected virtual IResult GetPrimaryResult(string fuelMode, VectoRunData simulationRunData,
				OvcHevMode ovcHevMode)
			{
				ovcHevMode = simulationRunData.VehicleData.InputData.OVC ? ovcHevMode : OvcHevMode.NotApplicable; //Results are store with OvcHevMode NotApplicable for non OVC hevs
				var primaryResult = DataProvider.MultistageJobInputData.JobInputData.PrimaryVehicle.GetResult(
					simulationRunData.Mission.BusParameter.BusGroup, simulationRunData.Mission.MissionType, fuelMode,
					simulationRunData.VehicleData.Loading, ovcHevMode);
				
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

				return primaryResult;
			}

			protected virtual Segment GetCompletedSegment()
			{
				var segment = DeclarationData.CompletedBusSegments.Lookup(
					PrimaryVehicle.AxleConfiguration.NumAxles(), CompletedVehicle.VehicleCode, CompletedVehicle.RegisteredClass, CompletedVehicle.NumberPassengerSeatsLowerDeck,
					CompletedVehicle.Height, CompletedVehicle.LowEntry);
				if (!segment.Found) {
					throw new VectoException(
						"no segment found for vehicle configruation: vehicle category: {0}, axle configuration: {1}, articulated: {2}, vehicle code: {3}, registered class: {4}, passengersLowerDeck: {5}, height: {6}, lowEntry: {7}. completed",
						CompletedVehicle.VehicleCategory, PrimaryVehicle.AxleConfiguration,
						CompletedVehicle.Articulated, CompletedVehicle.VehicleCode, CompletedVehicle.RegisteredClass.GetLabel(), CompletedVehicle.NumberPassengerSeatsLowerDeck,
						CompletedVehicle.Height, CompletedVehicle.LowEntry);
				}

				return segment;
			}
			#endregion

			protected abstract void CreateGearboxAndGearshiftData(VectoRunData runData);
		}

		public class Conventional : CompletedBusBase
		{
			public Conventional(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific,
					dataAdapterGeneric, cycleFactory, missionFilter, ptBuilder) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var engineModes = PrimaryVehicle.Components.EngineInputData
					?.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					var fuelMode = "single fuel mode";
					if (engineModes[modeIdx].Fuels.Count > 1) {
						fuelMode = "dual fuel mode";
					}

					foreach (var mission in _segment.Missions) {
						foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {
							foreach (var run in CreateVectoRunData(mission, loading, modeIdx, fuelMode)) {
								yield return run;
							}
						}
					}
				}
			}

			protected override VectoRunData CreateVectoRunDataGeneric(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx,
				OvcHevMode ovcHevMode)
			{
				var simulationRunData = CreateCommonRunData(mission, loading, _modSuffixGeneric, ovcHevMode);
				simulationRunData.InMotionCharging = !PrimaryVehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable);
				simulationRunData.InMotionChargingTechnology = PrimaryVehicle.InMotionCharging.Technology;

				var primaryBusAuxiliaries = PrimaryVehicle.Components.BusAuxiliaries;

                simulationRunData.VehicleData =
                    DataAdapterGeneric.CreateVehicleData(PrimaryVehicle, primarySegment, mission, loading, false);
                simulationRunData.AirdragData = DataAdapterGeneric.CreateAirdragData(PrimaryVehicle, mission, primarySegment, ovcHevMode);
                simulationRunData.EngineData =
                    DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx.Value, mission);
                simulationRunData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
                simulationRunData.AxleGearData =
                    DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
                simulationRunData.AngledriveData =
                    DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
                simulationRunData.Aux = DataAdapterGeneric.CreateAuxiliaryData(
                    PrimaryVehicle.Components.AuxiliaryInputData, primaryBusAuxiliaries, mission.MissionType,
                    primarySegment.VehicleClass, mission.BusParameter.VehicleLength,
                    PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);

				simulationRunData.EngineData.FuelMode = 0;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
				simulationRunData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, simulationRunData);
				var gbx = simulationRunData.GearboxData;
				simulationRunData.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData((simulationRunData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
															(simulationRunData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						PrimaryVehicle.EngineIdleSpeed, gbx.Type, gbx.Gears.Count);
				simulationRunData.Retarder =
					DataAdapterGeneric.CreateGenericRetarderData(PrimaryVehicle.Components.RetarderInputData, simulationRunData);
				simulationRunData.BusAuxiliaries =
					DataAdapterGeneric.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, simulationRunData);
				return simulationRunData;
			}

			protected override VectoRunData CreateVectoRunDataSpecific(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var simulationRunData = CreateCommonRunData(mission, loading, _modSuffixSpecific, ovcMode);
				simulationRunData.InMotionCharging = !CompletedVehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable);
				simulationRunData.InMotionChargingTechnology = CompletedVehicle.InMotionCharging.Technology;


				simulationRunData.VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle,
					CompletedVehicle, _segment, mission, loading);
				simulationRunData.AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission, _segment, ovcMode);
				simulationRunData.EngineData =
					DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx.Value, mission);
				simulationRunData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
				simulationRunData.AxleGearData =
					DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
				simulationRunData.AngledriveData =
					DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
				simulationRunData.Aux = DataAdapterSpecific.CreateAuxiliaryData(
					PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					CompletedVehicle.Length, PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);
				simulationRunData.EngineData.FuelMode = 0;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;

				CreateGearboxAndGearshiftData(simulationRunData);
				simulationRunData.Retarder =
					DataAdapterGeneric.CreateGenericRetarderData(PrimaryVehicle.Components.RetarderInputData, simulationRunData);
				simulationRunData.BusAuxiliaries =
					DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, simulationRunData);

				return simulationRunData;
			}


			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, runData);
				GearboxData gbx = runData.GearboxData;
				runData.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData((runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
															(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						PrimaryVehicle.EngineIdleSpeed, gbx.Type, gbx.Gears.Count);

			}
		}
		#region Hybrid
		public abstract class Hybrid : CompletedBusBase
		{
			protected Hybrid(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report,
					dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter, ptBuilder) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var engineModes = PrimaryVehicle.Components.EngineInputData
					?.EngineModes;
				var ovc = PrimaryVehicle.OVC;
				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					var fuelMode = "single fuel mode";
					if (engineModes[modeIdx].Fuels.Count > 1) {
						fuelMode = "dual fuel mode";
					}

					foreach (var mission in _segment.Missions) {
						foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {
							// TODO: charge sustaining / charge depleting
							if (ovc) {
								if (PrimaryVehicle.MaxChargingPower == null) {
									throw new VectoException(
										$"{nameof(PrimaryVehicle.MaxChargingPower)} must be provided for OVC HEVs");
								}
								foreach (var run in CreateVectoRunData(mission, loading, modeIdx, fuelMode,
											OvcHevMode.ChargeSustaining)) {
									yield return run;
								}
								foreach (var run in CreateVectoRunData(mission, loading, modeIdx, fuelMode,
											OvcHevMode.ChargeDepleting)) {
									yield return run;
								}
							}
							else
							{
								foreach (var run in CreateVectoRunData(mission, loading, modeIdx, fuelMode, OvcHevMode.ChargeSustaining)) {
									yield return run;
								}
							}
						}
					}
				}
			}

			#region Overrides of CompletedBusBase

			protected override VectoRunData CreateVectoRunDataGeneric(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx,
				OvcHevMode ovcHevMode)
			{
				throw new VectoException("Dummy implementation");
			}

			protected override VectoRunData CreateVectoRunDataSpecific(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				throw new VectoException("Dummy implementation");
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				throw new VectoException("Dummy implementation");
			}

			#endregion

			protected void SetOvcModeProperties(OvcHevMode ovcHevMode, VectoRunData rd)
			{
				if (ovcHevMode != OvcHevMode.NotApplicable) {
					if (rd.BatteryData != null) {
						rd.BatteryData.InitialSoC = rd.HybridStrategyParameters.InitialSoc;
					}

					if (rd.SuperCapData != null) {
						rd.SuperCapData.InitialSoC = rd.HybridStrategyParameters.InitialSoc;
					}
				}

				if (ovcHevMode != OvcHevMode.NotApplicable && PrimaryVehicle.OVC) {
					rd.ModFileSuffix += ovcHevMode == OvcHevMode.ChargeSustaining ? "CS" : "CD";
				}

				if (ovcHevMode == OvcHevMode.ChargeDepleting) {
					rd.BatteryData.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
				}
			}
		}

		#region SerialHybrid
		public abstract class SerialHybrid : Hybrid
		{


			protected SerialHybrid(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of Hybrid

			protected override VectoRunData CreateVectoRunDataGeneric(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx,
				OvcHevMode ovcHevMode)
			{
				var rd = CreateCommonRunData(mission, loading, _modSuffixGeneric, ovcHevMode);
				rd.InMotionCharging = !PrimaryVehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable);
				rd.InMotionChargingTechnology = PrimaryVehicle.InMotionCharging.Technology;


				DataAdapterGeneric.CreateREESSData(
					componentsElectricStorage: PrimaryVehicle.Components.ElectricStorage,
					PrimaryVehicle.VehicleType,
					true,
					(bs) => rd.BatteryData = bs,
					(sc) => rd.SuperCapData = sc);

				rd.ElectricMachinesData = DataAdapterGeneric.CreateElectricMachines(PrimaryVehicle.Components.ElectricMachines,
					PrimaryVehicle.ElectricMotorTorqueLimits, rd.BatteryData.CalculateVoltageCenterSoc(), null);


				if (PrimaryVehicle.VehicleType == VectoSimulationJobType.IEPC_S)
				{
					var iepcData = DataAdapterGeneric.CreateIEPCElectricMachines(PrimaryVehicle.Components.IEPC,
						rd.BatteryData.CalculateVoltageCenterSoc());
					iepcData.ForEach(iepc => rd.ElectricMachinesData.Add(iepc));
				}

				var primaryBusAuxiliaries = PrimaryVehicle.Components.BusAuxiliaries;

                rd.VehicleData =
                    DataAdapterGeneric.CreateVehicleData(PrimaryVehicle, primarySegment, mission, loading, false);
                rd.AirdragData = DataAdapterGeneric.CreateAirdragData(PrimaryVehicle, mission, primarySegment, ovcHevMode);
                rd.EngineData =
                    DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx.Value, mission);
                //rd.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();
                rd.AxleGearData =
                    DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
                rd.AngledriveData =
                    DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
                rd.Aux = DataAdapterGeneric.CreateAuxiliaryData(
                    PrimaryVehicle.Components.AuxiliaryInputData, primaryBusAuxiliaries, mission.MissionType,
                    primarySegment.VehicleClass, mission.BusParameter.VehicleLength,
                    PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);
				rd.EngineData.FuelMode = 0;
				rd.VehicleData.VehicleClass = _segment.VehicleClass;
				
				CreateGearboxAndGearshiftData(rd);
				rd.Retarder =
					DataAdapterGeneric.CreateGenericRetarderData(PrimaryVehicle.Components.RetarderInputData, rd);
				rd.BusAuxiliaries =
					DataAdapterGeneric.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, rd);

				rd.HybridStrategyParameters =
					DataAdapterGeneric.CreateHybridStrategy(rd.BatteryData, rd.SuperCapData, rd.VehicleData.TotalVehicleMass,
						ovcHevMode, loading.Key, rd.VehicleData.VehicleClass, mission.MissionType);

				SetOvcModeProperties(ovcHevMode, rd);

				return rd;
			}

			protected override VectoRunData CreateVectoRunDataSpecific(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var rd = CreateCommonRunData(mission, loading, _modSuffixSpecific, ovcMode);
				rd.InMotionCharging = !CompletedVehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable);
				rd.InMotionChargingTechnology = CompletedVehicle.InMotionCharging.Technology;


				DataAdapterGeneric.CreateREESSData(
					componentsElectricStorage: PrimaryVehicle.Components.ElectricStorage,
					PrimaryVehicle.VehicleType,
					true,
					(bs) => rd.BatteryData = bs,
					(sc) => rd.SuperCapData = sc);


				
				rd.ElectricMachinesData = DataAdapterGeneric.CreateElectricMachines(PrimaryVehicle.Components.ElectricMachines,
						PrimaryVehicle.ElectricMotorTorqueLimits, rd.BatteryData.CalculateVoltageCenterSoc(), null);
				

				if (PrimaryVehicle.VehicleType == VectoSimulationJobType.IEPC_S)
				{
					var iepcData = DataAdapterGeneric.CreateIEPCElectricMachines(PrimaryVehicle.Components.IEPC,
						rd.BatteryData.CalculateVoltageCenterSoc());
					iepcData.ForEach(iepc => rd.ElectricMachinesData.Add(iepc));
				}

				rd.VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle,
					CompletedVehicle, _segment, mission, loading);
				rd.AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission, _segment, ovcMode);
				rd.EngineData =
					DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx.Value, mission);
				rd.HybridStrategyParameters = DataAdapterGeneric.CreateHybridStrategy(
					rd.BatteryData,
					rd.SuperCapData,
					rd.VehicleData.GrossVehicleMass,
					ovcMode,
					rd.Loading,
					rd.VehicleData.VehicleClass,
					rd.Mission.MissionType);

				rd.AxleGearData =
					DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
				rd.AngledriveData =
					DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
				rd.Aux = DataAdapterSpecific.CreateAuxiliaryData(
					PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					CompletedVehicle.Length, PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);
				rd.EngineData.FuelMode = 0;
				rd.VehicleData.VehicleClass = _segment.VehicleClass;


				CreateGearboxAndGearshiftData(rd);
				rd.HybridStrategyParameters = DataAdapterGeneric.CreateHybridStrategy(
					rd.BatteryData,
					rd.SuperCapData,
					rd.VehicleData.GrossVehicleMass,
					ovcMode,
					rd.Loading,
					rd.VehicleData.VehicleClass,
					rd.Mission.MissionType);
				rd.Retarder = DataAdapterGeneric.CreateGenericRetarderData(PrimaryVehicle.Components.RetarderInputData, rd);
				rd.BusAuxiliaries =
					DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle,
						rd);

				SetOvcModeProperties(ovcMode, rd);

				return rd;
			}


			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (PrimaryVehicle.ArchitectureID.IsOneOf(ArchitectureID.S2, ArchitectureID.S_IEPC))
				{
					throw new ArgumentException(nameof(PrimaryVehicle.ArchitectureID));
				}
				runData.GearshiftParameters = new ShiftStrategyParameters()
				{
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}

			#endregion
		}

		public class HEV_S2 : SerialHybrid
		{
			public HEV_S2(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of SerialHybrid

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (PrimaryVehicle.ArchitectureID != ArchitectureID.S2)
				{
					throw new ArgumentException(nameof(PrimaryVehicle));
				}

				var gbxInput = PrimaryVehicle.Components.GearboxInputData;
				runData.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData((runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
															(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null, gbxInput.Type, gbxInput.Gears.Count);

				runData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, runData);
			}

			#endregion
		}
	   
		public class HEV_S3 : SerialHybrid
		{
			public HEV_S3(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric,
					cycleFactory, missionFilter, ptBuilder) { }
		}
		
		public class HEV_S4 : SerialHybrid
		{
			public HEV_S4(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }
		}
		
		public class HEV_S_IEPC : SerialHybrid
		{
			public HEV_S_IEPC(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }

			#region Overrides of SerialHybrid

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						null,
						GearboxType.APTN,
						PrimaryVehicle.Components.IEPC.Gears.Count
					);
				
				runData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, runData, GearboxType.APTN);

			}

			#endregion
		}

		#endregion SerialHybrid

		#region ParallelHybrid
		public abstract class ParallelHybrid : Hybrid
		{
			protected ParallelHybrid(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }

			#region Overrides of Hybrid

			protected override VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx,
				OvcHevMode ovcHevMode)
			{
				var rd = CreateCommonRunData(mission, loading, _modSuffixGeneric, ovcHevMode);
				rd.InMotionCharging = !PrimaryVehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable);
				rd.InMotionChargingTechnology = PrimaryVehicle.InMotionCharging.Technology;

				DataAdapterGeneric.CreateREESSData(
					componentsElectricStorage: PrimaryVehicle.Components.ElectricStorage,
					PrimaryVehicle.VehicleType,
					true,
					(bs) => rd.BatteryData = bs,
					(sc) => rd.SuperCapData = sc);

				var averageVoltage = rd.BatteryData != null
					? rd.BatteryData.CalculateVoltageCenterSoc()
					: (rd.SuperCapData.MaxVoltage - rd.SuperCapData.MinVoltage) / 2.0;

				rd.ElectricMachinesData = DataAdapterGeneric.CreateElectricMachines(PrimaryVehicle.Components.ElectricMachines,
					PrimaryVehicle.ElectricMotorTorqueLimits, averageVoltage, null);
			


				var primaryBusAuxiliaries = PrimaryVehicle.Components.BusAuxiliaries;

				rd.VehicleData =
					DataAdapterGeneric.CreateVehicleData(PrimaryVehicle, primarySegment, mission, loading, false);
				rd.AirdragData = DataAdapterGeneric.CreateAirdragData(PrimaryVehicle, mission, primarySegment, ovcHevMode);
				rd.EngineData =
					DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx.Value, mission);

				rd.AxleGearData =
					DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
				rd.AngledriveData =
					DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
				rd.Aux = DataAdapterGeneric.CreateAuxiliaryData(
					PrimaryVehicle.Components.AuxiliaryInputData, primaryBusAuxiliaries, mission.MissionType,
					primarySegment.VehicleClass, mission.BusParameter.VehicleLength,
					PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);
				rd.EngineData.FuelMode = 0;
				rd.VehicleData.VehicleClass = _segment.VehicleClass;
				
				rd.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, rd);
				var gbx = rd.GearboxData;
				rd.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData((rd.AxleGearData?.AxleGear.Ratio ?? 1.0) *
															(rd.AngledriveData?.Angledrive.Ratio ?? 1.0),
						PrimaryVehicle.EngineIdleSpeed, gbx.Type, gbx.Gears.Count);
				rd.Retarder =
					DataAdapterGeneric.CreateGenericRetarderData(PrimaryVehicle.Components.RetarderInputData, rd);
				rd.BusAuxiliaries =
					DataAdapterGeneric.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle, rd);

				rd.HybridStrategyParameters = DataAdapterGeneric.CreateHybridStrategy(
					rd.BatteryData,
					rd.SuperCapData,
					rd.VehicleData.GrossVehicleMass,
					ovcHevMode,
					rd.Loading,
					GetPrimaryGroup(),//DeclarationData.PrimaryBusSegments.Lookup(GetPrimarySegment().VehicleClass, CompletedVehicle.LowEntry, CompletedVehicle.VehicleCode)
					rd.Mission.MissionType,
					PrimaryVehicle.BoostingLimitations,
					rd.GearboxData,
					rd.EngineData,
					rd.ElectricMachinesData,
					PrimaryVehicle.ArchitectureID
				);
				SetOvcModeProperties(ovcHevMode, rd);
				if (ovcHevMode == OvcHevMode.ChargeSustaining) {
					rd.IterativeRunStrategy = new HevChargeSustainingIterativeRunStrategy();
				}
				return rd;
			}

			protected override VectoRunData CreateVectoRunDataSpecific(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var rd = CreateCommonRunData(mission, loading, _modSuffixSpecific, ovcMode);
				rd.InMotionCharging = !CompletedVehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable);
				rd.InMotionChargingTechnology = CompletedVehicle.InMotionCharging.Technology;

				DataAdapterGeneric.CreateREESSData(
					componentsElectricStorage: PrimaryVehicle.Components.ElectricStorage,
					PrimaryVehicle.VehicleType,
					true,
					(bs) => rd.BatteryData = bs,
					(sc) => rd.SuperCapData = sc);

				var averageVoltage = rd.BatteryData != null
					? rd.BatteryData.CalculateVoltageCenterSoc()
					: (rd.SuperCapData.MaxVoltage - rd.SuperCapData.MinVoltage) / 2.0;

				rd.ElectricMachinesData = DataAdapterGeneric.CreateElectricMachines(PrimaryVehicle.Components.ElectricMachines,
					PrimaryVehicle.ElectricMotorTorqueLimits, averageVoltage, null);

				rd.VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle,
					CompletedVehicle, _segment, mission, loading);
				rd.AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission, _segment, ovcMode);
				rd.EngineData =
					DataAdapterGeneric.CreateEngineData(PrimaryVehicle, modeIdx.Value, mission);

				rd.AxleGearData =
					DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
				rd.AngledriveData =
					DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
				rd.Aux = DataAdapterSpecific.CreateAuxiliaryData(
					PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					CompletedVehicle.Length, PrimaryVehicle.Components.AxleWheels.NumSteeredAxles, PrimaryVehicle.VehicleType);
				rd.EngineData.FuelMode = 0;
				rd.VehicleData.VehicleClass = _segment.VehicleClass;
				
				CreateGearboxAndGearshiftData(rd);
				rd.HybridStrategyParameters = DataAdapterGeneric.CreateHybridStrategy(
					rd.BatteryData,
					rd.SuperCapData,
					rd.VehicleData.GrossVehicleMass,
					ovcMode,
					rd.Loading,
					GetPrimaryGroup(),//DeclarationData.PrimaryBusSegments.Lookup(GetPrimarySegment().VehicleClass, CompletedVehicle.LowEntry, CompletedVehicle.VehicleCode)
					rd.Mission.MissionType,
					PrimaryVehicle.BoostingLimitations,
					rd.GearboxData,
					rd.EngineData,
					rd.ElectricMachinesData,
					PrimaryVehicle.ArchitectureID
					);
				rd.Retarder = DataAdapterGeneric.CreateGenericRetarderData(PrimaryVehicle.Components.RetarderInputData, rd);
				rd.BusAuxiliaries =
					DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle,
						rd);

				SetOvcModeProperties(ovcMode, rd);
				if (ovcMode == OvcHevMode.ChargeSustaining) {
					rd.IterativeRunStrategy = new HevChargeSustainingIterativeRunStrategy();
				}
                return rd;
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, runData);
				GearboxData gbx = runData.GearboxData;
				runData.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData((runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
															(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						PrimaryVehicle.EngineIdleSpeed, gbx.Type, gbx.Gears.Count);
			}

			#endregion
		}

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }
		}
		
		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }
		}
		
		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }
		}
		
		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }
		}
		
		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }
		}

		public class HEV_P_IHPC : HEV_P2
		{
			public HEV_P_IHPC(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }
		}

		#endregion ParallelHybrid
		#endregion Hybrid


		#region BatteryElectric
		public abstract class BatteryElectric : CompletedBusBase
		{
			public BatteryElectric(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report,
					dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter, ptBuilder) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				foreach (var mission in _segment.Missions) {
					foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {
						foreach (var run in CreateVectoRunData(mission, loading)) {
							run.BatteryData.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
							yield return run;
						}
					}
				}
			}

			protected override VectoRunData CreateVectoRunDataGeneric(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx,
				OvcHevMode ovcHevMode)
			{
				var result = CreateCommonRunData(mission, loading, _modSuffixGeneric, ovcHevMode);
				result.InMotionCharging = !PrimaryVehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable);
				result.InMotionChargingTechnology = PrimaryVehicle.InMotionCharging.Technology;

				DataAdapterGeneric.CreateREESSData(
					componentsElectricStorage: PrimaryVehicle.Components.ElectricStorage,
					PrimaryVehicle.VehicleType,
					true,
					(bs) => result.BatteryData = bs,
					(sc) => result.SuperCapData = sc);

                if (PrimaryVehicle.VehicleType == VectoSimulationJobType.IEPC_E)
                {
                    result.ElectricMachinesData = DataAdapterGeneric.CreateIEPCElectricMachines(PrimaryVehicle.Components.IEPC,
                        result.BatteryData.CalculateVoltageCenterSoc());
                }
                else
                {
                    result.ElectricMachinesData = DataAdapterGeneric.CreateElectricMachines(PrimaryVehicle.Components.ElectricMachines,
                        PrimaryVehicle.ElectricMotorTorqueLimits, result.BatteryData.CalculateVoltageCenterSoc(), null);
                }

				result.VehicleData =
					DataAdapterGeneric.CreateVehicleData(PrimaryVehicle, _segment, mission, loading, _allowVocational);
				//result.VehicleData = DataAdapterGeneric.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segment,
				 //   mission, loading);
				result.AirdragData =
					DataAdapterGeneric.CreateAirdragData(PrimaryVehicle, mission, _segment, ovcHevMode);
				if (AxleGearRequired() || PrimaryVehicle.Components.AxleGearInputData != null)
				{
					result.AxleGearData =
						DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
				}

				result.AngledriveData =
					DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
				result.Aux = DataAdapterGeneric.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					CompletedVehicle.Length, PrimaryVehicle.Components.AxleWheels.NumSteeredAxles,
					PrimaryVehicle.VehicleType);
				result.MaxChargingPower = PrimaryVehicle.MaxChargingPower;

				//result.EngineData.FuelMode = 0;
				result.VehicleData.VehicleClass = _segment.VehicleClass;

				CreateGearboxAndGearshiftData(result);
				result.Retarder = DataAdapterGeneric.CreateGenericRetarderData(PrimaryVehicle.Components.RetarderInputData, result);
				result.BusAuxiliaries =
					DataAdapterGeneric.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle,
						result);

				return result;
			}

			protected override VectoRunData CreateVectoRunDataSpecific(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var result = CreateCommonRunData(mission, loading, _modSuffixSpecific, ovcMode);
				result.InMotionCharging = !CompletedVehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable);
				result.InMotionChargingTechnology = CompletedVehicle.InMotionCharging.Technology;


				DataAdapterGeneric.CreateREESSData(
					componentsElectricStorage: PrimaryVehicle.Components.ElectricStorage,
					PrimaryVehicle.VehicleType,
					true,
					(bs) => result.BatteryData = bs,
					(sc) => result.SuperCapData = sc);

				if (PrimaryVehicle.VehicleType == VectoSimulationJobType.IEPC_E) {
					result.ElectricMachinesData = DataAdapterGeneric.CreateIEPCElectricMachines(PrimaryVehicle.Components.IEPC,
						result.BatteryData.CalculateVoltageCenterSoc());
				} else {
					result.ElectricMachinesData = DataAdapterGeneric.CreateElectricMachines(PrimaryVehicle.Components.ElectricMachines,
						PrimaryVehicle.ElectricMotorTorqueLimits, result.BatteryData.CalculateVoltageCenterSoc(), null);
				}

				result.VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, _segment,
					mission, loading);
				result.AirdragData = DataAdapterSpecific.CreateAirdragData(CompletedVehicle, mission, _segment, ovcMode);
				if (AxleGearRequired() || PrimaryVehicle.Components.AxleGearInputData != null) {
					result.AxleGearData =
						DataAdapterGeneric.CreateAxleGearData(PrimaryVehicle.Components.AxleGearInputData);
				}

				result.AngledriveData =
					DataAdapterGeneric.CreateAngledriveData(PrimaryVehicle.Components.AngledriveInputData);
				result.Aux = DataAdapterSpecific.CreateAuxiliaryData(PrimaryVehicle.Components.AuxiliaryInputData,
					PrimaryVehicle.Components.BusAuxiliaries, mission.MissionType, _segment.VehicleClass,
					CompletedVehicle.Length, PrimaryVehicle.Components.AxleWheels.NumSteeredAxles,
					PrimaryVehicle.VehicleType);
				result.MaxChargingPower = PrimaryVehicle.MaxChargingPower;
				   
				//result.EngineData.FuelMode = 0;
				result.VehicleData.VehicleClass = _segment.VehicleClass;

				CreateGearboxAndGearshiftData(result);
				result.Retarder = DataAdapterGeneric.CreateGenericRetarderData(PrimaryVehicle.Components.RetarderInputData, result);
				result.BusAuxiliaries =
					DataAdapterSpecific.CreateBusAuxiliariesData(mission, PrimaryVehicle, CompletedVehicle,
						result);
				
				return result;
			}

			
			protected virtual bool AxleGearRequired()
			{
				var req = PrimaryVehicle.ArchitectureID != ArchitectureID.E4;
				if (req && PrimaryVehicle.Components.AxleGearInputData == null) {
					throw new VectoException("Axlegear required");
				}
				return req;
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (PrimaryVehicle.ArchitectureID == ArchitectureID.E2) {
					throw new ArgumentException();
				}
				runData.GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}
		}
	
		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (PrimaryVehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(PrimaryVehicle));
				}

				var gbxInput = PrimaryVehicle.Components.GearboxInputData;
				runData.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData((runData.AxleGearData?.AxleGear.Ratio ?? 1.0) *
															(runData.AngledriveData?.Angledrive.Ratio ?? 1.0),
						null, gbxInput.Type, gbxInput.Gears.Count);

				
				runData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, runData);
				
			}
		}
		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }
		}
		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }
		}
		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }


			#region Overrides of BatteryElectric

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapterGeneric.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						null,
						GearboxType.APTN,
						PrimaryVehicle.Components.IEPC.Gears.Count
					);
				
				runData.GearboxData = DataAdapterGeneric.CreateGearboxData(PrimaryVehicle, runData, GearboxType.APTN);
			}

			#endregion

			protected override bool AxleGearRequired()
			{
				var vehicle = PrimaryVehicle;
				var iepcInput = vehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
				if (axleGearRequired && vehicle.Components.AxleGearInputData == null)
				{
					throw new VectoException(
						$"Axlegear required for selected type of IEPC! DifferentialIncluded: {iepcInput.DifferentialIncluded}, DesignTypeWheelMotor: {iepcInput.DesignTypeWheelMotor}");
				}

				//var numGearsPowermap =
				//	iepcInput.VoltageLevels.Select(x => Tuple.Create(x.VoltageLevel, x.PowerMap.Count)).ToArray();
				//var gearCount = iepcInput.Gears.Count;
				//var numGearsDrag = iepcInput.DragCurves.Count;

				//if (numGearsPowermap.Any(x => x.Item2 != gearCount))
				//{
				//	throw new VectoException(
				//		$"Number of gears for voltage levels does not match! PowerMaps: {numGearsPowermap.Select(x => $"{x.Item1}: {x.Item2}").Join()}; Gear count: {gearCount}");
				//}

				//if (numGearsDrag > 1 && numGearsDrag != gearCount)
				//{
				//	throw new VectoException(
				//		$"Number of gears drag curve does not match gear count! DragCurve {numGearsDrag}; Gear count: {gearCount}");
				//}

				return axleGearRequired || vehicle.Components.AxleGearInputData != null;

			}
		}
		#endregion BatteryElectric

		public class Exempted : CompletedBusBase
		{
			public Exempted(IMultistageVIFInputData dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ISpecificCompletedBusDeclarationDataAdapter dataAdapterSpecific,
				IGenericCompletedBusDeclarationDataAdapter dataAdapterGeneric, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, dataAdapterSpecific, dataAdapterGeneric, cycleFactory, missionFilter,
					ptBuilder) { }

			protected override IEnumerable<VectoRunData> GetNextRun()
			{

				return CreateVectoRunData(null,
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(),
					0);

			}

			protected override IEnumerable <VectoRunData> CreateVectoRunData(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, string fuelMode = null,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				yield return new VectoRunData() {
					Exempted = true,
					VehicleData = DataAdapterSpecific.CreateVehicleData(PrimaryVehicle, CompletedVehicle, new Segment(), null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>()),
					Report = Report,
					Mission = new Mission() {
						MissionType = MissionType.ExemptedMission
					},
					InputData = DataProvider.MultistageJobInputData
				};
			}

			protected override VectoRunData CreateVectoRunDataGeneric(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int? modeIdx,
				OvcHevMode ovcHevMode)
			{
				throw new NotImplementedException();
			}

			protected override VectoRunData CreateVectoRunDataSpecific(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				throw new NotImplementedException();
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				throw new NotImplementedException();
			}
		}
	}
}