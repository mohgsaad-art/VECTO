using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data {
	public class DragCurve
	{
		internal readonly List<DragLoadEntry> Entries;

		public DragCurve(List<DragLoadEntry> entries)
		{
			Entries = entries;
		}

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

		public string[] SerializedEntries
		{
			get { return Entries.Select(x => $"{x.MotorSpeed.AsRPM} {x.DragTorque}").ToArray(); }
		}
	}
}