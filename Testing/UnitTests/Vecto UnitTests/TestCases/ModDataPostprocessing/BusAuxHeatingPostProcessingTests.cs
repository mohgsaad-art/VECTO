using Moq;
using NUnit.Framework;
using TUGraz.Vecto.UnitTests.Utils.MockInputData;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModDataPostprocessing;

public class BusAuxHeatingPostProcessingTests
{
    private const BusHVACSystemConfiguration CFG1 = BusHVACSystemConfiguration.Configuration1;
    private const BusHVACSystemConfiguration CFG2 = BusHVACSystemConfiguration.Configuration2;
    private const BusHVACSystemConfiguration CFG3 = BusHVACSystemConfiguration.Configuration3;
    private const BusHVACSystemConfiguration CFG4 = BusHVACSystemConfiguration.Configuration4;
    private const BusHVACSystemConfiguration CFG5 = BusHVACSystemConfiguration.Configuration5;
    private const BusHVACSystemConfiguration CFG6 = BusHVACSystemConfiguration.Configuration6;
    private const BusHVACSystemConfiguration CFG7 = BusHVACSystemConfiguration.Configuration7;
    private const BusHVACSystemConfiguration CFG8 = BusHVACSystemConfiguration.Configuration8;
    private const BusHVACSystemConfiguration CFG9 = BusHVACSystemConfiguration.Configuration9;
    private const BusHVACSystemConfiguration CFG10 = BusHVACSystemConfiguration.Configuration10;

    private const HeatPumpType HeatPumpNone = HeatPumpType.none;
    private const HeatPumpType HeatPumpR744 = HeatPumpType.R_744;
    private const HeatPumpType HeatPump2Stage = HeatPumpType.non_R_744_2_stage;
    private const HeatPumpType HeatPump3Stage = HeatPumpType.non_R_744_3_stage;
    private const HeatPumpType HeatPumpCont = HeatPumpType.non_R_744_continuous;
    private const HeaterType NoElHtr = HeaterType.None;
    private const HeaterType AirElHtr = HeaterType.AirElectricHeater;
    private const HeaterType WaterElHtr = HeaterType.WaterElectricHeater;
    private const HeaterType OthrElHtr = HeaterType.OtherElectricHeating;

    private const double AuxHtrPwr0 = 0.0;
    private const double AuxHtrPwr30 = 30.0e3;

    private const string FuelMap = "engine speed, torque, fuel consumption\n" +
                                        "500,-31,0\n" +
                                        "500,0,500\n" +
                                        "500,1000,24000\n" +
                                        "2500,-109,0\n" +
                                        "2500,0,3800\n" +
                                        "2500,1000,32000\n";

    protected static readonly IFuelProperties Fuel = FuelData.Diesel;



    [
        TestCase("a", CFG1, HeatPumpNone, HeatPumpNone, 0.0, AuxHtrPwr0, 0, 0, 0, 0, 0, 0, 0, 0),
        TestCase("b", CFG1, HeatPumpNone, HeatPumpNone, 0.0, AuxHtrPwr30, 0, 0, 0, 0, 0, 0, 9415850.1, 0.2205117),
        TestCase("c", CFG1, HeatPumpNone, HeatPumpNone, 4.0, AuxHtrPwr30, 0, 0, 0, 0, 0, 0, 697201.7, 0.0163279),
        TestCase("d", CFG1, HeatPumpNone, HeatPumpNone, 10.0, AuxHtrPwr30, 0, 0, 0, 0, 0, 0, 0, 0),

        TestCase("d", CFG2, HeatPump2Stage, HeatPumpNone, 0.0, AuxHtrPwr0, 0.000, 0.000000, 99.712643, 0.001496, 0, 0, 0.0, 0.0000000),
        TestCase("e", CFG2, HeatPump2Stage, HeatPumpNone, 0.0, AuxHtrPwr30, 0.000, 0.000000, 81.370822, 0.001221, 0, 0, 8646116.9, 0.2024852),
        TestCase("f", CFG2, HeatPump2Stage, HeatPumpNone, 4.0, AuxHtrPwr30, 0.000, 0.000000, 5.211176, 0.000078, 0, 0, 661088.2, 0.0154822),

        TestCase("g", CFG6, HeatPumpNone, HeatPump3Stage, 0.0, AuxHtrPwr0, 0.000, 0.000000, 947.372508, 0.014211, 0, 0, 0.0, 0.0000000),
        TestCase("h", CFG6, HeatPumpNone, HeatPump3Stage, 0.0, AuxHtrPwr30, 0.000, 0.000000, 774.516305, 0.011618, 0, 0, 1718517.8, 0.0402463),
        TestCase("i", CFG6, HeatPumpNone, HeatPump3Stage, 4.0, AuxHtrPwr30, 0.000, 0.000000, 48.934215, 0.000734, 0, 0, 336067.2, 0.0078704),
        TestCase("j", CFG6, HeatPumpNone, HeatPumpCont, 0.0, AuxHtrPwr30, 1038.910, 0.015584, 0.000000, 0.000000, 0, 0, 1718517.8, 0.0402463),

        TestCase("k", CFG7, HeatPump2Stage, HeatPump3Stage, 0.0, AuxHtrPwr0, 0.000, 0.000000, 952.347901, 0.014285, 0, 0, 0.0, 0.0000000),
        TestCase("l", CFG7, HeatPump2Stage, HeatPump3Stage, 0.0, AuxHtrPwr30, 0.000, 0.000000, 778.435497, 0.011677, 0, 0, 1718517.8, 0.0402463),
        TestCase("m", CFG7, HeatPump2Stage, HeatPump3Stage, 4.0, AuxHtrPwr30, 0.000, 0.000000, 49.251969, 0.000739, 0, 0, 336067.2, 0.0078704),
        TestCase("n", CFG7, HeatPumpCont, HeatPumpCont, 4.0, AuxHtrPwr30, 64.408, 0.000966, 0.000000, 0.000000, 0, 0, 336067.2, 0.0078704),
    ]
    public void TestModDataPostprocessing_Conventional(string sort, BusHVACSystemConfiguration cfg, HeatPumpType driverHpHeating,
        HeatPumpType passengerHpHeating, double fuelConsumptionKg, double auxhHeaterPwr,
        double expectedHpHeatingElPwrMechW, double expectedFcHPHeatingElKg, double expectedHpHeatingMechPwrW,
        double expectedFcHPHeatingMechKg, double expectedElectricHeaterPwrW,
        double expectedFcElectricHeaterKg, double expectedAuxHeatingDemandJ, double expectedFcAuxHeaterKg)
    {
		var busAux = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatPumpHeating: driverHpHeating,
            passengerHeatPumpHeating: passengerHpHeating, auxHeaterPower: auxhHeaterPwr.SI<Watt>(), electricHeater: NoElHtr);
        var runData = GetVectoRunDataConventional(busAux);
        var mockModData = GetMockModData(runData, fuelConsumptionKg.SI<Kilogram>());

		var correction = new ConventionalModalDataPostprocessingCorrection();
        var corrected = correction.ApplyCorrection(mockModData, runData);
        var duration = mockModData.Duration.Value();

