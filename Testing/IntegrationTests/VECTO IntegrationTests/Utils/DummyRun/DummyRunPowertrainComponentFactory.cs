using Moq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunPowertrainComponentFactory : IPowertrainComponentFactory
{
	#region Implementation of IVehicleContainerFactory

	public IVehicleContainer CreateVehicleContainer(VectoRunData runData, IModalDataContainer modData, ISumData writeSumData)
	{
		var retVal = new Mock<IVehicleContainer>();
		var mc = new Mock<IMileageCounter>();
		var vi = new Mock<IVehicleInfo>();
		var gi = new Mock<IGearboxInfo>();
		retVal.Setup(c => c.MileageCounter).Returns(mc.Object);
		retVal.Setup(c => c.VehicleInfo).Returns(vi.Object);
		retVal.Setup(c => c.GearboxInfo(It.IsAny<int>())).Returns(gi.Object);
		retVal.Setup(c => c.ModalData).Returns(modData);
		retVal.Setup(c => c.RunData).Returns(runData);
		retVal.SetupProperty(c => c.RunStatus);
		retVal.Setup(c => c.FinishSimulationRun(It.IsAny<Exception>())).Callback((Exception e) => { modData.Finish(retVal.Object.RunStatus, e); });
		retVal.Setup(c => c.FinishSingleSimulationRun(It.IsAny<Exception>())).Callback((Exception e) => { modData.Finish(retVal.Object.RunStatus, e); });
		mc.Setup(m => m.Distance).Returns(0.SI<Meter>());
		vi.Setup(v => v.VehicleSpeed).Returns(0.KMPHtoMeterPerSecond());
		gi.Setup(g => g.Gear).Returns(new GearshiftPosition(0));
		return retVal.Object;
	}

	public ISimpleVehicleContainer CreateSimpleVehicleContainer(VectoRunData runData)
	{
		return new Mock<ISimpleVehicleContainer>().Object;
	}

	public IDistanceBasedDrivingCycle CreateDistanceBasedDrivingCycle(IVehicleContainer container, IDrivingCycleData cycle)
	{
		throw new NotImplementedException();
	}

	public IMeasuredSpeedDrivingCycle CreateMeasuredSpeedDrivingCycle(IVehicleContainer container, IDrivingCycleData cycle)
	{
		throw new NotImplementedException();
	}

	public IVTPCycle CreateVTPCycle(IVehicleContainer container, IDrivingCycleData cycle)
	{
		throw new NotImplementedException();
	}

	public IVehicle CreateVehicle(IVehicleContainer container, VehicleData modelData, AirdragData airdrag)
	{
		throw new NotImplementedException();
	}

	public IWheels CreateWheels(IVehicleContainer container, Meter rdyn, KilogramSquareMeter totalWheelsInertia)
	{
		throw new NotImplementedException();
	}

	public IDriver CreateDriver(IVehicleContainer container, DriverData driverData, IDriverStrategy strategy)
	{
		throw new NotImplementedException();
	}

	public IDriverStrategy CreateDriverStrategy(IVehicleContainer container)
	{
		throw new NotImplementedException();
	}

	public IBrakes CreateBrakes(IVehicleContainer container)
	{
		throw new NotImplementedException();
	}

	public IElectricPowerJunctionBox CreateElectricPowerJunctionBox(IVehicleContainer container)
	{
		throw new NotImplementedException();
	}

	public ITorqueSplitter CreateTorqueSplitter(IVehicleContainer container, IElectricPowerJunctionBox junctionBox)
	{
		throw new NotImplementedException();
	}

	public IAxlegear CreateAxleGear(IVehicleContainer container, AxleGearData modelData,
		int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
	{
		throw new NotImplementedException();
	}

	public IAngledrive CreateAngledrive(IVehicleContainer container, AngledriveData modelData,
		int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
	{
		throw new NotImplementedException();
	}

	public IRetarder CreateRetarder(IVehicleContainer container, RetarderLossMap lossMap, double ratio, int axleNumber)
	{
		throw new NotImplementedException();
	}

	public IAxlegear CreateAxleGear(IVehicleContainer container, AxleGearData modelData)
	{
		throw new NotImplementedException();
	}

	public IAngledrive CreateAngledrive(IVehicleContainer container, AngledriveData modelData)
	{
		throw new NotImplementedException();
	}

	public IRetarder CreateRetarder(IVehicleContainer container, RetarderLossMap lossMap, double ratio)
	{
		throw new NotImplementedException();
	}

	public IClutchInfo CreateATClutchInfo(IVehicleContainer container, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
	{
		throw new NotImplementedException();
	}

	public IGearbox CreateGearbox(GearboxType gbxType, bool measuredSpeedHybrid, IVehicleContainer container,
		IShiftStrategy strategy)
	{
		throw new NotImplementedException();
	}

	public IClutchInfo CreateATClutchInfo(IVehicleContainer container)
	{
		throw new NotImplementedException();
	}

	public IClutch CreateClutch(VectoSimulationJobType vectoSimulationJobType, IVehicleContainer container,
		CombustionEngineData engineData)
	{
		throw new NotImplementedException();
	}

	public ICombustionEngine CreateCombustionEngine(CycleType cycleType, IVehicleContainer container,
		CombustionEngineData modelData, bool pt1Disabled = false)
	{
		throw new NotImplementedException();
	}

	public IWHRCharger CreateWHRCharger(IVehicleContainer container, double dcDcConverterEfficiency)
	{
		throw new NotImplementedException();
	}

	public IDCDCConverter CreateDCDCConverter(IVehicleContainer container, double efficiency)
	{
		throw new NotImplementedException();
	}

	public ISimpleBattery CreateSimpleBattery(bool smartAlternator, IVehicleContainer container, WattSecond capacity,
		double efficiency)
	{
		throw new NotImplementedException();
	}

	public IBusAuxiliariesAdapter CreateBusAuxiliariesAdapter(IVehicleContainer container, IAuxiliaryConfig auxiliaryConfig,
		IAuxPort additionalAux = null, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
	{
		throw new NotImplementedException();
	}

	public IElectricMotor CreateElectricMotor(bool isIEPC, IVehicleContainer container, ElectricMotorData data,
		IElectricMotorControl control, PowertrainPosition position, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
	{
		throw new NotImplementedException();
	}

	public IPWheelCycle CreatePWheelCycle(IVehicleContainer container, IDrivingCycleData dataCycle)
	{
		throw new NotImplementedException();
	}

	public IElectricMotor CreateElectricMotor(IVehicleContainer container, ElectricMotorData data, IElectricMotorControl control,
		PowertrainPosition position)
	{
		throw new NotImplementedException();
	}

	public IElectricMotor CreateIEPC(IVehicleContainer container, ElectricMotorData data, IElectricMotorControl control,
		PowertrainPosition position)
	{
		throw new NotImplementedException();
	}

	public IElectricChargerPort CreateGensetChargerAdapter(IElectricMotor motor)
	{
		throw new NotImplementedException();
	}

	public IElectricSystem CreateElectricSystem(IVehicleContainer container, BatterySystemData batterySystemData)
	{
		throw new NotImplementedException();
	}

	public IElectricEnergyStorage CreateREESS(REESSType reessType, IVehicleContainer container, SuperCapData modelData)
	{
		throw new NotImplementedException();
	}

	public IElectricEnergyStorage CreateREESS(REESSType reessType, IVehicleContainer container,
		BatterySystemData batterySystemData)
	{
		throw new NotImplementedException();
	}

	public IGearboxInfo CreateDummyGearboxInfo(bool engineOnly, IVehicleContainer container, GearshiftPosition gear = null,
		int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
	{
		throw new NotImplementedException();
	}

	public IAxlegearInfo CreateDummyAxleGearInfo(IVehicleContainer container, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
	{
		throw new NotImplementedException();
	}

	public IEngineInfo CreateDummyEngineInfo(IVehicleContainer container)
	{
		throw new NotImplementedException();
	}

	public IDriverInfo CreateDummyDriverInfo(IVehicleContainer container)
	{
		throw new NotImplementedException();
	}

	public IMileageCounter CreateDummyMileageCounter(IVehicleContainer container)
	{
		throw new NotImplementedException();
	}

	public IPowertrainDrivingCycle CreatePowertrainDrivingCycle(IVehicleContainer container, IDrivingCycleData cycle)
	{
		throw new NotImplementedException();
	}

	public IEngineAuxiliary CreateEngineAuxiliary(IVehicleContainer container)
	{
		throw new NotImplementedException();
	}

	public IHybridControlStrategy CreateHybridStrategy(VectoSimulationJobType jobType, CycleType cycleType, bool atTransmission, bool batteryOnlyMode,
		VectoRunData runData, IVehicleContainer container)
	{
		throw new NotImplementedException();
	}

	public IHybridController CreateHybridController(CycleType cycleType, IVehicleContainer container,
		IHybridControlStrategy strategy, IElectricSystem es)
	{
		throw new NotImplementedException();
	}

	public ISerialHybridController CreateSerialHybridController(CycleType cycleType, IVehicleContainer container,
		IHybridControlStrategy strategy, IElectricSystem es)
	{
		throw new NotImplementedException();
	}

	public IElectricMotorControl CreateElectricMotorController(CycleType cycle, IVehicleContainer container, IElectricSystem es,
		AxlePowertrainData axlePt = null)
	{
		throw new NotImplementedException();
	}

	public IWheelEnd CreateWheelEnd(IVehicleContainer container, WheelEndData modelData)
	{
		throw new NotImplementedException();
	}

	public IFuelCellSystem CreateFuelCellSystem(IVehicleContainer container, FuelCellSystemData modelData)
	{
		throw new NotImplementedException();
	}

	public IElectricMotorControl CreateElectricMotorControllerBatteryOnlyHybrid(CycleType cycle, IVehicleContainer container,
		IElectricSystem es)
	{
		throw new NotImplementedException();
	}

	public ICombustionEngine CreateCombustionEngineBatteryOnlyHybrid(CycleType cycleType, IVehicleContainer container,
		CombustionEngineData modelData, bool pt1Disabled = false)
	{
		throw new NotImplementedException();
	}

	public IGearbox CreateGearboxBatteryOnlyHybrid(VectoSimulationJobType jobType, CycleType cycle, GearboxType gbxType,
		PowertrainPosition emPos, IVehicleContainer container, IShiftStrategy strategy, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
	{
		throw new NotImplementedException();
	}

	public IClutch CreateClutchBatteryOnlyHybrid(VectoSimulationJobType jobType, IVehicleContainer container,
		CombustionEngineData engineData)
	{
		throw new NotImplementedException();
	}

	public IExemptedVehicleContainer CreateExemptedVehicleContainer(VectoRunData runData, IModalDataContainer modData,
		ISumData writeSumData)
	{
		return new Mock<IExemptedVehicleContainer>().Object;
	}

    public IGearbox CreateGearbox(VectoSimulationJobType jobType, CycleType cycle, GearboxType gbxType, IVehicleContainer container, IShiftStrategy strategy, int axleNumber = Constants.NOT_IN_AXLE_POWERTRAIN)
    {
        throw new NotImplementedException();
    }

    #endregion
}