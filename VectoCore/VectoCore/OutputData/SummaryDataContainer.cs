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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

// ReSharper disable MemberCanBePrivate.Global  -- used by API!

namespace TUGraz.VectoCore.OutputData
{
	public delegate void WriteSumData(IModalDataContainer data);

	/// <summary>
	/// Class for the sum file in vecto.
	/// </summary>
	public class SummaryDataContainer : LoggingObject, IDisposable
	{
		protected readonly string[] FcColumns = {
			Fields.FCMAP_H, Fields.FCMAP_KM,
			Fields.FCNCVC_H, Fields.FCNCVC_KM,
			Fields.FCWHTCC_H, Fields.FCWHTCC_KM,
			Fields.FCESS_H, Fields.FCESS_KM,
			Fields.FCESS_H_CORR, Fields.FCESS_KM_CORR,
			Fields.FC_BusAux_PS_CORR_H, Fields.FC_BusAux_PS_CORR_KM,
			Fields.FC_BusAux_ES_CORR_H, Fields.FC_BusAux_ES_CORR_KM,
			Fields.FCWHR_H_CORR, Fields.FCWHR_KM_CORR,
			Fields.FC_HEV_SOC_H, Fields.FC_HEV_SOC_KM,
			Fields.FC_HEV_SOC_CORR_H, Fields.FC_HEV_SOC_CORR_KM,
			Fields.FC_AUXHTR_H, Fields.FC_AUXHTR_KM,
			Fields.FC_AUXHTR_H_CORR, Fields.FC_AUXHTR_KM_CORR,
			Fields.FCFINAL_H, Fields.FCFINAL_KM, Fields.FCFINAL_LITERPER100KM, Fields.FCFINAL_LITERPER100TKM,
			Fields.FCFINAL_LiterPer100M3KM, Fields.FCFINAL_LiterPer100PassengerKM,
			Fields.SPECIFIC_FC, Fields.K_VEHLINE, Fields.K_ENGLINE
		};

		private object _tableLock = new object();
		internal readonly DataTable Table;
		private readonly ISummaryWriter _sumWriter;

		protected SummaryDataContainer() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SummaryDataContainer"/> class.
		/// </summary>
		/// <param name="writer"></param>
		public SummaryDataContainer(ISummaryWriter writer)
		{
			_sumWriter = writer;
			Table = new DataTable();
			InitTableColumns();
		}

