using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public interface IDeclarationDataAdapter
	{
		WheelEndData CreateWheelEndData(VehicleClass vehicleClass, IVehicleDeclarationInputData vehicle);

		VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission first, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> keyValuePair, bool allowVocational);
		
		GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData, GearboxType? overrideGbxType = null);

		ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount);

		RetarderData CreateRetarderData(IRetarderInputData retarderData, ArchitectureID archID,
			IIEPCDeclarationInputData iepcInputData);
				
		AxleGearData CreateAxleGearData(IAxleGearInputData axleGearInputData);

		AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData);

        List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(
			IIEPCDeclarationInputData iepc, Volt averageVoltage);

		IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage, GearList gears = null);

		void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
			VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
			Action<SuperCapData> setSuperCapData);
    }

    public interface ILorryDeclarationDataAdapter : IDeclarationDataAdapter
	{
		DriverData CreateDriverData(Segment segment);

        AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);

		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData, IGearboxDeclarationInputData gbx);
		PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto);

        AirdragData CreateAirdragData(IVehicleDeclarationInputData vehicleData, Mission mission, Segment segment, OvcHevMode ovcMode);

		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);
		
		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType);

		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
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
            ArchitectureID archId);

		FuelCellSystemDeclarationData CreateFuelCells(IFuelCellSystemDeclarationInputData fuelCellSystem);
	}

	public interface IPrimaryBusDeclarationDataAdapter : IDeclarationDataAdapter
	{
		IAuxiliaryConfig CreateBusAuxiliariesData(
			Mission mission, IVehicleDeclarationInputData vehicleData, VectoRunData runData);

		DriverData CreateBusDriverData(Segment segment, VectoSimulationJobType jobType, ArchitectureID arch, CompressorDrive compressorDrive);


        AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData);
		
		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode, Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);

		AirdragData CreateAirdragData(IVehicleDeclarationInputData vehicleData, Mission mission, Segment segment, OvcHevMode ovcMode);

		// serial hybrid strategy
		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType);

		// paralllel hybrid strategy
        HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode,
			LoadingType loading, VehicleClass vehicleClass, MissionType missionType, TableData boostingLimitations,
			GearboxData gearboxData, CombustionEngineData engineData,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> runDataElectricMachinesData,
			ArchitectureID architectureId);
		
		RetarderData CreateGenericRetarderData(IRetarderInputData retarderData, VectoRunData vectoRun);
		
		FuelCellSystemDeclarationData CreateFuelCells(IFuelCellSystemDeclarationInputData fuelCellSystem);
	}

	public interface IGenericCompletedBusDeclarationDataAdapter : IDeclarationDataAdapter
	{
		AirdragData CreateAirdragData(IVehicleDeclarationInputData vehicleData, Mission mission, Segment segment, OvcHevMode ovcMode);
		DriverData CreateBusDriverData(Segment segment, VectoSimulationJobType jobType, ArchitectureID arch, CompressorDrive compressorDrive);
        CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx,
			Mission mission);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);

		IAuxiliaryConfig CreateBusAuxiliariesData(
			Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle, VectoRunData runData);

		// serial hybrid strategy
        HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType);

		// parallel hybrid strategy
        HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode,
			LoadingType loading, VehicleClass vehicleClass, MissionType missionType, TableData boostingLimitations,
			GearboxData gearboxData, CombustionEngineData engineData,
			IList<Tuple<PowertrainPosition, ElectricMotorData>> runDataElectricMachinesData, ArchitectureID architectureId);

		RetarderData CreateGenericRetarderData(IRetarderInputData retarderData, VectoRunData vectoRun);
	}

	public interface ISpecificCompletedBusDeclarationDataAdapter
	{
		IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, VectoRunData runData);
		VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading);

		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType missionType, VehicleClass vehicleClass,
			Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType);

		AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission, Segment segment,
			OvcHevMode ovcMode);

	}

	public interface ISingleBusDeclarationDataAdapter : IDeclarationDataAdapter
	{
		AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission, Segment segment, OvcHevMode ovcMode);
		CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, 
			IEngineModeDeclarationInputData engineMode, Mission mission);
		DriverData CreateBusDriverData(Segment segment, VectoSimulationJobType jobType, ArchitectureID arch, CompressorDrive compressorDrive);

        IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
			IBusAuxiliariesDeclarationData busAuxInput, MissionType mission, VehicleClass segment, Meter vehicleLength,
			int? numSteeredAxles, VectoSimulationJobType jobType);
		IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, IVehicleDeclarationInputData completedVehicle, VectoRunData simulationRunData);

		VehicleData CreateVehicleData(ISingleBusInputDataProvider vehicle, Segment segment, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational);

		// serial hybrid strategy
		HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType);

		// parallel hybrid strategy
        HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
			SuperCapData runDataSuperCapData, Kilogram vehicleMass, OvcHevMode ovcMode,
			LoadingType loading, VehicleClass vehicleClass, MissionType missionType, TableData boostingLimitations,
			GearboxData gearboxData, CombustionEngineData engineData, IList<Tuple<PowertrainPosition, ElectricMotorData>> emData, ArchitectureID architectureId);

		RetarderData CreateGenericRetarderData(IRetarderInputData retarderData, VectoRunData vectoRun);
	}
}