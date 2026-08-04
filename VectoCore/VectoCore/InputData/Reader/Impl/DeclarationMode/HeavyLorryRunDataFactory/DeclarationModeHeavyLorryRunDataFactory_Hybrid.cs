using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Declaration.IterativeRunStrategies;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

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

				if (ovcMode == OvcHevMode.ChargeDepleting) {
					runData.BatteryOnlyHybridMode = Vehicle.BatteryOnlyMode;
				}

                runData.DriverData = DriverData;
				runData.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);

				runData.EngineData = DataAdapter.CreateEngineData(Vehicle, engineMode, mission);

				DataAdapter.CreateREESSData(Vehicle.Components.ElectricStorage, Vehicle.VehicleType, Vehicle.OVC, Vehicle.BatteryOnlyMode,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));

				runData.ElectricMachinesSinglePwt = DataAdapter.CreateElectricMachines(
					Vehicle.Components.ElectricMachines, Vehicle.ElectricMotorTorqueLimits,
					runData.BatteryData.CalculateVoltageCenterSoc());

				if (Vehicle.VehicleType == VectoSimulationJobType.IEPC_S) {
					var iepcData = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						runData.BatteryData.CalculateVoltageCenterSoc());
					iepcData.ForEach(iepc => runData.ElectricMachinesSinglePwt.Add(iepc));
				}

				if (AxleGearRequired()) {
					runData.AxleGearSinglePwt = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				runData.RetarderSinglePwt = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.ArchitectureID, Vehicle.Components.IEPC);

				runData.Aux = DataAdapter.CreateAuxiliaryData(Vehicle.Components.AuxiliaryInputData, null, mission.MissionType,
					_segment.VehicleClass, Vehicle.Length, Vehicle.Components.AxleWheels.NumSteeredAxles,
					VectoSimulationJobType.SerialHybridVehicle, runData.BatteryOnlyHybridMode);

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

				runData.PTOSinglePwt = mission.MissionType == MissionType.MunicipalUtility
					? DataAdapter.CreatePTOCycleData(Vehicle.Components.GearboxInputData, Vehicle.Components.PTOTransmissionInputData, runData.BatteryOnlyHybridMode)
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
				runData.GearshiftParametersSinglePwt = new ShiftStrategyParameters() {
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

				if (ovcMode == OvcHevMode.ChargeDepleting) {
					runData.BatteryOnlyHybridMode = Vehicle.BatteryOnlyMode;
				}

                runData.DriverData = DriverData;

				runData.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				runData.VehicleData = DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);
				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);

				runData.EngineData = DataAdapter.CreateEngineData(Vehicle, engineMode, mission);
				DataAdapter.CreateREESSData(Vehicle.Components.ElectricStorage, Vehicle.VehicleType, Vehicle.OVC, Vehicle.BatteryOnlyMode,
					((batteryData) => runData.BatteryData = batteryData),
					((sCdata => runData.SuperCapData = sCdata)));

				if (Vehicle.Components.AxleGearInputData != null)
				{
					runData.AxleGearSinglePwt = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				runData.RetarderSinglePwt = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData, Vehicle.VehicleType == VectoSimulationJobType.IHPC ? ArchitectureID.P_IHPC : Vehicle.ArchitectureID, null);
				runData.Aux = DataAdapter.CreateAuxiliaryData(Vehicle.Components.AuxiliaryInputData, null, mission.MissionType,
					_segment.VehicleClass, Vehicle.Length, Vehicle.Components.AxleWheels.NumSteeredAxles,
					VectoSimulationJobType.ParallelHybridVehicle, runData.BatteryOnlyHybridMode);

				if (runData.BatteryOnlyHybridMode && runData.Aux.Any(x => x.ID != Constants.Auxiliaries.IDs.Fan && !x.ConnectToREESS)) {
					throw new VectoException(
						"Vehicles with a battery dominant mode are required to have electrically powered auxiliaries");
				}

			    var gearboxType = InputDataProvider.JobInputData.Vehicle.Components.GetGearboxTypes().FirstOrDefault();

				var gearlist = gearboxType == GearboxType.IHPC
					? new GearList(InputDataProvider.JobInputData.Vehicle.Components.GearboxInputData.Gears
						.Select(x => new GearshiftPosition((uint)x.Gear)).ToArray())
					: null;
				
				runData.ElectricMachinesSinglePwt = DataAdapter.CreateElectricMachines(
						Vehicle.Components.ElectricMachines,
						Vehicle.ElectricMotorTorqueLimits,
						runData.BatteryData.CalculateVoltageCenterSoc(),
						gearlist);

                CreateGearboxAndGearshiftData(runData);
                

                runData.HybridStrategyParameters =
					DataAdapter.CreateHybridStrategy(runData.BatteryData,
						runData.SuperCapData,
						runData.VehicleData.TotalVehicleMass,
						ovcMode, loading.Key,
						runData.VehicleData.VehicleClass,
						mission.MissionType, Vehicle.BoostingLimitations, runData.GearboxSinglePwt, runData.EngineData, runData.ElectricMachinesSinglePwt, Vehicle.ArchitectureID);

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

				runData.PTOSinglePwt = mission.MissionType == MissionType.MunicipalUtility
					? DataAdapter.CreatePTOCycleData(Vehicle.Components.GearboxInputData, Vehicle.Components.PTOTransmissionInputData, Vehicle.BatteryOnlyMode)
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
				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						null,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count
					);
				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData);
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
				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						null,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count
					);
				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData);
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
				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0,
						null,
						GearboxType.APTN,
						Vehicle.Components.IEPC.Gears.Count
					);

				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData, GearboxType.APTN);

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
						var ovcMode = vehicle.OVC ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;

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

				if (ovcMode == OvcHevMode.ChargeDepleting) {
					runData.BatteryOnlyHybridMode = Vehicle.BatteryOnlyMode;
				}

                runData.AirdragData =
					DataAdapter.CreateAirdragData(Vehicle, mission, _segment, ovcMode);
				runData.DriverData = DriverData;

				DataAdapter.CreateREESSData(
					componentsElectricStorage: Vehicle.Components.ElectricStorage,
					Vehicle.VehicleType,
                    Vehicle.OVC,
					Vehicle.BatteryOnlyMode,
					(bs) => runData.BatteryData = bs,
					(sc) => runData.SuperCapData = sc);

                runData.ElectricMachinesSinglePwt = DataAdapter.CreateElectricMachines(Vehicle.Components.ElectricMachines, Vehicle.ElectricMotorTorqueLimits, runData.BatteryData.CalculateVoltageCenterSoc(), null);
				if (Vehicle.VehicleType == VectoSimulationJobType.FCHV_IEPC)
				{
					runData.ElectricMachinesSinglePwt = DataAdapter.CreateIEPCElectricMachines(Vehicle.Components.IEPC,
						runData.BatteryData.CalculateVoltageCenterSoc());
				}

				runData.AngledriveSinglePwt = DataAdapter.CreateAngledriveData(Vehicle.Components.AngledriveInputData);
				if (AxleGearRequired() || Vehicle.Components.AxleGearInputData != null)
				{
					runData.AxleGearSinglePwt = DataAdapter.CreateAxleGearData(Vehicle.Components.AxleGearInputData);
				}

				runData.VehicleData =
					DataAdapter.CreateVehicleData(Vehicle, _segment, mission, loading, _allowVocational);

				runData.WheelEndData = DataAdapter.CreateWheelEndData(_segment.VehicleClass, Vehicle);

				if (Vehicle.Components.RetarderInputData != null)
				{
					runData.RetarderSinglePwt = DataAdapter.CreateRetarderData(Vehicle.Components.RetarderInputData,
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
					Vehicle.VehicleType, runData.BatteryOnlyHybridMode);

				if (Vehicle.BatteryOnlyMode && runData.Aux.Any(x => x.ID != Constants.Auxiliaries.IDs.Fan && !x.ConnectToREESS)) {
					throw new VectoException(
						"Vehicles with a battery dominant mode are required to have electrically powered auxiliaries");
				}

                var ptoTransmissionData = DataAdapter.CreatePTOTransmissionData(Vehicle.Components.PTOTransmissionInputData, Vehicle.Components.GearboxInputData);

				var municipalPtoTransmissionData = DataAdapter.CreatePTOCycleData(Vehicle.Components.GearboxInputData, Vehicle.Components.PTOTransmissionInputData, runData.BatteryOnlyHybridMode);

				runData.PTOSinglePwt = mission.MissionType == MissionType.MunicipalUtility
					? municipalPtoTransmissionData
					: ptoTransmissionData;

				/// Refer to [1] EngineeringModeVectoRunDataFactory.GetFCHV_RunData():
				/// Comment from [1]:
				///		In case the battery is modified after creating the rundata
				///		(testing, do not create new battery data).
				runData.BatteryData = new FCHVDeclarationDataAdapter(DataProvider.DataSource).CreateFuelCellPreProcessingBattery(
					DataAdapter.CreateFuelCells(Vehicle.Components.FuelCellSystem).ConvertToEngineeringData(),
					runData.BatteryData,
					out var fcBatteries);

				runData.OVCMode = ovcMode;
				runData.ModFileSuffix += "_pre";
				runData.IterativeRunStrategy = DeclarationFuelCellIterativeStrategy.SetUpFuelCellIterativeRunStrategy(
					runData,
                    InputDataProvider.JobInputData.Vehicle,
                    DataAdapter, 
					InputDataProvider, 
					FuelCellJobType, 
					fcBatteries);

				runData.BatteryData.Batteries.ForEach(t => t.Item2.ChargeDepletingBattery = true);

				return runData;
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
				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						(runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0) * (runData.AngledriveSinglePwt?.Angledrive.Ratio ?? 1.0),
						null,
						Vehicle.Components.GearboxInputData.Type,
						Vehicle.Components.GearboxInputData.Gears.Count
					);
				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData);
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
				runData.GearshiftParametersSinglePwt =
					DataAdapter.CreateGearshiftData(
						runData.AxleGearSinglePwt?.AxleGear.Ratio ?? 1.0,
						null,
						GearboxType.APTN,
						Vehicle.Components.IEPC.Gears.Count
					);
				runData.GearboxSinglePwt = DataAdapter.CreateGearboxData(Vehicle, runData, GearboxType.APTN);

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