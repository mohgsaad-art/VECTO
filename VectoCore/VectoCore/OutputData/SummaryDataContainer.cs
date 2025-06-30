/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

// ReSharper disable MemberCanBePrivate.Global  -- used by API!

namespace TUGraz.VectoCore.OutputData
{
    //public delegate void WriteSumData(IModalDataContainer data);

    public interface ISumData
	{
		void Write(IModalDataContainer modData, VectoRunData runData);
		void RegisterComponent(VectoSimulationComponent component, VectoRunData runData);
		void AddAuxiliary(string id);
	}

	/// <summary>
	/// Class for the sum file in vecto.
	/// </summary>
	public class SummaryDataContainer : LoggingObject, ISumData, IDisposable
	{
		public static readonly string[] FcColumns = {
			SumDataFields.FCMAP_H, 
			SumDataFields.FCMAP_KM,
			SumDataFields.FCNCVC_H, 
			SumDataFields.FCNCVC_KM,
			SumDataFields.FCWHTCC_H, 
			SumDataFields.FCWHTCC_KM,
			SumDataFields.FCESS_H, 
			SumDataFields.FCESS_KM,
			SumDataFields.FCESS_H_CORR, 
			SumDataFields.FCESS_KM_CORR,
			SumDataFields.FC_BusAux_PS_CORR_H, 
			SumDataFields.FC_BusAux_PS_CORR_KM,
			SumDataFields.FC_BusAux_ES_CORR_H, 
			SumDataFields.FC_BusAux_ES_CORR_KM,
			SumDataFields.FCWHR_H_CORR, 
			SumDataFields.FCWHR_KM_CORR,
			SumDataFields.FC_HEV_SOC_H, 
			SumDataFields.FC_HEV_SOC_KM,
			SumDataFields.FC_HEV_SOC_CORR_H, 
			SumDataFields.FC_HEV_SOC_CORR_KM,
			SumDataFields.FC_AUXHTR_H, 
			SumDataFields.FC_AUXHTR_KM,
			SumDataFields.FC_AUXHTR_H_CORR, 
			SumDataFields.FC_AUXHTR_KM_CORR,
			SumDataFields.FCFINAL_H, 
			SumDataFields.FCFINAL_KM, 
			SumDataFields.FCFINAL_LITERPER100KM,
			SumDataFields.FCFINAL_LITERPER100TKM,
			SumDataFields.FCFINAL_LiterPer100M3KM,
			SumDataFields.FCFINAL_LiterPer100PassengerKM,
			SumDataFields.SPECIFIC_FC,
			SumDataFields.K_VEHLINE,
			SumDataFields.K_ENGLINE
		};

