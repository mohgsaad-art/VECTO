using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data {
	public class DragCurve : IEquatable<DragCurve>
	{
		internal readonly List<DragLoadEntry> Entries;

		public DragCurve(List<DragLoadEntry> entries)
		{
			Entries = entries;
		}

		public override bool Equals(object obj) => Equals(obj as DragCurve);

        public override int GetHashCode()
        {
            return Entries.GetHashCode();
        }

        public bool Equals(DragCurve other)
		{
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (ReferenceEquals(other, null))
            {
                return false;
            }

			return Entries.SequenceEqual(other.Entries, new DragLoadEntryComparer());
        }

        public static bool operator ==(DragCurve a, DragCurve b)
		{
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(DragCurve a, DragCurve b) => !(a == b);

        public NewtonMeter Lookup(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Interpolate(
				Entries[idx - 1].MotorSpeed, Entries[idx].MotorSpeed, Entries[idx - 1].DragTorque, Entries[idx].DragTorque,
				angularVelocity);
		}

		protected int FindIndex(PerSecond angularVelocity)
		{
			if (angularVelocity < Entries.First().MotorSpeed) {
				return 1;
			}
			if (angularVelocity > Entries.Last().MotorSpeed) {
				return Entries.Count - 1;

			}
			for (var index = 1; index < Entries.Count; index++) {
				if (angularVelocity >= Entries[index - 1].MotorSpeed && angularVelocity <= Entries[index].MotorSpeed) {
					return index;
				}
			}
			throw new VectoException("angular velocity {0} exceeds drag curve. min: {1} max: {2}", angularVelocity, Entries.First().MotorSpeed, Entries.Last().MotorSpeed);

		}

		public class DragLoadEntry
		{
			public PerSecond MotorSpeed { get; set; }

			public NewtonMeter DragTorque { get; set; }
		}

		public class DragLoadEntryComparer : IEqualityComparer<DragLoadEntry>
		{
            public bool Equals(DragLoadEntry x, DragLoadEntry y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
                {
                    return false;
                }

                return (x.MotorSpeed == y.MotorSpeed) && (x.DragTorque == y.DragTorque);
            }

            public int GetHashCode(DragLoadEntry obj)
            {
                return (obj is null) ? 0 : (obj.MotorSpeed, obj.DragTorque).GetHashCode();
            }
        }

		public string[] SerializedEntries
		{
			get { return Entries.Select(x => $"{x.MotorSpeed.AsRPM} {x.DragTorque}").ToArray(); }
		}
	}
}