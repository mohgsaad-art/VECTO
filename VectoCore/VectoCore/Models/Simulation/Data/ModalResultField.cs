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
using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.DataBus;

namespace TUGraz.VectoCore.Models.Simulation.Data
{
	/// <summary>
	///     Enum with field definitions of the Modal Results File (.vmod).
	/// </summary>
	public enum ModalResultField
	{
		/// <summary>
		///     Time step [s].
		///     Midpoint of the simulated interval.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "time [s]")] time,

		/// <summary>
		///     Simulation interval around the current time step. [s]
		/// </summary>
		[ModalResultField(typeof(SI), "simulation_interval", "dt [s]")] simulationInterval,

		[ModalResultField(typeof(bool), "ICE On")] ICEOn,

		[ModalResultField(typeof(SI), caption: "P_ice_start [kW]", outputFactor: 1e-3)] P_ice_start,


		[ModalResultField(typeof(SI), caption: "P_aux_ESS_mech_ICE_off [kW]", outputFactor: 1e-3)] P_aux_ESS_mech_ice_off,

		[ModalResultField(typeof(SI), caption: "P_aux_ESS_mech_ICE_on [kW]", outputFactor: 1e-3)] P_aux_ESS_mech_ice_on,

		/// <summary>
		///     Engine speed [1/min].
		/// </summary>
		[ModalResultField(typeof(SI), caption: "n_eng_avg [1/min]", outputFactor: 60 / (2 * Math.PI))] n_eng_avg,

		/// <summary>
		///     [Nm]	Engine torque.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "T_eng_fcmap [Nm]")] T_eng_fcmap,

		/// <summary>
		///     [Nm]	Full load torque
		/// </summary>
		[ModalResultField(typeof(SI), caption: "Tq_full [Nm]")] Tq_full,

		/// <summary>
		///     [Nm]	Motoring torque
		/// </summary>
		[ModalResultField(typeof(SI), caption: "Tq_drag [Nm]")] Tq_drag,

		/// <summary>
		///     [kW]	Engine power.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_eng_out [kW]", outputFactor: 1e-3)] P_eng_out,

		/// <summary>
		///     [kW]	Engine full load power.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_eng_full [kW]", outputFactor: 1e-3)] P_eng_full,

		/// <summary>
		///     [kW]	Engine drag power.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_eng_drag [kW]", outputFactor: 1e-3)] P_eng_drag,

		/// <summary>
		///     [kW]	Engine power at clutch (equals Pe minus loss due to rotational inertia Pa Eng).
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_clutch_out [kW]", outputFactor: 1e-3)] P_clutch_out,

		/// <summary>
		///     [kW]	Rotational acceleration power: Engine.
		/// </summary>
		[ModalResultField(typeof(SI), name: "P_eng_inertia", caption: "P_eng_inertia [kW]", outputFactor: 1e-3)] P_eng_inertia,

		/// <summary>
		///     [kW]	Total auxiliary power demand .
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_aux [kW]", outputFactor: 1e-3)] P_aux,

		/// <summary>
		/// [g/h] Fuel consumption from FC map..
		/// </summary>
		[ModalResultField(typeof(SI), name: "FC-Map", caption: "FC-Map{0} [g/h]", outputFactor: 3600 * 1000)] FCMap,

		/// <summary>
		/// [g/h] Fuel consumption after correction for different NCV in VECTO Engine and VECTO sim. (Based on FC.)
		/// </summary>
		[ModalResultField(typeof(SI), name: "FC-NCVc", caption: "FC-NCVc{0} [g/h]", outputFactor: 3600 * 1000)] FCNCVc,

		/// <summary>
		/// [g/h] Fuel consumption after WHTC Correction. (Based on FC-AUXc.)
		/// </summary>
		[ModalResultField(typeof(SI), name: "FC-WHTCc", caption: "FC-WHTCc{0} [g/h]", outputFactor: 3600 * 1000)] FCWHTCc,

		/// <summary>
		/// [g/h] Fuel consumption after smart auxiliary correction.
		/// </summary>
		[ModalResultField(typeof(SI), name: "FC-AAUX", caption: "FC-AAUX{0} [g/h]", outputFactor: 3600 * 1000)] FCAAUX,

