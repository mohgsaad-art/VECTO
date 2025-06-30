using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Reports
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ModDataPostprocessingTest
	{
		private double busAuxAlternatorEff = 0.753;
        private AmpereSecond BatCapacity = 10000.SI<AmpereSecond>();
		const double dcdc_efficiency = 0.926;
		const double UF_ESS_Driving = 0.821;
		const double UF_ESS_Standstill = 0.753;
		const double emEff = 0.95;
		const double batEff = 0.982;

		private const PowertrainPosition emPos = PowertrainPosition.HybridP2;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestAuxESSStandstill_ModDataCorrection()
		{
			var runData = GetRunData();
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

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

			var fcEssStandStillOff = E_auxICEOffStandstill * k_engline * UF_ESS_Standstill; // 0.0003034945726763096
			var fcEssStandStillOn =
				(E_auxICEOnStandstill * k_engline + (fcIdle * T2).Value()) * (1 - UF_ESS_Standstill); // 0.00400097222027925

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
			Assert.AreEqual(fcModSum + fcEssStandStillOff + fcEssStandStillOn , f.FcEssCorr.Value(), 1e-6);
			
			Assert.AreEqual(fcModSum + fcEssStandStillOff + fcEssStandStillOn, f.FcFinal.Value(), 1e-6);
		}



		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestAuxESSDriving_ModDataCorrection()
		{
			var runData = GetRunData();
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

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

			var fcEssDrivingOff = E_auxICEOffDriving * k_engline * UF_ESS_Driving; // 0.00033090178508266954;
			var fcEssDrivingOn = (E_auxICEOnDriving * k_engline + (fcIdle * T2).Value()) * (1 - UF_ESS_Driving); // 0.002899489989595085;

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

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestAuxESSEngineStart_ModDataCorrection()
		{
			var runData = GetRunData();
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

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
				if (tStart == null) tStart = dt;

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


			var fcEssStandStillOff = E_auxICEOffStandstill * k_engline * UF_ESS_Standstill; // 0.0003034945726763096
			var fcEssStandStillOn =
				(E_auxICEOnStandstill * k_engline + (fcIdle * T2).Value()) * (1 - UF_ESS_Standstill); // 0.00400097222027925
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

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestBusAuxPsESSStandstill_ModDataCorrection()
		{
			var runData = GetRunData(true);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

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
			var fcPSICEOffStop = (comp2.PowerOff * k_engline * (1-UF_ESS_Standstill) * T2).Value(); // 0.00011309017231128844

			var fcESS = fcIdle * T2.Value() * (1 - UF_ESS_Standstill); // 0.0037023142144981809

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

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestBusAuxPsESSDriving_ModDataCorrection()
		{
			var runData = GetRunData(true);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

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
			var fcPSICEOffDriving = (comp2.PowerOff * k_engline * (1 - UF_ESS_Driving) * T2).Value(); // 8.1956035804536988E-05

			var fcESS = fcIdle * T2.Value() * (1 - UF_ESS_Driving); // 0.0026830536210330951

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

		[TestCase(500, 0, 4000, AlternatorType.Conventional),
		TestCase(500, 500, 4000, AlternatorType.Conventional),
		TestCase(500, 0, 550, AlternatorType.Conventional),
		TestCase(500, 0, 4000, AlternatorType.None),
		TestCase(500, 500, 4000, AlternatorType.None),
		TestCase(500, 0, 550, AlternatorType.None),
			Category(Definitions.TESTCASE_MIGRATED)
		]
		public void TestBusAuxSmartES_ModDataCorrection(double p_es_cons, double p_es_gen, double p_es_smartgen,
			AlternatorType alternatorType)
		{
			var runData = GetRunData(true, alternatorType: alternatorType);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name + $"_{p_es_cons}_{p_es_gen}_{p_es_smartgen}";
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};
			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
			modData.Data.CreateCombustionEngineColumns(runData);
			modData.Data.CreateColumns(ModalResults.DriverSignals);
			modData.Data.CreateColumns(ModalResults.BusAuxiliariesSignals);
			modData.Data.CreateColumns(ModalResults.DCDCConverterSignals);
			modData.Data.CreateColumns(ModalResults.BatterySignals);

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

			var P_ES_cons = p_es_cons.SI<Watt>();
			var P_ES_gen = p_es_gen.SI<Watt>();
			var P_ES_smartGen = p_es_smartgen.SI<Watt>();

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

				modData[ModalResultField.P_busAux_ES_generated] = alternatorType == AlternatorType.None ? 0.SI<Watt>() : P_ES_gen;
				modData[ModalResultField.P_busAux_ES_consumer_sum] = P_ES_cons;

				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

				modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
				modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
				modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
				modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();


				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			// driving battery empty
			for (; i < 200; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
				T2 += dt;

				modData[ModalResultField.time] = absTime + dt / 2;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = true;

				modData[ModalResultField.n_ice_avg] = 0.RPMtoRad();
				modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
				modData[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
				modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();

				modData[ModalResultField.P_busAux_ES_generated] = alternatorType == AlternatorType.None ? 0.SI<Watt>() : P_ES_smartGen;
				modData[ModalResultField.P_busAux_ES_consumer_sum] = P_ES_cons;


				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

				modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
				modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
				modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
				modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

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

				modData[ModalResultField.P_busAux_ES_generated] = alternatorType == AlternatorType.None ? 0.SI<Watt>() : P_ES_gen;
				modData[ModalResultField.P_busAux_ES_consumer_sum] = P_ES_cons;


				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

				modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
				modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
				modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
				modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			modData.Finish(VectoRun.Status.Success);
			var corr = modData.CorrectedModalData as CorrectedModalData;

			var k_engline = 2.6254521511724e-8;

			var E_es_missing_mech = 0.0;

			if (alternatorType != AlternatorType.None) {
				E_es_missing_mech = (P_ES_cons * (T1 + T2 + T3) - (P_ES_gen * (T1 + T3) + P_ES_smartGen * T2)).Value() /
									busAuxAlternatorEff; // -171229.97701000841
														// -237850.27541296941
														// 63222.43732563284
			}

			
			var fcModSum = 0.195725; // 

			var fcEs = E_es_missing_mech * k_engline; // -0.0044955611148612719
													  // -0.0062446451723992833
													  // 0.0016598748407894498

			Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
			Assert.AreEqual(0, corr.WorkWHREl.Value());
			Assert.AreEqual(0, corr.WorkWHRElMech.Value());
			Assert.AreEqual(0, corr.WorkWHRMech.Value());

			Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
			Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

			Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
			Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

			Assert.AreEqual(0, corr.EnergyDCDCMissing.Value(), 1e-3);

			Assert.AreEqual(E_es_missing_mech, corr.WorkBusAuxESMech.Value(), 1e-3);

			var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

			Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

			Assert.AreEqual(0, f.FcESS_DCDCMissing.Value(), 1e-12);

			Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
			
			Assert.AreEqual(fcEs, f.FcBusAuxEs.Value(), 1e-12);

            Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
            Assert.AreEqual(fcModSum + fcEs, f.FcBusAuxEsCorr.Value(), 1e-6);

			Assert.AreEqual(fcModSum + fcEs, f.FcFinal.Value(), 1e-6);

		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestBusAuxDCDCMissingConventional_ModDataCorrection()
		{
			var runData = GetRunData(true, alternatorType: AlternatorType.None);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};
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

			var P_DCDC = 3000.SI<Watt>();
			var T1 = 0.SI<Second>(); // 50.135121407888441 ["s"]
			var T2 = 0.SI<Second>(); // 51.171787824561704 ["s"]
			var T3 = 0.SI<Second>(); // 50.195047986970792 ["s"]
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

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			// driving battery empty
			for (; i < 200; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
				T2 += dt;

				modData[ModalResultField.time] = absTime + dt / 2;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = true;

				modData[ModalResultField.n_ice_avg] = 0.RPMtoRad();
				modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
				modData[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
				modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();

				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

				modData[ModalResultField.P_DCDC_missing] = P_DCDC;
				modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
				modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
				modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();

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

				modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
				modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
				modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
				modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			modData.Finish(VectoRun.Status.Success);
			var corr = modData.CorrectedModalData as CorrectedModalData;

			var k_engline = 2.6254521511724e-8;
			var E_dcdc_missing = (T2 * P_DCDC).Value() / emEff / dcdc_efficiency; // 174508.76830019907
			var fcDCDCMissing = E_dcdc_missing * k_engline; // 0.0045816442113220356
			var fcModSum = 0.195725;
			
			Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
			Assert.AreEqual(0, corr.WorkWHREl.Value());
			Assert.AreEqual(0, corr.WorkWHRElMech.Value());
			Assert.AreEqual(0, corr.WorkWHRMech.Value());

			Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
			Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

			Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
			Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

			Assert.AreEqual(E_dcdc_missing, corr.EnergyDCDCMissing.Value(), 1e-3);

			var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

			Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

			Assert.AreEqual(fcDCDCMissing, f.FcESS_DCDCMissing.Value(), 1e-12);

			Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
			Assert.AreEqual(fcModSum + fcDCDCMissing, f.FcEssCorr.Value(), 1e-6);

			Assert.AreEqual(fcModSum + fcDCDCMissing, f.FcFinal.Value(), 1e-6);
		}

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestBusAuxDCDCMissingHEV_ModDataCorrection()
		{
			var runData = GetRunData(true, alternatorType: AlternatorType.Smart);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

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

			var P_DCDC = 3000.SI<Watt>();
			var T1 = 0.SI<Second>();
			var T2 = 0.SI<Second>();
			var T3 = 0.SI<Second>();
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

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			// driving battery empty
			for (; i < 200; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
				T2 += dt;

				modData[ModalResultField.time] = absTime + dt / 2;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = true;

				modData[ModalResultField.n_ice_avg] = 0.RPMtoRad();
				modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
				modData[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
				modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();

				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 300.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 900.SI<Watt>();

				modData[ModalResultField.P_DCDC_missing] = P_DCDC;
				modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
				modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
				modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();

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

				modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
				modData[ModalResultField.Nl_busAux_PS_generated] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_consumer] = 0.SI<NormLiter>();
				modData[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = 0.SI<NormLiter>();
				modData[ModalResultField.P_busAux_PS_generated_dragOnly] = 0.SI<Watt>();
				modData[ModalResultField.P_busAux_PS_generated_alwaysOn] = 0.SI<Watt>();

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			modData.Finish(VectoRun.Status.Success);
			var corr = modData.CorrectedModalData as CorrectedModalData;

			var k_engline = 2.6254521511724e-8;
			var E_dcdc_missing = (T2 * P_DCDC).Value() / emEff / dcdc_efficiency; 
			var fcDCDCMissing = E_dcdc_missing * k_engline;
			var fcModSum = 0.195725;

			Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
			Assert.AreEqual(0, corr.WorkWHREl.Value());
			Assert.AreEqual(0, corr.WorkWHRElMech.Value());
			Assert.AreEqual(0, corr.WorkWHRMech.Value());

			Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
			Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

			Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
			Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

			Assert.AreEqual(E_dcdc_missing, corr.EnergyDCDCMissing.Value(), 1e-3);

			var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

			Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

			Assert.AreEqual(fcDCDCMissing, f.FcESS_DCDCMissing.Value(), 1e-12);

			Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
			Assert.AreEqual(fcModSum + fcDCDCMissing, f.FcEssCorr.Value(), 1e-6);

			Assert.AreEqual(fcModSum + fcDCDCMissing, f.FcFinal.Value(), 1e-6);
		}

		[TestCase()]
		public void TestBusAuxSmartPS_ModDataCorrection()
		{
			var runData = GetRunData(true);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

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

			// smart PS action
			for (; i < 200; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();
				T2 += dt;

				modData[ModalResultField.time] = absTime + dt / 2;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = true;

				modData[ModalResultField.n_ice_avg] = 0.RPMtoRad();
				modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
				modData[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
				modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();

				modData[ModalResultField.P_DCDC_missing] = 0.SI<Watt>();
				var comp = compressorMap.Interpolate(runData.EngineData.IdleSpeed);
				modData[ModalResultField.Nl_busAux_PS_generated] = comp.FlowRate * dt;
				modData[ModalResultField.Nl_busAux_PS_consumer] = Nl_consumed * dt;
				
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

			var k_engline = 2.6254521511724e-8;

			var comp1 = compressorMap.Interpolate(n_ice1); // rad/s: 83.775804095727821 | rpm: 800 4.1756093266666667 ["Nl/s"] 3139.5 ["W"] 340.79499999999996 ["W"] (exceeded: false)
			var comp2 = compressorMap.Interpolate(runData.EngineData.IdleSpeed); // rad/s: 83.775804095727821 | rpm: 800 4.1756093266666667 ["Nl/s"] 3139.5 ["W"] 340.79499999999996 ["W"] (exceeded: true)
			var comp3 = compressorMap.Interpolate(n_ice2); // rad/s: 125.66370614359173 | rpm: 1200 6.23922331 ["Nl/s"] 4609.5 ["W"] 667.68000000000006 ["W"] (exceeded: false)

			var airDemand = (Nl_generated * (T1 + T2 + T3)).Value(); // 106.05137005359465
			var deltaAir = ((Nl_consumed - comp2.FlowRate) * T2).Value(); // -177.85314302525444

			var workCompOff = comp1.PowerOff * T1 + comp3.PowerOff * T3; // 50600.028340142 ["Ws"]
			var workCompOn = comp1.PowerOn * T1 + comp3.PowerOn * T3; // 388773.28735600761 ["Ws"]
			var airOn = comp1.FlowRate * T1 + comp3.FlowRate * T3; // 522.52279399122142 ["Nl"]

			var kAir = ((workCompOn - workCompOff) / airOn).Value(); // 647193.31463566166
			var E_busAuxPS = kAir * deltaAir / 1e3; // -115105.36515288483 convert Nl to m^3

			var fcPSAir = k_engline * E_busAuxPS; // -0.0030220362855212608
			var fcPSICEOffDriving = 0;

			var fcESS = 0;

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
			Category(Definitions.TESTCASE_MIGRATED)
                 ]
		public void TestBusAuxPSDemand_ModDataCorrection(double nlConsumedCorrected)
		{
			var runData = GetRunData(true);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name + $"_{nlConsumedCorrected}";
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};
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

			var airDemand = (NlConsumedCorrected * (T1  + T2)); // 75.750978609710458 ["Nl"]
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

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestWHRElTruckAlternator_ModDataCorrection()
		{
			var runData = GetRunData();
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
			modData.Data.CreateCombustionEngineColumns(runData);
			modData.Data.CreateColumns(ModalResults.DriverSignals);
			modData.Data.CreateColumns(ModalResults.WheelSignals);
			modData.Data.CreateColumns(ModalResults.DCDCConverterSignals);
			modData.Data.CreateColumns(ModalResults.BusAuxiliariesSignals);
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
			var E_WHR_mech = -E_WHR_el / DeclarationData.AlternatorEfficiency; // -64929.410236894684

			var fcWHR = (E_WHR_mech) * k_engline; // -0.0017046905978081038
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

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestWHRElBusAuxAlternator_ModDataCorrection()
		{
			var runData = GetRunData(true, alternatorType: AlternatorType.Conventional);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

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

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();


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

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();

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
			var E_WHR_mech = -E_WHR_el / busAuxAlternatorEff; // -60359.34550574539

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

		[TestCase(),
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestWHRElBusAuxNoAlternator_ModDataCorrection()
		{
			var runData = GetRunData(true, alternatorType: AlternatorType.Smart);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

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

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();


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

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();

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
			var E_WHR_mech = -E_WHR_el / emEff / dcdc_efficiency; // -51666.007918411138

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

		[TestCase,
		Category(Definitions.TESTCASE_MIGRATED)]
		public void TestWHRMech_ModDataCorrection()
		{
			var runData = GetRunData();
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};

			modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
			modData.Data.CreateCombustionEngineColumns(runData);
			modData.Data.CreateColumns(ModalResults.DriverSignals);
			modData.Data.CreateColumns(ModalResults.WheelSignals);
			modData.Data.CreateColumns(ModalResults.DCDCConverterSignals);
			modData.Data.CreateColumns(ModalResults.BusAuxiliariesSignals);
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

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>() ;
				modData[ModalResultField.P_WHR_mech_corr] = P_WHR;

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

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = P_WHR;

				modData.CommitSimulationStep();
				absTime += dt;
			}

			modData.Finish(VectoRun.Status.Success);
			var corr = modData.CorrectedModalData as CorrectedModalData;

			var k_engline = 2.6254521511724e-8;
			var E_WHR_mech = -((T1 + T2) * P_WHR).Value(); // -45450.587165826277

			var fcWHR = (E_WHR_mech) * k_engline; // -0.0011932834184656728
			var fcModSum = 0.306799952;

			Assert.AreEqual(k_engline, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-15);
			Assert.AreEqual(0, corr.WorkWHREl.Value(), 1e-3);
			Assert.AreEqual(0, corr.WorkWHRElMech.Value(), 1e-3);
			Assert.AreEqual(E_WHR_mech, corr.WorkWHRMech.Value(), 1e-3);

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


		[TestCase(10),
		 TestCase(-10)]
		public void TestREESSoC_ModDataCorrection(double batPowerDemand)
		{
			var runData = GetRunData(true, alternatorType: AlternatorType.Smart);
			runData.JobName = new StackTrace().GetFrame(0).GetMethod().Name;
			var writer = new FileOutputWriter(".");
			var modData = new ModalDataContainer(runData, writer, null) {
				WriteModalResults = true
			};
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

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = 120.SI<Watt>() * emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = 120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();

				modData[ModalResultField.P_reess_int] = P_bat;
				modData[ModalResultField.P_reess_terminal] = P_bat * (P_bat < 0 ? batEff : 1/batEff);
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

				modData[ModalResultField.P_EM_electricMotor_el_, emPos] = -120.SI<Watt>() / emEff;
				modData[ModalResultField.P_EM_mech_, emPos] = -120.SI<Watt>();
				modData[ModalResultField.EM_Off_, emPos] = 0.SI<Scalar>();

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
			var E_REESS = (T2 * P_bat + batConditioning * T1).Value();	// 963533.23670743615 
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


		private VectoRunData GetRunData(bool withBusAux = false, bool smartCompressor = false, AlternatorType alternatorType = AlternatorType.Conventional)
		{
			var fcMapHeader = "engine speed [rpm],torque [Nm],fuel consumption [g/h]";
			var fcMapEntries = new[] {
				"600,-107,0",
				"600,0,1042",
				"600,916,10963",
				"2000,-215,0",
				"2000,0,6519",
				"2000,966,40437"
			};

			var retVal = new VectoRunData() {
				DriverData = new DriverData() {
					EngineStopStart = new DriverData.EngineStopStartData() {
						UtilityFactorDriving = UF_ESS_Driving,
						UtilityFactorStandstill = UF_ESS_Standstill,
					}
				},
				EngineData = new CombustionEngineData() {
					IdleSpeed = 600.RPMtoRad(),
					Fuels = new List<CombustionEngineFuelData>() {
						new CombustionEngineFuelData() {
							FuelData = FuelData.Diesel,
							FuelConsumptionCorrectionFactor = 1.012,
							ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(InputDataHelper.InputDataAsStream(fcMapHeader, fcMapEntries))
						}
					}
				},
				Cycle = new DrivingCycleData() {
					CycleType = CycleType.DistanceBased
				}
			};

			if (withBusAux) {
				retVal.BusAuxiliaries = new AuxiliaryConfig() {
					ElectricalUserInputsConfig = new ElectricsUserInputsConfig() {
						ConnectESToREESS = true,
						AlternatorMap = new SimpleAlternator(busAuxAlternatorEff),
						AlternatorType = alternatorType,
						AlternatorGearEfficiency = 1,
						DCDCEfficiency = dcdc_efficiency,
					},
					PneumaticAuxiliariesConfig = CreatePneumaticAuxConfig(0.7.SI<NormLiterPerSecond>()),
					PneumaticUserInputsConfig = CreatePneumaticUserInputsConfig(smartCompressor),
					Actuations = new Actuations() {
						Braking = 0,
						Kneeling = 0,
						ParkBrakeAndDoors = 0,
						CycleTime = 1.SI<Second>()
					},
					VehicleData = new VehicleData() {
						CurbMass = 14000.SI<Kilogram>()
					}
				};
				retVal.ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>() {
					Tuple.Create(emPos, new ElectricMotorData() { Overload = new OverloadData() {OverloadBuffer = 0.SI<Joule>()}})
				};
			}

			return retVal;
		}

		protected internal virtual IPneumaticsConsumersDemand CreatePneumaticAuxConfig(NormLiterPerSecond averageAirDemand)
		{
			return new PneumaticsConsumersDemand() {
				AdBlueInjection = 0.SI<NormLiterPerSecond>(),
				AirControlledSuspension = averageAirDemand,
				Braking = 0.SI<NormLiterPerKilogram>(),
				BreakingWithKneeling = 0.SI<NormLiterPerKilogramMeter>(),
				DeadVolBlowOuts = 0.SI<PerSecond>(),
				DeadVolume = 0.SI<NormLiter>(),
				NonSmartRegenFractionTotalAirDemand = 0,
				SmartRegenFractionTotalAirDemand = 0,
				OverrunUtilisationForCompressionFraction =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
				DoorOpening = 0.SI<NormLiter>(),
				StopBrakeActuation = 0.SI<NormLiterPerKilogram>(),
			};
		}

		protected PneumaticUserInputsConfig CreatePneumaticUserInputsConfig(bool smartCompressor)
		{
			var mock = new Mock<IPneumaticSupplyDeclarationData>();
			mock.Setup(x => x.CompressorDrive).Returns(CompressorDrive.mechanically);
			mock.Setup(x => x.CompressorSize).Returns("Medium Supply 2-stage");
			mock.Setup(x => x.Clutch).Returns("visco");

			return new PneumaticUserInputsConfig() {
				CompressorMap =
					DeclarationData.BusAuxiliaries.GetCompressorMap(mock.Object),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = 1.0,
				SmartAirCompression = smartCompressor,
				SmartRegeneration = false,
				KneelingHeight = 0.SI<Meter>(),
				AirSuspensionControl = ConsumerTechnology.Pneumatically,
				AdBlueDosing = ConsumerTechnology.Electrically,
				Doors = ConsumerTechnology.Electrically
			};
		}
	}
}