        Assert.NotNull(corrected);
        var fcCorr = corrected.FuelConsumptionCorrection(Fuel) as FuelConsumptionCorrection;
        Assert.NotNull(fcCorr);

        Console.WriteLine(new[] { (corrected.WorkBusAuxHeatPumpHeatingElMech / duration).Value().ToString("F3"),
            fcCorr.FcHeatPumpHeatingEl.Value().ToString("F6"),
            (corrected.WorkBusAuxHeatPumpHeatingMech.Value() / duration).ToString("F3"),
            fcCorr.FcHeatPumpHeatingMech.Value().ToString("F6"),
            (corrected.WorkBusAuxElectricHeater / duration).Value().ToString("F3"),
            fcCorr.FcBusAuxEletcricHeater.Value().ToString("F6"),
            corrected.AuxHeaterDemand.Value().ToString("F1"),
            fcCorr.FcAuxHtr.Value().ToString("F6")
            }.Join(", "));

        Assert.AreEqual(expectedHpHeatingElPwrMechW, (corrected.WorkBusAuxHeatPumpHeatingElMech / duration).Value(), 1e-3, "Heatpump Power El Mech");
        Assert.AreEqual(expectedHpHeatingMechPwrW, (corrected.WorkBusAuxHeatPumpHeatingMech / duration).Value(), 1e-3, "Heatpump Power Mech");
        Assert.AreEqual(expectedFcHPHeatingElKg, fcCorr.FcHeatPumpHeatingEl.Value(), 1e-6, "FC heatpump heating El");
        Assert.AreEqual(expectedFcHPHeatingMechKg, fcCorr.FcHeatPumpHeatingMech.Value(), 1e-6, "FC heatpump heating Mech");
        Assert.AreEqual(expectedElectricHeaterPwrW, (corrected.WorkBusAuxElectricHeater / duration).Value(), 1e-3, "expectedElectricHeaterDemand");
        Assert.AreEqual(expectedFcElectricHeaterKg, fcCorr.FcBusAuxEletcricHeater.Value(), 1e-6, "expectedFcElectricHeater");
        Assert.AreEqual(expectedAuxHeatingDemandJ, corrected.AuxHeaterDemand.Value(), 1e-1, "expectedAuxHeatingDemand");
        Assert.AreEqual(expectedFcAuxHeaterKg, fcCorr.FcAuxHtr.Value(), 1e-6, "expectedFcAuxHeater");

