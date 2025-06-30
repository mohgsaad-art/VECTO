using System.Diagnostics;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.ModDataPostprocessing;

public class BusAuxPsPostProcessingTests
{
	private StandardKernel _kernel;

	[OneTimeSetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
	}

    [TestCase()]
    public void TestBusAuxPsESSStandstill_ModDataCorrection()
    {
        var runData = PostProcessingRunData.GetRunData(true);
        runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
        var writer = new FileOutputWriter(".");
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, writer, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);
		modData.WriteModalResults = true;

        modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
        modData.Data.CreateCombustionEngineColumns(runData);
        modData.Data.CreateColumns(ModalResults.DriverSignals);
        modData.Data.CreateColumns(ModalResults.WheelSignals);
        modData.Data.CreateColumns(ModalResults.DCDCConverterSignals);
        modData.Data.CreateColumns(ModalResults.BusAuxiliariesSignals);
        modData.Data.CreateElectricMotorColumns(runData.ElectricMachinesData.FirstOrDefault().Item1, null, ModalResults.ElectricMotorSignals);
        modData.Data.CreateColumns(ModalResults.BatterySignals);

        var fuel = runData.EngineData.Fuels[0];

        var absTime = 0.SI<Second>();
        var rnd = new Random(210629);
        var dt = 0.SI<Second>();

        var i = 0;
        // constant driving
        var n_ice1 = 800.RPMtoRad();
        var n_ice2 = 1200.RPMtoRad();
        var n_ice = n_ice1;
        var t_ice = 400.SI<NewtonMeter>();
        var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
        var T2 = 0.SI<Second>(); // 51.171787824561704 ["s"]
        var T3 = 0.SI<Second>(); // 50.195047986970792 ["s"]

        var Nl_consumed = 0.7.SI<NormLiterPerSecond>();
        var Nl_generated = Nl_consumed;

        var compressorMap = runData.BusAuxiliaries.PneumaticUserInputsConfig.CompressorMap;