		///// <summary>
		///// [g/h] Fuel consumption after correction for ADAS technologies. (Based on FC-AAUXc.)
		///// </summary>
		//[ModalResultField(typeof(SI), name: "FC-ADAS", caption: "FC-ADAS{0} [g/h]", outputFactor: 3600 * 1000)] FCADAS,

		/// <summary>
		/// [g/h] Fuel consumption after WHTC Correction. (Based on FC-ADAS.)
		/// </summary>
		[ModalResultField(typeof(SI), name: "FC-Final_mod", caption: "FC-Final_mod{0} [g/h]", outputFactor: 3600 * 1000)] FCFinal,

		/// <summary>
		///     [km]	Travelled distance.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "dist [m]")] dist,

		/// <summary>
		///     [km/h]	Actual vehicle speed.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "v_act [km/h]", outputFactor: 3.6)] v_act,

		/// <summary>
		///     [km/h]	Target vehicle speed.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "v_targ [km/h]", outputFactor: 3.6)] v_targ,

		/// <summary>
		///     [m/s2]	Vehicle acceleration.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "acc [m/s^2]")] acc,

		/// <summary>
		///     [%]	    Road gradient.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "grad [%]")] grad,

		/// <summary>
		///     [-]	 GearData. "0" = clutch opened / neutral. "0.5" = lock-up clutch is open (AT with torque converter only, see
		///     Gearbox)
		/// </summary>
		[ModalResultField(typeof(uint), caption: "Gear [-]")] Gear,

		[ModalResultField(typeof(SI), caption: "n_gbx_out_avg [1/min]", outputFactor: 60 / (2 * Math.PI))] n_gbx_out_avg,

		[ModalResultField(typeof(SI), caption: "T_gbx_out [Nm]")] T_gbx_out,

		/// <summary>
		///     [kW]	Gearbox losses.
		/// </summary>
		[ModalResultField(typeof(SI), name: "P_gbx_loss", caption: "P_gbx_loss [kW]", outputFactor: 1e-3)] P_gbx_loss,


		[ModalResultField(typeof(SI), name: "P_gbx_shift_loss", caption: "P_gbx_shift_loss [kW]", outputFactor: 1e-3)] P_gbx_shift_loss,

		/// <summary>
		///     [kW]	Losses in differential / axle transmission.
		/// </summary>
		[ModalResultField(typeof(SI), name: "Ploss Diff", caption: "P_axle_loss [kW]", outputFactor: 1e-3)] P_axle_loss,

		/// <summary>
		///     [kW]	Losses in angle transmission.
		/// </summary>
		[ModalResultField(typeof(SI), name: "Ploss Angle", caption: "P_angle_loss [kW]", outputFactor: 1e-3)] P_angle_loss,

		/// <summary>
		///     [kW]	Retarder losses.
		/// </summary>
		[ModalResultField(typeof(SI), name: "P_ret_loss", caption: "P_ret_loss [kW]", outputFactor: 1e-3)] P_ret_loss,

		/// <summary>
		///     [kW]	Rotational acceleration power: Gearbox.
		/// </summary>
		[ModalResultField(typeof(SI), name: "P_gbx_inertia", caption: "P_gbx_inertia [kW]", outputFactor: 1e-3)] P_gbx_inertia,

		/// <summary>
		///     [kW]	Vehicle acceleration power.
		/// </summary>
		[ModalResultField(typeof(SI), name: "P_veh_inertia", caption: "P_veh_inertia [kW]", outputFactor: 1e-3)] P_veh_inertia,

		/// <summary>
		///     [kW]	Rolling resistance power demand.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_roll [kW]", outputFactor: 1e-3)] P_roll,

		/// <summary>
		///     [kW]	Air resistance power demand.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_air [kW]", outputFactor: 1e-3)] P_air,

		/// <summary>
		///     [kW]	Power demand due to road gradient.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_slope [kW]", outputFactor: 1e-3)] P_slope,

		/// <summary>
		///     [kW]	Total power demand at wheel = sum of rolling, air, acceleration and road gradient resistance.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_wheel_in [kW]", outputFactor: 1e-3)] P_wheel_in,

		/// <summary>
		///     [kW]	Brake power. Drag power is included in Pe.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_brake_loss [kW]", outputFactor: 1e-3)] P_brake_loss,