		public static readonly Tuple<string, Type>[] FuelCell_FcColumns = {
			Tuple.Create(SumDataFields.FuelCellFields.FCMAP_H, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FCMAP_KM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_BusAux_PS_CORR_H, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_BusAux_PS_CORR_KM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_BusAux_ES_CORR_H, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_BusAux_ES_CORR_KM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_AUXHTR_H, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_AUXHTR_KM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_AUXHTR_H_CORR, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_AUXHTR_KM_CORR, typeof(ConvertedSI)),

			Tuple.Create(SumDataFields.FuelCellFields.FC_HEV_SOC_CORR_H, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_HEV_SOC_CORR_KM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_HEV_SOC_H, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FC_HEV_SOC_KM, typeof(ConvertedSI)),

			Tuple.Create(SumDataFields.FuelCellFields.K_FCSLine, typeof(ConvertedSI)),
            Tuple.Create(SumDataFields.FuelCellFields.FCFINAL_H, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.FCFINAL_KM, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] FuelCell_Columns = {
			Tuple.Create(SumDataFields.FuelCellFields.P_FCS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.FuelCellFields.E_FCS, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] CommonColumns = {
			Tuple.Create(SumDataFields.SORT, typeof(int)),
			Tuple.Create(SumDataFields.JOB, typeof(string)),
			Tuple.Create(SumDataFields.INPUTFILE, typeof(string)),
			Tuple.Create(SumDataFields.CYCLE, typeof(string)),
			Tuple.Create(SumDataFields.STATUS, typeof(string)),
			Tuple.Create(SumDataFields.CURB_MASS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.LOADING, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.TIME, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.DISTANCE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.SPEED, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ALTITUDE_DELTA, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ACC, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ACC_POS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ACC_NEG, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AVERAGE_POS_ACC, typeof(ConvertedSI)),
			// Engine Infos
			Tuple.Create(SumDataFields.ENGINE_MANUFACTURER, typeof(string)),
			Tuple.Create(SumDataFields.ENGINE_MODEL, typeof(string)),
			Tuple.Create(SumDataFields.ENGINE_FUEL_TYPE, typeof(string)),
			Tuple.Create(SumDataFields.ENGINE_RATED_POWER, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ENGINE_IDLING_SPEED, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ENGINE_RATED_SPEED, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ENGINE_DISPLACEMENT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ENGINE_WHTC_URBAN, typeof(string)),
			Tuple.Create(SumDataFields.ENGINE_WHTC_RURAL, typeof(string)),
			Tuple.Create(SumDataFields.ENGINE_WHTC_MOTORWAY, typeof(string)),
			Tuple.Create(SumDataFields.ENGINE_BF_COLD_HOT, typeof(string)),
			Tuple.Create(SumDataFields.ENGINE_CF_REG_PER, typeof(string)),
			Tuple.Create(SumDataFields.ENGINE_ACTUAL_CORRECTION_FACTOR, typeof(string)),
			Tuple.Create(SumDataFields.VEHICLE_FUEL_TYPE, typeof(string)),
			Tuple.Create(SumDataFields.ENGINE_CERTIFICATION_NUMBER, typeof(string)),
			// Vehicle Infos
			Tuple.Create(SumDataFields.VEHICLE_MANUFACTURER, typeof(string)),
			Tuple.Create(SumDataFields.VIN_NUMBER, typeof(string)),
			Tuple.Create(SumDataFields.VEHICLE_MODEL, typeof(string)),
			Tuple.Create(SumDataFields.HDV_CO2_VEHICLE_CLASS, typeof(string)),

			Tuple.Create(SumDataFields.AIRDRAG_MODEL, typeof(string)),
			Tuple.Create(SumDataFields.AIRDRAG_CERTIFICATION_NUMBER, typeof(string)),
			Tuple.Create(SumDataFields.AIRDRAG_CERTIFICATION_METHOD, typeof(string)),
			Tuple.Create(SumDataFields.CD_x_A_DECLARED, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.CD_x_A, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.SLEEPER_CAB, typeof(string)),
			Tuple.Create(SumDataFields.DECLARED_RRC_AXLE1, typeof(double)),
			Tuple.Create(SumDataFields.DECLARED_FZISO_AXLE1, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.DECLARED_RRC_AXLE2, typeof(double)),
			Tuple.Create(SumDataFields.DECLARED_FZISO_AXLE2, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.DECLARED_RRC_AXLE3, typeof(double)),
			Tuple.Create(SumDataFields.DECLARED_FZISO_AXLE3, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.DECLARED_RRC_AXLE4, typeof(double)),
			Tuple.Create(SumDataFields.DECLARED_FZISO_AXLE4, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER, typeof(double)),
			Tuple.Create(SumDataFields.ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER, typeof(double)),
			Tuple.Create(SumDataFields.R_DYN, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.NUM_AXLES_DRIVEN, typeof(int)),
			Tuple.Create(SumDataFields.NUM_AXLES_NON_DRIVEN, typeof(int)),
			Tuple.Create(SumDataFields.NUM_AXLES_TRAILER, typeof(int)),
			Tuple.Create(SumDataFields.GEARBOX_MANUFACTURER, typeof(string)),
			Tuple.Create(SumDataFields.GEARBOX_MODEL, typeof(string)),
			Tuple.Create(SumDataFields.GEARBOX_CERTIFICATION_METHOD, typeof(string)),
			Tuple.Create(SumDataFields.GEARBOX_CERTIFICATION_NUMBER, typeof(string)),
			Tuple.Create(SumDataFields.GEARBOX_TYPE, typeof(string)),
			Tuple.Create(SumDataFields.TORQUECONVERTER_MANUFACTURER, typeof(string)),
			Tuple.Create(SumDataFields.TORQUECONVERTER_MODEL, typeof(string)),
			Tuple.Create(SumDataFields.TORQUE_CONVERTER_CERTIFICATION_METHOD, typeof(string)),
			Tuple.Create(SumDataFields.TORQUE_CONVERTER_CERTIFICATION_NUMBER, typeof(string)),
			Tuple.Create(SumDataFields.RETARDER_MANUFACTURER, typeof(string)),
			Tuple.Create(SumDataFields.RETARDER_MODEL, typeof(string)),
			Tuple.Create(SumDataFields.RETARDER_TYPE, typeof(string)),
			Tuple.Create(SumDataFields.RETARDER_CERTIFICATION_METHOD, typeof(string)),
			Tuple.Create(SumDataFields.RETARDER_CERTIFICATION_NUMBER, typeof(string)),
			Tuple.Create(SumDataFields.ANGLEDRIVE_MANUFACTURER, typeof(string)),
			Tuple.Create(SumDataFields.ANGLEDRIVE_MODEL, typeof(string)),
			Tuple.Create(SumDataFields.ANGLEDRIVE_RATIO, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ANGLEDRIVE_CERTIFICATION_METHOD, typeof(string)),
			Tuple.Create(SumDataFields.ANGLEDRIVE_CERTIFICATION_NUMBER, typeof(string)),
			Tuple.Create(SumDataFields.AXLE_MANUFACTURER, typeof(string)),
			Tuple.Create(SumDataFields.AXLE_MODEL, typeof(string)),
			Tuple.Create(SumDataFields.AXLE_RATIO, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AXLEGEAR_CERTIFICATION_METHOD, typeof(string)),
			Tuple.Create(SumDataFields.AXLEGEAR_CERTIFICATION_NUMBER, typeof(string)),
			Tuple.Create(string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.SteeringPump),
				typeof(string)),
			Tuple.Create(string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.Fan), typeof(string)),
			Tuple.Create(
				string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.HeatingVentilationAirCondition),
				typeof(string)),
			Tuple.Create(string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.PneumaticSystem),
				typeof(string)),
			Tuple.Create(string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.ElectricSystem),
				typeof(string)),
			Tuple.Create(SumDataFields.E_AUX, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.TCU_MODEL, typeof(string)),
			Tuple.Create(SumDataFields.ADAS_TECHNOLOGY_COMBINATION, typeof(string)),
			Tuple.Create(SumDataFields.PTO_TECHNOLOGY, typeof(string)),
			Tuple.Create(SumDataFields.REESS_CAPACITY, typeof(string)),
			Tuple.Create(SumDataFields.CARGO_VOLUME, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.COASTING_TIME_SHARE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.BRAKING_TIME_SHARE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ACC_TIMESHARE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.DEC_TIMESHARE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.CRUISE_TIMESHARE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.STOP_TIMESHARE, typeof(ConvertedSI)),

		};

		public static readonly Tuple<string, Type>[] CombustionEngineColumns = {
			Tuple.Create(SumDataFields.ICE_FULL_LOAD_TIME_SHARE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ICE_OFF_TIME_SHARE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.NUM_ICE_STARTS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.P_FCMAP, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.P_FCMAP_POS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_FCMAP_POS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_FCMAP_NEG, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.MAX_ENGINE_SPEED, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AVG_ENGINE_SPEED, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_WHR_EL, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_WHR_MECH, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_ICE_START, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_AUX_ESS_missing, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AVERAGE_ENGINE_EFFICIENCY, typeof(double)),
		};

		public static readonly Tuple<string, Type>[] OVCModeColumns = {
			Tuple.Create(SumDataFields.OVCHEVMode, typeof(string))
		};

		public static readonly Tuple<string, Type>[] PHEVColumns = {
			Tuple.Create(SumDataFields.f_equiv, typeof(double))
		};

		public static readonly Tuple<string, Type>[] VehilceColumns = {
			Tuple.Create(SumDataFields.E_VEHICLE_INERTIA, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_AIR, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_ROLL, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_GRAD, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_POWERTRAIN_INERTIA, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.MAX_ACCELERATION, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.MAX_DECELERATION, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.MAX_SPEED, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.TOTAL_VEHICLE_MASS, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] BusVehicleColumns = {
			Tuple.Create(SumDataFields.PassengerCount, typeof(double)),
		};

		

		public static readonly Tuple<string, Type>[] ClutchColumns = {
			Tuple.Create(SumDataFields.E_CLUTCH_LOSS, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] GearboxColumns = {
			Tuple.Create(SumDataFields.NUM_GEARSHIFTS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_GBX_LOSS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AVERAGE_GEARBOX_EFFICIENCY, typeof(double)),
		};
		public static readonly Tuple<string, Type>[] GearboxColumns_AT = {
			Tuple.Create(SumDataFields.NUM_GEARSHIFTS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_GBX_LOSS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_SHIFT_LOSS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AVERAGE_GEARBOX_EFFICIENCY, typeof(double)),
		};

		public static readonly Tuple<string, Type>[] IEPCTransmissionColumns = { };

		public static readonly Tuple<string, Type>[] TorqueConverterColumns = {
			Tuple.Create(SumDataFields.E_TC_LOSS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITHOUT_LOCKUP, typeof(double)),
			Tuple.Create(SumDataFields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITH_LOCKUP, typeof(double)),
		};

		public static readonly Tuple<string, Type>[] AngledriveColumns = {
			Tuple.Create(SumDataFields.E_ANGLE_LOSS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AVERAGE_ANGLEDRIVE_EFFICIENCY, typeof(double)),
		};

		public static readonly Tuple<string, Type>[] AxlegearColumns = {
			Tuple.Create(SumDataFields.E_AXL_LOSS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AVERAGE_AXLEGEAR_EFFICIENCY, typeof(double)),
		};

		public static readonly Tuple<string, Type>[] RetarderColumns = {
			Tuple.Create(SumDataFields.E_RET_LOSS, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] WheelColumns = {
			Tuple.Create(SumDataFields.P_WHEEL, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.P_WHEEL_POS, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] WheelEndColumns = {
			Tuple.Create(SumDataFields.E_WHEELEND_SAVED, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] BrakeColumns = {
			Tuple.Create(SumDataFields.E_BRAKE, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] BatteryColumns = {
			Tuple.Create(SumDataFields.REESS_StartSoC, typeof(double)),
			Tuple.Create(SumDataFields.REESS_EndSoC, typeof(double)),
			Tuple.Create(SumDataFields.REESS_DeltaEnergy, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.REESS_MinSoC, typeof(double)),
			Tuple.Create(SumDataFields.REESS_MaxSoC, typeof(double)),

			Tuple.Create(SumDataFields.E_REESS_LOSS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_REESS_T_chg, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_REESS_T_dischg, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_REESS_int_chg, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_REESS_int_dischg, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] ElectricMotorColumns = {
			Tuple.Create(SumDataFields.EM_AVG_SPEED_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_EM_Mot_DRIVE_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_EM_Mot_GENERATE_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ETA_EM_Mot_DRIVE_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ETA_EM_Mot_GEN_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_EM_DRIVE_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_EM_GENERATE_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ETA_EM_DRIVE_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ETA_EM_GEN_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_EM_OFF_Loss_Format, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_EM_LOSS_TRANSM_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_EM_Mot_LOSS_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_EM_LOSS_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_EM_OFF_TIME_SHARE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EM_RATED_POWER, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EM_RATED_SPEED_HI, typeof(int)),
			Tuple.Create(SumDataFields.EM_RATED_SPEED_LO, typeof(int)),
			Tuple.Create(SumDataFields.EM_RATED_TORQUE_HI, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EM_RATED_TORQUE_LO, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EM_MOTOR_NUMBER, typeof(int)),
		};

		public static readonly Tuple<string, Type>[] IEPCColumns = {
			Tuple.Create(SumDataFields.IEPC_AVG_SPEED_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_IEPC_DRIVE_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_IEPC_GENERATE_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ETA_IEPC_DRIVE_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ETA_IEPC_GEN_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_IEPC_OFF_Loss_Format, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_IEPC_LOSS_FORMAT, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_IEPC_OFF_TIME_SHARE, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EM_RATED_POWER, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EM_RATED_SPEED_HI, typeof(int)),
			Tuple.Create(SumDataFields.EM_RATED_SPEED_LO, typeof(int)),
			Tuple.Create(SumDataFields.EM_RATED_TORQUE_HI, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EM_RATED_TORQUE_LO, typeof(ConvertedSI)),
        };

		public static readonly Tuple<string, Type>[] CO2Columns = {
			Tuple.Create(SumDataFields.CO2_KM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.CO2_TKM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.CO2_M3KM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.CO2_PKM, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] ElectricEnergyConsumption = {
			Tuple.Create(SumDataFields.EC_el_SOC, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EC_el_SOC_corr, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EC_el_final, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EC_el_final_KM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EC_el_final_TKM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.EC_el_final_M3KM, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.ElectricEnergyConsumption_PKM, typeof(ConvertedSI)),
		};


		public static readonly Tuple<string, Type>[] VTPCycleColumns = {
			Tuple.Create(SumDataFields.E_WHEEL, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] BusAuxiliariesSignals = {
			Tuple.Create(SumDataFields.AirGenerated, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.AirConsumed, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_BusAux_AuxHeater, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_BusAux_ES_consumed, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_BusAux_ES_generated, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_BusAux_ES_mech_corr, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_BusAux_HVAC_El, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_BusAux_HVAC_Mech, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_BusAux_PS_corr, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_BusAux_el_PS_corr, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_PS_CompressorOff, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_PS_CompressorOn, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.Delta_E_BusAux_Battery, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] PWheelCycleColumns = {
			Tuple.Create(SumDataFields.P_WHEEL, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.P_WHEEL_POS, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.TOTAL_VEHICLE_MASS, typeof(ConvertedSI)),
		};

		public static readonly Tuple<string, Type>[] ElectricAuxiliariesSignals = {
			Tuple.Create(SumDataFields.E_AUX_EL, typeof(ConvertedSI)),
			Tuple.Create(SumDataFields.E_AUX_EL_HV, typeof(ConvertedSI)),
		};

		private object _tableLock = new object();
		internal readonly DataTable Table;
		private readonly ISummaryWriter _sumWriter;
		
		protected IList<string> FcCols = new List<string>();
		protected IList<string> GearColumns = new List<string>();
		protected IList<string> AuxColumns = new List<string>();
		protected IList<string> EmColumns = new List<string>();
		protected IList<string> GearRatioColumns = new List<string>();

		/// <summary>
		/// Initializes a new instance of the <see cref="SummaryDataContainer"/> class.
		/// </summary>
		/// <param name="writer"></param>
		public SummaryDataContainer(ISummaryWriter writer)
		{
			_sumWriter = writer;
			Table = new DataTable();
			//InitTableColumns();
			CreateColumns(CommonColumns);
			
			
		}

		public void RegisterComponent(VectoSimulationComponent component, VectoRunData runData)
		{
			switch (component) {
				case ICombustionEngine _:
					CreateColumns(CombustionEngineColumns);
					UpdateTableColumns(runData.EngineData);
					CreateColumns(CO2Columns);
					break;
				case IBusAuxiliariesAdapter _:
					CreateColumns(BusAuxiliariesSignals);
					break;
				case IEngineAuxiliary _:
					break;
				case IClutch _:
					CreateColumns(ClutchColumns);
					break;
				case IGearbox _ when runData.JobType != VectoSimulationJobType.IEPC_E && runData.JobType != VectoSimulationJobType.IEPC_S:
					CreateColumns(runData.GearboxData.Type.IsOneOf(GearboxType.ATPowerSplit, GearboxType.ATSerial)
						? GearboxColumns_AT
						: GearboxColumns);
					CreateGearTimeShareColumns(runData.GearboxData.GearList);
					CreateGearRatioColumns(runData);
					break;
				case IGearbox _ when runData.JobType == VectoSimulationJobType.IEPC_E:
				case IGearbox _ when runData.JobType == VectoSimulationJobType.IEPC_S:
					CreateColumns(IEPCTransmissionColumns);
					CreateGearTimeShareColumns(runData.GearboxData.GearList);
					CreateGearRatioColumns(runData);
					break;
				case IVTPCycle _:
					CreateColumns(VTPCycleColumns);
					break;
				case ITorqueConverter _:
					CreateColumns(TorqueConverterColumns);
					break;
				case IAngledrive _:
					CreateColumns(AngledriveColumns);
					break;
				case IAxlegear _:
					CreateColumns(AxlegearColumns);
					break;
				case Retarder _:
					CreateColumns(RetarderColumns);
					break;
				case IWheels _:
					CreateColumns(WheelColumns);
					break;
				case IBrakes _:
					CreateColumns(BrakeColumns);
					break;
				case WheelEnd _:
					CreateColumns(WheelEndColumns);
					break;
				case IDriver _:
					//CreateColumns(DriverSignals);
					break;
				case PWheelCycle _:
					CreateColumns(PWheelCycleColumns);
					break;
				case IVehicle _:
					CreateColumns(VehilceColumns);
					if (runData.VehicleData.VehicleCategory.IsBus()) {
						CreateColumns(BusVehicleColumns);
					}

					if (runData.JobType == VectoSimulationJobType.ParallelHybridVehicle ||
						runData.JobType == VectoSimulationJobType.IHPC) {
						CreateColumns(PHEVColumns);
					}

					if (runData.OVCMode != OvcHevMode.NotApplicable) {
						CreateColumns(OVCModeColumns);
					}
					break;
				case IElectricMotor c3 when c3.Position == PowertrainPosition.IEPC:
					CreateElectricMotorColumns(c3, runData, IEPCColumns);
					break;
				case IElectricMotor c4 when c4.Position != PowertrainPosition.IEPC:
					CreateElectricMotorColumns(c4, runData, ElectricMotorColumns);
					break;
				case IElectricEnergyStorage c5 when c5 is BatterySystem:
					CreateColumns(ElectricEnergyConsumption.Where(x => runData.VehicleData.VehicleCategory.IsBus() || x.Item1 != SumDataFields.ElectricEnergyConsumption_PKM).ToArray());
					CreateColumns(BatteryColumns);
					break;
				case IElectricEnergyStorage _:
					CreateColumns(ElectricEnergyConsumption.Where(x => runData.VehicleData.VehicleCategory.IsBus() || x.Item1 != SumDataFields.ElectricEnergyConsumption_PKM).ToArray());
					CreateColumns(BatteryColumns);
					break;
				case IElectricSystem _:
					//CreateColumns(ElectricSystemSignals);
					break;
				case IHybridController _:
					//CreateColumns(HybridControllerSignals);
					break;
				case IDCDCConverter _:
				//CreateColumns(DCDCConverterSignals);
				case ElectricAuxiliaries _:
					CreateColumns(ElectricAuxiliariesSignals);
					break;
				case FuelCellSystem _:
					CreateColumns(FuelCell_Columns);
					CreateColumns(FuelCell_FcColumns);
					break;
			}
		}

		public void AddAuxiliary(string id)
		{
			lock (Table) {
				var colName = GetAuxColName(id);
				if (Table.Columns.Contains(colName)) {
					return;
				}
				Table.Columns.Add(colName, typeof(ConvertedSI));
				AuxColumns.Add(colName);
			}
		}

		private void CreateElectricMotorColumns(IElectricMotor em, VectoRunData runData, Tuple<string, Type>[] cols)
		{
			lock (Table) {
				var emColNames = cols.Select(x => string.Format(x.Item1, em.Position.GetName()));
				Table.Columns.AddRange(cols
					.Select(x => Tuple.Create(string.Format(x.Item1, em.Position.GetName()), x.Item2))
					.Where(x => !Table.Columns.Contains(x.Item1)).Select(x => new DataColumn(x.Item1, x.Item2))
					.ToArray());
				foreach (var emColName in emColNames) {
					if(EmColumns.Contains(emColName)) {
						continue;
					}
					EmColumns.Add(emColName);
				}
			}
		}

		private void CreateGearTimeShareColumns(GearList gears)
		{
			
			lock (Table) {
				var gearNumbers = new uint[] { 0 }.Concat(gears.Select(x => x.Gear)).Distinct().OrderBy(x => x);
				
				var gearColNames = gearNumbers
					.Select(x => string.Format(SumDataFields.TIME_SHARE_PER_GEAR_FORMAT, x))
					.Where(x => !Table.Columns.Contains(x)).Select(x => new DataColumn(x, typeof(ConvertedSI)))
					.ToArray();
				Table.Columns.AddRange(gearColNames);
				foreach (var gearColName in gearColNames) {
					GearColumns.Add(gearColName.ColumnName);
				}
//				gearColNames.Select(x => GearColumns.Add(x.ColumnName));
			}
		}

		private void CreateGearRatioColumns(VectoRunData runData)
		{
			lock (Table) {
				var gears = runData.GearboxData.GearList;
				var gearNumbers = new uint[] { }.Concat(gears.Select(x => x.Gear)).Distinct().OrderBy(x => x);
				
				var gearColNames = gearNumbers
					.Select(x => string.Format(SumDataFields.RATIO_PER_GEAR_FORMAT, x))
					.Where(x => !Table.Columns.Contains(x)).Select(x => new DataColumn(x, typeof(ConvertedSI)));
					
				if (runData.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP2_5)) {
					gearColNames = gearColNames.Concat(
						gearNumbers
							.Select(x => string.Format(SumDataFields.P2_5_RATIO_PER_GEAR_FORMAT, x))
							.Where(x => !Table.Columns.Contains(x)).Select(x => new DataColumn(x, typeof(ConvertedSI))));
				}

				var gearColumnsArray = gearColNames.ToArray();

				Table.Columns.AddRange(gearColumnsArray);
				
				foreach (var gearColName in gearColumnsArray) {
					GearRatioColumns.Add(gearColName.ColumnName);
				}
			}
		}

		protected internal void CreateColumns(Tuple<string, Type>[] cols)
		{
			lock (Table) {
				Table.Columns.AddRange(cols.Where(x => !Table.Columns.Contains(x.Item1))
					.Select(x => new DataColumn(x.Item1, x.Item2)).ToArray());
			}
		}

		private IList<string> GetOutputColumnsOrdered()
		{
			var cols = new List<string>();
			cols.AddRange(new[] {
				SumDataFields.JOB,
				SumDataFields.INPUTFILE,
				SumDataFields.CYCLE,
				SumDataFields.OVCHEVMode,
				SumDataFields.STATUS,
				SumDataFields.VEHICLE_MANUFACTURER,
				SumDataFields.VIN_NUMBER,
				SumDataFields.VEHICLE_MODEL,
				SumDataFields.HDV_CO2_VEHICLE_CLASS,
				SumDataFields.CURB_MASS,
				SumDataFields.LOADING,
				SumDataFields.PassengerCount,
				SumDataFields.TOTAL_VEHICLE_MASS,

				SumDataFields.ENGINE_MANUFACTURER,
				SumDataFields.ENGINE_MODEL,
				SumDataFields.ENGINE_FUEL_TYPE,
				SumDataFields.ENGINE_RATED_POWER,
				SumDataFields.ENGINE_IDLING_SPEED,
				SumDataFields.ENGINE_RATED_SPEED,
				SumDataFields.ENGINE_DISPLACEMENT,
				SumDataFields.ENGINE_WHTC_URBAN,
				SumDataFields.ENGINE_WHTC_RURAL,
				SumDataFields.ENGINE_WHTC_MOTORWAY,
				SumDataFields.ENGINE_BF_COLD_HOT,
				SumDataFields.ENGINE_CF_REG_PER,
				SumDataFields.ENGINE_ACTUAL_CORRECTION_FACTOR,
				SumDataFields.VEHICLE_FUEL_TYPE,

				SumDataFields.AIRDRAG_MODEL,
				SumDataFields.CD_x_A_DECLARED,
				SumDataFields.CD_x_A,
				SumDataFields.SLEEPER_CAB,
				SumDataFields.DECLARED_RRC_AXLE1,
				SumDataFields.DECLARED_FZISO_AXLE1,
				SumDataFields.DECLARED_RRC_AXLE2,
				SumDataFields.DECLARED_FZISO_AXLE2,
				SumDataFields.DECLARED_RRC_AXLE3,
				SumDataFields.DECLARED_FZISO_AXLE3,
				SumDataFields.DECLARED_RRC_AXLE4,
				SumDataFields.DECLARED_FZISO_AXLE4,
				SumDataFields.ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER,
				SumDataFields.ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER,
				SumDataFields.R_DYN,
				SumDataFields.NUM_AXLES_DRIVEN,
				SumDataFields.NUM_AXLES_NON_DRIVEN,
				SumDataFields.NUM_AXLES_TRAILER,
				SumDataFields.GEARBOX_MANUFACTURER,
				SumDataFields.GEARBOX_MODEL,
				SumDataFields.GEARBOX_TYPE
			});

			cols.AddRange(GearRatioColumns);

			cols.AddRange(new[] {
				SumDataFields.TORQUECONVERTER_MANUFACTURER,
				SumDataFields.TORQUECONVERTER_MODEL,
				SumDataFields.RETARDER_MANUFACTURER,
				SumDataFields.RETARDER_MODEL,
				SumDataFields.RETARDER_TYPE,
				SumDataFields.ANGLEDRIVE_MANUFACTURER,
				SumDataFields.ANGLEDRIVE_MODEL,
				SumDataFields.ANGLEDRIVE_RATIO,
				SumDataFields.AXLE_MANUFACTURER,
				SumDataFields.AXLE_MODEL,
				SumDataFields.AXLE_RATIO
			});

            cols.AddRange(new[] {
                Constants.Auxiliaries.IDs.SteeringPump, 
				Constants.Auxiliaries.IDs.Fan,
                Constants.Auxiliaries.IDs.HeatingVentilationAirCondition,
                Constants.Auxiliaries.IDs.PneumaticSystem,
				Constants.Auxiliaries.IDs.ElectricSystem
            }.Select(x => string.Format(SumDataFields.AUX_TECH_FORMAT, x)));

			cols.AddRange(new[] {
				SumDataFields.TCU_MODEL,
				SumDataFields.ADAS_TECHNOLOGY_COMBINATION,
				SumDataFields.PTO_TECHNOLOGY,
				SumDataFields.REESS_CAPACITY,
				SumDataFields.CARGO_VOLUME,
				SumDataFields.TIME,
				SumDataFields.DISTANCE,
				SumDataFields.SPEED,
				SumDataFields.ALTITUDE_DELTA,
			});

			cols.AddRange(FcCols.Reverse());

			cols.AddRange(FuelCell_FcColumns.Select(x => x.Item1));
			cols.AddRange(FuelCell_Columns.Select(x => x.Item1));

			cols.AddRange(new[] {
				SumDataFields.CO2_KM,
				SumDataFields.CO2_TKM,
				SumDataFields.CO2_M3KM,
				SumDataFields.CO2_PKM,
				SumDataFields.P_WHEEL,
				SumDataFields.P_WHEEL_POS,
				SumDataFields.P_FCMAP,
				SumDataFields.P_FCMAP_POS,
				SumDataFields.E_FCMAP_POS,
				SumDataFields.E_FCMAP_NEG,
				SumDataFields.E_POWERTRAIN_INERTIA,
			});

			cols.AddRange(new [] {
				SumDataFields.EC_el_SOC,
				SumDataFields.EC_el_SOC_corr,
				SumDataFields.EC_el_final,
				SumDataFields.EC_el_final_KM,
				SumDataFields.EC_el_final_TKM,
				SumDataFields.EC_el_final_M3KM,
				SumDataFields.ElectricEnergyConsumption_PKM,
			});
			cols.AddRange( new [] {
				SumDataFields.f_equiv,
			});

			cols.AddRange(EmColumns);




			cols.AddRange(new[] {
				SumDataFields.REESS_StartSoC,
				SumDataFields.REESS_EndSoC,
				SumDataFields.REESS_DeltaEnergy,
				SumDataFields.REESS_MinSoC,
				SumDataFields.REESS_MaxSoC,
				SumDataFields.E_REESS_LOSS,
				SumDataFields.E_REESS_T_chg,
				SumDataFields.E_REESS_T_dischg,
				SumDataFields.E_REESS_int_chg,
				SumDataFields.E_REESS_int_dischg,
			});
			cols.AddRange(AuxColumns.OrderBy(x => x));
			cols.AddRange(new[] {
				SumDataFields.E_AUX,
				SumDataFields.E_AUX_EL,
				SumDataFields.E_AUX_EL_HV, 
				SumDataFields.E_CLUTCH_LOSS,
				SumDataFields.E_TC_LOSS, 
				SumDataFields.E_SHIFT_LOSS, 
				SumDataFields.E_GBX_LOSS, 
				SumDataFields.E_RET_LOSS,
				SumDataFields.E_ANGLE_LOSS,
				SumDataFields.E_AXL_LOSS, 
				SumDataFields.E_WHEELEND_SAVED,
				SumDataFields.E_BRAKE, 
				SumDataFields.E_VEHICLE_INERTIA, 
				SumDataFields.E_WHEEL, 
				SumDataFields.E_AIR,
				SumDataFields.E_ROLL, 
				SumDataFields.E_GRAD,
				SumDataFields.AirConsumed, 
				SumDataFields.AirGenerated, 
				SumDataFields.E_PS_CompressorOff, 
				SumDataFields.E_PS_CompressorOn,
				SumDataFields.E_BusAux_ES_consumed, 
				SumDataFields.E_BusAux_ES_generated, 
				SumDataFields.Delta_E_BusAux_Battery,
				SumDataFields.E_BusAux_PS_corr,
				SumDataFields.E_BusAux_el_PS_corr,
				SumDataFields.E_BusAux_ES_mech_corr,
				SumDataFields.E_BusAux_HVAC_Mech, 
				SumDataFields.E_BusAux_HVAC_El,
				SumDataFields.E_BusAux_AuxHeater,
				SumDataFields.E_WHR_EL, 
				SumDataFields.E_WHR_MECH, 
				SumDataFields.E_ICE_START, 
				SumDataFields.E_AUX_ESS_missing,
				SumDataFields.NUM_ICE_STARTS, 
				SumDataFields.ACC,
				SumDataFields.ACC_POS, 
				SumDataFields.ACC_NEG, 
				SumDataFields.ACC_TIMESHARE, 
				SumDataFields.DEC_TIMESHARE,
				SumDataFields.CRUISE_TIMESHARE,
				SumDataFields.MAX_SPEED, 
				SumDataFields.MAX_ACCELERATION, 
				SumDataFields.MAX_DECELERATION, 
				SumDataFields.AVG_ENGINE_SPEED,
				SumDataFields.MAX_ENGINE_SPEED, 
				SumDataFields.NUM_GEARSHIFTS, 
				SumDataFields.STOP_TIMESHARE,
				SumDataFields.ICE_FULL_LOAD_TIME_SHARE, 
				SumDataFields.ICE_OFF_TIME_SHARE,
				SumDataFields.COASTING_TIME_SHARE, 
				SumDataFields.BRAKING_TIME_SHARE, 
				SumDataFields.AVERAGE_POS_ACC,

				SumDataFields.ENGINE_CERTIFICATION_NUMBER,
				SumDataFields.AVERAGE_ENGINE_EFFICIENCY,
				SumDataFields.TORQUE_CONVERTER_CERTIFICATION_METHOD, 
				SumDataFields.TORQUE_CONVERTER_CERTIFICATION_NUMBER, 
				SumDataFields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITHOUT_LOCKUP,
				SumDataFields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITH_LOCKUP, 
				SumDataFields.GEARBOX_CERTIFICATION_METHOD, 
				SumDataFields.GEARBOX_CERTIFICATION_NUMBER, 
				SumDataFields.AVERAGE_GEARBOX_EFFICIENCY, 
				SumDataFields.RETARDER_CERTIFICATION_METHOD,
				SumDataFields.RETARDER_CERTIFICATION_NUMBER, 
				SumDataFields.ANGLEDRIVE_CERTIFICATION_METHOD, 
				SumDataFields.ANGLEDRIVE_CERTIFICATION_NUMBER,
				SumDataFields.AVERAGE_ANGLEDRIVE_EFFICIENCY,
				SumDataFields.AXLEGEAR_CERTIFICATION_METHOD, 
				SumDataFields.AXLEGEAR_CERTIFICATION_NUMBER, 
				SumDataFields.AVERAGE_AXLEGEAR_EFFICIENCY, 
				SumDataFields.AIRDRAG_CERTIFICATION_NUMBER,
				SumDataFields.AIRDRAG_CERTIFICATION_METHOD, 
			});
			cols.AddRange(GearColumns);

			return cols.Where(x => Table.Columns.Contains(x)).ToArray();
		}

		public ISummaryWriter SummaryWriter { get { return _sumWriter; } }

		/// <summary>
		/// Finishes the summary data container (writes the data to the sumWriter).
		/// </summary>
		public virtual void Finish()
		{
			if (_sumWriter != null) {
				lock (Table) {
					AddFinalCSResult();
					var outputColumns = GetOutputColumnsOrdered().Distinct().ToArray();
					var view = new DataView(Table, "", SumDataFields.SORT, DataViewRowState.CurrentRows).ToTable(false, outputColumns);
					
					try {
						_sumWriter.WriteSumData(view);
					} catch (Exception e) {
						LogManager.GetLogger(typeof(SummaryDataContainer).FullName).Error(e.Message);
					}
				}
			}
		}

		private void AddFinalCSResult()
		{
			lock (Table) {
				if (!Table.Columns.Contains(SumDataFields.OVCHEVMode) || !Table.AsEnumerable().Any(r =>
						r.Field<string>(SumDataFields.OVCHEVMode) == OvcHevMode.ChargeSustaining.ToString())) {
					return;
				}

				var bestCSResult = Table.AsEnumerable().GroupBy(r => Tuple.Create(
						r.Field<string>(SumDataFields.JOB).Split('-').First(),
						r.Field<string>(SumDataFields.CYCLE),
						r.Field<ConvertedSI>(SumDataFields.LOADING),
						r.Field<string>(SumDataFields.HDV_CO2_VEHICLE_CLASS),
						r.Field<string>(SumDataFields.OVCHEVMode)),
					(grp, rows) => {
						if (rows.Count() == 1) {
							return null; // it is the only result - no need to duplicate
						}
						var myRows = rows.ToList();
						var bestResult = DeclarationData.GetSumDataFinalResultEntryIndex(myRows);
						if (bestResult < 0 || bestResult >= myRows.Count) {
							throw new VectoException(
								$"Invalid index for best result entry. got {bestResult}, max. {myRows.Count}");
						}

						return myRows[bestResult];
					}).ToList();
				// copy row with best results (for OVC-CS) at the end of sum data
				foreach (var row in bestCSResult) {
					if (row == null || row.Field<string>(SumDataFields.OVCHEVMode) != OvcHevMode.ChargeSustaining.ToString()) {
						continue;
					}

					var jobNbrs = row.Field<string>(SumDataFields.JOB).Split('-');

					var newRow = Table.Rows.Add(row.ItemArray);
					if (jobNbrs.Length < 2) {
						throw new VectoException(
							"error writing sum data - expected at least two parts in JobNumber field");
					}

					newRow[SumDataFields.JOB] =
						SumDataFields.GetSumDataJobID(jobNbrs[0].ToInt(), jobNbrs[1].ToInt(), -1);
					newRow[SumDataFields.SORT] =
						SumDataFields.GetSumDataSortingValue(jobNbrs[0].ToInt(), jobNbrs[1].ToInt(), -1);
				}
			}
		}

		protected internal void UpdateTableColumns(CombustionEngineData engineData)
		{
			foreach (var entry in engineData.Fuels) {
				foreach (var column in FcColumns.Reverse()) {
					var colName = string.Format(column,
						engineData.Fuels.Count <= 1 && !engineData.MultipleEngineFuelModes
							? ""
							: "_" + entry.FuelData.FuelType.GetLabel());
					lock (Table) {
						if (!Table.Columns.Contains(colName)) {
							var col = new DataColumn(colName, typeof(ConvertedSI));
							Table.Columns.Add(col);
							col.SetOrdinal(Table.Columns.IndexOf(SumDataFields.ALTITUDE_DELTA) + 1);
							FcCols.Add(colName);
							
						}
					}
				}
			}
			
		}

		protected Dictionary<string, object> GetResultDictionary(IModalDataContainer modData, VectoRunData runData)
		{
			return new Dictionary<string, object>();
		}

		private void AddResultDictionary(Dictionary<string, object> row)
		{
			lock (_tableLock) {
				var tableRow = Table.NewRow();
				foreach (var keyValuePair in row) {
					tableRow[keyValuePair.Key] = keyValuePair.Value;
				}

				Table.Rows.Add(tableRow);
			}
		}

		public virtual void Write(IModalDataContainer modData, VectoRunData runData)
		{
			var row = GetResultDictionary(modData, runData);

			foreach (DataColumn col in Table.Columns) {
				SetValue(modData, runData, col, row, SumDataFields.SumDataValue.GetVECTOValueOrDefault(col.ColumnName));
			}

			var multipleEngineModes = runData.EngineData?.MultipleEngineFuelModes ?? false;
			foreach (var fuel in modData.FuelData) {
				var suffix = modData.FuelData.Count <= 1 && !multipleEngineModes ? "" : "_" + fuel.FuelType.GetLabel();
				foreach (var tuple in SumDataFields.FuelDataValue) {
					if (tuple.Value.Item1 == null || tuple.Value.Item1.All(x => modData.ContainsColumn(x.GetName()))) {
						var value = tuple.Value.Item2(runData, modData, fuel);
						if (value != null) {
							row[FcCol(tuple.Key, suffix)] = value;
						}
					}
				}
			}

			foreach (var em in runData.ElectricMachinesData) {
				var fields = em.Item1 == PowertrainPosition.IEPC
					? SumDataFields.IEPCValue
					: SumDataFields.ElectricMotorValue;
				foreach (var entry in fields) {
					var value = entry.Value(runData, modData, em.Item1);
					row[string.Format(entry.Key, em.Item1.GetName())] = value;
				}
			}

			foreach (var aux in modData.Auxiliaries) {
				var colName = GetAuxColName(aux.Key);
				row[colName] = SumDataFields.AuxDataValue(runData, modData, aux.Value);
				var auxTechCol = string.Format(SumDataFields.AUX_TECH_FORMAT, aux.Key);
				if (Table.Columns.Contains(auxTechCol)) {
					row[auxTechCol] = runData.Aux
						.FirstOrDefault(x => x.ID.Equals(aux.Key, StringComparison.InvariantCultureIgnoreCase))?
						.Technology.Join("; ");
				}
			}

			if (runData.JobType.IsOneOf(VectoSimulationJobType.FCHV, VectoSimulationJobType.FCHV_IEPC) 
				&& runData.FuelCellSystemData?.FuelCellStrings?.Count > 0) {

				foreach (DataColumn col in Table.Columns) {
					SetValue(modData, runData, col, row, SumDataFields.FuelCellValue.GetVECTOValueOrDefault(col.ColumnName));
                }
			}

			if ((runData.GearboxData?.Gears.Count ?? 0) > 0) {
				WriteGearshiftStats(modData, row, (uint?)runData.GearboxData?.Gears.Count ?? 0u);
				WriteGearRatios(row, runData);
			}

			AddResultDictionary(row);
		}

		private static void SetValue(IModalDataContainer modData, VectoRunData runData, DataColumn col, Dictionary<string, object> row, Tuple<ModalResultField[], SumDataFields.WriteSumEntry> ValueFunc)
		{
			var func = ValueFunc;
			if (func == null) {
				return;
			}

			if (func.Item1 == null || func.Item1.All(x => modData.ContainsColumn(x.GetName()))) {
				var value = func.Item2(runData, modData);
				if (value != null) {
					row[col.ColumnName] = value;
				}
			}
		}

		private string GetAuxColName(string auxKey)
		{
			return string.Format(
				auxKey.IsOneOf(new[]
					{ Constants.Auxiliaries.IDs.PTOConsumer, Constants.Auxiliaries.IDs.PTOTransmission })
					? SumDataFields.E_FORMAT
					: SumDataFields.E_AUX_FORMAT, auxKey);
		}

		private static string FcCol(string col, string suffix)
		{
			return string.Format(col, suffix);
		}

		private void WriteGearshiftStats(IModalDataContainer modData, Dictionary<string, object> row, uint gearCount)
		{
			//row[SumDataFields.NUM_GEARSHIFTS] = gearCount == 1 ? 0.SI<Scalar>() : (ConvertedSI)modData.GearshiftCount();
			var timeSharePerGear = modData.TimeSharePerGear(gearCount);

			for (uint i = 0; i <= gearCount; i++) {
				var colName = string.Format(SumDataFields.TIME_SHARE_PER_GEAR_FORMAT, i);
				row[colName] = (ConvertedSI)timeSharePerGear[i];
			}
		}

		private void WriteGearRatios(Dictionary<string, object> row, VectoRunData runData)
		{ 
			var emData = runData.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP2_5)
				? runData.ElectricMachinesData.First(x => x.Item1 == PowertrainPosition.HybridP2_5).Item2
				: null;

			foreach (var gear in runData.GearboxData.Gears) {
				var colName = string.Format(SumDataFields.RATIO_PER_GEAR_FORMAT, gear.Key);

				row[colName] = (double.IsNaN(gear.Value.Ratio)
								? (ConvertedSI)gear.Value.TorqueConverterRatio.SI<Scalar>()
								: (ConvertedSI)gear.Value.Ratio.SI<Scalar>());

				if (emData != null) {
					var colNameP2_5 = string.Format(SumDataFields.P2_5_RATIO_PER_GEAR_FORMAT, gear.Key);

					row[colNameP2_5] = (ConvertedSI)emData.RatioPerGear[gear.Key - 1].SI<Scalar>();
				}
			}
		}

		public static string ReplaceNotAllowedCharacters(string text)
		{
			return text.Replace('#', '_').Replace(',', '_').Replace('\n', '_').Replace('\r', '_');
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
				lock (Table)
					Table.Dispose();
			}
		}
	}
}
