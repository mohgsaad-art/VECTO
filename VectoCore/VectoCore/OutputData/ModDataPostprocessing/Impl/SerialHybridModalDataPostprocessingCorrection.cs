using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
    public class SerialHybridModalDataPostprocessingCorrection : ConventionalModalDataPostprocessingCorrection
    {
        #region Overrides of ModalDataPostprocessingCorrection

        protected override CorrectedModalData DoApplyCorrection(IModalDataContainer modData, VectoRunData runData)
        {
            var r = base.DoApplyCorrection(modData, runData);

            var etaChtBatWeighted = 1.0;
            var electricEnergyConsumption = 0.SI<WattSecond>();

            if (runData.OVCMode == OvcHevMode.ChargeDepleting && runData.Mission != null) {
                var vehicleOperation = DeclarationData.VehicleOperation.LookupVehicleOperation(runData.Mission.BusParameter?.BusGroup ?? runData.VehicleData.VehicleClass, runData.Mission.MissionType);
                etaChtBatWeighted =
                    DeclarationData.CalculateChargingEfficiencyOVCHEV(runData, vehicleOperation).EtaChargingWeighted;
                electricEnergyConsumption = -modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int);
            }

			r.ElectricEnergyConsumption_SoC = electricEnergyConsumption;
            r.ElectricEnergyConsumption_Final = electricEnergyConsumption / etaChtBatWeighted;
            return r;
        }

        #endregion

        protected override void SetReesCorrectionDemand(IModalDataContainer modData, VectoRunData runData,
            CorrectedModalData r)
        {
			if (runData.OVCMode != OvcHevMode.ChargeDepleting) {
				var deltaEReess = modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int) - r.WorkBusAux_elPS_SoC_Corr;
				var startSoc = modData.REESSStartSoC();
				var endSoc = modData.REESSEndSoC();
				var emEff = 0.0;
				if (endSoc < startSoc) {
					var etaReessChg = modData.WorkREESSChargeInternal().Value() /
									modData.WorkREESSChargeTerminal_ES().Value();
					emEff = 1.0 / etaReessChg;
				}

				if (endSoc > startSoc) {
					var etaReessDischg = modData.WorkREESSDischargeTerminal_ES().Value() /
										modData.WorkREESSDischargeInternal().Value();
					emEff = etaReessDischg;
				}

				r.DeltaEReessMech = double.IsNaN(emEff) ? 0.SI<WattSecond>() : -deltaEReess * emEff;
			} else {
				r.DeltaEReessMech = 0.SI<WattSecond>();
			}
		}

        protected override FuelConsumptionCorrection SetFuelConsumptionCorrection(IModalDataContainer modData, VectoRunData runData,
            CorrectedModalData r, IFuelProperties fuel)
        {
            var duration = modData.Duration;
            var distance = modData.Distance;
            var essParams = runData.DriverData.EngineStopStart;
            var engFuel = runData.EngineData.Fuels.First(x => x.FuelData.Equals(fuel));

            var fcIceIdle = engFuel.ConsumptionMap.GetFuelConsumptionValue(
                                0.SI<NewtonMeter>(),
                                runData.EngineData.IdleSpeed) *
                            engFuel.FuelConsumptionCorrectionFactor;

            var elPowerGenerated = modData.TimeIntegral<WattSecond>(string.Format(ModalResultField.P_EM_electricMotor_el_.GetCaption(), PowertrainPosition.GEN));
            var fcGenCharging = modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel);
            var socCorr = elPowerGenerated.IsEqual(0)
                ? (runData.GenSet.GenSetCharacteristics.OptimalPoint.FuelConsumption /
                    runData.GenSet.GenSetCharacteristics.OptimalPoint.ElectricPower).Cast<KilogramPerWattSecond>()
                : (fcGenCharging / elPowerGenerated).Cast<KilogramPerWattSecond>();
            var engLine = modData.EngineLineCorrectionFactor(fuel);
            var comp =
                runData.BusAuxiliaries?.PneumaticUserInputsConfig.CompressorMap?
                    .Interpolate(runData.EngineData.IdleSpeed);

            var f = new FuelConsumptionCorrection {
                Fuel = fuel,
                Distance = distance != null && distance.IsGreater(0) ? distance : null,
                Duration = duration != null && duration.IsGreater(0) ? duration : null,
                EngineLineCorrectionFactor = engLine,
                VehicleLine = modData.VehicleLineSlope(fuel),
                FcModSum = modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel),
                FcESS_EngineStart = engLine * modData.WorkEngineStart(),

                FcESS_AuxStandstill_ICEOff = r.EnergyAuxICEOffStandstill_UF * engLine,
                FcESS_AuxStandstill_ICEOn = r.EnergyAuxICEOnStandstill_UF * engLine +
                                            fcIceIdle * r.ICEOffTimeStandstill * (1 - r.UtilityFactorStandstill),

                FcESS_AuxDriving_ICEOff = r.EnergyAuxICEOffDriving_UF * engLine,
                FcESS_AuxDriving_ICEOn = r.EnergyAuxICEOnDriving_UF * engLine +
                                        fcIceIdle * r.ICEOffTimeDriving * (1 - r.UtilityFactorDriving),

                FcESS_DCDCMissing = r.EnergyDCDCMissing * engLine,
                FcBusAuxPSAirDemand = engLine * r.WorkBusAuxPSCorr,

                FcBusAuxPSDragICEOffStandstill = comp == null
                    ? 0.SI<Kilogram>()
                    : comp.PowerOff * r.ICEOffTimeStandstill * engLine * (1 - essParams.UtilityFactorStandstill),
                FcBusAuxPSDragICEOffDriving = comp == null
                    ? 0.SI<Kilogram>()
                    : comp.PowerOff * r.ICEOffTimeDriving * engLine * (1 - essParams.UtilityFactorDriving),
                FcREESSSoc = r.DeltaEReessMech * socCorr,
                FcBusAuxEs = engLine * r.WorkBusAuxESMech,
                FcHeatPumpHeatingEl = engLine * r.WorkBusAuxHeatPumpHeatingElMech,
                FcHeatPumpHeatingMech = engLine * r.WorkBusAuxHeatPumpHeatingMech,
                FcBusAuxEletcricHeater = engLine * r.WorkBusAuxElectricHeater,
				FcBusAuxElPS = engLine * r.WorkBusAux_elPS_Corr_mech,
                FcWHR = engLine * r.WorkWHR,
                FcAuxHtr = 0.SI<Kilogram>()
            };

            return f;
        }
    }
}