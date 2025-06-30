using System.Diagnostics;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;

namespace TUGraz.Vecto.UnitTests.TestCases.ModDataPostprocessing;

public class LorryESSModDataCorrectionTests
{
	private StandardKernel _kernel;

	[OneTimeSetUp]
	public void Setup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
	}

    [TestCase()]
    public void TestAuxESSStandstill_ModDataCorrection()
    {
        var runData = PostProcessingRunData.GetRunData();
        runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
        var writer = new FileOutputWriter(".");
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, null, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);


        modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
        modData.Data.CreateCombustionEngineColumns(runData);
        modData.Data.CreateColumns(ModalResults.DriverSignals);
        modData.Data.CreateColumns(ModalResults.WheelSignals);
        var fuel = runData.EngineData.Fuels[0];

        var absTime = 0.SI<Second>();
        var rnd = new Random(210629);
        var dt = 0.SI<Second>();

        var i = 0;
        // constant driving
        var n_ice = 800.RPMtoRad();
        var t_ice = 400.SI<NewtonMeter>();
        var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
        var T2 = 0.SI<Second>(); // 51.171787824561704 ["s"]
        var T3 = 0.SI<Second>(); // 50.195047986970792 ["s"]

        var P_off = 300.SI<Watt>();
        var P_on = 900.SI<Watt>();

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

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = P_off;
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = P_on;

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // constant driving different speed
        modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
        n_ice = 1200.RPMtoRad();
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
        var E_auxICEOnStandstill = (P_on * T2).Value(); // 46054.609042105534
        var E_auxICEOffStandstill = (P_off * T2).Value(); // 15351.536347368512

        var fcEssStandStillOff = E_auxICEOffStandstill * k_engline * PostProcessingRunData.UF_ESS_Standstill; // 0.0003034945726763096
        var fcEssStandStillOn =
            (E_auxICEOnStandstill * k_engline + (fcIdle * T2).Value()) * (1 - PostProcessingRunData.UF_ESS_Standstill); // 0.00400097222027925

        var fcModSum = 0.195725;

        Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
        Assert.AreEqual(0, corr.WorkWHREl.Value());
        Assert.AreEqual(0, corr.WorkWHRElMech.Value());
        Assert.AreEqual(0, corr.WorkWHRMech.Value());

        Assert.AreEqual(E_auxICEOnStandstill, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
        Assert.AreEqual(E_auxICEOffStandstill, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

        Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

        var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

        Assert.AreEqual(fcEssStandStillOff, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
        Assert.AreEqual(fcEssStandStillOn, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

        Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcEssStandStillOff + fcEssStandStillOn, f.FcEssCorr.Value(), 1e-6);

        Assert.AreEqual(fcModSum + fcEssStandStillOff + fcEssStandStillOn, f.FcFinal.Value(), 1e-6);
    }


    [TestCase()]
    public void TestAuxESSDriving_ModDataCorrection()
    {
        var runData = PostProcessingRunData.GetRunData();
        runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
        var writer = new FileOutputWriter(".");
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, null, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);

        modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
        modData.Data.CreateCombustionEngineColumns(runData);
        modData.Data.CreateColumns(ModalResults.DriverSignals);
        modData.Data.CreateColumns(ModalResults.WheelSignals);
        var fuel = runData.EngineData.Fuels[0];

        var absTime = 0.SI<Second>();
        var rnd = new Random(210629);
        var dt = 0.SI<Second>();

        var i = 0;
        // constant driving
        var n_ice = 800.RPMtoRad();
        var t_ice = 400.SI<NewtonMeter>();
        var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
        var T2 = 0.SI<Second>(); // 51.171787824561704 ["s"]
        var T3 = 0.SI<Second>(); // 50.195047986970792 ["s"]

        var P_off = 300.SI<Watt>();
        var P_on = 900.SI<Watt>();

        for (; i < 100; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T1 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            //modData[ModalResultField.dist] = i.SI<Meter>();
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

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // driving ICE Off
        for (; i < 200; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T2 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            //modData[ModalResultField.dist] = i.SI<Meter>();
            modData[ModalResultField.simulationInterval] = dt;

            modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
            modData[ModalResultField.ICEOn] = false;

            modData[ModalResultField.n_ice_avg] = 0.RPMtoRad();
            modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
            modData[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
            modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = P_off;
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = P_on;

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // constant driving different speed
        modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
        n_ice = 1200.RPMtoRad();
        for (; i < 300; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T3 += dt;

            modData[ModalResultField.time] = absTime + dt / 2;
            //modData[ModalResultField.dist] = i.SI<Meter>();
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

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        modData.Finish(VectoRun.Status.Success);
        var corr = modData.CorrectedModalData as CorrectedModalData;

        var k_engline = 2.6254521511724e-8;
        var E_auxICEOnDriving = (P_on * T2).Value(); // 46054.609042105534;
        var E_auxICEOffDriving = (P_off * T2).Value(); // 15351.536347368512

        var fcIdle =
            fuel.ConsumptionMap.GetFuelConsumption(0.SI<NewtonMeter>(), runData.EngineData.IdleSpeed).Value
                .Value() * fuel.FuelConsumptionCorrectionFactor; // 0.0002929177777777778

        var fcEssDrivingOff = E_auxICEOffDriving * k_engline * PostProcessingRunData.UF_ESS_Driving; // 0.00033090178508266954;
        var fcEssDrivingOn = (E_auxICEOnDriving * k_engline + (fcIdle * T2).Value()) * (1 - PostProcessingRunData.UF_ESS_Driving); // 0.002899489989595085;

        var fcModSum = 0.195725;

        Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
        Assert.AreEqual(0, corr.WorkWHREl.Value());
        Assert.AreEqual(0, corr.WorkWHRElMech.Value());
        Assert.AreEqual(0, corr.WorkWHRMech.Value());

        Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

        Assert.AreEqual(E_auxICEOnDriving, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
        Assert.AreEqual(E_auxICEOffDriving, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

        var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
        Assert.AreEqual(fcEssDrivingOff, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
        Assert.AreEqual(fcEssDrivingOn, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

        Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcEssDrivingOff + fcEssDrivingOn, f.FcEssCorr.Value(), 1e-6);

        Assert.AreEqual(fcModSum + fcEssDrivingOff + fcEssDrivingOn, f.FcFinal.Value(), 1e-6);
    }

    [TestCase()]
    public void TestAuxESSEngineStart_ModDataCorrection()
    {
        var runData = PostProcessingRunData.GetRunData();
        runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
        var writer = new FileOutputWriter(".");
		var modData = _kernel.Get<IModalDataFactory>().CreateModDataContainer(runData, null, null, null) as ModalDataContainer;
		Assert.IsNotNull(modData);


        modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
        modData.Data.CreateCombustionEngineColumns(runData);
        modData.Data.CreateColumns(ModalResults.DriverSignals);
        modData.Data.CreateColumns(ModalResults.WheelSignals);
        var fuel = runData.EngineData.Fuels[0];

        var absTime = 0.SI<Second>();
        var rnd = new Random(210629);
        Second dt;

        var i = 0;
        // constant driving
        var n_ice = 800.RPMtoRad();
        var t_ice = 400.SI<NewtonMeter>();
        var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
        var T2 = 0.SI<Second>(); // 51.171787824561704 ["s"]
        var T3 = 0.SI<Second>(); // 50.195047986970792 ["s"]
        Second tStart = null;

        var P_off = 300.SI<Watt>();
        var P_on = 900.SI<Watt>();
        var P_ICEStart = 20000.SI<Watt>();

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

            modData[ModalResultField.P_aux_ESS_mech_ice_off] = P_off;
            modData[ModalResultField.P_aux_ESS_mech_ice_on] = P_on;

            // WHR
            modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
            modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

            modData.CommitSimulationStep();
            absTime += dt;
        }

        // constant driving different speed
        modData[ModalResultField.P_ice_start] = P_ICEStart;
        n_ice = 1200.RPMtoRad();
        for (; i < 300; i++) {
            dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
            T3 += dt;
            if (tStart == null)
                tStart = dt;

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
        var E_auxICEOnStandstill = (P_on * T2).Value(); // 46054.609042105534
        var E_auxICEOffStandstill = (P_off * T2).Value(); // 15351.536347368512
        var E_ICEStart = (P_ICEStart * tStart).Value(); // 11525.53891554733


        var fcEssStandStillOff = E_auxICEOffStandstill * k_engline * PostProcessingRunData.UF_ESS_Standstill; // 0.0003034945726763096
        var fcEssStandStillOn =
            (E_auxICEOnStandstill * k_engline + (fcIdle * T2).Value()) * (1 - PostProcessingRunData.UF_ESS_Standstill); // 0.00400097222027925
        var fcEngineStart = E_ICEStart * k_engline; // 0.00030259750939244944

        var fcModSum = 0.195725;

        Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
        Assert.AreEqual(0, corr.WorkWHREl.Value());
        Assert.AreEqual(0, corr.WorkWHRElMech.Value());
        Assert.AreEqual(0, corr.WorkWHRMech.Value());

        Assert.AreEqual(E_auxICEOnStandstill, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
        Assert.AreEqual(E_auxICEOffStandstill, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);
        Assert.AreEqual(E_ICEStart, modData.WorkEngineStart().Value());

        Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
        Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

        var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

        Assert.AreEqual(fcEssStandStillOff, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
        Assert.AreEqual(fcEssStandStillOn, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
        Assert.AreEqual(fcEngineStart, f.FcESS_EngineStart.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
        Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

        Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
        Assert.AreEqual(fcModSum + fcEssStandStillOff + fcEssStandStillOn + fcEngineStart, f.FcEssCorr.Value(), 1e-6);

        Assert.AreEqual(fcModSum + fcEssStandStillOff + fcEssStandStillOn + fcEngineStart, f.FcFinal.Value(), 1e-6);
    }

}