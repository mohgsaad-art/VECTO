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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class AccelerationCurveData : SimulationComponentData
	{
		[Required, ValidateObject] private readonly List<KeyValuePair<MeterPerSecond, AccelerationEntry>> _entries;

		protected internal AccelerationCurveData(List<KeyValuePair<MeterPerSecond, AccelerationEntry>> entries)
		{
			_entries = entries;
			var smallValues = _entries.Where(e => e.Key < 5.KMPHtoMeterPerSecond()).OrderBy(e => e.Key).ToList();
			if (smallValues.Count >= 2) {
				Log.Error(
					"Found small velocity entries in Driver-Acceleration/Deceleration file. Values dismissed:" +
					smallValues.Skip(1).Select(e => e.Key.AsKmph.ToString("F1")).Join());
				foreach (var kv in smallValues.Skip(1)) {
					_entries.Remove(kv);
				}
			}
		}

		public AccelerationEntry Lookup(MeterPerSecond key)
		{
			var index = FindIndex(key);

			return new AccelerationEntry {
				Acceleration =
					VectoMath.Interpolate(
						_entries[index - 1].Key, _entries[index].Key,
						_entries[index - 1].Value.Acceleration,
						_entries[index].Value.Acceleration, key),
				Deceleration =
					VectoMath.Interpolate(
						_entries[index - 1].Key, _entries[index].Key,
						_entries[index - 1].Value.Deceleration,
						_entries[index].Value.Deceleration, key)
			};
		}

		protected int FindIndex(MeterPerSecond key)
		{
			var index = 1;
			if (key < _entries[0].Key) {
				Log.Error(
					"requested velocity below minimum - extrapolating. velocity: {0}, min: {1}",
					key.ConvertToKiloMeterPerHour(), _entries[0].Key.ConvertToKiloMeterPerHour());
			} else {
				index = _entries.FindIndex(x => x.Key > key);
				if (index <= 0) {
					index = key > _entries[0].Key ? _entries.Count - 1 : 1;
				}
			}
			return index;
		}

		public MeterPerSquareSecond MaxDeceleration()
		{
			return _entries.Min(x => x.Value.Deceleration);
		}

		public MeterPerSquareSecond MaxAcceleration()
		{
			return _entries.Max(x => x.Value.Acceleration);
		}

		[DebuggerDisplay("Acceleration: {Acceleration}, Deceleration: {Deceleration}")]
		public class AccelerationEntry
		{
			[Required, SIRange(0.05, 20)]
			public MeterPerSquareSecond Acceleration { get; set; }

			[Required, SIRange(-20, -0.05)]
			public MeterPerSquareSecond Deceleration { get; set; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="v1">current speed of the vehicle</param>
		/// <param name="v2">desired speed of the vehicle at the end of acceleration/deceleration phase</param>
		/// <returns>distance required to accelerate/decelerate the vehicle from v1 to v2 according to the acceleration curve</returns>
		public Meter ComputeDecelerationDistance(MeterPerSecond v1, MeterPerSecond v2)
		{
			var index1 = FindIndex(v1);
			var index2 = FindIndex(v2);

			var distance = 0.SI<Meter>();
			for (var i = index2; i <= index1; i++) {
				distance += ComputeAccelerationSegmentDistance(i, v1, v2);
			}

			return distance;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i">segment of the acceleration curve to use [(i-1) ... i]</param>
		/// <param name="v1">current speed of the vehicle</param>
		/// <param name="v2">desired speed of the vehicle at the end of acceleration/deceleration phase</param>
		/// <returns>distance required to accelerate/decelerate the vehicle from v1 to v2 according to the acceleration curve</returns>
		private Meter ComputeAccelerationSegmentDistance(int i, MeterPerSecond v1, MeterPerSecond v2)
		{
			var leftEntry = _entries[i - 1]; // entry with lower velocity
			var rightEntry = _entries[i]; // entry with higher velocity

			v2 = VectoMath.Max(v2, leftEntry.Key); // min. velocity within current segment
			v1 = VectoMath.Min(v1, rightEntry.Key); // max. velocity within current segment

			if (leftEntry.Value.Deceleration.IsEqual(rightEntry.Value.Deceleration)) {
				// v(t) = a * t + v1  => t = (v2 - v1) / a
				// s(t) = a/2 * t^2 + v1 * t + s0  {s0 == 0}  => s(t)
				var acceleration = v2 > v1 ? leftEntry.Value.Acceleration : leftEntry.Value.Deceleration;
				return ((v2 - v1) * (v2 - v1) / 2.0 / acceleration + v1 * (v2 - v1) / acceleration).Cast<Meter>();
			}

			// a(v) = k * v + d
			// dv/dt = a(v) = d * v + d  ==> v(t) = sgn(k * v1 + d) * exp(-k * c) / k * exp(t * k) - d / k 
			// v(0) = v1  => c = - ln(|v1 * k + d|) / k
			// v(t) = (v1 + d / k) * exp(t * k) - d / k   => t = 1 / k * ln((v2 * k + d) / (v1 * k + d))
			// s(t) = m / k* exp(t * k) + b * t + c'   {m = v1 + d / k, b = -d / k}

			var k = (leftEntry.Value.Deceleration - rightEntry.Value.Deceleration) / (leftEntry.Key - rightEntry.Key);
			var d = leftEntry.Value.Deceleration - k * leftEntry.Key;
			if (v2 > v1) {
				if (leftEntry.Value.Acceleration.IsEqual(rightEntry.Value.Acceleration)) {
					var acceleration =  leftEntry.Value.Acceleration;
					return ((v2 - v1) * (v2 - v1) / 2.0 / acceleration + v1 * (v2 - v1) / acceleration).Cast<Meter>();
				}
				k = (leftEntry.Value.Acceleration - rightEntry.Value.Acceleration) / (leftEntry.Key - rightEntry.Key);
				d = leftEntry.Value.Acceleration - k * leftEntry.Key;
				
			}
			var m = v1 + d / k;
			var b = -d / k;
			var c = 0.SI<Meter>() - m / k;
			var t = Math.Log(((v2 * k + d) / (v1 * k + d)).Cast<Scalar>()) / k;
			return m / k * Math.Exp((k * t).Value()) + b * t + c;
		}

		public MeterPerSecond ComputeEndVelocityAccelerate(MeterPerSecond startSpeed, Second accelerationTime)
		{
			while (true) {
				var index1 = FindIndex(startSpeed);

				var leftEntry = _entries[index1 - 1];
				var rightEntry = _entries[index1];

				if (leftEntry.Value.Acceleration.IsEqual(rightEntry.Value.Acceleration)) {
					// v(t) = a * t + v1  => t = (v2 - v1) / a
					var accTime = (rightEntry.Key - startSpeed) / leftEntry.Value.Acceleration;
					if (accTime > accelerationTime) {
						return startSpeed + leftEntry.Value.Acceleration * accelerationTime;
					}

					startSpeed = rightEntry.Key;
					accelerationTime = accelerationTime - accTime;
				} else {
					// a(v) = k * v + d
					// dv/dt = a(v) = d * v + d  ==> v(t) = sgn(k * v1 + d) * exp(-k * c) / k * exp(t * k) - d / k 
					// v(0) = v1  => c = - ln(|v1 * k + d|) / k
					// v(t) = (v1 + d / k) * exp(t * k) - d / k   => t = 1 / k * ln((v2 * k + d) / (v1 * k + d))
					var k = (leftEntry.Value.Acceleration - rightEntry.Value.Acceleration) / (leftEntry.Key - rightEntry.Key);
					var d = leftEntry.Value.Acceleration - k * leftEntry.Key;
					var t = Math.Log(((rightEntry.Key * k + d) / (startSpeed * k + d)).Cast<Scalar>()) / k;
					if (t > accelerationTime) {
						return (startSpeed + d / k) * Math.Exp((accelerationTime * k).Value()) - d / k;
					}

					startSpeed = rightEntry.Key;
					accelerationTime = accelerationTime - t;
				}

				if (index1 == _entries.Count - 1) {
					return _entries[index1].Key;
				}
			}
		}
	}
}