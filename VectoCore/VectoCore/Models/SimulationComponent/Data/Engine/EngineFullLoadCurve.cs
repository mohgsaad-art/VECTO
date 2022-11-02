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
using Newtonsoft.Json;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
{
	/// <summary>
	/// Represents the Full load curve.
	/// </summary>
	public class EngineFullLoadCurve : SimulationComponentData
	{
		private Watt _maxPower;
		private PerSecond _ratedSpeed;
		private NewtonMeter _maxTorque;
		private NewtonMeter _maxDragTorque;

		private PerSecond _preferredSpeed;
		private PerSecond _engineSpeedLo; // 55% of Pmax
		//private PerSecond _engineSpeedHi; // 70% of Pmax
		private PerSecond _n95hSpeed; // 95% of Pmax
		private PerSecond _n80hSpeed; // 80% of Pmax

		private PerSecond _nTq99hSpeed; // 99% of max Torque above rated
		private PerSecond _nTq99lSpeed; // 99% of max Torque below rated speed
		private PerSecond _nP99hSpeed; // 99% of Pmax above rated speed

		private PerSecond _nTq98hSpeed; // 99% of max Torque above rated
		private PerSecond _nP98hSpeed; // 99% of Pmax above rated speed


		[Required, ValidateObject] internal readonly List<FullLoadCurveEntry> FullLoadEntries;

		private SortedList<PerSecond, int> _quickLookup;

		[Required] public readonly LookupData<PerSecond, PT1.PT1Result> PT1Data;

		internal EngineFullLoadCurve(List<FullLoadCurveEntry> entries, LookupData<PerSecond, PT1.PT1Result> pt1Data)
		{
			FullLoadEntries = entries;
			PT1Data = pt1Data;
		}

		public Watt FullLoadStationaryPower(PerSecond angularVelocity)
		{
			return Formulas.TorqueToPower(FullLoadStationaryTorque(angularVelocity), angularVelocity);
		}

		public Watt DragLoadStationaryPower(PerSecond angularVelocity)
		{
			return Formulas.TorqueToPower(DragLoadStationaryTorque(angularVelocity), angularVelocity);
		}

		[JsonIgnore]
		public CombustionEngineData EngineData { get; internal set; }

		public PT1.PT1Result PT1(PerSecond angularVelocity)
		{
			return PT1Data.Lookup(angularVelocity);
		}

		public string[] FullLoadCurve
		{
			get { return FullLoadEntries.Select(x => $"{x.EngineSpeed.AsRPM} [rpm], {x.TorqueFullLoad}, {x.TorqueDrag}").ToArray(); }
		}
		
		/// <summary>
		/// Get the rated speed from the given full-load curve (i.e. speed with max. power)
		/// </summary>
		[Required, SIRange(0, 5000 * Constants.RPMToRad)]
		public PerSecond RatedSpeed => _ratedSpeed ?? ComputeRatedSpeed().Item1;

		/// <summary>
		/// Gets the maximum power.
		/// </summary>
		[Required, SIRange(0, 10000 * 5000 * Constants.RPMToRad)]
		public Watt MaxPower => _maxPower ?? ComputeRatedSpeed().Item2;

		public NewtonMeter MaxTorque
		{
			get { return _maxTorque ?? (_maxTorque = FullLoadEntries.Max(x => x.TorqueFullLoad)); }
		}

		public NewtonMeter MaxDragTorque
		{
			get { return _maxDragTorque ?? (_maxDragTorque = FullLoadEntries.Min(x => x.TorqueDrag)); }
		}

		public NewtonMeter FullLoadStationaryTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].EngineSpeed, FullLoadEntries[idx].EngineSpeed,
				FullLoadEntries[idx - 1].TorqueFullLoad, FullLoadEntries[idx].TorqueFullLoad,
				angularVelocity);
		}

		public NewtonMeter DragLoadStationaryTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].EngineSpeed, FullLoadEntries[idx].EngineSpeed,
				FullLoadEntries[idx - 1].TorqueDrag, FullLoadEntries[idx].TorqueDrag,
				angularVelocity);
		}

		
		/// <summary>
		/// Compute the engine's rated speed from the given full-load curve (i.e. engine speed with max. power)
		/// </summary>
		protected Tuple<PerSecond, Watt> ComputeRatedSpeed()
		{
			var max = new Tuple<PerSecond, Watt>(0.SI<PerSecond>(), 0.SI<Watt>());
			for (var idx = 1; idx < FullLoadEntries.Count; idx++) {
				var currentMax = FindMaxPower(FullLoadEntries[idx - 1], FullLoadEntries[idx]);
				if (currentMax.Item2 > max.Item2) {
					max = currentMax;
				}
			}

			_ratedSpeed = max.Item1;
			_maxPower = max.Item2;

			return max;
		}

		private Tuple<PerSecond, Watt> FindMaxPower(FullLoadCurveEntry p1, FullLoadCurveEntry p2)
		{
			if (p1.EngineSpeed.IsEqual(p2.EngineSpeed)) {
				return Tuple.Create(p1.EngineSpeed, p1.TorqueFullLoad * p1.EngineSpeed);
			}

			if (p2.EngineSpeed < p1.EngineSpeed) {
				var tmp = p1;
				p1 = p2;
				p2 = tmp;
			}

			// y = kx + d
			var k = (p2.TorqueFullLoad - p1.TorqueFullLoad) / (p2.EngineSpeed - p1.EngineSpeed);
			var d = p2.TorqueFullLoad - k * p2.EngineSpeed;
			if (k.IsEqual(0)) {
				return Tuple.Create(p2.EngineSpeed, p2.TorqueFullLoad * p2.EngineSpeed);
			}
			var engineSpeedMaxPower = -d / (2 * k);
			if (engineSpeedMaxPower.IsSmaller(p1.EngineSpeed) || engineSpeedMaxPower.IsGreater(p2.EngineSpeed)) {
				if (p2.TorqueFullLoad * p2.EngineSpeed > p1.TorqueFullLoad * p1.EngineSpeed) {
					return Tuple.Create(p2.EngineSpeed, p2.TorqueFullLoad * p2.EngineSpeed);
				}
				return Tuple.Create(p1.EngineSpeed, p1.TorqueFullLoad * p1.EngineSpeed);
			}
			var engineTorqueMaxPower = FullLoadStationaryTorque(engineSpeedMaxPower);
			return Tuple.Create(engineSpeedMaxPower, engineTorqueMaxPower * engineSpeedMaxPower);
		}


		/// <summary>
		/// Get item index for the segment of the full-load curve where the angularVelocity lies within.
		/// </summary>
		protected int FindIndex(PerSecond angularVelocity)
		{
			if (angularVelocity < FullLoadEntries.First().EngineSpeed) {
				return 1;
			}
			if (angularVelocity > FullLoadEntries.Last().EngineSpeed) {
				return FullLoadEntries.Count - 1;
			}

			if (_quickLookup == null) {
				_quickLookup = new SortedList<PerSecond, int>();
				var i = 10;
				for (; i < FullLoadEntries.Count; i += 10) {
					_quickLookup.Add(FullLoadEntries[i].EngineSpeed, Math.Max(1, i - 10));
				}
				_quickLookup.Add(FullLoadEntries.Last().EngineSpeed + 0.1.SI<PerSecond>(), Math.Max(1, i - 10));
			}
			var start = 1;
			foreach (var lookup in _quickLookup.Where(lookup => angularVelocity < lookup.Key)) {
				start = lookup.Value;
				break;
			}

			for (var index = start; index < FullLoadEntries.Count; index++) {
				if (angularVelocity >= FullLoadEntries[index - 1].EngineSpeed &&
					angularVelocity <= FullLoadEntries[index].EngineSpeed) {
					return index;
				}
			}
			throw new VectoException("angular velocity {0} exceeds full load curve: min: {1}  max: {2}", angularVelocity,
				FullLoadEntries.First().EngineSpeed, FullLoadEntries.Last().EngineSpeed);
		}

		/// <summary>
		///	Get the engine's preferred speed from the given full-load curve (i.e. Speed at 51% torque/speed-integral between idling and N95h.)
		/// </summary>
		public PerSecond PreferredSpeed
		{
			get {
				if (_preferredSpeed == null) {
					ComputePreferredSpeed();
				}
				return _preferredSpeed;
			}
		}

		public PerSecond N80hSpeed
		{
			get {
				if (_n80hSpeed != null) {
					return _n80hSpeed;
				}
				_n80hSpeed = FindEngineSpeedForPower(0.8 * MaxPower).Last();
				if (_n80hSpeed <= RatedSpeed) {
					throw new VectoException(
						"failed to compute N80h speed - must be higher than rated speed. rated speed: {0}, n80h speed: {1}", RatedSpeed,
						_n80hSpeed);
				}
				return _n80hSpeed;
			}
		}

		public PerSecond N95hSpeed
		{
			get {
				if (_n95hSpeed != null) {
					return _n95hSpeed;
				}
				_n95hSpeed = FindEngineSpeedForPower(0.95 * MaxPower).Last();
				if (_n95hSpeed <= RatedSpeed) {
					throw new VectoException(
						"failed to compute N95h speed - must be higher than rated speed. rated speed: {0}, n95h speed: {1}", RatedSpeed,
						_n95hSpeed);
				}
				return _n95hSpeed;
			}
		}

		public PerSecond NTq99hSpeed => _nTq99hSpeed ?? (_nTq99hSpeed = FindEnginSpeedForTorque(0.99 * MaxTorque).Last());

		public PerSecond NTq99lSpeed => _nTq99lSpeed ?? (_nTq99lSpeed = FindEnginSpeedForTorque(0.99 * MaxTorque).First());

		public PerSecond NP99hSpeed => _nP99hSpeed ?? (_nP99hSpeed = ComputeNP99HSpeed());

		public PerSecond NTq98hSpeed => _nTq98hSpeed ?? (_nTq98hSpeed = FindEnginSpeedForTorque(0.98 * MaxTorque).Last());

		public PerSecond NP98hSpeed => _nP98hSpeed ?? (_nP98hSpeed = ComputeNP98HSpeed());

		private PerSecond ComputeNP99HSpeed()
		{
			var retVal = FindEngineSpeedForPower(0.99 * MaxPower).Last();
			if (retVal <= RatedSpeed) {
				throw new VectoException(
					"failed to compute n_P99H - must be higher than rated speed. rated speed: {0}, n_tq99h: {1}", RatedSpeed, retVal);
			}
			return retVal;
		}

		private PerSecond ComputeNP98HSpeed()
		{
			var retVal = FindEngineSpeedForPower(0.98 * MaxPower).Last();
			if (retVal <= RatedSpeed) {
				throw new VectoException(
					"failed to compute n_P99H - must be higher than rated speed. rated speed: {0}, n_tq99h: {1}", RatedSpeed, retVal);
			}
			return retVal;
		}

		public PerSecond LoSpeed => _engineSpeedLo ?? (_engineSpeedLo = FindEngineSpeedForPower(0.55 * MaxPower).First());

		//public PerSecond HiSpeed
		//{
		//	get {
		//		if (_engineSpeedHi != null) {
		//			return _engineSpeedHi;
		//		}
		//		_engineSpeedHi = FindEngineSpeedForPower(0.7 * MaxPower).Last();
		//		if (_engineSpeedHi <= RatedSpeed) {
		//			throw new VectoException(
		//				"failed to compute n70h speed - must be higher than rated speed. rated speed: {0}, n70h speed: {1}", RatedSpeed,
		//				_engineSpeedHi);
		//		}
		//		return _engineSpeedHi;
		//	}
		//}

		private void ComputePreferredSpeed()
		{
			var maxArea = ComputeArea(EngineData.IdleSpeed, N95hSpeed);

			var area = 0.SI<Watt>();
			var idx = 0;
			while (++idx < FullLoadEntries.Count) {
				var additionalArea = ComputeArea(FullLoadEntries[idx - 1].EngineSpeed, FullLoadEntries[idx].EngineSpeed);
				if (area + additionalArea > 0.51 * maxArea) {
					var deltaArea = 0.51 * maxArea - area;
					_preferredSpeed = ComputeEngineSpeedForSegmentArea(FullLoadEntries[idx - 1], FullLoadEntries[idx], deltaArea);
					return;
				}
				area += additionalArea;
			}
			Log.Warn("Could not compute preferred speed, check FullLoadCurve! N95h: {0}, maxArea: {1}", N95hSpeed, maxArea);
		}

		private PerSecond ComputeEngineSpeedForSegmentArea(FullLoadCurveEntry p1, FullLoadCurveEntry p2, Watt area)
		{
			var k = (p2.TorqueFullLoad - p1.TorqueFullLoad) / (p2.EngineSpeed - p1.EngineSpeed);
			var d = p2.TorqueFullLoad - k * p2.EngineSpeed;

			if (k.IsEqual(0)) {
				// rectangle
				// area = M * n_eng_avg
				return p1.EngineSpeed + area / d;
			}

			// non-constant torque, M(n) = k * n + d
			// area = (M(n1) + M(n2))/2 * (n2 - n1) => solve for n2
			var retVal = VectoMath.QuadraticEquationSolver(k.Value() / 2.0, d.Value(),
				(-k * p1.EngineSpeed * p1.EngineSpeed / 2 - p1.EngineSpeed * d - area).Value());
			if (retVal.Length == 0) {
				Log.Info("No real solution found for requested area: P: {0}, p1: {1}, p2: {2}", area, p1, p2);
			}
			return retVal.First(x => x.IsBetween(p1.EngineSpeed, p2.EngineSpeed)).SI<PerSecond>();
		}

		private List<PerSecond> FindEnginSpeedForTorque(NewtonMeter torque)
		{
			var retVal = new List<PerSecond>();
			foreach (var pair in FullLoadEntries.Pairwise(Tuple.Create)) {
				var solution = FindEnginSpeedForTorque(pair.Item1, pair.Item2, torque);
				if (solution != null) {
					retVal.Add(solution);
				}
			}
			retVal.Sort();
			return retVal.Distinct(new SI.EqualityComparer<PerSecond>()).ToList();
		}

		private PerSecond FindEnginSpeedForTorque(FullLoadCurveEntry p1, FullLoadCurveEntry p2, NewtonMeter torque)
		{
			if (p1.TorqueFullLoad.IsEqual(p2.TorqueFullLoad) && p1.TorqueFullLoad.IsEqual(torque)) {
				// horizontal line in FLD that equals requested torque
				return p2.EngineSpeed;
			}
			if (torque.IsBetween(p1.TorqueFullLoad, p2.TorqueFullLoad)) {
				return VectoMath.Interpolate(p1.TorqueFullLoad, p2.TorqueFullLoad, p1.EngineSpeed, p2.EngineSpeed, torque);
			}
			return null;
		}

		private List<PerSecond> FindEngineSpeedForPower(Watt power)
		{
			var retVal = new List<PerSecond>();
			for (var idx = 1; idx < FullLoadEntries.Count; idx++) {
				var solutions = FindEngineSpeedForPower(FullLoadEntries[idx - 1], FullLoadEntries[idx], power);
				retVal.AddRange(solutions);
			}
			retVal.Sort();
			return retVal.Distinct(new SI.EqualityComparer<PerSecond>()).ToList();
		}

		private IEnumerable<PerSecond> FindEngineSpeedForPower(FullLoadCurveEntry p1, FullLoadCurveEntry p2, Watt power)
		{
			var k = (p2.TorqueFullLoad - p1.TorqueFullLoad) / (p2.EngineSpeed - p1.EngineSpeed);
			var d = p2.TorqueFullLoad - k * p2.EngineSpeed;

			if (k.IsEqual(0, 0.0001)) {
				// constant torque: solve linear equation
				// power = M * n_eng_avg
				if (d.IsEqual(0, 0.0001)) {
					return new List<PerSecond>();
				}
				return  FilterSolutions((power / d).ToEnumerable(), p1, p2);
			}

			// non-constant torque: solve quadratic equation for engine speed (n_eng_avg)
			// power = M(n_eng_avg) * n_eng_avg = (k * n_eng_avg + d) * n_eng_avg =  k * n_eng_avg^2 + d * n_eng_avg
			var retVal = VectoMath.QuadraticEquationSolver(k.Value(), d.Value(), -power.Value());
			if (retVal.Length == 0) {
				Log.Info("No real solution found for requested power demand: P: {0}, p1: {1}, p2: {2}", power, p1, p2);
			}
			return FilterSolutions(retVal.Select(x => Math.Round(x, 6).SI<PerSecond>()), p1, p2);
		}

		private IEnumerable<PerSecond> FilterSolutions(IEnumerable<PerSecond> solutions, FullLoadCurveEntry p1, FullLoadCurveEntry p2)
		{
			return solutions.Where(
				x => x.IsGreaterOrEqual(p1.EngineSpeed.Value()) && x.IsSmallerOrEqual(p2.EngineSpeed.Value()));
		}

		protected internal Watt ComputeArea(PerSecond lowEngineSpeed, PerSecond highEngineSpeed)
		{
			var startSegment = FindIndex(lowEngineSpeed);
			var endSegment = FindIndex(highEngineSpeed);

			var area = 0.SI<Watt>();
			if (lowEngineSpeed < FullLoadEntries[startSegment].EngineSpeed) {
				// add part of the first segment
				area += (FullLoadEntries[startSegment].EngineSpeed - lowEngineSpeed) *
						(FullLoadStationaryTorque(lowEngineSpeed) + FullLoadEntries[startSegment].TorqueFullLoad) / 2.0;
			}
			for (var i = startSegment + 1; i <= endSegment; i++) {
				var speedHigh = FullLoadEntries[i].EngineSpeed;
				var torqueHigh = FullLoadEntries[i].TorqueFullLoad;
				if (highEngineSpeed < FullLoadEntries[i].EngineSpeed) {
					// add part of the last segment
					speedHigh = highEngineSpeed;
					torqueHigh = FullLoadStationaryTorque(highEngineSpeed);
				}
				area += (speedHigh - FullLoadEntries[i - 1].EngineSpeed) * (torqueHigh + FullLoadEntries[i - 1].TorqueFullLoad) /
						2.0;
			}
			return area;
		}

		#region Equality members

		protected bool Equals(EngineFullLoadCurve other)
		{
			return Equals(FullLoadEntries, other.FullLoadEntries) && Equals(PT1Data, other.PT1Data);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != GetType()) {
				return false;
			}
			return Equals((EngineFullLoadCurve)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				return ((FullLoadEntries != null ? FullLoadEntries.GetHashCode() : 0) * 397) ^
						(PT1Data != null ? PT1Data.GetHashCode() : 0);
			}
		}

		#endregion

		[DebuggerDisplay("n: {EngineSpeed}, fullTorque: {TorqueFullLoad}, dragTorque: {TorqueDrag}")]
		public class FullLoadCurveEntry
		{
			[Required, SIRange(0, 5000 * Constants.RPMToRad)]
			public PerSecond EngineSpeed { get; set; }

			[Required, SIRange(0, 10000)]
			public NewtonMeter TorqueFullLoad { get; set; }

			[Required, SIRange(-10000, 0)]
			public NewtonMeter TorqueDrag { get; set; }

			#region Equality members

			protected bool Equals(FullLoadCurveEntry other)
			{
				return Equals(EngineSpeed, other.EngineSpeed) && Equals(TorqueFullLoad, other.TorqueFullLoad) &&
						Equals(TorqueDrag, other.TorqueDrag);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) {
					return false;
				}
				if (ReferenceEquals(this, obj)) {
					return true;
				}
				if (obj.GetType() != GetType()) {
					return false;
				}
				return Equals((FullLoadCurveEntry)obj);
			}

			public override int GetHashCode()
			{
				unchecked {
					var hashCode = EngineSpeed != null ? EngineSpeed.GetHashCode() : 0;
					hashCode = (hashCode * 397) ^ (TorqueFullLoad != null ? TorqueFullLoad.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ (TorqueDrag != null ? TorqueDrag.GetHashCode() : 0);
					return hashCode;
				}
			}

			#endregion
		}
	}
}