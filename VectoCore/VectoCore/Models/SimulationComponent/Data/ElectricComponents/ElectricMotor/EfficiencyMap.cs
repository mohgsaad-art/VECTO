using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor
{

	public class EfficiencyMapNew : EfficiencyMap, IEquatable<EfficiencyMapNew>
	{
		
		protected internal EfficiencyMapNew(DelaunayMap efficiencyMapMech2El) : base(efficiencyMapMech2El) { }

        public bool Equals(EfficiencyMapNew other) => base.Equals(other);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj) => Equals(obj as EfficiencyMapNew);

        public static bool operator ==(EfficiencyMapNew map1, EfficiencyMapNew map2)
        {
            if (ReferenceEquals(map1, map2))
            {
                return true;
            }

            if (ReferenceEquals(map1, null) || ReferenceEquals(map2, null))
            {
                return false;
            }

            return map1.Equals(map2);
        }

        public static bool operator !=(EfficiencyMapNew map1, EfficiencyMapNew map2) => !(map1 == map2);

        public override EfficiencyResult LookupElectricPower(PerSecond angularSpeed, NewtonMeter torque, bool allowExtrapolation = false)
		{
			var result = new EfficiencyResult();
			result.Torque = torque;
			result.Speed = angularSpeed;
			var value = _efficiencyMapMech2El.Interpolate(torque, angularSpeed);
			if (!value.IsNaN()) {
				result.ElectricalPower = value.SI<NewtonMeter>() * (angularSpeed) + angularSpeed * torque;
				return result;
			}
			if (allowExtrapolation) {
				value = _efficiencyMapMech2El.Extrapolate(torque, angularSpeed);
				result.ElectricalPower = value.SI<NewtonMeter>() * (angularSpeed) + angularSpeed * torque;
				result.Extrapolated = true;
				return result;
			}
			return result;
		}

		public double GetDelaunayZValue(Entry entry)
		{
			if (entry.MotorSpeed.IsEqual(0)) {
				throw new VectoException("Electric motor speed has to be greater than 0", ErrorCodes.ELECTRICMOTOR_SPEED_IS_ZERO);
			}
			return - ((entry.PowerElectrical - entry.MotorSpeed * entry.Torque) /
						(entry.MotorSpeed)).Value();
		}

		public override string[] SerializedEntries {
			get {
				return _efficiencyMapMech2El.Entries.Select(
						entry => $"{entry.Y.SI<PerSecond>().AsRPM} [rpm], {entry.X.SI<NewtonMeter>()}, {(entry.Z * entry.Y + entry.Y * entry.X).SI<Watt>()} ({entry.Z.SI<NewtonMeter>()})")
					.ToArray();
			}
		}
    }

	public class EfficiencyMap : LoggingObject, IEquatable<EfficiencyMap>
	{
		protected readonly DelaunayMap _efficiencyMapMech2El;
		private PerSecond _maxSpeed;

		protected internal EfficiencyMap(DelaunayMap efficiencyMapMech2El)
		{
			_efficiencyMapMech2El = efficiencyMapMech2El;
		}

        public override bool Equals(object obj) => Equals(obj as EfficiencyMap);

        public override int GetHashCode()
        {
            return _efficiencyMapMech2El.GetHashCode();
        }

		public bool Equals(EfficiencyMap other)
		{
			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (ReferenceEquals(other, null))
			{
				return false;
			}

			return (_efficiencyMapMech2El == other._efficiencyMapMech2El);
		}

        public static bool operator ==(EfficiencyMap map1, EfficiencyMap map2)
        {
            if (ReferenceEquals(map1, map2))
            {
                return true;
            }

            if (ReferenceEquals(map1, null) || ReferenceEquals(map2, null))
            {
                return false;
            }

            return map1.Equals(map2);
        }

        public static bool operator !=(EfficiencyMap map1, EfficiencyMap map2) => !(map1 == map2);

        public virtual EfficiencyResult LookupElectricPower(PerSecond angularSpeed, NewtonMeter torque, bool allowExtrapolation = false)
		{
			var result = new EfficiencyResult();
			result.Torque = torque;
			result.Speed = angularSpeed;
			var value = _efficiencyMapMech2El.Interpolate(torque, angularSpeed);
			if (!value.IsNaN())
			{
				result.ElectricalPower = value.SI<Watt>();
				return result;
			}
			if (allowExtrapolation)
			{
				result.ElectricalPower = _efficiencyMapMech2El.Extrapolate(torque, angularSpeed).SI<Watt>();
				result.Extrapolated = true;
				return result;
			}
			return result;
		}

		public virtual string[] SerializedEntries
		{
			get { return _efficiencyMapMech2El.Entries.Select(
												entry => $"{entry.Y.SI<PerSecond>().AsRPM} [rpm], {entry.X.SI<NewtonMeter>()}, {entry.Z.SI<Watt>()}")
											.ToArray();
			}
		}

		[JsonIgnore]
		public IReadOnlyCollection<Entry> Entries
		{
			get
			{
				var entries = _efficiencyMapMech2El.Entries;
				var retVal = new Entry[entries.Count];
				var i = 0;
				foreach (var entry in entries)
				{
					retVal[i++] = new Entry(entry.Y.SI<PerSecond>(), entry.X.SI<NewtonMeter>(), entry.Z.SI<Watt>());
				}
				return retVal;
			}
		}

        public class Entry
		{
			public Entry(PerSecond speed, NewtonMeter torque, Watt powerElectrical)
			{
				MotorSpeed = speed;
				Torque = torque;
				PowerElectrical = powerElectrical;
			}

			public readonly PerSecond MotorSpeed;
			public readonly NewtonMeter Torque;
			public Watt PowerElectrical;

			public override string ToString()
			{
				return $"{MotorSpeed.AsRPM} [rpm] / {Torque} / {PowerElectrical}";
			}
		}

		public class EfficiencyResult
		{
			public PerSecond Speed;
			public NewtonMeter Torque;
			public Watt ElectricalPower;
			public bool Extrapolated;
		}

		public NewtonMeter LookupTorque(Watt batPower, PerSecond avgSpeed, NewtonMeter maxEmTorque)
		{
			var elPowerMaxEM = LookupElectricPower(avgSpeed, maxEmTorque, true);
			if (maxEmTorque < 0) { // distinguish between propulsion and recuperation
				if (!elPowerMaxEM.Extrapolated && elPowerMaxEM.ElectricalPower.IsGreaterOrEqual(batPower)) {
					// the battery can provide more electric power than the EM  - no limitation here
					return null;
				}
			} else {
				if (!elPowerMaxEM.Extrapolated && elPowerMaxEM.ElectricalPower.IsSmallerOrEqual(batPower)) {
					// the battery can provide more electric power than the EM  - no limitation here
					return null;
				}
			}

			if (batPower.IsEqual(0, 1e-3)) {
				return null;
			}

			if (avgSpeed.IsEqual(0.RPMtoRad()) || avgSpeed.IsGreaterOrEqual(MaxSpeed)) {
				return 0.SI<NewtonMeter>();
			}

			try {
				var retVal = SearchTorqueForElectricPower(batPower, avgSpeed, maxEmTorque, elPowerMaxEM);
				var tmp = LookupElectricPower(avgSpeed, retVal, true);
				if (VectoMath.Abs(tmp.ElectricalPower - batPower).IsGreater(Constants.SimulationSettings.InterpolateSearchTolerance)) {
					// searched operating point is not accurate enough...
					retVal = SearchTorqueForElectricPower(batPower, avgSpeed, maxEmTorque, elPowerMaxEM, 1e3);
				}

				if (maxEmTorque < 0) {
					// propelling
					if (retVal.IsSmaller(maxEmTorque)) {
						retVal = SearchTorqueForElectricPower(batPower, avgSpeed, maxEmTorque, elPowerMaxEM, 1e3, true);
						if (retVal.IsSmaller(maxEmTorque)) {
							// the line search does not yield a better solution, still 'above' max EM torque - battery power is sufficient
							return null;
						}
					}
				} else {
					// recuperating
					if (retVal.IsGreater(maxEmTorque)) {
						retVal = SearchTorqueForElectricPower(batPower, avgSpeed, maxEmTorque, elPowerMaxEM, 1e3, true);
						if (retVal.IsGreater(maxEmTorque)) {
							// the line search does not yield a better solution, still 'above' max EM torque - battery power is sufficient
							return null;
						}
					}
                }
				return retVal;
			} 
			catch (VectoException e) 
			{
				if (e.GetErrorCode() == ErrorCodes.SEARCH_FAILED)
				{
#if DEBUG
                    Log.Warn("Failed to find mechanic power for given electric power! n_avg: {0} P_el: {1}; {2}", avgSpeed.AsRPM, batPower, e.Message);
#endif
                }
                else
				{
					throw;
				}
			}

			return null;
		}

		private NewtonMeter SearchTorqueForElectricPower(Watt batPower, PerSecond avgSpeed, NewtonMeter maxEmTorque,
			EfficiencyResult elPowerMaxEM, double factor = 1.0, bool forceLinesearch = false)
		{
			var delta = -maxEmTorque * 0.1 * (maxEmTorque > 0 ? -1 : 1);
			if (delta.IsEqual(0)) {
				delta = 0.1.SI<NewtonMeter>();
			}
            var retVal = SearchAlgorithm.Search(
				maxEmTorque, elPowerMaxEM.ElectricalPower - batPower,
				delta,
				getYValue: x => {
					var myX = (EfficiencyResult)x;
					return (myX.ElectricalPower - batPower) * factor;
				},
				evaluateFunction: x => LookupElectricPower(avgSpeed, x, true),
				criterion: x => {
					var myX = (EfficiencyResult)x;
					return (myX.ElectricalPower - batPower).Value() * factor;
				},
				searcher: this,
				forceLineSearch: forceLinesearch);
			return retVal;
		}

		public PerSecond MaxSpeed
		{
			get { return _maxSpeed ?? (_maxSpeed = _efficiencyMapMech2El.Entries.Select(x => x.Y).Max().SI<PerSecond>()); }
		}
	}
}