using System;
using System.Collections.Generic;
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

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus
{
    public abstract class DeclarationDataAdapterPrimaryBus
	{
		public abstract class PrimaryBusBase : IPrimaryBusDeclarationDataAdapter
		{

			public abstract GearboxType[] SupportedGearboxTypes { get; }

			private readonly IDriverDataAdapterBus _driverDataAdapter = new PrimaryBusDriverDataAdapter();
			//protected readonly IVehicleDataAdapter _vehicleDataAdapter = new PrimaryBusVehicleDataAdapter();
			protected readonly IAxleGearDataAdapter _axleGearDataAdapter = new AxleGearDataAdapter();
			//protected readonly IPrimaryBusAuxiliaryDataAdapter _auxDataAdapter = new PrimaryBusAuxiliaryDataAdapter();
			protected readonly IRetarderDataAdapter _retarderDataAdapter = new RetarderDataAdapter();
			protected readonly IAirdragDataAdapter _airdragDataAdapter = new AirdragDataAdapter();
			private readonly IAngledriveDataAdapter _angledriveDataAdapter = new AngledriveDataAdapter();

			protected virtual IVehicleDataAdapter VehicleDataAdapter { get; } = new PrimaryBusVehicleDataAdapter();
			protected abstract IEngineDataAdapter EngineDataAdapter { get; }

			protected abstract IGearboxDataAdapter GearboxDataAdapter { get; }

			protected virtual IElectricMachinesDataAdapter ElectricMachinesDataAdapter => throw new NotImplementedException();

			protected abstract IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; }

			protected abstract IPrimaryBusAuxiliaryDataAdapter AuxDataAdapter { get; }


			public DriverData CreateBusDriverData(Segment segment, VectoSimulationJobType jobType, ArchitectureID arch,
				CompressorDrive compressorDrive)
			{
				return _driverDataAdapter.CreateBusDriverData(segment, jobType, arch, compressorDrive);
			}


            public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return VehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading.Value.Item1,
					loading.Value.Item2, allowVocational);
			}

			public virtual AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment)
			{
				return _airdragDataAdapter.CreateAirdragData(airdragData, mission, segment);
			}

			public abstract void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData);

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
				Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass,
				MissionType missionType,
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
					missionType: missionType, architectureId, engineData, runDataElectricMachinesData, gearboxData, boostingLimitations);
			}

			public virtual AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				return _axleGearDataAdapter.CreateAxleGearData(axlegearData);
			}

			public virtual AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
			{
				return _angledriveDataAdapter.CreateAngledriveData(angledriveData);
			}

			public virtual CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData engineMode,
				Mission mission)
			{
				return EngineDataAdapter.CreateEngineData(vehicle, engineMode, mission);
			}

			public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return GearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, SupportedGearboxTypes);
			}

			public virtual ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed,
				GearboxType gearboxType, int gearsCount)
			{
				return GearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, gearboxType, gearsCount);
			}


			public virtual RetarderData CreateRetarderData(IRetarderInputData retarderData, ArchitectureID archID,
				IIEPCDeclarationInputData iepcInputData)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData, archID, iepcInputData);
			}


			public virtual IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits,
				Volt averageVoltage, GearList gears = null)
			{
				return ElectricMachinesDataAdapter.CreateElectricMachines(electricMachines, torqueLimits, averageVoltage, gears);
			}

			public virtual List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				return ElectricMachinesDataAdapter.CreateIEPCElectricMachines(iepc, averageVoltage);
			}

			public virtual IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				{
					return AuxDataAdapter.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass, vehicleLength,
						numSteeredAxles, jobType);
				}
			}



			public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
			{
				throw new NotImplementedException();
			}

			public virtual IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission,
				IVehicleDeclarationInputData vehicleData,
				VectoRunData runData)
			{
				return AuxDataAdapter.CreateBusAuxiliariesData(mission, vehicleData, runData);
			}

			public RetarderData CreateGenericRetarderData(IRetarderInputData retarderData, VectoRunData vectoRun)
			{
				throw new NotImplementedException("Not applicable to Primary Buses");
			}
		}

		public class Conventional : PrimaryBusBase
		{

			#region Overrides of PrimaryBusBase

			public override GearboxType[] SupportedGearboxTypes => new GearboxType[]
			{
				GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial
			};

			protected override IEngineDataAdapter EngineDataAdapter { get; } = new CombustionEngineComponentDataAdapter();

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter =>
				throw new NotImplementedException();

			protected override IPrimaryBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new PrimaryBusAuxiliaryDataAdapter();
			
			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}

			
			#endregion
		}

		public abstract class Hybrid : PrimaryBusBase
		{
			private IElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();


            #region Overrides of PrimaryBusBase
			protected override IVehicleDataAdapter VehicleDataAdapter { get; } = new PrimaryBusVehicleDataAdapter_HEV();

            protected override IEngineDataAdapter EngineDataAdapter { get; } = new CombustionEngineComponentDataAdapter();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

			protected override IPrimaryBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new PrimaryBusAuxiliaryDataAdapter();

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

			#endregion

	
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
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			#region Overrides of SerialHybrid

			public override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.APTN, GearboxType.ATSerial };

			#endregion
		}

		public class HEV_S3 : SerialHybrid
		{

		}

		public class HEV_S4 : SerialHybrid
		{

		}

		public class HEV_S_IEPC : SerialHybrid
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();

			#region Overrides of PrimaryBusBase

			public override GearboxType[] SupportedGearboxTypes => Array.Empty<GearboxType>();

			#endregion
		}

		public abstract class ParallelHybrid : Hybrid
		{
			public override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATSerial, GearboxType.ATPowerSplit };

			#region Overrides of PrimaryBusBase

			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(new TorqueConverterDataAdapter());

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter { get; } = new
				ParallelHybridStrategyParameterDataAdapter();
			#endregion


		}

		public class HEV_P1 : ParallelHybrid
		{

		}

		public class HEV_P2 : ParallelHybrid
		{
			#region Overrides of ParallelHybrid

			public override GearboxType[] SupportedGearboxTypes => new[] { GearboxType.AMT, GearboxType.IHPC };

			#endregion
		}

		public class HEV_P2_5 : ParallelHybrid
		{

		}

		public class HEV_P3 : ParallelHybrid
		{

		}

		public class HEV_P4 : ParallelHybrid
		{

		}

		public abstract class BatteryElectric : PrimaryBusBase
		{
			private readonly IElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();

			public override GearboxType[] SupportedGearboxTypes => new GearboxType[] {  };

            #region Overrides of PrimaryBusBase

			protected override IVehicleDataAdapter VehicleDataAdapter { get; } = new PrimaryBusVehicleDataAdapter_PEV();

            protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IElectricMachinesDataAdapter ElectricMachinesDataAdapter { get; } = new ElectricMachinesDataAdapter();

			protected override IPrimaryBusAuxiliaryDataAdapter AuxDataAdapter { get; } = new PrimaryBusPEVAuxiliaryDataAdapter();

            protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

			#endregion


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

            protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new GearboxDataAdapter(null);
        }

		public class PEV_E3 : BatteryElectric
		{

		}


		public class PEV_E4 : BatteryElectric
		{

		}


		public class PEV_E_IEPC : BatteryElectric
		{
			protected override IGearboxDataAdapter GearboxDataAdapter { get; } = new IEPCGearboxDataAdapter();

		}


		public class Exempted : PrimaryBusBase
		{
			#region Overrides of PrimaryBusBase

			public override GearboxType[] SupportedGearboxTypes => new GearboxType[]{};

			protected override IVehicleDataAdapter VehicleDataAdapter { get; } = new ExemptedPrimaryBusVehicleDataAdapter();
			protected override IEngineDataAdapter EngineDataAdapter => throw new NotImplementedException();

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}

			protected override IPrimaryBusAuxiliaryDataAdapter AuxDataAdapter => throw new NotImplementedException();

			protected override IGearboxDataAdapter GearboxDataAdapter => throw new NotImplementedException();

			protected override IHybridStrategyDataAdapter HybridStrategyDataAdapter => throw new NotImplementedException();

			#endregion
		}
    }
}
