using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	[DebuggerDisplay("{Name}")]
	public class GearshiftPosition
	{
		public uint Gear { get; }
		public bool? TorqueConverterLocked { get; }

		public GearshiftPosition(uint gear, bool? torqueConverterLocked = null)
		{
			Gear = gear;
			TorqueConverterLocked = torqueConverterLocked;
		}

		public override string ToString()
		{
			return Name;
		}

		public string Name => $"{Gear}{(Gear == 0 ? "" : (TorqueConverterLocked.HasValue ? (TorqueConverterLocked.Value ? "L" : "C") : ""))}";

		public bool Engaged => Gear != 0;

		public override bool Equals(object x)
		{
			var other = x as GearshiftPosition;
			if (other == null)
				return false;

			return other.Gear == Gear && other.TorqueConverterLocked == TorqueConverterLocked;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public static bool operator >(GearshiftPosition p1, GearshiftPosition p2)
		{
			if (p1.Gear > p2.Gear) {
				return true;
			}

			if (p1.Gear != p2.Gear) {
				return false;
			}

			if (!p1.TorqueConverterLocked.HasValue || !p2.TorqueConverterLocked.HasValue) {
				return false;
			}

			return p1.TorqueConverterLocked.Value && !p2.TorqueConverterLocked.Value;
		}

		public static bool operator <(GearshiftPosition p1, GearshiftPosition p2)
		{
			if (p1.Gear < p2.Gear) {
				return true;
			}

			if (p1.Gear != p2.Gear) {
				return false;
			}

			if (!p1.TorqueConverterLocked.HasValue || !p2.TorqueConverterLocked.HasValue) {
				return false;
			}

			return  p2.TorqueConverterLocked.Value && !p1.TorqueConverterLocked.Value;
		}


		public bool IsLockedGear()
		{
			return !TorqueConverterLocked.HasValue || TorqueConverterLocked.Value;
		}
	}

	public class GearList :IEnumerable<GearshiftPosition>
	{
		protected GearshiftPosition[] Entries;

		public GearList(GearshiftPosition[] gearList)
		{
			Entries = gearList;
		}

		public bool HasPredecessor(GearshiftPosition cur)
		{
			var idx = Array.IndexOf(Entries, cur);
			return idx > 0;
		}


		public GearshiftPosition Predecessor(GearshiftPosition cur)
		{
			var idx = Array.IndexOf(Entries, cur);
			
			return idx <= 0 ? null : Entries[idx - 1];
		}

		public bool HasSuccessor(GearshiftPosition cur)
        {
			var idx = Array.IndexOf(Entries, cur);
			return idx < Entries.Length - 1;
		}

		public GearshiftPosition Successor(GearshiftPosition cur)
		{
			var idx = Array.IndexOf(Entries, cur);

			return idx < 0 || idx >= Entries.Length - 1 ? null : Entries[idx + 1];
		}

		public GearshiftPosition Successor(GearshiftPosition cur, uint numUp)
		{
			var idx = Array.IndexOf(Entries, cur);

			if (idx < 0) {
				return null;
			}

			var next = idx + numUp;

			return next >= Entries.Length ? Entries.Last() : Entries[next];
		}

		public GearshiftPosition Predecessor(GearshiftPosition cur, uint numDown)
		{
			var idx = Array.IndexOf(Entries, cur);

			if (idx < 0) {
				return null;
			}

			var next = idx - numDown;

			return next < 0 ? Entries.First() : Entries[next];
		}

		public IEnumerator<GearshiftPosition> GetEnumerator()
		{
			foreach (var entry in Entries) {
				yield return entry;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Distance(GearshiftPosition from, GearshiftPosition to)
		{
			var startIdx = Array.IndexOf(Entries, from);
			var endIdx = Array.IndexOf(Entries, to);
			return startIdx - endIdx;
		}


		public IEnumerable<GearshiftPosition> IterateGears(GearshiftPosition from, GearshiftPosition to)
		{
			var startIdx = Array.IndexOf(Entries, from);
			var endIdx = Array.IndexOf(Entries, to);

			if (endIdx > startIdx) {
				for (var i = startIdx; i <= endIdx; i++) {
					yield return Entries[i];
				}
			} else {
				for (var i = startIdx; i >= endIdx; i--) {
					yield return Entries[i];
				}
			}
		}
	}
}