		[ModalResultField(typeof(SI), caption: "P_wheel_inertia [kW]", outputFactor: 1e-3)] P_wheel_inertia,

		[ModalResultField(typeof(SI), caption: "P_brake_in [kW]", outputFactor: 1e-3)] P_brake_in,

		[ModalResultField(typeof(SI), caption: "P_axle_in [kW]", outputFactor: 1e-3)] P_axle_in,

		[ModalResultField(typeof(SI), caption: "P_angle_in [kW]", outputFactor: 1e-3)] P_angle_in,

		[ModalResultField(typeof(SI), caption: "P_ret_in [kW]", outputFactor: 1e-3)] P_retarder_in,

		[ModalResultField(typeof(SI), caption: "P_gbx_in [kW]", outputFactor: 1e-3)] P_gbx_in,

		[ModalResultField(typeof(SI), caption: "P_clutch_loss [kW]", outputFactor: 1e-3)] P_clutch_loss,

		[ModalResultField(typeof(SI), caption: "P_trac [kW]", outputFactor: 1e-3)] P_trac,

		[ModalResultField(typeof(SI), caption: "P_eng_fcmap [kW]", outputFactor: 1e-3)] P_eng_fcmap,

		[ModalResultField(typeof(SI), caption: "P_eng_full_stat [kW]", outputFactor: 1e-3)] P_eng_full_stat,

		/// <summary>
		///     [kW]	Power demand of Auxiliary with ID xxx. See also Aux Dialog and Driving Cycle.
		/// </summary>
		[ModalResultField(typeof(SI), caption: "P_aux_{0} [kW]", outputFactor: 1e-3)] P_aux_,

		/// Bus Aux Data
		[ModalResultField(typeof(SI), caption: "P_busAux_ES_HVAC [kW]", outputFactor: 1e-3)] P_busAux_ES_HVAC,

		[ModalResultField(typeof(SI), caption: "P_busAux_ES_other [kW]", outputFactor: 1e-3)] P_busAux_ES_other,

		[ModalResultField(typeof(SI), caption: "P_busAux_ES_consumer_sum [kW]", outputFactor: 1e-3)] P_busAux_ES_consumer_sum,

		[ModalResultField(typeof(SI), caption: "P_busAux_ES_gen [kW]", outputFactor: 1e-3)] P_busAux_ES_generated,

		[ModalResultField(typeof(SI), caption: "P_busAux_ES_mech [kW]", outputFactor: 1e-3)] P_busAux_ES_sum_mech,

		[ModalResultField(typeof(SI), caption: "Nl_busAux_consumer [Nl]")] Nl_busAux_PS_consumer,

		[ModalResultField(typeof(SI), caption: "Nl_busAux_gen [Nl]")] Nl_busAux_PS_generated,

		[ModalResultField(typeof(SI), caption: "Nl_busAux_gen_max [Nl]")] Nl_busAux_PS_generated_alwaysOn,

		//[ModalResultField(typeof(SI), caption: "Nl_busAux_gen_drag [Nl]")] Nl_busAux_PS_generated_dragOnly,

		[ModalResultField(typeof(SI), caption: "P_busAux_PS_gen [kW]", outputFactor: 1e-3)] P_busAux_PS_generated,

		[ModalResultField(typeof(SI), caption: "P_busAux_PS_gen_max [kW]", outputFactor: 1e-3)] P_busAux_PS_generated_alwaysOn,

		[ModalResultField(typeof(SI), caption: "P_busAux_PS_gen_drag [kW]", outputFactor: 1e-3)] P_busAux_PS_generated_dragOnly,

		[ModalResultField(typeof(SI), caption: "P_busAux_HVACmech_consumer [kW]", outputFactor: 1e-3)] P_busAux_HVACmech_consumer,

		[ModalResultField(typeof(SI), caption: "P_busAux_HVACmech_gen [kW]", outputFactor: 1e-3)] P_busAux_HVACmech_gen,

		[ModalResultField(typeof(double), caption: "Battery SoC")] BatterySOC,

		[ModalResultField(typeof(SI), caption: "P_busAux_bat [kW]", outputFactor: 1e-3)] P_busAux_bat,

