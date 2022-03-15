using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class M04Impl : AbstractModule, IM4_AirCompressor
	{
		private const double MinRatio = 1;
		private const double MaxRatio = 10;
		private const double MinEff = 0;
		private const double MaxEff = 1;

		private double _pulleyGearRatio;
		private double _pulleyGearEfficiency;
		private ICompressorMap _map;
		private ISignals _signals;

		public M04Impl(ICompressorMap map, double pulleyGearRatio, double pulleyGearEfficiency, ISignals signals)
		{
			_map = map;
			_pulleyGearRatio = pulleyGearRatio;
			_pulleyGearEfficiency = pulleyGearEfficiency;
			_signals = signals;
		}

		#region Implementation of IM4_AirCompressor

		public double PulleyGearRatio
		{
			get => _pulleyGearRatio;
			set {
				if (value < MinRatio || value > MaxRatio) {
					throw new ArgumentOutOfRangeException(
						"pulleyGearRatio", value,
						$"Invalid value, should be in the range {MinRatio} to {MaxRatio}");
				}

				_pulleyGearRatio = value;
			}
		}

		public double PulleyGearEfficiency
		{
			get => _pulleyGearEfficiency;
			set {
				if (value < MinEff || value > MaxEff) {
					throw new ArgumentOutOfRangeException(
						"pulleyGearEfficiency", value,
						$"Invalid value, should be in the range {MinEff} to {MaxEff}"
					);
				}

				_pulleyGearEfficiency = value;
			}
		}

		public NormLiterPerSecond GetFlowRate()
		{
			return _map.Interpolate(_signals.EngineSpeed * PulleyGearRatio).FlowRate;
		}

		public Watt GetPowerCompressorOff()
		{
			return GetCompressorPower(false);
		}

		public Watt GetPowerCompressorOn()
		{
			return GetCompressorPower(true);
		}

		public Watt GetPowerDifference()
		{
			var powerOn = GetPowerCompressorOn();
			var powerOff = GetPowerCompressorOff();
			return powerOn - powerOff;
		}

		public JoulePerNormLiter GetAveragePowerDemandPerCompressorUnitFlowRate()
		{
			return _map.GetAveragePowerDemandPerCompressorUnitFlowRate();
		}

		#endregion

		private Watt GetCompressorPower(bool compressorOn)
		{
			
			var rslt = _map.Interpolate(_signals.EngineSpeed * PulleyGearRatio);
			return compressorOn ? rslt.PowerOn : rslt.PowerOff;
		}
	}
}
