using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public class M08Impl : AbstractModule, IM8
	{
		protected Watt _auxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries;
		protected Watt _smartElectricalAlternatorPowerGenAtCrank;
		protected bool _compressorFlag;

		protected readonly IM1_AverageHVACLoadDemand _m1;
		protected readonly IM6 _m6;
		protected readonly IM7 _m7;
		protected readonly ISignals _signals;
		private bool _smartElectrics;
		private bool _smartPneumatics;

		public M08Impl(IAuxiliaryConfig auxCfg, IM1_AverageHVACLoadDemand m1, IM6 m6, IM7 m7, ISignals signals)
		{
			_m1 = m1;
			_m6 = m6;
			_m7 = m7;
			_signals = signals;
			_smartElectrics = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart;
			_smartPneumatics = auxCfg.PneumaticUserInputsConfig.SmartAirCompression;
		}


		protected override void DoCalculate()
		{
			var sum1 = _m7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank +
						_m7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank;
			var sum2 = _m7.SmartElectricalOnlyAuxAltPowerGenAtCrank + _m6.AveragePowerDemandAtCrankFromPneumatics;
			var sum3 = _m7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank + _m6.AvgPowerDemandAtCrankFromElectricsIncHVAC;
			var sum4 = _m6.AvgPowerDemandAtCrankFromElectricsIncHVAC + _m6.AveragePowerDemandAtCrankFromPneumatics;
			var sw1 = _smartPneumatics ? sum1 : sum2;
			var sw2 = _smartPneumatics ? sum3 : sum4;
			var sw5 = _smartElectrics ? sw1 : sw2;
			var sw6 = !_signals.EngineStopped;
			var sum5 = _m1.AveragePowerDemandAtCrankFromHVACMechanicals + sw5;
			var sum6 = sum5; // sw6 ? sum5 : 0.SI<Watt>();

			var sw3 = _smartPneumatics ? _m7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank :
				_m7.SmartElectricalOnlyAuxAltPowerGenAtCrank;

			var sw4 = _smartElectrics ? _m6.SmartElecAndPneumaticsCompressorFlag : _m6.SmartPneumaticsOnlyCompressorFlag;

			_auxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries = sum6;
			_smartElectricalAlternatorPowerGenAtCrank = sw3;
			_compressorFlag = sw4;
		}

		#region Implementation of IM8

		public Watt AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _auxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries; }
		}

		public Watt SmartElectricalAlternatorPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElectricalAlternatorPowerGenAtCrank; }
		}

		public bool CompressorFlag
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _compressorFlag; }
		}

		#endregion
	}
}
