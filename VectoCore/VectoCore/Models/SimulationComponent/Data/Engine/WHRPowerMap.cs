using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.ComponentData
{
	public class WHRPowerMap
	{
		protected readonly DelaunayMap WHRMap;

		public WHRPowerMap(DelaunayMap whrMap)
		{
			WHRMap = whrMap;
		}

		public string Name => WHRMap.Name;

		public class Entry
		{
			[Required, SIRange(0, 5000 * Constants.RPMToRad)] public readonly PerSecond EngineSpeed;
			[Required] public readonly NewtonMeter Torque;
			[Required, SIRange(0, double.MaxValue)] public readonly Watt ElectricPower;

			public Entry(PerSecond engineSpeed, NewtonMeter torque, Watt electricPower)
			{
				EngineSpeed = engineSpeed;
				Torque = torque;
				ElectricPower = electricPower;
			}
		}

		public WHRPowerResult GetWHRPower(NewtonMeter torque, PerSecond engineSpeed, bool allowExtrapolation)
		{
			var result = new WHRPowerResult();
			// delaunay map needs is initialised with rpm, therefore the angularVelocity has to be converted.
			var value = WHRMap.Interpolate(torque, engineSpeed);
			if (!value.IsNaN()) {
				result.GeneratedPower = value.SI<Watt>();
				return result;
			}

			if (allowExtrapolation) {
				result.GeneratedPower =
					WHRMap.Extrapolate(torque, engineSpeed).SI<Watt>();
				result.Extrapolated = true;
				return result;
			}

			throw new VectoException("WHR Map: Interpolation failed. torque: {0}, n: {1}", torque.Value(),
									engineSpeed.AsRPM);
		}
	}

	public class WHRPowerResult
	{
		public Watt GeneratedPower;
		public bool Extrapolated;

	}
}