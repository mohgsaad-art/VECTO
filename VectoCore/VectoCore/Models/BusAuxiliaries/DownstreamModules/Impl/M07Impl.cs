using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public class M07Impl : AbstractModule, IM7
	{
		protected Watt _smartElectricalAndPneumaticAuxAltPowerGenAtCrank;
		protected Watt _smartElectricalAndPneumaticAuxAirCompPowerGenAtCrank;
		protected Watt _smartElectricalOnlyAuxAltPowerGenAtCrank;
		protected Watt _smartPneumaticOnlyAuxAirCompPowerGenAtCrank;

		protected IM5_SmartAlternatorSetGeneration _m5;
		protected IM6 _m6;
		protected readonly ISignals _signals;
		private ISimpleBatteryInfo _bat;
		private IM2_AverageElectricalLoadDemand _m2;
		private IM1_AverageHVACLoadDemand _m1;
		private IM0_NonSmart_AlternatorsSetEfficiency _m0;
		private double _alternatorGearEfficiency;

		public M07Impl(IM0_NonSmart_AlternatorsSetEfficiency m0, IM1_AverageHVACLoadDemand m1,
			IM2_AverageElectricalLoadDemand m2, IM5_SmartAlternatorSetGeneration m5, IM6 m6, ISimpleBatteryInfo bat,
			double alternatorGearEfficiency, ISignals signals)
		{
			_m5 = m5;
			_m6 = m6;
			_signals = signals;
			_bat = bat;
			_m0 = m0;
			_m1 = m1;
			_m2 = m2;
			_alternatorGearEfficiency = alternatorGearEfficiency;
		}

		protected override void DoCalculate()
		{
			//var idle = _signals.EngineSpeed <= _signals.EngineIdleSpeed &&
			//			(!_signals.ClutchEngaged || _signals.InNeutral);

			//var sw1 = idle
			//	? _m5.AlternatorsGenerationPowerAtCrankIdle()
			//	: _m5.AlternatorsGenerationPowerAtCrankTractionOn();

			var maxBatPower = _bat.SOC * _bat.Capacity / _signals.SimulationInterval;
			var elConsumerPower = _m1.AveragePowerDemandAtAlternatorFromHVACElectrics +
								_m2.AveragePowerDemandAtAlternatorFromElectrics;

			var sw1 = maxBatPower > elConsumerPower
				? 0.SI<Watt>()
				: (elConsumerPower - maxBatPower) / _m0.AlternatorsEfficiency / _alternatorGearEfficiency;

			var c1 = _m6.OverrunFlag && _signals.ClutchEngaged && _signals.InNeutral == false;
			var sw2 = c1 ? _m6.SmartElecAndPneumaticAltPowerGenAtCrank : sw1;

			var sw3 = c1 ? _m6.SmartElecAndPneumaticAirCompPowerGenAtCrank : _m6.AveragePowerDemandAtCrankFromPneumatics;

			var sw4 = c1 ? _m6.SmartElecOnlyAltPowerGenAtCrank : sw1;

			var sw5 = c1 ? _m6.SmartPneumaticOnlyAirCompPowerGenAtCrank : _m6.AveragePowerDemandAtCrankFromPneumatics;

			_smartElectricalAndPneumaticAuxAltPowerGenAtCrank = sw2;
			_smartElectricalAndPneumaticAuxAirCompPowerGenAtCrank = sw3;
			_smartElectricalOnlyAuxAltPowerGenAtCrank = sw4;
			_smartPneumaticOnlyAuxAirCompPowerGenAtCrank = sw5;
		}

		#region Implementation of IM7

		public Watt SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElectricalAndPneumaticAuxAltPowerGenAtCrank;
			}
		}

		public Watt SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElectricalAndPneumaticAuxAirCompPowerGenAtCrank;
			}
		}

		public Watt SmartElectricalOnlyAuxAltPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElectricalOnlyAuxAltPowerGenAtCrank;
			}
		}

		public Watt SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartPneumaticOnlyAuxAirCompPowerGenAtCrank;
			}
		}

		#endregion
	}
}