		private void InitTableColumns()
		{
			lock (Table) {
				Table.Columns.AddRange(
					new[] {
						Tuple.Create(Fields.SORT, typeof(int)),
						Tuple.Create(Fields.JOB, typeof(string)),
						Tuple.Create(Fields.INPUTFILE, typeof(string)),
						Tuple.Create(Fields.CYCLE, typeof(string)),
						Tuple.Create(Fields.STATUS, typeof(string)),
						Tuple.Create(Fields.VEHICLE_MANUFACTURER, typeof(string)),
						Tuple.Create(Fields.VIN_NUMBER, typeof(string)),
						Tuple.Create(Fields.VEHICLE_MODEL, typeof(string)),
						Tuple.Create(Fields.HDV_CO2_VEHICLE_CLASS, typeof(string)),
						Tuple.Create(Fields.CURB_MASS, typeof(ConvertedSI)),
						Tuple.Create(Fields.LOADING, typeof(ConvertedSI)),
						Tuple.Create(Fields.PassengerCount, typeof(double)),
						Tuple.Create(Fields.TOTAL_VEHICLE_MASS, typeof(ConvertedSI)),
						Tuple.Create(Fields.ENGINE_MANUFACTURER, typeof(string)),
						Tuple.Create(Fields.ENGINE_MODEL, typeof(string)),
						Tuple.Create(Fields.ENGINE_FUEL_TYPE, typeof(string)),
						Tuple.Create(Fields.ENGINE_RATED_POWER, typeof(ConvertedSI)),
						Tuple.Create(Fields.ENGINE_IDLING_SPEED, typeof(ConvertedSI)),
						Tuple.Create(Fields.ENGINE_RATED_SPEED, typeof(ConvertedSI)),
						Tuple.Create(Fields.ENGINE_DISPLACEMENT, typeof(ConvertedSI)),
						Tuple.Create(Fields.ENGINE_WHTC_URBAN, typeof(string)),
						Tuple.Create(Fields.ENGINE_WHTC_RURAL, typeof(string)),
						Tuple.Create(Fields.ENGINE_WHTC_MOTORWAY, typeof(string)),
						Tuple.Create(Fields.ENGINE_BF_COLD_HOT, typeof(string)),
						Tuple.Create(Fields.ENGINE_CF_REG_PER, typeof(string)),
						Tuple.Create(Fields.ENGINE_ACTUAL_CORRECTION_FACTOR, typeof(string)),
						Tuple.Create(Fields.VEHICLE_FUEL_TYPE, typeof(string)),
						Tuple.Create(Fields.AIRDRAG_MODEL, typeof(string)),
						Tuple.Create(Fields.CD_x_A_DECLARED, typeof(ConvertedSI)),
						Tuple.Create(Fields.CD_x_A, typeof(ConvertedSI)),
						Tuple.Create(Fields.SLEEPER_CAB, typeof(string)),
						Tuple.Create(Fields.DECLARED_RRC_AXLE1, typeof(double)),
						Tuple.Create(Fields.DECLARED_FZISO_AXLE1, typeof(ConvertedSI)),
						Tuple.Create(Fields.DECLARED_RRC_AXLE2, typeof(double)),
						Tuple.Create(Fields.DECLARED_FZISO_AXLE2, typeof(ConvertedSI)),
						Tuple.Create(Fields.DECLARED_RRC_AXLE3, typeof(double)),
						Tuple.Create(Fields.DECLARED_FZISO_AXLE3, typeof(ConvertedSI)),
						Tuple.Create(Fields.DECLARED_RRC_AXLE4, typeof(double)),
						Tuple.Create(Fields.DECLARED_FZISO_AXLE4, typeof(ConvertedSI)),
						Tuple.Create(Fields.ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER, typeof(double)),
						Tuple.Create(Fields.ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER, typeof(double)),
						Tuple.Create(Fields.R_DYN, typeof(ConvertedSI)),
						Tuple.Create(Fields.NUM_AXLES_DRIVEN, typeof(int)),
						Tuple.Create(Fields.NUM_AXLES_NON_DRIVEN, typeof(int)),
						Tuple.Create(Fields.NUM_AXLES_TRAILER, typeof(int)),
						Tuple.Create(Fields.GEARBOX_MANUFACTURER, typeof(string)),
						Tuple.Create(Fields.GEARBOX_MODEL, typeof(string)),
						Tuple.Create(Fields.GEARBOX_TYPE, typeof(string)),
						Tuple.Create(Fields.GEAR_RATIO_FIRST_GEAR, typeof(ConvertedSI)),
						Tuple.Create(Fields.GEAR_RATIO_LAST_GEAR, typeof(ConvertedSI)),
						Tuple.Create(Fields.TORQUECONVERTER_MANUFACTURER, typeof(string)),
						Tuple.Create(Fields.TORQUECONVERTER_MODEL, typeof(string)),
						Tuple.Create(Fields.RETARDER_MANUFACTURER, typeof(string)),
						Tuple.Create(Fields.RETARDER_MODEL, typeof(string)),
						Tuple.Create(Fields.RETARDER_TYPE, typeof(string)),
						Tuple.Create(Fields.ANGLEDRIVE_MANUFACTURER, typeof(string)),
						Tuple.Create(Fields.ANGLEDRIVE_MODEL, typeof(string)),
						Tuple.Create(Fields.ANGLEDRIVE_RATIO, typeof(string)),
						Tuple.Create(Fields.AXLE_MANUFACTURER, typeof(string)),
						Tuple.Create(Fields.AXLE_MODEL, typeof(string)),
						Tuple.Create(Fields.AXLE_RATIO, typeof(ConvertedSI)),
						Tuple.Create(string.Format(Fields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.SteeringPump),
							typeof(string)),
						Tuple.Create(string.Format(Fields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.Fan),
							typeof(string)),
						Tuple.Create(
							string.Format(Fields.AUX_TECH_FORMAT,
								Constants.Auxiliaries.IDs.HeatingVentilationAirCondition),
							typeof(string)),
						Tuple.Create(string.Format(Fields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.PneumaticSystem),
							typeof(string)),
						Tuple.Create(string.Format(Fields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.ElectricSystem),
							typeof(string)),
						Tuple.Create(Fields.TCU_MODEL, typeof(string)),
						Tuple.Create(Fields.ADAS_TECHNOLOGY_COMBINATION, typeof(string)),
						Tuple.Create(Fields.PTO_TECHNOLOGY, typeof(string)),
						Tuple.Create(Fields.REESS_CAPACITY, typeof(string)),
						//Tuple.Create(PTO_OTHER_ELEMENTS, typeof(string)),
					}.Select(x => new DataColumn(x.Item1, x.Item2)).ToArray());

				Table.Columns.AddRange(
					new[] {
						Fields.CARGO_VOLUME, Fields.TIME, Fields.DISTANCE, Fields.SPEED, Fields.ALTITUDE_DELTA,
					}.Select(x => new DataColumn(x, typeof(ConvertedSI))).ToArray());


				Table.Columns.AddRange(
					new[] {
						Fields.CO2_KM, Fields.CO2_TKM, Fields.CO2_M3KM, Fields.CO2_PKM, Fields.P_WHEEL,
						Fields.P_WHEEL_POS, Fields.P_FCMAP, Fields.P_FCMAP_POS,
						Fields.E_FCMAP_POS, Fields.E_FCMAP_NEG, Fields.E_POWERTRAIN_INERTIA, Fields.E_AUX,
						Fields.E_AUX_EL_HV, Fields.E_CLUTCH_LOSS,
						Fields.E_TC_LOSS, Fields.E_SHIFT_LOSS, Fields.E_GBX_LOSS, Fields.E_RET_LOSS,
						Fields.E_ANGLE_LOSS,
						Fields.E_AXL_LOSS, Fields.E_BRAKE, Fields.E_VEHICLE_INERTIA, Fields.E_WHEEL, Fields.E_AIR,
						Fields.E_ROLL, Fields.E_GRAD,
						Fields.AirConsumed, Fields.AirGenerated, Fields.E_PS_CompressorOff, Fields.E_PS_CompressorOn,
						Fields.E_BusAux_ES_consumed, Fields.E_BusAux_ES_generated, Fields.Delta_E_BusAux_Battery,
						Fields.E_BusAux_PS_corr, Fields.E_BusAux_ES_mech_corr,
						Fields.E_BusAux_HVAC_Mech, Fields.E_BusAux_HVAC_El,
						Fields.E_BusAux_AuxHeater,
						Fields.E_WHR_EL, Fields.E_WHR_MECH, Fields.E_ICE_START, Fields.E_AUX_ESS_missing,
						Fields.NUM_ICE_STARTS, Fields.ACC,
						Fields.ACC_POS, Fields.ACC_NEG, Fields.ACC_TIMESHARE, Fields.DEC_TIMESHARE,
						Fields.CRUISE_TIMESHARE,
						Fields.MAX_SPEED, Fields.MAX_ACCELERATION, Fields.MAX_DECELERATION, Fields.AVG_ENGINE_SPEED,
						Fields.MAX_ENGINE_SPEED, Fields.NUM_GEARSHIFTS, Fields.STOP_TIMESHARE,
						Fields.ICE_FULL_LOAD_TIME_SHARE, Fields.ICE_OFF_TIME_SHARE,
						Fields.COASTING_TIME_SHARE, Fields.BRAKING_TIME_SHARE, Fields.AVERAGE_POS_ACC
					}.Select(x => new DataColumn(x, typeof(ConvertedSI))).ToArray());

				Table.Columns.AddRange(
					new[] {
						Tuple.Create(Fields.ENGINE_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(Fields.AVERAGE_ENGINE_EFFICIENCY, typeof(double)),
						Tuple.Create(Fields.TORQUE_CONVERTER_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(Fields.TORQUE_CONVERTER_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(Fields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITHOUT_LOCKUP, typeof(double)),
						Tuple.Create(Fields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITH_LOCKUP, typeof(double)),
						Tuple.Create(Fields.GEARBOX_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(Fields.GEARBOX_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(Fields.AVERAGE_GEARBOX_EFFICIENCY, typeof(double)),
						Tuple.Create(Fields.RETARDER_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(Fields.RETARDER_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(Fields.ANGLEDRIVE_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(Fields.ANGLEDRIVE_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(Fields.AVERAGE_ANGLEDRIVE_EFFICIENCY, typeof(double)),
						Tuple.Create(Fields.AXLEGEAR_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(Fields.AXLEGEAR_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(Fields.AVERAGE_AXLEGEAR_EFFICIENCY, typeof(double)),
						Tuple.Create(Fields.AIRDRAG_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(Fields.AIRDRAG_CERTIFICATION_METHOD, typeof(string)),
					}.Select(x => new DataColumn(x.Item1, x.Item2)).ToArray());
			}
		}

		/// <summary>
		/// Finishes the summary data container (writes the data to the sumWriter).
		/// </summary>
		public virtual void Finish()
		{
			if (_sumWriter != null) {
				lock (Table) {
					var view = new DataView(Table, "", Fields.SORT, DataViewRowState.CurrentRows).ToTable();

					var probablyEmptyCols = new[] { Fields.E_WHEEL, Fields.SPECIFIC_FC }.Select(x =>
						x.Contains("{") ? x.Substring(0, x.IndexOf("{", StringComparison.Ordinal)) : x).ToArray();
					var removeCandidates =
						view.Columns.Cast<DataColumn>()
							.Where(column => probablyEmptyCols.Any(x => column.ColumnName.StartsWith(x))).ToList();
					var toRemove = new List<string>();
					foreach (var column in removeCandidates) {
						//var column = view.Columns[colName];
						if (view.AsEnumerable().All(dr => dr.IsNull(column))) {
							toRemove.Add(column.ColumnName);
						}
					}

					toRemove = toRemove.Concat(
						view.Columns.Cast<DataColumn>()
							.Where(column => column.ColumnName.StartsWith(Fields.INTERNAL_PREFIX))
							.Select(x => x.ColumnName)).ToList();

					foreach (var dataColumn in toRemove) {
						view.Columns.Remove(dataColumn);
					}

					try {
						_sumWriter.WriteSumData(view);
					} catch (Exception e) {
						LogManager.GetLogger(typeof(SummaryDataContainer).FullName).Error(e.Message);
					}
				}
			}
		}

		private void UpdateTableColumns(ICollection<IFuelProperties> modDataFuelData,
			bool engineDataMultipleEngineFuelModes)
		{
			foreach (var entry in modDataFuelData) {
				foreach (var column in FcColumns.Reverse()) {
					var colName = string.Format(column,
						modDataFuelData.Count <= 1 && !engineDataMultipleEngineFuelModes
							? ""
							: "_" + entry.FuelType.GetLabel());
					lock (Table) {
						if (!Table.Columns.Contains(colName)) {
							var col = new DataColumn(colName, typeof(ConvertedSI));
							Table.Columns.Add(col);
							col.SetOrdinal(Table.Columns.IndexOf(Fields.ALTITUDE_DELTA) + 1);
						}
					}
				}
			}
		}

		/// <summary>
		/// Writes the result of one run into the summary data container.
		/// </summary>
		//[MethodImpl(MethodImplOptions.Synchronized)]
		protected DataRow GetResultRow(IModalDataContainer modData, VectoRunData runData)
		{
			lock (_tableLock) {
				if (modData.HasCombustionEngine) {
					UpdateTableColumns(modData.FuelData, runData.EngineData.MultipleEngineFuelModes);
				}
			}

			lock (Table) {
				var row = Table.NewRow();
				//Table.Rows.Add(row);
				return row;
			}
		}

		protected Dictionary<string, object> GetResultDictionary(IModalDataContainer modData, VectoRunData runData)
		{
			if (modData.HasCombustionEngine) {
				lock (_tableLock) {
					UpdateTableColumns(modData.FuelData, runData.EngineData.MultipleEngineFuelModes);
				}
			}

			return new Dictionary<string, object>();
		}

		protected void AddResultRow(DataRow row)
		{
			lock (_tableLock) {
				Table.Rows.Add(row);
			}
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
	

	//[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual void Write(IModalDataContainer modData, int jobNr, int runNr, VectoRunData runData)
		{
			//var row = GetResultRow(modData, runData); // Replace row with dictionary

			var row = GetResultDictionary(modData, runData);
			row[Fields.SORT] = jobNr * 1000 + runNr;
			row[Fields.JOB] = $"{jobNr}-{runNr}"; //ReplaceNotAllowedCharacters(current);
			row[Fields.INPUTFILE] = ReplaceNotAllowedCharacters(runData.JobName);
			row[Fields.CYCLE] = ReplaceNotAllowedCharacters(runData.Cycle.Name + Constants.FileExtensions.CycleFile);

			row[Fields.STATUS] = modData.RunStatus;

			var vehicleLoading = 0.SI<Kilogram>();
			var cargoVolume = 0.SI<CubicMeter>();
			var gearCount = 0u;
			double? passengerCount = null;
			if (runData.Cycle.CycleType != CycleType.EngineOnly) {
				WriteFullPowertrain(runData, row);

				cargoVolume = runData.VehicleData.CargoVolume;
				vehicleLoading = runData.VehicleData.Loading;
				gearCount = (uint?)runData.GearboxData?.Gears.Count ?? 0u;
				passengerCount = runData.VehicleData.PassengerCount;
			}

			row[Fields.VEHICLE_FUEL_TYPE] = modData.FuelData.Select(x => x.GetLabel()).Join();

			var totalTime = modData.Duration;
			row[Fields.TIME] = (ConvertedSI)totalTime;

			var distance = modData.Distance;
			if (distance != null) {
				row[Fields.DISTANCE] = distance.ConvertToKiloMeter();
			}

			var speed = modData.Speed();
			if (speed != null) {
				row[Fields.SPEED] = speed.ConvertToKiloMeterPerHour();
			}

			row[Fields.ALTITUDE_DELTA] = (ConvertedSI)modData.AltitudeDelta();

			if (modData.HasCombustionEngine) {
				WriteFuelConsumptionEntries(modData, row, vehicleLoading, cargoVolume, passengerCount, runData);
			} else {
				if (runData.ElectricMachinesData.Count > 0) {
					lock (Table) {
						if (!Table.Columns.Contains(Fields.ElectricEnergyConsumptionPerKm)) {
							lock (_tableLock) {
								var col = Table.Columns.Add(Fields.ElectricEnergyConsumptionPerKm, typeof(ConvertedSI));
								col.SetOrdinal(Table.Columns[Fields.CO2_KM].Ordinal);
							}
						}
					}

					row[Fields.ElectricEnergyConsumptionPerKm] =
						(-modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int) / modData.Distance).Cast<JoulePerMeter>().ConvertToKiloWattHourPerKiloMeter();
				}
			}

			if (runData.Mission?.MissionType == MissionType.VerificationTest) {
				var fuelsWhtc = runData.EngineData.Fuels.Select(
											fuel => modData.TimeIntegral<Kilogram>(modData.GetColumnName(fuel.FuelData, ModalResultField.FCWHTCc)) /
													modData.TimeIntegral<Kilogram>(modData.GetColumnName(fuel.FuelData, ModalResultField.FCMap)))
										.Select(dummy => (double)dummy).ToArray();
				row[Fields.ENGINE_ACTUAL_CORRECTION_FACTOR] = fuelsWhtc.Join(" / ");
			}

			row[Fields.P_WHEEL_POS] = modData.PowerWheelPositive().ConvertToKiloWatt();
			row[Fields.P_WHEEL] = modData.PowerWheel().ConvertToKiloWatt();

			if (modData.HasCombustionEngine) {
				row[Fields.P_FCMAP_POS] = modData.TotalPowerEnginePositiveAverage().ConvertToKiloWatt();
				row[Fields.P_FCMAP] = modData.TotalPowerEngineAverage().ConvertToKiloWatt();
			}

			WriteAuxiliaries(modData, row, runData.BusAuxiliaries != null);

			WriteWorkEntries(modData, row, runData);

			WritePerformanceEntries(runData, modData, row);

			row[Fields.ICE_FULL_LOAD_TIME_SHARE] = (ConvertedSI)modData.ICEMaxLoadTimeShare();
			row[Fields.ICE_OFF_TIME_SHARE] = (ConvertedSI)modData.ICEOffTimeShare();
			row[Fields.COASTING_TIME_SHARE] = (ConvertedSI)modData.CoastingTimeShare();
			row[Fields.BRAKING_TIME_SHARE] = (ConvertedSI)modData.BrakingTimeShare();

			if (runData.EngineData != null) {
				row[Fields.NUM_ICE_STARTS] = (ConvertedSI)modData.NumICEStarts().SI<Scalar>();
			}

			if (gearCount > 0) {
				WriteGearshiftStats(modData, row, gearCount);
			}

			//AddResultRow(row); //Add dictionary to datatable
			AddResultDictionary(row);
		}


		private void WriteFuelConsumptionEntries(
			IModalDataContainer modData, Dictionary<string, object> row, Kilogram vehicleLoading,
			CubicMeter cargoVolume, double? passengers, VectoRunData runData)
		{
			var multipleEngineModes = runData.EngineData.MultipleEngineFuelModes;
			var vtpCycle = runData.Cycle.CycleType == CycleType.VTP;


			row[Fields.E_WHR_EL] = modData.CorrectedModalData.WorkWHREl.ConvertToKiloWattHour();
			row[Fields.E_WHR_MECH] = modData.CorrectedModalData.WorkWHRMech.ConvertToKiloWattHour();

			row[Fields.E_BusAux_PS_corr] = modData.CorrectedModalData.WorkBusAuxPSCorr.ConvertToKiloWattHour();
			row[Fields.E_BusAux_ES_mech_corr] = modData.CorrectedModalData.WorkBusAuxESMech.ConvertToKiloWattHour();

			row[Fields.E_BusAux_AuxHeater] = modData.CorrectedModalData.AuxHeaterDemand.Cast<WattSecond>().ConvertToKiloWattHour();

			foreach (var fuel in modData.FuelData) {
				var suffix = modData.FuelData.Count <= 1 && !multipleEngineModes ? "" : "_" + fuel.FuelType.GetLabel();

				row[FcCol(Fields.FCMAP_H, suffix)] =
					modData.FuelConsumptionPerSecond(ModalResultField.FCMap, fuel)?.ConvertToGrammPerHour();
				row[FcCol(Fields.FCMAP_KM, suffix)] =
					modData.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel)?.ConvertToGrammPerKiloMeter();


				row[FcCol(Fields.FCNCVC_H, suffix)] =
					modData.FuelConsumptionPerSecond(ModalResultField.FCNCVc, fuel)?.ConvertToGrammPerHour();
				row[FcCol(Fields.FCNCVC_KM, suffix)] =
					modData.FuelConsumptionPerMeter(ModalResultField.FCNCVc, fuel)?.ConvertToGrammPerKiloMeter();

				row[FcCol(Fields.FCWHTCC_H, suffix)] =
					modData.FuelConsumptionPerSecond(ModalResultField.FCWHTCc, fuel)?.ConvertToGrammPerHour();
				row[FcCol(Fields.FCWHTCC_KM, suffix)] =
					modData.FuelConsumptionPerMeter(ModalResultField.FCWHTCc, fuel)?.ConvertToGrammPerKiloMeter();

				//modData.FuelConsumptionPerSecond(ModalResultField.FCICEStopStart, fuel)
				//                                               ?.ConvertToGrammPerHour();
				//modData.FuelConsumptionPerMeter(ModalResultField.FCICEStopStart, fuel)
				//                                               ?.ConvertToGrammPerKiloMeter();

				var fuelConsumption = modData.CorrectedModalData.FuelConsumptionCorrection(fuel);

				row[FcCol(Fields.K_ENGLINE, suffix)] = fuelConsumption.EngineLineCorrectionFactor.ConvertToGramPerKiloWattHour();
				row[FcCol(Fields.K_VEHLINE, suffix)] = fuelConsumption.VehicleLine?.ConvertToGramPerKiloWattHour();

				var vehLine = modData.VehicleLineSlope(fuel);
				if (vehLine != null) {
					row[FcCol(Fields.K_VEHLINE, suffix)] = vehLine.ConvertToGramPerKiloWattHour();
				}

				row[FcCol(Fields.FCESS_H, suffix)] = fuelConsumption.FC_ESS_H?.ConvertToGrammPerHour();
				row[FcCol(Fields.FCESS_H_CORR, suffix)] = fuelConsumption.FC_ESS_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(Fields.FC_BusAux_PS_CORR_H, suffix)] = fuelConsumption.FC_BusAux_PS_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(Fields.FC_BusAux_ES_CORR_H, suffix)] = fuelConsumption.FC_BusAux_ES_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(Fields.FCWHR_H_CORR, suffix)] = fuelConsumption.FC_WHR_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(Fields.FC_HEV_SOC_CORR_H, suffix)] = fuelConsumption.FC_REESS_SOC_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(Fields.FC_HEV_SOC_H, suffix)] = fuelConsumption.FC_REESS_SOC_H?.ConvertToGrammPerHour();
				row[FcCol(Fields.FC_AUXHTR_H, suffix)] = fuelConsumption.FC_AUXHTR_H?.ConvertToGrammPerHour();
				row[FcCol(Fields.FC_AUXHTR_H_CORR, suffix)] = fuelConsumption.FC_AUXHTR_H_CORR?.ConvertToGrammPerHour();

				row[FcCol(Fields.FCFINAL_H, suffix)] = fuelConsumption.FC_FINAL_H?.ConvertToGrammPerHour();

				row[FcCol(Fields.FCWHR_KM_CORR, suffix)] = fuelConsumption.FC_WHR_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(Fields.FC_BusAux_PS_CORR_KM, suffix)] = fuelConsumption.FC_BusAux_PS_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(Fields.FC_BusAux_ES_CORR_KM, suffix)] = fuelConsumption.FC_BusAux_ES_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(Fields.FC_HEV_SOC_CORR_KM, suffix)] = fuelConsumption.FC_REESS_SOC_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(Fields.FC_HEV_SOC_KM, suffix)] = fuelConsumption.FC_REESS_SOC_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(Fields.FC_AUXHTR_KM, suffix)] = fuelConsumption.FC_AUXHTR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(Fields.FC_AUXHTR_KM_CORR, suffix)] = fuelConsumption.FC_AUXHTR_KM_CORR?.ConvertToGrammPerKiloMeter();

				row[FcCol(Fields.FCESS_KM, suffix)] = fuelConsumption.FC_ESS_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(Fields.FCESS_KM_CORR, suffix)] = fuelConsumption.FC_ESS_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(Fields.FCFINAL_KM, suffix)] = fuelConsumption.FC_FINAL_KM?.ConvertToGrammPerKiloMeter();

