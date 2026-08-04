using Moq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.Utils;

namespace TUGraz.Vecto.IntegrationTests.Utils.DummyRun;

public class DummyRunModDataFactory : IModalDataFactory
{
	#region Implementation of IModalDataFactory

	public IModalDataContainer CreateModDataContainer(VectoRunData runData, IModalDataWriter writer, Action<IModalDataContainer> addReportResult,
		IModalDataFilter[] filter)
	{
		var fuels = runData.JobType.IsOneOf(VectoSimulationJobType.FCHV, VectoSimulationJobType.FCHV_IEPC, VectoSimulationJobType.Multiple_FCHV) ?
				new [] {FuelType.H2FC }
			: runData.EngineData?.Fuels?.Select(x => x.FuelData.FuelType).ToArray() ?? null;

        var modData = GetMockModData(VectoRun.Status.Success, fuels, runData);

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

	public IModalDataContainer GetMockModData(VectoRun.Status runStatus, FuelType[] fuelTypes,
		VectoRunData vectoRunData, OvcHevMode ovcMode = OvcHevMode.NotApplicable)
	{
		var isPrimaryBusPEV =
			vectoRunData.JobType.IsBatteryElectric() && vectoRunData.VehicleData.VehicleClass.IsPrimaryBus();
		var fuels = isPrimaryBusPEV ? new FuelType[] { } :
			fuelTypes == null || fuelTypes.Length == 0 ? new[] { FuelType.DieselCI } : fuelTypes;

		var modData = new Mock<IModalDataContainer>();
		modData.Setup(x => x.RunStatus).Returns(runStatus);
		modData.Setup(x => x.Duration).Returns(3600.SI<Second>());
		modData.Setup(x => x.Distance).Returns(30000.SI<Meter>());
		modData.Setup(x => x.GetValues<MeterPerSecond>(ModalResultField.v_act)).Returns(new[] { 0.KMPHtoMeterPerSecond(), 50.KMPHtoMeterPerSecond() });
		modData.Setup(x => x.GetValues<MeterPerSquareSecond>(ModalResultField.acc)).Returns(new[] { -1.SI<MeterPerSquareSecond>(), 0.SI<MeterPerSquareSecond>(), 1.SI<MeterPerSquareSecond>() });

        var e_gbxIn = 1000.SI<WattSecond>();
        var e_retarderIn = 980.SI<WattSecond>();
		var e_angleIn = 980.SI<WattSecond>();
        var gbxEff = 0.98;
        var axlEff = 0.97;

        if (!vectoRunData.AxlePowertrains.Any())
		{
			modData.Setup(x => x.GetValues<uint>(ModalResultField.Gear, Constants.NOT_IN_AXLE_POWERTRAIN.FormatAxleNumber())).Returns(new[] { 0u, 2u, 0u, 3u, 0u });
            modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_retarder_in, Constants.NOT_IN_AXLE_POWERTRAIN, It.IsNotNull<Func<SI, bool>>())).Returns(e_retarderIn);
            modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, Constants.NOT_IN_AXLE_POWERTRAIN, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn);
            modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, Constants.NOT_IN_AXLE_POWERTRAIN, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff);
        }
        else
		{
			foreach (var axlePt in vectoRunData.AxlePowertrains)
			{
                modData.Setup(x => x.GetValues<uint>(ModalResultField.Gear, axlePt.AxleNumber.FormatAxleNumber())).Returns(new[] { 0u, 2u, 0u, 3u, 0u });
                modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_retarder_in, axlePt.AxleNumber, It.IsNotNull<Func<SI, bool>>())).Returns(e_retarderIn);
                modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, axlePt.AxleNumber, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn);
                modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, axlePt.AxleNumber, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff);
                modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_angle_in, axlePt.AxleNumber, It.IsNotNull<Func<SI, bool>>())).Returns(e_angleIn);
            }
        }
		
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