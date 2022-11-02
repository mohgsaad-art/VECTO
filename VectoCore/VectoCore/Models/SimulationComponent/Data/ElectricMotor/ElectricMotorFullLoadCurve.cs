using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor {
	public class ElectricMotorFullLoadCurve : SimulationComponentData
	{
		internal readonly List<FullLoadEntry> FullLoadEntries;
		private NewtonMeter _maxDriveTorque;
		private NewtonMeter _maxGenerationTorque;
		private PerSecond _maxSpeed;
		private PerSecond _nP80Low;
		private Watt _maxPower;
		private PerSecond _ratedSpeed;


		internal ElectricMotorFullLoadCurve(List<FullLoadEntry> entries)
		{
			FullLoadEntries = entries;
		}


		public NewtonMeter FullLoadDriveTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].MotorSpeed, FullLoadEntries[idx].MotorSpeed,
				FullLoadEntries[idx - 1].FullDriveTorque, FullLoadEntries[idx].FullDriveTorque,
				angularVelocity);
		}

		public NewtonMeter FullGenerationTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(FullLoadEntries[idx - 1].MotorSpeed, FullLoadEntries[idx].MotorSpeed,
				FullLoadEntries[idx - 1].FullGenerationTorque, FullLoadEntries[idx].FullGenerationTorque,
				angularVelocity);
		}

		protected int FindIndex(PerSecond angularVelocity)
		{
			if (angularVelocity < FullLoadEntries.First().MotorSpeed)
			{
				return 1;
			}
			if (angularVelocity > FullLoadEntries.Last().MotorSpeed)
			{
				return FullLoadEntries.Count - 1;

			}
			for (var index = 1; index < FullLoadEntries.Count; index++)
			{
				if (angularVelocity >= FullLoadEntries[index - 1].MotorSpeed && angularVelocity <= FullLoadEntries[index].MotorSpeed)
				{
					return index;
				}
			}
			throw new VectoException("angular velocity {0} exceeds full-load curve. min: {1} max: {2}", angularVelocity, FullLoadEntries.First().MotorSpeed, FullLoadEntries.Last().MotorSpeed);

		}

		public NewtonMeter MaxDriveTorque
		{
			get { return _maxDriveTorque ?? (_maxDriveTorque = FullLoadEntries.Min(x => x.FullDriveTorque)); }
		}

		public NewtonMeter MaxGenerationTorque
		{
			get
			{
				return _maxGenerationTorque ??
						(_maxGenerationTorque = FullLoadEntries.Max(x => x.FullGenerationTorque));
			}
		}

		public PerSecond MaxSpeed
		{
			get { return _maxSpeed ?? (_maxSpeed = FullLoadEntries.Max(x => x.MotorSpeed)); }
		}

		[DebuggerDisplay("{MotorSpeed.AsRPM}: {FullDriveTorque} / {FullGenerationTorque}")]
		internal class FullLoadEntry
		{
			public PerSecond MotorSpeed { get; set; }

			public NewtonMeter FullDriveTorque { get; set; }

			public NewtonMeter FullGenerationTorque { get; set; }
		}

		public string[] SerializedEntries
		{
			get { return FullLoadEntries.Select(x => $"{x.MotorSpeed.AsRPM} {x.FullDriveTorque} {x.FullGenerationTorque}").ToArray(); }
		}

		public PerSecond NP80low => _nP80Low ?? (_nP80Low = ComputeNP80LowSpeed());

		public Watt MaxPower => _maxPower ?? (_maxPower = ComputeMaxPower().Item2);

		public PerSecond RatedSpeed => _ratedSpeed ?? (_ratedSpeed = ComputeMaxPower().Item1);

		private Tuple<PerSecond, Watt> ComputeMaxPower()
		{
			var max = new Tuple<PerSecond, Watt>(0.SI<PerSecond>(), 0.SI<Watt>());
			for (var idx = 1; idx < FullLoadEntries.Count; idx++) {
				var entry2 = FullLoadEntries[idx];
				if (FullLoadEntries[idx].MotorSpeed > MaxSpeed) {
					entry2 = new FullLoadEntry() {
						MotorSpeed = MaxSpeed,
						FullDriveTorque =  FullLoadDriveTorque(MaxSpeed),
						FullGenerationTorque = FullGenerationTorque(MaxSpeed)
					};
				}
				var currentMax = FindMaxPower(FullLoadEntries[idx - 1], entry2);
				if (currentMax.Item2 > max.Item2) {
					max = currentMax;
				}
			}

			return max;
		}

		private Tuple<PerSecond, Watt> FindMaxPower(FullLoadEntry p1, FullLoadEntry p2)
		{
			if (p1.MotorSpeed.IsEqual(p2.MotorSpeed)) {
				return Tuple.Create(p1.MotorSpeed, p1.FullDriveTorque * p1.MotorSpeed);
			}

			if (p2.MotorSpeed < p1.MotorSpeed) {
				var tmp = p1;
				p1 = p2;
				p2 = tmp;
			}

			// y = kx + d
			var k = (-p2.FullDriveTorque + p1.FullDriveTorque) / (p2.MotorSpeed - p1.MotorSpeed);
			var d = -p2.FullDriveTorque - k * p2.MotorSpeed;
			if (k.IsEqual(0)) {
				return Tuple.Create(p2.MotorSpeed, -p2.FullDriveTorque * p2.MotorSpeed);
			}
			var engineSpeedMaxPower = -d / (2 * k);
			if (engineSpeedMaxPower.IsSmaller(p1.MotorSpeed) || engineSpeedMaxPower.IsGreater(p2.MotorSpeed)) {
				if (-p2.FullDriveTorque * p2.MotorSpeed > -p1.FullDriveTorque * p1.MotorSpeed) {
					return Tuple.Create(p2.MotorSpeed, -p2.FullDriveTorque * p2.MotorSpeed);
				}
				return Tuple.Create(p1.MotorSpeed, -p1.FullDriveTorque * p1.MotorSpeed);
			}
			var engineTorqueMaxPower = FullLoadDriveTorque(engineSpeedMaxPower);
			return Tuple.Create(engineSpeedMaxPower, -engineTorqueMaxPower * engineSpeedMaxPower);
		}

		private PerSecond ComputeNP80LowSpeed()
		{
			var retVal = FindEngineSpeedForPower(0.8 * MaxPower).First();
			return retVal;
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

		private IEnumerable<PerSecond> FindEngineSpeedForPower(FullLoadEntry p1, FullLoadEntry p2, Watt power)
		{
			var k = (-p2.FullDriveTorque + p1.FullDriveTorque) / (p2.MotorSpeed - p1.MotorSpeed);
			var d = -p2.FullDriveTorque - k * p2.MotorSpeed;

			if (k.IsEqual(0, 0.0001)) {
				// constant torque: solve linear equation
				// power = M * n_eng_avg
				if (d.IsEqual(0, 0.0001)) {
					return new List<PerSecond>();
				}
				return FilterSolutions((power / d).ToEnumerable(), p1, p2);
			}

			// non-constant torque: solve quadratic equation for engine speed (n_eng_avg)
			// power = M(n_eng_avg) * n_eng_avg = (k * n_eng_avg + d) * n_eng_avg =  k * n_eng_avg^2 + d * n_eng_avg
			var retVal = VectoMath.QuadraticEquationSolver(k.Value(), d.Value(), -power.Value());
			if (retVal.Length == 0) {
				Log.Info("No real solution found for requested power demand: P: {0}, p1: {1}, p2: {2}", power, p1, p2);
			}
			return FilterSolutions(retVal.Select(x => Math.Round(x, 6).SI<PerSecond>()), p1, p2);
		}

		private IEnumerable<PerSecond> FilterSolutions(IEnumerable<PerSecond> solutions, FullLoadEntry p1, FullLoadEntry p2)
		{
			return solutions.Where(
				x => x.IsGreaterOrEqual(p1.MotorSpeed.Value()) && x.IsSmallerOrEqual(p2.MotorSpeed.Value()));
		}

		protected internal Watt ComputeArea(PerSecond lowEngineSpeed, PerSecond highEngineSpeed)
		{
			var startSegment = FindIndex(lowEngineSpeed);
			var endSegment = FindIndex(highEngineSpeed);

			var area = 0.SI<Watt>();
			if (lowEngineSpeed < FullLoadEntries[startSegment].MotorSpeed) {
				// add part of the first segment
				area += (FullLoadEntries[startSegment].MotorSpeed - lowEngineSpeed) *
						(FullLoadDriveTorque(lowEngineSpeed) + FullLoadEntries[startSegment].FullDriveTorque) / 2.0;
			}
			for (var i = startSegment + 1; i <= endSegment; i++) {
				var speedHigh = FullLoadEntries[i].MotorSpeed;
				var torqueHigh = FullLoadEntries[i].FullDriveTorque;
				if (highEngineSpeed < FullLoadEntries[i].MotorSpeed) {
					// add part of the last segment
					speedHigh = highEngineSpeed;
					torqueHigh = FullLoadDriveTorque(highEngineSpeed);
				}
				area += (speedHigh - FullLoadEntries[i - 1].MotorSpeed) * (torqueHigh + FullLoadEntries[i - 1].FullDriveTorque) /
						2.0;
			}
			return area;
		}
	}
}