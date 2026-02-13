using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Ninject.PowertrainComponents;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation
{
	/// <summary>
	/// Automatically generated factory for powertrain components, <see cref="PowertrainComponentNinjectModule"/>
	/// </summary>
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

		IDriver CreateDriver(VectoSimulationJobType jobType, int numShiftableGearboxes, IVehicleContainer container, DriverData driverData, IDriverStrategy strategy);

		IDriverStrategy CreateDriverStrategy(VectoSimulationJobType jobType, int numShiftableGearboxes, IVehicleContainer container);

		IBrakes CreateBrakes(IVehicleContainer container);

        IElectricPowerJunctionBox CreateElectricPowerJunctionBox(IVehicleContainer container);

		ITorqueSplitter CreateTorqueSplitter(bool useSimpleTorqueSplitter, IVehicleContainer container, IElectricPowerJunctionBox junctionBox);

        IAxlegear CreateAxleGear(IVehicleContainer container, IPowertrainContainer powertrainContainer,
	        AxleGearData modelData);

		IAngledrive CreateAngledrive(IVehicleContainer container, IPowertrainContainer powertrainContainer,
			AngledriveData modelData);
		
		IRetarder CreateRetarder(IVehicleContainer container, IPowertrainContainer powertrainContainer,
			RetarderLossMap lossMap, double ratio);

		IGearbox CreateGearbox(VectoSimulationJobType jobType, CycleType cycle, GearboxType gbxType,
			IVehicleContainer container, IPowertrainContainer powertrainContainer, IShiftStrategy strategy);

        IClutchInfo CreateATClutchInfo(IVehicleContainer container, IPowertrainContainer powertrainContainer);

        IClutch CreateClutch(VectoSimulationJobType jobType, IVehicleContainer container,
	        IPowertrainContainer powertrainContainer,
	        CombustionEngineData engineData);

		ICombustionEngine CreateCombustionEngine(CycleType cycleType, IVehicleContainer container,
			IPowertrainContainer powertrainContainer, CombustionEngineData modelData,
			bool pt1Disabled = false);

		IWHRCharger CreateWHRCharger(IVehicleContainer container, double efficiency);

		IDCDCConverter CreateDCDCConverter(IVehicleContainer container, double efficiency);

		ISimpleBattery CreateSimpleBattery(bool smartAlternator, IVehicleContainer container, WattSecond capacity, double storageEfficiency);

		IBusAuxiliariesAdapter CreateBusAuxiliariesAdapter(IVehicleContainer container,
			IPowertrainContainer powertrainContainer,
			IAuxiliaryConfig auxiliaryConfig,
			IAuxPort additionalAux = null);
		
		
		IElectricMotor CreateElectricMotor(bool isIEPC,
			IVehicleContainer container,
			IPowertrainContainer powertrainContainer,
			ElectricMotorData data,
			IElectricMotorControl control,
			PowertrainPosition position);
		
		IElectricChargerPort CreateGensetChargerAdapter(IElectricMotor motor);

		IElectricSystem CreateElectricSystem(IVehicleContainer container, BatterySystemData batterySystemData);

		IElectricEnergyStorage CreateREESS(REESSType reessType, IVehicleContainer container, SuperCapData modelData);
		IElectricEnergyStorage CreateREESS(REESSType reessType, IVehicleContainer container, BatterySystemData batterySystemData);

		IGearboxInfo CreateDummyGearboxInfo(bool engineOnly, IVehicleContainer container,
			IPowertrainContainer powertrainContainer, GearshiftPosition gear = null);
		
		IAxlegearInfo CreateDummyAxleGearInfo(IVehicleContainer container, IPowertrainContainer powertrainContainer);

		IEngineInfo CreateDummyEngineInfo(IVehicleContainer container);

		IDriverInfo CreateDummyDriverInfo(IVehicleContainer container);

		IMileageCounter CreateDummyMileageCounter(IVehicleContainer container);

		IPowertrainDrivingCycle CreatePowertrainDrivingCycle(IVehicleContainer container, IDrivingCycleData cycle);

		IEngineAuxiliary CreateEngineAuxiliary(IVehicleContainer container);
		
		IHybridControlStrategy CreateHybridStrategy(VectoSimulationJobType jobType, CycleType cycleType,
			bool atTransmission, bool batteryOnlyMode, VectoRunData runData, IVehicleContainer container,
			IPowertrainContainer powertrainContainer);
		
		IHybridController CreateHybridController(CycleType cycleType,
			IVehicleContainer container, IPowertrainContainer powertrainContainer, IHybridControlStrategy strategy,
			IElectricSystem es);
		
		ISerialHybridController CreateSerialHybridController(CycleType cycleType,
			IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es);

		IElectricMotorControl CreateElectricMotorController(CycleType cycle, IVehicleContainer container,
			IPowertrainContainer powertrainContainer, IElectricSystem es, AxlePowertrainData axlePt = null);

        IWheelEnd CreateWheelEnd(IVehicleContainer container, WheelEndData modelData);

		IFuelCellSystem CreateFuelCellSystem(IVehicleContainer container, FuelCellSystemData modelData);

		IElectricMotorControl CreateElectricMotorControllerBatteryOnlyHybrid(CycleType cycle, IVehicleContainer container, IPowertrainContainer powertrainContainer, IElectricSystem es);
		
		ICombustionEngine CreateCombustionEngineBatteryOnlyHybrid(CycleType cycleType, IVehicleContainer container, IPowertrainContainer powertrainContainer, CombustionEngineData modelData,
			bool pt1Disabled = false);

		IGearbox CreateGearboxBatteryOnlyHybrid(VectoSimulationJobType jobType, CycleType cycle, GearboxType gbxType, PowertrainPosition emPos, IVehicleContainer container, IPowertrainContainer powertrainContainer, IShiftStrategy strategy);

		IClutch CreateClutchBatteryOnlyHybrid(VectoSimulationJobType jobType, IVehicleContainer container, IPowertrainContainer powertrainContainer,
            CombustionEngineData engineData);

		

		IPowertrainContainer CreatePowertrainContainer(IVehicleContainer vehicleContainer, AxleNumber axleNumber);

		/// <summary>
		/// Creates the powertrain container for the single axle powertrain (axleNumber == -1)
		/// </summary>
		/// <returns></returns>
		IPowertrainContainer CreatePowertrainContainer(IVehicleContainer vehicleContainer);
		ISimplePowertrainContainer CreateSimplePowertrainContainer(ISimpleVehicleContainer simpleVehicleContainer, AxleNumber axleNumber, bool useSimpleTorqueSplitter = false);
		ISimplePowertrainContainer CreateSimplePowertrainContainer(ISimpleVehicleContainer simpleVehicleContainer);
	}

    public interface IIEPCGearboxFactory
	{
		IGearbox CreateIEPCGearbox(bool singleSpeed, IVehicleContainer container, IShiftStrategy strategy);
	}

}