using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

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

			protected string GetShiftStrategyName(IVehicleDeclarationInputData inputData,
				GearboxType? overrideGearboxType, bool isTestPowertrain = false)
			{
				var gbxType = overrideGearboxType ?? inputData.Components.GearboxInputData.Type;
				return ShiftStrategyFactory.GetShiftStrategyName(gbxType, inputData.VehicleType);
			}

            #region Implementation of IDeclarationDataAdapter

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
				var name = GetShiftStrategyName(inputData, overrideGearboxType);
				var retVal = GearboxDataAdapter.CreateGearboxData(inputData, runData, ShiftStrategyFactory.CreateShiftPolygonCalculator(name, runData.GearshiftParameters), supportedGearboxTypes: SupportedGearboxTypes);
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
				VehicleClass segment, Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType)
			{
				return AuxDataAdapter.CreateAuxiliaryData(auxInputData, busAuxInput, mission, segment,
					vehicleLength, numSteeredAxles, jobType);
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

			public abstract void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
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
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
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
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
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
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
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


		public class Exempted : SingleBusBase
		{
			public override GearboxType[] SupportedGearboxTypes => new GearboxType[] { };

            protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();
			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();
			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();
			protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter => throw new NotImplementedException();

			
			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}
        }
	}
}
