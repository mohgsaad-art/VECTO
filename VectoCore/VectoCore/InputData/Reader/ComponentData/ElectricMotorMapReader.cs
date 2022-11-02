using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData {

	public class ElectricMotorMapReader
	{
		public static EfficiencyMap Create(Stream data, int count)
		{
			return Create(VectoCSVFile.ReadStream(data), count);
		}

		public static EfficiencyMap Create(DataTable data, int count)
		{
			var headerValid = HeaderIsValid(data.Columns);
			if (!headerValid) {
				LoggingObject.Logger<ElectricMotorMapReader>().Warn(
					"Efficiency Map: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}. Falling back to column index.",
					Fields.MotorSpeed,
					Fields.Torque,
					Fields.PowerElectrical,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.Torque;
				data.Columns[2].ColumnName = Fields.PowerElectrical;
			}
			
			var entries = (from DataRow row in data.Rows select CreateEntry(row)).ToList();
			var entriesZero = GetEntriesAtZeroRpm(entries);

			var delaunayMap = new DelaunayMap("ElectricMotorEfficiencyMap Mechanical to Electric");
			var retVal = new EfficiencyMapNew(delaunayMap);
			
			foreach (var entry in entriesZero.OrderBy(x => x.Torque)) {
				try {
					delaunayMap.AddPoint(-entry.Torque.Value() * count,
						0,
						retVal.GetDelaunayZValue(entry) * count);
				} catch (Exception e) {
					throw new VectoException($"EfficiencyMap - Entry {entry}: {e.Message}", e);
				}
			}

			foreach (var entry in entries.Where(x => x.MotorSpeed.IsGreater(0)).OrderBy(x => x.MotorSpeed)
				.ThenBy(x => x.Torque)) {
				try {
					delaunayMap.AddPoint(-entry.Torque.Value() * count,
						entry.MotorSpeed.Value(),
						retVal.GetDelaunayZValue(entry) * count);
				} catch (Exception e) {
					throw new VectoException($"EfficiencyMap - Entry {entry}: {e.Message}", e);
				}
			}

			delaunayMap.Triangulate();
			return retVal;
		}

		private static List<EfficiencyMap.Entry> GetEntriesAtZeroRpm(List<EfficiencyMap.Entry> entries)
		{
			// find entries at first grid point above 0. em-speed might vary slightly,
			// so apply clustering, and use distance between first clusters to select all entries at lowest speed grid point

			const int numEntriesExtrapolationFitting = 4;

			var speeds = new MeanShiftClustering(){ClusterCount = 100}.FindClusters(entries.Select(x => x.MotorSpeed.AsRPM).ToArray(), 10)
				.Where(x => x > 0).ToList();
			var lowerSpeed = speeds.First().RPMtoRad() / 2.0;
			var upperSpeed = speeds.First().RPMtoRad() + (speeds[1] - speeds.First()).RPMtoRad() / 2.0;
			
			//entries at lowest speed gridpoint
			var torquesMinRpm = entries.Where(x => x.MotorSpeed.IsBetween(lowerSpeed, upperSpeed)).OrderBy(x => x.Torque).ToList();
			// entries at 0 rpm grid point
			var torquesZeroRpm = entries.Where(x => x.MotorSpeed.IsEqual(0)).OrderBy(x => x.Torque).ToList();
			if (torquesZeroRpm.Count == 0) {
				throw new VectoException("Electric Motor PowerMap contains no entries at 0 rpm!");
			}

			var entriesZero = new List<EfficiencyMap.Entry>();
			var avgSpeed = torquesMinRpm.Average(x => x.MotorSpeed.Value()).SI<PerSecond>();
			var torquesZeroMin = torquesZeroRpm.Min(x => x.Torque);
			// if at 0 rpm a torque entry below the min torque at min speed is present, extrapolate to this torque
			if (torquesZeroMin.IsSmaller(torquesMinRpm.Min(x => x.Torque))) {
				// extrapolate entry at 0 rpm with min torque
				var negTorque = torquesMinRpm.Where(x => x.Torque <= 0).OrderBy(x => x.Torque).ToList();
				if (negTorque.Count < 2) {
					throw new VectoException(
						"Failed to generate electric power map - at least two negative entries are required");
				}

				var (k, d) = VectoMath.LeastSquaresFitting(negTorque.Take(numEntriesExtrapolationFitting), x => x.Torque.Value(),
					x => x.PowerElectrical.Value());
				var extrapolatedPwr = (torquesZeroMin.Value() * k + d).SI<Watt>();
				entriesZero.Add(new EfficiencyMap.Entry(avgSpeed, torquesZeroMin, extrapolatedPwr));
			}
			// copy all entries in-between
			foreach (var entry in torquesMinRpm) {
				entriesZero.Add(new EfficiencyMap.Entry(avgSpeed, entry.Torque, entry.PowerElectrical));
			}

			// if at 0 rpm a torque entry above the max torqe at min speed is present, extrapolate to this torque
			var torquesZeroMax = torquesZeroRpm.Max(x => x.Torque);
			if (torquesZeroMax.IsGreater(torquesMinRpm.Max(x => x.Torque))) {
				// extrapolate entry at 0 rpm with max torque
				var posTorque = torquesMinRpm.Where(x => x.Torque >= 0).OrderBy(x => x.Torque).Reverse().ToList();
				if (posTorque.Count < 2) {
					throw new VectoException(
						"Failed to generate electrip power map - at least two positive entries are required");
				}

				var (k, d) = VectoMath.LeastSquaresFitting(posTorque.Take(numEntriesExtrapolationFitting), x => x.Torque.Value(),
					x => x.PowerElectrical.Value());
				var extrapolatedPwr = (torquesZeroMax.Value() * k + d).SI<Watt>();
				entriesZero.Add(new EfficiencyMap.Entry(avgSpeed, torquesZeroRpm.Max(x => x.Torque), extrapolatedPwr));
			}

			return entriesZero;
		}

		private static EfficiencyMap.Entry CreateEntry(DataRow row)
		{
			return new EfficiencyMap.Entry(
				speed: row.ParseDouble(Fields.MotorSpeed).RPMtoRad(),
				torque: row.ParseDouble(Fields.Torque).SI<NewtonMeter>(),
				powerElectrical: row.ParseDouble(Fields.PowerElectrical).SI(Unit.SI.Kilo.Watt).Cast<Watt>());
		}


		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.MotorSpeed) && columns.Contains(Fields.Torque) &&
					columns.Contains(Fields.PowerElectrical);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n";
			public const string Torque = "T";
			public const string PowerElectrical = "P_el";
		}

		
	}
}