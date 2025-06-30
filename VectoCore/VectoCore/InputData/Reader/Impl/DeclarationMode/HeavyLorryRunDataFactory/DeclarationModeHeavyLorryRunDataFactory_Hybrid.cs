using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory
{
    public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{

		public abstract class Hybrid : LorryBase
		{
			public Hybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report,
					declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of AbstractDeclarationVectoRunDataFactory

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;

				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;

				for (var modeIdx = 0; modeIdx < engineModes.Count; modeIdx++)
				{
					foreach (var mission in _segment.Missions)
					{
						if (mission.MissionType.IsEMS() &&
							DeclarationData.GetReferencePropulsionPower(vehicle).IsSmaller(DeclarationData.MinEnginePowerForEMS))
						{
							continue;
						}

						foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true))
						{
							if (vehicle.OVC)
							{
								if (vehicle.MaxChargingPower != null && vehicle.MaxChargingPower.IsEqual(0))
								{
									throw new VectoException("MaxChargingPower has to be greater than 0 if OVC is selected");
								}

								yield return CreateVectoRunData(mission, loading, modeIdx, OvcHevMode.ChargeDepleting);
								yield return CreateVectoRunData(mission, loading, modeIdx, OvcHevMode.ChargeSustaining);
							} else {
								yield return CreateVectoRunData(mission, loading, modeIdx, OvcHevMode.ChargeSustaining);
							}
						}
					}
				}
			}

			#endregion
		}

		public class SerialHybrid : Hybrid
		{
			#region Overrides of LorryBase

			protected override bool AxleGearRequired()
			{
				return InputDataProvider.JobInputData.Vehicle.Components.AxleGearInputData != null;
			}

			#endregion

			public SerialHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report,
					declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of Hybrid

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				var engineMode = engineModes[modeIdx.Value];
				var runData = CreateCommonRunData(Vehicle, mission, loading, _segment, engineModes, modeIdx.Value);


				runData.DriverData = DriverData;
				runData.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);

				runData.EngineData = DataAdapter.CreateEngineData(Vehicle, engineMode, mission);

				DataAdapter.CreateREESSData(Vehicle.Components.ElectricStorage, Vehicle.VehicleType, Vehicle.OVC,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));

				runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(
					Vehicle.Components.ElectricMachines, Vehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateVoltageCenterSoc());

				if (Vehicle.VehicleType == VectoSimulationJobType.IEPC_S) {
					var iepcData = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						runData.BatteryData.CalculateVoltageCenterSoc());
					iepcData.ForEach(iepc => runData.ElectricMachinesData.Add(iepc));
				}

				if (AxleGearRequired()) {
					runData.AxleGearData = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				runData.Retarder = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);

				runData.Aux = DataAdapter.CreateAuxiliaryData(Vehicle.Components.AuxiliaryInputData, null, mission.MissionType,
					_segment.VehicleClass, Vehicle.Length, Vehicle.Components.AxleWheels.NumSteeredAxles,
					VectoSimulationJobType.SerialHybridVehicle);

				CreateGearboxAndGearshiftData(runData);

				runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData, runData.SuperCapData, runData.VehicleData.TotalVehicleMass, ovcMode, loading.Key, runData.VehicleData.VehicleClass, mission.MissionType);

				if (ovcMode != OvcHevMode.NotApplicable) {
					if (runData.BatteryData != null) {
						runData.BatteryData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}

					if (runData.SuperCapData != null) {
						runData.SuperCapData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}
				}

				runData.PTO = mission.MissionType == MissionType.MunicipalUtility
					? DataAdapter.CreatePTOCycleData(Vehicle.Components.GearboxInputData, Vehicle.Components.PTOTransmissionInputData)
					: DataAdapter.CreatePTOTransmissionData(Vehicle.Components.PTOTransmissionInputData, Vehicle.Components.GearboxInputData);


				if (ovcMode != OvcHevMode.NotApplicable && runData.InputData.JobInputData.Vehicle.OVC) {
					runData.ModFileSuffix += ovcMode == OvcHevMode.ChargeSustaining ? "CS" : "CD";
				}

				if (ovcMode == OvcHevMode.ChargeDepleting) {
					runData.BatteryData.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
				}

				runData.OVCMode = ovcMode;

				return runData;
			}

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				if (Vehicle.ArchitectureID.IsOneOf(ArchitectureID.S2, ArchitectureID.S_IEPC)) {
					throw new ArgumentException(nameof(Vehicle.ArchitectureID));
				}
				runData.GearshiftParameters = new ShiftStrategyParameters() {
					StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
					StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
				};
			}

            #endregion
        }

		public class ParallelHybrid : Hybrid
		{
			public ParallelHybrid(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }


			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx = null, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var engine = InputDataProvider.JobInputData.Vehicle.Components.EngineInputData;
				var engineModes = engine.EngineModes;
				if (!modeIdx.HasValue) {
					throw new VectoException("Engine mode has to be specified for parallel hybrid vehicle");
				}
				var engineMode = engineModes[modeIdx.Value];
				var runData = CreateCommonRunData(Vehicle, mission, loading, _segment, engineModes, modeIdx.Value);

				runData.DriverData = DriverData;

				runData.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);

				runData.EngineData = DataAdapter.CreateEngineData(Vehicle, engineMode, mission);
				DataAdapter.CreateREESSData(Vehicle.Components.ElectricStorage, Vehicle.VehicleType, Vehicle.OVC,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));

				if (Vehicle.Components.AxleGearInputData != null)
				{
					runData.AxleGearData = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				runData.Retarder = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.VehicleType == VectoSimulationJobType.IHPC ? ArchitectureID.P_IHPC : Vehicle.ArchitectureID, null);
				runData.Aux = DataAdapter.CreateAuxiliaryData(Vehicle.Components.AuxiliaryInputData, null, mission.MissionType,
					_segment.VehicleClass, Vehicle.Length, Vehicle.Components.AxleWheels.NumSteeredAxles,
					VectoSimulationJobType.ParallelHybridVehicle);


				CreateGearboxAndGearshiftData(runData);
				runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(
					Vehicle.Components.ElectricMachines, Vehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateVoltageCenterSoc(), runData.GearboxData.GearList);

				runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData,
						runData.SuperCapData,
						runData.VehicleData.TotalVehicleMass,
						ovcMode, loading.Key,
						runData.VehicleData.VehicleClass,
						mission.MissionType, Vehicle.BoostingLimitations, runData.GearboxData, runData.EngineData, runData.ElectricMachinesData, Vehicle.ArchitectureID);

				if (ovcMode != OvcHevMode.NotApplicable) {
					if (runData.BatteryData?.InitialSoC != null) {
						runData.BatteryData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}

					if (runData.SuperCapData?.InitialSoC != null) {
						runData.SuperCapData.InitialSoC = runData.HybridStrategyParameters.InitialSoc;
					}
				}

				if (ovcMode == OvcHevMode.ChargeDepleting) {
					runData.BatteryData.Batteries.ForEach(b => b.Item2.ChargeDepletingBattery = true);
				}

				if (ovcMode == OvcHevMode.ChargeSustaining) {
					runData.IterativeRunStrategy = new HevChargeSustainingIterativeRunStrategy();
				}

				runData.PTO = mission.MissionType == MissionType.MunicipalUtility
					? DataAdapter.CreatePTOCycleData(Vehicle.Components.GearboxInputData, Vehicle.Components.PTOTransmissionInputData)
					: DataAdapter.CreatePTOTransmissionData(Vehicle.Components.PTOTransmissionInputData, Vehicle.Components.GearboxInputData);


				if (ovcMode != OvcHevMode.NotApplicable && runData.InputData.JobInputData.Vehicle.OVC)
				{
					runData.ModFileSuffix += ovcMode == OvcHevMode.ChargeSustaining ? "CS" : "CD";
				}
				runData.OVCMode = ovcMode;
				return runData;
			}




			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						null,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count
					);
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData);
			}

			protected override bool AxleGearRequired()
			{
				return true;
			}
		}

		public class HEV_S2 : SerialHybrid
		{
			public HEV_S2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report,
					declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of SerialHybrid

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
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

		public class HEV_S3 : SerialHybrid
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_S4 : SerialHybrid
		{
			public HEV_S4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_S_IEPC : SerialHybrid
		{
			public HEV_S_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }

			#region Overrides of LorryBase

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

				return axleGearRequired;
			}

			#endregion

			#region Overrides of SerialHybrid

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				AxleGearRequired();
				return base.CreateVectoRunData(mission, loading, modeIdx, ovcMode);
			}

			#endregion

			#region Overrides of SerialHybrid

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

		public class FuelCellHybrid : BatteryElectric
		{
			public FuelCellHybrid(IDeclarationInputDataProvider dataProvider,
						 IDeclarationReport report,
						 ILorryDeclarationDataAdapter declarationDataAdapter,
						 IDeclarationCycleFactory cycleFactory,
						 IMissionFilter missionFilter,
						 IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder)
			{
			}

			public virtual VectoSimulationJobType FuelCellJobType => VectoSimulationJobType.FCHV;

			protected override IEnumerable<VectoRunData> GetNextRun()
			{
				var vehicle = InputDataProvider.JobInputData.Vehicle;
				foreach (var mission in _segment.Missions)
				{
					if (mission.MissionType.IsEMS() &&
						DeclarationData.GetReferencePropulsionPower(vehicle)
							.IsSmaller(DeclarationData.MinEnginePowerForEMS_PEV))
					{
						continue;
					}

					foreach (var loading in mission.Loadings.Where(l => MissionFilter?.Run(mission.MissionType, l.Key) ?? true))
					{
						var ovcMode = vehicle.OVC ? OvcHevMode.ChargeSustaining : OvcHevMode.NotApplicable;

						var simulationRunData = CreateVectoRunData(mission, loading, null, ovcMode);
						yield return simulationRunData;
					}
				}
			}

			protected override VectoRunData CreateVectoRunData(
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var runData = CreateCommonRunData(Vehicle, mission, loading, _segment);

				runData.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				runData.DriverData = DriverData;

				DataAdapter.CreateREESSData(
					componentsElectricStorage: Vehicle.Components.ElectricStorage,
					Vehicle.VehicleType,
                    Vehicle.OVC,
					(bs) => runData.BatteryData = bs,
					(sc) => runData.SuperCapData = sc);

                runData.ElectricMachinesData = DataAdapter.CreateElectricMachines(Vehicle.Components.ElectricMachines, Vehicle.ElectricMotorTorqueLimits, runData.BatteryData.CalculateVoltageCenterSoc(), null);
				if (Vehicle.VehicleType == VectoSimulationJobType.IEPC_E)
				{
					runData.ElectricMachinesData = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						runData.BatteryData.CalculateVoltageCenterSoc());
				}

				runData.AngledriveData = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				if (AxleGearRequired() || Vehicle.Components.AxleGearInputData != null)
				{
					runData.AxleGearData = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				runData.VehicleData =
					DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);

				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);

				if (Vehicle.Components.RetarderInputData != null)
				{
					runData.Retarder = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData,
						Vehicle.ArchitectureID, Vehicle.Components.IEPC);
				}

				CreateGearboxAndGearshiftData(runData);

				runData.Aux = DataAdapter.CreateAuxiliaryData(
					Vehicle.Components.AuxiliaryInputData,
					null,
					mission.MissionType,
					_segment.VehicleClass,
					Vehicle.Length,
					Vehicle.Components.AxleWheels.NumSteeredAxles,
					Vehicle.VehicleType);

				var ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(Vehicle.Components.PTOTransmissionInputData, Vehicle.Components.GearboxInputData);

				var municipalPtoTransmissionData = DataAdapter.CreatePTOCycleData(Vehicle.Components.GearboxInputData, Vehicle.Components.PTOTransmissionInputData);

				runData.PTO = mission.MissionType == MissionType.MunicipalUtility
					? municipalPtoTransmissionData
					: ptoTransmissionData;

				runData.OVCMode = ovcMode;
				runData.ModFileSuffix += "_pre";
				runData.IterativeRunStrategy = SetUpFuelCellIterativeRunStrategy(runData);
				runData.BatteryData.Batteries.ForEach(t => t.Item2.ChargeDepletingBattery = true);

				return runData;
			}

			// runData.IterativeRunStrategy = SetUpFuelCellIterativeRunStrategy(runData);
			private FCHEVIterativeRunStrategy SetUpFuelCellIterativeRunStrategy(VectoRunData runData)
			{
				var iterativeRunStrategy = SetUpFCHEVIterativeRunStrategy();
				var fuelCellSystemData = DataAdapter.CreateFuelCells(Vehicle.Components.FuelCellSystem).ConvertToEngineeringData();

				iterativeRunStrategy.Update = (modData, iterationRunData) =>
				{
					var fchvDataAdapter = new FCHVDeclarationDataAdapter(DataProvider.DataSource);

					/// Refer to [1] EngineeringModeVectoRunDataFactory.GetFCHV_RunData():
					/// Comment from [1]:
					///		In case the battery is modified after creating the rundata
					///		(testing, do not create new battery data).
					iterationRunData.BatteryData = fchvDataAdapter.CreateFuelCellPreProcessingBattery(
						fuelCellSystemData,
						iterationRunData.BatteryData,
						out var fcBatteries);

					runData.BatteryData.Batteries = runData.BatteryData.Batteries
						.Where(b => b.Item1 != fcBatteries.Item1)
						.ToList();

					iterationRunData.JobType = FuelCellJobType;
					iterationRunData.ModFileSuffix = string.Empty;
					iterationRunData.FuelCellSystemData = fuelCellSystemData;
					modData.PostProcessingCorrection = new FCHVPostProcessingCorrection()
					{
						FCHVElectricEnergyConsumptionSoC = FCHVPostProcessingCorrection.CalculateElectricEnergyConsumption(modData),
					};

					iterationRunData.FuelCellSystemData.FuelCellPowerMap =
						fchvDataAdapter.CreateFuelCellPowerMap(modData, iterationRunData.FuelCellSystemData, iterationRunData.BatteryData);
					iterationRunData.FuelCellSystemData.FuelCellShareMap =
						fchvDataAdapter.CreateFuelCellShareMap(fuelCellSystemData);

					/// Comment from [1]: In the real run we don't use a charge sustaining battery
					runData.BatteryData.ChargeSustainingBatterySystem = false;
					runData.ModFileSuffix += runData.Loading;
					runData.Iteration++;
				};

				return iterativeRunStrategy;
			}

			private FCHEVIterativeRunStrategy SetUpFCHEVIterativeRunStrategy()
			{
				return new FCHEVIterativeRunStrategy(
						new[]
						{
							// Pre-run, iteration 0.
							new PreRunOptions()
							{
								WriteModAndSumData = true,
//#if TRACE_FC
//								WriteModAndSumData = true,
//#else
//								WriteModAndSumData = false
//#endif
							},

							// Real run, iteration 1.
							new PreRunOptions()
							{
								WriteModAndSumData = true
							}
						});
			}

			protected override bool AxleGearRequired()
			{
				return InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.E4
					&& InputDataProvider.JobInputData.Vehicle.ArchitectureID != ArchitectureID.F4;
			}
		}

		public class HEV_F2 : FuelCellHybrid
		{
			public HEV_F2(IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter, // following are injected
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder)
				: base(
					  dataProvider,
					  report,
					  declarationDataAdapter,
					  cycleFactory,
					  missionFilter,
					  ptBuilder)
			{ }

			#region Overrides of SerialHybrid

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
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

		public class HEV_F3 : FuelCellHybrid
		{
			public HEV_F3(IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter, // following are injected
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder)
				: base(
					  dataProvider,
					  report,
					  declarationDataAdapter,
					  cycleFactory,
					  missionFilter,
					  ptBuilder) { }
		}

		public class HEV_F4 : FuelCellHybrid
		{
			public HEV_F4(IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter, // following are injected
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder)
				: base(
					  dataProvider,
					  report,
					  declarationDataAdapter,
					  cycleFactory,
					  missionFilter,
					  ptBuilder) { }
		}

		public class HEV_F_IEPC : FuelCellHybrid
		{
			public HEV_F_IEPC(
				IDeclarationInputDataProvider dataProvider,
				IDeclarationReport report,
				ILorryDeclarationDataAdapter declarationDataAdapter, // following are injected
				IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter,
				IPowertrainBuilder ptBuilder)
				: base(
					  dataProvider,
					  report,
					  declarationDataAdapter,
					  cycleFactory,
					  missionFilter,
					  ptBuilder) { }

			public override VectoSimulationJobType FuelCellJobType => VectoSimulationJobType.FCHV_IEPC;

			protected override VectoRunData CreateVectoRunData(Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
				int? modeIdx,
				OvcHevMode ovcMode = OvcHevMode.NotApplicable)
			{
				var runData = base.CreateVectoRunData(mission, loading, modeIdx, ovcMode);
				runData.JobType = VectoSimulationJobType.IEPC_E;

				return runData;
			}

			#region Overrides of LorryBase

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

				return axleGearRequired;
			}

			#endregion

			#region Overrides of SerialHybrid

			protected override void CreateGearboxAndGearshiftData(VectoRunData runData)
			{
				runData.GearshiftParameters =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearData?.AxleGear.Ratio ?? 1.0,
						null,
						GearboxType.APTN,
						Vehicle.Components.IEPC.Gears.Count
					);
				runData.GearboxData = DataAdapter.CreateGearboxData(Vehicle, runData);

			}

			#endregion
		}

		public class HEV_P1 : ParallelHybrid
		{
			public HEV_P1(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P2 : ParallelHybrid
		{
			public HEV_P2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P2_5 : ParallelHybrid
		{
			public HEV_P2_5(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P3 : ParallelHybrid
		{
			public HEV_P3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P4 : ParallelHybrid
		{
			public HEV_P4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}

		public class HEV_P_IHPC : HEV_P2
		{

			public HEV_P_IHPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				// the following parameters are injected
				ILorryDeclarationDataAdapter declarationDataAdapter, IDeclarationCycleFactory cycleFactory,
				IMissionFilter missionFilter, IPowertrainBuilder ptBuilder)
				: base(dataProvider, report, declarationDataAdapter, cycleFactory, missionFilter, ptBuilder) { }
		}
	}
}