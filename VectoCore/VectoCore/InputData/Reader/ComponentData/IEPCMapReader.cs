using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public class IEPCMapReader
	{
		public static EfficiencyMap Create(Stream data, int count, double ratio,
			ElectricMotorFullLoadCurve fullLoadCurve, ExecutionMode mode)
		{
			return Create(VectoCSVFile.ReadStream(data), count, ratio, fullLoadCurve, mode);
		}

		public static EfficiencyMap Create(DataTable data, int count, double ratio,
			ElectricMotorFullLoadCurve fullLoadCurve, ExecutionMode mode)
		{
			if (fullLoadCurve == null) {
				throw new ArgumentNullException("Provide fullloadcurve for extrapolation");
			}
			var headerValid = HeaderIsValid(data.Columns);
			if (!headerValid) {
				LoggingObject.Logger<IEPCMapReader>().Warn(
					"Efficiency Map: Header Line is not valid. Expected: '{0}, {1}, {2}', Got: {3}. Falling back to column index.",
					Fields.MotorSpeed,
					Fields.Torque,
					Fields.PowerElectrical,
					data.Columns.Cast<DataColumn>().Select(c => c.ColumnName).Join());
				data.Columns[0].ColumnName = Fields.MotorSpeed;
				data.Columns[1].ColumnName = Fields.Torque;
				data.Columns[2].ColumnName = Fields.PowerElectrical;
			}

			var entries = GetEntries(data, ratio, mode);
			entries = ExtendEfficiencyMap(entries, fullLoadCurve);
			var entriesZero = GetEntriesAtZeroRpm(entries);

			var delaunayMap = new DelaunayMap("ElectricMotorEfficiencyMap Mechanical to Electric");
			var retVal = new EfficiencyMapNew(delaunayMap);

			foreach (var entry in entriesZero.OrderBy(x => x.Torque)) {
				try {
					delaunayMap.AddPoint(-entry.Torque.Value() * count,
						0,
						retVal.GetDelaunayZValue(entry) * count);
				} catch (Exception e) {
					throw new VectoException($"EfficiencyMap - Entry {entry}: {e.Message}", ErrorCodes.IEPC_EFFICIENCY_MAP_BAD_ENTRY);
				}
			}

			foreach (var entry in entries.Where(x => x.MotorSpeed.IsGreater(0)).OrderBy(x => x.MotorSpeed)
						.ThenBy(x => x.Torque)) {
				try {
					delaunayMap.AddPoint(-entry.Torque.Value() * count,
						entry.MotorSpeed.Value(),
						retVal.GetDelaunayZValue(entry) * count);
				} catch (Exception e) {
					throw new VectoException($"EfficiencyMap - Entry {entry}: {e.Message}", ErrorCodes.IEPC_EFFICIENCY_MAP_BAD_ENTRY);
				}
			}

			delaunayMap.Triangulate();
			return retVal;
		}

		private static List<EfficiencyMap.Entry> GetEntriesAtZeroRpm(IList<EfficiencyMap.Entry> entries)
		{
			// find entries at first grid point above 0. em-speed might vary slightly,
			// so apply clustering, and use distance between first clusters to select all entries at lowest speed grid point

			const int numEntriesExtrapolationFitting = 4;

			var speeds = new MeanShiftClustering() { ClusterCount = 100 }.FindClusters(entries.Select(x => x.MotorSpeed.AsRPM).ToArray(), 10)
				.Where(x => x > 0).ToList();
			var lowerSpeed = speeds.First().RPMtoRad() / 2.0;
			var upperSpeed = speeds.First().RPMtoRad() + (speeds[1] - speeds.First()).RPMtoRad() / 2.0;

			//entries at lowest speed gridpoint
			var torquesMinRpm = entries.Where(x => x.MotorSpeed.IsBetween(lowerSpeed, upperSpeed)).OrderBy(x => x.Torque).ToList();
			// entries at 0 rpm grid point
			var torquesZeroRpm = entries.Where(x => x.MotorSpeed.IsEqual(0)).OrderBy(x => x.Torque).ToList();
			if (torquesZeroRpm.Count == 0) {
				throw new VectoException("Electric Motor PowerMap contains no entries at 0 rpm!", ErrorCodes.DATATABLE_INSUFFICIENT_DATA);
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
						"Failed to generate electric power map - at least two negative entries are required", ErrorCodes.DATATABLE_INSUFFICIENT_DATA);
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
						"Failed to generate electric power map - at least two positive entries are required", ErrorCodes.DATATABLE_INSUFFICIENT_DATA);
				}

				var (k, d) = VectoMath.LeastSquaresFitting(posTorque.Take(numEntriesExtrapolationFitting), x => x.Torque.Value(),
					x => x.PowerElectrical.Value());
				var extrapolatedPwr = (torquesZeroMax.Value() * k + d).SI<Watt>();
				entriesZero.Add(new EfficiencyMap.Entry(avgSpeed, torquesZeroRpm.Max(x => x.Torque), extrapolatedPwr));
			}

			return entriesZero;
		}

		private static IList<EfficiencyMap.Entry> ExtendEfficiencyMap(
			IList<EfficiencyMap.Entry> entries, ElectricMotorFullLoadCurve fullLoadCurve)
		{
			var clusterer = new MeanShiftClustering();
			var clusterTolerance = 50.RPMtoRad().Value();
			var extrapolationfactor = Constants.PowerMapSettings.EfficiencyMapExtrapolationFactor;

			//ignore entries where speed is 0, added manually to speed bucket (clustering doesn't work because the distance is too small)
			var cluster = clusterer.FindClusters(entries.Where(x => x.MotorSpeed.IsGreater(0)).Select(x => x.MotorSpeed.Value()).ToArray(), clusterTolerance);
			var minDistance = cluster.Pairwise((x, y) => Math.Abs(y - x)).Min();

			var speedBuckets = new Dictionary<PerSecond, List<EfficiencyMap.Entry>>(cluster.Length + 1);
			
			foreach (var c in cluster)
			{
				speedBuckets.Add(c.SI<PerSecond>(), new List<EfficiencyMap.Entry>());
			}
			foreach (var entry in entries.Where(x => x.MotorSpeed.IsGreater(0)))
			{
				foreach (var speed in speedBuckets.Keys.ToDouble())
				{
					if (Math.Abs(speed - entry.MotorSpeed.Value()) < minDistance / 2.0) {
						speedBuckets[speed.SI<PerSecond>()].Add(entry);
					}
				}
			}

			//Add zero rpm entries
			speedBuckets.Add(0.SI<PerSecond>(), new List<EfficiencyMap.Entry>());
			foreach (var entry in entries.Where(x => x.MotorSpeed.IsEqual(0))) {
				speedBuckets[0.SI<PerSecond>()].Add(entry);
			}


			//Don't extrapolate speedbuckets which have a left AND right neighbour with significantly higher torque values
			//

			var ignoredSpeedBucketsRecuperation = new HashSet<PerSecond>();
			var ignoredSpeedBucketsDrive = new HashSet<PerSecond>();
			var ignoreThreshold = 0.8;
			var orderedBuckets = speedBuckets.OrderBy(x => x.Key).ToList();
			for (var i = 1; i < speedBuckets.Count - 1; i++) {
				var current = orderedBuckets[i];
				var prev = orderedBuckets[i - 1];
				var next = orderedBuckets[i + 1];


				//Drive
				if (prev.Value.MinBy(x => x.Torque).Torque.IsEqual(0) || next.Value.MinBy(x => x.Torque).Torque.IsEqual(0)) {
					ignoredSpeedBucketsDrive.Add(current.Key);
				} else {
					if (current.Value.MinBy(x => x.Torque).Torque / prev.Value.MinBy(x => x.Torque).Torque <
						ignoreThreshold
						&&
						current.Value.MinBy(x => x.Torque).Torque / next.Value.MinBy(x => x.Torque).Torque <
						ignoreThreshold) {
						ignoredSpeedBucketsDrive.Add(current.Key);
					}
				}

				//Recuperation
				if (prev.Value.MaxBy(x => x.Torque).Torque.IsEqual(0) || next.Value.MaxBy(x => x.Torque).Torque.IsEqual(0)) {
					ignoredSpeedBucketsRecuperation.Add(current.Key);
				} else {
					if (current.Value.MaxBy(x => x.Torque).Torque / prev.Value.MaxBy(x => x.Torque).Torque <
						ignoreThreshold
						&&
						current.Value.MaxBy(x => x.Torque).Torque / next.Value.MaxBy(x => x.Torque).Torque <
						ignoreThreshold) {
						ignoredSpeedBucketsRecuperation.Add(current.Key);
					}
				}
			}


			// full-load curve has already inverted sign, power map is not yet sign-changed
			var maxTargetTorque = -fullLoadCurve.MaxDriveTorque * extrapolationfactor;
			var minTargetTorque = -fullLoadCurve.MaxGenerationTorque * extrapolationfactor;
			var ratedSpeed = ElectricMotorRatedSpeedHelper.GetRatedSpeed(fullLoadCurve.FullLoadEntries,
				e => e.MotorSpeed, e => e.FullDriveTorque);
			PerSecond prevSpeed = null;
			foreach (var speedBucket in orderedBuckets)
			{
				

				var maxRecuperationEntry = speedBucket.Value.MaxBy(x => x.Torque);
				var maxDriveEntry = speedBucket.Value.MinBy(x => x.Torque); //drive torque < 0
				var recuperationFactor = maxRecuperationEntry.Torque.IsEqual(0) ? 1.0 : maxTargetTorque / maxRecuperationEntry.Torque;
				var driveFactor = maxDriveEntry.Torque.IsEqual(0) ? 1.0 : minTargetTorque / maxDriveEntry.Torque;

				//Recuperation
				if (!recuperationFactor.IsSmallerOrEqual(1) && !ignoredSpeedBucketsRecuperation.Contains(speedBucket.Key)) {
					var nrExtrapolationPointsRecuperation = (uint)Math.Ceiling(speedBucket.Value.Count(x => x.Torque.IsGreater(0)) * (recuperationFactor - 1));
					for (var i = 1; i <= nrExtrapolationPointsRecuperation; i++)
					{
						var factor = CalculateExtrapolationFactor(recuperationFactor, i, nrExtrapolationPointsRecuperation);

						entries.Add(new EfficiencyMap.Entry(speedBucket.Key, maxRecuperationEntry.Torque * factor, maxRecuperationEntry.PowerElectrical * factor));
					}
				}

				//Drive
				if (!driveFactor.IsSmallerOrEqual(1) && !ignoredSpeedBucketsDrive.Contains(speedBucket.Key)) {

					var nrExtrapolationPointsDrive = (uint)Math.Ceiling(speedBucket.Value.Count(x => x.Torque.IsSmaller(0)) * (driveFactor - 1));

					for (var i = 1; i <= nrExtrapolationPointsDrive; i++)
					{
						var factor = CalculateExtrapolationFactor(driveFactor, i, nrExtrapolationPointsDrive);

						entries.Add(new EfficiencyMap.Entry(speedBucket.Key, maxDriveEntry.Torque * factor, maxDriveEntry.PowerElectrical * factor));
					}
				}


				if (prevSpeed != null && prevSpeed.IsGreaterOrEqual(ratedSpeed))
				{
					//update target values for next speed entry, 1 entry after rated speed should still be extrapolated to maxtorque * 1.2
					maxTargetTorque = - fullLoadCurve.FullLoadDriveTorque(speedBucket.Key) * extrapolationfactor;
					minTargetTorque = - fullLoadCurve.FullGenerationTorque(speedBucket.Key)  * extrapolationfactor;
				}
				prevSpeed = speedBucket.Key;
			}

			return entries;
		}

		private static double CalculateExtrapolationFactor(double targetFactor, int currentStep,
			uint nrOfSteps)
		{
			if (currentStep < 1) {
				throw new ArgumentException($"{nameof(currentStep)} must be >= 1");
			}
			var factor = 1 + ((targetFactor - 1)) * (currentStep / (double)nrOfSteps);
			return factor;
		}


		private static EfficiencyMap.Entry CreateEntry(DataRow row, double ratio)
		{
			return new EfficiencyMap.Entry(
				speed: row.ParseDouble(Fields.MotorSpeed).RPMtoRad() * ratio,
				torque: row.ParseDouble(Fields.Torque).SI<NewtonMeter>() / ratio,
				powerElectrical: row.ParseDouble(Fields.PowerElectrical).SI<Watt>());
		}

		internal static IList<EfficiencyMap.Entry> GetEntries(DataTable data, double ratio, ExecutionMode mode)
		{
			var entries = (from DataRow row in data.Rows select CreateEntry(row, ratio)).OrderBy(x => x.MotorSpeed)
				.ThenBy(x => x.Torque).ToList();

			var duplicates = entries.GroupBy(x => Tuple.Create(x.MotorSpeed, x.Torque)).Where(g => g.Count() > 1).Select(x => x.Key).ToList();
			if (duplicates.Count > 0) {
				throw new VectoException($"Duplicate entries in IEPC power map: {duplicates.Select(x => $"{x.Item1.AsRPM / ratio} rpm / {x.Item2 * ratio}").Join()}", ErrorCodes.DATATABLE_DUPLICATE_ENTRY);
			}
			var highEff = entries.Where(x => !x.Torque.IsEqual(0) && !x.MotorSpeed.IsEqual(0))
                .Select(x => Tuple.Create(x,
					x.Torque.IsGreater(0)
						? x.MotorSpeed * x.Torque / x.PowerElectrical
						: x.PowerElectrical / (x.MotorSpeed * x.Torque))).Where(x => x.Item2.IsGreater(1)).ToArray();
			if (highEff.Any(x => x.Item2.IsGreater(2))) {
				if (mode == ExecutionMode.Declaration) {
					throw new VectoException($"Electric motor power map contains entries with efficiencies > 2!{Environment.NewLine}{highEff.Length} entries:{Environment.NewLine}" + 
						$"{highEff.Select(x => $"{x.Item1.MotorSpeed.AsRPM} rpm {x.Item1.Torque} => {x.Item1.PowerElectrical}").Join(Environment.NewLine)}", 
						ErrorCodes.DATATABLE_INVALID_DATA,
						"Make sure the electric motor power map is correctly configured. Pay attention to the error message.");
				}

				LogManager.GetLogger(typeof(ElectricMotorMapReader).FullName).Debug(
					"Electric motor power map contains entries with efficiencies > 1! {0}",
					highEff.Select(x =>
						$"{x.Item1.MotorSpeed.AsRPM} rpm {x.Item1.Torque} => {x.Item1.PowerElectrical}").Join());

			}
			if (highEff.Any()) {
				LogManager.GetLogger(typeof(ElectricMotorMapReader).FullName).Debug(
					"Electric motor power map contains {0} entries with efficiencies > 1! These will be set to an efficiency of {1}!", highEff.Length, DeclarationData.ElectricMachineDefaultEfficiencyFallback);
				foreach (var entry in highEff) {
					// entry contains a referencee to the original entry. so changing it in this array also changes the entries array.
					entry.Item1.PowerElectrical = entry.Item1.Torque * entry.Item1.MotorSpeed *
												(entry.Item1.Torque.IsGreater(0)
													? 1 / DeclarationData.ElectricMachineDefaultEfficiencyFallback
													: DeclarationData.ElectricMachineDefaultEfficiencyFallback);
				}
			}
            return entries;
		}

		private static bool HeaderIsValid(DataColumnCollection columns)
		{
			return columns.Contains(Fields.MotorSpeed) && columns.Contains(Fields.Torque) &&
					columns.Contains(Fields.PowerElectrical);
		}

		public static class Fields
		{
			public const string MotorSpeed = "n";// = "n_out";
			public const string Torque = "T";// "_out";
			public const string PowerElectrical = "P_el";
		}


	}
}