		/// <summary>
		///		[-]  true/false  indicate whether torque converter is locked or not (only applicable for gears with TC)
		/// </summary>
		[ModalResultField(typeof(int), caption: "TC locked")] TC_Locked,

		/// <summary>
		///     [-]	    Torque converter speed ratio
		/// </summary>
		[ModalResultField(typeof(double), name: "TCnu")] TorqueConverterSpeedRatio,

		/// <summary>
		///     [-]	    Torque converter torque ratio
		/// </summary>
		[ModalResultField(typeof(double), name: "TCmu")] TorqueConverterTorqueRatio,

		[ModalResultField(typeof(SI), "P_TC_out [kW]", outputFactor: 1e-3)] P_TC_out,

		[ModalResultField(typeof(SI), "P_TC_in [kW]", outputFactor: 1e-3)] P_TC_in,

		/// <summary>
		///     [kW]	Power loss at the torque converter.
		/// </summary>
		[ModalResultField(typeof(SI), "P_TC_loss [kW]", outputFactor: 1e-3)] P_TC_loss,

		/// <summary>
		///     [Nm]	Torque converter output torque
		/// </summary>
		[ModalResultField(typeof(SI), "T_TC_out")] TC_TorqueOut,

		/// <summary>
		///     [1/min]	Torque converter output speed
		/// </summary>
		[ModalResultField(typeof(SI), "n_TC_out", outputFactor: 60 / (2 * Math.PI))] TC_angularSpeedOut,

		/// <summary>
		///     [Nm]	Torque converter output torque
		/// </summary>
		[ModalResultField(typeof(SI), "T_TC_in")] TC_TorqueIn,

		/// <summary>
		///     [1/min]	Torque converter output speed
		/// </summary>
		[ModalResultField(typeof(SI), "n_TC_in", outputFactor: 60 / (2 * Math.PI))] TC_angularSpeedIn,

		/// <summary>
		///     [m]	Altitude
		/// </summary>
		[ModalResultField(typeof(SI), caption: "altitude [m]")] altitude,

		[ModalResultField(typeof(SI), name: "ds [m]")] simulationDistance,

		[ModalResultField(typeof(DrivingBehavior))] drivingBehavior,

		[ModalResultField(typeof(SI), "P_WHR_el [kW]", outputFactor: 1e-3)] P_WHR_el_map,
		[ModalResultField(typeof(SI), "P_WHR_el_corr [kW]", outputFactor: 1e-3)] P_WHR_el_corr,
		[ModalResultField(typeof(SI), "P_WHR_mech [kW]", outputFactor: 1e-3)] P_WHR_mech_map,
		[ModalResultField(typeof(SI), "P_WHR_mech_corr [kW]", outputFactor: 1e-3)] P_WHR_mech_corr,

		[ModalResultField(typeof(int), caption: "DriverAction")] DriverAction,
		[ModalResultField(typeof(int), caption: "EcoRollConditionsMet")] EcoRollConditionsMet,
		[ModalResultField(typeof(int), caption: "PCCSegment")] PCCSegment,
		[ModalResultField(typeof(int), caption: "PCCState")] PCCState,

