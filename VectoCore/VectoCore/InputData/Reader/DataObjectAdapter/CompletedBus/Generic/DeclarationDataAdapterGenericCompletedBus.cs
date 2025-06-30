using System;
using System.Collections.Generic;
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

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Generic
{
    public abstract class DeclarationDeclarationDataAdapterGenericCompletedBusDeclaration
	{
		public abstract class CompletedBusDeclarationBase : BaseSimulationDataAdapter, IGenericCompletedBusDeclarationDataAdapter
		{
			[Inject]
			public IShiftStrategyFactory ShiftStrategyFactory { get; private set; }

            protected virtual GearboxType[] SupportedGearboxTypes => new []
				{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

			protected string GetShiftStrategyName(IVehicleDeclarationInputData inputData,
				GearboxType? overrideGearboxType, bool isTestPowertrain = false)
			{
				var gbxType = overrideGearboxType ?? inputData.Components.GearboxInputData.Type;
				return ShiftStrategyFactory.GetShiftStrategyName(gbxType, inputData.VehicleType);
			}

            #region ComponentDataAdapter
            protected virtual IDriverDataAdapterBus DriverDataAdapter => new CompletedBusGenericDriverDataAdapter();
			protected virtual IAxleGearDataAdapter AxleGearDataAdapter => new GenericCompletedBusAxleGearDataAdapter();
			protected virtual IGenericRetarderDataAdapter RetarderDataAdapter => new GenericRetarderDataAdapter();
			protected virtual IAirdragDataAdapter AirdragDataAdapter => new AirdragDataAdapter();
			protected virtual IAngledriveDataAdapter AngledriveDataAdapter => new GenericAngledriveDataAdapter();
            #endregion

			protected virtual IVehicleDataAdapter VehicleDataAdapter { get; } =
				new CompletedBusGenericVehicleDataAdapter();

			protected abstract IEngineDataAdapter EngineDataAdapter { get; }

			protected abstract IGearboxDataAdapter GearboxDataAdapter { get; }

			protected virtual IElectricMachinesDataAdapter ElectricMachinesDataAdapter  => throw new NotImplementedException();

			protected abstract IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; }

			protected abstract ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; }
			protected abstract  IElectricStorageAdapter ElectricStorageAdapter { get; }

			#region Implementation of IGenericCompletedBusDeclarationDataAdapter
            public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return VehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading.Value.Item1,
					loading.Value.Item2, allowVocational);
			}

			public AirdragData CreateAirdragData(IVehicleDeclarationInputData vehicleData, Mission mission, Segment segment, OvcHevMode ovcMode)
			{
				return AirdragDataAdapter.CreateAirdragData(vehicleData, mission, segment, ovcMode);
			}

			public DriverData CreateBusDriverData(Segment segment, VectoSimulationJobType jobType, ArchitectureID arch,
				CompressorDrive compressorDrive)
			{
				return DriverDataAdapter.CreateBusDriverData(segment, jobType, arch, compressorDrive);
			}

			public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission)
			{
				return EngineDataAdapter.CreateEngineData(primaryVehicle, modeIdx, mission);
			}

			public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				return AuxDataAdapter.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass, vehicleLength,
					numSteeredAxles, jobType);
			}

			public virtual AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				return AxleGearDataAdapter.CreateAxleGearData(axlegearData);
			}

			public virtual AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
			{
				return AngledriveDataAdapter.CreateAngledriveData(angledriveData);
			}

			public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData, GearboxType? overrideGearboxType = null)
			{
				var name = GetShiftStrategyName(inputData, overrideGearboxType);
				var retVal = GearboxDataAdapter.CreateGearboxData(inputData, runData, ShiftStrategyFactory.CreateShiftPolygonCalculator(name, runData.GearshiftParameters), supportedGearboxTypes: SupportedGearboxTypes);
				retVal.ShiftStrategy = name;
				return retVal;
            }

            public virtual ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed,
				GearboxType gearboxType, int gearsCount)
			{
				return GearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, gearboxType, gearsCount);
			}

			public RetarderData CreateRetarderData(IRetarderInputData retarderData, ArchitectureID archID,
				IIEPCDeclarationInputData iepcInputData)
			{
				throw new NotImplementedException("No longer applicable to buses.");
			}

			public RetarderData CreateGenericRetarderData(IRetarderInputData retarderData, VectoRunData vectoRun)
			{
				return RetarderDataAdapter.CreateGenericRetarderData(retarderData, vectoRun);
			}

			public virtual IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits,
				Volt averageVoltage, GearList gears = null)
			{
				return ElectricMachinesDataAdapter.CreateElectricMachines(electricMachines, torqueLimits, averageVoltage, gears);
			}

            public virtual List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				return ElectricMachinesDataAdapter.CreateIEPCElectricMachines(iepc, averageVoltage);
            }

			public abstract void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData);

			/// <summary>
			/// Used for Serial Hybrids
			/// </summary>
			/// <param name="runDataBatteryData"></param>
			/// <param name="runDataSuperCapData"></param>
			/// <param name="vehicleMass"></param>
			/// <param name="ovcMode"></param>
			/// <param name="loading"></param>
			/// <param name="vehicleClass"></param>
			/// <param name="missionType"></param>
			/// <returns></returns>
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

			/// <summary>
			/// Used for Parallel Hybrids
			/// </summary>
			/// <param name="runDataBatteryData"></param>
			/// <param name="runDataSuperCapData"></param>
			/// <param name="vehicleMass"></param>
			/// <param name="ovcMode"></param>
			/// <param name="loading"></param>
			/// <param name="vehicleClass"></param>
			/// <param name="missionType"></param>
			/// <param name="boostingLimitations"></param>
			/// <param name="gearboxData"></param>
			/// <param name="engineData"></param>
			/// <param name="runDataElectricMachinesData"></param>
			/// <param name="architectureId"></param>
			/// <returns></returns>
			public virtual HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData, SuperCapData runDataSuperCapData,
				Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType,
				TableData boostingLimitations, GearboxData gearboxData, CombustionEngineData engineData,
				IList<Tuple<PowertrainPosition, ElectricMotorData>> runDataElectricMachinesData,
				ArchitectureID architectureId)
			{
				return HybridStrategyDataAdapter.CreateHybridStrategyParameters(
					batterySystemData: runDataBatteryData,
					superCap: runDataSuperCapData,
					ovcMode: ovcMode,
					loading: loading,
					vehicleClass: vehicleClass,
					missionType: missionType, archID: architectureId, engineData: engineData, runDataElectricMachinesData, gearboxData: gearboxData, boostingLimitations: boostingLimitations);
            }


	

			public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle,
                VectoRunData runData)
			{
				return AuxDataAdapter.CreateBusAuxiliariesData(mission, primaryVehicle, null, runData);
			}
			#endregion
		}

		public class Conventional : CompletedBusDeclarationBase
		{
			#region Overrides of CompletedBusDeclarationBase

			protected override IEngineDataAdapter EngineDataAdapter { get; } = new GenericCombustionEngineComponentDataAdapter();
			
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GenericCompletedBusGearboxDataAdapter(new GenericCompletedBusTorqueConverterDataAdapter());
			
			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter =>
				throw new NotImplementedException();

			protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new GenericCompletedBusAuxiliaryDataAdapter();
			protected override IElectricStorageAdapter ElectricStorageAdapter => throw new NotImplementedException();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}

			#endregion
        }

		public abstract class Hybrid : CompletedBusDeclarationBase
		{
            #region Overrides of CompletedBusDeclarationBase

			protected override IVehicleDataAdapter VehicleDataAdapter { get; } =
				new CompletedBusGenericVehicleDataAdapter_HEV();

            protected override IElectricStorageAdapter ElectricStorageAdapter => new GenericElectricStorageDataAdapter();

            #endregion

            protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } =
				new GenericElectricMachinesDataAdapter();

            protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new GenericCompletedBusAuxiliaryDataAdapter();

            #region Overrides of CompletedBusDeclarationBase

            protected override IEngineDataAdapter EngineDataAdapter { get; } = new GenericCombustionEngineComponentDataAdapter();


            public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = ElectricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
				var superCapData = ElectricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

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

			#endregion
		}

		public abstract class SerialHybrid : Hybrid
		{
			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				SerialHybridStrategyParameterDataAdapter();
        }

		public class HEV_S2 : SerialHybrid
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GenericCompletedBusGearboxDataAdapter(new GenericCompletedBusTorqueConverterDataAdapter());

        }

        public class HEV_S3 : SerialHybrid { }
		
		public class HEV_S4 : SerialHybrid { }

		public class HEV_S_IEPC : SerialHybrid
		{
			#region Overrides of SerialHybrid

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } =
				new GenericCompletedBusIEPCGearboxDataAdapter();

			#endregion
		}

		public abstract class ParallelHybrid : Hybrid
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GenericCompletedBusGearboxDataAdapter(new GenericCompletedBusTorqueConverterDataAdapter());

            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				ParallelHybridStrategyParameterDataAdapter();
        }
		
		public class HEV_P1 : ParallelHybrid { }

		public class HEV_P2 : ParallelHybrid
		{
			#region Overrides of ParallelHybrid

			protected override GearboxType[] SupportedGearboxTypes => new[] { GearboxType.AMT, GearboxType.IHPC };

			#endregion
        }

        public class HEV_P2_5 : ParallelHybrid { }
		
		public class HEV_P3 : ParallelHybrid { }
		
		public class HEV_P4 : ParallelHybrid { }

		public abstract class BatteryElectric : CompletedBusDeclarationBase
		{
			protected override IVehicleDataAdapter VehicleDataAdapter { get; } =
				new CompletedBusGenericVehicleDataAdapter_PEV();

            protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new GenericCompletedPEVBusAuxiliaryDataAdapter();

            protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IElectricStorageAdapter ElectricStorageAdapter => new GenericElectricStorageDataAdapter();

            protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } =
				new GenericElectricMachinesDataAdapter();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = ElectricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
				var superCapData = ElectricStorageAdapter.CreateSuperCapData(componentsElectricStorage);


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
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GenericCompletedBusGearboxDataAdapter(new GenericCompletedBusTorqueConverterDataAdapter());

        }

        public class PEV_E3 : BatteryElectric { }
		
		public class PEV_E4 : BatteryElectric { }

		public class PEV_E_IEPC : BatteryElectric
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GenericCompletedBusIEPCGearboxDataAdapter();
        }


		
		public class Exempted : CompletedBusDeclarationBase
		{
			#region Overrides of CompletedBusBase

			protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();
			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();
            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();
            protected override ICompletedBusAuxiliaryDataAdapter AuxDataAdapter => throw new NotImplementedException();
			protected override IElectricStorageAdapter ElectricStorageAdapter => throw new NotImplementedException();

			protected override IAxleGearDataAdapter AxleGearDataAdapter => throw new NotImplementedException();

			protected override IGenericRetarderDataAdapter RetarderDataAdapter => throw new NotImplementedException();

			protected override IAirdragDataAdapter AirdragDataAdapter => throw new NotImplementedException();

			protected override IAngledriveDataAdapter AngledriveDataAdapter => throw new NotImplementedException();

            public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				throw new NotImplementedException();
            }

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}
            #endregion
        }
	}
}