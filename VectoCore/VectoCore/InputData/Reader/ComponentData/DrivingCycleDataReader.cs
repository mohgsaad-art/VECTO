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
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class DrivingCycleDataReader
	{
		/// <summary>
		/// Detects the appropriate cycle type for a cycle in a DataTable.
		/// </summary>
		/// <param name="cycleData">The cycle data.</param>
		/// <returns></returns>
		/// <exception cref="VectoException">CycleFile Format is unknown.</exception>
		public static CycleType DetectCycleType(DataTable cycleData)
		{
			var cols = cycleData.Columns;

			if (DistanceBasedCycleDataParser.ValidateHeader(cols, false)) {
				return CycleType.DistanceBased;
			}

			if (PTOCycleDataParser.ValidateHeader(cols, false)) {
				return CycleType.PTO;
			}

			if (PTODuringDriveCycleParser.ValidateHeader(cols, false)) {
				return CycleType.PTODuringDrive;
			}
			if (PWheelCycleDataParser.ValidateHeader(cols, false)) {
				return CycleType.PWheel;
			}
			if (MeasuredSpeedGearDataParser.ValidateHeader(cols, false)) {
				return CycleType.MeasuredSpeedGear;
			}
			if (MeasuredSpeedDataParser.ValidateHeader(cols, false)) {
				return CycleType.MeasuredSpeed;
			}
			if (EngineOnlyCycleDataParser.ValidateHeader(cols, false)) {
				return CycleType.EngineOnly;
			}
			if (VTPCycleDataParser.ValidateHeader(cols, false)) {
				return CycleType.VTP;
			}

			throw new VectoException("CycleFile format is unknown.");
		}

		private static ICycleDataParser GetDataParser(CycleType type)
		{
			switch (type) {
				case CycleType.EngineOnly: return new EngineOnlyCycleDataParser();
				case CycleType.DistanceBased: return new DistanceBasedCycleDataParser();
				case CycleType.PWheel: return new PWheelCycleDataParser();
				case CycleType.MeasuredSpeedGear: return new MeasuredSpeedGearDataParser();
				case CycleType.MeasuredSpeed: return new MeasuredSpeedDataParser();
				case CycleType.PTO: return new PTOCycleDataParser();
				case CycleType.VTP: return new VTPCycleDataParser();
				case CycleType.PTODuringDrive: return new PTODuringDriveCycleParser();
				default: throw new ArgumentOutOfRangeException("Cycle Type", type.ToString());
			}
		}

		/// <summary>
		/// Reads a cycle from a file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <param name="type">The type.</param>
		/// <param name="crossWindRequired"></param>
		/// <returns></returns>
		/// <exception cref="VectoException">ERROR while reading DrivingCycle File:  + ex.Message</exception>
		public static DrivingCycleData ReadFromFile(string fileName, CycleType type, bool crossWindRequired)
		{
			try {
				var stream = File.OpenRead(fileName);
				return ReadFromStream(stream, type, Path.GetFileNameWithoutExtension(fileName), crossWindRequired);
			} catch (Exception ex) {
				throw new VectoException("ERROR while opening DrivingCycle File: " + ex.Message, ex);
			}
		}

		/// <summary>
		/// Reads the cycle from a stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="type">The type.</param>
		/// <param name="crossWindRequired"></param>
		/// <param name="name">the name of the cycle</param>
		/// <returns></returns>
		public static DrivingCycleData ReadFromStream(Stream stream, CycleType type, string name, bool crossWindRequired)
		{
			try {
				return ReadFromDataTable(VectoCSVFile.ReadStream(stream), type, name, crossWindRequired);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading DrivingCycle Stream: " + ex.Message, ex);
			}
		}

		/// <summary>
		/// Creates a cycle from a DataTable with automatic determination of Cycle Type.
		/// </summary>
		/// <param name="data">The cycle data.</param>
		/// <param name="name">The name.</param>
		/// <param name="crossWindRequired"></param>
		/// <returns></returns>
		public static DrivingCycleData ReadFromDataTable(DataTable data, string name, bool crossWindRequired)
		{
			if (data == null) {
				LogManager.GetLogger(typeof(DrivingCycleDataReader).FullName)
						.Warn("Invalid data for DrivingCycle -- dataTable is null");
				throw new VectoException("Invalid data for DrivingCycle -- dataTable is null");
			}

			return ReadFromDataTable(data, DetectCycleType(data), name, crossWindRequired);
		}

		/// <summary>
		/// Creates a cycle from a DataTable
		/// </summary>
		/// <param name="data">The cycle data.</param>
		/// <param name="type">The type.</param>
		/// <param name="name">The name.</param>
		/// <param name="crossWindRequired"></param>
		/// <returns></returns>
		private static DrivingCycleData ReadFromDataTable(DataTable data, CycleType type, string name, bool crossWindRequired)
		{
			if (data == null) {
				LogManager.GetLogger(typeof(DrivingCycleDataReader).FullName)
						.Warn("Invalid data for DrivingCycle -- dataTable is null");
				throw new VectoException("Invalid data for DrivingCycle -- dataTable is null");
			}

			var entries = GetDataParser(type).Parse(data, crossWindRequired).ToList();

			if (type == CycleType.DistanceBased) {
				entries = FilterDrivingCycleEntries(entries);
			}
			if (type == CycleType.MeasuredSpeed || type == CycleType.MeasuredSpeedGear) {
				entries = ComputeAltitudeTimeBased(entries);
			}
			var cycle = new DrivingCycleData {
				Entries = entries,
				CycleType = type,
				Name = name
			};
			return cycle;
		}

		private static List<DrivingCycleData.DrivingCycleEntry> ComputeAltitudeTimeBased(
			List<DrivingCycleData.DrivingCycleEntry> entries)
		{
			var current = entries.First();
			current.Altitude = 0.SI<Meter>();
			var altitude = current.Altitude;
			var lastTime = entries.First().Time;
			foreach (var drivingCycleEntry in entries) {
				altitude += drivingCycleEntry.VehicleTargetSpeed * (drivingCycleEntry.Time - lastTime) *
							drivingCycleEntry.RoadGradient;
				drivingCycleEntry.Altitude = altitude;
				lastTime = drivingCycleEntry.Time;
			}

			return entries;
		}

		private static List<DrivingCycleData.DrivingCycleEntry> FilterDrivingCycleEntries(
			List<DrivingCycleData.DrivingCycleEntry> entries)
		{
			var filtered = new List<DrivingCycleData.DrivingCycleEntry>();
			var current = entries.First();
			current.Altitude = 0.SI<Meter>();
			filtered.Add(current);
			var distance = current.Distance;
			var altitude = current.Altitude;
			for (var i = 0; i < entries.Count; i++) {
				var entry = entries[i];
				if (i == 0 && entry.VehicleTargetSpeed.IsEqual(0) && entry.StoppingTime.IsEqual(0)) {
					entry.StoppingTime = 1.SI<Second>();
				}
				if (!entry.StoppingTime.IsEqual(0) && !entry.VehicleTargetSpeed.IsEqual(0)) {
					throw new VectoException(
						"Error in DrivingCycle: stop time specified but target-speed > 0! Distance: {0}, stop-time: {1}, target speed: {2}",
						entry.Distance, entry.StoppingTime, entry.VehicleTargetSpeed);
				}
				if (entry.Distance < distance) {
					throw new VectoException(
						"Error in DrivingCycle: distance entry is smaller than last distance! last distance: {0}, current distance: {1} ",
						distance, entry.Distance);
				}

				if (i > 0) {
					altitude += (entry.Distance - distance) * entries[i - 1].RoadGradientPercent / 100.0;
				}
				entry.Altitude = altitude;

				// if something changes in the cycle, add it to the filtered cycle but always add last entry
				if (!CycleEntriesAreEqual(current, entry) || i == entries.Count - 1) {
					entry.Altitude = altitude;
					filtered.Add(entry);
					current = entry;
				}
				if (entry.VehicleTargetSpeed.IsEqual(0)) {
					// vehicle stops. duplicate current distance entry with 0 waiting time
					var tmp = new DrivingCycleData.DrivingCycleEntry(entry) {
						StoppingTime = 0.SI<Second>(),
						PTOActive = entry.PTOActive == PTOActivity.PTOActivityRoadSweeping ? entry.PTOActive : PTOActivity.Inactive,
						RoadGradient = entry.RoadGradient,
						VehicleTargetSpeed = i < entries.Count - 1 ? entries[i + 1].VehicleTargetSpeed : 0.SI<MeterPerSecond>()
					};
					filtered.Add(tmp);
					current = tmp;
				}

				distance = entry.Distance;
			}

			LogManager.GetLogger(typeof(DrivingCycleDataReader).FullName)
					.Info("Data loaded. Number of Entries: {0}, filtered Entries: {1}", entries.Count, filtered.Count);
			entries = filtered;

			AdjustDistanceAfterStop(entries);

			return entries;
		}

		private static void AdjustDistanceAfterStop(List<DrivingCycleData.DrivingCycleEntry> entries)
		{
			using (var currentIt = entries.GetEnumerator()) {
				using (var nextIt = entries.GetEnumerator()) {
					nextIt.MoveNext();
					while (currentIt.MoveNext() && nextIt.MoveNext()) {
						if (currentIt.Current != null && !currentIt.Current.StoppingTime.IsEqual(0)) {
							if (nextIt.Current != null) {
								nextIt.Current.Distance = currentIt.Current.Distance;
							}
						}
					}
				}
			}
		}

		private static bool CycleEntriesAreEqual(
			DrivingCycleData.DrivingCycleEntry first,
			DrivingCycleData.DrivingCycleEntry second)
		{
			if (first.Distance.IsEqual(second.Distance)) {
				return true;
			}

			if (first.VehicleTargetSpeed != second.VehicleTargetSpeed) {
				return false;
			}

			if (!first.RoadGradient.IsEqual(
				second.RoadGradient, Constants.SimulationSettings.DrivingCycleRoadGradientTolerance)) {
				return false;
			}

			if (!(first.StoppingTime.IsEqual(0) && second.StoppingTime.IsEqual(0))) {
				return false;
			}

			if (first.AdditionalAuxPowerDemand != second.AdditionalAuxPowerDemand) {
				return false;
			}

			if (first.AuxiliarySupplyPower.Count != second.AuxiliarySupplyPower.Count) {
				return false;
			}

			if (!first.AuxiliarySupplyPower.SequenceEqual(second.AuxiliarySupplyPower)) {
				return false;
			}

			if (first.PTOActive != second.PTOActive || first.PTOActive == PTOActivity.PTOActivityWhileDrive || first.PTOActive == PTOActivity.PTOActivityRoadSweeping) {
				return false;
			}

			if (first.PTOPowerDemandDuringDrive != null &&
				!first.PTOPowerDemandDuringDrive.IsEqual(second.PTOPowerDemandDuringDrive)) {
				return false;
			}

			if (first.AirSpeedRelativeToVehicle != null && second.AirSpeedRelativeToVehicle != null &&
				!first.AirSpeedRelativeToVehicle.IsEqual(second.AirSpeedRelativeToVehicle)) {
				return false;
			}

			if (!first.WindYawAngle.IsEqual(second.WindYawAngle)) {
				return false;
			}

			return true;
		}

		public static class Fields
		{
			public const string PTOPowerDemand = "P_PTO";
			public const string PTOTorque = "PTO Torque";
			public const string EngineSpeedFull = "Engine speed";
			public const string PWheel = "Pwheel";
			public const string Distance = "s";
			public const string Time = "t";
			public const string VehicleSpeed = "v";
			public const string RoadGradient = "grad";
			public const string StoppingTime = "stop";
			public const string EngineSpeed = "n";
			public const string EngineSpeedSuffix = "n_eng";
			public const string FanSpeed = "n_fan";
			public const string WheelTorqueLeft = "tq_left";
			public const string WheelTorqueRight = "tq_right";
			public const string WheelSpeedLeft = "n_wh_left";
			public const string WheelSpeedRight = "n_wh_right";
			public const string FuelConsumption = "fc";
			public const string Gear = "gear";
			public const string AdditionalAuxPowerDemand = "Padd";
			public const string AirSpeedRelativeToVehicle = "vair_res";
			public const string WindYawAngle = "vair_beta";
			public const string EnginePower = "Pe";
			public const string EngineTorque = "Me";
			public const string TorqueConverterActive = "tc_active";
			public const string PTOActive = "PTO";
			public const string Highway = "HW";
			public const string PTODuringDrivePower = "PTO_Power";
			public const string VTPPSCompressorActive = "PS_comp_active";
		}

		#region DataParser

		private interface ICycleDataParser
		{
			/// <summary>
			/// Parses a datatable to a list of drivingcycleentries for the current implementation.
			/// </summary>
			/// <param name="table"></param>
			/// <param name="crossWindRequired"></param>
			/// <returns></returns>
			IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired);
		}

		private abstract class AbstractCycleDataParser : LoggingObject, ICycleDataParser
		{
			protected static bool CheckColumns(
				DataColumnCollection header, IEnumerable<string> allowedCols,
				IEnumerable<string> requiredCols, bool throwExceptions, bool allowAux)
			{
				var headerStr = header.Cast<DataColumn>().Select(col => col.ColumnName.ToLowerInvariant()).ToArray();

				var diff = headerStr.GroupBy(c => c).Where(g => g.Count() > 2).SelectMany(g => g).ToList();
				if (diff.Any()) {
					if (throwExceptions) {
						throw new VectoException($"Column(s) defined more than once: {diff.OrderBy(x => x).Join()}");
					}

					return false;
				}

				if (allowAux) {
					headerStr = headerStr.Where(c => !c.ToUpper().StartsWith(Constants.Auxiliaries.Prefix)).ToArray();
				}

				diff = headerStr.Except(allowedCols.Select(x => x.ToLowerInvariant())).ToList();
				if (diff.Any()) {
					if (throwExceptions) {
						throw new VectoException($"Column(s) not allowed: {diff.Join()}");
					}

					return false;
				}

				diff = requiredCols.Select(x => x.ToLowerInvariant()).Except(headerStr).ToList();
				if (diff.Any()) {
					if (throwExceptions) {
						throw new VectoException($"Column(s) required: {diff.Join()}");
					}

					return false;
				}

				return true;
			}

			protected static bool CheckComboColumns(DataColumnCollection header, string[] cols, bool throwExceptions)
			{
				var colCount = header.Cast<DataColumn>()
									.Select(col => col.ColumnName.ToLowerInvariant())
									.Intersect(cols.Select(x => x.ToLowerInvariant())).Count();

				if (colCount != 0 && colCount != cols.Length) {
					if (throwExceptions) {
						throw new VectoException("Either all columns have to be defined or none of them: {0}", cols.Join());
					}

					return false;
				}

				return true;
			}

			public abstract IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired);
		}

		private class DistanceBasedCycleDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns);

				return table.Rows.Cast<DataRow>().Select(
					row => new DrivingCycleData.DrivingCycleEntry {
						Distance = row.ParseDouble(Fields.Distance).SI<Meter>(),
						VehicleTargetSpeed = row.ParseDouble(Fields.VehicleSpeed).KMPHtoMeterPerSecond(),
						RoadGradient = VectoMath.InclinationToAngle(row.ParseDoubleOrGetDefault(Fields.RoadGradient) / 100.0),
						StoppingTime = row.ParseDouble(Fields.StoppingTime).SI<Second>(),
						AdditionalAuxPowerDemand = row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI(Unit.SI.Kilo.Watt)
													.Cast<Watt>(),
						AngularVelocity = row.ParseDoubleOrGetDefault(Fields.EngineSpeed).RPMtoRad(),
						Gear = (uint)row.ParseDoubleOrGetDefault(Fields.Gear),
						AirSpeedRelativeToVehicle =
							crossWindRequired ? row.ParseDouble(Fields.AirSpeedRelativeToVehicle).KMPHtoMeterPerSecond() : null,
						WindYawAngle = crossWindRequired ? row.ParseDoubleOrGetDefault(Fields.WindYawAngle) : 0,
						AuxiliarySupplyPower = row.GetAuxiliaries(),
						Highway = table.Columns.Contains(Fields.Highway) && row.Field<string>(Fields.Highway) == "1",
						PTOActive = table.Columns.Contains(Fields.PTOActive)
							? (PTOActivity)row.Field<string>(Fields.PTOActive).ToInt()
							: PTOActivity.Inactive,
						PTOPowerDemandDuringDrive = table.Columns.Contains(Fields.PTOPowerDemand)
							? row.ParseDouble(Fields.PTOPowerDemand).SI(Unit.SI.Kilo.Watt).Cast<Watt>()
							: null
					});
			}

			public static bool ValidateHeader(DataColumnCollection header, bool throwExceptions = true)
			{
				var requiredCols = new[] {
					Fields.Distance,
					Fields.VehicleSpeed,
					Fields.StoppingTime
				};

				var allowedCols = new[] {
					Fields.Distance,
					Fields.VehicleSpeed,
					Fields.StoppingTime,
					Fields.AdditionalAuxPowerDemand,
					Fields.RoadGradient,
					Fields.AirSpeedRelativeToVehicle,
					Fields.WindYawAngle,
					Fields.PTOActive,
					Fields.Highway,
					Fields.PTOPowerDemand,
				};

				const bool allowAux = true;

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux) &&
						CheckComboColumns(header, new[] { Fields.AirSpeedRelativeToVehicle, Fields.WindYawAngle }, throwExceptions);
			}
		}

		/// <summary>
		/// Parser for EngineOnly Cycles.
		/// </summary>
		// <t>, <n>, (<Pe>|<Me>)[, <Padd>]
		private class EngineOnlyCycleDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns);

				var absTime = 0;
				foreach (DataRow row in table.Rows) {
					var entry = new DrivingCycleData.DrivingCycleEntry {
						Time = row.ParseDoubleOrGetDefault(Fields.Time, absTime).SI<Second>(),
						AngularVelocity = row.ParseDoubleOrGetDefault(Fields.EngineSpeed).RPMtoRad(),
						AdditionalAuxPowerDemand =
							row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
						AuxiliarySupplyPower = row.GetAuxiliaries()
					};

					if (row.Table.Columns.Contains(Fields.EngineTorque)) {
						if (row.Field<string>(Fields.EngineTorque).Equals("<DRAG>")) {
							entry.Drag = true;
						} else {
							entry.Torque = row.ParseDouble(Fields.EngineTorque).SI<NewtonMeter>();
						}
					} else {
						if (row.Field<string>(Fields.EnginePower).Equals("<DRAG>")) {
							entry.Drag = true;
						} else {
							entry.Torque = row.ParseDouble(Fields.EnginePower).SI(Unit.SI.Kilo.Watt).Cast<Watt>() / entry.AngularVelocity;
						}
					}
					absTime += 1;

					yield return entry;
				}
			}

			public static bool ValidateHeader(DataColumnCollection header, bool throwExceptions = true)
			{
				var requiredCols = new[] {
					//Fields.Time not needed --> if missing 1 second resolution is assumed
					Fields.EngineSpeed
				};

				var allowedCols = new[] {
					Fields.Time,
					Fields.EngineSpeed,
					Fields.EngineTorque,
					Fields.EnginePower,
					Fields.AdditionalAuxPowerDemand
				};

				const bool allowAux = false;

				if (!CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux)) {
					return false;
				}

				var containsNone = !header.Contains(Fields.EngineTorque) && !header.Contains(Fields.EnginePower);
				if (containsNone) {
					if (throwExceptions) {
						throw new VectoException(
							"Column(s) missing: Either column '{0}' or column '{1}' must be defined.",
							Fields.EngineTorque, Fields.EnginePower);
					}

					return false;
				}

				var containsBoth = header.Contains(Fields.EngineTorque) && header.Contains(Fields.EnginePower);
				if (containsBoth) {
					LogManager.GetLogger(typeof(EngineOnlyCycleDataParser).FullName)
							.Warn(
								"Found column '{0}' and column '{1}': Only column '{0}' will be used.",
								Fields.EngineTorque, Fields.EnginePower);
				}
				return true;
			}
		}

		/// <summary>
		/// Parser for PWheel (SiCo) Mode.
		/// </summary>
		// <t>, <Pwheel>, <Gear>, <n>, <Padd>
		private class PWheelCycleDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns);

				var entries = table.Rows.Cast<DataRow>().Select(
					row => new DrivingCycleData.DrivingCycleEntry {
						Time = row.ParseDouble(Fields.Time).SI<Second>(),
						PWheel = row.ParseDouble(Fields.PWheel).SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
						Gear = (uint)row.ParseDoubleOrGetDefault(Fields.Gear),
						AngularVelocity = row.ParseDouble(Fields.EngineSpeed).RPMtoRad(),
						AdditionalAuxPowerDemand = row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI(Unit.SI.Kilo.Watt)
													.Cast<Watt>()
					}).ToArray();

				return entries;
			}

			public static bool ValidateHeader(DataColumnCollection header, bool throwExceptions = true)
			{
				var requiredCols = new[] {
					Fields.Time,
					Fields.PWheel,
					Fields.EngineSpeed
				};
				var allowedCols = new[] {
					Fields.Time,
					Fields.PWheel,
					Fields.Gear,
					Fields.EngineSpeed,
					Fields.AdditionalAuxPowerDemand
				};

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux: false);
			}
		}

		/// <summary>
		/// Parser for Measured Speed Mode Option 1.
		/// </summary>
		// <t>, <v>[, <grad>, <Padd>, <vair_res>, <vair_beta>, Aux_...]
		private class MeasuredSpeedDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns);

				var entries = table.Rows.Cast<DataRow>().Select(
					row => new DrivingCycleData.DrivingCycleEntry {
						Time = row.ParseDouble(Fields.Time).SI<Second>(),
						VehicleTargetSpeed = row.ParseDouble(Fields.VehicleSpeed).KMPHtoMeterPerSecond(),
						RoadGradient = VectoMath.InclinationToAngle(row.ParseDoubleOrGetDefault(Fields.RoadGradient) / 100.0),
						AdditionalAuxPowerDemand = row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI(Unit.SI.Kilo.Watt)
													.Cast<Watt>(),
						AirSpeedRelativeToVehicle =
							crossWindRequired ? row.ParseDouble(Fields.AirSpeedRelativeToVehicle).KMPHtoMeterPerSecond() : null,
						WindYawAngle = crossWindRequired ? row.ParseDouble(Fields.WindYawAngle) : 0,
						AuxiliarySupplyPower = row.GetAuxiliaries()
					}).ToArray();

				return entries;
			}

			public static bool ValidateHeader(DataColumnCollection header, bool throwExceptions = true)
			{
				var requiredCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed
				};

				var allowedCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed,
					Fields.AdditionalAuxPowerDemand,
					Fields.RoadGradient,
					Fields.AirSpeedRelativeToVehicle,
					Fields.WindYawAngle
				};

				const bool allowAux = true;

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux) &&
						CheckComboColumns(header, new[] { Fields.AirSpeedRelativeToVehicle, Fields.WindYawAngle }, throwExceptions);
			}
		}

		/// <summary>
		/// Parser for Measured Speed Mode Option 2.
		/// </summary>
		// <t>, <v>, <n>, <gear>[, <tc_active>, <grad>, <Padd>, <vair_res>, <vair_beta>, Aux_...]
		private class MeasuredSpeedGearDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns);

				var entries = table.Rows.Cast<DataRow>().Select(
					row => new DrivingCycleData.DrivingCycleEntry {
						Time = row.ParseDouble(Fields.Time).SI<Second>(),
						VehicleTargetSpeed = row.ParseDouble(Fields.VehicleSpeed).KMPHtoMeterPerSecond(),
						RoadGradient = VectoMath.InclinationToAngle(row.ParseDoubleOrGetDefault(Fields.RoadGradient) / 100.0),
						AdditionalAuxPowerDemand = row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI(Unit.SI.Kilo.Watt)
													.Cast<Watt>(),
						AngularVelocity = row.ParseDoubleOrGetDefault(Fields.EngineSpeed).RPMtoRad(),
						Gear = (uint)row.ParseDouble(Fields.Gear),
						TorqueConverterActive = table.Columns.Contains(Fields.TorqueConverterActive)
							? row.ParseBoolean(Fields.TorqueConverterActive)
							: (bool?)null,
						AirSpeedRelativeToVehicle = crossWindRequired
							? row.ParseDouble(Fields.AirSpeedRelativeToVehicle).KMPHtoMeterPerSecond()
							: null,
						WindYawAngle = crossWindRequired ? row.ParseDoubleOrGetDefault(Fields.WindYawAngle) : 0,
						AuxiliarySupplyPower = row.GetAuxiliaries()
					}).ToArray();

				return entries;
			}

			public static bool ValidateHeader(DataColumnCollection header, bool throwExceptions = true)
			{
				var requiredCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed,
					Fields.Gear
				};
				var allowedCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed,
					Fields.EngineSpeed,
					Fields.Gear,
					Fields.TorqueConverterActive,
					Fields.AdditionalAuxPowerDemand,
					Fields.RoadGradient,
					Fields.AirSpeedRelativeToVehicle,
					Fields.WindYawAngle
				};

				const bool allowAux = true;

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux) &&
						CheckComboColumns(header, new[] { Fields.AirSpeedRelativeToVehicle, Fields.WindYawAngle }, throwExceptions);
			}
		}

		/// <summary>
		/// Parser for PTO Cycles.
		/// </summary>
		// <t> [s], <Engine Speed> [rpm], <PTO Torque> [Nm]
		private class PTOCycleDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns);

				var entries = table.Rows.Cast<DataRow>().Select(
					row => new DrivingCycleData.DrivingCycleEntry {
						Time = row.ParseDouble(Fields.Time).SI<Second>(),
						AngularVelocity = row.ParseDouble(Fields.EngineSpeedFull).RPMtoRad(),
						Torque = row.ParseDouble(Fields.PTOTorque).SI<NewtonMeter>()
					}).ToArray();

				return entries;
			}

			public static bool ValidateHeader(DataColumnCollection header, bool throwExceptions = true)
			{
				var requiredCols = new[] {
					Fields.Time,
					Fields.EngineSpeedFull,
					Fields.PTOTorque
				};
				var allowedCols = new[] {
					Fields.Time,
					Fields.EngineSpeedFull,
					Fields.PTOTorque
				};

				const bool allowAux = false;

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux);
			}
		}

		/// <summary>
		/// Parser for PTO Cycles.
		/// </summary>
		// <t>,<v> [km/h],<Pwheel> [kW],<n_eng> [rpm],<n_fan> [rpm], <Padd> [kW]
		private class VTPCycleDataParser : AbstractCycleDataParser
		{
			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table,
				bool crossWindRequired)
			{
				ValidateHeader(table.Columns);

				var fuels = table.Columns.Cast<DataColumn>().Where(x => x.ColumnName.StartsWith("fc_"))
					.Select(x => x.ColumnName.Replace("fc_", "").ParseEnum<FuelType>()).ToArray();
				var entries = table.Rows.Cast<DataRow>().Select(
					row => {

						var tqLeft = row.ParseDouble(Fields.WheelTorqueLeft).SI<NewtonMeter>();
						var tqRight = row.ParseDouble(Fields.WheelTorqueRight).SI<NewtonMeter>();
						var speedLeft = row.ParseDouble(Fields.WheelSpeedLeft).RPMtoRad();
						var speedRight = row.ParseDouble(Fields.WheelSpeedRight).RPMtoRad();
						var fc = new Dictionary<FuelType, KilogramPerSecond>();
						foreach (var fuelType in fuels) {
							fc[fuelType] = row.ParseDoubleOrGetDefault("fc_" + fuelType.ToXMLFormat())
								.SI(Unit.SI.Gramm.Per.Hour)
								.Cast<KilogramPerSecond>();
						}

						return new DrivingCycleData.DrivingCycleEntry {
							Time = row.ParseDouble(Fields.Time).SI<Second>(),
							VehicleTargetSpeed = row.ParseDouble(Fields.VehicleSpeed).KMPHtoMeterPerSecond(),
							AdditionalAuxPowerDemand =
								row.ParseDoubleOrGetDefault(Fields.AdditionalAuxPowerDemand).SI(Unit.SI.Kilo.Watt)
									.Cast<Watt>(),
							EngineSpeed = row.ParseDouble(Fields.EngineSpeedSuffix).RPMtoRad(),
							FanSpeed = row.ParseDouble(Fields.FanSpeed).RPMtoRad(),
							Gear = (uint)row.ParseDoubleOrGetDefault(Fields.Gear),
							VTPFuelconsumption = fc,
							VTPPSCompressorActive = row.ParseBooleanOrGetDefault(Fields.VTPPSCompressorActive) ?? false,
							//row.ParseDoubleOrGetDefault(Fields.FuelConsumption).SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>(),
							TorqueConverterActive = row.ParseBooleanOrGetDefault(Fields.TorqueConverterActive),
							TorqueWheelLeft = tqLeft,
							TorqueWheelRight = tqRight,
							WheelSpeedLeft = speedLeft,
							WheelSpeedRight = speedRight
						};
					}).ToArray();

				return entries;
			}

			public static bool ValidateHeader(DataColumnCollection header, bool throwExceptions = true)
			{
				var requiredCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed,
					Fields.EngineSpeedSuffix,
					Fields.FanSpeed,
					Fields.WheelSpeedLeft,
					Fields.WheelSpeedRight,
					Fields.WheelTorqueLeft,
					Fields.WheelTorqueRight,
				};

				var allowedCols = new[] {
					Fields.Time,
					Fields.VehicleSpeed,
					Fields.EngineSpeedSuffix,
					Fields.FanSpeed,
					Fields.WheelSpeedLeft,
					Fields.WheelSpeedRight,
					Fields.WheelTorqueLeft,
					Fields.WheelTorqueRight,
					Fields.Gear,
					Fields.TorqueConverterActive,
					Fields.VTPPSCompressorActive,
					//Fields.FuelConsumption
				}.Concat(EnumHelper.GetValues<FuelType>().Select(x => "fc_" + x.ToXMLFormat()));

				const bool allowAux = true;

				var valid =  CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux) &&
						CheckComboColumns(header, new[] { Fields.AirSpeedRelativeToVehicle, Fields.WindYawAngle }, throwExceptions);

				//valid &= header.Cast<DataColumn>().Any(x => x.ColumnName.StartsWith("fc_"));

				return valid;
			}
		}



		private class PTODuringDriveCycleParser : AbstractCycleDataParser {
			#region Overrides of AbstractCycleDataParser

			public override IEnumerable<DrivingCycleData.DrivingCycleEntry> Parse(DataTable table, bool crossWindRequired)
			{
				ValidateHeader(table.Columns);

				var entries = table.Rows.Cast<DataRow>().Select(
					row => new DrivingCycleData.DrivingCycleEntry {
						Time = row.ParseDouble(Fields.Time).SI<Second>(),
						PTOPowerDemandDuringDrive = row.ParseDouble(Fields.PTODuringDrivePower).SI(Unit.SI.Kilo.Watt).Cast<Watt>()
					}).ToArray();

				return entries;
			}

			#endregion

			public static bool ValidateHeader(DataColumnCollection header, bool throwExceptions = true)
			{
				var requiredCols = new[] {
					Fields.Time,
					Fields.PTODuringDrivePower
				};

				var allowedCols = new[] {
					Fields.Time,
					Fields.PTODuringDrivePower
				};
				const bool allowAux = false;

				return CheckColumns(header, allowedCols, requiredCols, throwExceptions, allowAux) &&
						CheckComboColumns(header, new[] { Fields.AirSpeedRelativeToVehicle, Fields.WindYawAngle }, throwExceptions);
			}
		}
	}

	#endregion
}
