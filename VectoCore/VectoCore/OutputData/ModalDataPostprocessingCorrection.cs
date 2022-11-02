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

namespace TUGraz.VectoCore.OutputData
{
	public class SerialHybridModalDataPostprocessingCorrection : ModalDataPostprocessingCorrection
	{

        protected override void SetReesCorrectionDemand(IModalDataContainer modData, VectoRunData runData,
            CorrectedModalData r)
        {
            var deltaEReess = modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int.GetName());
            var startSoc = modData.REESSStartSoC();
            var endSoc = modData.REESSEndSoC();
            var emEff = 0.0;
            if (endSoc < startSoc) {
                var etaReessChg = modData.WorkREESSChargeInternal().Value() / modData.WorkREESSChargeTerminal().Value();
                emEff = 1.0 / (etaReessChg);
            }
            if (endSoc > startSoc) {
                var etaReessDischg = modData.WorkREESSDischargeTerminal().Value() / modData.WorkREESSDischargeInternal().Value();
                emEff = etaReessDischg;
            }

            r.DeltaEReessMech = double.IsNaN(emEff) ? 0.SI<WattSecond>() : -deltaEReess * emEff;
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
                runData.BusAuxiliaries?.PneumaticUserInputsConfig.CompressorMap
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
                FcWHR = engLine * r.WorkWHR,
                FcAuxHtr = 0.SI<Kilogram>()
            };

