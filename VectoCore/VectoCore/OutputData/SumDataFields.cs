using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using static TUGraz.VectoCore.OutputData.SumDataFields;

namespace TUGraz.VectoCore.OutputData
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "IdentifierTypo")]
	public static class SumDataFields
	{
		public const string INTERNAL_PREFIX = "INTERNAL";

		public const string SORT = INTERNAL_PREFIX + " Sorting";
		public const string JOB = "Job [-]";
		public const string INPUTFILE = "Input File [-]";
		public const string CYCLE = "Cycle [-]";
		public const string OVCHEVMode = "Ovc Mode [-]";
		public const string STATUS = "Status";
		public const string CURB_MASS = "Corrected Actual Curb Mass [kg]";
		public const string LOADING = "Loading [kg]";
		public const string PassengerCount = "Passenger count [-]";

		public const string VEHICLE_MANUFACTURER = "Vehicle manufacturer [-]";
		public const string VIN_NUMBER = "VIN number";
		public const string VEHICLE_MODEL = "Vehicle model [-]";

		public const string ENGINE_MANUFACTURER = "Engine manufacturer [-]";
		public const string ENGINE_MODEL = "Engine model [-]";
		public const string ENGINE_FUEL_TYPE = "Engine fuel type [-]";
		public const string ENGINE_WHTC_URBAN = "Engine WHTCUrban";
		public const string ENGINE_WHTC_RURAL = "Engine WHTCRural";
		public const string ENGINE_WHTC_MOTORWAY = "Engine WHTCMotorway";
		public const string ENGINE_BF_COLD_HOT = "Engine BFColdHot";
		public const string ENGINE_CF_REG_PER = "Engine CFRegPer";
		public const string ENGINE_ACTUAL_CORRECTION_FACTOR = "Engine actual CF";
		public const string ENGINE_RATED_POWER = "Engine rated power [kW]";
		public const string ENGINE_IDLING_SPEED = "Engine idling speed [rpm]";
		public const string ENGINE_RATED_SPEED = "Engine rated speed [rpm]";
		public const string ENGINE_DISPLACEMENT = "Engine displacement [ccm]";

		public const string ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER = "total RRC [-]";
		public const string ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER = "weighted RRC w/o trailer [-]";

		public const string GEARBOX_MANUFACTURER = "Gearbox manufacturer [-]";
		public const string GEARBOX_MODEL = "Gearbox model [-]";
		public const string GEARBOX_TYPE = "Gearbox type [-]";

		public const string TORQUECONVERTER_MANUFACTURER = "Torque converter manufacturer [-]";
		public const string TORQUECONVERTER_MODEL = "Torque converter model [-]";

		public const string RETARDER_MANUFACTURER = "Retarder manufacturer [-]";
		public const string RETARDER_MODEL = "Retarder model [-]";
		public const string RETARDER_TYPE = "Retarder type [-]";

		public const string ANGLEDRIVE_MANUFACTURER = "Angledrive manufacturer [-]";
		public const string ANGLEDRIVE_MODEL = "Angledrive model [-]";
		public const string ANGLEDRIVE_RATIO = "Angledrive ratio [-]";

		public const string AXLE_MANUFACTURER = "Axle manufacturer [-]";
		public const string AXLE_MODEL = "Axle model [-]";
		public const string AXLE_RATIO = "Axle gear ratio [-]";

		public const string AUX_TECH_FORMAT = "Auxiliary technology {0} [-]";

		public const string HDV_CO2_VEHICLE_CLASS = "HDV CO2 vehicle class [-]";
		public const string TOTAL_VEHICLE_MASS = "Total vehicle mass [kg]";
		public const string CD_x_A_DECLARED = "Declared CdxA [m²]";

		public const string CD_x_A = "CdxA [m²]";

		public const string R_DYN = "r_dyn [m]";

		public const string CARGO_VOLUME = "Cargo Volume [m³]";
		public const string TIME = "time [s]";
		public const string DISTANCE = "distance [km]";
		public const string SPEED = "speed [km/h]";
		public const string ALTITUDE_DELTA = "altitudeDelta [m]";

		public const string FCMAP_H = "FC-Map{0} [g/h]";
		public const string FCMAP_KM = "FC-Map{0} [g/km]";
		public const string FCNCVC_H = "FC-NCVc{0} [g/h]";
		public const string FCNCVC_KM = "FC-NCVc{0} [g/km]";
		public const string FCWHTCC_H = "FC-WHTCc{0} [g/h]";
		public const string FCWHTCC_KM = "FC-WHTCc{0} [g/km]";

		public const string FCESS_H = "FC-ESS{0} [g/h]";
		public const string FCESS_KM = "FC-ESS{0} [g/km]";
		public const string FCESS_H_CORR = "FC-ESS_Corr{0} [g/h]";
		public const string FCESS_KM_CORR = "FC-ESS_Corr{0} [g/km]";
		public const string FCWHR_H_CORR = "FC-WHR_Corr{0} [g/h]";
		public const string FCWHR_KM_CORR = "FC-WHR_Corr{0} [g/km]";
		public const string FC_HEV_SOC_H = "FC-SoC{0} [g/h]";
		public const string FC_HEV_SOC_KM = "FC-SoC{0} [g/km]";
		public const string FC_HEV_SOC_CORR_H = "FC-SoC_Corr{0} [g/h]";
		public const string FC_HEV_SOC_CORR_KM = "FC-SoC_Corr{0} [g/km]";


		public const string FC_BusAux_PS_CORR_H = "FC-BusAux_PS_Corr{0} [g/h]";
		public const string FC_BusAux_PS_CORR_KM = "FC-BusAux_PS_Corr{0} [g/km]";
		public const string FC_BusAux_ES_CORR_H = "FC-BusAux_ES_Corr{0} [g/h]";
		public const string FC_BusAux_ES_CORR_KM = "FC-BusAux_ES_Corr{0} [g/km]";
		public const string FC_AUXHTR_H = "FC-BusAux_AuxHeater{0} [g/h]";
		public const string FC_AUXHTR_KM = "FC-BusAux_AuxHeater{0} [g/km]";
		public const string FC_AUXHTR_H_CORR = "FC-BusAux_AuxHeater_Corr{0} [g/h]";
		public const string FC_AUXHTR_KM_CORR = "FC-BusAux_AuxHeater_Corr{0} [g/km]";


		public const string FCFINAL_H = "FC-Final{0} [g/h]";
		public const string FCFINAL_KM = "FC-Final{0} [g/km]";
		public const string FCFINAL_LITERPER100KM = "FC-Final{0} [l/100km]";
		public const string FCFINAL_LITERPER100TKM = "FC-Final{0} [l/100tkm]";
		public const string FCFINAL_LiterPer100M3KM = "FC-Final{0} [l/100m³km]";
		public const string FCFINAL_LiterPer100PassengerKM = "FC-Final{0} [l/100Pkm]";

		public const string EC_el_SOC = "EC_el_SoC [kWh]";
		public const string EC_el_SOC_corr = "EC_el_SoC_corr [kWh]";

		public const string EC_el_final = "EC_el_final [kWh]";
		public const string EC_el_final_KM = "EC_el_final [kWh/km]";
		public const string EC_el_final_TKM = "EC_el_final [kWh/tkm]";
		public const string EC_el_final_M3KM = "EC_el_final  [g/m³km]";
		public const string ElectricEnergyConsumption_PKM = "EC_el_final [g/Pkm]";

		public const string CO2_KM = "CO2 [g/km]";
		public const string CO2_TKM = "CO2 [g/tkm]";
		public const string CO2_M3KM = "CO2 [g/m³km]";
		public const string CO2_PKM = "CO2 [g/Pkm]";

		public const string P_WHEEL_POS = "P_wheel_in_pos [kW]";
		public const string P_WHEEL = "P_wheel_in [kW]";
		public const string P_FCMAP_POS = "P_fcmap_pos [kW]";
		public const string P_FCMAP = "P_fcmap [kW]";

		public const string E_FORMAT = "E_{0} [kWh]";
		public const string E_AUX_FORMAT = "E_aux_{0} [kWh]";
		public const string E_AUX = "E_aux_sum [kWh]";
		public const string E_AUX_EL = "E_aux_sum_el [kWh]";

		public const string E_AUX_EL_HV = "E_aux_el(HV) [kWh]";

		public const string E_ICE_START = "E_ice_start [kWh]";
		public const string NUM_ICE_STARTS = "ice_starts [-]";
		public const string K_ENGLINE = "k_engline{0} [g/kWh]";
		public const string K_VEHLINE = "k_vehline{0} [g/kWh]";

		public const string E_WHR_EL = "E_WHR_el [kWh]";
		public const string E_WHR_MECH = "E_WHR_mech [kWh]";

		public const string E_BusAux_PS_corr = "E_BusAux_PS_corr [kWh]";
		public const string E_BusAux_el_PS_corr = "E_BusAux_el_PS_corr [kWh]";
		public const string E_BusAux_ES_mech_corr = "E_BusAux_ES_mech_corr [kWh]";
		public const string E_BusAux_AuxHeater = "E_BusAux_AuxHeater [kWh]";

		public const string E_AIR = "E_air [kWh]";
		public const string E_ROLL = "E_roll [kWh]";
		public const string E_GRAD = "E_grad [kWh]";
		public const string E_VEHICLE_INERTIA = "E_vehi_inertia [kWh]";
		public const string E_POWERTRAIN_INERTIA = "E_powertrain_inertia [kWh]";
		public const string E_WHEEL = "E_wheel [kWh]";
		public const string E_BRAKE = "E_brake [kWh]";
		public const string E_GBX_LOSS = "E_gbx_loss [kWh]";
		public const string E_SHIFT_LOSS = "E_shift_loss [kWh]";
		public const string E_AXL_LOSS = "E_axl_loss [kWh]";
		public const string E_RET_LOSS = "E_ret_loss [kWh]";
		public const string E_TC_LOSS = "E_tc_loss [kWh]";
		public const string E_ANGLE_LOSS = "E_angle_loss [kWh]";
		public const string E_CLUTCH_LOSS = "E_clutch_loss [kWh]";
		public const string E_FCMAP_POS = "E_fcmap_pos [kWh]";
		public const string E_FCMAP_NEG = "E_fcmap_neg [kWh]";

		public const string AirGenerated = "BusAux PS air generated [Nl]";
		public const string AirConsumed = "BusAux PS air consumed [Nl]";
		public const string E_PS_CompressorOff = "E_PS_compressorOff [kWh]";
		public const string E_PS_CompressorOn = "E_PS_compressorOn [kWh]";

		public const string E_BusAux_ES_generated = "E_BusAux_ES_generated [kWh]";
		public const string E_BusAux_ES_consumed = "E_BusAux_ES_consumed [kWh]";
		public const string Delta_E_BusAux_Battery = "ΔE_BusAux_Bat [kWh]";

		public const string E_BusAux_HVAC_Mech = "E_BusAux_HVAC_mech [kWh]";
		public const string E_BusAux_HVAC_El = "E_BusAux_HVAC_el [kWh]";

		public const string E_AUX_ESS_missing = "E_aux_ESS_missing [kWh]";

		public const string SPECIFIC_FC = "Specific FC{0} [g/kWh] wheel pos.";

		public const string ACC = "a [m/s^2]";
		public const string ACC_POS = "a_pos [m/s^2]";
		public const string ACC_NEG = "a_neg [m/s^2]";

		public const string ACC_TIMESHARE = "AccelerationTimeShare [%]";
		public const string DEC_TIMESHARE = "DecelerationTimeShare [%]";
		public const string CRUISE_TIMESHARE = "CruiseTimeShare [%]";
		public const string STOP_TIMESHARE = "StopTimeShare [%]";

		public const string MAX_SPEED = "max. speed [km/h]";
		public const string MAX_ACCELERATION = "max. acc [m/s²]";
		public const string MAX_DECELERATION = "max. dec [m/s²]";
		public const string AVG_ENGINE_SPEED = "n_eng_avg [rpm]";
		public const string MAX_ENGINE_SPEED = "n_eng_max [rpm]";
		public const string NUM_GEARSHIFTS = "gear shifts [-]";
		public const string ICE_FULL_LOAD_TIME_SHARE = "ICE max. Load time share [%]";
		public const string ICE_OFF_TIME_SHARE = "ICE off time share [%]";
		public const string COASTING_TIME_SHARE = "CoastingTimeShare [%]";
		public const string BRAKING_TIME_SHARE = "BrakingTimeShare [%]";

		public const string TIME_SHARE_PER_GEAR_FORMAT = "Gear {0} TimeShare [%]";
		public const string RATIO_PER_GEAR_FORMAT = "Gear {0} Ratio [-]";
		public const string P2_5_RATIO_PER_GEAR_FORMAT = "Gear {0} P2.5 Ratio [-]";

		public const string NUM_AXLES_DRIVEN = "Number axles vehicle driven [-]";
		public const string NUM_AXLES_NON_DRIVEN = "Number axles vehicle non-driven [-]";
		public const string NUM_AXLES_TRAILER = "Number axles trailer [-]";

		public const string TCU_MODEL = "ShiftStrategy";

		public const string VEHICLE_FUEL_TYPE = "Vehicle fuel type [-]";
		public const string AIRDRAG_MODEL = "AirDrag model [-]";
		public const string SLEEPER_CAB = "Sleeper cab [-]";
		public const string DECLARED_RRC_AXLE1 = "Declared RRC axle 1 [-]";
		public const string DECLARED_FZISO_AXLE1 = "Declared FzISO axle 1 [N]";
		public const string DECLARED_RRC_AXLE2 = "Declared RRC axle 2 [-]";
		public const string DECLARED_FZISO_AXLE2 = "Declared FzISO axle 2 [N]";
		public const string DECLARED_RRC_AXLE3 = "Declared RRC axle 3 [-]";
		public const string DECLARED_FZISO_AXLE3 = "Declared FzISO axle 3 [N]";
		public const string DECLARED_RRC_AXLE4 = "Declared RRC axle 4 [-]";
		public const string DECLARED_FZISO_AXLE4 = "Declared FzISO axle 4 [N]";
		public const string ADAS_TECHNOLOGY_COMBINATION = "ADAS technology combination [-]";

		public const string PTO_TECHNOLOGY = "PTOShaftsGearWheels";

		//public const string PTO_OTHER_ELEMENTS = "PTOOtherElements";

		public const string ENGINE_CERTIFICATION_NUMBER = "Engine certification number";
		public const string AVERAGE_ENGINE_EFFICIENCY = "Average engine efficiency [-]";
		public const string TORQUE_CONVERTER_CERTIFICATION_NUMBER = "TorqueConverter certification number";
		public const string TORQUE_CONVERTER_CERTIFICATION_METHOD = "Torque converter certification option";

		public const string AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITH_LOCKUP =
			"Average torque converter efficiency with lockup [-]";

		public const string AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITHOUT_LOCKUP =
			"Average torque converter efficiency w/o lockup [-]";

		public const string GEARBOX_CERTIFICATION_NUMBER = "Gearbox certification number";
		public const string GEARBOX_CERTIFICATION_METHOD = "Gearbox certification option";
		public const string AVERAGE_GEARBOX_EFFICIENCY = "Average gearbox efficiency [-]";
		public const string RETARDER_CERTIFICATION_NUMBER = "Retarder certification number";
		public const string RETARDER_CERTIFICATION_METHOD = "Retarder certification option";
		public const string ANGLEDRIVE_CERTIFICATION_NUMBER = "Angledrive certification number";
		public const string ANGLEDRIVE_CERTIFICATION_METHOD = "Angledrive certification option";
		public const string AVERAGE_ANGLEDRIVE_EFFICIENCY = "Average angledrive efficiency [-]";
		public const string AXLEGEAR_CERTIFICATION_NUMBER = "Axlegear certification number";
		public const string AXLEGEAR_CERTIFICATION_METHOD = "Axlegear certification method";
		public const string AVERAGE_AXLEGEAR_EFFICIENCY = "Average axlegear efficiency [-]";
		public const string AIRDRAG_CERTIFICATION_NUMBER = "AirDrag certification number";
		public const string AIRDRAG_CERTIFICATION_METHOD = "AirDrag certification option";

		public const string AVERAGE_POS_ACC = "a_avg_acc";

		public const string E_EM_DRIVE_FORMAT = "E_EM_{0}_drive [kWh]";
		public const string E_EM_GENERATE_FORMAT = "E_EM_{0}_gen [kWh]";
		public const string ETA_EM_DRIVE_FORMAT = "η_EM_{0}_drive";
		public const string ETA_EM_GEN_FORMAT = "η_EM_{0}_gen";

		public const string E_EM_Mot_DRIVE_FORMAT = "E_EM_{0}-em_drive [kWh]";
		public const string E_EM_Mot_GENERATE_FORMAT = "E_EM_{0}-em_gen [kWh]";
		public const string ETA_EM_Mot_DRIVE_FORMAT = "η_EM_{0}-em_drive";
		public const string ETA_EM_Mot_GEN_FORMAT = "η_EM_{0}-em_gen";

		public const string EM_AVG_SPEED_FORMAT = "n_EM_{0}-em_avg [rpm]";

		public const string E_EM_OFF_Loss_Format = "E_EM_{0}_off_loss [kWh]";
		public const string E_EM_LOSS_TRANSM_FORMAT = "E_EM_{0}_transm_loss [kWh]";
		public const string E_EM_Mot_LOSS_FORMAT = "E_EM_{0}-em_loss [kWh]";
		public const string E_EM_LOSS_FORMAT = "E_EM_{0}_loss [kWh]";
		public const string E_EM_OFF_TIME_SHARE = "EM {0} off time share [%]";
		public const string EM_RATED_TORQUE_HI = "EM {0} high voltage rated T [Nm]";
		public const string EM_RATED_TORQUE_LO = "EM {0} low voltage rated T [Nm]";
		public const string EM_RATED_POWER = "EM {0} total rated power [kW]";
		public const string EM_RATED_SPEED_HI = "EM {0} high voltage rated speed [rpm]";
		public const string EM_RATED_SPEED_LO = "EM {0} low voltage rated speed [rpm]";
		public const string EM_MOTOR_NUMBER = "EM number of motors";

		public const string REESS_CAPACITY = "REESS Capacity";
		public const string REESS_StartSoC = "REESS Start SoC [%]";
		public const string REESS_EndSoC = "REESS End SoC [%]";
		public const string REESS_MinSoC = "REESS Min SoC [%]";
		public const string REESS_MaxSoC = "REESS Max SoC [%]";
		public const string REESS_DeltaEnergy = "ΔE_REESS [kWh]";

		public const string E_REESS_LOSS = "E_REESS_loss [kWh]";
		public const string E_REESS_T_chg = "E_REESS_T_chg [kWh]";
		public const string E_REESS_T_dischg = "E_REESS_T_dischg [kWh]";
		public const string E_REESS_int_chg = "E_REESS_int_chg [kWh]";
		public const string E_REESS_int_dischg = "E_REESS_int_dischg [kWh]";

		public const string IEPC_AVG_SPEED_FORMAT = "n_{0}-em_avg [rpm]";
		public const string E_IEPC_DRIVE_FORMAT = "E_{0}_drive [kWh]";
		public const string E_IEPC_GENERATE_FORMAT = "E_{0}_gen [kWh]";

		public const string ETA_IEPC_DRIVE_FORMAT = "η_{0}_drive";
		public const string ETA_IEPC_GEN_FORMAT = "η_{0}_gen";
		public const string E_IEPC_OFF_Loss_Format = "E_{0}_off_loss [kWh]";
		public const string E_IEPC_LOSS_FORMAT = "E_{0}_loss [kWh]";
		public const string E_IEPC_OFF_TIME_SHARE = "{0} off time share [%]";

		public const string E_WHEELEND_SAVED = "E_wheelEnd_saved [kWh]";

		public const string f_equiv = "f_equiv";

		#region FuelCells

		public static class FuelCellFields
		{
			public const string FCMAP_H = "FC-Map [g/h]";
			public const string FCMAP_KM = "FC-Map [g/km]";

			public const string FC_BusAux_PS_CORR_H = "FC-BusAux_PS_Corr [g/h]";
			public const string FC_BusAux_PS_CORR_KM = "FC-BusAux_PS_Corr [g/km]";
			public const string FC_BusAux_ES_CORR_H = "FC-BusAux_ES_Corr [g/h]";
			public const string FC_BusAux_ES_CORR_KM = "FC-BusAux_ES_Corr [g/km]";
			public const string FC_AUXHTR_H = "FC-BusAux_AuxHeater [g/h]";
			public const string FC_AUXHTR_KM = "FC-BusAux_AuxHeater [g/km]";
			public const string FC_AUXHTR_H_CORR = "FC-BusAux_AuxHeater_Corr [g/h]";
			public const string FC_AUXHTR_KM_CORR = "FC-BusAux_AuxHeater_Corr [g/km]";

			public const string FCFINAL_H = "FC-Final [g/h]";
			public const string FCFINAL_KM = "FC-Final [g/km]";

			public const string K_FCSLine = "k_CFCSline [g/kWh]";

			public const string P_FCS = "P_CFCS [kW]";
			public const string E_FCS = "E_CFCS [kWh]";

			public const string FC_HEV_SOC_H = "FC-SoC [g/h]";
			public const string FC_HEV_SOC_KM = "FC-SoC [g/km]";
			public const string FC_HEV_SOC_CORR_H = "FC-SoC_Corr [g/h]";
			public const string FC_HEV_SOC_CORR_KM = "FC-SoC_Corr [g/km]";
		}

		#endregion



		public delegate object WriteSumEntry(VectoRunData r, IModalDataContainer m);

		public delegate object WriteFuelEntry(VectoRunData r, IModalDataContainer m, IFuelProperties f);

		public delegate object WriteEmEntry(VectoRunData r, IModalDataContainer m, PowertrainPosition emPos);


		public delegate object WriteAuxEntry(VectoRunData r, IModalDataContainer m, DataColumn aux);

		private static Tuple<ModalResultField[], WriteSumEntry> SumFunc(WriteSumEntry w, params ModalResultField[] mrf)
		{
			return Tuple.Create(mrf, w);
		}

		private static Tuple<ModalResultField[], WriteSumEntry> SumFunc(WriteSumEntry w)
		{
			return Tuple.Create<ModalResultField[], WriteSumEntry>(null, w);
		}

		private static Tuple<ModalResultField[], WriteFuelEntry> FuelFunc(WriteFuelEntry w)
		{
			return Tuple.Create<ModalResultField[], WriteFuelEntry>(null, w);
		}

		public static int GetSumDataSortingValue(int jobNbr, int runNbr, int iterationCnt)
		{
			var iteration = iterationCnt < 0 ? 99 : iterationCnt;
			return jobNbr * 1000000 + runNbr * 1000 + iteration;
		}

		public static string GetSumDataJobID(int jobNbr, int runNbr, int iterationCnt)
		{
			var iteration = "";
			if (iterationCnt < 0) {
				iteration = "F";
			}

			if (iterationCnt > 0) {
				iteration = iterationCnt.ToString();
			}

			return $"{jobNbr}-{runNbr}-{iteration}";
		}

		public static readonly Dictionary<string, Tuple<ModalResultField[], WriteSumEntry>> SumDataValue =
			new Dictionary<string, Tuple<ModalResultField[], WriteSumEntry>>() {
				// common fields
				{ SORT, SumFunc((r, m) => GetSumDataSortingValue(r.JobNumber, r.RunNumber, r.Iteration))},
				{
					JOB,
					SumFunc((r, m) => GetSumDataJobID(r.JobNumber, r.RunNumber, r.Iteration))
				},
				{ INPUTFILE, SumFunc((r, m) => SummaryDataContainer.ReplaceNotAllowedCharacters(r.JobName)) },
				{
					CYCLE,
					SumFunc((r, m) =>
						SummaryDataContainer.ReplaceNotAllowedCharacters(r.Cycle.Name +
																		Constants.FileExtensions.CycleFile))
				},
				{ STATUS, SumFunc((r, m) => m.RunStatus) },
				{ OVCHEVMode, SumFunc((r, m) => r.OVCMode) },
				{ TIME, SumFunc((r, m) => (ConvertedSI)m.Duration, ModalResultField.time) },
				{ DISTANCE, SumFunc((r, m) => m.Distance?.ConvertToKiloMeter(), ModalResultField.dist) },
				{
					SPEED,
					SumFunc((r, m) => m.Speed()?.ConvertToKiloMeterPerHour(), ModalResultField.dist,
						ModalResultField.time)
				},
				{ ALTITUDE_DELTA, SumFunc((r, m) => (ConvertedSI)m.AltitudeDelta(), ModalResultField.altitude) },

				// Vehicle 
				{ VEHICLE_FUEL_TYPE, SumFunc((r, m) => m.FuelData.Select(x => x.GetLabel()).Join()) }, {
					P_WHEEL_POS,
					SumFunc((r, m) => m.PowerWheelPositive().ConvertToKiloWatt(), ModalResultField.P_wheel_in)
				},
				{ P_WHEEL, SumFunc((r, m) => m.PowerWheel().ConvertToKiloWatt(), ModalResultField.P_wheel_in) },
				{ VEHICLE_MANUFACTURER, SumFunc((r, m) => r.VehicleData?.Manufacturer ?? Constants.NOT_AVAILABLE) },
				{ VIN_NUMBER, SumFunc((r, m) => r.VehicleData?.VIN ?? Constants.NOT_AVAILABLE) },
				{ VEHICLE_MODEL, SumFunc((r, m) => r.VehicleData?.ModelName ?? Constants.NOT_AVAILABLE) }, {
					HDV_CO2_VEHICLE_CLASS,
					SumFunc((r, m) =>
						r.Mission?.BusParameter?.BusGroup.GetClassNumber() ??
						r.VehicleData?.VehicleClass.GetClassNumber() ?? VehicleClass.Unknown.GetClassNumber())
				},
				{ CURB_MASS, SumFunc((r, m) => (ConvertedSI)r.VehicleData?.CurbMass) },
				{ LOADING, SumFunc((r, m) => (ConvertedSI)r.VehicleData?.Loading) },
				{ PassengerCount, SumFunc((r, m) => r.VehicleData.PassengerCount) },
				{ CARGO_VOLUME, SumFunc((r, m) => (ConvertedSI)r.VehicleData?.CargoVolume) },
				{ TOTAL_VEHICLE_MASS, SumFunc((r, m) => (ConvertedSI)r.VehicleData?.TotalVehicleMass) }, {
					SLEEPER_CAB,
					SumFunc((r, m) =>
						(r.VehicleData?.SleeperCab.HasValue ?? false)
							? (r.VehicleData.SleeperCab.Value ? "yes" : "no")
							: "-")
				}, {
					ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER,
					SumFunc((r, m) => r.VehicleData?.RollResistanceCoefficientWithoutTrailer)
				}, {
					ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER,
					SumFunc((r, m) => r.VehicleData?.TotalRollResistanceCoefficient)
				},
				{ R_DYN, SumFunc((r, m) => (ConvertedSI)r.VehicleData?.DynamicTyreRadius) }, {
					ADAS_TECHNOLOGY_COMBINATION, SumFunc((r, m) => {
						string ret = "";
						if (r.VehicleData?.ADAS == null) {
							return null;
						}

						GearboxType? gbxType = null;
						switch (r.InputData) {
							case IMultistepBusInputDataProvider multistep:
								gbxType = multistep.JobInputData?.PrimaryVehicle?.Vehicle?.Components.GetGearboxType();
								break;
							default:
								gbxType = r.InputData?.JobInputData.Vehicle.Components?.GetGearboxType() ??
										r.InputData?.PrimaryVehicleData?.Vehicle.Components?.GetGearboxType();
								break;
						}

						if (gbxType != null) {
							ret = DeclarationData.ADASCombinations.Lookup(r.VehicleData.ADAS, gbxType.Value).ID;
						}

						return ret;
					})
				}, {
					REESS_CAPACITY,
					SumFunc((r, m) => r.BatteryData?.Capacity != null
						?
						$"{r.BatteryData?.Capacity.AsAmpHour.ToString(CultureInfo.InvariantCulture)} Ah"
						: r.SuperCapData?.Capacity != null
							? $"{r.SuperCapData.Capacity.Value().ToString(CultureInfo.InvariantCulture)} F"
							: null)
				},
				{ TCU_MODEL, SumFunc((r, m) => r.ShiftStrategy) },
				{ PTO_TECHNOLOGY, SumFunc((r, m) => r.PTO?.TransmissionType ?? "") },

				// air drag infos
				{ AIRDRAG_MODEL, SumFunc((r, m) => r.AirdragData?.ModelName ?? Constants.NOT_AVAILABLE) }, {
					AIRDRAG_CERTIFICATION_METHOD,
					SumFunc((r, m) => r.AirdragData?.CertificationMethod.GetName() ?? Constants.NOT_AVAILABLE)
				}, {
					AIRDRAG_CERTIFICATION_NUMBER,
					SumFunc((r, m) =>
						r.AirdragData?.CertificationMethod == CertificationMethod.StandardValues
							? ""
							: r.AirdragData?.CertificationNumber)
				},
				{ CD_x_A_DECLARED, SumFunc((r, m) => (ConvertedSI)r.AirdragData?.DeclaredAirdragArea) },
				{ CD_x_A, SumFunc((r, m) => (ConvertedSI)r.AirdragData?.CrossWindCorrectionCurve.AirDragArea) },

				// engine infos
				{ ENGINE_MANUFACTURER, SumFunc((r, m) => r.EngineData?.Manufacturer ?? Constants.NOT_AVAILABLE) },
				{ ENGINE_MODEL, SumFunc((r, m) => r.EngineData?.ModelName ?? Constants.NOT_AVAILABLE) }, {
					ENGINE_CERTIFICATION_NUMBER,
					SumFunc((r, m) => r.EngineData?.CertificationNumber ?? Constants.NOT_AVAILABLE)
				}, {
					ENGINE_FUEL_TYPE,
					SumFunc((r, m) => r.EngineData?.Fuels.Select(x => x.FuelData.GetLabel()).Join(" / "))
				}, {
					ENGINE_RATED_POWER,
					SumFunc((r, m) =>
						r.EngineData?.RatedPowerDeclared != null && r.EngineData.RatedPowerDeclared > 0
							? r.EngineData.RatedPowerDeclared.ConvertToKiloWatt()
							: r.EngineData?.FullLoadCurves[0].MaxPower.ConvertToKiloWatt())
				},
				{ ENGINE_IDLING_SPEED, SumFunc((r, m) => (ConvertedSI)r.EngineData?.IdleSpeed.AsRPM.SI<Scalar>()) }, {
					ENGINE_RATED_SPEED,
					SumFunc((r, m) => r.EngineData?.RatedSpeedDeclared != null && r.EngineData.RatedSpeedDeclared > 0
						? (ConvertedSI)r.EngineData.RatedSpeedDeclared.AsRPM.SI<Scalar>()
						: (ConvertedSI)r.EngineData?.FullLoadCurves[0].RatedSpeed.AsRPM.SI<Scalar>())
				},
				{ ENGINE_DISPLACEMENT, SumFunc((r, m) => r.EngineData?.Displacement.ConvertToCubicCentiMeter()) }, {
					ENGINE_WHTC_URBAN,
					SumFunc((r, m) =>
						r.EngineData?.Fuels.Select(x => x.WHTCUrban.ToString(CultureInfo.InvariantCulture)).Join(" / "))
				}, {
					ENGINE_WHTC_RURAL,
					SumFunc((r, m) =>
						r.EngineData?.Fuels.Select(x => x.WHTCRural.ToString(CultureInfo.InvariantCulture)).Join(" / "))
				}, {
					ENGINE_WHTC_MOTORWAY,
					SumFunc((r, m) =>
						r.EngineData?.Fuels.Select(x => x.WHTCMotorway.ToString(CultureInfo.InvariantCulture))
							.Join(" / "))
				}, {
					ENGINE_BF_COLD_HOT,
					SumFunc((r, m) =>
						r.EngineData?.Fuels
							.Select(x => x.ColdHotCorrectionFactor.ToString(CultureInfo.InvariantCulture)).Join(" / "))
				}, {
					ENGINE_CF_REG_PER,
					SumFunc((r, m) =>
						r.EngineData?.Fuels.Select(x => x.CorrectionFactorRegPer.ToString(CultureInfo.InvariantCulture))
							.Join(" / "))
				}, {
					ENGINE_ACTUAL_CORRECTION_FACTOR, SumFunc((r, m) => {
						if (r.Mission?.MissionType == MissionType.VerificationTest) {
							var fuelsWhtc = r.EngineData?.Fuels.Select(
									fuel => m.TimeIntegral<Kilogram>(m.GetColumnName(fuel.FuelData,
												ModalResultField.FCWHTCc)) /
											m.TimeIntegral<Kilogram>(m.GetColumnName(fuel.FuelData,
												ModalResultField.FCMap)))
								.Select(dummy => ((double)dummy).ToString(CultureInfo.InvariantCulture)).ToArray();
							return fuelsWhtc?.Join(" / ");
						}

						return r.EngineData?.Fuels.Select(x =>
							x.FuelConsumptionCorrectionFactor.ToString(CultureInfo.InvariantCulture)).Join(" / ");
					})
				},

				// axlegear infos
				{ AXLE_MANUFACTURER, SumFunc((r, m) => r.AxleGearData?.Manufacturer ?? Constants.NOT_AVAILABLE) },
				{ AXLE_MODEL, SumFunc((r, m) => r.AxleGearData?.ModelName ?? Constants.NOT_AVAILABLE) },
				{ AXLE_RATIO, SumFunc((r, m) => (ConvertedSI)r.AxleGearData?.AxleGear.Ratio.SI<Scalar>()) }, {
					AXLEGEAR_CERTIFICATION_METHOD,
					SumFunc((r, m) => r.AxleGearData?.CertificationMethod.GetName() ?? Constants.NOT_AVAILABLE)
				}, {
					AXLEGEAR_CERTIFICATION_NUMBER,
					SumFunc((r, m) =>
						r.AxleGearData?.CertificationMethod == CertificationMethod.StandardValues
							? ""
							: r.AxleGearData?.CertificationNumber)
				},

				// axle/tyre infos
				{
					NUM_AXLES_DRIVEN,
					SumFunc((r, m) => r.VehicleData?.AxleData.Count(x => x.AxleType == AxleType.VehicleDriven))
				}, {
					NUM_AXLES_NON_DRIVEN,
					SumFunc((r, m) => r.VehicleData?.AxleData.Count(x => x.AxleType == AxleType.VehicleNonDriven))
				}, {
					NUM_AXLES_TRAILER,
					SumFunc((r, m) => r.VehicleData?.AxleData.Count(x => x.AxleType == AxleType.Trailer))
				}, {
					DECLARED_RRC_AXLE1,
					SumFunc((r, m) =>
						r.VehicleData?.AxleData?.Count > 0 && r.VehicleData.AxleData[0].AxleType != AxleType.Trailer
							? r.VehicleData.AxleData[0].RollResistanceCoefficient
							: double.NaN)
				}, {
					DECLARED_RRC_AXLE2,
					SumFunc((r, m) =>
						r.VehicleData?.AxleData?.Count > 1 && r.VehicleData.AxleData[1].AxleType != AxleType.Trailer
							? r.VehicleData.AxleData[1].RollResistanceCoefficient
							: double.NaN)
				}, {
					DECLARED_RRC_AXLE3,
					SumFunc((r, m) =>
						r.VehicleData?.AxleData?.Count > 2 && r.VehicleData.AxleData[2].AxleType != AxleType.Trailer
							? r.VehicleData.AxleData[2].RollResistanceCoefficient
							: double.NaN)
				}, {
					DECLARED_RRC_AXLE4,
					SumFunc((r, m) =>
						r.VehicleData?.AxleData?.Count > 3 && r.VehicleData.AxleData[3].AxleType != AxleType.Trailer
							? r.VehicleData.AxleData[3].RollResistanceCoefficient
							: double.NaN)
				}, {
					DECLARED_FZISO_AXLE1,
					SumFunc((r, m) =>
						r.VehicleData?.AxleData?.Count > 0 && r.VehicleData.AxleData[0].AxleType != AxleType.Trailer
							? (ConvertedSI)r.VehicleData.AxleData[0].TyreTestLoad
							: null)
				}, {
					DECLARED_FZISO_AXLE2,
					SumFunc((r, m) =>
						r.VehicleData?.AxleData?.Count > 1 && r.VehicleData.AxleData[1].AxleType != AxleType.Trailer
							? (ConvertedSI)r.VehicleData.AxleData[1].TyreTestLoad
							: null)
				}, {
					DECLARED_FZISO_AXLE3,
					SumFunc((r, m) =>
						r.VehicleData?.AxleData?.Count > 2 && r.VehicleData.AxleData[2].AxleType != AxleType.Trailer
							? (ConvertedSI)r.VehicleData.AxleData[2].TyreTestLoad
							: null)
				}, {
					DECLARED_FZISO_AXLE4,
					SumFunc((r, m) =>
						r.VehicleData?.AxleData?.Count > 3 && r.VehicleData.AxleData[3].AxleType != AxleType.Trailer
							? (ConvertedSI)r.VehicleData.AxleData[3].TyreTestLoad
							: null)
				},

				// angle drive
				{
					ANGLEDRIVE_MANUFACTURER,
					SumFunc((r, m) => r.AngledriveData?.Manufacturer ?? Constants.NOT_AVAILABLE)
				},
				{ ANGLEDRIVE_MODEL, SumFunc((r, m) => r.AngledriveData?.ModelName ?? Constants.NOT_AVAILABLE) },
				{ ANGLEDRIVE_RATIO, SumFunc((r, m) => (ConvertedSI)r.AngledriveData?.Angledrive.Ratio.SI<Scalar>()) }, {
					ANGLEDRIVE_CERTIFICATION_METHOD,
					SumFunc((r, m) => r.AngledriveData?.CertificationMethod.GetName() ?? "")
				}, {
					ANGLEDRIVE_CERTIFICATION_NUMBER,
					SumFunc((r, m) =>
						r.AngledriveData == null ||
						r.AngledriveData.CertificationMethod == CertificationMethod.StandardValues
							? ""
							: r.AngledriveData.CertificationNumber)
				},

				// retarder
				{ RETARDER_TYPE, SumFunc((r, m) => (r.Retarder?.Type ?? RetarderType.None).GetLabel()) }, {
					RETARDER_MANUFACTURER,
					SumFunc((r, m) =>
						r.Retarder != null && r.Retarder.Type.IsDedicatedComponent()
							? r.Retarder.Manufacturer
							: Constants.NOT_AVAILABLE)
				}, {
					RETARDER_MODEL,
					SumFunc((r, m) =>
						r.Retarder != null && r.Retarder.Type.IsDedicatedComponent()
							? r.Retarder.ModelName
							: Constants.NOT_AVAILABLE)
				}, {
					RETARDER_CERTIFICATION_METHOD,
					SumFunc((r, m) =>
						r.Retarder != null && r.Retarder.Type.IsDedicatedComponent()
							? r.Retarder?.CertificationMethod.GetName()
							: "")
				}, {
					RETARDER_CERTIFICATION_NUMBER,
					SumFunc((r, m) =>
						r.Retarder == null || !r.Retarder.Type.IsDedicatedComponent() ||
						r.Retarder.CertificationMethod == CertificationMethod.StandardValues
							? ""
							: r.Retarder.CertificationNumber)
				},

				// gearbox
				{ GEARBOX_MANUFACTURER, SumFunc((r, m) => r.GearboxData?.Manufacturer ?? Constants.NOT_AVAILABLE) },
				{ GEARBOX_MODEL, SumFunc((r, m) => r.GearboxData?.ModelName ?? Constants.NOT_AVAILABLE) },
				{ GEARBOX_TYPE, SumFunc((r, m) => r.GearboxData?.Type.ToXMLFormat() ?? Constants.NOT_AVAILABLE) }, {
					GEARBOX_CERTIFICATION_NUMBER,
					SumFunc((r, m) =>
						r.GearboxData?.CertificationMethod == CertificationMethod.StandardValues
							? ""
							: r.GearboxData?.CertificationNumber)
				},
				{ GEARBOX_CERTIFICATION_METHOD, SumFunc((r, m) => r.GearboxData?.CertificationMethod.GetName()) },

				// torque converter
				{
					TORQUECONVERTER_MANUFACTURER,
					SumFunc((r, m) => r.GearboxData?.TorqueConverterData?.Manufacturer ?? Constants.NOT_AVAILABLE)
				}, {
					TORQUECONVERTER_MODEL,
					SumFunc((r, m) => r.GearboxData?.TorqueConverterData?.ModelName ?? Constants.NOT_AVAILABLE)
				}, {
					TORQUE_CONVERTER_CERTIFICATION_NUMBER,
					SumFunc((r, m) =>
						r.GearboxData?.TorqueConverterData?.CertificationMethod == CertificationMethod.StandardValues
							? ""
							: r.GearboxData?.TorqueConverterData?.CertificationNumber ?? "")
				}, {
					TORQUE_CONVERTER_CERTIFICATION_METHOD,
					SumFunc((r, m) => r.GearboxData?.TorqueConverterData?.CertificationMethod.GetName() ?? "")
				},

				// engine 
				{
					P_FCMAP_POS,
					SumFunc((r, m) => m.TotalPowerEnginePositiveAverage().ConvertToKiloWatt(),
						ModalResultField.P_ice_fcmap, ModalResultField.simulationInterval)
				}, {
					P_FCMAP,
					SumFunc((r, m) => m.TotalPowerEngineAverage().ConvertToKiloWatt(), ModalResultField.P_ice_fcmap,
						ModalResultField.simulationInterval)
				}, {
					E_FCMAP_POS,
					SumFunc((r, m) => m.TotalEngineWorkPositive().ConvertToKiloWattHour(), ModalResultField.P_ice_fcmap)
				}, {
					E_FCMAP_NEG,
					SumFunc((r, m) => (-m.TotalEngineWorkNegative()).ConvertToKiloWattHour(),
						ModalResultField.P_ice_fcmap)
				}, {
					E_ICE_START,
					SumFunc((r, m) => m.WorkEngineStart().ConvertToKiloWattHour(), ModalResultField.P_ice_start)
				},

				{ E_WHR_EL, SumFunc((r, m) => m.CorrectedModalData.WorkWHREl.ConvertToKiloWattHour()) },
				{ E_WHR_MECH, SumFunc((r, m) => m.CorrectedModalData.WorkWHRMech.ConvertToKiloWattHour()) },
				{ E_BusAux_PS_corr, SumFunc((r, m) => m.CorrectedModalData.WorkBusAuxPSCorr.ConvertToKiloWattHour()) }, {
					E_BusAux_el_PS_corr,
					SumFunc((r, m) => m.CorrectedModalData.WorkBusAux_elPS_SoC_ElRange.ConvertToKiloWattHour())
				}, {
					E_BusAux_ES_mech_corr,
					SumFunc((r, m) => m.CorrectedModalData.WorkBusAuxESMech.ConvertToKiloWattHour())
				}, {
					E_BusAux_AuxHeater,
					SumFunc((r, m) => m.CorrectedModalData.AuxHeaterDemand.Cast<WattSecond>().ConvertToKiloWattHour())
				}, {
					E_POWERTRAIN_INERTIA,
					SumFunc((r, m) => m.PowerAccelerations().ConvertToKiloWattHour(), ModalResultField.P_ice_inertia,
						ModalResultField.P_gbx_inertia)
				},

				{ E_AUX, SumFunc((r, m) => m.WorkAuxiliaries()?.ConvertToKiloWattHour(), ModalResultField.P_aux_mech) }, {
					E_AUX_EL,
					SumFunc((r, m) => m.WorkElectricAuxiliaries()?.ConvertToKiloWattHour(), ModalResultField.P_aux_el)
				}, {
					E_AUX_EL_HV,
					SumFunc((r, m) => m.TimeIntegral<WattSecond>(ModalResultField.P_Aux_el_HV).ConvertToKiloWattHour(),
						ModalResultField.P_Aux_el_HV)
				}, {
					E_CLUTCH_LOSS,
					SumFunc((r, m) => m.WorkClutch().ConvertToKiloWattHour(), ModalResultField.P_clutch_loss)
				}, {
					E_TC_LOSS,
					SumFunc((r, m) => m.WorkTorqueConverter().ConvertToKiloWattHour(), ModalResultField.P_TC_loss)
				}, {
					E_SHIFT_LOSS,
					SumFunc((r, m) => m.WorkGearshift().ConvertToKiloWattHour(), ModalResultField.P_gbx_shift_loss)
				},
				{ E_GBX_LOSS, SumFunc((r, m) => m.WorkGearbox().ConvertToKiloWattHour(), ModalResultField.P_gbx_loss) }, {
					E_RET_LOSS, SumFunc((r, m) => m.WorkRetarder().ConvertToKiloWattHour(), ModalResultField.P_ret_loss)
				}, {
					E_AXL_LOSS,
					SumFunc((r, m) => m.WorkAxlegear().ConvertToKiloWattHour(), ModalResultField.P_axle_loss)
				}, {
					E_ANGLE_LOSS,
					SumFunc((r, m) => m.WorkAngledrive().ConvertToKiloWattHour(), ModalResultField.P_angle_loss)
				}, {
					E_WHEELEND_SAVED,
					SumFunc((r, m) => m.WorkWheelEnd().ConvertToKiloWattHour(), ModalResultField.P_wheelEnd_saving)
				}, {
					E_BRAKE,
					SumFunc((r, m) => m.WorkTotalMechanicalBrake().ConvertToKiloWattHour(),
						ModalResultField.P_brake_loss)
				}, {
					E_VEHICLE_INERTIA,
					SumFunc((r, m) => m.WorkVehicleInertia().ConvertToKiloWattHour(), ModalResultField.P_veh_inertia,
						ModalResultField.P_wheel_inertia)
				},
				{ E_AIR, SumFunc((r, m) => m.WorkAirResistance().ConvertToKiloWattHour(), ModalResultField.P_air) }, {
					E_ROLL,
					SumFunc((r, m) => m.WorkRollingResistance().ConvertToKiloWattHour(), ModalResultField.P_roll)
				}, {
					E_GRAD,
					SumFunc((r, m) => m.WorkRoadGradientResistance().ConvertToKiloWattHour(), ModalResultField.P_slope)
				},
				{ E_AUX_ESS_missing, SumFunc((r, m) => m.CorrectedModalData.WorkESSMissing.ConvertToKiloWattHour()) },


			{E_WHEEL, SumFunc((r, m) => m.WorkWheels().ConvertToKiloWattHour(), ModalResultField.P_wheel_in)},
			
			// BusAux
			{ AirGenerated, SumFunc((r, m) => (ConvertedSI)m.AirGenerated(), ModalResultField.Nl_busAux_PS_generated)},
			{ AirConsumed, SumFunc((r, m) => (ConvertedSI)m.AirConsumed(), ModalResultField.Nl_busAux_PS_consumer)},
			{ E_PS_CompressorOff, SumFunc((r, m) => m.EnergyPneumaticCompressorPowerOff().ConvertToKiloWattHour(), ModalResultField.P_busAux_PS_generated_dragOnly)},
			{ E_PS_CompressorOn, SumFunc((r, m) => m.EnergyPneumaticCompressorOn().ConvertToKiloWattHour(), ModalResultField.Nl_busAux_PS_generated)},
			{ E_BusAux_ES_generated, SumFunc((r, m) => m.EnergyBusAuxESGenerated().ConvertToKiloWattHour(), ModalResultField.P_busAux_ES_generated)},
			{ E_BusAux_ES_consumed, SumFunc((r, m) => m.EnergyBusAuxESConsumed().ConvertToKiloWattHour(), ModalResultField.P_busAux_ES_consumer_sum)},
			{ Delta_E_BusAux_Battery, SumFunc((r, m) => (r.BusAuxiliaries?.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
					? m.DeltaSOCBusAuxBattery() * r.BusAuxiliaries.ElectricalUserInputsConfig.ElectricStorageCapacity
					: 0.SI<WattSecond>())
				.ConvertToKiloWattHour()) },
			{ E_BusAux_HVAC_Mech, SumFunc((r, m) => m.TimeIntegral<WattSecond>(ModalResultField.P_busAux_HVACmech_consumer).ConvertToKiloWattHour(), ModalResultField.P_busAux_HVACmech_consumer)}, 
			{ E_BusAux_HVAC_El, SumFunc((r, m) => m.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_HVAC).ConvertToKiloWattHour(), ModalResultField.P_busAux_ES_HVAC)},

				// REESS
				{
					E_REESS_LOSS,
					SumFunc((r, m) => (m.REESSLoss() + m.ESConnectorLoss()).ConvertToKiloWattHour(),
						ModalResultField.P_reess_loss)
				}, {
					E_REESS_T_chg,
					SumFunc((r, m) => m.WorkREESSChargeTerminal().ConvertToKiloWattHour(),
						ModalResultField.P_reess_terminal)
				}, {
					E_REESS_T_dischg,
					SumFunc((r, m) => m.WorkREESSDischargeTerminal().ConvertToKiloWattHour(),
						ModalResultField.P_reess_terminal)
				}, {
					E_REESS_int_chg,
					SumFunc((r, m) => m.WorkREESSChargeInternal().ConvertToKiloWattHour(), ModalResultField.P_reess_int)
				}, {
					E_REESS_int_dischg,
					SumFunc((r, m) => m.WorkREESSDischargeInternal().ConvertToKiloWattHour(),
						ModalResultField.P_reess_int)
				}, {
					REESS_StartSoC,
					SumFunc((r, m) =>
						r.BatteryData != null ? r.BatteryData.InitialSoC * 100 :
						r.SuperCapData != null ? r.SuperCapData.InitialSoC * 100 : double.NaN)
				},
				{ REESS_EndSoC, SumFunc((r, m) => m.REESSEndSoC(), ModalResultField.REESSStateOfCharge) },
				{ REESS_MinSoC, SumFunc((r, m) => m.REESSMinSoc(), ModalResultField.REESSStateOfCharge) },
				{ REESS_MaxSoC, SumFunc((r, m) => m.REESSMaxSoc(), ModalResultField.REESSStateOfCharge) }, {
					REESS_DeltaEnergy,
					SumFunc(
						(r, m) => m.TimeIntegral<WattSecond>(ModalResultField.P_reess_int.GetName())
							.ConvertToKiloWattHour(), ModalResultField.P_reess_int)
				},

				//P-HEV
				{ f_equiv, SumFunc((r, m) => r.HybridStrategyParameters?.EquivalenceFactor ?? double.NaN) },

				// performance entries
				{ ACC, SumFunc((r, m) => (ConvertedSI)m.AccelerationAverage(), ModalResultField.acc) },
				{ ACC_POS, SumFunc((r, m) => (ConvertedSI)m.AccelerationsPositive(), ModalResultField.acc) },
				{ ACC_NEG, SumFunc((r, m) => (ConvertedSI)m.AccelerationsNegative(), ModalResultField.acc) },
				{ ACC_TIMESHARE, SumFunc((r, m) => (ConvertedSI)m.AccelerationTimeShare(), ModalResultField.acc) },
				{ DEC_TIMESHARE, SumFunc((r, m) => (ConvertedSI)m.DecelerationTimeShare(), ModalResultField.acc) }, {
					CRUISE_TIMESHARE,
					SumFunc((r, m) => (ConvertedSI)m.CruiseTimeShare(), ModalResultField.v_act, ModalResultField.acc)
				},
				{ STOP_TIMESHARE, SumFunc((r, m) => (ConvertedSI)m.StopTimeShare(), ModalResultField.v_act) },
				{ MAX_SPEED, SumFunc((r, m) => (ConvertedSI)m.MaxSpeed().AsKmph.SI<Scalar>(), ModalResultField.v_act) },
				{ MAX_ACCELERATION, SumFunc((r, m) => (ConvertedSI)m.MaxAcceleration(), ModalResultField.acc) },
				{ MAX_DECELERATION, SumFunc((r, m) => (ConvertedSI)m.MaxDeceleration(), ModalResultField.acc) }, {
					AVG_ENGINE_SPEED,
					SumFunc((r, m) => (ConvertedSI)m.AvgEngineSpeed().AsRPM.SI<Scalar>(), ModalResultField.n_ice_avg)
				}, {
					MAX_ENGINE_SPEED,
					SumFunc((r, m) => (ConvertedSI)m.MaxEngineSpeed().AsRPM.SI<Scalar>(), ModalResultField.n_ice_avg)
				}, {
					AVERAGE_POS_ACC,
					SumFunc((r, m) => (ConvertedSI)m.AverageAccelerationBelowTargetSpeed(), ModalResultField.acc,
						ModalResultField.v_act, ModalResultField.v_targ)
				}, {
					AVERAGE_ENGINE_EFFICIENCY, SumFunc((r, m) => {
						var eFC = 0.SI<Joule>();
						foreach (var fuel in m.FuelData) {
							eFC += m.TimeIntegral<Kilogram>(m.GetColumnName(fuel, ModalResultField.FCFinal)) *
									fuel.LowerHeatingValueVecto;
						}

						var eIcePos = m.TimeIntegral<WattSecond>(ModalResultField.P_ice_fcmap, x => x > 0);
						return eFC.IsEqual(0, 1e-9) ? 0 : (eIcePos / eFC).Value();
					}, ModalResultField.FCFinal, ModalResultField.P_ice_fcmap)
				}, {
					AVERAGE_GEARBOX_EFFICIENCY, SumFunc((r, m) => {
						var gbxOutSignal =
							r.Retarder != null && r.Retarder.Type == RetarderType.TransmissionOutputRetarder
								? ModalResultField.P_retarder_in
								: (r.AngledriveData == null ? ModalResultField.P_axle_in : ModalResultField.P_angle_in);
						var eGbxIn = m.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, x => x > 0);
						var eGbxOut = m.TimeIntegral<WattSecond>(gbxOutSignal, x => x > 0);
						return eGbxIn.IsEqual(0, 1e-9) ? 0 : (eGbxOut / eGbxIn).Value();
					}, ModalResultField.P_gbx_in)
				}, {
					AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITHOUT_LOCKUP, SumFunc((r, m) => {
						var eTcIn = m.TimeIntegral<WattSecond>(ModalResultField.P_TC_in, x => x > 0);
						var eTcOut = m.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, x => x > 0);
						;
						return eTcIn.IsEqual(0, 1e-9) ? 0 : (eTcOut / eTcIn).Value();
					}, ModalResultField.P_gbx_in, ModalResultField.P_TC_in)
				}, {
					AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITH_LOCKUP, SumFunc((r, m) => {
						var tcData = m.GetValues(
							x => new {
								dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()),
								locked = x.Field<int>(ModalResultField.TC_Locked.GetName()),
								P_TCin = x.Field<Watt>(ModalResultField.P_TC_in.GetName()),
								P_TCout = x.Field<Watt>(ModalResultField.P_TC_out.GetName())
							});
						var eTcIn = 0.SI<WattSecond>();
						var eTcOut = 0.SI<WattSecond>();
						foreach (var entry in tcData.Where(x => x.locked == 0)) {
							eTcIn += entry.dt * entry.P_TCin;
							eTcOut += entry.dt * entry.P_TCout;
						}

						return eTcIn.IsEqual(0, 1e-9) ? 0 : (eTcOut / eTcIn).Value();
					}, ModalResultField.P_TC_in, ModalResultField.P_TC_out, ModalResultField.TC_Locked)
				}, {
					AVERAGE_ANGLEDRIVE_EFFICIENCY, SumFunc((r, m) => {
						if (r.AngledriveData == null) {
							return null;
						}

						var eAngleIn = m.TimeIntegral<WattSecond>(ModalResultField.P_angle_in, x => x > 0);
						var eAngleOut = m.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, x => x > 0);
						return (eAngleOut / eAngleIn).Value();
					}, new[] { ModalResultField.P_angle_in, ModalResultField.P_axle_in })
				}, {
					AVERAGE_AXLEGEAR_EFFICIENCY, SumFunc((r, m) => {
						if (r.AxleGearData == null) {
							return null;
						}

						var eAxlIn = m.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, x => x > 0);
						var eAxlOut = m.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, x => x > 0);
						return eAxlIn.IsEqual(0, 1e-9) ? 0 : (eAxlOut / eAxlIn).Value();
					}, ModalResultField.P_axle_in, ModalResultField.P_brake_in)
				}, {
					NUM_GEARSHIFTS, SumFunc((r, m) => {
						var gears = ((uint?)r.GearboxData?.Gears.Count ?? 0u);
						return (gears == 1 || gears == 0)
							? 0.SI<Scalar>()
							: (ConvertedSI)m.GearshiftCount();
					})
				}, {
					COASTING_TIME_SHARE,
					SumFunc((r, m) => (ConvertedSI)m.CoastingTimeShare(), ModalResultField.drivingBehavior)
				}, {
					BRAKING_TIME_SHARE,
					SumFunc((r, m) => (ConvertedSI)m.BrakingTimeShare(), ModalResultField.drivingBehavior)
				}, {
					ICE_FULL_LOAD_TIME_SHARE,
					SumFunc((r, m) => (ConvertedSI)m.ICEMaxLoadTimeShare(), ModalResultField.T_ice_fcmap,
						ModalResultField.T_ice_full)
				},
				{ ICE_OFF_TIME_SHARE, SumFunc((r, m) => (ConvertedSI)m.ICEOffTimeShare(), ModalResultField.ICEOn) }, {
					NUM_ICE_STARTS,
					SumFunc((r, m) => (ConvertedSI)m.NumICEStarts().SI<Scalar>(), ModalResultField.ICEOn)
				},

				// CO2
				{
					CO2_KM, SumFunc((r, m)
						=> m.CorrectedModalData.KilogramCO2PerMeter.ConvertToGrammPerKiloMeter(), ModalResultField.dist)
				}, {
					CO2_TKM, SumFunc((r, m)
						=> r.VehicleData?.Loading == null || r.VehicleData.Loading.IsEqual(0) || m.Distance.IsEqual(0)
							? null
							: (m.CorrectedModalData.KilogramCO2PerMeter / r.VehicleData.Loading)
							.ConvertToGrammPerTonKilometer(), ModalResultField.dist)
				}, {
					CO2_M3KM, SumFunc((r, m)
						=> r.VehicleData?.CargoVolume == null || r.VehicleData.CargoVolume.IsEqual(0) ||
							m.Distance.IsEqual(0)
							? null
							: (m.CorrectedModalData.KilogramCO2PerMeter / r.VehicleData.CargoVolume)
							.ConvertToGrammPerCubicMeterKiloMeter(), ModalResultField.dist)
				}, {
					CO2_PKM, SumFunc((r, m)
						=> r.VehicleData?.PassengerCount == null || m.Distance.IsEqual(0)
							? null
							: (m.CorrectedModalData.KilogramCO2PerMeter / r.VehicleData.PassengerCount.Value)
							.ConvertToGrammPerKiloMeter(), ModalResultField.dist)
				},

				// electric consumption
				{
					EC_el_SOC, SumFunc((r, m) =>
						m.CorrectedModalData.ElectricEnergyConsumption_SoC?.ConvertToKiloWattHour())
				}, {
					EC_el_SOC_corr, SumFunc((r, m) =>
						m.CorrectedModalData.ElectricEnergyConsumption_SoC_Corr?.ConvertToKiloWattHour())
				}, {
					EC_el_final, SumFunc((r, m)
						=> (m.CorrectedModalData.ElectricEnergyConsumption_Final?.ConvertToKiloWattHour()))
				}, {
					EC_el_final_KM, SumFunc((r, m)
						=> (m.CorrectedModalData.ElectricEnergyConsumption_Final_PerMeter)
						?.ConvertToKiloWattHourPerKiloMeter())
				}, {
					EC_el_final_TKM, SumFunc((r, m)
						=> r.VehicleData?.Loading == null ||
							r.VehicleData.Loading.IsEqual(0) ||
							m.CorrectedModalData.ElectricEnergyConsumption_Final_PerMeter == null
							? null
							: (m.CorrectedModalData.ElectricEnergyConsumption_Final_PerMeter / r.VehicleData.Loading)
							.ConvertToKiloWattHourPerTonKiloMeter())
				}, {
					EC_el_final_M3KM, SumFunc((r, m)
						=> r.VehicleData.CargoVolume == null ||
							r.VehicleData.CargoVolume.IsEqual(0) ||
							m.CorrectedModalData.ElectricEnergyConsumption_Final_PerMeter == null
							? null
							: (m.CorrectedModalData.ElectricEnergyConsumption_Final_PerMeter /
								r.VehicleData.CargoVolume).ConvertToKiloWattHourPerCubicMeterKiloMeter())
				}, {
					ElectricEnergyConsumption_PKM, SumFunc((r, m)
						=> r.VehicleData?.PassengerCount == null ||
							m.CorrectedModalData.ElectricEnergyConsumption_Final_PerMeter == null
							? null
							: (m.CorrectedModalData.ElectricEnergyConsumption_Final_PerMeter /
								r.VehicleData.PassengerCount.Value).ConvertToKiloWattHourPerPassengerKiloMeter())
				},
				//			{, SumFunc((r, m) =>)},

			};


		public static readonly Dictionary<string, Tuple<ModalResultField[], WriteFuelEntry>> FuelDataValue =
			new Dictionary<string, Tuple<ModalResultField[], WriteFuelEntry>>() {
				{
					FCMAP_H,
					FuelFunc(
						(r, m, f) => m.FuelConsumptionPerSecond(ModalResultField.FCMap, f)?.ConvertToGrammPerHour())
				}, {
					FCMAP_KM,
					FuelFunc((r, m, f) =>
						m.FuelConsumptionPerMeter(ModalResultField.FCMap, f)?.ConvertToGrammPerKiloMeter())
				}, {
					FCNCVC_H,
					FuelFunc((r, m, f) =>
						m.FuelConsumptionPerSecond(ModalResultField.FCNCVc, f)?.ConvertToGrammPerHour())
				}, {
					FCNCVC_KM,
					FuelFunc((r, m, f) =>
						m.FuelConsumptionPerMeter(ModalResultField.FCNCVc, f)?.ConvertToGrammPerKiloMeter())
				}, {
					FCWHTCC_H,
					FuelFunc((r, m, f) =>
						m.FuelConsumptionPerSecond(ModalResultField.FCWHTCc, f)?.ConvertToGrammPerHour())
				}, {
					FCWHTCC_KM,
					FuelFunc((r, m, f) =>
						m.FuelConsumptionPerMeter(ModalResultField.FCWHTCc, f)?.ConvertToGrammPerKiloMeter())
				}, {
					K_ENGLINE,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).EngineLineCorrectionFactor
						?.ConvertToGramPerKiloWattHour())
				}, {
					K_VEHLINE,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).VehicleLine
						?.ConvertToGramPerKiloWattHour())
				}, {
					FCESS_H,
					FuelFunc((r, m, f) =>
						m.CorrectedModalData.FuelConsumptionCorrection(f).FC_ESS_H?.ConvertToGrammPerHour())
				}, {
					FCESS_H_CORR,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_ESS_CORR_H
						?.ConvertToGrammPerHour())
				}, {
					FC_BusAux_PS_CORR_H,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_BusAux_PS_CORR_H
						?.ConvertToGrammPerHour())
				}, {
					FC_BusAux_ES_CORR_H,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_BusAux_ES_CORR_H
						?.ConvertToGrammPerHour())
				}, {
					FCWHR_H_CORR,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_WHR_CORR_H
						?.ConvertToGrammPerHour())
				}, {
					FC_HEV_SOC_CORR_H,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_REESS_SOC_CORR_H
						?.ConvertToGrammPerHour())
				}, {
					FC_HEV_SOC_H,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_REESS_SOC_H
						?.ConvertToGrammPerHour())
				}, {
					FC_AUXHTR_H,
					FuelFunc((r, m, f) =>
						m.CorrectedModalData.FuelConsumptionCorrection(f).FC_AUXHTR_H?.ConvertToGrammPerHour())
				}, {
					FC_AUXHTR_H_CORR,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_AUXHTR_H_CORR
						?.ConvertToGrammPerHour())
				}, {
					FCFINAL_H,
					FuelFunc((r, m, f) =>
						m.CorrectedModalData.FuelConsumptionCorrection(f).FC_FINAL_H?.ConvertToGrammPerHour())
				}, {
					FCWHR_KM_CORR,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_WHR_CORR_KM
						?.ConvertToGrammPerKiloMeter())
				}, {
					FC_BusAux_PS_CORR_KM,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_BusAux_PS_CORR_KM
						?.ConvertToGrammPerKiloMeter())
				}, {
					FC_BusAux_ES_CORR_KM,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_BusAux_ES_CORR_KM
						?.ConvertToGrammPerKiloMeter())
				}, {
					FC_HEV_SOC_CORR_KM,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_REESS_SOC_CORR_KM
						?.ConvertToGrammPerKiloMeter())
				}, {
					FC_HEV_SOC_KM,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_REESS_SOC_KM
						?.ConvertToGrammPerKiloMeter())
				}, {
					FC_AUXHTR_KM,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_AUXHTR_KM
						?.ConvertToGrammPerKiloMeter())
				}, {
					FC_AUXHTR_KM_CORR,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_AUXHTR_KM_CORR
						?.ConvertToGrammPerKiloMeter())
				}, {
					FCESS_KM,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_ESS_KM
						?.ConvertToGrammPerKiloMeter())
				}, {
					FCESS_KM_CORR,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_ESS_CORR_KM
						?.ConvertToGrammPerKiloMeter())
				}, {
					FCFINAL_KM,
					FuelFunc((r, m, f) => m.CorrectedModalData.FuelConsumptionCorrection(f).FC_FINAL_KM
						?.ConvertToGrammPerKiloMeter())
				}, {
					FCFINAL_LITERPER100KM,
					FuelFunc((r, m, f) =>
						f.FuelDensity == null
							? null
							: m.CorrectedModalData.FuelConsumptionCorrection(f).FuelVolumePerMeter
								.ConvertToLiterPer100Kilometer())
				}, {
					FCFINAL_LITERPER100TKM,
					FuelFunc((r, m, f) =>
						f.FuelDensity == null || r.VehicleData?.Loading == null || r.VehicleData.Loading.IsEqual(0) ||
						m.CorrectedModalData.FuelConsumptionCorrection(f).FuelVolumePerMeter == null
							? null
							: (m.CorrectedModalData.FuelConsumptionCorrection(f).FuelVolumePerMeter /
								r.VehicleData.Loading).ConvertToLiterPer100TonKiloMeter())
				}, {
					FCFINAL_LiterPer100M3KM,
					FuelFunc((r, m, f) =>
						f.FuelDensity == null || r.VehicleData?.CargoVolume == null ||
						r.VehicleData.CargoVolume.IsEqual(0) ||
						m.CorrectedModalData.FuelConsumptionCorrection(f).FuelVolumePerMeter == null
							? null
							: (m.CorrectedModalData.FuelConsumptionCorrection(f).FuelVolumePerMeter /
								r.VehicleData.CargoVolume).ConvertToLiterPerCubicMeter100KiloMeter())
				}, {
					FCFINAL_LiterPer100PassengerKM,
					FuelFunc((r, m, f) =>
						f.FuelDensity == null || r.VehicleData?.PassengerCount == null ||
						m.CorrectedModalData.FuelConsumptionCorrection(f).FuelVolumePerMeter == null
							? null
							: (m.CorrectedModalData.FuelConsumptionCorrection(f).FuelVolumePerMeter /
								r.VehicleData.PassengerCount.Value).ConvertToLiterPer100Kilometer())
				}, {
					SPECIFIC_FC,
					FuelFunc((r, m, f) =>
						r.Cycle.CycleType == CycleType.VTP
							? (m.TotalFuelConsumption(ModalResultField.FCFinal, f) / m.WorkWheelsPos())
							.ConvertToGramPerKiloWattHour()
							: null)
				},

			};

		public static readonly Dictionary<string, WriteEmEntry> ElectricMotorValue = new Dictionary<string, WriteEmEntry>() {
			{ EM_AVG_SPEED_FORMAT, (r, m, em) =>    m.ElectricMotorAverageSpeed(em).ConvertToRoundsPerMinute() },
			{ E_EM_Mot_DRIVE_FORMAT, (r, m, em) => m.TotalElectricMotorMotWorkDrive(em).ConvertToKiloWattHour() },
			{ E_EM_Mot_GENERATE_FORMAT, (r, m, em) => m.TotalElectricMotorMotWorkRecuperate(em).ConvertToKiloWattHour() },
			{ ETA_EM_Mot_DRIVE_FORMAT, (r, m, em) =>    new ConvertedSI(m.ElectricMotorMotEfficiencyDrive(em), "") },
			{ ETA_EM_Mot_GEN_FORMAT, (r, m, em) =>  new ConvertedSI(m.ElectricMotorMotEfficiencyGenerate(em), "") },
			{ E_EM_DRIVE_FORMAT, (r, m, em) => m.TotalElectricMotorWorkDrive(em).ConvertToKiloWattHour() },
			{ E_EM_GENERATE_FORMAT, (r, m, em) => m.TotalElectricMotorWorkRecuperate(em).ConvertToKiloWattHour() },
			{ ETA_EM_DRIVE_FORMAT, (r, m, em) => new ConvertedSI(m.ElectricMotorEfficiencyDrive(em), "") },
			{ ETA_EM_GEN_FORMAT, (r, m, em) => new ConvertedSI(m.ElectricMotorEfficiencyGenerate(em), "") },
			{ E_EM_OFF_Loss_Format, (r, m, em) => m.ElectricMotorOffLosses(em).ConvertToKiloWattHour() },
			{ E_EM_LOSS_TRANSM_FORMAT, (r, m, em) => m.ElectricMotorTransmissionLosses(em)?.ConvertToKiloWattHour() },
			{ E_EM_Mot_LOSS_FORMAT, (r, m, em) => m.ElectricMotorMotLosses(em)?.ConvertToKiloWattHour() },
			{ E_EM_LOSS_FORMAT, (r, m, em) => m.ElectricMotorLosses(em)?.ConvertToKiloWattHour() },
			{ E_EM_OFF_TIME_SHARE, (r, m, em) => (ConvertedSI)m.ElectricMotorOffTimeShare(em) },
			{ EM_RATED_POWER, (r, m, em) => DeclarationData.GetReferencePropulsionPower(r.VehicleData.InputData).ConvertToKiloWatt() },
			{ EM_RATED_SPEED_HI, (r, m, em) => r.VehicleData.InputData.Components?.ElectricMachines?.Entries.First().ElectricMachine.VoltageLevels.MaxBy(v  => v.VoltageLevel)?.ContinuousTorqueSpeed.AsRPM ?? 0 },
			{ EM_RATED_SPEED_LO, (r, m, em) => r.VehicleData.InputData.Components?.ElectricMachines?.Entries.First().ElectricMachine.VoltageLevels.MinBy(v  => v.VoltageLevel)?.ContinuousTorqueSpeed.AsRPM ?? 0 },
			{ EM_RATED_TORQUE_HI, (r, m, em) => (ConvertedSI)(r.VehicleData.InputData.Components?.ElectricMachines?.Entries.First().ElectricMachine.VoltageLevels.MaxBy(v  => v.VoltageLevel)?.ContinuousTorque ?? 0.SI<NewtonMeter>()) },
			{ EM_RATED_TORQUE_LO, (r, m, em) => (ConvertedSI)(r.VehicleData.InputData.Components?.ElectricMachines?.Entries.First().ElectricMachine.VoltageLevels.MinBy(v  => v.VoltageLevel)?.ContinuousTorque ?? 0.SI<NewtonMeter>()) },
			{ EM_MOTOR_NUMBER, (r, m, em) => r.VehicleData.InputData.Components?.ElectricMachines?.Entries.First().Count ?? 0 },
		};

		public static readonly Dictionary<string, WriteEmEntry> IEPCValue = new Dictionary<string, WriteEmEntry>() {
			{ IEPC_AVG_SPEED_FORMAT, (r, m, em) => m.ElectricMotorAverageSpeed(em).ConvertToRoundsPerMinute() },
			{ E_IEPC_DRIVE_FORMAT, (r, m, em) => m.TotalElectricMotorWorkDrive(em).ConvertToKiloWattHour() },
			{ E_IEPC_GENERATE_FORMAT, (r, m, em) => m.TotalElectricMotorWorkRecuperate(em).ConvertToKiloWattHour() },
			{ ETA_IEPC_DRIVE_FORMAT, (r, m, em) => new ConvertedSI(m.ElectricMotorEfficiencyDrive(em), "") },
			{ ETA_IEPC_GEN_FORMAT, (r, m, em) => new ConvertedSI(m.ElectricMotorEfficiencyGenerate(em), "") },
			{ E_IEPC_OFF_Loss_Format, (r, m, em) => m.ElectricMotorOffLosses(em).ConvertToKiloWattHour() },
			{ E_IEPC_LOSS_FORMAT, (r, m, em) => m.ElectricMotorLosses(em).ConvertToKiloWattHour() },
			{ E_IEPC_OFF_TIME_SHARE, (r, m, em) => (ConvertedSI)m.ElectricMotorOffTimeShare(em) },
			{ EM_RATED_POWER, (r, m, em) => r.VehicleData.InputData.Components?.IEPC?.TotalRatedPowerCalculated.ConvertToKiloWatt() ?? 0.SI<Watt>().ConvertToKiloWatt() },
			{ EM_RATED_SPEED_HI, (r, m, em) => r.VehicleData.InputData.Components?.IEPC?.VoltageLevels.MaxBy(v  => v.VoltageLevel)?.ContinuousTorqueSpeed.AsRPM ?? 0 },
			{ EM_RATED_SPEED_LO, (r, m, em) => r.VehicleData.InputData.Components?.IEPC?.VoltageLevels.MinBy(v  => v.VoltageLevel)?.ContinuousTorqueSpeed.AsRPM ?? 0 },
			{ EM_RATED_TORQUE_HI, (r, m, em) => (ConvertedSI)(r.VehicleData.InputData.Components.IEPC?.VoltageLevels.MaxBy(v  => v.VoltageLevel)?.ContinuousTorque ?? 0.SI<NewtonMeter>()) },
			{ EM_RATED_TORQUE_LO, (r, m, em) => (ConvertedSI)(r.VehicleData.InputData.Components.IEPC?.VoltageLevels.MinBy(v  => v.VoltageLevel)?.ContinuousTorque ?? 0.SI<NewtonMeter>()) },
        };

		public static readonly WriteAuxEntry AuxDataValue = (r, m, a) => m.AuxiliaryWork(a).ConvertToKiloWattHour();


		public static readonly Dictionary<string, Tuple<ModalResultField[], WriteSumEntry>> FuelCellValue =
			new Dictionary<string, Tuple<ModalResultField[], WriteSumEntry>>() {
				{
					FuelCellFields.FCMAP_H,
					SumFunc((r, m) => {
						return m.Data.FuelCellComponentIds.Aggregate(0.SI<KilogramPerSecond>(),
							(a, id) => {
								var singleFc =
									(m.TimeIntegral<Kilogram>(ModalResultField.FC_FCS.Format(id)) / m.Duration) ??
									0.SI<KilogramPerSecond>();
								return a + singleFc;
							}).ConvertToGrammPerHour();
					})
				}, {
					FuelCellFields.FCMAP_KM,
					SumFunc((r, m) => {
						return m.Data.FuelCellComponentIds.Aggregate(0.SI<KilogramPerMeter>(),
							(a, id) => {
								var singleFc =
									(m.TimeIntegral<Kilogram>(ModalResultField.FC_FCS.Format(id)) / m.Distance) ??
									0.SI<KilogramPerMeter>();
								return a + singleFc;
							}).ConvertToGrammPerKiloMeter();
					})
				}, {
					FuelCellFields.FCFINAL_H, SumFunc((r, m) =>
						m.CorrectedModalData.FuelConsumptionCorrection(FuelData.H2).FC_FINAL_H.ConvertToGrammPerHour())
				}, {
					FuelCellFields.FCFINAL_KM, SumFunc((r, m) =>
						m.CorrectedModalData.FuelConsumptionCorrection(FuelData.H2).FC_FINAL_KM
							.ConvertToGrammPerKiloMeter())
				}, {
					FuelCellFields.K_FCSLine, SumFunc((r, m) =>
						m.CorrectedModalData.FuelConsumptionCorrection(FuelData.H2).FuelCellLine
							.ConvertToGramPerKiloWattHour())
				}, {
					FuelCellFields.P_FCS, SumFunc((r, m) => {
						var p_fcs = m.TimeIntegral<WattSecond>(ModalResultField.P_FCSystem) / m.Duration;
						return p_fcs.ConvertToKiloWatt();
					})
				}, {
					FuelCellFields.E_FCS, SumFunc((r, m) => {
						var e_fcs = m.TimeIntegral<WattSecond>(ModalResultField.P_FCSystem);
						return e_fcs.ConvertToKiloWattHour();
					})
				}, {
					FuelCellFields.FC_HEV_SOC_CORR_H, SumFunc((r, m) => m.CorrectedModalData
						.FuelConsumptionCorrection(FuelData.H2).FC_REESS_SOC_CORR_H
						.ConvertToGrammPerHour())
				}, {
					FuelCellFields.FC_HEV_SOC_CORR_KM, SumFunc((r, m) => m.CorrectedModalData
						.FuelConsumptionCorrection(FuelData.H2).FC_REESS_SOC_CORR_KM
						.ConvertToGrammPerKiloMeter())
				}, {
					FuelCellFields.FC_HEV_SOC_H, SumFunc((r, m) => m.CorrectedModalData
						.FuelConsumptionCorrection(FuelData.H2).FC_REESS_SOC_H
						.ConvertToGrammPerHour())
				}, {
					FuelCellFields.FC_HEV_SOC_KM, SumFunc((r, m) => m.CorrectedModalData
						.FuelConsumptionCorrection(FuelData.H2).FC_REESS_SOC_KM
						.ConvertToGrammPerKiloMeter())
				},
			};

		
	}

}