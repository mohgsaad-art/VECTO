using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation
{
    public interface IPowertrainComponentFactory
	{
		IVehicleContainer CreateVehicleContainer(VectoRunData runData, IModalDataContainer modData,
			ISumData writeSumData);

		IExemptedVehicleContainer CreateExemptedVehicleContainer(VectoRunData runData, IModalDataContainer modData,
			ISumData writeSumData);

		ISimpleVehicleContainer CreateSimpleVehicleContainer(VectoRunData runData);

        IDistanceBasedDrivingCycle CreateDistanceBasedDrivingCycle(IVehicleContainer container, IDrivingCycleData cycle);
		IMeasuredSpeedDrivingCycle CreateMeasuredSpeedDrivingCycle(IVehicleContainer container, IDrivingCycleData cycle);
		IPWheelCycle CreatePWheelCycle(IVehicleContainer container, IDrivingCycleData cycle);
		IVTPCycle CreateVTPCycle(IVehicleContainer container, IDrivingCycleData cycle);

        IVehicle CreateVehicle(IVehicleContainer container, VehicleData modelData, AirdragData airdrag);

		IWheels CreateWheels(IVehicleContainer container, Meter rdyn, KilogramSquareMeter totalWheelsInertia);

		IDriver CreateDriver(IVehicleContainer container, DriverData driverData, IDriverStrategy strategy);

		IDriverStrategy CreateDriverStrategy(IVehicleContainer container);

		IBrakes CreateBrakes(IVehicleContainer container);

		IAxlegear CreateAxleGear(IVehicleContainer container, AxleGearData modelData);

		IAngledrive CreateAngledrive(IVehicleContainer container, AngledriveData modelData);
		
		IRetarder CreateRetarder(IVehicleContainer container, RetarderLossMap lossMap, double ratio);

		IGearbox CreateGearbox(VectoSimulationJobType jobType, CycleType cycle, GearboxType gbxType, IVehicleContainer container, IShiftStrategy strategy);

        IClutchInfo CreateATClutchInfo(IVehicleContainer container);

        IClutch CreateClutch(VectoSimulationJobType jobType, IVehicleContainer container,
			CombustionEngineData engineData);

		ICombustionEngine CreateCombustionEngine(CycleType cycleType, IVehicleContainer container, CombustionEngineData modelData,
			bool pt1Disabled = false);

		IWHRCharger CreateWHRCharger(IVehicleContainer container, double dcDcConverterEfficiency);

		IDCDCConverter CreateDCDCConverter(IVehicleContainer container, double efficiency);

		ISimpleBattery CreateSimpleBattery(bool smartAlternator, IVehicleContainer container, WattSecond capacity, double storageEfficiency);

		IBusAuxiliariesAdapter CreateBusAuxiliariesAdapter(IVehicleContainer container, IAuxiliaryConfig auxiliaryConfig, IAuxPort additionalAux = null);
		
		
		IElectricMotor CreateElectricMotor(bool isIEPC, IVehicleContainer container, ElectricMotorData data, IElectricMotorControl control, PowertrainPosition position);
		
		IElectricChargerPort CreateGensetChargerAdapter(IElectricMotor motor);

		IElectricSystem CreateElectricSystem(IVehicleContainer container, BatterySystemData batterySystemData);

		IElectricEnergyStorage CreateREESS(REESSType reessType, IVehicleContainer container, SuperCapData modelData);
		IElectricEnergyStorage CreateREESS(REESSType reessType, IVehicleContainer container, BatterySystemData batterySystemData);

		IGearboxInfo CreateDummyGearboxInfo(bool engineOnly, IVehicleContainer container, GearshiftPosition gear = null);
		
		IAxlegearInfo CreateDummyAxleGearInfo(IVehicleContainer container);

		IEngineInfo CreateDummyEngineInfo(IVehicleContainer container);

		IDriverInfo CreateDummyDriverInfo(IVehicleContainer container);

		IMileageCounter CreateDummyMileageCounter(IVehicleContainer container);

		IPowertrainDrivingCycle CreatePowertrainDrivingCycle(IVehicleContainer container, IDrivingCycleData cycle);

		IEngineAuxiliary CreateEngineAuxiliary(IVehicleContainer container);
		
		IHybridControlStrategy CreateHybridStrategy(VectoSimulationJobType jobType, CycleType cycleType,
			bool atTransmission, VectoRunData runData, IVehicleContainer container);
		
		IHybridController CreateHybridController(CycleType cycleType,
			IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es);
		
		ISerialHybridController CreateSerialHybridController(CycleType cycleType,
			IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es);

		IElectricMotorControl CreateElectricMotorController(CycleType cycle, IVehicleContainer container, IElectricSystem es);

		IWheelEnd CreateWheelEnd(IVehicleContainer container, WheelEndData modelData);
	}

	public interface IIEPCGearboxFactory
	{
		IGearbox CreateIEPCGearbox(bool singleSpeed, IVehicleContainer container, IShiftStrategy strategy);
	}

}