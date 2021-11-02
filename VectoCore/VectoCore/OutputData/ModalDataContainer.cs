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
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData
{
	public class ModalDataContainer : IModalDataContainer
	{
		//private readonly bool _writeEngineOnly;
		private readonly IModalDataFilter[] _filters;
		private readonly Action<ModalDataContainer> _addReportResult;
		protected internal ModalResults Data { get; set; }
		private DataRow CurrentRow { get; set; }

		private readonly IModalDataWriter _writer;
		private readonly List<string> _additionalColumns = new List<string>();
		private Exception SimException;

		protected internal readonly Dictionary<IFuelProperties, Dictionary<ModalResultField, DataColumn>> FuelColumns = new Dictionary<IFuelProperties, Dictionary<ModalResultField, DataColumn>>();
		private Second _duration;
		private Meter _distance;

		
		protected Dictionary<int, Dictionary<ModalResultField, DataColumn>> BatteryColumns =
			new Dictionary<int, Dictionary<ModalResultField, DataColumn>>();

		public static readonly IList<ModalResultField> FuelConsumptionSignals = new[] {
			ModalResultField.FCMap, ModalResultField.FCNCVc, ModalResultField.FCWHTCc, // ModalResultField.FCAAUX,
			/*ModalResultField.FCICEStopStart,*/ ModalResultField.FCFinal
		};

		private readonly Dictionary<String, SI> _timeIntegrals = new Dictionary<string, SI>();
		private readonly Dictionary<FuelType, KilogramPerWattSecond> _engLine = new Dictionary<FuelType, KilogramPerWattSecond>();
		private readonly Dictionary<FuelType, KilogramPerWattSecond> _vehLine = new Dictionary<FuelType, KilogramPerWattSecond>();
		
		protected VectoRunData _runData;
		private ICorrectedModalData _correctedModalData;

		public IModalDataPostProcessor PostProcessingCorrection { set; protected get; }


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

			PostProcessingCorrection = new ModalDataPostprocessingCorrection();

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

		public int JobRunId => _runData.JobRunId;

		public string RunName => _runData.JobName;

		public string CycleName => _runData.Cycle?.Name ?? "";

		public string RunSuffix => _runData.ModFileSuffix;

		public bool WriteModalResults { get; set; }

		public VectoRun.Status RunStatus { get; protected set; }

		public string Error => SimException?.Message;

		public string StackTrace =>
			SimException == null
				? null
				: (SimException.StackTrace ?? SimException.InnerException?.StackTrace);


		public void Reset()
		{
			Data.Rows.Clear();
			CurrentRow = Data.NewRow();
			ClearAggregateResults();
		}

		public Second Duration => _duration ?? (_duration = CalcDuration());

		public Meter Distance => _distance ?? (_distance = CalcDistance());

		public Func<Second, Joule, Joule> AuxHeaterDemandCalc { get; set; }

		public KilogramPerWattSecond EngineLineCorrectionFactor(IFuelProperties fuel)
		{
			if (_engLine.TryGetValue(fuel.FuelType, out var value)) {
				return value;
			}

			VectoMath.LeastSquaresFitting(
				GetValues(
					x => x.Field<bool>(ModalResultField.ICEOn.GetName())
						? new Point(
							x.Field<SI>(ModalResultField.P_eng_fcmap.GetName()).Value(),
							x.Field<SI>(GetColumnName(fuel, ModalResultField.FCWHTCc)).Value())
						: null).Where(x => x != null && x.Y > 0).ToArray(),out var k, out var _, out var _);
			if (double.IsInfinity(k) || double.IsNaN(k)) {
				LogManager.GetLogger(typeof(ModalDataContainer).FullName).Warn("could not calculate engine correction line - k: {0}", k);
				k = 0;
			}

			_engLine[fuel.FuelType] = k.SI<KilogramPerWattSecond>();

			return _engLine[fuel.FuelType];
		}

		public KilogramPerWattSecond VehicleLineSlope(IFuelProperties fuel)
		{
			if (_vehLine.TryGetValue(fuel.FuelType, out var value)) {
				return value;
			}

			if (Data.AsEnumerable().Any(r => r.Field<SI>(ModalResultField.P_wheel_in.GetName()) != null)) {
				VectoMath.LeastSquaresFitting(
					GetValues(row => row.Field<bool>(ModalResultField.ICEOn.GetName())
								? new Point(
									row.Field<SI>(ModalResultField.P_wheel_in.GetName()).Value(),
									row.Field<SI>(GetColumnName(fuel, ModalResultField.FCFinal)).Value())
								: null)
						.Where(x => x != null && x.X > 0 && x.Y > 0).ToArray(), out var k, out var _, out var _);
				if (double.IsInfinity(k) || double.IsNaN(k)) {
					LogManager.GetLogger(typeof(ModalDataContainer).FullName).Warn("could not calculate vehicle correction line - k: {0}", k);
					k = 0;
				}

				_vehLine[fuel.FuelType] = k.SI<KilogramPerWattSecond>();
				return _vehLine[fuel.FuelType];
			}

			return null;
		}


		public ICorrectedModalData CorrectedModalData => _correctedModalData ?? (_correctedModalData = PostProcessingCorrection.ApplyCorrection(this, _runData));


		public void CalculateAggregateValues()
		{
			var duration = Duration;
			var distance = Distance;
			if (distance != null && duration != null && !duration.IsEqual(0)) {
				var speed = distance / duration;
			}

			//if (HasCombustionEngine) {
				foreach (var fuel in FuelColumns.Keys) {
					EngineLineCorrectionFactor(fuel);
					VehicleLineSlope(fuel);
					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCMap));
					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCNCVc));
					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCWHTCc));

					//TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCAAUX));
					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCMap));
					//TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCICEStopStart));
					TimeIntegral<Kilogram>(GetColumnName(fuel, ModalResultField.FCFinal));

				}

				TimeIntegral<WattSecond>(ModalResultField.P_WHR_el_corr);
				TimeIntegral<WattSecond>(ModalResultField.P_WHR_mech_corr);
				TimeIntegral<WattSecond>(ModalResultField.P_aux_ESS_mech_ice_off);
				TimeIntegral<WattSecond>(ModalResultField.P_aux_ESS_mech_ice_on);
				TimeIntegral<WattSecond>(ModalResultField.P_ice_start);
			//}

			TimeIntegral<WattSecond>(ModalResultField.P_clutch_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_gbx_shift_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_gbx_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_wheel_in);
			TimeIntegral<WattSecond>(ModalResultField.P_axle_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_ret_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_angle_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_TC_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_brake_loss);
			TimeIntegral<WattSecond>(ModalResultField.P_wheel_inertia);
			TimeIntegral<WattSecond>(ModalResultField.P_veh_inertia);
			TimeIntegral<WattSecond>(ModalResultField.P_aux);
			TimeIntegral<WattSecond>(ModalResultField.P_slope);
			TimeIntegral<WattSecond>(ModalResultField.P_roll);
			TimeIntegral<WattSecond>(ModalResultField.P_air);

		}

		
		public bool HasTorqueConverter => _runData.GearboxData?.TorqueConverterData != null;

		public void CommitSimulationStep()
		{
			Data.Rows.Add(CurrentRow);
			CurrentRow = Data.NewRow();
			ClearAggregateResults();
		}

		protected virtual void ClearAggregateResults()
		{
			_duration = null;
			_distance = null;
			_timeIntegrals.Clear();
			_correctedModalData = null;
		}

		protected virtual Second CalcDuration()
		{
			var time = GetValues<Second>(ModalResultField.time).ToList();
			var dt = GetValues<Second>(ModalResultField.simulationInterval).ToList();
			if (time.Count == 1) {
				return time.First();
			}
			return time.Last() - time.First() + dt.First() / 2 + dt.Last() / 2;
		}

		protected virtual Meter CalcDistance()
		{
			var max = GetValues<Meter>(ModalResultField.dist).LastOrDefault() ?? 0.SI<Meter>();
			var first = GetValues(
				r => new {
					dist = r.Field<Meter>(ModalResultField.dist.GetName()),
					vact = r.Field<MeterPerSecond>(ModalResultField.v_act.GetName()),
					acc = r.Field<MeterPerSquareSecond>(ModalResultField.acc.GetName()),
					dt = r.Field<Second>(ModalResultField.simulationInterval.GetName())
				}).First();
			var min = 0.SI<Meter>();
			if (first != null && first.vact != null && first.acc != null && first.dt != null) {
				min = first.dist - first.vact * first.dt - first.acc * first.dt * first.dt / 2.0;
			}

			return max == null || min == null ? null : max - min;
		}

		public IList<IFuelProperties> FuelData => FuelColumns.Keys.ToList();

		public void Finish(VectoRun.Status runStatus, Exception exception = null)
		{
			RunStatus = runStatus;
			SimException = exception;

			var dataColumns = GetOutputColumns();

			var strCols = dataColumns.Concat(Auxiliaries.Values.Where(x => !x.ColumnName.Contains("P_aux_ENG_AUX_"))
					.Select(c => c.ColumnName))
				.Concat(
					new[] {
						ModalResultField.P_WHR_el_map, ModalResultField.P_WHR_el_corr, ModalResultField.P_WHR_mech_map,
						ModalResultField.P_WHR_mech_corr, ModalResultField.P_aux_ESS_mech_ice_off, ModalResultField.P_aux_ESS_mech_ice_on,
						ModalResultField.P_ice_start //, ModalResultField.altitude
					}.Select(x => x.GetName()))
				.Concat(FuelColumns.SelectMany(kv => kv.Value.Select(kv2 => kv2.Value.ColumnName)));
			// TODO: 2018-11-20: Disable additional columns after testing gearshifting!
#if TRACE
			strCols = strCols.Concat(_additionalColumns);
			//dataColumns.Add(ModalResultField.altitude);
#endif
			strCols = strCols.Concat(new[] { ModalResultField.ICEOn }.Select(x => x.GetName()));
			if (_runData.JobType != VectoSimulationJobType.EngineOnlySimulation) {
				strCols = strCols.Concat(new[] {
					ModalResultField.DriverAction,
					ModalResultField.EcoRollConditionsMet,
					ModalResultField.PCCSegment,
					ModalResultField.PCCState
				}.Select(x => x.GetName()));
			}
			var fileSuffix = RunSuffix;
			if (WriteModalResults) {
				var filteredData = Data;
				foreach (var filter in _filters) {
					fileSuffix += "_" + filter.ID;
					filteredData = filter.Filter(filteredData);
				}

				try {
					_writer.WriteModData(
						JobRunId, RunName, CycleName, fileSuffix,
						new DataView(filteredData).ToTable(false, strCols.ToArray()));
				} catch (Exception e) {
					LogManager.GetLogger(typeof(ModalDataContainer).FullName).Error(e.Message);
				}
			}

			_addReportResult(this);
		}

		private IList<string> GetOutputColumns()
		{
			var dataColumns = new List<string> { ModalResultField.time.GetName() };
			var writeEngineOnly = _runData.JobType == VectoSimulationJobType.EngineOnlySimulation;

			if (!writeEngineOnly) {
				dataColumns.AddRange(
					new[] {
						ModalResultField.simulationInterval,
						ModalResultField.dist,
						ModalResultField.v_act,
						ModalResultField.v_targ,
						ModalResultField.acc,
						ModalResultField.grad,
						ModalResultField.altitude
			}.Select(x => x.GetName()));
			}
			if (!writeEngineOnly) {
				dataColumns.AddRange(
					new[] {
						ModalResultField.Gear,
					}.Select(x => x.GetName()));
				if (HasTorqueConverter) {
					dataColumns.AddRange(new[] { ModalResultField.TC_Locked }.Select(x => x.GetName()));
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
				}.Select(x => x.GetName()));
			if (HasTorqueConverter) {
				dataColumns.AddRange(
					new[] {
						ModalResultField.P_gbx_shift_loss,
						ModalResultField.P_TC_loss,
						ModalResultField.P_TC_out,
					}.Select(x => x.GetName()));
			} else {
				dataColumns.AddRange(
					new[] {
						ModalResultField.P_clutch_loss,
						ModalResultField.P_clutch_out,
					}.Select(x => x.GetName()));
			}
			dataColumns.AddRange(
				new[] {
					ModalResultField.P_aux,
					//ModalResultField.P_aux_el
				}.Select(x => x.GetName()));

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
						ModalResultField.T_gbx_out,
						//ModalResultField.T_gbx_in
					}.Select(x => x.GetName()));
				if (_runData.BusAuxiliaries != null) {
					dataColumns.AddRange(
						new[] {
							ModalResultField.P_busAux_ES_HVAC,
							ModalResultField.P_busAux_ES_other,
							ModalResultField.P_busAux_ES_consumer_sum,
							ModalResultField.P_busAux_ES_sum_mech,
							ModalResultField.P_busAux_ES_generated,
							ModalResultField.BatterySOC,
							ModalResultField.P_busAux_HVACmech_consumer,
							ModalResultField.P_busAux_HVACmech_gen,
							ModalResultField.Nl_busAux_PS_consumer,
							ModalResultField.Nl_busAux_PS_generated,
							ModalResultField.Nl_busAux_PS_generated_alwaysOn,
							//ModalResultField.Nl_busAux_PS_generated_dragOnly,
							ModalResultField.P_busAux_PS_generated,
							ModalResultField.P_busAux_PS_generated_alwaysOn,
							ModalResultField.P_busAux_PS_generated_dragOnly,
							//ModalResultField.P_DCDC_In,
							//ModalResultField.P_DCDC_Out,
							//ModalResultField.P_DCDC_missing,
						}.Select(x => x.GetName()));
				}
				if (HasTorqueConverter) {
					dataColumns.AddRange(
						new[] {
							ModalResultField.TorqueConverterSpeedRatio,
							ModalResultField.TorqueConverterTorqueRatio,
							ModalResultField.TC_TorqueOut,
							ModalResultField.TC_angularSpeedOut,
							ModalResultField.TC_TorqueIn,
							ModalResultField.TC_angularSpeedIn,
						}.Select(x => x.GetName()));
				}
			}
			//if (_runData.HybridStrategyParameters != null) {
			//	dataColumns.AddRange(new[] {
			//		ModalResultField.HybridStrategyScore, ModalResultField.HybridStrategySolution,
			//		ModalResultField.MaxPropulsionTorqe
			//	}.Select(x => x.GetName()));
			//}
			return dataColumns;
		}

		public IEnumerable<T> GetValues<T>(DataColumn col) => GetValues(x => x.Field<T>(col));

		public IEnumerable<T> GetValues<T>(Func<DataRow, T> selectorFunc) =>
			Data.Rows.Cast<DataRow>().Select(selectorFunc);

		public T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null) where T : SIBase<T> =>
			TimeIntegral<T>(field.GetName(), filter);

		public T TimeIntegral<T>(string field, Func<SI, bool> filter = null) where T : SIBase<T>
		{

			if (filter == null && _timeIntegrals.TryGetValue(field, out var val)) {
				return (T)val;
			}

			var dt = Data.Columns[ModalResultField.simulationInterval.GetName()];
			var result = 0.0;
			var idx = Data.Columns[field];
			foreach (DataRow row in Data.Rows) {
				var value = row[idx];
				if (value != null && value != DBNull.Value) {
					var siValue = (SI)value;
					if (filter == null || filter(siValue)) {
						result += siValue.Value() * ((Second)row[dt]).Value();
					}
				}
			}

			var retVal = result.SI<T>();
			if (filter == null) {
				_timeIntegrals[field] = retVal;
			}
			return retVal;
		}

		public IEnumerable<T> GetValues<T>(ModalResultField key)
		{
			return GetValues<T>(Data.Columns[key.GetName()]);
		}

		public object this[ModalResultField key] {
			get => CurrentRow[key.GetName()];
			set => CurrentRow[key.GetName()] = value;
		}

		public string GetColumnName(IFuelProperties fuelData, ModalResultField mrf)
		{
			try {
				return FuelColumns[fuelData][mrf].ColumnName;
			} catch (KeyNotFoundException e) {
				throw new VectoException($"unknown fuel {fuelData.GetLabel()} for key {mrf.GetName()}", e);
			}
		}

		public object this[ModalResultField key, IFuelProperties fuel] {
			get {
				try {
					return CurrentRow[FuelColumns[fuel][key]];
				} catch (KeyNotFoundException e) {
					throw new VectoException($"unknown fuel {fuel.GetLabel()} for key {key.GetName()}", e);
				}
			}
			set {
				try {
					CurrentRow[FuelColumns[fuel][key]] = value;
				} catch (KeyNotFoundException e) {
					throw new VectoException($"unknown fuel {fuel.GetLabel()} for key {key.GetName()}", e);
				}
			}
		}

		public object this[string auxId] {
			get => CurrentRow[Auxiliaries[auxId]];
			set => CurrentRow[Auxiliaries[auxId]] = value;
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
				var col = Data.Columns.Add(columnName ?? string.Format(ModalResultField.P_aux_.GetCaption(), id), typeof(SI));
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