            return f;
        }
    }

	public class ModalDataPostprocessingCorrection : IModalDataPostProcessor
	{
		
		#region Implementation of IModalDataPostProcessor

		public ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			var essParams = runData.DriverData.EngineStopStart;
			var r = new CorrectedModalData {
				UtilityFactorDriving = essParams.UtilityFactorDriving,
				UtilityFactorStandstill = essParams.UtilityFactorStandstill
			};
			var duration = modData.Duration;
			var distance = modData.Distance;
			
			SetMissingEnergyICEOFf(modData, r);

			SetWHRWork(modData, runData, r);

			if (runData.BusAuxiliaries != null) {
				SetBusAuxMissingPSWork(modData, runData, r);

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

			SetAuxHeaterDemand(modData, r, duration);

			SetReesCorrectionDemand(modData, runData, r);

			var kilogramCO2PerMeter = 0.SI<KilogramPerMeter>();

			var firstFuel = true;
			foreach (var fuel in modData.FuelData) {
				
				var f = SetFuelConsumptionCorrection(modData, runData, r, fuel);
				if (firstFuel) {
					firstFuel = false;
					f.FcAuxHtr = r.AuxHeaterDemand / fuel.LowerHeatingValueVecto;
				}
				
				kilogramCO2PerMeter += distance == null || distance.IsEqual(0)
					? 0.SI<KilogramPerMeter>()
					: f.FcFinal * fuel.CO2PerFuelWeight / distance;
				
				r.FuelCorrection[fuel.FuelType] = f;
			}

			r.KilogramCO2PerMeter = kilogramCO2PerMeter;
			return r;
		}

		protected virtual void SetReesCorrectionDemand(IModalDataContainer modData, VectoRunData runData,
			CorrectedModalData r)
		{
			var em = runData.ElectricMachinesData?.FirstOrDefault(x => x.Item1 != PowertrainPosition.GEN);
			
			if (em != null) {
				var deltaEReess = modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int.GetName());
				var startSoc = modData.REESSStartSoC();
				var endSoc = modData.REESSEndSoC();
				var emEff = 0.0;
				if (endSoc < startSoc) {
					var etaEmChg = modData.ElectricMotorEfficiencyGenerate(em.Item1);
					var etaReessChg = modData.WorkREESSChargeInternal().Value() / modData.WorkREESSChargeTerminal().Value();
					emEff = 1.0 / (etaEmChg * etaReessChg);
				} 
				if (endSoc > startSoc) {
					var etaEmDischg = modData.ElectricMotorEfficiencyDrive(em.Item1);
					var etaReessDischg = modData.WorkREESSDischargeTerminal().Value() / modData.WorkREESSDischargeInternal().Value();
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
				runData.BusAuxiliaries?.PneumaticUserInputsConfig.CompressorMap
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
				FcWHR = engLine * r.WorkWHR,
				FcAuxHtr = 0.SI<Kilogram>()
			};

			return f;
		}

		protected virtual void SetAuxHeaterDemand(IModalDataContainer modData, CorrectedModalData r, Second duration)
		{
			var engineWasteheatSum = modData.FuelData.Aggregate(
				0.SI<Joule>(),
				(current, fuel) => current + modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel) *
					fuel.LowerHeatingValueVecto);
			r.AuxHeaterDemand = modData.AuxHeaterDemandCalc == null
				? 0.SI<Joule>()
				: modData.AuxHeaterDemandCalc(duration, engineWasteheatSum);
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
			var airBusAuxPSON = nonSmartAirGen.Sum(x => x.Nl_alwaysOn);

			r.kAir = airBusAuxPSON.IsEqual(0)
				? 0.SI(Unit.SI.Watt.Second.Per.Cubic.Meter)
				: (workBusAuxPSCompOn - workBusAuxPSCompOff) / (airBusAuxPSON - 0.SI<NormLiter>());
			r.WorkBusAuxPSCorr = (r.kAir * r.DeltaAir).Cast<WattSecond>();
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


	public class BatteryElectricPostprocessingCorrection : IModalDataPostProcessor
	{
		#region Implementation of IModalDataPostProcessor

		public ICorrectedModalData ApplyCorrection(IModalDataContainer modData, VectoRunData runData)
		{
			return new NoCorrectionModalData(modData);
		}

		#endregion
	}

	public class CorrectedModalData : ICorrectedModalData
	{
		public SI kAir { get; set; }
		public Dictionary<FuelType, IFuelConsumptionCorrection> FuelCorrection { get; }
		#region Implementation of ICorrectedModalData

		public CorrectedModalData()
		{
			FuelCorrection = new Dictionary<FuelType, IFuelConsumptionCorrection>();
		}

		public WattSecond WorkESSMissing {
			get {
				return EnergyAuxICEOffStandstill_UF + EnergyAuxICEOffDriving_UF + EnergyAuxICEOnDriving_UF +
						EnergyAuxICEOnStandstill_UF;
			}
		}
		//public WattSecond WorkESS { get; set; }
		public WattSecond WorkWHREl { get; set; }
		public WattSecond WorkWHRElMech { get; set; }
		public WattSecond WorkWHRMech { get; set; }
		public WattSecond WorkWHR => WorkWHRElMech + WorkWHRMech;
		public WattSecond WorkBusAuxPSCorr { get; set; }
		public WattSecond WorkBusAuxESMech { get; set; }
		public WattSecond WorkBusAuxCorr => WorkBusAuxPSCorr + WorkBusAuxESMech;

		public Joule AuxHeaterDemand { get; set; }
		public KilogramPerMeter KilogramCO2PerMeter { get; set; }

		public IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
		{
			if (!FuelCorrection.ContainsKey(fuel.FuelType)) {
				throw new VectoException("Invalid fuel {0}", fuel);
			}

			return FuelCorrection[fuel.FuelType];
		}

		public Kilogram CO2Total
		{
			get
			{
				return FuelCorrection.Sum(x => x.Value.TotalFuelConsumptionCorrected * x.Value.Fuel.CO2PerFuelWeight);
			}
		}

		public Joule EnergyConsumptionTotal
		{
			get
			{
				return FuelCorrection.Sum(x =>
					x.Value.TotalFuelConsumptionCorrected * x.Value.Fuel.LowerHeatingValueVecto);
			}
		}

		public Second ICEOffTimeStandstill { get; set; }
		public WattSecond EnergyAuxICEOffStandstill { get; set; }
		public WattSecond EnergyAuxICEOffStandstill_UF {
			get {
				return EnergyAuxICEOffStandstill * UtilityFactorStandstill;
			}
		}

		public double UtilityFactorStandstill { get; set; }

		public WattSecond EnergyAuxICEOnStandstill { get; set; }
		public WattSecond EnergyAuxICEOnStandstill_UF {
			get { return EnergyAuxICEOnStandstill * (1 - UtilityFactorStandstill); }
		}
		public Watt AvgAuxPowerICEOnStandstill => ICEOffTimeStandstill.IsEqual(0) ? 0.SI<Watt>() : EnergyAuxICEOnStandstill / ICEOffTimeStandstill;


		public Second ICEOffTimeDriving { get; set; }
		public WattSecond EnergyAuxICEOffDriving { get; set; }
		public WattSecond EnergyAuxICEOffDriving_UF { get { return EnergyAuxICEOffDriving * UtilityFactorDriving; } }
		public double UtilityFactorDriving { get; set; }

		public WattSecond EnergyAuxICEOnDriving { get; set; }
		public WattSecond EnergyAuxICEOnDriving_UF {
			get {
				return EnergyAuxICEOnDriving * (1 - UtilityFactorDriving);
			}
		}
		public Watt AvgAuxPowerICEOnDriving => ICEOffTimeDriving.IsEqual(0) ? 0.SI<Watt>() : EnergyAuxICEOnDriving / ICEOffTimeDriving;

		public WattSecond EnergyDCDCMissing { get; set; }
		public NormLiter CorrectedAirDemand { get; set; }
		public NormLiter DeltaAir { get; set; }
		public WattSecond DeltaEReessMech { get; set; }

		#endregion
	}

	public class FuelConsumptionCorrection : IFuelConsumptionCorrection
	{

		public IFuelProperties Fuel { get; set; }
		public Meter Distance { get; set; }
		public Second Duration { get; set; }

		public Kilogram FcModSum { get; set; }

		public Kilogram FcESS_EngineStart { get; set; }
		public Kilogram FcESS_AuxStandstill_ICEOff { get; set; }
		public Kilogram FcESS_AuxStandstill_ICEOn { get; set; }
		public Kilogram FcESS_AuxDriving_ICEOn { get; set; }
		public Kilogram FcESS_AuxDriving_ICEOff { get; set; }

		public Kilogram FcESS_DCDCMissing { get; set; }

		public Kilogram FcESS =>
			FcESS_EngineStart + FcESS_AuxStandstill_ICEOff + FcESS_AuxStandstill_ICEOn
			+ FcESS_AuxDriving_ICEOn + FcESS_AuxDriving_ICEOff + FcESS_DCDCMissing;

		public Kilogram FcBusAuxPSAirDemand { get; set; }

		public Kilogram FcBusAuxPSDragICEOffStandstill { get; set; }
		public Kilogram FcBusAuxPSDragICEOffDriving { get; set; }
		public Kilogram FcBusAuxPs
		{
			get => FcBusAuxPSAirDemand + FcBusAuxPSDragICEOffDriving + FcBusAuxPSDragICEOffStandstill;
		}
		public Kilogram FcBusAuxEs { get; set; }
		public Kilogram FcWHR { get; set; }
		public Kilogram FcREESSSoc { get; set; }

		public Kilogram FcAuxHtr { get; set; }


		public Kilogram FcEssCorr => FcModSum + FcESS;
		public Kilogram FcBusAuxPsCorr => FcEssCorr + FcBusAuxPs;
		public Kilogram FcBusAuxEsCorr => FcBusAuxPsCorr + FcBusAuxEs;
		public Kilogram FcWHRCorr => FcBusAuxEsCorr + FcWHR;
		public Kilogram FcREESSSoCCorr => FcWHRCorr + FcREESSSoc;

		public Kilogram FcAuxHtrCorr => FcREESSSoCCorr + FcAuxHtr;

		public Kilogram FcFinal => FcAuxHtrCorr;

		#region Implementation of IFuelConsumptionCorrection

		public KilogramPerWattSecond EngineLineCorrectionFactor { get; set; }
		public KilogramPerWattSecond VehicleLine { get; set; }
		public KilogramPerSecond FC_ESS_H => Duration != null ? (FcESS / Duration) : null;
		public KilogramPerSecond FC_ESS_CORR_H => Duration != null ? (FcEssCorr / Duration) : null;
		public KilogramPerSecond FC_BusAux_PS_CORR_H => Duration != null ? (FcBusAuxPsCorr / Duration) : null;
		public KilogramPerSecond FC_BusAux_ES_CORR_H => Duration != null ? (FcBusAuxEsCorr / Duration) : null;
		public KilogramPerSecond FC_WHR_CORR_H => Duration != null ? (FcWHRCorr / Duration) : null;
		public KilogramPerSecond FC_AUXHTR_H => Duration != null ? (FcAuxHtr / Duration) : null;
		public KilogramPerSecond FC_AUXHTR_H_CORR => Duration != null ? (FcAuxHtrCorr / Duration) : null;
		public KilogramPerSecond FC_REESS_SOC_H => Duration != null ? FcREESSSoc / Duration : null;
		public KilogramPerSecond FC_REESS_SOC_CORR_H => Duration != null ? (FcREESSSoCCorr / Duration) : null;
		public KilogramPerSecond FC_FINAL_H => Duration != null ? FcFinal / Duration : null;

		public KilogramPerMeter FC_REESS_SOC_KM => Distance != null ? FcREESSSoc / Distance : null;
		public KilogramPerMeter FC_REESS_SOC_CORR_KM => Distance != null ? (FcREESSSoCCorr / Distance) : null;
		public KilogramPerMeter FC_ESS_KM => Distance != null ? (FcESS / Distance) : null;
		public KilogramPerMeter FC_ESS_CORR_KM => Distance != null ? (FcEssCorr / Distance) : null;
		public KilogramPerMeter FC_WHR_CORR_KM => Distance != null ? (FcWHRCorr / Distance) : null;
		public KilogramPerMeter FC_BusAux_PS_CORR_KM => Distance != null ? (FcBusAuxPsCorr / Distance) : null;
		public KilogramPerMeter FC_BusAux_ES_CORR_KM => Distance != null ? (FcBusAuxEsCorr / Distance) : null;
		public KilogramPerMeter FC_AUXHTR_KM => Distance != null ? (FcAuxHtr / Distance) : null;
		public KilogramPerMeter FC_AUXHTR_KM_CORR => Distance != null ? (FcAuxHtrCorr / Distance) : null;
		public KilogramPerMeter FC_FINAL_KM => Distance != null ? FcFinal / Distance : null;

		public VolumePerMeter FuelVolumePerMeter =>
			Fuel.FuelDensity != null && Distance != null
				? (FcFinal / Distance / Fuel.FuelDensity).Cast<VolumePerMeter>()
				: null;

		public Kilogram TotalFuelConsumptionCorrected => FcFinal;

		public Joule EnergyDemand => FcFinal * Fuel.LowerHeatingValueVecto;

		#endregion
	}


	public class NoCorrectionModalData : ICorrectedModalData
	{
		private IModalDataContainer _modData;

		public NoCorrectionModalData(IModalDataContainer modData)
		{
			_modData = modData;
		}

		#region Implementation of ICorrectedModalData

		public WattSecond WorkESSMissing => 0.SI<WattSecond>();
		public WattSecond WorkWHREl => 0.SI<WattSecond>();
		public WattSecond WorkWHRElMech => 0.SI<WattSecond>();
		public WattSecond WorkWHRMech => 0.SI<WattSecond>();
		public WattSecond WorkWHR => 0.SI<WattSecond>();
		public WattSecond WorkBusAuxPSCorr => 0.SI<WattSecond>();
		public WattSecond WorkBusAuxESMech => 0.SI<WattSecond>();
		public WattSecond WorkBusAuxCorr => 0.SI<WattSecond>();
		public WattSecond EnergyDCDCMissing => 0.SI<WattSecond>();
		public Joule AuxHeaterDemand => 0.SI<WattSecond>();
		public IFuelConsumptionCorrection FuelConsumptionCorrection(IFuelProperties fuel)
		{
			return new NoFuelConsumptionCorrection();
		}

		public KilogramPerMeter KilogramCO2PerMeter => 0.SI<KilogramPerMeter>();
		public Dictionary<FuelType, IFuelConsumptionCorrection> FuelCorrection => new Dictionary<FuelType, IFuelConsumptionCorrection>();
		public Kilogram CO2Total => 0.SI<Kilogram>();
		public Joule EnergyConsumptionTotal => 0.SI<Joule>();

		#endregion
	}

	public class NoFuelConsumptionCorrection : IFuelConsumptionCorrection
	{
		#region Implementation of IFuelConsumptionCorrection

		public IFuelProperties Fuel { get; }
		public KilogramPerWattSecond EngineLineCorrectionFactor { get; }
		public KilogramPerWattSecond VehicleLine { get; }
		public KilogramPerSecond FC_ESS_H { get; }
		public KilogramPerSecond FC_ESS_CORR_H { get; }
		public KilogramPerSecond FC_BusAux_PS_CORR_H { get; }
		public KilogramPerSecond FC_BusAux_ES_CORR_H { get; }
		public KilogramPerSecond FC_WHR_CORR_H { get; }
		public KilogramPerSecond FC_AUXHTR_H { get; }
		public KilogramPerSecond FC_AUXHTR_H_CORR { get; }
		public KilogramPerSecond FC_REESS_SOC_H { get; }
		public KilogramPerSecond FC_REESS_SOC_CORR_H { get; }
		public KilogramPerSecond FC_FINAL_H { get; }
		public KilogramPerMeter FC_WHR_CORR_KM { get; }
		public KilogramPerMeter FC_BusAux_PS_CORR_KM { get; }
		public KilogramPerMeter FC_BusAux_ES_CORR_KM { get; }
		public KilogramPerMeter FC_AUXHTR_KM { get; }
		public KilogramPerMeter FC_AUXHTR_KM_CORR { get; }
		public KilogramPerMeter FC_REESS_SOC_KM { get; }
		public KilogramPerMeter FC_REESS_SOC_CORR_KM { get; }
		public KilogramPerMeter FC_ESS_KM { get; }
		public KilogramPerMeter FC_ESS_CORR_KM { get; }
		public KilogramPerMeter FC_FINAL_KM { get; }
		public VolumePerMeter FuelVolumePerMeter { get; }
		public Kilogram TotalFuelConsumptionCorrected { get; }
		public Joule EnergyDemand { get; }

		#endregion
	}
}