        for (; i < 100; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T1 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            modData[ModalResultField.simulationInterval] = dt;

            modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
            modData[ModalResultField.ICEOn] = true;

            modData[ModalResultField.n_ice_avg] = n_ice;
            modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
            var fc = fuel.ConsumptionMap.GetFuelConsumption(t_ice, n_ice).Value *
                    fuel.FuelConsumptionCorrectionFactor;
            modData[ModalResultField.FCWHTCc] = fc;
            modData[ModalResultField.FCFinal] = fc;

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();

            modData[ModalResultField.Nl_busAux_PS_generated] = Nl_generated * dt;
            modData[ModalResultField.Nl_busAux_PS_consumer] = Nl_consumed * dt;
            var comp = compressorMap.Interpolate(n_ice);
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = comp.FlowRate * dt;
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = comp.PowerOff;
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = comp.PowerOn;

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // standstill
        for (; i < 200; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T2 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            modData[ModalResultField.simulationInterval] = dt;

            modData[ModalResultField.v_act] = 0.KMPHtoMeterPerSecond();
            modData[ModalResultField.ICEOn] = false;

            modData[ModalResultField.n_ice_avg] = 0.RPMtoRad();
            modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
            modData[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
            modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
            modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_consumer] = Nl_consumed * dt;
            var comp = compressorMap.Interpolate(runData.EngineData.IdleSpeed);
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = comp.FlowRate * dt;
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = comp.PowerOff;
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = comp.PowerOn;

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // constant driving different speed
        modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
        n_ice = n_ice2;
        for (; i < 300; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T3 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            modData[ModalResultField.simulationInterval] = dt;

            modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
            modData[ModalResultField.ICEOn] = true;

            modData[ModalResultField.n_ice_avg] = n_ice;
            modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
            var fc = fuel.ConsumptionMap.GetFuelConsumption(400.SI<NewtonMeter>(), n_ice).Value *
                fuel.FuelConsumptionCorrectionFactor;
            modData[ModalResultField.FCWHTCc] = fc;
            modData[ModalResultField.FCFinal] = fc;

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();

            modData[ModalResultField.Nl_busAux_PS_generated] = Nl_generated * dt;
            modData[ModalResultField.Nl_busAux_PS_consumer] = Nl_consumed * dt;
            var comp = compressorMap.Interpolate(n_ice);
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = comp.FlowRate * dt;
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = comp.PowerOff;
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = comp.PowerOn;


            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        modData.Finish(VectoRun.Status.Success);
        var corr = modData.CorrectedModalData as CorrectedModalData;

        var fcIdle =
            fuel.ConsumptionMap.GetFuelConsumption(0.SI<NewtonMeter>(), runData.EngineData.IdleSpeed).Value
                .Value() * fuel.FuelConsumptionCorrectionFactor; // 0.0002929177777777778

        var k_engline = 2.6254521511724e-8;

        var airDemand = (Nl_generated * (T1 + T2 + T3)).Value(); // 106.05137005359465
        var deltaAir = (Nl_generated * T2).Value(); // 35.820251477193189

        var comp1 = compressorMap.Interpolate(n_ice1); // rad/s: 83.775804095727821 | rpm: 800 4.1756093266666667 ["Nl/s"] 3139.5 ["W"] 340.79499999999996 ["W"] (exceeded: false)
        var comp2 = compressorMap.Interpolate(runData.EngineData.IdleSpeed); // rad/s: 83.775804095727821 | rpm: 800 4.1756093266666667 ["Nl/s"] 3139.5 ["W"] 340.79499999999996 ["W"] (exceeded: true)
        var comp3 = compressorMap.Interpolate(n_ice2); // rad/s: 125.66370614359173 | rpm: 1200 6.23922331 ["Nl/s"] 4609.5 ["W"] 667.68000000000006 ["W"] (exceeded: false)

        var workCompOff = comp1.PowerOff * T1 + comp3.PowerOff * T3; // 50600.028340142 ["Ws"]
        var workCompOn = comp1.PowerOn * T1 + comp3.PowerOn * T3; // 388773.28735600761 ["Ws"]
        var airOn = comp1.FlowRate * T1 + comp3.FlowRate * T3; // 522.52279399122142 ["Nl"]

        var kAir = ((workCompOn - workCompOff) / airOn).Value(); // 647193.31463566166
        var E_busAuxPS = kAir * deltaAir / 1e3; // 23182.627284607614 convert Nl to m^3

        var fcPSAir = k_engline * E_busAuxPS; // 0.00060864878674201034
        var fcPSICEOffStop = (comp2.PowerOff * k_engline * (1 - PostProcessingRunData.UF_ESS_Standstill) * T2).Value(); // 0.00011309017231128844

        var fcESS = fcIdle * T2.Value() * (1 - PostProcessingRunData.UF_ESS_Standstill); // 0.0037023142144981809

        var fcModSum = 0.195725;

        Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
        Assert.AreEqual(0, corr.WorkWHREl.Value());
        Assert.AreEqual(0, corr.WorkWHRElMech.Value());
        Assert.AreEqual(0, corr.WorkWHRMech.Value());

        Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

        Assert.AreEqual(deltaAir, (modData.AirConsumed() - modData.AirGenerated()).Value(), 1e-6);
        Assert.AreEqual(airDemand, corr.CorrectedAirDemand.Value(), 1e-6);
        Assert.AreEqual(deltaAir, corr.DeltaAir.Value(), 1e-6);

        Assert.AreEqual(kAir, corr.kAir.Value(), 1e-3);
        Assert.AreEqual(E_busAuxPS, corr.WorkBusAuxPSCorr.Value(), 1e-3);

        var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
        Assert.AreEqual(fcESS, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);


        Assert.AreEqual(fcPSAir, f.FcBusAuxPSAirDemand.Value(), 1e-12);
        Assert.AreEqual(fcPSICEOffStop, f.FcBusAuxPSDragICEOffStandstill.Value(), 1e-12);
        Assert.AreEqual(0, f.FcBusAuxPSDragICEOffDriving.Value(), 1e-12);
        Assert.AreEqual(fcPSAir + fcPSICEOffStop, f.FcBusAuxPs.Value(), 1e-12);

        Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcESS, f.FcEssCorr.Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcPSAir + fcPSICEOffStop + fcESS, f.FcBusAuxPsCorr.Value(), 1e-6);

        Assert.AreEqual(fcModSum + fcPSAir + fcPSICEOffStop + fcESS, f.FcFinal.Value(), 1e-6);

    }

    [TestCase()]
    public void TestBusAuxPsESSDriving_ModDataCorrection()
    {
        var runData = PostProcessingRunData.GetRunData(true);
        runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
        var writer = new FileOutputWriter(".");
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, writer, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);
		modData.WriteModalResults = true;

        modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
        modData.Data.CreateCombustionEngineColumns(runData);
        modData.Data.CreateColumns(ModalResults.DriverSignals);
        modData.Data.CreateColumns(ModalResults.WheelSignals);
        modData.Data.CreateColumns(ModalResults.DCDCConverterSignals);
        modData.Data.CreateColumns(ModalResults.BusAuxiliariesSignals);
        modData.Data.CreateElectricMotorColumns(runData.ElectricMachinesData.FirstOrDefault().Item1, null, ModalResults.ElectricMotorSignals);
        modData.Data.CreateColumns(ModalResults.BatterySignals);

        var fuel = runData.EngineData.Fuels[0];

        var absTime = 0.SI<Second>();
        var rnd = new Random(210629);
        var dt = 0.SI<Second>();

        var i = 0;
        // constant driving
        var n_ice1 = 800.RPMtoRad();
        var n_ice2 = 1200.RPMtoRad();
        var n_ice = n_ice1;
        var t_ice = 400.SI<NewtonMeter>();
        var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
        var T2 = 0.SI<Second>(); // 51.171787824561704 ["s"]
        var T3 = 0.SI<Second>(); // 50.195047986970792 ["s"]

        var Nl_consumed = 0.7.SI<NormLiterPerSecond>();
        var Nl_generated = Nl_consumed;

        var compressorMap = runData.BusAuxiliaries.PneumaticUserInputsConfig.CompressorMap;

        for (; i < 100; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T1 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            modData[ModalResultField.simulationInterval] = dt;

            modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
            modData[ModalResultField.ICEOn] = true;

            modData[ModalResultField.n_ice_avg] = n_ice;
            modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
            var fc = fuel.ConsumptionMap.GetFuelConsumption(t_ice, n_ice).Value *
                    fuel.FuelConsumptionCorrectionFactor;
            modData[ModalResultField.FCWHTCc] = fc;
            modData[ModalResultField.FCFinal] = fc;

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();

            modData[ModalResultField.Nl_busAux_PS_generated] = Nl_generated * dt;
            modData[ModalResultField.Nl_busAux_PS_consumer] = Nl_consumed * dt;
            var comp = compressorMap.Interpolate(n_ice);
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = comp.FlowRate * dt;
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = comp.PowerOff;
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = comp.PowerOn;

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // driving, ice off
        for (; i < 200; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T2 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            modData[ModalResultField.simulationInterval] = dt;

            modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
            modData[ModalResultField.ICEOn] = false;

            modData[ModalResultField.n_ice_avg] = 0.RPMtoRad();
            modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
            modData[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
            modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
            modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_consumer] = Nl_consumed * dt;
            var comp = compressorMap.Interpolate(runData.EngineData.IdleSpeed);
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = comp.FlowRate * dt;
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = comp.PowerOff;
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = comp.PowerOn;

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // constant driving different speed
        modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
        n_ice = n_ice2;
        for (; i < 300; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T3 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            modData[ModalResultField.simulationInterval] = dt;

            modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
            modData[ModalResultField.ICEOn] = true;

            modData[ModalResultField.n_ice_avg] = n_ice;
            modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
            var fc = fuel.ConsumptionMap.GetFuelConsumption(400.SI<NewtonMeter>(), n_ice).Value *
                fuel.FuelConsumptionCorrectionFactor;
            modData[ModalResultField.FCWHTCc] = fc;
            modData[ModalResultField.FCFinal] = fc;

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();

            modData[ModalResultField.Nl_busAux_PS_generated] = Nl_generated * dt;
            modData[ModalResultField.Nl_busAux_PS_consumer] = Nl_consumed * dt;
            var comp = compressorMap.Interpolate(n_ice);
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = comp.FlowRate * dt;
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = comp.PowerOff;
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = comp.PowerOn;


            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        modData.Finish(VectoRun.Status.Success);
        var corr = modData.CorrectedModalData as CorrectedModalData;

        var fcIdle =
            fuel.ConsumptionMap.GetFuelConsumption(0.SI<NewtonMeter>(), runData.EngineData.IdleSpeed).Value
                .Value() * fuel.FuelConsumptionCorrectionFactor;

        var dts = modData.GetValues(x => (Second)x[ModalResultField.simulationInterval.GetName()]).ToArray();

        var time = modData.GetValues<Second>(ModalResultField.time).ToArray();

        var k_engline = 2.6254521511724e-8;

        var airDemand = (Nl_generated * (T1 + T2 + T3)).Value(); // 106.05137005359465
        var deltaAir = (Nl_generated * T2).Value(); // 35.820251477193189

        var comp1 = compressorMap.Interpolate(n_ice1); // rpm: 800 4.1756093266666667 ["Nl/s"] 3139.5 ["W"] 340.79499999999996 ["W"] (exceeded: false)
        var comp2 = compressorMap.Interpolate(runData.EngineData.IdleSpeed); // rpm: 800 4.1756093266666667 ["Nl/s"] 3139.5 ["W"] 340.79499999999996 ["W"] (exceeded: true)
        var comp3 = compressorMap.Interpolate(n_ice2); // rpm: 1200 6.23922331 ["Nl/s"] 4609.5 ["W"] 667.68000000000006 ["W"] (exceeded: false)

        var workCompOff = comp1.PowerOff * T1 + comp3.PowerOff * T3; // 50600.028340142 ["Ws"]
        var workCompOn = comp1.PowerOn * T1 + comp3.PowerOn * T3; // 388773.28735600761 ["Ws"]
        var airOn = comp1.FlowRate * T1 + comp3.FlowRate * T3; // 522.52279399122142 ["Nl"]

        var kAir = ((workCompOn - workCompOff) / airOn).Value(); // 647193.31463566166
        var E_busAuxPS = kAir * deltaAir / 1e3; // 23182.627284607614  convert Nl to m^3

        var fcPSAir = k_engline * E_busAuxPS; // 0.00060864878674201034
        var fcPSICEOffDriving = (comp2.PowerOff * k_engline * (1 - PostProcessingRunData.UF_ESS_Driving) * T2).Value(); // 8.1956035804536988E-05

        var fcESS = fcIdle * T2.Value() * (1 - PostProcessingRunData.UF_ESS_Driving); // 0.0026830536210330951

        var fcModSum = 0.195725;

        Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
        Assert.AreEqual(0, corr.WorkWHREl.Value());
        Assert.AreEqual(0, corr.WorkWHRElMech.Value());
        Assert.AreEqual(0, corr.WorkWHRMech.Value());

        Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

        Assert.AreEqual(deltaAir, (modData.AirConsumed() - modData.AirGenerated()).Value(), 1e-6);
        Assert.AreEqual(airDemand, corr.CorrectedAirDemand.Value(), 1e-6);
        Assert.AreEqual(deltaAir, corr.DeltaAir.Value(), 1e-6);

        Assert.AreEqual(kAir, corr.kAir.Value(), 1e-3);
        Assert.AreEqual(E_busAuxPS, corr.WorkBusAuxPSCorr.Value(), 1e-3);

        var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
        Assert.AreEqual(fcESS, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);


        Assert.AreEqual(fcPSAir, f.FcBusAuxPSAirDemand.Value(), 1e-12);
        Assert.AreEqual(0, f.FcBusAuxPSDragICEOffStandstill.Value(), 1e-12);
        Assert.AreEqual(fcPSICEOffDriving, f.FcBusAuxPSDragICEOffDriving.Value(), 1e-12);
        Assert.AreEqual(fcPSAir + fcPSICEOffDriving, f.FcBusAuxPs.Value(), 1e-12);

        Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcESS, f.FcEssCorr.Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcPSAir + fcPSICEOffDriving + fcESS, f.FcBusAuxPsCorr.Value(), 1e-6);

        Assert.AreEqual(fcModSum + fcPSAir + fcPSICEOffDriving + fcESS, f.FcFinal.Value(), 1e-6);
    }

    [TestCase(0.5), // lower actual average air demand
        TestCase(0.7),  // no difference in air demand
        TestCase(0.8),  // higher actual average air demand
                 ]
    public void TestBusAuxPSDemand_ModDataCorrection(double nlConsumedCorrected)
    {
        var runData = PostProcessingRunData.GetRunData(true);
        runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name + $"_{nlConsumedCorrected}";
        var writer = new FileOutputWriter(".");
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, writer, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);
		modData.WriteModalResults = true;

        modData.Data.CreateCombustionEngineColumns(runData);
        modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
        modData.Data.CreateColumns(ModalResults.DriverSignals);
        modData.Data.CreateColumns(ModalResults.WheelSignals);
        modData.Data.CreateColumns(ModalResults.DCDCConverterSignals);
        modData.Data.CreateColumns(ModalResults.BusAuxiliariesSignals);
        modData.Data.CreateElectricMotorColumns(runData.ElectricMachinesData.FirstOrDefault().Item1, null, ModalResults.ElectricMotorSignals);
        modData.Data.CreateColumns(ModalResults.BatterySignals);

        var fuel = runData.EngineData.Fuels[0];

        var absTime = 0.SI<Second>();
        var rnd = new Random(210629);
        var dt = 0.SI<Second>();

        var i = 0;
        // constant driving
        var n_ice1 = 800.RPMtoRad();
        var n_ice2 = 1200.RPMtoRad();
        var n_ice = n_ice1;
        var t_ice = 400.SI<NewtonMeter>();
        var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
        var T2 = 0.SI<Second>(); // 101.36683581153247 ["s"]

        var Nl_consumed = 0.7.SI<NormLiterPerSecond>();
        var Nl_generated = Nl_consumed;

        var compressorMap = runData.BusAuxiliaries.PneumaticUserInputsConfig.CompressorMap;

        for (; i < 100; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T1 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            modData[ModalResultField.simulationInterval] = dt;

            modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
            modData[ModalResultField.ICEOn] = true;

            modData[ModalResultField.n_ice_avg] = n_ice;
            modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
            var fc = fuel.ConsumptionMap.GetFuelConsumption(t_ice, n_ice).Value *
                    fuel.FuelConsumptionCorrectionFactor;
            modData[ModalResultField.FCWHTCc] = fc;
            modData[ModalResultField.FCFinal] = fc;

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();

            modData[ModalResultField.Nl_busAux_PS_generated] = Nl_generated * dt;
            modData[ModalResultField.Nl_busAux_PS_consumer] = Nl_consumed * dt;
            var comp = compressorMap.Interpolate(n_ice);
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = comp.FlowRate * dt;
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = comp.PowerOff;
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = comp.PowerOn;

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // constant driving different speed
        modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
        n_ice = n_ice2;
        for (; i < 300; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T2 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            modData[ModalResultField.simulationInterval] = dt;

            modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
            modData[ModalResultField.ICEOn] = true;

            modData[ModalResultField.n_ice_avg] = n_ice;
            modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
            var fc = fuel.ConsumptionMap.GetFuelConsumption(400.SI<NewtonMeter>(), n_ice).Value *
                fuel.FuelConsumptionCorrectionFactor;
            modData[ModalResultField.FCWHTCc] = fc;
            modData[ModalResultField.FCFinal] = fc;

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();

            modData[ModalResultField.Nl_busAux_PS_generated] = Nl_generated * dt;
            modData[ModalResultField.Nl_busAux_PS_consumer] = Nl_consumed * dt;
            var comp = compressorMap.Interpolate(n_ice);
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = comp.FlowRate * dt;
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = comp.PowerOff;
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = comp.PowerOn;


            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        modData.Finish(VectoRun.Status.Success);

        // fake that the actual air deman is different than initially assumed
        var NlConsumedCorrected = nlConsumedCorrected.SI<NormLiterPerSecond>();
        (runData.BusAuxiliaries.PneumaticAuxiliariesConfig as PneumaticsConsumersDemand).AirControlledSuspension = NlConsumedCorrected;

        var corr = modData.CorrectedModalData as CorrectedModalData;

        var k_engline = 2.6254521511724e-8;

        var airDemand = (NlConsumedCorrected * (T1 + T2)); // 75.750978609710458 ["Nl"]
                                                           // 106.05137005359464 ["Nl"]
                                                           // 121.20156577553674 ["Nl"]
        var deltaAir = (airDemand - Nl_generated * (T1 + T2)).Value(); // -30.30039144388418
                                                                       // 0
                                                                       // 15.150195721942097

        var comp1 = compressorMap.Interpolate(n_ice1); // rpm: 800 4.1756093266666667 ["Nl/s"] 3139.5 ["W"] 340.79499999999996 ["W"] (exceeded: false)
        var comp3 = compressorMap.Interpolate(n_ice2); // rpm: 1200 6.23922331 ["Nl/s"] 4609.5 ["W"] 667.68000000000006 ["W"] (exceeded: false)

        var workCompOff = comp1.PowerOff * T1 + comp3.PowerOff * T2; // 84766.407634845338 ["Ws"]
        var workCompOn = comp1.PowerOn * T1 + comp3.PowerOn * T2; // 624649.6433333247 ["Ws"]
        var airOn = comp1.FlowRate * T1 + comp3.FlowRate * T2; // 841.79500540060076 ["Nl"]

        var kAir = ((workCompOn - workCompOff) / airOn).Value(); // 641347.63479804085

        var E_busAuxPS = kAir * deltaAir / 1e3; // -19433.084385989911  convert Nl to m^3  
                                                // 0 
                                                // 9716.54219299496

        var fcPSAir = k_engline * E_busAuxPS; // -0.00051020633205111981
                                              // 0
                                              // 0.00025510316602556007
        var fcPSICEOffDriving = 0;

        var fcESS = 0;

        var fcModSum = 0.306799952;

        Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
        Assert.AreEqual(0, corr.WorkWHREl.Value());
        Assert.AreEqual(0, corr.WorkWHRElMech.Value());
        Assert.AreEqual(0, corr.WorkWHRMech.Value());

        Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

        Assert.AreEqual(0, (modData.AirConsumed() - modData.AirGenerated()).Value(), 1e-6);
        Assert.AreEqual(airDemand.Value(), corr.CorrectedAirDemand.Value(), 1e-6);
        Assert.AreEqual(deltaAir, corr.DeltaAir.Value(), 1e-6);

        Assert.AreEqual(kAir, corr.kAir.Value(), 1e-3);
        Assert.AreEqual(E_busAuxPS, corr.WorkBusAuxPSCorr.Value(), 1e-3);

        var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
        Assert.AreEqual(fcESS, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);


        Assert.AreEqual(fcPSAir, f.FcBusAuxPSAirDemand.Value(), 1e-12);
        Assert.AreEqual(0, f.FcBusAuxPSDragICEOffStandstill.Value(), 1e-12);
        Assert.AreEqual(fcPSICEOffDriving, f.FcBusAuxPSDragICEOffDriving.Value(), 1e-12);
        Assert.AreEqual(fcPSAir + fcPSICEOffDriving, f.FcBusAuxPs.Value(), 1e-12);

        Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcESS, f.FcEssCorr.Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcPSAir + fcPSICEOffDriving + fcESS, f.FcBusAuxPsCorr.Value(), 1e-6);

        Assert.AreEqual(fcModSum + fcPSAir + fcPSICEOffDriving + fcESS, f.FcFinal.Value(), 1e-6);
    }

}