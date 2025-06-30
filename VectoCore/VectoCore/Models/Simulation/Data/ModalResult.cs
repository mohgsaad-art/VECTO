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
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

// ReSharper disable InconsistentNaming

namespace TUGraz.VectoCore.Models.Simulation.Data
{
    [DesignerCategory("")] // Full qualified attribute needed to disable design view in VisualStudio
	[Serializable]
	public class ModalResults : DataTable
	{
		public static class ExtendedPropertyNames
		{
			public const string Decimals = "decimals";
			public const string OutputFactor = "outputFactor";
			public const string ShowUnit = "showUnit";
		}

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] DistanceCycleSignals = {
			ModalResultField.time,
			ModalResultField.simulationInterval,
			ModalResultField.dist,
			ModalResultField.simulationDistance,

			ModalResultField.v_act,
			ModalResultField.v_targ,
			
			ModalResultField.grad,
			ModalResultField.altitude,
			ModalResultField.Highway,

			ModalResultField.drivingBehavior,
		};

		public static readonly ModalResultField[] TimeCycleSignals = {
			ModalResultField.time,
			ModalResultField.simulationInterval,
			ModalResultField.dist,
			ModalResultField.simulationDistance,

			ModalResultField.v_act,
			ModalResultField.v_targ,

			ModalResultField.grad,
			ModalResultField.altitude,
			ModalResultField.Highway,

			ModalResultField.drivingBehavior,
		};

		public static readonly ModalResultField[] EngineOnlySignals = {
			ModalResultField.time,
			ModalResultField.simulationInterval,
			ModalResultField.drivingBehavior,
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] DriverSignals = {
			ModalResultField.acc,
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] CombustionEngineSignals = {
			ModalResultField.ICEOn,
			ModalResultField.P_ice_start,
			ModalResultField.P_ice_drag,
			ModalResultField.P_ice_full,
			ModalResultField.P_ice_full_stat,
			ModalResultField.P_ice_out,
			ModalResultField.P_ice_inertia,
			ModalResultField.P_ice_fcmap,
			ModalResultField.P_aux_ESS_mech_ice_on,
			ModalResultField.P_aux_ESS_mech_ice_off,
			ModalResultField.n_ice_avg,
			ModalResultField.T_ice_fcmap,
			ModalResultField.T_ice_full,
			ModalResultField.T_ice_drag,
			ModalResultField.P_WHR_el_map,
			ModalResultField.P_WHR_el_corr,
			ModalResultField.P_WHR_mech_map,
			ModalResultField.P_WHR_mech_corr,
			ModalResultField.P_aux_mech
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] ClutchSignals = {
			ModalResultField.P_clutch_loss,
			ModalResultField.P_clutch_out
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] GearboxSignals = {
			ModalResultField.P_gbx_in,
			ModalResultField.P_gbx_inertia,
			ModalResultField.P_gbx_loss,
			//ModalResultField.P_gbx_shift_loss,   // only for APT-S and APT-P
			ModalResultField.T_gbx_in,
			ModalResultField.T_gbx_out,
			ModalResultField.n_gbx_out_avg,
			ModalResultField.n_gbx_in_avg,

			ModalResultField.Gear
		};

