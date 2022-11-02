using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class VehicleMaxPropulsionTorque
	{
		internal readonly IList<FullLoadEntry> FullLoadEntries;

		internal VehicleMaxPropulsionTorque(IList<FullLoadEntry> entries)
		{
			FullLoadEntries = entries;
		}


		public NewtonMeter FullLoadDriveTorque(PerSecond angularVelocity)
		{
			var idx = FindIndex(angularVelocity);
			return VectoMath.Max(0.SI<NewtonMeter>(), VectoMath.Interpolate(FullLoadEntries[idx - 1].MotorSpeed, FullLoadEntries[idx].MotorSpeed,
				FullLoadEntries[idx - 1].FullDriveTorque, FullLoadEntries[idx].FullDriveTorque,
				angularVelocity));
		}

		

		protected int FindIndex(PerSecond angularVelocity)
		{
			if (angularVelocity < FullLoadEntries.First().MotorSpeed) {
				return 1;
			}
			if (angularVelocity > FullLoadEntries.Last().MotorSpeed) {
				return FullLoadEntries.Count - 1;

			}
			for (var index = 1; index < FullLoadEntries.Count; index++) {
				if (angularVelocity >= FullLoadEntries[index - 1].MotorSpeed && angularVelocity <= FullLoadEntries[index].MotorSpeed) {
					return index;
				}
			}
			throw new VectoException("angular velocity {0} exceeds max-torque curve. min: {1} max: {2}", angularVelocity, FullLoadEntries.First().MotorSpeed, FullLoadEntries.Last().MotorSpeed);

		}

		[DebuggerDisplay("{MotorSpeed.AsRPM}: {FullDriveTorque}")]
		internal class FullLoadEntry
		{
			public PerSecond MotorSpeed { get; set; }

			public NewtonMeter FullDriveTorque { get; set; }

		}

		public string[] SerializedEntries {
			get { return FullLoadEntries.Select(x => $"{x.MotorSpeed.AsRPM} {x.FullDriveTorque}").ToArray(); }
		}
	}
}