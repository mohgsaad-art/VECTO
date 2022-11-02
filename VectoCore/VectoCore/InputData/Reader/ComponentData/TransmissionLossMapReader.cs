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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public static class TransmissionLossMapReader
	{
		public static TransmissionLossMap ReadFromFile(string fileName, double gearRatio, string gearName, bool extendLossMap = false)
		{
			try {
				var data = VectoCSVFile.Read(fileName, true);
				return Create(data, gearRatio, gearName, extendLossMap);
			} catch (Exception ex) {
				throw new VectoException("ERROR while reading TransmissionLossMap " + gearName + ": " + ex.Message, ex);
			}
		}

		/// <summary>
		/// Create a TransmissionLoss Map from a DataTable.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="gearRatio"></param>
		/// <param name="gearName"></param>
		/// <param name="extendLossMap"></param>
		/// <returns></returns>
		public static TransmissionLossMap Create(DataTable data, double gearRatio, string gearName, bool extendLossMap = false)
		{
			if (data == null || data.Columns.Count < 3) {
				throw new VectoException("TransmissionLossMap Data File for {0} must consist of 3 columns.", gearName);
			}

			if (data.Rows.Count < 4) {
				throw new VectoException(
					"TransmissionLossMap for {0} must consist of at least four lines with numeric values (below file header", gearName);
			}

			List<TransmissionLossMap.GearLossMapEntry> entries;
			if (HeaderIsValid(data.Columns)) {
				entries = CreateFromColumnNames(data);
			} else {
				LoggingObject.Logger<TransmissionLossMap>().Warn(
					"TransmissionLossMap {0}: Header line is not valid. Expected: '{1}, {2}, {3}'. Got: '{4}'. Falling back to column index.",
					gearName,
					Fields.InputSpeed,
					Fields.InputTorque,
					Fields.TorqeLoss,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());

				entries = CreateFromColumIndizes(data);
			}
			if (!extendLossMap) {
				return new TransmissionLossMap(entries, gearRatio, gearName);
			}
			entries = ExtendLossMap(entries);
			return new TransmissionLossMap(entries, gearRatio, gearName);
		}

		public static TransmissionLossMap CreateEmADCLossMap(DataTable data, double gearRatio, string gearName)
		{
			if (data == null || data.Columns.Count < 3) {
				throw new VectoException("TransmissionLossMap Data File for {0} must consist of 3 columns.", gearName);
			}

			if (data.Rows.Count < 4) {
				throw new VectoException(
					"TransmissionLossMap for {0} must consist of at least four lines with numeric values (below file header", gearName);
			}

			List<TransmissionLossMap.GearLossMapEntry> entries;
			if (!HeaderIsValid(data.Columns)) {
				LoggingObject.Logger<TransmissionLossMap>().Warn(
					"TransmissionLossMap {0}: Header line is not valid. Expected: '{1}, {2}, {3}'. Got: '{4}'. Falling back to column index.",
					gearName,
					Fields.InputSpeed, 
					Fields.InputTorque, 
					Fields.TorqeLoss,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());

				data.Columns[0].ColumnName = Fields.InputSpeed;
				data.Columns[1].ColumnName = Fields.InputTorque;
				data.Columns[2].ColumnName = Fields.TorqeLoss;
			}
			entries = (from DataRow row in data.Rows
					   select new TransmissionLossMap.GearLossMapEntry(
						   inputSpeed: row.ParseDouble(Fields.InputSpeed).RPMtoRad(),
						   inputTorque: -row.ParseDouble(Fields.InputTorque).SI<NewtonMeter>(),
						   torqueLoss: -row.ParseDouble(Fields.TorqeLoss).SI<NewtonMeter>()))
				.ToList();

			return new TransmissionLossMap(entries, gearRatio, gearName);
		}

		private static List<TransmissionLossMap.GearLossMapEntry> ExtendLossMap(
			List<TransmissionLossMap.GearLossMapEntry> entries)
		{
			var maxTorque = entries.Max(x => x.InputTorque);

			var clusterer = new MeanShiftClustering();
			var cluster = clusterer.FindClusters(entries.Select(x => x.InputSpeed.Value()).ToArray(), 1e-1);
			var minDistance = cluster.Pairwise((x, y) => Math.Abs(y - x)).Min();

			var speedBuckets = new Dictionary<PerSecond, List<TransmissionLossMap.GearLossMapEntry>>();
			foreach (var c in cluster) {
				speedBuckets.Add(c.SI<PerSecond>(), new List<TransmissionLossMap.GearLossMapEntry>());
			}
			foreach (var entry in entries) {
				foreach (var speed in cluster) {
					if (entry.InputTorque.IsGreaterOrEqual(0) && Math.Abs(speed - entry.InputSpeed.Value()) < minDistance / 2.0) {
						speedBuckets[speed.SI<PerSecond>()].Add(entry);
					}
				}
			}
			foreach (var speedBucket in speedBuckets) {
				if (speedBucket.Value.Count < 2) {
					continue;
				}

				var (k, d) = VectoMath.LeastSquaresFitting(speedBucket.Value, x => x.InputTorque.Value(), x => x.TorqueLoss.Value());

				for (var i = 2; i <= DeclarationData.LossMapExtrapolationFactor; i++) {
					var inTq = i * maxTorque;
					if (k > 0) {
						entries.Add(new TransmissionLossMap.GearLossMapEntry(speedBucket.Key, inTq, k * inTq + d.SI<NewtonMeter>()));
						entries.Add(new TransmissionLossMap.GearLossMapEntry(speedBucket.Key, -inTq, k * inTq + d.SI<NewtonMeter>()));
					} else {
						var torqueLossLastEntry = speedBucket.Value.OrderBy(x => x.InputSpeed).Last().TorqueLoss;
						entries.Add(new TransmissionLossMap.GearLossMapEntry(speedBucket.Key, inTq, torqueLossLastEntry));
						entries.Add(new TransmissionLossMap.GearLossMapEntry(speedBucket.Key, -inTq, torqueLossLastEntry));
					}
				}
			}

			return entries;
		}

		/// <summary>
		/// Create a DataTable from an efficiency value.
		/// </summary>
		/// <param name="efficiency"></param>
		/// <param name="gearRatio"></param>
		/// <param name="gearName"></param>
		/// <returns></returns>
		public static TransmissionLossMap Create(double efficiency, double gearRatio, string gearName)
		{
			if (double.IsNaN(efficiency)) {
				throw new VectoException("TransmissionLossMap: Efficiency is not a number.");
			}

			if (efficiency <= 0) {
				throw new VectoException("TransmissionLossMap: Efficiency for gear {0} must be greater than 0", gearName);
			}
			if (efficiency > 1) {
				throw new VectoException("TransmissionLossMap: Efficiency for gear {1} must not be greater than 1", gearName);
			}
			var entries = new List<TransmissionLossMap.GearLossMapEntry> {
				new TransmissionLossMap.GearLossMapEntry(0.RPMtoRad(), 1e5.SI<NewtonMeter>(),
					(1 - efficiency) * 1e5.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(0.RPMtoRad(), -1e5.SI<NewtonMeter>(),
					(1 - efficiency) * 1e5.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(0.RPMtoRad(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(10000.RPMtoRad(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(10000.RPMtoRad(), -1e5.SI<NewtonMeter>(),
					(1 - efficiency) * 1e5.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(10000.RPMtoRad(), 1e5.SI<NewtonMeter>(),
					(1 - efficiency) * 1e5.SI<NewtonMeter>()),
			};
			return new TransmissionLossMap(entries, gearRatio, gearName);
		}

		public static TransmissionLossMap CreateEmADCLossMap(double efficiency, double gearRatio, string gearName)
		{
			if (double.IsNaN(efficiency)) {
				throw new VectoException("TransmissionLossMap: Efficiency is not a number.");
			}

			if (efficiency <= 0) {
				throw new VectoException("TransmissionLossMap: Efficiency for gear {0} must be greater than 0", gearName);
			}
			if (efficiency > 1) {
				throw new VectoException("TransmissionLossMap: Efficiency for gear {1} must not be greater than 1", gearName);
			}
			// different signs due to e-motor: negative torque means EM propels the vehicle, positive torque means the EM recuperates energy
			// to use the same equations as for other components (i.e. lookup methods of TransmissionLossMap) set losses to negative values
			// for positive torque entries torque is transferred from drivetrain to EM -> drivetrain is reference for computing efficiency
			// for negative torque entries torque is transferred from EM to drivetrain -> EM is reference for computing efficiency
			var entries = new List<TransmissionLossMap.GearLossMapEntry> {
				new TransmissionLossMap.GearLossMapEntry(0.RPMtoRad(), 1e5.SI<NewtonMeter>(),
					(1 - 1/efficiency) * 1e5.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(0.RPMtoRad(), -1e5.SI<NewtonMeter>(),
					-(1 - efficiency) * 1e5.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(0.RPMtoRad(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(100000.RPMtoRad(), 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(100000.RPMtoRad(), -1e5.SI<NewtonMeter>(),
					-(1 - efficiency) * 1e5.SI<NewtonMeter>()),
				new TransmissionLossMap.GearLossMapEntry(100000.RPMtoRad(), 1e5.SI<NewtonMeter>(),
					(1 - 1/efficiency) * 1e5.SI<NewtonMeter>()),
			};
			return new TransmissionLossMap(entries, gearRatio, gearName);
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.InputSpeed) &&
					columns.Contains(Fields.InputTorque) &&
					columns.Contains(Fields.TorqeLoss);
		}

		private static List<TransmissionLossMap.GearLossMapEntry> CreateFromColumnNames(DataTable data)
		{
			return (from DataRow row in data.Rows
					select new TransmissionLossMap.GearLossMapEntry(
						inputSpeed: row.ParseDouble(Fields.InputSpeed).RPMtoRad(),
						inputTorque: row.ParseDouble(Fields.InputTorque).SI<NewtonMeter>(),
						torqueLoss: row.ParseDouble(Fields.TorqeLoss).SI<NewtonMeter>()))
				.ToList();
		}

		private static List<TransmissionLossMap.GearLossMapEntry> CreateFromColumIndizes(DataTable data)
		{
			return (from DataRow row in data.Rows
					select new TransmissionLossMap.GearLossMapEntry(
						inputSpeed: row.ParseDouble(0).RPMtoRad(),
						inputTorque: row.ParseDouble(1).SI<NewtonMeter>(),
						torqueLoss: row.ParseDouble(2).SI<NewtonMeter>()))
				.ToList();
		}

		public static class Fields
		{
			/// <summary>[rpm]</summary>
			public const string InputSpeed = "Input Speed";

			/// <summary>[Nm]</summary>
			public const string InputTorque = "Input Torque";

			/// <summary>[Nm]</summary>
			public const string TorqeLoss = "Torque Loss";
		}
	}
}