		public static readonly ModalResultField[] GearboxSignals_AT =
			GearboxSignals.Concat(new[] { ModalResultField.P_gbx_shift_loss }).ToArray();

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] TorqueConverterSignals = {
			ModalResultField.TC_Locked,
			ModalResultField.TC_TorqueIn,
			ModalResultField.TC_TorqueOut,
			ModalResultField.TC_angularSpeedIn,
			ModalResultField.TC_angularSpeedOut,
			ModalResultField.P_TC_in,
			ModalResultField.P_TC_loss,
			ModalResultField.P_TC_out,
			ModalResultField.TorqueConverterSpeedRatio,
			ModalResultField.TorqueConverterTorqueRatio
		};

		// ------------------------------------------------------------------------------------t
		public static readonly ModalResultField[] RetarderSignals = {
			ModalResultField.P_retarder_in,
			ModalResultField.P_ret_loss
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] AngledriveSignals = {
			ModalResultField.P_angle_in,
			ModalResultField.P_angle_loss
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] AxlegearSignals = {
			ModalResultField.P_axle_in,
			ModalResultField.P_axle_loss,
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] VehicleSignals = {
			ModalResultField.P_veh_inertia,
			ModalResultField.P_roll,
			ModalResultField.P_air,
			ModalResultField.EffectiveAirDragArea,
			ModalResultField.P_slope,
			ModalResultField.P_trac
		};
		
		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] WheelSignals = {
			ModalResultField.P_wheel_in,
			ModalResultField.P_wheel_inertia
		};
		
		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] BusAuxiliariesSignals = {
			ModalResultField.BusAux_OverrunFlag,
			ModalResultField.Nl_busAux_PS_consumer,
			ModalResultField.Nl_busAux_PS_generated,
			ModalResultField.Nl_busAux_PS_generated_alwaysOn,
			ModalResultField.P_busAux_ES_HVAC,
			ModalResultField.P_busAux_ES_other,
			ModalResultField.P_busAux_ES_consumer_sum,
			ModalResultField.P_busAux_ES_generated,
			ModalResultField.P_busAux_ES_sum_mech,
			ModalResultField.P_busAux_HVACmech_consumer,
			ModalResultField.P_busAux_HVACmech_gen,
			ModalResultField.P_busAux_PS_generated,
			ModalResultField.P_busAux_PS_generated_alwaysOn,
			ModalResultField.P_busAux_bat,
			ModalResultField.P_busAux_PS_generated_dragOnly,
			ModalResultField.BatterySOC
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] FuelConsumptionSignals =  {
			ModalResultField.FCMap,
			ModalResultField.FCNCVc,
			ModalResultField.FCWHTCc, 
			// ModalResultField.FCAAUX,
			/*ModalResultField.FCICEStopStart,*/ 
			ModalResultField.FCFinal
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] ElectricMotorSignals = {
			ModalResultField.EM_ratio_,
			ModalResultField.P_EM_out_,
			ModalResultField.P_EM_mech_,
			ModalResultField.P_EM_in_,

			ModalResultField.P_EM_TransmissionLoss_,
			ModalResultField.P_EM_electricMotor_em_mech_,
			ModalResultField.P_EM_electricMotorInertiaLoss_,
			ModalResultField.P_EM_electricMotor_em_mech_map_,
			ModalResultField.P_EM_electricMotorLoss_,
			ModalResultField.P_EM_electricMotor_el_,

			ModalResultField.P_EM_loss_,

			ModalResultField.n_EM_electricMotor_,
			ModalResultField.T_EM_electricMotor_,
			ModalResultField.T_EM_electricMotor_map_,

			ModalResultField.T_EM_electricMotor_drive_max_,
			ModalResultField.T_EM_electricMotor_gen_max_,

			ModalResultField.P_EM_electricMotor_drive_max_,
			ModalResultField.P_EM_electricMotor_gen_max_,

			ModalResultField.ElectricMotor_OvlBuffer_,
			ModalResultField.EM_Off_,
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] IEPCSignals = {
			ModalResultField.n_IEPC_int_,
			ModalResultField.T_IEPC_,
			ModalResultField.T_IEPC_map_,
			ModalResultField.T_IEPC_int_drive_max_,
			ModalResultField.T_IEPC_int_gen_max_,
			ModalResultField.P_IEPC_int_gen_max_,
			ModalResultField.P_IEPC_int_drive_max_,
			ModalResultField.P_IEPC_electricMotorInertiaLoss_,
			ModalResultField.P_IEPC_int_mech_map_,
			ModalResultField.P_IEPC_el_,
			ModalResultField.P_IEPC_out_,
			ModalResultField.P_IEPC_electricMotorLoss_,
			ModalResultField.IEPC_Off_,
			ModalResultField.IEPC_OvlBuffer_,
			
		};

		public static readonly ModalResultField[] IEPCTransmissionSignals = {
			ModalResultField.T_IEPC_out,
			ModalResultField.n_IEPC_out_avg,

			ModalResultField.Gear
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] BatterySignals = {
			ModalResultField.U0_reess,
			ModalResultField.U_reess_terminal,
			ModalResultField.I_reess,
			ModalResultField.REESSStateOfCharge,
			ModalResultField.P_reess_terminal,
			ModalResultField.P_reess_int,
			ModalResultField.P_reess_loss,
			ModalResultField.P_reess_charge_max,
			ModalResultField.P_reess_discharge_max,
			ModalResultField.P_terminal_ES,
			ModalResultField.P_ES_Conn_loss
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] FuelCellSystemSignals = {
			ModalResultField.P_FCSystem,
			ModalResultField.FC_FCSystem,
		};

		public static readonly ModalResultField[] FuelCellSignals = {
			ModalResultField.FC_FCS,
			ModalResultField.P_FCS,
		};

		public static readonly ModalResultField[] FuelCellComponentSignals = {
			ModalResultField.t_FCS_On,
		};


		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] BrakeSignals = {
			ModalResultField.P_brake_loss,
			ModalResultField.P_brake_in
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] WheelEndSignals = {
			ModalResultField.P_wheelEnd_in,
			ModalResultField.P_wheelEnd_saving
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] ElectricSystemSignals = {
			ModalResultField.P_Aux_el_HV,
		};

		public static readonly ModalResultField[] ElectricAuxiliarySignals = {
			ModalResultField.P_aux_el,
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] HybridControllerSignals = {
			ModalResultField.HybridStrategyScore,
			ModalResultField.HybridStrategySolution,
			ModalResultField.MaxPropulsionTorqe,
			ModalResultField.HybridStrategyState,
		};

		// ------------------------------------------------------------------------------------
		public static readonly ModalResultField[] DCDCConverterSignals = {
			ModalResultField.P_DCDC_In,
			ModalResultField.P_DCDC_Out,
			ModalResultField.P_DCDC_missing,

			////Debug
			//ModalResultField.DCDCStateCount_,
			//ModalResultField.SimIntervalCurrent_,
			//ModalResultField.SimIntervalPrev_
		};

		protected internal readonly Dictionary<IFuelProperties, Dictionary<ModalResultField, DataColumn>> FuelColumns = new Dictionary<IFuelProperties, Dictionary<ModalResultField, DataColumn>>();

		protected internal List<PowertrainPosition> ElectricMotors = new List<PowertrainPosition>();

		protected internal List<string> FuelCellStringColumns = new List<string>();
		protected internal List<string> FuelCellColumns = new List<string>(); //contains fuel cell ids as string
		protected internal List<string> FuelCellComponentIds = new List<string>();


		protected internal Dictionary<int, Dictionary<ModalResultField, DataColumn>> BatteryColumns =
			new Dictionary<int, Dictionary<ModalResultField, DataColumn>>();

		protected ModalResults(SerializationInfo info, StreamingContext context) : base(info, context) {}

		public ModalResults()
		{
			//CreateColumns(CommonSignals);
		}

		protected internal void CreateColumns(ModalResultField[] columns, Func<ModalResultField, string> nameFunc = null, Func<ModalResultField, string> captionFunc = null)
		{
			foreach (var value in columns) {
				var colName = nameFunc != null ? nameFunc(value) : value.GetName();
				if (Columns.Contains(colName)) {
					continue;
				}
				var col = new DataColumn(colName, value.GetAttribute().DataType)
					{ Caption = captionFunc != null ? captionFunc(value) : value.GetCaption() };
				col.ExtendedProperties[ExtendedPropertyNames.Decimals] = value.GetAttribute().Decimals;
				col.ExtendedProperties[ExtendedPropertyNames.OutputFactor] = value.GetAttribute().OutputFactor;
				col.ExtendedProperties[ExtendedPropertyNames.ShowUnit] = value.GetAttribute().ShowUnit;
				Columns.Add(col);
			}
		}

		public void RegisterComponent(VectoSimulationComponent component, VectoRunData runData)
		{
			switch (component) {
				case IDrivingCycleInfo d when d is IDistanceBasedDrivingCycle:
					CreateColumns(DistanceCycleSignals);
					break;
				case IDrivingCycleInfo t when t is IMeasuredSpeedDrivingCycle:
					CreateColumns(TimeCycleSignals);
					CreateColumns(DriverSignals);
					break;
				case IDrivingCycleInfo v when v is IVTPCycle:
				case IDrivingCycleInfo p when p is IPWheelCycle:
					CreateColumns(TimeCycleSignals);
					CreateColumns(WheelSignals);
					CreateColumns(DriverSignals);
					break;
				case IDrivingCycleInfo e when e is PowertrainDrivingCycle:
					CreateColumns(EngineOnlySignals);
					break;
				case ICombustionEngine c1: CreateCombustionEngineColumns(runData); break;
				case BusAuxiliariesAdapter _: CreateColumns(BusAuxiliariesSignals); break;
				case IClutch _:
					CreateColumns(ClutchSignals);
					break;
				case IGearbox g when runData.JobType != VectoSimulationJobType.FCHV_IEPC
						&& runData.JobType != VectoSimulationJobType.IEPC_E 
						&& runData.JobType != VectoSimulationJobType.IEPC_S:
					
					CreateColumns(runData.GearboxData?.Type.IsOneOf(GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.IHPC) ?? false
						? GearboxSignals_AT
						: GearboxSignals);
					if (g is MeasuredSpeedHybridsCycleGearbox && runData.GearboxData.Type == GearboxType.IHPC) {
						CreateColumns(TorqueConverterSignals);
                    }
					break;
				case IGearbox g when g is BEVCycleGearbox && runData.JobType == VectoSimulationJobType.IEPC_E:
					CreateColumns(GearboxSignals_AT);
					CreateColumns(TorqueConverterSignals);
					break;
				case IGearbox _ when runData.JobType == VectoSimulationJobType.IEPC_E:
				case IGearbox _ when runData.JobType == VectoSimulationJobType.IEPC_S:
				case IGearbox _ when runData.JobType == VectoSimulationJobType.FCHV_IEPC:
					CreateColumns(IEPCTransmissionSignals);
					break;
				case ITorqueConverter _: CreateColumns(TorqueConverterSignals);
					break;
				case IAngledrive _: CreateColumns(AngledriveSignals); break;
				case IAxlegear _: CreateColumns(AxlegearSignals); break;
				case Retarder _: CreateColumns(RetarderSignals); break;
				case IWheels _: CreateColumns(WheelSignals); break;
				case IBrakes _: CreateColumns(BrakeSignals); break;
				case WheelEnd _: CreateColumns(WheelEndSignals); break;	
				case IDriverInfo _: CreateColumns(DriverSignals); break;
				case IVehicle _: CreateColumns(VehicleSignals); break;
				case IElectricMotor c3 when c3.Position == PowertrainPosition.IEPC: 
					CreateElectricMotorColumns(c3.Position, runData, IEPCSignals);
					break;
				case IElectricMotor c4 when c4.Position != PowertrainPosition.IEPC:
					CreateElectricMotorColumns(c4.Position, runData, ElectricMotorSignals);
					break;
				case IElectricEnergyStorage c5 when c5 is BatterySystem: CreateBatteryColumns(runData);
					break;
				case IElectricEnergyStorage _: CreateColumns(BatterySignals);
					break;
				case IElectricSystem _: CreateColumns(ElectricSystemSignals);
					break;
				case IHybridController _: CreateColumns(HybridControllerSignals);
					break;
				case IDCDCConverter _: CreateColumns(DCDCConverterSignals);
					break;
				case FuelCellSystem _: CreateColumns(FuelCellSystemSignals);
					break;
				case FuelCellString fcs: 
					CreateFuelCellColumns(fcs.StringId,  FuelCellSignals, FuelCellStringColumns, FuelCellColumns);
					break;
				case FuelCell fc: 
					CreateFuelCellColumns(fc.Id.ToString(), FuelCellSignals, FuelCellColumns);
					CreateFuelCellColumns(fc.Id.ToString(), FuelCellComponentSignals, FuelCellColumns);
					FuelCellComponentIds.Add(fc.Id.ToString());
					break;
				case ElectricAuxiliaries _:
					CreateElectricAuxColumns();
					break;
			}
		}

		private void CreateElectricAuxColumns()
		{
			CreateColumns(ElectricAuxiliarySignals);
		}

		private void CreateBatteryColumns(VectoRunData vectoRunData)
		{
			CreateColumns(BatterySignals);

			foreach (var bat in vectoRunData.BatteryData.Batteries) {
				var idx = bat.Item2.BatteryId;
				var entry = BatteryColumns.GetOrAdd(idx, _ => new Dictionary<ModalResultField, DataColumn>());
				foreach (var key in BatterySignals) {
					entry.GetOrAdd(key, _ => {
						var c = Columns.Add($"{key.GetName()}_{idx}", 
							typeof(SI));
						c.Caption = key.GetCaption("_" + idx.ToString());
						c.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] = key.GetAttribute().Decimals;
						c.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] =
							key.GetAttribute().OutputFactor;
						c.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] = key.GetAttribute().ShowUnit;
						return c;
					});
				}
			}
		}
	

		protected internal void CreateElectricMotorColumns(PowertrainPosition emPos, VectoRunData runData,
			ModalResultField[] signals)
		{
			ElectricMotors.Add(emPos);
			foreach (var entry in signals) {
				var col = Columns.Add(string.Format(entry.GetAttribute().Caption, emPos.GetName()), typeof(SI));
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] =
					entry.GetAttribute().Decimals;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] =
					entry.GetAttribute().OutputFactor;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] =
					entry.GetAttribute().ShowUnit;
			}
		}

		protected internal void CreateFuelCellColumns(string id, IEnumerable<ModalResultField> signals, params List<string>[] columnLists)
		{
			foreach (var entry in signals) {
				foreach (var columnList in columnLists) {
					columnList.Add(string.Format(entry.GetCaption(), id));
				}

				var col = Columns.Add(string.Format(entry.GetAttribute().Caption, id), typeof(SI));
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] = entry.GetAttribute().Decimals;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] =
					entry.GetAttribute().OutputFactor;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] =
					entry.GetAttribute().ShowUnit;
			}
			
		}

		protected internal void CreateCombustionEngineColumns(VectoRunData runData)
		{
			CreateColumns(CombustionEngineSignals);
			//if (runData.BusAuxiliaries != null) {
			//	CreateColumns(BusAuxiliariesSignals);
			//}
			var multipleEngineModes = runData.EngineData?.MultipleEngineFuelModes ?? false;
			var fuels = runData.EngineData?.Fuels ?? new List<CombustionEngineFuelData>();
			foreach (var fuel in fuels) {
				var entry = fuel.FuelData;
				if (FuelColumns.ContainsKey(entry)) {
					throw new VectoException("Fuel {0} already added!", entry.FuelType.GetLabel());
				}

				FuelColumns[entry] = new Dictionary<ModalResultField, DataColumn>();
				foreach (var fcCol in ModalResults.FuelConsumptionSignals) {
					var col = new DataColumn(
						fuels.Count == 1 && !multipleEngineModes
							? fcCol.GetName()
							: $"{fcCol.GetName()}_{entry.FuelType.GetLabel()}", typeof(SI)) {
						Caption = string.Format(fcCol.GetCaption(),
							fuels.Count == 1 && !multipleEngineModes ? "" : "_" + entry.FuelType.GetLabel())
					};
					col.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] =
						fcCol.GetAttribute().Decimals;
					col.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] =
						fcCol.GetAttribute().OutputFactor;
					col.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] =
						fcCol.GetAttribute().ShowUnit;
					FuelColumns[entry][fcCol] = col;
					Columns.Add(col);
				}
			}
		}

		public void Reset()
		{
			FuelColumns.Clear();
			ElectricMotors.Clear();
			BatteryColumns.Clear();
		}
	}
}