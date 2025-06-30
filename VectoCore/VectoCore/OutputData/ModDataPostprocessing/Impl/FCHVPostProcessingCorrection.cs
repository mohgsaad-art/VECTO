using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Castle.DynamicProxy.Contributors;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
    public class FCHVPostProcessingCorrection : ModalDataPostProcessingCorrectionBase
    {
		#region Implementation of IModalDataPostProcessor

		public Joule FCHVElectricEnergyConsumptionSoC { get; set; } = null;

		public override ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			if (modData.Duration.IsEqual(0))
			{
				return new FCHVCorrectedModalData(modData);
			}

			/// Determines if this is a pre-run simulation for FCHVs, in that case, a PEV correction is applied.
			///		FuelCellSystemData == null -> Simulated vehicle is not a FCHV and therefore a PEV.
			///		Iteration == 0 -> Run is a pre-run simulation.
			bool isPEVPreRunSimulation = runData.FuelCellSystemData == null && runData.Iteration == 0;
			if (isPEVPreRunSimulation)
			{
				return new BatteryElectricPostprocessingCorrection().ApplyCorrection(modData, runData);
			}

			return DoApplyFCHVCorrection(modData, runData);
		}

		public static WattSecond CalculateElectricEnergyConsumption(IModalDataContainer modData)
		{
			return -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int);
		}

		private ICorrectedModalData DoApplyFCHVCorrection(IModalDataContainer modData, VectoRunData runData)
		{

			var deltaEPSel = 0.SI<WattSecond>();
			var airDemandCorr = 0.SI<NormLiter>();
			var deltaAir = 0.SI<NormLiter>();
			if (runData.BusAuxiliaries != null && runData.BusAuxiliaries.PneumaticUserInputsConfig.CompressorMap == null)
			{
				// no compressor map - electric compressor
				var actuations = new Actuations()
				{
					Braking = runData.BusAuxiliaries.Actuations.Braking,
					ParkBrakeAndDoors = runData.BusAuxiliaries.Actuations.ParkBrakeAndDoors,
					Kneeling = runData.BusAuxiliaries.Actuations.Kneeling,
					CycleTime = modData.Duration
				};
				airDemandCorr = M03Impl.TotalAirDemandCalculation(runData.BusAuxiliaries, actuations);
				deltaAir = airDemandCorr - modData.AirGenerated();
				deltaEPSel = deltaAir * DeclarationData.BusAuxiliaries.PneumaticSystemElectricDemandPerAirGenerated /
							runData.DCDCData.DCDCEfficiency;

			}

			var electricEnergyConsumption_SoC = FCHVElectricEnergyConsumptionSoC ?? CalculateElectricEnergyConsumption(modData);
			
			var corrected = new FCHVCorrectedModalData(modData)
			{
				CorrectedAirDemand = airDemandCorr,
				DeltaAir = deltaAir,
				WorkBusAux_elPS_SoC_ElRange = deltaEPSel,
				ElectricEnergyConsumption_SoC = electricEnergyConsumption_SoC,
				ElectricEnergyConsumption_SoC_Corr = electricEnergyConsumption_SoC
			};

			SetReessCorrectionDemand(modData, runData, corrected);
			SetFuelConsumptionCorrection(modData, runData, corrected);
			SetAuxHeaterDemand(modData, runData, corrected);

			if (corrected.AuxHeaterDemand?.IsGreater(0) ?? false)
			{
				var f = FuelData.Diesel;
				var fc = new AuxHeaterFuelConsumptionCorrection(
					fuel: f,
					distance: modData.Distance,
					duration: modData.Duration,
					fcAuxHeater: corrected.AuxHeaterDemand / f.LowerHeatingValueVecto);


				corrected.FuelCorrection[f.FuelType] = fc;
			}

			return corrected;
		}

		private void SetFuelConsumptionCorrection(IModalDataContainer modData, VectoRunData runData,
			FCHVCorrectedModalData corrected)
		{
			var f = FuelData.H2;

			var fcLine = modData.FuelCellLine;
			var fc = new FuelCellFuelConsumptionCorrection(
				fuel: f,
				fuelCellLine: modData.FuelCellLine,
				fcReessSoc: corrected.DeltaEReessFuelCell * fcLine,
				fcMap: modData.TimeIntegral<Kilogram>(ModalResultField.FC_FCSystem),
				duration: modData.Duration,
				distance: modData.Distance
			);
			corrected.FuelCorrection[f.FuelType] = fc;
		}


		private void SetReessCorrectionDemand(IModalDataContainer modData, VectoRunData runData, FCHVCorrectedModalData corrected)
		{
			var startSoc = modData.REESSStartSoC();
			var endSoc = modData.REESSEndSoC();
            if ((endSoc - startSoc).IsEqual(0)) {
				//Set everything to zero
				corrected.DeltaEReessFuelCell = 0.SI<WattSecond>();
			}

			var batEff = 1d;
			if (endSoc.IsSmaller(startSoc)) {
				//Bat charge eff

                Debug.Assert(modData.REESSEndSoC() < modData.REESSStartSoC());
				var etaReessChg = modData.WorkREESSChargeInternal().Value() /
								modData.WorkREESSChargeTerminal_ES().Value();
				batEff = 1.0 / etaReessChg;
			} else {
				//Bat discharge eff
                var etaReessDischg = modData.WorkREESSDischargeTerminal_ES().Value() /
									modData.WorkREESSDischargeInternal().Value();
				batEff = etaReessDischg;
            }

			corrected.DeltaEReessFuelCell = batEff * corrected.ElectricEnergyConsumption_SoC_Corr; //includes work ps bus aux
		}

		#endregion



    }
}