using Moq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunNinjectModule : AbstractNinjectModule
{
    #region Overrides of NinjectModule

    public override void Load()
    {
        Rebind<ISimulatorFactory>().To<DummyRunDeclarationSimulatorFactory>().Named(ExecutionMode.Declaration.ToString());

        Rebind<IModalDataFactory>().To<DummyRunModDataFactory>().InSingletonScope();

        Rebind<IVectoRunDataFactoryFactory>().To<DummyRunRunDataFactoryFactory>();
        Rebind<IVectoRunDataFactoryFactory>().To<DummyRunRunDataFactoryFactory>();
        Rebind<IPowertrainBuilder>().To<DummyRunPowertrainBuilder>().InSingletonScope();
		Rebind<IPowertrainComponentFactory>().To<DummyRunPowertrainComponentFactory>().InSingletonScope();
		//Rebind<IVehicleContainerFactory>().To<DummyRunVehicleContainerFactory>().InSingletonScope();
	}

    #endregion
}

public class DummyRunModDataFactory : IModalDataFactory
{
    #region Implementation of IModalDataFactory

    public IModalDataContainer CreateModDataContainer(VectoRunData runData, IModalDataWriter writer, Action<IModalDataContainer> addReportResult,
        IModalDataFilter[] filter)
    {
        var modData = GetMockModData(VectoRun.Status.Success, runData.EngineData?.Fuels?.Select(x => x.FuelData.FuelType).ToArray() ?? null);

        var modMock = Mock.Get(modData);
        var runStatus = VectoRun.Status.Pending;
        Exception ex = null;
        modMock.SetupGet(m => m.RunStatus).Returns(() => runStatus);
        modMock.SetupGet(m => m.Error).Returns(() => ex?.Message);
        modMock.SetupGet(m => m.StackTrace).Returns(() => ex == null ? null : ex.StackTrace ?? ex.InnerException.StackTrace);
        modMock.Setup(m => m.Finish(It.IsAny<VectoRun.Status>(), It.IsAny<Exception>()))
            .Callback((VectoRun.Status s, Exception e) =>
            {
                runStatus = s;
                ex = e;
                addReportResult(modMock.Object);
            });

        if (runData.VehicleData.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle && runData.JobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle, VectoSimulationJobType.IEPC_E))
        {
            //modMock.Setup(x => x.CorrectedModalData).Returns(new PEVCorrectedModalData(modData));
            var mc = Mock.Get(modData.CorrectedModalData);
            mc.Setup(x => x.FuelCorrection).Returns(new Dictionary<FuelType, IFuelConsumptionCorrection>());
        }

