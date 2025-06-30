using System.Diagnostics;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModDataPostprocessing;

public class BusAuxWHRElPostProcessingTests
{
	
	const double emEff = 0.95;

	private StandardKernel _kernel;

	[OneTimeSetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
	}


    [TestCase()]
    public void TestWHRElBusAuxAlternator_ModDataCorrection()
    {
        var runData = PostProcessingRunData.GetRunData(true, alternatorType: AlternatorType.Conventional);
        runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
        var writer = new FileOutputWriter(".");
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, writer, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);


        //modData.AddElectricMotor(emPos);
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
        var n_ice = 800.RPMtoRad();
        var t_ice = 400.SI<NewtonMeter>();

        var P_WHR = 300.SI<Watt>();
        var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
        var T2 = 0.SI<Second>(); // 101.36683581153247 ["s"]
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

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
            modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

            modData[ModalResultField.P_EM_electricMotor_el_, PostProcessingRunData.emPos] = 120.SI<Watt>() * emEff;
            modData[ModalResultField.P_EM_mech_, PostProcessingRunData.emPos] = 120.SI<Watt>();


            // WHR
            modData[ModalResultField.P_WHR_el_corr] = P_WHR;
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // constant driving different speed
        modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
        n_ice = 1200.RPMtoRad();
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


            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
            modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

            modData[ModalResultField.P_EM_electricMotor_el_, PostProcessingRunData.emPos] = 120.SI<Watt>() * emEff;
            modData[ModalResultField.P_EM_mech_, PostProcessingRunData.emPos] = 120.SI<Watt>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = P_WHR;
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        modData.Finish(VectoRun.Status.Success);
        var corr = modData.CorrectedModalData as CorrectedModalData;

        var k_engline = 2.6254521511724e-8;
        var E_WHR_el = ((T1 + T2) * P_WHR).Value(); // 45450.587165826277
        var E_WHR_mech = -E_WHR_el / PostProcessingRunData.busAuxAlternatorEff; // -60359.34550574539

        var fcWHR = (E_WHR_mech) * k_engline; // -0.0015847057350141737
        var fcModSum = 0.306799952;

        Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
        Assert.AreEqual(E_WHR_el, corr.WorkWHREl.Value(), 1e-3);
        Assert.AreEqual(E_WHR_mech, corr.WorkWHRElMech.Value(), 1e-3);
        Assert.AreEqual(0, corr.WorkWHRMech.Value());

        Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyDCDCMissing.Value(), 1e-3);

        var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

        Assert.AreEqual(fcWHR, f.FcWHR.Value(), 1e-12);

        Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcWHR, f.FcWHRCorr.Value(), 1e-6);

        Assert.AreEqual(fcModSum + fcWHR, f.FcFinal.Value(), 1e-6);

    }

    [TestCase()]
    public void TestWHRElBusAuxNoAlternator_ModDataCorrection()
    {
        var runData = PostProcessingRunData.GetRunData(true, alternatorType: AlternatorType.Smart);
        runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
        var writer = new FileOutputWriter(".");
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, writer, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);


        //modData.AddElectricMotor(emPos);
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
        var n_ice = 800.RPMtoRad();
        var t_ice = 400.SI<NewtonMeter>();

        var P_WHR = 300.SI<Watt>();
        var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
        var T2 = 0.SI<Second>(); // 101.36683581153247 ["s"]
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

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
            modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

            modData[ModalResultField.P_EM_electricMotor_el_, PostProcessingRunData.emPos] = 120.SI<Watt>() * emEff;
            modData[ModalResultField.P_EM_mech_, PostProcessingRunData.emPos] = 120.SI<Watt>();
            modData[ModalResultField.EM_Off_, PostProcessingRunData.emPos] = 0.SI<Scalar>();


            // WHR
            modData[ModalResultField.P_WHR_el_corr] = P_WHR;
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // constant driving different speed
        modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
        n_ice = 1200.RPMtoRad();
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


            modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

            modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
            modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
            modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
            modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
            modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

            modData[ModalResultField.P_EM_electricMotor_el_, PostProcessingRunData.emPos] = 120.SI<Watt>() * emEff;
            modData[ModalResultField.P_EM_mech_, PostProcessingRunData.emPos] = 120.SI<Watt>();
            modData[ModalResultField.EM_Off_, PostProcessingRunData.emPos] = 0.SI<Scalar>();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = P_WHR;
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        modData.Finish(VectoRun.Status.Success);
        var corr = modData.CorrectedModalData as CorrectedModalData;

        var k_engline = 2.6254521511724e-8;
        var E_WHR_el = ((T1 + T2) * P_WHR).Value(); // 45450.587165826277
        var E_WHR_mech = -E_WHR_el / emEff / PostProcessingRunData.dcdc_efficiency; // -51666.007918411138

        var fcWHR = (E_WHR_mech) * k_engline; // -0.0013564663163188277
        var fcModSum = 0.306799952;

        Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
        Assert.AreEqual(E_WHR_el, corr.WorkWHREl.Value(), 1e-3);
        Assert.AreEqual(E_WHR_mech, corr.WorkWHRElMech.Value(), 1e-3);
        Assert.AreEqual(0, corr.WorkWHRMech.Value());

        Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyDCDCMissing.Value(), 1e-3);

        var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

        Assert.AreEqual(fcWHR, f.FcWHR.Value(), 1e-12);

        Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcWHR, f.FcWHRCorr.Value(), 1e-6);

        Assert.AreEqual(fcModSum + fcWHR, f.FcFinal.Value(), 1e-6);

    }


	
}