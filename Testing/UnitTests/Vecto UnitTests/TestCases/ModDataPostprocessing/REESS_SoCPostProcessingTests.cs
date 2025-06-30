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

namespace TUGraz.Vecto.UnitTests.TestCases.ModDataPostprocessing;

public class REESS_SoCPostProcessingTests
{
	const double emEff = 0.95;
	const double batEff = 0.982;

	private AmpereSecond BatCapacity = 10000.SI<AmpereSecond>();

	private StandardKernel _kernel;

	[OneTimeSetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
	}

    [TestCase(10),
         TestCase(-10)]
    public void TestREESSoC_ModDataCorrection(double batPowerDemand)
    {
        var runData = PostProcessingRunData.GetRunData(true, alternatorType: AlternatorType.Smart);
        runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
        var writer = new FileOutputWriter(".");
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, writer, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);

        modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
        modData.Data.CreateCombustionEngineColumns(runData);
        modData.Data.CreateColumns(ModalResults.DriverSignals);
        modData.Data.CreateColumns(ModalResults.BusAuxiliariesSignals);
        modData.Data.CreateColumns(ModalResults.DCDCConverterSignals);
        modData.Data.CreateColumns(ModalResults.BatterySignals);
        modData.Data.CreateElectricMotorColumns(runData.ElectricMachinesData.First().Item1, null, ModalResults.ElectricMotorSignals);
        //modData.AddElectricMotor(emPos);

        var fuel = runData.EngineData.Fuels[0];

        var absTime = 0.SI<Second>();
        var rnd = new Random(210629);
        var dt = 0.SI<Second>();

        var i = 0;
        // constant driving
        var n_ice = 800.RPMtoRad();
        var t_ice = 400.SI<NewtonMeter>();

        var batConditioning = (Math.Sign(batPowerDemand) * -1e3).SI<Watt>();
        var P_bat = batConditioning;
        var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
        var T2 = 0.SI<Second>(); // 101.36683581153247 ["s"]
        var soc = 0.5;

        for (; i < 100; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T1 += dt;
            soc += (P_bat / 600.SI<Volt>() * dt) / BatCapacity;

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

            modData[ModalResultField.P_reess_int] = P_bat;
            modData[ModalResultField.P_reess_terminal] = P_bat * (P_bat < 0 ? batEff : 1 / batEff);
            modData[ModalResultField.P_terminal_ES] = P_bat * (P_bat < 0 ? batEff : 1 / batEff);
            modData[ModalResultField.REESSStateOfCharge] = soc.SI();


            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        P_bat = (batPowerDemand * 1e3).SI<Watt>();

        // constant driving different speed
        modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
        n_ice = 1200.RPMtoRad();
        for (; i < 300; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T2 += dt;
            soc += (P_bat / 600.SI<Volt>() * dt) / BatCapacity;

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

            modData[ModalResultField.P_EM_electricMotor_el_, PostProcessingRunData.emPos] = -120.SI<Watt>() / emEff;
            modData[ModalResultField.P_EM_mech_, PostProcessingRunData.emPos] = -120.SI<Watt>();
            modData[ModalResultField.EM_Off_, PostProcessingRunData.emPos] = 0.SI<Scalar>();

            modData[ModalResultField.P_reess_int] = P_bat;
            modData[ModalResultField.P_reess_terminal] = P_bat * (P_bat < 0 ? batEff : 1 / batEff);
            modData[ModalResultField.P_terminal_ES] = P_bat * (P_bat < 0 ? batEff : 1 / batEff);
            modData[ModalResultField.REESSStateOfCharge] = soc.SI();

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        modData.Finish(VectoRun.Status.Success);
        var corr = modData.CorrectedModalData as CorrectedModalData;

        var k_engline = 2.6254521511724e-8;
        var E_REESS = (T2 * P_bat + batConditioning * T1).Value();  // 963533.23670743615 
        var E_REESS_mech = -E_REESS * (P_bat > 0 ? emEff * batEff : 1 / (emEff * batEff));      // -898880.15652436716

        var fcModSum = 0.306799952;
        var fcReessSoc = E_REESS_mech * k_engline;  // -0.023599668405930831

        Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
        Assert.AreEqual(0, corr.WorkWHREl.Value(), 1e-3);
        Assert.AreEqual(0, corr.WorkWHRElMech.Value(), 1e-3);
        Assert.AreEqual(0, corr.WorkWHRMech.Value());

        Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyDCDCMissing.Value(), 1e-3);

        Assert.AreEqual(E_REESS, modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int.GetName()).Value(), 1e-6);
        Assert.AreEqual(E_REESS_mech, corr.DeltaEReessMech.Value(), 1e-6);

        var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

        Assert.AreEqual(0, f.FcWHR.Value(), 1e-12);

        Assert.AreEqual(fcReessSoc, f.FcREESSSoc.Value(), 1e-12);

        Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcReessSoc, f.FcREESSSoCCorr.Value(), 1e-6);

        Assert.AreEqual(fcModSum + fcReessSoc, f.FcFinal.Value(), 1e-6);

    }
}