		//[ModalResultField(typeof(double), caption: "AA_NonSmartAlternatorsEfficiency [%]")] AA_NonSmartAlternatorsEfficiency,
		//[ModalResultField(typeof(SI), caption: "AA_SmartIdleCurrent_Amps [A]")] AA_SmartIdleCurrent_Amps,
		//[ModalResultField(typeof(double), caption: "AA_SmartIdleAlternatorsEfficiency [%]")] AA_SmartIdleAlternatorsEfficiency,
		//[ModalResultField(typeof(SI), caption: "AA_SmartTractionCurrent_Amps [A]")] AA_SmartTractionCurrent_Amps,
		//[ModalResultField(typeof(double), caption: "AA_SmartTractionAlternatorEfficiency [%]")] AA_SmartTractionAlternatorEfficiency,
		//[ModalResultField(typeof(SI), caption: "AA_SmartOverrunCurrent_Amps [A]")] AA_SmartOverrunCurrent_Amps,
		//[ModalResultField(typeof(double), caption: "AA_SmartOverrunAlternatorEfficiency [%]")] AA_SmartOverrunAlternatorEfficiency,
		//[ModalResultField(typeof(SI), caption: "AA_CompressorFlowRate_LitrePerSec [Ni L/s]")] AA_CompressorFlowRate_LitrePerSec,
		//[ModalResultField(typeof(int), caption: "AA_OverrunFlag [bool]")] AA_OverrunFlag,
		//[ModalResultField(typeof(int), caption: "AA_EngineIdleFlag [bool]")] AA_EngineIdleFlag,
		//[ModalResultField(typeof(int), caption: "AA_CompressorFlag [bool]")] AA_CompressorFlag,
		//[ModalResultField(typeof(SI), caption: "AA_TotalCycleFC_Grams [g]", outputFactor: 1000)] AA_TotalCycleFC_Grams,
		//[ModalResultField(typeof(SI), caption: "AA_TotalCycleFC_Litres [l]")] AA_TotalCycleFC_Litres,
		//[ModalResultField(typeof(SI), caption: "AA_AveragePowerDemandCrankHVACMechanicals [W]")] AA_AveragePowerDemandCrankHVACMechanicals,
		//[ModalResultField(typeof(SI), caption: "AA_AveragePowerDemandCrankHVACElectricals [W]")] AA_AveragePowerDemandCrankHVACElectricals,
		//[ModalResultField(typeof(SI), caption: "AA_AveragePowerDemandCrankElectrics [W]")] AA_AveragePowerDemandCrankElectrics,
		//[ModalResultField(typeof(SI), caption: "AA_AveragePowerDemandCrankPneumatics [W]")] AA_AveragePowerDemandCrankPneumatics,
		//[ModalResultField(typeof(SI), caption: "AA_TotalCycleFuelConsumptionCompressorOff [g]", outputFactor: 1000)] AA_TotalCycleFuelConsumptionCompressorOff,
		//[ModalResultField(typeof(SI), caption: "AA_TotalCycleFuelConsumptionCompressorOn [g]", outputFactor: 1000)] AA_TotalCycleFuelConsumptionCompressorOn,

	}

	[AttributeUsage(AttributeTargets.Field)]
	public class ModalResultFieldAttribute : Attribute
	{
		internal ModalResultFieldAttribute(Type dataType, string name = null, string caption = null, uint decimals = 4,
			double outputFactor = 1, bool showUnit = false)
		{
			DataType = dataType;
			Name = name;
			Caption = caption;
			Decimals = decimals;
			OutputFactor = outputFactor;
			ShowUnit = showUnit;
		}

		public bool ShowUnit { get; private set; }
		public double OutputFactor { get; private set; }
		public uint Decimals { get; private set; }
		public Type DataType { get; private set; }
		public string Name { get; private set; }
		public string Caption { get; private set; }
	}

	public static class ModalResultFieldExtensionMethods
	{
		private static ConcurrentDictionary<ModalResultField, ModalResultFieldAttribute> _attributeDictionary =
			new ConcurrentDictionary<ModalResultField, ModalResultFieldAttribute>();

		public static string GetName(this ModalResultField field)
		{
			return GetAttribute(field).Name ?? field.ToString();
		}

		public static string GetCaption(this ModalResultField field)
		{
			return GetAttribute(field).Caption ?? GetAttribute(field).Name ?? field.ToString();
		}

		public static string GetShortCaption(this ModalResultField field)
		{
			var caption = GetCaption(field);
			return Regex.Replace(caption, @"\[.*?\]|\<|\>", "").Trim();
		}

		public static Type GetDataType(this ModalResultField field)
		{
			return GetAttribute(field).DataType;
		}

		public static ModalResultFieldAttribute GetAttribute(this ModalResultField field)
		{
			var attributeCached = _attributeDictionary.TryGetValue(field, out var attribute);
			if (attributeCached) {
				return attribute;
			} else {
				attribute = (ModalResultFieldAttribute)Attribute.GetCustomAttribute(ForValue(field), typeof(ModalResultFieldAttribute));
				_attributeDictionary.TryAdd(field, attribute);
			}
			return attribute;
		}

		private static MemberInfo ForValue(ModalResultField field)
		{
			return typeof(ModalResultField).GetField(Enum.GetName(typeof(ModalResultField), field));
		}
	}
}