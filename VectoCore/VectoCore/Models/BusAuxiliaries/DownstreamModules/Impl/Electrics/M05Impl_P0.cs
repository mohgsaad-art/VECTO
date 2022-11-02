using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M05Impl_P0 : IM5_SmartAlternatorSetGeneration
	{
		private IM2_AverageElectricalLoadDemand _m02;
		private IM1_AverageHVACLoadDemand _m01;
		private ISimpleBatteryInfo _bat;
		private IM0_NonSmart_AlternatorsSetEfficiency _m00;
		private ISignals _signals;
		private double _alternatorGearEfficiency;
		private Watt _maxAlternatorPower;

		public M05Impl_P0(IM0_NonSmart_AlternatorsSetEfficiency m0, IM1_AverageHVACLoadDemand m01, IM2_AverageElectricalLoadDemand m02, ISimpleBatteryInfo bat, IElectricsUserInputsConfig elCfg, ISignals signals)
		{
			_m00 = m0;
			_m01 = m01;
			_m02 = m02;
			_bat = bat;
			_maxAlternatorPower =
				elCfg.AlternatorType == AlternatorType.Smart ? elCfg.MaxAlternatorPower : 0.SI<Watt>();
			_alternatorGearEfficiency = elCfg.AlternatorGearEfficiency;
			_signals = signals;
		}

		#region Implementation of IAbstractModule

		public void ResetCalculations()
		{
			
		}

		#endregion

		#region Implementation of IM5_SmartAlternatorSetGeneration

		public Watt AlternatorsGenerationPowerAtCrankIdle()
		{
			return (_m02.GetAveragePowerAtCrankFromElectrics() + _m01.AveragePowerDemandAtCrankFromHVACElectrics).LimitTo(
				0.SI<Watt>(), MaxAlternatorPowerAtCrank);
		}

		public Watt AlternatorsGenerationPowerAtCrankTractionOn()
		{
			return (_m02.GetAveragePowerAtCrankFromElectrics() + _m01.AveragePowerDemandAtCrankFromHVACElectrics).LimitTo(
				0.SI<Watt>(), MaxAlternatorPowerAtCrank);
		}

		public Watt AlternatorsGenerationPowerAtCrankOverrun()
		{
			return (_maxAlternatorPower / _m00.AlternatorsEfficiency / _alternatorGearEfficiency).LimitTo(
				0.SI<Watt>(), MaxAlternatorPowerAtCrank);
		}

		#endregion

		private Watt MaxAlternatorPowerAtCrank
		{
			get {
				var electricConsumer = _m01.AveragePowerDemandAtAlternatorFromHVACElectrics +
										_m02.AveragePowerDemandAtAlternatorFromElectrics;
				var maxBatChargePower = (1 - _bat.SOC) * _bat.Capacity / _signals.SimulationInterval + electricConsumer;

				return maxBatChargePower.LimitTo(0.SI<Watt>(), _maxAlternatorPower) / _m00.AlternatorsEfficiency /
						_alternatorGearEfficiency;
			}
		}
	}
}
