using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
    public abstract class ModalDataPostProcessingCorrectionBase : IModalDataPostProcessor
	{
		#region Implementation of IModalDataPostProcessor

		public abstract ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData);

        protected virtual void SetAuxHeaterDemand(IModalDataContainer modData, VectoRunData runData, AbstractCorrectedModalData r)
        {

            if (modData.AuxHeaterDemandCalc == null)
            {
                r.AuxHeaterDemand = 0.SI<Joule>();
                r.WorkBusAuxHeatPumpHeatingMech = 0.SI<WattSecond>();
                r.WorkBusAuxHeatPumpHeatingElMech = 0.SI<WattSecond>();
                r.WorkBusAuxElectricHeater = 0.SI<WattSecond>();
                return;
            }
            var duration = modData.Duration;
            var engineWasteheatSum = modData.FuelData.Aggregate(
                0.SI<Joule>(),
                (current, fuel) => current + modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel) *
                    fuel.LowerHeatingValueVecto);
            var emLossEnergy = 0.SI<Joule>();
            //if (runData.ElectricMachinesData.Count > 0) {
            foreach (var em in runData.ElectricMachinesData)
            {
                var emPos = em.Item1; //runData.ElectricMachinesData.First().Item1;
                var colName = modData.GetColumnName(emPos, ModalResultField.P_EM_electricMotorLoss_);
                if (emPos == PowertrainPosition.IEPC)
                {
                    colName = modData.GetColumnName(emPos, ModalResultField.P_IEPC_electricMotorLoss_);
                }
                emLossEnergy += modData.TimeIntegral<WattSecond>(colName).Cast<Joule>();
            }

            //r.WorkBusAuxHeatPumpHeatingElMech = workBusAuxES /
            //                              runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>()) /
            //                             runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorGearEfficiency;
            var heatingDemand = modData.AuxHeaterDemandCalc(duration, engineWasteheatSum, emLossEnergy);

            r.WorkBusAuxHeatPumpHeatingMech = heatingDemand.HeatPumpMechanicalEnergy;
            r.WorkBusAuxHeatPumpHeatingElMech = heatingDemand.HeatPumpElectricEnergy /
                                                runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>()) /
                                                runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorGearEfficiency;
            r.AuxHeaterDemand = heatingDemand.AuxHeater;
            r.WorkBusAuxElectricHeater = heatingDemand.ElectricHeaterEnergy;
        }
        #endregion
    }

    public class ConventionalModalDataPostprocessingCorrection : ModalDataPostProcessingCorrectionBase
    {

        #region Implementation of IModalDataPostProcessor

		public override ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			return DoApplyCorrection(modData, runData);
		}

		protected virtual CorrectedModalData DoApplyCorrection(IModalDataContainer modData, VectoRunData runData)
        {
            var essParams = runData.DriverData.EngineStopStart;
            var r = new CorrectedModalData(modData) {
                UtilityFactorDriving = essParams.UtilityFactorDriving,
                UtilityFactorStandstill = essParams.UtilityFactorStandstill
            };
            //var duration = modData.Duration;
            var distance = modData.Distance;

            SetMissingEnergyICEOFf(modData, r);

            SetWHRWork(modData, runData, r);

            if (runData.BusAuxiliaries != null) {
                SetBusAuxMissingPSWork(modData, runData, r);

				SetBusAuxMissingPSElWork(modData, runData, r);

                var workBusAuxES = modData.EnergyBusAuxESConsumed() - modData.EnergyBusAuxESGenerated();
                if (runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType == AlternatorType.None) {
                    workBusAuxES = 0.SI<WattSecond>();
                }
                r.WorkBusAuxESMech = workBusAuxES /
                                    runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>()) /
                                    runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorGearEfficiency;

            } else {
                r.WorkBusAuxPSCorr = 0.SI<WattSecond>();
                r.WorkBusAuxESMech = 0.SI<WattSecond>();
            }


            r.EnergyDCDCMissing = 0.SI<WattSecond>();
            if (runData.BusAuxiliaries != null && runData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
                SetMissingDCDCEnergy(modData, runData, r);
            }

            SetAuxHeaterDemand(modData, runData, r);

            SetReesCorrectionDemand(modData, runData, r);

            var kilogramCO2 = 0.SI<Kilogram>();

            var firstFuel = true;
            foreach (var fuel in modData.FuelData) {

                var f = SetFuelConsumptionCorrection(modData, runData, r, fuel);
                if (firstFuel) {
                    firstFuel = false;
                    f.FcAuxHtr = r.AuxHeaterDemand / fuel.LowerHeatingValueVecto;
                }

                kilogramCO2 += f.FcFinal * fuel.CO2PerFuelWeight;

                r.FuelCorrection[fuel.FuelType] = f;
            }

            r.CO2Total = kilogramCO2;
            r.KilogramCO2PerMeter = distance == null || distance.IsEqual(0) ? null : kilogramCO2 / distance;
            return r;
        }

        protected virtual void SetReesCorrectionDemand(IModalDataContainer modData, VectoRunData runData,
            CorrectedModalData r)
        {
            var em = runData.ElectricMachinesData?.FirstOrDefault(x => x.Item1 != PowertrainPosition.GEN);

            if (em != null && runData.OVCMode != OvcHevMode.ChargeDepleting) {
                var deltaEReess = modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int) - r.WorkBusAux_elPS_SoC_Corr;
                var startSoc = modData.REESSStartSoC();
                var endSoc = modData.REESSEndSoC();
                var emEff = 0.0;
                if (endSoc < startSoc) {
                    var etaEmChg = modData.ElectricMotorEfficiencyGenerate(em.Item1);
                    var etaReessChg = modData.WorkREESSChargeInternal().Value() / modData.WorkREESSChargeTerminal_ES().Value();
                    emEff = 1.0 / (etaEmChg * etaReessChg);
                }
                if (endSoc > startSoc) {
                    var etaEmDischg = modData.ElectricMotorEfficiencyDrive(em.Item1);
                    var etaReessDischg = modData.WorkREESSDischargeTerminal_ES().Value() / modData.WorkREESSDischargeInternal().Value();
                    emEff = etaEmDischg * etaReessDischg;
                }

                r.DeltaEReessMech = double.IsNaN(emEff) ? 0.SI<WattSecond>() : -deltaEReess * emEff;
            } else {
                r.DeltaEReessMech = 0.SI<WattSecond>();
            }
        }

        protected virtual FuelConsumptionCorrection SetFuelConsumptionCorrection(IModalDataContainer modData, VectoRunData runData,
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
                FcREESSSoc = r.DeltaEReessMech * engLine,
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



        protected virtual void SetMissingDCDCEnergy(IModalDataContainer modData, VectoRunData runData, CorrectedModalData r)
        {
            // either case C1, C2a, or C3a
            var missingDCDCEnergy = modData.TimeIntegral<WattSecond>(ModalResultField.P_DCDC_missing);
            if (missingDCDCEnergy.IsEqual(0)) {
                r.EnergyDCDCMissing = missingDCDCEnergy;
                return;
            }
            //if (runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart) {
            // case C3a
            if (runData.ElectricMachinesData.Count != 1) {
                throw new VectoException("exactly 1 electric machine is required. got {0} ({1})",
                    runData.ElectricMachinesData.Count,
                    runData.ElectricMachinesData.Select(x => x.Item1.ToString()).Join());
            }

            var emPos = runData.ElectricMachinesData.First().Item1;
            var averageEmEfficiencyCharging = modData.ElectricMotorEfficiencyGenerate(emPos);
            r.EnergyDCDCMissing = missingDCDCEnergy /
                                runData.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency /
                                averageEmEfficiencyCharging;
            //} else {
            //	// case C1, C2a
            //	r.EnergyDCDCMissing = missingDCDCEnergy /
            //						runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(
            //							0.RPMtoRad(), 0.SI<Ampere>()); //  DeclarationData.AlternaterEfficiency;
            //}
        }

        protected virtual void SetBusAuxMissingPSWork(IModalDataContainer modData, VectoRunData runData, CorrectedModalData r)
        {
            var actuations = new Actuations() {
                Braking = runData.BusAuxiliaries.Actuations.Braking,
                ParkBrakeAndDoors = runData.BusAuxiliaries.Actuations.ParkBrakeAndDoors,
                Kneeling = runData.BusAuxiliaries.Actuations.Kneeling,
                CycleTime = modData.Duration
            };

            r.CorrectedAirDemand = M03Impl.TotalAirDemandCalculation(runData.BusAuxiliaries, actuations);
            r.DeltaAir = r.CorrectedAirDemand - modData.AirGenerated();

            if (runData.BusAuxiliaries.PneumaticUserInputsConfig.CompressorMap == null) {
                // electric compressor
                r.kAir = 0.SI(Unit.SI.Watt.Second.Per.Cubic.Meter);
                r.WorkBusAuxPSCorr = 0.SI<WattSecond>();
                return;
            }

            var nonSmartAirGen = modData.GetValues(x => new {
                dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()),
                airConsumed = x.Field<NormLiter>(ModalResultField.Nl_busAux_PS_consumer.GetName()),
                airGenerated = x.Field<NormLiter>(ModalResultField.Nl_busAux_PS_generated.GetName()),
                P_compOff = x.Field<Watt>(ModalResultField.P_busAux_PS_generated_dragOnly.GetName()),
                P_compOn = x.Field<Watt>(ModalResultField.P_busAux_PS_generated_alwaysOn.GetName()),
                Nl_alwaysOn = x.Field<NormLiter>(ModalResultField.Nl_busAux_PS_generated_alwaysOn.GetName())
            }).Where(x => x.airConsumed.IsEqual(x.airGenerated)).ToArray();

            var workBusAuxPSCompOff = nonSmartAirGen.Sum(x => x.P_compOff * x.dt);
            var workBusAuxPSCompOn = nonSmartAirGen.Sum(x => x.P_compOn * x.dt);
            var airBusAuxPSON = nonSmartAirGen.Sum(x => x.Nl_alwaysOn) ?? 0.SI<NormLiter>();

            r.kAir = airBusAuxPSON.IsEqual(0)
                ? 0.SI(Unit.SI.Watt.Second.Per.Cubic.Meter)
                : (workBusAuxPSCompOn - workBusAuxPSCompOff) / (airBusAuxPSON - 0.SI<NormLiter>());
            r.WorkBusAuxPSCorr = (r.kAir * r.DeltaAir).Cast<WattSecond>();
        }

		protected virtual void SetBusAuxMissingPSElWork(IModalDataContainer modData, VectoRunData runData,
			CorrectedModalData r)
		{
			if (runData.BusAuxiliaries.PneumaticUserInputsConfig.CompressorMap != null) {
                // no electric compressor
				return;
			}
            // SetBusAuxMissingPSWork is called before, so correctedAirDemand and DeltaAir is already set!

            var busAux = runData.BusAuxiliaries;
			if (runData.JobType == VectoSimulationJobType.ConventionalVehicle) {
                // case A, B
				ApplyElectricPS_FuelCorrection(modData, runData, r);
			} else {
                // parallel or serial hybrid
				if (busAux.ElectricalUserInputsConfig.ConnectESToREESS) {
                    // case C1, C2a, C3a
					ApplyElectricPS_Electric(modData, runData, r);
                } else {
                    // case C2b, C3b
					ApplyElectricPS_FuelCorrection(modData, runData, r);

				}
			}
		}

		private void ApplyElectricPS_Electric(IModalDataContainer modData, VectoRunData runData, CorrectedModalData r)
        {
			// case C1, C2a
			if (runData.OVCMode == OvcHevMode.ChargeDepleting) {
                r.WorkBusAux_elPS_SoC_ElRange = r.DeltaAir *
                            DeclarationData.BusAuxiliaries.PneumaticSystemElectricDemandPerAirGenerated /
                            runData.DCDCData.DCDCEfficiency;
            } else {
                // either Charge Sustaining, or non-OVC
                r.WorkBusAux_elPS_SoC_Corr = r.DeltaAir *
											DeclarationData.BusAuxiliaries.PneumaticSystemElectricDemandPerAirGenerated /
											runData.DCDCData.DCDCEfficiency;

            }
		}

		private void ApplyElectricPS_FuelCorrection(IModalDataContainer modData, VectoRunData runData, CorrectedModalData r)
		{
            // case A, B
			// case C2b, C3a, C3b
			r.WorkBusAux_elPS_Corr_mech =
				r.DeltaAir * DeclarationData.BusAuxiliaries.PneumaticSystemElectricDemandPerAirGenerated / 
				runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>()) /
				runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorGearEfficiency;
			
		}

		protected virtual void SetMissingEnergyICEOFf(IModalDataContainer modData, CorrectedModalData r)
        {
            var entriesAuxICEStandstill = modData.GetValues(x => new {
                dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()),
                P_off = x.Field<Watt>(ModalResultField.P_aux_ESS_mech_ice_off.GetName()) ?? 0.SI<Watt>(),
                P_on = x.Field<Watt>(ModalResultField.P_aux_ESS_mech_ice_on.GetName()) ?? 0.SI<Watt>(),
                v = x.Field<MeterPerSecond>(ModalResultField.v_act.GetName()),
                IceOn = x.Field<bool>(ModalResultField.ICEOn.GetName())
            }).Where(x => x.v != null && x.v.IsEqual(0) && !x.IceOn).ToArray();

            var entriesAuxICEDriving = modData.GetValues(x => new {
                dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()),
                P_off = x.Field<Watt>(ModalResultField.P_aux_ESS_mech_ice_off.GetName()) ?? 0.SI<Watt>(),
                P_on = x.Field<Watt>(ModalResultField.P_aux_ESS_mech_ice_on.GetName()) ?? 0.SI<Watt>(),
                v = x.Field<MeterPerSecond>(ModalResultField.v_act.GetName()),
                IceOn = x.Field<bool>(ModalResultField.ICEOn.GetName())
            }).Where(x => x.v != null && !x.v.IsEqual(0) && !x.IceOn).ToArray();

            r.ICEOffTimeStandstill = entriesAuxICEStandstill.Sum(x => x.dt) ?? 0.SI<Second>();

            r.EnergyAuxICEOffStandstill = entriesAuxICEStandstill.Sum(x => x.P_off * x.dt) ?? 0.SI<WattSecond>();
            r.EnergyAuxICEOnStandstill = entriesAuxICEStandstill.Sum(x => x.P_on * x.dt) ?? 0.SI<WattSecond>();

            r.ICEOffTimeDriving = entriesAuxICEDriving.Sum(x => x.dt) ?? 0.SI<Second>();

            r.EnergyAuxICEOffDriving = entriesAuxICEDriving.Sum(x => x.P_off * x.dt) ?? 0.SI<WattSecond>();
            r.EnergyAuxICEOnDriving = entriesAuxICEDriving.Sum(x => x.P_on * x.dt) ?? 0.SI<WattSecond>();
        }

        protected virtual void SetWHRWork(IModalDataContainer modData, VectoRunData runData, CorrectedModalData r)
        {
            // no post-processing correction for electric WHR for HEV (WHR is connected to REESS in simulation)
            r.WorkWHREl = (runData.BatteryData != null || runData.SuperCapData != null) && runData.EngineData.WHRType.IsElectrical()
                    ? 0.SI<WattSecond>()
                    : modData.TimeIntegral<WattSecond>(ModalResultField.P_WHR_el_corr);
            var altEff = DeclarationData.AlternatorEfficiency;
            // in case of bus-auxiliaries update alternator efficiency depending on busaux configuration
            if (runData.BusAuxiliaries != null) {
                if (runData.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS &&
                    runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart) {
                    // case C3a
                    if (runData.ElectricMachinesData.Count != 1) {
                        throw new VectoException("exactly 1 electric machine is required. got {0} ({1})",
                            runData.ElectricMachinesData.Count,
                            runData.ElectricMachinesData.Select(x => x.Item1.ToString()).Join());
                    }

                    var emPos = runData.ElectricMachinesData.First().Item1;
                    altEff = modData.ElectricMotorEfficiencyGenerate(emPos) * runData.BusAuxiliaries.ElectricalUserInputsConfig.DCDCEfficiency;

                } else {
                    altEff = runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(
                        0.RPMtoRad(), 0.SI<Ampere>());
                }
            }

            r.WorkWHRElMech = -r.WorkWHREl / altEff;

            r.WorkWHRMech = -modData.TimeIntegral<WattSecond>(ModalResultField.P_WHR_mech_corr);
        }

        #endregion
    }


	

  
}