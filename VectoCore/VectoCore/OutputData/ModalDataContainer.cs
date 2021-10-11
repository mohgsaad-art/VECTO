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
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.OutputData
{
	public class ModalDataContainer : IModalDataContainer
	{
		private readonly IModalDataFilter[] _filters;
		private readonly Action<ModalDataContainer> _addReportResult;
		protected internal ModalResults Data { get; set; }
		private DataRow CurrentRow { get; set; }

		private readonly IModalDataWriter _writer;
		private readonly List<string> _additionalColumns = new List<string>();
		private Exception SimException;

		protected internal readonly Dictionary<IFuelProperties, Dictionary<ModalResultField, DataColumn>> FuelColumns = new Dictionary<IFuelProperties, Dictionary<ModalResultField, DataColumn>>();

		public int JobRunId { get; private set; }
		public string RunName { get; private set; }
		public string CycleName { get; private set; }
		public string RunSuffix { get; private set; }

		public bool WriteModalResults { get; set; }

		public VectoRun.Status RunStatus { get; protected set; }

		public string Error
		{
			get { return SimException == null ? null : SimException.Message; }
		}

		public string StackTrace
		{
			get {
				return SimException == null
					? null
					: (SimException.StackTrace ?? (SimException.InnerException != null ? SimException.InnerException.StackTrace : null));
			}
		}

		public void Reset()
		{
			Data.Rows.Clear();
			CurrentRow = Data.NewRow();
			//ClearAggregateResults();
		}

		public bool WriteAdvancedAux { get; set; }


		public static readonly IList<ModalResultField> FuelConsumptionSignals = new[] {
			ModalResultField.FCMap, ModalResultField.FCNCVc, ModalResultField.FCWHTCc, // ModalResultField.FCAAUX,
			/*ModalResultField.FCICEStopStart,*/ ModalResultField.FCFinal
		};

		protected VectoRunData _runData;

		public ModalDataContainer(VectoRunData runData, IModalDataWriter writer,
			Action<ModalDataContainer> addReportResult, params IModalDataFilter[] filter)
		{
			_runData = runData;
			_writer = writer;

			_filters = filter ?? new IModalDataFilter[0];
			_addReportResult = addReportResult ?? (x => { });

			Auxiliaries = new Dictionary<string, DataColumn>();
			Data = new ModalResults(false);
			CurrentRow = Data.NewRow();

			//if (runData.JobType == VectoSimulationJobType.BatteryElectricVehicle) {
			//	return;
			//}

			//PostProcessingCorrection = new ModalDataPostprocessingCorrection();

			var multipleEngineModes = runData.EngineData?.MultipleEngineFuelModes ?? false;
			var fuels = runData.EngineData?.Fuels ?? new List<CombustionEngineFuelData>();
			foreach (var fuel in fuels) {
				var entry = fuel.FuelData;
				if (FuelColumns.ContainsKey(entry)) {
					throw new VectoException("Fuel {0} already added!", entry.FuelType.GetLabel());
				}

				FuelColumns[entry] = new Dictionary<ModalResultField, DataColumn>();
				foreach (var fcCol in FuelConsumptionSignals) {

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
					Data.Columns.Add(col);
				}
			}


		}



		public bool HasTorqueConverter { get; set; }

		public void CommitSimulationStep()
		{
			Data.Rows.Add(CurrentRow);
			CurrentRow = Data.NewRow();
		}

		public FuelData.Entry FuelData { get; internal set; }

		public void Finish(VectoRun.Status runStatus, Exception exception = null)
		{
			RunStatus = runStatus;
			SimException = exception;

			var dataColumns = GetOutputColumns();

			var strCols = dataColumns.Select(x => x.GetName())
				.Concat(Auxiliaries.Values.Select(c => c.ColumnName))
				.Concat(
					new[] {
						ModalResultField.FCMap, ModalResultField.FCNCVc, ModalResultField.FCWHTCc,
						ModalResultField.FCAAUX, ModalResultField.FCADAS, ModalResultField.FCFinal
					}.Select(x => x.GetName()));
#if TRACE
			strCols = strCols.Concat(_additionalColumns);
#endif
			if (WriteModalResults) {
				var filteredData = Data;
				foreach (var filter in _filters) {
					RunSuffix += "_" + filter.ID;
					filteredData = filter.Filter(filteredData);
				}

				try {
					_writer.WriteModData(
						JobRunId, RunName, CycleName, RunSuffix,
						new DataView(filteredData).ToTable(false, strCols.ToArray()));
				} catch (Exception e) {
					LogManager.GetLogger(typeof(ModalDataContainer).FullName).Error(e.Message);
				}
			}

			_addReportResult(this);
		}

		private IEnumerable<ModalResultField> GetOutputColumns()
		{
			var dataColumns = new List<ModalResultField> { ModalResultField.time };
            var writeEngineOnly = _runData.JobType == VectoSimulationJobType.EngineOnlySimulation;

            if (!writeEngineOnly) {
				dataColumns.AddRange(
					new[] {
						ModalResultField.simulationInterval,
						ModalResultField.dist,
						ModalResultField.v_act,
						ModalResultField.v_targ,
						ModalResultField.acc,
						ModalResultField.grad
					});
			}
			if (!writeEngineOnly) {
				dataColumns.AddRange(
					new[] {
						ModalResultField.Gear,
					});
				if (HasTorqueConverter) {
					dataColumns.AddRange(new[] { ModalResultField.TC_Locked });
				}
			}
			dataColumns.AddRange(
				new[] {
					ModalResultField.n_eng_avg,
					ModalResultField.T_eng_fcmap,
					ModalResultField.Tq_full,
					ModalResultField.Tq_drag,
					ModalResultField.P_eng_fcmap,
					ModalResultField.P_eng_full,
					ModalResultField.P_eng_full_stat,
					ModalResultField.P_eng_drag,
					ModalResultField.P_eng_inertia,
					ModalResultField.P_eng_out,
				});
			if (HasTorqueConverter) {
				dataColumns.AddRange(
					new[] {
						ModalResultField.P_gbx_shift_loss,
						ModalResultField.P_TC_loss,
						ModalResultField.P_TC_out,
					});
			} else {
				dataColumns.AddRange(
					new[] {
						ModalResultField.P_clutch_loss,
						ModalResultField.P_clutch_out,
					});
			}
			dataColumns.AddRange(
				new[] {
					ModalResultField.P_aux
				});

			if (!writeEngineOnly) {
				dataColumns.AddRange(
					new[] {
						ModalResultField.P_gbx_in,
						ModalResultField.P_gbx_loss,
						ModalResultField.P_gbx_inertia,
						ModalResultField.P_retarder_in,
						ModalResultField.P_ret_loss,
						ModalResultField.P_angle_in,
						ModalResultField.P_angle_loss,
						ModalResultField.P_axle_in,
						ModalResultField.P_axle_loss,
						ModalResultField.P_brake_in,
						ModalResultField.P_brake_loss,
						ModalResultField.P_wheel_in,
						ModalResultField.P_wheel_inertia,
						ModalResultField.P_trac,
						ModalResultField.P_slope,
						ModalResultField.P_air,
						ModalResultField.P_roll,
						ModalResultField.P_veh_inertia,
						ModalResultField.n_gbx_out_avg,
						ModalResultField.T_gbx_out
					});

				if (HasTorqueConverter) {
					dataColumns.AddRange(
						new[] {
							ModalResultField.TorqueConverterSpeedRatio,
							ModalResultField.TorqueConverterTorqueRatio,
							ModalResultField.TC_TorqueOut,
							ModalResultField.TC_angularSpeedOut,
							ModalResultField.TC_TorqueIn,
							ModalResultField.TC_angularSpeedIn,
						});
				}
			}
			if (!writeEngineOnly && WriteAdvancedAux) {
				dataColumns.AddRange(
					new[] {
						ModalResultField.AA_NonSmartAlternatorsEfficiency,
						ModalResultField.AA_SmartIdleCurrent_Amps,
						ModalResultField.AA_SmartIdleAlternatorsEfficiency,
						ModalResultField.AA_SmartTractionCurrent_Amps,
						ModalResultField.AA_SmartTractionAlternatorEfficiency,
						ModalResultField.AA_SmartOverrunCurrent_Amps,
						ModalResultField.AA_SmartOverrunAlternatorEfficiency,
						ModalResultField.AA_CompressorFlowRate_LitrePerSec,
						ModalResultField.AA_OverrunFlag,
						ModalResultField.AA_EngineIdleFlag,
						ModalResultField.AA_CompressorFlag,
						ModalResultField.AA_TotalCycleFC_Grams,
						ModalResultField.AA_TotalCycleFC_Litres,
						ModalResultField.AA_AveragePowerDemandCrankHVACMechanicals,
						ModalResultField.AA_AveragePowerDemandCrankHVACElectricals,
						ModalResultField.AA_AveragePowerDemandCrankElectrics,
						ModalResultField.AA_AveragePowerDemandCrankPneumatics,
						ModalResultField.AA_TotalCycleFuelConsumptionCompressorOff,
						ModalResultField.AA_TotalCycleFuelConsumptionCompressorOn,
					});
			}
			return dataColumns;
		}

		public IEnumerable<T> GetValues<T>(DataColumn col)
		{
			return Data.Rows.Cast<DataRow>().Select(x => x.Field<T>(col));
		}

		public IEnumerable<T> GetValues<T>(Func<DataRow, T> selectorFunc)
		{
			return from DataRow row in Data.Rows select selectorFunc(row);
		}

		public T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null) where T : SIBase<T>
		{
			var result = 0.0;
			for (var i = 0; i < Data.Rows.Count; i++) {
				var value = Data.Rows[i][(int)field];
				if (value != null && value != DBNull.Value) {
					var siValue = (SI)value;
					if (filter == null || filter(siValue)) {
						result += siValue.Value() * ((Second)Data.Rows[i][(int)ModalResultField.simulationInterval]).Value();
					}
				}
			}

			return result.SI<T>();
		}

		public IEnumerable<T> GetValues<T>(ModalResultField key)
		{
			return GetValues<T>(Data.Columns[(int)key]);
		}

		public object this[ModalResultField key]
		{
			get { return CurrentRow[(int)key]; }
			set { CurrentRow[(int)key] = value; }
		}

		public object this[string auxId]
		{
			get { return CurrentRow[Auxiliaries[auxId]]; }
			set { CurrentRow[Auxiliaries[auxId]] = value; }
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SetDataValue(string fieldName, object value)
		{
			if (!Data.Columns.Contains(fieldName)) {
				_additionalColumns.Add(fieldName);
				Data.Columns.Add(fieldName);
			}
			if (value is double) {
				CurrentRow[fieldName] = string.Format(CultureInfo.InvariantCulture, "{0}", value);
			} else {
				CurrentRow[fieldName] = value;
			}
		}

		public Dictionary<string, DataColumn> Auxiliaries { get; set; }

		/// <summary>
		/// Adds a new auxiliary column into the mod data.
		/// </summary>
		/// <param name="id">The Aux-ID. This is the internal identification for the auxiliary.</param>
		/// <param name="columnName">(Optional) The column name in the mod file. Default: "P_aux_" + id</param>
		public void AddAuxiliary(string id, string columnName = null)
		{
			if (!string.IsNullOrWhiteSpace(id) && !Auxiliaries.ContainsKey(id)) {
				var col = Data.Columns.Add(columnName ?? ModalResultField.P_aux_ + id, typeof(SI));
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] =
					ModalResultField.P_aux_.GetAttribute().Decimals;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] =
					ModalResultField.P_aux_.GetAttribute().OutputFactor;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] =
					ModalResultField.P_aux_.GetAttribute().ShowUnit;

				Auxiliaries[id] = col;
			}
		}

		public void FinishSimulation()
		{
			//Data.Clear(); //.Rows.Clear();
			Data = null;
			CurrentRow = null;
			Auxiliaries.Clear();
		}
	}
}