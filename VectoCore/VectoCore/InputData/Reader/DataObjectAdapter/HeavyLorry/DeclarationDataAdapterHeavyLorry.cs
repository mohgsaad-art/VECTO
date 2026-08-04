#if CERTIFICATION_RELEASE || RELEASE_CANDIDATE
#define PROHIBIT_NEW_XML
#endif

//#define PROHIBIT_NEW_XML

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Ninject.Activation.Caching;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.AuxiliaryDataAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry
{
    public class DeclarationDataAdapterHeavyLorry
	{
		public abstract class LorryBase : AbstractSimulationDataAdapter, ILorryDeclarationDataAdapter
		{

			protected virtual IDriverDataAdapter DriverDataAdapter { get; } = new LorryDriverDataAdapter();
            //protected readonly IVehicleDataAdapter _vehicleDataAdapter = new LorryVehicleDataAdapter();
			protected virtual IAxleGearDataAdapter AxleGearDataAdapter { get; }  = new AxleGearDataAdapter();
			protected virtual IRetarderDataAdapter RetarderDataAdapter { get; }  = new RetarderDataAdapter();
			protected virtual IAirdragDataAdapter AirdragDataAdapter { get; }  = new AirdragDataAdapter();

			protected virtual IAngledriveDataAdapter AngleDriveDataAdapter { get; } = new AngledriveDataAdapter();

			protected virtual IVehicleDataAdapter VehicleDataAdapter { get; } = new LorryVehicleDataAdapter();
			protected abstract IEngineDataAdapter EngineDataAdapter { get; }
			protected abstract IGearboxDataAdapter GearboxDataAdapter { get; }
			protected abstract IAuxiliaryDataAdapter AuxDataAdapter { get; }

			protected abstract IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; }

			protected abstract IPTODataAdapter PtoDataAdapter { get; }

			protected virtual IElectricMachinesDataAdapter ElectricMachinesDataAdapter => throw new NotImplementedException();

			protected virtual IFuelCellDataAdapter FuelCellDataAdapter { get; }

			public virtual IList<AxlePowertrainData> CreateAxlePowertrainsData(
                IVehicleDeclarationInputData vehicle, Volt averageVoltage, bool batteryOnlyHybridMode, VehicleData vehicleData, Mission mission)
			{
				return null;
			}

            public virtual DriverData CreateDriverData(Segment segment)
			{
				return DriverDataAdapter.CreateDriverData(segment);
			}

			protected abstract GearboxType[] SupportedGearboxTypes { get; }

			public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return VehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading.Value.Item1,
					loading.Value.Item2, allowVocational);
			}

			public abstract void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode,Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData);


			// serial hybrids
			public virtual HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData, 
				SuperCapData runDataSuperCapData,
				Kilogram vehicleMass, 
				OvcHevMode ovcMode, 
				LoadingType loading, 
				VehicleClass vehicleClass, 
				MissionType missionType)
			{
				return HybridStrategyDataAdapter.CreateHybridStrategyParameters(runDataBatteryData,
					runDataSuperCapData, vehicleMass, ovcMode);
			}

			// parallel hybrids
			public virtual HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
				SuperCapData runDataSuperCapData,
				Kilogram vehicleMass,
				OvcHevMode ovcMode,
				LoadingType loading,
				VehicleClass vehicleClass,
				MissionType missionType,
				TableData boostingLimitations, 
				GearboxData gearboxData, 
				CombustionEngineData engineData,
				IList<Tuple<PowertrainPosition, ElectricMotorData>> emData,
				ArchitectureID archId)
			{
				return HybridStrategyDataAdapter.CreateHybridStrategyParameters(
					batterySystemData: runDataBatteryData,
					superCap: runDataSuperCapData,
					ovcMode: ovcMode,
					loading: loading,
					vehicleClass: vehicleClass,
					missionType: missionType, archID: archId, engineData: engineData, emData, gearboxData: gearboxData, boostingLimitations: boostingLimitations);
			}


			public virtual AirdragData CreateAirdragData(IVehicleDeclarationInputData vehicleData, Mission mission,
				Segment segment, OvcHevMode ovcMode)
			{
				return AirdragDataAdapter.CreateAirdragData(vehicleData, mission, segment, ovcMode);
			}

			public AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				return AxleGearDataAdapter.CreateAxleGearData(axlegearData);
			}


			public AngledriveData CreateAngledriveData(IAngledriveInputData data)
			{
				return AngleDriveDataAdapter.CreateAngledriveData(data, false);
			}

			public virtual CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
				IEngineModeDeclarationInputData engineMode,
				Mission mission)
			{
				return EngineDataAdapter.CreateEngineData(vehicle, engineMode, mission);
			}

			public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData, GearboxType? overrideGearboxType = null)
			{
				var name = GetShiftStrategyName(inputData, overrideGearboxType, runData.BatteryOnlyHybridMode);
				var retVal = GearboxDataAdapter.CreateGearboxData(inputData, runData, ShiftStrategyFactory.CreateShiftPolygonCalculator(name, runData.GearshiftParametersSinglePwt), SupportedGearboxTypes);
				retVal.ShiftStrategy = name;
				return retVal;
			}

			public virtual ShiftStrategyParameters CreateGearshiftData(double axleRatio,
				PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount)
			{
				return GearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, gearboxType, gearsCount);
			}

			public RetarderData CreateRetarderData(IRetarderInputData retarderData, ArchitectureID archID,
				IIEPCDeclarationInputData iepcInputData)
			{
				return RetarderDataAdapter.CreateRetarderData(retarderData, archID, iepcInputData);
			}

			public virtual PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto, bool batteryOnlyHybridMode)
			{
				return PtoDataAdapter.CreateDefaultPTOData(pto, gbx);
			}

			public virtual PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData,
				IGearboxDeclarationInputData gbx)
			{
				return PtoDataAdapter.CreatePTOTransmissionData(ptoData, gbx);
			}


			public virtual IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType, bool batteryOnlyHybridMode)
			{
				return AuxDataAdapter.CreateAuxiliaryData(auxData, null, missionType, vehicleClass, vehicleLength, numSteeredAxles,
					jobType, batteryOnlyHybridMode);
			}

			public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
			{
				return AxleGearDataAdapter.CreateDummyAxleGearData(gbxData);
			}

			public virtual IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(
				IElectricMachinesDeclarationInputData electricMachines,
				IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage,
				GearList gears = null)
			{
				return ElectricMachinesDataAdapter.CreateElectricMachines(electricMachines, torqueLimits, averageVoltage,
					gears);
			}

			public virtual List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(
				IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				return ElectricMachinesDataAdapter.CreateIEPCElectricMachines(iepc, averageVoltage);
			}

            public virtual Tuple<PowertrainPosition, ElectricMotorData> CreateElectricMachine(
				ElectricMachineEntry<IElectricMotorDeclarationInputData> em,
				IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits,
				Volt averageVoltage,
				int axleNumber)
			{
				return ElectricMachinesDataAdapter.CreateElectricMachine(em, torqueLimits, averageVoltage, axleNumber);
			}

            public RetarderData CreateGenericRetarderData(IRetarderInputData retarderData, VectoRunData vectoRun)
			{
				throw new NotImplementedException("Not applicable to Heavy Lorries");
			}

			FuelCellSystemDeclarationData IDeclarationDataAdapter.CreateFuelCells(IFuelCellSystemDeclarationInputData fuelCellSystem)
			{
				return FuelCellDataAdapter.CreateFuelCells(fuelCellSystem);
			}
		}

		public abstract class MultiplePowertrainsLorryDataAdapter : LorryBase
		{
            protected override IPTODataAdapter PtoDataAdapter => new ElectricPTODataAdapter();

            protected IGearboxDataAdapter IEPCGearboxDataAdapter => new IEPCGearboxDataAdapter();

            protected IElectricStorageAdapter ElectricStorageAdapter => new ElectricStorageAdapter();

            protected override IGearboxDataAdapter GearboxDataAdapter => new GearboxDataAdapter(new TorqueConverterDataAdapter());

            protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter => new ElectricMachinesDataAdapter();

            protected override GearboxType[] SupportedGearboxTypes => new[] { 
				GearboxType.AMT, 
				GearboxType.ATPowerSplit, 
				GearboxType.ATSerial, 
				GearboxType.APTN 
			};

            public override IList<AxlePowertrainData> CreateAxlePowertrainsData(
                IVehicleDeclarationInputData vehicle, 
				Volt averageVoltage, 
				bool batteryOnlyHybridMode,
				VehicleData vehicleData,
				Mission mission)
            {
                IList<AxlePowertrainData> axlePts = new List<AxlePowertrainData>();

                foreach (var axlePtData in vehicle.Components.AxlePowertrainInputData)
                {
                    ValidateIEPCData(axlePtData.IEPCInputData, axlePtData.AxleGearInputData);

#if PROHIBIT_NEW_XML
                    if ((axlePtData.IEPCInputData != null) && axlePtData.IEPCInputData.Gears.Count > 1)
                    {
                        throw new VectoException($"Architecture {axlePtData.Architecture} with multiple gears is not supported yet!");
                    }
#endif

                    var axlegearData = (axlePtData.AxleGearInputData != null) ? CreateAxleGearData(axlePtData.AxleGearInputData) : null;

                    var emData = (axlePtData.IEPCInputData != null)
                        ? CreateIEPCElectricMachines(axlePtData.IEPCInputData, averageVoltage).First()
                        : ElectricMachinesDataAdapter.CreateElectricMachine(
                            axlePtData.ElectricMotor,
                            vehicle.ElectricMotorTorqueLimits,
                            averageVoltage,
                            axlePtData.AxleNumber);

					Utils.ValidatePowertrainEMPosition(emData.Item1, axlePtData);

                    var angledriveData = CreateAngledriveData(axlePtData.AngledriveInputData);

					var retarderData = CreateRetarderData(axlePtData.RetarderInputData, axlePtData.Architecture, axlePtData.IEPCInputData);

					var pto = (mission.MissionType == MissionType.MunicipalUtility)
						? CreatePTOCycleData(axlePtData.GearboxInputData, axlePtData.PTOTransmissionInputData, batteryOnlyHybridMode)
                        : CreatePTOTransmissionData(axlePtData.PTOTransmissionInputData, axlePtData.GearboxInputData);

                    var (gearboxData, gearshiftParams, shiftStrategy) = 
						CreateGearboxDataForAxlePowertrain(emData, axlePtData, axlegearData, batteryOnlyHybridMode, vehicleData);

                    axlePts.Add(new AxlePowertrainData()
                    {
                        AxleNumber = axlePtData.AxleNumber,
                        Architecture = axlePtData.Architecture,
                        AxleGearData = axlegearData,
                        ElectricMachineData = emData,
                        AngledriveData = angledriveData,
                        GearboxData = gearboxData,
                        Retarder = retarderData,
                        PTO = pto,
                        GearshiftParameters = gearshiftParams,
						ShiftStrategy = shiftStrategy
                    });
                }

				Utils.CrossValidatePowertrains(vehicle, axlePts);

                return axlePts;
            }

			protected Tuple<GearboxData, ShiftStrategyParameters, string> CreateGearboxDataForAxlePowertrain(
				Tuple<PowertrainPosition, ElectricMotorData> emData,
				IAxlePowertrainDeclarationInputData axlePtData,
				AxleGearData axlegearData,
				bool batteryOnlyHybridMode,
                VehicleData vehicleData
                )
			{
				GearboxData gearboxData = null;
				ShiftStrategyParameters gearshiftParams = null;
				string shiftStrategyName = null;
                
                if (!emData.Item1.IsOneOf(PowertrainPosition.BatteryElectricE2, PowertrainPosition.IEPC))
				{
                    gearshiftParams = new ShiftStrategyParameters()
                    {
                        StartSpeed = DeclarationData.GearboxTCU.StartSpeed,
                        StartAcceleration = DeclarationData.GearboxTCU.StartAcceleration
                    };

                    return new Tuple<GearboxData, ShiftStrategyParameters, string>(gearboxData, gearshiftParams, shiftStrategyName);
                }

				var gearboxType = axlePtData.GearboxInputData?.Type ?? GearboxType.APTN;

                gearshiftParams = CreateGearshiftData(
					axlePtData.AxleGearInputData?.Ratio ?? 1,
                    null,
                    gearboxType,
                    axlePtData.GearboxInputData?.Gears.Count ?? axlePtData.IEPCInputData.Gears.Count);

                shiftStrategyName = GetShiftStrategyName(vehicleData.InputData, gearboxType, batteryOnlyHybridMode);

                var gearboxRunData = new VectoRunData()
                {
                    VehicleData = vehicleData,
                    AxleGearSinglePwt = axlegearData,
                    ElectricMachinesSinglePwt = new List<Tuple<PowertrainPosition, ElectricMotorData>>() { emData },
                };

				var shiftPolygonCalculator = ShiftStrategyFactory.CreateShiftPolygonCalculator(shiftStrategyName, gearshiftParams);

                gearboxData = (axlePtData.IEPCInputData != null)
                    ? IEPCGearboxDataAdapter.CreateGearboxData(
                        gearboxRunData,
                        shiftPolygonCalculator,
                        axlePtData.IEPCInputData)
                    : GearboxDataAdapter.CreateGearboxData(
                        vehicleData.InputData,
                        gearboxRunData,
                        shiftPolygonCalculator,
                        SupportedGearboxTypes,
                        axlePtData.GearboxInputData,
                        axlePtData.TorqueConverterInputData);

                return new Tuple<GearboxData, ShiftStrategyParameters, string>(gearboxData, gearshiftParams, shiftStrategyName);
            }
		}

		public class MultiplePEVLorryDataAdapter : MultiplePowertrainsLorryDataAdapter
		{
            protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

            protected override IAuxiliaryDataAdapter AuxDataAdapter => new HeavyLorryPEVAuxiliaryDataAdapter();

            public override void CreateREESSData(
                IElectricStorageSystemDeclarationInputData componentsElectricStorage,
                VectoSimulationJobType jobType,
                bool ovc,
				bool batteryOnlyMode,
                Action<BatterySystemData> setBatteryData,
                Action<SuperCapData> setSuperCapData)
            {
                var batteryData = ElectricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc, batteryOnlyMode);
                var superCapData = ElectricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

                if (batteryData == null)
                {
                    throw new VectoException($"Could not create BatterySystem for {jobType}");
                }

                if (superCapData != null)
                {
                    throw new VectoException($"Supercaps are not allowed for {jobType}");
                }

                setBatteryData(batteryData);
            }
		}

		public class MultipleFCHVLorryDataAdapter : MultiplePEVLorryDataAdapter
		{
            protected override IFuelCellDataAdapter FuelCellDataAdapter { get; } = new FuelCellDataAdapter();
        }

		public class MultipleSHEVLorryDataAdapter : MultiplePowertrainsLorryDataAdapter
		{
            protected override IAuxiliaryDataAdapter AuxDataAdapter => new HeavyLorryAuxiliaryDataAdapter();

            protected override IEngineDataAdapter EngineDataAdapter => new CombustionEngineComponentDataAdapter();

            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => new SerialHybridStrategyParameterDataAdapter();

            public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
                VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
            {
                var batteryData = ElectricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc, batteryOnlyMode);
                var superCapData = ElectricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

                if (batteryData != null && superCapData != null)
                {
                    throw new VectoException("Either battery or super cap must be provided");
                }

                if (batteryData != null)
                {
                    setBatteryData(batteryData);
                }

                if (superCapData != null)
                {
                    setSuperCapData(superCapData);
                }
            }
        }

		public class Conventional : LorryBase
		{
			public static GearboxType[] SupportsGearboxTypes = { GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType
				.ATSerial};

			protected override GearboxType[] SupportedGearboxTypes => new []
				{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

			protected override IAuxiliaryDataAdapter AuxDataAdapter { get; } = new HeavyLorryAuxiliaryDataAdapter();
			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();
			protected override IPTODataAdapter PtoDataAdapter { get; } = new PTODataAdapterLorry();

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			protected override IEngineDataAdapter EngineDataAdapter { get; } = new CombustionEngineComponentDataAdapter();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}
		}

		public abstract class Hybrid : LorryBase
		{
			private IElectricStorageAdapter _eletricStorageAdapter = new ElectricStorageAdapter();
			protected IAuxiliaryDataAdapter _auxAdapter = new HeavyLorryAuxiliaryDataAdapter();

			protected override IEngineDataAdapter EngineDataAdapter { get; } = new CombustionEngineComponentDataAdapter();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

			protected override IAuxiliaryDataAdapter AuxDataAdapter => _auxAdapter;

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _eletricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc, batteryOnlyMode);
				var superCapData = _eletricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

				if (batteryData != null) {
					setBatteryData(batteryData);
				}
				if (superCapData != null) {
					setSuperCapData(superCapData);
				}

				if (batteryData != null && superCapData != null) {
					throw new VectoException("Either battery or super cap must be provided");
				}
			}
		}

		public abstract class SerialHybrid : Hybrid
		{
			#region Overrides of LorryBase

			protected override GearboxType[] SupportedGearboxTypes { get; }

			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				SerialHybridStrategyParameterDataAdapter();

			protected override IPTODataAdapter PtoDataAdapter { get; } = new ElectricPTODataAdapter();

			#endregion

		}

		public abstract class ParallelHybrid : Hybrid
		{
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				ParallelHybridStrategyParameterDataAdapter();

			protected override IPTODataAdapter PtoDataAdapter { get; } = new PTODataAdapterLorry();

			protected virtual IPTODataAdapter BatteryElectricPtoDataAdapter { get; } = new ElectricPTODataAdapter();

			public override PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto, bool batteryOnlyHybridMode)
			{
				if (batteryOnlyHybridMode) {
					return BatteryElectricPtoDataAdapter.CreateDefaultPTOData(pto, gbx);
				}
				return PtoDataAdapter.CreateDefaultPTOData(pto, gbx);
			}
        }

		public abstract class BatteryElectric : LorryBase
		{
			#region Overrides of LorryBase


			protected override GearboxType[] SupportedGearboxTypes { get; }

			//private readonly GearboxDataAdapter _gearboxDataAdapter = new GearboxDataAdapter(null);
			private readonly IElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();
			private readonly ElectricMachinesDataAdapter _electricMachineAdapter = new ElectricMachinesDataAdapter();
			
			private readonly HeavyLorryPEVAuxiliaryDataAdapter
				_auxDataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = null;

			protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

			protected override IPTODataAdapter PtoDataAdapter { get; } = new ElectricPTODataAdapter();

			protected override IAuxiliaryDataAdapter AuxDataAdapter => _auxDataAdapter;

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc, batteryOnlyMode);
				var superCapData = _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

				
				if (batteryData == null) {
					throw new VectoException("Could not create BatterySystem for PEV");
				}
				setBatteryData(batteryData);
				
				
				if (superCapData != null) {
					throw new VectoException("Supercaps are not allowed for PEVs");
				}

			}

			#endregion
		}

		public class HEV_S2 : SerialHybrid
		{
			#region Overrides of LorryBase
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.APTN };

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			#endregion
		}
		public class HEV_S3 : SerialHybrid { }
		public class HEV_S4 : SerialHybrid { }

		public class HEV_S_IEPC : SerialHybrid
		{
			
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.APTN };

			private ElectricMachinesDataAdapter _electricMachinesDataAdapter = new ElectricMachinesDataAdapter();

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

		}

		public class FuelCellHybrid : SerialHybrid
		{
			protected override IFuelCellDataAdapter FuelCellDataAdapter { get; } = new FuelCellDataAdapter();

			protected override IAuxiliaryDataAdapter AuxDataAdapter => new HeavyLorryFCHVAuxiliaryDataAdapter();
		}

		public class HEV_F2 : FuelCellHybrid
		{
			#region Overrides of LorryBase
			protected override GearboxType[] SupportedGearboxTypes => new[]
			{
				GearboxType.AMT,
				GearboxType.ATPowerSplit,
				GearboxType.ATSerial,
				GearboxType.APTN
			};

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			#endregion
		}

		public class HEV_F3 : FuelCellHybrid { }

		public class HEV_F4 : FuelCellHybrid { }

		public class HEV_F_IEPC : FuelCellHybrid
		{
			protected override GearboxType[] SupportedGearboxTypes => new[]
			{ 
				GearboxType.AMT,
				GearboxType.ATPowerSplit,
				GearboxType.ATSerial,
				GearboxType.APTN
			};

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();
		}

		public class HEV_P1 : ParallelHybrid
		{
			
		}

		public class HEV_P2 : ParallelHybrid
		{
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.IHPC, };
		}

		public class HEV_P2_5 : ParallelHybrid
		{

		}
		public class HEV_P3 : ParallelHybrid { }
		public class HEV_P4 : ParallelHybrid { }

		public class PEV_E2 : BatteryElectric
		{
			#region Overrides of BatteryElectric

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(null);

            protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.APTN };

			#endregion


		}
		public class PEV_E3 : BatteryElectric { }
		public class PEV_E4 : BatteryElectric { }

		public class PEV_E_IEPC : BatteryElectric
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

		}
		public class Exempted : LorryBase
		{
			#region Overrides of LorryBase

			protected override GearboxType[] SupportedGearboxTypes { get; }

			protected override IVehicleDataAdapter VehicleDataAdapter { get; } = new ExemptedLorryVehicleDataAdapter();

			//public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
			//	KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			//{
			//	return _vehicleDataAdapter.CreateExemptedVehicleData(vehicle);
			//}

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType, bool batteryOnlyHybridMode)
			{
				throw new NotImplementedException();
			}

			protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IAuxiliaryDataAdapter AuxDataAdapter => throw new NotImplementedException();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

			protected override IPTODataAdapter PtoDataAdapter => throw new NotImplementedException();

			protected override IAxleGearDataAdapter AxleGearDataAdapter => throw new NotImplementedException();

			protected override IRetarderDataAdapter RetarderDataAdapter => throw new NotImplementedException();

			protected override IAirdragDataAdapter AirdragDataAdapter => throw new NotImplementedException();

			protected override IAngledriveDataAdapter AngleDriveDataAdapter => throw new NotImplementedException();

            #endregion
        }
    }
}