        var expectedFc = fuelConsumptionKg + expectedFcHPHeatingElKg + expectedFcHPHeatingMechKg +
                        expectedFcElectricHeaterKg + expectedFcAuxHeaterKg;
        Assert.AreEqual(expectedFc, fcCorr.TotalFuelConsumptionCorrected.Value(), 1e-6, "TotalFuelConsumptionCorrected");
    }

    // =========================================
    // =========================================

    // TODO MQ 2022-11-24: Extend Test and assert electric consumption / range
    [
        TestCase("a", CFG1, HeatPumpNone, HeatPumpNone, NoElHtr, 0.0, AuxHtrPwr0, 0, 0, 0, 0, 0, 0, 0, 0),
        TestCase("b", CFG1, HeatPumpNone, HeatPumpNone, AirElHtr, 0.0, AuxHtrPwr30, 0.000, 0.0, 0.000, 0.0, 16.776, 0.0, 9345644.6, 0.0),
        TestCase("c", CFG1, HeatPumpNone, HeatPumpNone, WaterElHtr, 4.0, AuxHtrPwr30, 0.000, 0.0, 0.000, 0.0, 13.698, 0.0, 3879017.3, 0.0),
        TestCase("d", CFG1, HeatPumpNone, HeatPumpNone, OthrElHtr, 10.0, AuxHtrPwr30, 0.000, 0.0, 0.000, 0.0, 9.082, 0.0, 804228.6, 0.0),

        TestCase("e", CFG2, HeatPumpCont, HeatPumpNone, AirElHtr, 0.0, AuxHtrPwr0, 126.798, 0.0, 0.000, 0.0, 2033.302, 0.0, 0.0, 0.0),
        TestCase("f", CFG2, HeatPumpCont, HeatPumpNone, WaterElHtr, 0.0, AuxHtrPwr30, 103.891, 0.0, 0.000, 0.0, 16.776, 0.0, 8575911.3, 0.0),
        TestCase("g", CFG2, HeatPumpCont, HeatPumpNone, OthrElHtr, 4.0, AuxHtrPwr0, 58.417, 0.0, 0.000, 0.0, 853.375, 0.0, 0.0, 0.0),
        TestCase("h", CFG2, HeatPumpCont, HeatPumpNone, OthrElHtr, 4.0, AuxHtrPwr30, 43.749, 0.0, 0.000, 0.0, 13.698, 0.0, 3600615.7, 0.0),

        TestCase("i", CFG6, HeatPumpNone, HeatPumpCont, NoElHtr, 0.0, AuxHtrPwr0, 1267.984, 0.0, 0.000, 0.0, 0.000, 0.0, 0.0, 0.0),
        TestCase("j", CFG6, HeatPumpNone, HeatPumpCont, AirElHtr, 0.0, AuxHtrPwr30, 1038.910, 0.0, 0.000, 0.0, 16.776, 0.0, 1648312.3, 0.0),
        TestCase("k", CFG6, HeatPumpNone, HeatPumpCont, WaterElHtr, 4.0, AuxHtrPwr30, 437.492, 0.0, 0.000, 0.0, 13.698, 0.0, 1095001.3, 0.0),
        TestCase("l", CFG6, HeatPumpNone, HeatPumpCont, WaterElHtr, 4.0, AuxHtrPwr0, 584.167, 0.0, 0.000, 0.0, 68.490, 0.0, 0.0, 0.0),

        TestCase("o", CFG7, HeatPumpR744, HeatPumpCont, NoElHtr, 0.0, AuxHtrPwr0, 1263.261, 0.0, 0.000, 0.0, 0.000, 0.0, 0.0, 0.0),
        TestCase("p", CFG7, HeatPumpR744, HeatPumpCont, AirElHtr, 0.0, AuxHtrPwr30, 1033.300, 0.0, 0.000, 0.0, 16.776, 0.0, 1634271.2, 0.0),
        TestCase("q", CFG7, HeatPumpR744, HeatPumpCont, WaterElHtr, 4.0, AuxHtrPwr0, 579.983, 0.0, 0.000, 0.0, 65.066, 0.0, 0.0, 0.0),
        TestCase("r", CFG7, HeatPumpR744, HeatPumpCont, WaterElHtr, 4.0, AuxHtrPwr30, 434.624, 0.0, 0.000, 0.0, 13.698, 0.0, 1083536.0, 0.0),
        TestCase("s", CFG7, HeatPumpCont, HeatPumpCont, OthrElHtr, 4.0, AuxHtrPwr0, 584.167, 0.0, 0.000, 0.0, 68.490, 0.0, 0.0, 0.0),
        TestCase("t", CFG7, HeatPumpCont, HeatPumpCont, OthrElHtr, 4.0, AuxHtrPwr30, 437.492, 0.0, 0.000, 0.0, 13.698, 0.0, 1095001.3, 0.0),
    ]
    public void TestModDataPostprocessing_PEV(string sort, BusHVACSystemConfiguration cfg, HeatPumpType driverHpHeating,
        HeatPumpType passengerHpHeating, HeaterType heater, double emLossKwH, double auxhHeaterPwr,
        double expectedHpHeatingElPwrMechW, double expectedFcHPHeatingElKg, double expectedHpHeatingMechPwrW,
        double expectedFcHPHeatingMechKg, double expectedElectricHeaterPwrW,
        double expectedFcElectricHeaterKg, double expectedAuxHeatingDemandJ, double expectedFcAuxHeaterKg)
    {
        var busAux = GetAuxParametersParallelHybridSingleDeckForTest(cfg, driverHeatPumpHeating: driverHpHeating,
            passengerHeatPumpHeating: passengerHpHeating, auxHeaterPower: auxhHeaterPwr.SI<Watt>(), electricHeater: heater);
        var runData = GetVectoRunDataBatteryElectric(busAux);
        var mockModData = GetMockModData(runData, emLoss: emLossKwH.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>());

        var corrected = new BatteryElectricPostprocessingCorrection().ApplyCorrection(mockModData, runData);

        var duration = mockModData.Duration.Value();

        Assert.NotNull(corrected);
        var fcCorr = corrected.FuelConsumptionCorrection(Fuel) as NoFuelConsumptionCorrection;
        Assert.NotNull(fcCorr);

        Console.WriteLine(new[] { (corrected.WorkBusAuxHeatPumpHeatingElMech / duration).Value().ToString("F3"),
            "0.0", //fcCorr.FcHeatPumpHeatingEl.Value().ToString("F6"),
			(corrected.WorkBusAuxHeatPumpHeatingMech.Value() / duration).ToString("F3"),
            "0.0",//fcCorr.FcHeatPumpHeatingMech.Value().ToString("F6"),
			(corrected.WorkBusAuxElectricHeater / duration).Value().ToString("F3"),
            "0.0",//fcCorr.FcBusAuxEletcricHeater.Value().ToString("F6"),
			corrected.AuxHeaterDemand.Value().ToString("F1"),
            "0.0",//fcCorr.FcAuxHtr.Value().ToString("F6")
			}.Join(", "));

        Assert.AreEqual(expectedHpHeatingElPwrMechW, (corrected.WorkBusAuxHeatPumpHeatingElMech / duration).Value(), 1e-3, "Heatpump Power El Mech");
        Assert.AreEqual(expectedHpHeatingMechPwrW, (corrected.WorkBusAuxHeatPumpHeatingMech / duration).Value(), 1e-3, "Heatpump Power Mech");
        //Assert.AreEqual(expectedFcHPHeatingElKg, fcCorr.FcHeatPumpHeatingEl.Value(), 1e-6, "FC heatpump heating El");
        //Assert.AreEqual(expectedFcHPHeatingMechKg, fcCorr.FcHeatPumpHeatingMech.Value(), 1e-6, "FC heatpump heating Mech");
        Assert.AreEqual(expectedElectricHeaterPwrW, (corrected.WorkBusAuxElectricHeater / duration).Value(), 1e-3, "expectedElectricHeaterDemand");
        //Assert.AreEqual(expectedFcElectricHeaterKg, fcCorr.FcBusAuxEletcricHeater.Value(), 1e-6, "expectedFcElectricHeater");
        Assert.AreEqual(expectedAuxHeatingDemandJ, corrected.AuxHeaterDemand.Value(), 1e-1, "expectedAuxHeatingDemand");
        //Assert.AreEqual(expectedFcAuxHeaterKg, fcCorr.FcAuxHtr.Value(), 1e-6, "expectedFcAuxHeater");
        //Assert.AreEqual(0, fcCorr.FC_FINAL_H.Value());
    }

    // =========================================
    // =========================================


    [
        TestCase("a", CFG1, HeatPumpNone, HeatPumpNone, NoElHtr, 0.0, 0.0, 0.0, AuxHtrPwr0, 0, 0, 0, 0, 0, 0, 0, 0),
        TestCase("b", CFG1, HeatPumpNone, HeatPumpNone, AirElHtr, 0.0, 0.0, 7.0, AuxHtrPwr0, 0.000, 0.000000, 0.000, 0.000000, 473.748, 0.007106, 0.0, 0.000000),
        TestCase("b", CFG1, HeatPumpNone, HeatPumpNone, AirElHtr, 1.0, 2.0, 7.0, AuxHtrPwr0, 0.000, 0.000000, 0.000, 0.000000, 96.890, 0.001453, 0.0, 0.000000),

        TestCase("c", CFG1, HeatPumpNone, HeatPumpNone, NoElHtr, 0.0, 0.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 0.000, 0.000000, 0.000, 0.000000, 9415850.1, 0.220512),
        TestCase("d", CFG1, HeatPumpNone, HeatPumpNone, AirElHtr, 0.0, 0.0, 7.0, AuxHtrPwr30, 0.000, 0.000000, 0.000, 0.000000, 11.390, 0.000171, 1934969.0, 0.045315),
        TestCase("e", CFG1, HeatPumpNone, HeatPumpNone, AirElHtr, 1.0, 2.0, 7.0, AuxHtrPwr30, 0.000, 0.000000, 0.000, 0.000000, 7.823, 0.000117, 372742.4, 0.008729),

        TestCase("f", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 0.0, 0.0, 0.0, AuxHtrPwr0, 0.000, 0.000000, 947.372508, 0.014211, 0, 0, 0.0, 0.0000000),
        TestCase("g", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 0.0, 0.0, 7.0, AuxHtrPwr0, 0.000, 0.000000, 226.281, 0.003394, 0.000, 0.000000, 0.0, 0.000000),
        TestCase("h", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 1.0, 2.0, 7.0, AuxHtrPwr0, 0.000, 0.000000, 32.761, 0.000491, 0.000, 0.000000, 0.0, 0.000000),

        TestCase("i", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 0.0, 0.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 774.516, 0.011618, 0.000, 0.000000, 1718517.8, 0.040246),
        TestCase("j", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 0.0, 0.0, 7.0, AuxHtrPwr30, 0.000, 0.000000, 161.988, 0.002430, 0.000, 0.000000, 727685.3, 0.017042),
        TestCase("k", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 1.0, 2.0, 7.0, AuxHtrPwr30, 0.000, 0.000000, 22.933, 0.000344, 0.000, 0.000000, 236239.2, 0.005533),

        TestCase("l", CFG6, HeatPumpNone, HeatPumpCont, NoElHtr, 0.0, 0.0, 0.0, AuxHtrPwr0, 1267.984, 0.019020, 0.000, 0.000000, 0.000, 0.000000, 0.0, 0.000000),
        TestCase("m", CFG6, HeatPumpNone, HeatPumpCont, NoElHtr, 0.0, 0.0, 7.0, AuxHtrPwr0, 299.096, 0.004486, 0.000, 0.000000, 0.000, 0.000000, 0.0, 0.000000),
        TestCase("n", CFG6, HeatPumpNone, HeatPumpCont, NoElHtr, 1.0, 2.0, 7.0, AuxHtrPwr0, 43.121, 0.000647, 0.000, 0.000000, 0.000, 0.000000, 0.0, 0.000000),

        TestCase("o", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 0.0, 0.0, 0.0, AuxHtrPwr0, 1267.984, 0.019020, 0.000, 0.000000, 83.878, 0.001258, 0.0, 0.000000),
        TestCase("p", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 0.0, 0.0, 7.0, AuxHtrPwr0, 299.096, 0.004486, 0.000, 0.000000, 56.950, 0.000854, 0.0, 0.000000),
        TestCase("q", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 1.0, 2.0, 7.0, AuxHtrPwr0, 43.121, 0.000647, 0.000, 0.000000, 39.117, 0.000587, 0.0, 0.000000),

        TestCase("r", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 0.0, 0.0, 0.0, AuxHtrPwr30, 1038.910, 0.015584, 0.000, 0.000000, 16.776, 0.000252, 1648312.3, 0.038602),
        TestCase("s", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 0.0, 0.0, 7.0, AuxHtrPwr30, 214.221, 0.003213, 0.000, 0.000000, 11.390, 0.000171, 680018.0, 0.015925),
        TestCase("t", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 1.0, 2.0, 7.0, AuxHtrPwr30, 30.184, 0.000453, 0.000, 0.000000, 7.823, 0.000117, 203498.0, 0.004766),

    //TestCase("o", CFG7, HeatPump2Stage, HeatPump3Stage, NoElHtr, 0.0, AuxHtrPwr0, 0.000, 0.000000, 952.347901, 0.014285, 0, 0, 0.0, 0.0000000),
    //TestCase("p", CFG7, HeatPump2Stage, HeatPump3Stage, AirElHtr, 0.0, AuxHtrPwr30, 0.000, 0.000000, 778.435497, 0.011677, 0, 0, 1718517.8, 0.0402463),
    //TestCase("q", CFG7, HeatPump2Stage, HeatPump3Stage, WaterElHtr, 4.0, AuxHtrPwr0, 0.000, 0.000000, 49.251969, 0.000739, 0, 0, 336067.2, 0.0078704),
    //TestCase("r", CFG7, HeatPump2Stage, HeatPump3Stage, WaterElHtr, 4.0, AuxHtrPwr30, 0.000, 0.000000, 49.251969, 0.000739, 0, 0, 336067.2, 0.0078704),
    //TestCase("s", CFG7, HeatPumpCont, HeatPumpCont, OthrElHtr, 4.0, AuxHtrPwr0, 64.408, 0.000966, 0.000000, 0.000000, 0, 0, 336067.2, 0.0078704),
    //TestCase("t", CFG7, HeatPumpCont, HeatPumpCont, OthrElHtr, 4.0, AuxHtrPwr30, 64.408, 0.000966, 0.000000, 0.000000, 0, 0, 336067.2, 0.0078704),
    ]
    public void TestModDataPostprocessing_HEV_S(string sort, BusHVACSystemConfiguration cfg, HeatPumpType driverHpHeating,
    HeatPumpType passengerHpHeating, HeaterType heater, double fuelConsumptionKg, double genLoss, double emLossKwH, double auxhHeaterPwr,
    double expectedHpHeatingElPwrMechW, double expectedFcHPHeatingElKg, double expectedHpHeatingMechPwrW,
    double expectedFcHPHeatingMechKg, double expectedElectricHeaterPwrW,
    double expectedFcElectricHeaterKg, double expectedAuxHeatingDemandJ, double expectedFcAuxHeaterKg)
    {
        var busAux = GetAuxParametersParallelHybridSingleDeckForTest(cfg, driverHeatPumpHeating: driverHpHeating,
            passengerHeatPumpHeating: passengerHpHeating, auxHeaterPower: auxhHeaterPwr.SI<Watt>(), electricHeater: heater);
        var runData = GetVectoRunDataSerialHybrid(busAux);
        var mockModData = GetMockModData(runData, fuelConsumptionKg.SI<Kilogram>(), emLoss: emLossKwH.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>(),
            genLoss: genLoss.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>());

        var corrected = new ParallelHybridModalDataPostprocessingCorrection().ApplyCorrection(mockModData, runData);

        var duration = mockModData.Duration.Value();

        Assert.NotNull(corrected);
        var fcCorr = corrected.FuelConsumptionCorrection(Fuel) as FuelConsumptionCorrection;
        Assert.NotNull(fcCorr);

        Console.WriteLine(new[] { (corrected.WorkBusAuxHeatPumpHeatingElMech / duration).Value().ToString("F3"),
            fcCorr.FcHeatPumpHeatingEl.Value().ToString("F6"),
            (corrected.WorkBusAuxHeatPumpHeatingMech.Value() / duration).ToString("F3"),
            fcCorr.FcHeatPumpHeatingMech.Value().ToString("F6"),
            (corrected.WorkBusAuxElectricHeater / duration).Value().ToString("F3"),
            fcCorr.FcBusAuxEletcricHeater.Value().ToString("F6"),
            corrected.AuxHeaterDemand.Value().ToString("F1"),
            fcCorr.FcAuxHtr.Value().ToString("F6")
            }.Join(", "));

        Assert.AreEqual(expectedHpHeatingElPwrMechW, (corrected.WorkBusAuxHeatPumpHeatingElMech / duration).Value(), 1e-3, "Heatpump Power El Mech");
        Assert.AreEqual(expectedHpHeatingMechPwrW, (corrected.WorkBusAuxHeatPumpHeatingMech / duration).Value(), 1e-3, "Heatpump Power Mech");
        Assert.AreEqual(expectedFcHPHeatingElKg, fcCorr.FcHeatPumpHeatingEl.Value(), 1e-6, "FC heatpump heating El");
        Assert.AreEqual(expectedFcHPHeatingMechKg, fcCorr.FcHeatPumpHeatingMech.Value(), 1e-6, "FC heatpump heating Mech");
        Assert.AreEqual(expectedElectricHeaterPwrW, (corrected.WorkBusAuxElectricHeater / duration).Value(), 1e-3, "expectedElectricHeaterDemand");
        Assert.AreEqual(expectedFcElectricHeaterKg, fcCorr.FcBusAuxEletcricHeater.Value(), 1e-6, "expectedFcElectricHeater");
        Assert.AreEqual(expectedAuxHeatingDemandJ, corrected.AuxHeaterDemand.Value(), 1e-1, "expectedAuxHeatingDemand");
        Assert.AreEqual(expectedFcAuxHeaterKg, fcCorr.FcAuxHtr.Value(), 1e-6, "expectedFcAuxHeater");

        var expectedFc = fuelConsumptionKg + expectedFcHPHeatingElKg + expectedFcHPHeatingMechKg +
                        expectedFcElectricHeaterKg + expectedFcAuxHeaterKg;
        Assert.AreEqual(expectedFc, fcCorr.TotalFuelConsumptionCorrected.Value(), 1e-6, "TotalFuelConsumptionCorrected");
    }

    // =========================================
    // =========================================


    [
        TestCase("aa", CFG1, HeatPumpNone, HeatPumpNone, NoElHtr, 0.0, 0.0, AuxHtrPwr0, 0, 0, 0, 0, 0, 0, 0, 0),
        TestCase("ab", CFG1, HeatPumpNone, HeatPumpNone, AirElHtr, 0.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 0.000, 0.000000, 16.776, 0.000252, 9345644.6, 0.218868),
        TestCase("ac", CFG1, HeatPumpNone, HeatPumpNone, AirElHtr, 0.0, 1.0, AuxHtrPwr30, 0.000, 0.000000, 0.000, 0.000000, 16.006, 0.000240, 7707580.1, 0.180505),
        TestCase("ad", CFG1, HeatPumpNone, HeatPumpNone, WaterElHtr, 4.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 0.000, 0.000000, 8.664, 0.000130, 660942.6, 0.015479),
        TestCase("ae", CFG1, HeatPumpNone, HeatPumpNone, WaterElHtr, 4.0, 1.0, AuxHtrPwr30, 0.000, 0.000000, 0.000, 0.000000, 7.895, 0.000118, 397166.1, 0.009301),

        TestCase("af", CFG2, HeatPump2Stage, HeatPumpNone, AirElHtr, 0.0, 0.0, AuxHtrPwr0, 0.000, 0.000000, 99.713, 0.001496, 2033.302, 0.030500, 0.0, 0.00000),
        TestCase("ag", CFG2, HeatPump2Stage, HeatPumpNone, AirElHtr, 0.0, 1.0, AuxHtrPwr0, 0.000, 0.000000, 84.563, 0.001268, 1679.953, 0.025199, 0.0, 0.000000),
        TestCase("ah", CFG2, HeatPump2Stage, HeatPumpNone, WaterElHtr, 0.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 81.371, 0.001221, 16.776, 0.000252, 8575911.3, 0.200841),
        TestCase("ai", CFG2, HeatPump2Stage, HeatPumpNone, WaterElHtr, 0.0, 1.0, AuxHtrPwr30, 0.000, 0.000000, 67.865, 0.001018, 16.006, 0.000240, 7087820.6, 0.165991),
        TestCase("aj", CFG2, HeatPump2Stage, HeatPumpNone, OthrElHtr, 4.0, 0.0, AuxHtrPwr0, 0.000, 0.000000, 7.445, 0.000112, 154.268, 0.002314, 0.0, 0.000000),
        TestCase("ak", CFG2, HeatPump2Stage, HeatPumpNone, OthrElHtr, 4.0, 1.0, AuxHtrPwr0, 0.000, 0.000000, 3.824, 0.000057, 96.465, 0.001447, 0.0, 0.000000),
        TestCase("al", CFG2, HeatPump2Stage, HeatPumpNone, OthrElHtr, 4.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 5.211, 0.000078, 8.664, 0.000130, 624829.2, 0.014633),
        TestCase("am", CFG2, HeatPump2Stage, HeatPumpNone, OthrElHtr, 4.0, 1.0, AuxHtrPwr30, 0.000, 0.000000, 2.677, 0.000040, 7.895, 0.000118, 378615.5, 0.008867),

        TestCase("ao", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 0.0, 0.0, AuxHtrPwr0, 0.000, 0.000000, 947.372508, 0.014211, 0, 0, 0.0, 0.0000000),
        TestCase("ao1", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 0.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 774.516, 0.011618, 0.000, 0.000000, 1718517.8, 0.040246),
        TestCase("ap", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 0.0, 1.0, AuxHtrPwr0, 0.000, 0.000000, 802.389, 0.012036, 0.000, 0.000000, 0.0, 0.000000),
        TestCase("ap1", CFG6, HeatPumpNone, HeatPump3Stage, NoElHtr, 0.0, 1.0, AuxHtrPwr30, 0.000, 0.000000, 645.042, 0.009676, 0.000, 0.000000, 1576970.3, 0.036931),
        TestCase("aq", CFG6, HeatPumpNone, HeatPump3Stage, AirElHtr, 0.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 774.516, 0.011618, 16.776, 0.000252, 1648312.3, 0.038602),
        TestCase("ar", CFG6, HeatPumpNone, HeatPump3Stage, AirElHtr, 0.0, 1.0, AuxHtrPwr30, 0.000, 0.000000, 645.042, 0.009676, 16.006, 0.000240, 1509984.5, 0.035363),
        TestCase("as", CFG6, HeatPumpNone, HeatPump3Stage, WaterElHtr, 4.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 48.934, 0.000734, 8.664, 0.000130, 299808.1, 0.007021),
        TestCase("at", CFG6, HeatPumpNone, HeatPump3Stage, WaterElHtr, 4.0, 1.0, AuxHtrPwr30, 0.000, 0.000000, 25.136, 0.000377, 7.895, 0.000118, 211659.9, 0.004957),
        TestCase("au", CFG6, HeatPumpNone, HeatPump3Stage, WaterElHtr, 4.0, 0.0, AuxHtrPwr0, 0.000, 0.000000, 69.906, 0.001049, 43.320, 0.000650, 0.0, 0.000000),
        TestCase("av", CFG6, HeatPumpNone, HeatPump3Stage, WaterElHtr, 4.0, 1.0, AuxHtrPwr0, 0.000, 0.000000, 35.909, 0.000539, 39.473, 0.000592, 0.0, 0.000000),
        TestCase("aw", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 0.0, 0.0, AuxHtrPwr30, 1038.910, 0.015584, 0.000, 0.000000, 16.776, 0.000252, 1648312.3, 0.038602),
        TestCase("ax", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 0.0, 1.0, AuxHtrPwr30, 863.938, 0.012959, 0.000, 0.000000, 16.006, 0.000240, 1509984.5, 0.035363),
        TestCase("ay", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 0.0, 0.0, AuxHtrPwr0, 1267.984, 0.019020, 0.000, 0.000000, 83.878, 0.001258, 0.0, 0.000000),
        TestCase("az", CFG6, HeatPumpNone, HeatPumpCont, OthrElHtr, 0.0, 1.0, AuxHtrPwr0, 1072.412, 0.016086, 0.000, 0.000000, 80.031, 0.001200, 0.0, 0.000000),

        TestCase("ba", CFG7, HeatPump2Stage, HeatPump3Stage, NoElHtr, 0.0, 0.0, AuxHtrPwr0, 0.000, 0.000000, 952.348, 0.014285, 0.000, 0.000000, 0.0, 0.000000),
        TestCase("bb", CFG7, HeatPump2Stage, HeatPump3Stage, NoElHtr, 0.0, 1.0, AuxHtrPwr0, 0.000, 0.000000, 806.713, 0.012101, 0.000, 0.000000, 0.0, 0.000000),
        TestCase("bc", CFG7, HeatPump2Stage, HeatPump3Stage, AirElHtr, 0.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 778.435, 0.011677, 16.776, 0.000252, 1648312.3, 0.038602),
        TestCase("bd", CFG7, HeatPump2Stage, HeatPump3Stage, AirElHtr, 0.0, 1.0, AuxHtrPwr30, 0.000, 0.000000, 648.403, 0.009726, 16.006, 0.000240, 1509984.5, 0.035363),
        TestCase("be", CFG7, HeatPump2Stage, HeatPump3Stage, WaterElHtr, 4.0, 0.0, AuxHtrPwr0, 0.000, 0.000000, 70.360, 0.001055, 43.320, 0.000650, 0.0, 0.000000),
        TestCase("bf", CFG7, HeatPump2Stage, HeatPump3Stage, WaterElHtr, 4.0, 1.0, AuxHtrPwr0, 0.000, 0.000000, 36.142, 0.000542, 39.473, 0.000592, 0.0, 0.000000),
        TestCase("bg", CFG7, HeatPump2Stage, HeatPump3Stage, WaterElHtr, 4.0, 0.0, AuxHtrPwr30, 0.000, 0.000000, 49.252, 0.000739, 8.664, 0.000130, 299808.1, 0.007021),
        TestCase("bh", CFG7, HeatPump2Stage, HeatPump3Stage, WaterElHtr, 4.0, 1.0, AuxHtrPwr30, 0.000, 0.000000, 25.300, 0.000379, 7.895, 0.000118, 211659.9, 0.004957),
        TestCase("bi", CFG7, HeatPumpCont, HeatPumpCont, OthrElHtr, 4.0, 0.0, AuxHtrPwr0, 92.011, 0.001380, 0.000, 0.000000, 43.320, 0.000650, 0.0, 0.000000),
        TestCase("bj", CFG7, HeatPumpCont, HeatPumpCont, OthrElHtr, 4.0, 1.0, AuxHtrPwr0, 47.264, 0.000709, 0.000, 0.000000, 39.473, 0.000592, 0.0, 0.000000),
        TestCase("bk", CFG7, HeatPumpCont, HeatPumpCont, OthrElHtr, 4.0, 0.0, AuxHtrPwr30, 64.408, 0.000966, 0.000, 0.000000, 8.664, 0.000130, 299808.1, 0.007021),
        TestCase("bl", CFG7, HeatPumpCont, HeatPumpCont, OthrElHtr, 4.0, 1.0, AuxHtrPwr30, 33.085, 0.000496, 0.000, 0.000000, 7.895, 0.000118, 211659.9, 0.004957),
    ]
    public void TestModDataPostprocessing_HEV_P(string sort, BusHVACSystemConfiguration cfg, HeatPumpType driverHpHeating,
    HeatPumpType passengerHpHeating, HeaterType heater, double fuelConsumptionKg, double emLossKwH, double auxhHeaterPwr,
    double expectedHpHeatingElPwrMechW, double expectedFcHPHeatingElKg, double expectedHpHeatingMechPwrW,
    double expectedFcHPHeatingMechKg, double expectedElectricHeaterPwrW,
    double expectedFcElectricHeaterKg, double expectedAuxHeatingDemandJ, double expectedFcAuxHeaterKg)
    {
        var busAux = GetAuxParametersParallelHybridSingleDeckForTest(cfg, driverHeatPumpHeating: driverHpHeating,
            passengerHeatPumpHeating: passengerHpHeating, auxHeaterPower: auxhHeaterPwr.SI<Watt>(), electricHeater: heater);
        var runData = GetVectoRunDataParallelHybrid(busAux);
        var mockModData = GetMockModData(runData, fuelConsumptionKg.SI<Kilogram>(), emLossKwH.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>());

        var corrected = new ParallelHybridModalDataPostprocessingCorrection().ApplyCorrection(mockModData, runData);

        var duration = mockModData.Duration.Value();

        Assert.NotNull(corrected);
        var fcCorr = corrected.FuelConsumptionCorrection(Fuel) as FuelConsumptionCorrection;
        Assert.NotNull(fcCorr);

        Console.WriteLine(new[] { (corrected.WorkBusAuxHeatPumpHeatingElMech / duration).Value().ToString("F3"),
            fcCorr.FcHeatPumpHeatingEl.Value().ToString("F6"),
            (corrected.WorkBusAuxHeatPumpHeatingMech.Value() / duration).ToString("F3"),
            fcCorr.FcHeatPumpHeatingMech.Value().ToString("F6"),
            (corrected.WorkBusAuxElectricHeater / duration).Value().ToString("F3"),
            fcCorr.FcBusAuxEletcricHeater.Value().ToString("F6"),
            corrected.AuxHeaterDemand.Value().ToString("F1"),
            fcCorr.FcAuxHtr.Value().ToString("F6")
            }.Join(", "));

        Assert.AreEqual(expectedHpHeatingElPwrMechW, (corrected.WorkBusAuxHeatPumpHeatingElMech / duration).Value(), 1e-3, "Heatpump Power El Mech");
        Assert.AreEqual(expectedHpHeatingMechPwrW, (corrected.WorkBusAuxHeatPumpHeatingMech / duration).Value(), 1e-3, "Heatpump Power Mech");
        Assert.AreEqual(expectedFcHPHeatingElKg, fcCorr.FcHeatPumpHeatingEl.Value(), 1e-6, "FC heatpump heating El");
        Assert.AreEqual(expectedFcHPHeatingMechKg, fcCorr.FcHeatPumpHeatingMech.Value(), 1e-6, "FC heatpump heating Mech");
        Assert.AreEqual(expectedElectricHeaterPwrW, (corrected.WorkBusAuxElectricHeater / duration).Value(), 1e-3, "expectedElectricHeaterDemand");
        Assert.AreEqual(expectedFcElectricHeaterKg, fcCorr.FcBusAuxEletcricHeater.Value(), 1e-6, "expectedFcElectricHeater");
        Assert.AreEqual(expectedAuxHeatingDemandJ, corrected.AuxHeaterDemand.Value(), 1e-1, "expectedAuxHeatingDemand");
        Assert.AreEqual(expectedFcAuxHeaterKg, fcCorr.FcAuxHtr.Value(), 1e-6, "expectedFcAuxHeater");

        var expectedFc = fuelConsumptionKg + expectedFcHPHeatingElKg + expectedFcHPHeatingMechKg +
                        expectedFcElectricHeaterKg + expectedFcAuxHeaterKg;
        Assert.AreEqual(expectedFc, fcCorr.TotalFuelConsumptionCorrected.Value(), 1e-6, "TotalFuelConsumptionCorrected");
    }

    // =========================================
    // =========================================



    protected VectoRunData GetVectoRunDataConventional(IAuxiliaryConfig busAux)
    {

        var runData = new VectoRunData() {
            SimulationType = SimulationType.DistanceCycle,
            ExecutionMode = ExecutionMode.Declaration,
            Exempted = false,
            JobType = VectoSimulationJobType.ConventionalVehicle,
            DriverData = new DriverData() {
                EngineStopStart = new DriverData.EngineStopStartData() {
                    UtilityFactorDriving = 0.8,
                    UtilityFactorStandstill = 0.8
                }
            },
            EngineData = new CombustionEngineData() {
                WHRType = WHRType.None,
                IdleSpeed = 600.RPMtoRad(),
                Fuels = new[] { new CombustionEngineFuelData() {
                    FuelData =Fuel,
                    ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(FuelMap.ToStream()),
                }}.ToList(),
            },
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
            BusAuxiliaries = busAux
        };
        return runData;
    }

    protected VectoRunData GetVectoRunDataParallelHybrid(IAuxiliaryConfig busAux)
    {
        var emData = new Mock<ElectricMotorData>();


        var runData = new VectoRunData() {
            SimulationType = SimulationType.DistanceCycle,
            ExecutionMode = ExecutionMode.Declaration,
            Exempted = false,
            JobType = VectoSimulationJobType.ParallelHybridVehicle,
            DriverData = new DriverData() {
                EngineStopStart = new DriverData.EngineStopStartData() {
                    UtilityFactorDriving = 0.8,
                    UtilityFactorStandstill = 0.8
                }
            },
            EngineData = new CombustionEngineData() {
                WHRType = WHRType.None,
                IdleSpeed = 600.RPMtoRad(),
                Fuels = new[] { new CombustionEngineFuelData() {
                    FuelData =Fuel,
                    ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(FuelMap.ToStream()),
                }}.ToList(),
            },
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
                Tuple.Create(PowertrainPosition.HybridP2, emData.Object)
            },
            BusAuxiliaries = busAux
        };
        return runData;
    }

    protected VectoRunData GetVectoRunDataSerialHybrid(IAuxiliaryConfig busAux)
    {
        var emData = new Mock<ElectricMotorData>();
        var genData = new Mock<ElectricMotorData>();


        var runData = new VectoRunData() {
            SimulationType = SimulationType.DistanceCycle,
            ExecutionMode = ExecutionMode.Declaration,
            Exempted = false,
            JobType = VectoSimulationJobType.SerialHybridVehicle,
            DriverData = new DriverData() {
                EngineStopStart = new DriverData.EngineStopStartData() {
                    UtilityFactorDriving = 0.8,
                    UtilityFactorStandstill = 0.8
                }
            },
            EngineData = new CombustionEngineData() {
                WHRType = WHRType.None,
                IdleSpeed = 600.RPMtoRad(),
                Fuels = new[] { new CombustionEngineFuelData() {
                    FuelData =Fuel,
                    ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(FuelMap.ToStream()),
                }}.ToList(),
            },
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
                Tuple.Create(PowertrainPosition.BatteryElectricE2, emData.Object),
                Tuple.Create(PowertrainPosition.GEN, genData.Object),
            },
            BusAuxiliaries = busAux
        };
        return runData;
    }

    protected VectoRunData GetVectoRunDataBatteryElectric(IAuxiliaryConfig busAux)
    {
        var emData = new Mock<ElectricMotorData>();

        var runData = new VectoRunData() {
            SimulationType = SimulationType.DistanceCycle,
            ExecutionMode = ExecutionMode.Declaration,
            Exempted = false,
            JobType = VectoSimulationJobType.BatteryElectricVehicle,
            DriverData = new DriverData() {
                EngineStopStart = new DriverData.EngineStopStartData() {
                    UtilityFactorDriving = 0.8,
                    UtilityFactorStandstill = 0.8
                }
            },
            //EngineData = new CombustionEngineData() {
            //	WHRType = WHRType.None,
            //	IdleSpeed = 600.RPMtoRad(),
            //	Fuels = new[] { new CombustionEngineFuelData() {
            //		FuelData =Fuel,
            //		ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(FuelMap.ToStream()),
            //	}}.ToList(),
            //},
            ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
                Tuple.Create(PowertrainPosition.BatteryElectricE2, emData.Object),
            },
            BusAuxiliaries = busAux,
            BatteryData = new BatterySystemData() {
                Batteries = new List<Tuple<int, BatteryData>>() {
                    Tuple.Create(0, new BatteryData() {
                        BatteryId = 0,
                        Capacity = 7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
                        ChargeDepletingBattery = false,
                        InternalResistance = BatteryInternalResistanceReader.Create("SoC, Ri\n0,  0.024\n100,  0.024".ToStream(), false),
                        SOCMap = BatterySOCReader.Create("SOC, V\n0, 590\n100, 614".ToStream()),
                        MinSOC = 10,
                        MaxSOC = 90,
                        MaxCurrent = BatteryMaxCurrentReader.Create("SOC, I_charge, I_discharge\n0, 1620, 1620\n100, 1620, 1620".ToStream()),
                    })
                }
            }
        };
        return runData;
    }

    protected IModalDataContainer GetMockModData(VectoRunData runData, Kilogram totalFuelConsumption = null, WattSecond emLoss = null, WattSecond genLoss = null)
    {
        var mockContainer = new Mock<IVehicleContainer>();

        if (runData.JobType != VectoSimulationJobType.BatteryElectricVehicle) {
            var eng = new Mock<IEngineInfo>();
            eng.Setup(e => e.EngineIdleSpeed).Returns(runData.EngineData.IdleSpeed);
            mockContainer.Setup(c => c.EngineInfo).Returns(eng.Object);
        } else {
            var eng = new Mock<IEngineInfo>();
            eng.Setup(e => e.EngineIdleSpeed).Returns(100.RPMtoRad());
            mockContainer.Setup(c => c.EngineInfo).Returns(eng.Object);
        }


        var busAux = new BusAuxiliariesAdapter(mockContainer.Object, runData.BusAuxiliaries);

        var m = new Mock<IModalDataContainer>();
        m.Setup(x => x.Duration).Returns(3600.SI<Second>());
        m.Setup(x => x.Distance).Returns(30000.SI<Meter>());
        if (totalFuelConsumption != null) {
            m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_WHR_el_corr, null)).Returns(0.SI<WattSecond>());
            m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_WHR_mech_corr, null)).Returns(0.SI<WattSecond>());
            m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_ice_start, null)).Returns(0.SI<WattSecond>());
            m.Setup(x => x.FuelData).Returns(new IFuelProperties[] { Fuel });
            m.Setup(x => x.TimeIntegral<Kilogram>(It.IsIn(Fuel.FuelType.GetLabel()), null)).Returns<string, Func<SI, bool>>((f, _) => totalFuelConsumption);
            m.Setup(x => x.GetColumnName(It.IsNotNull<IFuelProperties>(), It.IsNotNull<ModalResultField>()))
                .Returns<IFuelProperties, ModalResultField>((f, m) => f.GetLabel());
            m.Setup(x => x.EngineLineCorrectionFactor(It.IsIn(Fuel)))
                .Returns(15.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<KilogramPerWattSecond>());
        } else {
            m.Setup(x => x.FuelData).Returns(new IFuelProperties[] { });
        }
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_consumer_sum, null))
            .Returns(0.SI<WattSecond>());
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_generated, null))
            .Returns(0.SI<WattSecond>());
        m.Setup(x => x.GetValues<NormLiter>(ModalResultField.Nl_busAux_PS_generated)).Returns(new[] { 2470.5180.SI<NormLiter>() });
        m.Setup(x => x.AuxHeaterDemandCalc).Returns(busAux.AuxHeaterDemandCalculation);

        if (!runData.JobType.IsOneOf(VectoSimulationJobType.ConventionalVehicle,
                VectoSimulationJobType.EngineOnlySimulation)
            && !runData.ElectricMachinesData.Any()) {
            throw new VectoException("hybrid vehicle requires electric machine");
        }

        if (runData.ElectricMachinesData.Any(x => x.Item1 != PowertrainPosition.GEN)) {
            m.Setup(x => x.GetColumnName(It.IsAny<PowertrainPosition>(), It.IsAny<ModalResultField>()))
                .Returns<PowertrainPosition, ModalResultField>((pos, mrf) =>
                    string.Format(mrf.GetCaption(), pos.GetName()));
            var emPos = runData.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
            SetupMockEMotorValues(emLoss, m, emPos);

            if (runData.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.GEN)) {
                SetupMockEMotorValues(genLoss, m, PowertrainPosition.GEN);
            }
        }

        return m.Object;
    }

    private static void SetupMockEMotorValues(WattSecond emLoss, Mock<IModalDataContainer> m, PowertrainPosition emPos)
    {
        var colName = m.Object.GetColumnName(emPos, ModalResultField.P_EM_electricMotorLoss_);
        if (emPos == PowertrainPosition.IEPC) {
            colName = m.Object.GetColumnName(emPos, ModalResultField.P_IEPC_electricMotorLoss_);
        }

        m.Setup(x => x.TimeIntegral<WattSecond>(It.Is<string>(c => colName.Equals(c)), null))
            .Returns(emLoss ?? 0.SI<WattSecond>());
        m.Setup(x => x.ElectricMotorEfficiencyDrive(emPos)).Returns(0.94);
        m.Setup(x => x.ElectricMotorEfficiencyGenerate(emPos)).Returns(0.92);
        SetupMockBatteryValues(m);
    }

    private static void SetupMockBatteryValues(Mock<IModalDataContainer> m)
    {
        var batChgEff = 0.95;
        var batDischgEff = 0.93;
        var batEnergy = 1000.SI<WattSecond>();
        var batteryEntries = new[] {
			// internal , terminal
			Tuple.Create(batEnergy, batEnergy / batChgEff),
            Tuple.Create(-batEnergy, -batEnergy * batDischgEff)
        };
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_int, null))
            .Returns(batteryEntries.Select(x => x.Item1).Sum());
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_int, It.IsAny<Func<SI, bool>>()))
            .Returns<ModalResultField, Func<SI, bool>>((_, f) =>
                batteryEntries.Select(x => x.Item1).Where(x => f == null || f(x)).Sum());
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_reess_terminal, It.IsAny<Func<SI, bool>>()))
            .Returns<ModalResultField,
                Func<SI, bool>>((_, f) => batteryEntries.Select(x => x.Item2).Where(x => f(x)).Sum());
        m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_terminal_ES, It.IsAny<Func<SI, bool>>()))
            .Returns<ModalResultField,
                Func<SI, bool>>((_, f) => batteryEntries.Select(x => x.Item2).Where(x => f(x)).Sum());

        m.Setup(x => x.GetValues<SI>(ModalResultField.REESSStateOfCharge)).Returns(new[] { 0.5.SI<Scalar>(), 0.4.SI<Scalar>() });
    }


    protected IAuxiliaryConfig GetAuxParametersConventionalSingleDeckForTest(BusHVACSystemConfiguration hvacConfig, HeatPumpType? driverHeatpumpCooling = null,
        HeatPumpType? passengerHeatpumpCooling = null, HeatPumpType? driverHeatPumpHeating = null, HeatPumpType? passengerHeatPumpHeating = null,
        Watt auxHeaterPower = null, HeaterType? electricHeater = null, int passengerCount = 40, double length = 12, double height = 3)
    {
        return SSMBusAuxModelParameters.CreateBusAuxInputParameters(MissionType.Urban,
            VehicleClass.Class31a,
            VectoSimulationJobType.ConventionalVehicle,
            VehicleCode.CA,
            RegistrationClass.II,
            AxleConfiguration.AxleConfig_4x2,
            articulated: false,
            lowEntry: false,
            length: length.SI<Meter>(),
            height: height.SI<Meter>(),
            width: 2.55.SI<Meter>(),
            numPassengersLowerdeck: passengerCount,
            numPassengersUpperdeck: 0,
            hpHeatingDriver: driverHeatPumpHeating ?? HeatPumpType.none,
            hpCoolingDriver: driverHeatpumpCooling ?? HeatPumpType.none,
            hpHeatingPassenger: passengerHeatPumpHeating ?? HeatPumpType.none,
            hpCoolingPassenger: passengerHeatpumpCooling ?? HeatPumpType.none,
            auxHeaterPower: auxHeaterPower ?? 0.SI<Watt>(),
            airElectricHeater: electricHeater != null && (electricHeater & HeaterType.AirElectricHeater) != 0,
            waterElectricHeater: electricHeater != null && (electricHeater & HeaterType.WaterElectricHeater) != 0,
            otherElectricHeater: electricHeater != null && (electricHeater & HeaterType.OtherElectricHeating) != 0,
            hvacConfig: hvacConfig,
            doubleGlazing: false,
            adjustableAuxHeater: false,
            separateAirdistributionDicts: false,
            adjustableCoolantThermostat: false,
            engineWasteGasHeatExchanger: false,
            steeringpumps: new[] { "Dual displacement" },
            fanTech: "Crankshaft mounted - Discrete step clutch",
            alternatorTech: AlternatorType.Conventional,
            entranceHeight: 0.3.SI<Meter>(),
            loading: LoadingType.LowLoading);

    }

    protected IAuxiliaryConfig GetAuxParametersParallelHybridSingleDeckForTest(BusHVACSystemConfiguration hvacConfig, HeatPumpType? driverHeatpumpCooling = null,
        HeatPumpType? passengerHeatpumpCooling = null, HeatPumpType? driverHeatPumpHeating = null, HeatPumpType? passengerHeatPumpHeating = null,
        Watt auxHeaterPower = null, HeaterType? electricHeater = null, int passengerCount = 40, double length = 12, double height = 3)
    {
        return SSMBusAuxModelParameters.CreateBusAuxInputParameters(MissionType.Urban,
            VehicleClass.Class31a,
            VectoSimulationJobType.ConventionalVehicle,
            VehicleCode.CA,
            RegistrationClass.II,
            AxleConfiguration.AxleConfig_4x2,
            articulated: false,
            lowEntry: false,
            length: length.SI<Meter>(),
            height: height.SI<Meter>(),
            width: 2.55.SI<Meter>(),
            numPassengersLowerdeck: passengerCount,
            numPassengersUpperdeck: 0,
            hpHeatingDriver: driverHeatPumpHeating ?? HeatPumpType.none,
            hpCoolingDriver: driverHeatpumpCooling ?? HeatPumpType.none,
            hpHeatingPassenger: passengerHeatPumpHeating ?? HeatPumpType.none,
            hpCoolingPassenger: passengerHeatpumpCooling ?? HeatPumpType.none,
            auxHeaterPower: auxHeaterPower ?? 0.SI<Watt>(),
            airElectricHeater: electricHeater != null && (electricHeater & HeaterType.AirElectricHeater) != 0,
            waterElectricHeater: electricHeater != null && (electricHeater & HeaterType.WaterElectricHeater) != 0,
            otherElectricHeater: electricHeater != null && (electricHeater & HeaterType.OtherElectricHeating) != 0,
            hvacConfig: hvacConfig,
            doubleGlazing: false,
            adjustableAuxHeater: false,
            separateAirdistributionDicts: false,
            adjustableCoolantThermostat: false,
            engineWasteGasHeatExchanger: false,
            steeringpumps: new[] { "Dual displacement" },
            fanTech: "Crankshaft mounted - Discrete step clutch",
            alternatorTech: AlternatorType.Conventional,
            entranceHeight: 0.3.SI<Meter>(),
            loading: LoadingType.LowLoading);

    }
}