				if (fuel.FuelDensity != null) {

					var fcVolumePerMeter = fuelConsumption.FuelVolumePerMeter;
					row[FcCol(Fields.FCFINAL_LITERPER100KM, suffix)] = fcVolumePerMeter?.ConvertToLiterPer100Kilometer();

					if (vehicleLoading != null && !vehicleLoading.IsEqual(0) && fcVolumePerMeter != null) {
						row[FcCol(Fields.FCFINAL_LITERPER100TKM, suffix)] =
							(fcVolumePerMeter / vehicleLoading).ConvertToLiterPer100TonKiloMeter();
					}
					if (cargoVolume > 0 && fcVolumePerMeter != null) {
						row[FcCol(Fields.FCFINAL_LiterPer100M3KM, suffix)] =
							(fcVolumePerMeter / cargoVolume).ConvertToLiterPerCubicMeter100KiloMeter();
					}

					if (passengers != null && fcVolumePerMeter != null) {
						// subtract driver!
						row[FcCol(Fields.FCFINAL_LiterPer100PassengerKM, suffix)] =
							(fcVolumePerMeter / passengers.Value).ConvertToLiterPer100Kilometer();
					}
				}

				if (vtpCycle) {
					row[FcCol(Fields.SPECIFIC_FC, suffix)] = (modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel) / modData.WorkWheelsPos())
						.ConvertToGramPerKiloWattHour();
				}
			}



			row[Fields.CO2_KM] = modData.CorrectedModalData.KilogramCO2PerMeter.ConvertToGrammPerKiloMeter();
			if (vehicleLoading != null && !vehicleLoading.IsEqual(0)) {
				row[Fields.CO2_TKM] = (modData.CorrectedModalData.KilogramCO2PerMeter / vehicleLoading).ConvertToGrammPerTonKilometer();
			}
			if (cargoVolume > 0) {
				row[Fields.CO2_M3KM] = (modData.CorrectedModalData.KilogramCO2PerMeter / cargoVolume).ConvertToGrammPerCubicMeterKiloMeter();
			}
			if (passengers != null) {
				row[Fields.CO2_PKM] = (modData.CorrectedModalData.KilogramCO2PerMeter / passengers.Value).ConvertToGrammPerKiloMeter();
			}
		}

		private static string FcCol(string col, string suffix)
		{
			return string.Format(col, suffix);
		}

		private void WriteAuxiliaries(IModalDataContainer modData, Dictionary<string, object> row, bool writeBusAux)
		{
			foreach (var aux in modData.Auxiliaries) {
				string colName;
				if (aux.Key == Constants.Auxiliaries.IDs.PTOConsumer || aux.Key == Constants.Auxiliaries.IDs.PTOTransmission) {
					colName = string.Format(Fields.E_FORMAT, aux.Key);
				} else {
					colName = string.Format(Fields.E_AUX_FORMAT, aux.Key);
				}

				lock (Table)
					if (!Table.Columns.Contains(colName)) {
					lock (_tableLock) {
						var col = Table.Columns.Add(colName, typeof(ConvertedSI));

						// move the new column to correct position
						col.SetOrdinal(Table.Columns[Fields.E_AUX].Ordinal);
					}
				}

				row[colName] = modData.AuxiliaryWork(aux.Value).ConvertToKiloWattHour();
			}

			if (writeBusAux) {
				row[Fields.E_BusAux_HVAC_Mech] = modData.TimeIntegral<WattSecond>(ModalResultField.P_busAux_HVACmech_consumer).ConvertToKiloWattHour();
				row[Fields.E_BusAux_HVAC_El] = modData.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_HVAC).ConvertToKiloWattHour();
			}
		}

		private void WriteGearshiftStats(IModalDataContainer modData, Dictionary<string, object> row, uint gearCount)
		{
			row[Fields.NUM_GEARSHIFTS] = gearCount == 1 ? 0.SI<Scalar>() : (ConvertedSI)modData.GearshiftCount();
			var timeSharePerGear = modData.TimeSharePerGear(gearCount);

			for (uint i = 0; i <= gearCount; i++) {
				var colName = string.Format(Fields.TIME_SHARE_PER_GEAR_FORMAT, i);
				lock (Table)
					if (!Table.Columns.Contains(colName)) {
					lock (_tableLock) {
						Table.Columns.Add(colName, typeof(ConvertedSI));
					}
				}
				row[colName] = (ConvertedSI)timeSharePerGear[i];
			}
		}

		private void WritePerformanceEntries(VectoRunData runData, IModalDataContainer modData, Dictionary<string, object> row)
		{
			row[Fields.ACC] = (ConvertedSI)modData.AccelerationAverage();
			row[Fields.ACC_POS] = (ConvertedSI)modData.AccelerationsPositive();
			row[Fields.ACC_NEG] = (ConvertedSI)modData.AccelerationsNegative();
			var accTimeShare = modData.AccelerationTimeShare();
			row[Fields.ACC_TIMESHARE] = (ConvertedSI)accTimeShare;
			var decTimeShare = modData.DecelerationTimeShare();
			row[Fields.DEC_TIMESHARE] = (ConvertedSI)decTimeShare;
			var cruiseTimeShare = modData.CruiseTimeShare();
			row[Fields.CRUISE_TIMESHARE] = (ConvertedSI)cruiseTimeShare;
			var stopTimeShare = modData.StopTimeShare();
			row[Fields.STOP_TIMESHARE] = (ConvertedSI)stopTimeShare;

			row[Fields.MAX_SPEED] = (ConvertedSI)modData.MaxSpeed().AsKmph.SI<Scalar>();
			row[Fields.MAX_ACCELERATION] = (ConvertedSI)modData.MaxAcceleration();
			row[Fields.MAX_DECELERATION] = (ConvertedSI)modData.MaxDeceleration();
			if (runData.EngineData != null) {
				row[Fields.AVG_ENGINE_SPEED] = (ConvertedSI)modData.AvgEngineSpeed().AsRPM.SI<Scalar>();
				row[Fields.MAX_ENGINE_SPEED] = (ConvertedSI)modData.MaxEngineSpeed().AsRPM.SI<Scalar>();
			}

			row[Fields.AVERAGE_POS_ACC] = (ConvertedSI)modData.AverageAccelerationBelowTargetSpeed();
			if (accTimeShare != null && decTimeShare != null && cruiseTimeShare != null) {
				var shareSum = accTimeShare + decTimeShare + cruiseTimeShare + stopTimeShare;
				if (!shareSum.IsEqual(100, 1e-2)) {
					Log.Warn(
						"Sumfile Error: driving behavior timeshares must sum up to 100%: acc: {0}%, dec: {1}%, cruise: {2}%, stop: {3}%, sum: {4}%",
						accTimeShare.ToOutputFormat(1, null, false), decTimeShare.ToOutputFormat(1, null, false),
						cruiseTimeShare.ToOutputFormat(1, null, false), stopTimeShare.ToOutputFormat(1, null, false),
						shareSum.ToOutputFormat(1, null, false));
				}
			}

			var eFC = 0.SI<Joule>();
			foreach (var fuel in modData.FuelData) {
				eFC += modData.TimeIntegral<Kilogram>(modData.GetColumnName(fuel, ModalResultField.FCFinal)) * fuel.LowerHeatingValueVecto;
			}
			var eIcePos = modData.TimeIntegral<WattSecond>(ModalResultField.P_ice_fcmap, x => x > 0);
			row[Fields.AVERAGE_ENGINE_EFFICIENCY] = eFC.IsEqual(0, 1e-9) ? 0 : (eIcePos / eFC).Value();

			if (runData.SimulationType == SimulationType.EngineOnly) {
				return;
			}

			var gbxOutSignal = runData.Retarder != null && runData.Retarder.Type == RetarderType.TransmissionOutputRetarder
				? ModalResultField.P_retarder_in
				: (runData.AngledriveData == null ? ModalResultField.P_axle_in : ModalResultField.P_angle_in);
			var eGbxIn = modData.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, x => x > 0);
			var eGbxOut = modData.TimeIntegral<WattSecond>(gbxOutSignal, x => x > 0);
			row[Fields.AVERAGE_GEARBOX_EFFICIENCY] = eGbxIn.IsEqual(0, 1e-9) ? 0 : (eGbxOut / eGbxIn).Value();

			if (runData.GearboxData != null && runData.GearboxData.Type.AutomaticTransmission() && runData.GearboxData.Type != GearboxType.APTN && runData.GearboxData.Type != GearboxType.IHPC) {
				var eTcIn = modData.TimeIntegral<WattSecond>(ModalResultField.P_TC_in, x => x > 0);
				var eTcOut = eGbxIn;
				row[Fields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITHOUT_LOCKUP] = eTcIn.IsEqual(0, 1e-9) ? 0 : (eTcOut / eTcIn).Value();

				var tcData = modData.GetValues(
					x => new {
						dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()),
						locked = x.Field<int>(ModalResultField.TC_Locked.GetName()),
						P_TCin = x.Field<Watt>(ModalResultField.P_TC_in.GetName()),
						P_TCout = x.Field<Watt>(ModalResultField.P_TC_out.GetName())
					});
				eTcIn = 0.SI<WattSecond>();
				eTcOut = 0.SI<WattSecond>();
				foreach (var entry in tcData.Where(x => x.locked == 0)) {
					eTcIn += entry.dt * entry.P_TCin;
					eTcOut += entry.dt * entry.P_TCout;
				}

				row[Fields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITH_LOCKUP] = eTcIn.IsEqual(0, 1e-9) ? 0 : (eTcOut / eTcIn).Value();
			}

			if (runData.AngledriveData != null) {
				var eAngleIn = modData.TimeIntegral<WattSecond>(ModalResultField.P_angle_in, x => x > 0);
				var eAngleOut = modData.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, x => x > 0);

				row[Fields.AVERAGE_ANGLEDRIVE_EFFICIENCY] = (eAngleOut / eAngleIn).Value();
			}

			var eAxlIn = modData.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, x => x > 0);
			var eAxlOut = modData.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, x => x > 0);
			row[Fields.AVERAGE_AXLEGEAR_EFFICIENCY] = eAxlIn.IsEqual(0, 1e-9) ? 0 : (eAxlOut / eAxlIn).Value();
		}

		private void WriteWorkEntries(IModalDataContainer modData, Dictionary<string, object> row, VectoRunData runData)
		{
			row[Fields.E_FCMAP_POS] = modData.TotalEngineWorkPositive().ConvertToKiloWattHour();
			row[Fields.E_FCMAP_NEG] = (-modData.TotalEngineWorkNegative()).ConvertToKiloWattHour();
			row[Fields.E_POWERTRAIN_INERTIA] = modData.PowerAccelerations().ConvertToKiloWattHour();
			row[Fields.E_AUX] = modData.WorkAuxiliaries().ConvertToKiloWattHour();
			row[Fields.E_AUX_EL_HV] = modData.TimeIntegral<WattSecond>(ModalResultField.P_aux_el).ConvertToKiloWattHour();
			row[Fields.E_CLUTCH_LOSS] = modData.WorkClutch().ConvertToKiloWattHour();
			row[Fields.E_TC_LOSS] = modData.WorkTorqueConverter().ConvertToKiloWattHour();
			row[Fields.E_SHIFT_LOSS] = modData.WorkGearshift().ConvertToKiloWattHour();
			row[Fields.E_GBX_LOSS] = modData.WorkGearbox().ConvertToKiloWattHour();
			row[Fields.E_RET_LOSS] = modData.WorkRetarder().ConvertToKiloWattHour();
			row[Fields.E_AXL_LOSS] = modData.WorkAxlegear().ConvertToKiloWattHour();
			row[Fields.E_ANGLE_LOSS] = modData.WorkAngledrive().ConvertToKiloWattHour();
			row[Fields.E_BRAKE] = modData.WorkTotalMechanicalBrake().ConvertToKiloWattHour();
			row[Fields.E_VEHICLE_INERTIA] = modData.WorkVehicleInertia().ConvertToKiloWattHour();
			row[Fields.E_AIR] = modData.WorkAirResistance().ConvertToKiloWattHour();
			row[Fields.E_ROLL] = modData.WorkRollingResistance().ConvertToKiloWattHour();
			row[Fields.E_GRAD] = modData.WorkRoadGradientResistance().ConvertToKiloWattHour();
			row[Fields.E_AUX_ESS_missing] = modData.CorrectedModalData.WorkESSMissing.ConvertToKiloWattHour();
			if (runData.Cycle.CycleType == CycleType.VTP) {
				row[Fields.E_WHEEL] = modData.WorkWheels().ConvertToKiloWattHour();
			}

			if (runData.BusAuxiliaries != null) {
				row[Fields.AirGenerated] = (ConvertedSI)modData.AirGenerated();
				row[Fields.AirConsumed] = (ConvertedSI)modData.AirConsumed();
				row[Fields.E_PS_CompressorOff] = modData.EnergyPneumaticCompressorPowerOff().ConvertToKiloWattHour();
				row[Fields.E_PS_CompressorOn] = modData.EnergyPneumaticCompressorOn().ConvertToKiloWattHour();

				row[Fields.E_BusAux_ES_generated] = modData.EnergyBusAuxESGenerated().ConvertToKiloWattHour();
				row[Fields.E_BusAux_ES_consumed] = modData.EnergyBusAuxESConsumed().ConvertToKiloWattHour();
				row[Fields.Delta_E_BusAux_Battery] = (runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
						? modData.DeltaSOCBusAuxBattery() * runData.BusAuxiliaries.ElectricalUserInputsConfig.ElectricStorageCapacity
						: 0.SI<WattSecond>())
					.ConvertToKiloWattHour();
			}

			row[Fields.E_ICE_START] = modData.WorkEngineStart().ConvertToKiloWattHour();

			foreach (var em in runData.ElectricMachinesData) {
				var fields = em.Item1 == PowertrainPosition.IEPC
					? GetIEPCWorkEntries(modData, row, em)
					: GetElectricMachineWorkEntries(modData, row, em);
				fields.Reverse();
				foreach (var entry in fields) {
					var colName = string.Format(entry.Item1, em.Item1.GetName());
					lock (Table) {
						if (!Table.Columns.Contains(colName)) {
							lock (_tableLock) {
								var col = Table.Columns.Add(colName, typeof(ConvertedSI));
								col.SetOrdinal(Table.Columns[Fields.E_GRAD].Ordinal + 1);
							}
						}
					}

					row[colName] = entry.Item2;
				}
			}

			if (runData.BatteryData != null || runData.SuperCapData != null) {
				foreach (var field in new[] { Fields.REESS_StartSoC, Fields.REESS_EndSoC }) {
					lock (Table) {
						if (Table.Columns.Contains(field)) {
							continue;
						}

						lock (_tableLock) {
							var col = Table.Columns.Add(field, typeof(double));
							col.SetOrdinal(Table.Columns[Fields.P_WHEEL].Ordinal);
						}
					}
				}

				foreach (var field in new[] {
					Fields.REESS_DeltaEnergy, Fields.E_REESS_LOSS, Fields.E_REESS_T_chg, Fields.E_REESS_T_dischg,
					Fields.E_REESS_int_chg, Fields.E_REESS_int_dischg
				}) {
					lock (Table) {
						if (Table.Columns.Contains(field)) {
							continue;
						}

						lock (_tableLock) {
							var col = Table.Columns.Add(field, typeof(ConvertedSI));
							col.SetOrdinal(Table.Columns[Fields.P_WHEEL].Ordinal);
						}
					}
				}
				row[Fields.E_REESS_LOSS] = modData.REESSLoss().ConvertToKiloWattHour();
				row[Fields.E_REESS_T_chg] = modData.WorkREESSChargeTerminal().ConvertToKiloWattHour();
				row[Fields.E_REESS_T_dischg] = modData.WorkREESSDischargeTerminal().ConvertToKiloWattHour();
				row[Fields.E_REESS_int_chg] = modData.WorkREESSChargeInternal().ConvertToKiloWattHour();
				row[Fields.E_REESS_int_dischg] = modData.WorkREESSDischargeInternal().ConvertToKiloWattHour();
			}

			if (runData.BatteryData != null) {
				row[Fields.REESS_StartSoC] = runData.BatteryData.InitialSoC * 100;
				row[Fields.REESS_EndSoC] = modData.REESSEndSoC();
				row[Fields.REESS_DeltaEnergy] = modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int.GetName())
					.ConvertToKiloWattHour();

			}
			if (runData.SuperCapData != null) {
				row[Fields.REESS_StartSoC] = runData.SuperCapData.InitialSoC * 100;
				row[Fields.REESS_EndSoC] = modData.REESSEndSoC();
				row[Fields.REESS_DeltaEnergy] = modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int.GetName())
					.ConvertToKiloWattHour();
			}
		}

		private List<Tuple<string, ConvertedSI>> GetIEPCWorkEntries(IModalDataContainer modData, Dictionary<string, object> row, Tuple<PowertrainPosition, ElectricMotorData> em)
		{
			var emColumns = new List<Tuple<string, ConvertedSI>>() {
				Tuple.Create(Fields.IEPC_AVG_SPEED_FORMAT,
					modData.ElectricMotorAverageSpeed(em.Item1).ConvertToRoundsPerMinute()),

				//Tuple.Create(Fields.E_EM_Mot_DRIVE_FORMAT,
				//	modData.TotalElectricMotorMotWorkDrive(em.Item1).ConvertToKiloWattHour()),
				//Tuple.Create(Fields.E_EM_Mot_GENERATE_FORMAT,
				//	modData.TotalElectricMotorMotWorkRecuperate(em.Item1).ConvertToKiloWattHour()),

				//Tuple.Create(Fields.ETA_EM_Mot_DRIVE_FORMAT,
				//	new ConvertedSI(modData.ElectricMotorMotEfficiencyDrive(em.Item1), "")),
				//Tuple.Create(Fields.ETA_EM_Mot_GEN_FORMAT,
				//	new ConvertedSI(modData.ElectricMotorMotEfficiencyGenerate(em.Item1), "")),


				Tuple.Create(Fields.E_IEPC_DRIVE_FORMAT, modData.TotalElectricMotorWorkDrive(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(Fields.E_IEPC_GENERATE_FORMAT,
					modData.TotalElectricMotorWorkRecuperate(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(Fields.ETA_IEPC_DRIVE_FORMAT, new ConvertedSI(modData.ElectricMotorEfficiencyDrive(em.Item1), "")),
				Tuple.Create(Fields.ETA_IEPC_GEN_FORMAT, new ConvertedSI(modData.ElectricMotorEfficiencyGenerate(em.Item1), "")),

				Tuple.Create(Fields.E_IEPC_OFF_Loss_Format, modData.ElectricMotorOffLosses(em.Item1).ConvertToKiloWattHour()),
				//Tuple.Create(Fields.E_EM_LOSS_TRANSM_FORMAT,
				//	modData.ElectricMotorTransmissionLosses(em.Item1).ConvertToKiloWattHour()),
				//Tuple.Create(Fields.E_EM_Mot_LOSS_FORMAT, modData.ElectricMotorMotLosses(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(Fields.E_IEPC_LOSS_FORMAT, modData.ElectricMotorLosses(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(Fields.E_IEPC_OFF_TIME_SHARE, (ConvertedSI)modData.ElectricMotorOffTimeShare(em.Item1))
			};
			return emColumns;
		}

		private List<Tuple<string, ConvertedSI>> GetElectricMachineWorkEntries(IModalDataContainer modData, Dictionary<string, object> row, Tuple<PowertrainPosition, ElectricMotorData> em)
		{
			var emColumns = new List<Tuple<string, ConvertedSI>>() {
				Tuple.Create(Fields.EM_AVG_SPEED_FORMAT,
					modData.ElectricMotorAverageSpeed(em.Item1).ConvertToRoundsPerMinute()),

				Tuple.Create(Fields.E_EM_Mot_DRIVE_FORMAT,
					modData.TotalElectricMotorMotWorkDrive(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(Fields.E_EM_Mot_GENERATE_FORMAT,
					modData.TotalElectricMotorMotWorkRecuperate(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(Fields.ETA_EM_Mot_DRIVE_FORMAT,
					new ConvertedSI(modData.ElectricMotorMotEfficiencyDrive(em.Item1), "")),
				Tuple.Create(Fields.ETA_EM_Mot_GEN_FORMAT,
					new ConvertedSI(modData.ElectricMotorMotEfficiencyGenerate(em.Item1), "")),


				Tuple.Create(Fields.E_EM_DRIVE_FORMAT, modData.TotalElectricMotorWorkDrive(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(Fields.E_EM_GENERATE_FORMAT,
					modData.TotalElectricMotorWorkRecuperate(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(Fields.ETA_EM_DRIVE_FORMAT, new ConvertedSI(modData.ElectricMotorEfficiencyDrive(em.Item1), "")),
				Tuple.Create(Fields.ETA_EM_GEN_FORMAT, new ConvertedSI(modData.ElectricMotorEfficiencyGenerate(em.Item1), "")),

				Tuple.Create(Fields.E_EM_OFF_Loss_Format, modData.ElectricMotorOffLosses(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(Fields.E_EM_LOSS_TRANSM_FORMAT,
					modData.ElectricMotorTransmissionLosses(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(Fields.E_EM_Mot_LOSS_FORMAT, modData.ElectricMotorMotLosses(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(Fields.E_EM_LOSS_FORMAT, modData.ElectricMotorLosses(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(Fields.E_EM_OFF_TIME_SHARE, (ConvertedSI)modData.ElectricMotorOffTimeShare(em.Item1))
			};
			return emColumns;
			
		}

		private void WriteFullPowertrain(VectoRunData runData, Dictionary<string, object> row)
		{
			WriteVehicleData(runData, row);

			if (runData.BusAuxiliaries?.InputData != null) {
				// only write in declaration mode - if input data is set
				// subtract driver!
				row[Fields.PassengerCount] = runData.VehicleData.PassengerCount;
			}

			row[Fields.TCU_MODEL] = runData.ShiftStrategy;
			row[Fields.PTO_TECHNOLOGY] = runData.PTO?.TransmissionType ?? "";

			WriteEngineData(runData.EngineData, row);

			WriteGearboxData(runData.GearboxData, row);

			WriteRetarderData(runData.Retarder, row);

			WriteAngledriveData(runData.AngledriveData, row);

			WriteAxlegearData(runData.AxleGearData, row);

			WriteAuxTechnologies(runData, row);

			WriteAxleWheelsData(runData.VehicleData.AxleData, row);

			WriteAirdragData(runData.AirdragData, row);

		}

		private static void WriteVehicleData(VectoRunData runData, Dictionary<string, object> row)
		{
			var data = runData.VehicleData;
			//if (runData.VehicleData.b)
			var gbxType = runData.GearboxData?.Type ?? GearboxType.NoGearbox;

			row[Fields.VEHICLE_MANUFACTURER] = data.Manufacturer;
			row[Fields.VIN_NUMBER] = data.VIN;
			row[Fields.VEHICLE_MODEL] = data.ModelName;

			row[Fields.HDV_CO2_VEHICLE_CLASS] = runData.Mission?.BusParameter?.BusGroup.GetClassNumber() ?? data.VehicleClass.GetClassNumber();
			row[Fields.CURB_MASS] = (ConvertedSI)data.CurbMass;

			// - (data.BodyAndTrailerWeight ?? 0.SI<Kilogram>());
			row[Fields.LOADING] = (ConvertedSI)data.Loading;
			row[Fields.CARGO_VOLUME] = (ConvertedSI)data.CargoVolume;

			row[Fields.TOTAL_VEHICLE_MASS] = (ConvertedSI)data.TotalVehicleMass;

			row[Fields.SLEEPER_CAB] = data.SleeperCab.HasValue ? (data.SleeperCab.Value ? "yes" : "no") : "-";

			row[Fields.ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER] =
				data.RollResistanceCoefficientWithoutTrailer;
			row[Fields.ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER] =
				data.TotalRollResistanceCoefficient;

			row[Fields.R_DYN] = (ConvertedSI)data.DynamicTyreRadius;

			row[Fields.ADAS_TECHNOLOGY_COMBINATION] = data.ADAS != null ? DeclarationData.ADASCombinations.Lookup(data.ADAS, gbxType).ID : "";

			var cap = "";
			if (runData.BatteryData?.Capacity != null) {
				cap = $"{runData.BatteryData.Capacity.AsAmpHour} Ah";
			}

			if (runData.SuperCapData?.Capacity != null) {
				cap = $"{runData.SuperCapData.Capacity} F";
			}

			row[Fields.REESS_CAPACITY] = cap;
		}

		private static void WriteAirdragData(AirdragData data, Dictionary<string, object> row)
		{
			row[Fields.AIRDRAG_MODEL] = data.ModelName;
			row[Fields.AIRDRAG_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
			row[Fields.AIRDRAG_CERTIFICATION_NUMBER] =
				data.CertificationMethod == CertificationMethod.StandardValues ? "" : data.CertificationNumber;
			row[Fields.CD_x_A_DECLARED] = (ConvertedSI)data.DeclaredAirdragArea;
			row[Fields.CD_x_A] = (ConvertedSI)data.CrossWindCorrectionCurve.AirDragArea;
		}

		private static void WriteEngineData(CombustionEngineData data, Dictionary<string, object> row)
		{
			if (data == null) {
				return;
			}
			row[Fields.ENGINE_MANUFACTURER] = data.Manufacturer;
			row[Fields.ENGINE_MODEL] = data.ModelName;
			row[Fields.ENGINE_CERTIFICATION_NUMBER] = data.CertificationNumber;
			row[Fields.ENGINE_FUEL_TYPE] = data.Fuels.Select(x => x.FuelData.GetLabel()).Join(" / ");
			row[Fields.ENGINE_RATED_POWER] = data.RatedPowerDeclared != null && data.RatedPowerDeclared > 0
				? data.RatedPowerDeclared.ConvertToKiloWatt()
				: data.FullLoadCurves[0].MaxPower.ConvertToKiloWatt();
			row[Fields.ENGINE_IDLING_SPEED] = (ConvertedSI)data.IdleSpeed.AsRPM.SI<Scalar>();
			row[Fields.ENGINE_RATED_SPEED] = data.RatedSpeedDeclared != null && data.RatedSpeedDeclared > 0
				? (ConvertedSI)data.RatedSpeedDeclared.AsRPM.SI<Scalar>()
				: (ConvertedSI)data.FullLoadCurves[0].RatedSpeed.AsRPM.SI<Scalar>();
			row[Fields.ENGINE_DISPLACEMENT] = data.Displacement.ConvertToCubicCentiMeter();

			row[Fields.ENGINE_WHTC_URBAN] = data.Fuels.Select(x => x.WHTCUrban).Join(" / ");
			row[Fields.ENGINE_WHTC_RURAL] = data.Fuels.Select(x => x.WHTCRural).Join(" / ");
			row[Fields.ENGINE_WHTC_MOTORWAY] = data.Fuels.Select(x => x.WHTCMotorway).Join(" / ");
			row[Fields.ENGINE_BF_COLD_HOT] = data.Fuels.Select(x => x.ColdHotCorrectionFactor).Join(" / ");
			row[Fields.ENGINE_CF_REG_PER] = data.Fuels.Select(x => x.CorrectionFactorRegPer).Join(" / ");
			row[Fields.ENGINE_ACTUAL_CORRECTION_FACTOR] = data.Fuels.Select(x => x.FuelConsumptionCorrectionFactor).Join(" / ");
		}

		private static void WriteAxleWheelsData(List<Axle> data, Dictionary<string, object> row)
		{
			var fields = new[] {
				Tuple.Create(Fields.DECLARED_RRC_AXLE1, Fields.DECLARED_FZISO_AXLE1),
				Tuple.Create(Fields.DECLARED_RRC_AXLE2, Fields.DECLARED_FZISO_AXLE2),
				Tuple.Create(Fields.DECLARED_RRC_AXLE3, Fields.DECLARED_FZISO_AXLE3),
				Tuple.Create(Fields.DECLARED_RRC_AXLE4, Fields.DECLARED_FZISO_AXLE4),
			};
			for (var i = 0; i < Math.Min(fields.Length, data.Count); i++) {
				if (data[i].AxleType == AxleType.Trailer) {
					continue;
				}

				row[fields[i].Item1] = data[i].RollResistanceCoefficient;
				row[fields[i].Item2] = (ConvertedSI)data[i].TyreTestLoad;
			}

			row[Fields.NUM_AXLES_DRIVEN] = data.Count(x => x.AxleType == AxleType.VehicleDriven);
			row[Fields.NUM_AXLES_NON_DRIVEN] = data.Count(x => x.AxleType == AxleType.VehicleNonDriven);
			row[Fields.NUM_AXLES_TRAILER] = data.Count(x => x.AxleType == AxleType.Trailer);
		}

		private static void WriteAxlegearData(AxleGearData data, Dictionary<string, object> row)
		{
			if (data == null) {
				return;
			}
			row[Fields.AXLE_MANUFACTURER] = data.Manufacturer;
			row[Fields.AXLE_MODEL] = data.ModelName;
			row[Fields.AXLE_RATIO] = (ConvertedSI)data.AxleGear.Ratio.SI<Scalar>();
			row[Fields.AXLEGEAR_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
			row[Fields.AXLEGEAR_CERTIFICATION_NUMBER] = data.CertificationMethod == CertificationMethod.StandardValues
				? ""
				: data.CertificationNumber;
		}

		private void WriteAuxTechnologies(VectoRunData runData, Dictionary<string, object> row)
		{
			var auxData = runData.Aux;
			var busAux = runData.BusAuxiliaries;
			foreach (var aux in auxData) {
				if (aux.ID == Constants.Auxiliaries.IDs.PTOConsumer || aux.ID == Constants.Auxiliaries.IDs.PTOTransmission) {
					continue;
				}

				var colName = string.Format(Fields.AUX_TECH_FORMAT, aux.ID);

				lock (Table)
					if (!Table.Columns.Contains(colName)) {
					lock (_tableLock) {
						var col = Table.Columns.Add(colName, typeof(string));

						// move the new column to correct position
						col.SetOrdinal(Table.Columns[Fields.CARGO_VOLUME].Ordinal);
					}
				}

				row[colName] = aux.Technology == null ? "" : aux.Technology.Join("; ");
			}

			if (busAux == null) {
				return;
			}

			row[string.Format(Fields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.HeatingVentilationAirCondition)] =
				busAux.SSMInputs is ISSMDeclarationInputs inputs ? inputs.HVACTechnology : "engineering mode";
			row[string.Format(Fields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.ElectricSystem)] =
				busAux.ElectricalUserInputsConfig.AlternatorType.GetLabel();
			row[string.Format(Fields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.PneumaticSystem)] = runData.JobType == VectoSimulationJobType.BatteryElectricVehicle ? "-" :
				busAux.PneumaticUserInputsConfig.CompressorMap.Technology;
		}

		private static void WriteAngledriveData(AngledriveData data, Dictionary<string, object> row)
		{
			if (data != null) {
				row[Fields.ANGLEDRIVE_MANUFACTURER] = data.Manufacturer;
				row[Fields.ANGLEDRIVE_MODEL] = data.ModelName;
				row[Fields.ANGLEDRIVE_RATIO] = data.Angledrive.Ratio;
				row[Fields.ANGLEDRIVE_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
				row[Fields.ANGLEDRIVE_CERTIFICATION_NUMBER] =
					data.CertificationMethod == CertificationMethod.StandardValues
						? ""
						: data.CertificationNumber;
			} else {
				row[Fields.ANGLEDRIVE_MANUFACTURER] = Constants.NOT_AVAILABLE;
				row[Fields.ANGLEDRIVE_MODEL] = Constants.NOT_AVAILABLE;
				row[Fields.ANGLEDRIVE_RATIO] = Constants.NOT_AVAILABLE;
				row[Fields.ANGLEDRIVE_CERTIFICATION_METHOD] = "";
				row[Fields.ANGLEDRIVE_CERTIFICATION_NUMBER] = "";
			}
		}

		private static void WriteRetarderData(RetarderData data, Dictionary<string, object> row)
		{
			row[Fields.RETARDER_TYPE] = (data?.Type ?? RetarderType.None).GetLabel();
			if (data != null && data.Type.IsDedicatedComponent()) {
				row[Fields.RETARDER_MANUFACTURER] = data.Manufacturer;
				row[Fields.RETARDER_MODEL] = data.ModelName;
				row[Fields.RETARDER_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
				row[Fields.RETARDER_CERTIFICATION_NUMBER] = data.CertificationMethod == CertificationMethod.StandardValues
					? ""
					: data.CertificationNumber;
			} else {
				row[Fields.RETARDER_MANUFACTURER] = Constants.NOT_AVAILABLE;
				row[Fields.RETARDER_MODEL] = Constants.NOT_AVAILABLE;
				row[Fields.RETARDER_CERTIFICATION_METHOD] = "";
				row[Fields.RETARDER_CERTIFICATION_NUMBER] = "";
			}
		}

		private static void WriteGearboxData(GearboxData data, Dictionary<string, object> row)
		{
			if (data == null) {
				return;
			}
			row[Fields.GEARBOX_MANUFACTURER] = data.Manufacturer;
			row[Fields.GEARBOX_MODEL] = data.ModelName;
			row[Fields.GEARBOX_TYPE] = data.Type;
			row[Fields.GEARBOX_CERTIFICATION_NUMBER] = data.CertificationMethod == CertificationMethod.StandardValues
				? ""
				: data.CertificationNumber;
			row[Fields.GEARBOX_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
			if (data.Type.AutomaticTransmission()) {
				row[Fields.GEAR_RATIO_FIRST_GEAR] = data.Gears.Count > 0
					? (double.IsNaN(data.Gears.First().Value.Ratio)
						? (ConvertedSI)data.Gears.First().Value.TorqueConverterRatio.SI<Scalar>()
						: (ConvertedSI)data.Gears.First().Value.Ratio.SI<Scalar>())
					: 0.SI<Scalar>();
				row[Fields.GEAR_RATIO_LAST_GEAR] = data.Gears.Count > 0
					? (ConvertedSI)data.Gears.Last().Value.Ratio.SI<Scalar>()
					: (ConvertedSI)0.SI<Scalar>();
				if (data.Type != GearboxType.APTN && data.Type != GearboxType.IHPC) {
					row[Fields.TORQUECONVERTER_MANUFACTURER] = data.TorqueConverterData.Manufacturer;
					row[Fields.TORQUECONVERTER_MODEL] = data.TorqueConverterData.ModelName;
					row[Fields.TORQUE_CONVERTER_CERTIFICATION_NUMBER] =
						data.TorqueConverterData.CertificationMethod == CertificationMethod.StandardValues
							? ""
							: data.TorqueConverterData.CertificationNumber;
					row[Fields.TORQUE_CONVERTER_CERTIFICATION_METHOD] =
						data.TorqueConverterData.CertificationMethod.GetName();
				}
			} else {
				row[Fields.GEAR_RATIO_FIRST_GEAR] = data.Gears.Count > 0
					? (ConvertedSI)data.Gears.First().Value.Ratio.SI<Scalar>()
					: (ConvertedSI)0.SI<Scalar>();
				row[Fields.GEAR_RATIO_LAST_GEAR] = data.Gears.Count > 0
					? (ConvertedSI)data.Gears.Last().Value.Ratio.SI<Scalar>()
					: (ConvertedSI)0.SI<Scalar>();
				row[Fields.TORQUECONVERTER_MANUFACTURER] = Constants.NOT_AVAILABLE;
				row[Fields.TORQUECONVERTER_MODEL] = Constants.NOT_AVAILABLE;
				row[Fields.TORQUE_CONVERTER_CERTIFICATION_METHOD] = "";
				row[Fields.TORQUE_CONVERTER_CERTIFICATION_NUMBER] = "";
			}
		}

		private static string ReplaceNotAllowedCharacters(string text)
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

		[SuppressMessage("ReSharper", "InconsistentNaming")]
		[SuppressMessage("ReSharper", "IdentifierTypo")]
		public static class Fields
		{
			public const string INTERNAL_PREFIX = "INTERNAL";

			public const string SORT = INTERNAL_PREFIX + " Sorting";
			public const string JOB = "Job [-]";
			public const string INPUTFILE = "Input File [-]";
			public const string CYCLE = "Cycle [-]";
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
			public const string GEAR_RATIO_FIRST_GEAR = "Gear ratio first gear [-]";
			public const string GEAR_RATIO_LAST_GEAR = "Gear ratio last gear [-]";

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

			public const string ElectricEnergyConsumptionPerKm = "EC_el_final [kWh/km]";

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

			public const string E_AUX_EL_HV = "E_aux_el(HV) [kWh]";

			public const string E_ICE_START = "E_ice_start [kWh]";
			public const string NUM_ICE_STARTS = "ice_starts [-]";
			public const string K_ENGLINE = "k_engline{0} [g/kWh]";
			public const string K_VEHLINE = "k_vehline{0} [g/kWh]";

			public const string E_WHR_EL = "E_WHR_el [kWh]";
			public const string E_WHR_MECH = "E_WHR_mech [kWh]";

			public const string E_BusAux_PS_corr = "E_BusAux_PS_corr [kWh]";
			public const string E_BusAux_ES_mech_corr = "E_BusAux_ES_mech_corr [kWh]";
			public const string E_BusAux_AuxHeater = "E_BusAux_AuxhHeater [kWh]";

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

			public const string REESS_CAPACITY = "REESS Capacity";
			public const string REESS_StartSoC = "REESS Start SoC [%]";
			public const string REESS_EndSoC = "REESS End SoC [%]";
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


		}
	}
}
