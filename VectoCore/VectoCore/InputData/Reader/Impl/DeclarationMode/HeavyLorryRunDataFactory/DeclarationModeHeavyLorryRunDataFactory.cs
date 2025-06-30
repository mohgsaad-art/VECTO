using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory
{
    public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{
		public abstract class LorryBase : AbstractDeclarationVectoRunDataFactory
		{
			public ILorryDeclarationDataAdapter DataAdapter { get; }
			public IDeclarationInputDataProvider InputDataProvider { get; }
			public IDeclarationReport Report { get; }

			protected LorryBase(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, cycleFactory, missionFilter, false, ptBuilder)
			{
				DataAdapter = declarationDataAdapter;
				InputDataProvider = dataProvider;
				Report = report;
			}

			#region Overrides of AbstractDeclarationVectoRunDataFactory


			protected override DriverData CreateDriverData(Segment segment)
			{
				return DataAdapter.CreateDriverData(segment);
			}

			#endregion

			/// <summary>
			/// Sets <see cref="VectoRunData.Loading"/>
			///  ,<see cref="VectoRunData.JobName"/>
			///  ,<see cref="VectoRunData.JobType"/>
			///  ,<see cref="VectoRunData.Mission"/>
			///  ,<see cref="VectoRunData.Report"/>
			///  ,<see cref="VectoRunData.ExecutionMode"/>
			///  ,<see cref="VectoRunData.Cycle"/>
			///  ,<see cref="VectoRunData.SimulationType"/>
			///  ,<see cref="VectoRunData.InputData"/>
			///  ,<see cref="VectoRunData.ModFileSuffix"/>
			///  ,<see cref="VectoRunData.VehicleDesignSpeed"/>
			///  ,<see cref="VectoRunData.InputDataHash"/>
			/// </summary>
			/// <param name="vehicle"></param>
			/// <param name="mission"></param>
			/// <param name="loading"></param>
			/// <param name="segment"></param>
			/// <param name="engineModes"></param>
			/// <param name="modeIdx"></param>
			/// <returns></returns>
			protected VectoRunData CreateCommonRunData(IVehicleDeclarationInputData vehicle,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				Segment segment,
				IList<IEngineModeDeclarationInputData> engineModes = null, int modeIdx = 0)
			{
				var cycle = CycleFactory.GetDeclarationCycle(mission);
				
				CheckSuperCap(vehicle);
				
				var simulationRunData = new VectoRunData
				{
					Loading = loading.Key,
					JobName = InputDataProvider.JobInputData.JobName,
					JobType = vehicle.VehicleType,
					Mission = mission,
					Report = Report,
					ExecutionMode = ExecutionMode.Declaration,
					Cycle = new DrivingCycleProxy(cycle, mission.MissionType.ToString()),
					SimulationType = SimulationType.DistanceCycle,
					InputData = InputDataProvider,
					ModFileSuffix = (engineModes?.Count > 1 ? $"_EngineMode{modeIdx}_" : "") + loading.Key,
					VehicleDesignSpeed = segment.DesignSpeed,
					InputDataHash = InputDataProvider.XMLHash,
					MaxChargingPower = InputDataProvider.JobInputData.Vehicle.MaxChargingPower,
					InMotionCharging = !vehicle.InMotionCharging.Technology.IsOneOf(IMCTechnology.None, IMCTechnology.NotApplicable),
					InMotionChargingTechnology = vehicle.InMotionCharging.Technology,

                };

				return simulationRunData;
			}

			/// <summary>
			/// Super caps are not allowed for ovc hevs or pevs
			/// </summary>
			protected void CheckSuperCap(IVehicleDeclarationInputData vehicle)
			{
				if (vehicle.VehicleType == VectoSimulationJobType.BatteryElectricVehicle || vehicle.OVC) {
					if (vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
							e.REESSPack.StorageType == REESSType.SuperCap)) {
						throw new VectoException("Super caps are not allowed for OVC-HEVs or PEVs");
					}
				}

				if (vehicle.Components.ElectricStorage?.ElectricStorageElements == null) {
					return;
				}

				var hasSuperCap = vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
					e.REESSPack.StorageType == REESSType.SuperCap);
				var hasBattery = vehicle.Components.ElectricStorage.ElectricStorageElements.Any(e =>
						e.REESSPack.StorageType == REESSType.Battery);

				if (hasSuperCap && hasBattery) {
					//Already handled by XML Schema
					throw new VectoException("Super caps AND batteries are not supported");
				}
			}

			protected override void Initialize()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				if (vehicle.ExemptedVehicle)
				{
					return;
				}
				if (!vehicle.LegislativeClass.IsOneOf(LegislativeClass.M3,
						LegislativeClass.N2, LegislativeClass.N3)) {
					throw new VectoException("Unsupported Legislative class '{0}'",
						InputDataProvider.JobInputData.Vehicle.LegislativeClass.ToString());
				}

				var tmp = DeclarationData.GetTruckSegment(vehicle);
				_segment = tmp.Segment;
				_allowVocational = tmp.AllowVocational;
			}

			protected abstract void CreateGearboxAndGearshiftData(VectoRunData runData);

			#region Implementation of IVectoRunDataFactory

			public override IEnumerable<VectoRunData> NextRun()
			{
				Initialize();
				if (Report != null)
				{
					InitializeReport();
				}

				return GetNextRun();
			}

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				return GetNextRun().First(x => x != null);
			}

			#endregion

			


			#endregion

			protected abstract bool AxleGearRequired();
		}

		public class Conventional : LorryBase
		{
			public Conventional(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of LorryBase
			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++) {
					foreach (var mission in _segment.Missions) {
						if (mission.MissionType.IsEMS() &&
							engine.RatedPowerDeclared.IsSmaller(DeclarationData.MinEnginePowerForEMS)) {
							continue;
						}

						foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true)) {
							var simulationRunData = CreateVectoRunData(mission, loading, modeIdx);
							
							yield return simulationRunData;
						}
					}
				}
			}


			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				if (!modeIdx.HasValue) {
					throw new VectoException("Engine mode has to be specified for conventional vehicle");
				}
				var engineMode = engineModes[modeIdx.Value];

				var simulationRunData = CreateCommonRunData(Vehicle, mission, loading, _segment, engineModes, modeIdx.Value);

				simulationRunData.VehicleData =
				DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);

				simulationRunData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, InputDataProvider.JobInputData.Vehicle);

				simulationRunData.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);

				simulationRunData.EngineData =
					DataAdapter.CreateEngineData(InputDataProvider.JobInputData.Vehicle, engineMode,
						mission); // _engineData.Copy(), // a copy is necessary because every run has a different correction factor!

				simulationRunData.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>();

                CreateGearboxAndGearshiftData(simulationRunData);

                simulationRunData.AngledriveData = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				simulationRunData.Aux = DataAdapter.CreateAuxiliaryData(
					Vehicle.Components.AuxiliaryInputData,
					Vehicle.Components.BusAuxiliaries, mission.MissionType,
					_segment.VehicleClass, Vehicle.Length,
					Vehicle.Components.AxleWheels.NumSteeredAxles, Vehicle.VehicleType);

				simulationRunData.Retarder = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);
				simulationRunData.DriverData = DriverData;
				simulationRunData.PTO = mission.MissionType == MissionType.MunicipalUtility
					? DataAdapter.CreatePTOCycleData(Vehicle.Components.GearboxInputData, Vehicle.Components.PTOTransmissionInputData)
					: DataAdapter.CreatePTOTransmissionData(Vehicle.Components.PTOTransmissionInputData, Vehicle.Components.GearboxInputData);

				simulationRunData.EngineData.FuelMode = modeIdx.Value;
				simulationRunData.VehicleData.VehicleClass = _segment.VehicleClass;
				simulationRunData.VehicleData.InputData = Vehicle;
				return simulationRunData;
			}

			
			#endregion


			#region Overrides of LorryBase

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						Vehicle.EngineIdleSpeed,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count
					);

                if (InputDataProvider.JobInputData.Vehicle.AxleConfiguration.AxlegearIncludedInGearbox()) {
					runData.AxleGearData = DataAdapter.CreateDummyAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData);
                } else {
					runData.AxleGearData = DataAdapter.CreateAxleGearData(InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData);
                }
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData);
			}

			protected override bool AxleGearRequired()
			{
				return true;
			}

			#endregion
		}



		public abstract class BatteryElectric : LorryBase
		{
			#region Overrides of LorryBase

			protected override bool AxleGearRequired()
			{
				return InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E4;
			}

			#endregion

			public BatteryElectric(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report,
					declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				foreach (var mission in _segment.Missions) {
					if (mission.MissionType.IsEMS() &&
						DeclarationData.GetReferencePropulsionPower(vehicle)
							.IsSmaller(DeclarationData.MinEnginePowerForEMS_PEV))
                    {
                        continue;
                    }

                    foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true))
					{
						var simulationRunData = CreateVectoRunData(mission, loading);
						simulationRunData.BatteryData.Batteries.ForEach(t => t.Item2.ChargeDepletingBattery = true);
						yield return simulationRunData;
					}
				}
			}

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var result = CreateCommonRunData(Vehicle, mission, loading, _segment);
				result.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				result.DriverData = DriverData;

				DataAdapter.CreateREESSData(
					componentsElectricStorage: Vehicle.Components.ElectricStorage,
					Vehicle.VehicleType,
					true,
					(bs) => result.BatteryData = bs,
					(sc) => result.SuperCapData = sc);
				
				result.ElectricMachinesData = DataAdapter.CreateElectricMachines(Vehicle.Components.ElectricMachines, Vehicle.ElectricMotorTorqueLimits, result.BatteryData.CalculateVoltageCenterSoc(), null);
				if (Vehicle.VehicleType == VectoSimulationJobType.IEPC_E) {
					result.ElectricMachinesData = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						result.BatteryData.CalculateVoltageCenterSoc());
				}

				result.AngledriveData = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				if (AxleGearRequired() || Vehicle.Components.AxleGearInputData != null) {
					result.AxleGearData = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				result.VehicleData =
					DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);

				result.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);

				if (Vehicle.Components.RetarderInputData != null) {
					result.Retarder = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData,
						Vehicle.ArchitectureID, Vehicle.Components.IEPC);
				}

				CreateGearboxAndGearshiftData(result);

				result.Aux = DataAdapter.CreateAuxiliaryData(Vehicle.Components.AuxiliaryInputData, null,
					mission.MissionType, _segment.VehicleClass, Vehicle.Length,
					Vehicle.Components.AxleWheels.NumSteeredAxles, Vehicle.VehicleType);


				var ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(Vehicle.Components.PTOTransmissionInputData, Vehicle.Components.GearboxInputData);

				var municipalPtoTransmissionData = DataAdapter.CreatePTOCycleData(Vehicle.Components.GearboxInputData, Vehicle.Components.PTOTransmissionInputData);

				result.PTO = mission.MissionType == MissionType.MunicipalUtility
					? municipalPtoTransmissionData
					: ptoTransmissionData;

				return result;
			}

			#region Overrides of LorryBase

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID == ArchitectureID.E2) {
					throw new ArgumentException();
				}
				runData.GearshiftParameters = new ShiftStrategyParameters()
				{
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}

			#endregion

			protected override void Initialize()
			{
				if (!InputDataProvider.JobInputData.Vehicle.LegislativeClass.IsOneOf(LegislativeClass.M3,
						LegislativeClass.N2, LegislativeClass.N3)) {
					throw new VectoException("Unsupported Legislative class '{0}'",
						InputDataProvider.JobInputData.Vehicle.LegislativeClass.ToString());
				}
				var tmp = DeclarationData.GetTruckSegment(InputDataProvider.JobInputData.Vehicle, true);
				_segment = tmp.Segment;
				_allowVocational = tmp.AllowVocational;
			}

            #endregion

        }

		public class PEV_E2 : BatteryElectric
		{
			public PEV_E2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report,
					declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }


			#region Overrides of BatteryElectric

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID != ArchitectureID.E2) {
					throw new ArgumentException(nameof(Vehicle));
				}

				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						null,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count
					);

				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData);

			}

			#endregion
		}

		public class PEV_E3 : BatteryElectric
		{
			public PEV_E3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class PEV_E4 : BatteryElectric
		{
			public PEV_E4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class PEV_E_IEPC : BatteryElectric
		{
			public PEV_E_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report,
					declarationDataAdapter, cycleFactory, missionFilter: missionFilter, ptBuilder) { }

			protected override bool AxleGearRequired()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				var iepcInput = vehicle.Components.IEPC;
				var axleGearRequired = !iepcInput.DifferentialIncluded && !iepcInput.DesignTypeWheelMotor;
				if (axleGearRequired && vehicle.Components.AxleGearInputData == null)
				{
					throw new VectoException(
						$"Axlegear reqhired for selected type of IEPC! DifferentialIncluded: {iepcInput.DifferentialIncluded}, DesignTypeWheelMotor: {iepcInput.DesignTypeWheelMotor}");
				}

				var numGearsPowermap =
					iepcInput.VoltageLevels.Select(x => Tuple.Create(x.VoltageLevel, x.PowerMap.Count)).ToArray();
				var gearCount = iepcInput.Gears.Count;
				var numGearsDrag = iepcInput.DragCurves.Count;

				if (numGearsPowermap.Any(x => x.Item2 != gearCount))
				{
					throw new VectoException(
						$"Number of gears for voltage levels does not match! PowerMaps: {numGearsPowermap.Select(x => $"{x.Item1}: {x.Item2}").Join()}; Gear count: {gearCount}");
				}

				if (numGearsDrag > 1 && numGearsDrag != gearCount)
				{
					throw new VectoException(
						$"Number of gears drag curve does not match gear count! DragCurve {numGearsDrag}; Gear count: {gearCount}");
				}

				return axleGearRequired || vehicle.Components.AxleGearInputData != null;

			}

			#region Overrides of BatteryElectric

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						null,
						GearboxType.APTN,
						Vehicle.Components.IEPC.Gears.Count
					);
				
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData, GearboxType.APTN);

			}

			#endregion
		}

		public class Exempted : LorryBase
		{
			public Exempted(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of AbstractDeclarationVectoRunDataFactory


			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;


				var simulationRunData = CreateVectoRunData(null, 
					new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 
					0);

				yield return simulationRunData;
			}

		#region Overrides of LorryBase

			protected override VectoRunData GetPowertrainConfigForReportInit()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				return CreateVectoRunData(null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(), 0);
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				throw new NotImplementedException();
			}

			#endregion

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var runData = new VectoRunData
				{
					InputData = InputDataProvider,
					Exempted = true,
					Report = Report,
					Mission = new Mission() { MissionType = MissionType.ExemptedMission },
					VehicleData = DataAdapter.CreateVehicleData(InputDataProvider.JobInputData.Vehicle, new Segment(), null, new KeyValuePair<LoadingType, Tuple<Kilogram, double?>>(LoadingType.ReferenceLoad, Tuple.Create<Kilogram, double?>(0.SI<Kilogram>(), null)), _allowVocational),
					InputDataHash = InputDataProvider.XMLHash
				};
				runData.VehicleData.InputData = Vehicle;
				return runData;
			}

			#endregion

			#region Overrides of LorryBase

			protected override bool AxleGearRequired()
			{
				throw new NotImplementedException();
			}

			#endregion

		}
		
	}
}