        return modData;
    }

    #endregion

    public IModalDataContainer GetMockModData(VectoRun.Status runStatus, FuelType[] fuelTypes, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
    {
        var fuels = fuelTypes == null || fuelTypes.Length == 0 ? new[] { FuelType.DieselCI } : fuelTypes;

        var modData = new Mock<IModalDataContainer>();
        modData.Setup(x => x.RunStatus).Returns(runStatus);
        modData.Setup(x => x.Duration).Returns(3600.SI<Second>());
        modData.Setup(x => x.Distance).Returns(30000.SI<Meter>());
        modData.Setup(x => x.GetValues<MeterPerSecond>(ModalResultField.v_act)).Returns(new[] { 0.KMPHtoMeterPerSecond(), 50.KMPHtoMeterPerSecond() });
        modData.Setup(x => x.GetValues<MeterPerSquareSecond>(ModalResultField.acc)).Returns(new[] { -1.SI<MeterPerSquareSecond>(), 0.SI<MeterPerSquareSecond>(), 1.SI<MeterPerSquareSecond>() });
        modData.Setup(x => x.GetValues<uint>(ModalResultField.Gear)).Returns(new[] { 0u, 2u, 0u, 3u, 0u });

        var e_gbxIn = 1000.SI<WattSecond>();
        var gbxEff = 0.98;
        var axlEff = 0.97;
        modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn);
        modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff);
        modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff * axlEff);
		modData.Setup(x => x.GetValues<SI>(ModalResultField.REESSStateOfCharge))
			.Returns(() => new[] { 50.SI(), 50.SI() });
        if (runStatus != VectoRun.Status.Success) {
            modData.Setup(x => x.Error).Returns("TestCase Error!");
            modData.Setup(x => x.StackTrace).Returns("Testcase Stacktrace");
        }

        var mc = new Mock<ICorrectedModalData>();
        modData.Setup(x => x.CorrectedModalData).Returns(mc.Object);

        var fcCorrected = new Dictionary<FuelType, IFuelConsumptionCorrection>();
        var ovcFactor = ovcMode == OvcHevMode.ChargeDepleting ? 0.1 : 1.0;
        foreach (var fuelType in fuels) {
            var factor = fcCorrected.Count == 0 ? 1 : 0.1;
            var fc = new Mock<IFuelConsumptionCorrection>();
            fc.Setup(x => x.Fuel).Returns(DeclarationData.FuelData.Lookup(fuelType, TankSystem.Liquefied));
            fc.Setup(x => x.TotalFuelConsumptionCorrected).Returns(31.SI<Kilogram>() * factor * ovcFactor);
            fc.Setup(x => x.EnergyDemand).Returns(31.SI<Kilogram>() * factor * ovcFactor * FuelData.Diesel.LowerHeatingValueVecto);
            fc.Setup(x => x.FC_AUXHTR_KM).Returns(0.SI<KilogramPerMeter>());
            fcCorrected.Add(fuelType, fc.Object);
        }
        mc.Setup(x => x.FuelCorrection).Returns(fcCorrected);

        mc.Setup(x => x.CO2Total).Returns(20.SI<Kilogram>());
        mc.Setup(x => x.FuelEnergyConsumptionTotal).Returns(1e9.SI<Joule>());

        var elOvcFactor = ovcMode == OvcHevMode.ChargeSustaining ? 0 : 1.0;
        mc.Setup(x => x.ElectricEnergyConsumption_Final).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);
        mc.Setup(x => x.ElectricEnergyConsumption_SoC).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);
        mc.Setup(x => x.ElectricEnergyConsumption_SoC_Corr).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);

        return modData.Object;
    }

}

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
        retVal.Setup(c => c.GearboxInfo).Returns(gi.Object);
        retVal.Setup(c => c.ModalData).Returns(modData);
        retVal.Setup(c => c.RunData).Returns(runData);
        retVal.SetupProperty(c => c.RunStatus);
        retVal.Setup(c => c.FinishSimulationRun(It.IsAny<Exception>())).Callback((Exception e) => { modData.Finish(retVal.Object.RunStatus, e); });
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

	public IGearbox CreateGearbox(VectoSimulationJobType jobType, CycleType cycle, GearboxType gbxType,
		IVehicleContainer container, IShiftStrategy strategy)
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
		IAuxPort additionalAux = null)
	{
		throw new NotImplementedException();
	}

	public IElectricMotor CreateElectricMotor(bool isIEPC, IVehicleContainer container, ElectricMotorData data,
		IElectricMotorControl control, PowertrainPosition position)
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

	public IGearboxInfo CreateDummyGearboxInfo(bool engineOnly, IVehicleContainer container, GearshiftPosition gear = null)
	{
		throw new NotImplementedException();
	}

	public IAxlegearInfo CreateDummyAxleGearInfo(IVehicleContainer container)
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

	public IHybridControlStrategy CreateHybridStrategy(VectoSimulationJobType jobType, CycleType cycleType, bool atTransmission,
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

	public IElectricMotorControl CreateElectricMotorController(CycleType cycle, IVehicleContainer container, IElectricSystem es)
	{
		throw new NotImplementedException();
	}

	public IWheelEnd CreateWheelEnd(IVehicleContainer container, WheelEndData modelData)
	{
		throw new NotImplementedException();
	}

	public IExemptedVehicleContainer CreateExemptedVehicleContainer(VectoRunData runData, IModalDataContainer modData,
        ISumData writeSumData)
    {
        return new Mock<IExemptedVehicleContainer>().Object;
    }

    #endregion
}