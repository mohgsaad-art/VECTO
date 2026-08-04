#if CERTIFICATION_RELEASE || RELEASE_CANDIDATE
#define PROHIBIT_NEW_XML
#endif

//#define PROHIBIT_NEW_XML

using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
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
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SingleBus
{
    public abstract class DeclarationDataAdapterSingleBus
	{
		public abstract class SingleBusBase : BaseSimulationDataAdapter, ISingleBusDeclarationDataAdapter
		{
			[Inject]
			public IShiftStrategyFactory ShiftStrategyFactory { get; private set; }

            public abstract GearboxType[] SupportedGearboxTypes { get; }

			private IDriverDataAdapterBus _driverDataAdapter = new PrimaryBusDriverDataAdapter();
			private SingleBusVehicleDataAdapter _vehicleDataAdapter = new SingleBusVehicleDataAdapter();
			private IAxleGearDataAdapter _axleGearDataAdapter = new AxleGearDataAdapter();
			private IGenericRetarderDataAdapter _retarderDataAdapter = new GenericRetarderDataAdapter();
			private IAirdragDataAdapter _airdragDataAdapter = new SingleBusAirdragDataAdapter();
			private IAngledriveDataAdapter _angledriveDataAdapter = new AngledriveDataAdapter();

			protected abstract IEngineDataAdapter EngineDataAdapter { get; }

			protected abstract IGearboxDataAdapter GearboxDataAdapter { get; }

			protected virtual IElectricMachinesDataAdapter ElectricMachinesDataAdapter => throw new NotImplementedException();

			protected abstract IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; }

			protected virtual ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new SpecificCompletedBusAuxiliaryDataAdapter();

            protected virtual IFuelCellDataAdapter FuelCellDataAdapter { get; }

            protected string GetShiftStrategyName(IVehicleDeclarationInputData inputData,
				GearboxType? overrideGearboxType, bool batteryOnlyHybrid, bool isTestPowertrain = false)
			{
				var gbxType = overrideGearboxType ?? inputData.Components.GearboxInputData.Type;
				return ShiftStrategyFactory.GetShiftStrategyName(gbxType, inputData.VehicleType, batteryOnlyHybrid);
			}

            #region Implementation of IDeclarationDataAdapter

            public virtual IList<AxlePowertrainData> CreateAxlePowertrainsData(IVehicleDeclarationInputData vehicle, Volt averageVoltage, 
				bool batteryOnlyHybridMode, VehicleData vehicleData, Mission mission)
            {
                return null;
            }

            public VehicleData CreateVehicleData(ISingleBusInputDataProvider vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading
					, allowVocational);
			}

			public HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData, SuperCapData runDataSuperCapData,
				Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType)
			{
				return HybridStrategyDataAdapter.CreateHybridStrategyParameters(runDataBatteryData,
					runDataSuperCapData, vehicleMass, ovcMode);
            }

			public HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData, SuperCapData runDataSuperCapData,
				Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType,
				TableData boostingLimitations, GearboxData gearboxData, CombustionEngineData engineData, IList<Tuple<PowertrainPosition, ElectricMotorData>> emData,
                ArchitectureID architectureId)
			{
				return HybridStrategyDataAdapter.CreateHybridStrategyParameters(
					batterySystemData: runDataBatteryData,
					superCap: runDataSuperCapData,
					ovcMode: ovcMode,
					loading: loading,
					vehicleClass: vehicleClass,
					missionType: missionType, archID: architectureId, engineData: engineData, emData, gearboxData: gearboxData, boostingLimitations: boostingLimitations);
            }

            #endregion

            #region Implementation of ISingleBusDeclarationDataAdapter

            public FuelCellSystemDeclarationData CreateFuelCells(IFuelCellSystemDeclarationInputData fuelCellSystem)
            {
                return FuelCellDataAdapter.CreateFuelCells(fuelCellSystem);
            }

            public AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission, Segment segment, OvcHevMode ovcMode)
			{
				return _airdragDataAdapter.CreateAirdragData(completedVehicle, mission, segment, ovcMode);
			}

			public virtual CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
				IEngineModeDeclarationInputData engineMode, Mission mission)
			{
				return EngineDataAdapter.CreateEngineData(vehicle, engineMode, mission);
			}

			public DriverData CreateBusDriverData(Segment segment, VectoSimulationJobType jobType, ArchitectureID arch,
				CompressorDrive compressorDrive)
			{
				return _driverDataAdapter.CreateBusDriverData(segment, jobType, arch, compressorDrive);
			}


			//public virtual AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gearboxInputData)
			//{
			//	return _axleGearDataAdapter.CreateDummyAxleGearData(gearboxInputData);
			//}

			public virtual AxleGearData CreateAxleGearData(IAxleGearInputData axleGearInputData)
			{
				return _axleGearDataAdapter.CreateAxleGearData(axleGearInputData);
			}

			public virtual AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
			{
				return _angledriveDataAdapter.CreateAngledriveData(angledriveData);
			}

			public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				GearboxType? overrideGearboxType = null)
			{
				var name = GetShiftStrategyName(inputData, overrideGearboxType, runData.BatteryOnlyHybridMode);
				var retVal = GearboxDataAdapter.CreateGearboxData(inputData, runData, ShiftStrategyFactory.CreateShiftPolygonCalculator(name, runData.GearshiftParametersSinglePwt), supportedGearboxTypes: SupportedGearboxTypes);
				retVal.ShiftStrategy = name;
				return retVal;
			}

			public virtual RetarderData CreateRetarderData(IRetarderInputData retarderData, ArchitectureID archID,
				IIEPCDeclarationInputData iepcInputData)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData, archID, iepcInputData);
			}

			public virtual PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
			{
				throw new VectoException("PTO is not allowed for buses!");
			}

			public virtual ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gbxType, int gearsCount)
			{
				return GearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, gbxType, gearsCount);
			}

			public virtual IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
				IBusAuxiliariesDeclarationData busAuxInput, MissionType mission,
				VehicleClass segment, Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType, bool batteryOnlyHybridMode)
			{
				return AuxDataAdapter.CreateAuxiliaryData(auxInputData, busAuxInput, mission, segment,
					vehicleLength, numSteeredAxles, jobType, batteryOnlyHybridMode);
			}

			public virtual IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission,
				IVehicleDeclarationInputData primaryVehicle,
				IVehicleDeclarationInputData completedVehicle, VectoRunData simulationRunData)
			{
				return AuxDataAdapter.CreateBusAuxiliariesData(mission, primaryVehicle, completedVehicle,
					simulationRunData);
			}

			#endregion

			#region Implementation of IDeclarationDataAdapter

			public VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission first,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> keyValuePair, bool allowVocational)
			{
				throw new VectoException("Not applicable for Single Bus!");
			}

			public virtual List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				return ElectricMachinesDataAdapter.CreateIEPCElectricMachines(iepc, averageVoltage);
            }

			public virtual IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits,
				Volt averageVoltage, GearList gears = null)
			{
				return ElectricMachinesDataAdapter.CreateElectricMachines(electricMachines, torqueLimits, averageVoltage, gears);
            }

            public virtual Tuple<PowertrainPosition, ElectricMotorData> CreateElectricMachine(
                ElectricMachineEntry<IElectricMotorDeclarationInputData> em,
                IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits,
                Volt averageVoltage,
                int axleNumber)
            {
                return ElectricMachinesDataAdapter.CreateElectricMachine(em, torqueLimits, averageVoltage, axleNumber);
            }

            public abstract void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData);

			public RetarderData CreateGenericRetarderData(IRetarderInputData retarderData, VectoRunData vectoRun)
			{
				throw new NotImplementedException("Not applicable to SingleBus");
			}

			#endregion
		}

		public class Conventional : SingleBusBase
		{
            #region Overrides of SingleBusBase
			public override GearboxType[] SupportedGearboxTypes => new GearboxType[]
			{
				GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial
			};

            protected override IEngineDataAdapter EngineDataAdapter { get; } = new CombustionEngineComponentDataAdapter();
            protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter =>
				throw new NotImplementedException();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public abstract class Hybrid : SingleBusBase
		{
			private IElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();

			protected override IEngineDataAdapter EngineDataAdapter { get; } = new CombustionEngineComponentDataAdapter();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

			
            public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc, batteryOnlyMode);
				var superCapData = _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

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
			public override GearboxType[] SupportedGearboxTypes => new GearboxType[] { };

            protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				SerialHybridStrategyParameterDataAdapter();
        }

		public class HEV_S2 : SerialHybrid
		{
			public override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.APTN, GearboxType.ATSerial };

            protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

        }

        public class HEV_S3 : SerialHybrid { }

		public class HEV_S4 : SerialHybrid { }

		public class HEV_S_IEPC : SerialHybrid
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();
        }

		public abstract class ParallelHybrid : Hybrid
		{
			public override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATSerial, GearboxType.ATPowerSplit };

            protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				ParallelHybridStrategyParameterDataAdapter();
        }

		public class HEV_P1 : ParallelHybrid { }

		public class HEV_P2 : ParallelHybrid
		{
			public override GearboxType[] SupportedGearboxTypes => new[] { GearboxType.AMT, GearboxType.IHPC };
        }

		public class HEV_P2_5 : ParallelHybrid { }

		public class HEV_P3 : ParallelHybrid { }

		public class HEV_P4 : ParallelHybrid { }

		public abstract class FCHV : SingleBusBase
		{
            protected override IFuelCellDataAdapter FuelCellDataAdapter { get; } = new FuelCellDataAdapter();

            public override GearboxType[] SupportedGearboxTypes => new GearboxType[] { };

            private readonly IElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();

            protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

            protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

            protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } =
                new ElectricMachinesDataAdapter();

            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

            protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } =
                new SpecificCompletedPEVBusAuxiliaryDataAdapter();

            public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
                VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
            {
                var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc, batteryOnlyMode);
                var superCapData = _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

                if (batteryData != null)
                {
                    setBatteryData(batteryData);
                }
                if (superCapData != null)
                {
                    setSuperCapData(superCapData);
                }

                if (batteryData != null && superCapData != null)
                {
                    throw new VectoException("Either battery or super cap must be provided");
                }
            }

        }

        public class FCHV_F2 : FCHV
		{
            public override GearboxType[] SupportedGearboxTypes => new[]
                { GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.APTN, GearboxType.ATSerial };

            protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());
        }

		public class FCHV_F3 : FCHV
		{
		}

		public class FCHV_F4 : FCHV
		{
		}

		public class FCHV_IEPC : FCHV
		{
            protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();
        }

		public abstract class BatteryElectric : SingleBusBase
		{
			public override GearboxType[] SupportedGearboxTypes => new GearboxType[] {  };

            private readonly IElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();

			protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } =
				new ElectricMachinesDataAdapter();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

			protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } =
				new SpecificCompletedPEVBusAuxiliaryDataAdapter();

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
        }

		public class PEV_E2 : BatteryElectric
		{
			public override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.APTN, GearboxType.ATSerial };

            protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

        }

        public class PEV_E3 : BatteryElectric { }

		public class PEV_E4 : BatteryElectric { }

		public class PEV_E_IEPC : BatteryElectric
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();
        }

		public abstract class MultiplePowertrains : SingleBusBase
		{
            public override GearboxType[] SupportedGearboxTypes => new GearboxType[] { };

            protected IGearboxDataAdapter IEPCGearboxDataAdapter => new IEPCGearboxDataAdapter();

            protected readonly IElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();

            protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

            protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

            protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

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
                        GearshiftParameters = gearshiftParams,
                        ShiftStrategy = shiftStrategy
                    });
                }

                Utils.CrossValidatePowertrains(vehicle, axlePts);

                return axlePts;
            }

            private Tuple<GearboxData, ShiftStrategyParameters, string> CreateGearboxDataForAxlePowertrain(
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

                shiftStrategyName = GetShiftStrategyName(vehicleData.InputData, gearboxType, false);

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

		public class MultiplePEV : MultiplePowertrains
		{
            protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new SpecificCompletedPEVBusAuxiliaryDataAdapter();

            public override void CreateREESSData(
				IElectricStorageSystemDeclarationInputData componentsElectricStorage, 
				VectoSimulationJobType jobType, 
				bool ovc, 
				bool batteryOnlyMode,
				Action<BatterySystemData> setBatteryData, 
				Action<SuperCapData> setSuperCapData)
            {
                var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc, batteryOnlyMode);
                var superCapData = _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

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

		public class MultipleFCHV : MultiplePowertrains
		{
            protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new SpecificCompletedPEVBusAuxiliaryDataAdapter();

            protected override IFuelCellDataAdapter FuelCellDataAdapter { get; } = new FuelCellDataAdapter();

            public override void CreateREESSData(
				IElectricStorageSystemDeclarationInputData componentsElectricStorage,
                VectoSimulationJobType jobType, 
				bool ovc, 
				bool batteryOnlyMode,
				Action<BatterySystemData> setBatteryData, 
				Action<SuperCapData> setSuperCapData)
            {
                var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc, batteryOnlyMode);
                var superCapData = _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

                if (batteryData != null)
                {
                    setBatteryData(batteryData);
                }
                
				if (superCapData != null)
                {
                    setSuperCapData(superCapData);
                }

                if ((batteryData != null) && (superCapData != null))
                {
                    throw new VectoException("Either battery or super cap must be provided");
                }
			}
		}

		public class MultipleSHEV : MultiplePowertrains
		{
            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new SerialHybridStrategyParameterDataAdapter();

            protected override IEngineDataAdapter EngineDataAdapter { get; } = new CombustionEngineComponentDataAdapter();

            public override void CreateREESSData(
				IElectricStorageSystemDeclarationInputData componentsElectricStorage,
                VectoSimulationJobType jobType,
				bool ovc,
				bool batteryOnlyMode,
				Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData)
            {
                var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc, batteryOnlyMode);
                var superCapData = _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

                if (batteryData != null)
                {
                    setBatteryData(batteryData);
                }

                if (superCapData != null)
                {
                    setSuperCapData(superCapData);
                }

                if ((batteryData != null) && (superCapData != null))
                {
                    throw new VectoException("Either battery or super cap must be provided");
                }
            }
        }

		public class Exempted : SingleBusBase
		{
			public override GearboxType[] SupportedGearboxTypes => new GearboxType[] { };

            protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();
			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();
			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();
			protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter => throw new NotImplementedException();

			
			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, bool batteryOnlyMode, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}
        }
	}
}
