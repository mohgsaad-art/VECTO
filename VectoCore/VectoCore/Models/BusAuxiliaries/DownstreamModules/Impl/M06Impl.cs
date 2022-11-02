using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	public class M06Impl : AbstractModule, IM6
	{
		protected bool _overrunFlag;
		protected bool _smartElecAndPneumaticsCompressorFlag;
		protected Watt _smartElecAndPneumaticAltPowerGenAtCrank;
		protected Watt _smartElecAndPneumaticAirCompPowerGenAtCrank;
		protected Watt _smartElecOnlyAltPowerGenAtCrank;
		protected Watt _averagePowerDemandAtCrankFromPneumatics;
		protected Watt _smartPneumaticOnlyAirCompPowerGenAtCrank;
		protected Watt _avgPowerDemandAtCrankFromElectricsIncHVAC;
		protected bool _smartPneumaticsOnlyCompressorFlag;

		protected IM1_AverageHVACLoadDemand _m1;
		protected IM2_AverageElectricalLoadDemand _m2;
		protected IM3_AveragePneumaticLoadDemand _m3;
		protected IM4_AirCompressor _m4;
		protected IM5_SmartAlternatorSetGeneration _m5;
		protected ISignals _signals;
		private bool _smartElectrics;

		public M06Impl(IElectricsUserInputsConfig electricConfig, IM1_AverageHVACLoadDemand m1,
			IM2_AverageElectricalLoadDemand m2, IM3_AveragePneumaticLoadDemand m3, IM4_AirCompressor m4,
			IM5_SmartAlternatorSetGeneration m5, ISignals signals)
		{
			_m1 = m1;
			_m2 = m2;
			_m3 = m3;
			_m4 = m4;
			_m5 = m5;
			_signals = signals;
			_smartElectrics = electricConfig.AlternatorType == AlternatorType.Smart;
		}

		protected override void DoCalculate()
		{
			var sum1 = _m1.AveragePowerDemandAtCrankFromHVACElectrics + _m2.GetAveragePowerAtCrankFromElectrics();
			var sw1 = _smartElectrics
				? _m5.AlternatorsGenerationPowerAtCrankTractionOn()
				: sum1;
			var sum2 = _m1.AveragePowerDemandAtCrankFromHVACMechanicals + sw1 +
						_m3.GetAveragePowerDemandAtCrankFromPneumatics();
			//var sum3 = _signals.EngineMotoringPower + _signals.InternalEnginePower + sum2;
			var sum3 = _signals.ExcessiveDragPower + sum2;
			//VC0: prinzipiell reserve vorhanden - unter schleppkurve mit durchschnittlichen aux
			var vc0 = sum3.IsSmallerOrEqual(0); //sum3 <= 0;

			var sum4 = sum3 - sw1 - _m3.GetAveragePowerDemandAtCrankFromPneumatics() + _m4.GetPowerCompressorOff();
			var sum5 = vc0 ? sum4 : 0.SI<Watt>();
			var sum10 = _m5.AlternatorsGenerationPowerAtCrankOverrun() * -1.0;
			var max1 = sum5 > sum10 ? sum5 : sum10;
			var sum11 = sum5 - max1;
			var sum12 = _m4.GetPowerDifference() + sum11;

			// VC2: Smart Compressor Overrun and Smart Electrics overrun
			var vc2 = sum12.IsSmallerOrEqual(0); // sum12 < 0 || sum12.IsEqual(0);

			// VC1: Pneumatics Compressor Off and Smart Electrics
			//      compressor off power can be provided
			//      smart alternator can be provided (max possible)
			var vc1 = sum12.IsGreater(0); // sum12 > 0;
			var sum14 = vc1 ? _m4.GetPowerCompressorOff() : 0.SI<Watt>();
			var sum15 = vc2
				? _m4.GetPowerCompressorOn() * Constants.BusAuxiliaries.PneumaticUserConfig.PneumaticOverrunUtilisation
				: 0.SI<Watt>();
			var sum16 = sum14 + sum15;

			var sum6 = sum4 - _m4.GetPowerCompressorOff() + _m3.GetAveragePowerDemandAtCrankFromPneumatics();
			var sum7 = vc0 ? sum6 : 0.SI<Watt>();
			var max2 = sum7 > sum10 ? sum7 : sum10;

			var sum8 = sum4 + sw1;
			var sum9 = vc0 ? sum8 : 0.SI<Watt>();
			var sum13 = sum9 + _m4.GetPowerDifference();

			// VC3: Pneumatics compressor off, average elctric power (no overrun)
			var vc3 = sum13.IsGreater(0); //sum13 > 0;

			// VC4: smart compressor overrun and average electrics (no overrun)
			var vc4 = sum13.IsSmallerOrEqual(0); // sum13 < 0 || sum13.IsEqual(0);
			var sum17 = vc3 ? _m4.GetPowerCompressorOff() : 0.SI<Watt>();
			var sum18 = vc4
				? _m4.GetPowerCompressorOn() * Constants.BusAuxiliaries.PneumaticUserConfig.PneumaticOverrunUtilisation
				: 0.SI<Watt>();
			var sum19 = sum17 + sum18;

			_overrunFlag = vc0;
			_smartElecAndPneumaticsCompressorFlag = vc2;
			_smartElecAndPneumaticAltPowerGenAtCrank = max1 * -1;
			_smartElecAndPneumaticAirCompPowerGenAtCrank = sum16;
			_smartElecOnlyAltPowerGenAtCrank = max2 * -1;
			_averagePowerDemandAtCrankFromPneumatics = _m3.GetAveragePowerDemandAtCrankFromPneumatics();
			_smartPneumaticOnlyAirCompPowerGenAtCrank = sum19;
			_avgPowerDemandAtCrankFromElectricsIncHVAC = sum1;
			_smartPneumaticsOnlyCompressorFlag = vc4;
		}

		#region Implementation of IM6

		public bool OverrunFlag
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _overrunFlag;
			}
		}

		public bool SmartElecAndPneumaticsCompressorFlag
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElecAndPneumaticsCompressorFlag; }
		}

		public Watt SmartElecAndPneumaticAltPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElecAndPneumaticAltPowerGenAtCrank; }
		}

		public Watt SmartElecAndPneumaticAirCompPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElecAndPneumaticAirCompPowerGenAtCrank; }
		}

		public Watt SmartElecOnlyAltPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartElecOnlyAltPowerGenAtCrank; }
		}

		public Watt AveragePowerDemandAtCrankFromPneumatics
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _averagePowerDemandAtCrankFromPneumatics; }
		}

		public Watt SmartPneumaticOnlyAirCompPowerGenAtCrank
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartPneumaticOnlyAirCompPowerGenAtCrank; }
		}

		public Watt AvgPowerDemandAtCrankFromElectricsIncHVAC
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _avgPowerDemandAtCrankFromElectricsIncHVAC; }
		}

		public bool SmartPneumaticsOnlyCompressorFlag
		{
			get {
				if (!calculationValid) {
					Calculate();
				}
				return _smartPneumaticsOnlyCompressorFlag; }
		}

		#endregion
	}
}
